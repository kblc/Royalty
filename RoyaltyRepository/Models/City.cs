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
        /// Хранилище городов
        /// </summary>
        public DbSet<City> Cities { get; set; }
    }

    /// <summary>
    /// Город
    /// </summary>
    [Table("city")]
    public partial class City : HistoryEntityBase
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

        /// <summary>
        /// Телефонный код города
        /// </summary>
        [Column("phone_code"), MaxLength(10, ErrorMessageResourceName = "CityPhoneNumberCodeMaxLength")]
        public string PhoneNumberCode { get; set; }

        #region UndefinedArea
        [NotMapped]
        public Area UndefinedArea
        {
            get
            {
                var a = Areas.FirstOrDefault(ar => ar.IsDefault);
                if (a == null)
                { 
                    a = new Area() { Name = Area.defaultAreaName, IsDefault = true };
                    Areas.Add(a);
                }
                return a;
            }
        }
        #endregion

        [InverseProperty("City")]
        public virtual ICollection<Area> Areas { get; set; }

        public override string ToString()
        {
            return this.GetColumnPropertiesForEntity();
        }
    }
}
