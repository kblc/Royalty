using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RoyaltyService.Model
{
    [DataContract]
    public class AccountSettingsImportDirectory
    {
        [DataMember(IsRequired = false, Name = "Id")]
        public long AccountSettingsImportDirectoryID { get; set; }

        [DataMember(IsRequired = false)]
        public Guid AccountUID { get; set; }

        [DataMember(IsRequired = false)]
        public string Path { get; set; }

        [DataMember(IsRequired = false)]
        public bool ForAnalize { get; set; }

        [DataMember(IsRequired = false)]
        public bool RecursiveFolderSearch { get; set; }

        [DataMember(IsRequired = false)]
        public string EncodingName { get; set; }

        [DataMember(IsRequired = false)]
        public string Filter { get; set; }

        [DataMember(IsRequired = false)]
        public bool DeleteFileAfterImport { get; set; }

        private static bool isInitialize = false;
        [MapperInitialize]
        public static void InitializeMap()
        {
            if (isInitialize)
                return;
#pragma warning disable 618
            AutoMapper.Mapper.CreateMap<RoyaltyRepository.Models.AccountSettingsImportDirectory, AccountSettingsImportDirectory>();
            AutoMapper.Mapper.CreateMap<AccountSettingsImportDirectory, RoyaltyRepository.Models.AccountSettingsImportDirectory>();
#pragma warning restore 618
            isInitialize = true;
        }
    }
}
