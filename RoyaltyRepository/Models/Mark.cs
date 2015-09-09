﻿using System;
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

        void IDefaultRepositoryInitialization.InitializeDefault(RepositoryContext context)
        {
            var resType = System.Reflection.Assembly.GetExecutingAssembly().GetType("RoyaltyRepository.Properties.Resources");
            if (resType != null)
            { 
                foreach(var p in resType.GetProperties().Where(pi => pi.Name.StartsWith("MARK_")).Select(pi => pi.Name.Substring("MARK_".Length)))
                {
                    if (!context.Marks.Any(m => string.Compare(m.SystemName, p) == 0))
                        context.Marks.Add(new Mark() { SystemName = p });
                }
                context.SaveChanges();
            }
        }

        public override string ToString()
        {
            return string.Format("{0}:[mark_id:'{1}',system_name:'{2}']", this.GetType().Name, MarkID.ToString(), SystemName);
        }
    }
}
