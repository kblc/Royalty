using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RoyaltyService.Services.History
{
    public partial class HistoryService : Base.BaseService, IHistoryServiceREST
    {
        public HistoryExecutionResult RESTGet() => Get();

        public HistoryExecutionResult RESTGetFrom(string identifier)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    var id = GetLongByString(identifier);
                    return GetFrom(id);
                }
                catch (Exception ex)
                {
                    ex.Data.Add(nameof(identifier), identifier);
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    return new HistoryExecutionResult(ex);
                }
        }
    }
}
