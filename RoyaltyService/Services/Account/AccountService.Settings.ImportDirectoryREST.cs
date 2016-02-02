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
        public AccountSettingsImportDirectoryExecutionResult RESTSettingsImportDirectoryPut(Model.AccountSettingsImportDirectory item) => SettingsImportDirectoryPut(item);

        public Model.LongExecutionResult RESTSettingsImportDirectoryRemove(string identifier)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    var id = GetLongByString(identifier);
                    return SettingsImportDirectoryRemove(id);
                }
                catch (Exception ex)
                {
                    ex.Data.Add(nameof(identifier), identifier);
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    return new LongExecutionResult(ex);
                }
        }

        public Model.LongExecutionResults RESTSettingsImportDirectoryRemoveRange(string[] identifiers)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    var ids = identifiers.Select(i => GetLongByString(i)).ToArray();
                    return SettingsImportDirectoryRemoveRange(ids);
                }
                catch (Exception ex)
                {
                    ex.Data.Add(nameof(identifiers), identifiers.Concat(i => i, ","));
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    return new LongExecutionResults(ex);
                }
        }

        public AccountSettingsImportDirectoryExecutionResult RESTSettingsImportDirectoryUpdate(Model.AccountSettingsImportDirectory item) => SettingsImportDirectoryUpdate(item);
    }
}
