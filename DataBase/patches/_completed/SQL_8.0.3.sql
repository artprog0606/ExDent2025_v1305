
ALTER TABLE [Order].[Order] ADD
	TotalWeight float(53) NULL,
	TotalLength float(53) NULL,
	TotalWidth float(53) NULL,
	TotalHeight float(53) NULL

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Landings.OrderSourceName.Funnel','Воронка продаж')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Landings.OrderSourceName.Funnel','Sales funnel')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Orders.OrderType.LeadImport', 'Импорт лидов')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Orders.OrderType.LeadImport', 'Lead import')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.ExportImport.LeadFields.NotSelected', 'Не выбрано')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.ExportImport.LeadFields.NotSelected', 'Not selected')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.ExportImport.LeadFields.SalesFunnel', 'Воронка продаж')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.ExportImport.LeadFields.SalesFunnel', 'Sales funnel')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.ExportImport.LeadFields.DealStatus', 'Этап сделки')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.ExportImport.LeadFields.DealStatus', 'Deal status')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.ExportImport.LeadFields.ManagerName', 'Имя менеджера')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.ExportImport.LeadFields.ManagerName', 'Manager name')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.ExportImport.LeadFields.Title', 'Заголовок')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.ExportImport.LeadFields.Title', 'Title')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.ExportImport.LeadFields.Description', 'Описание')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.ExportImport.LeadFields.Description', 'Description')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.ExportImport.LeadFields.CustomerId', 'Id пользователя')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.ExportImport.LeadFields.CustomerId', 'Customer id')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.ExportImport.LeadFields.FirstName', 'Имя')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.ExportImport.LeadFields.FirstName', 'First name')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.ExportImport.LeadFields.LastName', 'Фамилия')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.ExportImport.LeadFields.LastName', 'Last name')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.ExportImport.LeadFields.Patronymic', 'Отчество')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.ExportImport.LeadFields.Patronymic', 'Patronymic')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.ExportImport.LeadFields.Organization', 'Организация')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.ExportImport.LeadFields.Organization', 'Organization')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.ExportImport.LeadFields.Email', 'email')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.ExportImport.LeadFields.Email', 'Email')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.ExportImport.LeadFields.Phone', 'Телефон')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.ExportImport.LeadFields.Phone', 'Phone')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.ExportImport.LeadFields.Country', 'Страна')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.ExportImport.LeadFields.Country', 'Country')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.ExportImport.LeadFields.Region', 'Регион')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.ExportImport.LeadFields.Region', 'Region')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.ExportImport.LeadFields.City', 'Город')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.ExportImport.LeadFields.City', 'City')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.ExportImport.LeadFields.BirthDay', 'День рождения')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.ExportImport.LeadFields.BirthDay', 'Birthday')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.ExportImport.LeadFields.MultiOffer', 'Артикул:Цена:Количество')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.ExportImport.LeadFields.MultiOffer', 'ArtNo:Price:Amount')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Leads.Index.Export', 'Экспорт в csv')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Leads.Index.Export', 'Export to csv')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Leads.Index.Import', 'Импорт из csv')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Leads.Index.Import', 'Import from csv')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.SettingsCrm.Index.ImportLeads', 'Импорт лидов')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.SettingsCrm.Index.ImportLeads', 'Import leads')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.SettingsCrm.ImportLeads.Import', 'Импортировать')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.SettingsCrm.ImportLeads.Import', 'Import')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.SettingsCrm.ImportLeads.Title', 'Импорт лидов')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.SettingsCrm.ImportLeads.Title', 'Import leads')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.SettingsCrm.ImportLeads.Params', 'Параметры загрузки')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.SettingsCrm.ImportLeads.Params', 'Loading options')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.SettingsCrm.ImportLeads.BasicSalesFunnel', 'Воронка продаж по умолчанию')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.SettingsCrm.ImportLeads.BasicSalesFunnel', 'Default sales funnel')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.SettingsCrm.ImportLeads.BasicSalesFunnel.Help', 'Если в csv файле не указана воронка продаж, то лид будет добавлен в воронку по умолчанию.')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.SettingsCrm.ImportLeads.BasicSalesFunnel.Help', 'If the sales funnel is not specified in the csv file, the lead will be added to the default sales funnel.')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.SettingsCrm.ImportLeads.ColumnSeparator', 'Разделитель между колонками')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.SettingsCrm.ImportLeads.ColumnSeparator', 'Column separator')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.SettingsCrm.ImportLeads.FileEncoding', 'Кодировка файла')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.SettingsCrm.ImportLeads.FileEncoding', 'File encoding')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Import.ImportLeads.PropertySeparator', 'Разделитель между свойствами')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Import.ImportLeads.PropertySeparator', 'Property separator')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Import.ImportLeads.PropertyValueSeparator', 'Разделитель между свойством и значением')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Import.ImportLeads.PropertyValueSeparator', 'Property value separator')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.SettingsCrm.ImportLeads.HasHeader', 'Первая строка файла содержит заголовки')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.SettingsCrm.ImportLeads.HasHeader', 'Has header')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.SettingsCrm.ImportLeads.UpdateCustomer', 'Обновлять информацию о покупателе')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.SettingsCrm.ImportLeads.UpdateCustomer', 'Update сustomer')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.SettingsCrm.ImportLeads.DoNotDuplicate', 'Не дублировать лиды')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.SettingsCrm.ImportLeads.DoNotDuplicate', 'Do not duplicate')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.SettingsCrm.ImportLeads.DoNotDuplicate.Help', 'Если данная опция включена, то для покупателей, которые уже имеют лид в воронке продаж, не будет создан новый лид.')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.SettingsCrm.ImportLeads.DoNotDuplicate.Help', 'If this option is enabled, for customers who already have a lead in the sales funnel, a new lead will not be created.')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.SettingsCrm.ImportLeads.CsvFile', '.Csv файл лидов')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.SettingsCrm.ImportLeads.CsvFile', 'Csv file')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.SettingsCrm.ImportLeads.ColumnInCsv', 'Колонка в .csv файле')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.SettingsCrm.ImportLeads.ColumnInCsv', 'Column in csv')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.SettingsCrm.ImportLeads.FirstLeadData', 'Данные первого лида')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.SettingsCrm.ImportLeads.FirstLeadData', 'First lead data')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.SettingsCrm.ImportLeads.DataType', 'Тип данных')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.SettingsCrm.ImportLeads.DataType', 'Data type')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.MobileVersion.DisplayCategoryMenuIcons', 'Показывать иконки в меню')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.MobileVersion.DisplayCategoryMenuIcons', 'Show icons in menu')

