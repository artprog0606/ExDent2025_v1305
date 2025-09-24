
CREATE TABLE Vk.VkCategoryImport
	(
	CategoryId int NOT NULL,
	AlbumId bigint NOT NULL
	)  ON [PRIMARY]
GO--

ALTER TABLE Vk.VkCategoryImport ADD CONSTRAINT
	FK_VkCategoryImport_Category FOREIGN KEY
	(
	CategoryId
	) REFERENCES Catalog.Category
	(
	CategoryID
	) ON UPDATE  NO ACTION 
	 ON DELETE  CASCADE 	 
GO--

Delete From [Vk].[VkProduct] Where not exists (Select 1 From [Catalog].[Product] Where [Product].[ProductId] = [VkProduct].[ProductId])

GO--

ALTER TABLE Vk.VkProduct ADD CONSTRAINT
	FK_VkProduct_Product FOREIGN KEY
	(
	ProductId
	) REFERENCES Catalog.Product
	(
	ProductId
	) ON UPDATE  NO ACTION 
	 ON DELETE  CASCADE 
	
GO--

ALTER PROCEDURE [Catalog].[sp_AddProductToCategory] 
	@ProductId int,
	@CategoryId int,
	@SortOrder int,
	@MainCategory bit
AS
BEGIN

DECLARE @Main bit
	SET NOCOUNT ON;

if(@MainCategory = 1) 
	Begin
		Set @Main = @MainCategory
		Update [Catalog].[ProductCategories] Set main = 0 Where ProductID=@ProductID
	end

if (select count(*) from [Catalog].[ProductCategories] Where ProductID=@ProductID and main=1) = 0
	set @Main = 1
else
	set @Main = 0

if (Select count(*) from [Catalog].[ProductCategories] Where CategoryID=@CategoryID and ProductID=@ProductID) = 0 
begin
	Insert Into [Catalog].[ProductCategories] (CategoryID, ProductID, SortOrder, Main) Values (@CategoryID, @ProductID, @SortOrder, @Main);
end
Else
	Update [Catalog].[ProductCategories] Set Main=@Main Where CategoryID=@CategoryID and ProductID=@ProductID

END

GO--

CREATE TABLE [CRM].[LeadField](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[FieldType] [int] NOT NULL,
	[SortOrder] [int] NOT NULL,
	[Required] [bit] NOT NULL,
	[Enabled] [bit] NOT NULL,
    [SalesFunnelId] [int] NOT NULL,
 CONSTRAINT [PK_LeadField] PRIMARY KEY CLUSTERED
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]

GO--

ALTER TABLE [CRM].[LeadField] WITH CHECK ADD CONSTRAINT [FK_LeadField_SalesFunnel] FOREIGN KEY([SalesFunnelId])
REFERENCES [CRM].[SalesFunnel] ([Id])
ON DELETE CASCADE

GO--

CREATE TABLE [CRM].[LeadFieldValue](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[LeadFieldId] [int] NOT NULL,
	[Value] [nvarchar](255) NOT NULL,
	[SortOrder] [int] NOT NULL,
 CONSTRAINT [PK_LeadFieldValue] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]

GO--

ALTER TABLE [CRM].[LeadFieldValue] WITH CHECK ADD CONSTRAINT [FK_LeadFieldValue_LeadField] FOREIGN KEY([LeadFieldId])
REFERENCES [CRM].[LeadField] ([Id])
ON DELETE CASCADE

GO--

CREATE TABLE [CRM].[LeadFieldValuesMap](
	[LeadId] [int] NOT NULL,
	[LeadFieldId] [int] NOT NULL,
	[Value] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_LeadFieldValues] PRIMARY KEY CLUSTERED 
(
	[LeadId] ASC,
	[LeadFieldId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO--

ALTER TABLE [CRM].[LeadFieldValuesMap] WITH CHECK ADD CONSTRAINT [FK_LeadFieldValues_Lead] FOREIGN KEY([LeadId])
REFERENCES [Order].[Lead] ([Id])
ON DELETE CASCADE

GO--

ALTER TABLE [CRM].[LeadFieldValuesMap] WITH CHECK ADD CONSTRAINT [FK_LeadFieldValues_LeadField] FOREIGN KEY([LeadFieldId])
REFERENCES [CRM].[LeadField] ([Id])
ON DELETE CASCADE

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Crm.LeadFields.ELeadFieldType.Select','Выбор')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Crm.LeadFields.ELeadFieldType.Select','Select')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Crm.LeadFields.ELeadFieldType.Text','Текстовое поле')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Crm.LeadFields.ELeadFieldType.Text','Text')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Crm.LeadFields.ELeadFieldType.Number','Числовое поле')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Crm.LeadFields.ELeadFieldType.Number','Number')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Crm.LeadFields.ELeadFieldType.TextArea','Многострочное текстовое поле')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Crm.LeadFields.ELeadFieldType.TextArea','Multiline text')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Crm.LeadFields.ELeadFieldType.Date','Дата')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Crm.LeadFields.ELeadFieldType.Date','Date')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.AddEditLeadField.Active','Активно')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.AddEditLeadField.Active','Active')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.AddEditLeadField.NewField','Новое поле')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.AddEditLeadField.NewField','New field')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.AddEditLeadField.EditField','Редактирование поля')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.AddEditLeadField.EditField','Editing a field')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.AddEditLeadField.FieldValues','Значения поля')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.AddEditLeadField.FieldValues','Field Values')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.AddEditLeadField.Name','Название')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.AddEditLeadField.Name','Name')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.AddEditLeadField.Required','Обязательное')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.AddEditLeadField.Required','Required')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.AddEditLeadField.Type','Тип')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.AddEditLeadField.Type','Type')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.LeadFields.Active','Актив.')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.LeadFields.Active','Active')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.LeadFields.Name','Название')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.LeadFields.Name','Name')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.LeadFields.Required','Обяз.')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.LeadFields.Required','Required')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.LeadFields.Type','Тип')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.LeadFields.Type','Type')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.LeadField.Errors.NameReauired','Укажите название поля')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.LeadField.Errors.NameReauired','Lead field name reauired')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'App.Landing.Domain.Forms.ELpFormFieldType.LeadField','Доп. поле лида')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'App.Landing.Domain.Forms.ELpFormFieldType.LeadField','Lead field')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.SalesFunnel.Errors.NotFound','Список лидов не найден')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.SalesFunnel.Errors.NotFound','Leads list not found')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.SalesFunnel.Errors.MissingDealStatuses','Укажите этапы сделки')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.SalesFunnel.Errors.MissingDealStatuses','Missing deal statuses')

GO--

