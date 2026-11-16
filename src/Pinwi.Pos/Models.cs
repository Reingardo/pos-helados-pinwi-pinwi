using System.ComponentModel;
using System.Runtime.CompilerServices;
namespace Pinwi.Pos;
public record Product(int Id, string Category, string Name, long Retail, long Wholesale, int Stock, string Image)
{
 public string Family => Category.StartsWith("Pinwis") ? "Bolis / Pinwis" : Category.StartsWith("Heladitos") ? "Bollos / Heladitos" : Category;
 public string Caption => $"{Name}\n${Retail / 100m:0.00} · Mayoreo ${Wholesale / 100m:0.00}";
 public decimal RetailPesos => Retail / 100m;
 public decimal WholesalePesos => Wholesale / 100m;
 public string StockWarning => Stock<=0 ? "Producto sin existencia" : "";
 public string CategoryImagePath => System.IO.Path.Combine(AppContext.BaseDirectory, "Assets", Image);
 public string ImagePath => System.IO.File.Exists(System.IO.Path.Combine(AppContext.BaseDirectory,"Assets","Products",$"{Id}.png")) ? System.IO.Path.Combine(AppContext.BaseDirectory,"Assets","Products",$"{Id}.png") : CategoryImagePath;
}
public record CartItem(Product Product, int Quantity);
public record PricedLine(int ProductId, string Name, int Quantity, long UnitPrice, bool IsWholesale, long Discount, string StockWarning = "", string ImagePath = "")
{
 public long Gross => UnitPrice * Quantity;
 public long Net => Gross - Discount;
 public string Mode => IsWholesale ? "Mayoreo" : "Menudeo";
 public decimal Price => UnitPrice / 100m;
 public decimal Total => Net / 100m;
}
public static class Pricing
{
 public static List<PricedLine> Calculate(IEnumerable<CartItem> source, decimal percent = 0, bool stack = false)
 {
  if (percent < 0 || percent > 100) throw new ArgumentException("El descuento debe estar entre 0 y 100%.");
  var items = source.GroupBy(x=>x.Product.Id).Select(g=>new CartItem(g.First().Product,g.Sum(x=>x.Quantity))).ToList();
  if (items.Any(x=>x.Quantity <= 0)) throw new ArgumentException("Cantidad inválida.");
  var global = items.Sum(x=>checked(x.Product.Retail*x.Quantity)) >= 40000;
  return items.Select(x=> { var wholesale = global || x.Quantity >= 10; var price = wholesale ? x.Product.Wholesale : x.Product.Retail; var discount = wholesale && !stack ? 0 : (long)decimal.Round(price*x.Quantity*percent/100m,0,MidpointRounding.AwayFromZero); return new PricedLine(x.Product.Id,x.Product.Category+" — "+x.Product.Name,x.Quantity,price,wholesale,discount,x.Quantity>x.Product.Stock?"Producto sin existencia suficiente registrada":"",x.Product.ImagePath); }).ToList();
 }
}
public class SaleViewModel : INotifyPropertyChanged
{
 public void SetQuantity(int productId,int quantity)
 {
  if(quantity<0 || quantity>100000)throw new ArgumentException("Captura entre 0 y 100,000 piezas.");
  var index=Cart.FindIndex(x=>x.Product.Id==productId);
  if(index<0)return;
  if(quantity==0)Cart.RemoveAt(index);else Cart[index]=Cart[index] with {Quantity=quantity};
  Refresh();
 }
 public List<CartItem> Cart { get; } = [];
 public decimal Discount { get; set; }
 public List<PricedLine> Lines => Pricing.Calculate(Cart,Discount);
 public long TotalCents => Lines.Sum(x=>x.Net);
 public string Total => $"${TotalCents/100m:N2}";
 public string Savings => $"Ahorro mayoreo: ${(Cart.Sum(x=>x.Product.Retail*x.Quantity)-Lines.Sum(x=>x.Gross))/100m:N2}   Descuento: ${Lines.Sum(x=>x.Discount)/100m:N2}";
 public event PropertyChangedEventHandler? PropertyChanged;
 public void Refresh() { foreach(var p in new[]{nameof(Lines),nameof(Total),nameof(Savings)}) PropertyChanged?.Invoke(this,new PropertyChangedEventArgs(p)); }
}
