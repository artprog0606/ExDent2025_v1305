using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using AdvantShop.Core.Caching;
using AdvantShop.Core.SQL;

namespace AdvantShop.Core.Services.Landing.Domains
{
    public class LpDomainService
    {
        public int Add(LpDomain domain)
        {
             domain.Id = 
                Convert.ToInt32(SQLDataAccess.ExecuteScalar(
                    "INSERT INTO [CMS].[LandingDomain] ([LandingSiteId],[DomainUrl],[IsMain]) VALUES (@LandingSiteId,@DomainUrl,@IsMain); Select scope_identity(); ",
                    CommandType.Text,
                    new SqlParameter("@LandingSiteId", domain.LandingSiteId),
                    new SqlParameter("@DomainUrl", !string.IsNullOrEmpty(domain.DomainUrl) ? domain.DomainUrl.ToLower() : ""),
                    new SqlParameter("@IsMain", domain.IsMain)));

            CacheManager.RemoveByPattern(LpConstants.LpCachePrefix);

            return domain.Id;
        }

        public void Update(LpDomain domain)
        {
            SQLDataAccess.ExecuteNonQuery(
                "Update [CMS].[LandingDomain] Set DomainUrl=@DomainUrl, IsMain=@IsMain, LandingSiteId=@LandingSiteId Where Id=@Id",
                CommandType.Text,
                new SqlParameter("@Id", domain.Id),
                new SqlParameter("@DomainUrl", !string.IsNullOrEmpty(domain.DomainUrl) ? domain.DomainUrl.ToLower() : ""),
                new SqlParameter("@IsMain", domain.IsMain),
                new SqlParameter("@LandingSiteId", domain.LandingSiteId));

            CacheManager.RemoveByPattern(LpConstants.LpCachePrefix);
        }

        public void Delete(int id)
        {
            SQLDataAccess.ExecuteNonQuery("Delete From [CMS].[LandingDomain] Where Id=@Id", CommandType.Text, new SqlParameter("@Id", id));

            CacheManager.RemoveByPattern(LpConstants.LpCachePrefix);
        }


        public List<LpDomain> GetList()
        {
            return
                SQLDataAccess.Query<LpDomain>("Select * From [CMS].[LandingDomain]").ToList();
        }

        public List<LpDomain> GetList(int landingSiteId)
        {
            return
                SQLDataAccess.Query<LpDomain>("Select * From [CMS].[LandingDomain] Where LandingSiteId=@landingSiteId",
                    new {landingSiteId}).ToList();
        }

        public LpDomain Get(string domainUrl)
        {
            return
                SQLDataAccess.Query<LpDomain>("Select * From [CMS].[LandingDomain] Where DomainUrl=@domainUrl",
                    new { domainUrl }).FirstOrDefault();
        }

    }
}
