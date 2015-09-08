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
        #endregion

        static RepositoryContext()
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<RepositoryContext, Migrations.Configuration>());
        }

        public RepositoryContext() : base() { }
        public RepositoryContext(string connectionStringName) : base(GetEntityConnectionString(connectionStringName)) { }
        public RepositoryContext(string connectionString, string connectionProviderName) : base(GetEntityConnectionString(connectionString, connectionProviderName)) { }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //
        }

        public Action<string> Log
        {
            get { return Database.Log; }
            set { Database.Log = value; }
        }
    }
}
