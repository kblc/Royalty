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
        /// Хранилище городов
        /// </summary>
        public DbSet<City> Cities { get; set; }
    }

    /// <summary>
    /// Город
    /// </summary>
    [Table("city")]
    public partial class City
    {
        public City()
        {
            Areas = new List<Area>();
        }

        /// <summary>
        /// Идентификатор записи
        /// </summary>
        [Key, Column("city_id"), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long CityID { get; set; }

        /// <summary>
        /// Название города
        /// </summary>
        [Column("name"), Index("UIX_CITY_NAME", IsUnique = true)]
        [Required(ErrorMessageResourceName = "CityNameRequred")]
        [MinLength(1, ErrorMessageResourceName = "CityNameMinLength"), MaxLength(100, ErrorMessageResourceName = "CityNameMaxLength")]
        public string Name { get; set; }

        #region UndefinedArea
        /// <summary>
        /// Идентификатор района, который используется в случае, когда район не определен
        /// </summary>
        [ForeignKey("UndefinedArea"), Column("undefined_area_id")]
        public long? UndefinedAreaID { get; set; }
        /// <summary>
        /// Район, который используется в случае, когда район не определен
        /// </summary>
        public virtual Area UndefinedArea { get; set; }
        #endregion

        [InverseProperty("City")]
        public virtual ICollection<Area> Areas { get; set; }

        public override string ToString()
        {
            return string.Format("{0}:[city_id:'{1}',name:'{2}',undefined_area_id:'{3}']", this.GetType().Name, CityID, Name ?? "NULL", !UndefinedAreaID.HasValue ? "NULL" : UndefinedAreaID.Value.ToString());
        }
    }
}
