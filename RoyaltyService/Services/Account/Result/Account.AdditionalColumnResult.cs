using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RoyaltyService.Services.Account.Result
{
    [DataContract(Name = "AccountDataRecordAdditionalColumnResult")]
    public class AccountDataRecordAdditionalColumnExecutionResult : Model.BaseExecutionResult<Model.AccountDataRecordAdditionalColumn>
    {
        public AccountDataRecordAdditionalColumnExecutionResult() { }
        public AccountDataRecordAdditionalColumnExecutionResult(Model.AccountDataRecordAdditionalColumn value) : base(value) { }
        public AccountDataRecordAdditionalColumnExecutionResult(Exception ex) : base(ex) { }
    }

    [DataContract(Name = "AccountDataRecordAdditionalColumnResults")]
    public class AccountDataRecordAdditionalColumnExecutionResults : Model.BaseExecutionResults<Model.AccountDataRecordAdditionalColumn>
    {
        public AccountDataRecordAdditionalColumnExecutionResults() { }
        public AccountDataRecordAdditionalColumnExecutionResults(Model.AccountDataRecordAdditionalColumn[] values) : base(values) { }
        public AccountDataRecordAdditionalColumnExecutionResults(Exception ex) : base(ex) { }
    }
}
