EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Order.Address.NotExist', 'Адрес не указан'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Order.Address.NotExist', 'Address not specified' 

GO--

CREATE FUNCTION [Settings].[fn_clearPhone](@Phone VARCHAR(max))
RETURNS BIGINT
AS
BEGIN
	DECLARE 
		@PhoneClean NVARCHAR(max),
		@Counter INT,
		@StringLength INT
 
		SET @PhoneClean = '';
		SET @Counter = 1;
		SET @StringLength = LEN(@Phone);

	WHILE @Counter <= @StringLength
	BEGIN
		DECLARE @CurrentChar NVARCHAR(1) = SUBSTRING(@Phone, @Counter, 1)
		SELECT @PhoneClean = @PhoneClean + CASE 
												WHEN @CurrentChar LIKE '[0-9]' 
													THEN @CurrentChar 
													ELSE ''	
										   END
		SELECT @Counter = @Counter + 1
	END

	DECLARE
		@PhoneCleanLength INT = LEN(@PhoneClean),
		@BigIntMaxLength INT = 18

	IF @PhoneCleanLength <= 1 OR @PhoneCleanLength > @BigIntMaxLength
        RETURN NULL
	RETURN CAST(@PhoneClean AS BIGINT)
END

GO--

UPDATE Customers.Customer SET StandardPhone = (SELECT [Settings].[fn_clearPhone](Phone)) WHERE StandardPhone IS NULL AND Phone IS NOT NULL

GO--

DROP FUNCTION  [Settings].[fn_clearPhone]

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.SettingsApiAuth.MobileAppStoryViewMode.Circle', 'Круг'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.SettingsApiAuth.MobileAppStoryViewMode.Circle', 'Circle'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.SettingsApiAuth.MobileAppStoryViewMode.Square', 'Квадрат'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.SettingsApiAuth.MobileAppStoryViewMode.Square', 'Square'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.SettingsApiAuth.MobileAppStorySizeMode.Middle', 'Средний'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.SettingsApiAuth.MobileAppStorySizeMode.Middle', 'Middle'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.SettingsApiAuth.MobileAppStorySizeMode.Big', 'Большой'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.SettingsApiAuth.MobileAppStorySizeMode.Big', 'Big'

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Template.CountLinesProductNameTitle', 'Число строк в названии товара'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Template.CountLinesProductNameTitle', 'Number of lines in the product name' 

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Template.CountLinesProductNameNote', 'Указываете число, оно означает сколько строк выводить в названии, если название слишком длинное.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Template.CountLinesProductNameNote', 'Specify a number, it means how many lines to display in the title if the title is too long.' 

GO--

IF EXISTS(SELECT * FROM [Settings].[Settings] WHERE [Name] = 'SearchDeep' AND [Value] = 'StrongPhase')
    UPDATE [Settings].[Settings] SET [Value] = 'WordsStartFrom' WHERE [Name] = 'SearchDeep'

GO--

IF NOT EXISTS(SELECT 1
              FROM sys.columns
              WHERE (name = N'RegionName') AND object_id = OBJECT_ID(N'[Shipping].[LPostPickPoints]'))
BEGIN
	ALTER TABLE [Shipping].[LPostPickPoints]
	ADD [RegionName] nvarchar(255) NULL
END

GO--

