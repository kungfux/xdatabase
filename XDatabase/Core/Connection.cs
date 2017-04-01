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
using System.Data.Common;

namespace XDatabase.Core
{
    public abstract partial class XQuery : IDisposable
    {
        public DbConnection Connection { get; private set; }
        public string ConnectionString;
        public bool KeepConnectionOpen = false;

        public int Timeout
        {
            get { return _timeout; }
            set
            {
                if (value > 0)
                {
                    _timeout = value;
                }
            }
        }

        public bool IsConnectionActive
        {
            get
            {
                if (Connection != null)
                {
                    return Connection.State == ConnectionState.Open ||
                           Connection.State == ConnectionState.Executing ||
                           Connection.State == ConnectionState.Fetching;
                }
                return false;
            }
        }

        public bool TestConnection(string connectionString = null)
        {
            ClearError();

            using (var newConnection = GetConnection())
            {
                try
                {
                    newConnection.ConnectionString = connectionString ?? ConnectionString;
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

        private int _timeout = (int)TimeSpan.FromSeconds(30).TotalMilliseconds;

        private void OpenConnection()
        {
            ClearError();

            Connection = GetConnection();
            try
            {
                Connection.ConnectionString = ConnectionString;
                Connection.Open();
            }
            catch (Exception ex)
            {
                RegisterError(ex.Message);
            }
        }

        private void CloseConnection()
        {
            if (Connection != null && !IsInTransactionMode && !KeepConnectionOpen)
            {
                Connection.Close();
            }
        }

        public void Dispose()
        {
            Connection.Close();
        }
    }
}
