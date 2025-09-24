//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using AdvantShop.Configuration;

namespace AdvantShop.Core.Services.Configuration.Settings
{
    public class SettingsMigration
    {
        public static bool FilesInRootMigrationСompleted
        {
            get => Convert.ToBoolean(SettingProvider.Items["FilesInRootMigrationСompleted"]);
            set => SettingProvider.Items["FilesInRootMigrationСompleted"] = value.ToString();
        }
    }
}