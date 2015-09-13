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
        /// <summary>
        /// Очередь загрузки
        /// </summary>
        public DbSet<ImportQueueRecord> ImportQueue { get; set; }
    }

    [Table("queue")]
    public partial class ImportQueueRecord
    {
        /// <summary>
        /// Идентификатор записи
        /// </summary>
        [Key, Column("queue_uid")]
        public Guid ImportQueueRecordUID { get; set; }
        
        #region Account
        /// <summary>
        /// Идентификатор аккаунта, которому принадлежат данные
        /// </summary>
        [ForeignKey("Account"), Column("account_uid"), Index("IX_DATA_ACCOUNT_UID_AND_ISPROCESSED", 2, IsUnique = false), Required]
        public Guid AccountID { get; set; }
        /// <summary>
        /// Аккаунт, которому принадлежат данные
        /// </summary>
        public virtual Account Account { get; set; }
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
        /// <summary>
        /// Обработан ли файл
        /// </summary>
        [Column("is_processed"), Index("IX_DATA_ACCOUNT_UID_AND_ISPROCESSED", 1, IsUnique = false), Required]
        public bool IsProcessed { get; set; }
        
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
    }
}
