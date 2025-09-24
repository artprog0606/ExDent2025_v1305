


IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Order].[ShippingReplaceGeo]') AND type in (N'U'))
BEGIN
CREATE TABLE [Order].[ShippingReplaceGeo](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ShippingType] [nvarchar](50) NOT NULL,
	[InCountryName] [nvarchar](70) NOT NULL,
	[InCountryISO2] [varchar](2) NOT NULL,
	[InRegionName] [nvarchar](70) NOT NULL,
	[InCityName] [nvarchar](255) NOT NULL,
	[InDistrict] [nvarchar](255) NOT NULL,
	[OutCountryName] [nvarchar](70) NOT NULL,
	[OutRegionName] [nvarchar](70) NOT NULL,
	[OutCityName] [nvarchar](255) NOT NULL,
	[OutDistrict] [nvarchar](255) NOT NULL,
	[OutDistrictClear] [bit] NOT NULL,
	[Enabled] [bit] NOT NULL,
	[Sort] [int] NOT NULL,
 CONSTRAINT [PK_ShippingReplaceGeo] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END

GO--

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[Order].[ShippingReplaceGeo]') AND name = N'ShippingReplaceGeo_ShippingType')
CREATE NONCLUSTERED INDEX [ShippingReplaceGeo_ShippingType] ON [Order].[ShippingReplaceGeo]
(
	[ShippingType] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO--

IF NOT EXISTS (SELECT * FROM [Order].[ShippingReplaceGeo])
BEGIN
DBCC CHECKIDENT ('Order.ShippingReplaceGeo', RESEED, 5000);
END

GO--


SET IDENTITY_INSERT [Order].[ShippingReplaceGeo] ON 


IF NOT EXISTS (SELECT * FROM [Order].[ShippingReplaceGeo] WHERE [Id] = 1)
INSERT INTO [Order].[ShippingReplaceGeo] ([Id],[ShippingType],[InCountryName],[InCountryISO2],[InRegionName],[InCityName],[InDistrict],[OutCountryName],[OutRegionName],[OutCityName],[OutDistrict],[OutDistrictClear],[Enabled],[Sort])
VALUES (1,'Sdek','','','Республика Саха (Якутия)','','','','Саха /Якутия/','','',0,1,0)

IF NOT EXISTS (SELECT * FROM [Order].[ShippingReplaceGeo] WHERE [Id] = 2)
INSERT INTO [Order].[ShippingReplaceGeo] ([Id],[ShippingType],[InCountryName],[InCountryISO2],[InRegionName],[InCityName],[InDistrict],[OutCountryName],[OutRegionName],[OutCityName],[OutDistrict],[OutDistrictClear],[Enabled],[Sort])
VALUES (2,'Sdek','','','Ханты-Мансийский АО','','','','Ханты-Мансийский Автономный округ - Югра','','',0,1,0)

IF NOT EXISTS (SELECT * FROM [Order].[ShippingReplaceGeo] WHERE [Id] = 3)
INSERT INTO [Order].[ShippingReplaceGeo] ([Id],[ShippingType],[InCountryName],[InCountryISO2],[InRegionName],[InCityName],[InDistrict],[OutCountryName],[OutRegionName],[OutCityName],[OutDistrict],[OutDistrictClear],[Enabled],[Sort])
VALUES (3,'Sdek','','KZ','Алма-Ата','','','','Алма-Атинская область','','',0,1,0)

IF NOT EXISTS (SELECT * FROM [Order].[ShippingReplaceGeo] WHERE [Id] = 4)
INSERT INTO [Order].[ShippingReplaceGeo] ([Id],[ShippingType],[InCountryName],[InCountryISO2],[InRegionName],[InCityName],[InDistrict],[OutCountryName],[OutRegionName],[OutCityName],[OutDistrict],[OutDistrictClear],[Enabled],[Sort])
VALUES (4,'Sdek','','KZ','Алматинская область','','','','Алма-Атинская область','','',0,1,0)

IF NOT EXISTS (SELECT * FROM [Order].[ShippingReplaceGeo] WHERE [Id] = 5)
INSERT INTO [Order].[ShippingReplaceGeo] ([Id],[ShippingType],[InCountryName],[InCountryISO2],[InRegionName],[InCityName],[InDistrict],[OutCountryName],[OutRegionName],[OutCityName],[OutDistrict],[OutDistrictClear],[Enabled],[Sort])
VALUES (5,'Sdek','','KZ','','Алма-Ата','','','','Алматы','',0,1,0)

IF NOT EXISTS (SELECT * FROM [Order].[ShippingReplaceGeo] WHERE [Id] = 6)
INSERT INTO [Order].[ShippingReplaceGeo] ([Id],[ShippingType],[InCountryName],[InCountryISO2],[InRegionName],[InCityName],[InDistrict],[OutCountryName],[OutRegionName],[OutCityName],[OutDistrict],[OutDistrictClear],[Enabled],[Sort])
VALUES (6,'Sdek','Киргизия','KG','Киргизия','','','','Кыргызстан','','',0,1,0)

IF NOT EXISTS (SELECT * FROM [Order].[ShippingReplaceGeo] WHERE [Id] = 7)
INSERT INTO [Order].[ShippingReplaceGeo] ([Id],[ShippingType],[InCountryName],[InCountryISO2],[InRegionName],[InCityName],[InDistrict],[OutCountryName],[OutRegionName],[OutCityName],[OutDistrict],[OutDistrictClear],[Enabled],[Sort])
VALUES (7,'Sdek','Казахстан','KZ','Астана','','','','Акмолинская область','','',0,1,0)

IF NOT EXISTS (SELECT * FROM [Order].[ShippingReplaceGeo] WHERE [Id] = 8)
INSERT INTO [Order].[ShippingReplaceGeo] ([Id],[ShippingType],[InCountryName],[InCountryISO2],[InRegionName],[InCityName],[InDistrict],[OutCountryName],[OutRegionName],[OutCityName],[OutDistrict],[OutDistrictClear],[Enabled],[Sort])
VALUES (8,'Sdek','Казахстан','KZ','','Астана','','','','Нур-Султан (Астана)','',0,1,0)

IF NOT EXISTS (SELECT * FROM [Order].[ShippingReplaceGeo] WHERE [Id] = 9)
INSERT INTO [Order].[ShippingReplaceGeo] ([Id],[ShippingType],[InCountryName],[InCountryISO2],[InRegionName],[InCityName],[InDistrict],[OutCountryName],[OutRegionName],[OutCityName],[OutDistrict],[OutDistrictClear],[Enabled],[Sort])
VALUES (9,'Sdek','Казахстан','KZ','','Нур-Султан','','','','Нур-Султан (Астана)','',0,1,0)

IF NOT EXISTS (SELECT * FROM [Order].[ShippingReplaceGeo] WHERE [Id] = 10)
INSERT INTO [Order].[ShippingReplaceGeo] ([Id],[ShippingType],[InCountryName],[InCountryISO2],[InRegionName],[InCityName],[InDistrict],[OutCountryName],[OutRegionName],[OutCityName],[OutDistrict],[OutDistrictClear],[Enabled],[Sort])
VALUES (10,'Sdek','','KZ','Кызылординская область','Кызылорда','','','Казахстан','','',0,1,0)

IF NOT EXISTS (SELECT * FROM [Order].[ShippingReplaceGeo] WHERE [Id] = 11)
INSERT INTO [Order].[ShippingReplaceGeo] ([Id],[ShippingType],[InCountryName],[InCountryISO2],[InRegionName],[InCityName],[InDistrict],[OutCountryName],[OutRegionName],[OutCityName],[OutDistrict],[OutDistrictClear],[Enabled],[Sort])
VALUES (11,'Sdek','','KZ','Атырауская область','Атырау','','','','Атырау (Гурьев)','',0,1,0)

IF NOT EXISTS (SELECT * FROM [Order].[ShippingReplaceGeo] WHERE [Id] = 12)
INSERT INTO [Order].[ShippingReplaceGeo] ([Id],[ShippingType],[InCountryName],[InCountryISO2],[InRegionName],[InCityName],[InDistrict],[OutCountryName],[OutRegionName],[OutCityName],[OutDistrict],[OutDistrictClear],[Enabled],[Sort])
VALUES (12,'Sdek','','KZ','Актюбинская область','Актюбинск','','','','Актобе','',0,1,0)

IF NOT EXISTS (SELECT * FROM [Order].[ShippingReplaceGeo] WHERE [Id] = 13)
INSERT INTO [Order].[ShippingReplaceGeo] ([Id],[ShippingType],[InCountryName],[InCountryISO2],[InRegionName],[InCityName],[InDistrict],[OutCountryName],[OutRegionName],[OutCityName],[OutDistrict],[OutDistrictClear],[Enabled],[Sort])
VALUES (13,'Sdek','','KZ','Костанайская область','Костанай','','','Казахстан','','',0,1,0)

IF NOT EXISTS (SELECT * FROM [Order].[ShippingReplaceGeo] WHERE [Id] = 14)
INSERT INTO [Order].[ShippingReplaceGeo] ([Id],[ShippingType],[InCountryName],[InCountryISO2],[InRegionName],[InCityName],[InDistrict],[OutCountryName],[OutRegionName],[OutCityName],[OutDistrict],[OutDistrictClear],[Enabled],[Sort])
VALUES (14,'Sdek','Казахстан','KZ','Нур-Султан','','','','Акмолинская область','','',0,1,0)

IF NOT EXISTS (SELECT * FROM [Order].[ShippingReplaceGeo] WHERE [Id] = 15)
INSERT INTO [Order].[ShippingReplaceGeo] ([Id],[ShippingType],[InCountryName],[InCountryISO2],[InRegionName],[InCityName],[InDistrict],[OutCountryName],[OutRegionName],[OutCityName],[OutDistrict],[OutDistrictClear],[Enabled],[Sort])
VALUES (15,'Sdek','Казахстан','KZ','Акмолинская область','Кокшетау','','','Казахстан','','',0,1,0)

IF NOT EXISTS (SELECT * FROM [Order].[ShippingReplaceGeo] WHERE [Id] = 16)
INSERT INTO [Order].[ShippingReplaceGeo] ([Id],[ShippingType],[InCountryName],[InCountryISO2],[InRegionName],[InCityName],[InDistrict],[OutCountryName],[OutRegionName],[OutCityName],[OutDistrict],[OutDistrictClear],[Enabled],[Sort])
VALUES (16,'Sdek','Армения','AM','Ереван','','','','Армения','','',0,1,0)


SET IDENTITY_INSERT [Order].[ShippingReplaceGeo] OFF

GO--

If exists(Select 1 From [Customers].[Region] Where RegionName = 'Удмуртия')
	Update [Customers].[Region] Set RegionName = 'Удмуртская Республика' Where RegionName = 'Удмуртия'

GO--

If exists(Select 1 From [Customers].[Region] Where RegionName = 'Ханты-Мансийский АО')
	Update [Customers].[Region] Set RegionName = 'Ханты-Мансийский АО - Югра' Where RegionName = 'Ханты-Мансийский АО'

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Checkout.TypeSortOrderShippings.Default', 'По порядку сортировки')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Checkout.TypeSortOrderShippings.Default', 'By sorting order')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Checkout.TypeSortOrderShippings.AscByRate', 'По возрастанию стоимости')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Checkout.TypeSortOrderShippings.AscByRate', 'Cost, ascending')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Checkout.TypeSortOrderShippings.DescByRate', 'По убыванию стоимости')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Checkout.TypeSortOrderShippings.DescByRate', 'Cost, descending')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Checkout.TypeSortOrderShippings', 'Выводить доставку')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Checkout.TypeSortOrderShippings', 'Sort delivery')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ShippingMethods.Common.MoveToEnd', 'Переносить в конец списка')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ShippingMethods.Common.MoveToEnd', 'Move to the end of the list')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ShippingMethods.Common.ShowIfNoOtherShippings', 'Показывать только при отсутствии других доставок')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ShippingMethods.Common.ShowIfNoOtherShippings', 'Show only if there are no other deliveries')

