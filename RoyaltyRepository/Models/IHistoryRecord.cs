using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoyaltyRepository.Models
{
    public interface IHistoryRecordSource
    {
        string SourceId { get; }
        string SourceName { get; }
    }

    [AttributeUsage(AttributeTargets.Method, Inherited = true)]
    public class HistoryResolverAttribute : Attribute
    {

    }
}
