using Microsoft.Data.Sqlite;
namespace Pinwi.Pos;

public sealed partial class Store
{
 // Each migration is additive and runs inside the initialization transaction.
 static void AddColumn(SqliteConnection c,string table,string column,string declaration)
 {
  var found=false;
  using(var r=Cmd(c,$"PRAGMA table_info({table})").ExecuteReader())while(r.Read())found|=r.GetString(1)==column;
  if(!found)Cmd(c,$"ALTER TABLE {table} ADD COLUMN {column} {declaration}").ExecuteNonQuery();
 }
 static void MigrateV8(SqliteConnection c)
 {
  AddColumn(c,"users","role","TEXT NOT NULL DEFAULT 'Administrador'");
  AddColumn(c,"users","active","INTEGER NOT NULL DEFAULT 1");
  AddColumn(c,"products","active","INTEGER NOT NULL DEFAULT 1");
  AddColumn(c,"products","barcode","TEXT NOT NULL DEFAULT ''");
  AddColumn(c,"sales","rules","TEXT NOT NULL DEFAULT ''");
  AddColumn(c,"lines","retail","INTEGER");
  AddColumn(c,"lines","kind","TEXT NOT NULL DEFAULT 'Sin clasificaciÃ³n histÃ³rica'");
  Cmd(c,"""
  CREATE TABLE IF NOT EXISTS settings(key TEXT PRIMARY KEY,value TEXT NOT NULL);
  CREATE UNIQUE INDEX IF NOT EXISTS product_barcode ON products(barcode) WHERE barcode<>'';
  CREATE INDEX IF NOT EXISTS sales_shift ON sales(shift);
  CREATE INDEX IF NOT EXISTS sales_business_dates ON sales(julianday(date));
  CREATE INDEX IF NOT EXISTS lines_sale ON lines(sale);
  CREATE TABLE IF NOT EXISTS cash_movements(id INTEGER PRIMARY KEY,token TEXT NOT NULL UNIQUE,date TEXT NOT NULL,shift INTEGER NOT NULL REFERENCES shifts(id),cashier TEXT NOT NULL,amount INTEGER NOT NULL CHECK(amount<>0),reason TEXT NOT NULL);
  CREATE INDEX IF NOT EXISTS cash_movements_shift ON cash_movements(shift);
  CREATE TABLE IF NOT EXISTS refunds(id INTEGER PRIMARY KEY,token TEXT NOT NULL UNIQUE,sale INTEGER NOT NULL REFERENCES sales(id),shift INTEGER NOT NULL REFERENCES shifts(id),date TEXT NOT NULL,cashier TEXT NOT NULL,authorizer TEXT NOT NULL,reason TEXT NOT NULL,total INTEGER NOT NULL CHECK(total>=0),cash INTEGER NOT NULL CHECK(cash>=0),card INTEGER NOT NULL CHECK(card>=0),kind TEXT NOT NULL);
  CREATE TABLE IF NOT EXISTS refund_lines(refund INTEGER NOT NULL REFERENCES refunds(id),product INTEGER NOT NULL REFERENCES products(id),quantity INTEGER NOT NULL CHECK(quantity>0),amount INTEGER NOT NULL CHECK(amount>=0),restocked INTEGER NOT NULL DEFAULT 0);
  CREATE INDEX IF NOT EXISTS refunds_sale ON refunds(sale);
  CREATE INDEX IF NOT EXISTS refunds_shift ON refunds(shift);
  CREATE INDEX IF NOT EXISTS refunds_business_dates ON refunds(julianday(date));
  CREATE TABLE IF NOT EXISTS saved_carts(id TEXT PRIMARY KEY,owner TEXT NOT NULL,label TEXT NOT NULL,payload TEXT NOT NULL,saved TEXT NOT NULL);
  """).ExecuteNonQuery();
  AddColumn(c,"cut_reports","adjustments_csv","TEXT NOT NULL DEFAULT ''");
  AddColumn(c,"refunds","receipt","TEXT NOT NULL DEFAULT ''");
 }
}
