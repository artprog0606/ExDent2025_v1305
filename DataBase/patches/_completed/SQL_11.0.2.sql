UPDATE [Settings].[Localization] SET [ResourceValue] = N'Дизайн' WHERE [ResourceKey] = 'Admin.Home.Menu.Design' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'Design' WHERE [ResourceKey] = 'Admin.Home.Menu.Design' AND [LanguageId] = 2

GO--

INSERT INTO [Settings].[Localization] (ResourceKey, ResourceValue, LanguageId) VALUES
	('Admin.ImportCsv.EndProcessName', 'Загрузка каталога товаров завершена', 1),
	('Admin.ImportCsv.EndProcessName', 'Loading of the product catalog is completed', 2),
	('Admin.ImportCsvV2.EndProcessName', 'Загрузка каталога товаров завершена', 1),
	('Admin.ImportCsvV2.EndProcessName', 'Loading of the product catalog is completed', 2)

GO--


IF not exists(Select 1 From sys.columns Where Name = N'ShortDescription' AND Object_ID = Object_ID(N'CMS.CarouselApi'))
Begin
	EXEC sp_rename 'CMS.CarouselApi.Text' , 'ShortDescription', 'COLUMN'
End

GO--

If not Exists(Select 1 From sys.columns WHERE Name = N'FullDescription' AND Object_ID = Object_ID(N'CMS.CarouselApi'))
Begin
	ALTER TABLE CMS.CarouselApi ADD
		FullDescription nvarchar(MAX) NULL,
		ExpirationDate datetime NULL,
        ShowOnMain bit NULL,
        CouponCode nvarchar(50) NULL
End

GO--

If not Exists(Select 1 From sys.columns WHERE Name = N'FullDescription' AND Object_ID = Object_ID(N'CMS.CarouselApi'))
Begin
	ALTER TABLE CMS.CarouselApi ADD
		FullDescription nvarchar(MAX) NULL,
		ExpirationDate datetime NULL,
        ShowOnMain bit NULL,
        CouponCode nvarchar(50) NULL
End

GO--

If (Select is_nullable From sys.columns WHERE Name = N'ShowOnMain' AND Object_ID = Object_ID(N'CMS.CarouselApi')) = 1
Begin
	Update CMS.CarouselApi Set ShowOnMain = 1

	ALTER TABLE CMS.CarouselApi ALTER COLUMN ShowOnMain bit NOT NULL
End

GO--

If not Exists(Select 1 From sys.columns WHERE Name = N'ProductId' AND Object_ID = Object_ID(N'CMS.CarouselApi'))
Begin
	ALTER TABLE CMS.CarouselApi ADD
		ProductId int NULL
End

GO--

INSERT INTO [Settings].[Localization] (ResourceKey, ResourceValue, LanguageId) VALUES
	('Admin.PaymentMethods.IntellectMoneyMainProtocol.ReceiptDataGroup', 'Группа устройств', 1),
	('Admin.PaymentMethods.IntellectMoneyMainProtocol.ReceiptDataGroup', 'Device group', 2)

GO--

INSERT INTO [Settings].[Localization] (ResourceKey, ResourceValue, LanguageId) VALUES
	('Admin.Settings.MobileVersion.BlockHeight', 'Высота изображения товара в каталоге при отображении плиткой и списком', 1),
	('Admin.Settings.MobileVersion.BlockHeight', 'The height of the product image in the catalog in the tile and stone catalog', 2);

GO--
    
INSERT INTO [Settings].[Localization] (ResourceKey, ResourceValue, LanguageId) VALUES
	('Admin.Settings.MobileVersion.BlockHeightForBlockHintTitle', 'Размер фото в карточки товара', 1),
	('Admin.Settings.MobileVersion.BlockHeightForBlockHintTitle', 'Photo size in product page', 2);

GO--
    
INSERT INTO [Settings].[Localization] (ResourceKey, ResourceValue, LanguageId) VALUES
	('Admin.Settings.MobileVersion.BlockHeightForBlockHintText', 'Данная настройка также используется для указания максимальной высоты фото в карточке товара.', 1),
	('Admin.Settings.MobileVersion.BlockHeightForBlockHintText', 'This setting is also used to specify the maximum photo height in the product page.', 2);

