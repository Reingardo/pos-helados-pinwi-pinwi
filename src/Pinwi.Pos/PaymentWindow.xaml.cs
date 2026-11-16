using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
namespace Pinwi.Pos;
public partial class PaymentWindow : Window
{
 readonly long total; bool initialized; string method="cash";
 public long CashReceived {get;private set;}
 public long CardPaid {get;private set;}
 public PaymentWindow(long total,string warning="")
 {
  this.total=total;InitializeComponent();TotalLabel.Text=Money(total);StockWarning.Text=warning;initialized=true;UpdateSuggestions();UpdateState();Loaded+=(_,_)=>CashInput.Focus();
 }
 static string Money(long n)=>(n/100m).ToString("C",CultureInfo.GetCultureInfo("es-MX"));
 static bool Parse(string s,out long cents){cents=0;if(!decimal.TryParse(s,NumberStyles.AllowDecimalPoint|NumberStyles.AllowLeadingWhite|NumberStyles.AllowTrailingWhite,CultureInfo.CurrentCulture,out var n)||n<0||n>100000000||decimal.Round(n,2)!=n)return false;cents=(long)(n*100);return true;}
 void SelectMethod(object sender,RoutedEventArgs e){method=(string)((Button)sender).Tag;CardPanel.Visibility=method=="cash"?Visibility.Collapsed:Visibility.Visible;CashPanel.Visibility=method=="card"?Visibility.Collapsed:Visibility.Visible;CardInput.IsReadOnly=method=="card";Authorized.IsChecked=false;CardInput.Text=method=="card"?(total/100m).ToString("0.00"):"0";UpdateSuggestions();UpdateState();}
 void AmountsChanged(object sender,TextChangedEventArgs e){if(!initialized)return;if(sender==CardInput){Authorized.IsChecked=false;UpdateSuggestions();}UpdateState();}
 void AuthorizationChanged(object sender,RoutedEventArgs e){if(initialized)UpdateState();}
 void UpdateSuggestions(){Suggested.Children.Clear();if(!Parse(CardInput.Text,out var card))return;var due=total-(method=="cash"?0:card);if(due<0)return;foreach(var value in Payments.Suggestions(due)){var b=new Button{Content=value==due?$"Exacto {Money(value)}":Money(value),Tag=value,Background=new SolidColorBrush(Color.FromRgb(236,232,246))};b.Click+=(_,_)=>CashInput.Text=((long)b.Tag/100m).ToString("0.00");Suggested.Children.Add(b);}}
 void UpdateState()
 {
  var cashValid=method=="card"||Parse(CashInput.Text,out _);var cardValid=method=="cash"||Parse(CardInput.Text,out _);Parse(CashInput.Text,out var cash);Parse(CardInput.Text,out var card);if(method=="cash")card=0;if(method=="card")cash=0;
  var state=Payments.Evaluate(total,cash,card);var authorized=card==0||Authorized.IsChecked==true;
  Confirm.IsEnabled=cashValid&&cardValid&&state.Valid&&authorized;
  ChangeLabel.Text=Confirm.IsEnabled?Money(state.Change):"—";
  PaymentHint.Text=!cashValid||!cardValid?"Escribe el importe recibido.":state.Missing>0?$"Faltan {Money(state.Missing)}":!state.Valid?state.Error:!authorized?"Confirma la autorización de la terminal.":state.Change==0?"Pago exacto. Sin cambio.":$"Entrega {Money(state.Change)} al cliente.";
  Validation.Text=!cashValid&&!string.IsNullOrWhiteSpace(CashInput.Text)?"Usa un importe válido con hasta dos decimales.":!cardValid?"Revisa el importe de tarjeta.":state.Error;
  CashReceived=cash;CardPaid=card;
  foreach(var b in new[]{CashMethod,CardMethod,MixedMethod}){var active=(string)b.Tag==method;b.Background=new SolidColorBrush((Color)ColorConverter.ConvertFromString(active?"#D73680":"#ECE8F6"));b.Foreground=active?Brushes.White:new SolidColorBrush(Color.FromRgb(50,45,70));}
 }
 void ConfirmPayment(object sender,RoutedEventArgs e){UpdateState();if(Confirm.IsEnabled)DialogResult=true;}
 void Cancel(object sender,RoutedEventArgs e)=>DialogResult=false;
}
