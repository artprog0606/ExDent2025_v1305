CREATE NONCLUSTERED INDEX ChangeHistory_Obj ON CMS.ChangeHistory
	(
	ObjId,
	ObjType
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO--

IF NOT EXISTS (SELECT * FROM Settings.Settings WHERE Name = 'PartnersAppActive')
BEGIN
	INSERT INTO Settings.Settings (Name, Value) VALUES ('PartnersAppActive', 'False')
END
GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.SystemSettings.AppsPartnersActive', 'Партнеры')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Home.Menu.Partners', 'Партнеры')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Customers.RoleActionCategory.Partners', 'Партнеры')
GO--

INSERT INTO [Settings].[Localization] ([LanguageId], [ResourceKey], [ResourceValue]) VALUES (1, 'Landing.Domain.Auth.ELpAuthFilterRule.Registered', 'Все зарегистрированные пользователи')
INSERT INTO [Settings].[Localization] ([LanguageId], [ResourceKey], [ResourceValue]) VALUES (2, 'Landing.Domain.Auth.ELpAuthFilterRule.Registered', 'All registered users')
INSERT INTO [Settings].[Localization] ([LanguageId], [ResourceKey], [ResourceValue]) VALUES (1, 'Landing.Domain.Auth.ELpAuthFilterRule.WithOrderAndProduct', 'Пользователи, имеющие оплаченный заказ на определенный товар')
INSERT INTO [Settings].[Localization] ([LanguageId], [ResourceKey], [ResourceValue]) VALUES (2, 'Landing.Domain.Auth.ELpAuthFilterRule.WithOrderAndProduct', 'Users with paid order for a selected product')
INSERT INTO [Settings].[Localization] ([LanguageId], [ResourceKey], [ResourceValue]) VALUES (1, 'Landing.Domain.Auth.ELpAuthFilterRule.WithLead', 'Пользователи, имеющие лид в списке лидов')
INSERT INTO [Settings].[Localization] ([LanguageId], [ResourceKey], [ResourceValue]) VALUES (2, 'Landing.Domain.Auth.ELpAuthFilterRule.WithLead', 'Users with a lead in sales funnel')

GO--


INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Catalog.UnloadingToAvito', 'Вгрузка на Avito')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Catalog.UnloadingToAvito', 'Export to Avito')

GO--

