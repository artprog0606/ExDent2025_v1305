INSERT INTO [Settings].[Localization] (ResourceKey, ResourceValue, LanguageId) VALUES
	('Core.ExportImport.MultiOrder.ArtNo', 'Артикул', 1),
	('Core.ExportImport.MultiOrder.ArtNo', 'ArtNo', 2),
	('Core.ExportImport.MultiOrder.Name', 'Название', 1),
	('Core.ExportImport.MultiOrder.Name', 'Name', 2),
	('Core.ExportImport.MultiOrder.Price', 'Цена', 1),
	('Core.ExportImport.MultiOrder.Price', 'Price', 2),
	('Core.ExportImport.MultiOrder.Amount', 'Количество', 1),
	('Core.ExportImport.MultiOrder.Amount', 'Amount', 2),
	('Admin.Js.ExportOrders.OrderItemsInString', 'Склеивать позиции заказа в одну строку', 1),
	('Admin.Js.ExportOrders.OrderItemsInString', 'Merge the order items into one line', 2)

GO--

UPDATE [Settings].[Localization] 
SET ResourceValue = 'Без названия'
WHERE ResourceKey like 'Admin.Js.ProductVideos.NameNotSpecified' AND LanguageId = 1

GO--

TRUNCATE TABLE Settings.Error404

GO--

ALTER TABLE Settings.Error404
    ADD Hash INT NOT NULL 

GO--

CREATE UNIQUE index Error404_Hash_uindex
    ON Settings.Error404 (Hash)

GO--

INSERT INTO [Settings].[Localization] (ResourceKey, ResourceValue, LanguageId) VALUES
	('Admin.Js.ProductSelect.Color', 'Цвет', 1),
	('Admin.Js.ProductSelect.Color', 'Color', 2),
	('Admin.Js.ProductSelect.Size', 'Размер', 1),
	('Admin.Js.ProductSelect.Size', 'Size', 2),
	('Admin.Js.ProductSelect.Property', 'Свойство', 1),
	('Admin.Js.ProductSelect.Property', 'Property', 2),
	('Admin.Js.ProductSelect.PropertyValue', 'Значение свойства', 1),
	('Admin.Js.ProductSelect.PropertyValue', 'Property value', 2)

GO--

Update oc 
Set oc.CustomerID = o.LinkedCustomerId
From [Order].[OrderCustomer] as oc
Left Join [Order].[Order] as o on o.OrderID = oc.OrderId 
Where o.LinkedCustomerId is not null and oc.CustomerID <> o.LinkedCustomerId

GO--

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Индекс для поиска необходим, чтобы собрать информацию (добавление нового товара, изменение товаров, которые были добавлены ранее; чтобы обеспечить быстрый и точный поиск информации).<br/><br/>  Подробнее: <br/><a href ="https://www.advantshop.net/help/pages/search#5" target="_blank">Поиск на сайте.</a>' WHERE [ResourceKey] = 'Admin.Settings.Feedback.FeedbackActionHint' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'The index for the search is necessary to collect information (adding a new product, changing products that were added earlier; to ensure a quick and accurate search for information).<br/><br/> Read more: <br/><a href ="https://www.advantshop.net/help/pages/search#5 " target="_blank">Search on the site.</a>' WHERE [ResourceKey] = 'Admin.Settings.Feedback.FeedbackActionHint' AND [LanguageId] = 2

GO--

INSERT INTO [Settings].[Localization] (ResourceKey, ResourceValue, LanguageId) VALUES
	('Admin.Js.OrderAnalysis.DeliveryDateFrom', 'Дата доставки от', 1),
	('Admin.Js.OrderAnalysis.DeliveryDateFrom', 'Delivery date from', 2),
	('Admin.Js.OrderAnalysis.DeliveryDateTo', 'Дата доставки до', 1),
	('Admin.Js.OrderAnalysis.DeliveryDateTo', 'Delivery date to', 2)

GO--

INSERT INTO [Settings].[Localization] (ResourceKey, ResourceValue, LanguageId) VALUES
	('Admin.Js.Tags.AddEdit.TagName', 'Название тега', 1),
	('Admin.Js.Tags.AddEdit.TagName', 'Tag name', 2);

INSERT INTO [Settings].[Localization] (ResourceKey, ResourceValue, LanguageId) VALUES
	('Admin.Js.Tags.AddEdit.Activity', 'Активность', 1),
	('Admin.Js.Tags.AddEdit.Activity', 'Activity', 2);

