using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RoyaltyRepository.Models;
using RoyaltyRepository.Extensions;
using Helpers;

namespace RoyaltyRepository
{
    public partial class Repository
    {
        /// <summary>
        /// Get new File without any link to database
        /// </summary>
        /// <returns>File instance</returns>
        public File NewFile(Action<File> filler = null)
        {
            return New<File>(f => 
            {
                f.FileUID = Guid.NewGuid();
                f.Date = DateTime.UtcNow;
                if (filler != null)
                    filler(f);
            });
        }

        /// <summary>
        /// Get one File by name
        /// </summary>
        /// <param name="instanceId">File identifier</param>
        /// <returns>File</returns>
        public File GetFile(string fileName)
        {
            return Get<File>(f => f.OriginalFileName.EndsWith(fileName)).SingleOrDefault();
        }
    }
}
