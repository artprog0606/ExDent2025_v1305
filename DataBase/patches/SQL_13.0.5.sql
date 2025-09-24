EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.ShippingMethods.FivePost.Rate', 'Тариф'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.ShippingMethods.FivePost.Rate', 'Rate' 

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.ShippingMethods.FivePost.DeliverySL', 'Логистическая схема'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.ShippingMethods.FivePost.DeliverySL', 'Logistics scheme' 

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.ShippingMethods.FivePost.RateDeliverySLReference', 'Логистическая схема доставки по тарифу'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.ShippingMethods.FivePost.RateDeliverySLReference', 'Logistics scheme of delivery according to the tariff' 

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.ShippingMethods.FivePost.DeliverySLHint', 'Логистическая схема, привязанная к тарифу. Можно узнать у менеджера 5Пост.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.ShippingMethods.FivePost.DeliverySLHint', 'A logistics scheme linked to a rate. You can ask the manager for 5 Posts.' 

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.OrderItemsSummary.DontCallBack', 'Не перезванивать'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.OrderItemsSummary.DontCallBack', 'Do not call back' 

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.VkAuth.Step1', '<span class="bold">Шаг 1.</span> Создайте приложение в ВКонтакте.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.VkAuth.Step1', '<span class="bold">Step 1.</span> Create VK ID application.'

GO--

if not exists (Select 1 From [Settings].[Settings] Where Name = 'SettingsVk.UserTokenData') and 
       exists (Select 1 From [Settings].[Settings] Where Name = 'SettingsVk.TokenUser' and [Value] IS NOT NULL and LTRIM(RTRIM(Value)) <> '')
begin
	-- save TokenUser to UserTokenData json
	Insert Into [Settings].[Settings] (Name, Value) 
	Select 'SettingsVk.UserTokenData', 
		   '{"refresh_token":null,"access_token":"' + [Value] + '","id_token":null,"token_type":null,"expires_in":0,"user_id":0,"state":null,"scope":null,"device_id":null,"client_id":null}' 
	From [Settings].[Settings] 
	Where Name = 'SettingsVk.TokenUser'
	
	-- rename TokenUser setting
	Update [Settings].[Settings] 
	Set [Name] = 'SettingsVk.TokenUser_previous'
	Where Name = 'SettingsVk.TokenUser'

	-- set use old integration (without refresh token)
	Insert Into [Settings].[Settings] (Name, Value) Values ('SettingsVk.IsOldIntegration', 'True') 
end

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.SystemSettings.AuthVkIdActive', 'Включен';
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.SystemSettings.AuthVkIdActive', 'Active';

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.SystemSettings.AuthVkIdClientId', 'ID приложения';
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.SystemSettings.AuthVkIdClientId', 'Application ID';

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.SystemSettings.AuthVkCallbackURL', 'Доверенный Redirect URL';
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.SystemSettings.AuthVkCallbackURL', 'Trusted Redirect URL';

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.SystemSettings.AuthVkIdInstruction', 'Инструкция. Настройка кнопок авторизации VK ID ВКонтакте';
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.SystemSettings.AuthVkIdInstruction', 'Instructions. Setting up VK ID authentication keys vKontakte';

GO--

CREATE FUNCTION [dbo].[NvarcharToBigint] (@input NVARCHAR(MAX))
    RETURNS BIGINT
AS
BEGIN
    IF @input IS NULL
        RETURN NULL;

    DECLARE
        @result NVARCHAR(MAX) = '', 
        @index INT = 1;
    
    WHILE @index <= LEN(@input) AND LEN(@result) <= 17
        BEGIN
            DECLARE @char NCHAR(1) = SUBSTRING(@input, @index, 1);
    
            IF @char LIKE '[0-9]'
                BEGIN
                    SET @result = @result + @char;
                END;
    
            SET @index = @index + 1;
        END;
        
    IF LEN(@result) <= 1
        RETURN NULL;

    RETURN CAST(@result AS BIGINT);
END;

GO--

UPDATE [Customers].[Customer]
SET [StandardPhone] = [dbo].[NvarcharToBigint]([Phone])
WHERE [StandardPhone] IS NULL
  AND [Phone] IS NOT NULL

GO--

DROP FUNCTION [dbo].[NvarcharToBigint];

GO--