UPDATE Settings.Localization SET ResourceValue = 'Невозможно применить купон или сертификат' WHERE ResourceKey = 'Checkout.CheckoutCart.CouponNotApplied' AND LanguageId = 1
UPDATE Settings.Localization SET ResourceValue = 'You cannot apply the coupon' WHERE ResourceKey = 'Checkout.CheckoutCart.CouponNotApplied' AND LanguageId = 2

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.System.Menu','Меню')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.System.Menu','Menu')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Error.FileNotFound','Файл не найден')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Error.FileNotFound','File not found')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Menus.RootElement','Корневой элемент')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Menus.RootElement','Root element')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Menus.Errors.NameRequired','Укажите название')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Menus.Errors.NameRequired','Name required')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.SystemSettings.CustomMenu','Дополнительные элементы меню')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.SystemSettings.CustomMenu','Additional menu items')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Design.Index.ThemeHote', ' <a href="https://www.advantshop.net/help/pages/upload-theme" target="_blank">Инструкция. Тема дизайна</a>')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Design.Index.ThemeHote', ' <a href="https://www.advantshop.net/help/pages/upload-theme" target="_blank">Instruction. Design theme</a>')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Design.Index.BackgroundHote', ' <a href="https://www.advantshop.net/help/pages/upload-background" target="_blank">Инструкция. Фон дизайна</a>')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Design.Index.BackgroundHote', ' <a href="https://www.advantshop.net/help/pages/upload-background" target="_blank">Instruction. Design background</a>')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Design.Index.ColorSchemeHote', ' <a href="https://www.advantshop.net/help/pages/upload-color" target="_blank">Инструкция. Цветовая схема</a>')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Design.Index.ColorSchemeHote', ' <a href="https://www.advantshop.net/help/pages/upload-color" target="_blank">Instruction. Color Scheme</a>')
UPDATE [Settings].[Localization] Set [ResourceValue] = 'Настройка позволяет отображать на главной странице 3 категории товаров: Хит продаж, Новинки, Скидки.<br/><br/> <a href="https://www.advantshop.net/help/pages/product-on-main" target="_blank">Инструкция. Товары на главной</a>'Where [ResourceKey] = 'Admin.Settings.Template.MainPageProductsVisibilityHelp' and [LanguageId] = 1
UPDATE [Settings].[Localization] Set [ResourceValue] = 'The setting allows you to display on the main page 3 categories of products: Bestsellers, Novelties, Discounts.<br/><br/><a href="https://www.advantshop.net/help/pages/product-on-main" target="_blank">Instruction. Goods on the main</a>'Where [ResourceKey] = 'Admin.Settings.Template.MainPageProductsVisibilityHelp' and [LanguageId] = 2
UPDATE [Settings].[Localization] Set [ResourceValue] = 'Настройка отвечает за количество товара, которое будет отображаться на главной странице в каждом блоке.<br/><br/> <a href="https://www.advantshop.net/help/pages/product-on-main#6" target="_blank">Инструкция. Как изменить количество товаров на главной</a>'Where [ResourceKey] = 'Admin.Settings.Template.CountMainPageProductInSectionHelp' and [LanguageId] = 1
UPDATE [Settings].[Localization] Set [ResourceValue] = 'The setting is responsible for the quantity of goods that will be displayed on the main page in each block.<br/><br/><a href="https://www.advantshop.net/help/pages/product-on-main#6" target="_blank">Instruction. How to change the number of products on the main</a>'Where [ResourceKey] = 'Admin.Settings.Template.CountMainPageProductInSectionHelp' and [LanguageId] = 2
UPDATE [Settings].[Localization] Set [ResourceValue] = 'Настройка отвечает за количество товара, которое будет отображаться в одной строке каждого блока на главной странице.<br/><br/> <a href="https://www.advantshop.net/help/pages/product-on-main#6" target="_blank">Инструкция. Как изменить количество товаров на главной</a>'Where [ResourceKey] = 'Admin.Settings.Template.CountMainPageProductInLineHelp' and [LanguageId] = 1
UPDATE [Settings].[Localization] Set [ResourceValue] = 'The setting is responsible for the quantity of goods that will be displayed in one line of each block on the main page.<br/><br/><a href="https://www.advantshop.net/help/pages/product-on-main#6" target="_blank">Instruction. How to change the number of products on the main</a>'Where [ResourceKey] = 'Admin.Settings.Template.CountMainPageProductInLineHelp' and [LanguageId] = 2
UPDATE [Settings].[Localization] Set [ResourceValue] = 'Настройка активирует отображение раздела новостей на главной странице<br/><br/> <a href="https://www.advantshop.net/help/pages/add-news" target="_blank">Инструкция. Настройка раздела новости на сайте</a>'Where [ResourceKey] = 'Admin.Settings.Template.NewsVisibilityHelp' and [LanguageId] = 1
UPDATE [Settings].[Localization] Set [ResourceValue] = 'The setting activates the display of the news section on the main page<br/><br/><a href="https://www.advantshop.net/help/pages/add-news" target="_blank">Instruction. Setting up the news section on the site</a>'Where [ResourceKey] = 'Admin.Settings.Template.NewsVisibilityHelp' and [LanguageId] = 2
UPDATE [Settings].[Localization] Set [ResourceValue] = 'Настройка позволяет отобразить блок "проверка статуса заказа" на главную страницу<br/><br/> <a href="https://www.advantshop.net/help/pages/statusy-zakazov#2" target="_blank">Инструкция. Статусы заказов</a>'Where [ResourceKey] = 'Admin.Settings.Template.CheckOrderVisibilityHelp' and [LanguageId] = 1
UPDATE [Settings].[Localization] Set [ResourceValue] = 'The setting allows you to display the block "check order status" on the main page<br/><br/><a href="https://www.advantshop.net/help/pages/statusy-zakazov#2" target="_blank">Instruction. Order Statuses</a>'Where [ResourceKey] = 'Admin.Settings.Template.CheckOrderVisibilityHelp' and [LanguageId] = 2
UPDATE [Settings].[Localization] Set [ResourceValue] = 'Включенная галочка активирует настройка вывода блока "Голосование/Опрос" на главную страницу<br/><br/> <a href="https://www.advantshop.net/help/pages/vote" target="_blank">Инструкция. Раздел опроса</a>'Where [ResourceKey] = 'Admin.Settings.Template.VotingVisibilityHelp' and [LanguageId] = 1
UPDATE [Settings].[Localization] Set [ResourceValue] = 'An enabled checkbox activates the setting of the "Voting / Polling" output block to the main page<br/><br/><a href="https://www.advantshop.net/help/pages/vote" target="_blank">Instruction. Poll section</a>'Where [ResourceKey] = 'Admin.Settings.Template.VotingVisibilityHelp' and [LanguageId] = 2
UPDATE [Settings].[Localization] Set [ResourceValue] = 'Настройка выводит блок подарочных сертификатов на главную страницу<br/><br/> <a href="https://www.advantshop.net/help/pages/kupons-i-podaroxhnie-certifikaty#2" target="_blank">Инструкция. Купоны и подарочные сертификаты</a>'Where [ResourceKey] = 'Admin.Settings.Template.GiftSertificateVisibilityHelp' and [LanguageId] = 1
UPDATE [Settings].[Localization] Set [ResourceValue] = 'Setting displays a block of gift certificates on the main page<br/><br/><a href="https://www.advantshop.net/help/pages/kupons-i-podaroxhnie-certifikaty#2" target="_blank">Instruction. Coupons and Gift Vouchers</a>'Where [ResourceKey] = 'Admin.Settings.Template.GiftSertificateVisibilityHelp' and [LanguageId] = 2
UPDATE [Settings].[Localization] Set [ResourceValue] = 'Настройка позволяет вывести на главную страницу карусель производителей (логотипы производителей)<br/><br/> <a href="https://www.advantshop.net/help/pages/brand#5" target="_blank">Инструкция. Бренды</a>'Where [ResourceKey] = 'Admin.Settings.Template.BrandCarouselVisibilityHelp' and [LanguageId] = 1
UPDATE [Settings].[Localization] Set [ResourceValue] = 'The setting allows you to display the carousel of manufacturers on the main page (manufacturers logos)<br/><br/><a href="https://www.advantshop.net/help/pages/brand#5" target="_blank">Instruction. Brands</a>'Where [ResourceKey] = 'Admin.Settings.Template.BrandCarouselVisibilityHelp' and [LanguageId] = 2
UPDATE [Settings].[Localization] Set [ResourceValue] = 'Включенная галочка в настройке позволяет вывести блок "подписка на новости" на главную страницу<br/><br/> <a href="https://www.advantshop.net/help/pages/add-news#3" target="_blank">Инструкция. Подписка на новости</a>'Where [ResourceKey] = 'Admin.Settings.Template.NewsSubscriptionVisibilityHelp' and [LanguageId] = 1
UPDATE [Settings].[Localization] Set [ResourceValue] = 'NThe included checkmark in the setting allows you to display the "subscribe to news" block on the main page<br/><br/><a href="https://www.advantshop.net/help/pages/add-news#3" target="_blank">Instruction. News subscription</a>'Where [ResourceKey] = 'Admin.Settings.Template.NewsSubscriptionVisibilityHelp' and [LanguageId] = 2
UPDATE [Settings].[Localization] Set [ResourceValue] = 'Настройка позволяет отображать список товаров, которые клиенты просматривали ранее на сайте<br/><br/> <a href="https://www.advantshop.net/help/pages/template-settings#6" target="_blank">Инструкция. Вы уже смотрели</a>'Where [ResourceKey] = 'Admin.Settings.Template.RecentlyViewVisibilityHint' and [LanguageId] = 1
UPDATE [Settings].[Localization] Set [ResourceValue] = 'The setting allows you to display a list of products that customers viewed previously on the site.<br/><br/><a href="https://www.advantshop.net/help/pages/template-settings#6" target="_blank">Instruction. You already watched</a>'Where [ResourceKey] = 'Admin.Settings.Template.RecentlyViewVisibilityHint' and [LanguageId] = 2
UPDATE [Settings].[Localization] Set [ResourceValue] = 'Активность данной настройки позволяет клиентам добавлять товары в свой список желаний.<br/><br/> <a href="https://www.advantshop.net/help/pages/template-settings#6" target="_blank">Инструкция. Вы уже смотрели</a>'Where [ResourceKey] = 'Admin.Settings.Template.WishListVisibilityHint' and [LanguageId] = 1
UPDATE [Settings].[Localization] Set [ResourceValue] = 'The activity of this setting allows customers to add products to their wish list.<br/><br/><a href="https://www.advantshop.net/help/pages/template-settings#6" target="_blank">Instruction. You already watched</a>'Where [ResourceKey] = 'Admin.Settings.Template.WishListVisibilityHint' and [LanguageId] = 2
UPDATE [Settings].[Localization] Set [ResourceValue] = 'Настройка позволяет выводить для всех посетителей сайта всплывающее окно с уведомлением о том, что для функционирования сайта Вы осуществляете сбор данных пользователя (cookie, данные об IP-адресе и местоположении) в соответствии с законом 152-ФЗ.<br/><br/> <a href="https://www.advantshop.net/help/pages/152-fz" target="_blank">Инструкция. Как соблюсти требования закона 152-ФЗ на платформе AdvantShop</a>'Where [ResourceKey] = 'Admin.Settings.SystemSettings.ShowCookiesPolicyMessageNote' and [LanguageId] = 1
UPDATE [Settings].[Localization] Set [ResourceValue] = 'The setting allows you to display a pop-up window for all site visitors with a notification that for the operation of the site you are collecting user data (cookies, IP address and location data) in accordance with Law 152-FZ.<br/><br/><a href="https://www.advantshop.net/help/pages/152-fz" target="_blank">Instruction. How to comply with the requirements of law 152-ФЗ on the AdvantShop platform</a>'Where [ResourceKey] = 'Admin.Settings.SystemSettings.ShowCookiesPolicyMessageNote' and [LanguageId] = 2
UPDATE [Settings].[Localization] Set [ResourceValue] = 'Настройка позволяет сменить язык магазина (меняются кнопки, панель администрирования, товары останутся на том языке, на котором заполняли магазин).<br/><br/> <a href="https://www.advantshop.net/help/pages/change-buttons#4" target="_blank">Инструкция. Изменение языка магазина</a>'Where [ResourceKey] = 'Admin.Settings.SystemSettings.SiteLanguageNote' and [LanguageId] = 1
UPDATE [Settings].[Localization] Set [ResourceValue] = 'The setting allows you to change the language of the store (buttons, admin panel change, the products will remain in the language in which the store was filled).<br/><br/><a href="https://www.advantshop.net/help/pages/change-buttons#4" target="_blank">Instruction. Change store language</a>'Where [ResourceKey] = 'Admin.Settings.SystemSettings.SiteLanguageNote' and [LanguageId] = 2
UPDATE [Settings].[Localization] Set [ResourceValue] = 'Используйте файл, только если вы хорошо владеете навыками <a href="https://www.advantshop.net/help/pages/work-with-css" target="_blank"> работы с CSS</a>.'Where [ResourceKey] = 'Admin.Design.CssEditor.UseFile' and [LanguageId] = 1
UPDATE [Settings].[Localization] Set [ResourceValue] = 'Use the file only if you are good at <a href="https://www.advantshop.net/help/pages/work-with-css" target="_blank">working with CSS</a>. 'Where [ResourceKey] = 'Admin.Design.CssEditor.UseFile' and [LanguageId] = 2
UPDATE [Settings].[Localization] Set [ResourceValue] = 'Выводить дерево категорий <br/>(требуется больше ресурсов) ' Where [ResourceKey] = 'Admin.Settings.Template.ShowCategoryTreeInBrand' and [LanguageId] = 1
UPDATE [Settings].[Localization] Set [ResourceValue] = 'Display categories treeview <br />(more resources required)' Where [ResourceKey] = 'Admin.Settings.Template.ShowCategoryTreeInBrand' and [LanguageId] = 2
UPDATE [Settings].[Localization] Set [ResourceValue] = 'Исключающие фильтры <br />(требуется больше ресурсов)' Where [ResourceKey] = 'Admin.Settings.Catalog.ExluderingFilters' and [LanguageId] = 1
UPDATE [Settings].[Localization] Set [ResourceValue] = 'Exludering Filters <br />(more resources required)' Where [ResourceKey] = 'Admin.Settings.Catalog.ExluderingFilters' and [LanguageId] = 2
UPDATE [Settings].[Localization] Set [ResourceValue] = 'Учитывать в клиентском каталоге разные цены, цвета, фотографии у одного товара <br />(требуется больше ресурсов)' Where [ResourceKey] = 'Admin.Settings.Catalog.ComplexFilter' and [LanguageId] = 1
UPDATE [Settings].[Localization] Set [ResourceValue] = 'Observe in client side different sizes, colors, product photos <br />(more resources required)' Where [ResourceKey] = 'Admin.Settings.Catalog.ComplexFilter' and [LanguageId] = 2

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Errors.UnsupportedImageFormat','Неподдерживаемый формат изображения')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Errors.UnsupportedImageFormat','Unsupported image format')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Errors.FailedToAccessFile','Не удалось получить доступ к файлу')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Errors.FailedToAccessFile','Failed to access file')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Errors.FileNotFound','Файл не найден')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Errors.FileNotFound','File not found')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'App.Landing.Domain.Forms.ELpFormFieldType.Picture','Файл изображения')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'App.Landing.Domain.Forms.ELpFormFieldType.Picture','Picture file')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.AddEditLeadField.Type.NotSelected','Не выбран')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.AddEditLeadField.Type.NotSelected','Not selected')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Crm.ELeadFieldType.LeadField','Доп. поле лида')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Crm.ELeadFieldType.LeadField','Customer field')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ExportFeeed.SettingsYandex.VendorCodeType','Выгружать в качестве vendorCode')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ExportFeeed.SettingsYandex.VendorCodeType','Export as vendorCode')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ExportFeed.Settings.ProductArtNo','Артикул товара')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ExportFeed.Settings.ProductArtNo','Product artno')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ExportFeed.Settings.OfferArtNo','Артикул модификации')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ExportFeed.Settings.OfferArtNo','Offer artno')

