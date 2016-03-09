using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RoyaltyService.Model
{
    [DataContract]
    public class ImportQueueRecord
    {
        [DataMember(IsRequired = false, Name = "Id")]
        public Guid ImportQueueRecordUID { get; set; }

        [DataMember(IsRequired = false)]
        public Guid AccountUID { get; set; }

        [DataMember(IsRequired = false)]
        public DateTime CreatedDate { get; set; }

        [DataMember(IsRequired = false)]
        public DateTime ProcessedDate { get; set; }

        [DataMember(IsRequired = false)]
        public bool HasError { get; set; }

        [DataMember(IsRequired = false)]
        public string Error { get; set; }

        [DataMember(IsRequired = false)]
        public List<ImportQueueRecordFileInfo> FileInfoes { get; set; }

        private static bool isInitialize = false;
        [MapperInitialize]
        public static void InitializeMap()
        {
            if (isInitialize)
                return;
            AutoMapper.Mapper.CreateMap<RoyaltyRepository.Models.ImportQueueRecord, ImportQueueRecord>();
            AutoMapper.Mapper.CreateMap<ImportQueueRecord, RoyaltyRepository.Models.ImportQueueRecord>();
            isInitialize = true;
        }
    }
}
