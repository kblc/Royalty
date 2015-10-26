using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RoyaltyRepository.Extensions
{
    internal static class Helpers
    {
        public static T GetEnumValueByName<T>(string enumName) //where T : Enum
        {
            return typeof(T).GetEnumValues().Cast<T>().FirstOrDefault(ct => ct.ToString().ToUpper() == enumName);
        }
    }
}
