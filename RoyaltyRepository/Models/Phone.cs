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
        /// Хранилище телефонов
        /// </summary>
        public DbSet<Phone> Phones { get; set; }
    }

    /// <summary>
    /// Телефон
    /// </summary>
    [Table("phone")]
    public partial class Phone : HistoryEntityBase
    {
        /// <summary>
        /// Идентификатор записи
        /// </summary>
        [Key, Column("phone_id"), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long PhoneID { get; set; }
        /// <summary>
        /// Номер телефона
        /// </summary>
        [Column("phone_number"), Index("UIX_PHONE_NUMBER", IsUnique = true)]
        [Required(ErrorMessageResourceName = "PhoneNumberRequired")]
        [MinLength(1, ErrorMessageResourceName = "PhoneNumberMinLength"), MaxLength(20, ErrorMessageResourceName = "PhoneNumberMaxLength")]
        public string PhoneNumber { get; set; }

        public override string ToString()
        {
            return this.GetColumnPropertiesForEntity();
        }
    }
}
