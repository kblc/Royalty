using RoyaltyFileStorage;
using RoyaltyRepository;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Helpers;

namespace RoyaltyWorker.Base
{
    public abstract class RoyaltyBase<T, TRunOncePrm> : StartStopBase<T, TRunOncePrm>
        where TRunOncePrm : class
    {
        /// <summary>
        /// Royalty repository
        /// </summary>
        protected readonly Func<Repository> getNewRepository = null;

        /// <summary>
        /// File storage
        /// </summary>
        protected readonly IFileStorage storage = null;

        /// <summary>
        /// Is verbose log enabled
        /// </summary>
        public bool VerboseLog { get; set; } = false;

        /// <summary>
        /// Get config section for initialization
        /// </summary>
        /// <returns>Config section</returns>
        protected abstract ConfigurationSection GetConfigSection();

        /// <summary>
        /// Create new worker instance
        /// </summary>
        /// <param name="repository">Royalty repository</param>
        /// <param name="storage">File storage</param>
        /// <param name="timerInterval">Timer interval</param>
        public RoyaltyBase(Func<Repository> getNewRepository, IFileStorage storage, TimeSpan timerInterval)
            : base(timerInterval)
        {
            if (getNewRepository == null)
                throw new ArgumentNullException(nameof(getNewRepository));
            if (storage == null)
                throw new ArgumentNullException(nameof(storage));

            this.getNewRepository = getNewRepository;
            this.storage = storage;
            Init(GetConfigSection());
        }

        /// <summary>
        /// Create new worker instance
        /// </summary>
        /// <param name="repository">Royalty repository</param>
        /// <param name="storage">File storage</param>
        /// <param name="timerInterval">Timer interval</param>
        public RoyaltyBase(Func<Repository> getNewRepository, IFileStorage storage, bool createStarted, TimeSpan timerInterval)
            : base(createStarted, timerInterval)
        {
            if (getNewRepository == null)
                throw new ArgumentNullException(nameof(getNewRepository));
            if (storage == null)
                throw new ArgumentNullException(nameof(storage));

            this.getNewRepository = getNewRepository;
            this.storage = storage;
            Init(GetConfigSection());
        }

        /// <summary>
        /// Initialize properties
        /// </summary>
        /// <param name="configSection">Configuration section</param>
        private void Init(ConfigurationSection configSection)
        {
            if (configSection != null)
                this.CopyObjectFrom(configSection);
        }
    }
}
