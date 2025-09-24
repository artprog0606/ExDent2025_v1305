
GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.EditTask.Started', 'выполняется')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.EditTask.Started', 'in progress')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.Tasks.Tasks.HasBeenDeleted', 'Задача удалена')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.Tasks.Tasks.HasBeenDeleted', 'Task has been deleted')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.EditTask.TaskSaved', 'сохранена')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.EditTask.TaskSaved', 'saved')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.Tasks.Tasks.CopyTaskName', 'Копия задачи')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.Tasks.Tasks.CopyTaskName', 'Copy of task')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.Tasks.Tasks.HasBeenCopied', 'успешно сделана')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.Tasks.Tasks.HasBeenCopied', 'has been done')

Update [Settings].[Localization] Set [ResourceValue] = 'исполнитель изменен' Where [ResourceKey] = 'Admin.Js.Tasks.Tasks.HasBeenCopied' and [LanguageId] = 1

Update [Settings].[Localization] Set [ResourceValue] = 'наблюдающий изменен' Where [ResourceKey] = 'Admin.Js.Tasks.Tasks.ObserverChanged' and [LanguageId] = 1

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.Tasks.Tasks.CommentToTask', 'Комментарий к задаче')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.Tasks.Tasks.CommentToTask', 'Comment to task')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.Tasks.Tasks.CommentToTaskAdded', 'успешно добавлен')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.Tasks.Tasks.CommentToTaskAdded', 'successfully added')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.AddRecomProperty.NoCoincidesWithProperty', 'Не совпадает со свойством товара')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.AddRecomProperty.NoCoincidesWithProperty', 'Does not match the product property')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.CatProductRecommendations.NotSame', 'Не совпадает со свойством товара')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.CatProductRecommendations.NotSame', 'Does not match the product property')

GO--

ALTER TABLE Catalog.RelatedProperties ADD
	IsSame bit NULL
	
GO--

Update Catalog.RelatedProperties Set IsSame = 1

GO--

ALTER TABLE [Catalog].[ProductList] ADD
	ShuffleList bit NULL

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.MainPageProductsStore.Index.ShuffleList', 'Перемешивать товары раз в день')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.MainPageProductsStore.Index.ShuffleList', 'Shuffle products once a day')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.EditMainPageList.ShuffleList', 'Перемешивать товары раз в день')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.EditMainPageList.ShuffleList', 'Shuffle products once a day')

GO--

IF (NOT EXISTS(SELECT * FROM [Settings].[Localization] WHERE [ResourceKey] = 'Core.Customers.RoleActionCategory.Analytics'))
BEGIN
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Customers.RoleActionCategory.Analytics', 'Отчеты')
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Customers.RoleActionCategory.Analytics', 'Analytics')
END

GO--

ALTER TABLE Catalog.Coupon ADD
	IsMinimalOrderPriceFromAllCart bit NULL
GO--

Update Catalog.Coupon Set IsMinimalOrderPriceFromAllCart = 0

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.AddEditCoupon.MinimalOrderPriceFromAllCart', 'Учитывать всю сумму корзины')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.AddEditCoupon.MinimalOrderPriceFromAllCart', 'Use for all cart price')

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
		AND ((HirecalEnabled = 1 AND Enabled = 1) OR @exportNotAvailable = 1)


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
	WHERE (HirecalEnabled = 1 AND Enabled = 1) OR @exportNotAvailable = 1;

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
		AND (CategoryEnabled = 1 OR @exportNotAvailable = 1)
		AND (p.Enabled = 1 OR @exportNotAvailable = 1)	
		AND (@onlyMainOfferToExport = 0 OR Offer.Main = 1)
	END
	IF @sqlMode = 'GetProducts'
	BEGIN
	with cte as (
		SELECT Distinct tmp.CategoryId
		FROM @lcategorytemp AS tmp
		INNER JOIN CATALOG.Category ON Category.CategoryId = tmp.CategoryId
		WHERE (HirecalEnabled = 1 AND Enabled = 1) OR @exportNotAvailable = 1)
		
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
		AND (CategoryEnabled = 1 OR @exportNotAvailable = 1)
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
		AND (CategoryEnabled = 1 OR @exportNotAvailable = 1)
		AND (p.Enabled = 1 OR @exportNotAvailable = 1)	
		AND (@onlyMainOfferToExport = 0 OR Offer.Main = 1)
	END