GO--
ALTER TABLE CMS.LandingForm ADD AgreementDefaultChecked bit NULL

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Checkout.AgreementDefaultChecked','Соглашение принято по умолчанию')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Checkout.AgreementDefaultChecked','Agreement accepted by default')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.MainPageProductsStore.Index.DisplayLatestProducts','Если нет товаров в списке, отображать самые последние добавленные товары')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.MainPageProductsStore.Index.DisplayLatestProducts','If there are no products in the list, display the most recently added products')

GO--

DELETE FROM [Settings].[TemplateSettings] WHERE [Name] = 'Mobile_LevelMenu' OR [Name] = 'Mobile_LevelMenu_Modern'

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Compare.Index.Brand','Производитель')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Compare.Index.Brand','Brand')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Orders.OrderItem.Sku','Артикул')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Orders.OrderItem.Sku','Sku')

GO--

UPDATE [Settings].[Localization] Set [ResourceValue] = 'Комментарий администратора' WHERE [ResourceKey] = 'Core.ExportImport.MultiOrder.AdminComment' AND [LanguageId] = 1

GO--

UPDATE [Settings].[Localization] Set [ResourceKey] = 'Admin.Settings.Social.Widgets' Where [ResourceKey] = 'Admin.Settings.Social.SocialWidget'
UPDATE [Settings].[Localization] Set [ResourceKey] = 'Admin.Settings.Social.SocialWidget' Where [ResourceKey] = 'Admin.Settings.Template.SocialWidget'
UPDATE [Settings].[Localization] Set [ResourceKey] = 'Admin.Settings.Social.SocialWidget.Title' Where [ResourceKey] = 'Admin.Settings.Template.SocialWidget.Title'
UPDATE [Settings].[Localization] Set [ResourceKey] = 'Admin.Settings.Social.SocialWidget.Help' Where [ResourceKey] = 'Admin.Settings.Template.SocialWidget.Help'
UPDATE [Settings].[Localization] Set [ResourceKey] = 'Admin.Settings.Social.SocialWidget.ShowCommunicationWidget' Where [ResourceKey] = 'Admin.Settings.Template.SocialWidget.ShowCommunicationWidget'
UPDATE [Settings].[Localization] Set [ResourceKey] = 'Admin.Settings.Social.SocialWidget.ShowCommunicationWidget.HelpText' Where [ResourceKey] = 'Admin.Settings.Template.SocialWidget.ShowCommunicationWidget.HelpText'
UPDATE [Settings].[Localization] Set [ResourceKey] = 'Admin.Settings.Social.SocialWidget.ShowVkontakte' Where [ResourceKey] = 'Admin.Settings.Template.SocialWidget.ShowVkontakte'
UPDATE [Settings].[Localization] Set [ResourceKey] = 'Admin.Settings.Social.SocialWidget.IfIntegrationIsConfigured' Where [ResourceKey] = 'Admin.Settings.Template.SocialWidget.IfIntegrationIsConfigured'
UPDATE [Settings].[Localization] Set [ResourceKey] = 'Admin.Settings.Social.SocialWidget.ShowVkontakte.HelpText' Where [ResourceKey] = 'Admin.Settings.Template.SocialWidget.ShowVkontakte.HelpText'
UPDATE [Settings].[Localization] Set [ResourceKey] = 'Admin.Settings.Social.SocialWidget.ShowFaceBook' Where [ResourceKey] = 'Admin.Settings.Template.SocialWidget.ShowFaceBook'
UPDATE [Settings].[Localization] Set [ResourceKey] = 'Admin.Settings.Social.SocialWidget.IfIntegrationIsConfigured' Where [ResourceKey] = 'Admin.Settings.Template.SocialWidget.IfIntegrationIsConfigured'
UPDATE [Settings].[Localization] Set [ResourceKey] = 'Admin.Settings.Social.SocialWidget.ShowFaceBook.HelpText' Where [ResourceKey] = 'Admin.Settings.Template.SocialWidget.ShowFaceBook.HelpText'
UPDATE [Settings].[Localization] Set [ResourceKey] = 'Admin.Settings.Social.SocialWidget.ShowJivosite' Where [ResourceKey] = 'Admin.Settings.Template.SocialWidget.ShowJivosite'
UPDATE [Settings].[Localization] Set [ResourceKey] = 'Admin.Settings.Social.SocialWidget.IfJivositeModuleIsConnected' Where [ResourceKey] = 'Admin.Settings.Template.SocialWidget.IfJivositeModuleIsConnected'
UPDATE [Settings].[Localization] Set [ResourceKey] = 'Admin.Settings.Social.SocialWidget.ShowJivosite.HelpText' Where [ResourceKey] = 'Admin.Settings.Template.SocialWidget.ShowJivosite.HelpText'
UPDATE [Settings].[Localization] Set [ResourceKey] = 'Admin.Settings.Social.SocialWidget.ShowCallback' Where [ResourceKey] = 'Admin.Settings.Template.SocialWidget.ShowCallback'
UPDATE [Settings].[Localization] Set [ResourceKey] = 'Admin.Settings.Social.SocialWidget.IfCallBackModuleIsConnected' Where [ResourceKey] = 'Admin.Settings.Template.SocialWidget.IfCallBackModuleIsConnected'
UPDATE [Settings].[Localization] Set [ResourceKey] = 'Admin.Settings.Social.SocialWidget.ShowCallback.HelpText' Where [ResourceKey] = 'Admin.Settings.Template.SocialWidget.ShowCallback.HelpText'
UPDATE [Settings].[Localization] Set [ResourceKey] = 'Admin.Settings.Social.SocialWidget.ShowWhatsApp' Where [ResourceKey] = 'Admin.Settings.Template.SocialWidget.ShowWhatsApp'
UPDATE [Settings].[Localization] Set [ResourceKey] = 'Admin.Settings.Social.SocialWidget.OnlyMobileVersion' Where [ResourceKey] = 'Admin.Settings.Template.SocialWidget.OnlyMobileVersion'
UPDATE [Settings].[Localization] Set [ResourceKey] = 'Admin.Settings.Social.SocialWidget.ShowWhatsApp.HelpText' Where [ResourceKey] = 'Admin.Settings.Template.SocialWidget.ShowWhatsApp.HelpText'
UPDATE [Settings].[Localization] Set [ResourceKey] = 'Admin.Settings.Social.SocialWidget.WhatsApp.HelpText' Where [ResourceKey] = 'Admin.Settings.Template.SocialWidget.WhatsApp.HelpText'
UPDATE [Settings].[Localization] Set [ResourceKey] = 'Admin.Settings.Social.SocialWidget.ShowViber' Where [ResourceKey] = 'Admin.Settings.Template.SocialWidget.ShowViber'
UPDATE [Settings].[Localization] Set [ResourceKey] = 'Admin.Settings.Social.SocialWidget.OnlyMobileVersion' Where [ResourceKey] = 'Admin.Settings.Template.SocialWidget.OnlyMobileVersion'
UPDATE [Settings].[Localization] Set [ResourceKey] = 'Admin.Settings.Social.SocialWidget.ShowViber.HelpText' Where [ResourceKey] = 'Admin.Settings.Template.SocialWidget.ShowViber.HelpText'
UPDATE [Settings].[Localization] Set [ResourceKey] = 'Admin.Settings.Social.SocialWidget.Viber.HelpText' Where [ResourceKey] = 'Admin.Settings.Template.SocialWidget.Viber.HelpText'
UPDATE [Settings].[Localization] Set [ResourceKey] = 'Admin.Settings.Social.SocialWidget.ShowOdnoklassniki' Where [ResourceKey] = 'Admin.Settings.Template.SocialWidget.ShowOdnoklassniki'
UPDATE [Settings].[Localization] Set [ResourceKey] = 'Admin.Settings.Social.SocialWidget.ShowOdnoklassniki.HelpText' Where [ResourceKey] = 'Admin.Settings.Template.SocialWidget.ShowOdnoklassniki.HelpText'
UPDATE [Settings].[Localization] Set [ResourceKey] = 'Admin.Settings.Social.SocialWidget.ShowTelegram' Where [ResourceKey] = 'Admin.Settings.Template.SocialWidget.ShowTelegram'
UPDATE [Settings].[Localization] Set [ResourceKey] = 'Admin.Settings.Social.SocialWidget.ShowTelegram.HelpText' Where [ResourceKey] = 'Admin.Settings.Template.SocialWidget.ShowTelegram.HelpText'
UPDATE [Settings].[Localization] Set [ResourceKey] = 'Admin.Settings.Social.SocialWidget.CustomWidgetTitleNote' Where [ResourceKey] = 'Admin.Settings.Template.SocialWidget.CustomWidgetTitleNote'
UPDATE [Settings].[Localization] Set [ResourceKey] = 'Admin.Settings.Social.SocialWidget.Name' Where [ResourceKey] = 'Admin.Settings.Template.SocialWidget.Name'
UPDATE [Settings].[Localization] Set [ResourceKey] = 'Admin.Settings.Social.SocialWidget.UrlAddressOfLink' Where [ResourceKey] = 'Admin.Settings.Template.SocialWidget.UrlAddressOfLink'
UPDATE [Settings].[Localization] Set [ResourceKey] = 'Admin.Settings.Social.SocialWidget.CustomWidget1' Where [ResourceKey] = 'Admin.Settings.Template.SocialWidget.CustomWidget1'
UPDATE [Settings].[Localization] Set [ResourceKey] = 'Admin.Settings.Social.SocialWidget.CustomWidget2' Where [ResourceKey] = 'Admin.Settings.Template.SocialWidget.CustomWidget2'
UPDATE [Settings].[Localization] Set [ResourceKey] = 'Admin.Settings.Social.SocialWidget.CustomWidget3' Where [ResourceKey] = 'Admin.Settings.Template.SocialWidget.CustomWidget3'