INSERT INTO [Settings].[Localization] (ResourceKey, ResourceValue, LanguageId) VALUES
	('Admin.Js.Tags.AddEdit.VisibilityForUsers', 'Видимость для пользователей', 1),
	('Admin.Js.Tags.AddEdit.VisibilityForUsers', 'Visibility for users', 2);

INSERT INTO [Settings].[Localization] (ResourceKey, ResourceValue, LanguageId) VALUES
	('Admin.Js.Tags.AddEdit.Sorting', 'Сортировка', 1),
	('Admin.Js.Tags.AddEdit.Sorting', 'Sorting', 2);

INSERT INTO [Settings].[Localization] (ResourceKey, ResourceValue, LanguageId) VALUES
	('Admin.Js.Tags.AddEdit.Description', 'Описание', 1),
	('Admin.Js.Tags.AddEdit.Description', 'Description', 2);

INSERT INTO [Settings].[Localization] (ResourceKey, ResourceValue, LanguageId) VALUES
	('Admin.Js.Tags.AddEdit.ShortDescription', 'Краткое описание', 1),
	('Admin.Js.Tags.AddEdit.ShortDescription', 'Short description', 2);

INSERT INTO [Settings].[Localization] (ResourceKey, ResourceValue, LanguageId) VALUES
	('Admin.Js.Tags.AddEdit.Synonym', 'Синоним для URL запроса', 1),
	('Admin.Js.Tags.AddEdit.Synonym', 'Synonym for request URL', 2);

INSERT INTO [Settings].[Localization] (ResourceKey, ResourceValue, LanguageId) VALUES
	('Admin.Js.Tags.AddEdit.UseDefaultMeta', 'Использовать Meta по умолчанию', 1),
	('Admin.Js.Tags.AddEdit.UseDefaultMeta', 'Use default Meta', 2);

INSERT INTO [Settings].[Localization] (ResourceKey, ResourceValue, LanguageId) VALUES
	('Admin.Js.Category.Index.SeoTitle', 'Title страницы', 1),
	('Admin.Js.Category.Index.SeoTitle', 'Page title', 2);	

INSERT INTO [Settings].[Localization] (ResourceKey, ResourceValue, LanguageId) VALUES
	('Admin.Js.Category.Index.SeoH1', 'Заголовок H1', 1),
	('Admin.Js.Category.Index.SeoH1', 'H1 header', 2);

INSERT INTO [Settings].[Localization] (ResourceKey, ResourceValue, LanguageId) VALUES
	('Admin.Js.Category.Index.SeoKeywords', 'Ключевые слова', 1),
	('Admin.Js.Category.Index.SeoKeywords', 'Keywords', 2);	

INSERT INTO [Settings].[Localization] (ResourceKey, ResourceValue, LanguageId) VALUES
	('Admin.Js.Category.Index.SeoDescription', 'Мета описание', 1),
	('Admin.Js.Category.Index.SeoDescription', 'Meta Description', 2);	

INSERT INTO [Settings].[Localization] (ResourceKey, ResourceValue, LanguageId) VALUES
	('Admin.Js.Tags.AddEdit.Tag', 'Тег', 1),
	('Admin.Js.Tags.AddEdit.Tag', 'Tag', 2);	

INSERT INTO [Settings].[Localization] (ResourceKey, ResourceValue, LanguageId) VALUES
	('Admin.Js.Tags.AddEdit.NewTag', 'Новый тег', 1),
	('Admin.Js.Tags.AddEdit.NewTag', 'New tag', 2);

INSERT INTO [Settings].[Localization] (ResourceKey, ResourceValue, LanguageId) VALUES
	('Admin.Js.AddTag.TagAddedSuccessfully', 'Тег успешно добавлен', 1),
	('Admin.Js.AddTag.TagAddedSuccessfully', 'Tag added successfully', 2);

INSERT INTO [Settings].[Localization] (ResourceKey, ResourceValue, LanguageId) VALUES
	('Admin.Js.AddTag.Error', 'Не удалось добавить тег', 1),
	('Admin.Js.AddTag.Error', 'Failed to add tag', 2);

