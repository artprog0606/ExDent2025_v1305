
-- SQL_4.1.7_Part1

ALTER PROCEDURE [Settings].[sp_GetCsvProducts] @moduleName NVARCHAR(50)
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


CREATE TABLE [Order].[ShippingCityExcluded](
	[MethodId] [int] NOT NULL,
	[CityId] [int] NOT NULL,
 CONSTRAINT [PK_ShippingPaymentCityExcluded_1] PRIMARY KEY CLUSTERED 
(
	[MethodId] ASC,
	[CityId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]

GO--

ALTER TABLE [Order].[ShippingCityExcluded]  WITH CHECK ADD  CONSTRAINT [FK_ShippingCityExcluded_City] FOREIGN KEY([CityId])
REFERENCES [Customers].[City] ([CityID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO--

ALTER TABLE [Order].[ShippingCityExcluded] CHECK CONSTRAINT [FK_ShippingCityExcluded_City]
GO--

ALTER TABLE [Order].[ShippingCityExcluded]  WITH CHECK ADD  CONSTRAINT [FK_ShippingCityExcluded_ShippingMethod] FOREIGN KEY([MethodId])
REFERENCES [Order].[ShippingMethod] ([ShippingMethodID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO--

ALTER TABLE [Order].[ShippingCityExcluded] CHECK CONSTRAINT [FK_ShippingCityExcluded_ShippingMethod]
GO--


UPDATE [Settings].[InternalSettings] SET [settingValue] = '4.1.7' WHERE [settingKey] = 'db_version'




