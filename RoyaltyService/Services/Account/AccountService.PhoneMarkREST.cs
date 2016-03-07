using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RoyaltyService.Model;
using System.ServiceModel;
using Helpers.Linq;
using Helpers;
using RoyaltyService.Services.Account.Result;

namespace RoyaltyService.Services.Account
{
    public partial class AccountService : Base.BaseService, IAccountServiceREST
    {
        public AccountPhoneMarkExecutionResults RESTGetAccountPhoneMark(string accountId, string filter, string pageIndex, string itemsPerPage)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    var id = GetGuidByString(accountId);
                    var pageIndexVal = GetLongByString(pageIndex);
                    var itemsPerPageVal = GetLongByString(itemsPerPage);

                    return GetAccountPhoneMark(id, filter, (uint)pageIndexVal, (uint)itemsPerPageVal);
                }
                catch (Exception ex)
                {
                    ex.Data.Add(nameof(accountId), accountId);
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    return new AccountPhoneMarkExecutionResults(ex);
                }
        }

        public AccountPhoneMarkExecutionResult RESTPutAccountPhoneMark(Model.AccountPhoneMark item) => RESTPutAccountPhoneMark(item);

        public LongExecutionResult RESTRemoveAccountPhoneMark(string identifier)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    var id = GetLongByString(identifier);
                    return RemoveAccountPhoneMark(id);
                }
                catch (Exception ex)
                {
                    ex.Data.Add(nameof(identifier), identifier);
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    return new LongExecutionResult(ex);
                }
        }

        public LongExecutionResults RESTRemoveAccountPhoneMarkRange(string[] identifiers)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    var ids = identifiers.Select(i => GetLongByString(i)).ToArray();
                    return RemoveAccountPhoneMarkRange(ids);
                }
                catch (Exception ex)
                {
                    ex.Data.Add(nameof(identifiers), identifiers.Concat(i => i, ","));
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    return new LongExecutionResults(ex);
                }
        }

        public AccountPhoneMarkExecutionResult RESTUpdateAccountPhoneMark(Model.AccountPhoneMark item) => UpdateAccountPhoneMark(item);
    }
}
