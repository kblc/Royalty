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
        /// <summary>
        /// Серии номеров
        /// </summary>
        public DbSet<AccountSeriesOfNumbersRecord> AccountSeriesOfNumbersRecords { get; set; }
    }

    [Table("series_of_numbers")]
    public partial class AccountSeriesOfNumbersRecord
    {
        /// <summary>
        /// Идентификатор записи
        /// </summary>
        [Key, Column("series_of_numbers_id"), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long AccountNumberSeriaRecordID { get; set; }
        
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
        /// <summary>
        /// Количество цифр
        /// </summary>
        [Column("digit_count"), Required]
        public long DigitCount { get; set; }

        /// <summary>
        /// Количество тиков (Используйте)
        /// </summary>
        [Obsolete("Property Delay should be used instead.")]
        [Column("delay_ticks"), Required]
        public long DelayTicks { get; set; }

        /// <summary>
        /// Количество времени ожидания
        /// </summary>
        [NotMapped]
        public TimeSpan Delay
        {
#pragma warning disable 618
            get { return TimeSpan.FromTicks(DelayTicks); }
            set { DelayTicks = value.Ticks; }
#pragma warning restore 618
        }
    }
}
