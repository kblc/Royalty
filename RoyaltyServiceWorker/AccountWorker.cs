using Helpers;
using Helpers.Linq;
using RoyaltyServiceWorker.Additional;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RoyaltyServiceWorker
{
    public class AccountWorker : AbstractBaseWorker
    {
        static AccountWorker()
        {
            AutoMapper.Mapper.CreateMap<HistoryService.Account, AccountService.Account>();
            AutoMapper.Mapper.CreateMap<HistoryService.AccountSettings, AccountService.AccountSettings>();
            AutoMapper.Mapper.CreateMap<HistoryService.AccountSettingsColumn, AccountService.AccountSettingsColumn>();
            AutoMapper.Mapper.CreateMap<HistoryService.AccountSettingsExportDirectory, AccountService.AccountSettingsExportDirectory>();
            AutoMapper.Mapper.CreateMap<HistoryService.AccountSettingsImportDirectory, AccountService.AccountSettingsImportDirectory>();
            AutoMapper.Mapper.CreateMap<HistoryService.AccountSettingsSheduleTime, AccountService.AccountSettingsSheduleTime>();
        }

        private Thread initThread = null;
        private CancellationTokenSource stopCancellationTokenSource = null;

        private readonly List<AccountService.Account> Accounts = new List<AccountService.Account>();
        private readonly List<AccountService.Mark> Marks = new List<AccountService.Mark>();
        private readonly List<AccountService.ColumnType> ColumnTypes = new List<AccountService.ColumnType>();

        protected override bool DoStart()
        {
            stopCancellationTokenSource = new CancellationTokenSource();
            try
            {
                if (initThread == null)
                {
                    initThread = new Thread(new ParameterizedThreadStart(InitThread));
                    initThread.IsBackground = true;
                    initThread.Start(Context);
                }
                return true;
            }
            catch (Exception ex)
            {
                stopCancellationTokenSource.Dispose();
                stopCancellationTokenSource = null;
                SetError(ex);
                return false;
            }
        }
        protected override bool DoStop()
        {
            stopCancellationTokenSource.Cancel();
            try
            {
                if (initThread != null && initThread.IsAlive)
                    initThread.Abort();
                return true;
            }
            catch (Exception ex)
            {
                SetError(ex);
                return false;
            }
            finally
            {
                initThread = null;
            }
        }

        private void InitThread(object context)
        {
            var modelLevelThContext = (System.Runtime.Remoting.Contexts.Context)context;
            bool inited = false;

            #region Infinity try to connect then init and exit

            do
            {
                try
                {
                    using (var sClient = new AccountService.AccountServiceClient())
                    {
                        var getAccountsTask = sClient.GetAllAsync();
                        var getAccountsResTask = getAccountsTask.ContinueWith(res => 
                        {
                            stopCancellationTokenSource.Token.ThrowIfCancellationRequested();
                            if (res.Result.Error != null)
                                throw new Exception(res.Result.Error);
                            return res.Result.Values;
                        }, stopCancellationTokenSource.Token, TaskContinuationOptions.OnlyOnRanToCompletion, TaskScheduler.Current);

                        var getMarksTask = sClient.GetColumnMarksAsync();
                        var getMarksResTask = getMarksTask.ContinueWith(res =>
                        {
                            stopCancellationTokenSource.Token.ThrowIfCancellationRequested();
                            if (res.Result.Error != null)
                                throw new Exception(res.Result.Error);
                            return res.Result.Values;
                        }, stopCancellationTokenSource.Token, TaskContinuationOptions.OnlyOnRanToCompletion, TaskScheduler.Current);

                        var getColumnTypesTask = sClient.GetColumnTypesAsync();
                        var getColumnTypesResTask = getColumnTypesTask.ContinueWith(res =>
                        {
                            stopCancellationTokenSource.Token.ThrowIfCancellationRequested();
                            if (res.Result.Error != null)
                                throw new Exception(res.Result.Error);
                            return res.Result.Values;
                        }, stopCancellationTokenSource.Token, TaskContinuationOptions.OnlyOnRanToCompletion, TaskScheduler.Current);

                        Task.WaitAll(new Task[] { getAccountsTask, getMarksTask, getColumnTypesTask }, stopCancellationTokenSource.Token);
                        Task.WaitAll(new Task[] { getAccountsResTask, getMarksResTask, getColumnTypesResTask }, stopCancellationTokenSource.Token);

                        modelLevelThContext.DoCallBack(() => RaiseAccountInitialize(getAccountsResTask.Result.ToArray()));
                        modelLevelThContext.DoCallBack(() => RaiseMarkInitialize(getMarksResTask.Result.ToArray()));
                        modelLevelThContext.DoCallBack(() => RaiseColumnTypesInitialize(getColumnTypesResTask.Result.ToArray()));

                        inited = true;
                        SetError((string)null);
                    }
                }
                catch(ThreadAbortException ex)
                {
                    throw ex;
                }
                catch (Exception ex)
                {
                    SetError(ex);
                    Thread.Sleep(ConnectionTimeInterval);
                }
            } while (!inited);

            IsLoaded = true;

            #endregion
        }

        private void RaiseAccountInitialize(AccountService.Account[] accounts)
        {
            AccountService.Account[] oldAccounts = new AccountService.Account[] { };
            lock (Accounts)
            {
                oldAccounts = Accounts.ToArray();
                Accounts.Clear();
            }

            if (oldAccounts.Length > 0)
                OnAccountsChanged(this, new ListItemsEventArgs<AccountService.Account>(oldAccounts, ChangeAction.Remove));

            lock(Accounts)
            {
                Accounts.AddRange(accounts);
            }

            if (accounts.Length > 0)
                OnAccountsChanged(this, new ListItemsEventArgs<AccountService.Account>(accounts, ChangeAction.Add));
        }
        private void RaiseMarkInitialize(AccountService.Mark[] marks)
        {
            var oldMarks = new AccountService.Mark[] { };
            lock (Marks)
            {
                oldMarks = Marks.ToArray();
                Marks.Clear();
            }

            if (oldMarks.Length > 0)
                OnMarksChanged(this, ListItemsEventArgs.Create(oldMarks, ChangeAction.Remove));

            lock (Marks)
            {
                Marks.AddRange(marks);
            }

            if (marks.Length > 0)
                OnMarksChanged(this, ListItemsEventArgs.Create(marks, ChangeAction.Add));
        }
        private void RaiseColumnTypesInitialize(AccountService.ColumnType[] columnTypes)
        {
            var oldColumnTypes = new AccountService.ColumnType[] { };
            lock (ColumnTypes)
            {
                oldColumnTypes = ColumnTypes.ToArray();
                ColumnTypes.Clear();
            }

            if (oldColumnTypes.Length > 0)
                OnColumnTypesChanged(this, ListItemsEventArgs.Create(oldColumnTypes, ChangeAction.Remove));

            lock (ColumnTypes)
            {
                ColumnTypes.AddRange(columnTypes);
            }

            if (columnTypes.Length > 0)
                OnColumnTypesChanged(this, ListItemsEventArgs.Create(columnTypes, ChangeAction.Add));
        }

        public void ApplyHistoryChanges(HistoryService.History e)
        {
            if (!IsLoaded)
                return;

            ApplyHistoryChange(e?.Add);
            ApplyHistoryChange(e?.Change);
            ApplyHistoryRemove(e?.Remove);
        }

        #region Add and change

        private void ApplyHistoryChange(HistoryService.HistoryUpdatePart change)
        {
            if (change == null)
                return;

            ApplyHistoryUpdateAccount(change.Account);
            ApplyHistoryUpdateAccountSettings(change.AccountSettings);
            ApplyHistoryUpdateAccountSettingsColumn(change.AccountSettingsColumn);
            ApplyHistoryUpdateAccountSettingsExportDirectory(change.AccountSettingsExportDirectory);
            ApplyHistoryUpdateAccountSettingsImportDirectory(change.AccountSettingsImportDirectory);
            ApplyHistoryUpdateAccountSettingsSheduleTime(change.AccountSettingsSheduleTime);
        }

        private void ApplyHistoryUpdateAccount(IEnumerable<HistoryService.Account> items)
        {
            if (items == null || !items.Any()) return;

            var innerItems = items.Select(i => AutoMapper.Mapper.Map<AccountService.Account>(i));
            var itemsToUpdate = new List<AccountService.Account>();
            var itemsToInsert = new List<AccountService.Account>();

            lock (Accounts)
            {
                var upd = Accounts
                    .RightOuterJoin(innerItems, a => a.Id, a => a.Id, (Existed, New) => new { Existed, New })
                    .ToArray();

                foreach (var i in upd)
                {
                    if (i.Existed == null)
                    {
                        Accounts.Add(i.New);
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
                OnAccountsChanged(this, new ListItemsEventArgs<AccountService.Account>(itemsToUpdate.ToArray(), ChangeAction.Change));
            if (itemsToInsert.Count > 0)
                OnAccountsChanged(this, new ListItemsEventArgs<AccountService.Account>(itemsToInsert.ToArray(), ChangeAction.Add));
        }

        private void ApplyHistoryUpdateAccountSettings(IEnumerable<HistoryService.AccountSettings> items)
        {
            if (items == null || !items.Any()) return;

            var innerItems = items.Select(i => AutoMapper.Mapper.Map<AccountService.AccountSettings>(i));
            var itemsToUpdate = new List<AccountService.Account>();

            lock (Accounts)
            {
                var upd = Accounts
                    .Join(innerItems, a => a.Id, a => a.Id, (ExistedAccount, UpdateSettings) => new { ExistedAccount, UpdateSettings })
                    .ToArray();

                foreach (var i in upd)
                {
                    if (i.ExistedAccount.Settings == null)
                        i.ExistedAccount.Settings = i.UpdateSettings;
                    else
                        i.ExistedAccount.Settings.CopyObjectFrom(i.UpdateSettings);
                    itemsToUpdate.Add(i.ExistedAccount);
                }
            }

            if (itemsToUpdate.Count > 0)
                OnAccountsChanged(this, new ListItemsEventArgs<AccountService.Account>(itemsToUpdate.ToArray(), ChangeAction.Change));
        }

        private void ApplyHistoryUpdateAccountSettingsColumn(IEnumerable<HistoryService.AccountSettingsColumn> items)
        {
            if (items == null || !items.Any()) return;

            var updItems = items.Select(i => AutoMapper.Mapper.Map<AccountService.AccountSettingsColumn>(i));
            var itemsToUpdate = new List<AccountService.Account>();

            lock (Accounts)
            {
                var upd = updItems
                    .Join(Accounts, a => a.AccountUID, a => a.Id, (Update, Account) => new { Update, Account })
                    .Select(i => new
                    {
                        i.Update,
                        i.Account,
                        Existed = i.Account
                            .Settings
                            .Columns
                            .FirstOrDefault(c => c.Id == i.Update.Id)
                    })
                    .GroupBy(i => i.Account)
                    .Select(g => new { Account = g.Key, Columns = g.Select(i => new { i.Existed, i.Update }).ToArray() })
                    .ToArray();

                foreach (var a in upd)
                {
                    foreach (var c in a.Columns)
                    {
                        if (c.Existed == null)
                            a.Account.Settings.Columns.Add(c.Update);
                        else
                            c.Existed.CopyObjectFrom(c.Update);
                    }
                    itemsToUpdate.Add(a.Account);
                }
            }

            if (itemsToUpdate.Count > 0)
                OnAccountsChanged(this, new ListItemsEventArgs<AccountService.Account>(itemsToUpdate.ToArray(), ChangeAction.Change));
        }

        private void ApplyHistoryUpdateAccountSettingsExportDirectory(IEnumerable<HistoryService.AccountSettingsExportDirectory> items)
        {
            if (items == null || !items.Any()) return;

            var updItems = items.Select(i => AutoMapper.Mapper.Map<AccountService.AccountSettingsExportDirectory>(i));
            var itemsToUpdate = new List<AccountService.Account>();

            lock (Accounts)
            {
                var upd = updItems
                    .Join(Accounts, a => a.AccountUID, a => a.Id, (Update, Account) => new { Update, Account })
                    .Select(i => new
                    {
                        i.Update,
                        i.Account,
                        Existed = i.Account
                            .Settings
                            .ExportDirectories
                            .FirstOrDefault(c => c.Id == i.Update.Id)
                    })
                    .GroupBy(i => i.Account)
                    .Select(g => new { Account = g.Key, Directories = g.Select(i => new { i.Existed, i.Update }).ToArray() })
                    .ToArray();

                foreach (var a in upd)
                {
                    foreach (var c in a.Directories)
                    {
                        if (c.Existed == null)
                            a.Account.Settings.ExportDirectories.Add(c.Update);
                        else
                            c.Existed.CopyObjectFrom(c.Update);
                    }
                    itemsToUpdate.Add(a.Account);
                }
            }

            if (itemsToUpdate.Count > 0)
                OnAccountsChanged(this, new ListItemsEventArgs<AccountService.Account>(itemsToUpdate.ToArray(), ChangeAction.Change));
        }

        private void ApplyHistoryUpdateAccountSettingsImportDirectory(IEnumerable<HistoryService.AccountSettingsImportDirectory> items)
        {
            if (items == null || !items.Any()) return;

            var updItems = items.Select(i => AutoMapper.Mapper.Map<AccountService.AccountSettingsImportDirectory>(i));
            var itemsToUpdate = new List<AccountService.Account>();

            lock (Accounts)
            {
                var upd = updItems
                    .Join(Accounts, a => a.AccountUID, a => a.Id, (Update, Account) => new { Update, Account })
                    .Select(i => new
                    {
                        i.Update,
                        i.Account,
                        Existed = i.Account
                            .Settings
                            .ImportDirectories
                            .FirstOrDefault(c => c.Id == i.Update.Id)
                    })
                    .GroupBy(i => i.Account)
                    .Select(g => new { Account = g.Key, Directories = g.Select(i => new { i.Existed, i.Update }).ToArray() })
                    .ToArray();

                foreach (var a in upd)
                {
                    foreach (var c in a.Directories)
                    {
                        if (c.Existed == null)
                            a.Account.Settings.ImportDirectories.Add(c.Update);
                        else
                            c.Existed.CopyObjectFrom(c.Update);
                    }
                    itemsToUpdate.Add(a.Account);
                }
            }

            if (itemsToUpdate.Count > 0)
                OnAccountsChanged(this, new ListItemsEventArgs<AccountService.Account>(itemsToUpdate.ToArray(), ChangeAction.Change));
        }

        private void ApplyHistoryUpdateAccountSettingsSheduleTime(IEnumerable<HistoryService.AccountSettingsSheduleTime> items)
        {
            if (items == null || !items.Any()) return;

            var updItems = items.Select(i => AutoMapper.Mapper.Map<AccountService.AccountSettingsSheduleTime>(i));
            var itemsToUpdate = new List<AccountService.Account>();

            lock (Accounts)
            {
                var upd = updItems
                    .Join(Accounts, a => a.AccountUID, a => a.Id, (Update, Account) => new { Update, Account })
                    .Select(i => new
                    {
                        i.Update,
                        i.Account,
                        Existed = i.Account
                            .Settings
                            .SheduleTimes
                            .FirstOrDefault(c => c.Id == i.Update.Id)
                    })
                    .GroupBy(i => i.Account)
                    .Select(g => new { Account = g.Key, Directories = g.Select(i => new { i.Existed, i.Update }).ToArray() })
                    .ToArray();

                foreach (var a in upd)
                {
                    foreach (var c in a.Directories)
                    {
                        if (c.Existed == null)
                            a.Account.Settings.SheduleTimes.Add(c.Update);
                        else
                            c.Existed.CopyObjectFrom(c.Update);
                    }
                    itemsToUpdate.Add(a.Account);
                }
            }

            if (itemsToUpdate.Count > 0)
                OnAccountsChanged(this, new ListItemsEventArgs<AccountService.Account>(itemsToUpdate.ToArray(), ChangeAction.Change));
        }

        #endregion
        #region Remove

        private void ApplyHistoryRemove(HistoryService.HistoryRemovePart remove)
        {
            if (remove == null)
                return;

            ApplyHistoryRemoveAccount(remove.Account);
            ApplyHistoryRemoveAccountSettingsColumn(remove.AccountSettingsColumn);
            ApplyHistoryRemoveAccountSettingsExportDirectory(remove.AccountSettingsExportDirectory);
            ApplyHistoryRemoveAccountSettingsImportDirectory(remove.AccountSettingsImportDirectory);
            ApplyHistoryRemoveAccountSettingsSheduleTime(remove.AccountSettingsSheduleTime);
        }

        private void ApplyHistoryRemoveAccount(IList<Guid> ids)
        {
            if (ids == null) return;
            AccountService.Account[] itemsToDelete = new AccountService.Account[] { };
            lock (Accounts)
            {
                itemsToDelete = Accounts.Join(ids, a => a.Id, id => id, (a, id) => a).ToArray();
                foreach (var i in itemsToDelete)
                    Accounts.Remove(i);
            }
            if (itemsToDelete.Length > 0)
                OnAccountsChanged(this, new ListItemsEventArgs<AccountService.Account>(itemsToDelete, ChangeAction.Remove));
        }

        private void ApplyHistoryRemoveAccountSettingsColumn(IList<long> ids)
        {
            if (ids == null) return;
            AccountService.Account[] itemsToChange = new AccountService.Account[] { };
            lock (Accounts)
            {
                var itemsToDelete = Accounts
                    .SelectMany(a => a.Settings.Columns.Select(c => new { Account = a, Column = c }))
                    .Join(ids, a => a.Column.Id, id => id, (a, id) => a)
                    .GroupBy(i => i.Account)
                    .Select(g => new { Account = g.Key, Columns = g.Select(i => i.Column).ToArray() })
                    .ToArray();

                foreach (var i in itemsToDelete)
                    foreach (var c in i.Columns)
                        i.Account.Settings.Columns.Remove(c);

                itemsToChange = itemsToDelete.Select(i => i.Account).ToArray();
            }

            if (itemsToChange.Length > 0)
                OnAccountsChanged(this, new ListItemsEventArgs<AccountService.Account>(itemsToChange, ChangeAction.Change));
        }

        private void ApplyHistoryRemoveAccountSettingsExportDirectory(IList<long> ids)
        {
            if (ids == null) return;
            AccountService.Account[] itemsToChange = new AccountService.Account[] { };
            lock (Accounts)
            {
                var itemsToDelete = Accounts
                    .SelectMany(a => a.Settings.ExportDirectories.Select(c => new { Account = a, Directory = c }))
                    .Join(ids, a => a.Directory.Id, id => id, (a, id) => a)
                    .GroupBy(i => i.Account)
                    .Select(g => new { Account = g.Key, Directories = g.Select(i => i.Directory).ToArray() })
                    .ToArray();

                foreach (var i in itemsToDelete)
                    foreach (var c in i.Directories)
                        i.Account.Settings.ExportDirectories.Remove(c);

                itemsToChange = itemsToDelete.Select(i => i.Account).ToArray();
            }

            if (itemsToChange.Length > 0)
                OnAccountsChanged(this, new ListItemsEventArgs<AccountService.Account>(itemsToChange, ChangeAction.Change));
        }

        private void ApplyHistoryRemoveAccountSettingsImportDirectory(IList<long> ids)
        {
            if (ids == null) return;
            AccountService.Account[] itemsToChange = new AccountService.Account[] { };
            lock (Accounts)
            {
                var itemsToDelete = Accounts
                    .SelectMany(a => a.Settings.ImportDirectories.Select(c => new { Account = a, Directory = c }))
                    .Join(ids, a => a.Directory.Id, id => id, (a, id) => a)
                    .GroupBy(i => i.Account)
                    .Select(g => new { Account = g.Key, Directories = g.Select(i => i.Directory).ToArray() })
                    .ToArray();

                foreach (var i in itemsToDelete)
                    foreach (var c in i.Directories)
                        i.Account.Settings.ImportDirectories.Remove(c);

                itemsToChange = itemsToDelete.Select(i => i.Account).ToArray();
            }

            if (itemsToChange.Length > 0)
                OnAccountsChanged(this, new ListItemsEventArgs<AccountService.Account>(itemsToChange, ChangeAction.Change));
        }

        private void ApplyHistoryRemoveAccountSettingsSheduleTime(IList<long> ids)
        {
            if (ids == null) return;
            AccountService.Account[] itemsToChange = new AccountService.Account[] { };
            lock (Accounts)
            {
                var itemsToDelete = Accounts
                    .SelectMany(a => a.Settings.SheduleTimes.Select(c => new { Account = a, Time = c }))
                    .Join(ids, a => a.Time.Id, id => id, (a, id) => a)
                    .GroupBy(i => i.Account)
                    .Select(g => new { Account = g.Key, Times = g.Select(i => i.Time).ToArray() })
                    .ToArray();

                foreach (var i in itemsToDelete)
                    foreach (var c in i.Times)
                        i.Account.Settings.SheduleTimes.Remove(c);

                itemsToChange = itemsToDelete.Select(i => i.Account).ToArray();
            }

            if (itemsToChange.Length > 0)
                OnAccountsChanged(this, new ListItemsEventArgs<AccountService.Account>(itemsToChange, ChangeAction.Change));
        }

        #endregion

        public AccountService.Account[] GetAccounts()
        {
            lock(Accounts)
            {
                return Accounts.ToArray();
            }
        }
        public AccountService.Mark[] GetMarks()
        {
            lock (Marks)
            {
                return Marks.ToArray();
            }
        }

        public AccountService.ColumnType[] GetColumnTypes()
        {
            lock (ColumnTypes)
            {
                return ColumnTypes.ToArray();
            }
        }

        public event EventHandler<ListItemsEventArgs<AccountService.Account>> OnAccountsChanged;
        public event EventHandler<ListItemsEventArgs<AccountService.Mark>> OnMarksChanged;
        public event EventHandler<ListItemsEventArgs<AccountService.ColumnType>> OnColumnTypesChanged;
    }
}
