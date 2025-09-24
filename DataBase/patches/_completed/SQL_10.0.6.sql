
IF not EXISTS(SELECT 1 FROM sys.columns WHERE Name = N'ExtrachargeInPercents' AND Object_ID = Object_ID(N'[Order].[PaymentMethod]'))
BEGIN
	ALTER TABLE [Order].[PaymentMethod] ADD
		ExtrachargeInPercents float(53) NULL,
		ExtrachargeInNumbers float(53) NULL
END

GO--

IF EXISTS(SELECT 1 FROM sys.columns WHERE Name = N'Extracharge' AND Object_ID = Object_ID(N'[Order].[PaymentMethod]'))
BEGIN
	Update [Order].[PaymentMethod] 
	Set ExtrachargeInPercents = 0, ExtrachargeInNumbers = Extracharge
	Where [ExtrachargeType] = 0
END

GO--

IF EXISTS(SELECT 1 FROM sys.columns WHERE Name = N'Extracharge' AND Object_ID = Object_ID(N'[Order].[PaymentMethod]'))
BEGIN
	Update [Order].[PaymentMethod] 
	Set ExtrachargeInPercents = Extracharge, ExtrachargeInNumbers = 0
	Where [ExtrachargeType] = 1
END

GO--

IF EXISTS(SELECT 1 FROM sys.columns WHERE Name = N'Extracharge' AND Object_ID = Object_ID(N'[Order].[PaymentMethod]'))
BEGIN
    ALTER TABLE [Order].[PaymentMethod] 
		DROP COLUMN [Extracharge]
END

GO--

IF EXISTS(SELECT 1 FROM sys.columns WHERE Name = N'ExtrachargeType' AND Object_ID = Object_ID(N'[Order].[PaymentMethod]'))
BEGIN
	ALTER TABLE [Order].[PaymentMethod] 
		DROP COLUMN [ExtrachargeType]
END

GO--

IF not EXISTS(SELECT 1 FROM sys.columns WHERE Name = N'ExtrachargeInPercents' AND Object_ID = Object_ID(N'[Order].[ShippingMethod]'))
BEGIN
	ALTER TABLE [Order].[ShippingMethod] ADD
		ExtrachargeInPercents float(53) NULL,
		ExtrachargeInNumbers float(53) NULL
END

GO--

IF EXISTS(SELECT 1 FROM sys.columns WHERE Name = N'Extracharge' AND Object_ID = Object_ID(N'[Order].[ShippingMethod]'))
BEGIN
	Update [Order].[ShippingMethod] 
	Set ExtrachargeInPercents = 0, ExtrachargeInNumbers = Extracharge
	Where [ExtrachargeType] = 0
END

GO--

IF EXISTS(SELECT 1 FROM sys.columns WHERE Name = N'Extracharge' AND Object_ID = Object_ID(N'[Order].[ShippingMethod]'))
BEGIN
	Update [Order].[ShippingMethod] 
	Set ExtrachargeInPercents = Extracharge, ExtrachargeInNumbers = 0
	Where [ExtrachargeType] = 1
END

GO--

IF EXISTS(SELECT 1 FROM sys.columns WHERE Name = N'Extracharge' AND Object_ID = Object_ID(N'[Order].[ShippingMethod]'))
BEGIN
    ALTER TABLE [Order].[ShippingMethod] 
		DROP COLUMN [Extracharge]
END

GO--

IF EXISTS(SELECT 1 FROM sys.columns WHERE Name = N'ExtrachargeType' AND Object_ID = Object_ID(N'[Order].[ShippingMethod]'))
BEGIN
	ALTER TABLE [Order].[ShippingMethod] 
		DROP COLUMN [ExtrachargeType]
END

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Error.MobileAppBlocked', 'Мобильное приложение заблокировано')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Error.MobileAppBlocked', 'Mobile app blocked')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Error.MobileAppBlocked.OpenInBrowser', 'Вы можете продолжить работу в веб-версии. Для этого в браузере перейдите по адресу')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Error.MobileAppBlocked.OpenInBrowser', 'You can continue working in the web version. To do this, go to the address in the browser')

GO--

if not exists (select * from  [Settings].[Settings] where name =  'LogoImageWidth')
begin
	INSERT INTO [Settings].[Settings] ([Name],[Value]) VALUES ('LogoImageWidth', '203')
	INSERT INTO [Settings].[Settings] ([Name],[Value]) VALUES ('LogoImageHeight', '22')
End

GO--

update cms.staticblock set Content = replace(Content, '<img src="images/payment.png" alt="" />', '<img src="images/payment.png" alt="" width="387" height="22" />')

GO--

UPDATE [Settings].[InternalSettings] SET [settingValue] = '10.0.6' WHERE [settingKey] = 'db_version'
