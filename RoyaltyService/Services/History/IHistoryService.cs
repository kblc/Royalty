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
    public interface IHistoryService : Base.IBaseService
    {
        [OperationContract]
        [WebInvoke(UriTemplate = "/get", Method = "GET", BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json)]
        HistoryExecutionResult Get();

        [OperationContract]
        HistoryExecutionResult GetFrom(long identifier);

        [OperationContract]
        [WebInvoke(UriTemplate = "/get?from={identifier}", Method = "GET", BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json)]
        HistoryExecutionResult RESTGetFrom(string identifier);
    }
}
