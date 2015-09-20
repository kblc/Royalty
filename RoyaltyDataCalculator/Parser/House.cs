using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoyaltyDataCalculator.Parser
{
    /// <summary>
    /// Additional part for house number (e.g '/11a')
    /// </summary>
    public class HouseAdditionalPart : IComparable<HouseAdditionalPart>
    {
        public uint? Number { get; private set; }
        public string Letter { get; private set; }

        public HouseAdditionalPart() : this(null, string.Empty) { }
        public HouseAdditionalPart(uint? number, string letter)
        {
            Number = number;
            Letter = letter;
        }

        public override string ToString()
        {
            return !Number.HasValue ? Letter : (House.HousingDelimiter + Number.ToString() + Letter);
        }

        public static HouseAdditionalPart FromString(string additional)
        {
            uint? number = null;
            string letter = string.Empty;

            additional = additional.Replace(House.HousingDelimiter, string.Empty).Replace(" ", string.Empty);

            if (!string.IsNullOrWhiteSpace(additional))
            {
                int numb;
                for (int i = 0; i < additional.Length; i++)
                {
                    if (House.digits.IndexOf(additional[i]) <= 0)
                    {
                        if (int.TryParse(additional.Substring(0, i), out numb))
                            number = (uint)numb;
                        letter = additional.Substring(i);
                        break;
                    }
                }
                if (string.IsNullOrWhiteSpace(letter))
                    if (int.TryParse(additional, out numb))
                        number = (uint)numb;
            }
            return new HouseAdditionalPart(number, letter);
        }

        public static bool operator ==(HouseAdditionalPart a, HouseAdditionalPart b)
        {
            return (a != null) ? a.Equals(b) : false;
        }
        public static bool operator !=(HouseAdditionalPart a, HouseAdditionalPart b)
        {
            return !(a == b);
        }
        public override bool Equals(object obj)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(this, obj))
                return true;

            // If one is null, but not both, return false.
            if (((object)this == null) || ((object)obj == null))
                return false;

            // Return true if the fields match:
            return this.ToString() == obj.ToString();
        }
        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }

        public int CompareTo(HouseAdditionalPart other)
        {
            if (Number.HasValue && other.Number.HasValue)
            {
                if (Number.Value == other.Number.Value)
                {
                    return Letter.CompareTo(other.Letter);
                }
                else
                    return Number.Value.CompareTo(other.Number.Value);
            }
            else
                return ToString().CompareTo(other.ToString());
        }
    }

    /// <summary>
    /// House formatter
    /// </summary>
    public class House : IComparable<House>
    {
        public const string HousingDelimiter = "/";
        internal static readonly string digits = new string(Enumerable.Range(0, 10).Select(i => i.ToString().First()).ToArray());

        private uint? number = null;
        /// <summary>
        /// Номер дома
        /// </summary>
        public uint? Number
        {
            get
            {
                return (number.HasValue && number.Value != 0) ? number : null;
            }
            private set
            {
                number = value;
            }
        }

        /// <summary>
        /// Дополнительная буква и прочее от номера дома
        /// </summary>
        public readonly HouseAdditionalPart Additional = new HouseAdditionalPart();

        /// <summary>
        /// Создать новый экземпляр
        /// </summary>
        public House() { }

        /// <summary>
        /// Создать новый экземпляр
        /// </summary>
        /// <param name="number">Номер дома</param>
        public House(uint number) 
            : this()
        {
            Number = number;
        }

        /// <summary>
        /// Создать новый экземпляр
        /// </summary>
        /// <param name="number">Номер дома</param>
        /// <param name="additional"></param>
        public House(uint number, string additional)
            : this(number)
        {
            Additional = HouseAdditionalPart.FromString(additional);
        }

        public override string ToString()
        {
            return (!Number.HasValue || Number.Value == 0) 
                ? string.Empty 
                : Number.ToString() + Additional.ToString();
        }

        public static House FromString(string houseNumber)
        {
            if (houseNumber == null)
                throw new ArgumentNullException("houseNumber");

            houseNumber = houseNumber.Trim();

            uint number = 0;
            string add = string.Empty;
            if (!string.IsNullOrWhiteSpace(houseNumber))
            {
                int numb;
                for (int i = 0; i < houseNumber.Length; i++)
                {
                    if (House.digits.IndexOf(houseNumber[i]) < 0)
                    {
                        if (int.TryParse(houseNumber.Substring(0, i), out numb))
                            number = (uint)numb;
                        add = houseNumber.Substring(i);
                        break;
                    }
                }
                if (string.IsNullOrWhiteSpace(add))
                    if (int.TryParse(houseNumber, out numb))
                        number = (uint)numb;
            }

            return number == 0 ? new House() : new House(number, add);
        }

        public static bool operator ==(House a, House b)
        {
            return ((object)a != null) ? a.Equals(b) : false;
        }
        public static bool operator !=(House a, House b)
        {
            return !(a == b);
        }
        public override bool Equals(object obj)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(this, obj))
                return true;

            // If one is null, but not both, return false.
            if (((object)this == null) || ((object)obj == null))
                return false;

            // Return true if the fields match:
            return this.ToString() == obj.ToString();
        }
        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }

        public int CompareTo(House other)
        {
            if (Number.HasValue && other.Number.HasValue)
            {
                if (Number.Value == other.Number.Value)
                {
                    return Additional.CompareTo(other.Additional);
                }
                else
                    return Number.Value.CompareTo(other.Number.Value);
            }
            else
                return ToString().CompareTo(other.ToString());
        }
    }
}
