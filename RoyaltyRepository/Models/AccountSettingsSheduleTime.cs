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
        public DbSet<AccountSettingsSheduleTime> AccountSettingsSheduleTimes { get; set; }
    }

    [Table("shedule_time")]
    public partial class AccountSettingsSheduleTime
    {
        /// <summary>
        /// Идентификатор таймера
        /// </summary>
        [Key, Column("shedule_time_id"), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ID { get; set; }
        /// <summary>
        /// Идентификатор аккаунта, которому принадлежат данные настройки
        /// </summary>
        [Column("account_uid"), ForeignKey("AccountSettings"), Index("IX_ACCOUNT_SETTINGS_SHEDULE_TIME_ACCOUNT_UID", IsUnique = false), Required]
        public Guid AccountUID { get; set; }
        /// <summary>
        /// Аккаунт, которому принадлежат данные настройки
        /// </summary>
        public virtual AccountSettings AccountSettings { get; set; }

        /// <summary>
        /// Время запуска (используйте Time)
        /// </summary>
        [Obsolete("Property Time should be used instead.")]
        [Column("time_ticks"), Required]
        public long TimeTicks { get; set; }

        /// <summary>
        /// Время запуска
        /// </summary>
        [NotMapped]
        public TimeSpan Time
        {
            #pragma warning disable 618
            get { return TimeSpan.FromMilliseconds(TimeTicks); }
            set { TimeTicks = (long)value.TotalMilliseconds; }
            #pragma warning restore 618
        }

        public override string ToString()
        {
            return string.Format("{0}:[id:{1},account_uid:'{2}',ticks:{3}]", this.GetType().Name, ID, this.AccountSettings == null ? "NULL" : this.AccountSettings.AccountUID.ToString(), Time.Ticks);
        }
    }
}
