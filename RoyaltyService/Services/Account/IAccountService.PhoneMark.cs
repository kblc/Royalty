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
    public partial interface IAccountService : Base.IBaseService
    {
        /// <summary>
        /// Get account phone marks
        /// </summary>
        /// <param name="accountId">Account identifier</param>
        /// <returns>Account phone marks</returns>
        [OperationContract]
        AccountPhoneMarkExecutionResults GetAccountPhoneMark(Guid accountId, string filter, uint pageIndex, uint itemsPerPage);

        /// <summary>
        /// Add account phone marks
        /// </summary>
        /// <param name="item">Account phone marks</param>
        /// <returns>Account phone marks info</returns>
        [OperationContract]
        AccountPhoneMarkExecutionResult PutAccountPhoneMark(Model.AccountPhoneMark item);

        /// <summary>
        /// Update account phone marks
        /// </summary>
        /// <param name="item">Account phone marks</param>
        /// <returns>Account phone marks info</returns>
        [OperationContract]
        AccountPhoneMarkExecutionResult UpdateAccountPhoneMark(Model.AccountPhoneMark item);

        /// <summary>
        /// Remove account phone marks by identifier
        /// </summary>
        /// <param name="identifier">Account phone marks identifier</param>
        [OperationContract]
        Model.LongExecutionResult RemoveAccountPhoneMark(long identifier);

        /// <summary>
        /// Remove accounts phone marks by identifiers
        /// </summary>
        /// <param name="identifier">Account phone marks identifiers</param>
        [OperationContract]
        Model.LongExecutionResults RemoveAccountPhoneMarkRange(long[] identifier);
    }
}
