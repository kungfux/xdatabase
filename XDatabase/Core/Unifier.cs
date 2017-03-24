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
        private DbParameter GetParameter()
        {
            switch (TargetedDatabaseType)
            {
                case DatabaseType.MySql:
                    return new MySqlParameter();
                case DatabaseType.OleDb:
                    return new OleDbParameter();
                default:
                    return new SQLiteParameter();
            }
        }

        private DbParameter GetParameter(string name, object value)
        {
            switch (TargetedDatabaseType)
            {
                case DatabaseType.MySql:
                    return new MySqlParameter(name, value);
                case DatabaseType.OleDb:
                    return new OleDbParameter(name, value);
                default:
                    return new SQLiteParameter(name, value);
            }
        }

        private DbConnection GetConnection()
        {
            switch (TargetedDatabaseType)
            {
                case DatabaseType.MySql:
                    return new MySqlConnection();
                case DatabaseType.OleDb:
                    return new OleDbConnection();
                default:
                    return new SQLiteConnection();
            }
        }

        private DbDataAdapter GetDataAdapter()
        {
            switch (TargetedDatabaseType)
            {
                case DatabaseType.MySql:
                    return new MySqlDataAdapter();
                case DatabaseType.OleDb:
                    return new OleDbDataAdapter();
                default:
                    return new SQLiteDataAdapter();
            }
        }

        private DbCommand GetCommand()
        {
            switch (TargetedDatabaseType)
            {
                case DatabaseType.MySql:
                    return new MySqlCommand();
                case DatabaseType.OleDb:
                    return new OleDbCommand();
                default:
                    return new SQLiteCommand();
            }
        }
    }
}
