using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RoyaltyRepository.Extensions;
using System.ComponentModel;

namespace RoyaltyRepository.Models
{
    public partial class RepositoryContext
    {
        /// <summary>
        /// Список файлов для записи очереди загрузки
        /// </summary>
        public DbSet<ImportQueueRecordFileInfoFile> ImportQueueRecordFileInfoFiles { get; set; }
    }

    public enum ImportQueueRecordFileInfoFileType
    {
        /// <summary>
        /// Log file
        /// </summary>
        [Description("IMPORTQUEUERECORDFILEINFOFILETYPE_Log")]
        Log = 0,
        /// <summary>
        /// Imported file
        /// </summary>
        [Description("IMPORTQUEUERECORDFILEINFOFILETYPE_Import")]
        Import,
        /// <summary>
        /// Exported file
        /// </summary>
        [Description("IMPORTQUEUERECORDFILEINFOFILETYPE_Export")]
        Export,
    }

    [Table("import_queue_file_file")]
    public class ImportQueueRecordFileInfoFile
    {
        [Key, Column("import_queue_file_file_id"), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ImportQueueRecordFileInfoFileID { get; set; }

        #region ImportQueueRecordFileInfo
        /// <summary>
        /// Идентификатор очереди загрузки, которой принадлежит данный файл
        /// </summary>
        [ForeignKey("ImportQueueRecordFileInfo"), Column("import_queue_file_uid"), Required]
        public Guid ImportQueueRecordFileInfoUID { get; set; }
        /// <summary>
        /// Очереди загрузки, которой принадлежит данный файл
        /// </summary>
        public virtual ImportQueueRecordFileInfo ImportQueueRecordFileInfo { get; set; }
        #endregion

        /// <summary>
        /// File type name
        /// </summary>
        [Column("type"), Required, MaxLength(100)]
        [Obsolete("Use Type property instead")]
        public string TypeSystemName { get; set; }

        #region File
        /// <summary>
        /// Ссылка на файл
        /// </summary>
        [ForeignKey("File"), Column("file_uid"), Required]
        public Guid FileUID { get; set; }
        /// <summary>
        /// Файл
        /// </summary>
        public virtual File File { get; set; }
        #endregion

        /// <summary>
        /// Название типа файла
        /// </summary>
        [NotMapped]
        public string TypeName { get { return Extensions.Extensions.GetEnumNameFromType(Type); } }

        /// <summary>
        /// Тип файла
        /// </summary>
        [NotMapped]
        public ImportQueueRecordFileInfoFileType Type
        {
#pragma warning disable 618
            get { return Extensions.Helpers.GetEnumValueByName<ImportQueueRecordFileInfoFileType>(TypeSystemName); }
            set { TypeSystemName = value.ToString().ToUpper(); }
#pragma warning restore 618
        }
    }
}
