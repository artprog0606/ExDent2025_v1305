using AdvantShop.Web.Admin.Models.Tasks.TaskGroups;
using AdvantShop.Web.Admin.ViewModels.Tasks.TaskGroups;

namespace AdvantShop.Web.Admin.Handlers.Tasks.TaskGroups
{
    public class GetIndexModel
    {
        private readonly TaskGroupsFilterModel _filter;

        public GetIndexModel(TaskGroupsFilterModel filter)
        {
            _filter = filter;
        }

        public TaskGroupsListViewModel Execute()
        {
            var model = new TaskGroupsListViewModel();
            return model;
        }
    }
}
