
CREATE TABLE [Module].[AvitoProductProperties](
	[ProductId] [int] NOT NULL,
	[PropertyName] [nvarchar](250) NOT NULL,
	[PropertyValue] [nvarchar](max) NOT NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO--
ALTER TABLE [Module].[AvitoProductProperties]  WITH CHECK ADD  CONSTRAINT [FK_AvitoProductProperties_Product] FOREIGN KEY([ProductId])
REFERENCES [Catalog].[Product] ([ProductId])
ON DELETE CASCADE
GO--
ALTER TABLE [Module].[AvitoProductProperties] CHECK CONSTRAINT [FK_AvitoProductProperties_Product]
GO--
CREATE UNIQUE NONCLUSTERED INDEX IX_AvitoProductProperties ON Module.AvitoProductProperties
	(
	ProductId,
	PropertyName
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Landings.CreateFunnel.Title','Создание Воронки')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Landings.CreateFunnel.Title','Create funnel')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Landing.Domain.LpFunnelType.Default','Пустой шаблон')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Landing.Domain.LpFunnelType.Default','Empty template')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Landing.Domain.LpFunnelType.OneProductUpSellDownSell','Бесплатный товар + Доставка')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Landing.Domain.LpFunnelType.OneProductUpSellDownSell','Free product + Shipping')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Landing.Domain.LpFunnelType.MultyProducts','Мультитоварная')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Landing.Domain.LpFunnelType.MultyProducts','Multi-commodity funnel')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Landing.Domain.LpFunnelType.Events','Событие')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Landing.Domain.LpFunnelType.Events','Event')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Landing.Domain.LpFunnelType.LeadMagnet','Лид Магнит')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Landing.Domain.LpFunnelType.LeadMagnet','Lead Magnet')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Landing.Domain.LpFunnelType.Couponator','Купонатор')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Landing.Domain.LpFunnelType.Couponator','Couponator')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Landing.Domain.LpFunnelType.VideoLeadMagnet','Видео лид Магнит')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Landing.Domain.LpFunnelType.VideoLeadMagnet','Video lead magnet')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Js.Shipping.ShippingMethodsLoading','Загрузка данных...')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Js.Shipping.ShippingMethodsLoading','Loading...')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Js.Payment.PaymentMethodsLoading','Загрузка данных...')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Js.Payment.PaymentMethodsLoading','Loading...')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ShippingMethods.Boxberry.NoticeCalculateCourier','Включить рассчет курьерской доставки, если настройка отключена, то при оформлении заказа рассчет курьерской доставки Boxberry производиться не будет')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ShippingMethods.Boxberry.NoticeCalculateCourier','Include calculation of courier delivery, if the setting is disabled, then at checkout the calculation of courier delivery of Boxberry will not be made')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Booking.NoAccess','Нет доступа')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Booking.NoAccess','No access')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Social.SocialWidget','Социальные виджеты')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Social.SocialWidget','Social widget')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Social.LinkVkActive','Вконтакте')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Social.LinkVkActive','Vkontakte')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Social.LinkFacebookActive','Facebook')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Social.LinkFacebookActive','Facebook')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Social.LinkInstagrammActive','Instagram')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Social.LinkInstagrammActive','Instagram')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Social.LinkTwitterActive','Twitter')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Social.LinkTwitterActive','Twitter')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Social.LinkTelegramActive','Telegram')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Social.LinkTelegramActive','Telegram')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Social.LinkOkActive','Одноклассники')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Social.LinkOkActive','Odnoklassniki')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Social.LinkYoutubeActive','Youtube')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Social.LinkYoutubeActive','Youtube')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Social.SocialGroups','Сообщества в соц сетях')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Social.SocialGroups','Social groups')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Home.SocialLinks.Title','Мы в соц сетях')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Home.SocialLinks.Title','We are in social networks')

GO--

if(Select Count(SettingID) From [Settings].[Settings] 
	Where ([Name]='LinkVkActive' or [Name]='LinkFacebookActive' or [Name]='LinkInstagrammActive' or [Name]='LinkOkActive' or [Name]='LinkTwitterActive' or [Name]='LinkTelegramActive' or [Name]='LinkYoutubeActive') and [Value]='true') > 0
begin
	Update [Cms].[Menu] Set [Enabled] = 0 Where [MenuItemName] = 'Мы в соц сетях'
end

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Services.CMS.BookingAddedNotification.Title','Новая бронь')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Services.CMS.BookingAddedNotification.Body','Оформлена бронь №{0}')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Services.CMS.BookingAddedNotification.Title','New booking')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Services.CMS.BookingAddedNotification.Body','New booking #{0}')

