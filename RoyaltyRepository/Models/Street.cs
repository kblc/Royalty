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
        /// Хранилище улиц
        /// </summary>
        public DbSet<Street> Streets { get; set; }
    }

    /// <summary>
    /// Улица
    /// </summary>
    [Table("street")]
    public partial class Street
    {
        /// <summary>
        /// Идентификатор записи
        /// </summary>
        [Key, Column("street_id"), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long StreetID { get; set; }

        #region Area
        /// <summary>
        /// Идентификатор района
        /// </summary>
        [ForeignKey("Area"), Column("area_id"), Index("UIX_AREA_STREET_NAME", 1, IsUnique = true), Required]
        public long AreaID { get; set; }
        /// <summary>
        /// Район
        /// </summary>
        public virtual Area Area { get; set; }
        #endregion
        /// <summary>
        /// Название улицы
        /// </summary>
        [Column("name"), Index("UIX_AREA_STREET_NAME", 2, IsUnique = true)]
        [Required(ErrorMessageResourceName = "StreetNameRequred"), MaxLength(100, ErrorMessageResourceName = "StreetNameMaxLength")]
        public string Name { get; set; }

        public override string ToString()
        {
            return this.GetColumnPropertiesForEntity();
        }
    }
}
