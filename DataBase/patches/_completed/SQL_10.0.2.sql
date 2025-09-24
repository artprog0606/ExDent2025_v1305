
UPDATE [Settings].[Localization] SET [ResourceValue] = 'По умолчанию выставлена новая версия, если у вас сайт версии ниже 7.0 , можете переключить на старую панель администрирования. Не рекомендуем этого делать, так как многие функции будут недоступны. Обновите магазин до актуальной версии' WHERE [ResourceKey] = 'Admin.SettingsSystem.SystemCommon.AdministrationPanelHint' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'By default, the new version is set, if you have a site version lower than 7.0, you can switch to the old administration panel. We do not recommend doing this, as many functions will be unavailable. Update your store to the latest version' WHERE [ResourceKey] = 'Admin.SettingsSystem.SystemCommon.AdministrationPanelHint' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Функция поиска фотографий позволяет искать фотографии по названию товара прямо из панели администрирования<br><br>Подробнее:<br><a target="_blank" href="https://www.advantshop.net/help/pages/poisk-izobrazhenii-advantshop">Поиск изображений Advantshop</a>' WHERE [ResourceKey] = 'Admin.Settings.SystemSettings.ShowImageSearchEnabledNote' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'Photo search function allows you to search for photos by product name directly from the administration panel <br> <br> More: <br> <a target = "_ blank" href = "https://www.advantshop.net/help/pages/poisk - advantshop-image "> Advantshop Image Search </a>' WHERE [ResourceKey] = 'Admin.Settings.SystemSettings.ShowImageSearchEnabledNote' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Настройка позволяет добавить пункт меню в панель администрирования для быстрого перехода к настройке, которой вы чаще всего пользуетесь' WHERE [ResourceKey] = 'Admin.Settings.SystemSettings.CustomMenuHint' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'Customization allows you to add a menu item to the administration panel to quickly navigate to the setting that you use most often' WHERE [ResourceKey] = 'Admin.Settings.SystemSettings.CustomMenuHint' AND [LanguageId] = 2

GO-- 

SET IDENTITY_INSERT [Order].[ShippingReplaceGeo] ON 


IF NOT EXISTS (SELECT * FROM [Order].[ShippingReplaceGeo] WHERE [Id] = 25)
INSERT INTO [Order].[ShippingReplaceGeo] ([Id],[ShippingType],[InCountryName],[InCountryISO2],[InRegionName],[InCityName],[InDistrict],[OutCountryName],[OutRegionName],[OutCityName],[OutDistrict],[OutDistrictClear],[Enabled],[Sort],[InZip],[OutZip],[Comment])
VALUES (25,'Sdek','','RU','Кемеровская область','','','','Кемеровская область - Кузбасс ','','',0,1,0,'','','')

IF NOT EXISTS (SELECT * FROM [Order].[ShippingReplaceGeo] WHERE [Id] = 26)
INSERT INTO [Order].[ShippingReplaceGeo] ([Id],[ShippingType],[InCountryName],[InCountryISO2],[InRegionName],[InCityName],[InDistrict],[OutCountryName],[OutRegionName],[OutCityName],[OutDistrict],[OutDistrictClear],[Enabled],[Sort],[InZip],[OutZip],[Comment])
VALUES (26,'Sdek','','RU','Москва','Десна','','','Московская обл.','','',0,1,0,'','','Десна в Москве в СДЭК числится в МО')


SET IDENTITY_INSERT [Order].[ShippingReplaceGeo] OFF

GO--



INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Category.AdminStaticPageModel.Error.SeoTitle', 'Укажите заголовок')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Category.AdminStaticPageModel.Error.SeoTitle', 'Enter Title')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Category.AdminStaticPageModel.Error.SeoH1', 'Укажите H1')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Category.AdminStaticPageModel.Error.SeoH1', 'Enter H1')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'UseGlobalVariables', 'Доступные переменные для SEO:')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'UseGlobalVariables', 'Available variables for SEO:')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Js.Inplace.UseDefaultMeta', 'Использовать Meta по умолчанию')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Js.Inplace.UseDefaultMeta', 'Use default Meta')

GO--

Update [Settings].[Settings] Set [Value] = 'True' Where Name = 'DashboardActive'

