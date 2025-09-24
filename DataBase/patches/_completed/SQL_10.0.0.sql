
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.SystemSettings.DefaultCityIfNotAutodetect', 'Город по умолчанию, если отключено автоопределение')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.SystemSettings.DefaultCityIfNotAutodetect', 'Default city if auto-detection is disabled')

GO-- 

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Купоны, скидки, сертификаты' WHERE [ResourceKey] = 'Admin.Settings.Commonpage.Coupons' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'Coupons, discounts, certificates' WHERE [ResourceKey] = 'Admin.Settings.Commonpage.Coupons' AND [LanguageId] = 2
UPDATE [Settings].[Localization] SET [ResourceValue] = 'Купоны, скидки, сертификаты' WHERE [ResourceKey] = 'Admin.Settings.SettingsCoupons.Title' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'Coupons, discounts, certificates' WHERE [ResourceKey] = 'Admin.Settings.SettingsCoupons.Title' AND [LanguageId] = 2

GO--

ALTER TABLE [Order].OrderStatus ADD
	ShowInMenu bit NULL
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
	@CancelForbidden bit,
	@ShowInMenu bit
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
	
	insert into [Order].[OrderStatus] (StatusName, CommandID, IsDefault, IsCanceled, IsCompleted, Color, SortOrder, Hidden, CancelForbidden, ShowInMenu) 
							   VALUES (@StatusName, @CommandID, @IsDefault | ~@hasDefault, @IsCanceled, @IsCompleted, @Color, @SortOrder, @Hidden, @CancelForbidden, @ShowInMenu)
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
	@CancelForbidden bit,
	@ShowInMenu bit
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
		CancelForbidden = @CancelForbidden, ShowInMenu = @ShowInMenu
		Where OrderStatusID = @OrderStatusID
END

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.AddEditOrderStatus.ShowInMenu', 'Показывать в меню в заказах')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.AddEditOrderStatus.ShowInMenu', 'Show in orders menu')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Mobile.BrowserColorVariantsNotSet', 'Не задано')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Mobile.BrowserColorVariantsNotSet', 'Not set')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Mobile.BrowserColorVariantsAsColorScheme', 'Согласно цветовой схеме')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Mobile.BrowserColorVariantsAsColorScheme', 'According to the color scheme')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Mobile.BrowserColorVariantsCustomColor', 'Свой цвет')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Mobile.BrowserColorVariantsCustomColor', 'Your colore')

GO--

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Считать тарифы' WHERE [ResourceKey] = 'Admin.ShippingMethods.Sdek.ActiveTariffs' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'To calculate tariffs' WHERE [ResourceKey] = 'Admin.ShippingMethods.Sdek.ActiveTariffs' AND [LanguageId] = 2
UPDATE [Settings].[Localization] SET [ResourceValue] = 'Тут необходимо выбрать варианты, которые доступены вашему интернет-магазину.' WHERE [ResourceKey] = 'Admin.ShippingMethods.Sdek.NeedChooseOneOption' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'You need to choose options that is available to your online store.' WHERE [ResourceKey] = 'Admin.ShippingMethods.Sdek.NeedChooseOneOption' AND [LanguageId] = 2

GO--

if not exists (Select 1 From [Settings].[MailFormatType] Where [MailType] = 'OnPreOrder')
begin
	Insert Into [Settings].[MailFormatType] ([TypeName],[SortOrder],[Comment],[MailType])
	Values ('Под заказ', 420, 'Письмо при оформнении товара под заказ (#ORDER_ID#, #NUMBER#, #NAME#, #COMMENTS#, #PHONE#, #EMAIL#, #ORDERTABLE#, #STORE_NAME#, #MANAGER_NAME#)', 'OnPreOrder')
end

GO--

if not exists (Select 1 From [Settings].[MailFormat] Where [MailFormatTypeId] = 48)
begin
	Insert Into [Settings].[MailFormat] ([FormatName],[FormatText],[SortOrder],[Enable],[AddDate],[ModifyDate],[FormatSubject],[MailFormatTypeId]) 
	Select 'Под заказ', [FormatText], [SortOrder], 1, getdate(), getdate(), 'Заказ № #ORDER_ID# (под заказ)', 48
	From [Settings].[MailFormat]
	Where [MailFormatTypeId] = 16
end

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.ProductVideos.NameNotSpecified', 'Не указано')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.ProductVideos.NameNotSpecified', 'Not specified')

GO--

ALTER PROCEDURE [Catalog].[sp_AddProductToCategory] 
    @ProductId int,
    @CategoryId int,
    @SortOrder int,
    @MainCategory bit
AS
BEGIN

    DECLARE @Main bit = @MainCategory
    SET NOCOUNT ON;

    IF (@MainCategory = 1)
        UPDATE [Catalog].ProductCategories SET Main = 0 WHERE ProductID = @ProductID
    ELSE
        SET @Main = CASE WHEN EXISTS (SELECT 1 FROM [Catalog].ProductCategories WHERE ProductID = @ProductID AND Main = 1 AND CategoryID <> @CategoryId) THEN 0 ELSE 1 END;

    IF NOT EXISTS (SELECT 1 FROM [Catalog].ProductCategories WHERE CategoryID = @CategoryID AND ProductID = @ProductID)
        INSERT INTO [Catalog].ProductCategories (CategoryID, ProductID, SortOrder, Main) VALUES (@CategoryID, @ProductID, @SortOrder, @Main);
    ELSE
        UPDATE [Catalog].ProductCategories SET Main = @Main WHERE CategoryID = @CategoryID AND ProductID = @ProductID
END

GO--

ALTER PROCEDURE [Catalog].[sp_AddProductToCategoryByExternalId] 
	@ProductID int,
	@ExternalId nvarchar(50),
	@SortOrder int	
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @CategoryId int = (SELECT TOP(1) CategoryId FROM [Catalog].Category WHERE ExternalId = @ExternalId)
	IF @CategoryId IS NOT NULL AND NOT EXISTS (SELECT 1 FROM [Catalog].ProductCategories WHERE CategoryId = @CategoryId and ProductId = @ProductId)
	BEGIN
		DECLARE @Main bit = CASE WHEN EXISTS (SELECT 1 FROM [Catalog].ProductCategories WHERE ProductID = @ProductID AND Main = 1) THEN 0 ELSE 1 END;
		INSERT INTO [Catalog].ProductCategories (CategoryID, ProductID, SortOrder, Main) VALUES (@CategoryId, @ProductId, @SortOrder, @Main);
	END
END

GO--

ALTER PROCEDURE [Order].[sp_GetCustomerOrderHistory]
    @CustomerID uniqueidentifier
AS
BEGIN
    SELECT
        o.OrderID,
        o.Number,
        o.OrderDiscount,
        OrderStatus.StatusName,
        o.PreviousStatus,
        OrderStatus.OrderStatusID,
        o.Sum,
        o.OrderDate,
        o.PaymentDate,
        o.PaymentMethodName,
        o.ShippingMethodName,
        ShippingMethod.Name as ShippingMethod,
        o.PaymentMethodID,
        o.ManagerID,
        o.TrackNumber,
        (Customer.FirstName + ' ' + Customer.LastName) as ManagerName,
        OrderCurrency.CurrencyCode,
        OrderCurrency.CurrencyNumCode,
        OrderCurrency.CurrencyValue,
        OrderCurrency.CurrencySymbol,
        OrderCurrency.IsCodeBefore
    FROM [Order].[Order] o
        LEFT JOIN [Order].OrderStatus ON o.OrderStatusID = OrderStatus.OrderStatusID
        INNER JOIN [Order].OrderCurrency ON o.OrderID = OrderCurrency.OrderID
        LEFT JOIN [Order].ShippingMethod ON o.ShippingMethodID = ShippingMethod.ShippingMethodID
        INNER JOIN [Order].OrderCustomer ON o.OrderID = OrderCustomer.OrderID
        LEFT JOIN Customers.Managers ON o.ManagerId = Managers.ManagerId
        LEFT JOIN Customers.Customer ON Managers.CustomerId = Customer.CustomerID
    WHERE OrderCustomer.CustomerID = @CustomerID and IsDraft = 0
    ORDER BY o.OrderDate DESC
END

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Js.OrderHistory.TrackNumber', 'Трек-номер')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Js.OrderHistory.TrackNumber', 'Track number')

GO--

if not exists (Select 1 From [Settings].[MailFormatType] Where [MailType] = 'OnTaskObserverAdded')
begin
	Insert Into [Settings].[MailFormatType] ([TypeName],[SortOrder],[Comment],[MailType])
	Values ('При добавлении наблюдателя к задаче', 430, 'Уведомление о добавлении нового наблюдатедя к задаче (#TASK_ID#,#TASK_PROJECT#,#MANAGER_NAME#,#APPOINTEDMANAGER#,#TASK_NAME#,#TASK_DESCRIPTION#,#TASK_STATUS#,#TASK_PRIORITY#,#DUEDATE#,#DATE_CREATED#,#TASK_URL#,#TASK_ATTACHMENTS#, #OBSERVER#)', 'OnTaskObserverAdded')
end

GO--

if not exists (Select 1 From [Settings].[MailFormat] Where [MailFormatTypeId] = 49)
begin
	Insert Into [Settings].[MailFormat] ([FormatName],[FormatText],[SortOrder],[Enable],[AddDate],[ModifyDate],[FormatSubject],[MailFormatTypeId]) 
	VALUES ('При добавлении наблюдателя к задаче',
'<div style="color: #4c4f56; font-family: Arial, Helvetica, sans-serif; font-size: 14px;">
	<div class="header" style="border-bottom: 1px solid #ededed; display: table; margin-bottom: 25px; padding-bottom: 25px; width: 100%;">
		<div class="logo" style="display: table-cell; text-align: left; vertical-align: middle;">#LOGO#</div>
		<div class="phone" style="display: table-cell; text-align: right; vertical-align: middle;">
			<div class="tel" style="font-size: 26px; font-weight: bold; line-height: 1; margin-bottom: 5px;">&nbsp;</div>
			<div class="inform" style="font-size: 12px;">&nbsp;</div>
		</div>
		</div>
			<div>
				<p>Для задачи <a href="#TASK_URL#">#TASK_PROJECT#-#TASK_ID#</a> добавлен наблюдатель #OBSERVER#.</p>
			</div>
		</div>
	</div>
</div>',1620,1,getdate(),getdate(),'Для задачи #TASK_PROJECT#-#TASK_ID# добавлен наблюдатель',49)
end

GO--

ALTER TABLE [Customers].[Managers] ADD
	[Sign] nvarchar(MAX) NULL

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.AddEditUser.Sign', 'Подпись')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.AddEditUser.Sign', 'Signature')

GO--

Update [Settings].[Localization] 
Set ResourceValue = 'Шаблон письма ответа (#FIRSTNAME#, #LASTNAME#, #PATRONYMIC#, #STORE_NAME#, #MANAGER_NAME#, #MANAGER_SIGN#, #LAST_ORDER_NUMBER#)'
where ResourceKey = 'Admin.Js.AddEditMailAnswerTemplate.Template' and LanguageId = 1

Update [Settings].[Localization] 
Set ResourceValue = 'Response mail template (#FIRSTNAME#, #LASTNAME#, #PATRONYMIC#, #STORE_NAME#, #MANAGER_NAME#, #MANAGER_SIGN#, #LAST_ORDER_NUMBER#)'
where ResourceKey = 'Admin.Js.AddEditMailAnswerTemplate.Template' and LanguageId = 2

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Mobile.HeaderColorVariantAsColorScheme', 'Согласно цветовой схеме')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Mobile.HeaderColorVariantAsColorScheme', 'According to the color scheme')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Mobile.HeaderColorVariantWhite', 'Белый')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Mobile.HeaderColorVariantWhite', 'White')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Mobile.ViewCategoriesOnMainNotOutput', 'Не выводить')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Mobile.ViewCategoriesOnMainNotOutput', 'Do not output')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Mobile.ViewCategoriesOnMainWithoutIcons', 'Без иконок')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Mobile.ViewCategoriesOnMainWithoutIcons', 'No icons')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Mobile.ViewCategoriesOnMainWithIcons', 'C иконками')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Mobile.ViewCategoriesOnMainWithIcons', 'With icons')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Mobile.LogoTypeText', 'Текст')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Mobile.LogoTypeText', 'Text')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Mobile.LogoTypeFromDesktop', 'Логотип десктопной версии')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Mobile.LogoTypeFromDesktop', 'Desktop version logo')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Mobile.LogoTypeCustom', 'Загрузить свой логотип')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Mobile.LogoTypeCustom', 'Upload your logo')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Mobile.CatalogMenuViewModeRootCategories', 'Выводить корневые категории')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Mobile.CatalogMenuViewModeRootCategories', 'Show root categories')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Mobile.CatalogMenuViewModeLink', 'Отображать только ссылку')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Mobile.CatalogMenuViewModeLink', 'Show link only')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Js.Design.SuccessSavingTemplate', 'Изменения успешно сохранены')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Js.Design.SuccessSavingTemplate', 'Changes successfully saved')
GO--
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Product.DescriptionLengthLimit.ErrorFormat', 'Количество символов в описании товара {0} превышает максимальное - {1}')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Product.DescriptionLengthLimit.ErrorFormat', 'Number of characters in the product {0} description exceeds limit - {1}')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.ExportImport.ImportCsv.TextLengthLimit', 'Количество символов в поле {0} в строке {1} превышает максимальное - {2}')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.ExportImport.ImportCsv.TextLengthLimit', 'Number of characters in field {0} in row {1} exceeds limit - {2}')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.TextLengthLimit.Format', 'Максимальное количество символов - {0}')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.TextLengthLimit.Format', 'Characters count limit - {0}')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Subscribe.Export.UnsubscribeReason', 'Причина отказа от подписки')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Subscribe.Export.UnsubscribeReason', 'Unsubscribe reason')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Subscribe.Import.WrongFile', 'Неверный формат файла')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Subscribe.Import.WrongFile', 'Wrong file format')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Subscribe.Import.ImportError', 'Ошибка при импорте')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Subscribe.Import.ImportError', 'Import error')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Js.Inplace.ErrorRichSave', 'Ошибка при сохранении')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Js.Inplace.ErrorRichSave', 'Save error')

GO--

ALTER TABLE CMS.LandingSite ADD
	ModifiedDate datetime NULL,
	ScreenShotDate datetime NULL
	
GO--

if not exists (Select 1 From [Settings].[Localization] Where [ResourceKey] = 'Core.Customers.RoleActionCategory.Desktop')
begin
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Customers.RoleActionCategory.Desktop', 'Рабочий стол')
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Customers.RoleActionCategory.Desktop', 'Desktop')
end 

GO--

Insert Into [Customers].[CustomerRoleAction] ([CustomerID],[RoleActionKey],[Enabled])
Select c.CustomerId, 'Desktop', 1
From [Customers].[Customer] c 
Where c.[CustomerRole] = 50 and not exists (Select 1 From [Customers].[CustomerRoleAction] cr Where cr.CustomerId = c.CustomerId and cr.[RoleActionKey] = 'Desktop')

GO--


UPDATE [Settings].[Redirect]
   SET [RedirectFrom] = [RedirectFrom] + '(disabled)'
  WHERE [RedirectFrom] like '%.рф%'
 
GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.ProductHistory.ProductCreated', 'Создан товар')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.ProductHistory.ProductCreated', 'Product has been created')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.ProductHistory.ProductDeleted', 'Товар удален')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.ProductHistory.ProductDeleted', 'Product has been deleted')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.ProductHistory.OfferCreated', 'Модификация добавлена')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.ProductHistory.OfferCreated', 'Offer has been added')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.ProductHistory.OfferDeleted', 'Модификация удалена')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.ProductHistory.OfferDeleted', 'Offer has been deleted')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Catalog.Product.ArtNo', 'Артикул')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Catalog.Product.ArtNo', 'ArtNo')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Catalog.Product.Name', 'Название')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Catalog.Product.Name', 'Name')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Catalog.Product.Ratio', 'Рейтинг')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Catalog.Product.Ratio', 'Ratio')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Catalog.Product.BriefDescription', 'Краткое описание')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Catalog.Product.BriefDescription', 'Brief description')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Catalog.Product.Description', 'Описание')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Catalog.Product.Description', 'Description')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Catalog.Product.Enabled', 'Активность')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Catalog.Product.Enabled', 'Enabled')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Catalog.Product.Recomended', 'Рекомендованный')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Catalog.Product.Recomended', 'Recomended')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Catalog.Product.New', 'Новинка')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Catalog.Product.New', 'New')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Catalog.Product.BestSeller', 'Хит продаж')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Catalog.Product.BestSeller', 'BestSeller')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Catalog.Product.OnSale', 'Распродажа')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Catalog.Product.OnSale', 'OnSale')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Catalog.Product.AllowPreOrder', 'Под заказ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Catalog.Product.AllowPreOrder', 'Allow pre order')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Catalog.Product.Unit', 'Единицы измерения')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Catalog.Product.Unit', 'Unit')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Catalog.Product.ShippingPrice', 'Стоимость доставки')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Catalog.Product.ShippingPrice', 'Shipping price')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Catalog.Product.MinAmount', 'Минимальное количество товара')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Catalog.Product.MinAmount', 'Min amount')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Catalog.Product.MaxAmount', 'Максимальное количество товара')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Catalog.Product.MaxAmount', 'Max amount')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Catalog.Product.Multiplicity', 'Кратность количества')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Catalog.Product.Multiplicity', 'Multiplicity')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Catalog.Product.SalesNote', 'Заметки для продажи')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Catalog.Product.SalesNote', 'Sales note')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Catalog.Product.GoogleProductCategory', 'Категория товара Google Merchant Center')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Catalog.Product.GoogleProductCategory', 'Google product category')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Catalog.Product.YandexMarketCategory', 'YandexMarketCategory')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Catalog.Product.YandexMarketCategory', 'Yandex market category')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Catalog.Product.YandexTypePrefix', 'Яндекс-маркет префикс')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Catalog.Product.YandexTypePrefix', 'Yandex type prefix')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Catalog.Product.YandexModel', 'Яндекс-маркет модель')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Catalog.Product.YandexModel', 'Yandex model')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Catalog.Product.Gtin', 'Код международной маркировки и учета логистических единиц')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Catalog.Product.Gtin', 'Gtin')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Catalog.Product.Adult', 'Товар для взрослых')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Catalog.Product.Adult', 'Adult')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Catalog.Product.ManufacturerWarranty', 'Гарантия производителя')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Catalog.Product.ManufacturerWarranty', 'Manufacturer warranty')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Catalog.Product.Bid', 'Ставка для карточки модели')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Catalog.Product.Bid', 'Bid')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Catalog.Product.BarCode', 'Штрих код')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Catalog.Product.BarCode', 'BarCode')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Catalog.Product.YandexName', 'Яндекс-маркет название товара')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Catalog.Product.YandexName', 'Yandex name')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Catalog.Product.YandexDeliveryDays', 'Срок доставки')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Catalog.Product.YandexDeliveryDays', 'Yandex delivery days')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Catalog.Product.YandexProductDiscounted', 'Яндекс-маркет уцененный товар')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Catalog.Product.YandexProductDiscounted', 'Yandex product discounted')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Catalog.Product.YandexProductDiscountCondition', 'Яндекс-маркет состояние товара')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Catalog.Product.YandexProductDiscountCondition', 'Yandex product discount condition')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Catalog.Product.YandexProductDiscountReason', 'Яндекс-маркет причина уценки')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Catalog.Product.YandexProductDiscountReason', 'Yandex product discount reason')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Catalog.Product.AccrueBonuses', 'Начислять баллы за товар')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Catalog.Product.AccrueBonuses', 'Yandex product discounted')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Catalog.Product.UrlPath', 'Синоним для URL запроса')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Catalog.Product.UrlPath', 'Url path')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Catalog.Product.Discount', 'Скидка')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Catalog.Product.Discount', 'Discount')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Catalog.Product.Tax', 'Налог')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Catalog.Product.Tax', 'Tax')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Catalog.Product.Brand', 'Бренд')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Catalog.Product.Brand', 'Brand')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Catalog.Product.Currency', 'Валюта')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Catalog.Product.Currency', 'Currency')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.ProductHistory.CategoryAdded', 'Товар добавлен в категорию ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.ProductHistory.CategoryAdded', 'Added in category ')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.ProductHistory.CategoryDeleted', 'Товар удален из категории ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.ProductHistory.CategoryDeleted', 'Removed from category ')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.ProductHistory.Title', 'История изменений')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.ProductHistory.Title', 'Change history')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Catalog.Offer.Amount', 'Количество модификации')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Catalog.Offer.Amount', 'Offer amount')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Catalog.Offer.BasePrice', 'Цена модификации')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Catalog.Offer.BasePrice', 'Offer price')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Catalog.Offer.SupplyPrice', 'Закупочная цена модификации')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Catalog.Offer.SupplyPrice', 'Offer supply price')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Catalog.Offer.Color', 'Цвет модификации')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Catalog.Offer.Color', 'Offer color')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Catalog.Offer.Size', 'Размер модификации')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Catalog.Offer.Size', 'Offer size')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Catalog.Offer.Main', 'Главная модификации')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Catalog.Offer.Main', 'Offer main')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Catalog.Offer.ArtNo', 'Артикул модификации')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Catalog.Offer.ArtNo', 'Offer artNo')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Catalog.Offer.Weight', 'Вес модификации')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Catalog.Offer.Weight', 'Offer weight')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Catalog.Offer.Length', 'Длина модификации')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Catalog.Offer.Length', 'Offer length')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Catalog.Offer.Width', 'Ширина модификации')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Catalog.Offer.Width', 'Offer width')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Catalog.Offer.Height', 'Высота модификации')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Catalog.Offer.Height', 'Offer height')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.ProductHistory.ProductChanged', 'Товар изменен')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.ProductHistory.ProductChanged', 'Product changed')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.SystemSettings.TrackProductChanges', 'Отслеживать изменения товара')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.SystemSettings.TrackProductChanges', 'Track product changes')

GO--

if not exists(Select 1 From [Settings].[Settings] Where [Name] = 'TrackProductChanges')
	Insert Into [Settings].[Settings] ([Name], [Value]) Values ('TrackProductChanges', 'True')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.SystemSettings.ImageQuality', 'Качество изображения')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.SystemSettings.ImageQuality', 'Image quality')

GO--

Insert Into [Settings].[Settings] ([Name], [Value]) Values ('ImageQuality', '90')

GO--


If not exists (Select 1 From [Settings].[Settings] Where Name = 'Features.EnableExperimentalFeatures')
	Insert Into [Settings].[Settings] (Name, [Value]) Values ('Features.EnableExperimentalFeatures', 'False')

GO--

If not exists (Select 1 From [Settings].[Settings] Where Name = 'Features.EnableNewDashboard')
	Insert Into [Settings].[Settings] (Name, [Value]) Values ('Features.EnableNewDashboard', 'True')
else 
	Update [Settings].[Settings] Set [Value] = 'True' Where Name = 'Features.EnableNewDashboard'
	
GO--

If not exists (Select 1 From [Settings].[Settings] Where Name = 'Features.EnableNewDesignConstructor')
	Insert Into [Settings].[Settings] (Name, [Value]) Values ('Features.EnableNewDesignConstructor', 'True')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Checkout.CheckoutCart.NeedLogin', 'Для просмотра списка товаров необходимо <a class="link-text-decoration-invert" href="./login?redirectTo={0}">авторизоваться</a>')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Checkout.CheckoutCart.NeedLogin', 'To view the list of products, you must <a class="link-text-decoration-invert" href="./login?redirectTo={0}">log in</a>')

GO--

Update [Settings].[Localization] 
Set [ResourceValue] = 'Рекомендуемые размеры: 16 x 16, 32 x 32, 96 x 96, 120 x 120, 144 x 144 px<br> Favicon может быть только форматов *.gif, *.png или *.ico'
Where [LanguageId] = 1 and [ResourceKey] = 'Admin.Settings.Index.FaviconRecommendations'

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.ExportImport.ProductFields.ModifiedDate', 'Дата изменения')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.ExportImport.ProductFields.ModifiedDate', 'Modified date')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.SalesChannels.SalesChannel.Dashboard', 'Мои сайты')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.SalesChannels.SalesChannel.Dashboard', 'My sites')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.SalesChannels.SalesChannel.Dashboard.Details.Title', 'Мои сайты')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.SalesChannels.SalesChannel.Dashboard.Details.Title', 'My sites')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.SalesChannels.SalesChannel.Dashboard.Details.Text', 'Создайте современный продающий сайт: <ul><li>Интернет-магазин, который позволит вам представить широкий ассортимент продукции с возможностью поиска, фильтрации, сравнения товаров.</li><li>Воронку продаж, с помощью которой можно  продавать отдельные товары или услуги, а также привлекать больше потенциальных покупателей.</li></ul>')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.SalesChannels.SalesChannel.Dashboard.Details.Text', 'Create a modern selling website: <ul><li>An online store that allows you to present a wide range of products with the ability to search, filter, and compare products.</li><li>A sales funnel that allows you to sell individual products or services, as well as attract more potential customers.</li></ul>')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.SalesChannels.SalesChannel.Dashboard.Description', 'Создайте современный продающий сайт')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.SalesChannels.SalesChannel.Dashboard.Description', 'Create a modern selling website')

GO--

ALTER TABLE [Order].LeadItem ALTER COLUMN Color nvarchar(300) NULL

GO--

ALTER TABLE [Order].LeadItem ALTER COLUMN [Size] nvarchar(300) NULL

GO--

DELETE FROM [Settings].[Localization] WHERE ResourceKey = 'Admin.SettingsTelephony.Index.AutoCalls'
DELETE FROM [Settings].[Localization] WHERE ResourceKey = 'Admin.SettingsTelephony.Index.SmartCalls'
DELETE FROM [Settings].[Localization] WHERE ResourceKey = 'Admin.SettingsTelephony.Index.SmartCalls.Subdomain'
DELETE FROM [Settings].[Localization] WHERE ResourceKey = 'Admin.SettingsTelephony.Index.SmartCalls.SubdomainHelp'
DELETE FROM [Settings].[Localization] WHERE ResourceKey = 'Admin.SettingsTelephony.Index.SmartCalls.Username'
DELETE FROM [Settings].[Localization] WHERE ResourceKey = 'Admin.SettingsTelephony.Index.SmartCalls.Password'
DELETE FROM [Settings].[Localization] WHERE ResourceKey = 'Admin.SettingsTelephony.Index.SmartCalls.Login'
DELETE FROM [Settings].[Localization] WHERE ResourceKey = 'Admin.SettingsTelephony.Index.SmartCalls.Account'
DELETE FROM [Settings].[Localization] WHERE ResourceKey = 'Admin.SettingsTelephony.Index.SmartCalls.DeActivate'
DELETE FROM [Settings].[Localization] WHERE ResourceKey = 'Admin.SettingsTelephony.Index.SmartCalls.ApiUrls'
DELETE FROM [Settings].[Localization] WHERE ResourceKey = 'Admin.SettingsTelephony.Index.SmartCalls.ApiLeadVerify'
DELETE FROM [Settings].[Localization] WHERE ResourceKey = 'Admin.SettingsTelephony.Index.SmartCalls.SelectFunnel'
DELETE FROM [Settings].[Localization] WHERE ResourceKey = 'Admin.SettingsTelephony.Index.SmartCalls.SelectLeadStatus'
DELETE FROM [Settings].[Localization] WHERE ResourceKey = 'Admin.SettingsTelephony.Index.SmartCalls.ApiOrderVerify'
DELETE FROM [Settings].[Localization] WHERE ResourceKey = 'Admin.SettingsTelephony.Index.SmartCalls.SelectOrderStatus'

DELETE FROM [CRM].[TriggerAction] WHERE ActionType = 5

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.SettingsCheckout.CheckoutCommon.ShowCartItemsInBilling', 'Показывать состав заказа неавторизованным пользователям на странице по ссылке на оплату заказа')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.SettingsCheckout.CheckoutCommon.ShowCartItemsInBilling', 'Show order composition to unauthorized users on billing page')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.SettingsCheckout.CheckoutCommon.ShowCartItemsInBillingHint', 'По умолчанию на странице оплаты заказа состав заказа видят только авторизованные пользователи. Неавторизованные видят ссылку на авторизацию. Это сделано для безопасности.')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.SettingsCheckout.CheckoutCommon.ShowCartItemsInBillingHint', 'By default, only authorized users can see the order composition on the order payment page. Unauthorized users see the authorization link. This is done for security.')

GO-- 

alter table [Order].OrderCustomer alter column House nvarchar(50) null
alter table [Order].OrderCustomer alter column Apartment nvarchar(50) null

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Customers.View.Tasks', 'Задачи')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Customers.View.Tasks', 'Tasks')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.OrderAnalysis.DeliveryMethodsShowByName', 'Группировать по названию')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.OrderAnalysis.DeliveryMethodsShowByName', 'Group by name')

DELETE FROM [Settings].[Localization] WHERE ResourceKey = 'Admin.Js.ManagersReport.NumberOfProcessedOrders'

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Количество заказов' WHERE [LanguageId] = 1 AND [ResourceKey] = 'Admin.Js.ManagersReport.NumberOfOrders'
UPDATE [Settings].[Localization] SET [ResourceValue] = 'Number of orders' WHERE [LanguageId] = 2 AND [ResourceKey] = 'Admin.Js.ManagersReport.NumberOfOrders'

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.ManagersReport.NumberOfPaidOrders', 'Количество оплаченных заказов')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.ManagersReport.NumberOfPaidOrders', 'Number of paid orders')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.ManagersReport.AmountOfPaidOrders', 'Сумма оплаченных заказов')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.ManagersReport.AmountOfPaidOrders', 'Amount of paid orders')

GO--

Update Settings.MailFormatType Set Comment = Replace(Comment, ' )', '; #SHIPPINGMETHOD#)') where MailType = 'OnBillingLink'

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.CustomerView.Street', 'улица')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.CustomerView.Street', 'street')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ExportFeeed.SettingsYandex.JoinPropertyValues', 'Выгружать несколько значений свойства - все в одну строку')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ExportFeeed.SettingsYandex.JoinPropertyValues', 'Join multiple property values - all in one line')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Catalog.Product.YandexSizeUnit', 'Обозначения размерных сеток в атрибуте unit')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Catalog.Product.YandexSizeUnit', 'Dimension naming conventions in the unit attribute')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Catalog.Product.PaymentSubjectType', 'Предмет расчета')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Catalog.Product.PaymentSubjectType', 'Payment subject type')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Catalog.Product.PaymentMethodType', 'Способ расчета')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Catalog.Product.PaymentMethodType', 'Payment method type')

GO--

ALTER TABLE Customers.Customer ADD RegisteredFrom nvarchar(500) NULL

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
    @RegisteredFrom nvarchar(500)
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
        ,[RegisteredFrom])
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
        ,@RegisteredFrom);

    SELECT CustomerID, InnerId From [Customers].[Customer] WHERE CustomerId = @CustomerID
END

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Customers.RightBlock.RegisteredFrom', 'Регистрация со страницы')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Customers.RightBlock.RegisteredFrom', 'Registration from page')


GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Js.Address.LastName', 'Фамилия')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Js.Address.LastName', 'Last name')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Js.Address.Patronymic', 'Отчество')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Js.Address.Patronymic', 'Patronymic')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Js.Cart.CertificateCode', 'Подарочный сертификат')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Js.Cart.CertificateCode', 'Gift certificate')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Checkout.CheckoutCoupon.CertificateCode', 'Подарочный сертификат')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Checkout.CheckoutCoupon.CertificateCode', 'Gift certificate')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.CategoryHistory.CategoryCreated', 'Категория создана')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.CategoryHistory.CategoryCreated', 'Category created')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.CategoryHistory.CategoryDeleted', 'Категория удалена')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.CategoryHistory.CategoryDeleted', 'Category deleted')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.CategoryHistory.CategoryChanged', 'Категория изменена')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.CategoryHistory.CategoryChanged', 'Category changed')


INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Catalog.Category.ExternalId', 'changed')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Catalog.Category.ExternalId', 'changed')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Catalog.Category.Name', 'Название')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Catalog.Category.Name', 'Name')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Catalog.Category.Description', 'Описание')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Catalog.Category.Description', 'Description')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Catalog.Category.BriefDescription', 'Дополнительное описание')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Catalog.Category.BriefDescription', 'Brief description')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Catalog.Category.Enabled', 'Активность')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Catalog.Category.Enabled', 'Enabled')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Catalog.Category.Hidden', 'Скрыть в меню')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Catalog.Category.Hidden', 'Hidden')

GO--

ALTER TABLE Customers.City ADD
	District nvarchar(MAX) NULL

GO--

ALTER TABLE [Customers].[Contact] ADD
	District nvarchar(70) NULL

GO--

ALTER TABLE [Order].[OrderCustomer] ADD
	District nvarchar(255) NULL

GO--

ALTER TABLE [Order].[Lead] ADD
	District nvarchar(70) NULL

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.AddEditCitys.District','Район региона')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.AddEditCitys.District','District')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Orders.OrderContact.District','Район региона')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Orders.OrderContact.District','District')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'User.Registration.District','Район региона')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'User.Registration.District','District')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Checkout.DistrictField','Район региона')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Checkout.DistrictField','District')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Js.Address.District','Район региона')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Js.Address.District','District')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Customers.Customer.District','Район региона')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Customers.Customer.District','District')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Orders.OrderCustomer.District','Район региона')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Orders.OrderCustomer.District','District')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.ShippingsCity.District','Район региона')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.ShippingsCity.District','District')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Checkout.CheckoutUser.District','Район региона')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Checkout.CheckoutUser.District','District')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Checkout.PrintOrder.District','Район региона')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Checkout.PrintOrder.District','District')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Catalog.Category.SortOrder', 'Порядок сортировки')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Catalog.Category.SortOrder', 'SortOrder')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Catalog.Category.ParentCategoryId', 'Родительская категория')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Catalog.Category.ParentCategoryId', 'Parent category')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Catalog.Category.DisplayStyle', 'Вид отображения подкатегорий')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Catalog.Category.DisplayStyle', 'Type of display of subcategories')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Catalog.Category.DisplayChildProducts', 'Показывать товары из подкатегорий')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Catalog.Category.DisplayChildProducts', 'Display child products')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Catalog.Category.DisplayBrandsInMenu', 'Отображать производителей в меню')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Catalog.Category.DisplayBrandsInMenu', 'Display brands in menu')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Catalog.Category.DisplaySubCategoriesInMenu', 'Отображать в меню два уровня подкатегорий')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Catalog.Category.DisplaySubCategoriesInMenu', 'Display subcategories in menu')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Catalog.Category.Sorting', 'Сортировка по умолчанию')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Catalog.Category.Sorting', 'Sorting')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Catalog.Category.UrlPath', 'Синоним для URL запроса')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Catalog.Category.UrlPath', 'UrlPath')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Catalog.Category.AutomapAction', 'Распределение товаров по категориям')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Catalog.Category.AutomapAction', 'Automap')

GO--

ALTER TABLE [Catalog].[Category] ADD ShowOnMainPage bit NULL

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
	@AutomapAction int,
	@ModifiedBy nvarchar(50),
	@ShowOnMainPage bit
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
		,[AutomapAction]
		,[ModifiedBy]
		,[ShowOnMainPage]
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
		,@AutomapAction
		,@ModifiedBy
		,@ShowOnMainPage
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
	@AutomapAction int,
	@ModifiedBy nvarchar(50),
	@ShowOnMainPage bit
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
		,[AutomapAction] = @AutomapAction
		,[DateModified] = getdate()
		,[ModifiedBy] = @ModifiedBy
		,[ShowOnMainPage] = @ShowOnMainPage
	WHERE CategoryID = @CategoryID
END

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Category.RightPanel.ShowOnMainPage','Выводить на главной')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Category.RightPanel.ShowOnMainPage','Show on main page')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Template.MainPageCategoriesTitle','Категории на главной')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Template.MainPageCategoriesTitle','Main page categories')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Template.MainPageCategoriesVisibilityTitle','Отображать категории на главной')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Template.MainPageCategoriesVisibilityTitle','Display categories on main page')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Template.CountMainPageCategoriesInSectionTitle','Количество категорий в блоке')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Template.CountMainPageCategoriesInSectionTitle','Count main page categories in section')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Template.CountMainPageCategoriesInLineTitle','Количество категорий в строке')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Template.CountMainPageCategoriesInLineTitle','Count main page categories in line')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Home.MainPageCategoriesTitle','Популярные категории')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Home.MainPageCategoriesTitle','Popular categories')

GO--

ALTER PROCEDURE [Settings].[sp_GetCsvProductPropertyNames] 
	@exportFeedId INT,
	@exportAllProducts BIT,
	@exportNotAvailable BIT,
	@exportNoInCategory BIT
AS
BEGIN
	DECLARE @lproductNoCat TABLE (productid INT PRIMARY KEY CLUSTERED);

    IF (@exportNoInCategory = 1)
    BEGIN
        INSERT INTO @lproductNoCat
            SELECT [productid] 
            FROM [Catalog].product 
            WHERE [productid] NOT IN (SELECT [productid] FROM [Catalog].[productcategories]);
    END
	
	
	DECLARE @lcategorytemp TABLE (CategoryId INT)
	DECLARE @l TABLE (categoryid INT PRIMARY KEY CLUSTERED, Opened bit)
    
	INSERT INTO @l
		SELECT t.categoryid, t.Opened
		FROM [Settings].[exportfeedselectedcategories] AS t 
			INNER JOIN catalog.category ON t.categoryid = category.categoryid
		WHERE [exportfeedid] = @exportFeedId 

	DECLARE @l1 INT = (SELECT Min(categoryid) FROM @l)
	WHILE @l1 IS NOT NULL
	BEGIN 
		if ((Select Opened from @l where CategoryId = @l1) = 1)
			INSERT INTO @lcategorytemp SELECT @l1
		else
			INSERT INTO @lcategorytemp SELECT id FROM Settings.GetChildCategoryByParent(@l1)

		SET @l1 = (SELECT Min(categoryid) FROM @l WHERE  categoryid > @l1)
	END

	DECLARE @lcategory TABLE (categoryid INT PRIMARY KEY CLUSTERED);
	INSERT INTO @lcategory SELECT Distinct CategoryId FROM @lcategorytemp

	SELECT DISTINCT prop.Name
	FROM Catalog.Product p 
		INNER JOIN Catalog.ProductPropertyValue ON ProductPropertyValue.ProductId = p.ProductId
		INNER JOIN Catalog.PropertyValue propVal ON propVal.PropertyValueID = ProductPropertyValue.PropertyValueID
		INNER JOIN Catalog.Property prop ON prop.PropertyId = propVal.PropertyId
	WHERE 
		(
			EXISTS (
						SELECT 1 FROM [Catalog].[productcategories]
						WHERE [productcategories].[productid] = p.[productid] 
						AND [productcategories].categoryid IN (SELECT categoryid FROM @lcategory)
					) OR EXISTS (
						SELECT 1 
						FROM @lproductNoCat AS TEMP
						WHERE  TEMP.productid = p.[productid]
					) 
		)
		AND 
		(
			@exportAllProducts = 1 
			OR (
				SELECT Count(productid)
				FROM settings.exportfeedexcludedproducts
				WHERE exportfeedexcludedproducts.productid = p.productid AND exportfeedexcludedproducts.exportfeedid = @exportFeedId
			) = 0
		) AND (
			p.Enabled = 1 OR @exportNotAvailable = 1
		) AND (
			@exportNotAvailable = 1
			OR EXISTS (
				SELECT 1
				FROM [Catalog].[Offer] o
				Where o.[ProductId] = p.[productid] AND o.Price > 0 and o.Amount > 0
			)
		)
