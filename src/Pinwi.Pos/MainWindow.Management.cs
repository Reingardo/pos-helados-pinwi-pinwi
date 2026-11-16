using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Win32;
namespace Pinwi.Pos;
public partial class MainWindow
{
 int reportPage;long reportCount;PosSettings settings=new();bool working;bool allowClose;bool closing;
 async void ClosingWindow(object? sender,System.ComponentModel.CancelEventArgs e)
 {
  if(allowClose)return;
  e.Cancel=true;
  if(closing)return;
  if(working){Status.Text="Espera a que termine la operación antes de cerrar.";return;}
  closing=true;IsEnabled=false;
  if(protection!=null)await protection.StopAsync();
  allowClose=true;Close();
 }
 async Task RunAsync(Func<Task> action)
 {
  if(working)return;working=true;IsEnabled=false;
  try{await action();}
  catch(Exception ex){LogError(ex);MessageBox.Show(this,ex is Microsoft.Data.Sqlite.SqliteException?"No se pudo completar la operación. Revisa los datos e intenta nuevamente. Tu información se conserva.":ex.Message,"Revisa la operación",MessageBoxButton.OK,MessageBoxImage.Warning);}
  finally{working=false;IsEnabled=true;}
 }
 void LogError(Exception ex)
 {
  try{var log=Path.Combine(store.DirectoryPath,"errores.log");if(File.Exists(log)&&new FileInfo(log).Length>2*1024*1024)File.Move(log,log+".anterior",true);File.AppendAllText(log,$"{DateTimeOffset.Now:O} {ex.GetType().Name}: {ex.Message}\n{ex.StackTrace}\n");}catch(Exception){}
 }
 void ReadSettings(){settings=store.GetSettings();vm.Policy=settings.Pricing;QuickNote.Text=$"Mayoreo desde {vm.Policy.Quantity} piezas de la misma gama o {vm.Policy.GlobalCents/100m:C} a menudeo. Precios independientes por gama.";}
 SalesFilter SelectedReportFilter()=>new(ReportFrom.SelectedDate,ReportThrough.SelectedDate,(ReportUser.SelectedItem as string) is string user&&user!="Todos"?user:"",ReportMethod.SelectedItem as string??"Todos");
 async Task LoadReport()
 {
  var filter=SelectedReportFilter();var page=reportPage;
  var result=await Task.Run(()=>store.QuerySales(filter,page));reportCount=result.Count;
  if(page>0&&result.Rows.Count==0){reportPage=0;await LoadReport();return;}
  History.ItemsSource=result.Rows;SalesTotal.Text=$"{result.Count:N0} operaciones · Neto {result.Total:C} · Efectivo {result.Cash:C} · Tarjeta {result.Card:C}";
  ReportPage.Text=$"Página {page+1} de {Math.Max(1,(result.Count+99)/100)}";
  PreviousPage.IsEnabled=page>0;NextPage.IsEnabled=(page+1)*100<result.Count;
  ProductStats.ItemsSource=await Task.Run(()=>store.ProductReport(filter));
 }
 async void ApplyReport(object sender,RoutedEventArgs e){reportPage=0;await RunAsync(LoadReport);}
 async void PreviousReport(object sender,RoutedEventArgs e){if(reportPage==0)return;reportPage--;await RunAsync(LoadReport);}
 async void NextReport(object sender,RoutedEventArgs e){if((reportPage+1)*100>=reportCount)return;reportPage++;await RunAsync(LoadReport);}
 async void MainTabChanged(object sender,SelectionChangedEventArgs e)
 {
  if(!ready||e.Source!=MainTabs||working)return;
  if(MainTabs.SelectedIndex==3)await RunAsync(LoadReport);
  if(MainTabs.SelectedIndex==5)Run(()=>{if(!store.IsAdmin)throw new Exception("Esta sección requiere una sesión de administrador.");UsersGrid.ItemsSource=store.Users();});
 }
 async void ExportProductReport(object sender,RoutedEventArgs e)=>await RunAsync(async()=>{var dialog=new SaveFileDialog{Filter="CSV|*.csv",FileName="productos-periodo.csv"};if(dialog.ShowDialog(this)!=true)return;var filter=SelectedReportFilter();await Task.Run(()=>store.ExportProducts(dialog.FileName,filter));MessageBox.Show(this,"Reporte de productos guardado.");});
 void CreateUser(object sender,RoutedEventArgs e)=>Run(()=>{if(!store.IsAdmin)throw new Exception("Inicia sesión como administrador.");if(new UserWindow(store){Owner=this}.ShowDialog()==true)UsersGrid.ItemsSource=store.Users();});
 void EditUser(object sender,RoutedEventArgs e)=>Run(()=>{if(!store.IsAdmin)throw new Exception("Inicia sesión como administrador.");if(UsersGrid.SelectedItem is not UserAccount user)throw new Exception("Selecciona un usuario.");if(new UserWindow(store,user){Owner=this}.ShowDialog()==true)UsersGrid.ItemsSource=store.Users();});
 void Configure(object sender,RoutedEventArgs e)=>Run(()=>{if(!store.IsAdmin)throw new Exception("Inicia sesión como administrador.");if(new SettingsWindow(store){Owner=this}.ShowDialog()==true){ReadSettings();vm.Refresh();Status.Text="Configuración guardada. Las ventas anteriores conservan sus importes.";}});
 void CashMovement(object sender,RoutedEventArgs e)=>Run(()=>{if(new CashMovementWindow(store){Owner=this}.ShowDialog()==true)RefreshStats();});
 async void ReturnSale(object sender,RoutedEventArgs e)=>await RunAsync(async()=>{if(History.SelectedItem is not SaleSummary sale||sale.Event!="Venta")throw new Exception("Selecciona una venta original.");var form=new ReturnWindow(store,sale.Folio){Owner=this};if(form.ShowDialog()==true){RefreshProductStocks();RefreshStats();await LoadReport();Ticket(form.Receipt);}});
 void ProductAvailability(object sender,RoutedEventArgs e)=>Run(()=>
 {
  if(Inventory.SelectedItem is not Product p)throw new Exception("Selecciona un producto.");
  var reason=Ask("Disponibilidad",$"Motivo para {(p.Active?"desactivar":"activar")} {p.Name}");if(string.IsNullOrWhiteSpace(reason))return;
  var password=Ask("Autorización","Contraseña del administrador",secret:true);if(password==null)return;store.SetProductAvailability(p.Id,!p.Active,p.Barcode,password,reason);Reload();RefreshProductStocks();
 });
 void ProductBarcode(object sender,RoutedEventArgs e)=>Run(()=>
 {
  if(Inventory.SelectedItem is not Product p)throw new Exception("Selecciona un producto.");var code=Ask("Código de barras","Código existente del producto (vacío para quitar)",p.Barcode);if(code==null)return;
  var password=Ask("Autorización","Contraseña del administrador",secret:true);if(password==null)return;store.SetProductAvailability(p.Id,p.Active,code,password,"Cambio de código de barras");Reload();
 });
 void ParkCurrentSale(object sender,RoutedEventArgs e)=>Run(()=>{if(vm.Cart.Count==0)throw new Exception("Agrega productos.");var label=Ask("Guardar venta pendiente","Nombre o referencia");if(label==null)return;SaveCart();store.ParkSale(label);Reset();});
 void ResumePendingSale(object sender,RoutedEventArgs e)=>Run(()=>
 {
  if(vm.Cart.Count>0)throw new Exception("Guarda o vacía la venta actual.");
  var pending=store.PendingSales();if(pending.Count==0)throw new Exception("No tienes ventas pendientes guardadas.");
  var dialog=new Window{Title="Recuperar venta pendiente",Width=560,Height=420,Owner=this,WindowStartupLocation=WindowStartupLocation.CenterOwner};var root=new DockPanel{Margin=new Thickness(20)};var button=new Button{Content="Recuperar"};DockPanel.SetDock(button,Dock.Bottom);root.Children.Add(button);var list=new ListBox{ItemsSource=pending,DisplayMemberPath="Label"};root.Children.Add(list);dialog.Content=root;button.Click+=(_,_)=>{if(list.SelectedItem!=null)dialog.DialogResult=true;};if(dialog.ShowDialog()!=true||list.SelectedItem is not PendingSale selected)return;
  store.ResumeSale(selected.Id);vm.Cart.Clear();RestoreCart();
 });
 void LockSession(object sender,RoutedEventArgs e)
 {
  if(working||!ready)return;SaveCart();ready=false;store.SignOut();
  var login=new LoginWindow(store){Owner=this};if(login.ShowDialog()!=true){Close();return;}
  vm.Cart.Clear();vm.Discount=0;vm.ExtraDiscounts=new();discountPassword=discountReason="";token=Guid.NewGuid().ToString();vm.Refresh();ready=true;Reload();RestoreCart();
 }
 void Shortcut(object sender,KeyEventArgs e)
 {
  if(!ready||working)return;
  if(e.Key==Key.F2){MainTabs.SelectedIndex=0;Search.Focus();Search.SelectAll();e.Handled=true;}
  else if(e.Key==Key.F4){Pay(sender,new());e.Handled=true;}
  else if(e.Key==Key.F8){ParkCurrentSale(sender,new());e.Handled=true;}
  else if(e.Key==Key.F12){LockSession(sender,new());e.Handled=true;}
  else if(e.Key==Key.Enter&&Search.IsKeyboardFocused)
  {
   var product=products.FirstOrDefault(x=>x.Active&&x.QuickReady&&(x.Barcode.Length>0&&x.Barcode==Search.Text.Trim()));
   if(product!=null){AddProduct(new Button{Tag=product},new());Search.Clear();e.Handled=true;}
  }
 }
 void RefreshProductStocks()
 {
  products=store.Products();Inventory.ItemsSource=products;
  for(var i=0;i<vm.Cart.Count;i++){var product=products.FirstOrDefault(p=>p.Id==vm.Cart[i].Product.Id);if(product!=null)vm.Cart[i]=vm.Cart[i] with{Product=product};}
  vm.Refresh();Filter();
 }
 async void RestoreBackup(object sender,RoutedEventArgs e)
 {
  if(working)return;
  await RunAsync(async()=>
  {
   if(!store.IsAdmin)throw new Exception("Inicia sesión como administrador para restaurar.");
   if(store.Shift()!=null||vm.Cart.Count>0)throw new Exception("Cierra el turno y guarda o vacía el carrito antes de restaurar.");
   var dialog=new OpenFileDialog{Filter="Respaldo SQLite|*.db",Title="Seleccionar respaldo para restaurar"};if(dialog.ShowDialog(this)!=true)return;
   var info=await Task.Run(()=>store.InspectBackup(dialog.FileName));
   if(MessageBox.Show(this,info.Description+"\n\nSe reemplazarán los datos actuales por los del respaldo. Los cambios posteriores a esa copia no estarán en los datos restaurados. Se conservará una copia completa del estado actual antes de continuar.\n\nEl POS se cerrará y deberás entrar con una cuenta del respaldo. ¿Restaurar?","Restaurar respaldo",MessageBoxButton.YesNo,MessageBoxImage.Warning)!=MessageBoxResult.Yes)return;
   var password=Ask("Autorización de restauración","Contraseña del administrador actual",secret:true);if(password==null)return;
   if(!await Task.Run(()=>store.Authenticate(password)))throw new Exception("Contraseña incorrecta.");
   if(protection!=null)await protection.StopAsync();
   try
   {
    var before=await Task.Run(()=>store.RestoreBackup(dialog.FileName,password));ready=false;
    MessageBox.Show(this,"Restauración completada. Vuelve a abrir el POS e inicia sesión con una cuenta del respaldo.\n\nCopia del estado anterior:\n"+before,"Datos restaurados");allowClose=true;Close();
   }
   catch{protection=new AutoProtection(store);throw;}
  });
 }
}
