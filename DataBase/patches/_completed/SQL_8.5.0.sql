INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Mobile.MobileVersion.OpenOnMobile.Title', 'Открой на мобильном')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Mobile.MobileVersion.OpenOnMobile.Title', 'Open on mobile')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Mobile.MobileVersion.OpenOnMobile.Subtitle', 'Сканируйте QR на мобильном устройстве, чтобы быстро перейти в мобильную версию.')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Mobile.MobileVersion.OpenOnMobile.Subtitle', 'Scan QR on your mobile device to quickly go to the mobile version.')

GO--

if not exists (select * from [Settings].[Localization] where ResourceKey = 'Catalog.Index.EditInAdminArea')
begin
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Catalog.Index.EditInAdminArea', 'Редактировать категорию через панель администрирования')
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Catalog.Index.EditInAdminArea', 'Edit category via admin panel')
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'News.NewsItem.EditInAdminArea', 'Редактировать новость через панель администрирования')
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'News.NewsItem.EditInAdminArea', 'Edit news via admin panel')
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'StaticPage.Index.EditInAdminArea', 'Редактировать статическую страницу через панель администрирования')
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'StaticPage.Index.EditInAdminArea', 'Edit static page via admin panel')
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Brand.BrandItem.EditInAdminArea', 'Редактировать производителя через панель администрирования')
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Brand.BrandItem.EditInAdminArea', 'Edit brand via admin panel')
end

GO--

DELETE FROM [Settings].[TemplateSettings] WHERE [Name] = 'Mobile_CategoryMenuIcons'

GO--

DELETE FROM [Settings].[Localization] WHERE ResourceKey = 'Core.Services.Features.EFeature.ProductCategoriesAutoMapping'
DELETE FROM [Settings].[Localization] WHERE ResourceKey = 'Core.Services.Features.EFeature.ProductCategoriesAutoMapping.Description'
DELETE FROM Settings.Settings WHERE Name = 'Features.EnableProductCategoriesAutoMapping'

GO--

ALTER SCHEMA Catalog TRANSFER [Module].[AvitoProductProperties]

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
  (SELECT TOP(1) PhotoName FROM [Catalog].[Photo] AS c WHERE c.[ObjId] = p.CategoryID and [Type]=@bigType) AS Picture,  
  (SELECT TOP(1) PhotoName FROM [Catalog].[Photo] AS c WHERE c.[ObjId] = p.CategoryID and [Type]=@smallType) AS MiniPicture  
 FROM [Catalog].[Category] AS p WHERE [ParentCategory] = @ParentCategoryID AND CategoryID <> 0   
 ORDER BY SortOrder, Name
else  
 SELECT   
  *,  
  (SELECT Count(CategoryID) FROM [Catalog].[Category] AS c WHERE c.ParentCategory = p.CategoryID) AS [ChildCategories_Count] ,  
  (SELECT TOP(1) PhotoName FROM [Catalog].[Photo] AS c WHERE c.[ObjId] = p.CategoryID and [Type]=@bigType) AS Picture,  
  (SELECT TOP(1) PhotoName FROM [Catalog].[Photo] AS c WHERE c.[ObjId] = p.CategoryID and [Type]=@smallType) AS MiniPicture  
 FROM [Catalog].[Category] AS p WHERE [ParentCategory] = @ParentCategoryID AND CategoryID <> 0 and Products_Count > 0  
 ORDER BY SortOrder, Name
END

GO--

ALTER FUNCTION [Settings].[ProductColorsToString]  
(  
 @ProductId int,
 @OnlyAvailable bit
)  
RETURNS nvarchar(Max)  
AS  
BEGIN  
	DECLARE @result nvarchar(max)  
    ;with cte as (  
		Select Color.ColorID, ColorName, ColorCode, SortOrder, isnull(PhotoName, '') as PhotoName, Sum(1*Offer.Main) as Main  
		From Catalog.Offer 
		Left join catalog.color on Color.ColorID=Offer.ColorID   
		Inner Join catalog.product on Product.ProductID = Offer.ProductID  
		Left join catalog.photo on color.Colorid = photo.ObjId and photo.type = 'color'  
		WHERE Product.ProductID=@ProductId and (Product.AllowPreorder=1 or @OnlyAvailable = 0 or Offer.Amount >0) 
		Group by Color.ColorID, ColorName, ColorCode, SortOrder,PhotoName  
    )  
    SELECT  @result= coalesce(@result + ', ', '[') + '{ColorId:' + convert(nvarchar(max), ColorID) + ', ColorName:''' + Replace(ColorName, '''', '&rsquo;') + ''', ColorCode:''' + ColorCode +''', PhotoName:''' + PhotoName + ''', Main:' + convert(nvarchar(max), Main) + '}'  
	FROM cte order by SortOrder  
	set @result = @result + ']'  
	return @result  
END 



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
		 SELECT [Settings].[ProductColorsToString](@productId, @OnlyAvailable)  
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
             SELECT CASE WHEN [Product].Discount > 0 THEN (@MinPrice - @MinPrice * [Product].Discount / 100) * CurrencyValue ELSE (@MinPrice - isnull([Product].DiscountAmount, 0)) * CurrencyValue END
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
		[Colors]=(SELECT [Settings].[ProductColorsToString](pe.ProductId, @OnlyAvailable)),
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

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Показывать список' WHERE [LanguageId] = '1' AND [ResourceKey] = 'Admin.Js.EditMainPageList.ShowOnMainPage'
UPDATE [Settings].[Localization] SET [ResourceValue] = 'Show list' WHERE [LanguageId] = '2' AND [ResourceKey] = 'Admin.Js.EditMainPageList.ShowOnMainPage'
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.MainPageProductsStore.Index.ShowList', 'Показывать список')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.MainPageProductsStore.Index.ShowList', 'Show list')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.MainPageProductsStore.Index.DisplayLatestProducts', 'Если нет товаров в списке, отображать самые последние добавленные товары')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.MainPageProductsStore.Index.DisplayLatestProducts', 'If there are no products in the list, display recently added products')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Home.Menu.SettingsTemplate', 'Параметры магазина')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Home.Menu.SettingsTemplate', 'Store options')

GO--

ALTER TABLE Vk.VkProduct ADD
	PhotosMapIds nvarchar(MAX) NULL
GO--

UPDATE [Settings].[SettingsSearch] SET [Link] = 'settings/common' WHERE [Title] = 'Favicon' 
UPDATE [Settings].[SettingsSearch] SET [Link] = 'settingsseo#?seoTab=seoParams' WHERE [Title] = 'SEO параметры' 
UPDATE [Settings].[SettingsSearch] SET [Link] = 'settingssystem#?systemTab=auth' WHERE [Title] = 'Авторизация' 
UPDATE [Settings].[SettingsSearch] SET [Link] = 'analytics#?analyticsReportTab=exportOrders' WHERE [Title] = 'Аналитика по заказам' 
UPDATE [Settings].[SettingsSearch] SET [Link] = 'analytics#?analyticsReportTab=exportCustomers' WHERE [Title] = 'Аналитика по покупателям' 
UPDATE [Settings].[SettingsSearch] SET [Link] = 'analytics#?analyticsReportTab=exportProducts' WHERE [Title] = 'Аналитика по товарам' 
UPDATE [Settings].[SettingsSearch] SET [Link] = 'settingscustomers#?tab=customerGroups' WHERE [Title] = 'Группы покупателей' 
UPDATE [Settings].[SettingsSearch] SET [Link] = 'settingscustomers#?tab=customerFields' WHERE [Title] = 'Дополнительные поля покупателя' 
UPDATE [Settings].[SettingsSearch] SET [Link] = 'import#?importTab=importProducts' WHERE [Title] = 'Импорт товаров' 
UPDATE [Settings].[SettingsSearch] SET [Link] = 'import#?importTab=importCategories' WHERE [Title] = 'Импорт категорий' 
UPDATE [Settings].[SettingsSearch] SET [Link] = 'settingscustomers#?tab=importCustomers' WHERE [Title] = 'Импорт покупателей' 
UPDATE [Settings].[SettingsSearch] SET [Link] = 'exportfeeds/indexcsv' WHERE [Title] = 'Дропшипперы' 
UPDATE [Settings].[SettingsSearch] SET [Link] = 'settingscoupons#?couponsTab=coupons' WHERE [Title] = 'Купоны' 
UPDATE [Settings].[SettingsSearch] SET [Link] = 'news#?newsTab=newscategories' WHERE [Title] = 'Категории новостей' 
UPDATE [Settings].[SettingsSearch] SET [Link] = 'settingscheckout#?checkoutTab=orderSources' WHERE [Title] = 'Источники заказов' 
UPDATE [Settings].[SettingsSearch] SET [Link] = 'vk' WHERE [Title] = 'Интеграция с ВКонтакте' 
UPDATE [Settings].[SettingsSearch] SET [Link] = 'settings/common' WHERE [Title] = 'Логотип' 
UPDATE [Settings].[SettingsSearch] SET [Link] = 'settingstemplate#?settingsTab=product' WHERE [Title] = 'Настройки карточки товара' 
UPDATE [Settings].[SettingsSearch] SET [Link] = 'settingscheckout#?checkoutTab=common' WHERE [Title] = 'Настройки оформления заказа' 
UPDATE [Settings].[SettingsSearch] SET [Link] = 'settingsmail#?notifyTab=emailsettings' WHERE [Title] = 'Настройки почты' 
UPDATE [Settings].[SettingsSearch] SET [Link] = 'settingscheckout#?checkoutTab=common' WHERE [Title] = 'Нумерация заказов' 
UPDATE [Settings].[SettingsSearch] SET [Link] = 'service/academy' WHERE [Title] = 'Обучение' 
UPDATE [Settings].[SettingsSearch] SET [Link] = 'settings/common#?indexTab=plan' WHERE [Title] = 'План продаж'
UPDATE [Settings].[SettingsSearch] SET [Link] = 'settingscoupons#?couponsTab=certificates' WHERE [Title] = 'Подарочные сертификаты'
UPDATE [Settings].[SettingsSearch] SET [Link] = 'settingscatalog#?catalogTab=priceregulation' WHERE [Title] = 'Регулирование цен'
UPDATE [Settings].[SettingsSearch] SET [Link] = 'home' WHERE [Title] = 'Рабочий стол'
UPDATE [Settings].[SettingsSearch] SET [Link] = 'settingscatalog#?catalogTab=brand' WHERE [Title] = 'Производители'
UPDATE [Settings].[SettingsSearch] SET [Link] = 'settingscoupons#?couponsTab=discounts' WHERE [Title] = 'Скидки'
UPDATE [Settings].[SettingsSearch] SET [Link] = 'smstemplates/smslog' WHERE [Title] = 'Смс история бонусов'
UPDATE [Settings].[SettingsSearch] SET [Link] = 'settingscatalog#?catalogTab=sizes' WHERE [Title] = 'Справочник размеров'
UPDATE [Settings].[SettingsSearch] SET [Link] = 'settingscatalog#?catalogTab=colors' WHERE [Title] = 'Справочник цветов'
UPDATE [Settings].[SettingsSearch] SET [Link] = 'settingscheckout#?checkoutTab=orderStatuses' WHERE [Title] = 'Статусы заказов'
UPDATE [Settings].[SettingsSearch] SET [Link] = 'settingscatalog#?catalogTab=tags' WHERE [Title] = 'Теги'
UPDATE [Settings].[SettingsSearch] SET [Link] = 'settingsseo#?seoTab=seoRobotTxt' WHERE [Title] = 'Файл robots.txt'
UPDATE [Settings].[SettingsSearch] SET [Link] = 'mainpageproductsstore' WHERE [Title] = 'Товары на главной'
UPDATE [Settings].[SettingsSearch] SET [Link] = 'settingstemplate#?settingsTab=catalog' WHERE [Title] = 'Фильтры'
UPDATE [Settings].[SettingsSearch] SET [Link] = 'settingscustomers#?tab=customerSubscribers' WHERE [Title] = 'Подписчики'
UPDATE [Settings].[SettingsSearch] SET [Link] = 'service/supportcenter' WHERE [Title] = 'Центр поддержки'
UPDATE [Settings].[SettingsSearch] SET [Link] = 'analytics#?analyticsReportTab=exportOrders' WHERE [Title] = 'Экспорт заказов'
UPDATE [Settings].[SettingsSearch] SET [Link] = 'exportfeeds/indexcsv#?exportTab=exportCategories' WHERE [Title] = 'Экспорт категорий'
UPDATE [Settings].[SettingsSearch] SET [Link] = 'exportfeeds/indexcsv#?exportTab=exportProducts' WHERE [Title] = 'Экспорт товаров'
UPDATE [Settings].[SettingsSearch] SET [Link] = 'settingstasks#?tasksTab=taskGroups' WHERE [Title] = 'Проекты'
UPDATE [Settings].[SettingsSearch] SET [Link] = 'analytics#?analyticsReportTab=telephony&telephonyTab=сallLog' WHERE [Title] = 'Журнал вызовов'
UPDATE [Settings].[SettingsSearch] SET [Link] = 'mainpageproductsstore?type=best' WHERE [Title] = 'Хиты'
UPDATE [Settings].[SettingsSearch] SET [Link] = 'mainpageproductsstore?type=new' WHERE [Title] = 'Новинки'
UPDATE [Settings].[SettingsSearch] SET [Link] = 'mainpageproductsstore?type=sale' WHERE [Title] = 'Товары со скидкой'
UPDATE [Settings].[SettingsSearch] SET [Link] = 'mainpageproductsstore' WHERE [Title] = 'Списки товаров'
UPDATE [Settings].[SettingsSearch] SET [Link] = 'settingstemplate#?settingsTab=news' WHERE [Title] = 'Настройки новостей'
UPDATE [Settings].[SettingsSearch] SET [Link] = 'settingstemplate#?settingsTab=brands' WHERE [Title] = 'Настройки производителей'
UPDATE [Settings].[SettingsSearch] SET [KeyWords] = 'Товары на главной, Свои списки' WHERE [Title] = 'Списки товаров'
UPDATE [Settings].[SettingsSearch] SET [KeyWords] = 'Характеристики, фильтры' WHERE [Title] = 'Свойства товаров'
UPDATE [Settings].[SettingsSearch] SET [KeyWords] = 'Шаблоны' WHERE [Title] = 'Дизайн'
UPDATE [Settings].[SettingsSearch] SET [KeyWords] = 'Товары, фотографии, отзывы, Перекрестный маркетинг, Похожие товары' WHERE [Title] = 'Настройки карточки товара'
UPDATE [Settings].[SettingsSearch] SET [KeyWords] = 'vk, вконтакте, товары вконтакте' WHERE [Title] = 'Интеграция с ВКонтакте'

DELETE FROM [Settings].[SettingsSearch] WHERE [Title] = 'Посадочные страницы' 
DELETE FROM [Settings].[SettingsSearch] WHERE [Title] = 'Товары на главной' 
DELETE FROM [Settings].[SettingsSearch] WHERE [Title] = 'Интеграция с Freshdesk' 

