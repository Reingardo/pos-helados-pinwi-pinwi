using Pinwi.Pos;
System.Globalization.CultureInfo.DefaultThreadCurrentCulture=System.Globalization.CultureInfo.GetCultureInfo("es-MX");
System.Windows.FrameworkElement.LanguageProperty.OverrideMetadata(typeof(System.Windows.FrameworkElement),new System.Windows.FrameworkPropertyMetadata(System.Windows.Markup.XmlLanguage.GetLanguage("es-MX")));
if(args.Contains("--docs")){DocumentationScreenshots.Run();return;}
if(args.Contains("--render"))
{
 var thread=new Thread(()=>{
  var app=new System.Windows.Application{ShutdownMode=System.Windows.ShutdownMode.OnExplicitShutdown};var xml=System.Xml.Linq.XDocument.Load(System.IO.Path.Combine(AppContext.BaseDirectory,"Theme.xaml"));var root=new System.Xml.Linq.XElement(System.Xml.Linq.XName.Get("ResourceDictionary","http://schemas.microsoft.com/winfx/2006/xaml/presentation"),new System.Xml.Linq.XAttribute(System.Xml.Linq.XNamespace.Xmlns+"x","http://schemas.microsoft.com/winfx/2006/xaml"),xml.Root!.Elements().First().Elements());app.Resources=(System.Windows.ResourceDictionary)System.Windows.Markup.XamlReader.Parse(root.ToString());
  var output=System.IO.Path.GetFullPath("artifacts/previews");System.IO.Directory.CreateDirectory(output);
  var payment=new PaymentWindow(47000,"Producto sin existencia suficiente registrada. Puedes continuar con el cobro.");
  var input=(System.Windows.Controls.TextBox)payment.FindName("CashInput");var confirm=(System.Windows.Controls.Button)payment.FindName("Confirm");
  input.Text="400";if(confirm.IsEnabled)throw new Exception("UI permite pago incompleto");
  input.Text="500.123";if(confirm.IsEnabled)throw new Exception("UI permite más de dos decimales");
  input.Text="500";if(!confirm.IsEnabled||((System.Windows.Controls.TextBlock)payment.FindName("ChangeLabel")).Text!="$30.00")throw new Exception("UI cambio incorrecto");
  var cardButton=(System.Windows.Controls.Button)payment.FindName("CardMethod");cardButton.RaiseEvent(new System.Windows.RoutedEventArgs(System.Windows.Controls.Button.ClickEvent));if(confirm.IsEnabled)throw new Exception("UI no exige autorización de tarjeta");
  ((System.Windows.Controls.CheckBox)payment.FindName("Authorized")).IsChecked=true;if(!confirm.IsEnabled||payment.CardPaid!=47000||payment.CashReceived!=0)throw new Exception("UI tarjeta incorrecta");
  ((System.Windows.Controls.Button)payment.FindName("CashMethod")).RaiseEvent(new System.Windows.RoutedEventArgs(System.Windows.Controls.Button.ClickEvent));input.Text="500";
  var receipt=new ReceiptWindow("HELADOS PINWI PINWI\n833 393 2616\nFolio 000001 · Vista de ejemplo\nCajero: Ana\n\nPaleta de leche — Fresa\n10 × $20.00 Mayoreo  $200.00\nPaleta artesanal — Gansito\n10 × $22.00 Mayoreo  $220.00\nPinwi de agua — Limón\n10 × $5.00 Mayoreo  $50.00\n\nTOTAL: $470.00\nEfectivo recibido: $500.00\nCambio: $30.00\n\n¡Gracias por tu compra!\nComprobante de venta");
  void Render(System.Windows.Window w,string name,bool close=true){w.Show();w.Dispatcher.Invoke(()=>{},System.Windows.Threading.DispatcherPriority.ApplicationIdle);w.UpdateLayout();var bmp=new System.Windows.Media.Imaging.RenderTargetBitmap((int)w.ActualWidth,(int)w.ActualHeight,96,96,System.Windows.Media.PixelFormats.Pbgra32);bmp.Render(w);var encoder=new System.Windows.Media.Imaging.PngBitmapEncoder();encoder.Frames.Add(System.Windows.Media.Imaging.BitmapFrame.Create(bmp));using var stream=System.IO.File.Create(System.IO.Path.Combine(output,name));encoder.Save(stream);if(close)w.Close();}
  Render(payment,"cobro.png");Render(receipt,"ticket.png");Render(new StoreWelcomeWindow(System.IO.Path.Combine(System.IO.Path.GetTempPath(),"PinwiWelcomePreview")),"store-bienvenida.png");
  var doc=ReceiptBuilder.Create("TOTAL: $470.00\nCambio: $30.00",219);if(doc.PageWidth!=219)throw new Exception("Ancho 58 mm incorrecto");
  var previewStore=new Store(System.IO.Path.Combine(System.IO.Path.GetTempPath(),"PinwiPreview",Guid.NewGuid().ToString("N")));var previewPassword=Guid.NewGuid().ToString();previewStore.CreateAdmin("Ana",previewPassword);previewStore.SignIn(previewPassword);previewStore.OpenShift(10000);
  var sampleCart=new List<CartItem>{new(previewStore.Products().First(x=>x.Id==42),10),new(previewStore.Products().First(x=>x.Id==51),10),new(previewStore.Products().First(x=>x.Id==1),10)};
  previewStore.Checkout(sampleCart,0,50000,0,Guid.NewGuid().ToString(),"","");
  previewStore.AddExpense(7000,"Compra de vasos","Proveedor de prueba","Caja",Guid.NewGuid().ToString());previewStore.AddExpense(6000,"Pago de apoyo","Personal de prueba","Caja",Guid.NewGuid().ToString());
  var expenseForm=new ExpenseWindow(previewStore);((System.Windows.Controls.TextBox)expenseForm.FindName("AmountInput")).Text="250.00";((System.Windows.Controls.TextBox)expenseForm.FindName("ConceptInput")).Text="Compra de material";((System.Windows.Controls.TextBox)expenseForm.FindName("RecipientInput")).Text="Proveedor";Render(expenseForm,"registrar-gasto.png");
  previewStore.CloseShift(44000);
  var productForm=new ProductWindow(previewStore,previewStore.Products().First());Render(productForm,"editar-producto.png");
  var newProductForm=new ProductWindow(previewStore);Render(newProductForm,"nuevo-producto.png");
  Render(new SettingsWindow(previewStore),"configuracion.png");Render(new UserWindow(previewStore),"usuario.png");Render(new LoginWindow(previewStore),"sesion.png");Render(new ReturnWindow(previewStore,previewStore.Sales()[0].Folio),"devolucion.png");var discountVm=new SaleViewModel();discountVm.Cart.AddRange(sampleCart);discountVm.Refresh();Render(new DiscountWindow(previewStore,discountVm),"descuentos.png");
  var main=new MainWindow(previewStore){Height=768};var previewFlags=System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic;main.Loaded-=(System.Windows.RoutedEventHandler)Delegate.CreateDelegate(typeof(System.Windows.RoutedEventHandler),main,typeof(MainWindow).GetMethod("LoadedWindow",previewFlags)!);typeof(MainWindow).GetField("ready",previewFlags)!.SetValue(main,true);typeof(MainWindow).GetMethod("Reload",previewFlags)!.Invoke(main,null);
  var previewVm=(SaleViewModel)typeof(MainWindow).GetField("vm",previewFlags)!.GetValue(main)!;previewVm.Cart.AddRange(sampleCart);previewVm.Refresh();
  Render(main,"venta.png",false);typeof(MainWindow).GetMethod("OpenQuickSales",previewFlags)!.Invoke(main,new object[]{main,new System.Windows.RoutedEventArgs()});previewVm.Cart.Clear();typeof(MainWindow).GetMethod("AddProduct",previewFlags)!.Invoke(main,new object[]{new System.Windows.Controls.Button{Tag=previewStore.Products().Single(p=>p.QuickGroup=="bolis")},new System.Windows.RoutedEventArgs()});previewVm.SetQuantity(previewVm.Cart.Single().Product.Id,10);if(previewVm.TotalCents!=5000)throw new Exception("UI rápida no agrega gama o recalcula mayoreo");Render(main,"cobro-rapido.png",false);previewVm.Cart.Clear();previewVm.Cart.AddRange(sampleCart);previewVm.Refresh();typeof(MainWindow).GetField("activeCategory",previewFlags)!.SetValue(main,"Paquetes");typeof(MainWindow).GetMethod("Filter",previewFlags)!.Invoke(main,null);Render(main,"paquetes.png",false);((System.Windows.Controls.DockPanel)main.Content).Children.OfType<System.Windows.Controls.TabControl>().Single().SelectedIndex=2;Render(main,"cortes.png",false);((System.Windows.Controls.TabControl)main.FindName("CashViews")).SelectedIndex=1;Render(main,"gastos-dia.png");
  previewStore.SetProductAvailability(1,false,"",previewPassword,"Prueba de pendiente desactivado");previewStore.SaveDraft(new(Guid.NewGuid().ToString(),previewStore.CurrentUserName!,[new(1,2)],0,"",DateTimeOffset.Now.ToString("O")));
  var recoveryMain=new MainWindow(previewStore);recoveryMain.Loaded-=(System.Windows.RoutedEventHandler)Delegate.CreateDelegate(typeof(System.Windows.RoutedEventHandler),recoveryMain,typeof(MainWindow).GetMethod("LoadedWindow",previewFlags)!);typeof(MainWindow).GetField("ready",previewFlags)!.SetValue(recoveryMain,true);typeof(MainWindow).GetMethod("Reload",previewFlags)!.Invoke(recoveryMain,null);typeof(MainWindow).GetMethod("RestoreCart",previewFlags)!.Invoke(recoveryMain,null);
  var recoveredVm=(SaleViewModel)typeof(MainWindow).GetField("vm",previewFlags)!.GetValue(recoveryMain)!;if(recoveredVm.Cart.Single().Quantity!=2||recoveredVm.Cart.Single().Product.Active||!recoveredVm.Lines.Single().StockWarning.Contains("desactivado"))throw new Exception("El pendiente perdió un producto desactivado");Render(recoveryMain,"pendiente-desactivado.png");Console.WriteLine("OK UI conserva productos desactivados en pendientes y pide revisión");
  Console.WriteLine("Vistas renderizadas en "+output);
 });thread.SetApartmentState(ApartmentState.STA);thread.Start();thread.Join();return;
}
if(args.Contains("--ui"))
{
 var thread=new Thread(()=>{
  var app=new System.Windows.Application{ShutdownMode=System.Windows.ShutdownMode.OnExplicitShutdown}; var xml=System.Xml.Linq.XDocument.Load(System.IO.Path.Combine(AppContext.BaseDirectory,"Theme.xaml")); var root=new System.Xml.Linq.XElement(System.Xml.Linq.XName.Get("ResourceDictionary","http://schemas.microsoft.com/winfx/2006/xaml/presentation"), new System.Xml.Linq.XAttribute(System.Xml.Linq.XNamespace.Xmlns+"x","http://schemas.microsoft.com/winfx/2006/xaml"),xml.Root!.Elements().First().Elements()); app.Resources=(System.Windows.ResourceDictionary)System.Windows.Markup.XamlReader.Parse(root.ToString());
  var isolated=new Store(System.IO.Path.Combine(System.IO.Path.GetTempPath(),"PinwiPreview",Guid.NewGuid().ToString("N")));var isolatedPassword=Guid.NewGuid().ToString();isolated.CreateAdmin("Prueba visual",isolatedPassword);isolated.SignIn(isolatedPassword);isolated.OpenShift(50000);var isolatedProduct=isolated.Products().Single(x=>x.QuickGroup=="bolis");isolated.Checkout([new(isolatedProduct,10)],0,10000,0,Guid.NewGuid().ToString(),"","");
  var w=new MainWindow(isolated){Title="PRUEBA AISLADA · Helados Pinwi Pinwi"};var flags=System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic;
  var handler=(System.Windows.RoutedEventHandler)Delegate.CreateDelegate(typeof(System.Windows.RoutedEventHandler),w,typeof(MainWindow).GetMethod("LoadedWindow",flags)!);
  w.Loaded-=handler;
  typeof(MainWindow).GetField("ready",flags)!.SetValue(w,true);
  typeof(MainWindow).GetMethod("Reload",flags)!.Invoke(w,null);
  ((System.Windows.Controls.CheckBox)w.FindName("AutoWhatsApp")).IsChecked=false;
  w.Closed+=(_,_)=>System.Windows.Threading.Dispatcher.CurrentDispatcher.InvokeShutdown(); w.Show();System.Windows.Threading.Dispatcher.Run();
 });thread.SetApartmentState(ApartmentState.STA);thread.Start();thread.Join();return;
}
var lemon = new Product(1,"Pinwis de agua","Limón",600,500,100,"");
var pineapple = lemon with {Id=2,Name="Piña"};
void Check(string name,long expected,List<CartItem> cart,decimal discount=0,bool stack=false){var result=Pricing.Calculate(cart,discount,stack).Sum(x=>x.Net);if(result!=expected)throw new Exception($"{name}: {result} != {expected}");Console.WriteLine("OK "+name);}
Check("9 piezas",5400,[new(lemon,9)]);
Check("10 piezas",5000,[new(lemon,10)]);
Check("Sabores de la misma gama activan mayoreo",5000,[new(lemon,5),new(pineapple,5)]);
Check("Renglones repetidos",5000,[new(lemon,4),new(lemon,6)]);
Check("399.99 sin mayoreo",39999,[new(lemon with {Retail=39999,Wholesale=30000},1)]);
Check("400 activa mayoreo y permanece",30000,[new(lemon with {Retail=40000,Wholesale=30000},1)]);
Check("Descuento menudeo",4860,[new(lemon,9)],10);
Check("Mayoreo excluido de descuento",5000,[new(lemon,10)],10);
Check("Acumulación habilitada",4500,[new(lemon,10)],10,true);
Check("Mayoreo en todos los sabores de una gama",5500,[new(lemon,10),new(pineapple,1)],10);
Check("Redondeo a centavo",91,[new(lemon with{Retail=101,Wholesale=100},1)],10);
var vm=new SaleViewModel();vm.Cart.Add(new(lemon,1));vm.SetQuantity(1,20);if(vm.TotalCents!=10000)throw new Exception("Edición a mayoreo");vm.SetQuantity(1,9);if(vm.TotalCents!=5400)throw new Exception("Edición a menudeo");vm.SetQuantity(1,0);if(vm.Cart.Count!=0)throw new Exception("Eliminar cantidad cero");
Console.WriteLine("OK edición directa de cantidades y recálculo");
Console.WriteLine("11 pruebas de precios aprobadas.");
var testFolder=System.IO.Path.Combine(System.IO.Path.GetTempPath(),"PinwiTests",Guid.NewGuid().ToString("N"));
var store=new Store(testFolder);
if(store.Products().Count!=88)throw new Exception("Catálogo incompleto");
if(new Store(testFolder).Products().Count!=88)throw new Exception("Catálogo duplicado");
var packs=store.Products().Where(x=>x.Category=="Paquetes").ToList();
if(packs.Count!=5 || !packs.Select(x=>x.Retail).SequenceEqual(new long[]{50000,48000,69500,44500,109000}))throw new Exception("Paquetes o precios incorrectos");
foreach(var pack in packs){Check(pack.Name,pack.Retail,[new(pack,1)]);Check("Diez paquetes",pack.Retail*10,[new(pack,10)]);if(!System.IO.File.Exists(pack.ImagePath))throw new Exception("Falta imagen de paquete");}
var upgradeFolder=System.IO.Path.Combine(System.IO.Path.GetTempPath(),"PinwiTests",Guid.NewGuid().ToString("N"));
var upgradeStore=new Store(upgradeFolder);
using(var db=new Microsoft.Data.Sqlite.SqliteConnection("Data Source="+upgradeStore.DatabasePath)){db.Open();using var q=db.CreateCommand();q.CommandText="DELETE FROM products WHERE category='Paquetes'; UPDATE products SET retail=777,wholesale=555 WHERE id=1; PRAGMA user_version=4;";q.ExecuteNonQuery();}
upgradeStore=new Store(upgradeFolder);
if(upgradeStore.Products().Count!=88 || upgradeStore.Products()[0].Retail!=777 || new Store(upgradeFolder).Products().Count!=88 || !System.IO.Directory.GetFiles(System.IO.Path.Combine(upgradeFolder,"Respaldos"),"antes-v8-*.db").Any())throw new Exception("Migración de paquetes no conserva precios, respaldo o idempotencia");
Console.WriteLine("OK migración antigua a v8: cinco paquetes y siete gamas, precios previos y respaldo conservados");
var password=Guid.NewGuid().ToString();
store.CreateAdmin("Prueba",password);if(!store.SignIn(password))throw new Exception("No inició sesión");
if(!store.Authenticate(password)||store.Authenticate("incorrecta"))throw new Exception("Autenticación inválida");
store.UpdateProduct(1,600,500,20,"Inventario de prueba",password);
store.OpenShift(10000);
var p=store.Products()[0];var token=Guid.NewGuid().ToString();
var receipt=store.Checkout([new(p,10)],0,10000,0,token,"","");
if(store.ExpectedCash()!=15000||store.Products()[0].Stock!=10)throw new Exception("Caja o inventario incorrectos");
if(store.Checkout([new(p,10)],0,10000,0,token,"","")!=receipt||store.Products()[0].Stock!=10)throw new Exception("Venta duplicada");
store.Checkout([new(p,11)],0,10000,0,Guid.NewGuid().ToString(),"","");
if(store.Sales().Count!=2||store.Products()[0].Stock!=0)throw new Exception("Debe permitir venta con existencia insuficiente sin existencias negativas");
if(store.Sales().Any(x=>x.Cashier!="Prueba")||!receipt.Contains("Cajero: Prueba"))throw new Exception("No guardó usuario real");
try{store.Checkout([new(p,1)],0,0,0,Guid.NewGuid().ToString(),"","");throw new Exception("Aceptó pago incompleto");}catch(Exception ex)when(ex.Message.StartsWith("El pago no cubre")){}
if(store.Sales().Count!=2)throw new Exception("Guardó pago incompleto");
store.CloseShift(20500);if(store.Shift()!=null)throw new Exception("Corte incompleto");
store.Backup(System.IO.Path.Combine(testFolder,"backup.db"));
Console.WriteLine("OK integración: catálogo, autenticación, venta, cambio, inventario, idempotencia, rollback, corte y respaldo. Datos aislados: "+testFolder);



