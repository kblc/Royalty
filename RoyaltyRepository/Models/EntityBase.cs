using RoyaltyRepository.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoyaltyRepository.Models
{
    public abstract class HistoryEntityBase : IHistoryRecordSource
    {
        protected abstract object GetSourceId();
        protected abstract HistorySourceType GetSourceType();

        #region IHistoryRecordSource

        object IHistoryRecordSource.SourceId { get { return GetSourceId(); } }
        HistorySourceType IHistoryRecordSource.SourceType { get { return GetSourceType(); } }

        #endregion
        #region ToString()

        public override string ToString()
        {
            return this.GetColumnPropertiesForEntity();
        }

        #endregion
    }
}