INSERT INTO [Settings].[SettingsSearch] ([Title],[Link],[KeyWords],[SortOrder]) VALUES ('Страница благодарности','settingscheckout#?checkoutTab=thankyoupage', 'успешное оформление заказа', '100')
INSERT INTO [Settings].[SettingsSearch] ([Title],[Link],[KeyWords],[SortOrder]) VALUES ('Обратная связь','settings/common#?indexTab=feedback', '', '0')
INSERT INTO [Settings].[SettingsSearch] ([Title],[Link],[KeyWords],[SortOrder]) VALUES ('Режим отображения цен','settingscatalog#?catalogTab=prices', 'Скрытие цен', '0')
INSERT INTO [Settings].[SettingsSearch] ([Title],[Link],[KeyWords],[SortOrder]) VALUES ('Бизнес-процессы','settingstasks#?tasksTab=tasks', 'Автоматическая постановка задач, автозадачи', '0')
INSERT INTO [Settings].[SettingsSearch] ([Title],[Link],[KeyWords],[SortOrder]) VALUES ('Автозвонки','settingstelephony#?telephonyTab=smartcalls', 'Автообзвон', '0')
INSERT INTO [Settings].[SettingsSearch] ([Title],[Link],[KeyWords],[SortOrder]) VALUES ('Уведомления','settingsmail#?notifyTab=notifications', 'почта', '100')
INSERT INTO [Settings].[SettingsSearch] ([Title],[Link],[KeyWords],[SortOrder]) VALUES ('Шаблоны ответов','settingsmail#?notifyTab=templates', '', '0')
INSERT INTO [Settings].[SettingsSearch] ([Title],[Link],[KeyWords],[SortOrder]) VALUES ('sms-уведомления','settingsmail#?notifyTab=sms', '', '0')
INSERT INTO [Settings].[SettingsSearch] ([Title],[Link],[KeyWords],[SortOrder]) VALUES ('Шаблоны документов', 'settingstemplatesdocx', '', '0')
INSERT INTO [Settings].[SettingsSearch] ([Title],[Link],[KeyWords],[SortOrder]) VALUES ('Каптча', 'settingssystem#?systemTab=system', 'captcha', '0')
INSERT INTO [Settings].[SettingsSearch] ([Title],[Link],[KeyWords],[SortOrder]) VALUES ('Аналитика email рассылок', 'analytics#?analyticsReportTab=emailMailing', 'рассылки, транзакционные письма', '100')
INSERT INTO [Settings].[SettingsSearch] ([Title],[Link],[KeyWords],[SortOrder]) VALUES ('Воронки продаж', 'funnels', 'Посадочные страницы, лендинг, допродажи', '0')
INSERT INTO [Settings].[SettingsSearch] ([Title],[Link],[KeyWords],[SortOrder]) VALUES ('Триггеры','triggers', 'триггерный маркетинг, события, авторассылка', '0')
INSERT INTO [Settings].[SettingsSearch] ([Title],[Link],[KeyWords],[SortOrder]) VALUES ('Бронирование','booking', 'услуги, ресурсы, журнал бронирования', '0')
INSERT INTO [Settings].[SettingsSearch] ([Title],[Link],[KeyWords],[SortOrder]) VALUES ('Avito','exportfeeds/indexavito', 'авито', '0')
INSERT INTO [Settings].[SettingsSearch] ([Title],[Link],[KeyWords],[SortOrder]) VALUES ('Ресселеры','exportfeeds/indexreseller', '', '0')
INSERT INTO [Settings].[SettingsSearch] ([Title],[Link],[KeyWords],[SortOrder]) VALUES ('Партнерская программа','partners', 'партнеры', '0')
INSERT INTO [Settings].[SettingsSearch] ([Title],[Link],[KeyWords],[SortOrder]) VALUES ('Одноклассники','ok', '', '0')
INSERT INTO [Settings].[SettingsSearch] ([Title],[Link],[KeyWords],[SortOrder]) VALUES ('Telegram','telegram', 'телеграмм', '0')
INSERT INTO [Settings].[SettingsSearch] ([Title],[Link],[KeyWords],[SortOrder]) VALUES ('Instagram','instagram', 'инстаграм', '0')
INSERT INTO [Settings].[SettingsSearch] ([Title],[Link],[KeyWords],[SortOrder]) VALUES ('Дополнительные стили','settingstemplate#?settingsTab=csseditor', 'css', '0')
INSERT INTO [Settings].[SettingsSearch] ([Title],[Link],[KeyWords],[SortOrder]) VALUES ('Виджет коммуникаций','settingstemplate#?settingsTab=socialwidget', 'Социальные виджеты', '0')
INSERT INTO [Settings].[SettingsSearch] ([Title],[Link],[KeyWords],[SortOrder]) VALUES ('Настройки дизайна','settingstemplate#?settingsTab=common', 'параметры магазина, цветовая схема, отображение, размеры изображений, иконки, дополнительный тег в Head,  пользовательское соглашение, язык, опредение города', '0')
INSERT INTO [Settings].[SettingsSearch] ([Title],[Link],[KeyWords],[SortOrder]) VALUES ('Сегменты покупателей','settingscustomers#?tab=customerSegments', '', '0')
INSERT INTO [Settings].[SettingsSearch] ([Title],[Link],[KeyWords],[SortOrder]) VALUES ('Источники лидов','settingscheckout#?checkoutTab=orderSources', '', '0')

GO--

if not exists (select * from [Settings].[Localization] where ResourceKey = 'BuyMore.NextAction.NotHaveEnough' and [LanguageId] = 2)
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'BuyMore.NextAction.NotHaveEnough', 'You do not have enough')
	
if not exists (select * from [Settings].[Localization] where ResourceKey = 'BuyMore.NextAction.ToReceive' and [LanguageId] = 2)
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'BuyMore.NextAction.ToReceive', 'to receive')
	
if not exists (select * from [Settings].[Localization] where ResourceKey = 'BuyMore.NextAction.FreeShipping' and [LanguageId] = 2)
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'BuyMore.NextAction.FreeShipping', 'free shipping')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Common.SpamCheckFailed', 'Не пройдена проверка на спам')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Common.SpamCheckFailed', 'Spam check failed')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Checkout.CheckoutBonus.WantToGetBonusCard', 'Хочу получить бонусную карту и оплачивать покупки бонусами!')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Checkout.CheckoutBonus.WantToGetBonusCard', 'I want to get a bonus card and pay for purchases with bonuses!')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Checkout.CheckoutBonus.ToBonusCart', 'на бонусную карту')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Checkout.CheckoutBonus.ToBonusCart', 'to the bonus card')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Checkout.CheckoutUser.AlreadyHaveAccount', 'У меня уже есть Учетная запись')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Checkout.CheckoutUser.AlreadyHaveAccount', 'I already have an Account')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Checkout.CheckoutUser.AlreadyHaveAccountBonusCard', 'У меня уже есть Учетная запись, Бонусная карта')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Checkout.CheckoutUser.AlreadyHaveAccountBonusCard', 'I already have an Account, Bonus card')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Checkout.Success.WillCreditedToBonusCard', 'будет начислено на Вашу Бонусную карту №')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Checkout.Success.WillCreditedToBonusCard', 'will be credited to your Bonus Card №')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Checkout.Success.WillCreditedToBonusCard.After payment', 'после оплаты данного заказа')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Checkout.Success.WillCreditedToBonusCard.After payment', 'after payment of this order')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Checkout.ThankYouPage.WeInSocialNetworks', 'Мы в соцсетях')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Checkout.ThankYouPage.WeInSocialNetworks', 'We are in social networks')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Checkout.ThankYouPage.TellAboutUs', 'Расскажите о нас в соцсетях')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Checkout.ThankYouPage.TellAboutUs', 'Tell about us on social networks')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Common.ClosedStore.Login', 'Войти')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Common.ClosedStore.Login', 'Login')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Product.ProductGifts.UponPurchase', 'При покупке')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Product.ProductGifts.UponPurchase', 'Upon purchase')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Product.ProdcutInfo.Available', 'Доступно')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Product.ProdcutInfo.Available', 'Available')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'User.Registration.PartnerRegistration', 'Зарегистрироваться в партнерской программе')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'User.Registration.PartnerRegistration', 'Sign up for an affiliate program')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Bonuses.UserNotRegistred', 'Пользователь не зарегистрирован')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Bonuses.UserNotRegistred', 'User not registred')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Bonuses.BonusCardAlreadyRegistered', 'Бонусная карта уже зарегистрирована')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Bonuses.BonusCardAlreadyRegistered', 'Bonus card already registered')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Checkout.PayRedirectError.OrderNotFound', 'Заказ не найден')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Checkout.PayRedirectError.OrderNotFound', 'Order not found')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Checkout.PayRedirectError.PaymentMethodNotFound', 'Метод оплаты не найден')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Checkout.PayRedirectError.PaymentMethodNotFound', 'Payment method not found')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Checkout.PayRedirectError.NotConfirmedByManager', 'Заказ не подтвержден менеджером')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Checkout.PayRedirectError.NotConfirmedByManager', 'Not confirmed by manager')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Checkout.PayRedirectError.LinkNotAvailable', 'Ссылка на оплату не доступна')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Checkout.PayRedirectError.LinkNotAvailable', 'Link not available')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Js.Bonus.BonusCardBlocked', 'Бонусная карта заблокирована')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Js.Bonus.BonusCardBlocked', 'Bonus card is blocked')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Js.Bonus.BonusCartCreated', 'Бонусная карта создана')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Js.Bonus.BonusCartCreated', 'Bonus card created')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Js.Cards.NotFoundTypeToRemove', 'Не найден тип дисконтной карты для удаления')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Js.Cards.NotFoundTypeToRemove', 'Not found type for remove discount card')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Js.ShoppingCart.СonfirmButtonText', 'Корзина')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Js.ShoppingCart.СonfirmButtonText', 'Shopping сart')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Js.ShoppingCart.СancelButtonText', 'Ок')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Js.ShoppingCart.СancelButtonText', 'Ok')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Js.Order.PrintOrder', 'Печать заказа')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Js.Order.PrintOrder', 'Print order')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Js.Order.TrackNumber', 'Номер отслеживания')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Js.Order.TrackNumber', 'Track number')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Js.Reviews.SelectFile', 'Выберите файл')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Js.Reviews.SelectFile', 'Select file')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Js.Shipping.Cost', 'Стоимость')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Js.Shipping.Cost', 'Cost')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Js.Shipping.IndividualDeliveryMethod', 'Индивидуальный метод доставки')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Js.Shipping.IndividualDeliveryMethod', 'Individual delivery method')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Js.Zone.SpecifyTheRegion', 'Уточните регион')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Js.Zone.SpecifyTheRegion', 'Specify the region')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Js.ShippingTemplate.SelectPickupPoint', 'Выбрать пункт выдачи')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Js.ShippingTemplate.SelectPickupPoint', 'Select pick-up point')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Js.ShippingTemplate.SelectPickupPoint.NotSelected', 'Необходимо выбрать пункт выдачи')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Js.ShippingTemplate.SelectPickupPoint.NotSelected', 'You must select a pick-up point')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Js.ShippingTemplate.SelectPickupPointOrDelivery.NotSelected', 'Необходимо выбрать пункт выдачи или указать курьерскую доставку')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Js.ShippingTemplate.SelectPickupPointOrDelivery.NotSelected', 'You must select a pick-up point or indicate courier delivery')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Js.ShippingTemplate.ShowOnMap', 'Показать на карте')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Js.ShippingTemplate.ShowOnMap', 'Show on map')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Js.ShippingTemplate.ShowScheme', 'Показать схему')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Js.ShippingTemplate.ShowScheme', 'Show scheme')

GO--

IF (NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'Customers' AND  TABLE_NAME = 'Tag'))
BEGIN

	CREATE TABLE [Customers].[Tag](
		[Id] [int] IDENTITY(1,1) NOT NULL,
		[Name] [nvarchar](450) NOT NULL,
		[Enabled] [bit] NOT NULL,
	 CONSTRAINT [PK_Tag] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]

END

GO--

IF (NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'Customers' AND  TABLE_NAME = 'TagMap'))
BEGIN

	CREATE TABLE [Customers].[TagMap](
		[CustomerId] [uniqueidentifier] NOT NULL,
		[TagId] [int] NOT NULL,
		[SortOrder] [int] NOT NULL,
	 CONSTRAINT [PK_TagMap] PRIMARY KEY CLUSTERED 
	(
		[CustomerId] ASC,
		[TagId] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]

	ALTER TABLE [Customers].[TagMap]  WITH CHECK ADD  CONSTRAINT [FK_TagMap_Tag] FOREIGN KEY([TagId])
	REFERENCES [Customers].[Tag] ([Id])
	ON DELETE CASCADE
	
	ALTER TABLE [Customers].[TagMap] CHECK CONSTRAINT [FK_TagMap_Tag]
	
	ALTER TABLE [Customers].[TagMap]  WITH CHECK ADD  CONSTRAINT [FK_TagMap_Customer] FOREIGN KEY([CustomerId])
	REFERENCES [Customers].[Customer] ([CustomerID])
	ON DELETE CASCADE
	
	ALTER TABLE [Customers].[TagMap] CHECK CONSTRAINT [FK_TagMap_Customer]
END

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Customers.ViewTags.Tags', 'Теги')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Customers.ViewTags.Tags', 'Tags')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Customers.RightBlock.Tags', 'Теги')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Customers.RightBlock.Tags', 'Tags')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Customers.RightBlock.SelectTags', 'Выберите теги')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Customers.RightBlock.SelectTags', 'Select tags')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.SettingsCustomer.Index.Tags', 'Теги')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.SettingsCustomer.Index.Tags', 'Tags')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.SettingsCustomer.CustomerTags.Title', 'Теги покупателей')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.SettingsCustomer.CustomerTags.Title', 'Customer tags')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.SettingsCustomer.CustomerTags.AddTag', 'Добавить тег')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.SettingsCustomer.CustomerTags.AddTag', 'Add tag')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.CustomerTags.Name', 'Название')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.CustomerTags.Name', 'Name')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.CustomerTags.Enabled', 'Активность')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.CustomerTags.Enabled', 'Enabled')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.CustomerTags.Active', 'Активные')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.CustomerTags.Active', 'Active')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.CustomerTags.Inactive', 'Неактивные')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.CustomerTags.Inactive', 'Inactive')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.CustomerTags.DeleteSelected', 'Удалить выбранные')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.CustomerTags.DeleteSelected', 'Delete selected')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.CustomerTags.AreYouSureDelete', 'Вы уверены, что хотите удалить?')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.CustomerTags.AreYouSureDelete', 'Are you sure to delete?')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.CustomerTags.Deleting', 'Удаление')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.CustomerTags.Deleting', 'Deleting')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.CustomerTags.Sorting', 'Сортировка')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.CustomerTags.Sorting', 'Sorting')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.CustomerTags.AddEdit.BackToTags', 'Теги покупателей')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.CustomerTags.AddEdit.BackToTags', 'Customer tags')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.CustomerTags.AddEdit.Tag', 'Тег')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.CustomerTags.AddEdit.Tag', 'Tag')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.CustomerTags.AddEdit.NewTag', 'Новый тег')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.CustomerTags.AddEdit.NewTag', 'New tag')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.CustomerTags.AddEdit.Delete', 'Удалить')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.CustomerTags.AddEdit.Delete', 'Delete')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.CustomerTags.AddEdit.TagName', 'Название')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.CustomerTags.AddEdit.TagName', 'Name')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.CustomerTags.AddEdit.Activity', 'Активность')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.CustomerTags.AddEdit.Activity', 'Activity')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.CustomerTags.AddEdit.Title', 'Теги покупателей')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.CustomerTags.AddEdit.Title', 'Customer tags')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.CustomerTags.AddEdit.Sorting', 'Сортировка')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.CustomerTags.AddEdit.Sorting', 'Sorting')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.Customers.Tag', 'Теги покупателей')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.Customers.Tag', 'Customer tags')

GO--


INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Checkout.CheckoutShipping.RequiredAddress', 'Укажите адрес для расчета доставки')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Checkout.CheckoutShipping.RequiredAddress', 'Enter the address for the calculation of delivery')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Checkout.CheckoutPayment.RequiredAddress', 'Укажите адрес для расчета оплаты')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Checkout.CheckoutPayment.RequiredAddress', 'Enter the address for the calculation of payment')
	
