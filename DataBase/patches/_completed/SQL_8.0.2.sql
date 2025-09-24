UPDATE [Settings].[Localization] SET [ResourceValue] = 'Все лиды' WHERE [LanguageId] = '1' AND [ResourceKey] = 'Admin.Leads.CrmNavMenu.ControlLeads'
UPDATE [Settings].[Localization] SET [ResourceValue] = 'All leads' WHERE [LanguageId] = '2' AND [ResourceKey] = 'Admin.Leads.CrmNavMenu.ControlLeads'

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Управление списками' WHERE [LanguageId] = '1' AND [ResourceKey] = 'Admin.Leads.CrmNavMenu.SettingsList'
UPDATE [Settings].[Localization] SET [ResourceValue] = 'Control lists' WHERE [LanguageId] = '2' AND [ResourceKey] = 'Admin.Leads.CrmNavMenu.SettingsList'

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

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Modules.Index.DeleteTitle', 'Удаление модуля')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Modules.Index.DeleteTitle', 'Delete module')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Modules.Index.DeleteText', 'Вы действительно хотите удалить модуль?')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Modules.Index.DeleteText', 'Are you sure you want to delete the module?')

GO--

if exists (Select 1 From [Settings].[Settings] Where Name = 'ActiveBonusSystem' and [Value] = 'True')
begin
	Update [Settings].[Settings] Set [Value] = 'True' Where Name = 'BonusAppActive'
end

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.SystemSettings.ExperimentalFeatures', 'Экспериментальные функции')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.SystemSettings.ExperimentalFeatures', 'Experimental features')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.SystemSettings.EnableExperimentalFeatures', 'Включить экспериментальные функции')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.SystemSettings.EnableExperimentalFeatures', 'Enable experimental features')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Services.Features.EFeature.ProductCategoriesAutoMapping', 'Автоматическое распределение товаров по категориям')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Services.Features.EFeature.ProductCategoriesAutoMapping', 'Products automapping by categories')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Services.Features.EFeature.ProductCategoriesAutoMapping.Description', 'Позволяет настраивать на странице редактирования категории правила переноса/добавления товара в другие категории при его добавлении в редактируемую категорию.')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.Category.AutomapCategoriesDeleted', 'Категории удалены')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.Category.AutomapCategoriesDeleted', 'Categories removed')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.Category.AutomapCategoryDeleted', 'Категория удалена')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.Category.AutomapCategoryDeleted', 'Category removed')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.Category.AutomapCategoriesAdded', 'Категории добавлены')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.Category.AutomapCategoriesAdded', 'Categories added')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Category.CategoryAutomap.AutomapProductsByCategories', 'Распределение товаров по категориям')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Category.CategoryAutomap.AutomapProductsByCategories', 'Products automapping by categories')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Category.CategoryAutomap.AutomapProductsByCategories.Description', 'Вы можете указать правила распределения товаров по категориям при добавлении товара в данную категорию')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Category.CategoryAutomap.AutomapProductsByCategories.Description', 'You can specify the rules for products mapping by categories when adding products to this category')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Category.CategoryAutomap.CopyProducts', 'Копировать товары')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Category.CategoryAutomap.CopyProducts', 'Copy products')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Category.CategoryAutomap.MoveProducts', 'Перемещать товары')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Category.CategoryAutomap.MoveProducts', 'Move products')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Category.CategoryAutomap.Category', 'Категория')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Category.CategoryAutomap.Category', 'Category')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Category.CategoryAutomap.SetMain', 'Назначить основной')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Category.CategoryAutomap.SetMain', 'Set as main')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Category.CategoryAutomap.DontChangeMainCategory', 'Не менять основную категорию товара')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Category.CategoryAutomap.DontChangeMainCategory', 'Don''t change main category')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Category.CategoryAutomap.AddCategories', 'Добавить категории')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Category.CategoryAutomap.AddCategories', 'Add categories')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Category.CategoryAutomap.ClearCategories', 'Очистить список')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Category.CategoryAutomap.ClearCategories', 'Clear categories')
GO--


UPDATE [Settings].[Localization] SET ResourceValue = 'Для отображения этого метода оплаты требуется создание метода доставки типа Edost.ru, СДЭК, Boxberry, Grastin и Shiptor.' WHERE [ResourceKey] = 'Admin.PaymentMethods.CashOnDelivery.NeedToCreateDeliveryMethod' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET ResourceValue = 'To display this payment method, you need to create a delivery method such as Edost.ru, CDEC, Boxberry, Grastin and Shiptor.' WHERE [ResourceKey] = 'Admin.PaymentMethods.CashOnDelivery.NeedToCreateDeliveryMethod' AND [LanguageId] = 2

GO--