Insert Into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (1, 'Core.Services.EBizProcessEventType.BookingCreated', 'Новая бронь')
Insert Into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (2, 'Core.Services.EBizProcessEventType.BookingCreated', 'New booking')
Insert Into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (1, 'Core.Services.EBizProcessEventType.BookingChanged', 'Бронь изменена')
Insert Into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (2, 'Core.Services.EBizProcessEventType.BookingChanged', 'Booking changed')
Insert Into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (1, 'Core.Services.EBizProcessEventType.BookingDeleted', 'Бронь удалена')
Insert Into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (2, 'Core.Services.EBizProcessEventType.BookingDeleted', 'Booking deleted')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Landing.Domain.LpFunnelType.LandingFunnel','Лендинг Воронка')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Landing.Domain.LpFunnelType.LandingFunnel','Landing Funnel')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Landing.Domain.LpFunnelType.VideoLandingFunnel','Видео Лендинг воронка')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Landing.Domain.LpFunnelType.VideoLandingFunnel','Video Landing Funnel')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Landing.Domain.LpFunnelType.Booking','Бронирование')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Landing.Domain.LpFunnelType.Booking','Booking')

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
	 @AccrueBonuses bit,
     @Taxid int, 
	 @YandexSizeUnit nvarchar(10),
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
			 ,AccrueBonuses
             ,TaxId		
			 ,YandexSizeUnit	 
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
			 @AccrueBonuses,
             @TaxId,
			 @YandexSizeUnit,
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
	 @AccrueBonuses bit,
	 @TaxId int,
	 @YandexSizeUnit nvarchar(10),
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
	 ,[AccrueBonuses] = @AccrueBonuses
	 ,[TaxId] = @TaxId 
	 ,[YandexSizeUnit] = @YandexSizeUnit
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
			,[Product].YandexSizeUnit
			,[Product].MinAmount
			,[Product].Multiplicity			
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

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.MetaVariables.Product.ProductArtNo','Артикул товара')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.MetaVariables.Product.ProductArtNo','Product sku')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.MetaVariables.Product.OfferArtNo','Артикулы модификаций')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.MetaVariables.Product.OfferArtNo','Offer sku')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.Design.TemplateInstalling','Идёт установка шаблона')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.Design.TemplateInstalling','Installation of the template is in progress')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.Modules.ModuleInstalling','Идёт установка модуля')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.Modules.ModuleInstalling','Installation of the module is in progress')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.Modules.ModuleUpdating','Идёт обновление модуля')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.Modules.ModuleUpdating','Update of the module is in progress')

GO--

update [Settings].[Localization] set  ResourceValue='Стр./корп.' where  [LanguageId] =1 and [ResourceKey] ='Admin.Customers.Customer.Structure'
update [Settings].[Localization] set  ResourceValue='Стр./корп.' where  [LanguageId] =1 and [ResourceKey] ='Admin.Orders.OrderCustomer.Structure'
update [Settings].[Localization] set  ResourceValue='подъезд' where  [LanguageId] =1 and [ResourceKey] ='Admin.Js.CustomerView.Entrance'


GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Product.LeftMenu.AdditionalSales', 'Допродажи')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Product.LeftMenu.AdditionalSales', 'Additional sales')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Product.LandingFunnels.AdditionalSales', 'Допродажи')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Product.LandingFunnels.AdditionalSales', 'Additional sales')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Landing.Domain.LpFunnelType.ProductCrossSellDownSell','Cross-sell и down-sell для продукта')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Landing.Domain.LpFunnelType.ProductCrossSellDownSell','Product cross-sell and down-sell')

GO--

ALTER TABLE CMS.LandingSite ADD
	AdditionalSalesProductId int NULL
GO--

ALTER TABLE CMS.Landing ADD
	ProductId int NULL
GO--

Update [Settings].[Localization] Set [ResourceValue] = 'Видео воронка товара (покупка в 1 клик)' Where [LanguageId] = 1 and [ResourceKey] = 'Landing.Domain.LpFunnelType.VideoLandingFunnel'

Update [Settings].[Localization] Set [ResourceValue] = 'Video product funnel (buy in 1-click)' Where [LanguageId] = 2 and [ResourceKey] = 'Landing.Domain.LpFunnelType.VideoLandingFunnel'


INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Landing.Domain.LpFunnelType.VideoLandingFunnelOrder','Видео воронка товара (оформление заказа)')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Landing.Domain.LpFunnelType.VideoLandingFunnelOrder','Video product funnel (with checkout)')

GO--

INSERT INTO [Settings].[Settings] ([Name], [Value]) VALUES ('SettingsLandingPage.UseCrossSellLandingsInCheckout', 'True')
GO--


INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.Validation.Error.Max','Введенное значение не входит в допустимый диапозон')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.Validation.Error.Max','The entered value is not within the allowed range')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.Validation.Error.Min','Введенное значение не входит в допустимый диапозон')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.Validation.Error.Min','The entered value is not within the allowed range')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ExportFeeed.SettingsYandex.NeedZip','Упаковать в zip архив')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ExportFeeed.SettingsYandex.NeedZip','Zip to file')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Checkout.UseCrossSellLandingsInCheckout','Использовать воронки допродаж в оформлении заказа')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Checkout.UseCrossSellLandingsInCheckout','Enable additional sales funnels in checkout')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Checkout.BuyInOneClickSuccess.Success','Спасибо, ваш заказ оформлен!')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Checkout.BuyInOneClickSuccess.Success','Thank you for order!')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Checkout.BuyInOneClickSuccess.CheckoutTitle','Спасибо, ваш заказ оформлен!')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Checkout.BuyInOneClickSuccess.CheckoutTitle','Thank you for order!')

GO--

ALTER TABLE Booking.Booking ADD
	ManagerId int NULL,
	OrderSourceId int NULL

