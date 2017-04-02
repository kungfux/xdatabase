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

namespace XDatabaseTests.Sqlite
{
    public class TransactionTests
    {
        [Test]
        public void TestTransactionFailsIfNoConnectionSpecified()
        {
            var xQuery = new XQuerySqlite();
            Assert.IsFalse(xQuery.BeginTransaction());
            Assert.IsNotNull(xQuery.LastErrorMessage);
        }

        [Test]
        public void TestCoomitTransactionFailsIfWasNotStarted()
        {
            var xQuery = new XQuerySqlite();
            Assert.IsFalse(xQuery.CommitTransaction());
        }

        [Test]
        public void TestConnectionIsActiveAfterBeginTransaction()
        {
            var xQuery = new XQuerySqlite(SetUp.SqliteConnectionString);
            xQuery.BeginTransaction();
            Assert.IsTrue(xQuery.IsConnectionActive);
        }

        [Test]
        public void TestConnectionIsClosedAfterCommit()
        {
            var xQuery = new XQuerySqlite(SetUp.SqliteConnectionString);
            xQuery.BeginTransaction();
            xQuery.CommitTransaction();
            Assert.IsFalse(xQuery.IsConnectionActive);
        }

        [Test]
        public void TestConnectionIsClosedAfterRollback()
        {
            var xQuery = new XQuerySqlite(SetUp.SqliteConnectionString);
            xQuery.BeginTransaction();
            xQuery.RollbackTransaction();
            Assert.IsFalse(xQuery.IsConnectionActive);
        }

        [Test]
        public void TestTransactionFlagIsBeingSet()
        {
            var xQuery = new XQuerySqlite(SetUp.SqliteConnectionString);
            xQuery.BeginTransaction();
            Assert.True(xQuery.IsInTransactionMode);
        }

        [Test]
        public void TestConnectionIsNotBeingClosedIfKeepOpenWhenCommit()
        {
            var xQuery = new XQuerySqlite(SetUp.SqliteConnectionString)
            {
                KeepConnectionOpen = true
            };
            xQuery.BeginTransaction();
            xQuery.CommitTransaction();
            Assert.IsTrue(xQuery.IsConnectionActive);
        }

        [Test]
        public void TestConnectionIsNotBeingClosedIfKeepOpenWhenRollback()
        {
            var xQuery = new XQuerySqlite(SetUp.SqliteConnectionString)
            {
                KeepConnectionOpen = true
            };
            xQuery.BeginTransaction();
            xQuery.RollbackTransaction();
            Assert.IsTrue(xQuery.IsConnectionActive);
        }
    }
}
