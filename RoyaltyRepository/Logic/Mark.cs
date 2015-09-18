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
        /// Get marks
        /// </summary>
        /// <returns>Mark queriable collection</returns>
        public IQueryable<Mark> MarkGet()
        {
            return this.Context.Marks;
        }
        /// <summary>
        /// Get one mark by identifier
        /// </summary>
        /// <param name="markId">Mark identifier</param>
        /// <returns>Mark</returns>
        public Mark MarkGet(long markId)
        {
            return MarkGet(new long[] { markId }).FirstOrDefault();
        }
        /// <summary>
        /// Get one mark by mark name
        /// </summary>
        /// <param name="systemName">Mark system name</param>
        /// <returns>Mark</returns>
        public Mark MarkGet(string systemName)
        {
            return MarkGet().FirstOrDefault(a => string.Compare(systemName, a.SystemName, true) == 0);
        }
        /// <summary>
        /// Get one mark by mark type
        /// </summary>
        /// <param name="markType">Mark type</param>
        /// <returns>Mark</returns>
        public Mark MarkGet(MarkTypes markType)
        {
            return MarkGet().FirstOrDefault(a => string.Compare(markType.ToString().ToUpper(), a.SystemName, true) == 0);
        }
        /// <summary>
        /// Get marks by identifiers
        /// </summary>
        /// <param name="instanceIds">Mark identifier array</param>
        /// <returns>Mark queriable collection</returns>
        public IQueryable<Mark> MarkGet(IEnumerable<long> instanceIds)
        {
            return MarkGet().Join(instanceIds, s => s.MarkID, i => i, (s, i) => s);
        }
    }
}
