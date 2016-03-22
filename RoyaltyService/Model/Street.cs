using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RoyaltyService.Model
{
    [DataContract]
    public class Street
    {
        [DataMember(IsRequired = false, Name = "Id")]
        public long StreetID { get; set; }

        [DataMember(IsRequired = false)]
        public string Name { get; set; }

        [DataMember(IsRequired = false)]
        public long AreaID { get; set; }

        private static bool isInitialize = false;
        [MapperInitialize]
        public static void InitializeMap()
        {
            if (isInitialize)
                return;
#pragma warning disable 618
            AutoMapper.Mapper.CreateMap<RoyaltyRepository.Models.Street, Street>();
            AutoMapper.Mapper.CreateMap<Street, RoyaltyRepository.Models.Street>();
#pragma warning restore 618
            isInitialize = true;
        }
    }
}
