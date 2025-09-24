using System.Linq;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Crm.LeadFields;
using AdvantShop.Web.Admin.Models.Crm.SalesFunnels;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Web.Admin.Handlers.Settings.Crm.LeadFields
{
    public class AddEditLeadFieldHandler : AbstractCommandHandler<LeadField>
    {
        private readonly LeadFieldModel _model;
        private bool _editMode;

        public AddEditLeadFieldHandler(LeadFieldModel model, bool editMode)
        {
            _model = model;
            _editMode = editMode;
        }

        protected override LeadField Handle()
        {
            if (!_editMode)
            {
                var list = LeadFieldService.GetLeadFields(_model.SalesFunnelId, false);
                _model.SortOrder = (list.Count > 0 ? list.Max(x => x.SortOrder) + 10 : 0);
                LeadFieldService.AddLeadField(_model);
            }
            else
                LeadFieldService.UpdateLeadField(_model);

            if (_editMode)
            {
                if (!_model.HasValues)
                    LeadFieldService.DeleteLeadFieldValues(_model.Id);
                else
                {
                    var prevFieldValues = LeadFieldService.GetLeadFieldValues(_model.Id);
                    foreach (var fV in prevFieldValues.Where(x => _model.FieldValues.FirstOrDefault(y => x.Id != 0 && y.Id == x.Id) == null))
                        LeadFieldService.DeleteLeadFieldValue(fV.Id);
                }
            }

            if (_model.HasValues)
            {
                for (int i = 0; i < _model.FieldValues.Count; i++)
                {
                    if (_model.FieldValues[i].Value.IsNullOrEmpty())
                        continue;

                    _model.FieldValues[i].LeadFieldId = _model.Id;
                    _model.FieldValues[i].SortOrder = i * 10;
                    
                    if (_model.FieldValues[i].Id == 0)
                        LeadFieldService.AddLeadFieldValue(_model.FieldValues[i]);
                    else
                        LeadFieldService.UpdateLeadFieldValue(_model.FieldValues[i]);
                }
            }

            return _model;
        }
    }
}
