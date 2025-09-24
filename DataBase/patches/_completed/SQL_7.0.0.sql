
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Bonuses.GetBonusCard.RegisterAndGetBonusCard', 'Зарегистрироваться и получить бонусную карту')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Bonuses.GetBonusCard.RegisterAndGetBonusCard', 'Sign up and get a bonus card')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Bonuses.GetBonusCard.GetBonusCard', 'Получить бонусную карту')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Bonuses.GetBonusCard.GetBonusCard', 'Get a bonus card')

GO--

Update [Settings].[Localization] Set [ResourceValue] = 'SEO и счетчики' Where [LanguageId] = 1 and [ResourceKey] = 'Admin.Home.Menu.SettingsSeo'
Update [Settings].[Localization] Set [ResourceValue] = 'SEO and counters' Where [LanguageId] = 2 and [ResourceKey] = 'Admin.Home.Menu.SettingsSeo'

Update [Settings].[Localization] Set [ResourceValue] = 'Видеоурок о Визуальном редакторе' Where [LanguageId] = 1 and [ResourceKey] = 'Admin.News.AddEdit.VideoTutorialVisualEditor'
 
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.Calls.Customer', 'Контакт')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.Calls.Customer', 'Contact')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.Calls.EmptyCustomerName', 'Перейти к покупателю')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.Calls.EmptyCustomerName', 'View customer')

GO--



  
ALTER PROCEDURE [Catalog].[PreCalcProductParams] 
	@productId INT,  
    @ModerateReviews BIT,  
    @OnlyAvailable BIT  
AS  
 BEGIN  
	 SET NOCOUNT ON;  
	 DECLARE @CountPhoto INT;  
	 DECLARE @Type NVARCHAR(10);  
	 DECLARE @PhotoId INT;  
	 DECLARE @MaxAvailable FLOAT;  
	 DECLARE @VideosAvailable BIT;  
	 DECLARE @Colors NVARCHAR(MAX);  
	 DECLARE @NotSamePrices BIT;  
	 DECLARE @MinPrice FLOAT;  
	 DECLARE @PriceTemp FLOAT;  
	 DECLARE @AmountSort BIT;  
	 DECLARE @OfferId INT;  
	 DECLARE @Comments INT;  
	 DECLARE @CategoryId INT;  
	 DECLARE @Gifts BIT;  
	 IF NOT EXISTS  
	 (  
		 SELECT ProductID  
		 FROM [Catalog].Product  
		 WHERE ProductID = @productId  
	 )  
		 RETURN;  
	 SET @Type = 'Product';        
	 --@CountPhoto        
	 SET @CountPhoto =  
	 (  
		 SELECT TOP (1) CASE  
							WHEN  
		 (  
			 SELECT Offer.ColorID  
			 FROM [Catalog].[Offer]  
			 WHERE [ProductID] = @productId  
				   AND main = 1  
		 ) IS NOT NULL  
							THEN  
		 (  
			 SELECT COUNT(DISTINCT PhotoId)  
			 FROM [Catalog].[Photo]  
				  INNER JOIN [Catalog].[Offer] ON [Photo].ColorID = Offer.ColorID OR [Photo].ColorID is NULL  
			 WHERE [Photo].[ObjId] = Offer.[ProductId]  
				   AND [Offer].Main = 1  
				   AND TYPE = @Type  
				   AND Offer.[ProductId] = @productId  
		 )  
							ELSE  
		 (  
			 SELECT COUNT(PhotoId)  
			 FROM [Catalog].[Photo]  
			 WHERE [Photo].[ObjId] = @productId  
				   AND TYPE = @Type  
		 )  
						END  
	 );        
	 --@PhotoId        
	 SET @PhotoId =  
	 (  
		 SELECT CASE  
					WHEN    



(  
			 SELECT Offer.ColorID  
			 FROM [Catalog].[Offer]  
			 WHERE [ProductID] = @productId  
				   AND main = 1  
		 ) IS NOT NULL  
					THEN  
		 (  
			 SELECT TOP (1) PhotoId  
			 FROM [Catalog].[Photo]  
 INNER JOIN [Catalog].[Offer] ON [Photo].ColorID = Offer.ColorID OR [Photo].ColorID is NULL  
			 WHERE([Photo].ColorID = Offer.ColorID  
				   OR [Photo].ColorID IS NULL)  
				  AND [Photo].[ObjId] = @productId  
				  AND Type = @Type  
			 ORDER BY [Photo]. main DESC,  
					  [Photo].[PhotoSortOrder],  
					  [PhotoId]  
		 )  
					ELSE  
		 (  
			 SELECT TOP (1) PhotoId  
			 FROM [Catalog].[Photo]  
			 WHERE [Photo].[ObjId] = @productId  
				   AND Type = @Type  
			 ORDER BY main DESC,  
					  [Photo].[PhotoSortOrder],  
					  [PhotoId]  
		 )  
		 END  



	 );         
	 --VideosAvailable        
	 IF  
	 (  
		 SELECT COUNT(ProductVideoID)  
		 FROM [Catalog].[ProductVideo]  
		 WHERE ProductID = @productId  
	 ) > 0  
		 BEGIN  
			 SET @VideosAvailable = 1;  
		 END;  
	 ELSE  
		 BEGIN  
			 SET @VideosAvailable = 0;  
		 END;        
	
	 --@MaxAvailable        
	 SET @MaxAvailable =  
	 (  
		 SELECT MAX(Offer.Amount)  
		 FROM [Catalog].Offer  
		 WHERE ProductId = @productId  
	 );     

	 --AmountSort        
	 SET @AmountSort =  
	 (  
		 SELECT CASE  
					WHEN @MaxAvailable <= 0  
						 OR @MaxAvailable < ISNULL(Product.MinAmount, 0)  
					THEN 0  
					ELSE 1  
				END  
		 FROM [Catalog].Offer  
			  INNER JOIN [Catalog].Product ON Product.ProductId = Offer.ProductId  
		 WHERE Offer.ProductId = @productId  
			   AND main = 1  
	 );        
	 --Colors        
	 SET @Colors =  
	 (  
		 SELECT [Settings].[ProductColorsToString](@productId)  
	 );        
	
	 --@NotSamePrices        
	 IF  
	 (  
		 SELECT MAX(price) - MIN(price)  
		 FROM [Catalog].offer  
		 WHERE offer.productid = @productId  
			   AND price > 0 AND (@OnlyAvailable = 0  

					OR amount > 0)  
	 ) > 0  
		 BEGIN  
			 SET @NotSamePrices = 1;  
		 END;  
	 ELSE  
		 BEGIN  
			 SET @NotSamePrices = 0;  
		 END;        

	 --@MinPrice        
	 SET @MinPrice =  
	 (  
		 SELECT MIN(price)  
		 FROM CATALOG.offer  
		 WHERE offer.productid = @productId  
			   AND price > 0 AND (@OnlyAvailable = 0  

					OR amount > 0)  
	 );        

	 --@OfferId      
	 SET @OfferId =  
	 (  
		 SELECT OfferID  
		 FROM CATALOG.offer  
		 WHERE offer.productid = @productId  
			   AND (offer.Main = 1  
					OR offer.Main IS NULL)  
	 );       


	 --@PriceTemp        
	 SET @PriceTemp =
         (
             SELECT CASE WHEN [Product].Discount > 0 THEN (@MinPrice - @MinPrice * [Product].Discount / 100) * CurrencyValue ELSE @MinPrice - isnull([Product].DiscountAmount, 0) END
             FROM Catalog.Product
                  INNER JOIN Catalog.Currency ON Product.Currencyid = Currency.Currencyid
             WHERE Product.Productid = @productId
         );         

	 --@Comments      
	 SET @Comments =  
	 (  
		 SELECT COUNT(ReviewId)  
		 FROM CMS.Review  
		 WHERE EntityId = @productId  
			   AND (Checked = 1  
					OR @ModerateReviews = 0)  
	 );       
	 
	 --@Gifts      
	 SET @Gifts =  
	 (  
		 SELECT TOP (1) CASE  
							WHEN COUNT([ProductGifts].ProductID) > 0  
							THEN 1  
							ELSE 0  
						END  
		 FROM [Catalog].[ProductGifts]
			INNER JOIN Catalog.Offer on ProductGifts.GiftOfferId = Offer.OfferId
			INNER JOIN Catalog.Product on Offer.ProductId = Product.ProductId
		 WHERE [ProductGifts].ProductID = @productId  and Offer.Amount > ISNULL(Product.MinAmount, 0) and Enabled = 1
	 );      
   
	 --@CategoryId      
	 DECLARE @MainCategoryId INT;  
	 SET @MainCategoryId =  
	 (  
		 SELECT TOP 1 CategoryID  
		 FROM [Catalog].ProductCategories  
		 WHERE ProductID = @productId  
		 ORDER BY Main DESC  
	 );  
	 IF @MainCategoryId IS NOT NULL  
		 BEGIN  
			 SET @CategoryId =  
			 (  
				 SELECT TOP 1 id  
				 FROM [Settings].[GetParentsCategoryByChild](@MainCategoryId)  
				 ORDER BY sort DESC  
			 );  
		 END;  
	 IF  
	 (  
		 SELECT COUNT(productid)  
		 FROM [Catalog].ProductExt  
		 WHERE productid = @productId  
	 ) > 0  
		 BEGIN  
			 UPDATE [Catalog].[ProductExt]  
			   SET  
				   [CountPhoto] = @CountPhoto,  
				   [PhotoId] = @PhotoId,  
				   [VideosAvailable] = @VideosAvailable,  
				   [MaxAvailable] = @MaxAvailable,  
				   [NotSamePrices] = @NotSamePrices,  
				   [MinPrice] = @MinPrice,  
				   [Colors] = @Colors,  
				   [AmountSort] = @AmountSort,  
				   [OfferId] = @OfferId,  
				   [Comments] = @Comments,  
				   [CategoryId] = @CategoryId,  
				   [PriceTemp] = @PriceTemp,  
				   [Gifts] = @Gifts  
			 WHERE [ProductId] = @productId;  
		 END;  
	 ELSE  
		 BEGIN  
			 INSERT INTO [Catalog].[ProductExt]  
			 ([ProductId],  
			  [CountPhoto],  
			  [PhotoId],  
			  [VideosAvailable],  
			  [MaxAvailable],  
			  [NotSamePrices],  
			  [MinPrice],  
			  [Colors],  
			  [AmountSort],  
			  [OfferId],  
			  [Comments],  
			  [CategoryId],  
			  [PriceTemp],  
			  [Gifts]  
			 )  
			 VALUES  
			 (@productId,  
			  @CountPhoto,  
			  @PhotoId,  
			  @VideosAvailable,  
			  @MaxAvailable,  
			  @NotSamePrices,  
			  @MinPrice,  
			  @Colors,  
			  @AmountSort,  
			  @OfferId,  
			  @Comments,  
			  @CategoryId,  
			  @PriceTemp,  
			  @Gifts  
			 );  
		 END;  
 END;    
GO--

  
ALTER PROCEDURE [Catalog].[PreCalcProductParamsMass] @ModerateReviews BIT, @OnlyAvailable bit AS 
BEGIN
   
