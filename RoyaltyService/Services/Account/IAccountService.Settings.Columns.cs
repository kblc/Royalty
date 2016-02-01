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
        /// Add account settings column
        /// </summary>
        /// <param name="item">Account settings column</param>
        /// <returns>Account settings column info</returns>
        [OperationContract]
        AccountSettingsColumnsExecutionResult SettingsColumnPut(Model.AccountSettingsColumn item);

        /// <summary>
        /// Update Account settings column
        /// </summary>
        /// <param name="item">Account settings column</param>
        /// <returns>Account settings column info</returns>
        [OperationContract]
        AccountSettingsColumnsExecutionResult SettingsColumnUpdate(Model.AccountSettingsColumn item);

        /// <summary>
        /// Remove Account settings column by identifier
        /// </summary>
        /// <param name="identifier">Account settings column identifier</param>
        [OperationContract]
        Model.LongExecutionResult SettingsColumnRemove(long identifier);

        /// <summary>
        /// Remove Account settings columns by identifiers
        /// </summary>
        /// <param name="identifiers">Account settings column identifiers</param>
        [OperationContract]
        Model.LongExecutionResults SettingsColumnRemoveRange(long[] identifiers);
    }
}
