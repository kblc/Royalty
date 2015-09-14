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
        public DbSet<ImportQueueRecordFile> ImportQueueRecordFiles { get; set; }
    }

    [Table("import_queue_file")]
    public partial class ImportQueueRecordFile
    {
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
        #region Message
        /// <summary>
        /// Идентификатор сообщения, содержащего комментарий к текущей очереди загрузки
        /// </summary>
        [ForeignKey("Message"), Column("message_uid")]
        public Guid? MessageID { get; set; }
        /// <summary>
        /// Cообщение, содержащее комментарий к текущей очереди загрузки
        /// </summary>
        public virtual Message Message { get; set; }
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
        #region ImportFile
        /// <summary>
        /// Идентификатор файла для импорта
        /// </summary>
        [ForeignKey("ImportFile"), Column("import_file_uid"), Required]
        public Guid ImportFileUID { get; set; }
        /// <summary>
        /// Файл для импорта
        /// </summary>
        public virtual File ImportFile { get; set; }
        #endregion
        #region LogFile
        /// <summary>
        /// Идентификатор log-файла
        /// </summary>
        [ForeignKey("LogFile"), Column("log_file_uid")]
        public Guid? LogFileUID { get; set; }
        /// <summary>
        /// log-файл
        /// </summary>
        public virtual File LogFile { get; set; }
        #endregion

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

        public override string ToString()
        {
            return this.GetColumnPropertiesForEntity();
        }
    }
}
