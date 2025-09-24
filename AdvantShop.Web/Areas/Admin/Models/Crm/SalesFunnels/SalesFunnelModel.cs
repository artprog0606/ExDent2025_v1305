using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using AdvantShop.Catalog;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Configuration.Settings;
using AdvantShop.Core.Services.Crm.SalesFunnels;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Customers;

namespace AdvantShop.Web.Admin.Models.Crm.SalesFunnels
{
    public class SalesFunnelModel : SalesFunnel
    {
        public SalesFunnelModel() { }

        public SalesFunnelModel(SalesFunnel funnel)
        {
            Id = funnel.Id;
            Name = funnel.Name;
            FinalSuccessAction = funnel.FinalSuccessAction;
            LeadAutoCompleteActionType = funnel.LeadAutoCompleteActionType;
            SortOrder = funnel.SortOrder;
            Enable = funnel.Enable;
            NotSendNotificationsOnLeadCreation = funnel.NotSendNotificationsOnLeadCreation;
            NotSendNotificationsOnLeadChanged = funnel.NotSendNotificationsOnLeadChanged;
        }

        public bool IsDefaultFunnel
        {
            get { return Id == SettingsCrm.DefaultSalesFunnelId; }
        }
    }

    public class SalesFunnelAddEditModel : SalesFunnelModel, IValidatableObject
    {
        public SalesFunnelAddEditModel() : base()
        {
        }

        public SalesFunnelAddEditModel(SalesFunnel funnel) : base(funnel)
        {
            ManagerIds = SalesFunnelService.GetSalesFunnelManagers(Id).Select(x => x.ManagerId).ToList();
            LeadAutoCompleteProductIds = SalesFunnelService.GetLeadAutoCompleteProductIds(Id);
            LeadAutoCompleteCategoryIds = SalesFunnelService.GetLeadAutoCompleteCategoryIds(Id);
        }

        public List<AddDealStatusModel> DealStatuses { get; set; }
        public List<LeadFieldModel> LeadFields { get; set; }

        public List<int> ManagerIds { get; set; }

        private List<SelectItemModel> _managers;
        public List<SelectItemModel> Managers
        {
            get
            {
                if (_managers == null)
                    _managers = ManagerService.GetManagers(RoleAction.Crm).Select(x => new SelectItemModel(x.FullName, x.ManagerId)).ToList();

                if (ManagerIds == null || !ManagerIds.Any())
                    return _managers;

                foreach (var managerId in ManagerIds.Where(id => !_managers.Any(m => m.value == id.ToString())))
                {
                    var manager = ManagerService.GetManager(managerId);
                    if (manager != null)
                        _managers.Add(new SelectItemModel(manager.FullName, manager.ManagerId));
                }

                return _managers;
            }
        }

        public List<int> LeadAutoCompleteProductIds { get; set; }

        private List<SelectItemModel> _leadAutoCompleteProducts;
        public List<SelectItemModel> LeadAutoCompleteProducts
        {
            get
            {
                if (LeadAutoCompleteProductIds == null || !LeadAutoCompleteProductIds.Any())
                    return null;

                if (_leadAutoCompleteProducts != null)
                    return _leadAutoCompleteCategories;

                _leadAutoCompleteProducts = 
                    SalesFunnelService.GetLeadAutoCompleteProductsInfo(LeadAutoCompleteProductIds)
                    .Select(x => new SelectItemModel(x.Value, x.Key)).ToList();

                return _leadAutoCompleteProducts;
            }
        }

        public List<int> LeadAutoCompleteCategoryIds { get; set; }

        private List<SelectItemModel> _leadAutoCompleteCategories;
        public List<SelectItemModel> LeadAutoCompleteCategories
        {
            get
            {
                if (LeadAutoCompleteCategoryIds == null || !LeadAutoCompleteCategoryIds.Any())
                    return null;

                if (_leadAutoCompleteCategories != null)
                    return _leadAutoCompleteCategories;

                _leadAutoCompleteCategories = 
                    SalesFunnelService.GetLeadAutoCompleteCategoriesInfo(LeadAutoCompleteCategoryIds)
                    .Select(x => new SelectItemModel(x.Value, x.Key)).ToList();

                return _leadAutoCompleteCategories;
            }
        }

        public List<SelectItemModel> LeadAutoCompleteActionTypes
        {
            get
            {
                return Enum.GetValues(typeof(ELeadAutoCompleteActionType)).Cast<ELeadAutoCompleteActionType>()
                    .Select(x => new SelectItemModel(x.Localize(), (int)x)).ToList();
            }
        }

        public List<SelectItemModel> FinalSuccessActions
        {
            get
            {
                return Enum.GetValues(typeof(SalesFunnelFinalSuccessAction)).Cast<SalesFunnelFinalSuccessAction>()
                    .Select(x => new SelectItemModel(x.Localize(), (int)x)).ToList();
            }
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Id != 0 && Id == SettingsCrm.DefaultSalesFunnelId && !Enable)
                yield return new ValidationResult(LocalizationService.GetResource("Admin.SalesFunnels.Edit.DeleteDefaultFunnel"));

            if (string.IsNullOrWhiteSpace(Name))
                yield return new ValidationResult(LocalizationService.GetResource("Admin.SalesFunnels.Edit.NameIsEmpty"));
        }
    }
}
