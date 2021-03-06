﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoyaltyDataCalculator;
using RoyaltyRepository;
using RoyaltyRepository.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Helpers;
using Helpers.Linq;
using System.Data;

namespace RoyaltyDataCalculatorTest
{
    [TestClass]
    public class DataCalculatorTest
    {
        private const string defAccountName = ".DataCalculatorTest";
        public Repository Rep { get; set; }

        public bool SqlLogEnabled { get; set; }

        [TestInitialize]
        public void Initialization()
        {
            SqlLogEnabled = true;
            Rep = new Repository("connectionStringHome");
            Rep.Remove(Rep.GetAccount(defAccountName, true));
            Rep.Add(Rep.NewAccount(byDefault: true, accountName: defAccountName));
            Rep.SqlLog += (s,e) => { if (SqlLogEnabled) Console.WriteLine(string.Format("{0}", e)); };
            Rep.Log += (s,e) => Console.WriteLine(string.Format("{0}", e));
            Console.WriteLine("############################## Initialization done");
        }

        [TestCleanup]
        public void Finalization()
        {
            Rep.Remove(Rep.GetAccount(defAccountName));
            Rep.Dispose();
            Rep = null;
            Console.WriteLine("############################## Finalization done");
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void DataCalculator_Preview_ExceptOnTableValidation()
        {
            var a = Rep.GetAccount(defAccountName, eagerLoad: new string[] { "Settings.Columns" });
            if (a != null)
                using (var dc = new DataCalculator(a, Rep))
                {
                    var columns = string.Empty;
                    var colValues = a.Settings.Columns
                        .Where(c => c.ColumnType.ImportTableValidation)
                        .Select(c => c.ColumnName.ToLower())
                        .ToArray()
                        .Skip(1); //Skip one required row for take exception

                    foreach (var colName in colValues)
                        columns += (string.IsNullOrWhiteSpace(columns) ? string.Empty : ";") + colName;

                    var csvLines = new List<string>();
                    csvLines.Add(columns);

                    try
                    {
                        var l = Helpers.CSV.CSVFile.Load(csvLines.AsEnumerable<string>(),
                            tableName: "{virtual}",
                            filePath: "{virtual}",
                            tableValidator: dc.TableValidator,
                            rowFilter: dc.RowFilter);
                    }
                    catch(Exception ex)
                    {
                        var exText = ex.GetExceptionText();
                        Console.WriteLine(exText);
                        Assert.AreEqual(true, exText.Contains(colValues.First()), "Exception must contains required column name");
                        throw;
                    }
                }
        }

        [TestMethod]
        public void DataCalculator_Preview_RowFilterAll()
        {
            var a = Rep.GetAccount(defAccountName);
            if (a != null)
                using (var dc = new DataCalculator(a, Rep))
                {
                    var columns = string.Empty;
                    var colValues = a.Settings.Columns
                        .Where(c => c.ColumnType.ImportTableValidation)
                        .Select(c => c.ColumnName.ToLower())
                        .ToArray();

                    var csvLines = new List<string>();

                    foreach (var colName in colValues)
                        columns += (string.IsNullOrWhiteSpace(columns) ? string.Empty : ";") + colName;
                    csvLines.Add(columns);

                    var cnt = 100000;
                    var goodCnt = 0;
                    for(int i=0; i< cnt; i++)
                    {
                        columns = string.Empty;
                        foreach (var colName in colValues.Skip(1))
                            columns += (string.IsNullOrWhiteSpace(columns) ? string.Empty : ";") + string.Format("data_{0}", i);

                        if (i % 2 == 0)
                        {
                            columns += ";bad_data";
                        } else
                        {
                            columns += ";good_data";
                            goodCnt++;
                        }
                        csvLines.Add(columns);
                    }

                    var l = Helpers.CSV.CSVFile.Load(csvLines, 
                        tableName: "{virtual}",
                        filePath: "{virtual}",
                        tableValidator: dc.TableValidator,
                        verboseLogAction: (s) =>
                        {
                            Console.WriteLine(s);
                            if (s.Contains("good_data"))
                                throw new Exception("Good data filtered: " + s);
                        },
                        rowFilter: r => r.Table.Columns.OfType<DataColumn>().Any(c => r[c].ToString().Contains("bad_data")) 
                        );

                    Assert.AreEqual(goodCnt, l.Table.Rows.Count, "Good row count must equals");
                }
        }

        [TestMethod]
        public void DataCalculator_Preview()
        {
            SqlLogEnabled = false;

            var rnd = new Random();
            rnd.Next();

            var alph = "йцукенгшщзфывапролдячсмить";

            var maxCnt = 100;
            var cityNames = Enumerable.Range(0, 3).Select(i => $"TestCity{i}").ToArray();
            var areaNames = Enumerable.Range(0, 10).Select(i => $"TestArea{i}").ToArray();
            var streetNames = Enumerable.Range(0, 100).Select(i => Enumerable.Range(0, 6).Select(n => alph[rnd.Next(alph.Length)].ToString()).Concat(c => c)).ToArray();
            var hostNames = new string[] { "testhost0.ru", "testhost1.ru" };

            try
            {
                hostNames.ToList().ForEach((h) =>
                    {
                        Rep.HostRemove(Rep.HostGet(h), saveAfterRemove: false);
                        var host = Rep.HostNew(h);
                        Rep.HostAdd(host, saveAfterInsert: false);
                    });
                Rep.SaveChanges();

                var acc = Rep.GetAccount(defAccountName, eagerLoad: new string[] { "Settings.Columns.ColumnType" });

                cityNames.ToList().ForEach((s) =>
                {
                    Rep.CityRemove(Rep.CityGet(s), saveAfterRemove: false);
                });
                Rep.SaveChanges();

                cityNames.ToList().ForEach((s) =>
                {
                    var city = Rep.CityNew(s);
                    areaNames.ToList().ForEach((a) =>
                    {
                        var area = Rep.AreaNew(a, city: city);
                        streetNames.ToList().ForEach((ss) =>
                        {
                            if (rnd.Next(5) != 0)
                                Rep.StreetNew(RoyaltyDataCalculator.Parser.Address.FromString(ss, area.Name, acc.Dictionary.Excludes.Select(e => e.Exclude)).Street, area);
                        });
                    });
                });

                Rep.SaveChanges();

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
                        switch(ct)
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
                                    + ((rnd.Next(0, 5) != 0) ? ", " + rnd.Next(1,99) : string.Empty);
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

                for (int i = 0; i < maxCnt; i++ )
                {
                    columns = string.Empty;
                    foreach (var colName in colValues)
                        columns += (string.IsNullOrWhiteSpace(columns) ? string.Empty : ";") + genNewData(colName.Type.Type);
                    csvLines.Add(columns);
                }

                Rep.SaveChanges();

                using (var dc = new DataCalculator(acc, Rep))
                {
                    var prg0 = new Helpers.PercentageProgress();
                    var prg1 = prg0.GetChild();
                    var prg2 = prg0.GetChild();
                    var prg3 = prg0.GetChild();
                    prg0.Change += (s, e) => Helpers.Log.Add($"Progress: {e.Value.ToString("N2")}");
                    var log = new Action<string>((s) => Helpers.Log.Add(s));

                    var l = Helpers.CSV.CSVFile.Load(csvLines.AsEnumerable(),
                        tableValidator: dc.TableValidator,
                        rowFilter: dc.RowFilter);

                    dc.Prepare(l.Table, i => prg1.Value = i, log);
                    var previewRes = dc.Preview(l.Table, i => prg2.Value = i, log);
                    Assert.AreEqual(l.Table.Rows.Count, previewRes.Count());

                    var readyToExport = dc.Import(previewRes.Values, null, i => prg3.Value = i, log);
                    Rep.SaveChanges();

                    Helpers.Log.Add($"Data imported and ready to export");

                    Rep.AccountDataRecordRemove(acc.Data);
                }
            }
            finally
            {
                hostNames.ToList().ForEach((s) =>
                {
                    Rep.HostRemove(Rep.HostGet(s), saveAfterRemove: false);
                });
                cityNames.ToList().ForEach((s) =>
                {
                    Rep.CityRemove(Rep.CityGet(s), saveAfterRemove: false);
                });
                Rep.SaveChanges();
                SqlLogEnabled = true;
            }
        }

        [TestMethod]
        public void DataCalculator_Prepare()
        {
            SqlLogEnabled = false;
            try
            {
                var acc = Rep.GetAccount(defAccountName, eagerLoad: new string[] { "Settings.Columns.ColumnType" });
                var csvLines = new List<string>();

                var colValues = acc.Settings.Columns
                    .Where(c => c.ColumnType.ImportTableValidation)
                    .Select(c => new { Name = c.ColumnName.ToLower(), Type = c.ColumnType })
                    .ToArray();

                var columns = string.Empty;
                foreach (var colName in colValues)
                    columns += (string.IsNullOrWhiteSpace(columns) ? string.Empty : ";") + colName.Name;

                columns += ";test_column";

                csvLines.Add(columns);

                using (var dc = new DataCalculator(acc, Rep))
                {
                    var prg0 = new Helpers.PercentageProgress();
                    prg0.Change += (s, e) => Helpers.Log.Add($"Progress: {e.Value.ToString("N2")}");
                    var log = new Action<string>((s) => Helpers.Log.Add(s));

                    var l = Helpers.CSV.CSVFile.Load(csvLines.AsEnumerable(),
                        tableValidator: dc.TableValidator,
                        rowFilter: dc.RowFilter);

                    dc.Prepare(l.Table, i => prg0.Value = i, log);
                    Assert.AreEqual(true, acc.AdditionalColumns.Any(ad => ad.ColumnName == "test_column"));
                    Rep.SaveChanges();

                    Rep.AccountDataRecordAdditionalColumnRemove(acc.AdditionalColumns);
                }
            }
            finally
            {
                SqlLogEnabled = true;
            }
        }
    }
}
