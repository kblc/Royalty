using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using RoyaltyRepository.Migrations;

namespace RoyaltyRepository.Models
{
    internal interface IDefaultRepositoryInitialization
    {
        void InitializeDefault(RepositoryContext context);
    }

    public partial class RepositoryContext : DbContext
    {
        #region Get Entity connection strings
        private static string GetEntityConnectionString(string connectionString, string connectionProviderName)
        {
            return new System.Data.Entity.Core.EntityClient.EntityConnectionStringBuilder()
            {
                Provider = connectionProviderName,
                ProviderConnectionString = connectionProviderName,
            }.ToString();
        }
        private static string GetEntityConnectionString(string connectionStringName)
        {
            return new System.Data.Entity.Core.EntityClient.EntityConnectionStringBuilder()
            {
                Name = connectionStringName,
            }.ToString();
        }

        private static string DefConnectionString = string.Empty;
        #endregion

        static RepositoryContext()
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<RepositoryContext, Migrations.Configuration>());
        }

        public RepositoryContext()
            : base() 
        {
            if (!string.IsNullOrEmpty(DefConnectionString))
                Database.Connection.ConnectionString = DefConnectionString;
        }
        public RepositoryContext(string connectionStringName)
            : base(GetEntityConnectionString(connectionStringName)) 
        {
            DefConnectionString = Database.Connection.ConnectionString;
        }
        public RepositoryContext(string connectionString, string connectionProviderName)
            : base(GetEntityConnectionString(connectionString, connectionProviderName)) 
        {
            DefConnectionString = Database.Connection.ConnectionString;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<AccountSettings>()
            //    .HasKey(e => e.AccountUID);
            //modelBuilder.Entity<Account>()
            //    .HasOptional(s => s.Settings)
            //    .WithRequired(ad => ad.Account);
        }

        public Action<string> Log
        {
            get { return Database.Log; }
            set { Database.Log = value; }
        }
    }
}
