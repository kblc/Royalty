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
    public partial class FileService : Base.BaseService, IFileService
    {
        #region Initialize

        private const string DefaultFileName = Config.ServicesConfigSection.DefaultDefaultFileName;

        static FileService()
        {
            Model.File.InitializeMap();
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

        /// <summary>
        /// Set some header for output response stream
        /// </summary>
        /// <param name="mime">File mime type</param>
        /// <param name="encoding">File encoding</param>
        /// <param name="fileName">File name</param>
        private void SetOutputResponseHeaders(string mime, Encoding encoding, string fileName, Helpers.Log.SessionInfo upperLogSession)
        {
            if (upperLogSession == null)
                throw new ArgumentNullException(nameof(upperLogSession));

            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, ss => ss.ToList().ForEach(s => upperLogSession.Add(s))))
                try
                {
                    var webContext = System.ServiceModel.Web.WebOperationContext.Current;
                    if (webContext != null)
                    {
                        webContext.OutgoingResponse.ContentType = new string[] { mime, encoding?.WebName }.Where(s => !string.IsNullOrWhiteSpace(s)).Concat(s => s, "; ");
                        if (!string.IsNullOrWhiteSpace(fileName))
                            webContext.OutgoingResponse.Headers.Add("Content-Disposition", $"attachment; filename={fileName}");
                    }
                }
                catch (Exception ex)
                {
                    logSession.Add(ex);
                    logSession.Enabled = true;
                    upperLogSession.Enabled = true;
                }
        }

        /// <summary>
        /// Get parameters from input request stream
        /// </summary>
        /// <param name="mime">File mime type</param>
        /// <param name="encoding">File encoding</param>
        /// <param name="fileName">File name</param>
        private void GetInputRequestHeaders(out string mime, out Encoding encoding, out string fileName, Helpers.Log.SessionInfo upperLogSession)
        {
            if (upperLogSession == null)
                throw new ArgumentNullException(nameof(upperLogSession));

            mime = null;
            encoding = null;
            fileName = null;
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, ss => ss.ToList().ForEach(s => upperLogSession.Add(s))))
                try
                {
                    var webContext = System.ServiceModel.Web.WebOperationContext.Current;
                    if (webContext != null)
                    {
                        var ct = webContext.IncomingRequest.ContentType.Split(new char[] { ';' }, StringSplitOptions.None).Select(i => i.Trim());
                        mime = ct.FirstOrDefault();
                        var encName = ct.Skip(1).FirstOrDefault();
                        if (!string.IsNullOrWhiteSpace(encName))
                            try { encoding = Encoding.GetEncoding(encName); } catch { }

                        var cd = webContext.IncomingRequest.Headers.Get("Content-Disposition");
                        if (!string.IsNullOrWhiteSpace(cd))
                        {
                            var fName = cd.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).FirstOrDefault(i => i.ToLower().StartsWith("filename="));
                            fileName = fName.Substring(fName.IndexOf("=") + 1);
                        }
                    }
                }
                catch (Exception ex)
                {
                    logSession.Add(ex);
                    logSession.Enabled = true;
                    upperLogSession.Enabled = true;
                }
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
                        var file = rep.Get<RoyaltyRepository.Models.File>(i => i.FileUID == identifier).FirstOrDefault();
                        if (file != null)
                        {
                            logSession.Add($"File found: '{file}'.");
                            rep.Remove(file);
                            logSession.Add($"File with id = '{identifier}' deleted from database.");
                            FileStorage.FileDelete(file.FileUID);
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
        /// Get file info by file identifier
        /// </summary>
        /// <param name="identifier">File identifier</param>
        /// <returns>File info</returns>
        public FileExecutionResult Get(Guid identifier)
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

                    return new FileExecutionResult(res.Values.First());
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
        public FileExecutionResults GetRange(Guid[] identifiers)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    using (var rep = GetNewRepository(logSession))
                    {
                        var res = rep.Get<RoyaltyRepository.Models.File>(f => identifiers.Contains(f.FileUID))
                            .ToArray()
                            .Select(f => Mapper.Map<Model.File>(f))
                            .ToArray();
                        return new FileExecutionResults(res);
                    }
                }
                catch (Exception ex)
                {
                    ex.Data.Add(nameof(identifiers), identifiers.Concat(i => i.ToString("N"), ","));
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    return new FileExecutionResults(ex);
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
                        var file = rep.Get<RoyaltyRepository.Models.File>(i => i.FileUID == identifier).FirstOrDefault();
                        if (file != null)
                            SetOutputResponseHeaders(file.MimeType, file.Encoding, file.FileName, logSession);
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
                            SetOutputResponseHeaders(file.MimeType, file.Encoding, file.FileName, logSession);
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
        /// Put file with source and parameters
        /// </summary>
        /// <param name="content">Source stream</param>
        /// <param name="file">New file information</param>
        /// <returns>File identifier</returns>
        public FileExecutionResult Put(System.IO.Stream content)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    using (var rep = GetNewRepository(logSession))
                    {
                        string mimeType;
                        Encoding encoding;
                        string fileName;

                        GetInputRequestHeaders(out mimeType, out encoding, out fileName, logSession);

                        var dbFile = rep.NewFile((f) =>
                        {
                            f.FileName = string.IsNullOrWhiteSpace(fileName) ? DefaultFileName : fileName;
                            f.Encoding = encoding;
                            f.MimeType = string.IsNullOrWhiteSpace(mimeType) ? RoyaltyFileStorage.MimeTypes.GetMimeTypeFromFileName(f.FileName) : mimeType;
                        });

                        logSession.Add($"Try to save file to file storage...");
                        var fi = FileStorage.FilePut(dbFile.FileUID, content, dbFile.FileName);

                        dbFile.FileSize = fi.Length;
                        dbFile.OriginalFileName = fi.Name;

                        logSession.Add($"Try to save file to database...");
                        rep.Add(dbFile);

                        return new FileExecutionResult(Mapper.Map<Model.File>(dbFile));
                    }
                }
                catch (Exception ex)
                {
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    return new FileExecutionResult(ex);
                }
        }

        /// <summary>
        /// Update file in database
        /// </summary>
        /// <param name="file">File to update</param>
        /// <returns>File info</returns>
        public FileExecutionResult Update(Model.File item)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    using (var rep = GetNewRepository(logSession))
                    {
                        logSession.Add($"Try to get file with id = '{item.FileUID}' from database...");
                        var dbFile = rep.Get<RoyaltyRepository.Models.File>(i => i.FileUID == item.FileUID).FirstOrDefault();
                        if (dbFile == null)
                            throw new Exception(Properties.Resources.SERVICES_FILE_FileNotFound);

                        if (item.Encoding != null)
                            dbFile.Encoding = item.Encoding;

                        if (!string.IsNullOrEmpty(item.FileName) && (string.Compare(dbFile.FileName, item.FileName, false) != 0))
                        {
                            dbFile.FileName = System.IO.Path.GetFileName(item.FileName);
                            logSession.Add($"Try to rename file in file storage...");
                            var fi = FileStorage.FileRename(item.FileUID, dbFile.FileName);
                            dbFile.OriginalFileName = fi.Name;
                        }

                        dbFile.MimeType = (string.IsNullOrEmpty(item.MimeType))
                            ? RoyaltyFileStorage.MimeTypes.GetMimeTypeFromFileName(dbFile.OriginalFileName)
                            : item.MimeType;

                        logSession.Add($"Try to update file in database...");
                        rep.SaveChanges();

                        return new FileExecutionResult(Mapper.Map<Model.File>(dbFile));
                    }
                }
                catch (Exception ex)
                {
                    ex.Data.Add(nameof(item), item);
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    return new FileExecutionResult(ex);
                }
        }

        #endregion
    }
}
