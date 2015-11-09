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
        /// <summary>
        /// Данные
        /// </summary>
        public DbSet<AccountDataRecord> AccountDataRecords { get; set; }
    }

    [Table("data")]
    public partial class AccountDataRecord: HistoryEntityBase<Guid, AccountDataRecord>
    {
        public AccountDataRecord()
        {
            LoadedByQueueFiles = new List<ImportQueueRecordFileAccountDataRecord>();
            ExportInfo = new HashSet<AccountDataRecordExport>();
        }

        /// <summary>
        /// Идентификатор записи
        /// </summary>
        [Key, Column("data_uid")]
        public Guid AccountDataRecordUID { get; set; }
        
        #region Account
        /// <summary>
        /// Идентификатор аккаунта, которому принадлежат данные
        /// </summary>
        [ForeignKey("Account"), Column("account_uid"), Index("IX_DATA_ACCOUNT_UID", IsUnique = false), Required]
        public Guid AccountUID { get; set; }
        /// <summary>
        /// Аккаунт, которому принадлежат данные
        /// </summary>
        public virtual Account Account { get; set; }
        #endregion
        #region Phone
        /// <summary>
        /// Идентификатор телефона
        /// </summary>
        [ForeignKey("Phone"), Column("phone_id"), Required, Index("UX_DATA_ACCOUNT_PHONE_STREET_HOUSE", IsUnique = true, Order = 1)]
        public long PhoneID { get; set; }
        /// <summary>
        /// Телефон
        /// </summary>
        public virtual Phone Phone { get; set; }
        #endregion
        #region Host
        /// <summary>
        /// Идентификатор хоста, откуда была зугружена запись
        /// </summary>
        [ForeignKey("Host"), Column("host_id"), Required]
        public long HostID { get; set; }
        /// <summary>
        /// Хост
        /// </summary>
        public virtual Host Host { get; set; }
        #endregion        
        #region Street
        /// <summary>
        /// Идентификатор улицы
        /// </summary>
        [ForeignKey("Street"), Column("street_id"), Index("UX_DATA_ACCOUNT_PHONE_STREET_HOUSE", IsUnique = true, Order = 2), Required]
        public long StreetID { get; set; }
        /// <summary>
        /// Улица
        /// </summary>
        public virtual Street Street { get; set; }
        #endregion        
        #region House number
        [Column("house_number"), MaxLength(20, ErrorMessageResourceName = "HouseNumberMaxLength"), Index("UX_DATA_ACCOUNT_PHONE_STREET_HOUSE", IsUnique = true, Order = 3)]
        public string HouseNumber { get; set; }
        #endregion

        [Column("created"), Required]
        public DateTime Created { get; set; }

        [Column("changed"), Required]
        public DateTime Changed { get; set; }

        [Column("exported")]
        public DateTime? Exported { get; set; }

        public virtual AccountDataRecordAdditional DataAdditional { get; set; }

        /// <summary>
        /// Файлы очереди, в которых загружался данный файл
        /// </summary>
        public virtual ICollection<ImportQueueRecordFileAccountDataRecord> LoadedByQueueFiles { get; set; }

        /// <summary>
        /// Информация об экспорте данной записи
        /// </summary>
        public virtual ICollection<AccountDataRecordExport> ExportInfo { get; set; }
    }
}
