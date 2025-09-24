GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'PreOrder.Index.MobileTitle', 'Под заказ')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'PreOrder.Index.MobileTitle', 'Preorder')

GO--

ALTER TABLE [Customers].[OkUser] 
ADD Photo nvarchar(MAX) NULL

GO--

Update [CMS].[LandingBlock] 
Set Settings = Replace(Settings, 'templates/article/pictures/', 'templates/article/images/')
Where Settings like '%templates/article/pictures/%'

Update [CMS].[LandingBlock] 
Set Settings = Replace(Settings, 'templates/CollectContacts/pictures/', 'templates/CollectContacts/images/')
Where Settings like '%templates/CollectContacts/pictures/%'

Update [CMS].[LandingBlock] 
Set Settings = Replace(Settings, 'templates/CollectContactsForAccess/pictures/', 'templates/CollectContactsForAccess/images/')
Where Settings like '%templates/CollectContactsForAccess/pictures/%'

Update [CMS].[LandingBlock] 
Set Settings = Replace(Settings, 'templates/ConferenceOnlineOrder/pictures/', 'templates/ConferenceOnlineOrder/images/')
Where Settings like '%templates/ConferenceOnlineOrder/pictures/%'

Update [CMS].[LandingBlock] 
Set Settings = Replace(Settings, 'templates/Consulting/pictures/', 'templates/Consulting/images/')
Where Settings like '%templates/Consulting/pictures/%'

Update [CMS].[LandingBlock] 
Set Settings = Replace(Settings, 'templates/Couponator/pictures/', 'templates/Couponator/images/')
Where Settings like '%templates/Couponator/pictures/%'

Update [CMS].[LandingBlock] 
Set Settings = Replace(Settings, 'templates/CouponatorOrder/pictures/', 'templates/CouponatorOrder/images/')
Where Settings like '%templates/CouponatorOrder/pictures/%'

Update [CMS].[LandingBlock] 
Set Settings = Replace(Settings, 'templates/CourseOnlineOrder/pictures/', 'templates/CourseOnlineOrder/images/')
Where Settings like '%templates/CourseOnlineOrder/pictures/%'

Update [CMS].[LandingBlock] 
Set Settings = Replace(Settings, 'templates/EventAction/pictures/', 'templates/EventAction/images/')
Where Settings like '%templates/EventAction/pictures/%'

Update [CMS].[LandingBlock] 
Set Settings = Replace(Settings, 'templates/InstagramFunnel/pictures/', 'templates/InstagramFunnel/images/')
Where Settings like '%templates/InstagramFunnel/pictures/%'

Update [CMS].[LandingBlock] 
Set Settings = Replace(Settings, 'templates/LandingFunnel/pictures/', 'templates/LandingFunnel/images/')
Where Settings like '%templates/LandingFunnel/pictures/%'

Update [CMS].[LandingBlock] 
Set Settings = Replace(Settings, 'templates/LandingFunnelOrder/pictures/', 'templates/LandingFunnelOrder/images/')
Where Settings like '%templates/LandingFunnelOrder/pictures/%'

Update [CMS].[LandingBlock] 
Set Settings = Replace(Settings, 'templates/LandingFunnelOrderSet/pictures/', 'templates/LandingFunnelOrderSet/images/')
Where Settings like '%templates/LandingFunnelOrderSet/pictures/%'

Update [CMS].[LandingBlock] 
Set Settings = Replace(Settings, 'templates/LandingFunnelOrderSetWithForm/pictures/', 'templates/LandingFunnelOrderSetWithForm/images/')
Where Settings like '%templates/LandingFunnelOrderSetWithForm/pictures/%'

Update [CMS].[LandingBlock] 
Set Settings = Replace(Settings, 'templates/LandingFunnelOrderWithForm/pictures/', 'templates/LandingFunnelOrderWithForm/images/')
Where Settings like '%templates/LandingFunnelOrderWithForm/pictures/%'

Update [CMS].[LandingBlock] 
Set Settings = Replace(Settings, 'templates/LeadMagnet/pictures/', 'templates/LeadMagnet/images/')
Where Settings like '%templates/LeadMagnet/pictures/%'

Update [CMS].[LandingBlock] 
Set Settings = Replace(Settings, 'templates/MultyProducts/pictures/', 'templates/MultyProducts/images/')
Where Settings like '%templates/MultyProducts/pictures/%'

Update [CMS].[LandingBlock] 
Set Settings = Replace(Settings, 'templates/MultyProductsKnowPrice/pictures/', 'templates/MultyProductsKnowPrice/images/')
Where Settings like '%templates/MultyProductsKnowPrice/pictures/%'

Update [CMS].[LandingBlock] 
Set Settings = Replace(Settings, 'templates/MultyProductsSale/pictures/', 'templates/MultyProductsSale/images/')
Where Settings like '%templates/MultyProductsSale/pictures/%'

Update [CMS].[LandingBlock] 
Set Settings = Replace(Settings, 'templates/OneProductUpSellDownSell/pictures/', 'templates/OneProductUpSellDownSell/images/')
Where Settings like '%templates/OneProductUpSellDownSell/pictures/%'

Update [CMS].[LandingBlock] 
Set Settings = Replace(Settings, 'templates/ProductCrossSellDownSell/pictures/', 'templates/ProductCrossSellDownSell/images/')
Where Settings like '%templates/ProductCrossSellDownSell/pictures/%'

Update [CMS].[LandingBlock] 
Set Settings = Replace(Settings, 'templates/QuizFunnel/pictures/', 'templates/QuizFunnel/images/')
Where Settings like '%templates/QuizFunnel/pictures/%'

Update [CMS].[LandingBlock] 
Set Settings = Replace(Settings, 'templates/QuizFunnelCollectContacts/pictures/', 'templates/QuizFunnelCollectContacts/images/')
Where Settings like '%templates/QuizFunnelCollectContacts/pictures/%'

Update [CMS].[LandingBlock] 
Set Settings = Replace(Settings, 'templates/ServicesOnline/pictures/', 'templates/ServicesOnline/images/')
Where Settings like '%templates/ServicesOnline/pictures/%'

Update [CMS].[LandingBlock] 
Set Settings = Replace(Settings, 'templates/VideoLandingFunnel/pictures/', 'templates/VideoLandingFunnel/images/')
Where Settings like '%templates/VideoLandingFunnel/pictures/%'

Update [CMS].[LandingBlock] 
Set Settings = Replace(Settings, 'templates/VideoLandingFunnelOrder/pictures/', 'templates/VideoLandingFunnelOrder/images/')
Where Settings like '%templates/VideoLandingFunnelOrder/pictures/%'

