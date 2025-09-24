EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Template.UseAdaptiveRootCategoryHint', 'Если настройка <b>включена</b>, то в качестве корневой категории будет использоваться первая по дереву категория, у которой больше одной подкатегории (по умолчанию Каталог), либо первая по дереву категория с товарами.<br/><br/>Настройку рекомендуется <b>выключить</b>, если необходимо выводить в меню единственную категорию первого уровня (с родителем Каталог).'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Template.UseAdaptiveRootCategoryHint', 'If the setting is <b>enabled</b>, then the first category in the tree that has more than one subcategory (by default Catalog) will be used as the root category, or the first category in the tree with goods.<br/><br/>It is recommended to <b>turn off</b> the setting. If you need to display a single first-level category in the menu (with a parent Catalog).'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.ExportImport.MultiOrder.GroupName', 'Группа покупателя'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.ExportImport.MultiOrder.GroupName', 'Customer group'

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Photo.LimitProductPhotos', 'Превышен лимит фотографий товара'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Photo.LimitProductPhotos', 'Product photo limit exceeded'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Photo.FailedDownload', 'Не удалось скачать файл'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Photo.FailedDownload', 'Failed to download file'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.ExportImport.ImportCsv.NotPhotoProcessed', 'Не удалось обработать фотографию "{0}" по причине "{1}"'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.ExportImport.ImportCsv.NotPhotoProcessed', 'The photo "{0}" could not be processed due to "{1}"'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Photo.NotPhotoProcessed', 'Не удалось обработать фотографию "{0}" по причине "{1}"'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Photo.NotPhotoProcessed', 'The photo "{0}" could not be processed due to "{1}"'

