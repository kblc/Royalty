using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoyaltyRepository;

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
            Rep = new Repository("connectionString");
            Rep.Log = (s) => { Console.WriteLine(string.Format("[~] SQL: {0}", s)); };
            Rep.AccountRemove(Rep.AccountGet(defAccountName, true));
            Rep.AccountAdd(Rep.AccountNew(byDefault: true, accountName: defAccountName));
            Console.WriteLine("############################## Initialization done");
        }

        [TestCleanup]
        public void Finalization()
        {
            Rep.AccountRemove(Rep.AccountGet(defAccountName));
            Rep.Log = null;
            Rep.Dispose();
            Rep = null;
            Console.WriteLine("############################## Finalization done");
        }

        [TestMethod]
        public void City_And_Area_Insert_Remove()
        {
            var defCityName = "newCityName";
            var defAreaName = "newAreaName";

            var cD = Rep.CityGet(defCityName);
            Rep.CityRemove(cD);
            var aD = Rep.AreaGet(defAreaName);
            Rep.AreaRemove(aD);

            var cCnt = Rep.CityGet().Count();
            var c = Rep.CityNew(defCityName);
            Rep.CityAdd(c);
            Assert.AreEqual(cCnt + 1, Rep.CityGet().Count(), "City count must increase by 1");

            var aCnt = Rep.AreaGet().Count();
            var a = Rep.AreaNew(defAreaName, c);
            Rep.AreaAdd(a);
            Assert.AreEqual(aCnt + 1, Rep.AreaGet().Count(), "Area count must increase by 1");

            c.UndefinedArea = a;
            Rep.SaveChanges();

            Rep.AreaRemove(a);
            Assert.AreEqual(aCnt, Rep.AreaGet().Count(), "Area count must decrease by 1");

            Assert.AreEqual(null, c.UndefinedArea, "Default area for city must be NULL (we just delete it)");

            Rep.CityRemove(c);
            Assert.AreEqual(cCnt, Rep.CityGet().Count(), "City count must decrease by 1");

            Assert.AreEqual(aCnt - 1, Rep.AreaGet().Count(), "Area count must decrease by 1 because we delete city with 1 default area");
        }

        [TestMethod]
        public void Mark_Select()
        {
            var n = Rep.MarkGet().Count();
            Assert.AreNotEqual(0, n, "Mark count should be more then 0");
        }

        [TestMethod]
        public void Phone_Insert_Remove()
        {
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
            var hCnt = Rep.HostGet().Count();
            var h = Rep.HostNew("test0.host.com");
            Rep.HostAdd(h);
            Assert.AreEqual(hCnt + 1, Rep.HostGet().Count(), "Host count must be increase by 1");
            Rep.HostRemove(h);
            Assert.AreEqual(hCnt, Rep.HostGet().Count(), "Host count must be decrease by 1");
        }

        [TestMethod]
        public void Account_State()
        {
            var acc = Rep.AccountGet(defAccountName);
            acc.State.IsActive = true;
            acc.State.LastBatch = DateTime.Now;
            Rep.SaveChanges();
        }

        [TestMethod]
        public void Account_Settings()
        {
            var acc = Rep.AccountGet(defAccountName);
            acc.Settings.DeleteFileAfterImport = false;
            acc.Settings.ExecuteAfterAnalizeCommand = "C:\abc.bat";
            acc.Settings.TimeForTrust = TimeSpan.FromDays(30);
            Rep.SaveChanges();
        }

        [TestMethod]
        public void Account_Settings_Shedule()
        {
            var acc = Rep.AccountGet(defAccountName);
            var cShd = acc.Settings.SheduleTimes.Count;
            Rep.AccountSettingsSheduleTimeNew(acc.Settings, new TimeSpan(09, 00, 00));
            Rep.AccountSettingsSheduleTimeNew(acc.Settings, new TimeSpan(12, 00, 00));
            Rep.AccountSettingsSheduleTimeNew(acc.Settings, new TimeSpan(18, 00, 00));
            Assert.AreEqual(cShd + 3, acc.Settings.SheduleTimes.Count, "Shedule count must be increase by 3");
            var st = acc.Settings.SheduleTimes.Last();
            Rep.AccountSettingsSheduleTimeRemove(st);
            Assert.AreEqual(cShd + 2, acc.Settings.SheduleTimes.Count, "Shedule count must be decrease by 1");
        }
    }
}
