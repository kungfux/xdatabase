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

using System.Data.Common;
using System.Data.OleDb;
using System.Data.SQLite;
using MySql.Data.MySqlClient;

namespace XDatabase.Core
{
    public abstract partial class XQuery
    {
        private const string TypeMySql = "XDatabase.XQueryMySql";
        private const string TypeOleDb = "XDatabase.XQueryOleDb";
        private const string TypeSqlite = "XDatabase.XQuerySqlite";

        private DbParameter GetParameter()
        {
            var type = GetType().FullName;
            switch (type)
            {
                case TypeMySql:
                    return new MySqlParameter();
                case TypeOleDb:
                    return new OleDbParameter();
                case TypeSqlite:
                    return new SQLiteParameter();
                default:
                    return null;
            }
        }

        private DbParameter GetParameter(string name, object value)
        {
            var type = GetType().FullName;
            switch (type)
            {
                case TypeMySql:
                    return new MySqlParameter(name, value);
                case TypeOleDb:
                    return new OleDbParameter(name, value);
                case TypeSqlite:
                    return new SQLiteParameter(name, value);
                default:
                    return null;
            }
        }

        private DbConnection GetConnection()
        {
            var type = GetType().FullName;
            switch (type)
            {
                case TypeMySql:
                    return new MySqlConnection();
                case TypeOleDb:
                    return new OleDbConnection();
                case TypeSqlite:
                    return new SQLiteConnection();
                default:
                    return null;
            }
        }

        private DbDataAdapter GetDataAdapter()
        {
            var type = GetType().FullName;
            switch (type)
            {
                case TypeMySql:
                    return new MySqlDataAdapter();
                case TypeOleDb:
                    return new OleDbDataAdapter();
                case TypeSqlite:
                    return new SQLiteDataAdapter();
                default:
                    return null;
            }
        }

        private DbCommand GetCommand()
        {
            var type = GetType().FullName;
            switch (type)
            {
                case TypeMySql:
                    return new MySqlCommand();
                case TypeOleDb:
                    return new OleDbCommand();
                case TypeSqlite:
                    return new SQLiteCommand();
                default:
                    return null;
            }
        }
    }
}
