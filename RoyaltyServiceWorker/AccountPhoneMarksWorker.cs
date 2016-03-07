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
    public class AccountPhoneMarksWorker : ListWorker<AccountPhoneMark, long>
    {
        public const uint DEFAULT_ITEMS_PER_PAGE = 20;

        static AccountPhoneMarksWorker()
        {
            AutoMapper.Mapper.CreateMap<HistoryService.AccountPhoneMark, AccountPhoneMark>();
        }

        private readonly Guid accountId;
        private uint pageIndex = 0;
        private uint pageCount = 0;
        private uint itemsPerPage = DEFAULT_ITEMS_PER_PAGE;
        private string filter = string.Empty;

        public AccountPhoneMarksWorker(Guid accountId)
            : base(h => h.AccountPhoneMark
                .Select(i => AutoMapper.Mapper.Map<AccountService.AccountPhoneMark>(i))
            , h => h.AccountPhoneMark)
        {
            this.accountId = accountId;
        }

        public uint PageIndex { get { return pageIndex; } set { pageIndex = value; RaisePropertyChanged(); Refresh(); } }
        public uint PageCount { get { return pageCount; } private set { pageCount = value; RaisePropertyChanged(); Refresh(); } }
        public uint ItemsPerPage { get { return itemsPerPage; } set { itemsPerPage = value; RaisePropertyChanged(); Refresh(); } }
        public string Filter { get { return filter; } set { filter = value; RaisePropertyChanged(); Refresh(); } }

        protected override AccountPhoneMark[] ServiceGetData()
        {
            using (var sClient = new AccountService.AccountServiceClient())
            {
                var taskRes = sClient.GetAccountPhoneMark(accountId, Filter, PageIndex, ItemsPerPage);
                if (taskRes.Error != null)
                    throw new Exception(taskRes.Error);

                this.pageCount = (uint)taskRes.PageCount;
                this.pageIndex = (uint)taskRes.PageIndex;

                RaisePropertyChanged(() => PageCount);
                RaisePropertyChanged(() => PageIndex);

                return taskRes.Values.ToArray();
            }
        }

        protected override void ApplyHistoryChange(IEnumerable<AccountPhoneMark> items)
        {
            if (items == null || !items.Any()) return;

            var innerItems = items
                .Where(i => i.AccountUID == accountId)
                .Where(i => string.IsNullOrWhiteSpace(Filter) || i.PhoneNumber.Contains(Filter))
                .Select(i => AutoMapper.Mapper.Map<AccountService.AccountPhoneMark>(i));
            var itemsToUpdate = new List<AccountService.AccountPhoneMark>();
            var itemsToInsert = new List<AccountService.AccountPhoneMark>();

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
            var itemsToDelete = new AccountService.AccountPhoneMark[] { };
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
