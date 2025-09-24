
GO--

if not Exists (select * from settings.settings where name='ActiveLandingPage')
begin
	insert into settings.settings (name, value) values ('ActiveLandingPage', 'False')
end

GO--

update [Settings].[Localization]  set ResourceKey = 'Admin.Common.SaasBlock.Test' where ResourceKey='Admin.Common.SaasBlock.Test"'


GO--

update [Settings].[Localization]  set ResourceValue = 'Есть ли у вас точки продаж в офлайне?' where ResourceKey='Admin.Js.UserInfoPopup.HaveOfflineSalePoint'

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ShippingMethods.Grastin.MarkupInBaseCurrency','Наценка в базовой валюте')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ShippingMethods.Grastin.MarkupInBaseCurrency','Markup in base currency')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ShippingMethods.Grastin.EnterTheMarkupInBaseCurrency','Введите наценку в базовой валюте')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ShippingMethods.Grastin.EnterTheMarkupInBaseCurrency','Enter a markup in the base currency')

GO--

Update [Settings].[Localization] Set ResourceValue = 'Название воронки' Where [LanguageId]=1 and [ResourceKey]='Admin.Js.AddLanding.Name'
Update [Settings].[Localization] Set ResourceValue = 'Funnel name' Where [LanguageId]=2 and [ResourceKey]='Admin.Js.AddLanding.Name'

Update [Settings].[Localization] Set ResourceValue = 'Создание воронки' Where [LanguageId]=1 and [ResourceKey]='Admin.Js.AddLanding.Title'
Update [Settings].[Localization] Set ResourceValue = 'Creating a funnel' Where [LanguageId]=2 and [ResourceKey]='Admin.Js.AddLanding.Title'

Update [Settings].[Localization]  set ResourceValue = 'Воронки' where ResourceKey='Admin.Home.Menu.Landing'

GO--

if not Exists (select * from settings.settings where name='SettingsCrm.CreateLeadFromCall')
begin
	insert into settings.settings (name, value) values ('SettingsCrm.CreateLeadFromCall', 'True')
end

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.CustomerSegments.Filters.HaveTelegramAccount','Есть аккаунт Telegram')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.CustomerSegments.Filters.HaveTelegramAccount','Have an account on Telegram')

Update [Settings].[Localization] Set ResourceValue = 'Есть аккаунт ВКонтакте, Instagram, Facebook или Telegram' Where ResourceKey = 'Admin.CustomerSegments.Filters.HaveAnAccount' and LanguageId = 1 
Update [Settings].[Localization] Set ResourceValue = 'Have an account on Facebook, Instagram, Facebook or Telegram' Where ResourceKey = 'Admin.CustomerSegments.Filters.HaveAnAccount' and LanguageId = 2 

Update [Settings].[Localization] Set ResourceValue = 'Есть аккаунт ВКонтакте, Instagram, Facebook или Telegram' Where ResourceKey = 'Admin.Js.Customers.HaveAccountOnVKandFB' and LanguageId = 1 
Update [Settings].[Localization] Set ResourceValue = 'Have an account on Facebook, Instagram, Facebook or Telegram' Where ResourceKey = 'Admin.Js.Customers.HaveAccountOnVKandFB' and LanguageId = 2 

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ShippingMethods.Sdek.StoreAddress','Адрес забора посылок')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ShippingMethods.Sdek.StoreAddress','Parcel picking address')

Update [Settings].[Localization] Set ResourceValue = 'Город' Where ResourceKey = 'Admin.ShippingMethods.Sdek.CityOfOnlineStore' and LanguageId = 1 
Update [Settings].[Localization] Set ResourceValue = 'City' Where ResourceKey = 'Admin.ShippingMethods.Sdek.CityOfOnlineStore' and LanguageId = 2 

GO--

Update [Settings].[Localization] Set ResourceValue = 'Комментарий к статусу' Where ResourceKey = 'Admin.Js.ChangeOrderStatus.Basis' and LanguageId = 1 
Update [Settings].[Localization] Set ResourceValue = 'Status comment' Where ResourceKey = 'Admin.Js.ChangeOrderStatus.Basis' and LanguageId = 2 

Update [Settings].[Localization] Set ResourceValue = 'Комментарий к статусу' Where ResourceKey = 'Admin.Js.Order.Base' and LanguageId = 1 
Update [Settings].[Localization] Set ResourceValue = 'Status comment' Where ResourceKey = 'Admin.Js.Order.Base' and LanguageId = 2 

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.Customers.SendTelegramMessage','Отправить сообщение в Telegram')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.Customers.SendTelegramMessage','Send message to telegram')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.Tasks.ModalEditTaskCtrl.WarningMissingData','Внесенные изменения задачи будут потеряны')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.Tasks.ModalEditTaskCtrl.WarningMissingData','Changes to the task will be lost')

GO--

Update [Settings].[Localization] Set ResourceValue = 'Ресурсы' Where [LanguageId]=1 and [ResourceKey]='Admin.BookingCategory.View.Employees'
Update [Settings].[Localization] Set ResourceValue = 'Resources' Where [LanguageId]=2 and [ResourceKey]='Admin.BookingCategory.View.Employees'

Update [Settings].[Localization] Set ResourceValue = 'Ресурсы' Where [LanguageId]=1 and [ResourceKey]='Admin.Booking.NavMenu.Employees'
Update [Settings].[Localization] Set ResourceValue = 'Resources' Where [LanguageId]=2 and [ResourceKey]='Admin.Booking.NavMenu.Employees'

