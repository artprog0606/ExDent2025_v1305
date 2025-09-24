using AdvantShop.Core.Services.Crm;

namespace AdvantShop.Web.Admin.Handlers.Leads
{
    public class ChangeLeadSorting
    {
        private readonly int _id;
        private readonly int? _prevId;
        private readonly int? _nextId;

        public ChangeLeadSorting(int id, int? prevId, int? nextId)
        {
            _id = id;
            _prevId = prevId;
            _nextId = nextId;
        }

        public bool Execute()
        {
            var lead = LeadService.GetLead(_id);
            if (lead == null)
                return false;

            LeadService.ChangeLeadSorting(_id, _prevId, _nextId);

            return true;
        }
    }
}
