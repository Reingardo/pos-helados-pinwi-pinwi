namespace Pinwi.Pos;
public record LineDiscount(decimal Percent=0,long Amount=0);
public record DiscountPlan(long GlobalAmount=0,Dictionary<int,LineDiscount>? PerProduct=null)
{
 public bool Any=>GlobalAmount>0||PerProduct?.Values.Any(x=>x.Percent>0||x.Amount>0)==true;
}
public static class Discounts
{
 public static List<PricedLine> Apply(List<PricedLine> source,decimal percent,bool stack,DiscountPlan? plan)
 {
  plan??=new();
  if(plan.GlobalAmount<0||plan.PerProduct?.Values.Any(x=>x.Percent<0||x.Percent>100||x.Amount<0||x.Percent>0&&x.Amount>0)==true)throw new ArgumentException("Revisa los descuentos por partida.");
  var lines=source.Select(x=>
  {
   if(x.IsWholesale&&!stack)return x;
   var discount=plan.PerProduct?.GetValueOrDefault(x.ProductId)??new();
   var lineAmount=discount.Amount+(long)decimal.Round((decimal)x.Gross*discount.Percent/100,0,MidpointRounding.AwayFromZero);
   if(lineAmount>x.Gross)throw new ArgumentException("El descuento supera el importe de una partida.");
   var global=(long)decimal.Round((decimal)(x.Gross-lineAmount)*percent/100,0,MidpointRounding.AwayFromZero);
   return x with{Discount=lineAmount+global};
  }).ToList();
  var eligible=lines.Where(x=>!x.IsWholesale||stack).ToList();var net=eligible.Sum(x=>x.Net);
  if(plan.GlobalAmount>net)throw new ArgumentException("El descuento fijo supera el total elegible. Revisa si hay partidas a mayoreo.");
  if(plan.GlobalAmount==0)return lines;
  var shares=eligible.Select(x=>{var value=(decimal)x.Net*plan.GlobalAmount/net;return new{Id=x.ProductId,Amount=(long)decimal.Floor(value),Remainder=value-decimal.Floor(value)};}).ToList();
  var remaining=plan.GlobalAmount-shares.Sum(x=>x.Amount);
  var extra=shares.OrderByDescending(x=>x.Remainder).ThenBy(x=>x.Id).Take((int)remaining).Select(x=>x.Id).ToHashSet();
  var amounts=shares.ToDictionary(x=>x.Id,x=>x.Amount+(extra.Contains(x.Id)?1:0));
  return lines.Select(x=>x with{Discount=x.Discount+amounts.GetValueOrDefault(x.ProductId)}).ToList();
 }
}
