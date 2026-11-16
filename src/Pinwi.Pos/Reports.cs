using System.IO;
using System.Text;
using Microsoft.Data.Sqlite;
namespace Pinwi.Pos;

public record SalesFilter(DateTime? From=null,DateTime? Through=null,string User="",string Method="Todos");
public record SalesPage(List<SaleSummary> Rows,long Count,decimal Total,decimal Cash,decimal Card);
public record ProductReport(string Kind,string Name,long Quantity,decimal Net,decimal? Savings,decimal Discount);
public sealed partial class Store
{
 SalesFilter AuthorizedFilter(SalesFilter filter)
 {
  if(CurrentUserName==null)throw new Exception("Inicia sesión para consultar reportes.");
  return IsAdmin?filter:filter with{User=CurrentUserName};
 }
 public List<string> ReportUsers()
 {
  if(!IsAdmin)return CurrentUserName==null?[]:[CurrentUserName];
  using var c=Open();using var r=Cmd(c,"SELECT name FROM users UNION SELECT cashier FROM sales UNION SELECT cashier FROM refunds ORDER BY 1").ExecuteReader();
  var users=new List<string>();while(r.Read())users.Add(r.GetString(0));return users;
 }
 public void ExportProducts(string path,SalesFilter filter)
 {
  var rows=ProductReport(filter);using var writer=new StreamWriter(path,false,new UTF8Encoding(true));writer.WriteLine("Tipo,Producto,Unidades netas,Importe neto,Ahorro mayoreo,Descuentos originales");
  foreach(var row in rows)writer.WriteLine(FormattableString.Invariant($"{CsvCell(row.Kind)},{CsvCell(row.Name)},{row.Quantity},{row.Net:0.00},{row.Savings:0.00},{row.Discount:0.00}"));
 }
 const string EventsSql="""
  SELECT id AS folio,date,cashier,total,cash,card,receipt,'Venta' AS event FROM sales
  UNION ALL
  SELECT sale,date,cashier,-total,-cash,-card,receipt,kind FROM refunds
  """;
 static (string Where,object[] Args) ReportWhere(SalesFilter filter)
 {
  if(filter.From>filter.Through)throw new Exception("La fecha inicial debe ser anterior a la final.");
  var clauses=new List<string>();var args=new List<object>();
  void Add(string sql,object value){clauses.Add(sql.Replace("?","@p"+args.Count));args.Add(value);}
  var zone=TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time (Mexico)");
  if(filter.From is DateTime from)Add("julianday(date)>=julianday(?)",new DateTimeOffset(from.Date,zone.GetUtcOffset(from.Date)).ToString("O"));
  if(filter.Through is DateTime through){var end=through.Date.AddDays(1);Add("julianday(date)<julianday(?)",new DateTimeOffset(end,zone.GetUtcOffset(end)).ToString("O"));}
  if(filter.User.Length>0)Add("cashier=?",filter.User);
  if(filter.Method=="Efectivo")clauses.Add("cash<>0 AND card=0");
  if(filter.Method=="Tarjeta")clauses.Add("card<>0 AND cash=0");
  if(filter.Method=="Mixto")clauses.Add("cash<>0 AND card<>0");
  return(clauses.Count==0?"1=1":string.Join(" AND ",clauses),args.ToArray());
 }
 public SalesPage QuerySales(SalesFilter filter,int page=0,int pageSize=100)
 {
  if(page<0||pageSize<1||pageSize>500)throw new ArgumentException("Página inválida.");
  var query=ReportWhere(AuthorizedFilter(filter));using var c=Open();using var t=c.BeginTransaction(deferred:true);
  using var totals=Cmd(c,$"SELECT COUNT(*),COALESCE(SUM(total),0),COALESCE(SUM(cash),0),COALESCE(SUM(card),0) FROM ({EventsSql}) WHERE {query.Where}",query.Args).ExecuteReader();
  totals.Read();var count=totals.GetInt64(0);var total=totals.GetInt64(1)/100m;var cash=totals.GetInt64(2)/100m;var card=totals.GetInt64(3)/100m;totals.Close();
  using var r=Cmd(c,$"SELECT folio,date,total,cash,card,receipt,cashier,event FROM ({EventsSql}) WHERE {query.Where} ORDER BY julianday(date) DESC,folio DESC,event LIMIT {pageSize} OFFSET {checked(page*pageSize)}",query.Args).ExecuteReader();
  var rows=new List<SaleSummary>();while(r.Read())rows.Add(new(r.GetInt64(0),r.GetString(1),r.GetInt64(2)/100m,r.GetInt64(3)/100m,r.GetInt64(4)/100m,r.GetString(5),r.GetString(6),r.GetString(7)));
  return new(rows,count,total,cash,card);
 }
 public void ExportSales(string path,SalesFilter filter)
 {
  var query=ReportWhere(AuthorizedFilter(filter));using var c=Open();
  using var r=Cmd(c,$"SELECT folio,date,cashier,total,cash,card,event FROM ({EventsSql}) WHERE {query.Where} ORDER BY julianday(date),folio",query.Args).ExecuteReader();
  using var writer=new StreamWriter(path,false,new UTF8Encoding(true));writer.WriteLine("Folio,Fecha,Usuario,Total,Efectivo,Tarjeta,Tipo");
  while(r.Read())writer.WriteLine(FormattableString.Invariant($"{r.GetInt64(0)},{CsvCell(r.GetString(1))},{CsvCell(r.GetString(2))},{r.GetInt64(3)/100m:0.00},{r.GetInt64(4)/100m:0.00},{r.GetInt64(5)/100m:0.00},{CsvCell(r.GetString(6))}"));
 }
 public List<ProductReport> ProductReport(SalesFilter filter)
 {
  var query=ReportWhere(AuthorizedFilter(filter));using var c=Open();using var r=Cmd(c,$"""
  SELECT kind,name,SUM(quantity),SUM(net),CASE WHEN COUNT(savings)=COUNT(*) THEN SUM(savings) ELSE NULL END,SUM(discount) FROM (
   SELECT l.kind,l.name,l.quantity,l.price*l.quantity-l.discount AS net,(l.retail-l.price)*l.quantity AS savings,l.discount,s.date,s.cashier,s.cash,s.card FROM lines l JOIN sales s ON s.id=l.sale
   UNION ALL
   SELECT l.kind,l.name,-rl.quantity,-rl.amount,NULL,0,r.date,r.cashier,r.cash,r.card FROM refund_lines rl JOIN refunds r ON r.id=rl.refund JOIN lines l ON l.sale=r.sale AND l.product=rl.product
  ) WHERE {query.Where} GROUP BY kind,name ORDER BY SUM(net) DESC,name
  """,query.Args).ExecuteReader();
  var rows=new List<ProductReport>();while(r.Read())rows.Add(new(r.GetString(0),r.GetString(1),r.GetInt64(2),r.GetInt64(3)/100m,r.IsDBNull(4)?null:r.GetInt64(4)/100m,r.GetInt64(5)/100m));return rows;
 }
 public static string CsvCell(string value)
 {
  // Protect text cells when opened in Excel; quoting alone does not neutralize formulas.
  if(value.TrimStart().FirstOrDefault() is '=' or '+' or '-' or '@')value="'"+value;
  return "\""+value.Replace("\"","\"\"")+"\"";
 }
}
