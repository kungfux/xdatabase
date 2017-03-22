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

namespace XDatabaseTests.MySql
{
    public class MySqlTests
    {
        [Test]
        public void TestMySqlInstanceCanBeCreated()
        {
            const string paramValue = "value";
            var xQuery = new XQueryMySql();
            var param = xQuery.AddParameter("@test", paramValue);
            Assert.AreEqual(paramValue, param.Value);
        }

        [Test]
        public void TestMySqlConnectionStringIsNullIfNotValid()
        {
            const string connectionString = "Source?";
            var xQuery = new XQueryMySql(connectionString);
            Assert.IsNull(xQuery.ConnectionString);
        }
    }
}
