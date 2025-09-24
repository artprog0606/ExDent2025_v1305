using System;
using AdvantShop.Core.Services.Bonuses.Model;
using AdvantShop.Core.SQL2;
using AdvantShop.Web.Admin.Models.Bonuses.Cards;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Web.Admin.Handlers.Bonuses.Cards
{
    public class GetHistoryPercentHandler : AbstractHandler<PersenthistoryFilterModel, int, PersentHistory>
    {
        public GetHistoryPercentHandler(PersenthistoryFilterModel filterModel) : base(filterModel)
        {
        }

        protected override SqlPaging Select(SqlPaging paging)
        {
            paging.Select("Id", "CardId", "GradeName", "BonusPersent", "CreateOn", "ByAction");
            paging.From("[Bonus].[PersentHistory]");
            return paging;
        }

        protected override SqlPaging Filter(SqlPaging paging)
        {
            if (FilterModel.CardId != Guid.Empty)
            {
                paging.Where("CardId = {0}", FilterModel.CardId);
            }
            return paging;
        }

        protected override SqlPaging Sorting(SqlPaging paging)
        {
            paging.OrderByDesc("CreateOn");
            return paging;
        }
    }
}