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
    public interface IFileService
    {
        /// <summary>
        /// Get file info by file identifier
        /// </summary>
        /// <param name="fileId">File identifier</param>
        /// <returns>File info</returns>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.Bare, Method = "GET", ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "/info/{fileId}")]
        FileInfoExecutionResult Get(string fileId);

        /// <summary>
        /// Delete file by identifier
        /// </summary>
        /// <param name="fileId">File identifier</param>
        /// <returns></returns>
        [OperationContract(IsOneWay = true)]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.Bare, Method = "DELETE", ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "/file/{fileId}")]
        void Delete(string fileId);

        /// <summary>
        /// Set session lang
        /// </summary>
        /// <param name="fileId">File identifier</param>
        /// <returns></returns>
        [OperationContract(IsOneWay = true)]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.Bare, Method = "GET", ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "/ln/{codename}")]
        void ChangeLanguage(string codename);

        /// <summary>
        /// Get file infos by identifiers
        /// </summary>
        /// <param name="fileIds">File info identifiers</param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.Bare, Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "/file/info")]
        FileInfoExecutionResults GetRange(IEnumerable<string> fileIds);

        /// <summary>
        /// Get file source stream
        /// </summary>
        /// <param name="fileIdOrName">File identifier</param>
        /// <returns>Source stream</returns>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.Bare, Method = "GET", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "/file?get={fileIdOrName}")]
        System.IO.Stream GetSource(string fileIdOrName);

        /// <summary>
        /// Put file with source and parameters
        /// </summary>
        /// <param name="content">Source stream</param>
        /// <param name="fileName">File name</param>
        /// <param name="encoding">File encoding</param>
        /// <param name="mime">File mime type</param>
        /// <returns>File identifier</returns>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.Bare, Method = "PUT", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "/file")]
        FileInfoExecutionResult Put(System.IO.Stream content);

        /// <summary>
        /// Update file in database
        /// </summary>
        /// <param name="fileId">File identifier</param>
        /// <param name="fileName">New file name</param>
        /// <param name="encoding">New file encoding</param>
        /// <param name="mime">New mime type</param>
        /// <returns>File info</returns>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.Bare, Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "info/{fileId}?name={fileName}&encoding={encoding}&mime={mime}")]
        FileInfoExecutionResult Update(string fileId, string fileName, string encoding, string mime);
    }
}
