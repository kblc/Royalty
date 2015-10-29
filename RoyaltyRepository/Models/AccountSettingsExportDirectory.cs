using RoyaltyRepository.Extensions;
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
        public DbSet<AccountSettingsExportDirectory> AccountSettingsExportDirectories { get; set; }
    }

    [Table("export_directory")]
    public partial class AccountSettingsExportDirectory : HistoryEntityBase
    {
        /// <summary>
        /// Идентификатор записи
        /// </summary>
        [Key, Column("export_directory_id"), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long AccountSettingsExportDirectoryID { get; set; }

        #region AccountSettings
        /// <summary>
        /// Идентификатор аккаунта, которому принадлежат данные настройки
        /// </summary>
        [Column("account_uid"), ForeignKey("AccountSettings"), Required]
        public Guid AccountUID { get; set; }
        /// <summary>
        /// Аккаунт, которому принадлежат данные настройки
        /// </summary>
        public virtual AccountSettings AccountSettings { get; set; }
        #endregion

        /// <summary>
        /// Путь до диреткории
        /// </summary>
        [Column("path"), Required]
        public string Path { get; set; }

        /// <summary>
        /// Экспортировать данные
        /// </summary>
        [Column("export_data"), Required]
        public bool ExportData { get; set; }

        /// <summary>
        /// Экспортировать телефоны
        /// </summary>
        [Column("export_phones"), Required]
        public bool ExportPhones { get; set; }

        /// <summary>
        /// Название кодировки для файлов в этой директории
        /// </summary>
        [Column("encoding")]
        [Obsolete("User Encoding property instead")]
        public string EncodingName { get; set; }

        /// <summary>
        /// Кодировки для файлов в этой директории
        /// </summary>
        [NotMapped]
        public Encoding Encoding
        {
#pragma warning disable 618
            get { return string.IsNullOrWhiteSpace(EncodingName) ? Encoding.Default : Encoding.GetEncoding(EncodingName); }
            set { EncodingName = value?.EncodingName; }
#pragma warning restore 618
        }

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

        #region Abstract implementation

        protected override object GetSourceId() => ((IHistoryRecordSource)AccountSettings).SourceId;
        protected override HistorySourceType GetSourceType() => HistorySourceType.AccountSettings;

        #endregion
    }
}
