using System.IO;
using System.Text.Json;
using Microsoft.Data.Sqlite;
namespace Pinwi.Pos;
public record DraftLine(int ProductId,int Quantity);
public record SaleDraft(string Token,string Owner,List<DraftLine> Lines,decimal Discount,string Reason,string Saved,DiscountPlan? ExtraDiscounts=null);
public sealed partial class Store
{
 public void SaveDraft(SaleDraft draft)
 {
  if(CurrentUserName==null||draft.Owner!=CurrentUserName)throw new Exception("La venta pendiente no corresponde a la sesión.");
  using var c=Open();using var t=c.BeginTransaction();
  if(Cmd(c,"SELECT 1 FROM sales WHERE token=@p0",draft.Token).ExecuteScalar()!=null)return;
  Cmd(c,"INSERT INTO drafts(owner,token,payload,saved) VALUES(@p0,@p1,@p2,@p3) ON CONFLICT(owner) DO UPDATE SET token=excluded.token,payload=excluded.payload,saved=excluded.saved",draft.Owner,draft.Token,JsonSerializer.Serialize(draft),draft.Saved).ExecuteNonQuery();t.Commit();NotifyChanged();
 }
 public SaleDraft? LoadDraft()
 {
  if(CurrentUserName==null)return null;using var c=Open();var payload=Cmd(c,"SELECT payload FROM drafts WHERE owner=@p0 AND token NOT IN (SELECT token FROM sales)",CurrentUserName).ExecuteScalar() as string;return payload==null?null:JsonSerializer.Deserialize<SaleDraft>(payload);
 }
}
public sealed class AutoProtection : IDisposable
{
 readonly Store store;readonly CancellationTokenSource stop=new();readonly SemaphoreSlim requests=new(0,1);readonly Task worker;readonly TimeSpan interval;readonly TimeSpan coalesce;int pending;
 public string Status {get;private set;}="Preparando respaldo automático…";
 public AutoProtection(Store store,TimeSpan? interval=null,TimeSpan? coalesce=null){this.store=store;this.interval=interval??TimeSpan.FromMinutes(5);this.coalesce=coalesce??TimeSpan.FromSeconds(1);if(this.interval<=TimeSpan.Zero||this.coalesce<TimeSpan.Zero)throw new ArgumentException("Intervalo inválido.");store.Changed+=Request;worker=Task.Run(Work);Request();}
 public void Request(){if(stop.IsCancellationRequested)return;if(Interlocked.Exchange(ref pending,1)==0)try{requests.Release();}catch(SemaphoreFullException){}}
 async Task Work()
 {
  while(!stop.IsCancellationRequested)
  {
   try
   {
    // También respalda sin actividad; agrupa ráfagas de cambios en una sola copia.
    await requests.WaitAsync(interval,stop.Token);await Task.Delay(coalesce,stop.Token);Interlocked.Exchange(ref pending,0);
    var folder=Path.Combine(store.DirectoryPath,"Respaldos","Automaticos");Directory.CreateDirectory(folder);
    var path=Path.Combine(folder,$"pinwi-{DateTime.Now:yyyyMMdd-HHmmss-fff}-{Guid.NewGuid():N}.db");
    store.Backup(path);
    KeepCalendarCopies(folder);
    Status=$"Respaldo automático: {DateTime.Now:t}";
    var external=store.GetSettings().BackupDirectory;
    if(external.Length>0)
    {
     try{Directory.CreateDirectory(external);var copy=Path.Combine(external,Path.GetFileName(path));store.Backup(copy);KeepCalendarCopies(external);TrimRecent(external);}
     catch(Exception ex){Status="Respaldo local guardado; falló la copia externa. Revisa la unidad.";Log(ex);}
    }
    TrimRecent(folder);
   }
   catch(OperationCanceledException){break;}
   catch(Exception ex){Status="No se pudo crear el respaldo automático. Revisa disco y permisos.";Log(ex);}
  }
 }
 public void Dispose(){store.Changed-=Request;stop.Cancel();}
 public async Task StopAsync(){Dispose();await worker.ConfigureAwait(false);}
 void Log(Exception ex){try{File.AppendAllText(Path.Combine(store.DirectoryPath,"respaldo-errores.log"),$"{DateTimeOffset.Now:O} {ex.Message}{Environment.NewLine}");}catch(Exception){}}
 static void TrimRecent(string folder){foreach(var old in new DirectoryInfo(folder).GetFiles("pinwi-*.db").OrderByDescending(f=>f.Name).Skip(288))old.Delete();}
 void KeepCalendarCopies(string folder)
 {
  var date=BusinessDay.Bounds(DateTimeOffset.Now).Start.Date;
  var daily=Path.Combine(folder,"Diarios");var weekly=Path.Combine(folder,"Semanales");Directory.CreateDirectory(daily);Directory.CreateDirectory(weekly);
  // Separate buckets cannot be displaced by a busy day's short-term copies.
  var dayPath=Path.Combine(daily,$"diario-{date:yyyyMMdd}.db");
  var week=date.AddDays(-((7+(int)date.DayOfWeek-(int)DayOfWeek.Monday)%7));var weekPath=Path.Combine(weekly,$"semanal-{week:yyyyMMdd}.db");
  if(!File.Exists(dayPath))store.Backup(dayPath);
  if(!File.Exists(weekPath))store.Backup(weekPath);
  foreach(var old in new DirectoryInfo(daily).GetFiles("diario-*.db").OrderByDescending(f=>f.Name).Skip(30))old.Delete();
  foreach(var old in new DirectoryInfo(weekly).GetFiles("semanal-*.db").OrderByDescending(f=>f.Name).Skip(12))old.Delete();
 }
}
