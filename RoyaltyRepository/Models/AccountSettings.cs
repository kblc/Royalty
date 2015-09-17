﻿using System;
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
    public partial class AccountSettings
    {
        public AccountSettings()
        {
            SheduleTimes = new List<AccountSettingsSheduleTime>();
            Columns = new HashSet<AccountSettingsColumn>();
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
        /// Директория для импорта файлов
        /// </summary>
        [Column("folder_import_main"), Required(AllowEmptyStrings = true)]
        public string FolderImportMain { get; set; }

        /// <summary>
        /// Директория для импорта файлов для анализа
        /// </summary>
        [Column("folder_import_analize"), Required(AllowEmptyStrings = true)]
        public string FolderImportAnalize { get; set; }

        /// <summary>
        /// Директория для экспорта анализированных данных
        /// </summary>
        [Column("folder_export_analize"), Required(AllowEmptyStrings = true)]
        public string FolderExportAnalize { get; set; }

        /// <summary>
        /// Директория для экспорта номеров телефонов
        /// </summary>
        [Column("folder_export_phones"), Required(AllowEmptyStrings = true)]
        public string FolderExportPhones { get; set; }

        /// <summary>
        /// Время, в которое должна запускаться обработка файлов
        /// </summary>
        public virtual ICollection<AccountSettingsSheduleTime> SheduleTimes { get; set; }

        /// <summary>
        /// Настройка колонок для импорта
        /// </summary>
        public virtual ICollection<AccountSettingsColumn> Columns { get; set; }

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

        /// <summary>
        /// Удалять файлы после импорта
        /// </summary>
        [Column("delete_file_after_import")]
        public bool? DeleteFileAfterImport { get; set; }

        /// <summary>
        /// Запускать комманду после анализа данных
        /// </summary>
        [Column("execute_after_analize_command")]
        public string ExecuteAfterAnalizeCommand { get; set; }

        /// <summary>
        /// Ожидать окончания выполнения запущенной команды
        /// </summary>
        [Column("wait_execution_after_analize")]
        public bool? WaitExecutionAfterAnalize { get; set; }

        /// <summary>
        /// Рекурсивный обход директории при поиске .csv-файлов
        /// </summary>
        [Column("recursive_folder_search_for_csv_files")]
        public bool? RecursiveFolderSearch { get; set; }

        public override string ToString()
        {
            return this.GetColumnPropertiesForEntity();
        }
    }
}
