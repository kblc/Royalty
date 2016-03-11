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
        /// Get import queue record states
        /// </summary>
        /// <returns>Available import queue record states</returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "/importQueueRecordStates", Method = "GET", BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json)]
        Result.ImportQueueRecordStateResults RESTGetImportQueueRecordStates();
    }
}
