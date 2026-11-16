using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
namespace Pinwi.Pos;

public sealed class SingleInstance : IDisposable
{
 readonly Mutex mutex;
 public bool Acquired { get; }
 public SingleInstance(string databasePath)
 {
  var key=Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(System.IO.Path.GetFullPath(databasePath).ToUpperInvariant())));
  mutex=new Mutex(true,@"Local\PinwiPOS-"+key,out var created);
  Acquired=created;
 }
 public static bool FocusExisting()
 {
  foreach(var process in Process.GetProcessesByName("Pinwi.Pos"))
  using(process)
  {
   if(process.Id==Environment.ProcessId||process.MainWindowHandle==IntPtr.Zero)continue;
   ShowWindow(process.MainWindowHandle,9);
   return SetForegroundWindow(process.MainWindowHandle);
  }
  return false;
 }
 public void Dispose(){if(Acquired)mutex.ReleaseMutex();mutex.Dispose();}
 [DllImport("user32.dll")]static extern bool SetForegroundWindow(IntPtr window);
 [DllImport("user32.dll")]static extern bool ShowWindow(IntPtr window,int command);
}
