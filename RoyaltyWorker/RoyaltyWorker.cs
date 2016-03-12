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
using System.Data;
using System.Globalization;
using RoyaltyRepository.Models;
using RoyaltyWorker.Base;
using System.Configuration;
using System.Diagnostics;

namespace RoyaltyWorker
{
    public class WorkerThreadParameters
    {
        public readonly Func<Repository> GetNewRepository = null;
        public readonly IFileStorage Storage = null;
        public readonly bool VerboseLog = false;
        public readonly bool SuspendTimerCheck = false;
        public readonly Account AccountForCheck = null;
        public readonly Action<IEnumerable<string>> Log;
        public readonly Action<Exception> ExceptionLog;
        public readonly Action<WorkerProcessElementEventArgs> WorkerStateChanged;
        public readonly TimeSpan TimerInterval;
        public readonly Encoding LogFileEncodig;
        public readonly Encoding ExportFileEncodig;
        public readonly CultureInfo DefaultCultureForExportThread;
        public readonly string DefaultImportLogFileName;
        public readonly string DefaultExportFileName;

        public WorkerThreadParameters(Func<Repository> getNewRepository, IFileStorage storage, bool verboseLog, bool suspendTimerCheck,
            Account accountForCheck, Action<IEnumerable<string>> log, Action<Exception> exceptionLog, Action<WorkerProcessElementEventArgs> workerStateChanged, TimeSpan timerInterval,
            Encoding logFileEncodig, Encoding exportFileEncodig, CultureInfo defaultCultureForExportThread, string defaultImportLogFileName, string defaultExportFileName)
        {
            GetNewRepository = getNewRepository;
            Storage = storage;
            VerboseLog = verboseLog;
            SuspendTimerCheck = suspendTimerCheck;
            AccountForCheck = accountForCheck;
            Log = log;
            ExceptionLog = exceptionLog;
            WorkerStateChanged = workerStateChanged;
            TimerInterval = timerInterval;
            LogFileEncodig = logFileEncodig;
            ExportFileEncodig = exportFileEncodig;
            DefaultCultureForExportThread = defaultCultureForExportThread;
            DefaultImportLogFileName = defaultImportLogFileName;
            DefaultExportFileName = defaultExportFileName;
        }
    }

    /// <summary>
    /// Задача Worker'a - выдергивать элементы из очереди и обрабатывать их
    /// </summary>
    public class RoyaltyWorker : RoyaltyBase<WorkerThreadParameters, Account>
    {

        /// <summary>
        /// Название по-умолчанию файла для экспорта
        /// </summary>
        public string DefaultExportFileName { get; set; } = Config.WorkerConfigSection.DefaultExportFileNameValue;

        /// <summary>
        /// Название по-умолчанию файла для экспорта
        /// </summary>
        public string DefaultImportLogFileName { get; set; } = Config.WorkerConfigSection.DefaultImportLogFileNamValue;

        /// <summary>
        /// Культура для работы потока экспорта
        /// </summary>
        public CultureInfo DefaultCultureForExportThread { get; set; } = new System.Globalization.CultureInfo(Config.WorkerConfigSection.DefaultCultureForExportThreadValue);

        /// <summary>
        /// Log file encoding
        /// </summary>
        public Encoding LogFileEncodig { get; set; } = Encoding.GetEncoding(Config.WorkerConfigSection.DefaultEncoding);

        /// <summary>
        /// Export file encoding
        /// </summary>
        public Encoding ExportFileEncodig { get; set; } = Encoding.GetEncoding(Config.WorkerConfigSection.DefaultEncoding);

        #region Constructor

        /// <summary>
        /// Инициализация нового экземпляра
        /// </summary>
        /// <param name="getNewRepository">Функия для получения нового экземпляра репозитория</param>
        /// <param name="storage">Хранилище файлов</param>
        public RoyaltyWorker(Func<Repository> getNewRepository, IFileStorage storage)
            : this(getNewRepository, storage, false) { }

