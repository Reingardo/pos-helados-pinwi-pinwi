using System.IO;
using System.Security.Cryptography;
using Microsoft.Data.Sqlite;
using Pinwi.Pos;

internal static class StoreDistributionTests
{
 static void Assert(bool condition,string message){if(!condition)throw new Exception(message);}
 static void Reject(Action action,string message)
 {var rejected=false;try{action();}catch{rejected=true;}Assert(rejected,message);}
 static void Sql(string path,string sql)
 {
  using var c=new SqliteConnection(new SqliteConnectionStringBuilder{DataSource=path,Pooling=false}.ToString());
  c.Open();using var q=c.CreateCommand();q.CommandText=sql;q.ExecuteNonQuery();
 }
 public static void Run()
 {
  var root=Path.Combine(Path.GetTempPath(),"PinwiDistributionTests",Guid.NewGuid().ToString("N"));
  var profile=Path.Combine(root,"Perfil");var local=Path.Combine(profile,"AppData","Local");
  var localPath=DataLocation.ForInstallation(false,profile,local);
  var storePath=DataLocation.ForInstallation(true,profile,local);
  Assert(localPath==Path.Combine(local,"PinwiPOS"),"Cambió la ruta de la entrega local");
  Assert(storePath==Path.Combine(profile,"PinwiPOS-Store")&&!storePath.StartsWith(local,StringComparison.OrdinalIgnoreCase),"Store depende de una exclusión de AppData");
  Assert(!DataLocation.IsPackaged,"El proceso de pruebas debe ejecutarse sin identidad MSIX");
  var original=new Store(localPath);var password=Guid.NewGuid().ToString("N");
  original.CreateAdmin("Administrador de traslado",password);Assert(original.SignIn(password),"Sesión de origen");
  var cashierPassword=Guid.NewGuid().ToString("N");original.SaveUser(null,"Cajero de traslado","Cajero",true,cashierPassword,password);
  var photo=Store.PrepareProductImage(Path.Combine(AppContext.BaseDirectory,"Assets","Packages","77.png"));
  original.SaveProduct(1,"Pinwis de agua","Limón traslado",700,600,"Precio y foto de traslado",password,photo);
  var settings=original.GetSettings() with{BusinessName="Negocio de traslado",Pricing=new(12,50000,true),PaperMm=58};
  original.SaveSettings(settings,password,"Configuración para traslado");
  original.OpenShift(10000);
  original.Checkout([new(original.Products().First(p=>p.Id==1),3)],0,5000,0,Guid.NewGuid().ToString("N"),"","");
  original.AddExpense(500,"Material de traslado","Proveedor ficticio","Caja",Guid.NewGuid().ToString("N"));
  original.CloseShift(original.ExpectedCash());
  original.SaveDraft(new(Guid.NewGuid().ToString("N"),original.CurrentUserName!,[new(1,2)],0,"",DateTimeOffset.Now.ToString("O")));
  original.ParkSale("Carrito para traslado");
  var backup=Path.Combine(root,"copia-elegida.db");original.Backup(backup);
  var sourceHash=SHA256.HashData(File.ReadAllBytes(backup));
  Store.ImportFirstBackup(backup,storePath);
  Assert(SHA256.HashData(File.ReadAllBytes(backup)).SequenceEqual(sourceHash),"Se modificó el respaldo de origen");
  var imported=new Store(storePath);
  Assert(imported.SignIn(password)&&imported.GetSettings()==settings,"No se conservaron acceso o configuración");
  Assert(imported.Sales().Single()==original.Sales().Single()&&imported.Cuts().Single()==original.Cuts().Single(),"Se modificó el historial al importar");
  Assert(imported.Expenses(DateTimeOffset.Now).Count==1&&imported.PendingSales().Single().Label=="Carrito para traslado","Se perdieron gastos o pendientes");
  Assert(imported.Users().Count==2&&File.ReadAllBytes(imported.Products().First(p=>p.Id==1).ImagePath).SequenceEqual(photo),"Se perdieron cuentas o fotos");
  Assert(imported.SignIn("Cajero de traslado",cashierPassword)&&!imported.IsAdmin,"Se perdió el rol de cajero");
  Reject(()=>Store.ImportFirstBackup(backup,storePath),"Se reemplazó una instalación existente sin autorización");
  Assert(new Store(storePath).Sales().Single()==original.Sales().Single(),"El rechazo modificó el destino existente");
  original.SaveProduct(1,"Pinwis de agua","Cambio solo local",800,700,"Después de copia",password);
  Assert(new Store(storePath).Products().First(p=>p.Id==1).Retail==700,"Se mezclaron las bases local y Store");
  var bad=Path.Combine(root,"corrupto.db");File.WriteAllText(bad,"no es una base");
  var failed=Path.Combine(root,"Importacion fallida");
  Reject(()=>Store.ImportFirstBackup(bad,failed),"Aceptó un archivo corrupto");
  Assert(!File.Exists(Path.Combine(failed,"pinwi.db")),"Un fallo publicó una base parcial");
  Store.ImportFirstBackup(backup,failed);Assert(new Store(failed).HasAdmin(),"No permitió reintentar tras un fallo");
  var future=Path.Combine(root,"futura.db");File.Copy(backup,future);Sql(future,"PRAGMA user_version=999");
  var futureTarget=Path.Combine(root,"Futura rechazada");
  Reject(()=>Store.ImportFirstBackup(future,futureTarget),"Aceptó una versión futura");
  Assert(!File.Exists(Path.Combine(futureTarget,"pinwi.db")),"Publicó una base futura");
  var noAdmin=new Store(Path.Combine(root,"Sin administrador"));var noAdminTarget=Path.Combine(root,"Sin acceso rechazado");
  Reject(()=>Store.ImportFirstBackup(noAdmin.DatabasePath,noAdminTarget),"Aceptó una copia sin administrador recuperable");
  Assert(!File.Exists(Path.Combine(noAdminTarget,"pinwi.db")),"Publicó una base sin acceso recuperable");
  var legacy=Path.Combine(root,"v7.db");File.Copy(backup,legacy);
  Sql(legacy,"ALTER TABLE users DROP COLUMN role; ALTER TABLE users DROP COLUMN active; PRAGMA user_version=7");
  var legacyHash=SHA256.HashData(File.ReadAllBytes(legacy));var legacyTarget=Path.Combine(root,"Migrada desde v7");
  Store.ImportFirstBackup(legacy,legacyTarget);
  var migrated=new Store(legacyTarget);
  Assert(migrated.SignIn(password)&&migrated.Sales().Count==1&&migrated.InspectBackup(migrated.DatabasePath).Version==8,"Falló la migración aislada desde v7");
  Assert(SHA256.HashData(File.ReadAllBytes(legacy)).SequenceEqual(legacyHash),"La migración modificó el origen v7");
  Assert(File.ReadAllBytes(migrated.Products().First(p=>p.Id==1).ImagePath).SequenceEqual(photo),"La migración perdió la foto");
  Console.WriteLine("OK distribución Store: ruta fuera de AppData; local intacta; importación aislada, historial/fotos/cuentas/roles/configuración/pendientes; rechazo sin reemplazo; reintento y migración v7 sin modificar origen");
 }
}
