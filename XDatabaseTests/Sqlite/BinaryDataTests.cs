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

using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using NUnit.Framework;
using XDatabase;

namespace XDatabaseTests.Sqlite
{
    public class BinaryDataTests
    {
        [Test]
        public void TestInsertBinaryDataIntoCell()
        {
            const string sqlCreateTable = "create table test (bin blob);";
            const string sqlInsertBin = "insert into test (bin) values (@bin);";
            var binData = new byte[] { 1,0,1,1,0,1,1,0 };
            
            var xQuery = new XQuerySqlite(SetUp.SqliteConnectionString);
            xQuery.BeginTransaction();
            xQuery.Insert(sqlCreateTable);
            Assert.IsTrue(xQuery.InsertBinaryIntoCell(binData, sqlInsertBin, "@bin"));
        }

        [Test]
        public void TestInsertFileIntoCell()
        {
            const string sqlCreateTable = "create table test (bin blob);";
            const string sqlInsertFile = "insert into test (bin) values (@bin);";
            var file = Assembly.GetExecutingAssembly().Location;

            var xQuery = new XQuerySqlite(SetUp.SqliteConnectionString);
            xQuery.BeginTransaction();
            xQuery.Insert(sqlCreateTable);
            Assert.IsTrue(xQuery.InsertFileIntoCell(file, sqlInsertFile, "@bin"));
        }

        [Test]
        public void TestSelectBinaryDataFromCell()
        {
            const string sqlCreateTable = "create table test (bin blob);";
            const string sqlInsertBin = "insert into test (bin) values (@bin);";
            var binData = new byte[] { 1, 0, 1, 1, 0, 1, 1, 0 };

            var xQuery = new XQuerySqlite(SetUp.SqliteConnectionString);
            xQuery.BeginTransaction();
            xQuery.Insert(sqlCreateTable);
            xQuery.InsertBinaryIntoCell(binData, sqlInsertBin, "@bin");
            var result = xQuery.SelectCell<byte[]>("select bin from test");
            Assert.AreEqual(binData, result);
        }

        [Test]
        public void TestSelectBinaryDataFromCellAsImage()
        {
            const string sqlCreateTable = "create table test (bin blob);";
            const string sqlInsertImage = "insert into test (bin) values (@bin);";
            const string sqlSelectImage = "select * from test;";
            const int imageDimension = 100;
            var image = new Bitmap(imageDimension, imageDimension);
            byte[] binary;

            using (var g = Graphics.FromImage(image))
            {
                g.Clear(Color.Gold);
            }

            using (var memory = new MemoryStream())
            {
                image.Save(memory, ImageFormat.Png);
                binary = memory.ToArray();
            }

            var xQuery = new XQuerySqlite(SetUp.SqliteConnectionString);
            xQuery.BeginTransaction();
            xQuery.Update(sqlCreateTable);
            xQuery.InsertBinaryIntoCell(binary, sqlInsertImage, "@bin");
            var retrivedImage = xQuery.SelectBinaryAsImage(sqlSelectImage);
            Assert.AreEqual(100, retrivedImage.Size.Width);
        }

        [Test]
        public void TestSelectBinaryDataFromCellAndSaveToFile()
        {
            const string sqlCreateTable = "create table test (bin blob);";
            const string sqlInsertImage = "insert into test (bin) values (@bin);";
            const string sqlSelectImage = "select bin from test limit 1;";
            const string fileName = "image.png";
            const int imageDimension = 100;
            var image = new Bitmap(imageDimension, imageDimension);
            byte[] binary;

            using (var g = Graphics.FromImage(image))
            {
                g.Clear(Color.Gold);
            }

            using (var memory = new MemoryStream())
            {
                image.Save(memory, ImageFormat.Png);
                binary = memory.ToArray();
            }

            var xQuery = new XQuerySqlite(SetUp.SqliteConnectionString);
            xQuery.BeginTransaction();
            xQuery.Update(sqlCreateTable);
            xQuery.InsertBinaryIntoCell(binary, sqlInsertImage, "@bin");
            File.Delete(fileName);
            var result = xQuery.SelectBinaryAndSave(fileName, sqlSelectImage);
            Assert.IsTrue(result);
        }
    }
}
