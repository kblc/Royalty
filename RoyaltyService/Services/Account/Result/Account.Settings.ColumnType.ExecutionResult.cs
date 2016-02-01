using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RoyaltyService.Services.Account.Result
{
    [DataContract(Name = "AccountSettingsColumnTypeResults")]
    public class AccountSettingsColumnTypeExecutionResults : Model.BaseExecutionResults<Model.ColumnType>
    {
        public AccountSettingsColumnTypeExecutionResults() { }
        public AccountSettingsColumnTypeExecutionResults(Model.ColumnType[] values) : base(values) { }
        public AccountSettingsColumnTypeExecutionResults(Exception ex) : base(ex) { }
    }
}
