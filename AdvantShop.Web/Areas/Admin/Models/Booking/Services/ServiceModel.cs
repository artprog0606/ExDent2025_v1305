using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Booking;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.FilePath;
using AdvantShop.Repository.Currencies;

namespace AdvantShop.Web.Admin.Models.Booking.Services
{
    public class ServiceModel : IValidatableObject
    {
        public int Id { get; set; }
        public string ArtNo { get; set; }
        public int? AffiliateId { get; set; }
        public bool BindAffiliate { get; set; }
        public int CategoryId { get; set; }
        public int CurrencyId { get; set; }
        public string Name { get; set; }
        public float Price { get; set; }

        public string PriceString
        {
            get
            {
                return PriceFormatService.FormatPrice(Price, CurrencyService.GetCurrency(CurrencyId, true));
            }
        }
        public string Description { get; set; }
        public int SortOrder { get; set; }
        public DateTime? Duration { get; set; }
        public bool Enabled { get; set; }
        public string Image { get; set; }
        public string PhotoSrc
        {
            get
            {
                return Image.IsNotEmpty()
                    ? FoldersHelper.GetPath(FolderType.BookingService, Image, false)
                    : string.Empty;
            }
        }

        public HttpPostedFileBase PhotoFile { get; set; }

        public static explicit operator ServiceModel(Service service)
        {
            return new ServiceModel()
            {
                Id = service.Id,
                ArtNo = service.ArtNo,
                CategoryId = service.CategoryId,
                CurrencyId = service.CurrencyId,
                Name = service.Name,
                Price = service.BasePrice,
                Description = service.Description,
                SortOrder = service.SortOrder,
                Enabled = service.Enabled,
                Image = service.Image,
                Duration = service.Duration.HasValue ? new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day) + service.Duration : null,
            };
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (CategoryId <= 0 || CategoryService.Get(CategoryId) == null)
                yield return new ValidationResult("Указанная категория отсутствует");

            if (AffiliateId.HasValue && AffiliateService.Get(AffiliateId.Value) == null)
                yield return new ValidationResult("Указанный филиал отсутствует");

            if (CurrencyId <= 0 || CurrencyService.GetCurrency(CurrencyId, true) == null)
                yield return new ValidationResult("Указанная валюта отсутствует");

            if (string.IsNullOrWhiteSpace(Name))
                yield return new ValidationResult("Укажите название");

            if (Id > 0 && string.IsNullOrWhiteSpace(ArtNo))
                yield return new ValidationResult("Укажите артикул");

            if (!string.IsNullOrWhiteSpace(ArtNo) && ServiceService.ExistsArtNo(ArtNo, Id))
                yield return new ValidationResult("Артикул занят");
        }
    }
}
