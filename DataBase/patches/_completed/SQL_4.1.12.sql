
-- SQL_4.1.12_Part1 (1C Fix)

if (select count(*) from [Settings].[Settings] where [Name]='Api_ApiKey') = 0
	Insert Into [Settings].[Settings] ([Name],[Value]) Values ('Api_ApiKey', '')
GO--

ALTER TABLE [Catalog].[Property] ADD [UseInBrief] bit null
GO--

UPDATE Catalog.Property set [UseInBrief]=0 where [UseInBrief] is null

GO--

ALTER TABLE [Catalog].[Property] ALTER COLUMN [UseInBrief] bit not null

GO--

ALTER PROCEDURE [Catalog].[sp_AddProperty]
	@Name nvarchar(100),
	@UseInFilter bit = 0, 
	@SortOrder int = 0,
	@Expanded bit = 0,
	@UseInDetails bit = 1,
	@Description nvarchar(500),
	@Unit nvarchar(25),
	@Type tinyint,
	@GroupId int,
	@UseInBrief bit = 0
AS
BEGIN
	SET NOCOUNT ON;

	INSERT INTO [Catalog].[Property] (Name, SortOrder, UseInFilter, Expanded, UseInDetails, Description, Unit, Type, GroupId, UseInBrief) 
	VALUES (@Name, @SortOrder, @UseInFilter, @Expanded, @UseInDetails, @Description, @Unit, @Type, @GroupId, @UseInBrief)
	SELECT SCOPE_IDENTITY()
END
GO--

ALTER PROCEDURE [Catalog].[sp_UpdateProperty]
	@PropertyID int,
    @Name nvarchar(100),
    @UseInFilter bit,
    @SortOrder int,
    @Expanded bit,
	@UseInDetails bit,
	@Description nvarchar(500),
	@Unit nvarchar(25),
	@Type tinyint,
	@GroupId int,
	@UseInBrief bit
AS
BEGIN
	SET NOCOUNT ON;

UPDATE [Catalog].[Property]
   SET [Name] = @Name
      ,[UseInFilter] = @UseInFilter
      ,[SortOrder] = @SortOrder
      ,[Expanded] = @Expanded
	  ,[UseInDetails] = @UseInDetails
	  ,[Description] = @Description
	  ,[Unit] = @Unit
	  ,[Type] = @Type
	  ,[GroupId] = @GroupId
	  ,[UseInBrief] = @UseInBrief
 WHERE [PropertyID] = @PropertyID
END
GO--

ALTER TABLE [Catalog].[PropertyValue] ADD [UseInBrief] bit null
GO--

ALTER PROCEDURE [Catalog].[sp_GetPropertyValuesByProductID] @ProductID INT
AS
BEGIN
	SET NOCOUNT ON;

	SELECT
		 [PropertyValue].[PropertyValueID]
		,[PropertyValue].[PropertyID]
		,[PropertyValue].[Value]
		,[ProductPropertyValue].[SortOrder]
		,[Property].UseinFilter
		,[Property].UseIndetails
		,[Property].UseInBrief
		,[Property].[Name] as PropertyName
		,[Property].[SortOrder] as PropertySortOrder
		,[Property].[Expanded] as Expanded
		,[Property].[Type] as [Type]
		,[Property].GroupId as GroupId
		,GroupName
		,GroupSortorder
	FROM [Catalog].[PropertyValue]
	INNER JOIN [Catalog].[ProductPropertyValue] ON [ProductPropertyValue].[PropertyValueID] = [PropertyValue].[PropertyValueID]
	inner join [Catalog].[Property] on [Property].[PropertyID] = [PropertyValue].[PropertyID]
	left join Catalog.PropertyGroup on propertyGroup.PropertyGroupID = [Property].GroupID
	WHERE [ProductID] = @ProductID
	ORDER BY case when PropertyGroup.GroupSortOrder is null then 1 else 0 end, PropertyGroup.GroupSortOrder, [Property].[SortOrder], [PropertyValue].[SortOrder]
END
GO--

