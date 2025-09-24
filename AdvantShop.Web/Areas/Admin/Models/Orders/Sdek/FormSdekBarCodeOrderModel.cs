using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace AdvantShop.Web.Admin.Models.Orders.Sdek
{
    public class FormSdekBarCodeOrderModel : IValidatableObject
    {
        public int OrderId { get; set; }
        public int? CopyCount { get; set; }
        public string Format { get; set; }
        public List<SelectListItem> Formats { get; set; }
        public string Lang { get; set; }
        public List<SelectListItem> Langs { get; set; }
        
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (OrderId <= 0)
                yield return new ValidationResult("Неизвестный заказ");
        }
    }
}