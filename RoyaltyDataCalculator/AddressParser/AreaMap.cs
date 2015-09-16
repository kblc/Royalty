using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoyaltyDataCalculator.AddressParser
{
    public class AreaMap : List<House>
    {
        internal static uint LengthBetween(House from, House to)
        {
            return (from == null || to == null || !from.Number.HasValue || !to.Number.HasValue)
                ? uint.MaxValue
                : (uint)(Math.Abs(from.Number.Value - to.Number.Value));
        }

        internal static decimal Weight(decimal lengthBetween, decimal maxLenght, decimal maxWeight)
        {
            return -(1m / (1.5m * maxLenght * (1 / maxWeight))) * lengthBetween + maxWeight;
        }
        internal static decimal Weight(House from, House to, decimal maxLenght, decimal maxWeight)
        {
            return Weight(LengthBetween(from, to), maxLenght, maxWeight);
        }

        public decimal Weight(House from, decimal radius, decimal maxWeight)
        {
            return this
                .Select(i => (decimal)LengthBetween(from, i))
                .Where(i => i <= radius)
                .Union(new decimal[] { 0 })
                .Sum(i => Weight(i, radius, maxWeight));
        }
    }
}
