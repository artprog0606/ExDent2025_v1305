ALTER PROCEDURE [Settings].[sp_GetExportFeedProducts] 
     @exportFeedId int
	,@exportNotAvailable bit
	,@selectedCurrency NVARCHAR(10)
	,@allowPreOrder bit = 0
	,@exportAllProducts bit
	,@onlyMainOfferToExport bit
	,@sqlMode NVARCHAR(200) = 'GetProducts'
AS
BEGIN
	
	DECLARE @res TABLE (productId INT PRIMARY KEY CLUSTERED);
	
	DECLARE @lcategory TABLE (CategoryId INT PRIMARY KEY CLUSTERED);
	DECLARE @lcategorytemp TABLE (CategoryId INT);
	DECLARE @l TABLE (CategoryId INT PRIMARY KEY CLUSTERED, Opened bit);

	INSERT INTO @l
	SELECT t.CategoryId, t.Opened
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
		if ((Select Opened from @l where CategoryId=@l1)=1)
		begin
			INSERT INTO @lcategorytemp
			SELECT @l1
		end
		else
		begin
	 		INSERT INTO @lcategorytemp
			SELECT id
			FROM Settings.GetChildCategoryByParent(@l1)
		end

		SET @l1 = (
				SELECT MIN(CategoryId)
				FROM @l
				WHERE CategoryId > @l1
				);
	END;

	INSERT INTO @lcategory
	SELECT Distinct tmp.CategoryId
	FROM @lcategorytemp AS tmp
	INNER JOIN CATALOG.Category ON Category.CategoryId = tmp.CategoryId
	WHERE HirecalEnabled = 1
		AND Enabled = 1;

	IF @sqlMode = 'GetCountOfProducts'
	BEGIN
		SELECT COUNT(Distinct OfferId)
		FROM [Catalog].[Product] p 
		INNER JOIN [Catalog].[Offer] offer ON offer.[ProductID] = p.[ProductID]
		INNER JOIN [Catalog].[ProductCategories] productCategories ON productCategories.[ProductID] = p.[ProductID]
		RIGHT JOIN  @lcategory lc ON lc.CategoryId = productCategories.CategoryID
		LEFT JOIN [Settings].[ExportFeedExcludedProducts]ep ON ep.ProductId = p.ProductId and ep.ExportFeedId=@exportFeedId															
		WHERE 
		(
			ep.ProductID IS NULL 
			OR 
			@exportAllProducts = 1
		)
		AND
			(offer.Price > 0 OR @exportNotAvailable = 1)
		AND (
			offer.Amount > 0
			OR (p.AllowPreOrder = 1 and  @allowPreOrder = 1)
			OR @exportNotAvailable = 1
			)
		AND CategoryEnabled = 1
		AND (p.Enabled = 1 OR @exportNotAvailable = 1)	
		AND (@onlyMainOfferToExport = 0 OR Offer.Main = 1)
	END
	IF @sqlMode = 'GetProducts'
	BEGIN	
		SELECT p.[Enabled]
			,p.[ProductID]
			,p.[Discount]
			,p.[DiscountAmount]
			,AllowPreOrder
			,Amount
			,[ProductCategories].[CategoryId] AS [ParentCategory]
			,[Offer].[Price] AS Price
			,ShippingPrice
			,p.[Name]
			,p.[UrlPath]
			,p.[Description]
			,p.[BriefDescription]
			,p.SalesNote
			,OfferId
			,p.ArtNo
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
			,[Settings].PhotoToString(Offer.ColorID, p.ProductId) AS Photos
			,ManufacturerWarranty
			,[Weight]
			,p.[Enabled]
			,[Offer].SupplyPrice AS SupplyPrice
			,[Offer].ArtNo AS OfferArtNo
			,p.BarCode
			,p.Bid			
			,p.YandexSizeUnit
			,p.MinAmount
			,p.Multiplicity			
			,p.YandexName
			,p.YandexDeliveryDays		
		FROM [Catalog].[Product] p 
		INNER JOIN [Catalog].[Offer] offer ON offer.[ProductID] = p.[ProductID]
		INNER JOIN [Catalog].[ProductCategories] productCategories ON productCategories.[ProductID] = p.[ProductID]
		RIGHT JOIN  @lcategory lc ON lc.CategoryId = productCategories.CategoryID
		LEFT JOIN [Settings].[ExportFeedExcludedProducts]ep ON ep.ProductId = p.ProductId and ep.ExportFeedId=@exportFeedId		
		LEFT JOIN [Catalog].[Color] ON [Color].ColorID = [Offer].ColorID
		LEFT JOIN [Catalog].[Size] ON [Size].SizeID = [Offer].SizeID
		LEFT JOIN [Catalog].Brand ON Brand.BrandID = p.BrandID
		LEFT JOIN [Customers].Country as country1 ON Brand.CountryID = country1.CountryID
		LEFT JOIN [Customers].Country as country2 ON Brand.CountryOfManufactureID = country2.CountryID
		INNER JOIN [Catalog].Currency ON Currency.CurrencyID = p.CurrencyID
		WHERE 
		(
			ep.ProductID IS NULL 
			OR 
			@exportAllProducts = 1
		)
		AND		
		(
			SELECT TOP (1) [ProductCategories].[CategoryId]
			FROM [Catalog].[ProductCategories]
			INNER JOIN @lcategory lc ON lc.[CategoryId] = [ProductCategories].[CategoryId] and [ProductID] = p.[ProductID]
			Order By Main DESC, [ProductCategories].[CategoryId] 
		) = productCategories.[CategoryId]
		AND
			(offer.Price > 0 OR @exportNotAvailable = 1)
		AND (
			offer.Amount > 0
			OR (p.AllowPreOrder = 1 and  @allowPreOrder = 1)
			OR @exportNotAvailable = 1
			)
		AND CategoryEnabled = 1
		AND (p.Enabled = 1 OR @exportNotAvailable = 1)	
		AND (@onlyMainOfferToExport = 0 OR Offer.Main = 1)
	END
	IF @sqlMode = 'GetOfferIds'
	BEGIN
		SELECT Distinct OfferId
		FROM [Catalog].[Product] p 
		INNER JOIN [Catalog].[Offer] offer ON offer.[ProductID] = p.[ProductID]
		INNER JOIN [Catalog].[ProductCategories] productCategories ON productCategories.[ProductID] = p.[ProductID]
		RIGHT JOIN  @lcategory lc ON lc.CategoryId = productCategories.CategoryID
		LEFT JOIN [Settings].[ExportFeedExcludedProducts]ep ON ep.ProductId = p.ProductId and ep.ExportFeedId=@exportFeedId															
		WHERE 
		(
			ep.ProductID IS NULL 
			OR 
			@exportAllProducts = 1
		)
		AND
			(offer.Price > 0 OR @exportNotAvailable = 1)
		AND (
			offer.Amount > 0
			OR (p.AllowPreOrder = 1 and  @allowPreOrder = 1)
			OR @exportNotAvailable = 1
			)
		AND CategoryEnabled = 1
		AND (p.Enabled = 1 OR @exportNotAvailable = 1)	
		AND (@onlyMainOfferToExport = 0 OR Offer.Main = 1)
	END
END

GO--


INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.MainPageProducts.Name', 'Название')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.MainPageProducts.Name', 'Name')
GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.LoadingData', 'Загрузка данных...')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.ChangesSaved', 'Изменения сохранены')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.Deleting', 'Удаление')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.ErrorWhileSaving', 'Ошибка при сохранении')
UPDATE [Settings].[Localization] SET [ResourceValue] = 'Вы уверены, что хотите удалить?' WHERE [LanguageId] = 1 AND [ResourceKey] = 'Admin.Js.AreYouSureDelete'
UPDATE [Settings].[Localization] SET [ResourceValue] = 'Are you sure you want to delete?' WHERE [LanguageId] = 2 AND [ResourceKey] = 'Admin.Js.AreYouSureDelete'

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Partners.EPartnerType.LegalEntity', 'Юридическое лицо')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Partners.EPartnerType.NaturalPerson', 'Физическое лицо')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Partners.EPartnerMessageType.CustomerBinded', 'Привязан покупатель')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Partners.EPartnerMessageType.RewardAdded', 'Начислено вознаграждение')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Partners.EPartnerMessageType.MonthReport', 'Ежемесячный отчет')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.LoadingData', 'Загрузка данных...')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Partners.Index.Title', 'Партнеры')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Partners', 'Партнеры')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Partners.NavMenu.Settings', 'Настройки')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Partners.Index.AddPartner', 'Создать партнера')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Partners.NewPartner', 'Новый партнер')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Partners.Header', 'Партнерская программа')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Partners.DefaultRewardPercent', 'Процент вознаграждения по умолчанию')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Partners.Title', 'Настройки партнерской программы')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Partners.PaymentTypes', 'Способы выплаты')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.NotifyEMails.EmailForPartners', 'E-mail для уведомлений о регистрации партнеров')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Partners.Partner.LastName', 'Фамилия')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Partners.Partner.FirstName', 'Имя')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Partners.Partner.Patronymic', 'Отчество')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Partners.Partner.Email', 'Email')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Partners.Partner.Phone', 'Телефон')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Partners.Partner.Password', 'Пароль')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Partners.Partner.City', 'Город')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Partners.Partner.AdminComment', 'Комментарий администратора')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Partners.Partner.DateCreated', 'Дата регистрации')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Partners.Partner.ChangePassword', 'Изменить пароль')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Partners.Partner.Enabled', 'Активность')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Partners.Partner.SendMessages', 'Отсылать уведомления')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Partners.Partner.PaymentType', 'Способ выплаты')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Partners.Partner.PaymentAccountNumber', '№ счета')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Partners.Partner.NaturalPerson.PassportData', 'Паспортные данные')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Partners.Partner.NaturalPerson.PassportSeria', 'Серия')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Partners.Partner.NaturalPerson.PassportNumber', 'Номер')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Partners.Partner.NaturalPerson.PassportWhoGive', 'Кем выдан')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Partners.Partner.NaturalPerson.PassportWhenGive', 'Когда')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Partners.Partner.NaturalPerson.RegistrationAddress', 'Адрес регистрации')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Partners.Partner.NaturalPerson.Zip', 'Индекс')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Partners.Partner.LegalEntity.CompanyName', 'Наименование')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Partners.Partner.LegalEntity.INN', 'ИНН')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Partners.Partner.LegalEntity.KPP', 'КПП')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Partners.Partner.LegalEntity.LegalAddress', 'Юридический адрес')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Partners.Partner.LegalEntity.ActualAddress', 'Фактический адрес')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Partners.Partner.LegalEntity.SettlementAccount', 'Расчетный счет')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Partners.Partner.LegalEntity.Bank', 'Банк')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Partners.Partner.LegalEntity.CorrespondentAccount', 'Корреспондентский счет')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Partners.Partner.LegalEntity.BIK', 'БИК')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Partners.Partner.LegalEntity.PostAddress', 'Почтовый адрес')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Partners.Partner.LegalEntity.Zip', 'Индекс')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Partners.Partner.LegalEntity.Phone', 'Контактный телефон')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Partners.Partner.LegalEntity.ContactPerson', 'Контактное лицо')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Partners.Partner.LegalEntity.Director', 'Руководитель')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Partners.Partner.LegalEntity.Accountant', 'Бухгалтер')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Partners.View.Title', 'Партнер')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Partners.View.GeneralInformation', 'Общая информация')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Partners.View.TimeFromCreated', 'Ваш партнер:')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Partners.View.Balance', 'Баланс:')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Partners.View.CustomersCount', 'Клиентов:')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Partners.View.PartnerInformation', 'Информация о партнере')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Partners.View.FullName', 'ФИО')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Partners.View.Phone', 'Телефон')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Partners.View.City', 'Город')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Partners.View.Type', 'Тип')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Partners.View.AdminComment', 'Комментарий администратора')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Partners.View.Customers', 'Покупатели')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Partners.View.NaturalPerson.PassportData', 'Паспортные данные')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Partners.View.NaturalPerson.SeriaNumber', 'Серия и номер')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Partners.View.NaturalPerson.PassportWhoGive', 'Выдан')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Partners.View.NaturalPerson.RegistrationAddress', 'Адрес регистрации')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Partners.View.NaturalPerson.Zip', 'Индекс')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Partners.View.LegalEntity.CompanyName', 'Наименование')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Partners.View.LegalEntity.INN', 'ИНН')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Partners.View.LegalEntity.KPP', 'КПП')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Partners.View.LegalEntity.LegalAddress', 'Юридический адрес')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Partners.View.LegalEntity.ActualAddress', 'Фактический адрес')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Partners.View.LegalEntity.SettlementAccount', 'Расчетный счет')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Partners.View.LegalEntity.Bank', 'Банк')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Partners.View.LegalEntity.CorrespondentAccount', 'Корреспондентский счет')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Partners.View.LegalEntity.BIK', 'БИК')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Partners.View.LegalEntity.PostAddress', 'Почтовый адрес')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Partners.View.LegalEntity.Zip', 'Индекс')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Partners.View.LegalEntity.Phone', 'Контактный телефон')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Partners.View.LegalEntity.ContactPerson', 'Контактное лицо')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Partners.View.LegalEntity.Director', 'Руководитель')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Partners.View.LegalEntity.Accountant', 'Бухгалтер')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Partner.Errors.NotFound', 'Партнер не найден')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Partner.Errors.AlreadyHasCoupon', 'К партнеру уже привязан купон')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Partner.Errors.CouponNotFound', 'Купон не найден')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Partner.Errors.CustomerNotFound', 'Покупатель не найден')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Partner.Errors.CustomerAlreadyBinded', 'Покупатель уже привязан к партнеру')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.Partners.FullName', 'Партнер')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.Partners.Phone', 'Телефон')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.Partners.Email', 'Email')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.Partners.DateCreated', 'Дата регистрации')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.Partners.Type', 'Тип')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.Partners.Balance', 'Баланс')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.Partners.Enabled', 'Актив.')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.Partners.Activity', 'Активность')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.Partners.Active', 'Активные')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.Partners.NotActive', 'Неактивные')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.Partner.CantDelete', 'Удаление невозможно')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.Partner.UnbindCoupon', 'Отвязать купон от партнера')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.PartnerCustomers.FullName', 'Покупатель')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.PartnerCustomers.Email', 'Email')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.PartnerCustomers.Phone', 'Телефон')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.PartnerCustomers.Location', 'Город')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.PartnerCustomers.PaidOrdersSum', 'Сумма оплаченных заказов')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.PartnerCustomers.PaidOrdersCount', 'Количество оплаченных заказов')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.PartnerCustomers.DateCreated', 'Дата привязки')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.PartnerCustomers.Customers', 'Привязанные покупатели')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.PartnerCustomers.CreateCustomer', 'Создать покупателя')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.PartnerCustomers.BindCustomer', 'Привязать покупателя')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.PartnerBalance.AddingMoney', 'Пополнение баланса')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.PartnerBalance.SubtractingMoney', 'Списание с баланса')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.PartnerBalance.Amount', 'Сумма')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.PartnerBalance.Basis', 'Основание')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.PartnerBalance.AddBalance', 'Пополнить')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.PartnerBalance.Subtract', 'Списать')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.PartnerBalance.Pay', 'Выплатить')
GO--

