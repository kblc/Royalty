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
        /// Информация о выгрузках
        /// </summary>
        public DbSet<AccountDataRecordExport> AccountDataRecordExport { get; set; }
    }

    /// <summary>
    /// Информация об экспорте данных
    /// </summary>
    [Table("data_export")]
    public partial class AccountDataRecordExport : HistoryEntityBase
    {
        /// <summary>
        /// Идентификатор записи
        /// </summary>
        [Key, Column("data_export_id"), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long AccountDataRecordExportID { get; set; }

        #region AccountDataRecord
        /// <summary>
        /// Идентификатор записи, которой принадлежат расширенные данные
        /// </summary>
        [Column("data_uid"), ForeignKey("AccountDataRecord"), Required]
        public Guid AccountDataRecordID { get; set; }
        /// <summary>
        /// Запись, которой принадлежат расширенные данные
        /// </summary>
        public virtual AccountDataRecord AccountDataRecord { get; set; }
        #endregion
        #region Host
        /// <summary>
        /// Идентификатор хоста была загружена запись
        /// </summary>
        [Column("host_id"), ForeignKey("Host"), Required]
        public long HostID { get; set; }
        /// <summary>
        /// Запись, которой принадлежат расширенные данные
        /// </summary>
        public virtual Host Host { get; set; }
        #endregion
        #region File
        /// <summary>
        /// Идентификатор файла, в котором была загружена запись
        /// </summary>
        [Column("file_uid"), ForeignKey("File"), Required]
        public Guid FileUID { get; set; }
        /// <summary>
        /// Запись, которой принадлежат расширенные данные
        /// </summary>
        public virtual File File { get; set; }
        #endregion

        /// <summary>
        /// Дата экспорта
        /// </summary>
        [Column("export_date"), Required]
        public DateTime ExportDate { get; set; }

        #region Abstract implementation

        protected override object GetSourceId() => AccountDataRecordID;
        protected override HistorySourceType GetSourceType() => HistorySourceType.AccountData;

        #endregion
    }
}
