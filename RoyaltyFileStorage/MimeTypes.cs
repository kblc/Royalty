using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoyaltyFileStorage
{
    public static class MimeTypes
    {
        /// <summary>
        /// Get extension from mime type
        /// </summary>
        /// <param name="mimeType">Mime type</param>
        /// <returns>File extension for mime type or null if not found</returns>
        public static string GetExtensionFromMimeType(string mimeType)
        {
            var mimeInfo = Config.Config.MimeTypes.FirstOrDefault(i => string.Compare(i.Name, mimeType, true) == 0);
            return mimeInfo?.Extension;
        }

        /// <summary>
        /// Get mime type from file name (file path or extension)
        /// </summary>
        /// <param name="fileName">File name or file path or File extension</param>
        /// <returns>Mime type or null if not found</returns>
        public static string GetMimeTypeFromFileName(string fileName)
        {
            var fileExtension = System.IO.Path.GetExtension(fileName); //with dot
            var mimeInfo = Config.Config.MimeTypes.FirstOrDefault(i => string.Compare(i.Extension, fileExtension, true) == 0);
            return mimeInfo?.Name;
        }
    }
}
