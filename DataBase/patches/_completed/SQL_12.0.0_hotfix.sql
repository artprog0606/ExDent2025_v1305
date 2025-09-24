EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.ExportImport.Yandex.Platform', 'AdvantShop'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.ExportImport.Yandex.Platform', 'AdvantShop'

GO-- 

IF EXISTS (SELECT 1
		   FROM sys.columns
		   WHERE (name = N'AllCustomerGroupEnabled') AND object_id = OBJECT_ID(N'[Catalog].[Coupon]'))
BEGIN
ALTER TABLE [Catalog].[Coupon] DROP COLUMN [AllCustomerGroupEnabled]
END

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Orders.Order.ItemsCount', 'Количество позиций в заказе'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Orders.Order.ItemsCount', 'Items count' 

GO--

UPDATE [Order].[ShippingReplaceGeo]
SET [Enabled] = 0
WHERE [Id] = 16

    GO--

UPDATE [Order].[ShippingReplaceGeo]
SET [OutRegionName] = 'Чувашская Республика - Чувашия'
WHERE [Id] = 27