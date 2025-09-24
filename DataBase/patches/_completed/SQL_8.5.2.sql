INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.Orders.CouponCode', 'Код купона')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.Orders.CouponCode', 'Coupon code')

GO--


INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.MobileVersion.BrowserPanelColor', 'Цвет панели браузера')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.MobileVersion.BrowserPanelColor', 'Browsers panel color')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.MobileVersion.BrowserPanelColorHint', 'Выберите цвет панели браузера. Работает только на телефонах с Android.')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.MobileVersion.BrowserPanelColorHint', 'Select the color of the browser panel. Works only on Android phones.')

GO--

declare @RegionIdMove int = (select RegionId from Customers.Region where RegionName = 'Ставрополь')
declare @RegionId int = (select RegionId from Customers.Region where RegionName = 'Ставропольский край')
if @RegionIdMove is not null and @RegionId is not null
begin
	UPDATE Customers.City SET RegionId = @RegionId WHERE RegionId = @RegionIdMove
	UPDATE Customers.Contact SET RegionId = @RegionId WHERE RegionId = @RegionIdMove
	UPDATE [Order].ShippingRegion SET RegionId = @RegionId WHERE RegionId = @RegionIdMove
	UPDATE [Order].ShippingRegionExcluded SET RegionId = @RegionId WHERE RegionId = @RegionIdMove
	DELETE FROM Customers.Region WHERE RegionId = @RegionIdMove
end

UPDATE Customers.Region SET RegionName = 'Республика Крым' WHERE RegionName = 'Крым'

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ShippingMethods.Edost.DemensionsUnit', 'Единицы измерения в edost')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ShippingMethods.Edost.DemensionsUnit', 'Demensions unit in edost')

GO--

Insert Into [Order].[ShippingParam] ([ShippingMethodID],[ParamName],[ParamValue])
Select [ShippingMethod].[ShippingMethodID],'DemensionsUnit', 'Millimeters'  
From [order].[ShippingMethod] 
Where [ShippingType] ='Edost' and not Exists(Select 1 From [order].[ShippingParam] sp Where sp.[ShippingMethodID] = [ShippingMethod].[ShippingMethodID] and sp.[ParamName] = 'DemensionsUnit')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.Tasks.AppointedManager.Automatic', 'Автоматическая задача')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.Tasks.AppointedManager.Automatic', 'Automatic task')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.ExportImport.MultiOrder.DiscountPrice', 'Скидка')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.ExportImport.MultiOrder.DiscountPrice', 'Discount')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.ExportImport.MultiOrder.ShippingCost', 'Доставка')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.ExportImport.MultiOrder.ShippingCost', 'Shipping cost')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.ExportImport.MultiOrder.PaymentCost', 'Наценка оплаты')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.ExportImport.MultiOrder.PaymentCost', 'Payment cost')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.ExportImport.MultiOrder.CouponCost', 'Купон')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.ExportImport.MultiOrder.CouponCost', 'Coupon cost')

GO--

UPDATE [Settings].[Localization] SET [ResourceValue] = 'доступно {0}' WHERE [LanguageId] = '1' AND [ResourceKey] = 'Admin.Orders.GetOrderItems.AvailableLimit' 

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Orders.Order.OrderShortDate', 'Дата заказа (дд.мм.гггг)')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Orders.Order.OrderShortDate', 'Order date (dd.mm.yyyy)')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Orders.Order.TotalWeight', 'Общий вес')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Orders.Order.TotalWeight', 'Total weight')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Orders.Order.TotalDimensions', 'Общие габариты')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Orders.Order.TotalDimensions', 'Total dimensions')

GO--


INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ShippingMethods.Sdek.WithInsurance','Со страховкой')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ShippingMethods.Sdek.WithInsurance','With insurance')

GO--