Update [CMS].[LandingBlock] 
Set Settings = Replace(Settings, 'templates/VideoLeadMagnet/pictures/', 'templates/VideoLeadMagnet/images/')
Where Settings like '%templates/VideoLeadMagnet/pictures/%'

Update [CMS].[LandingBlock] 
Set Settings = Replace(Settings, 'templates/VideoLeadMagnet2/pictures/', 'templates/VideoLeadMagnet2/images/')
Where Settings like '%templates/VideoLeadMagnet2/pictures/%'

GO--

Update [CMS].[LandingSubBlock] 
Set Settings = Replace(Settings, 'templates/article/pictures/', 'templates/article/images/')
Where Settings like '%templates/article/pictures/%'

Update [CMS].[LandingSubBlock] 
Set Settings = Replace(Settings, 'templates/CollectContacts/pictures/', 'templates/CollectContacts/images/')
Where Settings like '%templates/CollectContacts/pictures/%'

Update [CMS].[LandingSubBlock] 
Set Settings = Replace(Settings, 'templates/CollectContactsForAccess/pictures/', 'templates/CollectContactsForAccess/images/')
Where Settings like '%templates/CollectContactsForAccess/pictures/%'

Update [CMS].[LandingSubBlock] 
Set Settings = Replace(Settings, 'templates/ConferenceOnlineOrder/pictures/', 'templates/ConferenceOnlineOrder/images/')
Where Settings like '%templates/ConferenceOnlineOrder/pictures/%'

Update [CMS].[LandingSubBlock] 
Set Settings = Replace(Settings, 'templates/Consulting/pictures/', 'templates/Consulting/images/')
Where Settings like '%templates/Consulting/pictures/%'

Update [CMS].[LandingSubBlock] 
Set Settings = Replace(Settings, 'templates/Couponator/pictures/', 'templates/Couponator/images/')
Where Settings like '%templates/Couponator/pictures/%'

Update [CMS].[LandingSubBlock] 
Set Settings = Replace(Settings, 'templates/CouponatorOrder/pictures/', 'templates/CouponatorOrder/images/')
Where Settings like '%templates/CouponatorOrder/pictures/%'

Update [CMS].[LandingSubBlock] 
Set Settings = Replace(Settings, 'templates/CourseOnlineOrder/pictures/', 'templates/CourseOnlineOrder/images/')
Where Settings like '%templates/CourseOnlineOrder/pictures/%'

Update [CMS].[LandingSubBlock] 
Set Settings = Replace(Settings, 'templates/EventAction/pictures/', 'templates/EventAction/images/')
Where Settings like '%templates/EventAction/pictures/%'

Update [CMS].[LandingSubBlock] 
Set Settings = Replace(Settings, 'templates/InstagramFunnel/pictures/', 'templates/InstagramFunnel/images/')
Where Settings like '%templates/InstagramFunnel/pictures/%'

Update [CMS].[LandingSubBlock] 
Set Settings = Replace(Settings, 'templates/LandingFunnel/pictures/', 'templates/LandingFunnel/images/')
Where Settings like '%templates/LandingFunnel/pictures/%'

Update [CMS].[LandingSubBlock] 
Set Settings = Replace(Settings, 'templates/LandingFunnelOrder/pictures/', 'templates/LandingFunnelOrder/images/')
Where Settings like '%templates/LandingFunnelOrder/pictures/%'

Update [CMS].[LandingSubBlock] 
Set Settings = Replace(Settings, 'templates/LandingFunnelOrderSet/pictures/', 'templates/LandingFunnelOrderSet/images/')
Where Settings like '%templates/LandingFunnelOrderSet/pictures/%'

Update [CMS].[LandingSubBlock] 
Set Settings = Replace(Settings, 'templates/LandingFunnelOrderSetWithForm/pictures/', 'templates/LandingFunnelOrderSetWithForm/images/')
Where Settings like '%templates/LandingFunnelOrderSetWithForm/pictures/%'

Update [CMS].[LandingSubBlock] 
Set Settings = Replace(Settings, 'templates/LandingFunnelOrderWithForm/pictures/', 'templates/LandingFunnelOrderWithForm/images/')
Where Settings like '%templates/LandingFunnelOrderWithForm/pictures/%'

Update [CMS].[LandingSubBlock] 
Set Settings = Replace(Settings, 'templates/LeadMagnet/pictures/', 'templates/LeadMagnet/images/')
Where Settings like '%templates/LeadMagnet/pictures/%'

Update [CMS].[LandingSubBlock] 
Set Settings = Replace(Settings, 'templates/MultyProducts/pictures/', 'templates/MultyProducts/images/')
Where Settings like '%templates/MultyProducts/pictures/%'

Update [CMS].[LandingSubBlock] 
Set Settings = Replace(Settings, 'templates/MultyProductsKnowPrice/pictures/', 'templates/MultyProductsKnowPrice/images/')
Where Settings like '%templates/MultyProductsKnowPrice/pictures/%'

Update [CMS].[LandingSubBlock] 
Set Settings = Replace(Settings, 'templates/MultyProductsSale/pictures/', 'templates/MultyProductsSale/images/')
Where Settings like '%templates/MultyProductsSale/pictures/%'

Update [CMS].[LandingSubBlock] 
Set Settings = Replace(Settings, 'templates/OneProductUpSellDownSell/pictures/', 'templates/OneProductUpSellDownSell/images/')
Where Settings like '%templates/OneProductUpSellDownSell/pictures/%'

Update [CMS].[LandingSubBlock] 
Set Settings = Replace(Settings, 'templates/ProductCrossSellDownSell/pictures/', 'templates/ProductCrossSellDownSell/images/')
Where Settings like '%templates/ProductCrossSellDownSell/pictures/%'

Update [CMS].[LandingSubBlock] 
Set Settings = Replace(Settings, 'templates/QuizFunnel/pictures/', 'templates/QuizFunnel/images/')
Where Settings like '%templates/QuizFunnel/pictures/%'

Update [CMS].[LandingSubBlock] 
Set Settings = Replace(Settings, 'templates/QuizFunnelCollectContacts/pictures/', 'templates/QuizFunnelCollectContacts/images/')
Where Settings like '%templates/QuizFunnelCollectContacts/pictures/%'

Update [CMS].[LandingSubBlock] 
Set Settings = Replace(Settings, 'templates/ServicesOnline/pictures/', 'templates/ServicesOnline/images/')
Where Settings like '%templates/ServicesOnline/pictures/%'

Update [CMS].[LandingSubBlock] 
Set Settings = Replace(Settings, 'templates/VideoLandingFunnel/pictures/', 'templates/VideoLandingFunnel/images/')
Where Settings like '%templates/VideoLandingFunnel/pictures/%'

