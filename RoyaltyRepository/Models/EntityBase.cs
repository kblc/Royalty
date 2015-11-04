using RoyaltyRepository.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Helpers.Serialization;
using System.Xml.Serialization;

namespace RoyaltyRepository.Models
{
    public class EntityBase
    {
        #region ToString()

        public override string ToString()
        {
            return this.GetColumnPropertiesForEntity();
        }

        #endregion
    }

    [Serializable]
    [XmlRoot("id", IsNullable = false)]
    public class HistoryRecordSourceIdentifier : IHistoryRecordSourceIdentifier
    {
        //[XmlElement(ElementName = "name", IsNullable = false)]
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }
        [XmlAttribute(AttributeName = "value")]
        //[XmlElement(ElementName = "value", IsNullable = false)]
        public string Value { get; set; }

        /// <summary>
        /// Create new instance
        /// </summary>
        public HistoryRecordSourceIdentifier() { }

        /// <summary>
        /// Create new instance
        /// </summary>
        /// <param name="name">Name</param>
        /// <param name="value">Value</param>
        public HistoryRecordSourceIdentifier(string name, string value)
        {
            this.Name = name;
            this.Value = value;
        }
    }

    public abstract class HistoryEntityBase : EntityBase, IHistoryRecordSource
    {
        protected virtual IEnumerable<HistoryRecordSourceIdentifier> GetSourceId()
        {
            var properties = GetType().GetProperties();
            var validProperties = properties
                .Where(p =>
                {
                    var attr = p.GetCustomAttributes(typeof(System.ComponentModel.DataAnnotations.KeyAttribute), true);
                    return (attr != null && attr.Length > 0);
                })
                .ToArray();
            var res = validProperties
                .Select(p => new { p.Name, Value = p.GetValue(this) })
                .Where(p => p.Value != null)
                .Select(p => new HistoryRecordSourceIdentifier(p.Name, p.Value.ToString()))
                .ToArray();

            return res;
        }
        protected virtual string GetSourceType()
        {
            var parentType = GetType();
            while (parentType != null && parentType.BaseType != typeof(HistoryEntityBase))
                parentType = parentType.BaseType;
            return parentType?.Name;
        }
        protected virtual string SerializeIds()
        {
            throw new NotImplementedException();
        }

        #region IHistoryRecordSource

        IEnumerable<IHistoryRecordSourceIdentifier> IHistoryRecordSource.SourceId => GetSourceId();
        string IHistoryRecordSource.SourceName => GetSourceType();
        string IHistoryRecordSource.GetSourceIdString() => SerializeIdentifiers(GetSourceId());

        #endregion

        public static string SerializeIdentifiers(IEnumerable<HistoryRecordSourceIdentifier> ids)
        {
            var arr = ids.ToArray();
            var res = (arr.Length > 0 ? arr.SerializeToXML(true) : null)
                ?.Replace("ArrayOfHistoryRecordSourceIdentifier", "ids")
                ?.Replace(nameof(HistoryRecordSourceIdentifier),"id")
                ;
            return res;
        }

        public static IEnumerable<HistoryRecordSourceIdentifier> DeserializeIdentifiers(string serializedIds)
        {
            HistoryRecordSourceIdentifier[] res;
            typeof(HistoryRecordSourceIdentifier[]).DeserializeFromXML(serializedIds
                .Replace("ids", "ArrayOfHistoryRecordSourceIdentifier")
                .Replace("id", nameof(HistoryRecordSourceIdentifier))
                , out res);
            return res;
        }
    }
}