UPDATE Settings.ExportFeedSettings
SET FileName = 'export/catalog'
WHERE ExportFeedId = 2
  AND EXISTS (
    SELECT 1
    FROM Settings.ExportFeed
    WHERE Id = 2
      AND LastExportFileFullName IS NULL
)

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Checkout.ClearShoppingCartBeforeBuyByLink', 'Очищать корзину перед добавлением товара по ссылке'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Checkout.ClearShoppingCartBeforeBuyByLink', 'Empty the shopping cart before adding an item via a link'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Checkout.ClearShoppingCartBeforeBuyByLink.Help', 'При добавлении товара в корзину по ссылке корзина будет очищена'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Checkout.ClearShoppingCartBeforeBuyByLink.Help', 'When adding an item to the cart via the link, the cart will be emptied.'

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

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Home.Menu.ReferralProgram', 'Приведи друга'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Home.Menu.ReferralProgram', 'Bring a friend'


Delete From [Settings].[Localization] Where [ResourceKey] = 'Admin.Common.TopPanel.FindSpecialist'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Common.TopPanel.OrderImplementation', 'Заказать внедрение'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Common.TopPanel.OrderImplementation', 'Order implementation'

GO--

IF EXISTS (select * from dbo.sysobjects where id = object_id(N'[Customers].[sp_AddCustomer]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
    DROP PROCEDURE [Customers].[sp_AddCustomer]

IF EXISTS (select * from dbo.sysobjects where id = object_id(N'[Customers].[sp_UpdateCustomerInfo]  ') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	DROP PROCEDURE [Customers].[sp_UpdateCustomerInfo]

GO--

if not exists (select * from sys.columns where (name = N'AgreeForPromotionalNewsletterFrom') AND object_id = OBJECT_ID(N'[Customers].[Customer]'))
begin
    alter table [Customers].[Customer]
    add AgreeForPromotionalNewsletterFrom nvarchar(500),
		AgreeForPromotionalNewsletterFromIp nvarchar(200),
		AgreeForPromotionalNewsletterDateTime datetime,
		RegisteredFromIp nvarchar(200)
end

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Customers.RightBlock.RegisteredFromIp', 'Регистрация с IP'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Customers.RightBlock.RegisteredFromIp', 'Registered from IP'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Customers.RightBlock.AgreeForPromotionalNewsletterDateTime', 'Дата'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Customers.RightBlock.AgreeForPromotionalNewsletterDateTime', 'Date'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Customers.RightBlock.AgreeForPromotionalNewsletterFrom', 'Cо страницы '
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Customers.RightBlock.AgreeForPromotionalNewsletterFrom', 'Page'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Customers.RightBlock.AgreeForPromotionalNewsletterFromIp', 'IP'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Customers.RightBlock.AgreeForPromotionalNewsletterFromIp', 'IP'

GO--

if not exists (select * from sys.columns where (name = N'SubscribeFromPage') AND object_id = OBJECT_ID(N'[Customers].[Subscription]'))
begin
    alter table [Customers].[Subscription]
        add SubscribeFromPage nvarchar(500),
            SubscribeFromIp nvarchar(200)
end

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Customers.RightBlock.NewSubscription.Date', 'Дата'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Customers.RightBlock.NewSubscription.Date', 'Date'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Customers.RightBlock.NewSubscription.FromPage', 'Cо страницы '
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Customers.RightBlock.NewSubscription.FromPage', 'Page'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Customers.RightBlock.NewSubscription.FromIp', 'IP'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Customers.RightBlock.NewSubscription.FromIp', 'IP'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Subscription.FromPage', 'Cо страницы'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Subscription.FromPage', 'Page'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Subscription.FromIp', 'IP'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Subscription.FromIp', 'IP'

GO--

if not exists (Select 1 From [Settings].[InternalSettings] Where [settingKey] = 'ActivityEmailLogServiceUrl')
begin
	Insert Into [Settings].[InternalSettings] ([settingKey], [settingValue]) Values ('ActivityEmailLogServiceUrl', 'https://activity.advsrvone.pw/email/')
end

GO--

if not exists (Select 1 From [Settings].[InternalSettings] Where [settingKey] = 'ActivitySmsLogServiceUrl')
begin
	Insert Into [Settings].[InternalSettings] ([settingKey], [settingValue]) Values ('ActivitySmsLogServiceUrl', 'https://activity.advsrvone.pw/sms/')
end

GO--

if not exists (select * from sys.columns where (name = N'Ip') AND object_id = OBJECT_ID(N'[Order].[OrderTrafficSource]'))
begin
    alter table [Order].[OrderTrafficSource]
        add Ip nvarchar(200)
end

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Leads.Description.IP', 'IP'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Leads.Description.IP', 'IP'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Orders.Orderinfo.IP', 'IP'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Orders.Orderinfo.IP', 'IP'

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.GoogleAnalytics.RussianPersonalDataProcessingPolicyWarning', 'По законодательству РФ на сайте в политике обработки персональных данных должна быть отражена информация об использовании технологий собирающих данные о посещениях пользователя и о том, что осуществляется трансграничная передача данных'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.GoogleAnalytics.RussianPersonalDataProcessingPolicyWarning', 'According to Russian legislation, the personal data processing policy on the website must reflect information about the use of technologies that collect data on user visits and that cross-border data transfer is carried out'

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.GoogleAnalytics.RussianCrossBorderDataTransferWarning', 'Для работы в рамках действующего законодательства Российской Федерации необходимо подать заявление о трансграничной передаче данных'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.GoogleAnalytics.RussianCrossBorderDataTransferWarning', 'To operate within the framework of the current legislation of the Russian Federation, it is necessary to submit an application for cross-border data transfer'

GO--

if exists (select *
           from Settings.Localization
           where ResourceKey = 'MyAccount.CommonInfo.UserAgreementForPromotionalNewsletter'
             and LanguageId = 1
             and ResourceValue = N'Согласие на получение рассылок')
    EXEC [Settings].[sp_AddUpdateLocalization] 1,
         'MyAccount.CommonInfo.UserAgreementForPromotionalNewsletter',
         N'Согласен получать информационные и рекламные рассылки'

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.ExportImport.CustomerFields.RegisteredFrom', 'Регистрация со страницы'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.ExportImport.CustomerFields.RegisteredFrom', 'Registered from'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.ExportImport.CustomerFields.RegisteredFromIp', 'Регистрация с IP'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.ExportImport.CustomerFields.RegisteredFromIp', 'Registered from IP'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.ExportImport.CustomerFields.AgreeForPromotionalNewsletterDateTime', 'Дата получения согласия на информационные рассылки'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.ExportImport.CustomerFields.AgreeForPromotionalNewsletterDateTime', 'Date of agree for promotional newsletter'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.ExportImport.CustomerFields.AgreeForPromotionalNewsletterFrom', 'Страница получения согласия на информационные рассылки'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.ExportImport.CustomerFields.AgreeForPromotionalNewsletterFrom', 'Agree for promotional newsletter from'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.ExportImport.CustomerFields.AgreeForPromotionalNewsletterFromIp', 'IP получения согласия на информационные рассылки'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.ExportImport.CustomerFields.AgreeForPromotionalNewsletterFromIp', 'IP for promotional newsletter from'

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.CookiesPolicy.Modal.Accept', 'Принимаю';
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.CookiesPolicy.Modal.Accept', 'Accept';

GO--

if exists (select *
           from sys.columns
           where (name = N'IsAgreeForPromotionalNewsletter')
             AND object_id = OBJECT_ID(N'[Customers].[Customer]'))
    begin
        insert into Customers.Subscription (Email, Subscribe, SubscribeDate, SubscribeFromPage, SubscribeFromIp)
        select Customer.Email,
               1,
               ISNULL(AgreeForPromotionalNewsletterDateTime, GETDATE()),
               Customer.AgreeForPromotionalNewsletterFrom,
               Customer.AgreeForPromotionalNewsletterFromIp
        from Customers.Customer
        where IsAgreeForPromotionalNewsletter = 1
          and not exists (select * from Customers.Subscription where Subscription.Email = Customer.Email)

        alter table Customers.Customer
            drop column IsAgreeForPromotionalNewsletter
    end

if exists (select *
           from sys.columns
           where (name = N'AgreeForPromotionalNewsletterFrom')
             AND object_id = OBJECT_ID(N'[Customers].[Customer]'))
    begin
        alter table [Customers].[Customer]
            drop column AgreeForPromotionalNewsletterFrom,
						AgreeForPromotionalNewsletterFromIp,
						AgreeForPromotionalNewsletterDateTime
    end

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Subscribe.Export.SubscribeFromPage', 'Со страницы'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Subscribe.Export.SubscribeFromIp', 'From page'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Subscribe.Export.SubscribeFromIp', 'С IP'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Subscribe.Export.SubscribeFromIp', 'From IP'

GO--

UPDATE [Settings].[InternalSettings] SET [settingValue] = '13.0.5' WHERE [settingKey] = 'db_version'
