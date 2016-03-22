using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RoyaltyService.Model
{
    [DataContract]
    public class AccountDataRecord
    {
        [DataMember(IsRequired = false, Name = "Id")]
        public Guid AccountDataRecordUID { get; set; }

        [DataMember(IsRequired = false)]
        public Guid? AccountUID { get; set; }

        [DataMember(IsRequired = false)]
        public long PhoneID { get; set; }

        [DataMember(IsRequired = false)]
        public long HostID { get; set; }

        [DataMember(IsRequired = false)]
        public long StreetID { get; set; }

        [DataMember(IsRequired = false)]
        public string HouseNumber { get; set; }

        [DataMember(IsRequired = false)]
        public DateTime Created { get; set; }

        [DataMember(IsRequired = false)]
        public DateTime Changed { get; set; }

        [DataMember(IsRequired = false)]
        public DateTime? Exported { get; set; }

        [DataMember(IsRequired = false)]
        public AccountDataRecordAdditional DataAdditional { get; set; }

        private static bool isInitialize = false;
        [MapperInitialize]
        public static void InitializeMap()
        {
            if (isInitialize)
                return;
#pragma warning disable 618
            AutoMapper.Mapper.CreateMap<RoyaltyRepository.Models.AccountDataRecord, AccountDataRecord>();
            AutoMapper.Mapper.CreateMap<AccountDataRecord, RoyaltyRepository.Models.AccountDataRecord>();
#pragma warning restore 618
            isInitialize = true;
        }
    }
}
