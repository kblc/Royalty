using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RoyaltyRepository;
using RoyaltyRepository.Models;
using RoyaltyDataCalculator.Properties;
using System.Linq.Expressions;
using RoyaltyRepository.Extensions;

using Helpers;
using Helpers.Linq;
using RoyaltyDataCalculator.Model;

namespace RoyaltyDataCalculator
{
    /// <summary>
    /// Задача калькулятора - взять данные (импортируемые и существующие), обработать их и объединить
    /// </summary>
    public class DataCalculator : IDisposable
    {
        /// <summary>
        /// Создание экземпляра класса
        /// </summary>
        /// <param name="account">Аккаунт</param>
        /// <param name="repository">Репозитарий</param>
        public DataCalculator(Account account, Repository repository)
        {
            if (account == null)
                throw new ArgumentNullException(nameof(account));
            if (repository == null)
                throw new ArgumentNullException(nameof(repository));

            Account = account;
            Repository = repository;

            Init();
        }

        private void Init()
        {
            var columnNamesForTableValidation = Account.Settings.Columns
                .Where(c => c.ColumnType.ImportTableValidation)
                .Select(c => c.ColumnName.ToLower())
                .ToArray();

            var columnNamesForRowFilter = Account.Settings.Columns
                .Where(c => c.ColumnType.ImportRowValidation)
                .Select(c => c.ColumnName.ToLower())
                .ToArray();

            TableValidator = GetDefaultDataTableValidator(columnNamesForTableValidation, Account);
            RowFilter = GetDefaultRowFilter(columnNamesForTableValidation, Account);
        }

        /// <summary>
        /// Текущий аккаунт
        /// </summary>
        public Account Account { get; private set; }
        /// <summary>
        /// Репозитарий
        /// </summary>
        public Repository Repository { get; private set; }

        /// <summary>
        /// Использовать ли словарь при обработке данных (возможно добавление данных в сам словарь)
        /// </summary>
        public bool UseDictionary { get; set; } = true;

        public bool VerboseLog { get; set; } = false;

        /// <summary>
        /// Default table validator
        /// </summary>
        public Action<DataTable> TableValidator { get; set; }
        /// <summary>
        /// Default row filter
        /// </summary>
        public Expression<Func<DataRow, bool>> RowFilter { get; set; }

