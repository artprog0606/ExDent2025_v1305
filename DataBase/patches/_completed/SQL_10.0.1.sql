if not exists (Select 1 From [Settings].[Localization] Where [ResourceKey] = 'Admin.ExportFeed.SettingsYandex.DontExportCurrency')
begin
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.ExportFeed.SettingsYandex.DontExportCurrency', 'Не выгружать блок валют - <currencies>')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.ExportFeed.SettingsYandex.DontExportCurrency', 'Dont export <currencies> block')
end
GO-- 

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Ссылка' WHERE [ResourceKey] = 'Admin.Js.Carousel.SynonymForURL' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'Link' WHERE [ResourceKey] = 'Admin.Js.Carousel.SynonymForURL' AND [LanguageId] = 2

GO--

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Ссылка для перехода' WHERE [ResourceKey] = 'Admin.Js.AddEditCarousel.NavigationURL' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'Link to go' WHERE [ResourceKey] = 'Admin.Js.AddEditCarousel.NavigationURL' AND [LanguageId] = 2

GO--

UPDATE [Order].[ShippingReplaceGeo]
	SET [OutRegionName] = 'Алматинская область'
	WHERE [Id] = 3

UPDATE [Order].[ShippingReplaceGeo]
	SET [Enabled] = 0
	WHERE [Id] = 4

UPDATE [Order].[ShippingReplaceGeo]
	SET [Enabled] = 0
	WHERE [Id] = 10

UPDATE [Order].[ShippingReplaceGeo]
	SET [Enabled] = 0
	WHERE [Id] = 11

