using System;
using System.Linq;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Crm;
using AdvantShop.Core.UrlRewriter;

namespace AdvantShop.Letters
{
    public sealed class TaskLetterBuilder : BaseLetterTemplateBuilder<Task, TaskLetterTemplateKey>
    {
        public TaskLetterBuilder(Task task) : base(task, null)
        {
        }

        protected override string GetValue(TaskLetterTemplateKey key)
        {
            var task = _entity;

            switch (key)
            {
                case TaskLetterTemplateKey.TaskId:      return task.Id.ToString();
                case TaskLetterTemplateKey.Project:     return task.TaskGroup != null ? task.TaskGroup.Name : "";
                case TaskLetterTemplateKey.Name:        return task.Name;
                case TaskLetterTemplateKey.Description: return task.Description;
                case TaskLetterTemplateKey.Status:      return task.StatusName;
                case TaskLetterTemplateKey.Priority:    return task.Priority.Localize();
                
                case TaskLetterTemplateKey.DueDate: 
                    return task.DueDate.HasValue 
                        ? task.DueDate.Value.ToString("dd.MM.yyyy HH:mm") 
                        : "-";
                
                case TaskLetterTemplateKey.DateCreated: 
                    return task.DateAppointed.ToString("dd.MM.yyyy HH:mm");
                
                case TaskLetterTemplateKey.ManagerName:
                    return task.TaskGroup != null && task.TaskGroup.IsPrivateComments 
                        ? "-" 
                        : task.Managers.Select(x => x.FullName).AggregateString(", ");
                
                case TaskLetterTemplateKey.AppointedManagerName: 
                    return task.AppointedManager != null ? task.AppointedManager.FullName : "";
                
                case TaskLetterTemplateKey.Url: 
                    return UrlService.GetAdminUrl("tasks/view/" + task.Id);
                
                case TaskLetterTemplateKey.Attachments: 
                    return 
                        task.Attachments
                            .Select(x => $"<a href=\"{x.Path}\">{x.FileName}</a>")
                            .DefaultIfEmpty("-")
                            .AggregateString(", ");
                
                default:
                    throw new ArgumentOutOfRangeException(nameof(key), key, null);
            }
        }
    }
}