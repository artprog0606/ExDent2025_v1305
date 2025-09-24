UPDATE [Settings].[Localization] Set [ResourceValue] = 'Сортировка' Where [ResourceKey] = 'Catalog.Sorting.SortBy' and [LanguageId] = 1
UPDATE [Settings].[Localization] Set [ResourceValue] = 'Sorting' Where [ResourceKey] = 'Catalog.Sorting.SortBy' and [LanguageId] = 2

GO--


ALTER PROCEDURE [Order].[sp_GetCustomerOrderHistory]  
 @CustomerID uniqueidentifier  
AS  
BEGIN  
 SELECT   
 [Order].[Order].OrderID,   
    [Order].[Order].Number,   
    [Order].[Order].OrderDiscount,   
    [Order].[OrderStatus].StatusName,  
    [Order].PreviousStatus,  
    [Order].[OrderStatus].OrderStatusID,  
    [Order].[Order].Sum,   
    [Order].[Order].OrderDate,   
    [Order].[Order].PaymentDate,  
    [Order].[Order].PaymentMethodName,  
    [Order].[Order].ShippingMethodName,  
    [Order].[ShippingMethod].Name as ShippingMethod,   
    [Order].[PaymentMethodID],      
	[Order].[ManagerID],    
	([Customer].[FirstName] + ' ' +[Customer].[LastName]) as ManagerName,    
    [OrderCurrency].CurrencyCode,  
    [OrderCurrency].CurrencyNumCode,  
    [OrderCurrency].CurrencyValue,  
    [OrderCurrency].CurrencySymbol,  
    [OrderCurrency].IsCodeBefore  
    FROM [Order].[Order]   
    LEFT JOIN [Order].OrderStatus ON [Order].[Order].OrderStatusID = [Order].OrderStatus.OrderStatusID   
    INNER JOIN [Order].[OrderCurrency] ON [Order].[Order].OrderID = [Order].[OrderCurrency].OrderID   
    LEFT JOIN [Order].[ShippingMethod] ON [Order].[Order].ShippingMethodID = [Order].[ShippingMethod].ShippingMethodID   
    INNER JOIN [Order].[OrderCustomer] ON [Order].[Order].OrderID = [Order].[OrderCustomer].OrderID  
	LEFT JOIN [Customers].[Managers] ON [Order].[Order].ManagerId = [Customers].[Managers].ManagerId  
	LEFT JOIN [Customers].[Customer] ON [Customers].[Managers].CustomerId = [Customers].[Customer].CustomerID  
    
	WHERE [Order].[OrderCustomer].CustomerID = @CustomerID and [IsDraft] = 0
    ORDER BY [Order].[Order].OrderDate DESC  
END

GO--

Update [Settings].[Localization] 
Set ResourceValue = 'Шаблон письма ответа (#FIRSTNAME#, #LASTNAME#, #PATRONYMIC#, #STORE_NAME#, #MANAGER_NAME#, #LAST_ORDER_NUMBER#)'
where ResourceKey = 'Admin.Js.AddEditMailAnswerTemplate.Template' and LanguageId = 1

Update [Settings].[Localization] 
Set ResourceValue = 'Response mail template (#FIRSTNAME#, #LASTNAME#, #PATRONYMIC#, #STORE_NAME#, #MANAGER_NAME#, #LAST_ORDER_NUMBER#)'
where ResourceKey = 'Admin.Js.AddEditMailAnswerTemplate.Template' and LanguageId = 2

GO--

DELETE  FROM [Order].[OrderConfirmation]
WHERE OrderConfirmationData like '%AnonymousType%' and OrderConfirmationData like '%AdvantShop.Shipping.Shiptor.ShiptorWidgetOption%'

GO--

update Catalog.Product set YandexProductDiscountCondition = null where YandexProductDiscounted = 0

GO--

update Catalog.Product set YandexProductDiscountReason = SUBSTRING(YandexProductDiscountReason, 1, 3000) where len(YandexProductDiscountReason) > 3000

GO--

alter table Catalog.Product alter column YandexProductDiscountReason nvarchar(3000)

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
    @ManualRatio float,
    @YandexProductDiscounted bit,
	@YandexProductDiscountCondition nvarchar(10),
	@YandexProductDiscountReason nvarchar(3000)
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
        ,[YandexProductDiscounted] = @YandexProductDiscounted
		,[YandexProductDiscountCondition] = @YandexProductDiscountCondition
		,[YandexProductDiscountReason] = @YandexProductDiscountReason
    WHERE ProductID = @ProductID
