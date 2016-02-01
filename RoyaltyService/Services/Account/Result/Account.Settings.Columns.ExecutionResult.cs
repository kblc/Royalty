using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RoyaltyService.Services.Account.Result
{
    [DataContract(Name = "AccountSettingsColumnsResult")]
    public class AccountSettingsColumnsExecutionResult : Model.BaseExecutionResult<Model.AccountSettingsColumn>
    {
        public AccountSettingsColumnsExecutionResult() { }
        public AccountSettingsColumnsExecutionResult(Model.AccountSettingsColumn value) : base(value) { }
        public AccountSettingsColumnsExecutionResult(Exception ex) : base(ex) { }
    }

    [DataContract(Name = "AccountSettingsColumnsResults")]
    public class AccountSettingsColumnsExecutionResults : Model.BaseExecutionResults<Model.AccountSettingsColumn>
    {
        public AccountSettingsColumnsExecutionResults() { }
        public AccountSettingsColumnsExecutionResults(Model.AccountSettingsColumn[] values) : base(values) { }
        public AccountSettingsColumnsExecutionResults(Exception ex) : base(ex) { }
    }
}
