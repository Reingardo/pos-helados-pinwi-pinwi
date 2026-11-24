using System.Globalization;
using System.Windows;
namespace Pinwi.Pos;
public partial class App : Application
{
 SingleInstance? instance;
 protected override void OnStartup(StartupEventArgs e)
 {
  CultureInfo.DefaultThreadCurrentCulture=CultureInfo.GetCultureInfo("es-MX");
  System.Windows.FrameworkElement.LanguageProperty.OverrideMetadata(typeof(System.Windows.FrameworkElement),new FrameworkPropertyMetadata(System.Windows.Markup.XmlLanguage.GetLanguage("es-MX")));
  ShutdownMode=ShutdownMode.OnExplicitShutdown;
  try
  {
  var packaged=DataLocation.IsPackaged;
  var directory=DataLocation.DefaultDirectory;
  instance=new SingleInstance(System.IO.Path.Combine(directory,"pinwi.db"));
  if(!instance.Acquired){if(!SingleInstance.FocusExisting())MessageBox.Show("El POS ya está abierto. Revisa la ventana existente.","Helados Pinwi Pinwi");Shutdown();return;}
  // Older deliveries predate the mutex. Never open a second version over their database.
  if(System.Diagnostics.Process.GetProcessesByName("Pinwi.Pos").Any(p=>p.Id!=Environment.ProcessId))
  {SingleInstance.FocusExisting();MessageBox.Show("Cierra la versión anterior antes de abrir esta actualización.","POS abierto");Shutdown();return;}
  base.OnStartup(e);
  if(packaged&&!System.IO.File.Exists(System.IO.Path.Combine(directory,"pinwi.db")))
   if(new StoreWelcomeWindow(directory).ShowDialog()!=true){Shutdown();return;}
  MainWindow=new MainWindow(new Store(directory));ShutdownMode=ShutdownMode.OnMainWindowClose;MainWindow.Show();
  }
  catch(Exception ex){MessageBox.Show("No se pudo abrir el POS. Tus datos se conservan.\n"+ex.Message,"Inicio del POS");Shutdown(1);}
 }
 protected override void OnExit(ExitEventArgs e){instance?.Dispose();base.OnExit(e);}
}
