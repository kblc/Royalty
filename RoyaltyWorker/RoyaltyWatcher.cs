using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RoyaltyRepository;
using RoyaltyFileStorage;
using Helpers.Linq;
using RoyaltyRepository.Models;
using Helpers;

namespace RoyaltyWorker
{
    public class RoyaltyWatcher : IDisposable //IRoyaltyWorker, 
    {
        /// <summary>
        /// Royalty repository
        /// </summary>
        private readonly Func<Repository> getNewRepository = null;

        /// <summary>
        /// File storage
        /// </summary>
        private readonly IFileStorage fileStorage = null;

        /// <summary>
        /// Is verbose log enabled
        /// </summary>
        public bool VerboseLog { get; set; } = false;

        /// <summary>
        /// File mask for add in queue
        /// </summary>
        public string FileMaskForAddFileInQeueue { get; set; } = "*.csv";

        /// <summary>
        /// Watch only top directory
        /// </summary>
        public bool WatchOnlyTopDirectory { get; set; } = true;

        /// <summary>
        /// Throws exception when no one file in queue
        /// </summary>
        public bool ExceptionOnNoOneFileInQueue { get; set; } = true;

        private TimeSpan checkTimerInterval = new TimeSpan(0, 0, 30);

        /// <summary>
        /// Interval for check events
        /// </summary>
        public TimeSpan CheckTimerInterval
        {
            get { return checkTimerInterval; }
            set {
                if (checkTimerInterval == value)
                    return;
                checkTimer?.Change(new TimeSpan(), value);
                checkTimerInterval = value;
            }
        }

        /// <summary>
        /// Check timer
        /// </summary>
        private System.Threading.Timer checkTimer = null;

        /// <summary>
        /// Create new worker instance
        /// </summary>
        /// <param name="repository">Royalty repository</param>
        /// <param name="fileStorage">File storage</param>
        public RoyaltyWatcher(Func<Repository> getNewRepository, IFileStorage fileStorage)
        {
            if (getNewRepository == null)
                throw new ArgumentNullException(nameof(getNewRepository));
            if (fileStorage == null)
                throw new ArgumentNullException(nameof(fileStorage));

            this.getNewRepository = getNewRepository;
            this.fileStorage = fileStorage;

            Init();
        }

        /// <summary>
        /// Initialization
        /// </summary>
        private void Init()
        {
            checkTimer = new System.Threading.Timer(CheckTimerCallback, null, new TimeSpan(), checkTimerInterval);
            if (Config.Config.IsWatcherConfigured)
            {
                this.VerboseLog = Config.Config.WatcherConfig.VerboseLog;
                this.WatchOnlyTopDirectory = Config.Config.WatcherConfig.WatchOnlyTopDirectory;
                this.CheckTimerInterval = Config.Config.WatcherConfig.CheckTimerInterval;
                this.ExceptionOnNoOneFileInQueue = Config.Config.WatcherConfig.ExceptionOnNoOneFileInQueue;
                this.FileMaskForAddFileInQeueue = Config.Config.WatcherConfig.FileMaskForAddFileInQeueue ?? FileMaskForAddFileInQeueue;
            }
        }

        private bool callBackCalled = false;
        private object callBackCalledObject = new object();

        /// <summary>
        /// Check timer callback
        /// </summary>
        /// <param name="state"></param>
        private void CheckTimerCallback(object state)
        {
            lock(callBackCalledObject)
            {
                if (callBackCalled)
                    return;
                callBackCalled = true;
            }

            try
            {
                using (var logSession = Helpers.Log.Session($"{GetType().Name}.{nameof(CheckTimerCallback)}()", VerboseLog, RaiseLogEvent))
                    try
                    {
                        var currentTime = DateTime.UtcNow;
                        var utcTs = new TimeSpan(currentTime.Hour, currentTime.Minute, currentTime.Second);

                        logSession.Add($"Ckeck started at '{currentTime}'");

                        using (var rep = getNewRepository())
                        {
#pragma warning disable 618
                            var accountsToProcess = rep
                                .AccountSettingsSheduleTimeGet()
                                .Where(i => (long)utcTs.TotalMilliseconds - i.TimeTicks < (long)checkTimerInterval.TotalMilliseconds)
                                .Join(rep.AccountGet(), st => st.AccountUID, a => a.AccountUID, (st, a) => a)
                                .Distinct()
                                .ToList();
#pragma warning restore 618
                            if (accountsToProcess.Count > 0)
                            {
                                logSession.Add($"Accounts to process: {accountsToProcess.Concat(a => $"'{a.Name}'", ",")}");
                                accountsToProcess.ForEach(account =>
                                {
                                    try
                                    {
                                        logSession.Add($"Proceed account '{account.Name}'");
                                        var queueRecord = rep.ImportQueueRecordNew(account);
                                        ProcessAccount(queueRecord, rep, logSession);
                                    }
                                    catch (Exception ex)
                                    {
                                        ex.Data.Add(nameof(account), account?.Name ?? "NULL");
                                        throw ex;
                                    }
                                });
                                rep.SaveChanges();
                            }
                            logSession.Add($"All account processed");
                        }
                    }
                    catch (Exception ex)
                    {
                        logSession.Enabled = true;
                        logSession.Add(ex);
                        RaiseExceptionEvent(ex);
                        throw ex;
                    }
            }
            finally
            {
                lock(callBackCalledObject)
                    callBackCalled = false;
            }
        }

