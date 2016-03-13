using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RoyaltyService.Model
{
    [DataContract]
    public class ImportQueueRecordFileInfoFile
    {
        [DataMember(IsRequired = false, Name = "Id")]
        public long ImportQueueRecordFileInfoFileID { get; set; }

        [DataMember(IsRequired = false)]
        public Guid ImportQueueRecordFileInfoUID { get; set; }

        [DataMember(IsRequired = false)]
        public Guid FileUID { get; set; }

        [DataMember(IsRequired = false)]
        public File File { get; set; }

        [DataMember(IsRequired = false)]
        public string TypeName { get; set; }

        [DataMember(IsRequired = false)]
        public string TypeSystemName { get; set; }

        private static bool isInitialize = false;
        [MapperInitialize]
        public static void InitializeMap()
        {
            if (isInitialize)
                return;
            AutoMapper.Mapper.CreateMap<RoyaltyRepository.Models.ImportQueueRecordFileInfoFile, ImportQueueRecordFileInfoFile>();
            AutoMapper.Mapper.CreateMap<ImportQueueRecordFileInfoFile, RoyaltyRepository.Models.ImportQueueRecordFileInfoFile>();
            isInitialize = true;
        }
    }
}
