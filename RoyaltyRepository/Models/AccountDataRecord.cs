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
    public partial class AccountDataRecord: IHistoryRecordSource
    {
        /// <summary>
        /// Идентификатор записи
        /// </summary>
        [Key, Column("data_uid")]
        public Guid AccountDataRecordID { get; set; }
        
        #region Account
        /// <summary>
        /// Идентификатор аккаунта, которому принадлежат данные
        /// </summary>
        [ForeignKey("Account"), Column("account_uid"), Index("IX_DATA_ACCOUNT_UID", IsUnique = false), Required]
        public Guid AccountID { get; set; }
        /// <summary>
        /// Аккаунт, которому принадлежат данные
        /// </summary>
        public virtual Account Account { get; set; }
        #endregion
        #region Phone
        /// <summary>
        /// Идентификатор телефона
        /// </summary>
        [ForeignKey("Phone"), Column("phone_id"), Required]
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
        [ForeignKey("Street"), Column("street_id"), Required]
        public long StreetID { get; set; }
        /// <summary>
        /// Улица
        /// </summary>
        public virtual Street Street { get; set; }
        #endregion        
        #region House number
        [Column("house_number"), MaxLength(20, ErrorMessageResourceName = "HouseNumberMaxLength")]
        public string HouseNumber { get; set; }
        #endregion

        [Column("created"), Required]
        public DateTime Created { get; set; }

        [Column("changed"), Required]
        public DateTime Changed { get; set; }

        [Column("exported")]
        public DateTime? Exported { get; set; }

        public virtual AccountDataRecordAdditional DataAdditional { get; set; }

        #region IHistoryRecordSource

        object IHistoryRecordSource.SourceId { get { return ((IHistoryRecordSource)Account).SourceId; } }

        HistorySourceType IHistoryRecordSource.SourceType { get { return HistorySourceType.AccountData; } }

        #endregion
        #region ToString()

        public override string ToString()
        {
            return this.GetColumnPropertiesForEntity();
        }

        #endregion
    }
}