IF (NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'Customers' AND  TABLE_NAME = 'TaskObserver'))
BEGIN
	CREATE TABLE [Customers].[TaskObserver](
		[TaskId] [int] NOT NULL,
		[ManagerId] [int] NOT NULL,
	 CONSTRAINT [PK_TaskObserver] PRIMARY KEY CLUSTERED 
	(
		[TaskId] ASC,
		[ManagerId] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]
	
	ALTER TABLE [Customers].[TaskObserver]  WITH CHECK ADD  CONSTRAINT [FK_TaskObserver_Managers] FOREIGN KEY([ManagerId])
	REFERENCES [Customers].[Managers] ([ManagerId])
	ON DELETE CASCADE
	
	ALTER TABLE [Customers].[TaskObserver] CHECK CONSTRAINT [FK_TaskObserver_Managers]
	
	ALTER TABLE [Customers].[TaskObserver]  WITH CHECK ADD  CONSTRAINT [FK_TaskObserver_Task] FOREIGN KEY([TaskId])
	REFERENCES [Customers].[Task] ([Id])
	ON DELETE CASCADE
	
	ALTER TABLE [Customers].[TaskObserver] CHECK CONSTRAINT [FK_TaskObserver_Task]
END

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.Tasks.ModalEditTaskCtrl.ObserverChanged', 'Наблюдающий изменен')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.Tasks.ModalEditTaskCtrl.ObserverChanged', 'Observer сhanged')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.Tasks.EditTask.AddObserver', 'Добавить наблюдателя')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.Tasks.EditTask.AddObserver', 'Add observer')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.Tasks.ModalEditTaskCtrl.NoObservers', 'Нет наблюдателей')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.Tasks.ModalEditTaskCtrl.NoObservers', 'No observers')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.Tasks.Tasks.TaskObserver', 'Наблюдатель')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.Tasks.Tasks.TaskObserver', 'Observer')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.Tasks.ModalEditTaskCtrl.Observing', 'Отслеживается вами')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.Tasks.ModalEditTaskCtrl.Observing', 'Tracked by you')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.Tasks.ModalEditTaskCtrl.NotObserving', 'Не отслеживается вами')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.Tasks.ModalEditTaskCtrl.NotObserving', 'Not tracked by you')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.PriceRegulation.ChangeDiscountMsg', 'Для товаров в выбранных категориях была установлена скидка {0}{1}, кроме товаров с нулевой ценой')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.PriceRegulation.ChangeDiscountMsg', 'Discount {0}{1} was set for products in the selected categories, except for products with zero price')

GO--

CREATE TABLE [Customers].[ImapLetter](
	[Id] [nvarchar](max) NOT NULL,
	[Folder] [nvarchar](max) NULL,
	[Subject] [nvarchar](max) NOT NULL,
	[Date] [datetime] NOT NULL,
	[From] [nvarchar](max) NULL,
	[FromEmail] [nvarchar](max) NULL,
	[To] [nvarchar](max) NULL,
	[ToEmail] [nvarchar](max) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO--

if exists(Select 1 From [Settings].[Settings] Where [Name] = 'ImapLastUpdateId')
	Update [Settings].[Settings] Set [Value] = '0' Where [Name] = 'ImapLastUpdateId'
	
GO--
	
UPDATE [Settings].[Localization] SET [ResourceValue] = 'Отображать кнопку "В корзину" в списке товаров' where ResourceKey = 'Admin.Settings.MobileVersion.DisplayShowAddButton' AND [LanguageId] = 1

GO--
INSERT INTO [Settings].[TemplateSettings] ([Template], [Name], [Value]) VALUES ('_default', 'Mobile_CatalogMenuViewMode', 'RootCategories')
INSERT INTO [Settings].[TemplateSettings] ([Template], [Name], [Value]) VALUES ('_default', 'Mobile_CatalogMenuViewMode_Modern', 'RootCategories')

GO--

Insert Into [Settings].[Settings] (Name, Value) Values ('ColorsViewMode', '0')
GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Settings.SettingsCatalog.ColorsViewMode.Icon', 'Иконка')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Settings.SettingsCatalog.ColorsViewMode.Icon', 'Icon')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Settings.SettingsCatalog.ColorsViewMode.Text', 'Текст')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Settings.SettingsCatalog.ColorsViewMode.Text', 'Text')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Settings.SettingsCatalog.ColorsViewMode.IconAndText', 'Иконка и текст')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Settings.SettingsCatalog.ColorsViewMode.IconAndText', 'Icon and text')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Catalog.ColorsViewMode', 'Режим отображения цвета')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Catalog.ColorsViewMode', 'Color view mode')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Catalog.ColorsViewModeHint', 'Вы можете выбрать как показывать цвета: иконка, текст (название цвета) или иконка и название цвета')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Catalog.ColorsViewModeHint', 'You can choose color view mode: icon, text (color''s name) or icon and color''s name')


