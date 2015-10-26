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
        #region AccountSettings
        /// <summary>
        /// Идентификатор аккаунта, которому принадлежат данные настройки
        /// </summary>
        [Column("account_uid", Order = 0), ForeignKey("AccountSettings"), Key, Required]
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
        [Column("column_type_id", Order = 1), ForeignKey("ColumnType"), Key, Required]
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

        #region Abstract implementation

        protected override object GetSourceId() => ((IHistoryRecordSource)AccountSettings).SourceId;
        protected override HistorySourceType GetSourceType() => HistorySourceType.AccountSettings;

        #endregion
    }
}
