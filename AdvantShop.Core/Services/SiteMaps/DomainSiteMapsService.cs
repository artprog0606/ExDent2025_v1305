using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using AdvantShop.Core.SQL;
using AdvantShop.Helpers;

namespace AdvantShop.SiteMaps
{
    public class DomainSiteMapsService
    {
        public static List<DomainForSiteMapModel> GetSitemapDomainModels()
        {
            return SQLDataAccess
                .ExecuteReadList<DomainForSiteMapModel>(
                    "Select * From [Settings].[DomainGeoLocation]", CommandType.Text, GetSitemapDomainModelFromReader)
                .ToList();
        }
        
        private static DomainForSiteMapModel GetSitemapDomainModelFromReader(SqlDataReader reader)
        {
            return new DomainForSiteMapModel
            {
                Id = SQLDataHelper.GetInt(reader, "Id"),
                Url = SQLDataHelper.GetString(reader, "Url")
            };
        }
    }
}