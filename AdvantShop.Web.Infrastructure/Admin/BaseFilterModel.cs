using System.Collections.Generic;

namespace AdvantShop.Web.Infrastructure.Admin
{
    public enum FilterSortingType
    {
        None,
        Asc,
        Desc
    }

    public enum FilterOutputDataType
    {
        List,
        Csv,
        Xml
    }

    public class BaseFilterModel<T>
    {
        public BaseFilterModel(int itemsPerPage)
        {
            _itemsPerPage = itemsPerPage;
            
            Page = 1;
            OutputDataType = FilterOutputDataType.List;
            Ids = new List<T>();
        }

        public BaseFilterModel() : this(20)
        {
        }


        private int _page = 1;
        public int Page
        {
            get => _page;
            set => _page = value > 0 && value < int.MaxValue ? value : 1;
        }

        private bool _itemsPerPageChanged;

        private int _itemsPerPage;
        public int ItemsPerPage
        {
            get => _itemsPerPage;
            set 
            { 
                _itemsPerPage = value;
                _itemsPerPageChanged = true;
            }
        }

        public bool IsDefaultItemsPerPage => !_itemsPerPageChanged;
        
        public string Sorting { get; set; }

        public string Search { get; set; }

        public FilterSortingType SortingType { get; set; }

        public FilterOutputDataType OutputDataType { get; set; }


        public List<T> Ids { get; set; }
        public SelectModeCommand SelectMode { get; set; }
    }

    public class BaseFilterModel : BaseFilterModel<int>
    {

    }
}