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
        public Result.AccountSettingsColumnTypeExecutionResults GetColumnTypes()
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    using (var rep = GetNewRepository(logSession))
                    {
                        var items = rep.Get<RoyaltyRepository.Models.ColumnType>(asNoTracking: true).ToArray();
                        logSession.Add($"Column types found: {items.Length}");

                        var res = items.Select(i => AutoMapper.Mapper.Map<Model.ColumnType>(i)).ToArray();
                        return new Result.AccountSettingsColumnTypeExecutionResults(res);
                    }
                }
                catch (Exception ex)
                {
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    return new Result.AccountSettingsColumnTypeExecutionResults(ex);
                }
        }
    }
}
