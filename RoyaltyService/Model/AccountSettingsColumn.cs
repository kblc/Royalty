using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RoyaltyService.Model
{
    [DataContract]
    public class AccountSettingsColumn
    {
        [DataMember(IsRequired = false, Name = "Id")]
        public long AccountUIDAccountSettingsColumnID { get; set; }

        [DataMember(IsRequired = false)]
        public Guid AccountUID { get; set; }

        [DataMember(IsRequired = false)]
        public long ColumnTypeID { get; set; }

        [DataMember(IsRequired = false)]
        public string ColumnName { get; set; }

        private static bool isInitialize = false;
        [MapperInitialize]
        public static void InitializeMap()
        {
            if (isInitialize)
                return;
#pragma warning disable 618
            AutoMapper.Mapper.CreateMap<RoyaltyRepository.Models.AccountSettingsColumn, AccountSettingsColumn>();
            AutoMapper.Mapper.CreateMap<AccountSettingsColumn, RoyaltyRepository.Models.AccountSettingsColumn>();
#pragma warning restore 618
            isInitialize = true;
        }
    }
}