CREATE TABLE [Catalog].[CategoriesAutoMapping](
	[CategoryId] [int] NOT NULL,
	[NewCategoryId] [int] NOT NULL,
	[Main] [bit] NOT NULL,
 CONSTRAINT [PK_CategoriesAutoMapping] PRIMARY KEY CLUSTERED 
(
	[CategoryId] ASC,
	[NewCategoryId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO--

ALTER TRIGGER [Catalog].[CategoryDeleted] ON [Catalog].[Category]
WITH EXECUTE AS CALLER
FOR DELETE
AS
BEGIN
	SET NOCOUNT ON;
	DELETE FROM [SEO].[MetaInfo] WHERE [ObjId] in (select CategoryID FROM Deleted) and Type='Category' 
	DELETE FROM [Catalog].[RelatedCategories] WHERE [CategoryId] in (select CategoryID FROM Deleted) or [RelatedCategoryId] in (select CategoryID FROM Deleted)
	DELETE FROM [Catalog].[CategoriesAutoMapping] WHERE [CategoryId] in (select CategoryID FROM Deleted) or [NewCategoryId] in (select CategoryID FROM Deleted)
END
GO--

ALTER TABLE [Catalog].[Category] ADD MoveProductsOnAutomap bit null
GO--
UPDATE [Catalog].[Category] SET MoveProductsOnAutomap = 0
GO--
ALTER TABLE [Catalog].[Category] ALTER COLUMN MoveProductsOnAutomap bit not null
GO--
ALTER TABLE [Catalog].[Category] ADD CONSTRAINT [DF_Category_MoveProductsOnAutomap] DEFAULT ((0)) FOR MoveProductsOnAutomap
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
	@MoveProductsOnAutomap bit
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
		,[MoveProductsOnAutomap]
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
		,@MoveProductsOnAutomap
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
	@MoveProductsOnAutomap bit
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
		,[MoveProductsOnAutomap] = @MoveProductsOnAutomap
	WHERE CategoryID = @CategoryID
END
GO--

If exists (Select 1 From [dbo].[Modules] Where [ModuleStringID] = 'VkMarket' and [Active] = 1) and not exists (Select 1 From [Settings].[Settings] Where [Name] = 'VkChannelActive')
	Insert Into [Settings].[Settings] ([Name],[Value]) Values ('VkChannelActive', 'True')

GO--

UPDATE [Settings].[SettingsSearch] SET [Link] = 'settingscatalog#?catalogTab=properties' WHERE [Title] = 'Свойства товаров'

GO--

Update [Settings].[Localization] Set [ResourceValue] = 'Почта, sms, уведомления'
Where resourceKey = 'Admin.Settings.Commonpage.Settingsmail' and [LanguageId] = 1

Update [Settings].[Localization] Set [ResourceValue] = 'Mail, sms, notifications'
Where resourceKey = 'Admin.Settings.Commonpage.Settingsmail' and [LanguageId] = 2
 
GO--

IF exists (Select 1 From [Settings].[ExportFeed] Where [Type] = 'YandexMarket') and not exists (Select 1 From [Settings].[Settings] Where Name = 'YandexChannelActive')
	Insert Into [Settings].[Settings] ([Name],[Value]) Values ('YandexChannelActive', 'True')

GO--

IF exists (Select 1 From [Settings].[ExportFeed] Where [Type] = 'GoogleMerchentCenter') and not exists (Select 1 From [Settings].[Settings] Where Name = 'GoogleChannelActive')
	Insert Into [Settings].[Settings] ([Name],[Value]) Values ('GoogleChannelActive', 'True')

GO--
	
IF exists (Select 1 From [Settings].[ExportFeed] Where [Type] = 'Avito') and not exists (Select 1 From [Settings].[Settings] Where Name = 'AvitoChannelActive')
	Insert Into [Settings].[Settings] ([Name],[Value]) Values ('AvitoChannelActive', 'True')

GO--

IF exists (Select 1 From [Settings].[ExportFeed] Where [Type] = 'Reseller') and not exists (Select 1 From [Settings].[Settings] Where Name = 'ResellerChannelActive')
	Insert Into [Settings].[Settings] ([Name],[Value]) Values ('ResellerChannelActive', 'True')

GO--
	
IF exists (Select 1 From [Settings].[Settings] Where Name = 'SettingsInstagram.Login' and [Value] is not null and [Value] <> '') and exists (Select 1 From [Settings].[Settings] Where Name = 'SettingsInstagram.Password' and [Value] is not null and [Value] <> '') and not exists (Select 1 From [Settings].[Settings] Where Name = 'InstagramChannelActive')
	Insert Into [Settings].[Settings] ([Name],[Value]) Values ('InstagramChannelActive', 'True')

GO--

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Невозможно удалить список "{0}", указаный как список лидов по умолчанию' WHERE [ResourceKey] = 'Core.Crm.SalesFunnels.Errors.CantDeleteDefaultSalesFunnel' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'Can''t delete list "{0}" specified as the default leads list' WHERE [ResourceKey] = 'Core.Crm.SalesFunnels.Errors.CantDeleteDefaultSalesFunnel' AND [LanguageId] = 2
UPDATE [Settings].[Localization] SET [ResourceValue] = 'Вы не можете удалить список "{0}", пока в нем есть лиды. Перенесите лиды в другой список.' WHERE [ResourceKey] = 'Core.Crm.SalesFunnels.Errors.CantDeleteNotEmptySalesFunnel' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'Can''t delete not empty leads list "{0}". Move leads to another list.' WHERE [ResourceKey] = 'Core.Crm.SalesFunnels.Errors.CantDeleteNotEmptySalesFunnel' AND [LanguageId] = 2

GO--

ALTER PROCEDURE [Catalog].[sp_AddProductToCategoryByExternalId] 
	@ProductID int,
	@ExternalId nvarchar(50),
	@SortOrder int	
AS
BEGIN

DECLARE @Main bit
	SET NOCOUNT ON;
DECLARE @CategoryId int;

SET @CategoryId = (SELECT TOP(1) [CategoryId] FROM [Catalog].Category WHERE [ExternalId] = @ExternalId)
if @CategoryId is not null
begin
	if (select count([CategoryId]) from [Catalog].[ProductCategories] where ProductID=@ProductID and main=1) = 0
		set @Main = 1
	else
		set @Main = 0

	if (select count([CategoryId]) from [Catalog].[ProductCategories] where [CategoryId]=@CategoryId and [ProductId]=@ProductId) = 0 
	begin
		INSERT INTO [Catalog].[ProductCategories] (CategoryID, ProductID, SortOrder, Main) VALUES (@CategoryId, @ProductId, @SortOrder, @Main);
	end
end
END
GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES(1, 'GiftCertificate.Index.WrongNameFrom', 'Некорректное имя отправителя')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES(2, 'GiftCertificate.Index.WrongNameFrom', 'Invalid sender name')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES(1, 'GiftCertificate.Index.WrongNameTo', 'Некорректное имя получателя')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES(2, 'GiftCertificate.Index.WrongNameTo', 'Invalid recipient name')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES(1, 'GiftCertificate.Index.WrongEmailFrom', 'Некорректный email отправителя')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES(2, 'GiftCertificate.Index.WrongEmailFrom','Invalid email sender')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES(1, 'GiftCertificate.Index.WrongEmailTo', 'Некорректный email получателя')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES(2, 'GiftCertificate.Index.WrongEmailTo', 'Invalid email recipient')

GO--

ALTER TABLE Catalog.Offer ADD
	Length float(53) NULL,
	Width float(53) NULL,
	Height float(53) NULL,
	Weight float(53) NULL
GO--

ALTER PROCEDURE [Catalog].[sp_AddOffer]  
   @ArtNo nvarchar(100),  
   @ProductID int,  
   @Amount float,  
   @Price money,  
   @SupplyPrice money,  
   @ColorID int,  
   @SizeID int,  
   @Main bit,  
   @Weight float,  
   @Length float,  
   @Width float,  
   @Height float
AS  
BEGIN  
  
 INSERT INTO [Catalog].[Offer]  
  (  
     ArtNo
   ,[ProductID]  
   ,[Amount]  
   ,[Price]  
   ,[SupplyPrice]  
   ,ColorID  
   ,SizeID  
   ,Main
   ,Weight
   ,Length
   ,Width
   ,Height
  )  
 VALUES  
  (  
    @ArtNo  
   ,@ProductID  
   ,@Amount  
   ,@Price  
   ,@SupplyPrice  
   ,@ColorID  
   ,@SizeID  
   ,@Main
   ,@Weight
   ,@Length
   ,@Width
   ,@Height  
  );  
 SELECT SCOPE_IDENTITY();  
END

GO--

ALTER PROCEDURE [Catalog].[sp_UpdateOffer]  
   @OfferID int,  
   @ProductID int,  
   @ArtNo nvarchar(100),  
   @Amount float,  
   @Price money,  
   @SupplyPrice money,  
   @ColorID int,  
   @SizeID int,  
   @Main bit,
   @Weight float,  
   @Length float,  
   @Width float,  
   @Height float
AS  
BEGIN  
  UPDATE [Catalog].[Offer]  
  SET     
     [ProductID] = @ProductID
	 ,ArtNo=@ArtNo  
     ,[Amount] = @Amount  
     ,[Price] = @Price  
     ,[SupplyPrice] = @SupplyPrice  
     ,[ColorID] = @ColorID  
     ,[SizeID] = @SizeID  
     ,Main = @Main
	 ,Weight = @Weight
	 ,Length = @Length
	 ,Width = @Width
	 ,Height = @Height
  WHERE [OfferID] = @OfferID  
END

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.AddEditOffer.Weight', 'Вес')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.AddEditOffer.Weight', 'Weight')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.AddEditOffer.Kg', 'Кг')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.AddEditOffer.Kg', 'Kg')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.AddEditOffer.Dimensions', 'Габариты')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.AddEditOffer.Dimensions', 'Dimensions')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.AddEditOffer.Width', 'Ширина')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.AddEditOffer.Width', 'Width')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.AddEditOffer.Height', 'Высота')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.AddEditOffer.Height', 'Height')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.AddEditOffer.Length', 'Длина')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.AddEditOffer.Length', 'Length')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.AddEditOffer.Mm', 'мм')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.AddEditOffer.Mm', 'mm')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.Product.Weight', 'Вес (кг)')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.Product.Weight', 'Weight (kg)')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.Product.Dimensions', 'Габариты')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.Product.Dimensions', 'Dimensions')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.Product.DimensionsMm', '(мм)')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.Product.DimensionsMm', '(mm)')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Services.Features.EFeature.OfferWeightAndDimensions', 'Вес и габариты у модификаций')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Services.Features.EFeature.OfferWeightAndDimensions', 'Weight and dimensions for modifications')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Services.Features.EFeature.OfferWeightAndDimensions.Description', 'Позволяет задавать вес и размеры для каждой модификации товара. Если они указаны, то доставка будет рассчитываться по ним.')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Services.Features.EFeature.OfferWeightAndDimensions.Description', 'Allows you to set the weight and dimensions for each product modification. If they are specified, the delivery will be calculated on them.')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ShippingMethods.Boxberry.ApiUrl', 'URL-адрес для API')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ShippingMethods.Boxberry.ApiUrl', 'API URL')