Update [CMS].[LandingSubBlock] 
Set Settings = Replace(Settings, 'templates/VideoLandingFunnelOrder/pictures/', 'templates/VideoLandingFunnelOrder/images/')
Where Settings like '%templates/VideoLandingFunnelOrder/pictures/%'

Update [CMS].[LandingSubBlock] 
Set Settings = Replace(Settings, 'templates/VideoLeadMagnet/pictures/', 'templates/VideoLeadMagnet/images/')
Where Settings like '%templates/VideoLeadMagnet/pictures/%'

Update [CMS].[LandingSubBlock] 
Set Settings = Replace(Settings, 'templates/VideoLeadMagnet2/pictures/', 'templates/VideoLeadMagnet2/images/')
Where Settings like '%templates/VideoLeadMagnet2/pictures/%'

GO--

Update [CMS].[LandingBlock] 
Set Settings = Replace(Settings, 'areas/landing/images/pictures/imageBlock.jpg', 'areas/landing/images/image/imageBlock.jpg') 
Where Settings like '%areas/landing/images/pictures/imageBlock.jpg%'

Update [CMS].[LandingBlock] 
Set Settings = Replace(Settings, 'areas/landing/images/pictures/picture-1.jpg', 'areas/landing/images/gallery/picture-1.jpg') 
Where Settings like '%areas/landing/images/pictures/picture-1.jpg%'

Update [CMS].[LandingBlock] 
Set Settings = Replace(Settings, 'areas/landing/images/pictures/picture-2.jpg', 'areas/landing/images/gallery/picture-2.jpg') 
Where Settings like '%areas/landing/images/pictures/picture-2.jpg%'

Update [CMS].[LandingBlock] 
Set Settings = Replace(Settings, 'areas/landing/images/pictures/picture-3.jpg', 'areas/landing/images/gallery/picture-3.jpg') 
Where Settings like '%areas/landing/images/pictures/picture-3.jpg%'

Update [CMS].[LandingBlock] 
Set Settings = Replace(Settings, 'areas/landing/images/pictures/preview_image_background.jpg', 'areas/landing/images/imageBackground/preview_image_background.jpg') 
Where Settings like '%areas/landing/images/pictures/preview_image_background.jpg%'

Update [CMS].[LandingBlock] 
Set Settings = Replace(Settings, 'areas/landing/images/pictures/preview_image_carousel.jpg', 'areas/landing/images/gallery/preview_image_carousel.jpg') 
Where Settings like '%areas/landing/images/pictures/preview_image_carousel.jpg%'

Update [CMS].[LandingBlock] 
Set Settings = Replace(Settings, 'areas/landing/images/pictures/preview_image_carousel_alt.jpg', 'areas/landing/images/gallery/preview_image_carousel_alt.jpg') 
Where Settings like '%areas/landing/images/pictures/preview_image_carousel_alt.jpg%'

GO--

Update [CMS].[LandingSubBlock] 
Set Settings = Replace(Settings, 'areas/landing/images/pictures/imageBlock.jpg', 'areas/landing/images/image/imageBlock.jpg') 
Where Settings like '%areas/landing/images/pictures/imageBlock.jpg%'

Update [CMS].[LandingSubBlock] 
Set Settings = Replace(Settings, 'areas/landing/images/pictures/picture-1.jpg', 'areas/landing/images/gallery/picture-1.jpg') 
Where Settings like '%areas/landing/images/pictures/picture-1.jpg%'

Update [CMS].[LandingSubBlock] 
Set Settings = Replace(Settings, 'areas/landing/images/pictures/picture-2.jpg', 'areas/landing/images/gallery/picture-2.jpg') 
Where Settings like '%areas/landing/images/pictures/picture-2.jpg%'

Update [CMS].[LandingSubBlock] 
Set Settings = Replace(Settings, 'areas/landing/images/pictures/picture-3.jpg', 'areas/landing/images/gallery/picture-3.jpg') 
Where Settings like '%areas/landing/images/pictures/picture-3.jpg%'

Update [CMS].[LandingSubBlock] 
Set Settings = Replace(Settings, 'areas/landing/images/pictures/preview_image_background.jpg', 'areas/landing/images/imageBackground/preview_image_background.jpg') 
Where Settings like '%areas/landing/images/pictures/preview_image_background.jpg%'

Update [CMS].[LandingSubBlock] 
Set Settings = Replace(Settings, 'areas/landing/images/pictures/preview_image_carousel.jpg', 'areas/landing/images/gallery/preview_image_carousel.jpg') 
Where Settings like '%areas/landing/images/pictures/preview_image_carousel.jpg%'

Update [CMS].[LandingSubBlock] 
Set Settings = Replace(Settings, 'areas/landing/images/pictures/preview_image_carousel_alt.jpg', 'areas/landing/images/gallery/preview_image_carousel_alt.jpg') 
Where Settings like '%areas/landing/images/pictures/preview_image_carousel_alt.jpg%'

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Import.ImportProducts.OnlyUpdateProducts', 'Не добавлять новые товары')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Import.ImportProducts.OnlyUpdateProducts', 'Don''t add new products')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Product.CustomOptions.RequiredField', 'Поле обязательно для заполнения')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Product.CustomOptions.RequiredField', 'Field is required')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Product.CustomOptions.EnterValidNumber', 'Введите корректное число')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Product.CustomOptions.EnterValidNumber', 'Enter valid number')



INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Statistics.PageViews', 'Cтраниц просмотрено')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Statistics.PageViews', 'Page views')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Statistics.Visits', 'Посещения')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Statistics.Visits', 'Visits')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Statistics.Visitors', 'Посетители')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Statistics.Visitors', 'Visitors')


INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Statistics.Attendance', 'Посещаемость')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Statistics.Attendance', 'Attendance')


INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'AdminMobile.Orders', 'Заказы')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'AdminMobile.Orders', 'Orders')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'AdminMobile.Order', 'Заказ №')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'AdminMobile.Order', 'Order #')


INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'AdminMobile.Graphics', 'График по заказам')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'AdminMobile.Graphics', 'Orders chart')


INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Modules.NoSettings', 'У данного модуля нет дополнительных настроек')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Modules.NoSettings', 'This module has no any settings')


INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.UseGlobalVariables', 'Доступные переменные:')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.UseGlobalVariables', 'Available variables:')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.BrandName', 'Наименование производителя')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.BrandName', 'Brand name')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.CategoryName', 'Название категории')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.CategoryName', 'Category name')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.NewsTitle', 'Заголовок новости')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.NewsTitle', 'News title')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ProductName', 'Название товара')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ProductName', 'Product name')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.TagName', 'Название тега')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.TagName', 'Tag name')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.StaticPageName', 'Название страницы')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.StaticPageName', 'Pega name')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ProductListsName', 'Название списка')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ProductListsName', 'List name')


INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'AdminMobile.Leads', 'Лиды')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'AdminMobile.Leads', 'Leads')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'AdminMobile.Tasks', 'Задачи')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'AdminMobile.Tasks', 'Tasks')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Catalog.NotSelected', 'Не выбрана')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Catalog.NotSelected', 'Not selected')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Catalog.MainCategory', 'Основная')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Catalog.MainCategory', 'Main')

Update [Settings].[Localization]
set [ResourceValue] = 'У менеджера {0} есть назначенные {1}'
where [ResourceKey] = 'Core.Customers.ErrorDeleteManager' and [LanguageId] = 1

Update [Settings].[Localization]
set [ResourceValue] = 'There are {1}, assigned to manager {0}'
where [ResourceKey] = 'Core.Customers.ErrorDeleteManager' and [LanguageId] = 2

GO--

delete  from Settings.Redirect where redirectfrom in ('login.aspx', 'feedback.aspx', 'search.aspx', 'registration.aspx', 'fogotpassword.aspx', 'forgotpassword.aspx', 'myaccount.aspx', 'productlist.aspx', 'shoppingcart.aspx', 'compareproducts.aspx', 'wishlist.aspx', 'productlist.aspx?type=new', 'productlist.aspx?type=bestseller', 'productlist.aspx?type=discount', 'giftcertificate.aspx', 'orderconfirmation.aspx', 'articles/maincategory/aprilmarketing', 'articles/maincategory/photopickup' )

GO--

GO--

ALTER TABLE [Catalog].[Product] 
ADD ManualRatio float NULL

GO--

ALTER PROCEDURE [Catalog].[sp_AddProduct]
    @ArtNo nvarchar(100) = '',
    @Name nvarchar(255),
    @Ratio float,
    @Discount float,
    @DiscountAmount float,
    @Weight float,
    @BriefDescription nvarchar(max),
    @Description nvarchar(max),
    @Enabled tinyint,
    @Recomended bit,
    @New bit,
    @BestSeller bit,
    @OnSale bit,
    @BrandID int,
    @AllowPreOrder bit,
    @UrlPath nvarchar(150),
    @Unit nvarchar(50),
    @ShippingPrice float,
    @MinAmount float,
    @MaxAmount float,
    @Multiplicity float,
    @HasMultiOffer bit,
    @SalesNote nvarchar(50),
    @GoogleProductCategory nvarchar(500),
    @YandexMarketCategory nvarchar(500),
    @Gtin nvarchar(50),
    @Adult bit,
    @Length float,
    @Width float,
    @Height float,
    @CurrencyID int,
    @ActiveView360 bit,
    @ManufacturerWarranty bit,
    @ModifiedBy nvarchar(50),
    @YandexTypePrefix nvarchar(500),
    @YandexModel nvarchar(500),
    @BarCode nvarchar(50),
    @Bid float,
    @AccrueBonuses bit,
    @Taxid int,
    @PaymentSubjectType int,
    @PaymentMethodType int,
    @YandexSizeUnit nvarchar(10),
    @DateModified datetime,
    @YandexName nvarchar(255),
    @YandexDeliveryDays nvarchar(5),
    @CreatedBy nvarchar(50),
    @Hidden bit,
    @ManualRatio float
AS
BEGIN
    Declare @Id int
    INSERT INTO [Catalog].[Product]
        ([ArtNo]
        ,[Name]
        ,[Ratio]
        ,[Discount]
        ,[DiscountAmount]
        ,[Weight]
        ,[BriefDescription]
        ,[Description]
        ,[Enabled]
        ,[DateAdded]
        ,[DateModified]
        ,[Recomended]
        ,[New]
        ,[BestSeller]
        ,[OnSale]
        ,[BrandID]
        ,[AllowPreOrder]
        ,[UrlPath]
        ,[Unit]
        ,[ShippingPrice]
        ,[MinAmount]
        ,[MaxAmount]
        ,[Multiplicity]
        ,[HasMultiOffer]
        ,[SalesNote]
        ,GoogleProductCategory
        ,YandexMarketCategory
        ,Gtin
        ,Adult
        ,Length
        ,Width
        ,Height
        ,CurrencyID
        ,ActiveView360
        ,ManufacturerWarranty
        ,ModifiedBy
        ,YandexTypePrefix
        ,YandexModel
        ,BarCode
        ,Bid
        ,AccrueBonuses
        ,TaxId
        ,PaymentSubjectType
        ,PaymentMethodType
        ,YandexSizeUnit
        ,YandexName
        ,YandexDeliveryDays
        ,CreatedBy
        ,Hidden
        ,ManualRatio
        )
    VALUES
        (@ArtNo
        ,@Name
        ,@Ratio
        ,@Discount
        ,@DiscountAmount
        ,@Weight
        ,@BriefDescription
        ,@Description
        ,@Enabled
        ,@DateModified
        ,@DateModified
        ,@Recomended
        ,@New
        ,@BestSeller
        ,@OnSale
        ,@BrandID
        ,@AllowPreOrder
        ,@UrlPath
        ,@Unit
        ,@ShippingPrice
        ,@MinAmount
        ,@MaxAmount
        ,@Multiplicity
        ,@HasMultiOffer
        ,@SalesNote
        ,@GoogleProductCategory
        ,@YandexMarketCategory
        ,@Gtin
        ,@Adult
        ,@Length
        ,@Width
        ,@Height
        ,@CurrencyID
        ,@ActiveView360
        ,@ManufacturerWarranty
        ,@ModifiedBy
        ,@YandexTypePrefix
        ,@YandexModel
        ,@BarCode
        ,@Bid
        ,@AccrueBonuses
        ,@TaxId
        ,@PaymentSubjectType
        ,@PaymentMethodType
        ,@YandexSizeUnit
        ,@YandexName
        ,@YandexDeliveryDays
        ,@CreatedBy
        ,@Hidden
        ,@ManualRatio
        );

    SET @ID = SCOPE_IDENTITY();
    if @ArtNo=''
    begin
        set @ArtNo = Convert(nvarchar(100),@ID)

        WHILE (SELECT COUNT(*) FROM [Catalog].[Product] WHERE [ArtNo] = @ArtNo) > 0
        begin
            SET @ArtNo = @ArtNo + '_A'
        end

        UPDATE [Catalog].[Product] SET [ArtNo] = @ArtNo WHERE [ProductID] = @ID
    end
    Select @ID