INSERT INTO
   [Catalog].[ProductExt] (ProductId, CountPhoto, PhotoId, VideosAvailable, MaxAvailable, NotSamePrices, MinPrice, Colors, AmountSort, OfferId, Comments, CategoryId) (
   SELECT
      ProductId, 0, NULL, 0, 0, 0, 0, NULL, 0, NULL, 0, NULL 
   FROM
      [Catalog].Product 
   WHERE
      Product.ProductId NOT IN 
      (
         SELECT
            ProductId 
         FROM
            [Catalog].[ProductExt]
      )
) 
      UPDATE
         catalog.ProductExt 
      SET
         ProductExt.[CountPhoto] = tempTable.CountPhoto,
         ProductExt.[PhotoId] = tempTable.[PhotoId],
         ProductExt.[VideosAvailable] = tempTable.[VideosAvailable],
         ProductExt.[MaxAvailable] = tempTable.[MaxAvailable],
         ProductExt.[NotSamePrices] = tempTable.[NotSamePrices],
         ProductExt.[MinPrice] = tempTable.[MinPrice],
         ProductExt.[OfferId] = tempTable.[OfferId],
         ProductExt.[Comments] = tempTable.[Comments],
         ProductExt.[Gifts] = tempTable.[Gifts],
		 ProductExt.[Colors] = tempTable.[Colors],
         ProductExt.[CategoryId] = tempTable.[CategoryId] ,
		 ProductExt.PriceTemp = tempTable.PriceTemp,
		 ProductExt.AmountSort=tempTable.AmountSort
      FROM
         catalog.ProductExt 
         INNER JOIN
            (               
           select 
		pe.ProductId,
		CountPhoto=case when offerId.ColorID is null then countNocolor.countNocolor else countColor.countColor end,				
		PhotoId=case when offerId.ColorID is null then PhotoIdNoColor.PhotoIdNoColor else PhotoIdColor.PhotoIdColor end,
		[VideosAvailable]=isnull(videosAvailable.videosAvailable,0),
		[MaxAvailable]=maxAvailable.maxAvailable,
		[NotSamePrices]=isnull(notSamePrices.notSamePrices,0),
		[MinPrice]=isnull(minPrice.minPrice,0),
		[OfferId]=offerId.OfferId,
		[Comments]=isnull(comments.comments,0),
		[Gifts]=isnull(gifts.gifts,0),
		[Colors]=(SELECT [Settings].[ProductColorsToString](pe.ProductId)),
		[CategoryId] = (SELECT TOP 1 id	FROM [Settings].[GetParentsCategoryByChild](( SELECT TOP 1 CategoryID FROM [Catalog].ProductCategories	WHERE ProductID = pe.ProductId ORDER BY Main DESC))ORDER BY sort DESC),
		PriceTemp = CASE WHEN p.Discount > 0 THEN (isnull(minPrice.minPrice,0) - isnull(minPrice.minPrice,0) * p.Discount/100)*c.CurrencyValue ELSE (isnull(minPrice.minPrice,0) - isnull(p.DiscountAmount,0))*c.CurrencyValue END,
		AmountSort=CASE when ISNULL(maxAvailable.maxAvailable, 0) <= 0 OR maxAvailable.maxAvailable < IsNull(p.MinAmount, 0) THEN 0 ELSE 1 end

from Catalog.[ProductExt] pe
left join (SELECT o.ProductId, COUNT(*) countColor FROM [Catalog].[Photo] ph INNER JOIN [Catalog].[Offer] o  ON ph.[ObjId] = o.ProductId 
            WHERE ( ph.ColorID = o.ColorID OR ph.ColorID IS NULL ) AND TYPE = 'Product' AND o.Main = 1 
			group by o.ProductId
) countColor on pe.ProductId=countColor.ProductId

left join (SELECT [ObjId], COUNT(*) countNocolor FROM [Catalog].[Photo]  
            WHERE TYPE = 'Product'
			group by [ObjId]
) countNocolor on pe.ProductId=countNocolor.[ObjId]

left join (
select ProductId, PhotoId PhotoIdColor from (
SELECT o.ProductId, ph.PhotoId, Row_Number() over (PARTITION  by o.ProductId ORDER BY ph.main DESC ,ph.[PhotoSortOrder], ph.[PhotoId]) rn FROM [Catalog].[Photo] ph
							INNER JOIN [Catalog].[Offer] o ON ph.[ObjId] = o.ProductId
							WHERE (ph.ColorID = o.ColorID OR ph.ColorID IS NULL) AND TYPE = 'Product' ) ct where rn=1
) PhotoIdColor on pe.ProductId=PhotoIdColor.ProductId

left join (
select ProductId, PhotoId PhotoIdNoColor from (
SELECT ph.[ObjId] ProductId, ph.PhotoId, Row_Number() over (PARTITION  by ph.[ObjId] ORDER BY ph.main DESC ,ph.[PhotoSortOrder], ph.[PhotoId]) rn FROM [Catalog].[Photo] ph	WHERE TYPE = 'Product' ) ct where rn=1
) PhotoIdNoColor on pe.ProductId=PhotoIdNoColor.ProductId

left join (select pv.ProductID, CASE WHEN COUNT(pv.ProductVideoID) > 0 THEN 1	ELSE 0 END videosAvailable FROM [Catalog].[ProductVideo] pv group by pv.ProductID) videosAvailable on pe.ProductId=videosAvailable.ProductId
left join (select o.ProductID,Max(o.Amount) maxAvailable  FROM [Catalog].Offer o group by o.ProductID) maxAvailable on pe.ProductId=maxAvailable.ProductId
left join (select o.ProductID, CASE WHEN MAX(o.price) - MIN(o.price) > 0 THEN 1 ELSE 0 END notSamePrices  FROM [Catalog].Offer o where o.price > 0 AND (@OnlyAvailable = 0 OR o.amount > 0) group by o.ProductID) notSamePrices on pe.ProductId=notSamePrices.ProductId
left join (select o.ProductID,MIN(o.price) minPrice FROM [Catalog].Offer o where o.price > 0 AND (@OnlyAvailable = 0 OR o.amount > 0)  group by o.ProductID) minPrice on pe.ProductId=minPrice.ProductId
left join ( 
select ProductId, OfferID, colorId from (
select o.ProductID,o.OfferID, o.colorId, Row_Number() over (PARTITION  by o.OfferID ORDER BY o.OfferID) rn  FROM [Catalog].Offer o where o.Main = 1 )ct where rn=1
) offerId on pe.ProductId=offerId.ProductId
left join (select EntityId ProductID,count(ReviewId) comments  FROM CMS.Review  where (Checked = 1 OR @ModerateReviews = 0) group by EntityId) comments on pe.ProductId=comments.ProductId
left join (select pg.ProductID, CASE WHEN COUNT(pg.ProductID) > 0 THEN 1 ELSE 0 END gifts FROM [Catalog].[ProductGifts] pg INNER JOIN Catalog.Offer on pg.GiftOfferId = Offer.OfferId INNER JOIN Catalog.Product on Offer.ProductId = Product.ProductId WHERE Offer.Amount > ISNULL(Product.MinAmount, 0) and Enabled = 1 group by pg.ProductID) gifts on pe.ProductId=gifts.ProductId
inner join catalog.Product p on p.ProductID = pe.ProductID
INNER JOIN CATALOG.Currency c ON p.currencyid = c.currencyid
 )
AS tempTable 
ON tempTable.ProductId = ProductExt.ProductId
    
END

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.ExportImport.CustomerFields.Organization', 'Организация')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.ExportImport.CustomerFields.Organization', 'Organization')

GO--


INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Home.Menu.SearchQueries', 'Поисковые запросы')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Home.Menu.SearchQueries', 'Search queries')


INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Analytics.SearchQueries', 'Поисковые запросы')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Analytics.SearchQueries', 'Search queries')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.Analytics.SearchQueries', 'Поисковый запрос')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.Analytics.SearchQueries', 'Search queries')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.Analytics.SearchQueriesFoundCount', 'Найдено')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.Analytics.SearchQueriesFoundCount', 'Found')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.Analytics.SearchQueriesDate', 'Дата запроса')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.Analytics.SearchQueriesDate', 'Request date')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.Analytics.SearchQueriesCustomer', 'Пользователь')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.Analytics.SearchQueriesCustomer', 'Customer')

GO--
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.PaymentMethods.UniversalPayGate.SellerLogin','Логин продавца')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.PaymentMethods.UniversalPayGate.SellerLogin','Seller Login')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.PaymentMethods.UniversalPayGate.EnterVendorLoginSpecifiedInPaySystem','Введите логин продавца, указанный в платежной системе')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.PaymentMethods.UniversalPayGate.EnterVendorLoginSpecifiedInPaySystem','Enter the vendor login specified in the paysystem')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.PaymentMethods.UniversalPayGate.MerchantLogin','Логин продавца')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.PaymentMethods.UniversalPayGate.MerchantLogin','Merchant login')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.PaymentMethods.UniversalPayGate.EnterMerchantLogin','Введите логин продавца, указанный в системе Робокассы')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.PaymentMethods.UniversalPayGate.EnterMerchantLogin','Enter the merchant login specified in the RoboCassault system')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.PaymentMethods.UniversalPayGate.Password1','Пароль для создания платежной формы')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.PaymentMethods.UniversalPayGate.Password1','Password to create a payment form')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.PaymentMethods.UniversalPayGate.Password1UsedInterface','Используется интерфейсом инициализации оплаты')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.PaymentMethods.UniversalPayGate.Password1UsedInterface','It is used by the initialization of payment interface')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.PaymentMethods.UniversalPayGate.Password2','Пароль для уведомлений об оплате')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.PaymentMethods.UniversalPayGate.Password2','Password for payment notifications')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.PaymentMethods.UniversalPayGate.Password2UsedInterface','Используется интерфейсом оповещения о платеже')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.PaymentMethods.UniversalPayGate.Password2UsedInterface','Itis used by the payment notification interface')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.PaymentMethods.UniversalPayGate.SendDataForCheck','Передавать данные для чека')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.PaymentMethods.UniversalPayGate.SendDataForCheck','Send data for check')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.PaymentMethods.UniversalPayGate.Url','Доступный шлюз')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.PaymentMethods.UniversalPayGate.Url','Avalible gateway')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.PaymentMethods.UniversalPayGate.TestMode','Тестовый режим')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.PaymentMethods.UniversalPayGate.TestMode','Test mode')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.PaymentMethods.UniversalPayGate.Password1Used','Используется интерфейсом инициализации оплаты')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.PaymentMethods.UniversalPayGate.Password1Used','It is used by the initialization of payment interface')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.PaymentMethods.UniversalPayGate.Password2UsedByPaymentNotification','Используется интерфейсом оповещения о платеже')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.PaymentMethods.UniversalPayGate.Password2UsedByPaymentNotification','It is used by the payment notification interface')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.Tasks.CompleteTask.StatusCurrent', 'текущий статус')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.Tasks.CompleteTask.StatusCurrent', 'current status')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.Tasks.CompleteTask.StageCurrent', 'текущий этап')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.Tasks.CompleteTask.StageCurrent', 'current stage')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Home.BirthdayDashboard.Description.In', 'через')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Home.BirthdayDashboard.Description.In', 'in')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Home.BirthdayDashboard.Description.Ago', 'назад')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Home.BirthdayDashboard.Description.Ago', 'ago')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Home.BirthdayDashboard.Description.WasBirthday', 'отмечал(а) день рождения.')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Home.BirthdayDashboard.Description.WasBirthday', 'had a birhday')

UPDATE [Settings].[Localization] set ResourceValue = 'празднует день рождения!' where ResourceKey = 'Admin.Home.BirthdayDashboard.Description.HappyBirthday' and LanguageId = 1

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Checkout.PrintOrder.TrackNumber', 'Трек-номер')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Checkout.PrintOrder.TrackNumber', 'Track Number')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Module.MailChimp.Header', 'Mailchimp - модуль рассылки сообщений')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Module.MailChimp.Header', 'Mailchimp module')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Module.MailChimp.FromEmail', 'Email отправителя')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Module.MailChimp.FromEmail', '"Reply-to" email')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Module.MailChimp.FromName', 'Имя отправителя')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Module.MailChimp.FromName', 'From name')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Module.MailChimp.Id', 'ApiKey')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Module.MailChimp.Id', 'ApiKey in MailChimp system')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Module.MailChimp.ListSubscribers', 'Список подписчиков')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Module.MailChimp.ListSubscribers', 'List of subscribers')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Module.MailChimp.MailChimpSubscribers', 'Списки подписчиков в MailChimp')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Module.MailChimp.MailChimpSubscribers', 'Lists of MailChimp subscribers')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Module.MailChimp.OderCustomersList', 'Пользователи, оформившие заказ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Module.MailChimp.OderCustomersList', 'Users who place an order')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Module.MailChimp.Save', 'Сохранить')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Module.MailChimp.Save', 'Save')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Module.MailChimp.ShopSubscribers', 'Списки подписчиков магазина')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Module.MailChimp.ShopSubscribers', 'Lists of store subscribers')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Module.MailChimp.Funnels', 'Воронки продаж')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Module.MailChimp.Funnels', 'Funnels')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Module.MailChimp.SubscribeToMailChimp', 'Подписать текущих клиентов')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Module.MailChimp.SubscribeToMailChimp', 'Sign current customers')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Module.MailChimp.SubscribeNotify', 'Новые клиенты будут автоматически попадать в соответствующие списки в MailChimp. Для подписки текущих клиентов нажмите')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Module.MailChimp.SubscribeNotify', 'New clients will automatically fall into the appropriate lists in MailChimp. To subscribe to current customers, click')

GO--

CREATE TABLE [CMS].[LandingDomain](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[LandingSiteId] [int] NOT NULL,
	[DomainUrl] [nvarchar](max) NOT NULL,
	[IsMain] [bit] NOT NULL,
 CONSTRAINT [PK_LandingDomain] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO--

ALTER TABLE [CMS].[LandingDomain]  WITH CHECK ADD  CONSTRAINT [FK_LandingDomain_LandingSite] FOREIGN KEY([LandingSiteId])
REFERENCES [CMS].[LandingSite] ([Id])
ON DELETE CASCADE
GO--

ALTER TABLE [CMS].[LandingDomain] CHECK CONSTRAINT [FK_LandingDomain_LandingSite]
GO--

Insert Into [CMS].[LandingDomain] ([LandingSiteId],[DomainUrl],[IsMain]) 
	Select [Id], [DomainUrl], 1 From [CMS].[LandingSite] Where [LandingSite].[DomainUrl] is not null and [LandingSite].[DomainUrl] <> ''
GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ExportFeeed.SettingsYandex.OnlyMainOfferToExport', 'Выгружать только главную модификацию')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ExportFeeed.SettingsYandex.OnlyMainOfferToExport', 'Export only the main offer')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ExportFeeed.SettingsGoogle.OnlyMainOfferToExport', 'Выгружать только главную модификацию')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ExportFeeed.SettingsGoogle.OnlyMainOfferToExport', 'Export only the main offer')

GO--


INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Module.UniSender.Header', 'UniSender - модуль рассылки сообщений')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Module.UniSender.Header', 'UniSender module')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Module.UniSender.FromEmail', 'Email отправителя')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Module.UniSender.FromEmail', '"Reply-to" email')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Module.UniSender.FromName', 'Имя отправителя')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Module.UniSender.FromName', 'From name')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Module.UniSender.Id', 'ApiKey')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Module.UniSender.Id', 'ApiKey in UniSender system')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Module.UniSender.ListSubscribers', 'Список подписчиков')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Module.UniSender.ListSubscribers', 'List of subscribers')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Module.UniSender.UniSenderSubscribers', 'Списки подписчиков в UniSender')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Module.UniSender.UniSenderSubscribers', 'Lists of UniSender subscribers')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Module.UniSender.OderCustomersList', 'Пользователи, оформившие заказ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Module.UniSender.OderCustomersList', 'Users who place an order')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Module.UniSender.Save', 'Сохранить')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Module.UniSender.Save', 'Save')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Module.UniSender.ShopSubscribers', 'Списки подписчиков магазина')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Module.UniSender.ShopSubscribers', 'Lists of store subscribers')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Module.UniSender.Funnels', 'Воронки продаж')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Module.UniSender.Funnels', 'Funnels')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Module.UniSender.SubscribeToUniSender', 'Подписать текущих клиентов')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Module.UniSender.SubscribeToUniSender', 'Sign current customers')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Module.UniSender.SubscribeNotify', 'Новые клиенты будут автоматически попадать в соответствующие списки в UniSender. Для подписки текущих клиентов нажмите')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Module.UniSender.SubscribeNotify', 'New clients will automatically fall into the appropriate lists in UniSender. To subscribe to current customers, click')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Crm.TaskPriority.Critical', 'Критичный')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Crm.TaskPriority.Critical', 'Critical')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.Leads.Organization', 'Организация')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.Leads.Organization', 'Organization')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.SettingsSystem.Err400', 'BadRequest ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.SettingsSystem.Err400', 'BadRequest ')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.SettingsSystem.ErrModule', 'Ошибки модулей')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.SettingsSystem.ErrModule', 'Module errors')

GO--

UPDATE [Settings].[Localization] set ResourceValue = 'Воронка «Лид-магнит «Статья»' where ResourceKey = 'Landing.Domain.LpFunnelType.LeadMagnet' and LanguageId = 1
UPDATE [Settings].[Localization] set ResourceValue = 'Воронка «Лид-магнит «Видео»' where ResourceKey = 'Landing.Domain.LpFunnelType.VideoLeadMagnet' and LanguageId = 1
UPDATE [Settings].[Localization] set ResourceValue = 'Воронка «Товарная с заявкой»' where ResourceKey = 'Landing.Domain.LpFunnelType.LandingFunnel' and LanguageId = 1
UPDATE [Settings].[Localization] set ResourceValue = 'Воронка «Товарная с оплатой»' where ResourceKey = 'Landing.Domain.LpFunnelType.LandingFunnelOrder' and LanguageId = 1
UPDATE [Settings].[Localization] set ResourceValue = 'Видео-воронка «Товарная с заявкой»' where ResourceKey = 'Landing.Domain.LpFunnelType.VideoLandingFunnel' and LanguageId = 1
UPDATE [Settings].[Localization] set ResourceValue = 'Видео-воронка «Товарная с оплатой»' where ResourceKey = 'Landing.Domain.LpFunnelType.VideoLandingFunnelOrder' and LanguageId = 1
UPDATE [Settings].[Localization] set ResourceValue = 'Воронка «Купонатор с заявкой»' where ResourceKey = 'Landing.Domain.LpFunnelType.Couponator' and LanguageId = 1
UPDATE [Settings].[Localization] set ResourceValue = 'Воронка «Купонатор с оплатой»' where ResourceKey = 'Landing.Domain.LpFunnelType.CouponatorOrder' and LanguageId = 1
UPDATE [Settings].[Localization] set ResourceValue = 'Воронка «Мультитоварная»' where ResourceKey = 'Landing.Domain.LpFunnelType.MultyProducts' and LanguageId = 1
UPDATE [Settings].[Localization] set ResourceValue = 'Воронка «Бесплатный товар + доставка»' where ResourceKey = 'Landing.Domain.LpFunnelType.OneProductUpSellDownSell' and LanguageId = 1
UPDATE [Settings].[Localization] set ResourceValue = 'Воронка «Мероприятие»' where ResourceKey = 'Landing.Domain.LpFunnelType.Events' and LanguageId = 1
UPDATE [Settings].[Localization] set ResourceValue = 'Воронка «Конференция»' where ResourceKey = 'Landing.Domain.LpFunnelType.Conference' and LanguageId = 1
UPDATE [Settings].[Localization] set ResourceValue = 'Воронка «Курс»' where ResourceKey = 'Landing.Domain.LpFunnelType.Course' and LanguageId = 1
UPDATE [Settings].[Localization] set ResourceValue = 'Воронка «Консалтинг»' where ResourceKey = 'Landing.Domain.LpFunnelType.Consulting' and LanguageId = 1
UPDATE [Settings].[Localization] set ResourceValue = 'Воронка «Услуга с онлайн-записью»' where ResourceKey = 'Landing.Domain.LpFunnelType.ServicesOnline' and LanguageId = 1

GO--

IF EXISTS(SELECT TOP(1) * FROM [Order].[PaymentMethod] WHERE PaymentType = 'Bill')
AND NOT EXISTS(SELECT TOP(1) * FROM [Order].[PaymentParam] WHERE Name = 'Bill_ShowPaymentDetails' AND PaymentMethodID IN (SELECT PaymentMethodID FROM [Order].[PaymentMethod] WHERE PaymentType = 'Bill'))
BEGIN
	INSERT INTO [Order].[PaymentParam] (PaymentMethodID, Name, Value) 
	SELECT PaymentMethodID, 'Bill_ShowPaymentDetails', 'True' FROM [Order].[PaymentMethod] WHERE PaymentType = 'Bill'
END

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.PaymentMethods.Bill.ShowPaymentDetails', 'Запрашивать ИНН и название компании у покупателя')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.PaymentMethods.Bill.ShowPaymentDetails', 'Request details in the client side')
GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ShippingMethods.Sdek.SynchronizeOrderStatuses','Синхронизировать статусы заказов из СДЭК')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ShippingMethods.Sdek.SynchronizeOrderStatuses','Synchronize order statuses from Sdek')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ShippingMethods.Sdek.Statuses','Статусы')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ShippingMethods.Sdek.Statuses','Statuses')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ShippingMethods.YandexDelivery.SynchronizeOrderStatuses','Синхронизировать статусы заказов из Яндекс.Доставки')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ShippingMethods.YandexDelivery.SynchronizeOrderStatuses','Synchronize order statuses from YandexDelivery')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ShippingMethods.YandexDelivery.Statuses','Статусы')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ShippingMethods.YandexDelivery.Statuses','Statuses')
	
GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Orders.OrderItem.Organization','Организация:')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Orders.OrderItem.Organization','Organization:')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Orders.OrderItem.Inn','ИНН:')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Orders.OrderItem.Inn','INN:')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Orders.OrderItem.Company','Компания:')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Orders.OrderItem.Company','Company:')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Orders.OrderItem.Payer','Плательщик:')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Orders.OrderItem.Payer','Payer:')

GO--

