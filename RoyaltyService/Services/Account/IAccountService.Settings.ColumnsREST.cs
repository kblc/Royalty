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
        /// Add account settings column
        /// </summary>
        /// <param name="item">Account settings column</param>
        /// <returns>Account settings column info</returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "/accountSettingsColumn", Method = "PUT", BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        AccountSettingsColumnsExecutionResult RESTSettingsColumnPut(Model.AccountSettingsColumn item);

        /// <summary>
        /// Update Account settings column
        /// </summary>
        /// <param name="item">Account settings column</param>
        /// <returns>Account settings column info</returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "/accountSettingsColumn", Method = "POST", BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        AccountSettingsColumnsExecutionResult RESTSettingsColumnUpdate(Model.AccountSettingsColumn item);

        /// <summary>
        /// Remove Account settings column by identifier
        /// </summary>
        /// <param name="identifier">Account settings column identifier</param>
        [OperationContract]
        [WebInvoke(UriTemplate = "/accountSettingsColumn", Method = "DELETE", BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        Model.LongExecutionResult RESTSettingsColumnRemove(string identifier);

        /// <summary>
        /// Remove Account settings columns by identifiers
        /// </summary>
        /// <param name="identifiers">Account settings column identifiers</param>
        [OperationContract]
        [WebInvoke(UriTemplate = "/accountSettingsColumn/delete", Method = "POST", BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        Model.LongExecutionResults RESTSettingsColumnRemoveRange(string[] identifiers);
    }
}