END

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.SettingsSeo.GoogleAnalytics.UserRegistration','Регистрация пользователя')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.SettingsSeo.GoogleAnalytics.UserRegistration','User registration')

GO--

declare @PiterId int = (select top 1 CityId from Customers.City inner join Customers.Region on Region.RegionID = City.RegionID where CityName = 'Санкт-Петербург' and RegionName = 'Санкт-Петербург')
declare @PiterIdLenObl int = (select top 1 CityId from Customers.City inner join Customers.Region on Region.RegionID = City.RegionID where CityName = 'Санкт-Петербург' and RegionName = 'Ленинградская область')
if @PiterIdLenObl is not null and @PiterId is not null
begin
	if exists (select 1 from [Order].PaymentCity where CityId = @PiterIdLenObl)
		update [Order].PaymentCity set CityId = @PiterId where CityId = @PiterIdLenObl and MethodId not in (select MethodId from [Order].PaymentCity where CityId = @PiterId)
	if exists (select 1 from [Order].ShippingCityExcluded where CityId = @PiterIdLenObl)
		update [Order].ShippingCityExcluded set CityId = @PiterId where CityId = @PiterIdLenObl and MethodId not in (select MethodId from [Order].ShippingCityExcluded where CityId = @PiterId)
	if exists (select 1 from [Order].ShippingCity where CityId = @PiterIdLenObl)
		update [Order].ShippingCity set CityId = @PiterId where CityId = @PiterIdLenObl and MethodId not in (select MethodId from [Order].ShippingCity where CityId = @PiterId)
	if exists (select 1 from Booking.Affiliate where CityId = @PiterIdLenObl)
		update Booking.Affiliate set CityId = @PiterId where CityId = @PiterIdLenObl and Id not in (select Id from Booking.Affiliate where CityId = @PiterId)

	delete from Customers.City where CityId = @PiterIdLenObl
end

GO--

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (1, 'Admin.Settings.Catalog.ReviewsVoiteOnlyRegisteredUsers', 'Разрешить голосование за отзывы только зарегестрированным пользователям');
Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (2, 'Admin.Settings.Catalog.ReviewsVoiteOnlyRegisteredUsers', 'Allow only registered users to vote for reviews');
Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (1, 'Admin.SettingsCatalog.Product.DescriptionReviewsVoiteOnlyRegisteredUsers', ', голосовать за полезность отзыва смогут только зарегистрированные пользователи');
Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (2, 'Admin.SettingsCatalog.Product.DescriptionReviewsVoiteOnlyRegisteredUsers', ', only registered users can vote for the usefulness of the review');
GO--

IF NOT EXISTS(SELECT * FROM [Settings].[MailFormatType] WHERE [MailType] = 'OnMissedCall')
INSERT INTO [Settings].[MailFormatType] ([TypeName], [SortOrder], [Comment], [MailType]) 
VALUES ('Уведомление о пропущенном звонке', 410, 
	'Письмо о пропущенном звонке (#PHONE#)', 
	'OnMissedCall')
	
GO--

IF NOT EXISTS(SELECT * FROM [Settings].[MailFormat] WHERE [MailFormatTypeId] = (SELECT TOP(1) [MailFormatTypeID] FROM [Settings].[MailFormatType] WHERE [MailType] = 'OnMissedCall'))
INSERT INTO [Settings].[MailFormat] ([FormatName],[FormatText],[SortOrder],[Enable],[AddDate],[ModifyDate],[FormatSubject],[MailFormatTypeId])
Values ('Уведомление о пропущенном звонке', '<div style="color: #4c4f56; font-family: Arial, Helvetica, sans-serif; font-size: 14px;">
<div class="header" style="border-bottom: 1px solid #ededed; display: table; margin-bottom: 25px; padding-bottom: 25px; width: 100%;">
<div class="logo" style="display: table-cell; text-align: left; vertical-align: middle;">#LOGO#</div>
<div class="phone" style="display: table-cell; text-align: right; vertical-align: middle;">
<div class="tel" style="font-size: 26px; font-weight: bold; line-height: 1; margin-bottom: 5px;">&nbsp;</div>
<div class="inform" style="font-size: 12px;">&nbsp;</div>
</div>
</div>

<div>
<p>Пропущенный звонок от #PHONE#.</p>
</div>
</div>', 1610, 1, GETDATE(), GETDATE(), 'Пропущенный звонок от #PHONE#', (SELECT TOP(1) [MailFormatTypeID] FROM [Settings].[MailFormatType] WHERE [MailType] = 'OnMissedCall'))

GO--

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (1, 'Admin.Settings.NotifyEMails.EmailForMissedCall', 'E-mail для уведомлений о пропущенных звонках');
Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (2, 'Admin.Settings.NotifyEMails.EmailForMissedCall', 'E-mail for notifications missed calls');

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.SettingsCrm.Index.Common','Общие')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.SettingsCrm.Index.Common','Common')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.SettingsCrm.Index.Leads','Лиды')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.SettingsCrm.Index.Leads','Leads')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.SettingsCrm.Index.CrmActive','Активность')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.SettingsCrm.Index.CrmActive','Activity')

