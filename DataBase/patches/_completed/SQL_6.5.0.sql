
GO--
alter table bonus.Purchase alter column OrderId int null

GO--

INSERT INTO [Settings].[Settings] ([Name],[Value]) VALUES ('StoreActive','True')
INSERT INTO [Settings].[Settings] ([Name],[Value]) VALUES ('CrmActive','True')
INSERT INTO [Settings].[Settings] ([Name],[Value]) VALUES ('TasksActive','True')
INSERT INTO [Settings].[Settings] ([Name],[Value]) VALUES ('AcademyActive','True')
INSERT INTO [Settings].[Settings] ([Name],[Value]) VALUES ('BonusAppActive','False')
--INSERT INTO [Settings].[Settings] ([Name],[Value]) VALUES ('BookingActive','True')
GO--



ALTER PROCEDURE [Catalog].[sp_UpdateOrInsertProductProperty]
@ProductID int,
@Name nvarchar(255),
@Value nvarchar(255),
@RangeValue float,
@SortOrder int
AS
BEGIN
Declare @propertyId int;
Set @propertyId = 0;
Select @propertyId = PropertyID From Catalog.Property Where Name = @Name;
if( @propertyId = 0 )
begin
Insert into Catalog.Property (Name, UseInFilter, SortOrder, Expanded, [Type], [UseInDetails], [useinbrief], NameDisplayed) Values (@Name, 0, 0, 0, 0, 1, 0, @Name)
Select @propertyId = PropertyID From Catalog.Property Where Name = @Name
end
declare @propertyValueId int;
Set @propertyValueId = 0;
Select @propertyValueId = PropertyValueID From Catalog.PropertyValue Where Value = @Value and RangeValue = @RangeValue and PropertyID = @propertyId;
if(@propertyValueId = 0)
begin
Insert into Catalog.PropertyValue (PropertyID, Value, RangeValue) Values (@propertyId, @Value, @RangeValue)
Select @propertyValueId = PropertyValueID From Catalog.PropertyValue Where PropertyID = @propertyId and Value = @Value and RangeValue = @RangeValue
end
if((Select COUNT(ProductID) From Catalog.ProductPropertyValue Where ProductID = @ProductID and PropertyValueID = @propertyValueId) = 0 )
begin
Insert into Catalog.ProductPropertyValue (ProductID, PropertyValueID) Values (@ProductID, @propertyValueId)
end
END 

GO--

CREATE TABLE [Customers].[InstagramUser](
	[CustomerId] [uniqueidentifier] NOT NULL,
	[Pk] [nvarchar](max) NOT NULL,
	[UserName] [nvarchar](max) NOT NULL,
	[FullName] [nvarchar](max) NULL,
	[Email] [nvarchar](max) NULL,
	[PhoneNumber] [nvarchar](max) NULL,
	[ProfilePicture] [nvarchar](max) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO--

ALTER TABLE [Customers].[InstagramUser]  WITH CHECK ADD  CONSTRAINT [FK_InstagramUser_Customer] FOREIGN KEY([CustomerId])
REFERENCES [Customers].[Customer] ([CustomerID])
ON DELETE CASCADE
GO--

ALTER TABLE [Customers].[InstagramUser] CHECK CONSTRAINT [FK_InstagramUser_Customer]
GO--

CREATE TABLE [Customers].[InstagramMessage](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ThreadId] [nvarchar](max) NOT NULL,
	[MediaPk] [nvarchar](max) NOT NULL,
	[InstagramId] [nvarchar](max) NOT NULL,
	[FromUserPk] [nvarchar](max) NOT NULL,
	[ToUserPk] [nvarchar](max) NOT NULL,
	[Text] [nvarchar](max) NULL,
	[CreatedDate] [datetime] NOT NULL,
	[ItemType] [int] NULL,
 CONSTRAINT [PK_InstagramMessage] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO--

ALTER TABLE Customers.InstagramMessage ADD
	Title nvarchar(MAX) NULL
GO--

IF NOT EXISTS(SELECT 1 FROM sys.columns WHERE Name = N'ManualGrade' AND Object_ID = Object_ID(N'Bonus.Card'))
BEGIN
	ALTER TABLE Bonus.Card ADD ManualGrade bit NOT NULL DEFAULT 0
END
GO--

Update CMS.StaticBlock set InnerName = 'Блок над поиcком в Header' where [Key] = 'headerСenterBlock'

GO--

ALTER PROCEDURE [Customers].[sp_GetRecentlyView]
	@CustomerId uniqueidentifier,
	@rowsCount int,
	@Type nvarchar(50)
AS
BEGIN
	SET NOCOUNT ON;
	
	IF Exists (SELECT 1 FROM [Customers].[RecentlyViewsData] WHERE CustomerID=@CustomerId)
	Begin
		SELECT TOP(@rowsCount) Product.ProductID, Product.Name, Product.UrlPath, Ratio, PhotoName,
				[Photo].[Description] as PhotoDescription, Discount, DiscountAmount, MinPrice as BasePrice, CurrencyValue
		
		FROM [Customers].RecentlyViewsData
		
		Inner Join [Catalog].Product ON Product.ProductID = RecentlyViewsData.ProductId
		Left Join [Catalog].[ProductExt] ON [Product].[ProductID] = [ProductExt].[ProductID]
		Inner Join Catalog.Currency On Currency.CurrencyID = Product.CurrencyID
		Left Join Catalog.Photo ON Photo.[ObjId] = Product.ProductID and [Type]=@Type AND [Photo].[Main]=1
		
		WHERE RecentlyViewsData.CustomerID = @CustomerId AND Product.Enabled = 1 And CategoryEnabled = 1
		
		ORDER BY ViewDate Desc
	End
END

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
		PriceTemp = (isnull(minPrice.minPrice,0) - isnull(minPrice.minPrice,0) * p.Discount/100)*c.CurrencyValue,
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
left join (select pg.ProductID, CASE WHEN COUNT(pg.ProductID) > 0 THEN 1 ELSE 0 END gifts  FROM [Catalog].[ProductGifts] pg group by pg.ProductID) gifts on pe.ProductId=gifts.ProductId
inner join catalog.Product p on p.ProductID = pe.ProductID
INNER JOIN CATALOG.Currency c ON p.currencyid = c.currencyid
 )
AS tempTable 
ON tempTable.ProductId = ProductExt.ProductId
    
END

GO--

ALTER TABLE Settings.Error404 ADD
	DateAdded datetime NULL

GO--

Update Settings.Error404 Set DateAdded = getdate()

GO--

ALTER procedure [Catalog].[sp_GetPriceRange]
(
	@categoryId int,
	@useDepth bit
)
AS
begin
if (@useDepth = 1)
	begin
		if (@categoryId=0)
			begin
				SELECT 
					min(((Catalog.Offer.Price - isnull(Product.DiscountAmount,0)) * Currency.CurrencyValue)*(1-isnull(Product.Discount,0)/100)) as minprice,
					max(((Catalog.Offer.Price - isnull(Product.DiscountAmount,0)) * Currency.CurrencyValue)*(1-isnull(Product.Discount,0)/100)) as maxprice  

				FROM Catalog.Product 
				
				INNER JOIN Catalog.Offer ON Catalog.Product.ProductID = Catalog.Offer.ProductID
				INNER JOIN Catalog.Currency ON Catalog.Currency.CurrencyID = Catalog.Product.CurrencyID
				
				WHERE Product.Enabled = 1 and product.CategoryEnabled = 1
			end

		else
			begin
				SELECT 
					min(((Catalog.Offer.Price - isnull(Product.DiscountAmount,0))* Currency.CurrencyValue)*(1-isnull(Product.Discount,0)/100)) as minprice,
					max(((Catalog.Offer.Price - isnull(Product.DiscountAmount,0))* Currency.CurrencyValue)*(1-isnull(Product.Discount,0)/100)) as maxprice  

				FROM Catalog.Product 
				
				INNER JOIN Catalog.Offer ON Catalog.Product.ProductID = Catalog.Offer.ProductID 
				INNER JOIN Catalog.ProductCategories ON ProductCategories.ProductID = [Product].[ProductID] 
				INNER JOIN Catalog.Currency ON Catalog.Currency.CurrencyID = Catalog.Product.CurrencyID

				WHERE Product.Enabled = 1 and product.CategoryEnabled = 1 and 
					  Catalog.ProductCategories.CategoryID in (Select id from [Settings].[GetChildCategoryByParent](@categoryId))
			end
	end
else
	begin
		SELECT 
				min(((Catalog.Offer.Price - isnull(Product.DiscountAmount,0))* Currency.CurrencyValue)*(1-isnull(Product.Discount,0)/100)) as minprice,
				max(((Catalog.Offer.Price - isnull(Product.DiscountAmount,0))* Currency.CurrencyValue)*(1-isnull(Product.Discount,0)/100)) as maxprice 

		FROM Catalog.Product 
		
		INNER JOIN Catalog.Offer ON Catalog.Product.ProductID = Catalog.Offer.ProductID
		INNER JOIN Catalog.ProductCategories ON ProductCategories.ProductID = [Product].[ProductID] 
		INNER JOIN Catalog.Currency ON Catalog.Currency.CurrencyID = Catalog.Product.CurrencyID
		
		WHERE Product.Enabled = 1 and product.CategoryEnabled = 1 and Catalog.ProductCategories.CategoryID = @categoryId
	end 
end


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
		PriceTemp = CASE WHEN p.Discount > 0 THEN (isnull(minPrice.minPrice,0) - isnull(minPrice.minPrice,0) * p.Discount/100)*c.CurrencyValue ELSE (isnull(minPrice.minPrice,0) - p.DiscountAmount)*c.CurrencyValue END,
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
left join (select pg.ProductID, CASE WHEN COUNT(pg.ProductID) > 0 THEN 1 ELSE 0 END gifts  FROM [Catalog].[ProductGifts] pg group by pg.ProductID) gifts on pe.ProductId=gifts.ProductId
inner join catalog.Product p on p.ProductID = pe.ProductID
INNER JOIN CATALOG.Currency c ON p.currencyid = c.currencyid
 )
AS tempTable 
ON tempTable.ProductId = ProductExt.ProductId
    
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
             SELECT CASE WHEN [Product].Discount > 0 THEN (@MinPrice - @MinPrice * [Product].Discount / 100) * CurrencyValue ELSE @MinPrice - [Product].DiscountAmount END
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
							WHEN COUNT(ProductID) > 0  
							THEN 1  
							ELSE 0  
						END  
		 FROM [Catalog].[ProductGifts]  
		 WHERE ProductID = @productId  
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

ALTER TABLE [Order].OrderItems ALTER COLUMN ArtNo nvarchar(100) NOT NULL
GO--

ALTER PROCEDURE [Order].[sp_AddOrderItem]  
	 @OrderID int,  
	 @Name nvarchar(100),  
	 @Price float,  
	 @Amount float,  
	 @ProductID int,  
	 @ArtNo nvarchar(100),  
	 @SupplyPrice float,  
	 @Weight float,  
	 @IsCouponApplied bit,  
	 @Color nvarchar(50),  
	 @Size nvarchar(50),  
	 @DecrementedAmount float,  
	 @PhotoID int,  
	 @IgnoreOrderDiscount bit,  
	 @AccrueBonuses bit,
	 @TaxId int,
	 @TaxName nvarchar(50),
	 @TaxType int,
	 @TaxRate float(53),
	 @TaxShowInPrice bit
AS  
BEGIN  
   
 INSERT INTO [Order].OrderItems  
	   ([OrderID]  
	   ,[ProductID]  
	   ,[Name]  
	   ,[Price]  
	   ,[Amount]  
	   ,[ArtNo]  
	   ,[SupplyPrice]  
	   ,[Weight]  
	   ,[IsCouponApplied]  
	   ,[Color]  
	   ,[Size]  
	   ,[DecrementedAmount]  
	   ,[PhotoID]  
	   ,[IgnoreOrderDiscount]  
	   ,[AccrueBonuses] 
	   ,TaxId
	   ,TaxName
	   ,TaxType
	   ,TaxRate
	   ,TaxShowInPrice
	   )  
 VALUES  
	   (@OrderID  
	   ,@ProductID  
	   ,@Name  
	   ,@Price  
	   ,@Amount  
	   ,@ArtNo  
	   ,@SupplyPrice  
	   ,@Weight  
	   ,@IsCouponApplied  
	   ,@Color  
	   ,@Size  
	   ,@DecrementedAmount  
	   ,@PhotoID  
	   ,@IgnoreOrderDiscount  
	   ,@AccrueBonuses
	   ,@TaxId
	   ,@TaxName
	   ,@TaxType
	   ,@TaxRate
	   ,@TaxShowInPrice   
   );  
       
 SELECT SCOPE_IDENTITY()  
END  

GO--

ALTER PROCEDURE [Order].[sp_UpdateOrderItem]  
	@OrderItemID int,  
	@OrderID int,  
	@Name nvarchar(100),  
	@Price float,  
	@Amount float,  
	@ProductID int,  
	@ArtNo nvarchar(100),  
	@SupplyPrice float,  
	@Weight float,  
	@IsCouponApplied bit,  
	@Color nvarchar(50),  
	@Size nvarchar(50),  
	@DecrementedAmount float,  
	@PhotoID int,  
	@IgnoreOrderDiscount bit,  
	@AccrueBonuses bit,
	@TaxId int,
	@TaxName nvarchar(50),
	@TaxType int,
	@TaxRate float(53),
	@TaxShowInPrice bit
AS  
BEGIN  
 Update [Order].[OrderItems]  
 Set  
     [Name] = @Name  
	,[Price] = @Price  
	,[Amount] = @Amount  
	,[ArtNo] = @ArtNo  
	,[SupplyPrice] = @SupplyPrice  
	,[Weight] = @Weight  
	,[IsCouponApplied] = @IsCouponApplied  
	,[Color] = Color  
    ,[Size] = Size  
    ,[DecrementedAmount] = DecrementedAmount  
    ,[PhotoID] = @PhotoID  
    ,[IgnoreOrderDiscount] = @IgnoreOrderDiscount  
    ,[AccrueBonuses] = @AccrueBonuses
	,TaxId = @TaxId
	,TaxName = @TaxName
	,TaxType = @TaxType
	,TaxRate = @TaxRate
	,TaxShowInPrice = @TaxShowInPrice
 Where OrderItemID = @OrderItemID  
END  
  
GO--

ALTER TABLE Customers.Customer ADD
	SortOrder int NULL DEFAULT 0
GO--

Update Customers.Customer Set SortOrder = 0 
GO--

