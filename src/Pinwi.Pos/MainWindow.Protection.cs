using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
namespace Pinwi.Pos;
public partial class MainWindow
{
 AutoProtection? protection;DispatcherTimer? protectionTimer;
 void StartProtection()
 {
  protection=new AutoProtection(store);vm.PropertyChanged+=(_,e)=>{if(e.PropertyName==nameof(vm.Lines))SaveCart();};
  protectionTimer=new DispatcherTimer{Interval=TimeSpan.FromSeconds(2)};protectionTimer.Tick+=(_,_)=>{BackupState.Text=protection.Status;BackupState.Foreground=(protection.Status.StartsWith("No se pudo")||protection.Status.Contains("falló"))?Brushes.Crimson:Brushes.Gray;};protectionTimer.Start();
  Closed+=(_,_)=>{protectionTimer.Stop();protection.Dispose();};
 }
 void SaveCart(int? editedId=null,int? quantity=null)
 {
  if(!ready||store.CurrentUserName==null)return;
  try
  {
   var lines=vm.Cart.Select(x=>new DraftLine(x.Product.Id,editedId==x.Product.Id&&quantity.HasValue?quantity.Value:x.Quantity)).Where(x=>x.Quantity>0).ToList();
   store.SaveDraft(new(token,store.CurrentUserName,lines,vm.Discount,discountReason,DateTimeOffset.Now.ToString("O"),vm.ExtraDiscounts));
   DraftState.Text=$"Venta guardada · {DateTime.Now:t}";DraftState.Foreground=Brushes.Gray;
  }
  catch(Exception ex){DraftState.Text="No se pudo guardar la venta pendiente. Revisa espacio y permisos.";DraftState.Foreground=Brushes.Crimson;Status.Text=ex.Message;}
 }
 void RestoreCart()
 {
  var draft=store.LoadDraft();if(draft==null||draft.Lines.Count==0)return;
  token=draft.Token;discountReason=draft.Reason;discountPassword="";vm.Discount=draft.Discount;vm.ExtraDiscounts=draft.ExtraDiscounts??new();
  foreach(var l in draft.Lines){var p=products.FirstOrDefault(x=>x.Id==l.ProductId);if(p!=null&&l.Quantity>0&&l.Quantity<=100000)vm.Cart.Add(new(p,l.Quantity));}
  vm.Refresh();DraftState.Text="Venta pendiente recuperada. Precios actualizados al catálogo vigente.";
 }
 void QuantityTyping(object sender,TextChangedEventArgs e)
 {
  if(sender is TextBox input&&input.IsKeyboardFocused&&input.Tag is int id&&int.TryParse(input.Text,out var q)&&q>=0&&q<=100000)SaveCart(id,q);
 }
 void ShowCut(object sender,SelectionChangedEventArgs e){if(CutsGrid.SelectedItem is CashCut cut)CutSummary.Text=string.IsNullOrWhiteSpace(cut.Summary)?$"Corte anterior #{cut.Id}\nVentas: {cut.Total:C}\nEsperado: {cut.Expected:C}\nContado: {cut.CountedPesos:C}\nDiferencia: {cut.Difference:C}\nSin nota histórica generada.":cut.Summary;}
 CashCut SelectedCut()=>CutsGrid.SelectedItem as CashCut??throw new Exception("Selecciona un corte cerrado.");
 void OpenCutFolder(object sender,RoutedEventArgs e)=>Run(()=>{var cut=SelectedCut();if(string.IsNullOrWhiteSpace(cut.Summary))throw new Exception("Este corte es anterior a la generación de notas.");var folder=store.WriteCutFiles(cut);Process.Start(new ProcessStartInfo(folder){UseShellExecute=true});});
 void SendCutWhatsApp(object sender,RoutedEventArgs e)=>Run(()=>PrepareWhatsApp(SelectedCut()));
 void MarkCutSent(object sender,RoutedEventArgs e)=>Run(()=>{var cut=SelectedCut();if(MessageBox.Show(this,$"¿Ya enviaste el corte #{cut.Id} por WhatsApp?","Confirmar envío realizado",MessageBoxButton.YesNo)!=MessageBoxResult.Yes)return;store.ConfirmCutSent(cut.Id);RefreshStats();});
 void PrepareWhatsApp(CashCut cut)
 {
  if(string.IsNullOrWhiteSpace(cut.Summary))throw new Exception("Este corte anterior no tiene nota para WhatsApp.");
  store.WriteCutFiles(cut);
  var url="https://wa.me/528333932616?text="+Uri.EscapeDataString(cut.Summary);
  var copied=url.Length>1800; if(copied){Clipboard.SetText(cut.Summary);url="https://wa.me/528333932616";}
  Process.Start(new ProcessStartInfo(url){UseShellExecute=true});
  CutDelivery.Text=copied?"Nota completa copiada. Pégala en WhatsApp y pulsa Enviar. Los CSV del corte incluyen ventas y gastos. Sin conexión, queda pendiente.":"Nota preparada con ventas y gastos. Pulsa Enviar en WhatsApp; los CSV están en los archivos del corte. Sin conexión, queda pendiente.";
 }
}