INSERT INTO [Settings].[Localization] (ResourceKey, ResourceValue, LanguageId) VALUES
	('Admin.Js.EditTag.Error', 'Не удалось сохранить изменения', 1),
	('Admin.Js.EditTag.Error', 'Failed to save changes', 2);

GO--

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Нет доступных методов доставки, пожалуйста, свяжитесь с нами' WHERE [ResourceKey] = 'Js.Shipping.NoShippingMethods' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'No shipping methods available, please contact us' WHERE [ResourceKey] = 'Js.Shipping.NoShippingMethods' AND [LanguageId] = 2

GO--

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Загрузка способов доставки...' WHERE [ResourceKey] = 'Js.Shipping.ShippingMethodsLoading' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'Loading shipping methods...' WHERE [ResourceKey] = 'Js.Shipping.ShippingMethodsLoading' AND [LanguageId] = 2

GO--

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Нет доступных методов оплаты, пожалуйста, свяжитесь с нами' WHERE [ResourceKey] = 'Js.Payment.NoPaymentMethods' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'No payment methods available, please contact us' WHERE [ResourceKey] = 'Js.Payment.NoPaymentMethods' AND [LanguageId] = 2

GO--

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Загрузка способов оплаты...' WHERE [ResourceKey] = 'Js.Payment.PaymentMethodsLoading' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'Loading payment methods...' WHERE [ResourceKey] = 'Js.Payment.PaymentMethodsLoading' AND [LanguageId] = 2

GO--

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Индекс для поиска необходим, чтобы собрать информацию (добавление нового товара, изменение товаров, которые были добавлены ранее), чтобы обеспечить быстрый и точный поиск информации.<br/><br/>  Подробнее: <br/><a href ="https://www.advantshop.net/help/pages/search#5" target="_blank">Поиск на сайте.</a>' WHERE [ResourceKey] = 'Admin.Settings.Feedback.FeedbackActionHint' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'The index for the search is necessary to collect information (adding a new product, changing products that were added earlier) to ensure a quick and accurate search for information.<br/><br/> Read more: <br/><a href ="https://www.advantshop.net/help/pages/search#5 " target="_blank">Search on the site.</a>' WHERE [ResourceKey] = 'Admin.Settings.Feedback.FeedbackActionHint' AND [LanguageId] = 2

GO--

INSERT INTO [Settings].[Localization] (ResourceKey, ResourceValue, LanguageId) VALUES
	('Admin.Js.TestShippingCalculate.District', 'Район региона', 1),
	('Admin.Js.TestShippingCalculate.District', 'District', 2),
	('Admin.Js.TestShippingCalculate.Zip', 'Индекс', 1),
	('Admin.Js.TestShippingCalculate.Zip', 'Zip', 2);

GO--

INSERT INTO [Settings].[Localization] (ResourceKey, ResourceValue, LanguageId) VALUES
	('Admin.Settings.Checkout.AddToCartPreOrder', 'Разрешить добавлять в корзину только товары под заказ', 1),
	('Admin.Settings.Checkout.AddToCartPreOrder', 'Allow adding to the cart only products under the order', 2)

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Разрешить добавлять в корзину все товары' WHERE [ResourceKey] = 'Admin.Settings.Checkout.AddToCart' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'Allow adding all products to the cart' WHERE [ResourceKey] = 'Admin.Settings.Checkout.AddToCart' AND [LanguageId] = 2

GO--

Update [Settings].[MailFormat] 
Set FormatText = Replace( Replace(FormatText, '#ORDERID#', '#ORDER_NUMBER#') , '#ORDER_ID#', '#ORDER_NUMBER#')
Where (FormatText like '%#ORDER_ID#%' or FormatText like '%#ORDERID#%') and 
	 MailFormatTypeId in (SELECT [MailFormatTypeID]
							FROM [Settings].[MailFormatType]
							Where MailType = 'OnNewOrder' or 
								MailType = 'OnChangeOrderStatus' or  
								MailType = 'OnBuyInOneClick' or  
								MailType = 'OnPreOrder' or  
								MailType = 'OnBillingLink' or  
								MailType = 'OnPayOrder') 

GO--