GO--

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Catalog].[Warehouse]') AND type in (N'U'))
BEGIN
CREATE TABLE [Catalog].[Warehouse](
    [Id] [int] IDENTITY(1,1) NOT NULL,
    [Name] [nvarchar](255) NULL,
    [UrlPath] [nvarchar](150) NULL,
    [Description] [nvarchar](max) NULL,
    [TypeId] [int] NULL,
    [SortOrder] [int] NOT NULL,
    [Enabled] [bit] NOT NULL,
    [CityId] [int] NULL,
    [Address] [nvarchar](255) NULL,
    [Longitude] [float] NULL,
    [Latitude] [float] NULL,
    [AddressComment] [nvarchar](max) NULL,
    [Phone] [nvarchar](50) NULL,
    [Phone2] [nvarchar](50) NULL,
    [Email] [nvarchar](100) NULL,
    [DateAdded] [datetime] NOT NULL,
    [DateModified] [datetime] NOT NULL,
	[ModifiedBy] [nvarchar](50) NULL,
    CONSTRAINT [PK_Warehouse] PRIMARY KEY CLUSTERED
(
[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
    ) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END

GO--

IF NOT EXISTS (SELECT * FROM sys.triggers WHERE object_id = OBJECT_ID(N'[Catalog].[WarehouseDeleted]'))
EXEC dbo.sp_executesql @statement = N'CREATE TRIGGER [Catalog].[WarehouseDeleted] ON [Catalog].[Warehouse]
WITH EXECUTE AS CALLER
FOR DELETE
AS
BEGIN
	SET NOCOUNT ON;
	
	DELETE FROM [SEO].[MetaInfo] WHERE [ObjId] in (select Id FROM Deleted) and Type=''Warehouse''
	--DELETE FROM [SEO].[RoutingParams] WHERE [Paramvalue] in (select Id FROM Deleted) and ParamName = ''Warehouse''
    -- Insert statements for trigger here
END
' 

GO--

ALTER TABLE [Catalog].[Warehouse] ENABLE TRIGGER [WarehouseDeleted]

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Catalog.Index.WarehouseTitle', 'Склад';
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Catalog.Index.WarehouseTitle', 'Warehouse';
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Catalog.WarehouseStock.Quantity', 'Количество модификации';
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Catalog.WarehouseStock.Quantity', 'Amount of offer';
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.ExportImport.EProductField.WarehouseStock', 'Количество на складе';
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.ExportImport.EProductField.WarehouseStock', 'Quantity in warehouse';
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.ExportImport.ImportCsvV2.WrongWarehouseStocksHeader', 'Некорректный заголовок "Количество на складе" в колонке {0}';
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.ExportImport.ImportCsvV2.WrongWarehouseStocksHeader', 'Wrong "Quantity in warehouse" header in column {0}';
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Catalog.ShowWarehouseFilter', 'Фильтр по складу';
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Catalog.ShowWarehouseFilter', 'Warehouse filter';
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsCatalog.CatalogCommon.FilterByWarehouse', 'Выводить или нет фильтр по складу.';
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsCatalog.CatalogCommon.FilterByWarehouse', 'To output or not the filter on warehouses.';
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Catalog.FilterWarehouse.Warehouses', 'Склад';
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Catalog.FilterWarehouse.Warehouses', 'Warehouse';
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Product.AvailableInWarehouse', ' на {0} складах';
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Product.AvailableInWarehouse', 'in {0} warehouses';
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Catalog.ShowAvailableInWarehouseInProduct', 'Отображать наличие на складах';
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Catalog.ShowAvailableInWarehouseInProduct', 'Display stock in warehouses';
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Catalog..ShowAvailableInWarehouseInProductHelp', 'Отображать или нет количество складов в графе "наличие" у товара.<br><br>Например:<br>При включённой опции - В наличии на 3 складах (100 шт.)<br>При отключенной опции - В наличии (100 шт.)';
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Catalog..ShowAvailableInWarehouseInProductHelp', 'Show or not number of warehouses in the "availability" field of the product.<br><br>For example:<br>With the enabled option - In stock in 3 warehouses (100 pcs.)<br>With the option disabled - In stock (100 pcs.).';


GO--

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Catalog].[TypeWarehouse]') AND type in (N'U'))
BEGIN
CREATE TABLE [Catalog].[TypeWarehouse](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](255) NULL,
	[SortOrder] [int] NOT NULL,
	[Enabled] [bit] NOT NULL,
	[DateAdded] [datetime] NOT NULL,
	[DateModified] [datetime] NOT NULL,
 CONSTRAINT [PK_TypeWarehouse] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END

GO--

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[Catalog].[FK_Warehouse_TypeWarehouse]') AND parent_object_id = OBJECT_ID(N'[Catalog].[Warehouse]'))
ALTER TABLE [Catalog].[Warehouse]  WITH CHECK ADD  CONSTRAINT [FK_Warehouse_TypeWarehouse] FOREIGN KEY([TypeId])
REFERENCES [Catalog].[TypeWarehouse] ([Id])
ON DELETE SET NULL

GO--

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[Catalog].[FK_Warehouse_TypeWarehouse]') AND parent_object_id = OBJECT_ID(N'[Catalog].[Warehouse]'))
ALTER TABLE [Catalog].[Warehouse] CHECK CONSTRAINT [FK_Warehouse_TypeWarehouse]

GO--

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Catalog].[WarehouseStocks]') AND type in (N'U'))
BEGIN
CREATE TABLE [Catalog].[WarehouseStocks](
	[OfferId] [int] NOT NULL,
	[WarehouseId] [int] NOT NULL,
	[Quantity] [float] NOT NULL,
 CONSTRAINT [PK_WarehouseStocks] PRIMARY KEY CLUSTERED 
(
	[OfferId] ASC,
	[WarehouseId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END

GO--

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[Catalog].[FK_WarehouseStocks_Offer]') AND parent_object_id = OBJECT_ID(N'[Catalog].[WarehouseStocks]'))
ALTER TABLE [Catalog].[WarehouseStocks]  WITH CHECK ADD  CONSTRAINT [FK_WarehouseStocks_Offer] FOREIGN KEY([OfferId])
REFERENCES [Catalog].[Offer] ([OfferID])
ON DELETE CASCADE

GO--

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[Catalog].[FK_WarehouseStocks_Offer]') AND parent_object_id = OBJECT_ID(N'[Catalog].[WarehouseStocks]'))
ALTER TABLE [Catalog].[WarehouseStocks] CHECK CONSTRAINT [FK_WarehouseStocks_Offer]

GO--

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[Catalog].[FK_WarehouseStocks_Warehouse]') AND parent_object_id = OBJECT_ID(N'[Catalog].[WarehouseStocks]'))
ALTER TABLE [Catalog].[WarehouseStocks]  WITH CHECK ADD  CONSTRAINT [FK_WarehouseStocks_Warehouse] FOREIGN KEY([WarehouseId])
REFERENCES [Catalog].[Warehouse] ([Id])

GO--

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[Catalog].[FK_WarehouseStocks_Warehouse]') AND parent_object_id = OBJECT_ID(N'[Catalog].[WarehouseStocks]'))
ALTER TABLE [Catalog].[WarehouseStocks] CHECK CONSTRAINT [FK_WarehouseStocks_Warehouse]

GO--

SET IDENTITY_INSERT [Catalog].[TypeWarehouse] ON 

IF NOT EXISTS (SELECT * FROM [Catalog].[TypeWarehouse] WHERE [Id] = 1)
INSERT [Catalog].[TypeWarehouse] ([Id], [Name], [SortOrder], [Enabled], [DateAdded], [DateModified]) VALUES (1, N'Склады', 0, 1, GETDATE(), GETDATE())

SET IDENTITY_INSERT [Catalog].[TypeWarehouse] OFF

GO--

SET IDENTITY_INSERT [Catalog].[Warehouse] ON 

IF NOT EXISTS (SELECT * FROM [Catalog].[Warehouse] WHERE [Id] = 1)
INSERT [Catalog].[Warehouse] ([Id], [Name], [UrlPath], [Description], [TypeId], [SortOrder], [Enabled], [CityId], [Address], [Longitude], [Latitude], [AddressComment], [Phone], [Phone2], [Email], [DateAdded], [DateModified], [ModifiedBy]) VALUES (1, N'Основной', N'osnovnoi', N'', 1, 1000, 1, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, GETDATE(), GETDATE(), NULL)

SET IDENTITY_INSERT [Catalog].[Warehouse] OFF

GO--

IF NOT EXISTS (SELECT * FROM [Catalog].[WarehouseStocks])
INSERT INTO [Catalog].[WarehouseStocks] ([OfferId],[WarehouseId],[Quantity])
SELECT [OfferID], 1, [Amount]
FROM [Catalog].[Offer]

GO--

IF NOT EXISTS (SELECT * FROM [Settings].[Settings] WHERE [Name] = N'DefaultWarehouse')
INSERT INTO [Settings].[Settings] ([Name],[Value]) VALUES (N'DefaultWarehouse', '1')

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Catalog.Offer.Amount', 'Итоговое количество модификации';
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Catalog.Offer.Amount', 'Final amount of offer';

GO--

ALTER PROCEDURE [Catalog].[sp_UpdateOffer]  
   @OfferID int,  
   @ProductID int,  
   @ArtNo nvarchar(100),  
   --@Amount float,  
   @Price float,  
   @SupplyPrice float,  
   @ColorID int,  
   @SizeID int,  
   @Main bit,
   @Weight float,  
   @Length float,  
   @Width float,  
   @Height float,
   @BarCode nvarchar(50)
AS  
BEGIN  
  UPDATE [Catalog].[Offer]  
  SET     
      [ProductID] = @ProductID
	 ,ArtNo=@ArtNo  
     --,[Amount] = @Amount  
     ,[Price] = @Price  
     ,[SupplyPrice] = @SupplyPrice  
     ,[ColorID] = @ColorID  
     ,[SizeID] = @SizeID  
     ,Main = @Main
	 ,Weight = @Weight
	 ,Length = @Length
	 ,Width = @Width
	 ,Height = @Height
	 ,BarCode = @BarCode
  WHERE [OfferID] = @OfferID  
END

GO--

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Order].[ShippingWarehouse]') AND type in (N'U'))
BEGIN
CREATE TABLE [Order].[ShippingWarehouse](
	[MethodId] [int] NOT NULL,
	[WarehouseId] [int] NOT NULL,
 CONSTRAINT [PK_ShippingWarehouse] PRIMARY KEY CLUSTERED 
(
	[MethodId] ASC,
	[WarehouseId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END

GO--

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[Order].[FK_ShippingWarehouse_ShippingMethod]') AND parent_object_id = OBJECT_ID(N'[Order].[ShippingWarehouse]'))
ALTER TABLE [Order].[ShippingWarehouse]  WITH CHECK ADD  CONSTRAINT [FK_ShippingWarehouse_ShippingMethod] FOREIGN KEY([MethodId])
REFERENCES [Order].[ShippingMethod] ([ShippingMethodID])
ON DELETE CASCADE

GO--

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[Order].[FK_ShippingWarehouse_ShippingMethod]') AND parent_object_id = OBJECT_ID(N'[Order].[ShippingWarehouse]'))
ALTER TABLE [Order].[ShippingWarehouse] CHECK CONSTRAINT [FK_ShippingWarehouse_ShippingMethod]

GO--

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[Order].[FK_ShippingWarehouse_Warehouse]') AND parent_object_id = OBJECT_ID(N'[Order].[ShippingWarehouse]'))
ALTER TABLE [Order].[ShippingWarehouse]  WITH CHECK ADD  CONSTRAINT [FK_ShippingWarehouse_Warehouse] FOREIGN KEY([WarehouseId])
REFERENCES [Catalog].[Warehouse] ([Id])
ON DELETE CASCADE

GO--

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[Order].[FK_ShippingWarehouse_Warehouse]') AND parent_object_id = OBJECT_ID(N'[Order].[ShippingWarehouse]'))
ALTER TABLE [Order].[ShippingWarehouse] CHECK CONSTRAINT [FK_ShippingWarehouse_Warehouse]

GO--

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Order].[WarehouseOrderItem]') AND type in (N'U'))
BEGIN
CREATE TABLE [Order].[WarehouseOrderItem](
	[OrderItemId] [int] NOT NULL,
	[WarehouseId] [int] NOT NULL,
	[Amount] [float] NOT NULL,
	[DecrementedAmount] [float] NOT NULL,
 CONSTRAINT [PK_WarehouseOrderItem] PRIMARY KEY CLUSTERED 
(
	[OrderItemId] ASC,
	[WarehouseId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END

GO--

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[Order].[FK_WarehouseOrderItem_OrderItems]') AND parent_object_id = OBJECT_ID(N'[Order].[WarehouseOrderItem]'))
ALTER TABLE [Order].[WarehouseOrderItem]  WITH CHECK ADD  CONSTRAINT [FK_WarehouseOrderItem_OrderItems] FOREIGN KEY([OrderItemId])
REFERENCES [Order].[OrderItems] ([OrderItemID])
ON DELETE CASCADE

GO--

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[Order].[FK_WarehouseOrderItem_OrderItems]') AND parent_object_id = OBJECT_ID(N'[Order].[WarehouseOrderItem]'))
ALTER TABLE [Order].[WarehouseOrderItem] CHECK CONSTRAINT [FK_WarehouseOrderItem_OrderItems]

GO--

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[Order].[FK_WarehouseOrderItem_Warehouse]') AND parent_object_id = OBJECT_ID(N'[Order].[WarehouseOrderItem]'))
ALTER TABLE [Order].[WarehouseOrderItem]  WITH CHECK ADD  CONSTRAINT [FK_WarehouseOrderItem_Warehouse] FOREIGN KEY([WarehouseId])
REFERENCES [Catalog].[Warehouse] ([Id])
ON DELETE CASCADE

GO--

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[Order].[FK_WarehouseOrderItem_Warehouse]') AND parent_object_id = OBJECT_ID(N'[Order].[WarehouseOrderItem]'))
ALTER TABLE [Order].[WarehouseOrderItem] CHECK CONSTRAINT [FK_WarehouseOrderItem_Warehouse]

GO--

IF NOT EXISTS (SELECT * FROM [Order].[WarehouseOrderItem])
INSERT INTO [Order].[WarehouseOrderItem] ([OrderItemId],[WarehouseId],[Amount],[DecrementedAmount])
SELECT [OrderItemID], 1, [Amount], [DecrementedAmount]
FROM [Order].[OrderItems]

GO--

ALTER PROCEDURE [Order].[sp_UpdateOrderItem]  
	@OrderItemID int,  
	@OrderID int,  
	@Name nvarchar(255),  
	@Price float,  
	@Amount float,  
	@ProductID int,  
	@ArtNo nvarchar(100),  
	@BarCode nvarchar(50),
	@SupplyPrice float,  
	@Weight float,  
	@IsCouponApplied bit,  
	@Color nvarchar(50),  
	@Size nvarchar(50),  
	--@DecrementedAmount float,  
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
	@Length float,
	@PaymentMethodType int,
	@PaymentSubjectType int,
	@IsGift bit,
	@IsCustomPrice bit,
	@BasePrice float,
	@DiscountPercent float,
	@DiscountAmount float,
    @DoNotApplyOtherDiscounts bit,
	@Unit nvarchar(50),
	@MeasureType tinyint,
	@IsByCoupon bit
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
    --,[DecrementedAmount] = DecrementedAmount  
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
	,PaymentMethodType = @PaymentMethodType
	,PaymentSubjectType = @PaymentSubjectType
	,IsGift = @IsGift
	,[BarCode] = @BarCode
	,IsCustomPrice = @IsCustomPrice
	,BasePrice = @BasePrice
	,DiscountPercent = @DiscountPercent
	,DiscountAmount = @DiscountAmount
    ,DoNotApplyOtherDiscounts = @DoNotApplyOtherDiscounts
    ,Unit = @Unit
    ,MeasureType = @MeasureType
    ,IsByCoupon = @IsByCoupon
 Where OrderItemID = @OrderItemID  
END 

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Product.ProductTabs.Stocks', 'Наличие';
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Product.ProductTabs.Stocks', 'Stocks';

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Orders.GetOrderItems.AvailableLimit', 'не хватает {0} ед.';
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Orders.GetOrderItems.AvailableLimit', 'missing {0} units';

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Orders.GetOrderItems.WarehouseAvailableLimit', 'на складе "{0}" не хватает {1} ед.';
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Orders.GetOrderItems.WarehouseAvailableLimit', 'missing {1} units in warehouse "{0}"';

GO--

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Catalog].[WarehouseTimeOfWork]') AND type in (N'U'))
BEGIN
CREATE TABLE [Catalog].[WarehouseTimeOfWork](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[WarehouseId] [int] NOT NULL,
	[DayOfWeeks] [tinyint] NOT NULL,
	[OpeningTime] [smallint] NULL,
	[ClosingTime] [smallint] NULL,
	[BreakStartTime] [smallint] NULL,
	[BreakEndTime] [smallint] NULL,
 CONSTRAINT [PK_WarehouseTimeOfWork] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END

GO--

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[Catalog].[WarehouseTimeOfWork]') AND name = N'WarehouseId_WarehouseTimeOfWork')
CREATE NONCLUSTERED INDEX [WarehouseId_WarehouseTimeOfWork] ON [Catalog].[WarehouseTimeOfWork]
(
	[WarehouseId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO--

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[Catalog].[FK_WarehouseTimeOfWork_Warehouse]') AND parent_object_id = OBJECT_ID(N'[Catalog].[WarehouseTimeOfWork]'))
ALTER TABLE [Catalog].[WarehouseTimeOfWork]  WITH CHECK ADD  CONSTRAINT [FK_WarehouseTimeOfWork_Warehouse] FOREIGN KEY([WarehouseId])
REFERENCES [Catalog].[Warehouse] ([Id])
ON DELETE CASCADE

GO--

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[Catalog].[FK_WarehouseTimeOfWork_Warehouse]') AND parent_object_id = OBJECT_ID(N'[Catalog].[WarehouseTimeOfWork]'))
ALTER TABLE [Catalog].[WarehouseTimeOfWork] CHECK CONSTRAINT [FK_WarehouseTimeOfWork_Warehouse]

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'AdvantShop.Core.Services.Catalog.Warehouses.TimeOfWork.Weekend', 'выходной';
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'AdvantShop.Core.Services.Catalog.Warehouses.TimeOfWork.Weekend', 'closed';

GO--

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Catalog].[StockLabel]') AND type in (N'U'))
BEGIN
CREATE TABLE [Catalog].[StockLabel](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[ClientName] [nvarchar](50) NULL,
	[AmountUpTo] [float] NOT NULL,
	[Color] [nvarchar](10) NULL,
 CONSTRAINT [PK_StockLabel] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Catalog.YaMapsApiKeyFoMapWarehouse', 'API-ключ Яндекс.Карт для отображения карты';
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Catalog.YaMapsApiKeyFoMapWarehouse', 'Yandex.maps API key for displaying the map';
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Catalog.YaMapsApiKeyFoMapWarehouseHelp', '<a href="https://yandex.ru/dev/maps/jsapi/doc/2.1/quick-start/index.html#get-api-key" target="_blank">Как получить ключ?</a>';
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Catalog.YaMapsApiKeyFoMapWarehouseHelp', '<a href="https://yandex.ru/dev/maps/jsapi/doc/2.1/quick-start/index.html#get-api-key" target="_blank">How do I get the key?</a>';

GO--

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Order].[sp_DecrementProductsCountAccordingOrder]') AND type in (N'P', N'PC'))
DROP PROCEDURE [Order].[sp_DecrementProductsCountAccordingOrder]

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Warehouse.AddWarehouse.Error.ExceedingSaasValue', 'Превышение лимита тарифа';
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Warehouse.AddWarehouse.Error.ExceedingSaasValue', 'Exceeding the tariff limit';

GO--

IF NOT EXISTS(SELECT 1
              FROM sys.columns
              WHERE (name = N'Warehouses') AND object_id = OBJECT_ID(N'[Order].[OrderPickPoint]'))
BEGIN
	ALTER TABLE [Order].OrderPickPoint ADD
		[Warehouses] nvarchar(MAX) NULL
END

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.AddDeliveryZone.Warehouses', 'Контролировать наличие по складам';
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.AddDeliveryZone.Warehouses', 'Control availability in warehouses';
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.AddDeliveryZone.WarehousesEmpty', 'Нет';
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.AddDeliveryZone.WarehousesEmpty', 'No';

GO--

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Catalog].[PriceRule_Warehouse]') AND type in (N'U'))
BEGIN
	CREATE TABLE [Catalog].[PriceRule_Warehouse](
		[PriceRuleId] [int] NOT NULL,
		[WarehouseId] [int] NOT NULL,
	CONSTRAINT [PK_PriceRule_Warehouse] PRIMARY KEY CLUSTERED 
	(
		[PriceRuleId] ASC,
		[WarehouseId] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]
END

GO--

IF NOT EXISTS (SELECT * FROM sys.foreign_keys 
				WHERE object_id = OBJECT_ID(N'[Catalog].[FK_PriceRule_Warehouse_PriceRule]') AND parent_object_id = OBJECT_ID(N'[Catalog].[PriceRule_Warehouse]'))
BEGIN
	ALTER TABLE [Catalog].[PriceRule_Warehouse]  WITH CHECK ADD  CONSTRAINT [FK_PriceRule_Warehouse_PriceRule] FOREIGN KEY([PriceRuleId])
		REFERENCES [Catalog].[PriceRule] ([Id])
		ON DELETE CASCADE
END

GO--

IF EXISTS (SELECT * FROM sys.foreign_keys 
			WHERE object_id = OBJECT_ID(N'[Catalog].[FK_PriceRule_Warehouse_PriceRule]') AND parent_object_id = OBJECT_ID(N'[Catalog].[PriceRule_Warehouse]'))
BEGIN
	ALTER TABLE [Catalog].[PriceRule_Warehouse] CHECK CONSTRAINT [FK_PriceRule_Warehouse_PriceRule]
END

GO--

IF NOT EXISTS (SELECT * FROM sys.foreign_keys 
				WHERE object_id = OBJECT_ID(N'[Catalog].[FK_PriceRule_Warehouse_Warehouse]') AND parent_object_id = OBJECT_ID(N'[Catalog].[PriceRule_Warehouse]'))
BEGIN
	ALTER TABLE [Catalog].[PriceRule_Warehouse]  WITH CHECK ADD  CONSTRAINT [FK_PriceRule_Warehouse_Warehouse] FOREIGN KEY([WarehouseId])
	REFERENCES [Catalog].[Warehouse] ([Id])
	ON DELETE CASCADE
END

GO--

IF EXISTS (SELECT * FROM sys.foreign_keys 
			WHERE object_id = OBJECT_ID(N'[Catalog].[FK_PriceRule_Warehouse_Warehouse]') AND parent_object_id = OBJECT_ID(N'[Catalog].[PriceRule_Warehouse]'))
BEGIN
	ALTER TABLE [Catalog].[PriceRule_Warehouse] CHECK CONSTRAINT [FK_PriceRule_Warehouse_Warehouse]
END

GO--

UPDATE [Settings].[InternalSettings] SET [settingValue] = '12.0.5' WHERE [settingKey] = 'db_version'
