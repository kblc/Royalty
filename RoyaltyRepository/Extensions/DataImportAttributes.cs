using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoyaltyRepository.Extensions
{
    [System.AttributeUsage(AttributeTargets.Property)]
    public class IsRequiredForRowImportAttribute : System.Attribute
    {
    }

    [System.AttributeUsage(AttributeTargets.Property)]
    public class IsRequiredForColumnImportAttribute : System.Attribute
    {
    }
}
