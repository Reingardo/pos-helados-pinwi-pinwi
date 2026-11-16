using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
namespace Pinwi.Pos;
public sealed class ReceiptWindow : Window
{
 public ReceiptWindow(string text)
 {
  Title="Ticket del cliente";Width=520;Height=760;MinWidth=390;MinHeight=550;WindowStartupLocation=WindowStartupLocation.CenterOwner;Background=System.Windows.Media.Brushes.WhiteSmoke;
  var root=new DockPanel{Margin=new Thickness(20)};
  var heading=new TextBlock{Text="Ticket del cliente",FontSize=24,FontWeight=FontWeights.SemiBold,Margin=new Thickness(0,0,0,12)};DockPanel.SetDock(heading,Dock.Top);root.Children.Add(heading);
  var tools=new StackPanel{Orientation=Orientation.Horizontal};DockPanel.SetDock(tools,Dock.Top);root.Children.Add(tools);
  var paper=new ComboBox{ItemsSource=new[]{"80 mm","58 mm"},SelectedIndex=0,Width=110,Margin=new Thickness(4),Padding=new Thickness(8)};tools.Children.Add(paper);var print=new Button{Content="Imprimir ticket"};tools.Children.Add(print);
  var viewer=new FlowDocumentScrollViewer{IsToolBarVisible=false,VerticalScrollBarVisibility=ScrollBarVisibility.Auto,Document=ReceiptBuilder.Create(text)};root.Children.Add(viewer);
  paper.SelectionChanged+=(_,_)=>viewer.Document=ReceiptBuilder.Create(text,paper.SelectedIndex==0?302:219);
  print.Click+=(_,_)=>{try{var dialog=new PrintDialog();if(dialog.ShowDialog()!=true)return;var requested=paper.SelectedIndex==0?302d:219d;var width=Math.Min(requested,dialog.PrintableAreaWidth);if(width<100)throw new Exception("La impresora no ofrece un ancho válido.");var doc=ReceiptBuilder.Create(text,width);dialog.PrintDocument(((IDocumentPaginatorSource)doc).DocumentPaginator,"Ticket Helados Pinwi Pinwi");}catch(Exception ex){MessageBox.Show(this,"La venta está guardada. No se pudo imprimir: "+ex.Message,"Impresión");}};
  Content=root;
 }
}
