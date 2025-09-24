

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.OrderStatus.CreatedFromAdmin', 'Создан заказ из панели администрирования')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.OrderStatus.CreatedFromAdmin', 'Order has been created from admin panel')


IF NOT EXISTS(SELECT 1 FROM sys.columns WHERE Name = N'PhotoNameSize1' AND Object_ID = Object_ID(N'Catalog.Photo'))
BEGIN
ALTER TABLE Catalog.Photo ADD
	PhotoNameSize1 nvarchar(255) NULL,
	PhotoNameSize2 nvarchar(255) NULL
END
GO--


UPDATE [Settings].[InternalSettings] SET [settingValue] = '8.0.5' WHERE [settingKey] = 'db_version'