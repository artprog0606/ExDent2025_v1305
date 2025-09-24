using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Crm.DealStatuses;
using AdvantShop.Core.Services.Crm.LeadFields;
using AdvantShop.Core.Services.Crm.SalesFunnels;
using AdvantShop.Saas;
using AdvantShop.Web.Admin.Models.Crm.SalesFunnels;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Web.Admin.Handlers.Crm.SalesFunnels
{
    public class AddUpdateSalesFunnelHandler : AbstractCommandHandler<object>
    {
        private readonly SalesFunnelAddEditModel _model;
        private readonly bool _editMode;
        private SalesFunnel _funnel;

        public AddUpdateSalesFunnelHandler(SalesFunnelAddEditModel model, bool editMode)
        {
            _model = model;
            _editMode = editMode;
        }

        protected override void Load()
        {
            _funnel = _editMode ? SalesFunnelService.Get(_model.Id) : (SalesFunnel)_model;
        }

        protected override void Validate()
        {
            if (_editMode && _funnel == null)
                throw new BlException(T("Admin.SalesFunnel.Errors.NotFound"));

            if (!_editMode)
            {
                _model.DealStatuses = (_model.DealStatuses ?? new List<AddDealStatusModel>()).Where(x => x.Name.IsNotEmpty() && x.Color.IsNotEmpty()).ToList();
                if (!_model.DealStatuses.Any(x => x.Status == SalesFunnelStatusType.None) ||
                    !_model.DealStatuses.Any(x => x.Status == SalesFunnelStatusType.Canceled) ||
                    !_model.DealStatuses.Any(x => x.Status == SalesFunnelStatusType.FinalSuccess))
                    throw new BlException(T("Admin.SalesFunnel.Errors.MissingDealStatuses"));

            }
            
            if (!_editMode && SaasDataService.IsSaasEnabled && SalesFunnelService.GetCount("[Enable] = 1") >= SaasDataService.CurrentSaasData.LeadsListsCount)
                throw new BlException("Количество воронок по тарифному плану не может превышать " + SaasDataService.CurrentSaasData.LeadsListsCount);
            if (_editMode && _model.Enable && !_funnel.Enable && SaasDataService.IsSaasEnabled && SalesFunnelService.GetCount("[Enable] = 1") >= SaasDataService.CurrentSaasData.LeadsListsCount)
                throw new BlException("Количество воронок по тарифному плану не может превышать " + SaasDataService.CurrentSaasData.LeadsListsCount);
        }

        protected override object Handle()
        {
            if (_editMode)
            {
                _funnel.Name = _model.Name;
                _funnel.Enable = _model.Enable;
                _funnel.SortOrder = _model.SortOrder;
                _funnel.FinalSuccessAction = _model.FinalSuccessAction;
                _funnel.NotSendNotificationsOnLeadCreation = _model.NotSendNotificationsOnLeadCreation;
                _funnel.NotSendNotificationsOnLeadChanged = _model.NotSendNotificationsOnLeadChanged;
                _funnel.LeadAutoCompleteActionType = _model.LeadAutoCompleteActionType;
            }

            if (!_editMode)
            {
                SalesFunnelService.Add(_funnel);
                Track.TrackService.TrackEvent(Track.ETrackEvent.Core_Leads_AddLeadsList);
            }
            else
            {
                SalesFunnelService.Update(_funnel);
            }

            if (!_editMode && _model.DealStatuses.Any())
            {
                for (int i = 0; i < _model.DealStatuses.Count; i++)
                {
                    var statusModel = _model.DealStatuses[i];
                    var dealStatus = new DealStatus
                    {
                        Name = statusModel.Name,
                        Color = statusModel.Color,
                        Status = statusModel.Status,
                        SortOrder = i * 10
                    };
                    DealStatusService.Add(dealStatus);
                    SalesFunnelService.AddDealStatus(_funnel.Id, dealStatus.Id);
                }
            }

            if (!_editMode && _model.LeadFields != null)
            {
                for (int i = 0; i < _model.LeadFields.Count; i++)
                {
                    var field = _model.LeadFields[i];
                    if (field.Name.IsNullOrEmpty())
                        continue;
                    field.SalesFunnelId = _funnel.Id;
                    field.SortOrder = i * 10;
                    LeadFieldService.AddLeadField(field);

                    if (field.HasValues && field.FieldValues != null)
                    {
                        for (int j = 0; j < field.FieldValues.Count; j++)
                        {
                            var fieldValue = field.FieldValues[j];
                            if (fieldValue.Value.IsNullOrEmpty())
                                continue;

                            fieldValue.LeadFieldId = field.Id;
                            fieldValue.SortOrder = j * 10;
                            LeadFieldService.AddLeadFieldValue(fieldValue);
                        }
                    }
                }
            }

            if (_editMode)
                SalesFunnelService.ClearSalesFunnelManagers(_funnel.Id);
            if (_model.ManagerIds != null)
            {
                foreach (var managerId in _model.ManagerIds)
                    SalesFunnelService.AddSalesFunnelManager(_funnel.Id, managerId);
            }

            if (_editMode)
                SalesFunnelService.ClearLeadAutoCompleteProducts(_funnel.Id);
            if (_model.LeadAutoCompleteProductIds != null)
            {
                foreach (var productId in _model.LeadAutoCompleteProductIds)
                    SalesFunnelService.AddLeadAutoCompleteProduct(_funnel.Id, productId);
            }

            if (_editMode)
                SalesFunnelService.ClearLeadAutoCompleteCategories(_funnel.Id);
            if (_model.LeadAutoCompleteCategoryIds != null)
            {
                foreach (var categoryId in _model.LeadAutoCompleteCategoryIds)
                    SalesFunnelService.AddLeadAutoCompleteCategory(_funnel.Id, categoryId);
            }

            return new { _funnel.Id, _funnel.Name };
        }
    }
}

