using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AdvantShop.Core.Common.Extensions;

namespace AdvantShop.Web.Admin.Models.Settings.ShippingReplaceGeo
{
    public class ShippingReplaceGeoModel : IValidatableObject
    {
        public int Id { get; set; }
        public string ShippingType { get; set; }
        public string InCountryName { get; set; }
        public string InCountryISO2 { get; set; }
        public string InRegionName { get; set; }
        public string InCityName { get; set; }
        public string InDistrict { get; set; }
        public string InZip { get; set; }
        public string OutCountryName { get; set; }
        public string OutRegionName { get; set; }
        public string OutCityName { get; set; }
        public string OutDistrict { get; set; }
        public bool OutDistrictClear { get; set; }
        public string OutZip { get; set; }
        public bool Enabled { get; set; }
        public int Sort { get; set; }
        public string Comment { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (ShippingType.IsNullOrEmpty())
                yield return new ValidationResult("Укажите тип доставки");

            if (InCountryName.IsNullOrEmpty() &&
                InCountryISO2.IsNullOrEmpty() &&
                InRegionName.IsNullOrEmpty() &&
                InDistrict.IsNullOrEmpty() &&
                InCityName.IsNullOrEmpty() &&
                InZip.IsNullOrEmpty())
                yield return new ValidationResult("Укажите входные параметры");

            if (OutCountryName.IsNullOrEmpty() &&
                OutRegionName.IsNullOrEmpty() &&
                OutDistrict.IsNullOrEmpty() &&
                OutDistrictClear == false &&
                OutCityName.IsNullOrEmpty() &&
                OutZip.IsNullOrEmpty())
                yield return new ValidationResult("Укажите выходные параметры");

            if (OutCountryName.IsNotEmpty() &&
                InCountryName.IsNullOrEmpty() &&
                InCountryISO2.IsNullOrEmpty())
                yield return new ValidationResult("Заменять страну можно, когда обозначена страна во входных данных");

            if (OutRegionName.IsNotEmpty() &&
                InRegionName.IsNullOrEmpty())
                yield return new ValidationResult("Заменять регион можно, когда обозначен регион во входных данных");

            if (OutDistrict.IsNotEmpty() &&
                InDistrict.IsNullOrEmpty() &&
                InCityName.IsNullOrEmpty())
                yield return new ValidationResult("Заменять район региона можно, когда обозначен район региона или город во входных данных");

            if (OutDistrictClear &&
                InCountryName.IsNullOrEmpty() &&
                InCountryISO2.IsNullOrEmpty() &&
                InRegionName.IsNullOrEmpty() &&
                InDistrict.IsNullOrEmpty() &&
                InCityName.IsNullOrEmpty() &&
                InZip.IsNullOrEmpty())
                yield return new ValidationResult("Заменять очищать район региона можно, когда обозначены входные данных");

            if (OutCityName.IsNotEmpty() &&
                InCityName.IsNullOrEmpty())
                yield return new ValidationResult("Заменять город можно, когда обозначен город во входных данных");

            if (OutZip.IsNotEmpty() &&
                InZip.IsNullOrEmpty() &&
                InCityName.IsNullOrEmpty())
                yield return new ValidationResult("Заменять индекс можно, когда обозначен индекс или город во входных данных");
        }

        public static ShippingReplaceGeoModel CreateFromShippingReplaceGeo(Shipping.ShippingReplaceGeo replace)
        {
            if (replace == null)
                return null;

            return new ShippingReplaceGeoModel
            {
                Id = replace.Id,
                ShippingType = replace.ShippingType,
                InCountryName = replace.InCountryName,
                InCountryISO2 = replace.InCountryISO2,
                InRegionName = replace.InRegionName,
                InCityName = replace.InCityName,
                InDistrict = replace.InDistrict,
                InZip = replace.InZip,
                OutCountryName = replace.OutCountryName,
                OutRegionName = replace.OutRegionName,
                OutCityName = replace.OutCityName,
                OutDistrict = replace.OutDistrict,
                OutDistrictClear = replace.OutDistrictClear,
                OutZip = replace.OutZip,
                Enabled = replace.Enabled,
                Sort = replace.Sort,
                Comment = replace.Comment,
            };
        }
    }
}
