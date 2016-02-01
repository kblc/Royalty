using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace RoyaltyService.Services.File
{
    [ServiceContract(SessionMode = SessionMode.Allowed)]
    public interface IFileService : Base.IBaseService
    {
        /// <summary>
        /// Get file info by file identifier
        /// </summary>
        /// <param name="identifier">File identifier</param>
        /// <returns>File info</returns>
        [OperationContract]
        FileExecutionResult Get(Guid identifier);

        /// <summary>
        /// Delete file by identifier
        /// </summary>
        /// <param name="identifier">File identifier</param>
        [OperationContract(IsOneWay = true)]
        void Remove(Guid identifier);

        /// <summary>
        /// Get file infos by identifiers
        /// </summary>
        /// <param name="identifiers">File info identifiers</param>
        /// <returns>Files info</returns>
        [OperationContract]
        FileExecutionResults GetRange(Guid[] identifiers);

        /// <summary>
        /// Get file source stream
        /// </summary>
        /// <param name="fileName">File name</param>
        /// <returns>Source stream</returns>
        [OperationContract]
        System.IO.Stream GetSourceByName(string fileName);

        /// <summary>
        /// Get file source stream
        /// </summary>
        /// <param name="identifier">File identifier</param>
        /// <returns>Source stream</returns>
        [OperationContract]
        System.IO.Stream GetSource(Guid identifier);

        /// <summary>
        /// Put file with source and parameters
        /// </summary>
        /// <param name="content">Source stream</param>
        /// <returns>File identifier</returns>
        [OperationContract]
        FileExecutionResult Put(System.IO.Stream content);

        /// <summary>
        /// Update file in database
        /// </summary>
        /// <param name="file">File to update</param>
        /// <returns>File info</returns>
        [OperationContract]
        FileExecutionResult Update(Model.File item);
    }
}
