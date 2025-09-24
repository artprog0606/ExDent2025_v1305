

ALTER PROCEDURE [Customers].[sp_GetRecentlyView]
	@CustomerId uniqueidentifier,
	@rowsCount int,
	@Type nvarchar(50)
AS
BEGIN
	SET NOCOUNT ON;
	
	IF Exists (SELECT 1 FROM [Customers].[RecentlyViewsData] WHERE CustomerID=@CustomerId)
	Begin
		SELECT TOP(@rowsCount) Product.ProductID, Product.Name, Product.UrlPath, Product.AllowPreOrder, Ratio, ManualRatio, isnull(PhotoNameSize1, PhotoName) as PhotoName,
				[Photo].[Description] as PhotoDescription, Discount, DiscountAmount, MinPrice as BasePrice, CurrencyValue, 
				Offer.OfferID, MaxAvailable AS Amount, MinAmount, MaxAmount, Offer.Amount AS AmountOffer, Colors, NotSamePrices as MultiPrices
		
		FROM [Customers].RecentlyViewsData
		
		Inner Join [Catalog].Product ON Product.ProductID = RecentlyViewsData.ProductId
		Left Join [Catalog].[ProductExt] ON [Product].[ProductID] = [ProductExt].[ProductID]
		Inner Join Catalog.Currency On Currency.CurrencyID = Product.CurrencyID
		Left Join [Catalog].[Photo] ON [Photo].[PhotoId] = [ProductExt].[PhotoId]
		Left Join [Catalog].[Offer] ON [ProductExt].[OfferID] = [Offer].[OfferID]

		WHERE RecentlyViewsData.CustomerID = @CustomerId AND Product.Enabled = 1 And CategoryEnabled = 1
		
		ORDER BY ViewDate Desc
	End
END

GO--



ALTER PROCEDURE [Order].[sp_DecrementProductsCountAccordingOrder]   
 @orderId int  
AS
BEGIN
UPDATE offer
SET offer.Amount = Round(offer.Amount - orderItem.Amount + orderItem.DecrementedAmount, 4)
    FROM Catalog.Offer offer  
        JOIN (SELECT orderItems.ArtNo, SUM(orderItems.Amount) as Amount, SUM(orderItems.DecrementedAmount) as DecrementedAmount
          FROM [Order].[OrderItems] orderItems
          WHERE orderItems.OrderID = @orderId AND orderItems.TypeItem = 'Product'
          GROUP BY orderItems.ArtNo) orderItem
        ON offer.Artno = orderItem.ArtNo;


UPDATE [Order].[OrderItems]
SET decrementedAmount = amount
WHERE [OrderID] = @orderId AND TypeItem = 'Product'

UPDATE [Order].[Order]
SET [Decremented] = 1
WHERE [OrderID] = @orderId

END

GO--

SET IDENTITY_INSERT [Order].[ShippingReplaceGeo] ON 

IF NOT EXISTS (SELECT * FROM [Order].[ShippingReplaceGeo] WHERE [Id] = 33)
INSERT INTO [Order].[ShippingReplaceGeo] ([Id],[ShippingType],[InCountryName],[InCountryISO2],[InRegionName],[InCityName],[InDistrict],[OutCountryName],[OutRegionName],[OutCityName],[OutDistrict],[OutDistrictClear],[Enabled],[Sort],[InZip],[OutZip],[Comment])
VALUES (33, 'Boxberry','Казахстан','KZ','Алматинская область','Алматы','','','','Алма-Ата','',0,1,0,'','','')

IF NOT EXISTS (SELECT * FROM [Order].[ShippingReplaceGeo] WHERE [Id] = 34)
INSERT INTO [Order].[ShippingReplaceGeo] ([Id],[ShippingType],[InCountryName],[InCountryISO2],[InRegionName],[InCityName],[InDistrict],[OutCountryName],[OutRegionName],[OutCityName],[OutDistrict],[OutDistrictClear],[Enabled],[Sort],[InZip],[OutZip],[Comment])
VALUES (34, 'Boxberry','Казахстан','KZ','','Нур-Султан','','','Акмолинская область','','',0,1,0,'','','')

