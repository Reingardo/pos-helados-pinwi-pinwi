using System.Text.Json;
namespace Pinwi.Pos;

public record PricingPolicy(int Quantity=10,long GlobalCents=40000,bool StackDiscounts=false);
public record PosSettings
{
 public PricingPolicy Pricing {get;init;}=new();
 public string BusinessName {get;init;}="Helados Pinwi Pinwi";
 public string Phone {get;init;}="833 393 2616";
 public string Facebook {get;init;}="facebook.com/heladospinwi";
 public string Address {get;init;}="";
 public string BackupDirectory {get;init;}="";
 public int PaperMm {get;init;}=80;
 public string PrinterName {get;init;}="";
}
public sealed partial class Store
{
 public PosSettings GetSettings()
 {
  using var c=Open();var value=Cmd(c,"SELECT value FROM settings WHERE key='pos'").ExecuteScalar() as string;
  return value==null?new():JsonSerializer.Deserialize<PosSettings>(value)??new();
 }
 public void SaveSettings(PosSettings settings,string password,string reason)
 {
  RequireAdmin(password);
  if(settings.Pricing.Quantity<1||settings.Pricing.Quantity>100000||settings.Pricing.GlobalCents<0||settings.Pricing.GlobalCents>10000000000||settings.PaperMm is not (58 or 80)||string.IsNullOrWhiteSpace(reason)||string.IsNullOrWhiteSpace(settings.BusinessName)||settings.BusinessName.Length>120||settings.Phone.Length>40||settings.Address.Length>300||settings.Facebook.Length>200||new[]{settings.BusinessName,settings.Phone,settings.Address,settings.Facebook}.Any(x=>x.Any(char.IsControl)))throw new Exception("Revisa los datos, umbrales y motivo del cambio.");
  if(settings.BackupDirectory.Length>0)System.IO.Directory.CreateDirectory(settings.BackupDirectory);
  using var c=Open();using var t=c.BeginTransaction();
  Cmd(c,"INSERT INTO settings(key,value) VALUES('pos',@p0) ON CONFLICT(key) DO UPDATE SET value=excluded.value",JsonSerializer.Serialize(settings)).ExecuteNonQuery();
  Cmd(c,"INSERT INTO audit(date,action) VALUES(@p0,@p1)",DateTimeOffset.Now.ToString("O"),$"{CurrentUserName}: configuración modificada. {reason}").ExecuteNonQuery();t.Commit();NotifyChanged();
 }
}