Update [Settings].[Localization] Set ResourceValue = 'Добавить ресурс' Where [LanguageId]=1 and [ResourceKey]='Admin.BookingEmployees.Index.AddEmployee'
Update [Settings].[Localization] Set ResourceValue = 'Add resource' Where [LanguageId]=2 and [ResourceKey]='Admin.BookingEmployees.Index.AddEmployee'

Update [Settings].[Localization] Set ResourceValue = 'Менеджер' Where [LanguageId]=1 and [ResourceKey]='Admin.Js.AddUpdateBookingEmployee.Employee'
Update [Settings].[Localization] Set ResourceValue = 'Manager' Where [LanguageId]=2 and [ResourceKey]='Admin.Js.AddUpdateBookingEmployee.Employee'

Update [Settings].[Localization] Set ResourceValue = 'Редактирование ресурса' Where [LanguageId]=1 and [ResourceKey]='Admin.Js.AddUpdateBookingEmployee.Edit'
Update [Settings].[Localization] Set ResourceValue = 'Editing an resource' Where [LanguageId]=2 and [ResourceKey]='Admin.Js.AddUpdateBookingEmployee.Edit'

Update [Settings].[Localization] Set ResourceValue = 'Ресурс' Where [LanguageId]=1 and [ResourceKey]='Admin.Js.AddUpdateBooking.Employee'
Update [Settings].[Localization] Set ResourceValue = 'Reservation resource' Where [LanguageId]=2 and [ResourceKey]='Admin.Js.AddUpdateBooking.Employee'

Update [Settings].[Localization] Set ResourceValue = 'Ресурс успешно добавлен' Where [LanguageId]=1 and [ResourceKey]='Admin.Js.BookingUsers.EmployeeSuccessfullyAdded'
Update [Settings].[Localization] Set ResourceValue = 'Reservation resource successfully added' Where [LanguageId]=2 and [ResourceKey]='Admin.Js.BookingUsers.EmployeeSuccessfullyAdded'

Update [Settings].[Localization] Set ResourceValue = 'Новый ресурс' Where [LanguageId]=1 and [ResourceKey]='Admin.Js.AddUpdateBookingEmployee.Add'
Update [Settings].[Localization] Set ResourceValue = 'New resource' Where [LanguageId]=2 and [ResourceKey]='Admin.Js.AddUpdateBookingEmployee.Add'

GO--

CREATE TABLE [Booking].[ReservationResource](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ManagerId] [int] NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](max) NULL,
	[Image] [nvarchar](100) NULL,
	[Enabled] [bit] NOT NULL,
	[SortOrder] [int] NOT NULL,
 CONSTRAINT [PK_ReservationResource] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO--

ALTER TABLE [Booking].[ReservationResource]  WITH CHECK ADD  CONSTRAINT [FK_ReservationResource_Managers] FOREIGN KEY([ManagerId])
REFERENCES [Customers].[Managers] ([ManagerId])
ON DELETE CASCADE
GO--

ALTER TABLE [Booking].[ReservationResource] CHECK CONSTRAINT [FK_ReservationResource_Managers]
GO--