GO--

IF (EXISTS(Select 1 From [Order].[OrderSource] Where [Type] = 'None' And [Main] = 1))
BEGIN
	UPDATE Booking.Booking SET OrderSourceId = (Select TOP(1) [Id] From [Order].[OrderSource] Where [Type] = 'None' And [Main] = 1)
END
ELSE
BEGIN
	UPDATE Booking.Booking SET OrderSourceId = 0
END

GO--

ALTER TABLE Booking.Booking ALTER COLUMN
	OrderSourceId int NOT NULL

GO--

ALTER TABLE [Booking].[Booking]  WITH CHECK ADD  CONSTRAINT [FK_Booking_Managers] FOREIGN KEY([ManagerId])
REFERENCES [Customers].[Managers] ([ManagerId])
ON DELETE SET NULL

GO--

ALTER TABLE [Booking].[Booking] CHECK CONSTRAINT [FK_Booking_Managers]

GO--

update [Settings].[Localization] set  ResourceValue='Воронки' where  [LanguageId] =1 and [ResourceKey] ='Admin.Settings.SystemSettings.AppsLandingActive'

GO--

CREATE TABLE [Order].[DeferredMail](
	[EntityId] [int] NOT NULL,
	[EntityType] [int] NOT NULL,
	[CreatedDate] [datetime] NOT NULL
) ON [PRIMARY]

GO--

CREATE UNIQUE NONCLUSTERED INDEX [IX_DeferredMail] ON [Order].[DeferredMail] 
(
	[EntityId] ASC,
	[EntityType] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]

GO--


CREATE TABLE [CMS].[LandingEmailTemplate](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[BlockId] [int] NOT NULL,
	[Subject] [nvarchar](max) NOT NULL,
	[Body] [nvarchar](max) NOT NULL,
	[SendingTime] [int] NOT NULL,
 CONSTRAINT [PK_LandingEmailTemplate] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]

GO--

ALTER TABLE [CMS].[LandingEmailTemplate]  WITH CHECK ADD  CONSTRAINT [FK_LandingEmailTemplate_LandingBlock] FOREIGN KEY([BlockId])
REFERENCES [CMS].[LandingBlock] ([Id])
ON DELETE CASCADE

GO--

CREATE TABLE [CMS].[LandingDeferredEmail](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CustomerId] [uniqueidentifier] NOT NULL,
	[Email] [nvarchar](max) NOT NULL,
	[Subject] [nvarchar](max) NOT NULL,
	[Body] [nvarchar](max) NOT NULL,
	[SendingDate] [datetime] NOT NULL,
 CONSTRAINT [PK_LandingDeferredEmail] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO--



update [Settings].[MailFormatType]
	SET [Comment] = 'Письмо при создании брони (#BOOKING_ID#, #NAME#, #PHONE#, #DATE#, #RESERVATIONRESOURCE#, #EMAIL#, #ORDERTABLE#, #STORE_NAME#)'
WHERE [MailType]='OnBookingCreated'

GO--

DECLARE @MailFormatID INT
SELECT TOP(1) @MailFormatID = [MailFormatTypeID] FROM [Settings].[MailFormatType] WHERE [MailType]='OnBookingCreated'
update [Settings].[MailFormat]
     SET [FormatText] = '<div style="color: #4c4f56; font-family: Arial, Helvetica, sans-serif; font-size: 14px;">
<div class="header" style="border-bottom: 1px solid #ededed; display: table; margin-bottom: 25px; padding-bottom: 25px; width: 100%;">
<div class="logo" style="display: table-cell; text-align: left; vertical-align: middle;">#LOGO#</div>

<div class="phone" style="display: table-cell; text-align: right; vertical-align: middle;">
<div class="tel" style="font-size: 26px; font-weight: bold; line-height: 1; margin-bottom: 5px;">&nbsp;</div>

<div class="inform" style="font-size: 12px;">&nbsp;</div>
</div>
</div>

<div class="data" style="display: table; width: 100%;">
<div class="data-cell" style="display: table-cell; padding: 0; padding-right: 1%; width: 48%;">
<div class="l-row">
<div class="l-name vi cs-light" style="color: #acacac; display: inline-block; margin: 5px 0; padding-right: 15px; width: 150px; vertical-align: middle;">Имя:</div>

<div class="l-value vi" style="display: inline-block; margin: 5px 0;">#NAME#</div>
</div>

<div class="l-row">
<div class="l-name vi cs-light" style="color: #acacac; display: inline-block; margin: 5px 0; padding-right: 15px; width: 150px; vertical-align: middle;">Номер телефона:</div>

<div class="l-value vi" style="display: inline-block; margin: 5px 0;">#PHONE#</div>
</div>

<div class="l-row">
<div class="l-name vi cs-light" style="color: #acacac; display: inline-block; margin: 5px 0; padding-right: 15px; width: 150px; vertical-align: middle;">EMail:</div>

<div class="l-value vi" style="display: inline-block; margin: 5px 0;">#EMAIL#</div>
</div>

<div class="l-row">
<div class="l-name vi cs-light" style="color: #acacac; display: inline-block; margin: 5px 0; padding-right: 15px; width: 150px; vertical-align: middle;">Дата:</div>