GO--	

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Import.ImportCustomers.DefaultCustomerGroup', 'Группа покупателей по умолчанию')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Import.ImportCustomers.DefaultCustomerGroup', 'Default customer group')

GO--

UPDATE [Settings].[MailFormatType] SET [Comment] = 'Письмо партнеру при привязке нового покупателя (#SITEURL#, #SITENAME#, #PARTNERS_URL#, #NAME#, #EMAIL#, #PHONE#, #CITY#, #BALANCE#, #COUPONCODE#, #REWARDPERCENT#, #TYPE#, #CUSTOMER_EMAIL#, #CUSTOMER_PHONE#)' WHERE [MailType] = 'OnPartnerCustomerBinded'

UPDATE [Settings].[MailFormat] SET [FormatText] = '<div style="color: #4c4f56; font-family: Arial, Helvetica, sans-serif; font-size: 14px;">
    <div class="header" style="border-bottom: 1px solid #ededed; display: table; margin-bottom: 25px; padding-bottom: 25px; width: 100%;">
        <div class="logo" style="display: table-cell; text-align: left; vertical-align: middle;">#LOGO#</div>
        <div class="phone" style="display: table-cell; text-align: right; vertical-align: middle;">
            <div class="tel" style="font-size: 26px; font-weight: bold; line-height: 1; margin-bottom: 5px;">&nbsp;</div>
            <div class="inform" style="font-size: 12px;">&nbsp;</div>
        </div>
    </div>
    <p>Новый клиент закреплен за вами в личном кабинете партнера #SITENAME#</p>
    <p>Перейти в личный кабинет партнера: <a href="#PARTNERS_URL#">#PARTNERS_URL#</a></p>
    <div class="data" style="display: table; width: 100%;">
        <div class="data-cell" style="display: table-cell; padding: 0; padding-right: 1%; width: 24%;">
            <div class="o-title vi" style="font-size: 14px; font-weight: bold; margin: 5px 0;">Информация о клиенте</div>
            <div class="l-row">
                <div class="l-name vi cs-light" style="color: #acacac; display: inline-block; margin: 5px 0; padding-right: 15px; width: 80px; vertical-align: middle;">E-mail:</div>
                <div class="l-value vi" style="display: inline-block; margin: 5px 0;">#CUSTOMER_EMAIL#</div>
            </div>
            <div class="l-row">
                <div class="l-name vi cs-light" style="color: #acacac; display: inline-block; margin: 5px 0; padding-right: 15px; width: 80px; vertical-align: middle;">Телефон:</div>
                <div class="l-value vi" style="display: inline-block; margin: 5px 0;">#CUSTOMER_PHONE#</div>
            </div>
        </div>
    </div>
</div>' WHERE [MailFormatTypeId] = (SELECT TOP(1) [MailFormatTypeID] FROM [Settings].[MailFormatType] WHERE [MailType] = 'OnPartnerCustomerBinded')

UPDATE [Settings].[MailFormatType] SET [Comment] = 'Письмо партнеру при оплате заказа привязанным к нему покупателем (#SITEURL#, #SITENAME#, #PARTNERS_URL#, #NAME#, #EMAIL#, #PHONE#, #CITY#, #BALANCE#, #COUPONCODE#, #REWARDPERCENT#, #TYPE#, #CUSTOMER_EMAIL#, #CUSTOMER_PHONE#, #REWARD_SUM#, #PAYMENT_SUM#)' WHERE [MailType] = 'OnPartnerMoneyAdded'

UPDATE [Settings].[MailFormat] SET [FormatText] = '<div style="color: #4c4f56; font-family: Arial, Helvetica, sans-serif; font-size: 14px;">
    <div class="header" style="border-bottom: 1px solid #ededed; display: table; margin-bottom: 25px; padding-bottom: 25px; width: 100%;">
        <div class="logo" style="display: table-cell; text-align: left; vertical-align: middle;">#LOGO#</div>
        <div class="phone" style="display: table-cell; text-align: right; vertical-align: middle;">
            <div class="tel" style="font-size: 26px; font-weight: bold; line-height: 1; margin-bottom: 5px;">&nbsp;</div>
            <div class="inform" style="font-size: 12px;">&nbsp;</div>
        </div>
    </div>
    <p>Начислено вознаграждение в рамках партнерской программы #SITENAME#</p>
	<p>Вам начислено партнерское вознаграждение в размере <b>#REWARD_SUM#</b> за заказ клиента. Оплачено товаров на сумму <b>#PAYMENT_SUM#</b>. Текущий баланс: <b>#BALANCE#</b>.</p>
    <p>Перейти в личный кабинет партнера: <a href="#PARTNERS_URL#">#PARTNERS_URL#</a></p>
    <div class="data" style="display: table; width: 100%;">
        <div class="data-cell" style="display: table-cell; padding: 0; padding-right: 1%; width: 24%;">
            <div class="o-title vi" style="font-size: 14px; margin: 5px 0;">Информация о клиенте</div>
            <div class="l-row">
                <div class="l-name vi cs-light" style="color: #acacac; display: inline-block; margin: 5px 0; padding-right: 15px; width: 80px; vertical-align: middle;">E-mail:</div>
                <div class="l-value vi" style="display: inline-block; margin: 5px 0;">#CUSTOMER_EMAIL#</div>
            </div>
            <div class="l-row">
                <div class="l-name vi cs-light" style="color: #acacac; display: inline-block; margin: 5px 0; padding-right: 15px; width: 80px; vertical-align: middle;">Телефон:</div>
                <div class="l-value vi" style="display: inline-block; margin: 5px 0;">#CUSTOMER_PHONE#</div>
            </div>
        </div>
    </div>
</div>' WHERE [MailFormatTypeId] = (SELECT TOP(1) [MailFormatTypeID] FROM [Settings].[MailFormatType] WHERE [MailType] = 'OnPartnerMoneyAdded')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.Tasks.ModalEditTaskCtrl.WarningMissingComment', 'Комментарий не будет добавлен')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.Tasks.ModalEditTaskCtrl.WarningMissingComment', 'Comment will not be added')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.Customers.CustomerSegment', 'Сегмент покупателей')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.Customers.CustomerSegment', 'Customer segment')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.Catalog.Tag', 'Теги')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.Catalog.Tag', 'Tags')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.Customers.AddTags', 'Добавить теги')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.Customers.AddTags', 'Add tags')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.AddTagsToCustomers.AddingTags', 'Добавление тегов покупателям')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.AddTagsToCustomers.AddingTags', 'Adding tags to customers')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.AddTagsToCustomers.SelectTags', 'Выберите теги')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.AddTagsToCustomers.SelectTags', 'Select tags')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.AddTagsToCustomers.Add', 'Добавить')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.AddTagsToCustomers.Add', 'Add')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.AddTagsToCustomers.Cancel', 'Отменить')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.AddTagsToCustomers.Cancel', 'Cancel')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.AddTagsToCustomers.TagsAddedSuccessfully', 'Теги успешно добавлены')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.AddTagsToCustomers.TagsAddedSuccessfully', 'Tags added successfully')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.AddTagsToCustomers.Error', 'Не удалось добавить теги')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.AddTagsToCustomers.Error', 'Failed to add tags')

GO--

If not Exists(Select 1 From sys.columns WHERE Name = N'SortOrder' AND Object_ID = Object_ID(N'[Customers].[Tag]'))
Begin
	ALTER TABLE [Customers].[Tag] ADD
		SortOrder int NULL
end

GO--


ALTER TABLE Catalog.Product ADD
	SortPopular int NULL

GO--

Update Catalog.Product Set SortPopular = 0
GO--

Update Catalog.Product
Set SortPopular = isNull((Select Sum([Amount]) 
				   From [Order].[OrderItems]
				   Left Join [Order].[Order] On [Order].[OrderId] = [OrderItems].[OrderId] 
				   Where [OrderItems].[ProductId] = Product.ProductId and PaymentDate is not null), 0)

GO--
	
Update Catalog.Category Set Sorting = 200 Where Sorting = 2
Update Catalog.Category Set Sorting = 300 Where Sorting = 5
Update Catalog.Category Set Sorting = 400 Where Sorting = 6
Update Catalog.Category Set Sorting = 600 Where Sorting = 8

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Catalog.Sorting.SortByPopular', 'Популярные')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Catalog.Sorting.SortByPopular', 'Popular')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Catalog.ESortOrder.DescByPopular', 'Популярные')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Catalog.ESortOrder.DescByPopular', 'Popular')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Catalog.ESortOrder.DescByDiscount', 'По размеру скидки')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Catalog.ESortOrder.DescByDiscount', 'By discount')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Product.Edit.SortPopular', 'Сортировка по популярности')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Product.Edit.SortPopular', 'Sort order by popular')

UPDATE [Settings].[Localization] Set [ResourceValue] = 'Новинки' Where [ResourceKey] = 'Core.Catalog.ESortOrder.DescByAddingDate' and [LanguageId] = 1
UPDATE [Settings].[Localization] Set [ResourceValue] = 'New' Where [ResourceKey] = 'Core.Catalog.ESortOrder.DescByAddingDate' and [LanguageId] = 2
UPDATE [Settings].[Localization] Set [ResourceValue] = 'Сначала дешевле' Where [ResourceKey] = 'Core.Catalog.ESortOrder.AscByPrice' and [LanguageId] = 1
UPDATE [Settings].[Localization] Set [ResourceValue] = 'Сначала дороже' Where [ResourceKey] = 'Core.Catalog.ESortOrder.DescByPrice' and [LanguageId] = 1
UPDATE [Settings].[Localization] Set [ResourceValue] = 'Высокий рейтинг' Where [ResourceKey] = 'Core.Catalog.ESortOrder.DescByRatio' and [LanguageId] = 1

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.MobileVersion.DisplayShowAddButton', 'Отображать кнопку "В корзину" в списке товара')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.MobileVersion.DisplayShowAddButton', 'Display the "Add to Cart" button in the product list')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.MobileVersion.СountLinesProductName', 'Число строк в названии товара')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.MobileVersion.СountLinesProductName', 'The number of lines in the product name')

GO--

ALTER TABLE [CMS].[ChangeHistory]
ALTER COLUMN ParameterName nvarchar(350) NOT NULL

GO--

ALTER TABLE [Order].[Order]
ALTER COLUMN CouponCode nvarchar(50) NULL

GO--

if not exists (select * from [Settings].[Localization] where ResourceKey = 'Admin.ShippingMethods.Grastin.OrderWasReceivedAtTheWarehouse' AND [LanguageId] = 1)
begin
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ShippingMethods.Grastin.OrderWasReceivedAtTheWarehouse','Получен от клиента. Заказ получен на склад от клиента (Принят у партнера)')
end
else
begin
	UPDATE [Settings].[Localization] SET [ResourceValue] = 'Получен от клиента. Заказ получен на склад от клиента (Принят у партнера)' where ResourceKey = 'Admin.ShippingMethods.Grastin.OrderWasReceivedAtTheWarehouse' AND [LanguageId] = 1
end

if not exists (select * from [Settings].[Localization] where ResourceKey = 'Admin.ShippingMethods.Grastin.ReturnedToClient' AND [LanguageId] = 1)
begin
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ShippingMethods.Grastin.ReturnedToClient','Возвращен клиенту (Возвращен партнеру)')
end
else
begin
	UPDATE [Settings].[Localization] SET [ResourceValue] = 'Возвращен клиенту (Возвращен партнеру)' where ResourceKey = 'Admin.ShippingMethods.Grastin.ReturnedToClient' AND [LanguageId] = 1
end

if not exists (select * from [Settings].[Localization] where ResourceKey = 'Admin.ShippingMethods.Grastin.WrittenOff' AND [LanguageId] = 1)
begin
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ShippingMethods.Grastin.WrittenOff','Списан (Возмещен)')
end
else
begin
	UPDATE [Settings].[Localization] SET [ResourceValue] = 'Списан (Возмещен)' where ResourceKey = 'Admin.ShippingMethods.Grastin.WrittenOff' AND [LanguageId] = 1
end

GO--

ALTER TABLE Catalog.Product ADD CONSTRAINT
	DF_Product_SortPopular DEFAULT 0 FOR SortPopular

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Product.ProductTabs.Properties.ShowAll', 'Все характеристики')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Product.ProductTabs.Properties.ShowAll', 'All properties')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Product.ProductTabs.Properties.Hide', 'Скрыть характеристики')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Product.ProductTabs.Properties.Hide', 'Hide properties')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'MyAccount.Index.Greeting', 'Здравствуйте, <strong>{0}</strong>, добро пожаловать в Ваш личный кабинет.')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'MyAccount.Index.Greeting', 'Hello, <strong>{0}</strong>, welcome to your personal account.')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Catalog.MinimizeSearchResults', 'Пытаться минимизировать поисковую выдачу')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Catalog.MinimizeSearchResults', 'Try to minimize search results')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Catalog.MinimizeSearchResultsHint', 'Пытаться минимизировать поисковую выдачу. При полном совпадении слова, которое искали, выдаются только эти результаты. Если нет полного совпадения, поиск в соответствии с выбранными настройками.')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Catalog.MinimizeSearchResultsHint', 'Try to minimize search results. When a full match of the word that was searched for, only these results are given. If there is no full match, search according to the selected settings.')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Commonpage.SettingsTemplate', 'Витрина магазина')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Commonpage.SettingsTemplate', 'Shop showcase')

GO--

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[CMS].[ReviewLikes]') AND type in (N'U'))
BEGIN
CREATE TABLE [CMS].[ReviewLikes](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ReviewId] [int] NOT NULL,
	[IsLike] [bit] NOT NULL,
	[CustomerId] [uniqueidentifier] NOT NULL,
	[AddDate] [datetime] NOT NULL,
 CONSTRAINT [PK_ReviewLikes] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END

