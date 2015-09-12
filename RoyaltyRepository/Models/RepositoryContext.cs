using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using RoyaltyRepository.Migrations;
using System.Data.Common;

namespace RoyaltyRepository.Models
{
    internal class DisposableHelper : IDisposable
    {
        public DisposableHelper(Action onDisposeAction = null)
        {
            if (onDisposeAction != null)
                this.OnDispose += (s, e) => { onDisposeAction(); };
        }

        public event EventHandler OnDispose;

        public void Dispose()
        {
            var e = OnDispose;
            if (e != null)
                e(this, new EventArgs());
        }
    }

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
        private static DbConnection DefConnection = null;
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

        public RepositoryContext(DbConnection existingConnection, bool contextOwnsConnection)  
            : base(existingConnection, contextOwnsConnection)  
        {
        }

        private DbContextTransaction transaction = null;
        public IDisposable BeginTransaction(bool commitOnDispose)
        {
            if (transaction != null)
                throw new Exception("Transaction already started. Use CommitTransaction() or RollbackTransaction() before");
            transaction = Database.BeginTransaction();
            return new DisposableHelper(() => { if (commitOnDispose) CommitTransaction(); else RollbackTransaction(); });
        }

        public void CommitTransaction()
        {
            if (transaction != null)
                try
                {
                    transaction.Commit();
                }
                finally
                {
                    transaction.Dispose();
                    transaction = null;
                }
        }

        public void RollbackTransaction()
        {
            if (transaction != null)
                try
                {
                    transaction.Rollback();
                }
                finally
                {
                    transaction.Dispose();
                    transaction = null;
                }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                RollbackTransaction();
            base.Dispose(disposing);
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
