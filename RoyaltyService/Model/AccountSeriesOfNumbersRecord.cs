using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RoyaltyService.Model
{
    [DataContract]
    public class AccountSeriesOfNumbersRecord
    {
        [DataMember(IsRequired = false, Name = "Id")]
        public long AccountNumberSeriaRecordID { get; set; }

        [DataMember(IsRequired = false)]
        public Guid? AccountUID { get; set; }

        [DataMember(IsRequired = false)]
        public long DigitCount { get; set; }

        [DataMember(IsRequired = false)]
        public TimeSpan Delay { get; set; }

        private static bool isInitialize = false;
        [MapperInitialize]
        public static void InitializeMap()
        {
            if (isInitialize)
                return;
#pragma warning disable 618
            AutoMapper.Mapper.CreateMap<RoyaltyRepository.Models.AccountSeriesOfNumbersRecord, AccountSeriesOfNumbersRecord>();
            AutoMapper.Mapper.CreateMap<AccountSeriesOfNumbersRecord, RoyaltyRepository.Models.AccountSeriesOfNumbersRecord>();
#pragma warning restore 618
            isInitialize = true;
        }
    }
}