Update [Settings].[MailFormat] 
Set FormatSubject = Replace( Replace(FormatSubject, '#ORDERID#', '#ORDER_NUMBER#') , '#ORDER_ID#', '#ORDER_NUMBER#')
Where (FormatSubject like '%#ORDER_ID#%' or FormatSubject like '%#ORDERID#%') and 
	 MailFormatTypeId in (SELECT [MailFormatTypeID]
							FROM [Settings].[MailFormatType]
							Where MailType = 'OnNewOrder' or 
								MailType = 'OnChangeOrderStatus' or  
								MailType = 'OnBuyInOneClick' or  
								MailType = 'OnPreOrder' or  
								MailType = 'OnBillingLink' or  
								MailType = 'OnPayOrder') 

GO--

Update [Settings].[MailFormat] 
Set FormatSubject = Replace(FormatSubject, '#NUMBER#', '#ORDER_NUMBER#'), 
	FormatText = Replace(FormatText, '#NUMBER#', '#ORDER_NUMBER#')
Where (FormatSubject like '%#NUMBER#%' or FormatText like '%#NUMBER#%') and 
	 MailFormatTypeId in (SELECT [MailFormatTypeID]
							FROM [Settings].[MailFormatType]
							Where MailType = 'OnNewOrder' or 
								MailType = 'OnChangeOrderStatus' or  
								MailType = 'OnBuyInOneClick' or  
								MailType = 'OnPreOrder' or  
								MailType = 'OnBillingLink' or  
								MailType = 'OnPayOrder') 

GO--

Update [Settings].[MailFormat] 
Set FormatSubject = Replace(FormatSubject, '#ORDERSTATUS#', '#ORDER_STATUS#'), 
	FormatText = Replace(FormatText, '#ORDERSTATUS#', '#ORDER_STATUS#')
Where (FormatSubject like '%#ORDERSTATUS#%' or FormatText like '%#ORDERSTATUS#%') and 
	 MailFormatTypeId in (SELECT [MailFormatTypeID]
							FROM [Settings].[MailFormatType]
							Where MailType = 'OnNewOrder' or 
								MailType = 'OnChangeOrderStatus' or  
								MailType = 'OnBuyInOneClick' or  
								MailType = 'OnPreOrder' or  
								MailType = 'OnBillingLink' or  
								MailType = 'OnPayOrder') 

GO--

Update [Settings].[MailFormat] 
Set FormatSubject = Replace(FormatSubject, '#TRACKNUMBER#', '#ORDER_TRACK_NUMBER#'), 
	FormatText = Replace(FormatText, '#TRACKNUMBER#', '#ORDER_TRACK_NUMBER#')
Where (FormatSubject like '%#TRACKNUMBER#%' or FormatText like '%#TRACKNUMBER#%') and 
	 MailFormatTypeId in (SELECT [MailFormatTypeID]
							FROM [Settings].[MailFormatType]
							Where MailType = 'OnNewOrder' or 
								MailType = 'OnChangeOrderStatus' or  
								MailType = 'OnBuyInOneClick' or  
								MailType = 'OnPreOrder' or  
								MailType = 'OnBillingLink' or  
								MailType = 'OnPayOrder') 

GO--

Update [Settings].[MailFormat] 
Set FormatSubject = Replace(FormatSubject, '#SHIPPINGMETHOD#', '#SHIPPING_NAME_FULL#'), 
	FormatText = Replace(FormatText, '#SHIPPINGMETHOD#', '#SHIPPING_NAME_FULL#')
Where (FormatSubject like '%#SHIPPINGMETHOD#%' or FormatText like '%#SHIPPINGMETHOD#%') and 
	 MailFormatTypeId in (SELECT [MailFormatTypeID]
							FROM [Settings].[MailFormatType]
							Where MailType = 'OnNewOrder' or 
								MailType = 'OnChangeOrderStatus' or  
								MailType = 'OnBuyInOneClick' or  
								MailType = 'OnPreOrder' or  
								MailType = 'OnBillingLink' or  
								MailType = 'OnPayOrder') 

GO--

Update [Settings].[MailFormat] 
Set FormatSubject = Replace(FormatSubject, '#PAYMENTTYPE#', '#PAYMENT_NAME#'), 
	FormatText = Replace(FormatText, '#PAYMENTTYPE#', '#PAYMENT_NAME#')
Where (FormatSubject like '%#PAYMENTTYPE#%' or FormatText like '%#PAYMENTTYPE#%') and 
	 MailFormatTypeId in (SELECT [MailFormatTypeID]
							FROM [Settings].[MailFormatType]
							Where MailType = 'OnNewOrder' or 
								MailType = 'OnChangeOrderStatus' or  
								MailType = 'OnBuyInOneClick' or  
								MailType = 'OnPreOrder' or  
								MailType = 'OnBillingLink' or  
								MailType = 'OnPayOrder') 