CREATE TABLE [CRM].[TriggerRule](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[EventType] [int] NOT NULL,
	[ObjectType] [int] NOT NULL,
	[EventObjId] [int] NULL,
	[Name] [nvarchar](max) NOT NULL,
	[Filter] [nvarchar](max) NULL,
	[DateCreated] [datetime] NOT NULL,
	[DateModified] [datetime] NOT NULL,
	[Enabled] [bit] NOT NULL,
	[WorksOnlyOnce] [bit] NOT NULL
 CONSTRAINT [PK_TriggerRule] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO--

ALTER TABLE [CRM].[TriggerRule] ADD  CONSTRAINT [DF_TriggerRule_DateCreated]  DEFAULT (getdate()) FOR [DateCreated]
GO--

ALTER TABLE [CRM].[TriggerRule] ADD  CONSTRAINT [DF_TriggerRule_DateModified]  DEFAULT (getdate()) FOR [DateModified]
GO--


CREATE TABLE [CRM].[TriggerAction](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[TriggerRuleId] [int] NOT NULL,
	[ActionType] [int] NOT NULL,
	[TimeDelay] [nvarchar](max) NULL,
	[EmailSubject] [nvarchar](max) NULL,
	[EmailBody] [nvarchar](max) NULL,
	[SmsText] [nvarchar](max) NULL,
 CONSTRAINT [PK_TriggerAction] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO--

ALTER TABLE [CRM].[TriggerAction]  WITH CHECK ADD  CONSTRAINT [FK_TriggerAction_TriggerRule] FOREIGN KEY([TriggerRuleId])
REFERENCES [CRM].[TriggerRule] ([Id])
ON DELETE CASCADE
GO--

ALTER TABLE [CRM].[TriggerAction] CHECK CONSTRAINT [FK_TriggerAction_TriggerRule]
GO--

CREATE TABLE [CRM].[TriggerDeferredData](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[EntityId] [int] NOT NULL,
	[TriggerActionId] [int] NOT NULL,
	[TriggerObjectType] [int] NOT NULL,
	[DateCreated] [datetime] NOT NULL,
 CONSTRAINT [PK_TriggerDeferredData] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]

GO--

ALTER TABLE [CRM].[TriggerDeferredData] ADD  CONSTRAINT [DF_TriggerDeferredData_DateCreated]  DEFAULT (getdate()) FOR [DateCreated]
GO--

ALTER TABLE [CRM].[TriggerDeferredData]  WITH CHECK ADD  CONSTRAINT [FK_TriggerDeferredData_TriggerAction] FOREIGN KEY([TriggerActionId])
REFERENCES [CRM].[TriggerAction] ([Id])
ON DELETE CASCADE
GO--

ALTER TABLE [CRM].[TriggerDeferredData] CHECK CONSTRAINT [FK_TriggerDeferredData_TriggerAction]
GO--

CREATE TABLE [CRM].[TriggerSendOnceData](
	[TriggerId] [int] NOT NULL,
	[EntityId] [int] NOT NULL,
 CONSTRAINT [PK_TriggerSendOnceData] PRIMARY KEY CLUSTERED 
(
	[TriggerId] ASC,
	[EntityId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO--

ALTER TABLE [CRM].[TriggerSendOnceData]  WITH CHECK ADD  CONSTRAINT [FK_TriggerSendOnceData_TriggerRule] FOREIGN KEY([TriggerId])
REFERENCES [CRM].[TriggerRule] ([Id])
ON DELETE CASCADE
GO--

ALTER TABLE [CRM].[TriggerSendOnceData] CHECK CONSTRAINT [FK_TriggerSendOnceData_TriggerRule]
GO--

update catalog.currency set [Code] = ' €' where [Code]= '€'

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Social.SocialWidgetInstruction','Инструкция. Виджеты социальных сетей')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Social.SocialWidgetInstruction','Instructions. Social Network Widgets')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Social.SocialLinksInstruction','Инструкция. Сообщества в социальных сетях')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Social.SocialLinksInstruction','Instructions. Communities in social networks')

GO--

if (Select Count([MenuItemID]) From [CMS].[Menu] Where [MenuItemUrlPath] Like '%vk.com%') > 0
begin
	if (Select Count([Name]) From [Settings].[Settings] Where [Name] = 'LinkVk') = 0
	begin
		DECLARE @LinkVk NVARCHAR(MAX);  
		Set @LinkVk = (Select Top(1) [MenuItemUrlPath] From [CMS].[Menu] Where [MenuItemUrlPath] Like '%vk.com%')
		Insert into [Settings].[Settings] ([Name],[Value]) Values ('LinkVk',@LinkVk)
		if (Select Count([Name]) From [Settings].[Settings] Where [Name] = 'LinkVkActive') = 0
		begin
			Insert into [Settings].[Settings] ([Name],[Value]) Values ('LinkVkActive', 'True')
		end
		Update [CMS].[Menu] Set Enabled = 0 Where [MenuItemUrlPath] Like '%vk.com%'		
	end
end


if (Select Count([MenuItemID]) From [CMS].[Menu] Where [MenuItemUrlPath] Like '%facebook.com%') > 0
begin
	if (Select Count([Name]) From [Settings].[Settings] Where [Name] = 'LinkFacebook') = 0
	begin
		DECLARE @LinkFacebook NVARCHAR(MAX);
		Set @LinkFacebook = (Select Top(1) [MenuItemUrlPath] From [CMS].[Menu] Where [MenuItemUrlPath] Like '%facebook.com%')
		Insert into [Settings].[Settings] ([Name],[Value]) Values ('LinkFacebook',@LinkFacebook)
		if (Select Count([Name]) From [Settings].[Settings] Where [Name] = 'LinkFacebookActive') = 0
		begin
			Insert into [Settings].[Settings] ([Name],[Value]) Values ('LinkFacebookActive', 'True')
		end
		Update [CMS].[Menu] Set Enabled = 0 Where [MenuItemUrlPath] Like '%facebook.com%'
	end
end


if (Select Count([MenuItemID]) From [CMS].[Menu] Where [MenuItemUrlPath] Like '%instagram.com%') > 0
begin
	if (Select Count([Name]) From [Settings].[Settings] Where [Name] = 'LinkInstagramm') = 0
	begin
		DECLARE @LinkInstagramm NVARCHAR(MAX);
		Set @LinkInstagramm = (Select Top(1) [MenuItemUrlPath] From [CMS].[Menu] Where [MenuItemUrlPath] Like '%instagram.com%')
		Insert into [Settings].[Settings] ([Name],[Value]) Values ('LinkInstagramm',@LinkInstagramm)
		if (Select Count([Name]) From [Settings].[Settings] Where [Name] = 'LinkInstagrammActive') = 0
		begin
			Insert into [Settings].[Settings] ([Name],[Value]) Values ('LinkInstagrammActive', 'True')
		end
		Update [CMS].[Menu] Set Enabled = 0 Where [MenuItemUrlPath] Like '%instagram.com%'
	end
end

if (Select Count([MenuItemID]) From [CMS].[Menu] Where [MenuItemUrlPath] Like '%ok.ru%') > 0
begin
	if (Select Count([Name]) From [Settings].[Settings] Where [Name] = 'LinkOk') = 0
	begin
		DECLARE @LinkOk NVARCHAR(MAX);
		Set @LinkOk = (Select Top(1) [MenuItemUrlPath] From [CMS].[Menu] Where [MenuItemUrlPath] Like '%ok.ru%')
		Insert into [Settings].[Settings] ([Name],[Value]) Values ('LinkOk',@LinkOk)
		if (Select Count([Name]) From [Settings].[Settings] Where [Name] = 'LinkOkActive') = 0
		begin
			Insert into [Settings].[Settings] ([Name],[Value]) Values ('LinkOkActive', 'True')
		end
		Update [CMS].[Menu] Set Enabled = 0 Where [MenuItemUrlPath] Like '%ok.ru%'
	end
end

if (Select Count([MenuItemID]) From [CMS].[Menu] Where [MenuItemUrlPath] Like '%twitter.com%') > 0
begin
	if (Select Count([Name]) From [Settings].[Settings] Where [Name] = 'LinkTwitter') = 0
	begin
		DECLARE @LinkTwitter NVARCHAR(MAX);
		Set @LinkTwitter = (Select Top(1) [MenuItemUrlPath] From [CMS].[Menu] Where [MenuItemUrlPath] Like '%twitter.com%')
		Insert into [Settings].[Settings] ([Name],[Value]) Values ('LinkTwitter',@LinkTwitter)
		if (Select Count([Name]) From [Settings].[Settings] Where [Name] = 'LinkTwitterActive') = 0
		begin
			Insert into [Settings].[Settings] ([Name],[Value]) Values ('LinkTwitterActive', 'True')
		end
		Update [CMS].[Menu] Set Enabled = 0 Where [MenuItemUrlPath] Like '%twitter.com%'
	end
end

if (Select Count([MenuItemID]) From [CMS].[Menu] Where [MenuItemUrlPath] Like '%t.me%') > 0
begin
	if (Select Count([Name]) From [Settings].[Settings] Where [Name] = 'LinkTelegram') = 0
	begin
		DECLARE @LinkTelegram NVARCHAR(MAX);
		Set @LinkTelegram =(Select Top(1) [MenuItemUrlPath] From [CMS].[Menu] Where [MenuItemUrlPath] Like '%t.me%')
		Insert into [Settings].[Settings] ([Name],[Value]) Values ('LinkTelegram',@LinkTelegram)
		if (Select Count([Name]) From [Settings].[Settings] Where [Name] = 'LinkTelegramActive') = 0
		begin
			Insert into [Settings].[Settings] ([Name],[Value]) Values ('LinkTelegramActive', 'True')
		end
		Update [CMS].[Menu] Set Enabled = 0 Where [MenuItemUrlPath] Like '%t.me%'
	end
end

if (Select Count([MenuItemID]) From [CMS].[Menu] Where [MenuItemUrlPath] Like '%youtube.com%') > 0
begin
	if (Select Count([Name]) From [Settings].[Settings] Where [Name] = 'LinkYoutube') = 0
	begin
		DECLARE @LinkYoutube NVARCHAR(MAX);
		Set @LinkYoutube =(Select Top(1) [MenuItemUrlPath] From [CMS].[Menu] Where [MenuItemUrlPath] Like '%youtube.com%')
		Insert into [Settings].[Settings] ([Name],[Value]) Values ('LinkYoutube',@LinkYoutube)
		if (Select Count([Name]) From [Settings].[Settings] Where [Name] = 'LinkYoutubeActive') = 0
		begin
			Insert into [Settings].[Settings] ([Name],[Value]) Values ('LinkYoutubeActive', 'True')
		end
		Update [CMS].[Menu] Set Enabled = 0 Where [MenuItemUrlPath] Like '%youtube.me%'
	end
end

if(Select Count([MenuItemID]) From [CMS].[Menu] Where [MenuItemName] Like '%Мы в социальных сетях%' Or [MenuItemName] Like '%Мы в соц сетях%') > 0
begin
	DECLARE @menuParentId int;
	Set @menuParentId =(Select Top(1) [MenuItemID] From [CMS].[Menu] Where [MenuItemName] Like '%Мы в социальных сетях%' Or [MenuItemName] Like '%Мы в соц сетях%')
	if(Select Count([MenuItemID]) From [CMS].[Menu] Where [MenuItemParentID] = @menuParentId And [Enabled] = 1) = 0
	begin
		Update [CMS].[Menu] Set [Enabled] = 0 Where [MenuItemID] = @menuParentId
	end	
end

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.SystemSettings.SiteLanguage','Язык магазина')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.SystemSettings.SiteLanguage','Store language')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Triggers.Index.Title','Авторассылки')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Triggers.Index.Title','Triggers')

Insert Into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (1, 'Core.Services.Triggers.ETriggerEventType.OrderCreated', 'Новый заказ')
Insert Into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (1, 'Core.Services.Triggers.ETriggerEventType.Description.OrderCreated', 'Триггер на новый заказ')
Insert Into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (1, 'Core.Services.Triggers.ETriggerEventType.OrderStatusChanged', 'Смена статуса заказа')
Insert Into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (1, 'Core.Services.Triggers.ETriggerEventType.Description.OrderStatusChanged', 'Триггер при смене статуса заказа')
Insert Into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (1, 'Core.Services.Triggers.ETriggerEventType.LeadCreated', 'Новый лид')
Insert Into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (1, 'Core.Services.Triggers.ETriggerEventType.Description.LeadCreated', 'Триггер на новый лид')
Insert Into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (1, 'Core.Services.Triggers.ETriggerEventType.LeadStatusChanged', 'Смена этапа лида')
Insert Into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (1, 'Core.Services.Triggers.ETriggerEventType.Description.LeadStatusChanged', 'Триггер на смену этапа лида')
Insert Into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (1, 'Core.Services.Triggers.ETriggerEventType.CustomerCreated', 'Новый пользователь')
Insert Into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (1, 'Core.Services.Triggers.ETriggerEventType.Description.CustomerCreated', 'Триггер при регистрации пользователя')

Insert Into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (2, 'Core.Services.Triggers.ETriggerEventType.OrderCreated', 'New order')
Insert Into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (2, 'Core.Services.Triggers.ETriggerEventType.Description.OrderCreated', 'New order')
Insert Into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (2, 'Core.Services.Triggers.ETriggerEventType.OrderStatusChanged', 'Order status changing')
Insert Into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (2, 'Core.Services.Triggers.ETriggerEventType.Description.OrderStatusChanged', 'Order status changing')
Insert Into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (2, 'Core.Services.Triggers.ETriggerEventType.LeadCreated', 'New lead')
Insert Into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (2, 'Core.Services.Triggers.ETriggerEventType.Description.LeadCreated', 'New lead')
Insert Into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (2, 'Core.Services.Triggers.ETriggerEventType.LeadStatusChanged', 'Lead status changing')
Insert Into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (2, 'Core.Services.Triggers.ETriggerEventType.Description.LeadStatusChanged', 'Lead status changing')
Insert Into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (2, 'Core.Services.Triggers.ETriggerEventType.CustomerCreated', 'New customer')
Insert Into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (2, 'Core.Services.Triggers.ETriggerEventType.Description.CustomerCreated', 'New customer')

GO--


INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Product.Edit.EnabledInSalesChannel','Каналы продаж')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Product.Edit.EnabledInSalesChannel','Sales channels')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Product.Edit.SetExcludedSalesChannel','Настроить')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Product.Edit.SetExcludedSalesChannel','Tune')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.EditProduct.SalesChannelsEnable','Активность товара в каналах продаж')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.EditProduct.SalesChannelsEnable','Product activity in sales channels')

GO--
CREATE TABLE [CRM].[SalesChannelExcludedProduct](
	[ProductId] [int] NOT NULL,
	[SalesChannelKey] [nvarchar](50) NOT NULL,
 CONSTRAINT [IX_ProductSalesChannel] UNIQUE NONCLUSTERED 
(
	[ProductId] ASC,
	[SalesChannelKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO--

ALTER TABLE [CRM].[SalesChannelExcludedProduct]  WITH CHECK ADD  CONSTRAINT [FK_ProductSalesChannel_Product] FOREIGN KEY([ProductId])
REFERENCES [Catalog].[Product] ([ProductId])
ON DELETE CASCADE
GO--

ALTER TABLE [CRM].[SalesChannelExcludedProduct] CHECK CONSTRAINT [FK_ProductSalesChannel_Product]

GO--
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.Catalog.SalesChannelEnable','Активность товара в каналах продаж')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.Catalog.SalesChannelEnable','Product activity in sales channels')
GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ShippingMethods.Boxberry.SynchronizeOrderStatuses','Синхронизировать статусы заказов из Boxberry')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ShippingMethods.Boxberry.SynchronizeOrderStatuses','Synchronize order statuses from Boxberry')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ShippingMethods.Boxberry.Statuses','Статусы')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ShippingMethods.Boxberry.Statuses','Statuses')

GO--

ALTER PROCEDURE [Customers].[sp_AddCustomer]      
 @CustomerID uniqueidentifier,    
 @CustomerGroupID int,    
 @Password nvarchar(100),    
 @FirstName nvarchar(70),    
 @LastName nvarchar(70),    
 @Phone nvarchar(max),    
 @StandardPhone bigint,    
 @RegistrationDateTime datetime,    
 @Email nvarchar(100),    
 @CustomerRole int,    
 @Patronymic nvarchar(70),    
 @BonusCardNumber bigint,    
 @AdminComment nvarchar(MAX),    
 @ManagerId int,    
 @Rating int,    
 @Enabled bit,    
 @HeadCustomerId uniqueidentifier,    
 @BirthDay datetime,    
 @City nvarchar(70),    
 @Organization nvarchar(250),  
 @ClientStatus int  
AS    
    
BEGIN    
 if @CustomerID is null    
  Set @CustomerID = newID()    
    
 INSERT INTO [Customers].[Customer]    
  ([CustomerID]    
  ,[CustomerGroupID]    
  ,[Password]    
  ,[FirstName]    
  ,[LastName]    
  ,[Phone]    
  ,[StandardPhone]    
  ,[RegistrationDateTime]    
  ,[Email]    
  ,[CustomerRole]    
  ,[Patronymic]    
  ,[BonusCardNumber]    
  ,[AdminComment]    
  ,[ManagerId]    
  ,[Rating]    
  ,[Enabled]    
  ,[HeadCustomerId]    
  ,[BirthDay]    
  ,[City]    
  ,[Organization]  
  ,[ClientStatus])    
    
 VALUES    
  (@CustomerID    
  ,@CustomerGroupID    
  ,@Password    
  ,@FirstName    
  ,@LastName    
  ,@Phone    
  ,@StandardPhone    
  ,@RegistrationDateTime    
  ,@Email    
  ,@CustomerRole    
  ,@Patronymic    
  ,@BonusCardNumber    
  ,@AdminComment    
  ,@ManagerId    
  ,@Rating    
  ,@Enabled    
  ,@HeadCustomerId    
  ,@BirthDay    
  ,@City    
  ,@Organization  
  ,@ClientStatus);    
    
 SELECT CustomerID, InnerId From [Customers].[Customer] Where CustomerId = @CustomerID    
END

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.OrderItemsSummary.UsersComment', 'Комментарий пользователя к заказу')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.OrderItemsSummary.UsersComment', 'Customer comment')

GO--
  


Update [Settings].[Localization] Set [ResourceValue] = 'Логи' Where [LanguageId] = 1 and [ResourceKey] = 'Admin.Settings.System.LogError'
Update [Settings].[Localization] Set [ResourceValue] = 'Logs' Where [LanguageId] = 2 and [ResourceKey] = 'Admin.Settings.System.LogError'


INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.SettingsSystem.Warn','Предупреждения')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.SettingsSystem.Warn','Warnings')

