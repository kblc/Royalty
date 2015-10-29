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
        public DbSet<AccountSettings> AccountSettings { get; set; }
    }

    [Table("account_settings")]
    public partial class AccountSettings : HistoryEntityBase
    {
        public AccountSettings()
        {
            SheduleTimes = new List<AccountSettingsSheduleTime>();
            Columns = new HashSet<AccountSettingsColumn>();
            ImportFolders = new HashSet<AccountSettingsImportDirectory>();
            ExportFolders = new HashSet<AccountSettingsExportDirectory>();
        }
        #region Account
        /// <summary>
        /// Идентификатор аккаунта, которому принадлежат данные настройки
        /// </summary>
        [Key, ForeignKey("Account"), Column("account_uid")]
        public Guid AccountUID { get; set; }
        /// <summary>
        /// Аккаунт, которому принадлежат данные настройки
        /// </summary>
        public virtual Account Account { get; set; }
        #endregion

        /// <summary>
        /// Время, в которое должна запускаться обработка файлов
        /// </summary>
        public virtual ICollection<AccountSettingsSheduleTime> SheduleTimes { get; set; }

        /// <summary>
        /// Настройка колонок для импорта
        /// </summary>
        public virtual ICollection<AccountSettingsColumn> Columns { get; set; }

        /// <summary>
        /// Директории импорта
        /// </summary>
        public virtual ICollection<AccountSettingsImportDirectory> ImportFolders { get; set; }

        /// <summary>
        /// Директории экспорта
        /// </summary>
        public virtual ICollection<AccountSettingsExportDirectory> ExportFolders { get; set; }

        /// <summary>
        /// Игнорирование времени экспорта (используйте IgnoreExportTime)
        /// </summary>
        [Obsolete("Property IgnoreExportTime should be used instead.")]
        [Column("ignore_export_ticks")]
        public long? IgnoreExportTimeTicks { get; set; }

        /// <summary>
        /// Игнорирование времени экспорта
        /// </summary>
        [NotMapped]
        public TimeSpan? IgnoreExportTime
        {
            #pragma warning disable 618
            get { return IgnoreExportTimeTicks.HasValue ? TimeSpan.FromMilliseconds(IgnoreExportTimeTicks.Value) : (TimeSpan?)null ; }
            set { IgnoreExportTimeTicks = value == null ? (long?)null : (long?)value.Value.TotalMilliseconds; }
            #pragma warning restore 618
        }

        /// <summary>
        /// Сколько времени должно пройти для того, что бы данные стали доверенными (используйте TimeForTrust)
        /// </summary>
        [Obsolete("Property TimeForTrust should be used instead.")]
        [Column("time_for_trust_ticks")]
        public long? TimeForTrustTicks { get; set; }

        /// <summary>
        /// Сколько времени должно пройти для того, что бы данные стали доверенными
        /// </summary>
        [NotMapped]
        public TimeSpan? TimeForTrust
        {
            #pragma warning disable 618
            get { return TimeForTrustTicks.HasValue ? TimeSpan.FromMilliseconds(TimeForTrustTicks.Value) : (TimeSpan?)null; }
            set { TimeForTrustTicks = value == null ? (long?)null : (long)value.Value.TotalMilliseconds; }
            #pragma warning restore 618
        }

        #region Abstract implementation

        protected override object GetSourceId() => ((IHistoryRecordSource)Account).SourceId;
        protected override HistorySourceType GetSourceType() => HistorySourceType.AccountSettings;

        #endregion
    }
}
