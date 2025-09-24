using AdvantShop.Core.Services.Crm.Ok.OkMarket;
using System.Collections.Generic;
using System.Linq;

namespace AdvantShop.Web.Admin.Models.OkMarket
{
    public class OkMarketCatalogModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<int> CategoryIds { get; set; }
        public string Categories { get; set; }
        public long OkCatalogId { get; set; }
        public OkMarketCatalogModel() { }
        public OkMarketCatalogModel(OkMarketCatalog model)
        {
            Id = model.Id;
            Name = model.Name;
            OkCatalogId = model.OkCatalogId;
            var cats = OkMarketService.GetLinkedCategories(model.Id).Select(x => x.Name);
            if (cats.Count() > 10)
            {
                Categories = string.Join(", ", cats.Take(10)) + " ...";
            }
            else
            {
                Categories = string.Join(", ", cats);
            }
        }
    }
}