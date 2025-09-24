

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) 
VALUES
		(1,'MyAccount.SaveUserInfo.ErrorRequiredFields', 'Заполните обязательные поля'),
		(2,'MyAccount.SaveUserInfo.ErrorRequiredFields', 'Fill in all required fields'),
		(1,'MyAccount.SaveUserInfo.Error', 'Ошибка при сохранении'),
		(2,'MyAccount.SaveUserInfo.Error', 'Error on saving')

GO--

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Артикул' WHERE [ResourceKey] = 'Core.ExportImport.ProductFields.Sku' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'SKU' WHERE [ResourceKey] = 'Core.ExportImport.ProductFields.Sku' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Артикул модификации:Размер:Цвет:Цена:ЗакупочнаяЦена:Наличие' WHERE [ResourceKey] = 'Core.ExportImport.ProductFields.MultiOffer' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'OfferSKU:Size:Color:Price:PurchasePrice:Amount' WHERE [ResourceKey] = 'Core.ExportImport.ProductFields.MultiOffer' AND [LanguageId] = 2

GO--



IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Catalog].[sp_RemoveOffer]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [Catalog].[sp_RemoveOffer] AS' 
END

GO--

-- =============================================
-- Author:		<RuslanZV>
-- Create date: <17.05.2022>
-- Description:	<Удаляет оффер товара>
-- =============================================
ALTER PROCEDURE [Catalog].[sp_RemoveOffer] 
	-- Add the parameters for the stored procedure here
	@OfferID as int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @ProductId as int;

	SELECT TOP (1) @ProductId = [ProductID] FROM [Catalog].[Offer] WHERE [OfferID] = @OfferID

	IF @ProductId IS NOT NULL
	BEGIN
		-- offer exists
		DELETE FROM [Catalog].[Offer] WHERE [OfferID] = @OfferID;
		declare @temp int;
		set @temp=(CASE WHEN EXISTS(select * from [Catalog].[Offer] where [ProductID]=@ProductId and [Main]=1) THEN 1 ELSE 0 END);
		if @temp=0
		BEGIN
			UPDATE TOP (1) [Catalog].[Offer] 
			SET [Main]=1 
			WHERE [ProductID]=@ProductId
		END 
	END
END

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Template.DefaultSortOrderProductInBrand','Сортировка товаров по умолчанию')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Template.DefaultSortOrderProductInBrand','Sorting products by default')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Template.DefaultSortOrderProductInBrandHelp','При открытии страницы бренда, товары будут отсортированы по умолчанию')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Template.DefaultSortOrderProductInBrandHelp','When opening brand pages, products will be sorted by default')

GO--

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Загрузить изображение' WHERE [ResourceKey] = 'Js.Inplace.AddPicture' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'Download picture' WHERE [ResourceKey] = 'Js.Inplace.AddPicture' AND [LanguageId] = 2

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Checkout.Success.ProceedToPayment', 'Сейчас вы будете перенаправлены на страницу оплаты <span class="icon-spinner-before icon-animate-spin-before checkout-success-progress"></span><br>Если этого не произошло, нажмите на кнопку ниже')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Checkout.Success.ProceedToPayment', 'You will now be redirected to the payment page <span class="icon-spinner-before icon-animate-spin-before checkout-success-progress"></span><br>If this did not happen, click on the button below')

GO--

UPDATE [Settings].[InternalSettings] SET [settingValue] = '10.0.13' WHERE [settingKey] = 'db_version'