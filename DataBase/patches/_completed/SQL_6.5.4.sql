ALTER TABLE Booking.Service ADD
	Duration bigint NULL

GO--

UPDATE [Settings].[Localization] set [ResourceValue] = 'Значение свойства' WHERE [ResourceKey] = 'Admin.Js.ProductProperties.Value' AND [LanguageId] = 1

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.Productlists.ChangesSaved','Изменения сохранены')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.Productlists.ChangesSaved','Changes saved')

GO--


alter table [CMS].[LandingBlock] add NoCache bit null
GO--
update [CMS].[LandingBlock] set NoCache = 0
GO--
alter table [CMS].[LandingBlock] alter column NoCache bit not null
GO--

UPDATE [CMS].[StaticBlock] SET [Content] = 'Благодарим за Вашу заявку! После её обработки наш менеджер сразу же свяжется с Вами и сообщит о возможности и сроках поступления данной позиции в наш интернет-магазин.' WHERE [Key] = 'requestOnProductSuccess' AND [Content] = ''

GO--

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (1, 'Admin.Js.Product.YandexDeliveryDaysDocumentation', 'Срок доставки - атрибут days');
Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (2, 'Admin.Js.Product.YandexDeliveryDaysDocumentation', 'Delivery Time - days attribute');

GO--

If (EXISTS(Select * From [CMS].[StaticBlock] Where [Key] = 'requestOnProductSuccess') and EXISTS(Select * From [CMS].[StaticBlock] Where [Key] = 'requestOnProductSuccess' and [Content] = ''))
Begin
	Update [CMS].[StaticBlock] 
	Set [Content] = 'Благодарим за Вашу заявку! После её обработки наш менеджер сразу же свяжется с Вами и сообщит о возможности и сроках поступления данной позиции в наш интернет-магазин.' 
	Where [Key] = 'requestOnProductSuccess' and [Content] = ''
End

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (1, 'Admin.Category.AdminCategoryModel.Error.DiscountPercent', 'Укажите скидку в процентах от 0 до 100'); 
Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (2, 'Admin.Category.AdminCategoryModel.Error.DiscountPercent', 'Enter a discount percentage from 0 to 100'); 

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (1, 'Admin.Category.AdminCategoryModel.Error.DiscountAmount', 'Укажите скидку больше или равно 0'); 
Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (2, 'Admin.Category.AdminCategoryModel.Error.DiscountAmount', 'Enter a discount percentage from 0 to 100'); 

GO--
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.ExportImport.ExportFeedAvito.PaidPublicationOption.Package','package')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.ExportImport.ExportFeedAvito.PaidPublicationOption.Package','package')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.ExportImport.ExportFeedAvito.PaidPublicationOption.PackageSingle','PackageSingle')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.ExportImport.ExportFeedAvito.PaidPublicationOption.PackageSingle','PackageSingle')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.ExportImport.ExportFeedAvito.PaidPublicationOption.Single','Single')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.ExportImport.ExportFeedAvito.PaidPublicationOption.Single','Single')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.ExportImport.ExportFeedAvito.PaidServices.Free','Бесплатная')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.ExportImport.ExportFeedAvito.PaidServices.Free','Free')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.ExportImport.ExportFeedAvito.PaidServices.Premium','Премиум')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.ExportImport.ExportFeedAvito.PaidServices.Premium','Premium')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.ExportImport.ExportFeedAvito.PaidServices.VIP','VIP-объявление')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.ExportImport.ExportFeedAvito.PaidServices.VIP','VIP')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.ExportImport.ExportFeedAvito.PaidServices.PushUp','Поднятие объявления в поиске')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.ExportImport.ExportFeedAvito.PaidServices.PushUp','PushUp')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.ExportImport.ExportFeedAvito.PaidServices.Highlight','Выделение объявления')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.ExportImport.ExportFeedAvito.PaidServices.Highlight','Highlight')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.ExportImport.ExportFeedAvito.PaidServices.TurboSale','Применение пакета «Турбо-продажа»')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.ExportImport.ExportFeedAvito.PaidServices.TurboSale','TurboSale')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.ExportImport.ExportFeedAvito.PaidServices.QuickSale','Применение пакета «Быстрая продажа»')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.ExportImport.ExportFeedAvito.PaidServices.QuickSale','QuickSale')


INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ExportFeed.SettingsAvito.PublicationDateOffset','Смещение даты публикации')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ExportFeed.SettingsAvito.PublicationDateOffset','Publication date offset')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ExportFeed.SettingsAvito.DurationOfPublicationInDays','Длительность публикации в днях')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ExportFeed.SettingsAvito.DurationOfPublicationInDays','Duration of publication in days')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ExportFeed.SettingsAvito.PaidPublicationOption','Вариант платного размещения')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ExportFeed.SettingsAvito.PaidPublicationOption','Paid publication option')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ExportFeed.SettingsAvito.PaidServices','Платные услуги')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ExportFeed.SettingsAvito.PaidServices','Paid services')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ExportFeed.SettingsAvito.EmailMessages','Сообщения на email')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ExportFeed.SettingsAvito.EmailMessages','Email messages')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ExportFeed.SettingsAvito.ManagerName','Имя менеджера')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ExportFeed.SettingsAvito.ManagerName','Manager name')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ExportFeed.SettingsAvito.Phone','Контактный телефон')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ExportFeed.SettingsAvito.Phone','Phone')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ExportFeed.SettingsAvito.Region','Регион')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ExportFeed.SettingsAvito.Region','Region')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ExportFeed.SettingsAvito.City','Город или населенный пункт')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ExportFeed.SettingsAvito.City','City')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ExportFeed.SettingsAvito.MetroStation','Ближайшая станция метро')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ExportFeed.SettingsAvito.MetroStation','Metro station')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ExportFeed.SettingsAvito.CityArea','Район города')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ExportFeed.SettingsAvito.CityArea','City area')


INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.ExportImport.ExportFeed.Avito','Avito автовыгрузка')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.ExportImport.ExportFeed.Avito','Avito')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Import.ImportProducts.ImportRemains','Режим импорта остатков')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Import.ImportProducts.ImportRemains','Import remains mode')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.PaymentMethods.Common.Markup','Наценка')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.PaymentMethods.Common.Markup','Markup')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.PaymentMethods.Common.MarkupOfPaymentMethod','Тут вы указываете саму наценку на метод оплаты, если она есть.')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.PaymentMethods.Common.MarkupOfPaymentMethod','Here you specify the mark-up on the payment method, if any.')

GO--

ALTER TABLE [Booking].[ReservationResource] DROP CONSTRAINT [FK_ReservationResource_Managers]
GO--

ALTER TABLE [Booking].[ReservationResource]  WITH CHECK ADD  CONSTRAINT [FK_ReservationResource_Managers] FOREIGN KEY([ManagerId])
REFERENCES [Customers].[Managers] ([ManagerId])
ON DELETE SET NULL
GO--

ALTER TABLE [Booking].[ReservationResource] CHECK CONSTRAINT [FK_ReservationResource_Managers]
GO--

ALTER TABLE Booking.Affiliate ADD
	AccessForAll bit NULL

GO--

UPDATE Booking.Affiliate SET AccessForAll = 1

GO--

ALTER TABLE Booking.Affiliate ALTER COLUMN
	AccessForAll bit NOT NULL

GO--

CREATE TABLE [Booking].[AffiliateManager](
	[AffiliateId] [int] NOT NULL,
	[ManagerId] [int] NOT NULL,
 CONSTRAINT [PK_AffiliateManager] PRIMARY KEY CLUSTERED 
(
	[AffiliateId] ASC,
	[ManagerId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO--

ALTER TABLE [Booking].[AffiliateManager]  WITH CHECK ADD  CONSTRAINT [FK_AffiliateManager_Affiliate] FOREIGN KEY([AffiliateId])
REFERENCES [Booking].[Affiliate] ([Id])
ON DELETE CASCADE
GO--

ALTER TABLE [Booking].[AffiliateManager] CHECK CONSTRAINT [FK_AffiliateManager_Affiliate]
GO--

ALTER TABLE [Booking].[AffiliateManager]  WITH CHECK ADD  CONSTRAINT [FK_AffiliateManager_Managers] FOREIGN KEY([ManagerId])
REFERENCES [Customers].[Managers] ([ManagerId])
ON DELETE CASCADE
GO--

ALTER TABLE [Booking].[AffiliateManager] CHECK CONSTRAINT [FK_AffiliateManager_Managers]
GO--


INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.Tasks.ModalEditTaskCtrl.AreYouSureCopy','Сделать копию задачи?')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.Tasks.ModalEditTaskCtrl.AreYouSureCopy','Are you sure want to make copy of task?')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.Tasks.ModalEditTaskCtrl.Copy','Копирование')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.Tasks.ModalEditTaskCtrl.Copy','Copying')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.Tasks.EditTask.Copy','Копировать')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.Tasks.EditTask.Copy','Copy')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.Tasks.ModalEditTaskCtrl.CopySuccess','Копия успешно сделана')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.Tasks.ModalEditTaskCtrl.CopySuccess','Tasks copy has been done')

GO--

update [Settings].[Localization] set ResourceValue ='Академия'  where ResourceValue = 'Акдемия'

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.SettingsCrm.SocialNetworks.LimitRiched','Превышено число подключенных CRM-интеграций')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.SettingsCrm.SocialNetworks.LimitRiched','Exceeded the number of CRM-integrations')

GO--


