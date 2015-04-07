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
using Xclass.Database;

namespace XclassTests.Database
{
    [TestFixture, Timeout(30000)]
    class XQuerySQLiteTests
    {
        private const string sqliteConnectionString =
            "Data Source=TestDataStorage\\TestDatabase.sqlite;Version=3;UTF8Encoding=True;foreign keys=true;";
        private const XQuery.XDatabaseType dbType = XQuery.XDatabaseType.SQLite;

        [SetUp]
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

        [TearDown]
        public void Cleanup()
        {
            string sqlDropTestTable = "DROP TABLE test;";

            XQuery x = new XQuery(dbType);
            x.ConnectionString = sqliteConnectionString;
            x.ChangeData(sqlDropTestTable);
        }

        [Test]
        [Category("Initial state")]
        public void InitialState()
        {
            XQuery x = new XQuery(dbType);
            Assert.IsNull(x.ErrorMessage);
            Assert.AreEqual(30000, x.Timeout);
            Assert.IsNull(x.ConnectionString);
            Assert.IsFalse(x.IsActiveConnection);
            Assert.IsFalse(x.KeepDatabaseOpened);
            Assert.AreEqual(dbType, x.ActiveDatabaseType);
        }

        [Test]
        [Category("Connection")]
        public void DefineTheConnectionString()
        {
            XQuery x = new XQuery(dbType);
            // here is Xclass should perform test connection
            // and save connection string
            x.ConnectionString = sqliteConnectionString;
            Assert.IsNull(x.ErrorMessage);
            Assert.AreEqual(sqliteConnectionString, x.ConnectionString);
        }

        [Test]
        [Category("Connection")]
        public void DefineConnectionString_CheckNoActiveConnection()
        {
            XQuery x = new XQuery(dbType);
            // by default no active connection
            Assert.IsFalse(x.IsActiveConnection);
            // here is Xclass perform test connection
            x.ConnectionString = sqliteConnectionString;
            // connection should be closed after the test
            Assert.IsFalse(x.IsActiveConnection);
        }

        [Test]
        [Category("Select data")]
        public void SelectArithmeticOperation()
        {
            XQuery x = new XQuery(dbType);
            x.ConnectionString = sqliteConnectionString;
            Assert.IsNull(x.ErrorMessage);
            Assert.AreEqual(30, x.SelectCell<Int64>("select 10+20;"));
        }

        [Test]
        [Category("Transactions")]
        public void StartTransactionSuccess()
        {
            XQuery x = new XQuery(dbType);
            x.ConnectionString = sqliteConnectionString;
            Assert.IsNull(x.ErrorMessage);
            Assert.IsTrue(x.StartTransaction());
            Assert.IsNull(x.ErrorMessage);
        }

        [Test]
        [Category("Transactions")]
        public void StartTransactionFailsIfNoConnectionSpecified()
        {
            XQuery x = new XQuery(dbType);
            Assert.IsFalse(x.StartTransaction());
            Assert.IsNotNull(x.ErrorMessage);
        }
    }
}
