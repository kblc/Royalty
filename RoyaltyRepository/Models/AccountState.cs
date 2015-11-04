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
        /// <summary>
        /// Состояния аккаунтов
        /// </summary>
        public DbSet<AccountState> AccountStates { get; set; }
    }

    /// <summary>
    /// Состояние аккаунта
    /// </summary>
    [Table("account_state")]
    public partial class AccountState : HistoryEntityBase
    {
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
        /// Время последнего запуска (UTC)
        /// </summary>
        [Column("last_batch")]
        public DateTime? LastBatch { get; set; }
        /// <summary>
        /// Данный аккаунт активен
        /// </summary>
        [Column("is_active"), Required]
        public bool IsActive { get; set; }
    }
}
