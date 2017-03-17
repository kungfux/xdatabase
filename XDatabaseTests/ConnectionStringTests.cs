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
using XDatabase;

namespace XDatabaseTests
{
    public class ConnectionStringTests
    {
        public const string SqliteConnectionString = "Data Source=:memory:";
        public const XDatabaseType DbType = XDatabaseType.SqLite;

        [Test]
        public void TestConnectionStringIsAcceptedInCaseSuccessfulConnection()
        {
            var xQuery = new XQuery(DbType) {ConnectionString = SqliteConnectionString};
            Assert.AreEqual(SqliteConnectionString, xQuery.ConnectionString);
        }

        [Test]
        public void TestConnectionStringIsRejectedInCaseNoSuccessfulConnection()
        {
            var xQuery = new XQuery(DbType)
            {
                ConnectionString = $"Data Source={Guid.NewGuid()};FailIfMissing=true;"
            };
            Assert.IsNull(xQuery.ConnectionString);
        }

        [Test]
        public void TestNoActiveConnectionAfterDefiningConnectionString()
        {
            var xQuery = new XQuery(DbType) {ConnectionString = SqliteConnectionString};
            Assert.IsFalse(xQuery.IsConnectionOpened);
        }
    }
}
