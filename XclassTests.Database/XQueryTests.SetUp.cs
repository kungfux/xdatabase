/*
 * Copyright 2015 Fuks Alexander. Contacts: kungfux2010@gmail.com
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *   http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using NUnit.Framework;
using System;
using System.Drawing;
using Xclass.Database;

namespace XclassTests.Database
{
    [TestFixture, Timeout(30000)]
    public partial class XQueryTests
    {
        // connection to the test database
        public const string sqliteConnectionString =
            "Data Source=TestDataStorage\\TestDatabase.sqlite;Version=3;UTF8Encoding=True;foreign keys=true;";
        public const XQuery.XDatabaseType dbType = XQuery.XDatabaseType.SQLite;

        public XQuery getXQuery()
        {
            XQuery x = new XQuery(dbType);
            x.ConnectionString = sqliteConnectionString;
            return x;
        }

        [TestFixtureSetUp]
        public void Init()
        {
            string sqlCreateTestTable = string.Concat(
                "CREATE TABLE [test] (",
                "[f1] TEXT,",
                "[f2] INTEGER,",
                "[f3] REAL,",
                "[f4] BLOB",
                ");"
                );

            XQuery x = new XQuery(dbType);
            x.ConnectionString = sqliteConnectionString;
            x.ChangeData(sqlCreateTestTable);
        }

        [TestFixtureTearDown]
        public void Cleanup()
        {
            string sqlDropTestTable = "DROP TABLE test;";

            XQuery x = new XQuery(dbType);
            x.ConnectionString = sqliteConnectionString;
            x.ChangeData(sqlDropTestTable);
        }
    }
}