GO--

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[CMS].[ReviewLikes]') AND name = N'Likes_ReviewCustomer')
CREATE UNIQUE NONCLUSTERED INDEX [Likes_ReviewCustomer] ON [CMS].[ReviewLikes]
(
	[ReviewId] DESC,
	[CustomerId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO--

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[CMS].[FK_ReviewLikes_Review]') AND parent_object_id = OBJECT_ID(N'[CMS].[ReviewLikes]'))
ALTER TABLE [CMS].[ReviewLikes]  WITH CHECK ADD  CONSTRAINT [FK_ReviewLikes_Review] FOREIGN KEY([ReviewId])
REFERENCES [CMS].[Review] ([ReviewId])
ON DELETE CASCADE

GO--

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[CMS].[FK_ReviewLikes_Review]') AND parent_object_id = OBJECT_ID(N'[CMS].[ReviewLikes]'))
ALTER TABLE [CMS].[ReviewLikes] CHECK CONSTRAINT [FK_ReviewLikes_Review]

GO--

ALTER TABLE CMS.Review ADD
	[LikesCount] int NULL,
	[DislikesCount] int NULL,
	[RatioByLikes] int NULL

GO--

UPDATE CMS.Review SET [LikesCount] = 0, [DislikesCount] = 0, [RatioByLikes] = 0

GO--

ALTER TABLE CMS.Review ALTER COLUMN
	[LikesCount] int NOT NULL

GO--

ALTER TABLE CMS.Review ALTER COLUMN
	[DislikesCount] int NOT NULL

GO--

ALTER TABLE CMS.Review ALTER COLUMN
	[RatioByLikes] int NOT NULL

GO--

CREATE PROCEDURE [CMS].[sp_AddReviewLike]

		@ReviewId int,
		@IsLike bit,
		@CustomerId uniqueidentifier

AS
BEGIN
	DECLARE @PrevLike bit
	SELECT @PrevLike = [IsLike] FROM [CMS].[ReviewLikes] WHERE [ReviewId] = @ReviewId AND [CustomerId] = @CustomerId

	IF (@PrevLike IS NULL)
	BEGIN
		INSERT INTO [CMS].[ReviewLikes]
				([ReviewId],[IsLike],[CustomerId],[AddDate])
		  VALUES
				(@ReviewId,@IsLike,@CustomerId,GETDATE())

		UPDATE [CMS].[Review]
		   SET [LikesCount] = (CASE WHEN @IsLike = 1 THEN [LikesCount] + 1 ELSE [LikesCount] END)
			  ,[DislikesCount] = (CASE WHEN @IsLike = 1 THEN [DislikesCount] ELSE [DislikesCount] + 1 END)
			  ,[RatioByLikes] = (CASE WHEN @IsLike = 1 THEN [RatioByLikes] + 1 ELSE [RatioByLikes] - 1 END)
		 WHERE [ReviewId] = @ReviewId
	END

	IF (@PrevLike != @IsLike)
	BEGIN
		UPDATE [CMS].[ReviewLikes]
		   SET [IsLike] = @IsLike, [AddDate] = GETDATE()
		WHERE [ReviewId] = @ReviewId AND [CustomerId] = @CustomerId

		UPDATE [CMS].[Review]
		   SET [LikesCount] = (CASE WHEN @IsLike = 1 THEN [LikesCount] + 1 ELSE [LikesCount] - 1 END)
			  ,[DislikesCount] = (CASE WHEN @IsLike = 1 THEN [DislikesCount] - 1 ELSE [DislikesCount] + 1 END)
			  ,[RatioByLikes] = (CASE WHEN @IsLike = 1 THEN [RatioByLikes] + 2 ELSE [RatioByLikes] - 2 END)
		 WHERE [ReviewId] = @ReviewId

	END
END

GO--

ALTER TABLE [Order].ShippingMethod ADD
	ExtraDeliveryTime int NULL

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ShippingMethods.Common.ExtraDeliveryTime', 'Увеличивать срок доставки на')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ShippingMethods.Common.ExtraDeliveryTime', 'Increase the delivery time by')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ShippingMethods.Common.ExtraDeliveryTimeDays', 'дн.')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ShippingMethods.Common.ExtraDeliveryTimeDays', 'days')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ShippingMethods.Common.ExtraDeliveryTimeHint', 'Срок доставки будет увеличен на указанное колличество дней')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ShippingMethods.Common.ExtraDeliveryTimeHint', 'Delivery time will be increased by the entered number of days')

GO--

Update [Order].[ShippingMethod]
Set [ExtraDeliveryTime] = (Select top(1)[ParamValue] From [Order].[ShippingParam] Where [ShippingParam].[ShippingMethodID] = [ShippingMethod].[ShippingMethodID] and [ParamName] = 'IncreaseDeliveryTime')
Where [ShippingType] = 'Boxberry'

GO--

Update [Order].[ShippingMethod]
Set [ExtraDeliveryTime] = (Select top(1)[ParamValue] From [Order].[ShippingParam] Where [ShippingParam].[ShippingMethodID] = [ShippingMethod].[ShippingMethodID] and [ParamName] = 'IncreaseDeliveryTime')
Where [ShippingType] = 'Grastin'

GO--

Update [Order].[ShippingMethod]
Set [ExtraDeliveryTime] = (Select top(1)[ParamValue] From [Order].[ShippingParam] Where [ShippingParam].[ShippingMethodID] = [ShippingMethod].[ShippingMethodID] and [ParamName] = 'IncreaseDeliveryTime')
Where [ShippingType] = 'RussianPost'

GO--

Update [Order].[ShippingMethod]
Set [Extracharge] = (Select top(1)[ParamValue] From [Order].[ShippingParam] Where [ShippingParam].[ShippingMethodID] = [ShippingMethod].[ShippingMethodID] and [ParamName] = 'additionalPrice'),
    [ExtrachargeType] = CASE ((Select top(1)[ParamValue] From [Order].[ShippingParam] Where [ShippingParam].[ShippingMethodID] = [ShippingMethod].[ShippingMethodID] and [ParamName] = 'typeAdditionPrice')) When 'Percent' Then 1 Else 0 End
Where [ShippingType] = 'Sdek'

GO--

Update [Order].[ShippingMethod]
Set [Extracharge] = (Select top(1)[ParamValue] From [Order].[ShippingParam] Where [ShippingParam].[ShippingMethodID] = [ShippingMethod].[ShippingMethodID] and [ParamName] = 'Extracharge'),
    [ExtrachargeType] = 0
Where [ShippingType] = 'RussianPost'

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Mobile.Catalog.ShowMore', 'Показать еще')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Mobile.Catalog.ShowMore', 'Show more')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Telephony.Telphin.ServiceUrl', 'Url для оповещений')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Telephony.Telphin.ServiceUrl', 'Url for notifications')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Telephony.Telphin.ServiceUrlNote', 'На данный адрес будут отправляться оповещения о звонках.<br />Необходимо указать его в настройках в личном кабинете Телфин')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Telephony.Telphin.ServiceUrlNote', 'Call notifications will be sent to this url')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.SettingsTelephony.DeleteTelphinEvents', 'Вы уверены что хотите удалить оповещения о событиях?')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.SettingsTelephony.DeleteTelphinEvents', 'Are you sure you want to delete event notifications?')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.SettingsTelephony.SetDefaultTelphinEvents', 'Вы уверены что хотите установить настройки оповещений о событиях по умолчанию?')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.SettingsTelephony.SetDefaultTelphinEvents', 'Are you sure you want to set default settings of event notifications?')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Telphin.Extensions.Notifications.Add', 'Добавить настройки по умолчанию')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Telphin.Extensions.Notifications.Add', 'Add default settings?')
UPDATE [Settings].[Localization] SET [ResourceValue] = 'Установить настройки по умолчанию' WHERE [ResourceKey] = 'Admin.Telphin.Extensions.Notifications.Set' AND LanguageId = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'Set default settings' WHERE [ResourceKey] = 'Admin.Telphin.Extensions.Notifications.Set' AND LanguageId = 2
DELETE FROM [Settings].[Localization] WHERE [ResourceKey] = 'Admin.Telphin.Extensions.Notifications.Override'

GO--

IF NOT EXISTS(SELECT 1 FROM sys.columns WHERE Name = N'SortOrder' AND Object_ID = Object_ID(N'[CMS].[NewsProduct]'))
BEGIN
	ALTER TABLE [CMS].[NewsProduct] ADD
		SortOrder int NULL
END

GO--

CREATE PROCEDURE [CMS].[ChangeNewsProductsSorting]
	@newsId int,
	@Id int,
	@prevId int,
	@nextId int
AS
BEGIN
	IF @prevId IS NULL AND @nextId IS NULL
		RETURN;

	DECLARE @NewSort int

	DECLARE @prevSort int = (SELECT SortOrder FROM [CMS].[NewsProduct] WHERE ProductId = @prevId AND NewsId = @newsId)
	DECLARE @nextSort int = (SELECT SortOrder FROM [CMS].[NewsProduct] WHERE ProductId = @nextId AND NewsId = @newsId)

	if @prevSort IS NULL OR @nextSort IS NULL
	BEGIN
		SELECT @NewSort = 
			(SELECT CASE WHEN @prevSort IS NULL 
				THEN ISNULL(MIN(SortOrder), 0) - 10 
				ELSE ISNULL(MAX(SortOrder), 0) + 10 END 
			FROM [CMS].[NewsProduct]
			WHERE ProductId <> @Id AND NewsId = @newsId)
	END
	ELSE
	BEGIN
		if @nextSort - @prevSort > 1
		BEGIN
			SELECT @NewSort = (@prevSort + ((@nextSort - @prevSort) / 2))
		END
		ELSE
		BEGIN
			UPDATE [CMS].[NewsProduct] SET SortOrder = NewsProductsSort.Sort * 10
			FROM ( 
				SELECT ProductId as Id, ROW_NUMBER() OVER (ORDER BY SortOrder) AS Sort FROM [CMS].[NewsProduct]
				WHERE NewsId = @newsId
			) NewsProductsSort INNER JOIN [CMS].[NewsProduct] ON NewsProductsSort.Id = [NewsProduct].ProductId

			SELECT @prevSort = (SELECT SortOrder FROM [CMS].[NewsProduct] WHERE ProductId = @prevId AND NewsId = @newsId)
			SELECT @nextSort = (SELECT SortOrder FROM [CMS].[NewsProduct] WHERE ProductId = @nextId AND NewsId = @newsId)

			SELECT @NewSort = (@prevSort + ((@nextSort - @prevSort) / 2))
		END
	END

	if @NewSort IS NOT NULL
		UPDATE [CMS].[NewsProduct] SET SortOrder = @NewSort WHERE ProductId = @Id AND NewsId = @newsId
END

GO--

Delete from Customers.Region Where RegionName = 'Хэбэй'

GO--

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Регулирование скидок на товары по категориям' WHERE [ResourceKey] = 'Admin.Settings.Catalog.CategoryDiscountRegulation' AND LanguageId = 1

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Применить скидку' WHERE [ResourceKey] = 'Admin.Settings.Catalog.PriceRegulation.ChangeDiscounts' AND LanguageId = 1

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1, 'Admin.Settings.Catalog.CategoryDiscountRegulationHint', 'Скидка будет применена к товару, если выбранная категория для него основная. <a href="https://www.advantshop.net/help/pages/catalog-main" target="_blank">Подробнее об основных категориях</a>')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2, 'Admin.Settings.Catalog.CategoryDiscountRegulationHint', 'The discount will be applied to the product if the selected category for it is the main one. <a href="https://www.advantshop.net/help/pages/catalog-main" target="_blank">Read more about main category</a>')

GO--

ALTER TABLE [Catalog].[DeletedProducts]
ALTER COLUMN [ArtNo] nvarchar(100) NOT NULL

GO--

ALTER PROCEDURE [Catalog].[sp_AddPhoto] 
	@ObjId INT, @Description NVARCHAR(255),  
	@OriginName NVARCHAR(255),  
	@Type NVARCHAR(50),  
	@Extension NVARCHAR(10),  
	@ColorID int,  
	@PhotoSortOrder int,
	@PhotoNameSize1 NVARCHAR(255),	
	@PhotoNameSize2 NVARCHAR(255)
AS  
BEGIN  
	DECLARE @PhotoId int  
	DECLARE @ismain bit  
	SET @ismain = 1  
	
	IF EXISTS(SELECT * FROM [Catalog].[Photo] WHERE ObjId = @ObjId and [Type]=@Type AND main = 1)  
		SET @ismain = 0  

	INSERT INTO [Catalog].[Photo] ([ObjId],[PhotoName],[Description],[ModifiedDate],[PhotoSortOrder],[Main],[OriginName],[Type],[ColorID], PhotoNameSize1, PhotoNameSize2)  
		VALUES (@ObjId,'none',@Description,Getdate(),@PhotoSortOrder,@ismain,@OriginName,@Type,@ColorID, @PhotoNameSize1, @PhotoNameSize2)  

	SET @PhotoId = Scope_identity()  
	DECLARE @newphoto NVARCHAR(255)  
	Set @newphoto=Convert(NVARCHAR(255),@PhotoId)+@Extension  
	
	UPDATE [Catalog].[Photo] SET [PhotoName] = @newphoto WHERE [PhotoId] = @PhotoId

	SELECT * FROM [Catalog].[Photo] WHERE [PhotoId] = @PhotoId
	--select @newphoto  
END  

GO--

ALTER TABLE Customers.CustomerField ADD
	ShowInCheckout bit NULL

GO--

Update Customers.CustomerField Set ShowInCheckout = ShowInClient 

GO--

ALTER TABLE Customers.CustomerField
ALTER COLUMN ShowInCheckout bit NOT NULL

GO--

IF not exists(Select 1 From sys.columns Where Name = N'ShowInRegistration' AND Object_ID = Object_ID(N'Customers.CustomerField'))
Begin
	EXEC sp_RENAME 'Customers.CustomerField.ShowInClient' , 'ShowInRegistration', 'COLUMN'
End

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1, 'Admin.Js.SettingsCustomers.RequestFromBuyerInRegistration', 'Запрашивать при регистрации')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2, 'Admin.Js.SettingsCustomers.RequestFromBuyerInRegistration', 'Ask the buyer at registration')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1, 'Admin.Js.SettingsCustomers.RequestFromBuyerInCheckout', 'Запрашивать при оформлении заказа')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2, 'Admin.Js.SettingsCustomers.RequestFromBuyerInCheckout', 'Ask the buyer in checkout page')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1, 'Admin.Js.AddEditCustomerField.RequestInRegistration', 'Запрашивать у покупателя при регистрации')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2, 'Admin.Js.AddEditCustomerField.RequestInRegistration', 'Ask the buyer at registration')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1, 'Admin.Js.AddEditCustomerField.RequestInCheckout', 'Запрашивать у покупателя при оформлении заказа')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2, 'Admin.Js.AddEditCustomerField.RequestInCheckout', 'Ask the buyer in checkout page')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1, 'Admin.Js.AddEditCustomerField.DisableCustomerEditing', 'Запретить редактирование в личном кабинете пользвателя')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2, 'Admin.Js.AddEditCustomerField.DisableCustomerEditing', 'Deny editing in the user''s account page')

GO--

ALTER TABLE Customers.CustomerField ADD
	DisableCustomerEditing bit NULL
GO--

Update Customers.CustomerField Set DisableCustomerEditing = 0

GO--

ALTER TABLE Customers.CustomerField 
ALTER COLUMN DisableCustomerEditing bit NOT NULL

GO--


ALTER TABLE [Catalog].[Category] ADD AutomapAction int NULL
GO--

IF NOT EXISTS (SELECT 1 FROM [Catalog].[Category] WHERE AutomapAction IS NOT NULL)
BEGIN
	UPDATE [Catalog].[Category] SET AutomapAction = (CASE WHEN NOT EXISTS (SELECT 1 FROM Catalog.CategoriesAutoMapping WHERE CategoryId = Category.CategoryID) THEN 0 ELSE MoveProductsOnAutomap + 1 END)
END
GO--


ALTER TABLE Catalog.CategoriesAutoMapping ADD
	Id int NOT NULL IDENTITY (1, 1)

GO--

delete from [Catalog].[CategoriesAutoMapping]
where Id in (
SELECT id
  FROM [Catalog].[CategoriesAutoMapping]
  where CategoryId = NewCategoryId
  )
GO--

ALTER TABLE [Catalog].[Category] DROP CONSTRAINT DF_Category_MoveProductsOnAutomap
GO--
ALTER TABLE [Catalog].[Category] DROP COLUMN MoveProductsOnAutomap
GO--
ALTER TABLE [Catalog].[Category] ADD CONSTRAINT DF_Category_AutomapAction  DEFAULT ((0)) FOR AutomapAction
GO--

ALTER PROCEDURE [Catalog].[sp_AddCategory]
	@Name nvarchar(255),
	@ParentCategory int,
	@Description nvarchar(max),
	@BriefDescription nvarchar(max),
	@SortOrder int,
	@Enabled bit,
	@Hidden bit,
	@DisplayStyle int,
	@DisplayChildProducts bit,
	@DisplayBrandsInMenu bit,
	@DisplaySubCategoriesInMenu bit,
	@UrlPath nvarchar(150),
	@Sorting int,
	@ExternalId nvarchar(50),
	@AutomapAction int
