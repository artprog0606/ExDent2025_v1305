
update settings.settings set value = 'True' where name = 'StoreActive' 
update settings.settings set value = 'Modern' where name = 'Template' 
update settings.settings set value = 'logo.svg' where name = 'MainPageLogoFileName' 

go



insert into [Settings].[TemplateSettings] ([Template], [Name] ,[Value]) values ('Modern', 'Background', '_none')
insert into [Settings].[TemplateSettings] ([Template], [Name] ,[Value]) values ('Modern', 'BigCategoryImageHeight', '200')
insert into [Settings].[TemplateSettings] ([Template], [Name] ,[Value]) values ('Modern', 'BigCategoryImageWidth', '740')
insert into [Settings].[TemplateSettings] ([Template], [Name] ,[Value]) values ('Modern', 'BigProductImageHeight', '1000')
insert into [Settings].[TemplateSettings] ([Template], [Name] ,[Value]) values ('Modern', 'BigProductImageWidth', '1000')
insert into [Settings].[TemplateSettings] ([Template], [Name] ,[Value]) values ('Modern', 'BrandCarouselVisibility', 'False')
insert into [Settings].[TemplateSettings] ([Template], [Name] ,[Value]) values ('Modern', 'BrandLogoHeight', '80')
insert into [Settings].[TemplateSettings] ([Template], [Name] ,[Value]) values ('Modern', 'BrandLogoWidth', '100')
insert into [Settings].[TemplateSettings] ([Template], [Name] ,[Value]) values ('Modern', 'CarouselAnimationDelay', '10000')
insert into [Settings].[TemplateSettings] ([Template], [Name] ,[Value]) values ('Modern', 'CarouselAnimationSpeed', '500')
insert into [Settings].[TemplateSettings] ([Template], [Name] ,[Value]) values ('Modern', 'CarouselVisibility', 'True')
insert into [Settings].[TemplateSettings] ([Template], [Name] ,[Value]) values ('Modern', 'CheckOrderVisibility', 'True')
insert into [Settings].[TemplateSettings] ([Template], [Name] ,[Value]) values ('Modern', 'CollapseAdvantagesOnMainModern', 'True')
insert into [Settings].[TemplateSettings] ([Template], [Name] ,[Value]) values ('Modern', 'ColorScheme', '_none')
insert into [Settings].[TemplateSettings] ([Template], [Name] ,[Value]) values ('Modern', 'CountCatalogProductInLine', '3')
insert into [Settings].[TemplateSettings] ([Template], [Name] ,[Value]) values ('Modern', 'CountCategoriesInLine', '4')
insert into [Settings].[TemplateSettings] ([Template], [Name] ,[Value]) values ('Modern', 'CountMainPageCategoriesInLine', '3')
insert into [Settings].[TemplateSettings] ([Template], [Name] ,[Value]) values ('Modern', 'CountMainPageCategoriesInSection', '3')
insert into [Settings].[TemplateSettings] ([Template], [Name] ,[Value]) values ('Modern', 'CountMainPageProductInLine', '3')
insert into [Settings].[TemplateSettings] ([Template], [Name] ,[Value]) values ('Modern', 'CountMainPageProductInSection', '3')
insert into [Settings].[TemplateSettings] ([Template], [Name] ,[Value]) values ('Modern', 'DefaultLogo', 'templates/Modern/images/logo.svg')
insert into [Settings].[TemplateSettings] ([Template], [Name] ,[Value]) values ('Modern', 'DefaultSlides', 'https://img.advstatic.ru/templates/modern/carousel/slide1.jpg;https://img.advstatic.ru/templates/modern/carousel/slide2.jpg;https://img.advstatic.ru/templates/modern/carousel/slide3.jpg;https://img.advstatic.ru/templates/modern/carousel/slide1_one_clmn.jpg;https://img.advstatic.ru/templates/modern/carousel/slide2_one_clmn.jpg;https://img.advstatic.ru/templates/modern/carousel/slide3_one_clmn.jpg;https://img.advstatic.ru/templates/modern/carousel/slide1_mobile.jpg;https://img.advstatic.ru/templates/modern/carousel/slide2_mobile.jpg;https://img.advstatic.ru/templates/modern/carousel/slide3_mobile.jpg')
insert into [Settings].[TemplateSettings] ([Template], [Name] ,[Value]) values ('Modern', 'GiftSertificateVisibility', 'True')
insert into [Settings].[TemplateSettings] ([Template], [Name] ,[Value]) values ('Modern', 'Header', 'Type1')
insert into [Settings].[TemplateSettings] ([Template], [Name] ,[Value]) values ('Modern', 'HideDisplayToolBarBottomOption', 'True')
insert into [Settings].[TemplateSettings] ([Template], [Name] ,[Value]) values ('Modern', 'LogoImageHeight', '22')
insert into [Settings].[TemplateSettings] ([Template], [Name] ,[Value]) values ('Modern', 'LogoImageWidth', '203')
insert into [Settings].[TemplateSettings] ([Template], [Name] ,[Value]) values ('Modern', 'MainPageCategoriesVisibility', 'True')
insert into [Settings].[TemplateSettings] ([Template], [Name] ,[Value]) values ('Modern', 'MainPageMode', 'TwoColumns')
insert into [Settings].[TemplateSettings] ([Template], [Name] ,[Value]) values ('Modern', 'MainPageProductsVisibility', 'True')
insert into [Settings].[TemplateSettings] ([Template], [Name] ,[Value]) values ('Modern', 'MenuStyle', 'Modern')
insert into [Settings].[TemplateSettings] ([Template], [Name] ,[Value]) values ('Modern', 'MiddleProductImageHeight', '700')
insert into [Settings].[TemplateSettings] ([Template], [Name] ,[Value]) values ('Modern', 'MiddleProductImageWidth', '700')
insert into [Settings].[TemplateSettings] ([Template], [Name] ,[Value]) values ('Modern', 'Mobile_BrowserColor', '2D9CEE')

