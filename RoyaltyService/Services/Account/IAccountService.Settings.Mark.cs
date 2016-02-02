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
        /// Get account settings column marks
        /// </summary>
        /// <returns>Available column marks</returns>
        [OperationContract]
        Result.AccountSettingsMarkExecutionResults GetColumnMarks();
    }
}
