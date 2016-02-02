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
    public partial interface IAccountServiceREST : Base.IBaseService
    {
        /// <summary>
        /// Add account settings shedule time
        /// </summary>
        /// <param name="item">Account settings shedule time</param>
        /// <returns>Account settings shedule time info</returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "/accountSettingsSheduleTime", Method = "PUT", BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        AccountSettingsSheduleTimeExecutionResult RESTSettingsSheduleTimePut(Model.AccountSettingsSheduleTime item);

        /// <summary>
        /// Update Account settings shedule time
        /// </summary>
        /// <param name="item">Account settings shedule time</param>
        /// <returns>Account settings shedule time info</returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "/accountSettingsSheduleTime", Method = "POST", BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        AccountSettingsSheduleTimeExecutionResult RESTSettingsSheduleTimeUpdate(Model.AccountSettingsSheduleTime item);

        /// <summary>
        /// Remove Account settings shedule time by identifier
        /// </summary>
        /// <param name="identifier">Account settings shedule time identifier</param>
        [OperationContract]
        [WebInvoke(UriTemplate = "/accountSettingsSheduleTime", Method = "DELETE", BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        Model.LongExecutionResult RESTSettingsSheduleTimeRemove(string identifier);

        /// <summary>
        /// Remove Account settings shedule time by identifiers
        /// </summary>
        /// <param name="identifiers">Account settings shedule time identifiers</param>
        [OperationContract]
        [WebInvoke(UriTemplate = "/accountSettingsSheduleTime/delete", Method = "POST", BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        Model.LongExecutionResults RESTSettingsSheduleTimeRemoveRange(string[] identifiers);
    }
}
