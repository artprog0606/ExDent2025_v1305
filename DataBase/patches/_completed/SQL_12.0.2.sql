EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.Reviews.Rating', 'Рейтинг'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.Reviews.Rating', 'Rating' 

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.AddEditCitys.AdditionalSettigns', 'Доп настройки'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.AddEditCitys.AdditionalSettigns', 'Additional' 

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.AddEditCitiesAdditionalSettings.Header', 'Дополнительные настройки'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.AddEditCitiesAdditionalSettings.Header', 'Additional settings' 

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.AddEditCity.MainSettings', 'Основные настройки'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.AddEditCity.MainSettings', 'Main settings' 

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Orders.Order.ItemsCountAndShipping', 'Количество позиций в заказе с учетом доставки'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Orders.Order.ItemsCountAndShipping', 'The number of items in the order, including delivery' 

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Orders.Order.ItemsCountAndShippingFormatted', 'Количество позиций в заказе с учетом доставки прописью'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Orders.Order.ItemsCountAndShippingFormatted', 'The number of items in the order, including delivery in words' 

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Orders.Order.FinanceItemsCountAndShipping', 'Количество позиций в заказе с учтенными скидками и с учетом доставки'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Orders.Order.FinanceItemsCountAndShipping', 'The number of items in the order with discounts taken into account and taking into account delivery' 

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Orders.Order.FinanceItemsCountAndShippingFormatted', 'Количество позиций в заказе с учтенными скидками и с учетом доставки прописью'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Orders.Order.FinanceItemsCountAndShippingFormatted', 'The number of items in the order with discounts taken into account and taking into account delivery in words' 

GO--

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Settings].[AdditionalOption]') AND type in (N'U'))
	AND NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[Settings].[AdditionalOption]') AND name = N'AdditionalOption_Name')
BEGIN
    IF EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[Settings].[AdditionalOption]') AND name = N'PK_AdditionalOption')
    BEGIN
	    EXEC sp_rename N'Settings.AdditionalOption.PK_AdditionalOption', N'PK_AdditionalOptionTemp', N'INDEX';
    END
	EXEC sp_rename N'Settings.AdditionalOption', N'AdditionalOptionTemp';
END

GO--

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Settings].[AdditionalOption]') AND type in (N'U'))
BEGIN
CREATE TABLE [Settings].[AdditionalOption](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ObjId] [int] NOT NULL,
	[ObjType] [smallint] NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Value] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_AdditionalOption] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END

GO--

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[Settings].[AdditionalOption]') AND name = N'AdditionalOption_Name')
CREATE NONCLUSTERED INDEX [AdditionalOption_Name] ON [Settings].[AdditionalOption]
(
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO--

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[Settings].[AdditionalOption]') AND name = N'AdditionalOption_ObjectId')
CREATE NONCLUSTERED INDEX [AdditionalOption_ObjectId] ON [Settings].[AdditionalOption]
(
	[ObjId] ASC,
	[ObjType] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO--

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Settings].[AdditionalOptionTemp]') AND type in (N'U'))
BEGIN

	INSERT INTO [Settings].[AdditionalOption]
		([ObjId],[ObjType],[Name],[Value])
	SELECT [ObjId], 1, N'AddressPointsIframe', IsNull([Value], '')
	FROM [Settings].[AdditionalOptionTemp]
	WHERE [ObjType] = 0

	INSERT INTO [Settings].[AdditionalOption]
		([ObjId],[ObjType],[Name],[Value])
	SELECT [ObjId], 1, N'ShippingZonesIframe', IsNull([Value], '')
	FROM [Settings].[AdditionalOptionTemp]
	WHERE [ObjType] = 1

	INSERT INTO [Settings].[AdditionalOption]
		([ObjId],[ObjType],[Name],[Value])
	SELECT [ObjId], 1, N'Description', IsNull([Value], '')
	FROM [Settings].[AdditionalOptionTemp]
	WHERE [ObjType] = 3

	INSERT INTO [Settings].[AdditionalOption]
		([ObjId],[ObjType],[Name],[Value])
	SELECT [ObjId], 2, N'EditFieldAddBonusByItem', IsNull([Value], '')
	FROM [Settings].[AdditionalOptionTemp]
	WHERE [ObjType] = 2
END

GO--

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Settings].[AdditionalOptionTemp]') AND type in (N'U'))
DROP TABLE [Settings].[AdditionalOptionTemp]

GO--

    
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Catalog.ShowPropertiesFilterInParentCategories', 'Показывать фильтр по свойствам в промежуточных категориях'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Catalog.ShowPropertiesFilterInParentCategories', 'Show property filter in intermediate categories'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.Builder.ShowPropertiesFilterInParentCategories', 'Показывать фильтр по свойствам в промежуточных категориях'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.Builder.ShowPropertiesFilterInParentCategories', 'Show property filter in intermediate categories'

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Bonuses.YourBonuses', 'У вас'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Bonuses.YourBonuses', 'You have' 

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.Bonuses.ByBonusCard', 'Бонусами по карте'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.Bonuses.ByBonusCard', 'Bonus card' 

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsBonus.Index.AllowSpecifyBonusAmount', 'Давать покупателю возможность выбирать кол-во баллов для списания'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsBonus.Index.AllowSpecifyBonusAmount', 'Give the buyer the opportunity to choose the number of points to be debited' 

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Checkout.Checkout.CannotChargeMoreOrderAmount', 'Нельзя списать больше суммы заказа'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Checkout.Checkout.CannotChargeMoreOrderAmount', 'You cannot charge more than the order amount' 

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.Cart.CouponErrorMsg', 'Скидка по купону применяется только к товарам, скидка которых меньше скидки по купону.<br>Скидка по купону не применяется, если не выполнены условия применения купона (мин.сумма заказа, сроки действия, назначенные категории/товары, проч.)'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.Cart.CouponErrorMsg', 'The coupon discount applies only to products whose discount is less than the coupon discount.<br>The coupon discount is not applied if the conditions for applying the coupon are not met (min. order amount, expiration dates, assigned categories/products, etc.)' 

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Checkout.ShoppingCartMode', 'Режим отображения корзины на странице оформления заказа'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Checkout.ShoppingCartMode', 'Cart display mode on the checkout page' 