END

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Configuration.TemplateSettings_SectionMainPageCategories','Категории на главной')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Configuration.TemplateSettings_SectionMainPageCategories','Main page categories')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Configuration.TemplateSettings_MainPageCategoriesVisibility','Отображать категории на главной')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Configuration.TemplateSettings_MainPageCategoriesVisibility','Display categories on main page')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Configuration.TemplateSettings_CountMainPageCategoriesInSection','Количество категорий в блоке')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Configuration.TemplateSettings_CountMainPageCategoriesInSection','Count main page categories in section')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Configuration.TemplateSettings_CountMainPageCategoriesInLine','Количество категорий в строке')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Configuration.TemplateSettings_CountMainPageCategoriesInLine','Count main page categories in line')

GO--

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Shipping].[DpdCashCity]') AND type in (N'U'))
BEGIN
CREATE TABLE [Shipping].[DpdCashCity](
	[CityId] [bigint] NOT NULL,
	[CityName] [nvarchar](255) NOT NULL,
	[RegionName] [nvarchar](255) NOT NULL,
	[CountryCode] [nchar](2) NOT NULL,
	[Abbreviation] [nvarchar](50) NULL,
 CONSTRAINT [PK_DpdCashCity] PRIMARY KEY CLUSTERED 
(
	[CityId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END

GO--

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Shipping].[DpdParcelShops]') AND type in (N'U'))
BEGIN
CREATE TABLE [Shipping].[DpdParcelShops](
	[Code] [nvarchar](255) NOT NULL,
	[CityId] [bigint] NOT NULL,
	[CityName] [nvarchar](255) NOT NULL,
	[RegionName] [nvarchar](255) NOT NULL,
	[CountryCode] [nchar](2) NOT NULL,
	[Address] [nvarchar](255) NOT NULL,
	[Latitude] [float] NOT NULL,
	[Longitude] [float] NOT NULL,
	[IsSelfPickup] [bit] NOT NULL,
	[IsSelfDelivery] [bit] NOT NULL,
	[SelfDeliveryTimes] [nvarchar](255) NULL,
	[ExtraServices] [nvarchar](100) NOT NULL,
	[Services] [nvarchar](100) NOT NULL,
	[AddressDescription] [nvarchar](max) NULL,
	[MaxWeight] [float] NULL,
	[DimensionSum] [float] NULL,
	[MaxHeight] [float] NULL,
	[MaxWidth] [float] NULL,
	[MaxLength] [float] NULL,
	[Type] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_DpdParcelShops] PRIMARY KEY CLUSTERED 
(
	[Code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END

GO--

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Shipping].[DpdTerminals]') AND type in (N'U'))
BEGIN
CREATE TABLE [Shipping].[DpdTerminals](
	[Code] [nvarchar](255) NOT NULL,
	[CityId] [bigint] NOT NULL,
	[CityName] [nvarchar](255) NOT NULL,
	[RegionName] [nvarchar](255) NOT NULL,
	[CountryCode] [nchar](2) NOT NULL,
	[Address] [nvarchar](255) NOT NULL,
	[Latitude] [float] NOT NULL,
	[Longitude] [float] NOT NULL,
	[IsSelfPickup] [bit] NOT NULL,
	[IsSelfDelivery] [bit] NOT NULL,
	[SelfDeliveryTimes] [nvarchar](255) NULL,
	[ExtraServices] [nvarchar](100) NOT NULL,
	[Services] [nvarchar](100) NOT NULL,
	[AddressDescription] [nvarchar](max) NULL,
 CONSTRAINT [PK_DpdTerminals] PRIMARY KEY CLUSTERED 
(
	[Code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END

GO--

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[Shipping].[DpdParcelShops]') AND name = N'DpdParcelShops_CityId')
CREATE NONCLUSTERED INDEX [DpdParcelShops_CityId] ON [Shipping].[DpdParcelShops]
(
	[CityId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO--

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[Shipping].[DpdTerminals]') AND name = N'DpdTerminals_CityId')
CREATE NONCLUSTERED INDEX [DpdTerminals_CityId] ON [Shipping].[DpdTerminals]
(
	[CityId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Leads.Index.ImportAndExport', 'Импорт и экспорт')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Leads.Index.ImportAndExport', 'Import and export')

GO--

IF NOT EXISTS(SELECT * FROM [Settings].[Settings] WHERE [Name] = 'Pec -> PecEasyway')
BEGIN
	UPDATE [Order].[ShippingMethod]
	   SET [ShippingType] = 'PecEasyway'
	 WHERE [ShippingType] = 'Pec'

	UPDATE [Order].[OrderAdditionalData]
	   SET [Name] = 'PecEasywayOrderId'
	 WHERE [Name] = 'PecOrderId'
END

GO--

DELETE FROM [Settings].[Settings] WHERE [Name] = 'Pec -> PecEasyway'

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Catalog.ShowNotAvaliableLable','Отображать маркер "Нет в наличии"')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Catalog.ShowNotAvaliableLable','Show "not avaliable" lable')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.Customers.HasBonusCard','Есть бонусная карта')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.Customers.HasBonusCard','Has bonus card')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Catalog.Category.ShowOnMainPage','Выводить на главной')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Catalog.Category.ShowOnMainPage','Show on main page')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.ProductHistory.ProductMainCategoryChanged','Главная категория изменена')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.ProductHistory.ProductMainCategoryChanged','Main category changed')

GO--

UPDATE [CMS].[Menu] SET [MenuItemParentID] = NULL WHERE [MenuType] = 3

GO--

if not exists(Select 1 From [Settings].[Settings] Where Name = 'DashboardActive')
	Insert Into [Settings].[Settings] ([Name],[Value]) Values ('DashboardActive', 'True')

GO--

Update [Settings].[Localization] Set ResourceValue = 'Параметры' Where LanguageId = 1 and ResourceKey = 'Admin.Settings.Template.Title'

Update [Settings].[Localization] Set ResourceValue = 'Parameters' Where LanguageId = 2 and ResourceKey = 'Admin.Settings.Template.Title'

Update [Settings].[Localization] Set ResourceValue = 'Страницы и блоки' Where LanguageId = 1 and ResourceKey = 'Admin.StaticPages.Index.Title'

Update [Settings].[Localization] Set ResourceValue = 'Pages and blocks' Where LanguageId = 2 and ResourceKey = 'Admin.StaticPages.Index.Title'

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.Catalog.Discount', 'Скидка, %')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.Catalog.Discount', 'Discount, %')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.Catalog.DiscountAmount', 'Скидка (число)')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.Catalog.DiscountAmount', 'Discount')

GO--

IF (NOT EXISTS(SELECT * FROM [Order].[ShippingParam] WHERE ParamName='WithInsure' AND ShippingMethodID IN (SELECT ShippingMethodID FROM [Order].[ShippingMethod] WHERE [ShippingType] = 'Boxberry')))
BEGIN
	INSERT INTO [Order].[ShippingParam] ([ShippingMethodID],[ParamName],[ParamValue])
	SELECT [ShippingMethodID],'WithInsure','True' FROM [Order].[ShippingMethod] WHERE [ShippingType] = 'Boxberry'
END

IF (NOT EXISTS(SELECT * FROM [Order].[ShippingParam] WHERE ParamName='WithInsure' AND ShippingMethodID IN (SELECT ShippingMethodID FROM [Order].[ShippingMethod] WHERE [ShippingType] = 'Dpd')))
BEGIN
	INSERT INTO [Order].[ShippingParam] ([ShippingMethodID],[ParamName],[ParamValue])
	SELECT [ShippingMethodID],'WithInsure','True' FROM [Order].[ShippingMethod] WHERE [ShippingType] = 'Dpd'
END

GO-- 

declare @MORegionId int = (SELECT TOP 1 [RegionID] FROM [Customers].[Region] WHERE [RegionName] = 'Московская область')

IF (@MORegionId IS NOT NULL)
BEGIN
	DELETE FROM [Customers].[City]
	WHERE [CityName] = 'Северный (Москва)' AND [RegionID] = @MORegionId

	UPDATE [Customers].[City]
	   SET [CityName] = 'Северный',
	   [District] = 'Истра',
	   [Zip] = '143500'
	WHERE [CityName] = 'п.Северный' AND [RegionID] = @MORegionId

	UPDATE [Customers].[City]
	   SET [CityName] = 'Северный',
	   [District] = 'Талдомский',
	   [Zip] = '141912'
	WHERE [CityName] = 'Северный пгт' AND [RegionID] = @MORegionId

	UPDATE [Customers].[City]
	   SET [District] = 'Шатура'
	WHERE [CityName] = 'Северная Грива' AND [RegionID] = @MORegionId
END

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.Partners.DeleteSelected', 'Удалить выделенные')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.Partners.DeleteSelected', 'Delete selected')

GO-- 

Update [Vk].[VkCategory] Set VkCategoryId = 5001 Where VkCategoryId = 1
Update [Vk].[VkCategory] Set VkCategoryId = 5002 Where VkCategoryId = 2
Update [Vk].[VkCategory] Set VkCategoryId = 5003 Where VkCategoryId = 3
Update [Vk].[VkCategory] Set VkCategoryId = 5004 Where VkCategoryId = 4
Update [Vk].[VkCategory] Set VkCategoryId = 5005 Where VkCategoryId = 5

GO-- 

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Cart.Error.NotAvailableCustomOptions', 'С выбранными опциями доступно')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Cart.Error.NotAvailableCustomOptions', 'With selected options available')

GO-- 

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Orders.OrderCustomer.CustomerId', 'Изменился покупатель')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Orders.OrderCustomer.CustomerId', 'Customer has been changed')

GO-- 

If exists (Select 1 From [Settings].[Settings] Where [Name] = 'GoogleAnalyticsNumber' and [Value] <> '')
	Update [Settings].[Settings] Set [Value] = 'UA-' + [Value] Where [Name] = 'GoogleAnalyticsNumber'

GO-- 

ALTER TABLE [Order].ShippingMethod ADD
	CurrencyId int NULL

GO-- 
 
UPDATE [Order].ShippingMethod SET CurrencyId = (SELECT TOP 1 [CurrencyID] FROM [Catalog].[Currency] WHERE [CurrencyIso3] = 'RUB') WHERE [ShippingType] in ('Sdek', 'PickPoint', 'Edost', 'Dpd', 'Pec', 'Hermes', 'Shiptor', 'Grastin', 'YandexNewDelivery', 'YandexDelivery', 'Boxberry', 'RussianPost', 'DDelivery', 'EmsPost', 'PecEasyway')
UPDATE [Order].ShippingMethod SET CurrencyId = (SELECT TOP 1 [CurrencyID] FROM [Catalog].[Currency] WHERE [CurrencyIso3] = 'UAH') WHERE [ShippingType] = 'NovaPoshta'

GO-- 

DECLARE @BaseCurrencyId INT
SET @BaseCurrencyId = (SELECT TOP 1 [CurrencyID] FROM [Catalog].[Currency] WHERE [CurrencyValue] = 1)

IF (@BaseCurrencyId IS NOT NULL)
UPDATE [Order].ShippingMethod SET CurrencyId = @BaseCurrencyId WHERE [ShippingType] in ('SelfDelivery', 'PointDelivery', 'FixedRate', 'ShippingByWeight', 'ShippingByRangeWeightAndDistance', 'ShippingByPriceAndDistance', 'ShippingByOrderPrice', 'ShippingByProductAmount')
ELSE
RAISERROR ('Нет базовой валюты',18,1);

GO-- 

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Вернуться на главную' WHERE [ResourceKey] = 'Error.NotFound.BackToMainPage' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'Back to main page' WHERE [ResourceKey] = 'Error.NotFound.BackToMainPage' AND [LanguageId] = 2
UPDATE [Settings].[Localization] SET [ResourceValue] = 'Каталог' WHERE [ResourceKey] = 'Error.NotFound.BackCatalog' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'Catalog' WHERE [ResourceKey] = 'Error.NotFound.BackCatalog' AND [LanguageId] = 2

GO--

If exists(Select 1 From [Customers].[Region] Where RegionName = 'Кабардино-Балкария')
	Update [Customers].[Region] Set RegionName = 'Кабардино-Балкарская республика' Where RegionName = 'Кабардино-Балкария'

GO-- 

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Partners.ShowCaptchaInRegistrationPartners', 'Показывать капчу при регистрации в партнёрской программе')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Partners.ShowCaptchaInRegistrationPartners', 'Show captcha when registration in the partners program')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Design.Template.DefaultTemplateDescription', '"Стандартный" - это базовый дизайн интернет-магазина, предлагаемый платформой AdvantShop.<br/><br/>

Шаблон гибкий, полностью настраиваемый, он в обязательном порядке реализует все настройки административной панели.<br/><br/>

В "Стандартном" предустановлено множество цветовых схем, фонов и тем дизайна, которые позволят оформить магазин в соответствии с Вашими требованиями.<br/><br/>

Среди особенностей:<br/>
- 100% поддержка всех выпущенных актуальных модулей <br/>
- Гарантия корректного отображения клиентской части всех модулей<br/>
- Поддержка одно и двух колончатой компоновки главной страницы<br/>
- Большое разнообразие цветовых схем, тем дизайна и фоновых изображений<br/><br/>

Планируете собственный, индивидуальный дизайн, и у вас в команде есть frontend-специалист и дизайнер? Выбрав "Стандартный" шаблон в качестве базового, Вы существенно сэкономите время на доработку стилей магазина, и сократите объем css кода.<br/><br/>

Если Вы ищете надежность, стабильность и умеренный дизайн, то стандартный шаблон отвечает всем требованиям.')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Design.Template.DefaultTemplateDescription', '"Standard" is the basic design of an online store offered by the AdvantShop platform.<br/><br/>

Template is flexible, fully customizable, and it necessarily implements all the settings of the administrative panel.<br/><br/>

"Standard" has a variety of color schemes, backgrounds, and design themes that will allow you to design the store in accordance with Your requirements.<br/><br/>

Among the features:<br/>
- 100% support for all released current modules <br/>
- Guarantee of correct display of the client part of all modules<br/>
- Support for one-and two-column layout of the main page<br/>
- A wide variety of color schemes, design themes and background images<br/><br/>

Are you planning your own custom design, and do you have a frontend specialist and designer in your team? By choosing the "Standard" template as the base one, you will significantly save time on refining the store''s styles, and reduce the amount of css code.<br/><br/>

If you are looking for reliability, stability and a moderate design, then the standard template meets all the requirements.')

GO--

ALTER TABLE Booking.Booking ADD
	AdminComment nvarchar(255) NULL

GO--

declare @MoskowRegionId int = (SELECT TOP 1 [RegionID] FROM [Customers].[Region] WHERE [RegionName] = 'Москва')
declare @MoskowOblRegionId int = (SELECT TOP 1 [RegionID] FROM [Customers].[Region] WHERE [RegionName] = 'Московская область')

IF (@MoskowRegionId IS NOT NULL AND @MoskowOblRegionId IS NOT NULL)
BEGIN
	UPDATE [Customers].[City]
	   SET [RegionID] = @MoskowRegionId
	WHERE [CityName] = 'Троицк' AND [RegionID] = @MoskowOblRegionId
END

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Orders.Order.TotalAmount', 'Количество всех товаров')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Orders.Order.TotalAmount', 'Products amount')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Orders.Order.TotalAmountFormatted', 'Количество всех товаров прописью')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Orders.Order.TotalAmountFormatted', 'Products amount by words')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Orders.Order.ItemsCountFormatted', 'Количество позиций в заказе прописью')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Orders.Order.ItemsCountFormatted', 'Items count by words')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Landing.Product.ProductPhotos.GiftsTitle', 'Подарки')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Landing.Product.ProductPhotos.GiftsTitle', 'Gifts')


GO--

UPDATE [Settings].[Localization] SET ResourceValue = 'Для отображения этого метода оплаты требуется создание метода доставки типа {0}.' WHERE [ResourceKey] = 'Admin.PaymentMethods.PickPoint.CreateDeliveryMethod' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET ResourceValue = 'To display this payment method, you need to create a delivery method such as {0}.' WHERE [ResourceKey] = 'Admin.PaymentMethods.PickPoint.CreateDeliveryMethod' AND [LanguageId] = 2

GO-- 

ALTER TABLE [Order].PaymentDetails ADD
	IsCashOnDeliveryPayment bit NULL,
	IsPickPointPayment bit NULL

GO-- 

ALTER TABLE [Booking].PaymentDetails ADD
	IsCashOnDeliveryPayment bit NULL,
	IsPickPointPayment bit NULL

GO-- 

ALTER PROCEDURE [Order].[sp_AddPaymentDetails]
	@OrderID int,
	@CompanyName nvarchar(255),
	@INN nvarchar(255),
	@Phone nvarchar(20),
	@Contract nvarchar(255),
	@IsCashOnDeliveryPayment bit,
	@IsPickPointPayment bit
AS
BEGIN
	INSERT INTO [Order].[PaymentDetails]
           ([OrderID]
		   ,[CompanyName]
		   ,[INN]
		   ,[Phone]
		   ,[Contract]
		   ,[IsCashOnDeliveryPayment]
		   ,[IsPickPointPayment])
     VALUES
           (@OrderID
		   ,@CompanyName
		   ,@INN
		   ,@Phone
		   ,@Contract
		   ,@IsCashOnDeliveryPayment
		   ,@IsPickPointPayment)
	RETURN SCOPE_IDENTITY()
END

GO-- 

ALTER TABLE [Order].[Order] ADD
	AvailablePaymentCashOnDelivery bit NULL,
	AvailablePaymentPickPoint bit NULL

GO-- 

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Files.Index.FileWithDangerousContent', 'Файл с опасным содержанием')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Files.Index.FileWithDangerousContent', 'File with dangerous content')

GO--

ALTER TABLE [Order].ShippingMethod ADD
	ModuleStringId nvarchar(150) NULL

GO-- 

UPDATE [Settings].[Localization] SET ResourceValue = 'С иконками' WHERE [ResourceKey] = 'Admin.Settings.Mobile.ViewCategoriesOnMainWithIcons' AND [LanguageId] = 1

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Services.Webhook.EWebhookType.Api', 'Api')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Services.Webhook.EWebhookType.Api', 'Api')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Services.ApiWebhookEventType.OrderCreated', 'Новый заказ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Services.ApiWebhookEventType.OrderCreated', 'New order')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Services.ApiWebhookEventType.OrderStatusChanged', 'Смена статуса заказа')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Services.ApiWebhookEventType.OrderStatusChanged', 'Order status changed')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Services.ApiWebhookEventType.OrderPaymentStatusChanged', 'Смена статуса оплаты')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Services.ApiWebhookEventType.OrderPaymentStatusChanged', 'Order payment status changed')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.SettingsApi.Index.GetOrder', 'Получить заказ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.SettingsApi.Index.GetOrder', 'Get order')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.SettingsApi.Index.GetOrders', 'Получить список заказов')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.SettingsApi.Index.GetOrders', 'Get list of orders')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.SettingsApi.Index.OrderChangeStatus', 'Сменить статус заказа')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.SettingsApi.Index.OrderChangeStatus', 'Change order status')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.SettingsApi.Index.OrderSetPaid', 'Отметить заказ оплаченным')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.SettingsApi.Index.OrderSetPaid', 'Set order as paid')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.SettingsApi.Index.OrderStatusGetList', 'Получить список статусов')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.SettingsApi.Index.OrderStatusGetList', 'Get list of statuses')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Webhooks', 'Webhooks')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Webhooks', 'Webhooks')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Js.Builder.TopPanel', 'Верхняя панель')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Js.Builder.TopPanel', 'Top panel')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Js.Builder.Header', 'Шапка')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Js.Builder.Header', 'Header')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Js.Builder.TopMenu', 'Меню')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Js.Builder.TopMenu', 'Top menu')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Template.TopPanelTitle', 'Верхняя панель')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Template.TopPanelTitle', 'Top panel')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Template.HeaderTitle', 'Шапка')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Template.HeaderTitle', 'Header')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Template.TopMenuTitle', 'Меню')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Template.TopMenuTitle', 'Top menu')

GO-- 

ALTER TABLE [Order].ShippingReplaceGeo ADD
	InZip nvarchar(70) NULL,
	OutZip nvarchar(70) NULL,
	Comment nvarchar(255) NULL

GO-- 

UPDATE [Order].ShippingReplaceGeo SET InZip = '', OutZip = ''

GO-- 

ALTER TABLE [Order].ShippingReplaceGeo ALTER COLUMN
	InZip nvarchar(70) NOT NULL

GO-- 

ALTER TABLE [Order].ShippingReplaceGeo ALTER COLUMN
	OutZip nvarchar(70) NOT NULL

GO-- 

SET IDENTITY_INSERT [Order].[ShippingReplaceGeo] ON 

IF NOT EXISTS (SELECT * FROM [Order].[ShippingReplaceGeo] WHERE [Id] = 17)
INSERT INTO [Order].[ShippingReplaceGeo] ([Id],[ShippingType],[InCountryName],[InCountryISO2],[InRegionName],[InCityName],[InDistrict],[InZip],[OutCountryName],[OutRegionName],[OutCityName],[OutDistrict],[OutDistrictClear],[OutZip],[Enabled],[Sort],[Comment])
VALUES (17,'Boxberry','','RU','','','','690000','','','','',0,'690001',1,0,'Для Владивостока')

IF NOT EXISTS (SELECT * FROM [Order].[ShippingReplaceGeo] WHERE [Id] = 18)
INSERT INTO [Order].[ShippingReplaceGeo] ([Id],[ShippingType],[InCountryName],[InCountryISO2],[InRegionName],[InCityName],[InDistrict],[InZip],[OutCountryName],[OutRegionName],[OutCityName],[OutDistrict],[OutDistrictClear],[OutZip],[Enabled],[Sort],[Comment])
VALUES (18,'Boxberry','','RU','','','','664000','','','','',0,'664001',1,0,'Для Иркутска')

IF NOT EXISTS (SELECT * FROM [Order].[ShippingReplaceGeo] WHERE [Id] = 19)
INSERT INTO [Order].[ShippingReplaceGeo] ([Id],[ShippingType],[InCountryName],[InCountryISO2],[InRegionName],[InCityName],[InDistrict],[InZip],[OutCountryName],[OutRegionName],[OutCityName],[OutDistrict],[OutDistrictClear],[OutZip],[Enabled],[Sort],[Comment])
VALUES (19,'Boxberry','','RU','','','','443000','','','','',0,'443001',1,0,'Для Самары')

IF NOT EXISTS (SELECT * FROM [Order].[ShippingReplaceGeo] WHERE [Id] = 20)
INSERT INTO [Order].[ShippingReplaceGeo] ([Id],[ShippingType],[InCountryName],[InCountryISO2],[InRegionName],[InCityName],[InDistrict],[InZip],[OutCountryName],[OutRegionName],[OutCityName],[OutDistrict],[OutDistrictClear],[OutZip],[Enabled],[Sort],[Comment])
VALUES (20,'Boxberry','','RU','','','','644000','','','','',0,'644001',1,0,'Для Омска')

IF NOT EXISTS (SELECT * FROM [Order].[ShippingReplaceGeo] WHERE [Id] = 21)
INSERT INTO [Order].[ShippingReplaceGeo] ([Id],[ShippingType],[InCountryName],[InCountryISO2],[InRegionName],[InCityName],[InDistrict],[InZip],[OutCountryName],[OutRegionName],[OutCityName],[OutDistrict],[OutDistrictClear],[OutZip],[Enabled],[Sort],[Comment])
VALUES (21,'Boxberry','','RU','','','','634000','','','','',0,'634003',1,0,'Для Томска')

IF NOT EXISTS (SELECT * FROM [Order].[ShippingReplaceGeo] WHERE [Id] = 22)
INSERT INTO [Order].[ShippingReplaceGeo] ([Id],[ShippingType],[InCountryName],[InCountryISO2],[InRegionName],[InCityName],[InDistrict],[InZip],[OutCountryName],[OutRegionName],[OutCityName],[OutDistrict],[OutDistrictClear],[OutZip],[Enabled],[Sort],[Comment])
VALUES (22,'Boxberry','','RU','','','','170000','','','','',0,'170001',1,0,'Для Твери')


SET IDENTITY_INSERT [Order].[ShippingReplaceGeo] OFF

GO-- 

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Нет установленных шаблонов. Для установки перейдите в <a href="dashboard/createsite?mode=store">Магазин шаблонов</a>' WHERE [ResourceKey] = 'Admin.Design.Index.NoInstalledTemplates' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'No templates are installed. To install, go to the <a href="dashboard/createsite?mode=store">Template Shop</a>' WHERE [ResourceKey] = 'Admin.Design.Index.NoInstalledTemplates' AND [LanguageId] = 2

GO-- 

