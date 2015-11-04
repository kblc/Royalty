using System;
using RoyaltyService.Tests.FileServiceReference;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RoyaltyService.Tests
{
    [TestClass]
    public class RoyaltyServiceTests
    {
        [TestMethod]
        public void FileService_Test()
        {
            //var fileBody = "testBody";
            var id = "60e0a01b-dc2f-478d-9d80-fdb6b16e84d2".Replace("-","");

            using(var clnt = new FileServiceClient())
            {
                var res = clnt.Get(id);
                if (!string.IsNullOrWhiteSpace(res.Error))
                    throw new Exception(res.Error);

                Assert.AreEqual(id, res.Value.FileID);

                clnt.ChangeLanguage("en-US");
                var badRes = clnt.Get("123");
                Assert.AreNotEqual(true, string.IsNullOrWhiteSpace(badRes.Error));
            }
        }
    }
}