if(!Payments.Suggestions(47000).Contains(50000))throw new Exception("Falta sugerencia de 500");
if(Payments.Evaluate(47000,50000,0).Change!=3000)throw new Exception("Cambio incorrecto");
if(Payments.Evaluate(47000,40000,0).Valid)throw new Exception("Pago insuficiente aceptado");
if(Payments.Evaluate(47000,0,50000).Valid)throw new Exception("Tarjeta excesiva aceptada");
if(Payments.Evaluate(47000,30000,20000).Change!=3000)throw new Exception("Cambio mixto incorrecto");
if(Payments.Evaluate(47000,0,47000).Change!=0)throw new Exception("Cambio tarjeta incorrecto");
if(!Payments.Evaluate(0,0,0).Valid)throw new Exception("Total cero inválido");
Console.WriteLine("OK sugerencias, cambio, pago insuficiente, tarjeta y pago mixto");
var migrationFolder=System.IO.Path.Combine(testFolder,"migration");System.IO.Directory.CreateDirectory(migrationFolder);
var migrationDb=System.IO.Path.Combine(migrationFolder,"pinwi.db");store.Backup(migrationDb);
using(var oldDb=new Microsoft.Data.Sqlite.SqliteConnection(new Microsoft.Data.Sqlite.SqliteConnectionStringBuilder{DataSource=migrationDb}.ToString()))
{
 oldDb.Open();using var command=oldDb.CreateCommand();command.CommandText="ALTER TABLE sales DROP COLUMN cashier; PRAGMA user_version=1;";command.ExecuteNonQuery();
}
var migrated=new Store(migrationFolder);
if(migrated.Sales().Count!=2||migrated.Sales().Any(x=>x.Cashier!="No registrado")||migrated.Products().Count!=88)throw new Exception("Migración perdió datos o atribuyó un cajero falso");
if(System.IO.Directory.GetFiles(System.IO.Path.Combine(migrationFolder,"Respaldos"),"*.db").Length!=1)throw new Exception("Migración sin respaldo previo");
if(new Store(migrationFolder).Sales().Count!=2)throw new Exception("Migración no idempotente");
Console.WriteLine("OK migración v1 a v8 con respaldo, ventas históricas y usuario no registrado");
var draftToken=Guid.NewGuid().ToString();
store.SaveDraft(new(draftToken,"Prueba",[new(1,7)],10,"Cliente frecuente",DateTimeOffset.Now.ToString("O")));
var reopened=new Store(testFolder);reopened.SignIn(password);var recovered=reopened.LoadDraft();
if(recovered?.Token!=draftToken||recovered.Lines.Single().Quantity!=7||recovered.Discount!=10)throw new Exception("No recuperó el carrito");
store.OpenShift(1000);var cashCutId=store.Shift()!.Value;
var small=store.Products()[0];store.Checkout([new(small,7)],10,5000,0,draftToken,"Cliente frecuente",password);
if(reopened.LoadDraft()!=null)throw new Exception("Recuperó una venta ya cobrada");
store.SaveDraft(new(draftToken,"Prueba",[new(1,7)],10,"Cliente frecuente",DateTimeOffset.Now.ToString("O")));
if(reopened.LoadDraft()!=null)throw new Exception("Recreó un borrador ya cobrado");
var expected=store.ExpectedCash();var cutId=store.CloseShift(expected+100);
var cut=store.Cuts().First(x=>x.Id==cutId);
if(cut.Id!=cashCutId||cut.Cashier!="Prueba"||cut.Difference!=1m||cut.Total!=37.8m||cut.CountedPesos!=(expected+100)/100m)throw new Exception("Corte incorrecto");
if(!cut.Summary.Contains("VENTAS DEL DÍA")||!cut.Summary.Contains("Operaciones: 3")||!cut.Csv.Contains("Prueba")||!cut.Csv.Contains("37.80"))throw new Exception("Reporte diario incompleto");
var folder=store.WriteCutFiles(cut);if(!System.IO.File.Exists(System.IO.Path.Combine(folder,"resumen.txt"))||!System.IO.File.Exists(System.IO.Path.Combine(folder,"ventas-del-dia.csv")))throw new Exception("No exportó el corte");
var same=new Store(testFolder).Cuts().First(x=>x.Id==cutId);if(same.Summary!=cut.Summary||same.Csv!=cut.Csv)throw new Exception("No conservó el reporte histórico");
store.ConfirmCutSent(cutId);if(!store.Cuts().First(x=>x.Id==cutId).Status.StartsWith("Enviado"))throw new Exception("No registró confirmación manual");
Console.WriteLine("OK recuperación, venta sin borrador residual, corte, acumulado diario, nota, CSV e historial persistente");
using(var automatic=new AutoProtection(store,TimeSpan.FromMilliseconds(150),TimeSpan.FromMilliseconds(10)))
{
 automatic.Request();var deadline=DateTime.UtcNow.AddSeconds(10);var autoFolder=System.IO.Path.Combine(testFolder,"Respaldos","Automaticos");
 while((!System.IO.Directory.Exists(autoFolder)||System.IO.Directory.GetFiles(autoFolder,"*.db").Length==0)&&DateTime.UtcNow<deadline)Thread.Sleep(100);
 if(!System.IO.Directory.Exists(autoFolder)||System.IO.Directory.GetFiles(autoFolder,"*.db").Length==0)throw new Exception("No creó respaldo automático");
 var initialCopies=System.IO.Directory.GetFiles(autoFolder,"*.db").Length;while(System.IO.Directory.GetFiles(autoFolder,"*.db").Length<=initialCopies&&DateTime.UtcNow<deadline)Thread.Sleep(50);
 if(System.IO.Directory.GetFiles(autoFolder,"*.db").Length<=initialCopies)throw new Exception("No respaldó sin modificaciones");
 var backupFile=System.IO.Directory.GetFiles(autoFolder,"*.db").First();using var connection=new Microsoft.Data.Sqlite.SqliteConnection(new Microsoft.Data.Sqlite.SqliteConnectionStringBuilder{DataSource=backupFile}.ToString());connection.Open();using var command=connection.CreateCommand();command.CommandText="PRAGMA integrity_check";if((string?)command.ExecuteScalar()!="ok")throw new Exception("Respaldo inconsistente");
}
Console.WriteLine("OK respaldo automático consistente con SQLite WAL");
var expenseStore=new Store(System.IO.Path.Combine(testFolder,"expenses"));expenseStore.CreateAdmin("Caja de prueba",password);expenseStore.SignIn(password);
try{expenseStore.AddExpense(10000,"Pago proveedor","","Caja",Guid.NewGuid().ToString());throw new Exception("Aceptó gasto con caja cerrada");}catch(Exception ex)when(ex.Message.StartsWith("Abre caja")){}
expenseStore.UpdateProduct(1,500000,500000,0,"Prueba de gastos",password);expenseStore.OpenShift(10000);expenseStore.Checkout([new(expenseStore.Products()[0],1)],0,500000,0,Guid.NewGuid().ToString(),"","");
var firstExpenseToken=Guid.NewGuid().ToString();var firstExpense=expenseStore.AddExpense(70000,"Compra de material","Proveedor de prueba","Caja",firstExpenseToken);expenseStore.AddExpense(60000,"Pago de salario","Personal de prueba","Caja",Guid.NewGuid().ToString());
if(expenseStore.AddExpense(70000,"Compra de material","Proveedor de prueba","Caja",firstExpenseToken)!=firstExpense||expenseStore.Expenses(DateTimeOffset.Now).Count!=2)throw new Exception("Gasto duplicado");
if(expenseStore.ExpectedCash()!=380000)throw new Exception("No restó gastos al efectivo esperado");
try{expenseStore.AddExpense(400000,"Gasto excesivo","","Caja",Guid.NewGuid().ToString());throw new Exception("Aceptó retiro superior al efectivo");}catch(Exception ex)when(ex.Message.StartsWith("El gasto supera")){}
try{expenseStore.AddExpense(0,"Gasto cero","","Caja",Guid.NewGuid().ToString());throw new Exception("Aceptó gasto cero");}catch(Exception ex)when(ex.Message.StartsWith("Indica un importe")){}
var expenseCutId=expenseStore.CloseShift(380000);var expenseCut=expenseStore.Cuts().First(x=>x.Id==expenseCutId);
if(expenseCut.Total!=5000m||expenseCut.ExpensesPesos!=1300m||expenseCut.NetAfterExpenses!=3700m||expenseCut.Expected!=3800m||expenseCut.Difference!=0)throw new Exception("Ventas menos gastos incorrectos");
if(!expenseCut.Summary.Contains("Ventas: $5,000.00")||!expenseCut.Summary.Contains("Menos gastos: $1,300.00")||!expenseCut.Summary.Contains("Resultado del día: $3,700.00")||!expenseCut.Summary.Contains("Compra de material")||!expenseCut.Summary.Contains("Pago de salario"))throw new Exception("Nota de WhatsApp incompleta");
var expenseFiles=expenseStore.WriteCutFiles(expenseCut);if(!System.IO.File.ReadAllText(System.IO.Path.Combine(expenseFiles,"gastos-del-dia.csv")).Contains("Proveedor de prueba"))throw new Exception("CSV de gastos incompleto");
expenseStore.OpenShift(0);expenseStore.AddExpense(20000,"Pago externo","Proveedor externo","Externo",Guid.NewGuid().ToString());if(expenseStore.ExpectedCash()!=0)throw new Exception("Gasto externo alteró efectivo de caja");
var nextCutId=expenseStore.CloseShift(0);var nextCut=expenseStore.Cuts().First(x=>x.Id==nextCutId);if(!nextCut.Summary.Contains("Resultado del día: $3,500.00")||nextCut.Difference!=0)throw new Exception("No acumuló gastos de varios turnos");
if(expenseStore.Cuts().First(x=>x.Id==expenseCutId).Summary!=expenseCut.Summary)throw new Exception("Cambió un corte anterior");
var restoredExpenses=new Store(System.IO.Path.Combine(testFolder,"expenses"));if(restoredExpenses.Expenses(DateTimeOffset.Now).Count!=3||restoredExpenses.Cuts().Count!=2)throw new Exception("No conservó gastos y cortes");
Console.WriteLine("OK gastos: 5000 - 1300 = 3700, conceptos, caja vs externos, idempotencia, validaciones, CSV y cortes inmutables");