        /// <summary>
        /// Получить соответствия данным из загружаемой таблицы
        /// </summary>
        /// <param name="dataTable">Загружаемая таблица</param>
        /// <param name="progressAction">Действие для отображения прогресса</param>
        /// <param name="logAction">Действие для отображения лога</param>
        /// <returns>Словарь соответсвия найденных данных для каждой строки таблицы</returns>
        public IDictionary<DataRow, DataPreviewRow> Preview(DataTable dataTable, Action<decimal> progressAction = null, Action<string> logAction = null)
        {
            progressAction = progressAction ?? new Action<decimal>((i) => { });
            logAction = logAction ?? new Action<string>((i) => { });

            using (var logSession = Log.Session($"{this.GetType().Name}.{nameof(Preview)}()", VerboseLog))
                try
                {
                    logSession.Output = (strs) => strs.ToList().ForEach(s => logAction(s));

                    if (dataTable == null)
                        throw new ArgumentNullException(nameof(dataTable));

                    var pp = new Helpers.PercentageProgress();
                    var ppPrepare = pp.GetChild(weight: 0.1m);
                    var ppHostes = pp.GetChild(weight: 0.1m);
                    var ppPhones = pp.GetChild(weight: 0.1m);
                    var ppMarks = pp.GetChild(weight: 0.1m);
                    var ppCities = pp.GetChild(weight: 0.1m);
                    var ppStreets = pp.GetChild(weight: 0.7m);
                    var ppAddresses = pp.GetChild(weight: 0.15m);
                    var ppRows = pp.GetChild(weight: 0.3m);
                    pp.Change += (s, e) => progressAction(e.Value);

                    #region Get column names by column types
                    logSession.Add("Get column names by column types");

                    var columnNames = dataTable.Columns.OfType<DataColumn>().Select(c => c.ColumnName.ToUpper()).ToArray();

                    var columnByTypes = typeof(ColumnTypes)
                        .GetEnumValues()
                        .Cast<ColumnTypes>()
                        .Select(ct => new
                        {
                            Type = ct,
                            Column = Account.Settings.GetColumnFor(ct),
                        })
                        .Select(i => new
                        {
                            i.Type,
                            i.Column,
                            ExistsInDataTable = columnNames.Contains(i.Column.ColumnName.ToUpper()),
                        }).ToArray();

                    var badColumns = string.Empty;
                    foreach (var badColumn in columnByTypes.Where(c => c.Column == null))
                        badColumns += (string.IsNullOrWhiteSpace(badColumns) ? string.Empty : ", ") + ColumnType.GetNameFromType(badColumn.Type);
                    if (!string.IsNullOrWhiteSpace(badColumns))
                        throw new Exception(string.Format(Resources.DataCalculator_Preview_ColumnInSettingsNotSetted, badColumns));

                    var columnDict = columnByTypes.ToDictionary(i => i.Type, i => new { i.Column.ColumnName, i.ExistsInDataTable });

                    #endregion
                    #region Prepare data
                    logSession.Add("Parse incoming data");

                    var excludes = Account.Dictionary.Excludes
                        .Select(e => e.Exclude)
                        .ToArray();

                    var subRes0 = dataTable.Rows
                        .Cast<DataRow>()
                        .AsParallel()
                        .Select(dr => new
                        {
                            Row = dr,
                            IncomingAddressValue = columnDict[ColumnTypes.Address].ExistsInDataTable ? dr[columnDict[ColumnTypes.Address].ColumnName] : null,
                            IncomingHostValue = columnDict[ColumnTypes.Host].ExistsInDataTable ? dr[columnDict[ColumnTypes.Host].ColumnName] : null,
                            IncomingPhoneValue = columnDict[ColumnTypes.Phone].ExistsInDataTable ? dr[columnDict[ColumnTypes.Phone].ColumnName] : null,
                            IncomingCityValue = columnDict[ColumnTypes.City].ExistsInDataTable ? dr[columnDict[ColumnTypes.City].ColumnName] : null,
                            IncomingAreaValue = columnDict[ColumnTypes.Area].ExistsInDataTable ? dr[columnDict[ColumnTypes.Area].ColumnName] : null,
                            IncomingMarkValue = columnDict[ColumnTypes.Mark].ExistsInDataTable ? dr[columnDict[ColumnTypes.Mark].ColumnName] : null,
                        })
                        .Select(dr => new
                        {
                            dr.Row,
                            IncomingAddress = (dr.IncomingAddressValue == DBNull.Value || dr.IncomingAddressValue == null) ? string.Empty : dr.IncomingAddressValue.ToString(),
                            IncomingHost = (dr.IncomingHostValue == DBNull.Value || dr.IncomingHostValue == null) ? string.Empty : dr.IncomingHostValue.ToString(),
                            IncomingPhone = (dr.IncomingPhoneValue == DBNull.Value || dr.IncomingPhoneValue == null) ? string.Empty : dr.IncomingPhoneValue.ToString(),
                            IncomingCity = (dr.IncomingCityValue == DBNull.Value || dr.IncomingCityValue == null) ? string.Empty : dr.IncomingCityValue.ToString(),
                            IncomingArea = (dr.IncomingAreaValue == DBNull.Value || dr.IncomingAreaValue == null) ? string.Empty : dr.IncomingAreaValue.ToString(),
                            IncomingMark = (dr.IncomingMarkValue == DBNull.Value || dr.IncomingMarkValue == null) ? string.Empty : dr.IncomingMarkValue.ToString(),
                        })
                        .Select(dr => new
                        {
                            dr.Row,
                            IncomingAddress = Parser.Address.FromString(dr.IncomingAddress, dr.IncomingArea, excludes),
                            IncomingHost = Parser.Host.FromString(dr.IncomingHost),
                            IncomingPhone = Parser.Phone.FromString(dr.IncomingPhone),
                            dr.IncomingCity,
                            dr.IncomingMark,
                        })
                        .ToArray();

                    ppPrepare.Value = 100;

                    #endregion
                    #region Join hostes or create new
                    logSession.Add("Join hostes or create new");

                    var hosts = subRes0
                        .AsParallel()
                        .Select(i => i.IncomingHost.Hostname)
                        .Distinct()
                        .LeftOuterJoin(Repository.HostGet(), h => h.ToUpper(), h => h.Name.ToUpper(), (h, host) => new { HostName = h, Host = host })
                        .ToArray()
                        .Select(i => i.Host ?? Repository.HostNew(i.HostName))
                        .ToArray();

                    ppHostes.Value = 50;

                    var subRes1 = subRes0
                        .Join(hosts, i => i.IncomingHost.Hostname, h => h.Name, (i, h) => new
                        {
                            i.Row,
                            i.IncomingAddress,
                            i.IncomingCity,
                            i.IncomingPhone,
                            i.IncomingHost,
                            i.IncomingMark,
                            Host = h,
                        });

                    ppHostes.Value = 100;
                    #endregion
                    #region Join phones or create new
                    logSession.Add("Join phones or create new");

                    var phones = subRes0
                        .AsParallel()
                        .Select(i => i.IncomingPhone.PhoneNumber)
                        .Distinct()
                        .LeftOuterJoin(Repository.PhoneGet(), p => p, p => p.PhoneNumber, (p, phone) => new { PhoneNumber = p, Phone = phone })
                        .ToArray()
                        .Select(i => i.Phone ?? Repository.PhoneNew(i.PhoneNumber, Account))
                        .ToArray();

                    ppPhones.Value = 50;

                    var subRes2 = subRes1
                        .Join(phones, i => i.IncomingPhone.PhoneNumber, p => p.PhoneNumber, (i, p) => new
                        {
                            i.Row,
                            i.IncomingPhone,
                            i.IncomingAddress,
                            i.IncomingCity,
                            i.IncomingHost,
                            i.IncomingMark,
                            i.Host,
                            Phone = p,
                        });

                    ppPhones.Value = 100;
                    #endregion
                    #region Join marks
                    logSession.Add("Join marks");

                    var defMark = Repository.MarkGet(MarkTypes.Unknown);

#pragma warning disable 618
                    var subRes3 = subRes2
                        .LeftOuterJoin(Repository.MarkGet(), i => i.IncomingMark.ToUpper(), m => m.SystemName.ToUpper(), (i, m) => new
                        {
                            i.Row,
                            i.IncomingPhone,
                            i.IncomingAddress,
                            i.IncomingCity,
                            i.IncomingHost,
                            i.IncomingMark,
                            i.Host,
                            i.Phone,
                            Mark = m ?? defMark,
                        });
#pragma warning restore 618
                    ppMarks.Value = 100;

                    #endregion
                    #region Join cities or create new
                    logSession.Add("Join cities or create new");

                    var cities = subRes0
                        .AsParallel()
                        .Select(i => i.IncomingCity)
                        .Distinct()
                        .LeftOuterJoin(Repository.CityGet(), c => c.ToUpper(), c => c.Name.ToUpper(), (c, city) => new { CityName = c, City = city })
                        .ToArray()
                        .Select(i => i.City ?? Repository.CityNew(i.CityName))
                        .ToArray();

                    ppCities.Value = 50;

                    var subRes4 = subRes3
                        .Join(cities, i => i.IncomingCity.ToUpper(), c => c.Name.ToUpper(), (i, c) => new
                        {
                            i.Row,
                            i.IncomingPhone,
                            i.IncomingAddress,
                            i.IncomingCity,
                            i.IncomingHost,
                            i.IncomingMark,
                            i.Host,
                            i.Phone,
                            i.Mark,
                            City = c
                        });

                    ppCities.Value = 100;
                    #endregion
                    #region Join streets

                    logSession.Add("Join streets");

                    var aP = new Parser.AddressParser(Account, Repository);

                    var aPR = aP.Parse(subRes4
                        .GroupBy(i => new { i.City, i.IncomingAddress.Street, House = i.IncomingAddress.House.ToString(), i.IncomingAddress.Area })
                        .Select(g => new
                        {
                            City = g.Key.City,
                            Address = new Parser.Address(g.Key.Street, g.Key.House, g.Key.Area)
                        })
                        .Select(g => new Parser.AddressParserIncomingParameter()
                        {
                            City = g.City,
                            Address = g.Address
                        })
                        .ToArray(),
                        !UseDictionary,
                        (progress) => ppStreets.Value = progress,
                        (str) => logSession.Add(str));

                    var subRes5 = subRes4
                        .LeftOuterJoin(aPR,
                            r => new { r.IncomingAddress.Street, House = r.IncomingAddress.House.ToString(), r.IncomingAddress.Area, r.City.Name },
                            i => new { i.Key.Address.Street, House = i.Key.Address.House.ToString(), i.Key.Address.Area, i.Key.City.Name },
                            (r, i) => new
                            {
                                r.Row,
                                LoadedRow = new
                                {
                                    i.Value.Address, //change old address to new address
                                    i.Value.Street,
                                    r.Mark,
                                    r.Phone,
                                    r.City,
                                    r.Host,
                                },
                            });

                    #endregion
                    #region Update addresses

                    logSession.Add("Update house numbers");

                    var expectedData = Account.Data
                        .Where(d => !string.IsNullOrWhiteSpace(d.HouseNumber))
                        .GroupBy(d => new { d.Phone, d.Street })
                        .Select(d => new { d.Key.Phone, d.Key.Street, Houses = d.Select(i => i.HouseNumber), Cnt = d.Count() })
                        .Where(i => i.Cnt == 1)
                        .Select(i => new { i.Phone, i.Street, HouseNumber = i.Houses.FirstOrDefault() });

                    var currentData = subRes5
                        .Where(d => !string.IsNullOrWhiteSpace(d.LoadedRow.Address.House.ToString()))
                        .GroupBy(d => new { d.LoadedRow.Phone, d.LoadedRow.Street })
                        .Select(d => new { d.Key.Phone, d.Key.Street, Houses = d.Select(i => i.LoadedRow.Address.House.Number), Cnt = d.Count() })
                        .Where(i => i.Cnt == 1)
                        .Select(i => new { i.Phone, i.Street, HouseNumber = i.Houses.FirstOrDefault() });

                    ppAddresses.Value = 50;

                    var subRes6 = subRes5
                        .LeftOuterJoin(currentData, r => new { r.LoadedRow.Phone.PhoneNumber, r.LoadedRow.Street }, d => new { d?.Phone?.PhoneNumber, d?.Street }, (r, d) => new { Data = r, Grouped = d })
                        .LeftOuterJoin(expectedData, r => new { r.Data.LoadedRow.Phone.PhoneNumber, r.Data.LoadedRow.Street }, d => new { d?.Phone?.PhoneNumber, d?.Street },
                            (r, d) => new
                            {
                                r.Data.Row,
                                LoadedRow = new
                                {
                                    //Address = new Parser.Address(r.Data.LoadedRow.Address.Street, (r.Data.LoadedRow.Address.House.Number?.ToString() ?? r.Grouped?.HouseNumber?.ToString() ?? d?.HouseNumber ?? string.Empty), r.Data.LoadedRow.Address.Area),
                                    HouseNumber = (r.Data.LoadedRow.Address.House.Number?.ToString() ?? r.Grouped?.HouseNumber?.ToString() ?? d?.HouseNumber ?? string.Empty),
                                    r.Data.LoadedRow.Street,
                                    r.Data.LoadedRow.Mark,
                                    r.Data.LoadedRow.Phone,
                                    r.Data.LoadedRow.City,
                                    r.Data.LoadedRow.Host,
                                }
                            });

                    ppAddresses.Value = 100;

                    #endregion
                    #region Load rows

                    logSession.Add("Load rows");

                    var ppRowsPrepare = ppRows.GetChild();
                    var ppRowsLoad = ppRows.GetChild();

                    var rowCount0 = (decimal)subRes6.Count() - 1;
                    var currentIndex0 = 0m;

                    var res = subRes6.Select(r => new
                    {
                        r.Row,
                        DataRecord = Repository.AccountDataRecordNew(Account, r.LoadedRow),
                        Index = (ppRowsPrepare.Value = (currentIndex0++) / rowCount0 * 100m)
                    })
                    .Select(r => new
                    {
                        r.Row,
                        LoadedRow = new DataPreviewRow()
                        {
                            DataRecord = r.DataRecord,
                            DataRecordAdditional = Repository.AccountDataRecordAdditionalNew(r.DataRecord),
                        },
                    }
                    )
                    .ToArray();

                    var rowCount1 = (decimal)dataTable.Rows.Count - 1;
                    var currentIndex1 = 0m;
                    var columns = Account.AdditionalColumns.Where(c => !string.IsNullOrWhiteSpace(c.ColumnName));

                    res.ToList()
                        .ForEach(r =>
                        {
                            foreach (var c in columns)
                            {
                                var pi = r.LoadedRow.DataRecordAdditional.GetType().GetProperty(c.ColumnSystemName);
                                pi.SetValue(r.LoadedRow.DataRecordAdditional, r.Row[c.ColumnName]);
                            }
                            ppRowsLoad.Value = currentIndex1++ / rowCount1 * 100m;
                        });
                    #endregion
                    return res.ToDictionary(i => i.Row, i => i.LoadedRow);
                }
                catch (Exception ex)
                {
                    logSession.Add(ex);
                    logSession.Enabled = true;
                    throw;
                }
        }

