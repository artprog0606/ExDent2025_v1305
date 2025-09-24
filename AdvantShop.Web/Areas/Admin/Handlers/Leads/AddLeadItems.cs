using System.Collections.Generic;
using AdvantShop.Catalog;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.ChangeHistories;
using AdvantShop.Core.Services.Crm;
using AdvantShop.Customers;

namespace AdvantShop.Web.Admin.Handlers.Leads
{
    public class AddLeadItems
    {
        private readonly Lead _lead;
        private readonly List<int> _offerIds;

        public AddLeadItems(Lead lead, List<int> offerIds)
        {
            _lead = lead;
            _offerIds = offerIds;
        }

        public bool Execute()
        {
            var saveChanges = false;

            var history = new List<ChangeHistory>();

            foreach (var offerId in _offerIds)
            {
                var offer = OfferService.GetOffer(offerId);
                if (offer == null)
                    continue;

                var product = offer.Product;

                var prodMinAmount = product.GetMinAmount();

                var item = new LeadItem
                {
                    Name = product.Name,
                    Price = PriceService.GetFinalPrice(offer.RoundedPrice, offer.Product.Discount),
                    ProductId = product.ProductId,
                    Amount = prodMinAmount,
                    ArtNo = offer.ArtNo,
                    BarCode = offer.BarCode,
                    Color = offer.Color != null ? offer.Color.ColorName : null,
                    Size = offer.Size != null ? offer.Size.SizeName : null,
                    PhotoId = offer.Photo != null ? offer.Photo.PhotoId : default(int),
                    Weight = offer.GetWeight(), //product.Weight,
                    Width = offer.GetWidth(), //product.Width,
                    Height = offer.GetHeight(), //product.Height,
                    Length = offer.GetLength() //product.Length
                };

                var oItem = _lead.LeadItems.Find(x => x == item);
                if (oItem != null)
                {
                    oItem.Amount += 1;

                    history.AddRange(ChangeHistoryService.GetChanges(_lead.Id, ChangeHistoryObjType.Lead, LeadService.GetLeadItem(item.LeadItemId), oItem, null, item.LeadItemId));

                    LeadService.UpdateLeadItem(_lead.Id, oItem);
                }
                else
                {
                    LeadService.AddLeadItem(_lead.Id, item);

                    history.Add(new ChangeHistory(null)
                    {
                        ObjId = _lead.Id,
                        ObjType = ChangeHistoryObjType.Lead,
                        ParameterName =
                            "Добавлен товар " + item.Name +
                            (!string.IsNullOrEmpty(item.ArtNo) ? " (" + item.ArtNo + ")" : ""),
                        ParameterId = item.ProductId,
                    });
                }
                saveChanges = true;
            }

            _lead.LeadItems = LeadService.GetLeadItems(_lead.Id);

            LeadService.OnLeadChanged(_lead, history);


            return saveChanges;
        }
    }
}
