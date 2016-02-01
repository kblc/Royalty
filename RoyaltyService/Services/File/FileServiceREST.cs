using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using RoyaltyService.Model;
using Helpers;
using Helpers.Linq;
using AutoMapper;
using System.Globalization;
using System.Threading;

namespace RoyaltyService.Services.File
{
    public partial class FileService : Base.BaseService, IFileServiceREST
    {
        /// <summary>
        /// Delete file by identifier
        /// </summary>
        /// <param name="identifier">File identifier</param>
        public void RESTRemove(string identifier)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    var id = GetGuidByString(identifier);
                    Remove(id);
                }
                catch (Exception ex)
                {
                    ex.Data.Add(nameof(identifier), identifier);
                    logSession.Enabled = true;
                    logSession.Add(ex);
                }
        }

        /// <summary>
        /// Get file info by file identifier
        /// </summary>
        /// <param name="identifier">File identifier</param>
        /// <returns>File info</returns>
        public FileExecutionResult RESTGet(string identifier)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    var id = GetGuidByString(identifier);
                    return Get(id);
                }
                catch (Exception ex)
                {
                    ex.Data.Add(nameof(identifier), identifier);
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    return new FileExecutionResult(ex);
                }
        }

        /// <summary>
        /// Get file infos by identifiers
        /// </summary>
        /// <param name="identifiers">File info identifiers</param>
        /// <returns>Files info</returns>
        public FileExecutionResults RESTGetRange(string[] identifiers)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    var valiIdentifiers = identifiers.Select(i => GetGuidByString(i)).ToArray();
                    return GetRange(valiIdentifiers);
                }
                catch (Exception ex)
                {
                    ex.Data.Add(nameof(identifiers), identifiers.Concat(i => i ?? "NULL", ","));
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    return new FileExecutionResults(ex);
                }
        }

        /// <summary>
        /// Get file source stream
        /// </summary>
        /// <param name="fileIdOrName">File identifier or name</param>
        /// <returns>Source stream</returns>
        public Stream RESTGetSource(string fileIdOrName)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    logSession.Add($"Try to get file with id or name = '{fileIdOrName}' from database...");
                    var fileId = TryGetGuidByString(fileIdOrName);
                    if (fileId.HasValue)
                        return GetSource(fileId.Value);
                    return GetSourceByName(fileIdOrName);
                }
                catch (Exception ex)
                {
                    ex.Data.Add(nameof(fileIdOrName), fileIdOrName);
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    throw;
                }
        }

        /// <summary>
        /// Update file in database
        /// </summary>
        /// <param name="identifier">File identifier</param>
        /// <param name="fileName">New file name</param>
        /// <param name="encoding">New file encoding</param>
        /// <param name="mime">New mime type</param>
        /// <returns>File info</returns>
        public FileExecutionResult RESTUpdate(Model.File item) => Update(item);

        /// <summary>
        /// Put file with source and parameters
        /// </summary>
        /// <param name="content">Source stream</param>
        /// <returns>File identifier</returns>
        public FileExecutionResult RESTPut(System.IO.Stream content) => Put(content);
    }
}
