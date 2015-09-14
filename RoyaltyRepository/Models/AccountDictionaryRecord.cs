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
        public DbSet<AccountDictionaryRecord> AccountDictionaryRecords { get; set; }
    }

    [Table("dictionary_record")]
    public partial class AccountDictionaryRecord
    {
        public AccountDictionaryRecord()
        {
            Conditions = new List<AccountDictionaryRecordCondition>();
        }

        /// <summary>
        /// Идентификатор записи
        /// </summary>
        [Key, Column("dictionary_record_id"), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long AccountDictionaryRecordID { get; set; }
        #region AccountDictionary
        /// <summary>
        /// Идентификатор словаря, которому принадлежат данная запись
        /// </summary>
        [ForeignKey("Dictionary"), Column("dictionary_uid"), Index("IX_DICTIONARY_RECORD_DICTIONARY_UID", IsUnique = false), Required]
        public Guid DictionaryUID { get; set; }
        /// <summary>
        /// Словарь, которому принадлежат данная запись
        /// </summary>
        public virtual AccountDictionary Dictionary { get; set; }
        #endregion
        
        /// <summary>
        /// Улица
        /// </summary>
        [Column("street"), Required]
        public string Street { get; set; }

        #region Area
        /// <summary>
        /// Идентификатор района
        /// </summary>
        [ForeignKey("Area"), Column("area_id"), Required]
        public long AreaID { get; set; }
        /// <summary>
        /// Район
        /// </summary>
        public virtual Area Area { get; set; }
        #endregion        

        /// <summary>
        /// Переименование улицы
        /// </summary>
        [Column("rename_street_to")]
        public string RenameStreetTo { get; set; }

        /// <summary>
        /// Условия
        /// </summary>
        public virtual ICollection<AccountDictionaryRecordCondition> Conditions { get; set; }

        public override string ToString()
        {
            return this.GetColumnPropertiesForEntity();
        }
    }
}
