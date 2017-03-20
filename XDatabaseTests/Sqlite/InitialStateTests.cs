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
using XDatabase.Core;

namespace XDatabaseTests.Sqlite
{
    public class InitialStateTests
    {
        [Test]
        public void TestInitialStateOfErrorMessageIsNull()
        {
            var xQuery = new XQuerySqlite();
            Assert.IsNull(xQuery.ErrorMessage);
        }

        [Test]
        public void TestInitialStateOfTimeoutIs30S()
        {
            var xQuery = new XQuerySqlite();
            Assert.AreEqual(30000, xQuery.Timeout);
        }

        [Test]
        public void TestInitialStateOfConnectionStringIsNull()
        {
            var xQuery = new XQuerySqlite();
            Assert.IsNull(xQuery.ConnectionString);
        }

        [Test]
        public void TestInitialStateOfIsActiveConnectionIsFalse()
        {
            var xQuery = new XQuerySqlite();
            Assert.IsFalse(xQuery.IsConnectionOpened);
        }

        [Test]
        public void TestInitialStateOfIsTransactionModeIsFalse()
        {
            var xQuery = new XQuerySqlite();
            Assert.IsFalse(xQuery.IsInTransactionMode);
        }

        [Test]
        public void TestInitialStateOfDbTypeEqualsToSpecifiedOne()
        {
            var xQuery = new XQuerySqlite();
            Assert.AreEqual(XDatabaseType.SqLite, xQuery.CurrentXDatabaseType);
        }
    }
}
