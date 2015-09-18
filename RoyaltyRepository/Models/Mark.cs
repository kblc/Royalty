using RoyaltyRepository.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Helpers.Linq;

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
    /// Тип метки
    /// </summary>
    public enum MarkTypes
    {
        /// <summary>
        /// Не установленная
        /// </summary>
        Default = 0,
        /// <summary>
        /// Полудоверенная
        /// </summary>
        HalfTrusted,
        /// <summary>
        /// Не доверенная
        /// </summary>
        NotTrusted,
        /// <summary>
        /// Подозрительная
        /// </summary>
        Suspicious,
        /// <summary>
        /// Доверенная
        /// </summary>
        Trusted,
        /// <summary>
        /// Неизвестная
        /// </summary>
        Unknown
    }

    /// <summary>
    /// Метка записи
    /// </summary>
    [Table("mark")]
    public partial class Mark : IDefaultRepositoryInitialization
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

        public static string GetNameFromType(MarkTypes type)
        {
            var obj = RoyaltyRepository.Properties.Resources.ResourceManager.GetObject(string.Format("MARK_{0}", type.ToString().ToUpper()));
            return obj == null ? type.ToString() : obj.ToString();
        }

        /// <summary>
        /// Тип метки из существующих
        /// </summary>
        [NotMapped]
        public MarkTypes Type
        {
            get
            {
                return typeof(MarkTypes).GetEnumValues().Cast<MarkTypes>().FirstOrDefault(ct => ct.ToString().ToUpper() == SystemName);
            }
            set
            {
                SystemName = value.ToString().ToUpper();
            }
        }

        void IDefaultRepositoryInitialization.InitializeDefault(RepositoryContext context)
        {
            var defColumnTypes = new Mark[] 
            {
                new Mark() { Type = MarkTypes.Default },
                new Mark() { Type = MarkTypes.HalfTrusted },
                new Mark() { Type = MarkTypes.NotTrusted },
                new Mark() { Type = MarkTypes.Suspicious },
                new Mark() { Type = MarkTypes.Trusted },
                new Mark() { Type = MarkTypes.Unknown },
            };

            context.Marks.AddRange(
                defColumnTypes
                    .LeftOuterJoin(context.Marks, ct => ct.SystemName, c => c.SystemName, (def, existed) => new { Default = def, Existed = existed })
                    .Where(i => i.Existed == null)
                    .Select(i => i.Default)
                );
        }

        public override string ToString()
        {
            return this.GetColumnPropertiesForEntity();
        }
    }
}