        /// <summary>
        /// Подготовка таблицы аккаунта для импорта
        /// </summary>
        /// <param name="dataTable">Загружаемая таблица</param>
        /// <param name="progressAction">Действие для отображения прогресса</param>
        /// <param name="logAction">Действие для отображения лога</param>
        public void Prepare(DataTable dataTable, Action<decimal> progressAction = null, Action<string> logAction = null)
        {
            progressAction = progressAction ?? new Action<decimal>((i) => { });
            logAction = logAction ?? new Action<string>((i) => { });

            using (var logSession = Log.Session($"{GetType().Name}.{nameof(Prepare)}()", VerboseLog))
                try
                {
                    logSession.Output = (strs) => strs.ToList().ForEach(s => logAction(s));

                    if (dataTable == null)
                        throw new ArgumentNullException(nameof(dataTable));

                    var progress = new Helpers.PercentageProgress();
                    progress.Change += (s, e) => progressAction(e.Value);

                    logSession.Add($"Get ignore columns");
                    var columnNamesToIgnore = Account.Settings.Columns.Select(c => c.ColumnName.ToUpper());
                    progress.Value = 10;

                    logSession.Add($"Get columns to add from data table");
                    var dataColumnNamesToAdd = dataTable.Columns.OfType<DataColumn>().Select(dc => dc.ColumnName).Where(dc => !columnNamesToIgnore.Contains(dc.ToUpper()));
                    progress.Value = 20;

                    logSession.Add($"Join additional columns with columns to add");
                    var namesToAdd = dataColumnNamesToAdd
                        .LeftOuterJoin(Account.AdditionalColumns, n => n.ToUpper(), c => c.ColumnName.ToUpper(), (n, c) => new { ColumnName = n, AdditionalColumn = c })
                        .Where(c => c.AdditionalColumn == null)
                        .Select(c => c.ColumnName).ToArray()
                        .ToArray();
                    progress.Value = 30;

                    logSession.Add($"Get existed additional columns");
                    var existingColumnSystemNames = Account.AdditionalColumns.Select(ac => ac.ColumnSystemName).ToArray();
                    progress.Value = 40;

                    logSession.Add($"Check free additional columns");
                    var freeColumnNames = Enumerable.Range(0, (int)AccountDataRecordAdditional.ColumnCount)
                        .Select(i => new { Index = i, ColumnName = $"Column{i.ToString("00")}" })
                        .LeftOuterJoin(existingColumnSystemNames, n => n.ColumnName.ToUpper(), n => n?.ToUpper(), (n0, n1) => new { n0.Index, n0.ColumnName, Exists = n1 != null })
                        .Where(i => !i.Exists)
                        .OrderBy(i => i.Index)
                        .Select(i => i.ColumnName)
                        .ToArray();
                    progress.Value = 50;

                    logSession.Add($"Free columns count: {freeColumnNames.Length} and need to add folowing ({namesToAdd.Length}) columns: {namesToAdd.Select(c => $"'{c}'").Concat(i => i, ", ")}");
                    for (int i = 0; i < Math.Min(namesToAdd.Length, freeColumnNames.Length); i++)
                    {
                        var adrac = Repository.AccountDataRecordAdditionalColumnNew(Account, namesToAdd[i], freeColumnNames[i]);
                        logSession.Add($"Additional column added: '{adrac}'");
                    }
                    progress.Value = 100;
                }
                catch (Exception ex)
                {
                    logSession.Add(ex);
                    logSession.Enabled = true;
                    throw;
                }
        }