GO--



Update [Catalog].[Offer] 
Set [Weight] = (Select [Weight] From [Catalog].[Product] Where [Product].[ProductId] = [Offer].[ProductId])
Where [Weight] is null

Update [Catalog].[Offer] 
Set [Length] = (Select [Length] From [Catalog].[Product] Where [Product].[ProductId] = [Offer].[ProductId])
Where [Length] is null

Update [Catalog].[Offer] 
Set [Width] = (Select [Width] From [Catalog].[Product] Where [Product].[ProductId] = [Offer].[ProductId])
Where [Width] is null

Update [Catalog].[Offer] 
Set [Height] = (Select [Height] From [Catalog].[Product] Where [Product].[ProductId] = [Offer].[ProductId])
Where [Height] is null

GO--

ALTER PROCEDURE [Catalog].[sp_AddProduct]
    @ArtNo nvarchar(100) = '',
    @Name nvarchar(255),
    @Ratio float,
    @Discount float,
    @DiscountAmount float,
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
    @PaymentSubjectType int,
    @PaymentMethodType int,
    @YandexSizeUnit nvarchar(10),
    @DateModified datetime,
    @YandexName nvarchar(255),
    @YandexDeliveryDays nvarchar(5),
    @CreatedBy nvarchar(50),
    @Hidden bit,
    @ManualRatio float
