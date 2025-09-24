using System;
using System.Collections.Generic;
using System.Web;
using AdvantShop.Configuration;
using AdvantShop.Core.Modules;
using AdvantShop.Core.Services.Bonuses;
using AdvantShop.Core.Services.Bonuses.Model;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Customers;
using AdvantShop.Diagnostics;
using AdvantShop.Helpers;
using AdvantShop.Mails;
using AdvantShop.Security;
using AdvantShop.Core.Services.Mails;
using AdvantShop.Areas.Partners.ViewModels.Account;
using AdvantShop.Web.Infrastructure.Handlers;
using AdvantShop.Core;
using AdvantShop.Core.Services.Partners;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Catalog;

namespace AdvantShop.Areas.Partners.Handlers.Account
{
    public class RegistrationHandler : AbstractCommandHandler
    {
        private RegistrationViewModel _model;
        private Coupon _couponTpl;

        public RegistrationHandler(RegistrationViewModel model)
        {
            _model = model;
        }

        protected override void Load()
        {
            _couponTpl = CouponService.GetPartnersCouponTemplate();
        }

        protected override void Validate()
        {
            if (_couponTpl == null)
            {
                Debug.Log.Warn("Не настроен шаблон партнерского купона, регистрация партнеров невозможна. Попытка регистрации партнера: " + _model.Email);
                throw new BlException("Регистрация партнеров временно недоступна. Повторите попытку позже.");
            }

            if (_model.PartnerType == EPartnerType.None || !Enum.IsDefined(typeof(EPartnerType), _model.PartnerType))
                throw new BlException("Укажите, юридическим или физическим лицом вы являетесь");

            if (!ValidationHelper.IsValidEmail(_model.Email))
                throw new BlException("Некорректный email");

            if (PartnerService.ExistsEmail(_model.Email))
                throw new BlException("Партнер с таким email уже существует. Воспользуйтесь <a href=\"account/forgotpassword\">восстановлением пароля</a>");

            if (!_model.PasswordConfirm.IsNotEmpty() || !_model.Password.IsNotEmpty() || _model.Password != _model.PasswordConfirm)
                throw new BlException("Введенные пароли не совпадают");

            if (_model.Password.Length < 6)
                throw new BlException("Длина пароля должна быть не менее 6 символов");

            if (!(_model.Name.IsNotEmpty() && _model.Phone.IsNotEmpty()))
                throw new BlException("Заполните обязательные поля");

            if (SettingsCheckout.IsShowUserAgreementText && !_model.Agree)
                throw new BlException("Необходимо принять условия пользовательского соглашения");

            if (!ModulesExecuter.CheckInfo(HttpContext.Current, Core.Modules.Interfaces.ECheckType.Other, _model.Email, _model.Name, phone: _model.Phone))
                throw new BlException("Не пройдена проверка на спам");
        }

        protected override void Handle()
        {
            var partner = new Partner
            {
                Type = _model.PartnerType,
                Email = _model.Email,
                Phone = _model.Phone,
                City = _model.City,
                Name = _model.Name,
                Password = _model.Password,
                Enabled = SettingsPartners.ActivatePartners,
                RewardPercent = SettingsPartners.DefaultRewardPercent,
            };

            foreach (EPartnerMessageType messageType in Enum.GetValues(typeof(EPartnerMessageType)))
                partner.SendMessages |= messageType;

            var coupon = CouponService.GeneratePartnerCoupon(_couponTpl, CouponService.GenerateCouponCode());
            partner.CouponId = coupon.CouponID;

            PartnerService.AddPartner(partner);
            PartnerAuthService.SignIn(partner.Email, partner.Password, false);

            var mail = new PartnerRegistrationMailTemplate(partner);
            MailService.SendMailNow(partner.Email, mail);
            MailService.SendMailNow(SettingsMail.EmailForPartners, mail, replyTo: partner.Email);
        }
    }
}