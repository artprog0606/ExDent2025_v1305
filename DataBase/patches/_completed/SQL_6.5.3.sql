Update [Settings].[Localization] Set ResourceValue = 'Черный Список' Where ResourceKey = 'Admin.Customers.ViewCustomerHeader.Bad' and LanguageId = 1 
Update [Settings].[Localization] Set ResourceValue = 'Blacklisted' Where ResourceKey = 'Admin.Customers.ViewCustomerHeader.Bad' and LanguageId = 2 

GO--

if not exists(select * from [Settings].[Localization] where ResourceKey = 'Admin.Js.Customers.HaveSubscription')
begin
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.Customers.HaveSubscription','Есть подписка на новости')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.Customers.HaveSubscription','Have a subscription')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.Customers.HaveSubscriptionYes','Есть')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.Customers.HaveSubscriptionYes','Yes')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.Customers.HaveSubscriptionNo','Нет')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.Customers.HaveSubscriptionNo','No')
end

GO--

IF exists(SELECT [MailFormatTypeID] FROM [Settings].[MailFormatType] WHERE [MailType]='OnSetOrderManager')
BEGIN
	UPDATE [Settings].[MailFormatType]
	SET [Comment] = 'Письмо при назначении менеджеру заказа (#MANAGER_NAME#, #ORDER_ID#, #ORDER_URL#)'
	WHERE [MailType] = 'OnSetOrderManager'
END
ELSE
BEGIN
	INSERT INTO [Settings].[MailFormatType] ([TypeName],[SortOrder],[Comment],[MailType])
	VALUES ('Менеджеру назначен заказ',170,'Письмо при назначении менеджеру заказа (#MANAGER_NAME#, #ORDER_ID#, #ORDER_URL#)','OnSetOrderManager')
END

GO--
if not exists(select * from [Settings].[Localization] where ResourceKey = 'Core.Services.CMS.OrderManagerAssignedNotification.Title')
begin
Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (1, 'Core.Services.CMS.OrderManagerAssignedNotification.Title', 'Новый заказ'); 
Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (2, 'Core.Services.CMS.OrderManagerAssignedNotification.Title', 'New order'); 
Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (1, 'Core.Services.CMS.OrderManagerAssignedNotification.Body', 'Заказ №{0}'); 
Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (2, 'Core.Services.CMS.OrderManagerAssignedNotification.Body', 'Order number {0}'); 
Insert Into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (1, 'Core.Services.EBizProcessEventType.OrderManagerAssigned', 'Назначение менеджеру заказа')
Insert Into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (2, 'Core.Services.EBizProcessEventType.OrderManagerAssigned', 'Assignment to the order manager')
end
GO--

Update [Settings].[Localization] Set ResourceValue = 'Скачать лог' Where ResourceKey = 'Admin.ProgressData.DownloadErrorsLog' and LanguageId = 1 
Update [Settings].[Localization] Set ResourceValue = 'Download file log' Where ResourceKey = 'Admin.ProgressData.DownloadErrorsLog' and LanguageId = 2 

GO--


Update cms.menu set MenuItemUrlPath = Replace(MenuItemUrlPath, 'myaccount#?tab', 'myaccount?tab')

GO--

if not exists(select * from [Settings].[Localization] where ResourceKey = 'PreOrder.Index.FirstName')
begin
	Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (1, 'PreOrder.Index.FirstName', 'Имя'); 
	Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (2, 'PreOrder.Index.FirstName', 'First name'); 
	Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (1, 'PreOrder.Index.LastName', 'Фамилия'); 
	Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (2, 'PreOrder.Index.LastName', 'Last name'); 
end

GO--
if not exists(select * from [Settings].[Localization] where ResourceKey = 'Admin.Js.SendLetterToCustomer.LetterText')
begin
	Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (1, 'Admin.Js.SendLetterToCustomer.LetterText', 'Текст письма'); 
	Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (2, 'Admin.Js.SendLetterToCustomer.LetterText', 'Letter text'); 
