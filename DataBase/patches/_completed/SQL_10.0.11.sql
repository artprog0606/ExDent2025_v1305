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
	FROM cte order by SortOrder, ColorName
	set @result = @result + ']'  
	return @result  
END 

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
		 SELECT isNull(MIN(price), 0)  
		 FROM [Catalog].offer  
		 WHERE offer.productid = @productId  
			   AND price > 0 AND (@OnlyAvailable = 0  

					OR amount > 0)  
	 );        

	 --@OfferId      
	 SET @OfferId =  
	 (  
		 SELECT OfferID  
		 FROM [Catalog].offer  
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

IF NOT EXISTS(SELECT 1
              FROM sys.columns
              WHERE name = N'CurrentVkProduct' AND object_id = OBJECT_ID(N'[Vk].[VkProduct]'))
BEGIN
	ALTER TABLE Vk.VkProduct ADD
		CurrentVkProduct nvarchar(MAX) NULL,
		ModifiedDate date NULL
END

GO--

IF NOT EXISTS(SELECT 1
              FROM [Settings].[Localization]
              WHERE [ResourceKey] = 'Core.Orders.Order.ShippingTaxName')
BEGIN
    INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue])
    VALUES
        (1,'Core.Orders.Order.ShippingTaxName', 'Налог доставки'),
        (2,'Core.Orders.Order.ShippingTaxName', 'Shipping tax name'),
        (1,'Core.Orders.Order.ShippingTaxRate', 'Ставка налога доставки'),
        (2,'Core.Orders.Order.ShippingTaxRate', 'Shipping tax rate'),
        (1,'Core.Orders.Order.ShippingTaxSum', 'Сумма налога доставки'),
        (2,'Core.Orders.Order.ShippingTaxSum', 'Shipping tax sum'),
        (1,'Core.Orders.Order.ShippingTaxSumFormatted', 'Сумма налога доставки'),
        (2,'Core.Orders.Order.ShippingTaxSumFormatted', 'Shipping tax sum')
END

GO--

UPDATE [Settings].[Localization] SET [ResourceValue] = N'Передавать налог в зависимости от способа расчета' WHERE [LanguageId] = 1 AND [ResourceKey] = 'Admin.Settings.Checkout.TaxTypeByPaymentMethodType'
UPDATE [Settings].[Localization] SET [ResourceValue] = N'Для соблюдения статьи 164 п.4 НК РФ' WHERE [LanguageId] = 1 AND [ResourceKey] = 'Admin.SettingsCheckout.CheckoutCommon.TaxTypeByPaymentMethodTypeHint'
UPDATE [Settings].[Localization] SET [ResourceValue] = N'Увеличение количества товара "{0}" после смены статуса заказа {1} на "{2}"' WHERE [LanguageId] = 1 AND [ResourceKey] = 'Core.ProductHistory.IncrementProductAmountChangedByOrderStatus'
UPDATE [Settings].[Localization] SET [ResourceValue] = N'Уменьшение количества товара "{0}" после смены статуса заказа {1} на "{2}"' WHERE [LanguageId] = 1 AND [ResourceKey] = 'Core.ProductHistory.DecrementProductAmountChangedByOrderStatus'
UPDATE [Settings].[Localization] SET [ResourceValue] = N'Телефон отправителя' WHERE [LanguageId] = 1 AND [ResourceKey] = 'GiftCertificate.Index.Phone'
UPDATE [Settings].[Localization] SET [ResourceValue] = N'Покупатели' WHERE [LanguageId] = 1 AND [ResourceKey] = 'Admin.SettingsApi.Index.Customers'
UPDATE [Settings].[Localization] SET [ResourceValue] = N'Получить покупателя' WHERE [LanguageId] = 1 AND [ResourceKey] = 'Admin.SettingsApi.Index.GetCustomer'
UPDATE [Settings].[Localization] SET [ResourceValue] = N'Создать покупателя' WHERE [LanguageId] = 1 AND [ResourceKey] = 'Admin.SettingsApi.Index.AddCustomer'
UPDATE [Settings].[Localization] SET [ResourceValue] = N'Изменить покупателя' WHERE [LanguageId] = 1 AND [ResourceKey] = 'Admin.SettingsApi.Index.UpdateCustomer'
UPDATE [Settings].[Localization] SET [ResourceValue] = N'Послать смс-код по номеру телефона' WHERE [LanguageId] = 1 AND [ResourceKey] = 'Admin.SettingsApi.Index.UpdateCustomer'

