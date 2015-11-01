using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoyaltyRepository;
using RoyaltyFileStorage;
using Helpers.CSV;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;
using Helpers.Linq;
using System.Text;

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
            res.Log = (s) => { if (SqlLogEnabled)
                {
                    //Debug.WriteLine(string.Format("{0}", s));
                    Console.WriteLine(string.Format("{0}", s));
                } };
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
                var f = Rep.AccountSettingsImportDirectoryNew(a.Settings, new { Path = storeFolder, Filter = "*.csv", ForAnalize = true, DeleteFileAfterImport = true });
                Rep.SaveChanges();

                var itemsInQueueBeforeTest = a.ImportQueue.Count;

                using (var watcher = new RoyaltyWatcher(GetNewRepository, storage))
                {
                    watcher.OnQueueRecordAdded += (s, e) => Console.WriteLine("[WATCHER:RECORD_ADDED] {0}", e);
                    watcher.Log += (s, e) => Console.WriteLine("[WATCHER] {0}", e);
                    System.IO.File.WriteAllLines(System.IO.Path.Combine(storeFolder, "test.csv"), GenerateCSVData(100));
                    watcher.RunOnce();
                }

                var newRecordCount = Rep.ImportQueueRecordGet().Where(i => i.Account.AccountUID == a.AccountUID).Count();
                Assert.AreNotEqual(itemsInQueueBeforeTest, newRecordCount, "Count must not equals");
            }
            finally
            {
                SqlLogEnabled = true;
            }
        }

        [TestMethod]
        public void RoyaltyWorker_Worker()
        {
            SqlLogEnabled = false;
            try
            {
                var storeFolder = "D:\\filestorage.import.main";
                var exportFolder = "D:\\export";

                var storage = new FileStorage();
                storage.Log += (s, e) => Console.WriteLine("[FILESTORAGE] {0}", e);

                var a = Rep.AccountGet(defAccountName, eagerLoad: new string[] { "Settings" });
                a.Settings.IgnoreExportTime = TimeSpan.FromTicks(0);
                var f = Rep.AccountSettingsImportDirectoryNew(a.Settings, new { Path = storeFolder, Filter = "*.csv", ForAnalize = true, DeleteFileAfterImport = true, Encoding = Encoding.UTF8 });
                var ef = Rep.AccountSettingsExportDirectoryNew(a.Settings, new { DirectoryPath = exportFolder, Encoding = Encoding.UTF8 });
                Rep.SaveChanges();

                using (var watcher = new RoyaltyWatcher(() => GetNewRepository(), storage))
                {
                    watcher.OnQueueRecordAdded += (s, e) => Console.WriteLine("[WATCHER:RECORD_ADDED] {0}", e);
                    watcher.Log += (s, e) => Console.WriteLine("[WATCHER:LOG] {0}", e);
                    System.IO.File.WriteAllLines(System.IO.Path.Combine(storeFolder, "test1.csv"), GenerateCSVData(100), f.Encoding);
                    System.IO.File.WriteAllLines(System.IO.Path.Combine(storeFolder, "test2.csv"), GenerateCSVData(100), f.Encoding);
                    System.IO.File.WriteAllLines(System.IO.Path.Combine(storeFolder, "test3.csv"), GenerateCSVData(100), f.Encoding);
                    watcher.RunOnce();
                }

                bool workerStarted = false;
                using (var worker = new RoyaltyWorker(() => GetNewRepository(), storage))
                {
                    worker.Log += (s, e) => Console.WriteLine("[WORKER:LOG] {0}", e);
                    worker.WorkerStateChanged += (s, e) =>
                    {
                        workerStarted = true;
                        Console.WriteLine("[WORKER:{0}] progress: {1}", e.Action, e.Element.Progress);
                    };
                    worker.RunOnce();
                }

                Assert.AreEqual(true, workerStarted, "Worker must started");
            }
            finally
            {
                SqlLogEnabled = true;
            }
        }

        private IEnumerable<string> GenerateCSVData(int maxCnt)
        {
            var rnd = new Random();
            rnd.Next();

            var alph = "йцукенгшщзфывапролдячсмить";

            var cityNames = Enumerable.Range(0, 3).Select(i => $"TestCity{i}").ToArray();
            var areaNames = Enumerable.Range(0, 10).Select(i => $"TestArea{i}").ToArray();
            var streetNames = Enumerable.Range(0, 100).Select(i => Enumerable.Range(0, 6).Select(n => alph[rnd.Next(alph.Length)].ToString()).Concat(c => c)).ToArray();
            var hostNames = new string[] { "testhost0.ru", "testhost1.ru" };

            var acc = Rep.AccountGet(defAccountName, eagerLoad: new string[] { "Settings.Columns.ColumnType" });

            var addrCol = acc.Settings.GetColumnFor(RoyaltyRepository.Models.ColumnTypes.Address);
            var areaCol = acc.Settings.GetColumnFor(RoyaltyRepository.Models.ColumnTypes.Area);
            var cityCol = acc.Settings.GetColumnFor(RoyaltyRepository.Models.ColumnTypes.City);
            var cityHost = acc.Settings.GetColumnFor(RoyaltyRepository.Models.ColumnTypes.Host);
            var cityPhone = acc.Settings.GetColumnFor(RoyaltyRepository.Models.ColumnTypes.Phone);

            var csvLines = new List<string>();

            var colValues = acc.Settings.Columns
                .Where(c => c.ColumnType.ImportTableValidation)
                .Select(c => new { Name = c.ColumnName.ToLower(), Type = c.ColumnType })
                .ToArray();

            var columns = string.Empty;
            foreach (var colName in colValues)
                columns += (string.IsNullOrWhiteSpace(columns) ? string.Empty : ";") + colName.Name;
            csvLines.Add(columns);

            var lastAddedPhone = string.Empty;
            var genNewData = new Func<RoyaltyRepository.Models.ColumnTypes, string>(
                (ct) =>
                {
                    var res = string.Empty;
                    switch (ct)
                    {
                        case RoyaltyRepository.Models.ColumnTypes.Phone:
                            if (!string.IsNullOrWhiteSpace(lastAddedPhone) && rnd.Next(0, 5) == 0)
                                res = lastAddedPhone;
                            else
                                for (int n = 0; n < 5; n++)
                                    res += rnd.Next(10, 99).ToString();
                            break;
                        case RoyaltyRepository.Models.ColumnTypes.Address:
                            res = streetNames.Union(new string[] { "defaultPreviewStreet2" }).ToArray()[rnd.Next(0, streetNames.Length + 1)]
                                + ((rnd.Next(0, 5) != 0) ? ", " + rnd.Next(1, 99) : string.Empty);
                            break;
                        case RoyaltyRepository.Models.ColumnTypes.Area:
                            res = areaNames.Union(new string[] { string.Empty, "defaultPreviewArea2" }).ToArray()[rnd.Next(0, areaNames.Length + 2)];
                            break;
                        case RoyaltyRepository.Models.ColumnTypes.City:
                            res = cityNames.Union(new string[] { "defaultPreviewCity1" }).ToArray()[rnd.Next(0, cityNames.Length + 1)];
                            break;
                        case RoyaltyRepository.Models.ColumnTypes.Host:
                            res = (rnd.Next(3) == 0 ? string.Empty : @"http://")
                                + hostNames.Union(new string[] { "testhost2.ru" }).ToArray()[rnd.Next(0, hostNames.Length + 1)]
                                + "/" + Guid.NewGuid().ToString("N");
                            break;
                    }
                    return res;
                }
                );

            for (int i = 0; i < maxCnt; i++)
            {
                columns = string.Empty;
                foreach (var colName in colValues)
                    columns += (string.IsNullOrWhiteSpace(columns) ? string.Empty : ";") + genNewData(colName.Type.Type);
                csvLines.Add(columns);
            }

            return csvLines;
        }
    }
}
