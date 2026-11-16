using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Win32;
namespace Pinwi.Pos;

// Shared layout for small administrative operations, with validation kept in Store.
public class OperationWindow : Window
{
 protected readonly StackPanel Fields=new();
 protected readonly TextBlock Error=new(){Foreground=Brushes.Crimson,TextWrapping=TextWrapping.Wrap,Margin=new Thickness(4,8,4,8)};
 readonly Button save=new(){Content="Guardar",Background=new SolidColorBrush(Color.FromRgb(215,54,128)),Foreground=Brushes.White};
 protected OperationWindow(string title,int width=580)
 {
  Title=title;Width=width;Height=680;MinHeight=420;WindowStartupLocation=WindowStartupLocation.CenterOwner;Background=new SolidColorBrush(Color.FromRgb(246,245,250));
  var root=new DockPanel{Margin=new Thickness(24)};var footer=new StackPanel();DockPanel.SetDock(footer,Dock.Bottom);root.Children.Add(footer);footer.Children.Add(Error);
  var buttons=new StackPanel{Orientation=Orientation.Horizontal,HorizontalAlignment=HorizontalAlignment.Right};buttons.Children.Add(new Button{Content="Cancelar",IsCancel=true});buttons.Children.Add(save);footer.Children.Add(buttons);
  root.Children.Add(new ScrollViewer{Content=Fields,VerticalScrollBarVisibility=ScrollBarVisibility.Auto});Content=root;
  Note(title,25);
 }
 protected void Note(string text,double size=14)=>Fields.Children.Add(new TextBlock{Text=text,FontSize=size,TextWrapping=TextWrapping.Wrap,Margin=new Thickness(4,8,4,8)});
 protected TextBox Text(string label,string value=""){Note(label);var input=new TextBox{Text=value};Fields.Children.Add(input);return input;}
 protected PasswordBox Secret(string label){Note(label);var input=new PasswordBox{Padding=new Thickness(10),Margin=new Thickness(4)};Fields.Children.Add(input);return input;}
 protected ComboBox Choice(string label,IEnumerable<string> values,string selected=""){Note(label);var items=values.ToList();var input=new ComboBox{ItemsSource=items,SelectedItem=items.Contains(selected)?selected:items.FirstOrDefault(),Margin=new Thickness(4),Padding=new Thickness(8)};Fields.Children.Add(input);return input;}
 protected CheckBox Check(string label,bool value=false){var input=new CheckBox{Content=label,IsChecked=value,Margin=new Thickness(4,12,4,8)};Fields.Children.Add(input);return input;}
 protected void OnSave(Func<Task> action,string caption="Guardar")
 {
  save.Content=caption;save.Click+=async(_,_)=>{try{save.IsEnabled=false;Error.Text="";await action();DialogResult=true;}catch(Exception ex){Error.Text=ex is Microsoft.Data.Sqlite.SqliteException?"No se pudo guardar. Revisa duplicados y vuelve a intentar.":ex.Message;}finally{save.IsEnabled=true;}};
 }
 protected static long Money(string value){if(!decimal.TryParse(value,out var n)||n<0||decimal.Round(n,2)!=n)throw new Exception("Captura un importe no negativo con hasta dos decimales.");return checked((long)(n*100));}
}

public sealed class LoginWindow : OperationWindow
{
 public LoginWindow(Store store):base("Iniciar sesión",460)
 {
  Height=440;var users=Choice("Usuario",store.Users().Where(x=>x.Active).Select(x=>x.Name));var password=Secret("Contraseña");
  OnSave(async()=>{var name=(string?)users.SelectedItem??"";var secret=password.Password;if(!await Task.Run(()=>store.SignIn(name,secret)))throw new Exception("Usuario o contraseña incorrectos.");},"Entrar");
 }
}

public sealed class UserWindow : OperationWindow
{
 public UserWindow(Store store,UserAccount? user=null):base(user==null?"Añadir usuario":"Editar usuario")
 {
  var name=Text("Nombre",user?.Name??"");var role=Choice("Permisos",new[]{"Cajero","Administrador"},user?.Role??"Cajero");var active=Check("Usuario activo",user?.Active??true);
  var password=Secret(user==null?"Contraseña del nuevo usuario":"Nueva contraseña (vacía para conservar)");var authorization=Secret("Contraseña del administrador que autoriza");
  OnSave(async()=>{var n=name.Text;var r=(string)role.SelectedItem;var a=active.IsChecked==true;var p=password.Password;var auth=authorization.Password;await Task.Run(()=>store.SaveUser(user?.Id,n,r,a,p.Length==0?null:p,auth));});
 }
}

