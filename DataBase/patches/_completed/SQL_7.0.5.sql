INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.AddEditYandexPromo.WarningCouponWasDeleted', 'Купон был удален')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.AddEditYandexPromo.WarningCouponWasDeleted', 'Сoupon was deleted')
UPDATE [Settings].[Localization] SET [ResourceValue] = 'Нет доступных купонов, добавьте новый' WHERE [LanguageId] = '1' AND [ResourceKey] = 'Admin.Js.AddEditYandexPromo.ErrorGettingCouponsCount'

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Mobile.Search.CatalogFilter', 'Фильтры товаров')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Mobile.Search.CatalogFilter', 'Product filters')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Mobile.Catalog.Index.CatalogFilter', 'Фильтры товаров')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Mobile.Catalog.Index.CatalogFilter', 'Product filters')

GO--


ALTER PROCEDURE [Settings].[sp_GetExportFeedProducts] 
     @exportFeedId int
	,@exportNotAvailable bit
	,@selectedCurrency NVARCHAR(10)
	,@allowPreOrder bit = 0
	,@exportAllProducts bit
	,@onlyMainOfferToExport bit
	,@sqlMode NVARCHAR(200) = 'GetProducts'
AS
BEGIN
	
	DECLARE @res TABLE (productId INT PRIMARY KEY CLUSTERED);
	
	DECLARE @lcategory TABLE (CategoryId INT PRIMARY KEY CLUSTERED);
	DECLARE @lcategorytemp TABLE (CategoryId INT);
	DECLARE @l TABLE (CategoryId INT PRIMARY KEY CLUSTERED, Opened bit);

	INSERT INTO @l
	SELECT t.CategoryId, t.Opened
	FROM [Settings].[ExportFeedSelectedCategories] AS t
	INNER JOIN CATALOG.Category ON t.CategoryId = Category.CategoryId
	WHERE [ExportFeedId] = @exportFeedId
		AND HirecalEnabled = 1
		AND Enabled = 1


	DECLARE @l1 INT

	SET @l1 = (
			SELECT MIN(CategoryId)
			FROM @l
			);

	WHILE @l1 IS NOT NULL
	BEGIN
		if ((Select Opened from @l where CategoryId=@l1)=1)
		begin
			INSERT INTO @lcategorytemp
			SELECT @l1
		end
		else
		begin
	 		INSERT INTO @lcategorytemp
			SELECT id
			FROM Settings.GetChildCategoryByParent(@l1)
		end

		SET @l1 = (
				SELECT MIN(CategoryId)
				FROM @l
				WHERE CategoryId > @l1
				);
	END;

	INSERT INTO @lcategory
	SELECT Distinct tmp.CategoryId
	FROM @lcategorytemp AS tmp
	INNER JOIN CATALOG.Category ON Category.CategoryId = tmp.CategoryId
	WHERE HirecalEnabled = 1
		AND Enabled = 1;

	IF @sqlMode = 'GetCountOfProducts'
	BEGIN
		SELECT COUNT(Distinct OfferId)
		FROM [Catalog].[Product] p 
		INNER JOIN [Catalog].[Offer] offer ON offer.[ProductID] = p.[ProductID]
		INNER JOIN [Catalog].[ProductCategories] productCategories ON productCategories.[ProductID] = p.[ProductID]
		INNER JOIN  @lcategory lc ON lc.CategoryId = productCategories.CategoryID
		LEFT JOIN [Settings].[ExportFeedExcludedProducts]ep ON ep.ProductId = p.ProductId and ep.ExportFeedId=@exportFeedId															
		WHERE 
		(
			ep.ProductID IS NULL 
			OR 
			@exportAllProducts = 1
		)
		AND
			(offer.Price > 0 OR @exportNotAvailable = 1)
		AND (
			offer.Amount > 0
			OR (p.AllowPreOrder = 1 and  @allowPreOrder = 1)
			OR @exportNotAvailable = 1
			)
		AND CategoryEnabled = 1
		AND (p.Enabled = 1 OR @exportNotAvailable = 1)	
		AND (@onlyMainOfferToExport = 0 OR Offer.Main = 1)
	END
	IF @sqlMode = 'GetProducts'
	BEGIN
	with cte as (
	SELECT Distinct tmp.CategoryId
	FROM @lcategorytemp AS tmp
	INNER JOIN CATALOG.Category ON Category.CategoryId = tmp.CategoryId

	WHERE HirecalEnabled = 1 AND Enabled = 1)	
		SELECT p.[Enabled]
			,p.[ProductID]
			,p.[Discount]
			,p.[DiscountAmount]
			,AllowPreOrder
			,Amount
			,crossCategory.[CategoryId] AS [ParentCategory]
			,[Offer].[Price] AS Price
			,ShippingPrice
			,p.[Name]
			,p.[UrlPath]
			,p.[Description]
			,p.[BriefDescription]
			,p.SalesNote
			,OfferId
			,p.ArtNo
			,[Offer].Main
			,[Offer].ColorID
			,ColorName
			,[Offer].SizeID
			,SizeName
			,BrandName
			,country1.CountryName as BrandCountry
			,country2.CountryName as BrandCountryManufacture
			,GoogleProductCategory
			,YandexMarketCategory
			,YandexTypePrefix
			,YandexModel
			,Gtin
			,Adult
			,CurrencyValue
			,[Settings].PhotoToString(Offer.ColorID, p.ProductId) AS Photos
			,ManufacturerWarranty
			,[Weight]
			,p.[Enabled]
			,[Offer].SupplyPrice AS SupplyPrice
			,[Offer].ArtNo AS OfferArtNo
			,p.BarCode
			,p.Bid			
			,p.YandexSizeUnit
			,p.MinAmount
			,p.Multiplicity			
			,p.YandexName
			,p.YandexDeliveryDays
			,p.Length
			,p.Width
			,p.Height		
		FROM [Catalog].[Product] p 
		INNER JOIN [Catalog].[Offer] offer ON offer.[ProductID] = p.[ProductID]
		--INNER JOIN [Catalog].[ProductCategories] productCategories ON productCategories.[ProductID] = p.[ProductID]
		--RIGHT JOIN  @lcategory lc ON lc.CategoryId = productCategories.CategoryID
		LEFT JOIN [Settings].[ExportFeedExcludedProducts]ep ON ep.ProductId = p.ProductId and ep.ExportFeedId=@exportFeedId		
		LEFT JOIN [Catalog].[Color] ON [Color].ColorID = [Offer].ColorID
		LEFT JOIN [Catalog].[Size] ON [Size].SizeID = [Offer].SizeID
		LEFT JOIN [Catalog].Brand ON Brand.BrandID = p.BrandID
		LEFT JOIN [Customers].Country as country1 ON Brand.CountryID = country1.CountryID
		LEFT JOIN [Customers].Country as country2 ON Brand.CountryOfManufactureID = country2.CountryID
		INNER JOIN [Catalog].Currency ON Currency.CurrencyID = p.CurrencyID
		cross apply(SELECT TOP (1) [ProductCategories].[CategoryId] from [Catalog].[ProductCategories]
					INNER JOIN  cte lc ON lc.CategoryId = productCategories.CategoryID
					where  [ProductCategories].[ProductID] = p.[ProductID]
					Order By [ProductCategories].Main DESC, [ProductCategories].[CategoryId] ) crossCategory	
		WHERE 
		(
			ep.ProductID IS NULL 
			OR 
			@exportAllProducts = 1
		)		
		AND
			(offer.Price > 0 OR @exportNotAvailable = 1)
		AND (
			offer.Amount > 0
			OR (p.AllowPreOrder = 1 and  @allowPreOrder = 1)
			OR @exportNotAvailable = 1
			)
		AND CategoryEnabled = 1
		AND (p.Enabled = 1 OR @exportNotAvailable = 1)	
		AND (@onlyMainOfferToExport = 0 OR Offer.Main = 1)
	END
	IF @sqlMode = 'GetOfferIds'
	BEGIN
		SELECT Distinct OfferId
		FROM [Catalog].[Product] p 
		INNER JOIN [Catalog].[Offer] offer ON offer.[ProductID] = p.[ProductID]
		INNER JOIN [Catalog].[ProductCategories] productCategories ON productCategories.[ProductID] = p.[ProductID]
		INNER JOIN  @lcategory lc ON lc.CategoryId = productCategories.CategoryID
		LEFT JOIN [Settings].[ExportFeedExcludedProducts]ep ON ep.ProductId = p.ProductId and ep.ExportFeedId=@exportFeedId															
		WHERE 
		(
			ep.ProductID IS NULL 
			OR 
			@exportAllProducts = 1
		)
		AND
			(offer.Price > 0 OR @exportNotAvailable = 1)
		AND (
			offer.Amount > 0
			OR (p.AllowPreOrder = 1 and  @allowPreOrder = 1)
			OR @exportNotAvailable = 1
			)
		AND CategoryEnabled = 1
		AND (p.Enabled = 1 OR @exportNotAvailable = 1)	
		AND (@onlyMainOfferToExport = 0 OR Offer.Main = 1)
	END
