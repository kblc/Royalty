using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoyaltyRepository;
using System.Collections.Generic;

namespace RoyaltyRepositoryTests
{
    [TestClass]
    public class RepositoryTest
    {
        private const string defAccountName = "default0";
        public Repository Rep { get; set; }

        [TestInitialize]
        public void Initialization()
        {
            Rep = new Repository("connectionStringHome");
            Rep.AccountRemove(Rep.AccountGet(defAccountName, true));
            Rep.AccountAdd(Rep.AccountNew(byDefault: true, accountName: defAccountName));
            Console.WriteLine("############################## Initialization done");
            Rep.Log = (s) => { Console.WriteLine(string.Format("[~] SQL: {0}", s)); };
        }

        [TestCleanup]
        public void Finalization()
        {
            Rep.Log = null;
            Rep.AccountRemove(Rep.AccountGet(defAccountName));
            Rep.Dispose();
            Rep = null;
            Console.WriteLine("############################## Finalization done");
        }

        [TestMethod]
        public void City_And_Area_Insert_Remove()
        {
            var defCityName = "newCityName";
            var defAreaName = "newAreaName";

            Rep.CityRemove(Rep.CityGet(defCityName));

            var cCnt = Rep.CityGet().Count();
            var c = Rep.CityNew(defCityName);
            Rep.CityAdd(c);
            Assert.AreEqual(cCnt + 1, Rep.CityGet().Count(), "City count must increase by 1");

            var aCnt = Rep.AreaGet().Count();
            var a = Rep.AreaNew(defAreaName, city: c);
            Rep.AreaAdd(a);
            Assert.AreEqual(aCnt + 1, Rep.AreaGet().Count(), "Area count must increase by 1");

            Rep.AreaRemove(a);
            Assert.AreEqual(aCnt, Rep.AreaGet().Count(), "Area count must decrease by 1");

            Assert.AreEqual(0, c.Areas.Where(ar => ar.IsDefault).Count(), "Default area for city must be NULL (we just delete it)");

            a = c.UndefinedArea;
            Rep.SaveChanges();

            Assert.AreEqual(aCnt + 1, Rep.AreaGet().Count(), "Area count must increase by 1 cause we create default area");

            Rep.CityRemove(c);
            Assert.AreEqual(cCnt, Rep.CityGet().Count(), "City count must decrease by 1");
        }

        [TestMethod]
        public void City_InTransaction()
        {
            var defCityName = "newCityName2";

            Rep.CityRemove(Rep.CityGet(defCityName));
            using (Rep.BeginTransaction())
            {
                Rep.CityAdd(Rep.CityNew(defCityName));
            }
            var c = Rep.CityGet(defCityName);
            Assert.AreEqual(null, c, "City must not exists");

            using (Rep.BeginTransaction(commitOnDispose: true))
            {
                Rep.CityAdd(Rep.CityNew(defCityName));
            }
            var c2 = Rep.CityGet(defCityName);
            Assert.AreNotEqual(null, c2, "City must exists");
            Rep.CityRemove(c2);
        }

        [TestMethod]
        public void Mark_Select()
        {
            var n = Rep.MarkGet().Count();
            Assert.AreNotEqual(0, n, "Mark count should be more then 0");
        }

        [TestMethod]
        public void Mark_Select2()
        {
            var n = Rep.MarkGet().Count();
            Assert.AreNotEqual(0, n, "Mark count should be more then 0");
        }

        [TestMethod]
        public void AccountExportTypes_Insert_Remove()
        {
            var acc = Rep.AccountGet(defAccountName);
            var hPhn = acc.ExportTypes.Count;
            var mark = Rep.MarkGet().First();

            var p = Rep.AccountExportTypeNew(acc, "test.csv", mark);
            Rep.SaveChanges();

            Assert.AreEqual(hPhn + 1, acc.ExportTypes.Count, "AccountExportType count must be increase by 1");
            Rep.AccountExportTypeRemove(p);

            Assert.AreEqual(hPhn, acc.ExportTypes.Count, "AccountExportType count must be decrease by 1");
        }

