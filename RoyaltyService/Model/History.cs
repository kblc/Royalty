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

        [DataMember]
        [RepositoryHistoryLink(typeof(RoyaltyRepository.Models.AccountSettingsExportDirectory), typeof(long))]
        public long[] AccountSettingsExportDirectory { get; set; }

        [DataMember]
        [RepositoryHistoryLink(typeof(RoyaltyRepository.Models.AccountSettingsImportDirectory), typeof(long))]
        public long[] AccountSettingsImportDirectory { get; set; }

        [DataMember]
        [RepositoryHistoryLink(typeof(RoyaltyRepository.Models.AccountSettingsSheduleTime), typeof(long))]
        public long[] AccountSettingsSheduleTime { get; set; }

        [DataMember]
        [RepositoryHistoryLink(typeof(RoyaltyRepository.Models.AccountSeriesOfNumbersRecord), typeof(long))]
        public long[] AccountSeriesOfNumbersRecord { get; set; }

        [DataMember]
        [RepositoryHistoryLink(typeof(RoyaltyRepository.Models.AccountDataRecordAdditionalColumn), typeof(long))]
        public long[] AccountDataRecordAdditionalColumn { get; set; }

        [DataMember]
        [RepositoryHistoryLink(typeof(RoyaltyRepository.Models.AccountPhoneMark), typeof(long))]
        public long[] AccountPhoneMark { get; set; }

        [DataMember]
        [RepositoryHistoryLink(typeof(RoyaltyRepository.Models.ImportQueueRecord), typeof(Guid))]
        public Guid[] ImportQueueRecord { get; set; }

        [DataMember]
        [RepositoryHistoryLink(typeof(RoyaltyRepository.Models.ImportQueueRecordFileAccountDataRecord), typeof(long))]
        public long[] ImportQueueRecordFileAccountDataRecord { get; set; }

        [DataMember]
        [RepositoryHistoryLink(typeof(RoyaltyRepository.Models.ImportQueueRecordFileInfo), typeof(Guid))]
        public Guid[] ImportQueueRecordFileInfo { get; set; }

        [DataMember]
        [RepositoryHistoryLink(typeof(RoyaltyRepository.Models.ImportQueueRecordFileInfoFile), typeof(long))]
        public long[] ImportQueueRecordFileInfoFile { get; set; }
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
        [RepositoryHistoryLink(typeof(RoyaltyRepository.Models.AccountSettingsColumn))]
        public AccountSettingsColumn[] AccountSettingsColumn { get; set; }

        [DataMember]
        [RepositoryHistoryLink(typeof(RoyaltyRepository.Models.AccountSettingsExportDirectory))]
        public AccountSettingsExportDirectory[] AccountSettingsExportDirectory { get; set; }

        [DataMember]
        [RepositoryHistoryLink(typeof(RoyaltyRepository.Models.AccountSettingsImportDirectory))]
        public AccountSettingsImportDirectory[] AccountSettingsImportDirectory { get; set; }

        [DataMember]
        [RepositoryHistoryLink(typeof(RoyaltyRepository.Models.AccountSettingsSheduleTime))]
        public AccountSettingsSheduleTime[] AccountSettingsSheduleTime { get; set; }

        [DataMember]
        [RepositoryHistoryLink(typeof(RoyaltyRepository.Models.AccountSeriesOfNumbersRecord))]
        public AccountSeriesOfNumbersRecord[] AccountSeriesOfNumbersRecord { get; set; }

        [DataMember]
        [RepositoryHistoryLink(typeof(RoyaltyRepository.Models.AccountDataRecordAdditionalColumn))]
        public AccountDataRecordAdditionalColumn[] AccountDataRecordAdditionalColumn { get; set; }

        [DataMember]
        [RepositoryHistoryLink(typeof(RoyaltyRepository.Models.AccountPhoneMark))]
        public AccountPhoneMark[] AccountPhoneMark { get; set; }

        [DataMember]
        [RepositoryHistoryLink(typeof(RoyaltyRepository.Models.ImportQueueRecord))]
        public ImportQueueRecord[] ImportQueueRecord { get; set; }

        [DataMember]
        [RepositoryHistoryLink(typeof(RoyaltyRepository.Models.ImportQueueRecordFileAccountDataRecord))]
        public ImportQueueRecordFileAccountDataRecord[] ImportQueueRecordFileAccountDataRecord { get; set; }

        [DataMember]
        [RepositoryHistoryLink(typeof(RoyaltyRepository.Models.ImportQueueRecordFileInfo))]
        public ImportQueueRecordFileInfo[] ImportQueueRecordFileInfo { get; set; }

        [DataMember]
        [RepositoryHistoryLink(typeof(RoyaltyRepository.Models.ImportQueueRecordFileInfoFile))]
        public ImportQueueRecordFileInfoFile[] ImportQueueRecordFileInfoFile { get; set; }
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
