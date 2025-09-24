
-- SQL_4.1.10_Part1

UPDATE [Customers].[City]
SET [PhoneNumber] = Null
WHERE (CityID = 1002) AND (PhoneNumber = '+7 (495) 800 200 10<br>')

GO--

update cms.staticblock set content=replace(content, '<a href="http://www.advantshop.net" rel="nofollow" target="_blank">AdVantShop.NET</a>', 'AdVantShop.NET') where staticblockid=1
update cms.staticblock set content=replace(content, '<a href="http://www.advantshop.net" target="_blank">AdVantShop.NET</a>', 'AdVantShop.NET') where staticblockid=1
update cms.staticblock set content=replace(content, '<a data-cke-saved-href="http://www.advantshop.net" href="http://www.advantshop.net" rel="nofollow" target="_blank">AdVantShop.NET</a>', 'AdVantShop.NET') where staticblockid=1
update cms.staticblock set content=replace(content, '<a href="http://advantshop.net">AdVantShop.NET</a>', 'AdVantShop.NET') where staticblockid=1
update cms.staticblock set content=replace(content, '<a href="http://advantshop.net" rel="nofollow" target="_blank"> AdVantShop.NET</a>', 'AdVantShop.NET') where staticblockid=1
update cms.staticblock set content=replace(content, '<a href="http://advantshop.net" rel="nofollow">AdVantShop.NET</a>', 'AdVantShop.NET') where staticblockid=1
update cms.staticblock set content=replace(content, '<a href="http://advantshop.net" rel="nofollow" target="_blank">AdVantShop.NET</a>', 'AdVantShop.NET') where staticblockid=1
update cms.staticblock set content=replace(content, '<a href="http://advantshop.net" rel="nofollow " target="_blank"> AdVantShop.NET</a>', 'AdVantShop.NET') where staticblockid=1

GO--

ALTER PROCEDURE [Catalog].[sp_AddProductToCategory] 
	@ProductId int,
	@CategoryId int,
	@SortOrder int
AS
BEGIN

DECLARE @Main bit
	SET NOCOUNT ON;
if (select count(*) from [Catalog].[ProductCategories] where ProductID=@ProductID and main=1) = 0
	set @Main = 1
else
	set @Main = 0
if (select count(*) from [Catalog].[ProductCategories] where CategoryID=@CategoryID and ProductID=@ProductID) = 0 
begin
	INSERT INTO [Catalog].[ProductCategories] (CategoryID, ProductID, SortOrder, Main) VALUES (@CategoryID, @ProductID, @SortOrder, @Main);
end
END

GO--

ALTER PROCEDURE [Settings].[sp_GetExportFeedProducts] @moduleName NVARCHAR(50)
	,@onlyCount BIT
	,@selectedCurrency NVARCHAR(10)