<div class="l-value vi" style="display: inline-block; margin: 5px 0;">#DATE#</div>
</div>

<div class="l-row">
<div class="l-name vi cs-light" style="color: #acacac; display: inline-block; margin: 5px 0; padding-right: 15px; width: 150px; vertical-align: middle;">Ресурс:</div>

<div class="l-value vi" style="display: inline-block; margin: 5px 0;">#RESERVATIONRESOURCE#</div>
</div>
</div>
</div>

<div>
<div class="o-big-title" style="font-size: 18px; font-weight: bold; margin-bottom: 20px; margin-top: 40px;">Содержание брони:</div>
#ORDERTABLE#</div>
</div>'
WHERE MailFormatTypeId = @MailFormatID

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Users.ChangePassword.CustomerDisabled','Сотрудник не активен')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Users.ChangePassword.CustomerDisabled','Employee is not active')
GO--

ALTER TABLE [Customers].[Managers] ADD  CONSTRAINT [DF_Managers_Active]  DEFAULT ((1)) FOR [Active]
GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.Product.LandingFunnelCreated', 'Создана воронка допродаж')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.Product.LandingFunnelCreated', 'Cross-sell funnel has been created')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Checkout.GetCheckoutPage.ShippingName.WithoutShipping', 'Без доставки')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Checkout.GetCheckoutPage.ShippingName.WithoutShipping', 'Without shipping')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ExportFeeed.SettingsAvito.ExportNotAvailable', 'Выгружать товары, недоступные к покупке (не в наличии, неактивные, без цены)')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ExportFeeed.SettingsAvito.ExportNotAvailable', 'Unload products not available for purchase (not available, inactive, without price)')

GO--


INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Landing.Domain.LpFunnelType.Conference','Конференция, несколько спикеров')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Landing.Domain.LpFunnelType.Conference','Conference, several speakers')

GO--



CREATE TABLE [Booking].[AffiliateSmsTemplate](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[AffiliateId] [int] NOT NULL,
	[Status] [tinyint] NOT NULL,
	[Text] [nvarchar](500) NULL,
	[Enabled] [bit] NOT NULL,
 CONSTRAINT [PK_AffiliateSmsTemplate] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO--

CREATE NONCLUSTERED INDEX [AffiliateStatus_AffiliateSmsTemplate] ON [Booking].[AffiliateSmsTemplate]
(
	[AffiliateId] ASC,
	[Status] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO--

ALTER TABLE [Booking].[AffiliateSmsTemplate]  WITH CHECK ADD  CONSTRAINT [FK_AffiliateSmsTemplate_Affiliate] FOREIGN KEY([AffiliateId])
REFERENCES [Booking].[Affiliate] ([Id])
ON DELETE CASCADE

GO--

ALTER TABLE [Booking].[AffiliateSmsTemplate] CHECK CONSTRAINT [FK_AffiliateSmsTemplate_Affiliate]

GO--

ALTER TABLE Booking.Affiliate ADD
	IsActiveSmsNotification bit NULL,
	ForHowManyMinutesToSendSms int NULL,
	SmsTemplateBeforeStartBooiking nvarchar(500) NULL

GO--

UPDATE Booking.Affiliate SET IsActiveSmsNotification = 0

GO--

ALTER TABLE Booking.Affiliate ALTER COLUMN
	IsActiveSmsNotification bit NOT NULL

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Booking.Settings.SmsNotification','Смс информирование')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Booking.Settings.SmsNotification','SMS Notification')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Booking.Settings.SmsNotification.IsActiveSmsNotification','Активно')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Booking.Settings.SmsNotification.IsActiveSmsNotification','Activity')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Booking.Settings.SmsNotification.ForHowManyMinutesToSendSms','За сколько минут до начала брони отправить Смс')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Booking.Settings.SmsNotification.ForHowManyMinutesToSendSms','For how many minutes before the reservation to send SMS')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Booking.Settings.SmsNotification.SmsTemplate','Шаблон Смс')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Booking.Settings.SmsNotification.SmsTemplate','SMS Template')

GO--

ALTER TABLE Booking.Booking ADD
	IsSendedSmsBeforeStart bit NULL

GO--

UPDATE Booking.Booking SET IsSendedSmsBeforeStart = 0

ALTER TABLE Booking.Booking ALTER COLUMN
	IsSendedSmsBeforeStart bit NOT NULL

GO--

Update [Settings].[Localization] Set [ResourceValue] = 'Мероприятие' Where [LanguageId] = 1 and [ResourceKey] = 'Landing.Domain.LpFunnelType.Events'
Update [Settings].[Localization] Set [ResourceValue] = 'Event' Where [LanguageId] = 2 and [ResourceKey] = 'Landing.Domain.LpFunnelType.Events'

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Landing.Domain.LpFunnelType.Course', 'Курс, один спикер')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Landing.Domain.LpFunnelType.Course', 'Course, one speaker')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Landing.Domain.LpFunnelType.Consulting', 'Консалтинг')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Landing.Domain.LpFunnelType.Consulting', 'Consulting')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Landing.Domain.LpFunnelType.ServicesOnline','Услуга с онлайн-записью')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Landing.Domain.LpFunnelType.ServicesOnline','Services with online recording')