GO--

UPDATE [Settings].[Localization] Set [ResourceValue] = 'Впишите число - количество новостей отображаемых на главной странице.<br/><br/> <a href="https://www.advantshop.net/help/pages/add-news" target="_blank">Инструкция. Настройка раздела новости на сайте.</a>'Where [ResourceKey] = 'Admin.Settings.News.NewsMainPageCountNote' and [LanguageId] = 1
UPDATE [Settings].[Localization] Set [ResourceValue] = 'Enter the number - the number of news displayed on the main page.<br/><br/><a href="https://www.advantshop.net/help/pages/add-news" target="_blank">Instruction. Setting up the news section on the site.</a>'Where [ResourceKey] = 'Admin.Settings.News.NewsMainPageCountNote' and [LanguageId] = 2
UPDATE [Settings].[Localization] Set [ResourceValue] = 'Максимальные размеры изображений производителей (в пикселях).'Where [ResourceKey] = 'Admin.Settings.Template.BrandLogoHote' and [LanguageId] = 1
UPDATE [Settings].[Localization] Set [ResourceValue] = 'Maximum producer image sizes (in pixels).'Where [ResourceKey] = 'Admin.Settings.Template.BrandLogoHote' and [LanguageId] = 2
UPDATE [Settings].[Localization] Set [ResourceValue] = 'Максимальные размеры изображений новостей (в пикселях).<br/><br/> <a href="https://www.advantshop.net/help/pages/add-news" target="_blank">Инструкция. Настройка раздела новости на сайте.</a>'Where [ResourceKey] = 'Admin.Settings.Template.NewsImageHote' and [LanguageId] = 1
UPDATE [Settings].[Localization] Set [ResourceValue] = 'Maximum sizes of news images (in pixels).<br/><br/> <a href="https://www.advantshop.net/help/pages/add-news" target="_blank">Instruction. Setting up the news section on the site.</a>'Where [ResourceKey] = 'Admin.Settings.Template.NewsImageHote' and [LanguageId] = 2
UPDATE [Settings].[Localization] Set [ResourceValue] = 'Используется для подтверждения владения сайтом.<br /><br />Например:<br />  &lt;meta name="generator" content="AdVantShop.NET"&gt;<br /><br /> ! Не стоит использовать для вставки счётчиков посещаемости, для этого используйте статические блоки.'Where [ResourceKey] = 'Admin.Settings.SystemSettings.AdditionalHeadMetaTagNote' and [LanguageId] = 1
UPDATE [Settings].[Localization] Set [ResourceValue] = 'Used to confirm ownership of the site <br /> <br /> For example:<br />  &lt;Meta name = "generator" content = "AdVantShop.NET"&gt; <br /><br /> ! Do not use to insert counters attendance for that use static blocks.'Where [ResourceKey] = 'Admin.Settings.SystemSettings.AdditionalHeadMetaTagNote' and [LanguageId] = 2
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Catalog.ExluderingFiltersHote', ' Исключающие фильтры')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Catalog.ExluderingFiltersHote', ' Exludering Filters')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Social.SocialWidget.CustomWidgetTitle', 'Свой виджет')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Social.SocialWidget.CustomWidgetTitle', 'Custom widget')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Social.SocialWidget.TitleHote', 'Виджеты коммуникаций')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Social.SocialWidget.TitleHote', 'Communication Widgets')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Template.PaymentIconNote', 'Максимальные размеры иконки для оплаты (в пикселях).')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Template.PaymentIconNote', 'The maximum size of the icon for payment (in pixels).')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Template.ShowCategoryTreeInBrand.TitleHote', 'Выводить дерево категорий')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Template.ShowCategoryTreeInBrand.TitleHote', 'Display categories treeview')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Catalog.ComplexFilterHote', ' Учитывать в клиентском каталоге разные цены, цвета, фотографии у одного товара')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Catalog.ComplexFilterHote', ' Observe in client side different sizes, colors, product photos')
UPDATE [Settings].[Localization] Set [ResourceValue] = 'В данном поле укажите какое количество категорий необходимо выводить в каталоге в одной строке.<br/><br/> <a href="https://www.advantshop.net/help/pages/template-settings#4" target="_blank">Инструкция. Товары на главной.</a>'Where [ResourceKey] = 'Admin.SettingsCatalog.CatalogCommon.CountCategoriesInLineNote' and [LanguageId] = 1
UPDATE [Settings].[Localization] Set [ResourceValue] = 'In this field, specify how many categories should be displayed in the catalog on one line.<br/><br/><a href="https://www.advantshop.net/help/pages/template-settings#4" target="_blank">Instruction. Goods on the main.</a>'Where [ResourceKey] = 'Admin.SettingsCatalog.CatalogCommon.CountCategoriesInLineNote' and [LanguageId] = 2
UPDATE [Settings].[Localization] Set [ResourceValue] = 'Данная настройка позволяет просмотреть кратко карточку товара не переходя во внутрь.'Where [ResourceKey] = 'Admin.SettingsCatalog.CatalogCommon.ShowQuickViewNote' and [LanguageId] = 1
UPDATE [Settings].[Localization] Set [ResourceValue] = 'This setting allows you to view briefly the product card without going inside.'Where [ResourceKey] = 'Admin.SettingsCatalog.CatalogCommon.ShowQuickViewNote' and [LanguageId] = 2
UPDATE [Settings].[Localization] Set [ResourceValue] = 'Данная настройка позволяет выставить необходимое количество товаров отображающихся в одной строке в категории.<br/><br/> <a href="https://www.advantshop.net/help/pages/template-settings#5" target="_blank">Инструкция. Товары в каталоге.</a>'Where [ResourceKey] = 'Admin.Settings.Template.CountCatalogProductInLineNote' and [LanguageId] = 1
UPDATE [Settings].[Localization] Set [ResourceValue] = 'This setting allows you to set the required number of products displayed on one line in the category.<br/><br/><a href="https://www.advantshop.net/help/pages/template-settings#5" target="_blank">Instruction. Products in the catalog.</a>'Where [ResourceKey] = 'Admin.Settings.Template.CountCatalogProductInLineNote' and [LanguageId] = 2
UPDATE [Settings].[Localization] Set [ResourceValue] = 'В данном поле пишите рейтинг, который можно установить массово для всех товаров. Значения указываются от 1 до 5.<br/><br/> <a href="https://www.advantshop.net/help/pages/catalog-view#10" target="_blank">Инструкция. Как выставить рейтинг товаров вручную.</a>'Where [ResourceKey] = 'Admin.Settings.Catalog.SetAllProductsManualRatioNote' and [LanguageId] = 1
UPDATE [Settings].[Localization] Set [ResourceValue] = 'In this field, write a rating that can be set in bulk for all products.Values ??are from 1 to 5.<br/><br/><a href="https://www.advantshop.net/help/pages/catalog-view#10" target="_blank">Instruction. How to manually rate products.</a>'Where [ResourceKey] = 'Admin.Settings.Catalog.SetAllProductsManualRatioNote' and [LanguageId] = 2
UPDATE [Settings].[Localization] Set [ResourceValue] = 'Настройка позволяет вывести фильтр со свойствами в каталог. Другими словами настройка включает работу фильтра в магазине.<br/><br/> <a href="https://www.advantshop.net/help/pages/catalog-filtr#5" target="_blank">Инструкция. Как отключить фильтр производителей, фильтр по цене, по размеру, по цвету.</a>'Where [ResourceKey] = 'Admin.SettingsCatalog.CatalogCommon.ShowFilterNote' and [LanguageId] = 1
UPDATE [Settings].[Localization] Set [ResourceValue] = 'Setting allows you to display a filter with properties in a directory. In other words, the setting includes the filter in the store.<br/><br/><a href="https://www.advantshop.net/help/pages/catalog-filtr#5" target="_blank">Instruction. How to disable the filter manufacturers, filter for the price, size, color.</a>'Where [ResourceKey] = 'Admin.SettingsCatalog.CatalogCommon.ShowFilterNote' and [LanguageId] = 2
UPDATE [Settings].[Localization] Set [ResourceValue] = 'Функция пережатия позволяет изменять размеры изображений для ранее загруженных фотографий на сайт.<br/><br/> <a href="https://www.advantshop.net/help/pages/photo-product#2" target="_blank">Инструкция. Как изменить размер изображений?</a>'Where [ResourceKey] = 'Admin.SettingsCatalog.CatalogCommon.ResizeGoodsPicturesNote' and [LanguageId] = 1
UPDATE [Settings].[Localization] Set [ResourceValue] = 'The compression function allows you to resize images for previously uploaded photos to the site.<br/><br/><a href="https://www.advantshop.net/help/pages/photo-product#2" target="_blank">Instruction. How to resize images?</a>'Where [ResourceKey] = 'Admin.SettingsCatalog.CatalogCommon.ResizeGoodsPicturesNote' and [LanguageId] = 2
UPDATE [Settings].[Localization] Set [ResourceValue] = 'В поле пишите то количество, которое требуется для отображения на одной страниц.<br/><br/> <a href="https://www.advantshop.net/help/pages/brand#4" target="_blank">Инструкция. Как настроить отображение товаров на странице "Бренды".</a>'Where [ResourceKey] = 'Admin.Settings.Template.BrandsPerPageHote' and [LanguageId] = 1
UPDATE [Settings].[Localization] Set [ResourceValue] = 'In the field, write the amount that is required to display on one page.<br/><br/><a href="https://www.advantshop.net/help/pages/brand#4" target="_blank">Instruction. How to customize the display of products on the Brands page.</a>'Where [ResourceKey] = 'Admin.Settings.Template.BrandsPerPageHote' and [LanguageId] = 2
UPDATE [Settings].[Localization] Set [ResourceValue] = 'Настройка позволяет выводить на странице бренда список товаров данного бренда. <br/><br/> <a href="https://www.advantshop.net/help/pages/brand#4" target="_blank">Инструкция. Как настроить отображение товаров на странице "Бренды".</a>'Where [ResourceKey] = 'Admin.Settings.Template.ShowProductsInBrandHote' and [LanguageId] = 1
UPDATE [Settings].[Localization] Set [ResourceValue] = 'The setting allows you to display on the brand page a list of products of this brand.<br/><br/><a href="https://www.advantshop.net/help/pages/brand#4" target="_blank">Instruction. How to customize the display of products on the Brands page.</a>'Where [ResourceKey] = 'Admin.Settings.Template.ShowProductsInBrandHote' and [LanguageId] = 2
UPDATE [Settings].[Localization] Set [ResourceValue] = 'Настройка позволяет на странице бренда отображать категории, в которых имеются товары данного бренда.<br/><br/> <a href="https://www.advantshop.net/help/pages/brand#4" target="_blank">Инструкция. Как настроить отображение товаров на странице "Бренды".</a>'Where [ResourceKey] = 'Admin.Settings.Template.ShowCategoryTreeInBrandHote' and [LanguageId] = 1
UPDATE [Settings].[Localization] Set [ResourceValue] = 'The setting allows you to display on the brand page the categories in which the goods of this brand.<br/><br/><a href="https://www.advantshop.net/help/pages/brand#4" target="_blank">Instruction. How to customize the display of products on the Brands page.</a>'Where [ResourceKey] = 'Admin.Settings.Template.ShowCategoryTreeInBrandHote' and [LanguageId] = 2
UPDATE [Settings].[Localization] Set [ResourceValue] = 'Указываете текст, который будет отображаться в качестве заголовка в блоке подписки на новости.<br/><br/> <a href="https://www.advantshop.net/help/pages/add-news#3" target="_blank">Инструкция. Настройка раздела новости на сайте.</a>'Where [ResourceKey] = 'Admin.Settings.News.MainPageTextNote' and [LanguageId] = 1
UPDATE [Settings].[Localization] Set [ResourceValue] = 'Specify the text that will be displayed as the title in the newsletter subscription block<br/><br/><a href="https://www.advantshop.net/help/pages/add-news#3" target="_blank">Instruction. Setting up the news section on the site.</a>'Where [ResourceKey] = 'Admin.Settings.News.MainPageTextNote' and [LanguageId] = 2
UPDATE [Settings].[Localization] Set [ResourceValue] = 'В данной настройке указываете число - количество новостей, которые будут выводится в списке всех новостей.<br/><br/> <a href="https://www.advantshop.net/help/pages/add-news" target="_blank">Инструкция. Настройка раздела новости на сайте.</a>'Where [ResourceKey] = 'Admin.Settings.News.NewsPerPageNote' and [LanguageId] = 1
UPDATE [Settings].[Localization] Set [ResourceValue] = 'In this setting, specify the number - the number of news that will be displayed in the list of all news.<br/><br/><a href="https://www.advantshop.net/help/pages/add-news" target="_blank">Instruction. Setting up the news section on the site.</a>'Where [ResourceKey] = 'Admin.Settings.News.NewsPerPageNote' and [LanguageId] = 2
UPDATE [Settings].[Localization] Set [ResourceValue] = 'Впишите число - количество новостей отображаемых на главной странице.<br/><br/> <a href="https://www.advantshop.net/help/pages/add-news" target="_blank">Инструкция. Настройка раздела новости на сайте.</a>'Where [ResourceKey] = 'Admin.Settings.News.NewsMainPageCountNote' and [LanguageId] = 1
UPDATE [Settings].[Localization] Set [ResourceValue] = 'Enter the number - the number of news displayed on the main page.<br/><br/><a href="https://www.advantshop.net/help/pages/add-news" target="_blank">Instruction. Setting up the news section on the site.</a>'Where [ResourceKey] = 'Admin.Settings.News.NewsMainPageCountNote' and [LanguageId] = 2
UPDATE [Settings].[Localization] Set [ResourceValue] = 'Настройка включает отображение виджетов коммуникаций.<br/><br/> <a href="https://www.advantshop.net/help/pages/vidzhety-kommunikatsii" target="_blank">Инструкция. Виджеты коммуникаций.</a>'Where [ResourceKey] = 'Admin.Settings.Social.SocialWidget.ShowCommunicationWidget.HelpText' and [LanguageId] = 1
UPDATE [Settings].[Localization] Set [ResourceValue] = 'Customization enables the display of communication widgets.<br/><br/><a href="https://www.advantshop.net/help/pages/vidzhety-kommunikatsii" target="_blank">Instruction. Communication Widgets.</a>'Where [ResourceKey] = 'Admin.Settings.Social.SocialWidget.ShowCommunicationWidget.HelpText' and [LanguageId] = 2
UPDATE [Settings].[Localization] Set [ResourceValue] = '1) Активируйте виджет коммуникаций - поставьте галочку в поле "Показывать виджет коммуникаций" <br/>2) Активируйте виджеты, которые планируете выводить (для этого поставьте галочки в полях "Показывать ВКонтакте") <br/>3) И подключите виджеты согласно инструкции.<br/><br/> <a href="https://www.advantshop.net/help/pages/vidzhety-kommunikatsii" target="_blank">Инструкция. Виджеты коммуникаций.</a>'Where [ResourceKey] = 'Admin.Settings.Social.SocialWidget.ShowVkontakte.HelpText' and [LanguageId] = 1
UPDATE [Settings].[Localization] Set [ResourceValue] = '1) Activate the communications widget - check the box "Show communications widget"<br/>2) Activate the widgets that you plan to display (to do this, check the "Show VKontakte" boxes)<br/> 3) And connect widgets according to the instructions.<br/><br/><a href="https://www.advantshop.net/help/pages/vidzhety-kommunikatsii" target="_blank">Instruction. Communication Widgets.</a>'Where [ResourceKey] = 'Admin.Settings.Social.SocialWidget.ShowVkontakte.HelpText' and [LanguageId] = 2
UPDATE [Settings].[Localization] Set [ResourceValue] = '1) Активируйте виджет коммуникаций - поставьте галочку в поле "Показывать виджет коммуникаций" <br/>2) Активируйте виджеты, которые планируете выводить (для этого поставьте галочки в полях "Показывать Facebook") <br/>3) И подключите виджеты согласно инструкции.<br/><br/> <a href="https://www.advantshop.net/help/pages/vidzhety-kommunikatsii" target="_blank">Инструкция. Виджеты коммуникаций.</a>'Where [ResourceKey] = 'Admin.Settings.Social.SocialWidget.ShowFaceBook.HelpText' and [LanguageId] = 1
UPDATE [Settings].[Localization] Set [ResourceValue] = '1) Activate the communications widget - check the box "Show communications widget"<br/>2) Activate the widgets that you plan to display (to do this, check the "Show Facebook" boxes)<br/> 3) And connect widgets according to the instructions.<br/><br/><a href="https://www.advantshop.net/help/pages/vidzhety-kommunikatsii" target="_blank">Instruction. Communication Widgets.</a>'Where [ResourceKey] = 'Admin.Settings.Social.SocialWidget.ShowFaceBook.HelpText' and [LanguageId] = 2
UPDATE [Settings].[Localization] Set [ResourceValue] = '1) Активируйте виджет коммуникаций - поставьте галочку в поле "Показывать виджет коммуникаций" <br/>2) Активируйте виджеты, которые планируете выводить (для этого поставьте галочки в полях "Показывать JivoSite") <br/>3) И подключите виджеты согласно инструкции.<br/><br/> <a href="https://www.advantshop.net/help/pages/vidzhety-kommunikatsii" target="_blank">Инструкция. Виджеты коммуникаций.</a>'Where [ResourceKey] = 'Admin.Settings.Social.SocialWidget.ShowJivosite.HelpText' and [LanguageId] = 1
UPDATE [Settings].[Localization] Set [ResourceValue] = '1) Activate the communications widget - check the box "Show communications widget"<br/>2) Activate the widgets that you plan to display (to do this, check the "Show JivoSite" boxes)<br/> 3) And connect widgets according to the instructions.<br/><br/><a href="https://www.advantshop.net/help/pages/vidzhety-kommunikatsii" target="_blank">Instruction. Communication Widgets.</a>'Where [ResourceKey] = 'Admin.Settings.Social.SocialWidget.ShowJivosite.HelpText' and [LanguageId] = 2
UPDATE [Settings].[Localization] Set [ResourceValue] = '1) Активируйте виджет коммуникаций - поставьте галочку в поле "Показывать виджет коммуникаций" <br/>2) Активируйте виджеты, которые планируете выводить (для этого поставьте галочки в полях "Показывать Callback") <br/>3) И подключите виджеты согласно инструкции.<br/><br/> <a href="https://www.advantshop.net/help/pages/vidzhety-kommunikatsii" target="_blank">Инструкция. Виджеты коммуникаций.</a>'Where [ResourceKey] = 'Admin.Settings.Social.SocialWidget.ShowCallback.HelpText' and [LanguageId] = 1
UPDATE [Settings].[Localization] Set [ResourceValue] = '1) Activate the communications widget - check the box "Show communications widget"<br/>2) Activate the widgets that you plan to display (to do this, check the "Show Callback" boxes)<br/> 3) And connect widgets according to the instructions.<br/><br/><a href="https://www.advantshop.net/help/pages/vidzhety-kommunikatsii" target="_blank">Instruction. Communication Widgets.</a>'Where [ResourceKey] = 'Admin.Settings.Social.SocialWidget.ShowCallback.HelpText' and [LanguageId] = 2
UPDATE [Settings].[Localization] Set [ResourceValue] = '1) Активируйте виджет коммуникаций - поставьте галочку в поле "Показывать виджет коммуникаций" <br/>2) Активируйте виджеты, которые планируете выводить (для этого поставьте галочки в полях "Показывать WhatsApp") <br/>3) И подключите виджеты согласно инструкции.<br/><br/> <a href="https://www.advantshop.net/help/pages/vidzhety-kommunikatsii" target="_blank">Инструкция. Виджеты коммуникаций.</a>'Where [ResourceKey] = 'Admin.Settings.Social.SocialWidget.ShowWhatsApp.HelpText' and [LanguageId] = 1
UPDATE [Settings].[Localization] Set [ResourceValue] = '1) Activate the communications widget - check the box "Show communications widget"<br/>2) Activate the widgets that you plan to display (to do this, check the "Show WhatsApp" boxes)<br/> 3) And connect widgets according to the instructions.<br/><br/><a href="https://www.advantshop.net/help/pages/vidzhety-kommunikatsii" target="_blank">Instruction. Communication Widgets.</a>'Where [ResourceKey] = 'Admin.Settings.Social.SocialWidget.ShowWhatsApp.HelpText' and [LanguageId] = 2
UPDATE [Settings].[Localization] Set [ResourceValue] = '1) Активируйте виджет коммуникаций - поставьте галочку в поле "Показывать виджет коммуникаций" <br/>2) Активируйте виджеты, которые планируете выводить (для этого поставьте галочки в полях "Показывать Viber") <br/>3) И подключите виджеты согласно инструкции.<br/><br/> <a href="https://www.advantshop.net/help/pages/vidzhety-kommunikatsii" target="_blank">Инструкция. Виджеты коммуникаций.</a>'Where [ResourceKey] = 'Admin.Settings.Social.SocialWidget.ShowViber.HelpText' and [LanguageId] = 1
UPDATE [Settings].[Localization] Set [ResourceValue] = '1) Activate the communications widget - check the box "Show communications widget"<br/>2) Activate the widgets that you plan to display (to do this, check the "Show Viber" boxes)<br/> 3) And connect widgets according to the instructions.<br/><br/><a href="https://www.advantshop.net/help/pages/vidzhety-kommunikatsii" target="_blank">Instruction. Communication Widgets.</a>'Where [ResourceKey] = 'Admin.Settings.Social.SocialWidget.ShowViber.HelpText' and [LanguageId] = 2
UPDATE [Settings].[Localization] Set [ResourceValue] = '1) Активируйте виджет коммуникаций - поставьте галочку в поле "Показывать виджет коммуникаций" <br/>2) Активируйте виджеты, которые планируете выводить (для этого поставьте галочки в полях "Показывать Одноклассники") <br/>3) И подключите виджеты согласно инструкции.<br/><br/> <a href="https://www.advantshop.net/help/pages/vidzhety-kommunikatsii" target="_blank">Инструкция. Виджеты коммуникаций.</a>'Where [ResourceKey] = 'Admin.Settings.Social.SocialWidget.ShowOdnoklassniki.HelpText' and [LanguageId] = 1
UPDATE [Settings].[Localization] Set [ResourceValue] = '1) Activate the communications widget - check the box "Show communications widget"<br/>2) Activate the widgets that you plan to display (to do this, check the "Show Odnoklassniki" boxes)<br/> 3) And connect widgets according to the instructions.<br/><br/><a href="https://www.advantshop.net/help/pages/vidzhety-kommunikatsii" target="_blank">Instruction. Communication Widgets.</a>'Where [ResourceKey] = 'Admin.Settings.Social.SocialWidget.ShowOdnoklassniki.HelpText' and [LanguageId] = 2
UPDATE [Settings].[Localization] Set [ResourceValue] = '1) Активируйте виджет коммуникаций - поставьте галочку в поле "Показывать виджет коммуникаций" <br/>2) Активируйте виджеты, которые планируете выводить (для этого поставьте галочки в полях "Показывать Telegram") <br/>3) И подключите виджеты согласно инструкции.<br/><br/> <a href="https://www.advantshop.net/help/pages/vidzhety-kommunikatsii" target="_blank">Инструкция. Виджеты коммуникаций.</a>'Where [ResourceKey] = 'Admin.Settings.Social.SocialWidget.ShowTelegram.HelpText' and [LanguageId] = 1
UPDATE [Settings].[Localization] Set [ResourceValue] = '1) Activate the communications widget - check the box "Show communications widget"<br/>2) Activate the widgets that you plan to display (to do this, check the "Show Telegram" boxes)<br/> 3) And connect widgets according to the instructions.<br/><br/><a href="https://www.advantshop.net/help/pages/vidzhety-kommunikatsii" target="_blank">Instruction. Communication Widgets.</a>'Where [ResourceKey] = 'Admin.Settings.Social.SocialWidget.ShowTelegram.HelpText' and [LanguageId] = 2
UPDATE [Settings].[Localization] Set [ResourceValue] = 'Можно подключить дополнительно каналы связи, которых нет в списке.<br/><br/> <a href="https://www.advantshop.net/help/pages/vidzhety-kommunikatsii#a9" target="_blank">Инструкция. Свой виджет.</a>'Where [ResourceKey] = 'Admin.Settings.Social.SocialWidget.CustomWidgetTitleNote' and [LanguageId] = 1
UPDATE [Settings].[Localization] Set [ResourceValue] = 'You can connect additional communication channels that are not listed.<br/><br/><a href="https://www.advantshop.net/help/pages/vidzhety-kommunikatsii#a9" target="_blank">Instruction. Custom widget.</a>'Where [ResourceKey] = 'Admin.Settings.Social.SocialWidget.CustomWidgetTitleNote' and [LanguageId] = 2
UPDATE [Settings].[Localization] Set [ResourceValue] = 'Структура сайта может быть представлена в двухколоночном и одноколоночном режиме.<br/><br/> <a href="https://www.advantshop.net/help/pages/template-settings#11" target="_blank">Инструкция. Режим отображения главной страницы.</a>'Where [ResourceKey] = 'Admin.Settings.Template.MainPageModeHint' and [LanguageId] = 1
UPDATE [Settings].[Localization] Set [ResourceValue] = 'The site structure can be presented in two-column and single-column mode.<br/><br/><a href="https://www.advantshop.net/help/pages/template-settings#11" target="_blank">Instruction. Homepage display mode.</a>'Where [ResourceKey] = 'Admin.Settings.Template.MainPageModeHint' and [LanguageId] = 2
UPDATE [Settings].[Localization] Set [ResourceValue] = 'Данная настройка отвечает за то, в какой области на сайте будет отображаться окно поиска.<br/><br/> <a href="https://www.advantshop.net/help/pages/template-settings#13" target="_blank">Инструкция. Расположение поиска.</a>'Where [ResourceKey] = 'Admin.Settings.Template.SearchBlockLocationHint' and [LanguageId] = 1
UPDATE [Settings].[Localization] Set [ResourceValue] = 'This setting is responsible for the area in which the search box will be displayed on the site.<br/><br/><a href="https://www.advantshop.net/help/pages/template-settings#13" target="_blank">Instruction. Search Location.</a>'Where [ResourceKey] = 'Admin.Settings.Template.SearchBlockLocationHint' and [LanguageId] = 2

