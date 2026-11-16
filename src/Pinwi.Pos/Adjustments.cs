namespace Pinwi.Pos;

public record CashMovement(long Id,string Date,string Cashier,long Amount,string Reason)
{
 public decimal Pesos=>Amount/100m;
 public string DateLabel=>TimeZoneInfo.ConvertTime(DateTimeOffset.Parse(Date),TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time (Mexico)")).ToString("dd/MM/yyyy HH:mm");
}
public record ReturnLine(int ProductId,string Name,int Sold,int Returned,long Price,long Discount)
{
 public int Available=>Sold-Returned;
 public decimal OriginalNet=>(Price*Sold-Discount)/100m;
}
public record ReturnRequest(int ProductId,int Quantity,bool Restock=false);
public record RefundQuote(long Total,long Cash,long Card,string Detail);
public record RefundResult(long Id,long Total,long Cash,long Card,string Receipt);
public sealed partial class Store
{
 static long ExpectedCash(Microsoft.Data.Sqlite.SqliteConnection c,long shift)=>Convert.ToInt64(Cmd(c,"""
 SELECT (SELECT fund FROM shifts WHERE id=@p0)
  +COALESCE((SELECT SUM(cash) FROM sales WHERE shift=@p0),0)
  -COALESCE((SELECT SUM(amount) FROM expenses WHERE shift=@p0 AND source='Caja'),0)
  +COALESCE((SELECT SUM(amount) FROM cash_movements WHERE shift=@p0),0)
  -COALESCE((SELECT SUM(cash) FROM refunds WHERE shift=@p0),0)
 """,shift).ExecuteScalar());
 public long AddCashMovement(long amount,string reason,string token,string password)
 {
  RequireAdmin(password);reason=reason.Trim();
  if(amount==0||amount is >10000000000 or < -10000000000||reason.Length<3||reason.Length>240||reason.Any(char.IsControl)||string.IsNullOrWhiteSpace(token))throw new Exception("Captura un movimiento distinto de cero y su motivo.");
  using var c=Open();using var t=c.BeginTransaction();
  var existing=Cmd(c,"SELECT id FROM cash_movements WHERE token=@p0",token).ExecuteScalar();if(existing!=null)return Convert.ToInt64(existing);
  var shift=Convert.ToInt64(Cmd(c,"SELECT id FROM shifts WHERE closed IS NULL").ExecuteScalar()??throw new Exception("Abre caja antes de registrar el movimiento."));
  if(amount<0&&ExpectedCash(c,shift)+amount<0)throw new Exception("El retiro supera el efectivo esperado.");
  var now=DateTimeOffset.Now.ToString("O");Cmd(c,"INSERT INTO cash_movements(token,date,shift,cashier,amount,reason) VALUES(@p0,@p1,@p2,@p3,@p4,@p5)",token,now,shift,CurrentUserName!,amount,reason).ExecuteNonQuery();
  var id=Convert.ToInt64(Cmd(c,"SELECT last_insert_rowid()").ExecuteScalar());
  Cmd(c,"INSERT INTO audit(date,action) VALUES(@p0,@p1)",now,$"{CurrentUserName}: movimiento de caja #{id}, {amount} centavos. {reason}").ExecuteNonQuery();t.Commit();NotifyChanged();return id;
 }
 public List<CashMovement> CashMovements()
 {
  using var c=Open();using var r=Cmd(c,"SELECT id,date,cashier,amount,reason FROM cash_movements ORDER BY id DESC LIMIT 500").ExecuteReader();var list=new List<CashMovement>();while(r.Read())list.Add(new(r.GetInt64(0),r.GetString(1),r.GetString(2),r.GetInt64(3),r.GetString(4)));return list;
 }
 public List<ReturnLine> ReturnableLines(long sale)
 {
  using var c=Open();return ReturnableLines(c,sale);
 }
 static List<ReturnLine> ReturnableLines(Microsoft.Data.Sqlite.SqliteConnection c,long sale)
 {
  using var r=Cmd(c,"SELECT l.product,l.name,l.quantity,COALESCE((SELECT SUM(rl.quantity) FROM refund_lines rl JOIN refunds r ON r.id=rl.refund WHERE r.sale=l.sale AND rl.product=l.product),0),l.price,l.discount FROM lines l WHERE l.sale=@p0 ORDER BY l.product",sale).ExecuteReader();
  var result=new List<ReturnLine>();while(r.Read())result.Add(new(r.GetInt32(0),r.GetString(1),r.GetInt32(2),r.GetInt32(3),r.GetInt64(4),r.GetInt64(5)));return result;
 }
 public RefundQuote QuoteRefund(long sale,List<ReturnRequest> request,bool cancel=false)
 {
  if(CurrentUserName==null)throw new Exception("Inicia sesión.");using var c=Open();using var t=c.BeginTransaction(deferred:true);return BuildRefundQuote(c,sale,request,cancel).Summary;
 }
 static (RefundQuote Summary,List<ReturnLine> Lines,List<(ReturnRequest Request,long Amount)> Amounts) BuildRefundQuote(Microsoft.Data.Sqlite.SqliteConnection c,long sale,List<ReturnRequest> request,bool cancel)
 {
  long originalTotal,originalCard;
  using(var r=Cmd(c,"SELECT total,card FROM sales WHERE id=@p0",sale).ExecuteReader()){if(!r.Read())throw new Exception("Venta no encontrada.");originalTotal=r.GetInt64(0);originalCard=r.GetInt64(1);}
  var lines=ReturnableLines(c,sale);
  if(cancel)
  {
   if(lines.Any(x=>x.Returned>0))throw new Exception("La venta ya tiene devoluciones. Devuelve las unidades restantes.");
   // Cancellation never assumes the goods came back into inventory.
   request=lines.Select(x=>new ReturnRequest(x.ProductId,x.Sold)).ToList();
  }
  if(request.Count==0||request.Select(x=>x.ProductId).Distinct().Count()!=request.Count)throw new Exception("Selecciona partidas sin duplicados.");
  static long Portion(long net,int count,int quantity)=>(long)decimal.Round((decimal)net*count/quantity,0,MidpointRounding.AwayFromZero);
  var amounts=new List<(ReturnRequest Request,long Amount)>();
  foreach(var item in request)
  {
   var line=lines.FirstOrDefault(x=>x.ProductId==item.ProductId)??throw new Exception("La partida no pertenece a la venta.");
   if(item.Quantity<=0||item.Quantity>line.Available)throw new Exception("La cantidad supera las unidades pendientes de devolución.");
   var net=checked(line.Price*line.Sold-line.Discount);
   amounts.Add((item,Portion(net,line.Returned+item.Quantity,line.Sold)-Portion(net,line.Returned,line.Sold)));
  }
  var total=amounts.Sum(x=>x.Amount);
  var previousTotal=Convert.ToInt64(Cmd(c,"SELECT COALESCE(SUM(total),0) FROM refunds WHERE sale=@p0",sale).ExecuteScalar());
  var previousCard=Convert.ToInt64(Cmd(c,"SELECT COALESCE(SUM(card),0) FROM refunds WHERE sale=@p0",sale).ExecuteScalar());
  var card=originalTotal==0?0:(long)decimal.Round((decimal)originalCard*(previousTotal+total)/originalTotal,0,MidpointRounding.AwayFromZero)-previousCard;
  var cash=total-card;
  var detail=string.Join("\n",amounts.Select(x=>$"{lines.Single(l=>l.ProductId==x.Request.ProductId).Name}: {x.Request.Quantity} piezas · {x.Amount/100m:C}"));
  return(new(total,cash,card,detail),lines,amounts);
 }
 public RefundResult Refund(long sale,List<ReturnRequest> request,string reason,string password,string token,bool cancel=false,bool cardConfirmed=false)
 {
  var authorizer=AdminAuthorizer(password);if(CurrentUserName==null||authorizer==null)throw new Exception("Autoriza la devolución con la contraseña del administrador.");
  reason=reason.Trim();if(reason.Length<3||reason.Length>240||reason.Any(char.IsControl)||string.IsNullOrWhiteSpace(token))throw new Exception("Captura el motivo de la devolución.");
  using var c=Open();using var t=c.BeginTransaction();
  using(var r=Cmd(c,"SELECT id,total,cash,card,receipt,sale FROM refunds WHERE token=@p0",token).ExecuteReader())if(r.Read())
  {if(r.GetInt64(5)!=sale)throw new Exception("Ese identificador corresponde a otra devolución.");return new(r.GetInt64(0),r.GetInt64(1),r.GetInt64(2),r.GetInt64(3),r.GetString(4));}
  var shift=Convert.ToInt64(Cmd(c,"SELECT id FROM shifts WHERE closed IS NULL").ExecuteScalar()??throw new Exception("Abre caja antes de registrar una devolución."));
  var quote=BuildRefundQuote(c,sale,request,cancel);var total=quote.Summary.Total;var cash=quote.Summary.Cash;var card=quote.Summary.Card;var lines=quote.Lines;var amounts=quote.Amounts;
  if(card>0&&!cardConfirmed)throw new Exception("Confirma la devolución en la terminal externa antes de registrarla.");
  if(cash>ExpectedCash(c,shift))throw new Exception("No hay efectivo suficiente en caja para devolver ese importe.");
  var now=DateTimeOffset.Now.ToString("O");
  Cmd(c,"INSERT INTO refunds(token,sale,shift,date,cashier,authorizer,reason,total,cash,card,kind,receipt) VALUES(@p0,@p1,@p2,@p3,@p4,@p5,@p6,@p7,@p8,@p9,@p10,'')",token,sale,shift,now,CurrentUserName,authorizer,reason,total,cash,card,cancel?"Cancelación":"Devolución").ExecuteNonQuery();
  var id=Convert.ToInt64(Cmd(c,"SELECT last_insert_rowid()").ExecuteScalar());
  foreach(var item in amounts)
  {
   var restock=item.Request.Restock&&Cmd(c,"SELECT 1 FROM quick_products WHERE product=@p0",item.Request.ProductId).ExecuteScalar()==null&&Cmd(c,"SELECT 1 FROM products WHERE id=@p0 AND category='Paquetes'",item.Request.ProductId).ExecuteScalar()==null;
   Cmd(c,"INSERT INTO refund_lines(refund,product,quantity,amount,restocked) VALUES(@p0,@p1,@p2,@p3,@p4)",id,item.Request.ProductId,item.Request.Quantity,item.Amount,restock?item.Request.Quantity:0).ExecuteNonQuery();
   if(restock)Cmd(c,"UPDATE products SET stock=stock+@p1 WHERE id=@p0",item.Request.ProductId,item.Request.Quantity).ExecuteNonQuery();
  }
  var text=$"{(cancel?"CANCELACIÓN":"DEVOLUCIÓN")} #{id}\nVenta original: {sale}\nFecha: {now}\nUsuario: {CurrentUserName}\nAutorizó: {authorizer}\nMotivo: {reason}\n\n"+string.Join("\n",amounts.Select(x=>$"{lines.Single(l=>l.ProductId==x.Request.ProductId).Name}\n{x.Request.Quantity} piezas · ${x.Amount/100m:0.00}"))+$"\n\nTOTAL: ${total/100m:0.00}\nEfectivo devuelto: ${cash/100m:0.00}\nTarjeta externa: ${card/100m:0.00}";
  text=ReceiptBrand.From(GetSettings()).Wrap(text);
  Cmd(c,"UPDATE refunds SET receipt=@p1 WHERE id=@p0",id,text).ExecuteNonQuery();
  Cmd(c,"INSERT INTO audit(date,action) VALUES(@p0,@p1)",now,$"{CurrentUserName}: devolución #{id}, venta {sale}, {total} centavos. Autorizó {authorizer}. {reason}").ExecuteNonQuery();t.Commit();NotifyChanged();return new(id,total,cash,card,text);
 }
}
