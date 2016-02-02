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
        public AccountSettingsSheduleTimeExecutionResult SettingsSheduleTimePut(Model.AccountSettingsSheduleTime item)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    var res = Put<Model.AccountSettingsSheduleTime, RoyaltyRepository.Models.AccountSettingsSheduleTime>(item, (a, mappedDbItem, rep) => 
                    {
                        a.CopyObjectFrom(mappedDbItem, new string[] { nameof(a.AccountSettingsSheduleTimeID) });
                    }, logSession);
                    return new AccountSettingsSheduleTimeExecutionResult(res);
                }
                catch (Exception ex)
                {
                    ex.Data.Add(nameof(item), item);
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    return new AccountSettingsSheduleTimeExecutionResult(ex);
                }
        }

        public LongExecutionResult SettingsSheduleTimeRemove(long identifier)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    var res = SettingsSheduleTimeRemoveRange(new long[] { identifier });
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

        public LongExecutionResults SettingsSheduleTimeRemoveRange(long[] identifiers)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    using (var rep = GetNewRepository(logSession))
                    {
                        logSession.Add($"Try to get account settings columns with id = '{identifiers}' from database...");
                        var itemsToRemove = rep.Get<RoyaltyRepository.Models.AccountSettingsSheduleTime>(a => identifiers.Contains(a.AccountSettingsSheduleTimeID)).ToArray();
                        logSession.Add($"Account settings columns found for remove: {itemsToRemove.Length}");
                        rep.RemoveRange(itemsToRemove);
                        return new LongExecutionResults(itemsToRemove.Select(i => i.AccountSettingsSheduleTimeID).ToArray());
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

        public AccountSettingsSheduleTimeExecutionResult SettingsSheduleTimeUpdate(Model.AccountSettingsSheduleTime item)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    using (var rep = GetNewRepository(logSession))
                    {
                        var mappedDbItem = AutoMapper.Mapper.Map<RoyaltyRepository.Models.AccountSettingsSheduleTime>(item);
                        var dbItem = rep.Get<RoyaltyRepository.Models.AccountSettingsSheduleTime>(a => a.AccountSettingsSheduleTimeID == mappedDbItem.AccountSettingsSheduleTimeID).SingleOrDefault();

                        if (dbItem == null)
                            throw new Exception(Properties.Resources.SERVICES_FILE_AccountNotFound);

                        dbItem.CopyObjectFrom(mappedDbItem, new string[] { });
                        rep.SaveChanges();

                        var res = AutoMapper.Mapper.Map<Model.AccountSettingsSheduleTime>(dbItem);
                        return new AccountSettingsSheduleTimeExecutionResult(res);
                    }
                }
                catch (Exception ex)
                {
                    ex.Data.Add(nameof(item), item);
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    return new AccountSettingsSheduleTimeExecutionResult(ex);
                }
        }
    }
}
