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