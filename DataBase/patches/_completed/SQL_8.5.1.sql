update [Settings].[Localization] set ResourceValue = 'Полная форма оформления заказа (если не выбрано - заказ в 1 клик)' where  [LanguageId]=1 and [ResourceKey] = 'Admin.Settings.MobileVersion.IsFullCheckout'

GO--

ALTER PROCEDURE [Catalog].[sp_UpdateOffer]  
   @OfferID int,  
   @ProductID int,  
   @ArtNo nvarchar(100),  
   @Amount float,  
   @Price float,  
   @SupplyPrice float,  
   @ColorID int,  
   @SizeID int,  
   @Main bit,
   @Weight float,  
   @Length float,  
   @Width float,  
   @Height float
AS  
BEGIN  
  UPDATE [Catalog].[Offer]  
  SET     
      [ProductID] = @ProductID
	 ,ArtNo=@ArtNo  
     ,[Amount] = @Amount  
     ,[Price] = @Price  
     ,[SupplyPrice] = @SupplyPrice  
     ,[ColorID] = @ColorID  
     ,[SizeID] = @SizeID  
     ,Main = @Main
	 ,Weight = @Weight
	 ,Length = @Length
	 ,Width = @Width
	 ,Height = @Height
  WHERE [OfferID] = @OfferID  
END

GO--

ALTER PROCEDURE [Catalog].[sp_AddOffer]  
   @ArtNo nvarchar(100),  
   @ProductID int,  
   @Amount float,  
   @Price float,  
   @SupplyPrice float,  
   @ColorID int,  
   @SizeID int,  
   @Main bit,  
   @Weight float,  
   @Length float,  
   @Width float,  
   @Height float
AS  
BEGIN  
  
 INSERT INTO [Catalog].[Offer]  
  (  
     ArtNo
   ,[ProductID]  
   ,[Amount]  
   ,[Price]  
   ,[SupplyPrice]  
   ,ColorID  
   ,SizeID  
   ,Main
   ,Weight
   ,Length
   ,Width
   ,Height
  )  
 VALUES  
  (  
    @ArtNo  
   ,@ProductID  
   ,@Amount  
   ,@Price  
   ,@SupplyPrice  
   ,@ColorID  
   ,@SizeID  
   ,@Main
   ,@Weight
   ,@Length
   ,@Width
   ,@Height  
  );  
 SELECT SCOPE_IDENTITY();  
END

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.OrderStatuses.DeletingImpossibleByTariff', 'Удаление статусов заказов не доступно на вашем тарифном плане')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.OrderStatuses.DeletingImpossibleByTariff', 'The removal of the status of the orders is not available on your tariff plan')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.OrderStatuses.Index.CantDeleteByTariff', 'На данном тарифе создание статусов заказа запрещено')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.OrderStatuses.Index.CantDeleteByTariff', 'On this tariff creation of order statuses is prohibited')

GO--

ALTER TABLE [Order].OrderItems ADD
	IsGift bit NULL
GO--


ALTER PROCEDURE [Order].[sp_AddOrderItem]  
	 @OrderID int,  
	 @Name nvarchar(255),  
	 @Price float,  
	 @Amount float,  
	 @ProductID int,  
	 @ArtNo nvarchar(100),  
	 @SupplyPrice float,  
	 @Weight float,  
	 @IsCouponApplied bit,  
	 @Color nvarchar(50),  
	 @Size nvarchar(50),  
	 @DecrementedAmount float,  
	 @PhotoID int,  
	 @IgnoreOrderDiscount bit,  
	 @AccrueBonuses bit,
	 @TaxId int,
	 @TaxName nvarchar(50),
	 @TaxType int,
	 @TaxRate float(53),
	 @TaxShowInPrice bit,
	 @Width float,
	 @Height float,
	 @Length float,
	 @PaymentMethodType int,
	 @PaymentSubjectType int,
	 @IsGift bit
AS  
BEGIN  
   
 INSERT INTO [Order].OrderItems  
	   ([OrderID]  
	   ,[ProductID]  
	   ,[Name]  
	   ,[Price]  
	   ,[Amount]  
	   ,[ArtNo]  
	   ,[SupplyPrice]  
	   ,[Weight]  
	   ,[IsCouponApplied]  
	   ,[Color]  
	   ,[Size]  
	   ,[DecrementedAmount]  
	   ,[PhotoID]  
	   ,[IgnoreOrderDiscount]  
	   ,[AccrueBonuses] 
	   ,TaxId
	   ,TaxName
	   ,TaxType
	   ,TaxRate
	   ,TaxShowInPrice
	   ,Width
	   ,Height
	   ,[Length]
	   ,PaymentMethodType
	   ,PaymentSubjectType
	   ,IsGift
	   )  
 VALUES  
	   (@OrderID  
	   ,@ProductID  
	   ,@Name  
	   ,@Price  
	   ,@Amount  
	   ,@ArtNo  
	   ,@SupplyPrice  
	   ,@Weight  
	   ,@IsCouponApplied  
	   ,@Color  
	   ,@Size  
	   ,@DecrementedAmount  
	   ,@PhotoID  
	   ,@IgnoreOrderDiscount  
	   ,@AccrueBonuses
	   ,@TaxId
	   ,@TaxName
	   ,@TaxType
	   ,@TaxRate
	   ,@TaxShowInPrice   
	   ,@Width
	   ,@Height
	   ,@Length
	   ,@PaymentMethodType
	   ,@PaymentSubjectType
	   ,@IsGift
   );  
       
 SELECT SCOPE_IDENTITY()  
