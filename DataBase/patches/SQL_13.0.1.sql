ALTER TABLE [Order].[OrderPickPoint] ALTER COLUMN
	[PickPointId] nvarchar(255) NOT NULL

GO--

UPDATE [Catalog].[Category] SET Enabled = 1 WHERE CategoryID = 0;

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Warehouses.Title', 'Магазины'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Warehouses.Title', 'Shops'

GO--

ALTER PROCEDURE [Catalog].[sp_ParseProductProperty]      
  @nameProperty nvarchar(100),      
  @propertyValue nvarchar(255),      
  @rangeValue float,    
  @productId int,      
  @sort int      
AS      
BEGIN      
 -- select or create property      
 Declare @propertyId int      
 if ((select count(PropertyID) from Catalog.[Property] where Name = @nameProperty)= 0)      
  begin      
   insert into Catalog.[Property] (Name,UseInFilter,UseInBrief,Useindetails,SortOrder,[type], NameDisplayed) values (@nameProperty,0,0,0,0,1, @nameProperty)      
   set @propertyId = (Select SCOPE_IDENTITY())      
  end      
 else      
  set @propertyId = (select top(1) PropertyID from Catalog.[Property] where Name = @nameProperty)      

  -- select or create value      
  Declare @propertyValueId int      

  Declare @useinfilter bit      
  set @useinfilter = (Select Top 1 UseInFilter from Catalog.[Property] Where PropertyID=@propertyId)      
  Declare @useindetails bit      
  set @useindetails = (Select Top 1 UseInDetails from Catalog.[Property] Where PropertyID=@propertyId)      

  if ((select count(PropertyValueID) from Catalog.[PropertyValue] where Value = @propertyValue and PropertyId=@propertyId)= 0)      
   begin      
    insert into Catalog.[PropertyValue] (PropertyId, Value, UseInFilter, UseInDetails, SortOrder, RangeValue) values (@propertyId, @propertyValue, @useinfilter, @useindetails, 0, @rangeValue)      
    set @propertyValueId = (Select SCOPE_IDENTITY())      
   end      
  else      
   set @propertyValueId = (select top(1) PropertyValueID from Catalog.[PropertyValue] where Value = @propertyValue and PropertyId=@propertyId)      

 --create link between product and property value      
 if ((select Count(*) from Catalog.ProductPropertyValue where ProductID=@productId and PropertyValueID=@propertyValueId)=0)      
  insert into Catalog.ProductPropertyValue (ProductID,PropertyValueID) values (@productId,@propertyValueId)       
END 

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Orders.GetOrderItems.StocksNo', N'Остатки отсутствуют.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Orders.GetOrderItems.StocksNo', N'There are no remains.'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Orders.GetOrderItems.StocksInWarehouse', N'{1} ед. на складе "{0}".'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Orders.GetOrderItems.StocksInWarehouse', N'{1} units in warehouse "{0}".'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Order.Stoks', N'Остаток по складам'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Order.Stoks', N'Stock balance'

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.ExportImport.MultiOrder.DeliveryDateTime', 'Дата доставки'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.ExportImport.MultiOrder.DeliveryDateTime', 'Delivery date' 

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Orders.DeliveryAddress', 'Адрес доставки'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Orders.DeliveryAddress', 'Delivery address' 

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Orders.BuyerZip', 'Индекс'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Orders.BuyerZip', 'Zip' 

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Orders.BuyerApartment', 'Квартира'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Orders.BuyerApartment', 'Apartment' 

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Orders.BuyerStreet', 'Улица'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Orders.BuyerStreet', 'Street' 

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Orders.BuyerHouse', 'Дом'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Orders.BuyerHouse', 'House' 

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Orders.BuyerStructure', 'Строение'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Orders.BuyerStructure', 'Structure' 

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.ImportProducts.AmountErrorByWarehouses', 'Колонка "{0}" не импортируется, т.к. существует больше 1 склада. Используйте колонки {1} для импорта кол-ва на определенный склад.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.ImportProducts.AmountErrorByWarehouses', 'The column "{0}" will not be imported because there is more than 1 warehouse. Use columns {1} to import a quantity to a specific warehouse.'

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.ImportProducts.AmountErrorByWarehousesCsv1', 'Значение кол-ва из колонки "{0}" не импортируется, т.к. существует больше 1 склада. Используйте CSV в формате AdvantShop 2.0 для импорта кол-ва на определенный склад.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.ImportProducts.AmountErrorByWarehousesCsv1', 'Amount from column "{0}" will not be imported because there is more than 1 warehouse. Use CSV AdvantShop 2.0 format to import a quantity to a specific warehouse.'

GO--

ALTER PROCEDURE [Customers].[sp_GetRecentlyView]
	@CustomerId uniqueidentifier,
	@rowsCount int,
	@Type nvarchar(50)
AS
BEGIN
	SET NOCOUNT ON;

	IF Exists (SELECT 1 FROM [Customers].[RecentlyViewsData] WHERE CustomerID=@CustomerId)
    Begin
        SELECT TOP(@rowsCount) Product.ProductID, Product.ArtNo, Product.Name, Product.UrlPath, Product.AllowPreOrder, Ratio, ManualRatio, isnull(PhotoNameSize1, PhotoName) as PhotoName,
            [Photo].[Description] as PhotoDescription, Discount, DiscountAmount, MinPrice as BasePrice, CurrencyValue,
            Offer.OfferID, MaxAvailable AS Amount, MinAmount, MaxAmount, Offer.Amount AS AmountOffer, Colors, NotSamePrices as MultiPrices,
            Product.DoNotApplyOtherDiscounts,  Units.DisplayName as UnitDisplayName,Units.Name as UnitName

        FROM [Customers].RecentlyViewsData

            Inner Join [Catalog].Product ON Product.ProductID = RecentlyViewsData.ProductId
            Left Join [Catalog].[ProductExt] ON [Product].[ProductID] = [ProductExt].[ProductID]
            Inner Join Catalog.Currency On Currency.CurrencyID = Product.CurrencyID
            Left Join [Catalog].[Photo] ON [Photo].[PhotoId] = [ProductExt].[PhotoId]
            Left Join [Catalog].[Offer] ON [ProductExt].[OfferID] = [Offer].[OfferID]
			Left Join [Catalog].[Units] ON [Product].[Unit] = [Units].[Id]

        WHERE RecentlyViewsData.CustomerID = @CustomerId AND Product.Enabled = 1 And CategoryEnabled = 1

        ORDER BY ViewDate Desc
    End
END

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Checkout.DefaultShippingType', 'Тип доставки по умолчанию'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Checkout.DefaultShippingType', 'Default shipping type' 

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Checkout.DefaultShippingTypeHint', 'Тип доставки, который будет предвыбран по умолчанию при оформлении заказа'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Checkout.DefaultShippingTypeHint', 'The type of delivery that will be selected by default when placing an order' 

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.ExportImport.CustomerFields.Tags', 'Теги'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.ExportImport.CustomerFields.Tags', 'Tags'

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.CustomerTags.SelectWithoutTags', 'Нет ни одного тега'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.CustomerTags.SelectWithoutTags', 'Without tags'

GO--

ALTER TABLE [CMS].[StaticBlock] ALTER COLUMN
	[Key] nvarchar(255) NOT NULL

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsBonus.Index.ProhibitAccrualAndSubstractBonuses', 'Запретить одновременное начисление и списание бонусов'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsBonus.Index.ProhibitAccrualAndSubstractBonuses', 'Prohibit simultaneous accrual and write-off of bonuses' 

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.Bonus.AddBonus', 'Начислить'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.Bonus.AddBonus', 'Accrue' 

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.Bonus.SubstractBonus', 'Списать'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.Bonus.SubstractBonus', 'Subsctract' 

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Orders.CustomersFullName', 'Покупатель'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Orders.CustomersFullName', 'Customer' 

GO--

ALTER TABLE [Order].[OrderPickPoint] ALTER COLUMN
	[PickPointId] nvarchar(255) NOT NULL

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.ExportImport.MultiOrder.OrderSource', 'Источник заказа'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.ExportImport.MultiOrder.OrderSource', 'Order source' 

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.ExportOrders.Source', 'Выгружать заказы c источником заказа'

GO--

ALTER TABLE [Catalog].[ProductList] 
	ADD ShowOnMainPage BIT NOT NULL DEFAULT 1;

GO--

INSERT [Settings].[Settings] (Name, Value)
VALUES
	('BestEnabled', (SELECT TOP(1)Value FROM [Settings].[Settings] WHERE Name = 'ShowBestOnMainPage')),
	('NewEnabled', (SELECT TOP(1)Value FROM [Settings].[Settings] WHERE Name = 'ShowNewOnMainPage')),
	('SalesEnabled', (SELECT TOP(1)Value FROM [Settings].[Settings] WHERE Name = 'ShowSalesOnMainPage'));

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.MainPageProductsStore.Index.ShowOnMainPage', 'Показывать на главной странице'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.MainPageProductsStore.Index.ShowOnMainPage', 'Show on main page'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.EditMainPageList.Enabled', 'Активность'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.EditMainPageList.Enabled', 'Activity'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.EditMainPageList.ShowOnMainPage', 'Показывать на главной странице'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.EditMainPageList.ShowOnMainPage', 'Show on main page'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.MainPageProductsStore.Index.ShowList', 'Активность'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.MainPageProductsStore.Index.ShowList', 'Activity'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.AddEditProductList.ShowOnMainPage', 'Показывать на главной странице'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.AddEditProductList.ShowOnMainPage', 'Show on main page'

GO--

create table dbo.TrackEventLogs
(
    Id        int identity
        constraint TrackEventLogs_pk
            primary key,
    EventKey  nvarchar(255) not null,
    UserAgent nvarchar(500),
    Ip        nvarchar(100),
    TrackedAtUTC datetime default GETUTCDATE() not null
)

create index TrackEventLogs_eventKey_index
    on dbo.TrackEventLogs (eventKey)

GO--

ALTER TABLE [Catalog].[Product]
ADD DownloadLink nvarchar(MAX) NULL;

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Catalog.Product.DownloadLink', 'Ссылка на скачивание'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Catalog.Product.DownloadLink', 'Download link' 

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Product.Edit.DownloadLink', 'Ссылка на скачивание'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Product.Edit.DownloadLink', 'Download link' 

GO--

DROP PROCEDURE [Catalog].[sp_UpdateProductById]

GO--

ALTER PROCEDURE [Catalog].[sp_AddProduct]
    @ArtNo nvarchar(100) = '',
    @Name nvarchar(255),
    @Ratio float,
    @Discount float,
    @DiscountAmount float,
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
    @Unit int,
    @ShippingPrice float,
    @MinAmount float,
    @MaxAmount float,
    @Multiplicity float,
    @HasMultiOffer bit,     
    @CurrencyID int,
    @ActiveView360 bit,
    @ModifiedBy nvarchar(50),   
    @AccrueBonuses bit,
    @Taxid int,
    @PaymentSubjectType int,
    @PaymentMethodType int,
    @DateModified datetime,
    @CreatedBy nvarchar(50),
    @Hidden bit,
    @ManualRatio float,
	@IsMarkingRequired bit,
	@Comment nvarchar(max),
	@DoNotApplyOtherDiscounts bit,
	@IsDigital bit,
	@DownloadLink nvarchar(max)
