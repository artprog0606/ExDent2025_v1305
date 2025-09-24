EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.SizesViewer.ControlType.Unavailable', 'недоступно'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.SizesViewer.ControlType.Unavailable', 'unavailable'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.SizesViewer.ControlType.NoOptionsAvailable', 'Нет доступных опций'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.SizesViewer.ControlType.NoOptionsAvailable', 'No options available'

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Catalog.SizesColorsDisplayMode', 'Режим отображания'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Catalog.SizesColorsDisplayMode', 'Display mode'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Sizes.ControlType.Enumeration', 'Перечисление'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Sizes.ControlType.Enumeration', 'Enumeration'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Sizes.ControlType.Select', 'Выпадающий список'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Sizes.ControlType.Select', 'Select list'

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.Select.LoadingText', 'Загрузка данных ...'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.Select.LoadingText', 'Loading ...'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.Select.NoResultsText', 'Ничего не найдено'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.Select.NoResultsText', 'No results found'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.Select.NoChoicesText', 'Нет выбора, из которого можно было бы выбирать'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.Select.NoChoicesText', 'No choices to choose from'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.Select.ItemSelectText', ''
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.Select.ItemSelectText', ''

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.Select.UniqueItemText', 'Только уникальные значения могут быть добавлены'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.Select.UniqueItemText', 'Only unique values can be added'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.Select.СustomAddItemText', 'Только значения, которые удовлетворяют условию могут быть добавлены'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.Select.СustomAddItemText', 'Only values matching specific conditions can be added'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.Select.AddItemText', 'Нажмите на Enter, чтобы добавить <b>"{{value}}"</b>'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.Select.AddItemText', 'Press Enter to add <b>"{{value}}"</b>'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.Select.MaxItemText', 'Максимально можно добавить {{maxItemCount}}'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.Select.MaxItemText', 'Only {{maxItemCount}} values can be added'

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.ColorsViewer.ControlType.Unavailable', 'недоступно'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.ColorsViewer.ControlType.Unavailable', 'unavailable'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.ColorsViewer.ControlType.NoOptionsAvailable', 'Нет доступных опций'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.ColorsViewer.ControlType.NoOptionsAvailable', 'No options available'

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Catalog.ColorsViewMode', 'Предствление цвета'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Catalog.ColorsViewMode', 'Color representation'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Catalog.Sizes', 'Размеры'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Catalog.Sizes', 'Sizes'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Catalog.Colors', 'Цвета'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Catalog.Colors', 'Colors'

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.SettingProvider.FunctionIsNotAvailable', 'Функция не доступна на вашем тарифе'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.SettingProvider.FunctionIsNotAvailable', 'The function is not available on your tariff'

GO--

CREATE NONCLUSTERED INDEX Offer_Price_Inc_ProductId_Amount On [Catalog].[Offer] (Price) Include (ProductID, Amount)

GO--

