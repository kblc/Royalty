using RoyaltyRepository.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        /// Список возможных состояний записи очереди
        /// </summary>
        public DbSet<ImportQueueRecordState> ImportQueueRecordStates { get; set; }
    }

    public enum ImportQueueRecordStateType
    {
        /// <summary>
        /// Wait
        /// </summary>
        [Description("QUEUERECORDSTATE_WAIT")]
        Wait = 0,

        /// <summary>
        /// Error
        /// </summary>
        [Description("QUEUERECORDSTATE_ERROR")]
        Error,

        /// <summary>
        /// Processed
        /// </summary>
        [Description("QUEUERECORDSTATE_PROCESSED")]
        Processed,

        /// <summary>
        /// Try to process
        /// </summary>
        [Description("QUEUERECORDSTATE_TRYTOPROCESS")]
        TryToProcess
    }

    [Table("import_queue_record_state")]
    public partial class ImportQueueRecordState : EntityBase, IDefaultRepositoryInitialization
    {
        /// <summary>
        /// Идентификатор записи
        /// </summary>
        [Key, Column("import_queue_record_state_id"), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ImportQueueRecordStateID { get; set; }

        /// <summary>
        /// Системное имя, по которому можно подгрузить из ресурсов полное имя
        /// </summary>
        [Column("system_name"), Index("UIX_IMPORT_QUEUE_RECORD_STATE_SYSTEMNAME", IsUnique = true)]
        [Required(ErrorMessageResourceName = "QueueRecordStateNameRequred")]
        [MaxLength(40, ErrorMessageResourceName = "QueueRecordStateNameMaxLength")]
        [Obsolete("Use Type property instead")]
        public string SystemName { get; set; }

        /// <summary>
        /// Название метки (из файла ресурса)
        /// </summary>
        [NotMapped]
        public string Name { get { return Extensions.Extensions.GetEnumNameFromType(Type); } }

        /// <summary>
        /// Record type
        /// </summary>
        [NotMapped]
        public ImportQueueRecordStateType Type
        {
#pragma warning disable 618
            get { return Extensions.Helpers.GetEnumValueByName<ImportQueueRecordStateType>(SystemName); }
            set { SystemName = Type.ToString().ToUpper(); }
#pragma warning restore 618
        }

        /// <summary>
        /// Default initialization
        /// </summary>
        /// <param name="context"></param>
        void IDefaultRepositoryInitialization.InitializeDefault(RepositoryContext context)
        {
#pragma warning disable 618
            var defValues = typeof(ImportQueueRecordStateType)
                .GetEnumValues()
                .Cast<ImportQueueRecordStateType>()
                .Select(t => new ImportQueueRecordState() { Type = t })
                .LeftOuterJoin(context.ImportQueueRecordStates, d => d.SystemName, e => e.SystemName, (d,e) => new { Default = d, Existed = e })
                .Where(i => i.Existed == null)
                .Select(i => i.Default);
            context.ImportQueueRecordStates.AddRange(defValues);
#pragma warning restore 618
        }
    }
}