AS
BEGIN
    DECLARE @Id int,
			@ArtNoUpdateRequired bit

	IF @ArtNo=''
	BEGIN
		SET @ArtNo = CONVERT(nvarchar(100), NEWID())
		SET @ArtNoUpdateRequired = 1
	END

    INSERT INTO [Catalog].[Product]
        ([ArtNo]
        ,[Name]
        ,[Ratio]
        ,[Discount]
        ,[DiscountAmount]
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
        ,CurrencyID
        ,ActiveView360
        ,ModifiedBy
        ,AccrueBonuses
        ,TaxId
        ,PaymentSubjectType
        ,PaymentMethodType
        ,CreatedBy
        ,Hidden
        ,ManualRatio
		,IsMarkingRequired
		,Comment
		,DoNotApplyOtherDiscounts
		,IsDigital
		,DownloadLink
        )
    VALUES
        (@ArtNo
        ,@Name
        ,@Ratio
        ,@Discount
        ,@DiscountAmount
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
        ,@CurrencyID
        ,@ActiveView360
        ,@ModifiedBy
        ,@AccrueBonuses
        ,@TaxId
        ,@PaymentSubjectType
        ,@PaymentMethodType
        ,@CreatedBy
        ,@Hidden
        ,@ManualRatio
		,@IsMarkingRequired
        ,@Comment
		,@DoNotApplyOtherDiscounts
		,@IsDigital
		,@DownloadLink
		);

    SET @ID = SCOPE_IDENTITY();
    IF @ArtNoUpdateRequired = 1
    BEGIN
		DECLARE @NewArtNo nvarchar(100) = CONVERT(nvarchar(100),@ID)

        IF EXISTS (SELECT * FROM [Catalog].[Product] WHERE [ArtNo] = @NewArtNo)
        BEGIN
            SET @NewArtNo = @NewArtNo + '_' + SUBSTRING(@ArtNo, 1, 5)
        END

        UPDATE [Catalog].[Product] SET [ArtNo] = @NewArtNo WHERE [ProductID] = @ID
    END
    SELECT @ID
END

GO--

ALTER TABLE [Order].[OrderItems]
ADD DownloadLink nvarchar(MAX) NULL;

GO--

DROP PROCEDURE [Order].[sp_AddOrderItem]  

GO--

DROP PROCEDURE [Order].[sp_UpdateOrderItem]  

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Orders.Order.Letter.Download', 'Скачать'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Orders.Order.Letter.Download', 'Download' 

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Product.Edit.IsDigital', 'Цифровой товар'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Product.Edit.IsDigital', 'Digital product'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Product.Edit.IsDigital.Help', '<p>Товар будет считаться цифровым и появиться возможность внести ссылку на скачивание.</p><br /><b><p>Экспорт в Яндекс.Маркет и Merchant Center</p></b><p>В элементе downloadable (YML) будет указано значение true</p><p>Подробнее <a href="https://yandex.ru/support/marketplace/assortment/restrictions/digital.html" target="_blank">Цифровой товар</a></p>'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Product.Edit.IsDigital.Help', '<p>The product will be considered digital and there will be an option to enter a download link.</p>'

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.News.AddEdit.ENewsProductsDisplayMode.Vertically', 'Вертикально'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.News.AddEdit.ENewsProductsDisplayMode.Vertically', 'Vertically'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.News.AddEdit.ENewsProductsDisplayMode.Horizontally', 'Горизонтально'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.News.AddEdit.ENewsProductsDisplayMode.Horizontally', 'Horizontally'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.News.AddEdit.ProductsDisplayMode', 'Отображение продуктов'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.News.AddEdit.ProductsDisplayMode', 'Display of products'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.News.AddEdit.ProductsDisplayMode.HelpDiscription', 'Данная настройка используется только в <b>полной версии сайта</b>.<br><br>При выбранном варианте <b>Вертикально</b> - товары в новостях будут отображаться в боковом меню.<br><br>При выбранном варианте <b>Горизонтально</b> - товары в новостях будут отображаться под содержанием новости.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.News.AddEdit.ProductsDisplayMode.HelpDiscription', 'This setting is only used in the <b>full version of the site</b>.<br><br>If <b>Vertical</b> is selected - products in news will be displayed in the side menu.<br><br>If <b>Horizontal</b> is selected - products in news will be displayed below the news content.'

ALTER TABLE [Settings].[News] ADD [ProductsDisplayMode] INT NOT NULL DEFAULT 0

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Catalog.ShowProductArtNoOnProductCard', 'Показывать артикул товара'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Catalog.ShowProductArtNoOnProductCard', 'Display product sku' 

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Catalog.ShowProductArtNoOnProductCardHelp', 'Показывать артикул товара в карточке товара.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Catalog.ShowProductArtNoOnProductCardHelp', 'Show the SKU in the product card.' 

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.Builder.ShowProductArtNoOnProductCard', 'Показывать артикул товара'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.Builder.ShowProductArtNoOnProductCard', 'Display product sku' 

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.Builder.ShowProductArtNoOnProductCardHelp', 'Показывать артикул товара в карточке товара.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.Builder.ShowProductArtNoOnProductCardHelp', 'Show the SKU in the product card.' 

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Template.DeliveryWidgetVisibility', 'Отображать виджет доставки'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Template.DeliveryWidgetVisibility', 'Show delivery widget' 

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Template.DeliveryWidgetVisibilityHelp', 'Включает на главной странице виджет доставки, в котором можно изменить способ получения товара'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Template.DeliveryWidgetVisibilityHelp', 'Includes a delivery widget on the main page, in which you can change the method of receiving the goods' 

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Template.DeliveryWidget', 'Виджет доставки'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Template.DeliveryWidget', 'Delivery widget'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.Address.AddTitle', 'Добавление адреса'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.Address.AddTitle', 'Add address'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.Address.EditTitle', 'Редактирование адреса'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.Address.EditTitle', 'Edit address'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.Address.SelectAddress', 'Выберите адрес'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.Address.SelectAddress', 'Select address'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.Shipping.NotAvailableDeliveryPoints', 'Нет доступных пунктов самовывоза'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.Shipping.NotAvailableDeliveryPoints', 'No pickup locations available'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.Suggestion.Title', 'Вы находитесь здесь?'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.Suggestion.Title', 'You are here?'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.Suggestion.Confirm', 'Да'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.Suggestion.Confirm', 'Yes'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.Suggestion.Unconfirmed', 'Нет'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.Suggestion.Unconfirmed', 'No'

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.GeoMode.ShippingPoint', 'Пункты самовывоза'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.GeoMode.ShippingPoint', 'Pick-up points'

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Catalog.RemoveTagsToProducts', 'Удалить теги у товаров'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Catalog.RemoveTagsToProducts', 'Remove tags to products'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.RemoveTagsToProducts.RemovingTags', 'Удаление тегов у товаров'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.RemoveTagsToProducts.RemovingTags', 'Removing tags to products'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.RemoveTagsToProducts.Cancel', 'Отмена'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.RemoveTagsToProducts.Cancel', 'Cancel'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.RemoveTagsToProducts.RemoveTags', 'Удалить теги'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.RemoveTagsToProducts.RemoveTags', 'Remove tags'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.RemoveTagsToProducts.TagsRemovedSuccessfully', 'Теги успешно удалены'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.RemoveTagsToProducts.TagsRemovedSuccessfully', 'Tags removed successfully'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.RemoveTagsToProducts.Error', 'Удалить теги не удалось'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.RemoveTagsToProducts.Error', 'Failed to remove tags'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.RemoveTagsToProducts.MessageEmpty', 'У выбранных товаров отсутствуют теги.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.RemoveTagsToProducts.MessageEmpty', 'The selected products do not have tags.'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Catalog.RemovePropertyFromProducts', 'Удалить свойство у товаров'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.AddRemovePropertyToProducts.RemovingProperty', 'Удаление свойства у товаров'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.AddRemovePropertyToProducts.Remove', 'Удалить свойство'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.RemoveTagsToProducts.SelectTags', 'Выберите теги, которые хотите удалить'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.RemoveTagsToProducts.SelectTags', 'Select tags what you want to delete'

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.ExportImport.EProductFields.DownloadLink', 'Ссылка на скачивание'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.ExportImport.EProductFields.DownloadLink', 'Download link'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.ExportImport.ProductFields.DownloadLink', 'Ссылка на скачивание'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.ExportImport.ProductFields.DownloadLink', 'Download link'

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Catalog.ShowProductArtNoOnProductCard', 'Отображать артикул товара'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Catalog.ShowProductArtNoOnProductCard', 'Display product sku' 

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Catalog.ShowProductArtNoOnProductCardHelp', 'Отображать артикул товара в карточке товара.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Catalog.ShowProductArtNoOnProductCardHelp', 'Show the SKU in the product card.' 

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.Builder.ShowProductArtNoOnProductCard', 'Отображать артикул товара'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.Builder.ShowProductArtNoOnProductCard', 'Display product sku' 

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.Builder.ShowProductArtNoOnProductCardHelp', 'Отображать артикул товара в карточке товара.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.Builder.ShowProductArtNoOnProductCardHelp', 'Show the SKU in the product card.' 

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.AddEditSizeChart.Settings.ModalHeader', 'Заголовок окна'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.AddEditSizeChart.Settings.ModalHeader', 'Window title' 

IF NOT EXISTS(SELECT 1
              FROM sys.columns
              WHERE (name = N'ModalHeader') AND object_id = OBJECT_ID(N'[Catalog].[SizeChart]'))
BEGIN
	ALTER TABLE [Catalog].[SizeChart]
		ADD ModalHeader nvarchar(255) NULL
END

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.AddEditSizeChart.Settings.Categories', 'Категории'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.AddEditSizeChart.Settings.Categories', 'Categories'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.AddEditSizeChart.Settings.Products', 'Товары'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.AddEditSizeChart.Settings.Products', 'Products'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.AddEditSizeChart.Selected', 'выбрано'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.AddEditSizeChart.Selected', 'selected'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.AddEditSizeChart.NotSelected', 'Не выбрано'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.AddEditSizeChart.NotSelected', 'Not selected'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.AddEditSizeChart.Change', 'Изменить'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.AddEditSizeChart.Change', 'Change'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.AddEditSizeChart.Settings.Brands', 'Бренды'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.AddEditSizeChart.Settings.Brands', 'Brands'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.AddEditSizeChart.Settings.PropertyValues', 'Свойства'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.AddEditSizeChart.Settings.PropertyValues', 'Properties'

