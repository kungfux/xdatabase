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
    public abstract partial class XQuery
    {
        public bool IsInTransactionMode => _transaction != null;
        public XDatabaseType CurrentXDatabaseType { get; internal set; }

        public int Timeout
        {
            get
            {
                return _operationTimeout;
            }
            set
            {
                if (value > 0)
                {
                    _operationTimeout = value;
                }
            }
        }

        public string ConnectionString
        {
            get
            {
                return _databaseConnectionString;
            }
            set
            {
                if (CheckIsConnectionCanBeEstablished(value))
                {
                    _databaseConnectionString = value;
                }
            }
        }

        public bool IsConnectionOpened
        {
            get
            {
                if (_connection != null)
                {
                    return _connection.State == ConnectionState.Open ||
                           _connection.State == ConnectionState.Executing ||
                           _connection.State == ConnectionState.Fetching;
                }
                else
                {
                    return false;
                }
            }
        }

        private DbConnection _connection;
        private string _databaseConnectionString;
        private int _operationTimeout = (int)TimeSpan.FromSeconds(30).TotalMilliseconds;
    }
}
