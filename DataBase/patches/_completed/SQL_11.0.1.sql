INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
    VALUES
           (1,'Core.ExportImport.LeadFields.Currency', 'Валюта'),
           (2,'Core.ExportImport.LeadFields.Currency', 'Currency'),
           (1,'Core.ExportImport.LeadFields.Discount', 'Скидка процент'),
           (2,'Core.ExportImport.LeadFields.Discount', 'Discount percent'),
           (1,'Core.ExportImport.LeadFields.DiscountValue', 'Скидка число'),
           (2,'Core.ExportImport.LeadFields.DiscountValue', 'Discount value'),
           (1,'Core.ExportImport.LeadFields.ShippingName', 'Доставка'),
           (2,'Core.ExportImport.LeadFields.ShippingName', 'Shipping name'),
           (1,'Core.ExportImport.LeadFields.ShippingCost', 'Стоимость доставки'),
           (2,'Core.ExportImport.LeadFields.ShippingCost', 'Shipping cost'),
           (1,'Core.ExportImport.LeadFields.TotalSum', 'Итоговая стоимость'),
           (2,'Core.ExportImport.LeadFields.TotalSum', 'Sum')
GO--

Update [Settings].[Localization] 
Set [ResourceValue] = 'Список лидов' 
Where [ResourceKey] = 'Core.ExportImport.LeadFields.SalesFunnel' and [LanguageId] = 1

GO--

Update [Settings].[Localization] 
Set [ResourceValue] = 'Leads list' 
Where [ResourceKey] = 'Core.ExportImport.LeadFields.SalesFunnel' and [LanguageId] = 2

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
    VALUES
           (1,'Core.PriceRule.PriceRulesLimit', 'На вашем тарифном плане доступно {0} типов цен'),
           (2,'Core.PriceRule.PriceRulesLimit', 'There are {0} price types available on your tariff plan')

GO--


INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
    VALUES
           (1,'Admin.Js.AddTask.AddAndSee', 'Добавить и просмотреть'),
           (2,'Admin.Js.AddTask.AddAndSee', 'Add and see')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
    VALUES
           (1,'Admin.Settings.SystemSettings.ShowUserAgreementForPromotionalNewsletter', 'Показывать cогласие на получение рассылок'),
           (2,'Admin.Settings.SystemSettings.ShowUserAgreementForPromotionalNewsletter', 'Show the user agreement for subscribing to promotional news'),
           (1,'Admin.Settings.SystemSettings.ShowUserAgreementForPromotionalNewsletterNote', 'Запрашивать подтверждение cогласия на получение рассылок. <br><br> Подробнее: <br> <a href="https://www.advantshop.net/help/pages/152-fz " target="_blank">Как соблюсти требования закона 152-ФЗ на платформе AdvantShop</a>'),
           (2,'Admin.Settings.SystemSettings.ShowUserAgreementForPromotionalNewsletterNote', 'Request confirmation of consent to the terms of the user agreement for promotional news. <br><br><a href="https://www.advantshop.net/help/pages/152-fz " target="_blank">More</a>'),
           (1,'Admin.Settings.SystemSettings.UserAgreementForPromotionalNewsletter', 'Согласие на получение рассылок'),
           (2,'Admin.Settings.SystemSettings.UserAgreementForPromotionalNewsletter', 'User agreement for subscription to promotional news'),
           (1,'Admin.Settings.SystemSettings.SetUserAgreementForPromotionalNewsletterChecked', 'Согласие принято по умолчанию'),
           (2,'Admin.Settings.SystemSettings.SetUserAgreementForPromotionalNewsletterChecked', 'User agreement is accepted by default'),
           (1,'Admin.Settings.SystemSettings.SetUserAgreementForPromotionalNewsletterCheckedHint', 'Если настройка включена, то пользовательское соглашение будет принято автоматически'),
           (2,'Admin.Settings.SystemSettings.SetUserAgreementForPromotionalNewsletterCheckedHint', 'If the setting is enabled, the user agreement will be accepted automatically')
GO--

If not exists (Select 1 From [Settings].[Settings] Where [Name] = 'UserAgreementForPromotionalNewsletter')
	Insert Into [Settings].[Settings] (Name, [Value]) Values('UserAgreementForPromotionalNewsletter', 'Даю согласие на получение рассылки рекламно-информационного характера.')
else
    Update [Settings].[Settings] 
    Set [Value] = 'Даю согласие на получение рассылки рекламно-информационного характера.' 
    Where [Name] = 'UserAgreementForPromotionalNewsletter'
	
GO--

