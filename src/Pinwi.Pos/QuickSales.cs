using Microsoft.Data.Sqlite;
namespace Pinwi.Pos;
public static class ProductGroups
{
 public static string Key(Product product)=>product.QuickGroup ?? (product.Category switch
 {
  "Pinwis de agua" or "Pinwis de leche"=>"bolis",
  "Heladitos de agua" or "Heladitos de leche"=>"bollos",
  "Esquimales" or "Paquetes"=>"producto:"+product.Id,
  _=>"categoria:"+product.Category
 });
 public static readonly (string Key,string Name,string[] Categories)[] QuickDefinitions=
 [
  ("bolis","Bolis de agua y leche",["Pinwis de agua","Pinwis de leche"]),
  ("categoria:Paletas de agua","Paletas de agua",["Paletas de agua"]),
  ("categoria:Paletas de leche","Paletas de leche",["Paletas de leche"]),
  ("bollos","Bollos / Heladitos",["Heladitos de agua","Heladitos de leche"]),
  ("categoria:Paletas artesanales","Paletas artesanales",["Paletas artesanales"]),
  ("categoria:Nieve","Nieves",["Nieve"]),
  ("categoria:Sándwich","Sándwiches",["Sándwich"])
 ];
}
public sealed partial class Store
{
 void SeedQuickProducts(SqliteConnection c)
 {
  foreach(var group in ProductGroups.QuickDefinitions)
  {
   if(Cmd(c,"SELECT 1 FROM quick_products WHERE group_key=@p0",group.Key).ExecuteScalar()!=null)continue;
   var candidates=new List<Product>();using(var r=Cmd(c,"SELECT p.* FROM products p WHERE NOT EXISTS(SELECT 1 FROM quick_products q WHERE q.product=p.id) ORDER BY p.id").ExecuteReader())
    while(r.Read())if(group.Categories.Contains(r.GetString(1)))candidates.Add(new(r.GetInt32(0),r.GetString(1),r.GetString(2),r.GetInt64(3),r.GetInt64(4),r.GetInt32(5),r.GetString(6)));
   if(candidates.Count==0)continue;
   var first=candidates[0];var ready=candidates.Select(p=>(p.Retail,p.Wholesale)).Distinct().Count()==1;
   Cmd(c,"INSERT INTO products(category,name,retail,wholesale,stock,image) VALUES('Cobro rápido',@p0,@p1,@p2,0,@p3)",group.Name,first.Retail,first.Wholesale,first.Image).ExecuteNonQuery();
   var id=Convert.ToInt32(Cmd(c,"SELECT last_insert_rowid()").ExecuteScalar());
   Cmd(c,"INSERT INTO quick_products(product,group_key,ready) VALUES(@p0,@p1,@p2)",id,group.Key,ready?1:0).ExecuteNonQuery();
  }
 }
}
