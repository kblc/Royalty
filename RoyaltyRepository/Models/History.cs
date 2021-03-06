﻿using RoyaltyRepository.Extensions;
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
        public DbSet<History> History { get; set; }
    }

    /// <summary>
    /// Тип записи в истории
    /// </summary>
    public enum HistoryActionType
    {
        /// <summary>
        /// Добавление записи
        /// </summary>
        [Description("HISTORYACTIONTYPE_Add")]
        Add = 0,
        /// <summary>
        /// Изменение записи
        /// </summary>
        [Description("HISTORYACTIONTYPE_Change")]
        Change,
        /// <summary>
        /// Удаление записи
        /// </summary>
        [Description("HISTORYACTIONTYPE_Remove")]
        Remove,
    }

    /// <summary>
    /// Метка записи
    /// </summary>
    [Table("history")]
    public partial class History: EntityBase
    {
        /// <summary>
        /// Идентификатор записи
        /// </summary>
        [Key, Column("history_id"), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long HistoryID { get; set; }

        #region ActionType

        /// <summary>
        /// Системное имя действия для истории
        /// </summary>
        [Column("action_type_system_name"), Required, MaxLength(100)]
        [Obsolete("Use ActionType property instead")]
        public string ActionTypeSystemName { get; set; }

        /// <summary>
        /// Название метки (из файла ресурса)
        /// </summary>
        [NotMapped]
        public string ActionTypeName { get { return ActionType.GetEnumNameFromType(); } }

        /// <summary>
        /// Тип метки из существующих
        /// </summary>
        [NotMapped]
        public HistoryActionType ActionType
        {
#pragma warning disable 618
            get { return RoyaltyRepository.Extensions.Helpers.GetEnumValueByName<HistoryActionType>(ActionTypeSystemName); }
            set { ActionTypeSystemName = value.ToString().ToUpper(); }
#pragma warning restore 618
        }

        #endregion

        /// <summary>
        /// Системное имя действия для истории
        /// </summary>
        [Column("source_type_system_name"), Required, MaxLength(100)]
        public string SourceName { get; set; }

        /// <summary>
        /// Идентификаторы записи источника
        /// </summary>
        [Column("source_id"), Required]
        public string SourceID { get; set; }

        /// <summary>
        /// Дата изменения
        /// </summary>
        [Column("date"), Required]
        public DateTime Date { get; set; }
    }
}
