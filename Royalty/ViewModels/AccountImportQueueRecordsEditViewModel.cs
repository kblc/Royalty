using Helpers;
using Helpers.WPF;
using Royalty.Additional;
using Royalty.Components;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using RoyaltyServiceWorker.AccountService;
using RoyaltyServiceWorker.Additional;
using System.IO;
using Royalty.ViewModels.Additional;

namespace Royalty.ViewModels
{
    public class DateTimeFilter
    {
        public readonly DateTime? From;
        public readonly DateTime? To;
        public DateTimeFilter(DateTime? from, DateTime? to)
        {
            From = from;
            To = to;
        }
    }

    public class AccountImportQueueRecordsEditViewModel : AbstractActionWithBackViewModel, IDisposable
    {
        #region Account

        public static readonly DependencyProperty AccountProperty = DependencyProperty.Register(nameof(Account), typeof(RoyaltyServiceWorker.AccountService.Account),
            typeof(AccountImportQueueRecordsEditViewModel), new PropertyMetadata(null, (s, e) => { (s as AccountImportQueueRecordsEditViewModel)?.OnAccountChanged(e.NewValue as RoyaltyServiceWorker.AccountService.Account, e.OldValue as RoyaltyServiceWorker.AccountService.Account); }));

        public RoyaltyServiceWorker.AccountService.Account Account
        {
            get { return (RoyaltyServiceWorker.AccountService.Account)GetValue(AccountProperty); }
            set { SetValue(AccountProperty, value); }
        }

        #endregion
        #region States

        public static readonly DependencyProperty StatesProperty = DependencyProperty.Register(nameof(States), typeof(ICollectionView),
            typeof(AccountImportQueueRecordsEditViewModel), new PropertyMetadata(null, (s, e) => { }));

        public ICollectionView States
        {
            get { return (ICollectionView)GetValue(StatesProperty); }
            set { SetValue(StatesProperty, value); }
        }

        #endregion

        #region AccountsImportQueueRecordsComponent

        public static readonly DependencyProperty AccountsImportQueueRecordsComponentProperty = DependencyProperty.Register(nameof(AccountsImportQueueRecordsComponent), typeof(AccountsImportQueueRecordsComponent),
            typeof(AccountImportQueueRecordsEditViewModel), new PropertyMetadata(null, (s, e) =>
            {
                var model = s as AccountImportQueueRecordsEditViewModel;
                var newAccountsComponent = e.NewValue as AccountsImportQueueRecordsComponent;
                var oldAccountsComponent = e.OldValue as AccountsImportQueueRecordsComponent;
                if (model != null && newAccountsComponent != oldAccountsComponent)
                {
                    model.UpdateAccountsImportQueueRecordsComponentSource(newAccountsComponent, oldAccountsComponent);
                }
            }));

        public AccountsImportQueueRecordsComponent AccountsImportQueueRecordsComponent
        {
            get { return (AccountsImportQueueRecordsComponent)GetValue(AccountsImportQueueRecordsComponentProperty); }
            set { SetValue(AccountsImportQueueRecordsComponentProperty, value); }
        }

        #endregion
        #region ImportQueueRecords

        private static readonly DependencyPropertyKey ImportQueueRecordsPropertyKey
            = DependencyProperty.RegisterReadOnly(nameof(ImportQueueRecords), typeof(ICollectionView), typeof(AccountImportQueueRecordsEditViewModel),
                new FrameworkPropertyMetadata(null,
                    FrameworkPropertyMetadataOptions.None,
                    new PropertyChangedCallback((s, e) => { })));
        public static readonly DependencyProperty ReadOnlyImportQueueRecordsPropertyKey = ImportQueueRecordsPropertyKey.DependencyProperty;

        public ICollectionView ImportQueueRecords
        {
            get { return (ICollectionView)GetValue(ReadOnlyImportQueueRecordsPropertyKey); }
            private set { SetValue(ImportQueueRecordsPropertyKey, value); }
        }

        #endregion

        #region InsertCommand

        private static readonly DependencyPropertyKey InsertCommandPropertyKey
            = DependencyProperty.RegisterReadOnly(nameof(InsertCommand), typeof(DelegateCommand), typeof(AccountImportQueueRecordsEditViewModel),
                new FrameworkPropertyMetadata(null,
                    FrameworkPropertyMetadataOptions.None,
                    new PropertyChangedCallback((s, e) => { })));
        public static readonly DependencyProperty ReadOnlyInsertFilesCommandPropertyKey = InsertCommandPropertyKey.DependencyProperty;