GO--

IF NOT EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND OBJECT_ID = OBJECT_ID('[Catalog].[sp_GetBrandsByCategoryId]'))
	exec('CREATE PROCEDURE [Catalog].[sp_GetBrandsByCategoryId] 
			@CategoryId int,
			@Type nvarchar(50),
			@Indepth bit
		AS  
		Begin

		if (@Indepth = 1)
			Begin
				DECLARE @brandIds TABLE (brandId INT PRIMARY KEY CLUSTERED);

				with brandCategory as (
					SELECT p.BrandId,
							pc.CategoryId
					FROM [Catalog].Product p
							INNER JOIN [Catalog].ProductCategories pc ON pc.ProductId = p.ProductID
							INNER JOIN [Catalog].[ProductExt] pExt ON p.ProductID = pExt.ProductID
							INNER JOIN [Catalog].Category c ON c.CategoryID = pc.CategoryId
					WHERE p.BrandId IS NOT NULL               
							AND p.[Enabled] = 1
							AND p.Hidden = 0
							AND p.CategoryEnabled = 1
							AND c.Enabled = 1 GROUP BY pc.CategoryId,
								p.BrandId
				)

				INSERT INTO @brandIds
				Select Distinct brandCategory.BrandId From brandCategory 
				Inner Join [Settings].[GetChildCategoryByParent](@CategoryId) AS hCat ON hCat.id = brandCategory.[CategoryID]


				Select Brand.BrandID, Brand.BrandName, Brand.UrlPath, Brand.SortOrder, Photo.PhotoName 
				From Catalog.Brand
				Inner Join @brandIds b on b.brandId = Brand.BrandId
				Left Join Catalog.Photo on Photo.ObjId = Brand.BrandID and Type = @Type 
				Where Brand.Enabled = 1
			End
		Else
			Begin
				with brandCategory as (
					SELECT p.BrandId,
							pc.CategoryId
					FROM [Catalog].Product p
							INNER JOIN [Catalog].ProductCategories pc ON pc.ProductId = p.ProductID
							INNER JOIN [Catalog].[ProductExt] pExt ON p.ProductID = pExt.ProductID
							INNER JOIN [Catalog].Category c ON c.CategoryID = pc.CategoryId
					WHERE p.BrandId IS NOT NULL               
							AND p.[Enabled] = 1
							AND p.Hidden = 0
							AND p.CategoryEnabled = 1
							AND c.Enabled = 1 GROUP BY pc.CategoryId,
								p.BrandId
				)
			
				Select distinct Brand.BrandID, Brand.BrandName, Brand.UrlPath, Brand.SortOrder, Photo.PhotoName 
				From Catalog.Brand 
				Inner Join brandCategory bc on bc.BrandID = brand.BrandID
				Left Join Catalog.Photo on Photo.ObjId = Brand.BrandID and Type = @Type 
				Where Brand.Enabled = 1 and bc.CategoryId = @CategoryId
			End
		End')

GO--

ALTER PROCEDURE [Settings].[sp_GetExportFeedCategories] 
	 @exportFeedId int
	,@onlyCount bit
	,@exportNotAvailable bit = false
AS
BEGIN
	--template for result array
	DECLARE @result TABLE (CategoryId INT PRIMARY KEY CLUSTERED);
	DECLARE @resultTemp TABLE (CategoryId INT);
	-- templete for array of categories
	DECLARE @lcategory TABLE (CategoryId INT PRIMARY KEY CLUSTERED, Opened bit);

	Insert Into @lcategory
		Select t.CategoryId, t.Opened
		From [Settings].[ExportFeedSelectedCategories] AS t
		Inner Join Catalog.Category ON t.CategoryId = Category.CategoryId
		Where ((HirecalEnabled = 1 AND Enabled = 1) OR @exportNotAvailable = 1)
			  AND [ExportFeedId] = @exportFeedId

	DECLARE @l1 INT
	SET @l1 = (Select MIN(CategoryId) From @lcategory);

	WHILE @l1 IS NOT NULL
	BEGIN
		If ((Select Opened from @lcategory where CategoryId=@l1) = 0)
		begin
			--add categories by step thats no in array 
			Insert Into @resultTemp
				Select id From Settings.GetChildCategoryByParent(@l1)
		end
		
		Insert Into @resultTemp
			Select id From [Settings].[GetParentsCategoryByChild](@l1)
		
		SET @l1 = (Select MIN(CategoryId) From @lcategory WHERE CategoryId > @l1);
	END;

	Insert Into @result
		Select Distinct tmp.CategoryId
		From @resultTemp AS tmp
		Inner Join Catalog.Category ON Category.CategoryId = tmp.CategoryId
		Where ((HirecalEnabled = 1 AND Enabled = 1) OR @exportNotAvailable = 1);

	IF @onlyCount = 1
	BEGIN
		Select Count([CategoryID])
		From [Catalog].[Category]
		Where CategoryID <> 0 AND CategoryId IN (Select CategoryId From @result)
	END
	ELSE
	BEGIN
		Select [CategoryID]
			,[ParentCategory]
			,[Name]
		From [Catalog].[Category]
		Where CategoryID <> 0 AND CategoryId IN (Select CategoryId From @result)
	END
END

GO--

ALTER PROCEDURE [Catalog].[sp_GetBrandsByCategoryId] 
	@CategoryId int,
	@Type nvarchar(50),
	@Indepth bit,
	@OnlyAvailable bit
AS  
Begin

if (@Indepth = 1)
	Begin
		DECLARE @brandIds TABLE (brandId INT PRIMARY KEY CLUSTERED);

		with brandCategory as (
			SELECT p.BrandId,
					pc.CategoryId
			FROM [Catalog].Product p
					INNER JOIN [Catalog].ProductCategories pc ON pc.ProductId = p.ProductID
					INNER JOIN [Catalog].[ProductExt] pExt ON p.ProductID = pExt.ProductID
					INNER JOIN [Catalog].Category c ON c.CategoryID = pc.CategoryId
			WHERE p.BrandId IS NOT NULL               
					AND p.[Enabled] = 1
					AND p.Hidden = 0
					AND p.CategoryEnabled = 1
					AND c.Enabled = 1 
					AND (pExt.MaxAvailable > 0 OR @OnlyAvailable = 0)
			GROUP BY pc.CategoryId, p.BrandId
		)

		INSERT INTO @brandIds
		Select Distinct brandCategory.BrandId From brandCategory 
		Inner Join [Settings].[GetChildCategoryByParent](@CategoryId) AS hCat ON hCat.id = brandCategory.[CategoryID]


		Select Brand.BrandID, Brand.BrandName, Brand.UrlPath, Brand.SortOrder, Photo.PhotoName 
		From Catalog.Brand
		Inner Join @brandIds b on b.brandId = Brand.BrandId
		Left Join Catalog.Photo on Photo.ObjId = Brand.BrandID and Type = @Type 
		Where Brand.Enabled = 1
	End
Else
	Begin
		with brandCategory as (
			SELECT p.BrandId,
					pc.CategoryId
			FROM [Catalog].Product p
					INNER JOIN [Catalog].ProductCategories pc ON pc.ProductId = p.ProductID
					INNER JOIN [Catalog].[ProductExt] pExt ON p.ProductID = pExt.ProductID
					INNER JOIN [Catalog].Category c ON c.CategoryID = pc.CategoryId
			WHERE p.BrandId IS NOT NULL               
					AND p.[Enabled] = 1
					AND p.Hidden = 0
					AND p.CategoryEnabled = 1
					AND c.Enabled = 1 
					AND (pExt.MaxAvailable > 0 OR @OnlyAvailable = 0)
			GROUP BY pc.CategoryId, p.BrandId
		)
	
		Select distinct Brand.BrandID, Brand.BrandName, Brand.UrlPath, Brand.SortOrder, Photo.PhotoName 
		From Catalog.Brand 
		Inner Join brandCategory bc on bc.BrandID = brand.BrandID
		Left Join Catalog.Photo on Photo.ObjId = Brand.BrandID and Type = @Type 
		Where Brand.Enabled = 1 and bc.CategoryId = @CategoryId
	End
End

GO--

ALTER PROCEDURE [Catalog].[sp_GetPriceRange]
	@categoryId int,
	@useDepth bit,
	@onlyAvailable bit
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
				
				WHERE Product.Enabled = 1 and product.CategoryEnabled = 1 and (Offer.Amount > 0 or @onlyAvailable = 0)
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

				WHERE Product.Enabled = 1 and product.CategoryEnabled = 1 and (Offer.Amount > 0 or @onlyAvailable = 0) and 
					  ProductCategories.CategoryID in (Select id from [Settings].[GetChildCategoryByParent](@categoryId))
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
		
		WHERE Product.Enabled = 1 and product.CategoryEnabled = 1 and (Offer.Amount > 0 or @onlyAvailable = 0) and 
		      ProductCategories.CategoryID = @categoryId
	end 
end

GO--

Update [Settings].[Localization] Set [ResourceValue] = 'Печать заказа № ' Where [LanguageId] = 1 and [ResourceKey] = 'Checkout.PrintOrder.Title'
Update [Settings].[Localization] Set [ResourceValue] = 'Print order № ' Where [LanguageId] = 2 and [ResourceKey] = 'Checkout.PrintOrder.Title'

GO--


INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ShippingMethods.Common.DefaultProductParametersValue','Значения параметров товара по умолчанию:')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ShippingMethods.Common.DefaultProductParametersValue','Default product parameter values:')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ShippingMethods.Common.DefaultWeight','Вес')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ShippingMethods.Common.DefaultWeight','Weight')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ShippingMethods.Common.ProductWeightWillAssumeValue','Вес товара примет указанное значение, если у товара данный параметр не был задан.')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ShippingMethods.Common.ProductWeightWillAssumeValue','The product''s weight will assume the specified value if the item has not been specified for the product.')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ShippingMethods.Common.ValueIsSpecifiedInKg','Значение указывается в кг, возможно указать дробное значение.')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ShippingMethods.Common.ValueIsSpecifiedInKg','The value is specified in kg, it is possible to specify a fractional value.')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ShippingMethods.Common.ForExample1or02','Например: 1 или 0.2')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ShippingMethods.Common.ForExample1or02','For example: 1 or 0.2')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ShippingMethods.Common.DefaultCargo','Длина, высота, ширина')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ShippingMethods.Common.DefaultCargo','Length, height, width')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ShippingMethods.Common.Length','Длина')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ShippingMethods.Common.Length','Length')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ShippingMethods.Common.Height','Высота')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ShippingMethods.Common.Height','Height')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ShippingMethods.Common.Width','Ширина')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ShippingMethods.Common.Width','Width')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ShippingMethods.Common.LengthOfProductWillTakeSpecifiedValue','Параметр товара примет указанное значение, если у товара данный параметр не был задан.')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ShippingMethods.Common.LengthOfProductWillTakeSpecifiedValue','The parameter of the product will take the specified value if the item has not been specified for the product.')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ShippingMethods.Common.ValueIsIndicatedInMm','Значение указывается в мм.')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ShippingMethods.Common.ValueIsIndicatedInMm','The value is indicated in mm.')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ShippingMethods.Common.ForExample120','Например: 120')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ShippingMethods.Common.ForExample120','For example: 120')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ShippingMethods.Common.CargoExtracharge','Увеличить габариты на')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ShippingMethods.Common.CargoExtracharge','To increase the dimensions')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ShippingMethods.Common.CargoExtrachargeFixedNumber','Фиксированная - это фиксированное число, скажем 100 мм.')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ShippingMethods.Common.CargoExtrachargeFixedNumber','Fixed - is a fixed number, example 100mm.')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ShippingMethods.Common.CargoExtrachargePercentage','Процентная - это процент от габарита заказа, скажем 3%.')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ShippingMethods.Common.CargoExtrachargePercentage','Interest - this is the percentage of the order dimension, example 3%.')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ShippingMethods.Common.WeightExtracharge','Увеличить вес на')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ShippingMethods.Common.WeightExtracharge','To increase the weight')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ShippingMethods.Common.WeightExtrachargeFixedNumber','Фиксированная - это фиксированное число, скажем 1 кг.')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ShippingMethods.Common.WeightExtrachargeFixedNumber','Fixed - is a fixed number, example 1kg.')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ShippingMethods.Common.WeightExtrachargePercentage','Процентная - это процент от веса заказа, скажем 3%.')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ShippingMethods.Common.WeightExtrachargePercentage','Interest - this is the percentage of the order weight, example 3%.')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ShippingMethods.Common.Kg','кг')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ShippingMethods.Common.Kg','kg')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ShippingMethods.Common.Mm','мм')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ShippingMethods.Common.Mm','mm')

