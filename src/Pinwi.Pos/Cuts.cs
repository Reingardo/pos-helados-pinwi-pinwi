using System.Globalization;
using System.IO;
using System.Text;
namespace Pinwi.Pos;
public record CashCut(long Id,string Opened,string Closed,long Fund,long Cash,long Card,long Counted,string Cashier,string Summary,string Csv,string Status,long CashExpenses=0,long AllExpenses=0,string ExpensesCsv="",long Movements=0,long RefundCash=0,long RefundCard=0,string AdjustmentsCsv="")
{
 public decimal Total => (Cash+Card-RefundCash-RefundCard)/100m;
 public decimal Expected => (Fund+Cash-CashExpenses+Movements-RefundCash)/100m;
 public decimal ExpensesPesos=>AllExpenses/100m;
 public decimal NetAfterExpenses=>(Cash+Card-RefundCash-RefundCard-AllExpenses)/100m;
 public decimal CountedPesos=>Counted/100m;
 public decimal Difference=>(Counted-Fund-Cash+CashExpenses-Movements+RefundCash)/100m;
 public string ClosedLabel=>TimeZoneInfo.ConvertTime(DateTimeOffset.Parse(Closed),TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time (Mexico)")).ToString("dd/MM/yyyy HH:mm");
}
public sealed partial class Store
{
 public long CloseShift(long counted)
 {
  if(counted<0)throw new Exception("Conteo inválido.");if(CurrentUserName==null)throw new Exception("Inicia sesión.");
  using var c=Open();using var t=c.BeginTransaction();
  var id=Convert.ToInt64(Cmd(c,"SELECT id FROM shifts WHERE closed IS NULL").ExecuteScalar()??throw new Exception("No hay turno abierto."));
  var closed=DateTimeOffset.Now.ToString("O");Cmd(c,"UPDATE shifts SET closed=@p0,counted=@p1 WHERE id=@p2 AND closed IS NULL",closed,counted,id).ExecuteNonQuery();
  var fund=Convert.ToInt64(Cmd(c,"SELECT fund FROM shifts WHERE id=@p0",id).ExecuteScalar());
  var cash=Convert.ToInt64(Cmd(c,"SELECT COALESCE(SUM(cash),0) FROM sales WHERE shift=@p0",id).ExecuteScalar());
  var card=Convert.ToInt64(Cmd(c,"SELECT COALESCE(SUM(card),0) FROM sales WHERE shift=@p0",id).ExecuteScalar());
  var zone=TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time (Mexico)");var businessNow=TimeZoneInfo.ConvertTime(DateTimeOffset.Now,zone);var day=businessNow.Date;
  var start=new DateTimeOffset(day,zone.GetUtcOffset(day));var end=new DateTimeOffset(day.AddDays(1),zone.GetUtcOffset(day.AddDays(1)));
  var rows=new List<(long Id,string Date,string User,long Total,long Cash,long Card)>();
  using(var r=Cmd(c,"SELECT id,date,cashier,total,cash,card FROM sales WHERE julianday(date)>=julianday(@p0) AND julianday(date)<julianday(@p1) ORDER BY id",start.ToString("O"),end.ToString("O")).ExecuteReader())while(r.Read())rows.Add((r.GetInt64(0),r.GetString(1),r.GetString(2),r.GetInt64(3),r.GetInt64(4),r.GetInt64(5)));
  static string Money(long x)=>(x/100m).ToString("C",CultureInfo.GetCultureInfo("es-MX"));
  var expenses=new List<Expense>();
  using(var r=Cmd(c,"SELECT id,date,shift,cashier,concept,recipient,amount,source FROM expenses WHERE julianday(date)>=julianday(@p0) AND julianday(date)<julianday(@p1) ORDER BY id",start.ToString("O"),end.ToString("O")).ExecuteReader())while(r.Read())expenses.Add(new(r.GetInt64(0),r.GetString(1),r.GetInt64(2),r.GetString(3),r.GetString(4),r.GetString(5),r.GetInt64(6),r.GetString(7)));
  var shiftExpenses=Convert.ToInt64(Cmd(c,"SELECT COALESCE(SUM(amount),0) FROM expenses WHERE shift=@p0",id).ExecuteScalar());
  var cashExpenses=Convert.ToInt64(Cmd(c,"SELECT COALESCE(SUM(amount),0) FROM expenses WHERE shift=@p0 AND source='Caja'",id).ExecuteScalar());
  static string Plain(string x)=>x.Replace("*","").Replace("_","").Replace("~","").Replace("`","");
  var expenseDetail=expenses.Count==0?"Sin gastos registrados.":string.Join("\n",expenses.Select((x,i)=>$"{i+1}. {Plain(x.Concept)}{(string.IsNullOrWhiteSpace(x.Recipient)?"":" · "+Plain(x.Recipient))}\n   {Money(x.Amount)} · {x.SourceLabel}"));
  var movement=Convert.ToInt64(Cmd(c,"SELECT COALESCE(SUM(amount),0) FROM cash_movements WHERE shift=@p0",id).ExecuteScalar());
  var refundCash=Convert.ToInt64(Cmd(c,"SELECT COALESCE(SUM(cash),0) FROM refunds WHERE shift=@p0",id).ExecuteScalar());
  var refundCard=Convert.ToInt64(Cmd(c,"SELECT COALESCE(SUM(card),0) FROM refunds WHERE shift=@p0",id).ExecuteScalar());
  var dayRefunds=Convert.ToInt64(Cmd(c,"SELECT COALESCE(SUM(total),0) FROM refunds WHERE julianday(date)>=julianday(@p0) AND julianday(date)<julianday(@p1)",start.ToString("O"),end.ToString("O")).ExecuteScalar());
  var daySales=rows.Sum(x=>x.Total)-dayRefunds;var dayExpenses=expenses.Sum(x=>x.Amount);
  var expected=fund+cash-cashExpenses+movement-refundCash;
  var summary=$"*HELADOS PINWI PINWI*\n*Corte #{id} · {businessNow:dd/MM/yyyy HH:mm}*\nCerró: {Plain(CurrentUserName)}\n\n*VENTAS DEL DÍA* {businessNow:dd/MM/yyyy}\nVentas: {Money(daySales)}\nDevoluciones descontadas: {Money(dayRefunds)}\nMenos gastos: {Money(dayExpenses)}\n*Resultado del día: {Money(daySales-dayExpenses)}*\nAcumulado al momento del corte.\nOperaciones: {rows.Count} ventas\n\n*DETALLE DE GASTOS DEL DÍA*\n{expenseDetail}\n\n*VENTAS DEL TURNO*\nEfectivo neto de ventas: {Money(cash)}\nTarjeta: {Money(card)}\nVentas del turno: {Money(cash+card)}\nDevoluciones: {Money(refundCash+refundCard)}\nMenos gastos del turno: {Money(shiftExpenses)}\nResultado del turno: {Money(cash+card-refundCash-refundCard-shiftExpenses)}\n\n*CONTEO DE CAJA*\nFondo inicial: {Money(fund)}\nMás ventas en efectivo: {Money(cash)}\nMenos gastos de caja: {Money(cashExpenses)}\nMovimientos de efectivo: {Money(movement)}\nMenos devoluciones en efectivo: {Money(refundCash)}\nEfectivo esperado: {Money(expected)}\nEfectivo contado: {Money(counted)}\nDiferencia: {Money(counted-expected)}\n\nResultado = ventas menos gastos registrados; no equivale a utilidad.\nDetalle de ventas y gastos disponible en CSV.";
  static string Quote(string s)=>CsvCell(s);
  var csv="Folio,Fecha,Usuario,Total,Efectivo,Tarjeta\r\n"+string.Join("\r\n",rows.Select(x=>FormattableString.Invariant($"{x.Id},{Quote(x.Date)},{Quote(x.User)},{x.Total/100m:0.00},{x.Cash/100m:0.00},{x.Card/100m:0.00}")));
  var expenseCsv="Folio,Fecha,Turno,Usuario,Concepto,Proveedor o persona,Importe,Origen\r\n"+string.Join("\r\n",expenses.Select(x=>FormattableString.Invariant($"{x.Id},{Quote(x.Date)},{x.Shift},{Quote(x.Cashier)},{Quote(x.Concept)},{Quote(x.Recipient)},{x.Amount/100m:0.00},{Quote(x.SourceLabel)}")));
  var adjustmentRows=new List<string>();
  using(var r=Cmd(c,"SELECT date,cashier,amount,reason,'Movimiento' FROM cash_movements WHERE julianday(date)>=julianday(@p0) AND julianday(date)<julianday(@p1) UNION ALL SELECT date,cashier,-total,reason,kind FROM refunds WHERE julianday(date)>=julianday(@p0) AND julianday(date)<julianday(@p1) ORDER BY date",start.ToString("O"),end.ToString("O")).ExecuteReader())
   while(r.Read())adjustmentRows.Add(FormattableString.Invariant($"{Quote(r.GetString(0))},{Quote(r.GetString(1))},{r.GetInt64(2)/100m:0.00},{Quote(r.GetString(3))},{Quote(r.GetString(4))}"));
  var adjustmentCsv="Fecha,Usuario,Importe,Motivo,Tipo\r\n"+string.Join("\r\n",adjustmentRows);
  Cmd(c,"INSERT INTO cut_reports(shift,cashier,summary,csv,expenses_csv) VALUES(@p0,@p1,@p2,@p3,@p4)",id,CurrentUserName,summary,csv,expenseCsv).ExecuteNonQuery();
  Cmd(c,"UPDATE cut_reports SET adjustments_csv=@p1 WHERE shift=@p0",id,adjustmentCsv).ExecuteNonQuery();
  Cmd(c,"INSERT INTO audit(date,action) VALUES(@p0,@p1)",closed,$"Corte #{id} cerrado por {CurrentUserName}. Contado {counted}, esperado {expected}.").ExecuteNonQuery();t.Commit();NotifyChanged();return id;
 }
 public List<CashCut> Cuts()
 {
  using var c=Open();using var r=Cmd(c,"""
  SELECT s.id,s.opened,s.closed,s.fund,COALESCE(SUM(v.cash),0),COALESCE(SUM(v.card),0),COALESCE(s.counted,0),COALESCE(r.cashier,'No registrado'),COALESCE(r.summary,''),COALESCE(r.csv,''),COALESCE(r.status,'Corte anterior sin nota generada'),(SELECT COALESCE(SUM(amount),0) FROM expenses WHERE shift=s.id AND source='Caja'),(SELECT COALESCE(SUM(amount),0) FROM expenses WHERE shift=s.id),COALESCE(r.expenses_csv,''),(SELECT COALESCE(SUM(amount),0) FROM cash_movements WHERE shift=s.id),(SELECT COALESCE(SUM(cash),0) FROM refunds WHERE shift=s.id),(SELECT COALESCE(SUM(card),0) FROM refunds WHERE shift=s.id),COALESCE(r.adjustments_csv,'')
  FROM shifts s LEFT JOIN sales v ON v.shift=s.id LEFT JOIN cut_reports r ON r.shift=s.id WHERE s.closed IS NOT NULL GROUP BY s.id ORDER BY s.id DESC
  """).ExecuteReader();var result=new List<CashCut>();while(r.Read())result.Add(new(r.GetInt64(0),r.GetString(1),r.GetString(2),r.GetInt64(3),r.GetInt64(4),r.GetInt64(5),r.GetInt64(6),r.GetString(7),r.GetString(8),r.GetString(9),r.GetString(10),r.GetInt64(11),r.GetInt64(12),r.GetString(13),r.GetInt64(14),r.GetInt64(15),r.GetInt64(16),r.GetString(17)));return result;
 }
 public string WriteCutFiles(CashCut cut)
 {
  var folder=Path.Combine(DirectoryPath,"Cortes",$"corte-{cut.Id:000000}");Directory.CreateDirectory(folder);
  WriteAtomic(Path.Combine(folder,"resumen.txt"),cut.Summary);WriteAtomic(Path.Combine(folder,"ventas-del-dia.csv"),cut.Csv);if(!string.IsNullOrWhiteSpace(cut.ExpensesCsv))WriteAtomic(Path.Combine(folder,"gastos-del-dia.csv"),cut.ExpensesCsv);if(cut.AdjustmentsCsv.Length>0)WriteAtomic(Path.Combine(folder,"ajustes-del-dia.csv"),cut.AdjustmentsCsv);return folder;
 }
 public void ConfirmCutSent(long id)
 {
  if(CurrentUserName==null)throw new Exception("Inicia sesión.");using var c=Open();using var t=c.BeginTransaction();
  if(Cmd(c,"UPDATE cut_reports SET status='Enviado (confirmado por usuario)' WHERE shift=@p0",id).ExecuteNonQuery()!=1)throw new Exception("El corte no tiene nota.");
  Cmd(c,"INSERT INTO audit(date,action) VALUES(@p0,@p1)",DateTimeOffset.Now.ToString("O"),$"{CurrentUserName} confirmó manualmente el envío del corte #{id}.").ExecuteNonQuery();t.Commit();NotifyChanged();
 }
 static void WriteAtomic(string path,string text){var temp=path+".tmp";File.WriteAllText(temp,text,new UTF8Encoding(true));File.Move(temp,path,true);}
}
