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
        public DbParameter AddParameter(string parameterName, object value)
        {
            var newParam = GetParameter();
            newParam.ParameterName = parameterName;
            newParam.Value = value;
            return newParam;
        }

        private DbParameter GetParameter()
        {
            switch (CurrentXDatabaseType)
            {
                case XDatabaseType.MySql:
                    return new MySqlParameter();
                case XDatabaseType.OleDb:
                    return new OleDbParameter();
                default:
                    return new SQLiteParameter();
            }
        }

        private DbConnection GetConnection()
        {
            switch (CurrentXDatabaseType)
            {
                case XDatabaseType.MySql:
                    return new MySqlConnection();
                case XDatabaseType.OleDb:
                    return new OleDbConnection();
                default:
                    return new SQLiteConnection();
            }
        }

        private DbDataAdapter GetDataAdapter()
        {
            switch (CurrentXDatabaseType)
            {
                case XDatabaseType.MySql:
                    return new MySqlDataAdapter();
                case XDatabaseType.OleDb:
                    return new OleDbDataAdapter();
                default:
                    return new SQLiteDataAdapter();
            }
        }

        private DbCommand GetCommand()
        {
            switch (CurrentXDatabaseType)
            {
                case XDatabaseType.MySql:
                    return new MySqlCommand();
                case XDatabaseType.OleDb:
                    return new OleDbCommand();
                default:
                    return new SQLiteCommand();
            }
        }
    }
}