GO--

UPDATE [Order].[ShippingParam]
    SET [ParamName] = 'TypeInsure'
       ,[ParamValue] = CASE WHEN [ParamValue] = 'True' THEN 0 ELSE 2 END
WHERE ParamName = 'WithInsure' 
AND NOT EXISTS (SELECT 1 FROM [Order].[ShippingParam] WHERE [ShippingParam].ShippingMethodID = ShippingMethodID AND [ParamName] = 'TypeInsure')
AND EXISTS (SELECT * From [Order].[ShippingMethod] WHERE [ShippingMethod].ShippingMethodID = [ShippingParam].ShippingMethodID AND [ShippingMethod].[ShippingType] = 'RussianPost')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Checkout.DisableEditing', 'Запретить редактирование')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Checkout.DisableEditing', 'Disable editing')

GO--

IF NOT EXISTS(SELECT 1 FROM sys.columns WHERE Name = N'CancelForbidden' AND Object_ID = Object_ID(N'[Order].[OrderStatus]'))
BEGIN
	ALTER TABLE [Order].[OrderStatus] ADD
		CancelForbidden bit NOT NULL DEFAULT 0
		
	exec sp_executesql N'UPDATE [Order].[OrderStatus]
		SET CancelForbidden = 1
	WHERE IsCompleted = 1 OR IsCanceled = 1' 
END

GO--

ALTER PROCEDURE [Order].[sp_AddOrderStatus]
	@OrderStatusID int,
	@StatusName nvarchar(50),
	@CommandID int,
	@IsDefault bit,
	@IsCanceled bit,
	@IsCompleted bit,
	@Color nvarchar(10),
	@SortOrder int,
	@Hidden bit,
	@CancelForbidden bit
AS
BEGIN
	declare @hasDefault bit;
	if (select count(orderStatusID) from [Order].[OrderStatus] where isdefault=1) = 1
		set @hasDefault = 1
	else
		set @hasDefault = 0
		
	if (@hasDefault=1 & @IsDefault)
	begin
		update [Order].[OrderStatus] set IsDefault = 0
	end
	
	insert into [Order].[OrderStatus] (StatusName, CommandID, IsDefault, IsCanceled, IsCompleted, Color, SortOrder, Hidden, CancelForbidden) 
							   VALUES (@StatusName, @CommandID, @IsDefault | ~@hasDefault, @IsCanceled, @IsCompleted, @Color, @SortOrder, @Hidden, @CancelForbidden)
	select SCOPE_IDENTITY()	    
END

GO--

ALTER PROCEDURE [Order].[sp_UpdateOrderStatus]
	@OrderStatusID int,
	@StatusName nvarchar(50),
	@CommandID int,
	@IsDefault bit,
	@IsCanceled bit,	
	@IsCompleted bit,	
	@Color nvarchar(10),
	@SortOrder int,
	@Hidden bit,
	@CancelForbidden bit
