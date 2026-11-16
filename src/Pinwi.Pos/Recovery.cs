using System.IO;
using Microsoft.Data.Sqlite;
namespace Pinwi.Pos;

public record BackupInfo(int Version,long Sales,long Products,string LastSale)
{
 public string Description=>$"Base v{Version} · {Sales:N0} ventas · {Products:N0} productos\nÚltima venta: {(LastSale.Length==0?"Sin ventas":LastSale)}";
}
public sealed partial class Store
{
 static SqliteConnection ReadBackup(string path)
 {
  if(!File.Exists(path))throw new Exception("El respaldo no existe o la unidad no está disponible.");
  var c=new SqliteConnection(new SqliteConnectionStringBuilder{DataSource=path,Mode=SqliteOpenMode.ReadOnly,Pooling=false}.ToString());
  try{c.Open();return c;}catch{c.Dispose();throw new Exception("No se pudo leer el respaldo. Selecciona una base SQLite válida.");}
 }
 static BackupInfo InspectBackup(SqliteConnection c)
 {
  using(var r=Cmd(c,"PRAGMA integrity_check").ExecuteReader())
  {if(!r.Read()||r.GetString(0)!="ok"||r.Read())throw new Exception("El respaldo tiene errores de integridad. No se modificó la base actual.");}
  using(var r=Cmd(c,"PRAGMA foreign_key_check").ExecuteReader())if(r.Read())throw new Exception("El respaldo tiene relaciones incompletas.");
  var version=Convert.ToInt32(Cmd(c,"PRAGMA user_version").ExecuteScalar());
  if(version<1||version>SchemaVersion)throw new Exception("La versión del respaldo no es compatible con este POS.");
  // Check the core schema, not just the file extension or user_version marker.
  Cmd(c,"SELECT id,name,salt,hash FROM users LIMIT 0").ExecuteNonQuery();
  Cmd(c,"SELECT id,category,name,retail,wholesale,stock,image FROM products LIMIT 0").ExecuteNonQuery();
  Cmd(c,"SELECT id,date,total,cash,card,receipt,token,shift FROM sales LIMIT 0").ExecuteNonQuery();
  Cmd(c,"SELECT sale,product,quantity,price,discount,wholesale,name FROM lines LIMIT 0").ExecuteNonQuery();
  if(Convert.ToInt64(Cmd(c,"SELECT COUNT(*) FROM users WHERE length(salt)=32 AND length(hash)=32").ExecuteScalar())==0)throw new Exception("El respaldo no contiene una cuenta de administrador recuperable.");
  if(version>=8&&Convert.ToInt64(Cmd(c,"SELECT COUNT(*) FROM users WHERE role='Administrador' AND active=1 AND length(salt)=32 AND length(hash)=32").ExecuteScalar())==0)throw new Exception("El respaldo no contiene un administrador activo recuperable.");
  if(Cmd(c,"SELECT 1 FROM sales WHERE total<0 OR cash<0 OR card<0 OR cash+card<>total UNION ALL SELECT 1 FROM lines WHERE quantity<=0 OR price<0 OR discount<0 OR discount>price*quantity LIMIT 1").ExecuteScalar()!=null)throw new Exception("El respaldo contiene importes o cantidades inconsistentes.");
  return new(version,Convert.ToInt64(Cmd(c,"SELECT COUNT(*) FROM sales").ExecuteScalar()),Convert.ToInt64(Cmd(c,"SELECT COUNT(*) FROM products").ExecuteScalar()),Convert.ToString(Cmd(c,"SELECT COALESCE(MAX(date),'') FROM sales").ExecuteScalar())??"");
 }
 public BackupInfo InspectBackup(string path){using var c=ReadBackup(path);return InspectBackup(c);}

 // Call with background protection stopped and all UI operations suspended.
 // BackupDatabase replaces the destination inside SQLite's backup transaction.
 // An incomplete copy is rolled back by SQLite; no delete/rename of the active DB.
 public string RestoreBackup(string path,string adminPassword)
 {
  lock(backupGate)
  {
  RequireAdmin(adminPassword);
  if(Path.GetFullPath(path).Equals(Path.GetFullPath(DatabasePath),StringComparison.OrdinalIgnoreCase))throw new Exception("Selecciona un respaldo diferente de la base activa.");
  if(Shift()!=null)throw new Exception("Cierra el turno actual antes de restaurar.");
  if(LoadDraft()?.Lines.Count>0)throw new Exception("Guarda o vacía la venta actual antes de restaurar.");
  var stageFolder=Path.Combine(DirectoryPath,"Restauracion",Guid.NewGuid().ToString("N"));Directory.CreateDirectory(stageFolder);
  var stagePath=Path.Combine(stageFolder,"pinwi.db");
  // Freeze a consistent copy first, so changes to a USB/source cannot race validation.
  using(var source=ReadBackup(path))
  using(var stage=new SqliteConnection(new SqliteConnectionStringBuilder{DataSource=stagePath,Pooling=false}.ToString()))
  {stage.Open();source.BackupDatabase(stage);InspectBackup(stage);}
  var candidate=new Store(stageFolder); // migrate and seed only the isolated candidate
  candidate.InspectBackup(stagePath);
  // Force all expected tables/columns to be read before touching the live DB.
  candidate.Products();candidate.Sales();candidate.Cuts();candidate.Users();candidate.GetSettings();
  var before=Path.Combine(DirectoryPath,"Respaldos",$"antes-restaurar-{DateTime.Now:yyyyMMdd-HHmmss-fff}-{Guid.NewGuid():N}.db");
  Directory.CreateDirectory(Path.GetDirectoryName(before)!);Backup(before);
  using(var source=ReadBackup(stagePath))using(var target=Open())source.BackupDatabase(target);
  CurrentUserName=null;CurrentRole=null;InvalidatePhotos();
  // No NotifyChanged: restart/sign-in is required before auto backups resume.
  return before;
  }
 }
}