AS
BEGIN
	INSERT INTO [Catalog].[Category]
		([Name]
		,[ParentCategory]
		,[Description]
		,[BriefDescription]
		,[Products_Count]
		,[SortOrder]
		,[Enabled]
		,[Hidden]
		,[DisplayStyle]
		,[DisplayChildProducts]
		,[DisplayBrandsInMenu]
		,[DisplaySubCategoriesInMenu]
		,[UrlPath]
		,[Sorting]
		,[ExternalId]
		,[AutomapAction]
		)
	VALUES
		(@Name
		,@ParentCategory
		,@Description
		,@BriefDescription
		,0
		,@SortOrder
		,@Enabled
		,@Hidden
		,@DisplayStyle
		,@DisplayChildProducts
		,@DisplayBrandsInMenu
		,@DisplaySubCategoriesInMenu
		,@UrlPath
		,@Sorting
		,@ExternalId
		,@AutomapAction
		);

	DECLARE @CategoryId int = @@IDENTITY;
	if @ExternalId is null
		begin
			UPDATE [Catalog].[Category] SET [ExternalId] = @CategoryId WHERE [CategoryID] = @CategoryId
		end
	Select @CategoryId;   
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
	@Hidden bit,
	@DisplayStyle int,
	@DisplayChildProducts bit = '0',
	@DisplayBrandsInMenu bit,
	@DisplaySubCategoriesInMenu bit,
	@UrlPath nvarchar(150),
	@Sorting int,
	@ExternalId nvarchar(50),
	@AutomapAction int
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
		,[Hidden] = @Hidden
		,[DisplayStyle] = @DisplayStyle
		,[DisplayChildProducts] = @DisplayChildProducts
		,[DisplayBrandsInMenu] = @DisplayBrandsInMenu
		,[DisplaySubCategoriesInMenu] = @DisplaySubCategoriesInMenu
		,[UrlPath] = @UrlPath
		,[Sorting] = @Sorting
		,[ExternalId] = @ExternalId
		,[AutomapAction] = @AutomapAction
	WHERE CategoryID = @CategoryID
END
GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Catalog.ECategoryAutomapAction.None', 'Ничего не делать')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Catalog.ECategoryAutomapAction.None', 'Do nothing')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Catalog.ECategoryAutomapAction.Copy', 'Копировать товары')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Catalog.ECategoryAutomapAction.Copy', 'Copy products')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Catalog.ECategoryAutomapAction.Move', 'Перемещать товары')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Catalog.ECategoryAutomapAction.Move', 'Move products')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Catalog.ECategoryAutomapAction.CopyAndSetMain', 'Назначить текущую категорию основной')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Catalog.ECategoryAutomapAction.CopyAndSetMain', 'Set current category as main')
DELETE FROM Settings.Localization where ResourceKey = 'Admin.Category.CategoryAutomap.CopyProducts' or ResourceKey = 'Admin.Category.CategoryAutomap.MoveProducts'
GO--

ALTER TABLE Bonus.Card ADD
	DateLastNotifyBonusWipe datetime NULL

GO--

UPDATE Bonus.Card
SET DateLastWipeBonus = ISNULL((SELECT TOP (1) RuleLog.Created FROM Bonus.RuleLog WHERE RuleLog.RuleType=2 and RuleLog.CardId=Card.CardId ORDER BY RuleLog.Created DESC), Card.CreateOn)
WHERE DateLastWipeBonus is null

GO--

ALTER TABLE Bonus.AdditionBonus ADD
	NotifiedAboutExpiry bit NULL

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.ExportImport.MultiOrder.CouponCode', 'Код купона')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.ExportImport.MultiOrder.CouponCode', 'Coupon code')
GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Checkout.PrintOrderMapApiKey', 'API-ключ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Checkout.PrintOrderMapApiKey', 'API-key')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Checkout.PrintOrderMapApiKeyHint', 'Для работы с картами может понадобиться API-ключ. Для Яндекс карт его можно <a target="_blank" href="https://developer.tech.yandex.ru/services/">получить здесь</a>')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Checkout.PrintOrderMapApiKeyHint', 'You can create API-key for yandex map <a target="_blank" href="https://developer.tech.yandex.ru/services/">here</a>')

GO--

Update [Settings].[Localization] Set [ResourceValue]='Новые покупатели' Where [LanguageId]=1 and [ResourceKey]='Admin.Js.RFM.Customers'
Update [Settings].[Localization] Set [ResourceValue]='New customers' Where [LanguageId]=2 and [ResourceKey]='Admin.Js.RFM.Customers'

GO--


INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Common.Common.UseMobilePhone', 'Данный номер используется для мобильной версии.')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Common.Common.UseMobilePhone', 'This number is used for the mobile version.')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Common.Common.UseMobilePhoneExample', 'Пример заполнения: 74950000000')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Common.Common.UseMobilePhoneExample', 'Example: 74950000000')

GO--

UPDATE [Settings].[Localization] SET [ResourceKey] = 'Admin.Js.SettingsCheckout.HasProducts', [ResourceValue] = 'Налог используется в товарах' WHERE [ResourceKey] = 'Admin.Js.SettingsCheckout.TaxCanNotBeDeleted' AND [LanguageId] = '1'
UPDATE [Settings].[Localization] SET [ResourceKey] = 'Admin.Js.SettingsCheckout.HasProducts', [ResourceValue] = 'Tax is used in products' WHERE [ResourceKey] = 'Admin.Js.SettingsCheckout.TaxCanNotBeDeleted' AND [LanguageId] = '2'
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.SettingsCheckout.IsDefault', 'Налог по умолчанию')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.SettingsCheckout.IsDefault', 'Tax is default')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.SettingsCheckout.UsedInCertificates', 'Налог используется в подарочных сертификатах')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.SettingsCheckout.UsedInCertificates', 'Tax is used in gift certificates')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.SettingsCheckout.UsedInPaymentMethods', 'Налог используется в способах оплаты')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.SettingsCheckout.UsedInPaymentMethods', 'Tax is used in payment methods')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.SettingsCheckout.UsedInShippingMethods', 'Налог используется в методах доставки')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.SettingsCheckout.UsedInShippingMethods', 'Tax is used in shipping methods')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.SettingsCheckout.ChangeEnabledIsImpossible', 'Изменить активность налога невозможно')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.SettingsCheckout.ChangeEnabledIsImpossible', 'Impossible to change tax activity')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.SettingsCheckout.ChangeEnabledIsImpossible', 'Изменить активность налога невозможно, налог используется')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.SettingsCheckout.ChangeEnabledIsImpossible', 'Impossible to change tax activity, tax is used')

GO--

IF not exists(Select 1 From sys.columns Where Name = N'TaxId' AND Object_ID = Object_ID(N'Order.ShippingMethod'))
Begin
	UPDATE [Order].[ShippingMethod] SET [TaxType] = (SELECT TOP(1) [TaxID] FROM [Catalog].[Tax] WHERE [Tax].[TaxType] = [ShippingMethod].[TaxType]) WHERE [TaxType] != 0
	UPDATE [Order].[ShippingMethod] SET [TaxType] = NULL WHERE [TaxType] = 0
	EXEC sp_rename 'Order.ShippingMethod.TaxType', 'TaxId', 'COLUMN'
End

GO--


ALTER PROCEDURE [Catalog].[sp_AddPhoto] 
	@ObjId INT, @Description NVARCHAR(255),  
	@OriginName NVARCHAR(255),  
	@Type NVARCHAR(50),  
	@Extension NVARCHAR(10),  
	@ColorID int,  
	@PhotoSortOrder int,
	@PhotoName NVARCHAR(255) = 'none',
	@PhotoNameSize1 NVARCHAR(255),	
	@PhotoNameSize2 NVARCHAR(255)
AS  
BEGIN  
	DECLARE @PhotoId int  
	DECLARE @ismain bit  
	SET @ismain = 1  
	
	IF EXISTS(SELECT * FROM [Catalog].[Photo] WHERE ObjId = @ObjId and [Type]=@Type AND main = 1)  
		SET @ismain = 0  

	if @PhotoName is null 
	begin
	set @PhotoName = 'none'
	end

	INSERT INTO [Catalog].[Photo] ([ObjId],[PhotoName],[Description],[ModifiedDate],[PhotoSortOrder],[Main],[OriginName],[Type],[ColorID], PhotoNameSize1, PhotoNameSize2)  
		VALUES (@ObjId, @PhotoName,@Description,Getdate(),@PhotoSortOrder,@ismain,@OriginName,@Type,@ColorID, @PhotoNameSize1, @PhotoNameSize2)  

	SET @PhotoId = Scope_identity()  
	if @PhotoName = 'none'
	begin
		
		DECLARE @newphoto NVARCHAR(255)  
		Set @newphoto=Convert(NVARCHAR(255),@PhotoId)+@Extension  
	
		UPDATE [Catalog].[Photo] SET [PhotoName] = @newphoto WHERE [PhotoId] = @PhotoId
	end

	SELECT * FROM [Catalog].[Photo] WHERE [PhotoId] = @PhotoId
	--select @newphoto  
END  

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.Orders.OrderItems', 'Состав заказа')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.Orders.OrderItems', 'Order items')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Orders.OrderItemsModel.Certificate', 'Сертификат')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Orders.OrderItemsModel.Certificate', 'Certificate')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.Orders.AdminOrderComment', 'Комментарий администратора')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.Orders.AdminOrderComment', 'Administrator comment')

GO--

ALTER TABLE [Customers].[City] ADD [Zip] nvarchar(50) NULL
GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.SettingsSystem.Zip', 'Индекс')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.SettingsSystem.Zip', 'Zip')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.AddEditCitys.Zip', 'Индекс')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.AddEditCitys.Zip', 'Zip')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Product.Index.ShippingTo.ChooseCity', 'Выберите город')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Product.Index.ShippingTo.ChooseCity', 'Choose a City')
GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.PartnerBalance.RewardPayout', 'Выплата вознаграждения')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.PartnerBalance.RewardPayout', 'Reward payout')
GO--

ALTER TABLE Catalog.Product ADD YandexProductDiscounted bit NULL
ALTER TABLE Catalog.Product ADD YandexProductDiscountCondition nvarchar(10) NULL
ALTER TABLE Catalog.Product ADD YandexProductDiscountReason nvarchar(MAX) NULL

GO--

ALTER PROCEDURE [Catalog].[sp_UpdateProductById]
    @ProductID int,
    @ArtNo nvarchar(100),
    @Name nvarchar(255),
    @Ratio float,
    @Discount float,
    @DiscountAmount float,
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
    @PaymentSubjectType int,
    @PaymentMethodType int,
    @YandexSizeUnit nvarchar(10),
    @DateModified datetime,
    @YandexName nvarchar(255),
    @YandexDeliveryDays nvarchar(5),
    @CreatedBy nvarchar(50),
    @Hidden bit,
    @ManualRatio float,
	@YandexProductDiscounted bit,
	@YandexProductDiscountCondition nvarchar(10),
	@YandexProductDiscountReason nvarchar(MAX)
AS
BEGIN
    UPDATE [Catalog].[Product]
    SET 
         [ArtNo] = @ArtNo
        ,[Name] = @Name
        ,[Ratio] = @Ratio
        ,[Discount] = @Discount
        ,[DiscountAmount] = @DiscountAmount
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
        ,[PaymentSubjectType] = @PaymentSubjectType
        ,[PaymentMethodType] = @PaymentMethodType
        ,[YandexSizeUnit] = @YandexSizeUnit
        ,[YandexName] = @YandexName
        ,[YandexDeliveryDays] = @YandexDeliveryDays
        ,[CreatedBy] = @CreatedBy
        ,[Hidden] = @Hidden
        ,[Manualratio] = @ManualRatio
		,[YandexProductDiscounted] = @YandexProductDiscounted
		,[YandexProductDiscountCondition] = @YandexProductDiscountCondition
		,[YandexProductDiscountReason] = @YandexProductDiscountReason
    WHERE ProductID = @ProductID
END

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
			,[Offer].[Weight]
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
			,[Offer].Length
			,[Offer].Width
			,[Offer].Height
			,p.YandexProductDiscounted
			,p.YandexProductDiscountCondition
			,p.YandexProductDiscountReason
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

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.ExportOptions.YandexProductDiscounted', 'Яндекс-маркет уцененный товар')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.ExportOptions.YandexProductDiscounted', 'Yandex-market product discounted')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.ExportOptions.YandexProductDiscounted.HelpTitle', 'Уцененный товар')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.ExportOptions.YandexProductDiscounted.HelpTitle', 'Product discounted')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.ExportOptions.YandexProductDiscounted.YouCanPlaceDiscountedProducts', 'Во многих категориях на Маркете вы можете размещать уцененные товары.')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.ExportOptions.YandexProductDiscounted.YouCanPlaceDiscountedProducts', 'In many categories on the Market, you can place discounted products.')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.ExportOptions.YandexProductDiscounted.BothLikeNewAndUsedProducts', 'Это могут быть как новые товары, так и бывшие в употреблении, при этом товар должен быть пригоден для использования по прямому назначению.')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.ExportOptions.YandexProductDiscounted.BothLikeNewAndUsedProducts', 'This can be both like new products and used ones, while the product must be suitable for use for its intended purpose.')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.ExportOptions.YandexProductDiscountCondition', 'Яндекс-маркет состояние товара')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.ExportOptions.YandexProductDiscountCondition', 'Yandex-market discount condition')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.ExportOptions.YandexProductDiscountCondition.LikeNew', 'Как новый')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.ExportOptions.YandexProductDiscountCondition.LikeNew', 'Like new')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.ExportOptions.YandexProductDiscountCondition.Used', 'Подержанный')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.ExportOptions.YandexProductDiscountCondition.Used', 'Used')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.ExportOptions.YandexProductDiscountCondition.HelpTitle', 'Состояние товара')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.ExportOptions.YandexProductDiscountCondition.HelpTitle', 'Discount condition')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.ExportOptions.YandexProductDiscountCondition.NewLikeType', 'Новые товары — не были в употреблении и уценены из-за недостатков')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.ExportOptions.YandexProductDiscountCondition.NewLikeType', 'Like new products - were not in use and discounted due to deficiencies')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.ExportOptions.YandexProductDiscountCondition.NewLikeType.DamagedPackage', 'С поврежденной или вскрытой упаковкой, без упаковки.')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.ExportOptions.YandexProductDiscountCondition.NewLikeType.DamagedPackage', 'With damaged or opened packaging, without packaging.')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.ExportOptions.YandexProductDiscountCondition.NewLikeType.InIncompleteConfiguration', 'В неполной комплектации.')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.ExportOptions.YandexProductDiscountCondition.NewLikeType.InIncompleteConfiguration', 'In incomplete configuration.')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.ExportOptions.YandexProductDiscountCondition.NewLikeType.WithScratches', 'С царапинами, потертостями, следами тестирования или примерки.')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.ExportOptions.YandexProductDiscountCondition.NewLikeType.WithScratches', 'With scratches, scuffs, traces of testing or fitting.')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.ExportOptions.YandexProductDiscountCondition.NewLikeType.DamagedOrDefective', 'Поврежденные или бракованные, но полностью работоспособные.')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.ExportOptions.YandexProductDiscountCondition.NewLikeType.DamagedOrDefective', 'Damaged or defective, but fully functional.')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.ExportOptions.YandexProductDiscountCondition.NewLikeType.Renovated', 'Отремонтированные магазином или сервисом до продажи потребителю (например, заводской брак).')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.ExportOptions.YandexProductDiscountCondition.NewLikeType.Renovated', 'Renovated by a store or service prior to sale to the consumer (e.g. factory defect).')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.ExportOptions.YandexProductDiscountCondition.NewLikeType.ShowcaseSamples', 'Витринные образцы')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.ExportOptions.YandexProductDiscountCondition.NewLikeType.ShowcaseSamples', 'Showcase samples.')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.ExportOptions.YandexProductDiscountCondition.UsedType', 'Товары, бывшие в употреблении (подержанные)')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.ExportOptions.YandexProductDiscountCondition.UsedType', 'Used products (second-hand)')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.ExportOptions.YandexProductDiscountCondition.UsedType.Workable', 'Товары, которые были в употреблении и сохранили свою работоспособность, включая товары, отремонтированные после использования потребителем, товары по программам обмена старого товара на новый.')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.ExportOptions.YandexProductDiscountCondition.UsedType.Workable', 'Products that were in use and maintained their operability, including products repaired after use by the consumer, products under programs for exchanging old products for new ones.')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.ExportOptions.YandexProductDiscountReason', 'Яндекс-маркет причина уценки')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.ExportOptions.YandexProductDiscountReason', 'Yandex-market discount reason')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.ExportOptions.YandexProductDiscountReason.HelpTitle', 'Причина уценки')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.ExportOptions.YandexProductDiscountReason.HelpTitle', 'Discount reason')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.ExportOptions.YandexProductDiscountReason.Attention', 'Внимание!')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.ExportOptions.YandexProductDiscountReason.Attention', 'Attention!')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.ExportOptions.YandexProductDiscountReason.InformationMustBeComprehensive', 'Информация о причинах уценки должна быть исчерпывающей. Тексты не должны быть уклончивыми. Например, нельзя писать: «Причины уценки узнавайте у консультанта».')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.ExportOptions.YandexProductDiscountReason.InformationMustBeComprehensive', 'Information on the reasons for markdowns should be exhaustive. Texts should not be evasive. For example, you can’t write: “Ask the consultant for the reasons of the discount.')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.Product.YandexProductDiscountReason.Lengthy', 'Длинна текст причины уценки должна быть не более 3000 символов (включая знаки препинания).')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.Product.YandexProductDiscountReason.Lengthy', 'The text of the markdown reason should be no longer than 3000 characters (including punctuation marks).')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Crm.ELeadFieldType.UseIn1C', 'Выгружать заказ в 1С')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Crm.ELeadFieldType.UseIn1C', 'Export order in 1C')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Search.SearchBlock.Example', 'Например')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Search.SearchBlock.Example', 'Example')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.RFM.OrdersCount', 'Всего заказов')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.RFM.OrdersCount', 'Orders count')