        [TestMethod]
        public void AccountDataRecord_Insert_Remove()
        {
            var acc = Rep.AccountGet(defAccountName, eagerLoad: new string[] { "Data" });
            var hPhn = acc.Data.Count;

            var ph = Rep.PhoneGet("00-000-000-0000") ?? Rep.PhoneNew("00-000-000-0000");
            var h = Rep.HostGet("test0.host.com") ?? Rep.HostNew("test0.host.com");
            var c = Rep.CityGet("new test city") ?? Rep.CityNew("new test city");
            var a = Rep.AreaGet("new area test", c) ?? Rep.AreaNew("new area test", city: c);
            var s = Rep.StreetGet("test street", a) ?? Rep.StreetNew("test street", a);
            var m = Rep.MarkGet().First();

            var p = Rep.AccountDataRecordNew(anonymousFiller: new
            {
                Account = acc,
                Phone = ph,
                Street = s,
                HouseNumber = (string)null,
                Host = h,
                Mark = m
            });

            p.DataAdditional = new RoyaltyRepository.Models.AccountDataRecordAdditional() { AccountDataRecord = p, Column00 = "test" };

            Rep.SaveChanges();

            Assert.AreEqual(hPhn + 1, acc.Data.Count, "AccountDataRecord count must be increase by 1");

            Rep.AccountDataRecordRemove(p, saveAfterRemove: false);
            Rep.PhoneRemove(ph, saveAfterRemove: false);
            Rep.HostRemove(h, saveAfterRemove: false);
            Rep.AreaRemove(a, saveAfterRemove: false);
            Rep.CityRemove(c, saveAfterRemove: false);

            Rep.SaveChanges();

            Assert.AreEqual(hPhn, acc.Data.Count, "AccountDataRecord count must be decrease by 1");
        }

        [TestMethod]
        public void Phone_Insert_Remove()
        {
            Rep.PhoneRemove(Rep.PhoneGet("00-000-000-0000"));
            var hPhn = Rep.PhoneGet().Count();
            var p = Rep.PhoneNew("00-000-000-0000");
            Rep.PhoneAdd(p);
            Assert.AreEqual(hPhn + 1, Rep.PhoneGet().Count(), "Phones count must be increase by 1");
            Rep.PhoneRemove(p);
            Assert.AreEqual(hPhn, Rep.PhoneGet().Count(), "Phones count must be decrease by 1");
        }

        [TestMethod]
        public void Host_Insert_Remove()
        {
            Rep.HostRemove(Rep.HostGet("test0.host.com"));

            var hCnt = Rep.HostGet().Count();
            var h = Rep.HostNew("test0.host.com");
            Rep.HostAdd(h);
            Assert.AreEqual(hCnt + 1, Rep.HostGet().Count(), "Host count must be increase by 1");
            Rep.HostRemove(h);
            Assert.AreEqual(hCnt, Rep.HostGet().Count(), "Host count must be decrease by 1");
        }

        [TestMethod]
        public void File_Insert_Remove()
        {
            var hCnt = Rep.FileGet().Count();
            var h = Rep.FileNew();
            h.FileName = "test";
            h.FilePath = @"c:\test";
            h.FileSize = 0;
            h.MimeType = @"csv/plain";
            Rep.FileAdd(h);
            Assert.AreEqual(hCnt + 1, Rep.FileGet().Count(), "File count must be increase by 1");
            Rep.FileRemove(h);
            Assert.AreEqual(hCnt, Rep.FileGet().Count(), "File count must be decrease by 1");
        }

