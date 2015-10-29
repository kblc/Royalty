using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoyaltyWorker.Model
{
    public enum WorkerProcessElementAction
    {
        Start = 0,
        Update,
        End
    }

    public class WorkerProcessFileProgress
    {
        public Guid ImportQueueRecordFileUID { get; private set; }
        public decimal Progress { get; private set; }

        public WorkerProcessFileProgress(Guid importQueueRecordFileUID, decimal progress)
        {
            ImportQueueRecordFileUID = importQueueRecordFileUID;
            Progress = progress;
        }
    }

    public class WorkerProcessElement
    {
        public Guid ImportQueueRecordUID { get; private set; }
        public decimal Progress { get; private set; }
        public IEnumerable<WorkerProcessFileProgress> WorkerProcessFileProgress { get; private set; }

        public WorkerProcessElement(Guid importQueueRecordUID, decimal progress, IEnumerable<WorkerProcessFileProgress> workerProcessFileProgress)
        {
            ImportQueueRecordUID = importQueueRecordUID;
            Progress = progress;
            WorkerProcessFileProgress = workerProcessFileProgress;
        }
    }

    public class WorkerProcessElementEventArgs : EventArgs
    {
        public WorkerProcessElementAction Action { get; private set; }
        public WorkerProcessElement Element { get; private set; }

        public WorkerProcessElementEventArgs(WorkerProcessElement element, WorkerProcessElementAction action)
        {
            Action = action;
            Element = element;
        }
    }
}
