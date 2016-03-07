using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RoyaltyService.Model
{
    [DataContract]
    public class AccountPhoneMark
    {
        [DataMember(IsRequired = false, Name = "Id")]
        public long AccountPhoneMarkID { get; set; }

        [DataMember(IsRequired = false)]
        public Guid? AccountUID { get; set; }

        [DataMember(IsRequired = false)]
        public string Phone { get; set; }

        [DataMember(IsRequired = false)]
        public long MarkID { get; set; }

        private static bool isInitialize = false;
        [MapperInitialize]
        public static void InitializeMap()
        {
            if (isInitialize)
                return;
#pragma warning disable 618
            AutoMapper.Mapper.CreateMap<RoyaltyRepository.Models.AccountPhoneMark, AccountPhoneMark>();
            AutoMapper.Mapper.CreateMap<AccountPhoneMark, RoyaltyRepository.Models.AccountPhoneMark>();
#pragma warning restore 618
            isInitialize = true;
        }
    }
}
