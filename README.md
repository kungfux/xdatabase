[![Build status](https://ci.appveyor.com/api/projects/status/9ixq897elu0uut74?svg=true)](https://ci.appveyor.com/project/kungfux/xclass)

# XDatabase

XDatabase provides unified methods to interact with various databases. It allows to test and establish a connection to the databases and perform select and update queries. In addition, XDatabase contains a set of methods to work with binary data and images stored in the database.

## Supported databases
* SQLite
* MySql
* MS Access (.mdb)

## Dependency
_Note: The only dependencies must be meet according to databases you are going to use._

* SQLite  
Files: System.Data.SQLite.dll, SQLite.Interop.dll.  
Precompiled binaries may be downloaded from http://www.sqlite.org/ or from http://nuget.org/ `Install-Package System.Data.SQLite.Core`  
* MySql  
Files: MySql.Data.dll  
MySQL Connector/NET may be downloaded from http://www.mysql.com/ or from http://nuget.org/ `Install-Package MySql.Data`
* MS Access (OLEDB)  
Note: To interact with Microsoft Access Database, XDatabase must be built for x86 only!   
Files: System.Data.OleDb.dll - is a part of .NET Framework

## Examples

* Select data as Table, Column, Row or a single Cell
```
var xQuery = new XQuery(XDatabaseType.Sqlite);
xQuery.ConnectionString = "Data Source=:memory:";
var result = xQuery.SelectTable("select * from table;");
var result = xQuery.SelectColumn("select column from table;");
var result = xQuery.SelectRow("select * from table limit 1;");
var result = xQuery.SelectCell<string>("select 'asd';");
var result = xQuery.SelectCell<long>("select 100;");
var result = xQuery.SelectCell<double>("select 1.23;");
```

* Pass arguments
```
var xQuery = new XQuery(XDatabaseType.Sqlite);
xQuery.ConnectionString = "Data Source=:memory:";
var result = xQuery.SelectTable("select * from table where field1=@value;",
    xQuery.AddParameter("@value", "exact value"));
```

* Change data
```
var xQuery = new XQuery(XDatabaseType.Sqlite);
xQuery.ConnectionString = "Data Source=:memory:";
var result = xQuery.ChangeData("update table set field1=@value1",
    xQuery.AddParameter("@value1", "new value"));
if (result >= XResult.ChangesApplied)
   Console.WriteLine("Updated!");
```

* Transactions
```
var xQuery = new XQuery(XDatabaseType.Sqlite);
xQuery.ConnectionString = "Data Source=:memory:";
xQuery.BeginTransaction();
// Make some changes
xQuery.CommitTransaction();
```

* Work with Binary data
```
var xQuery = new XQuery(XDatabaseType.Sqlite);
xQuery.ConnectionString = "Data Source=:memory:";
xQuery.PutFile("image.png", "insert into table (image) values (@image);", "@image");
xQuery.GetBinaryAsImage("select image from table limit 1;");
```