GO--

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Title страницы' 
	WHERE [ResourceKey] IN ('Admin.Settings.SeoSettings.DefaultTitle',
							'Admin.Settings.SeoSettings.CategoriesDefaultTitle',
							'Admin.Settings.SeoSettings.TagsDefaultTitle',
							'Admin.Settings.SeoSettings.ProductsDefaultTitle',
							'Admin.Settings.SeoSettings.StaticPageDefaultTitle',
							'Admin.Settings.SeoSettings.CategoryNewsDefaultTitle',
							'Admin.Settings.SeoSettings.NewsDefaultTitle',
							'Admin.Settings.SeoSettings.BrandsDefaultTitle',
							'Admin.Settings.SeoSettings.BrandItemDefaultTitle',
							'Admin.Settings.SeoSettings.MainPageProductsDefaultTitle',
							'Admin.Category.Index.SeoTitle',
							'Admin.Product.SEO.Title',
							'Admin.Js.AddEditNewsCategpry.MetaHeader',
							'Admin.Js.EditMainPageList.MetaHeader',
							'Admin.Js.AddEditProductList.MetaHeader')
	AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'Page title' 
	WHERE [ResourceKey] IN ('Admin.Settings.SeoSettings.DefaultTitle',
							'Admin.Settings.SeoSettings.CategoriesDefaultTitle',
							'Admin.Settings.SeoSettings.TagsDefaultTitle',
							'Admin.Settings.SeoSettings.ProductsDefaultTitle',
							'Admin.Settings.SeoSettings.StaticPageDefaultTitle',
							'Admin.Settings.SeoSettings.CategoryNewsDefaultTitle',
							'Admin.Settings.SeoSettings.NewsDefaultTitle',
							'Admin.Settings.SeoSettings.BrandsDefaultTitle',
							'Admin.Settings.SeoSettings.BrandItemDefaultTitle',
							'Admin.Settings.SeoSettings.MainPageProductsDefaultTitle',
							'Admin.Category.Index.SeoTitle',
							'Admin.Product.SEO.Title',
							'Admin.Js.AddEditNewsCategpry.MetaHeader',
							'Admin.Js.EditMainPageList.MetaHeader',
							'Admin.Js.AddEditProductList.MetaHeader')
	AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Заголовок H1' 
	WHERE [ResourceKey] IN ('Admin.Settings.SeoSettings.CategoriesDefaultH1',
							'Admin.Settings.SeoSettings.TagsDefaultH1',
							'Admin.Settings.SeoSettings.ProductsDefaultH1',
							'Admin.Settings.SeoSettings.StaticPageDefaultH1',
							'Admin.Settings.SeoSettings.CategoryNewsDefaultH1',
							'Admin.Settings.SeoSettings.NewsDefaultH1',
							'Admin.Settings.SeoSettings.BrandItemDefaultH1',
							'Admin.Settings.SeoSettings.MainPageProductsDefaultH1',
							'Admin.Category.Index.SeoH1',
							'Admin.Js.AddEditNewsCategpry.H1',
							'Admin.Js.EditMainPageList.H1',
							'Admin.Js.AddEditProductList.H1')
	AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'H1 header' 
	WHERE [ResourceKey] IN ('Admin.Settings.SeoSettings.CategoriesDefaultH1',
							'Admin.Settings.SeoSettings.TagsDefaultH1',
							'Admin.Settings.SeoSettings.ProductsDefaultH1',
							'Admin.Settings.SeoSettings.StaticPageDefaultH1',
							'Admin.Settings.SeoSettings.CategoryNewsDefaultH1',
							'Admin.Settings.SeoSettings.NewsDefaultH1',
							'Admin.Settings.SeoSettings.BrandItemDefaultH1',
							'Admin.Settings.SeoSettings.MainPageProductsDefaultH1',
							'Admin.Category.Index.SeoH1',
							'Admin.Js.AddEditNewsCategpry.H1',
							'Admin.Js.EditMainPageList.H1',
							'Admin.Js.AddEditProductList.H1')
	AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Ключевые слова' 
	WHERE [ResourceKey] IN ('Admin.Category.Index.SeoKeywords',
							'Admin.Product.Seo.Keywords',
							'Admin.Js.AddEditNewsCategpry.MetaKeywords',
							'Admin.Js.EditMainPageList.MetaKeywords',
							'Admin.Js.AddEditProductList.MetaKeywords')
	AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'Keywords' 
	WHERE [ResourceKey] IN ('Admin.Category.Index.SeoKeywords',
							'Admin.Product.Seo.Keywords',
							'Admin.Js.AddEditNewsCategpry.MetaKeywords',
							'Admin.Js.EditMainPageList.MetaKeywords',
							'Admin.Js.AddEditProductList.MetaKeywords')
	AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Мета описание' 
	WHERE [ResourceKey] IN ('Admin.Category.Index.SeoDescription',
							'Admin.Product.Seo.MetaDescription',
							'Admin.Js.AddEditNewsCategpry.MetaDescription',
							'Admin.Js.EditMainPageList.MetaDescription',
							'Admin.Js.AddEditProductList.MetaDescription')
	AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'Meta Description' 
	WHERE [ResourceKey] IN ('Admin.Category.Index.SeoDescription',
							'Admin.Product.Seo.MetaDescription',
							'Admin.Js.AddEditNewsCategpry.MetaDescription',
							'Admin.Js.EditMainPageList.MetaDescription',
							'Admin.Js.AddEditProductList.MetaDescription')
	AND [LanguageId] = 2

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Product.SEO.H1', 'Заголовок H1')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Product.SEO.H1', 'H1 header')

GO--

UPDATE [Settings].[InternalSettings] SET [settingValue] = '10.0.2' WHERE [settingKey] = 'db_version'