GO--

--UPDATE [Settings].[Settings] SET [Value] = 'True' WHERE [Name] = 'BookingActive'

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Landings.CreateFunnel.Back','Типы воронок')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Landings.CreateFunnel.Back','Types of funnels')

GO--

ALTER TABLE Catalog.Product ADD
	CreatedBy nvarchar(50) NULL

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
	 @AccrueBonuses bit,
	 @TaxId int,
	 @YandexSizeUnit nvarchar(10),
	 @DateModified datetime,
	 @YandexName nvarchar(255),
	 @YandexDeliveryDays nvarchar(5),
	 @CreatedBy nvarchar(50)

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
	 ,[AccrueBonuses] = @AccrueBonuses
	 ,[TaxId] = @TaxId 
	 ,[YandexSizeUnit] = @YandexSizeUnit
	 ,[YandexName] = @YandexName
	 ,[YandexDeliveryDays] = @YandexDeliveryDays
	 ,[CreatedBy] = @CreatedBy
	WHERE ProductID = @ProductID      
END

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
	 @AccrueBonuses bit,
     @Taxid int, 
	 @YandexSizeUnit nvarchar(10),
	 @DateModified datetime,
	 @YandexName nvarchar(255),
	 @YandexDeliveryDays nvarchar(5),
	 @CreatedBy nvarchar(50)
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
			 ,AccrueBonuses
             ,TaxId		
			 ,YandexSizeUnit	 
			 ,YandexName
			 ,YandexDeliveryDays
			 ,CreatedBy
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
			 @AccrueBonuses,
             @TaxId,
			 @YandexSizeUnit,
			 @YandexName,
			 @YandexDeliveryDays,
			 @CreatedBy
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
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Checkout.CheckoutUser.RegistAndGetBonusCard','Зарегистрироваться и получить бонусную карту')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Checkout.CheckoutUser.RegistAndGetBonusCard','Register and get a bonus card')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Checkout.CheckoutUser.RegistAsRegularCustomer','Зарегистрироваться как постоянный покупатель')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Checkout.CheckoutUser.RegistAsRegularCustomer','Sign up as a regular customer')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Checkout.CheckoutUser.RegistAndGetBonusesOnCard','Зарегистрироваться и получить <br /> <b><span class=\"nowrap\">{0}</span> {1}</b> на бонусную карту')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Checkout.CheckoutUser.RegistAndGetBonusesOnCard','Register and get <br /> <b> <span class = \ "nowrap \"> {0} </ span> {1} </ b> on the bonus card')
GO--

Update [Settings].[Localization] Set [ResourceValue] = 'Лид магнит статья' Where [LanguageId] = 1 and [ResourceKey] = 'Landing.Domain.LpFunnelType.LeadMagnet'
Update [Settings].[Localization] Set [ResourceValue] = 'Lead magnet article' Where [LanguageId] = 2 and [ResourceKey] = 'Landing.Domain.LpFunnelType.LeadMagnet'

Update [Settings].[Localization] Set [ResourceValue] = 'Лид магнит видео' Where [LanguageId] = 1 and [ResourceKey] = 'Landing.Domain.LpFunnelType.VideoLeadMagnet'
Update [Settings].[Localization] Set [ResourceValue] = 'Lead magnet video' Where [LanguageId] = 2 and [ResourceKey] = 'Landing.Domain.LpFunnelType.VideoLeadMagnet'

Update [Settings].[Localization] Set [ResourceValue] = 'Товарная видео воронка с заявкой' Where [LanguageId] = 1 and [ResourceKey] = 'Landing.Domain.LpFunnelType.VideoLandingFunnel'
Update [Settings].[Localization] Set [ResourceValue] = 'Product video funnel with request' Where [LanguageId] = 2 and [ResourceKey] = 'Landing.Domain.LpFunnelType.VideoLandingFunnel'

Update [Settings].[Localization] Set [ResourceValue] = 'Товарная видео воронка с оплатой' Where [LanguageId] = 1 and [ResourceKey] = 'Landing.Domain.LpFunnelType.VideoLandingFunnelOrder'
Update [Settings].[Localization] Set [ResourceValue] = 'Product video funnel with checkout' Where [LanguageId] = 2 and [ResourceKey] = 'Landing.Domain.LpFunnelType.VideoLandingFunnelOrder'

Update [Settings].[Localization] Set [ResourceValue] = 'Товарная воронка бесплатно + доставка' Where [LanguageId] = 1 and [ResourceKey] = 'Landing.Domain.LpFunnelType.OneProductUpSellDownSell'
Update [Settings].[Localization] Set [ResourceValue] = 'Product funnel free + delivery' Where [LanguageId] = 2 and [ResourceKey] = 'Landing.Domain.LpFunnelType.OneProductUpSellDownSell'

Update [Settings].[Localization] Set [ResourceValue] = 'Воронка мероприятия' Where [LanguageId] = 1 and [ResourceKey] = 'Landing.Domain.LpFunnelType.Events'
Update [Settings].[Localization] Set [ResourceValue] = 'Event funnel' Where [LanguageId] = 2 and [ResourceKey] = 'Landing.Domain.LpFunnelType.Events'

