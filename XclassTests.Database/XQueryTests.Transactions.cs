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
        const string sqlInsert = "INSERT INTO [test] (f1) VALUES ('asd')";

        [Test]
        public void TestTransactionWorks()
        {
            XQuery x = new XQuery(dbType);
            x.ConnectionString = sqliteConnectionString;
            Assert.IsNull(x.ErrorMessage);

            // perform transaction
            Assert.IsTrue(x.BeginTransaction());
            Assert.IsNull(x.ErrorMessage);
            for (int a = 0; a < 5; a++)
            {
                Assert.AreEqual(1, x.ChangeData(sqlInsert));
            }
            Assert.IsTrue(x.CommitTransaction());
        }

        [Test]
        public void TestTransactionInsertsData()
        {
            XQuery x = new XQuery(dbType);
            x.ConnectionString = sqliteConnectionString;
            Assert.IsNull(x.ErrorMessage);

            // check current rows count
            Int64 rowsBeforeTransaction = 0;
            rowsBeforeTransaction = x.SelectCell<Int64>("select count(*) from test;");

            // perform transaction
            Assert.IsTrue(x.BeginTransaction());
            Assert.IsNull(x.ErrorMessage);
            for (int a = 0; a < 100; a++)
            {
                Assert.AreEqual(1, x.ChangeData(sqlInsert));
            }
            Assert.IsTrue(x.CommitTransaction());

            // check that count equals to before + 100
            Assert.AreEqual(rowsBeforeTransaction + 100, x.SelectCell<Int64>("select count(*) from test;"));
        }

        [Test]
        public void TestTransactionFailsIfNoConnectionSpecified()
        {
            XQuery x = new XQuery(dbType);
            Assert.IsFalse(x.BeginTransaction());
            Assert.IsNotNull(x.ErrorMessage);
        }

        [Test]
        public void TestEndTransactionFailsIfWasNotStarted()
        {
            XQuery x = new XQuery(dbType);
            x.ConnectionString = sqliteConnectionString;
            Assert.IsNull(x.ErrorMessage);
            Assert.IsFalse(x.CommitTransaction());
            Assert.IsNotNull(x.ErrorMessage);
        }

        [Test]
        public void TestInsertBinaryWhenTransactionIsStarted()
        {
            XQuery x = new XQuery(dbType);
            x.ConnectionString = sqliteConnectionString;
            Assert.IsTrue(x.BeginTransaction());
            Assert.IsNull(x.ErrorMessage);
            //
            Assert.IsTrue(x.PutFile(Environment.CurrentDirectory + "\\TestDataStorage\\test_image_picture-128.png",
                "insert into test (f4) values (@file);", "@file"));
            Assert.IsNull(x.ErrorMessage);
            //
            Assert.IsTrue(x.CommitTransaction());
            Assert.IsNull(x.ErrorMessage);
        }

        [Test]
        public void TestConnectionIsClosedAfterCommit()
        {
            XQuery x = new XQuery(dbType);
            x.ConnectionString = sqliteConnectionString;
            Assert.IsTrue(x.BeginTransaction());
            Assert.IsNull(x.ErrorMessage);
            Assert.IsTrue(x.IsActiveConnection);
            Assert.IsTrue(x.IsTransactionMode);
            Assert.AreEqual(1, x.ChangeData(sqlInsert));
            Assert.IsNull(x.ErrorMessage);
            Assert.IsTrue(x.CommitTransaction());
            Assert.IsNull(x.ErrorMessage);
            Assert.IsFalse(x.IsActiveConnection);
            Assert.IsFalse(x.IsTransactionMode);
        }

        [Test]
        public void TestConnectionIsClosedAfterRollback()
        {
            XQuery x = new XQuery(dbType);
            x.ConnectionString = sqliteConnectionString;
            Assert.IsTrue(x.BeginTransaction());
            Assert.IsNull(x.ErrorMessage);
            Assert.IsTrue(x.IsActiveConnection);
            Assert.IsTrue(x.IsTransactionMode);
            Assert.AreEqual(1, x.ChangeData(sqlInsert));
            Assert.IsNull(x.ErrorMessage);
            Assert.IsTrue(x.CommitTransaction());
            Assert.IsNull(x.ErrorMessage);
            Assert.IsFalse(x.IsActiveConnection);
            Assert.IsFalse(x.IsTransactionMode);
        }
    }
}
