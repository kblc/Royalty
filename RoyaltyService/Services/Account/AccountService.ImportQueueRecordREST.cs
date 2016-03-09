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
        public ImportQueueRecordExecutionResults RESTGetImportQueueRecords(string accountId, string from, string to, string pageIndex, string itemsPerPage)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    var id = GetGuidByString(accountId);
                    var pageIndexVal = GetLongByString(pageIndex);
                    var itemsPerPageVal = GetLongByString(itemsPerPage);

                    DateTime? fromDate = null;
                    DateTime fromDateValue;
                    if (DateTime.TryParse(from, out fromDateValue))
                        fromDate = fromDateValue;
                    DateTime? toDate = null;
                    DateTime toDateValue;
                    if (DateTime.TryParse(to, out toDateValue))
                        toDate = toDateValue;

                    return GetImportQueueRecords(id, fromDate, toDateValue, (uint)pageIndexVal, (uint)itemsPerPageVal);
                }
                catch (Exception ex)
                {
                    ex.Data.Add(nameof(accountId), accountId);
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    return new ImportQueueRecordExecutionResults(ex);
                }
        }

        public ImportQueueRecordExecutionResult RESTPutImportQueueRecord(Model.ImportQueueRecord item)
            => PutImportQueueRecord(item);

        public GuidExecutionResult RESTRemovemportQueueRecord(string identifier)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    var id = GetGuidByString(identifier);
                    return RemovemportQueueRecord(id);
                }
                catch (Exception ex)
                {
                    ex.Data.Add(nameof(identifier), identifier);
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    return new GuidExecutionResult(ex);
                }
        }

        public GuidExecutionResults RESTRemoveImportQueueRecordRange(string[] identifiers)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    var ids = identifiers.Select(i => GetGuidByString(i)).ToArray();
                    return RemoveImportQueueRecordRange(ids);
                }
                catch (Exception ex)
                {
                    ex.Data.Add(nameof(identifiers), identifiers.Concat(i => i, ","));
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    return new GuidExecutionResults(ex);
                }
        }
    }
}
