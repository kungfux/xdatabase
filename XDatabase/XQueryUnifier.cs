﻿/*
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

using MySql.Data.MySqlClient;
using System.Data.Common;
using System.Data.OleDb;
using System.Data.SQLite;

namespace XDatabase
{
    public partial class XQuery
    {
        private DbParameter GetParameter()
        {
            switch (CurrentXDatabaseType)
            {
                case XDatabaseType.SqLite:
                    return new SQLiteParameter();
                case XDatabaseType.MySql:
                    return new MySqlParameter();
                case XDatabaseType.MsAccess:
                    return new OleDbParameter();
            }

            return new SQLiteParameter();
        }

        private DbConnection GetConnectionInstance()
        {
            switch (CurrentXDatabaseType)
            {
                case XDatabaseType.SqLite:
                    return new SQLiteConnection();
                case XDatabaseType.MySql:
                    return new MySqlConnection();
                case XDatabaseType.MsAccess:
                    return new OleDbConnection();
            }

            return new SQLiteConnection();
        }

        private DbDataAdapter GetDataAdapter()
        {
            switch (CurrentXDatabaseType)
            {
                case XDatabaseType.SqLite:
                    return new SQLiteDataAdapter();
                case XDatabaseType.MySql:
                    return new MySqlDataAdapter();
                case XDatabaseType.MsAccess:
                    return new OleDbDataAdapter();
            }

            return new SQLiteDataAdapter();
        }

        private DbCommand GetCommand()
        {
            switch (CurrentXDatabaseType)
            {
                case XDatabaseType.SqLite:
                    return new SQLiteCommand();
                case XDatabaseType.MySql:
                    return new MySqlCommand();
                case XDatabaseType.MsAccess:
                    return new OleDbCommand();
            }

            return new SQLiteCommand();
        }
    }
}