GO--

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Подробное сравнение двух способов отправки писем приведено в инструкции' WHERE [ResourceKey] = 'Admin.SettingsMail.EmailSettings.DetailedComparison' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'A detailed comparison of the method of sending letters is given in the instructions' WHERE [ResourceKey] = 'Admin.SettingsMail.EmailSettings.DetailedComparison' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'E-mail, с которого будет производиться отправка. Требуется подтверждение: необходимо перейти по ссылке, отправленной на данный e-mail.' WHERE [ResourceKey] = 'Admin.SettingsMail.EmailSettings.SendingEmail' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'E-mail, from which the sending will be made. Confirmation is required: you must click on the link, sent to this e-mail.' WHERE [ResourceKey] = 'Admin.SettingsMail.EmailSettings.SendingEmail' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Если хотите изменить пароль, укажите его; если ничего не указано, пароль не изменится' WHERE [ResourceKey] = 'Admin.SettingsMail.EmailSettings.ChangePassword' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'If you want to change your password, enter it; if nothing is specified, the password will not change' WHERE [ResourceKey] = 'Admin.SettingsMail.EmailSettings.ChangePassword' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Определяет, использовать ли SSL соединения для подключения к почтовому серверу.' WHERE [ResourceKey] = 'Admin.SettingsMail.EmailSettings.SSL' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'Specifies, whether to use SSL connections to connect to the mail server.' WHERE [ResourceKey] = 'Admin.SettingsMail.EmailSettings.SSL' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Почта, SMS, уведомления' WHERE [ResourceKey] = 'Admin.Settings.Commonpage.Settingsmail' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'Mail, SMS, notifications' WHERE [ResourceKey] = 'Admin.Settings.Commonpage.Settingsmail' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Все лиды - доступны для редактирования все лиды.' WHERE [ResourceKey] = 'Admin.Settings.Users.Settings.AllLeadsAvailable' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'All leads - all leads are available for editing.' WHERE [ResourceKey] = 'Admin.Settings.Users.Settings.AllLeadsAvailable' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Какие задачи будет видеть и сможет редактировать менеджер (должны быть активны права модератора на задачи).<br><br> Все задачи - доступны для редактирования все задачи.<br><br> Назначенные задачи - только задачи, которые назначили менеджеру. Другие задачи не доступны для просмотра и редактирования.<br><br> Назначенные и свободные задачи - задачи, которые назначили менеджеру и задачи, на которые еще никого не назначили.' WHERE [ResourceKey] = 'Admin.Settings.Users.Settings.MangersTaskConstraintHint' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'What kind of tasks manager will see and will be able to edit (the rights of the moderator on Tasks must be active).<br><br> All tasks are available for editing all the tasks.<br><br> The appointed tasks are only the tasks that are assigned to the manager. Other tasks are not available for viewing and editing.<br><br> The appointed and free tasks are the tasks that have been assigned to the manager and the tasks to which no one has been appointed.' WHERE [ResourceKey] = 'Admin.Settings.Users.Settings.MangersTaskConstraintHint' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Каких покупателей будет видеть и сможет редактировать менеджер (должны быть активны права модератора на покупателей).<br><br> Все покупатели - доступны для редактирования все покупатели.<br><br> Назначенные покупатели - только покупатели, которых назначили менеджеру. Другие покупатели не доступны для просмотра и редактирования.<br><br> Назначенные и свободные покупатели - покупатели, которых назначили менеджеру и покупатели, которых еще никому не назначили.' WHERE [ResourceKey] = 'Admin.Settings.Users.Settings.ManagersCustomerConstraintHelpText' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'What kind of customers will see and be able to edit the manager (the permissions of the moderator should be active on the customers). <br> <br> All customers - all customers are available for editing. <br> <br> The assigned customers are only customers who have been assigned to the manager. Other customers are not available for viewing and editing. <br> <br> Assigned and free customers are customers who have been assigned to a manager and customers who have not been assigned to anyone yet.' WHERE [ResourceKey] = 'Admin.Settings.Users.Settings.ManagersCustomerConstraintHelpText' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Опция определяет, вызывать ли сразу переход к системе оплаты.' WHERE [ResourceKey] = 'Admin.SettingsCheckout.CheckoutCommon.SwitchPaymentSystem' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'The option determines whether to call the transition to the payment system immediately.' WHERE [ResourceKey] = 'Admin.SettingsCheckout.CheckoutCommon.SwitchPaymentSystem' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = ', то на последнем шаге оформления заказа клиента автоматически перенаправит на форму платежной системы.' WHERE [ResourceKey] = 'Admin.SettingsCheckout.CheckoutCommon.AutomaticallyRedirectToFormPaymentSystem' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = ', then at the last step of placing an order the customer will automatically redirect to the form of the payment system.' WHERE [ResourceKey] = 'Admin.SettingsCheckout.CheckoutCommon.AutomaticallyRedirectToFormPaymentSystem' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = ', то на последнем шаге оформления заказа клиенту покажется сообщение о завершении оплаты и кнопка "перейти к оплате".' WHERE [ResourceKey] = 'Admin.SettingsCheckout.CheckoutCommon.ButtonGoToPayment' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = ', then at the last step of placing an order the customer will see a message about completion of payment and a button "go to payment".' WHERE [ResourceKey] = 'Admin.SettingsCheckout.CheckoutCommon.ButtonGoToPayment' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Вывод номера корзины в клиентской части позволяет Вам при общении с клиентом по телефону, или онлайн-консультанту, получать максимально подробную информацию о клиенте и его действиях на сайте. Для вывода информации о клиенте введите номер корзины в строке поиска в панели администрирования<br/><br/>Подробнее: <br/> <a href="https://www.advantshop.net/help/pages/client-tracking" target="_blank" >Номер корзины (Client Tracking)</a> ' WHERE [ResourceKey] = 'Admin.Settings.Checkout.ShowShoppingCartNumberHint' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'Displaying the basket number in the client part allows you to receive the most detailed information about the client and his actions on the site when communicating with a client by phone, or an online consultant. To display customer information, enter the shopping cart number in the search bar in the administration panel. <br/> <br/> More details: <br/> <a href="https://www.advantshop.net/help/pages/client-tracking" target="_blank"> Trash number (Client Tracking)</a>' WHERE [ResourceKey] = 'Admin.Settings.Checkout.ShowShoppingCartNumberHint' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Опция определяет,' WHERE [ResourceKey] = 'Admin.SettingsCheckout.CheckoutCommon.OptionSpecifies' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'The option specifies,' WHERE [ResourceKey] = 'Admin.SettingsCheckout.CheckoutCommon.OptionSpecifies' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Опция определяет, включить или выключить возможность покупки и использования подарочного сертификата.<br />Для возможности использования подарочного сертифката при оформлении заказа, обязательно включить дополнительно настройку "Отображать поле ввода купона или сертификата" (то что в кавычках выделить жирным)' WHERE [ResourceKey] = 'Admin.Settings.Checkout.EnableGiftCertificateServiceInfo' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'The option determines, whether to enable or disable the ability to purchase and use a gift certificate. <br /> To be able to use a gift certificate when placing an order, be sure to additionally enable the option "Display coupon or certificate input field" (what should be highlighted in quotes)' WHERE [ResourceKey] = 'Admin.Settings.Checkout.EnableGiftCertificateServiceInfo' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Опция определяет, включить или выключить возможность использования купона и подарочного сертификата при оформлении заказа <br />Для возможности использования подарочного сертифката при оформлении заказа, обязательно включить дополнительно настройку "Разрешить использование подарочных сертификатов" (то что в кавычках выделить жирным)' WHERE [ResourceKey] = 'Admin.Settings.Checkout.DisplayPromoTextboxInfo' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'The option determines, whether to enable or disable the ability to use a coupon and a gift certificate when placing an order. <br /> To be able to use a gift certificate when placing an order, be sure to additionally enable the "Allow the use of gift certificates" setting (what should be highlighted in quotes)' WHERE [ResourceKey] = 'Admin.Settings.Checkout.DisplayPromoTextboxInfo' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Значение, ниже которого клиент не сможет указать номинал сертификата ' WHERE [ResourceKey] = 'Admin.Settings.Checkout.MinimalPriceCertificateHint' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'A value, below which the client will not be able to indicate the face value of the certificate ' WHERE [ResourceKey] = 'Admin.Settings.Checkout.MinimalPriceCertificateHint' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Значение, выше которого клиент не сможет указать номинал сертификата ' WHERE [ResourceKey] = 'Admin.Settings.Checkout.MaximalPriceCertificateHint' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'A value, above which the client cannot specify the nominal value of the certificate ' WHERE [ResourceKey] = 'Admin.Settings.Checkout.MaximalPriceCertificateHint' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = ', при добавлении в корзину товара с подарками количество подарков будет соответствовать количеству добавленных товаров,' WHERE [ResourceKey] = 'Admin.SettingsCheckout.CheckoutCommon.AddingProductsToBasketWithGifts' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = ', when adding products to the basket with gifts the number of gifts will correspond to the number of goods added,' WHERE [ResourceKey] = 'Admin.SettingsCheckout.CheckoutCommon.AddingProductsToBasketWithGifts' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Опция определяет, выводить или нет строчку со статусом заказа на распечатке заказа.' WHERE [ResourceKey] = 'Admin.SettingsCheckout.CheckoutCommon.DisplayLineWithStatus' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'The option determines, whether or not to display a line with order status on the order printout.' WHERE [ResourceKey] = 'Admin.SettingsCheckout.CheckoutCommon.DisplayLineWithStatus' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Опция определяет, отображать или нет карту по адресу клиента на распечатке заказа.' WHERE [ResourceKey] = 'Admin.SettingsCommon.CheckoutCommon.ShowAdressOnOrderPrint' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'The option determines, whether or not the card is displayed at the customer address on the order printout.' WHERE [ResourceKey] = 'Admin.SettingsCommon.CheckoutCommon.ShowAdressOnOrderPrint' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'На этой странице Вы можете настроить действие, которое посетитель сможет сделать после оформления заказа' WHERE [ResourceKey] = 'Admin.Settings.Checkout.ThankYouPageTitleAbout' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'On this page you can configure the action that the visitor can take after placing an order.' WHERE [ResourceKey] = 'Admin.Settings.Checkout.ThankYouPageTitleAbout' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Страница благодарности показывается, если посетитель совершил целевое действие (оставил заявку или оформил заказ). Когда мы благодарим посетителя за заказ, самое время продолжить строить отношения с этим покупателем и предложить ему сделать еще какое-либо целевое действие.' WHERE [ResourceKey] = 'Admin.Settings.Checkout.ThankYouPageTitleInfo' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'The thank you page is shown if the visitor has performed the target action (left a request or placed an order).When we thank a visitor for an order, it`s time to continue building relationships with this customer and invite him to take some other targeted action.' WHERE [ResourceKey] = 'Admin.Settings.Checkout.ThankYouPageTitleInfo' AND [LanguageId] = 2

GO--


UPDATE [Settings].[Localization] SET [ResourceValue] = 'API ключ - это параметр, необходимый для обеспечения возможности подключения сторонних сервисов к магазину.' WHERE [ResourceKey] = 'Admin.SettingsApi.Index.ApiKey' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'API key - this parameter, is necessary to enable the connection of third-party services to the store.' WHERE [ResourceKey] = 'Admin.SettingsApi.Index.ApiKey' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Обратите внимание, если вы повторно сгенерируете ключ, все ссылки, в которых он используется, так же необходимо обновить, включая те, что были указанны ранее в сторонних сервисах.' WHERE [ResourceKey] = 'Admin.SettingsApi.Index.NoteIfYouRegenerate' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'Note that if you re-generate the key, all the links, in which it is used, also need to be updated, including those that were previously listed in third-party services.' WHERE [ResourceKey] = 'Admin.SettingsApi.Index.NoteIfYouRegenerate' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Отключить списание товаров в магазине (весь учет наличия ведется в 1с)' WHERE [ResourceKey] = 'Admin.Settings.API.1CDisableProductsDecremention' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'To turn off goods write-off in shop (all account of existence is kept in 1s)' WHERE [ResourceKey] = 'Admin.Settings.API.1CDisableProductsDecremention' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Выбираете из списка проект, который будет автоматически проставляться в задачу' WHERE [ResourceKey] = 'Admin.Settings.Tasks.DefaultTaskGroupHint' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'Choose from the list a project that will be automatically added to the task ' WHERE [ResourceKey] = 'Admin.Settings.Tasks.DefaultTaskGroupHint' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Выбираете действие при новом звонке, либо не создавать лид, либо создавать в определённый список лидов' WHERE [ResourceKey] = 'Admin.SettingsTelephony.CallsSalesFunnelHint' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'Choose an action on a new call, either not create a lead, or create a specific list of leads' WHERE [ResourceKey] = 'Admin.SettingsTelephony.CallsSalesFunnelHint' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Выбираете оператора ip телефонии и далее настраиваете в соответствии с инструкцией того или иного оператора<br/><br/> Sipuni<br/>Подробнее: <br/> <a href="https://www.advantshop.net/help/pages/phone-sipuni" target="_blank" >Телефония. Sipuni</a> <br/><br/> Манго Телеком <br/> Подробнее: <br/> <a href="https://www.advantshop.net/help/pages/phone-mango" target="_blank">Телефония. Манго Телеком</a>' WHERE [ResourceKey] = 'Admin.Settings.Telephony.OperatorHint' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'Choose an ip telephony operator and then set it up in accordance with the instructions of one or another operator<br/><br/> Sipuni <br/>  Details: <br/> <a href = "https://www.advantshop.net/help/pages/ phone-sipuni "target =" _blank "> Telephony. Sipuni </a> <br/><br/>Telephony. Mango Telecom <br/> Details: <br/> <a href="https://www.advantshop.net/help/pages/phone-mango" target="_blank">Telephony. Mango Telecom</a>' WHERE [ResourceKey] = 'Admin.Settings.Telephony.OperatorHint' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Настройка включает отображение виджетов коммуникаций.<br/><br/>Подробнее:<br> <a href="https://www.advantshop.net/help/pages/vidzhety-kommunikatsii" target="_blank">Виджеты коммуникаций.</a>' WHERE [ResourceKey] = 'Admin.Settings.Social.SocialWidget.ShowCommunicationWidget.HelpText' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'The setting includes displaying communication widgets. <br/> <br/> More details: <br> <a href="https://www.advantshop.net/help/pages/vidzhety-kommunikatsii" target="_blank"> Communication widgets. </a>' WHERE [ResourceKey] = 'Admin.Settings.Social.SocialWidget.ShowCommunicationWidget.HelpText' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = '1) Активируйте виджет коммуникаций - поставьте галочку в поле "Показывать виджет коммуникаций" <br/>2) Активируйте виджеты, которые планируете выводить (для этого поставьте галочки в полях "Показывать ВКонтакте") <br/>3) И подключите виджеты согласно инструкции.<br/><br/>Подробнее:<br> <a href="https://www.advantshop.net/help/pages/vidzhety-kommunikatsii" target="_blank">Виджеты коммуникаций.</a>' WHERE [ResourceKey] = 'Admin.Settings.Social.SocialWidget.ShowVkontakte.HelpText' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = '1) Activate the communications widget - check the box "Show communications widget" <br/> 2) Activate the widgets that you plan to display (for this, check the boxes "Show VKontakte") <br/> 3) And connect the widgets according to the instructions . <br/> <br/> More details: <br> <a href="https://www.advantshop.net/help/pages/vidzhety-kommunikatsii" target="_blank"> Communication widgets. </a>' WHERE [ResourceKey] = 'Admin.Settings.Social.SocialWidget.ShowVkontakte.HelpText' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = '1) Активируйте виджет коммуникаций - поставьте галочку в поле "Показывать виджет коммуникаций" <br/>2) Активируйте виджеты, которые планируете выводить (для этого поставьте галочки в полях "Показывать JivoSite") <br/>3) И подключите виджеты согласно инструкции.<br/><br/>Подробнее:<br> <a href="https://www.advantshop.net/help/pages/vidzhety-kommunikatsii" target="_blank">Виджеты коммуникаций.</a>' WHERE [ResourceKey] = 'Admin.Settings.Social.SocialWidget.ShowJivosite.HelpText' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = '1) Activate the communication widget - check the box "Show communication widget" <br/> 2) Activate the widgets that you plan to display (for this, check the boxes "Show JivoSite") <br/> 3) And connect the widgets according to the instructions . <br/> <br/> More information: <br> <a href="https://www.advantshop.net/help/pages/vidzhety-kommunikatsii" target="_blank"> Communication widgets. </a>' WHERE [ResourceKey] = 'Admin.Settings.Social.SocialWidget.ShowJivosite.HelpText' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = '1) Активируйте виджет коммуникаций - поставьте галочку в поле "Показывать виджет коммуникаций" <br/>2) Активируйте виджеты, которые планируете выводить (для этого поставьте галочки в полях "Показывать Callback") <br/>3) И подключите виджеты согласно инструкции.<br/><br/>Подробнее:<br>  <a href="https://www.advantshop.net/help/pages/vidzhety-kommunikatsii" target="_blank">Виджеты коммуникаций.</a>' WHERE [ResourceKey] = 'Admin.Settings.Social.SocialWidget.ShowCallback.HelpText' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = ') Activate the communication widget - check the box "Show communication widget" <br/> 2) Activate the widgets that you plan to display (for this, check the boxes "Show Callback") <br/> 3) And connect the widgets according to the instructions . <br/> <br/> More information: <br> <a href="https://www.advantshop.net/help/pages/vidzhety-kommunikatsii" target="_blank"> Communication widgets.</a>' WHERE [ResourceKey] = 'Admin.Settings.Social.SocialWidget.ShowCallback.HelpText' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = '1) Активируйте виджет коммуникаций - поставьте галочку в поле "Показывать виджет коммуникаций" <br/>2) Активируйте виджеты, которые планируете выводить (для этого поставьте галочки в полях "Показывать WhatsApp") <br/>3) И подключите виджеты согласно инструкции.<br/><br/>Подробнее:<br> <a href="https://www.advantshop.net/help/pages/vidzhety-kommunikatsii" target="_blank">Виджеты коммуникаций.</a>' WHERE [ResourceKey] = 'Admin.Settings.Social.SocialWidget.ShowWhatsApp.HelpText' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = '1) Activate the communication widget - check the box "Show communication widget" <br/> 2) Activate the widgets that you plan to display (for this, check the boxes "Show WhatsApp") <br/> 3) And connect the widgets according to the instructions . <br/> <br/> More information: <br> <a href="https://www.advantshop.net/help/pages/vidzhety-kommunikatsii" target="_blank"> Communication widgets. </a>' WHERE [ResourceKey] = 'Admin.Settings.Social.SocialWidget.ShowWhatsApp.HelpText' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = '1) Активируйте виджет коммуникаций - поставьте галочку в поле "Показывать виджет коммуникаций" <br/>2) Активируйте виджеты, которые планируете выводить (для этого поставьте галочки в полях "Показывать Viber") <br/>3) И подключите виджеты согласно инструкции.<br/><br/>Подробнее:<br>  <a href="https://www.advantshop.net/help/pages/vidzhety-kommunikatsii" target="_blank">Виджеты коммуникаций.</a>' WHERE [ResourceKey] = 'Admin.Settings.Social.SocialWidget.ShowViber.HelpText' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = '1) Activate the communications widget - check the box "Show communications widget" <br/> 2) Activate the widgets that you plan to display (for this, check the boxes "Show Viber") <br/> 3) And connect the widgets according to the instructions . <br/> <br/> More information: <br> <a href="https://www.advantshop.net/help/pages/vidzhety-kommunikatsii" target="_blank"> Communication widgets. </a>' WHERE [ResourceKey] = 'Admin.Settings.Social.SocialWidget.ShowViber.HelpText' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = '1) Активируйте виджет коммуникаций - поставьте галочку в поле "Показывать виджет коммуникаций" <br/>2) Активируйте виджеты, которые планируете выводить (для этого поставьте галочки в полях "Показывать Одноклассники") <br/>3) И подключите виджеты согласно инструкции.<br/><br/>Подробнее:<br> <a href="https://www.advantshop.net/help/pages/vidzhety-kommunikatsii" target="_blank">Виджеты коммуникаций.</a>' WHERE [ResourceKey] = 'Admin.Settings.Social.SocialWidget.ShowOdnoklassniki.HelpText' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = '1) Activate the communications widget - check the box "Show communications widget" <br/> 2) Activate the widgets that you plan to display (for this, check the boxes "Show Odnoklassniki") <br/> 3) And connect the widgets according to the instructions . <br/> <br/> More information: <br> <a href="https://www.advantshop.net/help/pages/vidzhety-kommunikatsii" target="_blank"> Communication widgets. </a>' WHERE [ResourceKey] = 'Admin.Settings.Social.SocialWidget.ShowOdnoklassniki.HelpText' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = '1) Активируйте виджет коммуникаций - поставьте галочку в поле "Показывать виджет коммуникаций" <br/>2) Активируйте виджеты, которые планируете выводить (для этого поставьте галочки в полях "Показывать Telegram") <br/>3) И подключите виджеты согласно инструкции.<br/><br/>Подробнее:<br> <a href="https://www.advantshop.net/help/pages/vidzhety-kommunikatsii" target="_blank">Виджеты коммуникаций.</a>' WHERE [ResourceKey] = 'Admin.Settings.Social.SocialWidget.ShowTelegram.HelpText' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = '1) Activate the communications widget - check the box "Show communications widget" <br/> 2) Activate the widgets that you plan to display (for this, check the boxes "Show Telegram") <br/> 3) And connect the widgets according to the instructions . <br/> <br/> More information: <br> <a href="https://www.advantshop.net/help/pages/vidzhety-kommunikatsii" target="_blank"> Communication widgets. </a>' WHERE [ResourceKey] = 'Admin.Settings.Social.SocialWidget.ShowTelegram.HelpText' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Можно подключить дополнительно каналы связи, которых нет в списке.<br/><br/>Подробнее:<br> <a href="https://www.advantshop.net/help/pages/vidzhety-kommunikatsii#a9" target="_blank">Свой виджет</a>' WHERE [ResourceKey] = 'Admin.Settings.Social.SocialWidget.CustomWidgetTitleNote' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'You can connect additional communication channels that are not in the list. <br/> <br/> More details: <br> <a href = "https://www.advantshop.net/help/pages/vidzhety-kommunikatsii#a9" target="_blank"> Custom widget</a>' WHERE [ResourceKey] = 'Admin.Settings.Social.SocialWidget.CustomWidgetTitleNote' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Купоны могут применять только покупатели группы "Обычный покупатель", для остальных покупателей купоны учитываться не будут' WHERE [ResourceKey] = 'Admin.Coupons.Index.Hint' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'Coupons can be used by the "Usual customer" group users only,<br> coupons will not be taken into consideration for other customers' WHERE [ResourceKey] = 'Admin.Coupons.Index.Hint' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Символ, который должен указываться в файле CSV при перечислении свойств ' WHERE [ResourceKey] = 'Admin.Import.ImportLeads.PropertySeparatorHint' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'The character to be specified in the CSV file when listing properties ' WHERE [ResourceKey] = 'Admin.Import.ImportLeads.PropertySeparatorHint' AND [LanguageId] = 2

UPDATE [Settings].[Localization] SET [ResourceValue] = 'Вы так же можете изменить внешний вид номера телефона и указать 2 и более номера' WHERE [ResourceKey] = 'Admin.Settings.Common.Common.YouCanSpecify2OrMoreNumbers' AND [LanguageId] = 1
UPDATE [Settings].[Localization] SET [ResourceValue] = 'You can also change the appearance of the phone number and specify 2 or more numbers' WHERE [ResourceKey] = 'Admin.Settings.Common.Common.YouCanSpecify2OrMoreNumbers' AND [LanguageId] = 2


GO--


if not exists (Select 1 From [Settings].[Localization] Where [ResourceKey] = 'Admin.PaymentMeythods.Alfabank.GatewayCountry')
begin
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.PaymentMeythods.Alfabank.GatewayCountry', 'Шлюз api')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.PaymentMeythods.Alfabank.GatewayCountry', 'Api gateway')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.PaymentMethods.Alfabank.GatewayCountry.DescriptionRu', 'Россия - для клиентов из России (Договор заключен с российским подразделением банка).')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.PaymentMethods.Alfabank.GatewayCountry.DescriptionRu', 'Russia')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (1,'Admin.PaymentMethods.Alfabank.GatewayCountry.DescriptionKz', 'Казахстан - для клиентов из Казахстана (Договор заключен с казахским подразделением банка).')
INSERT INTO [Settings].[Localization] ([LanguageId],[ResourceKey],[ResourceValue]) VALUES (2,'Admin.PaymentMethods.Alfabank.GatewayCountry.DescriptionKz', 'Kazakhstan')
end
GO-- 

UPDATE [Settings].[InternalSettings] SET [settingValue] = '10.0.1' WHERE [settingKey] = 'db_version'