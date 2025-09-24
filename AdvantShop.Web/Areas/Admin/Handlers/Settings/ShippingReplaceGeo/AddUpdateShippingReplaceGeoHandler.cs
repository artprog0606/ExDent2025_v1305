using System.Collections.Generic;
using AdvantShop.Web.Admin.Models.Settings.ShippingReplaceGeo;

namespace AdvantShop.Web.Admin.Handlers.Settings.ShippingReplaceGeo
{
    public class AddUpdateShippingReplaceGeoHandler
    {
        private readonly ShippingReplaceGeoModel _model;

        public List<string> Errors { get; set; }

        public AddUpdateShippingReplaceGeoHandler(ShippingReplaceGeoModel model)
        {
            _model = model;
            Errors = new List<string>();
        }

        public bool Execute()
        {
            var replace = Shipping.ShippingReplaceGeoService.Get(_model.Id) ?? new Shipping.ShippingReplaceGeo();
            var isNew = replace.Id == 0;

            replace.ShippingType = _model.ShippingType ?? string.Empty;
            replace.InCountryName = _model.InCountryName ?? string.Empty;
            replace.InCountryISO2 = _model.InCountryISO2 ?? string.Empty;
            replace.InRegionName = _model.InRegionName ?? string.Empty;
            replace.InCityName = _model.InCityName ?? string.Empty;
            replace.InDistrict = _model.InDistrict ?? string.Empty;
            replace.InZip = _model.InZip ?? string.Empty;
            replace.OutCountryName = _model.OutCountryName ?? string.Empty;
            replace.OutRegionName = _model.OutRegionName ?? string.Empty;
            replace.OutCityName = _model.OutCityName ?? string.Empty;
            replace.OutDistrict = _model.OutDistrict ?? string.Empty;
            replace.OutDistrictClear = _model.OutDistrictClear;
            replace.OutZip = _model.OutZip ?? string.Empty;
            replace.Enabled = _model.Enabled;
            replace.Sort = _model.Sort;
            replace.Comment = _model.Comment;

            if (replace.Id <= 0)
                replace.Id = Shipping.ShippingReplaceGeoService.Add(replace);
            else
                Shipping.ShippingReplaceGeoService.Update(replace);

            return Errors.Count == 0;
        }
    }
}
