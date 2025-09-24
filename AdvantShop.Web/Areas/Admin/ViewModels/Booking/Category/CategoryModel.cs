using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Booking;
using AdvantShop.FilePath;

namespace AdvantShop.Web.Admin.ViewModels.Booking.Category
{
    public class CategoryModel : IValidatableObject
    {
        public int Id { get; set; }
        public int? AffiliateId { get; set; }
        public string Name { get; set; }
        public int SortOrder { get; set; }
        public bool Enabled { get; set; }
        public string Image { get; set; }
        public HttpPostedFileBase PhotoFile { get; set; }
        public string PhotoSrc
        {
            get
            {
                return Image.IsNotEmpty()
                    ? string.Format("{0}?rnd={1}", FoldersHelper.GetPath(FolderType.BookingCategory, Image, false), new Random().Next())
                    : string.Empty;
            }
        }

        public static explicit operator CategoryModel(Core.Services.Booking.Category category)
        {
            return new CategoryModel()
            {
                Id = category.Id,
                Name = category.Name,
                SortOrder = category.SortOrder,
                Enabled = category.Enabled,
                Image = category.Image,
            };
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (AffiliateId.HasValue && AffiliateService.Get(AffiliateId.Value) == null)
                yield return new ValidationResult("Указанный филиал отсутствует");

            if (string.IsNullOrWhiteSpace(Name))
                yield return new ValidationResult("Укажите название");
        }
    }
}
