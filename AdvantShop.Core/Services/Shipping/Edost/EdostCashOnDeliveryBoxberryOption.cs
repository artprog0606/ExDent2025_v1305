using AdvantShop.Core.UrlRewriter;
using System.Collections.Generic;
using AdvantShop.Orders;
using Newtonsoft.Json;
using System.Linq;

namespace AdvantShop.Shipping.Edost
{
    public class EdostCashOnDeliveryBoxberryOption : EdostCashOnDeliveryOption
    {
        public EdostBoxberryPoint SelectedPoint { get; set; }
        public List<EdostBoxberryPoint> ShippingPoints { get; set; }

        public EdostCashOnDeliveryBoxberryOption()
        {
        }

        public EdostCashOnDeliveryBoxberryOption(ShippingMethod method, float preCost, EdostTarif tarif, IEnumerable<EdostOffice> offices)
            : base(method, preCost, tarif)
        {
            var temp = offices.Where(x => x.TarifId == tarif.Id);
            ShippingPoints = temp.Select(x => new EdostBoxberryPoint
            {
                Id = x.Id.ToString(),
                Code = x.Code,
                Address = x.Name,
                Description = x.Address,
                Tel = x.Tel,
                Scheldule = x.Scheldule
            }).ToList();
            HideAddressBlock = true;
            DisplayIndex = false;
        }

        public override string TemplateName
        {
            get { return "EdostSelectOption.html"; }
        }

        public override void Update(BaseShippingOption option)
        {
            var opt = option as EdostCashOnDeliveryBoxberryOption;
            if (opt != null && this.Id == opt.Id)
            {
                this.SelectedPoint = opt.SelectedPoint != null ? this.ShippingPoints.FirstOrDefault(x => x.Id == opt.SelectedPoint.Id) : null;
                // this.SelectedPoint = this.SelectedPoint ?? opt.ShippingPoints.FirstOrDefault();
            }
        }

        public override OrderPickPoint GetOrderPickPoint()
        {
            return SelectedPoint is null
                ? null
                : new OrderPickPoint
                {
                    PickPointId = SelectedPoint.Code,
                    PickPointAddress = SelectedPoint.Address,
                    AdditionalData = JsonConvert.SerializeObject(SelectedPoint)
                };
        }
    }
}
