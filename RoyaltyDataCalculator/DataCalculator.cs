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
        public Expression<Func<DataRow, bool>> RowFilter { get; set; }
        
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

        public IDictionary<DataRow, DataPreviewRow> Preview(DataTable dataTable)
        {
            using (var logSession = Log.Session(this.GetType().Name + ".Preview()", false))
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
                            IncomingAddress = Parser.Address.FromString(dr.IncomingAddress, dr.IncomingArea, excludes),
                            IncomingHost = Parser.Host.FromString(dr.IncomingHost),
                            IncomingPhone = Parser.Phone.FromString(dr.IncomingPhone),
                            dr.IncomingCity,
                            dr.IncomingMark,
                        })
                        .ToArray();

                    #endregion
                    #region Join hostes or create new
                    Log.Add("Join hostes or create new");

                    var hosts = subRes0
                        .Select(i => i.IncomingHost.Hostname)
                        .Distinct()
                        .LeftOuterJoin(Repository.HostGet(), h => h, h => h.Name, (h, host) => new { HostName = h, Host = host })
                        .ToArray()
                        .Select(i => i.Host ?? Repository.HostNew(i.HostName))
                        .ToArray();

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
                            IsNewHost = h.HostID == 0
                        });
                    #endregion
                    #region Join phones or create new
                    Log.Add("Join phones or create new");

                    var phones = subRes0
                        .Select(i => i.IncomingPhone.PhoneNumber)
                        .Distinct()
                        .LeftOuterJoin(Repository.PhoneGet(), p => p, p => p.PhoneNumber, (p, phone) => new { PhoneNumber = p, Phone = phone })
                        .ToArray()
                        .Select(i => i.Phone ?? Repository.PhoneNew(i.PhoneNumber))
                        .ToArray();

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
                            i.IsNewHost,
                            Phone = p,
                            IsNewPhone = p.PhoneID == 0
                        });
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
                            i.IsNewHost,
                            i.Phone,
                            i.IsNewPhone,
                            Mark = m ?? defMark,
                        });
                    #endregion
                    #region Join cities or create new
                    Log.Add("Join cities or create new");

                    var cities = subRes0
                        .Select(i => i.IncomingCity)
                        .Distinct()
                        .LeftOuterJoin(Repository.CityGet(), c => c.ToUpper(), c => c.Name.ToUpper(), (c, city) => new { CityName = c, City = city })
                        .ToArray()
                        .Select(i => i.City ?? Repository.CityNew(i.CityName))
                        .ToArray();

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
                            i.IsNewHost,
                            i.Phone,
                            i.IsNewPhone,
                            i.Mark,
                            City = c,
                            IsNewCity = c.CityID == 0
                        });

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
                        .ToArray());

                    var res = subRes4
                        .LeftOuterJoin(aPR,
                            r => new { r.IncomingAddress.Street, House = r.IncomingAddress.House.ToString(), r.IncomingAddress.Area, r.City.Name },
                            i => new { i.IncomingAddress.Street, House = i.IncomingAddress.House.ToString(), i.IncomingAddress.Area, i.City.Name },
                            (r, i) => new
                            {
                                r.Row,
                                IncomingAddress = i.Address,
                                r.IncomingCity,
                                r.IncomingHost,
                                r.IncomingMark,
                                r.IncomingPhone,
                                r.IsNewCity,
                                r.IsNewHost,
                                r.IsNewPhone,
                                i.IsNewArea,
                                i.IsNewStreet,
                                r.Mark,
                                r.Phone,
                                r.City,
                                i.Area,
                                i.Street,
                                r.Host,
                            });

                    #endregion

                    return res.ToDictionary(i => i.Row, i => new DataPreviewRow()
                    {
                        IncomingPhone = i.IncomingPhone,
                        IncomingAddress = i.IncomingAddress,
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
                        Street = i.Street,
                        IsNewStreet = i.IsNewStreet,
                        IsNewArea = i.IsNewArea,
                    });
                }
                catch(Exception ex)
                {
                    logSession.Add(ex);
                    logSession.Enabled = true;
                    throw;
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