IF NOT EXISTS (SELECT * FROM [Order].[ShippingReplaceGeo] WHERE [Id] = 35)
INSERT INTO [Order].[ShippingReplaceGeo] ([Id],[ShippingType],[InCountryName],[InCountryISO2],[InRegionName],[InCityName],[InDistrict],[OutCountryName],[OutRegionName],[OutCityName],[OutDistrict],[OutDistrictClear],[Enabled],[Sort],[InZip],[OutZip],[Comment])
VALUES (35, 'Boxberry','Казахстан','KZ','Актюбинская область','Актюбинск','','','','Актобе','',0,1,0,'','','')

IF NOT EXISTS (SELECT * FROM [Order].[ShippingReplaceGeo] WHERE [Id] = 36)
INSERT INTO [Order].[ShippingReplaceGeo] ([Id],[ShippingType],[InCountryName],[InCountryISO2],[InRegionName],[InCityName],[InDistrict],[OutCountryName],[OutRegionName],[OutCityName],[OutDistrict],[OutDistrictClear],[Enabled],[Sort],[InZip],[OutZip],[Comment])
VALUES (36, 'Boxberry','Беларусь','BY','Могилёвская область','Могилёв','','','Могилевская область','Могилев','',0,1,10,'','','')

IF NOT EXISTS (SELECT * FROM [Order].[ShippingReplaceGeo] WHERE [Id] = 37)
INSERT INTO [Order].[ShippingReplaceGeo] ([Id],[ShippingType],[InCountryName],[InCountryISO2],[InRegionName],[InCityName],[InDistrict],[OutCountryName],[OutRegionName],[OutCityName],[OutDistrict],[OutDistrictClear],[Enabled],[Sort],[InZip],[OutZip],[Comment])
VALUES (37, 'Boxberry','Беларусь','BY','Могилёвская область','','','','Могилевская область','','',0,1,0,'','','')

SET IDENTITY_INSERT [Order].[ShippingReplaceGeo] OFF

GO--

declare @KyrgyzstanId int = (SELECT TOP 1 [CountryID] FROM [Customers].[Country] WHERE [CountryISO3] = 'KGZ')