AS
BEGIN
    Declare @Id int
    INSERT INTO [Catalog].[Product]
        ([ArtNo]
        ,[Name]
        ,[Ratio]
        ,[Discount]
        ,[DiscountAmount]
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
        ,PaymentSubjectType
        ,PaymentMethodType
        ,YandexSizeUnit
        ,YandexName
        ,YandexDeliveryDays
        ,CreatedBy
        ,Hidden
        ,ManualRatio
        )
    VALUES
        (@ArtNo
        ,@Name
        ,@Ratio
        ,@Discount
        ,@DiscountAmount
        ,@BriefDescription
        ,@Description
        ,@Enabled
        ,@DateModified
        ,@DateModified
        ,@Recomended
        ,@New
        ,@BestSeller
        ,@OnSale
        ,@BrandID
        ,@AllowPreOrder
        ,@UrlPath
        ,@Unit
        ,@ShippingPrice
        ,@MinAmount
        ,@MaxAmount
        ,@Multiplicity
        ,@HasMultiOffer
        ,@SalesNote
        ,@GoogleProductCategory
        ,@YandexMarketCategory
        ,@Gtin
        ,@Adult
        ,@CurrencyID
        ,@ActiveView360
        ,@ManufacturerWarranty
        ,@ModifiedBy
        ,@YandexTypePrefix
        ,@YandexModel
        ,@BarCode
        ,@Bid
        ,@AccrueBonuses
        ,@TaxId
        ,@PaymentSubjectType
        ,@PaymentMethodType
        ,@YandexSizeUnit
        ,@YandexName
        ,@YandexDeliveryDays
        ,@CreatedBy
        ,@Hidden
        ,@ManualRatio
        );

    SET @ID = SCOPE_IDENTITY();
    if @ArtNo=''
    begin
        set @ArtNo = Convert(nvarchar(100),@ID)

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
    @ManualRatio float
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
    WHERE ProductID = @ProductID
END

GO--

ALTER PROCEDURE [Settings].[sp_GetCsvProducts] 
    @exportFeedId INT, 
    @onlyCount BIT, 
    @exportNoInCategory BIT, 
    @exportAllProducts BIT, 
    @exportNotAvailable BIT 
