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
using System.IO;
using System.Drawing;

namespace Xclass.Database
{
    /// <summary>
    /// Provide methods for connection and performing queries to SQLite database
    /// WARNING! This class has dependencies: System.Data.SQLite.dll, SQLite.Interop.dll (Precompiled Binary for .NET from http://www.sqlite.org/)
    /// </summary>
    public class SQLite3Query
    {
        private readonly SQLiteConnection connection = new SQLiteConnection();

        /// <summary>
        /// Register error for last operation
        /// </summary>
        /// <param name="pMessage">Error message</param>
        private void registerError(string pMessage)
        {
            LastOperationErrorMessage = pMessage;
        }

        /// <summary>
        /// Clear error message before new operation begins
        /// </summary>
        private void clearError()
        {
            LastOperationErrorMessage = "";
        }

        /// <summary>
        /// Contains currently used connection string
        /// </summary>
        public string ConnectionString
        {
            get
            {
                return connection.ConnectionString;
            }
        }

        /// <summary>
        /// Contains time in seconds for all operations timeout
        /// </summary>
        private int operationTimeout = 30;

        /// <summary>
        /// Contains time in seconds for all operations timeout
        /// </summary>
        public int OperationTimeout
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
        /// Contains value indicated is connection should be kept opened
        /// </summary>
        public bool KeepDatabaseOpened
        {
            get;
            private set;
        }

        /// <summary>
        /// Get value indicated is connection is active now
        /// </summary>
        public bool IsConnectionIsActive
        {
            get
            {
                return ((connection.State == ConnectionState.Open) ||
                    (connection.State == ConnectionState.Executing) ||
                    (connection.State == ConnectionState.Fetching));
            }
        }

        /// <summary>
        /// Contains error message from last operation (if exist)
        /// </summary>
        public string LastOperationErrorMessage
        {
            get;
            private set;
        }

        /// <summary>
        /// Perform connection test
        /// </summary>
        /// <param name="pConnectionString">Connection string e.g. "Data Source=db.sqlite;Version=3;UTF8Encoding=True;foreign keys=true;"</param>
        /// <param name="pSaveIfSuccess">Save connection string from pSaveIfSuccess to ConnectionString if success</param>
        /// <param name="pKeepDatabaseOpened">Keep connection opened</param>
        /// <returns>True if connection can be established and false if operation is failed</returns>
        public bool TestConnection(string pConnectionString = null, bool pSaveIfSuccess = false, bool pKeepDatabaseOpened = false)
        {
            clearError();
            using (var connect = new SQLiteConnection(pConnectionString))
            {
                try
                {
                    connect.Open();
                    connect.Close();
                    if (pSaveIfSuccess)
                    {
                        connection.ConnectionString = pConnectionString;
                    }
                    KeepDatabaseOpened = pKeepDatabaseOpened;
                    return true;
                }
                catch (Exception ex)
                {
                    registerError(ex.Message);
                    return false;
                }
            }
        }

        /// <summary>
        /// Close active connection
        /// </summary>
        public void Disconnect()
        {
            connection.Close();
        }

        /// <summary>
        /// Perform SELECT query and return all results
        /// </summary>
        /// <param name="pQuerySql">Query statement</param>
        /// <param name="pArgs">Query arguments</param>
        /// <returns>System.Data.DataTable or null</returns>
        public DataTable SelectTable(string pQuerySql, params SQLiteParameter[] pArgs)
        {
            clearError();
            var table = new DataTable();
            try
            {
                if (!IsConnectionIsActive)
                {
                    connection.Open();
                }
                using (var adapter = new SQLiteDataAdapter(pQuerySql, connection))
                {
                    adapter.SelectCommand.Parameters.Clear();
                    adapter.SelectCommand.CommandTimeout = OperationTimeout;
                    if (pArgs != null)
                    {
                        foreach (var arg in pArgs)
                        {
                            adapter.SelectCommand.Parameters.Add(arg);
                        }
                    }
                    adapter.Fill(table);
                    return table;
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
                    connection.Close();
                }
            }
        }

