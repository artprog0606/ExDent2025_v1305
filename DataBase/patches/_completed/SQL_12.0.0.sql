SET IDENTITY_INSERT [Settings].[MailFormatType] ON

IF NOT EXISTS (SELECT 1 FROM [Settings].[MailFormatType] Where [MailFormatTypeID] = 50)
BEGIN
    INSERT INTO [Settings].[MailFormatType] ([MailFormatTypeID], [TypeName],[SortOrder],[Comment],[MailType])
    VALUES (50, 'При добавлении отзыва о заказе', 440, 'Уведомление о добавлении отзыва о заказе (#ORDER_NUMBER#,#RATIO#,#COMMENT#,#ORDER_LINK#)', 'OnNewOrderReview')
END

SET IDENTITY_INSERT [Settings].[MailFormatType] OFF

GO--

IF NOT EXISTS (SELECT 1 FROM [Settings].[MailFormat] Where [MailFormatTypeId] = 50)
BEGIN
    INSERT INTO [Settings].[MailFormat] ([FormatName],[FormatText],[SortOrder],[Enable],[AddDate],[ModifyDate],[FormatSubject],[MailFormatTypeId])
    VALUES ('При добавлении отзыва о заказе','<p>Покупатель оставил отзыв к заказу <a href="#ORDER_LINK#">№#ORDER_NUMBER#</a></p>
    <p>Оценка: #RATIO#</p>
    <p>Комментарий: #COMMENT#</p>',1630,1,GETDATE(),GETDATE(),'Добавлен отзыв к заказу №#ORDER_NUMBER#',50)
END

GO--

IF NOT EXISTS(SELECT 1
              FROM sys.columns
              WHERE (name = N'Id') AND object_id = OBJECT_ID(N'[Settings].[Localization]'))
    BEGIN
        ALTER TABLE [Settings].[Localization]
			ADD Id INT IDENTITY(1,1) NOT NULL
        ALTER TABLE [Settings].[Localization]
			ADD DateAdded DATETIME NOT NULL DEFAULT(GETDATE())
        ALTER TABLE [Settings].[Localization]
			ADD DateModified DATETIME NOT NULL DEFAULT(GETDATE())
        ALTER TABLE [Settings].[Localization]
			ADD ModifiedBy NVARCHAR(50) NULL
	END

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Settings].[sp_AddUpdateLocalization]') AND type in (N'P'))
	DROP PROCEDURE [Settings].[sp_AddUpdateLocalization]

GO--

CREATE PROCEDURE [Settings].[sp_AddUpdateLocalization]
	@LanguageId INT,
	@ResourceKey NVARCHAR(100),
	@ResourceValue NVARCHAR(MAX),
	@ModifiedBy NVARCHAR(50) = 'SQL_Patch'
AS
BEGIN
	DECLARE @DateNow DATETIME = GETDATE();
	IF NOT EXISTS (SELECT TOP(1) 1 FROM [Settings].[Localization] WHERE LanguageId=@LanguageId and ResourceKey=@ResourceKey)
	BEGIN
		INSERT INTO [Settings].[Localization] (LanguageId, ResourceKey, ResourceValue, DateAdded, DateModified, ModifiedBy)
			VALUES (@LanguageId, @ResourceKey, @ResourceValue, @DateNow, @DateNow, @ModifiedBy);
	END
	ELSE
	BEGIN
		UPDATE [Settings].[Localization] SET ResourceValue=@ResourceValue, DateModified = @DateNow, ModifiedBy = @ModifiedBy
			Where LanguageId=@LanguageId and ResourceKey=@ResourceKey
	End     
END

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Settings.SettingsCatalog.RelatedProductSourceType.FromCategory', 'Из настроенной категории';

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Catalog.RelatedProductSourceType', 'Источник товаров для перекрестного маркетинга (Блок 1)';
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Catalog.RelatedProductSourceType', 'Source of goods for cross-marketing (Block 1)';

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.Builder.RelatedProductSourceType', 'Источник товаров для перекрестного маркетинга (Блок 1)';
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.Builder.RelatedProductSourceType', 'Source of goods for cross-marketing (Block 1)';

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.Builder.SimilarProductSourceType', 'Источник товаров для перекрестного маркетинга (Блок 2)';
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.Builder.SimilarProductSourceType', 'Source of goods for cross-marketing (Block 2)';

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Settings.SettingsCatalog.RelatedProductSourceType.FromCurrentCategory', 'Из текущей категории';
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Settings.SettingsCatalog.RelatedProductSourceType.FromCurrentCategory', 'From current category';

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Catalog.SimilarProductSourceType', 'Источник товаров для перекрестного маркетинга (Блок 2)';
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Catalog.SimilarProductSourceType', 'Source of goods for cross-marketing (Block 2)';

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Catalog.ShowStockAvailability', 'Отображать количество товаров в наличии';
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Catalog.ShowStockAvailability', 'Display the number of products in stock';

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.Builder.ShowStockAvailability', 'Отображать количество товаров в наличии';
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.Builder.ShowStockAvailability', 'Display the number of products in stock';

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.ExportFeed.SettingsAvito.NotExportColorSize', 'Не выводить цвет, размер в описании';
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.ExportFeed.SettingsAvito.NotExportColorSize', 'Not export color, size in description';

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.ExportImport.ExportFeedAvito.EAvitoExportMode.Product', 'Товар';
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.ExportImport.ExportFeedAvito.EAvitoExportMode.Product', 'Product';

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.ExportImport.ExportFeedAvito.EAvitoExportMode.Offer', 'Модификацию';
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.ExportImport.ExportFeedAvito.EAvitoExportMode.Offer', 'Offer';

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.ExportFeed.SettingsAvito.ExportMode', 'Выгружать';
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.ExportFeed.SettingsAvito.ExportMode', 'Export';

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.DiscountByDatetime.DiscountCategories.Reset', '(сбросить)';
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.DiscountByDatetime.DiscountCategories.Reset', '(reset)';

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.DiscountByDatetime.DiscountCategories.Change', '(изменить)';
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.DiscountByDatetime.DiscountCategories.Change', '(change)';

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.DiscountByDatetime.DiscountCategories.All', 'Все';
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.DiscountByDatetime.DiscountCategories.All', 'All';

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.DiscountByDatetime.DiscountCategories.NotSelected', 'Не выбрано';
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.DiscountByDatetime.DiscountCategories.NotSelected', 'Not selected';

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.DiscountByDatetime.DiscountCategories', 'Категории на которые распространяется скидка';
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.DiscountByDatetime.DiscountCategories', 'Categories covered by the discount';

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.DiscountByDatetime.ActiveByTimeCategories', 'Категории, включающиеся на время скидки';
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.DiscountByDatetime.ActiveByTimeCategories', 'Categories enabled at the time of the discount';

GO--

IF NOT EXISTS(SELECT 1
              FROM sys.columns
              WHERE (name = N'ChangePaymentForbidden') AND object_id = OBJECT_ID(N'[Order].[OrderStatus]'))
    BEGIN
        ALTER TABLE [Order].[OrderStatus]
			ADD ChangePaymentForbidden bit NOT NULL DEFAULT 0
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
	@ShowInMenu bit,
	@ChangePaymentForbidden bit
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
		CancelForbidden = @CancelForbidden, ShowInMenu = @ShowInMenu, ChangePaymentForbidden = @ChangePaymentForbidden
		Where OrderStatusID = @OrderStatusID
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
	@CancelForbidden bit,
	@ShowInMenu bit,
	@ChangePaymentForbidden bit
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
	
	insert into [Order].[OrderStatus] (StatusName, CommandID, IsDefault, IsCanceled, IsCompleted, Color, SortOrder, Hidden, CancelForbidden, ShowInMenu,ChangePaymentForbidden) 
							   VALUES (@StatusName, @CommandID, @IsDefault | ~@hasDefault, @IsCanceled, @IsCompleted, @Color, @SortOrder, @Hidden, @CancelForbidden, @ShowInMenu,@ChangePaymentForbidden)
	select SCOPE_IDENTITY()	    
END

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.AddEditOrderStatus.ChangePaymentForbidden', 'Запретить смену способа оплаты клиентом';
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.AddEditOrderStatus.ChangePaymentForbidden', 'Prohibit changing the payment method by the client';

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.Builder.CopyrightText', 'Copyright';
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.Builder.CopyrightText', 'Copyright';

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.Builder.CopyrightMode.Default', 'По умолчанию';
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.Builder.CopyrightMode.Default', 'Default';

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.Builder.CopyrightMode.Custom', 'Настраиваемый';
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.Builder.CopyrightMode.Custom', 'Custom';

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.Builder.ShowCopyrightNote', 'Опция изменяет отображение надписей об Copyright в подвале сайта.<br/><br/>Подробнее:<br/><a href="https://www.advantshop.net/help/pages/footer#4" target="_blank">Copyright</a>';
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.Builder.ShowCopyrightNote', 'This option changes the display of copyright inscriptions in the footer of the site.<br/><br/>Details:<br/><a href="https://www.advantshop.net/help/pages/footer#4" target="_blank">Copyright</a>';

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.Builder.ShowUserAgreementForPromotionalNewsletter', 'Показывать cогласие на получение рассылок';
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.Builder.ShowUserAgreementForPromotionalNewsletter', 'Show the user agreement for subscribing to promotional news';

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.Builder.ShowUserAgreementForPromotionalNewsletterNote', 'Запрашивать подтверждение cогласия на получение рассылок. <br><br> Подробнее: <br> <a href="https://www.advantshop.net/help/pages/152-fz " target="_blank">Как соблюсти требования закона 152-ФЗ на платформе AdvantShop</a>';
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.Builder.ShowUserAgreementForPromotionalNewsletterNote', 'Request confirmation of consent to the terms of the user agreement for promotional news. <br><br><a href="https://www.advantshop.net/help/pages/152-fz " target="_blank">More</a>';

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.Builder.UserAgreementForPromotionalNewsletter', 'Согласие на получение рассылок';
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.Builder.UserAgreementForPromotionalNewsletter', 'User agreement for subscription to promotional news';

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.Builder.SetUserAgreementForPromotionalNewsletterChecked', 'Согласие принято по умолчанию';
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.Builder.SetUserAgreementForPromotionalNewsletterChecked', 'User agreement is accepted by default';

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.Builder.SetUserAgreementForPromotionalNewsletterCheckedHint', 'Если настройка включена, то пользовательское соглашение будет принято автоматически';
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.Builder.SetUserAgreementForPromotionalNewsletterCheckedHint', 'If the setting is enabled, the user agreement will be accepted automatically';

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.Builder.VariableStoreUrl', 'Переменная #STORE_URL# будет заменена на URL магазина';
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.Builder.VariableStoreUrl', 'Variable #STORE_URL# will be replaced with the store URL';

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Mobile.MainPageViewModeNone', 'Не выводить';
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Mobile.MainPageViewModeNone', 'Do not display';

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.CustomerSegments.Filters.OrderPeriod', 'Учитывать заказы за период';
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.CustomerSegments.Filters.OrderPeriod', 'Take into account orders for the period';

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.CustomerSegments.Filters.OrderType', 'Есть заказы с источником';
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.CustomerSegments.Filters.OrderType', 'There are orders with a source';

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.CustomerSegments.Filters.IsInstallMobileApp', 'Устанавливал мобильное приложение';
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.CustomerSegments.Filters.IsInstallMobileApp', 'Installed a mobile app';

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Mobile.ViewCategoriesOnMainBlocksMode', 'Блоками';
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Mobile.ViewCategoriesOnMainBlocksMode', 'Blocks';

GO--

ALTER TABLE Settings.QuartzJobRuns ALTER COLUMN Name nvarchar(255) NOT NULL

