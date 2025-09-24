using System.Collections.Generic;
using AdvantShop.Catalog;
using AdvantShop.Core;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Customers;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Handlers.ProductDetails
{
    public class GetPriceAmountList : AbstractCommandHandler<List<PriceRuleAmountListItem>>
    {
        private readonly int _productId;
        private readonly int _offerId;

        private Product _product;

        public GetPriceAmountList(int productId, int offerId)
        {
            _productId = productId;
            _offerId = offerId;
        }

        protected override void Validate()
        {
            _product = ProductService.GetProduct(_productId);
            if (_product == null)
                throw new BlException("Товар не найден");
        }

        protected override List<PriceRuleAmountListItem> Handle()
        {
            var items = 
                PriceRuleService.GetPriceRuleAmountListItems(_offerId, _product, CustomerContext.CurrentCustomer.CustomerGroup);

            return items;
        }
    }
}