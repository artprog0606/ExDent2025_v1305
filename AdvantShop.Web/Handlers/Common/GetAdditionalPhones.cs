using System;
using System.Collections.Generic;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Repository;
using AdvantShop.ViewModel.Common;
using AdvantShop.Web.Admin.Models;

namespace AdvantShop.Handlers.Common
{
    public class GetAdditionalPhones
    {
        private readonly bool _fromBuilder;

        public GetAdditionalPhones(bool fromBuilder = false)
        {
            _fromBuilder = fromBuilder;
        }

        public AddtionalPhonesModel Execute()
        {
            var phones = new List<AdditionalPhone>();

            if (_fromBuilder)
            {
                phones.Add(new AdditionalPhone()
                {
                    Phone = SettingsMain.Phone,
                    StandardPhone = SettingsMain.MobilePhone,
                    Description = SettingsMain.PhoneDescription,
                    Type = EAdditionalPhoneType.Phone
                });
            }

            phones.AddRange(SettingsMain.AdditionalPhones);

            var types = new List<SelectItemModel<int>>();

            foreach (EAdditionalPhoneType type in Enum.GetValues(typeof(EAdditionalPhoneType)))
            {
                types.Add(new SelectItemModel<int>(type.Localize(), (int)type));
            }

            return new AddtionalPhonesModel() {Phones = phones, Types = types};
        }
    }
}