UPDATE [Shipping].[LPostPickPoints] SET [RegionName] = 'Республика Адыгея' WHERE [RegionCode] = 1
UPDATE [Shipping].[LPostPickPoints] SET [RegionName] = 'Республика Башкортостан' WHERE [RegionCode] = 2
UPDATE [Shipping].[LPostPickPoints] SET [RegionName] = 'Республика Бурятия' WHERE [RegionCode] = 3
UPDATE [Shipping].[LPostPickPoints] SET [RegionName] = 'Республика Алтай' WHERE [RegionCode] = 4
UPDATE [Shipping].[LPostPickPoints] SET [RegionName] = 'Республика Дагестан' WHERE [RegionCode] = 5
UPDATE [Shipping].[LPostPickPoints] SET [RegionName] = 'Республика Ингушетия' WHERE [RegionCode] = 6
UPDATE [Shipping].[LPostPickPoints] SET [RegionName] = 'Кабардино-Балкарская республик' WHERE [RegionCode] = 7
UPDATE [Shipping].[LPostPickPoints] SET [RegionName] = 'Республика Калмыкия' WHERE [RegionCode] = 8
UPDATE [Shipping].[LPostPickPoints] SET [RegionName] = 'Карачаево-Черкесская Республика' WHERE [RegionCode] = 9
UPDATE [Shipping].[LPostPickPoints] SET [RegionName] = 'Республика Карелия' WHERE [RegionCode] = 10
UPDATE [Shipping].[LPostPickPoints] SET [RegionName] = 'Республика Коми' WHERE [RegionCode] = 11
UPDATE [Shipping].[LPostPickPoints] SET [RegionName] = 'Республика Марий Эл' WHERE [RegionCode] = 12
UPDATE [Shipping].[LPostPickPoints] SET [RegionName] = 'Республика Мордовия' WHERE [RegionCode] = 13
UPDATE [Shipping].[LPostPickPoints] SET [RegionName] = 'Республика Саха (Якутия)' WHERE [RegionCode] = 14
UPDATE [Shipping].[LPostPickPoints] SET [RegionName] = 'Республика Северная Осетия - Алания' WHERE [RegionCode] = 15
UPDATE [Shipping].[LPostPickPoints] SET [RegionName] = 'Республика Татарстан' WHERE [RegionCode] = 16
UPDATE [Shipping].[LPostPickPoints] SET [RegionName] = 'Республика Тыва' WHERE [RegionCode] = 17
UPDATE [Shipping].[LPostPickPoints] SET [RegionName] = 'Удмуртская Республика' WHERE [RegionCode] = 18
UPDATE [Shipping].[LPostPickPoints] SET [RegionName] = 'Республика Хакасия' WHERE [RegionCode] = 19
UPDATE [Shipping].[LPostPickPoints] SET [RegionName] = 'Чувашская Республика' WHERE [RegionCode] = 21
UPDATE [Shipping].[LPostPickPoints] SET [RegionName] = 'Алтайский край' WHERE [RegionCode] = 22
UPDATE [Shipping].[LPostPickPoints] SET [RegionName] = 'Краснодарский край' WHERE [RegionCode] = 23
UPDATE [Shipping].[LPostPickPoints] SET [RegionName] = 'Красноярский край' WHERE [RegionCode] = 24
UPDATE [Shipping].[LPostPickPoints] SET [RegionName] = 'Приморский край' WHERE [RegionCode] = 25
UPDATE [Shipping].[LPostPickPoints] SET [RegionName] = 'Ставропольский край' WHERE [RegionCode] = 26
UPDATE [Shipping].[LPostPickPoints] SET [RegionName] = 'Хабаровский край' WHERE [RegionCode] = 27
UPDATE [Shipping].[LPostPickPoints] SET [RegionName] = 'Амурская область' WHERE [RegionCode] = 28
UPDATE [Shipping].[LPostPickPoints] SET [RegionName] = 'Архангельская область' WHERE [RegionCode] = 29
UPDATE [Shipping].[LPostPickPoints] SET [RegionName] = 'Астраханская область' WHERE [RegionCode] = 30
UPDATE [Shipping].[LPostPickPoints] SET [RegionName] = 'Белгородская область' WHERE [RegionCode] = 31
UPDATE [Shipping].[LPostPickPoints] SET [RegionName] = 'Брянская область' WHERE [RegionCode] = 32
UPDATE [Shipping].[LPostPickPoints] SET [RegionName] = 'Владимирская область' WHERE [RegionCode] = 33
UPDATE [Shipping].[LPostPickPoints] SET [RegionName] = 'Волгоградская область' WHERE [RegionCode] = 34
UPDATE [Shipping].[LPostPickPoints] SET [RegionName] = 'Вологодская область' WHERE [RegionCode] = 35
UPDATE [Shipping].[LPostPickPoints] SET [RegionName] = 'Воронежская область' WHERE [RegionCode] = 36
UPDATE [Shipping].[LPostPickPoints] SET [RegionName] = 'Ивановская область' WHERE [RegionCode] = 37
UPDATE [Shipping].[LPostPickPoints] SET [RegionName] = 'Иркутская область' WHERE [RegionCode] = 38
UPDATE [Shipping].[LPostPickPoints] SET [RegionName] = 'Калининградская область' WHERE [RegionCode] = 39
UPDATE [Shipping].[LPostPickPoints] SET [RegionName] = 'Калужская область' WHERE [RegionCode] = 40
UPDATE [Shipping].[LPostPickPoints] SET [RegionName] = 'Камчатский край' WHERE [RegionCode] = 41
UPDATE [Shipping].[LPostPickPoints] SET [RegionName] = 'Кемеровская область' WHERE [RegionCode] = 42
UPDATE [Shipping].[LPostPickPoints] SET [RegionName] = 'Кировская область' WHERE [RegionCode] = 43
UPDATE [Shipping].[LPostPickPoints] SET [RegionName] = 'Костромская область' WHERE [RegionCode] = 44
UPDATE [Shipping].[LPostPickPoints] SET [RegionName] = 'Курганская область' WHERE [RegionCode] = 45
UPDATE [Shipping].[LPostPickPoints] SET [RegionName] = 'Курская область' WHERE [RegionCode] = 46
UPDATE [Shipping].[LPostPickPoints] SET [RegionName] = 'Ленинградская область' WHERE [RegionCode] = 47
UPDATE [Shipping].[LPostPickPoints] SET [RegionName] = 'Липецкая область' WHERE [RegionCode] = 48
UPDATE [Shipping].[LPostPickPoints] SET [RegionName] = 'Магаданская область' WHERE [RegionCode] = 49
UPDATE [Shipping].[LPostPickPoints] SET [RegionName] = 'Московская область' WHERE [RegionCode] = 50
UPDATE [Shipping].[LPostPickPoints] SET [RegionName] = 'Мурманская область' WHERE [RegionCode] = 51
UPDATE [Shipping].[LPostPickPoints] SET [RegionName] = 'Нижегородская область' WHERE [RegionCode] = 52
UPDATE [Shipping].[LPostPickPoints] SET [RegionName] = 'Новгородская область' WHERE [RegionCode] = 53
UPDATE [Shipping].[LPostPickPoints] SET [RegionName] = 'Новосибирская область' WHERE [RegionCode] = 54
UPDATE [Shipping].[LPostPickPoints] SET [RegionName] = 'Омская область' WHERE [RegionCode] = 55
UPDATE [Shipping].[LPostPickPoints] SET [RegionName] = 'Оренбургская область' WHERE [RegionCode] = 56
UPDATE [Shipping].[LPostPickPoints] SET [RegionName] = 'Орловская область' WHERE [RegionCode] = 57
UPDATE [Shipping].[LPostPickPoints] SET [RegionName] = 'Пензенская область' WHERE [RegionCode] = 58
UPDATE [Shipping].[LPostPickPoints] SET [RegionName] = 'Пермский край' WHERE [RegionCode] = 59
UPDATE [Shipping].[LPostPickPoints] SET [RegionName] = 'Псковская область' WHERE [RegionCode] = 60
UPDATE [Shipping].[LPostPickPoints] SET [RegionName] = 'Ростовская область' WHERE [RegionCode] = 61
UPDATE [Shipping].[LPostPickPoints] SET [RegionName] = 'Рязанская область' WHERE [RegionCode] = 62
UPDATE [Shipping].[LPostPickPoints] SET [RegionName] = 'Самарская область' WHERE [RegionCode] = 63
UPDATE [Shipping].[LPostPickPoints] SET [RegionName] = 'Саратовская область' WHERE [RegionCode] = 64
UPDATE [Shipping].[LPostPickPoints] SET [RegionName] = 'Сахалинская область' WHERE [RegionCode] = 65
UPDATE [Shipping].[LPostPickPoints] SET [RegionName] = 'Свердловская область' WHERE [RegionCode] = 66
UPDATE [Shipping].[LPostPickPoints] SET [RegionName] = 'Смоленская область' WHERE [RegionCode] = 67
UPDATE [Shipping].[LPostPickPoints] SET [RegionName] = 'Тамбовская область' WHERE [RegionCode] = 68
UPDATE [Shipping].[LPostPickPoints] SET [RegionName] = 'Тверская область' WHERE [RegionCode] = 69
UPDATE [Shipping].[LPostPickPoints] SET [RegionName] = 'Томская область' WHERE [RegionCode] = 70
UPDATE [Shipping].[LPostPickPoints] SET [RegionName] = 'Тульская область' WHERE [RegionCode] = 71
UPDATE [Shipping].[LPostPickPoints] SET [RegionName] = 'Тюменская область' WHERE [RegionCode] = 72
UPDATE [Shipping].[LPostPickPoints] SET [RegionName] = 'Ульяновская область' WHERE [RegionCode] = 73
UPDATE [Shipping].[LPostPickPoints] SET [RegionName] = 'Челябинская область' WHERE [RegionCode] = 74
UPDATE [Shipping].[LPostPickPoints] SET [RegionName] = 'Забайкальский край' WHERE [RegionCode] = 75
UPDATE [Shipping].[LPostPickPoints] SET [RegionName] = 'Ярославская область' WHERE [RegionCode] = 76
UPDATE [Shipping].[LPostPickPoints] SET [RegionName] = 'Москва' WHERE [RegionCode] = 77
UPDATE [Shipping].[LPostPickPoints] SET [RegionName] = 'Санкт-Петербург' WHERE [RegionCode] = 78
UPDATE [Shipping].[LPostPickPoints] SET [RegionName] = 'Еврейская АО' WHERE [RegionCode] = 79
UPDATE [Shipping].[LPostPickPoints] SET [RegionName] = 'Агинский Бурятский автономный округ' WHERE [RegionCode] = 80
UPDATE [Shipping].[LPostPickPoints] SET [RegionName] = 'Коми-Пермяцкий автономный округ' WHERE [RegionCode] = 81
UPDATE [Shipping].[LPostPickPoints] SET [RegionName] = 'Республика Крым' WHERE [RegionCode] = 82
UPDATE [Shipping].[LPostPickPoints] SET [RegionName] = 'Ненецкий АО' WHERE [RegionCode] = 83
UPDATE [Shipping].[LPostPickPoints] SET [RegionName] = 'Таймырский (Долгано - Ненецкий) АО' WHERE [RegionCode] = 84
UPDATE [Shipping].[LPostPickPoints] SET [RegionName] = 'Усть-Ордынский Бурятский автономный округ' WHERE [RegionCode] = 85
UPDATE [Shipping].[LPostPickPoints] SET [RegionName] = 'Ханты-Мансийский АО - Югра' WHERE [RegionCode] = 86
UPDATE [Shipping].[LPostPickPoints] SET [RegionName] = 'Чукотский АО' WHERE [RegionCode] = 87
UPDATE [Shipping].[LPostPickPoints] SET [RegionName] = 'Эвенкийский автономный округ' WHERE [RegionCode] = 88
UPDATE [Shipping].[LPostPickPoints] SET [RegionName] = 'Ямало-Ненецкий АО' WHERE [RegionCode] = 89
UPDATE [Shipping].[LPostPickPoints] SET [RegionName] = 'Севастополь' WHERE [RegionCode] = 92
UPDATE [Shipping].[LPostPickPoints] SET [RegionName] = 'Чеченская Республика' WHERE [RegionCode] = 95
UPDATE [Shipping].[LPostPickPoints] SET [RegionName] = 'Республика Татарстан' WHERE [RegionCode] = 101

