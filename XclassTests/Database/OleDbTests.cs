/*
 * Copyright 2010-2014 Fuks Alexander. Contacts: kungfux2010@gmail.com
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

using System;
using NUnit.Framework;
using Xclass.Database;
using System.Data.OleDb;

namespace XclassTests.Database
{
    public class OleDbTests
    {
        // connection string to connect to db.mdb local test database
        private const string connectionString = "Provider=Microsoft.Jet.OleDb.4.0;Data Source=db.mdb;";

        // sql query statement to drop test table
        private const string sqlDropTestTable = "DROP TABLE test;";

        // sql query statement to create test table
        private readonly string sqlTestTableStatement = string.Concat(
            "CREATE TABLE test (",
            "f1 CHAR(1),",
            "f2 TEXT(255),",
            "f3 MEMO,",
            "f4 BYTE,",
            "f5 SHORT,",
            "f6 LONG,",
            "f7 SINGLE,",
            "f8 DOUBLE,",
            "f9 GUID,",
            "f10 DECIMAL(2,2),",
            "f11 DATETIME,",
            "f12 CURRENCY,",
            "f13 COUNTER,",
            "f14 YESNO,",
            "f15 LONGBINARY,",
            "f16 BINARY(255)",
            ");"
            );

        // sql query statement to insert data to test table
        private readonly string sqlInsertTestData = string.Concat(
            "INSERT INTO test (f1, f2, f3, f4, f5, f6, f7, f8, f9, f10, f11, f12, f14) ",
            "VALUES (@p1, @p2, @p3, @p4, @p5, @p6, @p7, @p8, @p9, @p10, @p11, @p12, @p14);");

        private readonly OleDbParameter[] testDataParameters = new OleDbParameter[] {
            new OleDbParameter("@p1", 'a'),
            new OleDbParameter("@p2", "text"),
            new OleDbParameter("@p3", "multiline" + Environment.NewLine + "text"),
            new OleDbParameter("@p4", byte.MaxValue),
            new OleDbParameter("@p5", short.MaxValue),
            new OleDbParameter("@p6", int.MaxValue),
            new OleDbParameter("@p7", Single.MaxValue),
            new OleDbParameter("@p8", double.MaxValue),
            new OleDbParameter("@p9", Guid.NewGuid()),
            new OleDbParameter("@p10", DBNull.Value), // TODO: Replace DBNull.Value with real decimal(2,2)
            new OleDbParameter("@p11", DateTime.Now.ToString()),
            new OleDbParameter("@p12", string.Format("{0:c}", 100)),
            // @p13 is skipped because it is COUNTER
            new OleDbParameter("@p14", true)
            // TODO: Add longbinary testing
            // TODO: Add binary testing
        };

        [Test]
        public void TestInitialState_ConnectionString_IsNull()
        {
            OleDb odd = new OleDb();
            Assert.IsNullOrEmpty(odd.ConnectionString);
        }

        [Test]
        public void TestInitialState_KeepDatabaseOpened_IsFalse()
        {
            OleDb odd = new OleDb();
            Assert.IsFalse(odd.KeepDatabaseOpened);
        }

        [Test]
        public void TestInitialState_IsConnectionIsActive_IsFalse()
        {
            OleDb odd = new OleDb();
            Assert.IsFalse(odd.IsConnectionIsActive);
        }

        [Test]
        public void TestInitialState_LastOperationErrorMessage_IsNull()
        {
            OleDb odd = new OleDb();
            Assert.IsNullOrEmpty(odd.LastOperationErrorMessage);
        }

        [Test]
        public void TestConnection_ConnectionStringIsNotDefined()
        {
            OleDb odd = new OleDb();
            Assert.IsFalse(odd.TestConnection(""));
            Assert.IsNotNullOrEmpty(odd.LastOperationErrorMessage);
            Assert.IsFalse(odd.IsConnectionIsActive);
        }

        [Test]
        public void TestConnection_ConnectionStringIsNotDefined2()
        {
            OleDb odd = new OleDb();
            Assert.IsFalse(odd.TestConnection("", true));
            Assert.IsNotNullOrEmpty(odd.LastOperationErrorMessage);
            Assert.IsFalse(odd.IsConnectionIsActive);
        }

        [Test]
        public void TestConnection_AllOK1()
        {
            OleDb odd = new OleDb();
            Assert.IsTrue(odd.TestConnection(connectionString));
        }

        [Test]
        public void TestConnection_AllOK2()
        {
            OleDb odd = new OleDb();
            Assert.IsTrue(odd.TestConnection(connectionString, true, true));
            Assert.That(connectionString, Is.EqualTo(odd.ConnectionString));
            Assert.IsFalse(odd.IsConnectionIsActive);
            odd.Disconnect();
            Assert.IsFalse(odd.IsConnectionIsActive);
        }

        [Test]
        public void ChangeData_CreateTable_WithoutInitialization()
        {
            OleDb odd = new OleDb();
            Assert.AreEqual(-1, odd.ChangeData(sqlTestTableStatement));
            Assert.IsNotNullOrEmpty(odd.LastOperationErrorMessage);
        }

        [Test]
        public void ChangeData_CreateAndDropTable()
        {
            OleDb odd = new OleDb();
            Assert.IsTrue(odd.TestConnection(connectionString, true, false));
            odd.ChangeData(sqlDropTestTable);
            Assert.GreaterOrEqual(0, odd.ChangeData(sqlTestTableStatement));
            Assert.IsNullOrEmpty(odd.LastOperationErrorMessage);
        }

        [Test]
        public void ChangeData_InsertAndCheckData1()
        {
            OleDb odd = new OleDb();
            Assert.IsTrue(odd.TestConnection(connectionString, true, false));
            odd.ChangeData(sqlDropTestTable);
            Assert.GreaterOrEqual(0, odd.ChangeData(sqlTestTableStatement));
            Assert.IsNullOrEmpty(odd.LastOperationErrorMessage);
            Assert.AreEqual(1, odd.ChangeData(sqlInsertTestData, testDataParameters));
            Assert.IsNullOrEmpty(odd.LastOperationErrorMessage);
            Assert.IsNotNull(odd.SelectTable("SELECT * FROM test;"));
            Assert.IsNullOrEmpty(odd.LastOperationErrorMessage);
            Assert.IsFalse(odd.IsConnectionIsActive);
            Assert.IsTrue(odd.TestConnection(connectionString, true, true));
            Assert.AreEqual('a', Convert.ToChar(odd.SelectCell<string>("SELECT TOP 1 f1 FROM test;")));
            Assert.AreEqual("text", odd.SelectCell<string>("SELECT TOP 1 f2 FROM test;"));
            Assert.AreEqual("multiline" + Environment.NewLine + "text", odd.SelectCell<string>("SELECT TOP 1 f3 FROM test;"));
            Assert.AreEqual(byte.MaxValue, odd.SelectCell<byte>("SELECT TOP 1 f4 FROM test;"));
            Assert.AreEqual(short.MaxValue, odd.SelectCell<short>("SELECT TOP 1 f5 FROM test;"));
            Assert.AreEqual(int.MaxValue, odd.SelectCell<int>("SELECT TOP 1 f6 FROM test;"));
            Assert.AreEqual(Single.MaxValue, odd.SelectCell<Single>("SELECT TOP 1 f7 FROM test;"));
            Assert.AreEqual(double.MaxValue, odd.SelectCell<double>("SELECT TOP 1 f8 FROM test;"));
            Assert.IsNotNull(odd.SelectCell<System.Guid>("SELECT TOP 1 f9 FROM test;"));
            Assert.AreEqual(DBNull.Value, odd.SelectCell<DBNull>("SELECT TOP 1 f10 FROM test;")); // TODO: Replace DBNull.Value with real decimal(2,2)
            Assert.AreEqual(DateTime.Now.ToString("dd.MM.yyyy"), odd.SelectCell<DateTime>("SELECT TOP 1 f11 FROM test;").ToString("dd.MM.yyyy"));
            Assert.AreEqual(string.Format("{0:c}", 100), string.Format("{0:c}", odd.SelectCell<decimal>("SELECT TOP 1 f12 FROM test;")));
            Assert.AreEqual(1, odd.SelectCell<int>("SELECT TOP 1 f13 FROM test;"));
            Assert.AreEqual(true, odd.SelectCell<bool>("SELECT TOP 1 f14 FROM test;"));
            // TODO: Add longbinary testing
            // TODO: Add binary testing

            Assert.AreEqual(true, odd.SelectCell<bool>("SELECT TOP 1 f14 FROM test;", false));
            Assert.AreEqual(true, odd.SelectCell<bool>("SELECT TOP 1 f999 FROM test;", true));

            Assert.IsNotNull(odd.SelectColumn("SELECT f1 FROM test;"));
            Assert.IsNotNull(odd.SelectColumn("SELECT * FROM test;", 0));
            Assert.IsNotNull(odd.SelectRow("SELECT f1 FROM test;"));
            Assert.IsNotNull(odd.SelectRow("SELECT * FROM test;", 0));
        }
    }
}
