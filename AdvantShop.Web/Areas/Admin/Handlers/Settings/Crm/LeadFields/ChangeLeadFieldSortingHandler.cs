using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Services.Crm.LeadFields;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Web.Admin.Handlers.Settings.Crm.LeadFields
{
    public class ChangeLeadFieldSortingHandler : AbstractCommandHandler
    {
        private readonly int _salesFunnelId;
        private readonly int _id;
        private readonly int? _prevId;
        private readonly int? _nextId;

        private List<LeadField> _fields;
        private LeadField _field;

        public ChangeLeadFieldSortingHandler(int salesFunnelId, int id, int? prevId, int? nextId)
        {
            _salesFunnelId = salesFunnelId;
            _id = id;
            _prevId = prevId;
            _nextId = nextId;
        }

        protected override void Load()
        {
            _fields = LeadFieldService.GetLeadFields(_salesFunnelId, false);
            _field = _fields.FirstOrDefault(x => x.Id == _id);
        }

        protected override void Handle()
        {
            if (_field == null || !_fields.Any())
                return;

            _fields.Remove(_field);

            var prevIndex = _prevId.HasValue ? _fields.FindIndex(x => x.Id == _prevId.Value) : -1;
            var nextIndex = _nextId.HasValue ? _fields.FindIndex(x => x.Id == _nextId.Value) : -1;
            if (prevIndex == -1)
                _field.SortOrder = _fields.Min(x => x.SortOrder) - 10;
            else if (nextIndex == -1)
                _field.SortOrder = _fields.Max(x => x.SortOrder) + 10;
            else if (_fields[nextIndex].SortOrder - _fields[prevIndex].SortOrder > 1)
                _field.SortOrder = _fields[prevIndex].SortOrder + ((_fields[nextIndex].SortOrder - _fields[prevIndex].SortOrder) / 2);
            else
            {
                _fields.Insert(prevIndex + 1, _field);
                for (int i = 0; i < _fields.Count; i++)
                {
                    _fields[i].SortOrder = i * 10;
                    LeadFieldService.UpdateLeadField(_fields[i]);
                }
                return;
            }

            LeadFieldService.UpdateLeadField(_field);
        }
    }
}
