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
using System.ComponentModel;

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
        [Description("MARK_Default")]
        Default = 0,
        /// <summary>
        /// Полудоверенная
        /// </summary>
        [Description("MARK_HalfTrusted")]
        HalfTrusted,
        /// <summary>
        /// Не доверенная
        /// </summary>
        [Description("MARK_NotTrusted")]
        NotTrusted,
        /// <summary>
        /// Подозрительная
        /// </summary>
        [Description("MARK_Suspicious")]
        Suspicious,
        /// <summary>
        /// Доверенная
        /// </summary>
        [Description("MARK_Trusted")]
        Trusted,
        /// <summary>
        /// Неизвестная
        /// </summary>
        [Description("MARK_Unknown")]
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
        [Obsolete("Use Type property instead")]
        public string SystemName { get; set; }

        /// <summary>
        /// Название метки (из файла ресурса)
        /// </summary>
        [NotMapped]
        public string Name { get { return Type.GetEnumNameFromType(); } }

        /// <summary>
        /// Тип метки из существующих
        /// </summary>
        [NotMapped]
        public MarkTypes Type
        {
#pragma warning disable 618
            get { return RoyaltyRepository.Extensions.Helpers.GetEnumValueByName<MarkTypes>(SystemName); }
            set { SystemName = value.ToString().ToUpper(); }
#pragma warning restore 618
        }

        #region IDefaultRepositoryInitialization

        void IDefaultRepositoryInitialization.InitializeDefault(RepositoryContext context)
        {
            var defColumnTypes = typeof(MarkTypes).GetEnumValues().Cast<MarkTypes>().Select(t => new Mark() { Type = t });
#pragma warning disable 618
            context.Marks.AddRange(
                defColumnTypes
                    .LeftOuterJoin(context.Marks, ct => ct.SystemName, c => c.SystemName, (def, existed) => new { Default = def, Existed = existed })
                    .Where(i => i.Existed == null)
                    .Select(i => i.Default)
                );
#pragma warning restore 618
        }

        #endregion
        #region ToString()

        public override string ToString()
        {
            return this.GetColumnPropertiesForEntity();
        }

        #endregion
    }
}