GO--

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Catalog].[SizeChartBrand]') AND type in (N'U'))
BEGIN
	CREATE TABLE [Catalog].[SizeChartBrand](
		[SizeChartId] [int] NOT NULL,
		[BrandId] [int] NOT NULL,
	 CONSTRAINT [PK_SizeChartBrand] PRIMARY KEY CLUSTERED 
	(
		[SizeChartId] ASC,
		[BrandId] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]
END
GO--

IF NOT EXISTS (SELECT 1 
			   FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS 
			   WHERE CONSTRAINT_NAME='FK_SizeChartBrand_Brand')
BEGIN
	ALTER TABLE [Catalog].[SizeChartBrand]  WITH CHECK ADD CONSTRAINT [FK_SizeChartBrand_Brand] FOREIGN KEY([BrandId])
	REFERENCES [Catalog].[Brand] ([BrandID])
	ON DELETE CASCADE
END

IF NOT EXISTS (SELECT 1 
			   FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS 
			   WHERE CONSTRAINT_NAME='FK_SizeChartBrand_SizeChart')
BEGIN
	ALTER TABLE [Catalog].[SizeChartBrand]  WITH CHECK ADD CONSTRAINT [FK_SizeChartBrand_SizeChart] FOREIGN KEY([SizeChartId])
	REFERENCES [Catalog].[SizeChart] ([Id])
	ON DELETE CASCADE
END

GO--

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Catalog].[SizeChartPropertyValue]') AND type in (N'U'))
BEGIN
	CREATE TABLE [Catalog].[SizeChartPropertyValue](
		[SizeChartId] [int] NOT NULL,
		[PropertyValueId] [int] NOT NULL,
	 CONSTRAINT [PK_SizeChartPropertyValue] PRIMARY KEY CLUSTERED 
	(
		[SizeChartId] ASC,
		[PropertyValueId] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END

GO--

IF NOT EXISTS (SELECT 1 
			   FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS 
			   WHERE CONSTRAINT_NAME='FK_SizeChartPropertyValue_PropertyValue')
BEGIN
	ALTER TABLE [Catalog].[SizeChartPropertyValue]  WITH CHECK ADD CONSTRAINT [FK_SizeChartPropertyValue_PropertyValue] FOREIGN KEY([PropertyValueId])
	REFERENCES [Catalog].[PropertyValue] ([PropertyValueId])
	ON DELETE CASCADE
END

IF NOT EXISTS (SELECT 1 
			   FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS 
			   WHERE CONSTRAINT_NAME='FK_SizeChartPropertyValue_SizeChart')
BEGIN
	ALTER TABLE [Catalog].[SizeChartPropertyValue]  WITH CHECK ADD CONSTRAINT [FK_SizeChartPropertyValue_SizeChart] FOREIGN KEY([SizeChartId])
	REFERENCES [Catalog].[SizeChart] ([Id])
	ON DELETE CASCADE
END

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Checkout.CheckoutShipping.Delivery', 'Курьер'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Checkout.CheckoutShipping.Delivery', 'Courier'

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.ShippingWithInterval.Settings.CountVisibleDeliveryDay', 'Количество дней, в течение которых можно назначить доставку'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.ShippingWithInterval.Settings.CountVisibleDeliveryDay', 'The number of days during which delivery can be scheduled'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.ShippingWithInterval.Settings.CountHiddenDeliveryDay', 'Количество дней, через которые можно назначить доставку'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.ShippingWithInterval.Settings.CountHiddenDeliveryDay', 'The number of days after which delivery can be scheduled'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.ShippingWithInterval.Settings.MinDeliveryTime', 'Количество минут, через которые можно назначить доставку'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.ShippingWithInterval.Settings.MinDeliveryTime', 'The number of minutes after which delivery can be scheduled'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.ShippingWithInterval.Settings.ShowSoonest', 'Возможность назначить доставку заказа "Как можно скорее"'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.ShippingWithInterval.Settings.ShowSoonest', 'The ability to schedule the delivery of the order "As soon as possible"'



GO--


EXEC [Settings].[sp_AddUpdateLocalization] 1, 'AdvantShop.Shipping.AdminModel.Point.NotSet', 'Не выбрано'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'AdvantShop.Shipping.AdminModel.Point.NotSet', 'Not selected'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.ShippingMethods.ApiShip.CityOfWarehouse', 'Город отправления'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.ShippingMethods.ApiShip.CityOfWarehouse', 'City of departure'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.ShippingMethods.ApiShip.ApiKey', 'ApiKey'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.ShippingMethods.ApiShip.ApiKey', 'ApiKey'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.ShippingMethods.ApiShip.ApiYndexMap', 'API-ключ яндекс.карт'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.ShippingMethods.ApiShip.ApiYndexMap', 'Yandex.maps API key'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.ShippingMethods.ApiShip.CountrySending', 'Страна отправки'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.ShippingMethods.ApiShip.CountrySending', 'Country of origin'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.ShippingMethods.ApiShip.CountryReturn', 'Страна возврата'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.ShippingMethods.ApiShip.CountryReturn', 'Country of return'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.ShippingMethods.ApiShip.SelectedPointForSendingOrder', 'Выбор точки для отправки заказа'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.ShippingMethods.ApiShip.SelectedPointForSendingOrder', 'Choosing a point for sending an order'

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.Settings.Template.DeliveryWidget.Delivery', 'Отображать в виджете доставки вкладку "Доставка"'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.Settings.Template.DeliveryWidget.Delivery', 'Display the "Delivery" tab in the delivery widget' 

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.Settings.Template.DeliveryWidget.Pickup', 'Отображать в виджете доставки вкладку "Самовывоз"'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.Settings.Template.DeliveryWidget.Pickup', 'Display the "Pickup" tab in the delivery widget' 


GO--

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Catalog].[Category_Warehouse]') AND type in (N'U'))
BEGIN
    CREATE TABLE [Catalog].[Category_Warehouse](
        [CategoryId] [int] NOT NULL,
        [WarehouseId] [int] NOT NULL,
        [HasProducts] [bit] NOT NULL,
    CONSTRAINT [PK_Category_Warehouse] PRIMARY KEY CLUSTERED 
    (
        [CategoryId] ASC,
        [WarehouseId] ASC
    )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
    ) ON [PRIMARY]
END

GO--

IF NOT EXISTS (SELECT 1 
			   FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS 
			   WHERE CONSTRAINT_NAME='FK_Category_Warehouse_Category')
BEGIN
    ALTER TABLE [Catalog].[Category_Warehouse]  WITH CHECK ADD  CONSTRAINT [FK_Category_Warehouse_Category] FOREIGN KEY([CategoryId])
    REFERENCES [Catalog].[Category] ([CategoryID])
    ON DELETE CASCADE
END

GO--

IF NOT EXISTS (SELECT 1 
			   FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS 
			   WHERE CONSTRAINT_NAME='FK_Category_Warehouse_Warehouse')
BEGIN
    ALTER TABLE [Catalog].[Category_Warehouse]  WITH CHECK ADD  CONSTRAINT [FK_Category_Warehouse_Warehouse] FOREIGN KEY([WarehouseId])
    REFERENCES [Catalog].[Warehouse] ([Id])
    ON DELETE CASCADE
END

GO--


IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Catalog].[HasAvailableProductsByCategoryAndWarehouse]') AND type in (N'FN', N'IF', N'TF', N'FS', N'FT'))
BEGIN
    EXEC dbo.sp_executesql @statement = N'CREATE FUNCTION [Catalog].[HasAvailableProductsByCategoryAndWarehouse] (@CategoryId INT, @WarehouseId INT) RETURNS BIT AS BEGIN RETURN 0 END' 
END

GO--

ALTER FUNCTION [Catalog].[HasAvailableProductsByCategoryAndWarehouse] (@CategoryId INT, @WarehouseId INT) RETURNS BIT 
AS
BEGIN
    if exists (Select 1 
               From Catalog.Category_Warehouse 
               Where CategoryId in (select id from Settings.GetChildCategoryByParent(@CategoryId) Where id <> @CategoryId) 
                        and WarehouseId = @WarehouseId 
                        and HasProducts = 1)
    BEGIN
        RETURN 1
    END

    RETURN isNull(
        (
            Select 1 
            From Catalog.Category 
            Where Category.CategoryId = @CategoryId 
                and Category.Enabled = 1 
                and Exists
                    (
                        Select 1 
                        From Catalog.Product 
                        Inner Join Catalog.ProductCategories ON ProductCategories.CategoryID = @CategoryId and ProductCategories.ProductId = Product.ProductID
                        Where Product.Enabled = 1 
                            and (
                                Product.AllowPreOrder = 1 
                                OR Exists( 
                                            Select 1 
                                            From Catalog.Offer 
                                            Inner Join Catalog.WarehouseStocks ON Offer.OfferID = WarehouseStocks.OfferId 
                                            Where WarehouseStocks.WarehouseId = @WarehouseId 
                                                and Offer.ProductId = Product.ProductID 
                                                and WarehouseStocks.Quantity > 0
                                         ) 
                                )
                    )
		), 0)
END;

GO--

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Catalog].[sp_CalcHasProductsForWarehouseInProductCategories]') AND type in (N'P', N'PC'))
BEGIN
    EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [Catalog].[sp_CalcHasProductsForWarehouseInProductCategories] AS' 
END

GO--

ALTER PROCEDURE [Catalog].[sp_CalcHasProductsForWarehouseInProductCategories]
    @OfferId INT,
    @WarehouseId INT
AS
BEGIN
    SET NOCOUNT ON;

    Declare @CategoryIds TABLE (CategoryId int);
    Declare @SortedCategoryIds TABLE (CategoryId int, CatLevel int);

    -- get product categories
    Insert @CategoryIds 
        Select ProductCategories.CategoryId 
        From Catalog.ProductCategories 
        Where ProductId = (Select ProductId From Catalog.Offer Where OfferId = @OfferId)    

    -- get parent categories (need check if child HasProducts will changed)
    ;with parents as 
    (
        Select CategoryId, ParentCategory, CatLevel
        From Catalog.Category 
        Where CategoryId in (Select CategoryId From @CategoryIds) 
        Union all
        Select c.CategoryId, c.ParentCategory, c.CatLevel 
        From Catalog.Category c
        Join parents p on c.CategoryId = p.ParentCategory and (c.CategoryId <> c.ParentCategory or c.CategoryId <> 0)
    ) 
    Insert @SortedCategoryIds	
        Select p.CategoryId, p.CatLevel 
        From (Select distinct CategoryId, CatLevel From parents) as p 
        Order by p.CatLevel desc, p.CategoryId asc

    -- add empty rows not exists in Category_Warehouse
    Insert Into Catalog.Category_Warehouse (CategoryId, WarehouseId, HasProducts) 
        Select CategoryId, @WarehouseId, 0 
        From @SortedCategoryIds 
        Where CategoryId not in (Select CategoryId From Catalog.Category_Warehouse Where WarehouseId = @WarehouseId)

    -- update in while loop cause HasAvailableProductsByCategoryAndWarehouse depends on the child categories in same table
	Declare @level int;
	Set @level = (Select Max(CatLevel) From @SortedCategoryIds);

	While @level > 0
	begin
		Update Catalog.Category_Warehouse 
		Set HasProducts = [Catalog].[HasAvailableProductsByCategoryAndWarehouse](CategoryIds.CategoryId, @WarehouseId) 
		From Catalog.Category_Warehouse 
		Inner Join @SortedCategoryIds as CategoryIds on CategoryIds.CategoryId = Category_Warehouse.CategoryId
		Where WarehouseId = @WarehouseId and CategoryIds.CatLevel = @level

		Set @level = @level - 1
	end
