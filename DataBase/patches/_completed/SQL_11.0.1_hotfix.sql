UPDATE [Settings].[Localization] SET ResourceValue = 'Показывать принятые задачи' WHERE ResourceKey like 'Admin.Tasks.Index.ShowAcceptedTasks' AND LanguageId = 1
UPDATE [Settings].[Localization] SET ResourceValue = 'Отклоненные' WHERE ResourceKey like 'Core.Crm.TaskStatusType.Canceled' AND LanguageId = 1

GO--