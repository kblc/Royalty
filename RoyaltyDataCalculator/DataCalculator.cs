﻿using System;
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
            return r => columnNames.Select(cN => (string)r[cN]).Any(c => string.IsNullOrWhiteSpace(c));
        }

        #endregion

        public IDictionary<DataRow, DataPreviewRow> Preview(DataTable dataTable)
        {
            var colTypes = Repository.ColumnTypeGet().ToArray();

            if (dataTable == null)
                throw new ArgumentNullException("dataTable");

            var res = dataTable.Rows
                .OfType<DataRow>()
                .AsParallel()
                .Select(dr => new
                {
                    Row = dr,
                    AddressText = dr[Account.Settings.Columns.First(c => c.ColumnTypeID == colTypes.First(ct => ct.SystemName == ColumnTypes.Address.ToString().ToUpper()).ColumnTypeID ).ColumnName]
                })
                ;

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
