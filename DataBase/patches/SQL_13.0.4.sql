EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Services.Shipping.Yandex.IncompleteAddress', 'Укажите полный адрес доставки'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Services.Shipping.Yandex.IncompleteAddress', 'Enter the full delivery address'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Services.Shipping.Yandex.IntervalsNotAvailable', 'Указанный адрес не распознан или доставка по этому адресу недоступна'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Services.Shipping.Yandex.IntervalsNotAvailable', 'The specified address is not recognized or delivery to this address is unavailable'

GO--


SET IDENTITY_INSERT [Order].[ShippingReplaceGeo] ON 

IF NOT EXISTS (SELECT * FROM [Order].[ShippingReplaceGeo] WHERE [Id] = 52)
INSERT INTO [Order].[ShippingReplaceGeo] ([Id],[ShippingType],[InCountryName],[InCountryISO2],[InRegionName],[InCityName],[InDistrict],[OutCountryName],[OutRegionName],[OutCityName],[OutDistrict],[OutDistrictClear],[Enabled],[Sort],[InZip],[OutZip],[Comment])
VALUES (52,'FivePost','','RU','Республика Марий Эл','','','','Марий-Эл республика','','',0,1,0,'','','')

IF NOT EXISTS (SELECT * FROM [Order].[ShippingReplaceGeo] WHERE [Id] = 53)
INSERT INTO [Order].[ShippingReplaceGeo] ([Id],[ShippingType],[InCountryName],[InCountryISO2],[InRegionName],[InCityName],[InDistrict],[OutCountryName],[OutRegionName],[OutCityName],[OutDistrict],[OutDistrictClear],[Enabled],[Sort],[InZip],[OutZip],[Comment])
VALUES (53,'FivePost','','RU','Удмуртская Республика','','','','Удмуртия республика','','',0,1,0,'','','')


SET IDENTITY_INSERT [Order].[ShippingReplaceGeo] OFF

GO--

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'CMS.StaticPageCities') AND type in (N'U'))
BEGIN
    drop table CMS.StaticPageCities;
END

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Checkout.ClearShoppingCartBeforeBuyByLink', 'Очищать корзину перед добавлением товара по ссылке'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Checkout.ClearShoppingCartBeforeBuyByLink', 'Empty the shopping cart before adding an item via a link'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Checkout.ClearShoppingCartBeforeBuyByLink.Help', 'При добавлении товара в корзину по ссылке корзина будет очищена'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Checkout.ClearShoppingCartBeforeBuyByLink.Help', 'When adding an item to the cart via the link, the cart will be emptied.'

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Checkout.HideShippingNotAvailableForWarehouse', 'Скрывать доставку недоступную для склада'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Checkout.HideShippingNotAvailableForWarehouse', 'Hide a delivery that is unavailable to the warehouse' 

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Checkout.HideShippingNotAvailableForWarehouse.Help', 'Если позиция в заказе недоступна на заданных в доставке складах, то доставка будет скрыта.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Checkout.HideShippingNotAvailableForWarehouse.Help', 'If the item in the order is not available at the warehouses specified in the delivery, the delivery will be hidden.' 

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Shippings.ShippingOption.Error.HaveNotAvailableItems', 'Не все позиции доступны для этого метода'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Shippings.ShippingOption.Error.HaveNotAvailableItems', 'Not all positions are available for this method'

GO--

if exists (Select 1 From [Customers].[CustomerRoleAction] Where [RoleActionKey] = 'Settings')
begin
	Insert Into [Customers].[CustomerRoleAction] ([CustomerID], [RoleActionKey], [Enabled]) 
	Select distinct cra.CustomerID, 'CouponsAndDiscounts' as RoleActionKey, 1 as Enabled 
	From [Customers].[CustomerRoleAction] cra 
	Where cra.[RoleActionKey] = 'Settings' 
		  and not exists (Select 1 
						  From [Customers].[CustomerRoleAction] cra2 
						  Where cra2.CustomerID = cra.CustomerID and cra2.[RoleActionKey] = 'CouponsAndDiscounts')
