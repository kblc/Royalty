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
        public AccountPhoneMarkExecutionResults GetAccountPhoneMark(Guid accountId, string filter, uint pageIndex, uint itemsPerPage)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    using (var rep = GetNewRepository(logSession))
                    {
                        var totalCountWithFilter = 
                                rep.Get<RoyaltyRepository.Models.AccountPhoneMark>(a => a.AccountUID == accountId, new[] { "Phone" }, asNoTracking: true)
                                .OrderBy(i => i.Phone.PhoneNumber)
                                .AsQueryable();

                        if (!string.IsNullOrWhiteSpace(filter))
                            totalCountWithFilter = totalCountWithFilter
                                .Where(pm => pm.Phone.PhoneNumber.Contains(filter));

                        var itemsCount = totalCountWithFilter.Count();
                        var pageCount = (itemsCount / itemsPerPage) * itemsPerPage < itemsCount
                            ? (itemsCount / itemsPerPage) + 1
                            : (itemsCount / itemsPerPage);
                        pageIndex = Math.Min(pageIndex, (uint)pageCount - 1);
                        if (itemsCount == 0) pageCount = 0;
                        var items = totalCountWithFilter
                            .Skip((int)pageIndex * (int)itemsPerPage)
                            .Take((int)itemsPerPage)
                            .ToArray();
                        var res = items.Select(i => AutoMapper.Mapper.Map<Model.AccountPhoneMark>(i)).ToArray();
                        return new AccountPhoneMarkExecutionResults(res, pageIndex, (uint)pageCount);
                    }
                }
                catch (Exception ex)
                {
                    ex.Data.Add(nameof(accountId), accountId);
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    return new AccountPhoneMarkExecutionResults(ex);
                }
        }

        public AccountPhoneMarkExecutionResult PutAccountPhoneMark(Model.AccountPhoneMark item)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    using (var rep = GetNewRepository(logSession))
                    {
                        var dbItem = rep.New<RoyaltyRepository.Models.AccountPhoneMark>(item);
                        var phone = rep.Get<RoyaltyRepository.Models.Phone>(p => string.Compare(p.PhoneNumber, item.PhoneNumber, true) == 0).SingleOrDefault();
                        if (phone == null)
                        {
                            phone = rep.New<RoyaltyRepository.Models.Phone>((p) => { p.PhoneNumber = item.PhoneNumber; });
                            rep.Add(phone, saveAfterInsert: false);
                        }
                        dbItem.Phone = phone;
                        rep.Add(dbItem);
                        var res = AutoMapper.Mapper.Map<Model.AccountPhoneMark>(dbItem);
                        return new AccountPhoneMarkExecutionResult(res);
                    }
                }
                catch (Exception ex)
                {
                    ex.Data.Add(nameof(item), item);
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    return new AccountPhoneMarkExecutionResult(ex);
                }
        }

        public LongExecutionResult RemoveAccountPhoneMark(long identifier)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    var res = RemoveAccountPhoneMarkRange(new long[] { identifier });
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

        public LongExecutionResults RemoveAccountPhoneMarkRange(long[] identifiers)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    using (var rep = GetNewRepository(logSession))
                    {
                        logSession.Add($"Try to get account phone marks with ids = '{identifiers.Concat(i => i.ToString(), ",")}' from database...");
                        var itemsToRemove = rep.Get<RoyaltyRepository.Models.AccountPhoneMark>(a => identifiers.Contains(a.AccountPhoneMarkID)).ToArray();
                        logSession.Add($"Accounts phone marks found for remove: {itemsToRemove.Length}");
                        rep.RemoveRange(itemsToRemove);
                        rep.SaveChanges();
                        return new LongExecutionResults(itemsToRemove.Select(i => i.AccountPhoneMarkID).ToArray());
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

        public AccountPhoneMarkExecutionResult UpdateAccountPhoneMark(Model.AccountPhoneMark item)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    using (var rep = GetNewRepository(logSession))
                    {
                        var dbItem = rep.Get<RoyaltyRepository.Models.AccountPhoneMark>(a => a.AccountPhoneMarkID == item.AccountPhoneMarkID, new[] { nameof(RoyaltyRepository.Models.AccountPhoneMark.Phone) })
                            .SingleOrDefault();
                        if (dbItem == null)
                            throw new Exception(Properties.Resources.SERVICES_ACCOUNT_AccountPhoneMarkColumnNotFound);

                        dbItem.CopyObjectFrom(item);

                        var phone = rep.Get<RoyaltyRepository.Models.Phone>(p => string.Compare(p.PhoneNumber, item.PhoneNumber, true) == 0).SingleOrDefault();
                        if (phone == null)
                        {
                            phone = rep.New<RoyaltyRepository.Models.Phone>((p) => { p.PhoneNumber = item.PhoneNumber; });
                            rep.Add(phone, saveAfterInsert: false);
                            dbItem.PhoneID = default(long);
                            dbItem.Phone = phone;
                        }
                        else
                        {
                            if (dbItem.PhoneID != phone.PhoneID)
                                dbItem.PhoneID = phone.PhoneID;
                        }
                        rep.SaveChanges();

                        var res = AutoMapper.Mapper.Map<Model.AccountPhoneMark>(dbItem);
                        return new AccountPhoneMarkExecutionResult(res);
                    }
                }
                catch (Exception ex)
                {
                    ex.Data.Add(nameof(item), item);
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    return new AccountPhoneMarkExecutionResult(ex);
                }
        }
    }
}
