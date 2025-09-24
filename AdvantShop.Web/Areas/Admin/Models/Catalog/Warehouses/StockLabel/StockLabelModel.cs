using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AdvantShop.Web.Admin.Models.Catalog.Warehouses.StockLabel
{
    public class StockLabelModel : IValidatableObject
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ClientName { get; set; }
        public string Color { get; set; }
        public float AmountUpTo { get; set; }
        
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(Name))
                yield return new ValidationResult("Укажите название");
            if (string.IsNullOrWhiteSpace(ClientName))
                yield return new ValidationResult("Укажите название в клиентской части");
            if (string.IsNullOrWhiteSpace(Color))
                yield return new ValidationResult("Укажите цвет");
        }

    }
}