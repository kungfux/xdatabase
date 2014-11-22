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
using System.Data.SQLite;

namespace Xclass.Database
{
    /// <summary>
    /// Provide methods for connection and performing queries to SQLite database
    /// WARNING! This class has dependency: System.Data.SQLite.dll (Precompiled Binary for .NET from http://www.sqlite.org/)
    /// </summary>
    public class SQLite
    {
        /// <summary>
        /// Connection string
        /// </summary>
        public string ConnectionString
        {
            get
            {
                return Connect.ConnectionString;
            }
            set
            {
                Disconnect();
                Connect.ConnectionString = value;
            }
        }

        /// <summary>
        /// Last error message
        /// </summary>
        public string LastErrorMessage
        {
            get;
            private set;
        }

        /// <summary>
        /// Error message from last query operation
        /// </summary>
        public string LastQueryErrorMessage
        {
            get;
            private set;
        }

        /// <summary>
        /// Is connection should be kept open
        /// </summary>
        public bool KeepOpen
        {
            get;
            private set;
        }

        private readonly SQLiteConnection Connect = new SQLiteConnection();

        /// <summary>
        /// Perform connection test
        /// </summary>
        /// <returns>True if connection can be established false if failed</returns>
        public bool TestConnection()
        {
            return TestConnection(ConnectionString, false, null);
        }

        /// <summary>
        /// Perform connection test
        /// </summary>
        /// <param name="connectionString">Connection string</param>
        /// <returns>True if connection can be established false if failed</returns>
        public bool TestConnection(string connectionString)
        {
            return TestConnection(connectionString, false, null);
        }

        /// <summary>
        /// Perform connection test
        /// </summary>
        /// <param name="connectionString">Connection string</param>
        /// <param name="saveIfSuccess">Save new connection string if success</param>
        /// <returns>True if connection can be established false if failed</returns>
        public bool TestConnection(string connectionString, bool saveIfSuccess)
        {
            return TestConnection(connectionString, saveIfSuccess, null);
        }

        /// <summary>
        /// Perform connection test
        /// </summary>
        /// <param name="connectionString">Connection string</param>
        /// <param name="saveIfSuccess">Save new connection string if success</param>
        /// <param name="isKeepOpen">Keep open connection open if connection will be established</param>
        /// <returns>True if connection can be established false if failed</returns>
        public bool TestConnection(string connectionString, bool saveIfSuccess, bool? isKeepOpen)
        {
            using (var connect = new SQLiteConnection(connectionString))
            {
                try
                {
                    LastQueryErrorMessage = "";
                    connect.Open();
                    connect.Close();
                    if (saveIfSuccess) ConnectionString = connectionString;
                    if (isKeepOpen != null) KeepOpen = (bool)isKeepOpen;
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
            return ((Connect.State == ConnectionState.Open) || (Connect.State == ConnectionState.Executing) || (Connect.State == ConnectionState.Fetching));
        }
        /// <summary>
        /// Close connection
        /// </summary>
        public void Disconnect()
        {
            Connect.Close();
        }

        private void RegisterError(string Message)
        {
            LastErrorMessage = Message;
            LastQueryErrorMessage = Message;
        }

        /// <summary>
        /// Perform SELECT query
        /// </summary>
        /// <param name="query">Query statement</param>
        /// <param name="args">Query arguments</param>
        /// <returns>All found table</returns>
        public DataTable SelectTable(string query, params SQLiteParameter[] args)
        {
            var table = new DataTable();
            try
            {
                LastQueryErrorMessage = "";
                if (Connect.State == ConnectionState.Closed) Connect.Open();
                var adapter = new SQLiteDataAdapter(query, Connect);
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
                if (!KeepOpen) Connect.Close();
            }

        }

        /// <summary>
        /// Perform SELECT query
        /// </summary>
        /// <param name="query">Query statement</param>
        /// <param name="args">Query arguments</param>
        /// <returns>1st found row</returns>
        public DataRow SelectRow(string query, params SQLiteParameter[] args)
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
        public DataColumn SelectColumn(string query, params SQLiteParameter[] args)
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
        public TReturnType SelectCell<TReturnType>(string query, params SQLiteParameter[] args)
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
        public TReturnType SelectCell<TReturnType>(string query, TReturnType DefReturnValue = default(TReturnType), params SQLiteParameter[] args)
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
        public int ChangeData(string query, params SQLiteParameter[] args)
        {
            try
            {
                LastQueryErrorMessage = "";
                if (Connect.State == ConnectionState.Closed) Connect.Open();
                var command = new SQLiteCommand(query, Connect);
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
                if (!KeepOpen) Connect.Close();
            }
        }
    }
}