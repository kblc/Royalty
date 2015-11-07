using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;

namespace RoyaltyService.Services.Base
{
    [ServiceContract]
    public interface IBaseService
    {
        /// <summary>
        /// Set session lang
        /// </summary>
        /// <param name="fileId">File identifier</param>
        /// <returns></returns>
        [OperationContract(IsOneWay = true)]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.Bare, Method = "GET", ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "/ln/{codename}")]
        void ChangeLanguage(string codename);
    }
}
