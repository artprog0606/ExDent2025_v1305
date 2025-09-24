using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Orders;
using Newtonsoft.Json;

namespace AdvantShop.Shipping.Edost
{
	public class EdostBoxberryOption : EdostOption
    {
        //public string PickpointId { get; set; }
        //public string PickpointAddress { get; set; }
        public List<EdostBoxberryPoint> ShippingPoints { get; set; }

        public EdostBoxberryOption()
        {
        }

        public EdostBoxberryOption(ShippingMethod method, float preCost, EdostTarif tarif, IEnumerable<EdostOffice> offices)
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
        }

        public override string TemplateName
        {
            get { return "EdostSelectOption.html"; }
        }

        public override void Update(BaseShippingOption option)
        {
            var opt = option as EdostBoxberryOption;
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

        public override void UpdateFromOrderPickPoint(OrderPickPoint orderPickPoint)
        {
            if (orderPickPoint.AdditionalData.IsNotEmpty())
                SelectedPoint = JsonConvert.DeserializeObject<EdostBoxberryPoint>(orderPickPoint.AdditionalData);
        }
    }
}