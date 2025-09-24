EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Services.Shipping.Yandex.IncompleteAddress', 'Укажите полный адрес доставки'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Services.Shipping.Yandex.IncompleteAddress', 'Enter the full delivery address'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Services.Shipping.Yandex.IntervalsNotAvailable', 'Указанный адрес не распознан или доставка по этому адресу недоступна'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Services.Shipping.Yandex.IntervalsNotAvailable', 'The specified address is not recognized or delivery to this address is unavailable'

GO--


SET IDENTITY_INSERT [Order].[ShippingReplaceGeo] ON 

IF NOT EXISTS (SELECT * FROM [Order].[ShippingReplaceGeo] WHERE [Id] = 52)
INSERT INTO [Order].[ShippingReplaceGeo] ([Id],[ShippingType],[InCountryName],[InCountryISO2],[InRegionName],[InCityName],[InDistrict],[OutCountryName],[OutRegionName],[OutCityName],[OutDistrict],[OutDistrictClear],[Enabled],[Sort],[InZip],[OutZip],[Comment])
VALUES (52,'FivePost','','RU','Республика Марий Эл','','','','Марий-Эл республика','','',0,1,0,'','','')

IF NOT EXISTS (SELECT * FROM [Order].[ShippingReplaceGeo] WHERE [Id] = 53)
INSERT INTO [Order].[ShippingReplaceGeo] ([Id],[ShippingType],[InCountryName],[InCountryISO2],[InRegionName],[InCityName],[InDistrict],[OutCountryName],[OutRegionName],[OutCityName],[OutDistrict],[OutDistrictClear],[Enabled],[Sort],[InZip],[OutZip],[Comment])
VALUES (53,'FivePost','','RU','Удмуртская Республика','','','','Удмуртия республика','','',0,1,0,'','','')


SET IDENTITY_INSERT [Order].[ShippingReplaceGeo] OFF

GO--

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'CMS.StaticPageCities') AND type in (N'U'))
BEGIN
    drop table CMS.StaticPageCities;
END

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Checkout.ClearShoppingCartBeforeBuyByLink', 'Очищать корзину перед добавлением товара по ссылке'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Checkout.ClearShoppingCartBeforeBuyByLink', 'Empty the shopping cart before adding an item via a link'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Checkout.ClearShoppingCartBeforeBuyByLink.Help', 'При добавлении товара в корзину по ссылке корзина будет очищена'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Checkout.ClearShoppingCartBeforeBuyByLink.Help', 'When adding an item to the cart via the link, the cart will be emptied.'

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Checkout.HideShippingNotAvailableForWarehouse', 'Скрывать доставку недоступную для склада'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Checkout.HideShippingNotAvailableForWarehouse', 'Hide a delivery that is unavailable to the warehouse' 

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Checkout.HideShippingNotAvailableForWarehouse.Help', 'Если позиция в заказе недоступна на заданных в доставке складах, то доставка будет скрыта.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Checkout.HideShippingNotAvailableForWarehouse.Help', 'If the item in the order is not available at the warehouses specified in the delivery, the delivery will be hidden.' 

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Shippings.ShippingOption.Error.HaveNotAvailableItems', 'Не все позиции доступны для этого метода'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Shippings.ShippingOption.Error.HaveNotAvailableItems', 'Not all positions are available for this method'

GO--

if exists (Select 1 From [Customers].[CustomerRoleAction] Where [RoleActionKey] = 'Settings')
begin
	Insert Into [Customers].[CustomerRoleAction] ([CustomerID], [RoleActionKey], [Enabled]) 
	Select distinct cra.CustomerID, 'CouponsAndDiscounts' as RoleActionKey, 1 as Enabled 
	From [Customers].[CustomerRoleAction] cra 
	Where cra.[RoleActionKey] = 'Settings' 
		  and not exists (Select 1 
						  From [Customers].[CustomerRoleAction] cra2 
						  Where cra2.CustomerID = cra.CustomerID and cra2.[RoleActionKey] = 'CouponsAndDiscounts')
end

GO--

IF NOT EXISTS(SELECT 1 
			  FROM [Settings].[Settings] 
			  WHERE Name = 'TEMP_fix_product_list_sort')
BEGIN
    UPDATE [Settings].[Settings]
    SET [Value] = CAST(CAST([Value] AS INT) * -1 AS NVARCHAR(MAX))
    WHERE [Name] = 'NewSorting'
       OR [Name] = 'BestSorting'
       OR [Name] = 'SalesSorting'
    
    UPDATE [Catalog].[ProductList]
    SET [SortOrder] = SortOrder * -1
    
    INSERT INTO [Settings].[Settings] (Name, Value)
    VALUES ('TEMP_fix_product_list_sort', '')
END