        /// <summary>
        /// Perform SELECT query and return single row in case one row in results only
        /// </summary>
        /// <param name="pQuerySql">Query statement</param>
        /// <param name="pArgs">Query arguments</param>
        /// <returns>System.Data.DataRow or null</returns>
        public DataRow SelectRow(string pQuerySql, params SQLiteParameter[] pArgs)
        {
            var table = SelectTable(pQuerySql, pArgs);
            return table != null && table.Rows.Count == 1 ? table.Rows[0] : null;
        }

        /// <summary>
        /// Perform SELECT query and return specified single row
        /// </summary>
        /// <param name="pQuerySql">Query statement</param>
        /// <param name="pReturnRowIndex">Row index/number to return</param>
        /// <param name="pArgs">Query arguments</param>
        /// <returns>System.Data.DataRow or null</returns>
        public DataRow SelectRow(string pQuerySql, int pReturnRowIndex, params SQLiteParameter[] pArgs)
        {
            var table = SelectTable(pQuerySql, pArgs);
            return table != null && table.Rows.Count > 0 && table.Rows.Count >= pReturnRowIndex ? table.Rows[pReturnRowIndex] : null;
        }

        /// <summary>
        /// Perform SELECT query and return single column in case one column in results only
        /// </summary>
        /// <param name="pQuerySql">Query statement</param>
        /// <param name="pArgs">Query arguments</param>
        /// <returns>System.Data.DataRow or null</returns>
        public DataColumn SelectColumn(string pQuerySql, params SQLiteParameter[] pArgs)
        {
            var table = SelectTable(pQuerySql, pArgs);
            return table != null && table.Columns.Count == 1 ? table.Columns[0] : null;
        }

        /// <summary>
        /// Perform SELECT query and return specified single column
        /// </summary>
        /// <param name="pQuerySql">Query statement</param>
        /// <param name="pReturnColumnIndex">Column index/number to return</param>
        /// <param name="pArgs">Query arguments</param>
        /// <returns>System.Data.DataColumn or null</returns>
        public DataColumn SelectColumn(string pQuerySql, int pReturnColumnIndex, params SQLiteParameter[] pArgs)
        {
            var table = SelectTable(pQuerySql, pArgs);
            return table != null && table.Columns.Count > 0 && table.Columns.Count >= pReturnColumnIndex ? table.Columns[pReturnColumnIndex] : null;
        }