ALTER PROCEDURE [Customers].[sp_UpdateCustomerInfo]   
 @customerid uniqueidentifier,   
 @firstname nvarchar (70),   
 @lastname nvarchar(70),   
 @patronymic nvarchar(70),   
 @phone nvarchar(max),   
 @standardphone bigint,   
 @email nvarchar(100) ,  
 @customergroupid INT = NULL,   
 @customerrole INT,   
 @bonuscardnumber bigint,   
 @admincomment nvarchar (max),   
 @managerid INT,   
 @rating INT,  
 @avatar nvarchar(100),  
 @Enabled bit,  
 @HeadCustomerId uniqueidentifier,  
 @BirthDay datetime,  
 @City nvarchar(70),
 @SortOrder int 
AS  
BEGIN  
 UPDATE [customers].[customer]  
 SET [firstname] = @firstname,  
  [lastname] = @lastname,  
  [patronymic] = @patronymic,  
  [phone] = @phone,  
  [standardphone] = @standardphone,  
  [email] = @email,  
  [customergroupid] = @customergroupid,  
  [customerrole] = @customerrole,  
  [bonuscardnumber] = @bonuscardnumber,  
  [admincomment] = @admincomment,  
  [managerid] = @managerid,  
  [rating] = @rating,  
  [avatar] = @avatar,  
  [Enabled] = @Enabled,  
  [HeadCustomerId] = @HeadCustomerId,  
  [BirthDay] = @BirthDay,  
  [City] = @City,
  [SortOrder] = @SortOrder 
 WHERE customerid = @customerid  
END

GO--
 
Alter Table Catalog.Product
Add Cpa bit NULL

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
	 @Cpa bit
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
			 @Cpa			 
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
	 @Cpa bit
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
	WHERE ProductID = @ProductID      
END 


ALTER TABLE Customers.VkMessage ADD
	PostId bigint NULL

GO--

