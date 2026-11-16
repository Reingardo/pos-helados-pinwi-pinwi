using System.Text;
using System.Text.Json;
namespace Pinwi.Pos;
public record ReceiptBrand(string Name,string Phone,string Facebook,string Address)
{
 const string Prefix="[PINWI-BRAND:";
 public static ReceiptBrand From(PosSettings settings)=>new(settings.BusinessName,settings.Phone,settings.Facebook,settings.Address);
 public string Wrap(string receipt)=>Prefix+Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(this)))+"]\n"+receipt;
 public static (ReceiptBrand Brand,string Body) Read(string receipt)
 {
  var fallback=From(new());
  if(!receipt.StartsWith(Prefix))return(fallback,receipt);
  var end=receipt.IndexOf("]\n",StringComparison.Ordinal);if(end<0)return(fallback,receipt);
  try{return(JsonSerializer.Deserialize<ReceiptBrand>(Encoding.UTF8.GetString(Convert.FromBase64String(receipt[Prefix.Length..end])))??fallback,receipt[(end+2)..]);}
  catch(Exception){return(fallback,receipt[(end+2)..]);}
 }
}
