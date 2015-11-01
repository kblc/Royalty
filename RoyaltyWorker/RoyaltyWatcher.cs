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
using RoyaltyWorker.Base;
using System.Configuration;

namespace RoyaltyWorker
{
    public class WatcherThreadParameters
    {
        public readonly Func<Repository> GetNewRepository = null;
        public readonly IFileStorage Storage = null;
        public readonly bool VerboseLog = false;
        public readonly bool ExceptionIfNoOneFileInQueue = false;
        public readonly bool SuspendTimerCheck = false;
        public readonly Account AccountForCheck = null;
        public readonly Action<IEnumerable<string>> Log;
        public readonly Action<Exception> ExceptionLog;
        public readonly Action<ImportQueueRecord> OnQueueRecordAdded;
        public readonly TimeSpan TimerInterval;

        public WatcherThreadParameters(Func<Repository> getNewRepository, IFileStorage storage, bool verboseLog, bool exceptionIfNoOneFileInQueue, bool suspendTimerCheck, 
            Account accountForCheck, Action<IEnumerable<string>> log, Action<Exception> exceptionLog, Action<ImportQueueRecord> onQueueRecordAdded, TimeSpan timerInterval)
        {
            GetNewRepository = getNewRepository;
            Storage = storage;
            VerboseLog = verboseLog;
            ExceptionIfNoOneFileInQueue = exceptionIfNoOneFileInQueue;
            SuspendTimerCheck = suspendTimerCheck;
            AccountForCheck = accountForCheck;
            Log = log;
            ExceptionLog = exceptionLog;
            OnQueueRecordAdded = onQueueRecordAdded;
            TimerInterval = timerInterval;
        }
    }

    public class RoyaltyWatcher : RoyaltyBase<WatcherThreadParameters, Account>
    {
        /// <summary>
        /// Throws exception when no one file in queue
        /// </summary>
        public bool ExceptionIfNoOneFileInQueue { get; set; } = Config.WatcherConfigSection.DefaultExceptionIfNoOneFileInQueue;

        #region Constructor

        /// <summary>
        /// Create new watcher instance
        /// </summary>
        /// <param name="repository">Royalty repository</param>
        /// <param name="storage">File storage</param>
        public RoyaltyWatcher(Func<Repository> getNewRepository, IFileStorage storage)
            : this(getNewRepository, storage, false) { }

        /// <summary>
        /// Create new watcher instance
        /// </summary>
        /// <param name="repository">Royalty repository</param>
        /// <param name="storage">File storage</param>
        /// <param name="createStarted">Create started</param>
        public RoyaltyWatcher(Func<Repository> getNewRepository, IFileStorage storage, bool createStarted)
            : base(getNewRepository, storage, createStarted, TimeSpan.FromSeconds(30)) { }

        #endregion

        /// <summary>
        /// Main thread proc
        /// </summary>
        /// <param name="prm">Parameters for thread</param>
        private static void ProcessThreadProc(WatcherThreadParameters prm)
        {
            using (var logSession = Helpers.Log.Session($"{nameof(RoyaltyWatcher)}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", prm.VerboseLog, prm.Log))
                try
                {
                    var currentTime = DateTime.UtcNow;
                    var utcTs = new TimeSpan(currentTime.Hour, currentTime.Minute, currentTime.Second);

                    logSession.Add($"Ckeck started at '{currentTime}'");

                    using (var rep = prm.GetNewRepository())
                    {
                        List<Account> accountsToProcess = null;

                        if (prm.SuspendTimerCheck)
                        {
                            if (prm.AccountForCheck == null)
                                accountsToProcess = rep.AccountGet().ToList(); 
                            else
                                accountsToProcess = new Account[] { rep.AccountGet(prm.AccountForCheck.AccountUID) }.ToList();
                        } else
                        { 
#pragma warning disable 618
                            accountsToProcess = rep
                                .AccountSettingsSheduleTimeGet()
                                .Where(i => (long)utcTs.TotalMilliseconds - i.TimeTicks < (long)prm.TimerInterval.TotalMilliseconds)
                                .Join(rep.AccountGet(), st => st.AccountUID, a => a.AccountUID, (st, a) => a)
                                .Distinct()
                                .ToList();
                        }
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
                                    ProcessAccount(queueRecord, rep, prm.Storage, prm.ExceptionIfNoOneFileInQueue, logSession);
                                    prm.OnQueueRecordAdded(queueRecord);
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
                    prm.ExceptionLog(ex);
                }
        }

        /// <summary>
        /// Добавление в очередь файлов по событиям аккаунтов
        /// </summary>
        /// <param name="importQueue">Запись в очереди</param>
        /// <param name="repository">Репозиторий</param>
        /// <param name="upperLogSession">Лог сессия верхнего уровня</param>
        private static void ProcessAccount(ImportQueueRecord importQueue, Repository repository, IFileStorage storage, bool exceptionIfNoOneFileInQueue, Helpers.Log.SessionInfo upperLogSession)
        {
            using(var logSession = Helpers.Log.Session($"{nameof(RoyaltyWatcher)}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", upperLogSession.Enabled, s => s.ToList().ForEach(str => upperLogSession.Add(str))))
                try
                {
                    if (importQueue == null)
                        throw new ArgumentNullException(nameof(importQueue));
                    if (repository == null)
                        throw new ArgumentNullException(nameof(repository));

                    importQueue.Account.Settings.ImportDirectories.ToList()
                        .ForEach(f => ProcessAccount(importQueue, repository, storage, f, logSession));

                    if (importQueue.FileInfoes.Count == 0 && exceptionIfNoOneFileInQueue)
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
        private static void ProcessAccount(ImportQueueRecord importQueue, Repository rep, IFileStorage storage, AccountSettingsImportDirectory importDirectory, Helpers.Log.SessionInfo upperLogSession)
        {
            using (var logSession = Helpers.Log.Session($"{nameof(RoyaltyWatcher)}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", upperLogSession.Enabled, s => s.ToList().ForEach(str => upperLogSession.Add(str))))
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
        private void RaiseLogEvent(string logText)
        {
            Log?.Invoke(this, logText);
        }
        private void RaiseLogEvent(IEnumerable<string> logTexts)
        {
            logTexts?.ToList().ForEach(s => RaiseLogEvent(s));
        }
        private void RaiseExceptionLogEvent(Exception ex)
        {
            ExceptionLog?.Invoke(this, ex);
        }

        #endregion
        #region Abstract implementation

        protected override ConfigurationSection GetConfigSection() => Config.Config.WatcherConfig;

        protected override WatcherThreadParameters GetStartParameters() => new WatcherThreadParameters(
            getNewRepository, storage, VerboseLog, ExceptionIfNoOneFileInQueue, false, null, 
            RaiseLogEvent, RaiseExceptionLogEvent, RaiseOnQueueRecordAdded, TimerInterval);

        protected override WatcherThreadParameters GetRunOnceParameters(Account acc) => new WatcherThreadParameters(
            getNewRepository, storage, VerboseLog, ExceptionIfNoOneFileInQueue, true, acc,
            RaiseLogEvent, RaiseExceptionLogEvent, RaiseOnQueueRecordAdded, TimerInterval);

        protected override void ThreadProc(WatcherThreadParameters prm) => ProcessThreadProc(prm);

        #endregion
    }
}
