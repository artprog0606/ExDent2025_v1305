using AdvantShop.Catalog;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Localization;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AdvantShop.Web.Admin.Models.Catalog.SizeChart
{
    public class SizeChartModel : IValidatableObject
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ModalHeader { get; set; }
        public string LinkText { get; set; }
        public string Text { get; set; }
        public ESizeChartSourceType SourceType { get; set; }
        public int SortOrder { get; set; }
        public bool Enabled { get; set; }
        public List<int> ProductIds { get; set; }
        public List<int> CategoryIds { get; set; }
        public List<int> BrandIds { get; set; }
        public List<SizeChartPropertyValueModel> PropertyValues { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Name.IsNullOrEmpty() || Text.IsNullOrEmpty() || LinkText.IsNullOrEmpty())
                yield return new ValidationResult(LocalizationService.GetResource("Core.Catalog.SizeChart.Settings.RequiredFieldIsEmpty"));
        }
    }
}
