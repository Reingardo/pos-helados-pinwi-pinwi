using System.Globalization;
using System.Windows;
namespace Pinwi.Pos;
public partial class App : Application
{
 protected override void OnStartup(StartupEventArgs e) { CultureInfo.DefaultThreadCurrentCulture = CultureInfo.GetCultureInfo("es-MX"); base.OnStartup(e); }
}
