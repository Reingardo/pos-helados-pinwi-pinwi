using System.IO;
using System.Security.Cryptography;
using System.Windows.Media.Imaging;
namespace Pinwi.Pos;
public sealed partial class Store
{
 public static byte[] PrepareProductImage(string path)
 {
  if(new FileInfo(path).Length>20*1024*1024)throw new Exception("Elige una imagen de hasta 20 MB.");
  using var stream=File.OpenRead(path);
  var decoder=BitmapDecoder.Create(stream,BitmapCreateOptions.PreservePixelFormat,BitmapCacheOption.OnLoad);
  var frame=decoder.Frames[0];
  if(frame.PixelWidth>8000||frame.PixelHeight>8000)throw new Exception("La imagen debe medir como máximo 8,000 píxeles por lado.");
  System.Windows.Media.Imaging.BitmapSource image=frame;
  var scale=Math.Min(1,1000d/Math.Max(frame.PixelWidth,frame.PixelHeight));
  if(scale<1)image=new TransformedBitmap(frame,new System.Windows.Media.ScaleTransform(scale,scale));
  var encoder=new PngBitmapEncoder();encoder.Frames.Add(BitmapFrame.Create(image));using var output=new MemoryStream();encoder.Save(output);return output.ToArray();
 }
 public int SaveProduct(int? id,string category,string name,long retail,long wholesale,string reason,string password,byte[]? photo=null)
 {
  if(CurrentUserName==null||!Authenticate(password))throw new Exception("Autoriza con la contraseña del administrador.");
  category=category.Trim();name=name.Trim();reason=reason.Trim();
  if(category.Length==0||category.Length>100||name.Length==0||name.Length>300||reason.Length==0||retail<0||wholesale<0||wholesale>retail)throw new Exception("Revisa nombre, categoría, precios y motivo. Mayoreo no puede superar menudeo.");
  if(photo!=null){if(photo.Length>20*1024*1024)throw new Exception("Imagen demasiado grande.");using var input=new MemoryStream(photo);BitmapDecoder.Create(input,BitmapCreateOptions.PreservePixelFormat,BitmapCacheOption.OnLoad);}
  using var c=Open();using var t=c.BeginTransaction();
  if(Convert.ToInt64(Cmd(c,"SELECT COUNT(*) FROM products WHERE lower(trim(category))=lower(@p0) AND lower(trim(name))=lower(@p1) AND id<>@p2",category,name,id??-1).ExecuteScalar())>0)throw new Exception("Ya existe ese producto en la categoría.");
  int saved;
  if(id is int existing){if(Cmd(c,"UPDATE products SET category=@p1,name=@p2,retail=@p3,wholesale=@p4 WHERE id=@p0",existing,category,name,retail,wholesale).ExecuteNonQuery()!=1)throw new Exception("El producto ya no está disponible.");saved=existing;}
  else{Cmd(c,"INSERT INTO products(category,name,retail,wholesale,stock,image) VALUES(@p0,@p1,@p2,@p3,0,'Brand/logo-pinwi.png')",category,name,retail,wholesale).ExecuteNonQuery();saved=Convert.ToInt32(Cmd(c,"SELECT last_insert_rowid()").ExecuteScalar());}
  if(id!=null)Cmd(c,"UPDATE quick_products SET ready=1 WHERE product=@p0",saved).ExecuteNonQuery();
  if(photo!=null)Cmd(c,"INSERT INTO product_photos(product,image) VALUES(@p0,@p1) ON CONFLICT(product) DO UPDATE SET image=excluded.image",saved,photo).ExecuteNonQuery();
  Cmd(c,"INSERT INTO audit(date,action) VALUES(@p0,@p1)",DateTimeOffset.Now.ToString("O"),$"Usuario {CurrentUserName}. {(id==null?"Alta":"Edición")} producto {saved}: {category} — {name}. Menudeo {retail}, mayoreo {wholesale}, foto nueva {photo!=null}. {reason}").ExecuteNonQuery();t.Commit();NotifyChanged();return saved;
 }
 List<Product> LoadProductPhotos(List<Product> products)
 {
  using var c=Open();using var r=Cmd(c,"SELECT product,image FROM product_photos").ExecuteReader();
  while(r.Read())
  {
   var id=r.GetInt32(0);var bytes=(byte[])r[1];var directory=Path.Combine(DirectoryPath,"Imagenes");Directory.CreateDirectory(directory);
   var path=Path.Combine(directory,Convert.ToHexString(SHA256.HashData(bytes))+".png");if(!File.Exists(path))File.WriteAllBytes(path,bytes);
   var index=products.FindIndex(p=>p.Id==id);if(index>=0)products[index]=products[index] with {CustomImagePath=path};
  }
  return products;
 }
}