        /// <summary>
        /// Добавление в очередь файлов по событиям аккаунтов
        /// </summary>
        /// <param name="importQueue">Запись в очереди</param>
        /// <param name="repository">Репозиторий</param>
        /// <param name="upperLogSession">Лог сессия верхнего уровня</param>
        private void ProcessAccount(ImportQueueRecord importQueue, Repository repository, Helpers.Log.SessionInfo upperLogSession)
        {
            using(var logSession = Helpers.Log.Session($"{GetType().Name}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, s => s.ToList().ForEach(str => upperLogSession.Add(str))))
                try
                {
                    if (importQueue == null)
                        throw new ArgumentNullException(nameof(importQueue));
                    if (repository == null)
                        throw new ArgumentNullException(nameof(repository));

                    if (System.IO.Directory.Exists(importQueue.Account.Settings.FolderImportMain))
                    ProcessAccount(importQueue, repository, importQueue.Account.Settings.FolderImportMain, false, logSession);

                    if (System.IO.Directory.Exists(importQueue.Account.Settings.FolderImportAnalize))
                        ProcessAccount(importQueue, repository, importQueue.Account.Settings.FolderImportAnalize, true, logSession);

                    if (importQueue.Files.Count == 0 && ExceptionOnNoOneFileInQueue)
                        throw new System.Exception(Properties.Resources.ROYALTYWATCHER_NoOneFilesInQueue);

                    if (importQueue.Files.All(f => f.Finished != null))
                        importQueue.ProcessedDate = DateTime.UtcNow;
                } 
                catch(Exception ex)
                {
                    logSession.Add(ex);
                    logSession.Enabled = true;
                    importQueue.Error = ex.GetExceptionText();
                    importQueue.ProcessedDate = DateTime.UtcNow;
                    importQueue.HasError = true;
                }
                finally
                {
                    upperLogSession.Enabled = logSession.Enabled;
                }
        }

        /// <summary>
        ///  Добавление в очередь файлов из директорий
        /// </summary>
        /// <param name="importQueue">Запись в очереди</param>
        /// <param name="rep">Репозиторий</param>
        /// <param name="folderPath">Путь до диреткории с файлами</param>
        /// <param name="forAnalize">Данные используются для анализа</param>
        /// <param name="upperLogSession">Лог сессия верхнего уровня</param>
        private void ProcessAccount(ImportQueueRecord importQueue, Repository rep, string folderPath, bool forAnalize, Helpers.Log.SessionInfo upperLogSession)
        {
            using (var logSession = Helpers.Log.Session($"{GetType().Name}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, s => s.ToList().ForEach(str => upperLogSession.Add(str))))
                try
                {
                    var filesInFolder = System.IO.Directory.GetFiles(folderPath, FileMaskForAddFileInQeueue, WatchOnlyTopDirectory ? System.IO.SearchOption.TopDirectoryOnly : System.IO.SearchOption.AllDirectories);
                    logSession.Add($"Founded files count: {filesInFolder.Length} for analize: {forAnalize} in folder '{folderPath}'");
                    foreach (var filePath in filesInFolder)
                        try
                        {
                            logSession.Add($"File to process: {filePath}");
                            var importFileRecord = rep.ImportQueueRecordFileNew(importQueue, new { ForAnalize = forAnalize, SourceFilePath = filePath });
                            try
                            {
                                var file = rep.FileNew(new { FileName = System.IO.Path.GetFileName(filePath) });
                                logSession.Add($"Try put file '{file.FileName}' into storage");
                                using (var fileStream = System.IO.File.OpenRead(filePath))
                                {
                                    var fileInfo = fileStorage.FilePut(file.FileID, fileStream, file.FileName);
                                    file.FilePath = fileInfo.FullName;
                                    file.FileSize = fileInfo.Length;
                                    file.MimeType = MimeTypes.GetMimeTypeFromFileName(file.FileName);
                                    logSession.Add($"File '{file.FileName}' (size:{file.FileSize}) stored into storage with path: '{file.FilePath}'");
                                }
                                importFileRecord.ImportFile = file;
                            }
                            catch (Exception ex)
                            {
                                importFileRecord.ImportQueueRecordState = rep.ImportQueueRecordStateGet(ImportQueueRecordStateType.Error);
                                importFileRecord.Error = ex.GetExceptionText();
                                importFileRecord.Finished = DateTime.UtcNow;
                                importQueue.HasError = true;
                                logSession.Add(ex);
                                logSession.Enabled = true;
                                RaiseExceptionEvent(ex);
                            }
                            finally
                            {
                                importQueue.Files.Add(importFileRecord);
                                try
                                {
                                    System.IO.File.Delete(filePath);
                                }
                                catch(Exception ex)
                                {
                                    logSession.Add(ex);
                                    logSession.Enabled = true;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            ex.Data.Add(nameof(filePath), filePath);
                            logSession.Add(ex);
                            logSession.Enabled = true;
                            RaiseExceptionEvent(ex);
                        }
                }
                catch(Exception ex)
                {
                    logSession.Add(ex);
                    logSession.Enabled = true;
                    throw ex;
                }
                finally
                {
                    upperLogSession.Enabled = logSession.Enabled;
                }
        }

        #region Events

        public event EventHandler<string> Log;
        public event EventHandler<Exception> Exception;

        private void RaiseLogEvent(IEnumerable<string> logTexts)
        {
            logTexts?.ToList().ForEach(s => RaiseLogEvent(s));
        }
        private void RaiseLogEvent(string logText)
        {
            Log?.Invoke(this, logText);
        }
        private void RaiseExceptionEvent(Exception ex)
        {
            Exception?.Invoke(this, ex);
        }

        #endregion
        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (checkTimer != null)
                        checkTimer.Dispose();
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}
