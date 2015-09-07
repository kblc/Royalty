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
        public DbSet<Account> Accounts { get; set; }
    }

    [Table("account")]
    public partial class Account
    {
        public Account()
        {
            Settings = new AccountSettings() { Account = this };
            State = new AccountState() { Account = this };
            Dictionary = new AccountDictionary() { Account = this };
            Data = new List<AccountDataRecord>();
            AdditionalColumns = new List<AccountDataRecordAdditionalColumn>();
        }

        [Key, Column("account_uid")]
        public Guid AccountUID { get; set; }

        [Column("name"), Index("UIX_ACCOUNT_NAME", IsUnique = true)]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "AccountNameRequred"), MaxLength(250, ErrorMessageResourceName = "AccountNameMaxLength")]
        public string Name { get; set; }

        [Column("is_hidden"), Required]
        public bool IsHidden { get; set; }

        public virtual AccountSettings Settings { get; set; }
        public virtual AccountState State { get; set; }
        public virtual AccountDictionary Dictionary { get; set; }

        public virtual IEnumerable<AccountDataRecord> Data { get; set; }
        public virtual IEnumerable<AccountDataRecordAdditionalColumn> AdditionalColumns { get; set; }
    }
}