GO--    
    
INSERT INTO [Settings].[Localization] (ResourceKey, ResourceValue, LanguageId) VALUES
	('Admin.Js.Tasks.Tasks.TaskChangesApplied', 'Cохранены изменения поля "{{field}}" для задачи {{number}}', 1),
	('Admin.Js.Tasks.Tasks.TaskChangesApplied', 'Saved "{{field}}" fields for task {{number}}', 2);

GO--   

INSERT INTO [Settings].[Localization] (ResourceKey, ResourceValue, LanguageId) VALUES
	('Admin.Js.Tasks.EditTask.SaveAndClose', 'Сохранить и закрыть окно', 1),
	('Admin.Js.Tasks.EditTask.SaveAndClose', 'Saved and close window', 2);

GO--   

UPDATE [Settings].[Localization] 
SET ResourceValue = 'Показывать принятые задачи'
WHERE ResourceKey like 'Admin.Tasks.Index.ShowAcceptedTasks' AND LanguageId = 1

UPDATE [Settings].[Localization] 
SET ResourceValue = 'Отклоненные'
WHERE ResourceKey like 'Core.Crm.TaskStatusType.Canceled' AND LanguageId = 1

GO--

ALTER PROCEDURE [Catalog].[sp_GetParentCategories]
	@ChildCategoryId int
AS
BEGIN
	DECLARE @tbl TABLE ( level int not null PRIMARY KEY, id int, name nvarchar(255), url nvarchar(150) )

	DECLARE @id int, @level int
	set @id = @ChildCategoryId
	set @level = 0

	if (select COUNT([CategoryID]) from [Catalog].[Category] where [CategoryID] = @id) <> 0
		while(@id <> 0 AND NOT @id IS NULL)
		begin
			insert into @tbl (level, id, name, url) select @level, [CategoryID], [Name], [UrlPath] from [Catalog].[Category] where [CategoryID] = @id
			set @id = (Select [ParentCategory] from [Catalog].[Category] where [CategoryID] = @id)
			set @level = @level + 1
		end

	if(@id = 0)
		insert into @tbl (level, id, name, url) select @level, [CategoryID], [Name], [UrlPath] from [Catalog].[Category] where [CategoryID] = @id

	SELECT id, name, url FROM @tbl order by level
END


GO--
    
INSERT INTO [Settings].[Localization] (ResourceKey, ResourceValue, LanguageId) VALUES
	('Admin.Settings.MobileVersion.BlockHeightForBlock', 'Высота изображения товара в каталоге при отображении блоком', 1),
	('Admin.Settings.MobileVersion.BlockHeightForBlock', 'The height of the product image in the catalog if there is a block', 2);

GO--

INSERT INTO [Settings].[Localization] (ResourceKey, ResourceValue, LanguageId) VALUES
	('Core.Catalog.EYandexDiscountCondition.ShowcaseSample', 'Витринный образец', 1),
	('Core.Catalog.EYandexDiscountCondition.ShowcaseSample', 'Showcase sample', 2),
	('Core.Catalog.EYandexDiscountCondition.Reduction', 'Уцененный товар', 1),
	('Core.Catalog.EYandexDiscountCondition.Reduction', 'Reduction', 2),
	('Core.Catalog.EYandexDiscountCondition.Refurbished', 'Обновленный', 1),
	('Core.Catalog.EYandexDiscountCondition.Refurbished', 'Refurbished', 2),
	('Core.Catalog.EYandexDiscountCondition.Preowned', 'Бывший в употреблении', 1),
	('Core.Catalog.EYandexDiscountCondition.Preowned', 'Used', 2),
	('Admin.Js.ExportOptions.YandexProductDiscountCondition.Refurbished.Specified', 'Указывается только для одежды и аксессуаров', 1),
	('Admin.Js.ExportOptions.YandexProductDiscountCondition.Refurbished.Specified', 'Specified only for clothing and accessories', 2),
	('Admin.Js.ExportOptions.YandexProductDiscountCondition.Refurbished', 'Обновленный', 1),
	('Admin.Js.ExportOptions.YandexProductDiscountCondition.Refurbished', 'Refurbished', 2);

