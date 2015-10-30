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
        /// Список файлов очереди загрузки
        /// </summary>
        public DbSet<ImportQueueRecordFileInfo> ImportQueueRecordFileInfoes { get; set; }
    }

    [Table("import_queue_file")]
    public partial class ImportQueueRecordFileInfo : HistoryEntityBase
    {
        public ImportQueueRecordFileInfo()
        {
            Files = new HashSet<ImportQueueRecordFileInfoFile>();
            LoadedRecords = new List<ImportQueueRecordFileAccountDataRecord>();
        }

        /// <summary>
        /// Идентификатор записи
        /// </summary>
        [Key, Column("import_queue_file_uid")]
        public Guid ImportQueueRecordFileUID { get; set; }

        #region ImportQueueRecord
        /// <summary>
        /// Идентификатор очереди загрузки, которой принадлежит данный файл
        /// </summary>
        [ForeignKey("ImportQueueRecord"), Column("import_queue_uid"), Required]
        public Guid ImportQueueRecordUID { get; set; }
        /// <summary>
        /// Очереди загрузки, которой принадлежит данный файл
        /// </summary>
        public virtual ImportQueueRecord ImportQueueRecord { get; set; }
        #endregion
        #region State
        /// <summary>
        /// Идентификатор состояния загрузки файла
        /// </summary>
        [ForeignKey("ImportQueueRecordState"), Column("import_queue_record_state_id"), Required]
        public long ImportQueueRecordStateID { get; set; }
        /// <summary>
        /// Состояние загрузки файла
        /// </summary>
        public virtual ImportQueueRecordState ImportQueueRecordState { get; set; }
        #endregion

        /// <summary>
        /// Ошибка при загрузке файла
        /// </summary>
        [Column("error")]
        public string Error { get; set; }

        /// <summary>
        /// Загружаемый файл
        /// </summary>
        [Column("source_file_path"), Required]
        public string SourceFilePath { get; set; }

        /// <summary>
        /// Дата начала обработки
        /// </summary>
        [Column("started")]
        public DateTime? Started { get; set; }
        /// <summary>
        /// Дата окончания обработки
        /// </summary>
        [Column("finished")]
        public DateTime? Finished { get; set; }
        /// <summary>
        /// Файл для анализа?
        /// </summary>
        [Column("for_analize"), Required]
        public bool ForAnalize { get; set; }

        /// <summary>
        /// Записи, загруженные в этом файле
        /// </summary>
        public virtual ICollection<ImportQueueRecordFileAccountDataRecord> LoadedRecords { get; set; }

        /// <summary>
        /// Файлы для этой записи
        /// </summary>
        public virtual ICollection<ImportQueueRecordFileInfoFile> Files { get; set; }

        #region Abstract implementation

        protected override object GetSourceId() => ImportQueueRecordUID;
        protected override HistorySourceType GetSourceType() => HistorySourceType.Queue;

        #endregion
    }
}