GO--
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.ExportImport.ProductFields.SaleschannelStore','Канал продаж: Интернет-магазин')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.ExportImport.ProductFields.SaleschannelStore','Sales channel: Online store')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.ExportImport.ProductFields.SaleschannelFunnel','Канал продаж: Воронки продаж')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.ExportImport.ProductFields.SaleschannelFunnel','Sales channel: Sales funnels')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.ExportImport.ProductFields.SaleschannelVk','Канал продаж: ВКонтакте')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.ExportImport.ProductFields.SaleschannelVk','Sales channel: VKontakte')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.ExportImport.ProductFields.SaleschannelInstagram','Канал продаж: Instagram')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.ExportImport.ProductFields.SaleschannelInstagram','Sales channel: Instagram')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.ExportImport.ProductFields.SaleschannelYandex','Канал продаж: Яндекс.Маркет')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.ExportImport.ProductFields.SaleschannelYandex','Sales channel: Yandex.Market')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.ExportImport.ProductFields.SaleschannelAvito','Канал продаж: Avito')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.ExportImport.ProductFields.SaleschannelAvito','Sales channel: Avito')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.ExportImport.ProductFields.SaleschannelGoogle','Канал продаж: Google Merchant Center')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.ExportImport.ProductFields.SaleschannelGoogle','Sales channel: Google Merchant Center')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.ExportImport.ProductFields.SaleschannelFacebook','Канал продаж: Facebook')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.ExportImport.ProductFields.SaleschannelFacebook','Sales channel: Facebook')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.ExportImport.ProductFields.SaleschannelBonus','Канал продаж: Бонусная программа')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.ExportImport.ProductFields.SaleschannelBonus','Sales channel: Bonus program')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.ExportImport.ProductFields.SaleschannelReferal','Канал продаж: Партнерская программа')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.ExportImport.ProductFields.SaleschannelReferal','Sales channel: Affiliate program')

GO--
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.SalesChannels.SalesChannel.OnlineStore','Интернет-магазин')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.SalesChannels.SalesChannel.OnlineStore','Online store')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.SalesChannels.SalesChannel.SalesFunnels','Воронки продаж (новинка)')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.SalesChannels.SalesChannel.SalesFunnels','Sales funnels (new)')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.SalesChannels.SalesChannel.VKontakte','ВКонтакте')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.SalesChannels.SalesChannel.VKontakte','VKontakte')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.SalesChannels.SalesChannel.Instagram','Instagram')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.SalesChannels.SalesChannel.Instagram','Instagram')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.SalesChannels.SalesChannel.YandexMarket','Яндекс.Маркет')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.SalesChannels.SalesChannel.YandexMarket','Yandex.Market')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.SalesChannels.SalesChannel.Avito','Avito')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.SalesChannels.SalesChannel.Avito','Avito')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.SalesChannels.SalesChannel.GoogleMerchantCenter','Google Merchant Center')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.SalesChannels.SalesChannel.GoogleMerchantCenter','Google Merchant Center')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.SalesChannels.SalesChannel.Facebook','Facebook')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.SalesChannels.SalesChannel.Facebook','Facebook')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.SalesChannels.SalesChannel.BonusProgram','Бонусная программа')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.SalesChannels.SalesChannel.BonusProgram','Bonus program')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.SalesChannels.SalesChannel.AffiliateProgram','Партнерская программа')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.SalesChannels.SalesChannel.AffiliateProgram','Affiliate program')

GO--
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ExportFeeds.Index.H1','Выгрузка товаров')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ExportFeeds.Index.H1','Export feeds')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ExportFeeds.IndexYandex.H1','Яндекс Маркет')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ExportFeeds.IndexYandex.H1','Yandex Market')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ExportFeeds.IndexGoogle.H1','Google Merchant Center')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ExportFeeds.IndexGoogle.H1','Google Merchant Center')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ExportFeeds.IndexAvito.H1','Avito')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ExportFeeds.IndexAvito.H1','Avito')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ExportFeeds.IndexReseller.H1','Партнерская программа')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ExportFeeds.IndexReseller.H1','Reseller')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ExportFeeds.Index.subtitle','Выгружайте Ваши товары в формате CSV')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ExportFeeds.Index.subtitle','Upload your offers in CSV file')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ExportFeeds.IndexYandex.subtitle','Выгружайте Ваши товары на товарную площадку Яндекса')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ExportFeeds.IndexYandex.subtitle','Upload your offers to Yandex Market')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ExportFeeds.IndexGoogle.subtitle','Канал продаж Google Merchant Center')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ExportFeeds.IndexGoogle.subtitle','Google Merchant Center sales channel')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ExportFeeds.IndexAvito.subtitle','Развивайте свой бизнес на Авито')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ExportFeeds.IndexAvito.subtitle','Expand your business on Avito')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ExportFeeds.IndexReseller.subtitle','Продавайте через партнеров')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ExportFeeds.IndexReseller.subtitle','Sell ??through partners')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ExportFeeds.Index.Title','Выберите выгрузку для редактирования')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ExportFeeds.Index.Title','Select export to edit')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ExportFeed.ComeBackToList','Вернуться к списку')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ExportFeed.ComeBackToList','Come back to list')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ExportFeed.Delete','Удалить')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ExportFeed.Delete','Delete')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ExportFeed.SelectionGoods','Выбор товаров')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ExportFeed.SelectionGoods','Selection goods')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ExportFeed.ExportParameters','Параметры выгрузки')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ExportFeed.ExportParameters','Selection goods')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ExportFeed.YandexPromoCode','Промокоды')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ExportFeed.YandexPromoCode','Promo codes')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ExportFeed.YandexPromoFlash','Специальная цена')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ExportFeed.YandexPromoFlash','Flash discount')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ExportFeed.SettingsYandex.PositiveIntegerNumbers','Указывайте размер ставки в условных центах: например, значение 80 соответствует ставке 0,8 у. е. Значения должны быть целыми и положительными числами.')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ExportFeed.SettingsYandex.PositiveIntegerNumbers','Values must be integers and positive numbers.')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ExportFeeds.ExportFeedProgress.DownloadZipFile','Скачать zip архив')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ExportFeeds.ExportFeedProgress.DownloadZipFile','Download zip file')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Home.Menu.Avito','Avito')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Home.Menu.Avito','Avito')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Home.Menu.Reseller','Reseller')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Home.Menu.Reseller','Reseller')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ExportFeeed.SettingsCsv.CsvSeparatorCustom','Свой разделитель')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ExportFeeed.SettingsCsv.CsvSeparatorCustom','Custom separator')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ExportFeed.YandexPromoGift','Подарки')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ExportFeed.YandexPromoGift','Gifts')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ExportFeed.YandexPromoNPlusM','N + M')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ExportFeed.YandexPromoNPlusM','N + M')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.ExportFeeds.PromoName','Название')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.ExportFeeds.PromoName','Name')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.ExportFeeds.PromoDescription','Описание')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.ExportFeeds.PromoDescription','Description')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.ExportFeeds.PromoUrl','Ссылка на акцию')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.ExportFeeds.PromoUrl','Promo url')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.ExportFeeds.PromoStartDate','Начало акции')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.ExportFeeds.PromoStartDate','Start date')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.ExportFeeds.PromoExpirationDate','Завершение акции')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.ExportFeeds.PromoExpirationDate','Expiration date')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.AddEditYandexPromo.NewPromoCode','Промокод')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.AddEditYandexPromo.NewPromoCode','Promo code')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.AddEditYandexPromo.Name','Название')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.AddEditYandexPromo.Name','Name')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.AddEditYandexPromo.Description','Описание')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.AddEditYandexPromo.Description','Description')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.AddEditYandexPromo.PromoUrl','Ссылка на акцию')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.AddEditYandexPromo.PromoUrl','Promo Url')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.AddEditYandexPromo.Coupon','Купон')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.AddEditYandexPromo.Coupon','Coupon')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.AddEditYandexPromo.Ok','Ок')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.AddEditYandexPromo.Ok','Ok')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.AddEditYandexPromo.Cancel','Отмена')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.AddEditYandexPromo.Cancel','Cancel')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.AddEditYandexPromo.AddCoupon','Добавить купон')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.AddEditYandexPromo.AddCoupon','Add coupon')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.AddEditYandexPromo.ErrorGettingCouponsCount','Нет доступных купонов, добавте новый')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.AddEditYandexPromo.ErrorGettingCouponsCount','No coupons available')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.AddEditYandexPromo.WarningSomeCouponsFiltered','Некоторые купоны были отфильтрованы, т.к. имели код длиннее чем 20 символов')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.AddEditYandexPromo.WarningSomeCouponsFiltered','Some coupons were filtered')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.AddEditYandexPromo.ErrorGettingCouponsAllDisabled','Все купоны отключены')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.AddEditYandexPromo.ErrorGettingCouponsAllDisabled','All coupons disabled')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.AddEditYandexPromo.ErrorGettingCoupons','Ошибка получения купонов')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.AddEditYandexPromo.ErrorGettingCoupons','Obtaining coupons error')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.AddEditYandexPromo.NewPromoFlash','Специальная цена')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.AddEditYandexPromo.NewPromoFlash','Flash discount')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.AddEditYandexPromo.StartDate','Дата начала промоакции')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.AddEditYandexPromo.StartDate','Start date')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.AddEditYandexPromo.ExpirationDate','Дата окончания промоакции')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.AddEditYandexPromo.ExpirationDate','Expiration date')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.AddEditYandexPromo.Products','Продукты участвующие в промоакции')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.AddEditYandexPromo.Products','Products')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.AddEditYandexPromo.NewPromoGift','Подарки')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.AddEditYandexPromo.NewPromoGift','Gifts')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.AddEditYandexPromo.Gift','Подарок')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.AddEditYandexPromo.Gift','Gift')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.AddEditYandexPromo.Add','изменить')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.AddEditYandexPromo.Add','change')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.AddEditYandexPromo.Clear','сбросить')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.AddEditYandexPromo.Clear','clear')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.AddEditYandexPromo.NewPromoNPlusM','N + M')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.AddEditYandexPromo.NewPromoNPlusM','N + M')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.AddEditYandexPromo.RequiredQuantity','Товаров за полную цену')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.AddEditYandexPromo.RequiredQuantity','Required quantity')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.AddEditYandexPromo.FreeQuantity','Товаров в подарок')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.AddEditYandexPromo.FreeQuantity','Free quantity')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.ExportFeeds.JobStartHourMinuteError','Время запуска задано не корректно')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.ExportFeeds.JobStartHourMinuteError','Job startup time is not correct.')

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Описание магазина' WHERE [ResourceKey] = 'Admin.ExportFeeed.SettingsGoogle.DatafeedDescription' AND [LanguageId] = '1'
UPDATE [Settings].[Localization] SET [ResourceValue] = 'Категория товаров google по умолчанию' WHERE [ResourceKey] = 'Admin.ExportFeeed.SettingsGoogle.GoogleProductCategory' AND [LanguageId] = '1'
UPDATE [Settings].[Localization] SET [ResourceValue] = 'Рекомендованная наценка на товар %' WHERE [ResourceKey] = 'Admin.ExportFeeed.SettingsReseller.RecomendedPriceMargin' AND [LanguageId] = '1'


GO--

EXEC sp_RENAME 'Catalog.Product.Cbid' , 'Bid', 'COLUMN'


update [Settings].[Localization] set ResourceKey = 'Core.ExportImport.ProductFields.Bid' where  ResourceKey = 'Core.ExportImport.ProductFields.Cbid'

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
	 @Bid float,    
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
			 ,Bid   
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
			 @Bid,   
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
	 @Bid float,    
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
	 ,[Bid] = @Bid   
	 ,[AccrueBonuses] = @AccrueBonuses
	 ,[TaxId] = @TaxId 
	 ,[YandexSizeUnit] = @YandexSizeUnit
	 ,[YandexName] = @YandexName
	 ,[YandexDeliveryDays] = @YandexDeliveryDays
	 ,[CreatedBy] = @CreatedBy
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
	,@onlyMainOfferToExport bit
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
					AND (@onlyMainOfferToExport = 0 OR Offer.Main = 1)
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
			,[Product].Bid
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
			AND (@onlyMainOfferToExport = 0 OR Offer.Main = 1)
	END
END

GO--

ALTER TABLE Catalog.Coupon ADD
	TriggerId int NULL

GO--

ALTER TABLE [Order].[Order] ADD
	LpId int NULL
GO--

Insert Into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (1, 'Core.Services.Triggers.ETriggerEventType.OrderPaied', 'Заказ оплачен')
Insert Into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (1, 'Core.Services.Triggers.ETriggerEventType.Description.OrderPaied', 'Триггер при оплате заказа')

Insert Into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (2, 'Core.Services.Triggers.ETriggerEventType.OrderPaied', 'Order has been paied')
Insert Into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (2, 'Core.Services.Triggers.ETriggerEventType.Description.OrderPaied', 'Trigger on pay order')

GO--

