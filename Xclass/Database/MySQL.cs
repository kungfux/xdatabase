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

using System;
using System.Data;
using MySql.Data.MySqlClient;

namespace Xclass.Database
{
    /// <summary>
    /// Provide methods for connection and performing queries to MySQL database
    /// WARNING! This class has dependency: MySql.Data.dll (MySQL Connector/NET from http://www.mysql.com)
    /// </summary>
    public class MySQL
    {
        /// <summary>
        /// Connection string e.g. "Server=http://server.com;Database=DatabaseName;Uid=Username;Pwd=Password;"
        /// Changing will cause disconnect if database is opened at this moment
        /// </summary>
        public string ConnectionString
        {
            get
            {
                return connect.ConnectionString;
            }
            set
            {
                Disconnect();
                connect.ConnectionString = value;
            }
        }

        /// <summary>
        /// Get last registered error message
        /// </summary>
        public string LastRegisteredErrorMessage
        {
            get;
            private set;
        }

        /// <summary>
        /// Get error message from last query
        /// </summary>
        public string LastQueryErrorMessage
        {
            get;
            private set;
        }

        /// <summary>
        /// Is tracing enabled
        /// </summary>
        public bool TraceEnabled
        {
            get;
            private set;
        }

        /// <summary>
        /// Is connection should be kept open
        /// </summary>
        public bool KeepDatabaseOpened
        {
            get;
            private set;
        }

        private readonly MySqlConnection connect = new MySqlConnection();


        /// <summary>
        /// Perform connection test
        /// </summary>
        /// <param name="pConnectionString">Connection string e.g. "Provider=Microsoft.Jet.OleDb.4.0;Data Source=db.mdb;"</param>
        /// <param name="pSaveIfSuccess">Save connection string from pSaveIfSuccess to ConnectionString if success</param>
        /// <param name="pKeepDatabaseOpened">Keep open connection open if connection will be established</param>
        /// <returns>True if connection can be established and false if operation is failed</returns>
        public bool TestConnection(string pConnectionString = null, bool pSaveIfSuccess = false, bool pKeepDatabaseOpened = false)
        {
            using (var connect = new MySqlConnection(pConnectionString))
            {
                try
                {
                    connect.Open();
                    connect.Close();
                    if (pSaveIfSuccess)
                    {
                        ConnectionString = pConnectionString;
                    }
                    KeepDatabaseOpened = pKeepDatabaseOpened;
                    return true;
                }
                catch (Exception ex)
                {
                    RegisterError(ex.Message);
                    return false;
                }
            }
        }

        /// <summary>
        /// Check is connection is active now
        /// </summary>
        /// <returns>True if active, false if not</returns>
        public bool IsActiveConnection()
        {
            return ((connect.State == ConnectionState.Open) || (connect.State == ConnectionState.Executing) || (connect.State == ConnectionState.Fetching));
        }
        /// <summary>
        /// Close connection
        /// </summary>
        public void Disconnect()
        {
            connect.Close();
        }

        private void RegisterError(string Message)
        {
            LastRegisteredErrorMessage = Message;
            LastQueryErrorMessage = Message;
        }

        /// <summary>
        /// Perform SELECT query
        /// </summary>
        /// <param name="query">Query statement</param>
        /// <param name="args">Query arguments</param>
        /// <returns>All found table</returns>
        public DataTable SelectTable(string query, params MySqlParameter[] args)
        {
            var table = new DataTable();
            try
            {
                LastQueryErrorMessage = "";
                if (connect.State == ConnectionState.Closed) connect.Open();
                var adapter = new MySqlDataAdapter(query, connect);
                if (args != null)
                {
                    foreach (var param in args)
                    {
                        adapter.SelectCommand.Parameters.Add(param);
                    }
                }
                adapter.Fill(table);
                return table;
            }
            catch (Exception ex)
            {
                RegisterError(ex.Message);
                return null;
            }
            finally
            {
                if (!KeepDatabaseOpened) connect.Close();
            }

        }
        /// <summary>
        /// Perform SELECT query
        /// </summary>
        /// <param name="query">Query statement</param>
        /// <param name="args">Query arguments</param>
        /// <returns>1st found row</returns>
        public DataRow SelectRow(string query, params MySqlParameter[] args)
        {
            var table = SelectTable(query, args);
            return table != null && table.Rows.Count == 1 ? table.Rows[0] : null;
        }
        /// <summary>
        /// Perform SELECT query
        /// </summary>
        /// <param name="query">Query statement</param>
        /// <param name="args">Query arguments</param>
        /// <returns>1st found column</returns>
        public DataColumn SelectColumn(string query, params MySqlParameter[] args)
        {
            var table = SelectTable(query, args);
            return table != null && table.Columns.Count == 1 ? table.Columns[0] : null;
        }
        /// <summary>
        /// Perform SELECT query
        /// </summary>
        /// <typeparam name="TReturnType">Expected data type</typeparam>
        /// <param name="query">Query statement</param>
        /// <param name="args">Query arguments</param>
        /// <returns>1st cell in 1st row of 1st column</returns>
        public TReturnType SelectCell<TReturnType>(string query, params MySqlParameter[] args)
        {
            var table = SelectTable(query, args);
            if (table == null || table.Rows.Count != 1 || table.Rows[0].ItemArray[0].GetType() != typeof(TReturnType))
            {
                throw new InvalidOperationException(string.Concat("Wrong query or type of cell is not equal type of ReturnType. Return type is equals ", table.Rows[0].ItemArray[0].GetType().ToString()));
            }
            return (TReturnType)table.Rows[0].ItemArray[0];
        }

        /// <summary>
        /// Perform SELECT query
        /// </summary>
        /// <typeparam name="TReturnType">Expected data type</typeparam>
        /// <param name="query">Query statement</param>
        /// <param name="DefReturnValue">Default value that was return by method</param>
        /// <param name="args">Query arguments</param>
        /// <returns>1st cell in 1st row of 1st column</returns>
        public TReturnType SelectCell<TReturnType>(string query, TReturnType DefReturnValue = default(TReturnType), params MySqlParameter[] args)
        {
            var table = SelectTable(query, args);
            if (table == null || table.Rows.Count == 0 || table.Rows[0].ItemArray[0].GetType() != typeof(TReturnType))
            {
                return DefReturnValue;
            }
            return (TReturnType)table.Rows[0].ItemArray[0];
        }

        /// <summary>
        /// Perform INSERT,UPDATE,DELETE etc queries
        /// </summary>
        /// <param name="query">Query statement</param>
        /// <param name="args">Query arguments</param>
        /// <returns>Number of affected rows</returns>
        public int ChangeData(string query, params MySqlParameter[] args)
        {
            try
            {
                LastQueryErrorMessage = "";
                if (connect.State == ConnectionState.Closed) connect.Open();
                var command = new MySqlCommand(query, connect);
                if (args != null)
                {
                    foreach (var param in args)
                    {
                        command.Parameters.Add(param);
                    }
                }
                return command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                RegisterError(ex.Message);
                return -1;
            }
            finally
            {
                if (!KeepDatabaseOpened) connect.Close();
            }
        }
    }
}
