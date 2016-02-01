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
    public partial class AccountService : Base.BaseService, IAccountService
    {
        public AccountSettingsColumnsExecutionResult SettingsColumnPut(Model.AccountSettingsColumn item)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    var res = Put<Model.AccountSettingsColumn, RoyaltyRepository.Models.AccountSettingsColumn>(item, (a, mappedDbItem, rep) => 
                    {
                        a.CopyObjectFrom(mappedDbItem, new string[] { nameof(a.AccountSettingsColumnID) });
                    }, logSession);
                    return new AccountSettingsColumnsExecutionResult(res);
                }
                catch (Exception ex)
                {
                    ex.Data.Add(nameof(item), item);
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    return new AccountSettingsColumnsExecutionResult(ex);
                }
        }

        public LongExecutionResult SettingsColumnRemove(long identifier)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    var res = SettingsColumnRemoveRange(new long[] { identifier });
                    if (res.Exception != null)
                        throw res.Exception;

                    return new LongExecutionResult(res.Values.FirstOrDefault());
                }
                catch (Exception ex)
                {
                    ex.Data.Add(nameof(identifier), identifier);
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    return new LongExecutionResult(ex);
                }
        }

        public LongExecutionResults SettingsColumnRemoveRange(long[] identifiers)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    using (var rep = GetNewRepository(logSession))
                    {
                        logSession.Add($"Try to get account settings columns with id = '{identifiers}' from database...");
                        var itemsToRemove = rep.Get<RoyaltyRepository.Models.AccountSettingsColumn>(a => identifiers.Contains(a.AccountSettingsColumnID)).ToArray();
                        logSession.Add($"Account settings columns found for remove: {itemsToRemove.Length}");
                        rep.RemoveRange(itemsToRemove);
                        return new LongExecutionResults(itemsToRemove.Select(i => i.AccountSettingsColumnID).ToArray());
                    }
                }
                catch (Exception ex)
                {
                    ex.Data.Add(nameof(identifiers), identifiers.Concat(i => i.ToString(),","));
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    return new LongExecutionResults(ex);
                }
        }

        public AccountSettingsColumnsExecutionResult SettingsColumnUpdate(Model.AccountSettingsColumn item)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    using (var rep = GetNewRepository(logSession))
                    {
                        var mappedDbItem = AutoMapper.Mapper.Map<RoyaltyRepository.Models.AccountSettingsColumn>(item);
                        var dbItem = rep.Get<RoyaltyRepository.Models.AccountSettingsColumn>(a => a.AccountSettingsColumnID == mappedDbItem.AccountSettingsColumnID).SingleOrDefault();

                        if (dbItem == null)
                            throw new Exception(Properties.Resources.SERVICES_FILE_AccountNotFound);

                        dbItem.CopyObjectFrom(mappedDbItem, new string[] { });
                        rep.SaveChanges();

                        var res = AutoMapper.Mapper.Map<Model.AccountSettingsColumn>(dbItem);
                        return new AccountSettingsColumnsExecutionResult(res);
                    }
                }
                catch (Exception ex)
                {
                    ex.Data.Add(nameof(item), item);
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    return new AccountSettingsColumnsExecutionResult(ex);
                }
        }
    }
}