AS 
BEGIN
    DECLARE @res TABLE (productid INT PRIMARY KEY CLUSTERED);
    DECLARE @lproductNoCat TABLE (productid INT PRIMARY KEY CLUSTERED);

    IF (@exportNoInCategory = 1)
    BEGIN
        INSERT INTO @lproductNoCat
            SELECT [productid] 
            FROM [Catalog].product 
            WHERE [productid] NOT IN (SELECT [productid] FROM [Catalog].[productcategories]);
    END

    DECLARE @lcategory TABLE (categoryid INT PRIMARY KEY CLUSTERED);
    DECLARE @lcategorytemp TABLE (CategoryId INT);
    DECLARE @l TABLE (categoryid INT PRIMARY KEY CLUSTERED, Opened bit);
    
    INSERT INTO @l
        SELECT t.categoryid, t.Opened
        FROM [Settings].[exportfeedselectedcategories] AS t
            INNER JOIN catalog.category ON t.categoryid = category.categoryid
        WHERE [exportfeedid] = @exportFeedId 

    DECLARE @l1 INT
    SET @l1 = (SELECT Min(categoryid) FROM @l);
    WHILE @l1 IS NOT NULL
    BEGIN 
        if ((Select Opened from @l where CategoryId = @l1) = 1)
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

        SET @l1 = (SELECT Min(categoryid) FROM   @l WHERE  categoryid > @l1); 
    END; 

    INSERT INTO @lcategory
        SELECT Distinct tmp.CategoryId
        FROM @lcategorytemp AS tmp

    IF @onlyCount = 1 
    BEGIN 
        SELECT Count(productid) 
        FROM [Catalog].[product] 
        WHERE 
        (
            EXISTS (
                SELECT 1 FROM [Catalog].[productcategories]
                WHERE [productcategories].[productid] = [product].[productid] 
                AND [productcategories].categoryid IN (SELECT categoryid FROM @lcategory)
            ) OR EXISTS (
                SELECT 1 
                FROM @lproductNoCat AS TEMP
                WHERE  TEMP.productid = [product].[productid]
            ) 
        ) AND (
            @exportAllProducts = 1 
            OR (
                SELECT Count(productid)
                FROM settings.exportfeedexcludedproducts
                WHERE exportfeedexcludedproducts.productid = product.productid AND exportfeedexcludedproducts.exportfeedid = @exportFeedId
            ) = 0
        ) AND (
            Product.Enabled = 1 OR @exportNotAvailable = 1
        ) AND (
            @exportNotAvailable = 1
            OR EXISTS (
                SELECT 1
                FROM [Catalog].[Offer] o
                Where o.[ProductId] = [product].[productid] AND o.Price > 0 and o.Amount > 0
            )
        )
    END
    ELSE
    BEGIN
        SELECT *
        FROM [Catalog].[product]
            LEFT JOIN [Catalog].[photo] ON [photo].[objid] = [product].[productid] AND type = 'Product' AND photo.[main] = 1
        WHERE
        (
            EXISTS (
                SELECT 1
                FROM [Catalog].[productcategories]
                WHERE [productcategories].[productid] = [product].[productid]
                    AND [productcategories].categoryid IN (SELECT categoryid FROM @lcategory)
            ) OR EXISTS (
                SELECT 1
                FROM @lproductNoCat AS TEMP
                WHERE TEMP.productid = [product].[productid]
            )
        ) AND (
            @exportAllProducts = 1
            OR (
                SELECT Count(productid)
                FROM settings.exportfeedexcludedproducts
                WHERE exportfeedexcludedproducts.productid = product.productid AND exportfeedexcludedproducts.exportfeedid = @exportFeedId
            ) = 0
        ) AND (
            Product.Enabled = 1 OR @exportNotAvailable = 1
        ) AND (
            @exportNotAvailable = 1
            OR EXISTS (
                SELECT 1
                FROM [Catalog].[Offer] o
                Where o.[ProductId] = [product].[productid] AND o.Price > 0 and o.Amount > 0
            )
        )
    END
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