        /// <summary>
        /// Вставить записи в основную таблицу данных аккаунта
        /// </summary>
        /// <param name="previewRows">Rows to insert</param>
        /// <param name="importFile">Import queue file</param>
        /// <param name="progressAction">Действие для отображения прогресса</param>
        /// <param name="logAction">Действие для отображения лога</param>
        /// <returns>Возвращает обработанные данные, которые можно экспортировать</returns>
        public IEnumerable<AccountDataRecord> Import(IEnumerable<DataPreviewRow> previewRows, ImportQueueRecordFile importFile, Action<decimal> progressAction = null, Action<string> logAction = null)
        {
            progressAction = progressAction ?? new Action<decimal>((i) => { });
            logAction = logAction ?? new Action<string>((i) => { });

            using (var logSession = Log.Session($"{this.GetType().Name}.{nameof(Import)}()", VerboseLog))
                try
                {
                    logSession.Output = (strs) => strs.ToList().ForEach(s => logAction(s));

                    if (previewRows == null)
                        throw new ArgumentNullException(nameof(previewRows));

                    var progress = new Helpers.PercentageProgress();
                    var prgPreparation = progress.GetChild(weight: 0.3m);
                    var prgUpdateMarks = progress.GetChild(weight: 0.6m);
                    var prgPrepareExport = progress.GetChild(weight: 0.1m);
                    progress.Change += (s, e) => progressAction(e.Value);

                    logSession.Add($"grouping {previewRows.Count()} rows...");

                    var dataToImport = previewRows
                        .GroupBy(i => new { i.DataRecord.Phone, i.DataRecord.Street, i.DataRecord.HouseNumber })
                        .Select(i => i.LastOrDefault())
                        .LeftOuterJoin(Account.Data, g => new { g.DataRecord.Phone, g.DataRecord.Street, g.DataRecord.HouseNumber }, d => new { d.Phone, d.Street, d.HouseNumber }, 
                        (g, d) => new
                        {
                            Existed = d,
                            Insert = g
                        })
                        .ToList();

                    if (importFile != null)
                        dataToImport.ForEach(r => Repository.ImportQueueRecordFileAccountDataRecordNew(importFile, r.Existed ?? r.Insert.DataRecord));

                    prgPreparation.Value = 50;
                    logSession.Add($"grouping rows done. Insert it in account data.");

                    //Repository.AccountDataRecordAdd(dataToImport.Where(i => i.Existed == null).Select(i => i.Insert.DataRecord), Account, false);

                    dataToImport.ForEach(i =>
                    {
                        if (i.Existed == null)
                        {
                            Repository.AccountDataRecordAdd(i.Insert.DataRecord, saveAfterInsert: false);
                        }
                        else
                        {
                            i.Insert.DataRecord.CopyObject(i.Existed, new string[] { nameof(i.Insert.DataRecord.DataAdditional), nameof(i.Insert.DataRecord.LoadedByQueueFiles), nameof(i.Insert.DataRecord.AccountDataRecordID), nameof(i.Insert.DataRecord.Created), nameof(i.Insert.DataRecord.Exported) });
                            i.Existed.CopyObject(i.Insert.DataRecord, new string[] { nameof(i.Insert.DataRecord.DataAdditional), nameof(i.Insert.DataRecord.LoadedByQueueFiles), nameof(i.Insert.DataRecord.AccountDataRecordID) });

                            i.Insert.DataRecordAdditional.CopyObject(i.Existed.DataAdditional, new string[] { nameof(i.Insert.DataRecordAdditional.AccountDataRecordID) });
                        }
                    });

                    prgPreparation.Value = 100;

                    UpdateMarks(dataToImport.Select(i => i.Insert.DataRecord.Phone).Distinct(), (i) => prgUpdateMarks.Value = i, s => logSession.Add($"update marks: {s}"));

                    var exportStart = DateTime.UtcNow - (Account.Settings.IgnoreExportTime ?? new TimeSpan());

                    logSession.Add($"prepare results to return records older then '{exportStart}'");
                    var rowsToExport = dataToImport
                        .Where(r => (r.Insert.DataRecord.Exported ?? DateTime.MinValue) <= exportStart)
                        .Select(r => r.Insert.DataRecord);

                    prgPrepareExport.Value = 100;

                    return rowsToExport;
                }
                catch (Exception ex)
                {
                    logSession.Add(ex);
                    logSession.Enabled = true;
                    throw;
                }
        }

