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
        /// Расширение стандартной таблицы
        /// </summary>
        public DbSet<AccountDataRecordAdditional> AccountDataRecordAdditionals { get; set; }
    }

    [Table("data_additional")]
    public partial class AccountDataRecordAdditional
    {
        public const uint ColumnCount = 20;

        #region AccountDataRecord
        /// <summary>
        /// Идентификатор записи, которой принадлежат расширенные данные
        /// </summary>
        [Key, Column("data_uid"), ForeignKey("AccountDataRecord")]
        public Guid AccountDataRecordID { get; set; }
        /// <summary>
        /// Запись, которой принадлежат расширенные данные
        /// </summary>
        public virtual AccountDataRecord AccountDataRecord { get; set; }
        #endregion

        [Column("column00")]
        public string Column00 { get; set; }
        [Column("column01")]
        public string Column01 { get; set; }
        [Column("column02")]
        public string Column02 { get; set; }
        [Column("column03")]
        public string Column03 { get; set; }
        [Column("column04")]
        public string Column04 { get; set; }
        [Column("column05")]
        public string Column05 { get; set; }
        [Column("column06")]
        public string Column06 { get; set; }
        [Column("column07")]
        public string Column07 { get; set; }
        [Column("column08")]
        public string Column08 { get; set; }
        [Column("column09")]
        public string Column09 { get; set; }
        [Column("column10")]
        public string Column10 { get; set; }
        [Column("column11")]
        public string Column11 { get; set; }
        [Column("column12")]
        public string Column12 { get; set; }
        [Column("column13")]
        public string Column13 { get; set; }
        [Column("column14")]
        public string Column14 { get; set; }
        [Column("column15")]
        public string Column15 { get; set; }
        [Column("column16")]
        public string Column16 { get; set; }
        [Column("column17")]
        public string Column17 { get; set; }
        [Column("column18")]
        public string Column18 { get; set; }
        [Column("column19")]
        public string Column19 { get; set; }

        public override string ToString()
        {
            return this.GetColumnPropertiesForEntity();
        }
    }
}