Update [Settings].[Localization] Set [ResourceValue] = 'Воронка конференции' Where [LanguageId] = 1 and [ResourceKey] = 'Landing.Domain.LpFunnelType.Conference'
Update [Settings].[Localization] Set [ResourceValue] = 'Conference funnel' Where [LanguageId] = 2 and [ResourceKey] = 'Landing.Domain.LpFunnelType.Conference'

Update [Settings].[Localization] Set [ResourceValue] = 'Воронка курса' Where [LanguageId] = 1 and [ResourceKey] = 'Landing.Domain.LpFunnelType.Course'
Update [Settings].[Localization] Set [ResourceValue] = 'Course funnel' Where [LanguageId] = 2 and [ResourceKey] = 'Landing.Domain.LpFunnelType.Course'

Update [Settings].[Localization] Set [ResourceValue] = 'Воронка Услуга с онлайн-записью' Where [LanguageId] = 1 and [ResourceKey] = 'Landing.Domain.LpFunnelType.ServicesOnline'
Update [Settings].[Localization] Set [ResourceValue] = 'Funnel Service with online recording' Where [LanguageId] = 2 and [ResourceKey] = 'Landing.Domain.LpFunnelType.ServicesOnline'

Update [Settings].[Localization] Set [ResourceValue] = 'Воронка Консалтинговая' Where [LanguageId] = 1 and [ResourceKey] = 'Landing.Domain.LpFunnelType.Consulting'
Update [Settings].[Localization] Set [ResourceValue] = 'Consulting funnel' Where [LanguageId] = 2 and [ResourceKey] = 'Landing.Domain.LpFunnelType.Consulting'

Update [Settings].[Localization] Set [ResourceValue] = 'Товарная воронка с заявкой' Where [LanguageId] = 1 and [ResourceKey] = 'Landing.Domain.LpFunnelType.LandingFunnel'
Update [Settings].[Localization] Set [ResourceValue] = 'Product funnel with request' Where [LanguageId] = 2 and [ResourceKey] = 'Landing.Domain.LpFunnelType.LandingFunnel'

Update [Settings].[Localization] Set [ResourceValue] = 'Купонатор воронка с заявкой ' Where [LanguageId] = 1 and [ResourceKey] = 'Landing.Domain.LpFunnelType.Couponator'
Update [Settings].[Localization] Set [ResourceValue] = 'Couponator funnel with request' Where [LanguageId] = 2 and [ResourceKey] = 'Landing.Domain.LpFunnelType.Couponator'

UPDATE [Settings].[Localization] Set [ResourceValue] = 'Зарегистрироваться и получить <br /> <b><span class="nowrap">{0}</span> {1}</b> на бонусную карту' Where [LanguageId] = 1 and [ResourceKey] = 'Checkout.CheckoutUser.RegistAndGetBonusesOnCard'
UPDATE [Settings].[Localization] Set [ResourceValue] = 'Register and get <br /> <b> <span class = "nowrap"> {0} </ span> {1} </ b> on the bonus card' Where [LanguageId] = 2 and [ResourceKey] = 'Checkout.CheckoutUser.RegistAndGetBonusesOnCard'

GO--

ALTER TABLE CMS.LandingForm ADD
	OfferId nvarchar(MAX) NULL
GO--
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Configuration.TemplateSettings_SectionCategoryMenuIcons','Категория. Иконки меню')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Configuration.TemplateSettings_SectionCategoryMenuIcons','Category. Menu icons')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ExportFeed.SettingsAvito.Address','Адрес')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ExportFeed.SettingsAvito.Address','Address')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Landing.Domain.LpFunnelType.LandingFunnelOrder','Товарная воронка с оплатой')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Landing.Domain.LpFunnelType.LandingFunnelOrder','Product funnel with checkout')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Landing.Domain.LpFunnelType.CouponatorOrder','Купонатор воронка с оплатой')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Landing.Domain.LpFunnelType.CouponatorOrder','Couponator funnel with checkout')

GO--

Update Settings.MailFormatType Set Comment = Replace(Comment, ')', '; #MANAGER_NAME# )') where MailType in ('OnPayOrder','OnChangeOrderStatus','OnNewOrder','OnBuyInOneClick','OnChangeUserComment','OnLead','OnBillingLink')

GO--
ALTER PROCEDURE [Catalog].[sp_AddProduct]      
	 @ArtNo nvarchar(100) = '',    
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
	 @AccrueBonuses bit,
     @Taxid int, 
	 @YandexSizeUnit nvarchar(10),
	 @DateModified datetime,
	 @YandexName nvarchar(255),
	 @YandexDeliveryDays nvarchar(5),
	 @CreatedBy nvarchar(50)
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
			 ,AccrueBonuses
             ,TaxId		
			 ,YandexSizeUnit	 
			 ,YandexName
			 ,YandexDeliveryDays
			 ,CreatedBy
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
			 @AccrueBonuses,
             @TaxId,
			 @YandexSizeUnit,
			 @YandexName,
			 @YandexDeliveryDays,
			 @CreatedBy
   );    
    
 SET @ID = SCOPE_IDENTITY();    
 if @ArtNo=''    
 begin    
  set @ArtNo = Convert(nvarchar(100), @ID)     
    
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
	 @ArtNo nvarchar(100),    
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
	 @AccrueBonuses bit,
	 @TaxId int,
	 @YandexSizeUnit nvarchar(10),
	 @DateModified datetime,
	 @YandexName nvarchar(255),
	 @YandexDeliveryDays nvarchar(5),
	 @CreatedBy nvarchar(50)

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
	 ,[AccrueBonuses] = @AccrueBonuses
	 ,[TaxId] = @TaxId 
	 ,[YandexSizeUnit] = @YandexSizeUnit
	 ,[YandexName] = @YandexName
	 ,[YandexDeliveryDays] = @YandexDeliveryDays
	 ,[CreatedBy] = @CreatedBy
	WHERE ProductID = @ProductID      
