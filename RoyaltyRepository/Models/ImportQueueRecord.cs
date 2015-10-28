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
        /// Очередь загрузки
        /// </summary>
        public DbSet<ImportQueueRecord> ImportQueueRecords { get; set; }
    }

    /// <summary>
    /// Запись очереди загрузки
    /// </summary>
    [Table("import_queue")]
    public partial class ImportQueueRecord : HistoryEntityBase
    {
        private const string IX_NAME = "IX_DATA_ACCOUNT_UID_AND_PROCESSED";

        public ImportQueueRecord()
        {
            Files = new List<ImportQueueRecordFile>();
        }

        /// <summary>
        /// Идентификатор записи
        /// </summary>
        [Key, Column("import_queue_uid")]
        public Guid ImportQueueRecordUID { get; set; }
        
        #region Account
        /// <summary>
        /// Идентификатор аккаунта, которому принадлежат данные
        /// </summary>
        [ForeignKey("Account"), Column("account_uid"), Index(IX_NAME, 1, IsUnique = false), Required]
        public Guid AccountID { get; set; }
        /// <summary>
        /// Аккаунт, которому принадлежат данные
        /// </summary>
        public virtual Account Account { get; set; }
        #endregion

        /// <summary>
        /// Дата добавления в очередь
        /// </summary>
        [Column("created_date"), Index(IX_NAME, 2, IsUnique = false), Required]
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// Обработан ли файл
        /// </summary>
        [Column("processed_date"), Index(IX_NAME, 3, IsUnique = false)]
        public DateTime? ProcessedDate { get; set; }

        /// <summary>
        /// Имеются ли ошибки при обработке очереди
        /// </summary>
        [Column("has_error"), Required]
        public bool HasError { get; set; }

        /// <summary>
        /// Ошибка обработки самой очереди
        /// </summary>
        [Column("record_error")]
        public string Error { get; set; }

        public virtual ICollection<ImportQueueRecordFile> Files { get; set; }

        #region Abstract implementation

        protected override object GetSourceId() => ImportQueueRecordUID;
        protected override HistorySourceType GetSourceType() => HistorySourceType.Queue;

        #endregion
    }
}
