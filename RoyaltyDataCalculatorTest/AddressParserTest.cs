using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoyaltyDataCalculator.Parser;

namespace RoyaltyDataCalculatorTest
{
    [TestClass]
    public class AddressParserTest
    {
        [TestMethod]
        public void Address_Phone_Parse()
        {
            var ph0 = Phone.FromString("1b2c3d00-45");
            Assert.AreEqual("1230045", ph0);

            var ph1 = Phone.FromString("456789", "0123");
            Assert.AreEqual("8-0123456789", ph1);

            var ph2 = Phone.FromString("456789", "0123456");
            Assert.AreEqual("8-0123456789", ph2);
        }

        [TestMethod]
        public void Address_EngRusDictionary_Fix()
        {
            var ph0 = EngRusDictionary.Fix("1a2g");
            Assert.AreEqual("1а2г", ph0);
        }

        [TestMethod]
        public void Address_HouseAdditionalPart_FromString()
        {
            var hc = HouseAdditionalPart.FromString(" /1 г");
            Assert.AreEqual((uint)1, hc.Number);
            Assert.AreEqual("г", hc.Letter);
            Assert.AreEqual("/1г", hc.ToString());

            hc = HouseAdditionalPart.FromString("  г");
            Assert.AreEqual(null, hc.Number);
            Assert.AreEqual("г", hc.Letter);
            Assert.AreEqual("г", hc.ToString());
        }

        [TestMethod]
        public void Address_House_FromString()
        {
            var h = House.FromString(" 12 / 1 а");
            Assert.AreEqual((uint)12, h.Number);
            Assert.AreEqual((uint)1, h.Additional.Number);
            Assert.AreEqual("а", h.Additional.Letter);
            Assert.AreEqual("12/1а", h.ToString());

            h = House.FromString(" 12 / а");
            Assert.AreEqual((uint)12, h.Number);
            Assert.AreEqual(null, h.Additional.Number);
            Assert.AreEqual("а", h.Additional.Letter);
            Assert.AreEqual("12а", h.ToString());
        }

        [TestMethod]
        public void Address_Address_FromString()
        {
            var a = Address.FromString("79Гвардейская, 12 / 1 а", excludesStrings: new string[] { ",", "-", "/" });
            Assert.AreEqual("79 Гвардейская", a.Street, "Street must equals");

            //var h = a.House;
            //Assert.AreEqual((uint)12, h.Number, "Number must be 12");
            //Assert.AreEqual((uint)1, h.Additional.Number, "Additional number must be 1");
            //Assert.AreEqual("а", h.Additional.Letter, "Additional letter must be 'a'");
            //Assert.AreEqual("12/1а", h.ToString(), "Full house number must equals");
        }
    }
}
