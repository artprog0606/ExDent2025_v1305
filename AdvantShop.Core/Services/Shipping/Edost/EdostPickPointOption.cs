
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Orders;

namespace AdvantShop.Shipping.Edost
{
	public class EdostPickPointOption : EdostOption
    {
        public EdostPickPointOption()
        {
        }

        public EdostPickPointOption(ShippingMethod method, float preCost, EdostTarif tarif)
            : base(method, preCost, tarif)
        {
            Pickpointmap = tarif.PickpointMap;
            HideAddressBlock = true;
        }

        public string Pickpointmap { get; set; }
        public string PickpointId { get; set; }
        public string PickpointAddress { get; set; }

        public override string TemplateName
        {
            get { return "EdostPickPointOption.html"; }
        }

        public override void Update(BaseShippingOption option)
        {
            var opt = option as EdostPickPointOption;
            if (opt != null && opt.Id == this.Id)
            {
                this.PickpointId = opt.PickpointId;
                this.PickpointAddress = opt.PickpointAddress;
                this.Pickpointmap = opt.Pickpointmap;
            }
        }

        public override OptionValidationResult Validate()
        {
            var result = base.Validate();
            if (!string.IsNullOrEmpty(this.PickpointId)) return result;
            result.IsValid = false;
            result.ErrorMessage = LocalizationService.GetResource("Core.Shipping.EdostPickPointOption.PickpointError");
            return result;
        }

        public override OrderPickPoint GetOrderPickPoint()
        {
            return new OrderPickPoint
            {
                PickPointId = PickpointId,
                PickPointAddress = PickpointAddress,
            };
        }

        public override void UpdateFromOrderPickPoint(OrderPickPoint orderPickPoint)
        {
            if (orderPickPoint.PickPointId.IsNotEmpty())
            {
                PickpointId = orderPickPoint.PickPointId;
                PickpointAddress = orderPickPoint.PickPointAddress;
            }
        }
    }
}