GO--

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Интернет-магазин - Параметры магазина - Карточка товара - Перекрестный маркетинг' WHERE [ResourceKey] = 'Admin.Category.PropertyGroups.SettingsCatalog' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'Store - Store options - Product - Cross/Up sale' WHERE [ResourceKey] = 'Admin.Category.PropertyGroups.SettingsCatalog' AND [LanguageId] = 2

GO--



ALTER PROCEDURE [Catalog].[PreCalcProductParams] 
	@productId INT,  
    @ModerateReviews BIT,  
    @OnlyAvailable BIT,
	@ComplexFilter BIT
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
		 ) IS NOT NULL AND @ComplexFilter = 1
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
		 SELECT [Settings].[ProductColorsToString](@productId, @OnlyAvailable)  
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
             SELECT CASE WHEN [Product].Discount > 0 THEN (@MinPrice - @MinPrice * [Product].Discount / 100) * CurrencyValue ELSE (@MinPrice - isnull([Product].DiscountAmount, 0)) * CurrencyValue END
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

ALTER PROCEDURE [Catalog].[PreCalcProductParamsMass] @ModerateReviews BIT, @OnlyAvailable bit,  @ComplexFilter BIT AS 
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
		CountPhoto=case when offerId.ColorID is null OR @ComplexFilter = 0 then countNocolor.countNocolor else countColor.countColor end,				
		PhotoId=case when offerId.ColorID is null then PhotoIdNoColor.PhotoIdNoColor else PhotoIdColor.PhotoIdColor end,
		[VideosAvailable]=isnull(videosAvailable.videosAvailable,0),
		[MaxAvailable]=maxAvailable.maxAvailable,
		[NotSamePrices]=isnull(notSamePrices.notSamePrices,0),
		[MinPrice]=isnull(minPrice.minPrice,0),
		[OfferId]=offerId.OfferId,
		[Comments]=isnull(comments.comments,0),
		[Gifts]=isnull(gifts.gifts,0),
		[Colors]=(SELECT [Settings].[ProductColorsToString](pe.ProductId, @OnlyAvailable)),
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

IF NOT EXISTS (SELECT * FROM [Settings].[Localization] WHERE ResourceKey = 'Admin.Js.Cards.AccrueBonuses' AND [LanguageId] = 1)
BEGIN
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.Cards.AccrueBonuses', 'Доначислить к текущим бонусам')
END
IF NOT EXISTS (SELECT * FROM [Settings].[Localization] WHERE ResourceKey = 'Admin.Js.Cards.AccrueBonuses' AND [LanguageId] = 2)
BEGIN
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.Cards.AccrueBonuses', 'Accrue bonuses for existing cards')
END

GO--

ALTER PROCEDURE [Catalog].[sp_GetChildCategoriesByParentIDForMenu]  
 @CurrentCategoryID int  
AS  
BEGIN  
   
 IF (Select COUNT(CategoryID) From Catalog.Category Where ParentCategory = @CurrentCategoryID and Enabled = 1 and Hidden = 0) > 0   
  BEGIN  
   SELECT   
    [CategoryID],  
    [Name],  
    [Description],  
    [BriefDescription],  
    [ParentCategory], 
    [Products_Count],  
    [Total_Products_Count],  
    Available_Products_Count,  
    [SortOrder],  
    [Enabled],  
    [Hidden],
    [DisplayStyle],   
    [DisplayBrandsInMenu],  
    [DisplaySubCategoriesInMenu],  
    [UrlPath],  
    (SELECT Count(CategoryID) FROM [Catalog].[Category] AS c WHERE c.ParentCategory = cat.CategoryID and Enabled = 1 and Hidden = 0) AS [ChildCategories_Count]   
   FROM [Catalog].[Category] AS cat   
   WHERE [ParentCategory] = @CurrentCategoryID AND CategoryID <> 0 and Enabled = 1 AND Hidden = 0 AND HirecalEnabled = 1  
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
    [Products_Count],  
    [Total_Products_Count],  
    Available_Products_Count,  
    [SortOrder],  
    [Enabled],  
    [Hidden],  
    [DisplayStyle],   
    [DisplayBrandsInMenu],  
    [DisplaySubCategoriesInMenu],      
    [UrlPath],  
    (SELECT Count(CategoryID) FROM [Catalog].[Category] AS c WHERE c.ParentCategory = cat.CategoryID and Enabled = 1 and Hidden = 0) AS [ChildCategories_Count]   
   FROM [Catalog].[Category] AS cat WHERE [ParentCategory] = (Select ParentCategory From Catalog.Category   
   Where CategoryID = @CurrentCategoryID) AND CategoryID <> 0 and Enabled = 1 AND Hidden = 0 AND HirecalEnabled = 1  
   ORDER BY SortOrder, Name  
  END
END

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Settings.SettingsCatalog.ProductViewMode.Single', 'Блоки')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Settings.SettingsCatalog.ProductViewMode.Single', 'Single')

GO--

UPDATE [Settings].[Localization] SET [ResourceValue] = 'доступно ещё {0}' WHERE [LanguageId] = '1' AND [ResourceKey] = 'Admin.Orders.GetOrderItems.AvailableLimit' 

GO--

If not Exists(Select 1 From [Settings].[Settings] Where [Name] = 'IsLimitedPhotoNameLength')
	Insert Into [Settings].[Settings] ([Name],[Value]) Values ('IsLimitedPhotoNameLength', 'True')

GO--

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Рекомендуемые размеры изображения для десктопной версии:<br>1160px * 553px - одна колонка,<br>865px * 554px - две колонки' WHERE [LanguageId] = '1' AND [ResourceKey] = 'Admin.Js.AddEditCarousel.Size' 
UPDATE [Settings].[Localization] SET [ResourceValue] = 'Recommended image sizes for desktop version:<br>1160px * 553px - one column,<br>865px * 554px - two speakers' WHERE [LanguageId] = '2' AND [ResourceKey] = 'Admin.Js.AddEditCarousel.Size' 

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.AddEditCarousel.MobileSize', 'Рекомендуемые размеры изображения для мобильной версии:<br>375px * 150px')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.AddEditCarousel.MobileSize', 'Recommended image sizes for mobile version:<br>375px * 150px')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.CommonStatistic.AlreadyRunning','Уже запущена другая фоновая задача.')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.CommonStatistic.AlreadyRunning','Another task already running.')
GO--

if not exists (Select 1 From [Settings].[Localization] Where [ResourceKey] = 'User.Login.SignInFunnel')
begin
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'User.Login.SignInFunnel', 'Войти на страницу')
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'User.Login.SignInFunnel', 'Sign in the page')
end

GO--

if not exists (Select 1 From [Settings].[Localization] Where [ResourceKey] = 'Js.Shipping.DeliveryName')
begin
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Js.Shipping.DeliveryName', 'Название')
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Js.Shipping.DeliveryName', 'Name')
end

GO--

if not exists (Select 1 From [Settings].[Localization] Where [ResourceKey] = 'Js.Shipping.DeliveryCost')
begin
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Js.Shipping.DeliveryCost', 'Стоимость')
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Js.Shipping.DeliveryCost', 'Cost')
end

GO--

INSERT INTO [Settings].[TemplateSettings] ([Template], [Name], [Value]) VALUES ('_default', 'Mobile_BrowserColorVariantsSelected', 'ColorScheme')
INSERT INTO [Settings].[TemplateSettings] ([Template], [Name], [Value]) VALUES ('_default', 'Mobile_BrowserColor', '')
INSERT INTO [Settings].[TemplateSettings] ([Template], [Name], [Value]) VALUES ('_default', 'Mobile_HeaderColorVariantsSelected', 'ColorScheme')
INSERT INTO [Settings].[TemplateSettings] ([Template], [Name], [Value]) VALUES ('_default', 'Mobile_LevelMenu', '5')
INSERT INTO [Settings].[TemplateSettings] ([Template], [Name], [Value]) VALUES ('_default', 'Mobile_ViewCategoriesOnMain', 'Default')
INSERT INTO [Settings].[TemplateSettings] ([Template], [Name], [Value]) VALUES ('_default', 'Mobile_MainPageProductsCount', '3')
INSERT INTO [Settings].[TemplateSettings] ([Template], [Name], [Value]) VALUES ('_default', 'Mobile_LogoType', 'Text')
INSERT INTO [Settings].[TemplateSettings] ([Template], [Name], [Value]) VALUES ('_default', 'Mobile_ShowMenuLinkAll', 'False')
INSERT INTO [Settings].[TemplateSettings] ([Template], [Name], [Value]) VALUES ('_default', 'Mobile_ShowBreadsCrumbsInProductPage', 'True')
INSERT INTO [Settings].[TemplateSettings] ([Template], [Name], [Value]) VALUES ('_default', 'Mobile_DefaultCatalogView', '0')
INSERT INTO [Settings].[TemplateSettings] ([Template], [Name], [Value]) VALUES ('_default', 'Mobile_EnableCatalogViewChange', 'True')
INSERT INTO [Settings].[TemplateSettings] ([Template], [Name], [Value]) VALUES ('_default', 'Mobile_CountLinesProductName', '3')
INSERT INTO [Settings].[TemplateSettings] ([Template], [Name], [Value]) VALUES ('_default', 'Mobile_DisplayCity', 'True')
INSERT INTO [Settings].[TemplateSettings] ([Template], [Name], [Value]) VALUES ('_default', 'Mobile_DisplaySlider', 'True')
INSERT INTO [Settings].[TemplateSettings] ([Template], [Name], [Value]) VALUES ('_default', 'Mobile_DisplayHeaderTitle', 'True')
INSERT INTO [Settings].[TemplateSettings] ([Template], [Name], [Value]) VALUES ('_default', 'Mobile_HeaderCustomTitle', '')

GO--

INSERT INTO [Settings].[TemplateSettings] ([Template], [Name], [Value]) VALUES ('_default', 'Mobile_BrowserColorVariantsSelected_Modern', 'ColorScheme')
INSERT INTO [Settings].[TemplateSettings] ([Template], [Name], [Value]) VALUES ('_default', 'Mobile_BrowserColor_Modern', '0662c1')
INSERT INTO [Settings].[TemplateSettings] ([Template], [Name], [Value]) VALUES ('_default', 'Mobile_HeaderColorVariantsSelected_Modern', 'ColorScheme')
INSERT INTO [Settings].[TemplateSettings] ([Template], [Name], [Value]) VALUES ('_default', 'Mobile_LevelMenu_Modern', '5')
INSERT INTO [Settings].[TemplateSettings] ([Template], [Name], [Value]) VALUES ('_default', 'Mobile_ViewCategoriesOnMain_Modern', 'Default')
INSERT INTO [Settings].[TemplateSettings] ([Template], [Name], [Value]) VALUES ('_default', 'Mobile_MainPageProductsCount_Modern', '9')
INSERT INTO [Settings].[TemplateSettings] ([Template], [Name], [Value]) VALUES ('_default', 'Mobile_ShowAddButton_Modern', 'True')
INSERT INTO [Settings].[TemplateSettings] ([Template], [Name], [Value]) VALUES ('_default', 'Mobile_LogoType_Modern', 'Text')
INSERT INTO [Settings].[TemplateSettings] ([Template], [Name], [Value]) VALUES ('_default', 'Mobile_ShowMenuLinkAll_Modern', 'False')
INSERT INTO [Settings].[TemplateSettings] ([Template], [Name], [Value]) VALUES ('_default', 'Mobile_ShowBreadsCrumbsInProductPage_Modern', 'True')
INSERT INTO [Settings].[TemplateSettings] ([Template], [Name], [Value]) VALUES ('_default', 'Mobile_DefaultCatalogView_Modern', '0')
INSERT INTO [Settings].[TemplateSettings] ([Template], [Name], [Value]) VALUES ('_default', 'Mobile_EnableCatalogViewChange_Modern', 'True')
INSERT INTO [Settings].[TemplateSettings] ([Template], [Name], [Value]) VALUES ('_default', 'Mobile_CountLinesProductName_Modern', '3')
INSERT INTO [Settings].[TemplateSettings] ([Template], [Name], [Value]) VALUES ('_default', 'Mobile_DisplayCity_Modern', 'True')
INSERT INTO [Settings].[TemplateSettings] ([Template], [Name], [Value]) VALUES ('_default', 'Mobile_DisplaySlider_Modern', 'True')
INSERT INTO [Settings].[TemplateSettings] ([Template], [Name], [Value]) VALUES ('_default', 'Mobile_DisplayHeaderTitle_Modern', 'True')
INSERT INTO [Settings].[TemplateSettings] ([Template], [Name], [Value]) VALUES ('_default', 'Mobile_HeaderCustomTitle_Modern', '')

