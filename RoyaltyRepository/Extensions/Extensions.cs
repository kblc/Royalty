using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoyaltyRepository.Extensions
{
    public static class Extensions
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
                                        Attr = (pi.GetCustomAttributes(columnType, false).FirstOrDefault() as ColumnAttribute),
                                        Value = pi.GetValue(obj)
                                    })
                                .Select(i => new
                                    {
                                        Name = i.Attr == null ? "<err>" : i.Attr.Name,
                                        i.Value
                                    })
                                )
                properties += (string.IsNullOrWhiteSpace(properties) ? string.Empty : ",") + string.Format("{0}='{1}'", i.Name, i.Value == null ? "NULL" : i.Value.ToString());
            return string.Format("{0}:[{1}]", type.Name, properties);
        }

        public static void FillFromAnonymousType(this object obj, object anonymousObject)
        {
            if (obj == null)
                throw new ArgumentNullException("obj");
            if (anonymousObject == null)
                throw new ArgumentNullException("anonymousObject");

            Type type = obj.GetType();
            Type typeAn = anonymousObject.GetType();
            foreach(var pi in typeAn.GetProperties())
            {
                var value = pi.GetValue(anonymousObject, null);
                var setProp = type.GetProperty(pi.Name);
                if (setProp != null)
                    setProp.SetValue(obj, value);
            }
        }

        /// <summary>
        /// Get enum element valid name from enum
        /// </summary>
        /// <param name="type">Enum value</param>
        /// <returns>Enum valid name from resource</returns>
        public static string GetEnumNameFromType(this Enum type)
        {
            var descr = type.GetAttributeOfType<DescriptionAttribute>()?.Description ?? type.ToString();
            var obj = RoyaltyRepository.Properties.Resources.ResourceManager.GetObject(descr.ToUpper());
            return obj == null ? descr : obj.ToString();
        }

        /// <summary>
        /// Gets an attribute on an enum field value
        /// </summary>
        /// <typeparam name="T">The type of the attribute you want to retrieve</typeparam>
        /// <param name="enumVal">The enum value</param>
        /// <returns>The attribute of type T that exists on the enum value</returns>
        /// <example>string desc = myEnumVariable.GetAttributeOfType<DescriptionAttribute>().Description;</example>
        public static T GetAttributeOfType<T>(this object enumVal) where T : System.Attribute
        {
            var type = enumVal.GetType();
            var memInfo = type.GetMember(enumVal.ToString());
            if (memInfo.Length > 0)
            {
                var attributes = memInfo[0].GetCustomAttributes(typeof(T), false);
                return (attributes.Length > 0) ? (T)attributes[0] : null;
            }
            else
                return null;
        }

        /// <summary>
        /// Get encoding by encoding name
        /// </summary>
        /// <param name="encodingName">Encoding name</param>
        /// <returns>Founded encoding or null if not found</returns>
        internal static Encoding GetEncodingByName(string encodingName)
        {
            try
            {
                return Encoding.GetEncoding(encodingName);
            }
            catch
            {
                return Encoding.Default;
            }
        }
    }
}
