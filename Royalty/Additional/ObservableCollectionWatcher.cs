using Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using RoyaltyServiceWorker.Additional;

namespace Royalty.Additional
{
    public class ObservableCollectionWatcher<T> : ObservableCollection<T>, IHistoryItemsChange<T>
    {
        private readonly Func<T, T, bool> comparer;

        public ObservableCollectionWatcher(Func<T,T,bool> comparer)
        {
            if (comparer == null)
                throw new ArgumentNullException(nameof(comparer));
            this.comparer = comparer;
        }

        private long CollectionUpdatingCounter = 0;
        public bool IsCollectionUpdating { get { return CollectionUpdatingCounter > 0; } }

        public event EventHandler<ListItemsEventArgs<T>> Change;

        public void AddRange(IEnumerable<T> items)
        {
            foreach (var i in items)
                this.Add(i);
        }

        public void UpdateCollection(ListItemsEventArgs<T> e)
        {
            CollectionUpdatingCounter++;
            try
            {
                if (e.Action == RoyaltyServiceWorker.Additional.ChangeAction.Change || e.Action == RoyaltyServiceWorker.Additional.ChangeAction.Add)
                    UpdateCollectionAddOrUpdate(e.Items);

                if (e.Action == RoyaltyServiceWorker.Additional.ChangeAction.Remove)
                    UpdateCollectionRemove(e.Items);
            }
            finally
            {
                CollectionUpdatingCounter--;
            }
        }

        public void UpdateCollectionAddOrUpdate(T[] items)
        {
            CollectionUpdatingCounter++;
            try
            {
                var itemsToUpdate = new List<T> { };
                var itemsToAdd = new List<T> { };
                items.ToList().ForEach(i =>
                {
                    var item = this.FirstOrDefault(a => comparer(a, i));
                    if (item == null)
                    {
                        this.Add(i);
                        itemsToAdd.Add(i);
                    }
                    else {
                        item.CopyObjectFrom(i);
                        itemsToUpdate.Add(i);
                    }
                });
                if (itemsToAdd.Count > 0)
                    Change?.Invoke(this, new ListItemsEventArgs<T>(itemsToAdd.ToArray(), ChangeAction.Add));
                if (itemsToUpdate.Count > 0)
                    Change?.Invoke(this, new ListItemsEventArgs<T>(itemsToUpdate.ToArray(), ChangeAction.Change));
            }
            finally
            {
                CollectionUpdatingCounter--;
            }
        }

        public void UpdateCollectionRemove(T[] items)
        {
            CollectionUpdatingCounter++;
            try
            {
                items.ToList().ForEach(i =>
                {
                    var item = this.FirstOrDefault(a => comparer(a, i));
                    if (item != null)
                        this.Remove(item);
                });
                var args = new ListItemsEventArgs<T>(items, ChangeAction.Remove);
                Change?.Invoke(this, args);
            }
            finally
            {
                CollectionUpdatingCounter--;
            }
        }
    }

    public static class ObservableCollectionWatcher
    {
        public static ObservableCollectionWatcher<T> ObservableCollectionWatcherFromComparer<T>(IComparer<T> comparer)
        {
            if (comparer == null)
                throw new ArgumentNullException(nameof(comparer));
            return new ObservableCollectionWatcher<T>(new Func<T, T, bool>((i1, i2) => comparer.Compare(i1, i2) == 0));
        }
    }
}
