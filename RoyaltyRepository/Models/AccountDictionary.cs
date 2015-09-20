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
        public DbSet<AccountDictionary> AccountDictionaries { get; set; }
    }

    [Table("dictionary")]
    public partial class AccountDictionary
    {
        public AccountDictionary()
        {
            Excludes = new List<AccountDictionaryExclude>();
            Records = new List<AccountDictionaryRecord>();
        }
        /// <summary>
        /// Идентификатор аккаунта, которому принадлежат данные настройки
        /// </summary>
        [Key, Column("account_uid"), ForeignKey("Account"), Required]
        public Guid AccountUID { get; set; }
        /// <summary>
        /// Аккаунт, которому принадлежат данные настройки
        /// </summary>
        public virtual Account Account { get; set; }

        /// <summary>
        /// Процент (от 0 до 1) совпадения, для использования 
        /// </summary>
        [Column("similarity_for_trust"), Required]
        public decimal SimilarityForTrust { get; set; }

        /// <summary>
        /// Процент (от 0 до 1), при котором считать номер дома попавшим в радиус домов
        /// </summary>
        [Column("condition_score_for_trust"), Required]
        public decimal ConditionsScoreForTrust { get; set; }

        /// <summary>
        /// Разрешить добавлять слова в словарь автоматически
        /// </summary>
        [Column("allow_add_to_dictionary_automatically"), Required]
        public bool AllowAddToDictionaryAutomatically { get; set; }

        /// <summary>
        /// Производить расчёт для районов только если улица уже существует
        /// </summary>
        [Column("allow_calc_areas_if_street_exists_only"), Required]
        public bool AllowCalcAreasIfStreetExistsOnly { get; set; }

        public virtual ICollection<AccountDictionaryExclude> Excludes { get; set; }
        public virtual ICollection<AccountDictionaryRecord> Records { get; set; }

        public override string ToString()
        {
            return this.GetColumnPropertiesForEntity();
        }
    }
}
