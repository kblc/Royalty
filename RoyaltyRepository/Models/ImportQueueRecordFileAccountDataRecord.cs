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
        /// Записи, загруженные в этой файле
        /// </summary>
        public DbSet<ImportQueueRecordFileAccountDataRecord> ImportQueueRecordFileAccountDataRecords { get; set; }
    }

    [Table("import_queue_file_data_records")]
    public partial class ImportQueueRecordFileAccountDataRecord
    {
        #region ImportQueueRecordFile
        /// <summary>
        /// Идентификатор файла очереди загрузки, которой принадлежит данная запись
        /// </summary>
        [Key, ForeignKey(nameof(ImportQueueRecordFile)), Column("import_queue_file_uid", Order = 1)]
        public Guid ImportQueueRecordFileUID { get; set; }
        /// <summary>
        /// Очереди загрузки, которой принадлежит данный файл
        /// </summary>
        public virtual ImportQueueRecordFile ImportQueueRecordFile { get; set; }
        #endregion
        #region AccountDataRecord
        /// <summary>
        /// Идентификатор записи, которой принадлежат данные
        /// </summary>
        [Key, Column("data_uid", Order = 2), ForeignKey(nameof(AccountDataRecord))]
        public Guid AccountDataRecordID { get; set; }
        /// <summary>
        /// Запись, которой принадлежат данные
        /// </summary>
        public virtual AccountDataRecord AccountDataRecord { get; set; }
        #endregion

        /// <summary>
        /// Дата загрузки
        /// </summary>
        [Column("load_date"), Required]
        public DateTime LoadDate { get; set; }

        #region ToString()

        public override string ToString()
        {
            return this.GetColumnPropertiesForEntity();
        }

        #endregion
    }
}