CREATE SCHEMA Partners
GO-- 

CREATE TABLE [Partners].[PaymentType](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[SortOrder] [int] NOT NULL,
 CONSTRAINT [PK_Partners_PaymentType] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO--

CREATE TABLE [Partners].[Partner](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Email] [nvarchar](100) NOT NULL,
	[Password] [nvarchar](100) NOT NULL,
	[FirstName] [nvarchar](70) NOT NULL,
	[LastName] [nvarchar](70) NOT NULL,
	[Patronymic] [nvarchar](70) NOT NULL,
	[Phone] [nvarchar](70) NOT NULL,
	[City] [nvarchar](70) NOT NULL,
	[DateCreated] [datetime] NOT NULL,
	[DateUpdated] [datetime] NOT NULL,
	[SendMessages] [int] NOT NULL,
	[AdminComment] [nvarchar](max) NULL,
	[Enabled] [bit] NOT NULL,
	[Balance] [float] NOT NULL,
	[Type] [int] NOT NULL,
	[CouponId] [int] NULL,
	[RewardPercent] [float] NOT NULL,
	[ContractConcluded] [bit] NOT NULL,
	[ContractNumber] [nvarchar](250) NULL,
	[ContractDate] [datetime] NULL,
	[ContractScan] [nvarchar](250) NULL,
	[PaymentTypeId] [int] NULL,
	[PaymentAccountNumber] [nvarchar](250) NULL,
 CONSTRAINT [PK_Partner] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO--

ALTER TABLE [Partners].[Partner]  WITH CHECK ADD  CONSTRAINT [FK_Partner_Coupon] FOREIGN KEY([CouponId])
REFERENCES [Catalog].[Coupon] ([CouponID])
GO--

ALTER TABLE [Partners].[Partner]  WITH CHECK ADD  CONSTRAINT [FK_Partner_PaymentType] FOREIGN KEY([PaymentTypeId])
REFERENCES [Partners].[PaymentType] ([Id])
GO--

CREATE TABLE [Partners].[LegalEntity](
	[PartnerId] [int] NOT NULL,
	[CompanyName] [nvarchar](250) NULL,
	[INN] [nvarchar](50) NULL,
	[KPP] [nvarchar](50) NULL,
	[LegalAddress] [nvarchar](500) NULL,
	[PostAddress] [nvarchar](500) NULL,
	[ActualAddress] [nvarchar](500) NULL,
	[SettlementAccount] [nvarchar](50) NULL,
	[Bank] [nvarchar](250) NULL,
	[CorrespondentAccount] [nvarchar](50) NULL,
	[BIK] [nvarchar](50) NULL,
	[Phone] [nvarchar](70) NULL,
	[ContactPerson] [nvarchar](250) NULL,
	[Director] [nvarchar](250) NULL,
	[Accountant] [nvarchar](250) NULL,
	[DateCreated] [datetime] NOT NULL,
	[DateUpdated] [datetime] NOT NULL,
 CONSTRAINT [PK_Partners_LegalEntity] PRIMARY KEY CLUSTERED 
(
	[PartnerId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO--

ALTER TABLE [Partners].[LegalEntity]  WITH CHECK ADD  CONSTRAINT [FK_LegalEntity_Partner] FOREIGN KEY([PartnerId])
REFERENCES [Partners].[Partner] ([Id])
ON DELETE CASCADE
GO--

CREATE TABLE [Partners].[NaturalPerson](
	[PartnerId] [int] NOT NULL,
	[FirstName] [nvarchar](70) NULL,
	[LastName] [nvarchar](70) NULL,
	[Patronymic] [nvarchar](70) NULL,
	[PassportSeria] [nvarchar](50) NULL,
	[PassportNumber] [nvarchar](50) NULL,
	[PassportWhoGive] [nvarchar](250) NULL,
	[PassportWhenGive] [date] NULL,
	[RegistrationAddress] [nvarchar](500) NULL,
	[Zip] [nvarchar](50) NULL,
	[DateCreated] [datetime] NOT NULL,
	[DateUpdated] [datetime] NOT NULL,
 CONSTRAINT [PK_Partners_NaturalPerson] PRIMARY KEY CLUSTERED 
(
	[PartnerId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO--

ALTER TABLE [Partners].[NaturalPerson]  WITH CHECK ADD  CONSTRAINT [FK_NaturalPerson_Partner] FOREIGN KEY([PartnerId])
REFERENCES [Partners].[Partner] ([Id])
ON DELETE CASCADE
GO--

CREATE TABLE [Partners].[Transaction](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[PartnerId] [int] NOT NULL,
	[Balance] [money] NOT NULL,
	[Amount] [money] NOT NULL,
	[Basis] [nvarchar](500) NULL,
	[CustomerId] [uniqueidentifier] NULL,
	[OrderId] [int] NULL,
	[DateCreated] [datetime] NOT NULL,
	[IsRewardPayout] [bit] NOT NULL,
	[RewardPeriodTo] [date] NULL,
 CONSTRAINT [PK_Partners_Transaction] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO--

ALTER TABLE [Partners].[Transaction]  WITH CHECK ADD CONSTRAINT [FK_Transaction_Partner] FOREIGN KEY([PartnerId])
REFERENCES [Partners].[Partner] ([Id])
GO--

ALTER TABLE [Partners].[Transaction]  WITH CHECK ADD CONSTRAINT [FK_Transaction_Customer] FOREIGN KEY([CustomerId])
REFERENCES [Customers].[Customer] ([CustomerID])
ON DELETE SET NULL
GO--

ALTER TABLE [Partners].[Transaction]  WITH CHECK ADD CONSTRAINT [FK_Transaction_Order] FOREIGN KEY([OrderId])
REFERENCES [Order].[Order] ([OrderID])
ON DELETE SET NULL
GO--

ALTER TABLE [Partners].[Partner] ALTER COLUMN Balance money NOT NULL
GO--

CREATE TABLE [Partners].[BindedCustomer](
	[PartnerId] [int] NOT NULL,
	[CustomerId] [uniqueidentifier] NOT NULL,
	[DateCreated] [datetime] NOT NULL,
	[DateUpdated] [datetime] NOT NULL,
	[UrlReferrer] [nvarchar](500) NULL,
	[UtmSource] [nvarchar](500) NULL,
	[UtmMedium] [nvarchar](500) NULL,
	[UtmCampaign] [nvarchar](500) NULL,
	[UtmTerm] [nvarchar](500) NULL,
	[UtmContent] [nvarchar](500) NULL,
 CONSTRAINT [PK_Partners_BindedCustomer] PRIMARY KEY CLUSTERED 
(
	[CustomerId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO--

ALTER TABLE [Partners].[BindedCustomer]  WITH CHECK ADD  CONSTRAINT [FK_BindedCustomer_Customer] FOREIGN KEY([CustomerId])
REFERENCES [Customers].[Customer] ([CustomerID])
ON DELETE CASCADE
GO--

ALTER TABLE [Partners].[BindedCustomer]  WITH CHECK ADD  CONSTRAINT [FK_BindedCustomer_Partner] FOREIGN KEY([PartnerId])
REFERENCES [Partners].[Partner] ([Id])
ON DELETE CASCADE
GO--

CREATE TABLE Partners.TransactionCurrency (
	[TransactionId] [int] NOT NULL,
	[CurrencyCode] [nchar](3) NOT NULL,
	[CurrencyNumCode] [int] NOT NULL,
	[CurrencyValue] [float] NOT NULL,
	[CurrencySymbol] [nvarchar](7) NOT NULL,
	[IsCodeBefore] [bit] NOT NULL,
	[RoundNumbers] [float] NOT NULL,
	[EnablePriceRounding] [bit] NOT NULL,
 CONSTRAINT PK_Partners_TransactionCurrency PRIMARY KEY CLUSTERED 
(
	TransactionId ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO--

ALTER TABLE Partners.TransactionCurrency  WITH CHECK ADD CONSTRAINT [FK_Partners_TransactionCurrency_Transaction] FOREIGN KEY(TransactionId)
REFERENCES Partners.[Transaction] (Id)
ON DELETE CASCADE
GO--

CREATE TABLE [Partners].[CategoryRewardPercent](
	[CategoryId] [int] NOT NULL,
	[RewardPercent] [float] NOT NULL,
	[DateAdded] [datetime] NOT NULL,
 CONSTRAINT [PK_CategoryRewardPercent] PRIMARY KEY CLUSTERED 
(
	[CategoryId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO--

ALTER TABLE [Partners].[CategoryRewardPercent]  WITH CHECK ADD  CONSTRAINT [FK_CategoryRewardPercent_Category] FOREIGN KEY([CategoryId])
REFERENCES [Catalog].[Category] ([CategoryID])
ON DELETE CASCADE
GO--

ALTER PROCEDURE [Settings].[sp_GetExportFeedCategories] @exportFeedId int
	,@onlyCount BIT
AS
BEGIN
	--template for result array
	DECLARE @result TABLE (CategoryId INT PRIMARY KEY CLUSTERED);
	DECLARE @resultTemp TABLE (CategoryId INT);
	-- templete for array of categories
	DECLARE @lcategory TABLE (CategoryId INT PRIMARY KEY CLUSTERED, Opened bit);

	INSERT INTO @lcategory
	SELECT t.CategoryId, t.Opened
	FROM [Settings].[ExportFeedSelectedCategories] AS t
	INNER JOIN CATALOG.Category ON t.CategoryId = Category.CategoryId
	WHERE HirecalEnabled = 1
		AND Enabled = 1
		AND [ExportFeedId] = @exportFeedId

	DECLARE @l1 INT

	SET @l1 = (
			SELECT MIN(CategoryId)
			FROM @lcategory
			);

	WHILE @l1 IS NOT NULL
	BEGIN
		if ((Select Opened from @lcategory where CategoryId=@l1)=0)
		begin
			--add categories by step thats no in array 
			INSERT INTO @resultTemp
			SELECT id
			FROM Settings.GetChildCategoryByParent(@l1)
		end
		
		insert into @resultTemp
		select id from [Settings].[GetParentsCategoryByChild](@l1)
		
		SET @l1 = (
				SELECT MIN(CategoryId)
				FROM @lcategory
				WHERE CategoryId > @l1
				);
	END;

	INSERT INTO @result
	SELECT Distinct tmp.CategoryId
	FROM @resultTemp AS tmp
	INNER JOIN CATALOG.Category ON Category.CategoryId = tmp.CategoryId
	WHERE HirecalEnabled = 1
		AND Enabled = 1;

	IF @onlyCount = 1
	BEGIN
		SELECT Count([CategoryID])
		FROM [Catalog].[Category]
		WHERE CategoryID <> 0
			AND CategoryId IN (
				SELECT CategoryId
				FROM @result
				)
	END
	ELSE
	BEGIN
		SELECT [CategoryID]
			,[ParentCategory]
			,[Name]
		FROM [Catalog].[Category]
		WHERE CategoryID <> 0
			AND CategoryId IN (
				SELECT CategoryId
				FROM @result
				)
	END
END

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ExportFeed.SettingsYandex.ProductDescriptionHelp','Максимальная длина текста — 3 000 символов (включая знаки препинания).<br/><a href="https://yandex.ru/support/partnermarket/elements/description.html#requirements" target="_blank">Подробнее на Яндексе</a>')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ExportFeed.SettingsYandex.ProductDescriptionHelp','Maximum text length in the description element — 3,000 characters (including spaces).<br/><a href="https://yandex.ru/support/partnermarket/elements/description.html?lang=en#requirements" target="_blank">More info on Yandex</a>')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ExportFeeed.SettingsYandex.ProductDescription','Описание товара')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ExportFeeed.SettingsYandex.ProductDescription','Description')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.PaymentMethods.BillBy.OKPO', 'ОКПО')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.PaymentMethods.BillBy.TIN', 'УНП')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.PaymentMethods.BillBy.OKPO', 'OKPO')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.PaymentMethods.BillBy.TIN', 'UNP')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.PaymentMethods.BillBy.ShowPaymentDetails', 'Запрашивать УНП и название компании у покупателя')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.PaymentMethods.BillBy.ShowPaymentDetails', 'Request details in the client side')

GO--

ALTER TABLE [Order].[PaymentDetails] ADD
	[Contract] [nvarchar](255) NULL

GO--

ALTER PROCEDURE [Order].[sp_AddPaymentDetails]
	@OrderID int,
	@CompanyName nvarchar(255),
	@INN nvarchar(255),
	@Phone nvarchar(20),
	@Contract nvarchar(255)
AS
BEGIN
	INSERT INTO [Order].[PaymentDetails]
           ([OrderID]
		   ,[CompanyName]
		   ,[INN]
		   ,[Phone]
		   ,[Contract])
     VALUES
           (@OrderID
		   ,@CompanyName
		   ,@INN
		   ,@Phone
		   ,@Contract)
	RETURN SCOPE_IDENTITY()
END

GO--

ALTER PROCEDURE [Order].[sp_GetPaymentDetails]
	@OrderID int
AS
BEGIN
	SELECT *
	FROM [Order].[PaymentDetails]						  
	WHERE OrderID = @OrderID
END

GO--

Insert Into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (1, 'Core.Payment.PaymentDetails.Contract', 'Договор')
Insert Into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (2, 'Core.Payment.PaymentDetails.Contract', 'Contract')
Insert Into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (1, 'Admin.Js.OrderItemsSummary.Contract', 'Договор')
Insert Into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (2, 'Admin.Js.OrderItemsSummary.Contract', 'Contract')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.PaymentMethods.BillKz.ShowPaymentDetails', 'Запрашивать БИН/ИИН, название компании и договор у покупателя')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.PaymentMethods.BillKz.ShowPaymentDetails', 'Request details in the client side')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.PaymentMethods.BillKz.IIK', 'ИИК')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.PaymentMethods.BillKz.IIK', 'IIK')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.PaymentMethods.BillKz.PaymentPurposeCode', 'Код назначения платежа')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.PaymentMethods.BillKz.PaymentPurposeCode', 'Payment purpose code')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.PaymentMethods.BillKz.KBE', 'КБЕ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.PaymentMethods.BillKz.KBE', 'KBE')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.PaymentMethods.BillKz.BINIIN', 'БИН/ИИН')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.PaymentMethods.BillKz.BINIIN', 'BIN/IIN')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.PaymentMethods.BillKz.Contractor', 'Исполнитель')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.PaymentMethods.BillKz.Contractor', 'Contractor')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.PaymentMethods.BillKz.ContractorPosition', 'Должность исполнителя')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.PaymentMethods.BillKz.ContractorPosition', 'Contractor position')

GO--
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.ExportImport.ProductFields.PaymentSubjectType', 'Предмет расчета')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.ExportImport.ProductFields.PaymentSubjectType', 'Payment subject type')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.ExportImport.ProductFields.PaymentMethodType', 'Способ расчета')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.ExportImport.ProductFields.PaymentMethodType', 'Payment method type')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.ExportImport.ProductFields.AvitoProductProperties', 'Свойства товара в Авито')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.ExportImport.ProductFields.AvitoProductProperties', 'Propuct properties in Avito')

GO--

ALTER PROCEDURE [Catalog].[sp_IncCountProductInCategory]		
	@CategoryID int,
	@client bit = 1,
	@current bit = 1
AS
BEGIN
	IF (@client = 0)
		IF (@current = 0)
			UPDATE [Catalog].[Category] SET Products_Count = Products_Count + 1, Total_Products_Count = Total_Products_Count + 1 WHERE [CategoryID] = @CategoryID
		ELSE
			UPDATE [Catalog].[Category] SET Products_Count = Products_Count + 1, Total_Products_Count = Total_Products_Count + 1, [Current_Products_Count] = [Current_Products_Count] + 1 WHERE [CategoryID] = @CategoryID
	ELSE
		UPDATE [Catalog].[Category] SET Products_Count = Products_Count + 1 WHERE [CategoryID] = @CategoryID
	
	DECLARE @parentCategoryId int;
	SELECT @parentCategoryId = [ParentCategory] FROM [Catalog].[Category] WHERE [CategoryID] = @CategoryId
	IF(@parentCategoryId <> 0)
		EXEC [Catalog].[sp_IncCountProductInCategory] @parentCategoryId, @client, 0;
END

GO--

ALTER PROCEDURE [Catalog].[sp_DeIncCountProductInCategory]
			@CategoryID int,
			@client bit = 1,		
			@current bit = 1
		
AS
BEGIN
	IF (@client = 0)
		IF (@current = 0)
			UPDATE [Catalog].[Category] SET Products_Count = Products_Count - 1, Total_Products_Count = Total_Products_Count - 1 WHERE CategoryID = @CategoryID
		ELSE
			UPDATE [Catalog].[Category] SET Products_Count = Products_Count - 1, Total_Products_Count = Total_Products_Count - 1, [Current_Products_Count] = [Current_Products_Count] - 1 WHERE CategoryID = @CategoryID
	ELSE
		UPDATE [Catalog].[Category] SET Products_Count = Products_Count - 1 WHERE CategoryID = @CategoryID

		
		DECLARE @parentCategoryId int;
		SELECT @parentCategoryId = [ParentCategory] FROM [Catalog].[Category] WHERE [CategoryID] = @CategoryId
		IF(@parentCategoryId <> 0)
			EXEC [Catalog].[sp_DeIncCountProductInCategory] @parentCategoryId, @client, 0;
END

GO--

DROP TRIGGER [Catalog].[InsertProductInCategory]
GO--
DROP TRIGGER [Catalog].[RemoveProductFromCategory]
GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.EmailingLog.Subscribe', 'Подписать')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.EmailingLog.Subscribe', 'Subscribe')
GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'User.Login.SignInFunnel', 'Войти на страницу')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'User.Login.SignInFunnel', 'Sign in the page')
GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.SendLetterToCustomer.LettersToSubscribers', 'Письма подписчикам')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.SendLetterToCustomer.LettersToSubscribers', 'Letters to subscribers')
GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Index.Feedback', 'Обратная связь')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Index.Feedback', 'Feedback')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Feedback.FeedbackAction', 'Действие с формы')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Feedback.FeedbackAction', 'Action for feedback')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Feedback.FeedbackAction.SendEmail', 'Отправлять письмо')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Feedback.FeedbackAction.SendEmail', 'Send email')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Feedback.FeedbackAction.CreateLead', 'Создавать лид')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Feedback.FeedbackAction.CreateLead', 'Create lead')
GO--