END  

GO--

ALTER PROCEDURE [Order].[sp_UpdateOrderItem]  
	@OrderItemID int,  
	@OrderID int,  
	@Name nvarchar(255),  
	@Price float,  
	@Amount float,  
	@ProductID int,  
	@ArtNo nvarchar(100),  
	@SupplyPrice float,  
	@Weight float,  
	@IsCouponApplied bit,  
	@Color nvarchar(50),  
	@Size nvarchar(50),  
	@DecrementedAmount float,  
	@PhotoID int,  
	@IgnoreOrderDiscount bit,  
	@AccrueBonuses bit,
	@TaxId int,
	@TaxName nvarchar(50),
	@TaxType int,
	@TaxRate float(53),
	@TaxShowInPrice bit,
	@Width float,
	@Height float,
	@Length float,
	@PaymentMethodType int,
	@PaymentSubjectType int,
	@IsGift bit
AS  
BEGIN  
 Update [Order].[OrderItems]  
 Set  
     [Name] = @Name  
	,[Price] = @Price  
	,[Amount] = @Amount  
	,[ArtNo] = @ArtNo  
	,[SupplyPrice] = @SupplyPrice  
	,[Weight] = @Weight  
	,[IsCouponApplied] = @IsCouponApplied  
	,[Color] = Color  
    ,[Size] = Size  
    ,[DecrementedAmount] = DecrementedAmount  
    ,[PhotoID] = @PhotoID  
    ,[IgnoreOrderDiscount] = @IgnoreOrderDiscount  
    ,[AccrueBonuses] = @AccrueBonuses
	,TaxId = @TaxId
	,TaxName = @TaxName
	,TaxType = @TaxType
	,TaxRate = @TaxRate
	,TaxShowInPrice = @TaxShowInPrice
	,Width = @Width
	,Height = @Height
	,[Length] = @Length
	,PaymentMethodType = @PaymentMethodType
	,PaymentSubjectType = @PaymentSubjectType
	,IsGift = @IsGift
 Where OrderItemID = @OrderItemID  
END 

GO--

Insert Into [Settings].[Settings] ([Name],[Value]) Values ('LandingMetaKeywords', '')
Insert Into [Settings].[Settings] ([Name],[Value]) Values ('LandingMetaDescription', '')

GO--

update cms.staticblock set Content = replace(Content, 'Мы очень дорожим вашим мнением и готовы выслушать все что вы хотите сообщить. <br />Ответим а течении суток с момента отправки сообщения', 'Мы очень дорожим вашим мнением и прислушиваемся ко всем вашим пожеланиям и предложениям.<br />Ответ поступит в самое ближайшее время.')

GO--

UPDATE [Settings].[Localization] Set [ResourceValue] = 'Ошибка сохранения купона' Where [ResourceKey] = 'Admin.Js.Order.CouponSavingError' and [LanguageId] = 1

GO--

ALTER TABLE Catalog.Coupon ADD
	EntityId int NULL

GO--
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Core.Payment.PaymentMethod.ButtonTextWithoutStamp', 'Распечатать счет без печати')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Core.Payment.PaymentMethod.ButtonTextWithoutStamp', 'Print bill without stamp')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Js.Payment.PaymentMethod.ButtonTextWithoutStamp', 'Распечатать счет без печати')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Js.Payment.PaymentMethod.ButtonTextWithoutStamp', 'Print bill without stamp')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.SettingsSystem.SystemCommon.ShowCaptchaInBuyInOneClick', 'Показывать в покупке в один клик')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.SettingsSystem.SystemCommon.ShowCaptchaInBuyInOneClick', 'Show in buy one click')

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Checkout.CheckoutBuyInOneClick.CapchaWrong', 'Неверно введен код')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Checkout.CheckoutBuyInOneClick.CapchaWrong', 'Wrong captcha text')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.Tasks.ChangeTaskGroup.ChangeTheGroupOfTasks', 'Сменить проект')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.Tasks.ChangeTaskGroup.ChangeTheGroupOfTasks', 'Change the group of tasks')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.Tasks.ChangeTaskGroup.Edit', 'Сменить')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.Tasks.ChangeTaskGroup.Edit', 'Change')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.Tasks.ChangeTaskGroup.Cancel', 'Отменить')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.Tasks.ChangeTaskGroup.Cancel', 'Cancel')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.Tasks.ChangeTaskGroup.FailedToChange', 'Не удалось сменить проект')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.Tasks.ChangeTaskGroup.FailedToChange', 'Failed to change the group')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.Tasks.Tasks.ChangeTaskGroup', 'Сменить проект')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.Tasks.Tasks.ChangeTaskGroup', 'Change the group of tasks')

GO--

INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.Js.AddEditYandexPromo.WarningCouponTooLong', 'Текущий купон имеет код длиннее чем 20 символов')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.Js.AddEditYandexPromo.WarningCouponTooLong', 'Current coupon too long')

GO--

UPDATE [Settings].[InternalSettings] SET [settingValue] = '8.5.1' WHERE [settingKey] = 'db_version'