If not exists (Select 1 From [Settings].[Settings] Where [Name] = 'SetUserAgreementForPromotionalNewsletterChecked')
	Insert Into [Settings].[Settings] (Name, [Value]) Values('SetUserAgreementForPromotionalNewsletterChecked', 'True')

GO--

ALTER TABLE Customers.Customer ADD
	IsAgreeForPromotionalNewsletter bit NULL
GO--

Update Customers.Customer Set IsAgreeForPromotionalNewsletter = 1 Where IsAgreeForPromotionalNewsletter is null

GO--

ALTER TABLE Customers.Customer ALTER COLUMN [IsAgreeForPromotionalNewsletter] bit NOT NULL

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
    @ClientStatus int,
    @RegisteredFrom nvarchar(500),
	@CustomerType int,
    @IsAgreeForPromotionalNewsletter bit
AS
BEGIN
    IF @CustomerID IS NULL
        SET @CustomerID = NEWID()

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
        ,[ClientStatus]
        ,[RegisteredFrom]
		,[CustomerType]
        ,[IsAgreeForPromotionalNewsletter])
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
        ,@ClientStatus
        ,@RegisteredFrom
		,@CustomerType
        ,@IsAgreeForPromotionalNewsletter);

    SELECT CustomerID, InnerId From [Customers].[Customer] WHERE CustomerId = @CustomerID
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
    @ClientStatus int,
    @CustomerType int,
    @IsAgreeForPromotionalNewsletter bit
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
  [ClientStatus] = @ClientStatus,
  [CustomerType] = @CustomerType,
  [IsAgreeForPromotionalNewsletter] = @IsAgreeForPromotionalNewsletter
 WHERE customerid = @customerid    
END 

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
    VALUES
           (1,'Admin.Customers.RightBlock.IsAgreeForPromotionalNewsletter', 'Согласие на получение рассылок'),
           (2,'Admin.Customers.RightBlock.IsAgreeForPromotionalNewsletter', 'User agreement for subscription to promotional news'),
           (1,'Core.Customers.Customer.IsAgreeForPromotionalNewsletter', 'Согласие на получение рассылок'),
           (2,'Core.Customers.Customer.IsAgreeForPromotionalNewsletter', 'User agreement for subscription to promotional news')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
    VALUES
           (1,'Admin.Shared.ForgotPassword', 'Восстановление пароля'),
           (2,'Admin.Shared.ForgotPassword', 'Forgot password')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
    VALUES
           (1,'Common.ClosedStore.Title', 'Ведутся работы'),
           (2,'Common.ClosedStore.Title', 'Work is underway')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
    VALUES
           (1,'Admin.Js.OrderItemsSummary.UpdateDraftOrderSberlogistic', 'Обновить черновик заказа в Сберлогистике'),
           (2,'Admin.Js.OrderItemsSummary.UpdateDraftOrderSberlogistic', 'Update draft order Sberlogistic')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
    VALUES
           (1,'Admin.Js.OrderItemsSummary.CancelDraftOrderSberlogistic', 'Удалить черновик заказа в Сберлогистике'),
           (2,'Admin.Js.OrderItemsSummary.CancelDraftOrderSberlogistic', 'Delete draft order Sberlogistic')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
    VALUES 
        (1, 'InvalidFileExtension', N'Недопустимый формат файла'),
        (2, 'InvalidFileExtension', N'Invalid file format');
    
GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
    VALUES
           (1,'Core.ExportImport.CustomerFields.IsAgreeForPromotionalNewsletter', 'Согласие на получение рассылок'),
           (2,'Core.ExportImport.CustomerFields.IsAgreeForPromotionalNewsletter', 'User agreement for subscription to promotional news'),
           (1,'MyAccount.CommonInfo.UserAgreementForPromotionalNewsletter', 'Согласие на получение рассылок'),
           (2,'MyAccount.CommonInfo.UserAgreementForPromotionalNewsletter', 'User agreement for subscription to promotional news'),
           (1,'Admin.Js.UserNotAgreeForNewsletter', 'Пользователь не давал согласия на рассылки'),
           (2,'Admin.Js.UserNotAgreeForNewsletter', 'The user did not consent to the mailing list')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
    VALUES
           (1,'Admin.Js.Customers.Country', 'Страна'),
           (2,'Admin.Js.Customers.Country', 'Country')

GO--

DECLARE @content NVARCHAR(MAX);
DECLARE @enabled bit;
SET @content = REPLACE(
				(SELECT Content FROM [CMS].[StaticBlock] WHERE [Key] = 'liveoperator'),
					'<script language="JavaScript" id="clScript" type="text/javascript" src="http://live09.liveoperator.ru:8080/chat/cl.php?site=10401"></script>',
					'');
SET @enabled = (SELECT [Enabled] FROM [CMS].[StaticBlock] WHERE [Key] = 'liveoperator');