AS
BEGIN
	declare @hasDefault bit;
	if (select count(orderStatusID) from [Order].[OrderStatus] where isdefault=1 and OrderStatusID<>@OrderStatusID ) = 1
		set @hasDefault = 1
	else
		set @hasDefault = 0

	if (@hasDefault=1 & @IsDefault)
	begin
		update [Order].[OrderStatus] set IsDefault = 0
	end

	update [Order].[OrderStatus]
	SET StatusName = @StatusName, CommandID = @CommandID, IsDefault = @IsDefault | ~@hasDefault,
		IsCanceled = @IsCanceled, IsCompleted=@IsCompleted, Color = @Color, SortOrder = @SortOrder, Hidden=@Hidden,
		CancelForbidden = @CancelForbidden
		Where OrderStatusID = @OrderStatusID
END

GO--

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Заказ отменен' WHERE [LanguageId] = '1' AND [ResourceKey] = 'Admin.Js.OrderStatuses.OrderCancel'
UPDATE [Settings].[Localization] SET [ResourceValue] = 'Заказ отменен' WHERE [LanguageId] = '1' AND [ResourceKey] = 'Admin.Js.AddEditOrderStatus.CancelOrder'
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.AddEditOrderStatus.CancelForbidden', 'Запретить отмену заказа клиентом')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.AddEditOrderStatus.CancelForbidden', 'Cancel forbidden')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.OrderStatuses.CancelForbidden', 'Запрет отмены')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.OrderStatuses.CancelForbidden', 'Cancel forbidden')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.SettingsSeo.Redirects.Errors.RedirectFromRequired', 'Не заполнено поле "Откуда"')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.SettingsSeo.Redirects.Errors.RedirectFromRequired', 'Field "From" required')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.SettingsSeo.Redirects.Errors.RedirectFromExists', 'Редирект с указанной страницы уже настроен')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.SettingsSeo.Redirects.Errors.RedirectFromExists', 'A redirect from the specified page is already configured')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.SettingsSeo.Redirects.Errors.RedirectNotFound', 'Редирект не найден')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.SettingsSeo.Redirects.Errors.RedirectNotFound', 'Redirect not found')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ShippingMethods.Common.Categories','Категории')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ShippingMethods.Common.Categories','Categories')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ShippingMethods.Common.Products','Товары')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ShippingMethods.Common.Products','Products')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ShippingMethods.Common.Empty','Нет')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ShippingMethods.Common.Empty','Empty')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ShippingMethods.Common.CategoriesCount','категории(й)')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ShippingMethods.Common.CategoriesCount','category (s)')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ShippingMethods.Common.ProductsCount','товар(ов)')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ShippingMethods.Common.ProductsCount','product(s)')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ShippingMethods.Common.CategoriesChange','Изменить')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ShippingMethods.Common.CategoriesChange','Change')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ShippingMethods.Common.ProductsChange','Изменить')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ShippingMethods.Common.ProductsChange','Change')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ShippingMethods.Common.CategoriesReset','Сбросить')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ShippingMethods.Common.CategoriesReset','Reset')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ShippingMethods.Common.ProductsReset','Сбросить')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ShippingMethods.Common.ProductsReset','Reset')

