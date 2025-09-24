if not exists (SELECT * 
FROM sys.indexes 
WHERE name='Product_Discounts' AND object_id = OBJECT_ID('[Catalog].[Product]'))
begin
CREATE NONCLUSTERED INDEX Product_Discounts
ON [Catalog].[Product] ([Enabled],[CategoryEnabled],[Hidden])
INCLUDE ([ProductId],[Discount],[SortDiscount],[DiscountAmount])
end

GO--

ALTER PROCEDURE [Catalog].[sp_GetCategoriesIDsByProductId]
	@ProductID int
AS
BEGIN
	SELECT	CategoryID
	FROM Catalog.ProductCategories
	WHERE ProductID = @ProductID
	order by main desc
END

GO--
if not exists (SELECT * 
FROM sys.indexes 
WHERE name='Photo_Type_Main_objID' AND object_id = OBJECT_ID('[Catalog].[Photo]'))
begin
CREATE NONCLUSTERED INDEX Photo_Type_Main_objID
ON [Catalog].[Photo] ([Type],[Main])
INCLUDE ([ObjId])
end

GO--
ALTER PROCEDURE [Catalog].[sp_AddProductToCategory] 
    @ProductId int,
    @CategoryId int,
    @SortOrder int,
    @MainCategory bit
AS
BEGIN

    DECLARE @Main bit = @MainCategory
    SET NOCOUNT ON;

    IF (@MainCategory = 1)
        UPDATE [Catalog].ProductCategories SET Main = 0 WHERE ProductID = @ProductID
    ELSE
        SET @Main = CASE WHEN EXISTS (SELECT 1 FROM [Catalog].ProductCategories WHERE ProductID = @ProductID AND Main = 1 AND CategoryID <> @CategoryId) THEN 0 ELSE 1 END;

    IF NOT EXISTS (SELECT 1 FROM [Catalog].ProductCategories WHERE CategoryID = @CategoryID AND ProductID = @ProductID)
        INSERT INTO [Catalog].ProductCategories (CategoryID, ProductID, SortOrder, Main) VALUES (@CategoryID, @ProductID, @SortOrder, @Main);
    ELSE
        UPDATE [Catalog].ProductCategories SET Main = @Main WHERE CategoryID = @CategoryID AND ProductID = @ProductID
END

GO--

ALTER PROCEDURE [Catalog].[sp_AddProductToCategoryByExternalId] 
	@ProductID int,
	@ExternalId nvarchar(50),
	@SortOrder int	
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @CategoryId int = (SELECT TOP(1) CategoryId FROM [Catalog].Category WHERE ExternalId = @ExternalId)
	IF @CategoryId IS NOT NULL AND NOT EXISTS (SELECT 1 FROM [Catalog].ProductCategories WHERE CategoryId = @CategoryId and ProductId = @ProductId)
	BEGIN
		DECLARE @Main bit = CASE WHEN EXISTS (SELECT 1 FROM [Catalog].ProductCategories WHERE ProductID = @ProductID AND Main = 1) THEN 0 ELSE 1 END;
		INSERT INTO [Catalog].ProductCategories (CategoryID, ProductID, SortOrder, Main) VALUES (@CategoryId, @ProductId, @SortOrder, @Main);
	END
END

GO--

DECLARE SHOPPING_CART_CURSOR CURSOR  LOCAL STATIC FORWARD_ONLY 
	FOR SELECT MIN([ShoppingCartItemId]), [ShoppingCartType], [CustomerId], [OfferId], [IsGift]
	FROM [Catalog].[ShoppingCart] 
	GROUP BY [ShoppingCartType], [CustomerId], [OfferId], [IsGift] 
	HAVING COUNT(*) > 1

DECLARE @id INT, @Type INT, @CustomerId uniqueidentifier, @OfferId INT, @IsGift BIT
OPEN SHOPPING_CART_CURSOR
FETCH NEXT FROM SHOPPING_CART_CURSOR INTO @id, @Type, @CustomerId, @OfferId, @IsGift
WHILE @@FETCH_STATUS = 0
BEGIN
	DELETE FROM [Catalog].[ShoppingCart] 
		WHERE [ShoppingCartType] = @Type and [CustomerId] = @CustomerId and [OfferId] = @OfferId and [IsGift] = @IsGift and [ShoppingCartItemId] <> @id

	FETCH NEXT FROM SHOPPING_CART_CURSOR INTO @id, @Type, @CustomerId, @OfferId, @IsGift
END
CLOSE SHOPPING_CART_CURSOR
DEALLOCATE SHOPPING_CART_CURSOR

GO--


GO-- 

