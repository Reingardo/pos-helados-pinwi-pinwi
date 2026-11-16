namespace Pinwi.Pos;
public record PaymentState(long Total, long Cash, long Card, bool Valid, string Error)
{
 public long Change => Valid ? Cash + Card - Total : 0;
 public long Missing => Math.Max(0,Total-Cash-Card);
}
public static class Payments
{
 public static PaymentState Evaluate(long total,long cash,long card) => new(total,cash,card,
  total>=0 && cash>=0 && card>=0 && card<=total && cash>=total-card,
  card>total?"La tarjeta supera el total.":cash<0||card<0?"Revisa los importes.":cash<total-card?"El pago todavía no cubre el total.":"");
 public static long[] Suggestions(long due) => new[]{due}.Concat(new long[]{5000,10000,20000,50000,100000}.Select(step=>checked((due+step-1)/step*step))).Distinct().Where(x=>x>=due).Take(4).ToArray();
}