GO--

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Order].[OrderRecipient]') AND type in (N'U'))
BEGIN
	CREATE TABLE [Order].[OrderRecipient](
		[OrderId] [int] NOT NULL,
		[FirstName] [nvarchar](70) NOT NULL,
		[LastName] [nvarchar](70) NULL,
		[Patronymic] [nvarchar](1000) NULL,
		[Phone] [nvarchar](70) NULL,
		[StandardPhone] [bigint] NULL
	CONSTRAINT [PK_OrderRecipient] PRIMARY KEY CLUSTERED 
	(
		[OrderId] ASC
	) WITH( PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]

	ALTER TABLE [Order].[OrderRecipient]  WITH CHECK ADD  CONSTRAINT [FK_OrderRecipient_Order] FOREIGN KEY([OrderId])
	REFERENCES [Order].[Order] ([OrderID]) ON UPDATE CASCADE ON DELETE CASCADE
END

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.Order.Recipient', 'Получатель заказа';
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.Order.Recipient', 'Order recipient';

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.Order.Phone', 'Номер телефона';
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.Order.Phone', 'Phone';

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Orders.OrderInfo.OrderRecipient', 'Получатель заказа';
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Orders.OrderInfo.OrderRecipient', 'Order recipient';

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Checkout.CheckoutUser.AddRecipient', 'Добавить получателя';
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Checkout.CheckoutUser.AddRecipient', 'Add recipient';

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Checkout.CheckoutUser.AddRecipient.Help', 'Вы оформляете заказ не для себя, или хотите, чтобы товар забрал другой человек';
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Checkout.CheckoutUser.AddRecipient.Help', 'Are you placing an order not for yourself, or do you want another person to pick up the goods';

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Checkout.IsShowOrderRecipient', 'Разрешить оформлять заказ чтобы забрал другой человек';
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Checkout.IsShowOrderRecipient', 'Allow to place an order to be picked up by another person';

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Checkout.IsShowOrderRecipient.Help', 'При оформлении заказа можно будет указать контактные данные получателя, который заберет заказ';
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Checkout.IsShowOrderRecipient.Help', 'When placing an order, you can specify the contact details of the recipient who will pick up the order';

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.ExportImport.MultiOrder.RecipientFIO', 'ФИО получателя';
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.ExportImport.MultiOrder.RecipientFIO', 'Recipient full name';

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.ExportImport.MultiOrder.RecipientPhone', 'Телефон получателя';
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.ExportImport.MultiOrder.RecipientPhone', 'Phone';

GO--


Update [CRM].[TriggerAction] 
Set EmailBody = Replace( Replace(EmailBody, '#ORDERID#', '#ORDER_NUMBER#') , '#ORDER_ID#', '#ORDER_NUMBER#'),
	EmailSubject = Replace( Replace(EmailSubject, '#ORDERID#', '#ORDER_NUMBER#') , '#ORDER_ID#', '#ORDER_NUMBER#'),
	SmsText = Replace( Replace(SmsText, '#ORDERID#', '#ORDER_NUMBER#') , '#ORDER_ID#', '#ORDER_NUMBER#'),
	MessageText = Replace( Replace(MessageText, '#ORDERID#', '#ORDER_NUMBER#') , '#ORDER_ID#', '#ORDER_NUMBER#'),
	NotificationBody = Replace( Replace(NotificationBody, '#ORDERID#', '#ORDER_NUMBER#') , '#ORDER_ID#', '#ORDER_NUMBER#'),
	NotificationTitle = Replace( Replace(NotificationTitle, '#ORDERID#', '#ORDER_NUMBER#') , '#ORDER_ID#', '#ORDER_NUMBER#')
Where TriggerRuleId in (Select Id From [CRM].[TriggerRule] Where [ObjectType] = 1)

GO--

Update [CRM].[TriggerAction] 
Set EmailSubject = Replace(EmailSubject, '#NUMBER#', '#ORDER_NUMBER#'), 
	EmailBody = Replace(EmailBody, '#NUMBER#', '#ORDER_NUMBER#'), 
	SmsText = Replace(SmsText, '#NUMBER#', '#ORDER_NUMBER#'),
	MessageText = Replace(MessageText, '#NUMBER#', '#ORDER_NUMBER#'),
	NotificationBody = Replace(NotificationBody, '#NUMBER#', '#ORDER_NUMBER#'),
	NotificationTitle = Replace(NotificationTitle, '#NUMBER#', '#ORDER_NUMBER#')
Where TriggerRuleId in (Select Id From [CRM].[TriggerRule] Where [ObjectType] = 1)

GO--

Update [CRM].[TriggerAction] 
Set EmailSubject = Replace(EmailSubject, '#ORDERSTATUS#', '#ORDER_STATUS#'), 
	EmailBody = Replace(EmailBody, '#ORDERSTATUS#', '#ORDER_STATUS#'), 
	SmsText = Replace(SmsText, '#ORDERSTATUS#', '#ORDER_STATUS#'),
	MessageText = Replace(MessageText, '#ORDERSTATUS#', '#ORDER_STATUS#'),
	NotificationBody = Replace(NotificationBody, '#ORDERSTATUS#', '#ORDER_STATUS#'),
	NotificationTitle = Replace(NotificationTitle, '#ORDERSTATUS#', '#ORDER_STATUS#')	
Where TriggerRuleId in (Select Id From [CRM].[TriggerRule] Where [ObjectType] = 1)

GO--

Update [CRM].[TriggerAction] 
Set EmailSubject = Replace(EmailSubject, '#TRACKNUMBER#', '#ORDER_TRACK_NUMBER#'), 
	EmailBody = Replace(EmailBody, '#TRACKNUMBER#', '#ORDER_TRACK_NUMBER#'), 
	SmsText = Replace(SmsText, '#TRACKNUMBER#', '#ORDER_TRACK_NUMBER#'),
	MessageText = Replace(MessageText, '#TRACKNUMBER#', '#ORDER_TRACK_NUMBER#'),
	NotificationBody = Replace(NotificationBody, '#TRACKNUMBER#', '#ORDER_TRACK_NUMBER#'),
	NotificationTitle = Replace(NotificationTitle, '#TRACKNUMBER#', '#ORDER_TRACK_NUMBER#')
Where TriggerRuleId in (Select Id From [CRM].[TriggerRule] Where [ObjectType] = 1)

GO--

Update [CRM].[TriggerAction] 
Set EmailSubject = Replace(EmailSubject, '#SHIPPINGMETHOD#', '#SHIPPING_NAME_FULL#'), 
	EmailBody = Replace(EmailBody, '#SHIPPINGMETHOD#', '#SHIPPING_NAME_FULL#'), 
	SmsText = Replace(SmsText, '#SHIPPINGMETHOD#', '#SHIPPING_NAME_FULL#'),
	MessageText = Replace(MessageText, '#SHIPPINGMETHOD#', '#SHIPPING_NAME_FULL#'),
	NotificationBody = Replace(NotificationBody, '#SHIPPINGMETHOD#', '#SHIPPING_NAME_FULL#'),
	NotificationTitle = Replace(NotificationTitle, '#SHIPPINGMETHOD#', '#SHIPPING_NAME_FULL#')
Where TriggerRuleId in (Select Id From [CRM].[TriggerRule] Where [ObjectType] = 1)

GO--

Update [CRM].[TriggerAction] 
Set EmailSubject = Replace(EmailSubject, '#PAYMENTTYPE#', '#PAYMENT_NAME#'), 
	EmailBody = Replace(EmailBody, '#PAYMENTTYPE#', '#PAYMENT_NAME#'), 
	SmsText = Replace(SmsText, '#PAYMENTTYPE#', '#PAYMENT_NAME#'),
	MessageText = Replace(MessageText, '#PAYMENTTYPE#', '#PAYMENT_NAME#'),
	NotificationBody = Replace(NotificationBody, '#PAYMENTTYPE#', '#PAYMENT_NAME#'),
	NotificationTitle = Replace(NotificationTitle, '#PAYMENTTYPE#', '#PAYMENT_NAME#')
Where TriggerRuleId in (Select Id From [CRM].[TriggerRule] Where [ObjectType] = 1)

GO--

Update [CRM].[TriggerAction] 
Set EmailSubject = Replace(EmailSubject, '#BILLING_SHORTLINK#', '#BILLING_SHORT_LINK#'), 
	EmailBody = Replace(EmailBody, '#BILLING_SHORTLINK#', '#BILLING_SHORT_LINK#'), 
	SmsText = Replace(SmsText, '#BILLING_SHORTLINK#', '#BILLING_SHORT_LINK#'),
	MessageText = Replace(MessageText, '#BILLING_SHORTLINK#', '#BILLING_SHORT_LINK#'),
	NotificationBody = Replace(NotificationBody, '#BILLING_SHORTLINK#', '#BILLING_SHORT_LINK#'),
	NotificationTitle = Replace(NotificationTitle, '#BILLING_SHORTLINK#', '#BILLING_SHORT_LINK#')
Where TriggerRuleId in (Select Id From [CRM].[TriggerRule] Where [ObjectType] = 1)

GO--

Update [CRM].[TriggerAction] 
Set EmailSubject = Replace(EmailSubject, '#ORDERTABLE#', '#ORDER_ITEMS_HTML#'), 
	EmailBody = Replace(EmailBody, '#ORDERTABLE#', '#ORDER_ITEMS_HTML#'), 
	SmsText = Replace(SmsText, '#ORDERTABLE#', '#ORDER_ITEMS_HTML#'),
	MessageText = Replace(MessageText, '#ORDERTABLE#', '#ORDER_ITEMS_HTML#'),
	NotificationBody = Replace(NotificationBody, '#ORDERTABLE#', '#ORDER_ITEMS_HTML#'),
	NotificationTitle = Replace(NotificationTitle, '#ORDERTABLE#', '#ORDER_ITEMS_HTML#')
Where TriggerRuleId in (Select Id From [CRM].[TriggerRule] Where [ObjectType] = 1)

GO--

Update [CRM].[TriggerAction] 
Set EmailSubject = Replace(EmailSubject, '#CURRENTCURRENCYCODE#', '#ORDER_CURRENCY_CODE#'), 
	EmailBody = Replace(EmailBody, '#CURRENTCURRENCYCODE#', '#ORDER_CURRENCY_CODE#'), 
	SmsText = Replace(SmsText, '#CURRENTCURRENCYCODE#', '#ORDER_CURRENCY_CODE#'),
	MessageText = Replace(MessageText, '#CURRENTCURRENCYCODE#', '#ORDER_CURRENCY_CODE#'),
	NotificationBody = Replace(NotificationBody, '#CURRENTCURRENCYCODE#', '#ORDER_CURRENCY_CODE#'),
	NotificationTitle = Replace(NotificationTitle, '#CURRENTCURRENCYCODE#', '#ORDER_CURRENCY_CODE#')
Where TriggerRuleId in (Select Id From [CRM].[TriggerRule] Where [ObjectType] = 1)

GO--

Update [CRM].[TriggerAction] 
Set EmailSubject = Replace(EmailSubject, '#ADDITIONALCUSTOMERFIELDS#', '#ADDITIONAL_CUSTOMER_FIELDS#'), 
	EmailBody = Replace(EmailBody, '#ADDITIONALCUSTOMERFIELDS#', '#ADDITIONAL_CUSTOMER_FIELDS#'), 
	SmsText = Replace(SmsText, '#ADDITIONALCUSTOMERFIELDS#', '#ADDITIONAL_CUSTOMER_FIELDS#'),
	MessageText = Replace(MessageText, '#ADDITIONALCUSTOMERFIELDS#', '#ADDITIONAL_CUSTOMER_FIELDS#'),
	NotificationBody = Replace(NotificationBody, '#ADDITIONALCUSTOMERFIELDS#', '#ADDITIONAL_CUSTOMER_FIELDS#'),
	NotificationTitle = Replace(NotificationTitle, '#ADDITIONALCUSTOMERFIELDS#', '#ADDITIONAL_CUSTOMER_FIELDS#')
Where TriggerRuleId in (Select Id From [CRM].[TriggerRule] Where [ObjectType] = 1)

GO--

Update [CRM].[TriggerAction] 
Set EmailSubject = Replace(EmailSubject, '#CUSTOMERCONTACTS#', '#CUSTOMER_CONTACTS_HTML#'), 
	EmailBody = Replace(EmailBody, '#CUSTOMERCONTACTS#', '#CUSTOMER_CONTACTS_HTML#'), 
	SmsText = Replace(SmsText, '#CUSTOMERCONTACTS#', '#CUSTOMER_CONTACTS_HTML#'),
	MessageText = Replace(MessageText, '#CUSTOMERCONTACTS#', '#CUSTOMER_CONTACTS_HTML#'),
	NotificationBody = Replace(NotificationBody, '#CUSTOMERCONTACTS#', '#CUSTOMER_CONTACTS_HTML#'),
	NotificationTitle = Replace(NotificationTitle, '#CUSTOMERCONTACTS#', '#CUSTOMER_CONTACTS_HTML#')
Where TriggerRuleId in (Select Id From [CRM].[TriggerRule] Where [ObjectType] = 1)

GO--

Update [CRM].[TriggerAction] 
Set EmailSubject = Replace(EmailSubject, '#STATUSCOMMENT#', '#ORDER_STATUS_COMMENT_HTML#'), 
	EmailBody = Replace(EmailBody, '#STATUSCOMMENT#', '#ORDER_STATUS_COMMENT_HTML#'), 
	SmsText = Replace(SmsText, '#STATUSCOMMENT#', '#ORDER_STATUS_COMMENT_HTML#'),
	MessageText = Replace(MessageText, '#STATUSCOMMENT#', '#ORDER_STATUS_COMMENT_HTML#'),
	NotificationBody = Replace(NotificationBody, '#STATUSCOMMENT#', '#ORDER_STATUS_COMMENT_HTML#'),
	NotificationTitle = Replace(NotificationTitle, '#STATUSCOMMENT#', '#ORDER_STATUS_COMMENT_HTML#')
Where TriggerRuleId in (Select Id From [CRM].[TriggerRule] Where [ObjectType] = 1)

GO--

Update [CRM].[TriggerAction] 
Set EmailSubject = Replace(EmailSubject, '#COMMENTS#', '#ORDER_CUSTOMER_COMMENT#'), 
	EmailBody = Replace(EmailBody, '#COMMENTS#', '#ORDER_CUSTOMER_COMMENT#'), 
	SmsText = Replace(SmsText, '#COMMENTS#', '#ORDER_CUSTOMER_COMMENT#'),
	MessageText = Replace(MessageText, '#COMMENTS#', '#ORDER_CUSTOMER_COMMENT#'),
	NotificationBody = Replace(NotificationBody, '#COMMENTS#', '#ORDER_CUSTOMER_COMMENT#'),
	NotificationTitle = Replace(NotificationTitle, '#COMMENTS#', '#ORDER_CUSTOMER_COMMENT#')
Where TriggerRuleId in (Select Id From [CRM].[TriggerRule] Where [ObjectType] = 1)

GO--

Update [CRM].[TriggerAction] 
Set EmailSubject = Replace(EmailSubject, '#FIRSTNAME#', '#FIRST_NAME#'), 
	EmailBody = Replace(EmailBody, '#FIRSTNAME#', '#FIRST_NAME#'), 
	SmsText = Replace(SmsText, '#FIRSTNAME#', '#FIRST_NAME#'),
	MessageText = Replace(MessageText, '#FIRSTNAME#', '#FIRST_NAME#'),
	NotificationBody = Replace(NotificationBody, '#FIRSTNAME#', '#FIRST_NAME#'),
	NotificationTitle = Replace(NotificationTitle, '#FIRSTNAME#', '#FIRST_NAME#')
Where TriggerRuleId in (Select Id From [CRM].[TriggerRule] Where [ObjectType] = 1)

GO--

Update [CRM].[TriggerAction] 
Set EmailSubject = Replace(EmailSubject, '#LASTNAME#', '#LAST_NAME#'), 
	EmailBody = Replace(EmailBody, '#LASTNAME#', '#LAST_NAME#'), 
	SmsText = Replace(SmsText, '#LASTNAME#', '#LAST_NAME#'),
	MessageText = Replace(MessageText, '#LASTNAME#', '#LAST_NAME#'),
	NotificationBody = Replace(NotificationBody, '#LASTNAME#', '#LAST_NAME#'),
	NotificationTitle = Replace(NotificationTitle, '#LASTNAME#', '#LAST_NAME#')
Where TriggerRuleId in (Select Id From [CRM].[TriggerRule] Where [ObjectType] = 1)

GO--

Update [CRM].[TriggerAction] 
Set EmailSubject = Replace(EmailSubject, '#COMPANYNAME#', '#COMPANY_NAME#'), 
	EmailBody = Replace(EmailBody, '#COMPANYNAME#', '#COMPANY_NAME#'), 
	SmsText = Replace(SmsText, '#COMPANYNAME#', '#COMPANY_NAME#'),
	MessageText = Replace(MessageText, '#COMPANYNAME#', '#COMPANY_NAME#'),
	NotificationBody = Replace(NotificationBody, '#COMPANYNAME#', '#COMPANY_NAME#'),
	NotificationTitle = Replace(NotificationTitle, '#COMPANYNAME#', '#COMPANY_NAME#')
Where TriggerRuleId in (Select Id From [CRM].[TriggerRule] Where [ObjectType] = 1)

GO--

Update [CRM].[TriggerAction] 
Set EmailSubject = Replace(EmailSubject, '#TOTALPRICE#', '#SUM#'), 
	EmailBody = Replace(EmailBody, '#TOTALPRICE#', '#SUM#'), 
	SmsText = Replace(SmsText, '#TOTALPRICE#', '#SUM#'),
	MessageText = Replace(MessageText, '#TOTALPRICE#', '#SUM#'),
	NotificationBody = Replace(NotificationBody, '#TOTALPRICE#', '#SUM#'),
	NotificationTitle = Replace(NotificationTitle, '#TOTALPRICE#', '#SUM#')
Where TriggerRuleId in (Select Id From [CRM].[TriggerRule] Where [ObjectType] = 1)

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.Builder.ShowAvailableLableInProduct', 'Отображать маркер "Есть в наличии"';
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.Builder.ShowAvailableLableInProduct', 'Show "Available" label';

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.Builder.OptionShowAvailableLable', 'Опция определяет, отображать или нет маркер "Есть в наличии" в карточке товара';
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.Builder.OptionShowAvailableLable', 'The option determines whether or not to display the marker "Available" in the product card';

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.Builder.ShowNotAvailableLableInProduct', 'Отображать маркер "Нет в наличии"';
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.Builder.ShowNotAvailableLableInProduct', 'Show "Not available" label';

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.Builder.OptionShowNotAvailableLable', 'Опция определяет, отображать или нет маркер "Нет в наличии" в карточке товара';
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.Builder.OptionShowNotAvailableLable', 'The option determines whether or not to display the marker "Not available" in the product card';

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.Builder.DefaultSortOrderProductInBrand', 'Сортировка товаров по умолчанию';
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.Builder.DefaultSortOrderProductInBrand', 'Sorting products by default';

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.Builder.DefaultSortOrderProductInBrandHelp', 'При открытии страницы бренда, товары будут отсортированы по умолчанию';
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.Builder.DefaultSortOrderProductInBrandHelp', 'When opening brand pages, products will be sorted by default';

GO--

ALTER TABLE [Order].[PaymentDetails] ADD
	[Change] [nvarchar](255) NULL

GO--

ALTER PROCEDURE [Order].[sp_AddPaymentDetails]
	@OrderID int,
	@CompanyName nvarchar(255),
	@INN nvarchar(255),
	@Phone nvarchar(20),
	@Contract nvarchar(255),
	@Change nvarchar(255),
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
		   ,[Change]
		   ,[IsCashOnDeliveryPayment]
		   ,[IsPickPointPayment])
     VALUES
           (@OrderID
		   ,@CompanyName
		   ,@INN
		   ,@Phone
		   ,@Contract
		   ,@Change
		   ,@IsCashOnDeliveryPayment
		   ,@IsPickPointPayment)
	RETURN SCOPE_IDENTITY()
