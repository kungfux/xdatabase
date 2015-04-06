using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xclass.Database;

namespace XclassTests.Database
{
    [TestFixture]
    class XQueryTests
    {
        private const string sqliteConnectionString =
            "Data Source=TestDataStorage\\TestDatabase.sqlite;Version=3;UTF8Encoding=True;foreign keys=true;";
        private const string accessConnectionString =
            "Provider=Microsoft.Jet.OleDb.4.0;Data Source=TestDataStorage\\TestDatabase.mdb;";

        #region SQLite

        [Test]
        public void SQLite_Mode_DefineTheConnectionString()
        {
            XQuery x = new XQuery(XQuery.XDatabaseType.SQLite);
            // here is Xclass should perform test connection
            // and save connection string
            x.ConnectionString = sqliteConnectionString;
            Assert.IsNull(x.ErrorMessage);
            Assert.AreEqual(sqliteConnectionString, x.ConnectionString);
        }

        public void TestConnection_SQLite_Manually()
        {
            XQuery x = new XQuery(XQuery.XDatabaseType.SQLite);
            // by default no connection is opened
            Assert.IsFalse(x.IsActiveConnection);
            // here is Xclass perform test connection
            x.ConnectionString = sqliteConnectionString;
            // connection should be closed after the test
            Assert.IsFalse(x.IsActiveConnection);
        }

        [Test]
        public void SQLite_Mode_SelectData()
        {
            XQuery x = new XQuery(XQuery.XDatabaseType.SQLite);
            x.ConnectionString = sqliteConnectionString;
            Assert.AreEqual(sqliteConnectionString, x.ConnectionString);
            x.SelectTable("select 10+20;");
            Assert.IsNull(x.ErrorMessage);
        }

        #endregion

        #region MS_Access
        [Test]
        public void TestConnection_MS_Access()
        {
            XQuery x = new XQuery(XQuery.XDatabaseType.MS_Access);
            x.ConnectionString = accessConnectionString;
            Assert.IsNull(x.ErrorMessage);
            Assert.AreEqual(accessConnectionString, x.ConnectionString);
        }
        #endregion
    }
}