ALTER TABLE CMS.LandingForm ADD InputTextPosition int NULL

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Crm.EFieldType.GroupName.Client', 'Клиент')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Crm.EFieldType.GroupName.Client', 'Customer')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Crm.EFieldType.GroupName.Order', 'Заказ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Crm.EFieldType.GroupName.Order', 'Order')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Crm.EFieldType.GroupName.Lead', 'Лид')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Crm.EFieldType.GroupName.Lead', 'Lead')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Crm.EOrderFieldType.Manager', 'Менеджер')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Crm.EOrderFieldType.Manager', 'Manager')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Crm.EOrderFieldType.Products', 'Заказ содержит товары')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Crm.EOrderFieldType.Products', 'Order with products')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Crm.EOrderFieldType.Categories', 'Заказ содержит товары из категорий')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Crm.EOrderFieldType.Categories', 'Order with categories')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Crm.ELeadFieldType.Manager', 'Менеджер')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Crm.ELeadFieldType.Manager', 'Manager')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Crm.ELeadFieldType.Products', 'Лид содержит товары')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Crm.ELeadFieldType.Products', 'Lead with products')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Crm.ELeadFieldType.Categories', 'Лид содержит товары из категорий')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Crm.ELeadFieldType.Categories', 'Lead with categories')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Crm.ELeadFieldType.CustomerSegment', 'Сегмент покупателей')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Crm.ELeadFieldType.CustomerSegment', 'Customers segment')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Crm.ELeadFieldType.OrdersPaidSum', 'Сумма оплаченных заказов')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Crm.ELeadFieldType.OrdersPaidSum', 'Sum of paid orders')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Crm.ELeadFieldType.OrdersCount', 'Кол-во оформленных заказов')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Crm.ELeadFieldType.OrdersCount', 'Count of orders')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Crm.ELeadFieldType.OrdersPaidCount', 'Кол-во оплаченных заказов')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Crm.ELeadFieldType.OrdersPaidCount', 'Count of paid orders')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Crm.ELeadFieldType.ProductsByCustomer', 'Товары купленные покупателем')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Crm.ELeadFieldType.ProductsByCustomer', 'Products purchased by customer')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Crm.ELeadFieldType.CategoriesByCustomer', 'Товары купленные по категориям')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Crm.ELeadFieldType.CategoriesByCustomer', 'Categories of products purchased by customer')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Services.Triggers.ETriggerEventType.TimeFromLastOrder', 'Время с последнего заказа')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Services.Triggers.ETriggerEventType.TimeFromLastOrder', 'Time from last order')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Services.Triggers.ETriggerEventType.Description.TimeFromLastOrder', 'Триггер срабатывает через n дней с последнего заказа')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Services.Triggers.ETriggerEventType.Description.TimeFromLastOrder', 'Time from last order')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Services.Triggers.ETriggerEventType.SignificantDate', 'Важные даты')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Services.Triggers.ETriggerEventType.SignificantDate', 'Significant date')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Services.Triggers.ETriggerEventType.Description.SignificantDate', 'Триггер срабатывает по дате')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Services.Triggers.ETriggerEventType.Description.SignificantDate', 'Trigger by significant date')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Services.Triggers.ETriggerEventType.SignificantCustomerDate', 'Важные даты покупателя')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Services.Triggers.ETriggerEventType.SignificantCustomerDate', 'Significant date of customer')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Services.Triggers.ETriggerEventType.Description.SignificantCustomerDate', 'Триггер срабатывает по дате')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Services.Triggers.ETriggerEventType.Description.SignificantCustomerDate', 'Trigger by significant date')

GO--

Insert Into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (1, 'Admin.SettingsCrm.Index.WhatsApp.HelpText', 'Укажите номер телефона в числовом формате (без "+", "-", скобок и .т.п.)')
Insert Into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (2, 'Admin.SettingsCrm.Index.WhatsApp.HelpText', 'Enter the phone number in numeric format (without "+", "-", brackets etc.)')
GO--

if (exists(select * from [Settings].[Settings] where Name = 'SettingsSocialWidget.LinkWhatsApp') and not exists(select * from [Settings].[Settings] where Name = 'SettingsSocialWidget.WhatsAppPhone'))
begin
	declare @val nvarchar(max) = (select Value from [Settings].[Settings] where Name = 'SettingsSocialWidget.LinkWhatsApp');
	declare @ind int = CHARINDEX('=', @val);
	if (@ind <> 0)
	begin
		insert into [Settings].[Settings] (Name, Value) values ('SettingsSocialWidget.WhatsAppPhone', SUBSTRING(@val, @ind + 1, LEN(@val) - @ind + 1))
	end
end

GO--

Update [Settings].[Localization] Set ResourceValue = 'Есть аккаунт ВКонтакте, Instagram, Telegram' Where ResourceKey = 'Admin.CustomerSegments.Filters.HaveAnAccount' and LanguageId = 1 
Update [Settings].[Localization] Set ResourceValue = 'Have an account on Vk, Instagram, Telegram' Where ResourceKey = 'Admin.CustomerSegments.Filters.HaveAnAccount' and LanguageId = 2 

Update [Settings].[Localization] Set ResourceValue = 'Есть аккаунт ВКонтакте, Instagram, Telegram' Where ResourceKey = 'Admin.Js.Customers.HaveAccountOnVKandFB' and LanguageId = 1 
Update [Settings].[Localization] Set ResourceValue = 'Have an account on Vk, Instagram, Telegram' Where ResourceKey = 'Admin.Js.Customers.HaveAccountOnVKandFB' and LanguageId = 2 

Update [Settings].[Localization] Set [ResourceValue] = 'Cообщение из Вконтакте, Instagram, Telegram' WHERE [ResourceKey] = 'Core.Services.EBizProcessEventType.MessageReply' and [LanguageId] = 1

GO--

ALTER TABLE CRM.TriggerAction ADD EmailingId uniqueidentifier NULL
GO--
UPDATE CRM.TriggerAction SET EmailingId = NEWID()
GO--
ALTER TABLE CRM.TriggerAction ALTER COLUMN EmailingId uniqueidentifier NOT NULL
GO--
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Analytics.Trigger', 'Аналитика email рассылки')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Analytics.Trigger', 'Emailing analytics')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ManualEmailings.Title', 'Аналитика ручных email рассылок')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ManualEmailings.Title', 'Manual emailings analytics')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Home.Menu.ManualEmailings', 'Аналитика ручных email рассылок')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Home.Menu.ManualEmailings', 'Manual emailings analytics')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.ManualEmailings.Subject', 'Тема письма')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.ManualEmailings.Subject', 'Subject')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.ManualEmailings.DateCreated', 'Дата')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.ManualEmailings.DateCreated', 'Date')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.ManualEmailings.TotalCount', 'Отправлено писем')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.ManualEmailings.TotalCount', 'Sent emails count')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.ManualEmailings.DeleteSelected','Удалить выделенные')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.ManualEmailings.DeleteSelected','Delete selected')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.ManualEmailings.AreYouSureDelete','Вы уверены, что хотите удалить?')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.ManualEmailings.AreYouSureDelete','Are you sure you want to delete?')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.ManualEmailings.Deleting','Удаление')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.ManualEmailings.Deleting','Deleting')
GO--

CREATE TABLE [Customers].[ManualEmailing](
	[Id] [uniqueidentifier] NOT NULL,
	[Subject] [nvarchar](255) NOT NULL,
	[TotalCount] [int] NOT NULL,
	[DateCreated] [datetime] NOT NULL,
 CONSTRAINT [PK_ManualEmailing] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO--

ALTER TABLE CRM.TriggerAction ADD
	EditFieldValue nvarchar(MAX) NULL,
	EditFieldType int NULL,
	ObjId int NULL
	
GO--

ALTER TABLE CRM.TriggerRule ADD
	EventObjValue int NULL

GO--

ALTER TABLE CRM.TriggerRule ADD
	ProcessType int NULL
GO--

Update CRM.TriggerRule Set ProcessType = 0
GO--

ALTER TABLE CRM.TriggerRule
ALTER COLUMN ProcessType INT NOT NULL

GO--

ALTER TABLE CRM.TriggerRule ADD
	TriggerParams nvarchar(MAX) NULL
GO--
	

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.Modules.DeactivatedAndPayable', 'Модуль деактивирован, но все еще установлен в магазине и списания за него продолжатся.<br/>Чтобы полностью отключить модуль, необходимо удалить его из магазина.')
UPDATE [Settings].[Localization] SET [ResourceValue] = 'Модуль деактивирован' WHERE [ResourceKey] = 'Admin.Js.ModuleIsNotActive' AND [LanguageId] = '1'
UPDATE [Settings].[Localization] SET [ResourceValue] = 'Модуль деактивирован' WHERE [ResourceKey] = 'Admin.Js.Module.ModuleIsNotActive' AND [LanguageId] = '1'

GO--

ALTER FUNCTION [Settings].[ParsingBySeperator] (@SourceString nvarchar(max),@Delimeter nvarchar(max))
   RETURNS @tbl TABLE (item nvarchar(max), sort int) AS
BEGIN
DECLARE @item  nvarchar(max)
Declare @count int
set @count=0
Declare @LenDelimeter int
set @LenDelimeter = LEN(@Delimeter)
DECLARE @StartPos int, @Length int
WHILE LEN(@SourceString) > 0
  BEGIN
    SET @StartPos = CHARINDEX(@Delimeter, @SourceString)
    IF @StartPos < 0 SET @StartPos = 0
    SET @Length = LEN(@SourceString) - @StartPos - 1
    IF @Length < 0 SET @Length = 0
    IF @StartPos > 0
      BEGIN
        SET @item = SUBSTRING(@SourceString, 1, @StartPos - 1)
        SET @SourceString = SUBSTRING(@SourceString, @StartPos + @LenDelimeter, LEN(@SourceString) - @StartPos)
      END
    ELSE
      BEGIN
        SET @item = @SourceString
        SET @SourceString = ''
      END
	set @count=@count+1
    INSERT @tbl (item,sort) VALUES(@item,@count)
END
RETURN
END

GO--

ALTER TABLE CRM.TriggerRule ADD
	PreferredHour int NULL
GO--


INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ExportFeeed.SettingsYandex.ShopSettings','Настройки магазина')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ExportFeeed.SettingsYandex.ShopSettings','Shop settings')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ExportFeeed.SettingsYandex.DeliverySettings','Настройки доставки')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ExportFeeed.SettingsYandex.DeliverySettings','Delivery settings')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ExportFeeed.SettingsYandex.PricesAndCurrenciesSettings','Настройки цен и валюты')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ExportFeeed.SettingsYandex.PricesAndCurrenciesSettings','Prices and Currencies settings')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ExportFeeed.SettingsYandex.ProductsSettings','Настройки товаров')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ExportFeeed.SettingsYandex.ProductsSettings','Products settings')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ExportFeed.SettingsYandex.StoreHelp','Если у вас есть точка продаж (магазин), активируйте эту настройку')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ExportFeed.SettingsYandex.StoreHelp','If you have a store, activate this setting')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ExportFeed.SettingsYandex.PickupHelp','Если у вас есть пункт самовывоза, активируйте эту настройку')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ExportFeed.SettingsYandex.PickupHelp','If you have a pick-up point, activate this setting')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ExportFeed.SettingsYandex.DeliveryHelp','Если вы осуществляете доставку товаров покупателям, активируйте эту настройку')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ExportFeed.SettingsYandex.DeliveryHelp','If you are delivering goods to customers, activate this setting.')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ExportFeed.SettingsYandex.LocalDeliveryCostHelp','При использовании этой настройки в прайс-лист для каждого товара будут выгружаться те настройки стоимости и сроков доставки, которые заданы для товара в его редактировании.<br><br>Если в редактировании какого-либо товара для него не будут заданы настройки стоимости и сроков доставки, для такого товара будут использоваться значения стоимости и сроков доставки из этой настройки')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ExportFeed.SettingsYandex.LocalDeliveryCostHelp','When using this setting - for each product in your price list, product''s cost and delivery time settings will be as they specified for the product in its editing.<br><br>If the cost and delivery time settings are not specified, the cost and delivery time values from this setting will be used for that product.')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ExportFeed.SettingsYandex.GlobalDeliveryCostHelp','Эти настройки стоимости и сроков доставки будут использоваться сразу для всех товаров в прайс-листе')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ExportFeed.SettingsYandex.GlobalDeliveryCostHelp','These cost and delivery time settings will be used immediatly for all products in the price list')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ExportFeed.SettingsYandex.ExportPurchasePriceHelp','При активации настройки закупочная цена товаров будет выгружаться в прайс-лист в теге ''purchase_price''')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ExportFeed.SettingsYandex.ExportPurchasePriceHelp','When you activate the settings, the purchase price of the goods will be uploaded to the price list in the ''purchase_price'' tag')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ExportFeed.SettingsYandex.TypeExportYandexHelp','Установите данную настройку, если вы хотите, чтобы для всех товаров в прайс-листе использовался т.н. упрощённый тип описания.<br><br>При упрощённом типе описания название производителя товара не добавляется к названию товара в Яндекс.Маркете (тег ''vendor'' с именем производителя не выгружается в прайс-лист)')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ExportFeed.SettingsYandex.TypeExportYandexHelp','Set this setting if you want a simplified description type to be used for all products in the price list.<br><br>With a simplified description type, the name of the manufacturer of the product is not added to the name of the product in Yandex.Market (the ''vendor'' tag with the name of the manufacturer is not uploaded to the price list)')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ExportFeed.SettingsYandex.ExportAllPhotosHelp','Если настройка не активирована, в прайс-лист выгрузится ссылка только на основное фото товара.<br><br>Если настройка активирована, в прайс-лист выгрузятся ссылки на все фото товара.<br><br>Внимание! Яндекс.Маркет допускает указание максимум 10 ссылок на фото для каждого товара в прайс-листе.')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ExportFeed.SettingsYandex.ExportAllPhotosHelp','If the setting is not activated, only the main product photo will be downloaded to the price list.<br><br>If the setting is activated, links to all product photos will be downloaded to the price list.<br><br>Attention! Yandex.Market allows the indication of a maximum of 10 links to a photo for each product in the price list.')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ExportFeed.SettingsYandex.ExportRelatedProductsHelp','Если настройка активирована, в прайс-листе у товаров в теге ''rec'' будут указаны артикулы рекомендованных к ним товаров')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ExportFeed.SettingsYandex.ExportRelatedProductsHelp','If the setting is activated, in the price list for the goods in the ''rec'' tag will be indicated the articles of the products recommended for them')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ExportFeed.SettingsYandex.ExportProductDiscountHelp','При активации настройки в прайс-лист выгрузятся итоговая цена товара с учётом скидки и старая цена (без скидки) в теге ''oldprice''')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ExportFeed.SettingsYandex.ExportProductDiscountHelp','When the settings are activated, the final price of the product, taking into account the discount and the old price (without discount) in the ''oldprice'' tag, will be downloaded to the price list')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ExportFeed.Settings.PriceMarginHelp','В файле выгрузки цена товаров будет указана с данной наценкой (по сравнению с ценой в магазине).<br><br>Внимание! Если вы используете текущую выгрузку для Яндекс.Маркета - наценку устанавливать нельзя, так как в этом случае Яндекс.Маркет не примет ваш прайс-лист, из-за несоответствия цен в прайс листе и в магазине; оставьте значение наценки, равное нулю.')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ExportFeed.Settings.PriceMarginHelp','In the upload file, the price of the goods will be indicated with this mark-up (as compared with the price in the store).<br><br>Attention! If you use the current unloading for Yandex.Market - you cannot set a markup, since in this case Yandex.Market will not accept your price list, due to inconsistencies between prices in the price list and in the store; Leave the markup value equal to zero.')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ExportFeeed.Settings.AdvancedSettings','Advanced settings')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ExportFeeed.ChoiceOfProducts.Export','Export')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ExportFeeed.СhoiceOfProducts.ChoiceOfCategories','Choice of categories')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ExportFeeed.Settings.Name','Name')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ExportFeeed.Settings.Description','Description')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ExportFeeed.Settings.Active','Active')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ExportFeeed.Settings.Interval','Interval')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Scheduler.TimeIntervalType.Days','Days')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Scheduler.TimeIntervalType.Hours','Hours')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ExportFeeed.Settings.FileName','File name')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ExportFeeed.Settings.PriceMargin','Price margin')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ExportFeeed.SettingsGoogle.Currency','Currency')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ExportFeeed.SettingsGoogle.RemoveHtml','Remove html')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ExportFeeed.SettingsGoogle.DatafeedTitle','Datafeed title')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ExportFeeed.SettingsGoogle.DatafeedDescription','Datafeed description')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ExportFeeed.SettingsGoogle.GoogleProductCategory','Google product category')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ExportFeeed.SettingsGoogle.AllowPreOrderProducts','Allow preorder products')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ExportFeeed.SettingsGoogle.ProductDescriptionType','Product description type')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ExportFeeed.SettingsGoogle.OfferIdType','Offer ID type')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ExportFeeed.SettingsCsv.CsvEnconing','Csv enconing')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ExportFeeed.SettingsCsv.CsvSeparator','Csv separator')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ExportFeeed.SettingsCsv.CsvColumSeparator','Csv colum separator')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ExportFeeed.SettingsCsv.CsvPropertySeparator','Csv property separator')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ExportFeeed.SettingsCsv.CsvExportNoInCategory','Unload product without categories')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ExportFeeed.SettingsCsv.CsvCategorySort','Export category sort')

