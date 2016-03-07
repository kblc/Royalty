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
        /// Get account additional column
        /// </summary>
        /// <param name="accountId">Account identifier</param>
        /// <returns>Account additional column</returns>
        [OperationContract]
        AccountDataRecordAdditionalColumnExecutionResults GetAdditionalColumns(Guid accountId);

        /// <summary>
        /// Add account additional column
        /// </summary>
        /// <param name="item">Account additional column</param>
        /// <returns>Account additional column info</returns>
        [OperationContract]
        AccountDataRecordAdditionalColumnExecutionResult PutAdditionalColumn(Model.AccountDataRecordAdditionalColumn item);

        /// <summary>
        /// Update account additional column
        /// </summary>
        /// <param name="item">Account additional column</param>
        /// <returns>Account additional column info</returns>
        [OperationContract]
        AccountDataRecordAdditionalColumnExecutionResult UpdateAdditionalColumn(Model.AccountDataRecordAdditionalColumn item);

        /// <summary>
        /// Remove account additional column by identifier
        /// </summary>
        /// <param name="identifier">Account additional column identifier</param>
        [OperationContract]
        Model.LongExecutionResult RemoveAdditionalColumn(long identifier);

        /// <summary>
        /// Remove accounts additional column by identifiers
        /// </summary>
        /// <param name="identifier">Account additional column identifiers</param>
        [OperationContract]
        Model.LongExecutionResults RemoveAdditionalColumnsRange(long[] identifier);
    }
}
