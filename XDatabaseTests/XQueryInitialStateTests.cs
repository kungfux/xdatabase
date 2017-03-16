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
    public class XQueryInitialStateTests
    {
        private const XDatabaseType DbType = XDatabaseType.SqLite;

        [Test]
        public void TestInitialStateOfErrorMessageIsNull()
        {
            var x = new XQuery(DbType);
            Assert.IsNull(x.ErrorMessage);
        }

        [Test]
        public void TestInitialStateOfTimeoutIs30S()
        {
            var x = new XQuery(DbType);
            Assert.AreEqual(30000, x.Timeout);
        }

        [Test]
        public void TestInitialStateOfConnectionStringIsNull()
        {
            var x = new XQuery(DbType);
            Assert.IsNull(x.ConnectionString);
        }

        [Test]
        public void TestInitialStateOfIsActiveConnectionIsFalse()
        {
            var x = new XQuery(DbType);
            Assert.IsFalse(x.IsConnectionOpened);
        }

        [Test]
        public void TestInitialStateOfIsTransactionModeIsFalse()
        {
            var x = new XQuery(DbType);
            Assert.IsFalse(x.IsInTransactionMode);
        }

        [Test]
        public void TestInitialStateOfDbTypeEqualsToSpecifiedOne()
        {
            var x = new XQuery(DbType);
            Assert.AreEqual(DbType, x.CurrentXDatabaseType);
        }
    }
}
