using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;

namespace RoyaltyService.Services.History
{
    [ServiceContract(SessionMode = SessionMode.Allowed)]
    public interface IHistoryService : Base.IBaseService
    {
        [OperationContract]
        HistoryExecutionResult Get();

        [OperationContract]
        HistoryExecutionResult GetFrom(long identifier);
    }
}
