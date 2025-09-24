using System.Collections.Generic;
using AdvantShop.Configuration;
using AdvantShop.Core.Caching;
using AdvantShop.Core.Services.Helpers;
namespace AdvantShop.Core.Services.ImageSearches
{

    public class PexelsImageSearch
    {
        private const int ItemsPerPage = 20;
        private readonly Dictionary<string, string> _headers = new Dictionary<string, string>(){{ "lic", SettingsLic.LicKey}};

        public PexelsSearchResponse Search(string term, int page = 1)
        {
            if (string.IsNullOrWhiteSpace(term))
                return null;

            if (page <= 0)
                page = 1;

            term = term.Trim();

            return CacheManager.Get("PexelsImageSearch_" + term + "_" + page + "_" + ItemsPerPage,
                () =>
                    RequestHelper.MakeRequest<PexelsSearchResponse>(SettingsLic.ImageServiceUrl + "v1/pexelsimage/find",
                        new {term, page, ItemsPerPage, lickey = SettingsLic.LicKey}, _headers))
                    ?? new PexelsSearchResponse();
        }

        public PexelsSearchResponse GetPopular(int page = 1)
        {
            if (page <= 0)
                page = 1;

            return CacheManager.Get("PexelsImageSearch_popular_" + page + "_" + ItemsPerPage,
                () =>
                    RequestHelper.MakeRequest<PexelsSearchResponse>(SettingsLic.ImageServiceUrl + "v1/pexelsimage/popular",
                        new {page, ItemsPerPage, lickey = SettingsLic.LicKey}, _headers))
                    ?? new PexelsSearchResponse();
        }
    }
}
