//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Services.Crm;
using AdvantShop.Core.Services.Mails;
using AdvantShop.Letters;

namespace AdvantShop.Mails
{
    public abstract class BaseTaskMailTemplate : MailTemplate
    {
        private readonly TaskLetterBuilder _taskLetterBuilder;

        protected BaseTaskMailTemplate() { }
        
        protected BaseTaskMailTemplate(Task task)
        {
            _taskLetterBuilder = new TaskLetterBuilder(task);
        }

        protected override string FormatString(string text)
        {
            return _taskLetterBuilder.FormatText(text);
        }

        protected override Dictionary<string, string> GetFormattedParams(string text)
        {
            return _taskLetterBuilder.GetFormattedParams(text);
        }

        public override List<LetterFormatKey> GetKeyDescriptions()
        {
            return new TaskLetterBuilder(null).GetKeyDescriptions();
        }
    }

    [MailTemplateType(MailType.OnTaskCreated)]
    public sealed class TaskCreatedMailTemplate : BaseTaskMailTemplate
    {
        public override MailType Type => MailType.OnTaskCreated;
        
        private TaskCreatedMailTemplate() { }

        public TaskCreatedMailTemplate(Task task) : base(task)
        {
        }
    }

    [MailTemplateType(MailType.OnTaskAssigned)]
    public sealed class TaskAssignedMailTemplate : BaseTaskMailTemplate
    {
        public override MailType Type => MailType.OnTaskAssigned;
        
        private TaskAssignedMailTemplate() { }

        public TaskAssignedMailTemplate(Task task) : base(task)
        {
        }
    }

    [MailTemplateType(MailType.OnTaskDeleted)]
    public sealed class TaskDeletedMailTemplate : BaseTaskMailTemplate
    {
        public override MailType Type => MailType.OnTaskDeleted;

        private readonly string _modifier;
        
        private TaskDeletedMailTemplate() { }

        public TaskDeletedMailTemplate(Task task, string modifier) : base(task)
        {
            _modifier = modifier;
        }

        protected override string FormatString(string text)
        {
            var formattedStr = base.FormatString(text);
            formattedStr = formattedStr.Replace("#MODIFIER#", _modifier);
            return formattedStr;
        }

        public override List<LetterFormatKey> GetKeyDescriptions()
        {
            return base.GetKeyDescriptions()
                .Concat(new List<LetterFormatKey>()
                {
                    new LetterFormatKey("#MODIFIER#", "Кто менял")
                })
                .ToList();
        }
    }

    [MailTemplateType(MailType.OnTaskCommentAdded)]
    public sealed class TaskCommentAddedMailTemplate : BaseTaskMailTemplate
    {
        public override MailType Type => MailType.OnTaskCommentAdded;

        private readonly string _author;
        private readonly string _comment;
        
        private TaskCommentAddedMailTemplate() { }

        public TaskCommentAddedMailTemplate(Task task, string author, string comment) : base(task)
        {
            _author = author;
            _comment = comment;
        }

        protected override string FormatString(string text)
        {
            var formattedStr = base.FormatString(text);
            formattedStr = formattedStr.Replace("#AUTHOR#", _author);
            formattedStr = formattedStr.Replace("#COMMENT#", _comment);
            return formattedStr;
        }
        
        public override List<LetterFormatKey> GetKeyDescriptions()
        {
            return base.GetKeyDescriptions()
                .Concat(new List<LetterFormatKey>()
                {
                    new LetterFormatKey("#AUTHOR#", "Автор"),
                    new LetterFormatKey("#COMMENT#", "Комментарий")
                })
                .ToList();
        }
    }

    [MailTemplateType(MailType.OnTaskChanged)]
    public sealed class TaskChangedMailTemplate : BaseTaskMailTemplate
    {
        public override MailType Type => MailType.OnTaskChanged;

        private readonly string _modifier;
        private readonly string _changesTable;
        private readonly Task _taskPrev;
        
        private TaskChangedMailTemplate() { }

        public TaskChangedMailTemplate(Task task, string changesTable, string modifier, Task taskPrev) : base(task)
        {
            _modifier = modifier;
            _changesTable = changesTable;
            _taskPrev = taskPrev;
        }

        protected override string FormatString(string text)
        {
            // task name from previous state
            var formattedStr = text.Replace("#TASK_NAME#", _taskPrev.Name);

            formattedStr = base.FormatString(formattedStr);

            formattedStr = formattedStr.Replace("#MODIFIER#", _modifier);
            formattedStr = formattedStr.Replace("#CHANGES_TABLE#", _changesTable);
            return formattedStr;
        }
        
        public override List<LetterFormatKey> GetKeyDescriptions()
        {
            return base.GetKeyDescriptions()
                .Concat(new List<LetterFormatKey>()
                {
                    new LetterFormatKey("#MODIFIER#", "Кто менял"),
                    new LetterFormatKey("#CHANGES_TABLE#", "Изменения")
                })
                .ToList();
        }
    }

    [MailTemplateType(MailType.OnTaskReminder)]
    public sealed class TaskReminderMailTemplate : BaseTaskMailTemplate
    {
        public override MailType Type => MailType.OnTaskReminder;
        
        public TaskReminderMailTemplate() { }

        public TaskReminderMailTemplate(Task task) : base(task)
        {
        }
    }

    [MailTemplateType(MailType.OnTaskObserverAdded)]
    public sealed class TaskObserverAddedMailTemplate : BaseTaskMailTemplate
    {
        public override MailType Type => MailType.OnTaskObserverAdded;

        private readonly string _newObserver;
        
        public TaskObserverAddedMailTemplate() { }
        
        public TaskObserverAddedMailTemplate(Task task, string newObserver) : base(task)
        {
            _newObserver = newObserver;
        }

        protected override string FormatString(string text)
        {
            var formattedStr = base.FormatString(text);

            formattedStr = formattedStr.Replace("#OBSERVER#", _newObserver);

            return formattedStr;
        }
        
        public override List<LetterFormatKey> GetKeyDescriptions()
        {
            return base.GetKeyDescriptions()
                .Concat(new List<LetterFormatKey>()
                {
                    new LetterFormatKey("#OBSERVER#", "Наблюдатель")
                })
                .ToList();
        }
    }
}