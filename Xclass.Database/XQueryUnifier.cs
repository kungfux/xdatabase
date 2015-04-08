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

using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;
using System.Data.SQLite;

namespace Xclass.Database
{
    public partial class XQuery
    {
        // Get new connection instance based on XDatabaseType
        private DbConnection getConnectionInstance()
        {
            switch (databaseTypeChosen)
            {
                case XDatabaseType.SQLite:
                    return new SQLiteConnection();
                case XDatabaseType.MySQL:
                    return new MySqlConnection();
                case XDatabaseType.MS_Access:
                    return new OleDbConnection();
            }
            // not all code paths return a value
            return new SQLiteConnection();
        }

        // Get new instance of appropriate DataAdapter
        private DbDataAdapter getDataAdapter()
        {
            switch (databaseTypeChosen)
            {
                case XDatabaseType.SQLite:
                    return new SQLiteDataAdapter();
                case XDatabaseType.MySQL:
                    return new MySqlDataAdapter();
                case XDatabaseType.MS_Access:
                    return new OleDbDataAdapter();
            }
            // not all code paths return a value
            return new SQLiteDataAdapter();
        }

        // Get new instance of appropriate Command
        private DbCommand getCommand()
        {
            switch (databaseTypeChosen)
            {
                case XDatabaseType.SQLite:
                    return new SQLiteCommand();
                case XDatabaseType.MySQL:
                    return new MySqlCommand();
                case XDatabaseType.MS_Access:
                    return new OleDbCommand();
            }
            // not all code paths return a value
            return new SQLiteCommand();
        }

        private IDataParameter getParameter()
        {
            switch (databaseTypeChosen)
            {
                case XDatabaseType.SQLite:
                    return new SQLiteParameter();
                case XDatabaseType.MySQL:
                    return new MySqlParameter();
                case XDatabaseType.MS_Access:
                    return new OleDbParameter();
            }
            // not all code paths return a value
            return new SQLiteParameter();
        }
    }
}