end
if not exists(select * from [Settings].[Localization] where ResourceKey = 'Admin.ExportFeeed.СhoiceOfProducts.AllProducts')
begin
	Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (1, 'Admin.ExportFeeed.СhoiceOfProducts.AllProducts', 'Все товары'); 
	Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (2, 'Admin.ExportFeeed.СhoiceOfProducts.AllProducts', 'All products'); 
end
if not exists(select * from [Settings].[Localization] where ResourceKey = 'Admin.ExportFeeed.СhoiceOfProducts.Export')
begin
	Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (1, 'Admin.ExportFeeed.СhoiceOfProducts.Export', 'Экспортировать'); 
	Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (2, 'Admin.ExportFeeed.СhoiceOfProducts.Export', 'Export'); 
end
GO--

IF NOT EXISTS(SELECT 1 FROM sys.columns WHERE Name = N'YandexDeliveryDays' AND Object_ID = Object_ID(N'Catalog.Product'))
Begin
	ALTER TABLE Catalog.Product ADD
		YandexDeliveryDays nvarchar(5) NULL
End

GO--

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (1, 'Admin.Js.Product.YandexMarketDeliveryDays', 'Срок доставки');
Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (1, 'Admin.Js.Product.YandexDeliveryDays', 'Срок доставки товара');
Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (1, 'Admin.Js.Product.DeliveryDaysPrecept', 'Укажите число дней или промежуток, в который товар может доставлен покупателю. Например, 2 или 1-3. Не используйте буквы или иные символы, кроме дефиса.');
Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (1, 'Admin.Js.Product.ThisParameterUsedForYandexMarketDeliveryDays', 'Этот параметр выгружается в Яндекс.Маркет как атрибут ''days'' для элемента ''delivery-options''. Он указывает количество дней, за которое доставлен данный товар может быть доставлен покупателю.');
Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (1, 'Admin.Js.Product.IfProductDeliveryDaysIsEmpty', 'Если поле оставить пустым, в атрибут ''days'' элемента ''delivery-options'' будет выгружаться срок доставки, указанный в общих настройках выгрузки для Яндекс.Маркета.');

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (2, 'Admin.Js.Product.YandexMarketDeliveryDays', 'Delivery time');
Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (2, 'Admin.Js.Product.YandexDeliveryDays', 'Product delivery time');
Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (2, 'Admin.Js.Product.DeliveryDaysPrecept', 'Specify amount of days or days interval, in which this product can be delivered to customer. For example, 2 or 1-3. Don''t use letters or hyphens.');
Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (2, 'Admin.Js.Product.ThisParameterUsedForYandexMarketDeliveryDays', 'This parameter used as attribute ''days'' in element ''delivery-options'' for Yandex.Market feed. It sets number of days required for delivery of this product.');
Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (2, 'Admin.Js.Product.IfProductDeliveryDaysIsEmpty', 'If this field is empty, value for attribute ''days'' in element ''delivery-options'' will be taken from common Yandex.Market feed settings.');

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (1, 'Core.ExportImport.ProductFields.YandexDeliveryDays', 'Срок доставки');
Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (2, 'Core.ExportImport.ProductFields.YandexDeliveryDays', 'DeliveryTime');

Update [Settings].[Localization] set [ResourceValue] = 'Срок доставки в днях, если не указан у товара. Примеры: 0, 2, 1-3' where [ResourceKey] = 'Admin.ExportFeed.SettingsYandex.DeliveryTimeExample' and [LanguageId] = 1;
Update [Settings].[Localization] set [ResourceValue] = 'Days for delivery, if not specified in product. Examples: 0, 2, 1-3' where [ResourceKey] = 'Admin.ExportFeed.SettingsYandex.DeliveryTimeExample' and [LanguageId] = 2;


GO--