ALTER TABLE CRM.TriggerAction ADD
	RequestHeaderParams nvarchar(MAX) NULL
GO--

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Из общих настроек стоимости доставки и из индивидуальных настроек стоимости доставки товара' WHERE [LanguageId] = 1 AND [ResourceKey] = 'Core.ExportImport.ExportFeedYandexDeliveryCost.LocalDeliveryCost'
UPDATE [Settings].[Localization] SET [ResourceValue] = 'From global delivery settings and product delivery cost' WHERE [LanguageId] = 2 AND [ResourceKey] = 'Core.ExportImport.ExportFeedYandexDeliveryCost.LocalDeliveryCost'
UPDATE [Settings].[Localization] SET [ResourceValue] = 'Время, до которого нужно успеть заказать, чтобы сроки доставки не сдвинулись на один день вперед' WHERE [LanguageId] = 1 AND [ResourceKey] = 'Admin.ExportFeeds.YandexMarket.Time'
UPDATE [Settings].[Localization] SET [ResourceValue] = 'Time to which you need to have time to order, so that the delivery time does not move forward one day' WHERE [LanguageId] = 2 AND [ResourceKey] = 'Admin.ExportFeeds.YandexMarket.Time'

GO--

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Под заказ, если нет в наличии, либо не указана цена' WHERE [LanguageId] = 1 AND [ResourceKey] = 'Admin.Product.Edit.OnRequestOrStock'
UPDATE [Settings].[Localization] SET [ResourceValue] = 'On request if not in stock or no price' WHERE [LanguageId] = 2 AND [ResourceKey] = 'Admin.Product.Edit.OnRequestOrStock'