GO--

INSERT INTO [Settings].[Localization] (ResourceKey, ResourceValue, LanguageId) VALUES
	('Core.Catalog.EYandexProductQuality.None', 'Не выбрано', 1),
	('Core.Catalog.EYandexProductQuality.None', 'None', 2),
	('Core.Catalog.EYandexProductQuality.Perfect', 'Как новый', 1),
	('Core.Catalog.EYandexProductQuality.Perfect', 'Perfect', 2),
	('Core.Catalog.EYandexProductQuality.Excellent', 'Отличный', 1),
	('Core.Catalog.EYandexProductQuality.Excellent', 'Excellent', 2),
	('Core.Catalog.EYandexProductQuality.Good', 'Хороший', 1),
	('Core.Catalog.EYandexProductQuality.Good', 'Good', 2),
	('Core.Catalog.Product.YandexProductQuality', 'Внешний вид', 1),
	('Core.Catalog.Product.YandexProductQuality', 'Appearance', 2),
	('Core.ExportImport.ProductFields.YandexProductQuality', 'Яндекс.Маркет: Внешний вид товара', 1),
	('Core.ExportImport.ProductFields.YandexProductQuality', 'Yandex.Market: Product appearance', 2),
	('Core.ExportImport.EProductField.YandexProductQuality', 'Яндекс.Маркет: Внешний вид товара', 1),
	('Core.ExportImport.EProductField.YandexProductQuality', 'Yandex.Market: Product appearance', 2),
	('Admin.Js.ExportOptions.YandexProductQuality.Help', 
				'<b>Как новый(Б/у товар)</b><br />
				Не больше 2 незаметных царапин или других следов использования, для шин — до 10% износа.<br />
				<b>Как новый(Уцененный товар)</b><br />
				Дефектов нет.<br />
				<b>Отличный(Б/у товар)</b><br />
				Не больше 5 незаметных царапин или других следов использования, для шин — до 25% износа.<br />
				<b>Отличный(Уцененный товар)</b><br />
				Не больше 2 незаметных царапин или потертостей.<br />
				<b>Хороший(Б/у товар)</b><br />
				Не больше 6 заметных царапин, сколов или других следов использования, для шин — до 60% износа.<br />
				<b>Хороший(Уцененный товар)</b><br />
				Не больше 6 заметных царапин, сколов или потертостей.<br />', 1),
	('Admin.Js.ExportOptions.YandexProductQuality.Help', 
				'<b>As new(Used product)</b><br />
				No more than 2 inconspicuous scratches or other traces of use, for tires — up to 10% wear.<br />
				<b>As a new (Discounted item)</b><br />
				There are no defects.<br />
				<b>Excellent(Used product)</b><br />
				No more than 5 inconspicuous scratches or other traces of use, for tires — up to 25% wear.<br />
				<b>Excellent (Discounted product)</b><br />
				No more than 2 inconspicuous scratches or scuffs.<br />
				<b>Good(Used product)</b><br />
				No more than 6 noticeable scratches, chips or other traces of use, for tires — up to 60% wear.<br />
				<b>Good (Discounted product)</b><br />
				No more than 6 noticeable scratches, chips or scuffs.<br />', 2),
	('Admin.Js.ExportOptions.YandexProductQuality', 'Внешний вид', 1),
	('Admin.Js.ExportOptions.YandexProductQuality', 'Appearance', 2)

IF NOT EXISTS(SELECT 1 FROM sys.columns WHERE Name = N'YandexProductQuality' AND Object_ID = Object_ID(N'Catalog.ProductExportOptions'))
BEGIN
	ALTER TABLE Catalog.ProductExportOptions ADD
		YandexProductQuality NVARCHAR(10) NULL
END

GO--

UPDATE [Settings].[InternalSettings] SET [settingValue] = '11.0.2' WHERE [settingKey] = 'db_version'
