EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.GeoMode.ShippingPoint.Change', 'Изменить'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.GeoMode.ShippingPoint.Change', 'Change'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.GeoMode.ShippingPoint.SelectPickupPoint', 'Выберите точку самовывоза'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.GeoMode.ShippingPoint.SelectPickupPoint', 'Select pickup point'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.GeoMode.ShippingPoint.DeliverAddressTitle', 'Доставим по адресу'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.GeoMode.ShippingPoint.DeliverAddressTitle', 'We will deliver to your address'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.GeoMode.ShippingPoint.PickPointAddressTitle', 'Точка самовывоза'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.GeoMode.ShippingPoint.PickPointAddressTitle', 'Pickup point'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.ExportImport.MultiOrder.CustomOptions', 'Дополнительные опции'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.ExportImport.MultiOrder.CustomOptions', 'Custom options'

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.MainPageProducts.CopyTheLink', 'Скопировать ссылку'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.MainPageProducts.CopyTheLink', 'Copy the link'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.MainPageProducts.LinkCopiedToClipboard', 'Ссылка успешно скопирована'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.MainPageProducts.LinkCopiedToClipboard', 'Link copied to clipboard'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.MainPageProducts.FailedToCopyLink', 'Не удалось скопировать ссылку'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.MainPageProducts.FailedToCopyLink', 'Failed to copy link'

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.Landing.Cart.Return', 'Вернуться'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.Landing.Cart.Return', 'Return'

GO--

SET IDENTITY_INSERT [Order].[ShippingReplaceGeo] ON 


IF NOT EXISTS (SELECT * FROM [Order].[ShippingReplaceGeo] WHERE [Id] = 51)
INSERT INTO [Order].[ShippingReplaceGeo] ([Id],[ShippingType],[InCountryName],[InCountryISO2],[InRegionName],[InCityName],[InDistrict],[OutCountryName],[OutRegionName],[OutCityName],[OutDistrict],[OutDistrictClear],[Enabled],[Sort],[InZip],[OutZip],[Comment])
VALUES (51,'Boxberry','','RU','Республика Саха (Якутия)','','','','Республика Саха','','',0,1,0,'','','')


SET IDENTITY_INSERT [Order].[ShippingReplaceGeo] OFF

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Home.SettingStoreDashboard.HaveTrial', 'У вас подключен пробный период. Выберите тарифный план с помесячной или годовой оплатой со скидкой.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Home.SettingStoreDashboard.HaveTrial', 'You have a trial period enabled. Choose a monthly or annual plan with a discount.'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Home.CongratulationsDashboard.HaveTrial', 'У вас подключен пробный период. Выберите тарифный план с помесячной или годовой оплатой со скидкой.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Home.CongratulationsDashboard.HaveTrial', 'You have a trial period enabled. Choose a monthly or annual plan with a discount.'

GO--

If not Exists (SELECT 1 
				 FROM INFORMATION_SCHEMA.COLUMNS
				WHERE 
					TABLE_NAME = 'OrderCustomOptions' 
					AND TABLE_SCHEMA = 'Order'
					AND COLUMN_NAME = 'OptionTitle'
					AND DATA_TYPE = 'nvarchar'
					AND CHARACTER_MAXIMUM_LENGTH = 2000)
BEGIN
    ALTER TABLE [Order].[OrderCustomOptions] ALTER COLUMN [OptionTitle] nvarchar(2000) NOT NULL
END

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Bonuses.OnPurchaseTempalte.PurchaseFull', 'Стоимость товаров заказа, участвующих в бонусной программе'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Bonuses.OnPurchaseTempalte.PurchaseFull', 'Cost of order items participating in the bonus program'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Bonuses.OnPurchaseTempalte.Purchase', 'Стоимость заказа c учетом скидки и товаров, участвующих в бонусной программе '
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Bonuses.OnPurchaseTempalte.Purchase', 'Cost of order items participating in the bonus program and discount'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Bonuses.OnPurchaseTempalte.TotalSum', 'Итоговая стоимость заказа'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Bonuses.OnPurchaseTempalte.PurchaseFull', 'Order total cost'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Bonuses.OnPurchaseTempalte.ProductsSum', 'Стоимость товаров заказа'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Bonuses.OnPurchaseTempalte.ProductsSum', 'Order products cost'

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.ShippingMethods.SelfDelivery.Coordinates', 'Координаты'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.ShippingMethods.SelfDelivery.Coordinates', 'Coordinates'

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.MainPageProducts.Price', 'Цена'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.MainPageProducts.Price', 'Price'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.MainPageProducts.Quantity', 'Кол-во'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.MainPageProducts.Quantity', 'Quantity'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.MainPageProducts.Active', 'Актив.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.MainPageProducts.Active', 'Active'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.ProductLists.Price', 'Цена'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.ProductLists.Price', 'Price'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.ProductLists.Quantity', 'Кол-во'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.ProductLists.Quantity', 'Quantity'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.ProductLists.Active', 'Актив.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.ProductLists.Active', 'Active'

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.ExportOrders.Payment', 'Выгрузить заказы с методом оплаты'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.ExportOrders.Payment', 'Export orders with payment method' 

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.ExportImport.MultiOrder.TotalWeight', 'Общий вес'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.ExportImport.MultiOrder.TotalWeight', 'Total weight' 
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.ExportImport.MultiOrder.TotalDimensions', 'Общие габариты'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.ExportImport.MultiOrder.TotalDimensions', 'Total dimensions' 

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Warehouses.Title', 'Магазины'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Warehouses.Title', 'Shops'

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Product.Weight.Kg', 'кг.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Product.Weight.Kg', 'kg.' 
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Product.Weight.Grams', 'гр.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Product.Weight.Grams', 'g.' 

GO--

UPDATE [Order].[ShippingReplaceGeo]
SET [Enabled] = 0
WHERE [Id] = 48

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.ShippingWithInterval.Settings.OrderProcessingDeadline', 'Время оформления заказа, после которого доставка переносится на следующий день'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.ShippingWithInterval.Settings.OrderProcessingDeadline', 'The time of placing the order, after which the delivery is postponed to the next day'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.ShippingWithInterval.Settings.OrderProcessingDeadline.Help', 'При оформлении заказа после указанного времени, минимальной датой для выбора будет следующая доступная дата'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.ShippingWithInterval.Settings.OrderProcessingDeadline.Help', 'When placing an order after the specified time, the minimum date to select will be the next available date'

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.ExportFeeed.SettingsYandex.WarehouseIds', 'Выгружать товары и остатки c выбранных складов'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.ExportFeeed.SettingsYandex.WarehouseIds', 'Export products and stocks from selected warehouses' 
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.ExportFeeed.SettingsYandex.WarehouseIdsHint', 'Выгружать товары и суммарное кол-во c выбранных складов. Влияет на настройки "Не выгружать товар, если в наличии менее" и "Выгружать товары, недоступные к покупке (не в наличии, неактивные, без цены)", наличие будет с учетом складов.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.ExportFeeed.SettingsYandex.WarehouseIdsHint', 'Export products and total amount from selected warehouses' 

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Order.NoAccess', 'Нет доступа'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Order.NoAccess', 'No access'

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Checkout.ShowBriefDescriptionProductInCheckout', 'Отображать краткое описание товара на странице оформления заказа'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Checkout.ShowBriefDescriptionProductInCheckout', 'Display a short product description on the checkout page' 
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Checkout.ShowBriefDescriptionProductInCheckout.Help', 'Включает отображение краткого описания на странице оформления заказа'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Checkout.ShowBriefDescriptionProductInCheckout.Help', 'Enables display of a short description on the checkout page' 

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Partners.SkipRegPasportStepForIndividualEntity', 'Разрешить пропускать шаг регистрации паспортных данных для физ. лиц'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Partners.SkipRegPasportStepForIndividualEntity', 'Allow to skip the step of registering passport data for individuals' 

GO--

INSERT INTO [Settings].[Settings] (Name, Value)
VALUES
    ('PageWarehousesH1', 'Магазины'),
    ('PageWarehousesTitle', '#STORE_NAME# - Магазины'),
    ('PageWarehousesMetaKeywords', '#STORE_NAME# - Магазины'),
    ('PageWarehousesMetaDescription', '#STORE_NAME# - Магазины')
	
GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.SeoSettings.Shop', 'Склады (мета по умолчанию)'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.SeoSettings.ShopDefaultH1', 'Заголовок H1'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.SeoSettings.ShopDefaultMetaDescription', 'Мета описание'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.SeoSettings.ShopDefaultMetaKeywords', 'Ключевые слова'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.SeoSettings.ShopDefaultTitle', 'Title страницы'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.SeoSettings.Shop', 'Shop (default meta)'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.SeoSettings.ShopDefaultH1', 'H1 header'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.SeoSettings.ShopDefaultMetaDescription', 'Meta description'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.SeoSettings.ShopDefaultMetaKeywords', 'Meta keywords'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.SeoSettings.ShopDefaultTitle', 'Page title'
GO--
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsSeo.SeoSettings.ResetMetaShop', 'Сбросить мета информацию для всех складов'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsSeo.SeoSettings.ResetMetaShop', 'Reset meta information for all warehouses'

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Commonpage.Warehouses', 'Склады'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Commonpage.Warehouses', 'Warehouses'

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Commonpage.WarehousesAbout', 'Склады, типы складов, ярлыки остатков'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Commonpage.WarehousesAbout', 'Warehouses, warehouse types, stock labels'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Warehouses.Title', 'Склады'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Warehouses.Title', 'Warehouses' 
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.WarehousesTypes.Title', 'Типы складов'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.WarehousesTypes.Title', 'Warehouse types' 
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.WarehousesStockLabel.Title', 'Ярлыки остатков'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.WarehousesStockLabel.Title', 'Stock labels' 
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.WarehousesDomains.Title', 'Домены'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.WarehousesDomains.Title', 'Domains' 
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Warehouses.AddWarehouse', 'Добавить склад'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Warehouses.AddWarehouse', 'Add warehouse' 
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Warehouses.DefaultWarehouse.Title', 'Запасной склад'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Warehouses.DefaultWarehouse.Title', 'Spare warehouse' 
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Warehouses.DefaultWarehouse.About', 'Данный склад будет использоваться, когда по остальным нет доступных остатков'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Warehouses.DefaultWarehouse.About', 'This warehouse will be used when there are no available balances for the other warehouses' 
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.WarehousesList.WarehouseName', 'Название'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.WarehousesList.WarehouseName', 'Name' 
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.WarehousesList.CityName', 'Город'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.WarehousesList.CityName', 'City' 
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.WarehousesList.TypeName', 'Тип'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.WarehousesList.TypeName', 'Type' 
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.WarehousesList.SortOrder', 'Поряд.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.WarehousesList.SortOrder', 'Sort order'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.WarehousesList.Enabled', 'Актив.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.WarehousesList.Enabled', 'Active'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.WarehousesList.Amount', 'Остатки'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.WarehousesList.Amount', 'Amount'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.WarehousesList.EnabledFilter', 'Активность'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.WarehousesList.EnabledFilter', 'Activity'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.WarehousesList.EnabledFilter.Enabled', 'Активные'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.WarehousesList.EnabledFilter.Enabled', 'Active'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.WarehousesList.EnabledFilter.Disabled', 'Неактивные'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.WarehousesList.EnabledFilter.Disabled', 'Inactive'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.WarehousesList.SortOrderFilter', 'Сортировка'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.WarehousesList.SortOrderFilter', 'Sorting'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.WarehousesList.TypeNameFilter', 'Тип'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.WarehousesList.TypeNameFilter', 'Type'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.WarehousesList.CityNameFilter', 'Город'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.WarehousesList.CityNameFilter', 'City'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.WarehousesList.WarehouseNameFilter', 'Название'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.WarehousesList.WarehouseNameFilter', 'Name'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.WarehousesList.DeleteSelected', 'Удалить выделенные'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.WarehousesList.DeleteSelected', 'Delete selected'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.WarehousesList.DeleteSelected.Confirm', 'OK'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.WarehousesList.DeleteSelected.Confirm', 'OK'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.WarehousesList.DeleteSelected.Cancel', 'Отмена'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.WarehousesList.DeleteSelected.Cancel', 'Cancel'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.WarehousesList.DeleteSelected.Title', 'Удаление'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.WarehousesList.DeleteSelected.Title', 'Deleting'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.WarehousesList.DeleteSelected.Notification', 'Вы уверены, что хотите удалить?'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.WarehousesList.DeleteSelected.Notification', 'Are you sure you want to delete?'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.WarehousesList.MakeActive', 'Сделать активными'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.WarehousesList.MakeActive', 'Make active'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.WarehousesList.MakeInactive', 'Сделать неактивными'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.WarehousesList.MakeInactive', 'Make inactive'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.WarehousesList.Edit', 'Редактировать'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.WarehousesList.Edit', 'Edit'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.WarehousesList.Delete', 'Удалить'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.WarehousesList.Delete', 'Delete'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Warehouse.AddEdit.Title', 'Склад'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Warehouse.AddEdit.Title', 'Warehouse'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Warehouse.AddEdit.NewWarehouse', 'Новый склад'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Warehouse.AddEdit.NewWarehouse', 'New warehouse'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Warehouse.AddEdit.Delete', 'Удалить'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Warehouse.AddEdit.Delete', 'Delete'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Warehouse.AddEdit.Back', 'Все склады'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Warehouse.AddEdit.Back', 'All warehouses'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Warehouse.AddEdit.Main', 'Основное'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Warehouse.AddEdit.Main', 'Main'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Warehouse.AddEdit.Name', 'Название'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Warehouse.AddEdit.Name', 'Name'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Warehouse.AddEdit.Enabled', 'Активность'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Warehouse.AddEdit.Enabled', 'Enabled'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Warehouse.AddEdit.Type', 'Тип склада'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Warehouse.AddEdit.Type', 'Warhouse type'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Warehouse.AddEdit.Type.NotSelected', 'Не выбран'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Warehouse.AddEdit.Type.NotSelected', 'Not selected'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Warehouse.AddEdit.Type.Delete', 'Удалить'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Warehouse.AddEdit.Type.Delete', 'Delete'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Warehouse.AddEdit.Type.Select', 'Выбрать'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Warehouse.AddEdit.Type.Select', 'Select'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Warehouse.AddEdit.WorkTime', 'Рабочее время'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Warehouse.AddEdit.WorkTime', 'Work time'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Warehouse.AddEdit.WarehouseDescription', 'Описание склада'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Warehouse.AddEdit.WarehouseDescription', 'Warehouse description'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Warehouse.AddEdit.WarehouseDescription.Description', 'Описание'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Warehouse.AddEdit.WarehouseDescription.Description', 'Description'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Warehouse.AddEdit.Geo', 'Гео привязка'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Warehouse.AddEdit.Geo', 'Geo-referencing'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Warehouse.AddEdit.Geo.About', 'Населенные пункты, к которым привязан склад'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Warehouse.AddEdit.Geo.About', 'Settlements to which the warehouse is linked'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Warehouse.AddEdit.Address', 'Адрес'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Warehouse.AddEdit.Address', 'Address'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Warehouse.AddEdit.Address.About', 'Местонахождение склада'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Warehouse.AddEdit.Address.About', 'Warehouse location'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Warehouse.AddEdit.City', 'Город'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Warehouse.AddEdit.City', 'City'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Warehouse.AddEdit.Coordinates', 'Координаты'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Warehouse.AddEdit.Coordinates', 'Coordinates'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Warehouse.AddEdit.Latitude', 'широта'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Warehouse.AddEdit.Latitude', 'latitude'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Warehouse.AddEdit.Longitude', 'долгота'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Warehouse.AddEdit.Longitude', 'longitude'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Warehouse.AddEdit.Commentary', 'Комметарий'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Warehouse.AddEdit.Commentary', 'Commentary'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Warehouse.AddEdit.Contacts', 'Контакты'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Warehouse.AddEdit.Contacts', 'Contacts'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Warehouse.AddEdit.Phone', 'Телефон'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Warehouse.AddEdit.Phone', 'Phone'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Warehouse.AddEdit.AdditionalPhone', 'Дополнительный телефон'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Warehouse.AddEdit.AdditionalPhone', 'Additional phone'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Warehouse.AddEdit.Email', 'Электронная почта'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Warehouse.AddEdit.Email', 'Email'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Warehouse.AddEdit.SEO', 'SEO'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Warehouse.AddEdit.SEO', 'SEO'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Warehouse.AddEdit.SEO.About', 'SEO параметры заполняются автоматически по шаблону, указанному в настройках.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Warehouse.AddEdit.SEO.About', 'SEO parameters are filled in automatically according to the template specified in the settings.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Warehouse.AddEdit.SEOSynonym', 'Синоним для URL запроса'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Warehouse.AddEdit.SEOSynonym', 'Synonym for request URL'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Warehouse.AddEdit.SEOSynonym.Help', 'Обязательное поле. Синоним не должен содержать пробелов и знаков препинания, кроме "_" и "-" Например: podarki-detyam'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Warehouse.AddEdit.SEOSynonym.Help', 'Required field. The synonym must not contain spaces and punctuation marks except "_" and "-" For example: childrens-gifts'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Warehouse.AddEdit.MetaDefault', 'Использовать Meta по умолчанию'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Warehouse.AddEdit.MetaDefault', 'Use Meta by default'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Warehouse.AddEdit.Seo.IfEnable', 'Если опция включена, SEO настройки будут взяты из общих настроек магазина.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Warehouse.AddEdit.Seo.IfEnable', 'If this option is enabled, the SEO settings will be taken from the general store settings.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Warehouse.AddEdit.Seo.IfDisable', 'Если опция выключена, SEO настройки для товара будут взяты с формы ниже.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Warehouse.AddEdit.Seo.IfDisable', 'If the option is disabled, the SEO settings for the product will be taken from the form below.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Warehouse.AddEdit.Seo.MoreInfo', 'Подробнее:'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Warehouse.AddEdit.Seo.MoreInfo', 'More info:'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Warehouse.AddEdit.Seo.DefaultMetaSettings', 'Настройка мета по умолчанию для магазина.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Warehouse.AddEdit.Seo.DefaultMetaSettings', 'Default meta settings for the store.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Warehouse.AddEdit.Seo.SeoTitle', 'Title страницы'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Warehouse.AddEdit.Seo.SeoTitle', 'Page title'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Warehouse.AddEdit.Seo.SeoH1', 'Заголовок H1'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Warehouse.AddEdit.Seo.SeoH1', 'H1 header'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Warehouse.AddEdit.Seo.SeoKeywords', 'Ключевые слова'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Warehouse.AddEdit.Seo.SeoKeywords', 'Keywords'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Warehouse.AddEdit.Seo.SeoDescription', 'Мета описание'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Warehouse.AddEdit.Seo.SeoDescription', 'Meta Description'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Warehouse.AddEdit.Display', 'Отображение'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Warehouse.AddEdit.Display', 'Display'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Warehouse.AddEdit.SortingOrder', 'Порядок сортировки'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Warehouse.AddEdit.SortingOrder', 'Sorting order'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Warehouse.AddEdit.SortingOrder.About', 'Порядковый номер склада. Используется сортировка по возрастанию (ноль вверху).'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Warehouse.AddEdit.SortingOrder.About', 'Order number of the warehouse. Sorting in ascending order (zero at the top) is used.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.TimesOfWork.Add', 'Добавить'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.TimesOfWork.Add', 'Add'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.TimesOfWork.DaysOfWeek', 'Дни недели'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.TimesOfWork.DaysOfWeek', 'Days of Week'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.TimesOfWork.Monday', 'Пн'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.TimesOfWork.Monday', 'Mon'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.TimesOfWork.Tuesday', 'Вт'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.TimesOfWork.Tuesday', 'Tu'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.TimesOfWork.Wednesday', 'Ср'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.TimesOfWork.Wednesday', 'We'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.TimesOfWork.Thursday', 'Чт'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.TimesOfWork.Thursday', 'Th'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.TimesOfWork.Friday', 'Пт'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.TimesOfWork.Friday', 'Fr'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.TimesOfWork.Saturday', 'Сб'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.TimesOfWork.Saturday', 'Sa'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.TimesOfWork.Sunday', 'Вс'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.TimesOfWork.Sunday', 'Su'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.TimesOfWork.WorkingTime', 'Время работы'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.TimesOfWork.WorkingTime', 'Working time'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.TimesOfWork.OpeningTime', 'Время открытия'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.TimesOfWork.OpeningTime', 'Opening time'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.TimesOfWork.ClosingTime', 'Время закрытия'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.TimesOfWork.ClosingTime', 'Closing time'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.TimesOfWork.Break', 'Перерыв'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.TimesOfWork.Break', 'Break'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.TimesOfWork.StartOfBreak', 'Начало перерыва'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.TimesOfWork.StartOfBreak', 'Start of break'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.TimesOfWork.EndOfBreak', 'Окончание перерыва'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.TimesOfWork.EndOfBreak', 'End of break'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.WarehouseCities.Add', 'Добавить'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.WarehouseCities.Add', 'Add'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.SelectCities.Title', 'Выбор города'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.SelectCities.Title', 'City selection'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.SwitchOnOff.On', 'Вкл'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.SwitchOnOff.On', 'On'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.SwitchOnOff.Off', 'Выкл'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.SwitchOnOff.Off', 'Off'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.SelectTypeWarehouse.Title', 'Типы складов'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.SelectTypeWarehouse.Title', 'Warehouse types'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.SelectTypeWarehouse.TypeName', 'Название'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.SelectTypeWarehouse.TypeName', 'Name'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.SelectTypeWarehouse.SortOrder', 'Поряд.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.SelectTypeWarehouse.SortOrder', 'Sort order'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.SelectTypeWarehouse.Enabled', 'Актив.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.SelectTypeWarehouse.Enabled', 'Active'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.SelectTypeWarehouse.TypeNameFilter', 'Название'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.SelectTypeWarehouse.TypeNameFilter', 'Title'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.SelectTypeWarehouse.SortOrderFilter', 'Сортировка'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.SelectTypeWarehouse.SortOrderFilter', 'Sorting'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.SelectTypeWarehouse.EnabledFilter', 'Активность'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.SelectTypeWarehouse.EnabledFilter', 'Activity'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.SelectTypeWarehouse.EnabledFilter.Active', 'Активные'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.SelectTypeWarehouse.EnabledFilter.Active', 'Active'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.SelectTypeWarehouse.EnabledFilter.Inactive', 'Неактивные'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.SelectTypeWarehouse.EnabledFilter.Inactive', 'Inactive'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.SelectTypeWarehouse.Select', 'Выбрать'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.SelectTypeWarehouse.Select', 'Select'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.WarehousesTypes.Add', 'Добавить тип'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.WarehousesTypes.Add', 'Add type'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.WarehouseTypes.TypeName', 'Название'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.WarehouseTypes.TypeName', 'Name'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.WarehouseTypes.SortOrder', 'Поряд.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.WarehouseTypes.SortOrder', 'Sort order'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.WarehouseTypes.Enabled', 'Актив.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.WarehouseTypes.Enabled', 'Active'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.WarehouseTypes.TypeNameFilter', 'Название'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.WarehouseTypes.TypeNameFilter', 'Name'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.WarehouseTypes.SortOrderFilter', 'Сортировка'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.WarehouseTypes.SortOrderFilter', 'Sorting'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.WarehouseTypes.EnabledFilter', 'Активность'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.WarehouseTypes.EnabledFilter', 'Activity'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.WarehouseTypes.EnabledFilter.Active', 'Активные'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.WarehouseTypes.EnabledFilter.Active', 'Active'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.WarehouseTypes.EnabledFilter.Inactive', 'Неактивные'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.WarehouseTypes.EnabledFilter.Inactive', 'Inactive'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.WarehouseTypes.Edit', 'Редактировать'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.WarehouseTypes.Edit', 'Edit'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.WarehouseTypes.Delete', 'Удалить'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.WarehouseTypes.Delete', 'Delete'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.WarehouseTypes.DeleteSelected', 'Удалить выделенные'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.WarehouseTypes.DeleteSelected', 'Delete selected'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.WarehouseTypes.DeleteSelected.Confirm', 'OK'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.WarehouseTypes.DeleteSelected.Confirm', 'OK'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.WarehouseTypes.DeleteSelected.Cancel', 'Отмена'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.WarehouseTypes.DeleteSelected.Cancel', 'Cancel'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.WarehouseTypes.DeleteSelected.Title', 'Удаление'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.WarehouseTypes.DeleteSelected.Title', 'Deleting'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.WarehouseTypes.DeleteSelected.Notification', 'Вы уверены, что хотите удалить?'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.WarehouseTypes.DeleteSelected.Notification', 'Are you sure you want to delete?'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.WarehouseTypes.MakeActive', 'Сделать активными'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.WarehouseTypes.MakeActive', 'Make active'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.WarehouseTypes.MakeInactive', 'Сделать неактивными'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.WarehouseTypes.MakeInactive', 'Make inactive'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.GridCustomComponent.Confirm', 'OK'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.GridCustomComponent.Confirm', 'OK'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.GridCustomComponent.Cancel', 'Отмена'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.GridCustomComponent.Cancel', 'Cancel'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.AddEditTypeWarehouse.Title', 'Тип склада'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.AddEditTypeWarehouse.Title', 'Warehouse type'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.AddEditTypeWarehouse.Name', 'Название'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.AddEditTypeWarehouse.Name', 'Name'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.AddEditTypeWarehouse.Enabled', 'Активность'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.AddEditTypeWarehouse.Enabled', 'Enabled'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.AddEditTypeWarehouse.SortOrder', 'Порядок сортировки'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.AddEditTypeWarehouse.SortOrder', 'Sort order'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.AddEditTypeWarehouse.Success', 'Изменения сохранены'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.AddEditTypeWarehouse.Success', 'Changes have been saved'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.AddEditTypeWarehouse.Error', 'Ошибка'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.AddEditTypeWarehouse.Error', 'Error'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.AddEditTypeWarehouse.Error.Data', 'Неудалось получить данные типа склада'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.AddEditTypeWarehouse.Error.Data', 'Failed to get warehouse type data'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.AddEditTypeWarehouse.Error.Save', 'Ошибка при сохранении'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.AddEditTypeWarehouse.Error.Save', 'Error when saving'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.WarehousesStockLabel.Add', 'Добавить ярлык'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.WarehousesStockLabel.Add', 'Add label' 
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.StockLabel.Name', 'Название'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.StockLabel.Name', 'Name'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.StockLabel.NameFilter', 'Название'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.StockLabel.NameFilter', 'Name'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.StockLabel.AmountUpTo', 'до N единиц'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.StockLabel.AmountUpTo', 'Amount up to'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.StockLabel.AmountUpToFilter', 'до N единиц'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.StockLabel.AmountUpToFilter', 'Amount up to'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.StockLabel.Edit', 'Редактировать'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.StockLabel.Edit', 'Edit'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.StockLabel.Delete', 'Удалить'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.StockLabel.Delete', 'Delete'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.StockLabel.DeleteSelected', 'Удалить выделенные'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.StockLabel.DeleteSelected', 'Delete selected'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.StockLabel.DeleteSelected.Confirm', 'OK'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.StockLabel.DeleteSelected.Confirm', 'OK'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.StockLabel.DeleteSelected.Cancel', 'Отмена'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.StockLabel.DeleteSelected.Cancel', 'Cancel'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.StockLabel.DeleteSelected.Title', 'Удаление'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.StockLabel.DeleteSelected.Title', 'Deleting'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.StockLabel.DeleteSelected.Notification', 'Вы уверены, что хотите удалить?'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.StockLabel.DeleteSelected.Notification', 'Are you sure you want to delete?'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.AddEditStockLabel.Title', 'Ярлык остатков'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.AddEditStockLabel.Title', 'Stock label'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.AddEditStockLabel.Name', 'Название'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.AddEditStockLabel.Name', 'Name'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.AddEditStockLabel.ClientName', 'Название в клиентской части'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.AddEditStockLabel.ClientName', 'Name in client side'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.AddEditStockLabel.Color', 'Цвет'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.AddEditStockLabel.Color', 'Color'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.AddEditStockLabel.AmountUpTo', 'Количество до'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.AddEditStockLabel.AmountUpTo', 'Amount up to'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.AddEditStockLabel.Success', 'Изменения сохранены'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.AddEditStockLabel.Success', 'Changes have been saved'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.AddEditStockLabel.Error', 'Ошибка'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.AddEditStockLabel.Error', 'Error'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.AddEditStockLabel.Error.Data', 'Неудалось получить данные ярлыка'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.AddEditStockLabel.Error.Data', 'Failed to get label data'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.AddEditStockLabel.Error.Save', 'Ошибка при сохранении'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.AddEditStockLabel.Error.Save', 'Error when saving'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.WarehousesDomains.Subtitle', 'Возможность создать привязку домена к городам. Если у города назначен склад, то поэтому домену будут показываться товары со склада города.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.WarehousesDomains.Subtitle', 'Possibility to create binding of domain to cities. If a city has a warehouse assigned to it, the domain will show goods from the city''s warehouse.' 
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.WarehousesDomains.Add', 'Добавить домен'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.WarehousesDomains.Add', 'Add domain'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.DomainGeoLocationList.Url', 'Домен'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.DomainGeoLocationList.Url', 'Domain'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.DomainGeoLocationList.UrlFilter', 'Домен'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.DomainGeoLocationList.UrlFilter', 'Domain'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.DomainGeoLocationList.Cities', 'Города'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.DomainGeoLocationList.Cities', 'Cities'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.DomainGeoLocationList.CitiesFilter', 'Город'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.DomainGeoLocationList.CitiesFilter', 'City'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.DomainGeoLocationList.Edit', 'Редактировать'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.DomainGeoLocationList.Edit', 'Edit'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.DomainGeoLocationList.Delete', 'Удалить'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.DomainGeoLocationList.Delete', 'Delete'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.DomainGeoLocationList.DeleteSelected', 'Удалить выделенные'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.DomainGeoLocationList.DeleteSelected', 'Delete selected'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.DomainGeoLocationList.DeleteSelected.Confirm', 'OK'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.DomainGeoLocationList.DeleteSelected.Confirm', 'OK'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.DomainGeoLocationList.DeleteSelected.Cancel', 'Отмена'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.DomainGeoLocationList.DeleteSelected.Cancel', 'Cancel'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.DomainGeoLocationList.DeleteSelected.Title', 'Удаление'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.DomainGeoLocationList.DeleteSelected.Title', 'Deleting'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.DomainGeoLocationList.DeleteSelected.Notification', 'Вы уверены, что хотите удалить?'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.DomainGeoLocationList.DeleteSelected.Notification', 'Are you sure you want to delete?'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.AddEditDomainGeoLocation.Title', 'Домен с геопривязкой'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.AddEditDomainGeoLocation.Title', 'Geo-linked domain'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.AddEditDomainGeoLocation.Domain', 'Домен'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.AddEditDomainGeoLocation.Domain', 'Domain'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.AddEditDomainGeoLocation.Domain.About', 'Домен, без http, https и / на конце. Например: spb.site.ru, samara.site.ru'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.AddEditDomainGeoLocation.Domain.About', 'Domain, without http, https and / on the end. For example: spb.site.ru, samara.site.ru'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.AddEditDomainGeoLocation.Cities', 'Города'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.AddEditDomainGeoLocation.Cities', 'Cities'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.AddEditDomainGeoLocation.AddCities', 'Добавить города'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.AddEditDomainGeoLocation.AddCities', 'Add cities'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.AddEditDomainGeoLocation.Error', 'Ошибка'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.AddEditDomainGeoLocation.Error', 'Error'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.AddEditDomainGeoLocation.Success', 'Изменения сохранены'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.AddEditDomainGeoLocation.Success', 'Changes have been saved'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.AddEditDomainGeoLocation.Error.Save', 'Ошибка при сохранении'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.AddEditDomainGeoLocation.Error.Save', 'Error when saving'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.AddEditDomainGeoLocation.Error.NonValidDomain', 'Укажите валидный домен. Если домен на русском, используйте'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.AddEditDomainGeoLocation.Error.NonValidDomain', 'Specify a valid domain. If the domain is in Russian, use'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.AddEditDomainGeoLocation.Error.Domain', 'Укажите домен'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.AddEditDomainGeoLocation.Error.Domain', 'Enter the domain'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.AddEditDomainGeoLocation.Error.City', 'Укажите город'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.AddEditDomainGeoLocation.Error.City', 'Enter the city'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.OfferStocks.Title', 'Остатки по складам'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.OfferStocks.Title', 'Stock balances'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.OfferStocks.AddStock', 'Добавить остатки по другим складам'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.OfferStocks.AddStock', 'Add balances for other warehouses'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.OfferStocks.Error', 'Ошибка'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.OfferStocks.Error', 'Error'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.OfferStocks.Error.Save', 'Не удалось сохранить данные остатков'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.OfferStocks.Error.Save', 'Failed to save stock balances data'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.OfferStocks.Error.Warehouse', 'Укажите склад'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.OfferStocks.Error.Warehouse', 'Enter the warehouse'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.OfferStocks.Error.Data', 'Не удалось получить данные остатков'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.OfferStocks.Error.Data', 'Failed to get stock balances data'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.OfferStocks.Error.AdditionalData', 'Не удалось получить дополнительные данные'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.OfferStocks.Error.AdditionalData', 'Failed to get additional data'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.DistributionOfOrderItem.Title', 'Распределение по складам'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.DistributionOfOrderItem.Title', 'Distribution to warehouses'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.DistributionOfOrderItem.Notification', 'Позиция не распределена по складам.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.DistributionOfOrderItem.Notification', 'The item is not allocated to warehouses.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.DistributionOfOrderItem.Available', 'Доступно'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.DistributionOfOrderItem.Available', 'Available'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.DistributionOfOrderItem.GetFromAnother', 'Взять с другого склада'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.DistributionOfOrderItem.GetFromAnother', 'Get it from another warehouse'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.DistributionOfOrderItem.Error', 'Ошибка'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.DistributionOfOrderItem.Error', 'Error'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.DistributionOfOrderItem.Error.Data', 'Не удалось получить данные распределения'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.DistributionOfOrderItem.Error.Data', 'Failed to get distribution data'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.DistributionOfOrderItem.Error.AdditionalData', 'Не удалось получить дополнительные данные'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.DistributionOfOrderItem.Error.AdditionalData', 'Failed to get additional data'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.DistributionOfOrderItem.Error.Warehouse', 'Укажите склад'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.DistributionOfOrderItem.Error.Warehouse', 'Enter the warehouse'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.DistributionOfOrderItem.Error.Save', 'Не удалось сохранить данные распределения'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.DistributionOfOrderItem.Error.Save', 'Failed to save distribution data'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.DistributionOfOrderItem.Save.Title', 'Изменение количества позиции'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.DistributionOfOrderItem.Save.Title', 'Changing the amount of item'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.DistributionOfOrderItem.Save.Confirm', 'OK'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.DistributionOfOrderItem.Save.Confirm', 'OK'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.DistributionOfOrderItem.Save.Cancel', 'Отмена'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.DistributionOfOrderItem.Save.Cancel', 'Cancel'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.DistributionOfOrderItem.Save.More', 'больше'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.DistributionOfOrderItem.Save.More', 'more'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.DistributionOfOrderItem.Save.Less', 'меньше'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.DistributionOfOrderItem.Save.Less', 'less'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.DistributionOfOrderItem.Save.TotalAmount', 'Общее количество распределения'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.DistributionOfOrderItem.Save.TotalAmount', 'Total amount distribution'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.DistributionOfOrderItem.Save.CurrentAmount', 'текущего количества позиции на'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.DistributionOfOrderItem.Save.CurrentAmount', 'the current amount of positions on'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.DistributionOfOrderItem.Save.UpdateAmount', 'Обновить количество на'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.DistributionOfOrderItem.Save.UpdateAmount', 'Update amount on'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Product.ProductStocks.StoresOnMap', 'Магазины на карте'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Product.ProductStocks.StoresOnMap', 'Stores on map'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.StockList.Address', 'Адрес'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.StockList.Address', 'Address'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.StockList.Availability', 'Наличие'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.StockList.Availability', 'Availability'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.StockList.WorkingTime', 'Режим работы'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.StockList.WorkingTime', 'Working time'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.StockList.OutOfStock', 'Товара нет в наличии'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.StockList.OutOfStock', 'Out of stock'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.StockList.OutOfStock', 'Товара нет в наличии'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.StockList.OutOfStock', 'Out of stock'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.Product.ShopsMap.Header', 'Список магазинов'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.Product.ShopsMap.Header', 'Store list'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.WarehousesListMap.List', 'Список'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.WarehousesListMap.List', 'List'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.WarehousesListMap.Map', 'Карта'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.WarehousesListMap.Map', 'Map'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.WarehousesList.Back', 'Вернуться к списку'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.WarehousesList.Back', 'Back to list'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.CartStockInWarehouses.NotAvailable5', 'позиций недоступно в полном объеме'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.CartStockInWarehouses.NotAvailable5', 'positions are not available in full'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.CartStockInWarehouses.NotAvailable2', 'позиции недоступны в полном объеме'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.CartStockInWarehouses.NotAvailable2', 'positions are not available in full'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.CartStockInWarehouses.NotAvailable1', 'позиция недоступна в полном объеме'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.CartStockInWarehouses.NotAvailable1', 'position is not available in full'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.WarehousesMap.All', 'Все'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.WarehousesMap.All', 'All'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Warehouse.AddEdit.Geo', 'Геопривязка'

GO--

IF NOT EXISTS(SELECT 1
              FROM sys.columns
              WHERE (name = N'OriginFileName') AND object_id = OBJECT_ID(N'[CMS].[Attachment]'))
BEGIN
	ALTER TABLE [CMS].[Attachment]
		ADD [OriginFileName] nvarchar(255) NULL;
END

DELETE FROM [Settings].[Localization] WHERE ResourceKey = 'Common.Attachments.FileAlreadyExists'
DELETE FROM [Settings].[Localization] WHERE ResourceKey = 'Admin.Attachments.FileAlreadyExists'

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Checkout.EnableShoppingCartPopup', 'Показывать всплывающую корзину'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Checkout.EnableShoppingCartPopup', 'Show pop-up cart'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Checkout.EnableShoppingCartPopupHint', 'Если настройка включена, то при клике по иконке корзины в шапке для полной версии и в шапке/нижней панели для мобильной версии будет открываться всплывающая корзина.<br>Если настройка выключена, то при клике по иконке корзины в шапке для полной версии и в шапке/нижней панели для мобильной версии будет происходить переход на страницу корзины'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Checkout.EnableShoppingCartPopupHint', 'If the setting is enabled, then when you click on the cart icon in the header for the full version and in the header/bottom panel for the mobile version, a pop-up cart will open.<br>If the setting is disabled, then when you click on the cart icon in the header for the full version and in the header /bottom panel for the mobile version will go to the cart page'

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.MetaVariables.Warehouse.WarehouseName', 'Название склада'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.MetaVariables.Warehouse.WarehouseName', 'Warehouse name'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.EditableGridRow.MarkingRequiredValidation.Title', 'Обязательная маркировка Честный знак'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.EditableGridRow.MarkingRequiredValidation.Title', 'Mandatory marking "Fair sign"'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.EditableGridRow.MarkingRequiredValidation.Add', 'Задать маркировку'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.EditableGridRow.MarkingRequiredValidation.Add', 'Set marking'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.EditableGridRow.Warehouses.Title', 'Склады'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.EditableGridRow.Warehouses.Title', 'Warehouses'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.EditableGridRow.Warehouses.Add', 'Добавить распределение по складам'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.EditableGridRow.Warehouses.Add', 'Add distribution by warehouses'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.SelectTypeWarehouse.Select', 'Выбрать'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.SelectTypeWarehouse.Select', 'Select'

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.Shipping.PointList.NotSelected', 'Не выбрано'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.Shipping.PointList.NotSelected', 'Not selected'

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.MobileVersion.ShowNewsOnMainPage', 'Показывать новости на главной'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.MobileVersion.ShowNewsOnMainPage', 'Show news on main page'

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Bonuses.OnPurchaseTempalte.PurchaseFull', 'Стоимость товаров заказа, участвующих в бонусной программе'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Bonuses.OnPurchaseTempalte.PurchaseFull', 'Cost of order items participating in the bonus program'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Bonuses.OnPurchaseTempalte.Purchase', 'Стоимость заказа c учетом скидки и товаров, участвующих в бонусной программе '
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Bonuses.OnPurchaseTempalte.Purchase', 'Cost of order items participating in the bonus program and discount'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Bonuses.OnPurchaseTempalte.TotalSum', 'Итоговая стоимость заказа'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Bonuses.OnPurchaseTempalte.PurchaseFull', 'Order total cost'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Bonuses.OnPurchaseTempalte.ProductsSum', 'Стоимость товаров заказа'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Bonuses.OnPurchaseTempalte.ProductsSum', 'Order products cost'

GO--

IF NOT EXISTS(SELECT 1
              FROM sys.columns
              WHERE (name = N'OnlyOnCustomerBirthday') AND object_id = OBJECT_ID(N'[Catalog].[Coupon]'))
BEGIN
	ALTER TABLE [Catalog].[Coupon]
		ADD [OnlyOnCustomerBirthday] bit NULL;
END
IF NOT EXISTS(SELECT 1
              FROM sys.columns
              WHERE (name = N'DaysBeforeBirthday') AND object_id = OBJECT_ID(N'[Catalog].[Coupon]'))
BEGIN
	ALTER TABLE [Catalog].[Coupon]
		ADD [DaysBeforeBirthday] int NULL;
END
IF NOT EXISTS(SELECT 1
              FROM sys.columns
              WHERE (name = N'DaysAfterBirthday') AND object_id = OBJECT_ID(N'[Catalog].[Coupon]'))
BEGIN
	ALTER TABLE [Catalog].[Coupon]
		ADD [DaysAfterBirthday] int NULL;
END
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Coupon.CouponPost.OnlyOnCustomerBirthday', 'Купон действует только в день рождения'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Coupon.CouponPost.OnlyOnCustomerBirthday', 'Order products cost'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Coupon.CouponPost.OnlyOnDates', 'Купон действует только c {0} по {1}'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Coupon.CouponPost.OnlyOnDates', 'The coupon is valid only from {0} to {1}'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.AddEditCoupon.OnlyOnCustomerBirthday', 'Действует в день рождения покупателя'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.AddEditCoupon.OnlyOnCustomerBirthday', 'Valid on the customer birthday'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.AddEditCoupon.DaysBeforeBirthday', 'Дней до дня рождения'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.AddEditCoupon.DaysBeforeBirthday', 'Days before the birthday'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.AddEditCoupon.DaysAfterBirthday', 'Дней после дня рождения'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.AddEditCoupon.DaysAfterBirthday', 'Days after the birthday'

GO--

ALTER TABLE Catalog.ProductExportOptions ADD
	Mpn nvarchar(70) NULL
	
GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Catalog.Product.Mpn', 'Код производителя товара (mpn) в Google Merchant Center'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Catalog.Product.Mpn', 'Manufacturer Part Number (MPN) in Google Merchant Center'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.ExportImport.ProductFields.GoogleMpn', 'Код производителя товара (mpn) в Google Merchant Center'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.ExportImport.ProductFields.GoogleMpn', 'Manufacturer Part Number (MPN) in Google Merchant Center'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.ExportImport.EProductField.GoogleMpn', 'Google Merchant: MPN'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.ExportImport.EProductField.GoogleMpn', 'Google Merchant: MPN'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.ExportOptions.Mpn', 'Код производителя товара (mpn) в Google Merchant Center'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.ExportOptions.Mpn', 'Manufacturer Part Number (MPN) in Google Merchant Center'

GO--

CREATE FUNCTION [Settings].[fn_clearPhone](@Phone VARCHAR(max))
RETURNS BIGINT
AS
BEGIN
	DECLARE 
		@PhoneClean NVARCHAR(max),
		@Counter INT,
		@StringLength INT
 
		SET @PhoneClean = '';
		SET @Counter = 1;
		SET @StringLength = LEN(@Phone);
	WHILE @Counter <= @StringLength
	BEGIN
		DECLARE @CurrentChar NVARCHAR(1) = SUBSTRING(@Phone, @Counter, 1)
		SELECT @PhoneClean = @PhoneClean + CASE 
												WHEN @CurrentChar LIKE '[0-9]' 
													THEN @CurrentChar 
													ELSE ''	
										   END
		SELECT @Counter = @Counter + 1
	END
	DECLARE
		@PhoneCleanLength INT = LEN(@PhoneClean),
		@BigIntMaxLength INT = 18
	IF @PhoneCleanLength <= 1 OR @PhoneCleanLength > @BigIntMaxLength
        RETURN NULL
	RETURN CAST(@PhoneClean AS BIGINT)
END

GO--

UPDATE Customers.Customer 
SET StandardPhone = (SELECT [Settings].[fn_clearPhone](Phone)) 
WHERE (StandardPhone IS NULL or StandardPhone = '' or StandardPhone = '0') AND Phone IS NOT NULL AND Phone <> ''

GO--

DROP FUNCTION  [Settings].[fn_clearPhone]

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.SystemSettings.AdminStartPage', 'Начальная страница администрирования'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.SystemSettings.AdminStartPage', 'Administration Home Page'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.SettingsSystem.ScheduledTasks', 'Задачи по расписанию'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.SettingsSystem.ScheduledTasks', 'Scheduled tasks'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsSystem.SystemCommon.Capcha', 'Капча'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsSystem.SystemCommon.Capcha', 'Capcha'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsSystem.SystemCommon.MoreDetails', 'Подробнее'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsSystem.SystemCommon.MoreDetails', 'More details'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsSystem.SystemCommon.NumberOfCharacters', 'Кол-во символов'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsSystem.SystemCommon.NumberOfCharacters', 'Number of characters'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.SystemSettings.FilesSize', 'Размер файлов'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.SystemSettings.FilesSize', 'File size'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.OrderItemsSummary.OrderDiscount', 'Скидка'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.OrderItemsSummary.OrderDiscount', 'Discount'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.OrderItemsSummary.CouponCertificate', 'Купон/Сертификат'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.OrderItemsSummary.CouponCertificate', 'Coupon/Certificate'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Customers.ViewCustomerHeader.Statistics', 'Статистика'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Customers.ViewCustomerHeader.Statistics', 'Statistics'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.SystemSettings.DocumentGeneration', 'Генерация документов'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.SystemSettings.DocumentGeneration', 'Document generation'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.SystemSettings.SelectTemplates', 'Выберите шаблоны'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.SystemSettings.SelectTemplates', 'Select templates'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.SystemSettings.CreateDocuments', 'Создать документы'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.SystemSettings.CreateDocuments', 'Create documents'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.SystemSettings.AttachToFiles', 'Прикрепить к файлам'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.SystemSettings.AttachToFiles', 'Attach to files'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.SystemSettings.NoTemplates', 'Нет шаблонов'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.SystemSettings.NoTemplates', 'No templates'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.SystemSettings.ChangeAddress', 'Сменить адрес'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.SystemSettings.ChangeAddress', 'Change address'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.OrderRecipient.Recipient', 'Получатель заказа'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.OrderRecipient.Recipient', 'Order recipient'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.OrderRecipient.LastName', 'Фамилия'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.OrderRecipient.LastName', 'Last Name'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.OrderRecipient.FirstName', 'Имя'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.OrderRecipient.FirstName', 'First Name'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.OrderRecipient.PatronymicName', 'Отчество'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.OrderRecipient.PatronymicName', 'Patronymic Name'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.OrderRecipient.Phone', 'Номер телефона'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.OrderRecipient.Phone', 'Phone'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.OrderRecipient.Save', 'Сохранить'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.OrderRecipient.Save', 'Save'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.OrderRecipient.Cancel', 'Выйти'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.OrderRecipient.Cancel', 'Cancel'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Index.SalesPlan', 'Profitability settings'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Coupons.Index.Settings', 'Настройки'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Coupons.Index.Settings', 'Settings'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Index.Settings', 'Настройки'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Index.Settings', 'Settings'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Commonpage.Settings', 'Настройки'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Commonpage.Settings', 'Settings'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.PaymentMethods.Settings', 'Настройки'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.PaymentMethods.Settings', 'Settings'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Shippingsmethods.Settings', 'Настройки'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Shippingsmethods.Settings', 'Settings'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Telephony.Settings', 'Настройки'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Telephony.Settings', 'Settings'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsCoupons.Coupons.CouponInstruction', 'Инструкция. Купоны'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsCoupons.Coupons.CouponInstruction', 'Instructions. Coupons'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsCoupons.Coupons.TriggerMarketingInstruction', 'Инструкция. Триггерный маркетинг'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsCoupons.Coupons.TriggerMarketingInstruction', 'Instructions. Trigger marketing'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsCoupons.Coupons.TriggerMarketingText', 'Здесь отображаются купоны, сгенерированные в канале продаж "Триггерный маркетинг'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsCoupons.Coupons.TriggerMarketingText', 'Coupons generated in the Trigger Marketing sales channel are displayed here'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.AddEditCoupon.AddProductsText', 'Можно будет назначить после создания купона'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.AddEditCoupon.AddProductsText', 'Can be assigned after creating a coupon'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.CertificateSettings.SubjectOfCalculation', 'Предмет расчета'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.CertificateSettings.SubjectOfCalculation', 'Subject of calculation'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.CertificateSettings.CheckExplanation', 'Данный параметр будет передаваться для печати чеков'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.CertificateSettings.CheckExplanation', 'This parameter will be passed for printing receipts'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.CertificateSettings.CalculationMethod', 'Способ расчета'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.CertificateSettings.CalculationMethod', 'Calculation method'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Social.SocialWidget.Full', 'Полная версия'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Social.SocialWidget.Full', 'Full version'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Social.SocialWidget.Mobile', 'Мобильная версия'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Social.SocialWidget.Mobile', 'Mobile version'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.PaymentMethods.InstructionPayMethods', 'Инструкция. Cпособы оплаты в интернет-магазине'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.PaymentMethods.InstructionPayMethods', 'Instructions. Payment methods in the online store'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Shippingsmethods.InstructionShippingsMethods', 'Инструкция. Cпособы доставки в интернет-магазине'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Shippingsmethods.InstructionShippingsMethods', 'Instructions. Delivery methods in the online store'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Social.SocialWidget.Icon', 'Иконка'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Social.SocialWidget.Icon', 'Icon'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Telephony.Operator', 'Telecom operator'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Telephony.PhonerLite', 'Integration with PhonerLite'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Telephony.PhonerLiteNote', 'The operator installs the PhonerLite program on his computer. When you click on the phone number in the admin. panel starts calling the client.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Telephony.Mango.ApiUrl', 'Virtual PBX API Address'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Telephony.Mango.ApiKey', 'Unique PBX code'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Telephony.Mango.SecretKey', 'Key to create a signature'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Telephony.Mango.ServiceUrl', 'Url for notifications'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Telephony.Mango.ServiceUrlNote', 'Notifications about events on the PBX will be sent to this address.<br />You must specify it in the settings in your Mango Office personal account (PBX Settings -> API Connection -> External System Data -> "External System Address" field)'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Telephony.CallBack.MangoExtension', 'Extension number'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Telephony.CallBack.MangoExtensionNote', 'VATS employee ID. Required field. If the VATS employee does not have an ID (extension number), he will not be able to execute the call initiation command.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Telephony.CallBack.ShowMode', 'Display Mode'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Telephony.CallBack.TimeInterval', 'Number of seconds'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Telephony.CallBack.TimeIntervalNote', 'Only for display to the user.<br />Does not affect the speed of the service.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Telephony.CallBack.WorkTimeText', 'Text of a successful application during business hours'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Telephony.CallBack.WorkTimeTextHelp', 'The #SECONDS# variable will be replaced <br>by the number of seconds specified in the settings <br>(e.g. "20 seconds" if the specified number of seconds is 20)'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Telephony.CallBack.WorkTimeTextNote', 'Text displayed after clicking the "Request a call" button during business hours.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Telephony.CallBack.NotWorkTimeText', 'Text of a successful request after hours'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Telephony.CallBack.NotWorkTimeTextNote', 'Text displayed after clicking the "Request a call" button during non-working hours.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Telephony.CallBack.WorkSchedule', 'Call time'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Telephony.CallBack.WorkScheduleNote', 'Schedule for receiving calls.<br />During business hours, the call will be directed to an internal number; during non-working hours, a lead will be created.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Telephony.Instruction', 'Инструкция. Возможности при подключении IP телефонии'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Telephony.Instruction', 'Instructions. Possibilities when connecting IP telephony'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Booking.Tags.AddTag', 'Добавить тэг'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Booking.Tags.AddTag', 'Add tag'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Booking.Tags', 'Tags'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Booking.Tags.Grid.SearchPlaceholder', 'Search by name'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Booking.Commonpage.Settings', 'Настройки'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Booking.Commonpage.Settings', 'Settings'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Booking.Commonpage.InstructionsBranches', 'Инструкция. Филиалы'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Booking.Commonpage.InstructionsBranches', 'Instructions. Branches'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsCrm.Index.InstructionsBranches', 'Инструкция. Филиалы'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsCrm.Index.InstructionsBranches', 'Instructions. Branches'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Settings.Booking.AddEditTag.Tag', 'Тег'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Settings.Booking.AddEditTag.Tag', 'Tag'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Settings.Booking.AddEditTag.Name', 'Название'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Settings.Booking.AddEditTag.Name', 'Name'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Settings.Booking.AddEditTag.SortOrder', 'Порядок сортировки'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Settings.Booking.AddEditTag.SortOrder', 'Sort order'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Commonpage.Userssettings.Settings', 'Настройки'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Commonpage.Userssettings.Settings', 'Settings'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Users.Employees.InstructionEmployees', 'Инструкция. Добавление сотрудника'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Users.Employees.InstructionEmployees', 'Instructions. Adding an employee'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Users.GridUsers.SearchPlaceholder', 'Search by name and e-mail'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Users.Grid.TotalString', 'Users found: {0}'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Users.Departments.InstructionsDepartments', 'Инструкция. Отделы'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Users.Departments.InstructionsDepartments', 'Instructions. Departments'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Departments.Grid.SearchPlaceholder', 'Search by name'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Departments.Grid.TotalString', 'Departments found: {0}'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Users.Roles.InstructionsRoles', 'Инструкция. Роли'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Users.Roles.InstructionsRoles', 'Instructions. Roles'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.ManagerRoles.Grid.SearchPlaceholder', 'Search by name'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.ManagerRoles.Grid.TotalString', 'Roles found: {0}'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Users.EmployeeSettings', 'Настройки сотрудников'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Users.EmployeeSettings', 'Employee settings'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApi.Index.ApiKeyParam', 'Может передаваться в url-запросе как параметр apikey, например: http://mydomain.ru/api/managers?apikey=xxxxxxxxxxxxxxxxx'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApi.Index.ApiKeyParam', 'Can be passed in a URL request as an apikey parameter, for example: http://mydomain.ru/api/managers?apikey=xxxxxxxxxxxxxxxxxxx'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApi.Index.ApiKeyHTTPParam', 'Или в HTTP-заголовке X-API-KEY. X-API-KEY: xxxxxxxxxxxxxxxxx'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApi.Index.ApiKeyHTTPParam', 'Or in the X-API-KEY HTTP header. X-API-KEY: xxxxxxxxxxxxxxxxx'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApi.Index.Manager', 'Менеджер'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApi.Index.Manager', 'Manager'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApi.Index.BuyerGroup', 'Группа покупателей'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApi.Index.BuyerGroup', 'Buyer group'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApi.Index.BonusSystem', 'Бонусная система'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApi.Index.BonusSystem', 'Bonus system'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApi.Index.BonusSystemGrades', 'Бонусная система. Грейды'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApi.Index.BonusSystemGrades', 'Bonus system. Grades'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApi.Index.GetGrades', 'Получить грейды'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApi.Index.GetGrades', 'Get grades'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApi.Index.SampleAnswer', 'Пример ответа'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApi.Index.SampleAnswer', 'Sample answer'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApi.Index.SampleAnswer.Id', 'Идентификатор'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApi.Index.SampleAnswer.Id', 'ID'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApi.Index.SampleAnswer.Name', 'Название'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApi.Index.SampleAnswer.Name', 'Name'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApi.Index.SampleAnswer.Name.Guest', 'Гостевой'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApi.Index.SampleAnswer.Name.Guest', 'Guest'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApi.Index.SampleAnswer.BonusAccrualPercentage', 'Процент начисления бонусов'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApi.Index.SampleAnswer.BonusAccrualPercentage', 'Bonus accrual percentage'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApi.Index.SampleAnswer.Sort', 'Сортировка'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApi.Index.SampleAnswer.Sort', 'Sort'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApi.Index.SampleAnswer.PurchaseBarrier', 'Порог для перехода на след. гейд'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApi.Index.SampleAnswer.PurchaseBarrier', 'Purchase Barrier'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApi.Index.SampleAnswer.Name.Bronze', 'Бронзовый'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApi.Index.SampleAnswer.Name.Bronze', 'Bronze'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApi.Index.Category', 'Категория'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApi.Index.Category', 'Category'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApi.Index.Settings', 'Настройки'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApi.Index.Settings', 'Settings'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApi.Index.Carousel', 'Карусель'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApi.Index.Carousel', 'Carousel'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApi.Index.Notifications', 'Уведомления'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApi.Index.Notifications', 'Notifications'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApi.Index.CreateOrder.RequestMethod', 'Метод запроса'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApi.Index.CreateOrder.RequestMethod', 'Request method'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApi.Index.CreateOrder.OrderFields', 'Поля Order'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApi.Index.CreateOrder.OrderFields', 'Order fields'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApi.Index.CreateOrder.OrderFields.Necessarily', 'обяз'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApi.Index.CreateOrder.OrderFields.Necessarily', 'Necessarily'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApi.Index.CreateOrder.OrderFields.NotNecessary', 'не обяз'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApi.Index.CreateOrder.OrderFields.NotNecessary', 'not necessary'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApi.Index.CreateOrder.OrderFields.OrderCustomerType', 'Объект типа OrderCustomer'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApi.Index.CreateOrder.OrderFields.OrderCustomerType', 'Object of type OrderCustomer'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApi.Index.CreateOrder.OrderFields.OrderSourceName', 'Название источника заказа'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApi.Index.CreateOrder.OrderFields.OrderSourceName', 'Order source name'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApi.Index.CreateOrder.OrderFields.CurrencyCode', 'Код валюты. Должен совпадать с кодом валюты в магазине.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApi.Index.CreateOrder.OrderFields.CurrencyCode', 'Currency code. Must match the currency code in the store.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApi.Index.CreateOrder.OrderFields.UserComment', 'Комментарий пользователя'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApi.Index.CreateOrder.OrderFields.UserComment', 'User comment'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApi.Index.CreateOrder.OrderFields.AdminComment', 'Комментарий администратора'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApi.Index.CreateOrder.OrderFields.AdminComment', 'Admin comment'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApi.Index.CreateOrder.OrderFields.DeliveryMethodName', 'Название метода доставки'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApi.Index.CreateOrder.OrderFields.DeliveryMethodName', 'Delivery method name'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApi.Index.CreateOrder.OrderFields.PaymentMethodName', 'Название метода оплаты'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApi.Index.CreateOrder.OrderFields.PaymentMethodName', 'Payment method name'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApi.Index.CreateOrder.OrderFields.DeliveryTime', 'string Время доставки. Например, "Как можно скорее", "08:00-20:00|3"'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApi.Index.CreateOrder.OrderFields.DeliveryTime', 'string Delivery time. For example, "ASAP", "08:00-20:00|3"'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApi.Index.CreateOrder.OrderFields.DeliveryDate', 'DateTime? Дата доставки. Например, "2030-02-15 00:00:00.000"'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApi.Index.CreateOrder.OrderFields.DeliveryDate', 'DateTime? Delivery date. For example, "2030-02-15 00:00:00.000"'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApi.Index.CreateOrder.OrderFields.DeliveryCost', 'Стоимость доставки'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApi.Index.CreateOrder.OrderFields.DeliveryCost', 'Delivery cost'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApi.Index.CreateOrder.OrderFields.PaymentCost', 'Стоимость оплаты. Наценка или скида при оплате'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApi.Index.CreateOrder.OrderFields.PaymentCost', 'Payment cost. Markup or discount when paying'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApi.Index.CreateOrder.OrderFields.NumberOfBonuses', 'Кол-во использованных бонусов. У покупателя должна быть бонусная карта. Если её нет, то бонусы не будут списаны. 
                Покупатель будет искаться по CustomerId, Email или Phone.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApi.Index.CreateOrder.OrderFields.NumberOfBonuses', 'Number of bonuses used. The buyer must have a bonus card. If it is not there, then the bonuses will not be written off.
The buyer will be searched by CustomerId, Email or Phone.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApi.Index.CreateOrder.OrderFields.CheckNumberOfBonuses', 'Проверять присланное кол-во бонусов для списания.<br>
                Если стоит true и прислали бонусов для списания больше чем можно, то вернется ошибка "Заказ может быть оплачен бонусами на {p}%. Для этого заказ можно списать {n} бонусов.".<br>
                По умолчанию false, автоматически посчитается и поставится в заказе сколько бонусов доступно для списания, т.е. если BonusCost = 500, а списать можно только 50, то в заказе поставится 50.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApi.Index.CreateOrder.OrderFields.CheckNumberOfBonuses', 'Check the number of bonuses sent for debiting.<br>
If it is set to true and you sent more bonuses to write off than possible, then the error “The order can be paid for with {p} bonuses”. To do this, the order can be written off with {n} bonuses.”.<br>
The default is false, it will automatically calculate and add to the order how many bonuses are available for debiting, i.e. if BonusCost = 500, and only 50 can be written off, then 50 will be included in the order.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApi.Index.CreateOrder.OrderFields.BonusCardNumber', 'Номер бонусной карты. Если номер передается в запросе, то бонусная карта должна уже существовать у покупателя. 
                Покупатель будет искаться по CustomerId, Email или Phone.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApi.Index.CreateOrder.OrderFields.BonusCardNumber', 'Bonus card number. If the number is sent in the request, then the buyer must already have a bonus card.
The buyer will be searched by CustomerId, Email or Phone.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApi.Index.CreateOrder.OrderFields.OrderDiscountPercent', 'Скидка заказа. Процент'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApi.Index.CreateOrder.OrderFields.OrderDiscountPercent', 'Order discount. Percent'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApi.Index.CreateOrder.OrderFields.OrderDiscountValue', 'Скидка заказа. Значение'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApi.Index.CreateOrder.OrderFields.OrderDiscountValue', 'Order discount. Value'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApi.Index.CreateOrder.OrderFields.DeliveryTax', 'Налог на доставку. Название должно совпадать с названием налога в магазине.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApi.Index.CreateOrder.OrderFields.DeliveryTax', 'Delivery tax. The name must match the name of the tax in the store.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApi.Index.CreateOrder.OrderFields.TrackingNumber', 'Номер отслеживания'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApi.Index.CreateOrder.OrderFields.TrackingNumber', 'Tracking number'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApi.Index.CreateOrder.OrderFields.OrderWeight', 'Вес заказа'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApi.Index.CreateOrder.OrderFields.OrderWeight', 'Order weight'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApi.Index.CreateOrder.OrderFields.OrderDimensionsLength', 'Габариты заказа: длина'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApi.Index.CreateOrder.OrderFields.OrderDimensionsLength', 'Order dimensions: length'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApi.Index.CreateOrder.OrderFields.OrderDimensionsWidth', 'Габариты заказа: ширина'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApi.Index.CreateOrder.OrderFields.OrderDimensionsWidth', 'Order dimensions: width'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApi.Index.CreateOrder.OrderFields.OrderDimensionsHeight', 'Габариты заказа: высота'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApi.Index.CreateOrder.OrderFields.OrderDimensionsHeight', 'Order dimensions: height'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApi.Index.CreateOrder.OrderFields.OrderStatus', 'Статус заказа. Название должно совпадать с названием статуса в магазине.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApi.Index.CreateOrder.OrderFields.OrderStatus', 'Order status. The name must match the name of the status in the store.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApi.Index.CreateOrder.OrderFields.ManagerEmail', 'Email менеджера, на которого будет назначен заказ'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApi.Index.CreateOrder.OrderFields.ManagerEmail', 'Email of the manager to whom the order will be assigned'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApi.Index.CreateOrder.OrderFields.PaidOrNot', 'Оплачен заказ или нет. По умолчанию заказа не оплачен.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApi.Index.CreateOrder.OrderFields.PaidOrNot', 'Whether the order has been paid or not. By default, the order is not paid.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApi.Index.CreateOrder.OrderFields.ProductsExist', 'Проверять существуют ли товары в магазине.<br>
                Если стоит true и пришел товар, с артикулом которого нет в магазине, то по вернется ошибка и заказ не будет создан.<br>
                По умолчанию true.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApi.Index.CreateOrder.OrderFields.ProductsExist', 'Check whether products exist in the store.<br>
If it is set to true and a product has arrived whose article number is not in the store, then an error will be returned and the order will not be created.<br>
Defaults to true.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApi.Index.CreateOrder.OrderFields.GoodsAreAvailable', 'Проверять в наличии ли товары в магазине.<br>
                Если стоит true и пришел товар, с артикулом товара, который не в наличии, то по вернется ошибка и заказ не будет создан.<br>
                По умолчанию true.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApi.Index.CreateOrder.OrderFields.GoodsAreAvailable', 'Check whether goods are available in the store.<br>
If it is set to true and the product arrived with a product article that is not in stock, then an error will be returned and the order will not be created.<br>
Defaults to true.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApi.Index.CreateOrder.OrderFields.ArrayOrderItem', 'Массив объектов OrderItem'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApi.Index.CreateOrder.OrderFields.ArrayOrderItem', 'Array of OrderItem objects'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApi.Index.CreateOrder.OrderCustomerFields', 'Поля OrderCustomer'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApi.Index.CreateOrder.OrderCustomerFields', 'OrderCustomer Fields'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApi.Index.CreateOrder.OrderCustomerFields.OneFieldB', 'У OrderCustomer должно быть указано хотя бы одно из полей FirstName, Email, Phone или Organization.</b>'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApi.Index.CreateOrder.OrderCustomerFields.OneFieldB', 'OrderCustomer must have at least one of the FirstName, Email, Phone or Organization fields specified.</b>'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApi.Index.CreateOrder.OrderCustomerFields.OneField', 'Все остальные поля не обязательны для заполнения.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApi.Index.CreateOrder.OrderCustomerFields.OneField', 'All other fields are optional.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApi.Index.CreateOrder.OrderCustomerFields.BuyerID', 'Идентификатор покупателя'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApi.Index.CreateOrder.OrderCustomerFields.BuyerID', 'Buyer ID'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApi.Index.CreateOrder.OrderCustomerFields.BuyersName', 'Имя покупателя'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApi.Index.CreateOrder.OrderCustomerFields.BuyersName', 'Buyers name'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApi.Index.CreateOrder.OrderCustomerFields.BuyersLastName', 'Фамилия покупателя'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApi.Index.CreateOrder.OrderCustomerFields.BuyersLastName', 'Buyers last name'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApi.Index.CreateOrder.OrderCustomerFields.BuyersMiddleName', 'Отчество покупателя'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApi.Index.CreateOrder.OrderCustomerFields.BuyersMiddleName', 'Buyers middle name'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApi.Index.CreateOrder.OrderCustomerFields.OrganizationName', 'Название организации'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApi.Index.CreateOrder.OrderCustomerFields.OrganizationName', 'Organization name'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApi.Index.CreateOrder.OrderCustomerFields.BuyersPhoneNumber', 'Телефон покупателя'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApi.Index.CreateOrder.OrderCustomerFields.BuyersPhoneNumber', 'Buyers phone number'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApi.Index.CreateOrder.OrderCustomerFields.Country', 'Страна'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApi.Index.CreateOrder.OrderCustomerFields.Country', 'Country'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApi.Index.CreateOrder.OrderCustomerFields.Region', 'Регион'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApi.Index.CreateOrder.OrderCustomerFields.Region', 'Region'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApi.Index.CreateOrder.OrderCustomerFields.RegionDistrict', 'Район региона'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApi.Index.CreateOrder.OrderCustomerFields.RegionDistrict', 'District of the region'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApi.Index.CreateOrder.OrderCustomerFields.City', 'Город'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApi.Index.CreateOrder.OrderCustomerFields.City', 'City'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApi.Index.CreateOrder.OrderCustomerFields.Index', 'Индекс'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApi.Index.CreateOrder.OrderCustomerFields.Index', 'Index'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApi.Index.CreateOrder.OrderCustomerFields.Street', 'Улица'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApi.Index.CreateOrder.OrderCustomerFields.Street', 'Street'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApi.Index.CreateOrder.OrderCustomerFields.House', 'Дом'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApi.Index.CreateOrder.OrderCustomerFields.House', 'House'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApi.Index.CreateOrder.OrderCustomerFields.Build', 'Стр./корп.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApi.Index.CreateOrder.OrderCustomerFields.Build', 'Build'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApi.Index.CreateOrder.OrderCustomerFields.Apartment', 'Квартира'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApi.Index.CreateOrder.OrderCustomerFields.Apartment', 'Apartment'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApi.Index.CreateOrder.OrderCustomerFields.Entrance', 'Подъезд'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApi.Index.CreateOrder.OrderCustomerFields.Entrance', 'Entrance'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApi.Index.CreateOrder.OrderCustomerFields.Floor', 'Этаж'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApi.Index.CreateOrder.OrderCustomerFields.Floor', 'Floor'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApi.Index.CreateOrder.OrderCustomerFields.CustomField1', 'Настраиваемое поле 1'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApi.Index.CreateOrder.OrderCustomerFields.CustomField1', 'Custom Field 1'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApi.Index.CreateOrder.OrderCustomerFields.CustomField2', 'Настраиваемое поле 2'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApi.Index.CreateOrder.OrderCustomerFields.CustomField2', 'Custom Field 2'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApi.Index.CreateOrder.OrderCustomerFields.CustomField3', 'Настраиваемое поле 3'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApi.Index.CreateOrder.OrderCustomerFields.CustomField3', 'Custom Field 3'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApi.Index.CreateOrder.OrderItemFields', 'Поля OrderItem'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApi.Index.CreateOrder.OrderItemFields', 'OrderItem Fields'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApi.Index.CreateOrder.OrderItemFields.ProductArticle', 'Артикул товара. Если CheckOrderItemAvailable стоит true, то товар должен присутствовать в магазине.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApi.Index.CreateOrder.OrderItemFields.ProductArticle', 'Product article. If CheckOrderItemAvailable is true, then the product must be present in the store.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApi.Index.CreateOrder.OrderItemFields.ProductName', 'Название товара. Если товар найден по артикулу в магазине, то возьмется название из товара.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApi.Index.CreateOrder.OrderItemFields.ProductName', 'Product name. If a product is found by article number in a store, the name from the product will be taken.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApi.Index.CreateOrder.OrderItemFields.ProductPrice', 'Цена товара. Если не указана, то применится цена товара.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApi.Index.CreateOrder.OrderItemFields.ProductPrice', 'Product price. If not specified, the product price will apply.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApi.Index.CreateOrder.OrderItemFields.QuantityOfGoods', 'Колличество товара. Если не указана, то применится из товара.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApi.Index.CreateOrder.OrderItemFields.QuantityOfGoods', 'Quantity of goods. If not specified, then it is applied from the product.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApi.Index.Order.SampleAnswer', 'Пример ответа'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApi.Index.Order.SampleAnswer', 'Sample answer'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApi.Index.Order.SampleAnswerError', 'Пример ответа с ошибкой'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApi.Index.Order.SampleAnswerError', 'Example of an error response'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApi.Index.Order.SampleRequest', 'Пример запроса'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApi.Index.Order.SampleRequest', 'Sample request'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApi.Index.Customers.BuyersList', 'Список покупателей'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApi.Index.Customers.BuyersList', 'Buyers list'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApi.Index.Customers.ListOfClients', 'Получение списка клиентов, удовлетворяющих заданному фильтру. Результат возвращается постранично.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApi.Index.Customers.ListOfClients', 'Retrieving a list of clients that match a given filter. The result is returned page by page.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApi.Index.Customers.SendsAnSMS', 'Посылает смс-код по номеру телефона для подтверждения. Код действителен в течении 10 мин.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApi.Index.Customers.SendsAnSMS', 'Sends an SMS code to the phone number for confirmation. The code is valid for 10 minutes.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApi.Index.Customers.CheckingSMScode', 'Проверка смс-кода'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApi.Index.Customers.CheckingSMScode', 'Checking SMS code'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApi.Index.Customers.ChecksTheSMSCode', 'Проверяет введенный пользователем смс-код для подтверждения'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApi.Index.Customers.ChecksTheSMSCode', 'Checks the SMS code entered by the user for confirmation'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApi.Index.Customers.BuyersBonusCard', 'Бонусная карта покупателя'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApi.Index.Customers.BuyersBonusCard', 'Buyers bonus card'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApi.Index.BonusSystem.BonusCardByNumber', 'Получение бонусной карты по номеру'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApi.Index.BonusSystem.BonusCardByNumber', 'Receiving a bonus card by number'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApi.Index.BonusSystem.IDCardNumber', '{id} - номер карты'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApi.Index.BonusSystem.IDCardNumber', '{id} - card number'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApi.Index.BonusSystem.BonusCardCreating', 'Создание бонусной карты для покупателя'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApi.Index.BonusSystem.BonusCardCreating', 'Creating a bonus card for a buyer'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApi.Index.BonusSystem.CustomerIdForCard', 'customerId - id покупателя, для которого надо создать бонусную карту'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApi.Index.BonusSystem.CustomerIdForCard', 'customerId - id of the customer for whom a bonus card should be created'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApi.Index.BonusSystem.ReceivingBonuses', 'Получение бонусов'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApi.Index.BonusSystem.ReceivingBonuses', 'Receiving bonuses'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApi.Index.BonusSystem.ListOfBonusesComes', 'В ответ приходит список бонусов.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApi.Index.BonusSystem.ListOfBonusesComes', 'In response, a list of bonuses comes.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApi.Index.BonusSystem.AccrualOfBonuses', 'Начисление бонусов'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApi.Index.BonusSystem.AccrualOfBonuses', 'Accrual of bonuses'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApi.Index.BonusSystem.WriteOffOfBonuses', 'Списание бонусов'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApi.Index.BonusSystem.WriteOffOfBonuses', 'Write-off of bonuses'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApi.Index.BonusSystem.WriteOffOfBonusesRequiredQuantity', 'Списание бонусов на необходимое кол-во'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApi.Index.BonusSystem.WriteOffOfBonusesRequiredQuantity', 'Write-off of bonuses for the required quantity'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApi.Index.BonusSystem.CardTransactionsWithPagination', 'Список транзакций по карте с пагинацией'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApi.Index.BonusSystem.CardTransactionsWithPagination', 'List of card transactions with pagination'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApi.Index.BonusSystemSettings', 'Бонусная система. Настройки'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApi.Index.BonusSystemSettings', 'Bonus system. Settings'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApi.Index.BonusSystemSettings.Settings', 'Настройки бонусной системы'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApi.Index.BonusSystemSettings.Settings', 'Bonus system settings'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApi.Index.BonusSystemSettings.SaveBonusSystemSettings', 'Сохранение настроек бонусной системы'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApi.Index.BonusSystemSettings.SaveBonusSystemSettings', 'Saving bonus system settings'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApi.Index.Category.CategoryList', 'Список категорий'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApi.Index.Category.CategoryList', 'List of categories'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApi.Index.Category.RetrievingCategoryList', 'Получение списка категорий, удовлетворяющих заданному фильтру. Результат возвращается постранично.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApi.Index.Category.RetrievingCategoryList', 'Retrieving a list of categories that match a given filter. The result is returned page by page.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApi.Index.Category.FilterCategory', 'Фильтр. Все поля не обязательны.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApi.Index.Category.FilterCategory', 'Filter. All fields are optional.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApi.Index.Category.GetCategory', 'Получить категорию'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApi.Index.Category.GetCategory', 'Get category'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApi.Index.Category.CreateCategory', 'Создать категорию'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApi.Index.Category.CreateCategory', 'Create category'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApi.Index.Category.ChangeCategory', 'Изменить категорию'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApi.Index.Category.ChangeCategory', 'Change category'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApi.Index.Category.DeleteCategory', 'Удалить категорию'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApi.Index.Category.DeleteCategory', 'Delete category'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApi.Index.CategoryImage', 'Категория. Изображение'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApi.Index.CategoryImage', 'Category. Image'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApi.Index.CategoryImage.AddImage', 'Добавить изображение'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApi.Index.CategoryImage.AddImage', 'Add image'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApi.Index.CategoryImage.AddImage.FileTransfer', 'Передается файл'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApi.Index.CategoryImage.AddImage.FileTransfer', 'File is being transferred'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApi.Index.CategoryImage.AddImageByLink', 'Добавить изображение по ссылке'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApi.Index.CategoryImage.AddImageByLink', 'Add image via link'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApi.Index.CategoryImage.DeleteImage', 'Удалить изображение'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApi.Index.CategoryImage.DeleteImage', 'Delete image'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApi.Index.CategoryMiniImage', 'Категория. Мини-изображение'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApi.Index.CategoryMiniImage', 'Category. Thumbnail'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApi.Index.CategoryMiniImage.AddMiniImage', 'Добавить мини-изображение'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApi.Index.CategoryMiniImage.AddMiniImage', 'Add thumbnail image'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApi.Index.CategoryMiniImage.AddMiniImageLink', 'Добавить мини-изображение по ссылке'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApi.Index.CategoryMiniImage.AddMiniImageLink', 'Add thumbnail image via link'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApi.Index.CategoryMiniImage.DeleteMiniImage', 'Удалить мини-изображение'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApi.Index.CategoryMiniImage.DeleteMiniImage', 'Delete thumbnail'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApi.Index.CategoryMenuIcon', 'Категория. Иконка для меню'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApi.Index.CategoryMenuIcon', 'Category. Icon for menu'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApi.Index.CategoryMenuIcon.AddMenuIcon', 'Добавить иконку для меню'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApi.Index.CategoryMenuIcon.AddMenuIcon', 'Add icon to menu'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApi.Index.CategoryMenuIcon.AddMenuIconLink', 'Добавить иконку для меню по ссылке'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApi.Index.CategoryMenuIcon.AddMenuIconLink', 'Add an icon to the menu by link'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApi.Index.CategoryMenuIcon.DeleteMenuIcon', 'Удалить иконку для меню'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApi.Index.CategoryMenuIcon.DeleteMenuIcon', 'Remove icon for menu'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApi.Index.Settings.SettingsList', 'Список настроек'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApi.Index.Settings.SettingsList', 'List of settings'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApi.Index.Settings.SettingsList.GetByKeys', 'Получение настроек по ключам'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApi.Index.Settings.SettingsList.GetByKeys', 'Getting settings by keys'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApi.Index.Carousel.FilterCarousel', 'Фильтр. Все поля не обязательны.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApi.Index.Carousel.FilterCarousel', 'Filter. All fields are optional.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApi.Index.Carousel.SlidesListCarousel', 'Список слайдов в карусели'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApi.Index.Carousel.SlidesListCarousel', 'List of slides in carousel'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApi.Index.Notifications.SendPush', 'Послать push-уведомление'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApi.Index.Notifications.SendPush', 'Send push notification'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApiAuth.Index.ApiAuth', 'API с авторизацией'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApiAuth.Index.ApiAuth', 'API with authorization'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApiAuth.Index.ApiAuth.TextOne', 'Когда пользователь не авторизован, при использовании методов API в ответе в HTTP-заголовке будет возвращаться X-API-USER-ID.
        Его нужно использовать в последующих запросах к API. Это необходимо чтобы понимать для какого покупателя выполняются методы.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApiAuth.Index.ApiAuth.TextOne', 'When the user is not authorized, when using API methods, the X-API-USER-ID will be returned in the HTTP header in the response. It should be used in subsequent requests to the API. This is necessary to understand for which buyer the methods are executed.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApiAuth.Index.ApiAuth.TextTwo', 'После авторизации (users/signin) в ответе приходит id (userId) и ключ (userKey) для пользователя.
        Данные ключи следует использовать в HTTP-заголовках X-API-USER-ID и X-API-USER-KEY при использовании методов API.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApiAuth.Index.ApiAuth.TextTwo', 'After authorization (users/signin), the response contains the id (userId) and key (userKey) for the user.
These keys should be used in the X-API-USER-ID and X-API-USER-KEY HTTP headers when using API methods.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApiAuth.Index.ApiAuth.TextThree', 'Если используются типы цен со складами, то в url-адресе можно использовать параметр warehouseIds или передавать заголовок X-API-WAREHOUSES. 
        Например: https://mydomain.ru/api/catalog/all?warehouseIds=1,2 или X-API-WAREHOUSES: 1,2 
        Тогда по API вернутся товары с учетом типа цены и выбранного склада. '
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApiAuth.Index.ApiAuth.TextThree', 'If price types with warehouses are used, then the warehouseIds parameter can be used in the URL or the X-API-WAREHOUSES header can be passed.
For example: https://mydomain.ru/api/catalog/all?warehouseIds=1,2 or X-API-WAREHOUSES: 1,2
Then the API will return products taking into account the price type and the selected warehouse.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApiAuth.Index.ApiAuth.ApiKeyTextOne', 'Может передаваться в url-запросе как параметр apikey, например: https://mydomain.ru/api/managers?apikey=xxxxxxxxxxxxxxxxx'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApiAuth.Index.ApiAuth.ApiKeyTextOne', 'Can be passed in the URL request as an apikey parameter, for example: https://mydomain.ru/api/managers?apikey=xxxxxxxxxxxxxxxxxxx'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApiAuth.Index.ApiAuth.ApiKeyTextTwo', 'Или в HTTP-заголовке X-API-KEY. X-API-KEY: xxxxxxxxxxxxxxxxx'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApiAuth.Index.ApiAuth.ApiKeyTextTwo', 'Or in the HTTP header X-API-KEY. X-API-KEY: xxxxxxxxxxxxxxxxx'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApiAuth.Index.ApiAuth.Buyer', 'Покупатель'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApiAuth.Index.ApiAuth.Buyer', 'Buyer'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApiAuth.Index.ApiAuth.Order', 'Заказ'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApiAuth.Index.ApiAuth.Order', 'Order'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApiAuth.Index.ApiAuth.Catalog', 'Каталог'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApiAuth.Index.ApiAuth.Catalog', 'Catalog'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApiAuth.Index.ApiAuth.Search', 'Поиск'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApiAuth.Index.ApiAuth.Search', 'Search'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApiAuth.Index.ApiAuth.Product', 'Товар'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApiAuth.Index.ApiAuth.Product', 'Product'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApiAuth.Index.ApiAuth.Basket', 'Корзина'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApiAuth.Index.ApiAuth.Basket', 'Basket'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApiAuth.Index.ApiAuth.Coupon', 'Купон'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApiAuth.Index.ApiAuth.Coupon', 'Coupon'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApiAuth.Index.ApiAuth.PlacingOrder', 'Оформление заказа'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApiAuth.Index.ApiAuth.PlacingOrder', 'Placing an order'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApiAuth.Index.ApiAuth.ToOrder', 'Под заказ'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApiAuth.Index.ApiAuth.ToOrder', 'To order'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApiAuth.Index.ApiAuth.DeliveryMethods', 'Способы доставки'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApiAuth.Index.ApiAuth.DeliveryMethods', 'Delivery methods'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApiAuth.Index.ApiAuth.Territories', 'Страны, регионы, города'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApiAuth.Index.ApiAuth.Territories', 'Countries, regions, cities'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApiAuth.Index.ApiAuth.BonusSystem', 'Бонусная система'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApiAuth.Index.ApiAuth.BonusSystem', 'Bonus system'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApiAuth.Index.ApiAuth.Settings', 'Настройки'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApiAuth.Index.ApiAuth.Settings', 'Settings'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApiAuth.Index.ApiAuth.StaticBlocks', 'Статические блоки'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApiAuth.Index.ApiAuth.StaticBlocks', 'Static blocks'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApiAuth.Index.ApiAuth.StaticPages', 'Статические страницы'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApiAuth.Index.ApiAuth.StaticPages', 'Static pages'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApiAuth.Index.ApiAuth.Modules', 'Модули'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApiAuth.Index.ApiAuth.Modules', 'Modules'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApiAuth.Index.ApiAuth.Initialization', 'Инициализация'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApiAuth.Index.ApiAuth.Initialization', 'Initialization'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApiAuth.Index.ApiAuth.Leads', 'Лиды'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApiAuth.Index.ApiAuth.Leads', 'Leads'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApiAuth.Index.Init.Initialization', 'Инициализация'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApiAuth.Index.Init.Initialization', 'Initialization'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApiAuth.Index.Init.CurrentBuyer', 'Возвращается текущий покупатель, валюта, настройки, локация.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApiAuth.Index.Init.CurrentBuyer', 'Returns current buyer, currency, settings, location.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApiAuth.Index.Init.AllFields', 'Все поля необязательны для заполнения'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApiAuth.Index.Init.AllFields', 'All fields are optional'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApiAuth.Index.Users.EmailAuthorization', 'Авторизация по эл. почте'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApiAuth.Index.Users.EmailAuthorization', 'Authorization by email'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApiAuth.Index.OrdersMe.ListCustomerOrder', 'Список заказов покупателя'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApiAuth.Index.OrdersMe.ListCustomerOrder', 'List of customer orders'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApiAuth.Index.OrdersMe.TextOne', 'X-API-USER-ID и X-API-USER-KEY получены после авторизации (users/signin)'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApiAuth.Index.OrdersMe.TextOne', 'X-API-USER-ID and X-API-USER-KEY are obtained after authorization (users/signin)'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApiAuth.Index.OrdersMe.TextTwo', 'Если пользователь не авторизован, то вернется пустой список заказов'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApiAuth.Index.OrdersMe.TextTwo', 'If the user is not authorized, an empty list of orders will be returned.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApiAuth.Index.Users.SMSCodeAuthorization', 'Авторизация (или регистрация) по sms-коду. Отсылка sms-кода по номеру телефона.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApiAuth.Index.Users.SMSCodeAuthorization', 'Authorization (or registration) by SMS code. Sending SMS code by phone number.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApiAuth.Index.Users.SMSCodeAuthorization.TwoRequests', 'Авторизация по sms-коду делается двумя запросами:'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApiAuth.Index.Users.SMSCodeAuthorization.TwoRequests', 'Authorization by SMS code is done with two requests:'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApiAuth.Index.Users.SMSCodeAuthorization.FirstRequest', '1) /api/users/signInByPhone - пользователю отсылается sms-код по номеру телефона. Его нужно ввести в течении 10 мин.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApiAuth.Index.Users.SMSCodeAuthorization.FirstRequest', '1) /api/users/signInByPhone - the user is sent an SMS code by phone number. It must be entered within 10 minutes.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApiAuth.Index.Users.SMSCodeAuthorization.SecondRequest', '2) /api/users/signInByPhoneConfirmCode - проверяется введенный пользователем код, если верен, то возвращается авторизованный покупатель, userKey, userId'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApiAuth.Index.Users.SMSCodeAuthorization.SecondRequest', '2) /api/users/signInByPhoneConfirmCode - the code entered by the user is checked, if it is correct, then the authorized buyer, userKey, userId are returned'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApiAuth.Index.Users.SMSAuthorizationChecking', 'Авторизация (или регистрация) по sms-коду. Проверка введенного кода и авторизация.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApiAuth.Index.Users.SMSAuthorizationChecking', 'Authorization (or registration) by SMS code. Checking the entered code and authorization.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApiAuth.Index.Users.GetCurrentBuyer', 'Получить текущего покупателя'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApiAuth.Index.Users.GetCurrentBuyer', 'Get current buyer'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApiAuth.Index.Users.ChangeCurrentBuyer', 'Изменить текущего покупателя'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApiAuth.Index.Users.ChangeCurrentBuyer', 'Change current buyer'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApiAuth.Index.Users.CheckSMSCode', 'Проверка смс-кода'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApiAuth.Index.Users.CheckSMSCode', 'SMS code verification'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApiAuth.Index.Users.CheckSMSCodeByUser', 'Проверяет введенный пользователем смс-код для подтверждения'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApiAuth.Index.Users.CheckSMSCodeByUser', 'Checks the SMS code entered by the user for confirmation'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApiAuth.Index.Users.BuyersBonusCard', 'Бонусная карта покупателя'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApiAuth.Index.Users.BuyersBonusCard', 'Buyers bonus card'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApiAuth.Index.Users.AdditionalBuyerFields', 'Получить доп. поля покупателя'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApiAuth.Index.Users.AdditionalBuyerFields', 'Get additional buyer fields'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApiAuth.Index.Users.DeleteUserAccount', 'Удалить аккаунт пользователя'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApiAuth.Index.Users.DeleteUserAccount', 'Delete user account'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApiAuth.Index.Users.UpdateFirebaseCloudMessaging', 'Обновить Firebase Cloud Messaging token'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApiAuth.Index.Users.UpdateFirebaseCloudMessaging', 'Update Firebase Cloud Messaging token'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApiAuth.Index.Users.BuyersContacts', 'Контакты покупателя'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApiAuth.Index.Users.BuyersContacts', 'Buyers contacts'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApiAuth.Index.Users.GetBuyersContacts', 'Получить список контактов пользователя'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApiAuth.Index.Users.GetBuyersContacts', 'Get users contact list'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApiAuth.Index.Users.AddNewContact', 'Добавить новый контакт для пользователя'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApiAuth.Index.Users.AddNewContact', 'Add a new contact for a user'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApiAuth.Index.Users.UpdateNewContact', 'Обновить контакт для пользователя'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApiAuth.Index.Users.UpdateNewContact', 'Update contact for user'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApiAuth.Index.Users.DeleteNewContact', 'Удалить контакт для пользователя'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApiAuth.Index.Users.DeleteNewContact', 'Delete contact for user'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApiAuth.Index.Users.UserStatistics', 'Статистика по пользователю'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApiAuth.Index.Users.UserStatistics', 'User statistics'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApiAuth.Index.OrdersMe.ReceiveOrder', 'Получить заказ покупателя'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApiAuth.Index.OrdersMe.ReceiveOrder', 'Receive customer order'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApiAuth.Index.OrdersMe.ReceiveOrder.TextOne', 'Вместо {id} подставляется идентификатор заказа'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApiAuth.Index.OrdersMe.ReceiveOrder.TextOne', 'Instead of {id} the order ID is substituted'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApiAuth.Index.OrdersMe.ReceiveOrder.TextTwo', 'X-API-USER-ID и X-API-USER-KEY получены после авторизации (users/signin)'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApiAuth.Index.OrdersMe.ReceiveOrder.TextTwo', 'X-API-USER-ID and X-API-USER-KEY are obtained after authorization (users/signin)'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApiAuth.Index.OrdersMe.OrderRate', 'Оценить заказ'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApiAuth.Index.OrdersMe.OrderRate', 'Rate the order'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApiAuth.Index.OrdersMe.OrderCancel', 'Отменить заказ'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApiAuth.Index.OrdersMe.OrderCancel', 'Cancel order'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApiAuth.Index.Catalog.GetCatalog', 'Получить каталог'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApiAuth.Index.Catalog.GetCatalog', 'Get the catalog'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApiAuth.Index.Catalog.TextOne', 'X-API-USER-ID и X-API-USER-KEY получены после авторизации (users/signin),
        если пользователь не авторизован, то присылать X-API-USER-KEY не обязательно'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApiAuth.Index.Catalog.TextOne', 'X-API-USER-ID and X-API-USER-KEY are received after authorization (users/signin), if the user is not authorized, then it is not necessary to send X-API-USER-KEY'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApiAuth.Index.Catalog.GetOnePageCatalog', 'Получить одностраничный каталог'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApiAuth.Index.Catalog.GetOnePageCatalog', 'Get a one-page catalog'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApiAuth.Index.Catalog.GetOnePageCatalog.ReturnCatList', 'Возвращается список корневых категорий с товарами в них'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApiAuth.Index.Catalog.GetOnePageCatalog.ReturnCatList', 'Returns a list of root categories with products in them'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApiAuth.Index.Catalog.GetCatalogFilter', 'Получить фильтр каталога'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApiAuth.Index.Catalog.GetCatalogFilter', 'Get Catalog Filter'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApiAuth.Index.Catalog.GetQuantityByFilter', 'Получить кол-во товаров по фильтру'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApiAuth.Index.Catalog.GetQuantityByFilter', 'Get quantity of products by filter'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApiAuth.Index.Search.SearchByProdAndCat', 'Поиск по товарам и категориям'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApiAuth.Index.Search.SearchByProdAndCat', 'Search by products and categories'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApiAuth.Index.Search.GetFilterForSearch', 'Получить фильтр для поиска'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApiAuth.Index.Search.GetFilterForSearch', 'Get filter for search'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApiAuth.Index.Products.GetProduct', 'Получить товар'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApiAuth.Index.Products.GetProduct', 'Get product'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApiAuth.Index.Search.SearchAutocompleteByProdAndCat', 'Поиск автозаполнение по товарам и категориям'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApiAuth.Index.Search.SearchAutocompleteByProdAndCat', 'Search autocomplete by products and categories'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApiAuth.Index.Products.CalculatePrice', 'Расчитать цену товара'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApiAuth.Index.Products.CalculatePrice', 'Calculate the price of the product'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApiAuth.Index.Products.GetProductSpecifications', 'Получить характеристики товара'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApiAuth.Index.Products.GetProductSpecifications', 'Get product specifications'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApiAuth.Index.Products.GetProductReviews', 'Получить отзывы товара'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApiAuth.Index.Products.GetProductReviews', 'Get product reviews'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApiAuth.Index.Products.AddReview', 'Добавить отзыв'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApiAuth.Index.Products.AddReview', 'Add Reviews'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApiAuth.Index.Products.GetCrossUpSell', 'Получить cross-sell, up-sell товары'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApiAuth.Index.Products.GetCrossUpSell', 'Get cross-sell, up-sell products'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApiAuth.Index.Products.GetGifts', 'Получить подарки для товара'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApiAuth.Index.Products.GetGifts', 'Get gifts for the product'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApiAuth.Index.Products.GetWarehousesList', 'Получить список складов с наличием'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApiAuth.Index.Products.GetWarehousesList', 'Get a list of warehouses with availability'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApiAuth.Index.Products.GetListOfPrice', 'Получить список тип цен в зависимости от кол-ва'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApiAuth.Index.Products.GetListOfPrice', 'Get a list of price types depending on quantity'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApiAuth.Index.Basket.GetABasket', 'Получить корзину на основе переданных товарах'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApiAuth.Index.Basket.GetABasket', 'Get a basket based on the items transferred'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApiAuth.Index.Basket.GetCurrentBasket', 'Получить текущую корзину'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApiAuth.Index.Basket.GetCurrentBasket', 'Get current cart'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApiAuth.Index.Coupons.ApplyCoupon', 'Применить купон'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApiAuth.Index.Coupons.ApplyCoupon', 'Apply coupon'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApiAuth.Index.Coupons.DeleteCoupon', 'Удалить купон'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApiAuth.Index.Coupons.DeleteCoupon', 'Delete coupon'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApiAuth.Index.Checkout.DataPlaceOrder', 'Передать данные для оформления заказа'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApiAuth.Index.Checkout.DataPlaceOrder', 'Submit data to place an order'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApiAuth.Index.PreOrder.GetCustomSettings', 'Получить настройки для раздела "под заказ"'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApiAuth.Index.PreOrder.GetCustomSettings', 'Get settings for the "custom" section'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApiAuth.Index.PreOrder.MakeToOrder', 'Оформить под заказ'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApiAuth.Index.PreOrder.MakeToOrder', 'Make to order'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApiAuth.Index.Deliveries.GetDeliveryTypes', 'Получить типы доставок'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApiAuth.Index.Deliveries.GetDeliveryTypes', 'Get delivery types'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApiAuth.Index.Deliveries.CheckAdressDeliveryZone', 'Проверить адрес на возможность доставки доставками с типом "Доставки по зонам"'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApiAuth.Index.Deliveries.CheckAdressDeliveryZone', 'Check the address for the possibility of delivery by deliveries with the type "Deliveries by zones"'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApiAuth.Index.Deliveries.ShippingCalculation', 'Расчет доставки'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApiAuth.Index.Deliveries.ShippingCalculation', 'Shipping Calculation'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApiAuth.Index.Deliveries.GetPickupPoints', 'Получить пункты выдачи'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApiAuth.Index.Deliveries.GetPickupPoints', 'Get pick-up points'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApiAuth.Index.Locations.MajorCountriesCitiesList', 'Список основных стран и городов'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApiAuth.Index.Locations.MajorCountriesCitiesList', 'List of major countries and cities'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApiAuth.Index.Locations.GetCityByNameOrZip', 'Получить город по названию или индексу'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApiAuth.Index.Locations.GetCityByNameOrZip', 'Get city by name or zip code'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApiAuth.Index.Locations.CountriesList', 'Список стран'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApiAuth.Index.Locations.CountriesList', 'List of countries'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApiAuth.Index.BonusSystem.CreateBonusCard', 'Создание бонусной карты для покупателя'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApiAuth.Index.BonusSystem.CreateBonusCard', 'Creating a bonus card for the buyer'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApiAuth.Index.ApiAuth.Featured', 'Избранное'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApiAuth.Index.ApiAuth.Featured', 'Featured'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApiAuth.Index.WishList.GetFavoritesProducts', 'Получить товары из избранного'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApiAuth.Index.WishList.GetFavoritesProducts', 'Get products from favorites'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApiAuth.Index.WishList.AddFavoritesProducts', 'Добавить товар в избранное'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApiAuth.Index.WishList.AddFavoritesProducts', 'Add product to favorites'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApiAuth.Index.WishList.RemoveFavoritesProducts', 'Удалить товар из избранного'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApiAuth.Index.WishList.RemoveFavoritesProducts', 'Remove item from favorites'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApiAuth.Index.Settings.DadataSettings', 'Настройки Dadata'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApiAuth.Index.Settings.DadataSettings', 'Dadata settings'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApiAuth.Index.StaticBlock.GetStaticBlocks', 'Получить статические блоки'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApiAuth.Index.StaticBlock.GetStaticBlocks', 'Get static blocks'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApiAuth.Index.StaticPage.GetStaticPages', 'Получить статические страницы'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApiAuth.Index.StaticPage.GetStaticPages', 'Get static pages'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApiAuth.Index.StaticPage.GetStaticPage', 'Получить статическую страницу'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApiAuth.Index.StaticPage.GetStaticPage', 'Get static page'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApiAuth.Index.Modules.GetBlockModule', 'Получить блок срендеренный модулем'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApiAuth.Index.Modules.GetBlockModule', 'Get block rendered by module'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApiAuth.Index.Initv2.Initialization', 'Инициализация'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApiAuth.Index.Initv2.Initialization', 'Initialization'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApiAuth.Index.Initv2.ReturnsMeanings', 'Возвращается текущий покупатель, валюта, настройки, локация.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApiAuth.Index.Initv2.ReturnsMeanings', 'Returns current buyer, currency, settings, location.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApiAuth.Index.Leadv2.LeadGeneration', 'Создание лида'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApiAuth.Index.Leadv2.LeadGeneration', 'Lead generation'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApiAuth.Index.Deliveriesv2.GetDeliveries', 'Получить доставки'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApiAuth.Index.Deliveriesv2.GetDeliveries', 'Get deliveries'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.SettingsApiAuth.Index.Webhook.Event', 'Событие'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.SettingsApiAuth.Index.Webhook.Event', 'Event'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.SettingsApiAuth.Index.Webhook.NoWebhooks', 'Нет Webhooks'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.SettingsApiAuth.Index.Webhook.NoWebhooks', 'No Webhooks'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApiAuth.Index.ApiAuth.ExampleRequest', 'Пример запроса'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApiAuth.Index.ApiAuth.ExampleRequest', 'Example request'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApiAuth.Index.ApiAuth.ExampleAnswer', 'Пример ответа'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApiAuth.Index.ApiAuth.ExampleAnswer', 'Example responce'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApiAuth.Index.ApiAuth.ExampleAnswerError', 'Пример ответа с ошибкой'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApiAuth.Index.ApiAuth.ExampleAnswerError', 'Example of an error response'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApiAuth.Index.ApiAuth.AllFields', 'Фильтр. Все поля необязательны для заполнения'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApiAuth.Index.ApiAuth.AllFields', 'Filter. All fields are optional'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Commonpage.Leads.LeadsList', 'Инструкция. Списки лидов'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Commonpage.Leads.LeadsList', 'Instructions. Lead lists'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.SettingsCrm.EditSalesFunnel.Base', 'Основное'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.SettingsCrm.EditSalesFunnel.Base', 'Main'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.SettingsCrm.EditSalesFunnel.AdditionalFields', 'Дополнительные поля лида'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.SettingsCrm.EditSalesFunnel.AdditionalFields', 'Additional lead fields'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.SettingsCrm.EditSalesFunnel.ActionLeadClosure', 'Действие при успешном закрытии лида'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.SettingsCrm.EditSalesFunnel.ActionLeadClosure', 'Action upon successful lead closure'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.SettingsCrm.EditSalesFunnel.AutoLeadComplete', 'Автоматически завершать лид'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.SettingsCrm.EditSalesFunnel.AutoLeadComplete', 'Automatically complete lead'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.SettingsCrm.EditSalesFunnel.OrderMustContainAll', 'Заказ должен содержать все следующие товары'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.SettingsCrm.EditSalesFunnel.OrderMustContainAll', 'The order must contain all of the following items'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.SettingsCrm.EditSalesFunnel.SelectProducts', 'Выбрать товары'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.SettingsCrm.EditSalesFunnel.SelectProducts', 'Select products'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.SettingsCrm.EditSalesFunnel.OrderMustContainAny', 'Заказ должен содержать товары из любой категории из списка'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.SettingsCrm.EditSalesFunnel.OrderMustContainAny', 'The order must contain products from any category from the list'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.SettingsCrm.EditSalesFunnel.SelectCategories', 'Выбрать категории'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.SettingsCrm.EditSalesFunnel.SelectCategories', 'Select categories'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.SettingsCrm.EditSalesFunnel.Managers', 'Менеджеры'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.SettingsCrm.EditSalesFunnel.Managers', 'Managers'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.SettingsCrm.EditSalesFunnel.ManagersInfoOne', 'Менеджеры, которые могут работать со списком лидов и получать уведомления о лидах.<br />
                                        Если никто не указан, то push-уведомления получают администраторы и модераторы с правами, email - в
                                        соответствии с'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.SettingsCrm.EditSalesFunnel.ManagersInfoOne', 'Managers who can work with the list of leads and receive notifications about leads.<br />
If no one is specified, then push notifications are received by administrators and moderators with rights, email - in
accordance with'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.SettingsCrm.EditSalesFunnel.ManagersInfoTwo', 'настройками'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.SettingsCrm.EditSalesFunnel.ManagersInfoTwo', 'settings'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Commonpage.SettingsAPI.Settings', 'Настройки'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Commonpage.SettingsAPI.Settings', 'Settings'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.SettingsCrm.EditSalesFunnel.NoSendNotificationsCreate', 'Не отправлять уведомления при создании нового лида'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.SettingsCrm.EditSalesFunnel.NoSendNotificationsCreate', 'Do not send notifications when a new lead is created'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.SettingsCrm.EditSalesFunnel.NoSendNotificationsChange', 'Не отправлять уведомления при изменении лида'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.SettingsCrm.EditSalesFunnel.NoSendNotificationsChange', 'Do not send notifications when a lead changes'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.SettingsCrm.EditSalesFunnel.Activity', 'Активность'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.SettingsCrm.EditSalesFunnel.Activity', 'Activity'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Commonpage.Tasks.Instructions', 'Инструкция. Как включить раздел "Задачи"'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Commonpage.Tasks.Instructions', 'Instructions. How to enable the "Tasks" section'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Commonpage.Tasks.Settings', 'Настройки'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Commonpage.Tasks.Settings', 'Settings'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Tasks.BizProcesses.Instruction', 'Инструкция.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Tasks.BizProcesses.Instruction', 'Instruction.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Tasks.BizProcesses', 'Business processes'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Tasks.AutoTaskAssignment', 'Automatic task assignment'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Tasks.BizProcesses.Empty', 'Rules are not set'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Tasks.BizProcesses.AddRule', 'Add rule'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Taskgroups.AddEdit.Roles', 'роли'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Taskgroups.AddEdit.Roles', 'roles'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Taskgroups.AddEdit.Roles.TextOne', 'Сотрудники с выбранными ролями могут работать с задачами проекта. Если ни одна роль не выбрана, то проект доступен всем.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Taskgroups.AddEdit.Roles.TextOne', 'Employees with selected roles can work with project tasks. If no role is selected, the project is available to everyone.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Taskgroups.AddEdit.Roles.TextTwo', 'Подробнее:'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Taskgroups.AddEdit.Roles.TextTwo', 'Read more:'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Taskgroups.AddEdit.ProjectCreating', 'Создание проекта'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Taskgroups.AddEdit.ProjectCreating', 'Creating a project'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Taskgroups.AddEdit.Employees', 'сотрудники'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Taskgroups.AddEdit.Employees', 'employees'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Taskgroups.AddEdit.Employees.TextOne', 'Выбранные сотрудники могут работать с задачами проекта.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Taskgroups.AddEdit.Employees.TextOne', 'Selected employees can work on project tasks.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Taskgroups.AddEdit.Employees.TextTwo', 'Подробнее:'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Taskgroups.AddEdit.Employees.TextTwo', 'Read more:'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Taskgroups.AddEdit.Observers.TextOne', 'Список сотрудников, которые будут получать уведомления о задачах'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Taskgroups.AddEdit.Observers.TextOne', 'List of employees who will receive task notifications'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Taskgroups.AddEdit.Observers.TextTwo', 'Подробнее:'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Taskgroups.AddEdit.Observers.TextTwo', 'Read more:'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Taskgroups.AddEdit.ManagerTasks', 'Какие задачи может видеть менеджер'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Taskgroups.AddEdit.ManagerTasks', 'What tasks can a manager see?'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Taskgroups.AddEdit.ManagerTasks.TextOne', 'Какие задачи будет видеть и сможет редактировать менеджер (должны быть активны права модератора на задачи).'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Taskgroups.AddEdit.ManagerTasks.TextOne', 'Which tasks the manager will be able to see and edit (moderator rights to tasks must be active).'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Taskgroups.AddEdit.ManagerTasks.TextTwo', 'Все задачи - доступны для редактирования все задачи.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Taskgroups.AddEdit.ManagerTasks.TextTwo', 'All tasks - all tasks are available for editing.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Taskgroups.AddEdit.ManagerTasks.TextThree', 'Назначенные задачи - только задачи, которые назначили менеджеру. Другие задачи не доступны для просмотра и редактирования.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Taskgroups.AddEdit.ManagerTasks.TextThree', 'Assigned tasks - only tasks that have been assigned to the manager. Other tasks are not available for viewing or editing.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Taskgroups.AddEdit.ManagerTasks.TextFour', 'Назначенные и свободные задачи - задачи, которые назначили менеджеру, и задачи, на которые еще никого не назначили.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Taskgroups.AddEdit.ManagerTasks.TextFour', 'Assigned and Free Tasks - tasks that have been assigned to a manager and tasks that have not yet been assigned to anyone.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Taskgroups.AddEdit.PrivateComments', 'Приватные комментарии'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Taskgroups.AddEdit.PrivateComments', 'Private comments'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Taskgroups.AddEdit.PrivateComments.TextOne', 'Сотрудники могут видеть только свои комментарии и комментарии администратора. Администратор видит все комментарии'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Taskgroups.AddEdit.PrivateComments.TextOne', 'Employees can only see their own comments and the administrators comments. The administrator sees all comments.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Taskgroups.AddEdit.PrivateComments.TextTwo', 'Подробнее:'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Taskgroups.AddEdit.PrivateComments.TextTwo', 'Read more:'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Taskgroups.AddEdit.NotThisProjectTasks', 'Нельзя завершать задачи в этот проект'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Taskgroups.AddEdit.NotThisProjectTasks', 'Tasks in this project cannot be completed.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Taskgroups.AddEdit.NotThisProjectTasks.TextOne', 'Если данная опция включена, то в данный проект нельзя завершать задачи. '
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Taskgroups.AddEdit.NotThisProjectTasks.TextOne', 'If this option is enabled, tasks cannot be completed in this project.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Taskgroups.AddEdit.NotThisProjectTasks.TextTwo', 'При завершении будет предложено выбрать проект в который определить завершаемую задачу.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Taskgroups.AddEdit.NotThisProjectTasks.TextTwo', 'Upon completion, you will be prompted to select a project in which to define the task being completed.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.TemplatesDocx.Templates.Instructions', 'Инструкция. Торг-12, УПД, чек, счет-фактура'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.TemplatesDocx.Templates.Instructions', 'Instruction. Torg-12, UPD, check, invoice'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.TemplatesDocx.Templates.Settings', 'Настройки'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.TemplatesDocx.Templates.Settings', 'Settings'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.TemplatesDocx.Templates.AddTemplate', 'Добавить шаблон'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.TemplatesDocx.Templates.AddTemplate', 'Add template'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Settings.TemplatesDocx.Templates.DocumentTemplate', 'Шаблон документа'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Settings.TemplatesDocx.Templates.DocumentTemplate', 'Document template'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Settings.TemplatesDocx.Templates.Name', 'Название'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Settings.TemplatesDocx.Templates.Name', 'Name'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Settings.TemplatesDocx.Templates.Type', 'Тип'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Settings.TemplatesDocx.Templates.Type', 'Type'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Settings.TemplatesDocx.Templates.ViewDescription', 'Посмотреть описание шаблона'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Settings.TemplatesDocx.Templates.ViewDescription', 'View template description'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Settings.TemplatesDocx.Templates.TemplateFile', 'Файл шаблона'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Settings.TemplatesDocx.Templates.TemplateFile', 'Template file'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Settings.TemplatesDocx.Templates.ReadMore', 'Подробнее:'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Settings.TemplatesDocx.Templates.ReadMore', 'Read more:'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Settings.TemplatesDocx.Templates.DocumentTemplates', 'Шаблоны докуметов'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Settings.TemplatesDocx.Templates.DocumentTemplates', 'Document templates'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Settings.TemplatesDocx.Templates.SortOrder', 'Порядок сортировки'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Settings.TemplatesDocx.Templates.SortOrder', 'Sort order'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Settings.TemplatesDocx.Templates.ExpertMode', 'Режим эксперта'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Settings.TemplatesDocx.Templates.ExpertMode', 'Expert mode'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Settings.TemplatesDocx.Templates.ShowErrors', 'Выводить ошибки в шаблоне'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Settings.TemplatesDocx.Templates.ShowErrors', 'Show errors in template'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Settings.TemplatesDocx.Templates.Create', 'Добавить'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Settings.TemplatesDocx.Templates.Create', 'Create'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Settings.TemplatesDocx.Templates.Save', 'Сохранить'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Settings.TemplatesDocx.Templates.Save', 'Save'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Settings.TemplatesDocx.Templates.Cancel', 'Отмена'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Settings.TemplatesDocx.Templates.Cancel', 'Cancel'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Settings.TemplatesDocx.Templates.AttachFile', 'Прикрепить файл'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Settings.TemplatesDocx.Templates.AttachFile', 'Attach a file'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Settings.TemplatesDocx.Templates.AttachNewFile', 'Прикрепить новый файл'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Settings.TemplatesDocx.Templates.AttachNewFile', 'Attach a new file'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Settings.TemplatesDocx.Templates.DocumentTemplateFile', 'Расширения файла шаблона документа'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Settings.TemplatesDocx.Templates.DocumentTemplateFile', 'Document Template File Extensions'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.FileHelpers.FilesHelpText.Common', 'Allowed file extensions: {0}'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Commonpage.Seo.Settings', 'Настройки'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Commonpage.Seo.Settings', 'Settings'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.SeoSettings.RobotsTxtHeader.Instruction', 'Инструкция. Что такое'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.SeoSettings.RobotsTxtHeader.Instruction', 'Instructions. What is it'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.GoogleAnalytics.Instruction', 'Инструкция. Подключение к Google Analytics API'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.GoogleAnalytics.Instruction', 'Instructions. Connecting to Google Analytics API'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.GoogleAnalytics.TransferringOrders', 'Передача заказов в Ecommerce Analytics'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.GoogleAnalytics.TransferringOrders', 'Transferring orders to Ecommerce Analytics'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.OpenGraph.Instruction', 'Инструкция.Протокол Open Graph'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.OpenGraph.Instruction', 'Instructions: Open Graph Protocol'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.OpenGraph.CommentModerationSettings', 'Настройки модерации комментариев'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.OpenGraph.CommentModerationSettings', 'Comment moderation settings'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.OpenGraph.TextOne', 'ID пользователей/администраторов приложения, если несколько администраторов, указывать можно через запятую.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.OpenGraph.TextOne', 'The IDs of the application users/administrators; if there are several administrators, they can be specified separated by commas.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.OpenGraph.TextTwo', 'Данное перечисление необходимо, если у Вас подключен со стороны facebook плагин комментариев и необходимо модерировать комментарии.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.OpenGraph.TextTwo', 'This listing is necessary if you have a comment plugin connected from Facebook and need to moderate comments.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.OpenGraph.ReadMore', 'Подробнее:'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.OpenGraph.ReadMore', 'Read more:'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.OpenGraph.FacebookPlugin', 'Плагин комментариев от Facebook'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.OpenGraph.FacebookPlugin', 'Facebook Comments Plugin'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.System.301Redirects.Instruction', 'Инструкция. 301 редирект'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.System.301Redirects.Instruction', 'Instructions. 301 redirect'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.System.Error404.Instruction', 'Инструкция. 404 страницы'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.System.Error404.Instruction', 'Instruction. 404 pages'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Home.Menu.Reviews.InstructionReviews', 'Инструкция. Как включить добавление отзывов к товарам'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Home.Menu.Reviews.InstructionReviews', 'Instructions: How to enable adding reviews to products'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Import.ImportProducts.Import.Instruction', 'Инструкция. Импорт товаров через файл CSV в новом формате (2.0)'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Import.ImportProducts.Import.Instruction', 'Instructions. Importing products via CSV file in the new format (2.0)'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Category.Index.NewCategory.Instruction', 'Инструкция. Добавление / удаление категории'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Category.Index.NewCategory.Instruction', 'Instructions. Adding/removing a category'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Catalog.Index.Instruction', 'Инструкция. Добавление / удаление категории'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Catalog.Index.Instruction', 'Instructions. Adding/removing a category'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Partials.CategoriesBlock.SelectCategories', 'Выбрать все категории'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Partials.CategoriesBlock.SelectCategories', 'Select all categories'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Partials.CategoriesBlock.CategorySelected', 'категория выбрана'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Partials.CategoriesBlock.CategorySelected', 'category selected'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Partials.CategoriesBlock.CategoriesSelected', 'категории выбрано'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Partials.CategoriesBlock.CategoriesSelected', 'categories selected'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Catalog.PriceRegulation.Instruction', 'Инструкция. Регулирование цен'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Catalog.PriceRegulation.Instruction', 'Instruction. Price regulation'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Catalog.CategoryDiscountRegulation.Instruction', 'Инструкция. Регулирование скидок по категориям'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Catalog.CategoryDiscountRegulation.Instruction', 'Instructions. Adjusting discounts by category'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Catalog.BrandDiscountRegulation.Instruction', 'Инструкция. Регулирование скидок по брендам'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Catalog.BrandDiscountRegulation.Instruction', 'Instructions. Adjusting discounts by brands'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Catalog.BrandDiscountRegulation.ForManufacturer', 'Для производителя'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Catalog.BrandDiscountRegulation.ForManufacturer', 'For the manufacturer'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Catalog.BrandDiscountRegulation.NotSelected', 'Не выбрано'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Catalog.BrandDiscountRegulation.NotSelected', 'Not selected'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Catalog.BrandDiscountRegulation.Discount', 'Cкидка'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Catalog.BrandDiscountRegulation.Discount', 'Discount'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Catalog.BrandDiscountRegulation.ResetDiscount', 'Чтоб сбросить установленную скидку для каталога, категории или производителя, достаточно выбрать их и применить нулевую скидку'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Catalog.BrandDiscountRegulation.ResetDiscount', 'To reset the set discount for a catalog, category or manufacturer, simply select them and apply a zero discount'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.PriceRegulation.GreaterZero', 'Укажите значение больше 0'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.PriceRegulation.GreaterZero', 'Please specify a value greater than 0'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.PriceRegulation.ZeroToHundred', 'Укажите значение в диапазоне от 0 до 100'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.PriceRegulation.ZeroToHundred', 'Please enter a value between 0 and 100'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.PriceRegulation.SelectManufacturer', 'Выберете производителя'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.PriceRegulation.SelectManufacturer', 'Select manufacturer'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.AddBrand.Save', 'Сохранить'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.AddBrand.Save', 'Save'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.AddEditPriceRule.PriceType', 'Тип цен'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.AddEditPriceRule.PriceType', 'Price type'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.AddEditPriceRule.Name', 'Название'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.AddEditPriceRule.Name', 'Name'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.AddEditPriceRule.NumberProducts', 'Количество товаров'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.AddEditPriceRule.NumberProducts', 'Number of products'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.AddEditPriceRule.BuyerGroup', 'Группа покупателей'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.AddEditPriceRule.BuyerGroup', 'Buyer group'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.AddEditPriceRule.PaymentMethod', 'Метод оплаты'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.AddEditPriceRule.PaymentMethod', 'Payment method'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.AddEditPriceRule.DeliveryMethod', 'Метод доставки'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.AddEditPriceRule.DeliveryMethod', 'Delivery method'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.AddEditPriceRule.Warehouse', 'Склад'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.AddEditPriceRule.Warehouse', 'Warehouse'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.AddEditPriceRule.TakeDiscounts', 'Учитывать скидки'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.AddEditPriceRule.TakeDiscounts', 'Take discounts into account'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.AddEditPriceRule.OptionNotActive', 'Если опция не активна, то не будут применятся скидки товара, группы покупателей и др.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.AddEditPriceRule.OptionNotActive', 'If the option is not active, then discounts for products, customer groups, etc. will not be applied.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.AddEditPriceRule.Activity', 'Активность'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.AddEditPriceRule.Activity', 'Activity'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.AddEditPriceRule.Sort', 'Сортировка'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.AddEditPriceRule.Sort', 'Sort'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Catalog.PriceTypes', 'Типы цен'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Catalog.PriceTypes', 'Price types'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Catalog.PriceTypes.Add', 'Добавить'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Catalog.PriceTypes.Add', 'Add'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Catalog.PhotoCategory.Add', 'Добавить'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Catalog.PhotoCategory.Add', 'Add'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.AddEditPhotoCategory', 'Добавление категории фотографий'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.AddEditPhotoCategory', 'Adding a photo category'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.AddEditPhotoCategory.Activity', 'Активность'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.AddEditPhotoCategory.Activity', 'Activity'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.AddEditPhotoCategory.Name', 'Название'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.AddEditPhotoCategory.Name', 'Name'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.AddEditPhotoCategory.Sort', 'Сортировка'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.AddEditPhotoCategory.Sort', 'Sort'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Services.Features.EFeature.OfferWeightAndDimensions.DisplayingPropertyGroups', 'Отображение групп свойств в формате раскрывающегося списка'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Services.Features.EFeature.OfferWeightAndDimensions.DisplayingPropertyGroups', 'Displaying property groups in drop-down list format'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Services.Features.EFeature.OfferWeightAndDimensions.DisplayingPropertyGroups.TextOne', 'При активации данной опции группы свойств в карточке товара (в административной панели магазина) будут отображаться в виде раскрывающегося списка.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Services.Features.EFeature.OfferWeightAndDimensions.DisplayingPropertyGroups.TextOne', 'When this option is activated, groups of properties in the product card (in the stores administrative panel) will be displayed as a drop-down list.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Checkout.InstructionsUpsellingFunnels', 'Инструкция. Воронки допродаж. Что такое?'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Checkout.InstructionsUpsellingFunnels', 'Instructions. Upselling funnels. What is it?'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Checkout.OrderRating', 'Оценки заказов'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Checkout.OrderRating', 'Order ratings'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Checkout.Instructions.BuyersFields', 'Инструкция. Поля покупателя'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Checkout.Instructions.BuyersFields', 'Instructions. Buyers fields'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Checkout.Instructions.FieldsInDelivery', 'Инструкция. Поля в доставке'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Checkout.Instructions.FieldsInDelivery', 'Instructions. Fields in delivery'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Checkout.Instructions.FieldsBuyOneClick', 'Инструкция. Поля "Купить в один клик'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Checkout.Instructions.FieldsBuyOneClick', 'Instructions. Fields "Buy in one click"'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.ExportOrders.Status', 'Upload orders with status'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.ExportOrders.Source', 'Upload orders with order source'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.ExportOrders.Period', 'Download orders for the period'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.ExportOrders.Period.From', 'From'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.ExportOrders.Period.To', 'To'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.ExportOrders.Encoding', 'File encoding'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.NotifyEMails.Notifications.InstructionsNotifications', 'Инструкция. Уведомления'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.NotifyEMails.Notifications.InstructionsNotifications', 'Instructions. Notifications'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.NotifyEMails.EmailSettings.Instructions', 'Инструкция. Настройка e-mail почты магазина'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.NotifyEMails.EmailSettings.Instructions', 'Instructions. Setting up e-mail of the store'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.NotifyEMails.EmailSettings.ReadMore', 'Подробнее:'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.NotifyEMails.EmailSettings.ReadMore', 'Read more:'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.NotifyEMails.EmailSettings.EmailSetting', 'Настройка e-mail почты магазина'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.NotifyEMails.EmailSettings.EmailSetting', 'Setting up your stores email'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.NotifyEMails.EmailSettings.Attention', 'Внимание:'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.NotifyEMails.EmailSettings.Attention', 'Attention:'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.NotifyEMails.EmailSettings.AttentionTextOne', 'Гарантированная отправка массовых рассылок Email возможна только через почтовую службу ADVANTSHOP. Сервис является платным, подробнее о сервисе можно'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.NotifyEMails.EmailSettings.AttentionTextOne', 'Guaranteed sending of bulk emails is possible only through the ADVANTSHOP mail service. The service is paid, more details about the service can be found'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.NotifyEMails.EmailSettings.AttentionTextTwo', 'прочитать тут'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.NotifyEMails.EmailSettings.AttentionTextTwo', 'read here'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.NotifyEMails.EmailSettings.AttentionTextThree', 'SMTP является бесплатным, но не гарантирует корректную работу по отправке массовых рассылок.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.NotifyEMails.EmailSettings.AttentionTextThree', 'SMTP is free, but does not guarantee correct operation when sending mass mailings.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.NotifyEMails.EmailSettings.DemoEmail', 'Подключен demo email адрес'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.NotifyEMails.EmailSettings.DemoEmail', 'Demo email address connected'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.NotifyEMails.EmailSettings.ConnectYourEmail', 'Подключить свой email адрес'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.NotifyEMails.EmailSettings.ConnectYourEmail', 'Connect your email address'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.NotifyEMails.EmailSettings.AttentionTwo.AttentionTextOne', 'У Вас настроен демонстрационный Email адрес, через него нельзя осуществлять массовую рассылку email сообщений. Вам необходимо подключить свой Email адрес. Желательно чтобы это был адрес вида'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.NotifyEMails.EmailSettings.AttentionTwo.AttentionTextOne', 'You have a demo email address set up, you cannot send mass emails through it. You need to connect your email address. It is desirable that it be an address of the type'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.NotifyEMails.EmailSettings.AttentionTwo.AttentionTextTwo', 'info@@ваш-домен.ru'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.NotifyEMails.EmailSettings.AttentionTwo.AttentionTextTwo', 'info@@your-domain.ru'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.NotifyEMails.EmailSettings.InstructionsSettingsEmailDomain', 'Инструкция по настройке своей почты на домене'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.NotifyEMails.EmailSettings.InstructionsSettingsEmailDomain', 'Instructions for setting up your mail on a domain'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.AddEditMailAnswerTemplate.StoreVariables', 'Переменные магазина:'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.AddEditMailAnswerTemplate.StoreVariables', 'Store variables:'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.AddEditMailAnswerTemplate.BuyerVariables', 'Переменные покупателя:'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.AddEditMailAnswerTemplate.BuyerVariables', 'Buyer variables:'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.AddEditMailAnswerTemplate.OrderVariables', 'Переменные заказа:'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.AddEditMailAnswerTemplate.OrderVariables', 'Order variables:'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.AddEditMailAnswerTemplate.LeadVariables', 'Переменные лида:'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.AddEditMailAnswerTemplate.LeadVariables', 'Lead variables:'
    
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Checkout.AddTax', 'Add Tax'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Commonpage.Settingsmail.Settings', 'Настройки'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Commonpage.Settingsmail.Settings', 'Settings'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.NotifyEMails.SMSNotifications', 'SMS уведомления'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.NotifyEMails.SMSNotifications', 'SMS notifications'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.NotifyEMails.SMSNotifications.ConnectedSMSModule', 'Подключенный SMS-модуль'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.NotifyEMails.SMSNotifications.ConnectedSMSModule', 'Connected SMS module'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.NotifyEMails.SMSNotifications.FindInstallSMSModule', 'Найти и установить модули SMS-информирования'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.NotifyEMails.SMSNotifications.FindInstallSMSModule', 'Find and install SMS notification modules'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.NotifyEMails.SMSNotifications.AdministratorsPhone', 'Номер телефона администратора'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.NotifyEMails.SMSNotifications.AdministratorsPhone', 'Administrators phone number'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.NotifyEMails.SMSNotifications.AdministratorsPhoneExample', 'Номер телефона администратора (например 79091234567)'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.NotifyEMails.SMSNotifications.AdministratorsPhoneExample', 'Administrators phone number (for example 79091234567)'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.NotifyEMails.SMSNotifications.OrderNotifications', 'Уведомления по заказам'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.NotifyEMails.SMSNotifications.OrderNotifications', 'Order Notifications'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.NotifyEMails.SMSNotifications.SendOrderNotifications', 'Отправлять уведомления при создании заказа'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.NotifyEMails.SMSNotifications.SendOrderNotifications', 'Send notifications when an order is created'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.NotifyEMails.SMSNotifications.ToUser', 'Пользователю'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.NotifyEMails.SMSNotifications.ToUser', 'To the user'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.NotifyEMails.SMSNotifications.ToAdmin', 'Администратору'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.NotifyEMails.SMSNotifications.ToAdmin', 'To the administrator'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.NotifyEMails.SMSNotifications.SMSText', 'Текст sms'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.NotifyEMails.SMSNotifications.SMSText', 'sms text'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.NotifyEMails.SMSNotifications.NotificationsStatusChanges', 'Отправлять уведомления при изменении статуса заказа'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.NotifyEMails.SMSNotifications.NotificationsStatusChanges', 'Send notifications when order status changes'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.NotifyEMails.SMSNotifications.SMSTemplatesStatusChanges', 'Шаблоны SMS при смене статусов'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.NotifyEMails.SMSNotifications.SMSTemplatesStatusChanges', 'SMS templates for status changes'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.NotifyEMails.SMSNotifications.OrderStatus', 'Статус заказа'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.NotifyEMails.SMSNotifications.OrderStatus', 'Order status'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.NotifyEMails.SMSNotifications.AddTemplate', 'Добавить шаблон'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.NotifyEMails.SMSNotifications.AddTemplate', 'Add template'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.NotifyEMails.SMSNotifications.SMSVariables', 'Доступные переменные в SMS'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.NotifyEMails.SMSNotifications.SMSVariables', 'Available variables in SMS'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.NotifyEMails.SMSNotifications.SMSNumber', 'При использовании переменных число смс может значительно увеличиться'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.NotifyEMails.SMSNotifications.SMSNumber', 'When using variables, the number of SMS can increase significantly'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.NotifyEMails.SMSNotifications.LeadNotifications', 'Уведомления по лидам'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.NotifyEMails.SMSNotifications.LeadNotifications', 'Lead Notifications'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.NotifyEMails.SMSNotifications.LeadNotificationsSend', 'Отправлять уведомления при создании лида'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.NotifyEMails.SMSNotifications.LeadNotificationsSend', 'Send notifications when a lead is created'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.NotifyEMails.SMSNotifications.TestSMS', 'Отправка пробного sms-сообщения'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.NotifyEMails.SMSNotifications.TestSMS', 'Sending a test SMS message'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.NotifyEMails.SMSNotifications.Phone', 'Телефон'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.NotifyEMails.SMSNotifications.Phone', 'Phone number'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.NotifyEMails.SMSNotifications.Send', 'Отправить'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.NotifyEMails.SMSNotifications.Send', 'Send'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Settings.NotifyEMails.SMSNotifications.TemplateChange', 'Шаблон SMS при изменении статуса заказа'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Settings.NotifyEMails.SMSNotifications.TemplateChange', 'SMS template for order status change'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Settings.NotifyEMails.SMSNotifications.OrderStatus', 'Статус заказа'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Settings.NotifyEMails.SMSNotifications.OrderStatus', 'Order status'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Settings.NotifyEMails.SMSNotifications.Active', 'Активен'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Settings.NotifyEMails.SMSNotifications.Active', 'Active'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Settings.NotifyEMails.SMSNotifications.SMSText', 'Текст SMS'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Settings.NotifyEMails.SMSNotifications.SMSText', 'SMS text'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.CustomerGroups.Index.InstructionBuyerGroups', 'Инструкция. Группы покупателей'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.CustomerGroups.Index.InstructionBuyerGroups', 'Instructions. Buyer groups'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.CustomerGroups.Index.InstructionAdditionalFields', 'Инструкция. Дополнительные поля'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.CustomerGroups.Index.InstructionAdditionalFields', 'Instructions. Additional fields'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.CustomerFields.Grid.TotalString', 'Fields found: {0}'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.CustomerSegments.Index.BuyersTypes', 'Типы покупателей'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.CustomerSegments.Index.BuyersTypes', 'Types of buyers'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.CustomerSegments.Index.InstructionsSegmentationBase', 'Инструкция. Сегментация клиентской базы'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.CustomerSegments.Index.InstructionsSegmentationBase', 'Instructions. Segmentation of the customer base'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Settings.Customers.ExportCustomers.ExportOptions', 'Параметры экспорта'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Settings.Customers.ExportCustomers.ExportOptions', 'Export options'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Settings.Customers.ExportCustomers.BuyerFields', 'Поля покупателя'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Settings.Customers.ExportCustomers.BuyerFields', 'Buyer fields'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Settings.Customers.ExportCustomers.ResetSelected', 'Сбросить выбранные'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Settings.Customers.ExportCustomers.ResetSelected', 'Reset selected'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Settings.Customers.ExportCustomers.SetDefault', 'Установить по умолчанию'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Settings.Customers.ExportCustomers.SetDefault', 'Set as default'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Settings.Customers.ExportCustomers.UnloadingParameters', 'Параметры выгрузки'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Settings.Customers.ExportCustomers.UnloadingParameters', 'Unloading parameters'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsCustomer.CustomerTags.InstructionsBuyerTags', 'Инструкция. Теги покупателей'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsCustomer.CustomerTags.InstructionsBuyerTags', 'Instructions. Buyer tags'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.FileHelpers.FilesHelpText.MaxFileSize', 'Maximum file size: {0}'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Design.Index.InstructionsSample', 'Инструкция. Шаблон'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Design.Index.InstructionsSample', 'Instructions. Sample'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Design.Index.Name', 'Название'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Design.Index.Name', 'Name'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Design.Index.UpdateTemplate', 'Обновить шаблон'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Design.Index.UpdateTemplate', 'Update template'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Design.Index.DeleteTemplate', 'Удалить шаблон'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Design.Index.DeleteTemplate', 'Delete template'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Design.Index.DesignTemplate', 'Шаблон дизайна'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Design.Index.DesignTemplate', 'Design template'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Design.Index.InstalledTemplates', 'Установленные шаблоны'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Design.Index.InstalledTemplates', 'Installed templates'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Design.Index.TemplateStore', 'Магазин шаблонов'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Design.Index.TemplateStore', 'Template store'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Design.Index.TemplateFindInStore', 'В магазине вы сможете найти бесплатные и платные шаблоны'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Design.Index.TemplateFindInStore', 'In the store you can find free and paid templates'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Design.Index.SetNewTemplate', 'Установить новый шаблон'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Design.Index.SetNewTemplate', 'Set new template'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateSite.CreateSiteTemplateItem.ReadMore', 'Подробнее'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateSite.CreateSiteTemplateItem.ReadMore', 'Read more'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateSite.CreateSiteTemplateItem.FreeDays', '14 дней бесплатно'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateSite.CreateSiteTemplateItem.FreeDays', '14 days free'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateSite.Back', 'Назад'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateSite.Back', 'Back'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateSite.WebsiteCreation', 'Создание сайта'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateSite.WebsiteCreation', 'Website creation'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateSite.CreateStoreFunnelLanding', 'Создать интернет-магазин, воронку или лендинг'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateSite.CreateStoreFunnelLanding', 'Create an online store, funnel or landing page'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateSite.OnlineStoreDesignTemplates', 'Шаблоны дизайна для Интернет-магазина'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateSite.OnlineStoreDesignTemplates', 'Online Store Design Templates'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateSite.SelectWebsiteTypeTemplate', 'Выберите тип сайта и шаблон'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateSite.SelectWebsiteTypeTemplate', 'Select website type and template'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateSite.SelectTemplate', 'Выберите шаблон'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateSite.SelectTemplate', 'Select template'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateSite.Paid', 'Платные'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateSite.Paid', 'Paid'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateSite.All', 'Все'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateSite.All', 'All'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateSite.Free', 'Бесплатные'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateSite.Free', 'Free'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateSite.PaidTemplates', 'Платные шаблоны представлены в пробном режиме и доступны к установке без оплаты'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateSite.PaidTemplates', 'Paid templates are presented in trial mode and are available for installation without payment.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateSite.BlankTemplate', 'Пустой шаблон'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateSite.BlankTemplate', 'Blank template'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Commonpage.Mobile.InstructionsSettingMobile', 'Инструкция. Настройка мобильной версии'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Commonpage.Mobile.InstructionsSettingMobile', 'Instructions. Setting up the mobile version'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Commonpage.Mobile.Settings', 'Настройки'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Commonpage.Mobile.Settings', 'Settings'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Commonpage.Mobile.SelectAccordion', 'При выборе вида отображения меню "Аккордеон" - пункты нижнего меню будут свернуты.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Commonpage.Mobile.SelectAccordion', 'When you select the "Accordion" menu display type, the bottom menu items will be collapsed.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Commonpage.Mobile.BrowserBarColor', 'Цвет панели браузера'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Commonpage.Mobile.BrowserBarColor', 'Browser bar color'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Commonpage.Mobile.ReadMore', 'Подробнее'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Commonpage.Mobile.ReadMore', 'Read more'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Commonpage.Mobile.SelectSiteHeader', 'Выберите цвет шапки сайта'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Commonpage.Mobile.SelectSiteHeader', 'Select the color of the site header'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Commonpage.Mobile.HeaderColor', 'Цвет шапки'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Commonpage.Mobile.HeaderColor', 'Header color'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Commonpage.Mobile.Main', 'Основные'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Commonpage.Mobile.Main', 'Main'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Commonpage.Mobile.SelectLogoOption', 'Выберите вариант отображения логотипа'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Commonpage.Mobile.SelectLogoOption', 'Select a logo display option'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Commonpage.Mobile.LogoDisplayOptions', 'Варианты отображения логотипа'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Commonpage.Mobile.LogoDisplayOptions', 'Logo display options'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Commonpage.Mobile.DisplayTitleDefault', 'Настройка позволяет выводить заголовок по умолчанию'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Commonpage.Mobile.DisplayTitleDefault', 'The setting allows you to display the title by default'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Commonpage.Mobile.ShowTitleDefault', 'Показывать заголовок по умолчанию'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Commonpage.Mobile.ShowTitleDefault', 'Show title by default'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Commonpage.Mobile.EnterTextTitle', 'Настройка позволяет вписать свой текст для заголовка'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Commonpage.Mobile.EnterTextTitle', 'The setting allows you to enter your own text for the title'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Commonpage.Mobile.YourOwnTitle', 'Свой заголовок'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Commonpage.Mobile.YourOwnTitle', 'Your own title'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Commonpage.Mobile.SelectCatDisClickSideMenu', 'Выберите вид отображения категории при нажатии на боковое меню'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Commonpage.Mobile.SelectCatDisClickSideMenu', 'Select the category display type when clicking on the side menu'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Commonpage.Mobile.CategoryDisplayType', 'Вид отображения категорий'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Commonpage.Mobile.CategoryDisplayType', 'Category display type'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Commonpage.Mobile.CategoryDisplayTypeMenu', 'Вид отображения категорий в меню'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Commonpage.Mobile.CategoryDisplayTypeMenu', 'Display type of categories in the menu'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Commonpage.Mobile.DisplayProdCatLink', 'Отображать ссылку "Все товары категории" в меню'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Commonpage.Mobile.DisplayProdCatLink', 'Display "All products in category" link in menu'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Commonpage.Mobile.SettingLinkMenuPageAllProd', 'Настройка позволяет выводить ссылку в меню для перехода на страницу всех товаров'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Commonpage.Mobile.SettingLinkMenuPageAllProd', 'The setting allows you to display a link in the menu to go to the page of all products'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Commonpage.Mobile.DisplayLinkAllProdCatMenu', 'Отображать ссылку все товары категории в меню'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Commonpage.Mobile.DisplayLinkAllProdCatMenu', 'Display link all products category in menu'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Commonpage.Mobile.SettingAllowsAutoDisplayCity', 'Настройка позволяет выводить город в мобильной версии автоматически'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Commonpage.Mobile.SettingAllowsAutoDisplayCity', 'The setting allows you to display the city in the mobile version automatically'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Commonpage.Mobile.ShowCity', 'Выводить город'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Commonpage.Mobile.ShowCity', 'Show city'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Commonpage.Mobile.MainPage', 'Главная страница'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Commonpage.Mobile.MainPage', 'Main page'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Commonpage.Mobile.SettingShowCarouselMobile', 'Настройка отвечает за вывод карусели в мобильной версии'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Commonpage.Mobile.SettingShowCarouselMobile', 'The setting is responsible for displaying the carousel in the mobile version'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Commonpage.Mobile.ShowCarousel', 'Выводить карусель'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Commonpage.Mobile.ShowCarousel', 'Show carousel'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Commonpage.Mobile.NumberProdMainPage', 'Указываете количество товаров на главной странице'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Commonpage.Mobile.NumberProdMainPage', 'Specify the number of products on the main page'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Commonpage.Mobile.NumberProdOnMainPage', 'Количество товаров на главной'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Commonpage.Mobile.NumberProdOnMainPage', 'Number of products on the main page'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Commonpage.Mobile.OptionDispHorizOrVert', 'Указываете вариант отображения списка товаров на главной (горизонтально или вертикально)'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Commonpage.Mobile.OptionDispHorizOrVert', 'Specify the option for displaying the product list on the main page (horizontally or vertically)'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Commonpage.Mobile.TypeDispOnMainPage', 'Тип отображения списка товаров на главной'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Commonpage.Mobile.TypeDispOnMainPage', 'Type of display of the list of products on the main page'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Commonpage.Mobile.TypeDispCatOnMainPage', 'Тип отображения категорий на главной странице'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Commonpage.Mobile.TypeDispCatOnMainPage', 'Type of display of categories on the main page'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Commonpage.Mobile.TypeDispCatOnMainPageIcons', 'Выберите вид отображения категорий на главной странице (не выводить, без иконок, с иконками)'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Commonpage.Mobile.TypeDispCatOnMainPageIcons', 'Select the type of display of categories on the main page (do not display, without icons, with icons)'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Commonpage.Mobile.DisplayingCategoriesOnMainPage', 'Отображения категорий на главной странице'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Commonpage.Mobile.DisplayingCategoriesOnMainPage', 'Displaying categories on the main page'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Commonpage.Mobile.ProductCategory', 'Категория товаров'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Commonpage.Mobile.ProductCategory', 'Product category'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Commonpage.Mobile.DispTypeInCatalogDefault', 'Тип отображения списка товаров в каталоге (по умолчанию)'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Commonpage.Mobile.DispTypeInCatalogDefault', 'Type of display of the list of products in the catalog (default)'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Commonpage.Mobile.SelectTypeDispProdInCatTitListBlock', 'Выбираете тип отображения товаров в каталоге (плитка, список, блоки)'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Commonpage.Mobile.SelectTypeDispProdInCatTitListBlock', 'Select the type of display of products in the catalog (tiles, list, blocks)'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Commonpage.Mobile.TypeDispListProdInCat', 'Тип отображения списка товаров в каталоге'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Commonpage.Mobile.TypeDispListProdInCat', 'Type of display of the list of products in the catalog'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Commonpage.Mobile.SettingDispAddToCardButCatalog', 'Данная настройка выводит в каталоге кнопку "добавить в корзину"'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Commonpage.Mobile.SettingDispAddToCardButCatalog', 'This setting displays the "add to cart" button in the catalog'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Commonpage.Mobile.ShowAddToCartButInProdList', 'Отображать кнопку "В корзину" в списке товаров'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Commonpage.Mobile.ShowAddToCartButInProdList', 'Show "Add to Cart" button in product list'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Commonpage.Mobile.SizeProdBlock', 'Укажите размер высоты блока товара'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Commonpage.Mobile.SizeProdBlock', 'Specify the height size of the product block'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Commonpage.Mobile.HeightBlockCatalog', 'Высота блока изображения товара в каталоге'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Commonpage.Mobile.HeightBlockCatalog', 'Height of the product image block in the catalog'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Commonpage.Mobile.ProductCard', 'Карточка товара'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Commonpage.Mobile.ProductCard', 'Product card'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Commonpage.Mobile.DisplayUnitMeasurement', 'Отображать единицу измерения'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Commonpage.Mobile.DisplayUnitMeasurement', 'Display unit of measurement'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Commonpage.Mobile.CollapsInfoBlocksProdCart', 'Сворачивать информационные блоки в карточке товара'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Commonpage.Mobile.CollapsInfoBlocksProdCart', 'Collapse information blocks in the product card'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Commonpage.Mobile.DispTypeFooterSite', 'Вид отображения меню в подвале сайта'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Commonpage.Mobile.DispTypeFooterSite', 'Display type of menu in the footer of the site'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Commonpage.Mobile.ShowCityInMenu', 'Отображать город в меню'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Commonpage.Mobile.ShowCityInMenu', 'Show city in menu'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Commonpage.MySites', 'Мои сайты'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Commonpage.MySites', 'My sites'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Commonpage.Published', 'Опубликован'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Commonpage.Published', 'Published'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Commonpage.GoWebsite', 'Перейти на сайт'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Commonpage.GoWebsite', 'Go to website'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Commonpage.ConnectYourDomain', 'Подключить свой домен'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Commonpage.ConnectYourDomain', 'Connect your domain'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.MainPageProductsStore.Index.InstructionsProductsMain', 'Инструкция. Товары на главной'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.MainPageProductsStore.Index.InstructionsProductsMain', 'Instructions. Products on the main page'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.AddEditMenuItem.RecommendedSizes', 'Рекомендуемые размеры'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.AddEditMenuItem.RecommendedSizes', 'Recommended sizes'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.AddEditMenuItem.AvailableFormats', 'Доступные форматы'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.AddEditMenuItem.AvailableFormats', 'Available formats'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Carousel.Index.InstructionsCarousel', 'Инструкция. Карусель'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Carousel.Index.InstructionsCarousel', 'Instructions. Carousel'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Carousel.Index.ImagesFound', 'Найдено изображений: {0}'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Carousel.Index.ImagesFound', 'Images found: {0}'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.AddEditCarousel.AddCarouselImage', 'Добавить изображение карусели'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.AddEditCarousel.AddCarouselImage', 'Add Carousel Image'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.AddEditCarousel.EditCarouselImage', 'Редактировать изображение карусели'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.AddEditCarousel.EditCarouselImage', 'Edit Carousel Image'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Carousel.Index.InstructionsCreatingMenuItems', 'Инструкция. Создание пунктов меню'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Carousel.Index.InstructionsCreatingMenuItems', 'Instructions: Creating Menu Items'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.StaticPages.Index.InstructionsStaticPages', 'Инструкция. Работа со статическими страницами'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.StaticPages.Index.InstructionsStaticPages', 'Instructions. Working with static pages'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.StaticPages.Index.InstructionsStaticBlocks', 'Инструкция. Статические блоки'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.StaticPages.Index.InstructionsStaticBlocks', 'Instructions. Static blocks'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.StaticPages.Index.ChooseParent', 'Choose'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.StaticPages.Index.InstructionsNewsSectionSite', 'Инструкция. Настройка раздела новости на сайте'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.StaticPages.Index.InstructionsNewsSectionSite', 'Instructions. Setting up the news section on the site'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.News.AddEdit.NewsCategory', 'Category news'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.AddEditNewsCategpry.UseDefaultMeta', 'Использовать Meta по умолчанию'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.AddEditNewsCategpry.UseDefaultMeta', 'Use Meta by default'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.AddEditNewsCategpry.H1', 'H1 Heading'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.AddEditNewsCategpry.MetaHeader', 'Page Title'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.MainSite', 'Основной сайт'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.MainSite', 'Main site'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.Types.OnlineStore', 'Интернет-магазин'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.Types.OnlineStore', 'Online store'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.Types.SalesFunnel', 'Воронка продаж'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.Types.SalesFunnel', 'Sales Funnel'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.SalesChannels.AddSaleChannel', 'Добавить канал продаж'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.SalesChannels.AddSaleChannel', 'Add sales channel'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.SalesChannels.AddChannel', 'Добавить канал'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.SalesChannels.AddChannel', 'Add channel'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.SalesChannels.DeleteChannel', 'Удалить канал'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.SalesChannels.DeleteChannel', 'Delete channel'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.SalesChannels.Cancel', 'Отмена'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.SalesChannels.Cancel', 'Cancel'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.SalesChannels.Close', 'Закрыть'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.SalesChannels.Close', 'Close'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.ReferralLink.UsersReferredLinkProgramm', 'со всех платежей пользователей ADVANTSHOP, приведенных вами по партнёрской ссылке'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.ReferralLink.UsersReferredLinkProgramm', 'from all payments from ADVANTSHOP users referred by you via the affiliate link'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.ReferralLink.VKontakte', 'Поделиться ВКонтакте'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.ReferralLink.VKontakte', 'Share on VKontakte'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.ReferralLink.Facebook', 'Поделиться в Facebook'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.ReferralLink.Facebook', 'Share on Facebook'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.ReferralLink.Odnoklassniki', 'Поделиться в Однокласники'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.ReferralLink.Odnoklassniki', 'Share on Odnoklassniki'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.ReferralLink.Telegram', 'Поделиться в Telegram'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.ReferralLink.Telegram', 'Share on Telegram'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.ReferralLink.MoreAboutAffiliateProgram', 'Подробнее об условиях партнёрской программы'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.ReferralLink.MoreAboutAffiliateProgram', 'More about the terms of the affiliate program'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Home.Menu.ReferralProgram', 'Реферальная программа'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Home.Menu.ReferralProgram', 'Referral program'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Home.Menu.GoFullVersion', 'Перейти в полную версию сайта'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Home.Menu.GoFullVersion', 'Go to the full version of the site'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Home.Menu.Menu', 'Меню'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Home.Menu.Menu', 'Menu'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Home.Menu.BalanceRunningLow', 'На вашем балансе заканчиваются средства'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Home.Menu.BalanceRunningLow', 'Your balance is running low'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Home.Menu.TopUpBalance', 'Пополнить'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Home.Menu.TopUpBalance', 'Top up balance'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Home.Menu.Mobile.More', 'Еще'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Home.Menu.Mobile.More', 'More'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Modules.LeftMenu.Instructions', 'Инструкция. Как установить модули AdvantShop'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Modules.LeftMenu.Instructions', 'Instructions. How to install AdvantShop modules'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Modules.ModulesLayout.Instructions', 'Инструкция. Как установить модули AdvantShop'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Modules.ModulesLayout.Instructions', 'Instructions. How to install AdvantShop modules'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Modules.ModulesLayout.Delete', 'Удалить'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Modules.ModulesLayout.Delete', 'Delete'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Analytics.InstructionsReports', 'Инструкция. Отчёты'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Analytics.InstructionsReports', 'Instructions. Reports'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Analytics.ExportToExcel', 'Экспорт данных в Excel'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Analytics.ExportToExcel', 'Export data to Excel'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.OrderAnalysis.IncomeAllOrders', 'Доход (сумма всех заказов)'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.OrderAnalysis.IncomeAllOrders', 'Income (sum of all orders)'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.OrderAnalysis.NumberOrders', 'Кол-во заказов'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.OrderAnalysis.NumberOrders', 'Number of orders'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.OrderAnalysis.AverageBill', 'Средний чек'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.OrderAnalysis.AverageBill', 'Average bill'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Home.CongratulationsDashboard.HaveTrial', 'У вас подключен пробный период. Выберите тарифный план с помесячной или годовой оплатой со скидкой 25%.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Home.CongratulationsDashboard.HaveTrial', 'You have a trial period enabled. Choose a monthly or annual plan with a 25% discount.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Home.CongratulationsDashboard.SelectTariff', 'Выбрать тариф'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Home.CongratulationsDashboard.SelectTariff', 'Select tariff'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Home.CongratulationsDashboard.StoreProgress', 'Прогресс настройки магазина'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Home.CongratulationsDashboard.StoreProgress', 'Store setup progress'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Home.CongratulationsDashboard.CompleteTasks', 'Выполните задания ниже, чтобы быстрее добиться результатов в онлайн-продажах'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Home.CongratulationsDashboard.CompleteTasks', 'Complete the tasks below to achieve online sales results faster'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Home.CongratulationsDashboard.QuickSetup', 'Быстрая настройка'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Home.CongratulationsDashboard.QuickSetup', 'Quick setup'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Home.CongratulationsDashboard.EnterCompanyInfo', 'Введите данные о компании, которые увидят посетители вашего магазина'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Home.CongratulationsDashboard.EnterCompanyInfo', 'Enter the company information that your store visitors will see'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Home.CongratulationsDashboard.StoreName', 'Название магазина'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Home.CongratulationsDashboard.StoreName', 'Store name'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Home.CongratulationsDashboard.Country', 'Страна'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Home.CongratulationsDashboard.Country', 'Country'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Home.CongratulationsDashboard.Region', 'Регион'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Home.CongratulationsDashboard.Region', 'Region'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Home.CongratulationsDashboard.City', 'Город'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Home.CongratulationsDashboard.City', 'City'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Home.CongratulationsDashboard.PhoneNumber', 'Телефон'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Home.CongratulationsDashboard.PhoneNumber', 'Phone number'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Home.CongratulationsDashboard.DisplayClients', 'Для отображения клиентам'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Home.CongratulationsDashboard.DisplayClients', 'For display to clients'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Home.CongratulationsDashboard.Next', 'Далее'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Home.CongratulationsDashboard.Next', 'Next'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Home.CongratulationsDashboard.FirstItem', 'Добавьте первый товар'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Home.CongratulationsDashboard.FirstItem', 'Add your first item'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Home.CongratulationsDashboard.GoRemoveAdd', 'Перейдите в раздел «Товары» → Удалите тестовые категории и товары → Добавьте свои первые категории и товары.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Home.CongratulationsDashboard.GoRemoveAdd', 'Go to the Products section → Remove test categories and products → Add your first categories and products.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Home.CongratulationsDashboard.Goods', 'Товары'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Home.CongratulationsDashboard.Goods', 'Goods'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Home.CongratulationsDashboard.InstructionsAddManual', 'Инструкция по добавлению товаров вручную'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Home.CongratulationsDashboard.InstructionsAddManual', 'Instructions for adding products manually'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Home.CongratulationsDashboard.CustomizeStoreDesign', 'Настройте дизайн магазина'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Home.CongratulationsDashboard.CustomizeStoreDesign', 'Customize your store design'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Home.CongratulationsDashboard.ChooseDesignColorLogo', 'Выберите шаблон дизайна и настройте внешний вид. Выберите цветовую схему, загрузите или создайте логотип, добавьте слайды в карусель на главную страницу.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Home.CongratulationsDashboard.ChooseDesignColorLogo', 'Choose a design template and customize the look. Choose a color scheme, upload or create a logo, add slides to the carousel on the home page.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Home.CongratulationsDashboard.ChangeDesign', 'Изменить дизайн'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Home.CongratulationsDashboard.ChangeDesign', 'Change design'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Home.CongratulationsDashboard.InstructionsTemplates', 'Инструкция по настройке шаблонов'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Home.CongratulationsDashboard.InstructionsTemplates', 'Instructions for setting up templates'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Home.CongratulationsDashboard.ConnectDomain', 'Подключите домен'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Home.CongratulationsDashboard.ConnectDomain', 'Connect a domain'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Home.CongratulationsDashboard.NowTechnicalAddress', 'Сейчас у вас технический адрес магазина'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Home.CongratulationsDashboard.NowTechnicalAddress', 'Now you have the technical address of the store'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Home.CongratulationsDashboard.HaveYourOwnDomain', '. Если у вас имеется свой домен, вы можете подключить его или купить новый (цена от 189 руб).'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Home.CongratulationsDashboard.HaveYourOwnDomain', '. If you have your own domain, you can connect it or buy a new one (price from 189 rubles).'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Home.CongratulationsDashboard.ConnectDomainDo', 'Подключить домен'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Home.CongratulationsDashboard.ConnectDomainDo', 'Connect domain'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Home.CongratulationsDashboard.HowLaunchOnlineStore', 'Как запустить интернет-магазин?'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Home.CongratulationsDashboard.HowLaunchOnlineStore', 'How to launch an online store?'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Home.CongratulationsDashboard.EightLessonsCourse', 'Пройдите наш практический онлайн-курс «Как запустить интернет-магазин на Advantshop за 8 уроков». Получите готовый к продажам интернет-магазин на выходе.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Home.CongratulationsDashboard.EightLessonsCourse', 'Take our practical online course "How to launch an online store on Advantshop in 8 lessons". Get a ready-to-sell online store at the exit.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Home.CongratulationsDashboard.MoreAboutCourse', 'Подробнее о курсе'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Home.CongratulationsDashboard.MoreAboutCourse', 'More about the course'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Home.CongratulationsDashboard.SubscribeSocialNetworks', 'Подпишитесь на наши соцсети'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Home.CongratulationsDashboard.SubscribeSocialNetworks', 'Subscribe to our social networks'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Home.CongratulationsDashboard.LatestNewsArticlesLives', 'Свежие новости, статьи, прямые эфиры, советы по онлайн-продажам — это и многое другое вы найдете в наших социальных сетях.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Home.CongratulationsDashboard.LatestNewsArticlesLives', 'Latest news, articles, live broadcasts, online sales tips - you will find this and much more in our social networks.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Home.CongratulationsDashboard.InstructionsConnectingDomains', 'Инструкция по подключению доменов'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Home.CongratulationsDashboard.InstructionsConnectingDomains', 'Instructions for connecting domains'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Home.SettingStoreDashboard.SelectTariff', 'Выбрать тариф'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Home.SettingStoreDashboard.SelectTariff', 'Select tariff'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Home.SettingStoreDashboard.StoreProgress', 'Прогресс настройки магазина'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Home.SettingStoreDashboard.StoreProgress', 'Store setup progress'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Home.SettingStoreDashboard.CompleteTasks', 'Выполните задания ниже, чтобы быстрее добиться результатов в онлайн-продажах'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Home.SettingStoreDashboard.CompleteTasks', 'Complete the tasks below to achieve online sales results faster'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Home.SettingStoreDashboard.QuickSetup', 'Быстрая настройка'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Home.SettingStoreDashboard.QuickSetup', 'Quick setup'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Home.SettingStoreDashboard.EnterCompanyInfo', 'Введите данные о компании для отображения клиентам'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Home.SettingStoreDashboard.EnterCompanyInfo', 'Enter company details to display to clients'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Home.SettingStoreDashboard.StoreName', 'Название магазина'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Home.SettingStoreDashboard.StoreName', 'Store name'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Home.SettingStoreDashboard.City', 'Город'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Home.SettingStoreDashboard.City', 'City'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Home.SettingStoreDashboard.PhoneNumber', 'Телефон'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Home.SettingStoreDashboard.PhoneNumber', 'Phone number'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Home.SettingStoreDashboard.Next', 'Далее'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Home.SettingStoreDashboard.Next', 'Next'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Home.SettingStoreDashboard.FirstItem', 'Добавьте первый товар'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Home.SettingStoreDashboard.FirstItem', 'Add your first item'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Home.SettingStoreDashboard.GoRemoveAdd', 'Перейдите в раздел «Товары» → Удалите тестовые категории и товары → Добавьте свои первые категории и товары.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Home.SettingStoreDashboard.GoRemoveAdd', 'Go to the Products section → Remove test categories and products → Add your first categories and products.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Home.SettingStoreDashboard.Goods', 'Товары'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Home.SettingStoreDashboard.Goods', 'Goods'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Home.SettingStoreDashboard.InstructionsAddManual', 'Инструкция по добавлению товаров вручную'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Home.SettingStoreDashboard.InstructionsAddManual', 'Instructions for adding products manually'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Home.SettingStoreDashboard.CustomizeStoreDesign', 'Настройте дизайн магазина'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Home.SettingStoreDashboard.CustomizeStoreDesign', 'Customize your store design'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Home.SettingStoreDashboard.ChooseDesignColorLogo', 'Выберите шаблон дизайна и настройте внешний вид. Выберите цветовую схему, загрузите или создайте логотип, добавьте слайды в карусель на главную страницу.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Home.SettingStoreDashboard.ChooseDesignColorLogo', 'Choose a design template and customize the look. Choose a color scheme, upload or create a logo, add slides to the carousel on the home page.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Home.SettingStoreDashboard.ChangeDesign', 'Изменить дизайн'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Home.SettingStoreDashboard.ChangeDesign', 'Change design'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Home.SettingStoreDashboard.InstructionsTemplates', 'Инструкция по настройке шаблонов'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Home.SettingStoreDashboard.InstructionsTemplates', 'Instructions for setting up templates'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Home.SettingStoreDashboard.ConnectDomain', 'Подключите домен'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Home.SettingStoreDashboard.ConnectDomain', 'Connect a domain'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Home.SettingStoreDashboard.NowTechnicalAddress', 'Сейчас у вас технический адрес магазина'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Home.SettingStoreDashboard.NowTechnicalAddress', 'Now you have the technical address of the store'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Home.SettingStoreDashboard.HaveYourOwnDomain', ', вы можете подключить свой имеющийся у вас домен или купить новый (всего от 189 руб).'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Home.SettingStoreDashboard.HaveYourOwnDomain', ', you can connect your existing domain or buy a new one (from only 189 rubles).'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Home.SettingStoreDashboard.ConnectDomainDo', 'Подключить домен'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Home.SettingStoreDashboard.ConnectDomainDo', 'Connect domain'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Home.SettingStoreDashboard.InstructionsConnectingDomains', 'Инструкция по подключению доменов'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Home.SettingStoreDashboard.InstructionsConnectingDomains', 'Instructions for connecting domains'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Home.SettingStoreDashboard.HowLaunchOnlineStore', 'Как запустить интернет-магазин?'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Home.SettingStoreDashboard.HowLaunchOnlineStore', 'How to launch an online store?'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Home.SettingStoreDashboard.EightLessonsCourse', 'Пройдите наш практический онлайн-курс «Как запустить интернет-магазин на Advantshop за 8 уроков». Получите готовый к продажам интернет-магазин на выходе.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Home.SettingStoreDashboard.EightLessonsCourse', 'Take our practical online course "How to launch an online store on Advantshop in 8 lessons". Get a ready-to-sell online store at the exit.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Home.SettingStoreDashboard.MoreAboutCourse', 'Подробнее о курсе'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Home.SettingStoreDashboard.MoreAboutCourse', 'More about the course'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Home.SettingStoreDashboard.SubscribeSocialNetworks', 'Подпишись на нас'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Home.SettingStoreDashboard.SubscribeSocialNetworks', 'Subscribe to us'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Home.SettingStoreDashboard.LatestNewsArticlesLives', 'Новости, прямые эфиры, советы как продавать в интернете в наших группах.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Home.SettingStoreDashboard.LatestNewsArticlesLives', 'News, live broadcasts, tips on how to sell online in our groups.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Customers.ViewCustomerHeader.Buyers', 'Покупатели'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Customers.ViewCustomerHeader.Buyers', 'Buyers'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Customers.Index.InstructionsBuyers', 'Инструкция. Покупатели'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Customers.Index.InstructionsBuyers', 'Instructions. Buyers'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Customers.ViewInfo.ShowOnMap', 'Показать на карте'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Customers.ViewInfo.ShowOnMap', 'Show on map'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Orders.OrderInfo.ShowOnMap', 'Показать на карте'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Orders.OrderInfo.ShowOnMap', 'Show on map'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.BonusSystem.BonusSystemLayout.BonusProgram', 'Бонусная программа'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.BonusSystem.BonusSystemLayout.BonusProgram', 'Bonus program'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.BonusSystem.Index.BonusCards', 'Бонусные карты'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.BonusSystem.Index.BonusCards', 'Bonus cards'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.BonusSystem.Index.InstructionsBonusSystem', 'Инструкция. Бонусная система'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.BonusSystem.Index.InstructionsBonusSystem', 'Instructions. Bonus system'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.BonusSystem.Index.Grades', 'Грейды'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.BonusSystem.Index.Grades', 'Grades'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.BonusSystem.Index.Rules', 'Правила'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.BonusSystem.Index.Rules', 'Rules'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.BonusSystem.Index.SettingsBonus', 'Настройки'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.BonusSystem.Index.SettingsBonus', 'Settings Bonus'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.BonusSystem.Analytics.AnalyticsLink', 'Аналитика'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.BonusSystem.Analytics.AnalyticsLink', 'Analytics'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.SettingsBonus.GoToGetBonusCardPage', 'Go to the page for receiving a bonus card'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.SmsTemplate.NoAvailableVariables', 'Нет доступных переменных'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.SmsTemplate.NoAvailableVariables', 'No variables available'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Triggers.Index.InstructionsTriggerMarketing', 'Инструкция. Триггерный маркетинг'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Triggers.Index.InstructionsTriggerMarketing', 'Instructions. Trigger Marketing'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Triggers.AddTrigger', 'Добавление триггера'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Triggers.AddTrigger', 'Adding a trigger'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Triggers.SelectTrigger', 'Выберите триггер'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Triggers.SelectTrigger', 'Select trigger'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Triggers.AfterEvent', 'После того как в системе произойдет следующее событие:'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Triggers.AfterEvent', 'After the following event occurs in the system:'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Triggers.TriggerCategory', 'Категория триггера'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Triggers.TriggerCategory', 'Trigger Category'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Triggers.EditTrigger.EmailAnalytics', 'Аналитика email рассылки'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Triggers.EditTrigger.EmailAnalytics', 'Email newsletter analytics'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Triggers.EditTrigger.CopyTrigger', 'Копировать триггер'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Triggers.EditTrigger.CopyTrigger', 'Copy trigger'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Triggers.EditTrigger.SaveTriggerChanges', 'Сохранить'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Triggers.EditTrigger.SaveTriggerChanges', 'Save'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Triggers.EditTrigger.SelectTrigger', 'Выберите триггер'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Triggers.EditTrigger.SelectTrigger', 'Select trigger'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Triggers.EditTrigger.AfterEvent', 'После того как в системе произойдет следующее событие:'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Triggers.EditTrigger.AfterEvent', 'After the following event occurs in the system:'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Triggers.EditTrigger.Through', 'Через'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Triggers.EditTrigger.Through', 'Through'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Triggers.EditTrigger.Days', 'дней'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Triggers.EditTrigger.Days', 'days'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Triggers.EditTrigger.Date', 'Дата'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Triggers.EditTrigger.Date', 'Date'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Triggers.EditTrigger.IgnoreYear', 'Игнорировать год'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Triggers.EditTrigger.IgnoreYear', 'Ignore the year'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Triggers.EditTrigger.Type', 'Тип'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Triggers.EditTrigger.Type', 'Type'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Triggers.EditTrigger.TriggeredOnceOrderLead', 'Срабатывает только 1 раз для связки покупатель - заказ/лид'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Triggers.EditTrigger.TriggeredOnceOrderLead', 'Triggered only once for the buyer - order/lead link'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Triggers.EditTrigger.ResponseTime', 'Время срабатывания'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Triggers.EditTrigger.ResponseTime', 'Response time'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Triggers.EditTrigger.ServerTime', 'Время сервера'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Triggers.EditTrigger.ServerTime', 'Server time'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Triggers.EditTrigger.MoscowTime', 'Время Московское'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Triggers.EditTrigger.MoscowTime', 'Moscow time'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Triggers.EditTrigger.TriggeredOnceOrderLeadText', 'Триггер срабатает только 1 раз для связки покупателя и заказа/лида. Все действия выполнятся. <br />
                        Например, есть триггер на изменение статуса заказа "В обработке" и стоит галка. Когда статус заказа №123 изменится на "В
                        обработке", то сработает триггер для покупателя. Далее сколько бы раз не меняли на "В обработке" у заказа №123, триггер
                        больше не сработает для этого покупателя. Когда изменится статус заказа №124 и др. на "В обработке", то выполнится триггер
                        для покупателя из этого заказа.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Triggers.EditTrigger.TriggeredOnceOrderLeadText', 'The trigger will only work once for the buyer and order/lead link. All actions will be performed. <br />
For example, there is a trigger for changing the order status "In Processing" and the checkbox is checked. When the status of order #123 changes to "In
processing", the trigger will work for the buyer. Then, no matter how many times you change order #123 to "In
processing", the trigger will no longer work for this buyer. When the status of order #124 and others changes to "In
processing", the trigger will work for the buyer from this order.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Triggers.EditTrigger.TriggerCouponTemplate', 'Шаблон купона триггера'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Triggers.EditTrigger.TriggerCouponTemplate', 'Trigger coupon template'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Triggers.EditTrigger.GenerateCouponTriggered', 'Генерировать купон в момент срабатывания триггера'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Triggers.EditTrigger.GenerateCouponTriggered', 'Generate a coupon when the trigger is triggered'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Triggers.EditTrigger.InsertGenerateCoupon', 'Сгенерированный купон Вы можете вставлять в текст писем и смс через переменную #TRIGGER_COUPON#'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Triggers.EditTrigger.InsertGenerateCoupon', 'You can insert the generated coupon into the text of letters and SMS via the variable #TRIGGER_COUPON#'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Triggers.EditTrigger.TriggerCategory', 'Категория триггера'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Triggers.EditTrigger.TriggerCategory', 'Trigger Category'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Triggers.EditTrigger.SelectTriggerCondition', 'Выберите условия срабатывания действий (Проверяется каждый раз, перед выполнением каждого действия)'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Triggers.EditTrigger.SelectTriggerCondition', 'Select the conditions for triggering actions (Checked each time, before each action is performed)'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Triggers.EditTrigger.AddCondition', 'Добавить условие'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Triggers.EditTrigger.AddCondition', 'Add condition'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Triggers.EditTrigger.DeleteSomething', 'Удалить'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Triggers.EditTrigger.DeleteSomething', 'Delete'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Triggers.EditTrigger.SelectAction', 'Выберите действие'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Triggers.EditTrigger.SelectAction', 'Select an action'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Triggers.EditTrigger.AddAction', 'Добавить действие'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Triggers.EditTrigger.AddAction', 'Add action'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Triggers.EditTrigger.SendEmail', 'Отправить Email'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Triggers.EditTrigger.SendEmail', 'Send Email'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Triggers.EditTrigger.SendPush', 'Отправить Push'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Triggers.EditTrigger.SendPush', 'Send Push'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Triggers.EditTrigger.SendSMS', 'Отправить SMS'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Triggers.EditTrigger.SendSMS', 'Send SMS'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Triggers.EditTrigger.EditData', 'Изменить данные'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Triggers.EditTrigger.EditData', 'Edit data'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Triggers.EditTrigger.SendHTTP', 'Отправить HTTP-запрос'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Triggers.EditTrigger.SendHTTP', 'Send HTTP request'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Triggers.EditTrigger.SendWhatsApp', 'Отправить сообщение в WhatsApp'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Triggers.EditTrigger.SendWhatsApp', 'Send a message to WhatsApp'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Triggers.EditTrigger.SubjectLetter', 'Тема письма'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Triggers.EditTrigger.SubjectLetter', 'Subject of the letter'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Triggers.EditTrigger.TextLetter', 'Текст письма'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Triggers.EditTrigger.TextLetter', 'Text of the letter'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Triggers.EditTrigger.AvailableVariables', 'Доступные переменные'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Triggers.EditTrigger.AvailableVariables', 'Available variables'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Triggers.EditTrigger.SMSText', 'Текст sms'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Triggers.EditTrigger.SMSText', 'sms text'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Triggers.EditTrigger.SelectTemplate', 'Выбрать шаблон'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Triggers.EditTrigger.SelectTemplate', 'Select template'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Triggers.EditTrigger.Sample', 'Шаблон'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Triggers.EditTrigger.Sample', 'Sample'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Triggers.EditTrigger.ConditionSMS', '(При использовании переменных число смс может значительно увеличиться)'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Triggers.EditTrigger.ConditionSMS', '(When using variables, the number of SMS may increase significantly)'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Triggers.EditTrigger.SMSSetSend', 'Для отправки СМС необходимо настроить'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Triggers.EditTrigger.SMSSetSend', 'To send SMS you need to set up'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Triggers.EditTrigger.SMSNotifications', 'SMS уведомления'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Triggers.EditTrigger.SMSNotifications', 'SMS notifications'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Triggers.EditTrigger.MessageText', 'Текст сообщения'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Triggers.EditTrigger.MessageText', 'Message text'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Triggers.EditTrigger.SendMessagesModule', 'Для отправки сообщений необходимо установить\настроить модуль'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Triggers.EditTrigger.SendMessagesModule', 'To send messages you need to install/configure the module'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Triggers.EditTrigger.CouponGeneration', 'Генерация купона'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Triggers.EditTrigger.CouponGeneration', 'Coupon generation'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Triggers.EditTrigger.CouponTemplate', 'Шаблон купона'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Triggers.EditTrigger.CouponTemplate', 'Coupon template'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Triggers.EditTrigger.AddCouponTemplate', 'Добавить шаблон купона'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Triggers.EditTrigger.AddCouponTemplate', 'Add a coupon template'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Triggers.EditTrigger.Straightaway', 'Сразу'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Triggers.EditTrigger.Straightaway', 'Straightaway'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Triggers.EditTrigger.Delay', 'Задержка'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Triggers.EditTrigger.Delay', 'Delay'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Triggers.EditTrigger.SendPushInstallModule', 'Для отправки push уведомлений необходимо установить\настроить модуль'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Triggers.EditTrigger.SendPushInstallModule', 'To send push notifications, you need to install/configure the module'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Triggers.EditTrigger.MobileApplication', 'Мобильное приложение'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Triggers.EditTrigger.MobileApplication', 'Mobile application'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Triggers.EditTrigger.NewMeaning', 'Новое значение'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Triggers.EditTrigger.NewMeaning', 'New meaning'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Triggers.EditTrigger.Field', 'Поле'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Triggers.EditTrigger.Field', 'Field'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Triggers.TriggerActionSendRequest.Method', 'Метод'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Triggers.TriggerActionSendRequest.Method', 'Method'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Triggers.TriggerActionSendRequest.Add', 'Добавить'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Triggers.TriggerActionSendRequest.Add', 'Add'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Triggers.TriggerActionSendRequest.Name', 'Название'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Triggers.TriggerActionSendRequest.Name', 'Name'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Triggers.TriggerActionSendRequest.Meaning', 'Значение'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Triggers.TriggerActionSendRequest.Meaning', 'Meaning'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Triggers.TriggerActionSendRequest.Headlines', 'Заголовки'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Triggers.TriggerActionSendRequest.Headlines', 'Headlines'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Triggers.TriggerActionSendRequest.AvailableVariables', 'Доступные переменные:'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Triggers.TriggerActionSendRequest.AvailableVariables', 'Available variables:'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Home.Menu.Reviews.FeedbackSettings', 'Настройки отзывов'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Home.Menu.Reviews.FeedbackSettings', 'Feedback settings'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Home.Menu.Reviews.Review', 'отзыв'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Home.Menu.Reviews.Review', 'review'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Home.Menu.Reviews.ReviewText', 'Текст отзыва :'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Home.Menu.Reviews.ReviewText', 'Review text :'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.ExportFeeds.IndexCsv.InstructionsExportCategories', 'Инструкция. Экспорт категорий через CSV'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.ExportFeeds.IndexCsv.InstructionsExportCategories', 'Instructions. Export categories via CSV'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Catalog.ProductsFound', 'Найдено товаров'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Catalog.ProductsFound', 'Products found'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Product.Edit.InstructionsAddProduct', 'Инструкция. Как добавить новый товар'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Product.Edit.InstructionsAddProduct', 'Instructions. How to add a new product'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Product.RightBlock.HistoryChanges', 'История изменения'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Product.RightBlock.HistoryChanges', 'History of changes'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Category.RightBlock.HistoryChanges', 'История изменения'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Category.RightBlock.HistoryChanges', 'History of changes'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Product.Edit.Directory', 'Справочник'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Product.Edit.Directory', 'Directory'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Product.Edit.UnitsMeasurement', 'единиц измерения'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Product.Edit.UnitsMeasurement', 'units of measurement'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Product.Edit.SubjectCalculation', 'Предмет расчета'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Product.Edit.SubjectCalculation', 'Subject of calculation'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Product.Edit.ParameterPrintingChecks', 'Данный параметр будет передаваться для печати чеков'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Product.Edit.ParameterPrintingChecks.', 'This parameter will be passed for printing checks.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Product.Edit.MethodCalculation', 'Способ расчета'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Product.Edit.MethodCalculation', 'Method of calculation'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Product.Edit.HiddenProductText', 'Скрытый товар не отображается в каталоге и не учавствует в поиске, но в него можно перейти. Товар может быть скрытым при помощи модуля. Например, Сималенд.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Product.Edit.HiddenProductText', 'A hidden product is not displayed in the catalog and does not participate in the search, but you can go to it. The product can be hidden using a module. For example, Simalend.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Product.Edit.HiddenProduct', 'Скрытый товар'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Product.Edit.HiddenProduct', 'A hidden product'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Product.LandingFunnels.AfterPlacingFunnel', 'После оформления заказа клиент может быть перенаправлен на воронку с допродажами, где он сможет дополнить заказ нужными товарами.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Product.LandingFunnels.AfterPlacingFunnel', 'After placing an order, the client can be redirected to a funnel with additional sales, where he can supplement the order with the necessary products.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Product.LandingFunnels.FirstCrossSell', 'Первой показывается страница с товаром Cross Sell - это товар, который дополняет основной товар. Если клиент соглашается добавить его, то он сразу же переходит на страницу благодарности. Если же клиент отказывается от Cross Sell, то он переходит на страницу с Down Sell - это товар с более низкой ценой, но высокой маржинальностью. Это может быть более дешевый аналог Cross Sell.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Product.LandingFunnels.FirstCrossSell', 'The first page shown is the Cross Sell product page - this is a product that complements the main product. If the client agrees to add it, he/she is immediately taken to the thank you page. If the client refuses Cross Sell, he/she is taken to the Down Sell page - this is a product with a lower price but high marginality. This may be a cheaper analogue of Cross Sell.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Product.LandingFunnels.SelectExistingFunnel', 'Выбрать существую воронку'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Product.LandingFunnels.SelectExistingFunnel', 'Select an existing funnel'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Product.LandingFunnels.SalesFunnelsNotConnected', 'Воронки продаж не подключены'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Product.LandingFunnels.SalesFunnelsNotConnected', 'Sales funnels are not connected'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Product.LandingFunnels.UpsellingFunnel', 'Воронка допродаж:'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Product.LandingFunnels.UpsellingFunnel', 'Upselling Funnel:'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Product.LandingFunnels.UntieFunnel', 'Отвязать воронку'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Product.LandingFunnels.UntieFunnel', 'Untie the funnel'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Product.LandingFunnels.DeleteFunnel', 'Удалить'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Product.LandingFunnels.DeleteFunnel', 'Delete'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Landings.AddLanding.NewFunnel', 'Новая воронка'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Landings.AddLanding.NewFunnel', 'New funnel'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Landings.AddLanding.AnotherFunnelName', 'Укажите другое название воронки'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Landings.AddLanding.AnotherFunnelName', 'Please specify another funnel name'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Landings.AddLanding.ErrorCreatingFunnel', 'Ошибка при создании воронки'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Landings.AddLanding.ErrorCreatingFunnel', 'Error creating funnel'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.AddLandingSite.NewFunnel', 'Новая воронка'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.AddLandingSite.NewFunnel', 'New funnel'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.AddLandingSite.MainProduct', 'Основной товар'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.AddLandingSite.MainProduct', 'Main product'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.AddLandingSite.BasedDataSelectedProduct', 'На основе данных выбранного товара будет сгенерирована первая страница воронки.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.AddLandingSite.BasedDataSelectedProduct', 'Based on the data of the selected product, the first page of the funnel will be generated.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.AddLandingSite.SelectUpsellDownsellProducts', 'Выбрать товары допродаж (Upsell, Downsell)'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.AddLandingSite.SelectUpsellDownsellProducts', 'Select upsell, downsell products'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.AddLandingSite.UpsellOne', 'Допродажа 1 (Upsell)'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.AddLandingSite.UpsellOne', 'Upsell 1'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.AddLandingSite.SelectUpsellProductSecond', 'Выберите товар, который будет предложено добавить в заказ в дополнение к основному товару'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.AddLandingSite.SelectUpsellProductSecond', 'Select the product that will be offered to be added to the order in addition to the main product'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.AddLandingSite.UpsellTwoDownsell', 'Допродажа 2 (Downsell)'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.AddLandingSite.UpsellTwoDownsell', 'Upsell 2 (Downsell)'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.AddLandingSite.SelectProdRefuseFirstUpsell', 'Выберите товар, который будет предложен в случае отказа от первой допродажи'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.AddLandingSite.SelectProdRefuseFirstUpsell', 'Select the product that will be offered in case of refusal of the first upsell'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.AddLandingSite.Continue', 'Продолжить'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.AddLandingSite.Continue', 'Continue'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.AddLandingSite.FunnelName', 'Название воронки'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.AddLandingSite.FunnelName', 'Funnel name'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.AddLandingSite.InstallingFunnel', 'Установка воронки'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.AddLandingSite.InstallingFunnel', 'Installing a funnel'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.AddLandingSite.ContactCollection', 'Сбор контактов "Статья"'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.AddLandingSite.ContactCollection', 'Contact collection "Article"'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.AddLandingSite.FunnelDiagram', 'Схема воронки'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.AddLandingSite.FunnelDiagram', 'Funnel diagram'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.AddLandingSite.FunnelDemo', 'Демо воронки'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.AddLandingSite.FunnelDemo', 'Funnel demo'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.AddLandingSite.ActionAfterFilling', 'Действие после заполнения формы'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.AddLandingSite.ActionAfterFilling', 'Action after filling out the form'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.AddLandingSite.TransitionTo', 'Переход в'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.AddLandingSite.TransitionTo', 'Transition to'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.AddLandingSite.ActionAfterFilling', 'Действие после заполнения формы'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.AddLandingSite.ActionAfterFilling', 'Action after filling out the form'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.AddLandingSite.SelectCategory', 'Выберите категорию'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.AddLandingSite.SelectCategory', 'Select category'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.AddLandingSite.Funnel', 'Воронка'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.AddLandingSite.Funnel', 'Funnel'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.AddLandingSite.SpecifyURL', 'Укажите URL:'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.AddLandingSite.SpecifyURL', 'Specify URL:'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.AddLandingSite.Back', 'Назад'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.AddLandingSite.Back', 'Back'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.AddLandingSite.LinkSpecialOffer', 'Укажите ссылку на специальное предложение'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.AddLandingSite.LinkSpecialOffer', 'Please provide a link to the special offer'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.AddLandingSite.StepOneSelectProductsFunnel', 'Шаг 1. Выберите товары для воронки'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.AddLandingSite.StepOneSelectProductsFunnel', 'Step 1: Select Products for the Funnel'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.AddLandingSite.SelectProductsFunnel', 'Выбрать несколько товаров для воронки'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.AddLandingSite.SelectProductsFunnel', 'Select multiple products for the funnel'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.AddLandingSite.SelectCategoriesFunnel', 'Выбрать категории для воронки'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.AddLandingSite.SelectCategoriesFunnel', 'Select categories for the funnel'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.AddLandingSite.StepOneSelectCategoriesFunnel', 'Шаг 1. Выберите категории товаров для воронки'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.AddLandingSite.StepOneSelectCategoriesFunnel', 'Step 1: Select Product Categories for the Funnel'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.AddLandingSite.SelectMultipleProductsFunnel', 'Выберите несколько товаров для воронки.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.AddLandingSite.SelectMultipleProductsFunnel', 'Select multiple products for your funnel.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.AddLandingSite.SelectProductsForFunnel', 'Выберите товары для воронки'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.AddLandingSite.SelectProductsForFunnel', 'Select products for the funnel'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.AddLandingSite.BlankTemplate', 'Пустой шаблон'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.AddLandingSite.BlankTemplate', 'Blank template'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Product.CustomOptionsModal.AddNewOption', 'Добавить новую опцию'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Product.CustomOptionsModal.AddNewOption', 'Add new option'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Product.CustomOptionsModal.DeleteOption', 'Удалить опцию'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Product.CustomOptionsModal.DeleteOption', 'Delete option'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Product.CustomOptionsModal.Delete', 'Удалить'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Product.CustomOptionsModal.Delete', 'Delete'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Product.CustomOptionsModal.UploadImage', 'Загрузить изображение'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Product.CustomOptionsModal.UploadImage', 'Upload image'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Product.CustomOptionsModal.AddNewEntry', 'Добавить новую запись'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Product.CustomOptionsModal.AddNewEntry', 'Add a new entry'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Product.CustomOptionsModal.Description', 'Описание'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Product.CustomOptionsModal.Description', 'Description'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Product.CustomOptionsModal.Sort', 'Сортировка'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Product.CustomOptionsModal.Sort', 'Sort'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Product.CustomOptionsModal.ByDefault', 'По умолчанию'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Product.CustomOptionsModal.ByDefault', 'By default'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Product.CustomOptionsModal.MaxQuantity', 'Максимальное количество'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Product.CustomOptionsModal.MaxQuantity', 'Max quantity'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Product.CustomOptionsModal.MinQuantity', 'Минимальное количество'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Product.CustomOptionsModal.MinQuantity', 'Min quantity'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Product.CustomOptionsModal.PriceType', 'Тип цены'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Product.CustomOptionsModal.PriceType', 'Price type'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Product.CustomOptionsModal.Price', 'Цена'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Product.CustomOptionsModal.Price', 'Price'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Product.CustomOptionsModal.Name', 'Название'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Product.CustomOptionsModal.Name', 'Name'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Product.CustomOptionsModal.Product', 'Товар'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Product.CustomOptionsModal.Product', 'Product'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Product.CustomOptionsModal.SelectProduct', 'Выбрать товар'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Product.CustomOptionsModal.SelectProduct', 'Select product'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Product.CustomOptionsModal.RemoveProduct', 'Удалить товар'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Product.CustomOptionsModal.RemoveProduct', 'Remove item'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Product.CustomOptionsModal.Image', 'Изображение'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Product.CustomOptionsModal.Image', 'Image'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Product.CustomOptionsModal.SelectImage', 'Загрузить изображение'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Product.CustomOptionsModal.SelectImage', 'Upload image'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Product.CustomOptionsModal.RemoveImage', 'Удалить изображение'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Product.CustomOptionsModal.RemoveImage', 'Remove image'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Product.CustomOptionsModal.TableValues', 'Таблица значений'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Product.CustomOptionsModal.TableValues', 'Table of values'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Product.CustomOptionsModal.SortOrder', 'Порядок сортировки'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Product.CustomOptionsModal.SortOrder', 'Sort order'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Product.CustomOptionsModal.ComboFunctionalityAdditionalOptions', '"Функционал комбо в доп.опциях"'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Product.CustomOptionsModal.ComboFunctionalityAdditionalOptions', '"Combo functionality in additional options"'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Product.CustomOptionsModal.WorkCorrect', 'Для корректной работы настроек мин. и макс. количества убедитесь, что включена экспериментальная функция'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Product.CustomOptionsModal.WorkCorrect', 'For the min and max quantity settings to work correctly, make sure the experimental feature is enabled.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Product.CustomOptionsModal.MaxNumOptions', 'Максимальное количество опции, которое можно будет выбрать.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Product.CustomOptionsModal.MaxNumOptions', 'The maximum number of options that can be selected.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Product.CustomOptionsModal.AdvancedSettingsAdditionalOptions', '"Расширенные настройки доп.опций"'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Product.CustomOptionsModal.AdvancedSettingsAdditionalOptions', '"Advanced settings of additional options"'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Product.CustomOptionsModal.CorrectWorkExpFunc', 'Для корректной работы настроек мин. и макс. количества убедитесь, что включена экспериментальная функция'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Product.CustomOptionsModal.CorrectWorkExpFunc', 'For the min and max quantity settings to work correctly, make sure the experimental feature is enabled.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Product.CustomOptionsModal.MinNumOptionsQuantity', 'Минимальное количество опции, которое можно будет выбрать.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Product.CustomOptionsModal.MinNumOptionsQuantity', 'The minimum number of options that can be selected.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Product.CustomOptionsModal.Required', 'Обязательный'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Product.CustomOptionsModal.Required', 'Required'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Product.CustomOptionsModal.QuanOptZeroMandatory', 'Обязательный: если мин.кол-во опции больше нуля, опция должна быть обязательной'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Product.CustomOptionsModal.QuanOptZeroMandatory', 'Mandatory: If the min.qty of an option is greater than zero, the option must be mandatory.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Product.CustomOptionsModal.InputType', 'Тип ввода'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Product.CustomOptionsModal.InputType', 'Input type'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.ExportOptions.Expiry', 'Срок годности или службы (expiry)'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.ExportOptions.Expiry', 'Expiry date'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.ExportOptions.Expiry.Expiry', 'Срок годности или службы.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.ExportOptions.Expiry.Expiry', 'Expiry date.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.ExportOptions.Expiry.ExpiryFormat', 'Должен быть указан в формате P1Y2M10D. Расшифровка примера — 1 год, 2 месяца и 10 дней.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.ExportOptions.Expiry.ExpiryFormat', 'Must be specified in the format P1Y2M10D. Example decoding - 1 year, 2 months and 10 days.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.ExportOptions.Expiry.OtherExamples', 'Другие примеры'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.ExportOptions.Expiry.OtherExamples', 'Other examples'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.ExportOptions.Expiry.FifteenDays', 'P15D — 15 дней;'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.ExportOptions.Expiry.FifteenDays', 'P15D — 15 days;'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.ExportOptions.Expiry.TwoYears', 'P2Y10D — 2 года, 10 дней.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.ExportOptions.Expiry.TwoYears', 'P2Y10D - 2 years, 10 days.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.ExportOptions.WarrantyPeriod', 'Гарантийный срок (warranty-days)'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.ExportOptions.WarrantyPeriod', 'Warranty period (warranty days)'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.ExportOptions.WarrantyPeriod.WarrantyPeriod', 'Гарантийный срок. В течение этого периода возможны обслуживание и ремонт товара, возврат денег (в годах, месяцах или днях).'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.ExportOptions.WarrantyPeriod.WarrantyPeriod', 'Warranty period. During this period, service and repair of the product, refund of money (in years, months or days) are possible.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.ExportOptions.WarrantyPeriod.WarrantyFormat', 'Должен быть указан в формате P1Y2M10D. Расшифровка примера — 1 год, 2 месяца и 10 дней.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.ExportOptions.WarrantyPeriod.WarrantyFormat', 'Must be specified in the format P1Y2M10D. Example decoding - 1 year, 2 months and 10 days.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.ExportOptions.WarrantyPeriod.OtherExamples', 'Другие примеры'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.ExportOptions.WarrantyPeriod.OtherExamples', 'Other examples'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.ExportOptions.WarrantyPeriod.FifteenDays', 'P15D — 15 дней;'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.ExportOptions.WarrantyPeriod.FifteenDays', 'P15D — 15 days;'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.ExportOptions.WarrantyPeriod.TwoYears', 'P2Y10D — 2 года, 10 дней.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.ExportOptions.WarrantyPeriod.TwoYears', 'P2Y10D - 2 years, 10 days.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.ExportOptions.WarrantyConditions', 'Дополнительные условия гарантии (comment-warranty)'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.ExportOptions.WarrantyConditions', 'Additional warranty conditions (comment-warranty)'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.ExportOptions.WarrantyConditions.WarrantyConditions', 'Дополнительные условия гарантии, например «Гарантия на аккумулятор — 6 месяцев»'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.ExportOptions.WarrantyConditions.WarrantyConditions', 'Additional warranty terms, such as "Battery warranty - 6 months"'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.ExportOptions.ExpiryDate', 'Срок годности (period-of-validity-days)'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.ExportOptions.ExpiryDate', 'Expiry date (period-of-validity-days)'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.ExportOptions.ExpiryDate.ExpiryDate', 'Срок годности. Через какой период товар станет непригоден для использования (в годах, месяцах, днях, неделях или часах).'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.ExportOptions.ExpiryDate.ExpiryDate', 'Expiry date. After what period the product will become unsuitable for use (in years, months, days, weeks or hours).'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.ExportOptions.ExpiryDate.ForExample', 'Например, срок годности есть у таких категорий, как продукты питания и медицинские препараты.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.ExportOptions.ExpiryDate.ForExample', 'For example, categories such as food products and medicines have expiration dates.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.ExportOptions.ExpiryDate.ExpiryFormat', 'Должен быть указан в формате P1Y2M10D. Расшифровка примера — 1 год, 2 месяца и 10 дней.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.ExportOptions.ExpiryDate.ExpiryFormat', 'Must be specified in the format P1Y2M10D. Example decoding - 1 year, 2 months and 10 days.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.ExportOptions.ExpiryDate.OtherExamples', 'Другие примеры'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.ExportOptions.ExpiryDate.OtherExamples', 'Other examples'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.ExportOptions.ExpiryDate.FifteenDays', 'P15D — 15 дней;'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.ExportOptions.ExpiryDate.FifteenDays', 'P15D — 15 days;'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.ExportOptions.ExpiryDate.TwoYears', 'P2Y10D — 2 года, 10 дней.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.ExportOptions.ExpiryDate.TwoYears', 'P2Y10D - 2 years, 10 days.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.ExportOptions.ServiceLife', 'Срок службы (service-life-days)'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.ExportOptions.ServiceLife', 'Service life (service-life-days)'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.ExportOptions.ServiceLife.ServiceLife', 'Срок службы. В течение этого периода товар будет исправно выполнять свою функцию (в годах, месяцах или днях).'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.ExportOptions.ServiceLife.ServiceLife', 'Service life. During this period, the product will perform its function properly (in years, months or days).'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.ExportOptions.ServiceLife.ServiceFormat', 'Должен быть указан в формате P1Y2M10D. Расшифровка примера — 1 год, 2 месяца и 10 дней.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.ExportOptions.ServiceLife.ServiceFormat', 'Must be specified in the format P1Y2M10D. Example decoding - 1 year, 2 months and 10 days.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.ExportOptions.ServiceLife.OtherExamples', 'Другие примеры'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.ExportOptions.ServiceLife.OtherExamples', 'Other examples'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.ExportOptions.ServiceLife.FifteenDays', 'P15D — 15 дней;'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.ExportOptions.ServiceLife.FifteenDays', 'P15D — 15 days;'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.ExportOptions.ServiceLife.TwoYears', 'P2Y10D — 2 года, 10 дней.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.ExportOptions.ServiceLife.TwoYears', 'P2Y10D - 2 years, 10 days.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.ExportOptions.TNVEDCode', 'Код ТН ВЭД (tn-ved-code)'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.ExportOptions.TNVEDCode', 'TN VED code (tn-ved-code)'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.ExportOptions.TNVEDCode.TNVEDCode', 'Код ТН ВЭД. Состоит из 10 цифр без пробелов. Код применяется для товаров, которые ввозят на территорию Российской Федерации.
                    Код определяется Единой товарной номенклатурой ВЭД и указан в международной транспортной накладной (CMR).'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.ExportOptions.TNVEDCode.TNVEDCode', 'TN VED code. Consists of 10 digits without spaces. The code is used for goods imported into the territory of the Russian Federation.
The code is determined by the Unified Commodity Nomenclature of VED and is indicated in the international transport invoice (CMR).'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.ExportOptions.SalesQuantum', 'Квант продажи (step-quantity)'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.ExportOptions.SalesQuantum', 'Sales quantum (step-quantity)'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.ExportOptions.SalesQuantum.SalesQuantum', 'На сколько товаров можно увеличивать заказ. Например, если квант продажи равен 5, покупатель сможет добавить в корзину 5, 10,
                    15 и т. д. штук товара, а 1, 2 или 7 — не сможет. Целое число. Максимальный квант продажи — 100.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.ExportOptions.SalesQuantum.SalesQuantum', 'How many items can be added to the order. For example, if the sales quantum is 5, the buyer will be able to add 5, 10, 15, etc. items to the cart, but will not be able to add 1, 2, or 7. An integer. The maximum sales quantum is 100.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.ExportOptions.SalesQuantum.ReadMore', 'Подробнее'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.ExportOptions.SalesQuantum.ReadMore', 'Read more'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.ExportOptions.MinQuantity', 'Минимальное количество товара (min-quantity)'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.ExportOptions.MinQuantity', 'Minimum quantity of goods (min-quantity)'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.ExportOptions.MinQuantity.MinQuantity', 'Покупатели не смогут заказать меньше единиц товара, чем указано в этом поле. Целое число. Можно укзать значение не больше 30.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.ExportOptions.MinQuantity.MinQuantity', 'Customers will not be able to order fewer units of the product than specified in this field. An integer. The maximum value that can be specified is 30.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.ExportOptions.MinQuantity.ReadMore', 'Read more'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.ExportOptions.MinQuantity.ReadMore', 'Read more'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.ExportOptions.HonestSign', 'Подлежит обязательной маркировке «Честный знак»'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.ExportOptions.HonestSign', 'Subject to mandatory marking "Honest sign"'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.ExportOptions.HonestSign.ProductSubject', 'Товар подлежит'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.ExportOptions.HonestSign.ProductSubject', 'The product is subject to'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.ExportOptions.HonestSign.HonestSIGN', 'маркировке «Честный ЗНАК»'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.ExportOptions.HonestSign.HonestSIGN', 'marking "Honest SIGN"'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.ExportOptions.HonestSign.CargoTypes', 'В элементе cargo-types (YML) будет указано значение CIS_REQUIRED'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.ExportOptions.HonestSign.CargoTypes', 'The cargo-types element (YML) will have the value CIS_REQUIRED'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.ExportOptions.HonestSign.HonestSignIcon', 'Если в заказ добавлен товар с параметром "Обязательная маркировка", то в карточке заказа у этого товара будет отображаться
                        иконка "Честного Знака"'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.ExportOptions.HonestSign.HonestSignIcon', 'If a product with the "Mandatory marking" parameter is added to the order, then the "Honest Sign" icon will be displayed in the order card for this product'

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.ExportImport.MultiOrder.CustomOptions', 'Дополнительные опции'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.ExportImport.MultiOrder.CustomOptions', 'Custom options'

GO--

CREATE FUNCTION [Settings].[SPLIT_STRING](
    @string nvarchar(max), 
    @separator varchar(10))
RETURNS @returnList TABLE
(
	[value] [nvarchar](500)
)
AS
BEGIN
    DECLARE @name nvarchar(max);
    DECLARE @pos int;
    WHILE CHARINDEX(@separator, @string) > 0
    BEGIN
        SELECT @pos = CHARINDEX(@separator, @string);
        SELECT @name = SUBSTRING(@string, 1, @pos - 1);
        INSERT INTO @returnList
            SELECT @name;
        SELECT @string = SUBSTRING(@string, @pos + 1, LEN(@string) - @pos);
    END;
    INSERT INTO @returnList
        SELECT @string;
    RETURN;
END
GO--
CREATE FUNCTION [Settings].[SPLIT_INT](
    @string nvarchar(max), 
    @separator varchar(10))
RETURNS @returnList TABLE
(
	[value] [int]
)
AS
BEGIN
    DECLARE @name nvarchar(max);
    DECLARE @pos int;
    WHILE CHARINDEX(@separator, @string) > 0
    BEGIN
        SELECT @pos = CHARINDEX(@separator, @string);
        SELECT @name = SUBSTRING(@string, 1, @pos - 1);
        INSERT INTO @returnList
            SELECT convert(int, @name);
        SELECT @string = SUBSTRING(@string, @pos + 1, LEN(@string) - @pos);
    END;
    INSERT INTO @returnList
        SELECT convert(int, @string);
    RETURN;
END

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Triggers.Action.SendEmail.EmailRecipient.Customer', 'Покупатель'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Triggers.Action.SendEmail.EmailRecipient.Customer', 'Customer'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Triggers.Action.SendEmail.EmailRecipient.Other', 'Другое'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Triggers.Action.SendEmail.EmailRecipient.Other', 'Other'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Triggers.EditTrigger.RecipientIsCustomer', 'Получатель покупатель'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Triggers.EditTrigger.RecipientIsCustomer', 'Recipient is customer'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Triggers.EditTrigger.RecipientIsAnother', 'Получатель другой человек'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Triggers.EditTrigger.RecipientIsAnother', 'Recipient is another person'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Triggers.EditTrigger.EmailToReceive', 'Email получателя'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Triggers.EditTrigger.EmailToReceive', 'Recipient''s email address'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Triggers.EditTrigger.SmsToReceive', 'Телефон получателя'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Triggers.EditTrigger.SmsToReceive', 'Recipient''s phone'

GO--

IF NOT EXISTS(SELECT 1
              FROM sys.columns
              WHERE (name = N'CustomerPhone') AND object_id = OBJECT_ID(N'[CRM].[TriggerSendOnceData]'))
BEGIN
ALTER TABLE [CRM].[TriggerSendOnceData]
ADD TriggerType INT NULL,
CustomerMail VARCHAR(100) NULL,
CustomerPhone BIGINT NULL;
END;

IF NOT EXISTS(SELECT 1
              FROM sys.columns
              WHERE (name = N'AllowAddToCartInCatalog') AND object_id = OBJECT_ID(N'[Catalog].[ProductExt]'))
BEGIN
    ALTER TABLE Catalog.ProductExt ADD
        AllowAddToCartInCatalog bit NULL
END
	
GO--

ALTER PROCEDURE [Catalog].[PreCalcProductParams]
    @productId INT,
    @ModerateReviews BIT,
    @OnlyAvailable BIT,
    @ComplexFilter BIT
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @CountPhoto INT;
    DECLARE @Type NVARCHAR(10);
    DECLARE @PhotoId INT;
    DECLARE @MaxAvailable FLOAT;
    DECLARE @VideosAvailable BIT;
    DECLARE @Colors NVARCHAR(MAX);
    DECLARE @NotSamePrices BIT;
    DECLARE @MinPrice FLOAT;
    DECLARE @PriceTemp FLOAT;
    DECLARE @AmountSort BIT;
    DECLARE @OfferId INT;
    DECLARE @Comments INT;
    DECLARE @CategoryId INT;
    DECLARE @Gifts BIT;
	DECLARE @AllowAddToCartInCatalog BIT;
    IF NOT EXISTS
        (
            SELECT ProductID
            FROM [Catalog].Product
            WHERE ProductID = @productId
        )
        RETURN;
    SET @Type = 'Product';
    --@CountPhoto        
    SET @CountPhoto =
            (
                SELECT TOP (1) CASE
                                   WHEN
                                           (
                                               SELECT Offer.ColorID
                                               FROM [Catalog].[Offer]
                                               WHERE [ProductID] = @productId
                                                 AND main = 1
                                           ) IS NOT NULL AND @ComplexFilter = 1
                                       THEN
                                       (
                                           SELECT COUNT(DISTINCT PhotoId)
                                           FROM [Catalog].[Photo]
                                                    INNER JOIN [Catalog].[Offer] ON [Photo].ColorID = Offer.ColorID OR [Photo].ColorID is NULL
                                           WHERE [Photo].[ObjId] = Offer.[ProductId]
                                             AND [Offer].Main = 1
                                             AND TYPE = @Type
                                             AND Offer.[ProductId] = @productId
                                       )
                                   ELSE
                                       (
                                           SELECT COUNT(PhotoId)
                                           FROM [Catalog].[Photo]
                                           WHERE [Photo].[ObjId] = @productId
                                             AND TYPE = @Type
                                       )
                                   END
            );
    --@PhotoId        
    SET @PhotoId =
            (
                SELECT CASE
                           WHEN
                               (
                                   SELECT Offer.ColorID
                                   FROM [Catalog].[Offer]
                                   WHERE [ProductID] = @productId
                                     AND main = 1
                               ) IS NOT NULL
                               THEN
                               (
                                   SELECT TOP (1) PhotoId
                                   FROM [Catalog].[Photo]
                                            INNER JOIN [Catalog].[Offer] ON Offer.[ProductId] = @productId AND ([Photo].ColorID = Offer.ColorID OR [Photo].ColorID is NULL)
                                   WHERE([Photo].ColorID = Offer.ColorID
                                       OR [Photo].ColorID IS NULL)
                                     AND [Photo].[ObjId] = @productId
                                     AND Type = @Type
                                   ORDER BY [Photo]. main DESC,
                                            [Photo].[PhotoSortOrder],
                                            [PhotoId]
                               )
                           ELSE
                               (
                                   SELECT TOP (1) PhotoId
                                   FROM [Catalog].[Photo]
                                   WHERE [Photo].[ObjId] = @productId
                                     AND Type = @Type
                                   ORDER BY main DESC,
                                            [Photo].[PhotoSortOrder],
                                            [PhotoId]
                               )
                           END
            );
    --VideosAvailable        
    IF (SELECT COUNT(ProductVideoID) FROM [Catalog].[ProductVideo] WHERE ProductID = @productId) > 0
        BEGIN
            SET @VideosAvailable = 1;
        END;
    ELSE
        BEGIN
            SET @VideosAvailable = 0;
        END;
    --@MaxAvailable        
    SET @MaxAvailable = (SELECT MAX(Offer.Amount) FROM [Catalog].Offer WHERE ProductId = @productId);
    --AmountSort        
    SET @AmountSort =
            (
                SELECT CASE
                           WHEN @MaxAvailable <= 0
                               OR @MaxAvailable < ISNULL(Product.MinAmount, 0)
                               THEN 0
                           ELSE 1
                           END
                FROM [Catalog].Offer
                         INNER JOIN [Catalog].Product ON Product.ProductId = Offer.ProductId
                WHERE Offer.ProductId = @productId
                  AND main = 1
            );
    --Colors        
    SET @Colors =
            (
                SELECT [Settings].[ProductColorsToString](@productId, @OnlyAvailable)
            );
    --@NotSamePrices        
    IF
            (
                SELECT MAX(price) - MIN(price)
                FROM [Catalog].offer
                         INNER JOIN [Catalog].Product ON Product.ProductId = Offer.ProductId
                WHERE offer.productid = @productId AND
                        price > 0 AND
                    (@OnlyAvailable = 0 OR amount > 0 OR AllowPreOrder = 1)
            ) > 0
        BEGIN
            SET @NotSamePrices = 1;
        END;
    ELSE
        BEGIN
            SET @NotSamePrices = 0;
        END;
    --@MinPrice        
    SET @MinPrice =
            (
                SELECT isNull(MIN(price), 0)
                FROM [Catalog].offer
                         INNER JOIN [Catalog].Product ON Product.ProductId = Offer.ProductId
                WHERE offer.productid = @productId AND
                        price > 0 AND
                    (@OnlyAvailable = 0 OR amount > 0 OR AllowPreOrder = 1)
            );
    --@OfferId      
    SET @OfferId =
            (
                SELECT OfferID
                FROM [Catalog].offer
                WHERE offer.productid = @productId AND (offer.Main = 1 OR offer.Main IS NULL)
            );
    --@PriceTemp        
    SET @PriceTemp =
            (
                SELECT CASE WHEN [Product].Discount > 0 THEN (@MinPrice - @MinPrice * [Product].Discount / 100) * CurrencyValue ELSE (@MinPrice - isnull([Product].DiscountAmount, 0)) * CurrencyValue END
                FROM Catalog.Product
                         INNER JOIN Catalog.Currency ON Product.Currencyid = Currency.Currencyid
                WHERE Product.Productid = @productId
            );
    --@Comments      
    SET @Comments =
            (
                SELECT COUNT(ReviewId)
                FROM CMS.Review
                WHERE EntityId = @productId AND (Checked = 1 OR @ModerateReviews = 0)
            );
    --@Gifts      
    SET @Gifts =
            (
                SELECT TOP (1) CASE
                                   WHEN COUNT([ProductGifts].ProductID) > 0
                                       THEN 1
                                   ELSE 0
                                   END
                FROM [Catalog].[ProductGifts]
                         INNER JOIN Catalog.Offer on ProductGifts.GiftOfferId = Offer.OfferId
                         INNER JOIN Catalog.Product on Offer.ProductId = Product.ProductId
                WHERE [ProductGifts].ProductID = @productId  and Offer.Amount > ISNULL(Product.MinAmount, 0) and Enabled = 1
            );
	SET @AllowAddToCartInCatalog = 
			(
				CASE WHEN (
							(SELECT COUNT(OfferId) FROM Catalog.Offer WHERE Offer.ProductId = @productId) = 1 
                                and not Exists (SELECT 1 FROM [Catalog].[CustomOptions] WHERE ProductID = @productId)
							) THEN 1 
					 ELSE 0
					 END
			);
    IF
            (
                SELECT COUNT(productid)
                FROM [Catalog].ProductExt
                WHERE productid = @productId
            ) > 0
        BEGIN
            UPDATE [Catalog].[ProductExt]
            SET
                [CountPhoto] = @CountPhoto,
                [PhotoId] = @PhotoId,
                [VideosAvailable] = @VideosAvailable,
                [MaxAvailable] = @MaxAvailable,
                [NotSamePrices] = @NotSamePrices,
                [MinPrice] = @MinPrice,
                [Colors] = @Colors,
                [AmountSort] = @AmountSort,
                [OfferId] = @OfferId,
                [Comments] = @Comments,
                [PriceTemp] = @PriceTemp,
                [Gifts] = @Gifts,
				[AllowAddToCartInCatalog] = @AllowAddToCartInCatalog
            WHERE [ProductId] = @productId;
        END;
    ELSE
        BEGIN
            INSERT INTO [Catalog].[ProductExt]
            ([ProductId],
             [CountPhoto],
             [PhotoId],
             [VideosAvailable],
             [MaxAvailable],
             [NotSamePrices],
             [MinPrice],
             [Colors],
             [AmountSort],
             [OfferId],
             [Comments],
             [PriceTemp],
             [Gifts],
			 [AllowAddToCartInCatalog]
            )
            VALUES
                (@productId,
                 @CountPhoto,
                 @PhotoId,
                 @VideosAvailable,
                 @MaxAvailable,
                 @NotSamePrices,
                 @MinPrice,
                 @Colors,
                 @AmountSort,
                 @OfferId,
                 @Comments,
                 @PriceTemp,
                 @Gifts,
				 @AllowAddToCartInCatalog
            );
        END;
END;

GO--

ALTER PROCEDURE [Catalog].[PreCalcProductParamsMass] @ModerateReviews BIT, @OnlyAvailable bit,  @ComplexFilter BIT AS
BEGIN
    INSERT INTO [Catalog].[ProductExt] (ProductId, CountPhoto, PhotoId, VideosAvailable, MaxAvailable, NotSamePrices, MinPrice, Colors, AmountSort, OfferId, Comments) 
	(
        SELECT ProductId, 0, NULL, 0, 0, 0, 0, NULL, 0, NULL, 0
        FROM [Catalog].Product
        WHERE Product.ProductId NOT IN (SELECT ProductId FROM [Catalog].[ProductExt])
    )
    UPDATE
        catalog.ProductExt
    SET
        ProductExt.[CountPhoto] = tempTable.CountPhoto,
        ProductExt.[PhotoId] = tempTable.[PhotoId],
        ProductExt.[VideosAvailable] = tempTable.[VideosAvailable],
        ProductExt.[MaxAvailable] = tempTable.[MaxAvailable],
        ProductExt.[NotSamePrices] = tempTable.[NotSamePrices],
        ProductExt.[MinPrice] = tempTable.[MinPrice],
        ProductExt.[OfferId] = tempTable.[OfferId],
        ProductExt.[Comments] = tempTable.[Comments],
        ProductExt.[Gifts] = tempTable.[Gifts],
        ProductExt.[Colors] = tempTable.[Colors],
        --ProductExt.[CategoryId] = tempTable.[CategoryId] ,
        ProductExt.PriceTemp = tempTable.PriceTemp,
        ProductExt.AmountSort=tempTable.AmountSort,
		ProductExt.AllowAddToCartInCatalog = tempTable.AllowAddToCartInCatalog
    FROM
        catalog.ProductExt
            INNER JOIN
        (
            select
                pe.ProductId,
                CountPhoto=case when offerId.ColorID is null OR @ComplexFilter = 0 then countNocolor.countNocolor else countColor.countColor end,
                PhotoId=case when offerId.ColorID is null then PhotoIdNoColor.PhotoIdNoColor else PhotoIdColor.PhotoIdColor end,
                [VideosAvailable]=isnull(videosAvailable.videosAvailable,0),
                [MaxAvailable]=maxAvailable.maxAvailable,
                [NotSamePrices]=isnull(notSamePrices.notSamePrices,0),
                [MinPrice]=isnull(minPrice.minPrice,0),
                [OfferId]=offerId.OfferId,
                [Comments]=isnull(comments.comments,0),
                [Gifts]=isnull(gifts.gifts,0),
                [Colors]=(SELECT [Settings].[ProductColorsToString](pe.ProductId, @OnlyAvailable)),
                --[CategoryId] = (SELECT TOP 1 id	FROM [Settings].[GetParentsCategoryByChild](( SELECT TOP 1 CategoryID FROM [Catalog].ProductCategories	WHERE ProductID = pe.ProductId ORDER BY Main DESC))ORDER BY sort DESC),
                PriceTemp = CASE WHEN p.Discount > 0 THEN (isnull(minPrice.minPrice,0) - isnull(minPrice.minPrice,0) * p.Discount/100)*c.CurrencyValue ELSE (isnull(minPrice.minPrice,0) - isnull(p.DiscountAmount,0))*c.CurrencyValue END,
                AmountSort=CASE when ISNULL(maxAvailable.maxAvailable, 0) <= 0 OR maxAvailable.maxAvailable < IsNull(p.MinAmount, 0) THEN 0 ELSE 1 end,
				AllowAddToCartInCatalog = (CASE WHEN ((SELECT COUNT(OfferId) FROM Catalog.Offer WHERE Offer.ProductId = pe.ProductId) = 1 
													   and not Exists (SELECT 1 FROM [Catalog].[CustomOptions] WHERE ProductID = pe.ProductId)
													 ) THEN 1 
												ELSE 0 END)
            from Catalog.[ProductExt] pe
                     left join (
									SELECT o.ProductId, COUNT(*) countColor FROM [Catalog].[Photo] ph INNER JOIN [Catalog].[Offer] o  ON ph.[ObjId] = o.ProductId
									WHERE ( ph.ColorID = o.ColorID OR ph.ColorID IS NULL ) AND TYPE = 'Product' AND o.Main = 1
									group by o.ProductId
								) countColor on pe.ProductId=countColor.ProductId
                     left join (
									SELECT [ObjId], COUNT(*) countNocolor FROM [Catalog].[Photo]
									WHERE TYPE = 'Product'
									group by [ObjId]
								) countNocolor on pe.ProductId=countNocolor.[ObjId]
                     left join (
									select ProductId, PhotoId PhotoIdColor 
									from (
										   SELECT o.ProductId, ph.PhotoId, Row_Number() over (PARTITION  by o.ProductId ORDER BY ph.main DESC ,ph.[PhotoSortOrder], ph.[PhotoId]) rn 
										   FROM [Catalog].[Photo] ph 
										   INNER JOIN [Catalog].[Offer] o ON ph.[ObjId] = o.ProductId
										   WHERE (ph.ColorID = o.ColorID OR ph.ColorID IS NULL) AND TYPE = 'Product' 
										 ) ct 
									where rn=1
								) PhotoIdColor on pe.ProductId=PhotoIdColor.ProductId
                     left join (
									select ProductId, PhotoId PhotoIdNoColor 
									from (
											SELECT ph.[ObjId] ProductId, ph.PhotoId, Row_Number() over (PARTITION  by ph.[ObjId] ORDER BY ph.main DESC ,ph.[PhotoSortOrder], ph.[PhotoId]) rn 
											FROM [Catalog].[Photo] ph	
											WHERE TYPE = 'Product' 
										 ) ct 
									where rn=1
								) PhotoIdNoColor on pe.ProductId=PhotoIdNoColor.ProductId
                     left join (
									select pv.ProductID, CASE WHEN COUNT(pv.ProductVideoID) > 0 THEN 1	ELSE 0 END videosAvailable 
									FROM [Catalog].[ProductVideo] pv 
									group by pv.ProductID
								) videosAvailable on pe.ProductId=videosAvailable.ProductId
                     left join (
									select o.ProductID,Max(o.Amount) maxAvailable  FROM [Catalog].Offer o group by o.ProductID
								) maxAvailable on pe.ProductId=maxAvailable.ProductId
                     left join (
									select o.ProductID, CASE WHEN MAX(o.price) - MIN(o.price) > 0 THEN 1 ELSE 0 END notSamePrices  
									FROM [Catalog].Offer o 
									Inner Join [Catalog].Product On Product.ProductId = o.ProductID 
									where o.price > 0 AND (@OnlyAvailable = 0 OR o.amount > 0 OR AllowPreOrder = 1) 
									group by o.ProductID
								) notSamePrices on pe.ProductId=notSamePrices.ProductId
                     left join (
									select o.ProductID,MIN(o.price) minPrice 
									FROM [Catalog].Offer o 
									Inner Join [Catalog].Product On Product.ProductId = o.ProductID 
									where o.price > 0 AND (@OnlyAvailable = 0 OR o.amount > 0 OR AllowPreOrder = 1)  
									group by o.ProductID
								) minPrice on pe.ProductId=minPrice.ProductId
                     
					 left join (
									select ProductId, OfferID, colorId 
									from (
											select o.ProductID,o.OfferID, o.colorId, Row_Number() over (PARTITION  by o.OfferID ORDER BY o.OfferID) rn  
											FROM [Catalog].Offer o where o.Main = 1 
										 )ct 
									where rn=1
								) offerId on pe.ProductId=offerId.ProductId
                     left join (
									select EntityId ProductID,count(ReviewId) comments  
									FROM CMS.Review  
									where (Checked = 1 OR @ModerateReviews = 0) 
									group by EntityId
								) comments on pe.ProductId=comments.ProductId
                     left join (
									select pg.ProductID, CASE WHEN COUNT(pg.ProductID) > 0 THEN 1 ELSE 0 END gifts 
									FROM [Catalog].[ProductGifts] pg 
									INNER JOIN Catalog.Offer on pg.GiftOfferId = Offer.OfferId 
									INNER JOIN Catalog.Product on Offer.ProductId = Product.ProductId 
									WHERE Offer.Amount > ISNULL(Product.MinAmount, 0) and Enabled = 1 
									group by pg.ProductID
								) gifts on pe.ProductId=gifts.ProductId
                     inner join Catalog.Product p on p.ProductID = pe.ProductID
                     inner join Catalog.Currency c ON p.CurrencyId = c.CurrencyId
        )
            AS tempTable
        ON tempTable.ProductId = ProductExt.ProductId
END

GO--

if Exists (Select 1 From  Catalog.ProductExt Where AllowAddToCartInCatalog is null)
begin
    Update Catalog.ProductExt 
    Set AllowAddToCartInCatalog = (CASE WHEN ((SELECT COUNT(OfferId) FROM Catalog.Offer WHERE Offer.ProductId = pe.ProductId) = 1 
                                                and not Exists (SELECT 1 FROM [Catalog].[CustomOptions] WHERE ProductID = pe.ProductId)
                                                ) THEN 1 
                                        ELSE 0 END)
	From Catalog.ProductExt pe
end

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Bonuses.ERule.CancellationsBonus', 'Аннулирование бонусов'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Bonuses.ERule.CancellationsBonus', 'Cancellations Bonus'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Bonuses.ERule.NewCard', 'Начисление бонусов при получении карты'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Bonuses.ERule.NewCard', 'Receiving bonuses on receipt of a card'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Bonuses.ERule.PostingReview', 'Начисление бонусов при размещении отзыва'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Bonuses.ERule.PostingReview', 'Bonuses for posting a review'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Bonuses.ESmsType.OnCancellationsBonus', 'Аннулирование бонусов'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Bonuses.ESmsType.OnCancellationsBonus', 'Cancellations Bonus'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Cards.AllTransaction.AdditionPoints', 'Дополнительные бонусы'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Cards.AllTransaction.AdditionPoints', 'Addition bonuses'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Cards.AllTransaction.MainPoints', 'Основные бонусы'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Cards.AllTransaction.MainPoints', 'Main bonuses'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Cards.AllTransaction.Points', 'Бонусы'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Cards.AllTransaction.Points', 'Bonuses'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Cards.LastTransaction.AdditionPoints', 'Дополнительные бонусы'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Cards.LastTransaction.AdditionPoints', 'Additional bonuses'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Cards.LastTransaction.MainPoints', 'Основные бонусы'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Cards.LastTransaction.MainPoints', 'Main bonuses'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Cards.LastTransaction.Points', 'Бонусы'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Cards.LastTransaction.Points', 'Bonuses'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Orders.ClientBonusCard.CreateUser', 'Создайте пользователя, чтобы начислить бонусы'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Orders.ClientBonusCard.CreateUser', 'Create a user to credit bonuses'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Orders.ClientBonusCard.Points', 'Бонусы'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Orders.ClientBonusCard.Points', 'Bonuses'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Product.Edit.AddPointsForProduct', 'Начислять бонусы за товар'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Product.Edit.AddPointsForProduct', 'Add bonuses for a product'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsBonus.Index.AllowSpecifyBonusAmount', 'Давать покупателю возможность выбирать кол-во бонусов для списания'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsBonus.Index.AllowSpecifyBonusAmount', 'Give the buyer the opportunity to choose the number of bonuses to be debited'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'AdminMobile.Orders.OrderItem.BonusCardAmount', 'Бонусов к зачислению'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'AdminMobile.Orders.OrderItem.BonusCardAmount', 'Bonuses'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Catalog.Product.AccrueBonuses', 'Начислять бонусы за товар'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Catalog.Product.AccrueBonuses', 'Add bonuses for a product'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.Bonus.CardBonuses', 'Бонусы карты'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.Bonus.CardBonuses', 'Bonuses card'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.Cart.BonusPlus', 'Сумма бонусов начисляемых на бонусную карту'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.Cart.BonusPlus', 'The sum of bonuses earned on a bonus card'

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.Landing.Cart.Return', 'Вернуться'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.Landing.Cart.Return', 'Return'

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Users.TwoFactorAuth', 'Двухфакторная аутентификация'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Users.TwoFactorAuth', 'Two-factor authentication'

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Users.EnableTwoFactor', 'Включить двухфакторную аутентификацию'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Users.EnableTwoFactor', 'Enable two-factor authentication'

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Users.Email', 'E-mail'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Users.Email', 'E-mail'

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Users.Password', 'Пароль'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Users.Password', 'Password'

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Users.GetCode', 'Получить код для входа'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Users.GetCode', 'Get login code'

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Users.EnterKeyManually', 'Если нет возможности отсканировать qr, вы можете ввести ключ вручную в приложении:'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Users.EnterKeyManually', 'If it is not possible to scan the qr, you can enter the key manually in the application:'

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'User.Login.WrongCode', 'Неверный код'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'User.Login.WrongCode', 'Wrong code'

GO--

SET IDENTITY_INSERT [Order].[ShippingReplaceGeo] ON 
IF NOT EXISTS (SELECT * FROM [Order].[ShippingReplaceGeo] WHERE [Id] = 51)
INSERT INTO [Order].[ShippingReplaceGeo] ([Id],[ShippingType],[InCountryName],[InCountryISO2],[InRegionName],[InCityName],[InDistrict],[OutCountryName],[OutRegionName],[OutCityName],[OutDistrict],[OutDistrictClear],[Enabled],[Sort],[InZip],[OutZip],[Comment])
VALUES (51,'Boxberry','','RU','Республика Саха (Якутия)','','','','Республика Саха','','',0,1,0,'','','')
SET IDENTITY_INSERT [Order].[ShippingReplaceGeo] OFF

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Design.Index.ResizeGoodsCategoryBigPictures', 'Пережать большие фотографии категории'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Design.Index.ResizeGoodsCategoryBigPictures', 'Resize big category picture'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Design.Index.ResizeGoodsCategorySmallPictures', 'Пережать маленькие фотографии категории'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Design.Index.ResizeGoodsCategorySmallPictures', 'Resize small category picture'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Design.DoYouWantResizeCategoryPhotosTitle', 'Пережатие фотографий категории'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Design.DoYouWantResizeCategoryPhotosTitle', 'Resize category picture'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Design.DoYouWantResizeCategoryPhotosText', 'Вы действительно хотите пережать фотографии категории?'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Design.DoYouWantResizeCategoryPhotosText', 'Do you really want to resize category pictures?'

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.Builder.ResizeCategoryPictures', 'Пережать фотографии'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.Builder.ResizeCategoryPictures', 'Resize pictures'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.Design.DoYouWantResizeCategoryPhotosTitle', 'Пережатие фотографий категории'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.Design.DoYouWantResizeCategoryPhotosTitle', 'Resize category picture'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.Design.DoYouWantResizeCategoryPhotosText', 'Вы действительно хотите пережать фотографии категории?'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.Design.DoYouWantResizeCategoryPhotosText', 'Do you really want to resize category pictures?'

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Customers.RoleActionCategory.CouponsAndDiscounts', 'Купоны и скидки'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Customers.RoleActionCategory.CouponsAndDiscounts', 'Coupons and discounts'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Customers.RoleActionCategory.Api', 'API'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Customers.RoleActionCategory.Api', 'API'

GO--
               
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Services.Landing.Templates.LpTemplate.OnlineStore', 'Интернет-магазин'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Services.Landing.Templates.LpTemplate.OnlineStore', 'Online store'
               
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Services.Landing.Templates.LpTemplate.ProductFunnels', 'Товарные воронки'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Services.Landing.Templates.LpTemplate.ProductFunnels', 'Product funnels'
               
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Services.Landing.Templates.LpTemplate.LeadCollection', 'Сбор лидов'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Services.Landing.Templates.LpTemplate.LeadCollection', 'Lead collection'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Services.Landing.Templates.LpTemplate.Questionnaires', 'Опросники'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Services.Landing.Templates.LpTemplate.Questionnaires', 'Questionnaires'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Services.Landing.Templates.LpTemplate.PresentationFunnels', 'Презентационные воронки'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Services.Landing.Templates.LpTemplate.PresentationFunnels', 'Presentation Funnels'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Services.Landing.Templates.LpTemplate.Landings', 'Лендинги'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Services.Landing.Templates.LpTemplate.Landings', 'Landings'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateTemplate.Back', 'Назад'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateTemplate.Back', 'Back'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.Back', 'Назад'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.Back', 'Back'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Templates.Common.Header.Back', 'Назад'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Templates.Common.Header.Back', 'Back'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Content.QrCodeGenerator.QrCodeGenerator', 'QR код сайта'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Content.QrCodeGenerator.QrCodeGenerator', 'QR code of the site'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Content.QrCodeGenerator.FailedGenerateQRCode', 'Не удалось сгенерировать QR код'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Content.QrCodeGenerator.FailedGenerateQRCode', 'Failed to generate QR code'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Content.QrCodeGenerator.Download', 'Скачать'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Content.QrCodeGenerator.Download', 'Download'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Content.QrCodeGenerator.Close', 'Закрыть'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Content.QrCodeGenerator.Close', 'Close'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateSiteTemplateItem.OnlineDemo', 'Онлайн демо'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateSiteTemplateItem.OnlineDemo', 'Online demo'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.OnlineDemo', 'Онлайн демо'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.OnlineDemo', 'Online demo'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.CreateFunnel', 'Создать воронку'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.CreateFunnel', 'Create Funnel'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.FunnelCreation', 'Создание воронки'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.FunnelCreation', 'Funnel creation'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.Description', 'Описание:'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.Description', 'Description:'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.Goals', 'Цели:'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.Goals', 'Goals:'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.Advice', 'Совет:'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.Advice', 'Advice:'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.VideoReviewFunnel', 'Видео обзор воронки'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.VideoReviewFunnel', 'Video review of the funnel'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.FunnelDiagram', 'Схема воронки'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.FunnelDiagram', 'Funnel diagram'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.FunnelPages', 'Страницы воронки'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.FunnelPages', 'Funnel Pages'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.OfferPages', 'Страница предложения'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.OfferPages', 'Offer page'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Funnels.Site.MySites', 'Мои сайты'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Funnels.Site.MySites', 'My sites'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Funnels.Site.CreateCopyFunnel', 'Создать копию воронки'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Funnels.Site.CreateCopyFunnel', 'Create a copy of the funnel'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Funnels.Site.GoWebsite', 'Перейти на сайт'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Funnels.Site.GoWebsite', 'Go to website'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Funnels.Site.Funnel', 'Воронка:'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Funnels.Site.Funnel', 'Funnel:'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Funnels.Site.Published', 'Опубликован'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Funnels.Site.Published', 'Published'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Funnels.Site.NotPublished', 'Не опубликован'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Funnels.Site.NotPublished', 'Not published'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Funnels.Site.VisibleFunnelPublished', 'Когда воронка опубликована, ее видят все пользователи и появляется в карте сайта. Иначе воронку видит только администратор.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Funnels.Site.VisibleFunnelPublished', 'When a funnel is published, it is visible to all users and appears in the sitemap. Otherwise, only the administrator can see the funnel.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Funnels.Site.ConnectYourDomain', 'Подключить свой домен'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Funnels.Site.ConnectYourDomain', 'Connect your domain'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Funnels.Site.Pages', 'Страницы'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Funnels.Site.Pages', 'Pages'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Funnels.Site.EmailChains', 'Email цепочки'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Funnels.Site.EmailChains', 'Email chains'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Funnels.Site.Leads', 'Лиды'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Funnels.Site.Leads', 'Leads'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Funnels.Site.Orders', 'Заказы'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Funnels.Site.Orders', 'Orders'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Funnels.Site.Armor', 'Брони'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Funnels.Site.Armor', 'Booking'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Funnels.Site.Settings', 'Настройки'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Funnels.Site.Settings', 'Settings'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Funnels.Site.CreatePage', 'Создать страницу'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Funnels.Site.CreatePage', 'Create a page'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Funnels.Site.FunnelFunnel', 'Воронка'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Funnels.Site.FunnelFunnel', 'Funnel'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Funnels.Site.Rename', 'Переименовать'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Funnels.Site.Rename', 'Rename'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Funnels.Site.Publish', 'Опубликовать'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Funnels.Site.Publish', 'Publish'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Funnels.Site.RemoveFromPublication', 'Снять с публикации'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Funnels.Site.RemoveFromPublication', 'Remove from publication'
              EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Funnels.Site.Delete', 'Удалить'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Funnels.Site.Delete', 'Delete'
              EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Funnels.Site.Name', 'Название'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Funnels.Site.Name', 'Name'
              EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Funnels.Site.Home', 'Главная'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Funnels.Site.Home', 'Home'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Funnels.FunnelData.Emails.TimeInterval', 'Временной интервал:'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Funnels.FunnelData.Emails.TimeInterval', 'Time interval:'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Funnels.FunnelData.Emails.NewChainLetters', 'Новая цепочка писем'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Funnels.FunnelData.Emails.NewChainLetters', 'New chain of letters'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Funnels.FunnelData.Emails.NoChainsConfigured', 'Нет настроенных цепочек'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Funnels.FunnelData.Emails.NoChainsConfigured', 'No chains configured'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Funnels.FunnelData.Emails.SubjectLetter', 'Тема письма'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Funnels.FunnelData.Emails.SubjectLetter', 'Subject of the letter'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Funnels.FunnelData.Emails.ConnectStatistics', 'Подключить статистику'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Funnels.FunnelData.Emails.ConnectStatistics', 'Connect statistics'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Funnels.FunnelData.Emails.Delete', 'Удалить'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Funnels.FunnelData.Emails.Delete', 'Delete'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Funnels.FunnelData.Emails.Edit', 'Редактировать'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Funnels.FunnelData.Emails.Edit', 'Edit'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Reports.ProductOrdersStatistics.OrdersFound', 'Найдено заказов: {0} на сумму {1}'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Reports.ProductOrdersStatistics.OrdersFound', 'Orders found: {0} for a total of {1}'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Handlers.Orders.GetOrders.OrdersFound', 'Найдено заказов: {0} на сумму {1}'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Handlers.Orders.GetOrders.OrdersFound', 'Orders found: {0} for a total of {1}'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Handlers.Marketing.Analytics.Reports.OfferOrdersStatistics.OrdersFound', 'Найдено заказов: {0} на сумму {1}'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Handlers.Marketing.Analytics.Reports.OfferOrdersStatistics.OrdersFound', 'Orders found: {0} for a total of {1}'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Views.Funnels.Settings.Common.FunnelName', 'Название воронки'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Views.Funnels.Settings.Common.FunnelName', 'Funnel name'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Views.Funnels.Settings.Common.FunnelNameExample', 'Например: Корм для кошек, Порошковая краска и т.п.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Views.Funnels.Settings.Common.FunnelNameExample', 'For example: Cat food, Powder paint, etc.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Views.Funnels.SiteSettings.Settings', 'Настройки'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Views.Funnels.SiteSettings.Settings', 'Settings'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Views.Funnels.SiteSettings.General', 'Общие'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Views.Funnels.SiteSettings.General', 'General'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Views.Funnels.SiteSettings.Domains', 'Домены'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Views.Funnels.SiteSettings.Domains', 'Domains'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Views.Funnels.SiteSettings.Counters', 'Счетчики'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Views.Funnels.SiteSettings.Counters', 'Counters'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Views.Funnels.SiteSettings.CSSStyles', 'CSS стили'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Views.Funnels.SiteSettings.CSSStyles', 'CSS styles'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Views.Funnels.SiteSettings.SitemapRobots', 'Карта сайта и robots.txt'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Views.Funnels.SiteSettings.SitemapRobots', 'Sitemap and robots.txt'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Views.Funnels.SiteSettings.Upselling', 'Допродажи'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Views.Funnels.SiteSettings.Upselling', 'Upselling'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Views.Funnels.SiteSettings.Access', 'Доступ'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Views.Funnels.SiteSettings.Access', 'Access'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Views.Funnels.SiteSettings.MobileApp', 'Мобильное приложение'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Views.Funnels.SiteSettings.MobileApp', 'Mobile application'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Views.Funnels.Settings.Domains.FunnelInternalURL', 'Внутренний урл воронки'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Views.Funnels.Settings.Domains.FunnelInternalURL', 'Funnel Internal URL'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Views.Funnels.Settings.Domains.IfDomainNotLinked', 'Если не привязан домен, то воронка отображается по внутреннему урл адресу'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Views.Funnels.Settings.Domains.IfDomainNotLinked', 'If the domain is not linked, the funnel is displayed at the internal URL address'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Views.Funnels.Settings.Domains.FunnelDomainSubdomain', 'Домен (или поддомен) воронки'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Views.Funnels.Settings.Domains.FunnelDomainSubdomain', 'Funnel domain (or subdomain)'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Views.Funnels.Settings.Domains.AddedDomains', 'Добавленные домены'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Views.Funnels.Settings.Domains.AddedDomains', 'Added domains'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Views.Funnels.Settings.Domains.ManageDomains', 'Управлять доменами'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Views.Funnels.Settings.Domains.ManageDomains', 'Manage domains'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Views.Funnels.Settings.Domains.AddDomain', 'Добавить домен'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Views.Funnels.Settings.Domains.AddDomain', 'Add domain'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Views.Funnels.Settings.Domains.BuyDomainTextOne', 'Вы можете приобрести новый домен прямо из панели администрирования.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Views.Funnels.Settings.Domains.BuyDomainTextOne', 'You can purchase a new domain directly from the admin panel.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Views.Funnels.Settings.Domains.BuyDomainTextTwo', 'Домен будет юридически оформлен на Вас, все настройки домена будут осуществлены автоматически.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Views.Funnels.Settings.Domains.BuyDomainTextTwo', 'The domain will be legally registered to you, all domain settings will be performed automatically.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Views.Funnels.Settings.Domains.BuyDomainTextThree', 'Магазин начнет открываться по новому домену в течение'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Views.Funnels.Settings.Domains.BuyDomainTextThree', 'The store will start opening on the new domain within'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Views.Funnels.Settings.Domains.BuyDomainCost', 'Купить домен - от 199 руб'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Views.Funnels.Settings.Domains.BuyDomainCost', 'Buy a domain - from 199 rubles'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Views.Funnels.Settings.Domains.BuyDomain', 'Купить домен'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Views.Funnels.Settings.Domains.BuyDomain', 'Buy a domain'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Views.Funnels.Settings.Domains.BuyDomainTextFour', '2 часов'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Views.Funnels.Settings.Domains.BuyDomainTextFour', '2 hours'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Views.Funnels.Settings.Domains.BuyDomainTextFive', 'после оплаты.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Views.Funnels.Settings.Domains.BuyDomainTextFive', 'after payment.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Views.Funnels.Settings.Domains.HaveDomainSubdomain', 'У меня есть домен / поддомен'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Views.Funnels.Settings.Domains.HaveDomainSubdomain', 'I have a domain/subdomain'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Views.Funnels.Settings.Domains.HaveDomainSubdomainTextOne', 'Если у Вас есть ранее приобретённый домен, Вы можете использовать его.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Views.Funnels.Settings.Domains.HaveDomainSubdomainTextOne', 'If you have a previously purchased domain, you can use it.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Views.Funnels.Settings.Domains.HaveDomainSubdomainTextTwo', 'После привязки домена Вам будет необходимо прописать настройки домена на стороне регистратора. Инструкция прилагается.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Views.Funnels.Settings.Domains.HaveDomainSubdomainTextTwo', 'After linking the domain, you will need to register the domain settings on the registrars side. Instructions are attached.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Views.Funnels.Settings.Domains.HaveDomainSubdomainTextThree', 'Магазин начнёт открываться по новому адресу через'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Views.Funnels.Settings.Domains.HaveDomainSubdomainTextThree', 'The store will begin opening at the new address in'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Views.Funnels.Settings.Domains.HaveDomainSubdomainTextFour', '12-24 часа'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Views.Funnels.Settings.Domains.HaveDomainSubdomainTextFour', '12-24 hours'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Views.Funnels.Settings.Domains.HaveDomainSubdomainTextFive', 'после изменения настроек на стороне регистратора.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Views.Funnels.Settings.Domains.HaveDomainSubdomainTextFive', 'after changing the settings on the registrar side.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Views.Funnels.Settings.Domains.HaveDomainSubdomainTextSix', 'Инструкции, как прописать NS'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Views.Funnels.Settings.Domains.HaveDomainSubdomainTextSix', 'Instructions on how to register NS'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Views.Funnels.Settings.Domains.HaveDomainSubdomainTextSeven', 'В случае с поддоменом, следуйте инструкции в разделе "Все домены"'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Views.Funnels.Settings.Domains.HaveDomainSubdomainTextSeven', 'In case of a subdomain, follow the instructions in the "All domains" section'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Views.Funnels.Settings.Domains.ConnectDomain', 'Подключить домен'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Views.Funnels.Settings.Domains.ConnectDomain', 'Connect domain'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Views.Funnels.Settings.Domains.UsePreviouslyDomain', 'Использовать ранее подключенный домен'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Views.Funnels.Settings.Domains.UsePreviouslyDomain', 'Use a previously connected domain'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Views.Funnels.Settings.Domains.UsePreviouslyDomainTextOne', 'Если ранее вы уже подключили домен или поддомен, то вы можете переключить его использование для данной воронки.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Views.Funnels.Settings.Domains.UsePreviouslyDomainTextOne', 'If you have previously connected a domain or subdomain, you can switch its use for this funnel.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Views.Funnels.Settings.Domains.UsePreviouslyDomainTextTwo', 'Изменение привязки происходит быстро, в течение 1 минуты уже будет открываться новое содержимое.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Views.Funnels.Settings.Domains.UsePreviouslyDomainTextTwo', 'Changing the binding happens quickly, within 1 minute the new content will already be opening.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Views.Funnels.Settings.Domains.ChangeBinding', 'Изменить привязку'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Views.Funnels.Settings.Domains.ChangeBinding', 'Change binding'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Views.Funnels.Settings.Domains.UsePreviouslyAdded', 'Использовать ранее добавленный'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Views.Funnels.Settings.Domains.UsePreviouslyAdded', 'Use previously added'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Views.Funnels.Settings.Domains.New', 'Новый'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Views.Funnels.Settings.Domains.New', 'New'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Views.Funnels.Settings.Domains.EnterDomainNameSubdomain', 'Введите доменное имя или поддомен'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Views.Funnels.Settings.Domains.EnterDomainNameSubdomain', 'Enter a domain name or subdomain'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Views.Funnels.Settings.Domains.Add', 'Добавить'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Views.Funnels.Settings.Domains.Add', 'Add'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Views.Funnels.Settings.Domains.PleaseNote', 'Обратите внимание:'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Views.Funnels.Settings.Domains.PleaseNote', 'Please note:'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Views.Funnels.Settings.Domains.PleaseNoteTextOne', 'Домен/поддомен следует добавить без приставки www или http. Например: moysite.ru, мойсайт.рф или lp.moysite.ru.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Views.Funnels.Settings.Domains.PleaseNoteTextOne', 'The domain/subdomain should be added without the www or http prefix. For example: moysite.ru, мойсайт.рф or lp.moysite.ru.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Views.Funnels.Settings.Domains.PleaseNoteTextTwo', 'Данный список лишь указывает, какой домен используется для воронки, сам домен должен быть куплен и привязан к сайту заранее.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Views.Funnels.Settings.Domains.PleaseNoteTextTwo', 'This list only indicates which domain is used for the funnel, the domain itself must be purchased and linked to the site in advance.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Views.Funnels.Settings.Domains.SelectDomainNameSubdomain', 'Выберите доменное имя или поддомен'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Views.Funnels.Settings.Domains.SelectDomainNameSubdomain', 'Select a domain name or subdomain'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Views.Funnels.Settings.Domains.Toggle', 'Переключить'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Views.Funnels.Settings.Domains.Toggle', 'Toggle'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Views.Funnels.Settings.Domains.AfterChangingBinding', 'После изменения привязки, старые ссылки перестанут открываться.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Views.Funnels.Settings.Domains.AfterChangingBinding', 'After changing the binding, old links will no longer open.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Views.Funnels.Settings.Domains.BindingChanges', 'Изменения привязки происходят быстро, в течении 1 минуты уже будет открываться новое содержимое.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Views.Funnels.Settings.Domains.BindingChanges', 'Binding changes happen quickly, within 1 minute new content will be opened.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Views.Funnels.Settings.Domains.NoConnectedDomains', 'Пока нет подключенных доменов для подключения'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Views.Funnels.Settings.Domains.NoConnectedDomains', 'There are no connected domains to connect yet'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Views.Funnels.Settings.Domains.Open', 'Открыть'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Views.Funnels.Settings.Domains.Open', 'Open'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Views.Funnels.Settings.SeoCounters.HtmlHEAD', 'Html-код для вставки внутрь HEAD'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Views.Funnels.Settings.SeoCounters.HtmlHEAD', 'Html code to insert inside HEAD'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Views.Funnels.Settings.SeoCounters.HtmlBODY', 'Html-код для вставки внутрь BODY'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Views.Funnels.Settings.SeoCounters.HtmlBODY', 'Html code to insert inside BODY'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Views.Funnels.Settings.SeoCounters.HideCopyright', 'Скрыть копирайт'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Views.Funnels.Settings.SeoCounters.HideCopyright', 'Hide copyright'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Views.Funnels.Settings.SeoCounters.YandexMetrica', 'Яндекс.Метрика'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Views.Funnels.Settings.SeoCounters.YandexMetrica', 'Yandex.Metrica'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Views.Funnels.Settings.SeoCounters.MeterNumberMetrics', 'Номер счётчика в метрике'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Views.Funnels.Settings.SeoCounters.MeterNumberMetrics', 'Meter number in metrics'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Views.Funnels.Settings.SeoCounters.CounterCode', 'Код счетчика'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Views.Funnels.Settings.SeoCounters.CounterCode', 'Counter code'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Views.Funnels.Settings.SeoCounters.GoogleAnalyticsAccount', 'Аккаунт Google Analytics'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Views.Funnels.Settings.SeoCounters.GoogleAnalyticsAccount', 'Google Analytics Account'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Views.Funnels.Settings.SeoCounters.LineLike', '(строка вида: 12345678-1)'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Views.Funnels.Settings.SeoCounters.LineLike', '(line like: 12345678-1)'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Views.Funnels.Settings.SeoCounters.ContainerID', 'Идентификатор контейнера'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Views.Funnels.Settings.SeoCounters.ContainerID', 'Container ID'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Views.Funnels.Settings.CSSStyles.CSSStyles', 'CSS стили для всех страниц воронки'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Views.Funnels.Settings.CSSStyles.CSSStyles', 'CSS styles for all pages of the funnel'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Views.Funnels.Settings.Auth.RequireLogin', 'Требовать логин и пароль для доступа ко всем страницам воронки'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Views.Funnels.Settings.Auth.RequireLogin', 'Require login and password to access all funnel pages'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Views.Funnels.Settings.Auth.LinkRegistrationPage', 'Ссылка на страницу регистрации'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Views.Funnels.Settings.Auth.LinkRegistrationPage', 'Link to the registration page'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Views.Funnels.Settings.Auth.HereCanSpecifyAddress', 'Здесь можно указать адрес страницы, где посетитель сможет получить/купить доступ к страницам воронки'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Views.Funnels.Settings.Auth.HereCanSpecifyAddress', 'Here you can specify the address of the page where the visitor can get/buy access to the funnel pages'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Views.Funnels.Settings.Auth.PageDisplayConditions', 'Условия отображения страницы'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Views.Funnels.Settings.Auth.PageDisplayConditions', 'Page display conditions'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Views.Funnels.Settings.Auth.AddProducts', 'Добавить товары'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Views.Funnels.Settings.Auth.AddProducts', 'Add products'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Views.Funnels.Settings.Auth.LeadList', 'Список лидов'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Views.Funnels.Settings.Auth.LeadList', 'Lead List'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Views.Funnels.Settings.Auth.Status', 'Статус'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Views.Funnels.Settings.Auth.Status', 'Status'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Views.Funnels.Settings.Auth.Any', 'Любой'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Views.Funnels.Settings.Auth.Any', 'Any'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.Index.ViewSite', 'Смотреть сайт'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.Index.ViewSite', 'View site'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.Index.QRCode', 'QR код'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.Index.QRCode', 'QR code'
               
GO--
               
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.LandingFunnelOrderWithForm.Name', 'Воронка "Один товар с допродажами"'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.LandingFunnelOrderWithForm.Name', 'Funnel "One product with upsells"'
                    
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.LandingFunnelOrderWithForm.Description', 'Это одна из наших любимых воронок. Она позволяет окупать трафик и зарабатывать больше на одном клиенте.<br><br> Основной товар здесь предлагается по максимально привлекательной цене. Она должна быть настолько заманчивой, чтобы клиенту было сложно отказаться от покупки. После оформления основного заказа, ему предлагается два дополнительных товара, тоже нужных и ценных для него. Покупка товаров из допродажи увеличивает средний чек.<br><br> С помощью этой воронки можно продавать не только товары, но и услуги.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.LandingFunnelOrderWithForm.Description', 'This is one of our favorite funnels. It allows you to recoup your traffic and earn more from the client.<br><br> Here, the main product is offered at the most attractive price. It should be so tempting that it is difficult for the client to refuse the purchase. After placing the main order, he is offered two additional products, also necessary and valuable for him. Buying products from the upsell increases the average check.<br><br> With this funnel, you can sell not only products, but also services.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.LandingFunnelOrderWithForm.Description.Title.Downsell', 'Допродажа 2 (Downsell)'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.LandingFunnelOrderWithForm.Description.Title.Downsell', 'Upsell 2 (Downsell)'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.LandingFunnelOrderWithForm.Description.Title.MainOfferPage', 'Страница основного предложения'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.LandingFunnelOrderWithForm.Description.Title.MainOfferPage', 'Main Offer Page'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.LandingFunnelOrderWithForm.Description.Title.PlacingOrder', 'Оформление заказа'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.LandingFunnelOrderWithForm.Description.Title.PlacingOrder', 'Placing an order'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.LandingFunnelOrderWithForm.Description.Title.Upsell', 'Допродажа 1 (Upsell)'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.LandingFunnelOrderWithForm.Description.Title.Upsell', 'Upsell 1'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.LandingFunnelOrderWithForm.Description.Title.ThankPage', 'Страница благодарности'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.LandingFunnelOrderWithForm.Description.Title.ThankPage', 'Thank you page'
               
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.LandingFunnelOrderWithDetails.Name', 'Воронка "Один товар с допродажами. Детально"'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.LandingFunnelOrderWithDetails.Name', 'Funnel "One product with upsells. Detailed"'
                    
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.LandingFunnelOrderWithDetails.Description', 'Это одна из наших любимых воронок. Она позволяет окупать трафик и зарабатывать больше на одном клиенте.<br><br> Основной товар здесь предлагается по максимально привлекательной цене. Она должна быть настолько заманчивой, чтобы клиенту было сложно отказаться от покупки. После оформления основного заказа, ему предлагается два дополнительных товара, тоже нужных и ценных для него. Покупка товаров из допродажи увеличивает средний чек.<br><br> С помощью этой воронки можно продавать не только товары, но и услуги.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.LandingFunnelOrderWithDetails.Description', 'This is one of our favorite funnels. It allows you to recoup traffic and earn more per client.<br><br> The main product is offered here at the most attractive price. It should be so tempting that it is difficult for the client to refuse the purchase. After placing the main order, he is offered two additional products, also necessary and valuable for him. Buying products from upselling increases the average check.<br><br> With this funnel, you can sell not only products, but also services.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.LandingFunnelOrderWithDetails.Description.Title.Downsell', 'Допродажа 2 (Downsell)'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.LandingFunnelOrderWithDetails.Description.Title.Downsell', 'Upsell 2 (Downsell)'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.LandingFunnelOrderWithDetails.Description.Title.MainOfferPage', 'Страница основного предложения'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.LandingFunnelOrderWithDetails.Description.Title.MainOfferPage', 'Main Offer Page'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.LandingFunnelOrderWithDetails.Description.Title.PlacingOrder', 'Оформление заказа'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.LandingFunnelOrderWithDetails.Description.Title.PlacingOrder', 'Placing an order'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.LandingFunnelOrderWithDetails.Description.Title.Upsell', 'Допродажа 1 (Upsell)'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.LandingFunnelOrderWithDetails.Description.Title.Upsell', 'Upsell 1'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.LandingFunnelOrderWithDetails.Description.Title.ThankPage', 'Страница благодарности'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.LandingFunnelOrderWithDetails.Description.Title.ThankPage', 'Thank you page'
    
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.MultyProducts.Name', 'Воронка "Мультитоварная"'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.MultyProducts.Name', 'Funnel "Multi-product"'
                    
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.MultyProducts.Description', 'С помощью этой воронки вы можете продавать несколько товаров на одной странице, где ограничено количество отвлекающих действий. Так клиент не запутается и положит в корзину один из вариантов товара, представленного на странице. Кроме того, клиент сможет оплатить заказ прямо на страницах воронки.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.MultyProducts.Description', 'With this funnel, you can sell several products on one page, where the number of distracting actions is limited. This way, the client will not get confused and will put one of the product options presented on the page in the cart. In addition, the client will be able to pay for the order directly on the funnel pages.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.MultyProducts.Title.OffersPage', 'Страница с предложениями'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.MultyProducts.Title.OffersPage', 'Offers Page'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.MultyProducts.Title.PlacingOrder', 'Оформление заказа'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.MultyProducts.Title.PlacingOrder', 'Placing an order'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.MultyProducts.Title.Upsell', 'Допродажа 1 (Upsell)'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.MultyProducts.Title.Upsell', 'Upsell 1'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.MultyProducts.Title.Downsell', 'Допродажа 2 (Downsell)'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.MultyProducts.Title.Downsell', 'Upsell 2 (Downsell)'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.MultyProducts.Title.ThankPage', 'Страница благодарности'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.MultyProducts.Title.ThankPage', 'Thank you page'
               
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.MultyProductsShort.Name', 'Воронка "Мультитоварная упрощенная"'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.MultyProductsShort.Name', 'Funnel "Multi-product simplified"'
                    
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.MultyProductsShort.Description', 'С помощью этой воронки вы можете продавать несколько товаров на одной странице, где ограничено количество отвлекающих действий. Так клиент не запутается и положит в корзину один из вариантов товара, представленного на странице. Кроме того, клиент сможет оплатить заказ прямо на страницах воронки.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.MultyProductsShort.Description', 'With this funnel, you can sell several products on one page, where the number of distracting actions is limited. This way, the client will not get confused and will put one of the product options presented on the page in the cart. In addition, the client will be able to pay for the order directly on the funnel pages.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.MultyProductsShort.Title.OffersPage', 'Страница с предложениями'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.MultyProductsShort.Title.OffersPage', 'Offers Page'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.MultyProductsShort.Title.PlacingOrder', 'Оформление заказа'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.MultyProductsShort.Title.PlacingOrder', 'Placing an order'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.MultyProductsShort.Title.ThankPage', 'Страница благодарности'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.MultyProductsShort.Title.ThankPage', 'Thank you page'
            
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.MultyProductsWithCategories.Name', 'Воронка "Мультитоварная с категориями"'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.MultyProductsWithCategories.Name', 'Funnel "Multi-product with categories"'
                    
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.MultyProductsWithCategories.Description', 'С помощью этой воронки вы можете продавать несколько товаров на одной странице, где ограничено количество отвлекающих действий. Так клиент не запутается и положит в корзину один из вариантов товара, представленного на странице. Кроме того, клиент сможет оплатить заказ прямо на страницах воронки.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.MultyProductsWithCategories.Description', 'With this funnel, you can sell several products on one page, where the number of distracting actions is limited. This way, the client will not get confused and will put one of the product options presented on the page in the cart. In addition, the client will be able to pay for the order directly on the funnel pages.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.MultyProductsWithCategories.Title.OffersPage', 'Страница с предложениями'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.MultyProductsWithCategories.Title.OffersPage', 'Offers Page'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.MultyProductsWithCategories.Title.PlacingOrder', 'Оформление заказа'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.MultyProductsWithCategories.Title.PlacingOrder', 'Placing an order'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.MultyProductsWithCategories.Title.Upsell', 'Допродажа 1 (Upsell)'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.MultyProductsWithCategories.Title.Upsell', 'Upsell 1'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.MultyProductsWithCategories.Title.Downsell', 'Допродажа 2 (Downsell)'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.MultyProductsWithCategories.Title.Downsell', 'Upsell 2 (Downsell)'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.MultyProductsWithCategories.Title.ThankPage', 'Страница благодарности'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.MultyProductsWithCategories.Title.ThankPage', 'Thank you page'
               
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.LandingFunnelOrderFreeWithDelivery.Name', 'Воронка "Бесплатный товар + доставка"'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.LandingFunnelOrderFreeWithDelivery.Name', 'Funnel "Free product + shipping"'
                    
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.LandingFunnelOrderFreeWithDelivery.Description', 'На первой странице вовлекайте покупателя специальным предложением. Например, предлагайте товар бесплатно, с условием оплатить только доставку. На следующих страницах воронки предлагайте дополнительные товары, уже за полную стоимость. Это помогает увеличивать средний чек.<br><br>Стоимость доставки должна окупать стоимость бесплатного товара, а допродажи всегда помогают заработать на заказах.<br><br>Эта воронка может иметь одну из лучших конверсий в заказы.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.LandingFunnelOrderFreeWithDelivery.Description', 'On the first page, engage the buyer with a special offer. For example, offer a product for free, with the condition that they pay only for shipping. On the following pages of the funnel, offer additional products, but at full price. This helps increase the average check.<br><br>The cost of shipping should cover the cost of the free product, and upselling always helps make money on orders.<br><br>This funnel can have one of the best conversions into orders.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.LandingFunnelOrderFreeWithDelivery.Title.OffersPage', 'Страница с предложениями'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.LandingFunnelOrderFreeWithDelivery.Title.OffersPage', 'Offers Page'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.LandingFunnelOrderFreeWithDelivery.Title.PlacingOrder', 'Оформление заказа'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.LandingFunnelOrderFreeWithDelivery.Title.PlacingOrder', 'Placing an order'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.LandingFunnelOrderFreeWithDelivery.Title.Upsell', 'Допродажа 1 (Upsell)'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.LandingFunnelOrderFreeWithDelivery.Title.Upsell', 'Upsell 1'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.LandingFunnelOrderFreeWithDelivery.Title.Downsell', 'Допродажа 2 (Downsell)'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.LandingFunnelOrderFreeWithDelivery.Title.Downsell', 'Upsell 2 (Downsell)'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.LandingFunnelOrderFreeWithDelivery.Title.ThankPage', 'Страница благодарности'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.LandingFunnelOrderFreeWithDelivery.Title.ThankPage', 'Thank you page'
               
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.VideoWithCrossSells.Name', 'Воронка "Видеопредложение с допродажами"'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.VideoWithCrossSells.Name', 'Funnel "Video offer with upsells"'
                    
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.VideoWithCrossSells.Description', 'Очень простая воронка. Вы показываете видеопредложение «прогретой», подготовленной к покупке аудитории и размещаете под видео кнопку «заказать».'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.VideoWithCrossSells.Description', 'A very simple funnel. You show a video offer to a "warmed up" audience ready to buy and place an "order" button under the video.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.VideoWithCrossSells.Title.OffersPage', 'Страница с предложениями'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.VideoWithCrossSells.Title.OffersPage', 'Offers Page'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.VideoWithCrossSells.Title.PlacingOrder', 'Оформление заказа'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.VideoWithCrossSells.Title.PlacingOrder', 'Placing an order'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.VideoWithCrossSells.Title.Upsell', 'Допродажа 1 (Upsell)'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.VideoWithCrossSells.Title.Upsell', 'Upsell 1'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.VideoWithCrossSells.Title.Downsell', 'Допродажа 2 (Downsell)'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.VideoWithCrossSells.Title.Downsell', 'Upsell 2 (Downsell)'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.VideoWithCrossSells.Title.ThankPage', 'Страница благодарности'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.VideoWithCrossSells.Title.ThankPage', 'Thank you page'
               
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.ArticleWithCrossSells.Name', 'Воронка "Продающая статья с допродажами"'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.ArticleWithCrossSells.Name', 'Funnel "Selling article with upsells"'
                    
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.ArticleWithCrossSells.Description', 'В этой воронке вы рассказываете, как клиенты могут решить свою проблему с помощью вашего товара или услуги. Напишите для этого продающую статью, используя технику «Крючок — история — предложение». Привлеките внимание людей цепким заголовком. Расскажите им свою, похожую, историю, когда вы находились в этом же состоянии: не знали, что делать, ошибались, но в итоге решили этот вопрос. Затем предложите то, что принесло вам результат.<br><br>Когда вы продали товар или услугу с помощью статьи, опять же, используйте страницы допродаж на этапе оформления основного заказа.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.ArticleWithCrossSells.Description', 'In this funnel, you tell how customers can solve their problem with your product or service. Write a sales article for this, using the “Hook - Story - Offer” technique. Grab people’s attention with a catchy headline. Tell them your similar story, when you were in the same situation: you didn’t know what to do, you made mistakes, but eventually you solved this issue. Then offer what brought you results.<br><br>When you have sold a product or service with the help of an article, again, use upselling pages at the stage of placing the main order.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.ArticleWithCrossSells.Title.SellingArticle', 'Продающая статья'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.ArticleWithCrossSells.Title.SellingArticle', 'Selling article'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.ArticleWithCrossSells.Title.PlacingOrder', 'Оформление заказа'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.ArticleWithCrossSells.Title.PlacingOrder', 'Placing an order'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.ArticleWithCrossSells.Title.Upsell', 'Допродажа 1 (Upsell)'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.ArticleWithCrossSells.Title.Upsell', 'Upsell 1'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.ArticleWithCrossSells.Title.Downsell', 'Допродажа 2 (Downsell)'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.ArticleWithCrossSells.Title.Downsell', 'Upsell 2 (Downsell)'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.ArticleWithCrossSells.Title.ThankPage', 'Страница благодарности'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.ArticleWithCrossSells.Title.ThankPage', 'Thank you page'
               
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.CompanySiteCourse.Name', 'Воронка "Онлайн-школа"'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.CompanySiteCourse.Name', 'Funnel "Online School"'
                    
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.CompanySiteCourse.Description', 'Воронка Онлайн-школы представляет собой страницу с информацией: о чем ваш курс, для кого вы его создали, что в нем будет, сколько он стоит, отзывы других учеников. После приобретения курса покупатель попадает в личный кабинет ученика с доступом к ценным материалам.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.CompanySiteCourse.Description', 'The Online School Funnel is a page with information: what your course is about, who you created it for, what it will contain, how much it costs, reviews from other students. After purchasing the course, the buyer gets into the students personal account with access to valuable materials.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.CompanySiteCourse.Title.SellingCourse', 'Продажа курса'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.CompanySiteCourse.Title.SellingCourse', 'Selling a course'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.CompanySiteCourse.Title.CourseOffice', 'Кабинет курса'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.CompanySiteCourse.Title.CourseOffice', 'Course office'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.CompanySiteCourse.Title.InternalPageExample', 'Пример внутренней страницы'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.CompanySiteCourse.Title.InternalPageExample', 'Example of an internal page'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.CompanySiteCourse.Title.ThankPage', 'Страница благодарности'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.CompanySiteCourse.Title.ThankPage', 'Thank you page'
               
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.ContactForContent.Name', 'Воронка "Захват контакта за контент"'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.ContactForContent.Name', 'Funnel "Contact capture for content"'
                    
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.ContactForContent.Description', 'Обмен пользы на контакт. На первой странице разместите привлекательный заголовок, который обещает клиенту что-то ценное. Чтобы получить нужную информацию или файл, он должен оставить адрес электронной почты в специальном окошке. Полезный контент автоматически отправляется на указанный адрес, а адрес остается в вашей базе.<br><br>После того как посетитель оставит свой email, отправьте его на страницу благодарности.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.ContactForContent.Description', 'Exchange benefits for contact. On the first page, place an attractive headline that promises something valuable to the client. To receive the necessary information or file, he must leave an email address in a special window. Useful content is automatically sent to the specified address, and the address remains in your database.<br><br>After the visitor leaves his email, send him to the thank you page.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.ContactForContent.Title.CollectingContacts', 'Сбор контактов'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.ContactForContent.Title.CollectingContacts', 'Collecting contacts'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.ContactForContent.Title.ThankPage', 'Страница благодарности'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.ContactForContent.Title.ThankPage', 'Thank you page'
               
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.ArticleLead.Name', 'Воронка "Статья"'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.ArticleLead.Name', 'Funnel "Article"'
                    
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.ArticleLead.Description', 'Поделитесь ценной и полезной информацией с вашими потенциальными клиентами в статье. Встройте туда ссылку на ваше предложение. Статья помогает убедить потенциальных покупателей, которые только изучают вопрос, что решение этой проблемы уже есть — у вас.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.ArticleLead.Description', 'Share valuable and useful information with your potential customers in an article. Embed a link to your offer there. The article helps convince potential buyers who are just studying the issue that the solution to this problem already exists - you have it.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.ArticleLead.Title.CollectingContacts', 'Сбор контактов'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.ArticleLead.Title.CollectingContacts', 'Collecting contacts'
               
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.CouponWithDiscount.Name', 'Воронка "Получи купон на скидку"'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.CouponWithDiscount.Name', 'Funnel "Get a discount coupon"'
                    
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.CouponWithDiscount.Description', 'Предложите скидочный купон взамен на адрес электронной почты. Перенаправьте посетителя на страницу предложения, где он сможет использовать полученный купон. Это может быть интернет-магазин, мини-каталог или страница с одним предложением.<br><br>Отправьте купон на указанную электронную почту.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.CouponWithDiscount.Description', 'Offer a discount coupon in exchange for an email address. Redirect the visitor to the offer page where they can use the coupon they received. This could be an online store, a mini-catalog, or a page with one offer.<br><br>Send the coupon to the specified email.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.CouponWithDiscount.Title.CollectingContacts', 'Сбор контактов'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.CouponWithDiscount.Title.CollectingContacts', 'Collecting contacts'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.CouponWithDiscount.Title.ThankPage', 'Страница благодарности'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.CouponWithDiscount.Title.ThankPage', 'Thank you page'
               
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.VideoLeadMagnetNew.Name', 'Воронка "Лид-магнит "Видео""'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.VideoLeadMagnetNew.Name', 'Funnel "Lead Magnet "Video""'
                    
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.VideoLeadMagnetNew.Description', 'Дайте полезную информацию прежде, чем будете просить клиента оставить свои контакты. Так вас услышит и узнает большее количество людей, ведь им не нужно ничего оставлять взамен. В данном случае вы соберете меньше лидов, но их качество будет значительно выше, чем в схеме «вначале контакт — потом польза».<br><br>После просмотра первого видео предложите клиенту получить еще больше полезной информации, которую вы сможете ему отправить, если он оставит e-mail.<br><br>Для этой воронки подготовьте лучшую информацию, что у вас есть.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.VideoLeadMagnetNew.Description', 'Provide useful information before asking the client to leave their contact information. This way, more people will hear and know you, because they don’t have to leave anything in return. In this case, you will collect fewer leads, but their quality will be much higher than in the “contact first – benefit later” scheme.<br><br>After watching the first video, offer the client to receive even more useful information that you can send them if they leave their email.<br><br>For this funnel, prepare the best information you have.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.VideoLeadMagnetNew.Title.CollectingContacts', 'Сбор контактов'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.VideoLeadMagnetNew.Title.CollectingContacts', 'Collecting contacts'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.VideoLeadMagnetNew.Title.ThankPage', 'Страница благодарности'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.VideoLeadMagnetNew.Title.ThankPage', 'Thank you page'
               
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Orders.OrderInfo.Change', 'Изменить'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Orders.OrderInfo.Change', 'Change'
               
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.CollectContactsForAccess.Name', 'Воронка "Доступ к категории"'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.CollectContactsForAccess.Name', 'Funnel "Access to category"'
                    
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.CollectContactsForAccess.Description', 'Эта воронка также помогает собирать контакты потенциальных клиентов. На отдельной странице разместите ваше лучшее предложение. Чтобы посмотреть другие товары из этой категории, пользователь должен будет оставить email. После того как он заполнит поле для электронного адреса, перенаправьте его в каталог вашего магазина.<br><br> Помните, что вы собираете контакты не просто так, а чтобы общаться с вашими клиентами, через правильные письма стимулировать их к покупкам.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.CollectContactsForAccess.Description', 'This funnel also helps to collect contacts of potential customers. On a separate page, place your best offer. To view other products from this category, the user will have to leave an email. After he fills in the email field, redirect him to the catalog of your store.<br><br> Remember that you are collecting contacts not just like that, but to communicate with your customers, through the right letters to encourage them to buy.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.CollectContactsForAccess.Title.CollectingContact', 'Сбор контакта'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.CollectContactsForAccess.Title.CollectingContact', 'Collecting contact'
               
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.ContactForEBook.Name', 'Воронка "Захват контакта за эл. книгу"'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.ContactForEBook.Name', 'Funnel "Capture contact for e-book"'
                    
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.ContactForEBook.Description', 'Подключайте эту воронку, чтобы расширить базу потенциальных клиентов. Предложите вашим посетителям бесплатно скачать небольшую, но ценную для них электронную книгу, в обмен на e-mail. После того как вы получили контактные данные, перенаправьте пользователя на страницу благодарности. В тот же момент необходимо отправить ему автоматическое письмо со ссылкой на скачивание обещанной книги. Вы всегда сможете составить такую электронную книгу на базе статей, ранее написанных вашей компанией. Книга не должна быть большой. Набора из 5-7 статьей будет достаточно.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.ContactForEBook.Description', 'Connect this funnel to expand your potential customer base. Offer your visitors a free download of a small but valuable e-book in exchange for an e-mail. After you have received their contact information, redirect the user to a thank you page. At the same time, you need to send them an automatic letter with a link to download the promised book. You can always create such an e-book based on articles previously written by your company. The book does not have to be large. A set of 5-7 articles will be enough.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.ContactForEBook.Title.CollectingContact', 'Сбор контакта'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.ContactForEBook.Title.CollectingContact', 'Collecting contact'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.ContactForEBook.Title.ThankPage', 'Страница благодарности'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.ContactForEBook.Title.ThankPage', 'Thank you page'
       
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.ConsultingNew.Name', 'Воронка "Консалтинг"'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.ConsultingNew.Name', 'Funnel "Consulting"'
                    
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.ConsultingNew.Description', 'С помощью короткого ролика на первой странице привлеките внимание клиента, обозначьте волнующие его вопросы и предложите записаться на консультацию прямо на странице. Можно добавить анкету, которая поможет вам подготовиться к консультации.<br><br>Помогите клиенту, ответив лично на все его вопросы, чтобы он убедился, что именно ваш продукт ему подходит. Такая коммуникация повышает доверие и помогает выстроить правильные, крепкие отношения.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.ConsultingNew.Description', 'With a short video on the first page, grab the clients attention, outline their concerns, and offer to sign up for a consultation right on the page. You can add a questionnaire that will help you prepare for the consultation.<br><br>Help the client by personally answering all their questions so that they can be sure that your product is right for them. Such communication increases trust and helps build the right, strong relationships.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.ConsultingNew.Title.OfferWithVideo', 'Предложение с видео'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.ConsultingNew.Title.OfferWithVideo', 'Offer with video'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.ConsultingNew.Title.UsefulContent', 'Полезный контент'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.ConsultingNew.Title.UsefulContent', 'Useful content'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.ConsultingNew.Title.Questionnaire', 'Анкета'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.ConsultingNew.Title.Questionnaire', 'Questionnaire'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.ConsultingNew.Title.ThankPage', 'Страница благодарности'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.ConsultingNew.Title.ThankPage', 'Thank you page'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.Lottery.Name', 'Воронка "Розыгрыш"'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.Lottery.Name', 'Funnel "Draw"'
                    
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.Lottery.Description', 'Розыгрыш призов — это хороший способ привлечь аудиторию и познакомить ее с вашей компанией.<br><br>Продумайте условия участия. Например: «подпишитесь на нашу страницу в соцсетях», «отметьте друзей», «напишите комментарий», «поставьте лайки». Эти простые действия со стороны пользователей помогают собирать новую аудиторию, продвигать ваши посты и ролики.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.Lottery.Description', 'A prize draw is a good way to attract an audience and introduce them to your company.<br><br>Think about the conditions of participation. For example: "subscribe to our page on social networks", "tag friends", "write a comment", "like". These simple actions on the part of users help to collect a new audience, promote your posts and videos.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.Lottery.Title.OfferPage', 'Страница предложения'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.Lottery.Title.OfferPage', 'Offer page'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.Lottery.Title.ThankPage', 'Страница благодарности'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.Lottery.Title.ThankPage', 'Thank you page'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.QuizFunnelNew.Name', 'Воронка "Квиз"'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.QuizFunnelNew.Name', 'Funnel "Quiz"'
                    
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.QuizFunnelNew.Description', 'Эта воронка помогает предложить клиенту наиболее подходящий ему товар. Пользователь отвечает на вопросы по теме и, в зависимости от выбранных ответов, попадает на страницу с тем или иным предложением.<br><br>Подробнее об этой механике мы уже писали в рецепте №2.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.QuizFunnelNew.Description', 'This funnel helps to offer the client the most suitable product. The user answers questions on the topic and, depending on the selected answers, gets to the page with one or another offer.<br><br>We have already written about this mechanic in more detail in recipe #2.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.QuizFunnelNew.Title.InvitationQuiz', 'Приглашение пройти квиз'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.QuizFunnelNew.Title.InvitationQuiz', 'Invitation to take a quiz'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.QuizFunnelNew.Title.QuizPage', 'Страница квиза'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.QuizFunnelNew.Title.QuizPage', 'Quiz Page'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.QuizFunnelNew.Title.OtherPages', 'Другие страницы, в зависимости от типа предложения'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.QuizFunnelNew.Title.OtherPages', 'Other pages depending on the type of offer'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.Questionnaire.Name', 'Воронка "Анкета"'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.Questionnaire.Name', 'Funnel "Questionnaire"'
                    
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.Questionnaire.Description', 'Соберите у клиентов нужную вам информацию, а затем используйте ее для улучшения продаж. Вы можете разместить любое количество вопросов. Все данные об ответах будут автоматически сохранены в CRM-системе ADVANTSHOP, в карточке покупателя. Далее, используя эту информацию, вы сможете фильтровать клиентов, распределять их по категориям и делать персональные рассылки для каждой категории.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.Questionnaire.Description', 'Collect the information you need from your customers and then use it to improve sales. You can place any number of questions. All data on the answers will be automatically saved in the ADVANTSHOP CRM system, in the customer card. Then, using this information, you can filter customers, distribute them by category and make personalized mailings for each category.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.Questionnaire.Title.Questionnaire', 'Анкета'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.Questionnaire.Title.Questionnaire', 'Questionnaire'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.Questionnaire.Title.ThankPage', 'Страница благодарности'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.Questionnaire.Title.ThankPage', 'Thank you page'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.EventAction.Name', 'Воронка "Мероприятие"'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.EventAction.Name', 'Funnel "Event"'
                    
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.EventAction.Description', 'Основная задача этой воронки — записать максимально возможное число посетителей на мероприятие, если оно бесплатное. Или продать как можно больше билетов на платное мероприятие. За день до события вы должны отправить напоминания о нем всем зарегистрированным участникам.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.EventAction.Description', 'The main goal of this funnel is to register the maximum possible number of visitors to the event, if it is free. Or to sell as many tickets as possible to a paid event. The day before the event, you should send reminders about it to all registered participants.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.EventAction.Title.SignEvent', 'Запись на мероприятие'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.EventAction.Title.SignEvent', 'Sign up for the event'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.EventAction.Title.ThankPage', 'Страница благодарности'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.EventAction.Title.ThankPage', 'Thank you page'
             
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.Webinar.Name', 'Воронка "Вебинар"'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.Webinar.Name', 'Funnel "Webinar"'
                    
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.Webinar.Description', 'Задача вебинарной воронки — привести как можно больше людей на ваш веб-семинар. Обеспечить максимальную посещаемость помогут страница регистрации и страница благодарности. Ваша задача — в день проведения вебинара отправить клиентам напоминания о нем. Вы можете сделать это с помощью автоматической рассылки смс или электронных писем.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.Webinar.Description', 'The goal of a webinar funnel is to bring as many people as possible to your webinar. A registration page and a thank you page will help ensure maximum attendance. Your goal is to send reminders to your clients on the day of the webinar. You can do this by sending automated SMS or emails.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.Webinar.Title.SignWebinar', 'Запись на вебинар'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.Webinar.Title.SignWebinar', 'Sign up for the webinar'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.Webinar.Title.ThankPage', 'Страница благодарности'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.Webinar.Title.ThankPage', 'Thank you page'  
               
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.CompanySiteWithLeads.Name', 'Сайт компании, сбор заявок'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.CompanySiteWithLeads.Name', 'Company website, collection of applications'
                    
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.CompanySiteWithLeads.Description', 'Страница компании нужна, если требуется рассказать о вашей компании, ее услугах и товарах в формате официального веб-сайта. Часто сайт компании требуется, чтобы разместить здесь ваше портфолио, прайс-лист, контакты и форму обратной связи.<br><br>Вы можете добавить любое количество блоков на страницу: «О сотрудниках», «Отзывы», «Фотогалерея» и другие. Для этого используйте встроенный конструктор страниц в ADVANTSHOP.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.CompanySiteWithLeads.Description', 'A company page is needed if you want to tell about your company, its services and products in the format of an official website. Often, a company website is required to place your portfolio, price list, contacts and feedback form here.<br><br>You can add any number of blocks to the page: "About employees", "Reviews", "Photo gallery" and others. To do this, use the built-in page builder in ADVANTSHOP.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.CompanySiteWithLeads.Intent', '<ul><li>Познакомить аудиторию с товарами и услугами компании</li><li>Предоставить контактную информацию</li><li>Продать товары и услуги</li></ul>'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.CompanySiteWithLeads.Intent', '<ul><li>Introduce the audience to the companys products and services</li><li>Provide contact information</li><li>Sell products and services</li></ul>'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.CompanySiteWithLeads.Advice', 'Используйте как можно больше собственных фотографий и меньше копируйте из Интернета.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.CompanySiteWithLeads.Advice', 'Use as many of your own photos as possible and copy as little as possible from the Internet.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.CompanySiteWithLeads.Title.CollectingContacts', 'Сбор контактов'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.CompanySiteWithLeads.Title.CollectingContacts', 'Collecting contacts'
               
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.CompanySiteWithCatalog.Name', 'Сайт компании с каталогом товаров'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.CompanySiteWithCatalog.Name', 'Company website with product catalog'
                    
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.CompanySiteWithCatalog.Description', 'Страница компании нужна, если требуется рассказать о вашей компании, ее услугах и товарах в формате официального веб-сайта. Часто сайт компании требуется, чтобы разместить здесь ваше портфолио, прайс-лист, контакты и форму обратной связи.<br><br>Вы можете добавить любое количество блоков на страницу: «О сотрудниках», «Отзывы», «Фотогалерея» и другие. Для этого используйте встроенный конструктор страниц в ADVANTSHOP.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.CompanySiteWithCatalog.Description', 'A company page is needed if you want to tell about your company, its services and products in the format of an official website. Often, a company website is required to place your portfolio, price list, contacts and feedback form here.<br><br>You can add any number of blocks to the page: "About employees", "Reviews", "Photo gallery" and others. To do this, use the built-in page builder in ADVANTSHOP.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.CompanySiteWithCatalog.Intent', '<ul><li>Познакомить аудиторию с товарами и услугами компании</li><li>Предоставить контактную информацию</li><li>Продать товары и услуги</li></ul>'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.CompanySiteWithCatalog.Intent', '<ul><li>Introduce the audience to the companys products and services</li><li>Provide contact information</li><li>Sell products and services</li></ul>'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.CompanySiteWithCatalog.Advice', 'Используйте как можно больше собственных фотографий и меньше копируйте из Интернета.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.CompanySiteWithCatalog.Advice', 'Use as many of your own photos as possible and copy as little as possible from the Internet.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.CompanySiteWithCatalog.Title.CollectingContacts', 'Сбор контактов'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.CompanySiteWithCatalog.Title.CollectingContacts', 'Collecting contacts'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.CompanySiteWithCatalog.Title.AboutRestaurant', 'О ресторане'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.CompanySiteWithCatalog.Title.AboutRestaurant', 'About the restaurant'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.CompanySiteWithCatalog.Title.MenuDelivery', 'Меню и Доставка'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.CompanySiteWithCatalog.Title.MenuDelivery', 'Menu and Delivery'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.CompanySiteWithCatalog.Title.Banquets', 'Банкеты'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.CompanySiteWithCatalog.Title.Banquets', 'Banquets'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.CompanySiteWithCatalog.Title.PhotoGallery', 'Фотогалерея'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.CompanySiteWithCatalog.Title.PhotoGallery', 'Photo Gallery'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.CompanySiteWithCatalog.Title.Contacts', 'Контакты'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.CompanySiteWithCatalog.Title.Contacts', 'Contacts'
               
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.CompanySiteWithPrices.Name', 'Сайт компании с ценами'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.CompanySiteWithPrices.Name', 'Company website with prices'
                    
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.CompanySiteWithPrices.Description', 'Страница компании нужна, если требуется рассказать о вашей компании, ее услугах и товарах в формате официального веб-сайта. Часто сайт компании требуется, чтобы разместить здесь ваше портфолио, прайс-лист, контакты и форму обратной связи.<br><br>Вы можете добавить любое количество блоков на страницу: «О сотрудниках», «Отзывы», «Фотогалерея» и другие. Для этого используйте встроенный конструктор страниц в ADVANTSHOP.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.CompanySiteWithPrices.Description', 'A company page is needed if you want to tell about your company, its services and products in the format of an official website. Often, a company website is required to place your portfolio, price list, contacts and feedback form here.<br><br>You can add any number of blocks to the page: "About employees", "Reviews", "Photo gallery" and others. To do this, use the built-in page builder in ADVANTSHOP.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.CompanySiteWithPrices.Intent', '<ul><li>Познакомить аудиторию с товарами и услугами компании</li><li>Предоставить контактную информацию</li><li>Продать товары и услуги</li></ul>'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.CompanySiteWithPrices.Intent', '<ul><li>Introduce the audience to the companys products and services</li><li>Provide contact information</li><li>Sell products and services</li></ul></li></ul>'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.CompanySiteWithPrices.Advice', 'Используйте как можно больше собственных фотографий и меньше копируйте из Интернета.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.CompanySiteWithPrices.Advice', 'Use as many of your own photos as possible and copy as little as possible from the Internet.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.CompanySiteWithPrices.Title.CollectingContacts', 'Сбор контактов'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.CompanySiteWithPrices.Title.CollectingContacts', 'Collecting contacts'
               
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.InstagramFunnel.Name', 'Микролендинг "Instagram"'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.InstagramFunnel.Name', 'Micro Landing "Instagram"'
                    
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.InstagramFunnel.Description', 'Ни для кого не секрет, что в Instagram нельзя оставлять ссылки внутри постов и в комментариях. Вы можете разместить только одну активную ссылку в описании вашего профиля. А важных страниц, на которые нужно привести клиентов, у вас несколько: магазин, официальный сайт, страница с акцией, свежая статья о вашей работе, страница с опросом и так далее. Еще нужно дать интерактивную ссылку на мессенджеры. Иначе, чтобы связаться с вами, посетитель вынужден будет перебивать вручную ваш номер телефона и долго искать вас в WhatsApp, Viber или Telegram. Не каждый будет этим заниматься.<br><br>Для решения этой проблемы создаются микролендинги специально для Instagram, где размещается вся необходимая информация и ссылки на разные ресурсы.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.InstagramFunnel.Description', 'Its no secret that you cant leave links inside posts or in comments on Instagram. You can only place one active link in your profile description. And you have several important pages to which you need to bring clients: a store, an official website, a page with a promotion, a fresh article about your work, a page with a survey, and so on. You also need to provide an interactive link to messengers. Otherwise, in order to contact you, the visitor will have to manually enter your phone number and search for you for a long time on WhatsApp, Viber or Telegram. Not everyone will do this.<br><br>To solve this problem, micro landings are created specifically for Instagram, where all the necessary information and links to various resources are posted.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.InstagramFunnel.Intent', 'Разместите в микролендинге кнопку, которая позволит быстро связаться с вами в любом из популярных мессенджеров.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.InstagramFunnel.Intent', 'Place a button on your micro-landing page that will allow people to quickly contact you in any of the popular messengers.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.InstagramFunnel.Advice', 'Используйте как можно больше собственных фотографий и меньше копируйте из Интернета.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.InstagramFunnel.Advice', 'Use as many of your own photos as possible and copy as little as possible from the Internet.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.InstagramFunnel.Title.CollectingContacts', 'Сбор контактов'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.InstagramFunnel.Title.CollectingContacts', 'Collecting contacts'
               
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.CompanySiteExpert.Name', 'Личная страница эксперта'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.CompanySiteExpert.Name', 'Experts personal page'
                    
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.CompanySiteExpert.Description', 'Личная страница создается, чтобы рассказать о себе и своих проектах. Она может стать доказательством вашей экспертности. Ссылку на такую страницу размещают в аккаунтах социальных сетей, в подписи для электронной почты, на визитках.<br><br>Хорошо разместить на личной странице предложение бесплатных консультаций. Для этой работы нужно выделить определенные часы и дни недели. Посетители должны иметь возможность выбрать свободный временной слот и записаться.<br><br>При необходимости вы можете сделать платные консультации, подключив оплату к бронированию временного слота. Все эти возможности уже есть в ADVANTSHOP.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.CompanySiteExpert.Description', 'A personal page is created to tell about yourself and your projects. It can become proof of your expertise. A link to such a page is placed in social network accounts, in an email signature, on business cards.<br><br>It is a good idea to place an offer of free consultations on your personal page. For this work, you need to allocate certain hours and days of the week. Visitors should be able to choose a free time slot and sign up.<br><br>If necessary, you can make paid consultations by connecting payment to the time slot booking. All these options are already available in ADVANTSHOP.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.CompanySiteExpert.Intent', '<ul><li>Доказать вашу экспертность в какой-то области</li><li>Рассказать о себе</li><li>Завести новые связи</li><li>Заработать на консультациях</li></ul>'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.CompanySiteExpert.Intent', '<ul><li>Prove your expertise in a certain field</li><li>Tell about yourself</li><li>Make new connections</li><li>Earn money from consultations</li></ul>'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.CompanySiteExpert.Advice', 'Не забудьте пригласить посетителей подписаться на ваши соцсети, блог или рассылку в мессенджере.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.CompanySiteExpert.Advice', 'Dont forget to invite visitors to subscribe to your social networks, blog or messenger newsletter.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.CompanySiteExpert.Title.CollectingContacts', 'Сбор контактов'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.CompanySiteExpert.Title.CollectingContacts', 'Collecting contacts'
               
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.CompanySiteBePartner.Name', 'Стань партнером'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.CompanySiteBePartner.Name', 'Become a partner'
                    
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.CompanySiteBePartner.Description', 'В ADVANTSHOP вы можете подключить партнерскую программу для реферального маркетинга. Это модель привлечения клиентов, когда ваши действующие клиенты рекомендуют вас за вознаграждение. Механика простая: участник партнерской программы получает уникальную ссылку или промокод, мотивирует знакомых получить ваш товар или услугу по этой ссылке и за каждого приведенного клиента получает вознаграждение. Подробнее о партнерской программе вы можете почитать на сайте <a href="https://www.advantshop.net/features-tour/partnerprogram">advantshop.net</a> в разделе «Возможности».<br><br>Для привлечения партнеров вам понадобится страница, приглашающая к сотрудничеству. Напишите на ней, какие услуги ваши партнеры могут перепродавать и какие нужно выполнить условия для получения вознаграждения.<br><br>На странице размещается кнопка «Стать партнером», ведущая на форму регистрации в партнерской программе.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.CompanySiteBePartner.Description', 'In ADVANTSHOP, you can connect an affiliate program for referral marketing. This is a model for attracting customers when your current customers recommend you for a reward. The mechanics are simple: a participant in the affiliate program receives a unique link or promo code, motivates friends to get your product or service via this link, and receives a reward for each customer brought. You can read more about the affiliate program on the website <a href="https://www.advantshop.net/features-tour/partnerprogram">advantshop.net</a> in the "Opportunities" section.<br><br>To attract partners, you will need a page inviting you to cooperate. Write on it what services your partners can resell and what conditions must be met to receive a reward.<br><br>The page contains a "Become a partner" button leading to the registration form in the affiliate program.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.CompanySiteBePartner.Intent', '<ul><li>Презентация партнерской программы</li><li>Привлечение партнерского трафика</li></ul>'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.CompanySiteBePartner.Intent', '<ul><li>Presentation of the affiliate program</li><li>Attracting affiliate traffic</li></ul>'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.CompanySiteBePartner.Advice', 'Опционально вы можете добавить на страницу видеообращение к партнерам, где расскажете обо всех преимуществах сотрудничества с вами.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.CompanySiteBePartner.Advice', 'Optionally, you can add a video message to your partners on the page, where you will tell them about all the benefits of working with you.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.CompanySiteBePartner.Title.CollectingContacts', 'Сбор контактов'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.CompanySiteBePartner.Title.CollectingContacts', 'Collecting contacts'
               
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.CompanySiteUnderConstruction.Name', 'Сайт в разработке'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.CompanySiteUnderConstruction.Name', 'The site is under development'
                    
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.CompanySiteUnderConstruction.Description', 'Страница-заглушка отлично подходит, чтобы сообщить посетителям, что ваш сайт находится в разработке и скоро появится. Отредактируйте этот шаблон: добавьте контакты компании, ссылки на ваши страницы в соц. сетях, форму обратной связи. Пользователи будут оставлять свои электронные адреса, чтобы первыми узнать об открытии вашего сайта.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.CompanySiteUnderConstruction.Description', 'A placeholder page is great for letting visitors know that your site is under construction and will be available soon. Edit this template: add company contacts, links to your social media pages, a feedback form. Users will leave their email addresses to be the first to know about the opening of your site.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.CompanySiteUnderConstruction.Title.PlaceholderPage', 'Страница-заглушка'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.CompanySiteUnderConstruction.Title.PlaceholderPage', 'Placeholder page'
               
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.CompanySiteWithBooking.Name', 'Сайт компании с записью к специалистам'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.CompanySiteWithBooking.Name', 'Company website with appointments with specialists'
                    
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.CompanySiteWithBooking.Description', 'Готовый одностраничный сайт с позволит посетителям забронировать запись к нужным специалистам, а вам — организовать рабочее время сотрудников.<br><br>Бронирование и отображение доступных слотов происходит в режиме онлайн. Учет и работа с бронированиями находится в специальном разделе администрирования. В шаблоне предусмотрена запись в режиме заявки, а также с обязательным выбором услуг и онлайн-оплаты.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.CompanySiteWithBooking.Description', 'A ready-made one-page website will allow visitors to book an appointment with the necessary specialists, and you will be able to organize the working hours of employees.<br><br>Booking and display of available slots occurs online. Accounting and work with bookings is located in a special administration section. The template provides for recording in the application mode, as well as with the mandatory selection of services and online payment.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.CompanySiteWithBooking.Title.OffersPage', 'Страница с предложениями'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.CompanySiteWithBooking.Title.OffersPage', 'Offers Page'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.CompanySiteWithBooking.Title.Booking', 'Бронирование'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.CompanySiteWithBooking.Title.Booking', 'Booking'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.CompanySiteWithBooking.Title.ThankPage', 'Страница благодарности'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.CompanySiteWithBooking.Title.ThankPage', 'Thank you page'

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Triggers.EditTrigger.SendToShippingService', 'Передать в службу доставки'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Triggers.EditTrigger.SendToShippingService', 'Send it to the shipping service'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Triggers.EditTrigger.SendEmailOnError', 'Отправить Email при ошибке передачи'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Triggers.EditTrigger.SendEmailOnError', 'Send Email in case of transmission error'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Triggers.EditTrigger.EmailForError', 'Email получателя'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Triggers.EditTrigger.EmailForError', 'Recipient''s email address'

GO--

ALTER TABLE [Catalog].[Coupon]
    ADD Comment NVARCHAR(MAX);
	
GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.AddEditCoupon.Comment', 'Комментарий администратора'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.AddEditCoupon.Comment', 'Administrators comment' 

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Triggers.Action.ErrorSendToShippingService.DefaultEmailSubject', 'Ошибка при отправке заказа №#Number#'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Triggers.Action.ErrorSendToShippingService.DefaultEmailSubject', 'Error when sending order №#Number#'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Triggers.Action.ErrorSendToShippingService.DefaultEmailBody', '<p>Не удалось отправить заказ №#Number# в службу доставки #ShippingMethod#.</p><p>Ошибка: #ErrorMessage#</p>'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Triggers.Action.ErrorSendToShippingService.DefaultEmailBody', '<p>Order №#Number#could not be sent to the delivery service #ShippingMethod#.</p><p>Error: #ErrorMessage#</p>'

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.ExportImport.ProductFields.AccrueBonuses', 'Начислять бонусы за покупку этого товара'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.ExportImport.ProductFields.AccrueBonuses', 'Award bonuses for purchasing this product'

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.CustomerGroups.BaseDiscount', 'Базовая скидка'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.CustomerGroups.BaseDiscount', 'Base discount'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.ModalAddEditCustomerGroupCategoryDiscount.Category', 'Категория'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.ModalAddEditCustomerGroupCategoryDiscount.Category', 'Category'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.ModalAddEditCustomerGroupCategoryDiscount.Discount', 'Скидка (%)'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.ModalAddEditCustomerGroupCategoryDiscount.Discount', 'Discount'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.ModalAddEditCustomerGroupCategoryDiscount.CategoryDiscount', 'Скидка для категории'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.ModalAddEditCustomerGroupCategoryDiscount.CategoryDiscount', 'Discount for category'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.CustomerGroups.SetDiscountForCategory', 'Указать скидку для категорий'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.CustomerGroups.SetDiscountForCategory', 'Set discount for categories'
GO--
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Customers].[CustomerGroup_Category]') AND type in (N'U'))
BEGIN
    CREATE TABLE [Customers].[CustomerGroup_Category](
        [CustomerGroupId] [int] NOT NULL,
        [CategoryId] [int] NOT NULL,
        [Discount] [float] NOT NULL,
    CONSTRAINT [PK_CustomerGroup_Category] PRIMARY KEY CLUSTERED 
    (
        [CustomerGroupId] ASC,
        [CategoryId] ASC
    )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
    ) ON [PRIMARY]
END
GO--
IF NOT EXISTS (SELECT 1 
			   FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS 
			   WHERE CONSTRAINT_NAME='FK_CustomerGroup_Category_Category')
BEGIN
    ALTER TABLE [Customers].[CustomerGroup_Category]  WITH CHECK ADD  CONSTRAINT [FK_CustomerGroup_Category_Category] FOREIGN KEY([CategoryId])
    REFERENCES [Catalog].[Category] ([CategoryID])
    ON DELETE CASCADE
END
GO--
IF NOT EXISTS (SELECT 1 
			   FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS 
			   WHERE CONSTRAINT_NAME='FK_CustomerGroup_Category_CustomerGroup')
BEGIN
    ALTER TABLE [Customers].[CustomerGroup_Category]  WITH CHECK ADD  CONSTRAINT [FK_CustomerGroup_Category_CustomerGroup] FOREIGN KEY([CustomerGroupId])
    REFERENCES [Customers].[CustomerGroup] ([CustomerGroupID])
    ON DELETE CASCADE
END
GO--
IF NOT EXISTS(SELECT 1
              FROM sys.columns
              WHERE (name = N'MainCategoryId') AND object_id = OBJECT_ID(N'[Catalog].[Product]'))
BEGIN
    ALTER TABLE [Catalog].[Product] ADD
        MainCategoryId int NULL
END

GO--

UPDATE Catalog.Product 
SET MainCategoryId = pg.CategoryID 
From Catalog.Product 
Left Join [Catalog].[ProductCategories] pg ON pg.ProductId = Product.ProductId AND Main = 1
WHERE MainCategoryId is null

GO--

ALTER PROCEDURE [Catalog].[sp_AddProductToCategory] 
    @ProductId int,
    @CategoryId int,
    @SortOrder int,
    @MainCategory bit
AS
BEGIN
    DECLARE @Main bit = @MainCategory
    SET NOCOUNT ON;
    IF (@MainCategory = 1)
        UPDATE [Catalog].ProductCategories SET Main = 0 WHERE ProductID = @ProductID
    ELSE
        SET @Main = CASE WHEN EXISTS (SELECT 1 FROM [Catalog].ProductCategories WHERE ProductID = @ProductID AND Main = 1 AND CategoryID <> @CategoryId) THEN 0 ELSE 1 END;
    IF NOT EXISTS (SELECT 1 FROM [Catalog].ProductCategories WHERE CategoryID = @CategoryID AND ProductID = @ProductID)
        INSERT INTO [Catalog].ProductCategories (CategoryID, ProductID, SortOrder, Main) VALUES (@CategoryID, @ProductID, @SortOrder, @Main);
    ELSE
        UPDATE [Catalog].ProductCategories SET Main = @Main WHERE CategoryID = @CategoryID AND ProductID = @ProductID 
	IF (@Main = 1)
		UPDATE [Catalog].Product SET MainCategoryId = @CategoryID WHERE ProductID = @ProductID 
END
GO--
ALTER PROCEDURE [Catalog].[sp_AddProductToCategoryByExternalId] 
	@ProductID int,
	@ExternalId nvarchar(50),
	@SortOrder int	
AS
BEGIN
	SET NOCOUNT ON;
	DECLARE @CategoryId int = (SELECT TOP(1) CategoryId FROM [Catalog].Category WHERE ExternalId = @ExternalId)
	IF @CategoryId IS NOT NULL AND NOT EXISTS (SELECT 1 FROM [Catalog].ProductCategories WHERE CategoryId = @CategoryId and ProductId = @ProductId)
	BEGIN
		DECLARE @Main bit = CASE WHEN EXISTS (SELECT 1 FROM [Catalog].ProductCategories WHERE ProductID = @ProductID AND Main = 1) THEN 0 ELSE 1 END;
		INSERT INTO [Catalog].ProductCategories (CategoryID, ProductID, SortOrder, Main) VALUES (@CategoryId, @ProductId, @SortOrder, @Main);
		IF (@Main = 1)
			UPDATE [Catalog].Product SET MainCategoryId = @CategoryID WHERE ProductID = @ProductID 
	END
END
GO--
ALTER PROCEDURE [Catalog].[sp_RemoveProductFromCategory] 
	@ProductId as int,
	@CategoryId as int
AS
BEGIN
	SET NOCOUNT ON;
	DELETE FROM [Catalog].[ProductCategories] 
	WHERE [CategoryID] = @CategoryId AND [ProductID] = @ProductId;
	
	DECLARE @isMainCategoryExists bit = (CASE WHEN EXISTS(SELECT 1 
														  FROM [Catalog].[ProductCategories] 
														  WHERE [ProductID] = @ProductId and Main = 1) 
												THEN 1 
												ELSE 0 END)
	IF @isMainCategoryExists = 0
	BEGIN
		DECLARE @MainCategoryId int = (SELECT top(1) [CategoryID] FROM [Catalog].[ProductCategories] WHERE [ProductID] = @ProductId)
		
		IF @MainCategoryId is not NULL
		BEGIN
			Update [Catalog].[ProductCategories] 
			Set Main = 1 
			Where [ProductID] = @ProductId and [CategoryID] = @MainCategoryId
			Update [Catalog].[Product]
			Set MainCategoryId = @MainCategoryId 
			Where [ProductID] = @ProductId
		END
	END 
END
GO--
ALTER PROCEDURE [Catalog].[sp_SetMainCategoryLink]
	@ProductID int,
	@CategoryID int
AS
BEGIN
	SET NOCOUNT ON;
	if (SELECT [Main] FROM [Catalog].[ProductCategories] WHERE [ProductID] = @ProductID AND [CategoryID] = @CategoryID) = 0
	begin
		UPDATE [Catalog].[ProductCategories] SET [Main] = 0 WHERE [ProductID] = @ProductID
		UPDATE [Catalog].[ProductCategories] SET [Main] = 1 WHERE [ProductID] = @ProductID AND [CategoryID] = @CategoryID 
		UPDATE [Catalog].[Product] SET MainCategoryId = @CategoryID WHERE [ProductID] = @ProductID
	end
END
GO--
ALTER PROCEDURE [Catalog].[sp_DeleteCategoryWithSubCategoies]
	@id int
AS
BEGIN
	DECLARE @Hierarchycte TABLE (CategoryID int);
	WITH Hierarchycte (CategoryID) AS	
	(
		SELECT CategoryID FROM Catalog.Category WHERE CategoryID = @id
		Union ALL	
		SELECT Category.CategoryID 
		FROM Catalog.Category	
		INNER JOIN hierarchycte	ON Category.ParentCategory = hierarchycte.CategoryID
		WHERE Category.CategoryID <> @id
	) 
	Insert into @Hierarchycte 
	SELECT CategoryID FROM Hierarchycte WHERE CategoryID <> 0;
	DELETE [Catalog].[Category] WHERE CategoryID IN (SELECT CategoryID FROM @Hierarchycte);
	UPDATE pc
	SET main = 1
	FROM catalog.ProductCategories pc
	INNER JOIN
	(
		SELECT ProductId, min(CategoryID) as CategoryID
		FROM catalog.ProductCategories AS pc
		GROUP BY ProductID
		HAVING SUM(main*1) = 0
	) iddata ON pc.ProductID = iddata.ProductID and pc.CategoryID = iddata.CategoryID;
	UPDATE Catalog.Product 
	SET MainCategoryId = (Select top(1) pg.CategoryID FROM [Catalog].[ProductCategories] pg WHERE pg.ProductId = Product.ProductId Order By pg.Main desc) 
	From Catalog.Product 
	WHERE MainCategoryId in (SELECT CategoryID FROM @Hierarchycte);
	SELECT CategoryID FROM @Hierarchycte;
END
GO--
ALTER PROCEDURE [Customers].[sp_GetRecentlyView]
	@CustomerId uniqueidentifier,
	@rowsCount int,
	@Type nvarchar(50)
AS
BEGIN
	SET NOCOUNT ON;
	
	IF Exists (SELECT 1 FROM [Customers].[RecentlyViewsData] WHERE CustomerID=@CustomerId)
    Begin
        SELECT TOP(@rowsCount) Product.ProductID, Product.ArtNo, Product.Name, Product.UrlPath, Product.AllowPreOrder, Ratio, ManualRatio, isnull(PhotoNameSize1, PhotoName) as PhotoName,
            [Photo].[Description] as PhotoDescription, Discount, DiscountAmount, MinPrice as BasePrice, CurrencyValue,
            Offer.OfferID, MaxAvailable AS Amount, MinAmount, MaxAmount, Offer.Amount AS AmountOffer, Colors, NotSamePrices as MultiPrices,
            Product.DoNotApplyOtherDiscounts, Product.MainCategoryId, Units.DisplayName as UnitDisplayName, Units.Name as UnitName 
        
        FROM [Customers].RecentlyViewsData
        
            Inner Join [Catalog].Product ON Product.ProductID = RecentlyViewsData.ProductId
            Left Join [Catalog].[ProductExt] ON [Product].[ProductID] = [ProductExt].[ProductID]
            Inner Join Catalog.Currency On Currency.CurrencyID = Product.CurrencyID
            Left Join [Catalog].[Photo] ON [Photo].[PhotoId] = [ProductExt].[PhotoId]
            Left Join [Catalog].[Offer] ON [ProductExt].[OfferID] = [Offer].[OfferID]
			Left Join [Catalog].[Units] ON [Product].[Unit] = [Units].[Id]
        
        WHERE RecentlyViewsData.CustomerID = @CustomerId AND Product.Enabled = 1 And CategoryEnabled = 1
        
        ORDER BY ViewDate Desc
    End
END

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Triggers.Action.SendToShippingService.UnloadOrderNotSupport', 'Указанная в заказе доставка не поддерживает автоматическую выгрузку заказа в систему доставки.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Triggers.Action.SendToShippingService.UnloadOrderNotSupport', 'The delivery specified in the order does not support automatic unloading of the order into the delivery system.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Triggers.Action.SendToShippingService.UnknownResult', 'Неизвестный результат выполнения отправки заказа в систему доставки.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Triggers.Action.SendToShippingService.UnknownResult', 'Unknown result of the order being sent to the delivery system.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Triggers.EditTrigger.SendToShippingService.AvailableShippings', 'Поддерживают следующие модули доставок'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Triggers.EditTrigger.SendToShippingService.AvailableShippings', 'The following delivery modules are supported'

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Modules.Resellers.ProductsNotListed', 'Действие с товарами, которых нет в прайсе'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Modules.Resellers.ProductsNotListed', 'Action with goods that are not in the price list'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.ExportImport.ImportCsv.DeleteProducts', 'Удаление товаров'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.ExportImport.ImportCsv.DeleteProducts', 'Removing products'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.ExportImport.ImportCsv.ResetToZero', 'Обнулить остатки'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.ExportImport.ImportCsv.ResetToZero', 'Reset balances'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.ExportImport.ImportCsv.DisableProducts', 'Деактивировать товары'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.ExportImport.ImportCsv.DisableProducts', 'Disable products'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.ExportImport.ImportCsv.DoNothing', 'Ничего не делать'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.ExportImport.ImportCsv.DoNothing', 'Do nothing'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.ExportImport.ImportCsv.ActionGoodsNotPrice', 'Действия с товарами, которых нет в прайсе'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.ExportImport.ImportCsv.ActionGoodsNotPrice', 'Actions with goods that are not in the price list'

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Template.ProductReviews', 'Отзывы о товарах'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Template.ProductReviews', 'Product reviews'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Template.MainPageProductReviewsVisibility', 'Выводить отзывы о товарах на главной каруселью'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Template.MainPageProductReviewsVisibility', 'Display product reviews on the main carousel'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Template.CountMainPageProductReviewsInSection', 'Количество отзывов о товарах на главной в блоке'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Template.CountMainPageProductReviewsInSection', 'Count of product reviews on the main page in the block'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Template.CountMainPageProductReviewsInLine', 'Количество отзывов о товарах на главной в строке'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Template.CountMainPageProductReviewsInLine', 'Count of product reviews on the main page in a row'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.ProductReviewCarousel.Title', 'Отзывы о товарах'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.ProductReviewCarousel.Title', 'Product Reviews'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.ProductReviewCard.ShowPhotos', 'Показать все'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.ProductReviewCard.ShowPhotos', 'Show all'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.ProductReviewCard.HidePhotos', 'Скрыть фото'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.ProductReviewCard.HidePhotos', 'Hide photos'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.Builder.ProductReviews', 'Отзывы о товарах'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.Builder.ProductReviews', 'Product reviews'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.Builder.MainPageProductReviewsVisibility', 'Выводить отзывы о товарах на главной каруселью'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.Builder.MainPageProductReviewsVisibility', 'Display product reviews on the main carousel'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.Builder.CountMainPageProductReviewsInSection', 'Количество отзывов о товарах на главной в блоке'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.Builder.CountMainPageProductReviewsInSection', 'Count of product reviews on the main page in the block'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.Builder.CountMainPageProductReviewsInLine', 'Количество отзывов о товарах на главной в строке'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.Builder.CountMainPageProductReviewsInLine', 'Count of product reviews on the main page in a row'

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.Cart.Removing', 'Удаление'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.Cart.Removing', 'Remove'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.Cart.Removing.AreYouSureDelete', 'Вы уверены, что хотите удалить товар из корзины?'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.Cart.Removing.AreYouSureDelete', 'Are you sure you want to remove the item from your cart?'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.Cart.Clear.AreYouSureClear', 'Вы уверены, что хотите очистить корзину?'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.Cart.Clear.AreYouSureClear', 'Are you sure you want to clear?'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.Cart.Warning.Clear.Title', 'Очистить'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.Cart.Warning.Clear.Title', 'Clear'


GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsCatalog.CatalogCommon.ShowPropertiesFilterInParentCategoriesNote', 'Если настройка не включена, то фильтр по свойствам показывается только в категории последнего уровня. Категорией последнего уровня для товара является такая категория, у которой нет подкатегории, содержащей данный товар.<br><br>Если настройка включена, то фильтр по свойствам для товара показывается в каждой из назначенных товару категорий.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsCatalog.CatalogCommon.ShowPropertiesFilterInParentCategoriesNote', 'If the setting is not enabled, the property filter is shown only in the last-level category. The last-level category for a product is a category that does not have any subcategories that make up the product.<br><br>If the setting is enabled, the property filter for the product is shown in each of the designated category categories.'

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.SystemSettings.SetCookieOnMainDomainHint', 'Данная настройка необходима для кросс-доменной работы. Cookies будут устанавливаться на главный домен и будут доступны на его поддоменах. <b>Если у сайта есть другие домены, то они не будут нормально работать!</b><br><br> Например, у вас есть поддомены msk.site.ru, spb.site.ru и т.д. и вам необходимо, чтобы авторизованный покупатель мог свободно по ним перемещаться и при переходе не надо было каждый раз соглашаться на использование cookies или выбирать город. Cookies будут устанавливаться на .site.ru и будут общими для всех поддоменов. В этом случае активируйте эту настройку.<br> <b>В любых других случаях не используйте эту опцию.</b>'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.SystemSettings.SetCookieOnMainDomainHint', 'This setting is necessary for cross-domain operation. Cookies will be installed on the main domain and will be available on subdomains. For example, you have subdomains msk.site.ru , spb.site.ru and you need an authorized buyer to be able to freely navigate through them and not have to agree to use cookies every time or choose a city. Cookies will be installed on .site.ru and they will be common to all subdomains. In this case, activate this setting. <b>In any other cases, do not use this option.</b>'

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Template.CountMainPageCategoriesInLineTitleLink', 'Responsible for the number of categories that will be displayed on the main page in each block. <br> <br>More details: <br> <a href="https://www.advantshop.net/help/pages/category-on-main" target="_blank"> Main categories </a>'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Template.CountMainPageCategoriesInSectionTitleLink', 'Responsible for the number of categories that will be displayed on the main page in each block. <br> <br>More details: <br> <a href="https://www.advantshop.net/help/pages/category-on-main" target="_blank"> Main categories </a>'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.Builder.CountMainPageCategoriesInLineTitleLink', 'Responsible for the number of categories that will be displayed in one line of each block on the main page. <br> <br>More details: <br> <a href="https://www.advantshop.net/help/pages/category-on-main" target="_blank"> Main categories </a>'

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Services.Shipping.ParcelTerminalsDeliveryPoints', 'постаматы и пункты выдачи'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Services.Shipping.ParcelTerminalsDeliveryPoints', 'parcel terminals and delivery points'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Services.Shipping.ByCourier', 'Курьер {0}'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Services.Shipping.ByCourier', 'Courier {0}'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'AdvantShop.Core.Shipping.TypeOfDelivery.Courier', 'Доставка курьером'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'AdvantShop.Core.Shipping.TypeOfDelivery.Courier', 'Delivery by courier'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'AdvantShop.Core.Shipping.TypeOfDelivery.SelfDelivery', 'Доставка до пункта выдачи заказов'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'AdvantShop.Core.Shipping.TypeOfDelivery.SelfDelivery', 'Delivery to the point of delivery of orders'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'AdvantShop.Core.Shipping.CostOfDelivery.ShippingCostUp', 'Стоимость доставки увеличится на {0}'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'AdvantShop.Core.Shipping.CostOfDelivery.ShippingCostUp', 'Shipping cost will increase by {0}'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'AdvantShop.Core.Shipping.YandexApi.Courier', 'Курьер'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'AdvantShop.Core.Shipping.YandexApi.Courier', 'Courier'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'AdvantShop.Core.Shipping.YandexApi.OrderPickupPoint', 'ПВЗ'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'AdvantShop.Core.Shipping.YandexApi.OrderPickupPoint', 'Order Pickup Point'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'AdvantShop.Core.Shipping.YandexApi.Postamat', 'Постамат'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'AdvantShop.Core.Shipping.YandexApi.Postamat', 'Postamat'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Services.Shipping.ParcelTerminalsDeliveryPointsWithSpace', '{0} (постаматы и пункты выдачи)'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Services.Shipping.ParcelTerminalsDeliveryPointsWithSpace', '{0} (parcel terminals and delivery points)'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Services.Shipping.ParcelLockersWithSpace', ' (постаматы)'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Services.Shipping.ParcelLockersWithSpace', ' (parcel lockers)'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Services.Shipping.PickupPointsWithSpace', ' (пункты выдачи)'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Services.Shipping.PickupPointsWithSpace', ' (pick-up points)'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Services.Shipping.ByCourierWithSpace', ' (курьером)'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Services.Shipping.ByCourierWithSpace', ' (by courier)'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Services.Shipping.RussianPostWithSpace', ' (Почта России)'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Services.Shipping.RussianPostWithSpace', ' (Russian Post)'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Services.Shipping.ByCourierWithDeliveryService', '{0} (курьером {1})'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Services.Shipping.ByCourierWithDeliveryService', '{0} (by courier {1})'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Services.Shipping.ParcelTerminalsDeliveryPointsTwoParams', '{0} (постаматы и пункты выдачи {1})'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Services.Shipping.ParcelTerminalsDeliveryPointsTwoParams', '{0} (parcel terminals and delivery points {1}'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Services.Shipping.ByCourierWithParam', '{0} (курьером)'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Services.Shipping.ByCourierWithParam', '{0} (by courier)'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Services.Shipping.PickupPointsWithParam', '{0} (пункты выдачи)'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Services.Shipping.PickupPointsWithParam', '{0} (pick-up points)'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Services.Shipping.AtPickupPointsWithParam', '{0} (в пункт выдачи)'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Services.Shipping.AtPickupPointsWithParam', '{0} (в пункт выдачи)'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Services.Shipping.FromRussianPost', ' из отделений Почты России'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Services.Shipping.FromRussianPost', ' from Russian Post offices'

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApiAuth.Index.BonusSystem.GetBonusCardTransactionsMe', 'Список транзакций'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApiAuth.Index.BonusSystem.GetBonusCardTransactionsMe', 'Transactions list'

GO--

IF NOT EXISTS(SELECT 1
              FROM sys.columns
              WHERE (name = N'IsAppliedToPriceWithDiscount') AND object_id = OBJECT_ID(N'[Catalog].[Coupon]'))
BEGIN
    ALTER TABLE Catalog.Coupon ADD
        IsAppliedToPriceWithDiscount bit NULL
END

GO--

Update Catalog.Coupon 
Set IsAppliedToPriceWithDiscount = 0 
Where IsAppliedToPriceWithDiscount is null

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.AddEditCoupon.IsAppliedToPriceWithDiscount', 'Применять купон к цене со скидкой'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.AddEditCoupon.IsAppliedToPriceWithDiscount', 'Apply to products price with discount'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.AddEditCoupon.IsAppliedToPriceWithDiscountHint', 'По умолчанию, если скидка товара > стоимости купона, то применится скидка товара. Так сделано чтобы скидки товара не суммировались с купоном. Если активировать опцию, то купон будет применяться к цене со скидкой.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.AddEditCoupon.IsAppliedToPriceWithDiscountHint', 'By default, if the product discount is > coupon value, the product discount will be applied. It is done so that the product discounts are not combined with the coupon. If you activate the option, the coupon will be applied to the discounted price.'

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Bonuses.BonusSystem.EBonusNotificationMethod.SMS', 'по SMS'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Bonuses.BonusSystem.EBonusNotificationMethod.SMS', 'via SMS'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Bonuses.BonusSystem.EBonusNotificationMethod.Email', 'по e-mail'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Bonuses.BonusSystem.EBonusNotificationMethod.Email', 'by email'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Bonuses.BonusSystem.EBonusNotificationMethod.Push', 'через push уведомление'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Bonuses.BonusSystem.EBonusNotificationMethod.Push', 'via push notification'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Bonuses.BonusSystem.EBonusNotificationMethod.Nothing', 'Ничего'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Bonuses.BonusSystem.EBonusNotificationMethod.Nothing', 'Nothing'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsBonus.Index.SendIfPushNotWork', 'Отправлять, если не сработает Push уведомление'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsBonus.Index.SendIfPushNotWork', 'Send if Push notification does not work'

GO--

create table Customers.CustomerAdminPushNotification
(
    CustomerId             uniqueidentifier not null
        constraint CustomerAdminPushNotification_pk
            primary key
        constraint CustomerAdminPushNotification_Customer_CustomerID_fk
            references Customers.Customer
            on delete cascade,
    FcmToken               nvarchar(max),
    NotificationsEnabled bit              not null
)

go--

insert into Customers.CustomerAdminPushNotification (CustomerId, FcmToken, NotificationsEnabled)
select CustomerID, null, 1
from Customers.Customer
where CustomerRole = 100
   or CustomerRole = 50

go--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.AddEditUser.AdminAppNotificationsEnabled', N'Push-уведомления в мобильном приложении администратора'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.AddEditUser.AdminAppNotificationsEnabled', 'Push notifications in the administrator''s mobile application'

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.VkMarketApi.AccessDenied', 'У приложения недостаточно прав. Пожалуйста, настройте доступы приложению на "Сообщества", "Фотографии", "Товары" и "Оповещения об ответах" согласно инструкции <a target="_blank" href="https://www.advantshop.net/help/pages/module-vkmarket">https://www.advantshop.net/help/pages/module-vkmarket</a><br><br> Если доступы разрешили, то попробуйте "Удалить привязку" в настройках и подключить снова.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.VkMarketApi.AccessDenied', 'Access denied. Please, confirm access to "Communities", "Photos", "Products" and "Response Alerts" according to the instructions <a target="_blank" href="https://www.advantshop.net/help/pages/module-vkmarket">https://www.advantshop.net/help/pages/module-vkmarket</a><br><br> If access is allowed, then try to "Remove the binding" and connect again.'

GO--

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Catalog].[ShareShoppingCart]') AND type in (N'U'))
BEGIN
    CREATE TABLE [Catalog].[ShareShoppingCart](
        [Key] [nvarchar](8) NOT NULL,
        [CustomerId] [uniqueidentifier] NOT NULL,
        [OfferId] [int] NOT NULL,
		[AttributesXml] [nvarchar](max) NULL,
		[Amount] [Float] NOT NULL,
		[DateCreated] [datetime] NOT NULL
    ) ON [PRIMARY]
	
    ALTER TABLE [Catalog].[ShareShoppingCart]  WITH CHECK ADD  CONSTRAINT [FK_ShareShoppingCart_Offer] FOREIGN KEY([OfferId])
    REFERENCES [Catalog].[Offer] ([OfferId])
    ON DELETE CASCADE
END

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.Cart.CouldNotCopyLink', 'Скопировать ссылку не удалось'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.Cart.CouldNotCopyLink', 'The link could not be copied'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.Cart.LinkCopied', 'Ссылка скопирована'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.Cart.LinkCopied', 'The link has been copied'

GO--

IF NOT EXISTS(SELECT 1
              FROM sys.columns
              WHERE (name = N'YandexMarketCategoryId') AND object_id = OBJECT_ID(N'[Catalog].[ProductExportOptions]'))
BEGIN
    ALTER TABLE Catalog.ProductExportOptions ADD
        YandexMarketCategoryId bigint NULL
END

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.ExportImport.EProductField.YandexMarketCategoryId', N'Яндекс.Маркет: Категория на Маркете (market_category_id)'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.ExportImport.EProductField.YandexMarketCategoryId', 'Yandex Market: Market category (market_category_id)'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.ExportImport.ProductFields.YandexMarketCategoryId', N'Яндекс.Маркет: Категория на Маркете (market_category_id)'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.ExportImport.ProductFields.YandexMarketCategoryId', 'Yandex Market: Market category (market_category_id)'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.ExportOptions.YandexMarketCategoryId', N'Категория на Яндекс.Маркете (market_category_id)'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.ExportOptions.YandexMarketCategoryId', 'Market category (market_category_id)'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.ExportOptions.YandexMarketCategoryId.Hint', N'Категория на Яндекс.Маркете (market_category_id) может пригодиться в случае, если Маркет автоматически добавляет товар в неправильную категорию.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.ExportOptions.YandexMarketCategoryId.Hint', 'Market category (market_category_id) may be useful if the Market automatically adds an item to the wrong category.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.ExportOptions.YandexMarketCategoryId.ListHint', N'Cписок категорий и их номера'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.ExportOptions.YandexMarketCategoryId.ListHint', 'Categories list'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.ExportOptions.YandexMarketCategoryId.ReadMore', N'Подробнее'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.ExportOptions.YandexMarketCategoryId.ReadMore', 'Read more'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.ExportFeed.SettingsYandex.ExportMarketCategoryId', N'Выгружать тег market_category_id'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.ExportFeed.SettingsYandex.ExportMarketCategoryId', 'Export tag market_category_id'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.ExportFeed.SettingsYandex.ExportMarketCategoryIdHint', N'Выгружать категорию на Яндекс.Маркете в тег market_category_id'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.ExportFeed.SettingsYandex.ExportMarketCategoryIdHint', 'Export market category in market_category_id tag'

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Template.MainPageCategoriesVisibilityTitleLink', 'Responsible for displaying categories on the main page. Only those categories for which the "Show on home" setting is enabled will be displayed. <br> <br>More details: <br> <a href="https://www.advantshop.net/help/pages/category-on-main" target="_blank"> Main categories </a>'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Template.CountMainPageCategoriesInLineTitleLink', 'Отвечает за количество категорий,которое будет отображаться в одной строке каждого блока на главной странице. <br> <br>Подробнее: <br> <a href="https://www.advantshop.net/help/pages/category-on-main" target="_blank"> Категории на главной </a>'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Template.CountMainPageCategoriesInLineTitleLink', 'Responsible for the number of categories that will be displayed in one line of each block on the main page. <br> <br>More details: <br> <a href="https://www.advantshop.net/help/pages/category-on-main" target="_blank"> Main categories </a>'

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Template.MainPageProductReviewsVisibilityLink', 'Отвечает за вывод отзывов о товарах на главной странице. Если настройка выключена, отзывы будут выводиться списком, если включена, каруселью.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Template.MainPageProductReviewsVisibilityLink', 'Responsible for displaying product reviews on the main page. If the setting is off, the reviews will be displayed in a list, if it is on, in a carousel.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Template.CountMainPageProductReviewsInSectionLink', 'Настройка отвечает за общее количество отзывов, которые будут отображаться на главной странице.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Template.CountMainPageProductReviewsInSectionLink', 'The setting is responsible for the total number of reviews that will be displayed on the main page.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Template.CountMainPageProductReviewsInLineLink', 'Настройка отвечает за количество отзывов, которые будут отображаться на главной странице в блоке.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Template.CountMainPageProductReviewsInLineLink', 'The setting is responsible for the number of reviews that will be displayed on the main page in the block.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.Builder.MainPageProductReviewsVisibilityLink', 'Отвечает за вывод отзывов о товарах на главной странице. Если настройка выключена, отзывы будут выводиться списком, если включена, каруселью.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.Builder.MainPageProductReviewsVisibilityLink', 'Responsible for displaying product reviews on the main page. If the setting is off, the reviews will be displayed in a list, if it is on, in a carousel.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.Builder.CountMainPageProductReviewsInSectionLink', 'Настройка отвечает за общее количество отзывов, которые будут отображаться на главной странице.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.Builder.CountMainPageProductReviewsInSectionLink', 'The setting is responsible for the total number of reviews that will be displayed on the main page.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.Builder.CountMainPageProductReviewsInLineLink', 'Настройка отвечает за количество отзывов, которые будут отображаться на главной странице в блоке.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.Builder.CountMainPageProductReviewsInLineLink', 'The setting is responsible for the number of reviews that will be displayed on the main page in the block.'

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.AdminComments.Add.ValidationError.Task.EmptyManager', 'Создатель комментария должен быть менеджером'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.AdminComments.Add.ValidationError.Task.EmptyManager', 'The comment creator must be a manager'

GO--

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Order].[sp_AddOrder]') AND type = N'P')
BEGIN
	DROP PROCEDURE [Order].[sp_AddOrder];
END

GO--

IF ((SELECT COUNT(*) FROM [Settings].[TemplateSettings] WHERE [Name] = 'CheckOrderVisibility' AND [Value] = 'True') > 0)
BEGIN
	UPDATE 
		[Settings].[TemplateSettings]
	SET
		[Value] = 'False'
	WHERE
		[Name] = 'CheckOrderVisibility'
END

GO--

IF ((SELECT COUNT(*) FROM [Settings].[TemplateSettings] WHERE [Name] = 'NewsSubscriptionVisibility' AND [Value] = 'True') > 0)
BEGIN
	UPDATE 
		[Settings].[TemplateSettings]
	SET
		[Value] = 'False'
	WHERE
		[Name] = 'NewsSubscriptionVisibility'
END

GO--

If not Exists (SELECT 1 
				 FROM INFORMATION_SCHEMA.COLUMNS
				WHERE 
					TABLE_NAME = 'OrderCustomOptions' 
					AND TABLE_SCHEMA = 'Order'
					AND COLUMN_NAME = 'OptionTitle'
					AND DATA_TYPE = 'nvarchar'
					AND CHARACTER_MAXIMUM_LENGTH = 2000)
BEGIN
    ALTER TABLE [Order].[OrderCustomOptions] ALTER COLUMN [OptionTitle] nvarchar(2000) NOT NULL
END

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.AuthService.EAuthMethod.Email', 'По электронной почте'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.AuthService.EAuthMethod.Email', 'By email'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.AuthService.EAuthMethod.Code', 'По коду'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.AuthService.EAuthMethod.Code', 'By code'

GO--

IF ((SELECT [Value] FROM [Settings].[Settings] WHERE Name = 'OpenIdProviderAuthByCodeActive') = 'True' 
	AND NOT EXISTS (SELECT * FROM [Settings].[Settings] WHERE Name = 'AuthMethod'))
BEGIN
	INSERT INTO [Settings].[Settings] (Name, Value)
	VALUES ('AuthMethod', '1')
END

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'User.CodeDescription.Sms', 'Вам придет смс с кодом. Введите 4 цифры.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'User.CodeDescription.Sms', 'You will receive a SMS with a code. Enter 4 digits.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'User.CodeDescription.Call.Flash', 'Введите 4 последние цифры номера, с которого поступит звонок.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'User.CodeDescription.Call.Flash', 'Enter the last 4 digits of the number from which the call will be received.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'User.CodeDescription.Call.Voice', 'Введите 4 цифры кода, который вы услышите при звонке.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'User.CodeDescription.Call.Voice', 'Enter the 4 digits of the code you will hear when you will get a call.'

GO--
               
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Services.Landing.Templates.LpTemplate.OnlineStore', 'Интернет-магазин'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Services.Landing.Templates.LpTemplate.OnlineStore', 'Online store'
               
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Services.Landing.Templates.LpTemplate.ProductFunnels', 'Товарные воронки'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Services.Landing.Templates.LpTemplate.ProductFunnels', 'Product funnels'
               
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Services.Landing.Templates.LpTemplate.LeadCollection', 'Сбор лидов'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Services.Landing.Templates.LpTemplate.LeadCollection', 'Lead collection'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Services.Landing.Templates.LpTemplate.Questionnaires', 'Опросники'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Services.Landing.Templates.LpTemplate.Questionnaires', 'Questionnaires'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Services.Landing.Templates.LpTemplate.PresentationFunnels', 'Презентационные воронки'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Services.Landing.Templates.LpTemplate.PresentationFunnels', 'Presentation Funnels'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Services.Landing.Templates.LpTemplate.Landings', 'Лендинги'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Services.Landing.Templates.LpTemplate.Landings', 'Landings'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateTemplate.Back', 'Назад'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateTemplate.Back', 'Back'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.Back', 'Назад'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.Back', 'Back'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Templates.Common.Header.Back', 'Назад'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Templates.Common.Header.Back', 'Back'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Content.QrCodeGenerator.QrCodeGenerator', 'QR код сайта'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Content.QrCodeGenerator.QrCodeGenerator', 'QR code of the site'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Content.QrCodeGenerator.FailedGenerateQRCode', 'Не удалось сгенерировать QR код'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Content.QrCodeGenerator.FailedGenerateQRCode', 'Failed to generate QR code'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Content.QrCodeGenerator.Download', 'Скачать'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Content.QrCodeGenerator.Download', 'Download'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Content.QrCodeGenerator.Close', 'Закрыть'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Content.QrCodeGenerator.Close', 'Close'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateSiteTemplateItem.OnlineDemo', 'Онлайн демо'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateSiteTemplateItem.OnlineDemo', 'Online demo'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.OnlineDemo', 'Онлайн демо'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.OnlineDemo', 'Online demo'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.CreateFunnel', 'Создать воронку'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.CreateFunnel', 'Create Funnel'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.FunnelCreation', 'Создание воронки'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.FunnelCreation', 'Funnel creation'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.Description', 'Описание:'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.Description', 'Description:'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.Goals', 'Цели:'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.Goals', 'Goals:'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.Advice', 'Совет:'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.Advice', 'Advice:'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.VideoReviewFunnel', 'Видео обзор воронки'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.VideoReviewFunnel', 'Video review of the funnel'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.FunnelDiagram', 'Схема воронки'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.FunnelDiagram', 'Funnel diagram'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.FunnelPages', 'Страницы воронки'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.FunnelPages', 'Funnel Pages'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.OfferPages', 'Страница предложения'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.OfferPages', 'Offer page'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Funnels.Site.MySites', 'Мои сайты'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Funnels.Site.MySites', 'My sites'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Funnels.Site.CreateCopyFunnel', 'Создать копию воронки'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Funnels.Site.CreateCopyFunnel', 'Create a copy of the funnel'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Funnels.Site.GoWebsite', 'Перейти на сайт'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Funnels.Site.GoWebsite', 'Go to website'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Funnels.Site.Funnel', 'Воронка:'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Funnels.Site.Funnel', 'Funnel:'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Funnels.Site.Published', 'Опубликован'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Funnels.Site.Published', 'Published'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Funnels.Site.NotPublished', 'Не опубликован'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Funnels.Site.NotPublished', 'Not published'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Funnels.Site.VisibleFunnelPublished', 'Когда воронка опубликована, ее видят все пользователи и появляется в карте сайта. Иначе воронку видит только администратор.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Funnels.Site.VisibleFunnelPublished', 'When a funnel is published, it is visible to all users and appears in the sitemap. Otherwise, only the administrator can see the funnel.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Funnels.Site.ConnectYourDomain', 'Подключить свой домен'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Funnels.Site.ConnectYourDomain', 'Connect your domain'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Funnels.Site.Pages', 'Страницы'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Funnels.Site.Pages', 'Pages'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Funnels.Site.EmailChains', 'Email цепочки'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Funnels.Site.EmailChains', 'Email chains'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Funnels.Site.Leads', 'Лиды'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Funnels.Site.Leads', 'Leads'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Funnels.Site.Orders', 'Заказы'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Funnels.Site.Orders', 'Orders'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Funnels.Site.Armor', 'Брони'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Funnels.Site.Armor', 'Armor'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Funnels.Site.Settings', 'Настройки'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Funnels.Site.Settings', 'Settings'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Funnels.Site.CreatePage', 'Создать страницу'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Funnels.Site.CreatePage', 'Create a page'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Funnels.Site.FunnelFunnel', 'Воронка'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Funnels.Site.FunnelFunnel', 'Funnel'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Funnels.Site.Rename', 'Переименовать'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Funnels.Site.Rename', 'Rename'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Funnels.Site.Publish', 'Опубликовать'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Funnels.Site.Publish', 'Publish'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Funnels.Site.RemoveFromPublication', 'Снять с публикации'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Funnels.Site.RemoveFromPublication', 'Remove from publication'
              EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Funnels.Site.Delete', 'Удалить'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Funnels.Site.Delete', 'Delete'
              EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Funnels.Site.Name', 'Название'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Funnels.Site.Name', 'Name'
              EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Funnels.Site.Home', 'Главная'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Funnels.Site.Home', 'Home'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Funnels.FunnelData.Emails.TimeInterval', 'Временной интервал:'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Funnels.FunnelData.Emails.TimeInterval', 'Time interval:'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Funnels.FunnelData.Emails.NewChainLetters', 'Новая цепочка писем'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Funnels.FunnelData.Emails.NewChainLetters', 'New chain of letters'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Funnels.FunnelData.Emails.NoChainsConfigured', 'Нет настроенных цепочек'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Funnels.FunnelData.Emails.NoChainsConfigured', 'No chains configured'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Funnels.FunnelData.Emails.SubjectLetter', 'Тема письма'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Funnels.FunnelData.Emails.SubjectLetter', 'Subject of the letter'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Funnels.FunnelData.Emails.ConnectStatistics', 'Подключить статистику'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Funnels.FunnelData.Emails.ConnectStatistics', 'Connect statistics'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Funnels.FunnelData.Emails.Delete', 'Удалить'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Funnels.FunnelData.Emails.Delete', 'Delete'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Funnels.FunnelData.Emails.Edit', 'Редактировать'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Funnels.FunnelData.Emails.Edit', 'Edit'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Reports.ProductOrdersStatistics.OrdersFound', 'Найдено заказов: {0} на сумму {1}'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Reports.ProductOrdersStatistics.OrdersFound', 'Orders found: {0} for a total of {1}'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Handlers.Orders.GetOrders.OrdersFound', 'Найдено заказов: {0} на сумму {1}'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Handlers.Orders.GetOrders.OrdersFound', 'Orders found: {0} for a total of {1}'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Handlers.Marketing.Analytics.Reports.OfferOrdersStatistics.OrdersFound', 'Найдено заказов: {0} на сумму {1}'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Handlers.Marketing.Analytics.Reports.OfferOrdersStatistics.OrdersFound', 'Orders found: {0} for a total of {1}'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Views.Funnels.Settings.Common.FunnelName', 'Название воронки'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Views.Funnels.Settings.Common.FunnelName', 'Funnel name'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Views.Funnels.Settings.Common.FunnelNameExample', 'Например: Корм для кошек, Порошковая краска и т.п.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Views.Funnels.Settings.Common.FunnelNameExample', 'For example: Cat food, Powder paint, etc.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Views.Funnels.SiteSettings.Settings', 'Настройки'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Views.Funnels.SiteSettings.Settings', 'Settings'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Views.Funnels.SiteSettings.General', 'Общие'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Views.Funnels.SiteSettings.General', 'General'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Views.Funnels.SiteSettings.Domains', 'Домены'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Views.Funnels.SiteSettings.Domains', 'Domains'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Views.Funnels.SiteSettings.Counters', 'Счетчики'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Views.Funnels.SiteSettings.Counters', 'Counters'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Views.Funnels.SiteSettings.CSSStyles', 'CSS стили'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Views.Funnels.SiteSettings.CSSStyles', 'CSS styles'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Views.Funnels.SiteSettings.SitemapRobots', 'Карта сайта и robots.txt'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Views.Funnels.SiteSettings.SitemapRobots', 'Sitemap and robots.txt'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Views.Funnels.SiteSettings.Upselling', 'Допродажи'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Views.Funnels.SiteSettings.Upselling', 'Upselling'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Views.Funnels.SiteSettings.Access', 'Доступ'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Views.Funnels.SiteSettings.Access', 'Access'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Views.Funnels.SiteSettings.MobileApp', 'Мобильное приложение'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Views.Funnels.SiteSettings.MobileApp', 'Mobile application'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Views.Funnels.Settings.Domains.FunnelInternalURL', 'Внутренний урл воронки'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Views.Funnels.Settings.Domains.FunnelInternalURL', 'Funnel Internal URL'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Views.Funnels.Settings.Domains.IfDomainNotLinked', 'Если не привязан домен, то воронка отображается по внутреннему урл адресу'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Views.Funnels.Settings.Domains.IfDomainNotLinked', 'If the domain is not linked, the funnel is displayed at the internal URL address'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Views.Funnels.Settings.Domains.FunnelDomainSubdomain', 'Домен (или поддомен) воронки'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Views.Funnels.Settings.Domains.FunnelDomainSubdomain', 'Funnel domain (or subdomain)'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Views.Funnels.Settings.Domains.AddedDomains', 'Добавленные домены'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Views.Funnels.Settings.Domains.AddedDomains', 'Added domains'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Views.Funnels.Settings.Domains.ManageDomains', 'Управлять доменами'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Views.Funnels.Settings.Domains.ManageDomains', 'Manage domains'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Views.Funnels.Settings.Domains.AddDomain', 'Добавить домен'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Views.Funnels.Settings.Domains.AddDomain', 'Add domain'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Views.Funnels.Settings.Domains.BuyDomainTextOne', 'Вы можете приобрести новый домен прямо из панели администрирования.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Views.Funnels.Settings.Domains.BuyDomainTextOne', 'You can purchase a new domain directly from the admin panel.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Views.Funnels.Settings.Domains.BuyDomainTextTwo', 'Домен будет юридически оформлен на Вас, все настройки домена будут осуществлены автоматически.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Views.Funnels.Settings.Domains.BuyDomainTextTwo', 'The domain will be legally registered to you, all domain settings will be performed automatically.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Views.Funnels.Settings.Domains.BuyDomainTextThree', 'Магазин начнет открываться по новому домену в течение'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Views.Funnels.Settings.Domains.BuyDomainTextThree', 'The store will start opening on the new domain within'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Views.Funnels.Settings.Domains.BuyDomainCost', 'Купить домен - от 199 руб'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Views.Funnels.Settings.Domains.BuyDomainCost', 'Buy a domain - from 199 rubles'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Views.Funnels.Settings.Domains.BuyDomain', 'Купить домен'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Views.Funnels.Settings.Domains.BuyDomain', 'Buy a domain'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Views.Funnels.Settings.Domains.BuyDomainTextFour', '2 часов'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Views.Funnels.Settings.Domains.BuyDomainTextFour', '2 hours'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Views.Funnels.Settings.Domains.BuyDomainTextFive', 'после оплаты.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Views.Funnels.Settings.Domains.BuyDomainTextFive', 'after payment.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Views.Funnels.Settings.Domains.HaveDomainSubdomain', 'У меня есть домен / поддомен'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Views.Funnels.Settings.Domains.HaveDomainSubdomain', 'I have a domain/subdomain'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Views.Funnels.Settings.Domains.HaveDomainSubdomainTextOne', 'Если у Вас есть ранее приобретённый домен, Вы можете использовать его.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Views.Funnels.Settings.Domains.HaveDomainSubdomainTextOne', 'If you have a previously purchased domain, you can use it.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Views.Funnels.Settings.Domains.HaveDomainSubdomainTextTwo', 'После привязки домена Вам будет необходимо прописать настройки домена на стороне регистратора. Инструкция прилагается.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Views.Funnels.Settings.Domains.HaveDomainSubdomainTextTwo', 'After linking the domain, you will need to register the domain settings on the registrars side. Instructions are attached.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Views.Funnels.Settings.Domains.HaveDomainSubdomainTextThree', 'Магазин начнёт открываться по новому адресу через'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Views.Funnels.Settings.Domains.HaveDomainSubdomainTextThree', 'The store will begin opening at the new address in'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Views.Funnels.Settings.Domains.HaveDomainSubdomainTextFour', '12-24 часа'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Views.Funnels.Settings.Domains.HaveDomainSubdomainTextFour', '12-24 hours'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Views.Funnels.Settings.Domains.HaveDomainSubdomainTextFive', 'после изменения настроек на стороне регистратора.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Views.Funnels.Settings.Domains.HaveDomainSubdomainTextFive', 'after changing the settings on the registrar side.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Views.Funnels.Settings.Domains.HaveDomainSubdomainTextSix', 'Инструкции, как прописать NS'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Views.Funnels.Settings.Domains.HaveDomainSubdomainTextSix', 'Instructions on how to register NS'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Views.Funnels.Settings.Domains.HaveDomainSubdomainTextSeven', 'В случае с поддоменом, следуйте инструкции в разделе "Все домены"'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Views.Funnels.Settings.Domains.HaveDomainSubdomainTextSeven', 'In case of a subdomain, follow the instructions in the "All domains" section'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Views.Funnels.Settings.Domains.ConnectDomain', 'Подключить домен'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Views.Funnels.Settings.Domains.ConnectDomain', 'Connect domain'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Views.Funnels.Settings.Domains.UsePreviouslyDomain', 'Использовать ранее подключенный домен'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Views.Funnels.Settings.Domains.UsePreviouslyDomain', 'Use a previously connected domain'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Views.Funnels.Settings.Domains.UsePreviouslyDomainTextOne', 'Если ранее вы уже подключили домен или поддомен, то вы можете переключить его использование для данной воронки.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Views.Funnels.Settings.Domains.UsePreviouslyDomainTextOne', 'If you have previously connected a domain or subdomain, you can switch its use for this funnel.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Views.Funnels.Settings.Domains.UsePreviouslyDomainTextTwo', 'Изменение привязки происходит быстро, в течение 1 минуты уже будет открываться новое содержимое.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Views.Funnels.Settings.Domains.UsePreviouslyDomainTextTwo', 'Changing the binding happens quickly, within 1 minute the new content will already be opening.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Views.Funnels.Settings.Domains.ChangeBinding', 'Изменить привязку'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Views.Funnels.Settings.Domains.ChangeBinding', 'Change binding'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Views.Funnels.Settings.Domains.UsePreviouslyAdded', 'Использовать ранее добавленный'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Views.Funnels.Settings.Domains.UsePreviouslyAdded', 'Use previously added'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Views.Funnels.Settings.Domains.New', 'Новый'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Views.Funnels.Settings.Domains.New', 'New'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Views.Funnels.Settings.Domains.EnterDomainNameSubdomain', 'Введите доменное имя или поддомен'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Views.Funnels.Settings.Domains.EnterDomainNameSubdomain', 'Enter a domain name or subdomain'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Views.Funnels.Settings.Domains.Add', 'Добавить'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Views.Funnels.Settings.Domains.Add', 'Add'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Views.Funnels.Settings.Domains.PleaseNote', 'Обратите внимание:'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Views.Funnels.Settings.Domains.PleaseNote', 'Please note:'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Views.Funnels.Settings.Domains.PleaseNoteTextOne', 'Домен/поддомен следует добавить без приставки www или http. Например: moysite.ru, мойсайт.рф или lp.moysite.ru.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Views.Funnels.Settings.Domains.PleaseNoteTextOne', 'The domain/subdomain should be added without the www or http prefix. For example: moysite.ru, мойсайт.рф or lp.moysite.ru.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Views.Funnels.Settings.Domains.PleaseNoteTextTwo', 'Данный список лишь указывает, какой домен используется для воронки, сам домен должен быть куплен и привязан к сайту заранее.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Views.Funnels.Settings.Domains.PleaseNoteTextTwo', 'This list only indicates which domain is used for the funnel, the domain itself must be purchased and linked to the site in advance.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Views.Funnels.Settings.Domains.SelectDomainNameSubdomain', 'Выберите доменное имя или поддомен'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Views.Funnels.Settings.Domains.SelectDomainNameSubdomain', 'Select a domain name or subdomain'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Views.Funnels.Settings.Domains.Toggle', 'Переключить'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Views.Funnels.Settings.Domains.Toggle', 'Toggle'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Views.Funnels.Settings.Domains.AfterChangingBinding', 'После изменения привязки, старые ссылки перестанут открываться.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Views.Funnels.Settings.Domains.AfterChangingBinding', 'After changing the binding, old links will no longer open.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Views.Funnels.Settings.Domains.BindingChanges', 'Изменения привязки происходят быстро, в течении 1 минуты уже будет открываться новое содержимое.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Views.Funnels.Settings.Domains.BindingChanges', 'Binding changes happen quickly, within 1 minute new content will be opened.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Views.Funnels.Settings.Domains.NoConnectedDomains', 'Пока нет подключенных доменов для подключения'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Views.Funnels.Settings.Domains.NoConnectedDomains', 'There are no connected domains to connect yet'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Views.Funnels.Settings.Domains.Open', 'Открыть'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Views.Funnels.Settings.Domains.Open', 'Open'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Views.Funnels.Settings.SeoCounters.HtmlHEAD', 'Html-код для вставки внутрь HEAD'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Views.Funnels.Settings.SeoCounters.HtmlHEAD', 'Html code to insert inside HEAD'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Views.Funnels.Settings.SeoCounters.HtmlBODY', 'Html-код для вставки внутрь BODY'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Views.Funnels.Settings.SeoCounters.HtmlBODY', 'Html code to insert inside BODY'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Views.Funnels.Settings.SeoCounters.HideCopyright', 'Скрыть копирайт'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Views.Funnels.Settings.SeoCounters.HideCopyright', 'Hide copyright'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Views.Funnels.Settings.SeoCounters.YandexMetrica', 'Яндекс.Метрика'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Views.Funnels.Settings.SeoCounters.YandexMetrica', 'Yandex.Metrica'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Views.Funnels.Settings.SeoCounters.MeterNumberMetrics', 'Номер счётчика в метрике'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Views.Funnels.Settings.SeoCounters.MeterNumberMetrics', 'Meter number in metrics'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Views.Funnels.Settings.SeoCounters.CounterCode', 'Код счетчика'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Views.Funnels.Settings.SeoCounters.CounterCode', 'Counter code'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Views.Funnels.Settings.SeoCounters.GoogleAnalyticsAccount', 'Аккаунт Google Analytics'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Views.Funnels.Settings.SeoCounters.GoogleAnalyticsAccount', 'Google Analytics Account'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Views.Funnels.Settings.SeoCounters.LineLike', '(строка вида: 12345678-1)'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Views.Funnels.Settings.SeoCounters.LineLike', '(line like: 12345678-1)'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Views.Funnels.Settings.SeoCounters.ContainerID', 'Идентификатор контейнера'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Views.Funnels.Settings.SeoCounters.ContainerID', 'Container ID'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Views.Funnels.Settings.CSSStyles.CSSStyles', 'CSS стили для всех страниц воронки'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Views.Funnels.Settings.CSSStyles.CSSStyles', 'CSS styles for all pages of the funnel'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Views.Funnels.Settings.Auth.RequireLogin', 'Требовать логин и пароль для доступа ко всем страницам воронки'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Views.Funnels.Settings.Auth.RequireLogin', 'Require login and password to access all funnel pages'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Views.Funnels.Settings.Auth.LinkRegistrationPage', 'Ссылка на страницу регистрации'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Views.Funnels.Settings.Auth.LinkRegistrationPage', 'Link to the registration page'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Views.Funnels.Settings.Auth.HereCanSpecifyAddress', 'Здесь можно указать адрес страницы, где посетитель сможет получить/купить доступ к страницам воронки'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Views.Funnels.Settings.Auth.HereCanSpecifyAddress', 'Here you can specify the address of the page where the visitor can get/buy access to the funnel pages'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Views.Funnels.Settings.Auth.PageDisplayConditions', 'Условия отображения страницы'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Views.Funnels.Settings.Auth.PageDisplayConditions', 'Page display conditions'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Views.Funnels.Settings.Auth.AddProducts', 'Добавить товары'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Views.Funnels.Settings.Auth.AddProducts', 'Add products'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Views.Funnels.Settings.Auth.LeadList', 'Список лидов'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Views.Funnels.Settings.Auth.LeadList', 'Lead List'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Views.Funnels.Settings.Auth.Status', 'Статус'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Views.Funnels.Settings.Auth.Status', 'Status'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Views.Funnels.Settings.Auth.Any', 'Любой'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Views.Funnels.Settings.Auth.Any', 'Any'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.Index.ViewSite', 'Смотреть сайт'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.Index.ViewSite', 'View site'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.Index.QRCode', 'QR код'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.Index.QRCode', 'QR code'
               
GO--
               
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.LandingFunnelOrderWithForm.Name', 'Воронка "Один товар с допродажами"'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.LandingFunnelOrderWithForm.Name', 'Funnel "One product with upsells"'
                    
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.LandingFunnelOrderWithForm.Description', 'Это одна из наших любимых воронок. Она позволяет окупать трафик и зарабатывать больше на одном клиенте.<br><br> Основной товар здесь предлагается по максимально привлекательной цене. Она должна быть настолько заманчивой, чтобы клиенту было сложно отказаться от покупки. После оформления основного заказа, ему предлагается два дополнительных товара, тоже нужных и ценных для него. Покупка товаров из допродажи увеличивает средний чек.<br><br> С помощью этой воронки можно продавать не только товары, но и услуги.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.LandingFunnelOrderWithForm.Description', 'This is one of our favorite funnels. It allows you to recoup your traffic and earn more from the client.<br><br> Here, the main product is offered at the most attractive price. It should be so tempting that it is difficult for the client to refuse the purchase. After placing the main order, he is offered two additional products, also necessary and valuable for him. Buying products from the upsell increases the average check.<br><br> With this funnel, you can sell not only products, but also services.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.LandingFunnelOrderWithForm.Description.Title.Downsell', 'Допродажа 2 (Downsell)'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.LandingFunnelOrderWithForm.Description.Title.Downsell', 'Upsell 2 (Downsell)'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.LandingFunnelOrderWithForm.Description.Title.MainOfferPage', 'Страница основного предложения'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.LandingFunnelOrderWithForm.Description.Title.MainOfferPage', 'Main Offer Page'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.LandingFunnelOrderWithForm.Description.Title.PlacingOrder', 'Оформление заказа'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.LandingFunnelOrderWithForm.Description.Title.PlacingOrder', 'Placing an order'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.LandingFunnelOrderWithForm.Description.Title.Upsell', 'Допродажа 1 (Upsell)'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.LandingFunnelOrderWithForm.Description.Title.Upsell', 'Upsell 1'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.LandingFunnelOrderWithForm.Description.Title.ThankPage', 'Страница благодарности'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.LandingFunnelOrderWithForm.Description.Title.ThankPage', 'Thank you page'
               
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.LandingFunnelOrderWithDetails.Name', 'Воронка "Один товар с допродажами. Детально"'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.LandingFunnelOrderWithDetails.Name', 'Funnel "One product with upsells. Detailed"'
                    
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.LandingFunnelOrderWithDetails.Description', 'Это одна из наших любимых воронок. Она позволяет окупать трафик и зарабатывать больше на одном клиенте.<br><br> Основной товар здесь предлагается по максимально привлекательной цене. Она должна быть настолько заманчивой, чтобы клиенту было сложно отказаться от покупки. После оформления основного заказа, ему предлагается два дополнительных товара, тоже нужных и ценных для него. Покупка товаров из допродажи увеличивает средний чек.<br><br> С помощью этой воронки можно продавать не только товары, но и услуги.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.LandingFunnelOrderWithDetails.Description', 'This is one of our favorite funnels. It allows you to recoup traffic and earn more per client.<br><br> The main product is offered here at the most attractive price. It should be so tempting that it is difficult for the client to refuse the purchase. After placing the main order, he is offered two additional products, also necessary and valuable for him. Buying products from upselling increases the average check.<br><br> With this funnel, you can sell not only products, but also services.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.LandingFunnelOrderWithDetails.Description.Title.Downsell', 'Допродажа 2 (Downsell)'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.LandingFunnelOrderWithDetails.Description.Title.Downsell', 'Upsell 2 (Downsell)'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.LandingFunnelOrderWithDetails.Description.Title.MainOfferPage', 'Страница основного предложения'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.LandingFunnelOrderWithDetails.Description.Title.MainOfferPage', 'Main Offer Page'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.LandingFunnelOrderWithDetails.Description.Title.PlacingOrder', 'Оформление заказа'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.LandingFunnelOrderWithDetails.Description.Title.PlacingOrder', 'Placing an order'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.LandingFunnelOrderWithDetails.Description.Title.Upsell', 'Допродажа 1 (Upsell)'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.LandingFunnelOrderWithDetails.Description.Title.Upsell', 'Upsell 1'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.LandingFunnelOrderWithDetails.Description.Title.ThankPage', 'Страница благодарности'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.LandingFunnelOrderWithDetails.Description.Title.ThankPage', 'Thank you page'
    
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.MultyProducts.Name', 'Воронка "Мультитоварная"'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.MultyProducts.Name', 'Funnel "Multi-product"'
                    
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.MultyProducts.Description', 'С помощью этой воронки вы можете продавать несколько товаров на одной странице, где ограничено количество отвлекающих действий. Так клиент не запутается и положит в корзину один из вариантов товара, представленного на странице. Кроме того, клиент сможет оплатить заказ прямо на страницах воронки.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.MultyProducts.Description', 'With this funnel, you can sell several products on one page, where the number of distracting actions is limited. This way, the client will not get confused and will put one of the product options presented on the page in the cart. In addition, the client will be able to pay for the order directly on the funnel pages.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.MultyProducts.Title.OffersPage', 'Страница с предложениями'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.MultyProducts.Title.OffersPage', 'Offers Page'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.MultyProducts.Title.PlacingOrder', 'Оформление заказа'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.MultyProducts.Title.PlacingOrder', 'Placing an order'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.MultyProducts.Title.Upsell', 'Допродажа 1 (Upsell)'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.MultyProducts.Title.Upsell', 'Upsell 1'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.MultyProducts.Title.Downsell', 'Допродажа 2 (Downsell)'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.MultyProducts.Title.Downsell', 'Upsell 2 (Downsell)'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.MultyProducts.Title.ThankPage', 'Страница благодарности'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.MultyProducts.Title.ThankPage', 'Thank you page'
               
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.MultyProductsShort.Name', 'Воронка "Мультитоварная упрощенная"'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.MultyProductsShort.Name', 'Funnel "Multi-product simplified"'
                    
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.MultyProductsShort.Description', 'С помощью этой воронки вы можете продавать несколько товаров на одной странице, где ограничено количество отвлекающих действий. Так клиент не запутается и положит в корзину один из вариантов товара, представленного на странице. Кроме того, клиент сможет оплатить заказ прямо на страницах воронки.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.MultyProductsShort.Description', 'With this funnel, you can sell several products on one page, where the number of distracting actions is limited. This way, the client will not get confused and will put one of the product options presented on the page in the cart. In addition, the client will be able to pay for the order directly on the funnel pages.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.MultyProductsShort.Title.OffersPage', 'Страница с предложениями'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.MultyProductsShort.Title.OffersPage', 'Offers Page'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.MultyProductsShort.Title.PlacingOrder', 'Оформление заказа'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.MultyProductsShort.Title.PlacingOrder', 'Placing an order'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.MultyProductsShort.Title.ThankPage', 'Страница благодарности'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.MultyProductsShort.Title.ThankPage', 'Thank you page'
            
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.MultyProductsWithCategories.Name', 'Воронка "Мультитоварная с категориями"'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.MultyProductsWithCategories.Name', 'Funnel "Multi-product with categories"'
                    
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.MultyProductsWithCategories.Description', 'С помощью этой воронки вы можете продавать несколько товаров на одной странице, где ограничено количество отвлекающих действий. Так клиент не запутается и положит в корзину один из вариантов товара, представленного на странице. Кроме того, клиент сможет оплатить заказ прямо на страницах воронки.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.MultyProductsWithCategories.Description', 'With this funnel, you can sell several products on one page, where the number of distracting actions is limited. This way, the client will not get confused and will put one of the product options presented on the page in the cart. In addition, the client will be able to pay for the order directly on the funnel pages.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.MultyProductsWithCategories.Title.OffersPage', 'Страница с предложениями'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.MultyProductsWithCategories.Title.OffersPage', 'Offers Page'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.MultyProductsWithCategories.Title.PlacingOrder', 'Оформление заказа'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.MultyProductsWithCategories.Title.PlacingOrder', 'Placing an order'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.MultyProductsWithCategories.Title.Upsell', 'Допродажа 1 (Upsell)'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.MultyProductsWithCategories.Title.Upsell', 'Upsell 1'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.MultyProductsWithCategories.Title.Downsell', 'Допродажа 2 (Downsell)'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.MultyProductsWithCategories.Title.Downsell', 'Upsell 2 (Downsell)'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.MultyProductsWithCategories.Title.ThankPage', 'Страница благодарности'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.MultyProductsWithCategories.Title.ThankPage', 'Thank you page'
               
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.LandingFunnelOrderFreeWithDelivery.Name', 'Воронка "Бесплатный товар + доставка"'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.LandingFunnelOrderFreeWithDelivery.Name', 'Funnel "Free product + shipping"'
                    
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.LandingFunnelOrderFreeWithDelivery.Description', 'На первой странице вовлекайте покупателя специальным предложением. Например, предлагайте товар бесплатно, с условием оплатить только доставку. На следующих страницах воронки предлагайте дополнительные товары, уже за полную стоимость. Это помогает увеличивать средний чек.<br><br>Стоимость доставки должна окупать стоимость бесплатного товара, а допродажи всегда помогают заработать на заказах.<br><br>Эта воронка может иметь одну из лучших конверсий в заказы.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.LandingFunnelOrderFreeWithDelivery.Description', 'On the first page, engage the buyer with a special offer. For example, offer a product for free, with the condition that they pay only for shipping. On the following pages of the funnel, offer additional products, but at full price. This helps increase the average check.<br><br>The cost of shipping should cover the cost of the free product, and upselling always helps make money on orders.<br><br>This funnel can have one of the best conversions into orders.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.LandingFunnelOrderFreeWithDelivery.Title.OffersPage', 'Страница с предложениями'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.LandingFunnelOrderFreeWithDelivery.Title.OffersPage', 'Offers Page'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.LandingFunnelOrderFreeWithDelivery.Title.PlacingOrder', 'Оформление заказа'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.LandingFunnelOrderFreeWithDelivery.Title.PlacingOrder', 'Placing an order'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.LandingFunnelOrderFreeWithDelivery.Title.Upsell', 'Допродажа 1 (Upsell)'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.LandingFunnelOrderFreeWithDelivery.Title.Upsell', 'Upsell 1'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.LandingFunnelOrderFreeWithDelivery.Title.Downsell', 'Допродажа 2 (Downsell)'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.LandingFunnelOrderFreeWithDelivery.Title.Downsell', 'Upsell 2 (Downsell)'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.LandingFunnelOrderFreeWithDelivery.Title.ThankPage', 'Страница благодарности'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.LandingFunnelOrderFreeWithDelivery.Title.ThankPage', 'Thank you page'
               
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.VideoWithCrossSells.Name', 'Воронка "Видеопредложение с допродажами"'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.VideoWithCrossSells.Name', 'Funnel "Video offer with upsells"'
                    
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.VideoWithCrossSells.Description', 'Очень простая воронка. Вы показываете видеопредложение «прогретой», подготовленной к покупке аудитории и размещаете под видео кнопку «заказать».'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.VideoWithCrossSells.Description', 'A very simple funnel. You show a video offer to a "warmed up" audience ready to buy and place an "order" button under the video.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.VideoWithCrossSells.Title.OffersPage', 'Страница с предложениями'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.VideoWithCrossSells.Title.OffersPage', 'Offers Page'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.VideoWithCrossSells.Title.PlacingOrder', 'Оформление заказа'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.VideoWithCrossSells.Title.PlacingOrder', 'Placing an order'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.VideoWithCrossSells.Title.Upsell', 'Допродажа 1 (Upsell)'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.VideoWithCrossSells.Title.Upsell', 'Upsell 1'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.VideoWithCrossSells.Title.Downsell', 'Допродажа 2 (Downsell)'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.VideoWithCrossSells.Title.Downsell', 'Upsell 2 (Downsell)'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.VideoWithCrossSells.Title.ThankPage', 'Страница благодарности'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.VideoWithCrossSells.Title.ThankPage', 'Thank you page'
               
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.ArticleWithCrossSells.Name', 'Воронка "Продающая статья с допродажами"'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.ArticleWithCrossSells.Name', 'Funnel "Selling article with upsells"'
                    
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.ArticleWithCrossSells.Description', 'В этой воронке вы рассказываете, как клиенты могут решить свою проблему с помощью вашего товара или услуги. Напишите для этого продающую статью, используя технику «Крючок — история — предложение». Привлеките внимание людей цепким заголовком. Расскажите им свою, похожую, историю, когда вы находились в этом же состоянии: не знали, что делать, ошибались, но в итоге решили этот вопрос. Затем предложите то, что принесло вам результат.<br><br>Когда вы продали товар или услугу с помощью статьи, опять же, используйте страницы допродаж на этапе оформления основного заказа.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.ArticleWithCrossSells.Description', 'In this funnel, you tell how customers can solve their problem with your product or service. Write a sales article for this, using the “Hook - Story - Offer” technique. Grab people’s attention with a catchy headline. Tell them your similar story, when you were in the same situation: you didn’t know what to do, you made mistakes, but eventually you solved this issue. Then offer what brought you results.<br><br>When you have sold a product or service with the help of an article, again, use upselling pages at the stage of placing the main order.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.ArticleWithCrossSells.Title.SellingArticle', 'Продающая статья'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.ArticleWithCrossSells.Title.SellingArticle', 'Selling article'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.ArticleWithCrossSells.Title.PlacingOrder', 'Оформление заказа'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.ArticleWithCrossSells.Title.PlacingOrder', 'Placing an order'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.ArticleWithCrossSells.Title.Upsell', 'Допродажа 1 (Upsell)'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.ArticleWithCrossSells.Title.Upsell', 'Upsell 1'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.ArticleWithCrossSells.Title.Downsell', 'Допродажа 2 (Downsell)'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.ArticleWithCrossSells.Title.Downsell', 'Upsell 2 (Downsell)'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.ArticleWithCrossSells.Title.ThankPage', 'Страница благодарности'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.ArticleWithCrossSells.Title.ThankPage', 'Thank you page'
               
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.CompanySiteCourse.Name', 'Воронка "Онлайн-школа"'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.CompanySiteCourse.Name', 'Funnel "Online School"'
                    
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.CompanySiteCourse.Description', 'Воронка Онлайн-школы представляет собой страницу с информацией: о чем ваш курс, для кого вы его создали, что в нем будет, сколько он стоит, отзывы других учеников. После приобретения курса покупатель попадает в личный кабинет ученика с доступом к ценным материалам.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.CompanySiteCourse.Description', 'The Online School Funnel is a page with information: what your course is about, who you created it for, what it will contain, how much it costs, reviews from other students. After purchasing the course, the buyer gets into the students personal account with access to valuable materials.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.CompanySiteCourse.Title.SellingCourse', 'Продажа курса'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.CompanySiteCourse.Title.SellingCourse', 'Selling a course'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.CompanySiteCourse.Title.CourseOffice', 'Кабинет курса'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.CompanySiteCourse.Title.CourseOffice', 'Course office'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.CompanySiteCourse.Title.InternalPageExample', 'Пример внутренней страницы'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.CompanySiteCourse.Title.InternalPageExample', 'Example of an internal page'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.CompanySiteCourse.Title.ThankPage', 'Страница благодарности'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.CompanySiteCourse.Title.ThankPage', 'Thank you page'
               
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.ContactForContent.Name', 'Воронка "Захват контакта за контент"'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.ContactForContent.Name', 'Funnel "Contact capture for content"'
                    
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.ContactForContent.Description', 'Обмен пользы на контакт. На первой странице разместите привлекательный заголовок, который обещает клиенту что-то ценное. Чтобы получить нужную информацию или файл, он должен оставить адрес электронной почты в специальном окошке. Полезный контент автоматически отправляется на указанный адрес, а адрес остается в вашей базе.<br><br>После того как посетитель оставит свой email, отправьте его на страницу благодарности.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.ContactForContent.Description', 'Exchange benefits for contact. On the first page, place an attractive headline that promises something valuable to the client. To receive the necessary information or file, he must leave an email address in a special window. Useful content is automatically sent to the specified address, and the address remains in your database.<br><br>After the visitor leaves his email, send him to the thank you page.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.ContactForContent.Title.CollectingContacts', 'Сбор контактов'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.ContactForContent.Title.CollectingContacts', 'Collecting contacts'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.ContactForContent.Title.ThankPage', 'Страница благодарности'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.ContactForContent.Title.ThankPage', 'Thank you page'
               
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.ArticleLead.Name', 'Воронка "Статья"'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.ArticleLead.Name', 'Funnel "Article"'
                    
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.ArticleLead.Description', 'Поделитесь ценной и полезной информацией с вашими потенциальными клиентами в статье. Встройте туда ссылку на ваше предложение. Статья помогает убедить потенциальных покупателей, которые только изучают вопрос, что решение этой проблемы уже есть — у вас.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.ArticleLead.Description', 'Share valuable and useful information with your potential customers in an article. Embed a link to your offer there. The article helps convince potential buyers who are just studying the issue that the solution to this problem already exists - you have it.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.ArticleLead.Title.CollectingContacts', 'Сбор контактов'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.ArticleLead.Title.CollectingContacts', 'Collecting contacts'
               
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.CouponWithDiscount.Name', 'Воронка "Получи купон на скидку"'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.CouponWithDiscount.Name', 'Funnel "Get a discount coupon"'
                    
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.CouponWithDiscount.Description', 'Предложите скидочный купон взамен на адрес электронной почты. Перенаправьте посетителя на страницу предложения, где он сможет использовать полученный купон. Это может быть интернет-магазин, мини-каталог или страница с одним предложением.<br><br>Отправьте купон на указанную электронную почту.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.CouponWithDiscount.Description', 'Offer a discount coupon in exchange for an email address. Redirect the visitor to the offer page where they can use the coupon they received. This could be an online store, a mini-catalog, or a page with one offer.<br><br>Send the coupon to the specified email.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.CouponWithDiscount.Title.CollectingContacts', 'Сбор контактов'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.CouponWithDiscount.Title.CollectingContacts', 'Collecting contacts'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.CouponWithDiscount.Title.ThankPage', 'Страница благодарности'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.CouponWithDiscount.Title.ThankPage', 'Thank you page'
               
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.VideoLeadMagnetNew.Name', 'Воронка "Лид-магнит "Видео""'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.VideoLeadMagnetNew.Name', 'Funnel "Lead Magnet "Video""'
                    
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.VideoLeadMagnetNew.Description', 'Дайте полезную информацию прежде, чем будете просить клиента оставить свои контакты. Так вас услышит и узнает большее количество людей, ведь им не нужно ничего оставлять взамен. В данном случае вы соберете меньше лидов, но их качество будет значительно выше, чем в схеме «вначале контакт — потом польза».<br><br>После просмотра первого видео предложите клиенту получить еще больше полезной информации, которую вы сможете ему отправить, если он оставит e-mail.<br><br>Для этой воронки подготовьте лучшую информацию, что у вас есть.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.VideoLeadMagnetNew.Description', 'Provide useful information before asking the client to leave their contact information. This way, more people will hear and know you, because they don’t have to leave anything in return. In this case, you will collect fewer leads, but their quality will be much higher than in the “contact first – benefit later” scheme.<br><br>After watching the first video, offer the client to receive even more useful information that you can send them if they leave their email.<br><br>For this funnel, prepare the best information you have.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.VideoLeadMagnetNew.Title.CollectingContacts', 'Сбор контактов'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.VideoLeadMagnetNew.Title.CollectingContacts', 'Collecting contacts'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.VideoLeadMagnetNew.Title.ThankPage', 'Страница благодарности'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.VideoLeadMagnetNew.Title.ThankPage', 'Thank you page'
               
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Orders.OrderInfo.Change', 'Изменить'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Orders.OrderInfo.Change', 'Change'
               
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.CollectContactsForAccess.Name', 'Воронка "Доступ к категории"'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.CollectContactsForAccess.Name', 'Funnel "Access to category"'
                    
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.CollectContactsForAccess.Description', 'Эта воронка также помогает собирать контакты потенциальных клиентов. На отдельной странице разместите ваше лучшее предложение. Чтобы посмотреть другие товары из этой категории, пользователь должен будет оставить email. После того как он заполнит поле для электронного адреса, перенаправьте его в каталог вашего магазина.<br><br> Помните, что вы собираете контакты не просто так, а чтобы общаться с вашими клиентами, через правильные письма стимулировать их к покупкам.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.CollectContactsForAccess.Description', 'This funnel also helps to collect contacts of potential customers. On a separate page, place your best offer. To view other products from this category, the user will have to leave an email. After he fills in the email field, redirect him to the catalog of your store.<br><br> Remember that you are collecting contacts not just like that, but to communicate with your customers, through the right letters to encourage them to buy.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.CollectContactsForAccess.Title.CollectingContact', 'Сбор контакта'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.CollectContactsForAccess.Title.CollectingContact', 'Collecting contact'
               
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.ContactForEBook.Name', 'Воронка "Захват контакта за эл. книгу"'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.ContactForEBook.Name', 'Funnel "Capture contact for e-book"'
                    
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.ContactForEBook.Description', 'Подключайте эту воронку, чтобы расширить базу потенциальных клиентов. Предложите вашим посетителям бесплатно скачать небольшую, но ценную для них электронную книгу, в обмен на e-mail. После того как вы получили контактные данные, перенаправьте пользователя на страницу благодарности. В тот же момент необходимо отправить ему автоматическое письмо со ссылкой на скачивание обещанной книги. Вы всегда сможете составить такую электронную книгу на базе статей, ранее написанных вашей компанией. Книга не должна быть большой. Набора из 5-7 статьей будет достаточно.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.ContactForEBook.Description', 'Connect this funnel to expand your potential customer base. Offer your visitors a free download of a small but valuable e-book in exchange for an e-mail. After you have received their contact information, redirect the user to a thank you page. At the same time, you need to send them an automatic letter with a link to download the promised book. You can always create such an e-book based on articles previously written by your company. The book does not have to be large. A set of 5-7 articles will be enough.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.ContactForEBook.Title.CollectingContact', 'Сбор контакта'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.ContactForEBook.Title.CollectingContact', 'Collecting contact'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.ContactForEBook.Title.ThankPage', 'Страница благодарности'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.ContactForEBook.Title.ThankPage', 'Thank you page'
       
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.ConsultingNew.Name', 'Воронка "Консалтинг"'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.ConsultingNew.Name', 'Funnel "Consulting"'
                    
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.ConsultingNew.Description', 'С помощью короткого ролика на первой странице привлеките внимание клиента, обозначьте волнующие его вопросы и предложите записаться на консультацию прямо на странице. Можно добавить анкету, которая поможет вам подготовиться к консультации.<br><br>Помогите клиенту, ответив лично на все его вопросы, чтобы он убедился, что именно ваш продукт ему подходит. Такая коммуникация повышает доверие и помогает выстроить правильные, крепкие отношения.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.ConsultingNew.Description', 'With a short video on the first page, grab the clients attention, outline their concerns, and offer to sign up for a consultation right on the page. You can add a questionnaire that will help you prepare for the consultation.<br><br>Help the client by personally answering all their questions so that they can be sure that your product is right for them. Such communication increases trust and helps build the right, strong relationships.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.ConsultingNew.Title.OfferWithVideo', 'Предложение с видео'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.ConsultingNew.Title.OfferWithVideo', 'Offer with video'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.ConsultingNew.Title.UsefulContent', 'Полезный контент'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.ConsultingNew.Title.UsefulContent', 'Useful content'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.ConsultingNew.Title.Questionnaire', 'Анкета'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.ConsultingNew.Title.Questionnaire', 'Questionnaire'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.ConsultingNew.Title.ThankPage', 'Страница благодарности'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.ConsultingNew.Title.ThankPage', 'Thank you page'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.Lottery.Name', 'Воронка "Розыгрыш"'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.Lottery.Name', 'Funnel "Draw"'
                    
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.Lottery.Description', 'Розыгрыш призов — это хороший способ привлечь аудиторию и познакомить ее с вашей компанией.<br><br>Продумайте условия участия. Например: «подпишитесь на нашу страницу в соцсетях», «отметьте друзей», «напишите комментарий», «поставьте лайки». Эти простые действия со стороны пользователей помогают собирать новую аудиторию, продвигать ваши посты и ролики.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.Lottery.Description', 'A prize draw is a good way to attract an audience and introduce them to your company.<br><br>Think about the conditions of participation. For example: "subscribe to our page on social networks", "tag friends", "write a comment", "like". These simple actions on the part of users help to collect a new audience, promote your posts and videos.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.Lottery.Title.OfferPage', 'Страница предложения'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.Lottery.Title.OfferPage', 'Offer page'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.Lottery.Title.ThankPage', 'Страница благодарности'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.Lottery.Title.ThankPage', 'Thank you page'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.QuizFunnelNew.Name', 'Воронка "Квиз"'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.QuizFunnelNew.Name', 'Funnel "Quiz"'
                    
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.QuizFunnelNew.Description', 'Эта воронка помогает предложить клиенту наиболее подходящий ему товар. Пользователь отвечает на вопросы по теме и, в зависимости от выбранных ответов, попадает на страницу с тем или иным предложением.<br><br>Подробнее об этой механике мы уже писали в рецепте №2.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.QuizFunnelNew.Description', 'This funnel helps to offer the client the most suitable product. The user answers questions on the topic and, depending on the selected answers, gets to the page with one or another offer.<br><br>We have already written about this mechanic in more detail in recipe #2.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.QuizFunnelNew.Title.InvitationQuiz', 'Приглашение пройти квиз'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.QuizFunnelNew.Title.InvitationQuiz', 'Invitation to take a quiz'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.QuizFunnelNew.Title.QuizPage', 'Страница квиза'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.QuizFunnelNew.Title.QuizPage', 'Quiz Page'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.QuizFunnelNew.Title.OtherPages', 'Другие страницы, в зависимости от типа предложения'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.QuizFunnelNew.Title.OtherPages', 'Other pages depending on the type of offer'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.Questionnaire.Name', 'Воронка "Анкета"'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.Questionnaire.Name', 'Funnel "Questionnaire"'
                    
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.Questionnaire.Description', 'Соберите у клиентов нужную вам информацию, а затем используйте ее для улучшения продаж. Вы можете разместить любое количество вопросов. Все данные об ответах будут автоматически сохранены в CRM-системе ADVANTSHOP, в карточке покупателя. Далее, используя эту информацию, вы сможете фильтровать клиентов, распределять их по категориям и делать персональные рассылки для каждой категории.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.Questionnaire.Description', 'Collect the information you need from your customers and then use it to improve sales. You can place any number of questions. All data on the answers will be automatically saved in the ADVANTSHOP CRM system, in the customer card. Then, using this information, you can filter customers, distribute them by category and make personalized mailings for each category.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.Questionnaire.Title.Questionnaire', 'Анкета'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.Questionnaire.Title.Questionnaire', 'Questionnaire'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.Questionnaire.Title.ThankPage', 'Страница благодарности'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.Questionnaire.Title.ThankPage', 'Thank you page'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.EventAction.Name', 'Воронка "Мероприятие"'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.EventAction.Name', 'Funnel "Event"'
                    
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.EventAction.Description', 'Основная задача этой воронки — записать максимально возможное число посетителей на мероприятие, если оно бесплатное. Или продать как можно больше билетов на платное мероприятие. За день до события вы должны отправить напоминания о нем всем зарегистрированным участникам.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.EventAction.Description', 'The main goal of this funnel is to register the maximum possible number of visitors to the event, if it is free. Or to sell as many tickets as possible to a paid event. The day before the event, you should send reminders about it to all registered participants.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.EventAction.Title.SignEvent', 'Запись на мероприятие'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.EventAction.Title.SignEvent', 'Sign up for the event'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.EventAction.Title.ThankPage', 'Страница благодарности'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.EventAction.Title.ThankPage', 'Thank you page'
             
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.Webinar.Name', 'Воронка "Вебинар"'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.Webinar.Name', 'Funnel "Webinar"'
                    
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.Webinar.Description', 'Задача вебинарной воронки — привести как можно больше людей на ваш веб-семинар. Обеспечить максимальную посещаемость помогут страница регистрации и страница благодарности. Ваша задача — в день проведения вебинара отправить клиентам напоминания о нем. Вы можете сделать это с помощью автоматической рассылки смс или электронных писем.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.Webinar.Description', 'The goal of a webinar funnel is to bring as many people as possible to your webinar. A registration page and a thank you page will help ensure maximum attendance. Your goal is to send reminders to your clients on the day of the webinar. You can do this by sending automated SMS or emails.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.Webinar.Title.SignWebinar', 'Запись на вебинар'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.Webinar.Title.SignWebinar', 'Sign up for the webinar'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.Webinar.Title.ThankPage', 'Страница благодарности'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.Webinar.Title.ThankPage', 'Thank you page'  
               
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.CompanySiteWithLeads.Name', 'Сайт компании, сбор заявок'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.CompanySiteWithLeads.Name', 'Company website, collection of applications'
                    
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.CompanySiteWithLeads.Description', 'Страница компании нужна, если требуется рассказать о вашей компании, ее услугах и товарах в формате официального веб-сайта. Часто сайт компании требуется, чтобы разместить здесь ваше портфолио, прайс-лист, контакты и форму обратной связи.<br><br>Вы можете добавить любое количество блоков на страницу: «О сотрудниках», «Отзывы», «Фотогалерея» и другие. Для этого используйте встроенный конструктор страниц в ADVANTSHOP.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.CompanySiteWithLeads.Description', 'A company page is needed if you want to tell about your company, its services and products in the format of an official website. Often, a company website is required to place your portfolio, price list, contacts and feedback form here.<br><br>You can add any number of blocks to the page: "About employees", "Reviews", "Photo gallery" and others. To do this, use the built-in page builder in ADVANTSHOP.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.CompanySiteWithLeads.Intent', '<ul><li>Познакомить аудиторию с товарами и услугами компании</li><li>Предоставить контактную информацию</li><li>Продать товары и услуги</li></ul>'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.CompanySiteWithLeads.Intent', '<ul><li>Introduce the audience to the companys products and services</li><li>Provide contact information</li><li>Sell products and services</li></ul>'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.CompanySiteWithLeads.Advice', 'Используйте как можно больше собственных фотографий и меньше копируйте из Интернета.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.CompanySiteWithLeads.Advice', 'Use as many of your own photos as possible and copy as little as possible from the Internet.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.CompanySiteWithLeads.Title.CollectingContacts', 'Сбор контактов'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.CompanySiteWithLeads.Title.CollectingContacts', 'Collecting contacts'
               
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.CompanySiteWithCatalog.Name', 'Сайт компании с каталогом товаров'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.CompanySiteWithCatalog.Name', 'Company website with product catalog'
                    
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.CompanySiteWithCatalog.Description', 'Страница компании нужна, если требуется рассказать о вашей компании, ее услугах и товарах в формате официального веб-сайта. Часто сайт компании требуется, чтобы разместить здесь ваше портфолио, прайс-лист, контакты и форму обратной связи.<br><br>Вы можете добавить любое количество блоков на страницу: «О сотрудниках», «Отзывы», «Фотогалерея» и другие. Для этого используйте встроенный конструктор страниц в ADVANTSHOP.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.CompanySiteWithCatalog.Description', 'A company page is needed if you want to tell about your company, its services and products in the format of an official website. Often, a company website is required to place your portfolio, price list, contacts and feedback form here.<br><br>You can add any number of blocks to the page: "About employees", "Reviews", "Photo gallery" and others. To do this, use the built-in page builder in ADVANTSHOP.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.CompanySiteWithCatalog.Intent', '<ul><li>Познакомить аудиторию с товарами и услугами компании</li><li>Предоставить контактную информацию</li><li>Продать товары и услуги</li></ul>'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.CompanySiteWithCatalog.Intent', '<ul><li>Introduce the audience to the companys products and services</li><li>Provide contact information</li><li>Sell products and services</li></ul>'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.CompanySiteWithCatalog.Advice', 'Используйте как можно больше собственных фотографий и меньше копируйте из Интернета.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.CompanySiteWithCatalog.Advice', 'Use as many of your own photos as possible and copy as little as possible from the Internet.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.CompanySiteWithCatalog.Title.CollectingContacts', 'Сбор контактов'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.CompanySiteWithCatalog.Title.CollectingContacts', 'Collecting contacts'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.CompanySiteWithCatalog.Title.AboutRestaurant', 'О ресторане'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.CompanySiteWithCatalog.Title.AboutRestaurant', 'About the restaurant'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.CompanySiteWithCatalog.Title.MenuDelivery', 'Меню и Доставка'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.CompanySiteWithCatalog.Title.MenuDelivery', 'Menu and Delivery'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.CompanySiteWithCatalog.Title.Banquets', 'Банкеты'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.CompanySiteWithCatalog.Title.Banquets', 'Banquets'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.CompanySiteWithCatalog.Title.PhotoGallery', 'Фотогалерея'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.CompanySiteWithCatalog.Title.PhotoGallery', 'Photo Gallery'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.CompanySiteWithCatalog.Title.Contacts', 'Контакты'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.CompanySiteWithCatalog.Title.Contacts', 'Contacts'
               
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.CompanySiteWithPrices.Name', 'Сайт компании с ценами'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.CompanySiteWithPrices.Name', 'Company website with prices'
                    
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.CompanySiteWithPrices.Description', 'Страница компании нужна, если требуется рассказать о вашей компании, ее услугах и товарах в формате официального веб-сайта. Часто сайт компании требуется, чтобы разместить здесь ваше портфолио, прайс-лист, контакты и форму обратной связи.<br><br>Вы можете добавить любое количество блоков на страницу: «О сотрудниках», «Отзывы», «Фотогалерея» и другие. Для этого используйте встроенный конструктор страниц в ADVANTSHOP.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.CompanySiteWithPrices.Description', 'A company page is needed if you want to tell about your company, its services and products in the format of an official website. Often, a company website is required to place your portfolio, price list, contacts and feedback form here.<br><br>You can add any number of blocks to the page: "About employees", "Reviews", "Photo gallery" and others. To do this, use the built-in page builder in ADVANTSHOP.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.CompanySiteWithPrices.Intent', '<ul><li>Познакомить аудиторию с товарами и услугами компании</li><li>Предоставить контактную информацию</li><li>Продать товары и услуги</li></ul>'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.CompanySiteWithPrices.Intent', '<ul><li>Introduce the audience to the companys products and services</li><li>Provide contact information</li><li>Sell products and services</li></ul></li></ul>'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.CompanySiteWithPrices.Advice', 'Используйте как можно больше собственных фотографий и меньше копируйте из Интернета.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.CompanySiteWithPrices.Advice', 'Use as many of your own photos as possible and copy as little as possible from the Internet.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.CompanySiteWithPrices.Title.CollectingContacts', 'Сбор контактов'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.CompanySiteWithPrices.Title.CollectingContacts', 'Collecting contacts'
               
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.InstagramFunnel.Name', 'Микролендинг "Instagram"'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.InstagramFunnel.Name', 'Micro Landing "Instagram"'
                    
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.InstagramFunnel.Description', 'Ни для кого не секрет, что в Instagram нельзя оставлять ссылки внутри постов и в комментариях. Вы можете разместить только одну активную ссылку в описании вашего профиля. А важных страниц, на которые нужно привести клиентов, у вас несколько: магазин, официальный сайт, страница с акцией, свежая статья о вашей работе, страница с опросом и так далее. Еще нужно дать интерактивную ссылку на мессенджеры. Иначе, чтобы связаться с вами, посетитель вынужден будет перебивать вручную ваш номер телефона и долго искать вас в WhatsApp, Viber или Telegram. Не каждый будет этим заниматься.<br><br>Для решения этой проблемы создаются микролендинги специально для Instagram, где размещается вся необходимая информация и ссылки на разные ресурсы.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.InstagramFunnel.Description', 'Its no secret that you cant leave links inside posts or in comments on Instagram. You can only place one active link in your profile description. And you have several important pages to which you need to bring clients: a store, an official website, a page with a promotion, a fresh article about your work, a page with a survey, and so on. You also need to provide an interactive link to messengers. Otherwise, in order to contact you, the visitor will have to manually enter your phone number and search for you for a long time on WhatsApp, Viber or Telegram. Not everyone will do this.<br><br>To solve this problem, micro landings are created specifically for Instagram, where all the necessary information and links to various resources are posted.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.InstagramFunnel.Intent', 'Разместите в микролендинге кнопку, которая позволит быстро связаться с вами в любом из популярных мессенджеров.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.InstagramFunnel.Intent', 'Place a button on your micro-landing page that will allow people to quickly contact you in any of the popular messengers.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.InstagramFunnel.Advice', 'Используйте как можно больше собственных фотографий и меньше копируйте из Интернета.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.InstagramFunnel.Advice', 'Use as many of your own photos as possible and copy as little as possible from the Internet.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.InstagramFunnel.Title.CollectingContacts', 'Сбор контактов'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.InstagramFunnel.Title.CollectingContacts', 'Collecting contacts'
               
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.CompanySiteExpert.Name', 'Личная страница эксперта'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.CompanySiteExpert.Name', 'Experts personal page'
                    
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.CompanySiteExpert.Description', 'Личная страница создается, чтобы рассказать о себе и своих проектах. Она может стать доказательством вашей экспертности. Ссылку на такую страницу размещают в аккаунтах социальных сетей, в подписи для электронной почты, на визитках.<br><br>Хорошо разместить на личной странице предложение бесплатных консультаций. Для этой работы нужно выделить определенные часы и дни недели. Посетители должны иметь возможность выбрать свободный временной слот и записаться.<br><br>При необходимости вы можете сделать платные консультации, подключив оплату к бронированию временного слота. Все эти возможности уже есть в ADVANTSHOP.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.CompanySiteExpert.Description', 'A personal page is created to tell about yourself and your projects. It can become proof of your expertise. A link to such a page is placed in social network accounts, in an email signature, on business cards.<br><br>It is a good idea to place an offer of free consultations on your personal page. For this work, you need to allocate certain hours and days of the week. Visitors should be able to choose a free time slot and sign up.<br><br>If necessary, you can make paid consultations by connecting payment to the time slot booking. All these options are already available in ADVANTSHOP.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.CompanySiteExpert.Intent', '<ul><li>Доказать вашу экспертность в какой-то области</li><li>Рассказать о себе</li><li>Завести новые связи</li><li>Заработать на консультациях</li></ul>'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.CompanySiteExpert.Intent', '<ul><li>Prove your expertise in a certain field</li><li>Tell about yourself</li><li>Make new connections</li><li>Earn money from consultations</li></ul>'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.CompanySiteExpert.Advice', 'Не забудьте пригласить посетителей подписаться на ваши соцсети, блог или рассылку в мессенджере.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.CompanySiteExpert.Advice', 'Dont forget to invite visitors to subscribe to your social networks, blog or messenger newsletter.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.CompanySiteExpert.Title.CollectingContacts', 'Сбор контактов'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.CompanySiteExpert.Title.CollectingContacts', 'Collecting contacts'
               
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.CompanySiteBePartner.Name', 'Стань партнером'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.CompanySiteBePartner.Name', 'Become a partner'
                    
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.CompanySiteBePartner.Description', 'В ADVANTSHOP вы можете подключить партнерскую программу для реферального маркетинга. Это модель привлечения клиентов, когда ваши действующие клиенты рекомендуют вас за вознаграждение. Механика простая: участник партнерской программы получает уникальную ссылку или промокод, мотивирует знакомых получить ваш товар или услугу по этой ссылке и за каждого приведенного клиента получает вознаграждение. Подробнее о партнерской программе вы можете почитать на сайте <a href="https://www.advantshop.net/features-tour/partnerprogram">advantshop.net</a> в разделе «Возможности».<br><br>Для привлечения партнеров вам понадобится страница, приглашающая к сотрудничеству. Напишите на ней, какие услуги ваши партнеры могут перепродавать и какие нужно выполнить условия для получения вознаграждения.<br><br>На странице размещается кнопка «Стать партнером», ведущая на форму регистрации в партнерской программе.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.CompanySiteBePartner.Description', 'In ADVANTSHOP, you can connect an affiliate program for referral marketing. This is a model for attracting customers when your current customers recommend you for a reward. The mechanics are simple: a participant in the affiliate program receives a unique link or promo code, motivates friends to get your product or service via this link, and receives a reward for each customer brought. You can read more about the affiliate program on the website <a href="https://www.advantshop.net/features-tour/partnerprogram">advantshop.net</a> in the "Opportunities" section.<br><br>To attract partners, you will need a page inviting you to cooperate. Write on it what services your partners can resell and what conditions must be met to receive a reward.<br><br>The page contains a "Become a partner" button leading to the registration form in the affiliate program.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.CompanySiteBePartner.Intent', '<ul><li>Презентация партнерской программы</li><li>Привлечение партнерского трафика</li></ul>'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.CompanySiteBePartner.Intent', '<ul><li>Presentation of the affiliate program</li><li>Attracting affiliate traffic</li></ul>'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.CompanySiteBePartner.Advice', 'Опционально вы можете добавить на страницу видеообращение к партнерам, где расскажете обо всех преимуществах сотрудничества с вами.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.CompanySiteBePartner.Advice', 'Optionally, you can add a video message to your partners on the page, where you will tell them about all the benefits of working with you.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.CompanySiteBePartner.Title.CollectingContacts', 'Сбор контактов'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.CompanySiteBePartner.Title.CollectingContacts', 'Collecting contacts'
               
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.CompanySiteUnderConstruction.Name', 'Сайт в разработке'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.CompanySiteUnderConstruction.Name', 'The site is under development'
                    
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.CompanySiteUnderConstruction.Description', 'Страница-заглушка отлично подходит, чтобы сообщить посетителям, что ваш сайт находится в разработке и скоро появится. Отредактируйте этот шаблон: добавьте контакты компании, ссылки на ваши страницы в соц. сетях, форму обратной связи. Пользователи будут оставлять свои электронные адреса, чтобы первыми узнать об открытии вашего сайта.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.CompanySiteUnderConstruction.Description', 'A placeholder page is great for letting visitors know that your site is under construction and will be available soon. Edit this template: add company contacts, links to your social media pages, a feedback form. Users will leave their email addresses to be the first to know about the opening of your site.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.CompanySiteUnderConstruction.Title.PlaceholderPage', 'Страница-заглушка'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.CompanySiteUnderConstruction.Title.PlaceholderPage', 'Placeholder page'
               
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.CompanySiteWithBooking.Name', 'Сайт компании с записью к специалистам'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.CompanySiteWithBooking.Name', 'Company website with appointments with specialists'
                    
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.CompanySiteWithBooking.Description', 'Готовый одностраничный сайт с позволит посетителям забронировать запись к нужным специалистам, а вам — организовать рабочее время сотрудников.<br><br>Бронирование и отображение доступных слотов происходит в режиме онлайн. Учет и работа с бронированиями находится в специальном разделе администрирования. В шаблоне предусмотрена запись в режиме заявки, а также с обязательным выбором услуг и онлайн-оплаты.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.CompanySiteWithBooking.Description', 'A ready-made one-page website will allow visitors to book an appointment with the necessary specialists, and you will be able to organize the working hours of employees.<br><br>Booking and display of available slots occurs online. Accounting and work with bookings is located in a special administration section. The template provides for recording in the application mode, as well as with the mandatory selection of services and online payment.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.CompanySiteWithBooking.Title.OffersPage', 'Страница с предложениями'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.CompanySiteWithBooking.Title.OffersPage', 'Offers Page'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.CompanySiteWithBooking.Title.Booking', 'Бронирование'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.CompanySiteWithBooking.Title.Booking', 'Booking'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.CreateFunnel.CompanySiteWithBooking.Title.ThankPage', 'Страница благодарности'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.CreateFunnel.CompanySiteWithBooking.Title.ThankPage', 'Thank you page'
GO--
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Landing.Name', 'Название'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Landing.Name', 'Name'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Landing.Main', 'Главная'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Landing.Main', 'Main'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Funnel.TriggerMarketing', 'Канал продаж "Триггерный маркетинг" не подключен'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Funnel.TriggerMarketing', 'The "Trigger Marketing" sales channel is not connected'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Funnel.Connect', 'Подключить'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Funnel.Connect', 'Connect'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Funnel.Cancel', 'Отмена'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Funnel.Cancel', 'Cancel'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Funnel.SureWantToDelete', 'Вы уверены что хотите удалить?'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Funnel.SureWantToDelete', 'Are you sure you want to delete?'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Funnel.SuccessfullySaved', 'Изменения успешно сохранены.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Funnel.SuccessfullySaved', 'The changes were successfully saved.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Funnel.DeleteCancel', 'Отмена'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Funnel.DeleteCancel', 'Cancel'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Funnel.SelectCategoriesForFunnel', 'Выберите категории для воронки'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Funnel.SelectCategoriesForFunnel', 'Select categories for your funnel'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.SureWantDeleteSite', 'Вы уверены что хотите удалить сайт?'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.SureWantDeleteSite', 'Are you sure you want to delete the site?'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Delete', 'Удаление'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Delete', 'Delete'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Success', 'Сайт успешно удален'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Success', 'The site was successfully deleted'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Landing.DeleteSelected', 'Удалить выделенные'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Landing.DeleteSelected', 'Delete selected'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Landing.SaveSiteDomain', 'Домен добавлен. Чтобы он начал откликаться, необходимо привязать его на стороне хостинга.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Landing.SaveSiteDomain', 'Domain added. For it to start responding, you need to bind it on the hosting side.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Landing.SureWantDeleteDomain', 'Вы уверены что хотите удалить домен?'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Landing.SureWantDeleteDomain', 'Are you sure you want to delete the domain?'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Landing.DomainDeletedSuccessfully', 'Домен успешно удален'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Landing.DomainDeletedSuccessfully', 'Domain deleted successfully'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Landing.DomainAddedSuccessfully', 'Домен добавлен. Чтобы он начал откликаться, необходимо привязать его на стороне хостинга.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Landing.DomainAddedSuccessfully', 'Domain added. For it to start responding, you need to bind it on the hosting side.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Landing.GenerateSitemapXml', 'Карта сайта сгенерирована'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Landing.GenerateSitemapXml', 'Sitemap generated'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Landing.GenerateSitemapHtml', 'Карта сайта сгенерирована'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Landing.GenerateSitemapHtml', 'Sitemap generated'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Landing.ArtNo', 'Артикул'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Landing.ArtNo', 'Article'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Landing.UnsetProductsFunnel', 'Отвязать воронку от выделенных'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Landing.UnsetProductsFunnel', 'Untie the funnel from the selected ones'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Landing.SureUnsetProductsFunnel', 'Вы уверены, что хотите отвязать воронку от товаров?'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Landing.SureUnsetProductsFunnel', 'Are you sure you want to unlink your product funnel?'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Landing.SureUnsetProductFunnel', 'Вы уверены, что хотите отвязать воронку от товара?'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Landing.SureUnsetProductFunnel', 'Are you sure you want to unlink the funnel from the product?'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Views.Funnels.Settings.SeoSiteMapsAndRobots', 'Карта сайта'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Views.Funnels.Settings.SeoSiteMapsAndRobots', 'Site map'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Views.Funnels.Settings.UseHttpsInLinks', 'Использовать https в ссылках'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Views.Funnels.Settings.UseHttpsInLinks', 'Use https in links'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Views.Funnels.Settings.Generate', 'Сгенерировать'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Views.Funnels.Settings.Generate', 'Generate'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.LandingSite.FunnelBookings.Affiliate', 'Филиал'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.LandingSite.FunnelBookings.Affiliate', 'Affiliate'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.LandingSite.FunnelBookings.Number', 'Номер'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.LandingSite.FunnelBookings.Number', 'Number'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.LandingSite.FunnelBookings.Status', 'Статус'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.LandingSite.FunnelBookings.Status', 'Status'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.LandingSite.FunnelBookings.Contact', 'Контакт'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.LandingSite.FunnelBookings.Contact', 'Contact'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.LandingSite.FunnelBookings.Resource', 'Ресурс'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.LandingSite.FunnelBookings.Resource', 'Resource'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.LandingSite.FunnelBookings.Sum', 'Сумма'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.LandingSite.FunnelBookings.Sum', 'Sum'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.LandingSite.FunnelBookings.StartDate', 'Дата начала'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.LandingSite.FunnelBookings.StartDate', 'Start date'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.LandingSite.FunnelBookings.EndDate', 'Дата окончания'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.LandingSite.FunnelBookings.EndDate', 'End date'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.LandingSite.FunnelBookings.CreationDate', 'Дата создания'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.LandingSite.FunnelBookings.CreationDate', 'Creation date'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.GoToWebsite', 'Перейти на сайт'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.GoToWebsite', 'Go to website'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.SetAsMain', 'Сделать основным'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.SetAsMain', 'Set as main'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.WebsiteQRCode', 'QR код сайта'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.WebsiteQRCode', 'Website QR code'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.Remove', 'Удалить'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.Remove', 'Remove'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Shared.Dashboard.CreateFunnel.FailedToDeleteSite', 'Не удалось удалить сайт'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Shared.Dashboard.CreateFunnel.FailedToDeleteSite', 'Failed to delete site'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Shared.Dashboard.CreateFunnel.MySites', 'Мои сайты'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Shared.Dashboard.CreateFunnel.MySites', 'My sites'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Shared.Dashboard.CreateFunnel.CreatingNewSite', 'Создание нового сайта'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Shared.Dashboard.CreateFunnel.CreatingNewSite', 'Creating a new site'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Shared.Dashboard.CreateFunnel.FunnelNotFound', 'Воронка не найдена'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Shared.Dashboard.CreateFunnel.FunnelNotFound', 'Funnel not found'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Shared.Dashboard.CreateFunnel.TypeNotSupported', 'Не поддерживается тип'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Shared.Dashboard.CreateFunnel.TypeNotSupported', 'Type not supported'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Shared.Dashboard.CreateFunnel.LinkTextMissing', 'Отсутствует текст ссылки'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Shared.Dashboard.CreateFunnel.LinkTextMissing', 'Link text is missing'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Shared.Dashboard.CreateFunnel.FailedGenerateQRCode', 'Не удалось сгенерировать QR код'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Shared.Dashboard.CreateFunnel.FailedGenerateQRCode', 'Failed to generate QR code'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Landings.CreateCopyLandingSite.FunnelNotFound', 'Воронка не найдена'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Landings.CreateCopyLandingSite.FunnelNotFound', 'Funnel not found'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Landings.CreateCopyLandingSite.Copy', ' - Копия'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Landings.CreateCopyLandingSite.Copy', ' - Copy'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Controllers.Landings.Funnel', 'Воронка'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Controllers.Landings.Funnel', 'Funnel'

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Leads.NameOrVendorCodeOfProduct', 'Название или артикул товара'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Leads.NameOrVendorCodeOfProduct', 'Product name or article number'

GO--

ALTER TABLE Settings.Files ADD
	Charset nvarchar(20) NULL
GO--

Update [Settings].[Files] 
Set [Charset] = 'utf-8' 
Where [Path] = '/robots.txt' or [Path] = '/areas/mobile/robots.txt'

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Order.ArtNo', 'Артикул'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Order.ArtNo', 'ArtNo'

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApiAuth.Index.ApiAuth.Barcodes', 'Штрихкоды'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApiAuth.Index.ApiAuth.Barcodes', 'Barcodes'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApiAuth.Index.ApiAuth.BracodesDescription', 'Поиск товара по штрихкоду'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApiAuth.Index.ApiAuth.BracodesDescription', 'Search product by barcode' 
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApiAuth.Index.ApiAuth.BracodesDescriptionHint', ' Если товар найден по штрихкоду, то в ответе будет объект product (как в GET /api/products/{id}), если товар не найден, то в product будет null'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApiAuth.Index.ApiAuth.BracodesDescriptionHint', 'If the product is found by barcode, then the product object will be in the response (as in GET /api/products/{id}), if the product is not found, then the product will be null' 
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApiAuth.Index.ApiAuth.BracodesExample2', 'Пример ответа, если товар по штрихкоду не найден'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApiAuth.Index.ApiAuth.BracodesExample2', 'Example response, if product by barcode not found' 

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.SettingsApiAuth.BonusCardQrCodeMode.BonusCardNumber', 'Номер бонусной карты'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.SettingsApiAuth.BonusCardQrCodeMode.BonusCardNumber', 'Bonus card number'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.SettingsApiAuth.BonusCardQrCodeMode.Phone', 'Номер телефона'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.SettingsApiAuth.BonusCardQrCodeMode.Phone', 'Phone number' 

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'PrintOrder.ChangeTitle', 'Сумма с которой необходима сдача'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'PrintOrder.ChangeTitle', 'The amount from which the change is required'

GO--

IF NOT EXISTS(
	SELECT 
		1
	FROM 
		sys.columns
	WHERE 
		(name = N'UrlPath') AND object_id = OBJECT_ID(N'[Catalog].[ProductList]'))
BEGIN
		ALTER TABLE 
			[Catalog].[ProductList]
		ADD 
			UrlPath nvarchar(150) NULL;
END

GO--

IF EXISTS(
	SELECT 
		1
	FROM 
		sys.columns
	WHERE 
		(name = N'UrlPath') 
		AND object_id = OBJECT_ID(N'[Catalog].[ProductList]')
		AND is_nullable = 1)
BEGIN
		UPDATE 
			[Catalog].[ProductList]
		SET 
			UrlPath = CONVERT(varchar(150), Id)
		
		ALTER TABLE 
			[Catalog].[ProductList]
		ALTER COLUMN 
			UrlPath nvarchar(150) NOT NULL;
END

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.AddEditProductList.UrlPath', 'Синоним для URL запроса'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.AddEditProductList.UrlPath', 'URL synonym'

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Template.ReviewsSortingOnMainPage', 'Сортировать отзывы на главной'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Template.ReviewsSortingOnMainPage', 'Sort reviews on main page' 
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Settings.SettingsCatalog.ReviewsSortingOnMainPage.ByLikes', 'По популярности отзыва и дате добавления'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Settings.SettingsCatalog.ReviewsSortingOnMainPage.ByLikes', 'By likes and date' 
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Settings.SettingsCatalog.ReviewsSortingOnMainPage.ByDate', 'По дате добавления'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Settings.SettingsCatalog.ReviewsSortingOnMainPage.ByDate', 'By date' 
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Settings.SettingsCatalog.ReviewsSortingOnMainPage.ByReviewRatingThenDate', 'По рейтингу (оценке) и дате добавления'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Settings.SettingsCatalog.ReviewsSortingOnMainPage.ByReviewRatingThenDate', 'By reviews rating and date' 
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Reviews.ShowOnMain', 'Отображать на главной'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Reviews.ShowOnMain', 'Show on main page' 
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Reviews.LikesDislikes', 'За / Против'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Reviews.LikesDislikes', 'Likes / Dislikes' 
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Reviews.Rating', 'Рейтинг (оценка)'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Reviews.Rating', 'Rating' 

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Template.ReviewsSortingOnMainPageHint', '<p>Данная настройка отвечает за то, как отсортированы отзывы на главной:</p>
<p>- по дате добавления (сначала новые),</p>
<p>- по рейтингу и дате добавления (сначала отзывы с высокой оценкой, из них на первом месте более новые),</p>
<p>- по популярности отзыва (сначала отзывы с самым большим количеством голосов "За", из них на первом месте более новые).</p>'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Template.ReviewsSortingOnMainPageHint', '<p>This setting is responsible for how the reviews on the main page are sorted:</p>
<p>- by date of addition (new first),</p>
<p>- by rating and date of addition (first reviews with a high rating, of which newer ones are in the first place),</p>
<p>- by popularity of the review (first reviews with the largest number of votes "For", of newer ones are in the first place).</p>' 

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.Builder.ReviewsSortingOnMainPage', 'Сортировать отзывы на главной'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.Builder.ReviewsSortingOnMainPage', 'Sort reviews on main page' 
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.Builder.ReviewsSortingOnMainPageHint', '<p>Данная настройка отвечает за то, как отсортированы отзывы на главной:</p>
<p>- по дате добавления (сначала новые),</p>
<p>- по рейтингу и дате добавления (сначала отзывы с высокой оценкой, из них на первом месте более новые),</p>
<p>- по популярности отзыва (сначала отзывы с самым большим количеством голосов "За", из них на первом месте более новые).</p>'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.Builder.ReviewsSortingOnMainPageHint', '<p>This setting is responsible for how the reviews on the main page are sorted:</p>
<p>- by date of addition (new first),</p>
<p>- by rating and date of addition (first reviews with a high rating, of which newer ones are in the first place),</p>
<p>- by popularity of the review (first reviews with the largest number of votes "For", of newer ones are in the first place).</p>' 

GO--

IF NOT EXISTS(SELECT 1 FROM sys.columns 
          WHERE Name = N'IdempotenceKey'
          AND Object_ID = Object_ID(N'Bonus.Transaction'))
BEGIN
	ALTER TABLE [Bonus].[Transaction] ADD
		[IdempotenceKey] nvarchar(50) NULL    
END

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Index.LogoImgSizeRecommendations', 'Рекомендуемый размер 200 x 50 px<br> Формат может быть только *.gif, *.png, *.jpg или *.svg'

GO--

IF NOT EXISTS (SELECT * FROM [Settings].[Settings] WHERE [Name] = 'MainPageIsLogoImageSvg')
BEGIN
	IF (NOT EXISTS(SELECT 1 FROM [Settings].[Settings] WHERE [Name] = 'MainPageIsLogoImageSvg') OR (SELECT [Value] FROM [Settings].[Settings] WHERE [Name] = 'MainPageLogoFileNameSvg') = '')
	BEGIN
		INSERT INTO [Settings].[Settings] ([Name], [Value])
		VALUES ('MainPageIsLogoImageSvg', 'False');
		
		DELETE [Settings].[Settings]
		WHERE [Name] = 'MainPageLogoFileNameSvg';
	END
	ELSE
	BEGIN
		UPDATE [Settings].[Settings] 
		SET Value = (SELECT TOP(1) [Value] FROM [Settings].[Settings] WHERE [Name] = 'MainPageLogoFileNameSvg')
		WHERE [Name] = 'MainPageLogoFileName';
		
		INSERT INTO [Settings].[Settings] ([Name], [Value])
		VALUES ('MainPageIsLogoImageSvg', 'True');
		
		DELETE [Settings].[Settings]
		WHERE [Name] = 'MainPageLogoFileNameSvg';
	END
END

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Index.FaviconRecommendations', 'Рекомендуемые размеры: 16 x 16, 32 x 32, 96 x 96, 120 x 120, 144 x 144 px<br> Favicon может быть только форматов *.gif, *.png, *.ico или *.svg'

GO--

IF NOT EXISTS (SELECT * FROM [Settings].[Settings] WHERE [Name] = 'MainIsFaviconImageSvg')
BEGIN
	IF (NOT EXISTS(SELECT 1 FROM [Settings].[Settings] WHERE [Name] = 'FaviconSvg') OR (SELECT [Value] FROM [Settings].[Settings] WHERE [Name] = 'FaviconSvg') = '')
	BEGIN
		INSERT INTO [Settings].[Settings] ([Name], [Value])
		VALUES ('MainIsFaviconImageSvg', 'False');
		
		DELETE [Settings].[Settings]
		WHERE [Name] = 'FaviconSvg';
	END
	ELSE
	BEGIN
		UPDATE [Settings].[Settings] 
		SET Value = (SELECT TOP(1) [Value] FROM [Settings].[Settings] WHERE [Name] = 'FaviconSvg')
		WHERE [Name] = 'MainFaviconFileName';
		
		INSERT INTO [Settings].[Settings] ([Name], [Value])
		VALUES ('MainIsFaviconImageSvg', 'True');
		
		DELETE [Settings].[Settings]
		WHERE [Name] = 'FaviconSvg';
	END
END

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Catalog.ShowSpinboxInCatalog', 'Oтображать спинбокс в каталоге'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Catalog.ShowSpinboxInCatalog', 'Display spinbox in catalog'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.Builder.ShowSpinboxInCatalog', 'Oтображать спинбокс в каталоге'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.Builder.ShowSpinboxInCatalog', 'Display spinbox in catalog'

GO--

IF NOT EXISTS(SELECT 1
              FROM sys.columns
              WHERE (name = N'MaxPrice') AND object_id = OBJECT_ID(N'[Catalog].[ProductExt]'))
BEGIN
    ALTER TABLE Catalog.ProductExt ADD
        MaxPrice float NULL
END
GO--
ALTER PROCEDURE [Catalog].[PreCalcProductParams]
    @productId INT,
    @ModerateReviews BIT,
    @OnlyAvailable BIT,
    @ComplexFilter BIT
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @CountPhoto INT;
    DECLARE @Type NVARCHAR(10);
    DECLARE @PhotoId INT;
    DECLARE @MaxAvailable FLOAT;
    DECLARE @VideosAvailable BIT;
    DECLARE @Colors NVARCHAR(MAX);
    DECLARE @NotSamePrices BIT;
    DECLARE @MinPrice FLOAT;
	DECLARE @MaxPrice FLOAT;
    DECLARE @PriceTemp FLOAT;
    DECLARE @AmountSort BIT;
    DECLARE @OfferId INT;
    DECLARE @Comments INT;
    DECLARE @CategoryId INT;
    DECLARE @Gifts BIT;
	DECLARE @AllowAddToCartInCatalog BIT;
    IF NOT EXISTS
        (
            SELECT ProductID
            FROM [Catalog].Product
            WHERE ProductID = @productId
        )
        RETURN;
    SET @Type = 'Product';
    --@CountPhoto        
    SET @CountPhoto =
            (
                SELECT TOP (1) CASE
                                   WHEN
                                           (
                                               SELECT Offer.ColorID
                                               FROM [Catalog].[Offer]
                                               WHERE [ProductID] = @productId
                                                 AND main = 1
                                           ) IS NOT NULL AND @ComplexFilter = 1
                                       THEN
                                       (
                                           SELECT COUNT(DISTINCT PhotoId)
                                           FROM [Catalog].[Photo]
                                                    INNER JOIN [Catalog].[Offer] ON [Photo].ColorID = Offer.ColorID OR [Photo].ColorID is NULL
                                           WHERE [Photo].[ObjId] = Offer.[ProductId]
                                             AND [Offer].Main = 1
                                             AND TYPE = @Type
                                             AND Offer.[ProductId] = @productId
                                       )
                                   ELSE
                                       (
                                           SELECT COUNT(PhotoId)
                                           FROM [Catalog].[Photo]
                                           WHERE [Photo].[ObjId] = @productId
                                             AND TYPE = @Type
                                       )
                                   END
            );
    --@PhotoId        
    SET @PhotoId =
            (
                SELECT CASE
                           WHEN
                               (
                                   SELECT Offer.ColorID
                                   FROM [Catalog].[Offer]
                                   WHERE [ProductID] = @productId
                                     AND main = 1
                               ) IS NOT NULL
                               THEN
                               (
                                   SELECT TOP (1) PhotoId
                                   FROM [Catalog].[Photo]
                                            INNER JOIN [Catalog].[Offer] ON Offer.[ProductId] = @productId AND ([Photo].ColorID = Offer.ColorID OR [Photo].ColorID is NULL)
                                   WHERE([Photo].ColorID = Offer.ColorID
                                       OR [Photo].ColorID IS NULL)
                                     AND [Photo].[ObjId] = @productId
                                     AND Type = @Type
                                   ORDER BY [Photo]. main DESC,
                                            [Photo].[PhotoSortOrder],
                                            [PhotoId]
                               )
                           ELSE
                               (
                                   SELECT TOP (1) PhotoId
                                   FROM [Catalog].[Photo]
                                   WHERE [Photo].[ObjId] = @productId
                                     AND Type = @Type
                                   ORDER BY main DESC,
                                            [Photo].[PhotoSortOrder],
                                            [PhotoId]
                               )
                           END
            );
    --VideosAvailable        
    IF (SELECT COUNT(ProductVideoID) FROM [Catalog].[ProductVideo] WHERE ProductID = @productId) > 0
        BEGIN
            SET @VideosAvailable = 1;
        END;
    ELSE
        BEGIN
            SET @VideosAvailable = 0;
        END;
    --@MaxAvailable        
    SET @MaxAvailable = (SELECT MAX(Offer.Amount) FROM [Catalog].Offer WHERE ProductId = @productId);
    --AmountSort        
    SET @AmountSort =
            (
                SELECT CASE
                           WHEN @MaxAvailable <= 0
                               OR @MaxAvailable < ISNULL(Product.MinAmount, 0)
                               THEN 0
                           ELSE 1
                           END
                FROM [Catalog].Offer
                         INNER JOIN [Catalog].Product ON Product.ProductId = Offer.ProductId
                WHERE Offer.ProductId = @productId
                  AND main = 1
            );
    --Colors        
    SET @Colors =
            (
                SELECT [Settings].[ProductColorsToString](@productId, @OnlyAvailable)
            );
    --@NotSamePrices        
    IF
            (
                SELECT MAX(price) - MIN(price)
                FROM [Catalog].offer
                         INNER JOIN [Catalog].Product ON Product.ProductId = Offer.ProductId
                WHERE offer.productid = @productId AND
                        price > 0 AND
                    (@OnlyAvailable = 0 OR amount > 0 OR AllowPreOrder = 1)
            ) > 0
        BEGIN
            SET @NotSamePrices = 1;
        END;
    ELSE
        BEGIN
            SET @NotSamePrices = 0;
        END;
    --@MinPrice        
    SET @MinPrice =
            (
                SELECT isNull(MIN(price), 0)
                FROM [Catalog].offer
                         INNER JOIN [Catalog].Product ON Product.ProductId = Offer.ProductId
                WHERE offer.productid = @productId AND
                        price > 0 AND
                    (@OnlyAvailable = 0 OR amount > 0 OR AllowPreOrder = 1)
            );
	--@MaxPrice        
    SET @MaxPrice =
            (
                SELECT isNull(MAX(price), 0)
                FROM [Catalog].offer
                         INNER JOIN [Catalog].Product ON Product.ProductId = Offer.ProductId
                WHERE offer.productid = @productId AND
                        price > 0 AND
                    (@OnlyAvailable = 0 OR amount > 0 OR AllowPreOrder = 1)
            );
    --@OfferId      
    SET @OfferId =
            (
                SELECT OfferID
                FROM [Catalog].offer
                WHERE offer.productid = @productId AND (offer.Main = 1 OR offer.Main IS NULL)
            );
    --@PriceTemp        
    SET @PriceTemp =
            (
                SELECT CASE WHEN [Product].Discount > 0 THEN (@MinPrice - @MinPrice * [Product].Discount / 100) * CurrencyValue ELSE (@MinPrice - isnull([Product].DiscountAmount, 0)) * CurrencyValue END
                FROM Catalog.Product
                         INNER JOIN Catalog.Currency ON Product.Currencyid = Currency.Currencyid
                WHERE Product.Productid = @productId
            );
    --@Comments      
    SET @Comments =
            (
                SELECT COUNT(ReviewId)
                FROM CMS.Review
                WHERE EntityId = @productId AND (Checked = 1 OR @ModerateReviews = 0)
            );
    --@Gifts      
    SET @Gifts =
            (
                SELECT TOP (1) CASE
                                   WHEN COUNT([ProductGifts].ProductID) > 0
                                       THEN 1
                                   ELSE 0
                                   END
                FROM [Catalog].[ProductGifts]
                         INNER JOIN Catalog.Offer on ProductGifts.GiftOfferId = Offer.OfferId
                         INNER JOIN Catalog.Product on Offer.ProductId = Product.ProductId
                WHERE [ProductGifts].ProductID = @productId  and Offer.Amount > ISNULL(Product.MinAmount, 0) and Enabled = 1
            );
	SET @AllowAddToCartInCatalog = 
			(
				CASE WHEN (
							(SELECT COUNT(OfferId) FROM Catalog.Offer WHERE Offer.ProductId = @productId) = 1 
                                and not Exists (SELECT 1 FROM [Catalog].[CustomOptions] WHERE ProductID = @productId)
							) THEN 1 
					 ELSE 0
					 END
			);
    IF
            (
                SELECT COUNT(productid)
                FROM [Catalog].ProductExt
                WHERE productid = @productId
            ) > 0
        BEGIN
            UPDATE [Catalog].[ProductExt]
            SET
                [CountPhoto] = @CountPhoto,
                [PhotoId] = @PhotoId,
                [VideosAvailable] = @VideosAvailable,
                [MaxAvailable] = @MaxAvailable,
                [NotSamePrices] = @NotSamePrices,
                [MinPrice] = @MinPrice,
                [Colors] = @Colors,
                [AmountSort] = @AmountSort,
                [OfferId] = @OfferId,
                [Comments] = @Comments,
                [PriceTemp] = @PriceTemp,
                [Gifts] = @Gifts,
				[AllowAddToCartInCatalog] = @AllowAddToCartInCatalog,
				[MaxPrice] = @MaxPrice
            WHERE [ProductId] = @productId;
        END;
    ELSE
        BEGIN
            INSERT INTO [Catalog].[ProductExt]
            ([ProductId],
             [CountPhoto],
             [PhotoId],
             [VideosAvailable],
             [MaxAvailable],
             [NotSamePrices],
             [MinPrice],
             [Colors],
             [AmountSort],
             [OfferId],
             [Comments],
             [PriceTemp],
             [Gifts],
			 [AllowAddToCartInCatalog],
			 [MaxPrice]
            )
            VALUES
                (@productId,
                 @CountPhoto,
                 @PhotoId,
                 @VideosAvailable,
                 @MaxAvailable,
                 @NotSamePrices,
                 @MinPrice,
                 @Colors,
                 @AmountSort,
                 @OfferId,
                 @Comments,
                 @PriceTemp,
                 @Gifts,
				 @AllowAddToCartInCatalog,
				 @MaxPrice
            );
        END;
END;

GO--

ALTER PROCEDURE [Catalog].[PreCalcProductParamsMass] @ModerateReviews BIT, @OnlyAvailable bit,  @ComplexFilter BIT AS
BEGIN
    INSERT INTO [Catalog].[ProductExt] (ProductId, CountPhoto, PhotoId, VideosAvailable, MaxAvailable, NotSamePrices, MinPrice, Colors, AmountSort, OfferId, Comments, MaxPrice) 
	(
        SELECT ProductId, 0, NULL, 0, 0, 0, 0, NULL, 0, NULL, 0, 0
        FROM [Catalog].Product
        WHERE Product.ProductId NOT IN (SELECT ProductId FROM [Catalog].[ProductExt])
    )
    UPDATE
        catalog.ProductExt
    SET
        ProductExt.[CountPhoto] = tempTable.CountPhoto,
        ProductExt.[PhotoId] = tempTable.[PhotoId],
        ProductExt.[VideosAvailable] = tempTable.[VideosAvailable],
        ProductExt.[MaxAvailable] = tempTable.[MaxAvailable],
        ProductExt.[NotSamePrices] = tempTable.[NotSamePrices],
        ProductExt.[MinPrice] = tempTable.[MinPrice],
		ProductExt.[MaxPrice] = tempTable.[MaxPrice],
        ProductExt.[OfferId] = tempTable.[OfferId],
        ProductExt.[Comments] = tempTable.[Comments],
        ProductExt.[Gifts] = tempTable.[Gifts],
        ProductExt.[Colors] = tempTable.[Colors],
        --ProductExt.[CategoryId] = tempTable.[CategoryId] ,
        ProductExt.PriceTemp = tempTable.PriceTemp,
        ProductExt.AmountSort=tempTable.AmountSort,
		ProductExt.AllowAddToCartInCatalog = tempTable.AllowAddToCartInCatalog
    FROM
        catalog.ProductExt
            INNER JOIN
        (
            select
                pe.ProductId,
                CountPhoto=case when offerId.ColorID is null OR @ComplexFilter = 0 then countNocolor.countNocolor else countColor.countColor end,
                PhotoId=case when offerId.ColorID is null then PhotoIdNoColor.PhotoIdNoColor else PhotoIdColor.PhotoIdColor end,
                [VideosAvailable]=isnull(videosAvailable.videosAvailable,0),
                [MaxAvailable]=maxAvailable.maxAvailable,
                [NotSamePrices]=isnull(notSamePrices.notSamePrices,0),
                [MinPrice]=isnull(minMaxPrice.minPrice,0),
				[MaxPrice]=isnull(minMaxPrice.maxPrice,0),
                [OfferId]=offerId.OfferId,
                [Comments]=isnull(comments.comments,0),
                [Gifts]=isnull(gifts.gifts,0),
                [Colors]=(SELECT [Settings].[ProductColorsToString](pe.ProductId, @OnlyAvailable)),
                --[CategoryId] = (SELECT TOP 1 id	FROM [Settings].[GetParentsCategoryByChild](( SELECT TOP 1 CategoryID FROM [Catalog].ProductCategories	WHERE ProductID = pe.ProductId ORDER BY Main DESC))ORDER BY sort DESC),
                PriceTemp = CASE WHEN p.Discount > 0 THEN (isnull(minMaxPrice.minPrice,0) - isnull(minMaxPrice.minPrice,0) * p.Discount/100)*c.CurrencyValue ELSE (isnull(minMaxPrice.minPrice,0) - isnull(p.DiscountAmount,0))*c.CurrencyValue END,
                AmountSort=CASE when ISNULL(maxAvailable.maxAvailable, 0) <= 0 OR maxAvailable.maxAvailable < IsNull(p.MinAmount, 0) THEN 0 ELSE 1 end,
				AllowAddToCartInCatalog = (CASE WHEN ((SELECT COUNT(OfferId) FROM Catalog.Offer WHERE Offer.ProductId = pe.ProductId) = 1 
													   and not Exists (SELECT 1 FROM [Catalog].[CustomOptions] WHERE ProductID = pe.ProductId)
													 ) THEN 1 
												ELSE 0 END)
            from Catalog.[ProductExt] pe
                     left join (
									SELECT o.ProductId, COUNT(*) countColor FROM [Catalog].[Photo] ph INNER JOIN [Catalog].[Offer] o  ON ph.[ObjId] = o.ProductId
									WHERE ( ph.ColorID = o.ColorID OR ph.ColorID IS NULL ) AND TYPE = 'Product' AND o.Main = 1
									group by o.ProductId
								) countColor on pe.ProductId=countColor.ProductId
                     left join (
									SELECT [ObjId], COUNT(*) countNocolor FROM [Catalog].[Photo]
									WHERE TYPE = 'Product'
									group by [ObjId]
								) countNocolor on pe.ProductId=countNocolor.[ObjId]
                     left join (
									select ProductId, PhotoId PhotoIdColor 
									from (
										   SELECT o.ProductId, ph.PhotoId, Row_Number() over (PARTITION  by o.ProductId ORDER BY ph.main DESC ,ph.[PhotoSortOrder], ph.[PhotoId]) rn 
										   FROM [Catalog].[Photo] ph 
										   INNER JOIN [Catalog].[Offer] o ON ph.[ObjId] = o.ProductId
										   WHERE (ph.ColorID = o.ColorID OR ph.ColorID IS NULL) AND TYPE = 'Product' 
										 ) ct 
									where rn=1
								) PhotoIdColor on pe.ProductId=PhotoIdColor.ProductId
                     left join (
									select ProductId, PhotoId PhotoIdNoColor 
									from (
											SELECT ph.[ObjId] ProductId, ph.PhotoId, Row_Number() over (PARTITION  by ph.[ObjId] ORDER BY ph.main DESC ,ph.[PhotoSortOrder], ph.[PhotoId]) rn 
											FROM [Catalog].[Photo] ph	
											WHERE TYPE = 'Product' 
										 ) ct 
									where rn=1
								) PhotoIdNoColor on pe.ProductId=PhotoIdNoColor.ProductId
                     left join (
									select pv.ProductID, CASE WHEN COUNT(pv.ProductVideoID) > 0 THEN 1	ELSE 0 END videosAvailable 
									FROM [Catalog].[ProductVideo] pv 
									group by pv.ProductID
								) videosAvailable on pe.ProductId=videosAvailable.ProductId
                     left join (
									select o.ProductID,Max(o.Amount) maxAvailable  FROM [Catalog].Offer o group by o.ProductID
								) maxAvailable on pe.ProductId=maxAvailable.ProductId
                     left join (
									select o.ProductID, CASE WHEN MAX(o.price) - MIN(o.price) > 0 THEN 1 ELSE 0 END notSamePrices  
									FROM [Catalog].Offer o 
									Inner Join [Catalog].Product On Product.ProductId = o.ProductID 
									where o.price > 0 AND (@OnlyAvailable = 0 OR o.amount > 0 OR AllowPreOrder = 1) 
									group by o.ProductID
								) notSamePrices on pe.ProductId=notSamePrices.ProductId
                     left join (
									select o.ProductID, MIN(o.Price) minPrice, MAX(o.Price) maxPrice
									FROM [Catalog].Offer o 
									Inner Join [Catalog].Product On Product.ProductId = o.ProductID 
									where o.price > 0 AND (@OnlyAvailable = 0 OR o.amount > 0 OR AllowPreOrder = 1)  
									group by o.ProductID
								) minMaxPrice on pe.ProductId=minMaxPrice.ProductId
                     
					 left join (
									select ProductId, OfferID, colorId 
									from (
											select o.ProductID,o.OfferID, o.colorId, Row_Number() over (PARTITION  by o.OfferID ORDER BY o.OfferID) rn  
											FROM [Catalog].Offer o where o.Main = 1 
										 )ct 
									where rn=1
								) offerId on pe.ProductId=offerId.ProductId
                     left join (
									select EntityId ProductID,count(ReviewId) comments  
									FROM CMS.Review  
									where (Checked = 1 OR @ModerateReviews = 0) 
									group by EntityId
								) comments on pe.ProductId=comments.ProductId
                     left join (
									select pg.ProductID, CASE WHEN COUNT(pg.ProductID) > 0 THEN 1 ELSE 0 END gifts 
									FROM [Catalog].[ProductGifts] pg 
									INNER JOIN Catalog.Offer on pg.GiftOfferId = Offer.OfferId 
									INNER JOIN Catalog.Product on Offer.ProductId = Product.ProductId 
									WHERE Offer.Amount > ISNULL(Product.MinAmount, 0) and Enabled = 1 
									group by pg.ProductID
								) gifts on pe.ProductId=gifts.ProductId
                     inner join Catalog.Product p on p.ProductID = pe.ProductID
                     inner join Catalog.Currency c ON p.CurrencyId = c.CurrencyId
        )
            AS tempTable
        ON tempTable.ProductId = ProductExt.ProductId
END

GO--

if Exists (Select 1 From Catalog.ProductExt Where MaxPrice is null) 
begin
	Declare @ShowOnlyAvailable bit;
	Set @ShowOnlyAvailable = Case When isNull((Select Value From Settings.Settings Where Name = 'ShowOnlyAvalible'), 'False') = 'True' Then 1 Else 0 End;
    Update Catalog.ProductExt 
    Set MaxPrice =  (
                        SELECT isNull(MAX(price), 0)
                        FROM [Catalog].offer
                        INNER JOIN [Catalog].Product ON Product.ProductId = pe.ProductId
                        WHERE offer.productid = pe.ProductId 
                                AND price > 0 
                                AND (@ShowOnlyAvailable = 0 OR amount > 0 OR AllowPreOrder = 1)
                    )
	From Catalog.ProductExt pe
end

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Catalog.Warehouses', 'Наличие на складе'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Catalog.Warehouses', 'Stock availability'

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Handlers.Design.UnsupportedFileType', 'Допустимые расширения файлов: .zip'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Handlers.Design.UnsupportedFileType', 'Supported types: .zip'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Handlers.Design.ThemeAlreadyExists', 'Дизайн {0} уже существует. Название загружаемого файла, папки в архиве и название темы должны быть уникальными'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Handlers.Design.ThemeAlreadyExists', 'The design {0} already exists. The name of the uploaded file, the folder in the archive and the name of the theme must be unique'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Handlers.Design.UnzipError', 'Ошибка при загрузке. Не удалось разархивировать файл.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Handlers.Design.UnzipError', 'Error during upload. Failed to unzip the file.'

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.EditMainPageList.Sorting', 'Порядок сортировки'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.EditMainPageList.Sorting', 'Sort order'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Catalog.MainPageProducts.Update.Error', 'Не удалось обновить список'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Catalog.MainPageProducts.Update.Error', 'Failed to update the list'

GO--

IF NOT EXISTS (SELECT * FROM [Settings].[Settings] WHERE [Name] = 'BestSorting')
BEGIN
	INSERT INTO [Settings].[Settings] ([Name], [Value])
	VALUES ('BestSorting', '100000')
END
IF NOT EXISTS (SELECT * FROM [Settings].[Settings] WHERE [Name] = 'NewSorting')
BEGIN
	INSERT INTO [Settings].[Settings] ([Name], [Value])
	VALUES ('NewSorting', '99999')
END
IF NOT EXISTS (SELECT * FROM [Settings].[Settings] WHERE [Name] = 'SalesSorting')
BEGIN
	INSERT INTO [Settings].[Settings] ([Name], [Value])
	VALUES ('SalesSorting', '99998')
END

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Content.ModalAddLandingSite.Funnel', 'Воронка'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Content.ModalAddLandingSite.Funnel', 'Funnel'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Content.ModalAddLandingSite.YourURL', 'Свой URL'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Content.ModalAddLandingSite.YourURL', 'Your URL'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Content.ModalAddLandingSite.OnlineStoreCategory', 'Категория Интернет-магазина'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Content.ModalAddLandingSite.OnlineStoreCategory', 'Online store category'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.GridCustomComponent.OK', 'OK'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.GridCustomComponent.OK', 'OK'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.GridCustomComponent.Cancel', 'Отмена'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.GridCustomComponent.Cancel', 'Cancel'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.TriggerEdit.SureWantToDelete', 'Вы уверены, что хотите удалить?'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.TriggerEdit.SureWantToDelete', 'Are you sure you want to delete?'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.TriggerEdit.Removal', 'Удаление'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.TriggerEdit.Removal', 'Removal'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.TriggerEdit.OK', 'OK'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.TriggerEdit.OK', 'OK'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.TriggerEdit.Cancel', 'Отмена'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.TriggerEdit.Cancel', 'Cancel'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.ChangesSaved', 'Изменения успешно сохранены.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.ChangesSaved', 'Changes saved successfully.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Handlers.Landings.GetLpSitemapInfo.FileMissing', 'Файл отсутсвует'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Handlers.Landings.GetLpSitemapInfo.FileMissing', 'File missing'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Funnels.Settings.Goods', 'Товары:'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Funnels.Settings.Goods', 'Goods:'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Landing.LpSettings.Goods', 'Товары:'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Landing.LpSettings.Goods', 'Goods:'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Content.EditFunnelName.EditTitle', 'Редактировать название'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Content.EditFunnelName.EditTitle', 'Edit title'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Content.EditFunnelName.Name', 'Название'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Content.EditFunnelName.Name', 'Name'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Content.LandingSite.Remove', 'Удалить'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Content.LandingSite.Remove', 'Delete'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.NotificationsTemplates.Index.NotificationMethod', 'Метод уведомления'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.NotificationsTemplates.Index.NotificationMethod', 'Notification method'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.NotificationsTemplates.Title', 'Тип шаблона'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.NotificationsTemplates.Title', 'Template type'

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Services.Shipping.Measoft.SelectedPointNotAvailable', 'Пункт выдачи "{0}" недоступен'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Services.Shipping.Measoft.SelectedPointNotAvailable', 'The pick-up point "{0}" is not available'

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.PaymentMethods.Robokassa.Protocol', 'Способ подключения'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.PaymentMethods.Robokassa.Protocol', 'Connection method' 
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.PaymentMethods.Robokassa.ProtocolHint', 'Протокол взаимодействия<br />Платежный модуль - HTTP<br />Виджет - Оплата при помощи iFrame'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.PaymentMethods.Robokassa.ProtocolHint', 'Interaction protocol<br />Payment module - HTTP<br />Widget - Payment using an iFrame' 

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.ShippingMethods.ElectronicDelivery.PriceOfDelivery', 'Цена доставки'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.ShippingMethods.ElectronicDelivery.PriceOfDelivery', 'Price of delivery'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.ShippingMethods.ElectronicDelivery.DeliveryCostInBaseCurrency', 'Стомость доставки. Цена указывается в базовой валюте магазина, только числом.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.ShippingMethods.ElectronicDelivery.DeliveryCostInBaseCurrency', 'Delivery cost. The price is indicated in the base currency of the store, only the number.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.ShippingMethods.ElectronicDelivery.ForExample100', 'Например: 100 (руб.)'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.ShippingMethods.ElectronicDelivery.ForExample100', 'For example: 100 (rub.)'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.ShippingMethods.ElectronicDelivery.IfSelfDeliveryIfFree', 'Если бесплатно, укажите стоимость 0.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.ShippingMethods.ElectronicDelivery.IfSelfDeliveryIfFree', 'If is free, indicate the cost of 0.'

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.ExportImport.OrderFields.NotSelected', 'Не выбрано'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.ExportImport.OrderFields.NotSelected', 'Not selected'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.ExportImport.OrderFields.Number', 'Номер заказа'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.ExportImport.OrderFields.Number', 'Order number'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.ExportImport.OrderFields.Status', 'Статус заказа'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.ExportImport.OrderFields.Status', 'Order status'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.ExportImport.OrderFields.OrderSource', 'Источник заказа'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.ExportImport.OrderFields.OrderSource', 'Order source'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.ExportImport.OrderFields.OrderDateTime', 'Дата заказа'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.ExportImport.OrderFields.OrderDateTime', 'Order date'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.ExportImport.OrderFields.FullName', 'ФИО покупателя'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.ExportImport.OrderFields.FullName', 'Customer fullname'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.ExportImport.OrderFields.Email', 'Email покупателя'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.ExportImport.OrderFields.Email', 'Customer Email'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.ExportImport.OrderFields.Phone', 'Телефон покупателя'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.ExportImport.OrderFields.Phone', 'Customer phone'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.ExportImport.OrderFields.CustomerGroup', 'Группа покупателя'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.ExportImport.OrderFields.CustomerGroup', 'Customer group'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.ExportImport.OrderFields.RecipientFullName', 'ФИО получателя'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.ExportImport.OrderFields.RecipientFullName', 'Recipient fullname'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.ExportImport.OrderFields.RecipientPhone', 'Телефон получателя'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.ExportImport.OrderFields.RecipientPhone', 'Recipient phone'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.ExportImport.OrderFields.Weight', 'Общий вес'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.ExportImport.OrderFields.Weight', 'Weight'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.ExportImport.OrderFields.Dimensions', 'Общие габариты'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.ExportImport.OrderFields.Dimensions', 'Dimensions'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.ExportImport.OrderFields.Payed', 'Оплачен'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.ExportImport.OrderFields.Payed', 'Payed'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.ExportImport.OrderFields.Discount', 'Скидка'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.ExportImport.OrderFields.Discount', 'Discount'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.ExportImport.OrderFields.ShippingCost', 'Стоимость доставки'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.ExportImport.OrderFields.ShippingCost', 'Shipping cost'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.ExportImport.OrderFields.PaymentCost', 'Наценка оплаты'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.ExportImport.OrderFields.PaymentCost', 'Payment cost'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.ExportImport.OrderFields.Coupon', 'Купон'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.ExportImport.OrderFields.Coupon', 'Coupon'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.ExportImport.OrderFields.BonusCost', 'Оплачено бонусами'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.ExportImport.OrderFields.BonusCost', 'Bonus cost'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.ExportImport.OrderFields.Sum', 'Итоговая стоимость'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.ExportImport.OrderFields.Sum', 'Sum'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.ExportImport.OrderFields.Currency', 'Валюта'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.ExportImport.OrderFields.Currency', 'Currency'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.ExportImport.OrderFields.ShippingName', 'Метод доставки'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.ExportImport.OrderFields.ShippingName', 'Shipping method'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.ExportImport.OrderFields.PaymentName', 'Метод оплаты'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.ExportImport.OrderFields.PaymentName', 'Payment method'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.ExportImport.OrderFields.Country', 'Страна'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.ExportImport.OrderFields.Country', 'Country'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.ExportImport.OrderFields.Region', 'Регион'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.ExportImport.OrderFields.Region', 'Region'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.ExportImport.OrderFields.District', 'Район'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.ExportImport.OrderFields.District', 'District'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.ExportImport.OrderFields.City', 'Город'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.ExportImport.OrderFields.City', 'City'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.ExportImport.OrderFields.Street', 'Улица'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.ExportImport.OrderFields.Street', 'Street'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.ExportImport.OrderFields.House', 'Дом'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.ExportImport.OrderFields.House', 'House'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.ExportImport.OrderFields.Structure', 'Строение'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.ExportImport.OrderFields.Structure', 'Structure'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.ExportImport.OrderFields.Apartment', 'Квартира'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.ExportImport.OrderFields.Apartment', 'Apartment'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.ExportImport.OrderFields.Entrance', 'Подъезд'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.ExportImport.OrderFields.Entrance', 'Entrance'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.ExportImport.OrderFields.Floor', 'Этаж'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.ExportImport.OrderFields.Floor', 'Floor'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.ExportImport.OrderFields.DeliveryDate', 'Дата доставки'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.ExportImport.OrderFields.DeliveryDate', 'Delivery date'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.ExportImport.OrderFields.DeliveryTime', 'Время доставки'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.ExportImport.OrderFields.DeliveryTime', 'Delivery time'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.ExportImport.OrderFields.CustomerComment', 'Комментарий покупателя'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.ExportImport.OrderFields.CustomerComment', 'Customer сomment'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.ExportImport.OrderFields.AdminComment', 'Комментарий администратора'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.ExportImport.OrderFields.AdminComment', 'Admin comment'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.ExportImport.OrderFields.StatusComment', 'Комментарий к статусу'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.ExportImport.OrderFields.StatusComment', 'Status comment'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.ExportImport.OrderFields.Manager', 'Менеджер'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.ExportImport.OrderFields.Manager', 'Manager'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.ExportImport.OrderFields.Street', 'Улица'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.ExportImport.OrderFields.Street', 'Street'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.ExportImport.OrderFields.CouponCode', 'Код купона'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.ExportImport.OrderFields.CouponCode', 'Coupon code'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.ExportImport.OrderFields.OrderItemArtNo', 'Артикул товара'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.ExportImport.OrderFields.OrderItemArtNo', 'Product sku'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.ExportImport.OrderFields.OrderItemName', 'Название товара'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.ExportImport.OrderFields.OrderItemName', 'Product name'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.ExportImport.OrderFields.OrderItemCustomOptions', 'Дополнительные опции'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.ExportImport.OrderFields.OrderItemCustomOptions', 'Custom options'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.ExportImport.OrderFields.OrderItemSize', 'Размер товара'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.ExportImport.OrderFields.OrderItemSize', 'Product size'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.ExportImport.OrderFields.OrderItemColor', 'Цвет товара'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.ExportImport.OrderFields.OrderItemColor', 'Product color'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.ExportImport.OrderFields.OrderItemPrice', 'Цена товара'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.ExportImport.OrderFields.OrderItemPrice', 'Product price'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.ExportImport.OrderFields.OrderItemAmount', 'Количество товара'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.ExportImport.OrderFields.OrderItemAmount', 'Product amount'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.ExportImport.OrderFields.GoogleClientId', 'Google client id'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.ExportImport.OrderFields.GoogleClientId', 'Google client id'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.ExportImport.OrderFields.YandexClientId', 'Yandex client id'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.ExportImport.OrderFields.YandexClientId', 'Yandex client id'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.ExportImport.OrderFields.Referral', 'Реферал'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.ExportImport.OrderFields.Referral', 'Referral'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.ExportImport.OrderFields.LoginPage', 'Страница входа'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.ExportImport.OrderFields.LoginPage', 'Login page'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.ExportImport.OrderFields.UtmSource', 'UTM source'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.ExportImport.OrderFields.UtmSource', 'UTM source'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.ExportImport.OrderFields.UtmMedium', 'UTM medium'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.ExportImport.OrderFields.UtmMedium', 'UTM medium'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.ExportImport.OrderFields.UtmCampaign', 'UTM campaign'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.ExportImport.OrderFields.UtmCampaign', 'UTM campaign'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.ExportImport.OrderFields.UtmContent', 'UTM content'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.ExportImport.OrderFields.UtmContent', 'UTM content'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.ExportImport.OrderFields.UtmTerm', 'UTM term'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.ExportImport.OrderFields.UtmTerm', 'UTM term'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.ExportImport.MultiOrder.DeliveryDate', 'Дата доставки'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.ExportImport.MultiOrder.DeliveryDate', 'Delivery date'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.ExportImport.MultiOrder.DeliveryTime', 'Время доставки'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.ExportImport.MultiOrder.DeliveryTime', 'Delivery time'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.ExportOrders.CustomOptionOptionsSeparator', 'Разделитель опций в дополнительных опциях'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.ExportOrders.CustomOptionOptionsSeparator', 'Option separator in custom options'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsCheckout.ImportOrders.Title', 'Импорт заказов'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsCheckout.ImportOrders.Title', 'Import orders'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsCheckout.ImportOrders.Import', 'Импорт'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsCheckout.ImportOrders.Import', 'Import'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsCheckout.ImportOrders.Params', 'Параметры загрузки'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsCheckout.ImportOrders.Params', 'Params'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsCheckout.ImportOrders.ColumnSeparator', 'Разделитель между колонками'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsCheckout.ImportOrders.ColumnSeparator', 'Column separator'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsCheckout.ImportOrders.ColumnSeparatorHint', 'Разделитель, который указан между столбцами или колонками в файле CSV'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsCheckout.ImportOrders.ColumnSeparatorHint', 'The separator that is specified between columns or columns in the CSV file'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsCheckout.ImportOrders.FileEncoding', 'Кодировка файла'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsCheckout.ImportOrders.FileEncoding', 'Encoding of the file'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsCheckout.ImportOrders.FileEncodingHint', 'Это кодировка, в которой загружается каталог. Обычные кодировки, которые воспринимаются Microsoft Excel, это кодировки UTF-8 и Windows-1251'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsCheckout.ImportOrders.FileEncodingHint', 'This is the encoding in which the catalog is loaded. The usual encodings that are perceived by Microsoft Excel are UTF-8 and Windows-1251 encodings'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Import.ImportOrders.CustomOptionOptionsSeparator', 'Разделитель между значениями дополнительных опций'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Import.ImportOrders.CustomOptionOptionsSeparator', 'Separator between the values of additional options'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Import.ImportOrders.CustomOptionOptionsSeparatorHint', 'Разделитель между значениями дополнительных опций'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Import.ImportOrders.CustomOptionOptionsSeparatorHint', 'Separator between the values of additional options'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsCheckout.ImportOrders.SampleFile', 'Пример файла'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsCheckout.ImportOrders.SampleFile', 'Sample file'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsCheckout.ImportOrders.UpdateCustomer', 'Обновлять информацию о покупателе'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsCheckout.ImportOrders.UpdateCustomer', 'Update сustomer'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsCheckout.ImportOrders.UpdateCustomerHint', 'Если покупатель уже есть в магазине, информация по нему обновится из файла.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsCheckout.ImportOrders.UpdateCustomerHint', 'If the customer is already in the store, the information about him will be updated from the file.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsCheckout.ImportOrders.HasHeader', 'Первая строка файла содержит заголовки'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsCheckout.ImportOrders.HasHeader', 'The first line of the file contains the headers'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsCheckout.ImportOrders.CsvFile', '.Csv файл заказов'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsCheckout.ImportOrders.CsvFile', '.Csv file of orders'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsCheckout.ImportOrders.ColumnInCsv', 'Колонка в .csv файле'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsCheckout.ImportOrders.ColumnInCsv', 'Column in csv'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsCheckout.ImportOrders.FirstOrderData', 'Данные первого заказа'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsCheckout.ImportOrders.FirstOrderData', 'First order data'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsCheckout.ImportOrders.DataType', 'Тип данных'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsCheckout.ImportOrders.DataType', 'Data type'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.ExportImport.MultiOrder.Country', 'Страна'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.ExportImport.MultiOrder.Country', 'Country'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.ExportImport.MultiOrder.Region', 'Регион'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.ExportImport.MultiOrder.Region', 'Region'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.ExportImport.MultiOrder.City', 'Город'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.ExportImport.MultiOrder.City', 'City'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.ExportImport.MultiOrder.Street', 'Улица'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.ExportImport.MultiOrder.Street', 'Street'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.ExportImport.MultiOrder.House', 'Дом'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.ExportImport.MultiOrder.House', 'House'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.ExportImport.MultiOrder.Structure', 'Строение'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.ExportImport.MultiOrder.Structure', 'Structure'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.ExportImport.MultiOrder.Apartment', 'Квартира'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.ExportImport.MultiOrder.Apartment', 'Apartment'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.ExportImport.MultiOrder.Entrance', 'Подъезд'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.ExportImport.MultiOrder.Entrance', 'Entrance'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.ExportImport.MultiOrder.Floor', 'Этаж'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.ExportImport.MultiOrder.Floor', 'Floor'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsCheckout.ImportOrders.ChangeNewFile', 'Выбрать другой файл'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsCheckout.ImportOrders.ChangeNewFile', 'Change new file'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsCheckout.ImportOrders.HasHeaderHint', 'Данная опция означает, что в импортируемом файле CSV будет верхняя строка с заголовками'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsCheckout.ImportOrders.HasHeaderHint', 'This option means that the imported CSV file will have the top row with the headers'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.ImportOrders.ProcessName', 'Импорт заказов'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.ImportOrders.ProcessName', 'Importing orders'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Orders.OrderType.OrderImport', 'Импорт заказов'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Orders.OrderType.OrderImport', 'Order import'

GO--

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[CMS].[StaticPageCities]') AND type in (N'U'))
BEGIN
CREATE TABLE CMS.StaticPageCities (
                                      CityID INT NOT NULL,
                                      StaticPageID INT NOT NULL,
                                      FOREIGN KEY (StaticPageID) REFERENCES CMS.StaticPage(StaticPageID) ON DELETE CASCADE,
                                      FOREIGN KEY (CityID) REFERENCES Customers.City(CityID) ON DELETE CASCADE,
                                      PRIMARY KEY (StaticPageID, CityID)
);
END;

GO--

ALTER TABLE [Settings].[DomainGeoLocation]
ADD GeoName NVARCHAR(100) NOT NULL DEFAULT '';

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.MetaVariables.StaticPage.GeoName', 'Название локации'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.MetaVariables.StaticPage.GeoName', 'Geo Name' 

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.SeoSettings.BonusProgram', 'Бонусная программа (мета по умолчанию)'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.SeoSettings.BonusProgram', 'Bonus program (default meta)'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.SeoSettings.BonusProgramDefaultTitle', 'Title страницы'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.SeoSettings.BonusProgramDefaultTitle', 'Page title'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.SeoSettings.BonusProgramDefaultH1', 'Заголовок H1'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.SeoSettings.BonusProgramDefaultH1', 'H1 header'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.SeoSettings.BonusProgramDefaultMetaKeywords', 'Ключевые слова'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.SeoSettings.BonusProgramDefaultMetaKeywords', 'Meta keywords'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.SeoSettings.BonusProgramDefaultMetaDescription', 'Мета описание'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.SeoSettings.BonusProgramDefaultMetaDescription', 'Meta description'

GO--

INSERT INTO [Settings].[Settings] (Name, Value)
VALUES
    ('BonusProgramH1', 'Бонусная программа'),
    ('BonusProgramTitle', 'Бонусная программа - #STORE_NAME#'),
    ('BonusProgramMetaKeywords', 'Бонусная программа - #STORE_NAME#'),
    ('BonusProgramMetaDescription', 'Бонусная программа - #STORE_NAME#')

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApi.Index.CreateOrder.OrderFields.Number', 'Номер заказа. Если указан и в системе нет заказа с таким номером, то присвоется номер.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApi.Index.CreateOrder.OrderFields.Number', 'Order number'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsApi.Index.CreateOrder.OrderFields.OrderPrefix', 'Префикс для номера заказа. Если есть префикс и передается номер заказа, то заказ добавится с номером "{OrderPrefix}+{Number}". Если номер не передается или заказ с таким номером уже существует, то номер заказа будет "{OrderPrefix}+{Новый OrderId}"'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsApi.Index.CreateOrder.OrderFields.OrderPrefix', 'Order prefix'

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Services.Shipping.Boxberry.IndexNotSpecified', 'Не указан Индекс'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Services.Shipping.Boxberry.IndexNotSpecified', 'The Index is not specified'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Services.Shipping.Boxberry.InvalidIndex', 'Доставка по указанному индексу недоступна'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Services.Shipping.Boxberry.InvalidIndex', 'Delivery to the specified index is not available'

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.ImportOrders.Errors.FieldsRequired', 'Должно быть выбрано поле номер заказа'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.ImportOrders.Errors.FieldsRequired', 'The order number field must be selected'

GO--

IF NOT EXISTS (
    SELECT 1 
    FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE TABLE_SCHEMA = 'Bonus' 
    AND TABLE_NAME = 'NotificationTemplate' 
    AND COLUMN_NAME = 'NotificationMethod'
)
BEGIN
ALTER TABLE [Bonus].[NotificationTemplate]
    ADD NotificationMethod INT NULL;
END

GO--

UPDATE [Bonus].[NotificationTemplate]
SET NotificationMethod = 0
WHERE NotificationMethod IS NULL;

GO--

ALTER TABLE [Bonus].[NotificationTemplate]
ALTER COLUMN NotificationMethod INT NOT NULL;

GO--

IF NOT EXISTS (
    SELECT 1
    FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_SCHEMA = 'Bonus'
    AND TABLE_NAME = 'NotificationTemplate'
    AND COLUMN_NAME = 'NotificationTemplateId'
)
BEGIN
ALTER TABLE [Bonus].[NotificationTemplate]
    ADD NotificationTemplateId INT NOT NULL IDENTITY(1,1);
END
      
GO--

IF EXISTS (
    SELECT 1 
    FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS 
    WHERE CONSTRAINT_NAME = 'PK_SmsTemplate' 
    AND TABLE_NAME = 'NotificationTemplate' 
    AND TABLE_SCHEMA = 'Bonus'
)
BEGIN
ALTER TABLE [Bonus].[NotificationTemplate]
DROP CONSTRAINT [PK_SmsTemplate];
     
ALTER TABLE [Bonus].[NotificationTemplate]
ADD CONSTRAINT PK_NotificationTemplate PRIMARY KEY (NotificationTemplateId);
END

GO--

IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = 'UQ_NotificationTemplate'
      AND object_id = OBJECT_ID('[Bonus].[NotificationTemplate]')
)
BEGIN
CREATE UNIQUE INDEX UQ_NotificationTemplate
    ON [Bonus].[NotificationTemplate](NotificationMethod, NotificationTypeId);
END

GO--

IF NOT EXISTS(SELECT * FROM [Bonus].[NotificationTemplate] WHERE NotificationMethod = 1 OR NotificationMethod = 2)
BEGIN
INSERT INTO [Bonus].[NotificationTemplate] (NotificationTypeId, NotificationBody, NotificationMethod)
SELECT NotificationTypeId, NotificationBody, 1
FROM [Bonus].[NotificationTemplate] nt1
WHERE NOT EXISTS (
    SELECT 1
    FROM [Bonus].[NotificationTemplate] nt2
    WHERE nt2.NotificationMethod = 1
  AND nt2.NotificationTypeId = nt1.NotificationTypeId
    )
UNION ALL
SELECT NotificationTypeId, NotificationBody, 2
FROM [Bonus].[NotificationTemplate] nt1
WHERE NOT EXISTS (
    SELECT 1
    FROM [Bonus].[NotificationTemplate] nt2
    WHERE nt2.NotificationMethod = 2
  AND nt2.NotificationTypeId = nt1.NotificationTypeId
    );
END

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.SmsTemplate.Method', 'Метод уведомления'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.SmsTemplate.Method', 'Notification method'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.SmsTemplate.SelectMethod', 'Выберите метод уведомления'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.SmsTemplate.SelectMethod', 'Select notification method'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.ExportImport.OrderFields.Zip', 'Индекс'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.ExportImport.OrderFields.Zip', 'Zip'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.ExportImport.MultiOrder.Zip', 'Индекс'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.ExportImport.MultiOrder.Zip', 'Zip'

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Checkout.ShowCountDevices', 'Выводить поле "Количество приборов"'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Checkout.ShowCountDevices', 'Display the "Number of devices" field'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Checkout.ShowCountDevices.Help', 'В оформлении заказа будет выведено поле для ввода количества приборов'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Checkout.ShowCountDevices.Help', 'A field for entering the number of devices will be displayed at the checkout'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.OrderItemsSummary.CountDevices', 'Количество приборов'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.OrderItemsSummary.CountDevices', 'Number of devices'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Checkout.CheckoutComment.CountDevices', 'Количество приборов'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Checkout.CheckoutComment.CountDevices', 'Number of devices'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Checkout.CheckoutComment.CountDevicesPlaceholder', 'Укажите количество приборов'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Checkout.CheckoutComment.CountDevicesPlaceholder', 'Specify the number of devices'
IF NOT EXISTS(SELECT 1
              FROM sys.columns
              WHERE (name = N'CountDevices') AND object_id = OBJECT_ID(N'[Order].[Order]'))
    BEGIN
        ALTER TABLE [Order].[Order]
			ADD CountDevices INT NULL
	END
	
GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.NotificationsTemplates.Index.TemplateType', 'Тип шаблона'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.NotificationsTemplates.Index.TemplateType', 'Template type'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.NotificationsTemplates.Index.Message', 'Сообщение'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.NotificationsTemplates.Index.Message', 'Message'

GO--

IF NOT EXISTS(SELECT 1
              FROM sys.columns
              WHERE (name = N'GoogleAvailabilityDate') AND object_id = OBJECT_ID(N'[Catalog].[ProductExportOptions]'))
BEGIN
    ALTER TABLE Catalog.ProductExportOptions ADD
        GoogleAvailabilityDate datetime NULL
END

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.ExportOptions.GoogleAvailabilityDate', 'Дата поступления (availability_date) Google Merchant Center'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.ExportOptions.GoogleAvailabilityDate', 'Availability date (availability_date) Google Merchant Center'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.ExportOptions.GoogleAvailabilityDateHint', 'Если товар "предзаказ" [preorder] или "под заказ" [backorder], тогда с помощью атрибута "дата поступления" нужно указать дату отправки товара (она должна быть в пределах года). В остальных случаях необязательный атрибут.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.ExportOptions.GoogleAvailabilityDateHint', 'If the product is "preorder" or "backorder", then use the "availability date" attribute to specify the date of shipment of the product (it must be within a year). In other cases, an optional attribute.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.ExportOptions.ReadMore', 'Подробнее'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.ExportOptions.ReadMore', 'Read more'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.ExportImport.EProductField.GoogleAvailabilityDate', 'Google Merchant: Дата поступления'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.ExportImport.EProductField.GoogleAvailabilityDate', 'Google Merchant: Availability date'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.ExportImport.ProductFields.GoogleAvailabilityDate', 'Дата поступления Google Merchant Center'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.ExportImport.ProductFields.GoogleAvailabilityDate', 'Availability date Google Merchant Center'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Catalog.Product.GoogleAvailabilityDate', 'Дата поступления (availability_date) Google Merchant Center'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Catalog.Product.GoogleAvailabilityDate', 'Availability date (availability_date) Google Merchant Center'

GO--

IF (EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'Customers' 
                 AND  TABLE_NAME = 'CallAuthCodeConfirmation'))
BEGIN
    exec sp_rename '[Customers].[CallAuthCodeConfirmation]', 'CallCodeConfirmation';
END

GO--

IF (EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'Customers' 
                 AND  TABLE_NAME = 'CallAuthBan'))
BEGIN
    exec sp_rename '[Customers].[CallAuthBan]', 'CallBan';
END

GO--

IF (EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'Customers' 
                 AND  TABLE_NAME = 'CallAuthLog'))
BEGIN
    exec sp_rename '[Customers].[CallAuthLog]', 'CallLog';
END

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Cart.Index.ShoppingCart.Share', 'Поделиться'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Cart.Index.ShoppingCart.Share', 'Share'

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.ShippingMethods.SelfDelivery.Address', 'Адрес'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.ShippingMethods.SelfDelivery.Address', 'Address'

GO--  
  
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.AddEditDomainGeoLocation.GeoName', 'Название локации'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.AddEditDomainGeoLocation.GeoName', 'Geo Name'

GO--  

DELETE FROM [Settings].[Localization] WHERE ResourceKey = 'Js.Phone.PhoneMask'
DELETE FROM [Settings].[Localization] WHERE ResourceKey = 'Js.Bonus.PhoneMask'

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.SettingsSms.SmsBanLevel.Normal', 'Нормальный'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.SettingsSms.SmsBanLevel.Normal', 'Normal'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.SettingsSms.SmsBanLevel.Middle', 'Средний'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.SettingsSms.SmsBanLevel.Middle', 'Middle'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.SettingsSms.SmsBanLevel.High', 'Высокий'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.SettingsSms.SmsBanLevel.High', 'High'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.NotifyEMails.SMSNotifications.SmsBanLevel', 'Режим защиты от подозрительной активности'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.NotifyEMails.SMSNotifications.SmsBanLevel', 'Sms ban mode'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.NotifyEMails.SMSNotifications.SmsBanLevelHint', 'Настройка нужна для защиты от слива бюджета при смс-авторизации. В зависимости от выбранного режима при подозрительной активности будут блокироваться номера телефонов, ip-адреса, с которых идут запросы. <b>Не менять без необходимости.</b>'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.NotifyEMails.SMSNotifications.SmsBanLevelHint', 'The setting is necessary to protect against draining the budget during SMS authorization. Depending on the selected mode, phone numbers and IP addresses from which requests are sent will be blocked in case of suspicious activity. <b>Do not change unless necessary.</b>'

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Handlers.Design.NotAllowedExtensionInZip', 'Не удалось разархивировать из-за файла "{0}". Список разрешенных расширений: {1}.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Handlers.Design.NotAllowedExtensionInZip', 'Error to unzip because of the file "{0}". The list of allowed extensions is {1}.'

GO--
                                      
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.Cart.Index.ShoppingCart.Share', 'Поделиться'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.Cart.Index.ShoppingCart.Share', 'Share'

GO--

IF NOT EXISTS (SELECT * FROM Catalog.Tax WHERE TaxType = 6)
begin
    insert into Catalog.Tax (Name, Enabled, ShowInPrice, Rate, TaxType)
    values (N'НДС 5%',1,1,5, 6),
           (N'НДС 7%',1,1,7, 7)
end

GO--

UPDATE [Settings].[InternalSettings] SET [settingValue] = '13.0.3' WHERE [settingKey] = 'db_version'
