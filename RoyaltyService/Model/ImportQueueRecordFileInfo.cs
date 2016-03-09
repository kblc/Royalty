using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RoyaltyService.Model
{
    [DataContract]
    public class ImportQueueRecordFileInfo
    {
        [DataMember(IsRequired = false, Name = "Id")]
        public Guid ImportQueueRecordFileUID { get; set; }

        [DataMember(IsRequired = false)]
        public Guid ImportQueueRecordUID { get; set; }

        [DataMember(IsRequired = false)]
        public string Error { get; set; }

        [DataMember(IsRequired = false)]
        public string SourceFilePath { get; set; }

        [DataMember(IsRequired = false)]
        public DateTime? Started { get; set; }

        [DataMember(IsRequired = false)]
        public DateTime? Finished { get; set; }

        [DataMember(IsRequired = false)]
        public bool ForAnalize { get; set; }

        [DataMember(IsRequired = false)]
        public long ImportQueueRecordStateID { get; set; }

        [DataMember(IsRequired = false)]
        public List<ImportQueueRecordFileInfoFile> Files { get; set; }

        [DataMember(IsRequired = false)]
        public List<ImportQueueRecordFileAccountDataRecord> LoadedRecords { get; set; }

        private static bool isInitialize = false;
        [MapperInitialize]
        public static void InitializeMap()
        {
            if (isInitialize)
                return;
            AutoMapper.Mapper.CreateMap<RoyaltyRepository.Models.ImportQueueRecordFileInfo, ImportQueueRecordFileInfo>();
            AutoMapper.Mapper.CreateMap<ImportQueueRecordFileInfo, RoyaltyRepository.Models.ImportQueueRecordFileInfo>();
            isInitialize = true;
        }
    }
}
