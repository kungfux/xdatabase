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
    class XQuerySQLiteTests
    {
        private const string sqliteConnectionString =
            "Data Source=TestDataStorage\\TestDatabase.sqlite;Version=3;UTF8Encoding=True;foreign keys=true;";
        private const XQuery.XDatabaseType dbType = XQuery.XDatabaseType.SQLite;

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

        [Test]
        [Category("Positive")]
        public void TestInitialState()
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
        [Category("Positive")]
        public void TestDefiningOfTheConnectionString()
        {
            XQuery x = new XQuery(dbType);
            // here is Xclass should perform test connection
            // and save connection string
            x.ConnectionString = sqliteConnectionString;
            Assert.IsNull(x.ErrorMessage);
            Assert.AreEqual(sqliteConnectionString, x.ConnectionString);
        }

        [Test]
        [Category("Positive")]
        public void TestNoActiveConnectionOnDefiningConnectionString()
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
        [Category("Positive")]
        public void TestArithmeticOperationWithSelectCell()
        {
            XQuery x = new XQuery(dbType);
            x.ConnectionString = sqliteConnectionString;
            Assert.IsNull(x.ErrorMessage);
            Assert.AreEqual(30, x.SelectCell<Int64>("select 10+20;"));
        }

        [TestCase((string)"text1", "SELECT f1 FROM TestTable;")]
        [TestCase((Int64)1, "SELECT f2 FROM TestTable;")]
        [TestCase((double)1.1, "SELECT f3 FROM TestTable;")]
        public void TestThatSelectCellCanReturnDifferentTypesOfData<T>(T pExpectedValue, string sqlSelectStatement)
        {
            XQuery x = new XQuery(dbType);
            x.ConnectionString = sqliteConnectionString;
            Assert.IsNull(x.ErrorMessage);
            Assert.AreEqual(pExpectedValue, x.SelectCell<T>(sqlSelectStatement));
        }

        [Test]
        [Category("Positive")]
        public void TestArgumentsCanBePassedToSelectTable()
        {
            XQuery x = new XQuery(dbType);
            x.ConnectionString = sqliteConnectionString;
            Assert.IsNull(x.ErrorMessage);
            x.ChangeData("insert into test (f1) values ('asd')");
            Assert.IsNull(x.ErrorMessage);
            System.Data.DataTable t = 
                x.SelectTable("select * from test where f1 = @data",
                new System.Data.SQLite.SQLiteParameter("@data", "asd"));
            Assert.IsNull(x.ErrorMessage);
            Assert.AreEqual(1, t.Rows.Count);
        }

        [Test]
        [Category("Positive")]
        public void TestTransactionWorks()
        {
            const string sqlInsert = "INSERT INTO [test] (f1) VALUES ('asd')";

            XQuery x = new XQuery(dbType);
            x.ConnectionString = sqliteConnectionString;
            Assert.IsNull(x.ErrorMessage);

            // perform transaction
            Assert.IsTrue(x.StartTransaction());
            Assert.IsNull(x.ErrorMessage);
            for (int a = 0; a < 5; a++)
            {
                Assert.AreEqual(1, x.PerformTransactionCommand(sqlInsert));
            }
            Assert.IsTrue(x.EndTransaction());
        }

        [Test]
        [Category("Positive")]
        public void TestTransactionInsertsData()
        {
            const string sqlInsert = "INSERT INTO [test] (f1) VALUES ('asd')";

            XQuery x = new XQuery(dbType);
            x.ConnectionString = sqliteConnectionString;
            Assert.IsNull(x.ErrorMessage);

            // check current rows count
            Int64 rowsBeforeTransaction = 0;
            rowsBeforeTransaction = x.SelectCell<Int64>("select count(*) from test;");

            // perform transaction
            Assert.IsTrue(x.StartTransaction());
            Assert.IsNull(x.ErrorMessage);
            for (int a = 0; a < 100; a++)
            {
                Assert.AreEqual(1, x.PerformTransactionCommand(sqlInsert));
            }
            Assert.IsTrue(x.EndTransaction());

            // check that count equals to before + 100
            Assert.AreEqual(rowsBeforeTransaction + 100, x.SelectCell<Int64>("select count(*) from test;"));
        }

        [Test]
        [Category("Negative")]
        public void TestTransactionFailsIfNoConnectionSpecified()
        {
            XQuery x = new XQuery(dbType);
            Assert.IsFalse(x.StartTransaction());
            Assert.IsNotNull(x.ErrorMessage);
        }

        [Test]
        [Category("Negative")]
        public void TestEndTransactionFailsIfWasNotStarted()
        {
            XQuery x = new XQuery(dbType);
            x.ConnectionString = sqliteConnectionString;
            Assert.IsNull(x.ErrorMessage);
            Assert.IsFalse(x.EndTransaction());
            Assert.IsNotNull(x.ErrorMessage);
        }

        [Test]
        [Category("Negative")]
        public void TestTransactionFailsIfWasNotStarted()
        {
            const string sqlInsert = "INSERT INTO [test] (f1) VALUES ('asd')";

            XQuery x = new XQuery(dbType);
            x.ConnectionString = sqliteConnectionString;
            Assert.IsNull(x.ErrorMessage);
            // perform transaction
            for (int a = 0; a < 5; a++)
            {
                Assert.AreEqual(-1, x.PerformTransactionCommand(sqlInsert));
                Assert.IsNotNull(x.ErrorMessage);
            }
            Assert.IsFalse(x.EndTransaction());
            Assert.IsNotNull(x.ErrorMessage);
        }

        [Test]
        [Category("Positive")]
        public void TestPutFile()
        {
            XQuery x = new XQuery(dbType);
            x.ConnectionString = sqliteConnectionString;
            Assert.IsNull(x.ErrorMessage);

            Assert.IsTrue(x.PutFile(Environment.CurrentDirectory + "\\TestDataStorage\\test_image_picture-128.png", 
                "insert into test (f4) values (@file);"));
        }

        [Test]
        [Category("Positive")]
        public void TestGetImage()
        {
            XQuery x = new XQuery(dbType);
            x.ConnectionString = sqliteConnectionString;
            Assert.IsNull(x.ErrorMessage);

            Assert.IsTrue(x.PutFile(Environment.CurrentDirectory + "\\TestDataStorage\\test_image_picture-128.png",
                "insert into test (f4) values (@file);"));

            Image i = x.GetBinaryAsImage("select f4 from test where f4 is not null");
            Assert.NotNull(i);
            Assert.GreaterOrEqual(i.Height, 1);
        }
    }
}
