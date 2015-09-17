using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoyaltyDataCalculator;
using RoyaltyRepository;
using RoyaltyRepository.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Helpers;

namespace RoyaltyDataCalculatorTest
{
    [TestClass]
    public class DataCalculatorTest
    {
        private const string defAccountName = ".DataCalculatorTest";
        public Repository Rep { get; set; }

        [TestInitialize]
        public void Initialization()
        {
            Rep = new Repository("connectionStringHome");
            Rep.AccountRemove(Rep.AccountGet(defAccountName, true));
            Rep.AccountAdd(Rep.AccountNew(byDefault: true, accountName: defAccountName));
            Rep.Log = (s) => { Console.WriteLine(string.Format("{0}", s)); };
            Console.WriteLine("############################## Initialization done");
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
        [ExpectedException(typeof(Exception))]
        public void DataCalculator_Preview_ExceptOnTableValidation()
        {
            var a = Rep.AccountGet(defAccountName);
            if (a != null)
                using (var dc = new DataCalculator(a))
                {
                    var columns = string.Empty;
                    var colValues = a.Settings.GetType()
                        .GetProperties()
                        .Where(pi => pi.Name.EndsWith("ColumnName"))
                        .Where(pi => pi.GetCustomAttributes(typeof(IsRequiredForColumnImportAttribute), false).Length > 0)
                        .Select(pi => pi.GetValue(a.Settings, null))
                        .Where(pi => pi != null)
                        .Select(pi => pi.ToString().ToLower())
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
            var a = Rep.AccountGet(defAccountName);
            if (a != null)
                using (var dc = new DataCalculator(a))
                {
                    var columns = string.Empty;
                    var colValues = a.Settings.GetType()
                        .GetProperties()
                        .Where(pi => pi.Name.EndsWith("ColumnName"))
                        .Where(pi => pi.GetCustomAttributes(typeof(IsRequiredForColumnImportAttribute), false).Length > 0)
                        .Select(pi => pi.GetValue(a.Settings, null))
                        .Where(pi => pi != null)
                        .Select(pi => pi.ToString().ToLower());

                    var csvLines = new List<string>();

                    foreach (var colName in colValues)
                        columns += (string.IsNullOrWhiteSpace(columns) ? string.Empty : ";") + colName;
                    csvLines.Add(columns);

                    var cnt = 10000;
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
                            if (s.Contains("good_data") && !s.Contains("bad_data"))
                                throw new Exception("Good data filtered");
                        },
                        rowFilter: r => colValues.Select(c => r[c].ToString()).Any(i => i.Contains("bad_data")));

                    Assert.AreEqual(goodCnt, l.Table.Rows.Count, "Good row count must equals");
                }
        }
    }
}