END;

GO--

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Catalog].[sp_CalcHasProductsForAllWarehousesInProductCategories]') AND type in (N'P', N'PC'))
BEGIN
    EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [Catalog].[sp_CalcHasProductsForAllWarehousesInProductCategories] AS' 
END

GO--

ALTER PROCEDURE [Catalog].[sp_CalcHasProductsForAllWarehousesInProductCategories]
    @ProductId INT
AS
BEGIN
    SET NOCOUNT ON;

    Declare @CategoryIds TABLE (CategoryId int);
	Declare @SortedCategoryIds TABLE (CategoryId int, CatLevel int);

    -- get product categories
    Insert @CategoryIds 
        Select ProductCategories.CategoryId 
        From Catalog.ProductCategories 
        Where ProductId = @ProductId

    -- get parent categories (need check if child HasProducts will changed)
	;with parents as 
    (
        Select CategoryId, ParentCategory, CatLevel
        From Catalog.Category 
        Where CategoryId in (Select CategoryId From @CategoryIds) 
        Union all
        Select c.CategoryId, c.ParentCategory, c.CatLevel 
        From Catalog.Category c
        Join parents p on c.CategoryId = p.ParentCategory and (c.CategoryId <> c.ParentCategory or c.CategoryId <> 0)
    ) 
    Insert @SortedCategoryIds	
        Select p.CategoryId, p.CatLevel 
        From (Select distinct CategoryId, CatLevel From parents) as p 
        Order by p.CatLevel desc, p.CategoryId asc

    -- join categories with warehouses
    Declare @CategoryIdWarehouseIdTb TABLE (CategoryId int, WarehouseId int, CatLevel int);

    Insert @CategoryIdWarehouseIdTb 
        Select CategoryId, Warehouse.Id, CatLevel 
		From @SortedCategoryIds 
        CROSS JOIN (Select Id From Catalog.Warehouse) as Warehouse

    -- add empty rows not exists in Category_Warehouse
    Insert Into Catalog.Category_Warehouse (CategoryId, WarehouseId, HasProducts) 
        Select tb.CategoryId, tb.WarehouseId, 0 
        From  Catalog.Category_Warehouse 
        Right Join @CategoryIdWarehouseIdTb as tb On tb.CategoryId = Category_Warehouse.CategoryId and tb.WarehouseId = Category_Warehouse.WarehouseId 
        Where Category_Warehouse.CategoryId is null 

    -- update in while loop cause HasAvailableProductsByCategoryAndWarehouse depends on the child categories in same table
	Declare @level int;
	Set @level = (Select Max(CatLevel) From @CategoryIdWarehouseIdTb);

	While @level > 0
	begin
		Update Catalog.Category_Warehouse 
		Set HasProducts = [Catalog].[HasAvailableProductsByCategoryAndWarehouse](tb.CategoryId, tb.WarehouseId) 
		From Catalog.Category_Warehouse 
		Inner Join @CategoryIdWarehouseIdTb as tb on tb.CategoryId = Category_Warehouse.CategoryId and tb.WarehouseId = Category_Warehouse.WarehouseId
		Where tb.CatLevel = @level

		Set @level = @level - 1
	end
END;

GO--

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Catalog].[sp_CalcHasProductsForAllWarehousesInAllCategories]') AND type in (N'P', N'PC'))
BEGIN
    EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [Catalog].[sp_CalcHasProductsForAllWarehousesInAllCategories] AS' 
END

GO--

ALTER PROCEDURE [Catalog].[sp_CalcHasProductsForAllWarehousesInAllCategories]
AS
BEGIN
    SET NOCOUNT ON;

    -- get all categories 
    DECLARE @CategoryIds TABLE (CategoryId int, CatLevel int);

    Insert @CategoryIds 
        Select CategoryId, CatLevel  
        From Catalog.Category 
        Where CategoryId <> 0 
        Order by CatLevel desc, CategoryId asc

    -- join categories with warehouses
    DECLARE @CategoryIdWarehouseIdTb TABLE (CategoryId int, WarehouseId int, CatLevel int);

    Insert @CategoryIdWarehouseIdTb 
        Select CategoryId, Warehouse.Id, CatLevel 
		From @CategoryIds 
        CROSS JOIN (Select Id From Catalog.Warehouse) as Warehouse

    -- add empty rows not exists in Category_Warehouse
    Insert Into Catalog.Category_Warehouse (CategoryId, WarehouseId, HasProducts) 
        Select tb.CategoryId, tb.WarehouseId, 0 
        From  Catalog.Category_Warehouse 
        Right Join @CategoryIdWarehouseIdTb as tb On tb.CategoryId = Category_Warehouse.CategoryId and tb.WarehouseId = Category_Warehouse.WarehouseId 
        Where Category_Warehouse.CategoryId is null 

    -- update in while loop cause HasAvailableProductsByCategoryAndWarehouse depends on the child categories in same table
	Declare @level int;
	Set @level = (Select Max(CatLevel) From @CategoryIdWarehouseIdTb);

	While @level > 0
	begin
		Update Catalog.Category_Warehouse 
		Set HasProducts = [Catalog].[HasAvailableProductsByCategoryAndWarehouse](tb.CategoryId, tb.WarehouseId) 
		From Catalog.Category_Warehouse 
		Inner Join @CategoryIdWarehouseIdTb as tb on tb.CategoryId = Category_Warehouse.CategoryId and tb.WarehouseId = Category_Warehouse.WarehouseId
		Where tb.CatLevel = @level

		Set @level = @level - 1
	end
END;

GO--

EXEC [Catalog].[sp_CalcHasProductsForAllWarehousesInAllCategories]

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Checkout.CheckoutShipping.ReceivingMethod.InHall', 'В зале'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Checkout.CheckoutShipping.ReceivingMethod.InHall', 'In the hall' 

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Checkout.CheckoutShipping.ReceivingMethod.WithYou', 'С собой'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Checkout.CheckoutShipping.ReceivingMethod.WithYou', 'With you' 

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Checkout.ShowReceivingMethod', 'Отображать способ получения заказа'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Checkout.ShowReceivingMethod', 'Display the order receipt method' 

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Checkout.ShowReceivingMethod.Help', 'При включенной опции будет отображаться переключатель "В зале"/"С собой"'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Checkout.ShowReceivingMethod.Help', 'When the option is enabled, the switch "In the hall"/"With you" will be displayed' 

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Checkout.EnTypeOfReceivingMethod.InHall', 'В зале'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Checkout.EnTypeOfReceivingMethod.InHall', 'In the hall' 

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Checkout.EnTypeOfReceivingMethod.WithYou', 'С собой'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Checkout.EnTypeOfReceivingMethod.WithYou', 'With you' 

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.OrderItemsSummary.ReceivingMethod.InHall', 'В зале'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.OrderItemsSummary.ReceivingMethod.InHall', 'In the hall' 

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.OrderItemsSummary.ReceivingMethod.WithYou', 'С собой'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.OrderItemsSummary.ReceivingMethod.WithYou', 'With you' 

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.OrderItemsSummary.ReceivingMethod', 'Cпособ получения'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.OrderItemsSummary.ReceivingMethod', 'Receiving method' 

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Checkout.CheckoutShipping.ReceivingMethod', 'Cпособ получения'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Checkout.CheckoutShipping.ReceivingMethod', 'Receiving method' 

IF NOT EXISTS(SELECT 1
              FROM sys.columns
              WHERE (name = N'ReceivingMethod') AND object_id = OBJECT_ID(N'[Order].[Order]'))
BEGIN
	ALTER TABLE [Order].[Order]
		ADD ReceivingMethod int NULL
END

GO--

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Catalog].[Warehouse_City]') AND type in (N'U'))
BEGIN
    CREATE TABLE [Catalog].[Warehouse_City](
        [WarehouseId] [int] NOT NULL,
        [CityId] [int] NOT NULL,
        [SortOrder] [int] NOT NULL,
    CONSTRAINT [PK_Warehouse_City] PRIMARY KEY CLUSTERED 
    (
        [WarehouseId] ASC,
        [CityId] ASC
    )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
    ) ON [PRIMARY]
END


GO--

IF NOT EXISTS (SELECT 1 
			   FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS 
			   WHERE CONSTRAINT_NAME='FK_Warehouse_City_City')
BEGIN
    ALTER TABLE [Catalog].[Warehouse_City]  WITH CHECK ADD  CONSTRAINT [FK_Warehouse_City_City] FOREIGN KEY([CityId])
    REFERENCES [Customers].[City] ([CityID])
    ON DELETE CASCADE
END

GO--

IF NOT EXISTS (SELECT 1 
			   FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS 
			   WHERE CONSTRAINT_NAME='FK_Warehouse_City_Warehouse')
BEGIN
    ALTER TABLE [Catalog].[Warehouse_City]  WITH CHECK ADD  CONSTRAINT [FK_Warehouse_City_Warehouse] FOREIGN KEY([WarehouseId])
    REFERENCES [Catalog].[Warehouse] ([Id])
    ON DELETE CASCADE
END

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Catalog.SizesColorsDisplayMode', 'Режим отображения'

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.Builder.MainPageVisibleBriefDescription', N'Отображать краткое описание на главной'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.Builder.MainPageVisibleBriefDescription', 'Display a short description on the main page'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.Builder.CatalogVisibleBriefDescription', N'Отображать краткое описание в каталоге'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.Builder.CatalogVisibleBriefDescription', 'Display short description in catalog'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.Builder.MainPageVisibleBriefDescriptionTitleLink', N'Настройка позволяет выводить краткое описание товаров на главной в десктопной версии при режимах отображения каталога "Плитка" и "Список"'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.Builder.MainPageVisibleBriefDescriptionTitleLink', 'The setting allows you to display a brief description of products on the main page in the desktop version when the catalog display modes are "Tile" and "List"'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.Builder.CatalogVisibleBriefDescriptionInTile', N'Настройка позволяет выводить краткое описание товаров в каталоге в десктопной версии при режимах отображения каталога "Плитка" и "Список"'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.Builder.CatalogVisibleBriefDescriptionInTile', 'The setting allows you to display a brief description of products in the catalog in the desktop version in the "Tile" and "List" catalog display modes.'

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Dashboard.TotalSites', 'Сайтов:'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Dashboard.TotalSites', 'Sites:'

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Product.CannotUploadPhoto', 'Изображение можно загрузить только после сохранения опции'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Product.CannotUploadPhoto', 'The image can be uploaded only after saving the option'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Product.UploadPhotoAfterSaveOption', 'Можно будет загрузить после сохранения опции'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Product.UploadPhotoAfterSaveOption', 'It will be possible to download after saving the option'

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Product.Edit.DoNotApplyOtherDiscountsHint', 'Будет учитываться только скидка товара. Другие скидки, купоны, скидки модулей и т.д. применяться к данному товару не будут. <br>Также для данного товара нельзя будет списать бонусы.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Product.Edit.DoNotApplyOtherDiscountsHint', 'Only the discount of the product will be taken into account. Other discounts, coupons, module discounts, etc. will not apply to this product. <br>Also, bonuses cannot be deducted for this product.'