GO--


INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Loging.EmailStatus.PrepareSend.Description', 'Подготовка отправки сообщения')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Loging.EmailStatus.PrepareSend.Description', 'Preparing to send a message')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Loging.EmailStatus.Sending.Description', 'Отправление сообщения')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Loging.EmailStatus.Sending.Description', 'Message sending')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Loging.EmailStatus.Sent.Description', 'Сообщение отправлено, но пока не доставлено')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Loging.EmailStatus.Sent.Description', 'Message sent but not delivered yet')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Loging.EmailStatus.Delivered.Description', 'Сообщение доставлено')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Loging.EmailStatus.Delivered.Description', 'Message delivered')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Loging.EmailStatus.Opened.Description', 'Сообщение доставлено и зарегистрировано его прочтение')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Loging.EmailStatus.Opened.Description', 'Message delivered and recorded read')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Loging.EmailStatus.Clicked.Description', 'Сообщение доставлено, прочитано, был зарегистрирован переход по одной из ссылок в письме')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Loging.EmailStatus.Clicked.Description', 'The message is delivered, read, a click was registered on one of the links in the letter')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Loging.EmailStatus.Unsubscribed.Description', 'Сообщение было доставлено получателю и прочитано им, но пользователь отписался по ссылке в письме')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Loging.EmailStatus.Unsubscribed.Description', 'The message was delivered to the recipient and read by him, but the user unsubscribed by the link in the letter')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Loging.EmailStatus.SoftBounced.Description', 'Не удалось доставить сообщение по одной из следующих причин:<br/> 
попытки отправки не производились, или отправка отложена;<br/> 
ящик адресата переполнен;<br/> 
письмо доставлено, но попало в папку “Спам”;<br/> 
было не менее одной попытки отправки, попытки продолжаются;<br/> 
домен получателя не принимает почту из-за неправильных настроек;<br/> 
отправка была отменена;<br/> 
отправка отменена из-за некорректных данных отправителя;<br/> 
не хватает средств на счету;<br/> 
адрес временно недоступен;')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Loging.EmailStatus.SoftBounced.Description', 'The message could not be delivered for one of the following reasons: <br/>
no attempt was made to send, or sending was delayed; <br/>
destination mailbox is full; <br/>
the message has been delivered, but it’s in the Spam folder; <br/>
there were at least one attempt to send, attempts continue; <br/>
recipient domain does not accept mail due to incorrect settings; <br/>
sending has been canceled; <br/>
sending canceled due to incorrect sender data; <br/>
not enough funds in the account; <br/>
the address is temporarily unavailable;')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Loging.EmailStatus.HardBounced.Description', 'Не удалось доставить сообщение по одной из следующих причин:<br/> 
почтовый ящик адресата не существует, либо удален;<br/> 
email-адрес не используется;<br/> 
письмо было отвергнуто сервером получателя как спам;<br/> 
домен получателя не принимает никакую почту вообще по любой причине;<br/> 
некорректный адрес получателя;<br/> 
адресат отписался от Ваших рассылок;<br/> 
email-адрес недоступен;<br/> 
письмо превышает допустимый размер;<br/>
истек срок повторных отправок сообщения;')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Loging.EmailStatus.HardBounced.Description', 'The message could not be delivered for one of the following reasons: <br/>
Destination mailbox does not exist or is deleted; <br/>
Email address is not used; <br/>
the message was rejected by the recipient server as spam; <br/>
the recipient’s domain does not accept any mail at all for any reason; <br/>
Invalid recipient address; <br/>
the recipient has unsubscribed from your mailings; <br/>
email address unavailable; <br/>
the letter exceeds the permissible size; <br/>
Message resubmission has expired')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Loging.EmailStatus.Spam.Description', 'Сообщение доставлено, но попало в папку “спам” адресата')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Loging.EmailStatus.Spam.Description', 'The message was delivered, but got into the spam folder of the recipient')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Loging.EmailStatus.Error.Description', 'Не отправлено из-за ошибки')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Loging.EmailStatus.Error.Description', 'Not sent due to error')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Telephony.LogNotifications','Сбор отладочной информации о вызовах')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Telephony.LogNotifications','Log calls debug information')

