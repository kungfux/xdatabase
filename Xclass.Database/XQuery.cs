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

using System;
using System.Data;
using System.Data.Common;

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
    public partial class XQuery
    {
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
        /// Is library in transaction mode
        /// </summary>
        public bool IsTransactionMode
        {
            get
            {
                return transaction != null;
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
            if (connection != null && !IsTransactionMode)
            {
                connection.Close();
            }
        }

        #endregion

        #region Select/update/delete data

        /// <summary>
        /// Perform SELECT query and return all results
        /// </summary>
        /// <param name="pSqlQuery">Sql query statement</param>
        /// <param name="pDataArgs">Query arguments</param>
        /// <returns>System.Data.DataTable or null</returns>
        public DataTable SelectTable(string pSqlQuery, params DbParameter[] pDataArgs)
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

        /// <summary>
        /// Perform SELECT query and return single row in case one row in results only
        /// </summary>
        /// <param name="pSqlQuery">Query statement</param>
        /// <param name="pDataArgs">Query arguments</param>
        /// <returns>System.Data.DataRow or null</returns>
        public DataRow SelectRow(string pSqlQuery, params DbParameter[] pDataArgs)
        {
            var table = SelectTable(pSqlQuery, pDataArgs);
            return table != null && table.Rows.Count == 1 ? table.Rows[0] : null;
        }

        /// <summary>
        /// Perform SELECT query and return single column in case one column in results only
        /// </summary>
        /// <param name="pSqlQuery">Query statement</param>
        /// <param name="pDataArgs">Query arguments</param>
        /// <returns>System.Data.DataRow or null</returns>
        public DataColumn SelectColumn(string pSqlQuery, params DbParameter[] pDataArgs)
        {
            var table = SelectTable(pSqlQuery, pDataArgs);
            return table != null && table.Columns.Count == 1 ? table.Columns[0] : null;
        }

        /// <summary>
        /// Perform SELECT query and return single cell in case one cell in results only
        /// </summary>
        /// <typeparam name="T">Expected data type</typeparam>
        /// <param name="pSqlQuery">Query statement</param>
        /// <param name="pDataArgs">Query arguments</param>
        /// <returns>System.Data.DataRow or null</returns>
        public T SelectCell<T>(string pSqlQuery, params DbParameter[] pDataArgs)
        {
            var table = SelectTable(pSqlQuery, pDataArgs);
            if (table != null && 
                table.Rows.Count == 1 && 
                table.Columns.Count == 1 && 
                table.Rows[0].ItemArray[0].GetType() == typeof(T))
            {
                return (T)table.Rows[0].ItemArray[0];
            }
            else
            {
                if (table != null && 
                    table.Rows.Count == 1 && 
                    table.Columns.Count == 1)
                {
                    throw new FormatException("Type of cell is not equals to specified type of T. Type of cell is equals to " +
                    table.Rows[0].ItemArray[0].GetType().ToString());
                }
                else if (table != null && 
                    (table.Rows.Count != 1 || 
                    table.Columns.Count != 1))
                {
                    throw new DataException(
                        string.Format("Expected 1x1 table but returned {0}x{1}", table.Rows.Count, table.Columns.Count));
                }
                else
                {
                    throw new DataException("Nothing to select, empty results.");
                }
            }
        }

        /// <summary>
        /// Perform SELECT query and return single cell in case one cell in results only
        /// </summary>
        /// <typeparam name="T">Expected data type</typeparam>
        /// <param name="pSqlQuery">Query statement</param>
        /// <param name="pDefaultValue">Default value that will be returned in case of error</param>
        /// <param name="pDataArgs">Query arguments</param>
        /// <returns>System.Data.DataRow or null</returns>
        public T SelectCell<T>(string pSqlQuery, T pDefaultValue = default(T), params DbParameter[] pDataArgs)
        {
            var table = SelectTable(pSqlQuery, pDataArgs);
            if (table != null && table.Rows.Count == 1 && table.Columns.Count == 1 && table.Rows[0].ItemArray[0].GetType() == typeof(T))
            {
                return (T)table.Rows[0].ItemArray[0];
            }
            else
            {
                return pDefaultValue;
            }
        }

        /// <summary>
        /// Perform INSERT, UPDATE, DELETE etc queries
        /// </summary>
        /// <param name="pSqlQuery">Query statement</param>
        /// <param name="pDataArgs">Query arguments</param>
        /// <returns>Number of affected rows or -1 in case error occur</returns>
        public int ChangeData(string pSqlQuery, params DbParameter[] pDataArgs)
        {
            clearError();
            try
            {
                if (!IsActiveConnection)
                {
                    openConnection();
                }
                using (var command = getCommand())
                {
                    command.Connection = connection;
                    command.CommandText = pSqlQuery;
                    command.CommandTimeout = Timeout;

                    if (pDataArgs != null)
                    {
                        foreach (var arg in pDataArgs)
                        {
                            command.Parameters.Add(arg);
                        }
                    }
                    return command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                registerError(ex.Message);
                return -1;
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
