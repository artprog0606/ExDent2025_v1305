using System;
using System.Collections.Generic;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Core.Services.Loging.Emails;
using AdvantShop.Core.Services.Mails;
using AdvantShop.Core.Services.Mails.Analytics;
using AdvantShop.Web.Admin.Models.Marketing.Emailings;
using AdvantShop.Web.Infrastructure.Admin;

namespace AdvantShop.Web.Admin.Handlers.Marketing.Emailings
{
    public class GetEmailingLogHandler
    {
        private EmailingLogFilterModel _filterModel;

        public GetEmailingLogHandler(EmailingLogFilterModel filterModel)
        {
            _filterModel = filterModel;
        }

        public FilterResult<AdvantShopMail> Execute()
        {
            var data = AdvantShopMailService.GetEmailingLog(new GetEmailingLogDto
            {
                Page = _filterModel.Page > 0 ? _filterModel.Page : 1,
                ItemsPerPage = _filterModel.ItemsPerPage,
                EmailingId = _filterModel.EmailingId,
                DateFrom = _filterModel.DateFrom,
                DateTo = _filterModel.DateTo,
                Statuses = _filterModel.Statuses
            });

            var result = new FilterResult<AdvantShopMail>
            {
                TotalItemsCount = data.TotalItemsCount
            };
            result.TotalPageCount = (int)Math.Ceiling((decimal)result.TotalItemsCount / _filterModel.ItemsPerPage);
            result.TotalString = LocalizationService.GetResourceFormat("Admin.Grid.FildTotal", result.TotalItemsCount);
            result.DataItems = data.DataItems;

            return result;
        }
    }
}