CREATE TABLE [Booking].[ShoppingCart](
	[ShoppingCartItemId] [int] IDENTITY(1,1) NOT NULL,
	[CustomerId] [uniqueidentifier] NOT NULL,
	[AffiliateId] [int] NOT NULL,
	[ReservationResourceId] [int] NULL,
	[BeginDate] [datetime] NOT NULL,
	[EndDate] [datetime] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[UpdatedOn] [datetime] NOT NULL,
	[Services] [nvarchar](max) NULL,
 CONSTRAINT [PK_ShoppingCart_1] PRIMARY KEY CLUSTERED 
(
	[ShoppingCartItemId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO--

ALTER TABLE [Booking].[ShoppingCart]  WITH CHECK ADD  CONSTRAINT [FK_ShoppingCart_Affiliate] FOREIGN KEY([AffiliateId])
REFERENCES [Booking].[Affiliate] ([Id])
ON DELETE CASCADE
GO--

ALTER TABLE [Booking].[ShoppingCart] CHECK CONSTRAINT [FK_ShoppingCart_Affiliate]
GO--

ALTER TABLE [Booking].[ShoppingCart]  WITH CHECK ADD  CONSTRAINT [FK_ShoppingCart_ReservationResource] FOREIGN KEY([ReservationResourceId])
REFERENCES [Booking].[ReservationResource] ([Id])
ON DELETE CASCADE
GO--

ALTER TABLE [Booking].[ShoppingCart] CHECK CONSTRAINT [FK_ShoppingCart_ReservationResource]
GO--

CREATE NONCLUSTERED INDEX [ShoppingCart_Customer] ON [Booking].[ShoppingCart]
(
	[CustomerId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO--

CREATE NONCLUSTERED INDEX [ShoppingCart_ReservationResource] ON [Booking].[ShoppingCart]
(
	[ReservationResourceId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO--

INSERT INTO [Settings].[Localization] ([LanguageId], [ResourceKey], [ResourceValue]) VALUES (1, 'Core.ExportImport.MultiOrder.Manager', 'Менеджер')
INSERT INTO [Settings].[Localization] ([LanguageId], [ResourceKey], [ResourceValue]) VALUES (2, 'Core.ExportImport.MultiOrder.Manager', 'Manager')

GO--

UPDATE [Settings].[Localization] SET [ResourceValue]='Размеры высоты (в пикселях) иконки цвета. Минимальное значение 10х10 px, максимальное 1000х1000 px.' WHERE [ResourceKey] ='Admin.SettingsCatalog.CatalogCommon.HeightOfColorIcon' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue]='The height dimensions (in pixels) of the color icon.The minimum value is 10x10 px, the maximum is 1000x1000 px.' WHERE [ResourceKey] ='Admin.SettingsCatalog.CatalogCommon.HeightOfColorIcon' AND [LanguageId] = 2

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.AddEditYandexPromo.Categories', 'Категории участвующие в промоакции')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.AddEditYandexPromo.Categories', 'Categories')
UPDATE [Settings].[Localization] SET [ResourceValue] =  'Выгрузка на Avito' WHERE [LanguageId] = 1 AND [ResourceKey] = 'Admin.Catalog.UnloadingToAvito'
UPDATE [Settings].[Localization] SET [ResourceValue] =  'Внешний ключ категории' WHERE [LanguageId] = 1 AND [ResourceKey] = 'Core.ExportImport.CategoryFields.ExternalId'


GO--

if not exists (select * from catalog.tax where taxtype = 5)
begin
	insert into catalog.tax (Name, Enabled, Showinprice, rate, taxtype) values ('НДС 20%', 1, 1, 20, 5)
end

GO--
insert into Settings.Localization (LanguageId,ResourceKey,ResourceValue) Values (1,'Admin.Js.LeadsListSources.Name','Источник')
insert into Settings.Localization (LanguageId,ResourceKey,ResourceValue) Values (2,'Admin.Js.LeadsListSources.Name','Source')

insert into Settings.Localization (LanguageId,ResourceKey,ResourceValue) Values (1,'Admin.Js.LeadsListSources.LeadsCount','Количество')
insert into Settings.Localization (LanguageId,ResourceKey,ResourceValue) Values (2,'Admin.Js.LeadsListSources.LeadsCount','Count')

insert into Settings.Localization (LanguageId,ResourceKey,ResourceValue) Values (1,'Admin.Js.LeadsListSources.PercentLeads','%')
insert into Settings.Localization (LanguageId,ResourceKey,ResourceValue) Values (2,'Admin.Js.LeadsListSources.PercentLeads','%')

GO--
insert into Settings.Localization (LanguageId,ResourceKey,ResourceValue) Values (1,'Admin.Js.LeadsListChart.DateFrom','Дата от')
insert into Settings.Localization (LanguageId,ResourceKey,ResourceValue) Values (2,'Admin.Js.LeadsListChart.DateFrom','Date from')

insert into Settings.Localization (LanguageId,ResourceKey,ResourceValue) Values (1,'Admin.Js.LeadsListChart.DateTo','до')
insert into Settings.Localization (LanguageId,ResourceKey,ResourceValue) Values (2,'Admin.Js.LeadsListChart.DateTo','to')

GO--


INSERT INTO [Settings].[Localization] ([LanguageId], [ResourceKey], [ResourceValue]) VALUES (1, 'Landing.Domain.Common.ELpShoppingCartType.Goods', 'с товарами')
INSERT INTO [Settings].[Localization] ([LanguageId], [ResourceKey], [ResourceValue]) VALUES (2, 'Landing.Domain.Common.ELpShoppingCartType.Goods', 'goods')

INSERT INTO [Settings].[Localization] ([LanguageId], [ResourceKey], [ResourceValue]) VALUES (1, 'Landing.Domain.Common.ELpShoppingCartType.Booking', 'с бронями')
INSERT INTO [Settings].[Localization] ([LanguageId], [ResourceKey], [ResourceValue]) VALUES (2, 'Landing.Domain.Common.ELpShoppingCartType.Booking', 'booking')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId], [ResourceKey], [ResourceValue]) VALUES (1, 'Admin.Leads.LeadsGraph.LeadsListCount', 'Количество лидов')
INSERT INTO [Settings].[Localization] ([LanguageId], [ResourceKey], [ResourceValue]) VALUES (2, 'Admin.Leads.LeadsGraph.LeadsListCount', 'Leads count')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId], [ResourceKey], [ResourceValue]) VALUES (1, 'Admin.Js.SalesFunnels.DeleteSelected', 'Удалить выделенные')
INSERT INTO [Settings].[Localization] ([LanguageId], [ResourceKey], [ResourceValue]) VALUES (2, 'Admin.Js.SalesFunnels.DeleteSelected', 'Delete selected')

INSERT INTO [Settings].[Localization] ([LanguageId], [ResourceKey], [ResourceValue]) VALUES (1, 'Admin.SalesFunnels.Edit.DeleteDefaultFunnel', 'Нельзя удалить список заданный по умолчанию')
INSERT INTO [Settings].[Localization] ([LanguageId], [ResourceKey], [ResourceValue]) VALUES (2, 'Admin.SalesFunnels.Edit.DeleteDefaultFunnel', 'Cannot delete default list')

INSERT INTO [Settings].[Localization] ([LanguageId], [ResourceKey], [ResourceValue]) VALUES (1, 'Admin.SalesFunnels.Edit.NameIsEmpty', 'Имя списка лидов не может быть пустым')
INSERT INTO [Settings].[Localization] ([LanguageId], [ResourceKey], [ResourceValue]) VALUES (2, 'Admin.SalesFunnels.Edit.NameIsEmpty', 'Leads list name cannot be empty')
GO--

ALTER TABLE CRM.TriggerAction ADD
	RequestMethod int NULL,
	RequestUrl nvarchar(MAX) NULL,
	RequestParams nvarchar(MAX) NULL

GO--
INSERT INTO [Settings].[Localization] ([LanguageId], [ResourceKey], [ResourceValue]) VALUES (1, 'Admin.Import.Errors.NotColumnSeparator', 'Не задан разделитель между колонками')
INSERT INTO [Settings].[Localization] ([LanguageId], [ResourceKey], [ResourceValue]) VALUES (2, 'Admin.Import.Errors.NotColumnSeparator', 'No separator set between columns')

GO--

-- old mail formats
delete from [Settings].[MailFormat] where [MailFormatTypeId] in (select [MailFormatTypeID] from [Settings].[MailFormatType] where [MailType] in ('OnSetManagerTask', 'OnChangeManagerTaskStatus'))
delete from [Settings].[MailFormatType] where [MailType] in ('OnSetManagerTask', 'OnChangeManagerTaskStatus')
GO--

IF NOT EXISTS(SELECT * FROM [Settings].[MailFormatType] WHERE [MailType] = 'OnLeadAssigned')
INSERT INTO [Settings].[MailFormatType] ([TypeName], [SortOrder], [Comment], [MailType]) 
VALUES ('Менеджеру назначен лид', 201, 
	'Уведомление о назначенном лиде. (#STORE_NAME#, #LEAD_ID#, #NAME#, #PHONE#, #EMAIL#, #ORGANIZATION#, #ORDERTABLE#, #SHIPPINGMETHOD#, #CITY#, #COMMENTS#, #DESCRIPTION#, #DEAL_STATUS#, #DATE#, #MANAGER_NAME#, #ADDITIONALCUSTOMERFIELDS#, #LEADS_LIST#, #SOURCE#, #LEAD_ATTACHMENTS#', 
	'OnLeadAssigned')
GO--

UPDATE [Settings].[MailFormatType] SET [SortOrder] = 280 WHERE [MailType] = 'OnChangeUserComment'
GO--

UPDATE [Settings].[MailFormatType] 
SET  [Comment] = 'Письмо администратору при создании лида (#STORE_NAME#, #LEAD_ID#, #NAME#, #PHONE#, #EMAIL#, #ORGANIZATION#, #ORDERTABLE#, #SHIPPINGMETHOD#, #CITY#, #COMMENTS#, #DESCRIPTION#, #DEAL_STATUS#, #DATE#, #MANAGER_NAME#, #ADDITIONALCUSTOMERFIELDS#, #LEADS_LIST#, #SOURCE#, #LEAD_ATTACHMENTS#)' 
WHERE [MailType] = 'OnLead'
GO--


IF NOT EXISTS(SELECT * FROM [Settings].[MailFormat] WHERE [MailFormatTypeId] = (SELECT TOP(1) [MailFormatTypeID] FROM [Settings].[MailFormatType] WHERE [MailType] = 'OnLeadAssigned'))
INSERT INTO [Settings].[MailFormat] ([FormatName],[FormatText],[SortOrder],[Enable],[AddDate],[ModifyDate],[FormatSubject],[MailFormatTypeId])
Values ('Менеджеру назначен лид', '<div style="color: #4c4f56; font-family: Arial, Helvetica, sans-serif; font-size: 14px;">
    <div class="header" style="border-bottom: 1px solid #ededed; display: table; margin-bottom: 25px; padding-bottom: 25px; width: 100%;">
        <div class="logo" style="display: table-cell; text-align: left; vertical-align: middle;">
            #LOGO#
        </div>
        <div class="phone" style="display: table-cell; text-align: right; vertical-align: middle;">
            <div class="tel" style="font-size: 26px; font-weight: bold; line-height: 1; margin-bottom: 5px;">&nbsp;</div>
        </div>
    </div>
    <div style="padding:0 0 10px 0;font-weight:bold">Вам назначен лид №#LEAD_ID#</div>
    <div class="data" style="display: table; width: 100%;">
        <div class="data-cell" style="display: table-cell; padding: 0; padding-right: 1%; width: 48%;">
            <div class="l-row">
                <div class="l-name vi cs-light" style="color: #acacac; display: inline-block; margin: 5px 0; padding-right: 15px; width: 150px; vertical-align: middle;">
                    Список лидов:
                </div>
                <div class="l-value vi" style="display: inline-block; margin: 5px 0;">
                    #LEADS_LIST#
                </div>
            </div>
            <div class="l-row">
                <div class="l-name vi cs-light" style="color: #acacac; display: inline-block; margin: 5px 0; padding-right: 15px; width: 150px; vertical-align: middle;">
                    Этап сделки:
                </div>
                <div class="l-value vi" style="display: inline-block; margin: 5px 0;">
                    #DEAL_STATUS#
                </div>
            </div>
            <div class="l-row">
                <div class="l-name vi cs-light" style="color: #acacac; display: inline-block; margin: 5px 0; padding-right: 15px; width: 150px; vertical-align: middle;">
                    Источник:
                </div>
                <div class="l-value vi" style="display: inline-block; margin: 5px 0;">
                    #SOURCE#
                </div>
            </div>
            <div class="l-row">
                <div class="l-name vi cs-light" style="color: #acacac; display: inline-block; margin: 5px 0; padding-right: 15px; width: 150px; vertical-align: middle;">
                    Приложения:
                </div>
                <div class="l-value vi" style="display: inline-block; margin: 5px 0;">
                    #LEAD_ATTACHMENTS#
                </div>
            </div>
            <div class="l-row">
                <div class="l-name vi cs-light" style="color: #acacac; display: inline-block; margin: 5px 0; padding-right: 15px; width: 150px; vertical-align: middle;">
                    Описание:
                </div>
                <div class="l-value vi" style="display: inline-block; margin: 5px 0;">
                    #DESCRIPTION#
                </div>
            </div>
            <div style="padding:20px 0 10px;font-weight:bold;">Покупатель:</div>
            <div class="l-row">
                <div class="l-name vi cs-light" style="color: #acacac; display: inline-block; margin: 5px 0; padding-right: 15px; width: 150px; vertical-align: middle;">
                    Имя:
                </div>
                <div class="l-value vi" style="display: inline-block; margin: 5px 0;">
                    #NAME#
                </div>
            </div>
            <div class="l-row">
                <div class="l-name vi cs-light" style="color: #acacac; display: inline-block; margin: 5px 0; padding-right: 15px; width: 150px; vertical-align: middle;">
                    Номер телефона:
                </div>
                <div class="l-value vi" style="display: inline-block; margin: 5px 0;">
                    #PHONE#
                </div>
            </div>
            <div class="l-row">
                <div class="l-name vi cs-light" style="color: #acacac; display: inline-block; margin: 5px 0; padding-right: 15px; width: 150px; vertical-align: middle;">
                    E-mail:
                </div>
                <div class="l-value vi" style="display: inline-block; margin: 5px 0;">
                    #EMAIL#
                </div>
            </div>
            <div class="l-row">
                <div class="l-name vi cs-light" style="color: #acacac; display: inline-block; margin: 5px 0; padding-right: 15px; width: 150px; vertical-align: middle;">
                    Организация:
                </div>
                <div class="l-value vi" style="display: inline-block; margin: 5px 0;">
                    #ORGANIZATION#
                </div>
            </div>
            <div class="l-row">
                <div class="l-name vi cs-light" style="color: #acacac; display: inline-block; margin: 5px 0; padding-right: 15px; width: 150px; vertical-align: middle;">
                    Город:
                </div>
                <div class="l-value vi" style="display: inline-block; margin: 5px 0;">
                    #CITY#
                </div>
            </div>
            #ADDITIONALCUSTOMERFIELDS#
        </div>
    </div>
    <div>
        <div class="o-big-title" style="font-weight: bold; margin-bottom: 20px; margin-top: 40px;">Содержание заказа:</div>
        #ORDERTABLE#
        <div class="comment" style="margin-top: 15px;">
            <span class="comment-title" style="font-weight: bold;">Комментарии: </span>
            <span class="comment-txt" style="padding-left: 5px;"> #COMMENTS# </span>
        </div>
    </div>
</div>', 1099, 1, GETDATE(), GETDATE(), 'Вам назначен лид №#LEAD_ID#', (SELECT TOP(1) [MailFormatTypeID] FROM [Settings].[MailFormatType] WHERE [MailType] = 'OnLeadAssigned'))
GO--

ALTER TABLE [Catalog].[Property]
  ALTER COLUMN Description nvarchar(max) NULL
  
GO--  

  ALTER PROCEDURE [Catalog].[sp_UpdateProperty]
	@PropertyID int,
    @Name nvarchar(100),
	@NameDisplayed nvarchar(100),
    @UseInFilter bit,
    @SortOrder int,
    @Expanded bit,
	@UseInDetails bit,
	@Description nvarchar(max),
	@Unit nvarchar(25),
	@Type tinyint,
	@GroupId int,
	@UseInBrief bit
AS

BEGIN
	SET NOCOUNT ON;

UPDATE [Catalog].[Property]
   SET [Name] = @Name
	  ,[NameDisplayed] = @NameDisplayed
      ,[UseInFilter] = @UseInFilter
      ,[SortOrder] = @SortOrder
      ,[Expanded] = @Expanded
	  ,[UseInDetails] = @UseInDetails
	  ,[Description] = @Description
	  ,[Unit] = @Unit
	  ,[Type] = @Type
	  ,[GroupId] = @GroupId
	  ,[UseInBrief] = @UseInBrief
 WHERE [PropertyID] = @PropertyID
 END
 
GO--
INSERT INTO [Settings].[Localization] ([LanguageId], [ResourceKey], [ResourceValue]) VALUES (1, 'Admin.Import.ImportProducts.AddTheFile', 'Добавить файл')
INSERT INTO [Settings].[Localization] ([LanguageId], [ResourceKey], [ResourceValue]) VALUES (2, 'Admin.Import.ImportProducts.AddTheFile', 'Add the file')
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
	
	DECLARE @lcategory TABLE (CategoryId INT PRIMARY KEY CLUSTERED);
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
			INSERT INTO @lcategory
			SELECT CategoryId from
			CATALOG.Category cat  
			WHERE cat.CategoryId NOT IN (
				SELECT CategoryId FROM @lcategory)
			AND HirecalEnabled = 1
			AND Enabled = 1 
			and CategoryId = @l1
		end
		else
		begin
	 		INSERT INTO @lcategory
			SELECT id
			FROM Settings.GetChildCategoryByParent(@l1) AS dt
			INNER JOIN CATALOG.Category ON CategoryId = id
			WHERE dt.id NOT IN (
					SELECT CategoryId FROM @lcategory)
			AND HirecalEnabled = 1
			AND Enabled = 1
		end

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

drop table  [settings].[ExportFeedSelectedProducts]

GO--

INSERT INTO [Settings].[Localization] ([LanguageId], [ResourceKey], [ResourceValue]) VALUES (1, 'Core.Loging.AdvantShopEmailErrorStatus.Unsubscribed', 'Указанный адрес отписан')
INSERT INTO [Settings].[Localization] ([LanguageId], [ResourceKey], [ResourceValue]) VALUES (2, 'Core.Loging.AdvantShopEmailErrorStatus.Unsubscribed', 'Address is unsubscribed')

INSERT INTO [Settings].[Localization] ([LanguageId], [ResourceKey], [ResourceValue]) VALUES (1, 'Core.Loging.AdvantShopEmailErrorStatus.Invalid', 'Адрес не существует или введен некорректно')
INSERT INTO [Settings].[Localization] ([LanguageId], [ResourceKey], [ResourceValue]) VALUES (2, 'Core.Loging.AdvantShopEmailErrorStatus.Invalid', 'The address does not exist or is invalid')

INSERT INTO [Settings].[Localization] ([LanguageId], [ResourceKey], [ResourceValue]) VALUES (1, 'Core.Loging.AdvantShopEmailErrorStatus.Duplicate', 'Адрес повторяется в запросе')
INSERT INTO [Settings].[Localization] ([LanguageId], [ResourceKey], [ResourceValue]) VALUES (2, 'Core.Loging.AdvantShopEmailErrorStatus.Duplicate', 'The address is repeated in the request')

INSERT INTO [Settings].[Localization] ([LanguageId], [ResourceKey], [ResourceValue]) VALUES (1, 'Core.Loging.AdvantShopEmailErrorStatus.TemporaryUnavailable', 'Адрес временно недоступен. Возможные причины: предыдущая отправка была отвергнута сервером получателя как спам; почтовый ящик получателя переполнен; домен не принимает почту из-за неверной настройки на стороне получателя; домен не принимает никакую почту вообще, по любой причине; ящик не используется; сервер отправителя был отвергнут из-за блеклистинга')
INSERT INTO [Settings].[Localization] ([LanguageId], [ResourceKey], [ResourceValue]) VALUES (2, 'Core.Loging.AdvantShopEmailErrorStatus.TemporaryUnavailable', 'The address is temporarily unavailable. Possible reasons: the previous sending was rejected by the recipient''s server as spam; the recipient''s mailbox is full; the domain does not accept mail due to misconfiguration on the recipient''s side; the domain does not accept any mail at all, for any reason; the mailbox is not used; the sender''s server was rejected due to blacklisting')

INSERT INTO [Settings].[Localization] ([LanguageId], [ResourceKey], [ResourceValue]) VALUES (1, 'Core.Loging.AdvantShopEmailErrorStatus.PermanentUnavailable', 'Адрес перманентно недоступен, глобально отписан, либо в одном из предыдущих писем нажал “Это спам”')
INSERT INTO [Settings].[Localization] ([LanguageId], [ResourceKey], [ResourceValue]) VALUES (2, 'Core.Loging.AdvantShopEmailErrorStatus.PermanentUnavailable', 'The address is permanently unavailable, globally unsubscribed, or in one of the previous emails clicked "this is spam”')

INSERT INTO [Settings].[Localization] ([LanguageId], [ResourceKey], [ResourceValue]) VALUES (1, 'Core.Loging.AdvantShopEmailErrorStatus.InternalServerError', 'Внутренняя ошибка')
INSERT INTO [Settings].[Localization] ([LanguageId], [ResourceKey], [ResourceValue]) VALUES (2, 'Core.Loging.AdvantShopEmailErrorStatus.InternalServerError', 'Internal error')

GO--
INSERT INTO [Settings].[Localization] ([LanguageId], [ResourceKey], [ResourceValue]) VALUES (1, 'Admin.Js.EmailingLog.FormatName', 'Название формата письма')
INSERT INTO [Settings].[Localization] ([LanguageId], [ResourceKey], [ResourceValue]) VALUES (2, 'Admin.Js.EmailingLog.FormatName', 'Format email name')
INSERT INTO [Settings].[Localization] ([LanguageId], [ResourceKey], [ResourceValue]) VALUES (1, 'Admin.Js.EmailingLog.Count', 'Кол-во')
INSERT INTO [Settings].[Localization] ([LanguageId], [ResourceKey], [ResourceValue]) VALUES (2, 'Admin.Js.EmailingLog.Count', 'Count')

delete from [Settings].[Localization] where [ResourceKey] = 'Admin.ManualEmailings.WithoutEmailing.Title'
INSERT INTO [Settings].[Localization] ([LanguageId], [ResourceKey], [ResourceValue]) VALUES (1, 'Admin.ManualEmailings.WithoutEmailing.Title', 'Аналитика транзакционных писем')
INSERT INTO [Settings].[Localization] ([LanguageId], [ResourceKey], [ResourceValue]) VALUES (2, 'Admin.ManualEmailings.WithoutEmailing.Title', 'Analytics transaction emails')

INSERT INTO [Settings].[Localization] ([LanguageId], [ResourceKey], [ResourceValue]) VALUES (1, 'Admin.ManualEmailings.EmailingsWith', 'Email рассылки')
INSERT INTO [Settings].[Localization] ([LanguageId], [ResourceKey], [ResourceValue]) VALUES (2, 'Admin.ManualEmailings.EmailingsWith', 'Emailings')

INSERT INTO [Settings].[Localization] ([LanguageId], [ResourceKey], [ResourceValue]) VALUES (1, 'Admin.ManualEmailings.EmailingsWithous', 'Транзакционные письма')
INSERT INTO [Settings].[Localization] ([LanguageId], [ResourceKey], [ResourceValue]) VALUES (2, 'Admin.ManualEmailings.EmailingsWithous', 'Transaction emails')

delete from [Settings].[Localization] where [ResourceKey] = 'Admin.Home.Menu.ManualEmailings'
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Home.Menu.ManualEmailings', 'Почтовая аналитика')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Home.Menu.ManualEmailings', 'Email Analytics')

delete from [Settings].[Localization] where [ResourceKey] = 'Admin.ManualEmailings.Title'
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ManualEmailings.Title', 'Аналитика email рассылок')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ManualEmailings.Title', 'Manual emailings analytics')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId], [ResourceKey], [ResourceValue]) VALUES (1, 'Js.ShoppingCart.AddShoppingCartSuccess', 'Товар успешно добавлен в корзину')
INSERT INTO [Settings].[Localization] ([LanguageId], [ResourceKey], [ResourceValue]) VALUES (2, 'Js.ShoppingCart.AddShoppingCartSuccess', 'Product successfully added to cart')

