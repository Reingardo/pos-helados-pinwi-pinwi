using System.Windows;
namespace Pinwi.Pos;
public partial class MainWindow
{
 void RegisterExpense(object sender,RoutedEventArgs e)=>Run(()=>
 {
  if(store.Shift()==null)throw new Exception("Abre caja antes de registrar los gastos del día.");
  var window=new ExpenseWindow(store){Owner=this};if(window.ShowDialog()!=true)return;RefreshStats();CashViews.SelectedIndex=1;
 });
 void RefreshExpenses()
 {
  var expenses=store.Expenses(DateTimeOffset.Now);ExpensesGrid.ItemsSource=expenses;
  DailyExpensesLabel.Text=$"Gastos del día: {expenses.Sum(x=>x.Pesos):C}  ·  De caja: {expenses.Where(x=>x.Source=="Caja").Sum(x=>x.Pesos):C}  ·  Externos: {expenses.Where(x=>x.Source=="Externo").Sum(x=>x.Pesos):C}";
 }
}
