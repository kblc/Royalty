﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoyaltyDataCalculator.Parser;
using RoyaltyRepository;
using System.Linq;

namespace RoyaltyDataCalculatorTest
{
    [TestClass]
    public class AddressParserWithRepositoryTest
    {
        private const string defAccountName = "default0";
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
        public void AddressParser_GetStreets()
        {
            SqlLogEnabled = false;
            try
            {
                Rep.CityRemove(Rep.CityGet("testCity"));

                var c = Rep.CityNew("testCity");
                Rep.CityAdd(c, saveAfterInsert: false);

                var a0 = Rep.AreaNew("test area0", city: c);
                var a1 = Rep.AreaNew("test area1", city: c);

                Rep.AreaAdd(a0, saveAfterInsert: false);
                Rep.AreaAdd(a1, saveAfterInsert: false);

                var s00 = Rep.StreetNew("Ивановского", a0);
                var s01 = Rep.StreetNew("Пузановского", a0);
                var s10 = Rep.StreetNew("Победы", a1);
                var s11 = Rep.StreetNew("Успеха и добра", a1);

                Rep.StreetAdd(s00, saveAfterInsert: false);
                Rep.StreetAdd(s01, saveAfterInsert: false);
                Rep.StreetAdd(s10, saveAfterInsert: false);
                Rep.StreetAdd(s11, saveAfterInsert: false);

                var a = Rep.GetAccount(defAccountName, eagerLoad: new string[] { "Dictionary", "Dictionary.Records" });

                Rep.AccountDictionaryRecordNew(a.Dictionary, s00);
                Rep.AccountDictionaryRecordNew(a.Dictionary, s01, s00);
                Rep.AccountDictionaryRecordNew(a.Dictionary, s10);
                Rep.AccountDictionaryRecordNew(a.Dictionary, s11);

                Rep.SaveChanges();

                var ap = new AddressParser(a, Rep);

                var res0 = ap.GetStreetByDictionary(Address.FromString(s00.Name), c);
                Assert.AreNotEqual(null, res0, "res0 must exists!");
                Assert.AreEqual(s00.Name, res0.Name, "res0: Values must equals");

                var res1 = ap.GetStreetByDictionary(Address.FromString(s01.Name), c);
                Assert.AreNotEqual(null, res1, "res1 must exists!");
                Assert.AreEqual(s00.Name, res1.Name, "res1: Values must equals");

                var res2 = ap.GetStreetByDictionary(Address.FromString(s10.Name.Remove(0, 2)), c);
                Assert.AreNotEqual(null, res2, "res2 must exists!");
                Assert.AreEqual(s10.Name, res2.Name, "res2: Values must equals");

                var res3 = ap.GetStreetByDictionary(Address.FromString(s11.Name.Remove(2, 1)), c);
                Assert.AreNotEqual(null, res3, "res3 must exists!");
                Assert.AreEqual(s11.Name, res3.Name, "res3: Values must equals");

                var d = DateTime.UtcNow;

                var testName = "Тестовая Улица";

                var addrs = new string[] { s00.Name, s01.Name, s10.Name, s11.Name, testName }
                    .Select(cc => Address.FromString(cc))
                    .ToArray();

                var res = ap.GetStreets(addrs, c);

                var res01 = res[addrs[0]].Name;
                var res02 = res[addrs[1]].Name;
                var res03 = res[addrs[2]].Name;
                var res04 = res[addrs[3]].Name;
                var res05 = res[addrs[4]].Name;

                Assert.AreEqual(0, res[addrs[4]].StreetID, "res[4] must be new street");

                Assert.AreNotEqual(null, res01, "res01 cant be null");
                Assert.AreNotEqual(null, res02, "res02 cant be null");
                Assert.AreNotEqual(null, res03, "res03 cant be null");
                Assert.AreNotEqual(null, res04, "res04 cant be null");
                Assert.AreNotEqual(null, res05, "res05 cant be null");
                Assert.AreEqual(s00.Name, res01, "res01: Values must equals");
                Assert.AreEqual(s00.Name, res02, "res02: Values must equals");
                Assert.AreEqual(s10.Name, res03, "res03: Values must equals");
                Assert.AreEqual(s11.Name, res04, "res04: Values must equals");
                Assert.AreEqual(testName, res05, "res05: Values must equals");

                Console.WriteLine(string.Format("Time elapsed: {0} ms", (DateTime.UtcNow - d).TotalMilliseconds.ToString("N2")));

                Rep.CityRemove(c);

            }
            finally
            {
                SqlLogEnabled = true;
            }
        }

