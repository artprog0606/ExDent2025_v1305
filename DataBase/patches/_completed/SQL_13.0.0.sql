IF EXISTS(SELECT * FROM sys.indexes WHERE name = 'PropertyValue_Value' AND object_id = OBJECT_ID(N'[Catalog].[PropertyValue]'))
	DROP INDEX [PropertyValue_Value] ON [Catalog].[PropertyValue]
IF EXISTS(SELECT * FROM sys.indexes WHERE name = 'PropertyValue_PropertyID' AND object_id = OBJECT_ID(N'[Catalog].[PropertyValue]'))
	DROP INDEX [PropertyValue_PropertyID] ON [Catalog].[PropertyValue]

GO--

ALTER TABLE [Catalog].[PropertyValue] ALTER COLUMN [Value] nvarchar(MAX) NOT NULL

GO--

CREATE NONCLUSTERED INDEX [PropertyValue_PropertyID] ON [Catalog].[PropertyValue]
(
	[PropertyID] ASC
)INCLUDE ([Value]) WITH (PAD_INDEX = OFF, SORT_IN_TEMPDB = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Catalog.PropertyType.Textarea', 'Текстовое поле'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Catalog.PropertyType.Textarea', 'Textarea' 

GO--

ALTER PROCEDURE [Catalog].[sp_UpdatePropertyValue]	
	@PropertyValueID int,
    @Value nvarchar(max),
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
	@Value nvarchar(max),
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

Update p
Set p.IsMarkingRequired = 1 
From Catalog.Product p
Inner Join Catalog.ProductExportOptions eo on eo.ProductId = p.ProductId 
Where eo.YandexMarketCisRequired = 1

GO--

ALTER TABLE Catalog.ProductExportOptions
	DROP COLUMN YandexMarketCisRequired
GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.ExportImport.EProductField.Marking', 'Обязательная маркировка «Честный знак»'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.ExportImport.EProductField.Marking', 'Required marking «Honest mark»' 
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.ExportImport.ProductFields.Marking', 'Обязательная маркировка «Честный знак»'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.ExportImport.ProductFields.Marking', 'Required marking «Honest mark»' 

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.AddEditCity.FiasId', 'ФИАС код'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.AddEditCity.FiasId', 'FIAS code' 

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.AddEditCity.KladrId', 'КЛАДР код'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.AddEditCity.KladrId', 'KLADR code' 

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.AddEditRegions.AdditionalSettings.FiasId', 'ФИАС код'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.AddEditRegions.AdditionalSettings.FiasId', 'FIAS code' 

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.AddEditRegions.AdditionalSettings.KladrId', 'КЛАДР код'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.AddEditRegions.AdditionalSettings.KladrId', 'KLADR code' 

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.AddEditRegions.AdditionalSettigns', 'Доп настройки'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.AddEditRegions.AdditionalSettigns', 'Additional' 

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.AddEditRegions.AdditionalSettings.Header', 'Дополнительные настройки'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.AddEditRegions.AdditionalSettings.Header', 'Additional settings' 

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.AddEditRegions.MainSettings', 'Основные настройки'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.AddEditRegions.MainSettings', 'Main settings' 

GO--

IF NOT EXISTS(SELECT 1
              FROM sys.columns
              WHERE (name = N'ParamsJson') AND object_id = OBJECT_ID(N'[CRM].[TriggerAction]'))
BEGIN
	UPDATE [CRM].[TriggerAction]
		SET ObjValue = ('{ "BonusOperationType": 1' + 
		ISNULL((SELECT TOP(1) (CASE [Value] WHEN 'True' THEN ', "AddBonusesByItemComparers": true' ELSE ', "AddBonusesByItemComparers": false' END) FROM [Settings].[AdditionalOption] WHERE ObjType = 2 AND [ObjId] = [TriggerAction].Id AND [Name] = 'EditFieldAddBonusByItem'), '') + ' }') 
	WHERE EditFieldType <> '' AND (ObjValue IS NULL OR ObjValue <> '-1')
  
	UPDATE [CRM].[TriggerAction] 
		SET ObjValue = ('{ "BonusOperationType": 2' + 
		ISNULL((SELECT TOP(1) (CASE [Value] WHEN 'True' THEN ', "AddBonusesByItemComparers": true' ELSE ', "AddBonusesByItemComparers": false' END) FROM [Settings].[AdditionalOption] WHERE ObjType = 2 AND [ObjId] = [TriggerAction].Id AND [Name] = 'EditFieldAddBonusByItem'), '') + ' }') 
	WHERE EditFieldType <> '' AND ObjValue = '-1'
	
	DELETE FROM [Settings].[AdditionalOption] WHERE ObjType = 2 AND [Name] = 'EditFieldAddBonusByItem'

	EXEC sp_rename N'CRM.TriggerAction.ObjValue', 'ParamsJson', 'COLUMN'; 
END

GO--

CREATE TABLE [CMS].[StaticPageApi](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ParentId] [int] NULL,
	[Title] [nvarchar](255) NOT NULL,
	[Text] [nvarchar](max) NOT NULL,
	[Icon] [nvarchar](max) NULL,
	[Enabled] [bit] NOT NULL,
	[AddDate] [datetime] NOT NULL,
	[ModifyDate] [datetime] NOT NULL,
	[SortOrder] [int] NOT NULL,
	[ShowInProfile] [bit] NOT NULL,
 CONSTRAINT [PK_StaticPageApi] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO--

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Shipping].[FivePostPickPoints]') AND type in (N'U'))
BEGIN
	CREATE TABLE [Shipping].[FivePostPickPoints](
		[Id] nvarchar(max) NOT NULL,
		[Name] nvarchar(255) NOT NULL,
		[Type] int NOT NULL,
		[Description] nvarchar(max) NULL,
		[MaxWidth] float NOT NULL,
		[MaxHeight] float NOT NULL,
		[MaxLength] float NOT NULL,
		[MaxWeight] float NOT NULL,
		[Lattitude] float NOT NULL,
		[Longitude] float NOT NULL,
		[PossibleDeliveryList] nvarchar(max) NULL,
		[RateList] nvarchar(max) NULL,
		[ReturnAllowed] bit NOT NULL,
		[IsCash] bit NOT NULL,
		[IsCard] bit NOT NULL,
		[FiasCode] nvarchar(255) NOT NULL,
		[Address] nvarchar(max) NOT NULL,
		[Region] nvarchar(255) NOT NULL,
		[City] nvarchar(255) NOT NULL,
		[Phone] nvarchar(30) NOT NULL,
		[TimeWork] nvarchar(255) NOT NULL,
		[LastUpdate] DateTime NOT NULL
	)
END

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.PaymentMethods.Bill.ShowPaymentDetails', N'Вывести ИНН, КПП и организацию'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.PaymentMethods.Bill.ShowPaymentDetails', 'Show TIN, KPP and organization'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.PaymentMethods.Bill.ShowPaymentDetails.Help', N'Запрашивать ИНН, КПП и название организации у покупателя'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.PaymentMethods.Bill.ShowPaymentDetails.Help', 'Request details in the client side'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.OrderItemsSummary.INN', N'ИНН:'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.OrderItemsSummary.INN', 'TIN:'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.OrderItemsSummary.Kpp', N'КПП:'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.OrderItemsSummary.Kpp', 'KPP:'

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.ExportImport.ImportCsv.OfferCantBeUpdatedWithoutSku', N'Модификация не может быть обновлена/добавлена в строке {0}, если при импорте не указано поле {1} и у товара уже есть модификации'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.ExportImport.ImportCsv.OfferCantBeUpdatedWithoutSku', 'The offer cannot be updated/added in line {0} if the {1} field is not specified and the product already has offers'

GO--

IF NOT EXISTS (SELECT * 
			   FROM INFORMATION_SCHEMA.TABLES 
			   WHERE TABLE_SCHEMA = 'Catalog' AND  TABLE_NAME = 'PriceRule_CustomerGroup')
