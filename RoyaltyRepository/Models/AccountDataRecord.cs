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
    public partial class AccountDataRecord
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
        #region House
        /// <summary>
        /// Идентификатор дома
        /// </summary>
        [ForeignKey("House"), Column("house_id"), Required]
        public long HouseID { get; set; }
        /// <summary>
        /// Адрес
        /// </summary>
        public virtual House House { get; set; }
        #endregion        
        #region Mark
        /// <summary>
        /// Идентификатор метки
        /// </summary>
        [ForeignKey("Mark"), Column("mark_id"), Required]
        public long MarkID { get; set; }
        /// <summary>
        /// Метка
        /// </summary>
        public virtual Mark Mark { get; set; }
        #endregion

        [Column("created"), Required]
        public DateTime Created { get; set; }

        [Column("changed"), Required]
        public DateTime Changed { get; set; }

        [Column("exported")]
        public DateTime? Exported { get; set; }

        public virtual AccountDataRecordAdditional DataAdditional { get; set; }

        public override string ToString()
        {
            return this.GetColumnPropertiesForEntity();
        }
    }
}
