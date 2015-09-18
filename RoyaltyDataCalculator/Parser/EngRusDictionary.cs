using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoyaltyDataCalculator.Parser
{
    /// <summary>
    /// English to Russian dictionary static class for fix string with english characters. Use <b>Fix()</b> method to do this.
    /// </summary>
    public static class EngRusDictionary
    {
        #region Dictionary initialization
        private static IDictionary<string, string> Dictionary = new Dictionary<string, string>()
        {
            { "q", "к" },
            { "w", "ш" },
            { "e", "е" },
            { "r", "г" },
            { "t", "т" },
            { "y", "у" },
            { "u", "и" },
            { "i", "и" },
            { "o", "о" },
            { "p", "р" },
            { "a", "а" },
            { "s", "с" },
            { "d", "д" },
            { "f", "ф" },
            { "g", "г" },
            { "h", "х" },
            { "j", "ж" },
            { "k", "к" },
            { "l", "л" },
            { "z", "з" },
            { "x", "х" },
            { "c", "с" },
            { "v", "в" },
            { "b", "б" },
            { "n", "п" },
            { "m", "м" }
        };
        #endregion

        /// <summary>
        /// Fix inner string from eng characters to russian characters
        /// </summary>
        /// <param name="innerString">Inner string</param>
        /// <returns>Fixed inner string</returns>
        public static string Fix(string innerString)
        {
            if (innerString == null)
                throw new ArgumentNullException("innerString");

            if (innerString.Length > 0)
                foreach (var item in Dictionary.Keys)
                    innerString = innerString.Replace(item, Dictionary[item]);
            return innerString;
        }
    }
}
