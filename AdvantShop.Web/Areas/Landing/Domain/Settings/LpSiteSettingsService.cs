using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using AdvantShop.Core.Caching;
using AdvantShop.Core.Services.Landing;
using AdvantShop.Core.SQL;

namespace AdvantShop.App.Landing.Domain.Settings
{
    public class LpSiteSettingsService
    {
        public void AddOrUpdate(int landingSiteId, string name, string value)
        {
            SQLDataAccess.ExecuteNonQuery(
                "If ((Select Count(*) From [CMS].[LandingSiteSettings] Where LandingSiteId = @LandingSiteId and Name = @name) > 0) " +
                " Update [CMS].[LandingSiteSettings] Set [Value] = @Value Where LandingSiteId = @LandingSiteId and Name = @name " +
                "Else " +
                " Insert Into [CMS].[LandingSiteSettings] ([LandingSiteId],[Name],[Value]) Values (@LandingSiteId,@Name,@Value) ",
                CommandType.Text,
                new SqlParameter("@LandingSiteId", landingSiteId),
                new SqlParameter("@Name", name),
                new SqlParameter("@Value", value ?? string.Empty)
                );

            CacheManager.RemoveByPattern(LpConstants.LpCachePrefix + landingSiteId + "_site_setting_" + name);
        }

        public string Get(int landingSiteId, string name)
        {
            return
                CacheManager.Get(LpConstants.LpCachePrefix + landingSiteId + "_site_setting_" + name, LpConstants.LpCacheTime,
                    () =>
                    {
                        var setting =
                            SQLDataAccess.Query<LpSettings>(
                                "Select * From [CMS].[LandingSiteSettings] Where LandingSiteId = @landingSiteId and Name = @name",
                                new { landingSiteId, name}).FirstOrDefault();

                        return setting != null ? setting.Value : string.Empty;
                    });
        }

        public List<LpSettings> GetList(int landingSiteId)
        {
            return
                SQLDataAccess.Query<LpSettings>(
                    "Select * From [CMS].[LandingSiteSettings] Where LandingSiteId = @landingSiteId",
                    new {landingSiteId}).ToList();

        }

    }
}
