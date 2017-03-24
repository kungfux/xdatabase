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

using System;
using System.Data;

namespace XDatabase.Core
{
    public abstract partial class XQuery
    {
        public DataTable SelectTable(string sqlQuery, params XParameter[] args)
        {
            ClearError();
            var tableResults = new DataTable();
            try
            {
                if (!IsConnectionActive)
                {
                    OpenConnection();
                }
                using (var adapter = GetDataAdapter())
                {
                    var command = GetCommand();
                    command.Connection = Connection;
                    command.CommandText = sqlQuery;
                    command.CommandTimeout = Timeout;

                    adapter.SelectCommand = command;

                    adapter.SelectCommand.Parameters.Clear();
                    
                    if (args != null)
                    {
                        foreach (var arg in args)
                        {
                            var parameter = GetParameter(arg.ParameterName, arg.Value);
                            adapter.SelectCommand.Parameters.Add(parameter);
                        }
                    }
                    adapter.Fill(tableResults);
                    return tableResults;
                }
            }
            catch (Exception ex)
            {
                RegisterError(ex.Message);
                return null;
            }
            finally
            {
                CloseConnection();
            }
        }

        public DataRow SelectRow(string sqlQuery, params XParameter[] args)
        {
            var table = SelectTable(sqlQuery, args);
            return table != null && table.Rows.Count == 1 ? table.Rows[0] : null;
        }

        public DataColumn SelectColumn(string sqlQuery, params XParameter[] args)
        {
            var table = SelectTable(sqlQuery, args);
            return table != null && table.Columns.Count == 1 ? table.Columns[0] : null;
        }

        public T SelectCellAs<T>(string sqlQuery, params XParameter[] args)
        {
            var table = SelectTable(sqlQuery, args);
            if (table != null && 
                table.Rows.Count == 1 && 
                table.Columns.Count == 1 && 
                table.Rows[0].ItemArray[0].GetType() == typeof(T))
            {
                return (T)table.Rows[0].ItemArray[0];
            }

            if (table != null && 
                table.Rows.Count == 1 && 
                table.Columns.Count == 1)
            {
                throw new FormatException($"Type of the cell is not equals to specified type of T. Type of cell equals to {table.Rows[0].ItemArray[0].GetType()}.");
            }

            if (table != null && 
                (table.Rows.Count != 1 || 
                 table.Columns.Count != 1))
            {
                throw new DataException($"It is expected that query will return 1x1 size table but was returned {table.Rows.Count}x{table.Columns.Count}.");
            }

            throw new DataException("It is expected that query will return 1x1 size table but empty result was returned.");
        }

        public T SelectCellAs<T>(string sqlQuery, T defaultValue = default(T), params XParameter[] args)
        {
            var table = SelectTable(sqlQuery, args);
            if (table != null && table.Rows.Count == 1 && table.Columns.Count == 1 && table.Rows[0].ItemArray[0].GetType() == typeof(T))
            {
                return (T)table.Rows[0].ItemArray[0];
            }
            else
            {
                return defaultValue;
            }
        }
    }
}