END

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
    @ManualRatio float,
    @YandexProductDiscounted bit,
	@YandexProductDiscountCondition nvarchar(10),
	@YandexProductDiscountReason nvarchar(3000)
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
        ,YandexProductDiscounted
		,YandexProductDiscountCondition
		,YandexProductDiscountReason
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
        ,@YandexProductDiscounted
		,@YandexProductDiscountCondition
		,@YandexProductDiscountReason
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

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.ExportImport.ProductFields.YandexDiscounted','Яндекс.Маркет: Уцененный товар')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.ExportImport.ProductFields.YandexDiscounted','Yandex Market: Discounted')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.ExportImport.ProductFields.YandexDiscountCondition','Яндекс.Маркет: Состояние товара')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.ExportImport.ProductFields.YandexDiscountCondition','Yandex Market: Discount condition')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.ExportImport.ProductFields.YandexDiscountReason','Яндекс.Маркет: Причина уценки')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.ExportImport.ProductFields.YandexDiscountReason','Yandex Market: Discount reason')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.ExportImport.EProductField.YandexDiscounted','Яндекс.Маркет: Уцененный товар')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.ExportImport.EProductField.YandexDiscounted','Yandex Market: Discounted')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.ExportImport.EProductField.YandexDiscountCondition','Яндекс.Маркет: Состояние товара')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.ExportImport.EProductField.YandexDiscountCondition','Yandex Market: Discount condition')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.ExportImport.EProductField.YandexDiscountReason','Яндекс.Маркет: Причина уценки')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.ExportImport.EProductField.YandexDiscountReason','Yandex Market: Discount reason')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Product.ExportOptions.Error.NotFound','Товар не найден')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Product.ExportOptions.Error.NotFound','Product not found')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Product.ExportOptions.Error.DiscountConditionRequired','Укажите состояние товара')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Product.ExportOptions.Error.DiscountConditionRequired','Markdown condition required')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Product.ExportOptions.Error.DiscountReasonRequired','Укажите причину уценки')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Product.ExportOptions.Error.DiscountReasonRequired','Markdown reason required')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Product.ExportOptions.Error.DiscountReasonLength','Длина текста причины уценки не должна превышать 3000 символов')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Product.ExportOptions.Error.DiscountReasonLength','The text of the markdown reason should be no longer than 3000 characters')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Catalog.EYandexDiscountCondition.None','Не выбрано')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Catalog.EYandexDiscountCondition.None','Not selected')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Catalog.EYandexDiscountCondition.LikeNew','Как новый')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Catalog.EYandexDiscountCondition.LikeNew','Like new')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Catalog.EYandexDiscountCondition.Used','Подержанный')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Catalog.EYandexDiscountCondition.Used','Used')
delete from Settings.Localization where ResourceKey = 'Admin.Js.Product.YandexProductDiscountReason.Lengthy'
delete from Settings.Localization where ResourceKey = 'Admin.Js.ExportOptions.YandexProductDiscountCondition.LikeNew'
delete from Settings.Localization where ResourceKey = 'Admin.Js.ExportOptions.YandexProductDiscountCondition.Used'

GO--

IF (NOT EXISTS(SELECT * FROM [CMS].[StaticBlock] WHERE [Key] = 'ordersuccess-withgiftcertificate'))
BEGIN
   INSERT INTO [CMS].[StaticBlock] ([Key],[InnerName],[Content],[Added],[Modified],[Enabled]) 
   VALUES ('ordersuccess-withgiftcertificate', 'Успешное оформление заказа с примененным сертификатом', '', getdate(), getdate(), 1)
END

IF (NOT EXISTS(SELECT * FROM [CMS].[StaticBlock] WHERE [Key] = 'MobileOrderSuccess-WithGiftCertificate'))
BEGIN
   INSERT INTO [CMS].[StaticBlock] ([Key],[InnerName],[Content],[Added],[Modified],[Enabled]) 
   VALUES ('MobileOrderSuccess-WithGiftCertificate', 'Успешное оформление заказа с примененным сертификатом в мобильной версии', '', getdate(), getdate(), 1)
END

GO--

DELETE FROM [Settings].[TemplateSettings] WHERE [Name] = 'Mobile_AutoRedirect'
DELETE FROM [Settings].[TemplateSettings] WHERE [Name] = 'Mobile_AutoRedirect_Modern'

