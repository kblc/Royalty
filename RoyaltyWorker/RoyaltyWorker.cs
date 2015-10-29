using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RoyaltyRepository;
using RoyaltyFileStorage;
using System.Threading;
using System.ComponentModel;
using System.Collections.Concurrent;
using Helpers;
using RoyaltyWorker.Model;
using Helpers.Linq;
using RoyaltyDataCalculator;
using RoyaltyWorker.Extensions;

namespace RoyaltyWorker
{
    /// <summary>
    /// Задача Worker'a - выдергивать элементы из очереди и обрабатывать их
    /// </summary>
    public class RoyaltyWorker : IDisposable
    {
        /// <summary>
        /// Royalty repository
        /// </summary>
        private readonly Func<Repository> getNewRepository = null;

        /// <summary>
        /// File storage
        /// </summary>
        private readonly IFileStorage fileStorage = null;

        private TimeSpan checkTimerInterval = new TimeSpan(0, 0, 10);
        private Timer checkQueueTimer = null;
        private Thread checkQueueThread = null;
        private object checkLockObject = new object();

        /// <summary>
        /// Is verbose log enabled
        /// </summary>
        public bool VerboseLog { get; set; } = false;

        /// <summary>
        /// Interval between calculations
        /// </summary>
        public TimeSpan CheckTimerInterval
        {
            get { return checkTimerInterval; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));

                if (checkTimerInterval == value)
                    return;

