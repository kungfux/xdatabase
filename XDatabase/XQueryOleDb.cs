﻿/*
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

using System.Data.Common;
using System.Data.OleDb;
using XDatabase.Core;

namespace XDatabase
{
    public class XQueryOleDb : XQuery
    {
        public XQueryOleDb()
        {
            
        }

        public XQueryOleDb(string connectionString)
        {
            ConnectionString = connectionString;
        }

        protected override DbParameter GetParameter() => new OleDbParameter();
        protected override DbConnection GetConnection() => new OleDbConnection();
        protected override DbDataAdapter GetDataAdapter() => new OleDbDataAdapter();
        protected override DbCommand GetCommand() => new OleDbCommand();
    }
}
