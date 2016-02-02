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
        /// Add account settings import directory
        /// </summary>
        /// <param name="item">Account settings import directory</param>
        /// <returns>Account settings import directory info</returns>
        [OperationContract]
        AccountSettingsImportDirectoryExecutionResult SettingsImportDirectoryPut(Model.AccountSettingsImportDirectory item);

        /// <summary>
        /// Update Account settings import directory
        /// </summary>
        /// <param name="item">Account settings import directory</param>
        /// <returns>Account settings import directory info</returns>
        [OperationContract]
        AccountSettingsImportDirectoryExecutionResult SettingsImportDirectoryUpdate(Model.AccountSettingsImportDirectory item);

        /// <summary>
        /// Remove Account settings import directory by identifier
        /// </summary>
        /// <param name="identifier">Account settings import directory identifier</param>
        [OperationContract]
        Model.LongExecutionResult SettingsImportDirectoryRemove(long identifier);

        /// <summary>
        /// Remove Account settings import directory by identifiers
        /// </summary>
        /// <param name="identifiers">Account settings import directory identifiers</param>
        [OperationContract]
        Model.LongExecutionResults SettingsImportDirectoryRemoveRange(long[] identifiers);
    }
}