insert into [Settings].[TemplateSettings] ([Template], [Name] ,[Value]) values ('Modern', 'NewsImageHeight', '140')
insert into [Settings].[TemplateSettings] ([Template], [Name] ,[Value]) values ('Modern', 'NewsImageWidth', '140')
insert into [Settings].[TemplateSettings] ([Template], [Name] ,[Value]) values ('Modern', 'NewsSubscriptionVisibility', 'True')
insert into [Settings].[TemplateSettings] ([Template], [Name] ,[Value]) values ('Modern', 'NewsVisibility', 'True')
insert into [Settings].[TemplateSettings] ([Template], [Name] ,[Value]) values ('Modern', 'PaymentIconHeight', '40')
insert into [Settings].[TemplateSettings] ([Template], [Name] ,[Value]) values ('Modern', 'PaymentIconWidth', '60')
insert into [Settings].[TemplateSettings] ([Template], [Name] ,[Value]) values ('Modern', 'RecentlyViewVisibility', 'True')
insert into [Settings].[TemplateSettings] ([Template], [Name] ,[Value]) values ('Modern', 'SearchBlockLocation', 'TopMenu')
insert into [Settings].[TemplateSettings] ([Template], [Name] ,[Value]) values ('Modern', 'ShippingIconHeight', '40')
insert into [Settings].[TemplateSettings] ([Template], [Name] ,[Value]) values ('Modern', 'ShippingIconWidth', '60')
insert into [Settings].[TemplateSettings] ([Template], [Name] ,[Value]) values ('Modern', 'ShowAdditionalPhones', 'True')
insert into [Settings].[TemplateSettings] ([Template], [Name] ,[Value]) values ('Modern', 'ShowNotAvaliableLable', 'True')
insert into [Settings].[TemplateSettings] ([Template], [Name] ,[Value]) values ('Modern', 'SmallCategoryImageHeight', '80')
insert into [Settings].[TemplateSettings] ([Template], [Name] ,[Value]) values ('Modern', 'SmallCategoryImageWidth', '120')
insert into [Settings].[TemplateSettings] ([Template], [Name] ,[Value]) values ('Modern', 'SmallProductImageHeight', '295')
insert into [Settings].[TemplateSettings] ([Template], [Name] ,[Value]) values ('Modern', 'SmallProductImageWidth', '295')
insert into [Settings].[TemplateSettings] ([Template], [Name] ,[Value]) values ('Modern', 'Theme', '_none')
insert into [Settings].[TemplateSettings] ([Template], [Name] ,[Value]) values ('Modern', 'TopMenu', 'Type1')
insert into [Settings].[TemplateSettings] ([Template], [Name] ,[Value]) values ('Modern', 'TopMenuVisibility', 'False')
insert into [Settings].[TemplateSettings] ([Template], [Name] ,[Value]) values ('Modern', 'TopPanel', 'Type1')
insert into [Settings].[TemplateSettings] ([Template], [Name] ,[Value]) values ('Modern', 'VotingVisibility', 'True')
insert into [Settings].[TemplateSettings] ([Template], [Name] ,[Value]) values ('Modern', 'WishListVisibility', 'True')
insert into [Settings].[TemplateSettings] ([Template], [Name] ,[Value]) values ('Modern', 'XSmallProductImageHeight', '120')
insert into [Settings].[TemplateSettings] ([Template], [Name] ,[Value]) values ('Modern', 'XSmallProductImageWidth', '120')


