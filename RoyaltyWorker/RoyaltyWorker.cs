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
        /// Название по-умолчанию файла для экспорта
        /// </summary>
        public string DefaultExportFileName { get; set; } = Config.WorkerConfigSection.DefaultExportFileNameValue;

        /// <summary>
        /// Название по-умолчанию файла для экспорта
        /// </summary>
        public string DefaultImportLogFileName { get; set; } = Config.WorkerConfigSection.DefaultImportLogFileNamValue;

        /// <summary>
        /// Суффикс по-умолчанию для файлов с телефонами
        /// </summary>
        public string DefaultNotTrustedPhonesSuffix { get; set; } = Config.WorkerConfigSection.DefaultNotTrustedPhonesSuffixValue;

        /// <summary>
        /// Культура для работы потока экспорта
        /// </summary>
        public CultureInfo DefaultCultureForExportThread { get; set; } = new System.Globalization.CultureInfo(Config.WorkerConfigSection.DefaultCultureForExportThreadValue);

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
                this.CopyObjectFrom(Config.Config.WorkerConfig);

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
                checkQueueThread.Start(new
                {
                    GetNewRepository = this.getNewRepository,
                    FileStorage = this.fileStorage,
                    VerboseLog,
                    LogFileEncodig,
                    DefaultCultureForExportThread,
                    DefaultExportFileName,
                    DefaultImportLogFileName,
                    DefaultNotTrustedPhonesSuffix
                });
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
            var defaultCultureForExportThread = ((dynamic)prm).LogFileEncoding as System.Globalization.CultureInfo;
            var defaultExportFileName = ((dynamic)prm).DefaultExportFileName as string;
            var defaultImportLogFileName = ((dynamic)prm).DefaultImportLogFileName as string;
            var defaultNotTrustedPhonesSuffix = ((dynamic)prm).DefaultNotTrustedPhonesSuffix as string;

            Thread.CurrentThread.CurrentCulture = defaultCultureForExportThread;

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
                                var filesElement = queueItem.FileInfoes
                                    .Where(f => f.Finished == null)
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
                                        filesElement.ForEach(f => ProcessRecordFile(f.File, rep, storage, p => f.Progress.Value = p, (s) => logSession.Add(s)
                                            , logFileEncoding, defaultExportFileName, defaultImportLogFileName, defaultNotTrustedPhonesSuffix));

                                        //Check for error
                                        if (queueItem.FileInfoes.Any(f => !string.IsNullOrWhiteSpace(f.Error)))
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
        private void ProcessRecordFile(RoyaltyRepository.Models.ImportQueueRecordFileInfo queueFile, Repository repository, IFileStorage storage,
            Action<decimal> progressAction, Action<string> logAction, Encoding logFileEncoding, string defaultExportFileName, string defaultImportLogFileName, string defaultNotTrustedPhonesSuffix)
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
            var pBegin = progress.GetChild(weight: 0.05m);
            var pAction = progress.GetChild(weight: 0.90m);
            var pEnd = progress.GetChild(weight: 0.05m);

            using (var logSession = Helpers.Log.Session($"{GetType().Name}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", VerboseLog, (s) => s.ToList().ForEach(str => logAction(str))))
                try
                {
                    logSession.Add($"Change state to '{RoyaltyRepository.Models.ImportQueueRecordStateType.TryToProcess}' for '{queueFile}'");

                    queueFile.ImportQueueRecordState = repository.ImportQueueRecordStateGet(RoyaltyRepository.Models.ImportQueueRecordStateType.TryToProcess);
                    repository.SaveChanges();

                    pBegin.Value = 100;

                    var importFiles = queueFile
                        .GetFileByType(RoyaltyRepository.Models.ImportQueueRecordFileInfoFileType.Import)
                        .Where(f => f != null)
                        .ToList();

                    if (!importFiles.Any())
                        throw new Exception(Properties.Resources.ROYALTYWORKER_ImportFileNotFound);

                    foreach (var importFile in importFiles)
                    {
                        using (var dc = new DataCalculator(queueFile.ImportQueueRecord.Account, repository))
                        {
                            var prg0 = pAction.GetChild(weight: 0.1m);
                            var prg1 = pAction.GetChild(weight: 0.2m);
                            var prg2 = pAction.GetChild(weight: 0.8m);
                            var prg3 = pAction.GetChild(weight: 0.5m);
                            var prg4 = pAction.GetChild(weight: 0.8m);

                            logSession.Add($"Read lines from import file '{importFile}'");

                            var csvLines = repository.FileGetLines(storage, importFile);
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

                                var prg41 = prg4.GetChild(weight: 0.2m);
                                var prg42 = prg4.GetChild(weight: 0.8m);

                                logSession.Add($"Export not trusted phones...");
                                ExportNotTrustedPhones(queueFile, dc, storage, readyToExport, logSession, i => prg41.Value = i, defaultNotTrustedPhonesSuffix);

                                logSession.Add($"Export data...");
                                ExportData(queueFile, repository, dc, storage, readyToExport, logSession, i => prg42.Value = i, defaultExportFileName);

                                logSession.Add($"All export done.");
                            }
                            else
                                prg4.Value = 100;
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
                    var logFile = repository.FilePut(storage, fileLog, logFileEncoding, defaultImportLogFileName);
                    repository.ImportQueueRecordFileInfoFileNew(RoyaltyRepository.Models.ImportQueueRecordFileInfoFileType.Log, queueFile, logFile);
                    queueFile.Finished = DateTime.UtcNow;
                    repository.SaveChanges();
                    pEnd.Value = 100;

                    logSession.Add($"All actions done for '{queueFile}'");
                }
        }

        #region Export data

        /// <summary>
        /// Создает DataTable для списка данных
        /// </summary>
        /// <param name="settings">Настройки аккаунта (для определения названия колонки)</param>
        /// <param name="recordsToExport">Записи для экспорта</param>
        /// <returns>DataTable с данными</returns>
        private DataTable GetDataTableFromRecords(RoyaltyRepository.Models.AccountSettings settings, 
            IEnumerable<RoyaltyRepository.Models.AccountDataRecord> recordsToExport)
        {
            var mainColumns = settings.Columns.Select(c => new { Column = new DataColumn(c.ColumnType.Type.ToString(), typeof(string)), IsKey = RoyaltyRepository.Extensions.Extensions.GetAttributeOfType<RoyaltyRepository.Models.IsKeyAttribute>(c.ColumnType) != null });
            var additionalColumns = settings.Account.AdditionalColumns.Where(ad => ad.Export).Select(ad =>new { Column = new DataColumn(ad.ColumnName, typeof(string)), IsKey = false });

            var allColumns = mainColumns.Union(additionalColumns);

            var res = new DataTable();
            res.Columns.AddRange(allColumns.Select(i => i.Column).ToArray());
            res.PrimaryKey = allColumns.Where(i => i.IsKey).Select(i => i.Column).ToArray();

            foreach(var record in recordsToExport)
            {
                var newRow = res.NewRow();

                record.Exported = DateTime.UtcNow;
                foreach(var column in settings.Columns)
                    newRow[column.ColumnType.Type.ToString()] = RoyaltyRepository.Models.RepositoryExtensions.GetAccountDataForColumnType(column.ColumnType.Type, record);

                foreach (var column in settings.Account.AdditionalColumns.Where(ad => ad.Export))
                    newRow[column.ColumnName] = record.DataAdditional.GetType().GetProperty(column.ColumnSystemName).GetValue(record.DataAdditional);

                res.Rows.Add(newRow);
            }

            return res;
        }

        /// <summary>
        /// Экспортировать данные
        /// </summary>
        /// <param name="queueFile">Элемент очереди</param>
        /// <param name="dc">DataCalculator</param>
        /// <param name="storage">Хранилище файлов</param>
        /// <param name="readyToExport">Данные для экспорта</param>
        /// <param name="upperLogSession">Журнал сессии верхнего уровня</param>
        /// <param name="progressAction">Действие для отслуживания прогресса выполнения</param>
        private void ExportData(RoyaltyRepository.Models.ImportQueueRecordFileInfo queueFile, Repository rep, DataCalculator dc, IFileStorage storage, 
            IEnumerable<RoyaltyRepository.Models.AccountDataRecord> readyToExport, Helpers.Log.SessionInfo upperLogSession, Action<decimal> progressAction,
            string defaultExportFileName)
        {
            progressAction = progressAction ?? new Action<decimal>(i => { });

            using (var logSession = Helpers.Log.Session($"{GetType().Name}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", upperLogSession.Enabled, (s) => s.ToList().ForEach(str => upperLogSession.Add(str))))
                try
                {
                    var progress = new Helpers.PercentageProgress();
                    var saveProgress = progress.GetChild();

                    progress.Change += (s, e) => progressAction(e.Value);

                    using (var dataTable = GetDataTableFromRecords(dc.Account.Settings, readyToExport))
                    {
                        var validFileName = System.IO.Path.GetFileNameWithoutExtension(queueFile.SourceFilePath);
                        foreach (var c in System.IO.Path.GetInvalidFileNameChars())
                            validFileName = validFileName.Replace(c.ToString(), string.Empty);

                        var exportPhonesLines = Helpers.CSV.CSVFile.Save(dataTable, verboseLogAction: (s) => logSession.Add(s));
                        var dataFile = dc.Repository.FilePut(storage, exportPhonesLines, Encoding.Default, defaultExportFileName);
                        dc.Repository.ImportQueueRecordFileInfoFileNew(RoyaltyRepository.Models.ImportQueueRecordFileInfoFileType.ExportPhones, queueFile, dataFile);

                        logSession.Add($"Add export information for each record in data...");
                        readyToExport.ToList().ForEach(adr => rep.AccountDataRecordExportNew(adr, dataFile, adr.Host));

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

                        var saveTable = new Action<string,Encoding,DataTable>((exportFilePath, enc, dt) =>
                        {
                            logSession.Add($"Exporting data to '{exportFilePath}'");
                            using (var res = Helpers.CSV.CSVFile.Save(dt, filePath: exportFilePath,
                                encoding: enc,
                                columnRenamer: systemNameToName,
                                verboseLogAction: (s) => logSession.Add(s)))
                            {
                                logSession.Add($"Table saved to '{res.FilePath}', processed rows: {res.ProcessedRowCount}");
                            }
                        });

                        var dataExportDirectories = dc.Account.Settings.ExportDirectories
                            .Where(ed => ed.ExportData);
                        foreach (var dataDirectory in dataExportDirectories)
                        {
                            try
                            {
                                if (System.IO.Directory.Exists(dataDirectory.Path))
                                {
                                    var exportFilePath = System.IO.Path.Combine(dataDirectory.Path, $"{validFileName}.csv");
                                    if (System.IO.File.Exists(exportFilePath))
                                    {
                                        logSession.Add($"Merging data with '{exportFilePath}'");
                                        using (var toMergeDataLines = Helpers.CSV.CSVFile.Load(exportFilePath, fileEncoding: dataDirectory.Encoding, columnRenamer: nameToSystemName))
                                        {
                                            #region Update marks for stored data
                                            var phoneMarks = from phoneMark in queueFile.ImportQueueRecord.Account.PhoneMarks
                                                             join mark in rep.MarkGet() on phoneMark.MarkID equals mark.MarkID
                                                             join phone in rep.PhoneGet() on phoneMark.PhoneID equals phone.PhoneID
                                                             select new { MarkName = mark.Name, phone.PhoneNumber };

                                            toMergeDataLines.Table.Rows.Cast<DataRow>()
                                               .Join(phoneMarks, dr => dr[RoyaltyRepository.Models.ColumnTypes.Phone.ToString()], p => p.PhoneNumber, (dr, p) => new { DataRow = dr, Mark = p.MarkName })
                                               .Where(i => i.DataRow[RoyaltyRepository.Models.ColumnTypes.Mark.ToString()]?.ToString() != i.Mark)
                                               .ToList()
                                               .ForEach(i => { i.DataRow[RoyaltyRepository.Models.ColumnTypes.Mark.ToString()] = i.Mark; });
                                            #endregion

                                            var mergeProgress = progress.GetChild();

                                            using (var mergedTable = Helpers.CSV.CSVFile.MergeTables(new DataTable[] { dataTable, toMergeDataLines.Table }, dataTable.PrimaryKey.Select(c => c.ColumnName)))
                                            {
                                                mergeProgress.Value = 100;
                                                logSession.Add($"Exporting merged data to '{exportFilePath}'");
                                                saveTable(exportFilePath, dataDirectory.Encoding, mergedTable);
                                                saveProgress.Value = 100;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        logSession.Add($"Exporting data to '{exportFilePath}'");
                                        saveTable(exportFilePath, dataDirectory.Encoding, dataTable);
                                        saveProgress.Value = 100;
                                    }
                                }
                                else
                                    logSession.Add($"Directory '{dataDirectory.Path}' for export phones not found");
                            }
                            catch (Exception ex)
                            {
                                ex.Data.Add(nameof(dataDirectory), dataDirectory);
                                ex.Data.Add(nameof(validFileName), validFileName);
                                logSession.Add(ex);
                                logSession.Enabled = true;
                            }
                        }
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
        #region Export not trusted phones

        /// <summary>
        /// Создает DataTable для списка телефонов
        /// </summary>
        /// <param name="settings">Настройки аккаунта (для определения названия колонки)</param>
        /// <param name="phonesToExport">Телефоны для экспорта</param>
        /// <returns>DataTable с телефонами</returns>
        private DataTable GetDataTableFromPhones(RoyaltyRepository.Models.AccountSettings settings, IEnumerable<RoyaltyRepository.Models.Phone> phonesToExport)
        {
            var phoneColumnName = settings.GetColumnByType(RoyaltyRepository.Models.ColumnTypes.Phone)?.ColumnName 
                ?? RoyaltyRepository.Models.ColumnTypes.Phone.ToString();

            var res = new DataTable();
            res.Columns.Add(phoneColumnName, typeof(string));
            res.PrimaryKey = new DataColumn[] { res.Columns[0] };

            foreach (var p in phonesToExport.Select(p => new object[] { p.PhoneNumber }))
                res.Rows.Add(p);

            return res;
        }

        /// <summary>
        /// Экспорт недоверенных телефонов
        /// </summary>
        /// <param name="dc">DataCalculator</param>
        /// <param name="storage">Хранилище данных</param>
        /// <param name="readyToExport">Данные, подготовленные для экспорта</param>
        /// <param name="upperLogSession">Сессия журнала верхнего уровня</param>
        private void ExportNotTrustedPhones(RoyaltyRepository.Models.ImportQueueRecordFileInfo queueFile, DataCalculator dc, IFileStorage storage, 
            IEnumerable<RoyaltyRepository.Models.AccountDataRecord> readyToExport, Helpers.Log.SessionInfo upperLogSession, Action<decimal> progressAction,
            string defaultNotTrustedPhonesSuffix)
        {
            progressAction = progressAction ?? new Action<decimal>(i => { });

            using (var logSession = Helpers.Log.Session($"{GetType().Name}.{System.Reflection.MethodBase.GetCurrentMethod().Name}()", upperLogSession.Enabled, (s) => s.ToList().ForEach(str => upperLogSession.Add(str))))
                try
                {
                    var progress = new Helpers.PercentageProgress();
                    var saveProgress = progress.GetChild();

                    progress.Change += (s, e) => progressAction(e.Value);

                    var notTrustedPhones = dc.GetExportNotTrustedPhones(readyToExport);
                    using (var exportNotTrustedPhonesDataTable = GetDataTableFromPhones(dc.Account.Settings, notTrustedPhones))
                    {
                        var validFileName = System.IO.Path.GetFileNameWithoutExtension(queueFile.SourceFilePath);
                        foreach (var c in System.IO.Path.GetInvalidFileNameChars())
                            validFileName = validFileName.Replace(c.ToString(), string.Empty);

                        var exportPhonesLines = Helpers.CSV.CSVFile.Save(exportNotTrustedPhonesDataTable, verboseLogAction: (s) => logSession.Add(s));
                        var phoneFile = dc.Repository.FilePut(storage, exportPhonesLines, Encoding.Default, $"data-{defaultNotTrustedPhonesSuffix}.csv");
                        dc.Repository.ImportQueueRecordFileInfoFileNew(RoyaltyRepository.Models.ImportQueueRecordFileInfoFileType.ExportPhones, queueFile, phoneFile);

                        var phonesExportDirectories = dc.Account.Settings.ExportDirectories
                            .Where(ed => ed.ExportPhones);
                        foreach (var phoneDirectory in phonesExportDirectories)
                        {
                            try
                            {
                                if (System.IO.Directory.Exists(phoneDirectory.Path))
                                {
                                    var exportFilePath = System.IO.Path.Combine(phoneDirectory.Path, $"{validFileName}-{defaultNotTrustedPhonesSuffix}.csv");
                                    if (System.IO.File.Exists(exportFilePath))
                                    {
                                        logSession.Add($"Merging not trusted phones with '{exportFilePath}'");
                                        using (var toMergePhoneLines = Helpers.CSV.CSVFile.Load(exportFilePath, hasColumns: false, fileEncoding: phoneDirectory.Encoding))
                                        {
                                            if (toMergePhoneLines.Table.Columns.Count == 0)
                                                throw new Exception("Bad file format");

                                            for (int i = 0; i < exportNotTrustedPhonesDataTable.Columns.Count; i++)
                                                toMergePhoneLines.Table.Columns[i].ColumnName = exportNotTrustedPhonesDataTable.Columns[i].ColumnName;

                                            var mergeProgress = progress.GetChild();

                                            using (var mergedTable = Helpers.CSV.CSVFile.MergeTables(new DataTable[] { exportNotTrustedPhonesDataTable, toMergePhoneLines.Table }, exportNotTrustedPhonesDataTable.PrimaryKey.Select(c => c.ColumnName)))
                                            {
                                                mergeProgress.Value = 100;
                                                logSession.Add($"Exporting not trusted phones to '{exportFilePath}'");
                                                using (var res = Helpers.CSV.CSVFile.Save(mergedTable, filePath: exportFilePath,
                                                    hasColumns: false,
                                                    encoding: phoneDirectory.Encoding,
                                                    verboseLogAction: (s) => logSession.Add(s)))
                                                {
                                                    logSession.Add($"Table merged to '{res.FilePath}', processed rows: {res.ProcessedRowCount}");
                                                    saveProgress.Value = 100;
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        logSession.Add($"Exporting not trusted phones to '{exportFilePath}'");
                                        using (var res = Helpers.CSV.CSVFile.Save(exportNotTrustedPhonesDataTable, filePath: exportFilePath,
                                            hasColumns: false,
                                            encoding: phoneDirectory.Encoding,
                                            verboseLogAction: (s) => logSession.Add(s)))
                                        {
                                            logSession.Add($"Table saved to '{res.FilePath}', processed rows: {res.ProcessedRowCount}");
                                            saveProgress.Value = 100;
                                        }
                                    }
                                }
                                else
                                    logSession.Add($"Directory '{phoneDirectory.Path}' for export phones not found");
                            }
                            catch (Exception ex)
                            {
                                ex.Data.Add(nameof(phoneDirectory), phoneDirectory);
                                ex.Data.Add(nameof(validFileName), validFileName);
                                logSession.Add(ex);
                                logSession.Enabled = true;
                            }
                        }
                    }
                }
                catch(Exception ex)
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