        public DelegateCommand InsertCommand
        {
            get { return (DelegateCommand)GetValue(ReadOnlyInsertFilesCommandPropertyKey); }
            private set { SetValue(InsertCommandPropertyKey, value); }
        }

        #endregion
        #region DeleteCommand

        private static readonly DependencyPropertyKey DeleteCommandPropertyKey
            = DependencyProperty.RegisterReadOnly(nameof(DeleteCommand), typeof(DelegateCommand), typeof(AccountImportQueueRecordsEditViewModel),
                new FrameworkPropertyMetadata(null,
                    FrameworkPropertyMetadataOptions.None,
                    new PropertyChangedCallback((s, e) => { })));
        public static readonly DependencyProperty ReadOnlyDeleteCommandPropertyKey = DeleteCommandPropertyKey.DependencyProperty;

        public DelegateCommand DeleteCommand
        {
            get { return (DelegateCommand)GetValue(ReadOnlyDeleteCommandPropertyKey); }
            private set { SetValue(DeleteCommandPropertyKey, value); }
        }

        #endregion

        #region Encodings

        private static readonly DependencyPropertyKey EncodingsPropertyKey
            = DependencyProperty.RegisterReadOnly(nameof(Encodings), typeof(ICollectionView), typeof(AccountImportQueueRecordsEditViewModel),
                new FrameworkPropertyMetadata(null,
                    FrameworkPropertyMetadataOptions.None,
                    new PropertyChangedCallback((s, e) => { })));
        public static readonly DependencyProperty ReadOnlyEncodingsPropertyKey = EncodingsPropertyKey.DependencyProperty;

        public ICollectionView Encodings
        {
            get { return (ICollectionView)GetValue(ReadOnlyEncodingsPropertyKey); }
            private set { SetValue(EncodingsPropertyKey, value); }
        }

        #endregion
        #region LoadTypes

        private static readonly DependencyPropertyKey LoadTypesPropertyKey
            = DependencyProperty.RegisterReadOnly(nameof(LoadTypes), typeof(ICollectionView), typeof(AccountImportQueueRecordsEditViewModel),
                new FrameworkPropertyMetadata(null,
                    FrameworkPropertyMetadataOptions.None,
                    new PropertyChangedCallback((s, e) => { })));
        public static readonly DependencyProperty ReadOnlyLoadTypesPropertyKey = LoadTypesPropertyKey.DependencyProperty;

        public ICollectionView LoadTypes
        {
            get { return (ICollectionView)GetValue(ReadOnlyLoadTypesPropertyKey); }
            private set { SetValue(LoadTypesPropertyKey, value); }
        }

        #endregion

        #region SelectedEncoding

        public static readonly DependencyProperty SelectedEncodingProperty = DependencyProperty.Register(nameof(SelectedEncoding), typeof(object),
            typeof(AccountImportQueueRecordsEditViewModel), new PropertyMetadata(null, (s, e) => { }));

        public object SelectedEncoding
        {
            get { return GetValue(SelectedEncodingProperty); }
            set { SetValue(SelectedEncodingProperty, value); }
        }

        #endregion
        #region SelectedLoadType

        public static readonly DependencyProperty SelectedLoadTypeProperty = DependencyProperty.Register(nameof(SelectedLoadType), typeof(object),
            typeof(AccountImportQueueRecordsEditViewModel), new PropertyMetadata(null, (s, e) => { }));

        public object SelectedLoadType
        {
            get { return GetValue(SelectedLoadTypeProperty); }
            set { SetValue(SelectedLoadTypeProperty, value); }
        }

        #endregion

        #region From

        public static readonly DependencyProperty FromProperty = DependencyProperty.Register(nameof(From), typeof(DateTime?),
            typeof(AccountImportQueueRecordsEditViewModel), new PropertyMetadata(null, (s, e) => { (s as AccountImportQueueRecordsEditViewModel)?.OnFromChanged((DateTime?)e.NewValue, (DateTime?)e.OldValue); }));

        public DateTime? From
        {
            get { return (DateTime?)GetValue(FromProperty); }
            set { SetValue(FromProperty, value); }
        }

        #endregion
        #region To

