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
        /// Условия для работы правил в словаре
        /// </summary>
        public DbSet<AccountDictionaryRecordCondition> AccountDictionaryRecordConditions { get; set; }
    }

    [Table("dictionary_record_conditions")]
    public partial class AccountDictionaryRecordCondition : HistoryEntityBase
    {
        /// <summary>
        /// Идентификатор записи
        /// </summary>
        [Key, Column("dictionary_record_conditions_id"), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long AccountDictionaryRecordConditionsID { get; set; }
        /// <summary>
        /// Идентификатор записи словаря, которому принадлежат данные настройки
        /// </summary>
        [ForeignKey("DictionaryRecord"), Column("dictionary_record_id"), Index("IX_DICTIONARY_RECORD_CONDITIONS_DICTIONARY_RECORD_ID", IsUnique = false), Required]
        public long DictionaryRecordID { get; set; }
        /// <summary>
        /// Данные записи словаря, которому принадлежит данное условие
        /// </summary>
        public virtual AccountDictionaryRecord DictionaryRecord { get; set; }

        /// <summary>
        /// Номер дома, с которого действует данное условие
        /// </summary>
        [Column("from"), Required]
        public long From { get; set; }
        /// <summary>
        /// Номер дома, до которого действует данное условие
        /// </summary>
        [Column("to"), Required]
        public long To { get; set; }
        /// <summary>
        /// Только четные
        /// </summary>
        [Column("even")]
        public bool? Even { get; set; }
    }
}
