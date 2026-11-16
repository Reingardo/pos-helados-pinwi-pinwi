using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
namespace Pinwi.Pos;
public static class ReceiptBuilder
{
 public static FlowDocument Create(string receipt,double width=302)
 {
  var doc=new FlowDocument{PageWidth=width,ColumnWidth=width,PagePadding=new Thickness(10),FontFamily=new FontFamily("Segoe UI"),FontSize=11,Background=Brushes.White,Foreground=Brushes.Black};
  var logo=new BitmapImage();logo.BeginInit();logo.CacheOption=BitmapCacheOption.OnLoad;logo.UriSource=new Uri(Path.Combine(AppContext.BaseDirectory,"Assets","Brand","logo-pinwi.png"));logo.EndInit();logo.Freeze();
  doc.Blocks.Add(new BlockUIContainer(new Image{Source=logo,Width=Math.Min(150,width-30),Height=Math.Min(150,width-30),Stretch=Stretch.Uniform,HorizontalAlignment=HorizontalAlignment.Center}){Margin=new Thickness(0,0,0,8)});
  var body=receipt.Replace("\r\n","\n").Split('\n').ToList();
  if(body.Count>1&&body[0]=="HELADOS PINWI PINWI"){body.RemoveAt(0);if(body[0]=="833 393 2616")body.RemoveAt(0);}
  var footer=body.FindIndex(x=>x.StartsWith("¡Gracias por tu compra!")||x.StartsWith("¡Muchas gracias por su compra!"));if(footer>=0)body=body.Take(footer).ToList();
  foreach(var line in body){var p=new Paragraph(new Run(line)){Margin=new Thickness(0,0,0,3)};if(line.StartsWith("TOTAL:")||line.StartsWith("Cambio:")){p.FontWeight=FontWeights.Bold;p.FontSize=14;p.Margin=new Thickness(0,8,0,6);}doc.Blocks.Add(p);}
  doc.Blocks.Add(new Paragraph(new Run("────────────────────")){TextAlignment=TextAlignment.Center,Margin=new Thickness(0,8,0,8)});
  doc.Blocks.Add(new Paragraph(new Bold(new Run("¡Muchas gracias por su compra!"))){TextAlignment=TextAlignment.Center,Margin=new Thickness(0,0,0,8)});
  doc.Blocks.Add(new Paragraph(new Run("Cualquier sugerencia, estamos a la orden.")){TextAlignment=TextAlignment.Center,Margin=new Thickness(0,0,0,12)});
  doc.Blocks.Add(Contact("facebook","facebook.com/heladospinwi","https://www.facebook.com/heladospinwi"));
  doc.Blocks.Add(Contact("whatsapp","833 393 2616","https://wa.me/528333932616"));
  doc.Blocks.Add(new Paragraph(new Run("Comprobante de venta")){TextAlignment=TextAlignment.Center,FontSize=9,Margin=new Thickness(0,12,0,0)});
  return doc;
 }
 static Paragraph Contact(string kind,string label,string url)
 {
  var p=new Paragraph{TextAlignment=TextAlignment.Center,Margin=new Thickness(0,4,0,4),FontSize=10};
  var icon=new Grid{Width=18,Height=18,Margin=new Thickness(0,0,5,0),ToolTip=kind=="facebook"?"Facebook":"WhatsApp"};
  icon.Children.Add(new System.Windows.Shapes.Ellipse{Fill=kind=="facebook"?new SolidColorBrush(Color.FromRgb(24,119,242)):new SolidColorBrush(Color.FromRgb(25,150,75))});
  if(kind=="facebook")icon.Children.Add(new TextBlock{Text="f",Foreground=Brushes.White,FontFamily=new FontFamily("Arial"),FontWeight=FontWeights.Bold,FontSize=18,HorizontalAlignment=HorizontalAlignment.Center,Margin=new Thickness(1,-2,0,0)});
  else {icon.Children.Add(new System.Windows.Shapes.Path{Data=Geometry.Parse("M 4,14 L 5,11 A 6,6 0 1 1 8,14 Z"),Stroke=Brushes.White,StrokeThickness=1,Margin=new Thickness(1)});icon.Children.Add(new TextBlock{Text="☎",Foreground=Brushes.White,FontSize=10,HorizontalAlignment=HorizontalAlignment.Center,VerticalAlignment=VerticalAlignment.Center});}
  p.Inlines.Add(new InlineUIContainer(icon){BaselineAlignment=BaselineAlignment.Center});
  var link=new Hyperlink(new Run(label)){NavigateUri=new Uri(url),Foreground=Brushes.Black,TextDecorations=null};link.RequestNavigate+=(_,e)=>{Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri){UseShellExecute=true});e.Handled=true;};p.Inlines.Add(link);return p;
 }
}
