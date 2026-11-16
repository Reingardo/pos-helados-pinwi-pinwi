using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
namespace Pinwi.Pos;
public partial class ProductWindow : Window
{
 readonly Store store;readonly Product? product;byte[]? photo;
 public int SavedId {get;private set;}
 public ProductWindow(Store store,Product? product=null)
 {
  this.store=store;this.product=product;InitializeComponent();Heading.Text=Title=product==null?"Añadir producto":"Editar producto";
  Category.ItemsSource=store.Products().Select(p=>p.Category).Distinct().ToList();
  ProductName.Text=product?.Name??"";Category.Text=product?.Category??"";Retail.Text=(product?.Retail/100m??0).ToString("0.00");Wholesale.Text=(product?.Wholesale/100m??0).ToString("0.00");Reason.Text=product==null?"Alta de producto":"";
  var path=product?.ImagePath??Path.Combine(AppContext.BaseDirectory,"Assets","Brand","logo-pinwi.png");if(File.Exists(path))Photo.Source=Load(File.ReadAllBytes(path));
 }
 static BitmapImage Load(byte[] bytes){using var stream=new MemoryStream(bytes);var image=new BitmapImage();image.BeginInit();image.CacheOption=BitmapCacheOption.OnLoad;image.StreamSource=stream;image.EndInit();image.Freeze();return image;}
 void ChoosePhoto(object sender,RoutedEventArgs e)
 {
  var dialog=new OpenFileDialog{Filter="Imágenes|*.jpg;*.jpeg;*.png;*.webp;*.bmp;*.tif;*.tiff",Title="Elige la foto del producto"};if(dialog.ShowDialog(this)!=true)return;
  try{var prepared=Store.PrepareProductImage(dialog.FileName);Photo.Source=Load(prepared);photo=prepared;PhotoLabel.Text=Path.GetFileName(dialog.FileName);Error.Text="";}catch(Exception){Error.Text="No se pudo leer la imagen. Usa una imagen válida de hasta 20 MB y 8,000 píxeles por lado. Si es WebP, conviértela a PNG o JPG.";}
 }
 static long Money(string value){if(!decimal.TryParse(value,out var amount)||amount<0||decimal.Round(amount,2)!=amount)throw new Exception("Captura precios no negativos con hasta dos decimales.");return checked((long)(amount*100));}
 async void Save(object sender,RoutedEventArgs e)
 {
  try{SaveButton.IsEnabled=false;var category=Category.Text;var name=ProductName.Text;var retail=Money(Retail.Text);var wholesale=Money(Wholesale.Text);var reason=Reason.Text;var password=Password.Password;SavedId=await Task.Run(()=>store.SaveProduct(product?.Id,category,name,retail,wholesale,reason,password,photo));DialogResult=true;}
  catch(Exception ex){Error.Text=ex.Message;SaveButton.IsEnabled=true;}
 }
}
