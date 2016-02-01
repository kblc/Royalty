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
        /// Get account settings column types
        /// </summary>
        /// <returns>Available column types</returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "/columnTypes", Method = "GET", BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json)]
        Result.AccountSettingsColumnTypeExecutionResults RESTGetColumnTypes();
    }
}
