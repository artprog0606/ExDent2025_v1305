
-- SQL_4.1.11_Part1

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
		SELECT  Top(1) CASE 
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
		SELECT  Top(1) CASE 
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
		SELECT  Top(1) CASE 
				WHEN COUNT(ProductVideoID) > 0
					THEN 1
				ELSE 0
				END
		FROM [Catalog].[ProductVideo]
		WHERE ProductID = [ProductExt].ProductId
		)
	,[MaxAvailable] = (
		SELECT  Top(1) CASE 
				WHEN Max(Offer.Amount) > 0
					THEN 1
				ELSE 0
				END
		FROM [Catalog].Offer
		WHERE ProductId = [ProductExt].ProductId
		)
	,[NotSamePrices] = (
		SELECT  Top(1) CASE 
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
		SELECT  Top(1) CASE 
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
	,[OfferId] = (SELECT Top(1) OfferID
		FROM [Catalog].offer
		WHERE offer.productid = [ProductExt].ProductId and (offer.Main = 1 OR offer.Main IS NULL))   
END

GO--
Insert Into Customers.RoleAction ([Name], [Key], [Enabled], [Category], [SortOrder])
Values('Отправка ссылки на оплату', 'DisplaySendPaymentLink',1,'Заказы',5)

GO--
alter table [order].[OrderContact]
alter column Name nvarchar(1000) null

GO--
alter table [order].[OrderContact]
alter column Address nvarchar(1000) null

GO--
alter table [order].[OrderContact]
alter column CustomField1 nvarchar(1000) null

GO--
alter table [order].[OrderContact]
alter column CustomField2 nvarchar(1000) null

GO--
alter table [order].[OrderContact]
alter column CustomField3 nvarchar(1000) null


GO--
alter table [order].[OrderCustomer]
alter column FirstName nvarchar(1000) null

GO--
alter table [order].[OrderCustomer]
alter column LastName nvarchar(1000) null

GO--
alter table [order].[OrderCustomer]
alter column MobilePhone nvarchar(1000) null
GO--

alter table [order].[OrderContact]
alter column City nvarchar(1000) null
GO--

CREATE TABLE [Order].[ShippingCache](
	[ShippingMethodID] [int] NOT NULL,
	[ParamHash] [int] NOT NULL,
	[Options] [nvarchar](max) NOT NULL,
	[Created] [datetime] NOT NULL,
 CONSTRAINT [PK_ShippingCache] PRIMARY KEY CLUSTERED 
(
	[ShippingMethodID] ASC,
	[ParamHash] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO--

ALTER TABLE [Order].[ShippingCache]  WITH CHECK ADD  CONSTRAINT [FK_ShippingCache_ShippingMethod] FOREIGN KEY([ShippingMethodID])
REFERENCES [Order].[ShippingMethod] ([ShippingMethodID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO--

ALTER TABLE [Order].[ShippingCache] CHECK CONSTRAINT [FK_ShippingCache_ShippingMethod]
GO--

UPDATE [Settings].[InternalSettings] SET [settingValue] = '4.1.11' WHERE [settingKey] = 'db_version'