GO--

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Settings].[DomainGeoLocation]') AND type in (N'U'))
BEGIN
    CREATE TABLE [Settings].[DomainGeoLocation](
        [Id] [int] IDENTITY(1,1) NOT NULL,
        [Url] [nvarchar](255) NOT NULL,
    CONSTRAINT [PK_DomainOfCity] PRIMARY KEY CLUSTERED 
    (
        [Id] ASC
    )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
    ) ON [PRIMARY]
END

GO--

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Settings].[DomainGeoLocation_City]') AND type in (N'U'))
BEGIN
    CREATE TABLE [Settings].[DomainGeoLocation_City](
        [DomainGeoLocationId] [int] NOT NULL,
        [CityId] [int] NOT NULL,
    CONSTRAINT [PK_DomainOfCity_City] PRIMARY KEY CLUSTERED 
    (
        [DomainGeoLocationId] ASC,
        [CityId] ASC
    )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
    ) ON [PRIMARY]
END

GO--

IF NOT EXISTS (SELECT 1 
			   FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS 
			   WHERE CONSTRAINT_NAME='FK_DomainGeoLocation_City_City')
BEGIN
    ALTER TABLE [Settings].[DomainGeoLocation_City]  WITH CHECK ADD  CONSTRAINT [FK_DomainGeoLocation_City_City] FOREIGN KEY([CityId])
    REFERENCES [Customers].[City] ([CityID])
    ON DELETE CASCADE
END

GO--

IF NOT EXISTS (SELECT 1 
			   FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS 
			   WHERE CONSTRAINT_NAME='FK_DomainGeoLocation_City_DomainGeoLocation')
BEGIN
    ALTER TABLE [Settings].[DomainGeoLocation_City]  WITH CHECK ADD  CONSTRAINT [FK_DomainGeoLocation_City_DomainGeoLocation] FOREIGN KEY([DomainGeoLocationId])
    REFERENCES [Settings].[DomainGeoLocation] ([Id])
    ON DELETE CASCADE
END

GO--

IF NOT EXISTS(SELECT 1
              FROM sys.columns
              WHERE (name = N'ModuleId') AND object_id = OBJECT_ID(N'[Catalog].[Coupon]'))
BEGIN
	ALTER TABLE [Catalog].[Coupon]
		ADD ModuleId nvarchar(100) NULL
END

GO--

UPDATE [Order].[ShippingReplaceGeo]
SET [OutRegionName] = 'Астана'
WHERE [Id] = 8

GO--

UPDATE [Order].[ShippingReplaceGeo]
SET [OutRegionName] = 'Астана'
WHERE [Id] = 9

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Analytics.Bonus', 'Бонусы'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Analytics.Bonus', 'Bonuses'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.BonusReport.BonusParticipants.Title', 'Участники бонусной программы'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.BonusReport.BonusParticipants.Title', 'Participants of the bonus program'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.BonusReport.BonusParticipants.All', 'за все время'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.BonusReport.BonusParticipants.All', 'all the time'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.BonusReport.BonusParticipants.New', 'новых за период'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.BonusReport.BonusParticipants.New', 'new for the period'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.BonusReport.BonusParticipants.HavingMovement', 'имеющие движение по карте'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.BonusReport.BonusParticipants.HavingMovement', 'having movement on the card'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.BonusReport.BonusParticipants.Series', 'Участников бонусной программы'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.BonusReport.BonusParticipants.Series', 'Bonus program participants'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.BonusReport.BonusMovement.Title', 'Движение бонусов'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.BonusReport.BonusMovement.Title', 'Bonus movement'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.BonusReport.BonusMovement.SumAccrued', 'начислено за период'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.BonusReport.BonusMovement.SumAccrued', 'accrued for the period'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.BonusReport.BonusMovement.SumUsed', 'использовано за период'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.BonusReport.BonusMovement.SumUsed', 'used for the period'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.BonusReport.BonusMovement.CountOrdersUsedBonus', 'заказов с использованием бонусов'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.BonusReport.BonusMovement.CountOrdersUsedBonus', 'orders using bonuses'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.BonusReport.BonusMovement.Series.Accrued', 'Начислено за период'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.BonusReport.BonusMovement.Series.Accrued', 'orders using bonuses'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.BonusReport.BonusMovement.Series.Used', 'Использовано за период'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.BonusReport.BonusMovement.Series.Used', 'orders using bonuses'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.BonusReport.BonusCardGrades.Title', 'Уровни бонусных карт'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.BonusReport.BonusCardGrades.Title', 'Bonus card levels'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.BonusReport.BonusCardGrades.Series', 'Бонусных карт'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.BonusReport.BonusCardGrades.Series', 'Bonus cards'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.BonusReport.BonusRules.Title', 'Авто начисления/списания'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.BonusReport.BonusRules.Title', 'Auto accruals/write-offs'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.BonusReport.BonusRules.Name', 'Правило'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.BonusReport.BonusRules.Name', 'Rule'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.BonusReport.BonusRules.Accrued', 'Начислено'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.BonusReport.BonusRules.Accrued', 'Accrued'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.BonusReport.BonusRules.Help', 'данные формируются по названию правила'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.BonusReport.BonusRules.Help', 'data is generated by rule name'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.BonusReport.TopUsersAccrued.Title', 'Топ 10 клиентов по начисленным бонусам'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.BonusReport.TopUsersAccrued.Title', 'Top 10 clients by accrued bonuses'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.BonusReport.TopUsersUsed.Title', 'Топ 10 клиентов по использованным бонусам'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.BonusReport.TopUsersUsed.Title', 'Top 10 clients by used bonuses'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.BonusReport.TopUsers.Name', 'Имя'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.BonusReport.TopUsers.Name', 'Name'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.BonusReport.TopUsers.Accrued', 'Начислено'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.BonusReport.TopUsers.Accrued', 'Accrued'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.BonusReport.TopUsers.Used', 'Использовано'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.BonusReport.TopUsers.Used', 'Used'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.BonusReport.TopUsers.OrdersCount', 'Кол-во заказов'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.BonusReport.TopUsers.OrdersCount', 'Orders count'

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.CustomOption.CustomOptionInputType.MultiCheckBox', 'Мультивыбор'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.CustomOption.CustomOptionInputType.MultiCheckBox', 'Multiselect'

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsCoupons.Index.DiscountsPriceRangeTitle', 'Скидки от суммы заказа'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsCoupons.Index.DiscountsPriceRangeTitle', 'Discounts on order sum'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsCoupons.Index.DiscountsByTimeTitle', 'Динамические скидки'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsCoupons.Index.DiscountsByTimeTitle', 'Dynamic discounts'


EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.DiscountsByTime.Index.Title', 'Динамические скидки'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.DiscountsByTime.Index.Title', 'Dynamic discounts'

GO--
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.NotifyEMails.AuthCall', 'Идентификация по звонку'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.NotifyEMails.AuthCall', 'Identification by call'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.SystemSettings.AuthByCode', 'Идентификация по коду'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.SystemSettings.AuthByCode', 'Identification by code'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.SystemSettings.AuthByCodeActive', 'Включен'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.SystemSettings.AuthByCodeActive', 'Active'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.SystemSettings.AuthByCodeMethod', 'Метод идентификации'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.SystemSettings.AuthByCodeMethod', 'Identification method'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Settings.SettingsOAuth.EAuthByCodeMethod.Sms', 'СМС'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Settings.SettingsOAuth.EAuthByCodeMethod.Sms', 'SMS'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Settings.SettingsOAuth.EAuthByCodeMethod.Call', 'Звонок'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Settings.SettingsOAuth.EAuthByCodeMethod.Call', 'Call'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.AuthCall.Title', 'Идентификация по звонку'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.AuthCall.Title', 'Identification by call'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.AuthCall.ActiveAuthCallModule', 'Подключенный модуль'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.AuthCall.ActiveAuthCallModule', 'Connected module'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.AuthCall.AuthCallMode', 'Тип уведомления'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.AuthCall.AuthCallMode', 'Notification type'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Calls.EAuthCall.Flash', 'Flash уведомление'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Calls.EAuthCall.Flash', 'Flash notifications'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Calls.EAuthCall.Voice', 'Voice уведомление'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Calls.EAuthCall.Voice', 'Voice notifications'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Settings.AuthCall.AuthCallMods.DefaultMode', 'Выберите тип'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Settings.AuthCall.AuthCallMods.DefaultMode', 'Select type'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Settings.AuthCall.AuthCallModules.DefaultModule', 'Выберите модуль'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Settings.AuthCall.AuthCallModules.DefaultModule', 'Select module'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Loging.CallAuthStatus.Sent', 'Отправлено'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Loging.CallAuthStatus.Sent', 'Sent'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Loging.CallAuthStatus.Error', 'Ошибка'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Loging.CallAuthStatus.Error', 'Error'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Loging.CallAuthStatus.Fault', 'Неисправность'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Loging.CallAuthStatus.Fault', 'Fault'

GO--

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Customers].[CallAuthCodeConfirmation]') AND type in (N'U'))
BEGIN
    CREATE TABLE [Customers].[CallAuthCodeConfirmation] (
        [Id] [int] IDENTITY(1,1) NOT NULL,
        [Phone] [bigint] NOT NULL,
        [Code] [nvarchar](10) NOT NULL,
        [IsUsed] [bit] NOT NULL,
        [DateAdded] [datetime] NOT NULL,
        CONSTRAINT [PK_CallAuthCodeConfirmation] PRIMARY KEY CLUSTERED
        (
            [Id] ASC
        ) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
    ) ON [PRIMARY];
END

GO--

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Customers].[CallAuthBan]') AND type in (N'U'))
BEGIN
    CREATE TABLE [Customers].[CallAuthBan] (
        [Phone] [bigint] NULL,
        [Ip] [nvarchar](100) NULL,
        [UntilDate] [datetime] NOT NULL
    ) ON [PRIMARY];
END

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Customers].[CallAuthLog]') AND type in (N'U'))
BEGIN
    CREATE TABLE [Customers].[CallAuthLog](
        [Id] [int] IDENTITY(1,1) NOT NULL,
        [Phone] [bigint] NOT NULL,
        [CreatedOn] [datetime] NOT NULL,
        [Ip] [nvarchar](100) NOT NULL,
        [CustomerId] [uniqueidentifier] NULL,
        [Status] [smallint] NOT NULL,
        CONSTRAINT [PK_CallAuthLog_1] PRIMARY KEY CLUSTERED
        (
            [Id] ASC
        )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
    ) ON [PRIMARY];
END

GO--

IF NOT EXISTS (Select 1 From [Settings].[Settings] Where Name = 'OpenIdProviderAuthByCodeActive')
BEGIN
    INSERT INTO [Settings].[Settings] (Name, Value)
    VALUES ('OpenIdProviderAuthByCodeActive', (SELECT isnull((SELECT TOP(1) Value FROM [Settings].[Settings] WHERE Name = 'OpenIdProviderSmsActive'), 'False')))
