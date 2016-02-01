﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RoyaltyService.Model
{
    [DataContract]
    public class Account
    {
        [DataMember(IsRequired = false, Name = "Id")]
        public Guid AccountUID { get; set; }

        [DataMember(IsRequired = false)]
        public string Name { get; set; }

        [DataMember(IsRequired = false)]
        public bool IsHidden { get; set; }

        [DataMember(IsRequired = false)]
        public bool IsActive { get; set; }

        private static bool isInitialize = false;
        [MapperInitialize]
        public static void InitializeMap()
        {
            if (isInitialize)
                return;
#pragma warning disable 618
            AutoMapper.Mapper.CreateMap<RoyaltyRepository.Models.Account, Account>();
            AutoMapper.Mapper.CreateMap<Account, RoyaltyRepository.Models.Account>();
#pragma warning restore 618
            isInitialize = true;
        }
    }
}
