using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RoyaltyService.Services.Account.Result
{
    [DataContract(Name = "AccountPhoneMarkResult")]
    public class AccountPhoneMarkExecutionResult : Model.BaseExecutionResult<Model.AccountPhoneMark>
    {
        public AccountPhoneMarkExecutionResult() { }
        public AccountPhoneMarkExecutionResult(Model.AccountPhoneMark value) : base(value) { }
        public AccountPhoneMarkExecutionResult(Exception ex) : base(ex) { }
    }

    [DataContract(Name = "AccountPhoneMarkResults")]
    public class AccountPhoneMarkExecutionResults : Model.BasePagedExecutionResults<Model.AccountPhoneMark>
    {
        public AccountPhoneMarkExecutionResults() { }
        public AccountPhoneMarkExecutionResults(Model.AccountPhoneMark[] values, uint pageIndex, uint pageCount) : base(values, pageIndex, pageCount) { }
        public AccountPhoneMarkExecutionResults(Exception ex) : base(ex) { }
    }
}
