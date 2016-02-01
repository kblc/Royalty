using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RoyaltyService.Services.Account.Result
{
    [DataContract(Name = "AccountResult")]
    public class AccountExecutionResult : Model.BaseExecutionResult<Model.Account>
    {
        public AccountExecutionResult() { }
        public AccountExecutionResult(Model.Account value) : base(value) { }
        public AccountExecutionResult(Exception ex) : base(ex) { }
    }

    [DataContract(Name = "AccountResults")]
    public class AccountExecutionResults : Model.BaseExecutionResults<Model.Account>
    {
        public AccountExecutionResults() { }
        public AccountExecutionResults(Model.Account[] values) : base(values) { }
        public AccountExecutionResults(Exception ex) : base(ex) { }
    }
}
