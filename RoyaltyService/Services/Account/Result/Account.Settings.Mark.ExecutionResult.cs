using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RoyaltyService.Services.Account.Result
{
    [DataContract(Name = "AccountSettingsMarkResults")]
    public class AccountSettingsMarkExecutionResults : Model.BaseExecutionResults<Model.Mark>
    {
        public AccountSettingsMarkExecutionResults() { }
        public AccountSettingsMarkExecutionResults(Model.Mark[] values) : base(values) { }
        public AccountSettingsMarkExecutionResults(Exception ex) : base(ex) { }
    }
}
