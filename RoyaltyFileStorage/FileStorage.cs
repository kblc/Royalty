using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoyaltyFileStorage
{
    /// <summary>
    /// Default implementation for file storage
    /// </summary>
    public class FileStorage : IFileStorage
    {
        /// <summary>
        /// Create new instance
        /// </summary>
        public FileStorage()
        {
            if (Config.Config.IsStorageConfigured)
            try
            {
                Location = Config.Config.StorageConfig.Location;
                VerboseLog = Config.Config.StorageConfig.VerboseLog;
            }
            catch { }
        }

        /// <summary>
        /// File storage location
        /// </summary>
        public string Location { get; set; } = string.Empty;

        /// <summary>
        /// Enable verbose log events
        /// </summary>
        public bool VerboseLog { get; set; } = false;

        /// <summary>
        /// Delete file from file storage
        /// </summary>
        /// <param name="fileId">File identifier</param>
        public void FileDelete(Guid fileId)
        {
            using (var logSession = Helpers.Log.Session($"{GetType().Name}.{nameof(FileDelete)}()", VerboseLog, RaiseLogEvent))
                try
                {
                    var searchPattern = fileId.ToString("N") + "*";

                    logSession.Add($"Delete file with '{searchPattern}' names in next location: '{Location}'");
                    var filesToDelete = System.IO.Directory.GetFiles(Location, searchPattern, SearchOption.TopDirectoryOnly);
                    logSession.Add($"Files founded: {filesToDelete.Length}");

                    foreach(var fileName in filesToDelete)
                        try
                        {
                            logSession.Add($"Try delete file '{fileName}'");
                            System.IO.File.Delete(fileName);
                        }
                        catch(Exception ex)
                        {
                            ex.Data.Add(nameof(fileName), fileName);
                            throw ex;
                        }
                    logSession.Add("All files deleted");
                }
                catch(Exception ex)
                {
                    ex.Data.Add(nameof(fileId), fileId);
                    logSession.Add(ex);
                    logSession.Enabled = true;
                    RaiseExceptionEvent(ex);
                    throw ex;
                }
        }

        /// <summary>
        /// Get file stream from file storage
        /// </summary>
        /// <param name="fileId">File identifier</param>
        /// <returns>File stream</returns>
        public Stream FileGet(Guid fileId)
        {
            using (var logSession = Helpers.Log.Session($"{GetType().Name}.{nameof(FileGet)}()", VerboseLog, RaiseLogEvent))
                try
                {
                    var searchPattern = fileId.ToString("N") + "*";

                    logSession.Add($"Try get file with '{searchPattern}' names in next location: '{Location}'");
                    var filesExisted = System.IO.Directory.GetFiles(Location, searchPattern, SearchOption.TopDirectoryOnly);
                    logSession.Add($"Files founded: {filesExisted.Length}");
                    if (filesExisted.Length == 1)
                    {
                        var fileName = filesExisted.First();
                        logSession.Add($"Open following file: {fileName}");
                        return System.IO.File.OpenRead(fileName);
                    }
                    else
                        throw new Exception($"File count must be 1 instead of {filesExisted.Length}");
                }
                catch (Exception ex)
                {
                    ex.Data.Add(nameof(fileId), fileId);
                    logSession.Add(ex);
                    logSession.Enabled = true;
                    RaiseExceptionEvent(ex);
                    throw ex;
                }
        }

        /// <summary>
        /// Put file to file storage
        /// </summary>
        /// <param name="fileId">File identifier</param>
        /// <param name="stream">Input file stream</param>
        /// <param name="fileName">File name</param>
        public FileInfo FilePut(Guid fileId, Stream stream, string fileName)
        {
            return FilePutWithExtension(fileId, stream, fileName);
        }

        /// <summary>
        /// Put file to file storage with file identifier and extension
        /// </summary>
        /// <param name="fileId">File identifier</param>
        /// <param name="stream">Stream to save</param>
        /// <param name="originalFileName">File extension</param>
        /// <returns>Save file info</returns>
        private FileInfo FilePutWithExtension(Guid fileId, Stream stream, string originalFileName)
        {
            using (var logSession = Helpers.Log.Session($"{GetType().Name}.{nameof(FilePutWithExtension)}()", VerboseLog, RaiseLogEvent))
                try
                {
                    var origFileName = Path.GetFileName(originalFileName);
                    foreach (var c in Path.GetInvalidFileNameChars())
                        origFileName = origFileName.Replace(c.ToString(), "");

                    var fileName = fileId.ToString("N") + (Path.GetExtension(origFileName) != origFileName ? "_" : string.Empty) + origFileName;
                    var filePath = System.IO.Path.Combine(Location, fileName);
                    logSession.Add($"Try put file with '{fileName}' name in next location: '{Location}'");
                    try
                    {
                        using (var fileStream = File.OpenWrite(filePath))
                        {
                            stream.CopyTo(fileStream);
                            fileStream.Flush();
                        }
                        logSession.Add($"File saved in '{filePath}'");
                        return new FileInfo(filePath);
                    }
                    catch (Exception ex)
                    {
                        ex.Data.Add(nameof(filePath), filePath);
                        throw ex;
                    }
                }
                catch (Exception ex)
                {
                    ex.Data.Add(nameof(fileId), fileId);
                    logSession.Add(ex);
                    logSession.Enabled = true;
                    RaiseExceptionEvent(ex);
                    throw ex;
                }
        }

        #region Events

        public event EventHandler<string> Log;
        public event EventHandler<Exception> Exception;

        private void RaiseLogEvent(IEnumerable<string> logTexts)
        {
            logTexts?.ToList().ForEach(s => RaiseLogEvent(s));
        }
        private void RaiseLogEvent(string logText)
        {
            Log?.Invoke(this, logText);
        }
        private void RaiseExceptionEvent(Exception ex)
        {
            Exception?.Invoke(this, ex);
        }

        #endregion
    }
}
