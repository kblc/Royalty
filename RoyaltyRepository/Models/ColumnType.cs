using Helpers.Linq;
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
        /// Хранилище меток записей
        /// </summary>
        public DbSet<ColumnType> ColumnTypes { get; set; }
    }

    /// <summary>
    /// Тип колонки
    /// </summary>
    public enum ColumnTypes
    {
        Address = 0,
        Area,
        City,
        Host,
        Mark,
        Phone
    }

    /// <summary>
    /// Метка записи
    /// </summary>
    [Table("column_type")]
    public partial class ColumnType : IDefaultRepositoryInitialization
    {
        /// <summary>
        /// Идентификатор записи
        /// </summary>
        [Key, Column("column_type_id"), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ColumnTypeID { get; set; }

        /// <summary>
        /// Системное имя метки, по которому можно подгрузить из ресурсов полное имя
        /// </summary>
        [Column("system_name"), Index("UIX_COLUMNTYPE_SYSTEMNAME", IsUnique = true)]
        [Required(ErrorMessageResourceName = "ColumnTypeNameRequred"), MaxLength(100, ErrorMessageResourceName = "ColumnTypeNameMaxLength")]
        public string SystemName { get; set; }

        /// <summary>
        /// Название метки (из файла ресурса)
        /// </summary>
        [NotMapped]
        public string Name 
        { 
            get 
            {
                var obj = RoyaltyRepository.Properties.Resources.ResourceManager.GetObject(string.Format("COLUMNTYPE_{0}", SystemName.ToUpper()));
                return obj == null ? SystemName : obj.ToString(); 
            } 
        }

        public static string GetNameFromType(ColumnTypes type)
        {
            var obj = RoyaltyRepository.Properties.Resources.ResourceManager.GetObject(string.Format("COLUMNTYPE_{0}", type.ToString().ToUpper()));
            return obj == null ? type.ToString() : obj.ToString(); 
        }

        /// <summary>
        /// Тип колонки из существующих
        /// </summary>
        [NotMapped]
        public ColumnTypes Type
        {
            get
            {
                return typeof(ColumnTypes).GetEnumValues().Cast<ColumnTypes>().FirstOrDefault(ct => ct.ToString().ToUpper() == SystemName);
            }
            set
            {
                SystemName = value.ToString().ToUpper();
            }
        }

        /// <summary>
        /// Используется для валидации колонок импортируемых данных
        /// </summary>
        [Column("import_table_validation"), Required]
        public bool ImportTableValidation { get; set; }

        /// <summary>
        /// Используется для валидации строк импортируемых данных
        /// </summary>
        [Column("import_row_validation"), Required]
        public bool ImportRowValidation { get; set; }

        void IDefaultRepositoryInitialization.InitializeDefault(RepositoryContext context)
        {
            var defColumnTypes = new ColumnType[] 
            {
                new ColumnType() { SystemName = ColumnTypes.Address.ToString().ToUpper(), ImportRowValidation = true, ImportTableValidation = true },
                new ColumnType() { SystemName = ColumnTypes.Area.ToString().ToUpper(), ImportRowValidation = false, ImportTableValidation = true },
                new ColumnType() { SystemName = ColumnTypes.City.ToString().ToUpper(), ImportRowValidation = true, ImportTableValidation = true },
                new ColumnType() { SystemName = ColumnTypes.Host.ToString().ToUpper(), ImportRowValidation = true, ImportTableValidation = true },
                new ColumnType() { SystemName = ColumnTypes.Mark.ToString().ToUpper(), ImportRowValidation = false, ImportTableValidation = false },
                new ColumnType() { SystemName = ColumnTypes.Phone.ToString().ToUpper(), ImportRowValidation = true, ImportTableValidation = true },
            };

            context.ColumnTypes.AddRange(
                defColumnTypes
                    .LeftOuterJoin(context.ColumnTypes, ct => ct.SystemName, c => c.SystemName, (def, existed) => new { Default = def, Existed = existed })
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
