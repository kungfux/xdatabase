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
using System.Data.Common;

namespace XDatabase.Core
{
    public abstract partial class XQuery
    {
        private DbTransaction _transaction;

        public int ChangeData(string pSqlQuery, params DbParameter[] pDataArgs)
        {
            ClearError();
            try
            {
                if (!IsConnectionOpened)
                {
                    OpenConnection();
                }
                using (var command = GetCommand())
                {
                    command.Connection = _connection;
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
                RegisterError(ex.Message);
                return -1;
            }
            finally
            {
                CloseConnection();
            }
        }

        public bool BeginTransaction()
        {
            ClearError();
            try
            {
                if (!IsConnectionOpened)
                {
                    OpenConnection();
                }
                _transaction = _connection.BeginTransaction();
                return true;
            }
            catch (Exception ex)
            {
                RegisterError(ex.Message);
                return false;
            }
        }

        public bool CommitTransaction()
        {
            ClearError();
            try
            {
                _transaction.Commit();
                _transaction = null;
                return true;
            }
            catch (Exception ex)
            {
                RegisterError(ex.Message);
                return false;
            }
            finally
            {
                CloseConnection();
            }
        }

        public bool RollbackTransaction()
        {
            ClearError();
            try
            {
                _transaction.Rollback();
                _transaction = null;
                return true;
            }
            catch (Exception ex)
            {
                RegisterError(ex.Message);
                return false;
            }
            finally
            {
                CloseConnection();
            }
        }
    }
}