END

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Orders.GetOrderItems.StocksInWarehouse', '{1} ед. - "{0}"'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Orders.GetOrderItems.StocksInWarehouse', '{1} unit - "{0}"'

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Orders.GetOrderItems.StocksInWarehouseCollapse', 'Скрыть'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Orders.GetOrderItems.StocksInWarehouseCollapse', 'Collapse'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Orders.GetOrderItems.StocksInWarehouseExpand', 'Ещё'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Orders.GetOrderItems.StocksInWarehouseExpand', 'More'

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Import.ImportProducts.Warehouse', 'Склад'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Import.ImportProducts.Warehouse', 'Warehouse'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.API.Warehouse', 'Склад для остатков'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.API.Warehouse', 'Warehouse of stocks'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.ExportFeed.SettingsReseller.StocksFromWarehouses', 'Склады для остатков'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.ExportFeed.SettingsReseller.StocksFromWarehouses', 'Warehouses of stocks'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.ExportFeed.SettingsCsv.StocksFromWarehouses', 'Склады для остатков'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.ExportFeed.SettingsCsv.StocksFromWarehouses', 'Warehouses of stocks'

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.ExportFeeed.SettingsYandex.ExportPropertyDisplayedName', 'Экспорт публичного названия свойства товара'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.ExportFeeed.SettingsYandex.ExportPropertyDisplayedName', 'Exporting the public name of a product property'

DELETE FROM [Settings].[Localization] WHERE [ResourceKey] = 'Core.Services.Features.EFeature.FunnelBlocks';
DELETE FROM [Settings].[Localization] WHERE [ResourceKey] = 'Core.Services.Features.EFeature.FunnelBlocks.Description';

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.ExportFeeed.SettingsYandex.ExportPropertyDisplayedNameHelp', 'При экспорте товаров в формате "Яндекс Маркет (yml, xml)" в качестве названия свойства товара будет использоваться значение из параметра "Название в клиентской части"'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.ExportFeeed.SettingsYandex.ExportPropertyDisplayedNameHelp', 'When exporting products in the "Yandex Market (yml, xml)" format, the value from the "Name in the client side" parameter will be used as the name of the product property.'

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.CartAdd.Fail', 'Не удалось добавить товар в корзину'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.CartAdd.Fail', 'Failed to add item to cart'

GO--

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[Catalog].[RelatedProducts]') AND name = N'IX_RelatedProducts_ProductID_RelatedType')
begin
    CREATE NONCLUSTERED INDEX IX_RelatedProducts_ProductID_RelatedType ON [Catalog].[RelatedProducts] (ProductID, RelatedType)
end

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Warehouses.WarehousesTitle', 'Склады'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Warehouses.WarehousesTitle', 'Warehouses'

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Catalog.MoveNotAvaliableToEnd', 'Товары не в наличии или без цены перемещать в конец списка'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Catalog.MoveNotAvaliableToEnd', 'Items out of stock or without a price should be moved to the end of the list'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsCatalog.CatalogCommon.OptionToMoveProductsInTheEndList', 'При активации этой опции товары, которых нет в наличии или без цены, будут перемещаться в конец списка, несмотря на выбранную сортировку.<br/><br/> Подробнее:<br/><a href="https://www.advantshop.net/help/pages/catalog-view#9" target="_blank">Как переместить товары не в наличии в конец списка?</a>'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsCatalog.CatalogCommon.OptionToMoveProductsInTheEndList', 'When this option is activated, items that are out of stock or without a price will be moved to the end of the list, despite the selected sorting.<br/><br/> More:<br/><a href="https://www.advantshop.net/help/pages/catalog-view#9" target="_blank">How do I move out-of-stock items to the end of the list?</a>'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.Builder.MoveNotAvaliableToEnd', 'Товары не в наличии или без цены перемещать в конец списка'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.Builder.MoveNotAvaliableToEnd', 'Items out of stock or without a price should be moved to the end of the list'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.Builder.MoveNotAvaliableToEndHint', 'При активации этой опции товары, которых нет в наличии или без цены, будут перемещаться в конец списка, несмотря на выбранную сортировку.<br/><br/> Подробнее:<br/><a href="https://www.advantshop.net/help/pages/catalog-view#9" target="_blank">Как переместить товары не в наличии в конец списка?</a>'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.Builder.MoveNotAvaliableToEndHint', 'When this option is activated, items that are out of stock or without a price will be moved to the end of the list, despite the selected sorting.<br/><br/> More:<br/><a href="https://www.advantshop.net/help/pages/catalog-view#9" target="_blank">How do I move out-of-stock items to the end of the list?</a>'

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Catalog.ShowOnlyAvailableWarehousesInProduct', 'Показывать только склады с наличием товара';
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Catalog.ShowOnlyAvailableWarehousesInProduct', 'Show only available warehouses';

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.Bonus.TemporaryBonuses.Title', 'Успей потратить'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.Bonus.TemporaryBonuses.Title', 'Spend it now'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.Bonus.TemporaryBonuses.Name', 'Название'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.Bonus.TemporaryBonuses.Name', 'Name'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.Bonus.TemporaryBonuses.Amount', 'Сумма'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.Bonus.TemporaryBonuses.Amount', 'Amount'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.Bonus.TemporaryBonuses.StartDate', 'Действует от'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.Bonus.TemporaryBonuses.StartDate', 'Effective from'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.Bonus.TemporaryBonuses.EndDate', 'Действует до (вкл.)'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.Bonus.TemporaryBonuses.EndDate', 'Effective up to and including'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.Bonus.TransactionsMode.All', 'Все операции'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.Bonus.TransactionsMode.All', 'All operations'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.Bonus.TransactionsMode.Add', 'Зачисления'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.Bonus.TransactionsMode.Add', 'Enrollments'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.Bonus.TransactionsMode.Subtract', 'Списания'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.Bonus.TransactionsMode.Subtract', 'Writeoffs'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.Bonus.BonusHistory.Action', 'Действия'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.Bonus.BonusHistory.Action', 'Actions'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.Bonus.BonusHistory.Foundation', 'Основание'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.Bonus.BonusHistory.Foundation', 'Foundation'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.Bonus.BonusHistory.Amount', 'Бонусы'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.Bonus.BonusHistory.Amount', 'Bonuses'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.Bonus.Add', 'Зачисление'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.Bonus.Add', 'Enrollment'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.Bonus.Subtract', 'Списание'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.Bonus.Subtract', 'Writeoff'

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.Bonus.BonusHistory.Empty', 'Нет записей'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.Bonus.BonusHistory.Empty', 'No records'

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'MyAccount.OrderProductHistory.Header', 'Купленные товары'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'MyAccount.OrderProductHistory.Header', 'Purchased goods' 

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.MyAccount.OrderProductHistory.Sku', 'Артикул'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.MyAccount.OrderProductHistory.Sku', 'Sku'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.MyAccount.OrderProductHistory.LastOrder', 'Последний заказ товара'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.MyAccount.OrderProductHistory.LastOrder', 'Last order of good'

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.CheckOrder.Address', N'Адрес доставки'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.CheckOrder.Address', 'Address'

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.Bonus.BonusHistory.Empty', 'Нет записей'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.Bonus.BonusHistory.Empty', 'No records'

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsSystem.SystemCommon.SetCookieOnMainDomain', 'Использовать {0} как основной URL для cookies'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsSystem.SystemCommon.SetCookieOnMainDomain', 'Use {0} as main base domain for cookies'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.SystemSettings.SetCookieOnMainDomainHint', 'Данная настройка необходима для кросс-доменной работы. Cookies будут устанавливаться на главный домен и будут доступны на поддоменах. Например, у вас есть поддомены msk.site.ru, spb.site.ru и т.д. и вам необходимо, чтобы авторизованный покупатель мог свободно по ним перемещаться и при переходе не надо было каждый раз соглашаться на использование cookies или выбирать город. Cookies будут устанавливаться на .site.ru и будут общими для всех поддоменов. В этом случае активируйте эту настройку. В любых других случаях не используйте эту опцию.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.SystemSettings.SetCookieOnMainDomainHint', 'This setting is necessary for cross-domain operation. Cookies will be installed on the main domain and will be available on subdomains. For example, you have subdomains msk.site.ru , spb.site.ru and you need an authorized buyer to be able to freely navigate through them and not have to agree to use cookies every time or choose a city. Cookies will be installed on .site.ru and they will be common to all subdomains. In this case, activate this setting. In any other cases, do not use this option.'

GO--

UPDATE Catalog.Currency SET IsCodeBefore = 0 WHERE CurrencyNumIso3 = 643;
UPDATE Catalog.Currency SET IsCodeBefore = 0 WHERE CurrencyNumIso3 = 978;
UPDATE Catalog.Currency SET IsCodeBefore = 1 WHERE CurrencyNumIso3 = 840;

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.ExportFeed.SettingsYandex.ExportVendorInSimplifiedType', 'Выгружать тег vendor'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.ExportFeed.SettingsYandex.ExportVendorInSimplifiedType', 'Export vendor tag'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.ExportFeed.SettingsYandex.ExportVendorInSimplifiedTypeHelp', 'Выгружать тег vendor для упрощенного типа'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.ExportFeed.SettingsYandex.ExportVendorInSimplifiedTypeHelp', 'Export vendor tag for simlified type'

GO--

ALTER TABLE [Order].[Order] ADD
	WarehouseIdsJson nvarchar(MAX) NULL
	
GO--

update settings.Settings
set name = '_DELETE_' + Name
where name In(
'Email_4_ProductQuestion',
'YandexMarketDatafeedTitle',
'YandexMarketDatafeedDescription',
'YandexMarketFileName',
'YandexMarketSalesNotes',
'YandexMarketFileNameDirectory',
'YandexMarketCurrency',
'YandexMarketDescriptionSelection',
'YandexMarketShopName',
'YandexMarketCompanyName',
'YandexMarketDelivery',
'ShowSeeProductOnMainPage',
'ShowVotingOnMainPage',
'MiniBasketConditionalCount',
'ShowStatusCommentOnMainPage',
'MiniBasketConditional',
'DefaultPaymentMethod',
'CompressBigImage',
'CsvEnconing',
'CsvSeparator',
'CurrentSaasId',
'INN',
'RS',
'Director',
'Manager',
'Accountant',
'StampImage',
'MinimalPrice',
'BIK',
'BankName',
'KPP',
'KS',
'CompanyName',
'CarouseltVisibility',
'EnableSocialShareButtons',
'EnabledWWWRedirect',
'BigProductImageHeight',
'BigProductImageWidth',
'MiddleProductImageHeight',
'MiddleProductImageWidth',
'SmallProductImageHeight',
'SmallProductImageWidth',
'XSmallProductImageHeight',
'XSmallProductImageWidth',
'SmallCategoryImageHeight',
'SmallCategoryImageWidth',
'BigCategoryImageHeight',
'BigCategoryImageWidth',
'NewsImageHeight',
'NewsImageWidth',
'BrandLogoHeight',
'BrandLogoWidth',
'CarouselHeight',
'CarouselWidth',
'PaymentIconHeight',
'PaymentIconWidth',
'ShippingIconHeight',
'ShippingIconWidth',
'ModuleOrderConfirmationId',
'GoogleAnalyticsClientID',
'GoogleAnalyticsClientSecret',
'ChangePrices',
'CsvColumSeparator',
'CsvPropertySeparator',
'CsvExportNoInCategory',
'TelfinLogin',
'TelfinSecretKey',
'TelfinActive',
'ShowManagersPage',
'EnableManagersModule',
'TelphinLogin',
'TelphinSecretKey',
'EnableLoging',
'CallBack.ShowMode',
'ExportFeedTaskSqlSettings',
'StatisticsDashboardSetting',
'Avatar',
'NewsRssViewNews',
'Mobile_RedirectToSubdomain',
'SettingsVk.GroupId'
)