        public static readonly DependencyProperty ToProperty = DependencyProperty.Register(nameof(To), typeof(DateTime?),
            typeof(AccountImportQueueRecordsEditViewModel), new PropertyMetadata(null, (s, e) => { (s as AccountImportQueueRecordsEditViewModel)?.OnToChanged((DateTime?)e.NewValue, (DateTime?)e.OldValue); }));

        public DateTime? To
        {
            get { return (DateTime?)GetValue(ToProperty); }
            set { SetValue(ToProperty, value); }
        }

        #endregion

        protected override void OnIsActiveChanged(bool newValue)
        {
            base.OnIsActiveChanged(newValue);
            localCollection.Clear();
        }

        private FilterTimer<DateTimeFilter> setFilterTimer;

        protected virtual void OnFromChanged(DateTime? newFilter, DateTime? oldFilter)
        {
            if (setFilterTimer != null)
                setFilterTimer.Filter = new DateTimeFilter(newFilter, To);
        }

        protected virtual void OnToChanged(DateTime? newFilter, DateTime? oldFilter)
        {
            if (setFilterTimer != null)
                setFilterTimer.Filter = new DateTimeFilter(From, newFilter);
        }

        private ObservableCollectionWatcher<RoyaltyServiceWorker.AccountService.ImportQueueRecord> localCollection = null;
        public ObservableCollection<Encoding> encodings = null;
        public ObservableCollection<LoadType> loadTypes = null;

        public AccountImportQueueRecordsEditViewModel()
        {
            encodings = new ObservableCollection<Encoding>(new[] { Encoding.UTF8, Encoding.ASCII, Encoding.GetEncoding(1252), Encoding.Unicode });
            Encodings = CollectionViewSource.GetDefaultView(encodings);
            Encodings.SortDescriptions.Add(new SortDescription(nameof(Encoding.WebName), ListSortDirection.Ascending));
            SelectedEncoding = encodings.FirstOrDefault();

            loadTypes = new ObservableCollection<LoadType>(new[] { new LoadType("Для анализа", true), new LoadType("Обычная загрузка", false) });
            LoadTypes = CollectionViewSource.GetDefaultView(loadTypes);
            SelectedLoadType = loadTypes.FirstOrDefault();

            localCollection = new ObservableCollectionWatcher<ImportQueueRecord>((x, y) => x.Id == y.Id);
            ImportQueueRecords = CollectionViewSource.GetDefaultView(localCollection);
            //ImportQueueRecords.CollectionChanged += ImportQueueRecords_CollectionChanged;
            ImportQueueRecords.SortDescriptions.Add(new SortDescription(nameof(ImportQueueRecord.CreatedDate), ListSortDirection.Descending));
            setFilterTimer = new FilterTimer<DateTimeFilter>(TimeSpan.FromMilliseconds(500), (filter) =>
            {
                RunUnderDispatcher(() =>
                {
                    if (this.AccountsImportQueueRecordsComponent != null)
                    {
                        localCollection.Clear();
                        AccountsImportQueueRecordsComponent.From = filter.From;
                        AccountsImportQueueRecordsComponent.To = filter.To;
                    }
                });
            });
            InsertCommand = new DelegateCommand((o) => InsertNew());
            DeleteCommand = new DelegateCommand((o) => DeleteFromCollection(o as ImportQueueRecord));
        }

        private void OnAccountChanged(RoyaltyServiceWorker.AccountService.Account newItem, RoyaltyServiceWorker.AccountService.Account oldItem)
        {
            localCollection.Clear();
            From = null;
            To = null;
        }

