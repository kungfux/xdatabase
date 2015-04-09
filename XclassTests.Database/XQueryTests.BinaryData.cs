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
        public void TestPutFile()
        {
            XQuery x = new XQuery(dbType);
            x.ConnectionString = sqliteConnectionString;
            Assert.IsNull(x.ErrorMessage);

            Assert.IsTrue(x.PutFile(Environment.CurrentDirectory + "\\TestDataStorage\\test_image_picture-128.png",
                "insert into test (f4) values (@file);", "@file"));
        }

        [Test]
        public void TestGetImage()
        {
            XQuery x = new XQuery(dbType);
            x.ConnectionString = sqliteConnectionString;
            Assert.IsNull(x.ErrorMessage);

            Assert.IsTrue(x.PutFile(Environment.CurrentDirectory + "\\TestDataStorage\\test_image_picture-128.png",
                "insert into test (f4) values (@file);", "@file"));

            Image i = x.GetBinaryAsImage("select f4 from test where f4 is not null");
            Assert.NotNull(i);
            Assert.GreaterOrEqual(i.Height, 1);
        }
    }
}