AS
BEGIN
	DECLARE @res TABLE (productId INT PRIMARY KEY CLUSTERED);
	DECLARE @lproduct TABLE (productId INT PRIMARY KEY CLUSTERED);

	INSERT INTO @lproduct
	SELECT [ProductID]
	FROM [Settings].[ExportFeedSelectedProducts]
	WHERE [ModuleName] = @moduleName;

	DECLARE @lcategory TABLE (CategoryId INT PRIMARY KEY CLUSTERED);
	DECLARE @l TABLE (CategoryId INT PRIMARY KEY CLUSTERED);

	INSERT INTO @l
	SELECT t.CategoryId
	FROM [Settings].[ExportFeedSelectedCategories] AS t
	INNER JOIN CATALOG.Category ON t.CategoryId = Category.CategoryId
	WHERE [ModuleName] = @moduleName
		AND HirecalEnabled = 1
		AND Enabled = 1

	DECLARE @l1 INT

	SET @l1 = (
			SELECT MIN(CategoryId)
			FROM @l
			);

	WHILE @l1 IS NOT NULL
	BEGIN
		INSERT INTO @lcategory
		SELECT id
		FROM Settings.GetChildCategoryByParent(@l1) AS dt
		INNER JOIN CATALOG.Category ON CategoryId = id
		WHERE dt.id NOT IN (
				SELECT CategoryId
				FROM @lcategory
				)
			AND HirecalEnabled = 1
			AND Enabled = 1

		SET @l1 = (
				SELECT MIN(CategoryId)
				FROM @l
				WHERE CategoryId > @l1
				);
	END;

	IF @onlyCount = 1
	BEGIN
		SELECT COUNT(OfferId)
		FROM (
			(
				SELECT OfferId
				FROM [Catalog].[Product]
				INNER JOIN [Catalog].[Offer] ON [Offer].[ProductID] = [Product].[ProductID]
				INNER JOIN [Catalog].[ProductCategories] ON [ProductCategories].[ProductID] = [Product].[ProductID]
					AND (
						CategoryId IN (
							SELECT CategoryId
							FROM @lcategory
							)
						OR [ProductCategories].[ProductID] IN (
							SELECT productId
							FROM @lproduct
							)
						)
				--LEFT JOIN [Catalog].[Photo] ON [Product].[ProductID] = [Photo].[ObjId] and Type ='Product' AND [Photo].[Main] = 1
				WHERE (
						SELECT TOP (1) [ProductCategories].[CategoryId]
						FROM [Catalog].[ProductCategories]
						INNER JOIN [Catalog].[Category] ON [Category].[CategoryId] = [ProductCategories].[CategoryId]
						WHERE [ProductID] = [Product].[ProductID]
							AND [Enabled] = 1
							AND [Main] = 1
						) = [ProductCategories].[CategoryId]
					AND Offer.Price > 0
					AND (
						Offer.Amount > 0
						OR Product.AllowPreOrder = 1
						)
					AND CategoryEnabled = 1
					AND Enabled = 1
				)
			) AS dd
	END
	ELSE
	BEGIN
		DECLARE @defaultCurrencyRatio FLOAT;

		SELECT @defaultCurrencyRatio = CurrencyValue
		FROM [Catalog].[Currency]
		WHERE CurrencyIso3 = @selectedCurrency;

		SELECT [Product].[Enabled]
			,[Product].[ProductID]
			,[Product].[Discount]
			,AllowPreOrder
			,Amount
			,[ProductCategories].[CategoryId] AS [ParentCategory]
			,([Offer].[Price] / @defaultCurrencyRatio) AS Price
			,ShippingPrice
			,[Product].[Name]
			,[Product].[UrlPath]
			,[Product].[Description]
			,[Product].[BriefDescription]
			,[Product].SalesNote
			,OfferId
			,[Offer].ArtNo
			,[Offer].Main
			,[Offer].ColorID
			,ColorName
			,[Offer].SizeID
			,SizeName
			,BrandName
			,GoogleProductCategory
			,Gtin
			,Adult
			,[Settings].PhotoToString(Offer.ColorID, Product.ProductId) AS Photos
		FROM [Catalog].[Product]
		INNER JOIN [Catalog].[Offer] ON [Offer].[ProductID] = [Product].[ProductID]
		INNER JOIN [Catalog].[ProductCategories] ON [ProductCategories].[ProductID] = [Product].[ProductID]
			AND (
				CategoryId IN (
					SELECT CategoryId
					FROM @lcategory
					)
				OR [ProductCategories].[ProductID] IN (
					SELECT productId
					FROM @lproduct
					)
				)
		--LEFT JOIN [Catalog].[Photo] ON [Product].[ProductID] = [Photo].[ObjId] and Type ='Product' AND [Photo].[Main] = 1
		LEFT JOIN [Catalog].[Color] ON [Color].ColorID = [Offer].ColorID
		LEFT JOIN [Catalog].[Size] ON [Size].SizeID = [Offer].SizeID
		LEFT JOIN [Catalog].Brand ON Brand.BrandID = [Product].BrandID
		WHERE (
				SELECT TOP (1) [ProductCategories].[CategoryId]
				FROM [Catalog].[ProductCategories]
				INNER JOIN [Catalog].[Category] ON [Category].[CategoryId] = [ProductCategories].[CategoryId]
				WHERE [ProductID] = [Product].[ProductID]
					AND [Enabled] = 1
					AND [Main] = 1
				) = [ProductCategories].[CategoryId]
			AND Offer.Price > 0
			AND (
				Offer.Amount > 0
				OR Product.AllowPreOrder = 1
				)
			AND CategoryEnabled = 1
			AND Product.Enabled = 1
	END
END

GO--


ALTER procedure [Catalog].[sp_GetPropertyInFilter]
(		
	@categoryId int,
	@useDepth bit
)
AS
begin
if (@useDepth = 1)
	begin
		;WITH products(PropertyValueID)
		AS
		(
			Select distinct PropertyValueID	From [Catalog].[ProductPropertyValue]
			where ProductID in (
								select [ProductCategories].ProductID from [Catalog].[ProductCategories]
								Inner Join [Catalog].Product On [ProductCategories].ProductID = [Product].ProductID and [Product].[Enabled] = 1
								Where [CategoryID] in (Select id from  [Settings].[GetChildCategoryByParent](@categoryId))
								)
		)
		SELECT [PropertyValue].[PropertyValueID],
				[PropertyValue].[PropertyID],
				[PropertyValue].[Value],				
				[Property].Name as  PropertyName,
				[Property].SortOrder as  PropertySortOrder,
				[Property].Expanded as  PropertyExpanded,
				[Property].Unit as  PropertyUnit,
				[Property].Type as  PropertyType				
       
		FROM [Catalog].[PropertyValue]
		Inner Join [Catalog].[Property] On  [Property].PropertyID = [PropertyValue].PropertyID AND [Property].[UseInFilter] = 1 
		Where [PropertyValue].[UseInFilter] = 1 and [PropertyValue].PropertyValueID in (Select PropertyValueID from products) 
		ORDER BY PropertySortOrder, [PropertyValue].[PropertyID], [PropertyValue].[SortOrder], [PropertyValue].[Value]
	end