update  [Settings].[TemplateSettings] set value = 'True' where [Template] = 'Modern' and [Name] = 'ModernMainpageAdditionalText'
update  [Settings].[TemplateSettings] set value = 'True' where [Template] = 'Modern' and [Name] = 'ModernMainpageAdvantages'

if exists(select * from settings.settings where name='StoreScreenShot')
	begin
		update  [Settings].settings set value = '../Templates/Modern/preview.jpg' where [Name] = 'StoreScreenShot'
		update  [Settings].settings set value = '../Templates/Modern/preview.jpg' where  [Name] = 'StoreScreenShotMiddle'
	end
else
	begin
		insert into settings.settings (name, value) values ('StoreScreenShot', '../Templates/Modern/preview.jpg')
		insert into settings.settings (name, value) values ('StoreScreenShotMiddle', '../Templates/Modern/preview.jpg')
	end



GO
SET IDENTITY_INSERT [Catalog].[Photo] ON 
GO
INSERT [Catalog].[Photo] ([PhotoId], [ObjId], [Type], [PhotoName], [ModifiedDate], [Description], [PhotoSortOrder], [Main], [OriginName], [ColorID], [PhotoNameSize1], [PhotoNameSize2]) VALUES (9, 1, N'Carousel', N'https://img.advstatic.ru/templates/modern/carousel/slide1.jpg', CAST(0x0000ADCF0105588D AS DateTime), N'', 0, 0, N'', NULL, NULL, NULL)
GO
INSERT [Catalog].[Photo] ([PhotoId], [ObjId], [Type], [PhotoName], [ModifiedDate], [Description], [PhotoSortOrder], [Main], [OriginName], [ColorID], [PhotoNameSize1], [PhotoNameSize2]) VALUES (10, 2, N'Carousel', N'https://img.advstatic.ru/templates/modern/carousel/slide2.jpg', CAST(0x0000ADCF01055898 AS DateTime), N'', 0, 0, N'', NULL, NULL, NULL)
GO
INSERT [Catalog].[Photo] ([PhotoId], [ObjId], [Type], [PhotoName], [ModifiedDate], [Description], [PhotoSortOrder], [Main], [OriginName], [ColorID], [PhotoNameSize1], [PhotoNameSize2]) VALUES (11, 3, N'Carousel', N'https://img.advstatic.ru/templates/modern/carousel/slide3.jpg', CAST(0x0000ADCF01055899 AS DateTime), N'', 0, 0, N'', NULL, NULL, NULL)
GO
INSERT [Catalog].[Photo] ([PhotoId], [ObjId], [Type], [PhotoName], [ModifiedDate], [Description], [PhotoSortOrder], [Main], [OriginName], [ColorID], [PhotoNameSize1], [PhotoNameSize2]) VALUES (12, 4, N'Carousel', N'https://img.advstatic.ru/templates/modern/carousel/slide1_one_clmn.jpg', CAST(0x0000ADCF0105589C AS DateTime), N'', 0, 0, N'', NULL, NULL, NULL)
GO
INSERT [Catalog].[Photo] ([PhotoId], [ObjId], [Type], [PhotoName], [ModifiedDate], [Description], [PhotoSortOrder], [Main], [OriginName], [ColorID], [PhotoNameSize1], [PhotoNameSize2]) VALUES (13, 5, N'Carousel', N'https://img.advstatic.ru/templates/modern/carousel/slide2_one_clmn.jpg', CAST(0x0000ADCF010558A1 AS DateTime), N'', 0, 0, N'', NULL, NULL, NULL)
GO
INSERT [Catalog].[Photo] ([PhotoId], [ObjId], [Type], [PhotoName], [ModifiedDate], [Description], [PhotoSortOrder], [Main], [OriginName], [ColorID], [PhotoNameSize1], [PhotoNameSize2]) VALUES (14, 6, N'Carousel', N'https://img.advstatic.ru/templates/modern/carousel/slide3_one_clmn.jpg', CAST(0x0000ADCF010558A3 AS DateTime), N'', 0, 0, N'', NULL, NULL, NULL)
GO
INSERT [Catalog].[Photo] ([PhotoId], [ObjId], [Type], [PhotoName], [ModifiedDate], [Description], [PhotoSortOrder], [Main], [OriginName], [ColorID], [PhotoNameSize1], [PhotoNameSize2]) VALUES (15, 7, N'Carousel', N'https://img.advstatic.ru/templates/modern/carousel/slide1_mobile.jpg', CAST(0x0000ADCF010558A6 AS DateTime), N'', 0, 0, N'', NULL, NULL, NULL)
GO
INSERT [Catalog].[Photo] ([PhotoId], [ObjId], [Type], [PhotoName], [ModifiedDate], [Description], [PhotoSortOrder], [Main], [OriginName], [ColorID], [PhotoNameSize1], [PhotoNameSize2]) VALUES (16, 8, N'Carousel', N'https://img.advstatic.ru/templates/modern/carousel/slide2_mobile.jpg', CAST(0x0000ADCF010558A7 AS DateTime), N'', 0, 0, N'', NULL, NULL, NULL)
GO
INSERT [Catalog].[Photo] ([PhotoId], [ObjId], [Type], [PhotoName], [ModifiedDate], [Description], [PhotoSortOrder], [Main], [OriginName], [ColorID], [PhotoNameSize1], [PhotoNameSize2]) VALUES (17, 9, N'Carousel', N'https://img.advstatic.ru/templates/modern/carousel/slide3_mobile.jpg', CAST(0x0000ADCF010558A9 AS DateTime), N'', 0, 0, N'', NULL, NULL, NULL)
GO
SET IDENTITY_INSERT [Catalog].[Photo] OFF
GO
SET IDENTITY_INSERT [CMS].[Carousel] ON 

