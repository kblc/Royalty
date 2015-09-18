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
        /// Хранилище районов
        /// </summary>
        public DbSet<Area> Areas { get; set; }
    }

    /// <summary>
    /// Район
    /// </summary>
    [Table("area")]
    public partial class Area
    {
        internal const string defaultAreaName = ".default";

        public Area()
        {
            Streets = new List<Street>();
        }
        /// <summary>
        /// Идентификатор записи
        /// </summary>
        [Key, Column("area_id"), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long AreaID { get; set; }

        #region City
        /// <summary>
        /// Идентификатор города
        /// </summary>
        [ForeignKey("City"), Column("city_id"), Required, Index("UIX_AREA_NAME", 1, IsUnique = true)]
        public long CityID { get; set; }
        /// <summary>
        /// Город
        /// </summary>
        public virtual City City { get; set; }
        #endregion
        /// <summary>
        /// Данный район - район 'по умолчанию' для города
        /// </summary>
        [Column("is_default"), Required]
        public bool IsDefault { get; set; }
        /// <summary>
        /// Название района
        /// </summary>
        [Column("name"), Index("UIX_AREA_NAME", 2, IsUnique = true)]
        [Required(ErrorMessageResourceName = "AreaNameRequred")]
        [MinLength(1, ErrorMessageResourceName = "AreaNameMinLength"), MaxLength(100, ErrorMessageResourceName = "AreaNameMaxLength")]
        public string Name { get; set; }

        public virtual ICollection<Street> Streets { get; set; }

        public override string ToString()
        {
            return this.GetColumnPropertiesForEntity();
        }
    }
}