GO--

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Order].[ShippingCategoryExcluded]') AND type in (N'U'))
BEGIN
	CREATE TABLE [Order].[ShippingCategoryExcluded](
		[MethodId] [int] NOT NULL,
		[CategoryId] [int] NOT NULL,
	 CONSTRAINT [PK_ShippingCategoryExcluded] PRIMARY KEY CLUSTERED 
	(
		[MethodId] ASC,
		[CategoryId] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]
END

GO--

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Order].[ShippingProductExcluded]') AND type in (N'U'))
BEGIN
	CREATE TABLE [Order].[ShippingProductExcluded](
		[MethodId] [int] NOT NULL,
		[ProductId] [int] NOT NULL,
	 CONSTRAINT [PK_ShippingProductExcluded] PRIMARY KEY CLUSTERED 
	(
		[MethodId] ASC,
		[ProductId] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]
END

GO--

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[Order].[FK_ShippingCategoryExcluded_Category]') AND parent_object_id = OBJECT_ID(N'[Order].[ShippingCategoryExcluded]'))
	ALTER TABLE [Order].[ShippingCategoryExcluded]  WITH CHECK ADD  CONSTRAINT [FK_ShippingCategoryExcluded_Category] FOREIGN KEY([CategoryId])
	REFERENCES [Catalog].[Category] ([CategoryID])
	ON DELETE CASCADE

GO--

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[Order].[FK_ShippingCategoryExcluded_Category]') AND parent_object_id = OBJECT_ID(N'[Order].[ShippingCategoryExcluded]'))
	ALTER TABLE [Order].[ShippingCategoryExcluded] CHECK CONSTRAINT [FK_ShippingCategoryExcluded_Category]

GO--

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[Order].[FK_ShippingCategoryExcluded_ShippingMethod]') AND parent_object_id = OBJECT_ID(N'[Order].[ShippingCategoryExcluded]'))
ALTER TABLE [Order].[ShippingCategoryExcluded]  WITH CHECK ADD  CONSTRAINT [FK_ShippingCategoryExcluded_ShippingMethod] FOREIGN KEY([MethodId])
REFERENCES [Order].[ShippingMethod] ([ShippingMethodID])
ON DELETE CASCADE

GO--

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[Order].[FK_ShippingCategoryExcluded_ShippingMethod]') AND parent_object_id = OBJECT_ID(N'[Order].[ShippingCategoryExcluded]'))
ALTER TABLE [Order].[ShippingCategoryExcluded] CHECK CONSTRAINT [FK_ShippingCategoryExcluded_ShippingMethod]

GO--

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[Order].[FK_ShippingProductExcluded_Product]') AND parent_object_id = OBJECT_ID(N'[Order].[ShippingProductExcluded]'))
ALTER TABLE [Order].[ShippingProductExcluded]  WITH CHECK ADD  CONSTRAINT [FK_ShippingProductExcluded_Product] FOREIGN KEY([ProductId])
REFERENCES [Catalog].[Product] ([ProductId])
ON DELETE CASCADE

GO--

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[Order].[FK_ShippingProductExcluded_Product]') AND parent_object_id = OBJECT_ID(N'[Order].[ShippingProductExcluded]'))
ALTER TABLE [Order].[ShippingProductExcluded] CHECK CONSTRAINT [FK_ShippingProductExcluded_Product]

GO--

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[Order].[FK_ShippingProductExcluded_ShippingMethod]') AND parent_object_id = OBJECT_ID(N'[Order].[ShippingProductExcluded]'))
ALTER TABLE [Order].[ShippingProductExcluded]  WITH CHECK ADD  CONSTRAINT [FK_ShippingProductExcluded_ShippingMethod] FOREIGN KEY([MethodId])
REFERENCES [Order].[ShippingMethod] ([ShippingMethodID])
ON DELETE CASCADE

GO--

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[Order].[FK_ShippingProductExcluded_ShippingMethod]') AND parent_object_id = OBJECT_ID(N'[Order].[ShippingProductExcluded]'))
ALTER TABLE [Order].[ShippingProductExcluded] CHECK CONSTRAINT [FK_ShippingProductExcluded_ShippingMethod]

GO--

DELETE FROM Settings.Localization WHERE ResourceKey in 
('Core.Services.Features.EFeature.CsvV2', 'Core.Services.Features.EFeature.CsvV2.Description', 'Admin.Catalog.Export.ProductsV2', 'Admin.Js.AddExportFeed.Ok', 'Admin.Js.AddExportFeed.Cancel')

UPDATE Settings.Localization SET ResourceValue = 'AdvantShop 1.0' WHERE ResourceKey = 'Core.ExportImport.ExportFeed.CsvType'
UPDATE Settings.Localization SET ResourceValue = 'AdvantShop 2.0' WHERE ResourceKey = 'Core.ExportImport.ExportFeed.CsvV2Type'

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Import.ImportProducts.FileFormat', 'Формат')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Import.ImportProducts.FileFormat', 'Format')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.AddExportFeed.Format', 'Формат')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.AddExportFeed.Format', 'Format')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Import.ImportProducts.InstructionCsv', 'Инструкция. Импорт каталога через CSV в формате')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Import.ImportProducts.InstructionCsv', 'Instruction. Catalog import using csv in format')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.AddExportFeed.InstructionCsv', 'Инструкция. Экспорт каталога в формате')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.AddExportFeed.InstructionCsv', 'Instruction. Export catalog using format')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Services.Features.EFeature.FunnelBlocks', 'Новые блоки в воронках')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Services.Features.EFeature.ProductCategoriesAutoMapping', 'New funnel blocks')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Services.Features.EFeature.FunnelBlocks.Description', 'Позволяет показывать экспериментальные блоки в воронках')

