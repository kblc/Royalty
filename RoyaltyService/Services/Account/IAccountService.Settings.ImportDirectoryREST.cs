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
        /// Add account settings import directory
        /// </summary>
        /// <param name="item">Account settings import directory</param>
        /// <returns>Account settings import directory info</returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "/accountSettingsImportDirectory", Method = "PUT", BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        AccountSettingsImportDirectoryExecutionResult RESTSettingsImportDirectoryPut(Model.AccountSettingsImportDirectory item);

        /// <summary>
        /// Update Account settings import directory
        /// </summary>
        /// <param name="item">Account settings import directory</param>
        /// <returns>Account settings import directory info</returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "/accountSettingsImportDirectory", Method = "POST", BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        AccountSettingsImportDirectoryExecutionResult RESTSettingsImportDirectoryUpdate(Model.AccountSettingsImportDirectory item);

        /// <summary>
        /// Remove Account settings import directory by identifier
        /// </summary>
        /// <param name="identifier">Account settings import directory identifier</param>
        [OperationContract]
        [WebInvoke(UriTemplate = "/accountSettingsImportDirectory", Method = "DELETE", BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        Model.LongExecutionResult RESTSettingsImportDirectoryRemove(string identifier);

        /// <summary>
        /// Remove Account settings import directory by identifiers
        /// </summary>
        /// <param name="identifiers">Account settings import directory identifiers</param>
        [OperationContract]
        [WebInvoke(UriTemplate = "/accountSettingsImportDirectory/delete", Method = "POST", BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        Model.LongExecutionResults RESTSettingsImportDirectoryRemoveRange(string[] identifiers);
    }
}
