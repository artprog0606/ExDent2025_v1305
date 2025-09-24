UPDATE [Settings].[Localization] SET [ResourceValue] = 'Перемещено в спам' WHERE [ResourceKey] = 'Core.Loging.EmailStatus.Spam' AND [LanguageId] = 1

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.Analytics.ExportProducts.EnterProductArtno', 'Введите артикул товара или модификации')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.Analytics.ExportProducts.EnterProductArtno', 'Enter product or offer article')

GO--

if not exists(Select 1 From [Settings].[TemplateSettings] Where [Template] = 'Modern' and Name = 'ModernMainpageAdditionalText')
	Insert Into [Settings].[TemplateSettings] (Template, Name, Value) Values ('Modern', 'ModernMainpageAdditionalText', 'False')

GO--

if not exists(Select 1 From [Settings].[TemplateSettings] Where [Template] = 'Modern' and Name = 'ModernMainpageAdvantages')
	Insert Into [Settings].[TemplateSettings] (Template, Name, Value) Values ('Modern', 'ModernMainpageAdvantages', 'False')

GO--


Update [Settings].[Settings] Set [Value] = 'True' Where Name = 'Features.EnableNewDashboard'

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
		SELECT TOP(@rowsCount) Product.ProductID, Product.Name, Product.UrlPath, Ratio, isnull(PhotoNameSize1, PhotoName) as PhotoName,
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

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Js.ShippingTemplate.SelectCourier', 'Выбрать курьерскую службу')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Js.ShippingTemplate.SelectCourier', 'Select courier delivery')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Js.ShippingTemplate.SelectCourier.NotSelected', 'Необходимо выбрать курьерскую службу')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Js.ShippingTemplate.SelectCourier.NotSelected', 'You must select a courier delivery')

GO--


IF not EXISTS(SELECT 1 FROM sys.columns WHERE Name = N'PriceMarginInPercents' AND Object_ID = Object_ID(N'[Settings].[ExportFeedSettings]'))
BEGIN
	ALTER TABLE Settings.ExportFeedSettings ADD
		PriceMarginInPercents float(53) NULL,
		PriceMarginInNumbers float(53) NULL
END

GO--

IF EXISTS(SELECT 1 FROM sys.columns WHERE Name = N'PriceMargin' AND Object_ID = Object_ID(N'[Settings].[ExportFeedSettings]'))
BEGIN
	Update [Settings].[ExportFeedSettings] 
	Set PriceMarginInPercents = PriceMargin, PriceMarginInNumbers = 0
	Where [PriceMarginType] = 0
END
GO--

IF EXISTS(SELECT 1 FROM sys.columns WHERE Name = N'PriceMargin' AND Object_ID = Object_ID(N'[Settings].[ExportFeedSettings]'))
BEGIN
	Update [Settings].[ExportFeedSettings] 
	Set PriceMarginInPercents = 0, PriceMarginInNumbers = PriceMargin
	Where [PriceMarginType] = 1
END
GO--

IF EXISTS(SELECT 1 FROM sys.columns WHERE Name = N'PriceMargin' AND Object_ID = Object_ID(N'[Settings].[ExportFeedSettings]'))
BEGIN
    ALTER TABLE [Settings].[ExportFeedSettings] 
		DROP COLUMN [PriceMargin]
END

GO--

IF EXISTS(SELECT 1 FROM sys.columns WHERE Name = N'PriceMarginType' AND Object_ID = Object_ID(N'[Settings].[ExportFeedSettings]'))
BEGIN
	ALTER TABLE [Settings].[ExportFeedSettings] 
		DROP COLUMN [PriceMarginType]
END

GO--

UPDATE [Settings].[InternalSettings] SET [settingValue] = '10.0.5' WHERE [settingKey] = 'db_version'