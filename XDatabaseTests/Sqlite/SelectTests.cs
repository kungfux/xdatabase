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
using System.Data;
using NUnit.Framework;
using XDatabase;

namespace XDatabaseTests.Sqlite
{
    public class SelectTests
    {
        [Test]
        public void TestSelectTable()
        {
            const string sqlSelectTable = "select 1, 2, 3 union select 4, 5, 6;";
            var xQuery = new XQuerySqlite(SetUp.SqliteConnectionString);
            var result = xQuery.SelectTable(sqlSelectTable);
            Assert.AreEqual(2*3, result.Rows.Count*result.Columns.Count);
        }

        [Test]
        public void TestSelectRow()
        {
            const string sqlSelectRow = "select 1, 'value', 3.3;";
            var xQuery = new XQuerySqlite(SetUp.SqliteConnectionString);
            var result = xQuery.SelectRow(sqlSelectRow);
            Assert.AreEqual(3, result.ItemArray.Length);
        }

        [Test]
        public void TestSelectColumn()
        {
            const string sqlSelectColumn = "select 1 union select 2 union select 3;";
            var xQuery = new XQuerySqlite(SetUp.SqliteConnectionString);
            var result = xQuery.SelectColumn(sqlSelectColumn);
            Assert.AreEqual(3, result.Table.Rows.Count);
        }

        [Test]
        public void TestArithmeticOperationWithSelectCell()
        {
            var xQuery = new XQuerySqlite(SetUp.SqliteConnectionString);
            Assert.AreEqual(30, xQuery.SelectCell<long>("select 10+20;"));
        }

        [Test]
        public void TestArgumentsCanBeSpecifiedWithinSelectTable()
        {
            var xQuery = new XQuerySqlite(SetUp.SqliteConnectionString);
            const string value = "asd";
            var result = xQuery.SelectTable("select @a;", xQuery.AddParameter("@a", value))
                .Rows[0].ItemArray[0];
            Assert.AreEqual(result, value);
        }

        [Test]
        public void TestSelectCellCanReturnStringDataType()
        {
            var xQuery = new XQuerySqlite(SetUp.SqliteConnectionString);
            var result = xQuery.SelectCell<string>("select 'asd';");
            Assert.AreEqual("asd", result);
        }

        [Test]
        public void TestSelectCellCanReturnLongDataType()
        {
            var xQuery = new XQuerySqlite(SetUp.SqliteConnectionString);
            var result = xQuery.SelectCell<long>("select 100;");
            Assert.AreEqual(100L, result);
        }

        [Test]
        public void TestSelectCellCanReturnDoubleDataType()
        {
            var xQuery = new XQuerySqlite(SetUp.SqliteConnectionString);
            var result = xQuery.SelectCell<double>("select 1.1;");
            Assert.AreEqual(1.1d, result);
        }

        [Test]
        public void TestSelectCellReturnsDefaultValueOnError()
        {
            const string defValue = "default";
            var xQuery = new XQuerySqlite(SetUp.SqliteConnectionString);
            var result = xQuery.SelectCell("select 1.1;", defValue);
            Assert.AreEqual(defValue, result);
        }

        [Test]
        public void TestSelectCellReturnsValueIfNoErrors()
        {
            const string defValue = "default";
            const string nonDefValue = "non default";
            var xQuery = new XQuerySqlite(SetUp.SqliteConnectionString);
            var result = xQuery.SelectCell($"select '{nonDefValue}';", defValue);
            Assert.AreEqual(nonDefValue, result);
        }

        [Test]
        public void TestSelectCellThrowsFormatException()
        {
            const string sqlSelect = "select 1;";
            var xQuery = new XQuerySqlite(SetUp.SqliteConnectionString);
            Assert.Throws(Is.TypeOf<FormatException>(), delegate
            {
                xQuery.SelectCell<string>(sqlSelect);
            });
        }

        [Test]
        public void TestSelectCellThrowsDataException()
        {
            const string sqlSelect = "select 1, 2, 3;";
            var xQuery = new XQuerySqlite(SetUp.SqliteConnectionString);
            Assert.Throws(Is.TypeOf<DataException>(), delegate
            {
                xQuery.SelectCell<long>(sqlSelect);
            });
        }

        [Test]
        public void TestSelectCellThrowsDataExceptionOnEmptyResult()
        {
            const string sqlSelect = "select;";
            var xQuery = new XQuerySqlite(SetUp.SqliteConnectionString);
            Assert.Throws(Is.TypeOf<DataException>(), delegate
            {
                xQuery.SelectCell<long>(sqlSelect);
            });
        }

        [Test]
        public void TestErrorIsLogged()
        {
            const string sqlSelect = "select 1, 2, 3;";
            var xQuery = new XQuerySqlite(SetUp.SqliteConnectionString);
            xQuery.OnError += XQuery_OnError;
            try
            {
                xQuery.SelectCell<long>(sqlSelect);
            }
            catch (Exception)
            {
                // ignored
            }

            Assert.IsNotEmpty(xQuery.ErrorMessage);
        }

        private void XQuery_OnError(string pErrorMessage)
        {
            // empty subscriber
        }
    }
}
