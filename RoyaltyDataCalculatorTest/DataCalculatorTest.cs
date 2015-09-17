using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoyaltyDataCalculator;
using RoyaltyRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            Rep = new Repository("connectionString");
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
            using (var dc = new DataCalculator(a))
            {
                var csvLines = new List<string>();
                csvLines.Add("column0;column1;column2");

                var l = Helpers.CSV.CSVFile.Load(csvLines.AsEnumerable<string>(), 
                    tableName: "{virtual}", 
                    filePath: "{virtual}",
                    tableValidator: dc.TableValidator,
                    rowFilter: dc.RowFilter);

                dc.Preview(l.Table, Rep);
            }
        }
    }
}
