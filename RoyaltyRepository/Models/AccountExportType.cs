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
        /// Варианты экспорта (типы экспорта по метке)
        /// </summary>
        public DbSet<AccountExportType> AccountExportTypes { get; set; }
    }

    [Table("export_type")]
    public partial class AccountExportType
    {
        /// <summary>
        /// Идентификатор записи
        /// </summary>
        [Key, Column("export_type_id"), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long AccountExportTypeID { get; set; }
        
        #region Account
        /// <summary>
        /// Идентификатор аккаунта, которому принадлежат данные
        /// </summary>
        [ForeignKey("Account"), Column("account_uid"), Index("IX_DATA_ACCOUNT_UID", IsUnique = false), Required]
        public Guid AccountID { get; set; }
        /// <summary>
        /// Аккаунт, которому принадлежат данные
        /// </summary>
        public virtual Account Account { get; set; }
        #endregion
        #region Mark
        /// <summary>
        /// Идентификатор метки
        /// </summary>
        [ForeignKey("Mark"), Column("mark_id"), Required]
        public long MarkID { get; set; }
        /// <summary>
        /// Метка
        /// </summary>
        public virtual Mark Mark { get; set; }
        #endregion

        /// <summary>
        /// Имя файла
        /// </summary>
        [Column("filename"), Required]
        public string FileName { get; set; }

        public override string ToString()
        {
            return this.GetColumnPropertiesForEntity();
        }
    }
}