ALTER PROCEDURE [Catalog].[PreCalcProductParams]
    @productId INT,
    @ModerateReviews BIT,
    @OnlyAvailable BIT,
    @ComplexFilter BIT
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @CountPhoto INT;
    DECLARE @Type NVARCHAR(10);
    DECLARE @PhotoId INT;
    DECLARE @MaxAvailable FLOAT;
    DECLARE @VideosAvailable BIT;
    DECLARE @Colors NVARCHAR(MAX);
    DECLARE @NotSamePrices BIT;
    DECLARE @MinPrice FLOAT;
    DECLARE @PriceTemp FLOAT;
    DECLARE @AmountSort BIT;
    DECLARE @OfferId INT;
    DECLARE @Comments INT;
    DECLARE @CategoryId INT;
    DECLARE @Gifts BIT;
    IF NOT EXISTS
        (
            SELECT ProductID
            FROM [Catalog].Product
            WHERE ProductID = @productId
        )
        RETURN;
    SET @Type = 'Product';
    --@CountPhoto        
    SET @CountPhoto =
            (
                SELECT TOP (1) CASE
                                   WHEN
                                           (
                                               SELECT Offer.ColorID
                                               FROM [Catalog].[Offer]
                                               WHERE [ProductID] = @productId
                                                 AND main = 1
                                           ) IS NOT NULL AND @ComplexFilter = 1
                                       THEN
                                       (
                                           SELECT COUNT(DISTINCT PhotoId)
                                           FROM [Catalog].[Photo]
                                                    INNER JOIN [Catalog].[Offer] ON [Photo].ColorID = Offer.ColorID OR [Photo].ColorID is NULL
                                           WHERE [Photo].[ObjId] = Offer.[ProductId]
                                             AND [Offer].Main = 1
                                             AND TYPE = @Type
                                             AND Offer.[ProductId] = @productId
                                       )
                                   ELSE
                                       (
                                           SELECT COUNT(PhotoId)
                                           FROM [Catalog].[Photo]
                                           WHERE [Photo].[ObjId] = @productId
                                             AND TYPE = @Type
                                       )
                                   END
            );
    --@PhotoId        
    SET @PhotoId =
            (
                SELECT CASE
                           WHEN
                               (
                                   SELECT Offer.ColorID
                                   FROM [Catalog].[Offer]
                                   WHERE [ProductID] = @productId
                                     AND main = 1
                               ) IS NOT NULL
                               THEN
                               (
                                   SELECT TOP (1) PhotoId
                                   FROM [Catalog].[Photo]
                                            INNER JOIN [Catalog].[Offer] ON Offer.[ProductId] = @productId AND ([Photo].ColorID = Offer.ColorID OR [Photo].ColorID is NULL)
                                   WHERE([Photo].ColorID = Offer.ColorID
                                       OR [Photo].ColorID IS NULL)
                                     AND [Photo].[ObjId] = @productId
                                     AND Type = @Type
                                   ORDER BY [Photo]. main DESC,
                                            [Photo].[PhotoSortOrder],
                                            [PhotoId]
                               )
                           ELSE
                               (
                                   SELECT TOP (1) PhotoId
                                   FROM [Catalog].[Photo]
                                   WHERE [Photo].[ObjId] = @productId
                                     AND Type = @Type
                                   ORDER BY main DESC,
                                            [Photo].[PhotoSortOrder],
                                            [PhotoId]
                               )
                           END



            );

    --VideosAvailable        
    IF (SELECT COUNT(ProductVideoID) FROM [Catalog].[ProductVideo] WHERE ProductID = @productId) > 0
        BEGIN
            SET @VideosAvailable = 1;
        END;
    ELSE
        BEGIN
            SET @VideosAvailable = 0;
        END;

    --@MaxAvailable        
    SET @MaxAvailable = (SELECT MAX(Offer.Amount) FROM [Catalog].Offer WHERE ProductId = @productId);

    --AmountSort        
    SET @AmountSort =
            (
                SELECT CASE
                           WHEN @MaxAvailable <= 0
                               OR @MaxAvailable < ISNULL(Product.MinAmount, 0)
                               THEN 0
                           ELSE 1
                           END
                FROM [Catalog].Offer
                         INNER JOIN [Catalog].Product ON Product.ProductId = Offer.ProductId
                WHERE Offer.ProductId = @productId
                  AND main = 1
            );
    --Colors        
    SET @Colors =
            (
                SELECT [Settings].[ProductColorsToString](@productId, @OnlyAvailable)
            );

    --@NotSamePrices        
    IF
            (
                SELECT MAX(price) - MIN(price)
                FROM [Catalog].offer
                         INNER JOIN [Catalog].Product ON Product.ProductId = Offer.ProductId
                WHERE offer.productid = @productId AND
                        price > 0 AND
                    (@OnlyAvailable = 0 OR amount > 0 OR AllowPreOrder = 1)
            ) > 0
        BEGIN
            SET @NotSamePrices = 1;
        END;
    ELSE
        BEGIN
            SET @NotSamePrices = 0;
        END;

    --@MinPrice        
    SET @MinPrice =
            (
                SELECT isNull(MIN(price), 0)
                FROM [Catalog].offer
                         INNER JOIN [Catalog].Product ON Product.ProductId = Offer.ProductId
                WHERE offer.productid = @productId AND
                        price > 0 AND
                    (@OnlyAvailable = 0 OR amount > 0 OR AllowPreOrder = 1)
            );

    --@OfferId      
    SET @OfferId =
            (
                SELECT OfferID
                FROM [Catalog].offer
                WHERE offer.productid = @productId AND (offer.Main = 1 OR offer.Main IS NULL)
            );


    --@PriceTemp        
    SET @PriceTemp =
            (
                SELECT CASE WHEN [Product].Discount > 0 THEN (@MinPrice - @MinPrice * [Product].Discount / 100) * CurrencyValue ELSE (@MinPrice - isnull([Product].DiscountAmount, 0)) * CurrencyValue END
                FROM Catalog.Product
                         INNER JOIN Catalog.Currency ON Product.Currencyid = Currency.Currencyid
                WHERE Product.Productid = @productId
            );

    --@Comments      
    SET @Comments =
            (
                SELECT COUNT(ReviewId)
                FROM CMS.Review
                WHERE EntityId = @productId AND (Checked = 1 OR @ModerateReviews = 0)
            );

    --@Gifts      
    SET @Gifts =
            (
                SELECT TOP (1) CASE
                                   WHEN COUNT([ProductGifts].ProductID) > 0
                                       THEN 1
                                   ELSE 0
                                   END
                FROM [Catalog].[ProductGifts]
                         INNER JOIN Catalog.Offer on ProductGifts.GiftOfferId = Offer.OfferId
                         INNER JOIN Catalog.Product on Offer.ProductId = Product.ProductId
                WHERE [ProductGifts].ProductID = @productId  and Offer.Amount > ISNULL(Product.MinAmount, 0) and Enabled = 1
            );

    --@CategoryId      
    -- DECLARE @MainCategoryId INT;  
    -- SET @MainCategoryId =  
    -- (  
    -- SELECT TOP 1 CategoryID  
    -- FROM [Catalog].ProductCategories  
    -- WHERE ProductID = @productId  
    -- ORDER BY Main DESC  
    -- );  
    -- IF @MainCategoryId IS NOT NULL  
    -- BEGIN  
    -- SET @CategoryId =  
    -- (  
    -- SELECT TOP 1 id  
    -- FROM [Settings].[GetParentsCategoryByChild](@MainCategoryId)  
    -- ORDER BY sort DESC  
    -- );  
    -- END;  
    IF
            (
                SELECT COUNT(productid)
                FROM [Catalog].ProductExt
                WHERE productid = @productId
            ) > 0
        BEGIN
            UPDATE [Catalog].[ProductExt]
            SET
                [CountPhoto] = @CountPhoto,
                [PhotoId] = @PhotoId,
                [VideosAvailable] = @VideosAvailable,
                [MaxAvailable] = @MaxAvailable,
                [NotSamePrices] = @NotSamePrices,
                [MinPrice] = @MinPrice,
                [Colors] = @Colors,
                [AmountSort] = @AmountSort,
                [OfferId] = @OfferId,
                [Comments] = @Comments,
                --[CategoryId] = @CategoryId,  
                [PriceTemp] = @PriceTemp,
                [Gifts] = @Gifts
            WHERE [ProductId] = @productId;
        END;
    ELSE
        BEGIN
            INSERT INTO [Catalog].[ProductExt]
            ([ProductId],
             [CountPhoto],
             [PhotoId],
             [VideosAvailable],
             [MaxAvailable],
             [NotSamePrices],
             [MinPrice],
             [Colors],
             [AmountSort],
             [OfferId],
             [Comments],
                --[CategoryId],  
             [PriceTemp],
             [Gifts]
            )
            VALUES
                (@productId,
                 @CountPhoto,
                 @PhotoId,
                 @VideosAvailable,
                 @MaxAvailable,
                 @NotSamePrices,
                 @MinPrice,
                 @Colors,
                 @AmountSort,
                 @OfferId,
                 @Comments,
                    --@CategoryId,  
                 @PriceTemp,
                 @Gifts
                );
        END;
END;

GO-- 


