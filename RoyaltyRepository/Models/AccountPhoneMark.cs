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
        /// Хранилище меток для телефонов в аккаунте
        /// </summary>
        public DbSet<AccountPhoneMark> AccountPhoneMarks { get; set; }
    }

    /// <summary>
    /// Метка для телефона в аккаунте
    /// </summary>
    [Table("account_phone_mark")]
    public partial class AccountPhoneMark : IHistoryRecordSource
    {
        #region Account
        /// <summary>
        /// Идентификатор аккаунта, которому принадлежат данные
        /// </summary>
        [Key, ForeignKey("Account"), Column("account_uid", Order = 1)]
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
        [Key, ForeignKey("Phone"), Column("phone_id", Order = 2)]
        public long PhoneID { get; set; }
        /// <summary>
        /// Телефон
        /// </summary>
        public virtual Phone Phone { get; set; }
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
