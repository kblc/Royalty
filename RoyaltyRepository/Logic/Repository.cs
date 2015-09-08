using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RoyaltyRepository.Models;

namespace RoyaltyRepository.Logic
{
    public partial class Repository : IDisposable
    {
        public RepositoryContext Context { get; private set; }

        public Repository()
        {
            Context = new RepositoryContext();
        }
        public Repository(string connectionStringName)
        {
            Context = new RepositoryContext(connectionStringName);
        }
        public Repository(string connectionString, string connectionProviderName)
        {
            Context = new RepositoryContext(connectionString, connectionProviderName);
        }

        public void SaveChanges(bool waitUntilSaving = true)
        {
            if (Context == null)
                throw new ArgumentNullException("Context", "Initialize Context first for Repository");
            if (waitUntilSaving)
                Context.SaveChanges();
            else
                Context.SaveChangesAsync();
        }

        public void Dispose()
        {
            if (Context != null)
            {
                Context.Dispose();
                Context = null;
            }
        }
    }
}