GO--

IF EXISTS(SELECT 1
              FROM sys.columns
              WHERE (name = N'RegionName') AND object_id = OBJECT_ID(N'[Shipping].[LPostPickPoints]'))
BEGIN
	ALTER TABLE [Shipping].[LPostPickPoints]
	ALTER COLUMN [RegionName] nvarchar(255) NOT NULL
END

GO--

IF EXISTS (SELECT 1
		   FROM sys.columns
		   WHERE (name = N'CustomerGroupId') AND object_id = OBJECT_ID(N'[Catalog].[Coupon]'))
	BEGIN
		ALTER TABLE [Catalog].[Coupon] DROP COLUMN CustomerGroupId
	END

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Template.CountLinesProductNameNote', 'Данная настройка позволяет выставить количество строк для отображения названия товара в категории. Если название товара более длинное, оно будет скрыто.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Template.CountLinesProductNameNote', 'This setting allows you to set the number of lines for displaying the product name in the category. If the product name is longer, it will be hidden.' 

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.Builder.CountLinesProductName', 'Число строк в названии товара'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.Builder.CountLinesProductName', 'Number of lines in the product name' 

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.Builder.CountLinesProductNameNote', 'Данная настройка позволяет выставить количество строк для отображения названия товара в категории. Если название товара более длинное, оно будет скрыто.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.Builder.CountLinesProductNameNote', 'This setting allows you to set the number of lines for displaying the product name in the category. If the product name is longer, it will be hidden.' 

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Catalog.MenuCatalog.ViewAll', N'Посмотреть все'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Catalog.MenuCatalog.ViewAll', 'View all'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Catalog.MenuCatalogSubCategory.ViewAll', N'Посмотреть все...'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Catalog.MenuCatalogSubCategory.ViewAll', 'View all...'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Catalog.MenuCatalogSubCategory.More', N'Ещё...'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Catalog.MenuCatalogSubCategory.More', 'More...'

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Account.Logo', 'ADVANTSHOP'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Account.Logo', 'ADVANTSHOP' 

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Account.Copyright.Text', 'AdvantShop'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Account.Copyright.Text', 'AdvantShop' 

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Account.Copyright.Link', 'https://www.advantshop.net/'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Account.Copyright.Link', 'https://www.advantshop.net/' 

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Common.TopPanel.Logo', 'AdvantShop'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Common.TopPanel.Logo', 'AdvantShop' 

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.MobileVersion.DisplayShowAddButton', 'Отображать кнопку добавления в корзину в списке товаров'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.MobileVersion.DisplayShowAddButton', 'Display the add to cart button in the list of products' 

