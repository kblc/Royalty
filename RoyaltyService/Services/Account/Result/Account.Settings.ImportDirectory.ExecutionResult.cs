using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RoyaltyService.Services.Account.Result
{
    [DataContract(Name = "AccountSettingsImportDirectoryResult")]
    public class AccountSettingsImportDirectoryExecutionResult : Model.BaseExecutionResult<Model.AccountSettingsImportDirectory>
    {
        public AccountSettingsImportDirectoryExecutionResult() { }
        public AccountSettingsImportDirectoryExecutionResult(Model.AccountSettingsImportDirectory value) : base(value) { }
        public AccountSettingsImportDirectoryExecutionResult(Exception ex) : base(ex) { }
    }

    [DataContract(Name = "AccountSettingsImportDirectoryResults")]
    public class AccountSettingsImportDirectoryExecutionResults : Model.BaseExecutionResults<Model.AccountSettingsImportDirectory>
    {
        public AccountSettingsImportDirectoryExecutionResults() { }
        public AccountSettingsImportDirectoryExecutionResults(Model.AccountSettingsImportDirectory[] values) : base(values) { }
        public AccountSettingsImportDirectoryExecutionResults(Exception ex) : base(ex) { }
    }
}
