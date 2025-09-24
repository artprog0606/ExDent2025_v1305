IF NOT EXISTS(SELECT * FROM sys.indexes WHERE name = 'ProductExportOptions_ProductId_index' AND object_id = OBJECT_ID('[Catalog].[ProductExportOptions]'))
BEGIN
    CREATE CLUSTERED INDEX ProductExportOptions_ProductId_index
        ON Catalog.ProductExportOptions (ProductId)
END

GO--

DELETE FROM [Order].[PaymentMethod]
WHERE PaymentType IN (
    'Interkassa'
    ,'MailRu'
    ,'QIWI'
    ,'Check'
    ,'Rbkmoney'
    ,'AmazonSimplePay'
    ,'AuthorizeNet'
    ,'BitPay'
    ,'ChronoPay'
    ,'CyberPlat'
    ,'Dibs'
    ,'eWAY'
    ,'GateLine'
    ,'GoogleCheckout'
    ,'Moneybookers'
    ,'MoneXy'
    ,'PayPal'
    ,'PayPoint'
    ,'PSIGate'
    ,'SagePay'
    ,'TwoCheckout'
    ,'WorldPay'
    ,'Qppi'
    ,'MoscowBank'
    )

GO--

UPDATE Settings.Localization
SET [ResourceValue] = N'оплачен'
WHERE [ResourceKey] = 'Core.Orders.Order.OrderPaid'
  AND [LanguageId] = 1;

IF NOT EXISTS(SELECT 1
              FROM [Settings].[Localization]
              WHERE [ResourceKey] = 'Core.Orders.Order.OrderPaid'
                AND [LanguageId] = 2)
BEGIN
INSERT INTO [Settings].[Localization] ([LanguageId], [ResourceKey], [ResourceValue])
VALUES (2, 'Core.Orders.Order.OrderPaid', 'paid')
END


IF NOT EXISTS(SELECT 1
              FROM [Settings].[Localization]
              WHERE [ResourceKey] = 'Core.Orders.Order.OrderNotPaid'
                AND [LanguageId] = 1)
BEGIN
INSERT INTO [Settings].[Localization] ([LanguageId], [ResourceKey], [ResourceValue])
VALUES (1, 'Core.Orders.Order.OrderNotPaid', N'оплачен')
END

UPDATE Settings.Localization
SET [ResourceValue] = 'not paid'
WHERE [ResourceKey] = 'Core.Orders.Order.OrderNotPaid'
  AND [LanguageId] = 2;

GO--