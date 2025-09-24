using System;
namespace AdvantShop.Web.Admin.Models.Customers.CustomerTags
{
    public class CustomerTagsModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool Enabled { get; set; }
        public int SortOrder { get; set; }
    }
}
