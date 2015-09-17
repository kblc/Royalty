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

namespace RoyaltyDataCalculator
{
    /// <summary>
    /// Задача калькулятора - взять данные (импортируемые и существующие), обработать их и объединить
    /// </summary>
    public class DataCalculator : IDisposable
    {
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

                if (account != null)
                {
                    var cNames = value.Settings.GetType()
                        .GetProperties()
                        .Where(pi => pi.Name.EndsWith("ColumnName"));
                    columnNamesForTableValidation = cNames
                        .Where(pi => pi.GetCustomAttributes(typeof(IsRequiredForColumnImportAttribute), false).Length > 0)
                        .Select(pi => pi.GetValue(value.Settings, null))
                        .Where(pi => pi != null)
                        .Select(pi => pi.ToString().ToLower());

                    columnNamesForRowFilter = cNames
                        .Where(pi => pi.GetCustomAttributes(typeof(IsRequiredForColumnImportAttribute), false).Length > 0)
                        .Select(pi => pi.GetValue(value.Settings, null))
                        .Where(pi => pi != null)
                        .Select(pi => pi.ToString().ToLower());
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

        private IEnumerable<string> columnNamesForTableValidation = Enumerable.Empty<string>();
        private IEnumerable<string> columnNamesForRowFilter = Enumerable.Empty<string>();
        public Action<DataTable> TableValidator { get; set; }
        public Func<DataRow, bool> RowFilter { get; set; }

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
            return r => columnNames.Select(cN => (string)r[cN]).Any(c => string.IsNullOrWhiteSpace(c));
        }
        public DataCalculator(Account account = null, Repository repository = null)
        {
            Account = account;
            Repository = repository;
        }



        public IDictionary<DataRow, DataPreviewRow> Preview(DataTable dataTable, Repository repository)
        {


            throw new NotImplementedException();
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
