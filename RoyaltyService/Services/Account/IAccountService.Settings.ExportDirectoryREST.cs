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
        /// Add account settings export directory
        /// </summary>
        /// <param name="item">Account settings export directory</param>
        /// <returns>Account settings export directory info</returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "/accountSettingsExportDirectory", Method = "PUT", BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        AccountSettingsExportDirectoryExecutionResult RESTSettingsExportDirectoryPut(Model.AccountSettingsExportDirectory item);

        /// <summary>
        /// Update Account settings export directory
        /// </summary>
        /// <param name="item">Account settings export directory</param>
        /// <returns>Account settings export directory info</returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "/accountSettingsExportDirectory", Method = "POST", BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        AccountSettingsExportDirectoryExecutionResult RESTSettingsExportDirectoryUpdate(Model.AccountSettingsExportDirectory item);

        /// <summary>
        /// Remove Account settings export directory by identifier
        /// </summary>
        /// <param name="identifier">Account settings export directory identifier</param>
        [OperationContract]
        [WebInvoke(UriTemplate = "/accountSettingsExportDirectory", Method = "DELETE", BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        Model.LongExecutionResult RESTSettingsExportDirectoryRemove(string identifier);

        /// <summary>
        /// Remove Account settings export directory by identifiers
        /// </summary>
        /// <param name="identifiers">Account settings export directory identifiers</param>
        [OperationContract]
        [WebInvoke(UriTemplate = "/accountSettingsExportDirectory/delete", Method = "POST", BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        Model.LongExecutionResults RESTSettingsExportDirectoryRemoveRange(string[] identifiers);
    }
}
