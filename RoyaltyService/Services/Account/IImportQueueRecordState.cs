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
        /// Get import queue record states
        /// </summary>
        /// <returns>Available import queue record states</returns>
        [OperationContract]
        Result.ImportQueueRecordStateResults GetImportQueueRecordStates();
    }
}