END

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Payment.PaymentDetails.Change', 'Сумма с которой необходима сдача';
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Payment.PaymentDetails.Change', 'The amount from which the change is required';

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.OrderItemsSummary.Change', 'Сумма с которой необходима сдача';
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.OrderItemsSummary.Change', 'The amount from which the change is required';

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.Payment.Cash.Change', 'С какой суммы подготовить сдачу?';
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.Payment.Cash.Change', 'With what amount to prepare the change?';

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.PaymentMethods.Cash.ShowPaymentDetails', 'Вывести запрос суммы для сдачи';
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.PaymentMethods.Cash.ShowPaymentDetails', 'Display a request for the amount to change.';

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.PaymentMethods.Cash.RequiredPaymentDetails', 'Обязательное заполнение суммы для сдачи';
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.PaymentMethods.Cash.RequiredPaymentDetails', 'Mandatory filling of the amount for change.';

GO-- 

ALTER TABLE Customers.Contact ADD
	DadataJson nvarchar(MAX) NULL
GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Customers.Contact.IsMain', 'Основной адрес';
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Customers.Contact.IsMain', 'Default address';

GO--

ALTER TABLE Customers.Contact ADD
	IsMain bit NULL
GO--

Update [Customers].[Contact] 
Set IsMain = (CASE WHEN (Select top(1) c.ContactId From [Customers].[Contact] c Where c.CustomerID = [Contact].CustomerID) = [Contact].ContactID THEN 1 ELSE 0 END) 
Where IsMain is null

GO--

ALTER TABLE Customers.Contact ALTER COLUMN
	IsMain bit NOT NULL
GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Social.SocialWidget.SocialWidgetChatInvitation', 'Приглашение в чат';
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Social.SocialWidget.SocialWidgetChatInvitation', 'Chat invitation';

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Social.SocialWidget.SocialWidgetShowPopover', 'Включить';
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Social.SocialWidget.SocialWidgetShowPopover', 'Turn on';

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Social.SocialWidget.SocialWidgetPopoverText', 'Текст приглашения';
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Social.SocialWidget.SocialWidgetPopoverText', 'Invitation text';

GO-- 

INSERT INTO [Settings].[Settings]([Name],[Value]) VALUES('SettingsSocialWidget.ShowPopover', 'True')
INSERT INTO [Settings].[Settings]([Name],[Value]) VALUES('SettingsSocialWidget.PopoverText', 'Есть вопросы?<br>Мы готовы на них ответить!')

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.MobileVersion.ShowBriefDescription', 'Отображать краткое описание';
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.MobileVersion.ShowBriefDescription', 'Show briefdescription';  

GO-- 

IF NOT EXISTS(SELECT *
              FROM DBO.MODULES
              WHERE MODULESTRINGID = 'MoySklad')
    AND EXISTS(SELECT *
               FROM SYS.OBJECTS
               WHERE OBJECT_ID = OBJECT_ID(N'[Catalog].[ProductSuncMoysklad]')
                 AND TYPE IN (N'U'))
    BEGIN
        IF (SELECT COUNT(*) FROM CATALOG.PRODUCTSUNCMOYSKLAD) = 0
            DROP TABLE CATALOG.PRODUCTSUNCMOYSKLAD
    END

GO--

IF NOT EXISTS(SELECT *
              FROM DBO.MODULES
              WHERE MODULESTRINGID = 'MoySklad')
    AND EXISTS(SELECT *
               FROM SYS.OBJECTS
               WHERE OBJECT_ID = OBJECT_ID(N'[Order].[OrderItemsFromMoysklad]')
                 AND TYPE IN (N'U'))
    BEGIN
        IF (SELECT COUNT(*) FROM [Order].ORDERITEMSFROMMOYSKLAD) = 0
            DROP TABLE [Order].ORDERITEMSFROMMOYSKLAD
    END

GO--

IF NOT EXISTS(SELECT *
              FROM DBO.MODULES
              WHERE MODULESTRINGID = 'MoySklad')
    AND EXISTS(SELECT *
               FROM SYS.OBJECTS
               WHERE OBJECT_ID = OBJECT_ID(N'[Order].[OrderSendMoysklad]')
                 AND TYPE IN (N'U'))
    BEGIN
        DELETE FROM [Order].ORDERSENDMOYSKLAD WHERE ORDERID = 1
        IF (SELECT COUNT(*) FROM [Order].ORDERSENDMOYSKLAD) = 0
            DROP TABLE [Order].ORDERSENDMOYSKLAD
    END

GO--

IF NOT EXISTS(SELECT *
              FROM DBO.MODULES
              WHERE MODULESTRINGID = 'Voting')
    AND EXISTS(SELECT *
               FROM SYS.OBJECTS
               WHERE OBJECT_ID = OBJECT_ID(N'[Voice].[Answer]')
                 AND TYPE IN (N'U'))
    BEGIN
        IF (SELECT COUNT(*) FROM VOICE.ANSWER WHERE FKIDTheme = 4) >= 3
            DELETE FROM VOICE.ANSWER WHERE ANSWERID IN (1, 2, 3)
        IF (SELECT COUNT(*) FROM VOICE.ANSWER) = 0
            DROP TABLE VOICE.ANSWER
    END

GO--

IF NOT EXISTS(SELECT *
              FROM DBO.MODULES
              WHERE MODULESTRINGID = 'Voting')
    AND NOT EXISTS(SELECT *
               FROM SYS.OBJECTS
               WHERE OBJECT_ID = OBJECT_ID(N'[Voice].[Answer]')
                 AND TYPE IN (N'U'))
    AND EXISTS(SELECT *
               FROM SYS.OBJECTS
               WHERE OBJECT_ID = OBJECT_ID(N'[Voice].[VoiceTheme]')
                 AND TYPE IN (N'U'))
    BEGIN
        DELETE FROM VOICE.VOICETHEME WHERE VOICETHEMEID = 4
        IF (SELECT COUNT(*) FROM VOICE.VOICETHEME) = 0
            DROP TABLE VOICE.VOICETHEME
    END

GO--