ALTER PROCEDURE [Catalog].[PreCalcProductParamsMass] @ModerateReviews BIT, @OnlyAvailable bit,  @ComplexFilter BIT AS
BEGIN

    INSERT INTO
        [Catalog].[ProductExt] (ProductId, CountPhoto, PhotoId, VideosAvailable, MaxAvailable, NotSamePrices, MinPrice, Colors, AmountSort, OfferId, Comments
        --,CategoryId
    ) (
        SELECT
            ProductId, 0, NULL, 0, 0, 0, 0, NULL, 0, NULL, 0--, NULL 
        FROM
            [Catalog].Product
        WHERE
                Product.ProductId NOT IN
                (
                    SELECT
                        ProductId
                    FROM
                        [Catalog].[ProductExt]
                )
    )
    UPDATE
        catalog.ProductExt
    SET
        ProductExt.[CountPhoto] = tempTable.CountPhoto,
        ProductExt.[PhotoId] = tempTable.[PhotoId],
        ProductExt.[VideosAvailable] = tempTable.[VideosAvailable],
        ProductExt.[MaxAvailable] = tempTable.[MaxAvailable],
        ProductExt.[NotSamePrices] = tempTable.[NotSamePrices],
        ProductExt.[MinPrice] = tempTable.[MinPrice],
        ProductExt.[OfferId] = tempTable.[OfferId],
        ProductExt.[Comments] = tempTable.[Comments],
        ProductExt.[Gifts] = tempTable.[Gifts],
        ProductExt.[Colors] = tempTable.[Colors],
        --ProductExt.[CategoryId] = tempTable.[CategoryId] ,
        ProductExt.PriceTemp = tempTable.PriceTemp,
        ProductExt.AmountSort=tempTable.AmountSort
    FROM
        catalog.ProductExt
            INNER JOIN
        (
            select
                pe.ProductId,
                CountPhoto=case when offerId.ColorID is null OR @ComplexFilter = 0 then countNocolor.countNocolor else countColor.countColor end,
                PhotoId=case when offerId.ColorID is null then PhotoIdNoColor.PhotoIdNoColor else PhotoIdColor.PhotoIdColor end,
                [VideosAvailable]=isnull(videosAvailable.videosAvailable,0),
                [MaxAvailable]=maxAvailable.maxAvailable,
                [NotSamePrices]=isnull(notSamePrices.notSamePrices,0),
                [MinPrice]=isnull(minPrice.minPrice,0),
                [OfferId]=offerId.OfferId,
                [Comments]=isnull(comments.comments,0),
                [Gifts]=isnull(gifts.gifts,0),
                [Colors]=(SELECT [Settings].[ProductColorsToString](pe.ProductId, @OnlyAvailable)),
                --[CategoryId] = (SELECT TOP 1 id	FROM [Settings].[GetParentsCategoryByChild](( SELECT TOP 1 CategoryID FROM [Catalog].ProductCategories	WHERE ProductID = pe.ProductId ORDER BY Main DESC))ORDER BY sort DESC),
                PriceTemp = CASE WHEN p.Discount > 0 THEN (isnull(minPrice.minPrice,0) - isnull(minPrice.minPrice,0) * p.Discount/100)*c.CurrencyValue ELSE (isnull(minPrice.minPrice,0) - isnull(p.DiscountAmount,0))*c.CurrencyValue END,
                AmountSort=CASE when ISNULL(maxAvailable.maxAvailable, 0) <= 0 OR maxAvailable.maxAvailable < IsNull(p.MinAmount, 0) THEN 0 ELSE 1 end

            from Catalog.[ProductExt] pe
                     left join (SELECT o.ProductId, COUNT(*) countColor FROM [Catalog].[Photo] ph INNER JOIN [Catalog].[Offer] o  ON ph.[ObjId] = o.ProductId
                                WHERE ( ph.ColorID = o.ColorID OR ph.ColorID IS NULL ) AND TYPE = 'Product' AND o.Main = 1
                                group by o.ProductId
            ) countColor on pe.ProductId=countColor.ProductId

                     left join (SELECT [ObjId], COUNT(*) countNocolor FROM [Catalog].[Photo]
                                WHERE TYPE = 'Product'
                                group by [ObjId]
            ) countNocolor on pe.ProductId=countNocolor.[ObjId]

                     left join (
                select ProductId, PhotoId PhotoIdColor from (
                                                                SELECT o.ProductId, ph.PhotoId, Row_Number() over (PARTITION  by o.ProductId ORDER BY ph.main DESC ,ph.[PhotoSortOrder], ph.[PhotoId]) rn FROM [Catalog].[Photo] ph
                                                                                                                                                                                                                   INNER JOIN [Catalog].[Offer] o ON ph.[ObjId] = o.ProductId
                                                                WHERE (ph.ColorID = o.ColorID OR ph.ColorID IS NULL) AND TYPE = 'Product' ) ct where rn=1
            ) PhotoIdColor on pe.ProductId=PhotoIdColor.ProductId

                     left join (
                select ProductId, PhotoId PhotoIdNoColor from (
                                                                  SELECT ph.[ObjId] ProductId, ph.PhotoId, Row_Number() over (PARTITION  by ph.[ObjId] ORDER BY ph.main DESC ,ph.[PhotoSortOrder], ph.[PhotoId]) rn FROM [Catalog].[Photo] ph	WHERE TYPE = 'Product' ) ct where rn=1
            ) PhotoIdNoColor on pe.ProductId=PhotoIdNoColor.ProductId

                     left join (select pv.ProductID, CASE WHEN COUNT(pv.ProductVideoID) > 0 THEN 1	ELSE 0 END videosAvailable FROM [Catalog].[ProductVideo] pv group by pv.ProductID) videosAvailable on pe.ProductId=videosAvailable.ProductId
                     left join (select o.ProductID,Max(o.Amount) maxAvailable  FROM [Catalog].Offer o group by o.ProductID) maxAvailable on pe.ProductId=maxAvailable.ProductId
                     left join (select o.ProductID, CASE WHEN MAX(o.price) - MIN(o.price) > 0 THEN 1 ELSE 0 END notSamePrices  FROM [Catalog].Offer o Inner Join [Catalog].Product On Product.ProductId = o.ProductID where o.price > 0 AND (@OnlyAvailable = 0 OR o.amount > 0 OR AllowPreOrder = 1) group by o.ProductID) notSamePrices on pe.ProductId=notSamePrices.ProductId
                     left join (select o.ProductID,MIN(o.price) minPrice FROM [Catalog].Offer o Inner Join [Catalog].Product On Product.ProductId = o.ProductID where o.price > 0 AND (@OnlyAvailable = 0 OR o.amount > 0 OR AllowPreOrder = 1)  group by o.ProductID) minPrice on pe.ProductId=minPrice.ProductId
                     left join (
                select ProductId, OfferID, colorId from (
                                                            select o.ProductID,o.OfferID, o.colorId, Row_Number() over (PARTITION  by o.OfferID ORDER BY o.OfferID) rn  FROM [Catalog].Offer o where o.Main = 1 )ct where rn=1
            ) offerId on pe.ProductId=offerId.ProductId
                     left join (select EntityId ProductID,count(ReviewId) comments  FROM CMS.Review  where (Checked = 1 OR @ModerateReviews = 0) group by EntityId) comments on pe.ProductId=comments.ProductId
                     left join (select pg.ProductID, CASE WHEN COUNT(pg.ProductID) > 0 THEN 1 ELSE 0 END gifts FROM [Catalog].[ProductGifts] pg INNER JOIN Catalog.Offer on pg.GiftOfferId = Offer.OfferId INNER JOIN Catalog.Product on Offer.ProductId = Product.ProductId WHERE Offer.Amount > ISNULL(Product.MinAmount, 0) and Enabled = 1 group by pg.ProductID) gifts on pe.ProductId=gifts.ProductId
                     inner join catalog.Product p on p.ProductID = pe.ProductID
                     INNER JOIN CATALOG.Currency c ON p.currencyid = c.currencyid
        )
            AS tempTable
        ON tempTable.ProductId = ProductExt.ProductId

END

