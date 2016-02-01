using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;

namespace RoyaltyService.Services.Account
{
    public partial interface IAccountService : Base.IBaseService
    {
        /// <summary>
        /// Get account settings column types
        /// </summary>
        /// <returns>Available column types</returns>
        [OperationContract]
        Result.AccountSettingsColumnTypeExecutionResults GetColumnTypes();
    }
}