GO--

UPDATE [Settings].[Settings] SET [Value] = 'False' WHERE [Name] = 'ShowImageSearchEnabled'
UPDATE [Settings].[Settings] SET [Value] = 'False' WHERE [Name] = 'ImageSearchEnabled'

GO--

update [Settings].[Settings]
set Value = REPLACE(Value, 'f003.backblazeb2.com', 'scr.advstatic.ru')
where value like '%f003.backblazeb2.com%'

update [CMS].[LandingSite]
set [ScreenShot] = REPLACE([ScreenShot], 'f003.backblazeb2.com', 'scr.advstatic.ru')
where [ScreenShot] like '%f003.backblazeb2.com%'
  
GO--

if (SELECT DATA_TYPE FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'TelegramMessage' AND COLUMN_NAME = 'FromId') = 'int'
begin
    ALTER TABLE Customers.TelegramMessage
    ALTER COLUMN FromId bigint NOT NULL
end

GO--

if (SELECT DATA_TYPE FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'TelegramMessage' AND COLUMN_NAME = 'ToId') = 'int'
begin
    ALTER TABLE Customers.TelegramMessage
    ALTER COLUMN ToId bigint NULL
end

GO--

IF NOT EXISTS(SELECT 1
              FROM [Settings].[Localization]
              WHERE [ResourceKey] = 'Admin.Files.Index.HtmlFileMoreThanAllowed')
BEGIN
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue])
VALUES
    (1,'Admin.Files.Index.HtmlFileMoreThanAllowed', N'Файл превышает максимальный допустимый размер в {0}'),
    (2,'Admin.Files.Index.HtmlFileMoreThanAllowed', 'The file exceeds the maximum allowed size in {0}')
END

GO--


SET IDENTITY_INSERT [Order].[ShippingReplaceGeo] ON 


IF NOT EXISTS (SELECT * FROM [Order].[ShippingReplaceGeo] WHERE [Id] = 28)
INSERT INTO [Order].[ShippingReplaceGeo] ([Id],[ShippingType],[InCountryName],[InCountryISO2],[InRegionName],[InCityName],[InDistrict],[OutCountryName],[OutRegionName],[OutCityName],[OutDistrict],[OutDistrictClear],[Enabled],[Sort],[InZip],[OutZip],[Comment])
VALUES (28,'Sdek','','RU','Астраханская область','Знаменск','','','','Знаменск ЗАТО','',0,1,0,'','','')

IF NOT EXISTS (SELECT * FROM [Order].[ShippingReplaceGeo] WHERE [Id] = 29)
INSERT INTO [Order].[ShippingReplaceGeo] ([Id],[ShippingType],[InCountryName],[InCountryISO2],[InRegionName],[InCityName],[InDistrict],[OutCountryName],[OutRegionName],[OutCityName],[OutDistrict],[OutDistrictClear],[Enabled],[Sort],[InZip],[OutZip],[Comment])
VALUES (29,'Sdek','Казахстан','KZ','Жамбыльская область','','','','Жамбылская обл.','','',0,1,0,'','','')

IF NOT EXISTS (SELECT * FROM [Order].[ShippingReplaceGeo] WHERE [Id] = 30)
INSERT INTO [Order].[ShippingReplaceGeo] ([Id],[ShippingType],[InCountryName],[InCountryISO2],[InRegionName],[InCityName],[InDistrict],[OutCountryName],[OutRegionName],[OutCityName],[OutDistrict],[OutDistrictClear],[Enabled],[Sort],[InZip],[OutZip],[Comment])
VALUES (30,'Sdek','Казахстан','KZ','Южно-Казахстанская область','','','','Туркестанская обл.','','',0,1,0,'','','')

