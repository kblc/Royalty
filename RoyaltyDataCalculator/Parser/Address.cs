using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoyaltyDataCalculator.Parser
{
    public class Address
    {
        internal static readonly string digits = new string(Enumerable.Range(0, 10).Select(i => i.ToString().First()).ToArray());
        internal static readonly string alphabeet = "йцукенгшщзфывапролджэячсмитьбюё";
        //internal static readonly string alphabeet = "qwertyuiopasdfghjklzxcvbnmйцукенгшщзфывапролджэячсмитьбюё";
        internal static readonly string symbols = " ";
        private enum AddressPart { Symbol, Digit, Letter };


        public string Street { get; private set; }
        public House House { get; private set; }

        public Address()
        {
            House = new House();
        }
        public Address(string street) 
            : this()
        {
            this.Street = street;
        }
        public Address(string street, string house)
            : this(street)
        {
            this.House = House.FromString(house);
        }

        public static Address FromString(string textAddress, IEnumerable<string> excludesStrings = null)
        {
            if (textAddress == null)
                throw new Exception("textAddress");

            string street = string.Empty;
            string house = string.Empty;
            double step = 0;
            try
            {
                if (textAddress.Length > 0)
                {
                    step = 0;
                    #region Translate ENG to RUS

                    textAddress = EngRusDictionary.Fix(" " + textAddress.ToLower() + " ");

                    #endregion
                    step = 1;
                    #region Remove excludes

                    string textAddressBeforeExclude = textAddress;

                    foreach (string delimiter in excludesStrings.Where(s => !string.IsNullOrWhiteSpace(s)).OrderByDescending(s => s.Length))
                        textAddress = textAddress.Replace(delimiter.ToLower(), " ");

                    while (textAddress.Contains("  "))
                        textAddress = textAddress.Replace("  ", " ");
                    textAddress = textAddress.Trim();

                    // что бы после исключения символов не осталась пустая улица
                    if (string.IsNullOrWhiteSpace(textAddress))
                        textAddress = textAddressBeforeExclude;

                    #endregion
                    step = 2;
                    #region Reconstruct '123abc123' to '123 abc 123'

                    var newAddress = string.Empty;
                    var addressParts = Enumerable.Range(0, textAddress.Length)
                        .AsParallel()
                        .Select(i => new { Index = i, Char = textAddress[i] })
                        .Select(c =>
                        {
                            var ch = c.Char;
                            bool isSymbol = symbols.Contains(c.Char);
                            bool isDigit = isSymbol ? false : digits.Contains(c.Char);
                            bool isLetter = (isSymbol || isDigit) ? false : alphabeet.Contains(c.Char.ToString().ToLower().First());
                            if (!isSymbol && !isDigit && !isLetter)
                            {
                                ch = ' ';
                                isSymbol = true;
                            }
                            var t = isSymbol ? AddressPart.Symbol : ((isDigit ? AddressPart.Digit : AddressPart.Letter));

                            return new
                            {
                                Char = ch,
                                c.Index,
                                Type = t,
                            };
                        })
                        .OrderBy(c => c.Index)
                        .ToArray();

                    for (int i = 0; i < addressParts.Length; i++)
                    {
                        var part = addressParts[i];
                        if (i != 0)
                        {
                            var partPrev = addressParts[i - 1];
                            if (part.Type != partPrev.Type && part.Char != ' ')
                                newAddress += ' ';
                        }
                        newAddress += part.Char;
                    }

                    while (newAddress.Contains("  "))
                        newAddress = newAddress.Replace("  "," ");
                    textAddress = newAddress.Trim();

                    #endregion
                    step = 3;
                    #region Split street and house number
                    step = 3.1;
                    #region Street

                    if (!string.IsNullOrWhiteSpace(textAddress))
                    { 
                        string[] streetWords = textAddress.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        
                        bool street_started = false;
                        for (int i = 0; i < streetWords.Length; i++)
                        {
                            bool isDigit = digits.Contains(streetWords[i][0]);
                        
                            if (!isDigit)
                                street_started = true;

                            if ((isDigit && street_started) || (i == streetWords.Length - 1))
                            {
                                for (int n = 0; n < (isDigit ? i : i + 1); n++)
                                    street += " " + streetWords[n];
                                street = street.Trim();
                                break;
                            }
                        }

                        if (street.Length > 0)
                            house = textAddress.Remove(0, street.Length).Trim();
                        else
                            street = textAddress;
                    }
                    #endregion
                    step = 3.2;
                    #region House

                    if (!string.IsNullOrEmpty(house))
                    {
                        var houseItems = house
                            .Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                            .Select(i => new
                            {
                                Item = i,
                                //IsStartFromDigit = digits.Contains(i.First()),
                                IsDigit = digits.Contains(i.First())//i.All(c => digits.Contains(c))
                            })
                            .Take(3)
                            .ToArray();

                        house = string.Empty;
                        for (int i = 0; i < houseItems.Length; i++)
                        {
                            var item = houseItems[i];

                            //House number can contains only 1 letter (e.g. a/b/c/ etc)
                            if (!item.IsDigit && item.Item.Length > 1)
                                break;

                            if (string.IsNullOrEmpty(house))
                                house = item.Item;
                            else
                            {
                                //It is seccond digit
                                if (item.IsDigit)
                                {
                                    if (house.Contains(House.HousingDelimiter))
                                        break;
                                    house += House.HousingDelimiter + item.Item;
                                }
                                else
                                //It is not digit
                                {
                                    //If already contains house delimiter then break or if item part length more then 1
                                    if (item.Item.Length > 1)
                                        break;
                                    house += item.Item;
                                    //Letters adds last
                                    break;
                                }
                            }
                        }

                        int last_index_housing_delimiter;
                        if ((last_index_housing_delimiter = house.LastIndexOf(House.HousingDelimiter)) != house.IndexOf(House.HousingDelimiter))
                            house = house.Remove(last_index_housing_delimiter);
                    }

                    #endregion
                    step = 3.3;
                    #region concat Street and House

                    var words = street.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    street = string.Empty;
                    foreach (string word in words)
                        street += " " + word[0].ToString().ToUpper() + word.Remove(0, 1).ToLower();
                    street = street.Trim();

                    #endregion
                    #endregion
                }

                return new Address(street, house);
            }
            catch (Exception ex)
            {
                var e = new Exception("Parse string exception. See inner exception for details", ex);
                e.Data.Add("Address", textAddress);
                e.Data.Add("Step", step);
                Helpers.Log.Add(e, "Address.FromString()");
                throw e;
            }
        }

        public override string ToString()
        {
            var h = House.ToString();
            return Street + (string.IsNullOrWhiteSpace(h) ? string.Empty : " " + h);
        }
        public static bool operator ==(Address a, Address b)
        {
            return (a != null) ? a.Equals(b) : false;
        }
        public static bool operator !=(Address a, Address b)
        {
            return !(a == b);
        }
        public override bool Equals(object obj)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(this, obj))
            {
                return true;
            }

            // If one is null, but not both, return false.
            if (((object)this == null) || ((object)obj == null))
            {
                return false;
            }

            // Return true if the fields match:
            return this.ToString() == obj.ToString();
        }
        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }
    }
}
