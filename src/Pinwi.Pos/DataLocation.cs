using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;

namespace Pinwi.Pos;

public static class DataLocation
{
 public static bool IsPackaged
 {
  get
  {
   uint length=0;
   var result=GetCurrentPackageFullName(ref length,IntPtr.Zero);
   return result switch {122=>true,15700=>false,0=>true,_=>throw new Win32Exception(result,"No se pudo identificar la edición del POS.")};
  }
 }
 public static string DefaultDirectory=>ForInstallation(IsPackaged,
  Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
  Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData));

 public static string ForInstallation(bool packaged,string userProfile,string localAppData)
 {
  if(string.IsNullOrWhiteSpace(localAppData)||packaged&&string.IsNullOrWhiteSpace(userProfile))
   throw new IOException("No se pudo localizar la carpeta de datos de tu cuenta de Windows.");
  if(!packaged)return Path.GetFullPath(Path.Combine(localAppData,"PinwiPOS"));
  // User-owned business data outside AppData; no virtualization exemption is requested.
  var path=Path.GetFullPath(Path.Combine(userProfile,"PinwiPOS-Store"));
  foreach(var appData in new[]{Path.Combine(userProfile,"AppData"),localAppData})
   if(path.StartsWith(Path.GetFullPath(appData).TrimEnd(Path.DirectorySeparatorChar)+Path.DirectorySeparatorChar,StringComparison.OrdinalIgnoreCase))
    throw new IOException("La carpeta de Store debe estar fuera de AppData.");
  return path;
 }
 [DllImport("kernel32.dll",CharSet=CharSet.Unicode,ExactSpelling=true)]
 static extern int GetCurrentPackageFullName(ref uint packageFullNameLength,IntPtr packageFullName);
}
