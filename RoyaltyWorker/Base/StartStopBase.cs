using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RoyaltyWorker.Base
{
    public abstract class StartStopBase<T, TRunOncePrm> : IDisposable
        where TRunOncePrm : class
    {
        private TimeSpan timerInterval = default(TimeSpan);
        private Timer timer = null;
        private Thread runedThread = null;
        private object lockObject = new object();

        /// <summary>
        /// Create new instance
        /// </summary>
        /// <param name="timerInterval">Timer interval</param>
        public StartStopBase(TimeSpan timerInterval) : this(false, timerInterval) { }

        /// <summary>
        /// Create new instance
        /// </summary>
        /// <param name="createStarted">Start after create</param>
        /// <param name="timerInterval">Timer interval</param>
        public StartStopBase(bool createStarted, TimeSpan timerInterval)
        {
            this.TimerInterval = timerInterval;
            if (createStarted)
                Start();
        }

        /// <summary>
        /// Interval between calculations
        /// </summary>
        public TimeSpan TimerInterval
        {
            get { return timerInterval; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));

                if (timerInterval == value)
                    return;

                timer?.Change(TimeSpan.FromTicks(0), timerInterval);
                timerInterval = value;
            }
        }

        /// <summary>
        /// Is thread busy
        /// </summary>
        public bool IsBusy { get { lock(lockObject) return (runedThread != null && runedThread.IsAlive); } }

        /// <summary>
        /// Is runned
        /// </summary>
        public bool IsRunned { get { return (timer != null); } }

        protected abstract T GetStartParameters();
        protected abstract T GetRunOnceParameters(TRunOncePrm parameters);
        protected abstract void ThreadProc(T parameter);

        /// <summary>
        /// Start
        /// </summary>
        public virtual void Start()
        {
            if (timer != null)
                return;
            timer = new Timer(TimerCallback, GetStartParameters(), TimeSpan.FromTicks(0), TimerInterval);
        }

        /// <summary>
        /// Stop
        /// </summary>
        public virtual void Stop()
        {
            if (timer != null)
            {
                timer.Dispose();
                timer = null;
            }
            lock (lockObject)
            {
                if (runedThread != null && runedThread.IsAlive)
                    runedThread.Abort();
                runedThread = null;
            }
        }

        /// <summary>
        /// Run once without parameters
        /// </summary>
        public void RunOnce()
        {
            RunOnce(null, false);
        }

        /// <summary>
        /// Run once async without parameters
        /// </summary>
        /// <returns></returns>
        public Thread RunOnceAsync()
        {
            return RunOnce(null, true);
        }

        /// <summary>
        /// Run once with parameter
        /// </summary>
        /// <param name="parameter">Parameter to run</param>
        public void RunOnce(TRunOncePrm parameter)
        {
            RunOnce(parameter, false);
        }

        /// <summary>
        /// Run once async with parameters
        /// </summary>
        /// <param name="parameter">Parameter to run</param>
        /// <returns>Runned thread</returns>
        public Thread RunOnceAsync(TRunOncePrm parameter)
        {
            return RunOnce(parameter, true);
        }

        /// <summary>
        /// Run once with parameters
        /// </summary>
        /// <param name="parameter">Parameter to run</param>
        /// <param name="isAsync">Is this run async</param>
        /// <returns>Runned thread</returns>
        private Thread RunOnce(TRunOncePrm parameter, bool isAsync)
        {
            if (timer != null || IsBusy)
                throw new Exception(Properties.Resources.STARTSTOPBASE_StopBeforeRun);

            lock(lockObject)
                runedThread = StartThread(GetRunOnceParameters(parameter));
            if (!isAsync)
                runedThread.Join();
            return runedThread;
        }

        /// <summary>
        /// Timer callback
        /// </summary>
        /// <param name="state"></param>
        private void TimerCallback(object state)
        {
            lock (lockObject)
            {
                if (runedThread != null && runedThread.IsAlive)
                    return;
                runedThread = StartThread((T)state);
            }
        }

        /// <summary>
        /// Start new thread
        /// </summary>
        /// <param name="parameters">Parameter to start</param>
        /// <returns>Runed thread</returns>
        private Thread StartThread(T parameters)
        {
            var res = new Thread(new ParameterizedThreadStart( (prm) => ThreadProc((T)prm) ));
            res.Start(parameters);
            return res;
        }

        #region IDisposable Support
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Stop();
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}
