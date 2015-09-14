﻿using System;
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
    }
}
