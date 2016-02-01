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
        public DbSet<AccountSettingsColumn> AccountSettingsColumns { get; set; }
    }

    [Table("account_settings_column")]
    public partial class AccountSettingsColumn : HistoryEntityBase
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        [Column("account_settings_column_id", Order = 1), Key, Required, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long AccountSettingsColumnID { get; set; }

        #region AccountSettings
        /// <summary>
        /// Идентификатор аккаунта, которому принадлежат данные настройки
        /// </summary>
        [Column("account_uid"), ForeignKey("AccountSettings"), Required, Index("UIX_ACCOUNT_SETTINGS_COLUMN", IsUnique = true, Order = 2)]
        public Guid AccountUID { get; set; }
        /// <summary>
        /// Аккаунт, которому принадлежат данные настройки
        /// </summary>
        public virtual AccountSettings AccountSettings { get; set; }
        #endregion
        #region ColumnType
        /// <summary>
        /// Идентификатор типа колонки
        /// </summary>
        [Column("column_type_id"), ForeignKey("ColumnType"), Required, Index("UIX_ACCOUNT_SETTINGS_COLUMN", IsUnique = true, Order = 3)]
        public long ColumnTypeID { get; set; }
        /// <summary>
        /// Тип колонки
        /// </summary>
        public virtual ColumnType ColumnType { get; set; }
        #endregion

        /// <summary>
        /// Название колонки в импортируемом файле
        /// </summary>
        [Column("column_name"), Required]
        public string ColumnName { get; set; }
    }
}
