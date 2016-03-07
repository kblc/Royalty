using Helpers;
using Helpers.Linq;
using RoyaltyServiceWorker.Additional;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RoyaltyServiceWorker.AccountService;

namespace RoyaltyServiceWorker
{
    public class AccountAdditionalColumnsWorker : ListWorker<AccountDataRecordAdditionalColumn, long>
    {
        static AccountAdditionalColumnsWorker()
        {
            AutoMapper.Mapper.CreateMap<HistoryService.AccountDataRecordAdditionalColumn, AccountDataRecordAdditionalColumn>();
        }

        private readonly Guid accountId;

        public AccountAdditionalColumnsWorker(Guid accountId)
            : base(h => h.AccountDataRecordAdditionalColumn
                .Select(i => AutoMapper.Mapper.Map<AccountService.AccountDataRecordAdditionalColumn>(i))
            , h => h.AccountSeriesOfNumbersRecord)
        {
            this.accountId = accountId;
        }

        protected override AccountDataRecordAdditionalColumn[] ServiceGetData()
        {
            using (var sClient = new AccountService.AccountServiceClient())
            {
                var taskRes = sClient.GetAdditionalColumns(accountId);
                if (taskRes.Error != null)
                    throw new Exception(taskRes.Error);
                return taskRes.Values.ToArray();
            }
        }

        protected override void ApplyHistoryChange(IEnumerable<AccountDataRecordAdditionalColumn> items)
        {
            if (items == null || !items.Any()) return;

            var innerItems = items
                .Where(i => i.AccountUID == accountId)
                .Select(i => AutoMapper.Mapper.Map<AccountService.AccountDataRecordAdditionalColumn>(i));
            var itemsToUpdate = new List<AccountService.AccountDataRecordAdditionalColumn>();
            var itemsToInsert = new List<AccountService.AccountDataRecordAdditionalColumn>();

            lock (Items)
            {
                var upd = Items
                    .RightOuterJoin(innerItems, a => a.Id, a => a.Id, (Existed, New) => new { Existed, New })
                    .ToArray();

                foreach (var i in upd)
                {
                    if (i.Existed == null)
                    {
                        Items.Add(i.New);
                        itemsToInsert.Add(i.New);
                    }
                    else
                    {
                        i.Existed.CopyObjectFrom(i.New);
                        itemsToUpdate.Add(i.Existed);
                    }
                }
            }

            if (itemsToUpdate.Count > 0)
                RaiseOnItemsChanged(itemsToUpdate.ToArray(), ChangeAction.Change);
            if (itemsToInsert.Count > 0)
                RaiseOnItemsChanged(itemsToInsert.ToArray(), ChangeAction.Add);
        }

        protected override void ApplyHistoryRemove(IEnumerable<long> ids)
        {
            if (ids == null) return;
            var itemsToDelete = new AccountService.AccountDataRecordAdditionalColumn[] { };
            lock (Items)
            {
                itemsToDelete = Items.Join(ids, a => a.Id, id => id, (a, id) => a).ToArray();
                foreach (var i in itemsToDelete)
                    Items.Remove(i);
            }
            if (itemsToDelete.Length > 0)
                RaiseOnItemsChanged(itemsToDelete, ChangeAction.Remove);
        }
    }
}
