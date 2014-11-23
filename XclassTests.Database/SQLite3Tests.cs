﻿/*
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
using System.Data.SQLite;

namespace XclassTests.Database
{
    [TestFixture]
    public class SQLite3Tests
    {
        // connection string to connect to SQLiteTestDatabase.sqlite local test database
        private const string connectionString = @"Data Source=TestDataStorage\TestDatabase.sqlite;Version=3;FailIfMissing=True;UTF8Encoding=True;foreign keys=true;";

        // sql query statement to drop test table
        private const string sqlDropTestTable = "DROP TABLE test;";

        // sql query statement to create test table
        private readonly string sqlTestTableStatement = string.Concat(
            "CREATE TABLE [test] (",
            "[f1] TEXT,",
            "[f2] INTEGER,",
            "[f3] REAL,",
            "[f4] BLOB",
            ");"
            );

        // sql query statement to insert data to test table
        private readonly string sqlInsertTestData = string.Concat(
            "INSERT INTO test (f1, f2, f3, f4) VALUES (@p1, @p2, @p3, @p4);");

        private readonly SQLiteParameter[] testDataParameters = new SQLiteParameter[] {
            new SQLiteParameter("@p1", "text"),
            new SQLiteParameter("@p2", 123),
            new SQLiteParameter("@p3", 12.34),
            new SQLiteParameter("@p4", DBNull.Value),
        };

        [Test(Description = "Check that ConnectionString field is null")]
        public void TestInitialState_ConnectionString_IsNull()
        {
            SQLite3 sqlite = new SQLite3();
            Assert.IsNullOrEmpty(sqlite.ConnectionString);
        }

        [Test(Description = "Check that KeepOpened field if false")]
        public void TestInitialState_KeepDatabaseOpened_IsFalse()
        {
            SQLite3 sqlite = new SQLite3();
            Assert.IsFalse(sqlite.KeepDatabaseOpened);
        }

        [Test(Description = "Check that IsConnectionIsAlive field is false")]
        public void TestInitialState_IsConnectionIsActive_IsFalse()
        {
            SQLite3 sqlite = new SQLite3();
            Assert.IsFalse(sqlite.IsConnectionIsActive);
        }

        [Test(Description = "Check that LastOperationErrorMessage field is empty")]
        public void TestInitialState_LastOperationErrorMessage_IsNull()
        {
            SQLite3 sqlite = new SQLite3();
            Assert.IsNullOrEmpty(sqlite.LastOperationErrorMessage);
        }

        [Test(Description = "Check that error message is generated in case connection was unsuccessful")]
        public void TestConnection_ConnectionStringIsNotDefined()
        {
            SQLite3 sqlite = new SQLite3();
            Assert.IsFalse(sqlite.TestConnection(""));
            Assert.IsNotNullOrEmpty(sqlite.LastOperationErrorMessage);
            Assert.IsFalse(sqlite.IsConnectionIsActive);
        }

        [Test(Description = "Check that connection is not active in case no connection established")]
        public void TestConnection_ConnectionStringIsNotDefined2()
        {
            SQLite3 sqlite = new SQLite3();
            Assert.IsFalse(sqlite.TestConnection("", true));
            Assert.IsNotNullOrEmpty(sqlite.LastOperationErrorMessage);
            Assert.IsFalse(sqlite.IsConnectionIsActive);
        }

        [Test(Description = "Check that connection can be established")]
        public void TestConnection_CanBeEstablished1()
        {
            SQLite3 sqlite = new SQLite3();
            Assert.IsTrue(sqlite.TestConnection(connectionString));
        }

        [Test(Description = "Check that connection state determined correctly in case connection is established, " +
            "and disconnect method works properly")]
        public void TestConnection_CanBeEstablished2()
        {
            SQLite3 sqlite = new SQLite3();
            Assert.IsTrue(sqlite.TestConnection(connectionString, true, true));
            Assert.That(connectionString, Is.EqualTo(sqlite.ConnectionString));
            Assert.IsFalse(sqlite.IsConnectionIsActive);
            sqlite.Disconnect();
            Assert.IsFalse(sqlite.IsConnectionIsActive);
        }

        [Test(Description = "Check that ChangeData() return -1 in case errors, " +
            "and error message is generated in case change operation")]
        public void ChangeData_CreateTable_WithoutInitialization()
        {
            SQLite3 sqlite = new SQLite3();
            Assert.AreEqual(-1, sqlite.ChangeData(sqlTestTableStatement));
            Assert.IsNotNullOrEmpty(sqlite.LastOperationErrorMessage);
        }

        [Test(Description = "Check that new table can be created in the database without errors")]
        public void ChangeData_CreateTable()
        {
            SQLite3 sqlite = new SQLite3();
            Assert.IsTrue(sqlite.TestConnection(connectionString, true, false));
            if (sqlite.SelectTable("SELECT * FROM test;") != null)
            {
                sqlite.ChangeData(sqlDropTestTable);
            }
            Assert.GreaterOrEqual(0, sqlite.ChangeData(sqlTestTableStatement));
            Assert.IsNullOrEmpty(sqlite.LastOperationErrorMessage);
        }

        [Test(Description = "Check that select can be executed without errors")]
        public void SelectTable_SelectWholeTable()
        {
            SQLite3 sqlite = new SQLite3();
            Assert.IsTrue(sqlite.TestConnection(connectionString, true, false));
            Assert.IsNotNull(sqlite.SelectTable("SELECT * FROM TestTable;"));
            Assert.IsNullOrEmpty(sqlite.LastOperationErrorMessage);
        }

        [Test(Description = "Check that select can be executed without errors using arguments")]
        public void SelectTable_SelectTableUsingArguments()
        {
            SQLite3 sqlite = new SQLite3();
            Assert.IsTrue(sqlite.TestConnection(connectionString, true, false));
            Assert.AreEqual("text1", sqlite.SelectTable("SELECT f1 FROM TestTable WHERE f1=@param;", new SQLiteParameter("@param", "text1")).Rows[0].ItemArray[0]);
            Assert.IsNullOrEmpty(sqlite.LastOperationErrorMessage);
        }

        [Test(Description = "Check that SelectCell<> can be executed without errors")]
        public void SelectCell_DataTypes1()
        {
            SQLite3 sqlite = new SQLite3();
            Assert.IsTrue(sqlite.TestConnection(connectionString, true, true));
            Assert.AreEqual(DBNull.Value, sqlite.SelectCell<DBNull>("SELECT f4 FROM TestTable;"));
            Assert.AreEqual(DateTime.Now.ToString("dd.MM.yyyy"), Convert.ToDateTime(sqlite.SelectCell<string>("SELECT date('now');")).ToString("dd.MM.yyyy"));
        }

        [TestCase((string)"text1", "SELECT f1 FROM TestTable;")]
        [TestCase((Int64)1, "SELECT f2 FROM TestTable;")]
        [TestCase((double)1.1, "SELECT f3 FROM TestTable;")]
        public void SelectCellTest<T>(T pExpectedValue, string sqlSelectStatement)
        {
            SQLite3 sqlite = new SQLite3();
            Assert.IsTrue(sqlite.TestConnection(connectionString, true, false));
            Assert.AreEqual(pExpectedValue, sqlite.SelectCell<T>(sqlSelectStatement));
        }

        [Test(Description = "Check that SelectCell<> works correcly in case wrong sql statements")]
        [ExpectedException("System.FormatException")]
        public void SelectCell_WrongUsing1()
        {
            SQLite3 sqlite = new SQLite3();
            Assert.IsTrue(sqlite.TestConnection(connectionString, true, true));
            sqlite.SelectCell<bool>("SELECT  f1 FROM TestTable;");
        }

        [Test(Description = "Check that SelectCell<> works correcly in case wrong sql statements")]
        [ExpectedException("System.Data.DataException")]
        public void SelectCell_WrongUsing2()
        {
            SQLite3 sqlite = new SQLite3();
            Assert.IsTrue(sqlite.TestConnection(connectionString, true, true));
            sqlite.SelectCell<bool>("SELECT f1,f2 FROM TestTable;");
        }

        [Test(Description = "Check that SelectCell<> works correcly in case wrong sql statements")]
        [ExpectedException("System.Data.DataException")]
        public void SelectCell_WrongUsing3()
        {
            SQLite3 sqlite = new SQLite3();
            Assert.IsTrue(sqlite.TestConnection(connectionString, true, true));
            sqlite.SelectCell<bool>("SELECT f1,f2 FROM WrongTable;");
        }

        [Test(Description = "Check that SelectCell<> with default return values defined works correcly")]
        public void SelectCell_DefaultReturnValues()
        {
            SQLite3 sqlite = new SQLite3();
            Assert.IsTrue(sqlite.TestConnection(connectionString, true, false));
            Assert.AreEqual(true, Convert.ToBoolean(sqlite.SelectCell<Int64>("SELECT f2 FROM TestTable;", 0)));
            Assert.AreEqual(true, sqlite.SelectCell<bool>("SELECT f999 FROM TestTable;", true));
        }

        [Test(Description = "Check that SelectColumn() works correcly")]
        public void SelectColumn()
        {
            SQLite3 sqlite = new SQLite3();
            Assert.IsTrue(sqlite.TestConnection(connectionString, true, false));
            Assert.IsNotNull(sqlite.SelectColumn("SELECT f1 FROM TestTable;"));
            Assert.IsNotNull(sqlite.SelectColumn("SELECT * FROM TestTable;", 0));
        }

        [Test(Description = "Check that SelectColumn() works correcly in case wrong results")]
        public void SelectColumn_WrongUsing()
        {
            SQLite3 sqlite = new SQLite3();
            Assert.IsTrue(sqlite.TestConnection(connectionString, true, false));
            Assert.IsNull(sqlite.SelectColumn("SELECT f1 FROM WrongTable;"));
            Assert.IsNull(sqlite.SelectColumn("SELECT f1,f2 FROM TestTable;"));
        }

        [Test(Description = "Check that SelectRow() works correcly")]
        public void SelectRow()
        {
            SQLite3 sqlite = new SQLite3();
            Assert.IsTrue(sqlite.TestConnection(connectionString, true, false));
            Assert.IsNotNull(sqlite.SelectRow("SELECT f1 FROM TestTable;"));
            Assert.IsNotNull(sqlite.SelectRow("SELECT * FROM TestTable;", 0));
        }

        [Test(Description = "Check that SelectRow() works correcly in case wrong results")]
        public void SelectRow_WrongUsing()
        {
            SQLite3 sqlite = new SQLite3();
            Assert.IsTrue(sqlite.TestConnection(connectionString, true, false));
            Assert.IsNull(sqlite.SelectRow("SELECT f1 FROM WrongTable;"));
            Assert.IsNull(sqlite.SelectRow("SELECT f1 FROM TestTable UNION ALL SELECT f1 FROM TestTable;"));
        }
    }
}