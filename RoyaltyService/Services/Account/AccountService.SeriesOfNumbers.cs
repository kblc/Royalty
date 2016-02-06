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
        public AccountSeriesOfNumbersRecordExecutionResults GetSeriesOfNumbers(Guid accountId)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    using (var rep = GetNewRepository(logSession))
                    {
                        var items = rep.Get<RoyaltyRepository.Models.AccountSeriesOfNumbersRecord>(a => a.AccountUID == accountId, asNoTracking: true)
                            .ToArray();
                        var res = items.Select(i => AutoMapper.Mapper.Map<Model.AccountSeriesOfNumbersRecord>(i)).ToArray();
                        return new AccountSeriesOfNumbersRecordExecutionResults(res);
                    }
                }
                catch (Exception ex)
                {
                    ex.Data.Add(nameof(accountId), accountId);
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    return new AccountSeriesOfNumbersRecordExecutionResults(ex);
                }
        }

        public AccountSeriesOfNumbersRecordExecutionResult PutSeriesOfNumbers(Model.AccountSeriesOfNumbersRecord item)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    using (var rep = GetNewRepository(logSession))
                    {
                        var dbItem = rep.New<RoyaltyRepository.Models.AccountSeriesOfNumbersRecord>(item);
                        rep.Add(dbItem);
                        var res = AutoMapper.Mapper.Map<Model.AccountSeriesOfNumbersRecord>(dbItem);
                        return new AccountSeriesOfNumbersRecordExecutionResult(res);
                    }
                }
                catch (Exception ex)
                {
                    ex.Data.Add(nameof(item), item);
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    return new AccountSeriesOfNumbersRecordExecutionResult(ex);
                }
        }

        public LongExecutionResult RemoveSeriesOfNumbers(long identifier)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    var res = RemoveSeriesOfNumbersRange(new long[] { identifier });
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

        public LongExecutionResults RemoveSeriesOfNumbersRange(long[] identifiers)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    using (var rep = GetNewRepository(logSession))
                    {
                        logSession.Add($"Try to get account series of numbers with ids = '{identifiers.Concat(i => i.ToString(), ",")}' from database...");
                        var itemsToRemove = rep.Get<RoyaltyRepository.Models.AccountSeriesOfNumbersRecord>(a => identifiers.Contains(a.AccountNumberSeriaRecordID)).ToArray();
                        logSession.Add($"Accounts ccount series of numbers found for remove: {itemsToRemove.Length}");
                        rep.RemoveRange(itemsToRemove);
                        rep.SaveChanges();
                        return new LongExecutionResults(itemsToRemove.Select(i => i.AccountNumberSeriaRecordID).ToArray());
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

        public AccountSeriesOfNumbersRecordExecutionResult UpdateSeriesOfNumbers(Model.AccountSeriesOfNumbersRecord item)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    using (var rep = GetNewRepository(logSession))
                    {
                        var dbItem = rep.Get<RoyaltyRepository.Models.AccountSeriesOfNumbersRecord>(a => a.AccountNumberSeriaRecordID == item.AccountNumberSeriaRecordID)
                            .SingleOrDefault();
                        if (dbItem == null)
                            throw new Exception(Properties.Resources.SERVICES_ACCOUNT_AccountSeriesOfNumbersRecordNotFound);

                        dbItem.CopyObjectFrom(item);
                        rep.SaveChanges();

                        var res = AutoMapper.Mapper.Map<Model.AccountSeriesOfNumbersRecord>(dbItem);
                        return new AccountSeriesOfNumbersRecordExecutionResult(res);
                    }
                }
                catch (Exception ex)
                {
                    ex.Data.Add(nameof(item), item);
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    return new AccountSeriesOfNumbersRecordExecutionResult(ex);
                }
        }
    }
}
