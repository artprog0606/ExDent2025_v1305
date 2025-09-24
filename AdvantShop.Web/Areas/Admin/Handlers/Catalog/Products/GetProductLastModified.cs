using AdvantShop.Catalog;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.ChangeHistories;
using AdvantShop.Customers;

namespace AdvantShop.Web.Admin.Handlers.Catalog.Products
{
    public class GetProductLastModified
    {
        private readonly int _productId;

        public GetProductLastModified(int productId)
        {
            _productId = productId;
        }

        public ProductLastModifiedModel Execute()
        {
            var model = new ProductLastModifiedModel();

            var lastChanges = ChangeHistoryService.GetLast(_productId, ChangeHistoryObjType.Product);
            if (lastChanges == null)
            {
                var changedBy = ProductService.GetModifiedDateByProduct(_productId);
                if (changedBy != null)
                {
                    model.ModifiedDate = changedBy.ModificationTime.ToString("dd.MM.yy HH:mm");
                    model.ModifiedBy = changedBy.Name;

                    var customerId = !string.IsNullOrEmpty(changedBy.Name) ? changedBy.Name.TryParseGuid(true) : null;
                    if (customerId != null)
                    {
                        var modifiedByCustomer = CustomerService.GetCustomer(customerId.Value);
                        if (modifiedByCustomer != null)
                            model.ModifiedBy = modifiedByCustomer.GetShortName();
                    }
                    return model;
                }
                return null;
            }


            model.ModifiedDate = lastChanges.ModificationTime.ToString("dd.MM.yy HH:mm");

            if (lastChanges.ChangedById != null)
            {
                var modifiedByCustomer = CustomerService.GetCustomer(lastChanges.ChangedById.Value);
                if (modifiedByCustomer != null)
                    model.ModifiedBy = modifiedByCustomer.GetShortName();
            }
            else
            {
                model.ModifiedBy = lastChanges.ChangedByName;
            }

            return model;
        }
    }

    public class ProductLastModifiedModel
    {
        public string ModifiedDate { get; set; }
        public string ModifiedBy { get; set; }
    }
}