GO--

Update [Settings].[MailFormat] 
Set FormatSubject = Replace(FormatSubject, '#BILLING_SHORTLINK#', '#BILLING_SHORT_LINK#'), 
	FormatText = Replace(FormatText, '#BILLING_SHORTLINK#', '#BILLING_SHORT_LINK#')
Where (FormatSubject like '%#BILLING_SHORTLINK#%' or FormatText like '%#BILLING_SHORTLINK#%') and 
	 MailFormatTypeId in (SELECT [MailFormatTypeID]
							FROM [Settings].[MailFormatType]
							Where MailType = 'OnNewOrder' or 
								MailType = 'OnChangeOrderStatus' or  
								MailType = 'OnBuyInOneClick' or  
								MailType = 'OnPreOrder' or  
								MailType = 'OnBillingLink' or  
								MailType = 'OnPayOrder') 

GO--

Update [Settings].[MailFormat] 
Set FormatSubject = Replace(FormatSubject, '#ORDERTABLE#', '#ORDER_ITEMS_HTML#'), 
	FormatText = Replace(FormatText, '#ORDERTABLE#', '#ORDER_ITEMS_HTML#')
Where (FormatSubject like '%#ORDERTABLE#%' or FormatText like '%#ORDERTABLE#%') and 
	 MailFormatTypeId in (SELECT [MailFormatTypeID]
							FROM [Settings].[MailFormatType]
							Where MailType = 'OnNewOrder' or 
								MailType = 'OnChangeOrderStatus' or  
								MailType = 'OnBuyInOneClick' or  
								MailType = 'OnPreOrder' or  
								MailType = 'OnBillingLink' or  
								MailType = 'OnPayOrder') 

GO--

Update [Settings].[MailFormat] 
Set FormatSubject = Replace(FormatSubject, '#CURRENTCURRENCYCODE#', '#ORDER_CURRENCY_CODE#'), 
	FormatText = Replace(FormatText, '#CURRENTCURRENCYCODE#', '#ORDER_CURRENCY_CODE#')
Where (FormatSubject like '%#CURRENTCURRENCYCODE#%' or FormatText like '%#CURRENTCURRENCYCODE#%') and 
	 MailFormatTypeId in (SELECT [MailFormatTypeID]
							FROM [Settings].[MailFormatType]
							Where MailType = 'OnNewOrder' or 
								MailType = 'OnChangeOrderStatus' or  
								MailType = 'OnBuyInOneClick' or  
								MailType = 'OnPreOrder' or  
								MailType = 'OnBillingLink' or  
								MailType = 'OnPayOrder') 

GO--

Update [Settings].[MailFormat] 
Set FormatSubject = Replace(FormatSubject, '#ADDITIONALCUSTOMERFIELDS#', '#ADDITIONAL_CUSTOMER_FIELDS#'), 
	FormatText = Replace(FormatText, '#ADDITIONALCUSTOMERFIELDS#', '#ADDITIONAL_CUSTOMER_FIELDS#')
Where (FormatSubject like '%#ADDITIONALCUSTOMERFIELDS#%' or FormatText like '%#ADDITIONALCUSTOMERFIELDS#%') and 
	 MailFormatTypeId in (SELECT [MailFormatTypeID]
							FROM [Settings].[MailFormatType]
							Where MailType = 'OnNewOrder' or 
								MailType = 'OnChangeOrderStatus' or  
								MailType = 'OnBuyInOneClick' or  
								MailType = 'OnPreOrder' or  
								MailType = 'OnBillingLink' or  
								MailType = 'OnPayOrder') 

GO--

Update [Settings].[MailFormat] 
Set FormatSubject = Replace(FormatSubject, '#CUSTOMERCONTACTS#', '#CUSTOMER_CONTACTS_HTML#'), 
	FormatText = Replace(FormatText, '#CUSTOMERCONTACTS#', '#CUSTOMER_CONTACTS_HTML#')