        /// <summary>
        /// Обновление меток в данных
        /// </summary>
        /// <param name="phonesToUpdate">Телефоны, которые нужно проверить для обновления меток</param>
        /// <param name="progressAction">Действие для отображения прогресса</param>
        /// <param name="logAction">Действие для отображения лога</param>
        private void UpdateMarks(IEnumerable<Phone> phonesToUpdate, Action<decimal> progressAction = null, Action<string> logAction = null)
        {
            progressAction = progressAction ?? new Action<decimal>((i) => { });
            logAction = logAction ?? new Action<string>((i) => { });

            using (var logSession = Log.Session($"{this.GetType().Name}.{nameof(UpdateMarks)}()", VerboseLog))
                try
                {
                    logSession.Output = (strs) => strs.ToList().ForEach(s => logAction(s));

                    if (phonesToUpdate == null)
                        throw new ArgumentNullException(nameof(phonesToUpdate));

                    var progress = new Helpers.PercentageProgress();
                    var quickMarksProgress = progress.GetChild();
                    var similarityPhonesProgress = progress.GetChild();
                    progress.Change += (s, e) => progressAction(e.Value);

                    var phones = phonesToUpdate.Select(p => new { Phone = p, Progress = similarityPhonesProgress.GetChild() }).ToList();

                    logSession.Add($"update marks for ({phones.Count}) phones start...");

                    var now = DateTime.UtcNow;
                    var startFindDate = now - (Account.Settings.TimeForTrust ?? new TimeSpan());
                    var numberSeries = Account.SeriesOfNumbers.Where(s => s.DigitCount > 0).ToList();

                    #region Get default marks

                    var phoneMarksDictionary = phonesToUpdate
                        .Join(Account.Data, p => p, d => d.Phone, (p, d) => d)
                        .GroupBy(i => i.Phone)
                        .Select(g => new { Phone = g.Key, CountWithoutConstraint = g.Count(), CountWithConstraint = g.Count(d => d.Created <= startFindDate) })
                        .Select(i =>
                        {
                            var markType = MarkTypes.Default;
                            if (i.CountWithoutConstraint >= 3)
                            {
                                markType = MarkTypes.NotTrusted;
                            }
                            else
                            {
                                if (i.CountWithConstraint > 0)
                                {
                                    if (i.CountWithoutConstraint == 1)
                                    {
                                        markType = MarkTypes.Trusted;
                                    }
                                    else
                                    {
                                        if (i.CountWithoutConstraint == 2)
                                            markType = MarkTypes.HalfTrusted;
                                    }
                                }
                                else
                                {
                                    if (i.CountWithoutConstraint == 2)
                                        markType = MarkTypes.Suspicious;
                                }
                            }
                            return new { i.Phone, markType };
                        })
                        .ToDictionary(i => i.Phone, i => i.markType);

                    quickMarksProgress.Value = 100;

                    #endregion

#if DEBUG
                    phones.ForEach(phoneItem =>
#else
                    Parallel.ForEach(phones, phoneItem =>
#endif
                    {
                        var phone = phoneItem.Phone;
                        var currentMarkType = phoneMarksDictionary[phone];
                        logSession.Add($"mark selected for phone '{phone}' is '{currentMarkType}'");

                        #region Serial numbers

                        if (currentMarkType == MarkTypes.Default)
                        {
                            var prgPrepare = phoneItem.Progress.GetChild();
                            var validNumberSeries = numberSeries
                                .Where(ns => phone.PhoneNumber.Length >= ns.DigitCount)
                                .Select(ns => new { NumberSeries = ns, Progress = phoneItem.Progress.GetChild() })
                                .ToList();
                            prgPrepare.Value = 100;
                            validNumberSeries.ForEach(ns =>
                            {
                                logSession.Add($"similarity action start for phone '{phone}'...");
                                var startFindSimilarityDate = now - ns.NumberSeries.Delay;

                                string phoneSimilarity = phone.PhoneNumber.Substring(0, phone.PhoneNumber.Length - (int)ns.NumberSeries.DigitCount);
                                var similaryDataQuery = Account.Data
                                    .Where(r => r.Phone.PhoneNumber.StartsWith(phoneSimilarity));
                                if (ns.NumberSeries.Delay.Ticks > 0)
                                    similaryDataQuery = similaryDataQuery.Where(r => r.Created >= startFindSimilarityDate);

                                var similarytyPhones = similaryDataQuery.Select(sd => sd.Phone).Distinct().Except(new Phone[] { phone }).ToList();
                                if (similarytyPhones.Count > 0)
                                {
                                    currentMarkType = MarkTypes.Suspicious;
                                    lock (phoneMarksDictionary)
                                    {
                                        phoneMarksDictionary[phone] = currentMarkType;
                                        similarytyPhones.ForEach(sp =>
                                        {
                                            if (phoneMarksDictionary.ContainsKey(sp))
                                                phoneMarksDictionary[sp] = currentMarkType;
                                            else
                                                phoneMarksDictionary.Add(sp, currentMarkType);
                                        });
                                    }
                                }

                                ns.Progress.Value = 100;
                            });
                        }
                        else
                        {
                            phoneItem.Progress.Value = 100;
                        }
                        #endregion

                        logSession.Add($"mark detection done for phone '{phone}'. Finally mark is '{currentMarkType}'.");
                    });

                    var allMarks = Repository.MarkGet().ToArray();

                    var phoneMarksToUpdate = Account.PhoneMarks
                        .Join(phoneMarksDictionary, pm => pm.Phone, md => md.Key, (pm, md) => new { Item = pm, NewMarkType = md.Value })
                        .Join(allMarks, i => i.NewMarkType, m => m.Type, (i, m) => new { i.Item, NewMark = m })
                        .ToList();

                    phoneMarksToUpdate.ForEach(i => i.Item.Mark = i.NewMark);
                }
                catch (Exception ex)
                {
                    logSession.Add(ex);
                    logSession.Enabled = true;
                    throw;
                }
        }

        #region Static create default filters and validators

                private static Action<DataTable> GetDefaultDataTableValidator(IEnumerable<string> columnNames, Account account)
                {
                    return (account == null)
                        ? new Action<DataTable>((t) => { })
                        : new Action<DataTable>((t) =>
                        {
                            var columns = t.Columns.OfType<DataColumn>().Select(c => c.ColumnName);

                            var notExistsedColumns = string.Empty;
                            foreach (var cn in columnNames.Where(i => !columns.Contains(i.ToString())))
                                notExistsedColumns += (string.IsNullOrWhiteSpace(notExistsedColumns)
                                    ? string.Empty
                                    : ", ") + string.Format("'{0}'", cn);

                            if (!string.IsNullOrWhiteSpace(notExistsedColumns))
                                throw new Exception(string.Format(Resources.COLUMNS_NOT_FOUND_IN_IMPORT_FILE, notExistsedColumns));
                        });
                }
                private static Expression<Func<DataRow, bool>> GetDefaultRowFilter(IEnumerable<string> columnNames, Account account)
                {
                    return r => columnNames.Select(cN => r[cN] is DBNull ? (string)null : r[cN].ToString()).Any(c => string.IsNullOrWhiteSpace(c));
                }

        #endregion
        #region IDisposable

                // Flag: Has Dispose already been called?
                private bool disposed = false;

                protected virtual void Dispose(bool disposing)
                {
                    if (disposed)
                        return;

                    if (disposing)
                    {
                        Account = null;
                        Repository = null;
                    }
                    disposed = true;
                }

                public void Dispose()
                {
                    Dispose(true);
                    GC.SuppressFinalize(this);
                }

                // Use C# destructor syntax for finalization code.
                ~DataCalculator()
                {
                    // Simply call Dispose(false).
                    Dispose(false);
                }
        #endregion
    }
}