GO--

IF EXISTS (SELECT 1 FROM [dbo].[Modules] WHERE [ModuleStringID] = 'Simaland') AND 
	NOT EXISTS (SELECT 1 FROM Catalog.Category WHERE ExternalId LIKE '%_Simaland')
BEGIN
	UPDATE Catalog.Category SET ExternalId = ExternalId + '_Simaland' WHERE convert(nvarchar(50), CategoryId) <> ExternalId AND CategoryId <> 0
END

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.ExportImport.CustomerFields.CustomerId', 'Id покупателя')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.ExportImport.CustomerFields.CustomerId', 'Customer id')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.ExportImport.CustomerFields.ManagerName', 'Имя менеджера')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.ExportImport.CustomerFields.ManagerName', 'Manager name')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.Customers.Group', 'Группа покупателя')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.Customers.Group', 'Customer group')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Js.Zone.Loading', 'Загрузка данных')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Js.Zone.Loading', 'Loading')

GO--

If not Exists(Select 1 From sys.columns WHERE Name = N'AccessToViewBookingForResourceManagers' AND Object_ID = Object_ID(N'Booking.Affiliate'))
Begin
	ALTER TABLE Booking.Affiliate ADD
		AccessToViewBookingForResourceManagers bit NULL
end

GO--

If Exists(Select 1 From sys.columns WHERE Name = N'AccessToViewBookingForResourceManagers' AND Object_ID = Object_ID(N'Booking.Affiliate'))
Begin
	UPDATE Booking.Affiliate SET AccessToViewBookingForResourceManagers = 0 WHERE AccessToViewBookingForResourceManagers IS NULL
end

GO--

ALTER TABLE Booking.Affiliate ALTER COLUMN
	AccessToViewBookingForResourceManagers bit NOT NULL

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'MyAccount.CommonInfo.Organization', 'Организация')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'MyAccount.CommonInfo.Organization', 'Organization')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.SettingsCrm.VkFailedLogIn', 'Не удалось авторизоваться. Проверьте правильность ввода данных. Если подключена двухфакторная авторизация, отключите её.')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.SettingsCrm.VkFailedLogIn', 'Failed to login. Check the correctness of the data input. If two-factor authentication is enabled, disable it.')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Mobile.Category.Index.ProductsCount', 'Товаров')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Mobile.Category.Index.ProductsCount', 'Number of products')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Import.ImportCustomers.ChangeNewFile', 'Выбрать другой файл')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Import.ImportCustomers.ChangeNewFile', 'Choose another file')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.SettingsCrm.ImportLeads.ChangeNewFile', 'Выбрать другой файл')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.SettingsCrm.ImportLeads.ChangeNewFile', 'Choose another file')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.ExportImport.CustomerFields.ManagerId', 'ID менеджера')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.ExportImport.CustomerFields.ManagerId', 'Manager ID')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.SettingsCrm.ImportLeads.SampleFile', 'Пример файла')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.SettingsCrm.ImportLeads.SampleFile', 'Sample file')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Product.Index.ShippingTo', 'Доставка в')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Product.Index.ShippingTo', 'Shipping to')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Catalog.Index.EditInAdminArea', 'Редактировать категорию через панель администрирования')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Catalog.Index.EditInAdminArea', 'Edit category via admin panel')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'News.NewsItem.EditInAdminArea', 'Редактировать новость через панель администрирования')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'News.NewsItem.EditInAdminArea', 'Edit news via admin panel')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'StaticPage.Index.EditInAdminArea', 'Редактировать статическую страницу через панель администрирования')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'StaticPage.Index.EditInAdminArea', 'Edit static page via admin panel')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Brand.BrandItem.EditInAdminArea', 'Редактировать производителя через панель администрирования')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Brand.BrandItem.EditInAdminArea', 'Edit brand via admin panel')

GO--


UPDATE [Settings].[InternalSettings] SET [settingValue] = '8.0.3' WHERE [settingKey] = 'db_version'