IF EXISTS (Select * from dbo.sysobjects where id = object_id(N'[Settings].[sp_GetMailFormatByID]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	DROP PROCEDURE [Settings].[sp_GetMailFormatByID]

GO--

ALTER TABLE Customers.Task 
ADD Reminder int NULL

GO--

UPDATE Customers.Task 
   SET Reminder = 0
 WHERE Reminder is null
 
GO--

ALTER TABLE Customers.Task 
ALTER COLUMN Reminder int NOT NULL
 
GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.SettingsTask.Task.ReminderActive', 'Напоминать о приближении срока исполнения задачи')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.SettingsTask.Task.ReminderActive', 'Remind about tasks due date')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Crm.TaskReminder.NotRemind', 'не напоминать')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Crm.TaskReminder.NotRemind', 'not remind')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Crm.TaskReminder.AtTerm', 'при наступлении срока')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Crm.TaskReminder.AtTerm', 'at term')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Crm.TaskReminder.TenMinutesBefore', 'за 10 минут')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Crm.TaskReminder.TenMinutesBefore', 'ten minutes before')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Crm.TaskReminder.HourBefore', 'за 1 час')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Crm.TaskReminder.HourBefore', 'hour before')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Crm.TaskReminder.ThreeHoursBefore', 'за 3 часа')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Crm.TaskReminder.ThreeHoursBefore', 'three hours before')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Crm.TaskReminder.DayBefore', 'за 1 день')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Crm.TaskReminder.DayBefore', 'day before')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Crm.TaskReminder.ThreeDaysBefore', 'за 3 дня')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Crm.TaskReminder.ThreeDaysBefore', 'three days before')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.Tasks.EditTask.Reminder', 'Напомнить о задаче')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.Tasks.EditTask.Reminder', 'Reminder')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.AddTask.Reminder', 'Напомнить о задаче')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.AddTask.Reminder', 'Reminder')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Services.CMS.OnTaskReminderNotification.Title', 'Напоминание о задаче №{0}')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Services.CMS.OnTaskReminderNotification.Title', 'Reminder of task №{0}')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Services.CMS.OnTaskReminderNotification.Body', 'Время на выполнение задачи {0}-{1} подходит к концу. Крайний срок - {2}')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Services.CMS.OnTaskReminderNotification.Body', 'Time to complete the task {0}-{1} is coming to the end. Deadline - {2}')

GO--

INSERT INTO [Settings].[Settings] ([Name],[Value]) VALUES ('ReminderActive','False')

GO--

IF NOT EXISTS(SELECT * FROM [Settings].[MailFormatType] WHERE [MailType] = 'OnTaskReminder')
INSERT INTO [Settings].[MailFormatType] ([TypeName], [SortOrder], [Comment], [MailType]) 
VALUES ('Напоминание о задаче', 400, 
	'Письмо при напоминании о задаче  (#TASK_ID#,#TASK_PROJECT#,#MANAGER_NAME#,#APPOINTEDMANAGER#,#TASK_NAME#,#TASK_DESCRIPTION#,#TASK_STATUS#,#TASK_PRIORITY#,#DUEDATE#,#DATE_CREATED#,#TASK_URL#,#TASK_ATTACHMENTS#)', 
	'OnTaskReminder')
	
GO--

IF NOT EXISTS(SELECT * FROM [Settings].[MailFormat] WHERE [MailFormatTypeId] = (SELECT TOP(1) [MailFormatTypeID] FROM [Settings].[MailFormatType] WHERE [MailType] = 'OnTaskReminder'))
INSERT INTO [Settings].[MailFormat] ([FormatName],[FormatText],[SortOrder],[Enable],[AddDate],[ModifyDate],[FormatSubject],[MailFormatTypeId])
Values ('Напоминание о задаче', '<div style="color: #4c4f56; font-family: Arial, Helvetica, sans-serif; font-size: 14px;">
<div class="header" style="border-bottom: 1px solid #ededed; display: table; margin-bottom: 25px; padding-bottom: 25px; width: 100%;">
<div class="logo" style="display: table-cell; text-align: left; vertical-align: middle;">#LOGO#</div>
<div class="phone" style="display: table-cell; text-align: right; vertical-align: middle;">
<div class="tel" style="font-size: 26px; font-weight: bold; line-height: 1; margin-bottom: 5px;">&nbsp;</div>
<div class="inform" style="font-size: 12px;">&nbsp;</div>
</div>
</div>
<div>
<p>Напоминание о задаче #TASK_PROJECT#-#TASK_ID#.</p>
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
</div>', 1600, 1, GETDATE(), GETDATE(), 'Напоминание о задаче #TASK_PROJECT#-#TASK_ID#', (SELECT TOP(1) [MailFormatTypeID] FROM [Settings].[MailFormatType] WHERE [MailType] = 'OnTaskReminder'))

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Index.MobilePhoneStandard', 'Основной телефон в числовом формате')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Index.MobilePhoneStandard', 'Main phone number in numeric format')

GO--

ALTER TABLE [Catalog].[Coupon] 
ADD [StartDate] datetime NULL

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.AddEditCoupon.FromCreation', 'С даты создания купона')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.AddEditCoupon.FromCreation', 'From the date of creation')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.AddEditCoupon.StartDate', 'Дата начала действия')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.AddEditCoupon.StartDate', 'Start of coupon validity')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.Coupons.StartDate', 'Дата начала действия')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.Coupons.StartDate', 'Start of coupon validity')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Coupons.NoStartDate', 'С даты создания')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Coupons.NoStartDate', 'From the date of creation')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Service.DomainsManage', 'Управление доменами')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Service.DomainsManage', 'Domain management')

GO--

