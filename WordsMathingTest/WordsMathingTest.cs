using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WordsMathingTest
{
    [TestClass]
    public class WordsMathingTest
    {
        [TestMethod]
        public void WordsMathingTest_Test()
        {
            var res = (decimal)new WordsMatching.MatchsMaker("Победа", "Беда").Score;
            Assert.AreEqual(0.66, (double)res, 0.01, "Values must equals");
        }
    }
}
