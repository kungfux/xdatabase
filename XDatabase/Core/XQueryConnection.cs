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

namespace XDatabase.Core
{
    public abstract partial class XQuery
    {
        private bool CheckIsConnectionCanBeEstablished(string connectionString)
        {
            ClearError();

            using (var newConnection = GetConnection())
            {
                try
                {
                    newConnection.ConnectionString = connectionString;
                    newConnection.Open();
                    newConnection.Close();
                    return true;
                }
                catch (Exception ex)
                {
                    RegisterError(ex.Message);
                    return false;
                }
            }
        }

        private void OpenConnection()
        {
            ClearError();

            _connection = GetConnection();
            try
            {
                _connection.ConnectionString = ConnectionString;
                _connection.Open();
            }
            catch (Exception ex)
            {
                RegisterError(ex.Message);
            }
        }

        private void CloseConnection()
        {
            if (_connection != null && !IsInTransactionMode)
            {
                _connection.Close();
            }
        }
    }
}
