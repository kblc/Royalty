using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RoyaltyService.Model;
using System.ServiceModel;
using Helpers.Linq;
using Helpers;

namespace RoyaltyService.Services.Account
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.PerSession)]
    public class AccountService : Base.BaseService, IAccountService
    {
        #region Static initializer

        static AccountService()
        {
            Model.Account.InitializeMap();
        }

        #endregion

        public AccountExecutionResults GetAll()
        {
            return GetRange(new Guid[] { });
        }

        public AccountExecutionResult Get(Guid identifier)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    var res = GetRange(new Guid[] { identifier });
                    if (res.Exception != null)
                        throw res.Exception;

                    var resValue = res.Values.SingleOrDefault();
                    if (resValue == null)
                        throw new Exception(Properties.Resources.SERVICES_FILE_AccountNotFound);

                    return new AccountExecutionResult(resValue);
                }
                catch (Exception ex)
                {
                    ex.Data.Add(nameof(identifier), identifier);
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    return new AccountExecutionResult(ex);
                }
        }

        public AccountExecutionResults GetRange(Guid[] identifiers)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    using (var rep = GetNewRepository(logSession))
                    {
                        logSession.Add($"Try to get account with ids = '{identifiers.Concat(i => i.ToString(), ",")}' from database...");
                        var items = identifiers.Length == 0
                            ? rep.Get<RoyaltyRepository.Models.Account>(asNoTracking: true).ToArray()
                            : rep.Get<RoyaltyRepository.Models.Account>(a => identifiers.Contains(a.AccountUID), asNoTracking: true).ToArray();
                        logSession.Add($"Accounts found: {items.Length}");

                        var res = items.Select(i => AutoMapper.Mapper.Map<Model.Account>(i)).ToArray(); 
                        return new AccountExecutionResults(res);
                    }
                }
                catch (Exception ex)
                {
                    ex.Data.Add(nameof(identifiers), identifiers.Concat(i => i.ToString(), ","));
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    return new AccountExecutionResults(ex);
                }
        }

        public AccountExecutionResult Put(Model.Account item)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    using (var rep = GetNewRepository(logSession))
                    {
                        var mappedDbItem = AutoMapper.Mapper.Map<RoyaltyRepository.Models.Account>(item);
                        var dbItem = rep.New<RoyaltyRepository.Models.Account>((a) => 
                        {
                            a.CopyObjectFrom(mappedDbItem, new string[] { nameof(a.AccountUID) });
                        });
                        var res = AutoMapper.Mapper.Map<Model.Account>(dbItem);
                        return new AccountExecutionResult(res);
                    }
                }
                catch (Exception ex)
                {
                    ex.Data.Add(nameof(item), item);
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    return new AccountExecutionResult(ex);
                }
        }

        public GuidExecutionResult Remove(Guid identifier)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    var res = RemoveRange(new Guid[] { identifier });
                    if (res.Exception != null)
                        throw res.Exception;

                    return new GuidExecutionResult(res.Values.FirstOrDefault());
                }
                catch (Exception ex)
                {
                    ex.Data.Add(nameof(identifier), identifier);
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    return new GuidExecutionResult(ex);
                }
        }

        public GuidExecutionResults RemoveRange(Guid[] identifiers)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    using (var rep = GetNewRepository(logSession))
                    {
                        logSession.Add($"Try to get account with id = '{identifiers}' from database...");
                        var itemsToRemove = rep.Get<RoyaltyRepository.Models.Account>(a => identifiers.Contains(a.AccountUID)).ToArray();
                        logSession.Add($"Accounts found for remove: {itemsToRemove.Length}");
                        rep.RemoveRange(itemsToRemove);
                        rep.SaveChanges();
                        return new GuidExecutionResults(itemsToRemove.Select(i => i.AccountUID).ToArray());
                    }
                }
                catch (Exception ex)
                {
                    ex.Data.Add(nameof(identifiers), identifiers.Concat(i => i.ToString(),","));
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    return new GuidExecutionResults(ex);
                }
        }

        public AccountExecutionResult RESTGet(string identifier)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    var id = GetGuidByString(identifier);
                    return Get(id);
                }
                catch (Exception ex)
                {
                    ex.Data.Add(nameof(identifier), identifier);
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    return new AccountExecutionResult(ex);
                }
        }

        public AccountExecutionResults RESTGetRange(string[] identifiers)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    var ids = identifiers.Select(i => GetGuidByString(i)).ToArray();
                    return GetRange(ids);
                }
                catch (Exception ex)
                {
                    ex.Data.Add(nameof(identifiers), identifiers.Concat(i => i, ","));
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    return new AccountExecutionResults(ex);
                }
        }

        public GuidExecutionResult RESTRemove(string identifier)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    var id = GetGuidByString(identifier);
                    return Remove(id);
                }
                catch (Exception ex)
                {
                    ex.Data.Add(nameof(identifier), identifier);
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    return new GuidExecutionResult(ex);
                }
        }

        public GuidExecutionResults RESTRemoveRange(string[] identifiers)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    var ids = identifiers.Select(i => GetGuidByString(i)).ToArray();
                    return RemoveRange(ids);
                }
                catch (Exception ex)
                {
                    ex.Data.Add(nameof(identifiers), identifiers.Concat(i => i,","));
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    return new GuidExecutionResults(ex);
                }
        }

        public AccountExecutionResult Update(Model.Account item)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    using (var rep = GetNewRepository(logSession))
                    {
                        var mappedDbItem = AutoMapper.Mapper.Map<RoyaltyRepository.Models.Account>(item);
                        var dbItem = rep.Get<RoyaltyRepository.Models.Account>(a => a.AccountUID == mappedDbItem.AccountUID).SingleOrDefault();

                        if (dbItem == null)
                            throw new Exception(Properties.Resources.SERVICES_FILE_AccountNotFound);

                        dbItem.CopyObjectFrom(mappedDbItem, new string[] { nameof(dbItem) });

                        var res = AutoMapper.Mapper.Map<Model.Account>(dbItem);
                        return new AccountExecutionResult(res);
                    }
                }
                catch (Exception ex)
                {
                    ex.Data.Add(nameof(item), item);
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    return new AccountExecutionResult(ex);
                }
        }
    }
}