ALTER PROCEDURE [Catalog].[sp_GetPropertyValuesByPropertyID] @PropertyID INT
AS
BEGIN
	SET NOCOUNT ON;

	SELECT [PropertyValueID]
		,[Property].[PropertyID]
		,[Value]
		,[PropertyValue].[SortOrder]
		,[Property].UseinFilter
		,[Property].UseIndetails
		,[Property].UseInBrief
		,Property.NAME AS PropertyName
		,Property.SortOrder AS PropertySortOrder
		,Property.Expanded
		,Property.Type
		,GroupId
		,GroupName
		,GroupSortorder
	FROM [Catalog].[PropertyValue]
	INNER JOIN [Catalog].[Property] ON [Property].[PropertyID] = [PropertyValue].[PropertyID]
	LEFT JOIN [Catalog].PropertyGroup ON PropertyGroup.PropertyGroupID = [Property].GroupID
	WHERE [Property].[PropertyID] = @PropertyID
	order by [PropertyValue].[SortOrder]
END
GO--

ALTER PROCEDURE [Catalog].[sp_GetPropertyValueByID] @PropertyValueId INT
AS
BEGIN
	SELECT [PropertyValueID]
		,[Property].[PropertyID]
		,[Value]
		,[PropertyValue].[SortOrder]
		,[Property].UseinFilter
		,[Property].UseIndetails
		,[Property].UseInBrief
		,Property.NAME AS PropertyName
		,Property.SortOrder AS PropertySortOrder
		,Property.Expanded
		,Property.Type
		,GroupId
		,GroupName
		,GroupSortOrder
	FROM [Catalog].[PropertyValue]
	INNER JOIN [Catalog].[Property] ON [Property].[PropertyID] = [PropertyValue].[PropertyID]
	LEFT JOIN [Catalog].PropertyGroup ON PropertyGroup.PropertyGroupID = [Property].GroupID
	WHERE [PropertyValue].[PropertyValueID] = @PropertyValueId
END
GO--

ALTER PROCEDURE [Catalog].[sp_AddPropertyValue]
	@Value nvarchar(255),
	@PropertyId int,
	@SortOrder int = 0,
	@RangeValue float = 0
AS
BEGIN
	SET NOCOUNT ON;
	 
	INSERT INTO [Catalog].[PropertyValue]
           ([PropertyID],[Value],[SortOrder],[UseInFilter], [UseInDetails], UseInBrief, [RangeValue])
			select @PropertyID, @Value, @SortOrder, [UseInFilter], [UseInDetails], UseInBrief, @RangeValue from [Catalog].[Property] where  [Property].[PropertyID]=@PropertyId
     SELECT SCOPE_IDENTITY()
END
GO--

update [Catalog].[Property] set useinbrief = 0
update [Catalog].[Propertyvalue] set useinbrief = 0
GO--

IF EXISTS (SELECT * FROM sys.objects 
WHERE object_id = OBJECT_ID(N'[Order].[ShippingCache]') AND type in (N'U'))
	delete from [Order].[ShippingCache]
GO--

ALTER PROCEDURE [Catalog].[sp_GetRelatedProducts]
    (
        @ProductID int,
        @RelatedType int,
        @Type nvarchar(50)
    )
    AS
    SELECT Product.ProductID, Product.Name, PhotoName as Photo, [Photo].[Description] AS PhotoDesc, ArtNo, Product.BriefDescription,
     Product.Discount, UrlPath, AllowPreOrder
    FROM  Catalog.Product INNER JOIN Catalog.RelatedProducts ON Catalog.Product.ProductID = Catalog.RelatedProducts.LinkedProductID 
			LEFT JOIN [Catalog].[Photo] ON [Product].[ProductID] = [Photo].[ObjId] and [Type]=@Type AND [Photo].[Main] = 1
    WHERE Catalog.RelatedProducts.ProductID = @ProductID AND Catalog.Product.Enabled = 1 AND RelatedProducts.RelatedType = @RelatedType
GO--

ALTER PROCEDURE [Catalog].[sp_GetChildCategoriesByParentID]
	@ParentCategoryID int,
	@HasProducts bit,
	@bigType  nvarchar(50),
	@smallType  nvarchar(50)
AS
BEGIN

