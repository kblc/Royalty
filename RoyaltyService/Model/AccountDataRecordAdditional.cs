using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RoyaltyService.Model
{
    [DataContract]
    public class AccountDataRecordAdditional
    {
        [DataMember(IsRequired = false, Name = "Id")]
        public Guid AccountDataRecordUID { get; set; }

        [DataMember(IsRequired = false)]
        public string Column00 { get; set; }
        [DataMember(IsRequired = false)]
        public string Column01 { get; set; }
        [DataMember(IsRequired = false)]
        public string Column02 { get; set; }
        [DataMember(IsRequired = false)]
        public string Column03 { get; set; }
        [DataMember(IsRequired = false)]
        public string Column04 { get; set; }
        [DataMember(IsRequired = false)]
        public string Column05 { get; set; }
        [DataMember(IsRequired = false)]
        public string Column06 { get; set; }
        [DataMember(IsRequired = false)]
        public string Column07 { get; set; }
        [DataMember(IsRequired = false)]
        public string Column08 { get; set; }
        [DataMember(IsRequired = false)]
        public string Column09 { get; set; }
        [DataMember(IsRequired = false)]
        public string Column10 { get; set; }
        [DataMember(IsRequired = false)]
        public string Column11 { get; set; }
        [DataMember(IsRequired = false)]
        public string Column12 { get; set; }
        [DataMember(IsRequired = false)]
        public string Column13 { get; set; }
        [DataMember(IsRequired = false)]
        public string Column14 { get; set; }
        [DataMember(IsRequired = false)]
        public string Column15 { get; set; }
        [DataMember(IsRequired = false)]
        public string Column16 { get; set; }
        [DataMember(IsRequired = false)]
        public string Column17 { get; set; }
        [DataMember(IsRequired = false)]
        public string Column18 { get; set; }
        [DataMember(IsRequired = false)]
        public string Column19 { get; set; }

        private static bool isInitialize = false;
        [MapperInitialize]
        public static void InitializeMap()
        {
            if (isInitialize)
                return;
#pragma warning disable 618
            AutoMapper.Mapper.CreateMap<RoyaltyRepository.Models.AccountDataRecordAdditional, AccountDataRecordAdditional>();
            AutoMapper.Mapper.CreateMap<AccountDataRecordAdditional, RoyaltyRepository.Models.AccountDataRecordAdditional>();
#pragma warning restore 618
            isInitialize = true;
        }
    }
}
