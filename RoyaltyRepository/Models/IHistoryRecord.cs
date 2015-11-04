using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoyaltyRepository.Models
{
    public interface IHistoryRecordSourceIdentifier
    {
        string Value { get; }
        string Name { get; }
    }

    public interface IHistoryRecordSource
    {
        IEnumerable<IHistoryRecordSourceIdentifier> SourceId { get; }
        string GetSourceIdString();
        string SourceName { get; }
    }
}
