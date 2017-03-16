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
    public class XQueryTransactionTests
    {
        private const string SqliteConnectionString = "Data Source=:memory:";
        private const XDatabaseType DbType = XDatabaseType.SqLite;

        [Test]
        public void TestTransactionFailsIfNoConnectionSpecified()
        {
            var xQuery = new XQuery(DbType);
            Assert.IsFalse(xQuery.BeginTransaction());
            Assert.IsNotNull(xQuery.ErrorMessage);
        }

        [Test]
        public void TestCoomitTransactionFailsIfWasNotStarted()
        {
            var xQuery = new XQuery(DbType);
            Assert.IsFalse(xQuery.CommitTransaction());
        }

        [Test]
        public void TestConnectionIsActiveAfterBeginTransaction()
        {
            var xQuery = new XQuery(DbType) { ConnectionString = SqliteConnectionString };
            xQuery.BeginTransaction();
            Assert.IsTrue(xQuery.IsConnectionOpened);
        }

        [Test]
        public void TestConnectionIsClosedAfterCommit()
        {
            var xQuery = new XQuery(DbType) {ConnectionString = SqliteConnectionString};
            xQuery.BeginTransaction();
            xQuery.CommitTransaction();
            Assert.IsFalse(xQuery.IsConnectionOpened);
        }

        [Test]
        public void TestConnectionIsClosedAfterRollback()
        {
            var xQuery = new XQuery(DbType) { ConnectionString = SqliteConnectionString };
            xQuery.BeginTransaction();
            xQuery.RollbackTransaction();
            Assert.IsFalse(xQuery.IsConnectionOpened);
        }

        [Test]
        public void TestTransactionFlagIsBeingSet()
        {
            var xQuery = new XQuery(DbType) { ConnectionString = SqliteConnectionString };
            xQuery.BeginTransaction();
            Assert.True(xQuery.IsInTransactionMode);
        }
    }
}
