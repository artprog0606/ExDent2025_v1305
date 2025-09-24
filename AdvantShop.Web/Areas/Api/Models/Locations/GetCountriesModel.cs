using System.Collections.Generic;

namespace AdvantShop.Areas.Api.Models.Locations
{
    public sealed class GetCountriesModel
    {
        public List<int> Ids { get; set; }
        public List<string> Names { get; set; }
        public bool? DisplayInPopup { get; set; }
    }
}