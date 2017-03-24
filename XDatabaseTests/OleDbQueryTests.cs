/*
* Copyright © Alexander Fuks 2017 <Alexander.V.Fuks@gmail.com>
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
    [TestFixture]
    public class OleDbQueryTests
    {
        // connection string to connect to TestDataStorage\OleDbTestDatabase.mdb local test database
        private const string connectionString = @"Provider=Microsoft.Jet.OleDb.4.0;Data Source=.\TestDataStorage\TestDatabase.mdb;";

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

        [Test(Description="Check that ConnectionString field is null")]
        public void TestInitialState_ConnectionString_IsNull()
        {
            OleDbQuery odd = new OleDbQuery();
            Assert.IsNullOrEmpty(odd.ConnectionString);
        }

        [Test(Description = "Check that KeepOpened field if false")]
        public void TestInitialState_KeepDatabaseOpened_IsFalse()
        {
            OleDbQuery odd = new OleDbQuery();
            Assert.IsFalse(odd.KeepDatabaseOpened);
        }

        [Test(Description = "Check that IsConnectionIsAlive field is false")]
        public void TestInitialState_IsConnectionIsActive_IsFalse()
        {
            OleDbQuery odd = new OleDbQuery();
            Assert.IsFalse(odd.IsConnectionIsActive);
        }

        [Test(Description = "Check that LastOperationErrorMessage field is empty")]
        public void TestInitialState_LastOperationErrorMessage_IsNull()
        {
            OleDbQuery odd = new OleDbQuery();
            Assert.IsNullOrEmpty(odd.LastOperationErrorMessage);
        }

        [Test(Description = "Check that OperationTimeout is set to 30s by default")]
        public void TestInitialState_OperationTimeout_DefaultValue()
        {
            OleDbQuery odd = new OleDbQuery();
            Assert.AreEqual(30, odd.OperationTimeout);
        }

        [Test(Description = "Check that OperationTimeout can be changed")]
        public void TestInitialState_OperationTimeout_Change()
        {
            OleDbQuery odd = new OleDbQuery();
            odd.OperationTimeout = 2;
            Assert.AreEqual(2, odd.OperationTimeout);
        }

        [Test(Description = "Check that OperationTimeout can not be set to zero")]
        public void TestInitialState_OperationTimeout_Zero()
        {
            OleDbQuery odd = new OleDbQuery();
            odd.OperationTimeout = 0;
            Assert.AreEqual(30, odd.OperationTimeout);
        }

        [Test(Description = "Check that error message is generated in case connection was unsuccessful")]
        public void TestConnection_ConnectionStringIsNotDefined()
        {
            OleDbQuery odd = new OleDbQuery();
            Assert.IsFalse(odd.TestConnection(""));
            Assert.IsNotNullOrEmpty(odd.LastOperationErrorMessage);
            Assert.IsFalse(odd.IsConnectionIsActive);
        }

        [Test(Description = "Check that connection is not active in case no connection established")]
        public void TestConnection_ConnectionStringIsNotDefined2()
        {
            OleDbQuery odd = new OleDbQuery();
            Assert.IsFalse(odd.TestConnection("", true));
            Assert.IsNotNullOrEmpty(odd.LastOperationErrorMessage);
            Assert.IsFalse(odd.IsConnectionIsActive);
        }

        [Test(Description = "Check that connection can be established")]
        public void TestConnection_CanBeEstablished1()
        {
            OleDbQuery odd = new OleDbQuery();
            Assert.IsTrue(odd.TestConnection(connectionString));
        }

        [Test(Description = "Check that connection state determined correctly in case connection is established, "+
            "and disconnect method works properly")]
        public void TestConnection_CanBeEstablished2()
        {
            OleDbQuery odd = new OleDbQuery();
            Assert.IsTrue(odd.TestConnection(connectionString, true, true));
            Assert.That(connectionString, Is.EqualTo(odd.ConnectionString));
            Assert.IsFalse(odd.IsConnectionIsActive);
            odd.Disconnect();
            Assert.IsFalse(odd.IsConnectionIsActive);
        }

        [Test(Description = "Check that ChangeData() return -1 in case errors, " +
            "and error message is generated in case change operation")]
        public void ChangeData_CreateTable_WithoutInitialization()
        {
            OleDbQuery odd = new OleDbQuery();
            Assert.AreEqual(-1, odd.ChangeData(sqlTestTableStatement));
            Assert.IsNotNullOrEmpty(odd.LastOperationErrorMessage);
        }

        [Test(Description = "Check that new table can be created in the database without errors")]
        public void ChangeData_CreateTable()
        {
            OleDbQuery odd = new OleDbQuery();
            Assert.IsTrue(odd.TestConnection(connectionString, true, false));
            if (odd.SelectTable("SELECT * FROM test;") != null)
            {
                odd.ChangeData(sqlDropTestTable);
            }
            Assert.GreaterOrEqual(0, odd.ChangeData(sqlTestTableStatement));
            Assert.IsNullOrEmpty(odd.LastOperationErrorMessage);
        }

        [Test(Description = "Check that select can be executed without errors")]
        public void SelectTable_SelectWholeTable()
        {
            OleDbQuery odd = new OleDbQuery();
            Assert.IsTrue(odd.TestConnection(connectionString, true, false));
            Assert.IsNotNull(odd.SelectTable("SELECT * FROM TestTable;"));
            Assert.IsNullOrEmpty(odd.LastOperationErrorMessage);
        }

        [Test(Description = "Check that select can be executed without errors using arguments")]
        public void SelectTable_SelectTableUsingArguments()
        {
            OleDbQuery odd = new OleDbQuery();
            Assert.IsTrue(odd.TestConnection(connectionString, true, false));
            Assert.AreEqual("a", odd.SelectTable("SELECT f1 FROM TestTable WHERE f1=@param;", new OleDbParameter("@param", 'a')).Rows[0].ItemArray[0]);
            Assert.IsNullOrEmpty(odd.LastOperationErrorMessage);
        }

        [Test(Description = "Check that SelectCell<> can be executed without errors " + 
            "for several data types: Guid, DBNull, DateTime and currency")]
        public void SelectCell_DataTypes1()
        {
            OleDbQuery odd = new OleDbQuery();          
            Assert.IsTrue(odd.TestConnection(connectionString, true, true));
            Assert.AreEqual(Guid.Parse("{77E6E454-D883-4611-80B4-74AEE13F1C82}"), odd.SelectCell<Guid>("SELECT TOP 1 f9 FROM TestTable;"));
            Assert.AreEqual(DBNull.Value, odd.SelectCell<DBNull>("SELECT TOP 1 f10 FROM TestTable;")); // TODO: Replace DBNull.Value with real decimal(2,2)
            Assert.AreEqual(DateTime.Parse("21.11.2014").ToString("dd.MM.yyyy"), odd.SelectCell<DateTime>("SELECT TOP 1 f11 FROM TestTable;").ToString("dd.MM.yyyy"));
            Assert.AreEqual(string.Format("{0:c}", 100), string.Format("{0:c}", odd.SelectCell<decimal>("SELECT TOP 1 f12 FROM TestTable;")));
            // TODO: Add longbinary testing
            // TODO: Add binary testing
        }

        [TestCase((string)"a", "SELECT TOP 1 f1 FROM TestTable;")]
        [TestCase((string)"text", "SELECT TOP 1 f2 FROM TestTable;")]
        [TestCase((string)"multiline\r\ntext", "SELECT TOP 1 f3 FROM TestTable;")]
        [TestCase((byte)byte.MaxValue, "SELECT TOP 1 f4 FROM TestTable;")]
        [TestCase((short)short.MaxValue, "SELECT TOP 1 f5 FROM TestTable;")]
        [TestCase((int)int.MaxValue, "SELECT TOP 1 f6 FROM TestTable;")]
        [TestCase((Single)Single.MaxValue, "SELECT TOP 1 f7 FROM TestTable;")]
        [TestCase((double)double.MaxValue, "SELECT TOP 1 f8 FROM TestTable;")]
        [TestCase((int)1, "SELECT TOP 1 f13 FROM TestTable;")]
        [TestCase((bool)true, "SELECT TOP 1 f14 FROM TestTable;")]
        // TODO: Add longbinary testing
        // TODO: Add binary testing
        public void SelectCellTest<T>(T pExpectedValue, string sqlSelectStatement)
        {
            OleDbQuery odd = new OleDbQuery();
            Assert.IsTrue(odd.TestConnection(connectionString, true, false));
            Assert.AreEqual(pExpectedValue, odd.SelectCell<T>(sqlSelectStatement));
        }

        [Test(Description = "Check that SelectCell<> works correcly in case wrong sql statements")]
        [ExpectedException("System.FormatException")]
        public void SelectCell_WrongUsing1()
        {
            OleDbQuery odd = new OleDbQuery();
            Assert.IsTrue(odd.TestConnection(connectionString, true, true));
            odd.SelectCell<bool>("SELECT TOP 1 f1 FROM TestTable;");
        }

        [Test(Description = "Check that SelectCell<> works correcly in case wrong sql statements")]
        [ExpectedException("System.Data.DataException")]
        public void SelectCell_WrongUsing2()
        {
            OleDbQuery odd = new OleDbQuery();
            Assert.IsTrue(odd.TestConnection(connectionString, true, true));
            odd.SelectCell<bool>("SELECT TOP 1 f1,f2 FROM TestTable;");
        }

        [Test(Description = "Check that SelectCell<> works correcly in case wrong sql statements")]
        [ExpectedException("System.Data.DataException")]
        public void SelectCell_WrongUsing3()
        {
            OleDbQuery odd = new OleDbQuery();
            Assert.IsTrue(odd.TestConnection(connectionString, true, true));
            odd.SelectCell<bool>("SELECT TOP 1 f1,f2 FROM WrongTable;");
        }

        [Test(Description = "Check that SelectCell<> with default return values defined works correcly")]
        public void SelectCell_DefaultReturnValues()
        {
            OleDbQuery odd = new OleDbQuery();
            Assert.IsTrue(odd.TestConnection(connectionString, true, false));
            Assert.AreEqual(true, odd.SelectCell<bool>("SELECT TOP 1 f14 FROM TestTable;", false));
            Assert.AreEqual(true, odd.SelectCell<bool>("SELECT TOP 1 f999 FROM TestTable;", true));
        }

        [Test(Description = "Check that SelectColumn() works correcly")]
        public void SelectColumn()
        {
            OleDbQuery odd = new OleDbQuery();
            Assert.IsTrue(odd.TestConnection(connectionString, true, false));
            Assert.IsNotNull(odd.SelectColumn("SELECT f1 FROM TestTable;"));
            Assert.IsNotNull(odd.SelectColumn("SELECT * FROM TestTable;", 0));
        }

        [Test(Description = "Check that SelectColumn() works correcly in case wrong results")]
        public void SelectColumn_WrongUsing()
        {
            OleDbQuery odd = new OleDbQuery();
            Assert.IsTrue(odd.TestConnection(connectionString, true, false));
            Assert.IsNull(odd.SelectColumn("SELECT f1 FROM WrongTable;"));
            Assert.IsNull(odd.SelectColumn("SELECT f1,f2 FROM TestTable;"));
        }

        [Test(Description = "Check that SelectRow() works correcly")]
        public void SelectRow()
        {
            OleDbQuery odd = new OleDbQuery();
            Assert.IsTrue(odd.TestConnection(connectionString, true, false));
            Assert.IsNotNull(odd.SelectRow("SELECT f1 FROM TestTable;"));
            Assert.IsNotNull(odd.SelectRow("SELECT * FROM TestTable;", 0));
        }

        [Test(Description = "Check that SelectRow() works correcly in case wrong results")]
        public void SelectRow_WrongUsing()
        {
            OleDbQuery odd = new OleDbQuery();
            Assert.IsTrue(odd.TestConnection(connectionString, true, false));
            Assert.IsNull(odd.SelectRow("SELECT f1 FROM WrongTable;"));
            Assert.IsNull(odd.SelectRow("SELECT f1 FROM TestTable UNION ALL SELECT f2 FROM TestTable;"));
        }
    }
}
