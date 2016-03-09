using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RoyaltyService.Services.Account.Result
{
    [DataContract(Name = "ImportQueueRecordResult")]
    public class ImportQueueRecordExecutionResult : Model.BaseExecutionResult<Model.ImportQueueRecord>
    {
        public ImportQueueRecordExecutionResult() { }
        public ImportQueueRecordExecutionResult(Model.ImportQueueRecord value) : base(value) { }
        public ImportQueueRecordExecutionResult(Exception ex) : base(ex) { }
    }

    [DataContract(Name = "ImportQueueRecordResults")]
    public class ImportQueueRecordExecutionResults : Model.BasePagedExecutionResults<Model.ImportQueueRecord>
    {
        public ImportQueueRecordExecutionResults() { }
        public ImportQueueRecordExecutionResults(Model.ImportQueueRecord[] values, uint pageIndex, uint pageCount) : base(values, pageIndex, pageCount) { }
        public ImportQueueRecordExecutionResults(Exception ex) : base(ex) { }
    }
}
