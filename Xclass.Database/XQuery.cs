/*
 * Copyright 2014 Fuks Alexander. Contacts: kungfux2010@gmail.com
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
    /// <summary>
    /// XQuery provides unified methods for interaction with following databases: MySQL, SQLite, Microsoft Access Database (mdb).
    /// XQuery allows to connect, perform select and update queries (single query or transaction), provides ability to work with
    ///  binary data, images etc.
    ///  
    /// WARNING! XQuery has dependencies according to database you're going to use: 
    ///  1. MySQL database: MySql.Data.dll (MySQL Connector/NET from http://www.mysql.com)
    ///  2. SQLite database: System.Data.SQLite.dll, SQLite.Interop.dll (Precompiled Binary for .NET from http://www.sqlite.org/)
    ///  3. Microsoft Access Database: TODO: Add dependency
    ///  
    /// Restrictions:
    ///  1. In case you're going to use XQuery to interact with Microsoft Access Database
    ///     you should compile XQuery class library for x86 only!
    /// </summary>
    public class XQuery
    {
        public enum XDatabaseType
        {
            SQLite,
            MySQL,
            MS_Access
        }

        public XQuery(XDatabaseType pDatabaseType)
        {
            databaseTypeChosen = pDatabaseType;
        }

        #region Private fields

        // Database connection (can be SQLiteConnection, MySqlConnection or OleDbConnection)
        private DbConnection connection = null;
        private XDatabaseType databaseTypeChosen;
        private string lastOperationErrorMessage;
        private string databaseConnectionString = null;
        private int operationTimeout = 30000;
        private bool keepDatabaseOpened = false;

        #endregion

        #region Public fields

        /// <summary>
        /// Error message text from last operation
        /// </summary>
        public string ErrorMessage 
        {
            get 
            {
                return lastOperationErrorMessage;
            }
        }

        /// <summary>
        /// Operation timeout
        /// Note: specified in milliseconds
        /// </summary>
        public int Timeout
        {
            get
            {
                return operationTimeout;
            }
            set
            {
                if (value > 0)
                {
                    operationTimeout = value;
                }
            }
        }

        /// <summary>
        /// Connection string to the database.
        /// Note: New value will be saved only in case test connection was established in time of setting.
        /// </summary>
        public string ConnectionString
        {
            get
            {
                return databaseConnectionString;
            }
            set
            {
                if (testConnection(value))
                {
                    databaseConnectionString = value;
                }
            }
        }

        /// <summary>
        /// Is connection to the database active now?
        /// </summary>
        public bool IsActiveConnection
        {
            get
            {
                if (connection != null)
                {
                    return ((connection.State == ConnectionState.Open) ||
                        (connection.State == ConnectionState.Executing) ||
                        (connection.State == ConnectionState.Fetching));
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Is connection to the database should be kept opened
        /// </summary>
        public bool KeepDatabaseOpened
        {
            get
            {
                return keepDatabaseOpened;
            }
            set
            {
                keepDatabaseOpened = value;
            }
        }

        /// <summary>
        /// Currently used database type: XDatabaseType
        /// </summary>
        public XDatabaseType ActiveDatabaseType
        {
            get
            {
                return databaseTypeChosen;
            }
        }

        #endregion


        #region Error reporting

        public delegate void ErrorEventHandler(string pErrorMessage);
        public event ErrorEventHandler OperationError;

        // Register error
        private void registerError(string pMessage)
        {
            lastOperationErrorMessage = pMessage;
            // Call event if somebody use it
            if (OperationError != null)
            {
                OperationError(lastOperationErrorMessage);
            }
        }

        // Clear error message before new operation begins
        private void clearError()
        {
            lastOperationErrorMessage = null;
        }

        #endregion

        #region Test connection

        // Check is connection can be established
        private bool testConnection(string pConnectionString)
        {
            clearError();
            // To perform test we will use new connect instance (not existing one)
            using (var newConnection = getConnectionInstance())
            {
                try
                {
                    newConnection.ConnectionString = pConnectionString;
                    newConnection.Open();
                    return true;
                }
                catch (Exception ex)
                {
                    registerError(ex.Message);
                    return false;
                }
            }
        }

        // Open connection to the database
        private bool openConnection()
        {
            clearError();
            connection = getConnectionInstance();
            try
            {
                connection.ConnectionString = ConnectionString;
                connection.Open();
                return true;
            }
            catch (Exception ex)
            {
                registerError(ex.Message);
                return false;
            }
        }

        // Close active connection
        private void closeConnection()
        {
            if (connection != null)
            {
                connection.Close();
            }
        }

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
                default:
                    throw new Exception("XQuery internal error. Invalid DatabaseType passed.");
            }
        }

        #endregion

        #region Select/update/delete data

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
                default:
                    throw new Exception("XQuery internal error. Invalid DatabaseType passed.");
            }
        }

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
                default:
                    throw new Exception("XQuery internal error. Invalid DatabaseType passed.");
            }
        }

        /// <summary>
        /// Perform SELECT query and return all results
        /// </summary>
        public DataTable SelectTable(string pSqlQuery, params IDataParameter[] pDataArgs)
        {
            clearError();
            var tableResults = new DataTable();
            try
            {
                if (!IsActiveConnection)
                {
                    openConnection();
                }
                using (var adapter = getDataAdapter())
                {
                    var command = getCommand();
                    command.Connection = connection;
                    command.CommandText = pSqlQuery;
                    command.CommandTimeout = Timeout;

                    adapter.SelectCommand = command;

                    adapter.SelectCommand.Parameters.Clear();
                    
                    if (pDataArgs != null)
                    {
                        foreach (var arg in pDataArgs)
                        {
                            adapter.SelectCommand.Parameters.Add(arg);
                        }
                    }
                    adapter.Fill(tableResults);
                    return tableResults;
                }
            }
            catch (Exception ex)
            {
                registerError(ex.Message);
                return null;
            }
            finally
            {
                if (!KeepDatabaseOpened)
                {
                    closeConnection();
                }
            }
        }

        #endregion
    }
}