GO--

Update [Settings].[Localization] Set [ResourceValue] = 'Задача {{number}} удалена' Where [ResourceKey] = 'Admin.Js.Tasks.Tasks.HasBeenDeleted' and [LanguageId] = 1
Update [Settings].[Localization] Set [ResourceValue] = 'Task {{number}} has been deleted' Where [ResourceKey] = 'Admin.Js.Tasks.Tasks.HasBeenDeleted' and [LanguageId] = 2

Update [Settings].[Localization] Set [ResourceValue] = 'Витрина будет доступна только администратору и модераторам. Чтобы отредактировать текст, перейдите по ссылке (справа внизу)' Where [ResourceKey] = 'Admin.Settings.SystemSettings.IsStoreClosedNote' and [LanguageId] = 1

Update [Settings].[Localization] Set [ResourceValue] = 'Исполнитель к задаче {{number}} изменен' Where [ResourceKey] = 'Admin.Js.Tasks.ModalEditTaskCtrl.ExecutorChanged' and [LanguageId] = 1
Update [Settings].[Localization] Set [ResourceValue] = 'Executor to task {{number}} has been changed' Where [ResourceKey] = 'Admin.Js.Tasks.ModalEditTaskCtrl.ExecutorChanged' and [LanguageId] = 2

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.Tasks.Tasks.CommentToTaskDeleted','Комментарий к задаче {{number}} удален')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.Tasks.Tasks.CommentToTaskDeleted','Comment to task {{number}} has been deleted')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.Tasks.Tasks.TaskHasBeenCopied','Задача {{number}} успешно скопирована')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.Tasks.Tasks.TaskHasBeenCopied','Task {{number}} has been copied')

