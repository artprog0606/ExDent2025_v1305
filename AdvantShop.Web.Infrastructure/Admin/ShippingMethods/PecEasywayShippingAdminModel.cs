using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Shipping;
using AdvantShop.Shipping.PecEasyway;
using AdvantShop.Shipping.PecEasyway.Api;

namespace AdvantShop.Web.Infrastructure.Admin.ShippingMethods
{
    [ShippingAdminModel("PecEasyway")]
    public class PecEasywayShippingAdminModel : ShippingMethodAdminModel, IValidatableObject
    {
        public string Login
        {
            get { return Params.ElementOrDefault(PecEasywayTemplate.Login); }
            set { Params.TryAddValue(PecEasywayTemplate.Login, value.DefaultOrEmpty()); }
        }

        public string Password
        {
            get { return Params.ElementOrDefault(PecEasywayTemplate.Password); }
            set { Params.TryAddValue(PecEasywayTemplate.Password, value.DefaultOrEmpty()); }
        }

        public string LocationFrom
        {
            get { return Params.ElementOrDefault(PecEasywayTemplate.LocationFrom); }
            set { Params.TryAddValue(PecEasywayTemplate.LocationFrom, value.DefaultOrEmpty()); }
        }

        public string[] DeliveryTypes
        {
            get { return (Params.ElementOrDefault(PecEasywayTemplate.DeliveryTypes) ?? string.Empty).Split(","); }
            set { Params.TryAddValue(PecEasywayTemplate.DeliveryTypes, value != null ? string.Join(",", value) : string.Empty); }
        }

        public List<SelectListItem> ListDeliveryTypes
        {
            get
            {
                return EnDeliveryType.AsList()
                    .Select(x => new SelectListItem()
                    {
                        Text = x.Localize(),
                        Value = x.Value.ToString()
                    })
                    .ToList();
            }
        }

        public string TypeViewPoints
        {
            get { return Params.ElementOrDefault(PecEasywayTemplate.TypeViewPoints, ((int)Shipping.PecEasyway.TypeViewPoints.YaWidget).ToString()); }
            set { Params.TryAddValue(PecEasywayTemplate.TypeViewPoints, value.DefaultOrEmpty()); }
        }

        public List<SelectListItem> ListTypesViewPoints
        {
            get
            {
                return Enum.GetValues(typeof(TypeViewPoints))
                    .Cast<TypeViewPoints>()
                    .Select(x => new SelectListItem()
                    {
                        Text = x.Localize(),
                        Value = ((int)x).ToString()
                    })
                    .ToList();
            }
        }

        public string YaMapsApiKey
        {
            get { return Params.ElementOrDefault(PecEasywayTemplate.YaMapsApiKey); }
            set { Params.TryAddValue(PecEasywayTemplate.YaMapsApiKey, value.DefaultOrEmpty()); }
        }

        public bool OrderNoPickup
        {
            get { return Params.ElementOrDefault(PecEasywayTemplate.OrderNoPickup).TryParseBool(); }
            set { Params.TryAddValue(PecEasywayTemplate.OrderNoPickup, value.ToString()); }
        }

        public override bool StatusesSync
        {
            get { return Params.ElementOrDefault(PecEasywayTemplate.StatusesSync).TryParseBool(); }
            set { Params.TryAddValue(PecEasywayTemplate.StatusesSync, value.ToString()); }
        }

        public override string StatusesReference
        {
            get { return Params.ElementOrDefault(PecEasywayTemplate.StatusesReference); }
            set { Params.TryAddValue(PecEasywayTemplate.StatusesReference, value.DefaultOrEmpty()); }
        }

        private Dictionary<string, string> _statuses;
        public override Dictionary<string, string> Statuses => _statuses ?? (_statuses = PecEasyway.Statuses);

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(Login))
                yield return new ValidationResult("Введите логин", new[] { "Login" });
            if (string.IsNullOrWhiteSpace(Password))
                yield return new ValidationResult("Введите пароль", new[] { "Password" });
            if (string.IsNullOrWhiteSpace(LocationFrom))
                yield return new ValidationResult("Адрес отправления", new[] { "LocationFrom" });
        }
    }
}
