using System.Security.Cryptography;
namespace Pinwi.Pos;

public record UserAccount(long Id,string Name,string Role,bool Active);
public sealed partial class Store
{
 public string? CurrentRole {get;private set;}
 public string? AdminAuthorizer(string password)
 {
  using var c=Open();using var r=Cmd(c,"SELECT name,salt,hash FROM users WHERE role='Administrador' AND active=1 ORDER BY id").ExecuteReader();
  while(r.Read())if(CryptographicOperations.FixedTimeEquals(Rfc2898DeriveBytes.Pbkdf2(password,(byte[])r[1],600000,HashAlgorithmName.SHA256,32),(byte[])r[2]))return r.GetString(0);
  return null;
 }
 public bool IsAdmin=>CurrentUserName!=null&&CurrentRole=="Administrador";
 public List<UserAccount> Users()
 {
  using var c=Open();using var r=Cmd(c,"SELECT id,name,role,active FROM users ORDER BY id").ExecuteReader();
  var rows=new List<UserAccount>();while(r.Read())rows.Add(new(r.GetInt64(0),r.GetString(1),r.GetString(2),r.GetInt32(3)==1));return rows;
 }
 public void SignOut(){CurrentUserName=null;CurrentRole=null;}
 public bool SignIn(string name,string password)
 {
  using var c=Open();using var r=Cmd(c,"SELECT name,role,salt,hash FROM users WHERE name=@p0 AND active=1",name).ExecuteReader();
  if(!r.Read()||!CryptographicOperations.FixedTimeEquals(Rfc2898DeriveBytes.Pbkdf2(password,(byte[])r[2],600000,HashAlgorithmName.SHA256,32),(byte[])r[3]))return false;
  CurrentUserName=r.GetString(0);CurrentRole=r.GetString(1);return true;
 }
 public void RequireAdmin(string password)
 {
  if(CurrentUserName==null||!Authenticate(password))throw new Exception("Autoriza con la contraseña del administrador.");
 }
 public long SaveUser(long? id,string name,string role,bool active,string? newPassword,string adminPassword)
 {
  RequireAdmin(adminPassword);
  name=name.Trim();
  if(name.Length<2||name.Length>100||name.Any(char.IsControl)||role is not ("Administrador" or "Cajero")||newPassword is {Length:<8})throw new Exception("Revisa nombre, rol y contraseña (mínimo ocho caracteres).");
  if(id==null&&string.IsNullOrEmpty(newPassword))throw new Exception("Captura la contraseña del nuevo usuario.");
  using var c=Open();using var t=c.BeginTransaction();
  if(id!=null)
  {
   var oldName=(string?)Cmd(c,"SELECT name FROM users WHERE id=@p0",id.Value).ExecuteScalar()??throw new Exception("Usuario no encontrado.");
   if(oldName==CurrentUserName&&(name!=oldName||!active||role!=CurrentRole))throw new Exception("No puedes cambiar tu nombre, rol o estado durante tu sesión.");
   if(role!="Administrador"||!active)
    if(Convert.ToInt64(Cmd(c,"SELECT COUNT(*) FROM users WHERE role='Administrador' AND active=1 AND id<>@p0",id.Value).ExecuteScalar())==0)throw new Exception("Debe conservarse al menos un administrador activo.");
   if(name!=oldName)
   {
    Cmd(c,"UPDATE drafts SET owner=@p1,payload=json_set(payload,'$.Owner',@p1) WHERE owner=@p0",oldName,name).ExecuteNonQuery();
    Cmd(c,"UPDATE saved_carts SET owner=@p1,payload=json_set(payload,'$.Owner',@p1) WHERE owner=@p0",oldName,name).ExecuteNonQuery();
   }
   Cmd(c,"UPDATE users SET name=@p1,role=@p2,active=@p3 WHERE id=@p0",id.Value,name,role,active).ExecuteNonQuery();
  }
  else Cmd(c,"INSERT INTO users(name,salt,hash,role,active) VALUES(@p0,X'',X'',@p1,@p2)",name,role,active).ExecuteNonQuery();
  var saved=id??Convert.ToInt64(Cmd(c,"SELECT last_insert_rowid()").ExecuteScalar());
  if(!string.IsNullOrEmpty(newPassword))
  {
   var salt=RandomNumberGenerator.GetBytes(32);var hash=Rfc2898DeriveBytes.Pbkdf2(newPassword,salt,600000,HashAlgorithmName.SHA256,32);
   Cmd(c,"UPDATE users SET salt=@p1,hash=@p2 WHERE id=@p0",saved,salt,hash).ExecuteNonQuery();
  }
  Cmd(c,"INSERT INTO audit(date,action) VALUES(@p0,@p1)",DateTimeOffset.Now.ToString("O"),$"{CurrentUserName}: usuario {saved}, rol {role}, activo {active}.").ExecuteNonQuery();t.Commit();NotifyChanged();return saved;
 }
}