else
	begin
		;WITH products(PropertyValueID)
		AS
		(
			Select distinct PropertyValueID	From [Catalog].[ProductPropertyValue]
			where ProductID in (
								select [ProductCategories].ProductID from [Catalog].[ProductCategories]
								Inner Join [Catalog].Product On [ProductCategories].ProductID = [Product].ProductID and [Product].[Enabled] = 1
								Where [CategoryID] in (@categoryId)
								)
		)
		SELECT [PropertyValue].[PropertyValueID],
				[PropertyValue].[PropertyID],
				[PropertyValue].[Value],
				[Property].Name as  PropertyName,
				[Property].SortOrder as  PropertySortOrder,
				[Property].Expanded as  PropertyExpanded,
				[Property].Unit as  PropertyUnit,
				[Property].Type as  PropertyType				
       
		FROM [Catalog].[PropertyValue]
		Inner Join [Catalog].[Property] On  [Property].PropertyID = [PropertyValue].PropertyID AND [Property].[UseInFilter] = 1 
		Where [PropertyValue].[UseInFilter] = 1 and [PropertyValue].PropertyValueID in (Select PropertyValueID from products) 
		ORDER BY PropertySortOrder, [PropertyValue].[PropertyID], [PropertyValue].[SortOrder], [PropertyValue].[Value]
	end 
end

GO--


ALTER PROCEDURE [Catalog].[PreCalcProductParams] @productId INT
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @CountPhoto INT
	DECLARE @Type NVARCHAR(10)
	DECLARE @PhotoId INT
	DECLARE @MaxAvailable BIT
	DECLARE @VideosAvailable BIT
	DECLARE @Colors NVARCHAR(max)
	DECLARE @NotSamePrices BIT
	DECLARE @MinPrice FLOAT
	DECLARE @AmountSort BIT
	DECLARE @OfferId int


	if not exists(select  ProductID from [Catalog].Product where ProductID=@productId)
	return

	SET @Type = 'Product'
	--@CountPhoto
	SET @CountPhoto = (
			SELECT CASE 
					WHEN Offer.ColorID IS NOT NULL
						THEN (
								SELECT Count(PhotoId)
								FROM [Catalog].[Photo]
								WHERE (
										[Photo].ColorID = Offer.ColorID
										OR [Photo].ColorID IS NULL
										)
									AND [Photo].[ObjId] = @productId
									AND Type = @Type
								)
					ELSE (
							SELECT Count(PhotoId)
							FROM [Catalog].[Photo]
							WHERE [Photo].[ObjId] = @productId
								AND Type = @Type
							)
					END
			FROM [Catalog].[Offer]
			WHERE [ProductID] = @productId and main =1
			)
	--@PhotoId
	SET @PhotoId = (
			SELECT CASE 
					WHEN Offer.ColorID IS NOT NULL
						THEN (
								SELECT TOP (1) PhotoId
								FROM [Catalog].[Photo]
								WHERE (
										[Photo].ColorID = Offer.ColorID
										OR [Photo].ColorID IS NULL
										)
									AND [Photo].[ObjId] = @productId
									AND Type = @Type
								ORDER BY main DESC
									,[Photo].[PhotoSortOrder]
								)
					ELSE (
							SELECT TOP (1) PhotoId
							FROM [Catalog].[Photo]
							WHERE [Photo].[ObjId] = @productId
								AND Type = @Type
								ORDER BY main DESC
									,[Photo].[PhotoSortOrder]
							)
					END
			FROM [Catalog].[Offer] 
			WHERE [ProductID] = @productId and main =1
			)	
	--VideosAvailable
	IF (
			SELECT COUNT(ProductVideoID)
			FROM [Catalog].[ProductVideo]
			WHERE ProductID = @productId
			) > 0
	BEGIN
		SET @VideosAvailable = 1
	END
	ELSE
	BEGIN
		SET @VideosAvailable = 0
	END

	--@MaxAvailable
	IF (
			SELECT Max(Offer.Amount)
			FROM [Catalog].Offer
			WHERE ProductId = @productId
			) > 0
	BEGIN
		SET @MaxAvailable = 1
	END
	ELSE
	BEGIN
		SET @MaxAvailable = 0
	END

	--AmountSort
	SET @AmountSort = (
			SELECT CASE 
					WHEN @MaxAvailable <= 0
						OR @MaxAvailable < IsNull(Product.MinAmount, 0)
						THEN 0
					ELSE 1
					END
			FROM [Catalog].Offer inner join [Catalog].Product on Product.ProductId=Offer.ProductId
			WHERE Offer.ProductId = @productId
				AND main = 1
			)
	--Colors
	SET @Colors = (
			SELECT [Settings].[ProductColorsToString](@productId)
			)

	--@NotSamePrices
	IF (
			SELECT max(price) - min(price)
			FROM [Catalog].offer
			WHERE offer.productid = @productId
			) > 0
	BEGIN
		SET @NotSamePrices = 1
	END
	ELSE
	BEGIN
		SET @NotSamePrices = 0
	END

	--@MinPrice
	SET @MinPrice = (
			SELECT min(price)
			FROM CATALOG.offer
			WHERE offer.productid = @productId
			)
   Set @OfferId = (SELECT OfferID
			FROM CATALOG.offer
			WHERE offer.productid = @productId and (offer.Main = 1 OR offer.Main IS NULL))			

	IF (
			SELECT Count(productid)
			FROM [Catalog].ProductExt
			WHERE productid = @productId
			) > 0
	BEGIN
		UPDATE [Catalog].[ProductExt]
		SET [CountPhoto] = @CountPhoto
			,[PhotoId] = @PhotoId			
			,[VideosAvailable] = @VideosAvailable
			,[MaxAvailable] = @MaxAvailable
			,[NotSamePrices] = @NotSamePrices
			,[MinPrice] = @MinPrice
			,[Colors] = @Colors
			,[AmountSort] = @AmountSort
			,[OfferId] = @OfferId
		WHERE [ProductId] = @productId
	END
	ELSE
	BEGIN
		INSERT INTO [Catalog].[ProductExt] (
			[ProductId]
			,[CountPhoto]
			,[PhotoId]			
			,[VideosAvailable]
			,[MaxAvailable]
			,[NotSamePrices]
			,[MinPrice]
			,[Colors]
			,[AmountSort]
			,[OfferId]
			)
		VALUES (
			@productId
			,@CountPhoto
			,@PhotoId
			,@VideosAvailable
			,@MaxAvailable
			,@NotSamePrices
			,@MinPrice
			,@Colors
			,@AmountSort
			,@OfferId
			)
	END
