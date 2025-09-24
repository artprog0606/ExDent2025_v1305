INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.Order.CancelOzonRocketOrder', N'Отменить заказ в Ozon Rocket? Это действие необратимо.')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.Order.CancelOzonRocketOrder', 'Cancel an order at Ozon Rocket? This action is irreversible.')



GO--

ALTER TABLE Customers.TelegramUser
    ALTER COLUMN Id bigint NOT NULL

GO--

SET IDENTITY_INSERT [Order].[ShippingReplaceGeo] ON 

IF NOT EXISTS (SELECT * FROM [Order].[ShippingReplaceGeo] WHERE [Id] = 27)
INSERT INTO [Order].[ShippingReplaceGeo] ([Id],[ShippingType],[InCountryName],[InCountryISO2],[InRegionName],[InCityName],[InDistrict],[OutCountryName],[OutRegionName],[OutCityName],[OutDistrict],[OutDistrictClear],[Enabled],[Sort],[InZip],[OutZip],[Comment])
VALUES (27,'Sdek','','RU','Чувашская Республика','','','','Чувашия','','',0,1,0,'','','')


IF NOT EXISTS (SELECT * FROM [Order].[ShippingReplaceGeo] WHERE [Id] = 28)
INSERT INTO [Order].[ShippingReplaceGeo] ([Id],[ShippingType],[InCountryName],[InCountryISO2],[InRegionName],[InCityName],[InDistrict],[OutCountryName],[OutRegionName],[OutCityName],[OutDistrict],[OutDistrictClear],[Enabled],[Sort],[InZip],[OutZip],[Comment])
VALUES (28,'Sdek','','RU','Астраханская область','Знаменск','','','','Знаменск ЗАТО','',0,1,0,'','','')


SET IDENTITY_INSERT [Order].[ShippingReplaceGeo] OFF

GO--


INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.PaymentMethods.Bill.RequiredPaymentDetails', 'Сделать обязательным заполнение ИНН и название компании')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.PaymentMethods.Bill.RequiredPaymentDetails', 'Request details in the client side')

GO--


UPDATE [Settings].[InternalSettings] SET [settingValue] = '10.0.9' WHERE [settingKey] = 'db_version'

