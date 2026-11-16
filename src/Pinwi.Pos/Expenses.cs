using System.Globalization;
namespace Pinwi.Pos;
public record Expense(long Id,string Date,long Shift,string Cashier,string Concept,string Recipient,long Amount,string Source)
{
 public decimal Pesos=>Amount/100m;
 public string DateLabel=>TimeZoneInfo.ConvertTime(DateTimeOffset.Parse(Date),TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time (Mexico)")).ToString("g");
 public string SourceLabel=>Source=="Caja"?"Efectivo de caja":"Pago externo";
}
public static class BusinessDay
{
 public static (DateTimeOffset Start,DateTimeOffset End) Bounds(DateTimeOffset instant)
 {
  var zone=TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time (Mexico)");var date=TimeZoneInfo.ConvertTime(instant,zone).Date;return(new(date,zone.GetUtcOffset(date)),new(date.AddDays(1),zone.GetUtcOffset(date.AddDays(1))));
 }
}
public sealed partial class Store
{
 public long AddExpense(long amount,string concept,string recipient,string source,string token)
 {
  if(CurrentUserName==null)throw new Exception("Inicia sesión.");
  concept=concept.Trim();recipient=recipient.Trim();
  if(amount<=0||amount>10000000000||concept.Length<3||concept.Length>240||recipient.Length>120||concept.Any(char.IsControl)||recipient.Any(char.IsControl)||string.IsNullOrWhiteSpace(token)||source is not ("Caja" or "Externo"))throw new Exception("Indica un importe positivo, un concepto de 3 a 240 caracteres y el origen del pago.");
  using var c=Open();using var t=c.BeginTransaction();var existing=Cmd(c,"SELECT id FROM expenses WHERE token=@p0",token).ExecuteScalar();if(existing!=null)return Convert.ToInt64(existing);
  var shift=Cmd(c,"SELECT id FROM shifts WHERE closed IS NULL").ExecuteScalar()??throw new Exception("Abre caja antes de registrar un gasto.");
  if(source=="Caja")
  {
   var available=ExpectedCash(c,Convert.ToInt64(shift));
   if(amount>available)throw new Exception("El gasto supera el efectivo esperado en caja. Revisa el importe o selecciona pago externo si se pagó con otro dinero.");
  }
  var now=DateTimeOffset.Now.ToString("O");Cmd(c,"INSERT INTO expenses(token,date,shift,cashier,concept,recipient,amount,source) VALUES(@p0,@p1,@p2,@p3,@p4,@p5,@p6,@p7)",token,now,shift,CurrentUserName,concept,recipient,amount,source).ExecuteNonQuery();
  var id=Convert.ToInt64(Cmd(c,"SELECT last_insert_rowid()").ExecuteScalar());Cmd(c,"INSERT INTO audit(date,action) VALUES(@p0,@p1)",now,$"Gasto #{id}, usuario {CurrentUserName}, importe {amount} centavos, origen {source}: {concept}. {recipient}").ExecuteNonQuery();t.Commit();NotifyChanged();return id;
 }
 public List<Expense> Expenses(DateTimeOffset day)
 {
  var bounds=BusinessDay.Bounds(day);using var c=Open();using var r=Cmd(c,"SELECT id,date,shift,cashier,concept,recipient,amount,source FROM expenses WHERE julianday(date)>=julianday(@p0) AND julianday(date)<julianday(@p1) ORDER BY id DESC",bounds.Start.ToString("O"),bounds.End.ToString("O")).ExecuteReader();var rows=new List<Expense>();while(r.Read())rows.Add(new(r.GetInt64(0),r.GetString(1),r.GetInt64(2),r.GetString(3),r.GetString(4),r.GetString(5),r.GetInt64(6),r.GetString(7)));return rows;
 }
}
