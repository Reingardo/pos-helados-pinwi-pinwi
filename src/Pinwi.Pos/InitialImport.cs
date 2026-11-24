using System.IO;
using Microsoft.Data.Sqlite;

namespace Pinwi.Pos;

public sealed partial class Store
{
 // Only for an explicitly chosen backup on first use, before Store/automatic backups start.
 // Existing installations use RestoreBackup with administrator authorization instead.
 public static void ImportFirstBackup(string sourcePath,string destinationDirectory)
 {
  var destination=Path.Combine(Path.GetFullPath(destinationDirectory),"pinwi.db");
  if(File.Exists(destination)||File.Exists(destination+"-wal")||File.Exists(destination+"-shm"))
   throw new IOException("Ya hay datos en esta instalación. Entra y usa Restaurar respaldo con autorización.");
  Directory.CreateDirectory(destinationDirectory);
  var stageFolder=Path.Combine(destinationDirectory,"Importacion",Guid.NewGuid().ToString("N"));
  Directory.CreateDirectory(stageFolder);
  var stagePath=Path.Combine(stageFolder,"pinwi.db");
  using(var source=ReadBackup(sourcePath))
  using(var stage=new SqliteConnection(new SqliteConnectionStringBuilder{DataSource=stagePath,Pooling=false}.ToString()))
  {
   stage.Open();source.BackupDatabase(stage);
   InspectBackup(stage);
  }
  var candidate=new Store(stageFolder); // Migrations affect only this isolated snapshot.
  candidate.InspectBackup(stagePath);
  candidate.Products();candidate.Sales();candidate.Cuts();candidate.Users();candidate.GetSettings();
  var readyPath=Path.Combine(stageFolder,"validado.db");
  using(var source=ReadBackup(stagePath))
  using(var ready=new SqliteConnection(new SqliteConnectionStringBuilder{DataSource=readyPath,Pooling=false}.ToString()))
  {
   ready.Open();source.BackupDatabase(ready);InspectBackup(ready);
   // Publish one self-contained file, without a WAL dependency or pooled open handles.
   Cmd(ready,"PRAGMA journal_mode=DELETE").ExecuteScalar();
  }
  File.Move(readyPath,destination,overwrite:false);
 }
}