CREATE NONCLUSTERED INDEX IX_OrderHistory ON [Order].OrderHistory
	(
	OrderId
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO--

CREATE NONCLUSTERED INDEX IX_StatusHistory ON [Order].StatusHistory
	(
	OrderID
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	
GO--

Update [Settings].[Settings] 
Set [Value] = 'dd.MM.yyyy HH:mm' 
Where [Name] = 'AdminDateFormat' and [Value] = 'dd.MM.yyyy HH:mm:ss'

GO--


GO--

-- old formats
UPDATE [Settings].[MailFormat] SET [Enable] = 0 WHERE [MailFormatTypeId] IN 
(Select [MailFormatTypeID] From [Settings].[MailFormatType] Where [MailType] IN ('OnSetManagerTask', 'OnChangeManagerTaskStatus'))
GO--
	
IF NOT EXISTS(SELECT * FROM [Settings].[MailFormatType] WHERE [MailType] = 'OnTaskAssigned')
INSERT INTO [Settings].[MailFormatType] ([TypeName], [SortOrder], [Comment], [MailType]) 
VALUES ('Сотруднику назначена задача', 235, 'Уведомление о назначенной задаче. Доступные переменные:
#TASK_ID# - номер задачи; 
#TASK_PROJECT# - проект; 
#MANAGER_NAME# - исполнитель; 
#MANAGER_LINK# - сслыка на исполнителя; 
#APPOINTEDMANAGER# - постановщик; 
#APPOINTEDMANAGER_LINK# - ссылка на постановщика; 
#TASK_NAME# - название задачи; 
#TASK_DESCRIPTION# - описание задачи; 
#TASK_STATUS# - статус задачи; 
#TASK_PRIORITY# - приоритет задачи; 
#DUEDATE# - крайний срок; 
#DATE_CREATED# - дата создания задачи; 
#TASK_URL# - ссылка на задачу; 
#TASK_ATTACHMENTS# - прикрепленные файлы', 'OnTaskAssigned')
GO--

IF NOT EXISTS(SELECT * FROM [Settings].[MailFormat] WHERE [MailFormatTypeId] = (SELECT TOP(1) [MailFormatTypeID] FROM [Settings].[MailFormatType] WHERE [MailType] = 'OnTaskAssigned'))
INSERT INTO [Settings].[MailFormat] ([FormatName],[FormatText],[SortOrder],[Enable],[AddDate],[ModifyDate],[FormatSubject],[MailFormatTypeId])
Values ('Сотруднику назначена задача', '<div style="color: #4c4f56; font-family: Arial, Helvetica, sans-serif; font-size: 14px;">
<div class="header" style="border-bottom: 1px solid #ededed; display: table; margin-bottom: 25px; padding-bottom: 25px; width: 100%;">
<div class="logo" style="display: table-cell; text-align: left; vertical-align: middle;">#LOGO#</div>
<div class="phone" style="display: table-cell; text-align: right; vertical-align: middle;">
<div class="tel" style="font-size: 26px; font-weight: bold; line-height: 1; margin-bottom: 5px;">&nbsp;</div>
<div class="inform" style="font-size: 12px;">&nbsp;</div>
</div>
</div>
<div>
<p>Вам назначена задача №#TASK_ID#.</p>
<div class="o-title vi" style="font-size: 14px; font-weight: bold; margin: 5px 0;"><a href="#TASK_URL#">#TASK_NAME#</a></div>
<a href="#TASK_URL#">#TASK_URL#</a>
<div class="l-row">
<div class="l-value vi" style="display: inline-block; margin: 5px 0;">
<table>
	<tbody>
		<tr>
			<td style="color:#acacac;padding:5px 15px 5px 0;">Проект:</td>
			<td>#TASK_PROJECT#</td>
		</tr>
		<tr>
			<td style="color:#acacac;padding:5px 15px 5px 0;">Дата создания:</td>
			<td>#DATE_CREATED#</td>
		</tr>
		<tr>
			<td style="color:#acacac;padding:5px 15px 5px 0;">Исполнитель:</td>
			<td><a href="#MANAGER_LINK#">#MANAGER_NAME#</a></td>
		</tr>
		<tr>
			<td style="color:#acacac;padding:5px 15px 5px 0;">Постановщик:</td>
			<td><a href="#APPOINTEDMANAGER_LINK#">#APPOINTEDMANAGER#</a></td>
		</tr>
		<tr>
			<td style="color:#acacac;padding:5px 15px 5px 0;">Статус:</td>
			<td>#TASK_STATUS#</td>
		</tr>
		<tr>
			<td style="color:#acacac;padding:5px 15px 5px 0;">Приоритет:</td>
			<td>#TASK_PRIORITY#</td>
		</tr>
		<tr>
			<td style="color:#acacac;padding:5px 15px 5px 0;">Крайний срок:</td>
			<td>#DUEDATE#</td>
		</tr>
		<tr>
			<td style="color:#acacac;padding:5px 15px 5px 0;">Приложения:</td>
			<td>#TASK_ATTACHMENTS#</td>
		</tr>
		<tr>
			<td colspan="2" style="padding:10px 0;">#TASK_DESCRIPTION#</td>
		</tr>
	</tbody>
</table>
</div>
</div>
</div>
</div>', 1205, 1, GETDATE(), GETDATE(), 'Вам назначена задача №#TASK_ID#. #TASK_NAME#', (SELECT TOP(1) [MailFormatTypeID] FROM [Settings].[MailFormatType] WHERE [MailType] = 'OnTaskAssigned'))
GO--


ALTER TABLE [Order].Lead ADD SortOrder int NULL
GO--
UPDATE [Order].Lead SET SortOrder = Id * 5
GO--
ALTER TABLE [Order].Lead ALTER COLUMN SortOrder int NOT NULL
GO--

ALTER TABLE [CRM].[DealStatus] ADD Color nvarchar(10) NULL
GO--

UPDATE [CRM].[DealStatus] SET Color = '8bc34a' WHERE Name = 'Новый'
UPDATE [CRM].[DealStatus] SET Color = 'ffc73e' WHERE Name = 'Созвон с клиентом'
UPDATE [CRM].[DealStatus] SET Color = '1ec5b8' WHERE Name = 'Выставление КП'
UPDATE [CRM].[DealStatus] SET Color = '78909c' WHERE Name = 'Ожидание решения клиента'
UPDATE [CRM].[DealStatus] SET Color = 'b0bec5' WHERE Name = 'Сделка отклонена'
GO--


CREATE TABLE [Customers].[FacebookUser](
	[Id] [nvarchar](255) NOT NULL,
	[CustomerId] [uniqueidentifier] NOT NULL,
	[FirstName] [nvarchar](255) NULL,
	[LastName] [nvarchar](255) NULL,
	[Gender] [nvarchar](255) NULL
) ON [PRIMARY]
GO--

ALTER TABLE [Customers].[FacebookUser]  WITH CHECK ADD  CONSTRAINT [FK_FacebookUser_Customer] FOREIGN KEY([CustomerId])
REFERENCES [Customers].[Customer] ([CustomerID])
ON DELETE CASCADE
GO--

ALTER TABLE [Customers].[FacebookUser] CHECK CONSTRAINT [FK_FacebookUser_Customer]
GO--

CREATE TABLE [Customers].[FacebookMessage](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[MessageId] [nvarchar](255) NOT NULL,
	[FromId] [nvarchar](max) NULL,
	[ToId] [nvarchar](max) NULL,
	[Message] [nvarchar](max) NULL,
	[CreatedTime] [datetime] NULL,
	[Type] [int] NOT NULL,
	[PostId] [nvarchar](max) NULL,
	[ConversationId] [nvarchar](max) NULL,
 CONSTRAINT [PK_FacebookMessage] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO--

ALTER PROCEDURE [Settings].[sp_GetExportFeedProducts] @exportFeedId int
	,@onlyCount BIT
	,@exportNotAvailable bit
	,@selectedCurrency NVARCHAR(10)
	,@allowPreOrder bit = 0
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
			,CountryName as BrandCountry
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
		LEFT JOIN [Customers].Country ON Brand.CountryID = Country.CountryID
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
	END
END

GO--

CREATE TABLE [CRM].[SalesFunnel](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](max) NOT NULL,
	[SortOrder] [int] NOT NULL,
 CONSTRAINT [PK_SalesFunnel] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO--

CREATE TABLE [CRM].[SalesFunnel_DealStatus](
	[SalesFunnelId] [int] NOT NULL,
	[DealStatusId] [int] NOT NULL,
 CONSTRAINT [PK_SalesFunnel_DealStatus] PRIMARY KEY CLUSTERED 
(
	[SalesFunnelId] ASC,
	[DealStatusId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO--

ALTER TABLE [CRM].[SalesFunnel_DealStatus]  WITH CHECK ADD  CONSTRAINT [FK_SalesFunnelDealStatus_DealStatus] FOREIGN KEY([DealStatusId])
REFERENCES [CRM].[DealStatus] ([Id])
ON DELETE CASCADE
GO--

ALTER TABLE [CRM].[SalesFunnel_DealStatus] CHECK CONSTRAINT [FK_SalesFunnelDealStatus_DealStatus]
GO--

ALTER TABLE [CRM].[SalesFunnel_DealStatus]  WITH CHECK ADD  CONSTRAINT [FK_SalesFunnelDealStatus_SalesFunnel] FOREIGN KEY([SalesFunnelId])
REFERENCES [CRM].[SalesFunnel] ([Id])
ON DELETE CASCADE
GO--

ALTER TABLE [CRM].[SalesFunnel_DealStatus] CHECK CONSTRAINT [FK_SalesFunnelDealStatus_SalesFunnel]
GO--

Insert Into [CRM].[SalesFunnel] (Name, SortOrder) Values ('Лиды', 10);
GO--

Insert Into [CRM].[SalesFunnel_DealStatus] (SalesFunnelId, DealStatusId)
Select (Select top(1) Id From [CRM].[SalesFunnel]), ds.Id From [CRM].[DealStatus] as ds
GO--

ALTER TABLE CRM.DealStatus ADD
	Status int NULL
GO--

UPDATE [CRM].[DealStatus] SET Status = 0
GO--

UPDATE [CRM].[DealStatus] SET Status = 1 WHERE Name = 'Сделка заключена'
UPDATE [CRM].[DealStatus] SET Status = 2 WHERE Name = 'Сделка отклонена'
GO--

update Shipping.SdekCities set CityName = 'Петергоф' where Id = 1210
GO--

ALTER PROCEDURE [Order].[sp_DecrementProductsCountAccordingOrder]	
	@orderId int
AS
BEGIN
  UPDATE offer
  SET offer.Amount = offer.Amount - orderItems.Amount + orderItems.DecrementedAmount
  FROM Catalog.Offer offer
  INNER JOIN [Order].[OrderItems] orderItems
    ON offer.Artno = orderItems.ArtNo
  WHERE orderItems.OrderID = @orderId;


  UPDATE [Order].[OrderItems]
  SET decrementedAmount = amount
  WHERE [OrderID] = @orderId
  UPDATE [Order].[Order]
  SET [Decremented] = 1
  WHERE [OrderID] = @orderId
	
END
GO--

ALTER TABLE [Order].Lead ADD
	SalesFunnelId int NULL
GO--

Update [Order].Lead Set SalesFunnelId = (Select top(1) Id From [CRM].[SalesFunnel]) Where SalesFunnelId is null
GO--

CREATE TABLE [Settings].[MailTemplate](
	[TemplateId] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](250) NOT NULL,
	[Body] [nvarchar](max) NOT NULL,
	[Subject] [nvarchar](250) NOT NULL,
	[SortOrder] [int] NOT NULL,
	[Active] [bit] NOT NULL,
 CONSTRAINT [PK_MailTemplate] PRIMARY KEY CLUSTERED 
(
	[TemplateId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO--

alter table Customers.Customer add Organization nvarchar(250)
alter table [Order].[OrderCustomer] add Organization nvarchar(250)
alter table [Order].Lead add Organization nvarchar(250)

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
	@Organization nvarchar(250)
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
		,[Organization])

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
		,@Organization);

	SELECT CustomerID From [Customers].[Customer] Where CustomerId = @CustomerID
END

GO--


ALTER PROCEDURE [Customers].[sp_UpdateCustomerInfo]   
 @customerid uniqueidentifier,   
 @firstname nvarchar (70),   
 @lastname nvarchar(70),   
 @patronymic nvarchar(70),   
 @phone nvarchar(max),   
 @standardphone bigint,   
 @email nvarchar(100) ,  
 @customergroupid INT = NULL,   
 @customerrole INT,   
 @bonuscardnumber bigint,   
 @admincomment nvarchar (max),   
 @managerid INT,   
 @rating INT,  
 @avatar nvarchar(100),  
 @Enabled bit,  
 @HeadCustomerId uniqueidentifier,  
 @BirthDay datetime,  
 @City nvarchar(70),
 @SortOrder int,
 @Organization nvarchar(250)
AS  
BEGIN  
 UPDATE [customers].[customer]  
 SET [firstname] = @firstname,  
  [lastname] = @lastname,  
  [patronymic] = @patronymic,  
  [phone] = @phone,  
  [standardphone] = @standardphone,  
  [email] = @email,  
  [customergroupid] = @customergroupid,  
  [customerrole] = @customerrole,  
  [bonuscardnumber] = @bonuscardnumber,  
  [admincomment] = @admincomment,  
  [managerid] = @managerid,  
  [rating] = @rating,  
  [avatar] = @avatar,  
  [Enabled] = @Enabled,  
  [HeadCustomerId] = @HeadCustomerId,  
  [BirthDay] = @BirthDay,  
  [City] = @City,
  [SortOrder] = @SortOrder,
  [Organization] = @Organization 
 WHERE customerid = @customerid  
END

GO--




ALTER TABLE Customers.Customer ADD
	ClientStatus int NULL
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
  
 SELECT CustomerID From [Customers].[Customer] Where CustomerId = @CustomerID  
END

GO--

ALTER PROCEDURE [Customers].[sp_UpdateCustomerInfo]     
 @customerid uniqueidentifier,     
 @firstname nvarchar (70),     
 @lastname nvarchar(70),     
 @patronymic nvarchar(70),     
 @phone nvarchar(max),     
 @standardphone bigint,     
 @email nvarchar(100) ,    
 @customergroupid INT = NULL,     
 @customerrole INT,     
 @bonuscardnumber bigint,     
 @admincomment nvarchar (max),     
 @managerid INT,     
 @rating INT,    
 @avatar nvarchar(100),    
 @Enabled bit,    
 @HeadCustomerId uniqueidentifier,    
 @BirthDay datetime,    
 @City nvarchar(70),  
 @SortOrder int,  
 @Organization nvarchar(250),
 @ClientStatus int  
AS    
BEGIN    
 UPDATE [customers].[customer]    
 SET [firstname] = @firstname,    
  [lastname] = @lastname,    
  [patronymic] = @patronymic,    
  [phone] = @phone,    
  [standardphone] = @standardphone,    
  [email] = @email,    
  [customergroupid] = @customergroupid,    
  [customerrole] = @customerrole,    
  [bonuscardnumber] = @bonuscardnumber,    
  [admincomment] = @admincomment,    
  [managerid] = @managerid,    
  [rating] = @rating,    
  [avatar] = @avatar,    
  [Enabled] = @Enabled,    
  [HeadCustomerId] = @HeadCustomerId,    
  [BirthDay] = @BirthDay,    
  [City] = @City,  
  [SortOrder] = @SortOrder,  
  [Organization] = @Organization,
  [ClientStatus] = @ClientStatus 
 WHERE customerid = @customerid    
END 

GO--


IF (NOT EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'Order' 
                 AND  TABLE_NAME = 'OrderAdditionalData'))
BEGIN
    CREATE TABLE [Order].[OrderAdditionalData](
	[OrderID] [int] NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[Value] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_AdditionalData] PRIMARY KEY CLUSTERED 
(
	[OrderID] ASC,
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]



ALTER TABLE [Order].[OrderAdditionalData]  WITH CHECK ADD  CONSTRAINT [FK_AdditionalData_Order] FOREIGN KEY([OrderID])
REFERENCES [Order].[Order] ([OrderID])
ON DELETE CASCADE


ALTER TABLE [Order].[OrderAdditionalData] CHECK CONSTRAINT [FK_AdditionalData_Order]


END
GO--

CREATE FUNCTION [CRM].[LeadItemsToString]
(
	@leadId int,
	@separator nvarchar(10)
)
RETURNS varchar(Max)
AS
BEGIN
	DECLARE @result varchar(max)
	SELECT @result = COALESCE(@result + @separator, '') + Name +  ISNULL(' ' + Color, '') + ISNULL(' ' + Size, '') +  ' x ' + CONVERT(varchar(50), Amount)
	FROM [Order].LeadItem WHERE LeadId = @leadId

	RETURN @result
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
	 @DateModified datetime

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
	 @Fee float,  
	 @AccrueBonuses bit,
     @Taxid int, 
	 @YandexSizeUnit nvarchar(10),
	 @Cpa bit,
	 @DateModified datetime
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
			 @Cpa			 
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


IF (SELECT COUNT(*) FROM [CRM].[SalesFunnel]) = 1
	UPDATE [CRM].[SalesFunnel] SET Name = 'Лиды'
GO--


ALTER TABLE [Order].LeadEvent ADD
	CreatedById uniqueidentifier NULL
GO--


ALTER TABLE Customers.Task ALTER COLUMN SortOrder int NULL
GO--
CREATE TRIGGER [Customers].[TaskAdded]
	ON [Customers].[Task]
	AFTER INSERT
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @Status int = (SELECT [Status] FROM Inserted)
	DECLARE @TaskId int = (SELECT Id FROM Inserted)
	IF (SELECT IsDeferred FROM Inserted) = 0
	BEGIN
		UPDATE Customers.Task 
		SET SortOrder = (SELECT ISNULL(MAX(SortOrder), 0) + 10 FROM Customers.Task WHERE [Status] = @Status)
		WHERE Id = @TaskId
	END
END
GO--

CREATE TRIGGER [Customers].[TaskUpdated] 
	ON [Customers].[Task]
	WITH EXECUTE AS CALLER 
	FOR UPDATE
AS
BEGIN
	SET NOCOUNT ON;
	
	if (SELECT COUNT(*) FROM Inserted) > 1
		return;

	DECLARE @NewSort int
    DECLARE @TaskId int = (SELECT Id FROM Inserted)
	DECLARE @Status int = (SELECT [Status] FROM Inserted)
    DECLARE @IsDeferred bit = (SELECT IsDeferred FROM Inserted)
    DECLARE @IsDeferredOld bit = (SELECT IsDeferred FROM Deleted)
    DECLARE @Accepted bit = (SELECT Accepted FROM Inserted)
    DECLARE @Priority int = (SELECT [Priority] FROM Inserted)
    DECLARE @PriorityOld int = (SELECT [Priority] FROM Deleted)

	-- only if task not accepted and not deffered
	IF @Accepted = 0 AND @IsDeferred = 0
	BEGIN
		-- new task from deferred tasks
		IF @IsDeferred <> @IsDeferredOld
		BEGIN
			SELECT @NewSort = (SELECT ISNULL(MAX(SortOrder), 0) + 10 FROM Customers.Task 
			WHERE [Status] = @Status AND ((@Priority <> 2 AND [Priority] <> 2) OR (@Priority = 2 AND [Priority] = 2)) AND Accepted = 0 AND IsDeferred = 0)
		END
		-- changed priority
		ELSE IF (@Priority = 2 OR @PriorityOld = 2) AND @Priority <> @PriorityOld
		BEGIN
			SELECT @NewSort = 
				(SELECT case when @Priority <> 2
					then ISNULL(MIN(SortOrder), 0) - 10			-- priority changed to high - set task first
					else ISNULL(MAX(SortOrder), 0) + 10 end 	-- priority changed to not high - set task last
				FROM Customers.Task WHERE [Status] = @Status AND ((@Priority <> 2 AND [Priority] <> 2) OR (@Priority = 2 AND [Priority] = 2)) AND Accepted = 0 AND IsDeferred = 0)
		END
	END

	IF @NewSort is not null
		UPDATE Customers.Task SET SortOrder = @NewSort WHERE Id = @TaskId
END
GO--

CREATE PROCEDURE CRM.ChangeTaskSorting
	@Id int,
	@prevId int,
	@nextId int
AS
BEGIN
	IF @prevId IS NULL AND @nextId IS NULL
		RETURN;

	DECLARE @NewSort int

	DECLARE @priority int = (SELECT [Priority] FROM Customers.Task WHERE Id = @Id)
	DECLARE @status int = (SELECT [Status] FROM Customers.Task WHERE Id = @Id)

	DECLARE @prevSort int = (SELECT SortOrder FROM Customers.Task WHERE Id = @prevId)
	DECLARE @nextSort int = (SELECT SortOrder FROM Customers.Task WHERE Id = @nextId)

	if @prevSort IS NULL OR @nextSort IS NULL
	BEGIN
		SELECT @NewSort = 
			(SELECT CASE WHEN @prevSort IS NULL 
				THEN ISNULL(MIN(SortOrder), 0) - 10 
				ELSE ISNULL(MAX(SortOrder), 0) + 10 END 
			FROM Customers.Task 
			WHERE Id <> @Id AND [Status] = @status AND ((@priority <> 2 AND [Priority] <> 2) OR (@priority = 2 AND [Priority] = 2)) AND Accepted = 0 AND IsDeferred = 0)
	END
	ELSE
	BEGIN
		if @nextSort - @prevSort > 1
		BEGIN
			SELECT @NewSort = (@prevSort + ((@nextSort - @prevSort) / 2))
		END
		ELSE
		BEGIN
			UPDATE Customers.Task SET SortOrder = TaskSort.Sort * 10
			FROM ( 
				SELECT Id, ROW_NUMBER() OVER (ORDER BY SortOrder) AS Sort FROM Customers.Task 
				WHERE [Status] = @status AND ((@priority <> 2 AND [Priority] <> 2) OR (@priority = 2 OR [Priority] = 2)) AND Accepted = 0 AND IsDeferred = 0
			) TaskSort INNER JOIN Customers.Task ON TaskSort.Id = Task.Id

			SELECT @prevSort = (SELECT SortOrder FROM Customers.Task WHERE Id = @prevId)
			SELECT @nextSort = (SELECT SortOrder FROM Customers.Task WHERE Id = @nextId)

			SELECT @NewSort = (@prevSort + ((@nextSort - @prevSort) / 2))
		END
	END

	if @NewSort IS NOT NULL
		UPDATE Customers.Task SET SortOrder = @NewSort WHERE Id = @Id
END
GO--

CREATE PROCEDURE CRM.ChangeLeadSorting
	@Id int,
	@prevId int,
	@nextId int
AS
BEGIN
	IF @prevId IS NULL AND @nextId IS NULL
		RETURN;

	DECLARE @NewSort int

	DECLARE @DealStatusId int = (SELECT DealStatusId FROM [Order].Lead WHERE Id = @Id)

	DECLARE @prevSort int = (SELECT SortOrder FROM [Order].Lead WHERE Id = @prevId)
	DECLARE @nextSort int = (SELECT SortOrder FROM [Order].Lead WHERE Id = @nextId)

	if @prevSort IS NULL OR @nextSort IS NULL
	BEGIN
		SELECT @NewSort = 
			(SELECT CASE WHEN @prevSort IS NULL 
				THEN ISNULL(MIN(SortOrder), 0) - 10 
				ELSE ISNULL(MAX(SortOrder), 0) + 10 END 
			FROM [Order].Lead 
			WHERE Id <> @Id AND DealStatusId = @DealStatusId)
	END
	ELSE
	BEGIN
		if @nextSort - @prevSort > 1
		BEGIN
			SELECT @NewSort = (@prevSort + ((@nextSort - @prevSort) / 2))
		END
		ELSE
		BEGIN
			UPDATE [Order].Lead SET SortOrder = LeadSort.Sort * 10
			FROM ( 
				SELECT Id, ROW_NUMBER() OVER (ORDER BY SortOrder) AS Sort FROM [Order].Lead
				WHERE DealStatusId = @DealStatusId
			) LeadSort INNER JOIN [Order].Lead ON LeadSort.Id = Lead.Id

			SELECT @prevSort = (SELECT SortOrder FROM [Order].Lead WHERE Id = @prevId)
			SELECT @nextSort = (SELECT SortOrder FROM [Order].Lead WHERE Id = @nextId)

			SELECT @NewSort = (@prevSort + ((@nextSort - @prevSort) / 2))
		END
	END

	if @NewSort IS NOT NULL
		UPDATE [Order].Lead SET SortOrder = @NewSort WHERE Id = @Id
END
GO--


If (NOT EXISTS(Select * From [CMS].[StaticBlock] Where [Key] = 'mainpage_before_carousel'))
Begin
	Insert Into [CMS].[StaticBlock] ([Key],[InnerName],[Content],[Added],[Modified],[Enabled]) Values ('mainpage_before_carousel', 'Блок перед каруселью', '', GETDATE(), GETDATE(), 1)
End

GO--

CREATE TABLE [Customers].[TaskGroupManager](
	[TaskGroupId] [int] NOT NULL,
	[ManagerId] [int] NOT NULL,
 CONSTRAINT [PK_TaskGroupManager] PRIMARY KEY CLUSTERED 
(
	[TaskGroupId] ASC,
	[ManagerId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO--
ALTER TABLE [Customers].[TaskGroupManager]  WITH CHECK ADD  CONSTRAINT [FK_TaskGroupManager_Managers] FOREIGN KEY([ManagerId])
REFERENCES [Customers].[Managers] ([ManagerId])
ON DELETE CASCADE
GO--
ALTER TABLE [Customers].[TaskGroupManager]  WITH CHECK ADD  CONSTRAINT [FK_TaskGroupManager_TaskGroup] FOREIGN KEY([TaskGroupId])
REFERENCES [Customers].[TaskGroup] ([Id])
ON DELETE CASCADE
GO--

ALTER TABLE Customers.Task ADD BindedTaskId int null, BindedObjectStatus int null
GO--

CREATE TRIGGER Customers.TaskDeleted ON Customers.Task
WITH EXECUTE AS CALLER
FOR DELETE
AS
BEGIN
	SET NOCOUNT ON;
	UPDATE Customers.Task SET BindedTaskId = null WHERE BindedTaskId in (SELECT Id FROM Deleted)
END
GO--

UPDATE [Settings].[MailFormatType] SET [Comment] = 'Уведомление об изменении задачи. Доступные переменные: 
#CHANGES_TABLE# - таблица с изменениями задачи;
#MODIFIER# - менеджер, внесший изменения в задачу; 
#TASK_ID# - номер задачи; 
#TASK_PROJECT# - проект; 
#MANAGER_NAME# - исполнитель; 
#APPOINTEDMANAGER# - постановщик; 
#TASK_NAME# - название задачи; 
#TASK_DESCRIPTION# - описание задачи; 
#TASK_STATUS# - статус задачи; 
#TASK_PRIORITY# - приоритет задачи; 
#DUEDATE# - крайний срок; 
#DATE_CREATED# - дата создания задачи; 
#TASK_URL# - ссылка на задачу; 
#TASK_ATTACHMENTS# - прикрепленные файлы' WHERE [MailType] = 'OnTaskChanged'
GO--
UPDATE [Settings].[MailFormatType] SET [Comment] = 'Уведомление о новой задаче. Доступные переменные:
#TASK_ID# - номер задачи; 
#TASK_PROJECT# - проект; 
#MANAGER_NAME# - исполнитель; 
#APPOINTEDMANAGER# - постановщик; 
#TASK_NAME# - название задачи; 
#TASK_DESCRIPTION# - описание задачи; 
#TASK_STATUS# - статус задачи; 
#TASK_PRIORITY# - приоритет задачи; 
#DUEDATE# - крайний срок; 
#DATE_CREATED# - дата создания задачи; 
#TASK_URL# - ссылка на задачу; 
#TASK_ATTACHMENTS# - прикрепленные файлы' WHERE [MailType] = 'OnTaskCreated'
GO--
UPDATE [Settings].[MailFormatType] SET [Comment] = 'Уведомление о назначенной задаче. Доступные переменные:
#TASK_ID# - номер задачи; 
#TASK_PROJECT# - проект; 
#MANAGER_NAME# - исполнитель; 
#APPOINTEDMANAGER# - постановщик; 
#TASK_NAME# - название задачи; 
#TASK_DESCRIPTION# - описание задачи; 
#TASK_STATUS# - статус задачи; 
#TASK_PRIORITY# - приоритет задачи; 
#DUEDATE# - крайний срок; 
#DATE_CREATED# - дата создания задачи; 
#TASK_URL# - ссылка на задачу; 
#TASK_ATTACHMENTS# - прикрепленные файлы' WHERE [MailType] = 'OnTaskAssigned'
GO--
UPDATE [Settings].[MailFormatType] SET [Comment] = 'Уведомление об удалении задачи. Доступные переменные:
#MODIFIER# - менеджер, удаливший задачу; 
#TASK_ID# - номер задачи; 
#TASK_PROJECT# - проект; 
#MANAGER_NAME# - исполнитель; 
#APPOINTEDMANAGER# - постановщик; 
#TASK_NAME# - название задачи; 
#TASK_DESCRIPTION# - описание задачи; 
#TASK_STATUS# - статус задачи; 
#TASK_PRIORITY# - приоритет задачи; 
#DUEDATE# - крайний срок; 
#DATE_CREATED# - дата создания задачи; 
#TASK_ATTACHMENTS# - прикрепленные файлы' WHERE [MailType] = 'OnTaskDeleted'
GO--
UPDATE [Settings].[MailFormatType] SET [Comment] = 'Уведомление о новом комментарии к задаче. Доступные переменные:
#AUTHOR# - автор комментария; 
#COMMENT# - текст комментария; 
#TASK_ID# - номер задачи; 
#TASK_PROJECT# - проект; 
#MANAGER_NAME# - исполнитель; 
#APPOINTEDMANAGER# - постановщик; 
#TASK_NAME# - название задачи; 
#TASK_DESCRIPTION# - описание задачи; 
#TASK_STATUS# - статус задачи; 
#TASK_PRIORITY# - приоритет задачи; 
#DUEDATE# - крайний срок; 
#DATE_CREATED# - дата создания задачи; 
#TASK_URL# - ссылка на задачу; 
#TASK_ATTACHMENTS# - прикрепленные файлы' WHERE [MailType] = 'OnTaskCommentAdded'
GO--
UPDATE [Settings].[MailFormatType] SET [Comment] = 'Уведомление менеджера заказа о новом комментарии (#AUTHOR#, #COMMENT#, #ORDER_NUMBER#, #ORDER_LINK#)' WHERE [MailType] = 'OnOrderCommentAdded'
GO--
UPDATE [Settings].[MailFormatType] SET [Comment] = 'Уведомление менеджера покупателя о новом комментарии (#AUTHOR#, #COMMENT#, #CUSTOMER#, #CUSTOMER_LINK#)' WHERE [MailType] = 'OnCustomerCommentAdded'
GO--

UPDATE [Settings].[MailFormat] SET 
[FormatSubject] = 'Задача #TASK_PROJECT#-#TASK_ID# обновлена. #TASK_NAME#',
[FormatText] = '<div style="color: #4c4f56; font-family: Arial, Helvetica, sans-serif; font-size: 14px;">
<div class="header" style="border-bottom: 1px solid #ededed; display: table; margin-bottom: 25px; padding-bottom: 25px; width: 100%;">
<div class="logo" style="display: table-cell; text-align: left; vertical-align: middle;">#LOGO#</div>
<div class="phone" style="display: table-cell; text-align: right; vertical-align: middle;">
<div class="tel" style="font-size: 26px; font-weight: bold; line-height: 1; margin-bottom: 5px;">&nbsp;</div>
<div class="inform" style="font-size: 12px;">&nbsp;</div>
</div>
</div>
<div>
<p>#MODIFIER# обновил(-а) задачу #TASK_PROJECT#-#TASK_ID#.</p>
<div class="o-title vi" style="font-size: 14px; font-weight: bold; margin: 5px 0;"><a href="#TASK_URL#">#TASK_NAME#</a></div>
<div class="l-row">
<div class="l-value vi" style="display: inline-block; margin: 5px 0;">#CHANGES_TABLE#</div>
</div>
</div>
</div>
' WHERE [MailFormatTypeId] = (SELECT TOP(1) [MailFormatTypeID] FROM [Settings].[MailFormatType] WHERE [MailType] = 'OnTaskChanged')
GO--

UPDATE [Settings].[MailFormat] SET 
[FormatSubject] = 'Новая задача #TASK_PROJECT#-#TASK_ID#. #TASK_NAME#',
[FormatText] = '<div style="color: #4c4f56; font-family: Arial, Helvetica, sans-serif; font-size: 14px;">
<div class="header" style="border-bottom: 1px solid #ededed; display: table; margin-bottom: 25px; padding-bottom: 25px; width: 100%;">
<div class="logo" style="display: table-cell; text-align: left; vertical-align: middle;">#LOGO#</div>
<div class="phone" style="display: table-cell; text-align: right; vertical-align: middle;">
<div class="tel" style="font-size: 26px; font-weight: bold; line-height: 1; margin-bottom: 5px;">&nbsp;</div>
<div class="inform" style="font-size: 12px;">&nbsp;</div>
</div>
</div>
<div>
<p>Новая задача #TASK_PROJECT#-#TASK_ID#.</p>
<div class="o-title vi" style="font-size: 14px; font-weight: bold; margin: 5px 0;"><a href="#TASK_URL#">#TASK_NAME#</a></div>
<a href="#TASK_URL#">#TASK_URL#</a>
<div class="l-row">
<div class="l-value vi" style="display: inline-block; margin: 5px 0;">
<table>
	<tbody>
		<tr>
			<td style="color:#acacac;padding:5px 15px 5px 0;">Проект:</td>
			<td>#TASK_PROJECT#</td>
		</tr>
		<tr>
			<td style="color:#acacac;padding:5px 15px 5px 0;">Дата создания:</td>
			<td>#DATE_CREATED#</td>
		</tr>
		<tr>
			<td style="color:#acacac;padding:5px 15px 5px 0;">Исполнитель:</td>
			<td>#MANAGER_NAME#</td>
		</tr>
		<tr>
			<td style="color:#acacac;padding:5px 15px 5px 0;">Постановщик:</td>
			<td>#APPOINTEDMANAGER#</td>
		</tr>
		<tr>
			<td style="color:#acacac;padding:5px 15px 5px 0;">Статус:</td>
			<td>#TASK_STATUS#</td>
		</tr>
		<tr>
			<td style="color:#acacac;padding:5px 15px 5px 0;">Приоритет:</td>
			<td>#TASK_PRIORITY#</td>
		</tr>
		<tr>
			<td style="color:#acacac;padding:5px 15px 5px 0;">Крайний срок:</td>
			<td>#DUEDATE#</td>
		</tr>
		<tr>
			<td style="color:#acacac;padding:5px 15px 5px 0;">Приложения:</td>
			<td>#TASK_ATTACHMENTS#</td>
		</tr>
		<tr>
			<td colspan="2" style="padding:10px 0;">#TASK_DESCRIPTION#</td>
		</tr>
	</tbody>
</table>
</div>
</div>
</div>
</div>' WHERE [MailFormatTypeId] = (SELECT TOP(1) [MailFormatTypeID] FROM [Settings].[MailFormatType] WHERE [MailType] = 'OnTaskCreated')
GO--

UPDATE [Settings].[MailFormat] SET 
[FormatSubject] = 'Вам назначена задача #TASK_PROJECT#-#TASK_ID#. #TASK_NAME#',
[FormatText] = '<div style="color: #4c4f56; font-family: Arial, Helvetica, sans-serif; font-size: 14px;">
<div class="header" style="border-bottom: 1px solid #ededed; display: table; margin-bottom: 25px; padding-bottom: 25px; width: 100%;">
<div class="logo" style="display: table-cell; text-align: left; vertical-align: middle;">#LOGO#</div>
<div class="phone" style="display: table-cell; text-align: right; vertical-align: middle;">
<div class="tel" style="font-size: 26px; font-weight: bold; line-height: 1; margin-bottom: 5px;">&nbsp;</div>
<div class="inform" style="font-size: 12px;">&nbsp;</div>
</div>
</div>
<div>
<p>Вам назначена задача #TASK_PROJECT#-#TASK_ID#.</p>
<div class="o-title vi" style="font-size: 14px; font-weight: bold; margin: 5px 0;"><a href="#TASK_URL#">#TASK_NAME#</a></div>
<a href="#TASK_URL#">#TASK_URL#</a>
<div class="l-row">
<div class="l-value vi" style="display: inline-block; margin: 5px 0;">
<table>
	<tbody>
		<tr>
			<td style="color:#acacac;padding:5px 15px 5px 0;">Проект:</td>
			<td>#TASK_PROJECT#</td>
		</tr>
		<tr>
			<td style="color:#acacac;padding:5px 15px 5px 0;">Дата создания:</td>
			<td>#DATE_CREATED#</td>
		</tr>
		<tr>
			<td style="color:#acacac;padding:5px 15px 5px 0;">Исполнитель:</td>
			<td>#MANAGER_NAME#</td>
		</tr>
		<tr>
			<td style="color:#acacac;padding:5px 15px 5px 0;">Постановщик:</td>
			<td>#APPOINTEDMANAGER#</td>
		</tr>
		<tr>
			<td style="color:#acacac;padding:5px 15px 5px 0;">Статус:</td>
			<td>#TASK_STATUS#</td>
		</tr>
		<tr>
			<td style="color:#acacac;padding:5px 15px 5px 0;">Приоритет:</td>
			<td>#TASK_PRIORITY#</td>
		</tr>
		<tr>
			<td style="color:#acacac;padding:5px 15px 5px 0;">Крайний срок:</td>
			<td>#DUEDATE#</td>
		</tr>
		<tr>
			<td style="color:#acacac;padding:5px 15px 5px 0;">Приложения:</td>
			<td>#TASK_ATTACHMENTS#</td>
		</tr>
		<tr>
			<td colspan="2" style="padding:10px 0;">#TASK_DESCRIPTION#</td>
		</tr>
	</tbody>
</table>
</div>
</div>
</div>
</div>' WHERE [MailFormatTypeId] = (SELECT TOP(1) [MailFormatTypeID] FROM [Settings].[MailFormatType] WHERE [MailType] = 'OnTaskAssigned')
GO--

UPDATE [Settings].[MailFormat] SET 
[FormatSubject] = 'Задача #TASK_PROJECT#-#TASK_ID# удалена. #TASK_NAME#',
[FormatText] = '<div style="color: #4c4f56; font-family: Arial, Helvetica, sans-serif; font-size: 14px;">
<div class="header" style="border-bottom: 1px solid #ededed; display: table; margin-bottom: 25px; padding-bottom: 25px; width: 100%;">
<div class="logo" style="display: table-cell; text-align: left; vertical-align: middle;">#LOGO#</div>
<div class="phone" style="display: table-cell; text-align: right; vertical-align: middle;">
<div class="tel" style="font-size: 26px; font-weight: bold; line-height: 1; margin-bottom: 5px;">&nbsp;</div>
<div class="inform" style="font-size: 12px;">&nbsp;</div>
</div>
</div>
<div>
<p>#MODIFIER# удалил(-а) задачу #TASK_PROJECT#-#TASK_ID#.</p>
<div class="o-title vi" style="font-size: 14px; font-weight: bold; margin: 5px 0;">#TASK_NAME#</div>
<div class="l-row">
<div class="l-value vi" style="display: inline-block; margin: 5px 0;">
<table>
	<tbody>
		<tr>
			<td style="color:#acacac;padding:5px 15px 5px 0;">Проект:</td>
			<td>#TASK_PROJECT#</td>
		</tr>
		<tr>
			<td style="color:#acacac;padding:5px 15px 5px 0;">Дата создания:</td>
			<td>#DATE_CREATED#</td>
		</tr>
		<tr>
			<td style="color:#acacac;padding:5px 15px 5px 0;">Исполнитель:</td>
			<td>#MANAGER_NAME#</td>
		</tr>
		<tr>
			<td style="color:#acacac;padding:5px 15px 5px 0;">Постановщик:</td>
			<td>#APPOINTEDMANAGER#</td>
		</tr>
		<tr>
			<td style="color:#acacac;padding:5px 15px 5px 0;">Статус:</td>
			<td>#TASK_STATUS#</td>
		</tr>
		<tr>
			<td style="color:#acacac;padding:5px 15px 5px 0;">Приоритет:</td>
			<td>#TASK_PRIORITY#</td>
		</tr>
		<tr>
			<td style="color:#acacac;padding:5px 15px 5px 0;">Крайний срок:</td>
			<td>#DUEDATE#</td>
		</tr>
		<tr>
			<td style="color:#acacac;padding:5px 15px 5px 0;">Приложения:</td>
			<td>#TASK_ATTACHMENTS#</td>
		</tr>
		<tr>
			<td colspan="2" style="padding:10px 0;">#TASK_DESCRIPTION#</td>
		</tr>
	</tbody>
</table>
</div>
</div>
</div>
</div>' WHERE [MailFormatTypeId] = (SELECT TOP(1) [MailFormatTypeID] FROM [Settings].[MailFormatType] WHERE [MailType] = 'OnTaskDeleted')
GO--

UPDATE [Settings].[MailFormat] SET 
[FormatSubject] = 'Новый комментарий к задаче #TASK_PROJECT#-#TASK_ID#. #TASK_NAME#',
[FormatText] = '<div style="color: #4c4f56; font-family: Arial, Helvetica, sans-serif; font-size: 14px;">
<div class="header" style="border-bottom: 1px solid #ededed; display: table; margin-bottom: 25px; padding-bottom: 25px; width: 100%;">
<div class="logo" style="display: table-cell; text-align: left; vertical-align: middle;">#LOGO#</div>
<div class="phone" style="display: table-cell; text-align: right; vertical-align: middle;">
<div class="tel" style="font-size: 26px; font-weight: bold; line-height: 1; margin-bottom: 5px;">&nbsp;</div>
<div class="inform" style="font-size: 12px;">&nbsp;</div>
</div>
</div>
<div>
<p>#AUTHOR# добавил(-а) комментарий к задаче #TASK_PROJECT#-#TASK_ID#.</p>
<div class="o-title vi" style="font-size: 14px; font-weight: bold; margin: 5px 0;"><a href="#TASK_URL#">#TASK_NAME#</a></div>
<div class="l-row">
<div class="l-value vi" style="display: inline-block; margin: 5px 0;">#COMMENT#</div>
</div>
</div>
</div>' WHERE [MailFormatTypeId] = (SELECT TOP(1) [MailFormatTypeID] FROM [Settings].[MailFormatType] WHERE [MailType] = 'OnTaskCommentAdded')
GO--

UPDATE [Settings].[MailFormat] SET [FormatText] = '<div style="color: #4c4f56; font-family: Arial, Helvetica, sans-serif; font-size: 14px;">
<div class="header" style="border-bottom: 1px solid #ededed; display: table; margin-bottom: 25px; padding-bottom: 25px; width: 100%;">
<div class="logo" style="display: table-cell; text-align: left; vertical-align: middle;">#LOGO#</div>
<div class="phone" style="display: table-cell; text-align: right; vertical-align: middle;">
<div class="tel" style="font-size: 26px; font-weight: bold; line-height: 1; margin-bottom: 5px;">&nbsp;</div>
<div class="inform" style="font-size: 12px;">&nbsp;</div>
</div>
</div>
<div>
<p>#AUTHOR# добавил(-а) комментарий к <a href="#ORDER_LINK#">заказу №#ORDER_NUMBER#</a>.</p>
<div class="l-row">
<div class="l-value vi" style="display: inline-block; margin: 5px 0;">#COMMENT#</div>
</div>
</div>
</div>' WHERE [MailFormatTypeId] = (SELECT TOP(1) [MailFormatTypeID] FROM [Settings].[MailFormatType] WHERE [MailType] = 'OnOrderCommentAdded')
GO--

UPDATE [Settings].[MailFormat] SET [FormatText] = '<div style="color: #4c4f56; font-family: Arial, Helvetica, sans-serif; font-size: 14px;">
<div class="header" style="border-bottom: 1px solid #ededed; display: table; margin-bottom: 25px; padding-bottom: 25px; width: 100%;">
<div class="logo" style="display: table-cell; text-align: left; vertical-align: middle;">#LOGO#</div>
<div class="phone" style="display: table-cell; text-align: right; vertical-align: middle;">
<div class="tel" style="font-size: 26px; font-weight: bold; line-height: 1; margin-bottom: 5px;">&nbsp;</div>
<div class="inform" style="font-size: 12px;">&nbsp;</div>
</div>
</div>
<div>
<p>#AUTHOR# добавил(-а) комментарий к покупателю <a href="#CUSTOMER_LINK#">#CUSTOMER#</a>.</p>
<div class="l-row">
<div class="l-value vi" style="display: inline-block; margin: 5px 0;">#COMMENT#</div>
</div>
</div>
</div>' WHERE [MailFormatTypeId] = (SELECT TOP(1) [MailFormatTypeID] FROM [Settings].[MailFormatType] WHERE [MailType] = 'OnCustomerCommentAdded')
GO--


CREATE TABLE [Customers].[AdminInformer](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Type] [int] NOT NULL,
	[ObjId] [int] NOT NULL,
	[CustomerId] [uniqueidentifier] NULL,
	[Title] [nvarchar](max) NULL,
	[Body] [nvarchar](max) NULL,
	[Link] [nvarchar](max) NULL,
	[Count] [int] NOT NULL,
	[Seen] [bit] NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[PrivateCustomerId] [uniqueidentifier] NULL,
	[EntityId] [int] NULL,
 CONSTRAINT [PK_AdminInformer] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO--


ALTER TABLE Customers.FacebookUser ADD
	PsyId nvarchar(MAX) NULL
GO--

ALTER PROCEDURE [Order].[sp_GetChangeOrderStatus]
	@OrderStatusID int,
	@OrderID int
AS
BEGIN
	UPDATE [Order].[Order]
	SET [OrderStatusID] = @OrderStatusID, [ModifiedDate] = Getdate()
	WHERE     ([Order].[Order].OrderID = @OrderID)
	select [CommandID] from [Order].[OrderStatus] where [OrderStatusID] = @OrderStatusID
END
GO--


CREATE TABLE [CMS].[LandingForm](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[LpId] [int] NOT NULL,
	[BlockId] [int] NULL,
	[Title] [nvarchar](max) NULL,
	[ButtonText] [nvarchar](max) NULL,
	[FieldsJson] [nvarchar](max) NULL,
	[PostAction] [int] NOT NULL,
	[PostMessageText] [nvarchar](max) NULL,
	[PostMessageRedirectUrl] [nvarchar](300) NULL,
	[YaMetrikaEventName] [nvarchar](50) NULL,
	[GaEventCategory] [nvarchar](50) NULL,
	[GaEventAction] [nvarchar](50) NULL,
	[ShowAgreement] [bit] NULL,
	[AgreementText] [nvarchar](max) NULL,
	[PayProductOfferId] [int] NULL,
 CONSTRAINT [PK_LandingForm] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO--

ALTER TABLE [CMS].[LandingForm]  WITH CHECK ADD  CONSTRAINT [FK_LandingForm_Landing] FOREIGN KEY([LpId])
REFERENCES [CMS].[Landing] ([Id])
ON DELETE CASCADE
GO--

CREATE TRIGGER [CMS].[LandingBlockDeleted] ON [CMS].[LandingBlock]
WITH EXECUTE AS CALLER
FOR DELETE
AS
BEGIN
	SET NOCOUNT ON;
	DELETE FROM [CMS].[LandingForm] WHERE [BlockId] in (select Id FROM Deleted)		
END

GO--




ALTER TABLE Customers.TaskGroup ADD
	Enabled bit NOT NULL CONSTRAINT DF_TaskGroup_Enabled DEFAULT 1
	
GO--

Update Customers.TaskGroup set Enabled = 1

GO--


 UPDATE [CMS].[StaticPage]
 SET [PageText] = REPLACE([PageText],'col-xs-4','col-xs-12 col-sm-4') 
 WHERE [StaticPageID] = 132
 
GO--
 
 ALTER PROCEDURE [Catalog].[SetCategoryHierarchicallyEnabled] @CatParent INT
AS
     BEGIN
         DECLARE @tbl TABLE
         (Child          INT,
          Parent         INT,
          [Level]        INT,
          [Enabled]      BIT,
          HirecalEnabled BIT
         );
         WITH cteSort
              AS (
              SELECT [Category].CategoryID AS Child,
                     [Category].ParentCategory AS Parent,
                     1 AS [Level],
                     [Category].Enabled,
                     [Category].HirecalEnabled
              FROM [Catalog].[Category]
              WHERE CategoryID = @CatParent
              UNION ALL
              SELECT [Category].CategoryID AS Child,
                     [Category].ParentCategory AS Parent,
                     cteSort.[Level] + 1 AS [Level],
                     [Category].Enabled,
                     cteSort.Enabled&cteSort.HirecalEnabled AS HirecalEnabled
              FROM [Catalog].[Category]
                   INNER JOIN cteSort ON [Category].ParentCategory = cteSort.Child
                                         AND [Category].CategoryID <> 0)
              INSERT INTO @tbl
                     SELECT Child,
                            Parent,
                            [Level],
                            [Enabled],
                            HirecalEnabled
                     FROM cteSort;
         UPDATE [Catalog].[Category]
           SET
               HirecalEnabled = temp.HirecalEnabled
         FROM [Catalog].[Category] c
              INNER JOIN @tbl temp ON c.CategoryID = temp.Child;
         UPDATE [Catalog].[Product]
           SET
               CategoryEnabled = c.Enabled&c.HirecalEnabled
         FROM [Catalog].[Product] p
              INNER JOIN [Catalog].ProductCategories pc ON p.ProductID = pc.ProductID
                                                           AND Main = 1
              INNER JOIN [Catalog].[Category] c ON pc.CategoryID = c.CategoryID
              INNER JOIN @tbl temp ON c.CategoryID = temp.Child;
         UPDATE [Catalog].[Product]
           SET
               CategoryEnabled = 0
         FROM [Catalog].[Product] p
              LEFT JOIN [Catalog].ProductCategories pc ON p.ProductID = pc.ProductID
         WHERE pc.CategoryID IS NULL;
     END;

GO--




Update Settings.Settings set Value = 'yandexmap' where Name = 'PrintOrder_MapType'
GO--

IF NOT EXISTS(SELECT 1 FROM sys.columns WHERE Name = N'YandexName' AND Object_ID = Object_ID(N'Catalog.Product'))
Begin
	alter table Catalog.Product add YandexName nvarchar(255) null
End
GO--

ALTER PROCEDURE [Settings].[sp_GetExportFeedProducts] @exportFeedId int
	,@onlyCount BIT
	,@exportNotAvailable bit
	,@selectedCurrency NVARCHAR(10)
	,@allowPreOrder bit = 0
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
			,CountryName as BrandCountry
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
		LEFT JOIN [Customers].Country ON Brand.CountryID = Country.CountryID
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
	END
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
	 @Fee float,  
	 @AccrueBonuses bit,
     @Taxid int, 
	 @YandexSizeUnit nvarchar(10),
	 @Cpa bit,
	 @DateModified datetime,
	 @YandexName nvarchar(255)
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
			 @YandexName			 
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
	 @YandexName nvarchar(255)

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
	WHERE ProductID = @ProductID      
END 
GO--
alter table [Order].orderitems alter column name nvarchar(255)

GO--

alter table [Order].orderitems alter column ArtNo nvarchar(100) not null
GO--
alter table [Order].orderitems alter column Color nvarchar(300) null
GO--
alter table [Order].orderitems alter column Size nvarchar(300) null
GO--
ALTER PROCEDURE [Order].[sp_AddOrderItem]  
	 @OrderID int,  
	 @Name nvarchar(255),  
	 @Price float,  
	 @Amount float,  
	 @ProductID int,  
	 @ArtNo nvarchar(100),  
	 @SupplyPrice float,  
	 @Weight float,  
	 @IsCouponApplied bit,  
	 @Color nvarchar(50),  
	 @Size nvarchar(50),  
	 @DecrementedAmount float,  
	 @PhotoID int,  
	 @IgnoreOrderDiscount bit,  
	 @AccrueBonuses bit,
	 @TaxId int,
	 @TaxName nvarchar(50),
	 @TaxType int,
	 @TaxRate float(53),
	 @TaxShowInPrice bit,
	 @Width float,
	 @Height float,
	 @Length float
AS  
BEGIN  
   
 INSERT INTO [Order].OrderItems  
	   ([OrderID]  
	   ,[ProductID]  
	   ,[Name]  
	   ,[Price]  
	   ,[Amount]  
	   ,[ArtNo]  
	   ,[SupplyPrice]  
	   ,[Weight]  
	   ,[IsCouponApplied]  
	   ,[Color]  
	   ,[Size]  
	   ,[DecrementedAmount]  
	   ,[PhotoID]  
	   ,[IgnoreOrderDiscount]  
	   ,[AccrueBonuses] 
	   ,TaxId
	   ,TaxName
	   ,TaxType
	   ,TaxRate
	   ,TaxShowInPrice
	   ,Width
	   ,Height
	   ,[Length]
	   )  
 VALUES  
	   (@OrderID  
	   ,@ProductID  
	   ,@Name  
	   ,@Price  
	   ,@Amount  
	   ,@ArtNo  
	   ,@SupplyPrice  
	   ,@Weight  
	   ,@IsCouponApplied  
	   ,@Color  
	   ,@Size  
	   ,@DecrementedAmount  
	   ,@PhotoID  
	   ,@IgnoreOrderDiscount  
	   ,@AccrueBonuses
	   ,@TaxId
	   ,@TaxName
	   ,@TaxType
	   ,@TaxRate
	   ,@TaxShowInPrice   
	   ,@Width
	   ,@Height
	   ,@Length 
   );  
       
 SELECT SCOPE_IDENTITY()  
END  

GO--

ALTER PROCEDURE [Order].[sp_UpdateOrderItem]  
	@OrderItemID int,  
	@OrderID int,  
	@Name nvarchar(255),  
	@Price float,  
	@Amount float,  
	@ProductID int,  
	@ArtNo nvarchar(100),  
	@SupplyPrice float,  
	@Weight float,  
	@IsCouponApplied bit,  
	@Color nvarchar(50),  
	@Size nvarchar(50),  
	@DecrementedAmount float,  
	@PhotoID int,  
	@IgnoreOrderDiscount bit,  
	@AccrueBonuses bit,
	@TaxId int,
	@TaxName nvarchar(50),
	@TaxType int,
	@TaxRate float(53),
	@TaxShowInPrice bit,
	@Width float,
	@Height float,
	@Length float
AS  
BEGIN  
 Update [Order].[OrderItems]  
 Set  
     [Name] = @Name  
	,[Price] = @Price  
	,[Amount] = @Amount  
	,[ArtNo] = @ArtNo  
	,[SupplyPrice] = @SupplyPrice  
	,[Weight] = @Weight  
	,[IsCouponApplied] = @IsCouponApplied  
	,[Color] = Color  
    ,[Size] = Size  
    ,[DecrementedAmount] = DecrementedAmount  
    ,[PhotoID] = @PhotoID  
    ,[IgnoreOrderDiscount] = @IgnoreOrderDiscount  
    ,[AccrueBonuses] = @AccrueBonuses
	,TaxId = @TaxId
	,TaxName = @TaxName
	,TaxType = @TaxType
	,TaxRate = @TaxRate
	,TaxShowInPrice = @TaxShowInPrice
	,Width = @Width
	,Height = @Height
	,[Length] = @Length
 Where OrderItemID = @OrderItemID  
END  

GO--

INSERT INTO [Settings].[InternalSettings] ([settingKey],[settingValue]) VALUES ('GeoIPServiceUrl', 'https://geo.advsrvone.pw:8787/')
INSERT INTO [Settings].[InternalSettings] ([settingKey],[settingValue]) VALUES ('EmailServiceUrl', 'https://eml.advsrvone.pw:9898/')
INSERT INTO [Settings].[InternalSettings] ([settingKey],[settingValue]) VALUES ('ImageServiceUrl', 'https://img.advsrvone.pw:6565/')
GO--

IF NOT EXISTS(SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'Shipping' AND  TABLE_NAME = 'DDeliveryOrders')
Begin

	CREATE TABLE [Shipping].[DDeliveryOrders](
		[OrderId] [int] NOT NULL,
		[DDeliveryOrder] [nvarchar](100) NOT NULL
	) ON [PRIMARY]

	ALTER TABLE [Shipping].[DDeliveryOrders]  WITH CHECK ADD  CONSTRAINT [FK_DDeliveryOrders_Order] FOREIGN KEY([OrderId])
	REFERENCES [Order].[Order] ([OrderID])
End
GO--

Update [CMS].[StaticBlock] Set Content = REPLACE(Content, '2017', '2018') Where [Key] = 'LeftBottom'

GO--

CREATE TABLE [CMS].[ChangeHistory](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ObjId] [int] NOT NULL,
	[ObjType] [int] NOT NULL,
	[ParameterName] [nvarchar](255) NOT NULL,
	[OldValue] [nvarchar](max) NOT NULL,
	[NewValue] [nvarchar](max) NOT NULL,
	[ParameterType] [int] NOT NULL,
	[ParameterId] [int] NULL,
	[ChangedByName] [nvarchar](255) NOT NULL,
	[ChangedById] [uniqueidentifier] NULL,
	[ModificationTime] [datetime] NOT NULL,
 CONSTRAINT [PK_ChangeHistory] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO--

CREATE TABLE [Customers].[TaskGroupManagerRole](
	[TaskGroupId] [int] NOT NULL,
	[ManagerRoleId] [int] NOT NULL,
 CONSTRAINT [PK_TaskGroupManagerRole] PRIMARY KEY CLUSTERED 
(
	[TaskGroupId] ASC,
	[ManagerRoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO--

ALTER TABLE [Customers].[TaskGroupManagerRole]  WITH CHECK ADD  CONSTRAINT [FK_TaskGroupManagerRole_ManagerRole] FOREIGN KEY([ManagerRoleId])
REFERENCES [Customers].[ManagerRole] ([Id])
ON DELETE CASCADE
GO--

ALTER TABLE [Customers].[TaskGroupManagerRole]  WITH CHECK ADD  CONSTRAINT [FK_TaskGroupManagerRole_TaskGroup] FOREIGN KEY([TaskGroupId])
REFERENCES [Customers].[TaskGroup] ([Id])
ON DELETE CASCADE
GO--

CREATE TABLE [Customers].[TaskManager](
	[TaskId] [int] NOT NULL,
	[ManagerId] [int] NOT NULL,
 CONSTRAINT [PK_TaskManager] PRIMARY KEY CLUSTERED 
(
	[TaskId] ASC,
	[ManagerId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO--

ALTER TABLE [Customers].[TaskManager]  WITH CHECK ADD  CONSTRAINT [FK_TaskManager_Managers] FOREIGN KEY([ManagerId])
REFERENCES [Customers].[Managers] ([ManagerId])
ON DELETE CASCADE
GO--

ALTER TABLE [Customers].[TaskManager]  WITH CHECK ADD  CONSTRAINT [FK_TaskManager_Task] FOREIGN KEY([TaskId])
REFERENCES [Customers].[Task] ([Id])
ON DELETE CASCADE
GO--

INSERT INTO Customers.TaskManager SELECT Id, AssignedManagerId FROM Customers.Task WHERE AssignedManagerId IS NOT NULL
GO--

ALTER TABLE [Customers].[Task] DROP CONSTRAINT [FK_TaskAssignedManagerId_Managers]
GO--

ALTER TABLE Customers.Task DROP COLUMN AssignedManagerId
GO--

CREATE FUNCTION [CRM].[GetTaskManagersJson]
(
	@TaskId int
)
RETURNS varchar(Max)
AS
BEGIN
	DECLARE @result varchar(max)
	
	SELECT @result = 
		COALESCE(@result + ',', '') + 
		'{"CustomerId":' + '"' + CONVERT(varchar(50), c.CustomerId) + '",' + 
		'"FullName":' + '"' + ISNULL(c.FirstName, '') + ' ' + ISNULL(c.LastName, '') + '",' +
		'"Avatar":' + '"' + ISNULL(c.Avatar, '') + '"}'
	FROM Customers.Customer c
		INNER JOIN Customers.Managers m ON m.CustomerId = c.CustomerId
		INNER JOIN Customers.TaskManager tm ON tm.ManagerId = m.ManagerId AND tm.TaskId = @TaskId
	select @result = '[' + @result + ']'

	RETURN @result
END
GO--

if not exists(select * from [Settings].[Settings] where Name ='EmailSettingUseAdvantshopMail')
begin
	insert into [Settings].[Settings] (Name,Value) values ('EmailSettingUseAdvantshopMail','False')
end

GO--

Insert into Settings.Settings ([Name], [Value]) Values ('TextInsteadOfPrice', 'Авторизуйтесь для просмотра цен')
GO--
GO--

if ((Select COUNT(*) from [Settings].[Settings] Where Name = 'ShowImageSearchEnabled') = 0)
	Insert Into [Settings].[Settings] (Name, Value) Values ('ShowImageSearchEnabled', 'True')
else
	Update [Settings].[Settings] Set Value = 'True' Where Name = 'ShowImageSearchEnabled'

	
	
GO--
CREATE TABLE [Settings].[ExportFeedExcludedProducts](
	[ExportFeedId] [int] NOT NULL,
	[ProductId] [int] NOT NULL,
 CONSTRAINT [PK_ExportFeedExcludedProducts] PRIMARY KEY CLUSTERED 
(
	[ExportFeedId] ASC,
	[ProductId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO--
ALTER TABLE [Settings].[ExportFeedExcludedProducts]  WITH CHECK ADD  CONSTRAINT [FK_ExportFeedExcludedProducts_ExportFeed] FOREIGN KEY([ExportFeedId])
REFERENCES [Settings].[ExportFeed] ([Id])
ON DELETE CASCADE
GO--
ALTER TABLE [Settings].[ExportFeedExcludedProducts] CHECK CONSTRAINT [FK_ExportFeedExcludedProducts_ExportFeed]
GO--
ALTER TABLE [Settings].[ExportFeedExcludedProducts]  WITH CHECK ADD  CONSTRAINT [FK_ExportFeedExcludedProducts_Product] FOREIGN KEY([ProductId])
REFERENCES [Catalog].[Product] ([ProductId])
ON DELETE CASCADE
GO--
ALTER TABLE [Settings].[ExportFeedExcludedProducts] CHECK CONSTRAINT [FK_ExportFeedExcludedProducts_Product]
GO--

ALTER TABLE CMS.LandingForm ADD
	SalesFunnelId int NULL

GO--


ALTER PROCEDURE [Settings].[sp_GetExportFeedProducts] @exportFeedId int
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
			,CountryName as BrandCountry
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
		LEFT JOIN [Customers].Country ON Brand.CountryID = Country.CountryID
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

ALTER PROCEDURE [Settings].[sp_GetCsvProducts]     
   @exportFeedId int    
  ,@onlyCount BIT    
  ,@exportNoInCategory BIT 
  ,@exportAllProducts BIT
  --,@exportNotActive BIT  
  --,@exportNotAmount BIT  
AS    
BEGIN    
 DECLARE @res TABLE (productId INT PRIMARY KEY CLUSTERED);    
 DECLARE @lproduct TABLE (productId INT PRIMARY KEY CLUSTERED);    
 DECLARE @lproductNoCat TABLE (productId INT PRIMARY KEY CLUSTERED);    
    
 INSERT INTO @lproduct    
  SELECT [ProductID]    
  FROM [Settings].[ExportFeedSelectedProducts]    
  WHERE [ExportFeedId] = @exportFeedId;    
    
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
  WHERE [ExportFeedId] = @exportFeedId    
    
 DECLARE @l1 INT    
    
 SET @l1 = (SELECT MIN(CategoryId) FROM @l);    
    
 WHILE @l1 IS NOT NULL    
 BEGIN  
     
  INSERT INTO @lcategory    
   SELECT id    
   FROM Settings.GetChildCategoryByParent(@l1) AS dt    
   INNER JOIN CATALOG.Category ON CategoryId = id    
   WHERE dt.id NOT IN (SELECT CategoryId FROM @lcategory)  
    
  SET @l1 = (SELECT MIN(CategoryId) FROM @l  WHERE CategoryId > @l1);    
 END;    
    
 IF @onlyCount = 1    
 BEGIN    
  SELECT COUNT(ProductID)    
  FROM [Catalog].[Product]    
  WHERE   
  (  
 EXISTS (    
  SELECT 1    
  FROM [Catalog].[ProductCategories]    
  WHERE [ProductCategories].[ProductID] = [Product].[ProductID]    
   AND ([ProductCategories].[ProductID] IN (SELECT productId FROM @lproduct)    
    OR [ProductCategories].CategoryId IN (SELECT CategoryId FROM @lcategory))    
 )    
    OR EXISTS (    
  SELECT 1    
  FROM @lproductNoCat AS TEMP    
  WHERE TEMP.productId = [Product].[ProductID]    
 )  
   )     
   AND (@exportAllProducts=1 or (Select Count(ProductId) From Settings.ExportFeedExcludedProducts Where ExportFeedExcludedProducts.ProductId=Product.ProductId AND ExportFeedExcludedProducts.ExportFeedId=@exportFeedId) = 0)
 END    
 ELSE    
 BEGIN    
  SELECT *    
  FROM [Catalog].[Product]    
  LEFT JOIN [Catalog].[Photo] ON [Photo].[ObjId] = [Product].[ProductID] AND Type = 'Product' AND Photo.[Main] = 1    
  WHERE   
  (  
 EXISTS (    
	  SELECT 1    
	  FROM [Catalog].[ProductCategories]    
	  WHERE [ProductCategories].[ProductID] = [Product].[ProductID]    
	   AND ([ProductCategories].[ProductID] IN (SELECT productId FROM @lproduct)    
		OR [ProductCategories].CategoryId IN (SELECT CategoryId FROM @lcategory))        )    
    OR EXISTS (    
  SELECT 1    
  FROM @lproductNoCat AS TEMP    
  WHERE TEMP.productId = [Product].[ProductID]    
    )  
   )       
   AND (@exportAllProducts=1 OR (Select Count(ProductId) From Settings.ExportFeedExcludedProducts Where ExportFeedExcludedProducts.ProductId=Product.ProductId AND ExportFeedExcludedProducts.ExportFeedId=@exportFeedId) = 0)
 END    
END 


GO--
Alter table [Order].[LEAD] add Title nvarchar(MAX)
GO--
Update [Order].[LEAD] set Title = Description

GO--

if ((EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'Module.BuyInTime') AND type in (N'U'))) and ((SELECT count(Id) From Module.BuyInTime Where Id = 2) = 1))
Begin
	Update Module.BuyInTime 
	Set ActionText = Replace(ActionText, 'р.', '#CurrencyCode#'), MobileActionText = Replace(MobileActionText, 'р.', '#CurrencyCode#') 
	Where Id = 2
	
	Update Module.BuyInTime 
	Set ActionText = Replace(ActionText, '<span class="price-currency">', ' <span class="price-currency">'), MobileActionText = Replace(MobileActionText, '<span class="price-currency">', ' <span class="price-currency">') 
	Where Id = 2
	
	Update Module.BuyInTime 
	Set ActionText = Replace(ActionText, 'Экономия:#DiscountPrice# ', 'Экономия: #DiscountPrice# '), MobileActionText = Replace(MobileActionText, 'Экономия:#DiscountPrice# ', 'Экономия: #DiscountPrice# ') 
	Where Id = 2
End

GO--

Update [Settings].[ModuleSettings] 
Set [Value] = '<div class="buy-in-time-inner"> 
   <h3 class="buy-in-time-header">#ActionTitle#</h3>
   <div class="buy-in-time-content">
	   <div class="buy-in-time-countdown-block"> 
		   <div class="buy-in-time-text">До конца распродажи:</div>
		   #Countdown#
	   </div>
	   <figure class="buy-in-time-picture-block">
		   <a href="#ProductLink#"><img alt="#ProductName#" class="buy-in-time-picture" src="#ProductPictureSrc#" /></a>
		   <div class="buy-in-time-discount sticker-main">
			   <div class="buy-in-time-discount-number">#DiscountPercent#%</div>
			   <div class="buy-in-time-discount-text">скидка</div>
		   </div>
	   </figure>
	   <div class="buy-in-time-price-block">
		   <div class="buy-in-time-name"><a class="buy-in-time-name-link" href="#ProductLink#">#ProductName#</a></div>
		   <div class="buy-in-time-price-default">Цена: <span class="price"><span class="price-current"><span class="price-number">#OldPrice#</span> <span class="price-currency">#CurrencyCode#</span></span></span></div>
		   <div class="buy-in-time-price-today">Цена: <span class="price"><span class="price-current"><span class="price-number">#NewPrice#</span> <span class="price-currency">#CurrencyCode#</span></span></span></div>
		   <div class="buy-in-time-button-block"><a class="btn btn-small btn-action btn-buy-in-time" href="#ProductLink#">Экономия: #DiscountPrice# #CurrencyCode#</a></div>
	   </div>
   </div>
</div>'
Where Name = 'BuyInTimeDefaultActionText' and ModuleName = 'BuyInTime'

GO--

ALTER TABLE CMS.LandingForm ADD
	IsHidden bit NULL
GO--

Update CMS.LandingForm Set IsHidden=1

GO--

CREATE TABLE CMS.LandingSite
	(
	Id int NOT NULL IDENTITY (1, 1),
	Name nvarchar(260) NOT NULL,
	Enabled bit NOT NULL,
	Template nvarchar(MAX) NOT NULL,
	Url nvarchar(MAX) NOT NULL,
	DomainUrl nvarchar(MAX) NOT NULL,
	CreatedDate datetime NOT NULL
	)  ON [PRIMARY]
	 TEXTIMAGE_ON [PRIMARY]
GO--
ALTER TABLE CMS.LandingSite ADD CONSTRAINT
	PK_LandingSite PRIMARY KEY CLUSTERED 
	(
	Id
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO--


ALTER TABLE CMS.Landing ADD
	LandingSiteId int NULL,
	IsMain bit NULL
GO--

Update CMS.Landing Set IsMain = 1
GO--

ALTER TABLE CMS.Landing ALTER COLUMN IsMain bit NOT NULL
GO--

ALTER TABLE CMS.Landing ALTER COLUMN LandingSiteId int NOT NULL
GO--

CREATE TABLE CMS.LandingSiteSettings
	(
	Id int NOT NULL IDENTITY (1, 1),
	LandingSiteId int NOT NULL,
	Name nvarchar(255) NOT NULL,
	Value nvarchar(MAX) NOT NULL
	)  ON [PRIMARY]
	 TEXTIMAGE_ON [PRIMARY]
GO--
ALTER TABLE CMS.LandingSiteSettings ADD CONSTRAINT
	PK_LandingSiteSettings PRIMARY KEY CLUSTERED 
	(
	Id
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO--
ALTER TABLE CMS.LandingSiteSettings ADD CONSTRAINT
	FK_LandingSiteSettings_LandingSite FOREIGN KEY
	(
	LandingSiteId
	) REFERENCES CMS.LandingSite
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  CASCADE 
	
GO--



if not exists(select * from [Settings].[Settings] where Name = 'BirthDayFieldName')
begin
	insert into [Settings].[Settings] (Name,Value) values ('BirthDayFieldName', 'День рождения')
end

GO--

ALTER TABLE CRM.SalesFunnel ADD
	FinalSuccessAction int NULL
GO--

Update CRM.SalesFunnel Set FinalSuccessAction = 0
GO--

ALTER TABLE CRM.SalesFunnel ALTER COLUMN FinalSuccessAction int NOT NULL
GO--


CREATE TABLE CRM.SalesFunnel_Manager
	(
	SalesFunnelId int NOT NULL,
	ManagerId int NOT NULL
	)  ON [PRIMARY]
GO--
ALTER TABLE CRM.SalesFunnel_Manager ADD CONSTRAINT
	PK_SalesFunnel_Manager PRIMARY KEY CLUSTERED 
	(
	SalesFunnelId,
	ManagerId
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO--
ALTER TABLE CRM.SalesFunnel_Manager ADD CONSTRAINT
	FK_SalesFunnel_Manager_SalesFunnel FOREIGN KEY
	(
	SalesFunnelId
	) REFERENCES CRM.SalesFunnel
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  CASCADE 
	
GO--
ALTER TABLE CRM.SalesFunnel_Manager ADD CONSTRAINT
	FK_SalesFunnel_Manager_Managers FOREIGN KEY
	(
	ManagerId
	) REFERENCES Customers.Managers
	(
	ManagerId
	) ON UPDATE  NO ACTION 
	 ON DELETE  CASCADE 
	
GO--

IF NOT EXISTS(SELECT 1 FROM sys.columns WHERE Name = N'ModifiedDate' AND Object_ID = Object_ID(N'[Order].Lead'))
BEGIN
	ALTER TABLE [Order].Lead ADD
		ModifiedDate datetime NULL
	
	EXEC('Update [Order].Lead Set ModifiedDate = CreatedDate') 
END

GO--

CREATE TRIGGER [Catalog].[ProductListDeleted] ON [Catalog].[ProductList]
WITH EXECUTE AS CALLER
FOR DELETE
AS
BEGIN
	SET NOCOUNT ON;
	DELETE FROM [SEO].[MetaInfo] WHERE [ObjId] in (select Id FROM Deleted) and Type='MainPageProducts'
END

GO--


INSERT INTO [Settings].[Settings] ([Name], [Value]) VALUES ('MainPageProductsTitle', '#STORE_NAME# - #PAGE_NAME#')
INSERT INTO [Settings].[Settings] ([Name], [Value]) VALUES ('MainPageProductsMetaKeywords', '#STORE_NAME# - #PAGE_NAME#')
INSERT INTO [Settings].[Settings] ([Name], [Value]) VALUES ('MainPageProductsMetaDescription', '#STORE_NAME# - #PAGE_NAME#')
INSERT INTO [Settings].[Settings] ([Name], [Value]) VALUES ('MainPageProductsH1', '#PAGE_NAME#')

INSERT INTO [SEO].[MetaInfo] ([Title],[MetaKeywords],[MetaDescription],H1,[ObjId],[Type]) VALUES ('#STORE_NAME# - Хиты','#STORE_NAME# - Хиты','#STORE_NAME# - Хиты','#PAGE_NAME#',-1,'MainPageProducts')
INSERT INTO [SEO].[MetaInfo] ([Title],[MetaKeywords],[MetaDescription],H1,[ObjId],[Type]) VALUES ('#STORE_NAME# - Новинки','#STORE_NAME# - Новинки','#STORE_NAME# - Новинки','#PAGE_NAME#',-2,'MainPageProducts')
INSERT INTO [SEO].[MetaInfo] ([Title],[MetaKeywords],[MetaDescription],H1,[ObjId],[Type]) VALUES ('#STORE_NAME# - Скидка','#STORE_NAME# - Скидка','#STORE_NAME# - Скидка','#PAGE_NAME#',-3,'MainPageProducts')

GO--

ALTER FUNCTION [CRM].[GetTaskManagersJson]
(
	@TaskId int
)
RETURNS varchar(Max)
AS
BEGIN
	DECLARE @result varchar(max)
	
	SELECT @result = 
		COALESCE(@result + ',', '') + '{' +
		'"FullName":' + '"' + ISNULL(c.FirstName, '') + ' ' + ISNULL(c.LastName, '') + '",' +
		'"CustomerId":' + '"' + CONVERT(varchar(50), c.CustomerId) + '",' + 
		'"Avatar":' + '"' + ISNULL(c.Avatar, '') + '"}'
	FROM Customers.Customer c
		INNER JOIN Customers.Managers m ON m.CustomerId = c.CustomerId
		INNER JOIN Customers.TaskManager tm ON tm.ManagerId = m.ManagerId AND tm.TaskId = @TaskId
	select @result = '[' + @result + ']'

	RETURN @result
END

GO--

ALTER TABLE CMS.LandingForm ADD
	SubTitle [nvarchar](max) NULL
GO--
ALTER TABLE Catalog.Category ADD
	ExternalId nvarchar(50) NULL
GO--
ALTER PROCEDURE [Catalog].[sp_GetPropertyInFilter] (@categoryid INT, @usedepth bit)
AS
BEGIN
  IF (@usedepth = 1)
  BEGIN
      ;WITH ppvid (propertyvalueid)
           AS
           (
		   select DISTINCT ppv.propertyvalueid from [catalog].product p 
						   inner join [catalog].[productpropertyvalue] ppv on p.productid = ppv.productid  AND p.[enabled] = 1
						   inner join [catalog].[productcategories] pc on p.productid=pc.productid
						   inner join [settings].[getchildcategorybyparent] (@categoryid) ch on pc.[categoryid] = ch.id			   
           )
		   
		SELECT     pv.[propertyvalueid],
				   pv.[propertyid],
				   pv.[value],
				   pv.[rangevalue],
				   p.name                 AS propertyname,
				   p.namedisplayed        AS propertyNameDisplayed,
				   p.sortorder            AS propertysortorder,
				   p.expanded             AS propertyexpanded,
				   p.unit                 AS propertyunit,
				   p.TYPE                 AS propertytype,
				   p.Description		  AS propertydescription,
				   p.NameDisplayed		  AS PropertyNameDisplayed
				   
		FROM       [catalog].[propertyvalue] pv
		INNER JOIN [catalog].[property] p ON p.propertyid = pv.propertyid AND p.[useinfilter] = 1	
		inner join ppvid on ppvid.propertyvalueid = pv.[propertyvalueid]
					   
		WHERE      pv.[useinfilter] = 1				   
		ORDER BY   propertysortorder,
				   pv.[propertyid],
				   pv.[sortorder],
				   pv.[rangevalue],
				   pv.[value]
  END 
  ELSE 
  BEGIN 
  
	;WITH ppvid (propertyvalueid)
		 AS
		 (
		 select DISTINCT ppv.propertyvalueid from [catalog].product p 
						   inner join [catalog].[productpropertyvalue] ppv on p.productid = ppv.productid  AND p.[enabled] = 1
						   inner join [catalog].[productcategories] pc on p.productid=pc.productid and pc.[categoryid]=@categoryid			
		 )
		 
	SELECT     pv.[propertyvalueid],
			   pv.[propertyid],
			   pv.[value],
			   pv.[rangevalue],
			   p.[name]               AS propertyname,
			   p.namedisplayed        AS propertyNameDisplayed,
			   p.sortorder            AS propertysortorder,
			   p.expanded             AS propertyexpanded,
			   p.unit                 AS propertyunit,
			   p.[TYPE]               AS propertytype,
			   p.[Description]        AS propertydescription,
			   p.NameDisplayed		  AS PropertyNameDisplayed
			   
	FROM       [catalog].[propertyvalue] pv
	INNER JOIN [catalog].[property] p	ON p.propertyid = pv.propertyid AND p.[useinfilter] = 1
	inner join ppvid on ppvid.propertyvalueid = pv.[propertyvalueid]
	WHERE      pv.[useinfilter] = 1			   
	ORDER BY   propertysortorder,
			   pv.[propertyid],
			   pv.[sortorder],
			   pv.[rangevalue],
			   pv.[value]			   
  END
END
GO--

UPDATE Catalog.Category SET ExternalId = CategoryID


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
  @ExternalId nvarchar(50)
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
	  ,[Hidden]
      ,[DisplayStyle]  
      ,[DisplayChildProducts]  
      ,[DisplayBrandsInMenu]  
      ,[DisplaySubCategoriesInMenu]  
      ,[UrlPath]  
      ,[Sorting]
	  ,[ExternalId]
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
	@Hidden,
    @DisplayStyle,  
    @DisplayChildProducts,  
    @DisplayBrandsInMenu,  
    @DisplaySubCategoriesInMenu,  
    @UrlPath,  
    @Sorting,
	@ExternalId);  

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
  @ExternalId nvarchar(50)
  
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
  WHERE CategoryID = @CategoryID  
END  
  
GO--
CREATE TABLE Module.SimalandSettings
	(
	Id int NOT NULL IDENTITY (1, 1),
	Name nvarchar(50) NOT NULL,
	Value nvarchar(MAX) NOT NULL
	)  ON [PRIMARY]
	 TEXTIMAGE_ON [PRIMARY]
	 
GO--

ALTER TABLE Module.SimalandSettings ADD CONSTRAINT
	PK_SimalandSettings PRIMARY KEY CLUSTERED 
	(
	Id
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]	
GO--

CREATE PROCEDURE [Catalog].[sp_AddProductToCategoryByExternalId] 
	@ProductID int,
	@ExternalId nvarchar(50),
	@SortOrder int	
AS
BEGIN

	DECLARE @Main bit
		SET NOCOUNT ON;
	DECLARE @CategoryId int;

	SET @CategoryId = (SELECT [CategoryId] FROM [Catalog].Category WHERE [ExternalId] = @ExternalId)

	if (select count([CategoryId]) from [Catalog].[ProductCategories] where ProductID=@ProductID and main=1) = 0
		set @Main = 1
	else
		set @Main = 0

	if (select count([CategoryId]) from [Catalog].[ProductCategories] where [CategoryId]=@CategoryId and [ProductId]=@ProductId) = 0 
	begin
		INSERT INTO [Catalog].[ProductCategories] (CategoryID, ProductID, SortOrder, Main) VALUES (@CategoryId, @ProductId, @SortOrder, @Main);
	end
END

GO--

ALTER TABLE Customers.TaskGroup ADD
	IsPrivateComments bit NULL
GO--

Update Customers.TaskGroup Set IsPrivateComments = 0
GO--

ALTER TABLE Customers.TaskGroup ALTER COLUMN IsPrivateComments bit NOT NULL
GO--

ALTER TABLE Customers.FacebookUser ADD
	PhotoPicByPsyId nvarchar(MAX) NULL
GO--

CREATE TABLE [CMS].[LandingColorScheme](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[LpId] [int] NOT NULL,
	[LpBlockId] [int] NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[Class] [nvarchar](50) NOT NULL,
	[BackgroundColor] [nvarchar](30) NOT NULL,
	[BackgroundColorAlt] [nvarchar](30) NOT NULL,
	[TitleColor] [nvarchar](30) NOT NULL,
	[SubTitleColor] [nvarchar](30) NOT NULL,
	[TextColor] [nvarchar](30) NOT NULL,
	[TitleBold] [nvarchar](30) NOT NULL,
	[SubTitleBold] [nvarchar](30) NOT NULL,
	[TextBold] [nvarchar](30) NOT NULL,	
	[LinkColor] [nvarchar](30) NOT NULL,
	[LinkColorHover] [nvarchar](30) NOT NULL,
	[LinkColorActive] [nvarchar](30) NOT NULL,
	[ButtonTextColor] [nvarchar](30) NOT NULL,
	[ButtonBorderColor] [nvarchar](30) NOT NULL,
	[ButtonBorderWidth] [nvarchar](30) NOT NULL,
	[ButtonBorderRadius] [nvarchar](30) NOT NULL,
	[ButtonBackgroundColor] [nvarchar](30) NOT NULL,
	[ButtonBackgroundColorHover] [nvarchar](30) NOT NULL,
	[ButtonBackgroundColorActive] [nvarchar](30) NOT NULL,
	[ButtonSecondaryTextColor] [nvarchar](30) NOT NULL,
	[ButtonSecondaryBorderColor] [nvarchar](30) NOT NULL,
	[ButtonSecondaryBorderWidth] [nvarchar](30) NOT NULL,
	[ButtonSecondaryBorderRadius] [nvarchar](30) NOT NULL,
	[ButtonSecondaryBackgroundColor] [nvarchar](30) NOT NULL,
	[ButtonSecondaryBackgroundColorHover] [nvarchar](30) NOT NULL,
	[ButtonSecondaryBackgroundColorActive] [nvarchar](30) NOT NULL,
	[DelimiterColor] [nvarchar](30) NOT NULL,
 CONSTRAINT [PK_LandingColorScheme] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO--

ALTER TABLE [CMS].[LandingColorScheme]  WITH CHECK ADD  CONSTRAINT [FK_LandingColorScheme_LandingBlock] FOREIGN KEY([LpBlockId])
REFERENCES [CMS].[LandingBlock] ([Id])
ON DELETE CASCADE
GO--

ALTER TABLE [CMS].[LandingColorScheme] CHECK CONSTRAINT [FK_LandingColorScheme_LandingBlock]
GO--


ALTER TABLE CMS.LandingBlock ADD
	ShowOnAllPages bit NULL
GO--

Update CMS.LandingBlock Set ShowOnAllPages = 0
GO--

ALTER TABLE CMS.LandingBlock ALTER COLUMN ShowOnAllPages bit NOT NULL
GO--


--Временный файл. В готовом варианте будет перенесенно в основной

GO--

CREATE SCHEMA [Booking]

GO--

CREATE TABLE [Booking].[Affiliate](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CityId] [int] NULL,
	[Name] [nvarchar](255) NULL,
	[Description] [nvarchar](max) NULL,
	[Address] [nvarchar](max) NULL,
	[Phone] [nvarchar](255) NULL,
	[BookingIntervalMinutes] [int] NOT NULL,
	[SortOrder] [int] NOT NULL,
	[Enabled] [bit] NOT NULL,
 CONSTRAINT [PK_Affiliate] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO--

ALTER TABLE [Booking].[Affiliate]  WITH CHECK ADD  CONSTRAINT [FK_Affiliate_City] FOREIGN KEY([CityId])
REFERENCES [Customers].[City] ([CityID])
ON UPDATE CASCADE
ON DELETE SET NULL
GO--

ALTER TABLE [Booking].[Affiliate] CHECK CONSTRAINT [FK_Affiliate_City]
GO--




CREATE TABLE [Booking].[AffiliateTimeOfBooking](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[AffiliateId] [int] NOT NULL,
	[DayOfWeek] [tinyint] NOT NULL,
	[StartTime] [datetime] NOT NULL,
	[EndTime] [datetime] NOT NULL,
 CONSTRAINT [PK_AffiliateTimeOfBooking] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO--

ALTER TABLE [Booking].[AffiliateTimeOfBooking]  WITH CHECK ADD  CONSTRAINT [FK_AffiliateTimeOfBooking_Affiliate] FOREIGN KEY([AffiliateId])
REFERENCES [Booking].[Affiliate] ([Id])
ON DELETE CASCADE
GO--

ALTER TABLE [Booking].[AffiliateTimeOfBooking] CHECK CONSTRAINT [FK_AffiliateTimeOfBooking_Affiliate]
GO--

CREATE NONCLUSTERED INDEX [IX_AffiliateTimeOfBooking] ON [Booking].[AffiliateTimeOfBooking]
(
	[AffiliateId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO--





CREATE TABLE [Booking].[Category](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Image] [nvarchar](100) NULL,
	[SortOrder] [int] NOT NULL,
	[Enabled] [bit] NOT NULL,
 CONSTRAINT [PK_Category] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO--





CREATE TABLE [Booking].[Service](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CategoryId] [int] NOT NULL,
	[CurrencyId] [int] NOT NULL,
	[Name] [nvarchar](255) NULL,
	[Price] [float] NOT NULL,
	[Image] [nvarchar](100) NULL,
	[Description] [nvarchar](max) NULL,
	[SortOrder] [int] NOT NULL,
	[Enabled] [bit] NOT NULL,
 CONSTRAINT [PK_Service] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO--

ALTER TABLE [Booking].[Service]  WITH CHECK ADD  CONSTRAINT [FK_Service_Currency] FOREIGN KEY([CurrencyId])
REFERENCES [Catalog].[Currency] ([CurrencyID])
GO--

ALTER TABLE [Booking].[Service] CHECK CONSTRAINT [FK_Service_Currency]
GO--

CREATE NONCLUSTERED INDEX [IX_Service_Category] ON [Booking].[Service]
(
	[CategoryId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO--





CREATE TABLE [Booking].[Employees](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[AffiliateId] [int] NOT NULL,
	[CustomerId] [uniqueidentifier] NOT NULL,
	[BookingIntervalMinutes] [int] NULL,
	[Active] [bit] NOT NULL,
 CONSTRAINT [PK_AffiliateCustomer] PRIMARY KEY CLUSTERED 
(
	[AffiliateId] ASC,
	[CustomerId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO--

ALTER TABLE [Booking].[Employees] ADD  CONSTRAINT [DF_AffiliateCustomer_CustomerID]  DEFAULT (newid()) FOR [CustomerId]
GO--

ALTER TABLE [Booking].[Employees]  WITH CHECK ADD  CONSTRAINT [FK_AffiliateCustomer_Affiliate] FOREIGN KEY([AffiliateId])
REFERENCES [Booking].[Affiliate] ([Id])
ON DELETE CASCADE
GO--

ALTER TABLE [Booking].[Employees] CHECK CONSTRAINT [FK_AffiliateCustomer_Affiliate]
GO--

ALTER TABLE [Booking].[Employees]  WITH CHECK ADD  CONSTRAINT [FK_AffiliateCustomer_Customer] FOREIGN KEY([CustomerId])
REFERENCES [Customers].[Customer] ([CustomerID])
ON DELETE CASCADE
GO--

ALTER TABLE [Booking].[Employees] CHECK CONSTRAINT [FK_AffiliateCustomer_Customer]
GO--

CREATE UNIQUE NONCLUSTERED INDEX [IX_Employees] ON [Booking].[Employees]
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO--





CREATE TABLE [Booking].[EmployeeTimeOfBooking](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[EmployeeId] [int] NOT NULL,
	[DayOfWeek] [tinyint] NOT NULL,
	[StartTime] [datetime] NOT NULL,
	[EndTime] [datetime] NOT NULL,
 CONSTRAINT [PK_EmployeeTimeOfBooking] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO--

ALTER TABLE [Booking].[EmployeeTimeOfBooking]  WITH CHECK ADD  CONSTRAINT [FK_EmployeeTimeOfBooking_Employees] FOREIGN KEY([EmployeeId])
REFERENCES [Booking].[Employees] ([Id])
ON DELETE CASCADE
GO--

ALTER TABLE [Booking].[EmployeeTimeOfBooking] CHECK CONSTRAINT [FK_EmployeeTimeOfBooking_Employees]
GO--

CREATE NONCLUSTERED INDEX [IX_EmployeeTimeOfBooking] ON [Booking].[EmployeeTimeOfBooking]
(
	[EmployeeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO--





CREATE TABLE [Booking].[EmployeeService](
	[AffiliateId] [int] NOT NULL,
	[CustomerId] [uniqueidentifier] NOT NULL,
	[ServiceId] [int] NOT NULL,
 CONSTRAINT [PK_EmployeeService] PRIMARY KEY CLUSTERED 
(
	[AffiliateId] ASC,
	[CustomerId] ASC,
	[ServiceId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO--

ALTER TABLE [Booking].[EmployeeService]  WITH CHECK ADD  CONSTRAINT [FK_EmployeeService_Affiliate] FOREIGN KEY([AffiliateId])
REFERENCES [Booking].[Affiliate] ([Id])
ON DELETE CASCADE
GO--

ALTER TABLE [Booking].[EmployeeService] CHECK CONSTRAINT [FK_EmployeeService_Affiliate]
GO--

ALTER TABLE [Booking].[EmployeeService]  WITH CHECK ADD  CONSTRAINT [FK_EmployeeService_Customer] FOREIGN KEY([CustomerId])
REFERENCES [Customers].[Customer] ([CustomerID])
ON DELETE CASCADE
GO--

ALTER TABLE [Booking].[EmployeeService] CHECK CONSTRAINT [FK_EmployeeService_Customer]
GO--

ALTER TABLE [Booking].[EmployeeService]  WITH CHECK ADD  CONSTRAINT [FK_EmployeeService_Service] FOREIGN KEY([ServiceId])
REFERENCES [Booking].[Service] ([Id])
ON DELETE CASCADE
GO--

ALTER TABLE [Booking].[EmployeeService] CHECK CONSTRAINT [FK_EmployeeService_Service]
GO--





CREATE TABLE [Booking].[Booking](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[AffiliateId] [int] NOT NULL,
	[EmployeeId] [uniqueidentifier] NULL,
	[BeginDate] [datetime] NOT NULL,
	[EndDate] [datetime] NOT NULL,
	[Status] [tinyint] NOT NULL,
	[Sum] [float] NOT NULL,
	[DateAdded] [datetime] NOT NULL,
 CONSTRAINT [PK_Booking] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO--

ALTER TABLE [Booking].[Booking] ADD  CONSTRAINT [DF_Booking_Sum]  DEFAULT ((0)) FOR [Sum]
GO--

ALTER TABLE [Booking].[Booking]  WITH CHECK ADD  CONSTRAINT [FK_Booking_Affiliate] FOREIGN KEY([AffiliateId])
REFERENCES [Booking].[Affiliate] ([Id])
GO--

ALTER TABLE [Booking].[Booking] CHECK CONSTRAINT [FK_Booking_Affiliate]
GO--

ALTER TABLE [Booking].[Booking]  WITH CHECK ADD  CONSTRAINT [FK_Booking_Customer] FOREIGN KEY([EmployeeId])
REFERENCES [Customers].[Customer] ([CustomerID])
ON DELETE SET NULL
GO--

ALTER TABLE [Booking].[Booking] CHECK CONSTRAINT [FK_Booking_Customer]
GO--

CREATE NONCLUSTERED INDEX [Booking_Affiliate] ON [Booking].[Booking]
(
	[AffiliateId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO--





CREATE TABLE [Booking].[BookingCustomer](
	[BookingId] [int] NOT NULL,
	[CustomerId] [uniqueidentifier] NOT NULL,
	[FirstName] [nvarchar](70) NULL,
	[LastName] [nvarchar](70) NULL,
	[Email] [nvarchar](70) NULL,
	[Phone] [nvarchar](70) NULL,
	[Patronymic] [nvarchar](70) NULL,
	[StandardPhone] [bigint] NULL,
 CONSTRAINT [PK_BookingCustomer] PRIMARY KEY CLUSTERED 
(
	[BookingId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO--

ALTER TABLE [Booking].[BookingCustomer]  WITH CHECK ADD  CONSTRAINT [FK_BookingCustomer_Booking] FOREIGN KEY([BookingId])
REFERENCES [Booking].[Booking] ([Id])
ON DELETE CASCADE
GO--

ALTER TABLE [Booking].[BookingCustomer] CHECK CONSTRAINT [FK_BookingCustomer_Booking]
GO--





CREATE TABLE [Booking].[BookingItems](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[BookingId] [int] NOT NULL,
	[ServiceId] [int] NULL,
	[Name] [nvarchar](255) NULL,
	[Price] [float] NOT NULL,
	[Amount] [float] NOT NULL,
 CONSTRAINT [PK_BookingItems] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO--

ALTER TABLE [Booking].[BookingItems]  WITH CHECK ADD  CONSTRAINT [FK_BookingItems_Booking] FOREIGN KEY([BookingId])
REFERENCES [Booking].[Booking] ([Id])
ON DELETE CASCADE
GO--

ALTER TABLE [Booking].[BookingItems] CHECK CONSTRAINT [FK_BookingItems_Booking]
GO--

ALTER TABLE [Booking].[BookingItems]  WITH CHECK ADD  CONSTRAINT [FK_BookingItems_Service] FOREIGN KEY([ServiceId])
REFERENCES [Booking].[Service] ([Id])
ON DELETE SET NULL
GO--

ALTER TABLE [Booking].[BookingItems] CHECK CONSTRAINT [FK_BookingItems_Service]
GO--

CREATE NONCLUSTERED INDEX [IX_BookingItems] ON [Booking].[BookingItems]
(
	[BookingId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO--





CREATE TABLE [Booking].[BookingCurrency](
	[BookingId] [int] NOT NULL,
	[CurrencyCode] [nchar](3) NOT NULL,
	[CurrencyNumCode] [int] NOT NULL,
	[CurrencyValue] [float] NOT NULL,
	[CurrencySymbol] [nvarchar](7) NOT NULL,
	[IsCodeBefore] [bit] NOT NULL,
	[RoundNumbers] [float] NOT NULL,
	[EnablePriceRounding] [bit] NOT NULL,
 CONSTRAINT [PK_BookingCurrency] PRIMARY KEY CLUSTERED 
(
	[BookingId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO--

ALTER TABLE [Booking].[BookingCurrency]  WITH CHECK ADD  CONSTRAINT [FK_BookingCurrency_Booking] FOREIGN KEY([BookingId])
REFERENCES [Booking].[Booking] ([Id])
ON DELETE CASCADE
GO--

ALTER TABLE [Booking].[BookingCurrency] CHECK CONSTRAINT [FK_BookingCurrency_Booking]
GO--





CREATE TABLE [Booking].[AffiliateAdditionalTime](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[AffiliateId] [int] NOT NULL,
	[StartTime] [datetime] NOT NULL,
	[EndTime] [datetime] NOT NULL,
	[IsWork] [bit] NOT NULL,
 CONSTRAINT [PK_AffiliateAdditionalTime] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO--

ALTER TABLE [Booking].[AffiliateAdditionalTime]  WITH CHECK ADD  CONSTRAINT [FK_AffiliateAdditionalTime_Affiliate] FOREIGN KEY([AffiliateId])
REFERENCES [Booking].[Affiliate] ([Id])
ON DELETE CASCADE
GO--

ALTER TABLE [Booking].[AffiliateAdditionalTime] CHECK CONSTRAINT [FK_AffiliateAdditionalTime_Affiliate]
GO--

CREATE NONCLUSTERED INDEX [IX_AffiliateAdditionalTime] ON [Booking].[AffiliateAdditionalTime]
(
	[AffiliateId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO--






CREATE TABLE [Booking].[EmployeeAdditionalTime](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[EmployeeId] [int] NOT NULL,
	[StartTime] [datetime] NOT NULL,
	[EndTime] [datetime] NOT NULL,
	[IsWork] [bit] NOT NULL,
 CONSTRAINT [PK_EmployeeAdditionalTime] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO--

ALTER TABLE [Booking].[EmployeeAdditionalTime]  WITH CHECK ADD  CONSTRAINT [FK_EmployeeAdditionalTime_Employees] FOREIGN KEY([EmployeeId])
REFERENCES [Booking].[Employees] ([Id])
ON DELETE CASCADE
GO--

ALTER TABLE [Booking].[EmployeeAdditionalTime] CHECK CONSTRAINT [FK_EmployeeAdditionalTime_Employees]
GO--

CREATE NONCLUSTERED INDEX [IX_EmployeeAdditionalTime] ON [Booking].[EmployeeAdditionalTime]
(
	[EmployeeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO--






CREATE TABLE [Booking].[AffiliateCategory](
	[AffiliateId] [int] NOT NULL,
	[CategoryId] [int] NOT NULL,
 CONSTRAINT [PK_AffiliateCategory] PRIMARY KEY CLUSTERED 
(
	[AffiliateId] ASC,
	[CategoryId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO--

ALTER TABLE [Booking].[AffiliateCategory]  WITH CHECK ADD  CONSTRAINT [FK_AffiliateCategory_Affiliate] FOREIGN KEY([AffiliateId])
REFERENCES [Booking].[Affiliate] ([Id])
ON DELETE CASCADE
GO--

ALTER TABLE [Booking].[AffiliateCategory] CHECK CONSTRAINT [FK_AffiliateCategory_Affiliate]
GO--

ALTER TABLE [Booking].[AffiliateCategory]  WITH CHECK ADD  CONSTRAINT [FK_AffiliateCategory_Category] FOREIGN KEY([CategoryId])
REFERENCES [Booking].[Category] ([Id])
ON DELETE CASCADE
GO--

ALTER TABLE [Booking].[AffiliateCategory] CHECK CONSTRAINT [FK_AffiliateCategory_Category]
GO--




CREATE TABLE [Booking].[AffiliateService](
	[AffiliateId] [int] NOT NULL,
	[ServiceId] [int] NOT NULL,
 CONSTRAINT [PK_AffiliateService] PRIMARY KEY CLUSTERED 
(
	[AffiliateId] ASC,
	[ServiceId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO--

ALTER TABLE [Booking].[AffiliateService]  WITH CHECK ADD  CONSTRAINT [FK_AffiliateService_Affiliate] FOREIGN KEY([AffiliateId])
REFERENCES [Booking].[Affiliate] ([Id])
ON DELETE CASCADE
GO--

ALTER TABLE [Booking].[AffiliateService] CHECK CONSTRAINT [FK_AffiliateService_Affiliate]
GO--

ALTER TABLE [Booking].[AffiliateService]  WITH CHECK ADD  CONSTRAINT [FK_AffiliateService_Service] FOREIGN KEY([ServiceId])
REFERENCES [Booking].[Service] ([Id])
ON DELETE CASCADE
GO--

ALTER TABLE [Booking].[AffiliateService] CHECK CONSTRAINT [FK_AffiliateService_Service]
GO--




UPDATE [Settings].[InternalSettings] SET [settingValue] = '6.5.0' WHERE [settingKey] = 'db_version'