GO--
CREATE TABLE [Customers].[SmsLog](
                                     [Id] [int] IDENTITY(1,1) NOT NULL,
                                     [Phone] [bigint] NOT NULL,
                                     [SmsText] [nvarchar](max) NOT NULL,
                                     [CreatedOn] [datetime] NOT NULL,
                                     [Ip] [nvarchar](100) NOT NULL,
                                     [CustomerId] [uniqueidentifier] NULL,
                                     [Status] [smallint] NOT NULL,
                                     CONSTRAINT [PK_SmsLog_1] PRIMARY KEY CLUSTERED
                                         (
                                          [Id] ASC
                                             )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO--

CREATE TABLE [Customers].[SmsBan](
                                     [Phone] [bigint] NULL,
                                     [Ip] [nvarchar](100) NULL,
                                     [UntilDate] [datetime] NOT NULL
) ON [PRIMARY]

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.Spinbox.More', 'Увеличить'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.Spinbox.More', 'More'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.Spinbox.Less', 'Уменьшить'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.Spinbox.Less', 'Less'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.Spinbox.Quantity', 'Количество'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.Spinbox.Quantity', 'Quantity'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.Filter.Value', 'Значение фильтра'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.Filter.Value', 'Filter value'

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'PreOrder.Success.Title', 'Спасибо, ваша заявка отправлена!'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'PreOrder.Success.Title', 'Thank you, your application has been sent!'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.PreOrder.FirstName', 'Имя'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.PreOrder.FirstName', 'First name'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.PreOrder.LastName', 'Фамилия'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.PreOrder.LastName', 'Last name'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.PreOrder.Email', 'E-mail'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.PreOrder.Email', 'E-mail'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.PreOrder.Phone', 'Номер телефона'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.PreOrder.Phone', 'Phone'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.PreOrder.Comment', 'Комментарий'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.PreOrder.Comment', 'Comment'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.PreOrder.CaptchaCode', 'Код'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.PreOrder.CaptchaCode', 'Code'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.PreOrder.Send', 'Отправить'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.PreOrder.Send', 'Send'

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

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.SmsConfirmation.Code', 'Код подтверждения:'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.SmsConfirmation.Code', 'Confirmation code:'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.SmsConfirmation.SendCode', 'Получить код'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.SmsConfirmation.SendCode', 'Send code'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.SmsConfirmation.PhoneConfirmed', 'Телефон подтвержден'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.SmsConfirmation.PhoneConfirmed', 'Phone confirmed'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.SmsConfirmation.PhoneNotConfirmed', 'Телефон не подтвержден'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.SmsConfirmation.PhoneNotConfirmed', 'Phone not confirmed'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.SmsConfirmation.Confirm', 'Подтвердить'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.SmsConfirmation.Confirm', 'Confirm'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.ConfirmSms.ErrorEmptyPhone', 'Укажите корректный номер телефона'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.ConfirmSms.ErrorEmptyPhone', 'Укажите корректный номер телефона'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.ConfirmSms.ErrorPhoneChanged', 'Телефон изменился, нужно подтвердить телефон '
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.ConfirmSms.ErrorPhoneChanged', 'The phone has been changed, you need to confirm the phone number '

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.ConfirmSms.RetryPhoneCountdownText', 'Отправить код повторно можно через {{sec}} сек.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.ConfirmSms.RetryPhoneCountdownText', 'You can resend the code after {{sec}} sec.'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.Captcha.Code', 'Код'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.Captcha.Code', 'Code'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'User.Registration.ErrorPhoneNotConfirmed', 'Телефон не подтвержден'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'User.Registration.ErrorPhoneNotConfirmed', 'Phone number is not confirmed'

GO--

ALTER TABLE [Settings].[Settings]
    ALTER COLUMN Name nvarchar(100) NOT NULL

GO--

ALTER PROCEDURE [Settings].[sp_GetSettingValue]
@SettingName nvarchar(100)
AS
BEGIN
    SELECT [Value]
    FROM   [Settings].[Settings]
    WHERE ([Name] = @SettingName)
END

GO--

ALTER PROCEDURE [Settings].[sp_UpdateSettings]
    @Value nvarchar(Max),
    @Name nvarchar(100)
AS
BEGIN
    IF (SELECT COUNT(*) FROM [Settings].[Settings] WHERE [Name] = @Name) = 0
        BEGIN
            INSERT INTO [Settings].[Settings] ([Name], [Value]) VALUES (@Name, @Value)
        END
    ELSE
        UPDATE [Settings].[Settings] SET [Value] = @Value WHERE [Name] = @Name
END

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Checkout.AllowAddOrderReview', 'Разрешить оставлять оценки заказам'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Checkout.AllowAddOrderReview', 'Allow to leave ratings for orders'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Checkout.AllowAddOrderReviewHint', 'После оформления заказа у покупателя будет возможность поставить оценку и оставить отзыв к заказу'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Checkout.AllowAddOrderReviewHint', 'After placing an order, the buyer will have the opportunity to rate and leave feedback on the order'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Checkout.ReviewOnlyPaidOrder', 'Оценивать только оплаченные заказы'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Checkout.ReviewOnlyPaidOrder', 'Evaluate only paid orders'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Checkout.ReviewOnlyPaidOrderHint', 'Заказ нельзя будет оценить, пока он не будет оплачен'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Checkout.ReviewOnlyPaidOrderHint', 'The order cannot be evaluated until it is paid for'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Checkout.OrderReviewSettings', 'Оценка заказа'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Checkout.OrderReviewSettings', 'Order review'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.OrderReviews.Index.Title', 'Оценки заказов'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.OrderReviews.Index.Title', 'Order reviews'

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Bonuses.SelectBonusAmount', 'Выберите, сколько хотели бы списать:'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Bonuses.SelectBonusAmount', 'Choose how much you would like to deduct'

GO--

ALTER TABLE [Order].PaymentDetails ADD
    Kpp nvarchar(255) NULL

GO--

ALTER PROCEDURE [Order].[sp_AddPaymentDetails]
    @OrderID int,
    @CompanyName nvarchar(255),
    @INN nvarchar(255),
    @Phone nvarchar(20),
    @Contract nvarchar(255),
    @Change nvarchar(255),
    @IsCashOnDeliveryPayment bit,
    @IsPickPointPayment bit,
    @Kpp nvarchar(255)
AS
BEGIN
    INSERT INTO [Order].[PaymentDetails]
    ([OrderID]
    ,[CompanyName]
    ,[INN]
    ,[Phone]
    ,[Contract]
    ,[Change]
    ,[IsCashOnDeliveryPayment]
    ,[IsPickPointPayment]
    ,[Kpp])
    VALUES
        (@OrderID
        ,@CompanyName
        ,@INN
        ,@Phone
        ,@Contract
        ,@Change
        ,@IsCashOnDeliveryPayment
        ,@IsPickPointPayment
        ,@Kpp)
    RETURN SCOPE_IDENTITY()
END

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Payment.PaymentDetails.Kpp', 'КПП'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Payment.PaymentDetails.Kpp', 'Kpp'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.Payment.Bill.Kpp', 'КПП'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.Payment.Bill.Kpp', 'Kpp'

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.CopyTaskGroup.AddCopy', 'Создать копию'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.CopyTaskGroup.AddCopy', 'AddCopy'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.CopyTaskGroup.Name', 'Название'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.CopyTaskGroup.Name', 'Name'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.CopyTaskGroup.CopyTasks', 'Копировать задачи'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.CopyTaskGroup.CopyTasks', 'Copy tasks'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Taskgroups.AddEdit.Copy', 'Копировать'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Taskgroups.AddEdit.Copy', 'Copy'

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.OrderReviews.Index.Header', 'Оценки заказов'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.OrderReviews.Index.Header', 'Order reviews'

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Orders.GroupOfCustomers', 'Группа покупателей'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Orders.GroupOfCustomers', 'Group of customers'

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Payments.Bill.GetCustomerDataMethod.InPayment', 'Запрашивать название организации, ИНН и КПП в методе оплаты'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Payments.Bill.GetCustomerDataMethod.InPayment', 'Request the name of the organization, TIN, KPP in the payment method'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Orders.OrderItem.Kpp', 'КПП:'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Orders.OrderItem.Kpp', 'KPP:'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.PaymentMethods.Bill.CustomerKpp', 'Дополнительное поле покупателя, содержащее КПП'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.PaymentMethods.Bill.CustomerKpp', 'Additional buyer''s field containing the KPP'

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Payment.PaymentMethod.SuccessUrlLabel', N'Успешная оплата (Success url)'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Payment.PaymentMethod.SuccessUrlLabel', 'Success url'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Payment.PaymentMethod.CancelUrlLabel', N'Отмена оплаты (Cancel url)'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Payment.PaymentMethod.CancelUrlLabel', 'Cancel url'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Payment.PaymentMethod.FailUrlLabel', N'Неудачная оплата (Fail url)'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Payment.PaymentMethod.FailUrlLabel', 'Fail url'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Payment.PaymentMethod.NotificationUrlLabel', N'Уведомление об оплате (Notification URL)'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Payment.PaymentMethod.NotificationUrlLabel', 'Processing URL for payment notifications'

GO--

IF EXISTS(SELECT * FROM [Settings].[Localization] WHERE ResourceKey = 'Admin.PaymentMethods.CommonUrls.NotificationUrl')
    DELETE FROM [Settings].[Localization] WHERE ResourceKey = 'Admin.PaymentMethods.CommonUrls.NotificationUrl'

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.PaymentMethods.Robokassa.SellerLogin', 'Идентификатор магазина'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.PaymentMethods.Robokassa.SellerLogin', 'Store ID'

GO--

Update [Order].[ShippingMethod]
Set [TypeOfDelivery] = 2
Where [ShippingType] in ('PointDelivery', 'SelfDelivery')

GO--

Update [Order].[ShippingMethod]
Set [TypeOfDelivery] = 1
Where [ShippingType] in ('DeliveryByZones')

GO--

Create NONCLUSTERED INDEX IX_CreatedOn ON Customers.SmsLog (CreatedOn) INCLUDE (Id, Phone, Ip, Status)

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.OrderItemsSummary.Kpp', 'КПП'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.OrderItemsSummary.Kpp', 'KPP'

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Template.UseAdaptiveRootCategoryTitle', N'Скрывать в меню одиночную корневую категорию'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Template.UseAdaptiveRootCategoryTitle', 'Hide a single root category in the menu'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.Builder.UseAdaptiveRootCategoryTitle', N'Скрывать в меню одиночную корневую категорию'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.Builder.UseAdaptiveRootCategoryTitle', 'Hide a single root category in the menu'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Template.UseAdaptiveRootCategoryHint', N'Если настройка <b>включена</b>, то в качестве корневой категории будет использоваться первая по дереву категория, у которой больше одной подкатегории (по умолчанию Каталог).<br/><br/>Настройку рекомендуется <b>выключить</b>, если необходимо выводить в меню единственную категорию первого уровня (с родителем Каталог).'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Template.UseAdaptiveRootCategoryHint', 'If the setting is <b>enabled</b>, then the first category in the tree that has more than one subcategory (by default Catalog) will be used as the root category.<br/><br/>It is recommended to <b>turn off</b> the setting. If you need to display a single first-level category in the menu (with a parent Catalog).'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.Builder.UseAdaptiveRootCategoryHint', N'Если настройка <b>включена</b>, то в качестве корневой категории будет использоваться первая по дереву категория, у которой больше одной подкатегории (по умолчанию Каталог).<br/><br/>Настройку рекомендуется <b>выключить</b>, если необходимо выводить в меню единственную категорию первого уровня (с родителем Каталог).'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.Builder.UseAdaptiveRootCategoryHint', 'If the setting is <b>enabled</b>, then the first category in the tree that has more than one subcategory (by default Catalog) will be used as the root category.<br/><br/>It is recommended to <b>turn off</b> the setting. If you need to display a single first-level category in the menu (with a parent Catalog).'

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.SettingsApiAuth.MobileAppCategoryViewMode.Tile', N'Плитка'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.SettingsApiAuth.MobileAppCategoryViewMode.Tile', 'Tile'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.SettingsApiAuth.MobileAppCategoryViewMode.ListWithPhoto', N'Список с фотографией'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.SettingsApiAuth.MobileAppCategoryViewMode.ListWithPhoto', 'List with photo'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.SettingsApiAuth.MobileAppCategoryViewMode.ListWithoutPhoto', N'Список без фотографий'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.SettingsApiAuth.MobileAppCategoryViewMode.ListWithoutPhoto', 'List without photo'

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Catalog.Export.Products.CSV2', N'Инструкция. Экспорт товаров через файл CSV в новом формате (2.0)'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Catalog.Export.Products.CSV2', 'Instructions. Export products via CSV file in new format (2.0)'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Catalog.Export.Products.YandexMarket', N'Инструкция. Настройки выгрузки для Яндекс.Маркета'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Catalog.Export.Products.YandexMarket', 'Instructions. Upload settings for Yandex.Market'

GO--

IF NOT EXISTS (SELECT 1 FROM [Settings].[MailFormatType] Where [MailType] = 'OnNewOrderReview')
    BEGIN
        INSERT INTO [Settings].[MailFormatType] ([TypeName],[SortOrder],[Comment],[MailType])
        VALUES ('При добавлении отзыва о заказе', 440, 'Уведомление о добавлении отзыва о заказе (#ORDER_NUMBER#,#RATIO#,#COMMENT#,#ORDER_LINK#)', 'OnNewOrderReview')
    END

GO--

IF EXISTS (SELECT 1 FROM [Settings].[MailFormatType] Where [MailType] = 'OnNewOrderReview') and NOT EXISTS (SELECT 1 FROM [Settings].[MailFormat] Where MailFormatTypeId = (Select top(1) [MailFormatTypeId] FROM [Settings].[MailFormatType] Where [MailType] = 'OnNewOrderReview'))
    BEGIN
        INSERT INTO [Settings].[MailFormat] ([FormatName],[FormatText],[SortOrder],[Enable],[AddDate],[ModifyDate],[FormatSubject],[MailFormatTypeId])
        VALUES ('При добавлении отзыва о заказе','<p>Покупатель оставил отзыв к заказу <a href="#ORDER_LINK#">№#ORDER_NUMBER#</a></p>
    <p>Оценка: #RATIO#</p>
    <p>Комментарий: #COMMENT#</p>',1630,1,GETDATE(),GETDATE(),'Добавлен отзыв к заказу №#ORDER_NUMBER#',
                (Select top(1) [MailFormatTypeId] FROM [Settings].[MailFormatType] Where [MailType] = 'OnNewOrderReview'))
    END

GO--

declare @Russia int = (SELECT TOP 1 [CountryID] FROM [Customers].[Country] WHERE [CountryISO3] = 'RUS')

IF (@Russia IS NOT NULL)
    BEGIN
        declare @MORegionId int = (SELECT TOP 1 [RegionID] FROM [Customers].[Region] WHERE [RegionName] = 'Московская область' AND [CountryID] = @Russia)
        declare @ZnamyaId int = (SELECT TOP 1 [CityID] FROM [Customers].[City] WHERE [CityName] = 'Знамя Октября' AND [RegionID] = @MORegionId)
        declare @MoscowRegionId int = (SELECT TOP 1 [RegionID] FROM [Customers].[Region] WHERE [RegionName] = 'Москва' AND [CountryID] = @Russia)

        IF (@ZnamyaId IS NOT NULL AND @MoscowRegionId IS NOT NULL)
            BEGIN
                UPDATE [Customers].[City]
                SET [RegionID] = @MoscowRegionId
                WHERE [CityID] = @ZnamyaId
            END
    END

GO--

DELETE FROM [Settings].[Localization] WHERE ResourceKey = 'Core.IPTelephony.EOperatorType.Yandex'
DELETE FROM [Settings].[Localization] WHERE ResourceKey = 'Admin.Settings.Telephony.CallBack.YandexCallbackUserKey'
DELETE FROM [Settings].[Localization] WHERE ResourceKey = 'Admin.Settings.Telephony.CallBack.YandexCallbackUserKeyNote'
DELETE FROM [Settings].[Localization] WHERE ResourceKey = 'Admin.Settings.Telephony.CallBack.YandexCallbackBusinessNumber'
DELETE FROM [Settings].[Localization] WHERE ResourceKey = 'Admin.Settings.Telephony.CallBack.YandexCallbackBusinessNumberNote'
DELETE FROM [Settings].[Localization] WHERE ResourceKey = 'Admin.Settings.Telephony.Yandex.ApiKey'
DELETE FROM [Settings].[Localization] WHERE ResourceKey = 'Admin.Settings.Telephony.Yandex.ApiKeyNote'
DELETE FROM [Settings].[Localization] WHERE ResourceKey = 'Admin.Settings.Telephony.Yandex.MainUserKey'
DELETE FROM [Settings].[Localization] WHERE ResourceKey = 'Admin.Settings.Telephony.Yandex.MainUserKeyNote'
DELETE FROM [Settings].[Localization] WHERE ResourceKey = 'Admin.Settings.Telephony.Yandex.ServiceUrl'
DELETE FROM [Settings].[Localization] WHERE ResourceKey = 'Admin.Settings.Telephony.Yandex.ServiceUrlNote'

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Product.ProductInfo.MarketplaceButtons.Header', 'Купить на маркетплейсах'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Product.ProductInfo.MarketplaceButtons.Header', 'Buy on marketplaces'

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.ShippingMethods.DeliveryByZones.NoHouse', 'Не указан номер дома'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.ShippingMethods.DeliveryByZones.NoHouse', 'The house number is not specified'

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.ConfirmSms.GetNewSmsCodeCountdown', 'Получить новый код можно через {{sec}} сек.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.ConfirmSms.GetNewSmsCodeCountdown', 'Get new sms code after {{sec}} sec.'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.ConfirmSms.NoClickSendSmsCode', 'Нажмите получить код подтверждения'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.ConfirmSms.NoClickSendSmsCode', 'Click get confirmation code'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.SmsConfirmation.EnterCaptchaCode', 'Введите код с картинки'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.SmsConfirmation.EnterCaptchaCode', 'Enter captcha code'

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Template.UseAdaptiveRootCategoryHint', 'Если настройка <b>включена</b>, то в качестве корневой категории будет использоваться первая по дереву категория, у которой больше одной подкатегории (по умолчанию Каталог), либо первая по дереву категория с товарами.<br/><br/>Настройку рекомендуется <b>выключить</b>, если необходимо выводить в меню единственную категорию первого уровня (с родителем Каталог).'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Template.UseAdaptiveRootCategoryHint', 'If the setting is <b>enabled</b>, then the first category in the tree that has more than one subcategory (by default Catalog) will be used as the root category, or the first category in the tree with goods.<br/><br/>It is recommended to <b>turn off</b> the setting. If you need to display a single first-level category in the menu (with a parent Catalog).'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Js.Builder.UseAdaptiveRootCategoryHint', 'Если настройка <b>включена</b>, то в качестве корневой категории будет использоваться первая по дереву категория, у которой больше одной подкатегории (по умолчанию Каталог), либо первая по дереву категория с товарами.<br/><br/>Настройку рекомендуется <b>выключить</b>, если необходимо выводить в меню единственную категорию первого уровня (с родителем Каталог).'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Js.Builder.UseAdaptiveRootCategoryHint', 'If the setting is <b>enabled</b>, then the first category in the tree that has more than one subcategory (by default Catalog) will be used as the root category, or the first category in the tree with goods.<br/><br/>It is recommended to <b>turn off</b> the setting. If you need to display a single first-level category in the menu (with a parent Catalog).'

GO--

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Catalog].[CategoryBrands]') AND type in (N'U'))
    BEGIN
        CREATE TABLE [Catalog].[CategoryBrands](
                                                   [CategoryId] [int] NOT NULL,
                                                   [BrandId] [int] NOT NULL,
                                                   CONSTRAINT [PK_CategoryBrands] PRIMARY KEY CLUSTERED
                                                       (
                                                        [CategoryId] ASC,
                                                        [BrandId] ASC
                                                           )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
        ) ON [PRIMARY]

        ALTER TABLE [Catalog].[CategoryBrands]  WITH CHECK ADD  CONSTRAINT [FK_Category_CategoryBrands] FOREIGN KEY([CategoryId])
            REFERENCES [Catalog].[Category] ([CategoryId]) ON DELETE CASCADE

        ALTER TABLE [Catalog].[CategoryBrands]  WITH CHECK ADD  CONSTRAINT [FK_Brand_CategoryBrands] FOREIGN KEY([BrandId])
            REFERENCES [Catalog].[Brand] ([BrandId]) ON DELETE CASCADE
    END

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Category.RightPanel.Brands', 'Производители'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Category.RightPanel.Brands', 'Brands'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Category.RightPanel.SelectBrands', 'Выберите производителей'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Category.RightPanel.SelectBrands', 'Select brands'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Category.RightPanel.RecalculateBrands', 'Пересчитать'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Category.RightPanel.RecalculateBrands', 'Recalculate'