BEGIN
    CREATE TABLE [Catalog].[PriceRule_CustomerGroup](
		[PriceRuleId] [int] NOT NULL,
		[CustomerGroupId] [int] NOT NULL,
	CONSTRAINT [PK_PriceRule_CustomerGroup] PRIMARY KEY CLUSTERED 
	(
		[PriceRuleId] ASC,
		[CustomerGroupId] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]
END

GO--

IF NOT EXISTS (SELECT 1 
			   FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS 
			   WHERE CONSTRAINT_NAME='FK_PriceRule_CustomerGroup_PriceRule')
BEGIN
	ALTER TABLE [Catalog].[PriceRule_CustomerGroup]  WITH CHECK ADD CONSTRAINT [FK_PriceRule_CustomerGroup_PriceRule] FOREIGN KEY([PriceRuleId])
	REFERENCES [Catalog].[PriceRule] ([Id])
	ON DELETE CASCADE
END

GO--

IF EXISTS (SELECT 1 
		   FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS 
		   WHERE CONSTRAINT_NAME='FK_PriceRule_CustomerGroup_PriceRule')
BEGIN
	ALTER TABLE [Catalog].[PriceRule_CustomerGroup] CHECK CONSTRAINT [FK_PriceRule_CustomerGroup_PriceRule]
END

GO--

IF NOT EXISTS (SELECT * FROM [Catalog].[PriceRule_CustomerGroup]) and EXISTS (SELECT 1 FROM sys.columns WHERE Name = N'CustomerGroupId' AND Object_ID = Object_ID(N'[Catalog].[PriceRule]'))
BEGIN
	Insert Into [Catalog].[PriceRule_CustomerGroup] (PriceRuleId, CustomerGroupId) 
	Select Id, CustomerGroupId 
	From [Catalog].[PriceRule]
END

GO--

IF EXISTS (SELECT 1 FROM sys.columns 
		   WHERE Name = N'CustomerGroupId' AND Object_ID = Object_ID(N'[Catalog].[PriceRule]'))
BEGIN		  
	ALTER TABLE Catalog.PriceRule
		DROP COLUMN CustomerGroupId
END

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Catalog.ShowPriceAmountNextDiscountsInCart', N'Выводить следующую возможную цену и скидку от кол-ва на странице корзины'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Catalog.ShowPriceAmountNextDiscountsInCart', 'Display the next possible price and discount from the quantity on the cart page'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Catalog.ShowPriceAmountNextDiscountsInCartHint', N'Выводить следующую возможную цену и скидку от кол-ва на странице корзины. Это может мотивировать покупателя купить больше товара.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Catalog.ShowPriceAmountNextDiscountsInCartHint', 'Display the next possible price and discount from the quantity on the cart page. This may motivate the buyer to buy more goods.'

GO--

ALTER TABLE CRM.TriggerAction ADD
	SmsTemplateId int NULL
GO--


EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.SelectSmsTemplate.Text', 'Шаблоны'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.SelectSmsTemplate.Text', 'Templates' 

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Select', 'Выбрать'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Select', 'Select' 

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Cancel', 'Отмена'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Cancel', 'Cancel' 

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.NotInTariff','Функционал недоступен на вашем тарифном плане'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.NotInTariff','Functionality is not available on your tariff plan'

GO--

if exists (Select 1 from [Settings].[SmsTemplate]) 
begin
    Update [Settings].[SmsTemplate]
    Set [Text] = Replace([Text], '#ORDER_SUM#', '#SUM#')

    Update [Settings].[SmsTemplate]
    Set [Text] = Replace([Text], '#ORDER_SHIPPING_NAME#', '#SHIPPING_NAME#')

    Update [Settings].[SmsTemplate]
    Set [Text] = Replace([Text], '#ORDER_PAYMENT_NAME#', '#PAYMENT_NAME#')

    Update [Settings].[SmsTemplate]
    Set [Text] = Replace([Text], '#ORDER_BILLING_SHORTLINK#', '#BILLING_SHORT_LINK#')


    Update [Settings].[SmsTemplate]
    Set [Text] = Replace([Text], '#LEAD_SUM#', '#SUM#')

    Update [Settings].[SmsTemplate]
    Set [Text] = Replace([Text], '#LEAD_SALES_FUNNEL#', '#SALES_FUNNEL#')

    Update [Settings].[SmsTemplate]
    Set [Text] = Replace([Text], '#LEAD_DEAL_STATUS#', '#DEAL_STATUS#')

    Update [Settings].[SmsTemplate]
    Set [Text] = Replace([Text], '#LEAD_SHIPPING_NAME#', '#SHIPPING_NAME_WITHOUT_HTML#')
end

GO--

if exists (Select 1 From [Settings].[Settings] Where Name = 'SmsTextOnNewLead') and (Select len([Value]) From [Settings].[Settings] Where Name = 'SmsTextOnNewLead') > 0
begin
	Update [Settings].[Settings] 
	Set [Value] = Replace([Value], '#TITLE#', '#LEAD_TITLE#') 
	Where Name = 'SmsTextOnNewLead'

	Update [Settings].[Settings] 
	Set [Value] = Replace([Value], '#LEAD_SUM#', '#SUM#') 
	Where Name = 'SmsTextOnNewLead'

	Update [Settings].[Settings] 
	Set [Value] = Replace([Value], '#LEAD_SALES_FUNNEL#', '#SALES_FUNNEL#') 
	Where Name = 'SmsTextOnNewLead'

	Update [Settings].[Settings] 
	Set [Value] = Replace([Value], '#LEAD_DEAL_STATUS#', '#DEAL_STATUS#') 
	Where Name = 'SmsTextOnNewLead'

	Update [Settings].[Settings] 
	Set [Value] = Replace([Value], '#LEAD_SHIPPING_NAME#', '#SHIPPING_NAME_WITHOUT_HTML#') 
	Where Name = 'SmsTextOnNewLead'
end

GO--

if exists (Select 1 From [Settings].[Settings] Where Name = 'SmsTextOnNewOrder') and (Select len([Value]) From [Settings].[Settings] Where Name = 'SmsTextOnNewOrder') > 0
begin
	Update [Settings].[Settings] 
	Set [Value] = Replace([Value], '#ORDER_SUM#', '#SUM#') 
	Where Name = 'SmsTextOnNewOrder'

	Update [Settings].[Settings] 
	Set [Value] = Replace([Value], '#ORDER_PAYMENT_NAME#', '#PAYMENT_NAME#') 
	Where Name = 'SmsTextOnNewOrder'

	Update [Settings].[Settings] 
	Set [Value] = Replace([Value], '#ORDER_SHIPPING_NAME#', '#SHIPPING_NAME#') 
	Where Name = 'SmsTextOnNewOrder'

	Update [Settings].[Settings] 
	Set [Value] = Replace([Value], '#ORDER_BILLING_SHORTLINK#', '#BILLING_SHORT_LINK#') 
	Where Name = 'SmsTextOnNewOrder'
end

GO--

if exists (Select 1 From [Settings].[SmsTemplateOnOrderChanging]) 
begin
	Update [Settings].[SmsTemplateOnOrderChanging] 
	Set [SmsText] = Replace([SmsText], '#ORDER_SUM#', '#SUM#')

	Update [Settings].[SmsTemplateOnOrderChanging] 
	Set [SmsText] = Replace([SmsText], '#ORDER_PAYMENT_NAME#', '#PAYMENT_NAME#')

	Update [Settings].[SmsTemplateOnOrderChanging] 
	Set [SmsText] = Replace([SmsText], '#ORDER_SHIPPING_NAME#', '#SHIPPING_NAME#')

	Update [Settings].[SmsTemplateOnOrderChanging] 
	Set [SmsText] = Replace([SmsText], '#ORDER_BILLING_SHORTLINK#', '#BILLING_SHORT_LINK#')
end

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.ExportImport.CategoryFields.RelatedCategories', 'Категории "С этим товаром покупают"'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.ExportImport.CategoryFields.RelatedCategories', 'Categories "With this product buy"' 

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.ExportImport.CategoryFields.SimilarCategories', 'Категории "Похожие товары"'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.ExportImport.CategoryFields.SimilarCategories', 'Categories "Similar goods"' 

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.ExportImport.CategoryFields.RelatedProperties', 'Свойства "С этим товаром покупают"'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.ExportImport.CategoryFields.RelatedProperties', 'Properties "With this product buy"' 

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.ExportImport.CategoryFields.SimilarProperties', 'Свойства "Похожие товары"'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.ExportImport.CategoryFields.SimilarProperties', 'Properties "Similar goods"' 

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.ExportFeeed.SettingsCsv.PropertySeparator', 'Разделитель между свойствами'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.ExportFeeed.SettingsCsv.PropertySeparator', 'Property separator' 

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.ExportFeeed.SettingsCsv.NameSameProductProperty', 'Значение свойства для которого настроено "Совпадает со свойством товара"'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.ExportFeeed.SettingsCsv.NameSameProductProperty', 'The value of the property for which "Coincides with the property of the goods" is configured' 

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.ExportFeeed.SettingsCsv.NameNotSameProductProperty', 'Значение свойства для которого настроено "Не совпадает со свойством товара"'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.ExportFeeed.SettingsCsv.NameNotSameProductProperty', 'The value of the property for which "Does not match the product property" is configured' 

GO--

if exists (Select 1 From [Settings].[MailFormatType] Where MailType = 'OnRegistration')
begin
	Update [Settings].[MailFormat] 
	Set [FormatText] = Replace([FormatText], '#FIRSTNAME#', '#FIRST_NAME#'), 
	    [FormatSubject] = Replace([FormatSubject], '#FIRSTNAME#', '#FIRST_NAME#') 
	Where [MailFormatTypeId] in (Select [MailFormatTypeID] From [Settings].[MailFormatType] Where MailType = 'OnRegistration')

	Update [Settings].[MailFormat] 
	Set [FormatText] = Replace([FormatText], '#LASTNAME#', '#LAST_NAME#'), 
	    [FormatSubject] = Replace([FormatSubject], '#LASTNAME#', '#LAST_NAME#') 
	Where [MailFormatTypeId] in (Select [MailFormatTypeID] From [Settings].[MailFormatType] Where MailType = 'OnRegistration')

	Update [Settings].[MailFormat] 
	Set [FormatText] = Replace([FormatText], '#SUBSRCIBE#', '#NEWS_SUBSCRIPTION#'), 
	    [FormatSubject] = Replace([FormatSubject], '#SUBSRCIBE#', '#NEWS_SUBSCRIPTION#') 
	Where [MailFormatTypeId] in (Select [MailFormatTypeID] From [Settings].[MailFormatType] Where MailType = 'OnRegistration')

	Update [Settings].[MailFormat] 
	Set [FormatText] = Replace([FormatText], '#SHOPURL#', '#STORE_URL#'), 
	    [FormatSubject] = Replace([FormatSubject], '#SHOPURL#', '#STORE_URL#') 
	Where [MailFormatTypeId] in (Select [MailFormatTypeID] From [Settings].[MailFormatType] Where MailType = 'OnRegistration')

	Update [Settings].[MailFormatType]
	Set [Comment] = Replace( Replace( Replace( Replace([Comment], '#FIRSTNAME#', '#FIRST_NAME#'), '#LASTNAME#', '#LAST_NAME#'), '#SUBSRCIBE#', '#NEWS_SUBSCRIPTION#'), '#SHOPURL#', '#STORE_URL#') 
	Where MailType = 'OnRegistration'
end

GO--

Update [CRM].[TriggerAction] 
Set EmailSubject = Replace(EmailSubject, '#FIRSTNAME#', '#FIRST_NAME#'), 
	EmailBody = Replace(EmailBody, '#FIRSTNAME#', '#FIRST_NAME#'), 
	SmsText = Replace(SmsText, '#FIRSTNAME#', '#FIRST_NAME#'),
	MessageText = Replace(MessageText, '#FIRSTNAME#', '#FIRST_NAME#'),
	NotificationBody = Replace(NotificationBody, '#FIRSTNAME#', '#FIRST_NAME#'),
	NotificationTitle = Replace(NotificationTitle, '#FIRSTNAME#', '#FIRST_NAME#')
Where TriggerRuleId in (Select Id From [CRM].[TriggerRule] Where [EventType] in (5, 7, 8, 9, 10))

GO--

Update [CRM].[TriggerAction] 
Set EmailSubject = Replace(EmailSubject, '#LASTNAME#', '#LAST_NAME#'), 
	EmailBody = Replace(EmailBody, '#LASTNAME#', '#LAST_NAME#'), 
	SmsText = Replace(SmsText, '#LASTNAME#', '#LAST_NAME#'),
	MessageText = Replace(MessageText, '#LASTNAME#', '#LAST_NAME#'),
	NotificationBody = Replace(NotificationBody, '#LASTNAME#', '#LAST_NAME#'),
	NotificationTitle = Replace(NotificationTitle, '#LASTNAME#', '#LAST_NAME#')
Where TriggerRuleId in (Select Id From [CRM].[TriggerRule] Where [EventType] in (5, 7, 8, 9, 10))

GO--

Update [CRM].[TriggerAction] 
Set EmailSubject = Replace(EmailSubject, '#SUBSRCIBE#', '#NEWS_SUBSCRIPTION#'), 
	EmailBody = Replace(EmailBody, '#SUBSRCIBE#', '#NEWS_SUBSCRIPTION#'), 
	SmsText = Replace(SmsText, '#SUBSRCIBE#', '#NEWS_SUBSCRIPTION#'),
	MessageText = Replace(MessageText, '#SUBSRCIBE#', '#NEWS_SUBSCRIPTION#'),
	NotificationBody = Replace(NotificationBody, '#SUBSRCIBE#', '#NEWS_SUBSCRIPTION#'),
	NotificationTitle = Replace(NotificationTitle, '#SUBSRCIBE#', '#NEWS_SUBSCRIPTION#')
Where TriggerRuleId in (Select Id From [CRM].[TriggerRule] Where [EventType] in (5, 7, 8, 9, 10))

GO--

Update [CRM].[TriggerAction] 
Set EmailSubject = Replace(EmailSubject, '#SHOPURL#', '#STORE_URL#'), 
	EmailBody = Replace(EmailBody, '#SHOPURL#', '#STORE_URL#'), 
	SmsText = Replace(SmsText, '#SHOPURL#', '#STORE_URL#'),
	MessageText = Replace(MessageText, '#SHOPURL#', '#STORE_URL#'),
	NotificationBody = Replace(NotificationBody, '#SHOPURL#', '#STORE_URL#'),
	NotificationTitle = Replace(NotificationTitle, '#SHOPURL#', '#STORE_URL#')
Where TriggerRuleId in (Select Id From [CRM].[TriggerRule] Where [EventType] in (5, 7, 8, 9, 10))

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.Zone.YourLocation', 'Где вы находитесь?'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.Zone.YourLocation', 'Where are you located?' 

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Settings.SettingsCatalog.DisplayCityBubbleType.NoDisplay', 'Не спрашивать'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Settings.SettingsCatalog.DisplayCityBubbleType.NoDisplay', 'Don`t ask' 

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Settings.SettingsCatalog.DisplayCityBubbleType.DisplayZonePopover', 'Отображать окошко для уточнения верно ли выбран город'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Settings.SettingsCatalog.DisplayCityBubbleType.DisplayZonePopover', 'Display a window to clarify whether the city is selected correctly' 

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Settings.SettingsCatalog.DisplayCityBubbleType.DisplayZoneDialog', 'Принудительно открывать окно выбора города'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Settings.SettingsCatalog.DisplayCityBubbleType.DisplayZoneDialog', 'Force open the city selection window' 

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.SystemSettings.HideCountriesInZoneDialog', 'Скрывать выбор страны в окне выбора города'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.SystemSettings.HideCountriesInZoneDialog', 'Hide the country selection in the city selection window' 

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.SystemSettings.HideSearchInZoneDialog', 'Скрывать поиск в окне выбора города'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.SystemSettings.HideSearchInZoneDialog', 'Hide the search in the city selection window' 

IF EXISTS (SELECT TOP(1) 1 FROM [Settings].[Settings] WHERE [Name] = 'DisplayCityBubble' AND ([Value] = 'True' OR [Value] = 'False'))
	UPDATE [Settings].[Settings] SET [Value] = case (SELECT TOP(1) [Value] FROM [Settings].[Settings] WHERE [Name] = 'DisplayCityBubble') when 'True' then 1 else 0 end WHERE [Name] = 'DisplayCityBubble'

GO--

IF EXISTS (SELECT 1 FROM [Settings].[Settings] WHERE [Name] = 'LinkFacebook' AND [Value] = 'https://www.facebook.com/AdVantShop.NET')
	UPDATE [Settings].[Settings] SET [Value] = 'False' WHERE [Name] = 'LinkFacebookActive'
GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Checkout.AllowEditAmount', 'Разрешить редактировать количество товаров'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Checkout.AllowEditAmount', 'Allow editing quantity' 

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Checkout.AllowEditAmount.Help', 'При активной опции будет разрешено редактировать количество и удалять товар из корзины на странице оформления заказа'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Checkout.AllowEditAmount.Help', 'When the option is active, it will be allowed to edit the quantity and delete the product from the cart on the checkout page' 

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Category.RightPanel.OptionTurnOnOf', 'Опция включает или выключает отображение колонки "Производители" в меню каталога для данной категории. <br/>В целях оптимизации скорости работы сайта выводится не более 20 производителей. Настройка доступна только для категорий первого уровня. '
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Category.RightPanel.OptionTurnOnOf', 'The option enables or disables the display of the "Manufacturers" column in the catalog menu for this category. <br/>In order to optimize the speed of the site, no more than 20 manufacturers are displayed. The setting is only available for first-level categories.' 

GO--

ALTER PROCEDURE [Customers].[sp_GetRecentlyView]
	@CustomerId uniqueidentifier,
	@rowsCount int,
	@Type nvarchar(50)
AS
BEGIN
	SET NOCOUNT ON;
	
	IF Exists (SELECT 1 FROM [Customers].[RecentlyViewsData] WHERE CustomerID=@CustomerId)
    Begin
        SELECT TOP(@rowsCount) Product.ProductID, Product.Name, Product.UrlPath, Product.AllowPreOrder, Ratio, ManualRatio, isnull(PhotoNameSize1, PhotoName) as PhotoName,
            [Photo].[Description] as PhotoDescription, Discount, DiscountAmount, MinPrice as BasePrice, CurrencyValue,
            Offer.OfferID, MaxAvailable AS Amount, MinAmount, MaxAmount, Offer.Amount AS AmountOffer, Colors, NotSamePrices as MultiPrices,
            Product.DoNotApplyOtherDiscounts
        
        FROM [Customers].RecentlyViewsData
        
            Inner Join [Catalog].Product ON Product.ProductID = RecentlyViewsData.ProductId
            Left Join [Catalog].[ProductExt] ON [Product].[ProductID] = [ProductExt].[ProductID]
            Inner Join Catalog.Currency On Currency.CurrencyID = Product.CurrencyID
            Left Join [Catalog].[Photo] ON [Photo].[PhotoId] = [ProductExt].[PhotoId]
            Left Join [Catalog].[Offer] ON [ProductExt].[OfferID] = [Offer].[OfferID]
        
        WHERE RecentlyViewsData.CustomerID = @CustomerId AND Product.Enabled = 1 And CategoryEnabled = 1
        
        ORDER BY ViewDate Desc
    End
END

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.LimitedCategoryMenu', 'Скрывать в меню категории, которые не помещаются в экран'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.LimitedCategoryMenu', 'Closing categories in the menu that are not on the screen' 

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.LimitedCategoryMenuHint', 'Если настройка включена, то все категории из вертикального меню (двухколоночный режим отображения), которые выходят за пределы экрана браузера, будут скрываться, и вместо них выведется пункт "Показать все". Настройка работает, если категорий больше 7'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.LimitedCategoryMenuHint', 'If the setting is enabled, then all categories from the vertical menu (two-column display mode) that extend beyond the browser screen will be hidden, and the “Show all” option will be displayed instead. The setting works if there are more than 7 categories' 

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.Builder.LimitedCategoryMenu', 'Скрывать в меню категории, которые не помещаются в экран'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.Builder.LimitedCategoryMenu', 'Closing categories in the menu that are not on the screen' 

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.Builder.LimitedCategoryMenuHint', 'Если настройка включена, то все категории из вертикального меню (двухколоночный режим отображения), которые выходят за пределы экрана браузера, будут скрываться, и вместо них выведется пункт "Показать все". Настройка работает, если категорий больше 7'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.Builder.LimitedCategoryMenuHint', 'If the setting is enabled, then all categories from the vertical menu (two-column display mode) that extend beyond the browser screen will be hidden, and the “Show all” option will be displayed instead. The setting works if there are more than 7 categories' 

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.CustomOption.CustomOptionInputType.ChoiceOfProduct', 'Выбор товара'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.CustomOption.CustomOptionInputType.ChoiceOfProduct', 'Choice of products'

GO--

IF NOT EXISTS(SELECT 1
              FROM sys.columns
              WHERE (name = N'ProductId') AND object_id = OBJECT_ID(N'[Catalog].[Options]'))
BEGIN
	ALTER TABLE [Catalog].[Options]
	ADD ProductId int NULL
END

GO--

IF NOT EXISTS(SELECT 1
              FROM sys.columns
              WHERE (name = N'OfferId') AND object_id = OBJECT_ID(N'[Catalog].[Options]'))
BEGIN
	ALTER TABLE [Catalog].[Options]
	ADD OfferId int NULL
END

IF NOT EXISTS(SELECT 1
              FROM sys.columns
              WHERE (name = N'MinQuantity') AND object_id = OBJECT_ID(N'[Catalog].[Options]'))
BEGIN
	ALTER TABLE [Catalog].[Options]
	ADD MinQuantity float NULL
END

IF NOT EXISTS(SELECT 1
              FROM sys.columns
              WHERE (name = N'MaxQuantity') AND object_id = OBJECT_ID(N'[Catalog].[Options]'))
BEGIN
	ALTER TABLE [Catalog].[Options]
	ADD MaxQuantity float NULL
END

IF NOT EXISTS(SELECT 1
              FROM sys.columns
              WHERE (name = N'DefaultQuantity') AND object_id = OBJECT_ID(N'[Catalog].[Options]'))
BEGIN
	ALTER TABLE [Catalog].[Options]
	ADD DefaultQuantity float NULL
END

IF NOT EXISTS(SELECT 1
              FROM sys.columns
              WHERE (name = N'MaxQuantity') AND object_id = OBJECT_ID(N'[Catalog].[CustomOptions]'))
BEGIN
	ALTER TABLE [Catalog].[CustomOptions]
	ADD MaxQuantity float NULL
END

IF NOT EXISTS(SELECT 1
              FROM sys.columns
              WHERE (name = N'MinQuantity') AND object_id = OBJECT_ID(N'[Catalog].[CustomOptions]'))
BEGIN
	ALTER TABLE [Catalog].[CustomOptions]
	ADD MinQuantity float NULL
END

IF NOT EXISTS(SELECT 1
              FROM sys.columns
              WHERE (name = N'Description') AND object_id = OBJECT_ID(N'[Catalog].[CustomOptions]'))
BEGIN
	ALTER TABLE [Catalog].[CustomOptions]
	ADD Description nvarchar(max) NULL
END

IF NOT EXISTS(SELECT 1
              FROM sys.columns
              WHERE (name = N'Description') AND object_id = OBJECT_ID(N'[Catalog].[Options]'))
BEGIN
	ALTER TABLE [Catalog].[Options]
	ADD Description nvarchar(max) NULL
END

GO--

IF EXISTS (SELECT 1 FROM sys.objects WHERE type = 'P' AND OBJECT_ID = OBJECT_ID('[Catalog].[sp_GetOptionsByCustomOptionId]'))
BEGIN
	DROP PROCEDURE [Catalog].[sp_GetOptionsByCustomOptionId]
END

IF EXISTS (SELECT 1 FROM sys.objects WHERE type = 'P' AND OBJECT_ID = OBJECT_ID('[Catalog].[sp_AddCustomOption]'))
BEGIN
	DROP PROCEDURE [Catalog].[sp_AddCustomOption]
END

IF EXISTS (SELECT 1 FROM sys.objects WHERE type = 'P' AND OBJECT_ID = OBJECT_ID('[Catalog].[sp_UpdateCustomOption]'))
BEGIN
	DROP PROCEDURE [Catalog].[sp_UpdateCustomOption]
END

IF EXISTS (SELECT 1 FROM sys.objects WHERE type = 'P' AND OBJECT_ID = OBJECT_ID('[Catalog].[sp_GetCustomOptionsByProductId]'))
BEGIN
	DROP PROCEDURE [Catalog].[sp_GetCustomOptionsByProductId]
END

GO--

IF EXISTS (SELECT 1 FROM sys.objects WHERE type = 'P' AND OBJECT_ID = OBJECT_ID('[Catalog].[sp_UpdateOption]'))
BEGIN
	DROP PROCEDURE [Catalog].[sp_UpdateOption]
END

GO--

IF EXISTS (SELECT 1 FROM sys.objects WHERE type = 'P' AND OBJECT_ID = OBJECT_ID('[Catalog].[sp_AddOption]'))
BEGIN
	DROP PROCEDURE [Catalog].[sp_AddOption]
END

IF EXISTS (SELECT 1 FROM sys.objects WHERE type = 'P' AND OBJECT_ID = OBJECT_ID('[Order].[sp_GetSelectedOptionsByOrderItemId]'))
BEGIN
	DROP PROCEDURE [Order].[sp_GetSelectedOptionsByOrderItemId]
END

IF EXISTS (SELECT 1 FROM sys.objects WHERE type = 'P' AND OBJECT_ID = OBJECT_ID('[Order].[sp_AddOrderCustomOptions]'))
BEGIN
	DROP PROCEDURE [Order].[sp_AddOrderCustomOptions]
END

IF NOT EXISTS(SELECT 1
              FROM sys.columns
              WHERE (name = N'OptionAmount') AND object_id = OBJECT_ID(N'[Order].[OrderCustomOptions]'))
BEGIN
	ALTER TABLE [Order].[OrderCustomOptions]
	ADD OptionAmount float NULL
END

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.ExportFeed.SettingsYandex.PriceRuleIdForOldPrice', 'Тип цены для тега старой цены'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.ExportFeed.SettingsYandex.PriceRuleIdForOldPrice', 'Price type for oldprice tag' 

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.ExportFeed.SettingsYandex.PriceRuleIdForOldPrice.Help', 'Если выбран тип цены, то в тег olprice будет выгружаться цена с выбранным типом цен без скидок товара. Если у товара не указана цена с данным типом цен, то старая цена будет считаться от обычной цены товара с учетом скидки.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.ExportFeed.SettingsYandex.PriceRuleIdForOldPrice.Help', 'If the price type is selected, the oldprice tag will load the price with selected price type without product discounts. If the product does not have a price with this type of price, then the old price will be considered from the usual price of the product, taking into account the discount.' 

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Checkout.TypeStoreWorkMode', 'Время приема заказов'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Checkout.TypeStoreWorkMode', 'Order acceptance time' 

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Checkout.EStoreWorkMode.AroundTheClock', 'Круглосуточно'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Checkout.EStoreWorkMode.AroundTheClock', 'Around the clock' 

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Checkout.EStoreWorkMode.ByTime', 'По времени'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Checkout.EStoreWorkMode.ByTime', 'By time' 

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Cart.Error.NotAllowCheckoutNow', 'Оформление заказа в текущее время недоступно'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Cart.Error.NotAllowCheckoutNow', 'Checkout is not available at the current time' 

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.WorkingTime.Settings.AreYouSureDelete', 'Вы уверены, что хотите удалить все интервалы?'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.WorkingTime.Settings.AreYouSureDelete', 'Are you sure you want to delete all the intervals?' 

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.WorkingTime.Settings.DeletingIntervals', 'Удаление интервалов'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.WorkingTime.Settings.DeletingIntervals', 'Deleting intervals' 

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.WorkingTime.Settings.GMT', 'Часовой пояс'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.WorkingTime.Settings.GMT', 'GMT' 

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.WorkingTime.Settings.AdditionalSettings', 'Дополнительные настройки'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.WorkingTime.Settings.AdditionalSettings', 'Additional settings' 

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.WorkingTime.Settings.AddInterval', 'Добавить'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.WorkingTime.Settings.AddInterval', 'Add' 

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.WorkingTime.Settings.DayOfWeek', 'День недели'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.WorkingTime.Settings.DayOfWeek', 'Day of week' 

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.WorkingTime.Settings.DeleteAllIntervals', 'Удалить все интервалы'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.WorkingTime.Settings.DeleteAllIntervals', 'Delete all intervals' 

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.WorkingTime.Settings.IntervalIsEmpty', 'Нет интервалов'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.WorkingTime.Settings.IntervalIsEmpty', 'Not intervals' 

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.WorkingTime.Settings.Header', 'Интервалы рабочего времени'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.WorkingTime.Settings.Header', 'Working time intervals' 

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.WorkingTime.Settings.NotAllowCheckoutText', 'Сообщение в шапке сайта в нерабочее время'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.WorkingTime.Settings.NotAllowCheckoutText', 'A message in the header during non-working hours' 

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Checkout.WorkingTimeIntervals', 'Интервалы рабочего времени'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Checkout.WorkingTimeIntervals', 'Working time intervals' 

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.AddEdit.Delete', 'Удалить'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.AddEdit.Delete', 'Delete' 

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.WorkingTime.Settings.AdditionalTimeFrom', 'Дополнительное время с'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.WorkingTime.Settings.AdditionalTimeFrom', 'Additional time from' 

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.WorkingTime.Settings.AdditionalTimeTo', 'Дополнительное время по'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.WorkingTime.Settings.AdditionalTimeTo', 'Additional time to' 

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.WorkingTime.Settings.AdditionalTimeFor', 'Дополнительное время на'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.WorkingTime.Settings.AdditionalTimeFor', 'Additional time for' 

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.WorkingTime.Settings.WorkingDay', 'Рабочий'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.WorkingTime.Settings.WorkingDay', 'Working day' 

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.WorkingTime.Settings.Weekend', 'Выходной'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.WorkingTime.Settings.Weekend', 'Weekend' 

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Checkout.NotAllowCheckoutText.DefaultText', 'Извините, сейчас мы не работаем.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Checkout.NotAllowCheckoutText.DefaultText', 'Sorry, we''re not working right now.' 

IF NOT EXISTS (SELECT TOP(1) 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[Settings].[WorkingTime]') AND type in (N'U'))
BEGIN
	CREATE TABLE [Settings].[WorkingTime](
		[DayOfWeek] [tinyint] NOT NULL,
		[StartTime] [datetime] NOT NULL,
		[EndTime] [datetime] NOT NULL,
	 CONSTRAINT [PK_WorkingTime] PRIMARY KEY CLUSTERED 
	(
		[DayOfWeek] ASC,
		[StartTime] ASC,
		[EndTime] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]

	CREATE TABLE [Settings].[AdditionalWorkingTime](
		[StartTime] [datetime] NOT NULL,
		[EndTime] [datetime] NOT NULL,	
		[IsWork] [bit] NOT NULL,
	 CONSTRAINT [PK_AdditionalWorkingTime] PRIMARY KEY CLUSTERED 
	(
		[StartTime] ASC,
		[EndTime] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]
END

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.WorkigTime.InvalidSettingsModel', 'Не указаны настройки';
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.WorkigTime.InvalidSettingsModel', 'Settings are not specified';

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.WorkigTime.InvalidGMT', 'Неверный часовой пояс';
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.WorkigTime.InvalidGMT', 'Invalid time zone';

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.WorkigTime.DateNotSpecified', 'Дата не указана';
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.WorkigTime.DateNotSpecified', 'Date not specified';

GO--

IF NOT EXISTS(SELECT 1
			  FROM sys.columns
			  WHERE object_id = OBJECT_ID(N'[Catalog].[SizeChart]'))
	BEGIN
		CREATE TABLE [Catalog].[SizeChart](
			[Id] [int] IDENTITY(1,1) NOT NULL,
			[Name] [nvarchar](255) NOT NULL,
			[LinkText] [nvarchar](255) NOT NULL,
			[SourceType] [int] NOT NULL,
			[Text] [nvarchar](MAX) NOT NULL,
			[SortOrder] [int] NULL,
			[Enabled] [bit] NOT NULL
		 CONSTRAINT [PK_SizeChart] PRIMARY KEY CLUSTERED 
		(
			[Id] ASC
		)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
		) ON [PRIMARY]
	END
	
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Catalog.SizeCharts', 'Таблицы размеров'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Catalog.SizeCharts', 'Size charts'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Catalog.Product.SizeChart', 'Таблица размеров'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Catalog.Product.SizeChart', 'Size chart'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Product.Edit.SizeChart', 'Таблица размеров'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Product.Edit.SizeChart', 'Size chart'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Product.Edit.SizeChartHint', 'Таблица размеров товара. Ссылка на таблицу размеров выводится в карточке товара, при клике по ссылке открывается модальное окно с таблицей размеров либо происходит редирект на заданную страницу.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Product.Edit.SizeChartHint', 'Product size chart. The link to the size table is displayed in the product card, when you click on the link, a modal window with the size table opens or a redirect to the specified page occurs.'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Product.SizeCharts.NotSelected', 'Не выбрано'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Product.SizeCharts.NotSelected', 'Not selected'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Category.RightPanel.SizeChart', 'Таблица размеров'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Category.RightPanel.SizeChart', 'Size chart'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Category.RightPanel.SizeChartHint', 'Таблица размеров категории. <br />Будет использоваться для товаров, у которых не задана таблица размеров и данная категория является основной.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Category.RightPanel.SizeChartHint', 'Category size chart. <br />Will be used for products that do not have a size chart set and this category is the main one.'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Catalog.SizeChartSourceType.Text', 'Текст'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Catalog.SizeChartSourceType.Text', 'Text'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Catalog.SizeChartSourceType.Link', 'Ссылка'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Catalog.SizeChartSourceType.Link', 'Link'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.SizeChart.DefaultLinkText', 'Таблица размеров'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.SizeChart.DefaultLinkText', 'Size chart'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.AddEditSizeChart.Settings.Enabled', 'Активность'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.AddEditSizeChart.Settings.Enabled', 'Enabled'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.AddEditSizeChart.Settings.Name', 'Название'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.AddEditSizeChart.Settings.Name', 'Name'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.AddEditSizeChart.Header', 'Таблица размеров'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.AddEditSizeChart.Header', 'Size chart'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.AddEditSizeChart.Settings.LinkText', 'Текст ссылки на таблицу'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.AddEditSizeChart.Settings.LinkText', 'The text of the link to the table'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.AddEditSizeChart.Settings.TableAsText', 'Задать таблицу текстом'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.AddEditSizeChart.Settings.TableAsText', 'Set the table as text'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.AddEditSizeChart.Settings.TableAsLink', 'Задать ссылку на таблицу'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.AddEditSizeChart.Settings.TableAsLink', 'Set a link to the table'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.AddEditSizeChart.Settings.SourceType.Text', 'Текст таблицы'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.AddEditSizeChart.Settings.SourceType.Text', 'Text'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.AddEditSizeChart.Settings.SourceType.Link', 'Ссылка'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.AddEditSizeChart.Settings.SourceType.Link', 'Link'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.AddEditSizeChart.InsertExamlpe', 'Вставить пример таблицы'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.AddEditSizeChart.InsertExamlpe', 'Insert an example table'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.AddEditSizeChart.Settings.SortOrder', 'Сортировка'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.AddEditSizeChart.Settings.SortOrder', 'Sort'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.SizeChart.Name', 'Название'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.SizeChart.Name', 'Name'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.SizeChart.LinkText', 'Текст ссылки'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.SizeChart.LinkText', 'Link text'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.SizeChart.SortOrder', 'Сортировка'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.SizeChart.SortOrder', 'Sort'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.SizeChart.Enabled', 'Активность'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.SizeChart.Enabled', 'Enabled'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.SizeChart.SourceType', 'Тип источника'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.SizeChart.SourceType', 'Source type'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Catalog.SizeChart.Settings.RequiredFieldIsEmpty', 'Не все обязательные поля заполнены'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Catalog.SizeChart.Settings.RequiredFieldIsEmpty', 'Not all required fields are filled in'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Catalog.SizeChart.Reference', 'Справочник'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Catalog.SizeChart.Reference', 'Reference'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Catalog.SizeChart.SizeChart', 'таблиц размеров'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Catalog.SizeChart.SizeChart', 'size chart'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Catalog.Settings.SizeChart.AddSizeChart', 'Добавить таблицу размеров'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Catalog.Settings.SizeChart.AddSizeChart', 'Add size chart'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.SizeChart.Example', '<table style="min-width: 800px; width: 100%; padding: 0; margin: 0; table-layout: fixed; border: 0;">
    <thead style="background-color: #f6f6f9;">
        <tr>
            <th style="padding: 1.5625rem 0.625rem 1.5625rem 1.5625rem; border-width: 0 0 0.0625rem 0; box-sizing: border-box; border-bottom: 0;"><span style="font-size: 16px;">Российский размер</span></th>
            <th style="padding: 1.5625rem 0.625rem 1.5625rem 1.5625rem; border-width: 0 0 0.0625rem 0; box-sizing: border-box; border-bottom: 0;"><span style="font-size: 16px;">Размер производителя</span></th>
            <th style="padding: 1.5625rem 0.625rem 1.5625rem 1.5625rem; border-width: 0 0 0.0625rem 0; box-sizing: border-box; border-bottom: 0;"><span style="font-size: 16px;">Обхват бедер, в см</span></th>
            <th style="padding: 1.5625rem 0.625rem 1.5625rem 1.5625rem; border-width: 0 0 0.0625rem 0; box-sizing: border-box; border-bottom: 0;"><span style="font-size: 16px;">Обхват груди, в см</span></th>
            <th style="padding: 1.5625rem 0.625rem 1.5625rem 1.5625rem; border-width: 0 0 0.0625rem 0; box-sizing: border-box; border-bottom: 0;"><span style="font-size: 16px;">Обхват талии, в см</span></th>
        </tr>
    </thead>
    <tbody style="">
        <tr>
            <td style="padding: 1.5625rem 0 1.5625rem 1.5625rem; border-width: 0 0 0.0625rem 0; border-bottom: 0.0625rem #e2e3e4 solid; box-sizing: border-box;"><span style="font-size: 16px;">42</span></td>
            <td style="padding: 1.5625rem 0 1.5625rem 1.5625rem; border-width: 0 0 0.0625rem 0; border-bottom: 0.0625rem #e2e3e4 solid; box-sizing: border-box;"><span style="font-size: 16px;">42</span></td>
            <td style="padding: 1.5625rem 0 1.5625rem 1.5625rem; border-width: 0 0 0.0625rem 0; border-bottom: 0.0625rem #e2e3e4 solid; box-sizing: border-box;"><span style="font-size: 16px;">90-94</span></td>
            <td style="padding: 1.5625rem 0 1.5625rem 1.5625rem; border-width: 0 0 0.0625rem 0; border-bottom: 0.0625rem #e2e3e4 solid; box-sizing: border-box;"><span style="font-size: 16px;">82-86</span></td>
            <td style="padding: 1.5625rem 0 1.5625rem 1.5625rem; border-width: 0 0 0.0625rem 0; border-bottom: 0.0625rem #e2e3e4 solid; box-sizing: border-box;"><span style="font-size: 16px;">62-66</span></td>
        </tr>
        <tr>
            <td style="padding: 1.5625rem 0 1.5625rem 1.5625rem; border-width: 0 0 0.0625rem 0; border-bottom: 0.0625rem #e2e3e4 solid; box-sizing: border-box;"><span style="font-size: 16px;">44</span></td>
            <td style="padding: 1.5625rem 0 1.5625rem 1.5625rem; border-width: 0 0 0.0625rem 0; border-bottom: 0.0625rem #e2e3e4 solid; box-sizing: border-box;"><span style="font-size: 16px;">44</span></td>
            <td style="padding: 1.5625rem 0 1.5625rem 1.5625rem; border-width: 0 0 0.0625rem 0; border-bottom: 0.0625rem #e2e3e4 solid; box-sizing: border-box;"><span style="font-size: 16px;">94-98</span></td>
            <td style="padding: 1.5625rem 0 1.5625rem 1.5625rem; border-width: 0 0 0.0625rem 0; border-bottom: 0.0625rem #e2e3e4 solid; box-sizing: border-box;"><span style="font-size: 16px;">86-90</span></td>
            <td style="padding: 1.5625rem 0 1.5625rem 1.5625rem; border-width: 0 0 0.0625rem 0; border-bottom: 0.0625rem #e2e3e4 solid; box-sizing: border-box;"><span style="font-size: 16px;">66-70</span></td>
        </tr>
        <tr>
            <td style="padding: 1.5625rem 0 1.5625rem 1.5625rem; border-width: 0 0 0.0625rem 0; border-bottom: 0.0625rem #e2e3e4 solid; box-sizing: border-box;"><span style="font-size: 16px;">46</span></td>
            <td style="padding: 1.5625rem 0 1.5625rem 1.5625rem; border-width: 0 0 0.0625rem 0; border-bottom: 0.0625rem #e2e3e4 solid; box-sizing: border-box;"><span style="font-size: 16px;">46</span></td>
            <td style="padding: 1.5625rem 0 1.5625rem 1.5625rem; border-width: 0 0 0.0625rem 0; border-bottom: 0.0625rem #e2e3e4 solid; box-sizing: border-box;"><span style="font-size: 16px;">98-102</span></td>
            <td style="padding: 1.5625rem 0 1.5625rem 1.5625rem; border-width: 0 0 0.0625rem 0; border-bottom: 0.0625rem #e2e3e4 solid; box-sizing: border-box;"><span style="font-size: 16px;">90-94</span></td>
            <td style="padding: 1.5625rem 0 1.5625rem 1.5625rem; border-width: 0 0 0.0625rem 0; border-bottom: 0.0625rem #e2e3e4 solid; box-sizing: border-box;"><span style="font-size: 16px;">70-74</span></td>
        </tr>
        <tr>
            <td style="padding: 1.5625rem 0 1.5625rem 1.5625rem; border-width: 0 0 0.0625rem 0; border-bottom: 0.0625rem #e2e3e4 solid; box-sizing: border-box;"><span style="font-size: 16px;">48</span></td>
            <td style="padding: 1.5625rem 0 1.5625rem 1.5625rem; border-width: 0 0 0.0625rem 0; border-bottom: 0.0625rem #e2e3e4 solid; box-sizing: border-box;"><span style="font-size: 16px;">48</span></td>
            <td style="padding: 1.5625rem 0 1.5625rem 1.5625rem; border-width: 0 0 0.0625rem 0; border-bottom: 0.0625rem #e2e3e4 solid; box-sizing: border-box;"><span style="font-size: 16px;">102-106</span></td>
            <td style="padding: 1.5625rem 0 1.5625rem 1.5625rem; border-width: 0 0 0.0625rem 0; border-bottom: 0.0625rem #e2e3e4 solid; box-sizing: border-box;"><span style="font-size: 16px;">94-98</span></td>
            <td style="padding: 1.5625rem 0 1.5625rem 1.5625rem; border-width: 0 0 0.0625rem 0; border-bottom: 0.0625rem #e2e3e4 solid; box-sizing: border-box;"><span style="font-size: 16px;">74-78</span></td>
        </tr>
    </tbody>
</table>'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.SizeChart.Example', '<table style="min-width: 800px; width: 100%; padding: 0; margin: 0; table-layout: fixed; border: 0;">
    <thead style="background-color: #f6f6f9;">
        <tr>
            <th style="padding: 1.5625rem 0.625rem 1.5625rem 1.5625rem; border-width: 0 0 0.0625rem 0; box-sizing: border-box; border-bottom: 0;"><span style="font-size: 16px;">Mens</span></th>
            <th style="padding: 1.5625rem 0.625rem 1.5625rem 1.5625rem; border-width: 0 0 0.0625rem 0; box-sizing: border-box; border-bottom: 0;"><span style="font-size: 16px;">Small</span></th>
            <th style="padding: 1.5625rem 0.625rem 1.5625rem 1.5625rem; border-width: 0 0 0.0625rem 0; box-sizing: border-box; border-bottom: 0;"><span style="font-size: 16px;">Medium</span></th>
            <th style="padding: 1.5625rem 0.625rem 1.5625rem 1.5625rem; border-width: 0 0 0.0625rem 0; box-sizing: border-box; border-bottom: 0;"><span style="font-size: 16px;">Large</span></th>
            <th style="padding: 1.5625rem 0.625rem 1.5625rem 1.5625rem; border-width: 0 0 0.0625rem 0; box-sizing: border-box; border-bottom: 0;"><span style="font-size: 16px;">X-Large</span></th>
            <th style="padding: 1.5625rem 0.625rem 1.5625rem 1.5625rem; border-width: 0 0 0.0625rem 0; box-sizing: border-box; border-bottom: 0;"><span style="font-size: 16px;">XX-Large</span></th>
        </tr>
    </thead>
    <tbody style="">
        <tr>
            <td style="padding: 1.5625rem 0 1.5625rem 1.5625rem; border-width: 0 0 0.0625rem 0; border-bottom: 0.0625rem #e2e3e4 solid; box-sizing: border-box;"><span style="font-size: 16px;">Neck</span></td>
            <td style="padding: 1.5625rem 0 1.5625rem 1.5625rem; border-width: 0 0 0.0625rem 0; border-bottom: 0.0625rem #e2e3e4 solid; box-sizing: border-box;"><span style="font-size: 16px;">14-14.5</span></td>
            <td style="padding: 1.5625rem 0 1.5625rem 1.5625rem; border-width: 0 0 0.0625rem 0; border-bottom: 0.0625rem #e2e3e4 solid; box-sizing: border-box;"><span style="font-size: 16px;">15-15.5</span></td>
            <td style="padding: 1.5625rem 0 1.5625rem 1.5625rem; border-width: 0 0 0.0625rem 0; border-bottom: 0.0625rem #e2e3e4 solid; box-sizing: border-box;"><span style="font-size: 16px;">16-16.5</span></td>
            <td style="padding: 1.5625rem 0 1.5625rem 1.5625rem; border-width: 0 0 0.0625rem 0; border-bottom: 0.0625rem #e2e3e4 solid; box-sizing: border-box;"><span style="font-size: 16px;">17-17.5</span></td>
            <td style="padding: 1.5625rem 0 1.5625rem 1.5625rem; border-width: 0 0 0.0625rem 0; border-bottom: 0.0625rem #e2e3e4 solid; box-sizing: border-box;"><span style="font-size: 16px;">18-18.5</span></td>
        </tr>
        <tr>
            <td style="padding: 1.5625rem 0 1.5625rem 1.5625rem; border-width: 0 0 0.0625rem 0; border-bottom: 0.0625rem #e2e3e4 solid; box-sizing: border-box;"><span style="font-size: 16px;">Chest</span></td>
            <td style="padding: 1.5625rem 0 1.5625rem 1.5625rem; border-width: 0 0 0.0625rem 0; border-bottom: 0.0625rem #e2e3e4 solid; box-sizing: border-box;"><span style="font-size: 16px;">35-37</span></td>
            <td style="padding: 1.5625rem 0 1.5625rem 1.5625rem; border-width: 0 0 0.0625rem 0; border-bottom: 0.0625rem #e2e3e4 solid; box-sizing: border-box;"><span style="font-size: 16px;">38-40</span></td>
            <td style="padding: 1.5625rem 0 1.5625rem 1.5625rem; border-width: 0 0 0.0625rem 0; border-bottom: 0.0625rem #e2e3e4 solid; box-sizing: border-box;"><span style="font-size: 16px;">41-43</span></td>
            <td style="padding: 1.5625rem 0 1.5625rem 1.5625rem; border-width: 0 0 0.0625rem 0; border-bottom: 0.0625rem #e2e3e4 solid; box-sizing: border-box;"><span style="font-size: 16px;">44-46</span></td>
            <td style="padding: 1.5625rem 0 1.5625rem 1.5625rem; border-width: 0 0 0.0625rem 0; border-bottom: 0.0625rem #e2e3e4 solid; box-sizing: border-box;"><span style="font-size: 16px;">47-49</span></td>
        </tr>
        <tr>
            <td style="padding: 1.5625rem 0 1.5625rem 1.5625rem; border-width: 0 0 0.0625rem 0; border-bottom: 0.0625rem #e2e3e4 solid; box-sizing: border-box;"><span style="font-size: 16px;">Sleeve</span></td>
            <td style="padding: 1.5625rem 0 1.5625rem 1.5625rem; border-width: 0 0 0.0625rem 0; border-bottom: 0.0625rem #e2e3e4 solid; box-sizing: border-box;"><span style="font-size: 16px;">32-33</span></td>
            <td style="padding: 1.5625rem 0 1.5625rem 1.5625rem; border-width: 0 0 0.0625rem 0; border-bottom: 0.0625rem #e2e3e4 solid; box-sizing: border-box;"><span style="font-size: 16px;">33-34</span></td>
            <td style="padding: 1.5625rem 0 1.5625rem 1.5625rem; border-width: 0 0 0.0625rem 0; border-bottom: 0.0625rem #e2e3e4 solid; box-sizing: border-box;"><span style="font-size: 16px;">34-35</span></td>
            <td style="padding: 1.5625rem 0 1.5625rem 1.5625rem; border-width: 0 0 0.0625rem 0; border-bottom: 0.0625rem #e2e3e4 solid; box-sizing: border-box;"><span style="font-size: 16px;">35-36</span></td>
            <td style="padding: 1.5625rem 0 1.5625rem 1.5625rem; border-width: 0 0 0.0625rem 0; border-bottom: 0.0625rem #e2e3e4 solid; box-sizing: border-box;"><span style="font-size: 16px;">36-36.5</span></td>
        </tr>
        <tr>
            <td style="padding: 1.5625rem 0 1.5625rem 1.5625rem; border-width: 0 0 0.0625rem 0; border-bottom: 0.0625rem #e2e3e4 solid; box-sizing: border-box;"><span style="font-size: 16px;">Waist</span></td>
            <td style="padding: 1.5625rem 0 1.5625rem 1.5625rem; border-width: 0 0 0.0625rem 0; border-bottom: 0.0625rem #e2e3e4 solid; box-sizing: border-box;"><span style="font-size: 16px;">29-31</span></td>
            <td style="padding: 1.5625rem 0 1.5625rem 1.5625rem; border-width: 0 0 0.0625rem 0; border-bottom: 0.0625rem #e2e3e4 solid; box-sizing: border-box;"><span style="font-size: 16px;">32-34</span></td>
            <td style="padding: 1.5625rem 0 1.5625rem 1.5625rem; border-width: 0 0 0.0625rem 0; border-bottom: 0.0625rem #e2e3e4 solid; box-sizing: border-box;"><span style="font-size: 16px;">35-37</span></td>
            <td style="padding: 1.5625rem 0 1.5625rem 1.5625rem; border-width: 0 0 0.0625rem 0; border-bottom: 0.0625rem #e2e3e4 solid; box-sizing: border-box;"><span style="font-size: 16px;">38-40</span></td>
            <td style="padding: 1.5625rem 0 1.5625rem 1.5625rem; border-width: 0 0 0.0625rem 0; border-bottom: 0.0625rem #e2e3e4 solid; box-sizing: border-box;"><span style="font-size: 16px;">41-43</span></td>
        </tr>
    </tbody>
</table>'

GO--
	
IF NOT EXISTS(SELECT 1
			  FROM sys.columns
			  WHERE object_id = OBJECT_ID(N'[Catalog].[SizeChartMap]'))
	BEGIN
		CREATE TABLE [Catalog].[SizeChartMap](
			[SizeChartId] [int] NOT NULL,
			[ObjId] [int] NOT NULL,
			[ObjType] [smallint] NOT NULL
		 CONSTRAINT [PK_SizeChartMap] PRIMARY KEY CLUSTERED 
		(
			[ObjId] ASC,
			[ObjType] ASC,
			[SizeChartId] ASC
		)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
		) ON [PRIMARY]
		ALTER TABLE [Catalog].[SizeChartMap] WITH CHECK ADD CONSTRAINT [FK_SizeChartMap_SizeChart] FOREIGN KEY([SizeChartId])
		REFERENCES [Catalog].[SizeChart] ([Id]) ON DELETE CASCADE
	END
	
GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Configuration.AdminStartPage.Default', 'По умолчанию'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Configuration.AdminStartPage.Default', 'By default'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Configuration.AdminStartPage.Desktop', 'Рабочий стол'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Configuration.AdminStartPage.Desktop', 'Desktop'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Configuration.AdminStartPage.Orders', 'Заказы'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Configuration.AdminStartPage.Orders', 'Orders'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Configuration.AdminStartPage.Leads', 'Лиды'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Configuration.AdminStartPage.Leads', 'Leads'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Configuration.AdminStartPage.Dashboard', 'Мои сайты'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Configuration.AdminStartPage.Dashboard', 'My Sites'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Configuration.AdminStartPage.Tasks', 'Задачи'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Configuration.AdminStartPage.Tasks', 'Tasks'

GO--

-- RENAME AdditionBonus

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Bonus].[Bonuses]') AND type in (N'U'))
BEGIN
CREATE TABLE [Bonus].[Bonuses](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CardId] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](250) NULL,
	[Amount] [money] NOT NULL,
	[StartDate] [date] NULL,
	[EndDate] [date] NULL,
	[Description] [nvarchar](500) NULL,
	[Status] [int] NOT NULL,
	[NotifiedAboutExpiry] [bit] NOT NULL,
	[CreateOn] [datetime] NOT NULL,
 CONSTRAINT [PK_Bonuses] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END

GO--

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[Bonus].[FK_Bonuses_Card]') AND parent_object_id = OBJECT_ID(N'[Bonus].[Bonuses]'))
ALTER TABLE [Bonus].[Bonuses]  WITH CHECK ADD  CONSTRAINT [FK_Bonuses_Card] FOREIGN KEY([CardId])
REFERENCES [Bonus].[Card] ([CardId])
ON DELETE CASCADE

GO--

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[Bonus].[FK_Bonuses_Card]') AND parent_object_id = OBJECT_ID(N'[Bonus].[Bonuses]'))
ALTER TABLE [Bonus].[Bonuses] CHECK CONSTRAINT [FK_Bonuses_Card]

GO--

SET IDENTITY_INSERT [Bonus].[Bonuses] ON 

INSERT INTO [Bonus].[Bonuses] 
([Id],[CardId],[Name],[Amount],[StartDate],[EndDate],[Description],[Status],[NotifiedAboutExpiry],[CreateOn])
SELECT [Id],[CardId],[Name],[Amount],[StartDate],[EndDate],[Description],[Status],ISNULL([NotifiedAboutExpiry], 0),
 ISNULL((SELECT MIN([CreateOn]) FROM [Bonus].[Transaction] WHERE [AdditionalBonusId] = [AdditionBonus].[Id]),GETDATE()) 
FROM [Bonus].[AdditionBonus]

SET IDENTITY_INSERT [Bonus].[Bonuses] OFF 

GO--

IF NOT EXISTS(SELECT 1 FROM sys.columns 
          WHERE Name = N'BonusId'
          AND Object_ID = Object_ID(N'Bonus.Transaction'))
BEGIN
	ALTER TABLE [Bonus].[Transaction] ADD
		[BonusId] [int] NULL    
END

GO--

UPDATE [Bonus].[Transaction]
   SET [BonusId] = [AdditionalBonusId]

GO--

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[Bonus].[FK_Transaction_Bonuses]') AND parent_object_id = OBJECT_ID(N'[Bonus].[Transaction]'))
ALTER TABLE [Bonus].[Transaction]  WITH CHECK ADD  CONSTRAINT [FK_Transaction_Bonuses] FOREIGN KEY([BonusId])
REFERENCES [Bonus].[Bonuses] ([Id])

GO--

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[Bonus].[FK_Transaction_Bonuses]') AND parent_object_id = OBJECT_ID(N'[Bonus].[Transaction]'))
ALTER TABLE [Bonus].[Transaction] CHECK CONSTRAINT [FK_Transaction_Bonuses]

GO--

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[Bonus].[FK_Transaction_AdditionBonus]') AND parent_object_id = OBJECT_ID(N'[Bonus].[Transaction]'))
ALTER TABLE [Bonus].[Transaction] DROP CONSTRAINT [FK_Transaction_AdditionBonus]

GO--

IF EXISTS(SELECT 1 FROM sys.columns 
          WHERE Name = N'AdditionalBonusId'
          AND Object_ID = Object_ID(N'Bonus.Transaction'))
BEGIN
	ALTER TABLE [Bonus].[Transaction] 
		DROP COLUMN AdditionalBonusId
END

GO--

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Bonus].[AdditionBonus]') AND type in (N'U'))
DROP TABLE [Bonus].[AdditionBonus]

-- END RENAME AdditionBonus

GO--

DECLARE @AddedBonuses TABLE([Id] [int] NOT NULL, PRIMARY KEY CLUSTERED ([Id]));

INSERT INTO [Bonus].[Bonuses] ([CardId],[Name],[Amount],[StartDate],[EndDate],[Description],[Status],[NotifiedAboutExpiry],[CreateOn])
OUTPUT INSERTED.[Id] INTO @AddedBonuses
SELECT [CardId], N'Основные бонусы', [BonusAmount],NULL,NULL,N'Переход к единым баллам (автоматическое действие).',0,0,GETDATE()
FROM [Bonus].[Card]
WHERE [BonusAmount] > 0;

DECLARE @Today date
SET @Today = CAST(GETDATE() AS DATE)

INSERT INTO [Bonus].[Transaction] ([CardId],[Amount],[Basis],[CreateOn],[CreateOnCut],[OperationType],[Balance],[PurchaseId],[BonusId])
SELECT [CardId],[Amount],'Переход к единым баллам (автоматическое действие). Основные баллы были списаны в связи с ликвидацией реестра основных баллов и начислены на единый бонусный счет.',[CreateOn],CAST([CreateOn] AS DATE),7,
(SELECT SUM(subb.Amount) FROM [Bonus].[Bonuses] subb WHERE subb.[CardId]=b.[CardId] and (subb.EndDate is null or subb.EndDate>=@Today) and (subb.StartDate is null or subb.StartDate<=@Today) and subb.Status <> 1),
NULL,b.[Id]
FROM [Bonus].[Bonuses] b INNER JOIN @AddedBonuses ad ON b.[Id] = ad.[Id]

GO--

IF EXISTS(SELECT 1 FROM sys.columns 
          WHERE Name = N'BonusAmount'
          AND Object_ID = Object_ID(N'Bonus.Card'))
BEGIN
	ALTER TABLE [Bonus].[Card] ALTER COLUMN
		[BonusAmount] [money] NULL
END

GO--

IF EXISTS(SELECT 1 FROM sys.columns 
          WHERE Name = N'BonusAmount'
          AND Object_ID = Object_ID(N'Bonus.Card'))
BEGIN
	EXEC sp_rename 'Bonus.Card.BonusAmount', 'BonusAmountObsolete', 'COLUMN';
END

GO--

IF NOT EXISTS(SELECT 1 FROM sys.columns 
          WHERE Name = N'BonusAmount'
          AND Object_ID = Object_ID(N'Bonus.Purchase'))
BEGIN
	ALTER TABLE [Bonus].[Purchase] ADD
		[BonusAmount] [money] NULL,
		[BonusBalance] [money] NULL
END

GO--

UPDATE [Bonus].[Purchase]
   SET [BonusAmount] = [MainBonusAmount] + [AdditionBonusAmount]
   ,[BonusBalance] = [MainBonusBalance] + [AdditionBonusBalance]

GO--

ALTER TABLE [Bonus].[Purchase] ALTER COLUMN
	[BonusAmount] [money] NOT NULL

GO--

ALTER TABLE [Bonus].[Purchase] ALTER COLUMN
	[BonusBalance] [money] NOT NULL

GO--

ALTER TABLE [Bonus].[Purchase] ALTER COLUMN
	[MainBonusAmount] [money] NULL

GO--

ALTER TABLE [Bonus].[Purchase] ALTER COLUMN
	[AdditionBonusAmount] [money] NULL

GO--

ALTER TABLE [Bonus].[Purchase] ALTER COLUMN
	[MainBonusBalance] [money] NULL

GO--

ALTER TABLE [Bonus].[Purchase] ALTER COLUMN
	[AdditionBonusBalance] [money] NULL

GO--

IF EXISTS(SELECT 1 FROM sys.columns 
          WHERE Name = N'MainBonusAmount'
          AND Object_ID = Object_ID(N'Bonus.Purchase'))
BEGIN
	EXEC sp_rename 'Bonus.Purchase.MainBonusAmount', 'MainBonusAmountObsolete', 'COLUMN';
END

GO--

IF EXISTS(SELECT 1 FROM sys.columns 
          WHERE Name = N'AdditionBonusAmount'
          AND Object_ID = Object_ID(N'Bonus.Purchase'))
BEGIN
	EXEC sp_rename 'Bonus.Purchase.AdditionBonusAmount', 'AdditionBonusAmountObsolete', 'COLUMN';
END

GO--

IF EXISTS(SELECT 1 FROM sys.columns 
          WHERE Name = N'MainBonusBalance'
          AND Object_ID = Object_ID(N'Bonus.Purchase'))
BEGIN
	EXEC sp_rename 'Bonus.Purchase.MainBonusBalance', 'MainBonusBalanceObsolete', 'COLUMN';
END

GO--

IF EXISTS(SELECT 1 FROM sys.columns 
          WHERE Name = N'AdditionBonusBalance'
          AND Object_ID = Object_ID(N'Bonus.Purchase'))
BEGIN
	EXEC sp_rename 'Bonus.Purchase.AdditionBonusBalance', 'AdditionBonusBalanceObsolete', 'COLUMN';
END

GO--

IF EXISTS(SELECT 1 FROM sys.columns 
          WHERE Name = N'DateLastWipeBonus'
          AND Object_ID = Object_ID(N'Bonus.Card'))
BEGIN
	EXEC sp_rename 'Bonus.Card.DateLastWipeBonus', 'DateLastWipeBonusObsolete', 'COLUMN';
END

GO--

IF EXISTS(SELECT 1 FROM sys.columns 
          WHERE Name = N'DateLastNotifyBonusWipe'
          AND Object_ID = Object_ID(N'Bonus.Card'))
BEGIN
	EXEC sp_rename 'Bonus.Card.DateLastNotifyBonusWipe', 'DateLastNotifyBonusWipeObsolete', 'COLUMN';
END

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.AdditionBonus.AccrueAdditionalBonuses', 'Начислить бонусы'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.AdditionBonus.AccrueAdditionalBonuses', 'Accrue bonuses'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Cards.AdditionBonus.AdditionBonuses', 'Бонусы'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Cards.AdditionBonus.AdditionBonuses', 'Bonuses'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Cards.LastPurchase.PaidWithBonuses', 'Оплачено бонусами'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Cards.LastPurchase.PaidWithBonuses', 'Paid in bonuses'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Cards.LastTransaction.Points', 'Баллы'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Cards.LastTransaction.Points', 'Points'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Cards.Edit.AdditionBonuses', 'Бонусы'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Cards.Edit.AdditionBonuses', 'Bonuses'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.AdditionBonus.WriteOffAdditionalBonuses', 'Списать бонусы'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.AdditionBonus.WriteOffAdditionalBonuses', 'Write off bonuses'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Cards.AllPurchase.PaidInBonuses', 'Оплачено бонусами'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Cards.AllPurchase.PaidInBonuses', 'Paid in bonuses'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Cards.AllTransaction.Points', 'Баллы'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Cards.AllTransaction.Points', 'Points'

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Bonuses.OnAddBonusTempalte.CompanyName', 'Название магазина'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Bonuses.OnAddBonusTempalte.CompanyName', 'Store Name'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Bonuses.OnAddBonusTempalte.Bonus', 'Начислено бонусов'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Bonuses.OnAddBonusTempalte.Bonus', 'Bonuses accrued'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Bonuses.OnAddBonusTempalte.Balance', 'Бонусный баланс'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Bonuses.OnAddBonusTempalte.Balance', 'Bonus balance'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Bonuses.OnAddBonusTempalte.BalanceWithNewBonus', 'Бонусный баланс с учетом новых бонусов (будет отличаться от Balance, если бонусы действует не сразу, например, с завтра)'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Bonuses.OnAddBonusTempalte.BalanceWithNewBonus', 'Bonus balance including new bonuses (will be different from Balance if bonuses are not valid immediately, for example, from tomorrow)'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Bonuses.OnAddBonusTempalte.Basis', 'Основание'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Bonuses.OnAddBonusTempalte.Basis', 'Base'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Bonuses.OnPurchaseTempalte.CompanyName', 'Название магазина'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Bonuses.OnPurchaseTempalte.CompanyName', 'Store Name'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Bonuses.OnPurchaseTempalte.PurchaseFull', 'Стоимость заказа'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Bonuses.OnPurchaseTempalte.PurchaseFull', 'Order cost'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Bonuses.OnPurchaseTempalte.Purchase', 'Стоимость заказа с учетом скидки'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Bonuses.OnPurchaseTempalte.Purchase', 'Order cost with a discount'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Bonuses.OnPurchaseTempalte.UsedBonus', 'Оплачено бонусами'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Bonuses.OnPurchaseTempalte.UsedBonus', 'Paid in bonuses'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Bonuses.OnPurchaseTempalte.AddBonus', 'Начислено бонусов'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Bonuses.OnPurchaseTempalte.AddBonus', 'Bonuses accrued'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Bonuses.OnPurchaseTempalte.Balance', 'Бонусный баланс'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Bonuses.OnPurchaseTempalte.Balance', 'Bonus balance'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Bonuses.OnSubtractBonusTempalte.CompanyName', 'Название магазина'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Bonuses.OnSubtractBonusTempalte.CompanyName', 'Store Name'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Bonuses.OnSubtractBonusTempalte.Bonus', 'Списано бонусов'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Bonuses.OnSubtractBonusTempalte.Bonus', 'Deducted bonuses'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Bonuses.OnSubtractBonusTempalte.Balance', 'Бонусный баланс'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Bonuses.OnSubtractBonusTempalte.Balance', 'Bonus balance'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Bonuses.OnSubtractBonusTempalte.Basis', 'Основание'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Bonuses.OnSubtractBonusTempalte.Basis', 'Base'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Bonuses.OnUpgradePercentTempalte.CardNumber', 'Номер бонусной карты'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Bonuses.OnUpgradePercentTempalte.CardNumber', 'Bonus card number'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Bonuses.OnUpgradePercentTempalte.CompanyName', 'Название магазина'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Bonuses.OnUpgradePercentTempalte.CompanyName', 'Store Name'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Bonuses.OnUpgradePercentTempalte.GradeName', 'Название грейда'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Bonuses.OnUpgradePercentTempalte.GradeName', 'Grade name'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Bonuses.OnUpgradePercentTempalte.GradePercent', 'Процент грейда'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Bonuses.OnUpgradePercentTempalte.GradePercent', 'Grade percent'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Bonuses.OnUpgradePercentTempalte.Balance', 'Бонусный баланс'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Bonuses.OnUpgradePercentTempalte.Balance', 'Bonus balance'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Bonuses.CancelPurchaseTempalte.Balance', 'Бонусный баланс'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Bonuses.CancelPurchaseTempalte.Balance', 'Bonus balance'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Bonuses.OnBirthdayRuleTempalte.CompanyName', 'Название магазина'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Bonuses.OnBirthdayRuleTempalte.CompanyName', 'Store Name'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Bonuses.OnBirthdayRuleTempalte.AddBonus', 'Начислено бонусов'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Bonuses.OnBirthdayRuleTempalte.AddBonus', 'Bonuses accrued'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Bonuses.OnBirthdayRuleTempalte.ToDate', 'Действуют по дату'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Bonuses.OnBirthdayRuleTempalte.ToDate', 'Valid by date'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Bonuses.OnBirthdayRuleTempalte.Balance', 'Бонусный баланс'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Bonuses.OnBirthdayRuleTempalte.Balance', 'Bonus balance'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Bonuses.OnBirthdayRuleTempalte.Unlimited', 'неограниченно'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Bonuses.OnBirthdayRuleTempalte.Unlimited', 'unlimited'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Bonuses.OnCancellationsBonusTempalte.CompanyName', 'Название магазина'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Bonuses.OnCancellationsBonusTempalte.CompanyName', 'Store Name'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Bonuses.OnCancellationsBonusTempalte.Balance', 'Бонусный баланс'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Bonuses.OnCancellationsBonusTempalte.Balance', 'Bonus balance'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Bonuses.OnCancellationsBonusTempalte.DayLeft', 'Кол-во дней до списания'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Bonuses.OnCancellationsBonusTempalte.DayLeft', 'Number of days until write-off'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Bonuses.OnCleanExpiredBonusTempalte.CompanyName', 'Название магазина'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Bonuses.OnCleanExpiredBonusTempalte.CompanyName', 'Store Name'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Bonuses.OnCleanExpiredBonusTempalte.Balance', 'Бонусный баланс'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Bonuses.OnCleanExpiredBonusTempalte.Balance', 'Bonus balance'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Bonuses.OnCleanExpiredBonusTempalte.DayLeft', 'Кол-во дней до списания'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Bonuses.OnCleanExpiredBonusTempalte.DayLeft', 'Number of days until write-off'

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Bonuses.OnCleanExpiredBonusTempalte.CleanBonuses', 'Бонусов будет списано'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Bonuses.OnCleanExpiredBonusTempalte.CleanBonuses', 'Bonuses will be deducted'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Bonuses.OnCancellationsBonusTempalte.CleanBonuses', 'Бонусов будет списано'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Bonuses.OnCancellationsBonusTempalte.CleanBonuses', 'Bonuses will be deducted'

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Rules.BirthDay.AddAdditionalBonuses', 'Начислить бонусы на карту'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Rules.BirthDay.AddAdditionalBonuses', 'Add bonuses to the card'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Rules.BirthDay.BonusesValid', 'Бонусы должны действовать в течение (дней)'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Rules.BirthDay.BonusesValid', 'Bonuses must be valid for (days)'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Rules.NewCard.AccrueAdditionalBonuses', 'Начислить бонусы на карту'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Rules.NewCard.AccrueAdditionalBonuses', 'Accrue card bonuses'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Rules.NewCard.AdditionalBonuses', 'Бонусы должны действовать в течение (дней)'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Rules.NewCard.AdditionalBonuses', 'Bonuses must be valid for (days)'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Rules.PostingReview.AccrueAdditionalBonuses', 'Начислить бонусы на карту'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Rules.PostingReview.AccrueAdditionalBonuses', 'Accrue card bonuses'

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Cards.CardCommon.BonusesAmount', 'Кол-во активных бонусов'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Cards.CardCommon.BonusesAmount', 'Number of active bonuses'

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.NotificationTemplates.Error.TemplateExist', 'Шаблон с таким типом уже существует'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.NotificationTemplates.Error.TemplateExist', 'Template with this type already exists'

GO--

UPDATE [Bonus].[Bonuses]
   SET [Amount] = 0
 WHERE [Status] = 1
   
GO--

DELETE FROM [Bonus].[NotificationTemplate] WHERE [NotificationTypeId] = 8

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.ExportImport.EProductFields.SizeChart', 'Таблица размеров'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.ExportImport.EProductFields.SizeChart', 'Size chart'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.ExportImport.ProductFields.SizeChart', 'Таблица размеров'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.ExportImport.ProductFields.SizeChart', 'Size chart'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.ExportImport.CategoryFields.SizeChart', 'Таблица размеров'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.ExportImport.CategoryFields.SizeChart', 'Size chart'

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.SettingsApiAuth.MobileAppSalesMode.Shop', 'Интернет магазин'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.SettingsApiAuth.MobileAppSalesMode.Shop', 'Online store'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.SettingsApiAuth.MobileAppSalesMode.Resto', 'Доставка еды'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.SettingsApiAuth.MobileAppSalesMode.Resto', 'Food delivery'
   
GO--

ALTER PROCEDURE [Catalog].[sp_ParseProductProperty]      
  @nameProperty nvarchar(100),      
  @propertyValue nvarchar(255),      
  @rangeValue float,    
  @productId int,      
  @sort int      
AS      
BEGIN      
 -- select or create property      
 Declare @propertyId int      
 if ((select count(PropertyID) from Catalog.[Property] where Name = @nameProperty)= 0)      
  begin      
   insert into Catalog.[Property] (Name,UseInFilter,UseInBrief,Useindetails,SortOrder,[type], NameDisplayed) values (@nameProperty,0,0,0,0,1, @nameProperty)      
   set @propertyId = (Select SCOPE_IDENTITY())      
  end      
 else      
  set @propertyId = (select top(1) PropertyID from Catalog.[Property] where Name = @nameProperty)      
      
  -- select or create value      
  Declare @propertyValueId int      
      
  Declare @useinfilter bit      
  set @useinfilter = (Select Top 1 UseInFilter from Catalog.[Property] Where PropertyID=@propertyId)      
  Declare @useindetails bit      
  set @useindetails = (Select Top 1 UseInDetails from Catalog.[Property] Where PropertyID=@propertyId)      
      
  if ((select count(PropertyValueID) from Catalog.[PropertyValue] where Value = @propertyValue and PropertyId=@propertyId)= 0)      
   begin      
    insert into Catalog.[PropertyValue] (PropertyId, Value, UseInFilter, UseInDetails, SortOrder, RangeValue) values (@propertyId, @propertyValue, @useinfilter, @useindetails, 0, @rangeValue)      
    set @propertyValueId = (Select SCOPE_IDENTITY())      
   end      
  else      
   set @propertyValueId = (select top(1) PropertyValueID from Catalog.[PropertyValue] where Value = @propertyValue and PropertyId=@propertyId)      
       
 --create link between product and property value      
 if ((select Count(*) from Catalog.ProductPropertyValue where ProductID=@productId and PropertyValueID=@propertyValueId)=0)      
  insert into Catalog.ProductPropertyValue (ProductID,PropertyValueID) values (@productId,@propertyValueId)       
END 

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Payment.Receipt.ShippingName', 'Доставка'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Payment.Receipt.ShippingName', 'Delivery'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Payment.Receipt.GiftCertificateName', 'Подарочный сертификат'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Payment.Receipt.GiftCertificateName', 'Gift certificate'

GO--

IF NOT EXISTS(SELECT * FROM [Settings].[SettingsSearch] WHERE Title = N'Оценка заказа' AND Link = 'settingscheckout?checkoutTab=common')
  INSERT INTO [Settings].[SettingsSearch] (Title, Link, KeyWords, SortOrder) VALUES (N'Оценка заказа', 'settingscheckout?checkoutTab=common', N'оценка заказа, оценка заказов, рейтинг',0)

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Grades.DeleteGradeHandler.Error.ItDefault', 'Нельзя удалить грейд по умолчанию'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Grades.DeleteGradeHandler.Error.ItDefault', 'Can not delete the default grade'

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Import.ExportCustomers.Title', 'Экспорт покупателей'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Import.ExportCustomers.Title', 'Export customers'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.ExportImport.CustomerFields.RegistrationDateTime', 'Дата регистрации'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.ExportImport.CustomerFields.RegistrationDateTime', 'Registration time'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.ExportImport.CustomerFields.District', 'Район'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.ExportImport.CustomerFields.District', 'District'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.ExportImport.CustomerFields.Apartment', 'Квартира'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.ExportImport.CustomerFields.Apartment', 'Apartment'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.ExportImport.CustomerFields.Structure', 'Строение'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.ExportImport.CustomerFields.Structure', 'Structure'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.ExportImport.CustomerFields.Entrance', 'Подъезд'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.ExportImport.CustomerFields.Entrance', 'Entrance'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.ExportImport.CustomerFields.Floor', 'Этаж'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.ExportImport.CustomerFields.Floor', 'Floor'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.ExportImport.CustomerFields.BonusCard', 'Бонусная карта'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.ExportImport.CustomerFields.BonusCard', 'Bonus card'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.ExportCustomers.ExportCustomersTitle', 'Экспорт покупателей'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.ExportCustomers.ExportCustomersTitle', 'Export customers'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.ExportCustomers.RegistrationDateInterval', 'Дата регистрации'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.ExportCustomers.RegistrationDateInterval', 'Registration date'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.ExportCustomers.PaidOrdersCount', 'Кол-во оплаченных заказов'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.ExportCustomers.PaidOrdersCount', 'Paid orders count'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.ExportCustomers.PaidOrderSum', 'Сумма оплаченных заказов'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.ExportCustomers.PaidOrderSum', 'Paid orders sum'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.ExportCustomers.LastOrder', 'Последний заказ'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.ExportCustomers.LastOrder', 'Last order'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.ExportCustomers.LastOrderDateTime', 'Дата последнего заказа'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.ExportCustomers.LastOrderDateTime', 'Last order date'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.ExportCustomers.AverageCheck', 'Средний чек'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.ExportCustomers.AverageCheck', 'Average check'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.ExportImport.CustomerFields.LastOrder', 'Номер последнего заказ'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.ExportImport.CustomerFields.LastOrder', 'Last order number'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.ExportImport.CustomerFields.PaidOrdersCount', 'Кол-во оплаченных заказов'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.ExportImport.CustomerFields.PaidOrdersCount', 'Paid orders count'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.ExportImport.CustomerFields.PaidOrdersSum', 'Сумма оплаченных заказов'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.ExportImport.CustomerFields.PaidOrdersSum', 'Paid orders sum'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.ExportCustomers.SocialType', 'Есть аккаунт ВКонтакте, Instagram, Telegram'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.ExportCustomers.SocialType', 'Have a VKontakte, Instagram, Telegram account'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.ExportCustomers.HasBonusCard', 'Есть бонусная карта'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.ExportCustomers.HasBonusCard', 'Has bonus card'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.ExportCustomers.Subscription', 'Есть подписка на новости'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.ExportCustomers.Subscription', 'Has subscription'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.ExportCustomers.Manager', 'Менеджер'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.ExportCustomers.Manager', 'Manager'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.ExportCustomers.CustomerType', 'Тип покупателя'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.ExportCustomers.CustomerType', 'Customer type'

GO--

ALTER TABLE CMS.StaticPageApi ADD
	IconName nvarchar(MAX) NULL
GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.ExportFeed.SettingsYandex.ConsiderMultiplicityInPrice', 'Выгружать цену с учетом кратности товара'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.ExportFeed.SettingsYandex.ConsiderMultiplicityInPrice', 'Exporte price taking into account the multiplicity of the product'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.ExportFeed.SettingsYandex.ConsiderMultiplicityInPriceHelp', 'Например, если товар продается по 5 шт., то отобразится цена за комплект. Также в названии товара будет указано количество товара в комплекте.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.ExportFeed.SettingsYandex.ConsiderMultiplicityInPriceHelp', 'For example, if the product is sold for 5 pieces, the price per set will be displayed. The product name will also indicate the quantity of the product in the package.'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.ExportImport.Yandex.DefaultUnitName', 'шт.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.ExportImport.Yandex.DefaultUnitName', 'pc.'

GO--
IF NOT EXISTS(SELECT TOP 1 * FROM sys.columns 
				WHERE name = 'ShowInUserEditing' 
				AND object_id = OBJECT_ID(N'[Customers].[CustomerField]'))
BEGIN
	ALTER TABLE [Customers].[CustomerField]
	ADD [ShowInUserEditing] bit NULL
END

GO--

UPDATE [Customers].[CustomerField]
SET [ShowInUserEditing] = 0

GO--

IF EXISTS(SELECT TOP 1 * FROM sys.columns 
				WHERE name = 'ShowInUserEditing' 
				AND object_id = OBJECT_ID(N'[Customers].[CustomerField]'))
BEGIN
	ALTER TABLE [Customers].[CustomerField]
	ALTER COLUMN ShowInUserEditing bit NOT NULL
END

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.AddEditCustomerField.ShowInUserEditing', 'Выводить в редактировании сотрудника'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.AddEditCustomerField.ShowInUserEditing', 'Show in user editing'

GO--

IF ((SELECT COUNT(*) FROM [Catalog].[Units]) = 0)
BEGIN
	INSERT INTO [Catalog].[Units] ([Name], [DisplayName], [MeasureType], [SortOrder], [DateAdded], [DateModified])
	VALUES
		('шт.', 'шт.', 0, 0, GETDATE(), GETDATE()),
		('гр.', 'гр.', 10, 10, GETDATE(), GETDATE()),
		('кг.', 'кг.', 11, 20, GETDATE(), GETDATE()),
		('мл.', 'мл.', 40, 30, GETDATE(), GETDATE()),
		('л.', 'л.', 41, 40, GETDATE(), GETDATE())
END

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.AddEditRegions.Country', 'Страна'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.AddEditRegions.Country', 'Country'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.AddEditRegions.SortOrder', 'Порядок сортировки'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.AddEditRegions.SortOrder', 'Sort order'

GO--

declare @rusCountryId int
declare @ukrCountryId int

set @rusCountryId = (SELECT TOP(1) CountryID FROM [Customers].[Country] WHERE [CountryName] = 'Россия')
set @ukrCountryId = (SELECT TOP(1) CountryID FROM [Customers].[Country] WHERE [CountryName] = 'Украина')

IF @rusCountryId IS NOT NULL AND @ukrCountryId IS NOT NULL AND (SELECT TOP(1) CountryID FROM [Customers].[Region] WHERE [RegionName] = 'Донецкая область') = @ukrCountryId
BEGIN 
	UPDATE [Customers].[City] SET DisplayInPopup = 0 
	WHERE [CityName] IN ('Запорожье','Херсон','Луганск','Донецк','Мариуполь') AND (SELECT TOP(1) CountryID FROM [Customers].[Region] WHERE [Region].RegionID = [City].[RegionID]) = @ukrCountryId

	UPDATE [Customers].[City] SET [CityName] = 'Кропивницкий' WHERE [CityName] = 'Кировоград'
	UPDATE [Customers].[City] SET DisplayInPopup = 1 
	WHERE [CityName] IN ('Ровно','Кропивницкий','Ивано-Франковск','Кременчуг','Тернополь') AND (SELECT TOP(1) CountryID FROM [Customers].[Region] WHERE [Region].RegionID = [City].[RegionID]) = @ukrCountryId
	
	UPDATE [Customers].[Region] SET RegionName = 'Ровненская область' WHERE [RegionName] = 'Ровенская область'
	UPDATE [Customers].[Region] SET RegionName = 'Донецкая Народная Республика', CountryID = @rusCountryId WHERE [RegionName] = 'Донецкая область' AND [CountryID] = @ukrCountryId
	UPDATE [Customers].[Region] SET RegionName = 'Луганская Народная Республика', CountryID = @rusCountryId WHERE [RegionName] = 'Луганская область' AND [CountryID] = @ukrCountryId
	UPDATE [Customers].[Region] SET CountryID = @rusCountryId WHERE [RegionName] IN ('Запорожская область','Херсонская область') AND [CountryID] = @ukrCountryId
END

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.Builder.SizesControlTypes', N'Режим отображания для характеристики "Размер"'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.Builder.SizesControlTypes', 'Display mode for the "Size" characteristic'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.Builder.ColorsControlTypes', N'Режим отображания для характеристики "Цвет"'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.Builder.ColorsControlTypes', 'Display mode for the "Color" characteristic'

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.AddEditCarousel.Size', N'Рекомендуемые размеры изображения для десктопной версии:<br>1160px * 553px - одна колонка,<br>865px * 400px - две колонки'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.AddEditCarousel.Size', 'Recommended image sizes for desktop version:<br>1160px * 553px - one column,<br>865px * 400px - two speakers'

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.ShippingMethods.DeliveryByZones.WithoutZoneMessage', 'Текст стоимости, когда зона не определена'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.ShippingMethods.DeliveryByZones.WithoutZoneMessage', 'The cost text when the zone is not defined.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.ShippingMethods.DeliveryByZones.WithoutZoneMessageHelp', 'Текст, который будет показан вместо стоимости доставки, когда зона не определена. Например, когда адрес не указан, не распознан, не входит ни в одну из зон и т.д.<br/>Оставьте поле пустым и тогда будет браться одна из зон, для отображения цены.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.ShippingMethods.DeliveryByZones.WithoutZoneMessageHelp', 'The text that will be shown instead of the shipping cost when the zone is not defined. For example, when the address is not specified, is not recognized, does not belong to any of the zones, etc.<br/>Leave the field empty and then one of the zones will be taken to display the price.'

GO--

if exists (Select 1 From [dbo].[Modules] Where [ModuleStringID] in ('Ozon', 'Wildberries', 'AliExpress', 'Lamoda') and Active = 1) 
begin
	if exists (Select 1 From [Settings].[Settings] Where Name = 'IsTechDomainPicturesAllowed')
		Update [Settings].[Settings] Set [Value] = 'True' Where Name = 'IsTechDomainPicturesAllowed' 
	else 
		Insert Into [Settings].[Settings] (Name, Value) Values ('IsTechDomainPicturesAllowed', 'True')
end

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Checkout.CheckoutUser.Email', 'E-mail'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Checkout.CheckoutUser.Email', 'E-mail'

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Customergroups.CanNotDeleteDefaultGroup', 'Нельзя удалять группу пользователей по-умолчанию и группу, в которой есть пользователи'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Customergroups.CanNotDeleteDefaultGroup', 'You cannot delete the default user group and the group in which there are users'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Customergroups.UsedForCustomers', 'Используется у покупателей'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Customergroups.UsedForCustomers', 'Used for customers'

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1,'Admin.Settings.PriceRegulation.ChangeDiscountMsg', 'Для товаров в выбранных категориях была установлена скидка {0}{1}'
EXEC [Settings].[sp_AddUpdateLocalization] 2,'Admin.Settings.PriceRegulation.ChangeDiscountMsg', 'Discount {0}{1} was set for products in the selected categories'

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1,'Admin.Settings.PriceRegulation.ChangeDiscountByBrandMsg', 'Для товаров выбранного производителя была установлена скидка {0}{1}'
EXEC [Settings].[sp_AddUpdateLocalization] 2,'Admin.Settings.PriceRegulation.ChangeDiscountByBrandMsg', 'Discount {0}{1} was set for products in the selected brand'

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1,'Admin.Settings.Checkout.EnableGiftCertificateServiceInfo', 'Опция определяет, включить или выключить возможность покупки и использования подарочного сертификата.<br />Для возможности использования подарочного сертификата при оформлении заказа, обязательно включить дополнительно настройку "<b>Отображать поле ввода купона или сертификата</b>"'
EXEC [Settings].[sp_AddUpdateLocalization] 2,'Admin.Settings.Checkout.EnableGiftCertificateServiceInfo', 'The option determines, whether to enable or disable the ability to purchase and use a gift certificate. <br /> To be able to use a gift certificate when placing an order, be sure to additionally enable the option "<b>Display coupon or certificate input field</b>"'

EXEC [Settings].[sp_AddUpdateLocalization] 1,'Admin.Settings.Checkout.DisplayPromoTextboxInfo', 'Опция определяет, включить или выключить возможность использования купона и подарочного сертификата при оформлении заказа <br />Для возможности использования подарочного сертификата при оформлении заказа, обязательно включить дополнительно настройку "<b>Разрешить использование подарочных сертификатов</b>"'
EXEC [Settings].[sp_AddUpdateLocalization] 2,'Admin.Settings.Checkout.DisplayPromoTextboxInfo', 'The option determines, whether to enable or disable the ability to use a coupon and a gift certificate when placing an order. <br /> To be able to use a gift certificate when placing an order, be sure to additionally enable the "<b>Allow the use of gift certificates</b>" setting'

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Product.OneAvailableInWarehouse', N' на {0} складe'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Product.OneAvailableInWarehouse', ' in {0} warehouse'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.CustomOption.CustomOptionInputType.MultiCkeckBox', 'Галочка (мультивыбор)'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.CustomOption.CustomOptionInputType.MultiCkeckBox', 'Multi checkbox'

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.AddDeliveryZone.ZeroPriceMessage', 'Текст при нулевой стоимости'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.AddDeliveryZone.ZeroPriceMessage', 'Zero price message'

GO--

INSERT INTO [Settings].[TemplateSettings] ([Template] ,[Name] ,[Value]) VALUES ('_default', 'Mobile_ProductImageType', '2')
           
GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.MobileVersion.PhotoTypeImage', 'Тип размера фотографии'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.MobileVersion.PhotoTypeImage', 'Photo type image'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.FilePath.ProductViewMode.Big', 'Большой'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.FilePath.ProductViewMode.Big', 'Big'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.FilePath.ProductViewMode.Middle', 'Средний'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.FilePath.ProductViewMode.Middle', 'Middle'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.FilePath.ProductViewMode.Small', 'Маленький'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.FilePath.ProductViewMode.Small', 'Small'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.FilePath.ProductViewMode.XSmall', 'Самый маленький'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.FilePath.ProductViewMode.XSmall', 'XSmall'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.FilePath.ProductViewMode.Original', 'Оригинальный'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.FilePath.ProductViewMode.Original', 'Original'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.FilePath.ProductViewMode.Rotate', 'Вращаемый'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.FilePath.ProductViewMode.Rotate', 'Rotate'

GO--

UPDATE [CMS].[StaticBlock]
SET [Content] = REPLACE(
                    REPLACE(
                        REPLACE([Content], 'src="./images/payment/mastercard_icon.svg"', 'src="./images/payment/mastercard_icon.svg" width="40" height="28"'),
                        'src="./images/payment/visa_icon.svg"', 'src="./images/payment/visa_icon.svg" width="70" height="30"'),
                        'src="./images/payment/mir-logo.svg"', 'src="./images/payment/mir-logo.svg" width="70" height="30"')
WHERE [Key] = 'RightBottom';

GO--


IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Catalog].[DiscountByTime]') AND type in (N'U'))
BEGIN
	CREATE TABLE [Catalog].[DiscountByTime](
		[Id] [int] IDENTITY(1,1) NOT NULL,
		[Enabled] [bit] NOT NULL,
		[TimeFrom] [datetime] NOt NULL,
		[TimeTo] [datetime] NOT NULL,
		[Discount] [float] NOT NULL,
		[ShowPopup] [bit] NOT NULL,
		[PopupText] [nvarchar] (max) NULL,
		[SortOrder] [int] NOT NULL
	 CONSTRAINT [PK_DiscountByTime] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]