GO--
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ShippingMethods.Boxberry.ApiUrl.HelpText', 'URL-адрес для API Boxberry зависит от даты регистрации на стороне boxberry<br />Для клиентов зарегистрированных до 01 января 2019 указываем <span  class="bold" >http://api.boxberry.de/json.php</span><br />Для клиентов зарегистрированных после 01 января 2019 указываем  <span class="bold" >http://api.boxberry.ru/json.php </span>')
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ShippingMethods.Boxberry.ApiUrl.HelpText', 'The URL for the Boxberry API depends on the date of registration on the boxberry side <br /> For customers registered before January 01, 2019, specify <span class = "bold"> http://api.boxberry.de/json.php </span> <br /> For customers registered after January 01, 2019, specify <span class = "bold"> http://api.boxberry.ru/json.php </span>')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Services.Features.EFeature.MobileApp', 'Мобильная версия 2019')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Services.Features.EFeature.MobileApp', 'Mobile version 2019')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Services.Features.EFeature.MobileApp.Description', 'Позволяет выбрать современный шаблон мобильной версии и открывает настройки мобильного приложения')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Services.Features.EFeature.MobileApp.Description', 'Allows you to select a modern template for the mobile version and opens the settings of the mobile application')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'MobileApp.Title', 'Мобильное приложение')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'MobileApp.Title', 'Mobile application')
	

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Modules.Market.Buy', 'Купить')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Modules.Market.Buy', 'Buy')	
	
	
GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.PaymentMethods.PSBank.FirstComponent', 'Первая компонента секретного ключа')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.PaymentMethods.PSBank.FirstComponent', 'The first component of the private key')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.PaymentMethods.PSBank.SecondComponent', 'Вторая компонента секретного ключа')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.PaymentMethods.PSBank.SecondComponent', 'The second component of the private key')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.PaymentMethods.PSBank.Terminal', 'Уникальный номер виртуального терминала')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.PaymentMethods.PSBank.Terminal', 'Unique virtual terminal number')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.PaymentMethods.PSBank.Merchant', 'Номер торгово-сервисного предприятия')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.PaymentMethods.PSBank.Merchant', 'Company number')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.PaymentMethods.PSBank.MerchantName', 'Название торгово-сервисного предприятия')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.PaymentMethods.PSBank.MerchantName', 'Name of company')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.PaymentMethods.PSBank.TestMode', 'Тестовый режим')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.PaymentMethods.PSBank.TestMode', 'Test mode')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.CategoriesTree.AvailableProductsCount', 'Товаров на витрине')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.CategoriesTree.AvailableProductsCount', 'Available products count')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.CategoriesTree.TotalProductsCount', 'Товаров всего')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.CategoriesTree.TotalProductsCount', 'Total products count')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.CategoriesTree.ProductsCount.Subtext', 'Сумма количества товаров в этой и дочерних категориях')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.CategoriesTree.ProductsCount.Subtext', 'Current and child categories products count sum')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.CatalogLeftMenu.TotalProductsCount.Tooltip', 'Товаров всего')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.CatalogLeftMenu.TotalProductsCount.Tooltip', 'Total products count')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.CatalogLeftMenu.EnabledProductsCount.Tooltip', 'Активных товаров')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.CatalogLeftMenu.EnabledProductsCount.Tooltip', 'Enabled products count')

GO--

if not exists (select * from [Settings].[Localization] where ResourceKey = 'Js.Payment.BillBy.Company')
begin
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Js.Payment.BillBy.Company', 'Компания')
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Js.Payment.BillBy.Company', 'Company')
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Js.Payment.BillBy.UNP', 'УНП')
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Js.Payment.BillBy.UNP', 'UNP')
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Js.Payment.BillKz.Company', 'Компания')
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Js.Payment.BillKz.Company', 'Company')
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Js.Payment.BillKz.BINIIN', 'БИН/ИИН')
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Js.Payment.BillKz.BINIIN', 'BIN/IIN')
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Js.Payment.BillKz.Contract', 'Договор')
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Js.Payment.BillKz.Contract', 'Contract')
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Js.Payment.Bill.Company', 'Компания')
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Js.Payment.Bill.Company', 'Company')
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Js.Payment.Bill.INN', 'ИНН')
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Js.Payment.Bill.INN', 'INN')
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Js.Payment.Qiwi.Phone', 'Телефон')
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Js.Payment.Qiwi.Phone', 'Phone')
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Js.Payment.SberBank.INN', 'ИНН')
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Js.Payment.SberBank.INN', 'INN')
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Js.Payment.YandexKassa.Phone', 'Телефон')
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Js.Payment.YandexKassa.Phone', 'Phone')
end

GO--

UPDATE [Settings].[Localization] SET ResourceValue = 'Для отображения этого метода оплаты требуется создание метода доставки типа {0}.' WHERE [ResourceKey] = 'Admin.PaymentMethods.CashOnDelivery.NeedToCreateDeliveryMethod' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET ResourceValue = 'To display this payment method, you need to create a delivery method such as {0}.' WHERE [ResourceKey] = 'Admin.PaymentMethods.CashOnDelivery.NeedToCreateDeliveryMethod' AND [LanguageId] = 2

GO--

ALTER TABLE Catalog.Coupon ADD ForFirstOrder bit NULL
GO--
UPDATE Catalog.Coupon SET ForFirstOrder = 0
GO--
ALTER TABLE Catalog.Coupon ALTER COLUMN ForFirstOrder bit NOT NULL
GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.AddEditCoupon.ForFirstOrder', 'Скидка на первый заказ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.AddEditCoupon.ForFirstOrder', 'First order discount')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.Coupons.ForFirstOrder', 'Скидка на первый заказ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.Coupons.ForFirstOrder', 'First order discount')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.Yes', 'Да')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.Yes', 'Yes')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.No', 'Нет')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.No', 'No')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Partners.AutoApplyPartnerCoupon', 'Автоматически применять купон при переходе по реферальной ссылке')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Partners.AutoApplyPartnerCoupon', 'Apply coupon automatically when site visited via referral link')

GO--

IF NOT EXISTS (SELECT 1 FROM Settings.Settings WHERE Name = 'Partners.AutoApplyPartnerCoupon')
BEGIN
	INSERT INTO Settings.Settings (Name, Value) VALUES ('Partners.AutoApplyPartnerCoupon', 'True')
END

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.OrdersItemsSummary.DimensionsSaved', 'Габариты сохранены')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.OrdersItemsSummary.DimensionsSaved', 'Dimensions saved')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.OrdersItemsSummary.ErrorDimensionsSaved', 'Ошибка при сохранении габаритов')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.OrdersItemsSummary.ErrorDimensionsSaved', 'Error while saving dimensions')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.OrdersItemsSummary.InvalidDimensionsValues', 'Поля габаритов должны быть все заполнены или все пусты')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.OrdersItemsSummary.InvalidDimensionsValues', 'Dimension fields must be all filled out or all empty')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.OrdersItemsSummary.WeightSaved', 'Вес сохранен')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.OrdersItemsSummary.WeightSaved', 'Weight saved')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.OrdersItemsSummary.ErrorWeightSaved', 'Ошибка при сохранении веса')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.OrdersItemsSummary.ErrorWeightSaved', 'Weight Loss Error')

GO--

ALTER TABLE Catalog.Category ADD
	DateModified datetime NOT NULL CONSTRAINT DF_Category_DateModified DEFAULT (getdate()),
	DateAdded datetime NOT NULL CONSTRAINT DF_Category_DateAdded DEFAULT (getdate()),
	ModifiedBy nvarchar(50) NULL
GO--

ALTER PROCEDURE [Catalog].[sp_AddCategory]
	@Name nvarchar(255),
	@ParentCategory int,
	@Description nvarchar(max),
	@BriefDescription nvarchar(max),
	@SortOrder int,
	@Enabled bit,
	@Hidden bit,
	@DisplayStyle int,
	@DisplayChildProducts bit,
	@DisplayBrandsInMenu bit,
	@DisplaySubCategoriesInMenu bit,
	@UrlPath nvarchar(150),
	@Sorting int,
	@ExternalId nvarchar(50),
	@AutomapAction int,
	@ModifiedBy nvarchar(50)
AS
BEGIN
	INSERT INTO [Catalog].[Category]
		([Name]
		,[ParentCategory]
		,[Description]
		,[BriefDescription]
		,[Products_Count]
		,[SortOrder]
		,[Enabled]
		,[Hidden]
		,[DisplayStyle]
		,[DisplayChildProducts]
		,[DisplayBrandsInMenu]
		,[DisplaySubCategoriesInMenu]
		,[UrlPath]
		,[Sorting]
		,[ExternalId]
		,[AutomapAction]
		,[ModifiedBy]
		)
	VALUES
		(@Name
		,@ParentCategory
		,@Description
		,@BriefDescription
		,0
		,@SortOrder
		,@Enabled
		,@Hidden
		,@DisplayStyle
		,@DisplayChildProducts
		,@DisplayBrandsInMenu
		,@DisplaySubCategoriesInMenu
		,@UrlPath
		,@Sorting
		,@ExternalId
		,@AutomapAction
		,@ModifiedBy
		);

	DECLARE @CategoryId int = @@IDENTITY;
	if @ExternalId is null
		begin
			UPDATE [Catalog].[Category] SET [ExternalId] = @CategoryId WHERE [CategoryID] = @CategoryId
		end
	Select @CategoryId;   
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
	@Hidden bit,
	@DisplayStyle int,
	@DisplayChildProducts bit = '0',
	@DisplayBrandsInMenu bit,
	@DisplaySubCategoriesInMenu bit,
	@UrlPath nvarchar(150),
	@Sorting int,
	@ExternalId nvarchar(50),
	@AutomapAction int,
	@ModifiedBy nvarchar(50)
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
		,[Hidden] = @Hidden
		,[DisplayStyle] = @DisplayStyle
		,[DisplayChildProducts] = @DisplayChildProducts
		,[DisplayBrandsInMenu] = @DisplayBrandsInMenu
		,[DisplaySubCategoriesInMenu] = @DisplaySubCategoriesInMenu
		,[UrlPath] = @UrlPath
		,[Sorting] = @Sorting
		,[ExternalId] = @ExternalId
		,[AutomapAction] = @AutomapAction
		,[DateModified] = getdate()
		,[ModifiedBy] = @ModifiedBy
	WHERE CategoryID = @CategoryID
END

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.AnalyticsFilter.LastOrderNumber', 'Номер последнего заказа')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.AnalyticsFilter.LastOrderNumber', 'Last order number')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.AnalyticsFilter.LastOrderDate', 'Дата последнего заказа')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.AnalyticsFilter.LastOrderDate', 'Last order date')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.AnalyticsFilter.OrdersCount', 'Кол-во заказов')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.AnalyticsFilter.OrdersCount', 'Orders count')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.AnalyticsFilter.OrdersSum', 'Сумма заказов')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.AnalyticsFilter.OrdersSum', 'Orders sum')

GO--

CREATE PROCEDURE [Settings].[sp_GetCsvMaxProductCategoriesCount] 
	@exportFeedId INT,
	@exportAllProducts BIT,
	@exportNotAvailable BIT
AS
BEGIN
	DECLARE @lcategorytemp TABLE (CategoryId INT)
	DECLARE @l TABLE (categoryid INT PRIMARY KEY CLUSTERED, Opened bit)
    
	INSERT INTO @l
		SELECT t.categoryid, t.Opened
		FROM [Settings].[exportfeedselectedcategories] AS t 
			INNER JOIN catalog.category ON t.categoryid = category.categoryid
		WHERE [exportfeedid] = @exportFeedId 

	DECLARE @l1 INT = (SELECT Min(categoryid) FROM @l)
	WHILE @l1 IS NOT NULL
	BEGIN 
		if ((Select Opened from @l where CategoryId = @l1) = 1)
			INSERT INTO @lcategorytemp SELECT @l1
		else
			INSERT INTO @lcategorytemp SELECT id FROM Settings.GetChildCategoryByParent(@l1)

		SET @l1 = (SELECT Min(categoryid) FROM @l WHERE  categoryid > @l1)
	END

	DECLARE @lcategory TABLE (categoryid INT PRIMARY KEY CLUSTERED);
	INSERT INTO @lcategory SELECT Distinct CategoryId FROM @lcategorytemp

	SELECT TOP 1 COUNT(1) productCategoriesCount
	FROM Catalog.Product p INNER JOIN Catalog.ProductCategories pc ON pc.ProductID = p.ProductId
	WHERE p.ProductID IN (SELECT ProductId FROM @lcategory sel INNER JOIN Catalog.ProductCategories ON ProductCategories.CategoryID = sel.categoryID)
		AND 
		(
			@exportAllProducts = 1 
			OR (
				SELECT Count(productid)
				FROM settings.exportfeedexcludedproducts
				WHERE exportfeedexcludedproducts.productid = p.productid AND exportfeedexcludedproducts.exportfeedid = @exportFeedId
			) = 0
		) AND (
			p.Enabled = 1 OR @exportNotAvailable = 1
		) AND (
			@exportNotAvailable = 1
			OR EXISTS (
				SELECT 1
				FROM [Catalog].[Offer] o
				Where o.[ProductId] = p.[productid] AND o.Price > 0 and o.Amount > 0
			)
		)
	GROUP BY p.ProductId
	ORDER BY productCategoriesCount DESC
END

GO--

CREATE PROCEDURE [Settings].[sp_GetCsvProductPropertyNames] 
	@exportFeedId INT,
	@exportAllProducts BIT,
	@exportNotAvailable BIT
AS
BEGIN
	DECLARE @lcategorytemp TABLE (CategoryId INT)
	DECLARE @l TABLE (categoryid INT PRIMARY KEY CLUSTERED, Opened bit)
    
	INSERT INTO @l
		SELECT t.categoryid, t.Opened
		FROM [Settings].[exportfeedselectedcategories] AS t 
			INNER JOIN catalog.category ON t.categoryid = category.categoryid
		WHERE [exportfeedid] = @exportFeedId 

	DECLARE @l1 INT = (SELECT Min(categoryid) FROM @l)
	WHILE @l1 IS NOT NULL
	BEGIN 
		if ((Select Opened from @l where CategoryId = @l1) = 1)
			INSERT INTO @lcategorytemp SELECT @l1
		else
			INSERT INTO @lcategorytemp SELECT id FROM Settings.GetChildCategoryByParent(@l1)

		SET @l1 = (SELECT Min(categoryid) FROM @l WHERE  categoryid > @l1)
	END

	DECLARE @lcategory TABLE (categoryid INT PRIMARY KEY CLUSTERED);
	INSERT INTO @lcategory SELECT Distinct CategoryId FROM @lcategorytemp

	SELECT DISTINCT prop.Name
	FROM Catalog.Product p 
		INNER JOIN Catalog.ProductCategories pc ON pc.ProductID = p.ProductId
		INNER JOIN Catalog.ProductPropertyValue ON ProductPropertyValue.ProductId = p.ProductId
		INNER JOIN Catalog.PropertyValue propVal ON propVal.PropertyValueID = ProductPropertyValue.PropertyValueID
		INNER JOIN Catalog.Property prop ON prop.PropertyId = propVal.PropertyId
	WHERE p.ProductID IN (SELECT ProductId FROM @lcategory sel INNER JOIN Catalog.ProductCategories ON ProductCategories.CategoryID = sel.categoryID)
		AND 
		(
			@exportAllProducts = 1 
			OR (
				SELECT Count(productid)
				FROM settings.exportfeedexcludedproducts
				WHERE exportfeedexcludedproducts.productid = p.productid AND exportfeedexcludedproducts.exportfeedid = @exportFeedId
			) = 0
		) AND (
			p.Enabled = 1 OR @exportNotAvailable = 1
		) AND (
			@exportNotAvailable = 1
			OR EXISTS (
				SELECT 1
				FROM [Catalog].[Offer] o
				Where o.[ProductId] = p.[productid] AND o.Price > 0 and o.Amount > 0
			)
		)