GO--

alter table [Order].[OrderSource] add ObjId int null
GO--

UPDATE [Order].[OrderSource] SET [Type] = 'LandingPage' WHERE Name LIKE 'Воронка продаж%' AND [Type] = 'None'
GO--

UPDATE [Order].[OrderSource] SET [ObjId] = (select top(1) Id from CMS.LandingSite where [OrderSource].Name = 'Воронка продаж "' + LandingSite.Name + '"') WHERE Name LIKE 'Воронка продаж%'
GO--

INSERT INTO [Order].OrderSource ([Name],[SortOrder],[Main],[Type],[ObjId])
    SELECT SUBSTRING('Воронка продаж "' + LandingSite.Name + '"', 1, 250), 0, 0, 'LandingPage', LandingSite.Id
    FROM CMS.LandingSite LEFT JOIN [Order].OrderSource ON OrderSource.ObjId = LandingSite.Id AND OrderSource.Type = 'LandingPage' WHERE OrderSource.Id IS NULL
GO--

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Воронка продаж' WHERE [ResourceKey] = 'Core.Orders.OrderType.LandingPage' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'Sales funnel' WHERE [ResourceKey] = 'Core.Orders.OrderType.LandingPage' AND [LanguageId] = 2
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.OrderSources.GoToFunnel', 'Перейти к воронке продаж')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.OrderSources.GoToFunnel', 'Go to sales funnel')
UPDATE [Settings].[Localization] SET [ResourceValue] = 'Название' WHERE [ResourceKey] = 'Admin.Js.OrderSources.Name' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'Name' WHERE [ResourceKey] = 'Admin.Js.OrderSources.Name' AND [LanguageId] = 2

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.Tasks.Tasks.CommentToTaskUpdated','Комментарий к задаче {{number}} изменен')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.Tasks.Tasks.CommentToTaskUpdated','Comment to task {{number}} has been changed')

