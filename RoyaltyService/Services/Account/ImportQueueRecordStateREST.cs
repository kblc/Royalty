using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RoyaltyService.Model;
using System.ServiceModel;
using Helpers.Linq;
using Helpers;
using RoyaltyService.Services.Account.Result;

namespace RoyaltyService.Services.Account
{
    public partial class AccountService : Base.BaseService, IAccountServiceREST
    {
        public Result.ImportQueueRecordStateResults RESTGetImportQueueRecordStates() => GetImportQueueRecordStates();
    }
}
