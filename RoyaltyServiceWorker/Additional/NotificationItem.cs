using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoyaltyServiceWorker.Additional
{
    public class NotificationItem : NotifyPropertyChangedBase
    {
        private DateTime created;
        public DateTime Created { get { return created; } protected set { if (created == value) return; created = value; RaisePropertyChanged(); } }

        private string header;
        public string Header { get { return header; } protected set { if (header == value) return; header = value; RaisePropertyChanged(); } }

        private string message;
        public string Message { get { return message; } protected set { if (message == value) return; message = value; RaisePropertyChanged(); } }

        private bool isError;
        public bool IsError { get { return isError; } protected set { if (isError == value) return; isError = value; RaisePropertyChanged(); } }

        public NotificationItem(string header, string message, bool isError, DateTime created)
        {
            this.Header = header;
            this.Message = message;
            this.IsError = isError;
            this.Created = created;
        }
        public NotificationItem(string header, string message)
            : this(header, message, false, DateTime.UtcNow)
        { }
        public NotificationItem(string header, Exception ex)
            : this(header, ex.ToString(), true, DateTime.UtcNow)
        { }
    }

}