IF (@KyrgyzstanId IS NOT NULL)
BEGIN

	declare @ChuyId int = (SELECT TOP 1 [RegionID] FROM [Customers].[Region] WHERE [RegionName] = 'Чуйская область' AND [CountryID] = @KyrgyzstanId)
	IF (@ChuyId IS NULL)
	BEGIN
		INSERT INTO [Customers].[Region] ([CountryID],[RegionName],[RegionCode],[RegionSort])
		VALUES (@KyrgyzstanId,'Чуйская область','',0)
		
		SET @ChuyId = (SELECT SCOPE_IDENTITY())
	END

	declare @KyrgyzstanRegionId int = (SELECT TOP 1 [RegionID] FROM [Customers].[Region] WHERE [RegionName] = 'Киргизия' AND [CountryID] = @KyrgyzstanId)
	IF (@KyrgyzstanRegionId IS NOT NULL)
	BEGIN
		declare @BishkekId int = (SELECT TOP 1 [CityID] FROM [Customers].[City] WHERE [CityName] = 'Бишкек' AND [RegionID] = @KyrgyzstanRegionId)
		IF (@BishkekId IS NOT NULL)
		BEGIN
			UPDATE [Customers].[City]
			   SET [RegionID] = @ChuyId
			WHERE [CityID] = @BishkekId
		END

		declare @KantId int = (SELECT TOP 1 [CityID] FROM [Customers].[City] WHERE [CityName] = 'Кант' AND [RegionID] = @KyrgyzstanRegionId)

		IF (@KantId IS NOT NULL)
		BEGIN
			UPDATE [Customers].[City]
			   SET [RegionID] = @ChuyId
			WHERE [CityID] = @KantId
		END

		declare @OshRegionId int = (SELECT TOP 1 [RegionID] FROM [Customers].[Region] WHERE [RegionName] = 'Ошская обл.' AND [CountryID] = @KyrgyzstanId)

		IF (@OshRegionId IS NOT NULL)
		BEGIN
			UPDATE [Customers].[Region]
			   SET [RegionName] = 'Ошская область'
			WHERE [RegionID] = @OshRegionId
		END

		declare @OshId int = (SELECT TOP 1 [CityID] FROM [Customers].[City] WHERE [CityName] = 'Ош' AND [RegionID] = @KyrgyzstanRegionId)
		IF (@OshId IS NOT NULL)
		BEGIN
			DELETE FROM [Customers].[City] WHERE [CityID]=@OshId
		END

		IF (@OshRegionId IS NOT NULL)
		BEGIN
			SET @OshId = (SELECT TOP 1 [CityID] FROM [Customers].[City] WHERE [CityName] = 'Ош' AND [RegionID] = @OshRegionId)
			IF (@OshId IS NULL)
			BEGIN
				INSERT INTO [Customers].[City] ([RegionID],[CityName],[CitySort],[DisplayInPopup],[PhoneNumber],[MobilePhoneNumber],[Zip],[District])
				VALUES (@OshRegionId, 'Ош', 0, 0, '', '', '', '')
			END
		END

		declare @IssykKulId int = (SELECT TOP 1 [RegionID] FROM [Customers].[Region] WHERE [RegionName] = 'Иссык-Кульская обл.' AND [CountryID] = @KyrgyzstanId)

		IF (@IssykKulId IS NOT NULL)
		BEGIN
			UPDATE [Customers].[Region]
			   SET [RegionName] = 'Иссык-Кульская область'
			WHERE [RegionID] = @IssykKulId
		END

		declare @KarakolId int = (SELECT TOP 1 [CityID] FROM [Customers].[City] WHERE [CityName] = 'Каракол' AND [RegionID] = @KyrgyzstanRegionId)

		IF (@KarakolId IS NOT NULL)
		BEGIN
			UPDATE [Customers].[City]
			   SET [RegionID] = @IssykKulId
			WHERE [CityID] = @KarakolId
		END

		declare @JalalAbadRegionId int = (SELECT TOP 1 [RegionID] FROM [Customers].[Region] WHERE [RegionName] = 'Джалал-Абадская область' AND [CountryID] = @KyrgyzstanId)
		IF (@JalalAbadRegionId IS NULL)
		BEGIN
			INSERT INTO [Customers].[Region] ([CountryID],[RegionName],[RegionCode],[RegionSort])
			VALUES (@KyrgyzstanId,'Джалал-Абадская область','',0)
		
			SET @JalalAbadRegionId = (SELECT SCOPE_IDENTITY())
		END

		declare @JalalAbadId int = (SELECT TOP 1 [CityID] FROM [Customers].[City] WHERE [CityName] = 'Джалал-Абад' AND [RegionID] = @OshRegionId)
		IF (@JalalAbadId IS NOT NULL)
		BEGIN
			UPDATE [Customers].[City]
			   SET [RegionID] = @JalalAbadRegionId
				  ,[DisplayInPopup] = 1
			WHERE [CityID] = @JalalAbadId
		END

	
		declare @NarynRegionId int = (SELECT TOP 1 [RegionID] FROM [Customers].[Region] WHERE [RegionName] = 'Нарынская обл.' AND [CountryID] = @KyrgyzstanId)

		IF (@NarynRegionId IS NOT NULL)
		BEGIN
			UPDATE [Customers].[Region]
			   SET [RegionName] = 'Нарынская область'
			WHERE [RegionID] = @NarynRegionId
		END

		declare @NarynId int = (SELECT TOP 1 [CityID] FROM [Customers].[City] WHERE [CityName] = 'Нарын' AND [RegionID] = @KyrgyzstanRegionId)
		DELETE FROM [Customers].[City] WHERE [CityID]=@NarynId
	
		IF (@NarynRegionId IS NOT NULL)
		BEGIN
			SET @NarynId = (SELECT TOP 1 [CityID] FROM [Customers].[City] WHERE [CityName] = 'Нарын' AND [RegionID] = @NarynRegionId)
			IF (@NarynId IS NULL)
			BEGIN
				INSERT INTO [Customers].[City] ([RegionID],[CityName],[CitySort],[DisplayInPopup],[PhoneNumber],[MobilePhoneNumber],[Zip],[District])
				VALUES (@NarynRegionId, 'Нарын', 0, 1, '', '', '', '')
			END
		END

		declare @BelovodskoyeId int = (SELECT TOP 1 [CityID] FROM [Customers].[City] WHERE [CityName] = 'Беловодское' AND [RegionID] = @KyrgyzstanRegionId)

		IF (@BelovodskoyeId IS NOT NULL)
		BEGIN
			UPDATE [Customers].[City]
			   SET [RegionID] = @ChuyId
			WHERE [CityID] = @BelovodskoyeId
		END

		declare @KaraBaltaId int = (SELECT TOP 1 [CityID] FROM [Customers].[City] WHERE [CityName] = 'Кара-Балта' AND [RegionID] = @KyrgyzstanRegionId)

		IF (@KaraBaltaId IS NOT NULL)
		BEGIN
			UPDATE [Customers].[City]
			   SET [RegionID] = @ChuyId
			WHERE [CityID] = @KaraBaltaId
		END

		declare @SokulukId int = (SELECT TOP 1 [CityID] FROM [Customers].[City] WHERE [CityName] = 'Сокулук' AND [RegionID] = @KyrgyzstanRegionId)

		IF (@SokulukId IS NOT NULL)
		BEGIN
			UPDATE [Customers].[City]
			   SET [RegionID] = @ChuyId
			WHERE [CityID] = @SokulukId
		END
	END
