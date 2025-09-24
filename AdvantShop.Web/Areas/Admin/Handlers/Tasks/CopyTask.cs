using AdvantShop.Core.Services.Crm;
using AdvantShop.Web.Admin.Models.Tasks;

namespace AdvantShop.Web.Admin.Handlers.Tasks
{
    public class CopyTask
    {
        private readonly int _id;

        public CopyTask(int id)
        {
            _id = id;
        }

        public TaskCopyResult Execute()
        {
            var model = new TaskCopyResult();

            var task = TaskService.GetTask(_id);
            if (task == null)
                return model;

            if (!TaskService.CheckAccess(task))
                return model;
            
            var taskCopy = task.DeepClone();

            taskCopy.Name += " - Копия";
            taskCopy.DueDate = null;
            taskCopy.SetManagerIds(task.ManagerIds);

            TaskService.AddTask(taskCopy);

            model.Result = true;
            model.TaskId = taskCopy.Id;

            return model;
        }
    }
}
