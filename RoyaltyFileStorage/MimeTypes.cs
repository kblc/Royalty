using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoyaltyFileStorage
{
    public class MimePreviewInfo
    {
        public readonly string Small;
        public readonly string Big;
        public MimePreviewInfo(string small, string big)
        {
            Small = small;
            Big = big;
        }
    }

    public static class MimeTypes
    {
        /// <summary>
        /// Get extension from mime type
        /// </summary>
        /// <param name="mimeType">Mime type</param>
        /// <returns>File extension for mime type or unknown if not found</returns>
        public static string GetExtensionFromMimeType(string mimeType)
        {
            var mimeInfo = Config.Config.MimeTypes.FirstOrDefault(i => string.Compare(i.Name, mimeType, true) == 0);
            return mimeInfo?.Extension ?? UnknownExtension;
        }

        /// <summary>
        /// Get preview images for mime type
        /// </summary>
        /// <param name="mimeType">Mime type</param>
        /// <returns>File extension for mime type or unknown if not found</returns>
        public static MimePreviewInfo GetPreviewImagesForMimeType(string mimeType)
        {
            var mimeInfo = Config.Config.MimeTypes.FirstOrDefault(i => string.Compare(i.Name, mimeType, true) == 0);
            if (mimeInfo == null)
                return null;
            return new MimePreviewInfo(mimeInfo.Small, mimeInfo.Resource);
        }

        /// <summary>
        /// Get mime type from file name (file path or extension)
        /// </summary>
        /// <param name="fileName">File name or file path or File extension</param>
        /// <returns>Mime type or unknown if not found</returns>
        public static string GetMimeTypeFromFileName(string fileName)
        {
            var fileExtension = System.IO.Path.GetExtension(fileName); //with dot
            var mimeInfo = Config.Config.MimeTypes.FirstOrDefault(i => string.Compare(i.Extension, fileExtension, true) == 0);
            return mimeInfo?.Name ?? UnknownMimeType;
        }

        public const string UnknownMimeType = "unknown";
        public const string UnknownExtension = ".unknown";
    }
}
