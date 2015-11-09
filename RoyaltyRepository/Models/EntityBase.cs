using RoyaltyRepository.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Helpers.Serialization;
using System.Xml.Serialization;
using System.Linq.Expressions;

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

    public abstract class HistoryEntityBase : EntityBase, IHistoryRecordSource
    {
        protected virtual string GetSourceId()
        {
            var validProperties = GetType().GetProperties()
                .Where(p => p.GetCustomAttributes(typeof(System.ComponentModel.DataAnnotations.KeyAttribute), true)?.Length > 0)
                .ToArray();

            if (validProperties.Length > 1)
                throw new Exception(string.Format(Properties.Resources.HISTORYRECORD_EntityBreakOneToOneRule, GetType().FullName));

            if (validProperties.Length == 1)
                return validProperties[0].GetValue(this)?.ToString();

            return null;
        }
        protected virtual string GetSourceType()
        {
            var parentType = GetType();
            while (parentType != null && parentType.BaseType != typeof(HistoryEntityBase))
                parentType = parentType.BaseType;
            return parentType?.Name;
        }

        #region IHistoryRecordSource

        string IHistoryRecordSource.SourceId => GetSourceId();
        string IHistoryRecordSource.SourceName => GetSourceType();

        #endregion
    }

    public abstract class HistoryEntityBase<TKey, TEntity> : HistoryEntityBase
        where TEntity : class
    {
        protected override string GetSourceType()
        {
            var parentType = GetType();
            while (parentType != null && parentType.BaseType != typeof(HistoryEntityBase<TKey, TEntity>))
                parentType = parentType.BaseType;
            return parentType?.Name;
        }

        private static System.Linq.Expressions.Expression<Func<TEntity, bool>> GetHistoryFilter(IEnumerable<TKey> identifiers)
        {
            var validProperties = typeof(TEntity).GetProperties()
                    .Where(p => p.GetCustomAttributes(typeof(System.ComponentModel.DataAnnotations.KeyAttribute), true)?.Length > 0)
                    .ToArray();

            if (validProperties.Length > 1)
                throw new Exception(string.Format(Properties.Resources.HISTORYRECORD_EntityBreakOneToOneRule, typeof(TEntity).FullName));

            if (validProperties.Length == 1)
            {
                var keyPropertyName = validProperties[0].Name;

                var pe = Expression.Parameter(typeof(TEntity));
                var me = Expression.Property(pe, keyPropertyName);
                var ce = Expression.Constant(identifiers);
                var call = Expression.Call(typeof(Enumerable), "Contains", new[] { me.Type }, ce, me);
                return Expression.Lambda<Func<TEntity, bool>>(call, pe);

                //var paramEntry = Expression.Parameter(typeof(TEntity), "entry");
                //var keyProperty = Expression.Property(paramEntry, keyPropertyName);

                //var enumValues = identifiers.ToList();
                //var someValue = Expression.Constant(enumValues, enumValues.GetType());
                //var type = someValue.Type;

                //var method = type.GetMethod("Contains", new[] { typeof(TKey) });
                //var containsMethodExp = Expression.Call(keyProperty, method, someValue);

                //return Expression.Lambda<Func<TEntity, bool>>(containsMethodExp, paramEntry);
            }

            return (i) => false;
        }

        #region History resolver

        [HistoryResolver]
        internal static TKey[] GetIdentifiers(Repository rep, string[] identifiers)
        {
            if (typeof(TKey) == typeof(Guid))
            {
                var res = identifiers.Select(id => (TKey)(object)Guid.Parse(id))
                    .ToArray();
                return res;
            }
            else
            { 
                var res = identifiers.Select(id => (TKey)Convert.ChangeType(id, typeof(TKey)))
                    .ToArray();
                return res;
            }
        }

        [HistoryResolver]
        internal static TEntity[] GetItemsToDelete(Repository rep, string[] identifiers)
        {
            var ids = GetIdentifiers(rep, identifiers);
            return rep.Get<TEntity>(GetHistoryFilter(ids), asNoTracking: true).ToArray();
        }

        #endregion
    }
}
