using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RoyaltyRepository.Models;

namespace RoyaltyRepository
{
    public static class AccountSettingsExtensions
    {
        public static AccountSettingsColumn GetColumnFor(this AccountSettings settings, ColumnTypes type)
        {
            return settings.Columns.FirstOrDefault(c => c.ColumnType.Type == type);
        }
    }
}
