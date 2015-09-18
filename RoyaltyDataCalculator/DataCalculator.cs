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

namespace RoyaltyDataCalculator
{
    /// <summary>
    /// Задача калькулятора - взять данные (импортируемые и существующие), обработать их и объединить
    /// </summary>
    public class DataCalculator : IDisposable
    {
        public DataCalculator(Account account = null, Repository repository = null)
        {
            Account = account;
            Repository = repository;
        }

        private Account account = null;
        public Account Account
        {
            get
            {
                return account;
            }
            set
            {
                if (account != value)
                    account = value;

                var columnNamesForTableValidation = Enumerable.Empty<string>();
                var columnNamesForRowFilter = Enumerable.Empty<string>();

                if (account != null)
                {
                    columnNamesForTableValidation = value.Settings.Columns
                        .Where(c => c.ColumnType.ImportTableValidation)
                        .Select(c => c.ColumnName.ToLower())
                        .ToArray();

                    columnNamesForRowFilter = value.Settings.Columns
                        .Where(c => c.ColumnType.ImportRowValidation)
                        .Select(c => c.ColumnName.ToLower())
                        .ToArray();
                } else
                {
                    columnNamesForTableValidation = Enumerable.Empty<string>();
                    columnNamesForRowFilter = Enumerable.Empty<string>();
                }

                TableValidator = GetDefaultDataTableValidator(columnNamesForTableValidation, value);
                RowFilter = GetDefaultRowFilter(columnNamesForTableValidation, value);
            }
        }
        public Repository Repository { get; set; }

        public Action<DataTable> TableValidator { get; set; }
        public Func<DataRow, bool> RowFilter { get; set; }
        
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
        private static Func<DataRow, bool> GetDefaultRowFilter(IEnumerable<string> columnNames, Account account)
        {
            return r => columnNames.Select(cN => r[cN] is DBNull ? (string)null : r[cN].ToString()).Any(c => string.IsNullOrWhiteSpace(c));
        }

        #endregion

        public IDictionary<DataRow, DataPreviewRow> Preview(DataTable dataTable)
        {
            var logSession = Log.SessionStart(this.GetType().Name + ".Preview()", true);
            var hasError = false;
            try
            {
                if (dataTable == null)
                    throw new ArgumentNullException("dataTable");
                if (Account == null)
                    throw new ArgumentNullException("Account");
                if (Repository == null)
                    throw new ArgumentNullException("Repository");

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
                        IncomingAddress = Parser.Address.FromString(dr.IncomingAddress, excludes),
                        IncomingHost = Parser.Host.FromString(dr.IncomingHost),
                        IncomingPhone = Parser.Phone.FromString(dr.IncomingPhone),
                        dr.IncomingCity,
                        dr.IncomingArea,
                        dr.IncomingMark,
                    });

                #endregion
                #region Join hostes or create new
                Log.Add("Join hostes or create new");

                var subRes1 = subRes0
                    .LeftOuterJoin(Repository.HostGet(), i => i.IncomingHost.Hostname, h => h.Name, (i, h) => new
                    {
                        i.Row,
                        i.IncomingAddress,
                        i.IncomingArea,
                        i.IncomingCity,
                        i.IncomingPhone,
                        i.IncomingHost,
                        i.IncomingMark,
                        Host = h,
                    })
                    .GroupBy(i => i.IncomingHost.Hostname)
                    .SelectMany(g => g.Select(i => new 
                    {
                        i.Row,
                        i.IncomingAddress,
                        i.IncomingArea,
                        i.IncomingCity,
                        i.IncomingPhone,
                        i.IncomingHost,
                        i.IncomingMark,
                        Host = g.FirstOrDefault().Host == null 
                            ? Repository.HostNew(g.FirstOrDefault().IncomingHost.Hostname) 
                            : i.Host,
                        IsNewHost = g.FirstOrDefault().Host == null,
                    }));
                #endregion
                #region Join phones or create new
                Log.Add("Join phones or create new");

