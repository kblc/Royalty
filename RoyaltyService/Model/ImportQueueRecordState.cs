using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RoyaltyService.Model
{
    [DataContract]
    public class ImportQueueRecordState
    {
        [DataMember(IsRequired = false, Name = "Id")]
        public long ImportQueueRecordStateID { get; set; }

        [DataMember(IsRequired = false)]
        public string Name { get; set; }

        [DataMember(IsRequired = false)]
        public string SystemName { get; set; }

        private static bool isInitialize = false;
        [MapperInitialize]
        public static void InitializeMap()
        {
            if (isInitialize)
                return;
#pragma warning disable 618
            AutoMapper.Mapper.CreateMap<RoyaltyRepository.Models.ImportQueueRecordState, ImportQueueRecordState>();
            AutoMapper.Mapper.CreateMap<ImportQueueRecordState, RoyaltyRepository.Models.ImportQueueRecordState>();
#pragma warning restore 618
            isInitialize = true;
        }
    }
}
