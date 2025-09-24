namespace AdvantShop.Web.Infrastructure.Api
{
    public enum FilterSortingType
    {
        None,
        Asc,
        Desc
    }

    public class EntitiesFilterModel
    {
        public EntitiesFilterModel()
        {
            MaxItemsPerPage = 100;
            DefaultItemsPerPage = 100;

            Page = 1;
            ItemsPerPage = DefaultItemsPerPage;
        }

        protected int DefaultItemsPerPage { get; set; }
        protected int MaxItemsPerPage { get; set; }

        private int _page;
        public int Page
        {
            get { return _page; }
            set { _page = value > 0 && value < int.MaxValue ? value : 1; }
        }

        public int _itemsPerPage;
        public int ItemsPerPage
        {
            get { return _itemsPerPage > 0 && _itemsPerPage <= MaxItemsPerPage ? _itemsPerPage : DefaultItemsPerPage; }
            set { _itemsPerPage = value; }
        }

        public string Sorting { get; set; }

        public FilterSortingType SortingType { get; set; }
    }
}