IF ((NOT EXISTS (SELECT 1 FROM [CMS].[StaticBlock] WHERE [Key] = 'body_end')))
	INSERT INTO [CMS].[StaticBlock] ([Key], [InnerName], [Content], [Added], [Modified], [Enabled]) 
		VALUES ('body_end', 'Конец страницы', @content, GETDATE(), GETDATE(), @enabled)
ELSE IF (@content <> NULL)
	UPDATE [CMS].[StaticBlock] SET Content = Content + @content, Modified = GETDATE() WHERE [Key] = 'body_end'
	
DELETE FROM [CMS].[StaticBlock] WHERE [Key] = 'liveoperator'

GO--

DELETE FROM [Settings].[Settings] WHERE [Name] = 'Achievements'
DELETE FROM [Settings].[Settings] WHERE [Name] = 'AchievementsPoints'
DELETE FROM [Settings].[Settings] WHERE [Name] = 'AchievementsDescription'
DELETE FROM [Settings].[Settings] WHERE [Name] = 'AchievementsPopUp'

GO--

UPDATE [Settings].[Localization] SET [ResourceValue] = N'Как у товара' WHERE [ResourceKey] = 'Admin.Js.CatProductRecommendations.Same' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'The same as for the goods' WHERE [ResourceKey] = 'Admin.Js.CatProductRecommendations.Same' AND [LanguageId] = 2

GO--

ALTER TABLE [Catalog].[ProductExt] 
	DROP COLUMN CategoryId

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
    VALUES
           (1,'Admin.ShippingMethods.Sdek.SelectTariffs', 'Выберите тарифы'),
           (2,'Admin.ShippingMethods.Sdek.SelectTariffs', 'Select tariffs')



GO--

SET IDENTITY_INSERT [Order].[ShippingReplaceGeo] ON 


IF NOT EXISTS (SELECT * FROM [Order].[ShippingReplaceGeo] WHERE [Id] = 48)
INSERT INTO [Order].[ShippingReplaceGeo] ([Id],[ShippingType],[InCountryName],[InCountryISO2],[InRegionName],[InCityName],[InDistrict],[OutCountryName],[OutRegionName],[OutCityName],[OutDistrict],[OutDistrictClear],[Enabled],[Sort],[InZip],[OutZip],[Comment])
VALUES (48,'Sdek','','RU','Карачаево-Черкесская Республика','','','','Карачаево-Черкесия','','',0,1,0,'','','')



SET IDENTITY_INSERT [Order].[ShippingReplaceGeo] OFF

GO--

UPDATE [Settings].[SettingsSearch] SET [KeyWords] = 'Выгрузка, csv, excel, yml' WHERE Link = 'exportfeeds/indexcsv?exportTab=exportProducts'
UPDATE [Settings].[SettingsSearch] SET [Link] = 'modules/preview/yamarketbuying', [KeyWords] = 'yandex market' WHERE Link = 'exportfeeds/indexyandex'


UPDATE [Settings].[Localization] 
SET [ResourceValue] = 'Яндекс Маркет (yml, xml)' 
WHERE [ResourceKey] = 'Core.ExportImport.ExportFeed.YandexType' and [LanguageId] = 1


UPDATE [Settings].[Localization] 
SET [ResourceValue] = 'Yandex Market (yml, xml)' 
WHERE [ResourceKey] = 'Core.ExportImport.ExportFeed.YandexType' and [LanguageId] = 2

GO--

IF NOT EXISTS(SELECT 1 FROM sys.columns 
          WHERE Name = N'SelfDelivery'
          AND Object_ID = Object_ID(N'Shipping.PickPointPostamats'))
BEGIN
	ALTER TABLE Shipping.PickPointPostamats ADD
		[SelfDelivery] [bit] NULL
END

GO--
	