var catalogFolder=System.IO.Path.Combine(System.IO.Path.GetTempPath(),"PinwiTests",Guid.NewGuid().ToString("N"));
var catalogStore=new Store(catalogFolder);var catalogPassword=Guid.NewGuid().ToString();catalogStore.CreateAdmin("Catálogo",catalogPassword);catalogStore.SignIn(catalogPassword);
var photoBytes=Store.PrepareProductImage(System.IO.Path.Combine(AppContext.BaseDirectory,"Assets","Packages","77.png"));
var newId=catalogStore.SaveProduct(null,"Especiales","Producto de prueba",2500,2200,"Alta",catalogPassword,photoBytes);
var created=catalogStore.Products().Single(p=>p.Id==newId);
if(created.Stock!=0||created.Retail!=2500||created.Category!="Especiales"||!System.IO.File.Exists(created.ImagePath))throw new Exception("Alta de producto o imagen incorrecta");
var duplicateRejected=false;try{catalogStore.SaveProduct(null,"Especiales","Producto de prueba",2500,2200,"Alta",catalogPassword);}catch{duplicateRejected=true;}if(!duplicateRejected)throw new Exception("Permitió duplicar producto");
var unauthorizedRejected=false;try{catalogStore.SaveProduct(newId,"Especiales","Sin permiso",1,1,"Cambio","incorrecta");}catch{unauthorizedRejected=true;}if(!unauthorizedRejected)throw new Exception("Edición sin autorización");
catalogStore.UpdateProduct(newId,2500,2200,7,"Existencias",catalogPassword);
catalogStore.SaveProduct(newId,"Nueva categoría","Producto editado",3000,2600,"Edición",catalogPassword);
var reopenedCatalog=new Store(catalogFolder);var edited=reopenedCatalog.Products().Single(p=>p.Id==newId);
if(edited.Stock!=7||edited.Retail!=3000||edited.Name!="Producto editado"||!System.IO.File.ReadAllBytes(edited.ImagePath).SequenceEqual(photoBytes))throw new Exception("Edición perdió datos o foto");
var restoredFolder=System.IO.Path.Combine(System.IO.Path.GetTempPath(),"PinwiTests",Guid.NewGuid().ToString("N"));System.IO.Directory.CreateDirectory(restoredFolder);catalogStore.Backup(System.IO.Path.Combine(restoredFolder,"pinwi.db"));
var restored=new Store(restoredFolder).Products().Single(p=>p.Id==newId);
if(!restored.ImagePath.StartsWith(restoredFolder)||!System.IO.File.ReadAllBytes(restored.ImagePath).SequenceEqual(photoBytes))throw new Exception("El respaldo no recupera la imagen de forma independiente");
Check("Cobro de producto nuevo",3000,[new(restored,1)]);
Console.WriteLine("OK alta, edición, autorización, duplicados, existencias conservadas, persistencia y foto restaurada desde respaldo independiente");