Update [Settings].[Localization] set [ResourceValue] =  'Additional url tags (UTM-marks)' where [LanguageId] = 2 and [ResourceKey] = 'Admin.ExportFeed.Settings.AdditionalUrlTags'
Update [Settings].[Localization] set [ResourceValue] =  'Есть точка продаж' where [LanguageId] = 1 and [ResourceKey] = 'Admin.ExportFeed.SettingsYandex.Store'
Update [Settings].[Localization] set [ResourceValue] =  'Есть возможность самовывоза' where [LanguageId] = 1 and [ResourceKey] = 'Admin.ExportFeeed.SettingsYandex.Pickup'
Update [Settings].[Localization] set [ResourceValue] =  'Есть возможность доставки' where [LanguageId] = 1 and [ResourceKey] = 'Admin.ExportFeeed.SettingsYandex.Delivery'
Update [Settings].[Localization] set [ResourceValue] =  'Выгружать стоимость доставки в файл' where [LanguageId] = 1 and [ResourceKey] = 'Admin.ExportFeeed.SettingsYandex.DeliveryCost'
Update [Settings].[Localization] set [ResourceValue] =  'Индивидуальные настройки стоимости доставки по умолчанию' where [LanguageId] = 1 and [ResourceKey] = 'Admin.ExportFeed.SettingsYandex.LocalDeliveryCost'
Update [Settings].[Localization] set [ResourceValue] =  'Общие настройки стоимости доставки для всех товаров' where [LanguageId] = 1 and [ResourceKey] = 'Admin.ExportFeed.SettingsYandex.GlobalDeliveryCost'
Update [Settings].[Localization] set [ResourceValue] =  'Выгружать цену с учётом скидки' where [LanguageId] = 1 and [ResourceKey] = 'Admin.ExportFeeed.SettingsYandex.ExportProductDiscount'
Update [Settings].[Localization] set [ResourceValue] =  'Использовать упрощённый тип описания для товаров' where [LanguageId] = 1 and [ResourceKey] = 'Admin.ExportFeeed.SettingsYandex.TypeExportYandex'
Update [Settings].[Localization] set [ResourceValue] =  'Добавлять к названию товара цвет и размер' where [LanguageId] = 1 and [ResourceKey] = 'Admin.ExportFeeed.SettingsYandex.ColorSizeToName'
Update [Settings].[Localization] set [ResourceValue] =  'Какое описание товара следует использовать' where [LanguageId] = 1 and [ResourceKey] = 'Admin.ExportFeeed.SettingsYandex.ProductDescriptionType'
Update [Settings].[Localization] set [ResourceValue] =  'Удалять HTML из описания товара' where [LanguageId] = 1 and [ResourceKey] = 'Admin.ExportFeeed.SettingsYandex.RemoveHtml'
Update [Settings].[Localization] set [ResourceValue] =  'Упаковать прайс-лист в zip архив' where [LanguageId] = 1 and [ResourceKey] = 'Admin.ExportFeeed.SettingsYandex.NeedZip'
Update [Settings].[Localization] set [ResourceValue] =  'Из индивидуальных настроек стоимости доставки товара' where [LanguageId] = 1 and [ResourceKey] = 'Core.ExportImport.ExportFeedYandexDeliveryCost.LocalDeliveryCost'
Update [Settings].[Localization] set [ResourceValue] =  'Из общих настроек стоимости доставки' where [LanguageId] = 1 and [ResourceKey] = 'Core.ExportImport.ExportFeedYandexDeliveryCost.GlobalDeliveryCost'

GO--

Update [Settings].[Localization] Set [ResourceValue] = 'Триггерный маркетинг' Where [ResourceKey] = 'Admin.Triggers.Index.Title' and [LanguageId] = 1
Update [Settings].[Localization] Set [ResourceValue] = 'Trigger marketing' Where [ResourceKey] = 'Admin.Triggers.Index.Title' and [LanguageId] = 2

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Triggers.Index.NewTrigger', 'Новый триггер')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Triggers.Index.NewTrigger', 'New trigger')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Triggers.Index.Description', 'Автоматически на основе событий (Триггеров), отправляйте клиентам Email письма, СМС сообщения, а так же выполняйте другие автодействия.')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Triggers.Index.Description', 'Automatically send emails, sms messanges and perform other actions on triggers')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.Triggers.Title', 'Триггерный маркетинг')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.Triggers.Title', 'Trigger marketing')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.Triggers.Trigger', 'Триггер')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.Triggers.Trigger', 'Trigger')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.Triggers.NewTrigger', 'Новый триггер')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.Triggers.NewTrigger', 'New trigger')


GO--


Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (1, 'Core.ChangeHistories.BookingHistory.BookingCreated', 'Создана бронь {0}'); 
Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (2, 'Core.ChangeHistories.BookingHistory.BookingCreated', 'Booking {0} has been created'); 

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (1, 'Core.ChangeHistories.BookingHistory.BookingDeleted', 'Удалена бронь {0}'); 
Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (2, 'Core.ChangeHistories.BookingHistory.BookingDeleted', 'Booking {0} has been deleted'); 

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (1, 'Core.ChangeHistories.OnRefreshTotalBooking.Sum', 'Сумма брони'); 
Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (1, 'Core.Booking.BookingItem.Price', 'Цена'); 
Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (1, 'Core.Booking.BookingItem.Amount', 'Количество'); 
Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (1, 'Core.Booking.Booking.BeginDate', 'Дата начала'); 
Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (1, 'Core.Booking.Booking.EndDate', 'Дата окончания'); 
Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (1, 'Core.Booking.Booking.Status', 'Статус'); 
Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (1, 'Core.Booking.Booking.Sum', 'Сумма брони'); 
Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (1, 'Core.Booking.Booking.Manager', 'Менеджер'); 
Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (1, 'Core.Booking.Booking.OrderSource', 'Источник'); 

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (2, 'Core.ChangeHistories.OnRefreshTotalBooking.Sum', 'Sum'); 
Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (2, 'Core.Booking.BookingItem.Price', 'Price'); 
Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (2, 'Core.Booking.BookingItem.Amount', 'Amount'); 
Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (2, 'Core.Booking.Booking.BeginDate', 'Start date'); 
Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (2, 'Core.Booking.Booking.EndDate', 'Expiration date'); 
Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (2, 'Core.Booking.Booking.Status', 'Status'); 
Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (2, 'Core.Booking.Booking.Sum', 'Sum'); 
Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (2, 'Core.Booking.Booking.Manager', 'Manager'); 
Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (2, 'Core.Booking.Booking.OrderSource', 'Source'); 

GO--

ALTER TABLE Catalog.Coupon ADD
	Mode int NULL
GO--

Update Catalog.Coupon Set Mode=0

GO--

ALTER TABLE Catalog.Coupon ADD
	TriggerActionId int NULL
GO--

Update [Settings].[Localization] Set [ResourceValue] = 'Создание страницы' Where [LanguageId] = 1 and [ResourceKey] = 'Admin.Js.AddLanding.Title'

Update [Settings].[Localization] Set [ResourceValue] = 'Creating a page' Where [LanguageId] = 2 and [ResourceKey] = 'Admin.Js.AddLanding.Title'

GO--

ALTER TABLE Catalog.Coupon ADD
	Days int NULL
GO--

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (1, 'Core.Crm.EOrderFieldType.IsPaid', 'Оплачен'); 
Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (2, 'Core.Crm.EOrderFieldType.IsPaid', 'Is paid'); 

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Crm.ELeadFieldType.OpenLeadSalesFunnels', 'Наличие лидов в воронке')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Crm.ELeadFieldType.OpenLeadSalesFunnels', 'Availability of leads in the funnel')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Crm.EFieldType.GroupName.System', 'Системные')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Crm.EFieldType.GroupName.System', 'Systemic')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Crm.EOrderFieldType.Datetime', 'До определенной даты и времени')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Crm.EOrderFieldType.Datetime', 'Before a selected date and time')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.SettingsTasks.DDMMYYHHSS','дд.мм.гггг чч:сс')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.SettingsTasks.DDMMYYHHSS','dd.mm.yyyy hh:ss')

GO--

Update [Settings].[Localization] Set [ResourceValue] = 'Создать страницу' Where [LanguageId] = 1 and [ResourceKey] = 'Admin.Js.AddLanding.Add'

Update [Settings].[Localization] Set [ResourceValue] = 'Create a page' Where [LanguageId] = 2 and [ResourceKey] = 'Admin.Js.AddLanding.Add'

GO--

ALTER TABLE CRM.TriggerSendOnceData ADD
	CustomerId uniqueidentifier NULL

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.SystemSettings.AppsTriggersActive','Триггеры')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.SystemSettings.AppsTriggersActive','Triggers')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Home.Menu.Triggers','Триггеры')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Home.Menu.Triggers','Triggers')

GO--

INSERT INTO [Settings].[Settings] ([Name],[Value]) VALUES ('TriggersActive','False')

GO--

Update [Settings].[Localization] Set [ResourceValue] = 'дд.мм.гггг чч:мм', [ResourceKey] = 'Admin.Js.SettingsTasks.DDMMYYHHMM' Where [LanguageId] = 1 and [ResourceKey] = 'Admin.Js.SettingsTasks.DDMMYYHHSS'

Update [Settings].[Localization] Set [ResourceValue] = 'dd.mm.yyyy hh:mm', [ResourceKey] = 'Admin.Js.SettingsTasks.DDMMYYHHMM' Where [LanguageId] = 2 and [ResourceKey] = 'Admin.Js.SettingsTasks.DDMMYYHHSS'

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Crm.ELeadFieldType.DealStatus','Статус лида')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Crm.ELeadFieldType.DealStatus','Deal status')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Crm.EOrderFieldType.OrderStatus','Статус заказа')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Crm.EOrderFieldType.OrderStatus','Order status')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.Triggers.Name','Название')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.Triggers.Name','Name')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.Triggers.Description','Описание')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.Triggers.Description','Description')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.Triggers.SaveNameComplete','Название триггера успешно сохранено')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.Triggers.SaveNameComplete','Trigger name successfully saved')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.Triggers.SaveNameError','Произошла ошибка при сохранении названия триггера')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.Triggers.SaveNameError','An error occurred while saving the name of the trigger')

