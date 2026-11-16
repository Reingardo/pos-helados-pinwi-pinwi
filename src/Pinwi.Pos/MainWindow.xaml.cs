using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using Microsoft.Win32;
namespace Pinwi.Pos;
public partial class MainWindow : Window
{
 readonly Store store; readonly SaleViewModel vm=new(); List<Product> products=[]; string token=Guid.NewGuid().ToString(); string discountReason="", discountPassword=""; bool ready; string? activeCategory;
 public MainWindow():this(new Store()){}
 public MainWindow(Store store){this.store=store;InitializeComponent();BrandLogo.Source=new System.Windows.Media.Imaging.BitmapImage(new Uri(Path.Combine(AppContext.BaseDirectory,"Assets","Brand","logo-pinwi.png")));DataContext=vm;StartProtection();}
 void Run(Action action){try{action();}catch(Exception ex){LogError(ex);MessageBox.Show(this,ex is Microsoft.Data.Sqlite.SqliteException?"No se pudo guardar. Revisa duplicados y vuelve a intentar.":ex.Message,"Revisa la operación",MessageBoxButton.OK,MessageBoxImage.Warning);}}
 string? Ask(string title,string label,string initial="",bool secret=false)
 {
  var w=new Window{Title=title,Width=440,Height=220,Owner=this,WindowStartupLocation=WindowStartupLocation.CenterOwner,ResizeMode=ResizeMode.NoResize}; var panel=new StackPanel{Margin=new Thickness(20)}; panel.Children.Add(new TextBlock{Text=label,TextWrapping=TextWrapping.Wrap}); var input=new TextBox{Text=initial}; var pass=new PasswordBox{Margin=new Thickness(4),Padding=new Thickness(10)}; panel.Children.Add(secret?pass:input); var b=new Button{Content="Aceptar",IsDefault=true}; b.Click+=(_,_)=>w.DialogResult=true; panel.Children.Add(b); w.Content=panel; w.Loaded+=(_,_)=>{if(secret)pass.Focus();else {input.Focus();input.SelectAll();}}; return w.ShowDialog()==true?(secret?pass.Password:input.Text):null;
 }
 long Money(string value){if(!decimal.TryParse(value,out var n)||n<0||decimal.Round(n,2)!=n)throw new Exception("Captura un importe no negativo con hasta dos decimales.");return checked((long)(n*100));}
 async void LoadedWindow(object sender,RoutedEventArgs e)
 {
  await RunAsync(async()=>
  {
   if(!store.HasAdmin())
   {
    var name=Ask("Configuración inicial","Nombre del administrador");if(name==null){Close();return;}
    var password=Ask("Configuración inicial","Contraseña (mínimo 8 caracteres)",secret:true);if(password==null){Close();return;}
    await Task.Run(()=>{store.CreateAdmin(name,password);store.SignIn(name,password);});
   }
   else if(new LoginWindow(store){Owner=this}.ShowDialog()!=true){Close();return;}
   ready=true;Reload();RestoreCart();
  });
  if(!ready)Close();
 }
 void Reload(){ReadSettings();AdminTab.IsEnabled=store.IsAdmin;UsersGrid.ItemsSource=null;var selectedUser=ReportUser.SelectedItem as string;ReportUser.ItemsSource=new[]{"Todos"}.Concat(store.ReportUsers()).ToList();ReportUser.SelectedItem=store.IsAdmin?selectedUser??"Todos":store.CurrentUserName;ReportUser.IsEnabled=store.IsAdmin;if(ReportMethod.ItemsSource==null){ReportMethod.ItemsSource=new[]{"Todos","Efectivo","Tarjeta","Mixto"};ReportMethod.SelectedIndex=0;}products=store.Products();Inventory.ItemsSource=products;var selected=Categories.SelectedItem as string;Categories.ItemsSource=new[]{"Todos"}.Concat(products.Select(p=>p.Category).Distinct()).ToList();Categories.SelectedItem=selected??"Todos";Filter();RefreshStats();}
 void Filter()
 {
  if(!ready)return;var quick=activeCategory=="__quick";var searching=!string.IsNullOrWhiteSpace(Search.Text);
  CategoryList.Visibility=!quick&&activeCategory==null&&!searching?Visibility.Visible:Visibility.Collapsed;
  ProductList.Visibility=quick||CategoryList.Visibility==Visibility.Visible?Visibility.Collapsed:Visibility.Visible;
  QuickList.Visibility=quick?Visibility.Visible:Visibility.Collapsed;
  QuickList.ItemsSource=products.Where(p=>p.Active&&p.IsQuick&&($"{p.Name} {p.Id}".Contains(Search.Text,StringComparison.CurrentCultureIgnoreCase))).ToList();
  CategoryList.ItemsSource=products.Where(p=>p.Active&&!p.IsQuick).GroupBy(p=>p.Family).Select(g=>g.First()).ToList();
  CatalogHeading.Text=quick?"Cobro rápido · por gama":searching?"Resultados de búsqueda":activeCategory??"Productos";
  CatalogIntro.Text=quick?"Elige la gama y ajusta las piezas en el carrito. Los esquimales se venden desde Productos.":"Elige un tipo de producto y después un sabor.";
  QuickNote.Visibility=quick?Visibility.Visible:Visibility.Collapsed;
  HomeButton.Content="Productos";
  ProductList.ItemsSource=products.Where(p=>p.Active&&p.IsQuick==quick&&(quick||activeCategory==null||p.Family==activeCategory)&&($"{p.Name} {p.Category} {p.Id} {p.Barcode}".Contains(Search.Text,StringComparison.CurrentCultureIgnoreCase))).ToList();
 }
 void OpenQuickSales(object sender,RoutedEventArgs e){activeCategory="__quick";Search.Clear();Filter();}
 void BackCategories(object sender,RoutedEventArgs e){activeCategory=null;Search.Clear();Filter();}
 void EnterCategory(object sender,RoutedEventArgs e){activeCategory=(string)((Button)sender).Tag;Search.Clear();Filter();}
 void SearchChanged(object sender,RoutedEventArgs e)=>Filter();
 void AddProduct(object sender,RoutedEventArgs e)=>Run(()=>{var p=(Product)((Button)sender).Tag;if(!p.QuickReady||!p.Active)return;var index=vm.Cart.FindIndex(x=>x.Product.Id==p.Id);if(index<0)vm.Cart.Add(new(p,1));else vm.SetQuantity(p.Id,checked(vm.Cart[index].Quantity+1));vm.Refresh();});
 void IncreaseQuantity(object sender,RoutedEventArgs e)=>Run(()=>{var id=(int)((Button)sender).Tag;var line=vm.Cart.FirstOrDefault(x=>x.Product.Id==id);if(line!=null)vm.SetQuantity(id,checked(line.Quantity+1));});
 void DecreaseQuantity(object sender,RoutedEventArgs e)=>Run(()=>{var id=(int)((Button)sender).Tag;var line=vm.Cart.FirstOrDefault(x=>x.Product.Id==id);if(line!=null)vm.SetQuantity(id,line.Quantity-1);});
 void DeleteLine(object sender,RoutedEventArgs e){vm.SetQuantity((int)((Button)sender).Tag,0);}
 void QuantityEdited(object sender,System.Windows.Input.KeyboardFocusChangedEventArgs e)=>Run(()=>CommitQuantity((TextBox)sender));
 void QuantityKeyDown(object sender,System.Windows.Input.KeyEventArgs e){if(e.Key==System.Windows.Input.Key.Enter){Run(()=>CommitQuantity((TextBox)sender));e.Handled=true;}}
 void CommitQuantity(TextBox input){if(input.Tag is not int id)return;var line=vm.Cart.FirstOrDefault(x=>x.Product.Id==id);if(line==null)return;if(!int.TryParse(input.Text,out var quantity)||quantity<0||quantity>100000){input.Text=line.Quantity.ToString();Status.Text="Cantidad inválida. Usa piezas enteras entre 0 y 100,000.";return;}if(line.Quantity!=quantity)vm.SetQuantity(id,quantity);}
 void Reset(){vm.Cart.Clear();vm.Discount=0;vm.ExtraDiscounts=new();discountPassword=discountReason="";token=Guid.NewGuid().ToString();vm.Refresh();}
 void ClearCart(object sender,RoutedEventArgs e){if(MessageBox.Show(this,"¿Vaciar la venta?","Venta",MessageBoxButton.YesNo)==MessageBoxResult.Yes)Reset();}
 void SetDiscount(object sender,RoutedEventArgs e)=>Run(()=>{if(vm.Cart.Count==0)throw new Exception("Agrega productos antes de aplicar descuentos.");var dialog=new DiscountWindow(store,vm){Owner=this};if(dialog.ShowDialog()==true){discountReason=dialog.Reason;discountPassword=dialog.Authorization;SaveCart();}});
 async void Pay(object sender,RoutedEventArgs e)=>await RunAsync(async()=>
 {
  if(vm.Cart.Count==0)return;if(vm.Cart.Any(x=>!x.Product.Active))throw new Exception("Hay un producto desactivado. Reactívalo o retíralo del carrito antes de cobrar.");if(store.Shift()==null)throw new Exception("Abre un turno de caja primero.");
  var warning=vm.Cart.Any(x=>!x.Product.IsQuick&&x.Quantity>x.Product.Stock)?"Producto sin existencia suficiente registrada. Puedes continuar con el cobro.":"";
  if((vm.Discount>0||vm.ExtraDiscounts.Any)&&string.IsNullOrEmpty(discountPassword)){var authorization=Ask("Autorización de venta recuperada","Confirma el descuento con la contraseña del administrador",secret:true);if(authorization==null)return;if(!await Task.Run(()=>store.Authenticate(authorization)))throw new Exception("Contraseña incorrecta.");discountPassword=authorization;}
  var payment=new PaymentWindow(vm.TotalCents,warning){Owner=this};if(payment.ShowDialog()!=true)return;
  var cart=vm.Cart.ToList();var receipt=await Task.Run(()=>store.Checkout(cart,vm.Discount,payment.CashReceived,payment.CardPaid,token,discountReason,discountPassword,vm.ExtraDiscounts));
  Reset();RefreshProductStocks();RefreshStats();Ticket(receipt);
 });
 void CreateProduct(object sender,RoutedEventArgs e)=>Run(()=>OpenProductEditor(null));
 void EditProduct(object sender,RoutedEventArgs e)=>Run(()=>{if(Inventory.SelectedItem is not Product p)throw new Exception("Selecciona un producto.");OpenProductEditor(p);});
 void OpenProductEditor(Product? product)
 {
  var editor=new ProductWindow(store,product){Owner=this};if(editor.ShowDialog()!=true)return;
  Reload();for(var i=0;i<vm.Cart.Count;i++){var current=products.FirstOrDefault(p=>p.Id==vm.Cart[i].Product.Id);if(current!=null)vm.Cart[i]=vm.Cart[i] with {Product=current};}vm.Refresh();
  Inventory.SelectedItem=products.FirstOrDefault(p=>p.Id==editor.SavedId);
  Status.Text="Producto guardado. Ya está disponible en Venta.";
 }
 void OpenCash(object sender,RoutedEventArgs e)=>Run(()=>{if(store.Shift()!=null)throw new Exception("Ya hay un turno abierto.");var value=Ask("Apertura","Fondo inicial en efectivo","0");if(value==null)return;store.OpenShift(Money(value));RefreshStats();});
 void CloseCash(object sender,RoutedEventArgs e)=>Run(()=>{if(store.Shift()==null)throw new Exception("No hay turno abierto.");var value=Ask("Corte","Efectivo contado");if(value==null)return;var counted=Money(value);var expected=store.ExpectedCash();if(MessageBox.Show(this,$"Esperado: ${expected/100m:N2}\nContado: ${counted/100m:N2}\nDiferencia: ${(counted-expected)/100m:N2}\n¿Cerrar turno?","Corte",MessageBoxButton.YesNo)!=MessageBoxResult.Yes)return;var cutId=store.CloseShift(counted);RefreshStats();CashViews.SelectedIndex=0;var cut=store.Cuts().First(x=>x.Id==cutId);CutsGrid.SelectedItem=((IEnumerable<CashCut>)CutsGrid.ItemsSource).First(x=>x.Id==cutId);try{store.WriteCutFiles(cut);}catch(Exception ex){CutDelivery.Text="El corte quedó guardado. No se pudieron escribir sus archivos: "+ex.Message;}if(AutoWhatsApp.IsChecked==true)try{PrepareWhatsApp(cut);}catch(Exception ex){CutDelivery.Text="Corte guardado. WhatsApp pendiente: "+ex.Message;}});
 void RefreshStats(){RefreshExpenses();MovementsGrid.ItemsSource=store.CashMovements();ShiftLabel.Text=store.Shift() is long id?$"Turno #{id} abierto · Efectivo esperado ${store.ExpectedCash()/100m:N2}":"Caja cerrada";var selectedCut=(CutsGrid.SelectedItem as CashCut)?.Id;var cuts=store.Cuts();CutsGrid.ItemsSource=cuts;CutsGrid.SelectedItem=cuts.FirstOrDefault(x=>x.Id==selectedCut)??cuts.FirstOrDefault();Status.Text=$"Usuario: {store.CurrentUserName} · {store.CurrentRole} · Existencias informativas · F2 Buscar · F4 Cobrar · F8 Guardar · F12 Bloquear";}
 void RefreshHistory(object sender,RoutedEventArgs e)=>ApplyReport(sender,e);
 void ViewReceipt(object sender,RoutedEventArgs e){if(History.SelectedItem is SaleSummary s)Ticket(s.Receipt);}
 void Ticket(string text){new ReceiptWindow(text,settings){Owner=this}.ShowDialog();}
 static string Csv(string value)=>"\""+value.Replace("\"","\"\"")+"\"";
 async void Export(object sender,RoutedEventArgs e)=>await RunAsync(async()=>{var dialog=new SaveFileDialog{Filter="CSV|*.csv",FileName="ventas-periodo.csv"};if(dialog.ShowDialog()!=true)return;var filter=SelectedReportFilter();await Task.Run(()=>store.ExportSales(dialog.FileName,filter));MessageBox.Show(this,"Exportación del periodo completo guardada.");});
 async void Backup(object sender,RoutedEventArgs e)=>await RunAsync(async()=>{var dialog=new SaveFileDialog{Filter="Base SQLite|*.db",FileName=$"pinwi-{DateTime.Now:yyyyMMdd-HHmmss}.db"};if(dialog.ShowDialog(this)!=true)return;await Task.Run(()=>store.Backup(dialog.FileName));MessageBox.Show(this,"Respaldo guardado.");});
}
