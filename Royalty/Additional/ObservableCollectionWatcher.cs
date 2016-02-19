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

        public event EventHandler<ListItemsEventArgs<T>> Change;

        public void AddRange(IEnumerable<T> items)
        {
            foreach (var i in items)
                this.Add(i);
        }

        public void UpdateCollection(ListItemsEventArgs<T> e)
        {
            if (e.Action == RoyaltyServiceWorker.Additional.ChangeAction.Change || e.Action == RoyaltyServiceWorker.Additional.ChangeAction.Add)
                e.Items.ToList().ForEach(i => 
                {
                    var item = this.FirstOrDefault(a => comparer(a, i));
                    if (item == null)
                        this.Add(i); else
                        item.CopyObjectFrom(i);
                });
            
            if (e.Action == RoyaltyServiceWorker.Additional.ChangeAction.Remove)
                e.Items.ToList().ForEach(i => 
                {
                    var item = this.FirstOrDefault(a => comparer(a, i));
                    if (item != null)
                        this.Remove(item);
                });

            Change?.Invoke(this, e);
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
