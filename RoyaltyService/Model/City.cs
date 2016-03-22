using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RoyaltyService.Model
{
    [DataContract]
    public class City
    {
        [DataMember(IsRequired = false, Name = "Id")]
        public long CityID { get; set; }

        [DataMember(IsRequired = false)]
        public string Name { get; set; }

        [DataMember(IsRequired = false)]
        public string PhoneNumberCode { get; set; }

        private static bool isInitialize = false;
        [MapperInitialize]
        public static void InitializeMap()
        {
            if (isInitialize)
                return;
#pragma warning disable 618
            AutoMapper.Mapper.CreateMap<RoyaltyRepository.Models.City, City>();
            AutoMapper.Mapper.CreateMap<City, RoyaltyRepository.Models.City>();
#pragma warning restore 618
            isInitialize = true;
        }
    }
}