END


GO--
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.Order.Weight','Вес')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.Order.Weight','Weight')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.Order.Dimensions','Габариты')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.Order.Dimensions','Dimensions')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.OrderItemsSummary.TotalWeight','Общий вес')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.OrderItemsSummary.TotalWeight','Total weight')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.OrderItemsSummary.TotalDemensions','Общие габариты')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.OrderItemsSummary.TotalDemensions','Total demensions')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Js.Order.AreYouWantCancelOrder','Вы уверены что хотите отменить заказ?')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Js.Order.AreYouWantCancelOrder','Are you really want to cancel this order?')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Js.Order.OrderCancel','Отмена заказа')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Js.Order.OrderCancel','Order сancel')


GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Home.Menu.Support','Поддержка')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Home.Menu.Support','Support')

GO--
ALTER TABLE Catalog.Category alter column
	Products_Count int NOT NULL 
	
ALTER TABLE Catalog.Category ADD CONSTRAINT
    DF_Category_Products_Count DEFAULT 0 FOR Products_Count
GO--	
ALTER TABLE Catalog.Category ADD
	Available_Products_Count int NOT NULL CONSTRAINT DF_Category_Available_Products_Count DEFAULT 0
GO--	
ALTER TABLE Catalog.Category ADD
	Current_Products_Count int NOT NULL CONSTRAINT DF_Category_Current_Products_Count DEFAULT 0

GO--
ALTER PROCEDURE [Catalog].[sp_RecalculateProductsCount]
AS
BEGIN
SET NOCOUNT ON 
;WITH cteSort AS
      (
      SELECT [Category].CategoryID AS Child,
			 [Category].ParentCategory AS Parent,
			 1  AS [Level]
        FROM [Catalog].[Category] WHERE CategoryID = 0 
      union ALL
      SELECT 
		 [Category].CategoryID AS Child,
		 [Category].ParentCategory AS Parent,
		 cteSort.[Level] + 1 AS [Level]
      FROM [Catalog].[Category] 
		   INNER JOIN cteSort ON [Category].ParentCategory = cteSort.Child and [Category].CategoryID<>0)

update c set 
   c.Products_Count=isnull(g.Products_Count,0)*c.Enabled, 
   c.Total_Products_Count=isnull(g.Total_Products_Count,0),

   c.Available_Products_Count=isnull(g.Available_Products_Count,0)*c.Enabled,
   c.Current_Products_Count=isnull(g.Current_Products_Count,0)*c.Enabled,

   c.CatLevel =cteSort.[Level]
from [Catalog].Category c
left join (
   select 
      pc.CategoryID, 
      SUM(1*p.Enabled) Products_Count,
      COUNT(*) Total_Products_Count,
	  SUM(1*p.Enabled*pExt.AmountSort) Available_Products_Count,
	  SUM(1*p.Enabled) Current_Products_Count
   from [Catalog].ProductCategories pc 
   inner join [Catalog].Product p on p.ProductID=pc.ProductID
   inner join [Catalog].[ProductExt] pExt on p.ProductID=pExt.ProductID
   --where for some conditions
   group by pc.CategoryID
   )g on g.CategoryID=c.CategoryID
left join cteSort on cteSort.Child = c.[CategoryID]

declare @max int
set @max = (select top(1) CatLevel from [Catalog].[Category] order by CatLevel  Desc)
 while (@max >0)
 begin
     UPDATE t1
		SET t1.Products_Count = t1.Products_Count + t2.pc,
		t1.Total_Products_Count = t1.Total_Products_Count + t2.tpc,

		t1.Available_Products_Count = t1.Available_Products_Count + t2.apc
		--t1.Current_Products_Count = t1.Current_Products_Count + t2.atpc

		from [Catalog].[Category] as t1 
		cross apply (Select COALESCE(SUM(Products_Count),0) pc,
							COALESCE(SUM(Total_Products_Count),0) tpc,
							COALESCE(SUM(Available_Products_Count),0) apc
							--COALESCE(SUM(Avalible_Total_Products_Count),0) atpc
							from [Catalog].[Category] where ParentCategory =t1.CategoryID) t2
							where t1.CategoryID in (Select CategoryID from [Catalog].[Category] where CatLevel =@max)
     Set @max = @max -1
 end
END
GO--
exec [Catalog].[sp_RecalculateProductsCount]
GO--
ALTER PROCEDURE [Catalog].[sp_GetChildCategoriesByParentIDForMenu]
	@CurrentCategoryID int