CREATE TABLE [Settings].[ExportFeedCategoriesCache](
	[ExportFeedId] [int] NOT NULL,
	[CategoryID] [int] NOT NULL,
 CONSTRAINT [PK_ExportFeedCategoriesCache] PRIMARY KEY CLUSTERED 
(
	[ExportFeedId] ASC,
	[CategoryID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]

GO--

ALTER TABLE [Settings].[ExportFeedCategoriesCache]  WITH CHECK ADD  CONSTRAINT [FK_ExportFeedCategoriesCache_Category] FOREIGN KEY([CategoryID])
REFERENCES [Catalog].[Category] ([CategoryID])
ON UPDATE CASCADE
ON DELETE CASCADE

GO--

ALTER TABLE [Settings].[ExportFeedCategoriesCache]  WITH CHECK ADD  CONSTRAINT [FK_ExportFeedCategoriesCache_ExportFeed] FOREIGN KEY([ExportFeedId])
REFERENCES [Settings].[ExportFeed] ([Id])
ON UPDATE CASCADE
ON DELETE CASCADE

GO--

CREATE PROCEDURE [Settings].[sp_FillExportFeedCategoriesCache]
	@ExportFeedId int,
	@exportNotAvailable bit
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @selCategoryTemp TABLE (CategoryId INT);
	DECLARE @feedCategories TABLE (CategoryId INT PRIMARY KEY CLUSTERED, Opened bit);

	INSERT INTO @feedCategories
		SELECT t.CategoryId, t.Opened
		FROM [Settings].[ExportFeedSelectedCategories] AS t
			INNER JOIN Catalog.Category ON t.CategoryId = Category.CategoryId
		WHERE [ExportFeedId] = @exportFeedId
			AND (@exportNotAvailable = 1 OR (HirecalEnabled = 1 AND Enabled = 1))
			ORDER BY t.CategoryId

	DECLARE @categoryId INT
	SET @categoryId = (SELECT TOP 1 CategoryId FROM @feedCategories);

	WHILE @categoryId IS NOT NULL
	BEGIN

		if ((Select Opened from @feedCategories where CategoryId = @categoryId) = 1)
		begin
			INSERT INTO @selCategoryTemp
			SELECT @categoryId
		end
		else
		begin
	 		INSERT INTO @selCategoryTemp
			SELECT id FROM Settings.GetChildCategoryByParent(@categoryId)
		end

		SET @categoryId = (SELECT TOP 1 CategoryId FROM @feedCategories WHERE CategoryId > @categoryId);
	END;

	INSERT INTO Settings.ExportFeedCategoriesCache (ExportFeedId, CategoryId)
		SELECT DISTINCT @ExportFeedId, tmp.CategoryId FROM @selCategoryTemp AS tmp
			INNER JOIN Catalog.Category ON Category.CategoryId = tmp.CategoryId
		WHERE @exportNotAvailable = 1 OR (HirecalEnabled = 1 AND Enabled = 1)
END

GO--


CREATE NONCLUSTERED INDEX [ProductExOp_Adult] ON [Catalog].ProductExportOptions
(
	Adult ASC
)
INCLUDE([ProductId])

GO--


If not exists (Select 1 From [Settings].[Settings] Where [Name] = 'ExportOffersBatchSize')
	Insert Into [Settings].[Settings] (Name, [Value]) Values('ExportOffersBatchSize', '500')
	
If not exists (Select 1 From [Settings].[Settings] Where [Name] = 'ExportProductsBatchSize')
	Insert Into [Settings].[Settings] (Name, [Value]) Values('ExportProductsBatchSize', '500')
	
GO--

UPDATE [Settings].[Localization] SET [ResourceValue] = N'Экспорт проданных товаров' WHERE [ResourceKey] = 'Admin.Home.Menu.StatisticExportProducts' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'Export products sold' WHERE [ResourceKey] = 'Admin.Home.Menu.StatisticExportProducts' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = N'Экспорт проданных товаров' WHERE [ResourceKey] = 'Admin.Js.ExportProducts.Title' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'Export products sold' WHERE [ResourceKey] = 'Admin.Js.ExportProducts.Title' AND [LanguageId] = 2

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
    VALUES
           (1,'Admin.Js.ExportProducts.AllProducts', 'Все товары'),
           (2,'Admin.Js.ExportProducts.AllProducts', 'All products'), 
           (1,'Admin.Js.Analytics.ExportProducts.SelectProduct', 'Выбрать товар'),
           (2,'Admin.Js.Analytics.ExportProducts.SelectProduct', 'Select product'), 
           (1,'Admin.Js.ExportProducts.ProductNotSelected', 'Товар не выбран'),
           (2,'Admin.Js.ExportProducts.ProductNotSelected', 'Product not selected'),
           (1,'Admin.Js.ExportProducts.CategoryNotSelected', 'Категория не выбрана'),
           (2,'Admin.Js.ExportProducts.CategoryNotSelected', 'Category not selected')
 
GO--

IF ((SELECT COUNT(*) FROM [Customers].[Region] WHERE [RegionName] = 'Атырауская область') = 2
	AND (SELECT COUNT(*) FROM [Customers].[City]
  WHERE [RegionID] IN (SELECT [RegionID] FROM [Customers].[Region] WHERE [RegionName] = 'Атырауская область')) = 2)
BEGIN
	DELETE TOP (1)
	FROM [Customers].[Region]
	WHERE [RegionName] = 'Атырауская область'
END

GO--

IF ((SELECT COUNT(*) FROM [Customers].[Region] WHERE [RegionName] = 'Северо-Казахстанская область') = 2
	AND (SELECT COUNT(*) FROM [Customers].[City]
  WHERE [RegionID] IN (SELECT [RegionID] FROM [Customers].[Region] WHERE [RegionName] = 'Северо-Казахстанская область')) = 2)
BEGIN
	DELETE TOP (1)
	FROM [Customers].[Region]
	WHERE [RegionName] = 'Северо-Казахстанская область'
END

GO--
 
UPDATE [Settings].[Localization] SET [ResourceValue] = N'Себестоимость' WHERE [ResourceKey] = 'Core.ExportImport.MultiOrder.Cost' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = N'Прибыль' WHERE [ResourceKey] = 'Core.ExportImport.MultiOrder.Profit' AND [LanguageId] = 1

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
    VALUES
           (1,'Admin.Js.ExportOrders.BonusCost', N'Выгрузить заказы с бонусами'),
           (2,'Admin.Js.ExportOrders.BonusCost', 'Export orders with bonuses')
 
GO--

ALTER TABLE [Order].[Order] ADD
	LinkedCustomerId uniqueidentifier NULL
GO--


ALTER PROCEDURE [Settings].[sp_FillExportFeedCategoriesCache]
	@ExportFeedId int,
	@exportNotAvailable bit
AS
BEGIN
	SET NOCOUNT ON;

	DELETE FROM [Settings].[ExportFeedCategoriesCache] WHERE ExportFeedId = @ExportFeedId

	DECLARE @selCategoryTemp TABLE (CategoryId INT);
	DECLARE @feedCategories TABLE (CategoryId INT PRIMARY KEY CLUSTERED, Opened bit);

	INSERT INTO @feedCategories
		SELECT t.CategoryId, t.Opened
		FROM [Settings].[ExportFeedSelectedCategories] AS t
			INNER JOIN Catalog.Category ON t.CategoryId = Category.CategoryId
		WHERE [ExportFeedId] = @exportFeedId
			AND (@exportNotAvailable = 1 OR (HirecalEnabled = 1 AND Enabled = 1))
			ORDER BY t.CategoryId

	DECLARE @categoryId INT
	SET @categoryId = (SELECT TOP 1 CategoryId FROM @feedCategories);

	WHILE @categoryId IS NOT NULL
	BEGIN

		if ((Select Opened from @feedCategories where CategoryId = @categoryId) = 1)
		begin
			INSERT INTO @selCategoryTemp
			SELECT @categoryId
		end
		else
		begin
	 		INSERT INTO @selCategoryTemp
			SELECT id FROM Settings.GetChildCategoryByParent(@categoryId)
		end

		SET @categoryId = (SELECT TOP 1 CategoryId FROM @feedCategories WHERE CategoryId > @categoryId);
	END;

	INSERT INTO Settings.ExportFeedCategoriesCache (ExportFeedId, CategoryId)
		SELECT DISTINCT @ExportFeedId, tmp.CategoryId FROM @selCategoryTemp AS tmp
			INNER JOIN Catalog.Category ON Category.CategoryId = tmp.CategoryId
		WHERE @exportNotAvailable = 1 OR (HirecalEnabled = 1 AND Enabled = 1)
END
GO--

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Customers].[TaskStatus]'))
BEGIN
	DROP TABLE [Customers].[TaskStatus]
END

GO--

CREATE TABLE [Catalog].[ProductChangeHistory](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ProductId] [int] NOT NULL,
	[ParameterName] [nvarchar](350) NOT NULL,
	[OldValue] [nvarchar](max) NOT NULL,
	[NewValue] [nvarchar](max) NOT NULL,
	[ParameterType] [int] NOT NULL,
	[ParameterId] [int] NULL,
	[ChangedByName] [nvarchar](255) NOT NULL,
	[ChangedById] [uniqueidentifier] NULL,
	[ModificationTime] [datetime] NOT NULL,
 CONSTRAINT [PK_ProductChangeHistory] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO--

CREATE NONCLUSTERED INDEX [ProductChangeHistory_ProductId] ON [Catalog].[ProductChangeHistory]
(
	[ProductId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO--

IF (SELECT count(1) FROM [CMS].[ChangeHistory]) < 1000000
BEGIN
	IF NOT EXISTS(SELECT * FROM [Catalog].[ProductChangeHistory])
	BEGIN
		INSERT INTO [Catalog].[ProductChangeHistory]
		([ProductId],[ParameterName],[OldValue],[NewValue],[ParameterType],[ParameterId],[ChangedByName],[ChangedById],[ModificationTime])
		SELECT [ObjId],[ParameterName],[OldValue],[NewValue],[ParameterType],[ParameterId],[ChangedByName],[ChangedById],[ModificationTime]
		FROM [CMS].[ChangeHistory]
		WHERE [ObjType] = 4
	END
ELSE
	TRUNCATE TABLE [CMS].[ChangeHistory]
END

GO--

DELETE FROM [CMS].[ChangeHistory]
	WHERE [ObjType] = 4

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ShippingMethods.BoxBerry.CalculateShippingMethods','Расчитывать методы доставки')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ShippingMethods.BoxBerry.CalculateShippingMethods','Calculate shipping methods')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ShippingMethods.BoxBerry.ChooseShippingMethod','Выберите метод доставки')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ShippingMethods.BoxBerry.ChooseShippingMethod','Choose a shipping method')

GO--

UPDATE [Order].[ShippingReplaceGeo]
SET [Enabled] = 0
WHERE [Id] in (7, 14)

GO--

UPDATE [Order].[ShippingReplaceGeo]
SET [OutCityName] = 'Астана (Нур-Султан)', [OutRegionName] = 'Нур-Султан'
WHERE [Id] = 8

GO--

UPDATE [Order].[ShippingReplaceGeo]
SET [OutCityName] = 'Астана (Нур-Султан)'
WHERE [Id] = 9

GO--

ALTER TABLE Catalog.ProductExportOptions ADD
	YandexMarketCisRequired bit NULL

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
    VALUES
           (1,'Core.Catalog.Product.YandexMarketCisRequired', N'Подлежит обязательной маркировке «Честный знак»'),
           (2,'Core.Catalog.Product.YandexMarketCisRequired', 'Required "Honest Mark" marking')
 
GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
    VALUES
           (1,'Core.ExportImport.ProductFields.YandexMarketCisRequired', N'Яндекс.Маркет: Подлежит маркировке «Честный знак»'),
           (2,'Core.ExportImport.ProductFields.YandexMarketCisRequired', 'Yandex.Market: Required "Honest Mark" marking'),
           (1,'Core.ExportImport.EProductField.YandexMarketCisRequired', N'Яндекс.Маркет: Подлежит маркировке «Честный знак»'),
           (2,'Core.ExportImport.EProductField.YandexMarketCisRequired', 'Yandex.Market: Required "Honest Mark" marking'),           
           (1,'Admin.ExportFeed.SettingsYandex.ExportMarketCisRequired', N'Выгружать тег с маркировкой «Честный знак»'),
           (2,'Admin.ExportFeed.SettingsYandex.ExportMarketCisRequired', 'Export "Honest Mark" marking'),
           (1,'Admin.ExportFeed.SettingsYandex.ExportMarketCisRequiredHint', N'Если у товара выбрана настройка "Подлежит маркировке «Честный знак»", то будет выгружаться тег cargo-types со значением CIS_REQUIRED.'),
           (2,'Admin.ExportFeed.SettingsYandex.ExportMarketCisRequiredHint', 'If product has setting "Required "Honest Mark" marking", then cargo-types tag with the value CIS_REQUIRED will be exported.')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
    VALUES
           (1,'Admin.Js.Product.Edit.Length', 'Длина (мм)'),
           (2,'Admin.Js.Product.Edit.Length', 'Length (mm)')
 
GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
    VALUES
           (1,'Admin.Js.Product.Edit.Width', 'Ширина (мм)'),
           (2,'Admin.Js.Product.Edit.Width', 'Width (mm)')
 
GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
    VALUES
           (1,'Admin.Js.Product.Edit.Height', 'Высота (мм)'),
           (2,'Admin.Js.Product.Edit.Height', 'Height (mm)')
 
GO--

Update [Settings].[Localization] 
Set [ResourceValue] = 'Задача {{source}} скопирована в задачу {{dest}}' 
Where [ResourceKey] = 'Admin.Js.Tasks.Tasks.TaskHasBeenCopied' and [LanguageId] = 1

GO--

Update [Settings].[Localization] 
Set [ResourceValue] = 'Task {{source}} copied to task {{dest}}' 
Where [ResourceKey] = 'Admin.Js.Tasks.Tasks.TaskHasBeenCopied' and [LanguageId] = 2

GO--

ALTER PROCEDURE [Catalog].[sp_RecalculateProductsCount]
AS
BEGIN
SET NOCOUNT ON 
;WITH cteSort AS
      (
      SELECT [Category].CategoryID AS Child,
			 [Category].ParentCategory AS Parent,
			 1  AS [Level]
        FROM [Catalog].[Category] WHERE CategoryID = 0 
      union ALL
      SELECT 
		 [Category].CategoryID AS Child,
		 [Category].ParentCategory AS Parent,
		 cteSort.[Level] + 1 AS [Level]
      FROM [Catalog].[Category] 
		   INNER JOIN cteSort ON [Category].ParentCategory = cteSort.Child and [Category].CategoryID<>0)

update c set 
   c.Products_Count=isnull(g.Products_Count,0)*c.Enabled, 
   c.Total_Products_Count=isnull(g.Total_Products_Count,0),

   c.Available_Products_Count=isnull(g.Available_Products_Count,0)*c.Enabled,
   c.Current_Products_Count=isnull(g.Current_Products_Count,0)*c.Enabled,

   c.CatLevel =cteSort.[Level]
from [Catalog].Category c
left join (
   select 
      pc.CategoryID, 
      SUM(1*p.Enabled*~p.Hidden) Products_Count,
      COUNT(*) Total_Products_Count,
	  SUM(1*p.Enabled*~p.Hidden*(CASE WHEN pExt.AmountSort = 0 THEN p.AllowPreOrder ELSE pExt.AmountSort END)) Available_Products_Count,
	  SUM(1*p.Enabled*~p.Hidden) Current_Products_Count
   from [Catalog].ProductCategories pc 
   inner join [Catalog].Product p on p.ProductID=pc.ProductID
   inner join [Catalog].[ProductExt] pExt on p.ProductID=pExt.ProductID
   --where for some conditions
   group by pc.CategoryID
   )g on g.CategoryID=c.CategoryID
left join cteSort on cteSort.Child = c.[CategoryID]

declare @max int
set @max = (select top(1) CatLevel from [Catalog].[Category] order by CatLevel  Desc)
 while (@max >0)
 begin
     UPDATE t1
		SET t1.Products_Count = t1.Products_Count + t2.pc,
		t1.Total_Products_Count = t1.Total_Products_Count + t2.tpc,

		t1.Available_Products_Count = t1.Available_Products_Count + t2.apc
		--t1.Current_Products_Count = t1.Current_Products_Count + t2.atpc

		from [Catalog].[Category] as t1 
		cross apply (Select COALESCE(SUM(Products_Count),0) pc,
							COALESCE(SUM(Total_Products_Count),0) tpc,
							COALESCE(SUM(Available_Products_Count),0) apc
							--COALESCE(SUM(Avalible_Total_Products_Count),0) atpc
							from [Catalog].[Category] where ParentCategory =t1.CategoryID) t2
							where t1.CategoryID in (Select CategoryID from [Catalog].[Category] where CatLevel =@max)
     Set @max = @max -1
 end
END

GO--

ALTER TABLE [CMS].[LandingColorScheme] ADD
	ButtonTextColorHover nvarchar(30) NULL,
	ButtonTextColorActive nvarchar(30) NULL,
	ButtonSecondaryTextColorHover nvarchar(30) NULL,
	ButtonSecondaryTextColorActive nvarchar(30) NULL;
GO--

IF not Exists(Select 1 From [Settings].[Settings] where [Name] = 'ShowAmountsTableInProduct')
begin
    Insert Into [Settings].[Settings] ([Name], [Value]) Values ('ShowAmountsTableInProduct', 'True')
end

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
    VALUES
           (1,'Admin.Js.AddEditCoupon.ConsiderCostProducts', 'Учитывать только стоимость товаров, к которым применим купон'),
           (2,'Admin.Js.AddEditCoupon.ConsiderCostProducts', 'Consider only the cost of products to which the coupon is applicable')
 
GO--


INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
    VALUES
           (1,'Admin.Settings.SystemSettings.CopyrightMode.Default', 'По умолчанию'),
           (2,'Admin.Settings.SystemSettings.CopyrightMode.Default', 'Default'),
           (1,'Admin.Settings.SystemSettings.CopyrightMode.Custom', 'Настраиваемый'),
           (2,'Admin.Settings.SystemSettings.CopyrightMode.Custom', 'Custom')
    
GO--
    
IF EXISTS(SELECT 1 FROM [Settings].[Settings] WHERE [NAME] = 'ShowCopyright')
BEGIN
    DELETE FROM [Settings].[Settings] WHERE [NAME] = 'ShowCopyright' 
END

GO--

IF EXISTS(SELECT 1 FROM [Settings].[Localization] WHERE [ResourceKey] = 'Admin.Settings.SystemSettings.ShowCopyright')
BEGIN
    DELETE FROM [Settings].[Localization] WHERE [ResourceKey] = 'Admin.Settings.SystemSettings.ShowCopyright'
END
    
GO--

Update [Settings].[Settings]
Set [Value] = 'None'
Where [Name] = 'SettingsThankYouPage.ActionType' AND [Value] = 'Share'

GO--

IF EXISTS(SELECT 1 FROM [Settings].[Settings] WHERE [Name] = 'SocialShareCustomEnabled' AND [Value] = 'False')
BEGIN
    UPDATE [Settings].[Settings] SET [Value] = 'False' WHERE [Name] = 'SocialShareEnabled';
END

GO--

IF EXISTS(SELECT 1 FROM [Settings].[Settings] WHERE [Name] = 'SocialShareCustomEnabled')
BEGIN
    DELETE FROM [Settings].[Settings] WHERE [Name] = 'SocialShareCustomEnabled'
END

GO--

ALTER TABLE Customers.Customer ADD
	FcmToken nvarchar(MAX) NULL
GO--

UPDATE [Settings].[Localization]
SET [ResourceValue] = 'Опция изменяет отображение надписей об Copyright в подвале сайта.<br/><br/>Подробнее:<br/><a href="https://www.advantshop.net/help/pages/footer#4" target="_blank">Copyright</a>'
WHERE [ResourceKey] = 'Admin.Settings.SystemSettings.ShowCopyrightNote' and [LanguageId] = 1

UPDATE [Settings].[Localization]
SET [ResourceValue] = 'This option changes the display of copyright inscriptions in the footer of the site.<br/><br/>Details:<br/><a href="https://www.advantshop.net/help/pages/footer#4" target="_blank">Copyright</a>'
WHERE [ResourceKey] = 'Admin.Settings.SystemSettings.ShowCopyrightNote' and [LanguageId] = 2

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
    VALUES
           (1,'Core.ProductHistory.OfferAmountChangedByOrder', 'Кол-во товара "{1}" было изменено на {2} в заказе "{0}"'),
           (2,'Core.ProductHistory.OfferAmountChangedByOrder', 'Product amount for "{1}" has been changed on {2} in order "{0}"')
    
GO--

UPDATE [Order].[ShippingReplaceGeo]
SET [OutRegionName] = 'Алматы'
WHERE [Id] = 3

GO--

UPDATE [Order].[ShippingReplaceGeo]
SET [OutCityName] = 'Астана'
WHERE [Id] = 8

GO--

UPDATE [Order].[ShippingReplaceGeo]
SET [OutCityName] = 'Астана'
WHERE [Id] = 9

GO--

UPDATE [Order].[ShippingReplaceGeo]
SET [OutRegionName] = 'Шымкент'
WHERE [Id] = 31

GO--

UPDATE [Order].[ShippingReplaceGeo] SET [Enabled] = 1 WHERE [Id] = 25

GO--

UPDATE [Order].[ShippingReplaceGeo] SET [Enabled] = 0 WHERE [Id] = 28

GO--

SET IDENTITY_INSERT [Order].[ShippingReplaceGeo] ON

IF NOT EXISTS (SELECT * FROM [Order].[ShippingReplaceGeo] WHERE [Id] = 49)
BEGIN
    INSERT INTO [Order].[ShippingReplaceGeo] ([Id],[ShippingType],[InCountryName],[InCountryISO2],[InRegionName],[InCityName],[InDistrict],[OutCountryName],[OutRegionName],[OutCityName],[OutDistrict],[OutDistrictClear],[Enabled],[Sort],[InZip],[OutZip],[Comment])
    VALUES (49,'Sdek','Казахстан','KZ','Алматинская область','Алматы','','','Алматы','','',0,1,0,'','','')
END

SET IDENTITY_INSERT [Order].[ShippingReplaceGeo] OFF

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue])
VALUES
    (1,'Cart.Loading', 'Получение корзины...'),
    (2,'Cart.Loading', 'Getting baskets...')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue])
VALUES
    (1,'Admin.Js.Order.DiscountWhenOrdering', 'Скидка на момент заказа'),
    (2,'Admin.Js.Order.DiscountWhenOrdering', 'Discount at the time of order')

GO--

IF NOT EXISTS(SELECT 1
              FROM sys.columns
              WHERE (name = N'NotificationBody') AND object_id = OBJECT_ID(N'[CRM].[TriggerAction]'))
BEGIN
ALTER TABLE [CRM].[TriggerAction]
    ADD NotificationBody NVARCHAR(MAX) NULL
END

IF NOT EXISTS(SELECT 1
              FROM sys.columns
              WHERE (name = N'NotificationTitle') AND object_id = OBJECT_ID(N'[CRM].[TriggerAction]'))
BEGIN
ALTER TABLE [CRM].[TriggerAction]
    ADD NotificationTitle NVARCHAR(MAX) NULL
END

GO--
    
UPDATE [Settings].[InternalSettings] SET [settingValue] = '11.0.1' WHERE [settingKey] = 'db_version'
