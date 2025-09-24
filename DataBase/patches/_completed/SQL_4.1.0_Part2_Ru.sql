
-- Subjects for mail formats

  Update [Settings].[MailFormat] 
  Set [FormatSubject] = '#SHOPURL# - Пользователь #EMAIL# успешно зарегистрирован.'
  Where [FormatType] = 1
   GO--

  Update [Settings].[MailFormat] 
  Set [FormatSubject] = 'Восстановление пароля'
  Where [FormatType] = 2
   GO--

  Update [Settings].[MailFormat] 
  Set [FormatSubject] = 'Заказ № #ORDER_ID# принят'
  Where [FormatType] = 3
   GO--

  Update [Settings].[MailFormat] 
  Set [FormatSubject] = 'Статус заказа был изменен'
  Where [FormatType] = 4
   GO--

  Update [Settings].[MailFormat] 
  Set [FormatSubject] = 'Рассылка сообщений'
  Where [FormatType] = 5
   GO--

  Update [Settings].[MailFormat] 
  Set [FormatSubject] = 'Подписка на новости'
  Where [FormatType] = 6
   GO--

  Update [Settings].[MailFormat] 
  Set [FormatSubject] = 'Отписка от новостей'
  Where [FormatType] = 7
     GO--
  
  Update [Settings].[MailFormat] 
  Set [FormatSubject] = 'Отзыв'
  Where [FormatType] = 8
     GO--
  
  Update [Settings].[MailFormat] 
  Set [FormatSubject] = 'Сообщить другу'
  Where [FormatType] = 9
   GO--

  Update [Settings].[MailFormat] 
  Set [FormatSubject] = 'Вопрос о продукте'
  Where [FormatType] = 10
       GO--
 
  Update [Settings].[MailFormat] 
  Set [FormatSubject] = 'Добавлен отзыв к "#PRODUCTNAME#"'
  Where [FormatType] = 11
   GO--
       
  Update [Settings].[MailFormat] 
  Set [FormatSubject] = 'Товар под заказ'
  Where [FormatType] = 12
   GO--
  
  Update [Settings].[MailFormat] 
  Set [FormatSubject] = 'Ссылка на покупку товара под заказ'
  Where [FormatType] = 13
   GO--
  
  Update [Settings].[MailFormat] 
  Set [FormatSubject] = 'Невозможно выполнить заказ'
  Where [FormatType] = 14
  
   GO--
  Update [Settings].[MailFormat] 
  Set [FormatSubject] = 'Подарочный сертификат'
  Where [FormatType] = 15
  
    GO--
  Update [Settings].[MailFormat] 
  Set [FormatSubject] = 'Заказ в один клик № #ORDER_ID#'
  Where [FormatType] = 16
 
  GO--
 Insert Into [Settings].[Settings] ([Name],[Value]) Values('UserAgreementText', ' Нажимая кнопку "Продолжить", я подтверждаю свою дееспособность, даю согласие на обработку своих персональных данных.');

  GO--

 Insert Into [CMS].[StaticBlock] ([Key],[InnerName],[Content],[Added],[Modified],[Enabled]) 
