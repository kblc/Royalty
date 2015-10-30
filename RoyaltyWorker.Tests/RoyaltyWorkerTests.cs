﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoyaltyRepository;
using RoyaltyFileStorage;
using Helpers.CSV;
using System.Linq;

namespace RoyaltyWorker.Tests
{
    [TestClass]
    public class RoyaltyWorkerTests
    {
        private const string defAccountName = "default3";
        public Repository Rep { get; set; }

        public bool SqlLogEnabled { get; set; }

        [TestInitialize]
        public void Initialization()
        {
            SqlLogEnabled = true;
            Rep = GetNewRepository();
            Rep.AccountRemove(Rep.AccountGet(defAccountName, true));
            Rep.AccountAdd(Rep.AccountNew(byDefault: true, accountName: defAccountName));
            Console.WriteLine("############################## Initialization done");
        }

        private Repository GetNewRepository()
        {
            var res = new Repository("connectionStringHome");
            res.Log = (s) => { if (SqlLogEnabled) Console.WriteLine(string.Format("{0}", s)); };
            return res;
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
        public void RoyaltyWorker_Watcher()
        {
            SqlLogEnabled = false;
            try
            {
                var storage = new FileStorage();
                storage.Log += (s, e) => Console.WriteLine("[FILESTORAGE] {0}", e);

                var storeFolder = "D:\\filestorage.import.main";

                var a = Rep.AccountGet(defAccountName, eagerLoad: new string[] { "Settings" });
                Rep.AccountSettingsImportDirectoryNew(a.Settings, new { Path = storeFolder, Filter = "*.csv" });

                var dtStart = DateTime.UtcNow;
                var dtCurrent = dtStart;
                var addTimeSpan = new TimeSpan(0, 0, 1);
                while ((dtCurrent - dtStart).TotalMinutes < 1)
                {
                    dtCurrent += addTimeSpan;
                    Rep.AccountSettingsSheduleTimeNew(a.Settings, new TimeSpan(dtCurrent.Hour, dtCurrent.Minute, dtCurrent.Second));
                }
                Rep.SaveChanges();

                var itemsInQueueBeforeTest = a.ImportQueue.Count;

                using (var w = new RoyaltyWatcher(() => GetNewRepository(), storage) { CheckTimerInterval = addTimeSpan.Add(addTimeSpan) })
                {
                    w.OnQueueRecordAdded += (s, e) => Console.WriteLine("[WORKER:RECORD_ADDED] {0}", e);
                    w.Log += (s, e) => Console.WriteLine("[WORKER] {0}", e);
                    var lines = new string[] { "Column0;Column1;Column2", "data0;data1;data2" };
                    System.IO.File.WriteAllLines(System.IO.Path.Combine(storeFolder, "test.csv"), lines);
                    System.Threading.Thread.Sleep(5000);
                    while (w.IsBusy)
                        System.Threading.Thread.Sleep(200);
                }

                var newRecordCount = Rep.ImportQueueRecordGet().Where(i => i.Account.AccountUID == a.AccountUID).Count();
                Assert.AreNotEqual(itemsInQueueBeforeTest, newRecordCount, "Count must not equals");
            }
            finally
            {
                SqlLogEnabled = true;
            }
        }
    }
}