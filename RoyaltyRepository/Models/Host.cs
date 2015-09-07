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
        /// Хранилище хостов, откуда загружаются данные
        /// </summary>
        public DbSet<Host> Hosts { get; set; }
    }

    /// <summary>
    /// Хост
    /// </summary>
    [Table("host")]
    public partial class Host
    {
        /// <summary>
        /// Идентификатор записи
        /// </summary>
        [Key, Column("host_id"), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long HostID { get; set; }

        /// <summary>
        /// Название хоста
        /// </summary>
        [Column("name"), Index("UIX_HOST_NAME", IsUnique = true)]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "HostNameRequred")]
        [MaxLength(250, ErrorMessageResourceName = "HostNameMaxLength")]
        public string Name { get; set; }
    }
}