END

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Services.Features.EFeature.CsvV2', 'Новый формат экспорта/импорта товаров')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Services.Features.EFeature.CsvV2', 'New export/import products format')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Services.Features.EFeature.CsvV2.Description', 'Добавляет новую версию экспорта/импорта товаров в CSV. Главные отличия: модификации товара в разных строках; данные модификаций, категории и свойства товара в разных колонках;')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Services.Features.EFeature.CsvV2.Description', 'Adds a new version of products export/import to CSV. The main differences: product modifications in different lines; data of modifications, categories and properties in different columns;')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.Bonus.SendSms', 'Отправить СМС')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.Bonus.SendSms', 'Send sms')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.ExportImport.EProductField.None', 'Не выбрано')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.ExportImport.EProductField.None', 'Not selected')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.ExportImport.EProductField.Code', 'Код товара')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.ExportImport.EProductField.Code', 'Product code')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.ExportImport.EProductField.Sku', 'Артикул')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.ExportImport.EProductField.Sku', 'SKU')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.ExportImport.EProductField.Name', 'Наименование')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.ExportImport.EProductField.Name', 'Name')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.ExportImport.EProductField.Price', 'Цена')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.ExportImport.EProductField.Price', 'Price')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.ExportImport.EProductField.PurchasePrice', 'Закупочная цена')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.ExportImport.EProductField.PurchasePrice', 'Purchase price')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.ExportImport.EProductField.Amount', 'Количество')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.ExportImport.EProductField.Amount', 'Amount')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.ExportImport.EProductField.Size', 'Размер')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.ExportImport.EProductField.Size', 'Size')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.ExportImport.EProductField.Color', 'Цвет')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.ExportImport.EProductField.Color', 'Color')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.ExportImport.EProductField.OfferPhotos', 'Фото модификации')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.ExportImport.EProductField.OfferPhotos', 'Offer photos')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.ExportImport.EProductField.Weight', 'Вес в кг')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.ExportImport.EProductField.Weight', 'Weight in kg')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.ExportImport.EProductField.Dimensions', 'Размеры в мм')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.ExportImport.EProductField.Dimensions', 'Dimensions in mm')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.ExportImport.EProductField.ParamSynonym', 'URL адрес')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.ExportImport.EProductField.ParamSynonym', 'URL address')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.ExportImport.EProductField.Category', 'Категория')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.ExportImport.EProductField.Category', 'Category')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.ExportImport.EProductField.Sorting', 'Сортировка')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.ExportImport.EProductField.Sorting', 'Sorting')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.ExportImport.EProductField.Enabled', 'Включен')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.ExportImport.EProductField.Enabled', 'Enabled')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.ExportImport.EProductField.Currency', 'Валюта')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.ExportImport.EProductField.Currency', 'Currency')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.ExportImport.EProductField.Photos', 'Фото товара')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.ExportImport.EProductField.Photos', 'Product photos')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.ExportImport.EProductField.Property', 'Свойство')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.ExportImport.EProductField.Property', 'Property')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.ExportImport.EProductField.Unit', 'Ед. изм.')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.ExportImport.EProductField.Unit', 'Unit')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.ExportImport.EProductField.Discount', 'Скидка в процентах')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.ExportImport.EProductField.Discount', 'Discount percentage')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.ExportImport.EProductField.DiscountAmount', 'Числовая скидка в валюте товара')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.ExportImport.EProductField.DiscountAmount', 'Numeric discount in product currency')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.ExportImport.EProductField.ShippingPrice', 'Цена доставки')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.ExportImport.EProductField.ShippingPrice', 'Shipping cost')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.ExportImport.EProductField.BriefDescription', 'Краткое описание')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.ExportImport.EProductField.BriefDescription', 'Brief description')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.ExportImport.EProductField.Description', 'Описание')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.ExportImport.EProductField.Description', 'Description')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.ExportImport.EProductField.SeoTitle', 'SEO Titile')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.ExportImport.EProductField.SeoTitle', 'Seo Title')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.ExportImport.EProductField.SeoMetaKeywords', 'SEO Meta Keywords')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.ExportImport.EProductField.SeoMetaKeywords', 'Seo Meta Keywords')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.ExportImport.EProductField.SeoMetaDescription', 'SEO Meta Description')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.ExportImport.EProductField.SeoMetaDescription', 'Seo Meta Description')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.ExportImport.EProductField.SeoH1', 'SEO H1')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.ExportImport.EProductField.SeoH1', 'Seo H1')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.ExportImport.EProductField.Related', 'Артикулы связанных товаров')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.ExportImport.EProductField.Related', 'Related products SKUs')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.ExportImport.EProductField.Alternative', 'Артикулы альтернативных товаров')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.ExportImport.EProductField.Alternative', 'Alternative products SKUs')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.ExportImport.EProductField.Videos', 'Видео')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.ExportImport.EProductField.Videos', 'Videos')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.ExportImport.EProductField.MarkerNew', 'Новинка')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.ExportImport.EProductField.MarkerNew', 'New')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.ExportImport.EProductField.MarkerBestseller', 'Хит продаж')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.ExportImport.EProductField.MarkerBestseller', 'Bestseller')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.ExportImport.EProductField.MarkerRecomended', 'Рекомендованный')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.ExportImport.EProductField.MarkerRecomended', 'Recomended')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.ExportImport.EProductField.MarkerOnSale', 'Распродажа')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.ExportImport.EProductField.MarkerOnSale', 'OnSale')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.ExportImport.EProductField.ManualRatio', 'Ручной рейтинг товара')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.ExportImport.EProductField.ManualRatio', 'Manual product rating')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.ExportImport.EProductField.Producer', 'Производитель')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.ExportImport.EProductField.Producer', 'Producer')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.ExportImport.EProductField.OrderByRequest', 'Под заказ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.ExportImport.EProductField.OrderByRequest', 'Order by request')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.ExportImport.EProductField.CustomOptions', 'Дополнительные опции')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.ExportImport.EProductField.CustomOptions', 'Custom options')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.ExportImport.EProductField.YandexSalesNotes', 'Яндекс.Маркет: Заметки для продажи')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.ExportImport.EProductField.YandexSalesNotes', 'Yandex Market: Sales notes')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.ExportImport.EProductField.YandexDeliveryDays', 'Яндекс.Маркет: Срок доставки')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.ExportImport.EProductField.YandexDeliveryDays', 'Yandex Market: Delivery time')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.ExportImport.EProductField.YandexTypePrefix', 'Яндекс.Маркет: Префикс')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.ExportImport.EProductField.YandexTypePrefix', 'Yandex Market: Prefix')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.ExportImport.EProductField.YandexName', 'Яндекс.Маркет: Название товара')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.ExportImport.EProductField.YandexName', 'Yandex Market: Product name')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.ExportImport.EProductField.YandexModel', 'Яндекс.Маркет: Модель')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.ExportImport.EProductField.YandexModel', 'Yandex Market: Model')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.ExportImport.EProductField.YandexSizeUnit', 'Яндекс.Маркет: Обозначения размерных сеток')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.ExportImport.EProductField.YandexSizeUnit', 'Yandex Market: Dimension naming conventions in the unit attribute')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.ExportImport.EProductField.YandexBid', 'Яндекс.Маркет: Ставка для карточки модели')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.ExportImport.EProductField.YandexBid', 'Yandex Market: The rate for the card model')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.ExportImport.EProductField.GoogleGtin', 'Google Merchant: GTIN')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.ExportImport.EProductField.GoogleGtin', 'Google Merchant: GTIN')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.ExportImport.EProductField.GoogleProductCategory', 'Google Merchant: Категория товара')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.ExportImport.EProductField.GoogleProductCategory', 'Google Merchant: Product сategory')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.ExportImport.EProductField.Adult', 'Товар для взрослых')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.ExportImport.EProductField.Adult', 'Adult')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.ExportImport.EProductField.ManufacturerWarranty', 'Гарантия производителя')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.ExportImport.EProductField.ManufacturerWarranty', 'Manufacturer warranty')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.ExportImport.EProductField.AvitoProductProperties', 'Авито: Свойства товара')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.ExportImport.EProductField.AvitoProductProperties', 'Avito: Product properties')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.ExportImport.EProductField.Tags', 'Теги')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.ExportImport.EProductField.Tags', 'Tags')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.ExportImport.EProductField.Gifts', 'Подарки')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.ExportImport.EProductField.Gifts', 'Gifts')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.ExportImport.EProductField.MinAmount', 'Минимальное количество')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.ExportImport.EProductField.MinAmount', 'Minimal amount')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.ExportImport.EProductField.MaxAmount', 'Максимальное количество')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.ExportImport.EProductField.MaxAmount', 'Maximal amount')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.ExportImport.EProductField.Multiplicity', 'Кратность количества')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.ExportImport.EProductField.Multiplicity', 'Multiplicity')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.ExportImport.EProductField.BarCode', 'Штрихкод')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.ExportImport.EProductField.BarCode', 'Barcode')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.ExportImport.EProductField.Tax', 'Налог')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.ExportImport.EProductField.Tax', 'Tax')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.ExportImport.EProductField.PaymentSubjectType', 'Предмет расчета')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.ExportImport.EProductField.PaymentSubjectType', 'Payment subject')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.ExportImport.EProductField.PaymentMethodType', 'Способ расчета')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.ExportImport.EProductField.PaymentMethodType', 'Payment method')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.ExportImport.EProductField.ExternalCategoryId', 'Внешние ключи категорий')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.ExportImport.EProductField.ExternalCategoryId', 'External category IDs')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Import.Errors.FileNotFound', 'Файл не найден')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Import.Errors.FileNotFound', 'File not found')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Import.Errors.WrongFile', 'Файл не содержит данных или формат не поддерживается')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Import.Errors.WrongFile', 'File contains no data or format is not supported')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Import.Errors.NoHeaders', 'Не найдены заголовки')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Import.Errors.NoHeaders', 'Headers not found')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Import.Errors.DuplicateHeaders', 'Найдены дублирующие заголовки')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Import.Errors.DuplicateHeaders', 'Duplicate headers found')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.ExportImport.ImportCsv.StartImport', 'Начало импорта')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.ExportImport.ImportCsv.StartImport', 'Start import')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.ExportImport.ImportCsv.DisablingProducts', 'Деактивация товаров')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.ExportImport.ImportCsv.DisablingProducts', 'Disabling products')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.ExportImport.ImportCsv.ImportCompleted', 'Окончание импорта')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.ExportImport.ImportCsv.ImportCompleted', 'Import completed')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.ExportImport.ImportCsvV2.WrongPropertyHeader', 'Некорректный заголовок свойства в колонке {0}')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.ExportImport.ImportCsvV2.WrongPropertyHeader', 'Invalid property header in column {0}')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.ExportImport.ImportCsvV2.NoProductCode', 'Не указан код товара')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.ExportImport.ImportCsvV2.NoProductCode', 'Product code not specified')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.ExportImport.ImportCsv.FieldNotFound', '{0} "{1}" не найден в строке {2}')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.ExportImport.ImportCsv.FieldNotFound', '{0} "{1}" not found at line {2}')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.ExportImport.ImportCsv.OfferSkuIsBusy', 'Артикул цены занят, загрузка/обновление цен товара отменена. Артикул модификации: {0}.')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.ExportImport.ImportCsv.OfferSkuIsBusy', 'Offer SKU is busy, adding/updating product offers has been canceled. Offer SKU: {0}.')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.ExportImport.ImportCsv.ProductUpdated', 'Товар обновлен: {0}')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.ExportImport.ImportCsv.ProductUpdated', 'Product updated: {0}')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.ExportImport.ImportCsv.ProductAdded', 'Товар добавлен: {0}')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.ExportImport.ImportCsv.ProductAdded', 'Product added: {0}')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.ExportImport.ImportCsv.ProductsLimitRiched', 'Превышен лимит количества товаров по тарифному плану')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.ExportImport.ImportCsv.ProductsLimitRiched', 'Tariff plan products quantity limit exceeded')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.ExportImport.ImportCsv.ProductNotAdded', 'Не удалось добавить товар: {0}')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.ExportImport.ImportCsv.ProductNotAdded', 'Product not added: {0}')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.ExportImport.ImportCsv.ProcessRowError', 'Ошибка при обработке строки {0}: {1}')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.ExportImport.ImportCsv.ProcessRowError', 'Error while processing row {0}: {1}')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.ExportImport.ImportCsv.NotAllPhotosProcessed', 'Не удалось обработать все фотографии товара: {0}')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.ExportImport.ImportCsv.NotAllPhotosProcessed', 'Not all product photos processed: {0}')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.ExportImport.ImportCsv.ModuleError', 'Не удалось обработать часть данных товара: {0}, модулем: {1}')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.ExportImport.ImportCsv.ModuleError', 'Failed to process data of product {0} by module {1}')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Catalog.Export.ProductsV2', 'Товары (новый формат)')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Catalog.Export.ProductsV2', 'Products (new format)')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ImportCategories.Errors.FieldsRequired', 'Должно быть выбрано хотя бы одно из полей: Id, Внешний ключ категории, Название')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ImportCategories.Errors.FieldsRequired', 'At least one of the fields must be selected: Id, Category external ID, Name')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ImportCategories.ProcessName', 'Загрузка категорий')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ImportCategories.ProcessName', 'Import categories')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ImportCsvV2.Errors.FieldsRequired', 'Должно быть выбрано хотя бы одно из полей: Код товара, Наименование')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ImportCsvV2.Errors.FieldsRequired', 'At least one of the fields must be selected: Product code, Name')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ImportCsvV2.ProcessName', 'Загрузка каталога товаров')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ImportCsvV2.ProcessName', 'Import catalog')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ImportCustomers.Errors.FieldsRequired', 'Должно быть выбрано поле email или customerId')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ImportCustomers.Errors.FieldsRequired', 'At least one of the fields must be selected: email or customerId')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ImportCustomers.ProcessName', 'Загрузка покупателей')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ImportCustomers.ProcessName', 'Import customers')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ImportLeads.Errors.FieldsRequired', 'Должно быть выбрано поле email, id покупателя или телефон')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ImportLeads.Errors.FieldsRequired', 'At least one of the fields must be selected: email, customer ID or phone')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ImportLeads.ProcessName', 'Загрузка лидов')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ImportLeads.ProcessName', 'Import leads')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ImportCsv.Errors.FieldsRequired', 'Должно быть выбрано хотя бы одно из полей: Артикул, Наименование')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ImportCsv.Errors.FieldsRequired', 'At least one of the fields must be selected: SKU, Name')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ImportCsv.ProcessName', 'Загрузка каталога товаров')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ImportCsv.ProcessName', 'Import catalog')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.ExportImport.ExportFeed.CsvV2Type', 'Excel-файл (csv). Новый формат')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.ExportImport.ExportFeed.CsvV2Type', 'Excel-file (csv). New format')


GO--

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Нет установленных шаблонов. Для установки перейдите в <a href="design/templateshop">Магазин шаблонов</a>' WHERE [LanguageId] = '1' AND [ResourceKey] = 'Admin.Design.Index.NoInstalledTemplates'
UPDATE [Settings].[Localization] SET [ResourceValue] = 'No templates are installed. To install, go to the <a href="design/templateshop">Template Shop</a>' WHERE [LanguageId] = '2' AND [ResourceKey] = 'Admin.Design.Index.NoInstalledTemplates'

GO--

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Например, если товар 20см на 15см и 3мм, задайте' WHERE [LanguageId] = '1' AND [ResourceKey] = 'Admin.Product.Edit.Example15x20'
UPDATE [Settings].[Localization] SET [ResourceValue] = 'For example, if the goods are 20cm by 15cm and 3mm thick, specify' WHERE [LanguageId] = '2' AND [ResourceKey] = 'Admin.Product.Edit.Example15x20'

DELETE FROM [Settings].[Localization] WHERE [ResourceKey] = 'Admin.Product.Edit.DimensionsInCentimeters'

UPDATE [Settings].[Localization] SET [ResourceValue] = 'кг' WHERE [LanguageId] = '1' AND [ResourceKey] = 'Admin.Product.Edit.Kg'
UPDATE [Settings].[Localization] SET [ResourceValue] = 'kg' WHERE [LanguageId] = '2' AND [ResourceKey] = 'Admin.Product.Edit.Kg'

DELETE FROM [Settings].[Localization] WHERE [ResourceKey] = 'Admin.Products.Edit.Grams'

GO--
UPDATE [Settings].[InternalSettings] SET [settingValue] = '8.5.0' WHERE [settingKey] = 'db_version'