GO--

ALTER TABLE [Order].[ShippingMethod] ADD
	[MoveToEnd] bit NULL,
	[ShowIfNoOtherShippings] bit NULL
	
GO--



declare @MoskowRegionId int = (SELECT TOP 1 [RegionID] FROM [Customers].[Region] WHERE [RegionName] = 'Москва')

IF (@MoskowRegionId IS NOT NULL)
BEGIN
	UPDATE [Customers].[City]
	   SET [RegionID] = @MoskowRegionId
	WHERE [CityName] = 'Коммунарка'

	IF (NOT EXISTS(SELECT 1 FROM [Customers].[City] WHERE [CityName] = 'Зеленоград' AND [RegionID] = @MoskowRegionId))
	BEGIN
		INSERT INTO [Customers].[City] ([RegionID],[CityName],[CitySort],[DisplayInPopup],[PhoneNumber],[MobilePhoneNumber],[Zip])
		VALUES (@MoskowRegionId,'Зеленоград',0,0,NULL,NULL,'124365')
	END
END

GO-- 

declare @KZId int = (SELECT TOP 1 [CountryID] FROM [Customers].[Country] WHERE [CountryISO2] = 'KZ')
declare @NS_CityId int
declare @NS_RegionId int
IF (@KZId IS NOT NULL)
BEGIN
	SET @NS_RegionId = (SELECT TOP 1 [RegionID] FROM [Customers].[Region] WHERE [RegionName] = 'Нур-Султан')

	IF (@NS_RegionId IS NULL)
	BEGIN
		INSERT INTO [Customers].[Region] ([CountryID],[RegionName],[RegionCode],[RegionSort])
		VALUES (@KZId,'Нур-Султан',NULL,0);
		SET @NS_RegionId = (SELECT SCOPE_IDENTITY())
	END

	IF (NOT EXISTS(SELECT 1 FROM [Customers].[City] WHERE [CityName] = 'Нур-Султан' AND [RegionID] = @NS_RegionId))
	BEGIN
		INSERT INTO [Customers].[City] ([RegionID],[CityName],[CitySort],[DisplayInPopup],[PhoneNumber],[MobilePhoneNumber],[Zip])
		VALUES (@NS_RegionId,'Нур-Султан',100,1,NULL,NULL,NULL)
	END

END

GO-- 


declare @RegionIdMove int = (select RegionId from Customers.Region where RegionName = 'Ставрополь')
declare @RegionId int = (select RegionId from Customers.Region where RegionName = 'Ставропольский край')
if @RegionIdMove is not null and @RegionId is not null
begin
	UPDATE Customers.City SET RegionId = @RegionId WHERE RegionId = @RegionIdMove
	UPDATE Customers.Contact SET RegionId = @RegionId WHERE RegionId = @RegionIdMove
	UPDATE [Order].ShippingRegion SET RegionId = @RegionId WHERE RegionId = @RegionIdMove
	UPDATE [Order].ShippingRegionExcluded SET RegionId = @RegionId WHERE RegionId = @RegionIdMove
	DELETE FROM Customers.Region WHERE RegionId = @RegionIdMove
end

UPDATE Customers.Region SET RegionName = 'Республика Крым' WHERE RegionName = 'Крым'

GO--

UPDATE [Settings].[InternalSettings] SET [settingValue] = '8.5.7' WHERE [settingKey] = 'db_version'