using Microsoft.Data.Sqlite;
using System.IO;
using System.Security.Cryptography;
using System.Text.Json;
namespace Pinwi.Pos;
public sealed partial class Store
{
 public string DirectoryPath { get; }
 public string? CurrentUserName { get; private set; }
 public string DatabasePath => Path.Combine(DirectoryPath,"pinwi.db");
 SqliteConnection Open() { var c=new SqliteConnection(new SqliteConnectionStringBuilder { DataSource=DatabasePath,ForeignKeys=true }.ToString()); c.Open(); using var q=c.CreateCommand();q.CommandText="PRAGMA synchronous=FULL; PRAGMA busy_timeout=5000;";q.ExecuteNonQuery();return c; }
 public event Action? Changed;
 void NotifyChanged()=>Changed?.Invoke();
 static SqliteCommand Cmd(SqliteConnection c,string sql, params object[] values) { var q=c.CreateCommand(); q.CommandText=sql; for(var i=0;i<values.Length;i++) q.Parameters.AddWithValue("@p"+i,values[i]); return q; }
 public Store(string? directory=null)
 {
  DirectoryPath=directory ?? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),"PinwiPOS");
  Directory.CreateDirectory(DirectoryPath);
  using var c=Open();
  var version=Convert.ToInt32(Cmd(c,"PRAGMA user_version").ExecuteScalar());
  if(version>7)throw new Exception("Esta base requiere una versión más reciente del POS.");
  if(version is >= 1 and < 7){var folder=Path.Combine(DirectoryPath,"Respaldos");Directory.CreateDirectory(folder);using var copy=new SqliteConnection(new SqliteConnectionStringBuilder{DataSource=Path.Combine(folder,$"antes-v7-{DateTime.Now:yyyyMMdd-HHmmss-fff}.db")}.ToString());copy.Open();c.BackupDatabase(copy);}
  Cmd(c,"PRAGMA journal_mode=WAL").ExecuteScalar();
  using var t=c.BeginTransaction();
  Cmd(c,"""
  CREATE TABLE IF NOT EXISTS users(id INTEGER PRIMARY KEY,name TEXT UNIQUE NOT NULL,salt BLOB NOT NULL,hash BLOB NOT NULL);
  CREATE TABLE IF NOT EXISTS products(id INTEGER PRIMARY KEY,category TEXT NOT NULL,name TEXT NOT NULL,retail INTEGER NOT NULL CHECK(retail>=0),wholesale INTEGER NOT NULL CHECK(wholesale>=0 AND wholesale<=retail),stock INTEGER NOT NULL CHECK(stock>=0),image TEXT NOT NULL);
  CREATE TABLE IF NOT EXISTS product_photos(product INTEGER PRIMARY KEY REFERENCES products(id),image BLOB NOT NULL);
  CREATE TABLE IF NOT EXISTS quick_products(product INTEGER PRIMARY KEY REFERENCES products(id),group_key TEXT NOT NULL UNIQUE,ready INTEGER NOT NULL DEFAULT 1);
  CREATE TABLE IF NOT EXISTS shifts(id INTEGER PRIMARY KEY,opened TEXT NOT NULL,closed TEXT,fund INTEGER NOT NULL,counted INTEGER);
  CREATE UNIQUE INDEX IF NOT EXISTS one_shift ON shifts((1)) WHERE closed IS NULL;
  CREATE TABLE IF NOT EXISTS sales(id INTEGER PRIMARY KEY,token TEXT UNIQUE NOT NULL,date TEXT NOT NULL,shift INTEGER NOT NULL REFERENCES shifts(id),total INTEGER NOT NULL,cash INTEGER NOT NULL,card INTEGER NOT NULL,receipt TEXT NOT NULL);
  CREATE TABLE IF NOT EXISTS lines(sale INTEGER NOT NULL REFERENCES sales(id),product INTEGER NOT NULL REFERENCES products(id),quantity INTEGER NOT NULL,price INTEGER NOT NULL,discount INTEGER NOT NULL,wholesale INTEGER NOT NULL,name TEXT NOT NULL);
  CREATE TABLE IF NOT EXISTS audit(id INTEGER PRIMARY KEY,date TEXT NOT NULL,action TEXT NOT NULL);
  CREATE INDEX IF NOT EXISTS sale_dates ON sales(date);
  CREATE TABLE IF NOT EXISTS drafts(owner TEXT PRIMARY KEY,token TEXT NOT NULL,payload TEXT NOT NULL,saved TEXT NOT NULL);
  CREATE TABLE IF NOT EXISTS cut_reports(shift INTEGER PRIMARY KEY REFERENCES shifts(id),cashier TEXT NOT NULL,summary TEXT NOT NULL,csv TEXT NOT NULL,status TEXT NOT NULL DEFAULT 'Pendiente de envío');
  CREATE TABLE IF NOT EXISTS expenses(id INTEGER PRIMARY KEY,token TEXT UNIQUE NOT NULL,date TEXT NOT NULL,shift INTEGER NOT NULL REFERENCES shifts(id),cashier TEXT NOT NULL,concept TEXT NOT NULL,recipient TEXT NOT NULL,amount INTEGER NOT NULL CHECK(amount>0),source TEXT NOT NULL CHECK(source IN ('Caja','Externo')));
  CREATE INDEX IF NOT EXISTS expense_dates ON expenses(date);
  CREATE INDEX IF NOT EXISTS expense_shifts ON expenses(shift);
  """).ExecuteNonQuery();
  var columns=new List<string>();using(var reader=Cmd(c,"PRAGMA table_info(sales)").ExecuteReader())while(reader.Read())columns.Add(reader.GetString(1));
  if(!columns.Contains("cashier"))Cmd(c,"ALTER TABLE sales ADD COLUMN cashier TEXT NOT NULL DEFAULT 'No registrado'").ExecuteNonQuery();
  var reportColumns=new List<string>();using(var reader=Cmd(c,"PRAGMA table_info(cut_reports)").ExecuteReader())while(reader.Read())reportColumns.Add(reader.GetString(1));
  if(!reportColumns.Contains("expenses_csv"))Cmd(c,"ALTER TABLE cut_reports ADD COLUMN expenses_csv TEXT NOT NULL DEFAULT ''").ExecuteNonQuery();
  Cmd(c,"PRAGMA user_version=7").ExecuteNonQuery();
  if(Convert.ToInt64(Cmd(c,"SELECT COUNT(*) FROM products").ExecuteScalar())==0)
   foreach(var p in JsonSerializer.Deserialize<List<Product>>(File.ReadAllText(Path.Combine(AppContext.BaseDirectory,"catalog.json")))!)
    Cmd(c,"INSERT INTO products VALUES(@p0,@p1,@p2,@p3,@p4,0,@p5)",p.Id,p.Category,p.Name,p.Retail,p.Wholesale,p.Image).ExecuteNonQuery();
  if(version<5)
   foreach(var p in JsonSerializer.Deserialize<List<Product>>(File.ReadAllText(Path.Combine(AppContext.BaseDirectory,"packages.json")))!)
    Cmd(c,"INSERT OR IGNORE INTO products(id,category,name,retail,wholesale,stock,image) VALUES(@p0,@p1,@p2,@p3,@p4,0,@p5)",p.Id,p.Category,p.Name,p.Retail,p.Wholesale,p.Image).ExecuteNonQuery();
  if(version<7)SeedQuickProducts(c);
  t.Commit();NotifyChanged();
 }
 public bool HasAdmin() { using var c=Open(); return Convert.ToInt64(Cmd(c,"SELECT COUNT(*) FROM users").ExecuteScalar())>0; }
 public void CreateAdmin(string name,string password) { if(name.Trim().Length<2 || password.Length<8) throw new Exception("Usa un nombre y una contraseña de al menos 8 caracteres."); using var c=Open(); using var t=c.BeginTransaction(); if(Convert.ToInt64(Cmd(c,"SELECT COUNT(*) FROM users").ExecuteScalar())>0) throw new Exception("Ya existe administrador."); var salt=RandomNumberGenerator.GetBytes(32); var hash=Rfc2898DeriveBytes.Pbkdf2(password,salt,600000,HashAlgorithmName.SHA256,32); Cmd(c,"INSERT INTO users(name,salt,hash) VALUES(@p0,@p1,@p2)",name.Trim(),salt,hash).ExecuteNonQuery(); t.Commit();NotifyChanged(); }
 public bool Authenticate(string password) { using var c=Open(); using var r=Cmd(c,"SELECT salt,hash FROM users LIMIT 1").ExecuteReader(); return r.Read() && CryptographicOperations.FixedTimeEquals(Rfc2898DeriveBytes.Pbkdf2(password,(byte[])r[0],600000,HashAlgorithmName.SHA256,32),(byte[])r[1]); }
 public bool SignIn(string password){if(!Authenticate(password))return false;using var c=Open();CurrentUserName=(string?)Cmd(c,"SELECT name FROM users ORDER BY id LIMIT 1").ExecuteScalar();return CurrentUserName!=null;}
 public List<Product> Products() { using var c=Open(); using var r=Cmd(c,"SELECT p.*,q.group_key,COALESCE(q.ready,1) FROM products p LEFT JOIN quick_products q ON q.product=p.id ORDER BY p.id").ExecuteReader(); var list=new List<Product>(); while(r.Read()) list.Add(new(r.GetInt32(0),r.GetString(1),r.GetString(2),r.GetInt64(3),r.GetInt64(4),r.GetInt32(5),r.GetString(6)){QuickGroup=r.IsDBNull(7)?null:r.GetString(7),QuickReady=r.GetInt32(8)==1}); return LoadProductPhotos(list); }
 public void UpdateProduct(int id,long retail,long wholesale,int stock,string reason,string password) { if(!Authenticate(password)) throw new Exception("Contraseña incorrecta."); if(retail<0||wholesale<0||wholesale>retail||stock<0||string.IsNullOrWhiteSpace(reason)) throw new Exception("Revisa precios, existencia y motivo."); using var c=Open(); using var t=c.BeginTransaction(); Cmd(c,"UPDATE products SET retail=@p1,wholesale=@p2,stock=@p3 WHERE id=@p0",id,retail,wholesale,stock).ExecuteNonQuery(); Cmd(c,"INSERT INTO audit(date,action) VALUES(@p0,@p1)",DateTimeOffset.Now.ToString("O"),$"Administrador: producto {id}, existencia {stock}, menudeo {retail}, mayoreo {wholesale}. {reason}").ExecuteNonQuery(); t.Commit();NotifyChanged(); }
 public long? Shift() { using var c=Open(); var v=Cmd(c,"SELECT id FROM shifts WHERE closed IS NULL").ExecuteScalar(); return v is null ? null : Convert.ToInt64(v); }
 public void OpenShift(long fund) { if(fund<0) throw new Exception("Fondo inválido."); using var c=Open(); Cmd(c,"INSERT INTO shifts(opened,fund) VALUES(@p0,@p1)",DateTimeOffset.Now.ToString("O"),fund).ExecuteNonQuery();NotifyChanged(); }
 public long ExpectedCash() { using var c=Open(); return Convert.ToInt64(Cmd(c,"SELECT COALESCE((SELECT fund FROM shifts WHERE closed IS NULL),0)+COALESCE((SELECT SUM(cash) FROM sales WHERE shift=(SELECT id FROM shifts WHERE closed IS NULL)),0)-COALESCE((SELECT SUM(amount) FROM expenses WHERE source='Caja' AND shift=(SELECT id FROM shifts WHERE closed IS NULL)),0)").ExecuteScalar()); }
 public string Checkout(List<CartItem> items,decimal discount,long cashReceived,long card,string token,string reason,string password)
 {
  if(string.IsNullOrWhiteSpace(CurrentUserName))throw new Exception("Inicia sesión antes de cobrar.");
  if(discount>0 && (!Authenticate(password)||string.IsNullOrWhiteSpace(reason))) throw new Exception("Autoriza el descuento e indica el motivo.");
  using var c=Open(); using var t=c.BeginTransaction();
  var existing=Cmd(c,"SELECT receipt FROM sales WHERE token=@p0",token).ExecuteScalar(); if(existing is string receipt) return receipt;
  var shift=Cmd(c,"SELECT id FROM shifts WHERE closed IS NULL").ExecuteScalar() ?? throw new Exception("Abre un turno de caja primero.");
  if(items.Count==0) throw new Exception("Agrega productos.");
  foreach(var item in items) { using var r=Cmd(c,"SELECT p.retail,p.wholesale,q.group_key,COALESCE(q.ready,1) FROM products p LEFT JOIN quick_products q ON q.product=p.id WHERE p.id=@p0",item.Product.Id).ExecuteReader(); if(!r.Read()||r.GetInt64(0)!=item.Product.Retail||r.GetInt64(1)!=item.Product.Wholesale || (r.IsDBNull(2)?null:r.GetString(2))!=item.Product.QuickGroup || r.GetInt32(3)!=1) throw new Exception("Los precios cambiaron. Vuelve a crear la venta."); }
  var lines=Pricing.Calculate(items,discount); var total=lines.Sum(x=>x.Net);
  if(cashReceived<0||card<0||card>total||cashReceived+card<total) throw new Exception("El pago no cubre el total o la tarjeta excede el total.");
  var shortages=new List<string>();
  foreach(var l in lines){if(Cmd(c,"SELECT 1 FROM quick_products WHERE product=@p0",l.ProductId).ExecuteScalar()!=null)continue;var stock=Convert.ToInt32(Cmd(c,"SELECT stock FROM products WHERE id=@p0",l.ProductId).ExecuteScalar());if(stock<l.Quantity)shortages.Add(l.Name);if(Cmd(c,"UPDATE products SET stock=MAX(0,stock-@p1) WHERE id=@p0",l.ProductId,l.Quantity).ExecuteNonQuery()!=1)throw new Exception("Producto no disponible en el catálogo.");}
  var date=DateTimeOffset.Now.ToString("O");
  Cmd(c,"INSERT INTO sales(token,date,shift,total,cash,card,receipt,cashier) VALUES(@p0,@p1,@p2,@p3,@p4,@p5,'',@p6)",token,date,shift,total,total-card,card,CurrentUserName).ExecuteNonQuery();
  var id=Convert.ToInt64(Cmd(c,"SELECT last_insert_rowid()").ExecuteScalar());
  foreach(var l in lines) Cmd(c,"INSERT INTO lines VALUES(@p0,@p1,@p2,@p3,@p4,@p5,@p6)",id,l.ProductId,l.Quantity,l.UnitPrice,l.Discount,l.IsWholesale?1:0,l.Name).ExecuteNonQuery();
  var text=$"HELADOS PINWI PINWI\n833 393 2616\nFolio {id:000000} · {DateTime.Now:g}\nCajero: {CurrentUserName}\n\n"+string.Join("\n",lines.Select(l=>$"{l.Name}\n{l.Quantity} × ${l.Price:0.00} {l.Mode}  ${l.Total:0.00}"))+$"\n\nDescuento: ${lines.Sum(x=>x.Discount)/100m:0.00}\nTOTAL: ${total/100m:0.00}\nEfectivo recibido: ${cashReceived/100m:0.00}\nTarjeta externa: ${card/100m:0.00}\nCambio: ${(cashReceived+card-total)/100m:0.00}\n\n¡Gracias por tu compra!\nComprobante de venta";
  Cmd(c,"UPDATE sales SET receipt=@p1 WHERE id=@p0",id,text).ExecuteNonQuery();
  Cmd(c,"DELETE FROM drafts WHERE token=@p0",token).ExecuteNonQuery();
  Cmd(c,"INSERT INTO audit(date,action) VALUES(@p0,@p1)",date,$"Usuario {CurrentUserName}. Venta {id}, descuento {discount}%: {reason}. Sin existencia suficiente registrada: {string.Join("; ",shortages)}").ExecuteNonQuery(); t.Commit();NotifyChanged(); return text;
 }
 public List<SaleSummary> Sales() { using var c=Open(); using var r=Cmd(c,"SELECT id,date,total,cash,card,receipt,cashier FROM sales ORDER BY id DESC LIMIT 1000").ExecuteReader(); var rows=new List<SaleSummary>(); while(r.Read()) rows.Add(new(r.GetInt64(0),r.GetString(1),r.GetInt64(2)/100m,r.GetInt64(3)/100m,r.GetInt64(4)/100m,r.GetString(5),r.GetString(6))); return rows; }
 public void Backup(string path) { if(Path.GetFullPath(path)==Path.GetFullPath(DatabasePath)) throw new Exception("Elige un archivo diferente a la base activa."); using var c=Open(); using var dest=new SqliteConnection(new SqliteConnectionStringBuilder{DataSource=path}.ToString()); dest.Open(); c.BackupDatabase(dest); }
}
public record SaleSummary(long Folio,string Date,decimal Total,decimal Cash,decimal Card,string Receipt,string Cashier);
