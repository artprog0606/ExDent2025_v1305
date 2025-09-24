using System.Collections.Generic;

namespace AdvantShop.Web.Infrastructure.Api
{
    public class EntitiesFilterResult<TModel>
    {
        public List<TModel> DataItems { get; set; }

        public int TotalPageCount { get; set; }
        public int TotalItemsCount { get; set; }
        public int PageIndex { get; set; }
        public int ItemsPerPage { get; set; }
    }
}
