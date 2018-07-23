# Badges

[![Build status](https://ci.appveyor.com/api/projects/status/9ixq897elu0uut74/branch/master?svg=true)](https://ci.appveyor.com/project/kungfux/xclass/branch/master)
[![NuGet version](https://badge.fury.io/nu/xdatabase.svg)](https://badge.fury.io/nu/xdatabase)

# XDatabase

XDatabase provides unified methods to interact with various databases. It allows to establish a connection to the database and perform select, create, insert, update and delete queries. In addition, it contains a set of methods to work with binary data and images stored in the database.

## Supported databases
* SQLite
* MySql
* MS Access (.mdb) _*through OleDb connection_
* MS SQL

## Dependency
_Note: The only dependencies must be met according to databases you are going to use._

* SQLite  
Files: System.Data.SQLite.dll, SQLite.Interop.dll.  
Precompiled binaries may be downloaded from http://www.sqlite.org/ or from http://nuget.org/ `Install-Package System.Data.SQLite.Core`  
* MySql  
Files: MySql.Data.dll  
MySQL Connector/NET may be downloaded from http://www.mysql.com/ or from http://nuget.org/ `Install-Package MySql.Data`
* MS Access (OLEDB)  
_Note: To interact with Microsoft Access Database, XDatabase must be built for x86 only! Please refer to the MSDN https://msdn.microsoft.com/en-us/library/ms810810.aspx_   
Files: System.Data.OleDb.dll - is a part of .NET Framework

## Examples
[View Examples](https://gist.github.com/kungfux/b72b014547ccd0383bfd7543601d6a6f)
