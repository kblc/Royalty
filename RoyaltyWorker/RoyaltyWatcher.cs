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
using System.Threading;
using RoyaltyWorker.Extensions;

namespace RoyaltyWorker
{
    public class RoyaltyWatcher : IDisposable 
    {
        /// <summary>
        /// Royalty repository
        /// </summary>
        private readonly Func<Repository> getNewRepository = null;

        /// <summary>
        /// File storage
        /// </summary>
        private readonly IFileStorage fileStorage = null;

        private Thread checkQueueThread = null;
        private object checkLockObject = new object();

        /// <summary>
        /// Is verbose log enabled
        /// </summary>
        public bool VerboseLog { get; set; } = false;

        /// <summary>
        /// Throws exception when no one file in queue
        /// </summary>
        public bool ExceptionIfNoOneFileInQueue { get; set; } = true;

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
        /// Is watcher busy
        /// </summary>
        public bool IsBusy { get { return (checkQueueThread != null && checkQueueThread.IsAlive); } }

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
            if (Config.Config.IsWatcherConfigured)
            {
                this.VerboseLog = Config.Config.WatcherConfig.VerboseLog;
                this.CheckTimerInterval = Config.Config.WatcherConfig.CheckTimerInterval;
                this.ExceptionIfNoOneFileInQueue = Config.Config.WatcherConfig.ExceptionIfNoOneFileInQueue;
            }
            checkTimer = new System.Threading.Timer(CheckTimerCallback, null, new TimeSpan(), checkTimerInterval);
        }
          
        /// <summary>
        /// Check timer callback
        /// </summary>
        /// <param name="state"></param>
        private void CheckTimerCallback(object state)
        {
            lock (checkLockObject)
            {
                if (checkQueueThread != null && checkQueueThread.IsAlive)
                    return;
                checkQueueThread = new Thread(new ParameterizedThreadStart(ProcessAccountsThread));
                checkQueueThread.Start(new { GetNewRepository = this.getNewRepository, FileStorage = this.fileStorage, VerboseLog, ExceptionIfNoOneFileInQueue });
            }
        }

        private void ProcessAccountsThread(object prm)
        {
            var getNewRep = ((dynamic)prm).GetNewRepository as Func<Repository>;
            var storage = ((dynamic)prm).FileStorage as IFileStorage;
            var verboseLog = (bool)((dynamic)prm).VerboseLog;
            var exceptionIfNoOneFileInQueue = (bool)((dynamic)prm).ExceptionIfNoOneFileInQueue;

            using (var logSession = Helpers.Log.Session($"{GetType().Name}.{nameof(ProcessAccountsThread)}()", verboseLog, RaiseLogEvent))
                try
                {
                    var currentTime = DateTime.UtcNow;
                    var utcTs = new TimeSpan(currentTime.Hour, currentTime.Minute, currentTime.Second);

                    logSession.Add($"Ckeck started at '{currentTime}'");

                    using (var rep = getNewRep())
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
                                    ProcessAccount(queueRecord, rep, storage, logSession);
                                    RaiseOnQueueRecordAdded(queueRecord);
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
                    //throw ex;
                }
        }

        /// <summary>
        /// Добавление в очередь файлов по событиям аккаунтов
        /// </summary>
        /// <param name="importQueue">Запись в очереди</param>
        /// <param name="repository">Репозиторий</param>
        /// <param name="upperLogSession">Лог сессия верхнего уровня</param>
        private void ProcessAccount(ImportQueueRecord importQueue, Repository repository, IFileStorage storage, Helpers.Log.SessionInfo upperLogSession)
        {
            using(var logSession = Helpers.Log.Session($"{GetType().Name}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", upperLogSession.Enabled, s => s.ToList().ForEach(str => upperLogSession.Add(str))))
                try
                {
                    if (importQueue == null)
                        throw new ArgumentNullException(nameof(importQueue));
                    if (repository == null)
                        throw new ArgumentNullException(nameof(repository));

                    importQueue.Account.Settings.ImportDirectories.ToList()
                        .ForEach(f => ProcessAccount(importQueue, repository, storage, f, logSession));

                    if (importQueue.FileInfoes.Count == 0 && ExceptionIfNoOneFileInQueue)
                        throw new System.Exception(Properties.Resources.ROYALTYWATCHER_NoOneFilesInQueue);

                    if (importQueue.FileInfoes.All(f => f.Finished != null))
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
        private void ProcessAccount(ImportQueueRecord importQueue, Repository rep, IFileStorage storage, AccountSettingsImportDirectory importDirectory, Helpers.Log.SessionInfo upperLogSession)
        {
            using (var logSession = Helpers.Log.Session($"{GetType().Name}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, s => s.ToList().ForEach(str => upperLogSession.Add(str))))
                try
                {
                    if (!System.IO.Directory.Exists(importDirectory.Path))
                        throw new Exception($"Folder '{importDirectory.Path}' not found");

                    var filesInFolder = System.IO.Directory.GetFiles(importDirectory.Path, importDirectory.Filter, importDirectory.RecursiveFolderSearch ? System.IO.SearchOption.AllDirectories : System.IO.SearchOption.TopDirectoryOnly);
                    logSession.Add($"Founded files count: {filesInFolder.Length} for analize: {importDirectory.ForAnalize} in folder '{importDirectory.Path}'");
                    foreach (var filePath in filesInFolder)
                        try
                        {
                            logSession.Add($"File to process: {filePath}");
                            var importFileRecord = rep.ImportQueueRecordFileInfoNew(importQueue, new { ForAnalize = importDirectory.ForAnalize, SourceFilePath = filePath });
                            try
                            {
                                logSession.Add($"Try put file '{filePath}' into storage");
                                var file = rep.FilePut(storage, filePath, importDirectory.Encoding);
                                rep.ImportQueueRecordFileInfoFileNew(ImportQueueRecordFileInfoFileType.Import, importFileRecord, file);
                                logSession.Add($"File '{file.FileName}' (size:{file.FileSize}) stored into storage with path: '{file.FilePath}'");
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
                                if (importDirectory.DeleteFileAfterImport)
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
        public event EventHandler<Exception> ExceptionLog;
        public event EventHandler<ImportQueueRecord> OnQueueRecordAdded;

        private void RaiseOnQueueRecordAdded(ImportQueueRecord record)
        {
            OnQueueRecordAdded?.Invoke(this, record);
        }
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
            ExceptionLog?.Invoke(this, ex);
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

                    if (checkQueueThread != null && checkQueueThread.IsAlive)
                        checkQueueThread.Abort();
                    checkQueueThread = null;
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
