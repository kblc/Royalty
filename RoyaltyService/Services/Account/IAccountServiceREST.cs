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
    [ServiceContract(SessionMode = SessionMode.Allowed)]
    public partial interface IAccountServiceREST : Base.IBaseService
    {
        /// <summary>
        /// Get all account info
        /// </summary>
        /// <returns>Account infos</returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "/account", Method = "GET", BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json)]
        AccountExecutionResults RESTGetAll();

        /// <summary>
        /// Get account info by identifier
        /// </summary>
        /// <param name="identifiers">Account identifier</param>
        /// <returns>Account info</returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "/account/{identifier}", Method = "GET", BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json)]
        AccountExecutionResult RESTGet(string identifier);

        /// <summary>
        /// Get account info by identifier
        /// </summary>
        /// <param name="identifiers">Account identifier</param>
        /// <returns>Account info</returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "/account/range", Method = "POST", BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        AccountExecutionResults RESTGetRange(string[] identifiers);

        /// <summary>
        /// Add account
        /// </summary>
        /// <param name="item">Account</param>
        /// <returns>Account info</returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "/account", Method = "PUT", BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        AccountExecutionResult RESTPut(Model.Account item);

        /// <summary>
        /// Update account
        /// </summary>
        /// <param name="item">Account</param>
        /// <returns>Account info</returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "/account", Method = "POST", BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        AccountExecutionResult RESTUpdate(Model.Account item);

        /// <summary>
        /// Remove account by identifier
        /// </summary>
        /// <param name="identifier">Account identifier</param>
        [OperationContract]
        [WebInvoke(UriTemplate = "/account", Method = "DELETE", BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        Model.GuidExecutionResult RESTRemove(string identifier);

        /// <summary>
        /// Remove accounts by identifiers
        /// </summary>
        /// <param name="identifier">Account identifiers</param>
        [OperationContract]
        [WebInvoke(UriTemplate = "/account/delete", Method = "POST", BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        Model.GuidExecutionResults RESTRemoveRange(string[] identifier);
    }
}
