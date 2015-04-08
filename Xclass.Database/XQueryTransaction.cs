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
    public partial class XQuery
    {
        private DbTransaction transaction = null;

        /// <summary>
        /// Begin new transaction
        /// </summary>
        /// <returns>True if success, false if not</returns>
        public bool BeginTransaction()
        {
            clearError();
            try
            {
                if (!IsActiveConnection)
                {
                    openConnection();
                }
                transaction = connection.BeginTransaction();
                return true;
            }
            catch (Exception ex)
            {
                registerError(ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Commit transaction
        /// </summary>
        /// <returns>True if success, false if not</returns>
        public bool CommitTransaction()
        {
            clearError();
            try
            {
                transaction.Commit();
                return true;
            }
            catch (Exception ex)
            {
                registerError(ex.Message);
                return false;
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
        /// Rollback transaction
        /// </summary>
        /// <returns>True if success, false if not</returns>
        public bool RollbackTransaction()
        {
            clearError();
            try
            {
                transaction.Rollback();
                return true;
            }
            catch (Exception ex)
            {
                registerError(ex.Message);
                return false;
            }
            finally
            {
                if (!KeepDatabaseOpened)
                {
                    closeConnection();
                }
            }
        }
    }
}
