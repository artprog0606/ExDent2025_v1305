using System.Linq;
using AdvantShop.Catalog;
using AdvantShop.Core;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Partners;
using AdvantShop.Orders;
using AdvantShop.Web.Admin.Models.Partners;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Web.Admin.Handlers.Partners
{
    public class AddUpdatePartnerHandler : AbstractCommandHandler<int>
    {
        private readonly PartnerEditModel _model;
        private Partner _partner;
        private Coupon _couponTpl;

        public AddUpdatePartnerHandler(PartnerEditModel model)
        {
            _model = model;
        }

        protected override void Load()
        {
            _partner = _model.IsEditMode ? PartnerService.GetPartner(_model.PartnerId) : new Partner();
            _couponTpl = CouponService.GetPartnersCouponTemplate();
        }

        protected override void Validate()
        {
            if (_model.Partner == null)
                throw new BlException(T("Admin.Partner.Errors.NotFound"));

            if (_model.IsEditMode)
            {
                if (_partner == null)
                    throw new BlException(T("Admin.Partner.Errors.NotFound"));

                if (_partner.Email.IsNotEmpty() && _partner.Email != _model.Partner.Email && PartnerService.ExistsEmail(_model.Partner.Email))
                    throw new BlException("Введенный Email занят");
            }

            if (!_model.IsEditMode)
            {
                if (_couponTpl == null)
                    throw new BlException("Шаблон партнерского купона не настроен");

                if (!string.IsNullOrWhiteSpace(_model.Partner.Email) && PartnerService.ExistsEmail(_model.Partner.Email))
                    throw new BlException("Введенный Email занят");

                if (!string.IsNullOrWhiteSpace(_model.CouponCode) && (CouponService.IsExistCouponCode(_model.CouponCode) || GiftCertificateService.IsExistCertificateCode(_model.CouponCode)))
                    throw new BlException("Код купона уже занят");
            }
        }

        protected override int Handle()
        {
            _partner.Type = _model.Partner.Type;
            _partner.Email = _model.Partner.Email;
            _partner.Name = _model.Partner.Name;
            _partner.Phone = _model.Partner.Phone;
            _partner.Password = _model.Partner.Password;
            _partner.City = _model.Partner.City;
            _partner.AdminComment = _model.Partner.AdminComment;
            _partner.Enabled = _model.Partner.Enabled;
            _partner.RewardPercent = _model.Partner.RewardPercent;

            _partner.ContractConcluded = _model.Partner.ContractConcluded;
            _partner.ContractNumber = _model.Partner.ContractNumber;
            _partner.ContractDate = _model.Partner.ContractDate;

            switch (_model.Partner.Type)
            {
                case EPartnerType.LegalEntity:
                    _partner.LegalEntity = _model.Partner.LegalEntity;
                    break;
                case EPartnerType.NaturalPerson:
                    _partner.NaturalPerson = _model.Partner.NaturalPerson;
                    break;
            }

            _partner.SendMessages = EPartnerMessageType.None;
            foreach (var messageType in _model.SendMessages.Keys.Where(key => _model.SendMessages[key]).ToList())
            {
                _partner.SendMessages |= messageType.TryParseEnum<EPartnerMessageType>();
            }

            if (!_model.IsEditMode)
            {
                _partner.Password = _model.Partner.Password;

                if (string.IsNullOrWhiteSpace(_model.CouponCode))
                    _model.CouponCode = CouponService.GenerateCouponCode();
                var coupon = CouponService.GeneratePartnerCoupon(_couponTpl, _model.CouponCode);
                _partner.CouponId = coupon.CouponID;

                PartnerService.AddPartner(_partner);
                _model.PartnerId = _partner.Id;
            }
            else
            {
                PartnerService.UpdatePartner(_partner);
            }

            return _model.PartnerId;
        }
    }
}