GO--

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Информация' WHERE [LanguageId] = 1 AND [ResourceKey] = 'Admin.Home.RecommendationsDashboard.Title'
UPDATE [Settings].[Localization] SET [ResourceValue] = 'Information' WHERE [LanguageId] = 2 AND [ResourceKey] = 'Admin.Home.RecommendationsDashboard.Title'

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Leads.CrmNavMenu.SettingsList', 'Все списки')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Leads.CrmNavMenu.SettingsList', 'All lists')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.SalesChannels.SalesChannel.OnlineStore.Description', 'Многофункциональнй Интернет магазин, предназначен для продаж большого кол-ва товаров. Если у Вас более 3х категорий товаров, и более 50-100 товаров, то Вам нужен полноценный Интернет магазин.')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.SalesChannels.SalesChannel.OnlineStore.Description', 'Multifunctional online store, designed to sell a large number of goods. If you have more than 3 categories of products, and more than 50-100 products, then you need a full online store.')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.SalesChannels.SalesChannel.SalesFunnels.Description', 'Воронка продаж это последовательность посадочных страниц, предназначеных для продажи узкого ассортимента товаров или услуг. Всего представлено 12 типов воронок продаж, включая воронки продажи билетов на офлайн мероприятия и вебинары.')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.SalesChannels.SalesChannel.SalesFunnels.Description', 'A sales funnel is a sequence of landing pages designed to sell a narrow range of goods or services. There are 12 types of sales funnels, including offline events and webinars.')


INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.SalesChannels.SalesChannel.VKontakte.Description', 'Выгружайте Ваши товары или предложения услуг во вКонтакте. Обрабатывайте входящих сообщения и комментарии.')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.SalesChannels.SalesChannel.VKontakte.Description', 'Upload your products or service offers in VKontakte. Process incoming messages and comments.')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.SalesChannels.SalesChannel.Instagram.Description', 'Выгружайте Ваши товары в Instagram используя функционал Shopping Tags. Отслеживайте сообщения и комменарии.')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.SalesChannels.SalesChannel.Instagram.Description', 'Upload your products to Instagram using the shopping Tags functionality. Track posts and comments.')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.SalesChannels.SalesChannel.YandexMarket.Description', 'Яндекс.Маркет обладает большой аудиторией покупателей которые смогут найти Ваш товар и купить его перейдя в Ваш интернет магазин. Настройте процесс обмена кателогом товаров с этой товарной площадкой и получайте новых клиентов.')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.SalesChannels.SalesChannel.YandexMarket.Description', 'Yandex.Market has a large audience of buyers who can find Your product and buy it by going to your online store. Set up the process of exchange of goods with the catelog this commodity area and get new customers.')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.SalesChannels.SalesChannel.Avito.Description', 'Развивайте свой бизнес на Авито, расскажите о своих товарах, услугах')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.SalesChannels.SalesChannel.Avito.Description', 'Develop your business on Avito, tell us about your products, services')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.SalesChannels.SalesChannel.GoogleMerchantCenter.Description', 'Хотите предложить покупателям свои товары? Загрузите сведения о товарах в Google Merchant Center, и о них узнают миллионы пользователей со всего мира.')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.SalesChannels.SalesChannel.GoogleMerchantCenter.Description', 'Do you want to offer your products to customers? Upload your product information to Google Merchant Center and millions of users around the world will learn about it.')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.SalesChannels.SalesChannel.Facebook.Description', 'Раздел "Магазин" на Facebook позволяет демонстрировать и продавать продукты прямо на Facebook. Вы узнаете, как добавить магазин того типа, который лучше всего подойдет вашей компании.')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.SalesChannels.SalesChannel.Facebook.Description', 'The Store section on Facebook allows you to showcase and sell products directly on Facebook. You will learn how to add the type of store that best suits your company.')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.SalesChannels.SalesChannel.BonusProgram.Description', 'Создавайте виртуальные бонусные карты, начисляйте бонусы при покупке, удерживайте и возвращайте покупателя.')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.SalesChannels.SalesChannel.BonusProgram.Description', 'Create virtual bonus cards, earn bonuses when buying, hold and return the buyer.')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.SalesChannels.SalesChannel.AffiliateProgram.Description', 'Продавайте через партнеров. Cоздавайте партнерскую программу для увеличения Ваших продаж, запускать различные кампании для Ваших партнеров, а также эффективно отслеживать статистику и комиссионных выплат.')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.SalesChannels.SalesChannel.AffiliateProgram.Description', 'Sell through partners. Create an affiliate program to increase your sales, run various campaigns for your partners, and effectively track statistics and Commission payments.')


INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.SalesChannels.SalesChannel.Triggers','Триггерный маркетинг')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.SalesChannels.SalesChannel.Triggers','Trigger marketing')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.SalesChannels.SalesChannel.Triggers.Description','')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.SalesChannels.SalesChannel.Triggers.Description','')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.SettingsSystem.SystemCommon.AdminAreaColorScheme','Тема в панели администрирования')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.SettingsSystem.SystemCommon.AdminAreaColorScheme','Color theme in admin area')

GO--

INSERT INTO [Settings].[MailFormatType] 
		([TypeName],[SortOrder],[Comment],[MailType])
	VALUES
		('Новый комментарий к брони'
		,300
		,'Уведомление о новом комментарии к брони (#AUTHOR#, #COMMENT#, #BOOKING_ID#, #NAME#, #PHONE#, #DATE#, #RESERVATIONRESOURCE#, #EMAIL#, #ORDERTABLE#, #STORE_NAME#)'
		,'OnBookingCommentAdded')

GO--

INSERT INTO [Settings].[MailFormat]
([FormatName],[FormatText],[SortOrder],[Enable],[AddDate],[ModifyDate],[FormatSubject],[MailFormatTypeId])
VALUES
    ('Новый комментарий к брони'
    ,'<div style="color: #4c4f56; font-family: Arial, Helvetica, sans-serif; font-size: 14px;">
<div class="header" style="border-bottom: 1px solid #ededed; display: table; margin-bottom: 25px; padding-bottom: 25px; width: 100%;">
<div class="logo" style="display: table-cell; text-align: left; vertical-align: middle;">#LOGO#</div>

<div class="phone" style="display: table-cell; text-align: right; vertical-align: middle;">
<div class="tel" style="font-size: 26px; font-weight: bold; line-height: 1; margin-bottom: 5px;">&nbsp;</div>

<div class="inform" style="font-size: 12px;">&nbsp;</div>
</div>
</div>

<div>
<p>#AUTHOR# добавил(-а) комментарий к брони №#BOOKING_ID#.</p>

<div class="o-title vi" style="font-size: 14px; font-weight: bold; margin: 5px 0;"><a href="#BOOKING_URL#">Бронь №#BOOKING_ID#</a></div>

<div class="l-row">
<div class="l-value vi" style="display: inline-block; margin: 5px 0;">#COMMENT#</div>
</div>
</div>
</div>
'
    ,1510
    ,1
    ,GetDate()
    ,GetDate()
    ,'Новый комментарий к брони №#BOOKING_ID#'
    ,(SELECT TOP (1) [MailFormatTypeID] FROM [Settings].[MailFormatType] WHERE [MailType]='OnBookingCommentAdded'))

GO--

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (1, 'Core.Services.CMS.OnBookingCommentNotification.Title', 'Новый комментарий к брони №{0}'); 
Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (2, 'Core.Services.CMS.OnBookingCommentNotification.Title', 'New comment to the booking #{0}'); 
Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (1, 'Core.Services.CMS.OnBookingCommentAnswerNotification.Title', 'Получен ответ на комментарий к брони №{0}'); 
Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (2, 'Core.Services.CMS.OnBookingCommentAnswerNotification.Title', 'You have got answer to comment to the booking #{0}'); 	

GO--

