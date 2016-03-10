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
using RoyaltyServiceWorker.HistoryService;

namespace RoyaltyServiceWorker
{
    public class AccountImportQueueRecordsWorker : ListWorker<AccountService.ImportQueueRecord, Guid>
    {
        public const uint DEFAULT_ITEMS_PER_PAGE = 20;

        static AccountImportQueueRecordsWorker()
        {
            AutoMapper.Mapper.CreateMap<HistoryService.ImportQueueRecord, AccountService.ImportQueueRecord>();
            AutoMapper.Mapper.CreateMap<HistoryService.ImportQueueRecordFileAccountDataRecord, AccountService.ImportQueueRecordFileAccountDataRecord>();
            AutoMapper.Mapper.CreateMap<HistoryService.ImportQueueRecordFileInfo, AccountService.ImportQueueRecordFileInfo>();
            AutoMapper.Mapper.CreateMap<HistoryService.ImportQueueRecordFileInfoFile, AccountService.ImportQueueRecordFileInfoFile>();
        }

        private readonly Guid accountId;
        private uint pageIndex = 0;
        private uint pageCount = 0;
        private uint itemsPerPage = DEFAULT_ITEMS_PER_PAGE;
        private DateTime? to = null;
        private DateTime? from = null;

        public AccountImportQueueRecordsWorker(Guid accountId)
            : base(h => h.ImportQueueRecord.Select(i => AutoMapper.Mapper.Map<AccountService.ImportQueueRecord>(i))
            , h => h.ImportQueueRecord) 
        {
            this.accountId = accountId;
        }

        public uint PageIndex { get { return pageIndex; } set { pageIndex = value; RaisePropertyChanged(); Refresh(); } }
        public uint PageCount { get { return pageCount; } private set { pageCount = value; RaisePropertyChanged(); Refresh(); } }
        public uint ItemsPerPage { get { return itemsPerPage; } set { itemsPerPage = value; RaisePropertyChanged(); Refresh(); } }
        public DateTime? From { get { return from; } set { from = value; RaisePropertyChanged(); Refresh(); } }
        public DateTime? To { get { return to; } set { to = value; RaisePropertyChanged(); Refresh(); } }

        protected override AccountService.ImportQueueRecord[] ServiceGetData() 
        {
            using (var sClient = new AccountService.AccountServiceClient())
            {
                var taskRes = sClient.GetImportQueueRecords(accountId, From, To, PageIndex, ItemsPerPage);
                if (taskRes.Error != null)
                    throw new Exception(taskRes.Error);

                this.pageCount = (uint)taskRes.PageCount;
                this.pageIndex = (uint)taskRes.PageIndex;

                RaisePropertyChanged(() => PageCount);
                RaisePropertyChanged(() => PageIndex);

                return taskRes.Values.ToArray();
            }
        }

