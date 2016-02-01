﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RoyaltyService.Model
{
    [AttributeUsage(AttributeTargets.Property, Inherited = true)]
    public class RepositoryHistoryLinkAttribute : Attribute
    {
        public Type RepositoryEntityType { get; set; }
        public Type RepositoryArrayElementEntitySourceType { get; set; }

        public RepositoryHistoryLinkAttribute() { }
        public RepositoryHistoryLinkAttribute(Type repositoryEntityType) { RepositoryEntityType = repositoryEntityType; RepositoryArrayElementEntitySourceType = repositoryEntityType; }
        public RepositoryHistoryLinkAttribute(Type repositoryEntityType, Type repositoryArrayElementEntitySourceType) { RepositoryEntityType = repositoryEntityType; RepositoryArrayElementEntitySourceType = repositoryArrayElementEntitySourceType; }
    }


    [DataContract]
    public class HistoryRemovePart
    {
        [DataMember]
        [RepositoryHistoryLink(typeof(RoyaltyRepository.Models.Account), typeof(Guid))]
        public Guid[] Account { get; set; }

        [DataMember]
        [RepositoryHistoryLink(typeof(RoyaltyRepository.Models.AccountSettings), typeof(Guid))]
        public Guid[] AccountSettings { get; set; }

        [DataMember]
        [RepositoryHistoryLink(typeof(RoyaltyRepository.Models.AccountSettingsColumn), typeof(long))]
        public long[] AccountSettingsColumn { get; set; }
    }

    [DataContract]
    public class HistoryUpdatePart
    {
        [DataMember]
        [RepositoryHistoryLink(typeof(RoyaltyRepository.Models.Account))]
        public Account[] Account { get; set; }

        [DataMember]
        [RepositoryHistoryLink(typeof(RoyaltyRepository.Models.AccountSettings))]
        public AccountSettings[] AccountSettings { get; set; }

        [DataMember]
        [RepositoryHistoryLink(typeof(RoyaltyRepository.Models.AccountSettingsColumn), typeof(long))]
        public AccountSettingsColumn[] AccountSettingsColumn { get; set; }
    }

    [DataContract]
    public class History
    {
        [DataMember(IsRequired = true)]
        public long EventId { get; set; }

        [DataMember(IsRequired = false)]
        public HistoryUpdatePart Add { get; set; }

        [DataMember(IsRequired = false)]
        public HistoryUpdatePart Change { get; set; }

        [DataMember(IsRequired = false)]
        public HistoryRemovePart Remove { get; set; }
    }
}
