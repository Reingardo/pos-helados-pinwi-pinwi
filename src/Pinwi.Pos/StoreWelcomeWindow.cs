using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Win32;

namespace Pinwi.Pos;

public sealed class StoreWelcomeWindow : Window
{
 bool working;
 public StoreWelcomeWindow(string directory)
 {
  Title="Bienvenido a Pinwi POS · Microsoft Store";Width=720;Height=640;ResizeMode=ResizeMode.NoResize;
  WindowStartupLocation=WindowStartupLocation.CenterScreen;Background=new SolidColorBrush(Color.FromRgb(246,245,250));
  var root=new StackPanel{Margin=new Thickness(28)};Content=root;
  void Note(string text,int size=15)=>root.Children.Add(new TextBlock{Text=text,FontSize=size,TextWrapping=TextWrapping.Wrap,Margin=new Thickness(4,8,4,8)});
  Note("¿Cómo deseas comenzar?",28);
  Note("Esta edición de Microsoft Store guarda sus datos por separado de la versión local. La instalación no traslada ni modifica tu base anterior.");
  Note("Si ya usas el POS, crea un respaldo desde la versión local, ciérrala y elige aquí esa copia. Se validará antes de importar. Después entrarás con las cuentas y contraseñas del respaldo.");
  Note("Importar conserva los datos incluidos en la copia y no fusiona operaciones de dos equipos. Para el traslado definitivo, deja de vender en la versión anterior.");
  var choices=new StackPanel();root.Children.Add(choices);
  var import=new Button{Content="Traer un respaldo del POS",Background=new SolidColorBrush(Color.FromRgb(215,54,128)),Foreground=Brushes.White};choices.Children.Add(import);
  var fresh=new Button{Content="Comenzar una instalación independiente"};choices.Children.Add(fresh);
  var cancel=new Button{Content="Salir",IsCancel=true};choices.Children.Add(cancel);
  var status=new TextBlock{TextWrapping=TextWrapping.Wrap,Foreground=Brushes.Crimson,Margin=new Thickness(4,10,4,4)};root.Children.Add(status);
  import.Click+=async(_,_)=>
  {
   var dialog=new OpenFileDialog{Filter="Respaldo SQLite del POS|*.db",Title="Selecciona el respaldo creado desde el POS"};
   if(dialog.ShowDialog(this)!=true)return;
   if(MessageBox.Show(this,"Se creará esta instalación con los datos del respaldo seleccionado. El archivo de origen y la versión local se conservarán. No se combinarán ventas posteriores.\n\n¿Importar esta copia?","Traer respaldo",MessageBoxButton.YesNo,MessageBoxImage.Question)!=MessageBoxResult.Yes)return;
   working=true;choices.IsEnabled=false;status.Text="Validando y preparando tu respaldo. Espera a que termine…";
   try{await Task.Run(()=>Store.ImportFirstBackup(dialog.FileName,directory));working=false;DialogResult=true;}
   catch(Exception ex){working=false;choices.IsEnabled=true;status.Text="No se importó el respaldo. La base anterior y el archivo de origen se conservan.\n"+ex.Message;}
  };
  fresh.Click+=(_,_)=>
  {
   if(MessageBox.Show(this,"Comenzarás sin las ventas, cuentas ni precios personalizados de la versión local. Crearás un administrador nuevo. La versión anterior conserva sus datos.\n\n¿Crear una instalación independiente?","Instalación independiente",MessageBoxButton.YesNo,MessageBoxImage.Question)==MessageBoxResult.Yes)DialogResult=true;
  };
  cancel.Click+=(_,_)=>DialogResult=false;
  Closing+=(_,e)=>{if(working)e.Cancel=true;};
 }
}