GO--

Delete from settings.Settings
where name In(
'QRUserID',
'GoogleBaseDatafeedTitle',
'GoogleBaseDatafeedDescription',
'GoogleBaseFileName',
'GoogleBaseSalesNotes',
'ShopzillaDatafeedTitle',
'ShopzillaDatafeedDescription',
'ShopzillaFileName',
'ShopzillaSalesNotes',
'AmazonDatafeedTitle',
'AmazonDatafeedDescription',
'AmazonFileName',
'AmazonSalesNotes',
'PriceGrabberDatafeedTitle',
'PriceGrabberDatafeedDescription',
'PriceGrabberFileName',
'PriceGrabberSalesNotes',
'GoogleBaseFileNameDirectory',
'GoogleBaseCurrency',
'YahooShoppingDatafeedTitle',
'YahooShoppingDatafeedDescription',
'YahooShoppingFileName',
'YahooShoppingSalesNotes',
'YahooShoppingFileNameDirectory',
'YahooShoppingCurrency',
'ShoppingComDatafeedTitle',
'ShoppingComDatafeedDescription',
'ShoppingComFileName',
'ShoppingComSalesNotes',
'ShoppingComFileNameDirectory',
'ShoppingComCurrency',
'ShopzillaFileNameDirectory',
'ShopzillaCurrency',
'ShopzillaDescriptionSelection',
'GoogleBaseDescriptionSelection',
'ShoppingComDescriptionSelection',
'YahooShoppingDescriptionSelection',
'PriceGrabberFileNameDirectory',
'PriceGrabberCurrency',
'PriceGrabberDescriptionSelection',
'AmazonFileNameDirectory',
'AmazonCurrency',
'AmazonDescriptionSelection',
'DEL_ShowCatalogOnMainPage',
'Del_EnableMainPageViewChangel',
'DEL_DesignTheme',
'DEL_ShowVersionOnMainPage',
'FootMenuSiteMap',
'FootMenuCatalog',
'OpenIdProviderTwitterActive',
'oidTwitterConsumerKey',
'oidTwitterConsumerSecret',
'oidTwitterAccessToken',
'EnableUserOnline',
'MailChimpId',
'MailChimpRegUsersList',
'MailChimpSubscribeUsersList',
'MailChimpAllUsersList',
'MailChimpActive',
'MailChimpNoRegUsersList',
'FreshdeskDomain',
'oidTwitterAccessTokenSecret',
'RitmzLogin',
'RitmzPassword',
'GoogleAnalyticsCachedData',
'GoogleAnalyticsCachedDate',
'WatermarkPosition',
'WatermarkImage',
'WatermarkPositionX',
'WatermarkPositionY',
'EnableWatermark',
'LandingPageCommonStatic'
)

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.ShippingMethods.DeliveryIntervals.IntervalNotSelected', 'Интервал доставки не выбран'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.ShippingMethods.DeliveryIntervals.IntervalNotSelected', 'The delivery interval is not selected'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.ShippingMethods.DeliveryIntervals.SelectedDateNotAvailable', 'Доставка в выбранную дату недоступна'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.ShippingMethods.DeliveryIntervals.SelectedDateNotAvailable', 'Delivery on the selected date is not available'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.ShippingMethods.DeliveryIntervals.SelectedIntervalNotAvailable', 'Доставка в выбранный интервал недоступна'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.ShippingMethods.DeliveryIntervals.SelectedIntervalNotAvailable', 'Delivery at the selected interval is not available'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.ShippingMethods.DeliveryIntervals.AsapUnavailable', 'Возможность оформить заказ "Как можно скорее" недоступна'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.ShippingMethods.DeliveryIntervals.AsapUnavailable', 'The option to place an order "As soon as possible" is not available'

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.ExportFeed.SettingsYandex.ExportCollections', 'Выгружать тег collections'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.ExportFeed.SettingsYandex.ExportCollections', 'Export collections tag'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.ExportFeed.SettingsYandex.ExportCollectionsHint', 'Выгружать тег collections и collectionId в offer'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.ExportFeed.SettingsYandex.ExportCollectionsHint', 'Export collections tag and collectionId tag in offer'

GO--

ALTER PROCEDURE [Catalog].[sp_AddCategory]
	@Name nvarchar(255),
	@ParentCategory int,
	@Description nvarchar(max),
	@BriefDescription nvarchar(max),
	@SortOrder int,
	@Enabled bit,
	@Hidden bit,
	@DisplayStyle int,
	@DisplayChildProducts bit,
	@DisplayBrandsInMenu bit,
	@DisplaySubCategoriesInMenu bit,
	@UrlPath nvarchar(150),
	@Sorting int,
	@ExternalId nvarchar(50),
	@AutomapAction int,
	@ModifiedBy nvarchar(50),
	@ShowOnMainPage bit
AS
BEGIN
	INSERT INTO [Catalog].[Category]
		([Name]
		,[ParentCategory]
		,[Description]
		,[BriefDescription]
		,[Products_Count]
		,[SortOrder]
		,[Enabled]
		,[Hidden]
		,[DisplayStyle]
		,[DisplayChildProducts]
		,[DisplayBrandsInMenu]
		,[DisplaySubCategoriesInMenu]
		,[UrlPath]
		,[Sorting]
		,[ExternalId]
		,[AutomapAction]
		,[ModifiedBy]
		,[ShowOnMainPage]
		,[CatLevel]
		)
	VALUES
		(@Name
		,@ParentCategory
		,@Description
		,@BriefDescription
		,0
		,@SortOrder
		,@Enabled
		,@Hidden
		,@DisplayStyle
		,@DisplayChildProducts
		,@DisplayBrandsInMenu
		,@DisplaySubCategoriesInMenu
		,@UrlPath
		,@Sorting
		,@ExternalId
		,@AutomapAction
		,@ModifiedBy
		,@ShowOnMainPage
		,(Case When @ParentCategory = 0 Then 2 Else (Select ISNULL([CatLevel], 0) From [Catalog].[Category] Where CategoryID = @ParentCategory) + 1 End)
		);

	DECLARE @CategoryId int = @@IDENTITY;
	if @ExternalId is null
		begin
			UPDATE [Catalog].[Category] SET [ExternalId] = @CategoryId WHERE [CategoryID] = @CategoryId
		end
	Select @CategoryId;   
END

GO--

ALTER PROCEDURE [Catalog].[sp_GetSizesByCategory]  
 @CategoryID int,  
 @indepth bit,  
 @OnlyAvailable bit
AS  
BEGIN  
 if(@inDepth = 1)  
 begin  
  Select *, [Category_Size].SizeNameForCategory 
  From Catalog.Size
  Left Join [Catalog].[Category_Size] On Size.SizeId = Category_Size.SizeId and Category_Size.CategoryId = @CategoryId
  Where Size.SizeID in   
  (
	Select distinct SizeID 
	From Catalog.Offer   
	Inner Join Catalog.Product on Offer.ProductID = Product.ProductID   
	Inner Join Catalog.ProductCategories on ProductCategories.ProductID = Product.ProductID and ProductCategories.CategoryID in (select id from Settings.GetChildCategoryByParent(@CategoryID))   
    Where Product.Enabled = 1 and Product.CategoryEnabled = 1 and (@OnlyAvailable = 0 OR Amount > 0)
  )  
  Order by Size.SortOrder, Size.SizeName  
 end  
 else  
 begin  
  Select *, [Category_Size].SizeNameForCategory 
  From Catalog.Size
  Left Join [Catalog].[Category_Size] On Size.SizeId = Category_Size.SizeId and Category_Size.CategoryId = @CategoryId
  Where Size.SizeID in   
  (
	Select distinct SizeID 
	From Catalog.Offer 
	Inner Join Catalog.Product on Offer.ProductID = Product.ProductID   
	Inner Join Catalog.ProductCategories on ProductCategories.ProductID = Product.ProductID and ProductCategories.CategoryID = @CategoryID 
	Where Product.Enabled = 1 and Product.CategoryEnabled = 1  and (@OnlyAvailable = 0 OR Amount > 0)
  )   
  Order by Size.SortOrder, Size.SizeName  
 end  
END

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.SettingsCatalog.CatalogCommon.OptionToDisplayProductsAreNotAvailable', 'Если опция включена, то в каталоге будут скрыты товары не в наличии. Также в каталоге будут скрыты пустые категории и категории без товаров в наличии.<br><br/><b>Внимание!</b> При включении данной опции рекомендуется выключить опцию "Показывать число товаров в категории", поскольку скрытые товары не в наличии будут учитываться в общем количестве товаров.<br/><br/> Подробнее:<br/><a href="https://www.advantshop.net/help/pages/catalog-view#4" target="_blank">Настройка вывода товаров в наличии не в наличии</a>'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.SettingsCatalog.CatalogCommon.OptionToDisplayProductsAreNotAvailable', 'If this option is enabled, out-of-stock items will be hidden in the catalog. Empty categories and categories without products in stock will also be hidden in the catalog.<br><br/><b>Attention!</b> When enabling this option, it is recommended to turn off the option "Show the number of products in the category", since hidden products are not in stock will be counted in the total number of products. <br/><br/> More details: <br/> <a href = "https://www.advantshop.net/help/pages/catalog- view # 4 "target =" _blank "> Configuring the display of out of stock items </a>'

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.Builder.ShowAvailableInWarehouseInProduct', 'Отображать наличие на складах'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.Builder.ShowAvailableInWarehouseInProduct', 'Display stock in warehouses' 

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.Builder.OptionShowAvailableInWarehouseInProduct', 'Отображать или нет количество складов в графе "наличие" у товара.<br><br>Например:<br>При включённой опции - В наличии на 3 складах (100 шт.)<br>При отключенной опции - В наличии (100 шт.)'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.Builder.OptionShowAvailableInWarehouseInProduct', 'Show or not number of warehouses in the "availability" field of the product.<br><br>For example:<br>With the enabled option - In stock in 3 warehouses (100 pcs.)<br>With the option disabled - In stock (100 pcs.).' 


EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.Builder.ShowOnlyAvailableWarehousesInProduct', 'Показывать только склады с наличием товара'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.Builder.ShowOnlyAvailableWarehousesInProduct', 'Show only available warehouses' 


EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.Builder.YaMapsApiKeyFoMapWarehouse', 'API-ключ Яндекс.Карт для отображения карты'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.Builder.YaMapsApiKeyFoMapWarehouse', 'Yandex.maps API key for displaying the map' 

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.Builder.YaMapsApiKeyFoMapWarehouseHelp', '<a href="https://yandex.ru/dev/maps/jsapi/doc/2.1/quick-start/index.html#get-api-key" target="_blank">Как получить ключ?</a>'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.Builder.YaMapsApiKeyFoMapWarehouseHelp', '<a href="https://yandex.ru/dev/maps/jsapi/doc/2.1/quick-start/index.html#get-api-key" target="_blank">How do I get the key?</a>'

