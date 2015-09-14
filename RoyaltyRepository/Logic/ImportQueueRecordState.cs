using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RoyaltyRepository.Models;
using Helpers;

namespace RoyaltyRepository
{
    public partial class Repository
    {
        /// <summary>
        /// Get ImportQueueRecordState instances
        /// </summary>
        /// <returns>Instances queriable collection</returns>
        public IQueryable<ImportQueueRecordState> ImportQueueRecordStateGet()
        {
            return this.Context.ImportQueueRecordStates;
        }
        /// <summary>
        /// Get instance by identifier
        /// </summary>
        /// <param name="importQueueRecordStateId">Instance identifier</param>
        /// <returns>Instance by identifier</returns>
        public ImportQueueRecordState ImportQueueRecordStateGet(long importQueueRecordStateId)
        {
            return ImportQueueRecordStateGet(new long[] { importQueueRecordStateId }).FirstOrDefault();
        }
        /// <summary>
        /// Get instance by name
        /// </summary>
        /// <param name="systemName">Mark system name</param>
        /// <returns>Instance by name</returns>
        public ImportQueueRecordState ImportQueueRecordStateGet(string systemName)
        {
            return ImportQueueRecordStateGet().FirstOrDefault(a => string.Compare(systemName, a.SystemName, true) == 0);
        }
        /// <summary>
        /// Get instances by identifiers
        /// </summary>
        /// <param name="importQueueRecordStateId">Instance identifier array</param>
        /// <returns>Instances queriable collection</returns>
        public IQueryable<ImportQueueRecordState> ImportQueueRecordStateGet(IEnumerable<long> importQueueRecordStateId)
        {
            return ImportQueueRecordStateGet().Where(a => importQueueRecordStateId.Contains(a.ImportQueueRecordStateID));
        }
    }
}
