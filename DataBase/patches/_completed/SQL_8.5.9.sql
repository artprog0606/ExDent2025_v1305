UPDATE [Order].[PaymentParam] SET [Value] = 'yoo_money' WHERE [Name] = 'YandexKassa_PaymentType' AND [Value] = 'yandex_money'

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Import.ImportProducts.UpdatePhotos', 'Заменить фотографии товаров')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Import.ImportProducts.UpdatePhotos', 'Replace product photos')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Import.ImportProducts.UpdatePhotosHint', 'Фотографии товара будут заменены фотографиями из csv файла. Старые фотографии будут удалены.')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Import.ImportProducts.UpdatePhotosHint', 'Product photos will be replaced by photos from CSV file. Previos product photos will be removed')

GO--


if not exists (Select 1 From [Settings].[Localization] Where [ResourceKey] = 'Admin.Js.Product.FileDoesNotMeetSizeRequirements')
begin
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.Product.FileDoesNotMeetSizeRequirements', 'Файл не соответствует требованиям к размеру')
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.Product.FileDoesNotMeetSizeRequirements', 'File does not meet size requirements')
end

GO--

If Exists (Select 1 FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_NAME ='FK_VkProduct_Product')
Begin
	ALTER TABLE Vk.VkProduct
		DROP CONSTRAINT FK_VkProduct_Product
End

GO--


If not Exists(Select 1 From sys.columns WHERE Name = N'ItemGroupId' AND Object_ID = Object_ID(N'Vk.VkProduct'))
Begin
	ALTER TABLE Vk.VkProduct ADD
		ItemGroupId bigint NULL,
		OfferId int NULL
End

GO--

if not exists (Select 1 From [Settings].[Localization] Where [ResourceKey] = 'Admin.ExportFeeed.SettingsAvito.Currency')
begin
	Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (1, 'Admin.ExportFeeed.SettingsAvito.Currency', 'Выберите валюту, которая будет использоваться для экспорта'); 
	Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (2, 'Admin.ExportFeeed.SettingsAvito.Currency', 'Select the currency that will be used for export'); 

end

GO--

UPDATE [Settings].[InternalSettings] SET [settingValue] = '8.5.9' WHERE [settingKey] = 'db_version'