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
        public string DirectoryPath { get; set; }

        /// <summary>
        /// Имя выгружаемого файла
        /// </summary>
        [Column("file_name")]
        public string FileName { get; set; }

        #region Mark
        /// <summary>
        /// Идентификатор типа записей для выгрузки
        /// </summary>
        [Column("mark_id"), ForeignKey("Mark")]
        public long? MarkID { get; set; }
        /// <summary>
        /// Тип записей для выгрузки
        /// </summary>
        public virtual Mark Mark { get; set; }
        #endregion

        /// <summary>
        /// Название кодировки для файлов в этой директории
        /// </summary>
        [Column("encoding"), MaxLength(100)]
        [Obsolete("User Encoding property instead")]
        public string EncodingName { get; set; }

        /// <summary>
        /// Кодировки для файлов в этой директории
        /// </summary>
        [NotMapped]
        public Encoding Encoding
        {
#pragma warning disable 618
            get { return Extensions.Extensions.GetEncodingByName(EncodingName); }
            set { EncodingName = value?.WebName; }
#pragma warning restore 618
        }

        /// <summary>
        /// Запускать комманду после анализа данных
        /// </summary>
        [Column("execute_after_analize_command")]
        public string ExecuteAfterAnalizeCommand { get; set; }

        /// <summary>
        /// Время ожидания выполнения команды
        /// </summary>
        [Obsolete("Property TimeoutForExecute should be used instead.")]
        [Column("timeout_for_execute"), Required]
        public decimal TimeoutForExecuteTicks { get; set; }

        /// <summary>
        /// Время ожидания выполнения команды
        /// </summary>
        [NotMapped]
        public TimeSpan TimeoutForExecute
        {
#pragma warning disable 618
            get { return TimeSpan.FromMilliseconds((double)TimeoutForExecuteTicks); }
            set { TimeoutForExecuteTicks = (decimal)value.TotalMilliseconds; }
#pragma warning restore 618
        }
    }
}
