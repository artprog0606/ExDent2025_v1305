using AdvantShop.Core.Common.Attributes;

namespace AdvantShop.Letters
{
    public enum TaskLetterTemplateKey
    {
        [LetterFormatKey("#TASK_ID#", Description = "Номер задачи")]
        TaskId,
        
        [LetterFormatKey("#TASK_PROJECT#", Description = "Проект")]
        Project,
        
        [LetterFormatKey("#TASK_NAME#", Description = "Название")]
        Name,
        
        [LetterFormatKey("#TASK_DESCRIPTION#", Description = "Описание")]
        Description,

        [LetterFormatKey("#TASK_STATUS#", Description = "Статус")]
        Status,

        [LetterFormatKey("#TASK_PRIORITY#", Description = "Приоритет")]
        Priority,
        
        [LetterFormatKey("#DUEDATE#", Description = "Крайний срок")]
        DueDate,

        [LetterFormatKey("#DATE_CREATED#", Description = "Дата создания")]
        DateCreated,

        [LetterFormatKey("#MANAGER_NAME#", Description = "Исполнитель")]
        ManagerName,
        
        [LetterFormatKey("#APPOINTEDMANAGER#", Description = "Постановщик")]
        AppointedManagerName,
        
        [LetterFormatKey("#TASK_URL#", Description = "URL ссылка на задачу")]
        Url,
        
        [LetterFormatKey("#TASK_ATTACHMENTS#", Description = "Приложения")]
        Attachments
    }
}