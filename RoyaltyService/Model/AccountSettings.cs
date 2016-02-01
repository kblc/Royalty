using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RoyaltyService.Model
{
    [DataContract]
    public class AccountSettings
    {
        [DataMember(IsRequired = false, Name = "Id")]
        public Guid AccountUID { get; set; }

        [DataMember(IsRequired = false)]
        public TimeSpan? IgnoreExportTime { get; set; }

        [DataMember(IsRequired = false)]
        public TimeSpan? TimeForTrust { get; set; }

        [DataMember(IsRequired = false)]
        public List<AccountSettingsColumn> Columns { get; set; }

        private static bool isInitialize = false;
        [MapperInitialize]
        public static void InitializeMap()
        {
            if (isInitialize)
                return;
#pragma warning disable 618
            AutoMapper.Mapper.CreateMap<RoyaltyRepository.Models.AccountSettings, AccountSettings>();
            AutoMapper.Mapper.CreateMap<AccountSettings, RoyaltyRepository.Models.AccountSettings>();
#pragma warning restore 618
            isInitialize = true;
        }
    }
}