IF NOT EXISTS(SELECT *
              FROM DBO.MODULES
              WHERE MODULESTRINGID = 'Voting')
    AND NOT EXISTS(SELECT *
                   FROM SYS.OBJECTS
                   WHERE OBJECT_ID = OBJECT_ID(N'[Voice].[Answer]')
                     AND TYPE IN (N'U'))
    AND NOT EXISTS(SELECT *
                   FROM SYS.OBJECTS
                   WHERE OBJECT_ID = OBJECT_ID(N'[Voice].[VoiceTheme]')
                     AND TYPE IN (N'U'))
    BEGIN
        IF EXISTS(SELECT *
                  FROM SYS.OBJECTS
                  WHERE OBJECT_ID = OBJECT_ID(N'[Voice].[sp_Add_Voice]')
                    AND TYPE IN (N'P'))
            DROP PROCEDURE VOICE.SP_ADD_VOICE
        IF EXISTS(SELECT *
                  FROM SYS.OBJECTS
                  WHERE OBJECT_ID = OBJECT_ID(N'[Voice].[sp_Get_ListAnswerOfTheme]')
                    AND TYPE IN (N'P'))
            DROP PROCEDURE VOICE.SP_GET_LISTANSWEROFTHEME
        IF EXISTS(SELECT *
                  FROM SYS.OBJECTS
                  WHERE OBJECT_ID = OBJECT_ID(N'[Voice].[sp_Set_Visible]')
                    AND TYPE IN (N'P'))
            DROP PROCEDURE VOICE.SP_SET_VISIBLE

        IF EXISTS(SELECT *
                  FROM SYS.OBJECTS
                  WHERE OBJECT_ID = OBJECT_ID(N'[Voice].[sp_Valid_PSYIDTheme]')
                    AND TYPE IN (N'P'))
            DROP PROCEDURE VOICE.SP_VALID_PSYIDTHEME

        IF EXISTS(SELECT * FROM SYS.SCHEMAS WHERE NAME = 'Voice')
            DROP SCHEMA VOICE
    END

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.Builder.DefaultCityIfNotAutodetect', 'Город по умолчанию, если отключено автоопределение';
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.Builder.DefaultCityIfNotAutodetect', 'Default city if auto-detection is disabled';  

GO-- 

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.Builder.WhoAllowReviews', 'Разрешить оставлять отзывы к товарам';
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.Builder.WhoAllowReviews', 'Who can add reviews to products';  

GO-- 

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.Builder.WhoCanAddReviewsHelp', 'Опция определяет, кому разрешить добавление отзывов к товарам.';
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.Builder.WhoCanAddReviewsHelp', 'The option determines who is allowed to add reviews to products.';  

GO-- 


EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.ExportFeed.SettingsYandex.ExportMarketStepQuantityHint', 'Выгружать квант продажи в тег step-quantity. Если квант продажи не указан, то смотрится кратность товара. Если кратность товара целое число, то она выгрузится в тег step-quantity.';
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.ExportFeed.SettingsYandex.ExportMarketStepQuantityHint', 'Export step quantity in step-quantity tag. If step quantity is not specified, then the multiplicity of the product looks. If the multiplicity of the product is an integer, then it will be exported to step-quantity tag.';  

