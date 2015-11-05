using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using System.Runtime.Serialization;
using RoyaltyService.Config;

namespace RoyaltyService.Model
{
    public class GuidToStringConverter : ValueResolver<Guid, string>
    {
        protected override string ResolveCore(Guid source)
        {
            return source.ToString("N");
        }
    }

    public class StringToGuidConverter : ValueResolver<string, Guid>
    {
        protected override Guid ResolveCore(string source)
        {
            return Guid.ParseExact(source, "N");
        }
    }

    public class AddUrlPrefixConverter : ValueResolver<string, string>
    {
        public static string AddUrlPrefix(string source)
        {
            if (string.IsNullOrWhiteSpace(source))
                return null;

            var fileName = source;
            if (Config.Config.IsServicesConfigured && Config.Config.ServicesConfig.FileServiceUrlPrefix != null)
                fileName = Config.Config.ServicesConfig.FileServiceUrlPrefix.AbsoluteUri + fileName;
            return fileName;
        }

        protected override string ResolveCore(string source)
        {
            return AddUrlPrefix(System.IO.Path.GetFileName(source));
        }
    }

    [DataContract]
    public class FileInfo
    {
        [DataMember(IsRequired = true)]
        public string FileID { get; set; }
        [DataMember(IsRequired = true)]
        public string FileName { get; set; }
        [DataMember(IsRequired = true)]
        public long FileSize { get; set; }
        [DataMember(IsRequired = true)]
        public string MimeType { get; set; }
        [DataMember(IsRequired = true)]
        public DateTime Date { get; set; }
        [DataMember(IsRequired = false)]
        public string Encoding { get; set; }
        [DataMember(IsRequired = false)]
        public string StoredFileName { get; set; }
        [DataMember(IsRequired = false)]
        public string PreviewSmall { get; set; }
        [DataMember(IsRequired = false)]
        public string Preview { get; set; }

        public static void InitializeMap()
        {
#pragma warning disable 618
            AutoMapper.Mapper.CreateMap<RoyaltyRepository.Models.File, FileInfo>()
                .ForMember(dst => dst.FileID, a => a.ResolveUsing<GuidToStringConverter>().FromMember(src => src.FileID))
                .ForMember(dst => dst.StoredFileName, a => a.ResolveUsing<AddUrlPrefixConverter>().FromMember(src => src.FilePath))
                .ForMember(dst => dst.Encoding, a => a.MapFrom(src => src.EncodingName))
                .AfterMap((src, dst) =>
                {
                    var mimeInfo = RoyaltyFileStorage.MimeTypes.GetPreviewImagesForMimeType(dst.MimeType);
                    if (mimeInfo != null)
                    {
                        dst.Preview = AddUrlPrefixConverter.AddUrlPrefix(mimeInfo.Big);
                        dst.PreviewSmall = AddUrlPrefixConverter.AddUrlPrefix(mimeInfo.Small);
                    }
                })
                ;

            AutoMapper.Mapper.CreateMap<FileInfo, RoyaltyRepository.Models.File>()
                .ForMember(dst => dst.FileID, a => a.ResolveUsing<StringToGuidConverter>().FromMember(src => src.FileID))
                .ForMember(dst => dst.EncodingName, a => a.MapFrom(src => src.Encoding));
#pragma warning restore 618
        }
    }
}
