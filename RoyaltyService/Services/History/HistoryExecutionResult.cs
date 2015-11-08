using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RoyaltyService.Services.History
{
    [DataContract(Name = "HistoryResult")]
    public class HistoryExecutionResult : Model.BaseExecutionResult<Model.History>
    {
        public HistoryExecutionResult() { }
        public HistoryExecutionResult(Model.History value) { Value = value; }
        public HistoryExecutionResult(Exception ex) : base(ex) { }
    }
}