INSERT INTO [Settings].[Settings]([Name],[Value]) VALUES ('Mobile_AutoRedirect','True')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Models.Tasks.TasksPreFilterType.ObservedByMe','Наблюдаемые мной задачи')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Models.Tasks.TasksPreFilterType.ObservedByMe','Tasks observed by me')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Tasks.Index.TasksObservedByMe','Наблюдаемые мной задачи')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Tasks.Index.TasksObservedByMe','Tasks observed by me')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Crm.SalesFunnels.ELeadAutoCompleteActionType.None','Ничего не делать')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Crm.SalesFunnels.ELeadAutoCompleteActionType.None','Do nothing')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Crm.SalesFunnels.ELeadAutoCompleteActionType.OrderCreated','Если появился заказ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Crm.SalesFunnels.ELeadAutoCompleteActionType.OrderCreated','If an order is created')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Crm.SalesFunnels.ELeadAutoCompleteActionType.OrderPaid','Если появился оплаченный заказ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Crm.SalesFunnels.ELeadAutoCompleteActionType.OrderPaid','If the order is paid')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Crm.SalesFunnels.ELeadAutoCompleteActionType.BookingCreated','Если появилась запись в бронировании')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Crm.SalesFunnels.ELeadAutoCompleteActionType.BookingCreated','If a booking created')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Crm.SalesFunnels.SalesFunnelFinalSuccessAction.CreateOrder','Создать заказ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Crm.SalesFunnels.SalesFunnelFinalSuccessAction.CreateOrder','Create order')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Crm.SalesFunnels.SalesFunnelFinalSuccessAction.None','Ничего не делать')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Crm.SalesFunnels.SalesFunnelFinalSuccessAction.None','Do nothing')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Crm.SalesFunnels.ELeadAutoCompleteActionType.Description.OrderCreated','Автоматическое завершение лида №{0}. Новый заказ №{1}')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Crm.SalesFunnels.ELeadAutoCompleteActionType.Description.OrderCreated','Lead #{0} automcompletion. New order #{1}')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Crm.SalesFunnels.ELeadAutoCompleteActionType.Description.OrderPaid','Автоматическое завершение лида №{0}. Оплачен заказ №{1}')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Crm.SalesFunnels.ELeadAutoCompleteActionType.Description.OrderPaid','Lead #{0} automcompletion. Order #{1} has been paid')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Crm.SalesFunnels.ELeadAutoCompleteActionType.Description.BookingCreated','Автоматическое завершение лида №{0}. Новая бронь №{1}')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Crm.SalesFunnels.ELeadAutoCompleteActionType.Description.BookingCreated','Lead #{0} automcompletion. New booking #{1}')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Crm.Leads.LeadAutocompetion.NoSystemDealStatuses','У списка лидов "{0}" не указаны системные этапы сделки. Автоматическое завершение лидов невозможно.')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Crm.Leads.LeadAutocompetion.NoSystemDealStatuses','Leads list "{0}" has no system deal statuses. Leads automcompletion not available.')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.SettingsApi.Index.Order', 'Заказ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.SettingsApi.Index.Order', 'Order')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.SettingsApi.Index.CreateOrder', 'Создание заказа')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.SettingsApi.Index.CreateOrder', 'Create order')

GO--

ALTER PROCEDURE [Catalog].[sp_GetParentCategories]
	@ChildCategoryId int
AS
BEGIN
DECLARE @tbl TABLE ( id int, name nvarchar(255), url nvarchar(150) )

DECLARE @id int
set @id = @ChildCategoryId

if (select COUNT([CategoryID]) from [Catalog].[Category] where [CategoryID] = @id) <> 0
	while(@id <> 0 AND NOT @id IS NULL)
	begin
		insert into @tbl (id, name, url) select [CategoryID], [Name], [UrlPath] from [Catalog].[Category] where [CategoryID] = @id
		set @id = (Select [ParentCategory] from [Catalog].[Category] where [CategoryID] = @id)
	end

if(@id = 0)
	insert into @tbl (id, name, url) select [CategoryID], [Name], [UrlPath] from [Catalog].[Category] where [CategoryID] = @id

SELECT id, name, url FROM @tbl
END

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Template.SocialWidget.OnlyMobileVersion','доступно для мобильной версии')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Template.SocialWidget.OnlyMobileVersion','only in mobile version')

GO--


UPDATE [Settings].[Localization] Set [ResourceValue] = 'Отследить заказ' Where [ResourceKey] = 'Checkout.CheckOrderBlock.CheckStatus' and [LanguageId] = 1

GO--

