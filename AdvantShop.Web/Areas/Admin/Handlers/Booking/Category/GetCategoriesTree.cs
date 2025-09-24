using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Booking;
using AdvantShop.Web.Admin.Models.Booking.Category;
using AdvantShop.Web.Admin.ViewModels.Catalog;

namespace AdvantShop.Web.Admin.Handlers.Booking.Category
{
    public class GetCategoriesTree
    {
        private readonly int? _affiliateId;
        private readonly int? _reservationResourceId;
        private readonly string _id;
        private readonly int _categoryIdSelected;
        private readonly List<int> _excludeIds;
        private readonly List<int> _selectedIds;
        private readonly bool _showRoot;

        public GetCategoriesTree(CategoriesTree model)
        {
            _affiliateId = model.AffiliateId;
            _reservationResourceId = model.ReservationResourceId;
            _id = model.Id;
            _categoryIdSelected = model.CategoryIdSelected != null ? model.CategoryIdSelected.Value : -1;
            _excludeIds = model.ExcludeIds != null ? model.ExcludeIds.Split(',').Select(x => x.TryParseInt()).ToList() : new List<int>();
            _selectedIds = model.SelectedIds != null ? model.SelectedIds.Split(',').Select(x => x.TryParseInt()).ToList() : new List<int>();

            _showRoot = model.ShowRoot;
        }

        public List<AdminCatalogTreeViewItem> Execute()
        {
            if (_showRoot)
            {
                return new List<AdminCatalogTreeViewItem>()
                {
                    new AdminCatalogTreeViewItem()
                    {
                        id = "0",
                        parent = "#",
                        text = String.Format("<span class=\"jstree-advantshop-name\">{0}</span>", "Корень"),
                        name = "Корень",
                        children = true,
                        state = new AdminCatalogTreeViewItemState()
                        {
                            opened = true,
                            selected = _categoryIdSelected == 0 || _selectedIds.Contains(0)
                        },
                        li_attr = new Dictionary<string, string>() {
                            { "data-tree-id", "categoryItemId_" +  0}
                        },
                    }
                };
            }

            var categories = _affiliateId.HasValue ? CategoryService.GetList(_affiliateId.Value) : CategoryService.GetList();

            if (_affiliateId.HasValue && _reservationResourceId.HasValue)
            {
                var reservationResourceCategories = ServiceService.GetListCategoryIdsByReservationResourceServices(_affiliateId.Value, _reservationResourceId.Value);
                categories = categories.Where(x => reservationResourceCategories.Contains(x.Id)).ToList();
            }

            return categories.Where(x => !_excludeIds.Contains(x.Id)).Select(x => new AdminCatalogTreeViewItem()
            {
                id = x.Id.ToString(),
                parent = _id == "#" ? "#" : "0",
                text =
                    String.Format("<span class=\"jstree-advantshop-name\">{0}</span>", x.Name),
                name = x.Name,
                children = false,
                state = new AdminCatalogTreeViewItemState()
                {
                    opened = false,
                    selected = x.Id == _categoryIdSelected || _selectedIds.Contains(x.Id)
                },
                li_attr = new Dictionary<string, string>() {
                        { "data-tree-id", "categoryItemId_" +  x.Id},
                        { "class", "jstree-leaf"} //jstree-leaf - hide open icon 
                },
            }).ToList();
        }
    }
}
