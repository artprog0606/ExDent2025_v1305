INSERT INTO [Settings].[Localization] (ResourceKey, ResourceValue, LanguageId) VALUES
	('Js.ShippingTemplate.SelectInterval.Soonest', 'Как можно скорее', 1),
	('Js.ShippingTemplate.SelectInterval.Soonest', 'Soonest', 2),
	('Js.ShippingTemplate.SelectInterval.IntervalsNotAvailable', 'Нет доступных интервалов в выбранную дату', 1),
	('Js.ShippingTemplate.SelectInterval.IntervalsNotAvailable', 'There are no available intervals on the selected date', 2)

GO--

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Settings].[AdditionalOption]') AND type in (N'U'))
BEGIN
	CREATE TABLE [Settings].[AdditionalOption](
		[ObjId] [NVARCHAR] (100) NOT NULL,
		[ObjType] [INT] NOT NULL,
		[Value] [NVARCHAR] (MAX) NULL
	CONSTRAINT [PK_AdditionalOption] PRIMARY KEY CLUSTERED 
	(
		[ObjId] ASC,
		[ObjType] ASC
	) WITH( PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]
END

INSERT INTO [Settings].[Localization] (ResourceKey, ResourceValue, LanguageId) VALUES
	('Admin.Js.AddEditCity.AdditionalSettings', 'Дополнительные настройки', 1),
	('Admin.Js.AddEditCity.AdditionalSettings', 'Additional settings', 2),
	('Admin.Js.AddEditCity.ShowAdditionalSettings', 'Показать', 1),
	('Admin.Js.AddEditCity.ShowAdditionalSettings', 'Show', 2),
	('Admin.Js.AddEditCity.HideAdditionalSettings', 'Скрыть', 1),
	('Admin.Js.AddEditCity.HideAdditionalSettings', 'Hide', 2),
	('Admin.Js.AddEditCity.ShippingZones', 'Cсылка на png картинку с зонами доставки', 1),
	('Admin.Js.AddEditCity.ShippingZones', 'Link to png image with delivery zones', 2),
	('Admin.Js.AddEditCity.ShippingZonesIframe', 'Iframe с зоной доставки', 1),
	('Admin.Js.AddEditCity.ShippingZonesIframe', 'Iframe with delivery area', 2),
	('Admin.Js.AddEditCity.CityAddressPoints', 'Cсылка на png картинку с точками адресов', 1),
	('Admin.Js.AddEditCity.CityAddressPoints', 'Link to a png image with address points', 2),
	('Admin.Js.AddEditCity.CityAddressPointsIframe', 'Iframe с точками адресов', 1),
	('Admin.Js.AddEditCity.CityAddressPointsIframe', 'Iframe with address points', 2)

GO--

INSERT INTO [Settings].[Localization] (ResourceKey, ResourceValue, LanguageId) VALUES
	('Admin.Js.TestShippingCalculate.Street', 'Улица', 1),
	('Admin.Js.TestShippingCalculate.Street', 'Street', 2),
	('Admin.Js.TestShippingCalculate.House', 'Дом', 1),
	('Admin.Js.TestShippingCalculate.House', 'House', 2),
	('Admin.Js.TestShippingCalculate.Structure', 'Стр./корп.', 1),
	('Admin.Js.TestShippingCalculate.Structure', 'Structure', 2),
	('Admin.Js.TestShippingCalculate.Apartment', 'Квартира', 1),
	('Admin.Js.TestShippingCalculate.Apartment', 'Apartment', 2),
	('Admin.Js.TestShippingCalculate.Entrance', 'Подъезд', 1),
	('Admin.Js.TestShippingCalculate.Entrance', 'Entrance', 2),
	('Admin.Js.TestShippingCalculate.Floor', 'Этаж', 1),
	('Admin.Js.TestShippingCalculate.Floor', 'Floor', 2);

GO--

INSERT INTO [Settings].[Localization] (ResourceKey, ResourceValue, LanguageId) VALUES
	('Core.Crm.ELeadFieldType.BonusAccount', 'Бонусный счет', 1),
	('Core.Crm.ELeadFieldType.BonusAccount', 'Bonus account', 2),
	('Core.Services.Triggers.ETriggerEventType.InstallMobileApp', 'Установка мобильного приложения', 1),
	('Core.Services.Triggers.ETriggerEventType.InstallMobileApp', 'Install mobile app', 2),
	('Core.Services.Triggers.ETriggerEventType.Description.InstallMobileApp', 'Триггер на установку мобильного приложения', 1),
	('Core.Services.Triggers.ETriggerEventType.Description.InstallMobileApp', 'Trigger to install a mobile app', 2)

GO--

INSERT INTO [Settings].[Localization] (ResourceKey, ResourceValue, LanguageId) VALUES
	('Admin.ExportFeeed.SettingsYandex.PriceRange', 'Диапазон цен', 1),
	('Admin.ExportFeeed.SettingsYandex.PriceRange', 'Price range', 2),
	('Admin.ExportFeeed.SettingsYandex.PriceRangeFrom', 'от', 1),
	('Admin.ExportFeeed.SettingsYandex.PriceRangeFrom', 'from', 2),
	('Admin.ExportFeeed.SettingsYandex.PriceRangeTo', 'до', 1),
	('Admin.ExportFeeed.SettingsYandex.PriceRangeTo', 'to', 2),
	('Admin.ExportFeeed.SettingsYandex.PriceRange.Help', 'Можно ограничить выгрузку по цене товара. Скидки не учитываются. Если значение не указано, то фильтр не применится.', 1),
	('Admin.ExportFeeed.SettingsYandex.PriceRange.Help', 'You can limit export by products price. Discounts don''t apply. If no value is specified, the filter will not apply.', 2)

GO--

IF NOT EXISTS(SELECT 1
              FROM sys.columns
              WHERE (name = N'ObjValue') AND object_id = OBJECT_ID(N'[CRM].[TriggerAction]'))
    BEGIN
        ALTER TABLE CRM.TriggerAction 
			ADD ObjValue NVARCHAR(max) NULL
	END
GO--

INSERT INTO [Settings].[Localization] (ResourceKey, ResourceValue, LanguageId) VALUES
	('Admin.Import.ImportProducts.301Redirects', 'Создавать 301-редиректы', 1),
	('Admin.Import.ImportProducts.301Redirects', 'Create 301 redirects', 2)

GO--

ALTER TABLE Catalog.Product ADD
	IsDigital bit NULL
GO--

Update Catalog.Product Set IsDigital = 0
GO--

INSERT INTO [Settings].[Localization] (ResourceKey, ResourceValue, LanguageId) VALUES
	('Core.Catalog.Product.IsDigital', 'Цифровой товар', 1),
	('Core.Catalog.Product.IsDigital', 'Is digital', 2)

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
    @Unit int,
    @ShippingPrice float,
    @MinAmount float,
    @MaxAmount float,
    @Multiplicity float,
    @HasMultiOffer bit,     
    @CurrencyID int,
    @ActiveView360 bit,
    @ModifiedBy nvarchar(50),   
    @AccrueBonuses bit,
    @Taxid int,
    @PaymentSubjectType int,
    @PaymentMethodType int,
    @DateModified datetime,
    @CreatedBy nvarchar(50),
    @Hidden bit,
    @ManualRatio float,
	@IsMarkingRequired bit,
	@Comment nvarchar(max),
	@DoNotApplyOtherDiscounts bit,
	@IsDigital bit
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
        ,CurrencyID
        ,ActiveView360
        ,ModifiedBy
        ,AccrueBonuses
        ,TaxId
        ,PaymentSubjectType
        ,PaymentMethodType
        ,CreatedBy
        ,Hidden
        ,ManualRatio
		,IsMarkingRequired
		,Comment
		,DoNotApplyOtherDiscounts
		,IsDigital
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
        ,@CurrencyID
        ,@ActiveView360
        ,@ModifiedBy
        ,@AccrueBonuses
        ,@TaxId
        ,@PaymentSubjectType
        ,@PaymentMethodType
        ,@CreatedBy
        ,@Hidden
        ,@ManualRatio
		,@IsMarkingRequired
        ,@Comment
		,@DoNotApplyOtherDiscounts
		,@IsDigital
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
    @Unit int,
    @ShippingPrice money,
    @MinAmount float,
    @MaxAmount float,
    @Multiplicity float,
    @HasMultiOffer bit,
    @CurrencyID int,
    @ActiveView360 bit,
    @ModifiedBy nvarchar(50),
    @AccrueBonuses bit,
    @TaxId int,
    @PaymentSubjectType int,
    @PaymentMethodType int,
    @DateModified datetime,
    @CreatedBy nvarchar(50),
    @Hidden bit,
    @ManualRatio float,
	@IsMarkingRequired bit,
	@Comment nvarchar(max),
	@DoNotApplyOtherDiscounts bit,
	@IsDigital bit
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
        ,[CurrencyID] = @CurrencyID
        ,[ActiveView360] = @ActiveView360
        ,[ModifiedBy] = @ModifiedBy
        ,[AccrueBonuses] = @AccrueBonuses
        ,[TaxId] = @TaxId
        ,[PaymentSubjectType] = @PaymentSubjectType
        ,[PaymentMethodType] = @PaymentMethodType
        ,[CreatedBy] = @CreatedBy
        ,[Hidden] = @Hidden
        ,[Manualratio] = @ManualRatio
		,[IsMarkingRequired] = @IsMarkingRequired
		,[Comment] = @Comment
		,[DoNotApplyOtherDiscounts] = @DoNotApplyOtherDiscounts
		,[IsDigital] = @IsDigital
    WHERE ProductID = @ProductID
END

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
    VALUES
           (1,'Admin.Product.Edit.IsDigital', N'Цифровой товар'),
           (2,'Admin.Product.Edit.IsDigital', 'Is digital'),
           (1,'Core.ExportImport.ProductFields.IsDigital', N'Цифровой товар'),
           (2,'Core.ExportImport.ProductFields.IsDigital', 'Is digital'),
		   (1,'Core.ExportImport.EProductField.IsDigital', N'Цифровой товар'),
           (2,'Core.ExportImport.EProductField.IsDigital', 'Is digital')

GO--

INSERT INTO [Settings].[Localization] (ResourceKey, ResourceValue, LanguageId) VALUES
	('Admin.ShippingMethods.DeliveryByZones.YaMapsApiKey', 'API-ключ яндекс.карт', 1),
	('Admin.ShippingMethods.DeliveryByZones.YaMapsApiKey', 'Api key of Yandex.Maps', 2),
	('Admin.ShippingMethods.DeliveryByZones.YaMapsApiKeyHelp', 'Как получить ключ?', 1),
	('Admin.ShippingMethods.DeliveryByZones.YaMapsApiKeyHelp', 'How do I get the key?', 2),
	('Admin.ShippingMethods.DeliveryByZones.InfoGeocoderLine1', 'Для исользования данного типа доставки необходимо геокодирование адреса. По API-ключу яндекс.карт доступно 1000 запросов в сутки.', 1),
	('Admin.ShippingMethods.DeliveryByZones.InfoGeocoderLine1', 'To use this type of delivery, address geocoding is required. The yandex.maps API key is available for 1000 requests per day.', 2),
	('Admin.ShippingMethods.DeliveryByZones.InfoGeocoderLine2', 'Рекомендуется установить модуль', 1),
	('Admin.ShippingMethods.DeliveryByZones.InfoGeocoderLine2', 'It is recommended to install', 2),
	('Admin.ShippingMethods.DeliveryByZones.InfoGeocoderLine2Dadata', 'интеграции с DaData', 1),
	('Admin.ShippingMethods.DeliveryByZones.InfoGeocoderLine2Dadata', 'the DaData integration module', 2),
	('Admin.ShippingMethods.DeliveryByZones.Zones', 'Зоны доставки', 1),
	('Admin.ShippingMethods.DeliveryByZones.Zones', 'Delivery zones', 2),
	('Admin.Js.DeliveryByZonesList.Header', 'Создайте зоны доставки в <a href="https://yandex.ru/map-constructor/" target="_blank">конструкторе карт Яндекса</a> и экспортируйте в GEO JSON', 1),
	('Admin.Js.DeliveryByZonesList.Header', 'Create delivery zones in <a href="https://yandex.ru/map-constructor/" target="_blank">the Yandex Map Constructor</a> and export to GEO JSON', 2),
	('Admin.Js.DeliveryByZonesList.UploadGeoJson', 'Загрузить GEO JSON файл с зонами доставки', 1),
	('Admin.Js.DeliveryByZonesList.UploadGeoJson', 'Upload GEO JSON file with delivery zones', 2),
	('Admin.Js.DeliveryByZonesList.Remove', 'Удалить', 1),
	('Admin.Js.DeliveryByZonesList.Remove', 'Remove', 2),
	('Admin.Js.DeliveryByZonesList.DownloadGeoJson', 'Выгрузить GEO JSON файл с зонами доставки', 1),
	('Admin.Js.DeliveryByZonesList.DownloadGeoJson', 'Download GEO JSON file with delivery zones', 2),
	('Admin.Js.DeliveryByZonesList.Zones', 'Зоны доставки', 1),
	('Admin.Js.DeliveryByZonesList.Zones', 'Delivery zones', 2),
	('Admin.Js.AddDeliveryZone.Zone', 'Зона доставки', 1),
	('Admin.Js.AddDeliveryZone.Zone', 'Delivery zone', 2),
	('Admin.Js.AddDeliveryZone.Name', 'Название', 1),
	('Admin.Js.AddDeliveryZone.Name', 'Name', 2),
	('Admin.Js.AddDeliveryZone.Description', 'Описание', 1),
	('Admin.Js.AddDeliveryZone.Description', 'Description', 2),
	('Admin.Js.AddDeliveryZone.ShippingCost', 'Цена доставки', 1),
	('Admin.Js.AddDeliveryZone.ShippingCost', 'Shipping cost', 2),
	('Admin.Js.AddDeliveryZone.DeliveryTime', 'Срок доставки', 1),
	('Admin.Js.AddDeliveryZone.DeliveryTime', 'Delivery time', 2),
	('Js.ShippingTemplate.DeliveryByZones.ViewDeliveryZones', 'Посмотреть зоны доставки', 1),
	('Js.ShippingTemplate.DeliveryByZones.ViewDeliveryZones', 'View delivery zones', 2),
	('Admin.ShippingMethods.DeliveryByZones.AddressIsOutsideZones', 'Указанный адрес не входит в зону(ы) доставки', 1),
	('Admin.ShippingMethods.DeliveryByZones.AddressIsOutsideZones', 'The specified address is not included in the delivery zone(s)', 2),
	('Admin.ShippingMethods.DeliveryByZones.EnterFullAddress', 'Укажите полный адрес доставки', 1),
	('Admin.ShippingMethods.DeliveryByZones.EnterFullAddress', 'Enter the full delivery address', 2),
	('Admin.ShippingMethods.DeliveryByZones.IncorrectAddress', 'Указанный адрес не распознан', 1),
	('Admin.ShippingMethods.DeliveryByZones.IncorrectAddress', 'The specified address is not recognized', 2),
	('Admin.ShippingMethods.DeliveryByZones.NotFoundCorpusOfAddress', 'Указанный корпус, литера или строение не найдено', 1),
	('Admin.ShippingMethods.DeliveryByZones.NotFoundCorpusOfAddress', 'The specified building or structure was not found', 2),
	('Admin.ShippingMethods.DeliveryByZones.HouseNotFound', 'Указанный дом не найден', 1),
	('Admin.ShippingMethods.DeliveryByZones.HouseNotFound', 'The specified house was not found', 2),
	('Admin.ShippingMethods.DeliveryByZones.NoHouseOrAddressNotFound', 'Не указан номер дома либо указанный адрес не найден', 1),
	('Admin.ShippingMethods.DeliveryByZones.NoHouseOrAddressNotFound', 'The house number is not specified or the specified address was not found', 2),
	('Admin.ShippingMethods.DeliveryByZones.AddressNotFound', 'Указанный адрес не найден', 1),
	('Admin.ShippingMethods.DeliveryByZones.AddressNotFound', 'The specified address was not found', 2),
	('Admin.ShippingMethods.DeliveryByZones.StreetNotFound', 'Указанная улица не найдена', 1),
	('Admin.ShippingMethods.DeliveryByZones.StreetNotFound', 'The specified street was not found', 2),
	('Admin.ShippingMethods.DeliveryByZones.IsNotAddressOfHouse', 'Указанный адрес не распознан как адрес дома', 1),
	('Admin.ShippingMethods.DeliveryByZones.IsNotAddressOfHouse', 'The specified address is not recognized as a house address', 2);

GO--

INSERT INTO [Settings].[Localization] (ResourceKey, ResourceValue, LanguageId) VALUES
	('Admin.Settings.SystemSettings.SmsActive', 'Включен', 1),
	('Admin.Settings.SystemSettings.SmsActive', 'Active', 2),
	('Admin.SettingsSystem.SystemCommon.EnableCaptchaInSmsConfirmation', 'Показывать при подтверждении телефона по СМС', 1),
	('Admin.SettingsSystem.SystemCommon.EnableCaptchaInSmsConfirmation', 'Show when confirming the phone by SMS', 2),
	('Admin.Settings.SystemSettings.AuthSms', 'Sms', 1),
	('Admin.Settings.SystemSettings.AuthSms', 'Sms', 2),
	('Admin.Settings.SystemSettings.AuthSmsInstruction', 'Инструкция. Настройка авторизации с помощью СМС', 1),
	('Admin.Settings.SystemSettings.AuthSmsInstruction', 'Instruction. Configuring authorization with SMS', 2)

INSERT INTO [Settings].[Localization] (ResourceKey, ResourceValue, LanguageId) VALUES
	('User.SmsAuthorization.Login', 'Войти', 1),
	('User.SmsAuthorization.Login', 'Login', 2),
	('User.SmsAuthorization.SendCode', 'Выслать код', 1),
	('User.SmsAuthorization.SendCode', 'Send code', 2),
	('User.SmsAuthorization.SendCodeAgain', 'Выслать код повторно', 1),
	('User.SmsAuthorization.SendCodeAgain', 'Send code again', 2),
	('User.SmsAuthorization.ChangePhoneNumber', 'Изменить номер телефона', 1),
	('User.SmsAuthorization.ChangePhoneNumber', 'Change phone number', 2),
	('User.SmsAuthorization.LoginWithEmail', 'Войти по email', 1),
	('User.SmsAuthorization.LoginWithEmail', 'Login with email', 2),
	('User.SmsAuthorization.LoginPartners', 'Войти в партнерский кабинет', 1),
	('User.SmsAuthorization.LoginPartners', 'Login to the partner account', 2)

INSERT INTO [Settings].[Localization] (ResourceKey, ResourceValue, LanguageId) VALUES
	('Js.ConfirmSms.CodeSended', 'Код подтверждения выслан на номер ', 1),
	('Js.ConfirmSms.CodeSended', 'Confirmation code sended on number ', 2),
	('Js.ConfirmSms.WrongCode', 'Неверно введен код подтверждения телефона', 1),
	('Js.ConfirmSms.WrongCode', 'The phone confirmation code was entered incorrectly', 2)

INSERT INTO [Settings].[Localization] (ResourceKey, ResourceValue, LanguageId) VALUES
	('Shared.SmsConfirmation.Code', 'Код подтверждения:', 1),
	('Shared.SmsConfirmation.Code', 'Confirmation code:', 2),
	('Shared.SmsConfirmation.SendCode', 'Получить код', 1),
	('Shared.SmsConfirmation.SendCode', 'Send code', 2),
	('Shared.SmsConfirmation.PhoneConfirmed', 'Телефон подтвержден', 1),
	('Shared.SmsConfirmation.PhoneConfirmed', 'Phone confirmed', 2),
	('Shared.SmsConfirmation.PhoneNotConfirmed', 'Телефон не подтвержден', 1),
	('Shared.SmsConfirmation.PhoneNotConfirmed', 'Phone not confirmed', 2)
  
GO--

INSERT INTO [Settings].[Localization] (ResourceKey, ResourceValue, LanguageId) VALUES
	('Admin.Js.AddEditCoupon.CustomerGroup', 'Группа пользователя', 1),
	('Admin.Js.AddEditCoupon.CustomerGroup', 'Customer group', 2),
	('Admin.Js.AddEditCoupon.AllCustomerGroupEnabled', 'Все группы', 1),
	('Admin.Js.AddEditCoupon.AllCustomerGroupEnabled', 'All groups', 2)

GO--

ALTER TABLE [Catalog].[Coupon]
ADD AllCustomerGroupEnabled bit NULL;

GO--

UPDATE [Catalog].[Coupon] SET AllCustomerGroupEnabled = 1;

GO--

ALTER TABLE [Catalog].[Coupon]
ALTER COLUMN AllCustomerGroupEnabled bit NOT NULL;

GO--

ALTER TABLE [Catalog].[Coupon]
ADD CustomerGroupId int NULL

GO--

INSERT INTO [Settings].[Localization] (ResourceKey, ResourceValue, LanguageId) VALUES 
	('User.Authorization.LoginWithPhone', 'Войти по номеру телефона', 1),
	('User.Authorization.LoginWithPhone', 'Login by phone number', 2)

GO--

INSERT INTO [Settings].[Localization] (ResourceKey, ResourceValue, LanguageId) VALUES
	('Admin.Js.ShippingWithInterval.Settings.IntervalLength', 'Длина интервала', 1),
	('Admin.Js.ShippingWithInterval.Settings.IntervalLength', 'ArtNo', 2),
	('Admin.Js.ShippingWithInterval.Settings.DeliveryIntervalIsEmpty', 'Нет интервалов', 1),
	('Admin.Js.ShippingWithInterval.Settings.DeliveryIntervalIsEmpty', 'Not intervals', 2),
	('Admin.Js.ShippingWithInterval.Settings.GenerateIntervals', 'Сгенерировать интервалы по диапазону', 1),
	('Admin.Js.ShippingWithInterval.Settings.GenerateIntervals', 'Generate intervals by rang', 2),
	('Admin.Js.ShippingWithInterval.Settings.DayOfWeek', 'День недели', 1),
	('Admin.Js.ShippingWithInterval.Settings.DayOfWeek', 'Day of week', 2),
	('Admin.Js.ShippingWithInterval.Settings.AddInterval', 'Добавить', 1),
	('Admin.Js.ShippingWithInterval.Settings.AddInterval', 'Add', 2),
	('Admin.Js.ShippingWithInterval.Settings.GenerateInterval', 'Сгенерировать', 1),
	('Admin.Js.ShippingWithInterval.Settings.GenerateInterval', 'Generate', 2),
	('Admin.Js.ShippingWithInterval.Settings.AdditionalSettings', 'Дополнительные настройки', 1),
	('Admin.Js.ShippingWithInterval.Settings.AdditionalSettings', 'Additional settings', 2),
	('Admin.Js.ShippingWithInterval.Settings.GMT', 'Часовой пояс', 1),
	('Admin.Js.ShippingWithInterval.Settings.GMT', 'GMT', 2),
	('Admin.Js.ShippingWithInterval.Settings.CountVisibleDeliveryDay', 'Количество дней, в которые можно оформить заказ', 1),
	('Admin.Js.ShippingWithInterval.Settings.CountVisibleDeliveryDay', 'The number of days in which can place an order', 2),
	('Admin.Js.ShippingWithInterval.Settings.MinDeliveryTime', 'Количество минут, через которые можно оформить заказ', 1),
	('Admin.Js.ShippingWithInterval.Settings.MinDeliveryTime', 'The number of minutes after which you can place an order', 2),
	('Admin.Js.ShippingWithInterval.Settings.Header', 'Интервалы доставки', 1),
	('Admin.Js.ShippingWithInterval.Settings.Header', 'Delivery intervals', 2),
	('Admin.Js.ShippingMethods.Ready', 'Готово', 1),
	('Admin.Js.ShippingMethods.Ready', 'Ready', 2),
	('Admin.Js.ShippingWithInterval.Settings.MinDeliveryTime.Help', 'Ближайшая дата доставки будет не раньше, чем через указанное количество минут от времени оформления заказа', 1),
	('Admin.Js.ShippingWithInterval.Settings.MinDeliveryTime.Help', 'The nearest delivery date will not be earlier than in the specified number of minutes from the time of placing the order', 2),
	('Admin.Js.DeliveryTime.UseInterval', 'Использовать интервал', 1),
	('Admin.Js.DeliveryTime.UseInterval', 'UseInterval', 2),
	('Admin.Js.DeliveryTime.GMT', 'Часовой пояс', 1),
	('Admin.Js.DeliveryTime.GMT', 'GMT', 2),
	('Admin.Js.DeliveryTime.DisplayedDeliveryTime', 'Отображаемое время доставки', 1),
	('Admin.Js.DeliveryTime.DisplayedDeliveryTime', 'Displayed delivery time', 2),
	('Admin.Js.ShippingWithInterval.Configure', 'Настроить', 1),
	('Admin.Js.ShippingWithInterval.Configure', 'Configure', 2),
	('Admin.ShippingMethods.Common.DeliveryIntervals', 'Общие интервалы доставки', 1),
	('Admin.ShippingMethods.Common.DeliveryIntervals', 'Common delivery intervals', 2),
	('Admin.Js.ShippingWithInterval.Settings.СreationStep', 'Шаг создания', 1),
	('Admin.Js.ShippingWithInterval.Settings.СreationStep', 'Сreation step', 2),
	('Admin.Js.ShippingWithInterval.Settings.DeletingIntervals', 'Удаление интервалов', 1),
	('Admin.Js.ShippingWithInterval.Settings.DeletingIntervals', 'Deleting intervals', 2),
	('Admin.Js.ShippingWithInterval.Settings.AreYouSureDelete', 'Вы уверены, что хотите удалить все интервалы?', 1),
	('Admin.Js.ShippingWithInterval.Settings.AreYouSureDelete', 'Are you sure you want to delete all the intervals?', 2),
	('Admin.Js.ShippingWithInterval.Settings.DeleteAllIntervals', 'Удалить все интервалы', 1),
	('Admin.Js.ShippingWithInterval.Settings.DeleteAllIntervals', 'Delete all intervals', 2),
	('Admin.Js.ShippingWithInterval.Settings.TimeRange', 'Временной диапазон создания', 1),
	('Admin.Js.ShippingWithInterval.Settings.TimeRange', 'Creation time range', 2),
	('Admin.Js.ShippingWithInterval.Settings.CountVisibleDeliveryDay.Help', 'Последняя дата для выбора будет не позднее, чем через указанное количество дней от даты оформления заказа', 1),
	('Admin.Js.ShippingWithInterval.Settings.CountVisibleDeliveryDay.Help', 'The last date for selection will be no later than the specified number of days from the date of placing the order', 2),
	('Js.ShippingTemplate.SelectInterval.SelectDateAndTime', 'Выберите дату и время', 1),
	('Js.ShippingTemplate.SelectInterval.SelectDateAndTime', 'Select date and time', 2),
	('Admin.Js.ShippingWithInterval.Settings.ShowSoonest', 'Возможность оформить заказ как можно скорее', 1),
	('Admin.Js.ShippingWithInterval.Settings.ShowSoonest', 'Possibility to place an order as soon as possible', 2),
	('Admin.Js.ShippingWithInterval.Settings.ShowSoonest.Help', 'В оформлении заказа будет выводиться галочка "Как можно скорее", при ее активности скрывается выбор интервалов', 1),
	('Admin.Js.ShippingWithInterval.Settings.ShowSoonest.Help', 'The check mark "As soon as possible" will be displayed in the checkout, when it is active, the choice of intervals is hidden', 2)

GO--

INSERT INTO [Settings].[Localization] (ResourceKey, ResourceValue, LanguageId) VALUES 
	('Api.OfficeAddressesBlockName', 'Магазины', 1),
	('Api.OfficeAddressesBlockName', 'Stores', 2)

GO--

INSERT INTO [Settings].[Localization] (ResourceKey, ResourceValue, LanguageId) VALUES 
	('Js.ShippingTemplate.SelectInterval.OptionNotSelected', 'Не выбрано', 1),
	('Js.ShippingTemplate.SelectInterval.OptionNotSelected', 'Not selected', 2)

GO--

INSERT INTO [Settings].[Localization] (ResourceKey, ResourceValue, LanguageId) VALUES
	('Admin.Js.TaskGroups.NewTaskCount', 'Новых', 1),
	('Admin.Js.TaskGroups.NewTaskCount', 'New tasks', 2),
	('Admin.Js.TaskGroups.InProgressTaskCount', 'В работе', 1),
	('Admin.Js.TaskGroups.InProgressTaskCount', 'In progress tasks', 2),
	('Admin.Js.TaskGroups.CompletedTaskCount', 'Завершенных', 1),
	('Admin.Js.TaskGroups.CompletedTaskCount', 'Completed tasks', 2),
	('Admin.Js.TaskGroups.AcceptedTaskCount', 'Принятых', 1),
	('Admin.Js.TaskGroups.AcceptedTaskCount', 'Accepted tasks', 2),
	('Admin.Js.TaskGroups.CanceledTaskCount', 'Отклоненных', 1),
	('Admin.Js.TaskGroups.CanceledTaskCount', 'Canceled tasks', 2)

GO--

UPDATE [Settings].[InternalSettings] SET [settingValue] = '11.5.0' WHERE [settingKey] = 'db_version'
