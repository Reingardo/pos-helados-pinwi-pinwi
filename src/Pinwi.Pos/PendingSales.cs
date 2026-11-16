using System.Text.Json;
namespace Pinwi.Pos;
public record PendingSale(string Id,string Label,string Saved);
public sealed partial class Store
{
 public List<PendingSale> PendingSales()
 {
  if(CurrentUserName==null)throw new Exception("Inicia sesión.");
  using var c=Open();using var r=Cmd(c,"SELECT id,label,saved FROM saved_carts WHERE owner=@p0 ORDER BY saved DESC",CurrentUserName).ExecuteReader();
  var result=new List<PendingSale>();while(r.Read())result.Add(new(r.GetString(0),r.GetString(1),r.GetString(2)));return result;
 }
 public void ParkSale(string label)
 {
  if(CurrentUserName==null)throw new Exception("Inicia sesión.");label=label.Trim();if(label.Length<2||label.Length>100||label.Any(char.IsControl))throw new Exception("Captura una referencia de 2 a 100 caracteres.");
  using var c=Open();using var t=c.BeginTransaction();var payload=Cmd(c,"SELECT payload FROM drafts WHERE owner=@p0",CurrentUserName).ExecuteScalar() as string??throw new Exception("No hay una venta guardada.");
  var draft=JsonSerializer.Deserialize<SaleDraft>(payload)??throw new Exception("No se pudo leer el carrito.");if(draft.Lines.Count==0)throw new Exception("Agrega productos antes de guardar la venta.");
  if(Cmd(c,"SELECT 1 FROM sales WHERE token=@p0",draft.Token).ExecuteScalar()!=null)throw new Exception("La venta ya fue cobrada.");
  Cmd(c,"INSERT INTO saved_carts(id,owner,label,payload,saved) VALUES(@p0,@p1,@p2,@p3,@p4)",draft.Token,CurrentUserName,label,payload,DateTimeOffset.Now.ToString("O")).ExecuteNonQuery();
  Cmd(c,"DELETE FROM drafts WHERE owner=@p0",CurrentUserName).ExecuteNonQuery();t.Commit();NotifyChanged();
 }
 public void ResumeSale(string id)
 {
  if(CurrentUserName==null)throw new Exception("Inicia sesión.");using var c=Open();using var t=c.BeginTransaction();
  var current=Cmd(c,"SELECT payload FROM drafts WHERE owner=@p0",CurrentUserName).ExecuteScalar() as string;
  if(current!=null&&JsonSerializer.Deserialize<SaleDraft>(current)?.Lines.Count>0)throw new Exception("Guarda o vacía el carrito actual antes de recuperar otro.");
  var payload=Cmd(c,"SELECT payload FROM saved_carts WHERE owner=@p0 AND id=@p1",CurrentUserName,id).ExecuteScalar() as string??throw new Exception("Venta pendiente no encontrada.");
  var draft=JsonSerializer.Deserialize<SaleDraft>(payload)??throw new Exception("Venta pendiente inválida.");
  if(Cmd(c,"SELECT 1 FROM sales WHERE token=@p0",draft.Token).ExecuteScalar()!=null)throw new Exception("La venta ya fue cobrada.");
  Cmd(c,"INSERT INTO drafts(owner,token,payload,saved) VALUES(@p0,@p1,@p2,@p3) ON CONFLICT(owner) DO UPDATE SET token=excluded.token,payload=excluded.payload,saved=excluded.saved",CurrentUserName,draft.Token,payload,draft.Saved).ExecuteNonQuery();
  Cmd(c,"DELETE FROM saved_carts WHERE id=@p0 AND owner=@p1",id,CurrentUserName).ExecuteNonQuery();t.Commit();NotifyChanged();
 }
}