        /// <summary>
        /// Инициализация нового экземпляра
        /// </summary>
        /// <param name="getNewRepository">Функия для получения нового экземпляра репозитория</param>
        /// <param name="storage">Хранилище файлов</param>
        public RoyaltyWorker(Func<Repository> getNewRepository, IFileStorage storage, bool createStarted)
            : base(getNewRepository, storage, createStarted, TimeSpan.FromSeconds(30)) { }

        #endregion

        /// <summary>
        /// Обрабатываем очередь, пока в ней не кончатся элементы, затем выходим
        /// </summary>
        /// <param name="prm">Входные параметры</param>
        private static void ProcessThreadProc(WorkerThreadParameters prm)
        {
            Thread.CurrentThread.CurrentCulture = prm.DefaultCultureForExportThread;

            var progress = new Helpers.PercentageProgress();
            var processed = false;
            do
            {
                processed = false;
                using (var logSession = Helpers.Log.Session($"{nameof(RoyaltyWorker)}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", prm.VerboseLog, prm.Log))
                    try
                    {
                        using (var rep = prm.GetNewRepository())
                        {
                            var queueItem = (prm.AccountForCheck == null) 
                                ? rep.ImportQueueRecordGet()
                                    .Where(r => r.ProcessedDate == null)
                                    .OrderBy(r => r.CreatedDate)
                                    .FirstOrDefault()
                                : rep.ImportQueueRecordGet()
                                    .Where(r => r.ProcessedDate == null && r.AccountUID == prm.AccountForCheck.AccountUID)
                                    .OrderBy(r => r.CreatedDate)
                                    .FirstOrDefault();

                            if (queueItem != null)
                            {
                                processed = true;
                                logSession.Add($"Queue record found: {queueItem}. Try to process.");

                                var prgImport = progress.GetChild(weight: 0.9m);
                                var prgExport = progress.GetChild(weight: 0.1m);

                                //Get files with progress instances
                                var filesElement = queueItem.FileInfoes
                                    .Where(f => f.Finished == null)
                                    .Select(f => new { File = f, Progress = prgImport.GetChild() })
                                    .ToList();
                                var buildEvent = new Func<WorkerProcessElement>(() => new WorkerProcessElement(queueItem.ImportQueueRecordUID, progress.Value, filesElement.Select(i => new WorkerProcessFileProgress(i.File.ImportQueueRecordFileInfoUID, i.Progress.Value))));

                                logSession.Add($"Queue record files count to process: {filesElement.Count}");

                                prm.WorkerStateChanged(new WorkerProcessElementEventArgs(buildEvent(), WorkerProcessElementAction.Start));
                                progress.Change += (s, e) => prm.WorkerStateChanged(new WorkerProcessElementEventArgs(buildEvent(), WorkerProcessElementAction.Update));
                                try
                                {
                                    try
                                    {
                                        var dataToExport = new List<AccountDataRecord>();

                                        //Process every file
                                        filesElement.ForEach(f => 
                                        {
                                            var exportItems = ImportData(f.File, rep, prm.Storage, p => f.Progress.Value = p, logSession, prm.LogFileEncodig, prm.DefaultImportLogFileName);
                                            dataToExport.AddRange(exportItems);
                                        });

                                        //Check for error
                                        if (queueItem.FileInfoes.Any(f => !string.IsNullOrWhiteSpace(f.Error)))
                                            queueItem.HasError = true;

                                        if (dataToExport.Count > 0)
                                        {
                                            logSession.Add($"Data imported. Start export {dataToExport.Count} records...");
                                            ExportData(filesElement.Select(i => i.File), rep, queueItem.Account, prm.Storage, dataToExport.Distinct(), logSession, i => prgExport.Value = i, prm.ExportFileEncodig, prm.DefaultExportFileName);
                                            logSession.Add($"All export done.");
                                        }
                                        else
                                            prgExport.Value = 100;
                                    }
                                    catch (Exception ex)
                                    {
                                        ex.Data.Add(nameof(queueItem), queueItem);

                                        logSession.Enabled = true;
                                        logSession.Add(ex);
                                        queueItem.Error = ex.GetExceptionText();
                                        prm.ExceptionLog(ex);
                                    }
                                    finally
                                    {
                                        queueItem.ProcessedDate = DateTime.UtcNow;
                                        rep.SaveChanges();
                                    }
                                }
                                finally { prm.WorkerStateChanged(new WorkerProcessElementEventArgs(buildEvent(), WorkerProcessElementAction.End)); }
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
        private static IEnumerable<AccountDataRecord> ImportData(RoyaltyRepository.Models.ImportQueueRecordFileInfo queueFile, Repository repository, IFileStorage storage,
            Action<decimal> progressAction, Helpers.Log.SessionInfo upperLogSession, Encoding logFileEncoding, string defaultImportLogFileName) //Encoding exportFileEncoding, string defaultExportFileName, 
        {
            var res = Enumerable.Empty<AccountDataRecord>();

            if (queueFile == null)
                throw new ArgumentNullException(nameof(queueFile));
            if (repository == null)
                throw new ArgumentNullException(nameof(repository));
            if (storage == null)
                throw new ArgumentNullException(nameof(storage));

            var fileLog = new List<string>();
            var logAction = new Action<string>((s) =>
            {
                fileLog.Add(s);
                upperLogSession?.Add(s);
            });
            progressAction = progressAction ?? new Action<decimal>((i) => { });

            var progress = new Helpers.PercentageProgress();
            progress.Change += (s, e) => progressAction(e.Value);
            var pBegin = progress.GetChild(weight: 0.05m);
            var pAction = progress.GetChild(weight: 0.90m);
            var pEnd = progress.GetChild(weight: 0.05m);

            using (var logSession = Helpers.Log.Session($"{nameof(RoyaltyWorker)}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", true, (s) => s.ToList().ForEach(str => logAction(str))))
                try
                {
                    logSession.Add($"Change state to '{RoyaltyRepository.Models.ImportQueueRecordStateType.TryToProcess}' for '{queueFile}'");

                    queueFile.ImportQueueRecordState = repository.ImportQueueRecordStateGet(RoyaltyRepository.Models.ImportQueueRecordStateType.TryToProcess);
                    repository.SaveChanges();

                    pBegin.Value = 100;

                    var importFile = queueFile
                        .GetFileByType(RoyaltyRepository.Models.ImportQueueRecordFileInfoFileType.Import)
                        .Where(f => f != null)
                        .FirstOrDefault();

                    if (importFile == null)
                        throw new Exception(Properties.Resources.ROYALTYWORKER_ImportFileNotFound);

                    using (var dc = new DataCalculator(queueFile.ImportQueueRecord.Account, repository))
                    {
                        var prgGetFileFromRepository = pAction.GetChild(weight: 0.1m);
                        var prgPrepare = pAction.GetChild(weight: 0.2m);
                        var prgPreview = pAction.GetChild(weight: 0.8m);
                        var prgImport = pAction.GetChild(weight: 0.5m);
                        //var prgExport = pAction.GetChild(weight: 0.4m);

                        logSession.Add($"Read lines from import file '{importFile}'");

                        var csvLines = repository.FileGetLines(storage, importFile);
                        prgGetFileFromRepository.Value = 100;

                        logSession.Add($"Start load lines as CSV...");

                        using (var l = Helpers.CSV.CSVFile.Load(csvLines.AsEnumerable(),
                            verboseLogAction: (s) => logSession.Add(s),
                            tableValidator: dc.TableValidator,
                            rowFilter: dc.RowFilter))
                        { 

                            logSession.Add($"Load lines as CSV done. Prepare table...");
                            dc.Prepare(l.Table, i => prgPrepare.Value = i, s => logSession.Add(s));

                            logSession.Add($"Table prepared. Start preview data...");
                            var previewRes = dc.Preview(l.Table, i => prgPreview.Value = i, s => logSession.Add(s));

                            logSession.Add($"Preview done. Start import...");
                            var readyToExport = dc.Import(previewRes.Values, null, i => prgImport.Value = i, s => logSession.Add(s)).ToList();
                            readyToExport.ForEach(adr => repository.ImportQueueRecordFileAccountDataRecordNew(queueFile, adr));

                            logSession.Add($"Try to save imported data...");
                            repository.SaveChanges();

                            if (queueFile.ForAnalize)
                            {
                                res = readyToExport;

                                //logSession.Add($"Data imported. Start analize and export...");
                                //ExportData(new ImportQueueRecordFileInfo[] { queueFile }, repository, dc.Account, storage, readyToExport, logSession, i => prgExport.Value = i, exportFileEncoding, defaultExportFileName);
                                //logSession.Add($"All export done.");
                            }
                                //else
                                //    prgExport.Value = 100;
                        }
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
                    queueFile.Finished = DateTime.UtcNow;
                    repository.SaveChanges();
                    pEnd.Value = 100;
                }

            if (fileLog.Count > 0)
                try
                { 
                    var logFile = repository.FilePut(storage, fileLog, logFileEncoding, defaultImportLogFileName);
                    repository.ImportQueueRecordFileInfoFileNew(RoyaltyRepository.Models.ImportQueueRecordFileInfoFileType.Log, queueFile, logFile);
                    repository.SaveChanges();
                }
                catch(Exception ex)
                {
                    upperLogSession.Add(ex.GetExceptionText());
                }
            return res;
        }

        #region Export data

        /// <summary>
        /// Создает DataTable для списка данных
        /// </summary>
        /// <param name="settings">Настройки аккаунта (для определения названия колонки)</param>
        /// <param name="recordsToExport">Записи для экспорта</param>
        /// <returns>DataTable с данными</returns>
        private static DataTable GetDataTableFromRecords(AccountSettings settings, IEnumerable<AccountDataRecord> recordsToExport, Action<decimal> progressAction = null)
        {
            progressAction = progressAction ?? new Action<decimal>(i => { });
            var progress = new PercentageProgress();
            progress.Change += (s, e) => progressAction(e.Value);

            var mainColumns = settings.Columns
                .OrderBy(c => c.ColumnType.ExportColumnIndex)
                .Where(c => c.ColumnType.Export)
                .Select(c => new
                {
                    ColumnName = c.ColumnType.Type.ToString(),
                    IsKey = RoyaltyRepository.Extensions.Extensions.GetAttributeOfType<RoyaltyRepository.Models.IsKeyAttribute>(c.ColumnType.Type) != null
                });

            var additionalColumns = settings.Account.AdditionalColumns
                .Where(ad => ad.Export)
                .Select(ad => new
                {
                    ColumnName = ad.ColumnName,
                    IsKey = false
                });

            var allColumns = mainColumns.Union(additionalColumns);
            var keysColumnNames = allColumns.Where(c => c.IsKey).Select(c => c.ColumnName);

            var res = new DataTable();
            res.Columns.AddRange(allColumns.Select(i => new DataColumn(i.ColumnName, typeof(string))).ToArray());
            res.PrimaryKey = res.Columns.Cast<DataColumn>().Where(dc => keysColumnNames.Contains(dc.ColumnName)).ToArray();
            //res.PrimaryKey = allColumns.Where(i => i.IsKey).Select(i => i.Column).ToArray();

            var cnt = recordsToExport.Count();
            var current = 0;
            foreach (var record in recordsToExport)
            {
                var newRow = res.NewRow();
                foreach (var column in settings.Columns)
                    newRow[column.ColumnType.Type.ToString()] = RoyaltyRepository.Models.RepositoryExtensions.GetAccountDataForColumnType(column.ColumnType.Type, record);

                foreach (var column in settings.Account.AdditionalColumns.Where(ad => ad.Export))
                    newRow[column.ColumnName] = record.DataAdditional.GetType().GetProperty(column.ColumnSystemName).GetValue(record.DataAdditional);

                res.Rows.Add(newRow);
                progress.Value = ((decimal)(++current) / (decimal)cnt) * 100m;
            }

            return res;
        }

        /// <summary>
        /// Экспортировать данные
        /// </summary>
        /// <param name="queueFiles">Элемент очереди</param>
        /// <param name="account">Аккаунт для экспорта данных</param>
        /// <param name="repository">Репозиторий</param>
        /// <param name="storage">Хранилище файлов</param>
        /// <param name="readyToExport">Данные для экспорта</param>
        /// <param name="upperLogSession">Журнал сессии верхнего уровня</param>
        /// <param name="progressAction">Действие для отслуживания прогресса выполнения</param>
        /// <param name="exportFileEncoding">Кодировка по умолчанию для файла экспорта</param>
        /// <param name="defaultExportFileName">Имя файла экспорта (для хранения в БД)</param>
        private static void ExportData(IEnumerable<ImportQueueRecordFileInfo> queueFiles, Repository repository, Account account, IFileStorage storage,
            IEnumerable<AccountDataRecord> readyToExport, Log.SessionInfo upperLogSession, Action<decimal> progressAction,
            Encoding exportFileEncoding, string defaultExportFileName)
        {
            progressAction = progressAction ?? new Action<decimal>(i => { });

            using (var logSession = Helpers.Log.Session($"{nameof(RoyaltyWorker)}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", upperLogSession.Enabled, (s) => s.ToList().ForEach(str => upperLogSession.Add(str))))
                try
                {
                    var progress = new Helpers.PercentageProgress();
                    var generateTableProgress = progress.GetChild(weight: 0.3m);
                    var saveProgress = progress.GetChild(weight: 0.7m);
                    var commandProgress = progress.GetChild(weight: 0.05m);

                    progress.Change += (s, e) => progressAction(e.Value);

                    #region Name <-> SystemName

                    var namesDictionary = typeof(RoyaltyRepository.Models.ColumnTypes)
                        .GetEnumValues()
                        .Cast<RoyaltyRepository.Models.ColumnTypes>()
                        .Select(t => new { SystemName = t.ToString().ToUpper(), Name = RoyaltyRepository.Extensions.Extensions.GetEnumNameFromType(t) });

                    var nameToSystemName = new Func<string, string>((name) =>
                    {
                        var item = namesDictionary.FirstOrDefault(i => i.Name.ToUpper() == name.ToUpper());
                        return (item != null) ? item.SystemName : name;
                    });

                    var systemNameToName = new Func<string, string>((name) =>
                    {
                        var item = namesDictionary.FirstOrDefault(i => i.SystemName.ToUpper() == name.ToUpper());
                        return (item != null) ? item.Name : name;
                    });

                    #endregion

                    using (var dataTable = GetDataTableFromRecords(account.Settings, readyToExport, i => generateTableProgress.Value = i))
                    {
                        #region Store file in database

                        logSession.Add($"Store file in databae...");
                        var exportLines = Helpers.CSV.CSVFile.Save(dataTable, columnRenamer: systemNameToName, verboseLogAction: (s) => logSession.Add(s));
                        var dataFile = repository.FilePut(storage, exportLines, exportFileEncoding, defaultExportFileName);
                        foreach(var queueFile in queueFiles)
                            repository.ImportQueueRecordFileInfoFileNew(RoyaltyRepository.Models.ImportQueueRecordFileInfoFileType.Export, queueFile, dataFile);

                        #endregion
                        #region Add export information for each record in data

                        logSession.Add($"Add export information for each record in data...");
                        readyToExport.ToList().ForEach(adr =>
                        {
                            repository.AccountDataRecordExportNew(adr, dataFile, adr.Host);
                            adr.Exported = DateTime.UtcNow;
                        });

                        #endregion

                        var validAccountName = GetValidFileName(account.Name);
                        var dataDirectories = account.Settings.ExportDirectories
                            .Select(ed => new { ExportDirectory = ed, Progress = saveProgress.GetChild() })
                            .ToArray();

                        if (dataDirectories.Length > 0)
                            foreach (var dataDirectory in dataDirectories)
                                try
                                {
                                    if (System.IO.Directory.Exists(dataDirectory.ExportDirectory.DirectoryPath))
                                    {
                                        var exportFilePath = System.IO.Path.Combine(dataDirectory.ExportDirectory.DirectoryPath, $"{(GetValidFileName(dataDirectory.ExportDirectory.FileName) ?? validAccountName)}.csv");
                                        if (System.IO.File.Exists(exportFilePath))
                                        {
                                            logSession.Add($"Merging data with '{exportFilePath}'");
                                            using (var toMergeDataLines = Helpers.CSV.CSVFile.Load(exportFilePath, fileEncoding: dataDirectory.ExportDirectory.Encoding, columnRenamer: nameToSystemName))
                                            {
                                                #region Update marks for stored data
                                                var phoneMarks = from phoneMark in account.PhoneMarks
                                                                 join mark in repository.MarkGet() on phoneMark.MarkID equals mark.MarkID
                                                                 join phone in repository.PhoneGet() on phoneMark.PhoneID equals phone.PhoneID
                                                                 select new { MarkName = mark.Name, phone.PhoneNumber };

                                                toMergeDataLines.Table.Rows.Cast<DataRow>()
                                                   .Join(phoneMarks, dr => dr[RoyaltyRepository.Models.ColumnTypes.Phone.ToString()], p => p.PhoneNumber, (dr, p) => new { DataRow = dr, Mark = p.MarkName })
                                                   .Where(i => i.DataRow[RoyaltyRepository.Models.ColumnTypes.Mark.ToString()]?.ToString() != i.Mark)
                                                   .ToList()
                                                   .ForEach(i => { i.DataRow[RoyaltyRepository.Models.ColumnTypes.Mark.ToString()] = i.Mark; });
                                                #endregion

                                                using (var mergedTable = Helpers.CSV.CSVFile.MergeTables(new DataTable[] { dataTable, toMergeDataLines.Table }, dataTable.PrimaryKey.Select(c => c.ColumnName)))
                                                {
                                                    dataDirectory.Progress.Value = 50;
                                                    SaveTableToLocalFile(exportFilePath, dataDirectory.ExportDirectory.Encoding, dataDirectory.ExportDirectory.Mark, mergedTable, systemNameToName, s => logSession.Add(s));
                                                    dataDirectory.Progress.Value = 100;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            SaveTableToLocalFile(exportFilePath, dataDirectory.ExportDirectory.Encoding, dataDirectory.ExportDirectory.Mark, dataTable, systemNameToName, s => logSession.Add(s));
                                            dataDirectory.Progress.Value = 100;
                                        }

                                        if (!string.IsNullOrWhiteSpace(dataDirectory.ExportDirectory.ExecuteAfterAnalizeCommand))
                                        {
                                            logSession.Add($"Start command after export: {dataDirectory.ExportDirectory.ExecuteAfterAnalizeCommand} with following argument: {exportFilePath}");
                                            ProcessStartInfo startInfo = new ProcessStartInfo(dataDirectory.ExportDirectory.ExecuteAfterAnalizeCommand, $"\"{exportFilePath}\"");
                                            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                                            using (var p = Process.Start(startInfo))
                                            {
                                                if (dataDirectory.ExportDirectory.TimeoutForExecute.TotalMilliseconds > 0 && !p.WaitForExit((int)dataDirectory.ExportDirectory.TimeoutForExecute.TotalMilliseconds))
                                                    p.Kill();
                                            }
                                        }
                                        commandProgress.Value = 100;
                                    }
                                    else
                                        logSession.Add($"Directory '{dataDirectory.ExportDirectory.DirectoryPath}' for export not found");
                                }
                                catch (Exception ex)
                                {
                                    ex.Data.Add(nameof(dataDirectory), dataDirectory);
                                    ex.Data.Add(nameof(validAccountName), validAccountName);
                                    logSession.Add(ex);
                                    logSession.Enabled = true;
                                }
                            
                        else
                            saveProgress.Value = 100;
                    }
                }
                catch (Exception ex)
                {
                    logSession.Add(ex);
                    logSession.Enabled = true;
                }
                finally
                {
                    upperLogSession.Enabled = logSession.Enabled;
                }
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
        private void RaiseExceptionLogEvent(Exception ex)
        {
            ExceptionLog?.Invoke(this, ex);
        }
        private void RaiseStateChanged(WorkerProcessElementEventArgs element)
        {
            WorkerStateChanged?.Invoke(this, element);
        }

        #endregion
        #region Static

        private static string GetValidFileName(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return null;

            var validFileName = fileName;
            foreach (var c in System.IO.Path.GetInvalidFileNameChars())
                validFileName = validFileName.Replace(c.ToString(), string.Empty);
            return string.IsNullOrWhiteSpace(validFileName) ? null : validFileName;
        }

        private static DataTable FilterDataTable(DataTable dataTable, Mark mark)
        {
            var res = new DataTable();
            res.Columns.AddRange(dataTable.Columns.Cast<DataColumn>().Select(c => new DataColumn(c.ColumnName, c.DataType)).ToArray());
            foreach (var row in dataTable.Rows.Cast<DataRow>()
                .Where(r => (mark == null) || (r[ColumnTypes.Mark.ToString()] as string == mark.Name)))
            {
                var newRow = res.NewRow();
                foreach (var c in res.Columns.Cast<DataColumn>())
                    newRow[c] = row[c.ColumnName];
                res.Rows.Add(newRow);
            }
            return res;
        }

        private static void SaveTableToLocalFile(string exportFilePath, Encoding enc, Mark mark, DataTable dt, Func<string,string> systemNameToName, Action<string> logAction)
        {
            using (var dtExport = FilterDataTable(dt, mark))
            {
                logAction($"Exporting data to '{exportFilePath}'");
                using (var res = Helpers.CSV.CSVFile.Save(dtExport, filePath: exportFilePath,
                    encoding: enc,
                    columnRenamer: systemNameToName,
                    verboseLogAction: (s) => logAction(s)))
                {
                    logAction($"Table saved to '{res.FilePath}', processed rows: {res.ProcessedRowCount}");
                }
            }
        }

        #endregion
        #region Abstract implementation

        protected override ConfigurationSection GetConfigSection() => Config.Config.WorkerConfig;

        protected override WorkerThreadParameters GetStartParameters() => new WorkerThreadParameters(
            getNewRepository, storage, VerboseLog, false, null, RaiseLogEvent, RaiseExceptionLogEvent, 
            RaiseStateChanged, TimerInterval, LogFileEncodig, ExportFileEncodig, DefaultCultureForExportThread, DefaultImportLogFileName, DefaultExportFileName);

        protected override WorkerThreadParameters GetRunOnceParameters(Account acc) => new WorkerThreadParameters(
            getNewRepository, storage, VerboseLog, true, acc, RaiseLogEvent, RaiseExceptionLogEvent, 
            RaiseStateChanged, TimerInterval, LogFileEncodig, ExportFileEncodig, DefaultCultureForExportThread, DefaultImportLogFileName, DefaultExportFileName);

        protected override void ThreadProc(WorkerThreadParameters prm) => ProcessThreadProc(prm);

        #endregion
    }
}
