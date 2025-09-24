using System;
using System.Linq;
using System.Web.Mvc;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Booking;
using AdvantShop.Core.Services.Localization;
using AdvantShop.FilePath;
using AdvantShop.Helpers;
using AdvantShop.Repository.Currencies;
using AdvantShop.Web.Admin.Handlers.Booking.Services;
using AdvantShop.Web.Admin.Models;
using AdvantShop.Web.Admin.Models.Booking.Services;
using AdvantShop.Web.Infrastructure.Admin;
using AdvantShop.Web.Infrastructure.Filters;

namespace AdvantShop.Web.Admin.Controllers.Booking
{
    public partial class BookingServiceController : BaseBookingController
    {
        public JsonResult GetServices(ServicesFilterModel model)
        {
            return Json(new GetServicesHandler(model).Execute());
        }

        public JsonResult GetServiceData(int? categoryId)
        {
            return Json(new
            {
                currencies = CurrencyService.GetAllCurrencies().Select(x => new SelectItemModel(x.Name, x.CurrencyId)),
                maxSortOrder = (categoryId.HasValue ? ServiceService.GetList(categoryId.Value).Select(x => x.SortOrder).DefaultIfEmpty().Max() : 0) + 10
            });
        }

        public JsonResult GetService(int serviceId)
        {
            var service = ServiceService.Get(serviceId);
            if (service == null)
                return JsonError("Указанная услуга отсутствует");

            return JsonOk((ServiceModel)service);
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult Add(ServiceModel model)
        {
            if (ModelState.IsValid)
            {
                var id = ServiceService.Add(new Service()
                {
                    ArtNo = model.ArtNo,
                    Name = model.Name,
                    CategoryId = model.CategoryId,
                    CurrencyId = model.CurrencyId,
                    Description = model.Description,
                    Enabled = model.Enabled,
                    BasePrice = model.Price,
                    SortOrder = model.SortOrder,
                    Duration = model.Duration.HasValue ? model.Duration.Value.TimeOfDay : (TimeSpan?)null
                });

                if (model.PhotoFile != null)
                    new UploadImageCropped(ServiceService.Get(id), model.Image, model.PhotoFile).Execute();

                if (model.AffiliateId.HasValue && AffiliateService.CheckAccessToEditing(model.AffiliateId.Value))
                    ServiceService.AddRefAffiliate(id, model.AffiliateId.Value);

                Track.TrackService.TrackEvent(Track.ETrackEvent.Core_Booking_ServiceCreated);

                return JsonOk();
            }

            return JsonError();
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult Update(ServiceModel model)
        {
            if (ModelState.IsValid)
            {
                var service = ServiceService.Get(model.Id);
                if (service == null)
                    return JsonError("Услуга отсутствует");

                service.ArtNo = model.ArtNo;
                service.Name = model.Name;
                service.CurrencyId = model.CurrencyId;
                service.Description = model.Description;
                service.Enabled = model.Enabled;
                service.BasePrice = model.Price;
                service.SortOrder = model.SortOrder;
                service.Duration = model.Duration.HasValue ? model.Duration.Value.TimeOfDay : (TimeSpan?)null;

                ServiceService.Update(service);

                if (model.PhotoFile != null)
                    new UploadImageCropped(service, model.Image, model.PhotoFile).Execute();

                if (model.AffiliateId.HasValue && AffiliateService.CheckAccessToEditing(model.AffiliateId.Value))
                    ServiceService.AddRefAffiliate(service.Id, model.AffiliateId.Value);

                return JsonOk();
            }

            return JsonError();
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DeleteService(int id)
        {
            ServiceService.Delete(id);
            return JsonOk();
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DeleteServiceImage(int id)
        {
            var service = ServiceService.Get(id);
            if (service == null)
                return JsonError("Услуга отсутствует");

            FileHelpers.DeleteFile(FoldersHelper.GetPathAbsolut(FolderType.BookingService, service.Image));

            service.Image = null;
            ServiceService.Update(service);

            return JsonOk();
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult InplaceService(ServiceModel model)
        {
            if (model.Id != 0)
            {
                var service = ServiceService.Get(model.Id);
                if (service == null)
                    return JsonError("Услуга отсутствует");

                service.SortOrder = model.SortOrder;
                service.Enabled = model.Enabled;
                ServiceService.Update(service);

                if (model.AffiliateId.HasValue && AffiliateService.CheckAccessToEditing(model.AffiliateId.Value))
                {
                    if (model.BindAffiliate)
                        ServiceService.AddRefAffiliate(service.Id, model.AffiliateId.Value);
                    else
                        ServiceService.DeleteRefAffiliate(service.Id, model.AffiliateId.Value);
                }
                return JsonOk();
            }

            return JsonError();
        }

        private void Command(ServicesFilterModel model, Action<int, ServicesFilterModel> func)
        {
            if (model.SelectMode == SelectModeCommand.None)
            {
                foreach (var id in model.Ids)
                    func(id, model);
            }
            else
            {
                var ids = new GetServicesHandler(model).GetItemsIds();
                foreach (int id in ids)
                {
                    if (model.Ids == null || !model.Ids.Contains(id))
                        func(id, model);
                }
            }
        }

        public JsonResult GetServicesIds(ServicesFilterModel model)
        {
            return Json(new { ids = new GetServicesHandler(model).GetItemsIds() });
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DeleteServices(ServicesFilterModel model)
        {
            Command(model, (id, m) => ServiceService.Delete(id));
            return JsonOk();
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult BindAffiliate(ServicesFilterModel model)
        {
            var affiliateId = model.AffiliateId.HasValue
                ? model.AffiliateId.Value
                : model.LeftJoinAffiliateId.HasValue
                    ? model.LeftJoinAffiliateId.Value
                    : SelectedAffiliate.Id;

            if (AffiliateService.CheckAccessToEditing(affiliateId))
            {
                Command(model, (id, m) => ServiceService.AddRefAffiliate(id, affiliateId));
                return JsonOk();
            }
            return JsonError(LocalizationService.GetResource("Admin.Booking.NoAccess"));
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult UnBindAffiliate(ServicesFilterModel model)
        {
            var affiliateId = model.AffiliateId.HasValue
                ? model.AffiliateId.Value
                : model.LeftJoinAffiliateId.HasValue
                    ? model.LeftJoinAffiliateId.Value
                    : SelectedAffiliate.Id;

            if (AffiliateService.CheckAccessToEditing(affiliateId))
            {
                Command(model, (id, m) => ServiceService.DeleteRefAffiliate(id, affiliateId));
                return JsonOk();
            }
            return JsonError(LocalizationService.GetResource("Admin.Booking.NoAccess"));
        }
    }
}
