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
        public AccountDataRecordAdditionalColumnExecutionResults RESTGetAdditionalColumns(string accountId)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    var id = GetGuidByString(accountId);
                    return GetAdditionalColumns(id);
                }
                catch (Exception ex)
                {
                    ex.Data.Add(nameof(accountId), accountId);
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    return new AccountDataRecordAdditionalColumnExecutionResults(ex);
                }
        }

        public AccountDataRecordAdditionalColumnExecutionResult RESTPutAdditionalColumn(Model.AccountDataRecordAdditionalColumn item) => PutAdditionalColumn(item);

        public LongExecutionResult RESTRemoveAdditionalColumn(string identifier)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    var id = GetLongByString(identifier);
                    return RemoveAdditionalColumn(id);
                }
                catch (Exception ex)
                {
                    ex.Data.Add(nameof(identifier), identifier);
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    return new LongExecutionResult(ex);
                }
        }

        public LongExecutionResults RESTRemoveAdditionalColumnsRange(string[] identifiers)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    var ids = identifiers.Select(i => GetLongByString(i)).ToArray();
                    return RemoveAdditionalColumnsRange(ids);
                }
                catch (Exception ex)
                {
                    ex.Data.Add(nameof(identifiers), identifiers.Concat(i => i,","));
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    return new LongExecutionResults(ex);
                }
        }

        public AccountDataRecordAdditionalColumnExecutionResult RESTUpdateAdditionalColumn(Model.AccountDataRecordAdditionalColumn item) => UpdateAdditionalColumn(item);
    }
}
