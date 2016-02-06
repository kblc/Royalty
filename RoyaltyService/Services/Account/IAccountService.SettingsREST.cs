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
        [WebInvoke(UriTemplate = "/accountSettings", Method = "POST", BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        AccountSettingsExecutionResult RESTSettingsUpdate(Model.AccountSettings item);
    }
}
