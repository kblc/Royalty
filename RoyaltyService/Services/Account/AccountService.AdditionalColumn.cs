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
        public AccountDataRecordAdditionalColumnExecutionResults GetAdditionalColumns(Guid accountId)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    using (var rep = GetNewRepository(logSession))
                    {
                        var items = rep.Get<RoyaltyRepository.Models.AccountDataRecordAdditionalColumn>(a => a.AccountUID == accountId, asNoTracking: true)
                            .ToArray();
                        var res = items.Select(i => AutoMapper.Mapper.Map<Model.AccountDataRecordAdditionalColumn>(i)).ToArray();
                        return new AccountDataRecordAdditionalColumnExecutionResults(res);
                    }
                }
                catch (Exception ex)
                {
                    ex.Data.Add(nameof(accountId), accountId);
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    return new AccountDataRecordAdditionalColumnExecutionResults(ex);
                }
        }

        public AccountDataRecordAdditionalColumnExecutionResult PutAdditionalColumn(Model.AccountDataRecordAdditionalColumn item)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    using (var rep = GetNewRepository(logSession))
                    {
                        var dbItem = rep.New<RoyaltyRepository.Models.AccountDataRecordAdditionalColumn>(item);
                        //Additional logic inside
                        rep.AccountDataRecordAdditionalColumnAdd(dbItem);
                        var res = AutoMapper.Mapper.Map<Model.AccountDataRecordAdditionalColumn>(dbItem);
                        return new AccountDataRecordAdditionalColumnExecutionResult(res);
                    }
                }
                catch (Exception ex)
                {
                    ex.Data.Add(nameof(item), item);
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    return new AccountDataRecordAdditionalColumnExecutionResult(ex);
                }
        }

        public LongExecutionResult RemoveAdditionalColumn(long identifier)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    var res = RemoveAdditionalColumnsRange(new long[] { identifier });
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

        public LongExecutionResults RemoveAdditionalColumnsRange(long[] identifiers)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    using (var rep = GetNewRepository(logSession))
                    {
                        logSession.Add($"Try to get account data additional columns with ids = '{identifiers.Concat(i => i.ToString(), ",")}' from database...");
                        var itemsToRemove = rep.Get<RoyaltyRepository.Models.AccountDataRecordAdditionalColumn>(a => identifiers.Contains(a.AccountDataRecordAdditionalColumnID)).ToArray();
                        logSession.Add($"Accounts data additional columns found for remove: {itemsToRemove.Length}");

                        //Additional deletion logic
                        rep.AccountDataRecordAdditionalColumnRemove(itemsToRemove);

                        //rep.RemoveRange(itemsToRemove);
                        //rep.SaveChanges();
                        return new LongExecutionResults(itemsToRemove.Select(i => i.AccountDataRecordAdditionalColumnID).ToArray());
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

        public AccountDataRecordAdditionalColumnExecutionResult UpdateAdditionalColumn(Model.AccountDataRecordAdditionalColumn item)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    using (var rep = GetNewRepository(logSession))
                    {
                        var dbItem = rep.Get<RoyaltyRepository.Models.AccountDataRecordAdditionalColumn>(a => a.AccountDataRecordAdditionalColumnID == item.AccountDataRecordAdditionalColumnID)
                            .SingleOrDefault();
                        if (dbItem == null)
                            throw new Exception(Properties.Resources.SERVICES_ACCOUNT_AccountDataRecordAdditionalColumnNotFound);

                        dbItem.CopyObjectFrom(item);
                        rep.SaveChanges();

                        var res = AutoMapper.Mapper.Map<Model.AccountDataRecordAdditionalColumn>(dbItem);
                        return new AccountDataRecordAdditionalColumnExecutionResult(res);
                    }
                }
                catch (Exception ex)
                {
                    ex.Data.Add(nameof(item), item);
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    return new AccountDataRecordAdditionalColumnExecutionResult(ex);
                }
        }
    }
}
