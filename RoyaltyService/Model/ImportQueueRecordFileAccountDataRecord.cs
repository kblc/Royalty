using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RoyaltyService.Model
{
    [DataContract]
    public class ImportQueueRecordFileAccountDataRecord
    {
        [DataMember(IsRequired = false, Name = "Id")]
        public long ImportQueueRecordFileAccountDataRecordID { get; set; }

        [DataMember(IsRequired = false)]
        public Guid ImportQueueRecordFileInfoUID { get; set; }

        [DataMember(IsRequired = false)]
        public Guid AccountDataRecordID { get; set; }

        [DataMember(IsRequired = false)]
        public DateTime LoadDate { get; set; }

        private static bool isInitialize = false;
        [MapperInitialize]
        public static void InitializeMap()
        {
            if (isInitialize)
                return;
            AutoMapper.Mapper.CreateMap<RoyaltyRepository.Models.ImportQueueRecordFileAccountDataRecord, ImportQueueRecordFileAccountDataRecord>();
            AutoMapper.Mapper.CreateMap<ImportQueueRecordFileAccountDataRecord, RoyaltyRepository.Models.ImportQueueRecordFileAccountDataRecord>();
            isInitialize = true;
        }
    }
}
