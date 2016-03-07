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
        /// Get account additional column
        /// </summary>
        /// <param name="accountId">Account identifier</param>
        /// <returns>Account additional column</returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "/account/{accountId}/AdditionalColumns", Method = "GET", BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        AccountDataRecordAdditionalColumnExecutionResults RESTGetAdditionalColumns(string accountId);

        /// <summary>
        /// Add account additional column
        /// </summary>
        /// <param name="item">Account additional column</param>
        /// <returns>Account additional column info</returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "/accountAdditionalColumns", Method = "PUT", BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        AccountDataRecordAdditionalColumnExecutionResult RESTPutAdditionalColumn(Model.AccountDataRecordAdditionalColumn item);

        /// <summary>
        /// Update account additional column
        /// </summary>
        /// <param name="item">Account additional column</param>
        /// <returns>Account additional column info</returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "/accountAdditionalColumns", Method = "POST", BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        AccountDataRecordAdditionalColumnExecutionResult RESTUpdateAdditionalColumn(Model.AccountDataRecordAdditionalColumn item);

        /// <summary>
        /// Remove account additional column by identifier
        /// </summary>
        /// <param name="identifier">Account additional column identifier</param>
        [OperationContract]
        [WebInvoke(UriTemplate = "/account/AdditionalColumns", Method = "DELETE", BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        Model.LongExecutionResult RESTRemoveAdditionalColumn(string identifier);

        /// <summary>
        /// Remove accounts additional column by identifiers
        /// </summary>
        /// <param name="identifier">Account additional column identifiers</param>
        [OperationContract]
        [WebInvoke(UriTemplate = "/accountAdditionalColumns/delete", Method = "POST", BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        Model.LongExecutionResults RESTRemoveAdditionalColumnsRange(string[] identifier);
    }
}