CREATE TABLE [Booking].[AffiliateReservationResource](
	[AffiliateId] [int] NOT NULL,
	[ReservationResourceId] [int] NOT NULL,
	[BookingIntervalMinutes] [int] NULL,
 CONSTRAINT [PK_AffiliateReservationResource] PRIMARY KEY CLUSTERED 
(
	[AffiliateId] ASC,
	[ReservationResourceId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO--

ALTER TABLE [Booking].[AffiliateReservationResource]  WITH CHECK ADD  CONSTRAINT [FK_AffiliateReservationResource_Affiliate] FOREIGN KEY([AffiliateId])
REFERENCES [Booking].[Affiliate] ([Id])
ON DELETE CASCADE
GO--

ALTER TABLE [Booking].[AffiliateReservationResource] CHECK CONSTRAINT [FK_AffiliateReservationResource_Affiliate]
GO--

ALTER TABLE [Booking].[AffiliateReservationResource]  WITH CHECK ADD  CONSTRAINT [FK_AffiliateReservationResource_ReservationResource] FOREIGN KEY([ReservationResourceId])
REFERENCES [Booking].[ReservationResource] ([Id])
ON DELETE CASCADE
GO--

ALTER TABLE [Booking].[AffiliateReservationResource] CHECK CONSTRAINT [FK_AffiliateReservationResource_ReservationResource]
GO--


CREATE TABLE [Booking].[ReservationResourceService](
	[AffiliateId] [int] NOT NULL,
	[ReservationResourceId] [int] NOT NULL,
	[ServiceId] [int] NOT NULL,
 CONSTRAINT [PK_ReservationResourceService] PRIMARY KEY CLUSTERED 
(
	[AffiliateId] ASC,
	[ReservationResourceId] ASC,
	[ServiceId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO--

ALTER TABLE [Booking].[ReservationResourceService]  WITH CHECK ADD  CONSTRAINT [FK_ReservationResourceService_Affiliate] FOREIGN KEY([AffiliateId])
REFERENCES [Booking].[Affiliate] ([Id])
ON DELETE CASCADE
GO--

ALTER TABLE [Booking].[ReservationResourceService] CHECK CONSTRAINT [FK_ReservationResourceService_Affiliate]
GO--

ALTER TABLE [Booking].[ReservationResourceService]  WITH CHECK ADD  CONSTRAINT [FK_ReservationResourceService_ReservationResource] FOREIGN KEY([ReservationResourceId])
REFERENCES [Booking].[ReservationResource] ([Id])
ON DELETE CASCADE
GO--

ALTER TABLE [Booking].[ReservationResourceService] CHECK CONSTRAINT [FK_ReservationResourceService_ReservationResource]
GO--

ALTER TABLE [Booking].[ReservationResourceService]  WITH CHECK ADD  CONSTRAINT [FK_ReservationResourceService_Service] FOREIGN KEY([ServiceId])
REFERENCES [Booking].[Service] ([Id])
ON DELETE CASCADE
GO--

ALTER TABLE [Booking].[ReservationResourceService] CHECK CONSTRAINT [FK_ReservationResourceService_Service]
GO--


CREATE TABLE [Booking].[ReservationResourceTimeOfBooking](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[AffiliateId] [int] NOT NULL,
	[ReservationResourceId] [int] NOT NULL,
	[DayOfWeek] [tinyint] NOT NULL,
	[StartTime] [datetime] NOT NULL,
	[EndTime] [datetime] NOT NULL,
 CONSTRAINT [PK_ReservationResourceTimeOfBooking] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO--

ALTER TABLE [Booking].[ReservationResourceTimeOfBooking]  WITH CHECK ADD  CONSTRAINT [FK_ReservationResourceTimeOfBooking_Affiliate] FOREIGN KEY([AffiliateId])
REFERENCES [Booking].[Affiliate] ([Id])
ON DELETE CASCADE
GO--

ALTER TABLE [Booking].[ReservationResourceTimeOfBooking] CHECK CONSTRAINT [FK_ReservationResourceTimeOfBooking_Affiliate]
GO--

ALTER TABLE [Booking].[ReservationResourceTimeOfBooking]  WITH CHECK ADD  CONSTRAINT [FK_ReservationResourceTimeOfBooking_ReservationResource] FOREIGN KEY([ReservationResourceId])
REFERENCES [Booking].[ReservationResource] ([Id])
ON DELETE CASCADE
GO--

ALTER TABLE [Booking].[ReservationResourceTimeOfBooking] CHECK CONSTRAINT [FK_ReservationResourceTimeOfBooking_ReservationResource]
GO--

CREATE NONCLUSTERED INDEX [AffiliateAndRR_ReservationResourceTimeOfBooking] ON [Booking].[ReservationResourceTimeOfBooking]
(
	[AffiliateId] ASC,
	[ReservationResourceId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO--


CREATE TABLE [Booking].[ReservationResourceAdditionalTime](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[AffiliateId] [int] NOT NULL,
	[ReservationResourceId] [int] NOT NULL,
	[StartTime] [datetime] NOT NULL,
	[EndTime] [datetime] NOT NULL,
	[IsWork] [bit] NOT NULL,
 CONSTRAINT [PK_ReservationResourceAdditionalTime] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO--

ALTER TABLE [Booking].[ReservationResourceAdditionalTime]  WITH CHECK ADD  CONSTRAINT [FK_ReservationResourceAdditionalTime_Affiliate] FOREIGN KEY([AffiliateId])
REFERENCES [Booking].[Affiliate] ([Id])
ON DELETE CASCADE
GO--

ALTER TABLE [Booking].[ReservationResourceAdditionalTime] CHECK CONSTRAINT [FK_ReservationResourceAdditionalTime_Affiliate]
GO--

ALTER TABLE [Booking].[ReservationResourceAdditionalTime]  WITH CHECK ADD  CONSTRAINT [FK_ReservationResourceAdditionalTime_ReservationResource] FOREIGN KEY([ReservationResourceId])
REFERENCES [Booking].[ReservationResource] ([Id])
ON DELETE CASCADE
GO--

ALTER TABLE [Booking].[ReservationResourceAdditionalTime] CHECK CONSTRAINT [FK_ReservationResourceAdditionalTime_ReservationResource]
GO--

CREATE NONCLUSTERED INDEX [AffiliateAndRR_ReservationResourceAdditionalTime] ON [Booking].[ReservationResourceAdditionalTime]
(
	[AffiliateId] ASC,
	[ReservationResourceId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO--



ALTER TABLE Booking.Booking ADD
	ReservationResourceId int NULL

GO--

Declare @Id INT, @AffiliateId INT, @CustomerId uniqueidentifier, @BookingIntervalMinutes INT, @Active bit,
@ManagerId INT, @Name nvarchar(255), @Description nvarchar(MAX), @DepartmentName nvarchar(MAX), @Position nvarchar(MAX),
@ReservationResourceId INT;

DECLARE UpdateBookingEmployeeCursor CURSOR LOCAL FORWARD_ONLY STATIC FOR
SELECT [Id],[AffiliateId],[CustomerId],[BookingIntervalMinutes],[Active] FROM [Booking].[Employees];

Open UpdateBookingEmployeeCursor;
FETCH NEXT FROM UpdateBookingEmployeeCursor  
into @Id, @AffiliateId, @CustomerId, @BookingIntervalMinutes, @Active;


WHILE @@FETCH_STATUS = 0
BEGIN
	SET @Description = ''
	SET @ManagerId = NULL
	SET @Name = ''

	SELECT @ManagerId = [ManagerId], @Position = [Position], @DepartmentName = (SELECT [Name] FROM [Customers].[Departments] WHERE [Departments].DepartmentId = [Managers].[DepartmentId]) FROM [Customers].[Managers] WHERE [CustomerId] = @CustomerId
	SELECT @Name = Ltrim(ISNULL([LastName], '') + (CASE [FirstName] WHEN NULL THEN '' WHEN '' THEN '' ELSE ' ' END) + ISNULL([FirstName], '')) FROM [Customers].[Customer] WHERE [CustomerID] = @CustomerId
	SET @Description = ISNULL(@DepartmentName, '') + (CASE LEN(ISNULL(@DepartmentName, '') + ISNULL(@Position, '')) WHEN 0 THEN '' ELSE ' / ' END) + ISNULL(@Position, '')

	INSERT INTO [Booking].[ReservationResource]
			   ([ManagerId]
			   ,[Name]
			   ,[Description]
			   ,[Enabled]
			   ,[SortOrder])
		 VALUES
			   (@ManagerId
			   ,@Name
			   ,@Description
			   ,@Active
			   ,0);
	SELECT @ReservationResourceId = SCOPE_IDENTITY();

	INSERT INTO [Booking].[AffiliateReservationResource] ([AffiliateId],[ReservationResourceId],[BookingIntervalMinutes])
	VALUES (@AffiliateId, @ReservationResourceId, @BookingIntervalMinutes)

	INSERT INTO [Booking].[ReservationResourceService] ([AffiliateId],[ReservationResourceId],[ServiceId])
	SELECT @AffiliateId, @ReservationResourceId, [ServiceId] FROM [Booking].[EmployeeService] WHERE [AffiliateId] = @AffiliateId AND [CustomerId] = @CustomerId

	INSERT INTO [Booking].[ReservationResourceTimeOfBooking] ([AffiliateId],[ReservationResourceId],[DayOfWeek],[StartTime],[EndTime])
	SELECT @AffiliateId, @ReservationResourceId, [DayOfWeek],[StartTime],[EndTime] FROM [Booking].[EmployeeTimeOfBooking] WHERE [EmployeeId] = @Id

	INSERT INTO [Booking].[ReservationResourceAdditionalTime] ([AffiliateId],[ReservationResourceId],[StartTime],[EndTime],[IsWork])
	SELECT @AffiliateId, @ReservationResourceId,[StartTime],[EndTime],[IsWork] FROM [Booking].[EmployeeAdditionalTime] WHERE [EmployeeId] = @Id

	UPDATE [Booking].[Booking]
	   SET [ReservationResourceId] = @ReservationResourceId
	WHERE [EmployeeId] = @CustomerId
		
	FETCH NEXT FROM UpdateBookingEmployeeCursor  
	into @Id, @AffiliateId, @CustomerId, @BookingIntervalMinutes, @Active;
		
END

CLOSE UpdateBookingEmployeeCursor
DEALLOCATE UpdateBookingEmployeeCursor

GO-- 

ALTER TABLE [Booking].[Booking] DROP CONSTRAINT [FK_Booking_Employee]

GO--

ALTER TABLE Booking.Booking ADD CONSTRAINT
	FK_Booking_ReservationResource FOREIGN KEY
	(
	ReservationResourceId
	) REFERENCES Booking.ReservationResource
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  SET NULL 

GO--

ALTER TABLE [Booking].[Booking] DROP CONSTRAINT [FK_Booking_Affiliate]

GO--

ALTER TABLE [Booking].[Booking]  WITH CHECK ADD  CONSTRAINT [FK_Booking_Affiliate] FOREIGN KEY([AffiliateId])
REFERENCES [Booking].[Affiliate] ([Id])
ON DELETE CASCADE

GO--

ALTER TABLE [Booking].[Booking] CHECK CONSTRAINT [FK_Booking_Affiliate]

GO--

DROP TABLE [Booking].[EmployeeService]

GO-- 

DROP TABLE [Booking].[EmployeeAdditionalTime]

GO-- 

DROP TABLE [Booking].[EmployeeTimeOfBooking]

GO-- 

DROP TABLE [Booking].[Employees]

GO-- 

ALTER TABLE Booking.Booking  DROP COLUMN EmployeeId

GO--


update [Settings].[Localization] set [ResourceKey] = 'Admin.ExportFeeed.ChoiceOfProducts.Export' where ResourceKey = 'Admin.ExportFeeed.СhoiceOfProducts.Export'
update [Settings].[Localization] set [ResourceKey] = 'Admin.ExportFeeed.ChoiceOfProducts.AllProducts' where ResourceKey = 'Admin.ExportFeeed.СhoiceOfProducts.AllProducts'
update [Settings].[Localization] set [ResourceKey] = 'Admin.ExportFeeed.ChoiceOfProducts.ExportCatalog' where ResourceKey = 'Admin.ExportFeeed.СhoiceOfProducts.ExportCatalog'

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Booking','Бронирование')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Booking','Booking')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Booking.Title','Бронирование')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Booking.Title','Booking')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Booking.BookingCommon','Бронирование')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Booking.BookingCommon','Booking')

GO--

INSERT INTO [Settings].[Settings] ([Name],[Value]) VALUES ('BookingCategoryImageWidth','70')
INSERT INTO [Settings].[Settings] ([Name],[Value]) VALUES ('BookingCategoryImageHeight','70')
INSERT INTO [Settings].[Settings] ([Name],[Value]) VALUES ('BookingServiceImageWidth','70')
INSERT INTO [Settings].[Settings] ([Name],[Value]) VALUES ('BookingServiceImageHeight','70')
INSERT INTO [Settings].[Settings] ([Name],[Value]) VALUES ('BookingReservationResourceImageWidth','70')
INSERT INTO [Settings].[Settings] ([Name],[Value]) VALUES ('BookingReservationResourceImageHeight','70')

GO--
ALTER TABLE CMS.LandingSite ADD
	ProductId int NULL
GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Product.LeftMenu.LandingFunnels','Воронки')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Product.LeftMenu.LandingFunnels','Funnels')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Product.LandingFunnels.LandingFunnels','Воронки')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Product.LandingFunnels.LandingFunnels','Funnels')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Product.LandingFunnels.LandingFunnelsDescription','Создайте для товара отдельную посадочную страницу и превратите ее в воронку продаж.')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Product.LandingFunnels.LandingFunnelsDescription','Create a landing page for the product and turn it into a sales funnel.')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Product.LandingFunnels.LandingFunnelsList','Созданные воронки:')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Product.LandingFunnels.LandingFunnelsList','Funnels:')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Product.LandingFunnels.CreateLandingFunnel','Создать воронку')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Product.LandingFunnels.CreateLandingFunnel','Create a funnel')

GO--

Update [Settings].[Localization] Set ResourceValue = 'Добро пожаловать в ADVANTSHOP' Where ResourceKey = 'Admin.Home.Congratulations.Congratulations' and LanguageId = 1 
Update [Settings].[Localization] Set ResourceValue = 'Wellcome to ADVANTSHOP' Where ResourceKey = 'Admin.Home.Congratulations.Congratulations' and LanguageId = 2 

GO--

ALTER TABLE CMS.Landing ADD
	PageType int NULL
GO--

ALTER TABLE CMS.LandingForm ADD
	EmailText nvarchar(MAX) NULL,
	EmailSubject nvarchar(MAX) NULL
GO--


Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (1, 'Admin.Settings.NotifyEMails.EmailForBookings', 'E-mail для уведомлений о новых бронях');
Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (2, 'Admin.Settings.NotifyEMails.EmailForBookings', 'E-mail for new bookings notifications');

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (1, 'Core.Booking.Letter.Service', 'Услуга')
Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (1, 'Core.Booking.Letter.Price', 'Цена')
Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (1, 'Core.Booking.Letter.Count', 'Количество')
Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (1, 'Core.Booking.Letter.Cost', 'Стоимость')
Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (1, 'Core.Booking.Letter.BookingCost', 'Стоимость брони')

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (2, 'Core.Booking.Letter.Service', 'Service')
Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (2, 'Core.Booking.Letter.Price', 'Price')
Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (2, 'Core.Booking.Letter.Count', 'Quantity')
Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (2, 'Core.Booking.Letter.Cost', 'Total')
Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (2, 'Core.Booking.Letter.BookingCost', 'Subtotal')

GO--

DECLARE @MailFormatID INT

INSERT INTO [Settings].[MailFormatType] 
		([TypeName],[SortOrder],[Comment],[MailType])
	VALUES
		('Создание брони'
		,290
		,'Письмо при создании брони (#BOOKING_ID#, #NAME#, #PHONE#, #EMAIL#, #ORDERTABLE#, #STORE_NAME#)'
		,'OnBookingCreated')

SELECT @MailFormatID = SCOPE_IDENTITY()

INSERT INTO [Settings].[MailFormat]
           ([FormatName],[FormatText],[SortOrder],[Enable],[AddDate],[ModifyDate],[FormatSubject],[MailFormatTypeId])
     VALUES
           ('Создание брони'
           ,'<div style="color: #4c4f56; font-family: Arial, Helvetica, sans-serif; font-size: 14px;">
<div class="header" style="border-bottom: 1px solid #ededed; display: table; margin-bottom: 25px; padding-bottom: 25px; width: 100%;">
<div class="logo" style="display: table-cell; text-align: left; vertical-align: middle;">#LOGO#</div>

<div class="phone" style="display: table-cell; text-align: right; vertical-align: middle;">
<div class="tel" style="font-size: 26px; font-weight: bold; line-height: 1; margin-bottom: 5px;">&nbsp;</div>

<div class="inform" style="font-size: 12px;">&nbsp;</div>
</div>
</div>

<div class="data" style="display: table; width: 100%;">
<div class="data-cell" style="display: table-cell; padding: 0; padding-right: 1%; width: 48%;">
<div class="l-row">
<div class="l-name vi cs-light" style="color: #acacac; display: inline-block; margin: 5px 0; padding-right: 15px; width: 150px; vertical-align: middle;">Имя:</div>

<div class="l-value vi" style="display: inline-block; margin: 5px 0;">#NAME#</div>
</div>

<div class="l-row">
<div class="l-name vi cs-light" style="color: #acacac; display: inline-block; margin: 5px 0; padding-right: 15px; width: 150px; vertical-align: middle;">Номер телефона:</div>

<div class="l-value vi" style="display: inline-block; margin: 5px 0;">#PHONE#</div>
</div>

<div class="l-row">
<div class="l-name vi cs-light" style="color: #acacac; display: inline-block; margin: 5px 0; padding-right: 15px; width: 150px; vertical-align: middle;">EMail:</div>

<div class="l-value vi" style="display: inline-block; margin: 5px 0;">#EMAIL#</div>
</div>
</div>
</div>

<div>
<div class="o-big-title" style="font-size: 18px; font-weight: bold; margin-bottom: 20px; margin-top: 40px;">Содержание брони:</div>
#ORDERTABLE#</div>
</div>'
           ,1500
           ,1
           ,GETDATE()
           ,GETDATE()
           ,'Оформлена бронь № #BOOKING_ID#'
           ,@MailFormatID)

GO--


IF NOT EXISTS(SELECT 1 FROM sys.columns WHERE Name = N'PostId' AND Object_ID = Object_ID(N'Customers.VkMessage'))
BEGIN
    ALTER TABLE Customers.VkMessage ADD
		PostId bigint NULL
END

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'App.Landing.Domain.Forms.ELpFormFieldType.Address','Адрес')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'App.Landing.Domain.Forms.ELpFormFieldType.Address','Address')

GO--

ALTER TABLE CMS.LandingForm ADD
	PostMessageRedirectLpId nvarchar(MAX) NULL
GO--




ALTER TABLE Catalog.Brand ADD
	CountryOfManufactureID int NULL
GO--
ALTER PROCEDURE [Catalog].[sp_AddBrand]
	@BrandName nvarchar(MAX),
	@BrandDescription nvarchar(MAX),
	@BrandBriefDescription nvarchar(MAX),
	@Enabled bit,
	@SortOrder int,
	@CountryID int,
	@CountryOfManufactureID int,
	@UrlPath nvarchar(150),
	@BrandSiteUrl nvarchar(150)
AS
BEGIN
	Insert into Catalog.Brand (BrandName, BrandDescription, BrandBriefDescription, Enabled, SortOrder, CountryID,UrlPath,BrandSiteUrl,CountryOfManufactureID)
	                  values (@BrandName, @BrandDescription, @BrandBriefDescription, @Enabled, @SortOrder, @CountryID,@UrlPath,@BrandSiteUrl,@CountryOfManufactureID)
	Select Scope_Identity()
END
GO--
ALTER PROCEDURE [Catalog].[sp_UpdateBrandById]
	@BrandID int,
	@BrandName nvarchar(MAX),
	@BrandDescription nvarchar(MAX),
	@BrandBriefDescription nvarchar(MAX),
	@Enabled bit,
	@SortOrder int,
	@CountryID int,
	@CountryOfManufactureID int,
	@UrlPath nvarchar(150),
	@BrandSiteUrl nvarchar(150)
AS
BEGIN
	Update Catalog.Brand SET BrandName=@BrandName, BrandDescription=@BrandDescription, BrandBriefDescription=@BrandBriefDescription,
		Enabled = @Enabled, SortOrder=@SortOrder, CountryID=@CountryID, UrlPath=@UrlPath, BrandSiteUrl=@BrandSiteUrl, CountryOfManufactureID=@CountryOfManufactureID
	Where BrandID=@BrandID
END
GO--
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Brands.AddEdit.CountryOfManufacture','Страна производства')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Brands.AddEdit.CountryOfManufacture','Country of manufacture')

GO--

ALTER PROCEDURE [Settings].[sp_GetExportFeedProducts] 
     @exportFeedId int
	,@onlyCount BIT
	,@exportNotAvailable bit
	,@selectedCurrency NVARCHAR(10)
	,@allowPreOrder bit = 0
	,@exportAllProducts bit
AS
BEGIN
	DECLARE @res TABLE (productId INT PRIMARY KEY CLUSTERED);
	DECLARE @lproduct TABLE (productId INT PRIMARY KEY CLUSTERED);

	INSERT INTO @lproduct
	SELECT [ProductID]
	FROM [Settings].[ExportFeedSelectedProducts]
	WHERE [ExportFeedId] = @exportFeedId;

	DECLARE @lcategory TABLE (CategoryId INT PRIMARY KEY CLUSTERED);
	DECLARE @l TABLE (CategoryId INT PRIMARY KEY CLUSTERED);

	INSERT INTO @l
	SELECT t.CategoryId
	FROM [Settings].[ExportFeedSelectedCategories] AS t
	INNER JOIN CATALOG.Category ON t.CategoryId = Category.CategoryId
	WHERE [ExportFeedId] = @exportFeedId
		AND HirecalEnabled = 1
		AND Enabled = 1

	DECLARE @l1 INT

	SET @l1 = (
			SELECT MIN(CategoryId)
			FROM @l
			);

	WHILE @l1 IS NOT NULL
	BEGIN
		INSERT INTO @lcategory
		SELECT id
		FROM Settings.GetChildCategoryByParent(@l1) AS dt
		INNER JOIN CATALOG.Category ON CategoryId = id
		WHERE dt.id NOT IN (
				SELECT CategoryId
				FROM @lcategory
				)
			AND HirecalEnabled = 1
			AND Enabled = 1

		SET @l1 = (
				SELECT MIN(CategoryId)
				FROM @l
				WHERE CategoryId > @l1
				);
	END;

	IF @onlyCount = 1
	BEGIN
		SELECT COUNT(OfferId)
		FROM (
			(
				SELECT OfferId
				FROM [Catalog].[Product]
				INNER JOIN [Catalog].[Offer] ON [Offer].[ProductID] = [Product].[ProductID]
				INNER JOIN [Catalog].[ProductCategories] ON [ProductCategories].[ProductID] = [Product].[ProductID]
					AND (
						CategoryId IN (
							SELECT CategoryId
							FROM @lcategory
							)
						OR [ProductCategories].[ProductID] IN (
							SELECT productId
							FROM @lproduct
							)
						)
				--LEFT JOIN [Catalog].[Photo] ON [Product].[ProductID] = [Photo].[ObjId] and Type ='Product' AND [Photo].[Main] = 1
				WHERE 
					(
						SELECT TOP (1) [ProductCategories].[CategoryId]
						FROM [Catalog].[ProductCategories]
						INNER JOIN [Catalog].[Category] ON [Category].[CategoryId] = [ProductCategories].[CategoryId]
						WHERE [ProductID] = [Product].[ProductID]
							AND [Enabled] = 1				
							AND [ProductCategories].[CategoryId] in (SELECT CategoryId FROM @lcategory)
						Order By Main DESC, [ProductCategories].[CategoryId]
					) = [ProductCategories].[CategoryId]
					AND
					 (Offer.Price > 0 OR @exportNotAvailable = 1)
					AND (
						Offer.Amount > 0
						OR (Product.AllowPreOrder = 1 and  @allowPreOrder = 1)
						OR @exportNotAvailable = 1
						)
					AND CategoryEnabled = 1
					AND (Product.Enabled = 1 OR @exportNotAvailable = 1)
					AND (@exportAllProducts=1 OR (Select Count(ProductId) From Settings.ExportFeedExcludedProducts Where ExportFeedExcludedProducts.ProductId=Product.ProductId AND ExportFeedExcludedProducts.ExportFeedId=@exportFeedId) = 0)
				)
			) AS dd
	END
	ELSE
	BEGIN
		DECLARE @defaultCurrencyRatio FLOAT;

		SELECT @defaultCurrencyRatio = CurrencyValue
		FROM [Catalog].[Currency]
		WHERE CurrencyIso3 = @selectedCurrency;

		SELECT [Product].[Enabled]
			,[Product].[ProductID]
			,[Product].[Discount]
			,[Product].[DiscountAmount]
			,AllowPreOrder
			,Amount
			,[ProductCategories].[CategoryId] AS [ParentCategory]
			,[Offer].[Price] AS Price
			,ShippingPrice
			,[Product].[Name]
			,[Product].[UrlPath]
			,[Product].[Description]
			,[Product].[BriefDescription]
			,[Product].SalesNote
			,OfferId
			,[Product].ArtNo
			,[Offer].Main
			,[Offer].ColorID
			,ColorName
			,[Offer].SizeID
			,SizeName
			,BrandName
			,country1.CountryName as BrandCountry
			,country2.CountryName as BrandCountryManufacture
			,GoogleProductCategory
			,YandexMarketCategory
			,YandexTypePrefix
			,YandexModel
			,Gtin
			,Adult
			,CurrencyValue
			,[Settings].PhotoToString(Offer.ColorID, Product.ProductId) AS Photos
			,ManufacturerWarranty
			,[Weight]
			,[Product].[Enabled]
			,[Offer].SupplyPrice AS SupplyPrice
			,[Offer].ArtNo AS OfferArtNo
			,[Product].BarCode
			,[Product].Cbid
			,[Product].Fee
			,[Product].YandexSizeUnit
			,[Product].MinAmount
			,[Product].Multiplicity			
			,[Product].Cpa
			,[Product].YandexName		
		FROM [Catalog].[Product]
		INNER JOIN [Catalog].[Offer] ON [Offer].[ProductID] = [Product].[ProductID]
		INNER JOIN [Catalog].[ProductCategories] ON [ProductCategories].[ProductID] = [Product].[ProductID]
			AND (
				CategoryId IN (
					SELECT CategoryId
					FROM @lcategory
					)
				OR [ProductCategories].[ProductID] IN (
					SELECT productId
					FROM @lproduct
					)
				)
		--LEFT JOIN [Catalog].[Photo] ON [Product].[ProductID] = [Photo].[ObjId] and Type ='Product' AND [Photo].[Main] = 1
		LEFT JOIN [Catalog].[Color] ON [Color].ColorID = [Offer].ColorID
		LEFT JOIN [Catalog].[Size] ON [Size].SizeID = [Offer].SizeID
		LEFT JOIN [Catalog].Brand ON Brand.BrandID = [Product].BrandID
		LEFT JOIN [Customers].Country as country1 ON Brand.CountryID = country1.CountryID
		LEFT JOIN [Customers].Country as country2 ON Brand.CountryOfManufactureID = country2.CountryID
		INNER JOIN [Catalog].Currency ON Currency.CurrencyID = [Product].CurrencyID
		WHERE 
			(
				SELECT TOP (1) [ProductCategories].[CategoryId]
				FROM [Catalog].[ProductCategories]
				INNER JOIN [Catalog].[Category] ON [Category].[CategoryId] = [ProductCategories].[CategoryId]
				WHERE [ProductID] = [Product].[ProductID]
					AND [Enabled] = 1				
					AND [ProductCategories].[CategoryId] in (SELECT CategoryId FROM @lcategory)
				Order By Main DESC, [ProductCategories].[CategoryId]
			) = [ProductCategories].[CategoryId]		
			AND 
			(Offer.Price > 0 OR @exportNotAvailable = 1)
			AND (
				Offer.Amount > 0
				OR (Product.AllowPreOrder = 1 and  @allowPreOrder = 1)
				OR @exportNotAvailable = 1
				)
			AND CategoryEnabled = 1
			AND (Product.Enabled = 1 OR @exportNotAvailable = 1)
			AND (@exportAllProducts=1 OR (Select Count(ProductId) From Settings.ExportFeedExcludedProducts Where ExportFeedExcludedProducts.ProductId=Product.ProductId AND ExportFeedExcludedProducts.ExportFeedId=@exportFeedId) = 0)
	END
END

GO--

if not Exists (select * from [Settings].[Localization] where [ResourceKey]='Core.Bonuses.ESmsType.OnBirthdayRule')
begin
	Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (1, 'Core.Bonuses.ESmsType.OnBirthdayRule', 'День рождения');
	Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (2, 'Core.Bonuses.ESmsType.OnBirthdayRule', 'On Birthday');
end

GO--

ALTER PROCEDURE [Settings].[Sp_getcsvproducts] @exportFeedId       INT, 
                                               @onlyCount          BIT, 
                                               @exportNoInCategory BIT, 
                                               @exportAllProducts  BIT, 
                                               @exportNotAvailable BIT 
AS 
  BEGIN 
      DECLARE @res TABLE (productid INT PRIMARY KEY CLUSTERED); 
      DECLARE @lproduct TABLE (productid INT PRIMARY KEY CLUSTERED); 
      DECLARE @lproductNoCat TABLE (productid INT PRIMARY KEY CLUSTERED); 

      INSERT INTO @lproduct 
      SELECT [productid] 
      FROM   [Settings].[exportfeedselectedproducts] 
      WHERE  [exportfeedid] = @exportFeedId; 

      IF ( @exportNoInCategory = 1 ) 
        BEGIN 
            INSERT INTO @lproductNoCat 
				SELECT [productid] 
				FROM   [Catalog].product 
				WHERE  [productid] NOT IN (SELECT [productid] FROM   [Catalog].[productcategories]); 
        END 

      DECLARE @lcategory TABLE (categoryid INT PRIMARY KEY CLUSTERED); 
      DECLARE @l TABLE (categoryid INT PRIMARY KEY CLUSTERED); 

      INSERT INTO @l 
      SELECT t.categoryid 
      FROM   [Settings].[exportfeedselectedcategories] AS t 
             INNER JOIN catalog.category ON t.categoryid = category.categoryid 
      WHERE  [exportfeedid] = @exportFeedId 

      DECLARE @l1 INT 

      SET @l1 = (SELECT Min(categoryid) FROM @l); 

      WHILE @l1 IS NOT NULL 
        BEGIN 
            INSERT INTO @lcategory 
				SELECT id 
				FROM   settings.Getchildcategorybyparent(@l1) AS dt 
				INNER JOIN catalog.category ON categoryid = id 
				WHERE  dt.id NOT IN (SELECT categoryid FROM @lcategory) 

            SET @l1 = (SELECT Min(categoryid) FROM   @l WHERE  categoryid > @l1); 
        END; 

      IF @onlyCount = 1 
        BEGIN 
            SELECT Count(productid) 
            FROM   [Catalog].[product] 
            WHERE  ( EXISTS (SELECT 1 
                             FROM   [Catalog].[productcategories] 
                             WHERE  [productcategories].[productid] = [product].[productid] 
                                    AND ( [productcategories].[productid] IN (SELECT productid FROM @lproduct) OR [productcategories].categoryid IN (SELECT categoryid FROM @lcategory) ) 
							 ) 
                      OR EXISTS (SELECT 1 
                                 FROM   @lproductNoCat AS TEMP 
                                 WHERE  TEMP.productid = [product].[productid]) 
                   ) 
                   AND ( @exportAllProducts = 1 
                          OR (SELECT Count(productid) 
                              FROM  settings.exportfeedexcludedproducts 
                              WHERE exportfeedexcludedproducts.productid = product.productid AND exportfeedexcludedproducts.exportfeedid = @exportFeedId) = 0 )
				   AND (Product.Enabled = 1 OR @exportNotAvailable = 1)
				   AND ( @exportNotAvailable = 1
					      OR EXISTS (SELECT 1 
									 FROM [Catalog].[Offer] o 
									 Where o.[ProductId] = [product].[productid] AND o.Price > 0 and o.Amount > 0))
        END 
      ELSE 
        BEGIN 
            SELECT * 
            FROM   [Catalog].[product] 
                   LEFT JOIN [Catalog].[photo] ON [photo].[objid] = [product].[productid] AND type = 'Product' AND photo.[main] = 1 
            WHERE  ( EXISTS (SELECT 1 
                             FROM   [Catalog].[productcategories] 
                             WHERE  [productcategories].[productid] = [product].[productid] 
                                    AND ( [productcategories].[productid] IN (SELECT productid FROM @lproduct) OR [productcategories].categoryid IN (SELECT categoryid FROM @lcategory) ) 
							) 
                      OR EXISTS (SELECT 1 
                                 FROM   @lproductNoCat AS TEMP 
                                 WHERE  TEMP.productid = [product].[productid]) 
                   ) 
                   AND ( @exportAllProducts = 1 
                          OR (SELECT Count(productid) 
                              FROM   settings.exportfeedexcludedproducts 
                              WHERE exportfeedexcludedproducts.productid = product.productid AND exportfeedexcludedproducts.exportfeedid = @exportFeedId) = 0 ) 
				   AND (Product.Enabled = 1 OR @exportNotAvailable = 1)
				   AND ( @exportNotAvailable = 1
					      OR EXISTS (SELECT 1 
									 FROM [Catalog].[Offer] o 
									 Where o.[ProductId] = [product].[productid] AND o.Price > 0 and o.Amount > 0))
        END 
  END 

GO--

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (1, 'Core.Services.Crm.LeadEventType.Review', 'Отзыв');
Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (2, 'Core.Services.Crm.LeadEventType.Review', 'Review');

GO--

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (1, 'Core.ExportImport.CustomerFields.BirthDay', 'День рождения');
Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (2, 'Core.ExportImport.CustomerFields.BirthDay', 'Birthday');

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (1, 'Core.ExportImport.ImportCsv.MustBeDateTime', 'Поле {0} должно быть датой в строке  {1}');
Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (2, 'Core.ExportImport.ImportCsv.MustBeDateTime', 'Field {0} must be a date at line {1}');

GO--

UPDATE [Settings].[InternalSettings] SET [settingValue] = '6.5.2' WHERE [settingKey] = 'db_version'

