using System;

namespace AdvantShop.Core.Services.Catalog.Warehouses
{
    public class TypeWarehouse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int SortOrder { get; set; }
        public bool Enabled { get; set; }
        public DateTime DateAdded { get; set; }

        public DateTime DateModified { get; set; }
    }
}