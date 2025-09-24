using System.Collections.Generic;

namespace AdvantShop.Web.Admin.Models.Settings.CatalogSettings
{
    public enum PriceRegulationAction
    {
        Decrement = 0,
        Increment = 1,
        IncBySupply = 2
    }

    public enum PriceRegulationValueOption
    {
        Percent = 0,
        AbsoluteValue = 1,
    }

    public class PriceRegulationModel
    {
        public bool ChooseProducts { get; set; }

        public List<int> CategoryIds { get; set; }

        public PriceRegulationAction Action { get; set; }

        public float Value { get; set; }

        public PriceRegulationValueOption ValueOption { get; set; }
    }

    public enum CategoryDiscountRegulationValueOption
    {
        Percent = 0,
        AbsoluteValue = 1,
    }

    public class CategoryDiscountRegulationModel
    {
        public bool ChooseProducts { get; set; }

        public List<int> CategoryIds { get; set; }

        public float Value { get; set; }

        public CategoryDiscountRegulationValueOption ValueOption { get; set; }
    }

    public class BrandDiscountRegulationModel
    {
        public List<int> BrandIds { get; set; }

        public float Value { get; set; }

        public CategoryDiscountRegulationValueOption ValueOption { get; set; }
    }
}