END

GO--

Insert Into [Shipping].[CdekCities] ([Id],[CityName],[OblName],[NalSumLimit]) Values (15501,	'Алупка, Ялтинский р-н, АР Крым',	'АР Крым',	'no limit')
Insert Into [Shipping].[CdekCities] ([Id],[CityName],[OblName],[NalSumLimit]) Values (15609,	'Гаспра, Ялтинский р-н, АР Крым',	'АР Крым',	'no limit')
Insert Into [Shipping].[CdekCities] ([Id],[CityName],[OblName],[NalSumLimit]) Values (15608,	'Гурзуф, Ялтинский р-н, АР Крым',	'АР Крым',	'no limit')
Insert Into [Shipping].[CdekCities] ([Id],[CityName],[OblName],[NalSumLimit]) Values (7081,	'Евпатория',	'АР Крым',	'no limit')
Insert Into [Shipping].[CdekCities] ([Id],[CityName],[OblName],[NalSumLimit]) Values (15254,	'Керчь',	'АР Крым',	'no limit')
Insert Into [Shipping].[CdekCities] ([Id],[CityName],[OblName],[NalSumLimit]) Values (15610,	'Кореиз, Ялтинский р-н, АР Крым',	'АР Крым',	'no limit')
Insert Into [Shipping].[CdekCities] ([Id],[CityName],[OblName],[NalSumLimit]) Values (15612,	'Парковое, Ялтинский р-н, АР Крым',	'АР Крым',	'no limit')
Insert Into [Shipping].[CdekCities] ([Id],[CityName],[OblName],[NalSumLimit]) Values (15256,	'Севастополь',	'АР Крым',	'no limit')
Insert Into [Shipping].[CdekCities] ([Id],[CityName],[OblName],[NalSumLimit]) Values (15611,	'Симеиз, Ялтинский р-н, АР Крым',	'АР Крым',	'no limit')
Insert Into [Shipping].[CdekCities] ([Id],[CityName],[OblName],[NalSumLimit]) Values (15345,	'Симферополь',	'АР Крым',	'no limit')
Insert Into [Shipping].[CdekCities] ([Id],[CityName],[OblName],[NalSumLimit]) Values (12963,	'Ялта',	'АР Крым',	'no limit')

GO--

UPDATE [Settings].[InternalSettings] SET [settingValue] = '4.1.10' WHERE [settingKey] = 'db_version'