GO--

CREATE TABLE [CRM].[TriggerCategory](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[SortOrder] [int] NOT NULL,
 CONSTRAINT [PK_TriggerCategory] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO--

ALTER TABLE CRM.TriggerRule ADD
	CategoryId int NULL
	
GO--

ALTER TABLE CRM.TriggerRule ADD CONSTRAINT
	FK_TriggerRule_TriggerCategory FOREIGN KEY
	(
	CategoryId
	) REFERENCES CRM.TriggerCategory
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  SET NULL 
	
GO--

CREATE NONCLUSTERED INDEX Category_TriggerRule ON CRM.TriggerRule
	(
	CategoryId
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Triggers.Categories.Title','Категории')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Triggers.Categories.Title','Categories')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Triggers.Categories.AddCategory','Добавить категорию')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Triggers.Categories.AddCategory','Add category')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.TriggerCategory.AddEdit.NewCategory','Новая категория')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.TriggerCategory.AddEdit.NewCategory','New category')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.TriggerCategory.AddEdit.EditingCategory','Редактирование категории')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.TriggerCategory.AddEdit.EditingCategory','Editing the category')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.TriggerCategory.AddEdit.Name','Название')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.TriggerCategory.AddEdit.Name','Name')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.TriggerCategory.AddEdit.SortingOrder','Порядок сортировки')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.TriggerCategory.AddEdit.SortingOrder','Sorting order')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.TriggerCategory.AddEdit.Add','Добавить')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.TriggerCategory.AddEdit.Add','Add')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.TriggerCategory.AddEdit.Save','Сохранить')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.TriggerCategory.AddEdit.Save','Save')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.TriggerCategories.Categories.AreYouSureDelete','Вы уверены, что хотите удалить?')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.TriggerCategories.Categories.AreYouSureDelete','Are you sure you want to delete?')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.TriggerCategories.Categories.Deleting','Удаление')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.TriggerCategories.Categories.Deleting','Deleting')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.Triggers.Category','Категория')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.Triggers.Category','Category')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Triggers.NavMenu.Triggers','Триггеры')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Triggers.NavMenu.Triggers','Triggers')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Triggers.NavMenu.Categories','Категории')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Triggers.NavMenu.Categories','Categories')

GO--

ALTER TABLE Catalog.Coupon ADD
	CustomerId uniqueidentifier NULL
GO--


ALTER TABLE CRM.TriggerAction ADD
	SmartCallsCampaignID int NULL

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.SettingsTelephony.Index.AutoCalls', 'Автозвонки')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.SettingsTelephony.Index.AutoCalls', 'Auto calls')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.SettingsTelephony.Index.SmartCalls', 'Интеграция со SmartCalls')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.SettingsTelephony.Index.SmartCalls', 'SmartCalls integration')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.SettingsTelephony.Index.SmartCalls.Subdomain', 'Поддомен')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.SettingsTelephony.Index.SmartCalls.Subdomain', 'Subdomain')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.SettingsTelephony.Index.SmartCalls.Username', 'Имя пользователя')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.SettingsTelephony.Index.SmartCalls.Username', 'Username')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.SettingsTelephony.Index.SmartCalls.Password', 'Пароль')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.SettingsTelephony.Index.SmartCalls.Password', 'Password')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.SettingsTelephony.Index.SmartCalls.Login', 'Войти')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.SettingsTelephony.Index.SmartCalls.Login', 'Login')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.SettingsTelephony.Index.SmartCalls.SubdomainHelp', 'Например: advantshop. Указывайте без домена smartcalls.io')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.SettingsTelephony.Index.SmartCalls.SubdomainHelp', 'Like: advantshop. Use subdomain without smartcalls.io')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.SettingsTelephony.Index.SmartCalls.Account', 'Привязанный аккаунт:')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.SettingsTelephony.Index.SmartCalls.Account', 'Account:')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.SettingsTelephony.Index.SmartCalls.DeActivate', 'Удалить привязку')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.SettingsTelephony.Index.SmartCalls.DeActivate', 'Deactivate')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.SettingsTelephony.AreYouSureDeActivate', 'Вы уверены что хотите удалить привязку?')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.SettingsTelephony.AreYouSureDeActivate', 'Are you sure deactivate?')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.SettingsTelephony.DeActivating', 'Удаление привязки')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.SettingsTelephony.DeActivating', 'Deactivating')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.SettingsTelephony.Index.SmartCalls.ApiUrls', 'Генерация HTTP запросов для использования в SmartCalls')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.SettingsTelephony.Index.SmartCalls.ApiUrls', 'Generation HTTP requests for SmartCalls')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.SettingsTelephony.Index.SmartCalls.ApiLeadVerify', 'Смена статуса лида')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.SettingsTelephony.Index.SmartCalls.ApiLeadVerify', 'Lead verify')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.SettingsTelephony.Index.SmartCalls.ApiOrderVerify', 'Смена статуса заказа')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.SettingsTelephony.Index.SmartCalls.ApiOrderVerify', 'Order verify')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.SettingsTelephony.Index.SmartCalls.SelectFunnel', 'Выберите воронку')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.SettingsTelephony.Index.SmartCalls.SelectFunnel', 'Select funnel')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.SettingsTelephony.Index.SmartCalls.SelectLeadStatus', 'Выберите статус лида')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.SettingsTelephony.Index.SmartCalls.SelectLeadStatus', 'Select lead status')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.SettingsTelephony.Index.SmartCalls.SelectOrderStatus', 'Выберите статус заказа')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.SettingsTelephony.Index.SmartCalls.SelectOrderStatus', 'Select order status')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.Triggers.CategoryName', 'Категория')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.Triggers.CategoryName', 'Category')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.EmailingLog.Title', 'Таблица рассылки')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.EmailingLog.Title', 'Emailing log')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.EmailingLog.Email', 'Email')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.EmailingLog.Email', 'Email')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.EmailingLog.Created', 'Дата')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.EmailingLog.Created', 'Date')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.EmailingLog.Subject', 'Тема письма')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.EmailingLog.Subject', 'Subject')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.EmailingLog.Status', 'Статус')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.EmailingLog.Status', 'Status')
UPDATE [Settings].[Localization] SET [ResourceValue] = 'Доставка невозможна' WHERE [ResourceKey] = 'Core.Loging.EmailStatus.HardBounced' AND [LanguageId] = 1
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Analytics.EmailingAnalytics', 'Аналитика Email')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Analytics.EmailingAnalytics', 'Email analytics')

GO--

ALTER TABLE CRM.TriggerAction ADD
	DealStatusId int NULL

GO--

INSERT INTO [Settings].[Settings] ([Name],[Value]) Values ('GTMOfferIdType', 'id')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Checkout.ScriptsGTMOfferIdType', 'Выгружать в качестве идентификатора товарного предложения')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Checkout.ScriptsGTMOfferIdType', 'Use as product offer identifier')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Checkout.ScriptsGTMOfferId', 'Идентификатор модификации')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Checkout.ScriptsGTMOfferId', 'Modification ID')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Checkout.ScriptsGTMOfferArtno', 'Артикул модификации')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Checkout.ScriptsGTMOfferArtno', 'Modification vendor code')
GO--

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (1, 'Compare.Index.СlearList', 'Очистить список'); 
Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (2, 'Compare.Index.СlearList', 'Clear'); 
Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (1, 'Compare.Index.EmptyList.Info', 'Для добавления товаров к сравнению'); 
Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (2, 'Compare.Index.EmptyList.Info', 'To add products to the comparison'); 

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (1, 'Compare.Index.EmptyList.Info.Link', 'Перейдите в каталог'); 
Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (2, 'Compare.Index.EmptyList.Info.Link', 'Go to the catalog'); 

GO--

Update [Settings].[Localization] set [ResourceValue] = 'Список сравнения пуст' where [LanguageId] = 1 and [ResourceKey] = 'Compare.Index.EmptyList'
Update [Settings].[Localization] set [ResourceValue] = 'Comparison list is empty' where [LanguageId] = 2 and [ResourceKey] = 'Compare.Index.EmptyList'

GO--

ALTER TABLE [Booking].[Service] ADD
	[ArtNo] nvarchar(100) NULL

GO--

UPDATE [Booking].[Service] SET [ArtNo] = [Id]

GO--

ALTER TABLE [Booking].[Service] ALTER COLUMN
	[ArtNo] nvarchar(100) NOT NULL

GO--

CREATE UNIQUE NONCLUSTERED INDEX [ArtNo_Service] ON [Booking].[Service]
(
	[ArtNo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO--

ALTER TABLE [Booking].[BookingItems] ADD
	[ArtNo] nvarchar(100) NULL

GO--

UPDATE [Booking].[BookingItems] SET [ArtNo] = [ServiceId]

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.BookingServices.ArtNo', 'Артикул')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.BookingServices.ArtNo', 'ArtNo')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.AddUpdateBookingService.ArtNo', 'Артикул')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.AddUpdateBookingService.ArtNo', 'ArtNo')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.BookingJournal.ArtNo', 'Арт.')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.BookingJournal.ArtNo', 'ArtNo')

GO--


INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Js.Order.AmountShort', 'Кол-во')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Js.Order.AmountShort', 'Amount')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Js.Order.CostShort', 'Стоим.')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Js.Order.CostShort', 'Cost')

GO--

ALTER PROCEDURE [Settings].[sp_GetExportFeedProducts] 
     @exportFeedId int
	,@onlyCount BIT
	,@exportNotAvailable bit
	,@selectedCurrency NVARCHAR(10)
	,@allowPreOrder bit = 0
	,@exportAllProducts bit
	,@onlyMainOfferToExport bit
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
		SELECT COUNT(Distinct OfferId)
		FROM [Catalog].[Product] p 
		INNER JOIN [Catalog].[Offer] offer ON offer.[ProductID] = p.[ProductID]
		INNER JOIN [Catalog].[ProductCategories] productCategories ON productCategories.[ProductID] = p.[ProductID]
		RIGHT JOIN  @lcategory lc ON lc.CategoryId = productCategories.CategoryID
		LEFT JOIN [Settings].[ExportFeedExcludedProducts]ep ON ep.ProductId = p.ProductId and ep.ExportFeedId=@exportFeedId															
		WHERE 
		(
			ep.ProductID IS NULL 
			OR 
			@exportAllProducts = 1
		)
		--AND
		--(	Exists(select 1 from @lcategory where CategoryId = productCategories.CategoryID)
		--	or 
		--	Exists(select 1 from @lproduct where productId = productCategories.ProductID)
		--)
		--AND		
		--(
		--	SELECT TOP (1) [ProductCategories].[CategoryId]
		--	FROM [Catalog].[ProductCategories]
		--	INNER JOIN @lcategory lc ON lc.[CategoryId] = [ProductCategories].[CategoryId] and [ProductID] = p.[ProductID]
		--	Order By Main DESC, [ProductCategories].[CategoryId] 
		--) = productCategories.[CategoryId]
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
	ELSE
	BEGIN	
		SELECT p.[Enabled]
			,p.[ProductID]
			,p.[Discount]
			,p.[DiscountAmount]
			,AllowPreOrder
			,Amount
			,[ProductCategories].[CategoryId] AS [ParentCategory]
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
		FROM [Catalog].[Product] p 
		INNER JOIN [Catalog].[Offer] offer ON offer.[ProductID] = p.[ProductID]
		INNER JOIN [Catalog].[ProductCategories] productCategories ON productCategories.[ProductID] = p.[ProductID]
		RIGHT JOIN  @lcategory lc ON lc.CategoryId = productCategories.CategoryID
		LEFT JOIN [Settings].[ExportFeedExcludedProducts]ep ON ep.ProductId = p.ProductId and ep.ExportFeedId=@exportFeedId		
		
		--LEFT JOIN [Catalog].[Photo] ON [Product].[ProductID] = [Photo].[ObjId] and Type ='Product' AND [Photo].[Main] = 1
		LEFT JOIN [Catalog].[Color] ON [Color].ColorID = [Offer].ColorID
		LEFT JOIN [Catalog].[Size] ON [Size].SizeID = [Offer].SizeID
		LEFT JOIN [Catalog].Brand ON Brand.BrandID = p.BrandID
		LEFT JOIN [Customers].Country as country1 ON Brand.CountryID = country1.CountryID
		LEFT JOIN [Customers].Country as country2 ON Brand.CountryOfManufactureID = country2.CountryID
		INNER JOIN [Catalog].Currency ON Currency.CurrencyID = p.CurrencyID
		WHERE 
		(
			ep.ProductID IS NULL 
			OR 
			@exportAllProducts = 1
		)
		--AND
		--(	Exists(select 1 from @lcategory where CategoryId = productCategories.CategoryID)
		--	or 
		--	Exists(select 1 from @lproduct where productId = productCategories.ProductID)
		--)
		AND		
		(
			SELECT TOP (1) [ProductCategories].[CategoryId]
			FROM [Catalog].[ProductCategories]
			INNER JOIN @lcategory lc ON lc.[CategoryId] = [ProductCategories].[CategoryId] and [ProductID] = p.[ProductID]
			Order By Main DESC, [ProductCategories].[CategoryId] 
		) = productCategories.[CategoryId]
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

UPDATE [Settings].[InternalSettings] SET [settingValue] = '7.0.0' WHERE [settingKey] = 'db_version'