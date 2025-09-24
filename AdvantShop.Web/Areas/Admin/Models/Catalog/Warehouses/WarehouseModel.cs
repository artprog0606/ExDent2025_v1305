using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Core.UrlRewriter;
using Newtonsoft.Json;

namespace AdvantShop.Web.Admin.Models.Catalog.Warehouses
{
    public class WarehouseModel : IValidatableObject
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string UrlPath { get; set; }

        public string Description { get; set; }
        public int? TypeId { get; set; }
        public string TypeName { get; set; }

        public int SortOrder { get; set; }

        public bool Enabled { get; set; }

        public int? CityId { get; set; }
        public string City { get; set; }
        public string CityDescription { get; set; }

        public string Address { get; set; }

        public string Longitude { get; set; }

        public string Latitude { get; set; }

        public string AddressComment { get; set; }

        public string Phone { get; set; }

        public string Phone2 { get; set; }

        public string Email { get; set; }
        public bool DefaultMeta { get; set; }
        public string SeoTitle { get; set; }
        public string SeoH1 { get; set; }
        public string SeoKeywords { get; set; }
        public string SeoDescription { get; set; }
        public DateTime DateAdded { get; set; }

        public DateTime DateModified { get; set; }
        public string ModifiedBy { get; set; }
        public bool CanDeleting { get; set; }
        
        public List<TimeOfWorkModel> TimesOfWork { get; set; }

        public string TimesOfWorkJson
        {
            get => JsonConvert.SerializeObject(TimesOfWork);
            set => TimesOfWork = value.IsNullOrEmpty() ? null : JsonConvert.DeserializeObject<List<TimeOfWorkModel>>(value);
        }
        
        public string WarehouseCitiesJson { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrEmpty(Name))
            {
                yield return new ValidationResult(LocalizationService.GetResource("Admin.Warehouse.AdminWarehouseModel.Error.RequiredName") /* todo: localize */, new[] { "Name" });
            }

            if ((Longitude.IsNotEmpty()
                 || Latitude.IsNotEmpty())
                && (Longitude.IsNullOrEmpty()
                    || Latitude.IsNullOrEmpty()))
            {
                yield return new ValidationResult("Укажите полные координаты" /* todo: localize */, new[] { "Longitude", "Latitude" });
            }

            if (string.IsNullOrEmpty(UrlPath))
            {
                yield return new ValidationResult(LocalizationService.GetResource("Admin.Warehouse.AdminWarehouseModel.Error.Url") /* todo: localize */, new[] { "Url" });
            }
            else
            {
                // if new category or urlpath != previous urlpath
                if (Id == 0 || (UrlService.GetObjUrlFromDb(ParamType.Warehouse, Id) != UrlPath))
                {
                    if (!UrlService.IsValidUrl(UrlPath, ParamType.Warehouse))
                    {
                        UrlPath = UrlService.GetAvailableValidUrl(0, ParamType.Warehouse, UrlPath);
                    }
                }
            }

            if (SeoTitle != null || SeoKeywords != null || SeoDescription != null || SeoH1 != null)
            {
                SeoTitle = SeoTitle ?? "";
                SeoKeywords = SeoKeywords ?? "";
                SeoDescription = SeoDescription ?? "";
                SeoH1 = SeoH1 ?? "";
            }

            if (TimesOfWork != null)
            {
                var culture = AdvantShop.Localization.Culture.GetCulture();
                var existDayOfWeed = new HashSet<DayOfWeek>();
                
                foreach (var timeOfWorkModel in TimesOfWork)
                {
                    if (timeOfWorkModel.DayOfWeeks != null
                        && timeOfWorkModel.DayOfWeeks.Count > 0)
                    {
                        foreach (var dayOfWeek in timeOfWorkModel.DayOfWeeks)
                        {
                            if (existDayOfWeed.Contains(dayOfWeek))
                                yield return new ValidationResult(string.Format("День недели \"{0}\" повторяется", culture.DateTimeFormat.GetDayName(dayOfWeek)) /* todo: localize */, new[] {"TimesOfWork"});
                            else
                                existDayOfWeed.Add(dayOfWeek);
                        }
                    }
                    else
                    {
                        yield return new ValidationResult("Выберите день недели" /* todo: localize */, new[] {"TimesOfWork"});
                    }
                    
                    if ((timeOfWorkModel.BreakStartTime.IsNotEmpty()
                         || timeOfWorkModel.BreakEndTime.IsNotEmpty())
                        && (timeOfWorkModel.BreakStartTime.IsNullOrEmpty()
                            || timeOfWorkModel.BreakEndTime.IsNullOrEmpty()))
                        yield return new ValidationResult("Перерыв указан не полностью" /* todo: localize */, new[] { "TimesOfWork" });
                    
                    if ((timeOfWorkModel.OpeningTime.IsNotEmpty()
                         || timeOfWorkModel.ClosingTime.IsNotEmpty())
                        && (timeOfWorkModel.OpeningTime.IsNullOrEmpty()
                            || timeOfWorkModel.ClosingTime.IsNullOrEmpty()))
                        yield return new ValidationResult("Время работы указано не полностью" /* todo: localize */, new[] { "TimesOfWork" });
                    
                    if ((timeOfWorkModel.BreakStartTime.IsNotEmpty()
                         || timeOfWorkModel.BreakEndTime.IsNotEmpty())
                        && (timeOfWorkModel.OpeningTime.IsNullOrEmpty()
                            || timeOfWorkModel.ClosingTime.IsNullOrEmpty()))
                        yield return new ValidationResult("Перерыв не может быть указан без указания времени работы" /* todo: localize */, new[] { "TimesOfWork" });
                }
            }
        }
    }
}