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
        /// Add account settings shedule time
        /// </summary>
        /// <param name="item">Account settings shedule time</param>
        /// <returns>Account settings shedule time info</returns>
        [OperationContract]
        AccountSettingsSheduleTimeExecutionResult SettingsSheduleTimePut(Model.AccountSettingsSheduleTime item);

        /// <summary>
        /// Update Account settings shedule time
        /// </summary>
        /// <param name="item">Account settings shedule time</param>
        /// <returns>Account settings shedule time info</returns>
        [OperationContract]
        AccountSettingsSheduleTimeExecutionResult SettingsSheduleTimeUpdate(Model.AccountSettingsSheduleTime item);

        /// <summary>
        /// Remove Account settings shedule time by identifier
        /// </summary>
        /// <param name="identifier">Account settings shedule time identifier</param>
        [OperationContract]
        Model.LongExecutionResult SettingsSheduleTimeRemove(long identifier);

        /// <summary>
        /// Remove Account settings shedule time by identifiers
        /// </summary>
        /// <param name="identifiers">Account settings shedule time identifiers</param>
        [OperationContract]
        Model.LongExecutionResults SettingsSheduleTimeRemoveRange(long[] identifiers);
    }
}