AS
BEGIN
	
	IF (Select COUNT(CategoryID) From Catalog.Category Where ParentCategory = @CurrentCategoryID and Enabled = 1) > 0 
		BEGIN
			SELECT 
				[CategoryID],
				[Name],
				[Description],
				[BriefDescription],
				[ParentCategory],
				--[Picture],
				[Products_Count],
				[Total_Products_Count],
				Available_Products_Count,
				[SortOrder],
				[Enabled],
				[Hidden],
				--[MiniPicture],
				[DisplayStyle],	
				[DisplayBrandsInMenu],
				[DisplaySubCategoriesInMenu],
				[UrlPath],
				(SELECT Count(CategoryID) FROM [Catalog].[Category] AS c WHERE c.ParentCategory = cat.CategoryID and Enabled = 1) AS [ChildCategories_Count] 
			FROM [Catalog].[Category] AS cat 
			WHERE [ParentCategory] = @CurrentCategoryID AND CategoryID <> 0 and Enabled = 1 AND HirecalEnabled = 1
			ORDER BY SortOrder, Name
		END
	ELSE
		BEGIN
			SELECT 
				[CategoryID],
				[Name],
				[Description],
				[BriefDescription],
				[ParentCategory],
				--[Picture],
				[Products_Count],
				[Total_Products_Count],
				Available_Products_Count,
				[SortOrder],
				[Enabled],
				[Hidden],
				--[MiniPicture],
				[DisplayStyle],	
				[DisplayBrandsInMenu],
				[DisplaySubCategoriesInMenu],				
				[UrlPath],
				(SELECT Count(CategoryID) FROM [Catalog].[Category] AS c WHERE c.ParentCategory = cat.CategoryID and Enabled = 1) AS [ChildCategories_Count] 
			FROM [Catalog].[Category] AS cat WHERE [ParentCategory] = (Select ParentCategory From Catalog.Category 
			Where CategoryID = @CurrentCategoryID) AND CategoryID <> 0 and Enabled = 1 AND HirecalEnabled = 1
			ORDER BY SortOrder, Name
		END

END

GO--

UPDATE [Settings].[MailFormatType]
   SET [Comment] = 'Письмо покупателю (#FIRSTNAME#, #LASTNAME#, #PATRONYMIC#, #TRACKNUMBER#, #STORE_NAME#, #MANAGER_NAME#)'
 WHERE [MailType] = 'OnSendToCustomer'

GO--

update [Settings].[Localization] set  [ResourceValue]='Шаблон письма ответа (#FIRSTNAME#, #LASTNAME#, #PATRONYMIC#, #STORE_NAME#, #MANAGER_NAME#)' where  [LanguageId] =1 and [ResourceKey] ='Admin.Js.AddEditMailAnswerTemplate.Template'
update [Settings].[Localization] set  [ResourceValue]='Response mail template (#FIRSTNAME#, #LASTNAME#, #PATRONYMIC#, #STORE_NAME#, #MANAGER_NAME#)' where  [LanguageId] =2 and [ResourceKey] ='Admin.Js.AddEditMailAnswerTemplate.Template'

GO--

Update [Settings].[Localization] Set [ResourceValue] = 'Купонатор воронка с оплатой ' Where [LanguageId] = 1 and [ResourceKey] = 'Landing.Domain.LpFunnelType.CouponatorOrder'

GO--

Update   Settings.MailFormatType Set Comment = Replace(Comment,')', ',#ADDITIONALCUSTOMERFIELDS#)') Where MailType = 'OnNewOrder'

GO--

CREATE NONCLUSTERED INDEX [CreateOn_Card] ON [Bonus].[Card]
(
	[CreateOn] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO--

CREATE NONCLUSTERED INDEX [DateLastWipeBonus_Card] ON [Bonus].[Card]
(
	[DateLastWipeBonus] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO--

Update Settings.Localization Set ResourceValue = Replace(ResourceValue,'https://www.advantshop.net/help/pages/poisk-izobrazhenii-v-internete','https://www.advantshop.net/help/pages/poisk-izobrazhenii-advantshop') WHERE ResourceKey ='Admin.Settings.SystemSettings.ShowImageSearchEnabledNote'

GO--


INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Social.WidgetFacebookActive','Facebook')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Social.WidgetFacebookActive','Facebook')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Social.WidgetInstagrammActive','Instagram')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Social.WidgetInstagrammActive','Instagram')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Social.WidgetOkActive','Одноклассники')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Social.WidgetOkActive','Одноклассники')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Social.WidgetTelegramActive','Telegram')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Social.WidgetTelegramActive','Telegram')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Social.WidgetTwitterActive','Twitter')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Social.WidgetTwitterActive','Twitter')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Social.WidgetVkActive','Вконтакте')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Social.WidgetVkActive','Vkontakte')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Social.WidgetYoutubeActive','Youtube')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Social.WidgetYoutubeActive','Youtube')

GO--

Update Settings.MailFormatType Set Comment = Replace(Comment, ')', ',#INN#,#COMPANYNAME# )') where MailType = 'OnNewOrder'

GO--

UPDATE [Settings].[InternalSettings] SET [settingValue] = '6.5.5' WHERE [settingKey] = 'db_version'

