namespace Pinwi.Pos;
public sealed class DiscountWindow : OperationWindow
{
 public string Reason {get;private set;}="";
 public string Authorization {get;private set;}="";
 public DiscountWindow(Store store,SaleViewModel vm):base("Descuentos",620)
 {
  Note(vm.Policy.StackDiscounts?"La configuración permite acumular descuentos con mayoreo.":"Los descuentos solo se aplican a partidas a menudeo.");
  Note("Se aplica primero el descuento de cada partida y después el general. El total siempre se conserva en centavos.");
  var products=vm.Cart.Select(x=>x.Product).DistinctBy(x=>x.Id).ToList();
  var scope=Choice("Aplicar a",new[]{"Toda la venta"}.Concat(products.Select(x=>$"{x.Id} · {x.Category} — {x.Name}")));
  var type=Choice("Tipo",new[]{"Porcentaje","Importe fijo (MXN)"});
  type.SelectedIndex=vm.ExtraDiscounts.GlobalAmount>0?1:0;
  var value=Text("Valor",vm.ExtraDiscounts.GlobalAmount>0?(vm.ExtraDiscounts.GlobalAmount/100m).ToString():vm.Discount.ToString());
  var clear=Check("Quitar todos los descuentos de esta venta");
  var reason=Text("Motivo");var password=Secret("Contraseña del administrador");
  scope.SelectionChanged+=(_,_)=>
  {
   if(scope.SelectedIndex==0){type.SelectedIndex=vm.ExtraDiscounts.GlobalAmount>0?1:0;value.Text=vm.ExtraDiscounts.GlobalAmount>0?(vm.ExtraDiscounts.GlobalAmount/100m).ToString():vm.Discount.ToString();}
   else{var discount=vm.ExtraDiscounts.PerProduct?.GetValueOrDefault(products[scope.SelectedIndex-1].Id)??new();type.SelectedIndex=discount.Amount>0?1:0;value.Text=discount.Amount>0?(discount.Amount/100m).ToString():discount.Percent.ToString();}
  };
  OnSave(async()=>
  {
   if(!decimal.TryParse(value.Text,out var number)||number<0||(type.SelectedIndex==0&&number>100))throw new Exception("Revisa el valor del descuento.");
   var percent=vm.Discount;var amount=vm.ExtraDiscounts.GlobalAmount;var per=vm.ExtraDiscounts.PerProduct==null?new Dictionary<int,LineDiscount>():new(vm.ExtraDiscounts.PerProduct);
   if(clear.IsChecked==true){percent=0;amount=0;per.Clear();}
   else if(scope.SelectedIndex==0){percent=type.SelectedIndex==0?number:0;amount=type.SelectedIndex==1?Money(value.Text):0;}
   else per[products[scope.SelectedIndex-1].Id]=type.SelectedIndex==0?new(number):new(0,Money(value.Text));
   var plan=new DiscountPlan(amount,per);Pricing.Calculate(vm.Cart,percent,vm.Policy.StackDiscounts,vm.Policy,plan);
   var auth=password.Password;if(string.IsNullOrWhiteSpace(reason.Text))throw new Exception("Indica un motivo.");await Task.Run(()=>store.RequireAdmin(auth));
   Reason=reason.Text.Trim();Authorization=auth;vm.Discount=percent;vm.ExtraDiscounts=plan;vm.Refresh();
  },"Aplicar descuento");
 }
}
