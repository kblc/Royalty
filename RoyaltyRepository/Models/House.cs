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
        /// Хранилище домов
        /// </summary>
        public DbSet<House> Houses { get; set; }
    }

    /// <summary>
    /// Дом
    /// </summary>
    [Table("house")]
    public partial class House
    {
        /// <summary>
        /// Идентификатор записи
        /// </summary>
        [Key, Column("house_id"), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long HouseID { get; set; }

        #region Street
        /// <summary>
        /// Идентификатор улицы
        /// </summary>
        [ForeignKey("Street"), Column("street_id"), Index("UIX_STREET_HOUSE_NUMBER", 1, IsUnique = true), Required]
        public long StreetID { get; set; }
        /// <summary>
        /// Улица
        /// </summary>
        public virtual Street Street { get; set; }
        #endregion
        /// <summary>
        /// Номер дома
        /// </summary>
        [Column("number"), Index("UIX_STREET_HOUSE_NUMBER", 2, IsUnique = true)]
        [Required(ErrorMessageResourceName = "HouseNumberRequred")]
        [MaxLength(20, ErrorMessageResourceName = "HouseNumberMaxLength")]
        public string Number { get; set; }

        public override string ToString()
        {
            return this.GetColumnPropertiesForEntity();
        }
    }
}
