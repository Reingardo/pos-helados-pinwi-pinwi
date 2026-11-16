using System.Windows;
namespace Pinwi.Pos;
public partial class ExpenseWindow : Window
{
 readonly Store store;readonly string token=Guid.NewGuid().ToString();
 public ExpenseWindow(Store store){this.store=store;InitializeComponent();Loaded+=(_,_)=>AmountInput.Focus();}
 void Cancel(object sender,RoutedEventArgs e)=>DialogResult=false;
 void SaveExpense(object sender,RoutedEventArgs e)
 {
  try{Save.IsEnabled=false;if(!decimal.TryParse(AmountInput.Text,out var value)||value<=0||value>100000000||decimal.Round(value,2)!=value)throw new Exception("Captura un importe positivo con hasta dos decimales.");store.AddExpense((long)(value*100),ConceptInput.Text,RecipientInput.Text,SourceInput.SelectedIndex==0?"Caja":"Externo",token);DialogResult=true;}
  catch(Exception ex){Validation.Text=ex.Message;Save.IsEnabled=true;}
 }
}
