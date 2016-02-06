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
    public partial interface IAccountServiceREST : Base.IBaseService
    {
        /// <summary>
        /// Get account series of numbers
        /// </summary>
        /// <param name="accountId">Account identifier</param>
        /// <returns>Account series of numbers</returns>
        [OperationContract]
        AccountSeriesOfNumbersRecordExecutionResults RESTGetSeriesOfNumbers(string accountId);

        /// <summary>
        /// Add account series of numbers
        /// </summary>
        /// <param name="item">Account series of numbers</param>
        /// <returns>Account series of numbers info</returns>
        [OperationContract]
        AccountSeriesOfNumbersRecordExecutionResult RESTPutSeriesOfNumbers(Model.AccountSeriesOfNumbersRecord item);

        /// <summary>
        /// Update account series of numbers
        /// </summary>
        /// <param name="item">Account series of numbers</param>
        /// <returns>Account series of numbers info</returns>
        [OperationContract]
        AccountSeriesOfNumbersRecordExecutionResult RESTUpdateSeriesOfNumbers(Model.AccountSeriesOfNumbersRecord item);

        /// <summary>
        /// Remove account series of numbers by identifier
        /// </summary>
        /// <param name="identifier">Account series of numbers identifier</param>
        [OperationContract]
        Model.LongExecutionResult RESTRemoveSeriesOfNumbers(string identifier);

        /// <summary>
        /// Remove accounts series of numbers by identifiers
        /// </summary>
        /// <param name="identifier">Account series of numbers identifiers</param>
        [OperationContract]
        Model.LongExecutionResults RESTRemoveSeriesOfNumbersRange(string[] identifier);
    }
}