IF NOT EXISTS (SELECT * FROM [Order].[ShippingReplaceGeo] WHERE [Id] = 31)
INSERT INTO [Order].[ShippingReplaceGeo] ([Id],[ShippingType],[InCountryName],[InCountryISO2],[InRegionName],[InCityName],[InDistrict],[OutCountryName],[OutRegionName],[OutCityName],[OutDistrict],[OutDistrictClear],[Enabled],[Sort],[InZip],[OutZip],[Comment])
VALUES (31,'Sdek','Казахстан','KZ','Туркестанская область','Шымкент','','','Казахстан','','',0,1,0,'','','')



IF NOT EXISTS (SELECT * FROM [Order].[ShippingReplaceGeo] WHERE [Id] = 32)
INSERT INTO [Order].[ShippingReplaceGeo] ([Id],[ShippingType],[InCountryName],[InCountryISO2],[InRegionName],[InCityName],[InDistrict],[OutCountryName],[OutRegionName],[OutCityName],[OutDistrict],[OutDistrictClear],[Enabled],[Sort],[InZip],[OutZip],[Comment])
VALUES (32,'Sdek','Киргизия','KG','Киргизия','Бишкек','','','Чуйская обл.','','',0,1,0,'','','')


SET IDENTITY_INSERT [Order].[ShippingReplaceGeo] OFF

GO--

UPDATE [Order].[ShippingReplaceGeo] SET [Enabled] = 0 WHERE [Id] = 15
UPDATE [Order].[ShippingReplaceGeo] SET [Enabled] = 0 WHERE [Id] = 13
UPDATE [Order].[ShippingReplaceGeo] SET [Enabled] = 0 WHERE [Id] = 6

GO--




IF NOT EXISTS(SELECT 1
              FROM sys.columns
              WHERE (name = N'ObjectType' OR name = N'ObjectId')
                AND object_id = OBJECT_ID(N'[Settings].[QuartzJobRuns]'))
    BEGIN
        CREATE TABLE [Settings].[QuartzJobRuns]
        (
            Id        NVARCHAR(50)  NOT NULL
                CONSTRAINT QuartzJobRuns_pk
                    PRIMARY KEY,
            Name      NVARCHAR(100) NOT NULL,
            [Group]   NVARCHAR(100) NOT NULL,
            Initiator NVARCHAR(100),
            Status    NVARCHAR(50)  NOT NULL,
            StartDate DATETIME      NOT NULL,
            EndDate   DATETIME
        )

        CREATE UNIQUE INDEX QuartzJobRuns_Id_uindex
            ON [Settings].[QuartzJobRuns] (Id)
    END

GO--

IF NOT EXISTS(SELECT 1
              FROM sys.columns
              WHERE (name = N'ObjectType' OR name = N'ObjectId')
                AND object_id = OBJECT_ID(N'[Settings].[QuartzJobRunLogs]'))
    BEGIN
        CREATE TABLE [Settings].[QuartzJobRunLogs]
        (
            Id       INT IDENTITY
                CONSTRAINT QuartzJobRunLogs_pk
                    PRIMARY KEY,
            JobRunId NVARCHAR(50)  NOT NULL
                CONSTRAINT QuartzJobRunLogs_QuartzJobRuns_Id_fk
                    REFERENCES [Settings].[QuartzJobRuns]
                    ON DELETE CASCADE,
            Event  NVARCHAR(100) NOT NULL,
            Message  NVARCHAR(MAX),
            AddDate  DATETIME      NOT NULL
        )
        
        CREATE UNIQUE INDEX QuartzJobRunLogs_Id_uindex
            ON [Settings].[QuartzJobRunLogs] (Id)
    END

GO--


UPDATE Settings.Settings SET [Value] = REPLACE([Value], '{"JobType":"AdvantShop.Core.Scheduler.Jobs.GenerateHtmlMapJob","Enabled":true,"TimeInterval":12,"TimeHours":0,"TimeMinutes":0,"TimeType":2,"DataMap":null},{"JobType":"AdvantShop.Core.Scheduler.Jobs.GenerateXmlMapJob","Enabled":true,"TimeInterval":12,"TimeHours":0,"TimeMinutes":0,"TimeType":2,"DataMap":null}', '') 
WHERE [Name] = 'TaskSqlSettings'

GO--


INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue])
VALUES
    (1,'Admin.Js.SettingsCatalog.BrandDiscountRegulation.RegulationOfBrandDiscount', N'Регулирование скидок по производителю'),
    (2,'Admin.Js.SettingsCatalog.BrandDiscountRegulation.RegulationOfBrandDiscount', 'Brand discount regulation')