Where (FormatSubject like '%#CUSTOMERCONTACTS#%' or FormatText like '%#CUSTOMERCONTACTS#%') and 
	 MailFormatTypeId in (SELECT [MailFormatTypeID]
							FROM [Settings].[MailFormatType]
							Where MailType = 'OnNewOrder' or 
								MailType = 'OnChangeOrderStatus' or  
								MailType = 'OnBuyInOneClick' or  
								MailType = 'OnPreOrder' or  
								MailType = 'OnBillingLink' or  
								MailType = 'OnPayOrder') 

GO--

Update [Settings].[MailFormat] 
Set FormatSubject = Replace(FormatSubject, '#STATUSCOMMENT#', '#ORDER_STATUS_COMMENT_HTML#'), 
	FormatText = Replace(FormatText, '#STATUSCOMMENT#', '#ORDER_STATUS_COMMENT_HTML#')
Where (FormatSubject like '%#STATUSCOMMENT#%' or FormatText like '%#STATUSCOMMENT#%') and 
	 MailFormatTypeId in (SELECT [MailFormatTypeID]
							FROM [Settings].[MailFormatType]
							Where MailType = 'OnNewOrder' or 
								MailType = 'OnChangeOrderStatus' or  
								MailType = 'OnBuyInOneClick' or  
								MailType = 'OnPreOrder' or  
								MailType = 'OnBillingLink' or  
								MailType = 'OnPayOrder') 

GO--

Update [Settings].[MailFormat] 
Set FormatSubject = Replace(FormatSubject, '#COMMENTS#', '#ORDER_CUSTOMER_COMMENT#'), 
	FormatText = Replace(FormatText, '#COMMENTS#', '#ORDER_CUSTOMER_COMMENT#')
Where (FormatSubject like '%#COMMENTS#%' or FormatText like '%#COMMENTS#%') and 
	 MailFormatTypeId in (SELECT [MailFormatTypeID]
							FROM [Settings].[MailFormatType]
							Where MailType = 'OnNewOrder' or 
								MailType = 'OnChangeOrderStatus' or  
								MailType = 'OnBuyInOneClick' or  
								MailType = 'OnPreOrder' or  
								MailType = 'OnBillingLink' or  
								MailType = 'OnPayOrder') 

GO--

Update [Settings].[MailFormat] 
Set FormatSubject = Replace(FormatSubject, '#FIRSTNAME#', '#FIRST_NAME#'), 
	FormatText = Replace(FormatText, '#FIRSTNAME#', '#FIRST_NAME#')
Where (FormatSubject like '%#FIRSTNAME#%' or FormatText like '%#FIRSTNAME#%') and 
	 MailFormatTypeId in (SELECT [MailFormatTypeID]
							FROM [Settings].[MailFormatType]
							Where MailType = 'OnNewOrder' or 
								MailType = 'OnChangeOrderStatus' or  
								MailType = 'OnBuyInOneClick' or  
								MailType = 'OnPreOrder' or  
								MailType = 'OnBillingLink' or  
								MailType = 'OnPayOrder') 

GO--

Update [Settings].[MailFormat] 
Set FormatSubject = Replace(FormatSubject, '#LASTNAME#', '#LAST_NAME#'), 
	FormatText = Replace(FormatText, '#LASTNAME#', '#LAST_NAME#')
Where (FormatSubject like '%#LASTNAME#%' or FormatText like '%#LASTNAME#%') and 
	 MailFormatTypeId in (SELECT [MailFormatTypeID]
							FROM [Settings].[MailFormatType]
							Where MailType = 'OnNewOrder' or 
								MailType = 'OnChangeOrderStatus' or  
								MailType = 'OnBuyInOneClick' or  
								MailType = 'OnPreOrder' or  
								MailType = 'OnBillingLink' or  
								MailType = 'OnPayOrder') 

GO--

Update [Settings].[MailFormat] 
Set FormatSubject = Replace(FormatSubject, '#COMPANYNAME#', '#COMPANY_NAME#'), 
	FormatText = Replace(FormatText, '#COMPANYNAME#', '#COMPANY_NAME#')
Where (FormatSubject like '%#COMPANYNAME#%' or FormatText like '%#COMPANYNAME#%') and 
	 MailFormatTypeId in (SELECT [MailFormatTypeID]
							FROM [Settings].[MailFormatType]
							Where MailType = 'OnNewOrder' or 
								MailType = 'OnChangeOrderStatus' or  
								MailType = 'OnBuyInOneClick' or  
								MailType = 'OnPreOrder' or  
								MailType = 'OnBillingLink' or  
								MailType = 'OnPayOrder') 

