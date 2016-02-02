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
        /// Add account settings export directory
        /// </summary>
        /// <param name="item">Account settings export directory</param>
        /// <returns>Account settings export directory info</returns>
        [OperationContract]
        AccountSettingsExportDirectoryExecutionResult SettingsExportDirectoryPut(Model.AccountSettingsExportDirectory item);

        /// <summary>
        /// Update Account settings export directory
        /// </summary>
        /// <param name="item">Account settings export directory</param>
        /// <returns>Account settings export directory info</returns>
        [OperationContract]
        AccountSettingsExportDirectoryExecutionResult SettingsExportDirectoryUpdate(Model.AccountSettingsExportDirectory item);

        /// <summary>
        /// Remove Account settings export directory by identifier
        /// </summary>
        /// <param name="identifier">Account settings export directory identifier</param>
        [OperationContract]
        Model.LongExecutionResult SettingsExportDirectoryRemove(long identifier);

        /// <summary>
        /// Remove Account settings export directory by identifiers
        /// </summary>
        /// <param name="identifiers">Account settings export directory identifiers</param>
        [OperationContract]
        Model.LongExecutionResults SettingsExportDirectoryRemoveRange(long[] identifiers);
    }
}
