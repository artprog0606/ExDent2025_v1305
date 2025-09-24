EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.GeoMode.ShippingPoint.Change', 'Изменить'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.GeoMode.ShippingPoint.Change', 'Change'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.GeoMode.ShippingPoint.SelectPickupPoint', 'Выберите точку самовывоза'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.GeoMode.ShippingPoint.SelectPickupPoint', 'Select pickup point'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.GeoMode.ShippingPoint.DeliverAddressTitle', 'Доставим по адресу'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.GeoMode.ShippingPoint.DeliverAddressTitle', 'We will deliver to your address'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.GeoMode.ShippingPoint.PickPointAddressTitle', 'Точка самовывоза'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.GeoMode.ShippingPoint.PickPointAddressTitle', 'Pickup point'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.ExportImport.MultiOrder.CustomOptions', 'Дополнительные опции'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.ExportImport.MultiOrder.CustomOptions', 'Custom options'

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.MainPageProducts.CopyTheLink', 'Скопировать ссылку'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.MainPageProducts.CopyTheLink', 'Copy the link'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.MainPageProducts.LinkCopiedToClipboard', 'Ссылка успешно скопирована'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.MainPageProducts.LinkCopiedToClipboard', 'Link copied to clipboard'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.MainPageProducts.FailedToCopyLink', 'Не удалось скопировать ссылку'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.MainPageProducts.FailedToCopyLink', 'Failed to copy link'

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.Landing.Cart.Return', 'Вернуться'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.Landing.Cart.Return', 'Return'

GO--

SET IDENTITY_INSERT [Order].[ShippingReplaceGeo] ON 


IF NOT EXISTS (SELECT * FROM [Order].[ShippingReplaceGeo] WHERE [Id] = 51)
INSERT INTO [Order].[ShippingReplaceGeo] ([Id],[ShippingType],[InCountryName],[InCountryISO2],[InRegionName],[InCityName],[InDistrict],[OutCountryName],[OutRegionName],[OutCityName],[OutDistrict],[OutDistrictClear],[Enabled],[Sort],[InZip],[OutZip],[Comment])
VALUES (51,'Boxberry','','RU','Республика Саха (Якутия)','','','','Республика Саха','','',0,1,0,'','','')


SET IDENTITY_INSERT [Order].[ShippingReplaceGeo] OFF

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Home.SettingStoreDashboard.HaveTrial', 'У вас подключен пробный период. Выберите тарифный план с помесячной или годовой оплатой со скидкой.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Home.SettingStoreDashboard.HaveTrial', 'You have a trial period enabled. Choose a monthly or annual plan with a discount.'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Home.CongratulationsDashboard.HaveTrial', 'У вас подключен пробный период. Выберите тарифный план с помесячной или годовой оплатой со скидкой.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Home.CongratulationsDashboard.HaveTrial', 'You have a trial period enabled. Choose a monthly or annual plan with a discount.'

GO--

If not Exists (SELECT 1 
				 FROM INFORMATION_SCHEMA.COLUMNS
				WHERE 
					TABLE_NAME = 'OrderCustomOptions' 
					AND TABLE_SCHEMA = 'Order'
					AND COLUMN_NAME = 'OptionTitle'
					AND DATA_TYPE = 'nvarchar'
					AND CHARACTER_MAXIMUM_LENGTH = 2000)
BEGIN
    ALTER TABLE [Order].[OrderCustomOptions] ALTER COLUMN [OptionTitle] nvarchar(2000) NOT NULL
END

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Bonuses.OnPurchaseTempalte.PurchaseFull', 'Стоимость товаров заказа, участвующих в бонусной программе'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Bonuses.OnPurchaseTempalte.PurchaseFull', 'Cost of order items participating in the bonus program'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Bonuses.OnPurchaseTempalte.Purchase', 'Стоимость заказа c учетом скидки и товаров, участвующих в бонусной программе '
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Bonuses.OnPurchaseTempalte.Purchase', 'Cost of order items participating in the bonus program and discount'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Bonuses.OnPurchaseTempalte.TotalSum', 'Итоговая стоимость заказа'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Bonuses.OnPurchaseTempalte.PurchaseFull', 'Order total cost'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Bonuses.OnPurchaseTempalte.ProductsSum', 'Стоимость товаров заказа'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Bonuses.OnPurchaseTempalte.ProductsSum', 'Order products cost'

GO--


IF NOT EXISTS (SELECT * FROM Catalog.Tax WHERE TaxType = 6)
begin
    insert into Catalog.Tax (Name, Enabled, ShowInPrice, Rate, TaxType)
    values (N'НДС 5%',1,1,5, 6),
           (N'НДС 7%',1,1,7, 7)
end

GO--

UPDATE [Settings].[InternalSettings] SET [settingValue] = '13.0.2' WHERE [settingKey] = 'db_version'