        [TestMethod]
        public void AddressParser_GetStreetsByDifferentAreas()
        {
            SqlLogEnabled = false;
            try
            {
                Rep.CityRemove(Rep.CityGet("testCity"));
                var a = Rep.GetAccount(defAccountName, eagerLoad: new string[] { "Dictionary", "Dictionary.Records" });

                var c = Rep.CityNew("testCity");
                Rep.CityAdd(c, saveAfterInsert: false);

                var a0 = Rep.AreaNew("test area0", city: c);
                var a1 = Rep.AreaNew("test area1", city: c);
                var a2 = Rep.AreaNew("test area2", city: c);
                var s00 = Rep.StreetNew("Ивановского", a0);
                var s01 = Rep.StreetNew("Ивановского", a1);
                var s02 = Rep.StreetNew("Ивановского", a2);

                var adr0 = Rep.AccountDictionaryRecordNew(a.Dictionary, s00);
                var adr1 = Rep.AccountDictionaryRecordNew(a.Dictionary, s01);
                var adr2 = Rep.AccountDictionaryRecordNew(a.Dictionary, s02);

                Rep.AccountDictionaryRecordConditionNew(adr0, 0, 100);
                Rep.AccountDictionaryRecordConditionNew(adr1, 101, 200);
                Rep.AccountDictionaryRecordConditionNew(adr2, 201, 300, even: true);

                Rep.SaveChanges();

                var ap = new AddressParser(a, Rep);

                var addr0 = Address.FromString(s00.Name + " 50");
                var addr1 = Address.FromString(s01.Name + " 150");
                var addr2 = Address.FromString(s02.Name + " 250");

                var res0 = ap.GetStreetByDictionary(addr0, c, ss => Console.WriteLine(ss) );
                Assert.AreNotEqual(null, res0, "res0 must exists!");
                Assert.AreEqual(s00.Name, res0.Name, "res0: Street names must equals");
                Assert.AreEqual(a0.Name, res0.Area.Name, "res0: Area names must equals");

                var res1 = ap.GetStreetByDictionary(addr1, c, ss => Console.WriteLine(ss));
                Assert.AreNotEqual(null, res1, "res1 must exists!");
                Assert.AreEqual(s01.Name, res1.Name, "res0: Street names must equals");
                Assert.AreEqual(a1.Name, res1.Area.Name, "res0: Area names must equals");

                var res2 = ap.GetStreetByDictionary(addr2, c, ss => Console.WriteLine(ss));
                Assert.AreNotEqual(null, res2, "res2 must exists!");
                Assert.AreEqual(s02.Name, res2.Name, "res0: Street names must equals");
                Assert.AreEqual(a2.Name, res2.Area.Name, "res0: Area names must equals");

                var res = ap.GetStreets(new Address[] { addr0, addr1, addr2 }, c);

                var res01 = res[addr0]?.Area?.Name;
                var res02 = res[addr1]?.Area?.Name;
                var res03 = res[addr2]?.Area?.Name;

                Assert.AreNotEqual(null, res01, "res01 cant be null");
                Assert.AreNotEqual(null, res02, "res02 cant be null");
                Assert.AreNotEqual(null, res03, "res03 cant be null");
                Assert.AreEqual(a0.Name, res01, "res01: Values must equals");
                Assert.AreEqual(a1.Name, res02, "res02: Values must equals");
                Assert.AreEqual(a2.Name, res03, "res03: Values must equals");

                Rep.CityRemove(c);
            }
            finally
            {
                SqlLogEnabled = true;
            }
        }

        [TestMethod]
        public void AddressParser_GetStreetsBySettedAndUnsettedAreas()
        {
            SqlLogEnabled = false;
            try
            {
                Rep.CityRemove(Rep.CityGet("testCity"));
                var a = Rep.GetAccount(defAccountName, eagerLoad: new string[] { "Dictionary.Records.Street.Area.City" });

                var c = Rep.CityNew("testCity");
                Rep.CityAdd(c, saveAfterInsert: false);

                var a0 = Rep.AreaNew("test area0", city: c);

                Rep.SaveChanges();

                var ap = new AddressParser(a, Rep);

                var addr0 = new Address("Ивановского","50");
                var addr1 = new Address("Ивановского","150", "test area");

                var res = ap.GetStreets(new Address[] { addr0, addr1 }, c)
                    .Select(k => k.Value)
                    .ToArray();

                var res01 = res[0]?.Area?.Name;
                var res02 = res[1]?.Area?.Name;

                Assert.AreNotEqual(null, res01, "res01 cant be null");
                Assert.AreNotEqual(null, res02, "res02 cant be null");
                Assert.AreEqual("test area", res01, "res01: Values must equals");
                Assert.AreEqual("test area", res02, "res02: Values must equals");

                var addr2 = new Address("Петровского", "50");
                var addr3 = new Address("Петровского", "150", "test area");

                var res1 = ap.GetStreets(new Address[] { addr2 }, c)
                    .Select(k => k.Value)
                    .First();
                var res2 = ap.GetStreets(new Address[] { addr3 }, c)
                    .Select(k => k.Value)
                    .First();

                Assert.AreEqual(res1.Area, res2.Area, "re1 area must change area to res2 area");
                Rep.CityRemove(c);
            }
            finally
            {
                SqlLogEnabled = true;
            }
        }
    }
}
