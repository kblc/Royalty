using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RoyaltyService.Model
{
    [DataContract]
    public class Phone
    {
        [DataMember(IsRequired = false, Name = "Id")]
        public long PhoneID { get; set; }

        [DataMember(IsRequired = false)]
        public string PhoneNumber { get; set; }

        private static bool isInitialize = false;
        [MapperInitialize]
        public static void InitializeMap()
        {
            if (isInitialize)
                return;
#pragma warning disable 618
            AutoMapper.Mapper.CreateMap<RoyaltyRepository.Models.Phone, Phone>();
            AutoMapper.Mapper.CreateMap<Phone, RoyaltyRepository.Models.Phone>();
#pragma warning restore 618
            isInitialize = true;
        }
    }
}