INSERT INTO [Settings].[MailFormatType] 
		([TypeName],[SortOrder],[Comment],[MailType])
	VALUES
		('Новый комментарий к лиду'
		,300
		,'Уведомление о новом комментарии к лиду (#AUTHOR#, #COMMENT#, #LEAD_ID#, #NAME#, #PHONE#, #EMAIL#, #ORGANIZATION#, #ORDERTABLE#, #SHIPPINGMETHOD#, #CITY#, #COMMENTS#, #DESCRIPTION#, #DEAL_STATUS#, #DATE#, #MANAGER_NAME#, #ADDITIONALCUSTOMERFIELDS#, #LEADS_LIST#, #SOURCE#, #LEAD_ATTACHMENTS#)'
		,'OnLeadCommentAdded')

GO--

INSERT INTO [Settings].[MailFormat]
([FormatName],[FormatText],[SortOrder],[Enable],[AddDate],[ModifyDate],[FormatSubject],[MailFormatTypeId])
VALUES
    ('Новый комментарий к лиду'
    ,'<div style="color: #4c4f56; font-family: Arial, Helvetica, sans-serif; font-size: 14px;">
<div class="header" style="border-bottom: 1px solid #ededed; display: table; margin-bottom: 25px; padding-bottom: 25px; width: 100%;">
<div class="logo" style="display: table-cell; text-align: left; vertical-align: middle;">#LOGO#</div>

<div class="phone" style="display: table-cell; text-align: right; vertical-align: middle;">
<div class="tel" style="font-size: 26px; font-weight: bold; line-height: 1; margin-bottom: 5px;">&nbsp;</div>

<div class="inform" style="font-size: 12px;">&nbsp;</div>
</div>
</div>

<div>
<p>#AUTHOR# добавил(-а) комментарий к лиду №#LEAD_ID#.</p>

<div class="o-title vi" style="font-size: 14px; font-weight: bold; margin: 5px 0;"><a href="#LEAD_URL#">Лид №#LEAD_ID#</a></div>

<div class="l-row">
<div class="l-value vi" style="display: inline-block; margin: 5px 0;">#COMMENT#</div>
</div>
</div>
</div>
'
    ,1510
    ,1
    ,GetDate()
    ,GetDate()
    ,'Новый комментарий к лиду №#LEAD_ID#'
    ,(SELECT TOP (1) [MailFormatTypeID] FROM [Settings].[MailFormatType] WHERE [MailType]='OnLeadCommentAdded'))

GO--

Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (1, 'Core.Services.CMS.OnLeadCommentNotification.Title', 'Новый комментарий к лиду №{0}'); 
Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (2, 'Core.Services.CMS.OnLeadCommentNotification.Title', 'New comment to the lead #{0}'); 
Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (1, 'Core.Services.CMS.OnLeadCommentAnswerNotification.Title', 'Получен ответ на комментарий к лиду №{0}'); 
Insert into [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) Values (2, 'Core.Services.CMS.OnLeadCommentAnswerNotification.Title', 'You have got answer to comment to the lead #{0}'); 	
	
GO--

ALTER TABLE [Order].[ShippingMethod] ADD
	ExtrachargeType [int] NULL, 
	Extracharge [float] NULL, 
	ExtrachargeFromOrder [bit] NULL

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ShippingMethods.Common.ExtrachargeType','Тип наценки')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ShippingMethods.Common.ExtrachargeType','Markup type')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ShippingMethods.Common.Extracharge','Наценка')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ShippingMethods.Common.Extracharge','Markup')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ShippingMethods.Common.ExtrachargeFromOrder','От стоимости заказа')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ShippingMethods.Common.ExtrachargeFromOrder','From the cost of the order')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ShippingMethods.Common.ExtrachargeFixedNumber','Фиксированная - это фиксированное число, скажем 100 руб.')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ShippingMethods.Common.ExtrachargeFixedNumber','Fixed - is a fixed number, example 100$.')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ShippingMethods.Common.ExtrachargePercentage','Процентная - это процент от суммы доставки или суммы заказа, скажем 3%.')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ShippingMethods.Common.ExtrachargePercentage','Interest - this is the percentage of the shipping or order amount, example 3%.')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ShippingMethods.Common.ExtrachargeFromOrderHelp','Расчитывать наценку от стоимости заказа.')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ShippingMethods.Common.ExtrachargeFromOrderHelp','Calculate the margin on the cost of the order.')

GO--

ALTER TABLE Catalog.ShoppingCart ADD
	CustomPrice float(53) NULL
GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ShippingMethods.Boxberry.IncreaseDeliveryTime', 'Прибавка к времени доставки')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ShippingMethods.Boxberry.IncreaseDeliveryTime', 'Increase in delivery time')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ShippingMethods.Boxberry.IncreaseDeliveryTimeHelp', 'Указывается в днях')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ShippingMethods.Boxberry.IncreaseDeliveryTimeHelp', 'In days')

GO--

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Отчеты' WHERE [LanguageId] = 1 AND [ResourceKey] = 'Admin.Analytics.Analytics'
UPDATE [Settings].[Localization] SET [ResourceValue] = 'Reports' WHERE [LanguageId] = 2 AND [ResourceKey] = 'Admin.Analytics.Analytics'

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.MobileVersion.MobileTemlateHeader','Тема дизайна')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.MobileVersion.MobileTemlateHeader','Design theme')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.MobileVersion.MobileTemlate','Тема дизайна')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.MobileVersion.MobileTemlate','Design theme')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Mobiles.MobileTemplate.Classic','Классическая')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Mobiles.MobileTemplate.Classic','Classic')

Insert Into [Settings].[TemplateSettings] ([Template], [Name], [Value])
	Select 'mobile', 'Mobile_MainPageProductsCount', [Value] From Settings.Settings Where [Name]='Mobile_MainPageProductsCount' 

Insert Into [Settings].[TemplateSettings] ([Template], [Name], [Value])
	Select 'mobile', 'Mobile_DisplayCity', [Value] From Settings.Settings Where [Name]='Mobile_DisplayCity' 

Insert Into [Settings].[TemplateSettings] ([Template], [Name], [Value])
	Select 'mobile', 'Mobile_DisplaySlider', [Value] From Settings.Settings Where [Name]='Mobile_DisplaySlider' 

Insert Into [Settings].[TemplateSettings] ([Template], [Name], [Value])
	Select 'mobile', 'Mobile_DisplayHeaderTitle', [Value] From Settings.Settings Where [Name]='Mobile_DisplayHeaderTitle' 

Insert Into [Settings].[TemplateSettings] ([Template], [Name], [Value])
	Select 'mobile', 'Mobile_HeaderCustomTitle', [Value] From Settings.Settings Where [Name]='Mobile_HeaderCustomTitle' 


GO--

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Магазин модулей' WHERE [LanguageId] = 1 AND [ResourceKey] = 'Admin.Modules.LeftMenu.ModulesShop'
UPDATE [Settings].[Localization] SET [ResourceValue] = 'Store modules' WHERE [LanguageId] = 2 AND [ResourceKey] = 'Admin.Modules.LeftMenu.ModulesShop'

GO--
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.CompleteTask.OrderCreated','Создан заказ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.CompleteTask.OrderCreated','Order created')

GO--


GO--
ALTER PROCEDURE [Settings].[sp_GetExportFeedProducts] 
     @exportFeedId int
	,@exportNotAvailable bit
	,@selectedCurrency NVARCHAR(10)
	,@allowPreOrder bit = 0
	,@exportAllProducts bit
	,@onlyMainOfferToExport bit
	,@sqlMode NVARCHAR(200) = 'GetProducts'
