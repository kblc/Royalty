using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RoyaltyRepository.Models;
using Helpers;
using RoyaltyRepository.Extensions;

namespace RoyaltyRepository
{

    public static partial class RepositoryExtensions
    {
        public static AccountSettingsColumn GetColumnByType(this AccountSettings settings, ColumnTypes columnType)
        {
#pragma warning disable 618
            return settings.Columns
                .FirstOrDefault(c => string.Compare(c.ColumnType.SystemName, columnType.ToString(), true) == 0);
#pragma warning restore 618
        }
    }
}