GO--

INSERT INTO [Settings].[Settings] ([Name], [Value]) VALUES ('BackupPhotosBeforeDeleting', 'True')

GO--


INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue])
VALUES
	(1,'Admin.Js.EditMainPageList.ShuffleListHint', 'Настройка "Перемешивать товары раз в день" позволяет случайным образом менять вывод товаров ежедневно.<br> Товары с сортировкой меньше 0 перемешиваться не будут. Поэтому если надо чтобы в начале были определенные товары и не менялись, то задайте им отрицательную сортировку.'),
    (2,'Admin.Js.EditMainPageList.ShuffleListHint', 'This setting allows you to randomly change goods sorting daily.<br> Products with sorting less than 0 will not be mixed.')
GO--


INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue])
VALUES
    (1,'Core.VkMarket.Export.CaptchaNeededException', 'Достигнут лимит ВКонтакте по выгрузке товаров. Остальные товары можно будет выгружать не раньше чем через час. По расписанию товары выгружаются каждые 6 часов.'),
    (2,'Core.VkMarket.Export.CaptchaNeededException', 'The VKontakte limit on export goods has been reached. The rest of the goods can be exported no earlier than in an hour. According to the schedule every 6 hours.')
    
GO--



IF NOT EXISTS(SELECT 1
              FROM sys.columns
              WHERE name = N'CurrentVkProduct' AND object_id = OBJECT_ID(N'[Vk].[VkProduct]'))
BEGIN
	ALTER TABLE Vk.VkProduct ADD
		CurrentVkProduct nvarchar(MAX) NULL,
		ModifiedDate date NULL
END

GO--


IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Shipping].[RussianPostPickPoints]') AND type in (N'U'))
BEGIN
CREATE TABLE [Shipping].[RussianPostPickPoints](
	[Id] [int] NOT NULL,
	[TypePoint] [tinyint] NOT NULL,
	[Region] [nvarchar](255) NOT NULL,
	[Area] [nvarchar](255) NULL,
	[City] [nvarchar](255) NOT NULL,
	[Address] [nvarchar](255) NULL,
	[AddressDescription] [nvarchar](max) NULL,
	[BrandName] [nvarchar](255) NULL,
	[Latitude] [float] NOT NULL,
	[Longitude] [float] NOT NULL,
	[Cash] [bit] NULL,
	[Card] [bit] NULL,
	[Type] [nvarchar](255) NULL,
	[WorkTime] [nvarchar](455) NULL,
	[WeightLimit] [float] NULL,
	[DimensionLimit] [nvarchar](50) NULL,
	[LastUpdate] [datetime] NOT NULL,
 CONSTRAINT [PK_RussianPostPickPoints] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.MobileVersion.ShowBottomPanel', 'Показывать нижнюю панель')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.MobileVersion.ShowBottomPanel', 'Show bottom panel')

GO--

if not exists (Select 1 From [Settings].[TemplateSettings] Where [Template] = '_default' and [Name] = 'Mobile_ShowBottomPanel')
    Insert Into [Settings].[TemplateSettings] ([Template],[Name],[Value]) Values ('_default', 'Mobile_ShowBottomPanel', 'True')
	
if not exists (Select 1 From [Settings].[TemplateSettings] Where [Template] = 'Modern' and [Name] = 'Mobile_ShowBottomPanel')
    Insert Into [Settings].[TemplateSettings] ([Template],[Name],[Value]) Values ('Modern', 'Mobile_ShowBottomPanel', 'True')
	
	

GO--


INSERT INTO [Settings].[TemplateSettings] ([Template], [Name], [Value]) VALUES ('_default', 'Mobile_MainPageCatalogView', '0')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Mobile.MainPageViewModeHorizontal', 'Горизонтальная прокрутка по группам')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Mobile.MainPageViewModeHorizontal', 'Horizontal scrolling by groups')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Mobile.MainPageViewModeVertical', 'Вертикальный вывод')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Mobile.MainPageViewModeVertical', 'Vertical output')

GO--



UPDATE [Settings].[InternalSettings] SET [settingValue] = '10.0.11' WHERE [settingKey] = 'db_version'