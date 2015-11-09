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
    public class File
    {
        [DataMember(IsRequired = false, Name = "Id")]
        public string FileID { get; set; }
        [DataMember(IsRequired = false)]
        public string FileName { get; set; }
        [DataMember(IsRequired = false)]
        public long FileSize { get; set; }
        [DataMember(IsRequired = false)]
        public string MimeType { get; set; }
        [DataMember(IsRequired = false)]
        public DateTime Date { get; set; }
        [DataMember(Name = "Encoding", IsRequired = false)]
        public string EncodingName { get; set; }
        [IgnoreDataMember]
        public Encoding Encoding
        {
            get {
                try { return Encoding.GetEncoding(EncodingName); }
                catch { return null; }
                }
            set { EncodingName = value?.WebName; }
        }
        [DataMember(IsRequired = false)]
        public string StoredFileName { get; set; }
        [DataMember(IsRequired = false)]
        public string PreviewSmall { get; set; }
        [DataMember(IsRequired = false)]
        public string Preview { get; set; }

        private static bool isInitialized = false;
        [MapperInitialize]
        public static void InitializeMap()
        {
            if (isInitialized)
                return;

#pragma warning disable 618
            AutoMapper.Mapper.CreateMap<RoyaltyRepository.Models.File, File>()
                .ForMember(dst => dst.FileID, a => a.ResolveUsing<GuidToStringConverter>().FromMember(src => src.FileID))
                .ForMember(dst => dst.StoredFileName, a => a.ResolveUsing<AddUrlPrefixConverter>().FromMember(src => src.OriginalFileName))
                .ForMember(dst => dst.EncodingName, a => a.MapFrom(src => src.EncodingName))
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

            AutoMapper.Mapper.CreateMap<File, RoyaltyRepository.Models.File>()
                .ForMember(dst => dst.FileID, a => a.ResolveUsing<StringToGuidConverter>().FromMember(src => src.FileID))
                .ForMember(dst => dst.EncodingName, a => a.MapFrom(src => src.EncodingName));
#pragma warning restore 618
            isInitialized = true;
        }
    }
}