END

GO--

ALTER PROCEDURE [Catalog].[sp_UpdateProductById]
    @ProductID int,
    @ArtNo nvarchar(100),
    @Name nvarchar(255),
    @Ratio float,
    @Discount float,
    @DiscountAmount float,
    @Weight float,
    @BriefDescription nvarchar(max),
    @Description nvarchar(max),
    @Enabled bit,
    @Recomended bit,
    @New bit,
    @BestSeller bit,
    @OnSale bit,
    @BrandID int,
    @AllowPreOrder bit,
    @UrlPath nvarchar(150),
    @Unit nvarchar(50),
    @ShippingPrice money,
    @MinAmount float,
    @MaxAmount float,
    @Multiplicity float,
    @HasMultiOffer bit,
    @SalesNote nvarchar(50),
    @GoogleProductCategory nvarchar(500),
    @YandexMarketCategory nvarchar(500),
    @Gtin nvarchar(50),
    @Adult bit,
    @Length float,
    @Width float,
    @Height float,
    @CurrencyID int,
    @ActiveView360 bit,
    @ManufacturerWarranty bit,
    @ModifiedBy nvarchar(50),
    @YandexTypePrefix nvarchar(500),
    @YandexModel nvarchar(500),
    @BarCode nvarchar(50),
    @Bid float,
    @AccrueBonuses bit,
    @TaxId int,
    @PaymentSubjectType int,
    @PaymentMethodType int,
    @YandexSizeUnit nvarchar(10),
    @DateModified datetime,
    @YandexName nvarchar(255),
    @YandexDeliveryDays nvarchar(5),
    @CreatedBy nvarchar(50),
    @Hidden bit,
    @ManualRatio float
AS
BEGIN
    UPDATE [Catalog].[Product]
    SET 
         [ArtNo] = @ArtNo
        ,[Name] = @Name
        ,[Ratio] = @Ratio
        ,[Discount] = @Discount
        ,[DiscountAmount] = @DiscountAmount
        ,[Weight] = @Weight
        ,[BriefDescription] = @BriefDescription
        ,[Description] = @Description
        ,[Enabled] = @Enabled
        ,[Recomended] = @Recomended
        ,[New] = @New
        ,[BestSeller] = @BestSeller
        ,[OnSale] = @OnSale
        ,[DateModified] = @DateModified
        ,[BrandID] = @BrandID
        ,[AllowPreOrder] = @AllowPreOrder
        ,[UrlPath] = @UrlPath
        ,[Unit] = @Unit
        ,[ShippingPrice] = @ShippingPrice
        ,[MinAmount] = @MinAmount
        ,[MaxAmount] = @MaxAmount
        ,[Multiplicity] = @Multiplicity
        ,[HasMultiOffer] = @HasMultiOffer
        ,[SalesNote] = @SalesNote
        ,[GoogleProductCategory]=@GoogleProductCategory
        ,[YandexMarketCategory]=@YandexMarketCategory
        ,[Gtin]=@Gtin
        ,[Adult] = @Adult
        ,[Length] = @Length
        ,[Width] = @Width
        ,[Height] = @Height
        ,[CurrencyID] = @CurrencyID
        ,[ActiveView360] = @ActiveView360
        ,[ManufacturerWarranty] = @ManufacturerWarranty
        ,[ModifiedBy] = @ModifiedBy
        ,[YandexTypePrefix] = @YandexTypePrefix
        ,[YandexModel] = @YandexModel
        ,[BarCode] = @BarCode
        ,[Bid] = @Bid
        ,[AccrueBonuses] = @AccrueBonuses
        ,[TaxId] = @TaxId
        ,[PaymentSubjectType] = @PaymentSubjectType
        ,[PaymentMethodType] = @PaymentMethodType
        ,[YandexSizeUnit] = @YandexSizeUnit
        ,[YandexName] = @YandexName
        ,[YandexDeliveryDays] = @YandexDeliveryDays
        ,[CreatedBy] = @CreatedBy
        ,[Hidden] = @Hidden
        ,[Manualratio] = @ManualRatio
    WHERE ProductID = @ProductID
