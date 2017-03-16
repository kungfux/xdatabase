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

using NUnit.Framework;
using XDatabase;

namespace XDatabaseTests
{
    public class XQuerySimpleSelectTests
    {
        private const string SqliteConnectionString = "Data Source=:memory:";
        private const XDatabaseType DbType = XDatabaseType.SqLite;

        [Test]
        public void TestArithmeticOperationWithSelectCell()
        {
            var xQuery = new XQuery(DbType) {ConnectionString = SqliteConnectionString};
            Assert.AreEqual(30, xQuery.SelectCell<long>("select 10+20;"));
        }

        [Test]
        public void TestArgumentsCanBeSpecifiedWithinSelectTable()
        {
            var xQuery = new XQuery(DbType) { ConnectionString = SqliteConnectionString };
            const string value = "asd";
            var result = xQuery.SelectTable("select @a;", xQuery.AddParameter("@a", value))
                .Rows[0].ItemArray[0];
            Assert.AreEqual(result, value);
        }

        [Test]
        public void TestSelectCellCanReturnStringDataType()
        {
            var xQuery = new XQuery(DbType) { ConnectionString = SqliteConnectionString };
            var result = xQuery.SelectCell<string>("select 'asd';");
            Assert.AreEqual("asd", result);
        }

        [Test]
        public void TestSelectCellCanReturnLongDataType()
        {
            var xQuery = new XQuery(DbType) { ConnectionString = SqliteConnectionString };
            var result = xQuery.SelectCell<long>("select 100;");
            Assert.AreEqual(100L, result);
        }

        [Test]
        public void TestSelectCellCanReturnDoubleDataType()
        {
            var xQuery = new XQuery(DbType) { ConnectionString = SqliteConnectionString };
            var result = xQuery.SelectCell<double>("select 1.1;");
            Assert.AreEqual(1.1d, result);
        }
    }
}
