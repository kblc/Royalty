using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoyaltyFileStorage
{
    /// <summary>
    /// File storage contract
    /// </summary>
    public interface IFileStorage
    {
        /// <summary>
        /// Get file stream from file storage
        /// </summary>
        /// <param name="fileId">File identifier</param>
        /// <returns>File stream</returns>
        Stream FileGet(Guid fileId);

        /// <summary>
        /// Put file to file storage
        /// </summary>
        /// <param name="fileId">File identifier</param>
        /// <param name="stream">Input file stream</param>
        /// <param name="fileName">File name or file extension</param>
        FileInfo FilePut(Guid fileId, Stream stream, string fileName);

        /// <summary>
        /// Delete file from file storage
        /// </summary>
        /// <param name="fileId">File identifier</param>
        void FileDelete(Guid fileId);
    }
}
