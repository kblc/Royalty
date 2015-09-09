using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoyaltyRepository.Extensions
{
    internal static class Extensions
    {
        public static string GetColumnPropertiesForEntity(this object obj)
        {
            string properties = string.Empty;
            var type = obj.GetType();
            var columnType = typeof(ColumnAttribute);
            foreach (var i in type.GetProperties()
                                .Where(pi => pi.GetCustomAttributes(columnType, false).Any())
                                .Select(pi => new
                                    {
                                        Name = ((pi.GetCustomAttributes(columnType, false).FirstOrDefault() as ColumnAttribute)?.Name ?? "<err>").ToLower(),
                                        Value = pi.GetValue(obj)
                                    }))
                properties += (string.IsNullOrWhiteSpace(properties) ? string.Empty : ",") + string.Format("{0}='{1}'", i.Name, i.Value == null ? "NULL" : i.Value.ToString());
            return string.Format("{0}:[{1}]", type.Name, properties);
        }
    }
}
