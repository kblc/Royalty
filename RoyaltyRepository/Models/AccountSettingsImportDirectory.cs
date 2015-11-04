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
        public DbSet<AccountSettingsImportDirectory> AccountSettingsImportDirectories { get; set; }
    }

    [Table("import_directory")]
    public partial class AccountSettingsImportDirectory : HistoryEntityBase
    {
        /// <summary>
        /// Идентификатор записи
        /// </summary>
        [Key, Column("import_directory_id"), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long AccountSettingsImportDirectoryID { get; set; }

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
        /// Директория для анализа
        /// </summary>
        [Column("for_analize"), Required]
        public bool ForAnalize { get; set; }

        /// <summary>
        /// Рекурсивный обход директории при поиске файлов
        /// </summary>
        [Column("recursive_folder_search"), Required]
        public bool RecursiveFolderSearch { get; set; }

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
        /// Маска для файлов импорта
        /// <example>*.csv</example>
        /// </summary>
        [Column("filter")]
        public string Filter { get; set; }

        /// <summary>
        /// Удалять файлы после импорта
        /// </summary>
        [Column("delete_file_after_import"), Required]
        public bool DeleteFileAfterImport { get; set; }
    }
}