GO--

if not Exists (Select 1 From [CMS].[StaticBlock] Where [Key] = 'SberbankInvoice')
	Insert Into [CMS].[StaticBlock] ([Key],[InnerName],[Content],[Added],[Modified],[Enabled]) Values ('SberbankInvoice', 'Квитанция Сбербанка', '', getdate(), getdate(), 1)

if not Exists (Select 1 From [CMS].[StaticBlock] Where [Key] = 'BillInvoice')
	Insert Into [CMS].[StaticBlock] ([Key],[InnerName],[Content],[Added],[Modified],[Enabled]) Values ('BillInvoice', 'Счет на оплату', '', getdate(), getdate(), 1)

if not Exists (Select 1 From [CMS].[StaticBlock] Where [Key] = 'BillByInvoice')
	Insert Into [CMS].[StaticBlock] ([Key],[InnerName],[Content],[Added],[Modified],[Enabled]) Values ('BillByInvoice', 'Счет на оплату (Беларусь)', '', getdate(), getdate(), 1)

if not Exists (Select 1 From [CMS].[StaticBlock] Where [Key] = 'BillKzInvoice')
	Insert Into [CMS].[StaticBlock] ([Key],[InnerName],[Content],[Added],[Modified],[Enabled]) Values ('BillKzInvoice', 'Счет на оплату (Казахстан)', '', getdate(), getdate(), 1)