                checkQueueTimer?.Change(TimeSpan.FromTicks(0), checkTimerInterval);
                checkTimerInterval = value;
            }
        }

        /// <summary>
        /// Log file encoding
        /// </summary>
        public Encoding LogFileEncodig { get; set; } = Encoding.GetEncoding(Config.WorkerConfigSection.DefaultEncoding);

        /// <summary>
        /// Инициализация нового экземпляра
        /// </summary>
        /// <param name="getNewRepository">Функия для получения нового экземпляра репозитория</param>
        /// <param name="fileStorage">Хранилище файлов</param>
        public RoyaltyWorker(Func<Repository> getNewRepository, IFileStorage fileStorage)
        {
            if (getNewRepository == null)
                throw new ArgumentNullException(nameof(getNewRepository));
            if (fileStorage == null)
                throw new ArgumentNullException(nameof(fileStorage));

            this.getNewRepository = getNewRepository;
            this.fileStorage = fileStorage;

            Init(getNewRepository, fileStorage);
        }

        /// <summary>
        /// Отменить выполнение текущих действий. Может привести к ошибке
        /// </summary>
        public void CancelCurrent()
        {
            lock(checkLockObject)
            { 
                if (checkQueueThread != null && checkQueueThread.IsAlive)
                    checkQueueThread.Abort();
                checkQueueThread = null;
            }
        }

        /// <summary>
        /// Инициализация обработчика
        /// </summary>
        /// <param name="getNewRepository">Функия для получения нового экземпляра репозитория</param>
        /// <param name="fileStorage">Хранилище файлов</param>
        private void Init(Func<Repository> getNewRepository, IFileStorage fileStorage)
        {
            if (Config.Config.IsWorkerConfigured)
            {
                CheckTimerInterval = Config.Config.WorkerConfig.CheckTimerInterval;
                VerboseLog = Config.Config.WorkerConfig.VerboseLog;
                LogFileEncodig = Config.Config.WorkerConfig.LogFileEncoding;
            }

            checkQueueTimer = new Timer(CheckQueueTimerCallback, null, TimeSpan.FromTicks(0), CheckTimerInterval);
        }

        /// <summary>
        /// Событие по таймеру, означающее необходимость стартовать проверку
        /// </summary>
        /// <param name="state">Не используется</param>
        private void CheckQueueTimerCallback(object state)
        {
            lock(checkLockObject)
            {
                if (checkQueueThread != null && checkQueueThread.IsAlive)
                    return;
                checkQueueThread = new Thread(new ParameterizedThreadStart(ProcessRecordsThread));
                checkQueueThread.Start(new { GetNewRepository = this.getNewRepository, FileStorage = this.fileStorage, VerboseLog, LogFileEncodig });
            }
        }

        /// <summary>
        /// Обрабатываем очередь, пока в ней не кончатся элементы, затем выходим
        /// </summary>
        /// <param name="prm">Входные параметры</param>
        private void ProcessRecordsThread(object prm)
        {
            var getNewRep = ((dynamic)prm).GetNewRepository as Func<Repository>;
            var storage = ((dynamic)prm).FileStorage as IFileStorage;
            var verboseLog = (bool)((dynamic)prm).VerboseLog;
            var logFileEncoding = ((dynamic)prm).LogFileEncoding as Encoding;

            var progress = new Helpers.PercentageProgress();
            var processed = false;
            do
            {
                processed = false;
                using (var logSession = Helpers.Log.Session($"{GetType().Name}.{nameof(ProcessRecordsThread)}()", verboseLog, RaiseLogEvent))
                    try
                    {
                        using (var rep = getNewRep())
                        {
                            var queueItem = rep.ImportQueueRecordGet()
                                .Where(r => r.ProcessedDate == null)
                                .OrderBy(r => r.CreatedDate)
                                .FirstOrDefault();

                            if (queueItem != null)
                            {
                                processed = true;
                                logSession.Add($"Queue record found: {queueItem}. Try to process.");

                                //Get files with progress instances
                                var filesElement = queueItem.Files
                                    .Select(f => new { File = f, Progress = progress.GetChild() })
                                    .ToList();
                                var buildEvent = new Func<WorkerProcessElement>(() => new WorkerProcessElement(queueItem.ImportQueueRecordUID, progress.Value, filesElement.Select(i => new WorkerProcessFileProgress(i.File.ImportQueueRecordFileUID, i.Progress.Value))));

                                logSession.Add($"Queue record files count to process: {filesElement.Count}");

                                RaiseStart(buildEvent());
                                progress.Change += (s, e) => RaiseUpdate(buildEvent());
                                try
                                {
                                    try
                                    {
                                        //Process every file
                                        filesElement.ForEach(f => ProcessRecordFile(f.File, rep, storage, p => f.Progress.Value = p, (s) => logSession.Add(s), logFileEncoding));

                                        //Check for error
                                        if (queueItem.Files.Any(f => !string.IsNullOrWhiteSpace(f.Error)))
                                            queueItem.HasError = true;
                                    }
                                    catch(Exception ex)
                                    {
                                        ex.Data.Add(nameof(queueItem), queueItem);

                                        logSession.Enabled = true;
                                        logSession.Add(ex);
                                        queueItem.Error = ex.GetExceptionText();
                                        RaiseExceptionEvent(ex);
                                    }
                                    finally
                                    {
                                        queueItem.ProcessedDate = DateTime.UtcNow;
                                        rep.SaveChanges();
                                    }

                                }
                                finally { RaiseEnd(buildEvent()); }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        logSession.Enabled = true;
                        logSession.Add(ex);
                    }
            } while (processed);
        }

        /// <summary>
        /// Обрабатывает импортируемый файл
        /// </summary>
        /// <param name="queueFile">ImportQueueRecordFile instance<param>
        /// <param name="repository">Репозиторий</param>
        /// <param name="storage">Хранилище файлов</param>
        /// <param name="progressAction">Действие на изменение прогресса</param>
        /// <param name="logAction">Действие на событие логирования</param>
        private void ProcessRecordFile(RoyaltyRepository.Models.ImportQueueRecordFile queueFile, Repository repository, IFileStorage storage,
            Action<decimal> progressAction, Action<string> logAction, Encoding logFileEncoding)
        {
            if (queueFile == null)
                throw new ArgumentNullException(nameof(queueFile));
            if (repository == null)
                throw new ArgumentNullException(nameof(repository));
            if (storage == null)
                throw new ArgumentNullException(nameof(storage));

            var fileLog = new List<string>();
            logAction = new Action<string>((s) => 
            {
                fileLog.Add(s);
                logAction?.Invoke(s);
            });
            progressAction = progressAction ?? new Action<decimal>((i) => { });

            var progress = new Helpers.PercentageProgress();
            var pBegin = progress.GetChild(weight: 0.1m);
            var pAction = progress.GetChild(weight: 1.0m);
            var pEnd = progress.GetChild(weight: 0.1m);

            using (var logSession = Helpers.Log.Session($"{GetType().Name}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, (s) => s.ToList().ForEach(str => logAction(str))))
                try
                {
                    logSession.Add($"Change state to '{RoyaltyRepository.Models.ImportQueueRecordStateType.TryToProcess}' for '{queueFile}'");

                    queueFile.ImportQueueRecordState = repository.ImportQueueRecordStateGet(RoyaltyRepository.Models.ImportQueueRecordStateType.TryToProcess);
                    repository.SaveChanges();

                    pBegin.Value = 100;

                    using (var dc = new DataCalculator(queueFile.ImportQueueRecord.Account, repository))
                    {
                        var prg0 = pAction.GetChild(weight: 0.1m);
                        var prg1 = pAction.GetChild(weight: 0.2m);
                        var prg2 = pAction.GetChild(weight: 0.8m);
                        var prg3 = pAction.GetChild(weight: 0.5m);
                        var prg4 = pAction.GetChild(weight: 0.8m);

                        logSession.Add($"Read lines from import file '{queueFile.ImportFile}'");

                        var csvLines = repository.FileGetLines(storage, queueFile.ImportFile);
                        prg0.Value = 100;

                        logSession.Add($"Start load lines as CSV...");

                        var l = Helpers.CSV.CSVFile.Load(csvLines.AsEnumerable(),
                            verboseLogAction: (s) => logSession.Add(s),
                            tableValidator: dc.TableValidator,
                            rowFilter: dc.RowFilter);

                        logSession.Add($"Load lines as CSV done. Prepare table...");
                        dc.Prepare(l.Table, i => prg1.Value = i, s => logSession.Add(s));

                        logSession.Add($"Table prepared. Start preview data...");
                        var previewRes = dc.Preview(l.Table, i => prg2.Value = i, s => logSession.Add(s));

                        logSession.Add($"Preview done. Start import...");
                        var readyToExport = dc.Import(previewRes.Values, null, i => prg3.Value = i, s => logSession.Add(s)).ToList();
                        readyToExport.ForEach(adr => repository.ImportQueueRecordFileAccountDataRecordNew(queueFile, adr));

                        logSession.Add($"Try to save imported data...");
                        repository.SaveChanges();

                        if (queueFile.ForAnalize)
                        {
                            logSession.Add($"Data imported. Start analize and export...");
                            //TODO:
                        }
                        prg4.Value = 100;
                    }

                    logSession.Add($"Change state to '{RoyaltyRepository.Models.ImportQueueRecordStateType.Processed}' for '{queueFile}'");
                    queueFile.ImportQueueRecordState = repository.ImportQueueRecordStateGet(RoyaltyRepository.Models.ImportQueueRecordStateType.Processed);
                }
                catch (Exception ex)
                {
                    logSession.Add(ex);
                    logSession.Enabled = true;
                    logSession.Add($"Change state to '{RoyaltyRepository.Models.ImportQueueRecordStateType.Error}' for '{queueFile}'");
                    queueFile.ImportQueueRecordState = repository.ImportQueueRecordStateGet(RoyaltyRepository.Models.ImportQueueRecordStateType.Error);
                    queueFile.Error = ex.GetExceptionText();
                }
                finally
                {
                    queueFile.LogFile = repository.FilePut(storage, fileLog, logFileEncoding, "fileimport.log");
                    queueFile.Finished = DateTime.UtcNow;
                    repository.SaveChanges();
                    pEnd.Value = 100;

                    logSession.Add($"All actions done for '{queueFile}'");
                }
        }

        #region Work

        private void RaiseStart(WorkerProcessElement element)
        {
            WorkerStateChanged?.Invoke(this, new WorkerProcessElementEventArgs(element, WorkerProcessElementAction.Start));
        }
        private void RaiseUpdate(WorkerProcessElement element)
        {
            WorkerStateChanged?.Invoke(this, new WorkerProcessElementEventArgs(element, WorkerProcessElementAction.Update));
        }
        private void RaiseEnd(WorkerProcessElement element)
        {
            WorkerStateChanged?.Invoke(this, new WorkerProcessElementEventArgs(element, WorkerProcessElementAction.End));
        }

        #endregion
        #region Events

        public event EventHandler<string> Log;
        public event EventHandler<Exception> ExceptionLog;
        public event EventHandler<WorkerProcessElementEventArgs> WorkerStateChanged;

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
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    checkQueueTimer?.Dispose();
                    lock(checkLockObject)
                    { 
                        if (checkQueueThread != null && checkQueueThread.IsAlive)
                            checkQueueThread.Abort();
                        checkQueueThread = null;
                    }
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