GO-- 

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Order].[OrderReview]') AND type in (N'U'))
BEGIN
	CREATE TABLE [Order].[OrderReview](
		[OrderId] [int] NOT NULL,
		[Ratio] [float] NOT NULL,
		[Text] [NVARCHAR](MAX) NULL
	CONSTRAINT [PK_OrderReview] PRIMARY KEY CLUSTERED 
	(
		[OrderId] ASC
	) WITH( PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]

	ALTER TABLE [Order].[OrderReview]  WITH CHECK ADD  CONSTRAINT [FK_OrderReview_Order] FOREIGN KEY([OrderId])
	REFERENCES [Order].[Order] ([OrderID]) ON UPDATE CASCADE ON DELETE CASCADE
END

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.Order.OrderReview', 'Оценить заказ';
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.Order.OrderReview', 'Rate the order';

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.Order.MyOrderReview', 'Моя оценка заказа';
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.Order.MyOrderReview', 'My rate of the order';

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.Order.OrderReview.Rate', 'Оценитe заказ';
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.Order.OrderReview.Rate', 'Rate the order';

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.Order.OrderReview.Text', 'Комментарий';
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.Order.OrderReview.Text', 'Comment';

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Orders.OrderInfo.OrderReview', 'Оценка заказа';
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Orders.OrderInfo.OrderReview', 'Order review';

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Orders.OrderInfo.OrderReview.Text', 'Комментарий';
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Orders.OrderInfo.OrderReview.Text', 'Comment';

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.Order.ReviewIsEmpty', 'Необходимо указать оценку или комментарий';
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.Order.ReviewIsEmpty', 'You must specify a rating or comment';

GO--

if exists (Select 1 From [Settings].[MailFormatType] Where [MailType] = 'OnBuyInOneClick')
begin
	Update [Settings].[MailFormat]
	Set [FormatText] = Replace([FormatText], '#NAME#', '#FULL_NAME#') 
	Where MailFormatTypeId = (Select top(1) [MailFormatTypeID] From [Settings].[MailFormatType] Where [MailType] = 'OnBuyInOneClick')
end

GO--

if exists (Select 1 From [Settings].[MailFormatType] Where [MailType] = 'OnBuyInOneClick')
begin
	Update [Settings].[MailFormat]
	Set [FormatText] = Replace([FormatText], '<div class=''l-row''>                      <div class=''l-name vi cs-light'' style=''color: #acacac; display: inline-block; margin: 5px 0; padding-right: 15px; width: 150px; vertical-align: middle;''>                          Имя:                      </div>                      <div class=''l-value vi'' style=''display: inline-block; margin: 5px 0;''>                          #FULL_NAME#                      </div>                  </div>              </div>', '</div>') 
	Where MailFormatTypeId = (Select top(1) [MailFormatTypeID] From [Settings].[MailFormatType] Where [MailType] = 'OnBuyInOneClick')
end

GO--

if exists (Select 1 From [Settings].[MailFormatType] Where [MailType] = 'OnBuyInOneClick')
begin
	Update [Settings].[MailFormat]
	Set [FormatText] = Replace([FormatText], '<div class="comment" style="margin-top: 15px;"><span class="comment-title" style="font-weight: bold;">Ваши комментарии к заказу: </span> <span class="comment-txt" style="padding-left: 5px;"> #ORDER_CUSTOMER_COMMENT# </span></div>', '') 
	Where MailFormatTypeId = (Select top(1) [MailFormatTypeID] From [Settings].[MailFormatType] Where [MailType] = 'OnBuyInOneClick')
end

GO--

if exists (Select 1 From [Settings].[MailFormatType] Where [MailType] = 'OnPreOrder')
begin
	Update [Settings].[MailFormat]
	Set [FormatText] = Replace([FormatText], '#NAME#', '#FULL_NAME#') 
	Where MailFormatTypeId = (Select top(1) [MailFormatTypeID] From [Settings].[MailFormatType] Where [MailType] = 'OnPreOrder')
end

GO--

if exists (Select 1 From [Settings].[MailFormatType] Where [MailType] = 'OnPreOrder')
begin
	Update [Settings].[MailFormat]
	Set [FormatText] = Replace([FormatText], '<div class=''l-row''>                      <div class=''l-name vi cs-light'' style=''color: #acacac; display: inline-block; margin: 5px 0; padding-right: 15px; width: 150px; vertical-align: middle;''>                          Имя:                      </div>                      <div class=''l-value vi'' style=''display: inline-block; margin: 5px 0;''>                          #FULL_NAME#                      </div>                  </div>              </div>', '</div>') 
	Where MailFormatTypeId = (Select top(1) [MailFormatTypeID] From [Settings].[MailFormatType] Where [MailType] = 'OnPreOrder')
end

GO--

if exists (Select 1 From [Settings].[MailFormatType] Where [MailType] = 'OnPreOrder')
begin
	Update [Settings].[MailFormat]
	Set [FormatText] = Replace([FormatText], '<div class="comment" style="margin-top: 15px;"><span class="comment-title" style="font-weight: bold;">Ваши комментарии к заказу: </span> <span class="comment-txt" style="padding-left: 5px;"> #ORDER_CUSTOMER_COMMENT# </span></div>', '') 
	Where MailFormatTypeId = (Select top(1) [MailFormatTypeID] From [Settings].[MailFormatType] Where [MailType] = 'OnPreOrder')
end

GO--

If Exists (Select 1 FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS
                    WHERE CONSTRAINT_SCHEMA = 'Vk'
                      AND CONSTRAINT_NAME ='FK_VkProduct_Product')
Begin
	ALTER TABLE Vk.VkProduct
		DROP CONSTRAINT FK_VkProduct_Product
End

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.Builder.OtherItemsTitle', 'Дополнительные настройки шаблона';
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.Builder.OtherItemsTitle', 'Additional template settings';

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.Builder.MenuList.Other', 'Дополнительные настройки';
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.Builder.MenuList.Other', 'Additional settings';

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Template.ShowProductsPhotoInCheckoutCart', 'Отображать фото товаров в корзине';
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Template.ShowProductsPhotoInCheckoutCart', 'Display photos of items in the cart';

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Template.ShowProductsPhotoInCheckoutCart', 'Опция определяет, отображать или нет фото товаров в блоке корзины на странице оформления заказов';
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Template.ShowProductsPhotoInCheckoutCart', 'The option determines whether or not to display product photos in the cart block on the checkout page';

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Product.AvailablePreorder', 'Под заказ';
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Product.AvailablePreorder', 'PreOrder';

GO--

if exists (Select 1 From [Settings].[Settings] Where Name = 'SettingsVk.GroupMessageErrorStatus')
	Update [Settings].[Settings] 
	Set [Value] = ''
	Where Name = 'SettingsVk.GroupMessageErrorStatus'
	
GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Product.LeftMenu.Fiscalization', 'Фискализация';
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Product.LeftMenu.Fiscalization', 'Fiscalization';

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Product.Edit.Fiscalization', 'Фискализация';
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Product.Edit.Fiscalization', 'Fiscalization';

GO--

ALTER TABLE CMS.CarouselApi ADD
	CategoryId int NULL

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.DeliveryByZonesList.FreeOfFrom', 'бесплатно от';
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.DeliveryByZonesList.FreeOfFrom', 'free of from';
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.AddDeliveryZone.CostFree', 'Бесплатно при стоимости заказа от';
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.AddDeliveryZone.CostFree', 'Free for orders over';
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.AddDeliveryZone.AvailablePaymentMethods', 'Доступные методы оплаты';
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.AddDeliveryZone.AvailablePaymentMethods', 'Available payment methods';

GO--

DECLARE @enabled nvarchar(10) = (SELECT TOP(1) Value FROM [Settings].[Settings] WHERE Name = 'BonusSystem.SmsEnabled')

IF @enabled = 'True' 
	BEGIN
		DECLARE @notificationMethod  nvarchar(2) = (SELECT TOP(1) Value FROM [Settings].[Settings] WHERE Name = 'BonusSystem.NotificationMethod')
		IF @notificationMethod = '2'
			BEGIN
				UPDATE [Settings].[Settings] SET Value = '0,1' WHERE Name = 'BonusSystem.NotificationMethod';
			END
	END
IF @enabled IS NULL OR @enabled = '' OR @enabled = 'False'
	BEGIN
		DELETE [Settings].[Settings] WHERE Name = 'BonusSystem.NotificationMethod';
	END
	
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsBonus.Index.SmsNotificationEnabled', 'СМС уведомления';
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsBonus.Index.SmsNotificationEnabled', 'SMS notification';

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsBonus.Index.EmailNotificationEnabled', 'Email уведомления';
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsBonus.Index.EmailNotificationEnabled', 'Email notification';

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsBonus.Index.PushNotificationEnabled', 'Push уведомления';
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsBonus.Index.PushNotificationEnabled', 'Push notification';

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsBonus.Index.PushNotificationEnabled.Help', 'Push уведомление будет отправлено только при наличии у клиента установленного мобильного приложения и разрешения на получение Push уведомлений. <br>К Push уведомлению рекомендуем дублировать сообщение на Email.';
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsBonus.Index.PushNotificationEnabled.Help', 'A push notification will be sent only if the client has an installed mobile application and permission to receive Push notifications. <br>For Push notification, we recommend duplicating the message by Email.';

GO--

UPDATE [Settings].[MailFormat] SET [FormatText] = '
<div style="background: url(#LINK#/images/giftcertificate/giftcertificatbg-new2.png) no-repeat; background-size: 100% 100%; width: 711px; position: relative; min-height: 335px;border-radius: 28px;border: 1px solid #00000057">
        <table style="padding:0; margin:0; border:0; border-collapse:collapse; width:100%;">
            <tbody>
                <tr>
                    <td style="border:0; border-collapse:collapse;">
                        <div style="padding: 30px; max-width: 350px; height: auto; vertical-align: middle; display: table-cell;">#LOGO#</td>
                    </td>
                </tr>
                <tr>
                    <td style="padding-left: 30px;border:0; border-collapse:collapse;">
                        <h2 style="font-size: 60px;color: rgb(255, 41, 41);margin: 0;line-height: 1.1; margin-left: -5px;font-family:Circe,monospace, sans-serif;">Подарочный</h2>
                        <p style="font-size: 36px;color: rgb(83, 88, 93);margin: 0;line-height: 1.1;font-family:Circe,monospace, sans-serif;">сертификат</p>
                    </td>
                    <td style="padding-left:80px;border:0; border-collapse:collapse;">
                        <div style=""><img style="width: 230px;" src="#LINK#/images/giftcertificate/bow2.png"></div>
                    </td>
                    <td style="border:0; border-collapse:collapse;">
                        <div style="font-size: 16px;color: rgb(255, 255, 255);writing-mode: vertical-lr;width: 220px;;font-family:Circe,monospace, sans-serif;padding-left: 10px;">
                            <span>Код:</span>
                            <span style="margin-left: 5px;">#CODE#</span>
                        </div>
                    </td>
                </tr>
                <tr>
                    <td style="border:0; border-collapse:collapse;">
                        <div style="padding-left: 30px;max-width: 400px;">
                            <span style="margin-right: 10px;font-size: 14px;">
                                <span style="color: rgba(83, 88, 93, 0.6);font-family:Circe,monospace, sans-serif;">Кому:</span>
                                <span>#TONAME#</span>
                            </span>
                            <span style="font-size: 14px;">
                                <span style="color: rgba(83, 88, 93, 0.6);font-family:Circe,monospace, sans-serif;">От:</span>
                                <span>#FROMNAME#</span>
                            </span>
                        </div>
                    </td>
                </tr>
                <tr>
                    <td style="border:0; border-collapse:collapse;">
                        <div style="padding-left: 30px;margin-top: 15px;max-width: 400px;">
                           <div style="color:rgba(83, 88, 93, 0.6);font-family:Circe,monospace, sans-serif;font-size: 14px;">#MESSAGE#</div>
                        </div>
                    </td>
                </tr>
                <tr>
                    <td style="border:0; border-collapse:collapse;">
                        <div style="padding-left: 30px;margin-top: 15px;max-width: 400px;padding-bottom: 30px;">
                           <div style="color: rgba(83, 88, 93, 0.6);font-family:Circe,monospace, sans-serif;font-size: 14px;">Данный подарочный сертификат может быть использован на сайте <a href="#LINK#" style="color: rgba(83, 88, 93, 0.6);">#LINK#</a></div>
                        </div>
                    </td>
                    <td style="padding-left: 135px;padding-bottom: 30px;border:0; border-collapse:collapse;">
                        <div style="text-align: center;line-height:100px;width: 100px;height: 100px;background: #FF2728;border: 10px solid #D30713;border-radius: 50%;box-shadow: 0px 0px 7px rgba(0, 0, 0, 0.71);">
                            <div style="font-size: 24px;color: rgb(255, 255, 255);font-weight: bold;">
                                #SUM#
                            </div>
                        </div>
                    </td>
                </tr>
            </tbody>
        </table>
    </div>
' WHERE [FormatName] = 'Подарочный сертификат'

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Checkout.IsShowDontCallBack', 'Выводить галку "Не перезванивать"';
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Checkout.IsShowDontCallBack', 'Display the "Do not call back" checkbox';

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Checkout.IsShowDontCallBack.Help', 'В оформлении заказа будет выведена галка "Не перезванивать"';
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Checkout.IsShowDontCallBack.Help', 'When placing an order, the "Do not call back" checkbox will be displayed';

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Checkout.CheckoutComment.DontCallBack', 'Не перезванивать';
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Checkout.CheckoutComment.DontCallBack', 'Do not call back';

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Checkout.CheckoutComment.DontCallBack.Help', 'Перезвоним только в случае изменений условий заказа';
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Checkout.CheckoutComment.DontCallBack.Help', 'We will call you back only in case of changes in the terms of the order';

IF NOT EXISTS(SELECT 1
              FROM sys.columns
              WHERE (name = N'DontCallBack') AND object_id = OBJECT_ID(N'[Order].[Order]'))
    BEGIN
        ALTER TABLE [Order].[Order]
			ADD DontCallBack BIT NULL
	END

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.AnalyticsReport.OnlyPaid', 'Только оплаченные';
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.AnalyticsReport.OnlyPaid', 'Only paid';

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Coupon.CouponType.FixedOnGiftProduct', 'Фиксированный на подарочный товар';
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Coupon.CouponType.FixedOnGiftProduct', 'Fixed on a gift item';

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.AddEditCoupon.GiftProduct', 'Подарочный товар';
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.AddEditCoupon.GiftProduct', 'Gift product';

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Catalog].[CouponOffers]') AND type in (N'U'))
BEGIN
	CREATE TABLE [Catalog].[CouponOffers](
	[CouponID] [int] NOT NULL,
	[OfferID] [int] NOT NULL,
	CONSTRAINT [PK_Catalog.CouponOffers] PRIMARY KEY CLUSTERED 
	(
		[CouponID] ASC,
		[OfferID] ASC
	) WITH( PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]
	
	ALTER TABLE [Catalog].[CouponOffers]  WITH CHECK ADD  CONSTRAINT [FK_CouponOffers_Coupon] FOREIGN KEY([CouponID])
	REFERENCES [Catalog].[Coupon] ([CouponID]) ON DELETE CASCADE

	ALTER TABLE [Catalog].[CouponOffers]  WITH CHECK ADD  CONSTRAINT [FK_CouponOffers_Offer] FOREIGN KEY([OfferID])
	REFERENCES [Catalog].[Offer] ([OfferID]) ON DELETE CASCADE
END

IF NOT EXISTS(SELECT 1
              FROM sys.columns
              WHERE (name = N'IsForbiddenChangeAmount') AND object_id = OBJECT_ID(N'[Catalog].[ShoppingCart]'))
    BEGIN
        ALTER TABLE [Catalog].[ShoppingCart]
			ADD IsForbiddenChangeAmount BIT NULL
	END

GO--

declare @Russia int = (SELECT TOP 1 [CountryID] FROM [Customers].[Country] WHERE [CountryISO3] = 'RUS')

IF (@Russia IS NOT NULL)
BEGIN
	declare @MORegionId int = (SELECT TOP 1 [RegionID] FROM [Customers].[Region] WHERE [RegionName] = 'Московская область' AND [CountryID] = @Russia)
	declare @MoscowskiId int = (SELECT TOP 1 [CityID] FROM [Customers].[City] WHERE [CityName] = 'Московский' AND [RegionID] = @MORegionId)
	declare @MoscowRegionId int = (SELECT TOP 1 [RegionID] FROM [Customers].[Region] WHERE [RegionName] = 'Москва' AND [CountryID] = @Russia)

	IF (@MoscowskiId IS NOT NULL AND @MoscowRegionId IS NOT NULL)
	BEGIN
		UPDATE [Customers].[City]
		   SET [RegionID] = @MoscowRegionId
		WHERE [CityID] = @MoscowskiId
	END
END

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.ShippingMethods.BoxBerry.ChooseShippingMethod', 'Выберите методы доставки';
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.ShippingMethods.BoxBerry.ChooseShippingMethod', 'Choose shipping methods';

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Common.ProductNotActive', 'Товар не активен';
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Common.ProductNotActive', 'The product is not active';

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.AddEditCity.CityDescription', 'Описание';
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.AddEditCity.CityDescription', 'Description';

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Common.ShippingsPage.Meta.Title', 'Зоны доставки';
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Common.ShippingsPage.Meta.Title', 'Delivery zones';

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Common.ShippingsPage.DeliveryZonesNotAvailable', 'Для вашего города отсутствуют зоны доставки';
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Common.ShippingsPage.DeliveryZonesNotAvailable', 'There are no delivery zones for your city';

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Common.ShippingsPage.Header', 'Зоны доставки';
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Common.ShippingsPage.Header', 'Delivery zones';

GO--

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Необходимо разрешить текущему сайту показывать оповещения в настройках браузера <br><br><a href="https://www.advantshop.net/help/pages/kak-vkliuchit-ili-otkliuchit-uvedomleniya" target="_blank" class="link-academy link-invert inline-flex"><svg xmlns=\"http://www.w3.org/2000/svg\" xmlns:xlink="http://www.w3.org/1999/xlink" version="1.1" id="Layer_1" x="0px" y="0px" viewBox="0 0 512 512" style="enable-background:new 0 0 512 512;" xml:space="preserve" width="18" height="18" fill="#989a9b" class="m-r-xs flex-shrink-n"><path d="M20.005,512c-5.097,0-10.116-1.948-13.925-5.641c-5.845-5.666-7.672-14.309-4.621-21.855l45.411-112.333  C16.162,332.253,0,285.425,0,236c0-63.375,26.855-122.857,75.62-167.489C123.891,24.331,187.952,0,256,0  s132.109,24.331,180.38,68.511C485.145,113.143,512,172.625,512,236c0,45.448-14.04,89.577-40.602,127.615  c-6.325,9.057-18.792,11.271-27.849,4.947s-11.271-18.792-4.947-27.849C460.452,309.425,472,273.215,472,236  c0-108.075-96.897-196-216-196S40,127.925,40,236c0,43.783,15.577,85.2,45.046,119.773c4.834,5.671,6.115,13.561,3.321,20.47  l-31.366,77.589l91.345-40.266c5.063-2.231,10.826-2.267,15.916-0.095C193.082,425.766,223.946,432,256,432  c36.892,0,73.299-8.587,105.286-24.832c9.85-5,21.887-1.072,26.889,8.775c5.001,9.849,1.073,21.887-8.775,26.889  C341.828,461.914,299.157,472,256,472c-34.48,0-67.835-6.191-99.276-18.413L28.068,510.301C25.474,511.444,22.728,512,20.005,512z   M276,325V217c0-11.046-8.954-20-20-20s-20,8.954-20,20v108c0,11.046,8.954,20,20,20S276,336.046,276,325z M256,128  c-11.046,0-20,8.954-20,20l0,0c0,11.046,8.954,20,20,20s20-8.954,20-20l0,0C276,136.954,267.046,128,256,128z"></path></svg><span>Инструкция. Как разрешить доступ и уведомления</span></a>' WHERE [ResourceKey] = 'Admin.Js.AdminWebNotifications.AllowSiteShowNotifications' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'You must allow the current site to show notifications in the browser settings <br><br><a href="https://www.advantshop.net/help/pages/kak-vkliuchit-ili-otkliuchit-uvedomleniya" target="_blank" class="link-academy link-invert inline-flex"><svg xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink" version="1.1" id="Layer_1" x="0px" y="0px" viewBox="0 0 512 512" style="enable-background:new 0 0 512 512;" xml:space="preserve" width="18" height="18" fill="#989a9b" class="m-r-xs flex-shrink-n"><path d="M20.005,512c-5.097,0-10.116-1.948-13.925-5.641c-5.845-5.666-7.672-14.309-4.621-21.855l45.411-112.333  C16.162,332.253,0,285.425,0,236c0-63.375,26.855-122.857,75.62-167.489C123.891,24.331,187.952,0,256,0  s132.109,24.331,180.38,68.511C485.145,113.143,512,172.625,512,236c0,45.448-14.04,89.577-40.602,127.615  c-6.325,9.057-18.792,11.271-27.849,4.947s-11.271-18.792-4.947-27.849C460.452,309.425,472,273.215,472,236  c0-108.075-96.897-196-216-196S40,127.925,40,236c0,43.783,15.577,85.2,45.046,119.773c4.834,5.671,6.115,13.561,3.321,20.47  l-31.366,77.589l91.345-40.266c5.063-2.231,10.826-2.267,15.916-0.095C193.082,425.766,223.946,432,256,432  c36.892,0,73.299-8.587,105.286-24.832c9.85-5,21.887-1.072,26.889,8.775c5.001,9.849,1.073,21.887-8.775,26.889  C341.828,461.914,299.157,472,256,472c-34.48,0-67.835-6.191-99.276-18.413L28.068,510.301C25.474,511.444,22.728,512,20.005,512z   M276,325V217c0-11.046-8.954-20-20-20s-20,8.954-20,20v108c0,11.046,8.954,20,20,20S276,336.046,276,325z M256,128  c-11.046,0-20,8.954-20,20l0,0c0,11.046,8.954,20,20,20s20-8.954,20-20l0,0C276,136.954,267.046,128,256,128z"></path></svg><span>Instruction. How to allow access and notifications</span></a>' WHERE [ResourceKey] = 'Admin.Js.AdminWebNotifications.AllowSiteShowNotifications' AND [LanguageId] = 2

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Customer.PushSended', 'Уведомление отправлено';
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Customer.PushSended', 'Push sended';

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Customer.ErrorWhileSending', 'Ошибка при отправке уведомления';
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Customer.ErrorWhileSending', 'Error sending notification';

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Customers.View.MobileAppNotification.Title', 'Заголовок ';
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Customers.View.MobileAppNotification.Title', 'Title';

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Customers.View.MobileAppNotification.Body', 'Сообщение';
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Customers.View.MobileAppNotification.Body', 'Body';

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Customers.View.MobileAppNotification.SendNotification', 'Отправить уведомление';
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Customers.View.MobileAppNotification.SendNotification', 'Send notification';

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Customers.View.MobileAppNotification.SendNotification', 'Отправить уведомление';
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Customers.View.MobileAppNotification.SendNotification', 'Send notification';

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Customers.View.MobileAppNotification.Send', 'Отправить';
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Customers.View.MobileAppNotification.Send', 'Send';

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Customers.View.MobileAppNotification.Cancel', 'Отмена';
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Customers.View.MobileAppNotification.Cancel', 'Cancel';

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Configuration.TemplateSettings_SearchTopPanel', 'Верхняя панель';
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Configuration.TemplateSettings_SearchTopPanel', 'Top panel';

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Orders.OrderCustomer.Address', 'Адрес';
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Orders.OrderCustomer.Address', 'Address';

GO--

UPDATE [Settings].[SettingsSearch] SET [Link] = 'settingstemplate?settingsTemplateTab=product' WHERE Link = 'settingstemplate?settingsTab=product'

GO--

IF NOT EXISTS(SELECT 1 FROM sys.columns WHERE Name = N'ModuleStringId' AND Object_ID = Object_ID(N'Order.PaymentMethod'))
BEGIN
	ALTER TABLE [Order].[PaymentMethod] ADD
		ModuleStringId nvarchar(150) NULL
END

GO-- 

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.AddPaymentMethod.For', 'за ';
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.AddPaymentMethod.For', 'for ';
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.ShippingMethods.For', 'за ';
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.ShippingMethods.For', 'for ';


GO-- 

UPDATE [CMS].[StaticBlock]
SET [Content] = REPLACE([Content], 'Нажмите сюда, чтобы добавить описание', '')
WHERE [Content] like '%Нажмите сюда, чтобы добавить описание%'

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Template.ShowProductsPhotoInCheckoutCartNote', 'Опция определяет, отображать ли фото товара в блоке корзины (состав заказа) на странице оформления заказов';
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Template.ShowProductsPhotoInCheckoutCartNote', 'The option determines whether to display the product photo in the shopping cart block (order contents) on the checkout page';

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Template.ShowProductsPhotoInCheckoutCart', 'Отображать фото товара на странице оформления заказов';
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Template.ShowProductsPhotoInCheckoutCart', 'Show product photo on checkout page';

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.Spinbox.MaxTextNote', 'Должно быть не более';
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.Spinbox.MaxTextNote', 'Should be no more';

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.Spinbox.MinTextNote', 'Должно быть не менее';
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.Spinbox.MinTextNote', 'Must be at least';

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.Spinbox.MultiplicityTextNote', 'Должно быть кратно';
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.Spinbox.MultiplicityTextNote', 'Must be a multiple';

GO--

ALTER TABLE Catalog.Coupon ADD
	OnlyInMobileApp bit NULL
GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.AddEditCoupon.OnlyInMobileApp', 'Доступен только в мобильном приложении';
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.AddEditCoupon.OnlyInMobileApp', 'Available only in the mobile app';

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Coupon.CouponPost.CouponOnlyInMobileApp', 'Купон может быть применен только в мобильном приложении';
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Coupon.CouponPost.CouponOnlyInMobileApp', 'Coupon can be applied only in the mobile app';

GO--

ALTER PROCEDURE [Catalog].[sp_GetOptionsByCustomOptionId]
	-- Add the parameters for the stored procedure here
	@CustomOptionId int,
	@Type nvarchar(50)
AS
BEGIN
	SELECT [OptionID], [Options].[CustomOptionsId], [Options].[Title], [PriceBC], [PriceType], [Options].[SortOrder], CurrencyValue, [Photo].[PhotoName]
  FROM [Catalog].[Options]
  INNER JOIN [Catalog].[CustomOptions] on [CustomOptions].[CustomOptionsId] = [Options].[CustomOptionsId]
	INNER JOIN [Catalog].[Product] on [Product].ProductId = [CustomOptions].ProductId
	INNER JOIN [Catalog].[Currency] on [Product].CurrencyID = [Currency].CurrencyID
	LEFT JOIN [Catalog].[Photo] ON [Photo].[ObjId] = [Options].[OptionID] and [Type]=@Type
  WHERE [CustomOptions].CustomOptionsId = @CustomOptionId
  order by [SortOrder]
END

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.ExportImport.EProductFields.FullUrl', 'Полный URL';
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.ExportImport.EProductFields.FullUrl', 'Full URL';

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.ExportImport.ProductFields.FullUrl', 'Полный URL';
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.ExportImport.ProductFields.FullUrl', 'Full URL';

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Product.LeftMenu.AdditionalOptions', 'Доп. опции';

GO--

UPDATE [Settings].[SettingsSearch] SET [Link] = 'staticpages?staticPageTab=pages' WHERE Link = 'staticpages'

UPDATE [Settings].[SettingsSearch] SET [Link] = 'staticpages?staticPageTab=blocks' WHERE Link = 'staticblock'

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.Builder.ShowProductsPhotoInCheckoutCartNote', 'Опция определяет, отображать ли фото товара в блоке корзины (состав заказа) на странице оформления заказов';
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.Builder.ShowProductsPhotoInCheckoutCartNote', 'The option determines whether to display the product photo in the shopping cart block (order contents) on the checkout page';

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.Builder.ShowProductsPhotoInCheckoutCart', 'Отображать фото товара на странице оформления заказов';
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.Builder.ShowProductsPhotoInCheckoutCart', 'Show product photo on checkout page';

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Template.CustomOptions', 'Дополнительные опции товара. Фотография'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Template.CustomOptions', 'Product custom options. Photo' 

GO--

IF NOT EXISTS(SELECT 1
              FROM sys.columns
              WHERE (name = N'IsByCoupon') AND object_id = OBJECT_ID(N'[Catalog].[ShoppingCart]'))
    BEGIN
        ALTER TABLE [Catalog].[ShoppingCart]
			ADD IsByCoupon BIT NULL
	END

IF NOT EXISTS(SELECT 1
              FROM sys.columns
              WHERE (name = N'IsByCoupon') AND object_id = OBJECT_ID(N'[Order].[OrderItems]'))
    BEGIN
        ALTER TABLE [Order].[OrderItems]
			ADD IsByCoupon BIT NULL
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
	 @TypeItem nvarchar(50),
	 @IsMarkingRequired bit,
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
	   ,IsMarkingRequired
	   ,IsCustomPrice
	   ,BasePrice
	   ,DiscountPercent
	   ,DiscountAmount
       ,DoNotApplyOtherDiscounts
	   ,[Unit]
	   ,[MeasureType]
	   ,[IsByCoupon]
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
	   ,@IsMarkingRequired
	   ,@IsCustomPrice
	   ,@BasePrice
	   ,@DiscountPercent
	   ,@DiscountAmount
       ,@DoNotApplyOtherDiscounts
       ,@Unit
       ,@MeasureType
	   ,@IsByCoupon
   );  
       
 SELECT SCOPE_IDENTITY()  