GO--

ALTER TABLE CRM.TriggerAction ADD
	SortOrder int NULL
GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Crm.EOrderFieldType.Time', 'В определенное время')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Crm.EOrderFieldType.Time', 'In certain time')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.SettingsTasks.HHMM', 'чч:мм')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.SettingsTasks.HHMM', 'hh:mm')

GO--

if (Select [Value] From [Settings].[TemplateSettings] Where Name = 'Mobile_HeaderCustomTitle_Modern' and Template = '_default') = ''
	delete from [Settings].[TemplateSettings] Where Name = 'Mobile_HeaderCustomTitle_Modern' and Template = '_default'

GO--

ALTER TABLE [CMS].[LandingColorScheme] ADD [TextColorAlt] nvarchar(max) null

GO--

ALTER TABLE Booking.Booking ADD
	OrderId int NULL

GO--

ALTER TABLE Booking.Booking ADD CONSTRAINT
	FK_Booking_Order FOREIGN KEY
	(
	OrderId
	) REFERENCES [Order].[Order]
	(
	OrderID
	) ON UPDATE  NO ACTION 
	 ON DELETE  SET NULL 

GO--

ALTER TABLE [Order].OrderItems ADD
	BookingServiceId int NULL,
	TypeItem nvarchar(50) NULL

GO--

UPDATE [Order].OrderItems
	SET TypeItem = 'Product'

GO--

ALTER TABLE [Order].OrderItems ALTER COLUMN
	TypeItem nvarchar(50) NOT NULL

GO--

ALTER TABLE [Order].OrderItems ADD CONSTRAINT
	FK_OrderItems_Service FOREIGN KEY
	(
	BookingServiceId
	) REFERENCES Booking.Service
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  SET NULL 

GO--

Insert Into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (1, 'Core.Orders.OrderHistory.DeletedBookingService', 'Удалена услуга бронирования')
Insert Into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (2, 'Core.Orders.OrderHistory.DeletedBookingService', 'Booking service deleted')
Insert Into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (1, 'Core.Orders.OrderHistory.AddedBookingService', 'Добавлена услуга бронирования')
Insert Into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (2, 'Core.Orders.OrderHistory.AddedBookingService', 'Booking service added')
Insert Into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (1, 'Core.ChangeHistories.BookingHistory.AddOrder', 'Создан заказ <a target="_blank" href="orders/edit/{0}">{1}</a>')
Insert Into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (2, 'Core.ChangeHistories.BookingHistory.AddOrder', 'Order <a target="_blank" href="orders/edit/{0}">{1}</a> created')

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
	 @Length float,
	 @PaymentMethodType int,
	 @PaymentSubjectType int,
	 @IsGift bit,
	 @BookingServiceId int,
	 @TypeItem nvarchar(50)
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
	   ,PaymentMethodType
	   ,PaymentSubjectType
	   ,IsGift
	   ,BookingServiceId
	   ,TypeItem
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
	   ,@PaymentMethodType
	   ,@PaymentSubjectType
	   ,@IsGift
	   ,@BookingServiceId
	   ,@TypeItem
   );  
       
 SELECT SCOPE_IDENTITY()  
