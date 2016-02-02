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
        /// Get account settings column marks
        /// </summary>
        /// <returns>Available column marks</returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "/columnMarks", Method = "GET", BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json)]
        Result.AccountSettingsMarkExecutionResults RESTGetColumnMarks();
    }
}
