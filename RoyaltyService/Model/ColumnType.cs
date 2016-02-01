using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RoyaltyService.Model
{
    [DataContract]
    public class ColumnType
    {
        [DataMember(IsRequired = false, Name = "Id")]
        public long ColumnTypeID { get; set; }

        [DataMember(IsRequired = false)]
        public string Name { get; set; }

        [DataMember(IsRequired = false)]
        public string SystemName { get; set; }

        [DataMember(IsRequired = false)]
        public bool ImportTableValidation { get; set; }

        [DataMember(IsRequired = false)]
        public bool ImportRowValidation { get; set; }

        [DataMember(IsRequired = false)]
        public long ExportColumnIndex { get; set; }

        [DataMember(IsRequired = false)]
        public bool Export { get; set; }

        private static bool isInitialize = false;
        [MapperInitialize]
        public static void InitializeMap()
        {
            if (isInitialize)
                return;
#pragma warning disable 618
            AutoMapper.Mapper.CreateMap<RoyaltyRepository.Models.ColumnType, ColumnType>();
            AutoMapper.Mapper.CreateMap<ColumnType, RoyaltyRepository.Models.ColumnType>();
#pragma warning restore 618
            isInitialize = true;
        }
    }
}
