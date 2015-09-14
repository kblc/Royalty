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
        /// Список возможных состояний записи очереди
        /// </summary>
        public DbSet<ImportQueueRecordState> ImportQueueRecordStates { get; set; }
    }

    [Table("import_queue_record_state")]
    public partial class ImportQueueRecordState : IDefaultRepositoryInitialization
    {
        private const string strStart = "QUEUERECORDSTATE_";

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
        [MaxLength(20, ErrorMessageResourceName = "QueueRecordStateNameMaxLength")]
        public string SystemName { get; set; }

        /// <summary>
        /// Название метки (из файла ресурса)
        /// </summary>
        [NotMapped]
        public string Name
        {
            get
            {
                var obj = RoyaltyRepository.Properties.Resources.ResourceManager.GetObject(string.Format("{0}{1}", strStart, SystemName.ToUpper()));
                return obj == null ? SystemName : obj.ToString();
            }
        }

        void IDefaultRepositoryInitialization.InitializeDefault(RepositoryContext context)
        {
            
            var resType = System.Reflection.Assembly.GetExecutingAssembly().GetType("RoyaltyRepository.Properties.Resources");
            if (resType != null)
            {
                foreach (var p in resType.GetProperties().Where(pi => pi.Name.ToUpper().StartsWith(strStart)).Select(pi => pi.Name.Substring(strStart.Length)))
                    if (!context.ImportQueueRecordStates.Any(m => string.Compare(m.SystemName, p) == 0))
                        context.ImportQueueRecordStates.Add(new ImportQueueRecordState() { SystemName = p });
                context.SaveChanges();
            }
        }

        public override string ToString()
        {
            return this.GetColumnPropertiesForEntity();
        }
    }
}