END

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.exportImport.ProductFields.ManualRatio', 'Ручной рейтинг товара')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.exportImport.ProductFields.ManualRatio', 'Manual rating')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Product.LeftMenu.Reviews', 'Отзывы и рейтинг')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Product.LeftMenu.Reviews', 'Reviews and rating')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Catalog.SetAllProductsManualRatio', 'Выставить вручную для всех продуктов')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Catalog.SetAllProductsManualRatio', 'Set manual rating to all products')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Catalog.SetAllProductsManualRatioBtn', 'Выставить')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Catalog.SetAllProductsManualRatioBtn', 'Set rating')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.ProductReviews.Title', 'Отзывы и рейтинг')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.ProductReviews.Title', 'Reviews and rating')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.ProductReviews.Ratio', 'Рейтинг')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.ProductReviews.Ratio', 'Rating')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.ProductReviews.CurrentRatio', 'Текущий рейтинг')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.ProductReviews.CurrentRatio', 'Current rating')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.ProductReviews.ManualRatio', 'Выставить вручную')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.ProductReviews.ManualRatio', 'Set manual rating')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.ProductReviews.Reviews', 'Отзывы')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.ProductReviews.Reviews', 'Reviews')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.ProductReviews.SetManualRatio', 'Выставить')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.ProductReviews.SetManualRatio', 'Set rating')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.SalesChannels.SalesChannel.OnlineStore.Details.Title', 'Интернет магазин')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.SalesChannels.SalesChannel.OnlineStore.Details.Title', 'Online store')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.SalesChannels.SalesChannel.OnlineStore.Details.Text', 'Многофункциональный интернет-магазин позволит вам представить весь ваш ассортимент продукции с возможностью поиска, фильтрации, сравнения товаров.<br /><br /><ul><li>Личный кабинет постоянного покупателя.</li><li>Автоматический расчет стоимости доставки от различных служб доставок, онлайн-оплата.</li><li>Новые маркетинговые инструменты для увеличения конверсии, среднего чека и повторных продаж.</li></ul>')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.SalesChannels.SalesChannel.OnlineStore.Details.Text', 'Multifunctional online store will allow you to submit your entire product range with the ability to search, filter, compare products.<br/> <br/> <ul> <li> Personal account of the regular customer. </ li> <li> Automatic calculation of the cost of delivery from various delivery services, online payment. </li> <li> New marketing tools for increase conversion, average check and repeat sales. </li> </ul>')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.SalesChannels.SalesChannel.SalesFunnels.Details.Title', 'Воронки продаж')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.SalesChannels.SalesChannel.SalesFunnels.Details.Title', 'Sales funnels')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.SalesChannels.SalesChannel.SalesFunnels.Details.Text', 'Воронки продаж отлично подходят для продажи узкого ассортимента товаров. Воронка продаж максимально концентрирует покупателя на одном целевом действии на каждой странице.Сама же воронка продаж обязательно состоит из последовательности страниц, шагов, которые покупатель должен пройти.<br><br>В нашем конструкторе уже собраны основные типы воронок для продажи товаров, механику которых вы можете изучить по инструкциям и схемам, подставить свои товары,и сразу же воспользоваться этим новым, высококонверсионным инструментом продаж.')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.SalesChannels.SalesChannel.SalesFunnels.Details.Text', 'Sales funnels are great for selling a narrow range of products. The sales funnel maximally concentrates the buyer on one target action on each page. The sales funnel itself consists of a sequence of pages, steps that the buyer must follow. <br> <br> In our designer, we have already collected the main types of funnels for selling goods You can study the instructions and schemes, substitute their products, and immediately take advantage of this new, high conversion sales tool.')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.SalesChannels.SalesChannel.Triggers.Details.Title', 'Триггерный маркетинг')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.SalesChannels.SalesChannel.Triggers.Details.Title', 'Trigger Marketing')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.SalesChannels.SalesChannel.Triggers.Details.Text', 'Триггерный маркетинг - это технология, которая позволяет коммуницировать с клиентом до и после покупки. Триггер срабатывает после какого-либо события или действия клиента на сайте, и ему автоматически отправляется письмо или несколько писем - триггерные рассылки.<br /><br>Триггерные рассылки - это автоматические письма или SMS со спец. предложениями. Задача триггерных рассылок — превратить потенциального клиента в вашего покупателя, и возвращать его в ваш магазин за новыми покупками.<br /><br>С помощью триггерных рассылок можно:<ul><li>побудить потенциального клиента купить в вашем магазине</li><li>простимулировать купить дополнительные товары после заказа, или мотивировать клиента на новый заказ</li><li>возвращать покупателей в магазин за новыми покупками</li></ul>')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.SalesChannels.SalesChannel.Triggers.Details.Text', 'Trigger marketing is a technology that allows you to communicate with the customer before and after the purchase. The trigger is triggered after a client’s event or action on the site, and a letter or several emails are automatically sent to it — trigger messages. <br /> <br> Trigger mailings are automatic letters or SMS with specials. offers. The task of trigger emails is to turn a potential customer into your customer, and return it to your store for new purchases. <br /> <br> With the help of trigger mailings you can: <ul> <li> induce a potential customer to buy in your shop </ li > <li> stimulate the purchase of additional goods after an order, or motivate a customer for a new order </ li> <li> return customers to the store for new purchases </ li> </ ul>')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.SalesChannels.SalesChannel.Yandex.Details.Title', 'Яндекс.Маркет')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.SalesChannels.SalesChannel.Yandex.Details.Title', 'Yandex Market')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.SalesChannels.SalesChannel.Yandex.Details.Text', 'Добавьте товары на торговую площадку Яндекс.Маркет, и потенциальные покупатели смогут найти ваш товар и купить его, перейдя в ваш интернет-магазин.<br /><br />Выгружайте выбранные вами товарные категории автоматически по расписанию в Яндекс.Маркет. Оплачивайте сервису Яндекс.Маркет за клики по кнопке перехода в магазин. Расскажите на Яндекс.Маркете об акциях или о подарках в вашем интернет-магазине. Увеличивайте посещаемость и количество продаж. Указывайте в прайс-листе, какие товары участвуют в акции и какие подарки может получить покупатель.')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.SalesChannels.SalesChannel.Yandex.Details.Text', 'Add products to the Yandex.Market marketplace, and potential buyers will be able to find your product and buy it by going to your online store. <br /> <br /> Upload your selected product categories automatically according to the schedule to Yandex.Market. Pay Yandex.Market service for clicks on the button to go to the store. Tell Yandex.Market about promotions or gifts in your online store. Increase traffic and sales. Specify in the price list, what products are involved in the promotion and what gifts the buyer can receive.')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.SalesChannels.SalesChannel.Google.Details.Title', 'Google Merchant Center')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.SalesChannels.SalesChannel.Google.Details.Title', 'Google Merchant Center')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.SalesChannels.SalesChannel.Google.Details.Text', 'Загрузите сведения о товарах в Google Merchant Center и привлекайте больше потенциальных клиентов. Показывайте рекламу тем, кто смотрит товары в Google,на YouTube и на других сайтах. Вы в любой момент сможете обновлять данные, чтобы они всегда были актуальными.<br><br>Товарное объявление содержит: название товара с картинкой, стоимость и название магазина. При нажатии на объявление ваш потенциальный клиент автоматическипереходит на страницу товара на вашем сайте. Вы оплачиваете только переходы, а не показы. Для рекламы товаров через этот сервис необходимо зарегистрировать в Merchant Center свой аккаунт.')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.SalesChannels.SalesChannel.Google.Details.Text', 'Upload product information to Google Merchant Center and attract more potential customers. Advertise to those who watch products on Google, YouTube and other sites. You can update the data at any time so that it will always be relevant. <br> <br> A product ad contains: the name of the product with a picture, the cost and the name of the store. When you click on the ad, your potential client automatically goes to the product page on your site. You pay only conversions, not impressions. To advertise products through this service, you must register your account in the Merchant Center.')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.SalesChannels.SalesChannel.Avito.Details.Title', 'Avito')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.SalesChannels.SalesChannel.Avito.Details.Title', 'Avito')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.SalesChannels.SalesChannel.Avito.Details.Text', 'Привлекайте больше клиентов, используя дополнительную площадку продаж. Загружайте товары в Avito из магазина автоматически по расписаниюи редактируйте прайс-лист из магазина Avito. Автоматизируйте процесс размещения, редактирования, продвижения и снятия объявлений с публикации.')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.SalesChannels.SalesChannel.Avito.Details.Text', 'Attract more customers using the additional sales platform. Download products to Avito from the store automatically on a schedule and edit the price list from the Avito store. Automate the process of placing, editing, promoting and removing ads from publication.')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.SalesChannels.SalesChannel.Vk.Details.Title', 'ВКонтакте')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.SalesChannels.SalesChannel.Vk.Details.Title', 'VK')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.SalesChannels.SalesChannel.Vk.Details.Text', 'Выгружайте ваши товары в магазин ВКонтакте, а всю переписку с клиентом ведите прямо в CRM интернет-магазина.<br /><br />Канал продаж ВКонтакте позволяет автоматически по расписанию обновлять каталог товаров прямо в ADVANTSHOP, чтобы поддерживать актуальность.<br /><br />Кроме того, фиксируйте любое обращение в группе как лид, и продавайте больше и чаще. Все личные сообщения группы, комментарии к постам, заявки натовары будут сразу же попадать в ADVANTSHOP, где вы сможете сразу же отвечать клиентам, отправлять ссылки на оплату, сформировывать заказы, работатьсо списком клиентов, и проводить маркетинговые активности. Так ни одно обращение клиента не останется без ответа, а вам не придется переключаться между интерфейсами сервисов.')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.SalesChannels.SalesChannel.Vk.Details.Text', 'Upload your products to the VKontakte store, and conduct all correspondence with the customer directly to the CRM online store. <br /> <br /> The VKontakte sales channel allows you to automatically update the product catalog directly on ADVANTSHOP, in order to maintain relevance. <br /> <br /> In addition, record any appeal in the group as a lead, and sell more and more often. All personal messages of the group, comments on posts, requests to the commodities will immediately go to ADVANTSHOP, where you can immediately respond to customers, send links to pay, create orders, work with a list of customers, and conduct marketing activities. So, no customer request will remain unanswered, and you will not have to switch between the interfaces of the services.')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.SalesChannels.SalesChannel.Bonus.Details.Title', 'Бонусная программа')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.SalesChannels.SalesChannel.Bonus.Details.Title', 'Bonus program')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.SalesChannels.SalesChannel.Bonus.Details.Text', 'Cоздавайте партнерскую программу для увеличения ваших продаж, регистрируйте партнеров в ADVANTSHOP. Эффективно отслеживайте статистику и комиссионные выплаты партнерам.<br /><br />Для ваших партнеров мы разработали личный кабинет, благодаря которому партнер сможет отслеживать количество подключенных клиентов и ежедневное вознаграждение.')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.SalesChannels.SalesChannel.Bonus.Details.Text', 'Create an affiliate program to increase your sales, register partners in ADVANTSHOP. Effectively track statistics and commission payments to partners. <br /> <br /> We have developed a personal account for your partners, thanks to which a partner will be able to track the number of connected clients and daily rewards.')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.SalesChannels.SalesChannel.Partners.Details.Title', 'Партнерская программа')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.SalesChannels.SalesChannel.Partners.Details.Title', 'Affiliate program')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.SalesChannels.SalesChannel.Partners.Details.Text', 'Cоздавайте партнерскую программу для увеличения ваших продаж, регистрируйте партнеров в ADVANTSHOP. Эффективно отслеживайте статистику и комиссионные выплаты партнерам.<br><br>Для ваших партнеров мы разработали личный кабинет, благодаря которому партнер сможет отслеживать количество подключенных клиентов и ежедневное вознаграждение.')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.SalesChannels.SalesChannel.Partners.Details.Text', 'Create an affiliate program to increase your sales, register partners in ADVANTSHOP. Effectively track statistics and commission payments to partners. <br> <br> We have developed a personal account for your partners, thanks to which a partner will be able to track the number of connected customers and daily rewards.')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.SalesChannels.SalesChannel.Reseller.Details.Title', 'Реселлеры')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.SalesChannels.SalesChannel.Reseller.Details.Title', 'Resellers')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.SalesChannels.SalesChannel.Reseller.Details.Text', 'Вы можете создать собственную сеть интернет-магазинов, перепродающих вашу продукцию. Такой подход позволит вам увеличить свою прибыль.<br><br>Регистрируйте интернет-магазины ресселеров и присваивайте каждому из них уникальный идентификатор. Вы как поставщик одним нажатием кнопки можете автоматически актуализировать данные по каталогу в магазинах ресселеров. Каталог будет обновляться с полным списком характеристик и фото.<br><br>Увеличивайте число каналов продаж, создавайте собственную сеть магазинов!')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.SalesChannels.SalesChannel.Reseller.Details.Text', 'You can create your own network of online stores that resell your products. This approach will allow you to increase your profits. <br> <br> Register resellers online stores and assign each of them a unique identifier. You, as a supplier, can automatically update catalog data in reseller stores with one click of a button. The catalog will be updated with a complete list of features and photos. <br> <br> Increase the number of sales channels, create your own chain of stores!')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.SalesChannels.SalesChannel.Instagram.Details.Title', 'Instagram')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.SalesChannels.SalesChannel.Instagram.Details.Title', 'Instagram')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.SalesChannels.SalesChannel.Instagram.Details.Text', 'Обрабатывайте обращений из Instagram прямо внутри ADVANTSHOP. Все личные сообщения группы, комментарии к постам, заявки на товары будут сразу же попадать в ADVANTSHOP,где вы сможете сразу же отвечать клиентам, отправлять ссылки на оплату, сформировывать заказы, работать со списком клиентов. Вы сможете быстро реагировать на комментарии и не потеряете ни один заказ.')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.SalesChannels.SalesChannel.Instagram.Details.Text', 'Handle Instagram posts right inside ADVANTSHOP. All personal messages of the group, comments on posts, requests for goods will immediately go to ADVANTSHOP, where you can immediately respond to customers, send links to pay, form orders, work with a list of customers. You will be able to respond quickly to comments and not lose any order.')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.SalesChannels.SalesChannel.Ok.Details.Title', 'Одноклассники')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.SalesChannels.SalesChannel.Ok.Details.Title', 'OK')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.SalesChannels.SalesChannel.Ok.Details.Text', 'Выгружайте ваши товары в магазин в Одноклассники, а всю переписку с клиентом ведите прямо в CRM интернет-магазина.<br /><br />Канал продаж Одноклассники позволяет автоматически по расписанию обновлять каталог товаров прямо в ADVANTSHOP, чтобы поддерживать актуальность.<br /><br />Кроме того, фиксируйте любое обращение в группе как лид, и продавайте больше и чаще. Так ни одно обращение клиента не останется без ответа, а вам не придется переключаться между интерфейсами сервисов.')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.SalesChannels.SalesChannel.Ok.Details.Text', 'Upload your goods to the store in Odnoklassniki, and send all correspondence with the client directly to the CRM online store. <br /> <br /> Odnoklassniki`s sales channel allows you to automatically update the catalog of goods right in ADVANTSHOP to keep up to date. <Br / > <br /> In addition, record any appeal in the group as a lead, and sell more and more often. So, no customer request will remain unanswered, and you will not have to switch between the interfaces of the services.')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.SalesChannels.SalesChannel.Telegram.Details.Title', 'Telegram')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.SalesChannels.SalesChannel.Telegram.Details.Title', 'Telegram')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.SalesChannels.SalesChannel.Telegram.Details.Text', 'Принимайте обращения ваших клиентов в чате магазина в Telegram и отвечайте прямо в CRM интернет-магазина, где автоматически фиксируются все покупатели.Так ни одно обращение клиента не останется без ответа, а вам не придется переключаться между интерфейсами сервисов.')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.SalesChannels.SalesChannel.Telegram.Details.Text', 'Accept the appeals of your customers in the chat shop in Telegram and respond directly to the CRM online store, where all customers are automatically recorded. So no customer request will remain unanswered, and you will not have to switch between the interfaces of the services.')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Catalog.PriceRegulation.ChangeDiscounts', 'Изменить скидки')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Catalog.PriceRegulation.ChangeDiscounts', 'Change discounts')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Catalog.CategoryDiscountRegulation', 'Регулирование скидок для категорий')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Catalog.CategoryDiscountRegulation', 'Category discount regulation')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.SettingsCatalog.CategoryDiscountRegulation.RegulationOfCategoryDiscount', 'Регулирование скидок для категорий')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.SettingsCatalog.CategoryDiscountRegulation.RegulationOfCategoryDiscount', 'Category discount regulation')