GO--

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Catalog].[sp_RecalculateProductsCountInCategoriesByProduct]') AND type in (N'P', N'PC'))
BEGIN
    EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [Catalog].[sp_RecalculateProductsCountInCategoriesByProduct] AS' 
END

GO--

ALTER PROCEDURE [Catalog].[sp_RecalculateProductsCountInCategoriesByProduct] 
    @ProductId INT
AS
BEGIN
	SET NOCOUNT ON;

	Declare @CategoryIds TABLE (CategoryId int);

	-- get product categories with parent categories    
	;with parents as 
    (
        Select CategoryId, ParentCategory, CatLevel
        From Catalog.Category 
        Where CategoryId in (Select ProductCategories.CategoryId 
							 From Catalog.ProductCategories 
							 Where ProductId = @ProductId) 
        Union all
        Select c.CategoryId, c.ParentCategory, c.CatLevel 
        From Catalog.Category c
        Join parents p on c.CategoryId = p.ParentCategory and (c.CategoryId <> c.ParentCategory or c.CategoryId <> 0)
    ) 
    Insert @CategoryIds	
        Select p.CategoryId  
        From (Select distinct CategoryId From parents) as p 
        Order by p.CategoryId asc

	-- temp table
	Declare @Categories TABLE (CategoryId int, Products_Count int, Total_Products_Count int, Available_Products_Count int, Current_Products_Count int, [Level] int);

	-- set Level for categories
	;WITH cteSort AS (
	  SELECT
		[Category].CategoryID AS Child,
		[Category].ParentCategory AS Parent,
		1 AS [Level]
	  FROM [Catalog].[Category]
	  WHERE CategoryID = 0
	  UNION ALL
	  SELECT
		[Category].CategoryID AS Child,
		[Category].ParentCategory AS Parent,
		cteSort.[Level] + 1 AS [Level]
	  FROM [Catalog].[Category]    
	  INNER JOIN cteSort ON [Category].ParentCategory = cteSort.Child AND [Category].CategoryID <> 0
	)

	-- set products count in category (only products in category) and level 
	INSERT INTO @Categories
	SELECT 
		c.CategoryID,
		isnull(g.Products_Count, 0) * c.Enabled,
		isnull(g.Total_Products_Count, 0),
		isnull(g.Available_Products_Count, 0) * c.Enabled,
		isnull(g.Current_Products_Count, 0) * c.Enabled, 
		cteSort.[Level]

	FROM [Catalog].Category c
	  INNER JOIN (
		SELECT
		  pc.CategoryID,
		  SUM(1 * p.Enabled * ~p.Hidden) Products_Count,
		  COUNT(*) Total_Products_Count,
		  SUM(1 * p.Enabled * ~p.Hidden * (CASE WHEN pExt.AmountSort = 0 THEN p.AllowPreOrder ELSE pExt.AmountSort END)) Available_Products_Count,
		  SUM(1 * p.Enabled * ~p.Hidden) Current_Products_Count

		FROM [Catalog].ProductCategories pc 
		INNER JOIN @CategoryIds ids ON ids.CategoryId = pc.CategoryID 
		INNER JOIN [Catalog].Product p ON p.ProductID = pc.ProductID
		INNER JOIN [Catalog].[ProductExt] pExt ON p.ProductID = pExt.ProductID     
		GROUP BY pc.CategoryID
	  ) g ON g.CategoryID = c.CategoryID
	  LEFT JOIN cteSort ON cteSort.Child = c.CategoryID 

	-- set products count with sum count of subcategories products count
	DECLARE @level int
	SET @level = (SELECT TOP(1) [Level] FROM @Categories ORDER BY [Level] DESC) 

	-- t1 - category count, t2 - count in subcategories not in @Categories, t3 - count in subcategories in @Categories
	WHILE (@level > 1) 
	BEGIN
		UPDATE t1
		SET
		  t1.Products_Count = t1.Products_Count + t2.pc + t3.pc,
		  t1.Total_Products_Count = t1.Total_Products_Count + t2.tpc + t3.tpc,
		  t1.Available_Products_Count = t1.Available_Products_Count + t2.apc + t3.apc 

		FROM @Categories AS t1 	

		CROSS apply (
			SELECT
			  isnull(SUM(Products_Count), 0) pc,
			  isnull(SUM(Total_Products_Count), 0) tpc,
			  isnull(SUM(Available_Products_Count), 0) apc 
			FROM [Catalog].[Category]
			WHERE ParentCategory = t1.CategoryID and CategoryID not in (Select CategoryId From @Categories)
		  ) t2 

		CROSS apply (
			SELECT
			  isnull(SUM(cats.Products_Count), 0) pc,
			  isnull(SUM(cats.Total_Products_Count), 0) tpc,
			  isnull(SUM(cats.Available_Products_Count), 0) apc 
			FROM [Catalog].[Category] 
			INNER JOIN @Categories cats on cats.CategoryId = Category.CategoryID 
			WHERE ParentCategory = t1.CategoryID
		  ) t3 

		WHERE t1.[Level] = @level

		SET @level = @level -1
	END

	UPDATE c 
	SET 
		c.Products_Count = cat.Products_Count,
		c.Total_Products_Count = cat.Total_Products_Count,
		c.Available_Products_Count = cat.Available_Products_Count,
		c.Current_Products_Count = cat.Current_Products_Count,
		c.[CatLevel] = cat.[Level] 
	FROM Catalog.Category c 
	INNER JOIN @Categories cat ON cat.CategoryId = c.CategoryID
END

GO--

IF EXISTS (SELECT * FROM sys.objects WHERE [object_id] = OBJECT_ID(N'[Catalog].[EnabledChanged]') AND [type] = 'TR')
BEGIN
      DROP TRIGGER [Catalog].[EnabledChanged]
END

GO--

Update Settings.Settings Set Name = 'AutodetectCity' Where Name = 'DisplayCityInTopPanel'

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.SystemSettings.HideCityInTopPanel', 'Скрывать город в клиентской части'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.SystemSettings.HideCityInTopPanel', 'Hide city in top panel'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.SystemSettings.HideCityInTopPanelNote', 'Если данная настройка включена, то блок "Ваш город" будет скрыт в шапке полной версии'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.SystemSettings.HideCityInTopPanelNote', 'If this setting is enabled, the "Your city" block will be hidden in the header of the full version'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.Builder.HideCityInTopPanel', 'Скрывать город в клиентской части'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.Builder.HideCityInTopPanel', 'Hide city in top panel'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.Builder.HideCityInTopPanelHint', 'Если данная настройка включена, то блок "Ваш город" будет скрыт в шапке полной версии'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.Builder.HideCityInTopPanelHint', 'If this setting is enabled, the "Your city" block will be hidden in the header of the full version'

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.MyAccount.OrderProductHistory.Empty', 'Список купленных товаров пуст'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.MyAccount.OrderProductHistory.Empty', 'The list of purchased items is empty'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.MyAccount.OrderProductHistory.Loading', 'Загрузка данных...'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.MyAccount.OrderProductHistory.Loading', 'Loading...'

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.Address.Locate', 'Определить местоположение'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.Address.Locate', 'Determine location'

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'User.Registration.Error', 'Неверные данные'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'User.Registration.Error', 'Wrong data'

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.MyAccount.OrderProductHistory.BeforeOrderDate', 'от'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.MyAccount.OrderProductHistory.BeforeOrderDate', 'from'

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.Builder.ShowWarehouseFilter', 'Фильтр по складу'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.Builder.ShowWarehouseFilter', 'Warehouse filter' 

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.Builder.ShowWarehouseFilterHelp', 'Выводить или нет фильтр по складу.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.Builder.ShowWarehouseFilterHelp', 'To output or not the filter on warehouses.'

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.SalesChannels.SalesChannel.Bonus.Details.Text', 'Бонусная программа лояльности — это система, с помощью которой вы сможете дарить бонусы своим клиентам, проводить акции, информировать покупателей и стимулировать их к покупке.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.SalesChannels.SalesChannel.Bonus.Details.Text', 'The loyalty bonus program is a system through which you can give bonuses to your customers, hold promotions, inform buyers and stimulate them to buy.'

GO--

ALTER PROCEDURE [Catalog].[sp_ParseProductProperty]      
  @nameProperty nvarchar(100),      
  @propertyValue nvarchar(255),      
  @rangeValue float,    
  @productId int,      
  @sort int      
AS      
BEGIN      
 -- select or create property      
 Declare @propertyId int      
 if ((select count(PropertyID) from Catalog.[Property] where Name = @nameProperty)= 0)      
  begin      
   insert into Catalog.[Property] (Name,UseInFilter,UseInBrief,Useindetails,SortOrder,[type], NameDisplayed) values (@nameProperty,1,0,1,0,1, @nameProperty)      
   set @propertyId = (Select SCOPE_IDENTITY())      
  end      
 else      
  set @propertyId = (select top(1) PropertyID from Catalog.[Property] where Name = @nameProperty)      
      
  -- select or create value      
  Declare @propertyValueId int      
      
  Declare @useinfilter bit      
  set @useinfilter = (Select Top 1 UseInFilter from Catalog.[Property] Where PropertyID=@propertyId)      
  Declare @useindetails bit      
  set @useindetails = (Select Top 1 UseInDetails from Catalog.[Property] Where PropertyID=@propertyId)      
      
  if ((select count(PropertyValueID) from Catalog.[PropertyValue] where Value = @propertyValue and PropertyId=@propertyId)= 0)      
   begin      
    insert into Catalog.[PropertyValue] (PropertyId, Value, UseInFilter, UseInDetails, SortOrder, RangeValue) values (@propertyId, @propertyValue, @useinfilter, @useindetails, 0, @rangeValue)      
    set @propertyValueId = (Select SCOPE_IDENTITY())      
   end      
  else      
   set @propertyValueId = (select top(1) PropertyValueID from Catalog.[PropertyValue] where Value = @propertyValue and PropertyId=@propertyId)      
       
 --create link between product and property value      
 if ((select Count(*) from Catalog.ProductPropertyValue where ProductID=@productId and PropertyValueID=@propertyValueId)=0)      
  insert into Catalog.ProductPropertyValue (ProductID,PropertyValueID) values (@productId,@propertyValueId)       
END 

GO--

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Shipping].[FivePostPickPoints]') AND type in (N'U'))
	ALTER TABLE [Shipping].[FivePostPickPoints]
	ALTER COLUMN [Id] nvarchar(100) NOT NULL

GO--

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[Shipping].[FivePostPickPoints]') AND name = N'PK_FivePostPickPoints')
	ALTER TABLE [Shipping].[FivePostPickPoints] ADD CONSTRAINT
		[PK_FivePostPickPoints] PRIMARY KEY CLUSTERED 
		(
		[Id]
		) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO--

UPDATE [Settings].[InternalSettings] SET [settingValue] = '13.0.1' WHERE [settingKey] = 'db_version'
