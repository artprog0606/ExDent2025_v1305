using System.Collections.Generic;
using AdvantShop.Core.Services.Api;

namespace AdvantShop.Areas.Api.Models.Locations
{
    public class GetCountriesResponse : IApiResponse
    {
        public List<LocationCountry> Countries { get; set; }
    }
}