END  

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
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.Builder.MobileAppBannerVisibility', 'Баннер приложения'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.Builder.MobileAppBannerVisibility', 'Application banner'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.Builder.MobileAppBannerVisibilityTitle', 'Баннер приложения'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.Builder.MobileAppBannerVisibilityTitle', 'Application banner' 

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Template.MobileAppBannerVisibilityTitle', 'Баннер приложения'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Template.MobileAppBannerVisibilityTitle', 'Application banner' 

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Template.MobileAppBannerVisibilityNote', 'Баннер выводиться вверху мобильного сайта, для быстрой установки или открытие мобильного приложения. Требуется установленное мобильное приложение.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Template.MobileAppBannerVisibilityNote', 'The banner is displayed at the top of the mobile site, for quick installation or opening a mobile application. Mobile app required.' 

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.Builder.MobileAppBannerVisibilityNote', 'Баннер выводиться вверху мобильного сайта, для быстрой установки или открытие мобильного приложения. Требуется установленное мобильное приложение.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.Builder.MobileAppBannerVisibilityNote', 'The banner is displayed at the top of the mobile site, for quick installation or opening a mobile application. Mobile app required.' 

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Taskgroups.Taskgroups.AreYouSureCopy', 'Скопировать проект {{name}}?';
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Taskgroups.Taskgroups.AreYouSureCopy', 'Copy the {{name}} project?';
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Taskgroups.Taskgroups.Copy', 'Копирование';
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Taskgroups.Taskgroups.Copy', 'Copying';

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.ShippingWithInterval.Settings.CountHiddenDeliveryDay', 'Количество дней через которые можно оформить заказ'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.ShippingWithInterval.Settings.CountHiddenDeliveryDay', 'The number of days after which you can place an order' 

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.ShippingWithInterval.Settings.CountHiddenDeliveryDay.Help', 'Первая дата для выбора будет не раньше, чем через указанное количество дней от даты оформления заказа'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.ShippingWithInterval.Settings.CountHiddenDeliveryDay.Help', 'The first date to select will not be earlier than the specified number of days from the date of placing the order' 