var quickFolder=System.IO.Path.Combine(System.IO.Path.GetTempPath(),"PinwiTests",Guid.NewGuid().ToString("N"));var quickStore=new Store(quickFolder);var quickPassword=Guid.NewGuid().ToString();quickStore.CreateAdmin("Caja rápida",quickPassword);quickStore.SignIn(quickPassword);
var quickProducts=quickStore.Products().Where(p=>p.IsQuick).ToList();
if(quickProducts.Count!=7||quickProducts.Any(p=>p.Name.Contains("Esquimal")||!p.QuickReady))throw new Exception("Gamas rápidas incorrectas");
var quickBolis=quickProducts.Single(p=>p.QuickGroup=="bolis");var quickPaletas=quickProducts.Single(p=>p.QuickGroup=="categoria:Paletas de agua");
Check("Rápido: nueve bolis",5400,[new(quickBolis,9)]);
Check("Rápido: diez bolis",5000,[new(quickBolis,10)]);
Check("Gamas diferentes no suman diez",12000,[new(quickBolis,5),new(quickPaletas,5)]);
Check("Normal: agua y leche de la misma gama",5000,[new(lemon,5),new(lemon with{Id=9,Category="Pinwis de leche",Name="Coco"},5)]);
Check("Carrito mixto con descuento elegible",13100,[new(quickBolis,10),new(quickPaletas,5)],10);
Check("Normal y rápido suman la misma gama",5000,[new(lemon,5),new(quickBolis,5)]);
Check("Esquimales distintos conservan su presentación",24500,[new(new Product(73,"Esquimales","Grande",2200,2000,0,""),5),new(new Product(76,"Esquimales","Nuez",2700,2500,0,""),5)]);
quickStore.UpdateProduct(1,600,500,20,"Existencias para comprobación",quickPassword);quickStore.OpenShift(0);
var quickToken=Guid.NewGuid().ToString();quickStore.SaveDraft(new(quickToken,"Caja rápida",[new(quickBolis.Id,10)],0,"",DateTimeOffset.Now.ToString("O")));
var quickReopened=new Store(quickFolder);quickReopened.SignIn(quickPassword);if(quickReopened.LoadDraft()?.Lines.Single().ProductId!=quickBolis.Id)throw new Exception("Borrador rápido perdido");
var quickReceipt=quickStore.Checkout([new(quickBolis,10)],0,10000,0,quickToken,"","");
if(!quickReceipt.Contains("Cobro rápido — Bolis de agua y leche")||!quickReceipt.Contains("Cambio: $50.00")||quickStore.Products().Single(p=>p.Id==1).Stock!=20||quickStore.ExpectedCash()!=5000||quickStore.LoadDraft()!=null)throw new Exception("Venta rápida, caja, inventario o borrador incorrectos");
quickStore.Checkout([new(quickBolis,10)],0,10000,0,quickToken,"","");if(quickStore.Sales().Count!=1)throw new Exception("Venta rápida duplicada");
var quickCut=quickStore.CloseShift(5000);if(quickStore.Cuts().Single(p=>p.Id==quickCut).Total!=50m)throw new Exception("Corte rápido incorrecto");
Console.WriteLine("OK cobro rápido por gama, borrador, ticket sin sabores, cambio, caja, corte e inventario por sabor intacto");

var wholesaleVm=new SaleViewModel();wholesaleVm.Cart.AddRange([new(lemon,5),new(pineapple,5)]);if(wholesaleVm.TotalCents!=5000)throw new Exception("Mayoreo entre sabores");wholesaleVm.SetQuantity(pineapple.Id,4);if(wholesaleVm.TotalCents!=5400)throw new Exception("No regresó a menudeo al bajar de diez");
var otherGroup=quickPaletas with{Retail=34000,Wholesale=30000};
Check("Umbral global exacto con varias gamas",35000,[new(lemon,5),new(pineapple,5),new(otherGroup,1)]);
Check("Al quitar consumo conserva solo gama con diez",7700,[new(lemon,5),new(pineapple,5),new(quickPaletas with{Retail=2700,Wholesale=2500},1)]);
Console.WriteLine("OK recálculo por gama al quitar cantidades y al bajar del umbral global");
ImprovementsTests.Run();
StoreDistributionTests.Run();