end

GO--

IF NOT EXISTS(SELECT 1 
			  FROM [Settings].[Settings] 
			  WHERE Name = 'TEMP_fix_product_list_sort')
BEGIN
    UPDATE [Settings].[Settings]
    SET [Value] = CAST(CAST([Value] AS INT) * -1 AS NVARCHAR(MAX))
    WHERE [Name] = 'NewSorting'
       OR [Name] = 'BestSorting'
       OR [Name] = 'SalesSorting'
    
    UPDATE [Catalog].[ProductList]
    SET [SortOrder] = SortOrder * -1
    
    INSERT INTO [Settings].[Settings] (Name, Value)
    VALUES ('TEMP_fix_product_list_sort', '')
END

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.CongratulationsDashboardv2.TrialPeriod', 'У вас подключен пробный период. Выберите тарифный план с помесячной или годовой оплатой со скидкой 25%.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.CongratulationsDashboardv2.TrialPeriod', 'You have a trial period activated. Choose a monthly or annual plan for a 25% discount.'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.CongratulationsDashboardv2.SelectTariff', 'Выбрать тариф'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.CongratulationsDashboardv2.SelectTariff', 'Select tariff'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.CongratulationsDashboardv2.MyAchievements', 'Мои достижения'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.CongratulationsDashboardv2.MyAchievements', 'My achievements'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.CongratulationsDashboardv2.NumberOfCoins', 'Количество монет:'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.CongratulationsDashboardv2.NumberOfCoins', 'Number of coins:'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.CongratulationsDashboardv2.SpendCoins', 'Потратить монеты'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.CongratulationsDashboardv2.SpendCoins', 'Spend coins'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.CongratulationsDashboardv2.ReceiveAdvantcoins', 'Выполните задания ниже, чтобы получить адванткоины, которые можно будет обменять на полезные подарки, бонусы или купоны'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.CongratulationsDashboardv2.ReceiveAdvantcoins', 'Complete the tasks below to receive advantcoins, which can be exchanged for useful gifts, bonuses or coupons'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.CongratulationsDashboardv2.CompletedTasks', 'Выполнено заданий'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.CongratulationsDashboardv2.CompletedTasks', 'Completed tasks'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.CongratulationsDashboardv2.CompletedTasksFrom', 'из'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.CongratulationsDashboardv2.CompletedTasksFrom', 'from'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.CongratulationsDashboardv2.CompleteTheTask', 'Выполнить задание'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.CongratulationsDashboardv2.CompleteTheTask', 'Complete the task'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.CongratulationsDashboardv2.InstructionsForTheTask', 'Инструкция к заданию'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.CongratulationsDashboardv2.InstructionsForTheTask', 'Instructions for the task'
 
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.CongratulationsDashboardv2.Reward', 'Награда:'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.CongratulationsDashboardv2.Reward', 'Reward:'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.CongratulationsDashboardv2.RewardCoins', 'монет'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.CongratulationsDashboardv2.RewardCoins', 'coins'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.CongratulationsDashboardv2.Done', 'Выполнено'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.CongratulationsDashboardv2.Done', 'Done'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.CongratulationsDashboardv2.Earned', 'Заработано:'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.CongratulationsDashboardv2.Earned', 'Earned:'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.CongratulationsDashboardv2.EarnedCoins', 'монет'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.CongratulationsDashboardv2.EarnedCoins', 'coins:'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.CongratulationsDashboardv2.Subscribe', 'Подпишитесь на наши соцсети'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.CongratulationsDashboardv2.Subscribe', 'Subscribe to our social networks'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.CongratulationsDashboardv2.LatestNews', 'Свежие новости, статьи, прямые эфиры, советы по онлайн-продажам — это и многое другое вы найдете в наших социальных сетях.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.CongratulationsDashboardv2.LatestNews', 'Latest news, articles, live broadcasts, tips on online sales - you will find this and much more on our social networks.'

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Common.TopPanel.Achievements', 'Достижения'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Common.TopPanel.Achievements', 'Achievements'

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.CongratulationsDashboardv2.LinkForInstuction', 'По ссылке доступна инструкция для выполнения задания'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.CongratulationsDashboardv2.LinkForInstuction', 'Instructions for completing the task are available at this link'

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Home.CongratulationsDashboard.MyAchievements', 'Мои достижения'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Home.CongratulationsDashboard.MyAchievements', 'My achievements'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Home.CongratulationsDashboard.MyAchievementsCompleteTasks', 'Выполните простые задания по инструкциям, чтобы освоить работу с панелью администратора интернет-магазина.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Home.CongratulationsDashboard.MyAchievementsCompleteTasks', 'Complete simple tasks according to the instructions to master working with the online store admin panel.'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Home.CongratulationsDashboard.GoToAchievements', 'Начать'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Home.CongratulationsDashboard.GoToAchievements', 'Begin'

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Tour.Prev', 'Пред.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Tour.Prev', 'Prev'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Tour.Next', 'След.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Tour.Next', 'Next'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Tour.Finish', 'Готово'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Tour.Finish', 'Finish'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Tour.Settings.Common.Logo', 'Тут можно загрузить готовое изображение'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Tour.Settings.Common.Logo', 'You can upload the logo image here'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Tour.Settings.Common.LogoCreate', 'А тут создать свое прямо в панели администрирования'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Tour.Settings.Common.LogoCreate', 'And here you can create your own directly in the administration panel'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Tour.Settings.Common.LogoInstruction', 'Следуйте инструкциям и всё получится'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Tour.Settings.Common.LogoInstruction', 'Follow the instructions and everything will work out'

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Home.CongratulationsDashboardv2.DidntFindTheAnswer', 'Не нашли ответ на вопрос?'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Home.CongratulationsDashboardv2.DidntFindTheAnswer', 'Didnt find the answer to your question?'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Home.CongratulationsDashboardv2.WriteToUsFirstLine', 'Напишите нам в чат, мы поможем решить любой вопрос.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Home.CongratulationsDashboardv2.WriteToUsFirstLine', 'Write to us in chat, we will help resolve any issue.'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Home.CongratulationsDashboardv2.WriteToUsSecondLine', 'Посетите наш центр поддержки, в нашей “Базе знаний” уже есть ответы на большинство вопросов.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Home.CongratulationsDashboardv2.WriteToUsSecondLine', 'Visit our support center, our “Knowledge Base” already has answers to most questions.'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Home.CongratulationsDashboardv2.GoToSupportCenter', 'Перейти в центр поддержки'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Home.CongratulationsDashboardv2.GoToSupportCenter', 'Go to support center'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Home.CongratulationsDashboardv2.CreateProgect', 'Создайте проект'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Home.CongratulationsDashboardv2.CreateProgect', 'Create a project'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Home.CongratulationsDashboardv2.ProgectCreated', 'Поздравляем, Вы создали интернет-магазин!'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Home.CongratulationsDashboardv2.ProgectCreated', 'Congratulations, you have created an online store!'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Home.CongratulationsDashboardv2.FirstSteps', 'Первые шаги'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Home.CongratulationsDashboardv2.FirstSteps', 'First steps'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Home.CongratulationsDashboardv2.SimpleQuests', 'Выполните простые задания по инструкциям, чтобы освоить работу с панелью администратора интернет-магазина.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Home.CongratulationsDashboardv2.SimpleQuests', 'Complete simple tasks according to the instructions to master working with the online store admin panel.'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Home.CongratulationsDashboardv2.HelpLink', 'Перейти в раздел с инструкциями и ответами на вопросы'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Home.CongratulationsDashboardv2.HelpLink', 'Go to the section with instructions and answers to questions'

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1,'Admin.ExportFeeds.IndexReseller.H1','Реселлеры'
EXEC [Settings].[sp_AddUpdateLocalization] 2,'Admin.ExportFeeds.IndexReseller.H1','Reseller'
EXEC [Settings].[sp_AddUpdateLocalization] 1,'Admin.ExportFeeds.Index.subtitle','Выгружайте Ваши товары в формате CSV'
EXEC [Settings].[sp_AddUpdateLocalization] 2,'Admin.ExportFeeds.Index.subtitle','Upload your offers in CSV file'
EXEC [Settings].[sp_AddUpdateLocalization] 1,'Admin.ExportFeeds.IndexReseller.Instruction', 'Инструкция. Канал продаж "Реселлеры"'
EXEC [Settings].[sp_AddUpdateLocalization] 2,'Admin.ExportFeeds.IndexReseller.Instruction', 'Instruction. Sales channel "Resellers"'

