using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RoyaltyService.Services.Account.Result
{
    [DataContract(Name = "ImportQueueRecordStateResults")]
    public class ImportQueueRecordStateResults : Model.BaseExecutionResults<Model.ImportQueueRecordState>
    {
        public ImportQueueRecordStateResults() { }
        public ImportQueueRecordStateResults(Model.ImportQueueRecordState[] values) : base(values) { }
        public ImportQueueRecordStateResults(Exception ex) : base(ex) { }
    }
}
