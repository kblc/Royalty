using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RoyaltyService.Model
{
    [DataContract]
    public class Area
    {
        [DataMember(IsRequired = false, Name = "Id")]
        public long AreaID { get; set; }

        [DataMember(IsRequired = false)]
        public bool IsDefault { get; set; }

        [DataMember(IsRequired = false)]
        public string Name { get; set; }

        [DataMember(IsRequired = false)]
        public long CityID { get; set; }

        private static bool isInitialize = false;
        [MapperInitialize]
        public static void InitializeMap()
        {
            if (isInitialize)
                return;
#pragma warning disable 618
            AutoMapper.Mapper.CreateMap<RoyaltyRepository.Models.Area, Area>();
            AutoMapper.Mapper.CreateMap<Area, RoyaltyRepository.Models.Area>();
#pragma warning restore 618
            isInitialize = true;
        }
    }
}
