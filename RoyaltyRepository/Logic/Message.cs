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
        /// Add Message to database
        /// </summary>
        /// <param name="instance">Message instance</param>
        /// <param name="saveAfterInsert">Save database after insertion</param>
        /// <param name="waitUntilSaving">Wait until saving</param>
        public void MessageAdd(Message instance, bool saveAfterInsert = true, bool waitUntilSaving = true)
        {
            MessageAdd(new Message[] { instance }, saveAfterInsert, waitUntilSaving);
        }
        /// <summary>
        /// Add Messages to database
        /// </summary>
        /// <param name="instances">Message instance array</param>
        /// <param name="saveAfterInsert">Save database after insertion</param>
        /// <param name="waitUntilSaving">Wait until saving</param>
        public void MessageAdd(IEnumerable<Message> instances, bool saveAfterInsert = true, bool waitUntilSaving = true)
        {
            try
            {
                if (instances == null)
                    throw new ArgumentNullException("instances");
                instances = instances.Where(i => i != null).ToArray();

                try
                {
                    this.Context.Messages.AddRange(instances);
                    if (saveAfterInsert)
                        this.SaveChanges(waitUntilSaving);
                }
                catch (Exception ex)
                {
                    var e = new Exception(ex.Message, ex);
                    for (int i = 0; i < instances.Count(); i++)
                        e.Data.Add(string.Format("instance_{0}", i), instances.ElementAt(i).ToString());
                    throw e;
                }
            }
            catch (Exception ex)
            {
                Helpers.Log.Add(ex, string.Format("Repository.MessageAdd(instances=[{0}],saveAfterInsert={1},waitUntilSaving={2})", instances == null ? "NULL" : instances.Count().ToString(), saveAfterInsert, waitUntilSaving));
                throw;
            }
        }
        /// <summary>
        /// Remove Message from database
        /// </summary>
        /// <param name="instance">Message instance</param>
        /// <param name="saveAfterRemove">Save database after removing</param>
        /// <param name="waitUntilSaving">Wait until saving</param>
        public void MessageRemove(Message instance, bool saveAfterRemove = true, bool waitUntilSaving = true)
        {
            MessageRemove(new Message[] { instance }, saveAfterRemove, waitUntilSaving);
        }
        /// <summary>
        /// Remove Messages from database
        /// </summary>
        /// <param name="instances">Message instance array</param>
        /// <param name="saveAfterRemove">Save database after removing</param>
        /// <param name="waitUntilSaving">Wait until saving</param>
        public void MessageRemove(IEnumerable<Message> instances, bool saveAfterRemove = true, bool waitUntilSaving = true)
        {
            try
            {
                if (instances == null)
                    throw new ArgumentNullException("instances");
                instances = instances.Where(i => i != null).ToArray();
                var files = instances.SelectMany(i => i.Files).ToArray();

                try
                {
                    var save = new Action(() => 
                    {
                        this.Context.Messages.RemoveRange(instances);
                        this.FileRemove(files, saveAfterRemove: false);
                    });

                    if (saveAfterRemove)
                        using(BeginTransaction(commitOnDispose: true))
                        {
                            save();
                            this.SaveChanges(waitUntilSaving);
                        }
                    else
                        save();
                }
                catch (Exception ex)
                {
                    var e = new Exception(ex.Message, ex);
                    for (int i = 0; i < instances.Count(); i++)
                        e.Data.Add(string.Format("instance_{0}", i), instances.ElementAt(i).ToString());
                    throw e;
                }
            }
            catch (Exception ex)
            {
                Helpers.Log.Add(ex, string.Format("Repository.MessageRemove(instances=[{0}],saveAfterRemove={1},waitUntilSaving={2})", instances == null ? "NULL" : instances.Count().ToString(), saveAfterRemove, waitUntilSaving));
                throw;
            }
        }
        /// <summary>
        /// Create/Get new Message without any link to database
        /// </summary>
        /// <param name="MessageNumber">Message number</param>
        /// <returns>Message instance</returns>
        public Message MessageNew(string messageText = null)
        {
            try
            {
                var res = new Message() { MessageID = Guid.NewGuid() };
                if (messageText != null)
                    res.MessageText = messageText;
                return res;
            }
            catch (Exception ex)
            {
                Helpers.Log.Add(ex, string.Format("Repository.MessageNew(messageText='{0}')", messageText ?? "NULL"));
                throw;
            }
        }
        /// <summary>
        /// Get Messages
        /// </summary>
        /// <returns>Message queriable collection</returns>
        public IQueryable<Message> MessageGet()
        {
            return this.Context.Messages;
        }
        /// <summary>
        /// Get one Message by identifier
        /// </summary>
        /// <param name="instanceId">Message identifier</param>
        /// <returns>Message</returns>
        public Message MessageGet(Guid instanceId)
        {
            return MessageGet(new Guid[] { instanceId }).FirstOrDefault();
        }
        /// <summary>
        /// Get Messages by identifiers
        /// </summary>
        /// <param name="instanceIds">Message identifier array</param>
        /// <returns>Message queriable collection</returns>
        public IQueryable<Message> MessageGet(IEnumerable<Guid> instanceIds)
        {
            return MessageGet().Join(instanceIds, s => s.MessageID, i => i, (s, i) => s);
        }
    }
}