EXEC [Settings].[sp_AddUpdateLocalization] 1,'Admin.ExportFeeds.IndexGoogle.H1','Google Merchant Center'
EXEC [Settings].[sp_AddUpdateLocalization] 2,'Admin.ExportFeeds.IndexGoogle.H1','Google Merchant Center'
EXEC [Settings].[sp_AddUpdateLocalization] 1,'Admin.ExportFeeds.IndexGoogle.Instruction', 'Инструкция. Выгрузка товаров на Google Merchant Center'
EXEC [Settings].[sp_AddUpdateLocalization] 2,'Admin.ExportFeeds.IndexGoogle.Instruction', 'Instruction. Uploading products to Google Merchant Center'
EXEC [Settings].[sp_AddUpdateLocalization] 1,'Admin.ExportFeeds.IndexGoogle.subtitle','Канал продаж Google Merchant Center'
EXEC [Settings].[sp_AddUpdateLocalization] 2,'Admin.ExportFeeds.IndexGoogle.subtitle','Google Merchant Center sales channel'

EXEC [Settings].[sp_AddUpdateLocalization] 1,'Admin.ExportFeeds.IndexAvito.H1','Avito'
EXEC [Settings].[sp_AddUpdateLocalization] 2,'Admin.ExportFeeds.IndexAvito.H1','Avito'
EXEC [Settings].[sp_AddUpdateLocalization] 1,'Admin.ExportFeeds.IndexAvito.Instruction', 'Инструкция. Настройка выгрузки "Avito Автозагрузка"'
EXEC [Settings].[sp_AddUpdateLocalization] 2,'Admin.ExportFeeds.IndexAvito.Instruction', 'Instruction. Configuring "Avito Upload Auto Upload"'
EXEC [Settings].[sp_AddUpdateLocalization] 1,'Admin.ExportFeeds.IndexAvito.subtitle','Развивайте свой бизнес на Авито'
EXEC [Settings].[sp_AddUpdateLocalization] 2,'Admin.ExportFeeds.IndexAvito.subtitle','Expand your business on Avito'

EXEC [Settings].[sp_AddUpdateLocalization] 1,'Admin.ExportFeeds.IndexFacebook.H1', 'Facebook'
EXEC [Settings].[sp_AddUpdateLocalization] 2,'Admin.ExportFeeds.IndexFacebook.H1', 'Facebook'
EXEC [Settings].[sp_AddUpdateLocalization] 1,'Admin.ExportFeeds.IndexFacebook.Instruction', 'Инструкция. Выгрузка товаров в Facebook'
EXEC [Settings].[sp_AddUpdateLocalization] 2,'Admin.ExportFeeds.IndexFacebook.Instruction', 'Instruction. Uploading products to Facebook'
EXEC [Settings].[sp_AddUpdateLocalization] 1,'Admin.ExportFeeds.IndexFacebook.subtitle', 'Канал продаж Facebook'
EXEC [Settings].[sp_AddUpdateLocalization] 2,'Admin.ExportFeeds.IndexFacebook.subtitle', 'Facebook sales channel'

GO--

UPDATE [Settings].[InternalSettings] SET [settingValue] = '13.0.4' WHERE [settingKey] = 'db_version'
