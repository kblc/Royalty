using RoyaltyService.Services.Account.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;

namespace RoyaltyService.Services.Account
{
    public partial interface IAccountService : Base.IBaseService
    {
        /// <summary>
        /// Update account settings
        /// </summary>
        /// <param name="item">Account settings</param>
        /// <returns>Account settings info</returns>
        [OperationContract]
        AccountSettingsExecutionResult SettingsUpdate(Model.AccountSettings item);
    }
}
