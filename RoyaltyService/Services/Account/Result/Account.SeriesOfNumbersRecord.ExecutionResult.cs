using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RoyaltyService.Services.Account.Result
{
    [DataContract(Name = "AccountSeriesOfNumbersRecordResult")]
    public class AccountSeriesOfNumbersRecordExecutionResult : Model.BaseExecutionResult<Model.AccountSeriesOfNumbersRecord>
    {
        public AccountSeriesOfNumbersRecordExecutionResult() { }
        public AccountSeriesOfNumbersRecordExecutionResult(Model.AccountSeriesOfNumbersRecord value) : base(value) { }
        public AccountSeriesOfNumbersRecordExecutionResult(Exception ex) : base(ex) { }
    }

    [DataContract(Name = "AccountSeriesOfNumbersRecordResults")]
    public class AccountSeriesOfNumbersRecordExecutionResults : Model.BaseExecutionResults<Model.AccountSeriesOfNumbersRecord>
    {
        public AccountSeriesOfNumbersRecordExecutionResults() { }
        public AccountSeriesOfNumbersRecordExecutionResults(Model.AccountSeriesOfNumbersRecord[] values) : base(values) { }
        public AccountSeriesOfNumbersRecordExecutionResults(Exception ex) : base(ex) { }
    }
}