GO--

Update [Settings].[Localization] Set [ResourceValue] = 'Витрина будет доступна только администратору и модераторам. Остальные посетители увидят страницу-заглушку. Отредактировать текст на ней можно в статическом блоке "Витрина закрыта"' Where [ResourceKey] = 'Admin.Settings.SystemSettings.IsStoreClosedNote' and [LanguageId] = 1

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Crm.EOrderFieldType.HasGiftCertificate','Заказ содержит подарочный сертификат')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Crm.EOrderFieldType.HasGiftCertificate','Order with gift certificate')

GO--

CREATE TABLE Vk.VkOrder_Order
	(
	OrderId int NOT NULL,
	VkOrderId int NOT NULL
	)  ON [PRIMARY]
GO--

ALTER TABLE Vk.VkOrder_Order ADD CONSTRAINT
	PK_VkOrder_Order PRIMARY KEY CLUSTERED 
	(
	OrderId,
	VkOrderId
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO--

ALTER TABLE Vk.VkOrder_Order ADD CONSTRAINT
	FK_VkOrder_Order_Order FOREIGN KEY
	(
	OrderId
	) REFERENCES [Order].[Order]
	(
	OrderID
	) ON UPDATE  NO ACTION 
	 ON DELETE  CASCADE 
	
GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.SearchAutocomplete.Products','Товары')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.SearchAutocomplete.Products','Products')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.SearchAutocomplete.Categories','Категории')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.SearchAutocomplete.Categories','Categories')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.SearchAutocomplete.Leads','Лиды')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.SearchAutocomplete.Leads','Leads')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.SearchAutocomplete.Tasks','Задачи')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.SearchAutocomplete.Tasks','Tasks')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.SearchAutocomplete.Modules','Модули')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.SearchAutocomplete.Modules','Modules')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.SearchAutocomplete.Orders','Заказы')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.SearchAutocomplete.Orders','Orders')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.SearchAutocomplete.Settings','Настройки')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.SearchAutocomplete.Settings','Settings')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.SearchAutocomplete.Customers','Покупатели')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.SearchAutocomplete.Customers','Customers')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.SearchAutocomplete.AllResultsFormat','Все результаты ({0})')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.SearchAutocomplete.AllResultsFormat','All results ({0})')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.SearchAutocomplete.ProductsDescFormat','артикул: {0}')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.SearchAutocomplete.ProductsDescFormat','SKU: {0}')