DELETE FROM [CMS].[StaticBlock] WHERE [Key] = 'FashionStyleBanners'
DELETE FROM [CMS].[StaticBlock] WHERE [Key] = 'dressCollectionBanners'
DELETE FROM [CMS].[StaticBlock] WHERE [Key] = 'SearchBottom'
DELETE FROM [CMS].[StaticBlock] WHERE [Key] = 'SalesInProductListSale'
DELETE FROM [CMS].[StaticBlock] WHERE [Key] = 'SocialLogo'
DELETE FROM [CMS].[StaticBlock] WHERE [Key] = 'Блок под слайдером'
DELETE FROM [CMS].[StaticBlock] WHERE [Key] = 'feedbackAddons'
DELETE FROM [CMS].[StaticBlock] WHERE [Key] = 'static-temp'
DELETE FROM [CMS].[StaticBlock] WHERE [Key] = 'mobile_footer_socialbuttons'

UPDATE [CMS].[StaticBlock] SET [Key] = 'BlockInNews', [InnerName] = 'Блок в новостях' WHERE [Key] = 'TwitterInNews'

GO--

ALTER TABLE [Order].[Order] ADD PayCode nvarchar(20) NULL
GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.SendSMS.ShortBillingLink', 'Короткая ссылка на оплату')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.SendSMS.ShortBillingLink', 'Short billing link')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.GetBillingLink.ShortLink', 'Короткая ссылка на оплату')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.GetBillingLink.ShortLink', 'Short billing link')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.GetBillingLink.GenerateShortLink', 'Сгенерировать')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.GetBillingLink.GenerateShortLink', 'Generate')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.GetBillingLink.ShortLinkGenerated', 'Короткая ссылка на оплату сгенерирована')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.GetBillingLink.ShortLinkGenerated', 'Short billing link successfully generated')
GO--

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Settings].[SmsTemplate]') AND type in (N'U'))
BEGIN
	CREATE TABLE [Settings].[SmsTemplate](
		[TemplateId] [int] IDENTITY(1,1) NOT NULL,
		[Name] [nvarchar](250) NOT NULL,
		[Text] [nvarchar](max) NOT NULL,
		[SortOrder] [int] NOT NULL,
		[Active] [bit] NOT NULL,
	 CONSTRAINT [PK_SmsTemplate] PRIMARY KEY CLUSTERED 
	(
		[TemplateId] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
	) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.NotifyEMails.SmsAnswerTemplates', 'Смс шаблоны ответов')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.NotifyEMails.SmsAnswerTemplates', 'Sms response templates')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.AddEditSmsAnswerTemplate.Add','Добавление шаблона смс ответа')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.AddEditSmsAnswerTemplate.Add','Adding a reply sms template')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.AddEditSmsAnswerTemplate.Edit','Редактирование шаблона смс ответа')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.AddEditSmsAnswerTemplate.Edit','Editing the response sms template')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.AddEditSmsAnswerTemplate.Name','Название')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.AddEditSmsAnswerTemplate.Name','Name')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.AddEditSmsAnswerTemplate.Active','Активен')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.AddEditSmsAnswerTemplate.Active','Active')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.AddEditSmsAnswerTemplate.SmsText','Текст смс')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.AddEditSmsAnswerTemplate.SmsText','Text of the sms')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.AddEditSmsAnswerTemplate.Template','Шаблон смс ответа (#FIRST_NAME#, #LAST_NAME#, #FULL_NAME#) (#ORDER_NUMBER#, #ORDER_SUM#, #ORDER_STATUS#, #STATUS_COMMENT#, #PAYMENT_NAME#, #SHIPPING_NAME#, #PICKPOINT_ADDRESS#, #TRACKNUMBER#, #PAY_STATUS#, #STORE_NAME#) (#TITLE#, #LEAD_SUM#, #SALES_FUNNEL#, #DEAL_STATUS#, #SHIPPING_NAME#, #PICKPOINT_ADDRESS#, #STORE_NAME#)')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.AddEditSmsAnswerTemplate.Template','Response sms template (#FIRST_NAME#, #LAST_NAME#, #FULL_NAME#) (#ORDER_NUMBER#, #ORDER_SUM#, #ORDER_STATUS#, #STATUS_COMMENT#, #PAYMENT_NAME#, #SHIPPING_NAME#, #PICKPOINT_ADDRESS#, #TRACKNUMBER#, #PAY_STATUS#, #STORE_NAME#) (#TITLE#, #LEAD_SUM#, #SALES_FUNNEL#, #DEAL_STATUS#, #SHIPPING_NAME#, #PICKPOINT_ADDRESS#, #STORE_NAME#)')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.AddEditSmsAnswerTemplate.Sorting','Сортировка')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.AddEditSmsAnswerTemplate.Sorting','Sorting')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.SendSMS.SmsTemplate','Шаблон смс')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.SendSMS.SmsTemplate','Sms Template')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.SendSMS.Empty','Пустой')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.SendSMS.Empty','Empty')

GO--

ALTER TABLE Customers.Task 
ADD Reminded bit NULL

GO--

UPDATE Customers.Task 
   SET Reminded = 0
 WHERE Reminded is null

GO--

ALTER TABLE Customers.Task 
ALTER COLUMN Reminded bit NOT NULL
 
GO--

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Ошибка' WHERE [LanguageId] = '1' AND [ResourceKey] = 'Admin.Js.SettingsCatalog.Error'
UPDATE [Settings].[Localization] SET [ResourceValue] = 'Error' WHERE [LanguageId] = '2' AND [ResourceKey] = 'Admin.Js.SettingsCatalog.Error'

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.Catalog.AddPropertyToProducts','Добавить товарам свойство')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.Catalog.AddPropertyToProducts','Add property to products')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.AddPropertyToProducts.PropertyAddedSuccessfully','Свойство успешно добавлены')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.AddPropertyToProducts.PropertyAddedSuccessfully','Property added successfully')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.AddPropertyToProducts.Error','Добавить свойство не удалось')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.AddPropertyToProducts.Error','Failed to add property')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.AddPropertyToProducts.AddingProperty','Добавление свойства к товарам')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.AddPropertyToProducts.AddingProperty','Adding a property to a product')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.AddPropertyToProducts.PropertyName','Название свойства')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.AddPropertyToProducts.PropertyName','Name')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.AddPropertyToProducts.EnterPropertyName','Введите название свойства')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.AddPropertyToProducts.EnterPropertyName','Enter property name')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.AddPropertyToProducts.Properties','Свойства')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.AddPropertyToProducts.Properties','Properties')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.AddPropertyToProducts.EnterPropertyValue','Введите значение свойства')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.AddPropertyToProducts.EnterPropertyValue','Enter property value')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.AddPropertyToProducts.Add','Добавить свойство')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.AddPropertyToProducts.Add','Add property')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.AddPropertyToProducts.Cancel','Отмена')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.AddPropertyToProducts.Cancel','Cancel')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.Catalog.AddTagsToProducts','Добавить товарам теги')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.Catalog.AddTagsToProducts','Add tags to products')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.AddTagsToProducts.TagsAddedSuccessfully','Теги успешно добавлены')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.AddTagsToProducts.TagsAddedSuccessfully','Tags added successfully')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.AddTagsToProducts.Error','Добавить теги не удалось')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.AddTagsToProducts.Error','Failed to add tags')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.AddTagsToProducts.AddingTags','Добавление тегов к товарам')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.AddTagsToProducts.AddingTags','Adding tags to products')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.AddTagsToProducts.SelectTags','Выберите теги')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.AddTagsToProducts.SelectTags','Select tags')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.AddTagsToProducts.Add','Добавить теги')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.AddTagsToProducts.Add','Add tags')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.AddTagsToProducts.Cancel','Отмена')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.AddTagsToProducts.Cancel','Cancel')

