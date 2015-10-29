using RoyaltyFileStorage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoyaltyWorker.Extensions
{
    public static class RepositoryFileStorage
    {
        public static RoyaltyRepository.Models.File FilePut(this RoyaltyRepository.Repository repository, IFileStorage storage, Stream streamToUpload, string fileName)
        {
            var repFile = repository.FileNew(new { FileName = fileName, MimeType = MimeTypes.GetMimeTypeFromFileName(fileName) });
            var fileInfo = storage.FilePut(repFile.FileID, streamToUpload, fileName);
            repFile.FilePath = fileInfo.FullName;
            repFile.FileSize = fileInfo.Length;
            return repFile;
        }

        public static RoyaltyRepository.Models.File FilePut(this RoyaltyRepository.Repository repository, IFileStorage storage, IEnumerable<string> lines, Encoding encoding, string fileName)
        {
            using (var stream = new System.IO.MemoryStream())
            {
                using (var writer = new System.IO.StreamWriter(stream, encoding))
                    lines.ToList().ForEach(s => writer.WriteLine(s));
                var file = FilePut(repository, storage, stream, fileName);
                file.Encoding = encoding;
                return file;
            }
        }

        public static IEnumerable<string> FileGetLines(this RoyaltyRepository.Repository repository, IFileStorage storage, RoyaltyRepository.Models.File repositoryFile)
        {
            var res = new List<string>();
            using (var fileStream = storage.FileGet(repositoryFile.FileID))
            using (var sr = new System.IO.StreamReader(fileStream, repositoryFile.Encoding))
            {
                var line = string.Empty;
                while ((line = sr.ReadLine()) != null)
                    res.Add(line);
            }
            return res;
        }
    }
}
