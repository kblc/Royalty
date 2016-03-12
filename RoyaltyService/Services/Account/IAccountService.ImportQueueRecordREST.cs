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
        /// Get account import queue records
        /// </summary>
        /// <param name="accountId">Account identifier</param>
        /// <param name="from">Filter from date</param>
        /// <param name="to">Filter to date</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="itemsPerPage">Items per page</param>
        /// <returns>Account import queue records</returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "/account/{accountId}/importQueueRecords?pageIndex={pageIndex}&itemsPerPage={itemsPerPage}&from={from}&to={to}", Method = "GET", BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        ImportQueueRecordExecutionResults RESTGetImportQueueRecords(string accountId, string from, string to, string pageIndex, string itemsPerPage);

        /// <summary>
        /// Add account import queue record
        /// </summary>
        /// <param name="item">Account import queue record</param>
        /// <returns>Account import queue record info</returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "/importQueueRecords", Method = "PUT", BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        ImportQueueRecordExecutionResult RESTPutImportQueueRecord(Model.ImportQueueRecord item);

        /// <summary>
        /// Update account import queue record
        /// </summary>
        /// <param name="item">Account import queue record</param>
        /// <returns>Account import queue record info</returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "/importQueueRecords", Method = "POST", BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        ImportQueueRecordExecutionResult RESTUpdateImportQueueRecord(Model.ImportQueueRecord item);

        /// <summary>
        /// Remove account import queue record by identifier
        /// </summary>
        /// <param name="identifier">Account import queue record identifier</param>
        [OperationContract]
        [WebInvoke(UriTemplate = "/importQueueRecords", Method = "DELETE", BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        Model.GuidExecutionResult RESTRemovemportQueueRecord(string identifier);

        /// <summary>
        /// Remove account import queue records by identifiers
        /// </summary>
        /// <param name="identifiers">Account import queue record identifiers</param>
        [OperationContract]
        [WebInvoke(UriTemplate = "/importQueueRecords/delete", Method = "POST", BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        Model.GuidExecutionResults RESTRemoveImportQueueRecordRange(string[] identifiers);
    }
}