        [TestMethod]
        public void Message_Insert_Remove()
        {
            var hCnt = Rep.MessageGet().Count();
            var fCnt = Rep.FileGet().Count();
            var h = Rep.MessageNew("test message");

            var f = Rep.FileNew(new
            {
                FileName = "test.csv",
                FilePath = @"c:\test.csv",
                MimeType = @"csv/plain"
            });

            h.Files.Add(f);

            Rep.MessageAdd(h);
            Assert.AreEqual(hCnt + 1, Rep.MessageGet().Count(), "Message count must be increase by 1");
            Assert.AreEqual(fCnt + 1, Rep.FileGet().Count(), "File count must be increase by 1");
            Rep.MessageRemove(h);
            Assert.AreEqual(hCnt, Rep.MessageGet().Count(), "Message count must be decrease by 1");
            Assert.AreEqual(fCnt, Rep.FileGet().Count(), "File count must be decrease by 1");
        }

        [TestMethod]
        public void Account_State()
        {
            var acc = Rep.AccountGet(defAccountName, eagerLoad: new string[] { "State" });
            acc.State.IsActive = true;
            acc.State.LastBatch = DateTime.Now;
            Rep.SaveChanges();
        }

        [TestMethod]
        public void Account_Settings()
        {
            var acc = Rep.AccountGet(defAccountName, eagerLoad: new string[] { "Settings" });
            acc.Settings.DeleteFileAfterImport = false;
            acc.Settings.ExecuteAfterAnalizeCommand = "C:\abc.bat";
            acc.Settings.TimeForTrust = TimeSpan.FromDays(30);
            Rep.SaveChanges();
        }

        [TestMethod]
        public void Account_Settings_Shedule()
        {
            var acc = Rep.AccountGet(defAccountName, eagerLoad: new string[] { "Settings", "Settings.SheduleTimes" });
            var cShd = acc.Settings.SheduleTimes.Count;
            Rep.AccountSettingsSheduleTimeNew(acc.Settings, new TimeSpan(09, 00, 00));
            Rep.AccountSettingsSheduleTimeNew(acc.Settings, new TimeSpan(12, 00, 00));
            Rep.AccountSettingsSheduleTimeNew(acc.Settings, new TimeSpan(18, 00, 00));
            Rep.SaveChanges();
            Assert.AreEqual(cShd + 3, acc.Settings.SheduleTimes.Count, "Shedule count must be increase by 3");
            var st = acc.Settings.SheduleTimes.Last();
            Rep.AccountSettingsSheduleTimeRemove(st);
            Assert.AreEqual(cShd + 2, acc.Settings.SheduleTimes.Count, "Shedule count must be decrease by 1");
        }

        [TestMethod]
        public void Host_AddBulk()
        {
            var rnd = new Random();
            var str = "qwertyuiopasdfghjklzxcvbnm";
            int cnt = 3000;

            var names = new List<string>();
            for (int i = 0; i < cnt; i++)
            {
                var name = string.Empty;
                for (int n = 0; n < rnd.Next(4, 10); n++)
                    name += str[rnd.Next(str.Length - 1)];
                while(names.Contains(name))
                    name += str[rnd.Next(str.Length - 1)];
                names.Add(name);
            }

            using(var logSession = Helpers.Log.Session(isEnabled: false, output: (ss) => { ss.ToList().ForEach(s => Console.WriteLine(s)); }))
                try
                {
                    var itemsToDelete = Rep.HostGet(names).ToList();
                    Rep.HostRemoveBulk(itemsToDelete);

                    var oldCnt = Rep.HostGet().Count();

                    var items = names.Select(n => Rep.HostNew(n)).ToArray();
                    Rep.HostAddBulk(items);
                    Assert.AreEqual(oldCnt + names.Count(), Rep.HostGet().Count(), "Count must equals");

                    itemsToDelete = Rep.HostGet(names).ToList();
                    Rep.HostRemoveBulk(itemsToDelete);
                    Assert.AreEqual(oldCnt, Rep.HostGet().Count(), "Count must equals");
                }
                catch(Exception ex)
                {
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    throw ex;
                }
        }
    }
}
