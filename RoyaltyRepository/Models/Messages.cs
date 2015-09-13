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
        /// Хранилище сообщений
        /// </summary>
        public DbSet<Message> Messages { get; set; }
    }

    /// <summary>
    /// Метка записи
    /// </summary>
    [Table("message")]
    public partial class Message
    {
        /// <summary>
        /// Идентификатор записи
        /// </summary>
        [Key, Column("message_uid")]
        public Guid MessageID { get; set; }

        /// <summary>
        /// Сообщение
        /// </summary>
        [Column("message_text"), Required]
        public string MessageText { get; set; }

        public override string ToString()
        {
            return this.GetColumnPropertiesForEntity();
        }
    }
}
