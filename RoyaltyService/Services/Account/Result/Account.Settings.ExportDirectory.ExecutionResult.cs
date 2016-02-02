using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RoyaltyService.Services.Account.Result
{
    [DataContract(Name = "AccountSettingsExportDirectoryResult")]
    public class AccountSettingsExportDirectoryExecutionResult : Model.BaseExecutionResult<Model.AccountSettingsExportDirectory>
    {
        public AccountSettingsExportDirectoryExecutionResult() { }
        public AccountSettingsExportDirectoryExecutionResult(Model.AccountSettingsExportDirectory value) : base(value) { }
        public AccountSettingsExportDirectoryExecutionResult(Exception ex) : base(ex) { }
    }

    [DataContract(Name = "AccountSettingsExportDirectoryResults")]
    public class AccountSettingsExportDirectoryExecutionResults : Model.BaseExecutionResults<Model.AccountSettingsExportDirectory>
    {
        public AccountSettingsExportDirectoryExecutionResults() { }
        public AccountSettingsExportDirectoryExecutionResults(Model.AccountSettingsExportDirectory[] values) : base(values) { }
        public AccountSettingsExportDirectoryExecutionResults(Exception ex) : base(ex) { }
    }
}
