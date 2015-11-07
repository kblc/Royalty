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
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.PerSession)]
    public class FileService : Base.BaseService, IFileService
    {
        #region Initialize

        private const string DefaultFileName = "unknown.bin";

        static FileService()
        {
            Model.FileInfo.InitializeMap();
            //if (Config.Config.IsServicesConfigured && !string.IsNullOrWhiteSpace(Config.Config.ServicesConfig.FileServiceLogFileName))
            //{

            //}
        }

        #endregion
        #region File storage

        private RoyaltyFileStorage.IFileStorage fileStorage = null;
        protected RoyaltyFileStorage.IFileStorage FileStorage
        {
            get
            {
                if (fileStorage == null)
                {
                    fileStorage = new RoyaltyFileStorage.FileStorage();
                    fileStorage.Log += (s, e) => RaiseLog($"[FILESTORAGE] {e}");
                    fileStorage.VerboseLog = VerboseLog;
                }
                return fileStorage;
            }
        }

        #endregion

        private void SetOutputResponseHeaders(string mime, Encoding encoding)
        {
            //try
            //{ 
            //    System.ServiceModel.Web.WebOperationContext.Current.OutgoingResponse.Headers.Add(System.Net.HttpRequestHeader.ContentEncoding, );
            //}
            //catch { }
        }

        private void GetInputRequestHeaders(out string mime, out Encoding encoding)
        {
            mime = null;
            encoding = null;
            //try
            //{ 
            //    System.ServiceModel.Web.WebOperationContext.Current.OutgoingResponse.Headers.Add(System.Net.HttpRequestHeader.ContentEncoding, );
            //}
            //catch { }
        }

        #region Service contract implementation

        /// <summary>
        /// Delete file by identifier
        /// </summary>
        /// <param name="identifier">File identifier</param>
        public void Remove(Guid identifier)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}(fileId={identifier})", VerboseLog, RaiseLog))
                try
                {
                    using (var rep = GetNewRepository(logSession))
                    {
                        logSession.Add($"Try to get file with id = '{identifier}' from database...");
                        var file = rep.GetFile(identifier);
                        if (file != null)
                        {
                            logSession.Add($"File found: '{file}'.");
                            rep.Remove(file);
                            logSession.Add($"File with id = '{identifier}' deleted from database.");
                            FileStorage.FileDelete(file.FileID);
                            logSession.Add($"File with id = '{identifier}' deleted from file storage.");
                        }
                        else
                            throw new Exception(Properties.Resources.SERVICES_FILE_FileNotFound);
                    }
                }
                catch(Exception ex)
                {
                    ex.Data.Add(nameof(identifier), identifier);
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    throw;
                }
        }
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
        public FileInfoExecutionResult Get(Guid identifier)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    var res = GetRange(new Guid[] { identifier });
                    if (res.Exception != null)
                        throw res.Exception;

                    if (res.Values.Length != 1)
                        throw new Exception(Properties.Resources.SERVICES_FILE_FileNotFound);

                    return new FileInfoExecutionResult(res.Values.FirstOrDefault());
                }
                catch (Exception ex)
                {
                    ex.Data.Add(nameof(identifier), identifier);
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    return new FileInfoExecutionResult(ex);
                }
        }
        /// <summary>
        /// Get file info by file identifier
        /// </summary>
        /// <param name="identifier">File identifier</param>
        /// <returns>File info</returns>
        public FileInfoExecutionResult RESTGet(string identifier)
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
                    return new FileInfoExecutionResult(ex);
                }
        }

        /// <summary>
        /// Get file infos by identifiers
        /// </summary>
        /// <param name="identifiers">File info identifiers</param>
        /// <returns>Files info</returns>
        public FileInfoExecutionResults GetRange(Guid[] identifiers)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    using (var rep = GetNewRepository(logSession))
                    {
                        var res = rep.Get<RoyaltyRepository.Models.File>(f => identifiers.Contains(f.FileID))
                            .ToArray()
                            .Select(f => Mapper.Map<Model.FileInfo>(f))
                            .ToArray();
                        return new FileInfoExecutionResults(res);
                    }
                }
                catch (Exception ex)
                {
                    ex.Data.Add(nameof(identifiers), identifiers.Concat(i => i.ToString("N"), ","));
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    return new FileInfoExecutionResults(ex);
                }
        }
        /// <summary>
        /// Get file infos by identifiers
        /// </summary>
        /// <param name="identifiers">File info identifiers</param>
        /// <returns>Files info</returns>
        public FileInfoExecutionResults RESTGetRange(string[] identifiers)
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
                    return new FileInfoExecutionResults(ex);
                }
        }

        /// <summary>
        /// Get file source stream
        /// </summary>
        /// <param name="identifier">File identifier</param>
        /// <returns>Source stream</returns>
        public Stream GetSource(Guid identifier)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    using (var rep = GetNewRepository(logSession))
                    {
                        var file = rep.GetFile(identifier);
                        if (file != null)
                            SetOutputResponseHeaders(file.MimeType, file.Encoding);
                    }
                    logSession.Add($"Try to get file with id = '{identifier}' from data storage...");
                    return FileStorage.FileGet(identifier);
                }
                catch (Exception ex)
                {
                    ex.Data.Add(nameof(identifier), identifier);
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    throw;
                }
        }
        /// <summary>
        /// Get file source stream
        /// </summary>
        /// <param name="fileName">File name</param>
        /// <returns>Source stream</returns>
        public Stream GetSourceByName(string fileName)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    using (var rep = GetNewRepository(logSession))
                    {
                        var file = rep.GetFile(fileName);
                        if (file != null)
                            SetOutputResponseHeaders(file.MimeType, file.Encoding);
                    }
                    logSession.Add($"Try to get file with name = '{fileName}' from data storage...");
                    return FileStorage.FileGet(fileName);
                }
                catch (Exception ex)
                {
                    ex.Data.Add(nameof(fileName), fileName);
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    throw;
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
        /// Put file with source and parameters
        /// </summary>
        /// <param name="content">Source stream</param>
        /// <param name="file">New file information</param>
        /// <returns>File identifier</returns>
        public FileInfoExecutionResult Put(System.IO.Stream content)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    using (var rep = GetNewRepository(logSession))
                    {
                        string mimeType;
                        Encoding encoding;

                        GetInputRequestHeaders(out mimeType, out encoding);

                        var dbFile = rep.New<RoyaltyRepository.Models.File>((f) =>
                        {
                            f.FileName = DefaultFileName;
                            f.Encoding = encoding;
                            f.MimeType = string.IsNullOrWhiteSpace(mimeType) ? RoyaltyFileStorage.MimeTypes.GetMimeTypeFromFileName(f.FileName) : mimeType;
                        });

                        logSession.Add($"Try to save file to file storage...");
                        var fi = FileStorage.FilePut(dbFile.FileID, content, dbFile.FileName);

                        dbFile.FileSize = fi.Length;
                        dbFile.OriginalFileName = fi.Name;

                        logSession.Add($"Try to save file to database...");
                        rep.Add(dbFile);

                        return new FileInfoExecutionResult(Mapper.Map<Model.FileInfo>(dbFile));
                    }
                }
                catch (Exception ex)
                {
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    return new FileInfoExecutionResult(ex);
                }
        }

        /// <summary>
        /// Update file in database
        /// </summary>
        /// <param name="file">File to update</param>
        /// <returns>File info</returns>
        public FileInfoExecutionResult Update(Model.FileInfo item)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    using (var rep = GetNewRepository(logSession))
                    {
                        logSession.Add($"Try to get file with id = '{item.FileID}' from database...");
                        var id = GetGuidByString(item.FileID);

                        var dbFile = rep.GetFile(id);
                        if (dbFile == null)
                            throw new Exception(Properties.Resources.SERVICES_FILE_FileNotFound);

                        if (!string.IsNullOrEmpty(item.EncodingName))
                            dbFile.Encoding = Encoding.GetEncoding(item.EncodingName);

                        dbFile.MimeType = (!string.IsNullOrEmpty(item.MimeType)) ? item.MimeType : RoyaltyFileStorage.MimeTypes.GetMimeTypeFromFileName(dbFile.OriginalFileName);

                        if (!string.IsNullOrEmpty(item.FileName))
                        {
                            dbFile.FileName = System.IO.Path.GetFileName(item.FileName);

                            logSession.Add($"Try to rename file in file storage...");
                            var fi = FileStorage.FileRename(id, dbFile.FileName);
                            dbFile.OriginalFileName = fi.Name;
                        }

                        logSession.Add($"Try to update file in database...");
                        rep.SaveChanges();

                        return new FileInfoExecutionResult(Mapper.Map<Model.FileInfo>(dbFile));
                    }
                }
                catch (Exception ex)
                {
                    ex.Data.Add(nameof(item), item);
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    return new FileInfoExecutionResult(ex);
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
        public FileInfoExecutionResult RESTUpdate(string identifier, string fileName, string encoding, string mime)
        {
            return Update(new Model.FileInfo() { FileID = identifier, FileName = fileName, EncodingName = encoding, MimeType = mime });
        }

        #endregion
    }
}
