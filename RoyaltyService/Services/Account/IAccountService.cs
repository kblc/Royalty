using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;

namespace RoyaltyService.Services.Account
{
    [ServiceContract(SessionMode = SessionMode.Allowed)]
    public interface IAccountService : Base.IBaseService
    {
        /// <summary>
        /// Get all account info
        /// </summary>
        /// <returns>Account infos</returns>
        [OperationContract]
        AccountExecutionResults GetAll();

        /// <summary>
        /// Get account info by identifier
        /// </summary>
        /// <param name="identifier">Account identifier</param>
        /// <returns>Account info</returns>
        [OperationContract]
        AccountExecutionResult Get(Guid identifier);

        /// <summary>
        /// Get accounts info by identifiers
        /// </summary>
        /// <param name="identifiers">Account identifier</param>
        /// <returns>Account infos</returns>
        [OperationContract]
        AccountExecutionResults GetRange(Guid[] identifiers);

        /// <summary>
        /// Add account
        /// </summary>
        /// <param name="item">Account</param>
        /// <returns>Account info</returns>
        [OperationContract]
        AccountExecutionResult Put(Model.Account item);

        /// <summary>
        /// Update account
        /// </summary>
        /// <param name="item">Account</param>
        /// <returns>Account info</returns>
        [OperationContract]
        AccountExecutionResult Update(Model.Account item);

        /// <summary>
        /// Remove account by identifier
        /// </summary>
        /// <param name="identifier">Account identifier</param>
        [OperationContract]
        Model.GuidExecutionResult Remove(Guid identifier);

        /// <summary>
        /// Remove accounts by identifiers
        /// </summary>
        /// <param name="identifier">Account identifiers</param>
        [OperationContract]
        Model.GuidExecutionResults RemoveRange(Guid[] identifier);
    }
}
