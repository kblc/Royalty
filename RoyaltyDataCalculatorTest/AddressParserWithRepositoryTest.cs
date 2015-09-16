using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoyaltyDataCalculator.AddressParser;
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
            Rep = new Repository("connectionString");
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
        public void AddressParser_GetStreets()
        {
            using(Rep.BeginTransaction())
            {
                var c = Rep.CityNew("testCity");
                Rep.CityAdd(c);

                var a0 = Rep.AreaNew("test area0", c);
                var a1 = Rep.AreaNew("test area1", c);

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

                var a = Rep.AccountGet(defAccountName);

                Rep.AccountDictionaryRecordNew(a.Dictionary, s00);
                Rep.AccountDictionaryRecordNew(a.Dictionary, s01, s00);
                Rep.AccountDictionaryRecordNew(a.Dictionary, s10);
                Rep.AccountDictionaryRecordNew(a.Dictionary, s11, s10);

                Rep.SaveChanges();

                var res0 = AddressParser.GetStreet(s00.Name, Rep, a.Dictionary, c, doNotAddAnyDataToDictionary: true);
                Assert.AreEqual(s00.Name, res0.Name, "res0: Values must equals");

                var res1 = AddressParser.GetStreet(s01.Name, Rep, a.Dictionary, c, doNotAddAnyDataToDictionary: true);
                Assert.AreEqual(s00.Name, res1.Name, "res1: Values must equals");

                var res2 = AddressParser.GetStreet(s10.Name.Remove(0, 2), Rep, a.Dictionary, c, doNotAddAnyDataToDictionary: true);
                Assert.AreEqual(s10.Name, res2.Name, "res2: Values must equals");

                var res3 = AddressParser.GetStreet(s11.Name.Remove(2, 1), Rep, a.Dictionary, c, doNotAddAnyDataToDictionary: true);
                Assert.AreEqual(s10.Name, res3.Name, "res3: Values must equals");
            }
        }

        [TestMethod]
        public void AddressParser_TestAccountAutoDelete()
        {
            using (Rep.BeginTransaction())
            {
                var c = Rep.CityNew("testCity");
                Rep.CityAdd(c);

                var a0 = Rep.AreaNew("test area0", c);
                var a1 = Rep.AreaNew("test area1", c);

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

                var a = Rep.AccountGet(defAccountName);

                Rep.AccountDictionaryRecordNew(a.Dictionary, s00);
                //Rep.AccountDictionaryRecordNew(a.Dictionary, s01, s00);
                //Rep.AccountDictionaryRecordNew(a.Dictionary, s10);
                //Rep.AccountDictionaryRecordNew(a.Dictionary, s11, s10);

                Rep.SaveChanges();

                //var res0 = AddressParser.GetStreet(s00.Name, Rep, a.Dictionary, c, doNotAddAnyDataToDictionary: true);
                //Assert.AreEqual(s00.Name, res0.Name, "res0: Values must equals");

                //var res1 = AddressParser.GetStreet(s01.Name, Rep, a.Dictionary, c, doNotAddAnyDataToDictionary: true);
                //Assert.AreEqual(s00.Name, res1.Name, "res1: Values must equals");

                //var res2 = AddressParser.GetStreet(s10.Name.Remove(0, 2), Rep, a.Dictionary, c, doNotAddAnyDataToDictionary: true);
                //Assert.AreEqual(s10.Name, res2.Name, "res2: Values must equals");

                //var res3 = AddressParser.GetStreet(s11.Name.Remove(2, 1), Rep, a.Dictionary, c, doNotAddAnyDataToDictionary: true);
                //Assert.AreEqual(s10.Name, res3.Name, "res3: Values must equals");
            }
        }

    }
}