if not Exists (Select 1 From [CMS].[StaticBlock] Where [Key] = 'CheckInvoice')
	Insert Into [CMS].[StaticBlock] ([Key],[InnerName],[Content],[Added],[Modified],[Enabled]) Values ('CheckInvoice', 'Чек', '', getdate(), getdate(), 1)

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.TypesOfCustomers.MinimalOrderPriceForPhysicalEntity', 'Минимальная сумма заказа для физических лиц'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.TypesOfCustomers.MinimalOrderPriceForPhysicalEntity', 'Minimum order amount for physical entities'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.TypesOfCustomers.MinimalOrderPriceForPhysicalEntityHint', 'Параметр определяет минимальную сумму заказа для физических лиц, ниже которой нельзя оформить заказ.<br /><br />Если сумма заказа в корзине будет ниже, чем указанное значение, клиенту покажется сообщение с уведомлением.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.TypesOfCustomers.MinimalOrderPriceForPhysicalEntityHint', 'The parameter defines the minimum order amount for physical entity below which it is impossible to place an order.<br /><br />If the order amount in the cart is lower than the specified value, the customer will see a notification message.'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.TypesOfCustomers.MinimalOrderPriceForLegalEntity', 'Минимальная сумма заказа для юридических лиц'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.TypesOfCustomers.MinimalOrderPriceForLegalEntity', 'Minimum order amount for legal entities'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.TypesOfCustomers.MinimalOrderPriceForLegalEntityHint', 'Параметр определяет минимальную сумму заказа для юридических лиц, ниже которой нельзя оформить заказ.<br /><br />Если сумма заказа в корзине будет ниже, чем указанное значение, клиенту покажется сообщение с уведомлением.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.TypesOfCustomers.MinimalOrderPriceForLegalEntityHint', 'The parameter defines the minimum order amount for legal entity below which it is impossible to place an order.<br /><br />If the order amount in the cart is lower than the specified value, the customer will see a notification message.'