AS
BEGIN
	
	DECLARE @res TABLE (productId INT PRIMARY KEY CLUSTERED);
	
	DECLARE @lcategory TABLE (CategoryId INT PRIMARY KEY CLUSTERED);
	DECLARE @lcategorytemp TABLE (CategoryId INT);
	DECLARE @l TABLE (CategoryId INT PRIMARY KEY CLUSTERED, Opened bit);

	INSERT INTO @l
	SELECT t.CategoryId, t.Opened
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
		if ((Select Opened from @l where CategoryId=@l1)=1)
		begin
			INSERT INTO @lcategorytemp
			SELECT @l1
		end
		else
		begin
	 		INSERT INTO @lcategorytemp
			SELECT id
			FROM Settings.GetChildCategoryByParent(@l1)
		end

		SET @l1 = (
				SELECT MIN(CategoryId)
				FROM @l
				WHERE CategoryId > @l1
				);
	END;

	INSERT INTO @lcategory
	SELECT Distinct tmp.CategoryId
	FROM @lcategorytemp AS tmp
	INNER JOIN CATALOG.Category ON Category.CategoryId = tmp.CategoryId
	WHERE HirecalEnabled = 1
		AND Enabled = 1;

	IF @sqlMode = 'GetCountOfProducts'
	BEGIN
		SELECT COUNT(Distinct OfferId)
		FROM [Catalog].[Product] p 
		INNER JOIN [Catalog].[Offer] offer ON offer.[ProductID] = p.[ProductID]
		INNER JOIN [Catalog].[ProductCategories] productCategories ON productCategories.[ProductID] = p.[ProductID]
		INNER JOIN  @lcategory lc ON lc.CategoryId = productCategories.CategoryID
		LEFT JOIN [Settings].[ExportFeedExcludedProducts]ep ON ep.ProductId = p.ProductId and ep.ExportFeedId=@exportFeedId															
		WHERE 
		(
			ep.ProductID IS NULL 
			OR 
			@exportAllProducts = 1
		)
		AND
			(offer.Price > 0 OR @exportNotAvailable = 1)
		AND (
			offer.Amount > 0
			OR (p.AllowPreOrder = 1 and  @allowPreOrder = 1)
			OR @exportNotAvailable = 1
			)
		AND CategoryEnabled = 1
		AND (p.Enabled = 1 OR @exportNotAvailable = 1)	
		AND (@onlyMainOfferToExport = 0 OR Offer.Main = 1)
	END
	IF @sqlMode = 'GetProducts'
	BEGIN
	with cte as (
	SELECT Distinct tmp.CategoryId
	FROM @lcategorytemp AS tmp
	INNER JOIN CATALOG.Category ON Category.CategoryId = tmp.CategoryId

	WHERE HirecalEnabled = 1 AND Enabled = 1)	
		SELECT p.[Enabled]
			,p.[ProductID]
			,p.[Discount]
			,p.[DiscountAmount]
			,AllowPreOrder
			,Amount
			,crossCategory.[CategoryId] AS [ParentCategory]
			,[Offer].[Price] AS Price
			,ShippingPrice
			,p.[Name]
			,p.[UrlPath]
			,p.[Description]
			,p.[BriefDescription]
			,p.SalesNote
			,OfferId
			,p.ArtNo
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
			,[Settings].PhotoToString(Offer.ColorID, p.ProductId) AS Photos
			,ManufacturerWarranty
			,[Weight]
			,p.[Enabled]
			,[Offer].SupplyPrice AS SupplyPrice
			,[Offer].ArtNo AS OfferArtNo
			,p.BarCode
			,p.Bid			
			,p.YandexSizeUnit
			,p.MinAmount
			,p.Multiplicity			
			,p.YandexName
			,p.YandexDeliveryDays		
		FROM [Catalog].[Product] p 
		INNER JOIN [Catalog].[Offer] offer ON offer.[ProductID] = p.[ProductID]
		--INNER JOIN [Catalog].[ProductCategories] productCategories ON productCategories.[ProductID] = p.[ProductID]
		--RIGHT JOIN  @lcategory lc ON lc.CategoryId = productCategories.CategoryID
		LEFT JOIN [Settings].[ExportFeedExcludedProducts]ep ON ep.ProductId = p.ProductId and ep.ExportFeedId=@exportFeedId		
		LEFT JOIN [Catalog].[Color] ON [Color].ColorID = [Offer].ColorID
		LEFT JOIN [Catalog].[Size] ON [Size].SizeID = [Offer].SizeID
		LEFT JOIN [Catalog].Brand ON Brand.BrandID = p.BrandID
		LEFT JOIN [Customers].Country as country1 ON Brand.CountryID = country1.CountryID
		LEFT JOIN [Customers].Country as country2 ON Brand.CountryOfManufactureID = country2.CountryID
		INNER JOIN [Catalog].Currency ON Currency.CurrencyID = p.CurrencyID
		cross apply(SELECT TOP (1) [ProductCategories].[CategoryId] from [Catalog].[ProductCategories]
					INNER JOIN  cte lc ON lc.CategoryId = productCategories.CategoryID
					where  [ProductCategories].[ProductID] = p.[ProductID]
					Order By [ProductCategories].Main DESC, [ProductCategories].[CategoryId] ) crossCategory	
		WHERE 
		(
			ep.ProductID IS NULL 
			OR 
			@exportAllProducts = 1
		)		
		AND
			(offer.Price > 0 OR @exportNotAvailable = 1)
		AND (
			offer.Amount > 0
			OR (p.AllowPreOrder = 1 and  @allowPreOrder = 1)
			OR @exportNotAvailable = 1
			)
		AND CategoryEnabled = 1
		AND (p.Enabled = 1 OR @exportNotAvailable = 1)	
		AND (@onlyMainOfferToExport = 0 OR Offer.Main = 1)
	END
	IF @sqlMode = 'GetOfferIds'
	BEGIN
		SELECT Distinct OfferId
		FROM [Catalog].[Product] p 
		INNER JOIN [Catalog].[Offer] offer ON offer.[ProductID] = p.[ProductID]
		INNER JOIN [Catalog].[ProductCategories] productCategories ON productCategories.[ProductID] = p.[ProductID]
		INNER JOIN  @lcategory lc ON lc.CategoryId = productCategories.CategoryID
		LEFT JOIN [Settings].[ExportFeedExcludedProducts]ep ON ep.ProductId = p.ProductId and ep.ExportFeedId=@exportFeedId															
		WHERE 
		(
			ep.ProductID IS NULL 
			OR 
			@exportAllProducts = 1
		)
		AND
			(offer.Price > 0 OR @exportNotAvailable = 1)
		AND (
			offer.Amount > 0
			OR (p.AllowPreOrder = 1 and  @allowPreOrder = 1)
			OR @exportNotAvailable = 1
			)
		AND CategoryEnabled = 1
		AND (p.Enabled = 1 OR @exportNotAvailable = 1)	
		AND (@onlyMainOfferToExport = 0 OR Offer.Main = 1)
	END
END
GO--

UPDATE [Settings].[InternalSettings] SET [settingValue] = '7.0.3' WHERE [settingKey] = 'db_version'