using RoyaltyDataCalculator.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoyaltyDataCalculator.Parser
{
    internal static class AreaMap
    {
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
    }
}
