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
        /// Время запуска
        /// </summary>
        [Column("time"), Required]
        public TimeSpan Time { get; set; }
    }
}
