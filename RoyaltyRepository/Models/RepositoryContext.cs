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
        static RepositoryContext()
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<RepositoryContext, Migrations.Configuration>());
        }

        public RepositoryContext() : 
            base()
        {
        }

        public RepositoryContext(string connectionStringName) :
            base(string.Format("name={0}", connectionStringName))
        {
            //Database.Connection.ConnectionString = string.Format("name={0}", connectionStringName);
        }

        public RepositoryContext(string connectionString, string connectionProviderName)
            : base(new System.Data.Entity.Core.EntityClient.EntityConnectionStringBuilder()
            {
                Provider = connectionProviderName,
                ProviderConnectionString = connectionProviderName
            }.ToString())
        {
            Database.Connection.ConnectionString = new System.Data.Entity.Core.EntityClient.EntityConnectionStringBuilder()
            {
                Provider = connectionProviderName,
                ProviderConnectionString = connectionProviderName
            }.ToString();
        }

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