GO--

UPDATE [Settings].[Localization] SET [ResourceValue]='Нажимая на кнопку "Войти в мой магазин"' WHERE [ResourceKey] ='Admin.Js.UserInfoPopup.IAgreeToProcessing' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue]='By clicking on the button "Enter my store"' WHERE [ResourceKey] ='Admin.Js.UserInfoPopup.IAgreeToProcessing' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue]='я даю согласие на обработку своих персональных данных' WHERE [ResourceKey] ='Admin.Js.UserInfoPopup.PersonalData' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue]='I agree to process my own personal data' WHERE [ResourceKey] ='Admin.Js.UserInfoPopup.PersonalData' AND [LanguageId] = 2

GO--

ALTER PROCEDURE [Settings].[sp_GetExportFeedCategories] @exportFeedId int
	,@onlyCount BIT
AS
BEGIN
	--template for result array
	DECLARE @result TABLE (CategoryId INT PRIMARY KEY CLUSTERED);
	-- templete for array of categories
	DECLARE @lcategory TABLE (CategoryId INT PRIMARY KEY CLUSTERED, Opened bit);

	INSERT INTO @lcategory
	SELECT t.CategoryId, t.Opened
	FROM [Settings].[ExportFeedSelectedCategories] AS t
	INNER JOIN CATALOG.Category ON t.CategoryId = Category.CategoryId
	WHERE HirecalEnabled = 1
		AND Enabled = 1
		AND [ExportFeedId] = @exportFeedId

	DECLARE @l1 INT

	SET @l1 = (
			SELECT MIN(CategoryId)
			FROM @lcategory
			);

	WHILE @l1 IS NOT NULL
	BEGIN
	if ((Select Opened from @lcategory where CategoryId=@l1)=0)
	begin
		--add categories by step thats no in array 
		INSERT INTO @result
		SELECT id
		FROM Settings.GetChildCategoryByParent(@l1) AS dt
		INNER JOIN CATALOG.Category ON CategoryId = id
		WHERE dt.id NOT IN (
				SELECT CategoryId
				FROM @result
				)
			AND HirecalEnabled = 1
			AND Enabled = 1
		end	
		insert into @result
		select id from [Settings].[GetParentsCategoryByChild] (@l1) as dt
		where dt.id not in (SELECT CategoryId FROM @result)
		
		SET @l1 = (
				SELECT MIN(CategoryId)
				FROM @lcategory
				WHERE CategoryId > @l1
				);
	END;

	-- templete for array of categoiries by only selected product
	DECLARE @lproduct TABLE (CategoryId INT PRIMARY KEY CLUSTERED);

	INSERT INTO @lproduct
	SELECT DISTINCT CategoryID
	FROM [Catalog].[ProductCategories]
	INNER JOIN [Settings].[ExportFeedExcludedProducts] ON [ProductCategories].[ProductID] = [ExportFeedExcludedProducts].[ProductID]
		AND [ExportFeedId] = @exportFeedId
	WHERE [ExportFeedExcludedProducts].[ProductID] IN (
			SELECT Product.[ProductID]
			FROM CATALOG.Product
			INNER JOIN [Catalog].[Offer] ON [Offer].[ProductID] = [Product].[ProductID]
			WHERE Offer.Price > 0
				AND (
					Offer.Amount > 0
					OR Product.AllowPreorder = 1
					)
				AND CategoryEnabled = 1
				AND Enabled = 1
			)

	SET @l1 = (
			SELECT MIN(CategoryId)
			FROM @lproduct
			);

	WHILE @l1 IS NOT NULL
	BEGIN
		--add categories by step thats no in array 
		INSERT INTO @result
		SELECT id
		FROM Settings.[GetParentsCategoryByChild](@l1) AS dt
		INNER JOIN CATALOG.Category ON CategoryId = id
		WHERE dt.id NOT IN (
				SELECT CategoryId
				FROM @result
				)
			AND HirecalEnabled = 1
			AND Enabled = 1
			
		
		SET @l1 = (
				SELECT MIN(CategoryId)
				FROM @lproduct
				WHERE CategoryId > @l1
				);
	END;

	IF @onlyCount = 1
	BEGIN
		SELECT Count([CategoryID])
		FROM [Catalog].[Category]
		WHERE CategoryID <> 0
			AND CategoryId IN (
				SELECT CategoryId
				FROM @result
				)
	END
	ELSE
	BEGIN
		SELECT [CategoryID]
			,[ParentCategory]
			,[Name]
		FROM [Catalog].[Category]
		WHERE CategoryID <> 0
			AND CategoryId IN (
				SELECT CategoryId
				FROM @result
				)
	END
