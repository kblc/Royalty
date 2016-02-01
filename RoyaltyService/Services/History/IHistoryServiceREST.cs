using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;

namespace RoyaltyService.Services.History
{
    [ServiceContract(SessionMode = SessionMode.Allowed)]
    public interface IHistoryServiceREST : Base.IBaseService
    {
        [OperationContract]
        [WebInvoke(UriTemplate = "/get", Method = "GET", BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json)]
        HistoryExecutionResult RESTGet();

        [OperationContract]
        [WebInvoke(UriTemplate = "/get?from={identifier}", Method = "GET", BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json)]
        HistoryExecutionResult RESTGetFrom(string identifier);
    }
}
