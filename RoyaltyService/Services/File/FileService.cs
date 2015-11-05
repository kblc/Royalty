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
    public class FileService : IFileService
    {
        static FileService()
        {
            Model.FileInfo.InitializeMap();
            //if (Config.Config.IsServicesConfigured && !string.IsNullOrWhiteSpace(Config.Config.ServicesConfig.FileServiceLogFileName))
            //{

            //}
        }

        private static Dictionary<InstanceContext, CultureInfo> langDictionary = new Dictionary<InstanceContext, CultureInfo>();

        public FileService()
        {
            var removeAction = new Action<InstanceContext>((s) =>
            {
                lock (langDictionary)
                    if (langDictionary.ContainsKey(s))
                        langDictionary.Remove(s);
            });

            OperationContext.Current.InstanceContext.Faulted += (s, e) => removeAction((InstanceContext)s);
            OperationContext.Current.InstanceContext.Closed += (s, e) => removeAction((InstanceContext)s);
        }

        private bool verboseLog = Config.Config.ServicesConfig?.VerboseLog ?? false;
        public bool VerboseLog
        {
            get { return verboseLog; }
            set { verboseLog = value; FileStorage.VerboseLog = verboseLog; }
        }

        public CultureInfo CurrentCulture
        {
            get
            {
                lock(langDictionary)
                    return (langDictionary.ContainsKey(OperationContext.Current.InstanceContext))
                            ? langDictionary[OperationContext.Current.InstanceContext] ?? Thread.CurrentThread.CurrentCulture
                            : Thread.CurrentThread.CurrentCulture;
            }
            set
            {
                lock (langDictionary)
                    if (langDictionary.ContainsKey(OperationContext.Current.InstanceContext))
                        langDictionary[OperationContext.Current.InstanceContext] = value;
                    else
                        langDictionary.Add(OperationContext.Current.InstanceContext, value);
            }
        }

        private void UpdateSessionCulture()
        {
            var cultureForSession = CurrentCulture;
            Thread.CurrentThread.CurrentCulture = cultureForSession;
            Thread.CurrentThread.CurrentUICulture = cultureForSession;
        }

        private RoyaltyFileStorage.IFileStorage fileStorage = null;
        protected RoyaltyFileStorage.IFileStorage FileStorage
        {
            get
            {
                if (fileStorage == null)
                {
                    fileStorage = new RoyaltyFileStorage.FileStorage();
                    fileStorage.Log += (s, e) => RaiseLog($"[FILESTORAGE] {e}");
                    fileStorage.VerboseLog = verboseLog;
                }
                return fileStorage;
            }
        }

        protected RoyaltyRepository.Repository GetNewRepository(Helpers.Log.SessionInfo logSession)
        {
            var rep = new RoyaltyRepository.Repository();
            rep.SqlLog += (s, e) => RaiseSqlLog(e);
            rep.Log += (s, e) => logSession.Add(e, "[REPOSITORY]");
            return rep;
        }

        private Guid GetGuidByString(string guidText)
        {
            var res = TryGetGuidByString(guidText);
            if (res == null)
            {
                var ex = new Exception(Properties.Resources.SERVICES_FILE_BadIdentifierFormat);
                ex.Data.Add(nameof(guidText), guidText);
                throw ex;
            }
            return res.Value;
        }
        private Guid? TryGetGuidByString(string guidText)
        {
            Guid res;
            if (!Guid.TryParse(guidText, out res))
                if (!Guid.TryParseExact(guidText, "N", out res))
                    return null;
            return res;
        }

        public IEnumerable<Model.FileInfo> GetRangeFromFileIdentifiers(IEnumerable<string> fileIds)
        {
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}(fileIds={fileIds.Concat(i => i.ToString(), ",")})", VerboseLog, RaiseLog))
                try
                {
                    using (var rep = GetNewRepository(logSession))
                    {
                        var res = new List<Model.FileInfo>();
                        foreach (var fileId in fileIds.Select(fi => GetGuidByString(fi)).ToArray())
                        {
                            logSession.Add($"Try to get file with id = '{fileId}' from database...");
                            var file = rep.GetFile(fileId);
                            if (file != null)
                            {
                                logSession.Add($"File found: '{file}'.");
                                var fileInfo = Mapper.Map<Model.FileInfo>(file);
                                res.Add(fileInfo);
                            }
                            else
                                throw new Exception(Properties.Resources.SERVICES_FILE_FileNotFound);
                        }
                        return res;
                    }
                }
                catch (Exception ex)
                {
                    ex.Data.Add(nameof(fileIds), fileIds.Concat(i => i.ToString(), ","));
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    throw;
                }
        }

        #region Service contract implementation

        public void Delete(string fileId)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}(fileId={fileId})", VerboseLog, RaiseLog))
                try
                {
                    using (var rep = new RoyaltyRepository.Repository())
                    {
                        rep.SqlLog += (s, e) => RaiseSqlLog(e);
                        rep.Log += (s, e) => logSession.Add(e, "[REPOSITORY]");

                        logSession.Add($"Try to get file with id = '{fileId}' from database...");

                        var file = rep.GetFile(GetGuidByString(fileId));
                        if (file != null)
                        {
                            logSession.Add($"File found: '{file}'.");
                            rep.Remove(file);
                            logSession.Add($"File with id = '{fileId}' deleted from database.");
                            FileStorage.FileDelete(file.FileID);
                            logSession.Add($"File with id = '{fileId}' deleted from file storage.");
                        }
                        else
                            throw new Exception(Properties.Resources.SERVICES_FILE_FileNotFound);
                    }
                }
                catch(Exception ex)
                {
                    ex.Data.Add(nameof(fileId), fileId);
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    throw;
                }
        }

        public FileInfoExecutionResult Get(string fileId)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    var item = GetRangeFromFileIdentifiers(new string[] { fileId }).SingleOrDefault();
                    return new FileInfoExecutionResult(item);
                }
                catch (Exception ex)
                {
                    ex.Data.Add(nameof(fileId), fileId);
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    return new FileInfoExecutionResult(ex);
                }
        }

        public FileInfoExecutionResults GetRange(IEnumerable<string> fileIds)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    var item = GetRangeFromFileIdentifiers(fileIds);
                    return new FileInfoExecutionResults(item);
                }
                catch (Exception ex)
                {
                    ex.Data.Add(nameof(fileIds), fileIds.Concat(i => i?.ToString() ?? "NULL", ","));
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    return new FileInfoExecutionResults(ex);
                }
        }

        public Stream GetSource(string fileIdOrName)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    //try
                    //{ 
                    //    System.ServiceModel.Web.WebOperationContext.Current.OutgoingResponse.Headers.Add(System.Net.HttpRequestHeader.ContentEncoding, );
                    //}
                    //catch { }
                    logSession.Add($"Try to get file with id or name = '{fileIdOrName}' from database...");
                    var fileId = TryGetGuidByString(fileIdOrName);
                    return fileId.HasValue
                        ? FileStorage.FileGet(fileId.Value)
                        : FileStorage.FileGet(fileIdOrName);
                }
                catch (Exception ex)
                {
                    ex.Data.Add(nameof(fileIdOrName), fileIdOrName);
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    throw;
                }
        }

        public FileInfoExecutionResult Put(System.IO.Stream content)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    using (var rep = GetNewRepository(logSession))
                    {
                        var dbFile = rep.New<RoyaltyRepository.Models.File>((f) =>
                        {
                            f.FileName = "unknown";
                            f.MimeType = RoyaltyFileStorage.MimeTypes.GetMimeTypeFromFileName(f.FileName);
                        });

                        logSession.Add($"Try to save file to file storage...");
                        var fi = FileStorage.FilePut(dbFile.FileID, content, dbFile.FileName);

                        dbFile.FileSize = fi.Length;
                        dbFile.FilePath = fi.FullName;

                        logSession.Add($"Try to save file to database...");
                        rep.Add(dbFile);
                        rep.SaveChanges();

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

        public FileInfoExecutionResult Update(string fileIdstring, string fileName, string encoding, string mime)
        {
            UpdateSessionCulture();
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    using (var rep = GetNewRepository(logSession))
                    {
                        logSession.Add($"Try to get file with id = '{fileIdstring}' from database...");

                        var id = GetGuidByString(fileIdstring);

                        var dbFile = rep.GetFile(id);
                        if (dbFile == null)
                            throw new Exception(Properties.Resources.SERVICES_FILE_FileNotFound);

                        if (!string.IsNullOrEmpty(fileName))
                        { 
                            dbFile.FileName = System.IO.Path.GetFileName(fileName);
                            var fi = FileStorage.FileRename(id, fileName);
                            dbFile.FilePath = fi.FullName;
                        }

                        if (!string.IsNullOrEmpty(encoding))
                            dbFile.Encoding = Encoding.GetEncoding(encoding);

                        logSession.Add($"Try to rename file in file storage...");

                        if (!string.IsNullOrEmpty(mime))
                            dbFile.MimeType = mime;
                        else
                            dbFile.MimeType = RoyaltyFileStorage.MimeTypes.GetMimeTypeFromFileName(dbFile.FilePath);

                        logSession.Add($"Try to update file in database...");
                        rep.SaveChanges();

                        return new FileInfoExecutionResult(Mapper.Map<Model.FileInfo>(dbFile));
                    }
                }
                catch (Exception ex)
                {
                    ex.Data.Add(nameof(fileIdstring), fileIdstring);
                    ex.Data.Add(nameof(fileName), fileName);
                    ex.Data.Add(nameof(encoding), encoding);
                    ex.Data.Add(nameof(mime), mime);
                    logSession.Enabled = true;
                    logSession.Add(ex);
                    return new FileInfoExecutionResult(ex);
                }
        }

        public void ChangeLanguage(string codename)
        {
            using (var logSession = Helpers.Log.Session($"{GetType()}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, RaiseLog))
                try
                {
                    CurrentCulture = new CultureInfo(codename);
                    UpdateSessionCulture();
                }
                catch (Exception ex)
                {
                    ex.Data.Add(nameof(codename), codename);
                    logSession.Enabled = true;
                    logSession.Add(ex);
                }
        }

        #endregion
        #region Log events

        public event EventHandler<string> SqlLog;
        public event EventHandler<string> Log;

        private void RaiseSqlLog(string logMessage)
        {
            SqlLog?.Invoke(this, logMessage);
            StaticSqlLog?.Invoke(this, logMessage);
        }
        private void RaiseLog(string logMessage)
        {
            Log?.Invoke(this, logMessage);
            StaticLog?.Invoke(this, logMessage);
        }
        private void RaiseLog(IEnumerable<string> logMessages)
        {
            foreach (var s in logMessages)
                RaiseLog(s);
        }

        public static event EventHandler<string> StaticSqlLog;
        public event EventHandler<string> StaticLog;

        #endregion
    }
}
