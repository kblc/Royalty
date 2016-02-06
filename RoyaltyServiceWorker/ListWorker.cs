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
    public abstract class ListWorker<T, THistoryKey> : AbstractBaseWorker
    {
        private Thread initThread = null;
        protected CancellationTokenSource stopCancellationTokenSource = null;
        protected readonly List<T> Items = new List<T>();
        private readonly Func<HistoryService.HistoryUpdatePart, IEnumerable<T>> historyUpdationSelector;
        private readonly Func<HistoryService.HistoryRemovePart, IEnumerable<THistoryKey>> historyDeletionSelector;

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

        public ListWorker(Func<HistoryService.HistoryUpdatePart, IEnumerable<T>> historyUpdationSelector, Func<HistoryService.HistoryRemovePart, IEnumerable<THistoryKey>> historyDeletionSelector)
        {
            this.historyUpdationSelector = historyUpdationSelector;
            this.historyDeletionSelector = historyDeletionSelector;
        }

        protected abstract T[] ServiceGetData();

        private void InitThread(object context)
        {
            var modelLevelThContext = (System.Runtime.Remoting.Contexts.Context)context;
            bool inited = false;

            #region Infinity try to connect then init and exit
            do
            {
                try
                {
                    var result = ServiceGetData();
                    modelLevelThContext.DoCallBack(() => RaiseInitialize(result));
                    inited = true;
                    SetError((string)null);
                }
                catch (ThreadAbortException ex)
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

        private void RaiseInitialize(T[] items)
        {
            var oldItems = new T[] { };
            lock (Items)
            {
                oldItems = Items.ToArray();
                Items.Clear();
            }

            if (oldItems.Length > 0)
                OnItemsChanged(this, ListItemsEventArgs.Create(oldItems, ChangeAction.Remove));

            lock (Items)
            {
                Items.AddRange(items);
            }

            if (items.Length > 0)
                OnItemsChanged(this, ListItemsEventArgs.Create(items, ChangeAction.Add));
        }

        public void ApplyHistoryChanges(HistoryService.History e)
        {
            if (!IsLoaded)
                return;

            if (historyUpdationSelector != null)
            {
                if (e.Add != null)
                    ApplyHistoryChange(historyUpdationSelector(e.Add));
                if (e.Change != null)
                    ApplyHistoryChange(historyUpdationSelector(e.Change));
            }
            if (historyDeletionSelector != null)
            {
                if (e.Remove != null)
                    ApplyHistoryRemove(historyDeletionSelector(e.Remove));
            }
        }

        protected abstract void ApplyHistoryChange(IEnumerable<T> items);
        protected abstract void ApplyHistoryRemove(IEnumerable<THistoryKey> keys);

        public T[] GetItems()
        {
            lock (Items)
            {
                return Items.ToArray();
            }
        }

        protected void RaiseOnItemsChanged(T[] items, ChangeAction action)
         => OnItemsChanged?.Invoke(this, ListItemsEventArgs.Create(items, action));

        public event EventHandler<ListItemsEventArgs<T>> OnItemsChanged;
    }
}