if @hasProducts = 0
	SELECT 
		*,
		(SELECT Count(CategoryID) FROM [Catalog].[Category] AS c WHERE c.ParentCategory = p.CategoryID) AS [ChildCategories_Count],
		(SELECT PhotoName FROM [Catalog].[Photo] AS c WHERE c.[ObjId] = p.CategoryID and [Type]=@bigType) AS Picture,
		(SELECT PhotoName FROM [Catalog].[Photo] AS c WHERE c.[ObjId] = p.CategoryID and [Type]=@smallType) AS MiniPicture
	FROM [Catalog].[Category] AS p WHERE [ParentCategory] = @ParentCategoryID AND CategoryID <> 0 
	ORDER BY SortOrder, Name
else
	SELECT 
		*,
		(SELECT Count(CategoryID) FROM [Catalog].[Category] AS c WHERE c.ParentCategory = p.CategoryID) AS [ChildCategories_Count] ,
		(SELECT PhotoName FROM [Catalog].[Photo] AS c WHERE c.[ObjId] = p.CategoryID and [Type]=@bigType) AS Picture,
		(SELECT PhotoName FROM [Catalog].[Photo] AS c WHERE c.[ObjId] = p.CategoryID and [Type]=@smallType) AS MiniPicture
	FROM [Catalog].[Category] AS p WHERE [ParentCategory] = @ParentCategoryID AND CategoryID <> 0 and Products_Count > 0
	ORDER BY SortOrder, Name
END

GO--

-- 1c

if not exists(select * from sys.columns 
	where Name = N'UseIn1C' and Object_ID = Object_ID(N'[Order].[Order]'))
ALTER TABLE [Order].[Order] ADD
	UseIn1C bit NULL
GO--

if not exists(select * from sys.columns 
	where Name = N'ModifiedDate' and Object_ID = Object_ID(N'[Order].[Order]'))
ALTER TABLE [Order].[Order] ADD
	ModifiedDate datetime NULL
GO--

Update [Order].[Order] Set [ModifiedDate] = [OrderDate]
GO--

if (select count(*) from [Settings].[Settings] where [Name]='1c_Enabled') = 0
	Insert Into [Settings].[Settings] ([Name],[Value]) Values('1c_Enabled', 'False');
GO--

if (select count(*) from [Settings].[Settings] where [Name]='1c_OnlyUseIn1COrders') = 0
	Insert Into [Settings].[Settings] ([Name],[Value]) Values('1c_OnlyUseIn1COrders', 'True');
GO--




GO--

IF not  EXISTS (SELECT * FROM sys.objects 
WHERE object_id = OBJECT_ID(N'[Order].[DeletedOrders]') AND type in (N'U'))
	CREATE TABLE [Order].[DeletedOrders](
	[OrderId] [int] NOT NULL,
	[DateTime] [datetime] NOT NULL
) ON [PRIMARY]

GO--

-- 1C FIX (4.1.12)