if not exists (Select 1 From [Settings].[Localization] Where [ResourceKey] = 'Admin.ExportFeed.SettingsYandex.ExportCount')
begin
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ExportFeed.SettingsYandex.ExportCount', 'Выгружать количество товара')
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ExportFeed.SettingsYandex.ExportCount', 'Export products amount')

	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ExportFeed.SettingsYandex.ExportCountHelp', 'При активации настройки количество товара будет выгружаться в теге ''count''')
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ExportFeed.SettingsYandex.ExportCountHelp', 'Products amount will be exported in the ''count'' tag')
end 

GO--


update [Settings].[Settings] set value = 'True' Where Name = 'EnableCaptchaInRegistration'
update [Settings].[Settings] set value = 'True' Where Name = 'EnableCaptchaInFeedback'
update [Settings].[Settings] set value = '2' Where Name = 'CaptchaMode'

GO--
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.SystemSettings.SitemapXmlLastUpdateHint', 'Это файл, который необходим для корректной и своевременной индексации сайта в поисковых системах.<br/><br/>Подробнее: <br/> <a href="https://www.advantshop.net/help/pages/robots-txt#2" target="_blank" >Карта сайта (Sitemap)</a> ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.SystemSettings.SitemapXmlLastUpdateHint', 'This is a file that is necessary for the correct and timely indexing of the site in search engines.<br/><br/> More details:<br/> <a href="https://www.advantshop.net/help/pages/robots-txt#2" target="_blank" >Sitemap</a> ')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Tasks.DefaultTaskGroupHint', 'Выбираете проект из списка, который будет автоматически проставляется в задачу ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Tasks.DefaultTaskGroupHint', 'Choose a project from the list, which will be automatically added to the task ')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.SettingsTasks.Tasks.OpenTaskFromPushHint', 'Выбираете настройку, которая позволит открыть задачу из уведомления в том же окне, либо в новой вкладке браузера. ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.SettingsTasks.Tasks.OpenTaskFromPushHint', 'Choose the setting that will open the task from the notification in the same window or in a new browser tab. ')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.SettingsTask.Task.ReminderActiveHint', 'Данная настройка позволяет производить напоминание об окончании срока задачи ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.SettingsTask.Task.ReminderActiveHint', 'This setting allows you to make a reminder about the end of the task ')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Social.SocialShareEnabledHint', 'Активация данной настройки выводит соц.кнопки в клиентскую часть сайта ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Social.SocialShareEnabledHint', 'Activating this setting displays social buttons in the client side of the site ')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Social.SocialShareCustomEnabledByDefaultHint', 'Выводятся стандатные соц.кнопки ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Social.SocialShareCustomEnabledByDefaultHint', 'The standard social buttons are displayed ')


INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Social.SocialShareCustomEnabledByCustomCodeHint', 'Вписываете свой код кнопок ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Social.SocialShareCustomEnabledByCustomCodeHint', 'Enter your button code ')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.SettingsTelephony.CallsSalesFunnelHint', 'Выбираете действие при новом звонке, либо не создавать лид, либо создавать и в какую группу лида. ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.SettingsTelephony.CallsSalesFunnelHint', 'Choose an action for a new call, either not create a lead, or create and in which group the lead. ')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Telephony.OperatorHint','Выбираете оператора ip телефонии и далее настраиваете в соответствии с инструкцией того или иного оператора<br/><br/> Sipuni<br/>Подробнее: <br/> <a href="https://www.advantshop.net/help/pages/phone-sipuni" target="_blank" >Телефония. Sipuni</a> <br/><br/> Манго Телеком <br/> Подробнее: <br/> <a href="https://www.advantshop.net/help/pages/phone-mango" target="_blank" >Манго Телеком</a>')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Telephony.OperatorHint','Choose an ip telephony operator and then set it up in accordance with the instructions of one or another operator<br/><br/> Sipuni <br/>  Details: <br/> <a href = "https://www.advantshop.net/help/pages/ phone-sipuni "target =" _blank "> Telephony. Sipuni </a> <br/><br/> Mango Telecom <br/> Details: <br/> <a href="https://www.advantshop.net/help/pages/phone-mango" target="_blank">Telephony. Mango Telecom</a>')


INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Telephony.Mango.ApiUrlHint', 'Данную настройку вам необходимо взять на стороне Манго-Телеком ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Telephony.Mango.ApiUrlHint', 'You need to take this setting on the side of Mango-Telecom ')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Telephony.Mango.ApiKeyHint', 'Данную настройку вам необходимо взять на стороне Манго-Телеком ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Telephony.Mango.ApiKeyHint', 'You need to take this setting on the side of Mango-Telecom ')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Telephony.Mango.SecretKeyHint','Данную настройку вам необходимо взять на стороне Манго-Телеком<br/><br/>Телфин<br/>Подробнее: <br/> <a href="https://www.advantshop.net/help/pages/phone-telphin " target="_blank" >Телефония. Телфин</a><br/> <br/>Задарма<br/>Подробнее: <br/> <a href="https://www.advantshop.net/help/pages/phone-zadarma " target="_blank" >Телефония. Zadarma</a>')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Telephony.Mango.SecretKeyHint','You need to take this setting on the side of Mango-Telecom. <br/><br/> Telfin <br/> More details: <br/> <a href = "https://www.advantshop.net/help/pages/phone-telphin" target = "_blank"> Telephony. Telfin </a><br/> <br/> Zadarma <br/> Read more: <br/> <a href="https://www.advantshop.net/help/pages/phone-zadarma "target="_blank"> Telephony. Zadarma </a> ')


INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Booking.CategoryImageSizesHint', 'Настройка размеров изображения категории, рекомендуемый размер по умолчанию 70 на 70 px. ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Booking.CategoryImageSizesHint ',' Setting the size of the category image, the recommended default size is 70 by 70 px.')


INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Booking.ServiceImageDimensionsHint', 'Настройка размеров изображения услуги, рекомендуемый размер по умолчанию 70 на 70 px. ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Booking.ServiceImageDimensionsHint', 'Setting the size of the service image, the recommended default size is 70 x 70 px. ')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Booking.Width', 'Ширина ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Booking.Width', 'Width ')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Booking.Height', 'Высота ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Booking.Height', 'Height ')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Booking.ResourceImageDimensions', 'Размеры изображения ресурса ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Booking.ResourceImageDimensions', 'Resource image dimensions ')


INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Booking.ResourceImageDimensionsHint', 'Настройка размеров изображения ресурса, рекомендуемый размер по умолчанию 70 на 70 px. ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Booking.ResourceImageDimensionsHint', 'Setting the size of the service image, the recommended default size is 70 x 70 px. ')


INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.API.1CEnabledHint', 'Настройка активирует интеграцию с 1С<br/><br/>Подробнее: <br/> <a href="https://www.advantshop.net/help/pages/1c-integration" target="_blank" >Интеграция магазина с 1С</a> ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.API.1CEnabledHint', 'The setting activates the integration with 1C <br/> <br/> More details: <br/> <a href="https://www.advantshop.net/help/pages/1c-integration" target="_blank"> Shop integration with 1C </a> ')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.API.1CDisableProductsDecrementionHint', 'Если галочка активна, то весь учет товаров будет на стороне 1С. ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.API.1CDisableProductsDecrementionHint', 'If the checkbox is active, then all accounting of goods will be on the 1C side. ')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.API.ExportOrdersTypeHint', 'Выбираете из списка вариант выгрузки заказов<br/><br/>Подробнее: <br/> <a href="https://www.advantshop.net/help/pages/1c-integration" target="_blank" >Интеграция магазина с 1С</a> ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.API.ExportOrdersTypeHint', 'Choose from the list the option to unload orders <br/> <br/> More details: <br/> <a href="https://www.advantshop.net/help/pages/1c-integration" target="_blank"> Integration store with 1C </a> ')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.API.1CUpdateStatusesHint', 'Настройка позволяет обновлять статусы заказов из 1С ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.API.1CUpdateStatusesHint', 'The setting allows you to update the statuses of orders from 1C ')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.API.1CUpdateProductsHint', 'Выбираете вариант обновления товаров ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.API.1CUpdateProductsHint', 'Choose the option to update products ')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.API.1CSendProductsHint', 'Выбираете способ отправки номенклатуры в 1С ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.API.1CSendProductsHint', 'Choose a way to send items to 1C ')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.NotifyEMails.NotificationsHint', '<br/><br/>Подробнее: <br/> <a href="https://www.advantshop.net/help/pages/notification-settings" target="_blank" >Настройка почтовых уведомлений</a> ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.NotifyEMails.NotificationsHint', '<br/> <br/> More information: <br/> <a href="https://www.advantshop.net/help/pages/notification-settings" target="_blank"> Setting up email notifications </a> ')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.NotifyEMails.EmailForOrdersHint', 'Укажите E-mail для уведомлений о новых заказах ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.NotifyEMails.EmailForOrdersHint', 'Enter E-mail for notifications of new orders ')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.NotifyEMails.EmailForLeadsHint', 'Укажите E-mail для уведомлений о новых лидах ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.NotifyEMails.EmailForLeadsHint', 'Enter E-mail for notifications about new leads ')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.NotifyEMails.EmailForBookingsHint', 'Укажите E-mail для уведомлений о новых отзывах о товарах ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.NotifyEMails.EmailForBookingsHint', 'Enter your E-mail for notifications of new product reviews ')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.NotifyEMails.EmailForRegReportHint', 'Укажите E-mail для уведомлений о зарегистрированных пользователях ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.NotifyEMails.EmailForRegReportHint', 'Enter E-mail for notifications about registered users ')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.NotifyEMails.EmailForFeedbackHint', 'Укажите E-mail для отправки сообщений с формы обратной связи ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.NotifyEMails.EmailForFeedbackHint', 'Enter your E-mail to send messages from the feedback form ')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.NotifyEMails.EmailForPartnersHint', 'Укажите E-mail для уведомлений о регистрации партнеров ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.NotifyEMails.EmailForPartnersHint', 'Enter your E-mail for partner registration notifications ')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.NotifyEMails.ToHint', 'Впишите e-mail получателя для отправки пробного письма ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.NotifyEMails.ToHint', 'Enter the recipient`s e-mail to send a test letter ')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.NotifyEMails.SubjectHint', 'Впишите тему для отправки пробного письма ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.NotifyEMails.SubjectHint', 'Enter a subject for sending a test letter ')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.NotifyEMails.BodyHint', 'Напишите недлинный текст для отправки пробного письма ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.NotifyEMails.BodyHint', 'Write short text to send a test letter ')
GO--

if not exists (Select 1 From [Settings].[Localization] Where [ResourceKey] = 'Js.Design.DoYouWantSqueezePhotos')
begin
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Js.Design.DoYouWantSqueezePhotos', 'Вы действительно хотите пережать фотографии всех товаров (может занять длительное время)?')
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Js.Design.DoYouWantSqueezePhotos', 'Do you really want to squeeze photos of all products (can take a long time)?')

	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Js.Design.SqueezePhotosOfProducts', 'Пережатие фотографий товаров')
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Js.Design.SqueezePhotosOfProducts', 'Squeeze photos of products')
end 

GO--

if not exists (Select 1 From [Settings].[Localization] Where [ResourceKey] = 'App.Landing.Domain.Forms.ELpFormFieldType.Birthday')
begin
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'App.Landing.Domain.Forms.ELpFormFieldType.Birthday', 'День рождения')
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'App.Landing.Domain.Forms.ELpFormFieldType.Birthday', 'Birthday')
end
GO-- 

if not exists (Select 1 From [Settings].[Localization] Where [ResourceKey] = 'Admin.ExportFeed.SettingsYandex.ExportShopSku')
begin
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ExportFeed.SettingsYandex.ExportShopSku', 'Выгружать тег shop-sku')
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ExportFeed.SettingsYandex.ExportShopSku', 'Export shop-sku tag')

	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ExportFeed.SettingsYandex.ExportShopSkuHelp', 'Выбранный идентификатор товарного предложения будет выгружаться тег shop-sku')
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ExportFeed.SettingsYandex.ExportShopSkuHelp', 'The selected product offer ID will be exported in shop-sku tag')

	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ExportFeed.SettingsYandex.ExportManufacturer', 'Выгружать тег manufacturer')
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ExportFeed.SettingsYandex.ExportManufacturer', 'Export manufacturer tag')

	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ExportFeed.SettingsYandex.ExportManufacturerHelp', 'Производитель будет выгружаться тег manufacturer')
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ExportFeed.SettingsYandex.ExportManufacturerHelp', 'Brand name will be exported in manufacturer tag')
end

GO--

UPDATE [Settings].[Settings] SET [Value] = '#STORE_NAME#' WHERE [Name] = 'DefaultMetaKeywords' AND [Value] = '#STORE_NAME# '
UPDATE [Settings].[Settings] SET [Value] = '#STORE_NAME#' WHERE [Name] = 'DefaultMetaDescription' AND [Value] = '#STORE_NAME# '

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.SalesPlanSettings.SalesPlanHint', 'В данном поле указывается значение плана продаж в месяц. Это понадобится для аналитических отчетов при закрытии месяца.<br/><br/>Подробнее: <br/><a href ="https://www.advantshop.net/help/pages/plan-prodazh" target="_blank">График плана продаж.</a>')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.SalesPlanSettings.SalesPlanHint', 'This field specifies the value of the monthly sales plan. This will be needed for analytical reports at the close of the month.<br/><br/> More details: <br/><a href ="https://www.advantshop.net/help/pages/plan-prodazh" target="_blank">Sales plan chart.</a>')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.SalesPlanSettings.ProfitPlanHint', 'В данном поле указывается значение плана продаж в месяц. Это понадобится для аналитических отчетов при закрытии месяца.<br/><br/>  Подробнее: <br/><a href ="https://www.advantshop.net/help/pages/plan-prodazh" target="_blank">График плана продаж.</a>')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.SalesPlanSettings.ProfitPlanHint', 'This field specifies the value of the monthly sales plan. This will be needed for analytical reports at the close of the month.<br/><br/> More details: <br/><a href ="https://www.advantshop.net/help/pages/plan-prodazh" target="_blank">Sales plan chart.</a>')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Feedback.FeedbackActionHint', 'Выбираете действие, которое произойдет, если клиент воспользуется формой обратной связи на сайте.<br/><br/>  Подробнее: <br/><a href ="https://www.advantshop.net/help/pages/otpravit-soobschenie" target="_blank">Форма обратной связи.</a>')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Feedback.FeedbackActionHint', 'Choose the action that will happen if the client uses the feedback form on the site.<br/><br/> More details: <br/><a href ="https://www.advantshop.net/help/pages/otpravit-soobschenie" target="_blank">Feedback form.</a>')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Catalog.SearchIndexHint', 'Используете настройку, если требуется обеспечить быстрый и точный поиск информации не дожидаясь автоматического запуска.<br/><br/>  Подробнее: <br/><a href ="https://www.advantshop.net/help/pages/search#5" target="_blank">Индекс для поиска.</a>')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Catalog.SearchIndexHint', 'Use this setting if you need to provide fast and accurate information search without waiting for automatic launch.<br/><br/> More details: <br/><a href ="https://www.advantshop.net/help/pages/search#5" target="_blank">Index to search.</a>')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Catalog.SearchDeepHint', '<b>1 уровень </b> - поиск осуществляется четко по всей фразе <br /> <br /> <b>2 уровень </b>- поиск осуществляется по принципу 1 уровня по отдельным словам, т.е. сначала выводится результат жесткого соответствия фразы, затем результаты содержащие отдельные слова в поисковом запросе <br /> <br /> <b>3 уровень </b> - поиск осуществляется по принципу 1 уровня 2 уровня значения по принципу “слово*”, т.е. сначала выводится результат жесткого соответствия фразы, затем результаты содержащие отдельные слова в поисковом запросе, и после выводятся результаты с встречающимся набором отдельных слов из вводимой фразы в поисковой запрос. <br /> <br /> <b>4 уровень </b> - данный уровень самый лояльный, в поисковую выдачу будут попадать все результаты, если в названии, описании или артикуле есть фраза или слово, которую ввели. <br /> <br /> Подробнее: <br /> <a href="https://www.advantshop.net/help/pages/search#6" target="_blank"> Глубина поиска.</a> ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Catalog.SearchDeepHint', '<b> Level 1 </b> - the search is carried out clearly throughout the phrase <br /> <br /> <b> 2nd level </b> - search is carried out according to the principle of 1st level for separate words, i.e. first, the result of a hard match of the phrase is displayed, then the results containing individual words in the search query <br /> <br /> <b> 3 level </b> - search is carried out according to the principle of 1 level of 2 levels of meaning according to the principle “word *”, i.e. first, the result of a hard match of the phrase is displayed, then the results containing individual words in the search query are displayed, and then the results are displayed with a encountered set of individual words from the entered phrase into the search query. <br /> <br /> <b> Level 4 </b> - this level is the most loyal, all results will be included in the search results if the title, description or article contains a phrase or a word that you have entered. <br /> <br /> More details: <br /> <a href="https://www.advantshop.net/help/pages/search#6" target="_blank"> Search depth. </a> ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Catalog.Search_MaxItemsHint', 'Число указывает на значение возвращённых результатов. Чем больше число, тем больше могут быть разбросаны результаты.<br/><br/>Подробнее: <br/><a href="https://www.advantshop.net/help/pages/search#7" >Максимальное количество найденных результатов.</a> ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Catalog.Search_MaxItemsHint', 'The number indicates the value of the returned results. The larger the number, the more scattered the results can be.<br/><br/>More details: <br/><a href="https://www.advantshop.net/help/pages/search#7" >Maximum results found.</a> ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Catalog.SearchExampleHint', 'Примеры поискового запроса (каждый в новой строке)<br/><br/>Подробнее: <br/><a href="https://www.advantshop.net/help/pages/search#8" >Примеры поискового запроса. (каждый в новой строке)</a> ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Catalog.SearchExampleHint', 'Search query examples (each on a new line)<br/><br/>More details: <br/><a href="https://www.advantshop.net/help/pages/search#8" >Search query examples. (each on a new line)</a> ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Catalog.DefaultCurrencyHint', 'Данный параметр отвечает за то, какая валюта будет показана в первый раз в клиентской части магазина для новых пользователей.<br/><br/>Подробнее: <br/><a href="https://www.advantshop.net/help/pages/currency#4" >Валюта по умолчанию.</a> ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Catalog.DefaultCurrencyHint', 'This parameter is responsible for which currency will be shown for the first time in the client side of the store for new users.<br/><br/>More details: <br/><a href="https://www.advantshop.net/help/pages/currency#4" >Default currency.</a> ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Catalog.SearchExampleHintTitle', 'Примеры поискового запроса ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Catalog.SearchExampleHintTitle', 'Search query examples ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Catalog.AllowToChangeCurrencyHint', 'Данный параметр позволяет менять валюты покупателям, в случае, если в магазине несколько валют.<br/><br/>Подробнее: <br/> <a href="https://www.advantshop.net/help/pages/currency#5" >Разрешить покупателям переключение валют.</a> ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Catalog.AllowToChangeCurrencyHint', 'This parameter allows customers to change currencies if the store has several currencies.<br/><br/>More details: <br/> <a href="https://www.advantshop.net/help/pages/currency#5" >Allow buyers to switch currencies.  </a> ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Catalog.AutoUpdateCurrenciesHint', 'При активации настройки обновление валюты будет происходить автоматически.<br/><br/>Подробнее: <br/> <a href="https://www.advantshop.net/help/pages/currency#6" >Ежедневное автообновление курса валют.</a> ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Catalog.AutoUpdateCurrenciesHint', 'When the setting is activated, the currency will be updated automatically.<br/><br/>More details: <br/> <a href="https://www.advantshop.net/help/pages/currency#6" >Daily auto-update of exchange rates.</a> ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Catalog.DisplayModeOfPrices_DisplayForCustomersHint', 'Выбираете настройку для кого планируете отображать цены на сайте. (для всех, для зарегистрированных, либо для клиентов определенной группы)<br/><br/>Подробнее: <br/> <a href="https://www.advantshop.net/help/pages/skrytye-tseny#a1" >Видимость цен для пользователя.</a> ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Catalog.DisplayModeOfPrices_DisplayForCustomersHint', 'Choose the setting for whom you plan to display prices on the site. (for everyone, for registered ones, or for clients of a certain group)<br/><br/>More details: <br/> <a href="https://www.advantshop.net/help/pages/skrytye-tseny#a1" >Visibility of prices to the user.</a> ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Catalog.DisplayModeOfPrices_AvalableCustomerGroupsHint', 'Эта настройка доступна, когда выбрана в настройках видимость цен для пользователя "Только пользователям из перечисленных групп".<br/><br/>Подробнее: <br/> <a href="https://www.advantshop.net/help/pages/skrytye-tseny#a2" >Отображение цены для групп пользователей.</a> ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Catalog.DisplayModeOfPrices_AvalableCustomerGroupsHint', 'This setting is available when the visibility of prices for the user "Only to users from the listed groups" is selected in the settings.<br/><br/>More details: <br/> <a href="https://www.advantshop.net/help/pages/skrytye-tseny#a2" >Price display for user groups.</a> ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Catalog.DisplayModeOfPrices_TextInsteadOfPriceHint', 'Данная настройка доступна, когда выбрана одна из двух настроек "Только зарегистрированным пользователям" или "Только пользователям из перечисленных групп". Здесь можно указать текст, который будет выводиться при скрытых ценах.<br/><br/>Подробнее: <br/> <a href="https://www.advantshop.net/help/pages/skrytye-tseny#a3" >Текст, отображаемый при скрытой цене.</a> ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Catalog.DisplayModeOfPrices_TextInsteadOfPriceHint', 'This setting is available when one of the two settings "Only registered users" or "Only users from the listed groups" is selected. Here you can specify the text that will be displayed at hidden prices.<br/><br/>More details: <br/> <a href="https://www.advantshop.net/help/pages/skrytye-tseny#a3" >Text displayed at hidden price  </a> ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Checkout.AmountLimitationLink', '<br/><br/>Подробнее: <br/> <a href="https://www.advantshop.net/help/pages/parametry-zakaza#3" target="_blank" >Контроль наличия товара</a> ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Checkout.AmountLimitationLink', '<br/><br/> More details: <br/> <a href="https://www.advantshop.net/help/pages/parametry-zakaza#3" target="_blank" >Control of goods availability</a> ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Checkout.OutOfStockActionLink', '<br/><br/>Подробнее: <br/> <a href="https://www.advantshop.net/help/pages/function-preorder#2" target="_blank" >Варианты действий при отсутствии товара</a> ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Checkout.OutOfStockActionLink', '<br/><br/> More details: <br/> <a href="https://www.advantshop.net/help/pages/function-preorder#2" target="_blank" >Options for actions in the absence of goods</a> ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Checkout.ProceedToPaymentLink', '<br/><br/>Подробнее: <br/> <a href="https://www.advantshop.net/help/pages/parametry-zakaza#5" target="_blank" >Быстрый переход к оплате</a> ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Checkout.ProceedToPaymentLink', '<br/><br/> More details: <br/> <a href="https://www.advantshop.net/help/pages/parametry-zakaza#5" target="_blank" >Quick transition to payment</a> ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'MinimalOrderPriceForDefaultGroupLink', '<br/><br/>Подробнее: <br/> <a href="https://www.advantshop.net/help/pages/parametry-zakaza#2" target="_blank" >Минимальная сумма заказа</a> ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'MinimalOrderPriceForDefaultGroupLink', '<br/><br/> More details: <br/> <a href="https://www.advantshop.net/help/pages/parametry-zakaza#2" target="_blank" >Minimum order amount</a> ')
UPDATE [Settings].[Localization] SET [ResourceValue] = 'Запрещать оплату заказа до подтверждения менеджером.<br/><br/>Подробнее: <br/> <a href="https://www.advantshop.net/help/pages/parametry-zakaza#4" target="_blank" >Разрешение оплаты заказа после подтверждения менеджером</a>' WHERE [ResourceKey] = 'Admin.SettingsCheckout.CheckoutCommon.ProhibitPayment' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'Prohibit payment for the order until confirmed by the manager. <br/><br/> More details: <br/> <a href="https://www.advantshop.net/help/pages/parametry-zakaza#4" target="_blank" >Allowing payment for the order after confirmation by the manager</a>' WHERE [ResourceKey] = 'Admin.SettingsCheckout.CheckoutCommon.ProhibitPayment' AND [LanguageId] = 2
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Checkout.ShowShoppingCartNumberHint', 'Вывод номера корзины в клиентской части позволяет Вам при общении с клиентом, по телефону, или онлайн-консультанту получать максимально подробную информацию о клиенте и его действиях на сайте. Для вывода информации о клиенте, введите номер корзины в строке поиска в панели администрирования<br/><br/>Подробнее: <br/> <a href="https://www.advantshop.net/help/pages/client-tracking" target="_blank" >Номер корзины (Client Tracking)</a> ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Checkout.ShowShoppingCartNumberHint', 'Displaying the basket number in the client part allows you to receive the most detailed information about the client and his actions on the site when communicating with a client, by phone, or an online consultant. To display information about a client, enter the basket number in the search bar in the administration panel<br/><br/> More details:<br/> <a href="https://www.advantshop.net/help/pages/client-tracking" target="_blank" >Cart number (Client Tracking)</a> ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Checkout.BuyInOneClickActionHint', 'Выбираете действие, которое произойдет, если клиент воспользуется формой покупки в один клик. ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Checkout.BuyInOneClickActionHint', 'Choose the action that will happen if the customer uses the one-click purchase form. ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Checkout.BuyInOneClickDefaultShippingMethodHint', 'Выбираете из доступных методов тот, который будет подставляться автоматически в заказ при оформлении покупки в один клик ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Checkout.BuyInOneClickDefaultShippingMethodHint', 'Choose from the available methods the one that will be automatically inserted into the order when making a purchase in one click ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Checkout.BuyInOneClickDefaultPaymentMethodHint', 'Выбираете из доступных методов тот, который будет подставляться автоматически в заказ при оформлении покупки в один клик ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Checkout.BuyInOneClickDefaultPaymentMethodHint', 'Choose from the available methods the one that will be automatically inserted into the order when making a purchase in one click ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.SettingsCheckout.CheckoutCommon.DefaultFunnelHint', 'Выбираете название лида, которое будет автоматически подставляться при покупке в одни клик, если используется действие: создавать лид. ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.SettingsCheckout.CheckoutCommon.DefaultFunnelHint', 'Select the name of the lead, which will be automatically substituted when buying in one click, if the action is used: create a lead. ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Checkout.MinimalPriceCertificateHint', 'Значение ниже которого клиент не сможет указать номинал сертификата ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Checkout.MinimalPriceCertificateHint', 'A value below which the client will not be able to indicate the face value of the certificate ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Checkout.MaximalPriceCertificateHint', 'Значение выше которого клиент не сможет указать номинал сертификата ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Checkout.MaximalPriceCertificateHint', 'A value above which the client cannot specify the nominal value of the certificate ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Checkout.PrintOrderMapTypeHint', 'Выбираете тип карты, которая будет отображаться при распечатке заказа ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Checkout.PrintOrderMapTypeHint', 'Choose the type of card that will be displayed when printing the order ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Checkout.NextOrderNumberHint', 'Настройка позволяет задать число, с которого нужно начинать или продолжать нумерацию поступающих заказов. ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Checkout.NextOrderNumberHint', 'The setting allows you to set the number from which to start or continue the numbering of incoming orders. ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Checkout.OrderNumberFormatHint', 'Параметр позволяет сделать номер заказа более сложным, нежели простое число. Например, вы можете добавить текущий год и месяц в номер заказа. ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Checkout.OrderNumberFormatHint', 'This parameter allows you to make the order number more complex than a simple number. For example, you can add the current year and month to the order number. ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Analytics.ExportOrders.StatusHint', 'Настройка позволяет выбрать нужный статус, с которым будет выгрузка за весь период с выбранным статусом. ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Analytics.ExportOrders.StatusHint', 'The setting allows you to select the desired status, which will be unloaded for the entire period with the selected status. ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Analytics.ExportOrders.Period.FromHint', 'Данная настройка позволяет выгрузить заказы за определенный период. Для этого необходимо поставить галочку и выбрать период. ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Analytics.ExportOrders.Period.FromHint', 'This setting allows you to upload orders for a certain period. To do this, you must check the box and select the period. ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Analytics.ExportOrders.EncodingHint', 'Выбираете кодировку файла, по умолчанию "Windows 1251" ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Analytics.ExportOrders.EncodingHint', 'Choose the file encoding, by default &quot;Windows 1251&quot; ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Analytics.ExportOrders.PaidHint', 'Настройка позволяет выгрузить заказы за весь период, но только с одни из статусов (оплачен или не оплачен) ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Analytics.ExportOrders.PaidHint', 'The setting allows you to upload orders for the entire period, but only with one of the statuses (paid or not paid) ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Analytics.ExportOrders.OrderSumHint', 'Можете указать диапазон выгрузки по сумме заказа ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Analytics.ExportOrders.OrderSumHint', 'You can specify the range of unloading by order amount ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Analytics.ExportOrders.ShippingHint', 'При выборе необходимого метода доставки, будет выгрузка заказов именно с этим методом ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Analytics.ExportOrders.ShippingHint', 'When choosing the required delivery method, orders will be unloaded with this method ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Analytics.ExportOrders.CityHint', 'Данная настройка позволяет выгрузить заказы с определенного города, ставите галочку в поле настройки и пишите город ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Analytics.ExportOrders.CityHint', 'This setting allows you to unload orders from a specific city, put a tick in the settings field and write the city ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Analytics.ExportOrders.BonusCostHint', 'Данная настройка позволяет выгрузить заказы, оформленные с применением бонусов ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Analytics.ExportOrders.BonusCostHint', 'This setting allows you to upload orders issued with the use of bonuses ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.SettingsCrm.Index.OrderStatusIfFromLeadHint', 'Выбираете статус, который будет проставляться автоматически в заказе, в случае, если заказ был сформирован из лида ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.SettingsCrm.Index.OrderStatusIfFromLeadHint', 'Choose a status that will be put down automatically in the order, if the order was formed from lead ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.SettingsCrm.Index.DefaultLeadListHint', 'Указывается список лидов, в который попадают лиды, для которых список не определен. ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.SettingsCrm.Index.DefaultLeadListHint', 'A list of leads is indicated, which includes leads for which the list is not defined. ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.SettingsCrm.ImportLeads.ColumnSeparatorHint', 'Разделитель, который указан между столбцами или колонками в файле CSV ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.SettingsCrm.ImportLeads.ColumnSeparatorHint', 'Separator that is specified between columns or columns in a CSV file ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.SettingsCrm.ImportLeads.FileEncodingHint', 'Это кодировка, в которой загружается каталог. Обычные кодировки, которые воспринимаются Microsoft Excel, это кодировки UTF-8 и Windows-1251 ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.SettingsCrm.ImportLeads.FileEncodingHint', 'This is the encoding in which the directory is loaded. The usual encodings that Microsoft Excel interprets are UTF-8 and Windows-1251 encodings ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Import.ImportLeads.PropertySeparatorHint', 'Символ, который, должен указываться в файле CSV при перечислении свойств ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Import.ImportLeads.PropertySeparatorHint', 'The character to be specified in the CSV file when listing properties ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Import.ImportLeads.PropertyValueSeparatorHint', 'Символ, который должен указываться в файле CSV при разделении свойства и его значения ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Import.ImportLeads.PropertyValueSeparatorHint', 'The character that must be specified in the CSV file when separating the property and its value ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.SettingsCrm.ImportLeads.HasHeaderHint', 'Данная опция означает, что в импортируемом файле CSV будет верхняя строка с заголовками ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.SettingsCrm.ImportLeads.HasHeaderHint', 'This option means that in the imported CSV file there will be a top line with headers ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.SettingsCrm.ImportLeads.UpdateCustomerHint', 'Если покупатель уже есть в магазине, информация по нему обновится из файла. ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.SettingsCrm.ImportLeads.UpdateCustomerHint', 'If the customer is already in the store, the information about him will be updated from the file. ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Import.ImportCustomers.ColumnSeparatorHint', 'Разделитель, который указан между столбцами или колонками в файле CSV ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Import.ImportCustomers.ColumnSeparatorHint', 'Separator that is specified between columns or columns in a CSV file ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Import.ImportCustomers.FileEncodingHint', 'Это кодировка, в которой загружается каталог. Обычные кодировки, которые воспринимаются Microsoft Excel, это кодировки UTF-8 и Windows-1251 ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Import.ImportCustomers.FileEncodingHint', 'This is the encoding in which the directory is loaded. The usual encodings that Microsoft Excel interprets are UTF-8 and Windows-1251 encodings ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Import.ImportCustomers.DefaultCustomerGroupHint', 'Группа, которая присваивается автоматически покупателям при импорте.')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Import.ImportCustomers.DefaultCustomerGroupHint', 'A group that is automatically assigned to customers upon import.')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Import.ImportCustomers.HasHeaderHint', 'Данная опция означает, что в импортируемом файле CSV будет верхняя строка с заголовками ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Import.ImportCustomers.HasHeaderHint', 'This option means that in the imported CSV file there will be a top line with headers ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Checkout.AgreementDefaultCheckedHint', 'Если настройка включена, то пользовательское соглашение будет принято автоматически ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Checkout.AgreementDefaultCheckedHint', 'If the setting is enabled, the user agreement will be accepted automatically ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.SettingsSystem.SystemCommon.AdministrationPanel', 'Панель администрирования ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.SettingsSystem.SystemCommon.AdministrationPanel', 'Administration Panel ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.SettingsSystem.SystemCommon.AdministrationPanelHint', 'По умолчанию выставлена новая версия, если у вас сайта версии ниже 7.0 можете переключить на старую панель администрирования. Не рекомендуем этого делать, так как многие функции будут недоступны. Обновите магазин до актуальной версии ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.SettingsSystem.SystemCommon.AdministrationPanelHint', 'By default, the new version is set, if you have a site version below 7.0 you can switch to the old administration panel. We do not recommend doing this, as many functions will be unavailable. Update the store to the current version ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.SettingsSystem.SystemCommon.AdminAreaColorSchemeHint', 'Выбираете цвет панели администрирования из доступного списка ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.SettingsSystem.SystemCommon.AdminAreaColorSchemeHint', 'Choose the color of the administration panel from the available list ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.SettingsSystem.SystemCommon.AlphabetHint', 'Выбираете алфавит, который выводится в капче из выпадающего, наибольшую защиту от ботов дает капча вида: численно-буквенный кириллический.<br/><br/>Подробнее: <br/> <a href="https://www.advantshop.net/help/pages/captcha" target="_blank" >Инструкция. Капча</a> ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.SettingsSystem.SystemCommon.AlphabetHint', 'Choose the alphabet, which is displayed in the drop-out cap, the greatest protection from bots gives the dropcha of the view: numerical-letter Cyrillic.<br><br>Learn more: <br><a href="https://www.advantshop.net/help/pages/captcha" target="_blank">Kapcha</a> ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.SystemSettings.CustomMenuHint', 'Настройка позволяет добавить пункт меню в панель администрирования для быстрого перехода к какой-либо настройки, которой вы чаще всего пользуетесь. ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.SystemSettings.CustomMenuHint', 'Customization allows you to add a menu item to the admin panel to quickly navigate to any setting that you use most often. ')
GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Telephony.Zadarma.KeyHint', 'Данные настройки необходимо получить в АТС Zadarma. ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Telephony.Zadarma.KeyHint', 'These settings must be obtained from the Zadarma PBX. ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Telephony.Zadarma.SecretHint', 'Данные настройки необходимо получить в АТС Zadarma. ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Telephony.Zadarma.SecretHint', 'These settings must be obtained from the Zadarma PBX. ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Booking.CategoryImageSizes', 'Размеры изображения категории ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Booking.CategoryImageSizes', 'Category image sizes ')
GO--
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Booking.ServiceImageDimensions', 'Размеры изображения услуги ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Booking.ServiceImageDimensions', 'Featured image size ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.NotifyEMails.EmailForProductDiscussHint', 'Укажите E-mail для уведомлений о новых отзывах о товарах ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.NotifyEMails.EmailForProductDiscussHint', 'Enter E-mail for notifications about new product reviews ')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Grid.NewsTotalString', 'Найдено новостей: {0}')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Grid.NewsTotalString', 'Find news: {0}')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Grid.NewsCategoriesTotalString', 'Найдено категорий новостей: {0}')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Grid.NewsCategoriesTotalString', 'Find news categories: {0}')

GO--

if not exists (Select 1 From [Settings].[Localization] Where [ResourceKey] = 'Admin.Js.Product.FileDoesNotMeetSizeRequirements')
begin
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.Product.FileDoesNotMeetSizeRequirements', 'Файл не соответствует требованиям к размеру')
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.Product.FileDoesNotMeetSizeRequirements', 'File does not meet size requirements')
end

GO--
UPDATE [Settings].[Localization] SET [ResourceValue] = 'Настройка позволяет отображать на главной странице 3 категории товаров: Хит продаж, Новинки, Скидки.<br/><br/> Подробнее:<br/> <a href="https://www.advantshop.net/help/pages/product-on-main" target="_blank">Инструкция. Товары на главной</a>' WHERE [ResourceKey] = 'Admin.Settings.Template.MainPageProductsVisibilityHelp' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'The setting allows you to display on the main page 3 categories of products: Bestsellers, Novelties, Discounts.<br/><br/> More details: <br/><a href="https://www.advantshop.net/help/pages/product-on-main" target="_blank">Instruction. Goods on the main</a>' WHERE [ResourceKey] = 'Admin.Settings.Template.MainPageProductsVisibilityHelp' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Настройка отвечает за количество товара, которое будет отображаться на главной странице в каждом блоке.<br/><br/> Подробнее:<br/> <a href="https://www.advantshop.net/help/pages/product-on-main#6" target="_blank">Инструкция. Как изменить количество товаров на главной</a>' WHERE [ResourceKey] = 'Admin.Settings.Template.CountMainPageProductInSectionHelp' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'The setting is responsible for the quantity of goods that will be displayed on the main page in each block.<br/><br/> More details: <br/><a href="https://www.advantshop.net/help/pages/product-on-main#6" target="_blank">Instruction. How to change the number of products on the main</a>' WHERE [ResourceKey] = 'Admin.Settings.Template.CountMainPageProductInSectionHelp' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Настройка отвечает за количество товара, которое будет отображаться в одной строке каждого блока на главной странице.<br/><br/> Подробнее:<br/><a href="https://www.advantshop.net/help/pages/product-on-main#6" target="_blank">Инструкция. Как изменить количество товаров на главной</a>' WHERE [ResourceKey] = 'Admin.Settings.Template.CountMainPageProductInLineHelp' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'The setting is responsible for the quantity of goods that will be displayed in one line of each block on the main page.<br/><br/> More details: <br/><a href="https://www.advantshop.net/help/pages/product-on-main#6" target="_blank">Instruction. How to change the number of products on the main</a>' WHERE [ResourceKey] = 'Admin.Settings.Template.CountMainPageProductInLineHelp' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Настройка активирует отображение раздела новостей на главной странице<br/><br/> Подробнее:<br/> <a href="https://www.advantshop.net/help/pages/add-news" target="_blank">Инструкция. Настройка раздела новости на сайте</a>' WHERE [ResourceKey] = 'Admin.Settings.Template.NewsVisibilityHelp' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'The setting activates the display of the news section on the main page<br/><br/> More details: <br/><a href="https://www.advantshop.net/help/pages/add-news" target="_blank">Instruction. Setting up the news section on the site</a>' WHERE [ResourceKey] = 'Admin.Settings.Template.NewsVisibilityHelp' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Включенная галочка в настройке позволяет вывести блок "подписка на новости" на главную страницу<br/><br/> Подробнее:<br/> <a href="https://www.advantshop.net/help/pages/add-news#3" target="_blank">Инструкция. Подписка на новости</a>' WHERE [ResourceKey] = 'Admin.Settings.Template.NewsSubscriptionVisibilityHelp' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'NThe included checkmark in the setting allows you to display the "subscribe to news" block on the main page<br/><br/> More details: <br/><a href="https://www.advantshop.net/help/pages/add-news#3" target="_blank">Instruction. News subscription</a>' WHERE [ResourceKey] = 'Admin.Settings.Template.NewsSubscriptionVisibilityHelp' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Настройка позволяет отобразить блок "проверка статуса заказа" на главную страницу<br/><br/> Подробнее:<br/> <a href="https://www.advantshop.net/help/pages/statusy-zakazov#2" target="_blank">Инструкция. Статусы заказов</a>' WHERE [ResourceKey] = 'Admin.Settings.Template.CheckOrderVisibilityHelp' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'The setting allows you to display the block "check order status" on the main page<br/><br/> More details: <br/><a href="https://www.advantshop.net/help/pages/statusy-zakazov#2" target="_blank">Instruction. Order Statuses</a>' WHERE [ResourceKey] = 'Admin.Settings.Template.CheckOrderVisibilityHelp' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Настройка выводит блок подарочных сертификатов на главную страницу<br/><br/> Подробнее:<br/><a href="https://www.advantshop.net/help/pages/kupons-i-podaroxhnie-certifikaty#2" target="_blank">Инструкция. Купоны и подарочные сертификаты</a>' WHERE [ResourceKey] = 'Admin.Settings.Template.GiftSertificateVisibilityHelp' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'Setting displays a block of gift certificates on the main page<br/><br/> More details: <br/><a href="https://www.advantshop.net/help/pages/kupons-i-podaroxhnie-certifikaty#2" target="_blank">Instruction. Coupons and Gift Vouchers</a>' WHERE [ResourceKey] = 'Admin.Settings.Template.GiftSertificateVisibilityHelp' AND [LanguageId] = 2


UPDATE [Settings].[Localization] SET [ResourceValue] = 'Настройка позволяет вывести на главную страницу карусель производителей (логотипы производителей)<br/><br/> Подробнее:<br/> <a href="https://www.advantshop.net/help/pages/brand#5" target="_blank">Инструкция. Бренды</a>' WHERE [ResourceKey] = 'Admin.Settings.Template.BrandCarouselVisibilityHelp' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'The setting allows you to display the carousel of manufacturers on the main page (manufacturers logos)<br/><br/> More details: <br/><a href="https://www.advantshop.net/help/pages/brand#5" target="_blank">Instruction. Brands</a>' WHERE [ResourceKey] = 'Admin.Settings.Template.BrandCarouselVisibilityHelp' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Скорость перелистывания слайда карусели в милисекундах. Рекомендуемые значения 200-1000<br/><br/> Подробнее: <br/> <a href="https://www.advantshop.net/help/pages/karusel#2" target="_blank">Общие настройки карусели</a>' WHERE [ResourceKey] = 'Admin.Settings.Template.CarouselAnimationSpeedHint' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'The speed at which the carousel slide turns in milliseconds. Recommended values ​​200-1000 <br/> <br/> More details: <br/> <a href="https://www.advantshop.net/help/pages/karusel#2" target="_blank"> General settings carousel </a>' WHERE [ResourceKey] = 'Admin.Settings.Template.CarouselAnimationSpeedHint' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Частота перелистывания слайдов карусели. Рекомендуемые значения 5000-10000<br/><br/> Подробнее: <br/> <a href="https://www.advantshop.net/help/pages/karusel#2" target="_blank">Общие настройки карусели</a>' WHERE [ResourceKey] = 'Admin.Settings.Template.CarouselAnimationDelayHint' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'The frequency of turning slide carousel. Recommended values 5000-10000<br/><br/> More details: <br/> <a href="https://www.advantshop.net/help/pages/karusel#2" target="_blank"> General settings carousel </a>' WHERE [ResourceKey] = 'Admin.Settings.Template.CarouselAnimationDelayHint' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Запрашивать подтверждение согласия с условиями пользовательского соглашения. Будет выводится для всех незарегистрированных пользователей.<br/><br/> Подробнее: <br/> <a href="https://www.advantshop.net/help/pages/152-fz " target="_blank">Как соблюсти требования закона 152-ФЗ на платформе AdvantShop</a>' WHERE [ResourceKey] = 'Admin.Settings.SystemSettings.ShowUserAgreementTextNote' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'Require confirmation of consent to the terms of the user agreement. Will be displayed for all unregistered users.<br/><br/> More details: <br/> <a href="https://www.advantshop.net/help/pages/152-fz " target="_blank">How to comply with the requirements of Law 152-FZ on the AdvantShop platform</a>' WHERE [ResourceKey] = 'Admin.Settings.SystemSettings.ShowUserAgreementTextNote' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Настройка позволяет отобразить блок "Проверка статуса заказа" на главную страницу<br/><br/> Подробнее:<br/> <a href="https://www.advantshop.net/help/pages/statusy-zakazov#2" target="_blank">Инструкция. Статусы заказов</a>' WHERE [ResourceKey] = 'Admin.Settings.Template.CheckOrderVisibilityHelp' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'The setting allows you to display the block "Check order status" on the main page<br/><br/> More details: <br/><a href="https://www.advantshop.net/help/pages/statusy-zakazov#2" target="_blank">Instruction. Order Statuses</a>' WHERE [ResourceKey] = 'Admin.Settings.Template.CheckOrderVisibilityHelp' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Включенная галочка в настройке позволяет вывести блок "Подписка на новости" на главную страницу<br/><br/> Подробнее:<br/> <a href="https://www.advantshop.net/help/pages/add-news#3" target="_blank">Инструкция. Подписка на новости</a>' WHERE [ResourceKey] = 'Admin.Settings.Template.NewsSubscriptionVisibilityHelp' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'NThe included checkmark in the setting allows you to display the "Subscribe to news" block on the main page<br/><br/> More details: <br/><a href="https://www.advantshop.net/help/pages/add-news#3" target="_blank">Instruction. News subscription</a>' WHERE [ResourceKey] = 'Admin.Settings.Template.NewsSubscriptionVisibilityHelp' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Настройка позволяет отображать на главной странице 3 категории товаров: Хит продаж, Новинки, Скидки, а также дополнительные списки товаров, добавленные администратором.<br/><br/> Подробнее:<br/> <a href="https://www.advantshop.net/help/pages/product-on-main" target="_blank">Инструкция. Товары на главной</a>' WHERE [ResourceKey] = 'Admin.Settings.Template.MainPageProductsVisibilityHelp' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'The setting allows you to display on the main page 3 categories of products: Bestsellers, Novelties, Discounts, as well as additional product lists added by the administrator.<br/><br/> More details: <br/><a href="https://www.advantshop.net/help/pages/product-on-main" target="_blank">Instruction. Goods on the main</a>' WHERE [ResourceKey] = 'Admin.Settings.Template.MainPageProductsVisibilityHelp' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'В данном поле укажите, какое количество категорий необходимо выводить в каталоге в одной строке.<br/><br/> Подробнее:<br/><a href="https://www.advantshop.net/help/pages/template-settings#4" target="_blank">Инструкция. Товары на главной.</a>' WHERE [ResourceKey] = 'Admin.SettingsCatalog.CatalogCommon.CountCategoriesInLineNote' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'In this field specify, how many categories should be displayed in the catalog on one line.More details: <br/><a href="https://www.advantshop.net/help/pages/template-settings#4" target="_blank">Instruction. Goods on the main.</a>' WHERE [ResourceKey] = 'Admin.SettingsCatalog.CatalogCommon.CountCategoriesInLineNote' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Данная настройка позволяет выставить необходимое количество товаров, отображающихся в одной строке в категории.<br/><br/> Подробнее:<br/> <a href="https://www.advantshop.net/help/pages/template-settings#5" target="_blank">Инструкция. Товары в каталоге.</a>' WHERE [ResourceKey] = 'Admin.Settings.Template.CountCatalogProductInLineNote' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'This setting allows you to set the required number of products displayed in one line in a category.<br/><br/> More details: <br/><a href = "https://www.advantshop.net/help/pages/template-settings#5" target = "_blank"> Instruction. Products in the catalog. </a>' WHERE [ResourceKey] = 'Admin.Settings.Template.CountCatalogProductInLineNote' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Показывать или нет список категорий в нижнем меню сайта. Если категории наряду с обычными пунктами нижнего меню не нужны, отключите опцию.' WHERE [ResourceKey] = 'Admin.SettingsCatalog.CatalogCommon.ShowOrNot' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'Whether or not to show a list of categories in the bottom menu of the site. If categories along with the usual items in the bottom menu are not needed, disable the option.' WHERE [ResourceKey] = 'Admin.SettingsCatalog.CatalogCommon.ShowOrNot' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Показывать артикул товара в каталоге.<br/><br/> Подробнее:<br/> <a href="https://www.advantshop.net/help/pages/catalog-view#6" target="_blank">Как скрыть отображение артикула в каталоге</a>' WHERE [ResourceKey] = 'Admin.SettingsCatalog.CatalogCommon.ShowArticleInTile' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'Show the SKU in the catalog. <br/> <br/> More details: <br/> <a href = "https://www.advantshop.net/help/pages/catalog-view#6" target = "_ blank" > How to hide the display of the article in the catalog </a>' WHERE [ResourceKey] = 'Admin.SettingsCatalog.CatalogCommon.ShowArticleInTile' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'В данном поле пишите рейтинг, который можно установить массово для всех товаров. Значения указываются от 1 до 5.<br/><br/> Подробнее:<br/>  <a href="https://www.advantshop.net/help/pages/catalog-view#10" target="_blank">Инструкция. Как выставить рейтинг товаров вручную.</a>' WHERE [ResourceKey] = 'Admin.Settings.Catalog.SetAllProductsManualRatioNote' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'In this field, write a rating that can be set in bulk for all products.Values ??are from 1 to 5.<br/> <br/> More details: <br/> <a href="https://www.advantshop.net/help/pages/catalog-view#10" target="_blank">Instruction. How to manually rate products.</a>' WHERE [ResourceKey] = 'Admin.Settings.Catalog.SetAllProductsManualRatioNote' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Настройка позволяет вывести фильтр со свойствами в каталог. Другими словами настройка включает работу фильтра в магазине.<br/><br/> Подробнее:<br/>  <a href="https://www.advantshop.net/help/pages/catalog-filtr#5" target="_blank">Инструкция. Как отключить фильтр производителей, фильтр по цене, по размеру, по цвету.</a>' WHERE [ResourceKey] = 'Admin.SettingsCatalog.CatalogCommon.ShowFilterNote' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'Setting allows you to display a filter with properties in a directory. In other words, the setting includes the filter in the store.<br/><br/> More details: <br/><a href="https://www.advantshop.net/help/pages/catalog-filtr#5" target="_blank">Instruction. How to disable the filter manufacturers, filter for the price, size, color.</a>' WHERE [ResourceKey] = 'Admin.SettingsCatalog.CatalogCommon.ShowFilterNote' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Функция пережатия позволяет изменять размеры изображений для ранее загруженных фотографий на сайт.<br/><br/> Подробнее:<br/><a href="https://www.advantshop.net/help/pages/photo-product#2" target="_blank">Инструкция. Как изменить размер изображений?</a>' WHERE [ResourceKey] = 'Admin.SettingsCatalog.CatalogCommon.ResizeGoodsPicturesNote' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'The compression function allows you to resize images for previously uploaded photos to the site.<br/><br/> More details: <br/><a href="https://www.advantshop.net/help/pages/photo-product#2" target="_blank">Instruction. How to resize images?</a>' WHERE [ResourceKey] = 'Admin.SettingsCatalog.CatalogCommon.ResizeGoodsPicturesNote' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Выводить или нет рейтинг (звездочки) для товаров в категории и в карточке товара.<br/><br/> Подробнее:<br/><a href="https://www.advantshop.net/help/pages/catalog-view#7" target="_blank">Как вкл/выкл рейтинг товаров</a>' WHERE [ResourceKey] = 'Admin.SettingsCatalog.CatalogCommon.ShowOrNotStars' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'Whether or not to display the rating (stars) for products in the category and in the product card. <br/> <br/> More details: <br/> <a href = "https://www.advantshop.net/help/pages/catalog -view # 7 "target =" _blank "> How to enable / disable product ratings </a>' WHERE [ResourceKey] = 'Admin.SettingsCatalog.CatalogCommon.ShowOrNotStars' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Опция определяет, какое количество товаров выводить в категории.<br/><br/> Подробнее:<br/><a href="https://www.advantshop.net/help/pages/catalog-view#2" target="_blank">Количество товаров на одной странице</a>' WHERE [ResourceKey] = 'Admin.SettingsCatalog.CatalogCommon.OptionDetermines' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'The option determines how many products to display in the category. <br/> <br/> More details: <br/> <a href = "https://www.advantshop.net/help/pages/catalog-view#2" target = "_ blank"> Number of products per page </a>' WHERE [ResourceKey] = 'Admin.SettingsCatalog.CatalogCommon.OptionDetermines' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Показывать или нет список категорий в нижнем меню сайта. Если категории наряду с обычными пунктами нижнего меню не нужны, отключите опцию.<br/><br/> Подробнее:<br/><a href="https://www.advantshop.net/help/pages/catalog-view#catalog2" target="_blank">Выводить категории в нижнем меню</a>' WHERE [ResourceKey] = 'Admin.SettingsCatalog.CatalogCommon.ShowOrNot' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'Whether or not to show a list of categories in the bottom menu of the site. If categories are not needed along with the usual items in the bottom menu, disable the option. <br/><br/> More details: <br/> <a href = "https://www.advantshop.net/help/pages/catalog-view # catalog2 "target =" _ blank "> Display categories in the bottom menu </a>' WHERE [ResourceKey] = 'Admin.SettingsCatalog.CatalogCommon.ShowOrNot' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'В категории при наведении мыши на товар рядом с основной картинкой товара будут показаны другие фотографии товара.<br/><br/> Подробнее:<br/><a href="https://www.advantshop.net/help/pages/catalog-view#3" target="_blank">Настройка вывода превью фотографий в каталоге</a>' WHERE [ResourceKey] = 'Admin.SettingsCatalog.CatalogCommon.HoverMouseOnProduct' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'In a category, when you hover the mouse over a product, next to the main product picture, other product photos will be shown. <br/> <br/> More details: <br/> <a href = "https://www.advantshop.net/help/pages / catalog-view # 3 "target =" _ blank "> Setting the output of preview photos in the catalog </a>' WHERE [ResourceKey] = 'Admin.SettingsCatalog.CatalogCommon.HoverMouseOnProduct' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Опция определяет, отображать или нет в каталоге у товара небольшую иконку с количеством фотографий у товара.<br/><br/> Подробнее:<br/><a href="https://www.advantshop.net/help/pages/catalog-view#3" target="_blank">Настройка вывода превью фотографий в каталоге</a>' WHERE [ResourceKey] = 'Admin.SettingsCatalog.CatalogCommon.OptionDisplaySmallIcon' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'The option determines whether or not to display a small icon with the number of photos of the product in the product catalog. <br/> <br/> More details: <br/> <a href = "https://www.advantshop.net/help/pages / catalog-view # 3 "target =" _ blank "> Setting the output of preview photos in the catalog </a>' WHERE [ResourceKey] = 'Admin.SettingsCatalog.CatalogCommon.OptionDisplaySmallIcon' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Опция определяет, отображать или нет в каталоге товары, которых нет в наличии.<br/><br/> Подробнее:<br/><a href="https://www.advantshop.net/help/pages/catalog-view#4" target="_blank">Настройка вывода товаров в наличии не в наличии</a>' WHERE [ResourceKey] = 'Admin.SettingsCatalog.CatalogCommon.OptionToDisplayProductsAreNotAvailable' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'The option determines whether or not to display products that are out of stock in the catalog. <br/><br/> More details: <br/> <a href = "https://www.advantshop.net/help/pages/catalog- view # 4 "target =" _blank "> Configuring the display of out of stock items </a>' WHERE [ResourceKey] = 'Admin.SettingsCatalog.CatalogCommon.OptionToDisplayProductsAreNotAvailable' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'При активации этой опции товары, которых нет в наличии, будут перемещаться в конец списка, несмотря на выбранную сортировку.<br/><br/> Подробнее:<br/><a href="https://www.advantshop.net/help/pages/catalog-view#9" target="_blank">Как переместить товары не в наличии в конец списка?</a>' WHERE [ResourceKey] = 'Admin.SettingsCatalog.CatalogCommon.OptionToMoveProductsInTheEndList' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'При активации этой опции товары, которых нет в наличии, будут перемещаться в конец списка, несмотря на выбранную сортировку.<br/><br/> Подробнее:<br/><a href="https://www.advantshop.net/help/pages/catalog-view#9" target="_blank">Как переместить товары не в наличии в конец списка?</a>' WHERE [ResourceKey] = 'Admin.SettingsCatalog.CatalogCommon.OptionToMoveProductsInTheEndList' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Отображение цвета в фильтре зависит от настройки "Режим отображения цвета". Может быть иконкой, текстом или иконка и название цвета.' WHERE [ResourceKey] = 'Admin.SettingsCatalog.CatalogCommon.ColorsDisplayedLikeCubes' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'The color display in the filter depends on the "Color display mode" setting. Can be an icon, text or an icon and color name.' WHERE [ResourceKey] = 'Admin.SettingsCatalog.CatalogCommon.ColorsDisplayedLikeCubes' AND [LanguageId] = 2

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Catalog.ColorsHeaderLink', 'Подробнее:<br/><a href="https://www.advantshop.net/help/pages/spravochniki#params1" target="_blank">Изменение заголовка цвета</a> ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Catalog.ColorsHeaderLink', 'More details: <br/> <a href="https://www.advantshop.net/help/pages/spravochniki#params1" target="_blank"> Changing the title color </a> ')


INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Catalog.SizesHeaderLink', 'Подробнее:<br/><a href="https://www.advantshop.net/help/pages/spravochniki#params1" target="_blank">Изменение заголовка размера</a> ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Catalog.SizesHeaderLink', 'More details: <br/> <a href="https://www.advantshop.net/help/pages/spravochniki#params1" target="_blank"> Changing the size header </a> ')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Catalog.ComplexFilterLink', 'Подробнее:<br/><a href="https://www.advantshop.net/help/pages/icon-color" target="_blank">Отобразить/скрыть иконки цветов в каталоге товаров</a> ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Catalog.ComplexFilterLink', 'More details: <br/> <a href="https://www.advantshop.net/help/pages/icon-color" target="_blank">Show / hide color icons in the product catalog</a> ')

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Какой вид отображения использовать для товаров в категориях.<br/><br/> Подробнее:<br/><a href="https://www.advantshop.net/help/pages/catalog-view#1" target="_blank">Виды отображения каталога</a>' WHERE [ResourceKey] = 'Admin.SettingsCatalog.CatalogCommon.KindOfDisplayForProducts' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'What kind of display to use for products in categories. <br/> <br/> More details: <br/> <a href = "https://www.advantshop.net/help/pages/catalog-view#1" target = "_blank"> Catalog views </a>' WHERE [ResourceKey] = 'Admin.SettingsCatalog.CatalogCommon.KindOfDisplayForProducts' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Какой вид отображения использовать для товаров в результатах поиска.<br/><br/> Подробнее:<br/><a href="https://www.advantshop.net/help/pages/catalog-view#1" target="_blank">Виды отображения каталога</a>' WHERE [ResourceKey] = 'Admin.SettingsCatalog.CatalogCommon.KindOfDisplayForResults' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'What kind of display to use for products in search results.<br/> <br/> More details: <br/> <a href = "https://www.advantshop.net/help/pages/catalog-view#1" target = "_blank"> Catalog views </a>' WHERE [ResourceKey] = 'Admin.SettingsCatalog.CatalogCommon.KindOfDisplayForResults' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Отображать или нет графу "вес" в карточке товара.<br/><br/> Подробнее:<br/><a href="https://www.advantshop.net/help/pages/product-parametry#1" target="_blank">Отображать вес товара</a>' WHERE [ResourceKey] = 'Admin.SettingsCatalog.Product.ShowColumnWeight' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'Whether or not to display the "weight" column in the product card. <br/> <br/> More details: <br/> <a href = "https://www.advantshop.net/help/pages/product-parametry#1" target = "_blank"> Display item weight </a>' WHERE [ResourceKey] = 'Admin.SettingsCatalog.Product.ShowColumnWeight' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Отображать или нет графу "габариты" в карточке товара.<br/><br/> Подробнее:<br/><a href="https://www.advantshop.net/help/pages/product-parametry#3" target="_blank">Отображать габариты товара</a>' WHERE [ResourceKey] = 'Admin.SettingsCatalog.Product.ShowColumnDimensions' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'Whether or not to display the "dimensions" column in the product card. <br/> <br/> More details: <br/> <a href = "https://www.advantshop.net/help/pages/product-parametry#3" target = "_ blank"> Display product dimensions </a>' WHERE [ResourceKey] = 'Admin.SettingsCatalog.Product.ShowColumnDimensions' AND [LanguageId] = 2

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Catalog.ShowStockAvailabilityLink', 'Подробнее:<br/><a href="https://www.advantshop.net/help/pages/product-parametry#4" target="_blank">Отображать наличие товара</a> ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Catalog.ShowStockAvailabilityLink', 'More information: <br/> <a href="https://www.advantshop.net/help/pages/product-parametry#4" target="_blank"> Display product availability </a> ')

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Опция определяет, разрешить или нет добавление отзывов к товарам.<br/><br/> Подробнее:<br/><a href="https://www.advantshop.net/help/pages/product-review#1" target="_blank">Как включить добавление отзывов к товарам</a>' WHERE [ResourceKey] = 'Admin.SettingsCatalog.Product.OptionAllowToAddReviews' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'The option determines whether or not to allow adding reviews to products. <br/> <br/> More details: <br/> <a href = "https://www.advantshop.net/help/pages/product-review#1" target = "_ blank"> How to enable product reviews </a>' WHERE [ResourceKey] = 'Admin.SettingsCatalog.Product.OptionAllowToAddReviews' AND [LanguageId] = 2

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Catalog.ModerateReviewsLink', 'Подробнее:<br/><a href="https://www.advantshop.net/help/pages/product-review#4" target="_blank">Модерировать отзывы перед их добавлением</a> ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Catalog.ModerateReviewsLink', 'More details:<br/> <a href="https://www.advantshop.net/help/pages/product-review#4" target="_blank"> Moderate reviews before adding them </a> ')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Catalog.Reviews.HelpLink', '<br/><br/>Подробнее:<br/><a href="https://www.advantshop.net/help/pages/product-review#3" target="_blank">Отзывы в карточке товара.</a>')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Catalog.Reviews.HelpLink', '<br/><br/>More details:<br/><a href="https://www.advantshop.net/help/pages/product-review#3" target="_blank">Reviews in the product card.</a>')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Catalog.Shipping.HelpLink', '<br/><br/>Подробнее:<br/><a href="https://www.advantshop.net/help/pages/product-properties-delivery-social#2" target="_blank">Отображение доставки в карточке товара</a> ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Catalog.Shipping.HelpLink', '<br/><br/> More details: <br/> <a href="https://www.advantshop.net/help/pages/product-properties-delivery-social#2" target="_blank"> Display delivery in the product card </a> ')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Catalog.ProductName.HelpLink', '<br/><br/>Подробнее:<br/><a href="https://www.advantshop.net/help/pages/cross-marketing#3 " target="_blank">Изменение имени блоков</a> ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Catalog.ProductName.HelpLink', 'More details: <br/> <a href="https://www.advantshop.net/help/pages/cross-marketing#3 "target="_blank"> Changing the block name </a> ')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Catalog.RelatedProductSourceTypeLink', 'Подробнее:<br/><a href="https://www.advantshop.net/help/pages/auto-alternative" target="_blank">Автоматическое создание альтернативных товаров</a> ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Catalog.RelatedProductSourceTypeLink', 'More details:<br/><a href="https://www.advantshop.net/help/pages/auto-alternative" target="_blank"> Automatic creation of alternative products</a> ')

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Максимальные размеры иконки для оплаты (в пикселях).<br/><br/> Подробнее:<br/><a href="https://www.advantshop.net/help/pages/template-settings#14" target="_blank">Иконки для оплаты</a>' WHERE [ResourceKey] = 'Admin.Settings.Template.PaymentIconNote' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'Maximum icon sizes for payment (in pixels).<br/><br/>More details:<br/> <a href = "https://www.advantshop.net/help/pages/template-settings#14" target = "_ blank">Payment icons</a>' WHERE [ResourceKey] = 'Admin.Settings.Template.PaymentIconNote' AND [LanguageId] = 2

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Template.ShippingIconNote', 'Максимальные размеры иконки для доставки (в пикселях). <br/><br/> Подробнее:<br/><a href="https://www.advantshop.net/help/pages/template-settings#15" target="_blank">Иконки для доставки</a>')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Template.ShippingIconNote', 'Maximum size of the icon for delivery (in pixels).<br/><br/> More details: <br/> <a href="https://www.advantshop.net/help/pages/template-settings#15" target="_blank"> Shipping icons </a> ')

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Максимальные размеры изображений производителей (в пикселях).<br/><br/> Подробнее:<br/><a href="https://www.advantshop.net/help/pages/template-settings#7 ," target="_blank">Лого производителя</a>' WHERE [ResourceKey] = 'Admin.Settings.Template.BrandLogoHote' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'Maximum sizes of manufacturer images (in pixels). <br/><br/> More details: <br/><a href = "https://www.advantshop.net/help/pages/template-settings#7," target = "_ blank"> Manufacturer logo </a>' WHERE [ResourceKey] = 'Admin.Settings.Template.BrandLogoHote' AND [LanguageId] = 2

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.SystemSettings.AdditionalHeadMetaTagNoteLink', 'Подробнее:<br/><a href="https://www.advantshop.net/help/pages/podtverzhdenie-prava-vladeniya-saitom#4" target="_blank">Дополнительный тег в Head</a> ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.SystemSettings.AdditionalHeadMetaTagNoteLink', 'More details:<br/><a href="https://www.advantshop.net/help/pages/podtverzhdenie-prava-vladeniya-saitom#4" target="_blank"> Additional head tag </a> ')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Checkout.MinimalOrderPriceForDefaultGroupLink', '<br><br>Подробнее:<br/><a href="https://www.advantshop.net/help/pages/parametry-zakaza#2" target="_blank">Минимальная сумма заказа</a> ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Checkout.MinimalOrderPriceForDefaultGroupLink', '<br><br>More details:<br/><a href="https://www.advantshop.net/help/pages/parametry-zakaza#2" target="_blank">Minimum order amount</a> ')

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Общие' WHERE [ResourceKey] = 'Admin.Settings.SystemSettings.Common' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'Common' WHERE [ResourceKey] = 'Admin.Settings.SystemSettings.Common' AND [LanguageId] = 2

GO--

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Данный номер используется для мобильной версии.
Пример заполнения: +79000000000' 
WHERE [ResourceKey] = 'Admin.Settings.Common.Common.UseMobilePhoneExample' AND [LanguageId] = 1

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.OrderItemsSummary.CertificateTitle', 'Сертификат')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.OrderItemsSummary.CertificateTitle', 'Certificate')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.OrderItemsSummary.AddCertificate', 'cертификат')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.OrderItemsSummary.AddCertificate', 'certificate')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.OrderItemsSummary.AddCertificateTitle', 'Добавить cертификат')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.OrderItemsSummary.AddCertificateTitle', 'Add certificate')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.OrderItemsSummary.CertificateCode', 'Код cертификата')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.OrderItemsSummary.CertificateCode', 'Certificate сode')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.Order.CertificateSavingError', 'Ошибка сохранения cертификата')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.Order.CertificateSavingError', 'Can''t save certificate сode')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.Order.OrderCertificateRemoveError', 'Ошибка удаления cертификата')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.Order.OrderCertificateRemoveError', 'Can''t remove certificate сode')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Orders.CerticateUsed', 'Сертификат уже использован в другом заказе')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Orders.CerticateUsed', 'Certificate already has been used in other order')

GO--
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.SystemSettings.ImageQualityNote', 'Вы можете задать значение в диапазоне от 0 до 100 ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.SystemSettings.ImageQualityNote', 'You can set the value in the range from 0 to 100 ')
GO--

UPDATE [Settings].[Localization] 
SET [ResourceValue] = replace([ResourceValue], ':', '') 
WHERE [ResourceKey] = 'MyAccount.CommonInfo.RegistrationDate'

UPDATE [Settings].[Localization] 
SET [ResourceValue] = replace([ResourceValue], ':', '') 
WHERE [ResourceKey] = 'MyAccount.CommonInfo.Name'

UPDATE [Settings].[Localization] 
SET [ResourceValue] = replace([ResourceValue], ':', '') 
WHERE [ResourceKey] = 'MyAccount.CommonInfo.LastName'

UPDATE [Settings].[Localization] 
SET [ResourceValue] = replace([ResourceValue], ':', '') 
WHERE [ResourceKey] = 'MyAccount.CommonInfo.Patronymic'

UPDATE [Settings].[Localization] 
SET [ResourceValue] = replace([ResourceValue], ':', '') 
WHERE [ResourceKey] = 'MyAccount.CommonInfo.Phone'

UPDATE [Settings].[Localization] 
SET [ResourceValue] = replace([ResourceValue], ':', '') 
WHERE [ResourceKey] = 'MyAccount.CommonInfo.NewsSubscription'

GO--


INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ExportFeeds.IndexYandex.Instruction', 'Инструкция. Выгрузка товаров на Яндекс.Маркет')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ExportFeeds.IndexYandex.Instruction', 'Instruction. Uploading goods to Yandex.Market')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ExportFeeds.IndexGoogle.Instruction', 'Инструкция. Выгрузка товаров на Google Merchant Center')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ExportFeeds.IndexGoogle.Instruction', 'Instruction. Uploading products to Google Merchant Center')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ExportFeeds.IndexAvito.Instruction', 'Инструкция. Настройка выгрузки "Avito Автозагрузка"')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ExportFeeds.IndexAvito.Instruction', 'Instruction. Configuring "Avito Upload Auto Upload"')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ExportFeeds.IndexReseller.Instruction', 'Инструкция. Канал продаж "Реселлеры"')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ExportFeeds.IndexReseller.Instruction', 'Instruction. Sales channel "Resellers"')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.SystemSettings.HideGeneralMenu', 'Скрывать меню')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.SystemSettings.HideGeneralMenu', 'Hide menu')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.SystemSettings.SiteMapFileXmlLinkHint', 'Для перехода к сгенерированной карте сайта нажмите ссылку карты<br/><br/>Подробнее: <br/> <a href="https://www.advantshop.net/help/pages/robots-txt#4" target="_blank" >Адрес карты XML</a>')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.SystemSettings.SiteMapFileXmlLinkHint', 'To go to the generated map of the site, click the map link<br><br>Learn more: <br><a href="https://www.advantshop.net/help/pages/robots-txt#4" target="_blank">XML card address</a>')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.SystemSettings.SiteMapFileHtmlLinkHint', 'Для перехода к сгенерированной карте сайта нажмите ссылку карты<br/><br/>Подробнее: <br/> <a href="https://www.advantshop.net/help/pages/robots-txt#4" target="_blank" >Адрес карты HTML</a>')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.SystemSettings.SiteMapFileHtmlLinkHint', 'To go to the generated map of the site, click the map link<br><br>Learn more: <br><a href="https://www.advantshop.net/help/pages/robots-txt#4" target="_blank">HTML card address</a>')
UPDATE [Settings].[Localization] SET [ResourceValue] = 'Дата последнего обновления файла' WHERE [ResourceKey] = 'Admin.Settings.SystemSettings.SitemapXmlLastUpdateHint' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'Last file update date' WHERE [ResourceKey] = 'Admin.Settings.SystemSettings.SitemapXmlLastUpdateHint' AND [LanguageId] = 2
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.SystemSettings.SitemapHtmlLastUpdateHint', 'Дата последнего обновления файла')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.SystemSettings.SitemapHtmlLastUpdateHint', 'Last file update date')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.System.SiteMapInstruction', 'Инструкция. Карта сайта')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.System.SiteMapInstruction', 'Instruction. Site map')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Js.MyAccount.ChangesSaved', 'Изменения успешно сохранены')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Js.MyAccount.ChangesSaved', 'Changes has been saved')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Js.MyAccount.ChangesNotSaved', 'Не удалось сохранить изменения')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Js.MyAccount.ChangesNotSaved', 'Changes has not been saved')

GO--

UPDATE [Order].[PaymentParam] SET [Value] = 'yoo_money' WHERE [Name] = 'YandexKassa_PaymentType' AND [Value] = 'yandex_money'

GO--

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Основное' WHERE [ResourceKey] = 'Admin.StaticPages.AddEdit.Main' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'Main' WHERE [ResourceKey] = 'Admin.StaticPages.AddEdit.Main' AND [LanguageId] = 2

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Wishlist.WishlistName', 'Избранное')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Wishlist.WishlistName', 'Wishlist')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Compare.CompareName', 'Сравнение')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Compare.CompareName', 'Compare')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Js.Cart.Sku', 'Артикул')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Js.Cart.Sku', 'Sku')

GO-- 


UPDATE [Settings].[Localization] SET [ResourceValue] = 'Подробнее:<br/> <a href="https://www.advantshop.net/help/pages/upload-theme" target="_blank">Тема дизайна</a>' WHERE [ResourceKey] = 'Admin.Design.Index.ThemeHote' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'More details: <br/> <a href="https://www.advantshop.net/help/pages/upload-theme" target="_blank">Design theme</a>' WHERE [ResourceKey] = 'Admin.Design.Index.ThemeHote' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Подробнее:<br/> <a href="https://www.advantshop.net/help/pages/upload-background" target="_blank">Фон дизайна</a>' WHERE [ResourceKey] = 'Admin.Design.Index.BackgroundHote' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'More details: <br/> <a href="https://www.advantshop.net/help/pages/upload-background" target="_blank">Design background</a>' WHERE [ResourceKey] = 'Admin.Design.Index.BackgroundHote' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Подробнее:<br/>  <a href="https://www.advantshop.net/help/pages/upload-color" target="_blank">Цветовая схема</a>' WHERE [ResourceKey] = 'Admin.Design.Index.ColorSchemeHote' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'More details: <br/> <a href="https://www.advantshop.net/help/pages/upload-color" target="_blank">Color Scheme</a>' WHERE [ResourceKey] = 'Admin.Design.Index.ColorSchemeHote' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Структура сайта может быть представлена в двухколоночном и одноколоночном режиме.<br/><br/>Подробнее:<br/> <a href="https://www.advantshop.net/help/pages/template-settings#11" target="_blank">Режим отображения главной страницы.</a>' WHERE [ResourceKey] = 'Admin.Settings.Template.MainPageModeHint' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'The site structure can be presented in two-column and single-column mode.<br/><br/> More details: <br/> <a href="https://www.advantshop.net/help/pages/template-settings#11" target="_blank">Homepage display mode.</a>' WHERE [ResourceKey] = 'Admin.Settings.Template.MainPageModeHint' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Определяет, как будет отображаться меню каталога</br></br>Подробнее:<br/>  <a href="https://www.advantshop.net/help/pages/template-settings#12" target="_blank">Стиль отображения меню</a>' WHERE [ResourceKey] = 'Admin.Settings.Template.MenuStyleHint' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'Catalog menu style</br></br> More details: <br/> <a href="https://www.advantshop.net/help/pages/template-settings#12" target="_blank">Catalog menu style</a>' WHERE [ResourceKey] = 'Admin.Settings.Template.MenuStyleHint' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Настройка позволяет отображать список товаров, которые клиенты просматривали ранее на сайте<br/><br/> Подробнее:<br/>  <a href="https://www.advantshop.net/help/pages/template-settings#6" target="_blank">Вы уже смотрели</a>' WHERE [ResourceKey] = 'Admin.Settings.Template.RecentlyViewVisibilityHint' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'The setting allows you to display a list of products that customers viewed previously on the site.<br/><br/> More details: <br/> <a href="https://www.advantshop.net/help/pages/template-settings#6" target="_blank">You already watched</a>' WHERE [ResourceKey] = 'Admin.Settings.Template.RecentlyViewVisibilityHint' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Настройка позволяет сменить язык магазина (меняются кнопки, панель администрирования, товары останутся на том языке, на котором заполняли магазин).<br/><br/>Подробнее:<br/> <a href="https://www.advantshop.net/help/pages/change-buttons#4" target="_blank">Изменение языка магазина</a>' WHERE [ResourceKey] = 'Admin.Settings.SystemSettings.SiteLanguageNote' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'The setting allows you to change the language of the store (buttons, admin panel change, the products will remain in the language in which the store was filled).<br/><br/> More details:<br/> <a href="https://www.advantshop.net/help/pages/change-buttons#4" target="_blank">Change store language</a>' WHERE [ResourceKey] = 'Admin.Settings.SystemSettings.SiteLanguageNote' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Настройка позволяет выводить для всех посетителей сайта всплывающее окно с уведомлением о том, что для функционирования сайта Вы осуществляете сбор данных пользователя (cookie, данные об IP-адресе и местоположении) в соответствии с законом 152-ФЗ.<br/><br/>Подробнее:<br/>  <a href="https://www.advantshop.net/help/pages/152-fz" target="_blank">Как соблюсти требования закона 152-ФЗ на платформе AdvantShop</a>' WHERE [ResourceKey] = 'Admin.Settings.SystemSettings.ShowCookiesPolicyMessageNote' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'The setting allows you to display a pop-up window for all site visitors with a notification that for the operation of the site you are collecting user data (cookies, IP address and location data) in accordance with Law 152-FZ.<br/><br/>More details:<br/><a href="https://www.advantshop.net/help/pages/152-fz" target="_blank">How to comply with the requirements of law 152-ФЗ on the AdvantShop platform</a>' WHERE [ResourceKey] = 'Admin.Settings.SystemSettings.ShowCookiesPolicyMessageNote' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Активность данной настройки позволяет клиентам добавлять товары в свой список желаний.<br/><br/>Подробнее:<br/> <a href="https://www.advantshop.net/help/pages/template-settings#6" target="_blank">Cписок желаний</a>' WHERE [ResourceKey] = 'Admin.Settings.Template.WishListVisibilityHint' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'The activity of this setting allows customers to add products to their wish list.<br/><br/>More details:<br/> <a href="https://www.advantshop.net/help/pages/template-settings#6" target="_blank">Wish List</a>' WHERE [ResourceKey] = 'Admin.Settings.Template.WishListVisibilityHint' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Показывать слайдер на главной странице или нет <br/><br/> Подробнее:<br/><a href="https://www.advantshop.net/help/pages/karusel" target="_blank">Настройка слайдера/карусели</a>' WHERE [ResourceKey] = 'Admin.Settings.Template.CarouselVisibilityHint' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'Show carousel slider on main page <br/><br/> More details: <br/> <a href="https://www.advantshop.net/help/pages/karusel" target="_blank">Slider / carousel setting </a>' WHERE [ResourceKey] = 'Admin.Settings.Template.CarouselVisibilityHint' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Настройка позволяет отображать на главной странице 3 категории товаров: Хит продаж, Новинки, Скидки, а также дополнительные списки товаров, добавленные администратором.<br/><br/> Подробнее:<br/> <a href="https://www.advantshop.net/help/pages/product-on-main" target="_blank">Товары на главной</a>' WHERE [ResourceKey] = 'Admin.Settings.Template.MainPageProductsVisibilityHelp' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'The setting allows you to display on the main page 3 categories of products: Bestsellers, Novelties, Discounts, as well as additional product lists added by the administrator.<br/><br/> More details: <br/><a href="https://www.advantshop.net/help/pages/product-on-main" target="_blank">Goods on the main</a>' WHERE [ResourceKey] = 'Admin.Settings.Template.MainPageProductsVisibilityHelp' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Настройка отвечает за количество товара, которое будет отображаться на главной странице в каждом блоке.<br/><br/> Подробнее:<br/> <a href="https://www.advantshop.net/help/pages/product-on-main#6" target="_blank">Как изменить количество товаров на главной</a>' WHERE [ResourceKey] = 'Admin.Settings.Template.CountMainPageProductInSectionHelp' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'The setting is responsible for the quantity of goods that will be displayed on the main page in each block.<br/><br/> More details: <br/><a href="https://www.advantshop.net/help/pages/product-on-main#6" target="_blank">How to change the number of products on the main</a>' WHERE [ResourceKey] = 'Admin.Settings.Template.CountMainPageProductInSectionHelp' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Настройка отвечает за количество товара, которое будет отображаться в одной строке каждого блока на главной странице.<br/><br/> Подробнее:<br/><a href="https://www.advantshop.net/help/pages/product-on-main#6" target="_blank">Как изменить количество товаров на главной</a>' WHERE [ResourceKey] = 'Admin.Settings.Template.CountMainPageProductInLineHelp' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'The setting is responsible for the quantity of goods that will be displayed in one line of each block on the main page.<br/><br/> More details: <br/><a href="https://www.advantshop.net/help/pages/product-on-main#6" target="_blank">How to change the number of products on the main</a>' WHERE [ResourceKey] = 'Admin.Settings.Template.CountMainPageProductInLineHelp' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Настройка активирует отображение раздела новостей на главной странице<br/><br/> Подробнее:<br/> <a href="https://www.advantshop.net/help/pages/add-news" target="_blank">Настройка раздела новости на сайте</a>' WHERE [ResourceKey] = 'Admin.Settings.Template.NewsVisibilityHelp' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'The setting activates the display of the news section on the main page<br/><br/> More details: <br/><a href="https://www.advantshop.net/help/pages/add-news" target="_blank">Setting up the news section on the site</a>' WHERE [ResourceKey] = 'Admin.Settings.Template.NewsVisibilityHelp' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Включенная галочка в настройке позволяет вывести блок "Подписка на новости" на главную страницу<br/><br/> Подробнее:<br/> <a href="https://www.advantshop.net/help/pages/add-news#3" target="_blank">Подписка на новости</a>' WHERE [ResourceKey] = 'Admin.Settings.Template.NewsSubscriptionVisibilityHelp' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'NThe included checkmark in the setting allows you to display the "Subscribe to news" block on the main page<br/><br/> More details: <br/><a href="https://www.advantshop.net/help/pages/add-news#3" target="_blank">News subscription</a>' WHERE [ResourceKey] = 'Admin.Settings.Template.NewsSubscriptionVisibilityHelp' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Настройка позволяет отобразить блок "Проверка статуса заказа" на главную страницу<br/><br/> Подробнее:<br/> <a href="https://www.advantshop.net/help/pages/statusy-zakazov#2" target="_blank">Статусы заказов</a>' WHERE [ResourceKey] = 'Admin.Settings.Template.CheckOrderVisibilityHelp' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'The setting allows you to display the block "Check order status" on the main page<br/><br/> More details: <br/><a href="https://www.advantshop.net/help/pages/statusy-zakazov#2" target="_blank">Order Statuses</a>' WHERE [ResourceKey] = 'Admin.Settings.Template.CheckOrderVisibilityHelp' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Настройка выводит блок подарочных сертификатов на главную страницу<br/><br/> Подробнее:<br/><a href="https://www.advantshop.net/help/pages/kupons-i-podaroxhnie-certifikaty#2" target="_blank">Купоны и подарочные сертификаты</a>' WHERE [ResourceKey] = 'Admin.Settings.Template.GiftSertificateVisibilityHelp' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'Setting displays a block of gift certificates on the main page<br/><br/> More details: <br/><a href="https://www.advantshop.net/help/pages/kupons-i-podaroxhnie-certifikaty#2" target="_blank">Coupons and Gift Vouchers</a>' WHERE [ResourceKey] = 'Admin.Settings.Template.GiftSertificateVisibilityHelp' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Настройка позволяет вывести на главную страницу карусель производителей (логотипы производителей)<br/><br/> Подробнее:<br/> <a href="https://www.advantshop.net/help/pages/brand#5" target="_blank">Бренды</a>' WHERE [ResourceKey] = 'Admin.Settings.Template.BrandCarouselVisibilityHelp' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'The setting allows you to display the carousel of manufacturers on the main page (manufacturers logos)<br/><br/> More details: <br/><a href="https://www.advantshop.net/help/pages/brand#5" target="_blank">Brands</a>' WHERE [ResourceKey] = 'Admin.Settings.Template.BrandCarouselVisibilityHelp' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'В данном поле укажите, какое количество категорий необходимо выводить в каталоге в одной строке.<br/><br/> Подробнее:<br/><a href="https://www.advantshop.net/help/pages/template-settings#4" target="_blank">Товары на главной.</a>' WHERE [ResourceKey] = 'Admin.SettingsCatalog.CatalogCommon.CountCategoriesInLineNote' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'In this field specify, how many categories should be displayed in the catalog on one line.More details: <br/><a href="https://www.advantshop.net/help/pages/template-settings#4" target="_blank">Goods on the main.</a>' WHERE [ResourceKey] = 'Admin.SettingsCatalog.CatalogCommon.CountCategoriesInLineNote' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Настройка позволяет на странице бренда отображать категории, в которых имеются товары данного бренда.<br/><br/>Подробнее:<br/> <a href="https://www.advantshop.net/help/pages/brand#4" target="_blank">Как настроить отображение товаров на странице "Бренды".</a>' WHERE [ResourceKey] = 'Admin.Settings.Template.ShowCategoryTreeInBrandHote' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'The setting allows you to display on the brand page the categories in which the goods of this brand.<br/><br/>More details: <br/><a href="https://www.advantshop.net/help/pages/brand#4" target="_blank">How to customize the display of products on the Brands page.</a>' WHERE [ResourceKey] = 'Admin.Settings.Template.ShowCategoryTreeInBrandHote' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Настройка позволяет выводить на странице бренда список товаров данного бренда. <br/><br/>Подробнее:<br/>  <a href="https://www.advantshop.net/help/pages/brand#4" target="_blank">Как настроить отображение товаров на странице "Бренды".</a>' WHERE [ResourceKey] = 'Admin.Settings.Template.ShowProductsInBrandHote' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'The setting allows you to display on the brand page a list of products of this brand.<br/><br/>More details: <br/><a href="https://www.advantshop.net/help/pages/brand#4" target="_blank">How to customize the display of products on the Brands page.</a>' WHERE [ResourceKey] = 'Admin.Settings.Template.ShowProductsInBrandHote' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'В поле пишите то количество, которое требуется для отображения на одной страниц.<br/><br/>Подробнее:<br/> <a href="https://www.advantshop.net/help/pages/brand#4" target="_blank">Как настроить отображение товаров на странице "Бренды".</a>' WHERE [ResourceKey] = 'Admin.Settings.Template.BrandsPerPageHote' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'In the field, write the amount that is required to display on one page.<br/><br/>More details: <br/><a href="https://www.advantshop.net/help/pages/brand#4" target="_blank">How to customize the display of products on the Brands page.</a>' WHERE [ResourceKey] = 'Admin.Settings.Template.BrandsPerPageHote' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Максимальные размеры изображений новостей (в пикселях).<br/><br/>Подробнее:<br/> <a href="https://www.advantshop.net/help/pages/add-news" target="_blank">Настройка раздела новости на сайте.</a>' WHERE [ResourceKey] = 'Admin.Settings.Template.NewsImageHote' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'Maximum sizes of news images (in pixels).<br/><br/>More details: <br/> <a href="https://www.advantshop.net/help/pages/add-news" target="_blank">Setting up the news section on the site.</a>' WHERE [ResourceKey] = 'Admin.Settings.Template.NewsImageHote' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Указываете текст, который будет отображаться в качестве заголовка в блоке подписки на новости.<br/><br/>Подробнее:<br/> <a href="https://www.advantshop.net/help/pages/add-news#3" target="_blank">Настройка раздела новости на сайте.</a>' WHERE [ResourceKey] = 'Admin.Settings.News.MainPageTextNote' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'Specify the text that will be displayed as the title in the newsletter subscription block<br/><br/>More details: <br/><a href="https://www.advantshop.net/help/pages/add-news#3" target="_blank">Setting up the news section on the site.</a>' WHERE [ResourceKey] = 'Admin.Settings.News.MainPageTextNote' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'В данной настройке указываете число - количество новостей, которые будут выводится в списке всех новостей.<br/><br/>Подробнее:<br/> <a href="https://www.advantshop.net/help/pages/add-news" target="_blank">Настройка раздела новости на сайте.</a>' WHERE [ResourceKey] = 'Admin.Settings.News.NewsPerPageNote' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'In this setting, specify the number - the number of news that will be displayed in the list of all news.<br/><br/>More details: <br/><a href="https://www.advantshop.net/help/pages/add-news" target="_blank">Setting up the news section on the site.</a>' WHERE [ResourceKey] = 'Admin.Settings.News.NewsPerPageNote' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Впишите число - количество новостей отображаемых на главной странице.<br/><br/> Подробнее:<br/><a href="https://www.advantshop.net/help/pages/add-news" target="_blank">Настройка раздела новости на сайте.</a>' WHERE [ResourceKey] = 'Admin.Settings.News.NewsMainPageCountNote' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'Enter the number - the number of news displayed on the main page.<br/><br/>More details: <br/><a href="https://www.advantshop.net/help/pages/add-news" target="_blank">Setting up the news section on the site.</a>' WHERE [ResourceKey] = 'Admin.Settings.News.NewsMainPageCountNote' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Витрина будет доступна только администратору и модераторам. Остальные посетители увидят страницу-заглушку. Отредактировать текст на ней можно в статическом блоке "Витрина закрыта".<br/><br/> Подробнее:<br/><a href="https://www.advantshop.net/help/pages/shop-closed-page" target="_blank">Закрытие витрины</a>' WHERE [ResourceKey] = 'Admin.Settings.SystemSettings.IsStoreClosedNote' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'The showcase will be available only to the administrator and moderators. Other visitors will see a stub page. You can edit the text on it in the static block "Showcase closed". <br/> <br/> More details: <br/> <a href = "https://www.advantshop.net/help/pages/shop-closed- page "target =" _blank "> Close the showcase </a>' WHERE [ResourceKey] = 'Admin.Settings.SystemSettings.IsStoreClosedNote' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Включить или выключить In-place редактирование в клиентской части для администратора.<br/><br/> Подробнее:<br/><a href="https://www.advantshop.net/help/pages/html-editor#2" target="_blank">Как добавить таблицу</a>' WHERE [ResourceKey] = 'Admin.Settings.SystemSettings.EnableInplaceNote' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'Enable or disable In-place editing in the client-side for admin. <br/> <br/> More details: <br/> <a href = "https://www.advantshop.net/help/pages/html-editor# 2 "target =" _blank "> How to add a table </a>' WHERE [ResourceKey] = 'Admin.Settings.SystemSettings.EnableInplaceNote' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Включать или нет отображение нижней вспомогательной панели в клиентской части магазина.<br/><br/> Подробнее:<br/><a href="https://www.advantshop.net/help/pages/footer#6" target="_blank">Нижняя панель сайта</a>' WHERE [ResourceKey] = 'Admin.Settings.SystemSettings.DisplayToolBarBottomNote' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'Whether or not to enable the display of the bottom subbar in the front end of the store. <br/> <br/> More details: <br/> <a href = "https://www.advantshop.net/help/pages/footer#6" target = "_ blank"> Site Bottom Panel </a>' WHERE [ResourceKey] = 'Admin.Settings.SystemSettings.DisplayToolBarBottomNote' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Использовать ли механизм определения города клиента.<br/><br/> Подробнее:<br/><a href="https://www.advantshop.net/help/pages/city-autodetection#1" target="_blank">Настройка автоопределения города</a>' WHERE [ResourceKey] = 'Admin.Settings.SystemSettings.DisplayCityInTopPanelNote' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'Whether to use the client city detection mechanism. <br/> <br/> More details: <br/> <a href = "https://www.advantshop.net/help/pages/city-autodetection#1" target = "_ blank "> Autodetect city detection </a>' WHERE [ResourceKey] = 'Admin.Settings.SystemSettings.DisplayCityInTopPanelNote' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Использовать окошко уточнения верно ли выбран город, в котором находится клиент.<br/><br/> Подробнее:<br/><a href="https://www.advantshop.net/help/pages/city-autodetection#1" target="_blank">Настройка автоопределения города</a>' WHERE [ResourceKey] = 'Admin.Settings.SystemSettings.DisplayCityBubbleNote' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'Use the window to clarify if the city where the client is located is correctly selected. <br/> <br/> More details: <br/> <a href = "https://www.advantshop.net/help/pages/city-autodetection# 1 "target =" _blank "> Autodetecting city </a>' WHERE [ResourceKey] = 'Admin.Settings.SystemSettings.DisplayCityBubbleNote' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Опция включает или отключает отображение надписей об Copyright.<br/><br/> Подробнее:<br/><a href="https://www.advantshop.net/help/pages/footer#4" target="_blank">Copyright</a>' WHERE [ResourceKey] = 'Admin.Settings.SystemSettings.ShowCopyrightNote' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'This option enables or disables the display of Copyright labels. <br/> <br/> Details: <br/> <a href = "https://www.advantshop.net/help/pages/footer#4" target = "_ blank "> Copyright </a>' WHERE [ResourceKey] = 'Admin.Settings.SystemSettings.ShowCopyrightNote' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Если настройка включена, то пользовательское соглашение будет принято автоматически <br/><br/> Подробнее: <br/> <a href="https://www.advantshop.net/help/pages/152-fz " target="_blank">Как соблюсти требования закона 152-ФЗ на платформе AdvantShop</a>' WHERE [ResourceKey] = 'Admin.Settings.Checkout.AgreementDefaultCheckedHint' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'If the setting is enabled, the user agreement will be accepted automatically. <br/> <br/> Details: <br/> <a href = "https://www.advantshop.net/help/pages/152-fz" target = " _blank "> How to comply with the requirements of Law 152-FZ on the AdvantShop platform </a>' WHERE [ResourceKey] = 'Admin.Settings.Checkout.AgreementDefaultCheckedHint' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Данная настройка позволяет просмотреть кратко карточку товара не переходя во внутрь.<br/><br/> Подробнее: <br/> <a href="https://www.advantshop.net/help/pages/catalog-view#11" target="_blank">Быстрый просмотр товара</a>' WHERE [ResourceKey] = 'Admin.SettingsCatalog.CatalogCommon.ShowQuickViewNote' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'This setting allows you to view a short product card without going inside. <br/> <br/> More details: <br/> <a href = "https://www.advantshop.net/help/pages/catalog-view#11 "target =" _ blank "> Quick view of the product </a>' WHERE [ResourceKey] = 'Admin.SettingsCatalog.CatalogCommon.ShowQuickViewNote' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Данная настройка позволяет выставить необходимое количество товаров, отображающихся в одной строке в категории.<br/><br/> Подробнее:<br/> <a href="https://www.advantshop.net/help/pages/template-settings#5" target="_blank">Товары в каталоге.</a>' WHERE [ResourceKey] = 'Admin.Settings.Template.CountCatalogProductInLineNote' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'This setting allows you to set the required number of products displayed in one line in a category.<br/><br/> More details: <br/><a href = "https://www.advantshop.net/help/pages/template-settings#5" target = "_blank">Products in the catalog. </a>' WHERE [ResourceKey] = 'Admin.Settings.Template.CountCatalogProductInLineNote' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = ', в клиентской части, рядом с названием категории в скобочках, выводится количество товаров в ней.<br/><br/> Подробнее:<br/> <a href="https://www.advantshop.net/help/pages/catalog-view#12" target="_blank">Показывать число товаров в категории</a>' WHERE [ResourceKey] = 'Admin.SettingsCatalog.CatalogCommon.InTheClientPart' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = ', in the client side, next to the category name in brackets, the number of products in it is displayed. <br/> <br/> More details: <br/> <a href = "https://www.advantshop.net/help/pages / catalog-view # 12 "target =" _ blank ">Show the number of products in a category</a>' WHERE [ResourceKey] = 'Admin.SettingsCatalog.CatalogCommon.InTheClientPart' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'В данном поле пишите рейтинг, который можно установить массово для всех товаров. Значения указываются от 1 до 5.<br/><br/> Подробнее:<br/>  <a href="https://www.advantshop.net/help/pages/catalog-view#10" target="_blank">Как выставить рейтинг товаров вручную.</a>' WHERE [ResourceKey] = 'Admin.Settings.Catalog.SetAllProductsManualRatioNote' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'In this field, write a rating that can be set in bulk for all products.Values ??are from 1 to 5.<br/> <br/> More details: <br/> <a href="https://www.advantshop.net/help/pages/catalog-view#10" target="_blank">How to manually rate products.</a>' WHERE [ResourceKey] = 'Admin.Settings.Catalog.SetAllProductsManualRatioNote' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Настройка позволяет вывести фильтр со свойствами в каталог. Другими словами настройка включает работу фильтра в магазине.<br/><br/> Подробнее:<br/>  <a href="https://www.advantshop.net/help/pages/catalog-filtr#5" target="_blank">Как отключить фильтр производителей, фильтр по цене, по размеру, по цвету.</a>' WHERE [ResourceKey] = 'Admin.SettingsCatalog.CatalogCommon.ShowFilterNote' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'Setting allows you to display a filter with properties in a directory. In other words, the setting includes the filter in the store.<br/><br/> More details: <br/><a href="https://www.advantshop.net/help/pages/catalog-filtr#5" target="_blank">How to disable the filter manufacturers, filter for the price, size, color.</a>' WHERE [ResourceKey] = 'Admin.SettingsCatalog.CatalogCommon.ShowFilterNote' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Функция пережатия позволяет изменять размеры изображений для ранее загруженных фотографий на сайт.<br/><br/> Подробнее:<br/><a href="https://www.advantshop.net/help/pages/photo-product#2" target="_blank">Как изменить размер изображений?</a>' WHERE [ResourceKey] = 'Admin.SettingsCatalog.CatalogCommon.ResizeGoodsPicturesNote' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'The compression function allows you to resize images for previously uploaded photos to the site.<br/><br/> More details: <br/><a href="https://www.advantshop.net/help/pages/photo-product#2" target="_blank">How to resize images?</a>' WHERE [ResourceKey] = 'Admin.SettingsCatalog.CatalogCommon.ResizeGoodsPicturesNote' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Определяет, выводить или нет блок сравнения товаров.<br/><br/> Подробнее:<br/><a href="https://www.advantshop.net/help/pages/compare" target="_blank">Сравнение товаров</a>' WHERE [ResourceKey] = 'Admin.SettingsCatalog.CatalogCommon.DeterminesDisplayOrNot' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'Determines whether or not to display the product comparison block. <br/> <br/> More details: <br/> <a href="https://www.advantshop.net/help/pages/compare" target="_blank"> Compare products </a>' WHERE [ResourceKey] = 'Admin.SettingsCatalog.CatalogCommon.DeterminesDisplayOrNot' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Показывать количество фотографий' WHERE [ResourceKey] = 'Admin.Settings.Catalog.EnablePhotoPreviews' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'Show number of photos' WHERE [ResourceKey] = 'Admin.Settings.Catalog.EnablePhotoPreviews' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Вы можете выбрать как показывать цвета: иконка, текст (название цвета) или иконка и название цвета<br/><br/> Подробнее:<br/><a href="https://www.advantshop.net/help/pages/spravochniki#7" target="_blank">Настройка варианта представления</a>' WHERE [ResourceKey] = 'Admin.Settings.Catalog.ColorsViewModeHint' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'You can choose how to display colors: icon, text (color name) or icon and color name <br/> <br/> More details: <br/> <a href = "https://www.advantshop.net/help/ pages / spravochniki # 7 "target =" _blank "> Configuring the Presentation Option </a>' WHERE [ResourceKey] = 'Admin.Settings.Catalog.ColorsViewModeHint' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Включает приближение фотографии товара при наведении курсора мыши.<br/><br/> Подробнее:<br/><a href="https://www.advantshop.net/help/pages/lupa" target="_blank">Увеличение фотографии с помощью функции "Лупа"</a>' WHERE [ResourceKey] = 'Admin.SettingsCatalog.Product.TurnPhotoCloser' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'Enables zooming in on a product photo on hover. <br/> <br/> More details: <br/> <a href="https://www.advantshop.net/help/pages/lupa" target="_blank"> Enlarge a photo with the Magnifier </a>' WHERE [ResourceKey] = 'Admin.SettingsCatalog.Product.TurnPhotoCloser' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = ', голосовать за полезность отзыва смогут только зарегистрированные пользователи<br/><br/> Подробнее:<br/><a href="https://www.advantshop.net/help/pages/product-review#5" target="_blank">Голосование за отзывы</a>' WHERE [ResourceKey] = 'Admin.SettingsCatalog.Product.DescriptionReviewsVoiteOnlyRegisteredUsers' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = ', only registered users will be able to vote for the usefulness of the review. <br/> <br/> More details: <br/> <a href = "https://www.advantshop.net/help/pages/product-review#5" target = "_blank"> Vote for reviews </a>' WHERE [ResourceKey] = 'Admin.SettingsCatalog.Product.DescriptionReviewsVoiteOnlyRegisteredUsers' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Максимальное кол-во товаров из назначенной категории<br/><br/> Подробнее:<br/><a href="https://www.advantshop.net/help/pages/cross-marketing#4" target="_blank">Максимальное число выводимых товаров</a>' WHERE [ResourceKey] = 'Admin.SettingsCatalog.Product.MaximumNumberOfProducts' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'Maximum number of products from the assigned category <br/> <br/> More details: <br/> <a href = "https://www.advantshop.net/help/pages/cross-marketing#4" target = " _blank "> Maximum number of products displayed </a>' WHERE [ResourceKey] = 'Admin.SettingsCatalog.Product.MaximumNumberOfProducts' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'В категории при наведении мыши на товар рядом с основной картинкой товара будут показаны другие фотографии товара.<br/><br/> Подробнее:<br/><a href="https://www.advantshop.net/help/pages/catalog-view#3" target="_blank">Настройка вывода количества фотографий в каталоге</a>' WHERE [ResourceKey] = 'Admin.SettingsCatalog.CatalogCommon.HoverMouseOnProduct' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'In a category, when you hover the mouse over a product, next to the main product picture, other product photos will be shown. <br/> <br/> More details: <br/> <a href = "https://www.advantshop.net/help/pages / catalog-view # 3 "target =" _ blank "> Setting the display of the number of photos in the catalog </a>' WHERE [ResourceKey] = 'Admin.SettingsCatalog.CatalogCommon.HoverMouseOnProduct' AND [LanguageId] = 2

GO-- 

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Template.ColorsPictureHelpLink', 'Подробнее:<br/><a href="https://www.advantshop.net/help/pages/photo-product#6" target="_blank">Размеры картинки для кубика цвета</a> ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Template.ColorsPictureHelpLink', 'More details: <br/> <a href="https://www.advantshop.net/help/pages/photo-product#6" target="_blank"> Color block image dimensions </a>')

GO-- 

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Template.MainPageCategoriesVisibilityTitleLink', 'Отвечает за вывод категорий на главной странице. <br><br>Подробнее:<br><a href="https://www.advantshop.net/help/pages/template-settings#3-1" target="_blank">Категории на главной</a>')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Template.MainPageCategoriesVisibilityTitleLink', 'Responsible for displaying categories on the main page. <br> <br>More details: <br> <a href="https://www.advantshop.net/help/pages/template-settings#3-1" target="_blank"> Main categories </ a >')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Template.CountMainPageCategoriesInLineTitleLink', 'Отвечает за количество категорий,которое будет отображаться в одной строке каждого блока на главной странице.<br><br>Подробнее:<br><a href="https://www.advantshop.net/help/pages/template-settings#3-1" target="_blank">Категории на главной</a>')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Template.CountMainPageCategoriesInLineTitleLink', 'Responsible for the number of categories that will be displayed in one line of each block on the main page. <br> <br>More details: <br> <a href="https://www.advantshop.net/help/pages/template-settings#3-1" target="_blank"> Main categories </ a >')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Template.CountMainPageCategoriesInSectionTitleLink', 'Отвечает за количество категорий,которое будет отображаться на главной странице в каждом блоке. <br><br>Подробнее:<br><a href="https://www.advantshop.net/help/pages/template-settings#3-1" target="_blank">Категории на главной</a>')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Template.CountMainPageCategoriesInSectionTitleLink', 'Responsible for the number of categories that will be displayed on the main page in each block. <br> <br>More details: <br> <a href="https://www.advantshop.net/help/pages/template-settings#3-1" target="_blank"> Main categories </ a >')

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Настройка позволяет вывести фильтр со свойствами в каталог. Другими словами настройка включает работу фильтра в магазине.<br/><br/> Подробнее:<br/>  <a href="https://www.advantshop.net/help/pages/catalog-filtr" target="_blank">Фильтр</a>' WHERE [ResourceKey] = 'Admin.SettingsCatalog.CatalogCommon.ShowFilterNote' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'Setting allows you to display a filter with properties in a directory. In other words, the setting includes the filter in the store.<br/><br/> More details: <br/><a href="https://www.advantshop.net/help/pages/catalog-filtr" target="_blank">Filter</a>' WHERE [ResourceKey] = 'Admin.SettingsCatalog.CatalogCommon.ShowFilterNote' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Выводить или нет фильтр по цене. <br/><br/> Подробнее:<br/>  <a href="https://www.advantshop.net/help/pages/catalog-filtr#5" target="_blank">Как отключить фильтр по цене</a>' WHERE [ResourceKey] = 'Admin.SettingsCatalog.CatalogCommon.FilterByPrice' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'Whether or not to display a filter by price. <br/> <br/> More details: <br/> <a href="https://www.advantshop.net/help/pages/catalog-filtr#5" target="_blank"> How to disable price filter </a>' WHERE [ResourceKey] = 'Admin.SettingsCatalog.CatalogCommon.FilterByPrice' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Выводить или нет фильтр по производителям.<br/><br/> Подробнее:<br/>  <a href="https://www.advantshop.net/help/pages/catalog-filtr#5" target="_blank">Как отключить фильтр производителей</a>' WHERE [ResourceKey] = 'Admin.SettingsCatalog.CatalogCommon.ManufacturersFilter' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'Whether or not to display a filter by manufacturer. <br/> <br/> More details: <br/> <a href = "https://www.advantshop.net/help/pages/catalog-filtr#5" target = "_ blank "> How to disable the manufacturer filter </a>' WHERE [ResourceKey] = 'Admin.SettingsCatalog.CatalogCommon.ManufacturersFilter' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Выводить или нет фильтр по параметру "Размер".<br/><br/> Подробнее:<br/>  <a href="https://www.advantshop.net/help/pages/catalog-filtr#5" target="_blank">Как отключить фильтр по размеру</a>' WHERE [ResourceKey] = 'Admin.SettingsCatalog.CatalogCommon.FilterBySize' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'Whether or not to display a filter by the "Size" parameter. <br/> <br/> More details: <br/> <a href = "https://www.advantshop.net/help/pages/catalog-filtr#5" target = "_ blank"> How to disable filter by size </a>' WHERE [ResourceKey] = 'Admin.SettingsCatalog.CatalogCommon.FilterBySize' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Отображение цвета в фильтре зависит от настройки "Режим отображения цвета". Может быть иконкой, текстом или иконка и название цвета.<br/><br/> Подробнее:<br/>  <a href="https://www.advantshop.net/help/pages/catalog-filtr#5" target="_blank">Как отключить фильтр по цвету</a>' WHERE [ResourceKey] = 'Admin.SettingsCatalog.CatalogCommon.ColorsDisplayedLikeCubes' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'The color display in the filter depends on the Color Display Mode setting. Can be an icon, text or an icon and a color name. <br/> <br/> More details: <br/> <a href = "https://www.advantshop.net/help/pages/catalog-filtr#5" target = "_blank"> How to disable filter by color </a>' WHERE [ResourceKey] = 'Admin.SettingsCatalog.CatalogCommon.ColorsDisplayedLikeCubes' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Однако для работы данной опции необходимо больше ресурсов хостинга.<br/><br/> Подробнее:<br/>  <a href="https://www.advantshop.net/help/pages/catalog-filtr#6" target="_blank">Исключающие фильтры</a>' WHERE [ResourceKey] = 'Admin.SettingsCatalog.CatalogCommon.OptionRequiresMoreResources' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'However, this option requires more hosting resources. <br/> <br/> More details: <br/> <a href = "https://www.advantshop.net/help/pages/catalog-filtr#6" target = "_ blank"> Exclusion Filters </a>' WHERE [ResourceKey] = 'Admin.SettingsCatalog.CatalogCommon.OptionRequiresMoreResources' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Например: Купить.<br/><br/> Подробнее:<br/>  <a href="https://www.advantshop.net/help/pages/change-buttons" target="_blank">Как изменить надписи кнопок на сайте</a>' WHERE [ResourceKey] = 'Admin.SettingsCatalog.CatalogCommon.ExampleBuy' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'For example: Buy. <br/> <br/> More details: <br/> <a href="https://www.advantshop.net/help/pages/change-buttons" target="_blank"> How to change the captions buttons on the site </a>' WHERE [ResourceKey] = 'Admin.SettingsCatalog.CatalogCommon.ExampleBuy' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Например: Заказать.<br/><br/> Подробнее:<br/>  <a href="https://www.advantshop.net/help/pages/change-buttons" target="_blank">Как изменить надписи кнопок на сайте</a>' WHERE [ResourceKey] = 'Admin.SettingsCatalog.CatalogCommon.ExampleOrder' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'For example: Order.<br/> <br/> More details: <br/> <a href="https://www.advantshop.net/help/pages/change-buttons" target="_blank"> How to change the captions buttons on the site </a>' WHERE [ResourceKey] = 'Admin.SettingsCatalog.CatalogCommon.ExampleOrder' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Максимальные размеры изображений производителей (в пикселях).<br/><br/> Подробнее:<br/><a href="https://www.advantshop.net/help/pages/template-settings#7" target="_blank">Лого производителя</a>' WHERE [ResourceKey] = 'Admin.Settings.Template.BrandLogoHote' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'Maximum sizes of manufacturer images (in pixels). <br/><br/> More details: <br/><a href = "https://www.advantshop.net/help/pages/template-settings#7" target = "_ blank"> Manufacturer logo </a>' WHERE [ResourceKey] = 'Admin.Settings.Template.BrandLogoHote' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Максимальные размеры изображений новостей (в пикселях).<br/><br/>Подробнее:<br/> <a href="https://www.advantshop.net/help/pages/add-news#1" target="_blank">Настройка раздела новости на сайте.</a>' WHERE [ResourceKey] = 'Admin.Settings.Template.NewsImageHote' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'Maximum sizes of news images (in pixels).<br/><br/>More details: <br/> <a href="https://www.advantshop.net/help/pages/add-news#1" target="_blank">Setting up the news section on the site.</a>' WHERE [ResourceKey] = 'Admin.Settings.Template.NewsImageHote' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'В данной настройке указываете число - количество новостей, которые будут выводится в списке всех новостей.<br/><br/>Подробнее:<br/> <a href="https://www.advantshop.net/help/pages/add-news#1" target="_blank">Настройка раздела новости на сайте.</a>' WHERE [ResourceKey] = 'Admin.Settings.News.NewsPerPageNote' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'In this setting, specify the number - the number of news that will be displayed in the list of all news.<br/><br/>More details: <br/><a href="https://www.advantshop.net/help/pages/add-news#1" target="_blank">Setting up the news section on the site.</a>' WHERE [ResourceKey] = 'Admin.Settings.News.NewsPerPageNote' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Впишите число - количество новостей отображаемых на главной странице.<br/><br/> Подробнее:<br/><a href="https://www.advantshop.net/help/pages/add-news#1" target="_blank">Настройка раздела новости на сайте.</a>' WHERE [ResourceKey] = 'Admin.Settings.News.NewsMainPageCountNote' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'Впишите число - количество новостей отображаемых на главной странице.<br/><br/> Подробнее:<br/><a href="https://www.advantshop.net/help/pages/add-news#1" target="_blank">Настройка раздела новости на сайте.</a>' WHERE [ResourceKey] = 'Admin.Settings.News.NewsMainPageCountNote' AND [LanguageId] = 2

GO-- 

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Каталог товаров' WHERE [ResourceKey] = 'Catalog.MenuCatalog.AllProductsTitle' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'Сatalog' WHERE [ResourceKey] = 'Catalog.MenuCatalog.AllProductsTitle' AND [LanguageId] = 2
GO-- 

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Впишите число - количество новостей, отображаемых на главной странице.<br/><br/>Подробнее: <br/> <a href="https://www.advantshop.net/help/pages/add-news#1" target="_blank" >Настройка раздела "Новости" на сайте</a>' WHERE [ResourceKey] = 'Admin.Settings.News.NewsMainPageCountNote' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'Enter the number - the number of news items displayed on the main page.<br/><br/> More details:<br/> <a href="https://www.advantshop.net/help/pages/add-news#1" target="_blank" >Setting up the "News" section on the site</a>' WHERE [ResourceKey] = 'Admin.Settings.News.NewsMainPageCountNote' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'В данной настройке указываете число - количество новостей, которые будут выводится в списке всех новостей.<br/><br/>Подробнее:<br/> <a href="https://www.advantshop.net/help/pages/add-news#1" target="_blank">Настройка раздела "Новости" на сайте</a>' WHERE [ResourceKey] = 'Admin.Settings.News.NewsPerPageNote' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'In this setting, specify the number - the number of news that will be displayed in the list of all news.<br/><br/>More details: <br/><a href="https://www.advantshop.net/help/pages/add-news#1" target="_blank">Setting up the "News" section on the site</a>' WHERE [ResourceKey] = 'Admin.Settings.News.NewsPerPageNote' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Указываете текст, который будет отображаться в качестве заголовка в блоке подписки на новости.<br/><br/>Подробнее:<br/> <a href="https://www.advantshop.net/help/pages/add-news#3" target="_blank">Настройка раздела "Новости" на сайте</a>' WHERE [ResourceKey] = 'Admin.Settings.News.MainPageTextNote' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'Specify the text that will be displayed as the title in the newsletter subscription block<br/><br/>More details: <br/><a href="https://www.advantshop.net/help/pages/add-news#3" target="_blank">Setting up the "News" section on the site</a>' WHERE [ResourceKey] = 'Admin.Settings.News.MainPageTextNote' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Максимальные размеры изображений новостей (в пикселях).<br/><br/>Подробнее:<br/> <a href="https://www.advantshop.net/help/pages/add-news#1" target="_blank">Настройка раздела "Новости" на сайте</a>' WHERE [ResourceKey] = 'Admin.Settings.Template.NewsImageHote' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'Maximum sizes of news images (in pixels).<br/><br/>More details: <br/> <a href="https://www.advantshop.net/help/pages/add-news#1" target="_blank">Setting up the "News" section on the site</a>' WHERE [ResourceKey] = 'Admin.Settings.Template.NewsImageHote' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'В поле пишите то количество, которое требуется отобразить на одной странице.<br/><br/>Подробнее:<br/> <a href="https://www.advantshop.net/help/pages/brand#4" target="_blank">Как настроить отображение товаров на странице "Бренды"</a>' WHERE [ResourceKey] = 'Admin.Settings.Template.BrandsPerPageHote' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'In the field, write the amount that you want to display on one page.<br/><br/>More details: <br/><a href="https://www.advantshop.net/help/pages/brand#4" target="_blank">How to customize the display of products on the Brands page</a>' WHERE [ResourceKey] = 'Admin.Settings.Template.BrandsPerPageHote' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Разрешить голосование за отзывы только зарегистрированным пользователям' WHERE [ResourceKey] = 'Admin.Settings.Catalog.ReviewsVoiteOnlyRegisteredUsers' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'Allow only registered users to vote for reviews' WHERE [ResourceKey] = 'Admin.Settings.Catalog.ReviewsVoiteOnlyRegisteredUsers' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Функция пережатия позволяет изменять размеры изображений для ранее загруженных на сайт фотографий.<br/><br/> Подробнее:<br/><a href="https://www.advantshop.net/help/pages/photo-product#2" target="_blank">Как изменить размер изображений?</a>' WHERE [ResourceKey] = 'Admin.SettingsCatalog.CatalogCommon.ResizeGoodsPicturesNote' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'The clamp function allows you to resize images for previously uploaded photos.<br/><br/> More details: <br/><a href="https://www.advantshop.net/help/pages/photo-product#2" target="_blank">How to resize images?</a>' WHERE [ResourceKey] = 'Admin.SettingsCatalog.CatalogCommon.ResizeGoodsPicturesNote' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Настройка определяет, какой вид отображения использовать для товаров в результатах поиска.<br/><br/> Подробнее:<br/><a href="https://www.advantshop.net/help/pages/catalog-view#1" target="_blank">Виды отображения каталога</a>' WHERE [ResourceKey] = 'Admin.SettingsCatalog.CatalogCommon.KindOfDisplayForResults' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'The setting determines which type of display to use for products in search results.<br/> <br/> More details: <br/> <a href = "https://www.advantshop.net/help/pages/catalog-view#1" target = "_blank"> Catalog views </a>' WHERE [ResourceKey] = 'Admin.SettingsCatalog.CatalogCommon.KindOfDisplayForResults' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Настройка определяет, какой вид отображения использовать для товаров в категориях.<br/><br/> Подробнее:<br/><a href="https://www.advantshop.net/help/pages/catalog-view#1" target="_blank">Виды отображения каталога</a>' WHERE [ResourceKey] = 'Admin.SettingsCatalog.CatalogCommon.KindOfDisplayForProducts' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'The setting determines what type of display to use for products in categories. <br/> <br/> More details: <br/> <a href = "https://www.advantshop.net/help/pages/catalog-view#1" target = "_blank"> Catalog views </a>' WHERE [ResourceKey] = 'Admin.SettingsCatalog.CatalogCommon.KindOfDisplayForProducts' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Вы можете выбрать, как показывать цвета: иконка, текст (название цвета) или иконка и название цвета.<br/><br/> Подробнее:<br/><a href="https://www.advantshop.net/help/pages/spravochniki#7" target="_blank">Настройка варианта представления</a>' WHERE [ResourceKey] = 'Admin.Settings.Catalog.ColorsViewModeHint' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'You can choose how to display colors: icon, text (color name) or icon and color name. <br/> <br/> More details: <br/> <a href = "https://www.advantshop.net/help/ pages / spravochniki # 7" target =" _blank "> Configuring the Presentation Option </a>' WHERE [ResourceKey] = 'Admin.Settings.Catalog.ColorsViewModeHint' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Настройка позволяет вывести фильтр со свойствами в каталог. Другими словами, настройка включает работу фильтра в магазине.<br/><br/> Подробнее:<br/>  <a href="https://www.advantshop.net/help/pages/catalog-filtr" target="_blank">Фильтр</a>' WHERE [ResourceKey] = 'Admin.SettingsCatalog.CatalogCommon.ShowFilterNote' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'Setting allows you to display a filter with properties in a directory. In other words, the setting includes the filter in the store.<br/><br/> More details: <br/><a href="https://www.advantshop.net/help/pages/catalog-filtr" target="_blank">Filter</a>' WHERE [ResourceKey] = 'Admin.SettingsCatalog.CatalogCommon.ShowFilterNote' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Показывать превью фотографий' WHERE [ResourceKey] = 'Admin.Settings.Catalog.EnablePhotoPreviews' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'Show photo preview' WHERE [ResourceKey] = 'Admin.Settings.Catalog.EnablePhotoPreviews' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'В категории при наведении мыши на товар рядом с основной картинкой товара будут показаны другие фотографии товара.<br/><br/> Подробнее:<br/><a href="https://www.advantshop.net/help/pages/catalog-view#3" target="_blank">Настройка вывода превью фотографий в каталоге</a>' WHERE [ResourceKey] = 'Admin.SettingsCatalog.CatalogCommon.HoverMouseOnProduct' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'In a category, when you hover the mouse over a product, next to the main product picture, other product photos will be shown. <br/> <br/> More details: <br/> <a href = "https://www.advantshop.net/help/pages/catalog-view#3 "target =" _ blank "> Setting up the output of preview photos in the catalog </a>' WHERE [ResourceKey] = 'Admin.SettingsCatalog.CatalogCommon.HoverMouseOnProduct' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Опция определяет, отображать или нет в каталоге у товара небольшую иконку с количеством фотографий у товара.<br/><br/> Подробнее:<br/><a href="https://www.advantshop.net/help/pages/catalog-view#13" target="_blank">Настройка вывода количества фотографий в каталоге</a>' WHERE [ResourceKey] = 'Admin.SettingsCatalog.CatalogCommon.OptionDisplaySmallIcon' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'The option determines whether or not to display a small icon with the number of photos of the product in the product catalog. <br/> <br/> More details: <br/> <a href = "https://www.advantshop.net/help/pages/catalog-view#13 "target =" _ blank ">Setting the display of the number of photos in the catalog</a>' WHERE [ResourceKey] = 'Admin.SettingsCatalog.CatalogCommon.OptionDisplaySmallIcon' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Данная настройка позволяет просмотреть кратко карточку товара, не переходя вовнутрь.<br/><br/> Подробнее: <br/> <a href="https://www.advantshop.net/help/pages/catalog-view#11" target="_blank">Быстрый просмотр товара</a>' WHERE [ResourceKey] = 'Admin.SettingsCatalog.CatalogCommon.ShowQuickViewNote' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'This setting allows you to view a short product card without going inside. <br/> <br/> More details: <br/> <a href = "https://www.advantshop.net/help/pages/catalog-view#11 "target =" _ blank "> Quick view of the product </a>' WHERE [ResourceKey] = 'Admin.SettingsCatalog.CatalogCommon.ShowQuickViewNote' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Отвечает за количество категорий, которое будет отображаться на главной странице в каждом блоке. <br><br>Подробнее:<br><a href="https://www.advantshop.net/help/pages/template-settings#3-1" target="_blank">Категории на главной</a>' WHERE [ResourceKey] = 'Admin.Settings.Template.CountMainPageCategoriesInSectionTitleLink' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'Responsible for the number of categories that will be displayed on the main page in each block. <br> <br>More details: <br> <a href="https://www.advantshop.net/help/pages/template-settings#3-1" target="_blank"> Main categories </ a >' WHERE [ResourceKey] = 'Admin.Settings.Template.CountMainPageCategoriesInSectionTitleLink' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Отвечает за количество категорий, которое будет отображаться в одной строке каждого блока на главной странице.<br><br>Подробнее:<br><a href="https://www.advantshop.net/help/pages/template-settings#3-1" target="_blank">Категории на главной</a>' WHERE [ResourceKey] = 'Admin.Settings.Template.CountMainPageCategoriesInLineTitleLink' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'Responsible for the number of categories that will be displayed in one line of each block on the main page. <br> <br>More details: <br> <a href="https://www.advantshop.net/help/pages/template-settings#3-1" target="_blank"> Main categories </ a >' WHERE [ResourceKey] = 'Admin.Settings.Template.CountMainPageCategoriesInLineTitleLink' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'В данном поле укажите, какое количество категорий необходимо выводить в каталоге в одной строке.<br/><br/> Подробнее:<br/><a href="https://www.advantshop.net/help/pages/template-settings#4" target="_blank">Количество категорий в строке</a>' WHERE [ResourceKey] = 'Admin.SettingsCatalog.CatalogCommon.CountCategoriesInLineNote' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'In this field specify, how many categories should be displayed in the catalog on one line.More details: <br/><a href="https://www.advantshop.net/help/pages/template-settings#4" target="_blank">Number of categories per line</a>' WHERE [ResourceKey] = 'Admin.SettingsCatalog.CatalogCommon.CountCategoriesInLineNote' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Настройка позволяет на странице бренда отображать категории, в которых имеются товары данного бренда.<br/><br/>Подробнее:<br/> <a href="https://www.advantshop.net/help/pages/brand#4" target="_blank">Как настроить отображение товаров на странице "Бренды"</a>' WHERE [ResourceKey] = 'Admin.Settings.Template.ShowCategoryTreeInBrandHote' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'The setting allows you to display on the brand page the categories in which the goods of this brand.<br/><br/>More details: <br/><a href="https://www.advantshop.net/help/pages/brand#4" target="_blank">How to customize the display of products on the Brands page</a>' WHERE [ResourceKey] = 'Admin.Settings.Template.ShowCategoryTreeInBrandHote' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Настройка позволяет выводить на странице бренда список товаров данного бренда. <br/><br/>Подробнее:<br/>  <a href="https://www.advantshop.net/help/pages/brand#4" target="_blank">Как настроить отображение товаров на странице "Бренды"</a>' WHERE [ResourceKey] = 'Admin.Settings.Template.ShowProductsInBrandHote' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'The setting allows you to display on the brand page a list of products of this brand.<br/><br/>More details: <br/><a href="https://www.advantshop.net/help/pages/brand#4" target="_blank">How to customize the display of products on the Brands page</a>' WHERE [ResourceKey] = 'Admin.Settings.Template.ShowProductsInBrandHote' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Данная настройка позволяет выставить необходимое количество товаров, отображающихся в одной строке в категории.<br/><br/> Подробнее:<br/> <a href="https://www.advantshop.net/help/pages/template-settings#5" target="_blank">Товары в каталоге</a>' WHERE [ResourceKey] = 'Admin.Settings.Template.CountCatalogProductInLineNote' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'This setting allows you to set the required number of products displayed in one line in a category.<br/><br/> More details: <br/><a href = "https://www.advantshop.net/help/pages/template-settings#5" target = "_blank">Products in the catalog</a>' WHERE [ResourceKey] = 'Admin.Settings.Template.CountCatalogProductInLineNote' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'В данном поле пишите рейтинг, который можно установить массово для всех товаров. Значения указываются от 1 до 5.<br/><br/> Подробнее:<br/>  <a href="https://www.advantshop.net/help/pages/catalog-view#10" target="_blank">Как выставить рейтинг товаров вручную</a>' WHERE [ResourceKey] = 'Admin.Settings.Catalog.SetAllProductsManualRatioNote' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'In this field, write a rating that can be set in bulk for all products.Values ??are from 1 to 5.<br/> <br/> More details: <br/> <a href="https://www.advantshop.net/help/pages/catalog-view#10" target="_blank">How to manually rate products</a>' WHERE [ResourceKey] = 'Admin.Settings.Catalog.SetAllProductsManualRatioNote' AND [LanguageId] = 2

GO--

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Список способов' WHERE [ResourceKey] = 'Admin.PaymentMethods.WalletOneCheckout.ListOfMethods' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'List of methods' WHERE [ResourceKey] = 'Admin.PaymentMethods.WalletOneCheckout.ListOfMethods' AND [LanguageId] = 2

GO--

IF EXISTS (SELECT 1 FROM [Settings].[Settings] WHERE [Name] = 'SettingsSocialWidget.IsShowVk')
BEGIN
	INSERT INTO [Settings].[Settings] ([Name], [Value]) VALUES ('SettingsSocialWidget.IsShowVkInMobile', (SELECT [Value] FROM [Settings].[Settings] WHERE [Name] = 'SettingsSocialWidget.IsShowVk'))
	UPDATE [Settings].[Settings] SET [Name] = 'SettingsSocialWidget.IsShowVkInDesktop' WHERE [Name] = 'SettingsSocialWidget.IsShowVk'
END
ELSE IF NOT EXISTS (SELECT 1 FROM [Settings].[Settings] WHERE [Name] = 'SettingsSocialWidget.IsShowVkInDesktop')
BEGIN
	INSERT INTO [Settings].[Settings] ([Name], [Value]) VALUES ('SettingsSocialWidget.IsShowVkInDesktop', 'True')
	INSERT INTO [Settings].[Settings] ([Name], [Value]) VALUES ('SettingsSocialWidget.IsShowVkInMobile', 'True')
END

IF EXISTS (SELECT 1 FROM [Settings].[Settings] WHERE [Name] = 'SettingsSocialWidget.IsShowFb')
BEGIN
	INSERT INTO [Settings].[Settings] ([Name], [Value]) VALUES ('SettingsSocialWidget.IsShowFbInMobile', (SELECT [Value] FROM [Settings].[Settings] WHERE [Name] = 'SettingsSocialWidget.IsShowFb'))
	UPDATE [Settings].[Settings] SET [Name] = 'SettingsSocialWidget.IsShowFbInDesktop' WHERE [Name] = 'SettingsSocialWidget.IsShowFb'
END
ELSE IF NOT EXISTS (SELECT 1 FROM [Settings].[Settings] WHERE [Name] = 'SettingsSocialWidget.IsShowFbInDesktop')
BEGIN
	INSERT INTO [Settings].[Settings] ([Name], [Value]) VALUES ('SettingsSocialWidget.IsShowFbInDesktop', 'True')
	INSERT INTO [Settings].[Settings] ([Name], [Value]) VALUES ('SettingsSocialWidget.IsShowFbInMobile', 'True')
END

IF EXISTS (SELECT 1 FROM [Settings].[Settings] WHERE [Name] = 'SettingsSocialWidget.IsShowJivosite')
BEGIN
	INSERT INTO [Settings].[Settings] ([Name], [Value]) VALUES ('SettingsSocialWidget.IsShowJivositeInMobile', (SELECT [Value] FROM [Settings].[Settings] WHERE [Name] = 'SettingsSocialWidget.IsShowJivosite'))
	UPDATE [Settings].[Settings] SET [Name] = 'SettingsSocialWidget.IsShowJivositeInDesktop' WHERE [Name] = 'SettingsSocialWidget.IsShowJivosite'
END
ELSE IF NOT EXISTS (SELECT 1 FROM [Settings].[Settings] WHERE [Name] = 'SettingsSocialWidget.IsShowJivositeInDesktop')
BEGIN
	INSERT INTO [Settings].[Settings] ([Name], [Value]) VALUES ('SettingsSocialWidget.IsShowJivositeInDesktop', 'True')
	INSERT INTO [Settings].[Settings] ([Name], [Value]) VALUES ('SettingsSocialWidget.IsShowJivositeInMobile', 'True')
END

IF EXISTS (SELECT 1 FROM [Settings].[Settings] WHERE [Name] = 'SettingsSocialWidget.IsShowCallback')
BEGIN
	INSERT INTO [Settings].[Settings] ([Name], [Value]) VALUES ('SettingsSocialWidget.IsShowCallbackInMobile', (SELECT [Value] FROM [Settings].[Settings] WHERE [Name] = 'SettingsSocialWidget.IsShowCallback'))
	UPDATE [Settings].[Settings] SET [Name] = 'SettingsSocialWidget.IsShowCallbackInDesktop' WHERE [Name] = 'SettingsSocialWidget.IsShowCallback'
END
ELSE IF NOT EXISTS (SELECT 1 FROM [Settings].[Settings] WHERE [Name] = 'SettingsSocialWidget.IsShowCallbackInDesktop')
BEGIN
	INSERT INTO [Settings].[Settings] ([Name], [Value]) VALUES ('SettingsSocialWidget.IsShowCallbackInDesktop', 'True')
	INSERT INTO [Settings].[Settings] ([Name], [Value]) VALUES ('SettingsSocialWidget.IsShowCallbackInMobile', 'True')
END

IF EXISTS (SELECT 1 FROM [Settings].[Settings] WHERE [Name] = 'SettingsSocialWidget.IsShowWhatsApp')
BEGIN
	UPDATE [Settings].[Settings] SET [Name] = 'SettingsSocialWidget.IsShowWhatsAppInMobile' WHERE [Name] = 'SettingsSocialWidget.IsShowWhatsApp'
	INSERT INTO [Settings].[Settings] ([Name], [Value]) VALUES ('SettingsSocialWidget.IsShowWhatsAppInDesktop', 'False')
END
ELSE IF NOT EXISTS (SELECT 1 FROM [Settings].[Settings] WHERE [Name] = 'SettingsSocialWidget.IsShowWhatsAppInDesktop')
BEGIN
	INSERT INTO [Settings].[Settings] ([Name], [Value]) VALUES ('SettingsSocialWidget.IsShowWhatsAppInDesktop', 'False')
	INSERT INTO [Settings].[Settings] ([Name], [Value]) VALUES ('SettingsSocialWidget.IsShowWhatsAppInMobile', 'True')
END

IF EXISTS (SELECT 1 FROM [Settings].[Settings] WHERE [Name] = 'SettingsSocialWidget.IsShowViber')
BEGIN
	UPDATE [Settings].[Settings] SET [Name] = 'SettingsSocialWidget.IsShowViberInMobile' WHERE [Name] = 'SettingsSocialWidget.IsShowViber'
	INSERT INTO [Settings].[Settings] ([Name], [Value]) VALUES ('SettingsSocialWidget.IsShowViberInDesktop', 'False')
END
ELSE IF NOT EXISTS (SELECT 1 FROM [Settings].[Settings] WHERE [Name] = 'SettingsSocialWidget.IsShowViberInDesktop')
BEGIN
	INSERT INTO [Settings].[Settings] ([Name], [Value]) VALUES ('SettingsSocialWidget.IsShowViberInDesktop', 'False')
	INSERT INTO [Settings].[Settings] ([Name], [Value]) VALUES ('SettingsSocialWidget.IsShowViberInMobile', 'True')
END

IF EXISTS (SELECT 1 FROM [Settings].[Settings] WHERE [Name] = 'SettingsSocialWidget.IsShowOdnoklassniki')
BEGIN
	INSERT INTO [Settings].[Settings] ([Name], [Value]) VALUES ('SettingsSocialWidget.IsShowOdnoklassnikiInMobile', (SELECT [Value] FROM [Settings].[Settings] WHERE [Name] = 'SettingsSocialWidget.IsShowOdnoklassniki'))
	UPDATE [Settings].[Settings] SET [Name] = 'SettingsSocialWidget.IsShowOdnoklassnikiInDesktop' WHERE [Name] = 'SettingsSocialWidget.IsShowOdnoklassniki'
END
ELSE IF NOT EXISTS (SELECT 1 FROM [Settings].[Settings] WHERE [Name] = 'SettingsSocialWidget.IsShowOdnoklassnikiInDesktop')
BEGIN
	INSERT INTO [Settings].[Settings] ([Name], [Value]) VALUES ('SettingsSocialWidget.IsShowOdnoklassnikiInDesktop', 'True')
	INSERT INTO [Settings].[Settings] ([Name], [Value]) VALUES ('SettingsSocialWidget.IsShowOdnoklassnikiInMobile', 'True')
END

IF EXISTS (SELECT 1 FROM [Settings].[Settings] WHERE [Name] = 'SettingsSocialWidget.IsShowTelegram')
BEGIN
	INSERT INTO [Settings].[Settings] ([Name], [Value]) VALUES ('SettingsSocialWidget.IsShowTelegramInMobile', (SELECT [Value] FROM [Settings].[Settings] WHERE [Name] = 'SettingsSocialWidget.IsShowTelegram'))
	UPDATE [Settings].[Settings] SET [Name] = 'SettingsSocialWidget.IsShowTelegramInDesktop' WHERE [Name] = 'SettingsSocialWidget.IsShowTelegram'
END
ELSE IF NOT EXISTS (SELECT 1 FROM [Settings].[Settings] WHERE [Name] = 'SettingsSocialWidget.IsShowTelegramInDesktop')
BEGIN
	INSERT INTO [Settings].[Settings] ([Name], [Value]) VALUES ('SettingsSocialWidget.IsShowTelegramInDesktop', 'False')
	INSERT INTO [Settings].[Settings] ([Name], [Value]) VALUES ('SettingsSocialWidget.IsShowTelegramInMobile', 'True')
END

GO--

Delete from Settings.TemplateSettings
where Name like 'Mobile_%' and Name not like '%_Modern'

GO--

Update Settings.TemplateSettings
Set Name = Replace(Name, '_Modern', '')
where Name like 'Mobile_%' and Name like '%_Modern'

GO--

Update Settings.Settings Set [Value] = '' Where Name = 'MobileTemplate'

GO--

If not Exists(Select 1 From sys.columns WHERE Name = N'ItemGroupId' AND Object_ID = Object_ID(N'Vk.VkProduct'))
Begin
	ALTER TABLE Vk.VkProduct ADD
		ItemGroupId bigint NULL,
		OfferId int NULL
End

GO--

IF not EXISTS (
  SELECT * 
  FROM   sys.columns 
  WHERE  object_id = OBJECT_ID(N'Catalog.Offer') 
         AND name = 'BarCode'
)
Begin
	ALTER TABLE Catalog.Offer ADD
		BarCode nvarchar(50) NULL
End

GO--

ALTER PROCEDURE [Catalog].[sp_AddOffer]  
   @ArtNo nvarchar(100),  
   @ProductID int,  
   @Amount float,  
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
   ,BarCode
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
   ,@BarCode
  );  
 SELECT SCOPE_IDENTITY();  
END

GO--

ALTER PROCEDURE [Catalog].[sp_UpdateOffer]  
   @OfferID int,  
   @ProductID int,  
   @ArtNo nvarchar(100),  
   @Amount float,  
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
	 ,BarCode = @BarCode
  WHERE [OfferID] = @OfferID  
END

GO-- 

UPDATE Catalog.Offer
	SET [BarCode] = (Select [BarCode] From [Catalog].[Product] Where [Product].[ProductId] = [Offer].[ProductId])
WHERE [BarCode] is null

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
			,[Offer].BarCode
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

UPDATE [Settings].[Localization] SET [ResourceKey] = 'Core.Catalog.Offer.BarCode' WHERE [ResourceKey] = 'Core.Catalog.Product.BarCode'

GO-- 

IF (NOT EXISTS(SELECT * FROM [Settings].[Localization] WHERE ResourceKey = 'Core.Services.Features.EFeature.OfferBarCode'))
begin
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Services.Features.EFeature.OfferBarCode', 'Штрих код у модификаций')
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Services.Features.EFeature.OfferBarCode', 'BarCode for modifications')
end

GO-- 

IF (NOT EXISTS(SELECT * FROM [Settings].[Localization] WHERE ResourceKey = 'Core.Services.Features.EFeature.OfferBarCode.Description'))
begin
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Services.Features.EFeature.OfferBarCode.Description', 'Позволяет задавать штрих код для каждой модификации товара.')

	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Services.Features.EFeature.OfferBarCode.Description', 'Allows you to set the BarCode for each product modification.')
end

GO-- 

IF NOT EXISTS(SELECT * FROM [Settings].[Localization] WHERE [ResourceKey] = 'Admin.Js.Product.BarCode')
BEGIN
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.Product.BarCode', 'Штрих код')
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.Product.BarCode', 'BarCode')
END

GO-- 

IF NOT EXISTS(SELECT * FROM [Settings].[Localization] WHERE [ResourceKey] = 'Admin.Js.AddEditOffer.BarCode')
BEGIN
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.AddEditOffer.BarCode', 'Штрих код')
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.AddEditOffer.BarCode', 'BarCode')
END

GO-- 

IF not EXISTS (
  SELECT * 
  FROM   sys.columns 
  WHERE  object_id = OBJECT_ID(N'[Order].[OrderItems]') 
         AND name = 'BarCode'
)
Begin
	ALTER TABLE [Order].[OrderItems] ADD
		BarCode nvarchar(50) NULL
End

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
	@IsGift bit
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
    ,[DecrementedAmount] = DecrementedAmount  
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
 Where OrderItemID = @OrderItemID  
END 

GO-- 


ALTER PROCEDURE [Order].[sp_AddOrderItem]  
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
	   ,[BarCode]  
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
	   ,@BarCode
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

IF not EXISTS (
  SELECT * 
  FROM   sys.columns 
  WHERE  object_id = OBJECT_ID(N'[Order].[LeadItem]') 
         AND name = 'BarCode'
)
Begin
	ALTER TABLE [Order].[LeadItem] ADD
		BarCode nvarchar(50) NULL
End

GO-- 

IF NOT EXISTS(SELECT * FROM [Settings].[Localization] WHERE [ResourceKey] = 'Core.Orders.OrderItem.BarCode')
BEGIN
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Orders.OrderItem.BarCode', 'Штрих код')
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Orders.OrderItem.BarCode', 'BarCode')
END

GO-- 

IF NOT EXISTS(SELECT * FROM [Settings].[Localization] WHERE [ResourceKey] = 'Admin.Js.Order.BarCode')
BEGIN
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.Order.BarCode', 'Штрих код: ')
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.Order.BarCode', 'BarCode: ')
END

GO-- 

IF NOT EXISTS(SELECT * FROM [Settings].[Localization] WHERE [ResourceKey] = 'Admin.Js.Lead.BarCode')
BEGIN
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.Lead.BarCode', 'Штрих код: ')
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.Lead.BarCode', 'BarCode: ')
END

GO-- 

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'MyAccount.PersonalData', 'Личные данные')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'MyAccount.PersonalData', 'Personal Data')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'MyAccount.DeliveryAddresses', 'Мои адреса доставки')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'MyAccount.DeliveryAddresses', 'Delivery Addresses')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'MyAccount.MyOrders', 'Мои заказы')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'MyAccount.MyOrders', 'My Orders')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Js.Cart.Continue', 'Продолжить покупки')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Js.Cart.Continue', 'Continue shopping')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Js.Cart.ContinueBooking', 'Продолжить просмотр')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Js.Cart.ContinueBooking', 'Continue browsing')

GO-- 

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'User.Registration.ErrorCustomerPhoneExist', 'Пользователь с таким телефоном уже существует')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'User.Registration.ErrorCustomerPhoneExist', 'Customer with this phone already registred')

GO-- 

If Exists (Select 1 FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_NAME ='FK_VkProduct_Product')
Begin
	ALTER TABLE Vk.VkProduct
		DROP CONSTRAINT FK_VkProduct_Product
End

GO-- 

IF NOT EXISTS(SELECT * FROM [Settings].[Localization] WHERE [ResourceKey] = 'Admin.Settings.Catalog.ShowPropertiesFilterInProductList')
BEGIN
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Catalog.ShowPropertiesFilterInProductList', 'Показывать фильтр по свойствам в списках товаров')
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Catalog.ShowPropertiesFilterInProductList', 'Show filter by properties in product lists')
END

GO-- 

IF NOT EXISTS(SELECT * FROM [Settings].[Localization] WHERE [ResourceKey] = 'Admin.SettingsCatalog.CatalogCommon.ShowPropertiesFilterInProductListNote')
BEGIN
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.SettingsCatalog.CatalogCommon.ShowPropertiesFilterInProductListNote', 'Выводить или нет фильтр по свойствам в списках товаров для главной.')
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.SettingsCatalog.CatalogCommon.ShowPropertiesFilterInProductListNote', 'Whether or not to display a filter by properties in the product lists for the main page.')
END

GO-- 

IF NOT EXISTS(SELECT * FROM [Settings].[Settings] WHERE [Name] = 'ShowPropertiesFilterInProductList')
BEGIN
	INSERT INTO [Settings].[Settings] ([Name],[Value]) VALUES ('ShowPropertiesFilterInProductList', 'False')
END

GO--

IF NOT EXISTS(SELECT * FROM [Settings].[Localization] WHERE [ResourceKey] = 'Core.ExportImport.ExportFeedResellerOptions.PriceMarginType.Percent')
BEGIN
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.ExportImport.ExportFeedResellerOptions.PriceMarginType.Percent', '%')
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.ExportImport.ExportFeedResellerOptions.PriceMarginType.Percent', '%')
	
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.ExportImport.ExportFeedResellerOptions.PriceMarginType.AbsoluteValue', 'абсолютное значение')
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.ExportImport.ExportFeedResellerOptions.PriceMarginType.AbsoluteValue', 'absolute value')

	UPDATE [Settings].[Localization] SET [ResourceValue] = 'Рекомендованная наценка на товар' WHERE [ResourceKey] = 'Admin.ExportFeeed.SettingsReseller.RecomendedPriceMargin' AND [LanguageId] = 1
END

GO--

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Язык интерфейса' WHERE [ResourceKey] = 'Admin.Settings.SystemSettings.SiteLanguage' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'Store display language' WHERE [ResourceKey] = 'Admin.Settings.SystemSettings.SiteLanguage' AND [LanguageId] = 2

GO-- 

IF NOT EXISTS(SELECT * FROM [Settings].[Localization] WHERE [ResourceKey] = 'Admin.ExportFeeds.ExportFeedProgress.DataExportedWithErrors')
BEGIN
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ExportFeeds.ExportFeedProgress.DataExportedWithErrors', 'Данные экспортированы с ошибками либо экспорт вовсе не выполнен.')
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ExportFeeds.ExportFeedProgress.DataExportedWithErrors', 'The data was exported with errors or the export was not performed at all.')
END

GO--

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Главная страница' WHERE [ResourceKey] = 'Admin.Settings.Template.MainPage' AND [LanguageId] = 1

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.System.PageStructure', 'Структура страницы')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.System.PageStructure', 'Structure')

Go--

ALTER PROCEDURE [Customers].[sp_GetRecentlyView]
	@CustomerId uniqueidentifier,
	@rowsCount int,
	@Type nvarchar(50)
AS
BEGIN
	SET NOCOUNT ON;
	
	IF Exists (SELECT 1 FROM [Customers].[RecentlyViewsData] WHERE CustomerID=@CustomerId)
	Begin
		SELECT TOP(@rowsCount) Product.ProductID, Product.Name, Product.UrlPath, Ratio, PhotoName,
				[Photo].[Description] as PhotoDescription, Discount, DiscountAmount, MinPrice as BasePrice, CurrencyValue, 
				Offer.OfferID, MaxAvailable AS Amount, MinAmount, MaxAmount, Offer.Amount AS AmountOffer
		
		FROM [Customers].RecentlyViewsData
		
		Inner Join [Catalog].Product ON Product.ProductID = RecentlyViewsData.ProductId
		Left Join [Catalog].[ProductExt] ON [Product].[ProductID] = [ProductExt].[ProductID]
		Inner Join Catalog.Currency On Currency.CurrencyID = Product.CurrencyID
		Left Join Catalog.Photo ON Photo.[ObjId] = Product.ProductID and [Type]=@Type AND [Photo].[Main]=1
		Left Join [Catalog].[Offer] On [ProductExt].[OfferID] = [Offer].[OfferID] 

		WHERE RecentlyViewsData.CustomerID = @CustomerId AND Product.Enabled = 1 And CategoryEnabled = 1
		
		ORDER BY ViewDate Desc
	End
END


GO--

if not exists (SELECT * 
FROM sys.indexes 
WHERE name='Product_Discounts' AND object_id = OBJECT_ID('[Catalog].[Product]'))
begin
CREATE NONCLUSTERED INDEX Product_Discounts
ON [Catalog].[Product] ([Enabled],[CategoryEnabled],[Hidden])
INCLUDE ([ProductId],[Discount],[SortDiscount],[DiscountAmount])
end

GO--

ALTER PROCEDURE [Catalog].[sp_GetCategoriesIDsByProductId]
	@ProductID int
AS
BEGIN
	SELECT	CategoryID
	FROM Catalog.ProductCategories
	WHERE ProductID = @ProductID
	order by main desc
END

GO--
if not exists (SELECT * 
FROM sys.indexes 
WHERE name='Photo_Type_Main_objID' AND object_id = OBJECT_ID('[Catalog].[Photo]'))
begin
CREATE NONCLUSTERED INDEX Photo_Type_Main_objID
ON [Catalog].[Photo] ([Type],[Main])
INCLUDE ([ObjId])
end

GO--

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Добавить в закладки' WHERE [ResourceKey] = 'Wishlist.WishListBlock.AddToWishList' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'Add to bookmarks' WHERE [ResourceKey] = 'Wishlist.WishListBlock.AddToWishList' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'В закладках' WHERE [ResourceKey] = 'Wishlist.WishListBlock.InWishList' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'In bookmarks' WHERE [ResourceKey] = 'Wishlist.WishListBlock.InWishList' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Закладки' WHERE [ResourceKey] = 'Wishlist.Index.WishListHeader' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'Bookmarks' WHERE [ResourceKey] = 'Wishlist.Index.WishListHeader' AND [LanguageId] = 2

GO--

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Подробнее:<br/><a href="https://www.advantshop.net/help/pages/spravochniki#9" target="_blank">Изменение заголовка размера</a> ' WHERE [ResourceKey] = 'Admin.Settings.Catalog.SizesHeaderLink' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'More details: <br/> <a href="https://www.advantshop.net/help/pages/spravochniki#9" target="_blank"> Changing the size header </a> ' WHERE [ResourceKey] = 'Admin.Settings.Catalog.SizesHeaderLink' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Настройка позволяет отобразить блок "Проверка статуса заказа" на главной странице<br/><br/> Подробнее:<br/> <a href="https://www.advantshop.net/help/pages/statusy-zakazov#2" target="_blank">Статусы заказов</a>' WHERE [ResourceKey] = 'Admin.Settings.Template.CheckOrderVisibilityHelp' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'The setting allows you to display the "Check order status" block on the main page. <br/> <br/> More details: <br/> <a href = "https://www.advantshop.net/help/pages/statusy-zakazov#2 "target =" _blank "> Order statuses </a>' WHERE [ResourceKey] = 'Admin.Settings.Template.CheckOrderVisibilityHelp' AND [LanguageId] = 2

GO--

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Избранное' WHERE [ResourceKey] = 'Wishlist.Index.WishListHeader' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'Favorites' WHERE [ResourceKey] = 'Wishlist.Index.WishListHeader' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Ваш список избранного пуст.' WHERE [ResourceKey] = 'Wishlist.Index.EmptyList' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'Your favorites list is empty.' WHERE [ResourceKey] = 'Wishlist.Index.EmptyList' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Добавить в избранное' WHERE [ResourceKey] = 'Wishlist.WishListBlock.AddToWishList' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'Add to favorites' WHERE [ResourceKey] = 'Wishlist.WishListBlock.AddToWishList' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'В избранном' WHERE [ResourceKey] = 'Wishlist.WishListBlock.InWishList' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'Favorite' WHERE [ResourceKey] = 'Wishlist.WishListBlock.InWishList' AND [LanguageId] = 2

GO--

IF NOT EXISTS(SELECT 1 FROM sys.columns WHERE Name = N'ExportAdult' AND Object_ID = Object_ID(N'[Settings].[ExportFeedSettings]'))
BEGIN
	ALTER TABLE [Settings].[ExportFeedSettings] ADD ExportAdult bit NULL
	
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ExportFeeed.Settings.DoNotExportAdult', 'Не выгружать товары для взрослых')
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ExportFeeed.Settings.DoNotExportAdult', 'Do not export adult products')
END

GO--

ALTER PROCEDURE [Settings].[sp_GetCsvProducts] 
    @exportFeedId INT, 
    @onlyCount BIT, 
    @exportNoInCategory BIT, 
    @exportAllProducts BIT, 
    @exportNotAvailable BIT,
	@exportAdult BIT
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
        ) AND (
			@exportAdult = 1
			OR (
				Product.Adult = 0
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
        ) AND (
			@exportAdult = 1
			OR (
				Product.Adult = 0
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
	,@exportAdult BIT
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
		AND (
			@exportAdult = 1
			OR (
				p.Adult = 0
			)
		)
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
			,[Offer].BarCode
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
		AND (
			@exportAdult = 1
			OR (
				p.Adult = 0
			)
		)
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
		AND (
			@exportAdult = 1
			OR (
				p.Adult = 0
			)
		)
	END
END

GO--

UPDATE [Settings].[Localization] SET [ResourceValue] = 'ВКонтакте' WHERE [ResourceKey] = 'Admin.Customers.ShoppingCart.Vkontakte' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'ВКонтакте' WHERE [ResourceKey] = 'Admin.Customers.ViewSocial.Vkontakte' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = '<span class="bold">Шаг 1.</span> Создайте приложение в ВКонтакте.' WHERE [ResourceKey] = 'Admin.Js.VkAuth.Step1' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = '<span class="bold">Шаг 2.</span> Перейдите в Настройки созданного приложения и скопируйте "ID приложения" в магазин, вставьте в текстовое поле ниже полученный ИД приложение и нажмите на кнопку авторизоваться в ВКонтакте.' WHERE [ResourceKey] = 'Admin.Js.VkAuth.Step2' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'ВКонтакте' WHERE [ResourceKey] = 'Admin.Leads.CustomerSocial.Vkontakte' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'ВКонтакте' WHERE [ResourceKey] = 'Admin.Orders.AddEdit.Vkontakte' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'ВКонтакте' WHERE [ResourceKey] = 'Admin.Settings.Social.LinkVkActive' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'ВКонтакте' WHERE [ResourceKey] = 'Admin.Settings.Social.WidgetVkActive' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'ВКонтакте' WHERE [ResourceKey] = 'Core.Crm.EMessageReplyFieldType.Vk' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'Cообщение из ВКонтакте, Instagram, Telegram' WHERE [ResourceKey] = 'Core.Services.EBizProcessEventType.MessageReply' AND [LanguageId] = 1

GO--

if not exists (Select 1 From [Settings].[MailFormat] Where [MailFormatTypeId] = 21)
begin
INSERT [Settings].[MailFormat] ([FormatName], [FormatText], [SortOrder], [Enable], [AddDate], [ModifyDate], [FormatSubject], [MailFormatTypeId]) VALUES (N'Создание лида', N'<div style=''color: #4c4f56; font-family: Arial, Helvetica, sans-serif; font-size: 14px;''>          <div class=''header'' style=''border-bottom: 1px solid #ededed; display: table; margin-bottom: 25px; padding-bottom: 25px; width: 100%;''>              <div class=''logo'' style=''display: table-cell; text-align: left; vertical-align: middle;''>                  #LOGO#              </div>              <div class=''phone'' style=''display: table-cell; text-align: right; vertical-align: middle;''>                  <div class=''tel'' style=''font-size: 26px; font-weight: bold; line-height: 1; margin-bottom: 5px;''>                                  </div>                  <div class=''inform'' style=''font-size: 12px;''>                    </div>              </div>          </div>            <div class=''data'' style=''display: table; width: 100%;''>              <div class=''data-cell'' style=''display: table-cell; padding: 0; padding-right: 1%; width: 48%;''>                  <div class=''l-row''>                      <div class=''l-name vi cs-light'' style=''color: #acacac; display: inline-block; margin: 5px 0; padding-right: 15px; width: 150px; vertical-align: middle;''>                          Имя:                      </div>                      <div class=''l-value vi'' style=''display: inline-block; margin: 5px 0;''>                          #NAME#                      </div>                  </div>                  <div class=''l-row''>                      <div class=''l-name vi cs-light'' style=''color: #acacac; display: inline-block; margin: 5px 0; padding-right: 15px; width: 150px; vertical-align: middle;''>                          Номер телефона:                      </div>                      <div class=''l-value vi'' style=''display: inline-block; margin: 5px 0;''>                          #PHONE#                      </div>                  </div>                  <div class=''l-row''>                      <div class=''l-name vi cs-light'' style=''color: #acacac; display: inline-block; margin: 5px 0; padding-right: 15px; width: 150px; vertical-align: middle;''>                          Список лидов:                      </div>                      <div class=''l-value vi'' style=''display: inline-block; margin: 5px 0;''>                          #LEADS_LIST#                      </div>                  </div>              </div>          </div>          <div>              <div class="o-big-title" style="font-size: 18px; font-weight: bold; margin-bottom: 20px; margin-top: 40px;">Содержание заказа:</div>              #ORDERTABLE#              <div class="comment" style="margin-top: 15px;"><span class="comment-title" style="font-weight: bold;">Комментарии: </span> <span class="comment-txt" style="padding-left: 5px;"> #COMMENTS# </span></div>          </div>      </div>', 1100, 1, getdate(), getdate(), N'Создан лид № #LEAD_ID#', 21)
end
else
begin
	UPDATE [Settings].[MailFormat]
	SET [FormatText] = Replace([FormatText], '<div class=''l-row''>                      <div class=''l-name vi cs-light'' style=''color: #acacac; display: inline-block; margin: 5px 0; padding-right: 15px; width: 150px; vertical-align: middle;''>                          Номер телефона:                      </div>                      <div class=''l-value vi'' style=''display: inline-block; margin: 5px 0;''>                          #PHONE#                      </div>                  </div>              </div>          </div>', '<div class=''l-row''>                      <div class=''l-name vi cs-light'' style=''color: #acacac; display: inline-block; margin: 5px 0; padding-right: 15px; width: 150px; vertical-align: middle;''>                          Номер телефона:                      </div>                      <div class=''l-value vi'' style=''display: inline-block; margin: 5px 0;''>                          #PHONE#                      </div>                  </div>                  <div class=''l-row''>                      <div class=''l-name vi cs-light'' style=''color: #acacac; display: inline-block; margin: 5px 0; padding-right: 15px; width: 150px; vertical-align: middle;''>                          Список лидов:                      </div>                      <div class=''l-value vi'' style=''display: inline-block; margin: 5px 0;''>                          #LEADS_LIST#                      </div>                  </div>              </div>          </div>')
	WHERE [MailFormatTypeId] = 21
end

GO--

UPDATE [Settings].[MailFormatType] SET [Comment] = 'Письмо администратору при заказе в один клик (#ORDER_ID#, #NUMBER#, #NAME#, #COMMENTS#, #PHONE#, #EMAIL#, #ORDERTABLE#, #STORE_NAME#; #MANAGER_NAME#, #BILLING_LINK#)' WHERE [MailType] = 'OnBuyInOneClick'

GO--

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Shipping].[RussianPostCustomsDeclarationProductData]') AND type in (N'U'))
BEGIN
CREATE TABLE [Shipping].[RussianPostCustomsDeclarationProductData](
	[ProductId] [int] NOT NULL,
	[CountryCode] [int] NULL,
	[TnvedCode] [nvarchar](25) NULL,
 CONSTRAINT [PK_RussianPostCustomsDeclarationProductData] PRIMARY KEY CLUSTERED 
(
	[ProductId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END

GO--

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[Shipping].[FK_RussianPostCustomsDeclarationProductData_Product]') AND parent_object_id = OBJECT_ID(N'[Shipping].[RussianPostCustomsDeclarationProductData]'))
ALTER TABLE [Shipping].[RussianPostCustomsDeclarationProductData]  WITH CHECK ADD  CONSTRAINT [FK_RussianPostCustomsDeclarationProductData_Product] FOREIGN KEY([ProductId])
REFERENCES [Catalog].[Product] ([ProductId])
ON DELETE CASCADE

GO--

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[Shipping].[FK_RussianPostCustomsDeclarationProductData_Product]') AND parent_object_id = OBJECT_ID(N'[Shipping].[RussianPostCustomsDeclarationProductData]'))
ALTER TABLE [Shipping].[RussianPostCustomsDeclarationProductData] CHECK CONSTRAINT [FK_RussianPostCustomsDeclarationProductData_Product]

GO--

ALTER FUNCTION [Settings].[PhotoToString]
(
	@ColorId int,
	@ProductId int
)
RETURNS varchar(Max)
AS
BEGIN
	DECLARE @result varchar(max)
	if @ColorId is null
	begin
		SELECT  @result = coalesce(@result + ',', '') + PhotoName
		FROM    Catalog.Photo
		WHERE [Photo].[ObjId]=@ProductId and Type ='Product' order by Main DESC, PhotoSortOrder ASC
	end
	else
	Begin
		SELECT  @result = coalesce(@result + ',', '') + PhotoName
		FROM    Catalog.Photo
		WHERE   [Photo].[ObjId]=@ProductId and Type ='Product' and (Photo.ColorID = @ColorID OR Photo.ColorID is null) order by Photo.ColorID DESC, Main DESC, PhotoSortOrder ASC
	end
	RETURN @Result
END

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Home.Menu.Main', 'Основое')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Home.Menu.Main', 'Main')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Home.Menu.Products', 'Товары')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Home.Menu.Products', 'Products')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Home.Menu.Reports', 'Отчеты')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Home.Menu.Reports', 'Reports')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Home.Menu.Booking', 'Бронировние')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Home.Menu.Booking', 'Booking')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Home.Menu.Configuration', 'Конфигурация')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Home.Menu.Configuration', 'Configuration')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Home.Menu.SalesChannels', 'Каналы продаж')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Home.Menu.SalesChannels', 'Sales channels')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Home.Menu.AddChannel', 'Добавить канал')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Home.Menu.AddChannel', 'Add Channel')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Home.Menu.VKontakte', 'ВКонтакте')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Home.Menu.VKontakte', 'VK')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Home.Menu.BonusProgram', 'Бонусная программа')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Home.Menu.BonusProgram', 'Bonus programs')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Dashboard.MyWebsites', 'Мои сайты')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Dashboard.MyWebsites', 'My websites')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Dashboard.MyWebsitesLink', 'Создать новый')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Dashboard.MyWebsitesLink', 'Create new')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Dashboard.BtnEdit', 'Редактировать')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Dashboard.BtnEdit', 'Edit')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Dashboard.BtnDelete', 'Удалить')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Dashboard.BtnDelete', 'Delete')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Dashboard.BtnPublished', 'Опубликован')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Dashboard.BtnPublished', 'Published')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Dashboard.BtnNotPublished', 'Не опубликован')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Dashboard.BtnNotPublished', 'Not published')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Dashboard.Domain', 'Домен:')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Dashboard.Domain', 'Domain:')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Dashboard.MainProjectSite', 'Основной сайт проекта')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Dashboard.MainProjectSite', 'Main project site')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Dashboard.GoToTheSite', 'Перейти на сайт')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Dashboard.GoToTheSite', 'Go to the site')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Dashboard.CreateWebsite', 'Создать сайт')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Dashboard.CreateWebsite', 'Create a website')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Dashboard.ChangeDomain', 'Изменить домен')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Dashboard.ChangeDomain', 'Change domain')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Dashboard.DomainNotConnected', 'Домен: не подключен')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Dashboard.DomainNotConnected', 'Domain: not connected')

GO--


UPDATE [Settings].[Localization] SET [ResourceValue] = 'Витрина магазина' WHERE [ResourceKey] = 'Admin.Common.TopMenu.ShopWindow' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'View website' WHERE [ResourceKey] = 'Admin.Common.TopMenu.ShopWindow' AND [LanguageId] = 2

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Files', 'Файлы')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Files', 'Files')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.FilesAbout', 'Загрузить файл в корень сайта')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.FilesAbout', 'Upload the file to the root of the site')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Commonpage.SettingsTemplateAbout', 'Параметры отображения и работы витрины')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Commonpage.SettingsTemplateAbout', 'Showcase display and operation parameters')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Commonpage.CustomersAbout', 'Группы, дополнительные поля, теги')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Commonpage.CustomersAbout', 'Groups, additional fields, tags')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Commonpage.LeadsAbout', 'Списки, этапы сделки')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Commonpage.LeadsAbout', 'Lists, stages of the transaction')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Commonpage.SettingsCheckoutAbout', 'Параметры, поля, налоги')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Commonpage.SettingsCheckoutAbout', 'Parameters, fields, taxes')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Tasks.Common', 'Common')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Commonpage.CommonsettingsAbout', 'О компании, план продаж')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Commonpage.CommonsettingsAbout', 'About company, sales plan')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Commonpage.ProductsAbout', 'Параметры, поиск, валюты')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Commonpage.ProductsAbout', 'Parameters, search, currencies')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Dashboard.TypeSite', 'Тип сайта')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Dashboard.TypeSite', 'Site type')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Common.TopPanel.FindSpecialist', 'Найти специалиста')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Common.TopPanel.FindSpecialist', 'Find specialist')

GO--

If Exists (Select 1 FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_NAME ='FK_VkOrder_Order_Order')
Begin
	ALTER TABLE Vk.VkOrder_Order
		DROP CONSTRAINT FK_VkOrder_Order_Order
End

GO-- 

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Home.Menu.AffiliateProgram', 'Партнерская программа')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Home.Menu.AffiliateProgram', 'Affiliate program')

GO-- 

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Online store' WHERE [ResourceKey] = 'Admin.Settings.Commonpage.SettingsTemplate' AND [LanguageId] = 2

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Commonpage.TemplatesdocxAbout', 'Редактирование шаблонов документов')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Commonpage.TemplatesdocxAbout', 'Editing document templates')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Commonpage.SystemsettingsAbout', 'Капча, Карта сайта, Авторизация, Города, Локализация')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Commonpage.SystemsettingsAbout', 'Captcha, Sitemap, Authorization, Cities, Localization')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Commonpage.SettingsmailAbout', 'Подключение почты, шаблоны писем и ответов, sms')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Commonpage.SettingsmailAbout', 'Connection of mail, templates of letters and replies, sms')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Commonpage.UserssettingsAbout', 'Списки, отделы, роли')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Commonpage.UserssettingsAbout', 'Lists, departments, roles')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Commonpage.BookingAbout', 'Параметры, теги')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Commonpage.BookingAbout', 'Parameters, tags')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Commonpage.PaymentmethodsAbout', 'Настройки методов оплаты ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Commonpage.PaymentmethodsAbout', 'Payment method settings ')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Commonpage.TelephonyAbout', 'Настройка телефонии, трекинг ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Commonpage.TelephonyAbout', 'Telephony setup, tracking ')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Commonpage.ShippingsmethodsAbout', 'Настройка методов доставки ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Commonpage.ShippingsmethodsAbout', 'Setting up shipping methods ')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Commonpage.SettingssocialAbout', 'Связь с соц. сетями, виджеты ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Commonpage.SettingssocialAbout', 'Communication with social networks, widgets ')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Commonpage.CouponsAbout', 'Управление купонами, скидками ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Commonpage.CouponsAbout', 'Manage coupons and discounts ')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Commonpage.SettingsAPIAbout', 'Интеграция внешних систем ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Commonpage.SettingsAPIAbout', 'Integration of external systems ')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Commonpage.TasksAbout', 'Проекты, автозадачи ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Commonpage.TasksAbout', 'Projects, autotasks ')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Commonpage.SeoAbout', 'Robots.txt, Google Analytics, 301 редиректы ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Commonpage.SeoAbout', 'Robots.txt, Google Analytics, 301 redirects ')

UPDATE [Settings].[Localization] SET [ResourceValue] = 'System settings' WHERE [ResourceKey] = 'Admin.Settings.Commonpage.Systemsettings' AND [LanguageId] = 2

GO-- 

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Mail, templates, sms' WHERE [ResourceKey] = 'Admin.Settings.Commonpage.SettingsmailAbout' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Social networks, widgets' WHERE [ResourceKey] = 'Admin.Settings.Commonpage.SettingssocialAbout' AND [LanguageId] = 2

GO-- 

UPDATE [Settings].[Localization] SET  [ResourceValue] = 'При использовании этой настройки в прайс-лист для каждого товара будут выгружаться те настройки стоимости и сроков доставки, которые заданы в карточке товара.<br><br>Если в карточке товара не будут заданы настройки сроков доставки, то для такого товара будут использоваться значения cроков доставки из этой настройки<br><br>Если для конкретного товара не задана стоимость доставки в карточке товара, то индивидуальные настройки не выводятся, в таком случае применяются глобальные настройки выгрузки, если они указаны.' WHERE [ResourceKey] = 'Admin.ExportFeed.SettingsYandex.LocalDeliveryCostHelp' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET  [ResourceValue] = 'When using this setting, the price and delivery time settings that are specified in the product card will be uploaded to the price list for each product.<br><br>If delivery time settings are not specified in the product card, then for such product will be used values of delivery times from this setting <br><br> If delivery cost is not specified for a specific product in the product card, then individual settings are not displayed, in this case the global settings are applied, if they are specified.' WHERE [ResourceKey] = 'Admin.ExportFeed.SettingsYandex.LocalDeliveryCostHelp' AND [LanguageId] = 2

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Index.LogoInstruction', 'Инструкция. Создание логотипа')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Index.LogoInstruction', 'Instruction. Create a logotype')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Index.PhoneInstruction', 'Инструкция. Телефон')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Index.PhoneInstruction', 'Instruction. Phone')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Properties.Index.Instruction', 'Инструкция. Свойства / группы свойств')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Properties.Index.Instruction', 'Instruction. Properties/property groups')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Colors.Index.Instruction', 'Инструкция. Справочник цветов')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Colors.Index.Instruction', 'Instruction. Colors')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Sizes.Index.Instruction', 'Инструкция. Справочник размеров')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Sizes.Index.Instruction', 'Instruction. Sizes')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Brands.Index.Instruction', 'Инструкция. Производители')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Brands.Index.Instruction', 'Instruction. Manufacturers')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Catalog.SearchInstruction', 'Инструкция. Поиск')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Catalog.SearchInstruction', 'Instruction, search')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Catalog.SearchProductCategories', 'Искать по категориям товаров')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Catalog.SearchProductCategories', 'Search by product categories')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Tags.Index.TagInstruction', 'Инструкция. Механизм тегов')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Tags.Index.TagInstruction', 'Instruction. Tag mechanism')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Catalog.DisplayModeOfPricesInstruction', 'Инструкция. Режим отображения цен')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Catalog.DisplayModeOfPricesInstruction', 'Instruction. Price display mode')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Catalog.PriceRegulationDiscount', 'Скидка')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Catalog.PriceRegulationDiscount', 'Discount')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.IndexSettings', 'Настройки')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.IndexSettings', 'Settings')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Checkout.CheckoutSettingsInstruction', 'Инструкция. Параметры заказа')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Checkout.CheckoutSettingsInstruction', 'Instruction. Order parameters')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Checkout.BuyInOneClickSetingsInstruction', 'Инструкция. Покупка в 1 клик')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Checkout.BuyInOneClickSetingsInstruction', 'Instruction. Purchase in 1 click')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Checkout.GiftCertificateSettingsInstruction', 'Инструкция. Подарочные сертификаты')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Checkout.GiftCertificateSettingsInstruction', 'Instruction. Gift certificates')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Checkout.EnableGiftCertificateServiceInfo', 'Опция определяет включить или выключить возможность покупки и использования подарочного сертификата.<br />Для возможности использования подарочного сертифката при оформлении заказа, обязательно включить дополнительно настройку "Отображать поле ввода купона или сертификата" (то что в кавычках выделить жирным)')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Checkout.EnableGiftCertificateServiceInfo', 'The option determines whether to enable or disable the ability to purchase and use a gift certificate. <br /> To be able to use a gift certificate when placing an order, be sure to additionally enable the option "Display coupon or certificate input field" (what should be highlighted in quotes)')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Properties.Index.GroupName', 'Все свойства')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Properties.Index.GroupName', 'All properties')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Checkout.DisplayPromoTextboxInfo', 'Опция определяет включить или выключить возможность использования купона и подарочного сертификата при оформлении заказа <br />Для возможности использования подарочного сертифката при оформлении заказа, обязательно включить дополнительно настройку "Разрешить использование подарочных сертификатов" (то что в кавычках выделить жирным)')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Checkout.DisplayPromoTextboxInfo', 'The option determines whether to enable or disable the ability to use a coupon and a gift certificate when placing an order. <br /> To be able to use a gift certificate when placing an order, be sure to additionally enable the "Allow the use of gift certificates" setting (what should be highlighted in quotes)')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Checkout.GiftsSettingsInstruction', 'Инструкция. Подарок для товара')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Checkout.GiftsSettingsInstruction', 'Instruction. Gift for the product')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Checkout.PringOrderSettingsInstruction', 'Инструкция. Печать заказа')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Checkout.PringOrderSettingsInstruction', 'Instruction. Print order')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Checkout.OrderNumberSettingsInstruction', 'Инструкция. Нумерация заказов')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Checkout.OrderNumberSettingsInstruction', 'Instruction. Order numbering')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.OrderStatuses.Index.TitleInstruction', 'Инструкция. Статусы заказов')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.OrderStatuses.Index.TitleInstruction', 'Instruction. Order statuses')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Checkout.CheckoutCustomerFieldsInstruction', 'Инструкция. Полная форма оформления заказа')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Checkout.CheckoutCustomerFieldsInstruction', 'Instruction. Complete checkout form')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.OrderSources.Index.TitleInstruction', 'Инструкция. Источники заказов')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.OrderSources.Index.TitleInstruction', 'Instruction. Sources of orders')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Checkout.TaxesTitleInstruction', 'Инструкция. Налоги')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Checkout.TaxesTitleInstruction', 'Instruction. Taxes')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Checkout.TaxesTitle', 'Taxes')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Analytics.ExportOrders.TitleInstruction', 'Инструкция. Экспорт заказов')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Analytics.ExportOrders.TitleInstruction', 'Instruction. Export orders')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Checkout.ThankYouPageTitle', 'Настройка страницы благодарности')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Checkout.ThankYouPageTitle', 'Thank you page setup')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Checkout.ThankYouPageTitleInfo', 'Страница благодарности показывается, если посетитель совершил целевое действие (оставил заявку или оформил заказ). Когда мы благодарим посетителя за заказ, самое время продолжить строить отношения с этим покупателем и предложить ему сделать еще какое либо целевое действие.')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Checkout.ThankYouPageTitleInfo', 'The thank you page is shown if the visitor has performed the target action (left a request or placed an order).When we thank a visitor for an order, it`s time to continue building relationships with this customer and invite him to take some other targeted action.')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Checkout.ThankYouPageTitleAbout', 'На этой странице Вы можете настроить, какое действие Вы хотите чтобы сделал посетитель после оформления заказа.')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Checkout.ThankYouPageTitleAbout', 'On this page you can configure what action you want the visitor to take after placing an order.')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Checkout.ThankYouPageExceptions', 'Исключения')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Checkout.ThankYouPageExceptions', 'Exceptions')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Checkout.ThankYouPageExceptionsInfo', 'Иногда, когда клиент тут же хочет оплатить заказ, нужно отключить призыв к следующему действию и не отвлекать его от оплаты. Выберите те методы оплаты, при которых страница благодарности не будет содержать другого призыва, кроме оплаты.')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Checkout.ThankYouPageExceptionsInfo', 'Sometimes, when a client immediately wants to pay for an order, you need to turn off the call to the next action and not distract him from the payment.Choose those payment methods in which the thank you page will not contain any call other than payment.')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.Properties.GroupName', 'Все свойства')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.Properties.GroupName', 'All properties')

GO--

DELETE FROM [Settings].[Localization] WHERE ResourceKey = 'Admin.Home.Menu.Academy'
DELETE FROM [Settings].[Localization] WHERE ResourceKey = 'Admin.Service.Academy'
DELETE FROM [Settings].[Localization] WHERE ResourceKey = 'Admin.Settings.SystemSettings.AppsAcademyActive'
DELETE FROM [Settings].[Localization] WHERE ResourceKey = 'Admin.Js.StaticBlock.Modal.VideoTutorialAboutVisualEditor'
DELETE FROM [Settings].[Localization] WHERE ResourceKey = 'Admin.Catalog.Index.VideoTutorial'
DELETE FROM [Settings].[Localization] WHERE ResourceKey = 'Admin.Orders.List.VideoTutorialOrders'
DELETE FROM [Settings].[Localization] WHERE ResourceKey = 'Admin.Brands.AddEdit.VideoTutorial'
DELETE FROM [Settings].[Localization] WHERE ResourceKey = 'Admin.Category.Index.VideoTutorialVisualEditor'
DELETE FROM [Settings].[Localization] WHERE ResourceKey = 'Admin.Leads.Index.VideoTutorialCRM'
DELETE FROM [Settings].[Localization] WHERE ResourceKey = 'Admin.News.AddEdit.VideoTutorialVisualEditor'
DELETE FROM [Settings].[Localization] WHERE ResourceKey = 'Admin.Product.Edit.VideoTutorialVisualEditor'
DELETE FROM [Settings].[Localization] WHERE ResourceKey = 'Admin.ProductList.Index.VideoTutorialGoods'
DELETE FROM [Settings].[Localization] WHERE ResourceKey = 'Admin.Properties.Index.VideoTutorialProperties'
DELETE FROM [Settings].[Localization] WHERE ResourceKey = 'Admin.SettingsBonus.Index.VideoTutorial'
DELETE FROM [Settings].[Localization] WHERE ResourceKey = 'Admin.Settings.SystemSettings.AppsAcademyActive'
DELETE FROM [Settings].[Localization] WHERE ResourceKey = 'Admin.StaticPages.AddEdit.VideoTutorial'
DELETE FROM [Settings].[Localization] WHERE ResourceKey = 'Admin.Tasks.Index.VideoTutorial'

DELETE FROM [Settings].[SettingsSearch] WHERE Link = 'service/academy'

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.ReferralLink.Login', 'Логин')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.ReferralLink.Login', 'Login')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.ReferralLink.YourLink', 'Ваша реферальная ссылка')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.ReferralLink.YourLink', 'Your referral link')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.ReferralLink.Password', 'Пароль')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.ReferralLink.Password', 'Password')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.ReferralLink.GoToPartnerAccount', 'Перейти в партнерский кабинет')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.ReferralLink.GoToPartnerAccount', 'Go to the partner account')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.ReferralLink.ModalTitle', 'Реферальная программа')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.ReferralLink.ModalTitle', 'Referral program')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.ReferralLink.Welcome', 'Добро пожаловать в реферальную программу AdvantShop!')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.ReferralLink.Welcome', 'Welcome to the AdvantShop referral program!')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.ReferralLink.AuthText', 'Для входа в партнерский кабинет используйте следующие логин и пароль')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.ReferralLink.AuthText', 'To enter the partner account, use the following username and password')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.ReferralLink.LoadingError', 'Не удалось загрузить данные')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.ReferralLink.LoadingError', 'Data loading failed')

GO--

if not exists (Select 1 From [CMS].[StaticBlock] Where [Key] = 'head_mobile')
	Insert Into [CMS].[StaticBlock] ([Key],[InnerName],[Content],[Added],[Modified],[Enabled]) Values ('head_mobile', 'Блок в head в моб. версии', '', getdate(), getdate(), 1)

GO--

UPDATE [Settings].[Localization] SET  [ResourceValue] = 'Запретить редактирование в личном кабинете пользователя' WHERE [ResourceKey] = 'Admin.Js.AddEditCustomerField.DisableCustomerEditing' AND [LanguageId] = 1

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.ReferralLink.ModalReferralLinkCtrl.LinkCopiedToClipboard', 'Ссылка скопирована в буфер обмена')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.ReferralLink.ModalReferralLinkCtrl.LinkCopiedToClipboard', 'Link copied to clipboard')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.ReferralLink.ModalReferralLinkCtrl.FailedToCopyLink', 'Не удалось скопировать ссылку')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.ReferralLink.ModalReferralLinkCtrl.FailedToCopyLink', 'Failed to copy link')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.Search.NothingFound', 'Ничего не найдено')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.Search.NothingFound', 'Nothing found')

GO--

UPDATE [Settings].[Localization] SET  [ResourceValue] = 'Снять выделение с видимых' WHERE [ResourceKey] = 'Admin.Js.GridCustomSelectionAction.RemoveSelectionFromVisible' AND [LanguageId] = 1

GO--

UPDATE [Settings].[Localization] SET  [ResourceKey] = 'Admin.Js.AddRemovePropertyToProducts.AddingProperty' WHERE [ResourceKey] = 'Admin.Js.AddPropertyToProducts.AddingProperty'
UPDATE [Settings].[Localization] SET  [ResourceKey] = 'Admin.Js.AddRemovePropertyToProducts.PropertyName' WHERE [ResourceKey] = 'Admin.Js.AddPropertyToProducts.PropertyName'
UPDATE [Settings].[Localization] SET  [ResourceKey] = 'Admin.Js.AddRemovePropertyToProducts.EnterPropertyName' WHERE [ResourceKey] = 'Admin.Js.AddPropertyToProducts.EnterPropertyName'
UPDATE [Settings].[Localization] SET  [ResourceKey] = 'Admin.Js.AddRemovePropertyToProducts.Properties' WHERE [ResourceKey] = 'Admin.Js.AddPropertyToProducts.Properties'
UPDATE [Settings].[Localization] SET  [ResourceKey] = 'Admin.Js.AddRemovePropertyToProducts.EnterPropertyValue' WHERE [ResourceKey] = 'Admin.Js.AddPropertyToProducts.EnterPropertyValue'
UPDATE [Settings].[Localization] SET  [ResourceKey] = 'Admin.Js.AddRemovePropertyToProducts.Add' WHERE [ResourceKey] = 'Admin.Js.AddPropertyToProducts.Add'
UPDATE [Settings].[Localization] SET  [ResourceKey] = 'Admin.Js.AddRemovePropertyToProducts.Cancel' WHERE [ResourceKey] = 'Admin.Js.AddPropertyToProducts.Cancel'
UPDATE [Settings].[Localization] SET  [ResourceKey] = 'Admin.Js.AddRemovePropertyToProducts.PropertyAddedSuccessfully' WHERE [ResourceKey] = 'Admin.Js.AddPropertyToProducts.PropertyAddedSuccessfully'
UPDATE [Settings].[Localization] SET  [ResourceKey] = 'Admin.Js.AddRemovePropertyToProducts.Error' WHERE [ResourceKey] = 'Admin.Js.AddPropertyToProducts.Error'

UPDATE [Settings].[Localization] SET  [ResourceValue] = 'Свойство успешно добавлено' WHERE [ResourceKey] = 'Admin.Js.AddRemovePropertyToProducts.PropertyAddedSuccessfully' AND [LanguageId] = 1

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.AddRemovePropertyToProducts.Remove', 'Убрать свойство')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.AddRemovePropertyToProducts.Remove', 'Remove property')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.AddRemovePropertyToProducts.RemovingProperty', 'Убрать свойство у товаров')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.AddRemovePropertyToProducts.RemovingProperty', 'Removing property from products')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.AddRemovePropertyToProducts.PropertyRemovedSuccessfully', 'Свойство успешно убрано')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.AddRemovePropertyToProducts.PropertyRemovedSuccessfully', 'Property removed successfully')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.Catalog.RemovePropertyFromProducts', 'Убрать свойство у товаров')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.Catalog.RemovePropertyFromProducts', 'Remove property from products')

GO--

UPDATE [Settings].[SettingsSearch] SET [Link] = 'analytics#?analyticsReportTab=telephony&telephonyTab=callLog' WHERE Link = 'analytics#?analyticsReportTab=telephony&telephonyTab=сallLog'

GO--

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Избранное' WHERE [ResourceKey] = 'Common.ToolBarBottom.Wishlist' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'Favorites' WHERE [ResourceKey] = 'Common.ToolBarBottom.Wishlist' AND [LanguageId] = 2

GO--

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Shipping].[PickPointPostamats]') AND type in (N'U'))
BEGIN
CREATE TABLE [Shipping].[PickPointPostamats](
	[Id] [int] NOT NULL,
	[Number] [nvarchar](10) NOT NULL,
	[OwnerName] [nvarchar](255) NULL,
	[TypeTitle] [nvarchar](100) NULL,
	[Name] [nvarchar](100) NULL,
	[City] [nvarchar](255) NOT NULL,
	[Region] [nvarchar](255) NOT NULL,
	[CountryIso] [nchar](3) NULL,
	[Country] [nvarchar](255) NULL,
	[Address] [nvarchar](255) NULL,
	[AddressDescription] [nvarchar](max) NULL,
	[AmountTo] [float] NULL,
	[WorkTimeSMS] [nvarchar](100) NULL,
	[Latitude] [float] NOT NULL,
	[Longitude] [float] NOT NULL,
	[Cash] [tinyint] NOT NULL,
	[Card] [tinyint] NOT NULL,
	[PayPassAvailable] [bit] NOT NULL,
	[MaxWeight] [float] NULL,
	[DimensionSum] [float] NULL,
	[MaxHeight] [float] NULL,
	[MaxWidth] [float] NULL,
	[MaxLength] [float] NULL,
 CONSTRAINT [PK_PickPointPostamats] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END

GO--

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[Shipping].[PickPointPostamats]') AND name = N'PickPointPostamats_Number')
CREATE UNIQUE NONCLUSTERED INDEX [PickPointPostamats_Number] ON [Shipping].[PickPointPostamats]
(
	[Number] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO--

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Shipping].[PickPointPostamatsIkn]') AND type in (N'U'))
BEGIN
CREATE TABLE [Shipping].[PickPointPostamatsIkn](
	[PostamatId] [int] NOT NULL,
	[Ikn] [nvarchar](15) NOT NULL,
	[LastUpdate] [datetime] NOT NULL,
 CONSTRAINT [PK_PickPointPostamatsIkn] PRIMARY KEY CLUSTERED 
(
	[PostamatId] ASC,
	[Ikn] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END

GO--

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[Shipping].[FK_PickPointPostamatsIkn_PickPointPostamats]') AND parent_object_id = OBJECT_ID(N'[Shipping].[PickPointPostamatsIkn]'))
ALTER TABLE [Shipping].[PickPointPostamatsIkn]  WITH NOCHECK ADD  CONSTRAINT [FK_PickPointPostamatsIkn_PickPointPostamats] FOREIGN KEY([PostamatId])
REFERENCES [Shipping].[PickPointPostamats] ([Id])
ON DELETE CASCADE

GO--

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[Shipping].[FK_PickPointPostamatsIkn_PickPointPostamats]') AND parent_object_id = OBJECT_ID(N'[Shipping].[PickPointPostamatsIkn]'))
ALTER TABLE [Shipping].[PickPointPostamatsIkn] CHECK CONSTRAINT [FK_PickPointPostamatsIkn_PickPointPostamats]

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Design.Index.SettingTitle', 'Дизайн и представление')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Design.Index.SettingTitle', 'Design')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Template.Display', 'Отображать')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Template.Display', 'Display')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Template.CarouselTitle', 'Карусель')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Template.CarouselTitle', 'Carousel')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Template.MainPageProductsTitle', 'Товары на главной')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Template.MainPageProductsTitle', 'Products on the main')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Template.CountCategoriesTitle', 'Категории в каталоге')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Template.CountCategoriesTitle', 'Categories in the catalog')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Design.Index.ResizeGoodsPicturesTitle', 'Размер изображения')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Design.Index.ResizeGoodsPicturesTitle', 'Image size')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Template.PaymentIconTitle', 'Иконки на странице оформления заказа')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Template.PaymentIconTitle', 'Icons on the checkout page')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Template.BrandTitle', 'Производители')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Template.BrandTitle', 'Manufacturers')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Template.NewsTitle', 'Новости')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Template.NewsTitle', 'News')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Design.CssEditorTitle', 'Редактор CSS')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Design.CssEditorTitle', 'CSS editor')

GO--

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Все проекты' WHERE [ResourceKey] = 'Admin.Tasks.NavMenu.ControlProjects' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'All projects' WHERE [ResourceKey] = 'Admin.Tasks.NavMenu.ControlProjects' AND [LanguageId] = 2

GO--


INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Home.Menu.MobileVersion', 'Моб. версия')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Home.Menu.MobileVersion', 'Мob. version')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Home.Menu.Menus', 'Меню')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Home.Menu.Menus', 'Menus')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Home.Menu.MobileApp', 'Моб. приложение')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Home.Menu.MobileApp', 'Mob. attachment')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Home.Menu.SettingsTemp', 'Параметры')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Home.Menu.SettingsTemp', 'Options')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Payment.YandexKassa.Loading', 'Загрузка...')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Payment.YandexKassa.Loading', 'Loading...')

GO--

IF EXISTS (SELECT 1 FROM [CMS].[Menu] WHERE [MenuItemID] = 19) AND NOT EXISTS (SELECT 1 FROM [CMS].[Menu] WHERE [MenuItemUrlPath] = 'myaccount?tab=wishlist')
BEGIN
	INSERT INTO [CMS].[Menu] ([MenuItemParentID],[MenuItemName],[MenuItemUrlPath],[MenuItemUrlType],[SortOrder],[ShowMode],[Enabled],[Blank],[NoFollow],[MenuType]) VALUES (19,'Избранное','myaccount?tab=wishlist',5,240,1,1,0,0,1)
END

GO--

declare @CrimeaId int = (select top 1 RegionId from Customers.Region where RegionName = 'Республика Крым')
if @CrimeaId is not null
begin
	update Customers.City set 
		District = REPLACE(
			REPLACE(
				SUBSTRING(CityName, CHARINDEX(',', CityName) + 1, LEN(CityName)), 
				' р-н', ''), 
			' ', ''),
		CityName = SUBSTRING(CityName, 0, CHARINDEX(',', CityName))
	where CityName like '%,%р-н' and RegionId = @CrimeaId
end

GO--

ALTER TRIGGER [Customers].[CustomerDeleted] ON [Customers].[Customer]
WITH EXECUTE AS CALLER
FOR DELETE
AS
BEGIN
	SET NOCOUNT ON;
	UPDATE [Customers].[Customer] SET [HeadCustomerId] = null WHERE [HeadCustomerId] in (SELECT [CustomerID] FROM Deleted)
	Update [Order].[Lead] Set [Lead].[CustomerId] = null Where [Lead].[CustomerId] in (SELECT [CustomerID] FROM Deleted)
END

GO--

IF NOT EXISTS (SELECT 1 FROM [Settings].[Localization] WHERE [ResourceKey] = 'Admin.Import.ImportProducts.UpdatePhotos')
BEGIN

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Import.ImportProducts.UpdatePhotos', 'Обновлять фотографии')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Import.ImportProducts.UpdatePhotos', 'Update photos')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Import.ImportProducts.UpdatePhotosHint', 'Обновлять уже существующие фотографии')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Import.ImportProducts.UpdatePhotosHint', 'Update already existing photos')

END

GO--

if not exists (Select 1 From [Settings].[Localization] Where [ResourceKey] = 'Admin.ExportFeed.SettingsYandex.ExportShopSku')
begin
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ExportFeed.SettingsYandex.ExportShopSku', 'Выгружать тег shop-sku')
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ExportFeed.SettingsYandex.ExportShopSku', 'Export shop-sku tag')
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ExportFeed.SettingsYandex.ExportShopSkuHelp', 'Выбранный идентификатор товарного предложения будет выгружаться тег shop-sku')
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ExportFeed.SettingsYandex.ExportShopSkuHelp', 'The selected product offer ID will be exported in shop-sku tag')
end

if not exists (Select 1 From [Settings].[Localization] Where [ResourceKey] = 'Admin.ExportFeed.SettingsYandex.ExportManufacturer')
begin
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ExportFeed.SettingsYandex.ExportManufacturer', 'Выгружать тег manufacturer')
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ExportFeed.SettingsYandex.ExportManufacturer', 'Export manufacturer tag')

	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ExportFeed.SettingsYandex.ExportManufacturerHelp', 'Производитель будет выгружаться тег manufacturer')
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ExportFeed.SettingsYandex.ExportManufacturerHelp', 'Brand name will be exported in manufacturer tag')
end

GO--

CREATE NONCLUSTERED INDEX IX_ClientCode ON Customers.ClientCode
	(
	Code
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO--

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Пользователь с таким email уже существует. Воспользуйтесь <a class="link-text-decoration-underline" href="{0}">восстановлением пароля</a>' WHERE [ResourceKey] = 'User.Registration.ErrorCustomerExist' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'User with same email already exist. <a class="link-text-decoration-underline" href="{0}">Forgot password?</a>' WHERE [ResourceKey] = 'User.Registration.ErrorCustomerExist' AND [LanguageId] = 2

GO--


if not exists (Select 1 From [Settings].[Localization] Where [ResourceKey] = 'Admin.ExportFeeed.SettingsAvito.Currency')
begin
	Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (1, 'Admin.ExportFeeed.SettingsAvito.Currency', 'Выберите валюту, которая будет использоваться для экспорта'); 
	Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (2, 'Admin.ExportFeeed.SettingsAvito.Currency', 'Select the currency that will be used for export'); 

end

GO--

-- clear duplicate OfferIds in carts
DECLARE SHOPPING_CART_CURSOR CURSOR  LOCAL STATIC FORWARD_ONLY 
	FOR SELECT MIN([ShoppingCartItemId]), [ShoppingCartType], [CustomerId], [OfferId], [IsGift]
	FROM [Catalog].[ShoppingCart] 
	GROUP BY [ShoppingCartType], [CustomerId], [OfferId], [IsGift] 
	HAVING COUNT(*) > 1

DECLARE @id INT, @Type INT, @CustomerId uniqueidentifier, @OfferId INT, @IsGift BIT
OPEN SHOPPING_CART_CURSOR
FETCH NEXT FROM SHOPPING_CART_CURSOR INTO @id, @Type, @CustomerId, @OfferId, @IsGift
WHILE @@FETCH_STATUS = 0
BEGIN
	DELETE FROM [Catalog].[ShoppingCart] 
		WHERE [ShoppingCartType] = @Type and [CustomerId] = @CustomerId and [OfferId] = @OfferId and [IsGift] = @IsGift and [ShoppingCartItemId] <> @id

	FETCH NEXT FROM SHOPPING_CART_CURSOR INTO @id, @Type, @CustomerId, @OfferId, @IsGift
END
CLOSE SHOPPING_CART_CURSOR
DEALLOCATE SHOPPING_CART_CURSOR

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.Validation.Error.URL', 'Невалидный URL')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.Validation.Error.URL', 'Invalid URL')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.Validation.Error.Pattern', 'Не соответствует шаблону ввода')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.Validation.Error.Pattern', 'Doesn''t match the input pattern')

GO--

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Бронирование' WHERE [ResourceKey] = 'Admin.Home.Menu.Booking' AND [LanguageId] = 1

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.Validation.Error.ImageFormat', 'Некорректный формат изображения')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.Validation.Error.ImageFormat', 'Incorrect image format')

GO--


Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.SectionDesignTitle', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Core.Configuration.TemplateSettings_SectionDesign'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.SearchBlockLocationTitle', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.Settings.Template.SearchBlockLocationTitle'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.MenuStyle', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Core.Configuration.TemplateSettings_MenuStyle'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.MenuStyleHint', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.Settings.Template.MenuStyleHint'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.ViewTitle', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Core.Configuration.TemplateSettings_SectionView'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.RecentlyViewVisibilityTitle', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.Settings.Template.RecentlyViewVisibilityTitle'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.RecentlyViewVisibilityHint', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.Settings.Template.RecentlyViewVisibilityHint'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.WishListVisibilityTitle', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.Settings.Template.WishListVisibilityTitle'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.WishListVisibilityHint', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.Settings.Template.WishListVisibilityHint'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.IsStoreClosed', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.Settings.SystemSettings.IsStoreClosed'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.IsStoreClosedHint', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.Settings.SystemSettings.IsStoreClosedNote'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.EnableInplace', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.Settings.SystemSettings.EnableInplace'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.EnableInplaceHint', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.Settings.SystemSettings.EnableInplaceNote'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.DisplayToolBarBottom', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.Settings.SystemSettings.DisplayToolBarBottom'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.DisplayToolBarBottomHint', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.Settings.SystemSettings.DisplayToolBarBottomNote'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.DisplayCityInTopPanel', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.Settings.SystemSettings.DisplayCityInTopPanel'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.DisplayCityInTopPanelHint', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.Settings.SystemSettings.DisplayCityInTopPanelNote'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.Other', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.Settings.SystemSettings.Other'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.AdditionalHeadMetaTag', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.Settings.SystemSettings.AdditionalHeadMetaTag'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.AdditionalHeadMetaTagNote', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.Settings.SystemSettings.AdditionalHeadMetaTagNote'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.AdditionalHeadMetaTagNoteLink', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.Settings.SystemSettings.AdditionalHeadMetaTagNoteLink'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.CustomersNotifications', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.Settings.System.CustomersNotifications'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.ShowUserAgreementTextField', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.Settings.Checkout.ShowUserAgreementTextField'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.ShowUserAgreementTextHint', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.Settings.SystemSettings.ShowUserAgreementTextNote'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.AgreementDefaultChecked', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.Settings.Checkout.AgreementDefaultChecked'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.AgreementDefaultCheckedHint', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.Settings.Checkout.AgreementDefaultCheckedHint'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.UserAgreementTextField', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.Settings.Checkout.UserAgreementTextField'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.DisplayCityBubble', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.Settings.SystemSettings.DisplayCityBubble'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.DisplayCityBubbleHint', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.Settings.SystemSettings.DisplayCityBubbleNote'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.ShowCookiesPolicyMessage', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.Settings.SystemSettings.ShowCookiesPolicyMessage'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.ShowCookiesPolicyMessageHint', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.Settings.SystemSettings.ShowCookiesPolicyMessageNote'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.CookiesPolicyMessage', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.Settings.SystemSettings.CookiesPolicyMessage'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.SiteLanguage', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.Settings.SystemSettings.SiteLanguage'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.SiteLanguageHint', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.Settings.SystemSettings.SiteLanguageNote'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.MenuList.DesignSettings', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Core.Configuration.TemplateSettings_DesignSettings'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.MenuList.MainPage', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.Settings.Template.MainPage'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.MenuList.Catalog', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.Settings.Template.Catalog'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.MenuList.Product', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.Settings.Template.Product'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.MenuList.Checkout', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.Settings.Template.Checkout'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.MenuList.Brands', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.Settings.Template.Brands'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.MenuList.News', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.Settings.Template.News'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.MenuList.Other', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.Settings.Template.Other'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.MenuList.HideGeneralMenu', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.Settings.SystemSettings.HideGeneralMenu'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.PageStructure', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.Settings.System.PageStructure'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.MainPageModeTitle', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.Settings.Template.MainPageModeTitle'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.MainPageModeHint', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.Settings.Template.MainPageModeHint'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.SectionCarousel', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Core.Configuration.TemplateSettings_SectionCarousel'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.CarouselVisibilityTitle', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.Settings.Template.CarouselVisibilityTitle'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.CarouselVisibilityHint', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.Settings.Template.CarouselVisibilityHint'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.CarouselAnimationSpeed', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Core.Configuration.TemplateSettings_CarouselAnimationSpeed'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.CarouselAnimationSpeedHint', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.Settings.Template.CarouselAnimationSpeedHint'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.CarouselAnimationDelay', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Core.Configuration.TemplateSettings_CarouselAnimationDelay'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.CarouselAnimationDelayHint', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.Settings.Template.CarouselAnimationDelayHint'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.SectionMainPageProcuts', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Core.Configuration.TemplateSettings_SectionMainPageProcuts'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.MainPageProductsVisibilityTitle', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.Settings.Template.MainPageProductsVisibilityTitle'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.CountMainPageProductInSection', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Core.Configuration.TemplateSettings_CountMainPageProductInSection'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.CountMainPageProductInSectionHelp', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.Settings.Template.CountMainPageProductInSectionHelp'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.CountMainPageProductInLine', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Core.Configuration.TemplateSettings_CountMainPageProductInLine'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.CountMainPageProductInLineHelp', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.Settings.Template.CountMainPageProductInLineHelp'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.SectionMainPageCategories', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Core.Configuration.TemplateSettings_SectionMainPageCategories'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.MainPageCategoriesVisibility', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Core.Configuration.TemplateSettings_MainPageCategoriesVisibility'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.MainPageCategoriesVisibilityTitleLink', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.Settings.Template.MainPageCategoriesVisibilityTitleLink'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.CountMainPageCategoriesInSection', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Core.Configuration.TemplateSettings_CountMainPageCategoriesInSection'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.CountMainPageCategoriesInSectionTitleLink', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.Settings.Template.CountMainPageCategoriesInSectionTitleLink'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.CountMainPageCategoriesInLine', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Core.Configuration.TemplateSettings_CountMainPageCategoriesInLine'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.CountMainPageCategoriesInLineTitleLink', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.Settings.Template.CountMainPageCategoriesInLineTitleLink'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.SectionView', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Core.Configuration.TemplateSettings_SectionView'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.NewsVisibility', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Core.Configuration.TemplateSettings_NewsVisibility'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.NewsVisibilityHelp', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.Settings.Template.NewsVisibilityHelp'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.NewsSubscriptionVisibility', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Core.Configuration.TemplateSettings_NewsSubscriptionVisibility'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.NewsSubscriptionVisibilityHelp', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.Settings.Template.NewsSubscriptionVisibilityHelp'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.CheckOrderVisibility', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Core.Configuration.TemplateSettings_CheckOrderVisibility'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.CheckOrderVisibilityHelp', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.Settings.Template.CheckOrderVisibilityHelp'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.GiftSertificateVisibility', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Core.Configuration.TemplateSettings_GiftSertificateVisibility'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.GiftSertificateVisibilityHelp', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.Settings.Template.GiftSertificateVisibilityHelp'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.BrandCarouselVisibilityTitle', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.Settings.Template.BrandCarouselVisibilityTitle'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.BrandCarouselVisibilityHelp', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.Settings.Template.BrandCarouselVisibilityHelp'





Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.SectionCatalogCategories', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Core.Configuration.TemplateSettings_SectionCatalogCategories'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.CountCategoriesInLine', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Core.Configuration.TemplateSettings_CountCategoriesInLine'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.CountCategoriesInLineNote', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.SettingsCatalog.CatalogCommon.CountCategoriesInLineNote'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.ProductsDisplaying', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.Settings.Catalog.ProductsDisplaying'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.ShowQuickView', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.Settings.Catalog.ShowQuickView'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.ShowQuickViewNote', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.SettingsCatalog.CatalogCommon.ShowQuickViewNote'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.ProductsPerPage', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.Settings.Catalog.ProductsPerPage'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.OptionDetermines', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.SettingsCatalog.CatalogCommon.OptionDetermines'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.CountCatalogProductInLine', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Core.Configuration.TemplateSettings_CountCatalogProductInLine'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.CountCatalogProductInLineNote', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.Settings.Template.CountCatalogProductInLineNote'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.ShowProductsCount', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.Settings.Catalog.ShowProductsCount'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.ShowCategoriesInBottomMenu', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.Settings.Catalog.ShowCategoriesInBottomMenu'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.ShowCategoriesInBottomMenuHint', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.SettingsCatalog.CatalogCommon.ShowOrNot'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.ShowProductArtNo', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.Settings.Catalog.ShowProductArtNo'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.ShowArticleInTile', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.SettingsCatalog.CatalogCommon.ShowArticleInTile'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.EnableProductRating', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.Settings.Catalog.EnableProductRating'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.EnableProductRatingHint', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.SettingsCatalog.CatalogCommon.ShowOrNotStars'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.SetAllProductsManualRatio', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.Settings.Catalog.SetAllProductsManualRatio'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.SetAllProductsManualRatioBtn', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.Settings.Catalog.SetAllProductsManualRatioBtn'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.SetAllProductsManualRatioNote', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.Settings.Catalog.SetAllProductsManualRatioNote'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.EnableCompareProducts', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.Settings.Catalog.EnableCompareProducts'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.EnableCompareProductsHint', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.SettingsCatalog.CatalogCommon.DeterminesDisplayOrNot'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.EnablePhotoPreviews', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.Settings.Catalog.EnablePhotoPreviews'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.EnablePhotoPreviewsHint', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.SettingsCatalog.CatalogCommon.HoverMouseOnProduct'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.ShowCountPhoto', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.Settings.Catalog.ShowCountPhoto'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.OptionDisplaySmallIcon', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.SettingsCatalog.CatalogCommon.OptionDisplaySmallIcon'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.ShowOnlyAvalible', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.Settings.Catalog.ShowOnlyAvalible'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.ShowOnlyAvalibleHint', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.SettingsCatalog.CatalogCommon.OptionToDisplayProductsAreNotAvailable'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.MoveNotAvaliableToEnd', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.Settings.Catalog.MoveNotAvaliableToEnd'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.MoveNotAvaliableToEndHint', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.SettingsCatalog.CatalogCommon.OptionToMoveProductsInTheEndList'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.ShowNotAvaliableLable', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.Settings.Catalog.ShowNotAvaliableLable'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.HeadFilters', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.Settings.Catalog.HeadFilters'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.ShowFilter', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.Settings.Catalog.ShowFilter'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.ShowFilterNote', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.SettingsCatalog.CatalogCommon.ShowFilterNote'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.ShowPriceFilter', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.Settings.Catalog.ShowPriceFilter'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.ShowPriceFilterHint', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.SettingsCatalog.CatalogCommon.FilterByPrice'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.ShowProducerFilter', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.Settings.Catalog.ShowProducerFilter'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.ShowProducerFilterHint', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.SettingsCatalog.CatalogCommon.ManufacturersFilter'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.ShowSizeFilter', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.Settings.Catalog.ShowSizeFilter'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.ShowSizeFilterHint', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.SettingsCatalog.CatalogCommon.FilterBySize'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.ShowColorFilter', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.Settings.Catalog.ShowColorFilter'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.ShowColorFilterHint1', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.SettingsCatalog.CatalogCommon.FilterByColor'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.ShowColorFilterHint2', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.SettingsCatalog.CatalogCommon.ColorsDisplayedLikeCubes'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.ShowPropertiesFilterInProductList', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.Settings.Catalog.ShowPropertiesFilterInProductList'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.ShowPropertiesFilterInProductListNote', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.SettingsCatalog.CatalogCommon.ShowPropertiesFilterInProductListNote'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.ExluderingFilters', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.Settings.Catalog.ExluderingFilters'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.ExluderingFiltersHint1', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.SettingsCatalog.CatalogCommon.MarkedGray'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.ExluderingFiltersHint2', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.SettingsCatalog.CatalogCommon.AllowsToUnderstand'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.ExluderingFiltersHint3', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.SettingsCatalog.CatalogCommon.OptionRequiresMoreResources'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.SizesAndColors', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.Settings.Catalog.SizesAndColors'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.SizesHeader', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.Settings.Catalog.SizesHeader'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.SizesHeaderHint1', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.SettingsCatalog.CatalogCommon.SpecifyNameForSize'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.SizesHeaderHint2', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.SettingsCatalog.CatalogCommon.ExampleVolume'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.SizesHeaderHint3', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.Settings.Catalog.SizesHeaderLink'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.ColorsHeader', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.Settings.Catalog.ColorsHeader'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.ColorsHeaderHint1', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.SettingsCatalog.CatalogCommon.SpecifyNameForColor'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.ColorsHeaderHint2', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.SettingsCatalog.CatalogCommon.ExampleTexture'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.ColorsHeaderHint3', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.Settings.Catalog.ColorsHeaderLink'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.ColorsViewMode', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.Settings.Catalog.ColorsViewMode'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.ColorsViewModeHint', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.Settings.Catalog.ColorsViewModeHint'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.ColorsPictureInCatalog', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.Settings.Template.ColorsPictureInCatalog'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.ColorsPictureHelp', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.Settings.Template.ColorsPictureHelp'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.ColorsPictureHelpLink', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.Settings.Template.ColorsPictureHelpLink'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.Width', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Core.Configuration.TemplateSettings_Width'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.Height', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Core.Configuration.TemplateSettings_Height'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.ColorsPictureInDetails', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.Settings.Template.ColorsPictureInDetails'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.ComplexFilter', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.Settings.Catalog.ComplexFilter'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.PhotoCorrespondingSelectedColor', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.SettingsCatalog.CatalogCommon.PhotoCorrespondingSelectedColor'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.ThisOptionRequiers', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.SettingsCatalog.CatalogCommon.ThisOptionRequiers'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.ComplexFilterLink', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.Settings.Catalog.ComplexFilterLink'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.ButtonText', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.Settings.Catalog.ButtonText'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.DisplayButton', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.Settings.Catalog.DisplayButton'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.DisplayButtonHint1', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.SettingsCatalog.CatalogCommon.ShowAddButton'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.DisplayButtonHint2', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.SettingsCatalog.CatalogCommon.OwnTextForButton'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.DisplayButtonHint3', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.SettingsCatalog.CatalogCommon.ExampleBuy'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.PreOrderButtonText', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.Settings.Catalog.PreOrderButtonText'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.DisplayPreOrderButtonHint1', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.SettingsCatalog.CatalogCommon.ShowButtonForOrder'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.DisplayPreOrderButtonHint2', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.SettingsCatalog.CatalogCommon.OwnTextForButton'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.DisplayPreOrderButtonHint3', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.SettingsCatalog.CatalogCommon.ExampleOrder'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.CatalogView', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.Settings.Catalog.CatalogView'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.DefaultCatalogView', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.Settings.Catalog.DefaultCatalogView'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.EnableCatalogViewChange', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.Settings.Catalog.EnableCatalogViewChange'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.EnableCatalogViewChangeHint', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.SettingsCatalog.CatalogCommon.KindOfDisplayForProducts'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.DefaultSearchView', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.Settings.Catalog.DefaultSearchView'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.KindOfDisplayForResults', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.SettingsCatalog.CatalogCommon.KindOfDisplayForResults'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.SizesImage', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Core.Configuration.TemplateSettings_SizesImage'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.ResizeGoodsPictures', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.Design.Index.ResizeGoodsPictures'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.ResizeGoodsPicturesNote', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.SettingsCatalog.CatalogCommon.ResizeGoodsPicturesNote'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.SectionBigProductImages', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Core.Configuration.TemplateSettings_SectionBigProductImages'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.SectionMiddleProductImages', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Core.Configuration.TemplateSettings_SectionMiddleProductImages'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.SectionSmallProductImages', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Core.Configuration.TemplateSettings_SectionSmallProductImages'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.SectionXSmallProductImages', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Core.Configuration.TemplateSettings_SectionXSmallProductImages'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.SectionBigCategoryImages', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Core.Configuration.TemplateSettings_SectionBigCategoryImages'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.SectionSmallCategoryImages', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Core.Configuration.TemplateSettings_SectionSmallCategoryImages'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.ShowProductsCountHint1', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.SettingsCatalog.CatalogCommon.IfOption'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.ShowProductsCountHint2', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.SettingsCatalog.CatalogCommon.IsEnable'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.ShowProductsCountHint3', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.SettingsCatalog.CatalogCommon.InTheClientPart'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.BuyButtonText', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.Settings.Catalog.BuyButtonText'

GO--

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.Product', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.Settings.Template.Product'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.DisplayWeight', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.Settings.Catalog.DisplayWeight'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.DisplayDimensions', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.Settings.Catalog.DisplayDimensions'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.ShowStockAvailability', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.Settings.Catalog.ShowStockAvailability'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.ShowStockAvailabilityLink', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.Settings.Catalog.ShowStockAvailabilityLink'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.ProductPictures', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.Settings.Catalog.ProductPictures'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.EnableZoom', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.Settings.Catalog.EnableZoom'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.Reviews', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.Settings.Catalog.Reviews'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.AllowReviews', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.Settings.Catalog.AllowReviews'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.ModerateReviews', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.Settings.Catalog.ModerateReviews'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.ModerateReviewsLink', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.Settings.Catalog.ModerateReviewsLink'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.ReviewsVoiteOnlyRegisteredUsers', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.Settings.Catalog.ReviewsVoiteOnlyRegisteredUsers'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.DisplayReviewsImage', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.Settings.Catalog.DisplayReviewsImage'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.ReviewsHelpLink', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.Settings.Catalog.Reviews.HelpLink'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.AllowReviewsImageUploading', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.Settings.Catalog.AllowReviewsImageUploading'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.ReviewImageSize', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.Settings.Catalog.ReviewImageSize'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.Shipping', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.Settings.Catalog.Shipping'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.ShowShippingsMethodsInDetails', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.Settings.Catalog.ShowShippingsMethodsInDetails'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.ShowShippingsMethodsInDetails', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.Settings.ShowShippingsMethodsInDetails'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.ShippingHelpLink', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.Settings.Catalog.Shipping.HelpLink'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.ShippingsMethodsInDetailsCount', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.Settings.Catalog.ShippingsMethodsInDetailsCount'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.CrossUpSale', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.Settings.Catalog.CrossUpSale'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.RelatedProductName', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.Settings.Catalog.RelatedProductName'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.ProductNameHelpLink', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.Settings.Catalog.ProductName.HelpLink'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.AlternativeProductName', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.Settings.Catalog.AlternativeProductName'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.RelatedProductSourceType', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.Settings.Catalog.RelatedProductSourceType'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.ProductChooseWhereLoad', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.Settings.Catalog.Product.ChooseWhereLoad'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.RelatedProductSourceTypeLink', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.Settings.Catalog.RelatedProductSourceTypeLink'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.RelatedProductsMaxCount', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.Settings.Catalog.RelatedProductsMaxCount'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.DisplayWeightHint', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.SettingsCatalog.Product.ShowColumnWeight'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.DisplayDimensionsHint', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.SettingsCatalog.Product.ShowColumnDimensions'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.ShowStockAvailabilityHint1', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.SettingsCatalog.Product.ShowOrNotProductBalance'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.ShowStockAvailabilityHint2', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.SettingsCatalog.Product.ShowOrNotProductBalance.ForExample'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.ShowStockAvailabilityHint3', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.SettingsCatalog.Product.EnabledOption'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.ShowStockAvailabilityHint4', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.SettingsCatalog.Product.DisabledOption'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.EnableZoomHint', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.SettingsCatalog.Product.TurnPhotoCloser'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.AllowReviewsHint', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.SettingsCatalog.Product.OptionAllowToAddReviews'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.ModerateReviewsHint1', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.SettingsCatalog.Product.IfOption'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.ModerateReviewsHint2', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.SettingsCatalog.Product.IsEnabled'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.ModerateReviewsHint3', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.SettingsCatalog.Product.ReviewsFirstGoToModeration'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.DescriptionReviewsVoiteOnlyRegisteredUsers', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.SettingsCatalog.Product.DescriptionReviewsVoiteOnlyRegisteredUsers'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.ShowImagesToReviews', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.SettingsCatalog.Product.ShowImagesToReviews'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.AllowAttachImagesToReviews', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.SettingsCatalog.Product.AllowAttachImagesToReviews'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.MaximumSizeOfFeedbackImages', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.SettingsCatalog.Product.MaximumSizeOfFeedbackImages'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.ShowShippingsMethodsInDetailsHint1', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.SettingsCatalog.Product.TypeOfDelivery'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.ShowShippingsMethodsInDetailsHint2', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.SettingsCatalog.Product.OptionToTurnOffDisplayInList'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.ShippingsMethodsInDetailsCountHint1', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.SettingsCatalog.Product.AllowedDeliveryMethods'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.ShippingsMethodsInDetailsCountHint2', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.SettingsCatalog.Product.EachDeliveryMethodHasOptionTurnOff'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.Attach2listsToProductHint1', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.SettingsCatalog.Product.Attach2listsToProduct'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.Attach2listsToProductHint2', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.SettingsCatalog.Product.ForExampleWithThisProductBuy'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.ForExampleSimilarGoods', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.SettingsCatalog.Product.ForExampleSimilarGoods'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.DesignatedGoods', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.SettingsCatalog.Product.DesignatedGoods'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.ItMeansToShowProductFromList', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.SettingsCatalog.Product.ItMeansToShowProductFromList'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.MaximumNumberOfProducts', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.SettingsCatalog.Product.MaximumNumberOfProducts'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.IconsInCheckout', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Core.Configuration.TemplateSettings_IconsInCheckout'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.PaymentIcon', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.Settings.Template.PaymentIcon'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.PaymentIconNote', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.Settings.Template.PaymentIconNote'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.SectionShippingIcons', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Core.Configuration.TemplateSettings_SectionShippingIcons'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.ShippingIcon', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.Settings.Template.ShippingIcon'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.ShippingIconNote', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.Settings.Template.ShippingIconNote'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.SectionPaymentIcons', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Core.Configuration.TemplateSettings_SectionPaymentIcons'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.SectionBrandLogo', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Core.Configuration.TemplateSettings_SectionBrandLogo'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.BrandLogo', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.Settings.Template.BrandLogo'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.BrandLogoHote', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.Settings.Template.BrandLogoHote'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.BrandsPerPage', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.Settings.Template.BrandsPerPage'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.BrandsPerPageHote', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.Settings.Template.BrandsPerPageHote'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.ShowProductsInBrand', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.Settings.Template.ShowProductsInBrand'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.ShowProductsInBrandHote', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.Settings.Template.ShowProductsInBrandHote'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.ShowCategoryTreeInBrand', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.Settings.Template.ShowCategoryTreeInBrand'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.ShowCategoryTreeInBrandTitleHote', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.Settings.Template.ShowCategoryTreeInBrand.TitleHote'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.ShowCategoryTreeInBrandHote', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.Settings.Template.ShowCategoryTreeInBrandHote'

Go--

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Страницы и блоки' WHERE [ResourceKey] = 'Admin.Home.Menu.StaticPages' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'Pages and blocks' WHERE [ResourceKey] = 'Admin.Home.Menu.StaticPages' AND [LanguageId] = 2

GO--

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.SectionNewsPhotos', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Core.Configuration.TemplateSettings_SectionNewsPhotos'


Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.NewsImage', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.Settings.Template.NewsImage'

UPDATE [Settings].[InternalSettings] SET [settingValue] = '9.0.0' WHERE [settingKey] = 'db_version'
Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.NewsImageHote', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.Settings.Template.NewsImageHote'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.NewsMainPageText', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.Settings.News.MainPageText'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.NewsMainPageTextNote', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.Settings.News.MainPageTextNote'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.NewsPerPage', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.Settings.News.NewsPerPage'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.NewsPerPageNote', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.Settings.News.NewsPerPageNote'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.NewsMainPageCount', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.Settings.News.NewsMainPageCount'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.NewsMainPageCountNote', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.Settings.News.NewsMainPageCountNote'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.CssEditorAttention', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.Design.CssEditor.Attention'

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
Select [LanguageId], 'Js.Builder.CssEditorUseFile', [ResourceValue] From [Settings].[Localization] Where [ResourceKey] = 'Admin.Design.CssEditor.UseFile'

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Js.Builder.OtherItemsTitle', 'Настройки шаблона')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Js.Builder.OtherItemsTitle', 'Template settings')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'AdvantShop.Configuration.ETemplateSettingSection.MainPage', 'Главная страница')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'AdvantShop.Configuration.ETemplateSettingSection.MainPage', 'Main page')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'AdvantShop.Configuration.ETemplateSettingSection.Category', 'Категория товаров')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'AdvantShop.Configuration.ETemplateSettingSection.Category', 'Category')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'AdvantShop.Configuration.ETemplateSettingSection.Checkout', 'Оформление заказа')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'AdvantShop.Configuration.ETemplateSettingSection.Checkout', 'Checkout')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'AdvantShop.Configuration.ETemplateSettingSection.Design', 'Настройки дизайна')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'AdvantShop.Configuration.ETemplateSettingSection.Design', 'Design')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'AdvantShop.Configuration.ETemplateSettingSection.Brands', 'Производители')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'AdvantShop.Configuration.ETemplateSettingSection.Brands', 'Brands')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'AdvantShop.Configuration.ETemplateSettingSection.News', 'Новости')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'AdvantShop.Configuration.ETemplateSettingSection.News', 'News')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'AdvantShop.Configuration.ETemplateSettingSection.Product', 'Карточка товара')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'AdvantShop.Configuration.ETemplateSettingSection.Product', 'Product')

GO--

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (1, 'Admin.Settings.SystemSettings.AdminStartPage', 'Начальная страница администрирования'); 
Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (2, 'Admin.Settings.SystemSettings.AdminStartPage', 'Administration start page'); 

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (1, 'Admin.Settings.SystemSettings.AdminStartPageNote', 'Настройка позволяет выбрать начальную страницу панели администрирования'); 
Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (2, 'Admin.Settings.SystemSettings.AdminStartPageNote', 'The setting allows you to select the start page of the administration panel'); 

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (1, 'Admin.Settings.SystemSettings.MaskForPhone', 'Использовать маску ввода для телефона'); 
Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (2, 'Admin.Settings.SystemSettings.MaskForPhone', 'Use input mask for phone'); 

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (1, 'Admin.Settings.SystemSettings.MaskForPhoneNote', 'Настройка позволяет выводить маску для телефона при оформлении заказа'); 
Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (2, 'Admin.Settings.SystemSettings.MaskForPhoneNote', 'The setting allows you to display a mask for the phone when placing an order'); 

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (1, 'Admin.Settings.SystemSettings.ChangeProduct', 'Отслеживать изменения товара'); 
Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (2, 'Admin.Settings.SystemSettings.ChangeProduct', 'Track product changes');
 
Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (1, 'Admin.Settings.SystemSettings.ChangeProductNote', 'Настройка позволяет выводить логирование действий, а именно, когда и кем были внесены изменения в товар'); 
Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (2, 'Admin.Settings.SystemSettings.ChangeProductNote', 'The setting allows you to display logging of actions, namely, when and by whom changes were made to the product'); 

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (1, 'Admin.Settings.SystemSettings.FileSize', 'Размер файлов'); 
Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (2, 'Admin.Settings.SystemSettings.FileSize', 'File size'); 

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (1, 'Admin.Settings.SystemSettings.FileSizeNote', 'Настройка позволяет отображать объем данных, который содержит сайт'); 
Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (2, 'Admin.Settings.SystemSettings.FileSizeNote', 'The setting allows you to display the amount of data that the site contains'); 

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (1, 'Admin.Settings.SystemSettings.LicKeyNote', 'Это уникальный ключ для каждого магазина. Для пробных магазинов и магазинов в облаке он устанавливается автоматически. Приобретая лицензию, покупатель получает право на установку ключа'); 
Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (2, 'Admin.Settings.SystemSettings.LicKeyNote', 'This is a unique key for each store. For trial stores and cloud stores, it is automatically installed. By purchasing a license, the buyer gets the right to install the key'); 

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (1, 'Admin.Settings.SystemSettings.ActiveLicNote', 'Состояние лицензии. Должна быть активированна'); 
Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (2, 'Admin.Settings.SystemSettings.ActiveLicNote', 'License status. Must be activated'); 

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (1, 'Admin.Settings.SystemSettings.EnableExperimentalFeaturesNote', 'Подключение функций на сайте, которые носят экспериментальный характер<br /><br />Подробнее<br /><a target="_blank" href="https://www.advantshop.net/help/pages/product-parametry">Вес, единица измерения, габариты, наличие</a>'); 
Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (2, 'Admin.Settings.SystemSettings.EnableExperimentalFeaturesNote', 'Connecting features on the site that are experimental <br /> <br />More details<br /> <a target = "_ blank" href = "https://www.advantshop.net/help/pages/product-parametry" > Weight, unit of measure, dimensions, availability </a>'); 

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.SettingsCatalog.Brands.BrandsList', 'Список')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.SettingsCatalog.Brands.BrandsList', 'Brands list')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.SettingsCatalog.Brands.BrandsList.Title', 'Список производителей')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.SettingsCatalog.Brands.BrandsList.Title', 'Brands list')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.SettingsCatalog.Brands.Export', 'Экспорт')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.SettingsCatalog.Brands.Export', 'Export')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.SettingsCatalog.Brands.Import', 'Импорт')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.SettingsCatalog.Brands.Import', 'Import')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.SettingsCatalog.ImportBrands.Title', 'Импорт производителей')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.SettingsCatalog.ImportBrands.Title', 'Import brands')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.SettingsCatalog.ImportBrands.Import', 'Импортитровать')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.SettingsCatalog.ImportBrands.Import', 'Import')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.SettingsCatalog.ImportBrands.Params', 'Параметры загрузки')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.SettingsCatalog.ImportBrands.Params', 'Import options')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.SettingsCatalog.ImportBrands.ColumnSeparator', 'Разделитель между колонками')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.SettingsCatalog.ImportBrands.ColumnSeparator', 'Column separator')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.SettingsCatalog.ImportBrands.ColumnSeparatorHint', 'Разделитель, который указан между столбцами или колонками в файле CSV')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.SettingsCatalog.ImportBrands.ColumnSeparatorHint', 'Separator that is specified between columns or columns in a CSV file')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.SettingsCatalog.ImportBrands.FileEncoding', 'Кодировка файла')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.SettingsCatalog.ImportBrands.FileEncoding', 'File encoding')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.SettingsCatalog.ImportBrands.FileEncodingHint', 'Это кодировка, в которой загружается каталог. Обычные кодировки, которые воспринимаются Microsoft Excel, это кодировки UTF-8 и Windows-1251')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.SettingsCatalog.ImportBrands.FileEncodingHint', 'This is the encoding in which the directory is loaded. The usual encodings that Microsoft Excel interprets are UTF-8 and Windows-1251 encodings')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.SettingsCatalog.ImportBrands.HasHeader', 'Первая строка файла содержит заголовки')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.SettingsCatalog.ImportBrands.HasHeader', 'File has a header row')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.SettingsCatalog.ImportBrands.HasHeaderHint', 'Данная опция означает, что в импортируемом файле CSV будет верхняя строка с заголовками')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.SettingsCatalog.ImportBrands.HasHeaderHint', 'This option means that in the imported CSV file there will be a top line with headers')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.SettingsCatalog.ImportBrands.SampleFile', 'Пример файла')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.SettingsCatalog.ImportBrands.SampleFile', 'Sample file')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.SettingsCatalog.ImportBrands.CsvFile', '.Csv файл с производителями')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.SettingsCatalog.ImportBrands.CsvFile', '.Csv file of brands')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.SettingsCatalog.ImportBrands.ColumnInCsv', 'Колонка в .csv файле')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.SettingsCatalog.ImportBrands.ColumnInCsv', 'Column in .csv file')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.SettingsCatalog.ImportBrands.FirstBrandData', 'Данные первого производителя')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.SettingsCatalog.ImportBrands.FirstBrandData', 'First brand data')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.SettingsCatalog.ImportBrands.DataType', 'Тип данных')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.SettingsCatalog.ImportBrands.DataType', 'Data type')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.SettingsCatalog.ImportBrands.ChangeNewFile', 'Выбрать другой файл')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.SettingsCatalog.ImportBrands.ChangeNewFile', 'Select another file')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ImportBrands.Errors.FieldsRequired', 'Должно быть выбрано хотя бы одно из полей: Название')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ImportBrands.Errors.FieldsRequired', 'At least one of the fields must be selected: Name')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ImportBrands.ProcessName', 'Импорт производителей')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ImportBrands.ProcessName', 'Import brands')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.ExportImport.BrandFields.NotSelected', 'Не выбрано')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.ExportImport.BrandFields.NotSelected', 'Not selected')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.ExportImport.BrandFields.Name', 'Название')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.ExportImport.BrandFields.Name', 'Name')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.ExportImport.BrandFields.Description', 'Описание')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.ExportImport.BrandFields.Description', 'Description')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.ExportImport.BrandFields.BriefDescription', 'Краткое опасание')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.ExportImport.BrandFields.BriefDescription', 'Brief description')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.ExportImport.BrandFields.Enabled', 'Активность')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.ExportImport.BrandFields.Enabled', 'Enabled')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.ExportImport.BrandFields.UrlPath', 'Синоним для URL запроса')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.ExportImport.BrandFields.UrlPath', 'URL synonym')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.ExportImport.BrandFields.BrandSiteUrl', 'Сайт производителя')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.ExportImport.BrandFields.BrandSiteUrl', 'Brand''s site')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.ExportImport.BrandFields.CountryId', 'Идентификатор страны')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.ExportImport.BrandFields.CountryId', 'Country identifier')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.ExportImport.BrandFields.CountryName', 'Название страны')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.ExportImport.BrandFields.CountryName', 'Country name')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.ExportImport.BrandFields.CountryOfManufactureId', 'Идентификатор cтраны производства')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.ExportImport.BrandFields.CountryOfManufactureId', 'Country of manufacture identifier')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.ExportImport.BrandFields.CountryOfManufactureName', 'Название страны производства')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.ExportImport.BrandFields.CountryOfManufactureName', 'Country of manufacture name')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'User.LoginOpenId.AuthGoogleText', 'Войти через Google')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'User.LoginOpenId.AuthGoogleText', 'Login with Google')



INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'User.LoginOpenId.AuthFacebookText', 'Войти через Facebook')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'User.LoginOpenId.AuthFacebookText', 'Login with Facebook')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'User.LoginOpenId.AuthVKText', 'Войти через VK')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'User.LoginOpenId.AuthVKText', 'Login with VK')

GO--

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Отвечает за вывод категорий на главной странице. Отобразятся только те категории, для которых включена настройка "Выводить на главной".  <br><br>Подробнее:<br><a href="https://www.advantshop.net/help/pages/template-settings#3-1" target="_blank">Категории на главной</a>' WHERE [ResourceKey] = 'Admin.Settings.Template.MainPageCategoriesVisibilityTitleLink' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'Responsible for displaying categories on the main page. Only those categories for which the "Show on home" setting is enabled will be displayed. <br> <br>More details: <br> <a href="https://www.advantshop.net/help/pages/template-settings#3-1" target="_blank"> Main categories </ a >' WHERE [ResourceKey] = 'Admin.Settings.Template.MainPageCategoriesVisibilityTitleLink' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Отвечает за вывод категорий на главной странице. Отобразятся только те категории, для которых включена настройка "Выводить на главной".  <br><br>Подробнее:<br><a href="https://www.advantshop.net/help/pages/template-settings#3-1" target="_blank">Категории на главной</a>' WHERE [ResourceKey] = 'Js.Builder.MainPageCategoriesVisibilityTitleLink' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'Responsible for displaying categories on the main page. Only those categories for which the "Show on home" setting is enabled will be displayed. <br> <br>More details: <br> <a href="https://www.advantshop.net/help/pages/template-settings#3-1" target="_blank"> Main categories </ a >' WHERE [ResourceKey] = 'Js.Builder.MainPageCategoriesVisibilityTitleLink' AND [LanguageId] = 2

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.SettingsCatalog.ImportBrands.ZipFile', '.Zip архив с изображениями')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.SettingsCatalog.ImportBrands.ZipFile', '.Zip archive, photos of brands')

GO--

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Тип пользователя' WHERE [ResourceKey] = 'MyAccount.CommonInfo.CustomerType' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'User Type' WHERE [ResourceKey] = 'MyAccount.CommonInfo.CustomerType' AND [LanguageId] = 2

GO--

IF NOT EXISTS (SELECT * FROM [Order].[ShippingReplaceGeo] WHERE [Id] = 1)
	INSERT INTO [Order].[ShippingReplaceGeo] ([Id],[ShippingType],[InCountryName],[InCountryISO2],[InRegionName],[InCityName],[InDistrict],[OutCountryName],[OutRegionName],[OutCityName],[OutDistrict],[OutDistrictClear],[Enabled],[Sort],[InZip],[OutZip],[Comment])
	VALUES (1,'Sdek','','','Республика Саха (Якутия)','','','','Саха респ. (Якутия)','','',0,0,0,'','','')
ELSE
	UPDATE [Order].[ShippingReplaceGeo]
	SET [OutRegionName] = 'Саха респ. (Якутия)',
		[Enabled] = 0
	WHERE [Id] = 1

IF NOT EXISTS (SELECT * FROM [Order].[ShippingReplaceGeo] WHERE [Id] = 2)
	INSERT INTO [Order].[ShippingReplaceGeo] ([Id],[ShippingType],[InCountryName],[InCountryISO2],[InRegionName],[InCityName],[InDistrict],[OutCountryName],[OutRegionName],[OutCityName],[OutDistrict],[OutDistrictClear],[Enabled],[Sort],[InZip],[OutZip],[Comment])
	VALUES (2,'Sdek','','','Ханты-Мансийский АО - Югра','','','','Ханты-Мансийский авт. округ','','',0,1,0,'','','')
ELSE
	UPDATE [Order].[ShippingReplaceGeo]
	SET [InRegionName] = 'Ханты-Мансийский АО - Югра',
		[OutRegionName] = 'Ханты-Мансийский авт. округ'
	WHERE [Id] = 2

GO--

SET IDENTITY_INSERT [Order].[ShippingReplaceGeo] ON 


IF NOT EXISTS (SELECT * FROM [Order].[ShippingReplaceGeo] WHERE [Id] = 23)
INSERT INTO [Order].[ShippingReplaceGeo] ([Id],[ShippingType],[InCountryName],[InCountryISO2],[InRegionName],[InCityName],[InDistrict],[OutCountryName],[OutRegionName],[OutCityName],[OutDistrict],[OutDistrictClear],[Enabled],[Sort],[InZip],[OutZip],[Comment])
VALUES (23,'Sdek','','','Удмуртская Республика','','','','Удмуртия респ.','','',0,1,0,'','','')

IF NOT EXISTS (SELECT * FROM [Order].[ShippingReplaceGeo] WHERE [Id] = 24)
INSERT INTO [Order].[ShippingReplaceGeo] ([Id],[ShippingType],[InCountryName],[InCountryISO2],[InRegionName],[InCityName],[InDistrict],[OutCountryName],[OutRegionName],[OutCityName],[OutDistrict],[OutDistrictClear],[Enabled],[Sort],[InZip],[OutZip],[Comment])
VALUES (24,'Sdek','','','Республика Северная Осетия - Алания','','','','Северная Осетия респ.','','',0,1,0,'','','')


SET IDENTITY_INSERT [Order].[ShippingReplaceGeo] OFF

GO--

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Заказы со статусом выполнен' WHERE [ResourceKey] = 'Admin.Js.Vortex.ExecutedOrders' AND [LanguageId] = 1

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.ExportImport.BrandFields.Photo', 'Фотография')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.ExportImport.BrandFields.Photo', 'Photo')

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
    DECLARE @Id int,
			@ArtNoUpdateRequired bit

	IF @ArtNo=''
	BEGIN
		SET @ArtNo = CONVERT(nvarchar(100), NEWID())
		SET @ArtNoUpdateRequired = 1
	END

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
    IF @ArtNoUpdateRequired = 1
    BEGIN
		DECLARE @NewArtNo nvarchar(100) = CONVERT(nvarchar(100),@ID)

        IF EXISTS (SELECT * FROM [Catalog].[Product] WHERE [ArtNo] = @NewArtNo)
        BEGIN
            SET @NewArtNo = @NewArtNo + '_' + SUBSTRING(@ArtNo, 1, 5)
        END

        UPDATE [Catalog].[Product] SET [ArtNo] = @NewArtNo WHERE [ProductID] = @ID
    END
    SELECT @ID
END

GO--

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Виджет коммуникаций' WHERE [ResourceKey] = 'Admin.Settings.Social.SocialWidget.Title' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'Communications widget' WHERE [ResourceKey] = 'Admin.Settings.Social.SocialWidget.Title' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Индекс поиска необходим, чтоб собрать информацию о товарах и обеспечить быстрый и точный поиск информации<br/><br/>Подробнее:<br/><a href ="https://www.advantshop.net/help/pages/search#5" target="_blank">Индекс поиска</a>' WHERE [ResourceKey] = 'Admin.Settings.Feedback.FeedbackActionHint' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'A search index is needed to collect information about products and provide a quick and accurate search for information.<br/> <br/> More details: <br/> <a href = "https://www.advantshop.net/help/pages/search # 5 "target ="_blank">Search index</a>' WHERE [ResourceKey] = 'Admin.Settings.Feedback.FeedbackActionHint' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Эта настройка отвечает за текст под строкой поиска, но при нажатии срабатывает как поисковый запрос.<br/><br/>Подробнее: <br/><a href="https://www.advantshop.net/help/pages/search#8" target="_blank">Примеры поискового запроса</a>' WHERE [ResourceKey] = 'Admin.Settings.Catalog.SearchExampleHint' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'This setting is responsible for the text below the search bar, but when clicked, it works like a search query. <br/> <br/> More details: <br/><a href="https://www.advantshop.net/help/pages/search #8" target ="_blank"> Search query examples </a>' WHERE [ResourceKey] = 'Admin.Settings.Catalog.SearchExampleHint' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Примеры поискового запроса' WHERE [ResourceKey] = 'Admin.Settings.Catalog.SearchExample' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'Search terms examples' WHERE [ResourceKey] = 'Admin.Settings.Catalog.SearchExample' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'При полном совпадении слова, которое искали, выдаются только эти результаты. Если нет полного совпадения, поиск в соответствии с выбранными настройками. <br/><br/>Подробнее: <br/><a href="https://www.advantshop.net/help/pages/search#8" target="_blank">Примеры поискового запроса</a>' WHERE [ResourceKey] = 'Admin.Settings.Catalog.MinimizeSearchResultsHint' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'When a full match of the word that was searched for, only these results are given. If there is no full match, search according to the selected settings.<br/> <br/> More details: <br/> <a href="https://www.advantshop.net/help/pages/search#8" target="_blank"> Search query examples</a>' WHERE [ResourceKey] = 'Admin.Settings.Catalog.MinimizeSearchResultsHint' AND [LanguageId] = 2

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Catalog.MinimizeSearchResult', 'Пытаться минимизировать поисковую выдачу')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Catalog.MinimizeSearchResult', 'Try to minimize search results')

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Число указывает на значение возвращённых результатов. Чем больше число, тем больше могут быть разбросаны результаты.<br/><br/>Подробнее: <br/><a href="https://www.advantshop.net/help/pages/search#7" target ="_blank">Максимальное количество найденных результатов.</a> ' WHERE [ResourceKey] = 'Admin.Settings.Catalog.Search_MaxItemsHint' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'The number indicates the value of the returned results. The larger the number, the more scattered the results can be.<br/><br/>More details: <br/><a href="https://www.advantshop.net/help/pages/search#7" target ="_blank">Maximum results found.</a> ' WHERE [ResourceKey] = 'Admin.Settings.Catalog.Search_MaxItemsHint' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Данный параметр отвечает за то, какая валюта будет показана в первый раз в клиентской части магазина для новых пользователей.<br/><br/>Подробнее: <br/><a href="https://www.advantshop.net/help/pages/currency#4" target ="_blank">Валюта по умолчанию.</a> ' WHERE [ResourceKey] = 'Admin.Settings.Catalog.DefaultCurrencyHint' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'This parameter is responsible for which currency will be shown for the first time in the client side of the store for new users.<br/><br/>More details: <br/><a href="https://www.advantshop.net/help/pages/currency#4" target ="_blank">Default currency.</a> ' WHERE [ResourceKey] = 'Admin.Settings.Catalog.DefaultCurrencyHint' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Данный параметр позволяет менять валюты покупателям, в случае, если в магазине несколько валют.<br/><br/>Подробнее: <br/> <a href="https://www.advantshop.net/help/pages/currency#5" target ="_blank">Разрешить покупателям переключение валют.</a> ' WHERE [ResourceKey] = 'Admin.Settings.Catalog.AllowToChangeCurrencyHint' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'This parameter allows customers to change currencies if the store has several currencies.<br/><br/>More details: <br/> <a href="https://www.advantshop.net/help/pages/currency#5" target ="_blank">Allow buyers to switch currencies.  </a> ' WHERE [ResourceKey] = 'Admin.Settings.Catalog.AllowToChangeCurrencyHint' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'При активации настройки обновление валюты будет происходить автоматически.<br/><br/>Подробнее: <br/> <a href="https://www.advantshop.net/help/pages/currency#6" target ="_blank">Ежедневное автообновление курса валют.</a> ' WHERE [ResourceKey] = 'Admin.Settings.Catalog.AutoUpdateCurrenciesHint' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'When the setting is activated, the currency will be updated automatically.<br/><br/>More details: <br/> <a href="https://www.advantshop.net/help/pages/currency#6" target ="_blank">Daily auto-update of exchange rates.</a> ' WHERE [ResourceKey] = 'Admin.Settings.Catalog.AutoUpdateCurrenciesHint' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Выбираете, для кого планируете отображать цены на сайте: для всех пользователей, для зарегистрированных пользователей либо для клиентов определенной группы.<br/><br/>Подробнее: <br/> <a href="https://www.advantshop.net/help/pages/skrytye-tseny#a1" target ="_blank">Видимость цен для пользователя.</a> ' WHERE [ResourceKey] = 'Admin.Settings.Catalog.DisplayModeOfPrices_DisplayForCustomersHint' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'Choose for whom you plan to display prices on the site: for all users, for registered users or for clients of a certain group. <br/> <br/> More details: <br/> <a href ="https://www.advantshop.net/help/pages/skrytye-tseny#a1 "target ="_blank"> Visibility of prices for the user. </a>' WHERE [ResourceKey] = 'Admin.Settings.Catalog.DisplayModeOfPrices_DisplayForCustomersHint' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Эта настройка доступна, когда выбрана в видимость цен для пользователя "Только пользователям из перечисленных групп".<br/><br/>Подробнее: <br/> <a href="https://www.advantshop.net/help/pages/skrytye-tseny#a2" target ="_blank">Отображение цены для групп пользователей.</a> ' WHERE [ResourceKey] = 'Admin.Settings.Catalog.DisplayModeOfPrices_AvalableCustomerGroupsHint' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'This setting is available when selected in the pricing visibility for the user "Only to users from the listed groups". <br/> <br/> More details: <br/> <a href="https://www.advantshop.net/help/pages/skrytye-tseny#a2 "target ="_blank"> Display price for user groups. </a>' WHERE [ResourceKey] = 'Admin.Settings.Catalog.DisplayModeOfPrices_AvalableCustomerGroupsHint' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Данная настройка доступна, когда выбрана одна из двух настроек "Только зарегистрированным пользователям" или "Только пользователям из перечисленных групп". Здесь можно указать текст, который будет выводиться при скрытых ценах.<br/><br/>Подробнее: <br/> <a href="https://www.advantshop.net/help/pages/skrytye-tseny#a3" target ="_blank">Текст, отображаемый при скрытой цене.</a> ' WHERE [ResourceKey] = 'Admin.Settings.Catalog.DisplayModeOfPrices_TextInsteadOfPriceHint' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'This setting is available when one of the two settings "Only registered users" or "Only users from the listed groups" is selected. Here you can specify the text that will be displayed at hidden prices.<br/><br/>More details: <br/> <a href="https://www.advantshop.net/help/pages/skrytye-tseny#a3" target ="_blank">Text displayed at hidden price  </a> ' WHERE [ResourceKey] = 'Admin.Settings.Catalog.DisplayModeOfPrices_TextInsteadOfPriceHint' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Скидка будет применена к товару, если выбранная категория для него основная.<br/><br/>Подробнее: <br/>  <a href="https://www.advantshop.net/help/pages/catalog-main" target="_blank">Подробнее об основных категориях</a>' WHERE [ResourceKey] = 'Admin.Settings.Catalog.CategoryDiscountRegulationHint' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'The discount will be applied to the product if the selected category for it is the main one.<br/> <br/> More details: <br/> <a href="https://www.advantshop.net/help/pages/catalog-main" target="_blank">Read more about main category</a>' WHERE [ResourceKey] = 'Admin.Settings.Catalog.CategoryDiscountRegulationHint' AND [LanguageId] = 2

GO--

-------- making BrandName UNIQUE --------
DECLARE BRANDS_CURSOR CURSOR LOCAL STATIC FORWARD_ONLY 
	FOR SELECT [BrandName]
	FROM [Catalog].[Brand]
	GROUP BY [BrandName]
	HAVING COUNT([BrandName]) > 1

DECLARE @BrandName NVARCHAR(MAX)
OPEN BRANDS_CURSOR
FETCH NEXT FROM BRANDS_CURSOR INTO @BrandName
WHILE @@FETCH_STATUS = 0
BEGIN
	DECLARE @BrandIds TABLE(BrandID INT)
	DECLARE @BrandId int = 0
	DECLARE @Counter int = 0

	INSERT @BrandIds(BrandID)
		SELECT [BrandID]
		FROM [Catalog].[Brand]
		WHERE BrandName = @BrandName
		ORDER BY [BrandID]

	IF ((SELECT COUNT(*) FROM @BrandIds) > 0)
	BEGIN
		DELETE TOP(1) FROM @BrandIds
	END
	
	WHILE ((SELECT COUNT(*) FROM @BrandIds) > 0)
	BEGIN
		SET @Counter = @Counter + 1

		SELECT TOP(1) @BrandId = BrandID
		FROM @BrandIds

		UPDATE [Catalog].[Brand] 
			SET [BrandName] = [BrandName] + ' (' + CONVERT(nvarchar(MAX),@Counter) + ')'
			FROM [Catalog].[Brand]
			WHERE [BrandID] = @BrandId

		DELETE TOP(1) FROM @BrandIds
	END

	FETCH NEXT FROM BRANDS_CURSOR INTO @BrandName
END
CLOSE BRANDS_CURSOR

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Category.AdminCategoryModel.Error.NameExists', 'Такое название уже существует')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Category.AdminCategoryModel.Error.NameExists', 'This name already exists')

GO--

UPDATE [Settings].[InternalSettings] SET [settingValue] = '10.0.0' WHERE [settingKey] = 'db_version'
