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

GO--

IF NOT EXISTS(SELECT 1
              FROM sys.columns
              WHERE (name = N'TypeOfDelivery') AND object_id = OBJECT_ID(N'[Order].[ShippingMethod]'))
BEGIN
    ALTER TABLE [Order].[ShippingMethod] ADD
        [TypeOfDelivery] int NULL
END

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Checkout.SplitShippingByType', 'Разделить доставку по типу'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Checkout.SplitShippingByType', 'Split shipping by type' 
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Checkout.SplitShippingByTypeHint', 'Доставка будет разделена на Самовывоз и Доставка'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Checkout.SplitShippingByTypeHint', 'Delivery will be divided into Pickup and Delivery' 
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Checkout.CheckoutShipping.SelfDelivery', 'Самовывоз'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Checkout.CheckoutShipping.SelfDelivery', 'Selfdelivery' 
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Checkout.CheckoutShipping.Delivery', 'Доставка'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Checkout.CheckoutShipping.Delivery', 'Delivery' 
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'AdvantShop.Core.Shipping.EnTypeOfDelivery.Courier', 'Курьер'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'AdvantShop.Core.Shipping.EnTypeOfDelivery.Courier', 'Courier' 
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'AdvantShop.Core.Shipping.EnTypeOfDelivery.SelfDelivery', 'Самовывоз'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'AdvantShop.Core.Shipping.EnTypeOfDelivery.SelfDelivery', 'Selfвelivery' 
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.ShippingMethods.Common.TypeOfDelivery', 'Тип доставки'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.ShippingMethods.Common.TypeOfDelivery', 'TypeOfDelivery' 
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'AdvantShop.Shipping.AdminModel.TypesOfDelivery.NotSet', 'Укажите тип'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'AdvantShop.Shipping.AdminModel.TypesOfDelivery.NotSet', 'Specify the type' 

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.ExportImport.Yandex.Platform', 'AdvantShop'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.ExportImport.Yandex.Platform', 'AdvantShop' 

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.AddEditCoupon.CustomerGroup', 'Группа пользователя'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.AddEditCoupon.CustomerGroup', 'Customer group'

GO--

IF NOT EXISTS(SELECT 1
			  FROM sys.columns
			  WHERE object_id = OBJECT_ID(N'[Catalog].[CouponCustomerGroup]'))
BEGIN
    CREATE TABLE [Catalog].[CouponCustomerGroup](
        [CouponId] int NOT NULL,
        [CustomerGroupId] int NOT NULL
         CONSTRAINT [PK_CouponCustomerGroup] PRIMARY KEY CLUSTERED (
        [CouponID] ASC,
    [CustomerGroupId] ASC
    ) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
        CONSTRAINT [FK_CouponCustomerGroup_Coupon] FOREIGN KEY ([CouponId]) REFERENCES [Catalog].[Coupon] ([CouponID]) ON DELETE CASCADE,
        CONSTRAINT [FK_CouponCustomerGroup_CustomerGroup] FOREIGN KEY ([CustomerGroupId]) REFERENCES [Customers].[CustomerGroup] ([CustomerGroupId]) ON DELETE CASCADE
        ) ON [PRIMARY]
    
    INSERT INTO [Catalog].[CouponCustomerGroup]
    SELECT [CouponId], 1 FROM [Catalog].[Coupon]
END

GO--

IF EXISTS (SELECT 1
		   FROM sys.columns
		   WHERE (name = N'AllCustomerGroupEnabled') AND object_id = OBJECT_ID(N'[Catalog].[Coupon]'))
BEGIN
    ALTER TABLE [Catalog].[Coupon] DROP COLUMN [AllCustomerGroupEnabled]
END

GO--

IF EXISTS (SELECT 1
		   FROM sys.columns
		   WHERE (name = N'CustomerGroupId') AND object_id = OBJECT_ID(N'[Catalog].[Coupon]'))
BEGIN
    ALTER TABLE [Catalog].[Coupon] DROP COLUMN [CustomerGroupId]
END

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.ShippingMethods.PointDelivery.PointListTitle', 'Название списка'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.ShippingMethods.PointDelivery.PointListTitle', 'Name of the list' 

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.ShippingMethods.PointDelivery.PointListTitleHint', 'Текст отображаемый над списком.<br><br>Например: Пункт самовывоза.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.ShippingMethods.PointDelivery.PointListTitleHint', 'The text displayed above the list.<br><br>For example: Pick-up point.' 

GO--

UPDATE [CMS].[StaticBlock]
SET [Content] = replace([Content], '<div class="footer-payment">
                    <a href="#">
                        <img src="images/payment.png" alt="" width="387" height="22" /></a></div>',
			'<div class="footer-payment__list">
                            <img class="footer-payment__item" src="./images/payment/mastercard_icon.svg" alt="mastercard" />
                            <img class="footer-payment__item" src="./images/payment/visa_icon.svg" alt="visa" />
                            <img class="footer-payment__item" src="./images/payment/mir-logo.svg" alt="mir" />
                        </div>')
WHERE [Key] = 'RightBottom';

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.Reviews.Rating', 'Рейтинг'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.Reviews.Rating', 'Rating' 

GO--

IF NOT EXISTS (SELECT 1
		   FROM sys.columns
		   WHERE (name = N'Id') AND object_id = OBJECT_ID(N'[Order].OrderPaymentInfo'))
	BEGIN
		DROP INDEX [OrderPaymentInfo_PaymentMethodID] ON [Order].[OrderPaymentInfo]

		ALTER TABLE [Order].[OrderPaymentInfo] ADD 
		[Id] int NOT NULL IDENTITY (1, 1)

		ALTER TABLE [Order].[OrderPaymentInfo] ADD CONSTRAINT
		PK_OrderPaymentInfo PRIMARY KEY CLUSTERED 
		(
		Id
		) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

		CREATE NONCLUSTERED INDEX [OrderPaymentInfo_PaymentMethodID] ON [Order].[OrderPaymentInfo]
		(
			[PaymentMethodID] ASC
		) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

		CREATE NONCLUSTERED INDEX OrderPaymentInfo_OrderID ON [Order].OrderPaymentInfo
		(
		OrderID
		) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	END

GO--

UPDATE [Settings].[InternalSettings] SET [settingValue] = '12.0.1' WHERE [settingKey] = 'db_version'
