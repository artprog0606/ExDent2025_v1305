
If exists (Select 1 From [Settings].[Settings] Where [Name] = 'GoogleAnalyticsNumber' and [Value] <> '' and value not like '%UA-%' )
	Update [Settings].[Settings] Set [Value] = 'UA-' + [Value] Where [Name] = 'GoogleAnalyticsNumber'

GO--

If exists(Select 1 From [Customers].[Region] Where RegionName = 'Кабардино-Балкария')
	Update [Customers].[Region] Set RegionName = 'Кабардино-Балкарская республика' Where RegionName = 'Кабардино-Балкария'
	
GO--

declare @MoskowRegionId int = (SELECT TOP 1 [RegionID] FROM [Customers].[Region] WHERE [RegionName] = 'Москва')
declare @MoskowOblRegionId int = (SELECT TOP 1 [RegionID] FROM [Customers].[Region] WHERE [RegionName] = 'Московская область')

IF (@MoskowRegionId IS NOT NULL AND @MoskowOblRegionId IS NOT NULL)
BEGIN
	UPDATE [Customers].[City]
	   SET [RegionID] = @MoskowRegionId
	WHERE [CityName] = 'Троицк' AND [RegionID] = @MoskowOblRegionId
END

GO--


UPDATE [Settings].[InternalSettings] SET [settingValue] = '8.5.8' WHERE [settingKey] = 'db_version'