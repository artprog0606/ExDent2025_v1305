using System;
using System.Linq;
using System.Web.Mvc;
using AdvantShop.Core;
using AdvantShop.Core.Modules;
using AdvantShop.Core.Services.SalesChannels;
using AdvantShop.Web.Infrastructure.Extensions;
using AdvantShop.Web.Infrastructure.Filters;

namespace AdvantShop.Web.Admin.Controllers.Shared
{
    public partial class SalesChannelsController : BaseAdminController
    {
        [HttpGet]
        public JsonResult GetList()
        {
            var items = SalesChannelService.GetList().Where(x => x.IsShowInList());
            return Json(items);
        }

        [HttpGet]
        public JsonResult GetItem(ESalesChannelType type)
        {
            var item = SalesChannelService.GetByType(type);
            return Json(item);
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult Add(ESalesChannelType type, string moduleStringId)
        {
            try
            {
                var salesChannel = SalesChannelService.GetByType(type, moduleStringId);
                if (salesChannel == null)
                    throw new BlException("Канал продаж не найден");

                if (salesChannel.Type == ESalesChannelType.Module &&
                    AttachedModules.GetModuleById(salesChannel.ModuleStringId) == null)
                {
                    return JsonOk(new {url = "modules/market?name=" + salesChannel.ModuleStringId.ToLower()});
                }

                salesChannel.Enabled = true;

                var menuUrl = salesChannel.MenuUrlAction;
                var url = menuUrl != null
                    ? Url.AbsoluteActionUrl(menuUrl.Action, menuUrl.Controller, menuUrl.RouteDictionary)
                    : salesChannel.Url;

                switch (salesChannel.Type)
                {
                    case ESalesChannelType.Bonus:
                        Track.TrackService.TrackEvent(Track.ETrackEvent.SalesChannels_BonusSalesChannelAdded);
                        break;
                    case ESalesChannelType.Vk:
                        Track.TrackService.TrackEvent(Track.ETrackEvent.SalesChannels_VkSalesChannelAdded);
                        break;
                    case ESalesChannelType.OzonSeller:
                        Track.TrackService.TrackEvent(Track.ETrackEvent.SalesChannels_OzonSalesChannelAdded);
                        break;
                    case ESalesChannelType.Yandex:
                        Track.TrackService.TrackEvent(Track.ETrackEvent.SalesChannels_YandexSalesChannelAdded);
                        break;
                }

                return JsonOk(new {url});
            }
            catch (Exception ex)
            {
                return JsonError(ex.Message);
            }
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult Delete(ESalesChannelType type, string moduleStringId)
        {
            var salesChannel = SalesChannelService.GetByType(type, moduleStringId);
            if (salesChannel != null)
                salesChannel.Enabled = false;

            if (salesChannel != null && salesChannel.Type == ESalesChannelType.Module)
                return JsonOk(new { url = "modules/market?name=" + salesChannel.ModuleStringId.ToLower() });

            return JsonOk();
        }

    }
}