                var subRes2 = subRes1
                    .LeftOuterJoin(Repository.PhoneGet(), i => i.IncomingPhone.PhoneNumber, p => p.PhoneNumber, (i, p) => new 
                    {
                        i.Row,
                        i.IncomingPhone,
                        i.IncomingAddress,
                        i.IncomingArea,
                        i.IncomingCity,
                        i.IncomingHost,
                        i.IncomingMark,
                        i.Host,
                        i.IsNewHost,
                        Phone = p,
                    })
                    .GroupBy(i => i.IncomingPhone.PhoneNumber)
                    .SelectMany(g => g.Select(i => new
                    {
                        i.Row,
                        i.IncomingAddress,
                        i.IncomingArea,
                        i.IncomingCity,
                        i.IncomingPhone,
                        i.IncomingHost,
                        i.IncomingMark,
                        i.Host,
                        i.IsNewHost,
                        Phone = g.FirstOrDefault().Phone == null
                            ? Repository.PhoneNew(g.FirstOrDefault().IncomingPhone.PhoneNumber)
                            : i.Phone,
                        IsNewPhone = g.FirstOrDefault().Phone == null,
                    }));
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
                        i.IncomingArea,
                        i.IncomingCity,
                        i.IncomingHost,
                        i.IncomingMark,
                        i.Host,
                        i.IsNewHost,
                        i.Phone,
                        i.IsNewPhone,
                        Mark = m ?? defMark,
                    });
                #endregion
                #region Join cities or create new
                Log.Add("Join cities or create new");

                var res = subRes3
                    .LeftOuterJoin(Repository.CityGet(), i => i.IncomingCity.ToUpper(), c => c.Name.ToUpper(), (i, c) => new
                    {
                        i.Row,
                        i.IncomingPhone,
                        i.IncomingAddress,
                        i.IncomingArea,
                        i.IncomingCity,
                        i.IncomingHost,
                        i.IncomingMark,
                        i.Host,
                        i.IsNewHost,
                        i.Phone,
                        i.IsNewPhone,
                        i.Mark,
                        City = c
                    })
                    .GroupBy(i => i.IncomingCity)
                    .SelectMany(g => g.Select(i => new
                    {
                        i.Row,
                        i.IncomingPhone,
                        i.IncomingAddress,
                        i.IncomingArea,
                        i.IncomingCity,
                        i.IncomingHost,
                        i.IncomingMark,
                        i.Host,
                        i.IsNewHost,
                        i.Phone,
                        i.IsNewPhone,
                        i.Mark,
                        City = g.FirstOrDefault().City == null
                            ? Repository.CityNew(g.FirstOrDefault().IncomingCity)
                            : i.City,
                        IsNewCity = g.FirstOrDefault().City == null,
                    }));
                #endregion
                #region Join areas

                    

                #endregion
                return res.ToDictionary(i => i.Row, i => new DataPreviewRow()
                    {
                        IncomingPhone = i.IncomingPhone,
                        IncomingAddress = i.IncomingAddress,
                        IncomingArea = i.IncomingArea,
                        IncomingCity = i.IncomingCity,
                        IncomingHost = i.IncomingHost,
                        IncomingMark = i.IncomingMark,
                        House = i.IncomingAddress.House.ToString(),
                        Host = i.Host,
                        IsNewHost = i.IsNewHost,
                        Phone = i.Phone,
                        IsNewPhone = i.IsNewPhone,
                        Mark = i.Mark,
                        City = i.City,
                        IsNewCity = i.IsNewCity,
                        Street = null, //?????
                    });
            }
            catch
            {
                hasError = true;
                throw;
            }
            finally
            {
                Log.SessionEnd(logSession, hasError);
            }
        }

        #region IDisposable
        public void Dispose()
        {
            Account = null;
            Repository = null;
        }
        #endregion
    }
}
