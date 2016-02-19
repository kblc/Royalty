using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Royalty.Additional
{
    public interface IHistoryItemsChange<T>
    {
        event EventHandler<RoyaltyServiceWorker.Additional.ListItemsEventArgs<T>> Change;
    }
}