public sealed class SettingsWindow : OperationWindow
{
 public SettingsWindow(Store store):base("Configuración",680)
 {
  var settings=store.GetSettings();var name=Text("Nombre del negocio",settings.BusinessName);var phone=Text("Teléfono / WhatsApp",settings.Phone);var facebook=Text("Facebook",settings.Facebook);var address=Text("Dirección (opcional)",settings.Address);
  var pieces=Text("Piezas de la misma gama para mayoreo",settings.Pricing.Quantity.ToString());var threshold=Text("Consumo a menudeo para mayoreo global (MXN)",(settings.Pricing.GlobalCents/100m).ToString("0.00"));var stack=Check("Permitir descuentos adicionales sobre mayoreo",settings.Pricing.StackDiscounts);
  var paper=Choice("Ancho de ticket",new[]{"80","58"},settings.PaperMm.ToString());
  var printers=new List<string>{"Preguntar al imprimir"};try{using var server=new System.Printing.LocalPrintServer();printers.AddRange(server.GetPrintQueues().Select(x=>x.FullName));}catch(Exception){}
  var printer=Choice("Impresora",printers,settings.PrinterName.Length==0?printers[0]:settings.PrinterName);
  var backup=Text("Carpeta adicional de respaldos (opcional)",settings.BackupDirectory);var browse=new Button{Content="Elegir carpeta o USB…"};Fields.Children.Add(browse);browse.Click+=(_,_)=>{var dialog=new OpenFolderDialog{Title="Destino adicional de respaldos"};if(dialog.ShowDialog(this)==true)backup.Text=dialog.FolderName;};
  Note("Si la unidad externa no está disponible, se mantiene el respaldo local y se muestra un aviso.");
  var reason=Text("Motivo del cambio");var auth=Secret("Contraseña del administrador");
  OnSave(async()=>{if(!int.TryParse(pieces.Text,out var q))throw new Exception("Captura un número entero de piezas.");var updated=settings with{BusinessName=name.Text.Trim(),Phone=phone.Text.Trim(),Facebook=facebook.Text.Trim(),Address=address.Text.Trim(),Pricing=new(q,Money(threshold.Text),stack.IsChecked==true),PaperMm=int.Parse((string)paper.SelectedItem),PrinterName=printer.SelectedIndex==0?"":(string)printer.SelectedItem,BackupDirectory=backup.Text.Trim()};var password=auth.Password;var why=reason.Text;await Task.Run(()=>store.SaveSettings(updated,password,why));});
 }
}

public sealed class CashMovementWindow : OperationWindow
{
 readonly string token=Guid.NewGuid().ToString("N");
 public CashMovementWindow(Store store):base("Entrada o retiro de efectivo")
 {
  Note("Estos movimientos cambian el efectivo esperado; no se cuentan como ventas ni gastos.");
  var direction=Choice("Tipo",new[]{"Entrada","Retiro"});var amount=Text("Importe (MXN)");var reason=Text("Motivo");var password=Secret("Contraseña del administrador");
  OnSave(async()=>{var cents=Money(amount.Text)*(direction.SelectedIndex==0?1:-1);var why=reason.Text;var auth=password.Password;await Task.Run(()=>store.AddCashMovement(cents,why,token,auth));});
 }
}

public sealed class ReturnWindow : OperationWindow
{
 readonly string token=Guid.NewGuid().ToString("N");
 public string Receipt {get;private set;}="";
 public ReturnWindow(Store store,long sale):base($"Devolución · venta {sale:000000}",720)
 {
  Note("Se devuelve el precio realmente pagado, respetando descuentos y devoluciones anteriores. La venta original se conserva.");
  var inputs=new List<(ReturnLine Line,TextBox Quantity,CheckBox Restock)>();
  foreach(var line in store.ReturnableLines(sale).Where(x=>x.Available>0))
  {
   Note($"{line.Name} · disponibles: {line.Available}");var q=Text("Piezas a devolver","0");var restock=Check("Reintegrar estas piezas al inventario (solo si corresponde)");inputs.Add((line,q,restock));
  }
  var cancel=Check("Cancelar la venta completa (sin reintegrar existencias)");var reason=Text("Motivo");var terminal=Check("La parte de tarjeta ya fue devuelta en la terminal externa");
  Note("Los pagos mixtos se devuelven proporcionalmente a los importes originales de efectivo y tarjeta. La terminal se opera por separado.");
  var password=Secret("Contraseña del administrador");
  OnSave(async()=>
  {
   var request=new List<ReturnRequest>();foreach(var item in inputs){if(!int.TryParse(item.Quantity.Text,out var quantity)||quantity<0)throw new Exception("Captura cantidades enteras no negativas.");if(quantity>0)request.Add(new(item.Line.ProductId,quantity,item.Restock.IsChecked==true));}
   var why=reason.Text;var auth=password.Password;var all=cancel.IsChecked==true;var confirmed=terminal.IsChecked==true;
   var quote=await Task.Run(()=>store.QuoteRefund(sale,request,all));
   if(quote.Card>0&&!confirmed)throw new Exception("La devolución incluye tarjeta. Confirma el reembolso en la terminal externa.");
   if(MessageBox.Show(this,$"{quote.Detail}\n\nTotal a devolver: {quote.Total/100m:C}\nEfectivo: {quote.Cash/100m:C}\nTarjeta externa: {quote.Card/100m:C}\nMotivo: {why}\n\n¿Registrar la devolución?","Devolución",MessageBoxButton.YesNo,MessageBoxImage.Question)!=MessageBoxResult.Yes)throw new Exception("Revisa los datos antes de confirmar.");
   var result=await Task.Run(()=>store.Refund(sale,request,why,auth,token,all,confirmed));Receipt=result.Receipt;
  },"Registrar devolución");
 }
}