END  

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
  WHERE orderItems.OrderID = @orderId AND orderItems.TypeItem = 'Product';


  UPDATE [Order].[OrderItems]
  SET decrementedAmount = amount
  WHERE [OrderID] = @orderId AND TypeItem = 'Product'
  
  UPDATE [Order].[Order]
  SET [Decremented] = 1
  WHERE [OrderID] = @orderId
	
END

GO--

ALTER PROCEDURE [Order].[sp_IncrementProductsCountAccordingOrder]	
	@orderId int
AS
BEGIN

	Declare @orderItemID int, @ArtNo nvarchar(50), @Amount float, @DecrementedAmount float;

	DECLARE OrderItemsCursor CURSOR FOR
	SELECT OrderItemID, ArtNo, Amount, DecrementedAmount FROM [Order].[OrderItems] where OrderID=@orderId AND TypeItem = 'Product';

	Open OrderItemsCursor;
	FETCH NEXT FROM OrderItemsCursor  
	into @orderItemID, @ArtNo, @Amount, @DecrementedAmount;

	WHILE @@FETCH_STATUS = 0
	BEGIN
		Update Catalog.Offer set Amount = Amount + @DecrementedAmount
		Where Artno = @artno
		
		FETCH NEXT FROM OrderItemsCursor  
		into @orderItemID, @ArtNo, @Amount, @DecrementedAmount;
	END

	CLOSE OrderItemsCursor
	DEALLOCATE OrderItemsCursor

	update [Order].[OrderItems] set decrementedAmount = 0  WHERE [OrderID] = @orderId AND TypeItem = 'Product'
	UPDATE [Order].[Order] SET [Decremented] = 0 WHERE [OrderID] = @orderId
	
END

GO--

IF NOT EXISTS(SELECT 1 FROM sys.columns WHERE Name = N'CustomOptionsJson' AND Object_ID = Object_ID(N'[Order].LeadItem'))
BEGIN
	ALTER TABLE [Order].LeadItem ADD CustomOptionsJson nvarchar(max) NULL
END

GO--

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Booking].[PaymentDetails]') AND type in (N'U'))
BEGIN
CREATE TABLE [Booking].[PaymentDetails](
	[BookingId] [int] NOT NULL,
	[CompanyName] [nvarchar](255) NOT NULL,
	[INN] [nvarchar](255) NOT NULL,
	[Phone] [nvarchar](20) NULL,
	[Contract] [nvarchar](255) NULL,
 CONSTRAINT [PK_PaymentDetails] PRIMARY KEY CLUSTERED 
(
	[BookingId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END

GO--

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[Booking].[FK_PaymentDetails_Booking]') AND parent_object_id = OBJECT_ID(N'[Booking].[PaymentDetails]'))
	ALTER TABLE [Booking].[PaymentDetails]  WITH CHECK ADD  CONSTRAINT [FK_PaymentDetails_Booking] FOREIGN KEY([BookingId])
	REFERENCES [Booking].[Booking] ([Id])
	ON DELETE CASCADE

GO--

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[Booking].[FK_PaymentDetails_Booking]') AND parent_object_id = OBJECT_ID(N'[Booking].[PaymentDetails]'))
	ALTER TABLE [Booking].[PaymentDetails] CHECK CONSTRAINT [FK_PaymentDetails_Booking]

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Common.Telephony.PhoneMask', '+7(999)999-99-99')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Common.Telephony.PhoneMask', '')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.ImportColors.Import', 'Импорт цветов')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.ImportColors.Import', 'Import colors')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.ImportColors.Cancel', 'Отмена')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.ImportColors.Cancel', 'Cancel')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Colors.Index.Import', 'Импорт')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Colors.Index.Import', 'Import')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Colors.Index.Export', 'Экспорт')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Colors.Index.Export', 'Export')

