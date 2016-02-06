using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RoyaltyService.Services.Account.Result
{
    [DataContract(Name = "AccountSettingsResult")]
    public class AccountSettingsExecutionResult : Model.BaseExecutionResult<Model.AccountSettings>
    {
        public AccountSettingsExecutionResult() { }
        public AccountSettingsExecutionResult(Model.AccountSettings value) : base(value) { }
        public AccountSettingsExecutionResult(Exception ex) : base(ex) { }
    }
}