GO--
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Catalog.ShowVerificationCheckmarkAtAdminInReviews', 'Показывать галку у администратора'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Catalog.ShowVerificationCheckmarkAtAdminInReviews', 'Show the jackdaw at the administrator' 

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Catalog.ShowVerificationCheckmarkAtAdminInReviewsHint', 'Опция определяет, показывать или нет галку верификации рядом с именем в отзывах от администратора'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Catalog.ShowVerificationCheckmarkAtAdminInReviewsHint', 'The option determines whether or not to show the verification check mark next to the name in the reviews from the administratorr'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.AddEdit.Answer', 'Ответить'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.AddEdit.Answer', 'Answer'

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Catalog.PhotoCategory', 'Категории фотографий'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Catalog.PhotoCategory', 'Photo category'

IF NOT EXISTS(SELECT 1
              FROM sys.columns
              WHERE object_id = OBJECT_ID(N'[Catalog].[PhotoCategory]'))
    BEGIN
        CREATE TABLE [Catalog].[PhotoCategory](
                                                  [Id] [int] IDENTITY(1,1) NOT NULL,
                                                  [Name] [nvarchar](255) NOT NULL,
                                                  [SortOrder] [int] NULL,
                                                  [Enabled] [bit] NOT NULL
                                                      CONSTRAINT [PK_PhotoCategory] PRIMARY KEY CLUSTERED
                                                          (
                                                           [Id] ASC
                                                              )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
        ) ON [PRIMARY]
    END

GO--

IF NOT EXISTS(SELECT 1
              FROM sys.columns
              WHERE (name = N'PhotoCategoryId') AND object_id = OBJECT_ID(N'[Catalog].[Photo]'))
    BEGIN
        ALTER TABLE [Catalog].[Photo] ADD
            [PhotoCategoryId] int NULL

        ALTER TABLE [Catalog].[Photo]  WITH CHECK ADD  CONSTRAINT [FK_Photo_PhotoCategory] FOREIGN KEY([PhotoCategoryId])
            REFERENCES [Catalog].[PhotoCategory] ([Id]) ON DELETE SET NULL
    END

GO--

ALTER PROCEDURE [Catalog].[sp_AddPhoto]
    @ObjId INT, @Description NVARCHAR(255),
    @PhotoName NVARCHAR(255) = 'none',
    @OriginName NVARCHAR(255),
    @Type NVARCHAR(50),
    @Extension NVARCHAR(10),
    @ColorID int,
    @PhotoSortOrder int,
    @PhotoNameSize1 NVARCHAR(255),
    @PhotoNameSize2 NVARCHAR(255),
    @PhotoCategoryId int
AS
BEGIN
    DECLARE @PhotoId int
    DECLARE @ismain bit
    DECLARE @PhotoNameValue NVARCHAR(255)
    SET @PhotoNameValue = 'none'
    SET @ismain = 1

    IF EXISTS(SELECT * FROM [Catalog].[Photo] WHERE ObjId = @ObjId and [Type]=@Type AND main = 1)
        SET @ismain = 0

    IF (@PhotoName IS NOT NULL AND @PhotoName != '')
        SET @PhotoNameValue = @PhotoName

    INSERT INTO [Catalog].[Photo] ([ObjId],[PhotoName],[Description],[ModifiedDate],[PhotoSortOrder],[Main],[OriginName],[Type],[ColorID], PhotoNameSize1, PhotoNameSize2, PhotoCategoryId)
    VALUES (@ObjId,@PhotoNameValue,@Description,Getdate(),@PhotoSortOrder,@ismain,@OriginName,@Type,@ColorID, @PhotoNameSize1, @PhotoNameSize2, @PhotoCategoryId)

    SET @PhotoId = Scope_identity()

    IF (@PhotoNameValue = 'none')
        BEGIN
            DECLARE @newphoto NVARCHAR(255)
            Set @newphoto=Convert(NVARCHAR(255),@PhotoId)+@Extension

            UPDATE [Catalog].[Photo] SET [PhotoName] = @newphoto WHERE [PhotoId] = @PhotoId
        END

    SELECT * FROM [Catalog].[Photo] WHERE [PhotoId] = @PhotoId
    --select @newphoto  
END

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.PhotoCategories.NotSelected', 'Не выбрано'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.PhotoCategories.NotSelected', 'Not selected'

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Api.InHouseMethodName', 'В зале'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Api.InHouseMethodName', 'In room'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Api.InHouseMethodText', 'Выберите стол'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Api.InHouseMethodText', 'Choose a table'

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.MobileVersion.ShowBriefDescriptionNote', 'Настройка позволяет выводить краткое описание товаров в каталоге и на главной при режимах отображения каталога "Список" и "Блоки"'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.MobileVersion.ShowBriefDescriptionNote', 'The setting allows you to display a brief description of products in the catalog and on the main page in the "List" and "Blocks" catalog display modes'

GO--

UPDATE [Settings].[InternalSettings] SET [settingValue] = '12.0.2' WHERE [settingKey] = 'db_version'
