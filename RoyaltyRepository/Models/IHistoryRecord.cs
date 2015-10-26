using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoyaltyRepository.Models
{
    public interface IHistoryRecordSource
    {
        object SourceId { get; }
        HistorySourceType SourceType { get; }
    }
}
