using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RoyaltyRepository.Extensions;

namespace RoyaltyRepository.Models
{
    public partial class RepositoryContext
    {
        public DbSet<Account> Accounts { get; set; }
    }

    [Table("account")]
    public partial class Account : IDefaultRepositoryInitialization, IHistoryRecordSource
    {
        internal const string defaultAccountName = ".default";

        public Account()
        {
            Data = new List<AccountDataRecord>();
            PhoneMarks = new List<AccountPhoneMark>();
            AdditionalColumns = new List<AccountDataRecordAdditionalColumn>();
            SeriesOfNumbers = new List<AccountSeriesOfNumbersRecord>();
            ExportTypes = new List<AccountExportType>();
            ImportQueue = new List<ImportQueueRecord>();
        }

        [Key, Column("account_uid")]
        public Guid AccountUID { get; set; }

        [Column("name"), Index("UIX_ACCOUNT_NAME", IsUnique = true)]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "AccountNameRequred"), MaxLength(250, ErrorMessageResourceName = "AccountNameMaxLength")]
        public string Name { get; set; }

        [Column("is_hidden"), Required]
        public bool IsHidden { get; set; }

        public virtual AccountSettings Settings { get; set; }
        public virtual AccountState State { get; set; }
        public virtual AccountDictionary Dictionary { get; set; }

        public virtual ICollection<AccountDataRecord> Data { get; set; }
        public virtual ICollection<AccountDataRecordAdditionalColumn> AdditionalColumns { get; set; }
        public virtual ICollection<AccountSeriesOfNumbersRecord> SeriesOfNumbers { get; set; }
        public virtual ICollection<AccountExportType> ExportTypes { get; set; }
        public virtual ICollection<ImportQueueRecord> ImportQueue { get; set; }
        public virtual ICollection<AccountPhoneMark> PhoneMarks { get; set; }

        #region IHistoryRecord

        object IHistoryRecordSource.SourceId { get { return this.AccountUID; } }

        HistorySourceType IHistoryRecordSource.SourceType { get { return HistorySourceType.Account; } }

        #endregion
        #region IDefaultRepositoryInitialization

        void IDefaultRepositoryInitialization.InitializeDefault(RepositoryContext context)
        {
            var defAccount = context.Accounts.SingleOrDefault(a => string.Compare(a.Name, defaultAccountName) == 0 && a.IsHidden);
            if (defAccount == null)
            {
                defAccount = GenerateDefaultAccount(context);
                context.Accounts.Add(defAccount);
                context.SaveChanges();
            }
        }
        
        private static Account GenerateDefaultAccount(RepositoryContext context)
        {
            var cts = context.ColumnTypes;

            Account acc = new Account()
            {
                AccountUID = Guid.NewGuid(),
                Name = Account.defaultAccountName,
                IsHidden = true,
                Settings = new AccountSettings()
                {
                    ExecuteAfterAnalizeCommand = string.Empty,
                    FolderExportAnalize = string.Empty,
                    FolderExportPhones = string.Empty,
                    FolderImportAnalize = string.Empty,
                    FolderImportMain = string.Empty,
                    IgnoreExportTime = TimeSpan.FromHours(1),
                    DeleteFileAfterImport = false,
                    RecursiveFolderSearch = true,
                    TimeForTrust = TimeSpan.FromDays(30),
                    WaitExecutionAfterAnalize = true,
                    Columns = new AccountSettingsColumn[]
                    {
                        new AccountSettingsColumn() { ColumnName = "Адрес объекта", ColumnType = cts.FirstOrDefault(c => c.SystemName == ColumnTypes.Address.ToString().ToUpper()) },
                        new AccountSettingsColumn() { ColumnName = "Город", ColumnType = cts.FirstOrDefault(c => c.SystemName == ColumnTypes.City.ToString().ToUpper()) },
                        new AccountSettingsColumn() { ColumnName = "Район", ColumnType = cts.FirstOrDefault(c => c.SystemName == ColumnTypes.Area.ToString().ToUpper()) },
                        new AccountSettingsColumn() { ColumnName = "Метка", ColumnType = cts.FirstOrDefault(c => c.SystemName == ColumnTypes.Mark.ToString().ToUpper()) },
                        new AccountSettingsColumn() { ColumnName = "URL", ColumnType = cts.FirstOrDefault(c => c.SystemName == ColumnTypes.Host.ToString().ToUpper()) },
                        new AccountSettingsColumn() { ColumnName = "Контакты для связи", ColumnType = cts.FirstOrDefault(c => c.SystemName == ColumnTypes.Phone.ToString().ToUpper()) },
                    }
                },
                State = new AccountState()
                {
                    IsActive = false
                },
                Dictionary = new AccountDictionary()
                {
                    AllowAddToDictionaryAutomatically = true,
                    AllowCalcAreasIfStreetExistsOnly = false,
                    SimilarityForTrust = 0.6m,
                    ConditionsScoreForTrust = 0.9m
                }
            };

            foreach (var i in new string[] { 
                        "ул",
                        "ул.",
                        "улица",
                        "пр",
                        "пр.",
                        "про.",
                        "прос",
                        "проспект",
                        "пл",
                        "пл.",
                        "площадь",
                        "ст.",
                        "стр.",
                        "ст",
                        "стр",
                        "строение",
                        "-ое",
                        "-ая",
                        "-е",
                        "-ья",
                        "-я",
                        "-ый",
                        "-ой",
                        "-ий",
                        "-е",
                        }
                .Select(s => new AccountDictionaryExclude() { Exclude = s }))
                acc.Dictionary.Excludes.Add(i);

            return acc;
        }

        #endregion
        #region ToString()

        public override string ToString()
        {
            return this.GetColumnPropertiesForEntity();
        }

        #endregion
    }
}