GO--

Update [Settings].[MailFormat] 
Set FormatSubject = Replace(FormatSubject, '#TOTALPRICE#', '#SUM#'), 
	FormatText = Replace(FormatText, '#TOTALPRICE#', '#SUM#')
Where (FormatSubject like '%#TOTALPRICE#%' or FormatText like '%#TOTALPRICE#%') and 
	 MailFormatTypeId in (SELECT [MailFormatTypeID]
							FROM [Settings].[MailFormatType]
							Where MailType = 'OnNewOrder' or 
								MailType = 'OnChangeOrderStatus' or  
								MailType = 'OnBuyInOneClick' or  
								MailType = 'OnPreOrder' or  
								MailType = 'OnBillingLink' or  
								MailType = 'OnPayOrder') 

GO--

INSERT INTO [Settings].[Localization] (ResourceKey, ResourceValue, LanguageId) VALUES
	('Core.Orders.Order.OrderPickPoint', 'Пункт выдачи', 1),
	('Core.Orders.Order.OrderPickPoint', 'Pick point', 2)

GO--

IF NOT EXISTS(SELECT * FROM sys.indexes WHERE name = 'ProductExportOptions_ProductId_index' AND object_id = OBJECT_ID('[Catalog].[ProductExportOptions]'))
BEGIN
    CREATE CLUSTERED INDEX ProductExportOptions_ProductId_index
        ON Catalog.ProductExportOptions (ProductId)
END

GO--

IF NOT EXISTS(SELECT 1 FROM [Settings].[Settings] WHERE [Name] = 'Api_MobileApp_SignInPicture')
BEGIN
	Insert Into [Settings].[Settings] ([Name], [Value]) Values ('Api_MobileApp_SignInPicture', 'signin.png')
END

GO--

IF NOT EXISTS(SELECT 1 FROM [Settings].[Settings] WHERE [Name] = 'Api_MobileApp_CartPicture')
BEGIN
	Insert Into [Settings].[Settings] ([Name], [Value]) Values ('Api_MobileApp_CartPicture', 'cart.png')
END

GO--

INSERT INTO [Settings].[Localization] (ResourceKey, ResourceValue, LanguageId) VALUES
	('Core.SettingsApiAuth.MobileAppMainPageMode.MainCategories', 'Главные категории', 1),
	('Core.SettingsApiAuth.MobileAppMainPageMode.MainCategories', 'Main categories', 2),
	('Core.SettingsApiAuth.MobileAppMainPageMode.AllCategoriesWithProducts', 'Весь каталог', 1),
	('Core.SettingsApiAuth.MobileAppMainPageMode.AllCategoriesWithProducts', 'All catalog', 2)

GO--

INSERT INTO [Settings].[Localization] (ResourceKey, ResourceValue, LanguageId) VALUES
	('Core.SettingsApiAuth.MobileAppMainPageMode.Light', 'Светлая', 1),
	('Core.SettingsApiAuth.MobileAppMainPageMode.Light', 'Light', 2),
	('Core.SettingsApiAuth.MobileAppMainPageMode.Dark', 'Темная', 1),
	('Core.SettingsApiAuth.MobileAppMainPageMode.Dark', 'Dark', 2)

GO--

INSERT INTO [Settings].[Localization] (ResourceKey, ResourceValue, LanguageId) VALUES
	('Admin.Settings.Mobile.BottomPanelCatalogMenuModeRootCategories', 'Открывать страницу с главными категориями', 1),
	('Admin.Settings.Mobile.BottomPanelCatalogMenuModeRootCategories', 'Open a page with the main categories', 2)

INSERT INTO [Settings].[Localization] (ResourceKey, ResourceValue, LanguageId) VALUES
	('Admin.Settings.Mobile.BottomPanelCatalogMenuModeLink', 'Показывать боковое меню', 1),
	('Admin.Settings.Mobile.BottomPanelCatalogMenuModeLink', 'Show side menu', 2)
GO--

