using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RoyaltyFileStorage.Tests
{
    [TestClass]
    public class RoyaltyFileStorageTest
    {
        private FileStorage storage = null;

        [TestInitialize]
        public void Initialize()
        {
            storage = new FileStorage();
        }

        [TestMethod]
        public void RoyaltyFileStorage_ReadConfig()
        {
            Assert.AreNotEqual("", storage.Location, "Location must be not empty, becase it was readed from app.config");
        }

        [TestMethod]
        public void RoyaltyFileStorage_PutGetDeleteFile()
        {
            var fileId = Guid.NewGuid();
            var fileName = "test.jpg";
            var fileName2 = "test2.jpg";

            using (var fileStream = System.IO.File.OpenRead(fileName))
            { 
                var info = storage.FilePut(fileId, fileStream, fileName);
                Console.WriteLine("File stored in: {0}", info.FullName);
                try
                {
                    Assert.AreNotEqual(0, info.Length, "File length must be more then 0");
                    using (var res = storage.FileGet(fileId))
                    using (var fs = System.IO.File.OpenWrite(fileName2))
                    {
                        res.CopyTo(fs);
                    }
                    var info2 = new System.IO.FileInfo(fileName2);
                    Assert.AreEqual(info.Length, info2.Length, "Length must equals");
                    System.IO.File.Delete(fileName2);
                }
                finally
                {
                    storage.FileDelete(fileId);
                }
            }
        }
    }
}
