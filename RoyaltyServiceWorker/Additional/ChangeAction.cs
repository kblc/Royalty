using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoyaltyServiceWorker.Additional
{
    public enum ChangeAction
    {
        Add,
        Change,
        Remove
    }

    public class ListItemsEventArgs<T> : EventArgs
    {
        public readonly T[] Items;
        public readonly ChangeAction Action;

        public ListItemsEventArgs(T[] items, ChangeAction action)
        {
            Items = items;
            Action = action;
        }
    }

    public class ListItemsEventArgs
    {
        public static ListItemsEventArgs<T> Create<T>(T[] items, ChangeAction action)
        {
            return new ListItemsEventArgs<T>(items, action);
        }
    }
}