UPDATE [Settings].[Localization] SET [ResourceKey] = 'Admin.Settings.Catalog.PriceRegulation' WHERE ResourceKey = 'Admin.PriceRegulation.Index.Title'
UPDATE [Settings].[Localization] SET [ResourceKey] = 'Admin.Settings.Catalog.PriceRegulation.Increment' WHERE ResourceKey = 'Admin.PriceRegulation.Index.Increment'
UPDATE [Settings].[Localization] SET [ResourceKey] = 'Admin.Settings.Catalog.PriceRegulation.Decrement' WHERE ResourceKey = 'Admin.PriceRegulation.Index.Decrement'
UPDATE [Settings].[Localization] SET [ResourceKey] = 'Admin.Settings.Catalog.PriceRegulation.IncBySupply' WHERE ResourceKey = 'Admin.PriceRegulation.Index.IncBySupply'
UPDATE [Settings].[Localization] SET [ResourceKey] = 'Admin.Settings.Catalog.PriceRegulation.By' WHERE ResourceKey = 'Admin.PriceRegulation.Index.By'
UPDATE [Settings].[Localization] SET [ResourceKey] = 'Admin.Settings.Catalog.PriceRegulation.AbsoluteValue' WHERE ResourceKey = 'Admin.PriceRegulation.Index.AbsoluteValue'
UPDATE [Settings].[Localization] SET [ResourceKey] = 'Admin.Settings.Catalog.PriceRegulation.SelectionOfGoods' WHERE ResourceKey = 'Admin.PriceRegulation.Index.SelectionOfGoods'
UPDATE [Settings].[Localization] SET [ResourceKey] = 'Admin.Settings.Catalog.PriceRegulation.AllGoods' WHERE ResourceKey = 'Admin.PriceRegulation.Index.AllGoods'
UPDATE [Settings].[Localization] SET [ResourceKey] = 'Admin.Settings.Catalog.PriceRegulation.GoodsFromCategories' WHERE ResourceKey = 'Admin.PriceRegulation.Index.GoodsFromCategories'
UPDATE [Settings].[Localization] SET [ResourceKey] = 'Admin.Settings.Catalog.PriceRegulation.ChangePrices' WHERE ResourceKey = 'Admin.PriceRegulation.Index.ChangePrices'