        private void UpdateAccountsImportQueueRecordsComponentSource(AccountsImportQueueRecordsComponent newItem, AccountsImportQueueRecordsComponent oldItem)
        {
            if (oldItem != null)
            {
                oldItem.Change -= AccountsImportQueueRecordsComponent_Change;
                BindingOperations.ClearBinding(oldItem, AccountsImportQueueRecordsComponent.AccountProperty);
                BindingOperations.ClearBinding(oldItem, AbstractComponent.IsActiveProperty);
                DependencyPropertyDescriptor.FromProperty(AbstractComponent.ReadOnlyIsLoadedProperty, oldItem.GetType())
                    .RemoveValueChanged(oldItem, AccountsImportQueueRecordsComponent_IsLoadedChanged);
            }

            localCollection.Clear();

            if (newItem != null)
            {
                localCollection.AddRange(newItem.ImportQueueRecords);
                newItem.Change += AccountsImportQueueRecordsComponent_Change;

                var accountBinding = new Binding() {
                    Source = this,
                    Path = new PropertyPath(AccountProperty.Name),
                    Mode = BindingMode.OneWay,
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged };

                var isActiveBinding = new Binding() {
                    Source = this,
                    Path = new PropertyPath(IsActiveProperty.Name),
                    Mode = BindingMode.OneWay,
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged };

                BindingOperations.SetBinding(newItem, AccountsImportQueueRecordsComponent.AccountProperty, accountBinding);
                BindingOperations.SetBinding(newItem, AbstractComponent.IsActiveProperty, isActiveBinding);
                DependencyPropertyDescriptor.FromProperty(AbstractComponent.ReadOnlyIsLoadedProperty, newItem.GetType())
                    .AddValueChanged(newItem, AccountsImportQueueRecordsComponent_IsLoadedChanged);

                this.IsBusy = !AccountsImportQueueRecordsComponent.IsLoaded;
            }
        }

        private void AccountsImportQueueRecordsComponent_Change(object sender, ListItemsEventArgs<ImportQueueRecord> e)
            => localCollection.UpdateCollection(e);

        private void AccountsImportQueueRecordsComponent_IsLoadedChanged(object sender, EventArgs e)
        {
            this.IsBusy = !AccountsImportQueueRecordsComponent.IsLoaded;
        }

        //private void ImportQueueRecords_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        //{
        //    if (e.OldItems != null && IsActive && !localCollection.IsCollectionUpdating)
        //    {
        //        var oldItems = e.OldItems
        //            .OfType<ImportQueueRecord>()
        //            .Where(i => i.Id != default(Guid))
        //            .ToArray();
        //        if (oldItems.Length > 0)
        //        {
        //            DeleteTask(oldItems)
        //                .ContinueWith(res =>
        //                {
        //                    if (res.Result.Length > 0)
        //                        localCollection.AddRange(res.Result);
        //                }, GetCancellationToken(), TaskContinuationOptions.AttachedToParent, TaskScheduler.FromCurrentSynchronizationContext());
        //        }
        //    }
        //    #region Set account UID for new Items
        //    if (e.NewItems != null && this.Account != null)
        //    {
        //        var newItems = e.NewItems
        //            .OfType<ImportQueueRecord>()
        //            .Where(i => i.Id == default(Guid))
        //            .ToArray();
        //        foreach (var i in newItems)
        //            i.AccountUID = this.Account.Id;
        //    }
        //    #endregion
        //}

        private void InsertNew()
        {
            var ofd = new Microsoft.Win32.OpenFileDialog {
                CheckFileExists = true,
                CheckPathExists = true,
                Filter = "CSV-файлы|*.csv|Все файлы|*.*",
                Multiselect = true
            };
            if (ofd.ShowDialog() == true)
            {
                IsBusy = true;

                var forAnalize = ((LoadType)SelectedLoadType).ForAnalize;
                var encoding = (Encoding)SelectedEncoding;
                InsertNew(ofd.FileNames, forAnalize, encoding);
            }
        }