GO--

If Exists (Select 1 From [Order].[OrderSource] Where [Type] = 'LandingPage' and Name = 'Посадочная страница')
	Update [Order].[OrderSource] Set Name = 'Воронка продаж' Where [Type] = 'LandingPage' and Name = 'Посадочная страница'

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Landings.LeadSourceName.Funnel','Воронка продаж')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Landings.LeadSourceName.Funnel','Sales funnel')

GO--


INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Js.CatalogFilter.Empty', 'Нет данных для фильтра')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Js.CatalogFilter.Empty', 'No data to filter')

GO--

UPDATE [Settings].[Localization] SET [ResourceKey] = 'Admin.Js.TelephonyCallLog.Title' WHERE [ResourceKey] = 'Admin.Calls.Index.Title'
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Telephony.CallLog', 'Журнал вызовов')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Telephony.CallLog', 'Call log')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Telephony.CallAnalytics', 'Аналитика вызовов')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Telephony.CallAnalytics', 'Call analytics')

GO--

UPDATE [Settings].[Localization] SET [ResourceKey] = 'Admin.Settings.Template.BrandsPerPage' WHERE [ResourceKey] = 'Admin.Settings.Catalog.BrandsPerPage'
UPDATE [Settings].[Localization] SET [ResourceKey] = 'Admin.Settings.Template.ShowProductsInBrand' WHERE [ResourceKey] = 'Admin.Settings.Catalog.ShowProductsInBrand'
UPDATE [Settings].[Localization] SET [ResourceKey] = 'Admin.Settings.Template.ShowCategoryTreeInBrand' WHERE [ResourceKey] = 'Admin.Settings.Catalog.ShowCategoryTreeInBrand'

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Template.Brands', 'Производители')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Template.Brands', 'Brands')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Template.News', 'Новости')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Template.News', 'News')



UPDATE [Settings].[InternalSettings] SET [settingValue] = '8.0.2' WHERE [settingKey] = 'db_version'