GO--

CREATE FUNCTION [Settings].[GetRootCategory]
(
    @CategoryId int
)
    RETURNS int
AS
BEGIN
    DECLARE @id int
    DECLARE @parentId int
    set @id = @CategoryId
    set @parentId = @CategoryId
    while(@parentId <> 0)
        begin
            set @id = @parentId
            set @parentId = (Select [ParentCategory] from [Catalog].[Category] where [CategoryID] = @id)
        end
    RETURN @id
END

GO--

IF NOT EXISTS (SELECT TOP(1) 1 FROM [Catalog].[CategoryBrands]) AND (SELECT COUNT(ProductId) FROM [Catalog].[Product]) <= 100000
    BEGIN
        ;With CategoryBrands
                  AS
                  (
                      SELECT (Settings.[GetRootCategory](pc.CategoryID)) as CategoryId, p.BrandId, ROW_NUMBER() OVER (PARTITION BY pc.CategoryID Order by pc.CategoryID DESC) AS RowNum
                      FROM [Catalog].Product p
                               INNER JOIN [Catalog].ProductCategories pc ON pc.ProductId = p.ProductID
                               INNER JOIN [Catalog].[ProductExt] pExt ON p.ProductID = pExt.ProductID
                               INNER JOIN [Catalog].Category c ON c.CategoryID = pc.CategoryId
                      WHERE DisplayBrandsInMenu = 1
                        AND p.BrandId IS NOT NULL
                        AND p.[Enabled] = 1
                        AND p.Hidden = 0
                        AND p.CategoryEnabled = 1
                        AND c.Enabled = 1
                        AND (pExt.MaxAvailable > 0)
                      GROUP BY pc.CategoryID, p.BrandId
                  )

         INSERT INTO [Catalog].[CategoryBrands] (CategoryId, BrandId)
         SELECT DISTINCT CategoryId, BrandId FROM CategoryBrands WHERE RowNum <= 20

    END

