using System;
using AdvantShop.Catalog;
using AdvantShop.Core;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.Partners;
using AdvantShop.Web.Admin.Models.Partners;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Web.Admin.Handlers.Partners
{
    public class GetPartnerViewHandler : AbstractCommandHandler<PartnerViewModel>
    {
        private readonly int _id;
        private Partner _partner;

        public GetPartnerViewHandler(int id)
        {
            _id = id;
        }

        protected override void Load()
        {
            _partner = PartnerService.GetPartner(_id);
        }
        
        protected override void Validate()
        {
            if (_partner == null)
                throw new BlException(T("Admin.Partner.Errors.NotFound"));
        }

        protected override PartnerViewModel Handle()
        {
            var coupon = _partner.CouponId.HasValue ? CouponService.GetCoupon(_partner.CouponId.Value) : null;

            var paymentType = _partner.NaturalPerson != null && _partner.NaturalPerson.PaymentTypeId.HasValue 
                ? PaymentTypeService.GetPaymentType(_partner.NaturalPerson.PaymentTypeId.Value) 
                : null;

            var model = new PartnerViewModel
            {
                Partner = _partner,
                TypeFormatted = _partner.Type.Localize(),
                BalanceFormatted = _partner.Balance.FormatRoundPriceDefault(),
                CustomersCount = PartnerService.GetBindedCustomersCount(_id),
                TimeFromCreated = _partner.DateCreated.GetDurationString(DateTime.Now),
                ContractDateFormatted = _partner.ContractDate.HasValue ? _partner.ContractDate.Value.ToString("dd.MM.yyyy") : null,
                CouponCode = coupon != null ? coupon.Code : null,
                PaymentTypeName = paymentType != null ? paymentType.Name : null,
                CouponTemplateExists = CouponService.GetPartnersCouponTemplate() != null
            };

            return model;
        }
    }
}
