using Helpers.Linq;
using RoyaltyRepository.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
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

    public class IsKeyAttribute : Attribute
    {
    }

    ////[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    //[AttributeUsage(AttributeTargets.All, Inherited = true)]
    //public class LinkToAccountDataRecordAttribute : Attribute
    //{

    //    public Expression<Func<AccountDataRecord, string>> AccountExpression { get; private set; }
    //    public LinkToAccountDataRecordAttribute(Expression<Func<AccountDataRecord, string>> accountExpression) { AccountExpression = accountExpression; }
    //}

    /// <summary>
    /// Тип колонки
    /// </summary>
    public enum ColumnTypes
    {
        //[LinkToAccountDataRecord(dr => dr.Street.Name + (!string.IsNullOrWhiteSpace(dr.HouseNumber) ? ", " + dr.HouseNumber : string.Empty))]
        [Description("COLUMNTYPE_ADDRESS")]
        [IsKey]
        Address = 0,
        [Description("COLUMNTYPE_AREA")]
        [IsKey]
        Area,
        [Description("COLUMNTYPE_CITY")]
        [IsKey]
        City,
        [Description("COLUMNTYPE_HOST")]
        Host,
        [Description("COLUMNTYPE_MARK")]
        Mark,
        [Description("COLUMNTYPE_PHONE")]
        [IsKey]
        Phone,
        [Description("COLUMNTYPE_CHANGED")]
        Changed,
        [Description("COLUMNTYPE_CREATED")]
        Created,
        [Description("COLUMNTYPE_EXPORTED")]
        Exported,
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
        public string Name { get { return Extensions.Extensions.GetEnumNameFromType(Type); } }

        /// <summary>
        /// Тип колонки из существующих
        /// </summary>
        [NotMapped]
        public ColumnTypes Type
        {
            get { return Extensions.Helpers.GetEnumValueByName<ColumnTypes>(SystemName); }
            set { SystemName = value.ToString().ToUpper(); }
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

        [Column("export_column_index"), Required]
        public long ExportColumnIndex { get; set; }

        [Column("export"), Required]
        public bool Export { get; set; }

        void IDefaultRepositoryInitialization.InitializeDefault(RepositoryContext context)
        {
            var defColumnTypes = new ColumnType[]
            {
                new ColumnType() { SystemName = ColumnTypes.Address.ToString().ToUpper(), ImportRowValidation = true, ImportTableValidation = true, ExportColumnIndex = 1, Export = true },
                new ColumnType() { SystemName = ColumnTypes.Area.ToString().ToUpper(), ImportRowValidation = false, ImportTableValidation = true, ExportColumnIndex = 2, Export = true },
                new ColumnType() { SystemName = ColumnTypes.City.ToString().ToUpper(), ImportRowValidation = true, ImportTableValidation = true, ExportColumnIndex = 3, Export = true },
                new ColumnType() { SystemName = ColumnTypes.Host.ToString().ToUpper(), ImportRowValidation = true, ImportTableValidation = true, ExportColumnIndex = 5, Export = true },
                new ColumnType() { SystemName = ColumnTypes.Mark.ToString().ToUpper(), ImportRowValidation = false, ImportTableValidation = false, ExportColumnIndex = 4, Export = true },
                new ColumnType() { SystemName = ColumnTypes.Phone.ToString().ToUpper(), ImportRowValidation = true, ImportTableValidation = true, ExportColumnIndex = 0, Export = true },
                new ColumnType() { SystemName = ColumnTypes.Changed.ToString().ToUpper(), ImportRowValidation = false, ImportTableValidation = false, ExportColumnIndex = 6, Export = true },
                new ColumnType() { SystemName = ColumnTypes.Created.ToString().ToUpper(), ImportRowValidation = false, ImportTableValidation = false, ExportColumnIndex = 7, Export = true },
                new ColumnType() { SystemName = ColumnTypes.Exported.ToString().ToUpper(), ImportRowValidation = false, ImportTableValidation = false, ExportColumnIndex = 8, Export = true },
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

    public static partial class RepositoryExtensions
    {
        public static object GetAccountDataForColumnType(this ColumnTypes type, AccountDataRecord data)
        {
            switch(type)
            {
                case ColumnTypes.Address:
                    return data.Street.Name + (string.IsNullOrWhiteSpace(data.HouseNumber) ? string.Empty : ", " + data.HouseNumber);
                case ColumnTypes.Area:
                    return data.Street.Area.Name;
                case ColumnTypes.Changed:
                    return data.Changed;
                case ColumnTypes.City:
                    return data.Street.Area.City.Name;
                case ColumnTypes.Created:
                    return data.Created;
                case ColumnTypes.Exported:
                    return data.Exported;
                case ColumnTypes.Host:
                    return data.Host.Name;
                case ColumnTypes.Mark:
                    return data.Account.PhoneMarks.FirstOrDefault(pm => pm.PhoneID == data.PhoneID)?.Mark?.Name;
                case ColumnTypes.Phone:
                    return data.Phone.PhoneNumber;
            }
            return null;
        }
    }
}
