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
        /// Названия для колонок дополнительной таблицы
        /// </summary>
        public DbSet<AccountDataRecordAdditionalColumn> AccountDataRecordAdditionalColumns { get; set; }
    }

    [Table("data_additional_column")]
    public partial class AccountDataRecordAdditionalColumn
    {
        /// <summary>
        /// Идентификатор записи
        /// </summary>
        [Key, Column("data_additional_column_id"), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long AccountDataRecordAdditionalColumnID { get; set; }
        #region Account
        /// <summary>
        /// Идентификатор аккаунта, которому принадлежат данные настройки
        /// </summary>
        [Column("account_uid"), ForeignKey("Account"), Required, Index("UIX_DATA_ADDITIONAL_COLUMN_ACCOUNT_SYSTEMNAME", 1, IsUnique = true)]
        public Guid AccountUID { get; set; }
        /// <summary>
        /// Аккаунт, которому принадлежат данные настройки
        /// </summary>
        public virtual Account Account { get; set; }
        #endregion
        [Column("system_name"), Index("UIX_DATA_ADDITIONAL_COLUMN_ACCOUNT_SYSTEMNAME", 2, IsUnique = true)]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "DataAdditionalColumnSystemNameRequired"), MaxLength(250, ErrorMessageResourceName = "DataAdditionalColumnSystemNameMaxLength")]
        public string ColumnSystemName { get; set; }

        [Column("name")]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "DataAdditionalColumnNameRequired"), MaxLength(250, ErrorMessageResourceName = "DataAdditionalColumnNameMaxLength")]
        public string ColumnName { get; set; }
    }
}