        private void InsertNew(string[] filePath, bool forAnalize, Encoding encoding)
        {
            IsBusy = true;
            var accountId = Account.Id;
            var cancellationToken = GetCancellationToken();
            var fullLoadTask = Task.Factory.StartNew(() => {
                var storageClient = new RoyaltyServiceWorker.StorageService.FileServiceClient();
                var accountClient = new RoyaltyServiceWorker.AccountService.AccountServiceClient();

                var loadFileTasks = storageClient.UploadFiles(filePath, encoding, cancellationToken);
                //wait upload all files
                Task.WaitAll(loadFileTasks);
                var loadFileResult = loadFileTasks.Select(t => t.Result).ToArray();

                var newRecord = new ImportQueueRecord() {
                    AccountUID = accountId,
                    FileInfoes = loadFileResult.Select(f =>
                    new ImportQueueRecordFileInfo()
                    {
                        ForAnalize = forAnalize,
                        SourceFilePath = f.FileName,
                        Files = (new[] { new ImportQueueRecordFileInfoFile() { FileUID = f.Id } }).ToList()
                    }
                    ).ToList()
                };

                var res = accountClient.PutImportQueueRecord(newRecord);
                if (res.Error != null)
                    throw new Exception(res.Error);

                newRecord.CopyObjectFrom(res.Value);
                return newRecord;
            }, GetCancellationToken(), TaskCreationOptions.None, TaskScheduler.Default)
            .ContinueWith(r => {
                IsBusy = false;
                if (r.Exception != null)
                    Error = r.Exception.ToString();
                localCollection.UpdateCollectionAddOrUpdate(new[] { r.Result });
            }, System.Threading.CancellationToken.None, TaskContinuationOptions.AttachedToParent, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void DeleteFromCollection(ImportQueueRecord item)
        {
            if (item == null)
                return;

            var task = DeleteTask(new[] { item });
            task.ContinueWith((t) => {

                if (t.Exception != null)
                    return;

                if (t.Result.Length > 0)
                    return;

                localCollection.Remove(item);
            }, GetCancellationToken(), TaskContinuationOptions.AttachedToParent, TaskScheduler.FromCurrentSynchronizationContext());

        }

        //private Task<bool> SaveTask(ImportQueueRecord item)
        //{
        //    var startTask = Task.Factory.StartNew(() => 
        //    {
        //        IsBusy = true;
        //    }, GetCancellationToken(), TaskCreationOptions.None, TaskScheduler.FromCurrentSynchronizationContext());

        //    var clientTask = startTask.ContinueWith((t) => 
        //    {
        //        var client = new RoyaltyServiceWorker.AccountService.AccountServiceClient();
        //        try
        //        {
        //            return item.Id == default(Guid)
        //                ? client.PutImportQueueRecord(item)
        //                : client.UpdateImportQueueRecord(item);
        //        }
        //        finally
        //        {
        //            try { client.Close(); } catch { }
        //        }
        //    }, GetCancellationToken(), TaskContinuationOptions.AttachedToParent, TaskScheduler.Default);

        //    var taskRes = clientTask.ContinueWith((res) => 
        //    {
        //        try
        //        {
        //            if (res.Result.Error != null)
        //                throw new Exception(res.Result.Error);

        //            item.CopyObjectFrom(res.Result.Value);
                    
        //            return true;
        //        }
        //        catch(Exception ex)
        //        {
        //            Error = ex.ToString();
        //            return false;
        //        }
        //        finally
        //        {
        //            IsBusy = false;
        //        }
        //    }, GetCancellationToken(), TaskContinuationOptions.AttachedToParent, TaskScheduler.FromCurrentSynchronizationContext());

        //    return taskRes;
        //}

        private Task<ImportQueueRecord[]> DeleteTask(ImportQueueRecord[] items)
        {
            var startTask = Task.Factory.StartNew(() =>
            {
                IsBusy = true;
            }, GetCancellationToken(), TaskCreationOptions.None, TaskScheduler.FromCurrentSynchronizationContext());

            var clientTask = startTask.ContinueWith((t) =>
            {
                var client = new RoyaltyServiceWorker.AccountService.AccountServiceClient();
                try
                {
                    return client.RemoveImportQueueRecordRange(items.Select(i => i.Id).ToList());
                }
                finally
                {
                    try { client.Close(); } catch { }
                }
            }, GetCancellationToken(), TaskContinuationOptions.AttachedToParent, TaskScheduler.Default);

            var taskRes = clientTask.ContinueWith((res) =>
            {
                try
                {
                    if (res.Result.Error != null)
                        throw new Exception(res.Result.Error);

                    return items
                        .Where(i => !res.Result.Values.Contains(i.Id))
                        .ToArray();
                }
                catch (Exception ex)
                {
                    Error = ex.ToString();
                    return items;
                }
                finally
                {
                    IsBusy = false;
                }
            }, GetCancellationToken(), TaskContinuationOptions.AttachedToParent, TaskScheduler.FromCurrentSynchronizationContext());

            return taskRes;
        }

        protected override void RaiseCommands()
        {
            base.RaiseCommands();
            InsertCommand.RaiseCanExecuteChanged();
            DeleteCommand.RaiseCanExecuteChanged();
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (localCollection != null)
                        localCollection.Clear();
                }

                if (setFilterTimer != null)
                {
                    setFilterTimer.Dispose();
                    setFilterTimer = null;
                }

                disposedValue = true;
            }
        }

        ~AccountImportQueueRecordsEditViewModel()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
