using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RoyaltyService.Services.Account.Result
{
    [DataContract(Name = "AccountSettingsSheduleTimeResult")]
    public class AccountSettingsSheduleTimeExecutionResult : Model.BaseExecutionResult<Model.AccountSettingsSheduleTime>
    {
        public AccountSettingsSheduleTimeExecutionResult() { }
        public AccountSettingsSheduleTimeExecutionResult(Model.AccountSettingsSheduleTime value) : base(value) { }
        public AccountSettingsSheduleTimeExecutionResult(Exception ex) : base(ex) { }
    }

    [DataContract(Name = "AccountSettingsSheduleTimeResults")]
    public class AccountSettingsSheduleTimeExecutionResults : Model.BaseExecutionResults<Model.AccountSettingsSheduleTime>
    {
        public AccountSettingsSheduleTimeExecutionResults() { }
        public AccountSettingsSheduleTimeExecutionResults(Model.AccountSettingsSheduleTime[] values) : base(values) { }
        public AccountSettingsSheduleTimeExecutionResults(Exception ex) : base(ex) { }
    }
}