IF (NOT EXISTS(SELECT 1 FROM [Booking].[Affiliate]) AND NOT EXISTS(SELECT 1 FROM [Booking].[Category]) AND NOT EXISTS(SELECT 1 FROM [Booking].[Service]) AND NOT EXISTS(SELECT 1 FROM [Booking].[ReservationResource]))
BEGIN
	DECLARE @CurrencyID INT
	SELECT TOP 1 @CurrencyID = [CurrencyID] FROM [Catalog].[Currency] WHERE [CurrencyIso3] = 'RUB'
	IF (@CurrencyID IS NULL)
	BEGIN
		SELECT TOP 1 @CurrencyID = [CurrencyID] FROM [Catalog].[Currency]
	END
	
	SET IDENTITY_INSERT [Booking].[Category] ON
	INSERT [Booking].[Category] ([Id], [Name], [Image], [SortOrder], [Enabled]) VALUES (1, N'Категория услуг 1', NULL, 10, 1)
	INSERT [Booking].[Category] ([Id], [Name], [Image], [SortOrder], [Enabled]) VALUES (2, N'Категория услуг 2', NULL, 20, 1)
	SET IDENTITY_INSERT [Booking].[Category] OFF

	SET IDENTITY_INSERT [Booking].[Service] ON
	INSERT [Booking].[Service] ([Id], [CategoryId], [CurrencyId], [Name], [Price], [Image], [Description], [SortOrder], [Enabled], [Duration]) VALUES (1, 1, @CurrencyID, N'Наименование услуги 1', 990, N'', NULL, 10, 1, 18000000000)
	INSERT [Booking].[Service] ([Id], [CategoryId], [CurrencyId], [Name], [Price], [Image], [Description], [SortOrder], [Enabled], [Duration]) VALUES (2, 1, @CurrencyID, N'Наименование услуги 2', 1990, N'', NULL, 20, 1, 36000000000)
	INSERT [Booking].[Service] ([Id], [CategoryId], [CurrencyId], [Name], [Price], [Image], [Description], [SortOrder], [Enabled], [Duration]) VALUES (3, 1, @CurrencyID, N'Наименование услуги 3', 2990, N'', NULL, 30, 1, 18000000000)
	INSERT [Booking].[Service] ([Id], [CategoryId], [CurrencyId], [Name], [Price], [Image], [Description], [SortOrder], [Enabled], [Duration]) VALUES (4, 2, @CurrencyID, N'Наименование услуги 4', 450, N'', NULL, 10, 1, 18000000000)
	INSERT [Booking].[Service] ([Id], [CategoryId], [CurrencyId], [Name], [Price], [Image], [Description], [SortOrder], [Enabled], [Duration]) VALUES (5, 2, @CurrencyID, N'Наименование услуги 5', 750, NULL, NULL, 20, 1, 18000000000)
	INSERT [Booking].[Service] ([Id], [CategoryId], [CurrencyId], [Name], [Price], [Image], [Description], [SortOrder], [Enabled], [Duration]) VALUES (6, 2, @CurrencyID, N'Наименование услуги 6', 950, NULL, NULL, 30, 1, 18000000000)
	SET IDENTITY_INSERT [Booking].[Service] OFF

	SET IDENTITY_INSERT [Booking].[Affiliate] ON
	INSERT [Booking].[Affiliate] ([Id], [CityId], [Name], [Description], [Address], [Phone], [BookingIntervalMinutes], [SortOrder], [Enabled], [AccessForAll]) VALUES (1, NULL, N'Основной филиал', NULL, NULL, NULL, 30, 0, 1, 1)
	SET IDENTITY_INSERT [Booking].[Affiliate] OFF

	INSERT [Booking].[AffiliateCategory] ([AffiliateId], [CategoryId]) VALUES (1, 1)
	INSERT [Booking].[AffiliateCategory] ([AffiliateId], [CategoryId]) VALUES (1, 2)

	SET IDENTITY_INSERT [Booking].[AffiliateTimeOfBooking] ON
	INSERT [Booking].[AffiliateTimeOfBooking] ([Id], [AffiliateId], [DayOfWeek], [StartTime], [EndTime]) VALUES (1, 1, 1, CAST(0x00008EAC009450C0 AS DateTime), CAST(0x00008EAC009C8E20 AS DateTime))
	INSERT [Booking].[AffiliateTimeOfBooking] ([Id], [AffiliateId], [DayOfWeek], [StartTime], [EndTime]) VALUES (2, 1, 1, CAST(0x00008EAC009C8E20 AS DateTime), CAST(0x00008EAC00A4CB80 AS DateTime))
	INSERT [Booking].[AffiliateTimeOfBooking] ([Id], [AffiliateId], [DayOfWeek], [StartTime], [EndTime]) VALUES (3, 1, 1, CAST(0x00008EAC00A4CB80 AS DateTime), CAST(0x00008EAC00AD08E0 AS DateTime))
	INSERT [Booking].[AffiliateTimeOfBooking] ([Id], [AffiliateId], [DayOfWeek], [StartTime], [EndTime]) VALUES (4, 1, 1, CAST(0x00008EAC00AD08E0 AS DateTime), CAST(0x00008EAC00B54640 AS DateTime))
	INSERT [Booking].[AffiliateTimeOfBooking] ([Id], [AffiliateId], [DayOfWeek], [StartTime], [EndTime]) VALUES (5, 1, 1, CAST(0x00008EAC00B54640 AS DateTime), CAST(0x00008EAC00BD83A0 AS DateTime))
	INSERT [Booking].[AffiliateTimeOfBooking] ([Id], [AffiliateId], [DayOfWeek], [StartTime], [EndTime]) VALUES (6, 1, 1, CAST(0x00008EAC00BD83A0 AS DateTime), CAST(0x00008EAC00C5C100 AS DateTime))
	INSERT [Booking].[AffiliateTimeOfBooking] ([Id], [AffiliateId], [DayOfWeek], [StartTime], [EndTime]) VALUES (7, 1, 1, CAST(0x00008EAC00C5C100 AS DateTime), CAST(0x00008EAC00CDFE60 AS DateTime))
	INSERT [Booking].[AffiliateTimeOfBooking] ([Id], [AffiliateId], [DayOfWeek], [StartTime], [EndTime]) VALUES (8, 1, 1, CAST(0x00008EAC00CDFE60 AS DateTime), CAST(0x00008EAC00D63BC0 AS DateTime))
	INSERT [Booking].[AffiliateTimeOfBooking] ([Id], [AffiliateId], [DayOfWeek], [StartTime], [EndTime]) VALUES (9, 1, 1, CAST(0x00008EAC00D63BC0 AS DateTime), CAST(0x00008EAC00DE7920 AS DateTime))
	INSERT [Booking].[AffiliateTimeOfBooking] ([Id], [AffiliateId], [DayOfWeek], [StartTime], [EndTime]) VALUES (10, 1, 1, CAST(0x00008EAC00DE7920 AS DateTime), CAST(0x00008EAC00E6B680 AS DateTime))
	INSERT [Booking].[AffiliateTimeOfBooking] ([Id], [AffiliateId], [DayOfWeek], [StartTime], [EndTime]) VALUES (11, 1, 1, CAST(0x00008EAC00E6B680 AS DateTime), CAST(0x00008EAC00EEF3E0 AS DateTime))
	INSERT [Booking].[AffiliateTimeOfBooking] ([Id], [AffiliateId], [DayOfWeek], [StartTime], [EndTime]) VALUES (12, 1, 1, CAST(0x00008EAC00EEF3E0 AS DateTime), CAST(0x00008EAC00F73140 AS DateTime))
	INSERT [Booking].[AffiliateTimeOfBooking] ([Id], [AffiliateId], [DayOfWeek], [StartTime], [EndTime]) VALUES (13, 1, 1, CAST(0x00008EAC00F73140 AS DateTime), CAST(0x00008EAC00FF6EA0 AS DateTime))
	INSERT [Booking].[AffiliateTimeOfBooking] ([Id], [AffiliateId], [DayOfWeek], [StartTime], [EndTime]) VALUES (14, 1, 1, CAST(0x00008EAC00FF6EA0 AS DateTime), CAST(0x00008EAC0107AC00 AS DateTime))
	INSERT [Booking].[AffiliateTimeOfBooking] ([Id], [AffiliateId], [DayOfWeek], [StartTime], [EndTime]) VALUES (15, 1, 1, CAST(0x00008EAC0107AC00 AS DateTime), CAST(0x00008EAC010FE960 AS DateTime))
	INSERT [Booking].[AffiliateTimeOfBooking] ([Id], [AffiliateId], [DayOfWeek], [StartTime], [EndTime]) VALUES (16, 1, 1, CAST(0x00008EAC010FE960 AS DateTime), CAST(0x00008EAC011826C0 AS DateTime))
	INSERT [Booking].[AffiliateTimeOfBooking] ([Id], [AffiliateId], [DayOfWeek], [StartTime], [EndTime]) VALUES (17, 1, 1, CAST(0x00008EAC011826C0 AS DateTime), CAST(0x00008EAC01206420 AS DateTime))
	INSERT [Booking].[AffiliateTimeOfBooking] ([Id], [AffiliateId], [DayOfWeek], [StartTime], [EndTime]) VALUES (18, 1, 1, CAST(0x00008EAC01206420 AS DateTime), CAST(0x00008EAC0128A180 AS DateTime))
	INSERT [Booking].[AffiliateTimeOfBooking] ([Id], [AffiliateId], [DayOfWeek], [StartTime], [EndTime]) VALUES (19, 1, 1, CAST(0x00008EAC0128A180 AS DateTime), CAST(0x00008EAC0130DEE0 AS DateTime))
	INSERT [Booking].[AffiliateTimeOfBooking] ([Id], [AffiliateId], [DayOfWeek], [StartTime], [EndTime]) VALUES (20, 1, 1, CAST(0x00008EAC0130DEE0 AS DateTime), CAST(0x00008EAC01391C40 AS DateTime))
	INSERT [Booking].[AffiliateTimeOfBooking] ([Id], [AffiliateId], [DayOfWeek], [StartTime], [EndTime]) VALUES (21, 1, 1, CAST(0x00008EAC01391C40 AS DateTime), CAST(0x00008EAC014159A0 AS DateTime))
	INSERT [Booking].[AffiliateTimeOfBooking] ([Id], [AffiliateId], [DayOfWeek], [StartTime], [EndTime]) VALUES (22, 1, 1, CAST(0x00008EAC014159A0 AS DateTime), CAST(0x00008EAC01499700 AS DateTime))
	INSERT [Booking].[AffiliateTimeOfBooking] ([Id], [AffiliateId], [DayOfWeek], [StartTime], [EndTime]) VALUES (23, 1, 1, CAST(0x00008EAC01499700 AS DateTime), CAST(0x00008EAC0151D460 AS DateTime))
	INSERT [Booking].[AffiliateTimeOfBooking] ([Id], [AffiliateId], [DayOfWeek], [StartTime], [EndTime]) VALUES (24, 1, 1, CAST(0x00008EAC0151D460 AS DateTime), CAST(0x00008EAC015A11C0 AS DateTime))
	INSERT [Booking].[AffiliateTimeOfBooking] ([Id], [AffiliateId], [DayOfWeek], [StartTime], [EndTime]) VALUES (25, 1, 2, CAST(0x00008EAC009450C0 AS DateTime), CAST(0x00008EAC009C8E20 AS DateTime))
	INSERT [Booking].[AffiliateTimeOfBooking] ([Id], [AffiliateId], [DayOfWeek], [StartTime], [EndTime]) VALUES (26, 1, 2, CAST(0x00008EAC009C8E20 AS DateTime), CAST(0x00008EAC00A4CB80 AS DateTime))
	INSERT [Booking].[AffiliateTimeOfBooking] ([Id], [AffiliateId], [DayOfWeek], [StartTime], [EndTime]) VALUES (27, 1, 2, CAST(0x00008EAC00A4CB80 AS DateTime), CAST(0x00008EAC00AD08E0 AS DateTime))
	INSERT [Booking].[AffiliateTimeOfBooking] ([Id], [AffiliateId], [DayOfWeek], [StartTime], [EndTime]) VALUES (28, 1, 2, CAST(0x00008EAC00AD08E0 AS DateTime), CAST(0x00008EAC00B54640 AS DateTime))
	INSERT [Booking].[AffiliateTimeOfBooking] ([Id], [AffiliateId], [DayOfWeek], [StartTime], [EndTime]) VALUES (29, 1, 2, CAST(0x00008EAC00B54640 AS DateTime), CAST(0x00008EAC00BD83A0 AS DateTime))
	INSERT [Booking].[AffiliateTimeOfBooking] ([Id], [AffiliateId], [DayOfWeek], [StartTime], [EndTime]) VALUES (30, 1, 2, CAST(0x00008EAC00BD83A0 AS DateTime), CAST(0x00008EAC00C5C100 AS DateTime))
	INSERT [Booking].[AffiliateTimeOfBooking] ([Id], [AffiliateId], [DayOfWeek], [StartTime], [EndTime]) VALUES (31, 1, 2, CAST(0x00008EAC00C5C100 AS DateTime), CAST(0x00008EAC00CDFE60 AS DateTime))
	INSERT [Booking].[AffiliateTimeOfBooking] ([Id], [AffiliateId], [DayOfWeek], [StartTime], [EndTime]) VALUES (32, 1, 2, CAST(0x00008EAC00CDFE60 AS DateTime), CAST(0x00008EAC00D63BC0 AS DateTime))
	INSERT [Booking].[AffiliateTimeOfBooking] ([Id], [AffiliateId], [DayOfWeek], [StartTime], [EndTime]) VALUES (33, 1, 2, CAST(0x00008EAC00E6B680 AS DateTime), CAST(0x00008EAC00EEF3E0 AS DateTime))
	INSERT [Booking].[AffiliateTimeOfBooking] ([Id], [AffiliateId], [DayOfWeek], [StartTime], [EndTime]) VALUES (34, 1, 2, CAST(0x00008EAC00DE7920 AS DateTime), CAST(0x00008EAC00E6B680 AS DateTime))
	INSERT [Booking].[AffiliateTimeOfBooking] ([Id], [AffiliateId], [DayOfWeek], [StartTime], [EndTime]) VALUES (35, 1, 2, CAST(0x00008EAC00D63BC0 AS DateTime), CAST(0x00008EAC00DE7920 AS DateTime))
	INSERT [Booking].[AffiliateTimeOfBooking] ([Id], [AffiliateId], [DayOfWeek], [StartTime], [EndTime]) VALUES (36, 1, 2, CAST(0x00008EAC00EEF3E0 AS DateTime), CAST(0x00008EAC00F73140 AS DateTime))
	INSERT [Booking].[AffiliateTimeOfBooking] ([Id], [AffiliateId], [DayOfWeek], [StartTime], [EndTime]) VALUES (37, 1, 2, CAST(0x00008EAC00F73140 AS DateTime), CAST(0x00008EAC00FF6EA0 AS DateTime))
	INSERT [Booking].[AffiliateTimeOfBooking] ([Id], [AffiliateId], [DayOfWeek], [StartTime], [EndTime]) VALUES (38, 1, 2, CAST(0x00008EAC00FF6EA0 AS DateTime), CAST(0x00008EAC0107AC00 AS DateTime))
	INSERT [Booking].[AffiliateTimeOfBooking] ([Id], [AffiliateId], [DayOfWeek], [StartTime], [EndTime]) VALUES (39, 1, 2, CAST(0x00008EAC0107AC00 AS DateTime), CAST(0x00008EAC010FE960 AS DateTime))
	INSERT [Booking].[AffiliateTimeOfBooking] ([Id], [AffiliateId], [DayOfWeek], [StartTime], [EndTime]) VALUES (40, 1, 2, CAST(0x00008EAC010FE960 AS DateTime), CAST(0x00008EAC011826C0 AS DateTime))
	INSERT [Booking].[AffiliateTimeOfBooking] ([Id], [AffiliateId], [DayOfWeek], [StartTime], [EndTime]) VALUES (41, 1, 2, CAST(0x00008EAC011826C0 AS DateTime), CAST(0x00008EAC01206420 AS DateTime))
	INSERT [Booking].[AffiliateTimeOfBooking] ([Id], [AffiliateId], [DayOfWeek], [StartTime], [EndTime]) VALUES (42, 1, 2, CAST(0x00008EAC01206420 AS DateTime), CAST(0x00008EAC0128A180 AS DateTime))
	INSERT [Booking].[AffiliateTimeOfBooking] ([Id], [AffiliateId], [DayOfWeek], [StartTime], [EndTime]) VALUES (43, 1, 2, CAST(0x00008EAC0128A180 AS DateTime), CAST(0x00008EAC0130DEE0 AS DateTime))
	INSERT [Booking].[AffiliateTimeOfBooking] ([Id], [AffiliateId], [DayOfWeek], [StartTime], [EndTime]) VALUES (44, 1, 2, CAST(0x00008EAC0130DEE0 AS DateTime), CAST(0x00008EAC01391C40 AS DateTime))
	INSERT [Booking].[AffiliateTimeOfBooking] ([Id], [AffiliateId], [DayOfWeek], [StartTime], [EndTime]) VALUES (45, 1, 2, CAST(0x00008EAC01391C40 AS DateTime), CAST(0x00008EAC014159A0 AS DateTime))
	INSERT [Booking].[AffiliateTimeOfBooking] ([Id], [AffiliateId], [DayOfWeek], [StartTime], [EndTime]) VALUES (46, 1, 2, CAST(0x00008EAC014159A0 AS DateTime), CAST(0x00008EAC01499700 AS DateTime))
	INSERT [Booking].[AffiliateTimeOfBooking] ([Id], [AffiliateId], [DayOfWeek], [StartTime], [EndTime]) VALUES (47, 1, 2, CAST(0x00008EAC01499700 AS DateTime), CAST(0x00008EAC0151D460 AS DateTime))
	INSERT [Booking].[AffiliateTimeOfBooking] ([Id], [AffiliateId], [DayOfWeek], [StartTime], [EndTime]) VALUES (48, 1, 2, CAST(0x00008EAC0151D460 AS DateTime), CAST(0x00008EAC015A11C0 AS DateTime))
	INSERT [Booking].[AffiliateTimeOfBooking] ([Id], [AffiliateId], [DayOfWeek], [StartTime], [EndTime]) VALUES (49, 1, 3, CAST(0x00008EAC009450C0 AS DateTime), CAST(0x00008EAC009C8E20 AS DateTime))
	INSERT [Booking].[AffiliateTimeOfBooking] ([Id], [AffiliateId], [DayOfWeek], [StartTime], [EndTime]) VALUES (50, 1, 3, CAST(0x00008EAC009C8E20 AS DateTime), CAST(0x00008EAC00A4CB80 AS DateTime))
	INSERT [Booking].[AffiliateTimeOfBooking] ([Id], [AffiliateId], [DayOfWeek], [StartTime], [EndTime]) VALUES (51, 1, 3, CAST(0x00008EAC00A4CB80 AS DateTime), CAST(0x00008EAC00AD08E0 AS DateTime))
	INSERT [Booking].[AffiliateTimeOfBooking] ([Id], [AffiliateId], [DayOfWeek], [StartTime], [EndTime]) VALUES (52, 1, 3, CAST(0x00008EAC00AD08E0 AS DateTime), CAST(0x00008EAC00B54640 AS DateTime))
	INSERT [Booking].[AffiliateTimeOfBooking] ([Id], [AffiliateId], [DayOfWeek], [StartTime], [EndTime]) VALUES (53, 1, 3, CAST(0x00008EAC00B54640 AS DateTime), CAST(0x00008EAC00BD83A0 AS DateTime))
	INSERT [Booking].[AffiliateTimeOfBooking] ([Id], [AffiliateId], [DayOfWeek], [StartTime], [EndTime]) VALUES (54, 1, 3, CAST(0x00008EAC00BD83A0 AS DateTime), CAST(0x00008EAC00C5C100 AS DateTime))
	INSERT [Booking].[AffiliateTimeOfBooking] ([Id], [AffiliateId], [DayOfWeek], [StartTime], [EndTime]) VALUES (55, 1, 3, CAST(0x00008EAC00C5C100 AS DateTime), CAST(0x00008EAC00CDFE60 AS DateTime))
	INSERT [Booking].[AffiliateTimeOfBooking] ([Id], [AffiliateId], [DayOfWeek], [StartTime], [EndTime]) VALUES (56, 1, 3, CAST(0x00008EAC00CDFE60 AS DateTime), CAST(0x00008EAC00D63BC0 AS DateTime))
	INSERT [Booking].[AffiliateTimeOfBooking] ([Id], [AffiliateId], [DayOfWeek], [StartTime], [EndTime]) VALUES (57, 1, 3, CAST(0x00008EAC00D63BC0 AS DateTime), CAST(0x00008EAC00DE7920 AS DateTime))
	INSERT [Booking].[AffiliateTimeOfBooking] ([Id], [AffiliateId], [DayOfWeek], [StartTime], [EndTime]) VALUES (58, 1, 3, CAST(0x00008EAC00DE7920 AS DateTime), CAST(0x00008EAC00E6B680 AS DateTime))
	INSERT [Booking].[AffiliateTimeOfBooking] ([Id], [AffiliateId], [DayOfWeek], [StartTime], [EndTime]) VALUES (59, 1, 3, CAST(0x00008EAC00E6B680 AS DateTime), CAST(0x00008EAC00EEF3E0 AS DateTime))
	INSERT [Booking].[AffiliateTimeOfBooking] ([Id], [AffiliateId], [DayOfWeek], [StartTime], [EndTime]) VALUES (60, 1, 3, CAST(0x00008EAC00EEF3E0 AS DateTime), CAST(0x00008EAC00F73140 AS DateTime))
	INSERT [Booking].[AffiliateTimeOfBooking] ([Id], [AffiliateId], [DayOfWeek], [StartTime], [EndTime]) VALUES (61, 1, 3, CAST(0x00008EAC00F73140 AS DateTime), CAST(0x00008EAC00FF6EA0 AS DateTime))
	INSERT [Booking].[AffiliateTimeOfBooking] ([Id], [AffiliateId], [DayOfWeek], [StartTime], [EndTime]) VALUES (62, 1, 3, CAST(0x00008EAC00FF6EA0 AS DateTime), CAST(0x00008EAC0107AC00 AS DateTime))
	INSERT [Booking].[AffiliateTimeOfBooking] ([Id], [AffiliateId], [DayOfWeek], [StartTime], [EndTime]) VALUES (63, 1, 3, CAST(0x00008EAC0107AC00 AS DateTime), CAST(0x00008EAC010FE960 AS DateTime))
	INSERT [Booking].[AffiliateTimeOfBooking] ([Id], [AffiliateId], [DayOfWeek], [StartTime], [EndTime]) VALUES (64, 1, 3, CAST(0x00008EAC010FE960 AS DateTime), CAST(0x00008EAC011826C0 AS DateTime))
	INSERT [Booking].[AffiliateTimeOfBooking] ([Id], [AffiliateId], [DayOfWeek], [StartTime], [EndTime]) VALUES (65, 1, 3, CAST(0x00008EAC011826C0 AS DateTime), CAST(0x00008EAC01206420 AS DateTime))
	INSERT [Booking].[AffiliateTimeOfBooking] ([Id], [AffiliateId], [DayOfWeek], [StartTime], [EndTime]) VALUES (66, 1, 3, CAST(0x00008EAC01206420 AS DateTime), CAST(0x00008EAC0128A180 AS DateTime))
	INSERT [Booking].[AffiliateTimeOfBooking] ([Id], [AffiliateId], [DayOfWeek], [StartTime], [EndTime]) VALUES (67, 1, 3, CAST(0x00008EAC0128A180 AS DateTime), CAST(0x00008EAC0130DEE0 AS DateTime))
	INSERT [Booking].[AffiliateTimeOfBooking] ([Id], [AffiliateId], [DayOfWeek], [StartTime], [EndTime]) VALUES (68, 1, 3, CAST(0x00008EAC0130DEE0 AS DateTime), CAST(0x00008EAC01391C40 AS DateTime))
	INSERT [Booking].[AffiliateTimeOfBooking] ([Id], [AffiliateId], [DayOfWeek], [StartTime], [EndTime]) VALUES (69, 1, 3, CAST(0x00008EAC01391C40 AS DateTime), CAST(0x00008EAC014159A0 AS DateTime))
	INSERT [Booking].[AffiliateTimeOfBooking] ([Id], [AffiliateId], [DayOfWeek], [StartTime], [EndTime]) VALUES (70, 1, 3, CAST(0x00008EAC014159A0 AS DateTime), CAST(0x00008EAC01499700 AS DateTime))
	INSERT [Booking].[AffiliateTimeOfBooking] ([Id], [AffiliateId], [DayOfWeek], [StartTime], [EndTime]) VALUES (71, 1, 3, CAST(0x00008EAC01499700 AS DateTime), CAST(0x00008EAC0151D460 AS DateTime))
	INSERT [Booking].[AffiliateTimeOfBooking] ([Id], [AffiliateId], [DayOfWeek], [StartTime], [EndTime]) VALUES (72, 1, 3, CAST(0x00008EAC0151D460 AS DateTime), CAST(0x00008EAC015A11C0 AS DateTime))
	INSERT [Booking].[AffiliateTimeOfBooking] ([Id], [AffiliateId], [DayOfWeek], [StartTime], [EndTime]) VALUES (73, 1, 4, CAST(0x00008EAC009450C0 AS DateTime), CAST(0x00008EAC009C8E20 AS DateTime))
	INSERT [Booking].[AffiliateTimeOfBooking] ([Id], [AffiliateId], [DayOfWeek], [StartTime], [EndTime]) VALUES (74, 1, 4, CAST(0x00008EAC009C8E20 AS DateTime), CAST(0x00008EAC00A4CB80 AS DateTime))
	INSERT [Booking].[AffiliateTimeOfBooking] ([Id], [AffiliateId], [DayOfWeek], [StartTime], [EndTime]) VALUES (75, 1, 4, CAST(0x00008EAC00A4CB80 AS DateTime), CAST(0x00008EAC00AD08E0 AS DateTime))
	INSERT [Booking].[AffiliateTimeOfBooking] ([Id], [AffiliateId], [DayOfWeek], [StartTime], [EndTime]) VALUES (76, 1, 4, CAST(0x00008EAC00AD08E0 AS DateTime), CAST(0x00008EAC00B54640 AS DateTime))
	INSERT [Booking].[AffiliateTimeOfBooking] ([Id], [AffiliateId], [DayOfWeek], [StartTime], [EndTime]) VALUES (77, 1, 4, CAST(0x00008EAC00B54640 AS DateTime), CAST(0x00008EAC00BD83A0 AS DateTime))
	INSERT [Booking].[AffiliateTimeOfBooking] ([Id], [AffiliateId], [DayOfWeek], [StartTime], [EndTime]) VALUES (78, 1, 4, CAST(0x00008EAC00BD83A0 AS DateTime), CAST(0x00008EAC00C5C100 AS DateTime))
	INSERT [Booking].[AffiliateTimeOfBooking] ([Id], [AffiliateId], [DayOfWeek], [StartTime], [EndTime]) VALUES (79, 1, 4, CAST(0x00008EAC00C5C100 AS DateTime), CAST(0x00008EAC00CDFE60 AS DateTime))
	INSERT [Booking].[AffiliateTimeOfBooking] ([Id], [AffiliateId], [DayOfWeek], [StartTime], [EndTime]) VALUES (80, 1, 4, CAST(0x00008EAC00CDFE60 AS DateTime), CAST(0x00008EAC00D63BC0 AS DateTime))
	INSERT [Booking].[AffiliateTimeOfBooking] ([Id], [AffiliateId], [DayOfWeek], [StartTime], [EndTime]) VALUES (81, 1, 4, CAST(0x00008EAC00D63BC0 AS DateTime), CAST(0x00008EAC00DE7920 AS DateTime))
	INSERT [Booking].[AffiliateTimeOfBooking] ([Id], [AffiliateId], [DayOfWeek], [StartTime], [EndTime]) VALUES (82, 1, 4, CAST(0x00008EAC00DE7920 AS DateTime), CAST(0x00008EAC00E6B680 AS DateTime))
	INSERT [Booking].[AffiliateTimeOfBooking] ([Id], [AffiliateId], [DayOfWeek], [StartTime], [EndTime]) VALUES (83, 1, 4, CAST(0x00008EAC00E6B680 AS DateTime), CAST(0x00008EAC00EEF3E0 AS DateTime))
	INSERT [Booking].[AffiliateTimeOfBooking] ([Id], [AffiliateId], [DayOfWeek], [StartTime], [EndTime]) VALUES (84, 1, 4, CAST(0x00008EAC00EEF3E0 AS DateTime), CAST(0x00008EAC00F73140 AS DateTime))
	INSERT [Booking].[AffiliateTimeOfBooking] ([Id], [AffiliateId], [DayOfWeek], [StartTime], [EndTime]) VALUES (85, 1, 4, CAST(0x00008EAC00F73140 AS DateTime), CAST(0x00008EAC00FF6EA0 AS DateTime))
	INSERT [Booking].[AffiliateTimeOfBooking] ([Id], [AffiliateId], [DayOfWeek], [StartTime], [EndTime]) VALUES (86, 1, 4, CAST(0x00008EAC00FF6EA0 AS DateTime), CAST(0x00008EAC0107AC00 AS DateTime))
	INSERT [Booking].[AffiliateTimeOfBooking] ([Id], [AffiliateId], [DayOfWeek], [StartTime], [EndTime]) VALUES (87, 1, 4, CAST(0x00008EAC0107AC00 AS DateTime), CAST(0x00008EAC010FE960 AS DateTime))
	INSERT [Booking].[AffiliateTimeOfBooking] ([Id], [AffiliateId], [DayOfWeek], [StartTime], [EndTime]) VALUES (88, 1, 4, CAST(0x00008EAC010FE960 AS DateTime), CAST(0x00008EAC011826C0 AS DateTime))
	INSERT [Booking].[AffiliateTimeOfBooking] ([Id], [AffiliateId], [DayOfWeek], [StartTime], [EndTime]) VALUES (89, 1, 4, CAST(0x00008EAC011826C0 AS DateTime), CAST(0x00008EAC01206420 AS DateTime))
	INSERT [Booking].[AffiliateTimeOfBooking] ([Id], [AffiliateId], [DayOfWeek], [StartTime], [EndTime]) VALUES (90, 1, 4, CAST(0x00008EAC01206420 AS DateTime), CAST(0x00008EAC0128A180 AS DateTime))
	INSERT [Booking].[AffiliateTimeOfBooking] ([Id], [AffiliateId], [DayOfWeek], [StartTime], [EndTime]) VALUES (91, 1, 4, CAST(0x00008EAC0128A180 AS DateTime), CAST(0x00008EAC0130DEE0 AS DateTime))
	INSERT [Booking].[AffiliateTimeOfBooking] ([Id], [AffiliateId], [DayOfWeek], [StartTime], [EndTime]) VALUES (92, 1, 4, CAST(0x00008EAC0130DEE0 AS DateTime), CAST(0x00008EAC01391C40 AS DateTime))
	INSERT [Booking].[AffiliateTimeOfBooking] ([Id], [AffiliateId], [DayOfWeek], [StartTime], [EndTime]) VALUES (93, 1, 4, CAST(0x00008EAC01391C40 AS DateTime), CAST(0x00008EAC014159A0 AS DateTime))
	INSERT [Booking].[AffiliateTimeOfBooking] ([Id], [AffiliateId], [DayOfWeek], [StartTime], [EndTime]) VALUES (94, 1, 4, CAST(0x00008EAC014159A0 AS DateTime), CAST(0x00008EAC01499700 AS DateTime))
	INSERT [Booking].[AffiliateTimeOfBooking] ([Id], [AffiliateId], [DayOfWeek], [StartTime], [EndTime]) VALUES (95, 1, 4, CAST(0x00008EAC01499700 AS DateTime), CAST(0x00008EAC0151D460 AS DateTime))
	INSERT [Booking].[AffiliateTimeOfBooking] ([Id], [AffiliateId], [DayOfWeek], [StartTime], [EndTime]) VALUES (96, 1, 4, CAST(0x00008EAC0151D460 AS DateTime), CAST(0x00008EAC015A11C0 AS DateTime))
	INSERT [Booking].[AffiliateTimeOfBooking] ([Id], [AffiliateId], [DayOfWeek], [StartTime], [EndTime]) VALUES (97, 1, 5, CAST(0x00008EAC009450C0 AS DateTime), CAST(0x00008EAC009C8E20 AS DateTime))
	INSERT [Booking].[AffiliateTimeOfBooking] ([Id], [AffiliateId], [DayOfWeek], [StartTime], [EndTime]) VALUES (98, 1, 5, CAST(0x00008EAC009C8E20 AS DateTime), CAST(0x00008EAC00A4CB80 AS DateTime))
	INSERT [Booking].[AffiliateTimeOfBooking] ([Id], [AffiliateId], [DayOfWeek], [StartTime], [EndTime]) VALUES (99, 1, 5, CAST(0x00008EAC00A4CB80 AS DateTime), CAST(0x00008EAC00AD08E0 AS DateTime))
	INSERT [Booking].[AffiliateTimeOfBooking] ([Id], [AffiliateId], [DayOfWeek], [StartTime], [EndTime]) VALUES (100, 1, 5, CAST(0x00008EAC00AD08E0 AS DateTime), CAST(0x00008EAC00B54640 AS DateTime))
	INSERT [Booking].[AffiliateTimeOfBooking] ([Id], [AffiliateId], [DayOfWeek], [StartTime], [EndTime]) VALUES (101, 1, 5, CAST(0x00008EAC00B54640 AS DateTime), CAST(0x00008EAC00BD83A0 AS DateTime))
	INSERT [Booking].[AffiliateTimeOfBooking] ([Id], [AffiliateId], [DayOfWeek], [StartTime], [EndTime]) VALUES (102, 1, 5, CAST(0x00008EAC00BD83A0 AS DateTime), CAST(0x00008EAC00C5C100 AS DateTime))
	INSERT [Booking].[AffiliateTimeOfBooking] ([Id], [AffiliateId], [DayOfWeek], [StartTime], [EndTime]) VALUES (103, 1, 5, CAST(0x00008EAC00C5C100 AS DateTime), CAST(0x00008EAC00CDFE60 AS DateTime))
	INSERT [Booking].[AffiliateTimeOfBooking] ([Id], [AffiliateId], [DayOfWeek], [StartTime], [EndTime]) VALUES (104, 1, 5, CAST(0x00008EAC00CDFE60 AS DateTime), CAST(0x00008EAC00D63BC0 AS DateTime))
	INSERT [Booking].[AffiliateTimeOfBooking] ([Id], [AffiliateId], [DayOfWeek], [StartTime], [EndTime]) VALUES (105, 1, 5, CAST(0x00008EAC00D63BC0 AS DateTime), CAST(0x00008EAC00DE7920 AS DateTime))
	INSERT [Booking].[AffiliateTimeOfBooking] ([Id], [AffiliateId], [DayOfWeek], [StartTime], [EndTime]) VALUES (106, 1, 5, CAST(0x00008EAC00DE7920 AS DateTime), CAST(0x00008EAC00E6B680 AS DateTime))
	INSERT [Booking].[AffiliateTimeOfBooking] ([Id], [AffiliateId], [DayOfWeek], [StartTime], [EndTime]) VALUES (107, 1, 5, CAST(0x00008EAC00E6B680 AS DateTime), CAST(0x00008EAC00EEF3E0 AS DateTime))
	INSERT [Booking].[AffiliateTimeOfBooking] ([Id], [AffiliateId], [DayOfWeek], [StartTime], [EndTime]) VALUES (108, 1, 5, CAST(0x00008EAC00EEF3E0 AS DateTime), CAST(0x00008EAC00F73140 AS DateTime))
	INSERT [Booking].[AffiliateTimeOfBooking] ([Id], [AffiliateId], [DayOfWeek], [StartTime], [EndTime]) VALUES (109, 1, 5, CAST(0x00008EAC00F73140 AS DateTime), CAST(0x00008EAC00FF6EA0 AS DateTime))
	INSERT [Booking].[AffiliateTimeOfBooking] ([Id], [AffiliateId], [DayOfWeek], [StartTime], [EndTime]) VALUES (110, 1, 5, CAST(0x00008EAC00FF6EA0 AS DateTime), CAST(0x00008EAC0107AC00 AS DateTime))
	INSERT [Booking].[AffiliateTimeOfBooking] ([Id], [AffiliateId], [DayOfWeek], [StartTime], [EndTime]) VALUES (111, 1, 5, CAST(0x00008EAC0107AC00 AS DateTime), CAST(0x00008EAC010FE960 AS DateTime))
	INSERT [Booking].[AffiliateTimeOfBooking] ([Id], [AffiliateId], [DayOfWeek], [StartTime], [EndTime]) VALUES (112, 1, 5, CAST(0x00008EAC010FE960 AS DateTime), CAST(0x00008EAC011826C0 AS DateTime))
	INSERT [Booking].[AffiliateTimeOfBooking] ([Id], [AffiliateId], [DayOfWeek], [StartTime], [EndTime]) VALUES (113, 1, 5, CAST(0x00008EAC011826C0 AS DateTime), CAST(0x00008EAC01206420 AS DateTime))
	INSERT [Booking].[AffiliateTimeOfBooking] ([Id], [AffiliateId], [DayOfWeek], [StartTime], [EndTime]) VALUES (114, 1, 5, CAST(0x00008EAC01206420 AS DateTime), CAST(0x00008EAC0128A180 AS DateTime))
	INSERT [Booking].[AffiliateTimeOfBooking] ([Id], [AffiliateId], [DayOfWeek], [StartTime], [EndTime]) VALUES (115, 1, 5, CAST(0x00008EAC0128A180 AS DateTime), CAST(0x00008EAC0130DEE0 AS DateTime))
	INSERT [Booking].[AffiliateTimeOfBooking] ([Id], [AffiliateId], [DayOfWeek], [StartTime], [EndTime]) VALUES (116, 1, 5, CAST(0x00008EAC0130DEE0 AS DateTime), CAST(0x00008EAC01391C40 AS DateTime))
	INSERT [Booking].[AffiliateTimeOfBooking] ([Id], [AffiliateId], [DayOfWeek], [StartTime], [EndTime]) VALUES (117, 1, 5, CAST(0x00008EAC01391C40 AS DateTime), CAST(0x00008EAC014159A0 AS DateTime))
	INSERT [Booking].[AffiliateTimeOfBooking] ([Id], [AffiliateId], [DayOfWeek], [StartTime], [EndTime]) VALUES (118, 1, 5, CAST(0x00008EAC014159A0 AS DateTime), CAST(0x00008EAC01499700 AS DateTime))
	INSERT [Booking].[AffiliateTimeOfBooking] ([Id], [AffiliateId], [DayOfWeek], [StartTime], [EndTime]) VALUES (119, 1, 5, CAST(0x00008EAC01499700 AS DateTime), CAST(0x00008EAC0151D460 AS DateTime))
	INSERT [Booking].[AffiliateTimeOfBooking] ([Id], [AffiliateId], [DayOfWeek], [StartTime], [EndTime]) VALUES (120, 1, 5, CAST(0x00008EAC0151D460 AS DateTime), CAST(0x00008EAC015A11C0 AS DateTime))
	INSERT [Booking].[AffiliateTimeOfBooking] ([Id], [AffiliateId], [DayOfWeek], [StartTime], [EndTime]) VALUES (121, 1, 6, CAST(0x00008EAC009450C0 AS DateTime), CAST(0x00008EAC009C8E20 AS DateTime))
	INSERT [Booking].[AffiliateTimeOfBooking] ([Id], [AffiliateId], [DayOfWeek], [StartTime], [EndTime]) VALUES (122, 1, 6, CAST(0x00008EAC009C8E20 AS DateTime), CAST(0x00008EAC00A4CB80 AS DateTime))
	INSERT [Booking].[AffiliateTimeOfBooking] ([Id], [AffiliateId], [DayOfWeek], [StartTime], [EndTime]) VALUES (123, 1, 6, CAST(0x00008EAC00A4CB80 AS DateTime), CAST(0x00008EAC00AD08E0 AS DateTime))
	INSERT [Booking].[AffiliateTimeOfBooking] ([Id], [AffiliateId], [DayOfWeek], [StartTime], [EndTime]) VALUES (124, 1, 6, CAST(0x00008EAC00AD08E0 AS DateTime), CAST(0x00008EAC00B54640 AS DateTime))
	INSERT [Booking].[AffiliateTimeOfBooking] ([Id], [AffiliateId], [DayOfWeek], [StartTime], [EndTime]) VALUES (125, 1, 6, CAST(0x00008EAC00B54640 AS DateTime), CAST(0x00008EAC00BD83A0 AS DateTime))
	INSERT [Booking].[AffiliateTimeOfBooking] ([Id], [AffiliateId], [DayOfWeek], [StartTime], [EndTime]) VALUES (126, 1, 6, CAST(0x00008EAC00BD83A0 AS DateTime), CAST(0x00008EAC00C5C100 AS DateTime))
	INSERT [Booking].[AffiliateTimeOfBooking] ([Id], [AffiliateId], [DayOfWeek], [StartTime], [EndTime]) VALUES (127, 1, 6, CAST(0x00008EAC00C5C100 AS DateTime), CAST(0x00008EAC00CDFE60 AS DateTime))
	INSERT [Booking].[AffiliateTimeOfBooking] ([Id], [AffiliateId], [DayOfWeek], [StartTime], [EndTime]) VALUES (128, 1, 6, CAST(0x00008EAC00CDFE60 AS DateTime), CAST(0x00008EAC00D63BC0 AS DateTime))
	INSERT [Booking].[AffiliateTimeOfBooking] ([Id], [AffiliateId], [DayOfWeek], [StartTime], [EndTime]) VALUES (129, 1, 6, CAST(0x00008EAC00D63BC0 AS DateTime), CAST(0x00008EAC00DE7920 AS DateTime))
	INSERT [Booking].[AffiliateTimeOfBooking] ([Id], [AffiliateId], [DayOfWeek], [StartTime], [EndTime]) VALUES (130, 1, 6, CAST(0x00008EAC00DE7920 AS DateTime), CAST(0x00008EAC00E6B680 AS DateTime))
	INSERT [Booking].[AffiliateTimeOfBooking] ([Id], [AffiliateId], [DayOfWeek], [StartTime], [EndTime]) VALUES (131, 1, 6, CAST(0x00008EAC00E6B680 AS DateTime), CAST(0x00008EAC00EEF3E0 AS DateTime))
	INSERT [Booking].[AffiliateTimeOfBooking] ([Id], [AffiliateId], [DayOfWeek], [StartTime], [EndTime]) VALUES (132, 1, 6, CAST(0x00008EAC00EEF3E0 AS DateTime), CAST(0x00008EAC00F73140 AS DateTime))
	INSERT [Booking].[AffiliateTimeOfBooking] ([Id], [AffiliateId], [DayOfWeek], [StartTime], [EndTime]) VALUES (133, 1, 6, CAST(0x00008EAC00F73140 AS DateTime), CAST(0x00008EAC00FF6EA0 AS DateTime))
	INSERT [Booking].[AffiliateTimeOfBooking] ([Id], [AffiliateId], [DayOfWeek], [StartTime], [EndTime]) VALUES (134, 1, 6, CAST(0x00008EAC00FF6EA0 AS DateTime), CAST(0x00008EAC0107AC00 AS DateTime))
	INSERT [Booking].[AffiliateTimeOfBooking] ([Id], [AffiliateId], [DayOfWeek], [StartTime], [EndTime]) VALUES (135, 1, 6, CAST(0x00008EAC0107AC00 AS DateTime), CAST(0x00008EAC010FE960 AS DateTime))
	INSERT [Booking].[AffiliateTimeOfBooking] ([Id], [AffiliateId], [DayOfWeek], [StartTime], [EndTime]) VALUES (136, 1, 6, CAST(0x00008EAC010FE960 AS DateTime), CAST(0x00008EAC011826C0 AS DateTime))
	INSERT [Booking].[AffiliateTimeOfBooking] ([Id], [AffiliateId], [DayOfWeek], [StartTime], [EndTime]) VALUES (137, 1, 6, CAST(0x00008EAC011826C0 AS DateTime), CAST(0x00008EAC01206420 AS DateTime))
	INSERT [Booking].[AffiliateTimeOfBooking] ([Id], [AffiliateId], [DayOfWeek], [StartTime], [EndTime]) VALUES (138, 1, 6, CAST(0x00008EAC01206420 AS DateTime), CAST(0x00008EAC0128A180 AS DateTime))
	INSERT [Booking].[AffiliateTimeOfBooking] ([Id], [AffiliateId], [DayOfWeek], [StartTime], [EndTime]) VALUES (139, 1, 6, CAST(0x00008EAC0128A180 AS DateTime), CAST(0x00008EAC0130DEE0 AS DateTime))
	INSERT [Booking].[AffiliateTimeOfBooking] ([Id], [AffiliateId], [DayOfWeek], [StartTime], [EndTime]) VALUES (140, 1, 6, CAST(0x00008EAC0130DEE0 AS DateTime), CAST(0x00008EAC01391C40 AS DateTime))
	INSERT [Booking].[AffiliateTimeOfBooking] ([Id], [AffiliateId], [DayOfWeek], [StartTime], [EndTime]) VALUES (141, 1, 6, CAST(0x00008EAC01391C40 AS DateTime), CAST(0x00008EAC014159A0 AS DateTime))
	INSERT [Booking].[AffiliateTimeOfBooking] ([Id], [AffiliateId], [DayOfWeek], [StartTime], [EndTime]) VALUES (142, 1, 6, CAST(0x00008EAC014159A0 AS DateTime), CAST(0x00008EAC01499700 AS DateTime))
	INSERT [Booking].[AffiliateTimeOfBooking] ([Id], [AffiliateId], [DayOfWeek], [StartTime], [EndTime]) VALUES (143, 1, 6, CAST(0x00008EAC01499700 AS DateTime), CAST(0x00008EAC0151D460 AS DateTime))
	INSERT [Booking].[AffiliateTimeOfBooking] ([Id], [AffiliateId], [DayOfWeek], [StartTime], [EndTime]) VALUES (144, 1, 6, CAST(0x00008EAC0151D460 AS DateTime), CAST(0x00008EAC015A11C0 AS DateTime))
	INSERT [Booking].[AffiliateTimeOfBooking] ([Id], [AffiliateId], [DayOfWeek], [StartTime], [EndTime]) VALUES (145, 1, 0, CAST(0x00008EAC009450C0 AS DateTime), CAST(0x00008EAC009C8E20 AS DateTime))
	INSERT [Booking].[AffiliateTimeOfBooking] ([Id], [AffiliateId], [DayOfWeek], [StartTime], [EndTime]) VALUES (146, 1, 0, CAST(0x00008EAC009C8E20 AS DateTime), CAST(0x00008EAC00A4CB80 AS DateTime))
	INSERT [Booking].[AffiliateTimeOfBooking] ([Id], [AffiliateId], [DayOfWeek], [StartTime], [EndTime]) VALUES (147, 1, 0, CAST(0x00008EAC00A4CB80 AS DateTime), CAST(0x00008EAC00AD08E0 AS DateTime))
	INSERT [Booking].[AffiliateTimeOfBooking] ([Id], [AffiliateId], [DayOfWeek], [StartTime], [EndTime]) VALUES (148, 1, 0, CAST(0x00008EAC00AD08E0 AS DateTime), CAST(0x00008EAC00B54640 AS DateTime))
	INSERT [Booking].[AffiliateTimeOfBooking] ([Id], [AffiliateId], [DayOfWeek], [StartTime], [EndTime]) VALUES (149, 1, 0, CAST(0x00008EAC00B54640 AS DateTime), CAST(0x00008EAC00BD83A0 AS DateTime))
	INSERT [Booking].[AffiliateTimeOfBooking] ([Id], [AffiliateId], [DayOfWeek], [StartTime], [EndTime]) VALUES (150, 1, 0, CAST(0x00008EAC00BD83A0 AS DateTime), CAST(0x00008EAC00C5C100 AS DateTime))
	INSERT [Booking].[AffiliateTimeOfBooking] ([Id], [AffiliateId], [DayOfWeek], [StartTime], [EndTime]) VALUES (151, 1, 0, CAST(0x00008EAC00C5C100 AS DateTime), CAST(0x00008EAC00CDFE60 AS DateTime))
	INSERT [Booking].[AffiliateTimeOfBooking] ([Id], [AffiliateId], [DayOfWeek], [StartTime], [EndTime]) VALUES (152, 1, 0, CAST(0x00008EAC00CDFE60 AS DateTime), CAST(0x00008EAC00D63BC0 AS DateTime))
	INSERT [Booking].[AffiliateTimeOfBooking] ([Id], [AffiliateId], [DayOfWeek], [StartTime], [EndTime]) VALUES (153, 1, 0, CAST(0x00008EAC00D63BC0 AS DateTime), CAST(0x00008EAC00DE7920 AS DateTime))
	INSERT [Booking].[AffiliateTimeOfBooking] ([Id], [AffiliateId], [DayOfWeek], [StartTime], [EndTime]) VALUES (154, 1, 0, CAST(0x00008EAC00DE7920 AS DateTime), CAST(0x00008EAC00E6B680 AS DateTime))
	INSERT [Booking].[AffiliateTimeOfBooking] ([Id], [AffiliateId], [DayOfWeek], [StartTime], [EndTime]) VALUES (155, 1, 0, CAST(0x00008EAC00E6B680 AS DateTime), CAST(0x00008EAC00EEF3E0 AS DateTime))
	INSERT [Booking].[AffiliateTimeOfBooking] ([Id], [AffiliateId], [DayOfWeek], [StartTime], [EndTime]) VALUES (156, 1, 0, CAST(0x00008EAC00EEF3E0 AS DateTime), CAST(0x00008EAC00F73140 AS DateTime))
	INSERT [Booking].[AffiliateTimeOfBooking] ([Id], [AffiliateId], [DayOfWeek], [StartTime], [EndTime]) VALUES (157, 1, 0, CAST(0x00008EAC00F73140 AS DateTime), CAST(0x00008EAC00FF6EA0 AS DateTime))
	INSERT [Booking].[AffiliateTimeOfBooking] ([Id], [AffiliateId], [DayOfWeek], [StartTime], [EndTime]) VALUES (158, 1, 0, CAST(0x00008EAC00FF6EA0 AS DateTime), CAST(0x00008EAC0107AC00 AS DateTime))
	INSERT [Booking].[AffiliateTimeOfBooking] ([Id], [AffiliateId], [DayOfWeek], [StartTime], [EndTime]) VALUES (159, 1, 0, CAST(0x00008EAC0107AC00 AS DateTime), CAST(0x00008EAC010FE960 AS DateTime))
	INSERT [Booking].[AffiliateTimeOfBooking] ([Id], [AffiliateId], [DayOfWeek], [StartTime], [EndTime]) VALUES (160, 1, 0, CAST(0x00008EAC010FE960 AS DateTime), CAST(0x00008EAC011826C0 AS DateTime))
	INSERT [Booking].[AffiliateTimeOfBooking] ([Id], [AffiliateId], [DayOfWeek], [StartTime], [EndTime]) VALUES (161, 1, 0, CAST(0x00008EAC011826C0 AS DateTime), CAST(0x00008EAC01206420 AS DateTime))
	INSERT [Booking].[AffiliateTimeOfBooking] ([Id], [AffiliateId], [DayOfWeek], [StartTime], [EndTime]) VALUES (162, 1, 0, CAST(0x00008EAC01206420 AS DateTime), CAST(0x00008EAC0128A180 AS DateTime))
	INSERT [Booking].[AffiliateTimeOfBooking] ([Id], [AffiliateId], [DayOfWeek], [StartTime], [EndTime]) VALUES (163, 1, 0, CAST(0x00008EAC0128A180 AS DateTime), CAST(0x00008EAC0130DEE0 AS DateTime))
	INSERT [Booking].[AffiliateTimeOfBooking] ([Id], [AffiliateId], [DayOfWeek], [StartTime], [EndTime]) VALUES (164, 1, 0, CAST(0x00008EAC0130DEE0 AS DateTime), CAST(0x00008EAC01391C40 AS DateTime))
	INSERT [Booking].[AffiliateTimeOfBooking] ([Id], [AffiliateId], [DayOfWeek], [StartTime], [EndTime]) VALUES (165, 1, 0, CAST(0x00008EAC01391C40 AS DateTime), CAST(0x00008EAC014159A0 AS DateTime))
	INSERT [Booking].[AffiliateTimeOfBooking] ([Id], [AffiliateId], [DayOfWeek], [StartTime], [EndTime]) VALUES (166, 1, 0, CAST(0x00008EAC014159A0 AS DateTime), CAST(0x00008EAC01499700 AS DateTime))
	INSERT [Booking].[AffiliateTimeOfBooking] ([Id], [AffiliateId], [DayOfWeek], [StartTime], [EndTime]) VALUES (167, 1, 0, CAST(0x00008EAC01499700 AS DateTime), CAST(0x00008EAC0151D460 AS DateTime))
	INSERT [Booking].[AffiliateTimeOfBooking] ([Id], [AffiliateId], [DayOfWeek], [StartTime], [EndTime]) VALUES (168, 1, 0, CAST(0x00008EAC0151D460 AS DateTime), CAST(0x00008EAC015A11C0 AS DateTime))
	SET IDENTITY_INSERT [Booking].[AffiliateTimeOfBooking] OFF

	INSERT [Booking].[AffiliateService] ([AffiliateId], [ServiceId]) VALUES (1, 1)
	INSERT [Booking].[AffiliateService] ([AffiliateId], [ServiceId]) VALUES (1, 2)
	INSERT [Booking].[AffiliateService] ([AffiliateId], [ServiceId]) VALUES (1, 3)
	INSERT [Booking].[AffiliateService] ([AffiliateId], [ServiceId]) VALUES (1, 4)
	INSERT [Booking].[AffiliateService] ([AffiliateId], [ServiceId]) VALUES (1, 5)
	INSERT [Booking].[AffiliateService] ([AffiliateId], [ServiceId]) VALUES (1, 6)

	SET IDENTITY_INSERT [Booking].[ReservationResource] ON
	INSERT [Booking].[ReservationResource] ([Id], [ManagerId], [Name], [Description], [Image], [Enabled], [SortOrder]) VALUES (1, NULL, N'ФИО Сотрудника 1', NULL, N'1_180613020508.jpg', 1, 0)
	INSERT [Booking].[ReservationResource] ([Id], [ManagerId], [Name], [Description], [Image], [Enabled], [SortOrder]) VALUES (2, NULL, N'ФИО Сотрудника 2', NULL, N'1_180613020508.jpg', 1, 0)
	INSERT [Booking].[ReservationResource] ([Id], [ManagerId], [Name], [Description], [Image], [Enabled], [SortOrder]) VALUES (3, NULL, N'ФИО Сотрудника 3', NULL, N'1_180613020508.jpg', 1, 0)
	SET IDENTITY_INSERT [Booking].[ReservationResource] OFF

	SET IDENTITY_INSERT [Booking].[ReservationResourceTimeOfBooking] ON
	INSERT [Booking].[ReservationResourceTimeOfBooking] ([Id], [AffiliateId], [ReservationResourceId], [DayOfWeek], [StartTime], [EndTime]) VALUES (147, 1, 1, 1, CAST(0x00008EAC009450C0 AS DateTime), CAST(0x00008EAC009C8E20 AS DateTime))
	INSERT [Booking].[ReservationResourceTimeOfBooking] ([Id], [AffiliateId], [ReservationResourceId], [DayOfWeek], [StartTime], [EndTime]) VALUES (148, 1, 1, 1, CAST(0x00008EAC009C8E20 AS DateTime), CAST(0x00008EAC00A4CB80 AS DateTime))
	INSERT [Booking].[ReservationResourceTimeOfBooking] ([Id], [AffiliateId], [ReservationResourceId], [DayOfWeek], [StartTime], [EndTime]) VALUES (149, 1, 1, 1, CAST(0x00008EAC00A4CB80 AS DateTime), CAST(0x00008EAC00AD08E0 AS DateTime))
	INSERT [Booking].[ReservationResourceTimeOfBooking] ([Id], [AffiliateId], [ReservationResourceId], [DayOfWeek], [StartTime], [EndTime]) VALUES (150, 1, 1, 1, CAST(0x00008EAC00AD08E0 AS DateTime), CAST(0x00008EAC00B54640 AS DateTime))
	INSERT [Booking].[ReservationResourceTimeOfBooking] ([Id], [AffiliateId], [ReservationResourceId], [DayOfWeek], [StartTime], [EndTime]) VALUES (151, 1, 1, 1, CAST(0x00008EAC00B54640 AS DateTime), CAST(0x00008EAC00BD83A0 AS DateTime))
	INSERT [Booking].[ReservationResourceTimeOfBooking] ([Id], [AffiliateId], [ReservationResourceId], [DayOfWeek], [StartTime], [EndTime]) VALUES (152, 1, 1, 1, CAST(0x00008EAC00BD83A0 AS DateTime), CAST(0x00008EAC00C5C100 AS DateTime))
	INSERT [Booking].[ReservationResourceTimeOfBooking] ([Id], [AffiliateId], [ReservationResourceId], [DayOfWeek], [StartTime], [EndTime]) VALUES (153, 1, 1, 1, CAST(0x00008EAC00C5C100 AS DateTime), CAST(0x00008EAC00CDFE60 AS DateTime))
	INSERT [Booking].[ReservationResourceTimeOfBooking] ([Id], [AffiliateId], [ReservationResourceId], [DayOfWeek], [StartTime], [EndTime]) VALUES (154, 1, 1, 1, CAST(0x00008EAC00CDFE60 AS DateTime), CAST(0x00008EAC00D63BC0 AS DateTime))
	INSERT [Booking].[ReservationResourceTimeOfBooking] ([Id], [AffiliateId], [ReservationResourceId], [DayOfWeek], [StartTime], [EndTime]) VALUES (155, 1, 1, 1, CAST(0x00008EAC00D63BC0 AS DateTime), CAST(0x00008EAC00DE7920 AS DateTime))
	INSERT [Booking].[ReservationResourceTimeOfBooking] ([Id], [AffiliateId], [ReservationResourceId], [DayOfWeek], [StartTime], [EndTime]) VALUES (156, 1, 1, 1, CAST(0x00008EAC00DE7920 AS DateTime), CAST(0x00008EAC00E6B680 AS DateTime))
	INSERT [Booking].[ReservationResourceTimeOfBooking] ([Id], [AffiliateId], [ReservationResourceId], [DayOfWeek], [StartTime], [EndTime]) VALUES (157, 1, 1, 1, CAST(0x00008EAC00E6B680 AS DateTime), CAST(0x00008EAC00EEF3E0 AS DateTime))
	INSERT [Booking].[ReservationResourceTimeOfBooking] ([Id], [AffiliateId], [ReservationResourceId], [DayOfWeek], [StartTime], [EndTime]) VALUES (158, 1, 1, 1, CAST(0x00008EAC00EEF3E0 AS DateTime), CAST(0x00008EAC00F73140 AS DateTime))
	INSERT [Booking].[ReservationResourceTimeOfBooking] ([Id], [AffiliateId], [ReservationResourceId], [DayOfWeek], [StartTime], [EndTime]) VALUES (159, 1, 1, 1, CAST(0x00008EAC00F73140 AS DateTime), CAST(0x00008EAC00FF6EA0 AS DateTime))
	INSERT [Booking].[ReservationResourceTimeOfBooking] ([Id], [AffiliateId], [ReservationResourceId], [DayOfWeek], [StartTime], [EndTime]) VALUES (160, 1, 1, 1, CAST(0x00008EAC00FF6EA0 AS DateTime), CAST(0x00008EAC0107AC00 AS DateTime))
	INSERT [Booking].[ReservationResourceTimeOfBooking] ([Id], [AffiliateId], [ReservationResourceId], [DayOfWeek], [StartTime], [EndTime]) VALUES (161, 1, 1, 1, CAST(0x00008EAC0107AC00 AS DateTime), CAST(0x00008EAC010FE960 AS DateTime))
	INSERT [Booking].[ReservationResourceTimeOfBooking] ([Id], [AffiliateId], [ReservationResourceId], [DayOfWeek], [StartTime], [EndTime]) VALUES (162, 1, 1, 3, CAST(0x00008EAC009450C0 AS DateTime), CAST(0x00008EAC009C8E20 AS DateTime))
	INSERT [Booking].[ReservationResourceTimeOfBooking] ([Id], [AffiliateId], [ReservationResourceId], [DayOfWeek], [StartTime], [EndTime]) VALUES (163, 1, 1, 3, CAST(0x00008EAC009C8E20 AS DateTime), CAST(0x00008EAC00A4CB80 AS DateTime))
	INSERT [Booking].[ReservationResourceTimeOfBooking] ([Id], [AffiliateId], [ReservationResourceId], [DayOfWeek], [StartTime], [EndTime]) VALUES (164, 1, 1, 3, CAST(0x00008EAC00A4CB80 AS DateTime), CAST(0x00008EAC00AD08E0 AS DateTime))
	INSERT [Booking].[ReservationResourceTimeOfBooking] ([Id], [AffiliateId], [ReservationResourceId], [DayOfWeek], [StartTime], [EndTime]) VALUES (165, 1, 1, 3, CAST(0x00008EAC00AD08E0 AS DateTime), CAST(0x00008EAC00B54640 AS DateTime))
	INSERT [Booking].[ReservationResourceTimeOfBooking] ([Id], [AffiliateId], [ReservationResourceId], [DayOfWeek], [StartTime], [EndTime]) VALUES (166, 1, 1, 3, CAST(0x00008EAC00B54640 AS DateTime), CAST(0x00008EAC00BD83A0 AS DateTime))
	INSERT [Booking].[ReservationResourceTimeOfBooking] ([Id], [AffiliateId], [ReservationResourceId], [DayOfWeek], [StartTime], [EndTime]) VALUES (167, 1, 1, 3, CAST(0x00008EAC00BD83A0 AS DateTime), CAST(0x00008EAC00C5C100 AS DateTime))
	INSERT [Booking].[ReservationResourceTimeOfBooking] ([Id], [AffiliateId], [ReservationResourceId], [DayOfWeek], [StartTime], [EndTime]) VALUES (168, 1, 1, 3, CAST(0x00008EAC00C5C100 AS DateTime), CAST(0x00008EAC00CDFE60 AS DateTime))
	INSERT [Booking].[ReservationResourceTimeOfBooking] ([Id], [AffiliateId], [ReservationResourceId], [DayOfWeek], [StartTime], [EndTime]) VALUES (169, 1, 1, 3, CAST(0x00008EAC00CDFE60 AS DateTime), CAST(0x00008EAC00D63BC0 AS DateTime))
	INSERT [Booking].[ReservationResourceTimeOfBooking] ([Id], [AffiliateId], [ReservationResourceId], [DayOfWeek], [StartTime], [EndTime]) VALUES (170, 1, 1, 3, CAST(0x00008EAC00D63BC0 AS DateTime), CAST(0x00008EAC00DE7920 AS DateTime))
	INSERT [Booking].[ReservationResourceTimeOfBooking] ([Id], [AffiliateId], [ReservationResourceId], [DayOfWeek], [StartTime], [EndTime]) VALUES (171, 1, 1, 3, CAST(0x00008EAC00DE7920 AS DateTime), CAST(0x00008EAC00E6B680 AS DateTime))
	INSERT [Booking].[ReservationResourceTimeOfBooking] ([Id], [AffiliateId], [ReservationResourceId], [DayOfWeek], [StartTime], [EndTime]) VALUES (172, 1, 1, 3, CAST(0x00008EAC00E6B680 AS DateTime), CAST(0x00008EAC00EEF3E0 AS DateTime))
	INSERT [Booking].[ReservationResourceTimeOfBooking] ([Id], [AffiliateId], [ReservationResourceId], [DayOfWeek], [StartTime], [EndTime]) VALUES (173, 1, 1, 3, CAST(0x00008EAC00EEF3E0 AS DateTime), CAST(0x00008EAC00F73140 AS DateTime))
	INSERT [Booking].[ReservationResourceTimeOfBooking] ([Id], [AffiliateId], [ReservationResourceId], [DayOfWeek], [StartTime], [EndTime]) VALUES (174, 1, 1, 3, CAST(0x00008EAC00F73140 AS DateTime), CAST(0x00008EAC00FF6EA0 AS DateTime))
	INSERT [Booking].[ReservationResourceTimeOfBooking] ([Id], [AffiliateId], [ReservationResourceId], [DayOfWeek], [StartTime], [EndTime]) VALUES (175, 1, 1, 3, CAST(0x00008EAC00FF6EA0 AS DateTime), CAST(0x00008EAC0107AC00 AS DateTime))
	INSERT [Booking].[ReservationResourceTimeOfBooking] ([Id], [AffiliateId], [ReservationResourceId], [DayOfWeek], [StartTime], [EndTime]) VALUES (176, 1, 1, 3, CAST(0x00008EAC0107AC00 AS DateTime), CAST(0x00008EAC010FE960 AS DateTime))
	INSERT [Booking].[ReservationResourceTimeOfBooking] ([Id], [AffiliateId], [ReservationResourceId], [DayOfWeek], [StartTime], [EndTime]) VALUES (177, 1, 1, 5, CAST(0x00008EAC009450C0 AS DateTime), CAST(0x00008EAC009C8E20 AS DateTime))
	INSERT [Booking].[ReservationResourceTimeOfBooking] ([Id], [AffiliateId], [ReservationResourceId], [DayOfWeek], [StartTime], [EndTime]) VALUES (178, 1, 1, 5, CAST(0x00008EAC009C8E20 AS DateTime), CAST(0x00008EAC00A4CB80 AS DateTime))
	INSERT [Booking].[ReservationResourceTimeOfBooking] ([Id], [AffiliateId], [ReservationResourceId], [DayOfWeek], [StartTime], [EndTime]) VALUES (179, 1, 1, 5, CAST(0x00008EAC00A4CB80 AS DateTime), CAST(0x00008EAC00AD08E0 AS DateTime))
	INSERT [Booking].[ReservationResourceTimeOfBooking] ([Id], [AffiliateId], [ReservationResourceId], [DayOfWeek], [StartTime], [EndTime]) VALUES (180, 1, 1, 5, CAST(0x00008EAC00AD08E0 AS DateTime), CAST(0x00008EAC00B54640 AS DateTime))
	INSERT [Booking].[ReservationResourceTimeOfBooking] ([Id], [AffiliateId], [ReservationResourceId], [DayOfWeek], [StartTime], [EndTime]) VALUES (181, 1, 1, 5, CAST(0x00008EAC00B54640 AS DateTime), CAST(0x00008EAC00BD83A0 AS DateTime))
	INSERT [Booking].[ReservationResourceTimeOfBooking] ([Id], [AffiliateId], [ReservationResourceId], [DayOfWeek], [StartTime], [EndTime]) VALUES (182, 1, 1, 5, CAST(0x00008EAC00BD83A0 AS DateTime), CAST(0x00008EAC00C5C100 AS DateTime))
	INSERT [Booking].[ReservationResourceTimeOfBooking] ([Id], [AffiliateId], [ReservationResourceId], [DayOfWeek], [StartTime], [EndTime]) VALUES (183, 1, 1, 5, CAST(0x00008EAC00C5C100 AS DateTime), CAST(0x00008EAC00CDFE60 AS DateTime))
	INSERT [Booking].[ReservationResourceTimeOfBooking] ([Id], [AffiliateId], [ReservationResourceId], [DayOfWeek], [StartTime], [EndTime]) VALUES (184, 1, 1, 5, CAST(0x00008EAC00CDFE60 AS DateTime), CAST(0x00008EAC00D63BC0 AS DateTime))
	INSERT [Booking].[ReservationResourceTimeOfBooking] ([Id], [AffiliateId], [ReservationResourceId], [DayOfWeek], [StartTime], [EndTime]) VALUES (185, 1, 1, 5, CAST(0x00008EAC00D63BC0 AS DateTime), CAST(0x00008EAC00DE7920 AS DateTime))
	INSERT [Booking].[ReservationResourceTimeOfBooking] ([Id], [AffiliateId], [ReservationResourceId], [DayOfWeek], [StartTime], [EndTime]) VALUES (186, 1, 1, 5, CAST(0x00008EAC00DE7920 AS DateTime), CAST(0x00008EAC00E6B680 AS DateTime))
	INSERT [Booking].[ReservationResourceTimeOfBooking] ([Id], [AffiliateId], [ReservationResourceId], [DayOfWeek], [StartTime], [EndTime]) VALUES (187, 1, 1, 5, CAST(0x00008EAC00E6B680 AS DateTime), CAST(0x00008EAC00EEF3E0 AS DateTime))
	INSERT [Booking].[ReservationResourceTimeOfBooking] ([Id], [AffiliateId], [ReservationResourceId], [DayOfWeek], [StartTime], [EndTime]) VALUES (188, 1, 1, 5, CAST(0x00008EAC00EEF3E0 AS DateTime), CAST(0x00008EAC00F73140 AS DateTime))
	INSERT [Booking].[ReservationResourceTimeOfBooking] ([Id], [AffiliateId], [ReservationResourceId], [DayOfWeek], [StartTime], [EndTime]) VALUES (189, 1, 1, 5, CAST(0x00008EAC00F73140 AS DateTime), CAST(0x00008EAC00FF6EA0 AS DateTime))
	INSERT [Booking].[ReservationResourceTimeOfBooking] ([Id], [AffiliateId], [ReservationResourceId], [DayOfWeek], [StartTime], [EndTime]) VALUES (190, 1, 1, 5, CAST(0x00008EAC00FF6EA0 AS DateTime), CAST(0x00008EAC0107AC00 AS DateTime))
	INSERT [Booking].[ReservationResourceTimeOfBooking] ([Id], [AffiliateId], [ReservationResourceId], [DayOfWeek], [StartTime], [EndTime]) VALUES (191, 1, 1, 5, CAST(0x00008EAC0107AC00 AS DateTime), CAST(0x00008EAC010FE960 AS DateTime))
	INSERT [Booking].[ReservationResourceTimeOfBooking] ([Id], [AffiliateId], [ReservationResourceId], [DayOfWeek], [StartTime], [EndTime]) VALUES (192, 1, 1, 0, CAST(0x00008EAC009450C0 AS DateTime), CAST(0x00008EAC009C8E20 AS DateTime))
	INSERT [Booking].[ReservationResourceTimeOfBooking] ([Id], [AffiliateId], [ReservationResourceId], [DayOfWeek], [StartTime], [EndTime]) VALUES (193, 1, 1, 0, CAST(0x00008EAC009C8E20 AS DateTime), CAST(0x00008EAC00A4CB80 AS DateTime))
	INSERT [Booking].[ReservationResourceTimeOfBooking] ([Id], [AffiliateId], [ReservationResourceId], [DayOfWeek], [StartTime], [EndTime]) VALUES (194, 1, 1, 0, CAST(0x00008EAC00A4CB80 AS DateTime), CAST(0x00008EAC00AD08E0 AS DateTime))
	INSERT [Booking].[ReservationResourceTimeOfBooking] ([Id], [AffiliateId], [ReservationResourceId], [DayOfWeek], [StartTime], [EndTime]) VALUES (195, 1, 1, 0, CAST(0x00008EAC00AD08E0 AS DateTime), CAST(0x00008EAC00B54640 AS DateTime))
	INSERT [Booking].[ReservationResourceTimeOfBooking] ([Id], [AffiliateId], [ReservationResourceId], [DayOfWeek], [StartTime], [EndTime]) VALUES (196, 1, 1, 0, CAST(0x00008EAC00B54640 AS DateTime), CAST(0x00008EAC00BD83A0 AS DateTime))
	INSERT [Booking].[ReservationResourceTimeOfBooking] ([Id], [AffiliateId], [ReservationResourceId], [DayOfWeek], [StartTime], [EndTime]) VALUES (197, 1, 1, 0, CAST(0x00008EAC00BD83A0 AS DateTime), CAST(0x00008EAC00C5C100 AS DateTime))
	INSERT [Booking].[ReservationResourceTimeOfBooking] ([Id], [AffiliateId], [ReservationResourceId], [DayOfWeek], [StartTime], [EndTime]) VALUES (198, 1, 1, 0, CAST(0x00008EAC00C5C100 AS DateTime), CAST(0x00008EAC00CDFE60 AS DateTime))
	INSERT [Booking].[ReservationResourceTimeOfBooking] ([Id], [AffiliateId], [ReservationResourceId], [DayOfWeek], [StartTime], [EndTime]) VALUES (199, 1, 1, 0, CAST(0x00008EAC00CDFE60 AS DateTime), CAST(0x00008EAC00D63BC0 AS DateTime))
	INSERT [Booking].[ReservationResourceTimeOfBooking] ([Id], [AffiliateId], [ReservationResourceId], [DayOfWeek], [StartTime], [EndTime]) VALUES (200, 1, 1, 0, CAST(0x00008EAC00D63BC0 AS DateTime), CAST(0x00008EAC00DE7920 AS DateTime))
	INSERT [Booking].[ReservationResourceTimeOfBooking] ([Id], [AffiliateId], [ReservationResourceId], [DayOfWeek], [StartTime], [EndTime]) VALUES (201, 1, 1, 0, CAST(0x00008EAC00DE7920 AS DateTime), CAST(0x00008EAC00E6B680 AS DateTime))
	INSERT [Booking].[ReservationResourceTimeOfBooking] ([Id], [AffiliateId], [ReservationResourceId], [DayOfWeek], [StartTime], [EndTime]) VALUES (202, 1, 1, 0, CAST(0x00008EAC00E6B680 AS DateTime), CAST(0x00008EAC00EEF3E0 AS DateTime))
	INSERT [Booking].[ReservationResourceTimeOfBooking] ([Id], [AffiliateId], [ReservationResourceId], [DayOfWeek], [StartTime], [EndTime]) VALUES (203, 1, 1, 0, CAST(0x00008EAC00EEF3E0 AS DateTime), CAST(0x00008EAC00F73140 AS DateTime))
	INSERT [Booking].[ReservationResourceTimeOfBooking] ([Id], [AffiliateId], [ReservationResourceId], [DayOfWeek], [StartTime], [EndTime]) VALUES (204, 1, 1, 0, CAST(0x00008EAC00F73140 AS DateTime), CAST(0x00008EAC00FF6EA0 AS DateTime))
	INSERT [Booking].[ReservationResourceTimeOfBooking] ([Id], [AffiliateId], [ReservationResourceId], [DayOfWeek], [StartTime], [EndTime]) VALUES (205, 1, 1, 0, CAST(0x00008EAC00FF6EA0 AS DateTime), CAST(0x00008EAC0107AC00 AS DateTime))
	INSERT [Booking].[ReservationResourceTimeOfBooking] ([Id], [AffiliateId], [ReservationResourceId], [DayOfWeek], [StartTime], [EndTime]) VALUES (206, 1, 1, 0, CAST(0x00008EAC0107AC00 AS DateTime), CAST(0x00008EAC010FE960 AS DateTime))
	INSERT [Booking].[ReservationResourceTimeOfBooking] ([Id], [AffiliateId], [ReservationResourceId], [DayOfWeek], [StartTime], [EndTime]) VALUES (207, 1, 2, 1, CAST(0x00008EAC009450C0 AS DateTime), CAST(0x00008EAC009C8E20 AS DateTime))
	INSERT [Booking].[ReservationResourceTimeOfBooking] ([Id], [AffiliateId], [ReservationResourceId], [DayOfWeek], [StartTime], [EndTime]) VALUES (208, 1, 2, 1, CAST(0x00008EAC009C8E20 AS DateTime), CAST(0x00008EAC00A4CB80 AS DateTime))
	INSERT [Booking].[ReservationResourceTimeOfBooking] ([Id], [AffiliateId], [ReservationResourceId], [DayOfWeek], [StartTime], [EndTime]) VALUES (209, 1, 2, 1, CAST(0x00008EAC00A4CB80 AS DateTime), CAST(0x00008EAC00AD08E0 AS DateTime))
	INSERT [Booking].[ReservationResourceTimeOfBooking] ([Id], [AffiliateId], [ReservationResourceId], [DayOfWeek], [StartTime], [EndTime]) VALUES (210, 1, 2, 1, CAST(0x00008EAC00AD08E0 AS DateTime), CAST(0x00008EAC00B54640 AS DateTime))
	INSERT [Booking].[ReservationResourceTimeOfBooking] ([Id], [AffiliateId], [ReservationResourceId], [DayOfWeek], [StartTime], [EndTime]) VALUES (211, 1, 2, 1, CAST(0x00008EAC00B54640 AS DateTime), CAST(0x00008EAC00BD83A0 AS DateTime))
	INSERT [Booking].[ReservationResourceTimeOfBooking] ([Id], [AffiliateId], [ReservationResourceId], [DayOfWeek], [StartTime], [EndTime]) VALUES (212, 1, 2, 1, CAST(0x00008EAC00BD83A0 AS DateTime), CAST(0x00008EAC00C5C100 AS DateTime))
	INSERT [Booking].[ReservationResourceTimeOfBooking] ([Id], [AffiliateId], [ReservationResourceId], [DayOfWeek], [StartTime], [EndTime]) VALUES (213, 1, 2, 1, CAST(0x00008EAC00C5C100 AS DateTime), CAST(0x00008EAC00CDFE60 AS DateTime))
	INSERT [Booking].[ReservationResourceTimeOfBooking] ([Id], [AffiliateId], [ReservationResourceId], [DayOfWeek], [StartTime], [EndTime]) VALUES (214, 1, 2, 1, CAST(0x00008EAC00CDFE60 AS DateTime), CAST(0x00008EAC00D63BC0 AS DateTime))
	INSERT [Booking].[ReservationResourceTimeOfBooking] ([Id], [AffiliateId], [ReservationResourceId], [DayOfWeek], [StartTime], [EndTime]) VALUES (215, 1, 2, 1, CAST(0x00008EAC00D63BC0 AS DateTime), CAST(0x00008EAC00DE7920 AS DateTime))
	INSERT [Booking].[ReservationResourceTimeOfBooking] ([Id], [AffiliateId], [ReservationResourceId], [DayOfWeek], [StartTime], [EndTime]) VALUES (216, 1, 2, 1, CAST(0x00008EAC00DE7920 AS DateTime), CAST(0x00008EAC00E6B680 AS DateTime))
	INSERT [Booking].[ReservationResourceTimeOfBooking] ([Id], [AffiliateId], [ReservationResourceId], [DayOfWeek], [StartTime], [EndTime]) VALUES (217, 1, 2, 1, CAST(0x00008EAC00E6B680 AS DateTime), CAST(0x00008EAC00EEF3E0 AS DateTime))
	INSERT [Booking].[ReservationResourceTimeOfBooking] ([Id], [AffiliateId], [ReservationResourceId], [DayOfWeek], [StartTime], [EndTime]) VALUES (218, 1, 2, 1, CAST(0x00008EAC00EEF3E0 AS DateTime), CAST(0x00008EAC00F73140 AS DateTime))
	INSERT [Booking].[ReservationResourceTimeOfBooking] ([Id], [AffiliateId], [ReservationResourceId], [DayOfWeek], [StartTime], [EndTime]) VALUES (219, 1, 2, 1, CAST(0x00008EAC00F73140 AS DateTime), CAST(0x00008EAC00FF6EA0 AS DateTime))
	INSERT [Booking].[ReservationResourceTimeOfBooking] ([Id], [AffiliateId], [ReservationResourceId], [DayOfWeek], [StartTime], [EndTime]) VALUES (220, 1, 2, 1, CAST(0x00008EAC00FF6EA0 AS DateTime), CAST(0x00008EAC0107AC00 AS DateTime))
	INSERT [Booking].[ReservationResourceTimeOfBooking] ([Id], [AffiliateId], [ReservationResourceId], [DayOfWeek], [StartTime], [EndTime]) VALUES (221, 1, 2, 1, CAST(0x00008EAC0107AC00 AS DateTime), CAST(0x00008EAC010FE960 AS DateTime))
	INSERT [Booking].[ReservationResourceTimeOfBooking] ([Id], [AffiliateId], [ReservationResourceId], [DayOfWeek], [StartTime], [EndTime]) VALUES (222, 1, 2, 3, CAST(0x00008EAC009450C0 AS DateTime), CAST(0x00008EAC009C8E20 AS DateTime))
	INSERT [Booking].[ReservationResourceTimeOfBooking] ([Id], [AffiliateId], [ReservationResourceId], [DayOfWeek], [StartTime], [EndTime]) VALUES (223, 1, 2, 3, CAST(0x00008EAC009C8E20 AS DateTime), CAST(0x00008EAC00A4CB80 AS DateTime))
	INSERT [Booking].[ReservationResourceTimeOfBooking] ([Id], [AffiliateId], [ReservationResourceId], [DayOfWeek], [StartTime], [EndTime]) VALUES (224, 1, 2, 3, CAST(0x00008EAC00A4CB80 AS DateTime), CAST(0x00008EAC00AD08E0 AS DateTime))
	INSERT [Booking].[ReservationResourceTimeOfBooking] ([Id], [AffiliateId], [ReservationResourceId], [DayOfWeek], [StartTime], [EndTime]) VALUES (225, 1, 2, 3, CAST(0x00008EAC00AD08E0 AS DateTime), CAST(0x00008EAC00B54640 AS DateTime))
	INSERT [Booking].[ReservationResourceTimeOfBooking] ([Id], [AffiliateId], [ReservationResourceId], [DayOfWeek], [StartTime], [EndTime]) VALUES (226, 1, 2, 3, CAST(0x00008EAC00B54640 AS DateTime), CAST(0x00008EAC00BD83A0 AS DateTime))
	INSERT [Booking].[ReservationResourceTimeOfBooking] ([Id], [AffiliateId], [ReservationResourceId], [DayOfWeek], [StartTime], [EndTime]) VALUES (227, 1, 2, 3, CAST(0x00008EAC00BD83A0 AS DateTime), CAST(0x00008EAC00C5C100 AS DateTime))
	INSERT [Booking].[ReservationResourceTimeOfBooking] ([Id], [AffiliateId], [ReservationResourceId], [DayOfWeek], [StartTime], [EndTime]) VALUES (228, 1, 2, 3, CAST(0x00008EAC00C5C100 AS DateTime), CAST(0x00008EAC00CDFE60 AS DateTime))
	INSERT [Booking].[ReservationResourceTimeOfBooking] ([Id], [AffiliateId], [ReservationResourceId], [DayOfWeek], [StartTime], [EndTime]) VALUES (229, 1, 2, 3, CAST(0x00008EAC00CDFE60 AS DateTime), CAST(0x00008EAC00D63BC0 AS DateTime))
	INSERT [Booking].[ReservationResourceTimeOfBooking] ([Id], [AffiliateId], [ReservationResourceId], [DayOfWeek], [StartTime], [EndTime]) VALUES (230, 1, 2, 3, CAST(0x00008EAC00D63BC0 AS DateTime), CAST(0x00008EAC00DE7920 AS DateTime))
	INSERT [Booking].[ReservationResourceTimeOfBooking] ([Id], [AffiliateId], [ReservationResourceId], [DayOfWeek], [StartTime], [EndTime]) VALUES (231, 1, 2, 3, CAST(0x00008EAC00DE7920 AS DateTime), CAST(0x00008EAC00E6B680 AS DateTime))
	INSERT [Booking].[ReservationResourceTimeOfBooking] ([Id], [AffiliateId], [ReservationResourceId], [DayOfWeek], [StartTime], [EndTime]) VALUES (232, 1, 2, 3, CAST(0x00008EAC00E6B680 AS DateTime), CAST(0x00008EAC00EEF3E0 AS DateTime))
	INSERT [Booking].[ReservationResourceTimeOfBooking] ([Id], [AffiliateId], [ReservationResourceId], [DayOfWeek], [StartTime], [EndTime]) VALUES (233, 1, 2, 3, CAST(0x00008EAC00EEF3E0 AS DateTime), CAST(0x00008EAC00F73140 AS DateTime))
	INSERT [Booking].[ReservationResourceTimeOfBooking] ([Id], [AffiliateId], [ReservationResourceId], [DayOfWeek], [StartTime], [EndTime]) VALUES (234, 1, 2, 3, CAST(0x00008EAC00F73140 AS DateTime), CAST(0x00008EAC00FF6EA0 AS DateTime))
	INSERT [Booking].[ReservationResourceTimeOfBooking] ([Id], [AffiliateId], [ReservationResourceId], [DayOfWeek], [StartTime], [EndTime]) VALUES (235, 1, 2, 3, CAST(0x00008EAC00FF6EA0 AS DateTime), CAST(0x00008EAC0107AC00 AS DateTime))
	INSERT [Booking].[ReservationResourceTimeOfBooking] ([Id], [AffiliateId], [ReservationResourceId], [DayOfWeek], [StartTime], [EndTime]) VALUES (236, 1, 2, 3, CAST(0x00008EAC0107AC00 AS DateTime), CAST(0x00008EAC010FE960 AS DateTime))
	INSERT [Booking].[ReservationResourceTimeOfBooking] ([Id], [AffiliateId], [ReservationResourceId], [DayOfWeek], [StartTime], [EndTime]) VALUES (237, 1, 2, 5, CAST(0x00008EAC009450C0 AS DateTime), CAST(0x00008EAC009C8E20 AS DateTime))
	INSERT [Booking].[ReservationResourceTimeOfBooking] ([Id], [AffiliateId], [ReservationResourceId], [DayOfWeek], [StartTime], [EndTime]) VALUES (238, 1, 2, 5, CAST(0x00008EAC009C8E20 AS DateTime), CAST(0x00008EAC00A4CB80 AS DateTime))
	INSERT [Booking].[ReservationResourceTimeOfBooking] ([Id], [AffiliateId], [ReservationResourceId], [DayOfWeek], [StartTime], [EndTime]) VALUES (239, 1, 2, 5, CAST(0x00008EAC00A4CB80 AS DateTime), CAST(0x00008EAC00AD08E0 AS DateTime))
	INSERT [Booking].[ReservationResourceTimeOfBooking] ([Id], [AffiliateId], [ReservationResourceId], [DayOfWeek], [StartTime], [EndTime]) VALUES (240, 1, 2, 5, CAST(0x00008EAC00AD08E0 AS DateTime), CAST(0x00008EAC00B54640 AS DateTime))
	INSERT [Booking].[ReservationResourceTimeOfBooking] ([Id], [AffiliateId], [ReservationResourceId], [DayOfWeek], [StartTime], [EndTime]) VALUES (241, 1, 2, 5, CAST(0x00008EAC00B54640 AS DateTime), CAST(0x00008EAC00BD83A0 AS DateTime))
	INSERT [Booking].[ReservationResourceTimeOfBooking] ([Id], [AffiliateId], [ReservationResourceId], [DayOfWeek], [StartTime], [EndTime]) VALUES (242, 1, 2, 5, CAST(0x00008EAC00BD83A0 AS DateTime), CAST(0x00008EAC00C5C100 AS DateTime))
	INSERT [Booking].[ReservationResourceTimeOfBooking] ([Id], [AffiliateId], [ReservationResourceId], [DayOfWeek], [StartTime], [EndTime]) VALUES (243, 1, 2, 5, CAST(0x00008EAC00C5C100 AS DateTime), CAST(0x00008EAC00CDFE60 AS DateTime))
	INSERT [Booking].[ReservationResourceTimeOfBooking] ([Id], [AffiliateId], [ReservationResourceId], [DayOfWeek], [StartTime], [EndTime]) VALUES (244, 1, 2, 5, CAST(0x00008EAC00CDFE60 AS DateTime), CAST(0x00008EAC00D63BC0 AS DateTime))
	INSERT [Booking].[ReservationResourceTimeOfBooking] ([Id], [AffiliateId], [ReservationResourceId], [DayOfWeek], [StartTime], [EndTime]) VALUES (245, 1, 2, 5, CAST(0x00008EAC00D63BC0 AS DateTime), CAST(0x00008EAC00DE7920 AS DateTime))
	INSERT [Booking].[ReservationResourceTimeOfBooking] ([Id], [AffiliateId], [ReservationResourceId], [DayOfWeek], [StartTime], [EndTime]) VALUES (246, 1, 2, 5, CAST(0x00008EAC00DE7920 AS DateTime), CAST(0x00008EAC00E6B680 AS DateTime))
	INSERT [Booking].[ReservationResourceTimeOfBooking] ([Id], [AffiliateId], [ReservationResourceId], [DayOfWeek], [StartTime], [EndTime]) VALUES (247, 1, 2, 5, CAST(0x00008EAC00E6B680 AS DateTime), CAST(0x00008EAC00EEF3E0 AS DateTime))
	INSERT [Booking].[ReservationResourceTimeOfBooking] ([Id], [AffiliateId], [ReservationResourceId], [DayOfWeek], [StartTime], [EndTime]) VALUES (248, 1, 2, 5, CAST(0x00008EAC00EEF3E0 AS DateTime), CAST(0x00008EAC00F73140 AS DateTime))
	INSERT [Booking].[ReservationResourceTimeOfBooking] ([Id], [AffiliateId], [ReservationResourceId], [DayOfWeek], [StartTime], [EndTime]) VALUES (249, 1, 2, 5, CAST(0x00008EAC00F73140 AS DateTime), CAST(0x00008EAC00FF6EA0 AS DateTime))
	INSERT [Booking].[ReservationResourceTimeOfBooking] ([Id], [AffiliateId], [ReservationResourceId], [DayOfWeek], [StartTime], [EndTime]) VALUES (250, 1, 2, 5, CAST(0x00008EAC00FF6EA0 AS DateTime), CAST(0x00008EAC0107AC00 AS DateTime))
	INSERT [Booking].[ReservationResourceTimeOfBooking] ([Id], [AffiliateId], [ReservationResourceId], [DayOfWeek], [StartTime], [EndTime]) VALUES (251, 1, 2, 5, CAST(0x00008EAC0107AC00 AS DateTime), CAST(0x00008EAC010FE960 AS DateTime))
	INSERT [Booking].[ReservationResourceTimeOfBooking] ([Id], [AffiliateId], [ReservationResourceId], [DayOfWeek], [StartTime], [EndTime]) VALUES (252, 1, 2, 0, CAST(0x00008EAC009450C0 AS DateTime), CAST(0x00008EAC009C8E20 AS DateTime))
	INSERT [Booking].[ReservationResourceTimeOfBooking] ([Id], [AffiliateId], [ReservationResourceId], [DayOfWeek], [StartTime], [EndTime]) VALUES (253, 1, 2, 0, CAST(0x00008EAC009C8E20 AS DateTime), CAST(0x00008EAC00A4CB80 AS DateTime))
	INSERT [Booking].[ReservationResourceTimeOfBooking] ([Id], [AffiliateId], [ReservationResourceId], [DayOfWeek], [StartTime], [EndTime]) VALUES (254, 1, 2, 0, CAST(0x00008EAC00A4CB80 AS DateTime), CAST(0x00008EAC00AD08E0 AS DateTime))
	INSERT [Booking].[ReservationResourceTimeOfBooking] ([Id], [AffiliateId], [ReservationResourceId], [DayOfWeek], [StartTime], [EndTime]) VALUES (255, 1, 2, 0, CAST(0x00008EAC00AD08E0 AS DateTime), CAST(0x00008EAC00B54640 AS DateTime))
	INSERT [Booking].[ReservationResourceTimeOfBooking] ([Id], [AffiliateId], [ReservationResourceId], [DayOfWeek], [StartTime], [EndTime]) VALUES (256, 1, 2, 0, CAST(0x00008EAC00B54640 AS DateTime), CAST(0x00008EAC00BD83A0 AS DateTime))
	INSERT [Booking].[ReservationResourceTimeOfBooking] ([Id], [AffiliateId], [ReservationResourceId], [DayOfWeek], [StartTime], [EndTime]) VALUES (257, 1, 2, 0, CAST(0x00008EAC00BD83A0 AS DateTime), CAST(0x00008EAC00C5C100 AS DateTime))
	INSERT [Booking].[ReservationResourceTimeOfBooking] ([Id], [AffiliateId], [ReservationResourceId], [DayOfWeek], [StartTime], [EndTime]) VALUES (258, 1, 2, 0, CAST(0x00008EAC00C5C100 AS DateTime), CAST(0x00008EAC00CDFE60 AS DateTime))
	INSERT [Booking].[ReservationResourceTimeOfBooking] ([Id], [AffiliateId], [ReservationResourceId], [DayOfWeek], [StartTime], [EndTime]) VALUES (259, 1, 2, 0, CAST(0x00008EAC00CDFE60 AS DateTime), CAST(0x00008EAC00D63BC0 AS DateTime))
	INSERT [Booking].[ReservationResourceTimeOfBooking] ([Id], [AffiliateId], [ReservationResourceId], [DayOfWeek], [StartTime], [EndTime]) VALUES (260, 1, 2, 0, CAST(0x00008EAC00D63BC0 AS DateTime), CAST(0x00008EAC00DE7920 AS DateTime))
	INSERT [Booking].[ReservationResourceTimeOfBooking] ([Id], [AffiliateId], [ReservationResourceId], [DayOfWeek], [StartTime], [EndTime]) VALUES (261, 1, 2, 0, CAST(0x00008EAC00DE7920 AS DateTime), CAST(0x00008EAC00E6B680 AS DateTime))
	INSERT [Booking].[ReservationResourceTimeOfBooking] ([Id], [AffiliateId], [ReservationResourceId], [DayOfWeek], [StartTime], [EndTime]) VALUES (262, 1, 2, 0, CAST(0x00008EAC00E6B680 AS DateTime), CAST(0x00008EAC00EEF3E0 AS DateTime))
	INSERT [Booking].[ReservationResourceTimeOfBooking] ([Id], [AffiliateId], [ReservationResourceId], [DayOfWeek], [StartTime], [EndTime]) VALUES (263, 1, 2, 0, CAST(0x00008EAC00EEF3E0 AS DateTime), CAST(0x00008EAC00F73140 AS DateTime))
	INSERT [Booking].[ReservationResourceTimeOfBooking] ([Id], [AffiliateId], [ReservationResourceId], [DayOfWeek], [StartTime], [EndTime]) VALUES (264, 1, 2, 0, CAST(0x00008EAC00F73140 AS DateTime), CAST(0x00008EAC00FF6EA0 AS DateTime))
	INSERT [Booking].[ReservationResourceTimeOfBooking] ([Id], [AffiliateId], [ReservationResourceId], [DayOfWeek], [StartTime], [EndTime]) VALUES (265, 1, 2, 0, CAST(0x00008EAC00FF6EA0 AS DateTime), CAST(0x00008EAC0107AC00 AS DateTime))
	INSERT [Booking].[ReservationResourceTimeOfBooking] ([Id], [AffiliateId], [ReservationResourceId], [DayOfWeek], [StartTime], [EndTime]) VALUES (266, 1, 2, 0, CAST(0x00008EAC0107AC00 AS DateTime), CAST(0x00008EAC010FE960 AS DateTime))
	INSERT [Booking].[ReservationResourceTimeOfBooking] ([Id], [AffiliateId], [ReservationResourceId], [DayOfWeek], [StartTime], [EndTime]) VALUES (267, 1, 3, 2, CAST(0x00008EAC009450C0 AS DateTime), CAST(0x00008EAC009C8E20 AS DateTime))
	INSERT [Booking].[ReservationResourceTimeOfBooking] ([Id], [AffiliateId], [ReservationResourceId], [DayOfWeek], [StartTime], [EndTime]) VALUES (268, 1, 3, 2, CAST(0x00008EAC009C8E20 AS DateTime), CAST(0x00008EAC00A4CB80 AS DateTime))
	INSERT [Booking].[ReservationResourceTimeOfBooking] ([Id], [AffiliateId], [ReservationResourceId], [DayOfWeek], [StartTime], [EndTime]) VALUES (269, 1, 3, 2, CAST(0x00008EAC00A4CB80 AS DateTime), CAST(0x00008EAC00AD08E0 AS DateTime))
	INSERT [Booking].[ReservationResourceTimeOfBooking] ([Id], [AffiliateId], [ReservationResourceId], [DayOfWeek], [StartTime], [EndTime]) VALUES (270, 1, 3, 2, CAST(0x00008EAC00AD08E0 AS DateTime), CAST(0x00008EAC00B54640 AS DateTime))
	INSERT [Booking].[ReservationResourceTimeOfBooking] ([Id], [AffiliateId], [ReservationResourceId], [DayOfWeek], [StartTime], [EndTime]) VALUES (271, 1, 3, 2, CAST(0x00008EAC00B54640 AS DateTime), CAST(0x00008EAC00BD83A0 AS DateTime))
	INSERT [Booking].[ReservationResourceTimeOfBooking] ([Id], [AffiliateId], [ReservationResourceId], [DayOfWeek], [StartTime], [EndTime]) VALUES (272, 1, 3, 2, CAST(0x00008EAC00BD83A0 AS DateTime), CAST(0x00008EAC00C5C100 AS DateTime))
	INSERT [Booking].[ReservationResourceTimeOfBooking] ([Id], [AffiliateId], [ReservationResourceId], [DayOfWeek], [StartTime], [EndTime]) VALUES (273, 1, 3, 2, CAST(0x00008EAC00C5C100 AS DateTime), CAST(0x00008EAC00CDFE60 AS DateTime))
	INSERT [Booking].[ReservationResourceTimeOfBooking] ([Id], [AffiliateId], [ReservationResourceId], [DayOfWeek], [StartTime], [EndTime]) VALUES (274, 1, 3, 2, CAST(0x00008EAC00CDFE60 AS DateTime), CAST(0x00008EAC00D63BC0 AS DateTime))
	INSERT [Booking].[ReservationResourceTimeOfBooking] ([Id], [AffiliateId], [ReservationResourceId], [DayOfWeek], [StartTime], [EndTime]) VALUES (275, 1, 3, 2, CAST(0x00008EAC00D63BC0 AS DateTime), CAST(0x00008EAC00DE7920 AS DateTime))
	INSERT [Booking].[ReservationResourceTimeOfBooking] ([Id], [AffiliateId], [ReservationResourceId], [DayOfWeek], [StartTime], [EndTime]) VALUES (276, 1, 3, 2, CAST(0x00008EAC00DE7920 AS DateTime), CAST(0x00008EAC00E6B680 AS DateTime))
	INSERT [Booking].[ReservationResourceTimeOfBooking] ([Id], [AffiliateId], [ReservationResourceId], [DayOfWeek], [StartTime], [EndTime]) VALUES (277, 1, 3, 2, CAST(0x00008EAC00E6B680 AS DateTime), CAST(0x00008EAC00EEF3E0 AS DateTime))
	INSERT [Booking].[ReservationResourceTimeOfBooking] ([Id], [AffiliateId], [ReservationResourceId], [DayOfWeek], [StartTime], [EndTime]) VALUES (278, 1, 3, 2, CAST(0x00008EAC00EEF3E0 AS DateTime), CAST(0x00008EAC00F73140 AS DateTime))
	INSERT [Booking].[ReservationResourceTimeOfBooking] ([Id], [AffiliateId], [ReservationResourceId], [DayOfWeek], [StartTime], [EndTime]) VALUES (279, 1, 3, 2, CAST(0x00008EAC00F73140 AS DateTime), CAST(0x00008EAC00FF6EA0 AS DateTime))
	INSERT [Booking].[ReservationResourceTimeOfBooking] ([Id], [AffiliateId], [ReservationResourceId], [DayOfWeek], [StartTime], [EndTime]) VALUES (280, 1, 3, 2, CAST(0x00008EAC00FF6EA0 AS DateTime), CAST(0x00008EAC0107AC00 AS DateTime))
	INSERT [Booking].[ReservationResourceTimeOfBooking] ([Id], [AffiliateId], [ReservationResourceId], [DayOfWeek], [StartTime], [EndTime]) VALUES (281, 1, 3, 2, CAST(0x00008EAC0107AC00 AS DateTime), CAST(0x00008EAC010FE960 AS DateTime))
	INSERT [Booking].[ReservationResourceTimeOfBooking] ([Id], [AffiliateId], [ReservationResourceId], [DayOfWeek], [StartTime], [EndTime]) VALUES (282, 1, 3, 4, CAST(0x00008EAC009450C0 AS DateTime), CAST(0x00008EAC009C8E20 AS DateTime))
	INSERT [Booking].[ReservationResourceTimeOfBooking] ([Id], [AffiliateId], [ReservationResourceId], [DayOfWeek], [StartTime], [EndTime]) VALUES (283, 1, 3, 4, CAST(0x00008EAC009C8E20 AS DateTime), CAST(0x00008EAC00A4CB80 AS DateTime))
	INSERT [Booking].[ReservationResourceTimeOfBooking] ([Id], [AffiliateId], [ReservationResourceId], [DayOfWeek], [StartTime], [EndTime]) VALUES (284, 1, 3, 4, CAST(0x00008EAC00A4CB80 AS DateTime), CAST(0x00008EAC00AD08E0 AS DateTime))
	INSERT [Booking].[ReservationResourceTimeOfBooking] ([Id], [AffiliateId], [ReservationResourceId], [DayOfWeek], [StartTime], [EndTime]) VALUES (285, 1, 3, 4, CAST(0x00008EAC00AD08E0 AS DateTime), CAST(0x00008EAC00B54640 AS DateTime))
	INSERT [Booking].[ReservationResourceTimeOfBooking] ([Id], [AffiliateId], [ReservationResourceId], [DayOfWeek], [StartTime], [EndTime]) VALUES (286, 1, 3, 4, CAST(0x00008EAC00B54640 AS DateTime), CAST(0x00008EAC00BD83A0 AS DateTime))
	INSERT [Booking].[ReservationResourceTimeOfBooking] ([Id], [AffiliateId], [ReservationResourceId], [DayOfWeek], [StartTime], [EndTime]) VALUES (287, 1, 3, 4, CAST(0x00008EAC00BD83A0 AS DateTime), CAST(0x00008EAC00C5C100 AS DateTime))
	INSERT [Booking].[ReservationResourceTimeOfBooking] ([Id], [AffiliateId], [ReservationResourceId], [DayOfWeek], [StartTime], [EndTime]) VALUES (288, 1, 3, 4, CAST(0x00008EAC00C5C100 AS DateTime), CAST(0x00008EAC00CDFE60 AS DateTime))
	INSERT [Booking].[ReservationResourceTimeOfBooking] ([Id], [AffiliateId], [ReservationResourceId], [DayOfWeek], [StartTime], [EndTime]) VALUES (289, 1, 3, 4, CAST(0x00008EAC00CDFE60 AS DateTime), CAST(0x00008EAC00D63BC0 AS DateTime))
	INSERT [Booking].[ReservationResourceTimeOfBooking] ([Id], [AffiliateId], [ReservationResourceId], [DayOfWeek], [StartTime], [EndTime]) VALUES (290, 1, 3, 4, CAST(0x00008EAC00D63BC0 AS DateTime), CAST(0x00008EAC00DE7920 AS DateTime))
	INSERT [Booking].[ReservationResourceTimeOfBooking] ([Id], [AffiliateId], [ReservationResourceId], [DayOfWeek], [StartTime], [EndTime]) VALUES (291, 1, 3, 4, CAST(0x00008EAC00DE7920 AS DateTime), CAST(0x00008EAC00E6B680 AS DateTime))
	INSERT [Booking].[ReservationResourceTimeOfBooking] ([Id], [AffiliateId], [ReservationResourceId], [DayOfWeek], [StartTime], [EndTime]) VALUES (292, 1, 3, 4, CAST(0x00008EAC00E6B680 AS DateTime), CAST(0x00008EAC00EEF3E0 AS DateTime))
	INSERT [Booking].[ReservationResourceTimeOfBooking] ([Id], [AffiliateId], [ReservationResourceId], [DayOfWeek], [StartTime], [EndTime]) VALUES (293, 1, 3, 4, CAST(0x00008EAC00EEF3E0 AS DateTime), CAST(0x00008EAC00F73140 AS DateTime))
	INSERT [Booking].[ReservationResourceTimeOfBooking] ([Id], [AffiliateId], [ReservationResourceId], [DayOfWeek], [StartTime], [EndTime]) VALUES (294, 1, 3, 4, CAST(0x00008EAC00F73140 AS DateTime), CAST(0x00008EAC00FF6EA0 AS DateTime))
	INSERT [Booking].[ReservationResourceTimeOfBooking] ([Id], [AffiliateId], [ReservationResourceId], [DayOfWeek], [StartTime], [EndTime]) VALUES (295, 1, 3, 4, CAST(0x00008EAC00FF6EA0 AS DateTime), CAST(0x00008EAC0107AC00 AS DateTime))
	INSERT [Booking].[ReservationResourceTimeOfBooking] ([Id], [AffiliateId], [ReservationResourceId], [DayOfWeek], [StartTime], [EndTime]) VALUES (296, 1, 3, 4, CAST(0x00008EAC0107AC00 AS DateTime), CAST(0x00008EAC010FE960 AS DateTime))
	INSERT [Booking].[ReservationResourceTimeOfBooking] ([Id], [AffiliateId], [ReservationResourceId], [DayOfWeek], [StartTime], [EndTime]) VALUES (297, 1, 3, 6, CAST(0x00008EAC009450C0 AS DateTime), CAST(0x00008EAC009C8E20 AS DateTime))
	INSERT [Booking].[ReservationResourceTimeOfBooking] ([Id], [AffiliateId], [ReservationResourceId], [DayOfWeek], [StartTime], [EndTime]) VALUES (298, 1, 3, 6, CAST(0x00008EAC009C8E20 AS DateTime), CAST(0x00008EAC00A4CB80 AS DateTime))
	INSERT [Booking].[ReservationResourceTimeOfBooking] ([Id], [AffiliateId], [ReservationResourceId], [DayOfWeek], [StartTime], [EndTime]) VALUES (299, 1, 3, 6, CAST(0x00008EAC00A4CB80 AS DateTime), CAST(0x00008EAC00AD08E0 AS DateTime))
	INSERT [Booking].[ReservationResourceTimeOfBooking] ([Id], [AffiliateId], [ReservationResourceId], [DayOfWeek], [StartTime], [EndTime]) VALUES (300, 1, 3, 6, CAST(0x00008EAC00AD08E0 AS DateTime), CAST(0x00008EAC00B54640 AS DateTime))
	INSERT [Booking].[ReservationResourceTimeOfBooking] ([Id], [AffiliateId], [ReservationResourceId], [DayOfWeek], [StartTime], [EndTime]) VALUES (301, 1, 3, 6, CAST(0x00008EAC00B54640 AS DateTime), CAST(0x00008EAC00BD83A0 AS DateTime))
	INSERT [Booking].[ReservationResourceTimeOfBooking] ([Id], [AffiliateId], [ReservationResourceId], [DayOfWeek], [StartTime], [EndTime]) VALUES (302, 1, 3, 6, CAST(0x00008EAC00BD83A0 AS DateTime), CAST(0x00008EAC00C5C100 AS DateTime))
	INSERT [Booking].[ReservationResourceTimeOfBooking] ([Id], [AffiliateId], [ReservationResourceId], [DayOfWeek], [StartTime], [EndTime]) VALUES (303, 1, 3, 6, CAST(0x00008EAC00C5C100 AS DateTime), CAST(0x00008EAC00CDFE60 AS DateTime))
	INSERT [Booking].[ReservationResourceTimeOfBooking] ([Id], [AffiliateId], [ReservationResourceId], [DayOfWeek], [StartTime], [EndTime]) VALUES (304, 1, 3, 6, CAST(0x00008EAC00CDFE60 AS DateTime), CAST(0x00008EAC00D63BC0 AS DateTime))
	INSERT [Booking].[ReservationResourceTimeOfBooking] ([Id], [AffiliateId], [ReservationResourceId], [DayOfWeek], [StartTime], [EndTime]) VALUES (305, 1, 3, 6, CAST(0x00008EAC00D63BC0 AS DateTime), CAST(0x00008EAC00DE7920 AS DateTime))
	INSERT [Booking].[ReservationResourceTimeOfBooking] ([Id], [AffiliateId], [ReservationResourceId], [DayOfWeek], [StartTime], [EndTime]) VALUES (306, 1, 3, 6, CAST(0x00008EAC00DE7920 AS DateTime), CAST(0x00008EAC00E6B680 AS DateTime))
	INSERT [Booking].[ReservationResourceTimeOfBooking] ([Id], [AffiliateId], [ReservationResourceId], [DayOfWeek], [StartTime], [EndTime]) VALUES (307, 1, 3, 6, CAST(0x00008EAC00E6B680 AS DateTime), CAST(0x00008EAC00EEF3E0 AS DateTime))
	INSERT [Booking].[ReservationResourceTimeOfBooking] ([Id], [AffiliateId], [ReservationResourceId], [DayOfWeek], [StartTime], [EndTime]) VALUES (308, 1, 3, 6, CAST(0x00008EAC00EEF3E0 AS DateTime), CAST(0x00008EAC00F73140 AS DateTime))
	INSERT [Booking].[ReservationResourceTimeOfBooking] ([Id], [AffiliateId], [ReservationResourceId], [DayOfWeek], [StartTime], [EndTime]) VALUES (309, 1, 3, 6, CAST(0x00008EAC00F73140 AS DateTime), CAST(0x00008EAC00FF6EA0 AS DateTime))
	INSERT [Booking].[ReservationResourceTimeOfBooking] ([Id], [AffiliateId], [ReservationResourceId], [DayOfWeek], [StartTime], [EndTime]) VALUES (310, 1, 3, 6, CAST(0x00008EAC00FF6EA0 AS DateTime), CAST(0x00008EAC0107AC00 AS DateTime))
	INSERT [Booking].[ReservationResourceTimeOfBooking] ([Id], [AffiliateId], [ReservationResourceId], [DayOfWeek], [StartTime], [EndTime]) VALUES (311, 1, 3, 6, CAST(0x00008EAC0107AC00 AS DateTime), CAST(0x00008EAC010FE960 AS DateTime))
	SET IDENTITY_INSERT [Booking].[ReservationResourceTimeOfBooking] OFF

	INSERT [Booking].[ReservationResourceService] ([AffiliateId], [ReservationResourceId], [ServiceId]) VALUES (1, 1, 1)
	INSERT [Booking].[ReservationResourceService] ([AffiliateId], [ReservationResourceId], [ServiceId]) VALUES (1, 1, 2)
	INSERT [Booking].[ReservationResourceService] ([AffiliateId], [ReservationResourceId], [ServiceId]) VALUES (1, 1, 3)
	INSERT [Booking].[ReservationResourceService] ([AffiliateId], [ReservationResourceId], [ServiceId]) VALUES (1, 1, 4)
	INSERT [Booking].[ReservationResourceService] ([AffiliateId], [ReservationResourceId], [ServiceId]) VALUES (1, 1, 5)
	INSERT [Booking].[ReservationResourceService] ([AffiliateId], [ReservationResourceId], [ServiceId]) VALUES (1, 1, 6)
	INSERT [Booking].[ReservationResourceService] ([AffiliateId], [ReservationResourceId], [ServiceId]) VALUES (1, 2, 1)
	INSERT [Booking].[ReservationResourceService] ([AffiliateId], [ReservationResourceId], [ServiceId]) VALUES (1, 2, 2)
	INSERT [Booking].[ReservationResourceService] ([AffiliateId], [ReservationResourceId], [ServiceId]) VALUES (1, 2, 3)
	INSERT [Booking].[ReservationResourceService] ([AffiliateId], [ReservationResourceId], [ServiceId]) VALUES (1, 2, 4)
	INSERT [Booking].[ReservationResourceService] ([AffiliateId], [ReservationResourceId], [ServiceId]) VALUES (1, 2, 5)
	INSERT [Booking].[ReservationResourceService] ([AffiliateId], [ReservationResourceId], [ServiceId]) VALUES (1, 2, 6)
	INSERT [Booking].[ReservationResourceService] ([AffiliateId], [ReservationResourceId], [ServiceId]) VALUES (1, 3, 1)
	INSERT [Booking].[ReservationResourceService] ([AffiliateId], [ReservationResourceId], [ServiceId]) VALUES (1, 3, 2)
	INSERT [Booking].[ReservationResourceService] ([AffiliateId], [ReservationResourceId], [ServiceId]) VALUES (1, 3, 3)
	INSERT [Booking].[ReservationResourceService] ([AffiliateId], [ReservationResourceId], [ServiceId]) VALUES (1, 3, 4)
	INSERT [Booking].[ReservationResourceService] ([AffiliateId], [ReservationResourceId], [ServiceId]) VALUES (1, 3, 5)
	INSERT [Booking].[ReservationResourceService] ([AffiliateId], [ReservationResourceId], [ServiceId]) VALUES (1, 3, 6)

	INSERT [Booking].[AffiliateReservationResource] ([AffiliateId], [ReservationResourceId], [BookingIntervalMinutes]) VALUES (1, 1, NULL)
	INSERT [Booking].[AffiliateReservationResource] ([AffiliateId], [ReservationResourceId], [BookingIntervalMinutes]) VALUES (1, 2, NULL)
	INSERT [Booking].[AffiliateReservationResource] ([AffiliateId], [ReservationResourceId], [BookingIntervalMinutes]) VALUES (1, 3, NULL)
