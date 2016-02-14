using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoyaltyServiceWorker.AccountService
{
    public partial class AccountSeriesOfNumbersRecord
    {
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private bool IsBusyField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string ErrorField;

        [global::System.ComponentModel.BrowsableAttribute(false)]
        public bool IsBusy
        {
            get { return this.IsBusyField; }
            set 
            {
                if ((this.IsBusyField.Equals(value) != true))
                {
                    this.IsBusyField = value;
                    this.RaisePropertyChanged(nameof(IsBusy));
                }
            }
        }

        [global::System.ComponentModel.BrowsableAttribute(false)]
        public string Error
        {
            get { return this.ErrorField; }
            set
            {
                if ((this.ErrorField.Equals(value) != true))
                {
                    this.ErrorField = value;
                    this.RaisePropertyChanged(nameof(Error));
                }
            }
        }
    }
}
