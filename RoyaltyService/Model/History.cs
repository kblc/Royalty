using System;
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

        public RepositoryHistoryLinkAttribute() { }

        public RepositoryHistoryLinkAttribute(Type repositoryEntityType) { RepositoryEntityType = repositoryEntityType; }
    }


    [DataContract]
    public class HistoryIdOnlytPart
    {
        [DataMember]
        [RepositoryHistoryLink(typeof(RoyaltyRepository.Models.Account))]
        public Guid[] AccountId { get; set; }

        //[IgnoreDataMember]
        //public object[] AccountObjectId
        //{
        //    get { return AccountId.Cast<object>().ToArray(); }
        //    set { AccountId = value.Cast<Guid>().ToArray(); }
        //}
    }

    [DataContract]
    public class HistoryPart
    {
        [DataMember]
        [RepositoryHistoryLink(typeof(RoyaltyRepository.Models.Account))]
        public Account[] Account { get; set; }
    }

    [DataContract]
    public class History
    {
        [DataMember(IsRequired = true)]
        public long EventId { get; set; }

        [DataMember(IsRequired = false)]
        public HistoryPart Add { get; set; }

        [DataMember(IsRequired = false)]
        public HistoryPart Change { get; set; }

        [DataMember(IsRequired = false)]
        public HistoryIdOnlytPart Remove { get; set; }
    }
}
