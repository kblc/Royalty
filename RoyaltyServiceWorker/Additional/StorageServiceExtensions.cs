using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RoyaltyServiceWorker.Additional
{
    public static class StorageServiceExtensions
    {
        public static Task<StorageService.File> UploadFile(this StorageService.FileServiceClient client, string filePath, Encoding encoding, CancellationToken? cancellationToken = null)
        {
            return UploadFiles(client, new[] { filePath }, encoding, cancellationToken).FirstOrDefault();
        }

        public static Task<StorageService.File>[] UploadFiles(this StorageService.FileServiceClient client, IEnumerable<string> filePathes, Encoding encoding, CancellationToken? cancellationToken = null)
        {
            var cancToken = cancellationToken == null ? CancellationToken.None : cancellationToken.Value;

            var loadFileTasks = filePathes.Select(filePath =>
            {
                var fs = new FileStream(filePath, FileMode.Open);

                var putTask = client.PutAsync(fs);
                var closeStreamTask = putTask.ContinueWith(r => { try { fs.Dispose(); } catch { } }, System.Threading.CancellationToken.None, TaskContinuationOptions.AttachedToParent, TaskScheduler.Default);
                var updateTask = putTask
                .ContinueWith(r => {
                    if (r.Exception != null)
                        throw r.Exception;

                    if (r.Result.Error != null)
                        throw new Exception(r.Result.Error);

                    r.Result.Value.Encoding = encoding.WebName;
                    r.Result.Value.FileName = Path.GetFileName(filePath);
                    r.Result.Value.MimeType = null;

                    var res = client.Update(r.Result.Value);

                    if (res.Error != null)
                        throw new Exception(res.Error);

                    return res.Value;
                }, cancToken, TaskContinuationOptions.AttachedToParent, TaskScheduler.Default); //OnlyOnRanToCompletion
                return updateTask;
            }).ToArray();

            return loadFileTasks;
        }
    }
}
