using Pinwi.Pos;
using Microsoft.Data.Sqlite;
using System.IO;
public static class ImprovementsTests
{
 static void Assert(bool condition,string message){if(!condition)throw new Exception(message);}
 static void Reject(Action action,string message)
 {var rejected=false;try{action();}catch{rejected=true;}Assert(rejected,message);}
 static (Store Store,string Password) Fixture()
 {
  var store=new Store(Path.Combine(Path.GetTempPath(),"PinwiTests",Guid.NewGuid().ToString("N")));var password=Guid.NewGuid().ToString();store.CreateAdmin("Administración de prueba",password);store.SignIn(password);return(store,password);
 }
 static object? Sql(Store store,string sql)
 {
  using var c=new SqliteConnection(new SqliteConnectionStringBuilder{DataSource=store.DatabasePath}.ToString());c.Open();using var q=c.CreateCommand();q.CommandText=sql;return q.ExecuteScalar();
 }
 public static void Run()
 {
  var (store,password)=Fixture();
  using(var first=new SingleInstance(store.DatabasePath))
  using(var duplicate=new SingleInstance(store.DatabasePath.ToUpperInvariant()))Assert(first.Acquired&&!duplicate.Acquired,"Segunda instancia sobre la misma base");
  using(var next=new SingleInstance(store.DatabasePath))Assert(next.Acquired,"No liberó la instancia");
  var cashierPassword=Guid.NewGuid().ToString();var cashier=store.SaveUser(null,"Caja de prueba","Cajero",true,cashierPassword,password);
  Assert(store.SignIn("Caja de prueba",cashierPassword)&&!store.IsAdmin,"Rol de cajero");
  Reject(()=>store.SaveUser(null,"Sin permiso","Administrador",true,"12345678",cashierPassword),"Creó administrador con clave de cajero");
  Reject(()=>store.SaveProduct(null,"Otro","Sin permiso",100,50,"Prueba",cashierPassword),"Cambió catálogo con clave de cajero");
  Reject(()=>store.SaveSettings(new(),cashierPassword,"Sin permiso"),"Cambió configuración con clave de cajero");
  Assert(store.SignIn(password),"Administrador no disponible");store.SaveUser(cashier,"Caja de prueba","Cajero",false,null,password);
  Assert(!store.SignIn("Caja de prueba",cashierPassword),"Usuario desactivado pudo entrar");
  var administrator=store.Users().Single(x=>x.Role=="Administrador");Reject(()=>store.SaveUser(administrator.Id,administrator.Name,"Cajero",true,null,password),"Eliminó último administrador");
  store.SignOut();Reject(()=>store.OpenShift(0),"Abrió caja sin sesión");store.SignIn(password);
  Console.WriteLine("OK instancia única, cuentas, roles, desactivación, autorización en lógica y último administrador");

  var product=store.Products().First();var second=store.Products()[1];
  var plan=new DiscountPlan(101,new(){{product.Id,new(10)}});
  var discounted=Pricing.Calculate([new(product,1),new(second,1)],0,false,new(),plan);
  Assert(discounted.Sum(x=>x.Discount)==161&&discounted.Sum(x=>x.Net)==1039,"Descuento fijo distribuido a centavos");
  var proportional=Pricing.Calculate([new(product with{Retail=101},1),new(second with{Retail=101},1)],0,false,new(),new(1));
  Assert(proportional.Sum(x=>x.Net)==201&&proportional.Single(x=>x.ProductId==product.Id).Discount==1,"Reparto determinista de un centavo");
  Assert(Pricing.Calculate([new(product,10)],10,false,new(),new(0,new(){{product.Id,new(10)}})).Sum(x=>x.Discount)==0,"Descuento sobre mayoreo deshabilitado");
  Assert(Pricing.Calculate([new(product,10)],10,true,new(),new(0,new(){{product.Id,new(10)}})).Sum(x=>x.Net)==4050,"Orden partida antes de descuento global");
  Reject(()=>Pricing.Calculate([new(product,1)],0,false,new(),new(601)),"Aceptó descuento negativo neto");
  var vm=new SaleViewModel();vm.Cart.Add(new(product,2));vm.ExtraDiscounts=new(1000);vm.Refresh();vm.SetQuantity(product.Id,1);Assert(!vm.ExtraDiscounts.Any&&vm.TotalCents==600&&vm.DiscountNotice.Length>0,"No retiró descuento inválido tras reducir carrito");
  var settings=new PosSettings{BusinessName="Negocio de prueba",Phone="833 000 0000",Address="Dirección de prueba",Pricing=new(12,50000,true),PaperMm=58};store.SaveSettings(settings,password,"Prueba de configuración");
  Assert(store.GetSettings()==settings,"No conservó configuración");
  Assert(Pricing.Calculate([new(product,10)],0,false,settings.Pricing).Sum(x=>x.Net)==6000,"No usa umbral configurable");
  store.SetProductAvailability(product.Id,false,"123456789",password,"Prueba");store.OpenShift(10000);
  Reject(()=>store.Checkout([new(product,1)],0,1000,0,Guid.NewGuid().ToString(),"",""),"Vendió producto desactivado");
  store.SetProductAvailability(product.Id,true,"123456789",password,"Activar");
  Reject(()=>store.SetProductAvailability(second.Id,true,"123456789",password,"Duplicar"),"Código de barras duplicado");
  Assert(store.Products().Single(x=>x.Id==product.Id).Barcode=="123456789","Código de barras perdido");
  Console.WriteLine("OK descuentos fijos, por partida, acumulación, redondeo, cambios de carrito, configuración y disponibilidad");

  // Three pieces worth 18.00, paid 15.01 after discount, with 5.00 on card.
  var token=Guid.NewGuid().ToString();var receipt=store.Checkout([new(product,3)],0,1001,500,token,"Prueba",password,new(299));var sale=store.Sales().First().Folio;
  Assert(ReceiptBrand.Read(receipt).Brand.Name==settings.BusinessName,"No conserva identidad del ticket");
  store.UpdateProduct(product.Id,9999,8888,0,"Precio posterior",password);
  var quote=store.QuoteRefund(sale,[new(product.Id,1)]);Assert(quote.Total==500&&quote.Card==167&&quote.Cash==333,"Vista previa de devolución incorrecta");
  Reject(()=>store.Refund(sale,[new(product.Id,1)],"Sin terminal",password,"return-terminal"),"Aceptó devolución de tarjeta sin confirmar terminal");
  var r1=store.Refund(sale,[new(product.Id,1)],"Devolución parcial",password,"return-1",cardConfirmed:true);
  var r2=store.Refund(sale,[new(product.Id,1)],"Devolución parcial",password,"return-2",cardConfirmed:true);
  var r3=store.Refund(sale,[new(product.Id,1)],"Devolución final",password,"return-3",cardConfirmed:true);
  Assert(r1.Total+r2.Total+r3.Total==1501&&r1.Card+r2.Card+r3.Card==500&&r1.Cash+r2.Cash+r3.Cash==1001,"Devolución parcial no concilia centavos de pago mixto");
  Assert(store.Refund(sale,[new(product.Id,1)],"Reintento",password,"return-1",cardConfirmed:true).Id==r1.Id,"Devolución duplicada");
  Reject(()=>store.Refund(sale,[new(product.Id,1)],"Exceso",password,"return-excess",cardConfirmed:true),"Devolvió más de lo vendido");
  Assert(store.ExpectedCash()==10000&&store.Sales().First().Receipt==receipt,"Devolución alteró venta original o caja");
  store.AddCashMovement(5000,"Agregar cambio","cash-in",password);store.AddCashMovement(-2000,"Retiro a depósito","cash-out",password);
  Assert(store.AddCashMovement(-2000,"Reintento","cash-out",password)>0&&store.ExpectedCash()==13000,"Movimiento duplicado o efectivo incorrecto");
  Reject(()=>store.AddCashMovement(-13001,"Exceso","cash-excess",password),"Permitió retiro superior a caja");
  store.AddExpense(3000,"Material de prueba","","Caja","expense-adjusted");Assert(store.ExpectedCash()==10000,"Gastos no consideran movimientos y devoluciones");
  var cutId=store.CloseShift(10000);var cut=store.Cuts().Single(x=>x.Id==cutId);
  Assert(cut.Total==0&&cut.Expected==100&&cut.Difference==0&&cut.NetAfterExpenses==-30&&cut.Summary.Contains("Movimientos de efectivo: $30.00")&&cut.AdjustmentsCsv.Contains("Retiro a depósito"),"Corte con movimientos/devoluciones incorrecto");
  Assert(store.QuerySales(new()).Total==0&&store.QuerySales(new()).Count==4,"Reporte neto de devoluciones incorrecto");
  Console.WriteLine("OK devoluciones parciales a precio histórico, pagos mixtos, reintentos, movimientos, gastos y conciliación de corte");

  var (rollback,rollbackPassword)=Fixture();rollback.OpenShift(10000);var rollbackProduct=rollback.Products()[0];rollback.Checkout([new(rollbackProduct,2)],0,1200,0,"rollback-sale","","");var rollbackSale=rollback.Sales()[0].Folio;
  Sql(rollback,"CREATE TRIGGER fail_refund BEFORE INSERT ON refund_lines BEGIN SELECT RAISE(ABORT,'test refund failure'); END;");
  Reject(()=>rollback.Refund(rollbackSale,[new(rollbackProduct.Id,1,true)],"Rollback",rollbackPassword,"rollback-return"),"No falló la transacción de prueba");
  Assert(Convert.ToInt64(Sql(rollback,"SELECT COUNT(*) FROM refunds"))==0&&rollback.ExpectedCash()==11200&&rollback.ReturnableLines(rollbackSale)[0].Returned==0&&rollback.Products()[0].Stock==0,"Devolución dejó transacción parcial");
  Sql(rollback,"DROP TRIGGER fail_refund;");var cancelled=rollback.Refund(rollbackSale,[],"Cancelar venta",rollbackPassword,"cancel",cancel:true);Assert(cancelled.Total==1200&&rollback.ExpectedCash()==10000,"Cancelación no concilia");
  Reject(()=>rollback.Refund(rollbackSale,[],"Cancelar de nuevo",rollbackPassword,"cancel-again",cancel:true),"Cancelación repetida");
  Assert(rollback.Cuts().Count==0,"Cancelación cerró caja");
  Console.WriteLine("OK rollback de devolución y cancelación íntegra sin reescribir ventas");

  var draftToken=Guid.NewGuid().ToString();var draft=new SaleDraft(draftToken,store.CurrentUserName!,[new(second.Id,2)],0,"Prueba",DateTimeOffset.Now.ToString("O"),plan);
  store.SaveDraft(draft);store.ParkSale("Pedido de prueba");Assert(store.LoadDraft()==null&&store.PendingSales().Count==1,"No guardó venta pendiente independiente");
  store.ResumeSale(draftToken);Assert(store.PendingSales().Count==0&&store.LoadDraft()?.ExtraDiscounts?.GlobalAmount==101,"No recuperó descuentos del pendiente");
  Reject(()=>store.ResumeSale(draftToken),"Recuperó pendiente duplicado");
  Console.WriteLine("OK múltiples pendientes, recuperación atómica y descuentos sin contraseñas persistidas");

  var (reports,reportsPassword)=Fixture();reports.OpenShift(0);
  // More than the former 1,000-record boundary, including a Mexico date boundary.
  Sql(reports,"""
  WITH RECURSIVE n(x) AS(SELECT 1 UNION ALL SELECT x+1 FROM n WHERE x<1005)
  INSERT INTO sales(token,date,shift,total,cash,card,receipt,cashier)
  SELECT 'report-'||x,'2026-11-16T23:59:00-06:00',1,100,100,0,'Ticket de prueba','Caja A' FROM n;
  INSERT INTO sales(token,date,shift,total,cash,card,receipt,cashier) VALUES('next-day','2026-11-17T06:00:00+00:00',1,200,0,200,'Ticket de prueba','Caja B');
  """);
  var filter=new SalesFilter(new(2026,11,16),new(2026,11,16));var report=reports.QuerySales(filter,10);
  Assert(report.Count==1005&&report.Total==1005&&report.Rows.Count==5,"Paginación o total del periodo completo incorrectos");
  var csv=Path.Combine(reports.DirectoryPath,"periodo.csv");reports.ExportSales(csv,filter);Assert(File.ReadAllLines(csv).Length==1006,"Exportación sigue limitada a mil ventas");
  Assert(reports.QuerySales(new(new(2026,11,17),new(2026,11,17))).Count==1,"Límite diario de México incorrecto");
  Assert(reports.QuerySales(filter with{User="Caja A",Method="Tarjeta"}).Count==0,"Filtros de usuario/método incorrectos");
  var userPassword=Guid.NewGuid().ToString();reports.SaveUser(null,"Caja A","Cajero",true,userPassword,reportsPassword);reports.SignIn("Caja A",userPassword);Assert(reports.QuerySales(new(User:"Caja B")).Count==1005,"Cajero consultó operaciones ajenas");
  Assert(Store.CsvCell("=1+1").StartsWith("\"'="),"CSV permite fórmula en texto");
  Console.WriteLine("OK reportes y CSV de más de mil ventas, paginación, filtros, fecha de México y permisos de cajero");

  var photo=Store.PrepareProductImage(Path.Combine(AppContext.BaseDirectory,"Assets","Packages","77.png"));store.SaveProduct(second.Id,second.Category,second.Name,second.Retail,second.Wholesale,"Foto",password,photo);
  var backup=Path.Combine(store.DirectoryPath,"respaldo-prueba.db");store.Backup(backup);var info=store.InspectBackup(backup);Assert(info.Version==Store.SchemaVersion&&info.Sales==1,"Inspección de respaldo incorrecta");
  store.ParkSale("Pendiente antes de restaurar");
  var beforeSettings=store.GetSettings();store.SaveSettings(beforeSettings with{BusinessName="Cambio posterior"},password,"Cambio posterior");
  var bad=Path.Combine(store.DirectoryPath,"invalido.db");File.WriteAllText(bad,"no es sqlite");Reject(()=>store.RestoreBackup(bad,password),"Restauró archivo corrupto");Assert(store.GetSettings().BusinessName=="Cambio posterior","Respaldo inválido modificó base");
  var future=Path.Combine(store.DirectoryPath,"version-futura.db");store.Backup(future);
  using(var c=new SqliteConnection(new SqliteConnectionStringBuilder{DataSource=future}.ToString())){c.Open();using var q=c.CreateCommand();q.CommandText="PRAGMA user_version=999";q.ExecuteNonQuery();}
  Reject(()=>store.RestoreBackup(future,password),"Aceptó base de una versión futura");
  Reject(()=>store.Backup(store.DatabasePath.ToUpperInvariant()),"Permitió sobrescribir la base con otra combinación de mayúsculas");
  Reject(()=>store.RestoreBackup(backup,"incorrecta"),"Restauró sin autorización");
  store.OpenShift(0);Reject(()=>store.RestoreBackup(backup,password),"Restauró con turno abierto");store.CloseShift(0);
  var preserved=store.RestoreBackup(backup,password);Assert(File.Exists(preserved)&&store.CurrentUserName==null,"Restauración sin copia previa o sin cierre de sesión");
  var reopened=new Store(store.DirectoryPath);Assert(reopened.SignIn(password)&&reopened.GetSettings()==beforeSettings&&reopened.Sales().Count==1&&reopened.Cuts().Count==1,"Restauración no conserva ventas, cortes y configuración");
  Assert(reopened.LoadDraft()?.ExtraDiscounts?.GlobalAmount==101&&File.ReadAllBytes(reopened.Products().Single(x=>x.Id==second.Id).ImagePath).SequenceEqual(photo),"Restauración perdió carrito o foto");
  using(var old=new SqliteConnection(new SqliteConnectionStringBuilder{DataSource=preserved,Mode=SqliteOpenMode.ReadOnly}.ToString())){old.Open();using var q=old.CreateCommand();q.CommandText="SELECT value FROM settings WHERE key='pos'";Assert(((string)q.ExecuteScalar()!).Contains("Cambio posterior"),"Copia previa no contiene estado anterior");}
  Console.WriteLine("OK restauración real de base aislada, validación, autorización, copia previa, ventas, fotos, cuentas, configuración y pendientes");
  var external=Path.Combine(reopened.DirectoryPath,"Copia externa");Directory.CreateDirectory(external);reopened.SaveSettings(reopened.GetSettings() with{BackupDirectory=external},password,"Copia externa");
  var protection=new AutoProtection(reopened,TimeSpan.FromHours(1),TimeSpan.Zero);
  Assert(SpinWait.SpinUntil(()=>protection.Status.StartsWith("Respaldo automático:"),TimeSpan.FromSeconds(10)),"No completó respaldo automático");protection.StopAsync().GetAwaiter().GetResult();
  Assert(Directory.GetFiles(Path.Combine(external,"Diarios"),"*.db").Length==1&&Directory.GetFiles(Path.Combine(external,"Semanales"),"*.db").Length==1,"No creó copias de calendario externas");
  Console.WriteLine("OK protección detenible y respaldos recientes, diarios y semanales en destino externo");
  var unavailable=Path.Combine(reopened.DirectoryPath,"Unidad no disponible");reopened.SaveSettings(reopened.GetSettings() with{BackupDirectory=unavailable},password,"Simulación de unidad ausente");Directory.Delete(unavailable);File.WriteAllText(unavailable,"Ruta de prueba ocupada por un archivo");
  var missingProtection=new AutoProtection(reopened,TimeSpan.FromHours(1),TimeSpan.Zero);
  Assert(SpinWait.SpinUntil(()=>missingProtection.Status.Contains("falló la copia externa"),TimeSpan.FromSeconds(10)),"No avisó de fallo de respaldo externo");missingProtection.StopAsync().GetAwaiter().GetResult();
  var localBackup=Directory.GetFiles(Path.Combine(reopened.DirectoryPath,"Respaldos","Automaticos"),"pinwi-*.db").OrderByDescending(x=>x).First();Assert(reopened.InspectBackup(localBackup).Sales==1,"El fallo externo impidió el respaldo local");
  Console.WriteLine("OK fallo de unidad externa conserva copia local válida y muestra advertencia");
 }
}