GO--

If exists(Select 1 From [Customers].[City] Where CityName = 'Алма-Ата') and not exists(Select 1 From [Customers].[City] Where CityName = 'Алматы')
	Update [Customers].[City] Set CityName = 'Алматы' Where CityName = 'Алма-Ата'

GO--

If exists(Select 1 From [Customers].[Region] Where RegionName = 'Алма-Ата')
	Update [Customers].[Region] Set RegionName = 'Алматинская область' Where RegionName = 'Алма-Ата'

GO--

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Кнопка "Купить в кредит" будет отображаться у товаров, стоимость которых превышает минимальную цену (необходимо указать цену выше 3000 руб.)' WHERE [ResourceKey] = 'Admin.PaymentMethods.KupiVKredit.ButtonButOnKredit' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'The button "Buy on credit" will be displayed for products with price exceeding the minimum price (you must specify a price above 3000 rubles).' WHERE [ResourceKey] = 'Admin.PaymentMethods.KupiVKredit.ButtonButOnKredit' AND [LanguageId] = 2

GO--

if not exists (Select 1 From CMS.StaticBlock Where [Key] = 'checkout_top')
	Insert Into CMS.StaticBlock ([Key],[InnerName],[Content],[Added],[Modified],[Enabled]) 
	Values ('checkout_top', 'Оформление заказа сверху', '', getdate(), getdate(), 1)

if not exists (Select 1 From CMS.StaticBlock Where [Key] = 'checkout_bottom')
	Insert Into CMS.StaticBlock ([Key],[InnerName],[Content],[Added],[Modified],[Enabled]) 
	Values ('checkout_bottom', 'Оформление заказа снизу', '', getdate(), getdate(), 1)

if not exists (Select 1 From CMS.StaticBlock Where [Key] = 'checkout_after_cart')
	Insert Into CMS.StaticBlock ([Key],[InnerName],[Content],[Added],[Modified],[Enabled]) 
	Values ('checkout_after_cart', 'Оформление заказа под корзиной', '', getdate(), getdate(), 1)


GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.SendSMS.InstallModule','Установить')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.SendSMS.InstallModule','Install')