IF NOT EXISTS(SELECT * FROM [Settings].[Localization] WHERE [ResourceKey] = 'Admin.Settings.Catalog.ShowPropertiesFilterInProductList')
BEGIN
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Catalog.ShowPropertiesFilterInProductList', 'Показывать фильтр по свойствам в списках товаров')
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Catalog.ShowPropertiesFilterInProductList', 'Show filter by properties in product lists')
END

GO-- 

IF NOT EXISTS(SELECT * FROM [Settings].[Localization] WHERE [ResourceKey] = 'Admin.SettingsCatalog.CatalogCommon.ShowPropertiesFilterInProductListNote')
BEGIN
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.SettingsCatalog.CatalogCommon.ShowPropertiesFilterInProductListNote', 'Выводить или нет фильтр по свойствам в списках товаров для главной.')
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.SettingsCatalog.CatalogCommon.ShowPropertiesFilterInProductListNote', 'Whether or not to display a filter by properties in the product lists for the main page.')
END

GO-- 

IF NOT EXISTS(SELECT * FROM [Settings].[Settings] WHERE [Name] = 'ShowPropertiesFilterInProductList')
BEGIN
	INSERT INTO [Settings].[Settings] ([Name],[Value]) VALUES ('ShowPropertiesFilterInProductList', 'False')
END

GO--

IF NOT EXISTS(SELECT * FROM [Settings].[Localization] WHERE [ResourceKey] = 'Core.ExportImport.ExportFeedResellerOptions.PriceMarginType.Percent')
BEGIN
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.ExportImport.ExportFeedResellerOptions.PriceMarginType.Percent', '%')
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.ExportImport.ExportFeedResellerOptions.PriceMarginType.Percent', '%')
	
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.ExportImport.ExportFeedResellerOptions.PriceMarginType.AbsoluteValue', 'абсолютное значение')
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.ExportImport.ExportFeedResellerOptions.PriceMarginType.AbsoluteValue', 'absolute value')

	UPDATE [Settings].[Localization] SET [ResourceValue] = 'Рекомендованная наценка на товар' WHERE [ResourceKey] = 'Admin.ExportFeeed.SettingsReseller.RecomendedPriceMargin' AND [LanguageId] = 1
END

GO--

if not exists (Select 1 From [CMS].[StaticBlock] Where [Key] = 'head_mobile')
	Insert Into [CMS].[StaticBlock] ([Key],[InnerName],[Content],[Added],[Modified],[Enabled]) Values ('head_mobile', 'Блок в head в моб. версии', '', getdate(), getdate(), 1)

GO--

if not exists (Select 1 From [Settings].[Localization] Where [ResourceKey] = 'Admin.ExportFeed.SettingsYandex.ExportCount')
begin
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ExportFeed.SettingsYandex.ExportCount', 'Выгружать количество товара')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ExportFeed.SettingsYandex.ExportCount', 'Export products amount')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ExportFeed.SettingsYandex.ExportCountHelp', 'При активации настройки количество товара будет выгружаться в теге ''count''')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ExportFeed.SettingsYandex.ExportCountHelp', 'Products amount will be exported in the ''count'' tag')
end

if not exists (Select 1 From [Settings].[Localization] Where [ResourceKey] = 'Admin.ExportFeed.SettingsYandex.ExportShopSku')
begin
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ExportFeed.SettingsYandex.ExportShopSku', 'Выгружать тег shop-sku')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ExportFeed.SettingsYandex.ExportShopSku', 'Export shop-sku tag')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ExportFeed.SettingsYandex.ExportShopSkuHelp', 'Выбранный идентификатор товарного предложения будет выгружаться тег shop-sku')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ExportFeed.SettingsYandex.ExportShopSkuHelp', 'The selected product offer ID will be exported in shop-sku tag')
end

if not exists (Select 1 From [Settings].[Localization] Where [ResourceKey] = 'Admin.ExportFeed.SettingsYandex.ExportManufacturer')
begin
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ExportFeed.SettingsYandex.ExportManufacturer', 'Выгружать тег manufacturer')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ExportFeed.SettingsYandex.ExportManufacturer', 'Export manufacturer tag')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ExportFeed.SettingsYandex.ExportManufacturerHelp', 'Производитель будет выгружаться тег manufacturer')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ExportFeed.SettingsYandex.ExportManufacturerHelp', 'Brand name will be exported in manufacturer tag')
end

GO--

if not exists (Select 1 From [Settings].[Localization] Where [ResourceKey] = 'Admin.ExportFeeed.SettingsAvito.Currency')
begin
	Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (1, 'Admin.ExportFeeed.SettingsAvito.Currency', 'Выберите валюту, которая будет использоваться для экспорта'); 
	Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (2, 'Admin.ExportFeeed.SettingsAvito.Currency', 'Select the currency that will be used for export'); 

end

GO--

UPDATE [Settings].[InternalSettings] SET [settingValue] = '8.5.10' WHERE [settingKey] = 'db_version'