ALTER PROCEDURE [Catalog].[sp_AddProduct]      
	 @ArtNo nvarchar(50) = '',    
	 @Name nvarchar(255),       
	 @Ratio float,    
	 @Discount float,  
	 @DiscountAmount float,    
	 @Weight float,     
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
	 @YandexMarketCategory nvarchar(500),    
	 @Gtin nvarchar(50),    
	 @Adult bit,  
	 @Length float,  
	 @Width float,  
	 @Height float,  
	 @CurrencyID int,  
	 @ActiveView360 bit,  
	 @ManufacturerWarranty bit,  
	 @ModifiedBy nvarchar(50),  
	 @YandexTypePrefix nvarchar(500),  
	 @YandexModel nvarchar(500),  
	 @BarCode nvarchar(50),  
	 @Cbid float,  
	 @Fee float,  
	 @AccrueBonuses bit,
     @Taxid int, 
	 @YandexSizeUnit nvarchar(10),
	 @Cpa bit,
	 @DateModified datetime,
	 @YandexName nvarchar(255),
	 @YandexDeliveryDays nvarchar(5)
	AS    
BEGIN    
Declare @Id int    
INSERT INTO [Catalog].[Product]    
             ([ArtNo]    
			 ,[Name]                       
			 ,[Ratio]    
			 ,[Discount]  
			 ,[DiscountAmount]    
			 ,[Weight]    
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
			 ,YandexMarketCategory  
			 ,Gtin    
			 ,Adult  
			 ,Length  
			 ,Width  
			 ,Height  
			 ,CurrencyID  
			 ,ActiveView360  
			 ,ManufacturerWarranty  
			 ,ModifiedBy  
			 ,YandexTypePrefix  
			 ,YandexModel  
			 ,BarCode  
			 ,Cbid  
			 ,Fee  
			 ,AccrueBonuses
             ,TaxId		
			 ,YandexSizeUnit
			 ,Cpa	 
			 ,YandexName
			 ,YandexDeliveryDays
          )    
     VALUES    
           (@ArtNo,    
			 @Name,         
			 @Ratio,    
			 @Discount,    
			 @DiscountAmount,  
			 @Weight,  
			 @BriefDescription,    
			 @Description,    
			 @Enabled,    
			 @DateModified,    
			 @DateModified,    
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
			 @YandexMarketCategory,  
			 @Gtin,    
			 @Adult,  
			 @Length,  
			 @Width,  
			 @Height,  
			 @CurrencyID,  
			 @ActiveView360,  
			 @ManufacturerWarranty,  
			 @ModifiedBy,  
			 @YandexTypePrefix,  
			 @YandexModel,  
			 @BarCode,  
			 @Cbid,  
			 @Fee,  
			 @AccrueBonuses,
             @TaxId,
			 @YandexSizeUnit,
			 @Cpa,
			 @YandexName,
			 @YandexDeliveryDays
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
	 @DiscountAmount float,    
	 @Weight float,    
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
	 @YandexMarketCategory nvarchar(500),    
	 @Gtin nvarchar(50),    
	 @Adult bit,  
	 @Length float,  
	 @Width float,  
	 @Height float,  
	 @CurrencyID int,  
	 @ActiveView360 bit,  
	 @ManufacturerWarranty bit,  
	 @ModifiedBy nvarchar(50),  
	 @YandexTypePrefix nvarchar(500),  
	 @YandexModel nvarchar(500),  
	 @BarCode nvarchar(50),  
	 @Cbid float,  
	 @Fee float,  
	 @AccrueBonuses bit,
	 @TaxId int,
	 @YandexSizeUnit nvarchar(10),
	 @Cpa bit,
	 @DateModified datetime,
	 @YandexName nvarchar(255),
	 @YandexDeliveryDays nvarchar(5)

AS    
BEGIN    
	UPDATE [Catalog].[Product]    
	 SET [ArtNo] = @ArtNo    
	 ,[Name] = @Name    
	 ,[Ratio] = @Ratio    
	 ,[Discount] = @Discount  
	 ,[DiscountAmount] = @DiscountAmount    
	 ,[Weight] = @Weight  
	 ,[BriefDescription] = @BriefDescription    
	 ,[Description] = @Description    
	 ,[Enabled] = @Enabled    
	 ,[Recomended] = @Recomended    
	 ,[New] = @New    
	 ,[BestSeller] = @BestSeller    
	 ,[OnSale] = @OnSale    
	 ,[DateModified] = @DateModified    
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
	 ,[YandexMarketCategory]=@YandexMarketCategory  
	 ,[Gtin]=@Gtin    
	 ,[Adult] = @Adult  
	 ,[Length] = @Length  
	 ,[Width] = @Width  
	 ,[Height] = @Height  
	 ,[CurrencyID] = @CurrencyID  
	 ,[ActiveView360] = @ActiveView360  
	 ,[ManufacturerWarranty] = @ManufacturerWarranty  
	 ,[ModifiedBy] = @ModifiedBy  
	 ,[YandexTypePrefix] = @YandexTypePrefix  
	 ,[YandexModel] = @YandexModel  
	 ,[BarCode] = @BarCode  
	 ,[Cbid] = @Cbid  
	 ,[Fee] = @Fee  
	 ,[AccrueBonuses] = @AccrueBonuses
	 ,[TaxId] = @TaxId 
	 ,[YandexSizeUnit] = @YandexSizeUnit
	 ,[Cpa] = @Cpa
	 ,[YandexName] = @YandexName
	 ,[YandexDeliveryDays] = @YandexDeliveryDays
	WHERE ProductID = @ProductID      
END

GO--

ALTER PROCEDURE [Settings].[sp_GetExportFeedProducts] 
     @exportFeedId int
	,@onlyCount BIT
	,@exportNotAvailable bit
	,@selectedCurrency NVARCHAR(10)
	,@allowPreOrder bit = 0
	,@exportAllProducts bit
AS
BEGIN
	DECLARE @res TABLE (productId INT PRIMARY KEY CLUSTERED);
	DECLARE @lproduct TABLE (productId INT PRIMARY KEY CLUSTERED);

	INSERT INTO @lproduct
	SELECT [ProductID]
	FROM [Settings].[ExportFeedSelectedProducts]
	WHERE [ExportFeedId] = @exportFeedId;

	DECLARE @lcategory TABLE (CategoryId INT PRIMARY KEY CLUSTERED);
	DECLARE @l TABLE (CategoryId INT PRIMARY KEY CLUSTERED);

	INSERT INTO @l
	SELECT t.CategoryId
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
				WHERE 
					(
						SELECT TOP (1) [ProductCategories].[CategoryId]
						FROM [Catalog].[ProductCategories]
						INNER JOIN [Catalog].[Category] ON [Category].[CategoryId] = [ProductCategories].[CategoryId]
						WHERE [ProductID] = [Product].[ProductID]
							AND [Enabled] = 1				
							AND [ProductCategories].[CategoryId] in (SELECT CategoryId FROM @lcategory)
						Order By Main DESC, [ProductCategories].[CategoryId]
					) = [ProductCategories].[CategoryId]
					AND
					 (Offer.Price > 0 OR @exportNotAvailable = 1)
					AND (
						Offer.Amount > 0
						OR (Product.AllowPreOrder = 1 and  @allowPreOrder = 1)
						OR @exportNotAvailable = 1
						)
					AND CategoryEnabled = 1
					AND (Product.Enabled = 1 OR @exportNotAvailable = 1)
					AND (@exportAllProducts=1 OR (Select Count(ProductId) From Settings.ExportFeedExcludedProducts Where ExportFeedExcludedProducts.ProductId=Product.ProductId AND ExportFeedExcludedProducts.ExportFeedId=@exportFeedId) = 0)
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
			,[Product].[DiscountAmount]
			,AllowPreOrder
			,Amount
			,[ProductCategories].[CategoryId] AS [ParentCategory]
			,[Offer].[Price] AS Price
			,ShippingPrice
			,[Product].[Name]
			,[Product].[UrlPath]
			,[Product].[Description]
			,[Product].[BriefDescription]
			,[Product].SalesNote
			,OfferId
			,[Product].ArtNo
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
			,[Settings].PhotoToString(Offer.ColorID, Product.ProductId) AS Photos
			,ManufacturerWarranty
			,[Weight]
			,[Product].[Enabled]
			,[Offer].SupplyPrice AS SupplyPrice
			,[Offer].ArtNo AS OfferArtNo
			,[Product].BarCode
			,[Product].Cbid
			,[Product].Fee
			,[Product].YandexSizeUnit
			,[Product].MinAmount
			,[Product].Multiplicity			
			,[Product].Cpa
			,[Product].YandexName
			,[Product].YandexDeliveryDays
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
		LEFT JOIN [Customers].Country as country1 ON Brand.CountryID = country1.CountryID
		LEFT JOIN [Customers].Country as country2 ON Brand.CountryOfManufactureID = country2.CountryID
		INNER JOIN [Catalog].Currency ON Currency.CurrencyID = [Product].CurrencyID
		WHERE 
			(
				SELECT TOP (1) [ProductCategories].[CategoryId]
				FROM [Catalog].[ProductCategories]
				INNER JOIN [Catalog].[Category] ON [Category].[CategoryId] = [ProductCategories].[CategoryId]
				WHERE [ProductID] = [Product].[ProductID]
					AND [Enabled] = 1				
					AND [ProductCategories].[CategoryId] in (SELECT CategoryId FROM @lcategory)
				Order By Main DESC, [ProductCategories].[CategoryId]
			) = [ProductCategories].[CategoryId]		
			AND 
			(Offer.Price > 0 OR @exportNotAvailable = 1)
			AND (
				Offer.Amount > 0
				OR (Product.AllowPreOrder = 1 and  @allowPreOrder = 1)
				OR @exportNotAvailable = 1
				)
			AND CategoryEnabled = 1
			AND (Product.Enabled = 1 OR @exportNotAvailable = 1)
			AND (@exportAllProducts=1 OR (Select Count(ProductId) From Settings.ExportFeedExcludedProducts Where ExportFeedExcludedProducts.ProductId=Product.ProductId AND ExportFeedExcludedProducts.ExportFeedId=@exportFeedId) = 0)
	END
END

GO--

CREATE TABLE [Booking].[ReservationResourceTag](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[SortOrder] [int] NOT NULL,
 CONSTRAINT [PK_ReservationResourceTag] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO--


CREATE TABLE [Booking].[ReservationResourceTagsMap](
	[ReservationResourceId] [int] NOT NULL,
	[ReservationResourceTagId] [int] NOT NULL,
 CONSTRAINT [PK_ReservationResourceTagsMap] PRIMARY KEY CLUSTERED 
(
	[ReservationResourceId] ASC,
	[ReservationResourceTagId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO--

ALTER TABLE [Booking].[ReservationResourceTagsMap]  WITH CHECK ADD  CONSTRAINT [FK_ReservationResourceTagsMap_ReservationResource] FOREIGN KEY([ReservationResourceId])
REFERENCES [Booking].[ReservationResource] ([Id])
ON DELETE CASCADE
GO--

ALTER TABLE [Booking].[ReservationResourceTagsMap] CHECK CONSTRAINT [FK_ReservationResourceTagsMap_ReservationResource]
GO--

ALTER TABLE [Booking].[ReservationResourceTagsMap]  WITH CHECK ADD  CONSTRAINT [FK_ReservationResourceTagsMap_ReservationResourceTag] FOREIGN KEY([ReservationResourceTagId])
REFERENCES [Booking].[ReservationResourceTag] ([Id])
ON DELETE CASCADE
GO--

ALTER TABLE [Booking].[ReservationResourceTagsMap] CHECK CONSTRAINT [FK_ReservationResourceTagsMap_ReservationResourceTag]

GO--

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (1, 'Admin.Settings.Booking.Tags', 'Теги'); 
Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (1, 'Admin.Settings.Booking.Tags.Grid.SearchPlaceholder', 'Поиск по названию'); 

GO--

If (NOT EXISTS(Select * From [CMS].[StaticBlock] Where [Key] = 'requestOnProductSuccess'))
Begin
   Insert Into [CMS].[StaticBlock] ([Key],[InnerName],[Content],[Added],[Modified],[Enabled]) Values ('requestOnProductSuccess', 'Успешное оформление под заказ', 'Благодарим за Вашу заявку! После её обработки наш менеджер сразу же свяжется с Вами и сообщит о возможности и сроках поступления данной позиции в наш интернет-магазин.', getdate(), getdate(), 1)
End

GO--

UPDATE [Settings].[InternalSettings] SET [settingValue] = '6.5.3' WHERE [settingKey] = 'db_version'

