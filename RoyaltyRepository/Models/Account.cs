using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoyaltyRepository.Models
{
    public partial class RepositoryContext
    {
        public DbSet<Account> Accounts { get; set; }
    }

    [Table("account")]
    public partial class Account : IDefaultRepositoryInitialization
    {
        public Account()
        {
            Data = new List<AccountDataRecord>();
            AdditionalColumns = new List<AccountDataRecordAdditionalColumn>();
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

        public virtual IEnumerable<AccountDataRecord> Data { get; set; }
        public virtual IEnumerable<AccountDataRecordAdditionalColumn> AdditionalColumns { get; set; }

        void IDefaultRepositoryInitialization.InitializeDefault(RepositoryContext context)
        {
            var defAccount = context.Accounts.SingleOrDefault(a => string.Compare(a.Name, ".default") == 0 && a.IsHidden);
            if (defAccount == null)
            {
                defAccount = GenerateDefaultAccount();
                context.Accounts.Add(defAccount);
                context.SaveChanges();
            }
        }
        private static Account GenerateDefaultAccount()
        {
            Account acc = new Account()
            {
                AccountUID = Guid.NewGuid(),
                Name = ".default",
                IsHidden = true,
                Settings = new AccountSettings()
                {
                    AddressColumnName = "Адрес объекта",
                    AreaColumnName = "Район",
                    MarkColumnName = "Метка",
                    HostColumnName = "URL",
                    PhoneColumnName = "Контакты для связи",
                    ExecuteAfterAnalizeCommand = string.Empty,
                    FolderExportAnalize = string.Empty,
                    FolderExportPhones = string.Empty,
                    FolderImportAnalize = string.Empty,
                    FolderImportMain = string.Empty,
                    IgnoreExportTime = TimeSpan.FromHours(1),
                    DeleteFileAfterImport = false,
                    RecursiveFolderSearch = true,
                    TimeForTrust = TimeSpan.FromDays(30),
                    WaitExecutionAfterAnalize = true
                },
                State = new AccountState()
                {
                    IsActive = false
                },
                Dictionary = new AccountDictionary()
                {
                    AllowAddToDictionaryAutomatically = true,
                    AllowCalcAreasIfStreetExistsOnly = false,
                    SimilarityForTrust = 0.6m
                }
            };

            foreach (var i in new string[] { 
                        ".",",","|","(",")",@"\","/","~","!","@","#","$","%","^","&","*","<",">","?",";","'","\"",":","[","]","{","}","+","_","`",
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
    }
}