GO--

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Определяет, как будет отображаться меню каталога</br></br> <a href="https://www.advantshop.net/help/pages/template-settings#12" target="_blank">Инструкция. Стиль отображения меню</a>' WHERE [ResourceKey] = 'Admin.Settings.Template.MenuStyleHint' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'Catalog menu style</br></br> <a href="https://www.advantshop.net/help/pages/template-settings#12" target="_blank">Instruction. Catalog menu style</a>' WHERE [ResourceKey] = 'Admin.Settings.Template.MenuStyleHint' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Активность данной настройки позволяет клиентам добавлять товары в свой список желаний.<br/><br/> <a href="https://www.advantshop.net/help/pages/template-settings#6" target="_blank">Инструкция. Cписок желаний</a>' WHERE [ResourceKey] = 'Admin.Settings.Template.WishListVisibilityHint' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'The activity of this setting allows customers to add products to their wish list.<br/><br/><a href="https://www.advantshop.net/help/pages/template-settings#6" target="_blank">Instruction. Wish List</a>' WHERE [ResourceKey] = 'Admin.Settings.Template.WishListVisibilityHint' AND [LanguageId] = 2

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Catalog.Label.YourDiscount', 'Ваша скидка')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Catalog.Label.YourDiscount', 'Your discount')

GO--

ALTER TABLE [Order].Lead ADD
	Zip nvarchar(70) NULL
GO--

DROP INDEX [ProductDiscountEnabled] ON [Catalog].[Product]
GO--

CREATE NONCLUSTERED INDEX [ProductDiscountEnabled] ON [Catalog].[Product] 
(
	[Enabled] ASC,
	[CategoryEnabled] ASC,
	[Discount] ASC,
	[DiscountAmount] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY]
GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Configuration.TemplateSettings_IconsInCheckout','Иконки на странице оформления заказа')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Configuration.TemplateSettings_IconsInCheckout','Icons on the checkout page')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Configuration.TemplateSettings_SizesImage','Размер изображения')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Configuration.TemplateSettings_SizesImage','Image size')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Configuration.TemplateSettings_DesignSettings','Настройки дизайна')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Configuration.TemplateSettings_DesignSettings','Design settings')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Js.Builder.Settings','Настройки')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Js.Builder.Settings','Settings')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Common.TopMenu.Booking', 'Бронирование ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Common.TopMenu.Booking', 'Reservations ')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Js.Design.ErrorSavingTemplate', 'Ошибка при сохранении настроек шаблона')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Js.Design.ErrorSavingTemplate', 'Error saving template settings')

GO--

UPDATE [Settings].[Settings] SET [Value] = 'True' WHERE [Name] = 'EnablePhoneMask'

GO--

ALTER TABLE CMS.LandingSite ADD
	ScreenShot nvarchar(MAX) NULL
GO--

DELETE FROM [Settings].[SettingsSearch] WHERE Link = 'voting'

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.SystemSettings.UsePhoneMask', 'Использовать маску ввода для телефона')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.SystemSettings.UsePhoneMask', 'Use a phone mask')

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
		AND ((HirecalEnabled = 1 AND Enabled = 1) OR @exportNotAvailable = 1)


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
	WHERE (HirecalEnabled = 1 AND Enabled = 1) OR @exportNotAvailable = 1;

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
		AND (CategoryEnabled = 1 OR @exportNotAvailable = 1)
		AND (p.Enabled = 1 OR @exportNotAvailable = 1)	
		AND (@onlyMainOfferToExport = 0 OR Offer.Main = 1)
	END
	IF @sqlMode = 'GetProducts'
	BEGIN
	with cte as (
		SELECT Distinct tmp.CategoryId
		FROM @lcategorytemp AS tmp
		INNER JOIN CATALOG.Category ON Category.CategoryId = tmp.CategoryId
		WHERE (HirecalEnabled = 1 AND Enabled = 1) OR @exportNotAvailable = 1)
		
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
		AND (CategoryEnabled = 1 OR @exportNotAvailable = 1)
		AND (p.Enabled = 1 OR @exportNotAvailable = 1)	
		AND (@onlyMainOfferToExport = 0 OR Offer.Main = 1)
		Order By p.ProductId
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
		AND (CategoryEnabled = 1 OR @exportNotAvailable = 1)
		AND (p.Enabled = 1 OR @exportNotAvailable = 1)	
		AND (@onlyMainOfferToExport = 0 OR Offer.Main = 1)
	END
END

GO--


UPDATE [Settings].[InternalSettings] SET [settingValue] = '8.5.6' WHERE [settingKey] = 'db_version'