UPDATE [Settings].[Localization] SET [ResourceKey] = 'Admin.Settings.PriceRegulation.NoSelectedCategories' WHERE ResourceKey = 'Admin.PriceRegulation.Index.NoSelectedCategories'
UPDATE [Settings].[Localization] SET [ResourceKey] = 'Admin.Settings.PriceRegulation.DecrementMsg' WHERE ResourceKey = 'Admin.PriceRegulation.Index.DecrementMsg'
UPDATE [Settings].[Localization] SET [ResourceKey] = 'Admin.Settings.PriceRegulation.IncrementMsg' WHERE ResourceKey = 'Admin.PriceRegulation.Index.IncrementMsg'
UPDATE [Settings].[Localization] SET [ResourceKey] = 'Admin.Settings.PriceRegulation.IncBySupplyMsg' WHERE ResourceKey = 'Admin.PriceRegulation.Index.IncBySupplyMsg'

UPDATE [Settings].[Localization] SET [ResourceKey] = 'Admin.Js.SettingsCatalog.PriceRegulation.RegulationOfPrices' WHERE ResourceKey = 'Admin.Js.PriceRegulation.RegulationOfPrices'
UPDATE [Settings].[Localization] SET [ResourceKey] = 'Admin.Js.SettingsCatalog.PriceRegulation.Error' WHERE ResourceKey = 'Admin.Js.PriceRegulation.Error'

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.Settings.AddEdit301RedCtrl.SystemUrl', 'Нельзя создавать 301 редиректы для системных URL')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.Settings.AddEdit301RedCtrl.SystemUrl', 'Cannot create 301 redirects for system URLs')

GO--

ALTER TABLE CMS.LandingForm ADD
	ActionUpsellLpId int NULL
GO--

ALTER TABLE CMS.LandingForm ADD
	PostMessageRedirectDelay int NULL
GO--

ALTER TABLE CMS.LandingForm ADD
	PostMessageRedirectShowMessage bit NULL
GO--

CREATE TABLE Settings.SmsTemplateOnOrderChanging
	(
	Id int NOT NULL IDENTITY (1, 1),
	OrderStatusId int NOT NULL,
	SmsText nvarchar(MAX) NOT NULL,
	Enabled bit NOT NULL
	)  ON [PRIMARY]
	 TEXTIMAGE_ON [PRIMARY]
GO--

ALTER TABLE Settings.SmsTemplateOnOrderChanging ADD CONSTRAINT
	PK_SmsTemplateOnOrderChanging PRIMARY KEY CLUSTERED 
	(
	Id
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO--

ALTER TABLE Settings.SmsTemplateOnOrderChanging ADD CONSTRAINT
	FK_SmsTemplateOnOrderChanging_OrderStatus FOREIGN KEY
	(
	OrderStatusId
	) REFERENCES [Order].OrderStatus
	(
	OrderStatusID
	) ON UPDATE  NO ACTION 
	 ON DELETE  CASCADE 
	
GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Settings.Commonpage.DomainsManager', 'Менеджер доменов')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Settings.Commonpage.DomainsManager', 'Domain manager')


GO--

UPDATE [Settings].[InternalSettings] SET [settingValue] = '8.0.1' WHERE [settingKey] = 'db_version'