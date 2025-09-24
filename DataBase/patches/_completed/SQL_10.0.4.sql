GO--
IF NOT EXISTS (SELECT * FROM [Settings].[Localization] WHERE [ResourceKey] = 'Admin.Js.ReferralLink.ModalReferralLink.CopyTheLink')
BEGIN
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.ReferralLink.ModalReferralLink.CopyTheLink', 'Скопировать ссылку')
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.ReferralLink.ModalReferralLink.CopyTheLink', 'Copy the link')
END

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ExportFeed.SettingsYandex.NotExportAmountCount', 'Не выгружать товар, если в наличии менее')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ExportFeed.SettingsYandex.NotExportAmountCount', 'Do not export product if amount less than')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ExportFeed.SettingsYandex.NotExportAmountCountUnit', 'шт.')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ExportFeed.SettingsYandex.NotExportAmountCountUnit', 'items')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ExportFeed.SettingsYandex.NotExportAmountCountHelp', 'Не выгружать товар, если в наличии менее n штук. Если ничего не указано или 0, то опция не учитывается.')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ExportFeed.SettingsYandex.NotExportAmountCountHelp', 'Do not export product if amount less than n items in stock. If nothing is specified or 0, the option is ignored.')

GO--

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Наценка' WHERE [ResourceKey] = 'Admin.ExportFeeed.Settings.PriceMargin' AND [LanguageId] = 1

GO--

If not Exists(Select 1 From sys.columns WHERE Name = N'PriceMarginType' AND Object_ID = Object_ID(N'Settings.ExportFeedSettings'))
Begin
	ALTER TABLE Settings.ExportFeedSettings ADD
		PriceMarginType int NULL
End

GO--

ALTER TABLE [Order].OrderTrafficSource ADD
	YandexClientId nvarchar(50) NULL

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.SalesChannels.SalesChannel.FacebookFeed', 'Facebook')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.SalesChannels.SalesChannel.FacebookFeed', 'Facebook')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.SalesChannels.SalesChannel.FacebookFeed.Description', 'Автоматически выгружайте товары из своего интернет-магазина прямо в раздел «Магазин» на Facebook. Любые изменения, внесенные в каталог товаров, мгновенно обновляются и на страничке в соцсети.')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.SalesChannels.SalesChannel.FacebookFeed.Description', 'Automatically upload products from your online store directly to the "Store" section on Facebook. Any changes made to the product catalog are instantly updated on the page in the social network.')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Home.Menu.FacebookFeed', 'Facebook')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Home.Menu.FacebookFeed', 'Facebook')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.SalesChannels.SalesChannel.FacebookFeed.Details.Title', 'Facebook')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.SalesChannels.SalesChannel.FacebookFeed.Details.Title', 'Facebook')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.SalesChannels.SalesChannel.FacebookFeed.Details.Text', 'Автоматически выгружайте товары из своего интернет-магазина прямо в раздел «Магазин» на Facebook. Любые изменения, внесенные в каталог товаров, мгновенно обновляются и на страничке в соцсети.')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.SalesChannels.SalesChannel.FacebookFeed.Details.Text', 'Automatically upload products from your online store directly to the "Store" section on Facebook. Any changes made to the product catalog are instantly updated on the page in the social network.')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Customers.RoleActionCategory.FacebookFeed', 'Facebook')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Customers.RoleActionCategory.FacebookFeed', 'Facebook')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ExportFeeds.IndexFacebook.H1', 'Facebook')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ExportFeeds.IndexFacebook.H1', 'Facebook')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ExportFeeds.IndexFacebook.subtitle', 'Канал продаж Facebook')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ExportFeeds.IndexFacebook.subtitle', 'Facebook sales channel')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ExportFeeds.IndexFacebook.Instruction', 'Инструкция. Выгрузка товаров в Facebook')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ExportFeeds.IndexFacebook.Instruction', 'Instruction. Uploading products to Facebook')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.ExportImport.ExportFeed.Facebook', 'Facebook (xml)')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.ExportImport.ExportFeed.Facebook', 'Facebook (xml)')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Catalog.UnloadingToFacebook', 'Выгрузка в Facebook')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Catalog.UnloadingToFacebook', 'Export to Facebook')

GO--


UPDATE [Settings].[InternalSettings] SET [settingValue] = '10.0.4' WHERE [settingKey] = 'db_version'
