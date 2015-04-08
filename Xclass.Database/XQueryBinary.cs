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
using System.Drawing;
using System.IO;

namespace Xclass.Database
{
    public partial class XQuery
    {
        public bool PutBinary(byte[] pBinaryData, string pSqlQuery, string pBinaryArgumentName)
        {
            clearError();

            try
            {
                var parameter = getParameter();
                parameter.ParameterName = pBinaryArgumentName;
                parameter.Value = pBinaryData;

                return ChangeData(pSqlQuery, parameter) > 0;
            }
            catch (Exception ex)
            {
                registerError(ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Save source of file into special cell in database
        /// </summary>
        /// <param name="pFromFile">Source file</param>
        /// <param name="pSqlQuery">Query statement</param>
        /// <param name="pDataArgs">Query arguments</param>
        /// <returns>True if success operation, false if not</returns>
        public bool PutFile(string pFromFile, string pSqlQuery, string pBinaryArgumentName)
        {
            clearError();

            try
            {
                var fileStream = new FileStream(pFromFile, FileMode.Open, FileAccess.Read);
                var fileBytes = new byte[fileStream.Length];
                fileStream.Read(fileBytes, 0, (int)fileStream.Length);
                fileStream.Close();

                return PutBinary(fileBytes, pSqlQuery, pBinaryArgumentName);
            }
            catch (Exception ex)
            {
                registerError(ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Read data of cell and convert it to byte[]
        /// </summary>
        /// <param name="pSqlQuery">Query statement</param>
        /// <returns>Bytes or null</returns>
        public byte[] GetBinaryData(string pSqlQuery, params DbParameter[] pDataArgs)
        {
            clearError();

            try
            {
                var fileBytes = SelectCell<byte[]>(pSqlQuery, pDataArgs);
                return fileBytes;
            }
            catch (Exception ex)
            {
                registerError(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Read data of cell and convert it to Image
        /// </summary>
        /// <param name="pSqlQuery">Query statement</param>
        /// <returns>Image</returns>
        public Image GetBinaryAsImage(string pSqlQuery, params DbParameter[] pDataArgs)
        {
            clearError();

            try
            {
                var fileBytes = SelectCell<byte[]>(pSqlQuery, pDataArgs);
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
        /// Read data of cell, convert it to byte[] and save on disk
        /// </summary>
        /// <param name="pOutputFile">Destination file</param>
        /// <param name="pSqlQuery">Query statement</param>
        /// <returns>True if success operation. false if not</returns>
        public bool GetBinaryAndSaveToFile(string pOutputFile, string pSqlQuery)
        {
            clearError();

            try
            {
                var fileBytes = SelectCell<byte[]>(pSqlQuery);
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
