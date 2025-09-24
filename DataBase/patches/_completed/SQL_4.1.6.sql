
-- SQL_4.1.6_Part1

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
			Insert into Catalog.Property ( Name, UseInFilter, SortOrder, Expanded, [Type], [UseInDetails]) Values ( @Name, 0, 0, 0, 0, 1 )
			Select @propertyId = PropertyID From Catalog.Property Where Name = @Name
		end
	
	declare @propertyValueId int;
	Set @propertyValueId = 0;
	Select @propertyValueId = PropertyValueID From Catalog.PropertyValue Where Value = @Value and PropertyID = @propertyId;
	if(@propertyValueId = 0)			
		begin
			Insert into Catalog.PropertyValue ( Value, PropertyID ) Values ( @Value, @propertyId )
			Select @propertyValueId = PropertyValueID From Catalog.PropertyValue Where Value = @Value and PropertyID = @propertyId
		end
						
	if( (Select COUNT(ProductID) From Catalog.ProductPropertyValue Where ProductID = @ProductID and PropertyValueID = @propertyValueId) = 0 )
		begin	
			Insert into Catalog.ProductPropertyValue ( ProductID, PropertyValueID, SortOrder ) Values ( @ProductID, @propertyValueId, @SortOrder )
		end
END

GO--

ALTER PROCEDURE [Catalog].[PreCalcProductParamsMass]
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
				WHEN MaxAvailable <= 0
					OR MaxAvailable < IsNull(Product.MinAmount, 0)
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
Insert Into [Settings].[Settings] (Name, Value) VALUES ('ShowProducerFilter', 'True')
Insert Into [Settings].[Settings] (Name, Value) VALUES ('DisplayWeight', 'True')
Insert Into [Settings].[Settings] (Name, Value) VALUES ('DisplayDimensions', 'True')

GO--


ALTER TABLE [Order].ShippingMethod ADD
	ZeroPriceMessage nvarchar(255) NULL

GO--

UPDATE [Settings].[InternalSettings] SET [settingValue] = '4.1.6' WHERE [settingKey] = 'db_version'