GO--

DROP FUNCTION [Settings].[GetRootCategory]

GO--

IF NOT EXISTS (SELECT TOP(1) 1 FROM [Catalog].[CategoryBrands]) AND (SELECT COUNT(ProductId) FROM [Catalog].[Product]) <= 100000
    BEGIN
        INSERT INTO [Catalog].[CategoryBrands] (CategoryId, BrandId)
        SELECT TOP(20) pc.CategoryId, p.BrandId FROM [Catalog].Product p
                                                         INNER JOIN [Catalog].ProductCategories pc ON pc.ProductId = p.ProductID
                                                         INNER JOIN [Catalog].[ProductExt] pExt ON p.ProductID = pExt.ProductID
                                                         INNER JOIN [Catalog].Category c ON c.CategoryID = pc.CategoryId
        WHERE (ParentCategory = 0 OR ParentCategory = null)
          AND DisplayBrandsInMenu = 1
          AND p.BrandId IS NOT NULL
          AND p.[Enabled] = 1
          AND p.Hidden = 0
          AND p.CategoryEnabled = 1
          AND c.Enabled = 1
          AND (pExt.MaxAvailable > 0)
        GROUP BY pc.CategoryID, p.BrandId
    END

GO--
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Shipping].[FivePostPickPoints]') AND type in (N'U'))
    BEGIN
        CREATE TABLE [Shipping].[FivePostPickPoints](
                                                        [Id] nvarchar(max) NOT NULL,
                                                        [Name] nvarchar(255) NOT NULL,
                                                        [Type] int NOT NULL,
                                                        [Description] nvarchar(max) NULL,
                                                        [MaxWidth] float NOT NULL,
                                                        [MaxHeight] float NOT NULL,
                                                        [MaxLength] float NOT NULL,
                                                        [MaxWeight] float NOT NULL,
                                                        [Lattitude] float NOT NULL,
                                                        [Longitude] float NOT NULL,
                                                        [PossibleDeliveryList] nvarchar(max) NULL,
                                                        [RateList] nvarchar(max) NULL,
                                                        [ReturnAllowed] bit NOT NULL,
                                                        [IsCash] bit NOT NULL,
                                                        [IsCard] bit NOT NULL,
                                                        [FiasCode] nvarchar(255) NOT NULL,
                                                        [Address] nvarchar(max) NOT NULL,
                                                        [Region] nvarchar(255) NOT NULL,
                                                        [City] nvarchar(255) NOT NULL,
                                                        [Phone] nvarchar(30) NOT NULL,
                                                        [TimeWork] nvarchar(255) NOT NULL,
                                                        [LastUpdate] DateTime NOT NULL
        )
    END

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.ShippingMethods.FivePost.ApiKey', 'Api Key'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.ShippingMethods.FivePost.ApiKey', 'Api Key'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.ShippingMethods.FivePost.BarcodeEnrichment', 'Заполнение штрихкода'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.ShippingMethods.FivePost.BarcodeEnrichment', 'Barcode enrichment'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.ShippingMethods.FivePost.UndeliverableOption', 'Способ обработки невостребованного заказа'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.ShippingMethods.FivePost.UndeliverableOption', 'Method of processing an undeliverable order'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.ShippingMethods.FivePost.WithInsure', 'Со страховкой'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.ShippingMethods.FivePost.WithInsure', 'With insure'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.ShippingMethods.FivePost.OrderStatuses', 'Статусы заказов'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.ShippingMethods.FivePost.OrderStatuses', 'Order statuses'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.ShippingMethods.FivePost.CheckSettingsWithManagerHelpText', 'Значение данной настройки необходимо согласовать с менеджером 5Пост'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.ShippingMethods.FivePost.CheckSettingsWithManagerHelpText', 'The value of this setting must be agreed with the 5Post manager'

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Shipping.FivePost.BarCodeFromFivePost', 'Штрихкод берется из 5Пост'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Shipping.FivePost.BarCodeFromFivePost', 'The barcode taken from 5Post'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Shipping.FivePost.BarCodeFromAdvantshop', 'Штрихкод берется из AdvantShop'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Shipping.FivePost.BarCodeFromAdvantshop', 'The barcode taken from AdvantShop'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Shipping.FivePost.BarCodePartial', 'Штрихкод берется из AdvantShop, если заполнен, иначе из 5Пост'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Shipping.FivePost.BarCodePartial', 'The barcode is taken from AdvantShop, if completed, otherwise from 5Post'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Shipping.FivePost.ReturnToWarehouse', 'Возврат на склад'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Shipping.FivePost.ReturnToWarehouse', 'Return to warehouse'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Shipping.FivePost.Utilization', 'Утилизация'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Shipping.FivePost.Utilization', 'Utilization'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Shipping.FivePost.Monday', 'Понедельник'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Shipping.FivePost.Monday', 'Monday'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Shipping.FivePost.Tuesday', 'Вторник'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Shipping.FivePost.Tuesday', 'Tuesday'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Shipping.FivePost.Wednesday', 'Среда'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Shipping.FivePost.Wednesday', 'Wednesday'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Shipping.FivePost.Thursday', 'Четверг'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Shipping.FivePost.Thursday', 'Thursday'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Shipping.FivePost.Friday', 'Пятница'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Shipping.FivePost.Friday', 'Friday'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Shipping.FivePost.Saturday', 'Суббота'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Shipping.FivePost.Saturday', 'Saturday'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Shipping.FivePost.Sunday', 'Воскресенье'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Shipping.FivePost.Sunday', 'Sunday'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Shipping.FivePost.OrderStatus.New', 'Новый'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Shipping.FivePost.OrderStatus.New', 'New'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Shipping.FivePost.OrderStatus.Approved', 'Подтвержден'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Shipping.FivePost.OrderStatus.Approved', 'Approved'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Shipping.FivePost.OrderStatus.Rejected', 'Отклонен'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Shipping.FivePost.OrderStatus.Rejected', 'Rejected'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Shipping.FivePost.OrderStatus.InProgress', 'В процессе исполнения'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Shipping.FivePost.OrderStatus.InProgress', 'In progress'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Shipping.FivePost.OrderStatus.Done', 'Исполнен'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Shipping.FivePost.OrderStatus.Done', 'Done'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Shipping.FivePost.OrderStatus.Interrupted', 'Исполнение прервано'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Shipping.FivePost.OrderStatus.Interrupted', 'Interrupted'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Shipping.FivePost.OrderStatus.Cancelled', 'Заказ был отменен'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Shipping.FivePost.OrderStatus.Cancelled', 'Cancelled'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Shipping.FivePost.OrderStatus.Unclaimed', 'Не востребован'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Shipping.FivePost.OrderStatus.Unclaimed', 'Unclaimed'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Shipping.FivePost.OrderStatus.Problem', 'Проблема'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Shipping.FivePost.OrderStatus.Problem', 'Problem'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Shipping.TypeViewPoint.List', 'Списком'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Shipping.TypeViewPoint.List', 'By list'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Core.Shipping.TypeViewPoint.YandexMaps', 'Через Яндекс.Карты'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Core.Shipping.TypeViewPoint.YandexMaps', 'By yandex maps'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.ShippingMethods.FivePost.Tarifs', 'Тарифы доставки'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.ShippingMethods.FivePost.Tarifs', 'Tarifs'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.ShippingMethods.FivePost.Warehouses', 'Склады'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.ShippingMethods.FivePost.Warehouses', 'Warehouses'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.ShippingMethods.FivePost.Warehouse', 'Склад'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.ShippingMethods.FivePost.Warehouse', 'Warehouse'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.ShippingMethods.FivePost.DeliveryType', 'Город, к которому привязан склад в 5Пост'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.ShippingMethods.FivePost.DeliveryType', 'The city to which the warehouse in the 5th Post is linked'
EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.ShippingMethods.FivePost.DeliveryType.Hint', 'Логистическая схема, привязанная к складу. Можно узнать у менеджера 5Пост.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.ShippingMethods.FivePost.DeliveryType.Hint', 'Logistics scheme linked to the warehouse. You can find out from the manager 5 Post.'

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Settings.Catalog.SearchDeepInAdminPart', 'Глубинка поиска в части администрирования'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Settings.Catalog.SearchDeepInAdminPart', 'Search depth in admin part'

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.OfferReport.ChooseOffer', 'Выберите модификацию'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.OfferReport.ChooseOffer', 'Choose offer'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.OfferReport.Sum', 'Статистика продаж'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.OfferReport.Sum', 'Sales statistics'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.OfferReport.NumberOfOrder', 'Номер заказа'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.OfferReport.NumberOfOrder', 'Number of order'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.OfferReport.Customer', 'Покупатель'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.OfferReport.Customer', 'Customer'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.OfferReport.Paid', 'Оплачен'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.OfferReport.Paid', 'Paid'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.OfferReport.Date', 'Дата'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.OfferReport.Date', 'Date'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.OfferReport.QuantityOfOffer', 'Кол-во модификаций'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.OfferReport.QuantityOfOffer', 'Quantity of offer'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Analytics.Offer', 'Отчет по модификации'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Analytics.Offer', 'Offer report'

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Sizes.Edit.SizeNameForCategory', 'Размеры для категории'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Sizes.Edit.SizeNameForCategory', 'Sizes for the category'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.Sizes.SizeNameForCategory', 'Размеры для категорий'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.Sizes.SizeNameForCategory', 'Sizes for categories'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Sizes.Index.AddSizeNameForCategories', 'Размеры для категорий'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Sizes.Index.AddSizeNameForCategories', 'Sizes for categories'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.SizeForCategory.NameForCategory', 'Размер для категории'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.SizeForCategory.NameForCategory', 'Size for category'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.SizeForCategory.SizeName', 'Размер'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.SizeForCategory.SizeName', 'Size'

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[Catalog].[Category_Size]') AND type in (N'U'))
    BEGIN
        CREATE TABLE [Catalog].[Category_Size](
                                                  [CategoryId] [int] NOT NULL,
                                                  [SizeId] [int] NOT NULL,
                                                  [SizeNameForCategory] [nvarchar](300) NOT NULL,
                                                  CONSTRAINT [PK_Category_Size] PRIMARY KEY CLUSTERED
                                                      (
                                                       [CategoryId] ASC,
                                                       [SizeId] ASC
                                                          )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
        ) ON [PRIMARY]

        ALTER TABLE [Catalog].[Category_Size]  WITH CHECK ADD  CONSTRAINT [FK_Category_Category_Size] FOREIGN KEY([CategoryId])
            REFERENCES [Catalog].[Category] ([CategoryId]) ON DELETE CASCADE

        ALTER TABLE [Catalog].[Category_Size]  WITH CHECK ADD  CONSTRAINT [FK_Size_Category_Size] FOREIGN KEY([SizeId])
            REFERENCES [Catalog].[Size] ([SizeID]) ON DELETE CASCADE
    END

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Bonuses.YourBonuses', 'У вас есть'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Bonuses.YourBonuses', 'Do you have'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Bonuses.AvailableBonuses', 'Доступно для списания'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Bonuses.AvailableBonuses', 'Available for debiting'

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.Js.AddDeliveryZone.MinimalOrderPrice', 'Минимальная сумма заказа'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.Js.AddDeliveryZone.MinimalOrderPrice', 'Minimum order amount'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Admin.ShippingMethods.DeliveryByZones.MinimalOrderPrice', 'Минимальная сумма заказа: {0}. Вам необходимо приобрести еще товаров на сумму: {1}.'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Admin.ShippingMethods.DeliveryByZones.MinimalOrderPrice', 'Minimum price to order: {0}. You need to buy more products to the value: {1}.'

GO--

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Captcha.InputPlaceholder', 'Код с картинки'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Captcha.InputPlaceholder', 'Code with pictures'

EXEC [Settings].[sp_AddUpdateLocalization] 1, 'Captcha.InputNote', 'буквы на русском языке'
EXEC [Settings].[sp_AddUpdateLocalization] 2, 'Captcha.InputNote', ''

GO--

UPDATE [Settings].[InternalSettings] SET [settingValue] = '12.0.4' WHERE [settingKey] = 'db_version'
