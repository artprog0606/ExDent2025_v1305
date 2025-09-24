using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using AdvantShop.Core.Caching;
using AdvantShop.Core.Services.Landing;
using AdvantShop.Core.SQL;

namespace AdvantShop.App.Landing.Domain.Settings
{
    public class LpSettingsService
    {

        public void AddOrUpdate(int landingId, string name, string value)
        {
            SQLDataAccess.ExecuteNonQuery(
                "If ((Select Count(*) From [CMS].[LandingSettings] Where LandingId = @LandingId and Name = @name) > 0) " +
                " Update [CMS].[LandingSettings] Set [Value] = @Value Where LandingId = @LandingId and Name = @name " +
                "Else " +
                " Insert Into [CMS].[LandingSettings] ([LandingId],[Name],[Value]) Values (@LandingId,@Name,@Value) ",
                CommandType.Text,
                new SqlParameter("@LandingId", landingId),
                new SqlParameter("@Name", name),
                new SqlParameter("@Value", value ?? string.Empty)
                );

            CacheManager.RemoveByPattern(LpConstants.LpCachePrefix + landingId + "_setting_" + name);
        }

        public string Get(int landingId, string name)
        {
            return
                CacheManager.Get(LpConstants.LpCachePrefix + landingId + "_setting_" + name, LpConstants.LpCacheTime,
                    () =>
                    {
                        var setting =
                            SQLDataAccess.Query<LpSettings>(
                                "Select * From [CMS].[LandingSettings] Where LandingId = @landingId and Name = @name",
                                new {landingId, name}).FirstOrDefault();

                        return setting != null ? setting.Value : string.Empty;
                    });
        }

        public List<LpSettings> GetList(int landingId)
        {
            return
                SQLDataAccess.Query<LpSettings>("Select * From [CMS].[LandingSettings] Where LandingId = @landingId",
                    new {landingId}).ToList();
        }
    }
}
