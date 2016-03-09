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
        /// Get account import queue records
        /// </summary>
        /// <param name="accountId">Account identifier</param>
        /// <param name="from">Filter from date</param>
        /// <param name="to">Filter to date</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="itemsPerPage">Items per page</param>
        /// <returns>Account import queue records</returns>
        [OperationContract]
        ImportQueueRecordExecutionResults GetImportQueueRecords(Guid accountId, DateTime? from, DateTime? to, uint pageIndex, uint itemsPerPage);

        /// <summary>
        /// Add account import queue record
        /// </summary>
        /// <param name="item">Account import queue record</param>
        /// <returns>Account import queue record info</returns>
        [OperationContract]
        ImportQueueRecordExecutionResult PutImportQueueRecord(Model.ImportQueueRecord item);

        /// <summary>
        /// Remove account import queue record by identifier
        /// </summary>
        /// <param name="identifier">Account import queue record identifier</param>
        [OperationContract]
        Model.GuidExecutionResult RemovemportQueueRecord(Guid identifier);

        /// <summary>
        /// Remove account import queue records by identifiers
        /// </summary>
        /// <param name="identifiers">Account import queue record identifiers</param>
        [OperationContract]
        Model.GuidExecutionResults RemoveImportQueueRecordRange(Guid[] identifiers);
    }
}
