using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Areas.Api.Models.Locations;
using AdvantShop.Repository;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Areas.Api.Handlers.Locations
{
    public class GetCountriesApi : AbstractCommandHandler<GetCountriesResponse>
    {
        private readonly GetCountriesModel _filter;

        public GetCountriesApi(GetCountriesModel filter)
        {
            _filter = filter ?? new GetCountriesModel();
        }
        
        protected override GetCountriesResponse Handle()
        {
            var allCountries = CountryService.GetAllCountries() ?? new List<Country>();
            
            var countries = new List<Country>(allCountries).AsEnumerable();

            if (_filter.Ids != null && _filter.Ids.Count > 0)
                countries = countries.Where(x => _filter.Ids.Contains(x.CountryId));

            if (_filter.Names != null && _filter.Names.Count > 0)
                countries = countries.Where(x => _filter.Names.Contains(x.Name, StringComparer.OrdinalIgnoreCase));

            if (_filter.DisplayInPopup != null)
                countries = countries.Where(x => x.DisplayInPopup == _filter.DisplayInPopup.Value);
            
            
            var response = new GetCountriesResponse()
            {
                Countries = countries.Select(x => new LocationCountry(x)).ToList()
            };

            return response;
        }
    }
}