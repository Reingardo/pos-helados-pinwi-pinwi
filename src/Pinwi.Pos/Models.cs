using System.ComponentModel;
using System.Runtime.CompilerServices;
namespace Pinwi.Pos;
public record Product(int Id, string Category, string Name, long Retail, long Wholesale, int Stock, string Image)
{
 public bool Active {get;init;}=true;
 public string Barcode {get;init;}="";
 [System.Text.Json.Serialization.JsonIgnore]
 public string? CustomImagePath { get; init; }
 public string? QuickGroup { get; init; }
 public bool QuickReady { get; init; } = true;
 public bool IsQuick => QuickGroup != null;
 public string WholesaleGroup => ProductGroups.Key(this);
 public string DisplayName => Category == "Paquetes" ? Name.Split(" — ")[0] : Name;
 public string Detail => IsQuick ? "Por gama · sin elegir sabor" : Category == "Paquetes" && Name.Contains(" — ") ? Name[(Name.IndexOf(" — ")+3)..] : Category;
 public string PriceNote => Category == "Paquetes" ? "Precio por paquete" : $"Mayoreo ${Wholesale / 100m:0.00}";
 public string Family => Category.StartsWith("Pinwis") ? "Bolis / Pinwis" : Category.StartsWith("Heladitos") ? "Bollos / Heladitos" : Category;
 public string Caption => Category == "Paquetes" ? $"{Name}\n${Retail / 100m:0.00} por paquete" : $"{Name}\n${Retail / 100m:0.00} · Mayoreo ${Wholesale / 100m:0.00}";
 public decimal RetailPesos => Retail / 100m;
 public decimal WholesalePesos => Wholesale / 100m;
 public string StockWarning => !Active ? "Producto desactivado" : !QuickReady ? "Configura los precios de esta gama" : IsQuick ? "" : Stock<=0 ? "Producto sin existencia" : "";
 public string CategoryImagePath => CustomImagePath ?? System.IO.Path.Combine(AppContext.BaseDirectory, "Assets", Image);
 public string ImagePath => CustomImagePath ?? (System.IO.File.Exists(System.IO.Path.Combine(AppContext.BaseDirectory,"Assets","Products",$"{Id}.png")) ? System.IO.Path.Combine(AppContext.BaseDirectory,"Assets","Products",$"{Id}.png") : CategoryImagePath);
}
public record CartItem(Product Product, int Quantity);
public record PricedLine(int ProductId, string Name, int Quantity, long UnitPrice, bool IsWholesale, long Discount, string StockWarning = "", string ImagePath = "",string WholesaleReason="")
{
 public long Gross => UnitPrice * Quantity;
 public long Net => Gross - Discount;
 public string Mode => Name.StartsWith("Paquetes — ") ? "Precio de paquete" : IsWholesale ? "Mayoreo" : "Menudeo";
 public decimal Price => UnitPrice / 100m;
 public decimal Total => Net / 100m;
}
public static class Pricing
{
 public static List<PricedLine> Calculate(IEnumerable<CartItem> source, decimal percent = 0, bool stack = false, PricingPolicy? policy=null,DiscountPlan? plan=null)
 {
  if (percent < 0 || percent > 100) throw new ArgumentException("El descuento debe estar entre 0 y 100%.");
  var items = source.GroupBy(x=>x.Product.Id).Select(g=>new CartItem(g.First().Product,g.Sum(x=>x.Quantity))).ToList();
  if (items.Any(x=>x.Quantity <= 0)) throw new ArgumentException("Cantidad inválida.");
  policy??=new();
  var global = items.Sum(x=>checked(x.Product.Retail*x.Quantity)) >= policy.GlobalCents;
  var groupQuantities=items.GroupBy(x=>x.Product.WholesaleGroup).ToDictionary(g=>g.Key,g=>g.Sum(x=>x.Quantity));
  var lines=items.Select(x=> { var wholesale = global || groupQuantities[x.Product.WholesaleGroup] >= policy.Quantity; var price = wholesale ? x.Product.Wholesale : x.Product.Retail;return new PricedLine(x.Product.Id,x.Product.Category+" — "+x.Product.Name,x.Quantity,price,wholesale,0,!x.Product.Active?"Producto desactivado; reactívalo o retíralo del carrito":!x.Product.IsQuick && x.Quantity>x.Product.Stock?"Producto sin existencia suficiente registrada":"",x.Product.ImagePath,wholesale?(global?$"Consumo a menudeo desde {policy.GlobalCents/100m:C}":$"Desde {policy.Quantity} piezas de la misma {(x.Product.Category=="Esquimales"?"presentación":"gama")}"):""); }).ToList();
  return Discounts.Apply(lines,percent,stack,plan);
 }
}
public class SaleViewModel : INotifyPropertyChanged
{
 public void SetQuantity(int productId,int quantity)
 {
  if(quantity<0 || quantity>100000)throw new ArgumentException("Captura entre 0 y 100,000 piezas.");
  var index=Cart.FindIndex(x=>x.Product.Id==productId);
  if(index<0)return;
  var previous=Cart[index];
  if(quantity==0)Cart.RemoveAt(index);else Cart[index]=previous with {Quantity=quantity};
  try{Refresh();}catch{if(quantity==0)Cart.Insert(index,previous);else Cart[index]=previous;priced=null;throw;}
 }
 public List<CartItem> Cart { get; } = [];
 public decimal Discount { get; set; }
 public PricingPolicy Policy {get;set;}=new();
 public DiscountPlan ExtraDiscounts {get;set;}=new();
 public string DiscountNotice {get;private set;}="";
 List<PricedLine>? priced;
 public List<PricedLine> Lines => priced??=Pricing.Calculate(Cart,Discount,Policy.StackDiscounts,Policy,ExtraDiscounts);
 public long TotalCents => Lines.Sum(x=>x.Net);
 public string Total => $"${TotalCents/100m:N2}";
 public string Savings => $"Ahorro mayoreo: ${(Cart.Sum(x=>x.Product.Retail*x.Quantity)-Lines.Sum(x=>x.Gross))/100m:N2}   Descuento: ${Lines.Sum(x=>x.Discount)/100m:N2}"+(DiscountNotice.Length>0?"\n"+DiscountNotice:"");
 public event PropertyChangedEventHandler? PropertyChanged;
 public void Refresh()
 {
  DiscountNotice="";
  try{priced=Pricing.Calculate(Cart,Discount,Policy.StackDiscounts,Policy,ExtraDiscounts);}
  catch(ArgumentException) when(ExtraDiscounts.Any)
  {
   ExtraDiscounts=new();Discount=0;DiscountNotice="El carrito cambió y el descuento ya no es válido. Aplícalo de nuevo si corresponde.";
   priced=Pricing.Calculate(Cart,0,Policy.StackDiscounts,Policy);
  }
  foreach(var p in new[]{nameof(Lines),nameof(Total),nameof(Savings)}) PropertyChanged?.Invoke(this,new PropertyChangedEventArgs(p));
 }
}