IF not  EXISTS (SELECT * FROM sys.objects 
WHERE object_id = OBJECT_ID(N'[Order].[OrderStatus1C]') AND type in (N'U'))
	CREATE TABLE [Order].[OrderStatus1C](
	[OrderId] [int] NOT NULL,
	[Status1C] [nvarchar](max) NOT NULL,
	[OrderId1C] [nvarchar](max) NOT NULL,
	[OrderDate] [nvarchar](max) NOT NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO--

IF not  EXISTS (SELECT * FROM sys.objects 
WHERE object_id = OBJECT_ID(N'[Catalog].[DeletedProducts]') AND type in (N'U'))
	CREATE TABLE [Catalog].[DeletedProducts](
	[ProductId] [int] NOT NULL,
	[ArtNo] [nvarchar](50) NOT NULL,
	[DateTime] [datetime] NOT NULL
) ON [PRIMARY]

GO--

ALTER TABLE [Catalog].[Product] ADD [ManufacturerWarranty] bit null
GO--

ALTER PROCEDURE [Catalog].[sp_AddProduct]		
			@ArtNo nvarchar(50) = '',
			@Name nvarchar(255),			
			@Ratio float,
			@Discount float,
			@Weight float,
			@Size nvarchar(50),
			@BriefDescription nvarchar(max),
			@Description nvarchar(max),
			@Enabled tinyint,
			@Recomended bit,
			@New bit,
			@BestSeller bit,
			@OnSale bit,
			@BrandID int,
			@AllowPreOrder bit,
			@UrlPath nvarchar(150),
			@Unit nvarchar(50),
			@ShippingPrice float,
			@MinAmount float,
			@MaxAmount float,
			@Multiplicity float,
			@HasMultiOffer bit,
			@SalesNote nvarchar(50),
			@GoogleProductCategory nvarchar(500),
			@Gtin nvarchar(50),
			@Adult bit,
			@ManufacturerWarranty bit
AS
BEGIN
Declare @Id int
INSERT INTO [Catalog].[Product]
           ([ArtNo]
           ,[Name]                   
           ,[Ratio]
           ,[Discount]
           ,[Weight]
           ,[Size]
           ,[BriefDescription]
           ,[Description]
           ,[Enabled]
           ,[DateAdded]
           ,[DateModified]
           ,[Recomended]
           ,[New]
           ,[BestSeller]
           ,[OnSale]
           ,[BrandID]
           ,[AllowPreOrder]
		   ,[UrlPath]
		   ,[Unit]
		   ,[ShippingPrice]
		   ,[MinAmount]
		   ,[MaxAmount]
		   ,[Multiplicity]
		   ,[HasMultiOffer]
		   ,[SalesNote]
		   ,GoogleProductCategory
		   ,Gtin
		   ,Adult
		   ,[ManufacturerWarranty]
          )
     VALUES
           (@ArtNo,
			@Name,					
			@Ratio,
			@Discount,
			@Weight,
			@Size,
			@BriefDescription,
			@Description,
			@Enabled,
			GETDATE(),
			GETDATE(),
			@Recomended,
			@New,
			@BestSeller,
			@OnSale,
			@BrandID,
			@AllowPreOrder,
			@UrlPath,
			@Unit,
			@ShippingPrice,
			@MinAmount,
			@MaxAmount,
			@Multiplicity,
			@HasMultiOffer,
			@SalesNote,
			@GoogleProductCategory,
			@Gtin,
			@Adult,
			@ManufacturerWarranty
			);

	SET @ID = SCOPE_IDENTITY();
	if @ArtNo=''
	begin
		set @ArtNo = Convert(nvarchar(50), @ID)	

		WHILE (SELECT COUNT(*) FROM [Catalog].[Product] WHERE [ArtNo] = @ArtNo) > 0
		begin
				SET @ArtNo = @ArtNo + '_A'
		end

		UPDATE [Catalog].[Product] SET [ArtNo] = @ArtNo WHERE [ProductID] = @ID	
	end
	Select @ID
END
GO--

ALTER PROCEDURE [Catalog].[sp_UpdateProductById]
		@ProductID int,	   
		@ArtNo nvarchar(50),
		@Name nvarchar(255),
		@Ratio float,		
		@Discount float,
		@Weight float,
		@Size nvarchar(50),
		@BriefDescription nvarchar(max),
		@Description nvarchar(max),
		@Enabled bit,
		@Recomended bit,
		@New bit,
		@BestSeller bit,
		@OnSale bit,
		@BrandID int,
		@AllowPreOrder bit,
		@UrlPath nvarchar(150),
		@Unit nvarchar(50),
		@ShippingPrice money,
		@MinAmount float,
		@MaxAmount float,
		@Multiplicity float,
		@HasMultiOffer bit,
		@SalesNote nvarchar(50),
		@GoogleProductCategory nvarchar(500),
		@Gtin nvarchar(50),
		@Adult bit,
		@ManufacturerWarranty bit
AS
BEGIN
UPDATE [Catalog].[Product]
   SET [ArtNo] = @ArtNo
		,[Name] = @Name
		,[Ratio] = @Ratio
		,[Discount] = @Discount
		,[Weight] = @Weight
		,[Size] = @Size
		,[BriefDescription] = @BriefDescription
		,[Description] = @Description
		,[Enabled] = @Enabled
		,[Recomended] = @Recomended
		,[New] = @New
		,[BestSeller] = @BestSeller
		,[OnSale] = @OnSale
		,[DateModified] = GETDATE()
		,[BrandID] = @BrandID
		,[AllowPreOrder] = @AllowPreOrder
		,[UrlPath] = @UrlPath
		,[Unit] = @Unit
		,[ShippingPrice] = @ShippingPrice
		,[MinAmount] = @MinAmount
		,[MaxAmount] = @MaxAmount
		,[Multiplicity] = @Multiplicity
		,[HasMultiOffer] = @HasMultiOffer
		,[SalesNote] = @SalesNote
		,[GoogleProductCategory]=@GoogleProductCategory
		,[Gtin]=@Gtin
		,[Adult] = @Adult
		,[ManufacturerWarranty] = @ManufacturerWarranty
 WHERE ProductID = @ProductID	 
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
			,ManufacturerWarranty
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


GO--
Alter Table [Catalog].Product
Add AddManually bit NULL
GO--
Update [Catalog].Product Set AddManually = 1
GO--
Alter Table [Catalog].Product
Alter Column AddManually bit NOT NULL
GO--

ALTER PROCEDURE [Catalog].[sp_AddProduct]		
			@ArtNo nvarchar(50) = '',
			@Name nvarchar(255),			
			@Ratio float,
			@Discount float,
			@Weight float,
			@Size nvarchar(50),
			@BriefDescription nvarchar(max),
			@Description nvarchar(max),
			@Enabled tinyint,
			@Recomended bit,
			@New bit,
			@BestSeller bit,
			@OnSale bit,
			@BrandID int,
			@AllowPreOrder bit,
			@UrlPath nvarchar(150),
			@Unit nvarchar(50),
			@ShippingPrice float,
			@MinAmount float,
			@MaxAmount float,
			@Multiplicity float,
			@HasMultiOffer bit,
			@SalesNote nvarchar(50),
			@GoogleProductCategory nvarchar(500),
			@Gtin nvarchar(50),
			@Adult bit,
			@ManufacturerWarranty bit,
			@AddManually bit
AS
BEGIN
Declare @Id int
INSERT INTO [Catalog].[Product]
           ([ArtNo]
           ,[Name]                   
           ,[Ratio]
           ,[Discount]
           ,[Weight]
           ,[Size]
           ,[BriefDescription]
           ,[Description]
           ,[Enabled]
           ,[DateAdded]
           ,[DateModified]
           ,[Recomended]
           ,[New]
           ,[BestSeller]
           ,[OnSale]
           ,[BrandID]
           ,[AllowPreOrder]
		   ,[UrlPath]
		   ,[Unit]
		   ,[ShippingPrice]
		   ,[MinAmount]
		   ,[MaxAmount]
		   ,[Multiplicity]
		   ,[HasMultiOffer]
		   ,[SalesNote]
		   ,GoogleProductCategory
		   ,Gtin
		   ,Adult
		   ,[ManufacturerWarranty]
		   ,[AddManually]
          )
     VALUES
           (@ArtNo,
			@Name,					
			@Ratio,
			@Discount,
			@Weight,
			@Size,
			@BriefDescription,
			@Description,
			@Enabled,
			GETDATE(),
			GETDATE(),
			@Recomended,
			@New,
			@BestSeller,
			@OnSale,
			@BrandID,
			@AllowPreOrder,
			@UrlPath,
			@Unit,
			@ShippingPrice,
			@MinAmount,
			@MaxAmount,
			@Multiplicity,
			@HasMultiOffer,
			@SalesNote,
			@GoogleProductCategory,
			@Gtin,
			@Adult,
			@ManufacturerWarranty,
			@AddManually
			);

	SET @ID = SCOPE_IDENTITY();
	if @ArtNo=''
	begin
		set @ArtNo = Convert(nvarchar(50), @ID)	

		WHILE (SELECT COUNT(*) FROM [Catalog].[Product] WHERE [ArtNo] = @ArtNo) > 0
		begin
				SET @ArtNo = @ArtNo + '_A'
		end

		UPDATE [Catalog].[Product] SET [ArtNo] = @ArtNo WHERE [ProductID] = @ID	
	end
	Select @ID
END

GO--

ALTER PROCEDURE [Catalog].[sp_UpdateProductById]
		@ProductID int,	   
		@ArtNo nvarchar(50),
		@Name nvarchar(255),
		@Ratio float,		
		@Discount float,
		@Weight float,
		@Size nvarchar(50),
		@BriefDescription nvarchar(max),
		@Description nvarchar(max),
		@Enabled bit,
		@Recomended bit,
		@New bit,
		@BestSeller bit,
		@OnSale bit,
		@BrandID int,
		@AllowPreOrder bit,
		@UrlPath nvarchar(150),
		@Unit nvarchar(50),
		@ShippingPrice money,
		@MinAmount float,
		@MaxAmount float,
		@Multiplicity float,
		@HasMultiOffer bit,
		@SalesNote nvarchar(50),
		@GoogleProductCategory nvarchar(500),
		@Gtin nvarchar(50),
		@Adult bit,
		@ManufacturerWarranty bit,
		@AddManually bit
AS
BEGIN
UPDATE [Catalog].[Product]
   SET [ArtNo] = @ArtNo
		,[Name] = @Name
		,[Ratio] = @Ratio
		,[Discount] = @Discount
		,[Weight] = @Weight
		,[Size] = @Size
		,[BriefDescription] = @BriefDescription
		,[Description] = @Description
		,[Enabled] = @Enabled
		,[Recomended] = @Recomended
		,[New] = @New
		,[BestSeller] = @BestSeller
		,[OnSale] = @OnSale
		,[DateModified] = GETDATE()
		,[BrandID] = @BrandID
		,[AllowPreOrder] = @AllowPreOrder
		,[UrlPath] = @UrlPath
		,[Unit] = @Unit
		,[ShippingPrice] = @ShippingPrice
		,[MinAmount] = @MinAmount
		,[MaxAmount] = @MaxAmount
		,[Multiplicity] = @Multiplicity
		,[HasMultiOffer] = @HasMultiOffer
		,[SalesNote] = @SalesNote
		,[GoogleProductCategory]=@GoogleProductCategory
		,[Gtin]=@Gtin
		,[Adult] = @Adult
		,[ManufacturerWarranty] = @ManufacturerWarranty
 WHERE ProductID = @ProductID	 
END

GO--

ALTER PROCEDURE [Catalog].[sp_UpdateOrInsertProductProperty]		
		@ProductID int,
		@Name nvarchar(255),
		@Value nvarchar(255),
		@SortOrder int
AS
BEGIN
	Declare @propertyId int;
	Set @propertyId = 0;
	Select @propertyId = PropertyID From Catalog.Property Where Name = @Name;
	if( @propertyId = 0 )
		begin
			Insert into Catalog.Property ( Name, UseInFilter, SortOrder, Expanded, [Type] ) Values ( @Name, 0, 0, 0, 0 )
			Select @propertyId = PropertyID From Catalog.Property Where Name = @Name
		end
	
	declare @propertyValueId int;
	Set @propertyValueId = 0;
	Select @propertyValueId = PropertyValueID From Catalog.PropertyValue Where Value = @Value and PropertyID = @propertyId;
	if(@propertyValueId = 0)			
		begin
			Insert into Catalog.PropertyValue ( Value, PropertyID, RangeValue ) Values ( @Value, @propertyId, @Value )
			Select @propertyValueId = PropertyValueID From Catalog.PropertyValue Where Value = @Value and PropertyID = @propertyId
		end
						
	if( (Select COUNT(ProductID) From Catalog.ProductPropertyValue Where ProductID = @ProductID and PropertyValueID = @propertyValueId) = 0 )
		begin	
			Insert into Catalog.ProductPropertyValue ( ProductID, PropertyValueID, SortOrder ) Values ( @ProductID, @propertyValueId, @SortOrder )
		end
END

GO--

UPDATE [Settings].[InternalSettings] SET [settingValue] = '4.1.12' WHERE [settingKey] = 'db_version'
GO--