END

GO--

UPDATE [Order].[ShippingReplaceGeo]
   SET [Enabled] = 0
WHERE [Id] = 32

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue])
VALUES
    (1,'Js.Order.BonusesToCard', 'Бонусов зачислено на карту:'),
    (2,'Js.Order.BonusesToCard', 'Bonuses credited to card:'),
    (1,'Js.Order.BonusesToCardAfterPayment', 'Бонусов будет начислено на карту<br> после оплаты заказа:'),
    (2,'Js.Order.BonusesToCardAfterPayment', 'Bonus will be credited to the card<br> after payment of the order:')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
    VALUES
           (1,'Common.BottomPanel.Main', 'Главная'),
           (2,'Common.BottomPanel.Main', 'Main'),
           (1,'Common.BottomPanel.Catalog', 'Каталог'),
           (2,'Common.BottomPanel.Catalog', 'Catalog'),           
           (1,'Common.BottomPanel.Cart', 'Корзина'),
           (2,'Common.BottomPanel.Cart', 'Cart'),           
           (1,'Common.BottomPanel.WishList', 'Избранное'),
           (2,'Common.BottomPanel.WishList', 'Wish list'),           
           (1,'Common.BottomPanel.Account', 'Кабинет'),
           (2,'Common.BottomPanel.Account', 'Account'),           
           (1,'Common.BottomPanel.SignIn', 'Войти'),
           (2,'Common.BottomPanel.SignIn', 'Sign in')

GO--

UPDATE [Settings].[Localization] SET [ResourceValue] = N'Выбираете действие, которое произойдет, если клиент воспользуется формой обратной связи на сайте.<br/><br/>  Подробнее: <br/><a href ="https://www.advantshop.net/help/pages/otpravit-soobschenie" target="_blank">Форма обратной связи.</a>' WHERE [ResourceKey] = 'Admin.Settings.Feedback.FeedbackActionHint' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'Choose the action that will happen if the client uses the feedback form on the site.<br/><br/> More details: <br/><a href ="https://www.advantshop.net/help/pages/otpravit-soobschenie" target="_blank">Feedback form.</a>' WHERE [ResourceKey] = 'Admin.Settings.Feedback.FeedbackActionHint' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Индекс поиска необходим, чтоб собрать информацию о товарах и обеспечить быстрый и точный поиск информации<br/><br/>Подробнее:<br/><a href ="https://www.advantshop.net/help/pages/search#5" target="_blank">Индекс поиска</a>' WHERE [ResourceKey] = 'Admin.Settings.Catalog.SearchIndexHint' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'A search index is needed to collect information about products and provide a quick and accurate search for information.<br/> <br/> More details: <br/> <a href = "https://www.advantshop.net/help/pages/search # 5 "target ="_blank">Search index</a>' WHERE [ResourceKey] = 'Admin.Settings.Catalog.SearchIndexHint' AND [LanguageId] = 2

GO--



UPDATE [Settings].[InternalSettings] SET [settingValue] = '10.0.14' WHERE [settingKey] = 'db_version'