using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RoyaltyService.Model
{
    [DataContract]
    public class AccountSettingsExportDirectory
    {
        [DataMember(IsRequired = false, Name = "Id")]
        public long AccountSettingsExportDirectoryID { get; set; }

        [DataMember(IsRequired = false)]
        public Guid AccountUID { get; set; }

        [DataMember(IsRequired = false)]
        public string DirectoryPath { get; set; }

        [DataMember(IsRequired = false)]
        public string FileName { get; set; }

        [DataMember(IsRequired = false)]
        public long? MarkID { get; set; }

        [DataMember(IsRequired = false)]
        public string EncodingName { get; set; }

        [DataMember(IsRequired = false)]
        public string ExecuteAfterAnalizeCommand { get; set; }

        [DataMember(IsRequired = false)]
        public TimeSpan TimeoutForExecute { get; set; }

        private static bool isInitialize = false;
        [MapperInitialize]
        public static void InitializeMap()
        {
            if (isInitialize)
                return;
#pragma warning disable 618
            AutoMapper.Mapper.CreateMap<RoyaltyRepository.Models.AccountSettingsExportDirectory, AccountSettingsExportDirectory>();
            AutoMapper.Mapper.CreateMap<AccountSettingsExportDirectory, RoyaltyRepository.Models.AccountSettingsExportDirectory>();
#pragma warning restore 618
            isInitialize = true;
        }
    }
}