GO--

IF NOT EXISTS (SELECT * FROM [Settings].[Settings] WHERE [Name] = 'Mobile_CountLinesProductName')
    BEGIN
        declare @countLineProducts nvarchar(3)
        set @countLineProducts = ISNULL((SELECT TOP (1) [Value]
                                         FROM [Settings].[TemplateSettings]
                                         WHERE [Name] LIKE 'Mobile_CountLinesProductName%'
                                           AND [Template] =
                                               (SELECT TOP (1) (CASE [Value] WHEN '' then '_default' else [Value] end)
                                                FROM [Settings].[Settings]
                                                WHERE [Name] = 'Template')),
                                        3);
        INSERT INTO [Settings].[Settings] (Name, Value)
        VALUES ('Mobile_CountLinesProductName', @countLineProducts)
    END

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Import.ImportProducts.AddProductInParentCategory', 'Автоматически добавлять в родительские категории'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Import.ImportProducts.AddProductInParentCategory', 'Automatically add to parent categories'

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Catalog.CreatedBy', 'Источник создания'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Catalog.CreatedBy', 'Source of creation' 

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Catalog.CreatedByOptions.OtherSource', 'Иной источник'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Catalog.CreatedByOptions.OtherSource', 'Other source' 

IF EXISTS(SELECT * FROM sys.indexes WHERE name = 'Product_CreatedBy' AND object_id = OBJECT_ID(N'[Catalog].[Product]'))
	DROP INDEX [Product_CreatedBy] ON [Catalog].[Product]

GO--

CREATE NONCLUSTERED INDEX [Product_CreatedBy] ON [Catalog].[Product]
(
	[CreatedBy] ASC
) WITH (PAD_INDEX = OFF, SORT_IN_TEMPDB = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]

GO--

ALTER TABLE Customers.Task ADD
    BizProcessRuleId int NULL
GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Catalog.BarCode', 'Штрихкод'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Catalog.BarCode', 'BarCode'

GO--

IF EXISTS(SELECT * FROM sys.procedures WHERE name = 'sp_GetChildCategoriesByParentIDForMenu' and schema_id = 5)
    DROP PROCEDURE [Catalog].[sp_GetChildCategoriesByParentIDForMenu]

GO--

UPDATE [Settings].[InternalSettings] SET [settingValue] = '12.0.3' WHERE [settingKey] = 'db_version'
