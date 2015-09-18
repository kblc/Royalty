using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoyaltyDataCalculator.Parser
{
    public class Phone
    {
        public string PhoneNumber { get; private set; }

        private const string preffix = "8-";
        private static IDictionary<char, char> digitDictionary = Enumerable.Range(0, 10).Select(i => i.ToString().First()).ToDictionary(d => d);
        private const int phoneNumerLenghToAddPreffix = 10;

        private static string PreffixAdd(string phone)
        {
            return (!string.IsNullOrWhiteSpace(preffix) && !phone.StartsWith(preffix) && phone.Length == phoneNumerLenghToAddPreffix) 
                ? preffix + phone 
                : phone;
        }
        private static string PreffixRemove(string phone)
        {
            return (!string.IsNullOrWhiteSpace(preffix) && phone.StartsWith(preffix) && phone.Length > phoneNumerLenghToAddPreffix) 
                ? phone.Remove(0, preffix.Length) 
                : phone;
        }

        /// <summary>
        /// Parse phone number to normalized phone number
        /// </summary>
        /// <param name="incomingPhoneNumber">Incoming phone number string</param>
        /// <returns>Normalized phone number</returns>
        public static Phone FromString(string incomingPhoneNumber, string cityDefaultPhoneCode = null)
        {
            //convert to digits only
            var result = new string(
                PreffixRemove(incomingPhoneNumber != null ? incomingPhoneNumber : string.Empty)
                .Where(c => digitDictionary.ContainsKey(c))
                .ToArray()
                );

            if (!string.IsNullOrWhiteSpace(result))
            {
                //Set max number length equals const phoneNumerLenghToAddPreffix
                if (result.Length > phoneNumerLenghToAddPreffix)
                    result = result.Remove(0, result.Length - phoneNumerLenghToAddPreffix);

                //Add city phone code if exists
                if (!string.IsNullOrWhiteSpace(cityDefaultPhoneCode))
                {
                    if (cityDefaultPhoneCode.Length + result.Length == phoneNumerLenghToAddPreffix)
                    {
                        //Simple add city code
                        result = cityDefaultPhoneCode + result;
                    }
                    else if (cityDefaultPhoneCode.Length + result.Length > phoneNumerLenghToAddPreffix)
                    {
                        var simDigits = cityDefaultPhoneCode.Length + result.Length - phoneNumerLenghToAddPreffix;
                        //Add city code if digits equals
                        if (cityDefaultPhoneCode.Remove(0, cityDefaultPhoneCode.Length - simDigits) == incomingPhoneNumber.Substring(0, simDigits))
                            result = cityDefaultPhoneCode.Substring(0, cityDefaultPhoneCode.Length - simDigits) + incomingPhoneNumber;
                    }
                }
                //Add preffix to phone number
                result = PreffixAdd(result);
            }
            return new Phone { PhoneNumber = result };
        }

        public override string ToString()
        {
            return PhoneNumber;
        }
    }
}