GO--

Insert Into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (1, 'Admin.Settings.Catalog.PriceRegulation.AbsoluteValueHelp', 'Если выбрана скидка в абсоютном числе - скидка будет выставлена в валюте товара, если у вас товары в разных валютах, то цена будет изменена именно в валюте товара.')
Insert Into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (2, 'Admin.Settings.Catalog.PriceRegulation.AbsoluteValueHelp', 'If you choose a discount in the absolute number, the discount will be set in the currency of the goods, if you have goods in different currencies, then the price will be changed in the currency of the goods.')
Insert Into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (1, 'Admin.Settings.PriceRegulation.ChangedInProductCurrency', 'в валюте товара')
Insert Into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (2, 'Admin.Settings.PriceRegulation.ChangedInProductCurrency', 'in product currency')

GO--

ALTER TABLE Booking.Affiliate ADD
	CancelUnpaidViaMinutes int NULL
	
GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Booking.Settings.Payment','Оплата')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Booking.Settings.Payment','Payment')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Booking.Settings.SmsNotification.CancelUnpaidViaMinutes','Отмена неоплаченной брони через (минут)')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Booking.Settings.SmsNotification.CancelUnpaidViaMinutes','Cancel an unpaid booking via мinutes')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Booking.Settings.SmsNotification.Minutes','минут')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Booking.Settings.SmsNotification.Minutes','мinutes')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Booking.BookingCanceled','Отмена брони')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Booking.BookingCanceled','Canceling booking')

GO--

INSERT INTO [Settings].[TemplateSettings] ([Template], [Name], [Value]) VALUES ('_default', 'Mobile_BlockProductPhotoHeight', '180')
INSERT INTO [Settings].[TemplateSettings] ([Template], [Name], [Value]) VALUES ('_default', 'Mobile_BlockProductPhotoHeight_Modern', '180')

GO--

Update [Catalog].[Color] Set ColorName = ColorName + Cast(ColorId as nvarchar(max))
Where [Color].ColorId in (
	Select c.ColorId from [Catalog].[Color] as c 
	Where (Select Count(*) From Catalog.Color as c2 Where c2.ColorName = c.ColorName) > 1
	and c.ColorId <> (Select top(1)c3.ColorId From Catalog.Color as c3 Where c3.ColorName = c.ColorName)
)

GO--

ALTER TABLE Catalog.Color ADD CONSTRAINT
	IX_ColorName UNIQUE NONCLUSTERED 
	(
	ColorName
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO--

Insert Into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (1,'Admin.Settings.System.AdminCssEditor', 'CSS админки')
Insert Into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (2,'Admin.Settings.System.AdminCssEditor', 'CSS of admin panel')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.System.AdminCssEditor.Attention','Внимание! Используйте дополнительные стили с осторожностью.')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.System.AdminCssEditor.Attention','Attention! Use extra styles with caution.')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.System.AdminCssEditor.UseFile',' Используйте файл, только если вы хорошо владеете навыками работы с CSS.')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.System.AdminCssEditor.UseFile','Use the file only if you are good at working with CSS.')

GO--

INSERT INTO [Settings].[TemplateSettings] ([Template], [Name], [Value]) VALUES ('_default', 'Mobile_AutoRedirect', 'True')
INSERT INTO [Settings].[TemplateSettings] ([Template], [Name], [Value]) VALUES ('_default', 'Mobile_AutoRedirect_Modern', 'True')

GO--

Insert Into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (1,'Admin.Orders.OrderInfo.Contacts', 'Контакты')
Insert Into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (2,'Admin.Orders.OrderInfo.Contacts', 'Contacts')

GO--

UPDATE [Settings].[InternalSettings] SET [settingValue] = '8.5.2' WHERE [settingKey] = 'db_version'
