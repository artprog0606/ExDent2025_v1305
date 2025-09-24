using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Customers;
using AdvantShop.ExportImport;
using AdvantShop.Web.Admin.Models;
using AdvantShop.Web.Admin.Models.Customers.Export;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Web.Admin.Handlers.Customers.Export
{
    public class GetExportCustomersSettings : ICommandHandler<ExportCustomersSettings>
    {
        public ExportCustomersSettings Execute()
        {
            var encodings = new Dictionary<string, string>();
            foreach (var enumItem in (EncodingsEnum[])Enum.GetValues(typeof(EncodingsEnum)))
            {
                encodings.Add(enumItem.StrName(), enumItem.StrName());
            }

            var customerGroups = new Dictionary<int, string>() { { -1, LocalizationService.GetResource("Admin.Marketing.All") } };
            foreach (var group in CustomerGroupService.GetCustomerGroupList())
            {
                customerGroups.Add(group.CustomerGroupId, group.GroupName);
            }

            var separators = new Dictionary<string, string>();
            foreach (var enumItem in (SeparatorsEnum[])Enum.GetValues(typeof(SeparatorsEnum)))
            {
                if (enumItem == SeparatorsEnum.Custom) 
                    continue;
                separators.Add(enumItem.StrName(), enumItem.Localize());
            }

            var allFields = new List<SelectItemModel>();
            var selectedFields = new List<string>();
                
            foreach (ECustomerFields field in Enum.GetValues(typeof(ECustomerFields)))
            {
                allFields.Add(new SelectItemModel(field.Localize(), field.ToString()));
                
                if (field != ECustomerFields.None)
                    selectedFields.Add(field.ToString());
            }
            
            var customerFields = CustomerFieldService.GetCustomerFields(true);
            foreach (var field in customerFields)
            {
                allFields.Add(new SelectItemModel(field.Name, field.Name));
                selectedFields.Add(field.Name);
            }

            var managers = 
                ManagerService.GetManagersList()
                    .Select(x => new SelectItemModel(x.FullName, x.ManagerId))
                    .ToList();

            var customerTypes = Enum.GetValues(typeof(CustomerType))
                .Cast<CustomerType>()
                .Where(x => x != CustomerType.All)
                .Select(x => new SelectItemModel<int>(x.Localize(), (int) x))
                .ToList();

            var model = new ExportCustomersSettings
            {
                Encoding = EncodingsEnum.Windows1251.StrName(),
                PropertySeparator = ";",
                ColumnSeparator = SeparatorsEnum.SemicolonSeparated.StrName(),
                Encodings = encodings,
                Groups = customerGroups,
                Managers = managers,
                CustomerTypes = customerTypes,
                Separators = separators,
                AllFields = allFields,
                SelectedExportFields = selectedFields,
                
                GroupId = -1,
            };

            return model;
        }
    }
}