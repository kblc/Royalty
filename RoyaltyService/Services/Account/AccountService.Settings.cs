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
        public AccountSettingsExecutionResult SettingsUpdate(Model.AccountSettings item)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    using (var rep = GetNewRepository(logSession))
                    {
                        var dbItem = rep.Get<RoyaltyRepository.Models.AccountSettings>(a => a.AccountUID == item.AccountUID).SingleOrDefault();
                        if (dbItem == null)
                            throw new Exception(Properties.Resources.SERVICES_FILE_AccountNotFound);

                        UpdateAccountSettingsDbItemFromModelItem(dbItem, item, rep);
                        rep.SaveChanges();

                        var res = AutoMapper.Mapper.Map<Model.AccountSettings>(dbItem);
                        return new AccountSettingsExecutionResult(res);
                    }
                }
                catch (Exception ex)
                {
                    ex.Data.Add(nameof(item), item);
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    return new AccountSettingsExecutionResult(ex);
                }
        }
    }
}