END

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.BizProcessRules.FilterTypeName.MessageReply','Условие отбора социальной сети/сервиса')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.BizProcessRules.FilterTypeName.MessageReply','Condition of social network/service selection')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Crm.EMessageReplyFieldType.None','Любой')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Crm.EMessageReplyFieldType.None','Any')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Crm.EMessageReplyFieldType.Vk','Вконтакте')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Crm.EMessageReplyFieldType.Vk','vk.com')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Crm.EMessageReplyFieldType.Facebook','Facebook')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Crm.EMessageReplyFieldType.Facebook','Facebook')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Crm.EMessageReplyFieldType.Instagram','Instagram')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Crm.EMessageReplyFieldType.Instagram','Instagram')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Crm.EMessageReplyFieldType.Telegram','Telegram')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Crm.EMessageReplyFieldType.Telegram','Telegram')

Update [Settings].[Localization] 
Set [ResourceValue] = 'Cообщение из Вконтакте, Facebook, Instagram, Telegram' 
WHERE [ResourceKey] = 'Core.Services.EBizProcessEventType.MessageReply' and [LanguageId] = 1

Update [Settings].[Localization] 
Set [ResourceValue] = 'Message from social network / service' 
WHERE [ResourceKey] = 'Core.Services.EBizProcessEventType.MessageReply' and [LanguageId] = 2

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.Tasks.ModalEditTaskCtrl.WarningMissingTitle','Редактирование задачи')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.Tasks.ModalEditTaskCtrl.WarningMissingTitle','Editing a task')

