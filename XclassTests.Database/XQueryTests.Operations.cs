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
    [TestFixture]
    public partial class XQueryTests
    {
        [Test]
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
        public void TestArgumentsCanBePassed()
        {
            XQuery x = new XQuery(dbType);
            x.ConnectionString = sqliteConnectionString;
            Assert.IsNull(x.ErrorMessage);
            System.Data.DataTable t =
                x.SelectTable("select * from testtable where f1 = @f1",
                x.AddParameter("@f1", "text1"));
            Assert.AreEqual(1, t.Rows.Count);
        }
    }
}
