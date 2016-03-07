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
        /// Get account phone marks
        /// </summary>
        /// <param name="accountId">Account identifier</param>
        /// <returns>Account phone marks</returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "/account/{accountId}/phoneMarks?pageIndex={pageIndex}&itemsPerPage={itemsPerPage}&filter={filter}", Method = "GET", BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        AccountPhoneMarkExecutionResults RESTGetAccountPhoneMark(string accountId, string filter, string pageIndex, string itemsPerPage);

        /// <summary>
        /// Add account phone marks
        /// </summary>
        /// <param name="item">Account phone marks</param>
        /// <returns>Account phone marks info</returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "/accountphoneMarks", Method = "PUT", BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        AccountPhoneMarkExecutionResult RESTPutAccountPhoneMark(Model.AccountPhoneMark item);

        /// <summary>
        /// Update account phone marks
        /// </summary>
        /// <param name="item">Account phone marks</param>
        /// <returns>Account phone marks info</returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "/accountphoneMarks", Method = "POST", BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        AccountPhoneMarkExecutionResult RESTUpdateAccountPhoneMark(Model.AccountPhoneMark item);

        /// <summary>
        /// Remove account phone marks by identifier
        /// </summary>
        /// <param name="identifier">Account phone marks identifier</param>
        [OperationContract]
        [WebInvoke(UriTemplate = "/accountphoneMarks", Method = "DELETE", BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        Model.LongExecutionResult RESTRemoveAccountPhoneMark(string identifier);

        /// <summary>
        /// Remove accounts phone marks by identifiers
        /// </summary>
        /// <param name="identifier">Account phone marks identifiers</param>
        [OperationContract]
        [WebInvoke(UriTemplate = "/accountphoneMarks/delete", Method = "POST", BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        Model.LongExecutionResults RESTRemoveAccountPhoneMarkRange(string[] identifier);
    }
}