GO--

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Не используйте эту настройку для выгрузки на Яндекс.Маркет!<br><br>При установке галочки будут выгружаться товары, не доступные для покупки и товары, не разрешённые под заказ.' WHERE [ResourceKey] = 'Admin.ExportFeeds.YandexMarket.UnavailableProducts' AND [LanguageId] = 1

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Do not use this setting for Yandex.Market export!<br><br>This setting will export products, which are not available and which are not allowed for preorder.' WHERE [ResourceKey] = 'Admin.ExportFeeds.YandexMarket.UnavailableProducts' AND [LanguageId] = 2

GO--
if( (Select Count(ResourceKey) From [Settings].[Localization] where ResourceKey = 'Admin.PaymentMethods.RbkMoney2.ShopId') = 0 )
begin
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.PaymentMethods.RbkMoney2.ShopId','Идентификатор магазина')
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.PaymentMethods.RbkMoney2.ShopId','Shop ID')
end
if( (Select Count(ResourceKey) From [Settings].[Localization] where ResourceKey = 'Admin.PaymentMethods.RbkMoney2.ApiKey') = 0 )
begin
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.PaymentMethods.RbkMoney2.ApiKey','API ключ')
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.PaymentMethods.RbkMoney2.ApiKey','API key')
end
if( (Select Count(ResourceKey) From [Settings].[Localization] where ResourceKey = 'Admin.SettingsCrm.Index.ShowWhatsApp') = 0 )
begin
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.SettingsCrm.Index.ShowWhatsApp','Показывать WhatsApp')
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.SettingsCrm.Index.ShowWhatsApp','Show WhatsApp')
end
if( (Select Count(ResourceKey) From [Settings].[Localization] where ResourceKey = 'Admin.SettingsCrm.Index.ShowViber') = 0 )
begin
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.SettingsCrm.Index.ShowViber','Показывать Viber')
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.SettingsCrm.Index.ShowViber','Show Viber')
end
if( (Select Count(ResourceKey) From [Settings].[Localization] where ResourceKey = 'Admin.SettingsCrm.Index.ShowOdnoklassniki') = 0 )
begin
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.SettingsCrm.Index.ShowOdnoklassniki','Показывать Одноклассники')
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.SettingsCrm.Index.ShowOdnoklassniki','Show OK.RU')
end
if( (Select Count(ResourceKey) From [Settings].[Localization] where ResourceKey = 'Admin.Settings.Checkout.ThankYouPage') = 0 )
begin
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Checkout.ThankYouPage','Страница благодарности')
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Checkout.ThankYouPage','Thank You Page')
end
if( (Select Count(ResourceKey) From [Settings].[Localization] where ResourceKey = 'Admin.Settings.Checkout.ThankYouPageAction') = 0 )
begin
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Checkout.ThankYouPageAction','Действие')
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Checkout.ThankYouPageAction','Action')
end
if( (Select Count(ResourceKey) From [Settings].[Localization] where ResourceKey = 'Admin.Js.Common.AreYouSureDelete') = 0 )
begin
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.Common.AreYouSureDelete','Вы уверены, что хотите удалить?')
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.Common.AreYouSureDelete','Are you sure you want to delete?')
end
if( (Select Count(ResourceKey) From [Settings].[Localization] where ResourceKey = 'Admin.Js.Common.Deleting') = 0 )
begin
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.Common.Deleting','Удаление')
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.Common.Deleting','Deleting')
end
if( (Select Count(ResourceKey) From [Settings].[Localization] where ResourceKey = 'Admin.Js.Common.ChangesSaved') = 0 )
begin
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.Common.ChangesSaved','Изменения сохранены')
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.Common.ChangesSaved','Changes saved')
end
if( (Select Count(ResourceKey) From [Settings].[Localization] where ResourceKey = 'Core.Configuration.EThankYouPageActionType.None') = 0 )
begin
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Configuration.EThankYouPageActionType.None','Действие не указано')
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Configuration.EThankYouPageActionType.None','Action not specified')
end
if( (Select Count(ResourceKey) From [Settings].[Localization] where ResourceKey = 'Core.Configuration.EThankYouPageActionType.JoinGroup') = 0 )
begin
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Configuration.EThankYouPageActionType.JoinGroup','Призыв вступить в группу в соцсетях')
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Configuration.EThankYouPageActionType.JoinGroup','Join group in social networks')
end
if( (Select Count(ResourceKey) From [Settings].[Localization] where ResourceKey = 'Core.Configuration.EThankYouPageActionType.ShowProducts') = 0 )
begin
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Configuration.EThankYouPageActionType.ShowProducts','Показ товаров')
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Configuration.EThankYouPageActionType.ShowProducts','Show products')
end
if( (Select Count(ResourceKey) From [Settings].[Localization] where ResourceKey = 'Core.Configuration.EThankYouPageActionType.Share') = 0 )
begin
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Configuration.EThankYouPageActionType.Share','Просьба поделиться в соцсетях')
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Configuration.EThankYouPageActionType.Share','Share in social networks')
end
if( (Select Count(ResourceKey) From [Settings].[Localization] where ResourceKey = 'Core.Configuration.ESocialNetworkType.Vk') = 0 )
begin
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Configuration.ESocialNetworkType.Vk','ВКонтакте')
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Configuration.ESocialNetworkType.Vk','VK')
end
if( (Select Count(ResourceKey) From [Settings].[Localization] where ResourceKey = 'Core.Configuration.ESocialNetworkType.Facebook') = 0 )
begin
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Configuration.ESocialNetworkType.Facebook','Facebook')
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Configuration.ESocialNetworkType.Facebook','Facebook')
end
if( (Select Count(ResourceKey) From [Settings].[Localization] where ResourceKey = 'Core.Configuration.ESocialNetworkType.Youtube') = 0 )
begin
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Configuration.ESocialNetworkType.Youtube','YouTube')
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Configuration.ESocialNetworkType.Youtube','YouTube')
end
if( (Select Count(ResourceKey) From [Settings].[Localization] where ResourceKey = 'Core.Configuration.ESocialNetworkType.Twitter') = 0 )
begin
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Configuration.ESocialNetworkType.Twitter','Twitter')
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Configuration.ESocialNetworkType.Twitter','Twitter')
end
if( (Select Count(ResourceKey) From [Settings].[Localization] where ResourceKey = 'Core.Configuration.ESocialNetworkType.Instagram') = 0 )
begin
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Configuration.ESocialNetworkType.Instagram','Instagram')
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Configuration.ESocialNetworkType.Instagram','Instagram')
end
if( (Select Count(ResourceKey) From [Settings].[Localization] where ResourceKey = 'Core.Configuration.ESocialNetworkType.Telegram') = 0 )
begin
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Configuration.ESocialNetworkType.Telegram','Telegram')
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Configuration.ESocialNetworkType.Telegram','Telegram')
end
if( (Select Count(ResourceKey) From [Settings].[Localization] where ResourceKey = 'Core.Configuration.ESocialNetworkType.Odnoklassniki') = 0 )
begin
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Configuration.ESocialNetworkType.Odnoklassniki','Одноклассники')
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Configuration.ESocialNetworkType.Odnoklassniki','OK.RU')
end
if( (Select Count(ResourceKey) From [Settings].[Localization] where ResourceKey = 'Admin.Js.Landing.MakeActive') = 0 )
begin
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.Landing.MakeActive','Сделать активными')
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.Landing.MakeActive','Make active')
end
if( (Select Count(ResourceKey) From [Settings].[Localization] where ResourceKey = 'Admin.Js.Landing.MakeInactive') = 0 )
begin
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.Landing.MakeInactive','Сделать неактивными')
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.Landing.MakeInactive','Make inactive')
end
if( (Select Count(ResourceKey) From [Settings].[Localization] where ResourceKey = 'Admin.SettingsTelephony.DontCreateLeadFromCall') = 0 )
begin
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.SettingsTelephony.DontCreateLeadFromCall','Не создавать лид при новом входящем звонке')
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.SettingsTelephony.DontCreateLeadFromCall','Do not create lead from new incoming call')
end
if( (Select Count(ResourceKey) From [Settings].[Localization] where ResourceKey = 'Admin.SettingsTelephony.CallsSalesFunnel') = 0 )
begin
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.SettingsTelephony.CallsSalesFunnel','Воронка для создания лидов')
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.SettingsTelephony.CallsSalesFunnel','Sales funnel for new leads')
end
if( (Select Count(ResourceKey) From [Settings].[Localization] where ResourceKey = 'Admin.SettingsTelephony.DefaultCallsSalesFunnel') = 0 )
begin
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.SettingsTelephony.DefaultCallsSalesFunnel','Воронка по умолчанию')
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.SettingsTelephony.DefaultCallsSalesFunnel','Default sales funnel')
end
if( (Select Count(ResourceKey) From [Settings].[Localization] where ResourceKey = 'Admin.SettingsSystem.SystemCommon.ShowInProductReview') = 0 )
begin
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.SettingsSystem.SystemCommon.ShowInProductReview','Показывать при создании отзыва о товаре')
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.SettingsSystem.SystemCommon.ShowInProductReview','Show when creating a review of a product')
end
if( (Select Count(ResourceKey) From [Settings].[Localization] where ResourceKey = 'Js.Captcha.Wrong') = 0 )
begin
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Js.Captcha.Wrong','Неверно введен код')
	INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Js.Captcha.Wrong','Wrong captcha text')
end

GO--

UPDATE [Settings].[InternalSettings] SET [settingValue] = '6.5.4' WHERE [settingKey] = 'db_version'

