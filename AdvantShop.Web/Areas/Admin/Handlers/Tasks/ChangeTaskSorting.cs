using AdvantShop.Core.Services.Crm;

namespace AdvantShop.Web.Admin.Handlers.Tasks
{
    public class ChangeTaskSorting
    {
        private readonly int _id;
        private readonly int? _prevId;
        private readonly int? _nextId;

        public ChangeTaskSorting(int id, int? prevId, int? nextId)
        {
            _id = id;
            _prevId = prevId;
            _nextId = nextId;
        }

        public bool Execute()
        {
            var task = TaskService.GetTask(_id);
            if (task == null)
                return false;

            TaskService.ChangeTaskSorting(_id, _prevId, _nextId);

            return true;
        }
    }
}