GO--

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Shipping].[LPostPickPoints]') AND type in (N'U'))
BEGIN
	CREATE TABLE [Shipping].[LPostPickPoints](
		[Id] [int] NOT NULL,
		[City] [nvarchar](255) NOT NULL,
		[RegionCode] [int] NOT NULL,
		[Address] [nvarchar](255) NULL,
		[AddressDescription] [nvarchar](max) NULL,
		[Latitude] [float] NOT NULL,
		[Longitude] [float] NOT NULL,
		[Cash] [bit] NOT NULL,
		[Card] [bit] NOT NULL,
		[IsCourier] [bit] NOT NULL,
		[LastUpdate] [datetime] NOT NULL
	 CONSTRAINT [PK_LPostPickPoints] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH( PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END

GO--


EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.SettingsTemplate.Additional', 'Дополнительные настройки шаблона'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.SettingsTemplate.Additional', 'Additional template settings' 

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.ShippingMethods.Measoft.DeliveryServices', 'Режимы доставки'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.ShippingMethods.Measoft.DeliveryServices', 'Delivery services' 

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.AddEditReview.ShowOnMain', 'Отображать на главной'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.AddEditReview.ShowOnMain', 'Show on main' 

GO--

ALTER TABLE [CMS].[Review] ADD [ShowOnMain] [bit] NULL

GO--

UPDATE [CMS].[Review] SET [ShowOnMain] = 0

GO--

ALTER TABLE[CMS].[Review] ALTER COLUMN [ShowOnMain] bit NOT NULL

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Product.ProductReviewsOnMain.Header', 'Отзывы о товарах'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Product.ProductReviewsOnMain.Header', 'Product reviews'

GO--

IF EXISTS(SELECT 1
          FROM [Settings].[Settings]
          WHERE [Name] = 'RelatedProductSourceType')
    AND NOT EXISTS(SELECT 1
                   FROM [Settings].[Settings]
                   WHERE [Name] = 'SimilarProductSourceType')
    BEGIN
        INSERT INTO [Settings].[Settings] (Name, Value)
        VALUES ('SimilarProductSourceType',
                (SELECT TOP 1 Value FROM [Settings].[Settings] WHERE [Name] = 'RelatedProductSourceType'))
    END
    
GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Catalog.ShowMarketplaceButton', 'Выводить кнопки покупки товара на маркетплейсах'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Catalog.ShowMarketplaceButton', 'Display product purchase buttons on marketplaces'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Catalog.OptionShowMarketplaceButton', 'В карточке товара будут выводиться кнопки, со ссылкой на товар из маркетплейса'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Catalog.OptionShowMarketplaceButton', 'The product card will display buttons with a link to the product from the marketplace'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Product.RightBlock.Marketplace.Header', 'Размещен на'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Product.RightBlock.Marketplace.PostedOn', 'Posted on'

GO--

IF NOT EXISTS(SELECT 1
              FROM sys.columns
              WHERE (name = N'ShippingMethodId') AND object_id = OBJECT_ID(N'[Catalog].[PriceRule]'))
    BEGIN
        ALTER TABLE [Catalog].[PriceRule]
			ADD [ShippingMethodId] INT NULL
	END

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.OrderConfirmation.DiscountByShippingRule', 'Скидка при доставке данным способом {0}'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.OrderConfirmation.DiscountByShippingRule', 'Discount {0}'

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Customers.ShoppingCart.ArtNo', 'Артикул'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Customers.ShoppingCart.ArtNo', 'Sku'

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'User.Authorization.Phone', 'Номер телефона'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'User.Authorization.Phone', 'Phone number'

GO--

UPDATE [Settings].[SettingsSearch] SET [Link] = 'settingsapi?apiTab=1c' WHERE Link = 'settingsapi' AND Title = '1C'

DECLARE @keyWords nvarchar(1000) = (SELECT TOP(1) KeyWords FROM [Settings].[SettingsSearch] WHERE Link = 'settingsapi' AND Title = 'API')
IF @keyWords = '' OR @keyWords IS NULL
	SET @keyWords = N'апи';
ELSE
	SET @keyWords = @keyWords + N', апи'
UPDATE [Settings].[SettingsSearch] SET [Link] = 'settingsapi?apiTab=api', KeyWords = @keyWords  WHERE Link = 'settingsapi' AND Title = 'API'
UPDATE [Settings].[SettingsSearch] SET [Link] = 'settingstasks?tasksTab=tasksBusinessProcesses' WHERE Link = 'settingstasks?tasksTab=tasks' AND Title = 'Бизнес-процессы'
UPDATE [Settings].[SettingsSearch] SET [Link] = 'settingstemplate?settingsTab=csseditor&settingsTemplateTab=cssEditor' WHERE Link = 'settingstemplate?settingsTab=csseditor' AND Title = 'Дополнительные стили'
UPDATE [Settings].[SettingsSearch] SET [Link] = 'exportfeeds/indexreseller' WHERE Link = 'exportfeeds/indexcsv' AND Title = 'Дропшипперы'
UPDATE [Settings].[SettingsSearch] SET [Link] = 'settingstemplate?settingsTemplateTab=brands' WHERE Link = 'settingstemplate?settingsTab=brands' AND Title = 'Настройки производителей'
UPDATE [Settings].[SettingsSearch] SET [Link] = 'home/desktop', [KeyWords] = REPLACE([KeyWords], ', дашборд', '') WHERE Link = 'home' AND Title = 'Рабочий стол'
UPDATE [Settings].[SettingsSearch] SET [Link] = 'settingscatalog?catalogTab=additional' WHERE Link = 'settingscatalog?catalogTab=prices' AND Title = 'Режим отображения цен'

SET @keyWords = (SELECT TOP(1) KeyWords FROM [Settings].[SettingsSearch] WHERE Link = 'settingstemplate?settingsTab=news' AND Title = 'Настройки новостей')
IF @keyWords = '' OR @keyWords IS NULL
	SET @keyWords = N'Новости';
ELSE
	SET @keyWords = @keyWords + N', Новости'
UPDATE [Settings].[SettingsSearch] SET [Link] = 'settingstemplate?settingsTemplateTab=news', [KeyWords] = @keyWords WHERE Link = 'settingstemplate?settingsTab=news' AND Title = 'Настройки новостей'

SET @keyWords = (SELECT TOP(1) KeyWords FROM [Settings].[SettingsSearch] WHERE Link = 'smstemplates/smslog' AND Title = 'Смс история бонусов')
IF @keyWords = '' OR @keyWords IS NULL
	SET @keyWords = N'История уведомлений';
ELSE
	SET @keyWords = @keyWords + N', История уведомлений'
UPDATE [Settings].[SettingsSearch] SET [Link] = 'notificationtemplates/notificationlog', [KeyWords] = @keyWords WHERE Link = 'smstemplates/smslog' AND Title = 'Смс история бонусов'

SET @keyWords = (SELECT TOP(1) KeyWords FROM [Settings].[SettingsSearch] WHERE Link = 'smstemplates' AND Title = 'Смс шаблоны бонусов')
IF @keyWords = '' OR @keyWords IS NULL
	SET @keyWords = N'Шаблоны уведомлений';
ELSE
	SET @keyWords = @keyWords + N', Шаблоны уведомлений'
UPDATE [Settings].[SettingsSearch] SET [Link] = 'notificationtemplates', [KeyWords] = @keyWords WHERE Link = 'smstemplates' AND Title = 'Смс шаблоны бонусов'
UPDATE [Settings].[SettingsSearch] SET [Link] = 'settingssocial?socialTab=1' WHERE Link = 'settingssocial' AND Title = 'Социальные сети'

SET @keyWords = (SELECT TOP(1) KeyWords FROM [Settings].[SettingsSearch] WHERE Link = 'settingstemplate?settingsTab=catalog' AND Title = 'Фильтры')
IF @keyWords = '' OR @keyWords IS NULL
	SET @keyWords = N'отображение фильтров';
ELSE
	SET @keyWords = @keyWords + N', отображение фильтров'
UPDATE [Settings].[SettingsSearch] SET [Link] = 'settingstemplate?settingsTemplateTab=catalog', [KeyWords] = @keyWords WHERE Link = 'settingstemplate?settingsTab=catalog' AND Title = 'Фильтры'

UPDATE [Settings].[SettingsSearch] SET [Link] = 'dashboard' WHERE Link = 'funnels' AND Title = 'Воронки продаж'

SET @keyWords = (SELECT TOP(1) KeyWords FROM [Settings].[SettingsSearch] WHERE Link = 'settingsseo?seoTab=seoGA' AND Title = 'Google Analytics')
IF @keyWords = '' OR @keyWords IS NULL
	SET @keyWords = N'гугл аналитика';
ELSE IF NOT @keyWords LIKE '%гугл аналитика%'
	SET @keyWords = @keyWords + N', гугл аналитика'
UPDATE [Settings].[SettingsSearch] SET [KeyWords] = @keyWords WHERE Link = 'settingsseo?seoTab=seoGA' AND Title = 'Google Analytics'

SET @keyWords = (SELECT TOP(1) KeyWords FROM [Settings].[SettingsSearch] WHERE Link = 'settingsseo?seoTab=seoParams' AND Title = 'SEO параметры')
IF @keyWords = '' OR @keyWords IS NULL
	SET @keyWords = N'сео, мета';
ELSE IF NOT @keyWords LIKE '%сео, мета%'
	SET @keyWords = @keyWords + N', сео, мета'
UPDATE [Settings].[SettingsSearch] SET [KeyWords] = @keyWords WHERE Link = 'settingsseo?seoTab=seoParams' AND Title = 'SEO параметры'

SET @keyWords = (SELECT TOP(1) KeyWords FROM [Settings].[SettingsSearch] WHERE Link = 'leads' AND Title = 'Лиды')
IF @keyWords = '' OR @keyWords IS NULL
	SET @keyWords = N'заявки';
ELSE IF NOT @keyWords LIKE '%заявки%'
	SET @keyWords = @keyWords + N', заявки'
UPDATE [Settings].[SettingsSearch] SET [KeyWords] = @keyWords WHERE Link = 'leads' AND Title = 'Лиды'

SET @keyWords = (SELECT TOP(1) KeyWords FROM [Settings].[SettingsSearch] WHERE Link = 'settingstemplate?settingsTab=common' AND Title = 'Настройки дизайна')
IF @keyWords = '' OR @keyWords IS NULL
	SET @keyWords = N'витрина магазина';
ELSE IF NOT @keyWords LIKE '%витрина магазина%'
	SET @keyWords = @keyWords + N', витрина магазина'
UPDATE [Settings].[SettingsSearch] SET [KeyWords] = @keyWords WHERE Link = 'settingstemplate?settingsTab=common' AND Title = 'Настройки дизайна'

SET @keyWords = (SELECT TOP(1) KeyWords FROM [Settings].[SettingsSearch] WHERE Link = 'analytics?analyticsReportTab=exportOrders' AND Title = 'Экспорт заказов')
IF @keyWords = '' OR @keyWords IS NULL
	SET @keyWords = N'выгрузка заказов';
ELSE IF NOT @keyWords LIKE '%выгрузка заказов%'
	SET @keyWords = @keyWords + N', выгрузка заказов'
UPDATE [Settings].[SettingsSearch] SET [KeyWords] = @keyWords WHERE Link = 'analytics?analyticsReportTab=exportOrders' AND Title = 'Экспорт заказов'

SET @keyWords = (SELECT TOP(1) KeyWords FROM [Settings].[SettingsSearch] WHERE Link = 'settingssystem?systemTab=system' AND Title = 'Каптча')
IF @keyWords = '' OR @keyWords IS NULL
	SET @keyWords = N'каптча';
ELSE
	SET @keyWords = @keyWords + N', каптча'
UPDATE [Settings].[SettingsSearch] SET [Title] = 'Капча', [KeyWords] = @keyWords WHERE Link = 'settingssystem?systemTab=system' AND Title = 'Каптча'

SET @keyWords = (SELECT TOP(1) KeyWords FROM [Settings].[SettingsSearch] WHERE Link = 'settingsmail?notifyTab=sms' AND Title = 'sms-уведомления')
IF @keyWords = '' OR @keyWords IS NULL
	SET @keyWords = N'смс';
ELSE IF NOT @keyWords LIKE '%смс%'
	SET @keyWords = @keyWords + N', смс'
UPDATE [Settings].[SettingsSearch] SET [KeyWords] = @keyWords WHERE Link = 'settingsmail?notifyTab=sms' AND Title = 'sms-уведомления'

DELETE FROM [Settings].[SettingsSearch] WHERE Link = 'settingstelephony?telephonyTab=smartcalls' AND Title = 'Автозвонки'

IF NOT EXISTS (SELECT TOP(1) 1 FROM [Settings].[SettingsSearch] WHERE Title = 'Социальные виджеты' AND Link = 'settingssocial?socialTab=2')
	INSERT INTO [Settings].[SettingsSearch] (Title, Link, SortOrder)
		VALUES ('Социальные виджеты', 'settingssocial?socialTab=2', 0)
	
IF NOT EXISTS (SELECT TOP(1) 1 FROM [Settings].[SettingsSearch] WHERE Title = 'Аналитика по лидам' AND Link = 'leads?viewmode=analytics')
	INSERT INTO [Settings].[SettingsSearch] (Title, Link, SortOrder, KeyWords)
		VALUES ('Аналитика по лидам', 'leads?viewmode=analytics', 0, 'лиды')

IF NOT EXISTS (SELECT TOP(1) 1 FROM [Settings].[SettingsSearch] WHERE Title = 'Настройки лидов' AND Link = 'settingscrm')
	INSERT INTO [Settings].[SettingsSearch] (Title, Link, SortOrder)
		VALUES ('Настройки лидов', 'settingscrm', 0)

IF NOT EXISTS (SELECT TOP(1) 1 FROM [Settings].[SettingsSearch] WHERE Title = 'Списки лидов' AND Link = 'settingscrm?crmTab=salesfunnels')
	INSERT INTO [Settings].[SettingsSearch] (Title, Link, SortOrder)
		VALUES ('Списки лидов', 'settingscrm?crmTab=salesfunnels', 0)

IF NOT EXISTS (SELECT TOP(1) 1 FROM [Settings].[SettingsSearch] WHERE Title = 'Импорт лидов' AND Link = 'settingscrm?crmTab=importLeads')
	INSERT INTO [Settings].[SettingsSearch] (Title, Link, SortOrder, KeyWords)
		VALUES ('Импорт лидов', 'settingscrm?crmTab=importLeads', 0, 'лиды')

IF NOT EXISTS (SELECT TOP(1) 1 FROM [Settings].[SettingsSearch] WHERE Title = 'Единицы измерения' AND Link = 'settingscatalog?catalogTab=units')
	INSERT INTO [Settings].[SettingsSearch] (Title, Link, SortOrder)
		VALUES ('Единицы измерения', 'settingscatalog?catalogTab=units', 0)

IF NOT EXISTS (SELECT TOP(1) 1 FROM [Settings].[SettingsSearch] WHERE Title = 'Типы цен' AND Link = 'settingscatalog?catalogTab=pricerules')
	INSERT INTO [Settings].[SettingsSearch] (Title, Link, SortOrder, KeyWords)
		VALUES ('Типы цен', 'settingscatalog?catalogTab=pricerules', 0, 'цены')

IF NOT EXISTS (SELECT TOP(1) 1 FROM [Settings].[SettingsSearch] WHERE Title = 'Мои сайты' AND Link = 'dashboard')
	INSERT INTO [Settings].[SettingsSearch] (Title, Link, KeyWords, SortOrder)
		VALUES ('Мои сайты', 'dashboard', 'основной сайт', 0)

IF NOT EXISTS (SELECT TOP(1) 1 FROM [Settings].[SettingsSearch] WHERE Title = 'Настройки отзывов' AND Link = 'settingstemplate?settingsTemplateTab=product')
	INSERT INTO [Settings].[SettingsSearch] (Title, Link, SortOrder, KeyWords)
		VALUES ('Настройки отзывов', 'settingstemplate?settingsTemplateTab=product', 0, 'отзывы')

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.AddPropertyValue.Name', 'Название'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.AddPropertyValue.Name', 'Name'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.PropertyValues.Name', 'Название'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.PropertyValue.Name', 'Name'

IF NOT EXISTS(SELECT 1
              FROM sys.columns
              WHERE (name = N'NameOfValue') AND object_id = OBJECT_ID(N'[Catalog].[PropertyValue]'))
    BEGIN
        ALTER TABLE [Catalog].[PropertyValue]
			ADD [NameOfValue] NVARCHAR(100) NULL
	END

GO--

ALTER PROCEDURE [Catalog].[sp_GetPropertyValuesByPropertyID] @PropertyID INT
AS
BEGIN
	SET NOCOUNT ON;

	SELECT [PropertyValueID]
		,[Property].[PropertyID]
		,[Value]
		,[PropertyValue].[SortOrder]
		,[Property].UseinFilter
		,[Property].UseIndetails
		,[Property].UseInBrief
		,Property.Name AS PropertyName
		,Property.NameDisplayed AS PropertyNameDisplayed
		,Property.SortOrder AS PropertySortOrder
		,Property.Expanded
		,Property.[Type]
		,Property.[Description]
		,GroupId
		,GroupName
		,GroupNameDisplayed
		,GroupSortorder
		,unit
		,[PropertyValue].[NameOfValue]
	FROM [Catalog].[PropertyValue]
	INNER JOIN [Catalog].[Property] ON [Property].[PropertyID] = [PropertyValue].[PropertyID]
	LEFT JOIN [Catalog].PropertyGroup ON PropertyGroup.PropertyGroupID = [Property].GroupID
	WHERE [Property].[PropertyID] = @PropertyID
	order by [PropertyValue].[SortOrder]
END

GO--

ALTER PROCEDURE [Catalog].[sp_GetPropertyValuesByProductID] @ProductID INT  
AS  
BEGIN  
 SET NOCOUNT ON;  
  
 SELECT  
   [PropertyValue].[PropertyValueID]  
  ,[PropertyValue].[PropertyID]  
  ,[PropertyValue].[Value]  
  ,[PropertyValue].[SortOrder]  
  ,[Property].UseinFilter  
  ,[Property].UseIndetails  
  ,[Property].UseInBrief  
  ,[Property].[Name] as PropertyName  
  ,[Property].[NameDisplayed] AS PropertyNameDisplayed
  ,[Property].[SortOrder] as PropertySortOrder  
  ,[Property].[Expanded] as Expanded  
  ,[Property].[Type] as [Type]  
  ,[Property].GroupId as GroupId  
  ,[Property].[Description] as [Description]
  ,GroupName
  ,GroupNameDisplayed  
  ,GroupSortorder
  ,unit
  ,[PropertyValue].[NameOfValue]  
 FROM [Catalog].[PropertyValue]  
 INNER JOIN [Catalog].[ProductPropertyValue] ON [ProductPropertyValue].[PropertyValueID] = [PropertyValue].[PropertyValueID]  
 inner join [Catalog].[Property] on [Property].[PropertyID] = [PropertyValue].[PropertyID]  
 left join Catalog.PropertyGroup on propertyGroup.PropertyGroupID = [Property].GroupID  
 WHERE [ProductID] = @ProductID  
 ORDER BY case when PropertyGroup.GroupSortOrder is null then 1 else 0 end, 
 PropertyGroup.GroupSortOrder,PropertyGroup.GroupName, [Property].[SortOrder], [Property].Name, [PropertyValue].[SortOrder], [PropertyValue].Value  
END

GO--

ALTER PROCEDURE [Catalog].[sp_GetPropertyValueByID] @PropertyValueId INT
AS
  BEGIN
      SELECT [PropertyValueId],
             [Property].[PropertyId],
             [value],
             [PropertyValue].[sortorder],
             [Property].useinfilter,
             [Property].useindetails,
             [Property].useinbrief,
             [Property].Name       AS PropertyName,
			 [Property].NameDisplayed AS PropertyNameDisplayed,
             [Property].SortOrder  AS PropertySortOrder,
             [Property].Expanded,
             [Property].[Type],
			 [Property].[Description],
             GroupId,
             GroupName,
			 GroupNameDisplayed,
             GroupSortOrder,
			 unit,
             [PropertyValue].[NameOfValue]
      FROM   [Catalog].[PropertyValue]
      INNER JOIN [Catalog].[Property] ON [Property].[Propertyid] = [PropertyValue].[PropertyID]
      LEFT JOIN [Catalog].PropertyGroup ON PropertyGroup.PropertyGroupId = [Property].GroupId
      WHERE  [PropertyValue].[PropertyValueId] = @PropertyValueId
  END 

GO--

ALTER PROCEDURE [Catalog].[sp_UpdatePropertyValue]	
	@PropertyValueID int,
    @Value nvarchar(255),
    @SortOrder int,
    @NameOfValue nvarchar(100),
    @RangeValue float
AS
BEGIN
	SET NOCOUNT ON;
	UPDATE [Catalog].[PropertyValue]
	SET [Value] = @Value
       ,[SortOrder] = @SortOrder
       ,[RangeValue] = @RangeValue
	   ,[NameOfValue] = @NameOfValue
 WHERE [PropertyValueID] = @PropertyValueID
END

GO--

ALTER PROCEDURE [Catalog].[sp_AddPropertyValue]
	@Value nvarchar(255),
	@PropertyId int,
	@NameOfValue nvarchar(100),
	@SortOrder int = 0,
	@RangeValue float = 0
AS
BEGIN
	SET NOCOUNT ON;
	 
	INSERT INTO [Catalog].[PropertyValue]
           ([PropertyID],[Value],[SortOrder],[UseInFilter], [UseInDetails], UseInBrief, [RangeValue], [NameOfValue])
			select @PropertyID, @Value, @SortOrder, [UseInFilter], [UseInDetails], UseInBrief, @RangeValue, @NameOfValue from [Catalog].[Property] where  [Property].[PropertyID]=@PropertyId
     SELECT SCOPE_IDENTITY()
END

GO--

INSERT INTO [Settings].[Localization] (ResourceKey, ResourceValue, LanguageId) VALUES
	('Admin.Settings.Mobile.FooterMunuModeAccordion', 'Aккордеон', 1),
	('Admin.Settings.Mobile.FooterMunuModeAccordion', 'Accordion', 2),
	('Admin.Settings.Mobile.FooterMunuModeDeployed', 'Развёрнуто', 1),
	('Admin.Settings.Mobile.FooterMunuModeDeployed', 'Deployed', 2)
	
GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.ConfirmSms.ErrorSendSms', 'Не удалос отправить СМС'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.ConfirmSms.ErrorSendSms', 'Failed to send SMS'

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Checkout.ShoppingCartMode', 'Режим отображения корзины';
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Checkout.ShoppingCartMode', 'Shopping cart mode';

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Checkout.EShoppingCartMode.Compact', 'Компактный';
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Checkout.EShoppingCartMode.Compact', 'Compact';

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Checkout.EShoppingCartMode.Full', 'Полный';
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Checkout.EShoppingCartMode.Full', 'Full';

GO--

INSERT INTO [Settings].[Settings] ([Name], [Value]) VALUES ('ShoppingCartMode', 'Compact')

GO--

Update [CMS].[StaticBlock] Set [Content] = '
<div class="subscribe-block subscribe-block--p-b-n cs-bg-3">
<div class="row subscribe-block-elements-wrap">
<div class="col-xs-7">
<div class="subscribe-block-title">Заказывайте в новом приложении и получайте бонусы</div>

<div class="subscribe-block-icons row middle-xs">
<div class="col-xs-9">
<div class="row middle-xs m-b-md">
<div class="col-xs-4 col-lg-3"><img src="./Templates/Modern2/images/staticblock/appGallery.png" /></div>

<div class="col-xs-4 col-lg-3"><img src="./Templates/Modern2/images/staticblock/appStore.png" /></div>

<div class="col-xs-4 col-lg-3"><img src="./Templates/Modern2/images/staticblock/googlePlay.png" /></div>

<div class="col-xs-4 col-lg-3"><img src="./Templates/Modern2/images/staticblock/ruStore.png" /></div>
</div>
</div>
</div>
</div>

<div class="col-xs-4 subscribe-block-img-with-qr">
<div class="phone"><img src="./Templates/Modern2/images/staticblock/footer-mobile-app-order.png" /></div>

<div class="qr"><img src="./Templates/Modern2/images/staticblock/footer-mobile-app-order-qr-code.png" />
<p>Наведите камеру на QR-код, чтобы скачать</p></div></div></div></div>' Where [Key] = 'Modern2__FooterBanner'

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.MyAccount.AddressList.SetMainSuccess', 'Успешно изменен адрес по-умолчанию';
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.MyAccount.AddressList.SetMainSuccess', 'Successfully changed default address';

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.Address.AreYouSureDelete', 'Вы уверены, что хотите удалить?'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.Address.AreYouSureDelete', 'Are you sure delete?' 

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.Address.Deleting', 'Удаление'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.Address.Deleting', 'Deleting' 

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Customers.View.Addresses', 'Адреса'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Customers.View.Addresses', 'Addresses' 

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.Address.StructureShort', 'Стр./кор.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.Address.StructureShort', 'Structure' 

GO--

IF NOT EXISTS(SELECT 1
              FROM sys.columns
              WHERE (name = N'NotificationRequestParams') AND object_id = OBJECT_ID(N'[CRM].[TriggerAction]'))
    BEGIN
        ALTER TABLE [CRM].[TriggerAction]
			ADD NotificationRequestParams NVARCHAR(MAX) NULL
    END

GO--

IF NOT EXISTS (SELECT 1 FROM [Settings].[Settings] WHERE [Name] = 'IsShowFullAddress')
	INSERT INTO [Settings].[Settings] ([Name], [Value])
		VALUES ('IsShowFullAddress', 'True')

GO--

UPDATE [CMS].[Menu] SET [MenuItemUrlPath] = 'myaccount?#tab=commoninf' WHERE [MenuItemUrlPath] = 'myaccount?tab=commoninf'
UPDATE [CMS].[Menu] SET [MenuItemUrlPath] ='myaccount?#tab=addressbook' WHERE [MenuItemUrlPath]='myaccount?tab=addressbook'
UPDATE [CMS].[Menu] SET [MenuItemUrlPath] ='myaccount?#tab=orderhistory' WHERE [MenuItemUrlPath]='myaccount?tab=orderhistory'
UPDATE [CMS].[Menu] SET [MenuItemUrlPath] ='myaccount?#tab=changepassword' WHERE [MenuItemUrlPath]='myaccount?tab=changepassword'
UPDATE [CMS].[Menu] SET [MenuItemUrlPath] ='myaccount?#tab=wishlist' WHERE [MenuItemUrlPath]='myaccount?tab=wishlist'

GO--

if exists (Select 1 from [Settings].[Settings] Where Name = 'Api_ApiKeyAuth' and ([Value] is null or [Value] = '')) 
begin
	Update [Settings].[Settings] 
	Set [Value] = LOWER( replace( CONVERT(nvarchar(MAX),NEWID()) + CONVERT(nvarchar(MAX),NEWID()), '-', '') )
	Where Name = 'Api_ApiKeyAuth'
end
else if not exists (Select 1 from [Settings].[Settings] Where Name = 'Api_ApiKeyAuth')
begin
	Insert Into [Settings].[Settings] (Name, [Value]) Values ('Api_ApiKeyAuth', LOWER( replace( CONVERT(nvarchar(MAX),NEWID()) + CONVERT(nvarchar(MAX),NEWID()), '-', '') ))
end

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.NotifyEMails.EmailForProductDiscuss', 'E-mail для уведомлений о новых отзывах о товарах и заказах'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.NotifyEMails.EmailForProductDiscuss', 'E-mail for notifications about new product reviews and orders' 

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.NotifyEMails.EmailForProductDiscussHint', 'Укажите E-mail для уведомлений о новых отзывах о товарах и заказах'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.NotifyEMails.EmailForProductDiscussHint', 'Specify an E-mail for notifications about new product reviews and orders' 

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Checkout.PrintOrder.OrderRecipient', 'Получатель заказа:'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Checkout.PrintOrder.OrderRecipient', 'Order recipient:' 

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Checkout.PrintOrder.RecipientFIO', 'ФИО'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Checkout.PrintOrder.RecipientFIO', 'Full name:' 

GO--

ALTER TABLE Catalog.ProductGifts ADD
	OfferId int NULL

GO--

ALTER PROCEDURE [Catalog].[sp_RemoveOffer] 
	@OfferID as int
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @ProductId as int;

	SELECT TOP (1) @ProductId = [ProductID] FROM [Catalog].[Offer] WHERE [OfferID] = @OfferID

	IF @ProductId IS NOT NULL
	BEGIN
		DELETE FROM [Catalog].[Offer] WHERE [OfferID] = @OfferID;

		declare @temp int;
		set @temp=(CASE WHEN EXISTS(select * from [Catalog].[Offer] where [ProductID]=@ProductId and [Main]=1) THEN 1 ELSE 0 END);
		if @temp=0
		BEGIN
			UPDATE TOP (1) [Catalog].[Offer] 
			SET [Main]=1 
			WHERE [ProductID]=@ProductId
		END 

		DELETE FROM [Catalog].[ProductGifts] WHERE GiftOfferId = @OfferID OR OfferId = @OfferID 
	END
END

GO--

UPDATE [Settings].[InternalSettings] SET [settingValue] = '12.0.0' WHERE [settingKey] = 'db_version'
