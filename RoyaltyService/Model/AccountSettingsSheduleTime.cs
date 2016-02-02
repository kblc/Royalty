using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RoyaltyService.Model
{
    [DataContract]
    public class AccountSettingsSheduleTime
    {
        [DataMember(IsRequired = false, Name = "Id")]
        public long AccountSettingsSheduleTimeID { get; set; }

        [DataMember(IsRequired = false)]
        public Guid AccountUID { get; set; }

        [DataMember(IsRequired = false)]
        public TimeSpan Time { get; set; }

        private static bool isInitialize = false;
        [MapperInitialize]
        public static void InitializeMap()
        {
            if (isInitialize)
                return;
#pragma warning disable 618
            AutoMapper.Mapper.CreateMap<RoyaltyRepository.Models.AccountSettingsSheduleTime, AccountSettingsSheduleTime>();
            AutoMapper.Mapper.CreateMap<AccountSettingsSheduleTime, RoyaltyRepository.Models.AccountSettingsSheduleTime>();
#pragma warning restore 618
            isInitialize = true;
        }
    }
}
