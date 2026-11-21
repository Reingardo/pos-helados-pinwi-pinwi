using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Pinwi.Pos;

// Documentation only: no users, credentials, shifts or transactions are created.
// Render the actual WPF views against a new temporary catalog, never the business database.
internal static class DocumentationScreenshots
{
    internal static void Run()
    {
        Exception? failure = null;
        var thread = new Thread(() =>
        {
            try { RenderViews(); }
            catch (Exception ex) { failure = ex; }
        });
        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
        thread.Join();
        if (failure != null) throw new InvalidOperationException("No se generaron las capturas de documentación.", failure);
    }

    private static void RenderViews()
    {
        var app = new Application { ShutdownMode = ShutdownMode.OnExplicitShutdown };
        var xml = System.Xml.Linq.XDocument.Load(Path.Combine(AppContext.BaseDirectory, "Theme.xaml"));
        var resources = new System.Xml.Linq.XElement(
            System.Xml.Linq.XName.Get("ResourceDictionary", "http://schemas.microsoft.com/winfx/2006/xaml/presentation"),
            new System.Xml.Linq.XAttribute(System.Xml.Linq.XNamespace.Xmlns + "x", "http://schemas.microsoft.com/winfx/2006/xaml"),
            xml.Root!.Elements().First().Elements());
        app.Resources = (ResourceDictionary)System.Windows.Markup.XamlReader.Parse(resources.ToString());
        var output = Path.GetFullPath("artifacts/documentacion");
        Directory.CreateDirectory(output);
        var store = new Store(Path.Combine(Path.GetTempPath(), "PinwiDocumentation", Guid.NewGuid().ToString("N")));
        var main = new MainWindow(store) { Height = 800, ShowInTaskbar = false };
        const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
        main.Loaded -= (RoutedEventHandler)Delegate.CreateDelegate(typeof(RoutedEventHandler), main,
            typeof(MainWindow).GetMethod("LoadedWindow", flags)!);
        // Selecting a tab for a static image must not trigger login/report operations.
        ((TabControl)main.FindName("MainTabs")).SelectionChanged -=
            (SelectionChangedEventHandler)Delegate.CreateDelegate(typeof(SelectionChangedEventHandler), main,
                typeof(MainWindow).GetMethod("MainTabChanged", flags)!);
        typeof(MainWindow).GetField("ready", flags)!.SetValue(main, true);
        typeof(MainWindow).GetMethod("Reload", flags)!.Invoke(main, null);
        ((TextBlock)main.FindName("Status")).Text = "DEMOSTRACIÓN · Catálogo inicial · Sin cuentas ni operaciones guardadas";
        ((TextBlock)main.FindName("DraftState")).Text = "Carrito de ejemplo · No se ha confirmado ningún cobro";
        ((CheckBox)main.FindName("AutoWhatsApp")).IsChecked = false;
        var vm = (SaleViewModel)main.DataContext;

        void Catalog(string? family)
        {
            typeof(MainWindow).GetField("activeCategory", flags)!.SetValue(main, family);
            typeof(MainWindow).GetMethod("Filter", flags)!.Invoke(main, null);
        }
        void Capture(Window window, string name, bool close = true)
        {
            window.ShowInTaskbar = false;
            window.Show();
            window.Dispatcher.Invoke(() => { }, DispatcherPriority.ApplicationIdle);
            window.UpdateLayout();
            var content = (FrameworkElement)window.Content;
            var image = new RenderTargetBitmap((int)Math.Ceiling(content.ActualWidth) + 48,
                (int)Math.Ceiling(content.ActualHeight) + 48, 96, 96, PixelFormats.Pbgra32);
            var drawing = new DrawingVisual();
            using (var context = drawing.RenderOpen())
            {
                context.DrawRectangle(window.Background, null, new Rect(0, 0, image.PixelWidth, image.PixelHeight));
                context.DrawRectangle(new VisualBrush(content), null, new Rect(24, 24, content.ActualWidth, content.ActualHeight));
            }
            image.Render(drawing);
            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(image));
            using (var stream = File.Create(Path.Combine(output, name))) encoder.Save(stream);
            if (close) window.Close();
        }

        Capture(main, "01-inicio.png", false);
        Catalog("Bolis / Pinwis");
        Capture(main, "02-productos.png", false);
        Catalog("__quick");
        vm.Cart.Add(new(store.Products().Single(p => p.QuickGroup == "bolis"), 10));
        vm.Refresh();
        Capture(main, "03-cobro-rapido.png", false);
        var payment = new PaymentWindow(vm.TotalCents, "");
        ((TextBox)payment.FindName("CashInput")).Text = "100";
        Capture(payment, "04-cobrar.png");
        Capture(new DiscountWindow(store, vm) { Height = 900 }, "05-descuentos.png");
        Capture(new ProductWindow(store, store.Products().First()) { Height = 900 }, "06-editar-producto.png");
        vm.Cart.Clear();
        vm.Refresh();
        ((TabControl)main.FindName("MainTabs")).SelectedIndex = 2;
        Capture(main, "07-caja.png", false);
        ((TabControl)main.FindName("MainTabs")).SelectedIndex = 4;
        Capture(main, "08-respaldos.png", false);
        if (store.Users().Count != 0 || store.Sales().Count != 0 || store.Cuts().Count != 0 ||
            store.Shift() != null || store.Expenses(DateTimeOffset.Now).Count != 0)
            throw new InvalidOperationException("El entorno de documentación debe permanecer sin cuentas ni operaciones.");
        typeof(MainWindow).GetField("allowClose", flags)!.SetValue(main, true);
        main.Close();
        app.Shutdown();
        Console.WriteLine("Capturas de documentación generadas sin cuentas ni operaciones: " + output);
    }
}