Values ('OrderSuccessTop', 'Успешное оформление заказа (блок сверху)', 'Менеджеры магазина получили Ваш заказ.
<br />
<br />
На адрес Вашей электронной почты отправлено письмо с подтверждением. Спасибо!
<br />
<br />', GETDATE(), GETDATE(), 1)
 GO--
 Insert Into [CMS].[StaticBlock] ([Key],[InnerName],[Content],[Added],[Modified],[Enabled])
  Values ('SocialLogo', 'Логотип в соц сетях', '<figure class="logo-cell"><img id="logo" src="images/nophoto-logo.png"></figure>', GETDATE(), GETDATE(), 1)

  GO-- 

 Insert Into [Settings].[Settings] ([Name],[Value]) Values('CustomerFirstNameField', 'Имя');
 Insert Into [Settings].[Settings] ([Name],[Value]) Values('CustomerPhoneField', 'Контактный телефон');
 Insert Into [Settings].[Settings] ([Name],[Value]) Values('CustomShippingField1', 'Настраиваемое поле 1');
 Insert Into [Settings].[Settings] ([Name],[Value]) Values('CustomShippingField2', 'Настраиваемое поле 2');
 Insert Into [Settings].[Settings] ([Name],[Value]) Values('CustomShippingField3', 'Настраиваемое поле 3');

 Insert Into [Settings].[Settings] ([Name],[Value]) Values('BuyInOneClickName', 'Ваше имя');
 Insert Into [Settings].[Settings] ([Name],[Value]) Values('BuyInOneClickEmail', 'Email');
 Insert Into [Settings].[Settings] ([Name],[Value]) Values('BuyInOneClickPhone', 'Контактный телефон');
 Insert Into [Settings].[Settings] ([Name],[Value]) Values('BuyInOneClickComment', 'Комментарий');
 
 GO--

 Update Customers.Country Set SortOrder = 500, DisplayInPopup=1 where countryIso3='RUS'
 Update Customers.Country Set SortOrder = 400, DisplayInPopup=1 where countryIso3='UKR'
 Update Customers.Country Set SortOrder = 300, DisplayInPopup=1 where countryIso3='BLR'
 Update Customers.Country Set SortOrder = 200, DisplayInPopup=1 where countryIso3='KAZ'

 GO--

update Customers.City set CitySort = 900, DisplayInPopup=1 where CityName = 'Москва'
update Customers.City set CitySort = 800, DisplayInPopup=1 where CityName = 'Санкт-Петербург'
update Customers.City set CitySort = 100, DisplayInPopup=1 where CityName = 'Новосибирск'
update Customers.City set CitySort = 100, DisplayInPopup=1 where CityName = 'Екатеринбург'
update Customers.City set CitySort = 100, DisplayInPopup=1 where CityName = 'Казань'
update Customers.City set CitySort = 100, DisplayInPopup=1 where CityName = 'Нижний Новгород'
update Customers.City set CitySort = 100, DisplayInPopup=1 where CityName = 'Самара'
update Customers.City set CitySort = 100, DisplayInPopup=1 where CityName = 'Омск'
update Customers.City set CitySort = 100, DisplayInPopup=1 where CityName = 'Челябинск'
update Customers.City set CitySort = 100, DisplayInPopup=1 where CityName = 'Ростов-на-Дону'
update Customers.City set CitySort = 100, DisplayInPopup=1 where CityName = 'Уфа'
update Customers.City set CitySort = 100, DisplayInPopup=1 where CityName = 'Красноярск'
update Customers.City set CitySort = 100, DisplayInPopup=1 where CityName = 'Пермь'
update Customers.City set CitySort = 100, DisplayInPopup=1 where CityName = 'Волгоград'
update Customers.City set CitySort = 100, DisplayInPopup=1 where CityName = 'Воронеж'
update Customers.City set CitySort = 100, DisplayInPopup=1 where CityName = 'Саратов'
update Customers.City set CitySort = 100, DisplayInPopup=1 where CityName = 'Краснодар'
update Customers.City set CitySort = 100, DisplayInPopup=1 where CityName = 'Тольятти'
update Customers.City set CitySort = 100, DisplayInPopup=1 where CityName = 'Тюмень'
update Customers.City set CitySort = 100, DisplayInPopup=1 where CityName = 'Ижевск'
update Customers.City set CitySort = 100, DisplayInPopup=1 where CityName = 'Барнаул'
update Customers.City set CitySort = 100, DisplayInPopup=1 where CityName = 'Ульяновск'
update Customers.City set CitySort = 100, DisplayInPopup=1 where CityName = 'Иркутск'
update Customers.City set CitySort = 100, DisplayInPopup=1 where CityName = 'Владивосток'
update Customers.City set CitySort = 100, DisplayInPopup=1 where CityName = 'Ярославль'
update Customers.City set CitySort = 100, DisplayInPopup=1 where CityName = 'Хабаровск'
update Customers.City set CitySort = 100, DisplayInPopup=1 where CityName = 'Махачкала'
update Customers.City set CitySort = 100, DisplayInPopup=1 where CityName = 'Оренбург'
update Customers.City set CitySort = 100, DisplayInPopup=1 where CityName = 'Томск'
update Customers.City set CitySort = 100, DisplayInPopup=1 where CityName = 'Новокузнецк'
update Customers.City set CitySort = 100, DisplayInPopup=1 where CityName = 'Кемерово'
update Customers.City set CitySort = 100, DisplayInPopup=1 where CityName = 'Астрахань'


GO--
update Customers.City set CitySort = 900, DisplayInPopup=1 where CityName = 'Киев'
update Customers.City set CitySort = 100, DisplayInPopup=1 where CityName = 'Одесса'
update Customers.City set CitySort = 100, DisplayInPopup=1 where CityName = 'Харьков'
update Customers.City set CitySort = 100, DisplayInPopup=1 where CityName = 'Днепропетровск'
update Customers.City set CitySort = 100, DisplayInPopup=1 where CityName = 'Донецк' and regionid=90
update Customers.City set CitySort = 100, DisplayInPopup=1 where CityName = 'Запорожье'
update Customers.City set CitySort = 100, DisplayInPopup=1 where CityName = 'Львов'
update Customers.City set CitySort = 100, DisplayInPopup=1 where CityName = 'Кривой Рог'
update Customers.City set CitySort = 100, DisplayInPopup=1 where CityName = 'Николаев'
update Customers.City set CitySort = 100, DisplayInPopup=1 where CityName = 'Мариуполь'
update Customers.City set CitySort = 100, DisplayInPopup=1 where CityName = 'Луганск'
update Customers.City set CitySort = 100, DisplayInPopup=1 where CityName = 'Винница'
update Customers.City set CitySort = 100, DisplayInPopup=1 where CityName = 'Макеевкa'
update Customers.City set CitySort = 100, DisplayInPopup=1 where CityName = 'Херсон'
update Customers.City set CitySort = 100, DisplayInPopup=1 where CityName = 'Полтава'
update Customers.City set CitySort = 100, DisplayInPopup=1 where CityName = 'Чернигов'
update Customers.City set CitySort = 100, DisplayInPopup=1 where CityName = 'Черкассы'
update Customers.City set CitySort = 100, DisplayInPopup=1 where CityName = 'Житомир'
update Customers.City set CitySort = 100, DisplayInPopup=1 where CityName = 'Сумы'
update Customers.City set CitySort = 100, DisplayInPopup=1 where CityName = 'Хмельницкий'

GO--

Update Customers.Region set CountryID = 171 where regionid=84 or regionid=86

GO--

Insert Into [CMS].[StaticBlock] ([Key],[InnerName],[Content],[Added],[Modified],[Enabled])
  Values ('DescriptionDetails', 'Блок под ценой товара', '', GETDATE(), GETDATE(), 1)

GO--


----- города Беларуси

if (select count(countryid) from customers.country where Countryiso3='BLR') > 0 
	and (select count(regionID) from customers.Region inner join customers.country on country.Countryid = Region.CountryID where Countryiso3='BLR') = 0 
begin
	declare @countryID int;
	declare @regionId int;
	set @countryID = (select countryid from customers.country where Countryiso3='BLR')
	
	insert into customers.Region (CountryID, RegionName) values(@countryID, 'Минская область');
	set @regionId = (select scope_identity());

	insert into customers.City (RegionID, CityName, CitySort, DisplayInPopup) values(@regionId, 'Минск', 100, 1);
	insert into customers.City (RegionID, CityName, CitySort, DisplayInPopup) values(@regionId, 'Борисов', 0, 1);
	insert into customers.City (RegionID, CityName, CitySort, DisplayInPopup) values(@regionId, 'Солигорск', 0, 1);
	insert into customers.City (RegionID, CityName, CitySort, DisplayInPopup) values(@regionId, 'Молодечно', 0, 1);
	insert into customers.City (RegionID, CityName, CitySort, DisplayInPopup) values(@regionId, 'Жодино', 0, 0);
	insert into customers.City (RegionID, CityName, CitySort, DisplayInPopup) values(@regionId, 'Слуцк', 0, 0);
	insert into customers.City (RegionID, CityName, CitySort, DisplayInPopup) values(@regionId, 'Вилейка', 0, 0);
	insert into customers.City (RegionID, CityName, CitySort, DisplayInPopup) values(@regionId, 'Дзержинск', 0, 0);
	insert into customers.City (RegionID, CityName, CitySort, DisplayInPopup) values(@regionId, 'Марьина Горка', 0, 0);
	insert into customers.City (RegionID, CityName, CitySort, DisplayInPopup) values(@regionId, 'Столбцы', 0, 0);
	insert into customers.City (RegionID, CityName, CitySort, DisplayInPopup) values(@regionId, 'Смолевичи', 0, 0);
	insert into customers.City (RegionID, CityName, CitySort, DisplayInPopup) values(@regionId, 'Слуцк', 0, 0);
	insert into customers.City (RegionID, CityName, CitySort, DisplayInPopup) values(@regionId, 'Вилейка', 0, 0);




	insert into customers.Region (CountryID, RegionName) values(@countryID, 'Витебская область');
	set @regionId = (select scope_identity());

	insert into customers.City (RegionID, CityName, CitySort, DisplayInPopup) values(@regionId, 'Витебск', 0, 1);
	insert into customers.City (RegionID, CityName, CitySort, DisplayInPopup) values(@regionId, 'Орша', 0, 1);
	insert into customers.City (RegionID, CityName, CitySort, DisplayInPopup) values(@regionId, 'Новополоцк', 0, 1);
	insert into customers.City (RegionID, CityName, CitySort, DisplayInPopup) values(@regionId, 'Полоцк', 0, 1);
	insert into customers.City (RegionID, CityName, CitySort, DisplayInPopup) values(@regionId, 'Поставы', 0, 0);
	insert into customers.City (RegionID, CityName, CitySort, DisplayInPopup) values(@regionId, 'Глубокое', 0, 0);
	insert into customers.City (RegionID, CityName, CitySort, DisplayInPopup) values(@regionId, 'Лепель', 0, 0);
	insert into customers.City (RegionID, CityName, CitySort, DisplayInPopup) values(@regionId, 'Новолукомль', 0, 0);
	insert into customers.City (RegionID, CityName, CitySort, DisplayInPopup) values(@regionId, 'Городок', 0, 0);
	

	
	insert into customers.Region (CountryID, RegionName) values(@countryID, 'Могилёвская область');
	set @regionId = (select scope_identity());

	insert into customers.City (RegionID, CityName, CitySort, DisplayInPopup) values(@regionId, 'Могилёв', 0, 1);
	insert into customers.City (RegionID, CityName, CitySort, DisplayInPopup) values(@regionId, 'Бобруйск', 0, 1);
	insert into customers.City (RegionID, CityName, CitySort, DisplayInPopup) values(@regionId, 'Горки', 0, 1);
	insert into customers.City (RegionID, CityName, CitySort, DisplayInPopup) values(@regionId, 'Осиповичи', 0, 0);
	insert into customers.City (RegionID, CityName, CitySort, DisplayInPopup) values(@regionId, 'Кричев', 0, 0);

		
	insert into customers.Region (CountryID, RegionName) values(@countryID, 'Гомельская область');
	set @regionId = (select scope_identity());

	insert into customers.City (RegionID, CityName, CitySort, DisplayInPopup) values(@regionId, 'Гомель', 0, 1);
	insert into customers.City (RegionID, CityName, CitySort, DisplayInPopup) values(@regionId, 'Мозырь', 0, 1);
	insert into customers.City (RegionID, CityName, CitySort, DisplayInPopup) values(@regionId, 'Жлобин', 0, 1);
	insert into customers.City (RegionID, CityName, CitySort, DisplayInPopup) values(@regionId, 'Светлогорск', 0, 0);
	insert into customers.City (RegionID, CityName, CitySort, DisplayInPopup) values(@regionId, 'Калинковичи', 0, 0);
	insert into customers.City (RegionID, CityName, CitySort, DisplayInPopup) values(@regionId, 'Рогачёв', 0, 0);
	insert into customers.City (RegionID, CityName, CitySort, DisplayInPopup) values(@regionId, 'Добруш', 0, 0);

	insert into customers.Region (CountryID, RegionName) values(@countryID, 'Брестская область');
	set @regionId = (select scope_identity());

	insert into customers.City (RegionID, CityName, CitySort, DisplayInPopup) values(@regionId, 'Брест', 0, 1);
	insert into customers.City (RegionID, CityName, CitySort, DisplayInPopup) values(@regionId, 'Барановичи', 0, 1);
	insert into customers.City (RegionID, CityName, CitySort, DisplayInPopup) values(@regionId, 'Пинск', 0, 1);
	insert into customers.City (RegionID, CityName, CitySort, DisplayInPopup) values(@regionId, 'Кобрин', 0, 0);
	insert into customers.City (RegionID, CityName, CitySort, DisplayInPopup) values(@regionId, 'Берёза', 0, 0);
	insert into customers.City (RegionID, CityName, CitySort, DisplayInPopup) values(@regionId, 'Лунинец', 0, 0);
	insert into customers.City (RegionID, CityName, CitySort, DisplayInPopup) values(@regionId, 'Ивацевичи', 0, 0);


	insert into customers.Region (CountryID, RegionName) values(@countryID, 'Гродненская область');
	set @regionId = (select scope_identity());

	insert into customers.City (RegionID, CityName, CitySort, DisplayInPopup) values(@regionId, 'Гродно', 0, 1);
	insert into customers.City (RegionID, CityName, CitySort, DisplayInPopup) values(@regionId, 'Лида', 0, 1);
	insert into customers.City (RegionID, CityName, CitySort, DisplayInPopup) values(@regionId, 'Слоним', 0, 0);
	insert into customers.City (RegionID, CityName, CitySort, DisplayInPopup) values(@regionId, 'Волковыск', 0, 0);
	insert into customers.City (RegionID, CityName, CitySort, DisplayInPopup) values(@regionId, 'Сморгонь', 0, 0);
	insert into customers.City (RegionID, CityName, CitySort, DisplayInPopup) values(@regionId, 'Новогрудок', 0, 0);
	insert into customers.City (RegionID, CityName, CitySort, DisplayInPopup) values(@regionId, 'Мосты', 0, 0);

end

GO--


----- Города Казахстана
GO--

 
if (select count(countryid) from customers.country where Countryiso3='KAZ') > 0 
	and (select count(regionID) from customers.Region inner join customers.country on country.Countryid = Region.CountryID where Countryiso3='KAZ') = 0 
begin
	declare @countryID int;
	declare @regionId int;
	set @countryID = (select countryid from customers.country where Countryiso3='KAZ')
	
	insert into customers.Region (CountryID, RegionName) values(@countryID, 'Алма-Ата');
	set @regionId = (select scope_identity());
	insert into customers.City (RegionID, CityName, CitySort, DisplayInPopup) values(@regionId, 'Алма-Ата', 100, 1);
	
	insert into customers.Region (CountryID, RegionName) values(@countryID, 'Астана');
	set @regionId = (select scope_identity());
	insert into customers.City (RegionID, CityName, CitySort, DisplayInPopup) values(@regionId, 'Астана', 100, 1);
		
	insert into customers.Region (CountryID, RegionName) values(@countryID, 'Южно-Казахстанская область');
	set @regionId = (select scope_identity());
	insert into customers.City (RegionID, CityName, CitySort, DisplayInPopup) values(@regionId, 'Шымкент', 0, 1);
	insert into customers.City (RegionID, CityName, CitySort, DisplayInPopup) values(@regionId, 'Туркестан', 0, 1);
	
	insert into customers.Region (CountryID, RegionName) values(@countryID, 'Актюбинская область');
	set @regionId = (select scope_identity());
	insert into customers.City (RegionID, CityName, CitySort, DisplayInPopup) values(@regionId, 'Актюбинск', 0, 1);

	insert into customers.Region (CountryID, RegionName) values(@countryID, 'Жамбыльская область');
	set @regionId = (select scope_identity());
	insert into customers.City (RegionID, CityName, CitySort, DisplayInPopup) values(@regionId, 'Тараз', 0, 1);


	insert into customers.Region (CountryID, RegionName) values(@countryID, 'Мангистауская область');
	set @regionId = (select scope_identity());
	insert into customers.City (RegionID, CityName, CitySort, DisplayInPopup) values(@regionId, 'Актау', 0, 1);

	insert into customers.Region (CountryID, RegionName) values(@countryID, 'Атырауская область');
	set @regionId = (select scope_identity());
	insert into customers.City (RegionID, CityName, CitySort, DisplayInPopup) values(@regionId, 'Атырау', 0, 1);
	
	insert into customers.Region (CountryID, RegionName) values(@countryID, 'Карагандинская область');
	set @regionId = (select scope_identity());
	insert into customers.City (RegionID, CityName, CitySort, DisplayInPopup) values(@regionId, 'Караганда', 0, 1);
	insert into customers.City (RegionID, CityName, CitySort, DisplayInPopup) values(@regionId, 'Темиртау', 0, 1);
	
	insert into customers.Region (CountryID, RegionName) values(@countryID, 'Кызылординская область');
	set @regionId = (select scope_identity());
	insert into customers.City (RegionID, CityName, CitySort, DisplayInPopup) values(@regionId, 'Кызылорда', 0, 1);


	insert into customers.Region (CountryID, RegionName) values(@countryID, 'Восточно-Казахстанская область');
	set @regionId = (select scope_identity());
	insert into customers.City (RegionID, CityName, CitySort, DisplayInPopup) values(@regionId, 'Семей', 0, 1);
	insert into customers.City (RegionID, CityName, CitySort, DisplayInPopup) values(@regionId, 'Усть-Каменогорск', 0, 1);


	insert into customers.Region (CountryID, RegionName) values(@countryID, 'Павлодарская область');
	set @regionId = (select scope_identity());
	insert into customers.City (RegionID, CityName, CitySort, DisplayInPopup) values(@regionId, 'Павлодар', 0, 1);

	insert into customers.Region (CountryID, RegionName) values(@countryID, 'Западно-Казахстанская область');
	set @regionId = (select scope_identity());
	insert into customers.City (RegionID, CityName, CitySort, DisplayInPopup) values(@regionId, 'Уральск', 0, 1);

	insert into customers.Region (CountryID, RegionName) values(@countryID, 'Костанайская область');
	set @regionId = (select scope_identity());
	insert into customers.City (RegionID, CityName, CitySort, DisplayInPopup) values(@regionId, 'Костанай', 0, 1);

	insert into customers.Region (CountryID, RegionName) values(@countryID, 'Северо-Казахстанская область');
	set @regionId = (select scope_identity());
	insert into customers.City (RegionID, CityName, CitySort, DisplayInPopup) values(@regionId, 'Петропавловск', 0, 1);

	insert into customers.Region (CountryID, RegionName) values(@countryID, 'Атырауская область');
	set @regionId = (select scope_identity());
	insert into customers.City (RegionID, CityName, CitySort, DisplayInPopup) values(@regionId, 'Атырау', 0, 1);

	insert into customers.Region (CountryID, RegionName) values(@countryID, 'Северо-Казахстанская область');
	set @regionId = (select scope_identity());
	insert into customers.City (RegionID, CityName, CitySort, DisplayInPopup) values(@regionId, 'Петропавловск', 0, 1);

	insert into customers.Region (CountryID, RegionName) values(@countryID, 'Акмолинская область');
	set @regionId = (select scope_identity());
	insert into customers.City (RegionID, CityName, CitySort, DisplayInPopup) values(@regionId, 'Кокшетау', 0, 1);
	
end

GO--

	Update Catalog.Tax set enabled=0 where taxid <> 18 
GO--

If ((Select Count(*) From [CMS].[StaticBlock] Where [Key] = 'OrderSuccessTop') = 0) 
Begin 
	Insert Into [CMS].[StaticBlock] ([Key],[InnerName],[Content],[Added],[Modified],[Enabled]) 
	Values ('OrderSuccessTop', 'Успешное оформление заказа верхний блок', '<div class="congrat">Поздравляем!</div> <div class="congrat-num">Ваш заказ принят под номером #ORDER_ID#</div>', GETDATE(), GETDATE(), 1)
End
else
begin
	Update [CMS].[StaticBlock] set [Content]='<div class="congrat">Поздравляем!</div> <div class="congrat-num">Ваш заказ принят под номером #ORDER_ID#</div>' where [Key] = 'OrderSuccessTop'
end


GO-- 