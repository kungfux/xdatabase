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
using System.Drawing;
using System.IO;

namespace XDatabase.Core
{
    public abstract partial class XQuery
    {
        public bool InsertBinaryIntoCell(byte[] binaryData, string sqlQuery, string argumentNameWithBinaryData)
        {
            ClearError();

            try
            {
                var parameter = GetParameter();
                parameter.ParameterName = argumentNameWithBinaryData;
                parameter.Value = binaryData;

                return Update(sqlQuery, parameter) > 0;
            }
            catch (Exception ex)
            {
                RegisterError(ex.Message);
                return false;
            }
        }

        public bool InsertFileIntoCell(string fileFullPath, string sqlQuery, string argumentNameWithFilePath)
        {
            ClearError();

            try
            {
                var fileStream = new FileStream(fileFullPath, FileMode.Open, FileAccess.Read);
                var fileBytes = new byte[fileStream.Length];
                fileStream.Read(fileBytes, 0, (int)fileStream.Length);
                fileStream.Close();

                return InsertBinaryIntoCell(fileBytes, sqlQuery, argumentNameWithFilePath);
            }
            catch (Exception ex)
            {
                RegisterError(ex.Message);
                return false;
            }
        }

        public Image SelectBinaryAsImage(string sqlQuery, params XParameter[] args)
        {
            ClearError();

            try
            {
                var fileBytes = SelectCellAs<byte[]>(sqlQuery, args);
                var memStream = new MemoryStream(fileBytes);
                var image = Image.FromStream(memStream);
                return image;
            }
            catch (Exception ex)
            {
                RegisterError(ex.Message);
                return null;
            }
        }

        public bool SelectBinaryAndSave(string outputFileName, string sqlQuery)
        {
            ClearError();

            try
            {
                var fileBytes = SelectCellAs<byte[]>(sqlQuery);
                var newFileStream = new FileStream(outputFileName, FileMode.CreateNew);
                newFileStream.Write(fileBytes, 0, fileBytes.Length);
                newFileStream.Close();
                return true;
            }
            catch (Exception ex)
            {
                RegisterError(ex.Message);
                return false;
            }
        }
    }
}
