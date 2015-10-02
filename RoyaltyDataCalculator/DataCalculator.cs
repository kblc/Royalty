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
        /// Action для отображения текущего прогресса
        /// </summary>
        public Action<decimal> Progress { get; set; }

        private Action<string> output = (str) => { };
        /// <summary>
        /// Action для вывода лога
        /// </summary>
        public Action<string> Output { get { return output; } set { if (value == null) throw new ArgumentNullException(nameof(Output)); output = value; } }
        /// <summary>
        /// Использовать ли словарь при обработке данных (возможно добавление данных в сам словарь)
        /// </summary>
        public bool UseDictionary { get; set; } = true;

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
        /// <returns>Словарь соответсвия найденных данных для каждой строки таблицы</returns>
        public IDictionary<DataRow, DataPreviewRow> Preview(DataTable dataTable)
        {
            using (var logSession = Log.Session($"{this.GetType().Name}.{nameof(Preview)}()", false))
                try
                {
                    logSession.Output = (strs) => strs.ToList().ForEach(s => Output(s));

                    if (dataTable == null)
                        throw new ArgumentNullException("dataTable");
                    if (Account == null)
                        throw new ArgumentNullException("Account");
                    if (Repository == null)
                        throw new ArgumentNullException("Repository");

                    var pp = new Helpers.PercentageProgress();
                    var ppPrepare = pp.GetChild(weight: 0.1m);
                    var ppHostes = pp.GetChild(weight: 0.1m);
                    var ppPhones = pp.GetChild(weight: 0.1m);
                    var ppMarks = pp.GetChild(weight: 0.1m);
                    var ppCities = pp.GetChild(weight: 0.1m);
                    var ppStreets = pp.GetChild(weight: 0.7m);
                    var ppRows = pp.GetChild(weight: 0.3m);
                    pp.Change += (s, e) => { if (Progress != null) Progress(e.Value); };

                    #region Get column names by column types
                    Log.Add("Get column names by column types");

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
                    Log.Add("Parse incoming data");

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
                    Log.Add("Join hostes or create new");

                    var hosts = subRes0
                        .AsParallel()
                        .Select(i => i.IncomingHost.Hostname)
                        .Distinct()
                        .LeftOuterJoin(Repository.HostGet(), h => h, h => h.Name, (h, host) => new { HostName = h, Host = host })
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
                    Log.Add("Join phones or create new");

                    var phones = subRes0
                        .AsParallel()
                        .Select(i => i.IncomingPhone.PhoneNumber)
                        .Distinct()
                        .LeftOuterJoin(Repository.PhoneGet(), p => p, p => p.PhoneNumber, (p, phone) => new { PhoneNumber = p, Phone = phone })
                        .ToArray()
                        .Select(i => i.Phone ?? Repository.PhoneNew(i.PhoneNumber))
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
                    #region Join marks or create new
                    Log.Add("Join marks");

                    var defMark = Repository.MarkGet(MarkTypes.Unknown);

                    var subRes3 = subRes2
                        .LeftOuterJoin(Repository.MarkGet(), i => i.IncomingMark, m => m.SystemName, (i, m) => new
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

                    ppMarks.Value = 100;

                    #endregion
                    #region Join cities or create new
                    Log.Add("Join cities or create new");

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
                    #region Load rows
                    var ppRowsPrepare = ppRows.GetChild();
                    var ppRowsLoad = ppRows.GetChild();

                    var rowCount0 = (decimal)subRes5.Count();
                    var currentIndex0 = 0m;

                    var res = subRes5.Select(r => new
                    {
                        r.Row,
                        DataRecord = Repository.AccountDataRecordNew(Account, r.LoadedRow),
                        Index = (ppRowsPrepare.Value = (currentIndex0++) /rowCount0)
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

                    var rowCount1 = (decimal)dataTable.Rows.Count;
                    var currentIndex1 = 0m;
                    var columns = Account.AdditionalColumns.Where(c => !string.IsNullOrWhiteSpace(c.ColumnName));

                    res.ToList()
                        .ForEach(r =>
                        {
                            foreach(var c in columns)
                            {
                                var pi = r.LoadedRow.DataRecordAdditional.GetType().GetProperty(c.ColumnSystemName);
                                pi.SetValue(r.LoadedRow.DataRecordAdditional, r.Row[c.ColumnName]);
                            }
                            ppRowsLoad.Value = currentIndex1++ / rowCount1;
                        });
                    #endregion
                    return res.ToDictionary(i => i.Row, i => i.LoadedRow);
                }
                catch(Exception ex)
                {
                    logSession.Add(ex);
                    logSession.Enabled = true;
                    throw;
                }
        }

        public void Prepare(DataTable dataTable)
        {
            using (var logSession = Log.Session($"{GetType().Name}.{nameof(Prepare)}()", false))
                try
                {
                    if (dataTable == null)
                        throw new ArgumentNullException(nameof(dataTable));

                    logSession.Output = (strs) => strs.ToList().ForEach(s => Output(s));

                    logSession.Add($"Get ignore columns");
                    var columnNamesToIgnore = Account.Settings.Columns.Select(c => c.ColumnName.ToUpper());

                    logSession.Add($"Get columns to add from data table");
                    var dataColumnNamesToAdd = dataTable.Columns.OfType<DataColumn>().Select(dc => dc.ColumnName).Where(dc => !columnNamesToIgnore.Contains(dc.ToUpper()));

                    logSession.Add($"Join additional columns with columns to add");
                    var namesToAdd = dataColumnNamesToAdd
                        .LeftOuterJoin(Account.AdditionalColumns, n => n.ToUpper(), c => c.ColumnName.ToUpper(), (n, c) => new { ColumnName = n, AdditionalColumn = c })
                        .Where(c => c.AdditionalColumn == null)
                        .Select(c => c.ColumnName).ToArray()
                        .ToArray();

                    logSession.Add($"Get existed additional columns");
                    var existingColumnSystemNames = Account.AdditionalColumns.Select(ac => ac.ColumnSystemName).ToArray();

                    logSession.Add($"Check free additional columns");
                    var freeColumnNames = Enumerable.Range(0, (int)AccountDataRecordAdditional.ColumnCount)
                        .Select(i => new { Index = i, ColumnName = $"Column{i.ToString("00")}" })
                        .LeftOuterJoin(existingColumnSystemNames, n => n.ColumnName.ToUpper(), n => n?.ToUpper(), (n0, n1) => new { n0.Index, n0.ColumnName, Exists = n1 != null })
                        .Where(i => !i.Exists)
                        .OrderBy(i => i.Index)
                        .Select(i => i.ColumnName)
                        .ToArray();

                    logSession.Add($"Free columns count: {freeColumnNames.Length} and need to add folowing ({namesToAdd.Length}) columns: {namesToAdd.Select(c => $"'{c}'").Concat(i => i, ", ")}");
                    for (int i = 0; i < Math.Min(namesToAdd.Length, freeColumnNames.Length); i++)
                    {
                        var adrac = Repository.AccountDataRecordAdditionalColumnNew(Account, namesToAdd[i], freeColumnNames[i]);
                        logSession.Add($"Additional column added: '{adrac}'");
                    }
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
