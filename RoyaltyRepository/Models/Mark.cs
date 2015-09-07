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
        /// Хранилище меток записей
        /// </summary>
        public DbSet<Mark> Marks { get; set; }
    }

    /// <summary>
    /// Метка записи
    /// </summary>
    [Table("mark")]
    public partial class Mark
    {
        /// <summary>
        /// Идентификатор записи
        /// </summary>
        [Key, Column("mark_id"), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long MarkID { get; set; }

        /// <summary>
        /// Системное имя метки, по которому можно подгрузить из ресурсов полное имя
        /// </summary>
        [Column("system_name"), Index("UIX_MARK_SYSTEMNAME", IsUnique = true)]
        [Required(ErrorMessageResourceName = "MarkNameRequred")]
        [MinLength(1, ErrorMessageResourceName = "MarkNameMinLength"), MaxLength(100, ErrorMessageResourceName = "MarkNameMaxLength")]
        public string SystemName { get; set; }

        /// <summary>
        /// Название метки (из файла ресурса)
        /// </summary>
        [NotMapped]
        public string Name 
        { 
            get 
            {
                var obj = RoyaltyRepository.Properties.Resources.ResourceManager.GetObject(string.Format("MARK_{0}", SystemName.ToUpper()));
                return obj == null ? SystemName : obj.ToString(); 
            } 
        }


    }
}