        /// <summary>
        /// Perform SELECT query and return single cell in case one cell in results only
        /// </summary>
        /// <typeparam name="T">Expected data type</typeparam>
        /// <param name="pQuerySql">Query statement</param>
        /// <param name="pArgs">Query arguments</param>
        /// <returns>System.Data.DataRow or null</returns>
        public T SelectCell<T>(string pQuerySql, params SQLiteParameter[] pArgs)
        {
            var table = SelectTable(pQuerySql, pArgs);
            if (table != null && table.Rows.Count == 1 && table.Columns.Count == 1 && table.Rows[0].ItemArray[0].GetType() == typeof(T))
            {
                return (T)table.Rows[0].ItemArray[0];
            }
            else
            {
                if (table != null && table.Rows.Count == 1 && table.Columns.Count == 1)
                {
                    throw new FormatException("Type of cell is not equals to specified type of T. Type of cell is equals to " +
                    table.Rows[0].ItemArray[0].GetType().ToString());
                }
                else if (table != null && (table.Rows.Count != 1 || table.Columns.Count != 1))
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
        /// <param name="pQuerySql">Query statement</param>
        /// <param name="pDefaultValue">Default value that will be returned in case of error</param>
        /// <param name="pArgs">Query arguments</param>
        /// <returns>System.Data.DataRow or null</returns>
        public T SelectCell<T>(string pQuerySql, T pDefaultValue = default(T), params SQLiteParameter[] pArgs)
        {
            var table = SelectTable(pQuerySql, pArgs);
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
        /// <param name="pQuerySql">Query statement</param>
        /// <param name="pArgs">Query arguments</param>
        /// <returns>Number of affected rows or -1 in case error occur</returns>
        public int ChangeData(string pQuerySql, params SQLiteParameter[] pArgs)
        {
            clearError();
            try
            {
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }
                using (var command = new SQLiteCommand(pQuerySql, connection))
                {
                    command.CommandTimeout = OperationTimeout;
                    if (pArgs != null)
                    {
                        foreach (var arg in pArgs)
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
                    connection.Close();
                }
            }
        }

        /// <summary>
        /// Perform several INSERT, UPDATE, DELETE etc queries in one transaction
        /// </summary>
        /// <param name="pQueryStatements">Query statements organized using SQLiteQueryStatement[]</param>
        /// <returns>Number of total affected rows or -1 in case error occur</returns>
        public int PerformTransaction(SQLiteQueryStatement[] pQueryStatements)
        {
            clearError();

            int result = 0;
            
            try
            {
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }
                using (var command = new SQLiteCommand(connection))
                {
                    using (var transaction = connection.BeginTransaction())
                    {
                        for (int a = 0; a < pQueryStatements.Length; a++)
                        {
                            command.CommandText = pQueryStatements[a].QuerySql;
                            if (pQueryStatements[a].QueryParameters != null)
                            {
                                foreach (SQLiteParameter arg in pQueryStatements[a].QueryParameters)
                                {
                                    command.Parameters.Add(arg);
                                }
                            }
                            result += command.ExecuteNonQuery();
                        }
                        transaction.Commit();
                        return result;
                    }
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
                    connection.Close();
                }
            }
        }

        /// <summary>
        /// Save source of file into special cell in database
        /// </summary>
        /// <param name="pFileFullPath">Source file</param>
        /// <param name="pQuerySql">Query statement</param>
        /// <param name="pArgs">Query arguments</param>
        /// <returns>True if success operation, false if not</returns>
        public bool PutFile(string pFileFullPath, string pQuerySql, params SQLiteParameter[] pArgs)
        {
            clearError();

            try
            {
                var fileStream = new FileStream(pFileFullPath, FileMode.Open, FileAccess.Read);
                var fileBytes = new byte[fileStream.Length];
                fileStream.Read(fileBytes, 0, (int)fileStream.Length);
                fileStream.Close();
                var argsExtended = new SQLiteParameter[pArgs.Length + 1];
                var index = 0;
                foreach (var p in pArgs)
                {
                    argsExtended.SetValue(p, index);
                    index++;
                }
                argsExtended.SetValue(new SQLiteParameter("@file", fileBytes), index);
                return ChangeData(pQuerySql, argsExtended) > 0;
            }
            catch (Exception ex)
            {
                registerError(ex.Message);
                return false;
            }
        }
        /// <summary>
        /// Read data of cell and convert it to Image
        /// </summary>
        /// <param name="pQuerySql">Query statement</param>
        /// <returns>Image</returns>
        public Image GetImage(string pQuerySql, params SQLiteParameter[] pArgs)
        {
            clearError();

            try
            {
                var fileBytes = SelectCell<byte[]>(pQuerySql, pArgs);
                var memStream = new MemoryStream(fileBytes);
                var image = Image.FromStream(memStream);
                return image;
            }
            catch (Exception ex)
            {
                registerError(ex.Message);
                return null;
            }
        }
        /// <summary>
        /// Read data of cell and convert it to byte[]
        /// </summary>
        /// <param name="pQuerySql">Query statement</param>
        /// <returns>Bytes</returns>
        public byte[] GetFile(string pQuerySql)
        {
            clearError();

            try
            {
                var fileBytes = SelectCell<byte[]>(pQuerySql);
                return fileBytes;
            }
            catch (Exception ex)
            {
                registerError(ex.Message);
                return null;
            }
        }
        /// <summary>
        /// Read data of cell, convert it to byte[] and save on disc
        /// </summary>
        /// <param name="pOutputFile">Destination file</param>
        /// <param name="pQuerySql">Query statement</param>
        /// <returns>True if success operation. false if not</returns>
        public bool GetAndSaveFile(string pOutputFile, string pQuerySql)
        {
            clearError();

            try
            {
                var fileBytes = SelectCell<byte[]>(pQuerySql);
                var newFileStream = new FileStream(pOutputFile, FileMode.CreateNew);
                newFileStream.Write(fileBytes, 0, fileBytes.Length);
                newFileStream.Close();
                return true;
            }
            catch (Exception ex)
            {
                registerError(ex.Message);
                return false;
            }
        }
    }
}