INSERT INTO [Settings].[Localization] (ResourceKey, ResourceValue, LanguageId) VALUES
	('Core.SettingsApiAuth.MobileAppProductViewMode.Tile', 'Плитка', 1),
	('Core.SettingsApiAuth.MobileAppProductViewMode.Tile', 'Tile', 2),
	('Core.SettingsApiAuth.MobileAppProductViewMode.List', 'Список', 1),
	('Core.SettingsApiAuth.MobileAppProductViewMode.List', 'List', 2),
	('Core.SettingsApiAuth.MobileAppProductViewMode.Detail', 'Подробно', 1),
	('Core.SettingsApiAuth.MobileAppProductViewMode.Detail', 'Detail', 2)
GO--

INSERT INTO [Settings].[Localization] (ResourceKey, ResourceValue, LanguageId) VALUES 
	('SignInByPhone.SmsCodeTemplate', 'Ваш код для входа: {0}', 1),
	('SignInByPhone.SmsCodeTemplate', 'Code: {0}', 2)
GO--

INSERT INTO [Settings].[Localization] (ResourceKey, ResourceValue, LanguageId) VALUES 
	('Admin.Tasks.TaskChanges.TaskGroup', 'Проект', 1),
	('Admin.Tasks.TaskChanges.TaskGroup', 'Project', 2)
GO--

INSERT INTO [Settings].[Localization] (ResourceKey, ResourceValue, LanguageId) VALUES 
	('Admin.Js.ExportOrders.ShippingPeriod', 'Выгрузить заказы с датой доставки за период', 1),
	('Admin.Js.ExportOrders.ShippingPeriod', 'Unload orders with the delivery date for the period', 2)
GO--

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Вывести ИНН и организацию' WHERE [ResourceKey] = 'Admin.PaymentMethods.Bill.ShowPaymentDetails' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'Output TIN and organization' WHERE [ResourceKey] = 'Admin.PaymentMethods.Bill.ShowPaymentDetails' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Обязательные ИНН и организация' WHERE [ResourceKey] = 'Admin.PaymentMethods.Bill.RequiredPaymentDetails' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'Required TIN and organization' WHERE [ResourceKey] = 'Admin.PaymentMethods.Bill.RequiredPaymentDetails' AND [LanguageId] = 2

INSERT INTO [Settings].[Localization] (ResourceKey, ResourceValue, LanguageId) VALUES 
	('Admin.PaymentMethods.Bill.ShowPaymentDetails.Help', 'Запрашивать ИНН и название организации у покупателя', 1),
	('Admin.PaymentMethods.Bill.ShowPaymentDetails.Help', 'Request details in the client side', 2),
	('Admin.PaymentMethods.Bill.RequiredPaymentDetails.Help', 'Сделать обязательным заполнение ИНН и название организации', 1),
	('Admin.PaymentMethods.Bill.RequiredPaymentDetails.Help', 'Request details in the client side', 2)

GO--

UPDATE [Settings].[Localization] SET [ResourceValue] = N'Изменить покупателя' WHERE [LanguageId] = 1 AND [ResourceKey] = 'Admin.SettingsApi.Index.UpdateCustomer'
UPDATE [Settings].[Localization] SET [ResourceValue] = N'Послать смс-код по номеру телефона' WHERE [LanguageId] = 1 AND [ResourceKey] = 'Admin.SettingsApi.Index.CustomerConfirm'

GO--

SET IDENTITY_INSERT [Order].[ShippingReplaceGeo] ON

IF NOT EXISTS (SELECT * FROM [Order].[ShippingReplaceGeo] WHERE [Id] = 50)
BEGIN
    INSERT INTO [Order].[ShippingReplaceGeo] ([Id],[ShippingType],[InCountryName],[InCountryISO2],[InRegionName],[InCityName],[InDistrict],[OutCountryName],[OutRegionName],[OutCityName],[OutDistrict],[OutDistrictClear],[Enabled],[Sort],[InZip],[OutZip],[Comment])
    VALUES (50,'Sdek','','RU','Кабардино-Балкарская республика','','','','Кабардино-Балкария','','',0,1,0,'','','')
END

SET IDENTITY_INSERT [Order].[ShippingReplaceGeo] OFF

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

INSERT INTO [Settings].[Localization] (ResourceKey, ResourceValue, LanguageId) VALUES
	('Admin.Import.Errors.BadDataFound', 'Не удалось считать 1 или несколько строк из файла', 1),
	('Admin.Import.Errors.BadDataFound', 'Failed to read 1 or more lines from file', 2)

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

UPDATE [Settings].[InternalSettings] SET [settingValue] = '11.0.3' WHERE [settingKey] = 'db_version'
