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
        [ForeignKey("Dictionary"), Column("dictionary_uid"), Index("IX_DICTIONARY_RECORD_DICTIONARY_UID", 1, IsUnique = true), Required]
        public Guid DictionaryUID { get; set; }
        /// <summary>
        /// Словарь, которому принадлежат данная запись
        /// </summary>
        public virtual AccountDictionary Dictionary { get; set; }
        #endregion
        #region Street
        /// <summary>
        /// Идентификатор улицы
        /// </summary>
        [ForeignKey("Street"), Column("street_id"), Index("IX_DICTIONARY_RECORD_DICTIONARY_UID", 2, IsUnique = true), Required]
        public long StreetID { get; set; }
        /// <summary>
        /// Улица
        /// </summary>
        public virtual Street Street { get; set; }
        #endregion        
        #region ChangeStreetTo
        /// <summary>
        /// Идентификатор улицы, на которую необходимо изменить текущую улицу
        /// </summary>
        [ForeignKey("ChangeStreetTo"), Column("change_to_street_id")]
        public long? ChangeStreetToID { get; set; }
        /// <summary>
        /// Улица, на которую необходимо изменить
        /// </summary>
        public virtual Street ChangeStreetTo { get; set; }
        #endregion 

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
