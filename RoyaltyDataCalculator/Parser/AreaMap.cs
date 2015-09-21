using RoyaltyDataCalculator.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoyaltyDataCalculator.Parser
{
    public class AreaMap : List<House>
    {
        internal static uint LengthBetween(House from, House to)
        {
            return (from == null || to == null || !from.Number.HasValue || !to.Number.HasValue)
                ? uint.MaxValue
                : (uint)(Math.Abs(from.Number.Value - to.Number.Value));
        }

        internal static uint LengthBetween(uint fromHouseNumber, uint toHouseNumber)
        {
            return (uint)Math.Abs((long)fromHouseNumber - (long)toHouseNumber);
        }
        internal static decimal Weight(decimal lengthBetween, decimal radiusLenght, decimal maxWeight = 1m)
        {
            if (radiusLenght == 0)
                throw new ArgumentOutOfRangeException(nameof(radiusLenght), radiusLenght, Resources.AreaMap_Weight_RadiusLenghtCantEqualsZero);

            return -(1m / (1.5m * radiusLenght * (1 / maxWeight))) * lengthBetween + maxWeight;
        }
        internal static decimal Weight(House from, House to, decimal radiusLenght, decimal maxWeight = 1m)
        {
            return Weight(LengthBetween(from, to), radiusLenght, maxWeight);
        }

        public decimal Weight(House from, decimal radiusLenght, decimal maxWeight = 1m)
        {
            return this
                .Select(i => (decimal)LengthBetween(from, i))
                .Where(i => i <= radiusLenght)
                .Union(new decimal[] { 0 })
                .Sum(i => Weight(i, radiusLenght, maxWeight));
        }
    }
}
