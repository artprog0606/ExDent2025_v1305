

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Назначенные и свободные заказы - заказы, которые назначили менеджеру, и заказы, на которые еще никого не назначили.' WHERE [ResourceKey] = 'Admin.Settings.Users.Settings.AssignedAndFreeOrders' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'Assigned and free orders are orders that are assigned to the manager and orders for which no one has been appointed yet.' WHERE [ResourceKey] = 'Admin.Settings.Users.Settings.AssignedAndFreeOrders' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Назначенные и свободные лиды - лиды, которые назначили менеджеру, и лиды, на которые еще никого не назначили.' WHERE [ResourceKey] = 'Admin.Settings.Users.Settings.AppointedAndFreeLeads' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'The appointed and free leads are the leads that have been assigned to the manager and the leads to which no one has been appointed.' WHERE [ResourceKey] = 'Admin.Settings.Users.Settings.AppointedAndFreeLeads' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Какие задачи будет видеть и сможет редактировать менеджер (должны быть активны права модератора на задачи).<br><br> Все задачи - доступны для редактирования все задачи.<br><br> Назначенные задачи - только задачи, которые назначили менеджеру. Другие задачи не доступны для просмотра и редактирования.<br><br> Назначенные и свободные задачи - задачи, которые назначили менеджеру, и задачи, на которые еще никого не назначили.' WHERE [ResourceKey] = 'Admin.Settings.Users.Settings.MangersTaskConstraintHint' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'What kind of tasks manager will see and will be able to edit (the rights of the moderator on Tasks must be active).<br><br> All tasks are available for editing all the tasks.<br><br> The appointed tasks are only the tasks that are assigned to the manager. Other tasks are not available for viewing and editing.<br><br> The appointed and free tasks are the tasks that have been assigned to the manager and the tasks to which no one has been appointed.' WHERE [ResourceKey] = 'Admin.Settings.Users.Settings.MangersTaskConstraintHint' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Каких покупателей будет видеть и сможет редактировать менеджер (должны быть активны права модератора на покупателей).<br><br> Все покупатели - доступны для редактирования все покупатели.<br><br> Назначенные покупатели - только покупатели, которых назначили менеджеру. Другие покупатели не доступны для просмотра и редактирования.<br><br> Назначенные и свободные покупатели - покупатели, которых назначили менеджеру, и покупатели, которых еще никому не назначили.' WHERE [ResourceKey] = 'Admin.Settings.Users.Settings.ManagersCustomerConstraintHelpText' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'What kind of customers will see and be able to edit the manager (the permissions of the moderator should be active on the customers). <br> <br> All customers - all customers are available for editing. <br> <br> The assigned customers are only customers who have been assigned to the manager. Other customers are not available for viewing and editing. <br> <br> Assigned and free customers are customers who have been assigned to a manager and customers who have not been assigned to anyone yet.' WHERE [ResourceKey] = 'Admin.Settings.Users.Settings.ManagersCustomerConstraintHelpText' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Выбираете оператора ip телефонии и далее настраиваете в соответствии с инструкцией того или иного оператора<br/><br/>Подробнее: <br/> <a href="https://www.advantshop.net/help/pages/phone-sipuni" target="_blank" >Телефония. Sipuni</a> <br/><br/>Подробнее: <br/> <a href="https://www.advantshop.net/help/pages/phone-mango" target="_blank">Телефония. Манго Телеком</a><br/><br/>Подробнее: <br/><a href="https://www.advantshop.net/help/pages/phone-zadarma" target="_blank" >Телефония. Zadarma</a><br/><br/>Подробнее: <br/><a href="https://www.advantshop.net/help/pages/phone-telphin" target="_blank" >Телефония. Телфин</a>' WHERE [ResourceKey] = 'Admin.Settings.Telephony.OperatorHint' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'Choose an ip telephony operator and configure it in accordance with the instructions of this or that operator. <br/> <br/> More details: <br/> <a href = "https://www.advantshop.net/help/pages/phone- sipuni "target =" _blank "> Telephony. Sipuni </a> <br/> <br/> Learn more: <br/> <a href="https://www.advantshop.net/help/pages/phone-mango" target="_blank"> Telephony. Mango Telecom </a> <br/> <br/> More details: <br/> <a href="https://www.advantshop.net/help/pages/phone-zadarma" target="_blank"> Telephony ... Zadarma </a> <br/> <br/> Learn more: <br/> <a href="https://www.advantshop.net/help/pages/phone-telphin" target="_blank"> Telephony. Telfin </a>' WHERE [ResourceKey] = 'Admin.Settings.Telephony.OperatorHint' AND [LanguageId] = 2

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Leads.CrmNavMenu.EditCurrentList', 'Редактировать текущий список')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Leads.CrmNavMenu.EditCurrentList', 'Edit current list')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ExportFeed.SettingsYandex.DontExportCurrency.HelpTip', 'Активируйте эту настройку, если ваша модель сотрудничества с Я.Маркетом (ADV, FBS, FBY, DBS и т.д.) не предполагает выгрузку элемента <currencies> в прайс-лист.')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ExportFeed.SettingsYandex.DontExportCurrency.HelpTip', 'Activate this setting if your model of cooperation with Ya.Market (ADV, FBS, FBY, DBS, etc.) does not imply uploading the <currency> element to the price list.')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Js.Builder.Cancel', 'Отменить')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Js.Builder.Cancel', 'Cancel')

GO--


UPDATE [Settings].[InternalSettings] SET [settingValue] = '10.0.3' WHERE [settingKey] = 'db_version'