END

GO--

DECLARE @MailFormatTypeID INT

INSERT INTO [Settings].[MailFormatType] ([TypeName],[SortOrder],[Comment],[MailType])
VALUES ('Изменение лида', 205, 'Письмо администратору при создании лида ( )
Уведомление об изменении лида. Доступные переменные: 
#CHANGES_TABLE# - таблица с изменениями лида;
#MODIFIER# - менеджер, внесший изменения в лид; 
#STORE_NAME#, #LEAD_ID#, #NAME#, #PHONE#, #EMAIL#, #ORGANIZATION#, #ORDERTABLE#, #SHIPPINGMETHOD#, #CITY#, #COMMENTS#, #DESCRIPTION#, #DEAL_STATUS#, #DATE#, #MANAGER_NAME#, #ADDITIONALCUSTOMERFIELDS#, #LEADS_LIST#, #SOURCE#, #LEAD_ATTACHMENTS#', 'OnLeadChanged')

SELECT TOP 1 @MailFormatTypeID = scope_identity();

INSERT INTO [Settings].[MailFormat] ([FormatName],[FormatText],[SortOrder],[Enable],[AddDate],[ModifyDate],[FormatSubject],[MailFormatTypeId])
VALUES ('Изменение лида','<div style="color: #4c4f56; font-family: Arial, Helvetica, sans-serif; font-size: 14px;">
<div class="header" style="border-bottom: 1px solid #ededed; display: table; margin-bottom: 25px; padding-bottom: 25px; width: 100%;">
<div class="logo" style="display: table-cell; text-align: left; vertical-align: middle;">#LOGO#</div>

<div class="phone" style="display: table-cell; text-align: right; vertical-align: middle;">
<div class="tel" style="font-size: 26px; font-weight: bold; line-height: 1; margin-bottom: 5px;">&nbsp;</div>

<div class="inform" style="font-size: 12px;">&nbsp;</div>
</div>
</div>

<div class="o-big-title" style="font-size: 18px; font-weight: bold; margin-bottom: 20px; margin-top: 40px;">Менеджер #MODIFIER# внес следующие изменения в лид №#LEAD_ID#:</div>
#CHANGES_TABLE#

<div style="border-bottom: 1px solid #ededed;">&nbsp;</div>

<div class="o-big-title" style="font-size: 18px; font-weight: bold; margin-bottom: 20px; margin-top: 40px;">Текущая информация по лиду:</div>

<div class="data" style="display: table; width: 100%;">
<div class="data-cell" style="display: table-cell; padding: 0; padding-right: 1%; width: 48%;">
<div class="l-row">
<div class="l-name vi cs-light" style="color: #acacac; display: inline-block; margin: 5px 0; padding-right: 15px; width: 150px; vertical-align: middle;">Email:</div>

<div class="l-value vi" style="display: inline-block; margin: 5px 0;">#EMAIL#</div>
</div>

<div class="l-row">
<div class="l-name vi cs-light" style="color: #acacac; display: inline-block; margin: 5px 0; padding-right: 15px; width: 150px; vertical-align: middle;">Имя:</div>

<div class="l-value vi" style="display: inline-block; margin: 5px 0;">#NAME#</div>
</div>

<div class="l-row">
<div class="l-name vi cs-light" style="color: #acacac; display: inline-block; margin: 5px 0; padding-right: 15px; width: 150px; vertical-align: middle;">Номер телефона:</div>

<div class="l-value vi" style="display: inline-block; margin: 5px 0;">#PHONE#</div>
</div>
</div>
</div>

<div>
<div class="o-big-title" style="font-size: 18px; font-weight: bold; margin-bottom: 20px; margin-top: 40px;">Содержание заказа:</div>
#ORDERTABLE#

<div class="comment" style="margin-top: 15px;"><span class="comment-title" style="font-weight: bold;">Комментарии: </span> <span class="comment-txt" style="padding-left: 5px;"> #COMMENTS# </span></div>
</div>
</div>',1100,1,GetDate(),GetDate(),'Изменен лид № #LEAD_ID#',@MailFormatTypeID)

GO--


ALTER TABLE [CRM].[SalesFunnel] ADD
	[NotSendNotificationsOnLeadChanged] [bit] NULL

GO--

Insert Into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (1, 'Core.Services.EBizProcessEventType.LeadChanged', 'Лид изменен')
Insert Into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (2, 'Core.Services.EBizProcessEventType.LeadChanged', 'Lead changed')
Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (1, 'Core.Services.CMS.LeadChangedNotification.Title', 'Лид изменен'); 
Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (2, 'Core.Services.CMS.LeadChangedNotification.Title', 'Lead changed'); 
Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (1, 'Core.Services.CMS.LeadChangedNotification.Body', 'Лид №{0}'); 
Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (2, 'Core.Services.CMS.LeadChangedNotification.Body', 'Lead #{0}'); 

GO--

ALTER PROCEDURE [Settings].[sp_GetCsvProducts] @exportFeedId       INT, 
                                               @onlyCount          BIT, 
                                               @exportNoInCategory BIT, 
                                               @exportAllProducts  BIT, 
                                               @exportNotAvailable BIT 
AS 
  BEGIN 
      DECLARE @res TABLE (productid INT PRIMARY KEY CLUSTERED); 
      DECLARE @lproductNoCat TABLE (productid INT PRIMARY KEY CLUSTERED); 



      IF ( @exportNoInCategory = 1 ) 
        BEGIN 
            INSERT INTO @lproductNoCat 
				SELECT [productid] 
				FROM   [Catalog].product 
				WHERE  [productid] NOT IN (SELECT [productid] FROM   [Catalog].[productcategories]); 
        END 

      DECLARE @lcategory TABLE (categoryid INT PRIMARY KEY CLUSTERED); 
      DECLARE @l TABLE (categoryid INT PRIMARY KEY CLUSTERED, Opened bit); 

      INSERT INTO @l 
      SELECT t.categoryid, t.Opened
      FROM   [Settings].[exportfeedselectedcategories] AS t 
             INNER JOIN catalog.category ON t.categoryid = category.categoryid 
      WHERE  [exportfeedid] = @exportFeedId 

      DECLARE @l1 INT 

      SET @l1 = (SELECT Min(categoryid) FROM @l); 

      WHILE @l1 IS NOT NULL 
        BEGIN 
		if ((Select Opened from @l where CategoryId=@l1)=0)
		begin
            INSERT INTO @lcategory 
				SELECT id 
				FROM   settings.Getchildcategorybyparent(@l1) AS dt 
				INNER JOIN catalog.category ON categoryid = id 
				WHERE  dt.id NOT IN (SELECT categoryid FROM @lcategory) 
		end;

            SET @l1 = (SELECT Min(categoryid) FROM   @l WHERE  categoryid > @l1); 
        END; 

      IF @onlyCount = 1 
        BEGIN 
            SELECT Count(productid) 
            FROM   [Catalog].[product] 
            WHERE  ( EXISTS (SELECT 1 
                             FROM   [Catalog].[productcategories] 
                             WHERE  [productcategories].[productid] = [product].[productid] 
                                    AND [productcategories].categoryid IN (SELECT categoryid FROM @lcategory) 
							 ) 
                      OR EXISTS (SELECT 1 
                                 FROM   @lproductNoCat AS TEMP 
                                 WHERE  TEMP.productid = [product].[productid]) 
                   ) 
                   AND ( @exportAllProducts = 1 
                          OR (SELECT Count(productid) 
                              FROM  settings.exportfeedexcludedproducts 
                              WHERE exportfeedexcludedproducts.productid = product.productid AND exportfeedexcludedproducts.exportfeedid = @exportFeedId) = 0 )
				   AND (Product.Enabled = 1 OR @exportNotAvailable = 1)
				   AND ( @exportNotAvailable = 1
					      OR EXISTS (SELECT 1 
									 FROM [Catalog].[Offer] o 
									 Where o.[ProductId] = [product].[productid] AND o.Price > 0 and o.Amount > 0))
        END 
      ELSE 
        BEGIN 
            SELECT * 
            FROM   [Catalog].[product] 
                   LEFT JOIN [Catalog].[photo] ON [photo].[objid] = [product].[productid] AND type = 'Product' AND photo.[main] = 1 
            WHERE  ( EXISTS (SELECT 1 
                             FROM   [Catalog].[productcategories] 
                             WHERE  [productcategories].[productid] = [product].[productid] 
                                    AND  [productcategories].categoryid IN (SELECT categoryid FROM @lcategory)
							) 
                      OR EXISTS (SELECT 1 
                                 FROM   @lproductNoCat AS TEMP 
                                 WHERE  TEMP.productid = [product].[productid]) 
                   ) 
                   AND ( @exportAllProducts = 1 
                          OR (SELECT Count(productid) 
                              FROM   settings.exportfeedexcludedproducts 
                              WHERE exportfeedexcludedproducts.productid = product.productid AND exportfeedexcludedproducts.exportfeedid = @exportFeedId) = 0 ) 
				   AND (Product.Enabled = 1 OR @exportNotAvailable = 1)
				   AND ( @exportNotAvailable = 1
					      OR EXISTS (SELECT 1 
									 FROM [Catalog].[Offer] o 
									 Where o.[ProductId] = [product].[productid] AND o.Price > 0 and o.Amount > 0))
        END 
  END 

GO--

UPDATE [Settings].[InternalSettings] SET [settingValue] = '7.0.2' WHERE [settingKey] = 'db_version'