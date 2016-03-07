using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RoyaltyService.Model
{
    [DataContract]
    public class AccountDataRecordAdditionalColumn
    {
        [DataMember(IsRequired = false, Name = "Id")]
        public long AccountDataRecordAdditionalColumnID { get; set; }

        [DataMember(IsRequired = false)]
        public Guid? AccountUID { get; set; }

        [DataMember(IsRequired = false)]
        public string ColumnSystemName { get; set; }

        [DataMember(IsRequired = false)]
        public string ColumnName { get; set; }

        [DataMember(IsRequired = false)]
        public bool Export { get; set; }

        private static bool isInitialize = false;
        [MapperInitialize]
        public static void InitializeMap()
        {
            if (isInitialize)
                return;
#pragma warning disable 618
            AutoMapper.Mapper.CreateMap<RoyaltyRepository.Models.AccountDataRecordAdditionalColumn, AccountDataRecordAdditionalColumn>();
            AutoMapper.Mapper.CreateMap<AccountDataRecordAdditionalColumn, RoyaltyRepository.Models.AccountDataRecordAdditionalColumn>();
#pragma warning restore 618
            isInitialize = true;
        }
    }
}