UPDATE [Settings].[Localization] Set [ResourceValue] = 'Купоны могут применять только покупатели группы "Обычный покупатель", для остальных покупателей купоны учитываться не будут.' Where [ResourceKey] = 'Admin.Coupons.Index.Hint' and [LanguageId] = 1
UPDATE [Settings].[Localization] Set [ResourceValue] = 'Coupons can be used by the "Usual customer" group users only,<br> coupons will not be taken into consideration for other customers.' Where [ResourceKey] = 'Admin.Coupons.Index.Hint' and [LanguageId] = 2

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
  Update Catalog.Offer set Amount = Round(Amount + @DecrementedAmount, 4)  
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

ALTER PROCEDURE [Order].[sp_DecrementProductsCountAccordingOrder]   
 @orderId int  
AS  
BEGIN  
  UPDATE offer  
  SET offer.Amount = Round(offer.Amount - orderItems.Amount + orderItems.DecrementedAmount, 4)  
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

UPDATE [Settings].[Localization] Set [ResourceValue] = 'Показывать слайдер на главной странице или нет <br/><br/> Подробнее:<br/><a href="https://www.advantshop.net/help/pages/karusel" target="_blank">Инструкция. Настройка слайдера/карусели</a>' Where [ResourceKey] = 'Admin.Settings.Template.CarouselVisibilityHint' and [LanguageId] = 1
UPDATE [Settings].[Localization] Set [ResourceValue] = 'Show carousel slider on main page <br/><br/> More details: <br/> <a href="https://www.advantshop.net/help/pages/karusel" target="_blank">Instruction slider / carousel setting </a>' Where [ResourceKey] = 'Admin.Settings.Template.CarouselVisibilityHint' and [LanguageId] = 2

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Template.MainPageProductsVisibilityHelp', 'Отображать товары на главной <br/>Настройка позволяет отображать на главной странице 3 категории товаров: Хит продаж, Новинки, Скидки.<br/><br/> Инструкция:<br/><a href="https://www.advantshop.net/help/pages/product-on-main" target="_blank">https://www.advantshop.net/help/pages/product-on-main</a>')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Template.MainPageProductsVisibilityHelp', 'Show products on the main page <br/>The setting allows you to display on the main page 3 categories of products: Bestsellers, Novelties, Discounts.<br/>  Instruction:<br/> <a href="https://www.advantshop.net/help/pages/product-on-main" target="_blank">https://www.advantshop.net/help/pages/product-on-main</a>')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Template.CountMainPageProductInSectionHelp', 'Количество товаров в блоке<br/>Настройка отвечает за количество товара, которое будет отображаться на главной странице в каждом блоке.<br/><br/> Инструкция:<br/><a href="https://www.advantshop.net/help/pages/product-on-main#6" target="_blank">https://www.advantshop.net/help/pages/product-on-main#6</a>')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Template.CountMainPageProductInSectionHelp', 'Count products in section <br/>The setting is responsible for the quantity of goods that will be displayed on the main page in each block.<br/><br/> Instruction:<br/> <a href="https://www.advantshop.net/help/pages/product-on-main#6" target="_blank">https://www.advantshop.net/help/pages/product-on-main#6</a>')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Template.CountMainPageProductInLineHelp', 'Количество товаров в строке<br/>Настройка отвечает за количество товара, которое будет отображаться в одной строке каждого блока на главной странице.<br/><br/> Инструкция:<br/><a href="https://www.advantshop.net/help/pages/product-on-main#6" target="_blank">https://www.advantshop.net/help/pages/product-on-main#6</a>')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Template.CountMainPageProductInLineHelp', 'Count products in line <br/>The setting is responsible for the quantity of goods that will be displayed in one line of each block on the main page.<br/><br/> Instruction:<br/> <a href="https://www.advantshop.net/help/pages/product-on-main#6" target="_blank">https://www.advantshop.net/help/pages/product-on-main#6</a>')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Template.NewsVisibilityHelp', 'Новости<br/>Настройка активирует отображение раздела новостей на главной странице<br/><br/> Инструкция:<br/><a href="https://www.advantshop.net/help/pages/add-news" target="_blank">https://www.advantshop.net/help/pages/add-news</a>')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Template.NewsVisibilityHelp', 'News<br/>The setting activates the display of the news section on the main page<br/><br/> Instruction:<br/> <a href="https://www.advantshop.net/help/pages/add-news" target="_blank">https://www.advantshop.net/help/pages/add-news</a>')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Template.NewsSubscriptionVisibilityHelp', 'Подписка на новости<br/>Включенная галочка в настройке позволяет вывести блок "подписка на новости" на главную страницу<br/><br/> Инструкция:<br/><a href="https://www.advantshop.net/help/pages/add-news#3" target="_blank">https://www.advantshop.net/help/pages/add-news#3</a>')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Template.NewsSubscriptionVisibilityHelp', 'News subscription<br/>The included checkmark in the setting allows you to display the "subscribe to news" block on the main page<br/><br/> Instruction:<br/> <a href="https://www.advantshop.net/help/pages/add-news#3" target="_blank">https://www.advantshop.net/help/pages/add-news#3</a>')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Template.CheckOrderVisibilityHelp', 'Проверка статуса заказа<br/>Настройка позволяет отобразить блок "проверка статуса заказа" на главную страницу<br/><br/> Инструкция:<br/><a href="https://www.advantshop.net/help/pages/statusy-zakazov#2" target="_blank">https://www.advantshop.net/help/pages/statusy-zakazov#2</a>')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Template.CheckOrderVisibilityHelp', 'Check Order Status<br/>The setting allows you to display the block "check order status" on the main page<br/><br/> Instruction:<br/> <a href="https://www.advantshop.net/help/pages/statusy-zakazov#2" target="_blank">https://www.advantshop.net/help/pages/statusy-zakazov#2</a>')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Template.VotingVisibilityHelp', 'Голосование/Опрос<br/>Включенная галочка активирует настройка вывода блока "Голосование/Опрос" на главную страницу<br/><br/> Инструкция:<br/><a href="https://www.advantshop.net/help/pages/vote" target="_blank">https://www.advantshop.net/help/pages/vote</a>')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Template.VotingVisibilityHelp', 'Voting / Poll<br/>An enabled checkbox activates the setting of the "Voting / Polling" output block to the main page<br/><br/> Instruction:<br/> <a href="https://www.advantshop.net/help/pages/vote" target="_blank">https://www.advantshop.net/help/pages/vote</a>')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Template.GiftSertificateVisibilityHelp', 'Блок подарочных сертификатов<br/>Настройка выводит блок подарочных сертификатов на главную страницу<br/><br/> Инструкция:<br/><a href="https://www.advantshop.net/help/pages/kupons-i-podaroxhnie-certifikaty#2" target="_blank">https://www.advantshop.net/help/pages/kupons-i-podaroxhnie-certifikaty#2</a>')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Template.GiftSertificateVisibilityHelp', 'Gift Voucher Block<br/>Setting displays a block of gift certificates on the main page<br/><br/> Instruction:<br/> <a href="https://www.advantshop.net/help/pages/kupons-i-podaroxhnie-certifikaty#2" target="_blank">https://www.advantshop.net/help/pages/kupons-i-podaroxhnie-certifikaty#2</a>')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Template.BrandCarouselVisibilityHelp', 'Карусель производителей на главной<br/>Настройка позволяет вывести на главную страницу карусель производителей (логотипы производителей)<br/><br/> Инструкция:<br/><a href="https://www.advantshop.net/help/pages/brand#5" target="_blank">https://www.advantshop.net/help/pages/brand#5</a>')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Template.BrandCarouselVisibilityHelp', 'Carousel manufacturers on the main<br/>The setting allows you to display the carousel of manufacturers on the main page (manufacturers logos)<br/><br/> Instruction:<br/> <a href="https://www.advantshop.net/help/pages/brand#5" target="_blank">https://www.advantshop.net/help/pages/brand#5</a>')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Template.ColorsPictureInCatalog','Размеры иконки цвета в каталоге')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Template.ColorsPictureInCatalog','Sizes of the color icon in the catalog')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Template.ColorsPictureInDetails','Размеры иконки цвета в карточке товара')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Template.ColorsPictureInDetails','Sizes of the color icon in the product card')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Template.ColorsPictureHelp','Размеры иконки цвета. Минимальное значение 10х10 px, максимальное 1000х1000 px.')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Template.ColorsPictureHelp','The dimensions (in pixels) of the color icon. The minimum value is 10x10 px, the maximum is 1000x1000 px.')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.EditTask.Saved', 'сохранена')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.EditTask.Saved', 'saved')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.EditTask.Completed', 'завершена')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.EditTask.Completed', 'completed')

GO--

IF NOT EXISTS (SELECT * FROM [Settings].[Localization] WHERE ResourceKey = 'Admin.Settings.Commonpage.DomainsManager')
BEGIN
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Commonpage.DomainsManager', 'Менеджер доменов')
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Commonpage.DomainsManager', 'Domain manager')
END

GO--

UPDATE [Settings].[InternalSettings] SET [settingValue] = '8.5.3' WHERE [settingKey] = 'db_version'