        protected override void ApplyHistoryChange(IEnumerable<AccountService.ImportQueueRecord> items)
        {
            if (items == null || !items.Any()) return;

            var innerItems = items
                .Where(i => i.AccountUID == accountId)
                .Where(i => !from.HasValue || i.CreatedDate >= from.Value)
                .Where(i => !to.HasValue || i.CreatedDate <= to.Value)
                .Select(i => AutoMapper.Mapper.Map<AccountService.ImportQueueRecord>(i));
            var itemsToUpdate = new List<AccountService.ImportQueueRecord>();
            var itemsToInsert = new List<AccountService.ImportQueueRecord>();

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

        protected override void ApplyHistoryRemove(IEnumerable<Guid> ids)
        {
            if (ids == null) return;
            var itemsToDelete = new AccountService.ImportQueueRecord[] { };
            lock (Items)
            {
                itemsToDelete = Items.Join(ids, a => a.Id, id => id, (a, id) => a).ToArray();
                foreach (var i in itemsToDelete)
                    Items.Remove(i);
            }
            if (itemsToDelete.Length > 0)
                RaiseOnItemsChanged(itemsToDelete, ChangeAction.Remove);
        }

        protected void ApplyHistoryChangeFileAccountDataRecord(IEnumerable<AccountService.ImportQueueRecordFileAccountDataRecord> items)
        {

        }
        protected void ApplyHistoryRemoveFileAccountDataRecord(IEnumerable<long> ids)
        {

        }

        protected void ApplyHistoryChangeFileInfo(IEnumerable<AccountService.ImportQueueRecordFileInfo> items)
        {
            var raiseItems = new AccountService.ImportQueueRecord[] { };
            lock (Items)
            {
                var itemsToChange = Items.Join(items, i => i.Id, i => i.ImportQueueRecordUID, (item, changed) => new { item, changed })
                    .Select(i => new { i.item, i.changed, existed = i.item.FileInfoes.FirstOrDefault(fi => fi.Id == i.changed.Id) })
                    .ToArray();
                foreach(var i in itemsToChange)
                {
                    if (i.existed == null)
                        i.item.FileInfoes.Add(i.changed);
                    else
                        i.existed.CopyObjectFrom(i.changed);
                }
                raiseItems = itemsToChange.Select(i => i.item).Distinct().ToArray();
            }
            if (raiseItems.Length > 0)
                RaiseOnItemsChanged(raiseItems, ChangeAction.Change);
        }
        protected void ApplyHistoryRemoveFileInfo(IEnumerable<Guid> ids)
        {
            var raiseItems = new AccountService.ImportQueueRecord[] { };
            lock (Items)
            {
                var itemsToChange = Items.SelectMany(i => i.FileInfoes.Select(fi => new { item = i, fileInfo = fi }))
                    .Join(ids, i => i.fileInfo.Id, i => i, (item, changed) => new { item.item, item.fileInfo })
                    .ToArray();
                foreach (var i in itemsToChange)
                    i.item.FileInfoes.Remove(i.fileInfo);
                
                raiseItems = itemsToChange.Select(i => i.item).Distinct().ToArray();
            }
            if (raiseItems.Length > 0)
                RaiseOnItemsChanged(raiseItems, ChangeAction.Change);
        }

        protected void ApplyHistoryChangeFileInfoFile(IEnumerable<AccountService.ImportQueueRecordFileInfoFile> items)
        {

        }
        protected void ApplyHistoryRemoveFileInfoFile(IEnumerable<Guid> ids)
        {

        }

        public override void ApplyHistoryChanges(History e)
        {
            base.ApplyHistoryChanges(e);
            if (e.Add != null)
            {
                if (e.Add.ImportQueueRecordFileInfo != null)
                    ApplyHistoryChangeFileInfo(e.Add.ImportQueueRecordFileInfo.Select(i => AutoMapper.Mapper.Map<AccountService.ImportQueueRecordFileInfo>(i)));

                if (e.Add.ImportQueueRecordFileInfoFile != null)
                    ApplyHistoryChangeFileInfoFile(e.Add.ImportQueueRecordFileInfoFile.Select(i => AutoMapper.Mapper.Map<AccountService.ImportQueueRecordFileInfoFile>(i)));

                if (e.Add.ImportQueueRecordFileAccountDataRecord != null)
                    ApplyHistoryChangeFileAccountDataRecord(e.Add.ImportQueueRecordFileAccountDataRecord.Select(i => AutoMapper.Mapper.Map<AccountService.ImportQueueRecordFileAccountDataRecord>(i)));
            }
            if (e.Change != null)
            {
                if (e.Change.ImportQueueRecordFileInfo != null)
                    ApplyHistoryChangeFileInfo(e.Change.ImportQueueRecordFileInfo.Select(i => AutoMapper.Mapper.Map<AccountService.ImportQueueRecordFileInfo>(i)));

                if (e.Change.ImportQueueRecordFileInfoFile != null)
                    ApplyHistoryChangeFileInfoFile(e.Change.ImportQueueRecordFileInfoFile.Select(i => AutoMapper.Mapper.Map<AccountService.ImportQueueRecordFileInfoFile>(i)));

                if (e.Change.ImportQueueRecordFileAccountDataRecord != null)
                    ApplyHistoryChangeFileAccountDataRecord(e.Change.ImportQueueRecordFileAccountDataRecord.Select(i => AutoMapper.Mapper.Map<AccountService.ImportQueueRecordFileAccountDataRecord>(i)));
            }
            if (e.Remove != null)
            {
                if (e.Remove.ImportQueueRecordFileInfo != null)
                    ApplyHistoryRemoveFileInfo(e.Remove.ImportQueueRecordFileInfo);

                if (e.Remove.ImportQueueRecordFileInfoFile != null)
                    ApplyHistoryRemoveFileInfoFile(e.Remove.ImportQueueRecordFileInfoFile);

                if (e.Remove.ImportQueueRecordFileAccountDataRecord != null)
                    ApplyHistoryRemoveFileAccountDataRecord(e.Remove.ImportQueueRecordFileAccountDataRecord);
            }
        }
    }
}
