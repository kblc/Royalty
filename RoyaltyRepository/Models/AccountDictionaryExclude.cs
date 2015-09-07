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
        public DbSet<AccountDictionaryExclude> AccountDictionaryExcludes { get; set; }
    }

    [Table("dictionary_exclude")]
    public partial class AccountDictionaryExclude
    {
        /// <summary>
        /// Идентификатор записи
        /// </summary>
        [Key, Column("dictionary_exclude_id"), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long DictionaryExcludeID { get; set; }
        /// <summary>
        /// Идентификатор словаря, которому принадлежат данная запись
        /// </summary>
        [Column("dictionary_uid"), ForeignKey("Dictionary"), Index("IX_DICTIONARY_EXCLUDE_DICTIONARY_UID", IsUnique = false), Required]
        public Guid DictionaryUID { get; set; }
        /// <summary>
        /// Словарь, которому принадлежат данная запись
        /// </summary>
        public virtual AccountDictionary Dictionary { get; set; }
        /// <summary>
        /// Строка для исключения из адреса
        /// </summary>
        [Column("exclude"), Required(ErrorMessageResourceName = "AccountDictionaryExcludeRequired")]
        [MinLength(1, ErrorMessageResourceName = "AccountDictionaryExcludeMinLength"), MaxLength(250, ErrorMessageResourceName = "AccountDictionaryExcludeMaxLength")]
        public string Exclude { get; set; }
    }
}
