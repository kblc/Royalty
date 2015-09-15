using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoyaltyDataCalculator.AddressParser
{
    public static class Phone
    {
        private const string preffix = "8-";
        private static string digits = new string(Enumerable.Range(0, 10).Select(i => i.ToString().First()).ToArray());
        private const int phoneNumerLenghToAddPreffix = 10;

        private static string PreffixAdd(string phone)
        {
            return (!string.IsNullOrWhiteSpace(preffix) && !phone.StartsWith(preffix)) ? preffix + phone : phone;
        }
        private static string PreffixRemove(string phone)
        {
            return (!string.IsNullOrWhiteSpace(preffix) && phone.StartsWith(preffix)) ? phone.Remove(0, preffix.Length) : phone;
        }

        /// <summary>
        /// Parse phone number to normalized phone number
        /// </summary>
        /// <param name="incomingPhoneNumber">Incoming phone number string</param>
        /// <returns>Normalized phone number</returns>
        public static string Parse(string incomingPhoneNumber)
        {
            //convert to digits only
            var result = new string((incomingPhoneNumber != null ? incomingPhoneNumber : string.Empty)
                .Join(digits, c => c, d => d, (c, d) => c).ToArray());

            if (result.Length > 0)
            {
                if (result.Length == phoneNumerLenghToAddPreffix + 1)
                {
                    result = result.Remove(0, 1);
                    result = PreffixAdd(result);
                }
                else if (result.Length == phoneNumerLenghToAddPreffix)
                    result = PreffixAdd(result);
            }
            return result;
        }
    }
}