GO
INSERT [CMS].[Carousel] ([CarouselID], [URL], [SortOrder], [Enabled], [DisplayInOneColumn], [DisplayInTwoColumns], [DisplayInMobile], [Blank]) VALUES (1, N'', -2147483648, 1, 0, 1, 0, 0)
GO
INSERT [CMS].[Carousel] ([CarouselID], [URL], [SortOrder], [Enabled], [DisplayInOneColumn], [DisplayInTwoColumns], [DisplayInMobile], [Blank]) VALUES (2, N'', -2147483638, 1, 0, 1, 0, 0)
GO
INSERT [CMS].[Carousel] ([CarouselID], [URL], [SortOrder], [Enabled], [DisplayInOneColumn], [DisplayInTwoColumns], [DisplayInMobile], [Blank]) VALUES (3, N'', -2147483628, 1, 0, 1, 0, 0)
GO
INSERT [CMS].[Carousel] ([CarouselID], [URL], [SortOrder], [Enabled], [DisplayInOneColumn], [DisplayInTwoColumns], [DisplayInMobile], [Blank]) VALUES (4, N'', -2147483618, 1, 1, 0, 0, 0)
GO
INSERT [CMS].[Carousel] ([CarouselID], [URL], [SortOrder], [Enabled], [DisplayInOneColumn], [DisplayInTwoColumns], [DisplayInMobile], [Blank]) VALUES (5, N'', -2147483608, 1, 1, 0, 0, 0)
GO
INSERT [CMS].[Carousel] ([CarouselID], [URL], [SortOrder], [Enabled], [DisplayInOneColumn], [DisplayInTwoColumns], [DisplayInMobile], [Blank]) VALUES (6, N'', -2147483598, 1, 1, 0, 0, 0)
GO
INSERT [CMS].[Carousel] ([CarouselID], [URL], [SortOrder], [Enabled], [DisplayInOneColumn], [DisplayInTwoColumns], [DisplayInMobile], [Blank]) VALUES (7, N'', -2147483588, 1, 0, 0, 1, 0)
GO
INSERT [CMS].[Carousel] ([CarouselID], [URL], [SortOrder], [Enabled], [DisplayInOneColumn], [DisplayInTwoColumns], [DisplayInMobile], [Blank]) VALUES (8, N'', -2147483578, 1, 0, 0, 1, 0)
GO
INSERT [CMS].[Carousel] ([CarouselID], [URL], [SortOrder], [Enabled], [DisplayInOneColumn], [DisplayInTwoColumns], [DisplayInMobile], [Blank]) VALUES (9, N'', -2147483568, 1, 0, 0, 1, 0)
GO
SET IDENTITY_INSERT [CMS].[Carousel] OFF
GO