GO--

UPDATE [Settings].[SettingsSearch] SET [Link] = 'settingssocial#?socialTab=3' WHERE Title = 'Виджет коммуникаций'

GO--

IF NOT EXISTS (SELECT * FROM [Settings].[Localization] WHERE ResourceKey = 'Admin.Js.EditTask.Saved')
BEGIN
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.EditTask.Saved', 'сохранена')
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.EditTask.Saved', 'saved')
END

IF NOT EXISTS (SELECT * FROM [Settings].[Localization] WHERE ResourceKey = 'Admin.Js.EditTask.Completed')
BEGIN
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.EditTask.Completed', 'завершена')
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.EditTask.Completed', 'completed')
END

GO--

UPDATE Settings.Localization SET ResourceValue = 'Ресселеры' where ResourceKey = 'Core.Customers.RoleActionCategory.Reseller' AND LanguageId = 1
UPDATE Settings.Localization SET ResourceValue = 'Resellers' where ResourceKey = 'Core.Customers.RoleActionCategory.Reseller' AND LanguageId = 2
UPDATE Settings.Localization SET ResourceValue = 'Партнерская программа' where ResourceKey = 'Core.Customers.RoleActionCategory.Partners' AND LanguageId = 1
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Home.Menu.Partners', 'Partners')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Customers.RoleActionCategory.Partners', 'Affiliate program')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.Orders.DeliveryDate', 'Дата доставки')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.Orders.DeliveryDate', 'Delivery date')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.EditTask.Accepted', 'принята')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.EditTask.Accepted', 'accepted')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.EditTask.Opened', 'возобновлена')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.EditTask.Opened', 'opened')


UPDATE [Settings].[InternalSettings] SET [settingValue] = '8.5.5' WHERE [settingKey] = 'db_version'