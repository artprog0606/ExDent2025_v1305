
-- SQL_410_Part1

ALTER TABLE dbo.Modules 
ADD Active bit NULL
GO--

Update dbo.Modules Set Active = 1 
GO--

ALTER TABLE dbo.Modules 
ALTER COLUMN Active bit NOT NULL
GO--

DROP INDEX IX_Redirect ON Settings.Redirect
GO--

alter table Settings.Redirect
alter column RedirectFrom nvarchar(450)
GO--

alter table Settings.Redirect
alter column RedirectTo nvarchar(450)
GO--

CREATE NONCLUSTERED INDEX IX_Redirect ON Settings.Redirect
	(
	RedirectFrom
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO--


GO--

ALTER TABLE Catalog.Property ADD
	UseInDetails bit NULL
GO--	
ALTER TABLE Catalog.PropertyValue ADD
	UseInDetails bit NULL
GO--		

Update Catalog.Property set UseInDetails=1
GO--
Update Catalog.PropertyValue  set UseInDetails=1
GO--


ALTER PROCEDURE [Catalog].[sp_AddProperty]
	-- Add the parameters for the stored procedure here
	@Name nvarchar(100),
	@UseInFilter bit = 0, 
	@SortOrder int = 0,
	@Expanded bit = 0,
	@UseInDetails bit = 1
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	INSERT INTO [Catalog].[Property] (Name, SortOrder, UseInFilter, Expanded, UseInDetails) VALUES (@Name, @SortOrder, @UseInFilter, @Expanded, @UseInDetails)
	SELECT SCOPE_IDENTITY()
END
GO--

ALTER PROCEDURE [Catalog].[sp_AddPropertyValue]
	@Value nvarchar(255),
	@PropertyId int,
	@SortOrder int = 0
AS
BEGIN
	SET NOCOUNT ON;
	 
	INSERT INTO [Catalog].[PropertyValue]
           ([PropertyID],[Value],[SortOrder],[UseInFilter], [UseInDetails])
			select @PropertyID, @Value, @SortOrder, [UseInFilter], [UseInDetails] from [Catalog].[Property] where  [Property].[PropertyID]=@PropertyId
     SELECT SCOPE_IDENTITY()
END

GO--

ALTER PROCEDURE [Catalog].[sp_UpdateProperty]
	-- Add the parameters for the stored procedure here
	@PropertyID int,
    @Name nvarchar(100),
    @UseInFilter bit,
    @SortOrder int,
    @Expanded bit,
	@UseInDetails bit
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	UPDATE [Catalog].[Property]
   SET [Name] = @Name
      ,[UseInFilter] = @UseInFilter
      ,[SortOrder] = @SortOrder
      ,[Expanded] = @Expanded
	  ,[UseInDetails] =@UseInDetails
 WHERE [PropertyID] = @PropertyID
END

GO--


ALTER PROCEDURE [Catalog].[sp_GetPropertyValuesByProductID]
	@ProductID int
AS
BEGIN

	SET NOCOUNT ON;

	SELECT [PropertyValue].[PropertyValueID], [PropertyValue].[PropertyID], [PropertyValue].[Value], [ProductPropertyValue].[SortOrder], UseinFilter, UseIndetails,
	(select [Name] from [Catalog].[Property] where [PropertyID] = [PropertyValue].[PropertyID]) as PropertyName,
	(select [SortOrder] from [Catalog].[Property] where [PropertyID] = [PropertyValue].[PropertyID]) as PropertySortOrder,
	(select [Expanded] from [Catalog].[Property] where [PropertyID] = [PropertyValue].[PropertyID]) as Expanded
	FROM [Catalog].[PropertyValue]
	inner join [Catalog].[ProductPropertyValue] On [ProductPropertyValue].[PropertyValueID] = [PropertyValue].[PropertyValueID]

	WHERE [ProductID] = @ProductID
	ORDER BY [ProductPropertyValue].[SortOrder]
	
END

GO--

ALTER PROCEDURE [Catalog].[sp_GetPropertyValuesByPropertyID]
	@PropertyID int
AS
BEGIN

	SET NOCOUNT ON;

	SELECT [PropertyValueID],[PropertyID],[Value],[PropertyValue].[SortOrder],UseinFilter, UseIndetails,
	(select [Name] from [Catalog].[Property] where [PropertyID] = @PropertyID) as PropertyName,
	(select [SortOrder] from [Catalog].[Property] where [PropertyID] = @PropertyID) as PropertySortOrder,
	(select [Expanded] from [Catalog].[Property] where [PropertyID] = @PropertyID) as Expanded
	FROM [Catalog].[PropertyValue]
	WHERE [PropertyID] = @PropertyID
END

-- AZURE FIX -----------------

GO--
alter PROCEDURE [Order].[sp_GetProfitPeriods]
AS
BEGIN

	SET NOCOUNT ON;
	-- Temp table
	declare @Cost money
	declare @temp table (
		[NUM] int identity,
		[Count] int,
		[Sum] money,
		[SumWDiscount] money,
		[Cost] money default 0,
		[Tax] money,
		[Shipping] money,
		[ExtraCharge] money default 0
	)
   -- ExtraCharges
   declare @extraCharges table(
		[Value] money default 0,
		[OrderID] int,
		[OrderDate] datetime
   )
	insert into @extraCharges
			select [ParamValue] , [OrderID], [OrderDate]
			from [Order].[Order]
				inner join [Order].[ShippingMethod] 
					on [Order].[ShippingMethodID] = [ShippingMethod].[ShippingMethodID]
				inner join [Order].[ShippingParam] 
					on [ShippingParam].[ShippingMethodID] = [ShippingMethod].[ShippingMethodID] 
			where [ShippingParam].[ParamName] = 'Extracharge' and [ShippingMethod].[ShippingMethodID] = [Order].[ShippingMethodID]
				and [PaymentDate] is not null		
	
	-- Today profit
	insert into @temp 
		select 
		Count(*) as 'Count', 
		sum([Sum]) as 'Sum',
		Sum(case when OrderDiscount = 100 then   ([Sum] - [ShippingCost] - [TaxCost]) else  ([Sum] - [ShippingCost] - [TaxCost])  * 100 / (100 - OrderDiscount)end ) as 'SumWDiscount',
		SUM([SupplyTotal]) ,
		sum([TaxCost]) as 'Tax',  
		sum([ShippingCost]) as 'Shipping',
		(select sum([Value]) from @extraCharges where DATEADD(dd, 0, DATEDIFF(dd, 0, [OrderDate])) = DATEADD(dd, 0, DATEDIFF(dd, 0, Getdate())))
	from [Order].[Order]
	where [PaymentDate] is not null and DATEADD(dd, 0, DATEDIFF(dd, 0, [OrderDate])) = DATEADD(dd, 0, DATEDIFF(dd, 0, Getdate()))
	
	
	
   -- Yesterday profit
    
    insert into @temp 
		select 
		Count(*) as 'Count', 
		sum([Sum]) as 'Sum', 
		Sum(case when OrderDiscount = 100 then   ([Sum] - [ShippingCost] - [TaxCost]) else  ([Sum] - [ShippingCost] - [TaxCost])  * 100 / (100 - OrderDiscount)end ) as 'SumWDiscount',
		SUM([SupplyTotal]) ,
		sum([TaxCost]) as 'Tax',  
		sum([ShippingCost]) as 'Shipping',
		( select sum([Value]) from @extraCharges where DATEADD(dd, 0, DATEDIFF(dd, 0, [OrderDate])) = DATEADD(dd, -1, DATEDIFF(dd, 0, Getdate())) )
	from [Order].[Order] 
	where DATEADD(dd, 0, DATEDIFF(dd, 0, [OrderDate])) = DATEADD(dd, -1, DATEDIFF(dd, 0, Getdate()))and [PaymentDate] is not null
	
	
-- Month profit
    
    insert into @temp
		select 
		Count(*) as 'Count', 
		sum([Sum]) as 'Sum', 
		Sum(case when OrderDiscount = 100 then   ([Sum] - [ShippingCost] - [TaxCost]) else  ([Sum] - [ShippingCost] - [TaxCost])  * 100 / (100 - OrderDiscount)end ) as 'SumWDiscount',
		SUM([SupplyTotal]) ,
		sum([TaxCost]) as 'Tax',  
		sum([ShippingCost]) as 'Shipping',
		(select sum([Value]) from @extraCharges where Month([OrderDate]) = Month(getdate()) and Year([OrderDate]) = Year(getdate()))
	from [Order].[Order] 
	where Month([OrderDate]) = Month(getdate()) and Year([OrderDate]) = Year(getdate())and [PaymentDate] is not null
	
	--Total profit
	
	insert into @temp 
		select 
		Count(*) as 'Count', 
		sum([Sum]) as 'Sum', 
		Sum(case when OrderDiscount = 100 then   ([Sum] - [ShippingCost] - [TaxCost]) else  ([Sum] - [ShippingCost] - [TaxCost])  * 100 / (100 - OrderDiscount)end ) as 'SumWDiscount',
		SUM([SupplyTotal]) ,
		sum([TaxCost]) as 'Tax',  
		sum([ShippingCost]) as 'Shipping',
		(select sum([Value]) from @extraCharges)
	from [Order].[Order] where [PaymentDate] is not null
	
	update @temp set [ExtraCharge] = 0 where [ExtraCharge] is null
	
	select 
		[Count], 
		[Sum],
		[SumWDiscount], 
		[Cost], 
		[Tax], 
		[Shipping], 
		[Sum] - [Cost] - [Tax] - [Shipping] + [ExtraCharge] as 'Profit',
		Profitability=
		case 
		when [Sum] - [Tax] - [Shipping]=0 then 0 else ( 1 - ( [Cost]/( [Sum] - [Tax] - [Shipping] ) ) )*100 end 
		--([Sum] - [Cost] - [Tax] - [Shipping] + [ExtraCharge])/([Sum] - [Tax] - [Shipping])*100 as 'Profitability'
	from @temp

END
GO--

CREATE UNIQUE CLUSTERED INDEX Idx_GiftCertificatePayments ON [Settings].[GiftCertificatePayments]([PaymentID]);

GO--

CREATE UNIQUE CLUSTERED INDEX Idx_OrderPaymentInfo ON [Order].[OrderPaymentInfo]([OrderID]);

GO--
alter table [Order].[ShippingPayments]
add id int not null IDENTITY(1,1)

GO--

CREATE UNIQUE CLUSTERED INDEX Idx_ShippingPayments ON [Order].[ShippingPayments](id);
GO--

-- AZURE FIX -----------------

GO--
ALTER TABLE Catalog.Product ADD
	GoogleProductCategory nvarchar(500) NULL,
	Gtin nvarchar(50) NULL
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
			@Gtin nvarchar(50)
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
			@Gtin
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
		@Gtin nvarchar(50)
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
 WHERE ProductID = @ProductID	 
END

GO--

CREATE NONCLUSTERED INDEX IX_ProductVideo ON Catalog.ProductVideo
	(
	ProductID
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO--

Update Settings.Mailformat set formattext= Replace(formattext, 'исполенения', 'исполнения')

GO--

ALTER PROCEDURE [Catalog].[sp_GetAVGRatioByProductID]
     @ProductID int
AS
BEGIN

 SELECT  Round(AVG(convert(float, ProductRatio)), 0) AS AVGProduct
 FROM [Catalog].[Ratio]
 WHERE [ProductID] = @ProductID

END

GO--


ALTER TABLE Settings.MailFormat ADD
	FormatSubject nvarchar(MAX) NULL

GO--



---- add property features

CREATE TABLE [Catalog].[PropertyGroup](
	[PropertyGroupId] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
 CONSTRAINT [PK_PropertyGroup] PRIMARY KEY CLUSTERED 
	(
		[PropertyGroupId] ASC
	)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO--


CREATE TABLE [Catalog].[PropertyGroup_Category](
	[PropertyGroupId] [int] NOT NULL,
	[CategoryId] [int] NOT NULL,
	[SortOrder] [int] NOT NULL,
 CONSTRAINT [PK_PropertyGroup_Category] PRIMARY KEY CLUSTERED 
	(
		[PropertyGroupId] ASC,
		[CategoryId] ASC
	)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO--

ALTER TABLE [Catalog].[PropertyGroup_Category]  WITH CHECK ADD  CONSTRAINT [FK_PropertyGroup_Category_Category] FOREIGN KEY([CategoryId])
REFERENCES [Catalog].[Category] ([CategoryID])
ON DELETE CASCADE
GO--

ALTER TABLE [Catalog].[PropertyGroup_Category] CHECK CONSTRAINT [FK_PropertyGroup_Category_Category]
GO--

ALTER TABLE [Catalog].[PropertyGroup_Category]  WITH CHECK ADD  CONSTRAINT [FK_PropertyGroup_Category_PropertyGroup] FOREIGN KEY([PropertyGroupId])
REFERENCES [Catalog].[PropertyGroup] ([PropertyGroupId])
ON DELETE CASCADE
GO--

ALTER TABLE [Catalog].[PropertyGroup_Category] CHECK CONSTRAINT [FK_PropertyGroup_Category_PropertyGroup]
GO--



ALTER TABLE Catalog.Property ADD
	Description nvarchar(500) NULL,
	Unit nvarchar(25) NULL,
	Type tinyint NULL,
	GroupId int NULL

GO--

Update [Catalog].[Property]  Set [Type] = 1

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
	@GroupId int
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
 WHERE [PropertyID] = @PropertyID
END

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
	@GroupId int	
AS
BEGIN
	SET NOCOUNT ON;

	INSERT INTO [Catalog].[Property] (Name, SortOrder, UseInFilter, Expanded, UseInDetails, Description, Unit, Type, GroupId) 
	VALUES (@Name, @SortOrder, @UseInFilter, @Expanded, @UseInDetails, @Description, @Unit, @Type, @GroupId)
	SELECT SCOPE_IDENTITY()
END

GO--


ALTER PROCEDURE [Catalog].[sp_GetPropertyValuesByProductID]
	@ProductID int
AS
BEGIN
	SET NOCOUNT ON;

	SELECT [PropertyValue].[PropertyValueID], [PropertyValue].[PropertyID], [PropertyValue].[Value], [ProductPropertyValue].[SortOrder], UseinFilter, UseIndetails,
	(select [Name] from [Catalog].[Property] where [PropertyID] = [PropertyValue].[PropertyID]) as PropertyName,
	(select [SortOrder] from [Catalog].[Property] where [PropertyID] = [PropertyValue].[PropertyID]) as PropertySortOrder,
	(select [Expanded] from [Catalog].[Property] where [PropertyID] = [PropertyValue].[PropertyID]) as Expanded,
	(select [Type] from [Catalog].[Property] where [PropertyID] = [PropertyValue].[PropertyID]) as Type
	FROM [Catalog].[PropertyValue]
	inner join [Catalog].[ProductPropertyValue] On [ProductPropertyValue].[PropertyValueID] = [PropertyValue].[PropertyValueID]

	WHERE [ProductID] = @ProductID
	ORDER BY [ProductPropertyValue].[SortOrder]
	
END

GO--


ALTER PROCEDURE [Catalog].[sp_GetPropertyValuesByPropertyID]
	@PropertyID int
AS
BEGIN
	SET NOCOUNT ON;

	SELECT [PropertyValueID],[PropertyID],[Value],[PropertyValue].[SortOrder],UseinFilter, UseIndetails,
	(select [Name] from [Catalog].[Property] where [PropertyID] = @PropertyID) as PropertyName,
	(select [SortOrder] from [Catalog].[Property] where [PropertyID] = @PropertyID) as PropertySortOrder,
	(select [Expanded] from [Catalog].[Property] where [PropertyID] = @PropertyID) as Expanded,
	(select [Type] from [Catalog].[Property] where [PropertyID] = [PropertyValue].[PropertyID]) as Type
	FROM [Catalog].[PropertyValue]
	WHERE [PropertyID] = @PropertyID
END

GO--



ALTER PROCEDURE [Catalog].[sp_GetPropertyValueByID]
	@PropertyValueId int
AS
BEGIN
	SET NOCOUNT ON;

	SELECT *, 
	(select [Name] from [Catalog].[Property] where [PropertyID] = [PropertyValue].[PropertyID]) as PropertyName,
	(select [SortOrder] from [Catalog].[Property] where [PropertyID] = [PropertyValue].[PropertyID]) as PropertySortOrder,
	(select [Expanded] from [Catalog].[Property] where [PropertyID] = [PropertyValue].[PropertyID]) as Expanded,
	(select [Type] from [Catalog].[Property] where [PropertyID] = [PropertyValue].[PropertyID]) as Type
	FROM [Catalog].[PropertyValue] WHERE [PropertyValueID] = @PropertyValueId
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
		ORDER BY PropertySortOrder, [PropertyValue].[PropertyID], [PropertyValue].[SortOrder]
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
		ORDER BY PropertySortOrder, [PropertyValue].[PropertyID], [PropertyValue].[SortOrder]
	end 
end

GO--


ALTER TABLE Catalog.PropertyValue ADD
	RangeValue float(53) NOT NULL CONSTRAINT DF_PropertyValue_RangeValue DEFAULT 0	

GO--

Update [Catalog].[PropertyValue] 
	Set RangeValue = (case when isnumeric([Value])=(1) and [Value]<>'-' then CONVERT([float],REPLACE([Value], ',', '.'),0) else (0) end)

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
           ([PropertyID],[Value],[SortOrder],[UseInFilter], [UseInDetails], [RangeValue])
			select @PropertyID, @Value, @SortOrder, [UseInFilter], [UseInDetails], @RangeValue from [Catalog].[Property] where  [Property].[PropertyID]=@PropertyId
     SELECT SCOPE_IDENTITY()
END

GO--


ALTER PROCEDURE [Catalog].[sp_UpdatePropertyValue]	
	@PropertyValueID int,
    @Value nvarchar(255),
    @SortOrder int,
    @RangeValue float
AS
BEGIN
	SET NOCOUNT ON;
	UPDATE [Catalog].[PropertyValue]
	SET [Value] = @Value
       ,[SortOrder] = @SortOrder
       ,[RangeValue] = @RangeValue
 WHERE [PropertyValueID] = @PropertyValueID
END

GO--


ALTER PROCEDURE [Catalog].[sp_DeleteProductPropertyValue]
	@ProductID int,
	@PropertyValueID int
AS
BEGIN
	SET NOCOUNT ON;

	DELETE FROM [Catalog].[ProductPropertyValue] WHERE [ProductID] = @ProductID AND [PropertyValueID] = @PropertyValueID
END

GO--

---- /end property

GO--
IF NOT EXISTS (
  SELECT * 
  FROM   sys.columns 
  WHERE  object_id = OBJECT_ID(N'[Catalog].[Product]') 
         AND name = 'Adult'
)
begin 
ALTER TABLE Catalog.Product ADD
	Adult bit NOT NULL CONSTRAINT DF_Product_Adult DEFAULT 0
end

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
			@Adult bit
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
			@Adult
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
		@Adult	bit
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
 WHERE ProductID = @ProductID	 
END

GO--


ALTER TABLE Customers.Customer ADD
	Patronymic nvarchar(70) NULL
GO--

ALTER PROCEDURE [Customers].[sp_GetCustomerByEmailAndPassword]
	@Email nvarchar(100),
	@Password nvarchar(100)
AS
BEGIN
	SELECT *
	FROM [Customers].[Customer]
	WHERE ([Email] = @Email) and ([Password] = @Password)
END
GO--

ALTER PROCEDURE [Customers].[sp_AddCustomer]
		   @CustomerGroupID int,
           @Password nvarchar(100),
           @FirstName nvarchar(70),
           @LastName nvarchar(70),
           @Phone nvarchar(max),
		   @RegistrationDateTime datetime,
           @Subscribed4News bit,
           @Email nvarchar(100),
           @CustomerRole int,
           @Patronymic nvarchar(70)
AS
BEGIN
	INSERT INTO [Customers].[Customer]
           ([CustomerGroupID]
           ,[Password]
           ,[FirstName]
           ,[LastName]
           ,[Phone]
           ,[RegistrationDateTime]
           ,[Subscribed4News]
           ,[Email]
           ,[CustomerRole]
           ,[Patronymic])
     VALUES
           (@CustomerGroupID
           ,@Password
           ,@FirstName
           ,@LastName
           ,@Phone
           ,@RegistrationDateTime
           ,@Subscribed4News
           ,@Email
           ,@CustomerRole
           ,@Patronymic);
     SELECT CustomerID from [Customer] where Email =@Email
END
GO--

Insert Into [Settings].[Settings] ([Name],[Value]) Values('IsShowFirstName', 'True');
Insert Into [Settings].[Settings] ([Name],[Value]) Values('IsRequiredFirstName', 'True');
Insert Into [Settings].[Settings] ([Name],[Value]) Values('IsShowLastName', 'True');
Insert Into [Settings].[Settings] ([Name],[Value]) Values('IsRequiredLastName', 'True');
Insert Into [Settings].[Settings] ([Name],[Value]) Values('IsShowPatronymic', 'False');
Insert Into [Settings].[Settings] ([Name],[Value]) Values('IsRequiredPatronymic', 'False');
Insert Into [Settings].[Settings] ([Name],[Value]) Values('IsShowPhone', 'True');
Insert Into [Settings].[Settings] ([Name],[Value]) Values('IsRequiredPhone', 'True');
Insert Into [Settings].[Settings] ([Name],[Value]) Values('IsShowCountry', 'True');
Insert Into [Settings].[Settings] ([Name],[Value]) Values('IsRequiredCountry', 'True');
Insert Into [Settings].[Settings] ([Name],[Value]) Values('IsShowState', 'True');
Insert Into [Settings].[Settings] ([Name],[Value]) Values('IsRequiredState', 'True');
Insert Into [Settings].[Settings] ([Name],[Value]) Values('IsShowCity', 'True');
Insert Into [Settings].[Settings] ([Name],[Value]) Values('IsRequiredCity', 'True');
Insert Into [Settings].[Settings] ([Name],[Value]) Values('IsShowZip', 'True');
Insert Into [Settings].[Settings] ([Name],[Value]) Values('IsRequiredZip', 'True');
Insert Into [Settings].[Settings] ([Name],[Value]) Values('IsShowAddress', 'True');
Insert Into [Settings].[Settings] ([Name],[Value]) Values('IsRequiredAddress', 'True');
Insert Into [Settings].[Settings] ([Name],[Value]) Values('IsShowUserAgreementText', 'True');
Insert Into [Settings].[Settings] ([Name],[Value]) Values('IsShowUserComment', 'True');

Insert Into [Settings].[Settings] ([Name],[Value]) Values('IsShowCustomShippingField1', 'False');
Insert Into [Settings].[Settings] ([Name],[Value]) Values('IsReqCustomShippingField1', 'False');  
 
Insert Into [Settings].[Settings] ([Name],[Value]) Values('IsShowCustomShippingField2', 'False');
Insert Into [Settings].[Settings] ([Name],[Value]) Values('IsReqCustomShippingField2', 'False');   
 
Insert Into [Settings].[Settings] ([Name],[Value]) Values('IsShowCustomShippingField3', 'False');
Insert Into [Settings].[Settings] ([Name],[Value]) Values('IsReqCustomShippingField3', 'False');
 

Insert Into [Settings].[Settings] ([Name],[Value]) Values('IsShowBuyInOneClickName', 'True');
Insert Into [Settings].[Settings] ([Name],[Value]) Values('IsRequiredBuyInOneClickName', 'True'); 

Insert Into [Settings].[Settings] ([Name],[Value]) Values('IsShowBuyInOneClickEmail', 'False');
Insert Into [Settings].[Settings] ([Name],[Value]) Values('IsRequiredBuyInOneClickEmail', 'False'); 

Insert Into [Settings].[Settings] ([Name],[Value]) Values('IsShowBuyInOneClickPhone', 'True');
Insert Into [Settings].[Settings] ([Name],[Value]) Values('IsRequiredBuyInOneClickPhone', 'True');

Insert Into [Settings].[Settings] ([Name],[Value]) Values('IsShowBuyInOneClickComment', 'True');
Insert Into [Settings].[Settings] ([Name],[Value]) Values('IsRequiredBuyInOneClickComment', 'False');

Insert Into [Settings].[Settings] ([Name],[Value]) Values('DisplayPromoTextbox', 'True');
GO--

 ALTER PROCEDURE [Customers].[sp_GetRecentlyView]
	@CustomerId uniqueidentifier,
	@rowsCount int,
	@Type nvarchar(50)
AS
BEGIN

	--delete overflow product
delete from [Customers].RecentlyViewsData  where CustomerID=@CustomerId and [ProductID] not in 
(SELECT TOP(@rowsCount) ProductID FROM [Customers].[RecentlyViewsData] WHERE CustomerID=@CustomerId ORDER BY ViewDate desc)


--delete product not in category and enabled false
delete from [Customers].RecentlyViewsData where 
(select Enabled from Catalog.Product Where Product.ProductID=RecentlyViewsData.Productid) = 0
or 
(select CategoryEnabled from Catalog.Product Where Product.ProductID=RecentlyViewsData.Productid) = 0
and CustomerID =@CustomerId


--select
SELECT [RecentlyViewsData].[ProductID], [ViewDate], [Name],[UrlPath], PhotoName as [Photo],[Photo].[Description] as PhotoDesc, [Discount], Ratio, RatioID,
(select min(price) from catalog.Offer where Offer.ProductId = [RecentlyViewsData].[ProductID]) as Price, 
(select max(price) - min(price) from catalog.Offer where Offer.ProductId = [RecentlyViewsData].[ProductID]) as MultiPrice
 FROM [Customers].RecentlyViewsData
inner JOIN ([Catalog].Product inner JOIN [Catalog].[Offer] ON Product.ProductID = Offer.ProductID and offer.Main=1) ON Product.ProductID = RecentlyViewsData.ProductId 
LEFT JOIN Catalog.Photo ON Photo.[ObjId] = Product.ProductID and [Type]=@Type AND [Photo].[Main]=1 
LEFT JOIN Catalog.Ratio ON Ratio.[ProductID] = Product.ProductID and Ratio.CustomerID = @CustomerId 
WHERE RecentlyViewsData.CustomerID =@CustomerId
ORDER BY ViewDate desc

END
GO--


Update [Settings].[Settings] Set Name = 'SellerCountryId' where Name = 'SalerCountryID'
Update [Settings].[Settings] Set Name = 'SellerRegionId' where Name = 'SalerRegionID'

Update [Settings].[Settings] Set Name = LTRIM(RTRIM(Name))

GO--

alter table Settings.settings 
alter column value nvarchar(Max) not null

GO--

ALTER PROCEDURE [Settings].[sp_UpdateSettings]
   
		@Value nvarchar(Max),
		@Name nvarchar(50)

AS
BEGIN
	IF (SELECT COUNT(*) FROM [Settings].[Settings] WHERE [Name] = @Name) = 0
		BEGIN
			INSERT INTO [Settings].[Settings] ([Name], [Value]) VALUES (@Name, @Value)	
		END
	ELSE
		UPDATE [Settings].[Settings] SET [Value] = @Value WHERE [Name] = @Name
END


GO--

ALTER TABLE Customers.City ADD
	DisplayInPopup bit NULL

GO--
	
ALTER TABLE Customers.Country ADD
	DisplayInPopup bit NULL

GO--


ALTER TABLE [Order].OrderContact ADD
	CustomField1 nvarchar(MAX) NULL,
	CustomField2 nvarchar(MAX) NULL,
	CustomField3 nvarchar(MAX) NULL

GO--

ALTER TABLE [Order].ShippingMethod ADD
	DisplayCustomFields bit NULL

GO--


ALTER PROCEDURE [Settings].[sp_GetExportFeedCategories] @moduleName NVARCHAR(50)
	,@onlyCount BIT
AS
BEGIN
	--template for result array
	DECLARE @result TABLE (CategoryId INT PRIMARY KEY CLUSTERED);
	-- templete for array of categories
	DECLARE @lcategory TABLE (CategoryId INT PRIMARY KEY CLUSTERED);

	INSERT INTO @lcategory
	SELECT t.CategoryId
	FROM [Settings].[ExportFeedSelectedCategories] AS t
	INNER JOIN CATALOG.Category ON t.CategoryId = Category.CategoryId
	WHERE HirecalEnabled = 1
		AND Enabled = 1
		AND [ModuleName] = @moduleName

	DECLARE @l1 INT

	SET @l1 = (
			SELECT MIN(CategoryId)
			FROM @lcategory
			);

	WHILE @l1 IS NOT NULL
	BEGIN
		--add categories by step thats no in array 
		INSERT INTO @result
		SELECT id
		FROM Settings.GetChildCategoryByParent(@l1) AS dt
		INNER JOIN CATALOG.Category ON CategoryId = id
		WHERE dt.id NOT IN (
				SELECT CategoryId
				FROM @result
				)
			AND HirecalEnabled = 1
			AND Enabled = 1
			
		insert into @result
		select id from [Settings].[GetParentsCategoryByChild] (@l1) as dt
		where dt.id not in (SELECT CategoryId FROM @result)
		
		SET @l1 = (
				SELECT MIN(CategoryId)
				FROM @lcategory
				WHERE CategoryId > @l1
				);
	END;

	-- templete for array of categoiries by only selected product
	DECLARE @lproduct TABLE (CategoryId INT PRIMARY KEY CLUSTERED);

	INSERT INTO @lproduct
	SELECT DISTINCT CategoryID
	FROM [Catalog].[ProductCategories]
	INNER JOIN [Settings].[ExportFeedSelectedProducts] ON [ProductCategories].[ProductID] = [ExportFeedSelectedProducts].[ProductID]
		AND [ModuleName] = @moduleName
	WHERE [ExportFeedSelectedProducts].[ProductID] IN (
			SELECT Product.[ProductID]
			FROM CATALOG.Product
			INNER JOIN [Catalog].[Offer] ON [Offer].[ProductID] = [Product].[ProductID]
			WHERE Offer.Price > 0
				AND (
					Offer.Amount > 0
					OR Product.AllowPreorder = 1
					)
				AND CategoryEnabled = 1
				AND Enabled = 1
			)

	SET @l1 = (
			SELECT MIN(CategoryId)
			FROM @lproduct
			);

	WHILE @l1 IS NOT NULL
	BEGIN
		--add categories by step thats no in array 
		INSERT INTO @result
		SELECT id
		FROM Settings.[GetParentsCategoryByChild](@l1) AS dt
		INNER JOIN CATALOG.Category ON CategoryId = id
		WHERE dt.id NOT IN (
				SELECT CategoryId
				FROM @result
				)
			AND HirecalEnabled = 1
			AND Enabled = 1
			
		
		SET @l1 = (
				SELECT MIN(CategoryId)
				FROM @lproduct
				WHERE CategoryId > @l1
				);
	END;

	IF @onlyCount = 1
	BEGIN
		SELECT Count([CategoryID])
		FROM [Catalog].[Category]
		WHERE CategoryID <> 0
			AND CategoryId IN (
				SELECT CategoryId
				FROM @result
				)
	END
	ELSE
	BEGIN
		SELECT [CategoryID]
			,[ParentCategory]
			,[Name]
		FROM [Catalog].[Category]
		WHERE CategoryID <> 0
			AND CategoryId IN (
				SELECT CategoryId
				FROM @result
				)
	END
END

GO--

ALTER TABLE [Order].OrderPickPoint ADD
	AdditionalData nvarchar(MAX) NULL
GO--


ALTER TABLE [Order].[Order] ADD
	BonusCost float NULL
GO--

ALTER TABLE [Customers].[Customer] ADD
	BonusCardNumber bigint NULL
GO--

Alter PROCEDURE [Customers].[sp_UpdateCustomerInfo]  
   @CustomerID uniqueidentifier,  
   @FirstName nvarchar(70),  
   @LastName nvarchar(70),  
   @Phone nvarchar(max),  
   @Subscribed4News bit,  
   @Email nvarchar(100),  
   @CustomerGroupId int = NULL,  
   @CustomerRole int, 
   @BonusCardNumber bigint
AS  
BEGIN  
 UPDATE [Customers].[Customer]  
    SET [FirstName] = @FirstName,
		[LastName] = @LastName,
		[Phone] = @Phone,
		[Subscribed4News] = @Subscribed4News,
		[Email] = @Email,
		[CustomerGroupId] = @CustomerGroupId,
		[CustomerRole] = @CustomerRole,
		[BonusCardNumber] = @BonusCardNumber
   WHERE CustomerID = @CustomerID
END 
GO--


Insert Into [Settings].[Settings] ([Name], [Value]) Values ('GoogleAnalyticsEnableDemogrReports', 'False') 
GO--

Update [Settings].[Settings] Set [Name] = 'GoogleAnalyticsPassword' Where [Name] = 'oogleAnalyticsPassword'
GO--

ALTER TABLE Catalog.Category ADD
	Sorting int NOT NULL CONSTRAINT DF_Category_Sorting DEFAULT 0

GO--


ALTER PROCEDURE [Catalog].[sp_AddCategory]		
		@Name nvarchar(255),
		@ParentCategory int,
		@Description nvarchar(max),
		@BriefDescription nvarchar(max),
		@SortOrder int,
		@Enabled bit,
		@DisplayStyle nvarchar(50),
		@DisplayChildProducts bit,
		@DisplayBrandsInMenu bit,
		@DisplaySubCategoriesInMenu bit,
		@UrlPath nvarchar(150),
		@Sorting int
AS
BEGIN
	INSERT INTO [Catalog].[Category]
			 (
			    [Name]
			   ,[ParentCategory]
			   ,[Description]
			   ,[BriefDescription]
			   ,[Products_Count]
			   ,[SortOrder]
			   ,[Enabled]
			   ,[DisplayStyle]
			   ,[DisplayChildProducts]
			   ,[DisplayBrandsInMenu]
			   ,[DisplaySubCategoriesInMenu]
				,[UrlPath]
				,Sorting
			)
		 VALUES
			(
				@Name,
				@ParentCategory,
				@Description,
				@BriefDescription,
				0,
				@SortOrder,
				@Enabled,
				@DisplayStyle,
			    @DisplayChildProducts,
			    @DisplayBrandsInMenu,
			    @DisplaySubCategoriesInMenu,
				@UrlPath,
				@Sorting
			);
	Select SCOPE_IDENTITY();
END

GO--

ALTER PROCEDURE [Catalog].[sp_UpdateCategory]

		@CategoryID int,
		@Name nvarchar(255),
		@ParentCategory int,
		@Description nvarchar(max),
		@BriefDescription nvarchar(max),
		@SortOrder int,
		@Enabled bit,
		@DisplayStyle varchar(30) = NULL,
		@DisplayChildProducts bit = '0',
		@DisplayBrandsInMenu bit,
		@DisplaySubCategoriesInMenu bit,
		@UrlPath nvarchar(150),
		@Sorting int

AS
BEGIN

UPDATE [Catalog].[Category]
	   SET 
		   [Name] = @Name
		  ,[ParentCategory] = @ParentCategory
		  ,[Description] = @Description
		  ,[BriefDescription] = @BriefDescription
		  ,[SortOrder] = @SortOrder
		  ,[Enabled] = @Enabled
		  ,[DisplayStyle] = @DisplayStyle
		  ,[DisplayChildProducts] = @DisplayChildProducts
		  ,[DisplayBrandsInMenu] = @DisplayBrandsInMenu
		  ,[DisplaySubCategoriesInMenu] = @DisplaySubCategoriesInMenu
		  ,[UrlPath]=@UrlPath
		  ,Sorting = @Sorting
		WHERE CategoryID = @CategoryID
END

GO--


ALTER TABLE [Order].[Order] ADD
	DiscountCost float(53) NULL

GO--


CREATE TABLE [Customers].[Subscription](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Email] [nvarchar](50) NOT NULL,
	[Subscribe] [bit] NOT NULL,
	[SubscribeDate] [datetime] NOT NULL,
	[UnsubscribeDate] [datetime] NULL,
	[UnsubscribeReason] [nvarchar](max) NULL,
 CONSTRAINT [PK_Subscription] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO--

INSERT INTO [Customers].[Subscription] 
([Email],[Subscribe],[SubscribeDate],[UnsubscribeDate],[UnsubscribeReason])
SELECT [Email],[Subscribed4News] as [Subscribe], GETDATE(), NULL, NULL FROM [Customers].[Customer]

GO--

INSERT INTO [Customers].[Subscription] 
([Email],[Subscribe],[SubscribeDate],[UnsubscribeDate],[UnsubscribeReason])
SELECT [Email],[Enable] as [Subscribe], GETDATE(), NULL, NULL FROM [dbo].[Subscribe] WHERE [Email] not in (SELECt [Email] FROM [Customers].[Subscription])

GO--

ALTER TABLE [Customers].[Customer] DROP COLUMN [Subscribed4News]

GO--

ALTER PROCEDURE [Customers].[sp_UpdateCustomerInfo]  
   @CustomerID uniqueidentifier,  
   @FirstName nvarchar(70),  
   @LastName nvarchar(70),  
   @Phone nvarchar(max),     
   @Email nvarchar(100),  
   @CustomerGroupId int = NULL,  
   @CustomerRole int, 
   @BonusCardNumber bigint
AS  
BEGIN  
 UPDATE [Customers].[Customer]  
    SET [FirstName] = @FirstName,
		[LastName] = @LastName,
		[Phone] = @Phone,		
		[Email] = @Email,
		[CustomerGroupId] = @CustomerGroupId,
		[CustomerRole] = @CustomerRole,
		[BonusCardNumber] = @BonusCardNumber
   WHERE CustomerID = @CustomerID
END 

GO--

ALTER PROCEDURE [Customers].[sp_AddCustomer]
		   @CustomerGroupID int,
           @Password nvarchar(100),
           @FirstName nvarchar(70),
           @LastName nvarchar(70),
           @Phone nvarchar(max),
		   @RegistrationDateTime datetime,           
           @Email nvarchar(100),
           @CustomerRole int,
           @Patronymic nvarchar(70)
AS
BEGIN
	INSERT INTO [Customers].[Customer]
           ([CustomerGroupID]
           ,[Password]
           ,[FirstName]
           ,[LastName]
           ,[Phone]
           ,[RegistrationDateTime]           
           ,[Email]
           ,[CustomerRole]
           ,[Patronymic])
     VALUES
           (@CustomerGroupID
           ,@Password
           ,@FirstName
           ,@LastName
           ,@Phone
           ,@RegistrationDateTime           
           ,@Email
           ,@CustomerRole
           ,@Patronymic);
     SELECT CustomerID from [Customer] where Email =@Email
END

GO--

ALTER PROCEDURE [Customers].[sp_GetCustomerByID]
	@CustomerID uniqueidentifier
AS
BEGIN
	SELECT     Customers.Customer.CustomerID, 
				      Customers.Customer.Email,
				      Customers.Customer.Password, Customers.Customer.FirstName, 
                      Customers.Customer.LastName, 
                      Customers.Customer.Phone, Customers.Customer.RegistrationDateTime, 
                      Customers.CustomerGroup.GroupName, 
                      Customers.CustomerGroup.GroupDiscount, Customers.CustomerGroup.CustomerGroupID
	FROM         Customers.Customer INNER JOIN
                      Customers.CustomerGroup ON Customers.Customer.CustomerGroupID = Customers.CustomerGroup.CustomerGroupID
	WHERE Customers.Customer.CustomerID = @CustomerID
END

GO--

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SubscribeDeactivateReason]') AND type in (N'U'))
DROP TABLE [dbo].[SubscribeDeactivateReason]

GO--

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Subscribe]') AND type in (N'U'))
DROP TABLE [dbo].[Subscribe]

GO--

Insert Into [Settings].[Settings] ([Name], [Value]) Values ('DisplayToolBarBottom', 'True') 

GO--

ALTER TABLE Settings.Settings ADD CONSTRAINT UQ_Settings_Name UNIQUE (Name)

GO--

Insert Into [Settings].[Settings] (Name, Value) Values ('ShowShippingsMethodsInDetails', '0')

GO--

Insert Into [Settings].[Settings] (Name, Value) Values ('ShippingsMethodsInDetailsCount', '3')

GO--

Insert Into [Settings].[Settings] (Name, Value) Values ('SocialShareCustomEnabled', 'False')

GO--

Insert Into [Settings].[Settings] (Name, Value) Values ('SocialShareCustomCode', '')

GO--

ALTER TABLE Customers.Country ADD
	SortOrder int NULL
	
GO--

Update Customers.Country Set SortOrder = 0, DisplayInPopup=0

GO--

ALTER TABLE Customers.City ADD
	PhoneNumber nvarchar(MAX) NULL
GO--

update Customers.City set CitySort = 0, DisplayInPopup=0

GO--

alter table   Customers.City
alter column DisplayInPopup bit not null

GO--

Alter PROCEDURE [Customers].[sp_AddCustomer]  
	   @CustomerGroupID int,  
	   @Password nvarchar(100),  
	   @FirstName nvarchar(70),  
	   @LastName nvarchar(70),  
	   @Phone nvarchar(max),  
       @RegistrationDateTime datetime,             
       @Email nvarchar(100),  
       @CustomerRole int,  
       @Patronymic nvarchar(70),
       @BonusCardNumber bigint
AS  
BEGIN  
 INSERT INTO [Customers].[Customer]  
           ([CustomerGroupID]  
           ,[Password]  
           ,[FirstName]  
           ,[LastName]  
           ,[Phone]  
           ,[RegistrationDateTime]             
           ,[Email]  
           ,[CustomerRole]  
           ,[Patronymic]
           ,[BonusCardNumber])  
     VALUES  
           (@CustomerGroupID  
           ,@Password  
           ,@FirstName  
           ,@LastName  
           ,@Phone  
           ,@RegistrationDateTime             
           ,@Email  
           ,@CustomerRole  
           ,@Patronymic
           ,@BonusCardNumber);  
     SELECT CustomerID from [Customer] where Email =@Email  
END  

GO--

IF NOT EXISTS (
  SELECT * 
  FROM   sys.columns 
  WHERE  object_id = OBJECT_ID(N'[Catalog].[Product]') 
         AND name = 'Adult'
)
begin 
ALTER TABLE Catalog.Product ADD
	Adult bit NOT NULL CONSTRAINT DF_Product_Adult DEFAULT 0
end
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
			@Adult bit
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
			@Adult
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
		@Adult	bit
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
 WHERE ProductID = @ProductID	 
END

GO--



ALTER FUNCTION [Settings].[PhotoToString]
(
	@ColorId int,
	@ProductId int
)
RETURNS varchar(Max)
AS
BEGIN
	DECLARE @result varchar(max)
	if @ColorId is null
	begin
		SELECT  @result = coalesce(@result + ',', '') + PhotoName
		FROM    Catalog.Photo
		WHERE [Photo].[ObjId]=@ProductId and Type ='Product' order by Main DESC, PhotoSortOrder ASC
	end
	else
	Begin
		SELECT  @result = coalesce(@result + ',', '') + PhotoName
		FROM    Catalog.Photo
		WHERE   [Photo].[ObjId]=@ProductId and Type ='Product' and (Photo.ColorID = @ColorID OR Photo.ColorID is null) order by Main DESC, PhotoSortOrder ASC
	end
	RETURN @Result
END

GO--

CREATE TABLE [Catalog].[ProductExt](
	[ProductId] [int] NOT NULL,
	[CountPhoto] [int] NULL,
	[PhotoId] [int] NULL,	
	[VideosAvailable] [bit] NOT NULL,
	[MaxAvailable] [bit] NOT NULL,
	[NotSamePrices] [bit] NOT NULL,
	[MinPrice] [float] NULL,
	[Colors] [nvarchar](max) NULL,
	[AmountSort] [bit] NULL,
	[OfferId] [int] NULL,
 CONSTRAINT [PK_ProductExt] PRIMARY KEY CLUSTERED 
(
	[ProductId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO--

ALTER TABLE [Catalog].[ProductExt]  WITH CHECK ADD  CONSTRAINT [FK_ProductExt_Product] FOREIGN KEY([ProductId])
REFERENCES [Catalog].[Product] ([ProductId])
ON DELETE CASCADE

GO--

ALTER TABLE [Catalog].[ProductExt] CHECK CONSTRAINT [FK_ProductExt_Product]

GO--

CREATE PROCEDURE [Catalog].[PreCalcProductParams] @productId INT
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
					WHEN Offer.Amount <= 0
						OR Offer.Amount < IsNull(Product.MinAmount, 0)
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

CREATE PROCEDURE [Catalog].[PreCalcProductParamsMass]
AS
BEGIN
	SET NOCOUNT ON;
INSERT INTO CATALOG.ProductExt (
	ProductId
	,CountPhoto
	,PhotoId	
	,VideosAvailable
	,MaxAvailable
	,NotSamePrices
	,MinPrice
	,Colors
	,AmountSort
	,OfferId
	) (
	SELECT ProductId
	,0
	,NULL	
	,0
	,0
	,0
	,0
	,NULL
	,0
	,null FROM CATALOG.Product WHERE Product.ProductId NOT IN (
		SELECT ProductId
		FROM CATALOG.ProductExt
		)
	)
UPDATE [Catalog].[ProductExt]
SET [CountPhoto] = (
		SELECT CASE 
				WHEN Offer.ColorID IS NOT NULL
					THEN (
							SELECT Count(PhotoId)
							FROM [Catalog].[Photo]
							WHERE (
									[Photo].ColorID = Offer.ColorID
									OR [Photo].ColorID IS NULL
									)
								AND [Photo].[ObjId] = [ProductExt].ProductId
								AND Type = 'Product'
							)
				ELSE (
						SELECT Count(PhotoId)
						FROM [Catalog].[Photo]
						WHERE [Photo].[ObjId] = [ProductExt].ProductId
							AND Type = 'Product'
						)
				END
		FROM [Catalog].[Offer]
		WHERE [ProductID] = [ProductExt].ProductId and main =1
		)
	,[PhotoId] = (
		SELECT CASE 
				WHEN Offer.ColorID IS NOT NULL
					THEN (
							SELECT TOP (1) PhotoId
							FROM [Catalog].[Photo]
							WHERE (
									[Photo].ColorID = Offer.ColorID
									OR [Photo].ColorID IS NULL
									)
								AND [Photo].[ObjId] = [ProductExt].ProductId
								AND Type = 'Product'
							ORDER BY main DESC
								,[Photo].[PhotoSortOrder]
							)
				ELSE (
						SELECT TOP (1) PhotoId
						FROM [Catalog].[Photo]
						WHERE [Photo].[ObjId] = [ProductExt].ProductId
							AND Type = 'Product'
						ORDER BY main DESC
							,[Photo].[PhotoSortOrder]
						)
				END
		FROM [Catalog].[Offer]
		WHERE [ProductID] = [ProductExt].ProductId and main =1
		)	
	,[VideosAvailable] = (
		SELECT CASE 
				WHEN COUNT(ProductVideoID) > 0
					THEN 1
				ELSE 0
				END
		FROM [Catalog].[ProductVideo]
		WHERE ProductID = [ProductExt].ProductId
		)
	,[MaxAvailable] = (
		SELECT CASE 
				WHEN Max(Offer.Amount) > 0
					THEN 1
				ELSE 0
				END
		FROM [Catalog].Offer
		WHERE ProductId = [ProductExt].ProductId
		)
	,[NotSamePrices] = (
		SELECT CASE 
				WHEN max(price) - min(price) > 0
					THEN 1
				ELSE 0
				END
		FROM [Catalog].offer
		WHERE offer.productid = [ProductExt].ProductId
		)
	,[MinPrice] = (
		SELECT min(price)
		FROM [Catalog].offer
		WHERE offer.productid = [ProductExt].ProductId
		)
	,[Colors] = (
		SELECT [Settings].[ProductColorsToString]([ProductExt].ProductId)
		)
	,[AmountSort] = (
		SELECT CASE 
				WHEN Offer.Amount <= 0
					OR Offer.Amount < IsNull(Product.MinAmount, 0)
					THEN 0
				ELSE 1
				END
		FROM [Catalog].Offer
		INNER JOIN [Catalog].Product ON Product.ProductId = Offer.ProductId
		WHERE Offer.ProductId = [ProductExt].ProductId
			AND main = 1
		)
	,[OfferId] = (SELECT OfferID
		FROM [Catalog].offer
		WHERE offer.productid = [ProductExt].ProductId and (offer.Main = 1 OR offer.Main IS NULL))
   
END

GO--

exec [Catalog].[PreCalcProductParamsMass]

GO--


--missing patch 4.0.6


CREATE NONCLUSTERED INDEX Product_UrlPath
ON [Catalog].[Product] ([UrlPath],[ProductId])

GO--

CREATE NONCLUSTERED INDEX Category_UrlPath
ON [Catalog].[category] ([UrlPath],[categoryID])

GO--


CREATE  FUNCTION [Settings].[ArtNoToString]
(
	@ProductId int
)
RETURNS varchar(Max)
AS
BEGIN
	DECLARE @result varchar(max)
	SELECT  @result = coalesce(@result + ', ', '') + Artno
	FROM    Catalog.Offer 
	WHERE Offer.ProductID=@ProductId order by main desc
	RETURN @Result
END


GO--
CREATE NONCLUSTERED INDEX [Product_New] ON [Catalog].[Product] 
(
	[New] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]

GO--

CREATE NONCLUSTERED INDEX [Product_Bestseller] ON [Catalog].[Product] 
(
	[Bestseller] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]

GO--

CREATE NONCLUSTERED INDEX [Product_Discount] ON [Catalog].[Product] 
(
	[Discount] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]

GO--

alter table [order].[orderbyrequest]
alter column Quantity float not null
--missing patch 4.0.6


Drop Table [Customers].[GeoIP]

GO--

Alter Table [CMS].[MainMenu]
Alter Column [MenuItemUrlPath] nvarchar(MAX) NOT NULL

GO--

Alter Table [CMS].[BottomMenu]
Alter Column [MenuItemUrlPath] nvarchar(MAX) NOT NULL

GO--

ALTER PROCEDURE [CMS].[sp_UpdateMenuItemByItemId]
	@MenuType nvarchar(20),
	@MenuItemID int,
	@MenuItemParentID int,
	@MenuItemName nvarchar(100),
	@MenuItemUrlType int,
	@MenuItemUrlPath nvarchar(MAX),
	@MenuItemIcon nvarchar(100),
	@ShowMode int,
	@Enabled bit,
	@Blank bit,
	@SortOrder int,
	@NoFollow bit
AS  
BEGIN
IF @MenuType = 'Top'
	Begin
		Update [CMS].[MainMenu] 
			Set [MenuItemParentID] = @MenuItemParentID, 
				[MenuItemName] = @MenuItemName, 
				[MenuItemUrlType] = @MenuItemUrlType, 			
				[MenuItemUrlPath] = @MenuItemUrlPath, 
				[MenuItemIcon] = @MenuItemIcon, 
				[ShowMode] = @ShowMode,
				[Enabled] = @Enabled,
				[SortOrder] = @SortOrder,
				[Blank] = @Blank,
				[Nofollow] = @NoFollow
		Where [MenuItemID] = @MenuItemID
	END
Else
	Begin
		Update [CMS].[BottomMenu] 
			Set [MenuItemParentID] = @MenuItemParentID, 
				[MenuItemName] = @MenuItemName, 
				[MenuItemUrlType] = @MenuItemUrlType, 			
				[MenuItemUrlPath] = @MenuItemUrlPath, 
				[MenuItemIcon] = @MenuItemIcon, 
				[ShowMode] = @ShowMode,
				[Enabled] = @Enabled,
				[SortOrder] = @SortOrder,
				[Blank] = @Blank,
				[Nofollow] = @NoFollow
		Where [MenuItemID] = @MenuItemID
	END
END

GO--

DROP TABLE [Catalog].[TaxMappingOnProduct]
GO--

DROP TABLE [Catalog].[TaxRegionRate]
GO--

DROP TABLE [Catalog].[TaxSelectedCategories]
GO-- 


ALTER TABLE Catalog.Tax
	DROP CONSTRAINT DF_Tax_Priority
GO-- 

ALTER TABLE Catalog.Tax
	DROP CONSTRAINT DF_Tax_DependsON
GO-- 

ALTER TABLE Catalog.Tax
	DROP COLUMN Priority, DependsOnAddress, RegNumber, RateType
GO-- 

ALTER TABLE Catalog.Tax
	DROP CONSTRAINT FK_Tax_Country

GO-- 
ALTER TABLE Catalog.Tax
	DROP COLUMN CountryID

EXECUTE sp_rename N'Catalog.Tax.FederalRate', N'Rate', 'COLUMN' 
GO--

ALTER TABLE [Order].OrderTax ADD
	TaxRate float(53) NULL
GO--

Update [Order].OrderTax set TaxRate = (select rate from Catalog.Tax where OrderTax.TaxId=Tax.TaxID)

GO--

alter table [Order].OrderTax
alter column TaxRate float(53) NOT NULL

GO--
ALTER PROCEDURE [Catalog].[sp_GetPropertyValuesByProductID] @ProductID INT
AS
BEGIN
	SET NOCOUNT ON;

	SELECT [PropertyValue].[PropertyValueID]
		,[PropertyValue].[PropertyID]
		,[PropertyValue].[Value]
		,[ProductPropertyValue].[SortOrder]
		,[Property].UseinFilter
		,[Property].UseIndetails
		,[Property].[Name] as PropertyName
		,[Property].[SortOrder] as PropertySortOrder
		,[Property].[Expanded] as Expanded
		,[Property].[Type] as [Type]
		,[Property].[GroupId]
	FROM [Catalog].[PropertyValue]
	INNER JOIN [Catalog].[ProductPropertyValue] ON [ProductPropertyValue].[PropertyValueID] = [PropertyValue].[PropertyValueID]
	inner join [Catalog].[Property] on [Property].[PropertyID] = [PropertyValue].[PropertyID]
	WHERE [ProductID] = @ProductID
	ORDER BY [ProductPropertyValue].[SortOrder]
END

GO--

ALTER PROCEDURE [Catalog].[sp_GetPropertyValuesByPropertyID]
	@PropertyID int
AS
BEGIN
	SET NOCOUNT ON;

	SELECT [PropertyValueID],[Property].[PropertyID],[Value],[PropertyValue].[SortOrder],[Property].UseinFilter, [Property].UseIndetails, 
		   Property.Name as PropertyName, Property.SortOrder as PropertySortOrder, Property.Expanded, Property.Type, GroupId
	FROM [Catalog].[PropertyValue]
	INNER JOIN [Catalog].[Property] ON [Property].[PropertyID] = [PropertyValue].[PropertyID]
	WHERE [Property].[PropertyID] = @PropertyID
END

GO--

ALTER PROCEDURE [Catalog].[sp_GetPropertyValueByID]
	@PropertyValueId int
AS
BEGIN
	SELECT [PropertyValueID],[Property].[PropertyID],[Value],[PropertyValue].[SortOrder],[Property].UseinFilter, [Property].UseIndetails, 
		   Property.Name as PropertyName, Property.SortOrder as PropertySortOrder, Property.Expanded, Property.Type, GroupId
	FROM [Catalog].[PropertyValue]
	INNER JOIN [Catalog].[Property] ON [Property].[PropertyID] = [PropertyValue].[PropertyID]
	WHERE [PropertyValue].[PropertyValueID] = @PropertyValueId
END

GO--

Insert Into [Settings].[Settings] ([Name],[Value]) Values('DisplayCityInTopPanel', 'True');
Insert Into [Settings].[Settings] ([Name],[Value]) Values('DisplayCityBubble', 'True');

GO--

update [order].shippingparam set ParamValue='QjSPqGyo6BQm9gX6qDElCWXik2It8f25' where ShippingParamID='791' and ParamValue='AneMgQMokfG45eCB4i982Dbb556xtvvZ'

GO--

delete from [order].shippingmethod where ShippingMethodID=175 and 	ShippingType=9 and Name='Бесплатная доставка при заказе от 5000 р.' and Enabled=1 and SortOrder=0

GO--

update  [order].shippingmethod set ShippingType =12 where ShippingMethodID=209 and 	ShippingType=1 and Name='Самовывоз' and Enabled=1 and SortOrder=0
	
GO--

update  [order].shippingmethod set  DisplayCustomFields= 1 where ShippingType <>12

GO--

update dbo.Modules set Active=0 where ModuleStringID='Snowfall'

GO--

delete from dbo.Modules where ModuleStringID='OnePageCheckout'

Go--


CREATE SCHEMA [Shipping] AUTHORIZATION [dbo]

GO--

CREATE TABLE [Shipping].[CdekCities](
	[Id] [int] NOT NULL,
	[CityName] [nvarchar](200) NOT NULL,
	[OblName] [nvarchar](200) NOT NULL,
	[NalSumLimit] [nvarchar](100) NULL
) ON [PRIMARY]

GO--


Update Settings.Settings set value='logo.png' where name='MainPageLogoFileName' and value='logo_20130605155751.png'

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
		,[Property].[Name] as PropertyName
		,[Property].[SortOrder] as PropertySortOrder
		,[Property].[Expanded] as Expanded
		,[Property].[Type] as [Type]
		,[Property].GroupId as GroupId
	FROM [Catalog].[PropertyValue]
	INNER JOIN [Catalog].[ProductPropertyValue] ON [ProductPropertyValue].[PropertyValueID] = [PropertyValue].[PropertyValueID]
	inner join [Catalog].[Property] on [Property].[PropertyID] = [PropertyValue].[PropertyID]
	WHERE [ProductID] = @ProductID
	ORDER BY [ProductPropertyValue].[SortOrder]
END
GO--

create PROCEDURE [Settings].[sp_GetCsvProducts] @moduleName NVARCHAR(50)
	,@onlyCount BIT
	,@exportNoInCategory BIT
AS
BEGIN
	DECLARE @res TABLE (productId INT PRIMARY KEY CLUSTERED);
	DECLARE @lproduct TABLE (productId INT PRIMARY KEY CLUSTERED);
	DECLARE @lproductNoCat TABLE (productId INT PRIMARY KEY CLUSTERED);

	INSERT INTO @lproduct
	SELECT [ProductID]
	FROM [Settings].[ExportFeedSelectedProducts]
	WHERE [ModuleName] = @moduleName;

	IF (@exportNoInCategory = 1)
	BEGIN
		INSERT INTO @lproductNoCat
		SELECT [ProductID]
		FROM [Catalog].Product
		WHERE [ProductID] NOT IN (
				SELECT [ProductID]
				FROM [Catalog].[ProductCategories]
				);
	END

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
		SELECT COUNT(ProductID)
		FROM [Catalog].[Product]
		WHERE EXISTS (
				SELECT 1
				FROM [Catalog].[ProductCategories]
				WHERE [ProductCategories].[ProductID] = [Product].[ProductID]
					AND (
						[ProductCategories].[ProductID] IN (
							SELECT productId
							FROM @lproduct
							)
						OR [ProductCategories].CategoryId IN (
							SELECT CategoryId
							FROM @lcategory
							)
						)
				)
			OR EXISTS (
				SELECT 1
				FROM @lproductNoCat AS TEMP
				WHERE TEMP.productId = [Product].[ProductID]
				)
	END
	ELSE
	BEGIN
		SELECT *
		FROM [Catalog].[Product]
		LEFT JOIN [Catalog].[Photo] ON [Photo].[ObjId] = [Product].[ProductID]
			AND Type = 'Product'
			AND Photo.[Main] = 1
		WHERE EXISTS (
				SELECT 1
				FROM [Catalog].[ProductCategories]
				WHERE [ProductCategories].[ProductID] = [Product].[ProductID]
					AND (
						[ProductCategories].[ProductID] IN (
							SELECT productId
							FROM @lproduct
							)
						OR [ProductCategories].CategoryId IN (
							SELECT CategoryId
							FROM @lcategory
							)
						)
				)
			OR EXISTS (
				SELECT 1
				FROM @lproductNoCat AS TEMP
				WHERE TEMP.productId = [Product].[ProductID]
				)
	END
END

GO--
insert into [Settings].[Settings] (Name, Value) values ('CsvColumSeparator',';')
insert into [Settings].[Settings] (Name, Value) values ('CsvPropertySeparator',':')
update [Settings].[Settings] set Value='Windows-1251' where Name='CsvEnconing'
update [Settings].[Settings] set Value=';' where Name='CsvSeparator'

GO--

insert into [Settings].[ExportFeedSelectedCategories] (ModuleName, CategoryID) values ('CsvExport',0)

GO--

insert into Settings.Settings (name, value) values  ('EnableInplace', 'True')

GO--

ALTER TABLE Catalog.PropertyGroup ADD
	SortOrder int NULL

GO--

update catalog.PropertyGroup  set SortOrder = 0

GO--

alter table catalog.PropertyGroup
alter column SortOrder int not null

GO--

sp_rename 'Catalog.PropertyGroup_Category', 'PropertyGroupCategory'

GO--

ALTER TABLE Catalog.PropertyGroupCategory
	DROP COLUMN SortOrder

GO--

EXEC sp_rename 'Catalog.PropertyGroup.Name', 'GroupName', 'COLUMN'

GO--

EXEC sp_rename 'Catalog.PropertyGroup.SortOrder', 'GroupSortOrder', 'COLUMN'

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
END


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
	left join Catalog.PropertyGroup on PropertyGroup.PropertyGroupID = [Property].GroupID
	WHERE [ProductID] = @ProductID
	ORDER BY case when PropertyGroup.GroupSortOrder is null then 1 else 0 end, PropertyGroup.GroupSortOrder, [Property].[SortOrder], [ProductPropertyValue].[SortOrder]
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




UPDATE [Settings].[InternalSettings] SET [settingValue] = '4.1.0' WHERE [settingKey] = 'db_version'