INSERT [CMS].[StaticBlock] ([Key], [InnerName], [Content], [Added], [Modified], [Enabled]) VALUES (N'ModernMainpageAdditionalText', N'Текст на главной (Шаблон Modern)', N'<div class="additional-text__main js-additional-text__main">
                <p>Наш интернет-магазин сделан для того, чтобы Вы смогли удобно, без лишних забот найти и заказать то, что Вас интересует. Мы заботимся о том, чтобы ассортимент в нашем интернет-магазине был всегда актуальным, цены доступными, сервис лучшим.<br /><br />
                Предлагаем Вам убедится в этом и сделать заказ в нашем интернет-магазине. Вы сможете быстро оплатить и получить заказ. Подробнее о вариантах оплаты и доставки Вы сможете узнать на соответствующих страницах.<br /><br />
                Для всех постоянных клиентов мы делаем скидки и заботимся о том, чтобы интернет-магазин Вам нравился и Вы приходили в него снова и снова.</p>
                <p>Наш интернет-магазин сделан для того, чтобы Вы смогли удобно, без лишних забот найти и заказать то, что Вас интересует. Мы заботимся о том, чтобы ассортимент в нашем интернет-магазине был всегда актуальным, цены доступными, сервис лучшим.</p>
            </div>', CAST(0x0000ADEB00E199C2 AS DateTime), CAST(0x0000ADEB00E199C2 AS DateTime), 1)
GO
INSERT [CMS].[StaticBlock] ([Key], [InnerName], [Content], [Added], [Modified], [Enabled]) VALUES (N'ModernMainpageAdvantages', N'Преимущества на главной (Шаблон Modern)', N'<article class="advantages cs-bg-4">
    <h2 hidden>Преимущества</h2>
    <div class="advantages-item">
        <div class="advantages-item-img cs-t-10">
          <svg width="34" height="42">
            <use xlink:href="Templates/Modern/spriteAdvantages.svg#shield"></use>
          </svg>
        </div>
        <div class="advantages-item-text cs-t-6">Акции и скидки для <br> постоянных клиентов</div>
    </div>

    <div class="advantages-item">
        <div class="advantages-item-img cs-t-10">
          <svg width="34" height="34">
            <use xlink:href="Templates/Modern/spriteAdvantages.svg#percent"></use>
          </svg>
        </div>
        <div class="advantages-item-text cs-t-6">Качественные услуги <br> и сервис</div>
    </div>

    <div class="advantages-item">
        <div class="advantages-item-img cs-t-10">
          <svg width="43" height="43">
            <use xlink:href="Templates/Modern/spriteAdvantages.svg#star"></use>
          </svg>
        </div>
        <div class="advantages-item-text cs-t-6">Широкий ассортимент <br> товаров</div>
    </div>

    <div class="advantages-item">
        <div class="advantages-item-img cs-t-10">
          <svg width="37" height="28">
            <use xlink:href="Templates/Modern/spriteAdvantages.svg#blocks"></use>
          </svg>
        </div>
        <div class="advantages-item-text cs-t-6">Широкий ассортимент <br> товаров</div>
    </div>
</article>', CAST(0x0000ADEB00E199C5 AS DateTime), CAST(0x0000ADEB00E199C5 AS DateTime), 1)
GO
INSERT [CMS].[StaticBlock] ([Key], [InnerName], [Content], [Added], [Modified], [Enabled]) VALUES (N'ModernHeadAddress', N'Адрес магазина (Шаблон Modern)', N'г.Москва, ул.Баумана<br> 18, офис 347', CAST(0x0000ADEB00E199C8 AS DateTime), CAST(0x0000ADEB00E199C8 AS DateTime), 1)
GO
INSERT [CMS].[StaticBlock] ([Key], [InnerName], [Content], [Added], [Modified], [Enabled]) VALUES (N'ModernHeadWorkingHours', N'Рабочие часы (Шаблон Modern)', N'ПН-ПТ 8:00-18:00<br> СБ-ВС выходные', CAST(0x0000ADEB00E199CA AS DateTime), CAST(0x0000ADEB00E199CA AS DateTime), 1)
GO

if not exists (Select 1 From [dbo].[DownloadableContent] Where [StringId] = 'Modern')
begin
  INSERT INTO [dbo].[DownloadableContent] ([StringId], [IsInstall], [DateAdded], [DateModified], [Active], [Version], [DcType])
                                   VALUES ('Modern', 1, getdate(), getdate(), 1, 'В режиме отладки', 'template')
end

GO