using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoyaltyDataCalculator.Parser;
using RoyaltyRepository;

namespace RoyaltyDataCalculatorTest
{
    [TestClass]
    public class AddressParserWithRepositoryTest
    {
        private const string defAccountName = "default0";
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
        public void AddressParser_GetStreets()
        {
            Rep.CityRemove(Rep.CityGet("testCity"));

            var c = Rep.CityNew("testCity");
            Rep.CityAdd(c);

            var a0 = Rep.AreaNew("test area0", city: c);
            var a1 = Rep.AreaNew("test area1", city: c);

            Rep.AreaAdd(a0, saveAfterInsert: false);
            Rep.AreaAdd(a1, saveAfterInsert: false);
            Rep.SaveChanges();

            var s00 = Rep.StreetNew("Ивановского", a0);
            var s01 = Rep.StreetNew("Пузановского", a0);
            var s10 = Rep.StreetNew("Победы", a1);
            var s11 = Rep.StreetNew("Успеха", a1);

            Rep.StreetAdd(s00, saveAfterInsert: false);
            Rep.StreetAdd(s01, saveAfterInsert: false);
            Rep.StreetAdd(s10, saveAfterInsert: false);
            Rep.StreetAdd(s11, saveAfterInsert: false);
            Rep.SaveChanges();

            var a = Rep.AccountGet(defAccountName, eagerLoad: new string[] { "Dictionary", "Dictionary.Records" });

            Rep.AccountDictionaryRecordNew(a.Dictionary, s00);
            Rep.AccountDictionaryRecordNew(a.Dictionary, s01, s00);
            Rep.AccountDictionaryRecordNew(a.Dictionary, s10);
            Rep.AccountDictionaryRecordNew(a.Dictionary, s11, s10);

            Rep.SaveChanges();

            var res0 = AddressParser.GetStreet(s00.Name, Rep, a.Dictionary, c, doNotAddAnyDataToDictionary: true);
            Assert.AreEqual(s00.Name, res0, "res0: Values must equals");

            var res1 = AddressParser.GetStreet(s01.Name, Rep, a.Dictionary, c, doNotAddAnyDataToDictionary: true);
            Assert.AreEqual(s00.Name, res1, "res1: Values must equals");

            var res2 = AddressParser.GetStreet(s10.Name.Remove(0, 2), Rep, a.Dictionary, c, doNotAddAnyDataToDictionary: true);
            Assert.AreEqual(s10.Name, res2, "res2: Values must equals");

            var res3 = AddressParser.GetStreet(s11.Name.Remove(2, 1), Rep, a.Dictionary, c, doNotAddAnyDataToDictionary: true);
            Assert.AreEqual(s10.Name, res3, "res3: Values must equals");

            var d = DateTime.UtcNow;

            var testName = "Тестовая улица";

            var res = AddressParser.GetStreets(new string[] { s00.Name, s01.Name, s10.Name, s11.Name, testName }, Rep, a.Dictionary, c);
            var res01 = res[s00.Name];
            var res02 = res[s01.Name];
            var res03 = res[s10.Name];
            var res04 = res[s11.Name];
            var res05 = res[testName];

            Assert.AreNotEqual(null, res01, "res01 cant be null");
            Assert.AreNotEqual(null, res02, "res02 cant be null");
            Assert.AreNotEqual(null, res03, "res03 cant be null");
            Assert.AreNotEqual(null, res04, "res04 cant be null");
            Assert.AreNotEqual(null, res05, "res05 cant be null");
            Assert.AreEqual(s00.Name, res01, "res01: Values must equals");
            Assert.AreEqual(s00.Name, res02, "res02: Values must equals");
            Assert.AreEqual(s10.Name, res03, "res03: Values must equals");
            Assert.AreEqual(s10.Name, res04, "res04: Values must equals");
            Assert.AreEqual(testName, res05, "res05: Values must equals");

            Console.WriteLine(string.Format("Time elapsed: {0} ms", (DateTime.UtcNow - d).TotalMilliseconds.ToString("N2")));

            Rep.CityRemove(c);
        }
    }
}