END

GO--

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Catalog].[DiscountByTimeCategory]') AND type in (N'U'))
BEGIN
	CREATE TABLE [Catalog].[DiscountByTimeCategory](
		[DiscountByTimeId] [int] NOT NULL,
		[CategoryId] [int] NOT NULL,
		[ApplyDiscount] [bit] NOT NULL,
		[ActiveByTime] [bit] NOT NULL
	 CONSTRAINT [PK_DiscountByTimeCategory] PRIMARY KEY CLUSTERED 
	(
		[DiscountByTimeId] ASC,
		[CategoryId] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]

	ALTER TABLE [Catalog].[DiscountByTimeCategory]  WITH CHECK ADD CONSTRAINT [FK_DiscountByTimeCategory_DistountByTime] FOREIGN KEY([DiscountByTimeId])
	REFERENCES [Catalog].[DiscountByTime] ([Id])
	ON DELETE CASCADE
	
	ALTER TABLE [Catalog].[DiscountByTimeCategory]  WITH CHECK ADD CONSTRAINT [FK_DiscountByTimeCategory_Category] FOREIGN KEY([CategoryId])
	REFERENCES [Catalog].[Category] ([CategoryID])
	ON DELETE CASCADE
END

GO--

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Catalog].[DiscountByTimeDayOfWeek]') AND type in (N'U'))
BEGIN
	CREATE TABLE [Catalog].[DiscountByTimeDayOfWeek](
		[DiscountByTimeId] [int] NOT NULL,
		[DayOfWeek] [tinyint] NOT NULL
	 CONSTRAINT [PK_DiscountByTimeDayOfWeek] PRIMARY KEY CLUSTERED 
	(
		[DiscountByTimeId] ASC,
		[DayOfWeek] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]

	ALTER TABLE [Catalog].[DiscountByTimeDayOfWeek]  WITH CHECK ADD CONSTRAINT [FK_DiscountByTimeDayOfWeek_DistountByTime] FOREIGN KEY([DiscountByTimeId])
	REFERENCES [Catalog].[DiscountByTime] ([Id])
	ON DELETE CASCADE
END

GO--

IF EXISTS (SELECT * FROM [Settings].[Settings] WHERE [Name] LIKE 'SettingsDiscountByTime%')
BEGIN
	DECLARE @Enabled bit
	SET @Enabled = CAST((SELECT TOP(1) [Value] FROM [Settings].[Settings] WHERE [Name] = 'SettingsDiscountByTime_Enabled') AS BIT)
	DECLARE @TimeFrom datetime
	SET @TimeFrom = CAST('1/1/2024' as datetime) + CAST(CONVERT(DATETIME, (SELECT TOP(1) [Value] FROM [Settings].[Settings] WHERE [Name] = 'SettingsDiscountByTime_FromDateTime'), 101) AS TIME)
	DECLARE @TimeTo datetime
	SET @TimeTo = CAST('1/1/2024' as datetime) + CAST(CONVERT(DATETIME, (SELECT TOP(1) [Value] FROM [Settings].[Settings] WHERE [Name] = 'SettingsDiscountByTime_ToDateTime'), 101) AS TIME)
	DECLARE @Discount float
	SET @Discount = CAST((SELECT TOP(1) REPLACE([Value], ',', '.') FROM [Settings].[Settings] WHERE [Name] = 'SettingsDiscountByTime_DiscountByTime') AS FLOAT)
	DECLARE @ShowPopup bit
	SET @ShowPopup = CAST((SELECT TOP(1) [Value] FROM [Settings].[Settings] WHERE [Name] = 'SettingsDiscountByTime_ShowPopup') AS BIT)
	DECLARE @PopupText nvarchar(max)
	SET @PopupText = (SELECT TOP(1) [Value] FROM [Settings].[Settings] WHERE [Name] = 'SettingsDiscountByTime_PopupText')

	DECLARE @DiscountByTimeId int
	
	INSERT INTO [Catalog].[DiscountByTime] ([Enabled], [TimeFrom], [TimeTo], [Discount], [ShowPopup], [PopupText], [SortOrder])
		VALUES (@Enabled, @TimeFrom, @TimeTo, @Discount, @ShowPopup, @PopupText, 0) 
	SET @DiscountByTimeId  = (SELECT SCOPE_IDENTITY());

	INSERT INTO [Catalog].[DiscountByTimeDayOfWeek] ([DiscountByTimeId], [DayOfWeek])
		VALUES 
			(@DiscountByTimeId, 1),
			(@DiscountByTimeId, 2),
			(@DiscountByTimeId, 3),
			(@DiscountByTimeId, 4),
			(@DiscountByTimeId, 5),
			(@DiscountByTimeId, 6),
			(@DiscountByTimeId, 0)

	DECLARE @ActiveByTimeCategories nvarchar(max)
	SET @ActiveByTimeCategories = (SELECT TOP(1) [Value] FROM [Settings].[Settings] WHERE [Name] = 'SettingsDiscountByTime_ActiveByTimeCategories')
	DECLARE @DiscountCategories nvarchar(max)
	SET @DiscountCategories = (SELECT TOP(1) [Value] FROM [Settings].[Settings] WHERE [Name] = 'SettingsDiscountByTime_DiscountCategories')

	declare @startPos int
	set @startPos = 1
	declare @endPos int
	set @endPos = 2
	declare @categoryId int
 
    while @startPos <= len(@DiscountCategories)
    BEGIN
		if ((SELECT SUBSTRING(@DiscountCategories, @endPos, 1)) = ',' OR @endPos > len(@DiscountCategories))
		BEGIN
			set @categoryId = CAST((SELECT SUBSTRING(@DiscountCategories, @startPos, @endPos - @startPos)) AS INT)
			IF EXISTS(SELECT * FROM [Catalog].[Category] WHERE [CategoryID] = @categoryId)
				INSERT INTO [Catalog].[DiscountByTimeCategory] ([DiscountByTimeId], [CategoryId], [ApplyDiscount], [ActiveByTime])
					VALUES (@DiscountByTimeId, @categoryId, 1, 0)
			set @endPos = @endPos + 1
			set @startPos = @endPos
		END
		set @endPos = @endPos + 1
    END

	set @startPos = 1
	set @endPos = 2
	
    while @startPos <= len(@ActiveByTimeCategories)
    BEGIN
		if ((SELECT SUBSTRING(@ActiveByTimeCategories, @endPos, 1)) = ',' OR @endPos > len(@ActiveByTimeCategories))
		BEGIN
			set @categoryId = CAST((SELECT SUBSTRING(@ActiveByTimeCategories, @startPos, @endPos - @startPos)) AS INT)
			IF EXISTS(SELECT * FROM [Catalog].[Category] WHERE [CategoryID] = @categoryId)
			BEGIN
				IF EXISTS (SELECT * FROM [Catalog].[DiscountByTimeCategory] WHERE [CategoryId] = @categoryId AND DiscountByTimeId = @DiscountByTimeId)
					UPDATE [Catalog].[DiscountByTimeCategory] SET [ActiveByTime] = 1 WHERE [CategoryId] = @categoryId AND DiscountByTimeId = @DiscountByTimeId
				ELSE
					INSERT INTO [Catalog].[DiscountByTimeCategory] ([DiscountByTimeId], [CategoryId], [ApplyDiscount], [ActiveByTime])
						VALUES (@DiscountByTimeId, @categoryId, 0, 1)
			END
			set @endPos = @endPos + 1
			set @startPos = @endPos
		END
		set @endPos = @endPos + 1
    END

	DELETE FROM [Settings].[Settings] WHERE [Name] LIKE 'SettingsDiscountByTime%'
END

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.DiscountByDatetime.SortOrder', N'Порядок сортировки'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.DiscountByDatetime.SortOrder', 'Sort order'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.DiscountByTime.Discount', N'Скидка, %'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.DiscountByTime.Discount', 'Discount, %'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.DiscountByTime.Time', N'Время'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.DiscountByTime.Time', 'Time'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.DiscountByTime.Enabled', N'Активность'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.DiscountByTime.Enabled', 'Enabled'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.DiscountByTime.Enable', N'Активно'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.DiscountByTime.Enable', 'Enable'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.DiscountByTime.Disable', N'Не активно'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.DiscountByTime.Disable', 'Disable'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.DiscountByTime.SortOrder', N'Порядок сортировки'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.DiscountByTime.SortOrder', 'Sort order'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsCoupons.Index.DiscountsByTimeTitle', N'Скидки по времени'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsCoupons.Index.DiscountsByTimeTitle', 'Discounts by time'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.DiscountsByTime.Index.Title', N'Скидки по времени'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.DiscountsByTime.Index.Title', 'Discounts by time'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.DiscountsByTime.Monday', N'Понедельник'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.DiscountsByTime.Monday', 'Monday'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.DiscountsByTime.Tuesday', N'Вторник'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.DiscountsByTime.Tuesday', 'Tuesday'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.DiscountsByTime.Wednesday', N'Среда'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.DiscountsByTime.Wednesday', 'Wednesday'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.DiscountsByTime.Thursday', N'Четверг'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.DiscountsByTime.Thursday', 'Thursday'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.DiscountsByTime.Friday', N'Пятница'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.DiscountsByTime.Friday', 'Friday'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.DiscountsByTime.Saturday', N'Суббота'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.DiscountsByTime.Saturday', 'Saturday'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.DiscountsByTime.Sunday', N'Воскресенье'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.DiscountsByTime.Sunday', 'Sunday'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.DiscountsByTime.DiscountNotFound', N'Скидка не найдена'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.DiscountsByTime.DiscountNotFound', 'Discount not found'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.DiscountByDatetime.DaysOfWeek', N'Дни недели'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.DiscountByDatetime.DaysOfWeek', 'Days of week'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.DiscountByDatetime.DayOfWeek', N'День недели'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.DiscountByDatetime.DayOfWeek', 'Day of week'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.DiscountByTime.DiscountTime', N'Время скидки'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.DiscountByTime.DiscountTime', 'Discount time'

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Template.MainPageVisibleBriefDescription', N'Отображать краткое описание на главной'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Template.MainPageVisibleBriefDescription', 'Display a short description on the main page'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsCatalog.CatalogCommon.CatalogVisibleBriefDescription', N'Отображать краткое описание в каталоге'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsCatalog.CatalogCommon.CatalogVisibleBriefDescription', 'Display short description in catalog'

GO--

ALTER TABLE Catalog.Coupon ADD
	ForFirstOrderInMobileApp bit NULL
GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Coupon.CouponPost.CouponOnlyForFirstOrderInMobileApp', 'Купон может быть применен только для первого заказа в мобильном приложении';
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Coupon.CouponPost.CouponOnlyForFirstOrderInMobileApp', 'Coupon can be applied only for first order in the mobile app';

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.AddEditCoupon.ForFirstOrderInMobileApp', 'На первый заказ в мобильном приложении';
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.AddEditCoupon.ForFirstOrderInMobileApp', 'For first order in the mobile app';

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Catalog.ShowUnitsInCatalog', N'Oтображать единицы измерения в каталоге'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Catalog.ShowUnitsInCatalog', 'Display units of measurement in the catalog'

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Template.MainPageVisibleBriefDescriptionHelp', N'Настройка позволяет выводить краткое описание товаров на главной в десктопной версии при режимах отображения каталога "Плитка" и "Список"'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Template.MainPageVisibleBriefDescriptionHelp', 'The setting allows you to display a brief description of products on the main page in the desktop version when the catalog display modes are "Tile" and "List"'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsCatalog.CatalogCommon.CatalogVisibleBriefDescriptionHelp', N'Настройка позволяет выводить краткое описание товаров в каталоге в десктопной версии при режимах отображения каталога "Плитка" и "Список"'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsCatalog.CatalogCommon.CatalogVisibleBriefDescriptionHelp', 'The setting allows you to display a brief description of products in the catalog in the desktop version in the "Tile" and "List" catalog display modes.'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Template.MainPageVisibleBriefDescriptionMobileHelp', N' Настройка позволяет выводить краткое описание товаров на главной в мобильной версии при режимах отображения каталога "Плитка", "Список" и "Блоки"'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Template.MainPageVisibleBriefDescriptionMobileHelp', 'The setting allows you to display a brief description of products on the main page in the mobile version when the catalog display modes are "Tiles", "List" and "Blocks"'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsCatalog.CatalogCommon.CatalogVisibleBriefDescriptionMobileHelp', N'Настройка позволяет выводить краткое описание товаров в каталоге в мобильной версии при режимах отображения каталога "Плитка", "Список" и "Блоки"'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsCatalog.CatalogCommon.CatalogVisibleBriefDescriptionMobileHelp', 'The setting allows you to display a brief description of products in the catalog in the mobile version when the catalog display modes are "Tiles", "List" and "Blocks"'

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Orders.GetOrderItems.StocksNo', N'Остатки отсутствуют.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Orders.GetOrderItems.StocksNo', N'There are no remains.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Orders.GetOrderItems.StocksInWarehouse', N'{1} ед. на складе "{0}".'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Orders.GetOrderItems.StocksInWarehouse', N'{1} units in warehouse "{0}".'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Order.Stoks', N'Остаток по складам'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Order.Stoks', N'Stock balance'

GO--

IF (EXISTS(SELECT [SettingID] FROM [Settings].[Settings] WHERE [Name]=N'DefaultCityIfNotAutodetect')
	AND NOT EXISTS(SELECT [SettingID] FROM [Settings].[Settings] WHERE [Name]=N'DefaultCityIdIfNotAutodetect'))
BEGIN
	DECLARE @CityName nvarchar(255)

	SET @CityName = (SELECT TOP 1 [Value] FROM [Settings].[Settings] WHERE [Name]=N'DefaultCityIfNotAutodetect')

	IF (NOT @CityName IS NULL AND @CityName != '')
	BEGIN
		DECLARE @CityID int

		SET @CityID = (Select TOP 1 CityID from Customers.City where CityName=@CityName)

		IF (@CityID IS NOT NULL)
		BEGIN
			INSERT INTO [Settings].[Settings] ([Name],[Value])
			VALUES (N'DefaultCityIdIfNotAutodetect', @CityID)
		END
	END
END

GO--

ALTER PROCEDURE [Customers].[sp_GetRecentlyView]
	@CustomerId uniqueidentifier,
	@rowsCount int,
	@Type nvarchar(50)
AS
BEGIN
	SET NOCOUNT ON;
	
	IF Exists (SELECT 1 FROM [Customers].[RecentlyViewsData] WHERE CustomerID=@CustomerId)
    Begin
        SELECT TOP(@rowsCount) Product.ProductID, Product.Name, Product.UrlPath, Product.AllowPreOrder, Ratio, ManualRatio, isnull(PhotoNameSize1, PhotoName) as PhotoName,
            [Photo].[Description] as PhotoDescription, Discount, DiscountAmount, MinPrice as BasePrice, CurrencyValue,
            Offer.OfferID, MaxAvailable AS Amount, MinAmount, MaxAmount, Offer.Amount AS AmountOffer, Colors, NotSamePrices as MultiPrices,
            Product.DoNotApplyOtherDiscounts,  Units.DisplayName as UnitDisplayName,Units.Name as UnitName
        
        FROM [Customers].RecentlyViewsData
        
            Inner Join [Catalog].Product ON Product.ProductID = RecentlyViewsData.ProductId
            Left Join [Catalog].[ProductExt] ON [Product].[ProductID] = [ProductExt].[ProductID]
            Inner Join Catalog.Currency On Currency.CurrencyID = Product.CurrencyID
            Left Join [Catalog].[Photo] ON [Photo].[PhotoId] = [ProductExt].[PhotoId]
            Left Join [Catalog].[Offer] ON [ProductExt].[OfferID] = [Offer].[OfferID]
			Left Join [Catalog].[Units] ON [Product].[Unit] = [Units].[Id]
        
        WHERE RecentlyViewsData.CustomerID = @CustomerId AND Product.Enabled = 1 And CategoryEnabled = 1
        
        ORDER BY ViewDate Desc
    End
END

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.System.IpBlacklist', 'Черный список IP'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.System.IpBlacklist', 'IP Blacklist'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.SettingsSystem.IpBlacklist.AlreadyExistIp', 'Уже существует указанный IP'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.SettingsSystem.IpBlacklist.AlreadyExistIp', 'The specified IP already exists'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsSystem.IpBlacklist.InvalidIp', 'Недопустимый формат IP'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsSystem.IpBlacklist.InvalidIp', 'Invalid IP format'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsSystem.IpBlacklist.AlreadyExistCountry', 'Уже существует указанная страна'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsSystem.IpBlacklist.AlreadyExistCountry', 'The specified contry already exists'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsSystem.IpBlacklist.CountryNotFound', 'Страна не найдена'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsSystem.IpBlacklist.CountryNotFound', 'Country not found'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsSystem.AllowSearchBotsFromOtherCountries', 'Разрешить поисковых ботов из других стран'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsSystem.AllowSearchBotsFromOtherCountries', 'Allow search bots from other countries'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsSystem.IpBlacklist.BlockedIPList', 'Список заблокированных IP'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsSystem.IpBlacklist.BlockedIPList', 'List of blocked IP addresses'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsSystem.IpBlacklist.CountriesToAllowedSite', 'Список разрешенных стран для просмотра сайта'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsSystem.IpBlacklist.CountriesToAllowedSite', 'The list of allowed countries to view the site'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsSystem.IpBlacklist.TrafficStatistics', 'Статистика обращений за последний час'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsSystem.IpBlacklist.TrafficStatistics', 'Statistics of requests for the last hour'

GO--

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[Order].[Order]') AND name = N'IX_Order_Code')
begin
	CREATE NONCLUSTERED INDEX IX_Order_Code ON [Order].[Order] ([Code])
end

GO--

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[Order].[Order]') AND name = N'IX_Order_IsDraft_OrderStatusID')
begin
	CREATE NONCLUSTERED INDEX IX_Order_IsDraft_OrderStatusID ON [Order].[Order] ([IsDraft]) INCLUDE ([OrderStatusID])
end

GO--

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[Bonus].[Purchase]') AND name = N'IX_Purchase_OrderId_Status')
begin
	CREATE NONCLUSTERED INDEX IX_Purchase_OrderId_Status ON [Bonus].[Purchase] ([OrderId], [Status])
end

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsSystem.IpBlacklist.IpContainedInWhiteList', 'IP содержится в белом списке'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsSystem.IpBlacklist.IpContainedInWhiteList', 'The IP is contained in the white list'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsSystem.IpBlacklist.CannotBanCurrentIp', 'Вы не можете заблокировать текущий ip-адрес'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsSystem.IpBlacklist.CannotBanCurrentIp', 'You cannot ban the current ip'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.SettingsSystem.IpBlacklist.IpBanned', 'забанен'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.SettingsSystem.IpBlacklist.IpBanned', 'banned'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.SettingsSystem.IpBlacklist.IpUnban', 'разбанен'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.SettingsSystem.IpBlacklist.IpUnban', 'unban'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.SettingsSystem.IpBlacklist.ErrorWhenAdding', 'Ошибка при добавлении'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.SettingsSystem.IpBlacklist.ErrorWhenAdding', 'Error when adding'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.SettingsSystem.IpBlacklist.ErrorWhenDeleting', 'Ошибка при удалении'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.SettingsSystem.IpBlacklist.ErrorWhenDeleting', 'Error when deleting'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.SettingsSystem.IpBlacklist.RemovingBan', 'Снятие бана'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.SettingsSystem.IpBlacklist.RemovingBan', 'Removing the ban'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.SettingsSystem.IpBlacklist.AreYouSureRemovingBan', 'Вы уверены что хотите удалить IP из черного списка?'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.SettingsSystem.IpBlacklist.AreYouSureRemovingBan', 'Are you sure you want to remove the IP from the blacklist?'

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.System.RegistrationAndAuthorization', 'Регистрация и авторизация'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.System.RegistrationAndAuthorization', 'Registration and authorization'



EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.SystemSettings.Registration', 'Регистрация'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.SystemSettings.Registration', 'Registration'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.SystemSettings.ProhibitRegistration', 'Запретить регистрацию'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.SystemSettings.ProhibitRegistration', 'Prohibit registration'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'User.Registration.ErrorRegistrationIsProhibited', 'Регистрация закрыта'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'User.Registration.ErrorRegistrationIsProhibited', 'Registration is prohibited'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Checkout.BuyInOneClick.ErrorRegistrationIsProhibited', 'Пожалуйста авторизуйтесь, чтобы купить в один клик'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Checkout.BuyInOneClick.ErrorRegistrationIsProhibited', 'Please log in to buy in one click'

GO--

INSERT INTO Settings.InternalSettings (settingKey, settingValue) VALUES ('WebpDownGraderServiceUrl', 'https://webp.advsrvone.pw');

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.ExportImport.ExportFeed.YandexDirectType', 'Яндекс Директ (yml, xml)'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.ExportImport.ExportFeed.YandexDirectType', 'Yandex Direct (yml, xml)'


EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.ExportImport.ExportFeed.YandexWebmasterType', 'Яндекс Вебмастер (yml, xml)'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.ExportImport.ExportFeed.YandexWebmasterType', 'Yandex Webmaster (yml, xml)'

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.AddPropertyGroup.ProductProperties', 'Свойства товаров'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.AddPropertyGroup.ProductProperties', 'Product properties' 

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.WorkigTime.WorkingTimesNotSpecified', 'Рабочее время не указано'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.WorkigTime.WorkingTimesNotSpecified', 'Working hours are not specified' 

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsSystem.TrafficStatistics.ZeroRequest', 'запросов'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsSystem.TrafficStatistics.ZeroRequest', 'requests'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsSystem.TrafficStatistics.OneRequest', 'запрос'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsSystem.TrafficStatistics.OneRequest', 'request'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsSystem.TrafficStatistics.TwoRequest', 'запроса'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsSystem.TrafficStatistics.TwoRequest', 'request'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsSystem.TrafficStatistics.FiveRequest', 'запросов'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsSystem.TrafficStatistics.FiveRequest', 'requests'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsSystem.IpBlacklist.SortOrder', 'Сортировка'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsSystem.IpBlacklist.SortOrder', 'Sort order'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsSystem.IpBlacklist.Missing', 'Отсутствуют'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsSystem.IpBlacklist.Missing', 'Missing'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsSystem.IpBlacklist.SpecifyIP', 'Укажите IP, которые необходимо забанить (по одному в строке)'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsSystem.IpBlacklist.SpecifyIP', 'Specify the IP addresses to be banned (one per line)'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsSystem.IpBlacklist.Ban', 'Забанить'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsSystem.IpBlacklist.Ban', 'Ban'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsSystem.IpBlacklist.Unban', 'Разбанить'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsSystem.IpBlacklist.Unban', 'Unban'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.TrafficStatistics.SortOrder.NoSorting', 'Не сортировать'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.TrafficStatistics.SortOrder.NoSorting', 'Don''t sort'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.TrafficStatistics.SortOrder.DescByCountRequest', 'По убыванию числа запросов'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.TrafficStatistics.SortOrder.DescByCountRequest', 'In descending order of the number of requests'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.TrafficStatistics.SortOrder.AscByCountRequest', 'По возрастанию числа запросов'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.TrafficStatistics.SortOrder.AscByCountRequest', 'By increasing the number of requests'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.TrafficStatistics.SortOrder.DescByIp', 'По убыванию IP'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.TrafficStatistics.SortOrder.DescByIp', 'Descending IP'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.TrafficStatistics.SortOrder.AscByIp', 'По возрастанию IP'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.TrafficStatistics.SortOrder.AscByIp', 'Ascending IP'

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Directory', 'Справочник'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Directory', 'Directory'

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Checkout.AllowUploadFiles', 'Разрешить загружать файлы'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Checkout.AllowUploadFiles', 'Allow uploading an files'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Checkout.AllowUploadFiles.Help', 'При активной опции будет разрешено загружать файлы к заказу на странице оформления заказа'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Checkout.AllowUploadFiles.Help', 'When the option is active, it will be allowed to upload an files to the order on the checkout page'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Checkout.CheckoutComment.SelectFile', 'Выберите файл'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Checkout.CheckoutComment.SelectFile', 'Select a file'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Checkout.CheckoutComment.Files', 'Файлы'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Checkout.CheckoutComment.Files', 'Files'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.Order.SelectFile', 'Выберите файл'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.Order.SelectFile', 'Select a file'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.AreYouSureDelete', 'Вы уверены, что хотите удалить?'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.AreYouSureDelete', 'Are you sure you want to delete?'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.Deleting', 'Удаление'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.Deleting', 'Deleting'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.DeletingError', 'Ошибка при удалении'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.DeletingError', 'Error when deleting'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Checkout.CheckoutImage', 'Размер изображения'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Checkout.CheckoutImage', 'Image size'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Checkout.ImageHeight', 'Высота'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Checkout.ImageHeight', 'Height'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Checkout.ImageWidth', 'Ширина'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Checkout.ImageWidth', 'Width'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.Checkout.File', 'Файл "'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.Checkout.File', 'File "'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.Checkout.FileWasAdded', '" добавлен'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.Checkout.FileWasAdded', '" was added'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.ErrorLoading', 'Ошибка при загрузке'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.ErrorLoading', 'Error loading'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.Checkout.File.ExceededLimit', 'Можно загрузить не больше 10 файлов'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.Checkout.File.ExceededLimit', 'You can upload no more than 10 files'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Common.Attachments.FileStorageLimitReached', 'Достигнуто ограничение объема файлов'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Common.Attachments.FileStorageLimitReached', 'The file size limit has been reached'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Common.Attachments.FileNotFound', 'Файл не найден'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Common.Attachments.FileNotFound', 'File not found'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Common.Attachments.FileAlreadyExists', 'Файл с таким именем уже существует'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Common.Attachments.FileAlreadyExists', 'File already exists'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Common.Attachments.ExceededLimit', 'Можно загрузить не больше 10 файлов'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Common.Attachments.ExceededLimit', 'You can upload no more than 10 files'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Order.File', 'Файл "'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Order.File', 'File "'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Order.FileWasAdded', '" добавлен'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Order.FileWasAdded', '" was added'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.ErrorLoading', 'Ошибка при загрузке'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.ErrorLoading', 'Error loading'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Order.File.ExceededLimit', 'Можно загрузить не больше 10 файлов'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Order.File.ExceededLimit', 'You can upload no more than 10 files'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.OrderItemsSummary.Files', 'Файлы покупателя'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.OrderItemsSummary.Files', 'Customer''s files'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.OrderItemsSummary.SelectFile', 'Выберите файл'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.OrderItemsSummary.SelectFile', 'Select a file'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Checkout.CheckoutComment.DeleteFile', 'Удалить файл'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Checkout.CheckoutComment.DeleteFile', 'Delete file'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.Order.DeleteFile', 'Удалить файл'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.Order.DeleteFile', 'Delete file'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.Order.Files', 'Файлы'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.Order.Files', 'Files'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.Order.SuccessfullyDeleted', 'Успешно удалено'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.Order.SuccessfullyDeleted', 'Successfully deleted'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Checkout.Attachments.ProhibitUploadFiles', 'Запрещено загружать файлы'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Checkout.Attachments.ProhibitUploadFiles', 'It is forbidden to upload files'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Orders.OrderItems.Delete', 'Удалить'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Orders.OrderItems.Delete', 'Delete'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Orders.OrderItems.Files', 'Файлы'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Orders.OrderItems.Files', 'Files'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Orders.OrderItems.SelectFile', 'Выберите файл'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Orders.OrderItems.SelectFile', 'Select a file'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Orders.Order.ChangingCustomerOrderFiles', 'Файлы покупателя к заказу'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Orders.Order.ChangingCustomerOrderFiles', 'Buyer''s files for the order'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Orders.Order.ChangingAdminOrderFiles', 'Файлы администратора к заказу'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Orders.Order.ChangingAdminOrderFiles', 'Admin''s files for the order'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Orders.OrderAttachments.AttachedFiles', 'Прикрепленные файлы'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Orders.OrderAttachments.AttachedFiles', 'Attached files'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Orders.AddEdit.CustomerOrderAttachments', 'Прикрепленные файлы покупателя'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Orders.AddEdit.CustomerOrderAttachments', 'Attached buyer''s files'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Common.Attachments.CannotResizePhotoFile', 'Не удалось пережать фото'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Common.Attachments.CannotResizePhotoFile', 'Failed to transfer photo'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.Order.FileDoesNotMeet', 'Файл не соответствует требованиям'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.Order.FileDoesNotMeet', 'The file does not meet the requirements'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Order.FileDoesNotMeet', 'Файл не соответствует требованиям'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Order.FileDoesNotMeet', 'The file does not meet the requirements'

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.ExportCustomers.LastOrderId', 'Id последнего заказа'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.ExportCustomers.LastOrderId', 'Last order Id' 

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Search.SearchBlockMobile.SearchPlaceholder', 'Введите запрос'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Search.SearchBlockMobile.SearchPlaceholder', 'Enter your request' 

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Checkout.Success.ReturnOnMobileVersion', 'Перейти на мобильную версию'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Checkout.Success.ReturnOnMobileVersion', 'Go to mobile version'

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Checkout.OutOfStockSalesFunnel', 'Список лидов для заявок под заказ'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Checkout.OutOfStockSalesFunnel', 'Leads list for preorders' 

GO--

UPDATE [Settings].[InternalSettings] SET [settingValue] = '13.0.0' WHERE [settingKey] = 'db_version'