END

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ExportFeed.SettingsYandex.ExportDimensions', 'Выгружать габариты товара')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ExportFeed.SettingsYandex.ExportDimensions', 'Export dimensionsns')

GO--
UPDATE [Settings].[Localization] SET [ResourceValue] = 'Нет доступных купонов, добавьте новый' WHERE [LanguageId] = '1' AND [ResourceKey] = 'Admin.Js.AddEditYandexPromo.ErrorGettingCouponsCount'

GO--


CREATE TABLE [CMS].[LandingSite_Product](
	[ProductId] [int] NOT NULL,
	[LandingSiteId] [int] NOT NULL,
 CONSTRAINT [PK_LandingSite_Product] PRIMARY KEY CLUSTERED 
(
	[ProductId] ASC,
	[LandingSiteId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO--

ALTER TABLE [CMS].[LandingSite_Product]  WITH CHECK ADD  CONSTRAINT [FK_LandingSite_Product_LandingSite] FOREIGN KEY([LandingSiteId])
REFERENCES [CMS].[LandingSite] ([Id])
ON DELETE CASCADE
GO--

ALTER TABLE [CMS].[LandingSite_Product] CHECK CONSTRAINT [FK_LandingSite_Product_LandingSite]
GO--

ALTER TABLE [CMS].[LandingSite_Product]  WITH CHECK ADD  CONSTRAINT [FK_LandingSite_Product_Product] FOREIGN KEY([ProductId])
REFERENCES [Catalog].[Product] ([ProductId])
ON DELETE CASCADE
GO--

ALTER TABLE [CMS].[LandingSite_Product] CHECK CONSTRAINT [FK_LandingSite_Product_Product]
GO--

Insert Into [CMS].[LandingSite_Product] (ProductId, LandingSiteId) 
	Select [AdditionalSalesProductId], [Id] From [CMS].[LandingSite] Where [AdditionalSalesProductId] is not null and exists(select 1 from [Catalog].[Product] Where ProductId = [AdditionalSalesProductId])
GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Mobile.Product.ReturnText', 'Назад')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Mobile.Product.ReturnText', 'Back')

GO--

ALTER PROCEDURE [Catalog].[sp_IncCountProductInCategory]		
	@CategoryID int,
	@client bit = 1,
	@current bit = 1
AS
BEGIN
	IF (@client = 0)
		IF (@current = 0)
			UPDATE [Catalog].[Category] 
			SET Products_Count = Products_Count + 1, Total_Products_Count = Total_Products_Count + 1, 
				Available_Products_Count = Available_Products_Count + 1 
			WHERE [CategoryID] = @CategoryID
		ELSE
			UPDATE [Catalog].[Category] 
			SET Products_Count = Products_Count + 1, Total_Products_Count = Total_Products_Count + 1, [Current_Products_Count] = [Current_Products_Count] + 1, 
				Available_Products_Count = Available_Products_Count + 1
			WHERE [CategoryID] = @CategoryID
	ELSE
		UPDATE [Catalog].[Category] SET Products_Count = Products_Count + 1 WHERE [CategoryID] = @CategoryID
	
	DECLARE @parentCategoryId int;
	SELECT @parentCategoryId = [ParentCategory] FROM [Catalog].[Category] WHERE [CategoryID] = @CategoryId
	IF(@parentCategoryId <> 0)
		EXEC [Catalog].[sp_IncCountProductInCategory] @parentCategoryId, @client, 0;
END

GO--

ALTER PROCEDURE [Catalog].[sp_DeIncCountProductInCategory]
			@CategoryID int,
			@client bit = 1,		
			@current bit = 1
AS
BEGIN
	IF (@client = 0)
		IF (@current = 0)
			UPDATE [Catalog].[Category] 
			SET Products_Count = Products_Count - 1, Total_Products_Count = Total_Products_Count - 1, 
				Available_Products_Count = Available_Products_Count - 1 
			WHERE CategoryID = @CategoryID
		ELSE
			UPDATE [Catalog].[Category] 
			SET Products_Count = Products_Count - 1, Total_Products_Count = Total_Products_Count - 1, [Current_Products_Count] = [Current_Products_Count] - 1, 
				Available_Products_Count = Available_Products_Count - 1 
			WHERE CategoryID = @CategoryID
	ELSE
		UPDATE [Catalog].[Category] SET Products_Count = Products_Count - 1 WHERE CategoryID = @CategoryID

		
		DECLARE @parentCategoryId int;
		SELECT @parentCategoryId = [ParentCategory] FROM [Catalog].[Category] WHERE [CategoryID] = @CategoryId
		IF(@parentCategoryId <> 0)
			EXEC [Catalog].[sp_DeIncCountProductInCategory] @parentCategoryId, @client, 0;
END

GO--

UPDATE [Settings].[InternalSettings] SET [settingValue] = '7.0.5' WHERE [settingKey] = 'db_version'