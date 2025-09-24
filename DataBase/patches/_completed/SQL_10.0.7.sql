
update cms.staticblock set Content = replace(Content, '<img alt="Мы принимаем следующие виды оплаты" src="./areas/mobile/images/cards.png" />', '<img alt="Мы принимаем следующие виды оплаты" src="./areas/mobile/images/cards.png" width="299" height="22" />')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Js.Cart.Pcs', 'шт')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Js.Cart.Pcs', 'pcs')

GO--

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Перемещено в спам' WHERE [ResourceKey] = 'Core.Loging.EmailStatus.Spam' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'Введите артикул товара или модификации' WHERE [ResourceKey] = 'Admin.Js.Analytics.ExportProducts.EnterProductArtno' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'Выбрать курьерскую службу' WHERE [ResourceKey] = 'Js.ShippingTemplate.SelectCourier' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'Необходимо выбрать курьерскую службу' WHERE [ResourceKey] = 'Js.ShippingTemplate.SelectCourier.NotSelected' AND [LanguageId] = 1

GO--

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Мобильное приложение заблокировано' WHERE [ResourceKey] = 'Error.MobileAppBlocked' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'Вы можете продолжить работу в веб-версии. Для этого в браузере перейдите по адресу' WHERE [ResourceKey] = 'Error.MobileAppBlocked.OpenInBrowser' AND [LanguageId] = 1

GO--

UPDATE [Settings].[InternalSettings] SET [settingValue] = '10.0.7' WHERE [settingKey] = 'db_version'