using AdvantShop.Core.Common.Attributes;
using EShopMode = AdvantShop.Core.ModeConfigService.Modes;

namespace AdvantShop.Track
{
    public enum ETrackEvent
    {
        None = 0,

        #region Dashboard

        [TrackEvent(EShopMode.TrialMode, SendOnce = true)]
        Dashboard_SkipDashboard = 1,

        [TrackEvent(EShopMode.TrialMode, SendOnce = true)]
        Dashboard_ProductDone = 2,

        [TrackEvent(EShopMode.TrialMode, SendOnce = true)]
        Dashboard_DesignDone = 3,

        [TrackEvent(EShopMode.TrialMode, SendOnce = true)]
        Dashboard_LogoDone = 4,

        [TrackEvent(EShopMode.TrialMode, SendOnce = true)]
        Dashboard_DomenDone = 5,

        [TrackEvent(EShopMode.TrialMode, SendOnce = true)]
        Dashboard_StaticPagesDone = 6,

        [TrackEvent(EShopMode.TrialMode, SendOnce = true)]
        Dashboard_SupportDone = 7,

        [TrackEvent(EShopMode.TrialMode, SendOnce = true)]
        Dashboard_ClickProductButton = 8,

        [TrackEvent(EShopMode.TrialMode, SendOnce = true)]
        Dashboard_ClickDesignButton = 9,

        [TrackEvent(EShopMode.TrialMode, SendOnce = true)]
        Dashboard_ClickBuyDomainButton = 10,

        [TrackEvent(EShopMode.TrialMode, SendOnce = true)]
        Dashboard_ClickBindDomainButton = 11,

        [TrackEvent(EShopMode.TrialMode, SendOnce = true)]
        Dashboard_ClickProductVideo = 12,

        [TrackEvent(EShopMode.TrialMode, SendOnce = true)]
        Dashboard_ClickDesignVideo = 13,

        #endregion

        #region Landings

        [TrackEvent(EShopMode.TrialMode, SendOnce = true)]
        Landings_CreateFunnelShow = 14,

        [TrackEvent(EShopMode.TrialMode, SendOnce = true)]
        Landings_CreateEmptyFunnel_Step_1 = 15,

        [TrackEvent(EShopMode.TrialMode, SendOnce = true)]
        Landings_CreateEmptyFunnel_Step_2 = 16,

        [TrackEvent(EShopMode.TrialMode, SendOnce = true)]
        Landings_CreateEmptyFunnel_Step_3 = 17,

        [TrackEvent(EShopMode.TrialMode, SendOnce = true)]
        Landings_CreateFreeShippingFunnel_Step0 = 18,

        [TrackEvent(EShopMode.TrialMode, SendOnce = true)]
        Landings_CreateFreeShippingFunnel_Step1 = 19,

        [TrackEvent(EShopMode.TrialMode, SendOnce = true)]
        Landings_CreateFreeShippingFunnel_Step2 = 20,

        [TrackEvent(EShopMode.TrialMode, SendOnce = true)]
        Landings_CreateFreeShippingFunnel_Step3 = 21,

        [TrackEvent(EShopMode.TrialMode, SendOnce = true)]
        Landings_CreateFreeShippingFunnel_Step4 = 22,

        [TrackEvent(EShopMode.TrialMode, SendOnce = true)]
        Landings_CreateFreeShippingFunnel_Step5 = 23,

        [TrackEvent(EShopMode.TrialMode, SendOnce = true)]
        Landings_CreateFreeShippingFunnel_StepFinal = 24,

        [TrackEvent(EShopMode.TrialMode, SendOnce = true)]
        Landings_CreateArticleFunnel_Step1 = 25,

        [TrackEvent(EShopMode.TrialMode, SendOnce = true)]
        Landings_CreateArticleFunnel_Step2 = 26,

        [TrackEvent(EShopMode.TrialMode, SendOnce = true)]
        Landings_CreateArticleFunnel_Step3 = 27,

        #endregion

        #region Trial

        [TrackEvent(EShopMode.TrialMode, SendOnce = true)]
        Trial_FillUserData = 28,

        [TrackEvent(EShopMode.TrialMode, SendOnce = true)]
        Trial_UserDataFormShown = 29,

        [TrackEvent(EShopMode.TrialMode, SendOnce = true)]
        Trial_ClearData = 30,

        [TrackEvent(EShopMode.TrialMode, SendOnce = true)]
        Trial_ChangeAboutData = 31,

        [TrackEvent(EShopMode.TrialMode, SendOnce = true)]
        Trial_VisitCRM = 32,

        [TrackEvent(EShopMode.TrialMode, SendOnce = true)]
        Trial_VisitTasks = 33,

        [TrackEvent(EShopMode.TrialMode, SendOnce = true)]
        Trial_ImportYML = 34,

        [TrackEvent(EShopMode.TrialMode, SendOnce = true)]
        Trial_VisitClientSide = 35,

        [TrackEvent(EShopMode.TrialMode, SendOnce = true)]
        Trial_ChangeDesignTransformer = 36,

        [TrackEvent(EShopMode.TrialMode, SendOnce = true)]
        Trial_PreviewDesignTemplate = 37,

        [TrackEvent(EShopMode.TrialMode, SendOnce = true)]
        Trial_VisitMobileVersion = 38,

        // Зашел в n-ый день триала
        [TrackEvent(EShopMode.TrialMode, EventKey = "Trial.DailyVisit")]
        Trial_DailyVisit = 39,

        #endregion

        #region Core

        #region Orders

        // Создан заказ из админки
        [TrackEvent("Product.Core.Orders.OrderCreated.AdminArea")]
        Core_Orders_OrderCreated_AdminArea = 40,

        // Создан заказ из клиентки магазина (desktop)
        [TrackEvent("Product.Core.Orders.OrderCreated.Desktop")]
        Core_Orders_OrderCreated_Desktop = 41,

        // Создан заказ из клиентки магазина (mobile version)
        [TrackEvent("Product.Core.Orders.OrderCreated.Mobile")]
        Core_Orders_OrderCreated_Mobile = 42,

        // Создан заказ из воронки
        [TrackEvent("Product.Core.Orders.OrderCreated.SalesFunnel")]
        Core_Orders_OrderCreated_SalesFunnel = 43,

        // Изменен статус заказа
        [TrackEvent("Product.Core.Orders.OrderStatusChanged")]
        Core_Orders_OrderStatusChanged = 44,

        // Заказ изменен
        [TrackEvent("Product.Core.Orders.EditOrder")]
        Core_Orders_EditOrder = 45,

        // Добавлен товар в заказ админом
        [TrackEvent("Product.Core.Orders.AddOrderItem")]
        Core_Orders_OrderItemAdded = 46,

        // Выполнен экспорт заказов
        [TrackEvent("Product.Core.Orders.ExportOrders")]
        Core_Orders_ExportOrders = 47,

        // Добавлен комментарий к заказу в обсуждения
        [TrackEvent("Product.Core.Orders.AddComment.Discussion")]
        Core_Orders_AddComment_Discussion = 48,

        // Добавлен комментарий в ленту события из заказа
        [TrackEvent("Product.Core.Orders.AddComment.Events")]
        Core_Orders_AddComment_Events = 49,

        // Заказ переведен в статус оплачен
        [TrackEvent("Product.Core.Orders.OrderPayed.AdminArea")]
        Core_Orders_OrderPayed_AdminArea = 50,

        // Печать заказа
        [TrackEvent("Product.Core.Orders.PrintOrder.AdminArea")]
        Core_Orders_PrintOrder_AdminArea = 51,

        // Экспорт заказа в Excel
        [TrackEvent("Product.Core.Orders.ExportExcel")]
        Core_Orders_ExportExcel = 52,

        // Заказ подтвержден, разрешить оплату
        [TrackEvent("Product.Core.Orders.OrderConfirmedByManager")]
        Core_Orders_OrderConfirmedByManager = 53,

        // Заказ передан в СДЕК (другие службы доставки) на каждую службу отдельный event
        [TrackEvent("Product.Core.Orders.OrderSentToDeliveryService")]
        Core_Orders_OrderSentToDeliveryService = 54,

        // Добавлен новый статус заказа
        [TrackEvent("Product.Core.Orders.OrderStatusCreated")]
        Core_Orders_OrderStatusCreated = 55,

        // Отправить письмо из заказа
        [TrackEvent("Product.Core.Orders.SendLetterToCustomer")]
        Core_Orders_SendLetterToCustomer = 56,

        // Позвонить из заказа
        [TrackEvent("Product.Core.Orders.CallCustomer")]
        Core_Orders_CallCustomer = 57,

        // Отправить СМС из заказа
        [TrackEvent("Product.Core.Orders.SendSmsToCustomer")]
        Core_Orders_SendSmsToCustomer = 58,

        #endregion

        #region Customers

        // Создан покупатель
        [TrackEvent("Product.Core.Customers.CustomerCreated")]
        Core_Customers_CustomerCreated = 59,

        // Добавлен комментарий в ленту события из покупателя
        [TrackEvent("Product.Core.Customers.AddComment.Events")]
        Core_Customers_AddComment_Events = 60,

        // Отредактированы данные покупателя
        [TrackEvent("Product.Core.Customers.EditCustomer")]
        Core_Customers_EditCustomer = 61,

        // Выставлен статус (VIP, Bad)
        [TrackEvent("Product.Core.Customers.StatusChanged")]
        Core_Customers_StatusChanged = 62,

        // Отправить письмо из покупателя
        [TrackEvent("Product.Core.Customers.SendLetterToCustomer")]
        Core_Customers_SendLetterToCustomer = 63,

        // Позвонить из покупателя
        [TrackEvent("Product.Core.Customers.CallCustomer")]
        Core_Customers_CallCustomer = 64,

        // Отправить СМС из покупателя
        [TrackEvent("Product.Core.Customers.SendSmsToCustomer")]
        Core_Customers_SendSmsToCustomer = 65,

        // Добавлена группа покупателей
        [TrackEvent("Product.Core.Customers.CustomerGroupCreated")]
        Core_Customers_CustomerGroupCreated = 66,

        // Добавлен сегмент
        [TrackEvent("Product.Core.Customers.CustomerSegmentCreated")]
        Core_Customers_CustomerSegmentCreated = 67,

        // Редактирование сегмента
        [TrackEvent("Product.Core.Customers.EditCustomerSegment")]
        Core_Customers_EditCustomerSegment = 68,

        // Массовая email рассылка по сегменту
        [TrackEvent("Product.Core.Customers.BulkEmailSendingBySegment")]
        Core_Customers_BulkEmailSendingBySegment = 69,

        // Массовая смс рассылка по сегменту
        [TrackEvent("Product.Core.Customers.BulkSmsSendingBySegment")]
        Core_Customers_BulkSmsSendingBySegment = 70,

        // Экспорт покупателей входящих в сегмент в CSV
        [TrackEvent("Product.Core.Customers.ExportCustomersBySegment")]
        Core_Customers_ExportCustomersBySegment = 71,

        // Импортированы покупатели
        [TrackEvent("Product.Core.Customers.ImportCustomers")]
        Core_Customers_ImportCustomers = 72,

        // Покупателю добавлен тег
        [TrackEvent("Product.Core.Customers.AddTagToCustomer")]
        Core_Customers_AddTagToCustomer = 73,

        // Создан тег покупателя
        [TrackEvent("Product.Core.Customers.TagCreated")]
        Core_Customers_TagCreated = 74,

        #endregion

        #region Categories

        // Добавлена категория
        [TrackEvent("Product.Core.Categories.CategoryCreated")]
        Core_Categories_CategoryCreated = 75,

        // Изменена категория
        [TrackEvent("Product.Core.Categories.EditCategory")]
        Core_Categories_EditCategory = 76,

        // Добавлена категория списком
        [TrackEvent("Product.Core.Categories.CategoriesListCreated")]
        Core_Categories_CategoriesListCreated = 77,

        // Категории задан тег
        [TrackEvent("Product.Core.Categories.AddTagToCategory")]
        Core_Categories_AddTagToCategory = 78,

        // Категории задана группа свойств
        [TrackEvent("Product.Core.Categories.AddPropertyGroupToCategory")]
        Core_Categories_AddPropertyGroupToCategory = 79,

        // Категории задана автоподборка рекомендуемых товаров
        [TrackEvent("Product.Core.Categories.SetProductRecommendations")]
        Core_Categories_SetProductRecommendations = 80,

        // Экспортированы категории
        [TrackEvent("Product.Core.Categories.ExportCategories")]
        Core_Categories_ExportCategories = 81,

        // Импортированы категории
        [TrackEvent("Product.Core.Categories.ImportCategories")]
        Core_Categories_ImportCategories = 82,

        #endregion

        #region Products

        // Добавлен товар
        [TrackEvent("Product.Core.Products.ProductCreated")]
        Core_Products_ProductCreated = 83,

        // Товар изменен
        [TrackEvent("Product.Core.Products.EditProduct")]
        Core_Products_EditProduct = 84,

        // Добавлен товар списком
        [TrackEvent("Product.Core.Products.ProductListCreated")]
        Core_Products_ProductListCreated = 85,

        // Создана копия товара
        [TrackEvent("Product.Core.Products.ProductCopyCreated")]
        Core_Products_ProductCopyCreated = 86,

        // Товару добавлен тег
        [TrackEvent("Product.Core.Products.AddTagToProduct")]
        Core_Products_AddTagToProduct = 87,

        // Добавлен новый offer
        [TrackEvent("Product.Core.Products.AddOffer")]
        Core_Products_AddOffer = 88,

        // Добавлена фотография по ссылке
        [TrackEvent("Product.Core.Products.AddPhoto.ByUrl")]
        Core_Products_AddPhoto_ByUrl = 89,

        // Добавлена фотография с компа
        [TrackEvent("Product.Core.Products.AddPhoto.File")]
        Core_Products_AddPhoto_File = 90,

        // Добавлена фотография через bing
        [TrackEvent("Product.Core.Products.AddPhoto.Bing")]
        Core_Products_AddPhoto_Bing = 91,

        // Добавлено свойство товара
        [TrackEvent("Product.Core.Products.PropertyCreated")]
        Core_Products_PropertyCreated = 92,

        // Добавлен производитель
        [TrackEvent("Product.Core.Products.BrandCreated")]
        Core_Products_BrandCreated = 93,

        // Добавлен цвет
        [TrackEvent("Product.Core.Products.ColorCreated")]
        Core_Products_ColorCreated = 94,

        // Добавлен размер
        [TrackEvent("Product.Core.Products.SizeCreated")]
        Core_Products_SizeCreated = 95,

        // Добавлен тег
        [TrackEvent("Product.Core.Products.TagCreated")]
        Core_Products_TagCreated = 96,

        // Добавлен новый отзыв
        [TrackEvent("Product.Core.Products.AddReview")]
        Core_Products_AddReview = 97,

        // Изменены цены через ценорегулирование
        [TrackEvent("Product.Core.Products.ChangePricesByPriceRegulation")]
        Core_Products_ChangePricesByPriceRegulation = 98,

        // Импортированы товары
        [TrackEvent("Product.Core.Products.ImportProducts.Csv")]
        Core_Products_ImportProducts_Csv = 99,

        #endregion

        #region Leads

        // Создан лид из админки
        [TrackEvent("Product.Core.Leads.LeadCreated.AdminArea")]
        Core_Leads_LeadCreated_AdminArea = 100,

        // Создан лид из клиентки магазина (desktop)
        [TrackEvent("Product.Core.Leads.LeadCreated.Desktop")]
        Core_Leads_LeadCreated_Desktop = 101,

        // Создан лид из клиентки магазина (mobile version)
        [TrackEvent("Product.Core.Leads.LeadCreated.Mobile")]
        Core_Leads_LeadCreated_Mobile = 102,

        // Создан лид из воронки
        [TrackEvent("Product.Core.Leads.LeadCreated.SalesFunnel")]
        Core_Leads_LeadCreated_SalesFunnel = 103,

        // Создан лид из соцсети (какой)
        [TrackEvent("Product.Core.Leads.LeadCreated.SocialNetwork")]
        Core_Leads_LeadCreated_SocialNetwork = 104,

        // Создан лид из звонка
        [TrackEvent("Product.Core.Leads.LeadCreated.Call")]
        Core_Leads_LeadCreated_Call = 105,

        // Создан лид из API
        [TrackEvent("Product.Core.Leads.LeadCreated.Api")]
        Core_Leads_LeadCreated_Api = 106,

        // Изменен этап лида
        [TrackEvent("Product.Core.Leads.DealStatusChanged")]
        Core_Leads_DealStatusChanged = 107,

        // Изменен лид
        [TrackEvent("Product.Core.Leads.EditLead")]
        Core_Leads_EditLead = 108,

        // Добавлен комментарий в ленту события из лида
        [TrackEvent("Product.Core.Leads.AddComment.Events")]
        Core_Leads_AddComment_Events = 109,

        // Массовая email рассылка по лидам
        [TrackEvent("Product.Core.Leads.BulkEmailSending")]
        Core_Leads_BulkEmailSending = 110,

        // Массовая смс рассылка по лидам
        [TrackEvent("Product.Core.Leads.BulkSmsSending")]
        Core_Leads_BulkSmsSending = 111,

        // Добавление списка лидов
        [TrackEvent("Product.Core.Leads.AddLeadsList")]
        Core_Leads_AddLeadsList = 112,

        // Редактирование этапа сделки (добавление или удаление или редактирование)
        [TrackEvent("Product.Core.Leads.DealStatusCUD")]
        Core_Leads_DealStatusCUD = 113,

        #endregion

        #region Tasks

        // Создана задача
        [TrackEvent("Product.Core.Tasks.TaskCreated")]
        Core_Tasks_TaskCreated = 114,

        // Редактирование задачи
        [TrackEvent("Product.Core.Tasks.EditTask")]
        Core_Tasks_EditTask = 115,

        // Изменен статус задачи
        [TrackEvent("Product.Core.Tasks.TaskStatusChanged")]
        Core_Tasks_TaskStatusChanged = 116,

        // Добавление комментария к задаче
        [TrackEvent("Product.Core.Tasks.CommentAdded")]
        Core_Tasks_CommentAdded = 117,

        // Добавление проекта
        [TrackEvent("Product.Core.Tasks.TaskProjectCreated")]
        Core_Tasks_TaskProjectCreated = 118,

        #endregion

        #region Booking

        // Просмотр страницы списка броней
        [TrackEvent("Product.Core.Booking.ViewBookingList")]
        Core_Booking_ViewBookingList = 119,

        // Просмотр брони
        [TrackEvent("Product.Core.Booking.ViewBooking")]
        Core_Booking_ViewBooking = 120,

        // Редактирование брони
        [TrackEvent("Product.Core.Booking.EditBooking")]
        Core_Booking_EditBooking = 121,

        // Изменение статуса брони
        [TrackEvent("Product.Core.Booking.BookingStatusChanged")]
        Core_Booking_BookingStatusChanged = 122,

        // Добавление филиала
        [TrackEvent("Product.Core.Booking.AffiliateCreated")]
        Core_Booking_AffiliateCreated = 123,

        // Добавление категории услуг
        [TrackEvent("Product.Core.Booking.CategoryCreated")]
        Core_Booking_CategoryCreated = 124,

        // Добавление услуги
        [TrackEvent("Product.Core.Booking.ServiceCreated")]
        Core_Booking_ServiceCreated = 125,

        // Добавление ресурса
        [TrackEvent("Product.Core.Booking.ResourceCreated")]
        Core_Booking_ResourceCreated = 126,

        // Редактирование ресурса
        [TrackEvent("Product.Core.Booking.EditResource")]
        Core_Booking_EditResource = 127,

        // Просмотр отчетов
        [TrackEvent("Product.Core.Booking.ViewReports")]
        Core_Booking_ViewReports = 128,

        // Изменение настроек бронирования
        [TrackEvent("Product.Core.Booking.EditSettings")]
        Core_Booking_EditSettings = 129,

        #endregion

        #region Discounts

        // Добавление скидки
        [TrackEvent("Product.Core.Discounts.DiscountPriceRangeCreated")]
        Core_Discounts_DiscountPriceRangeCreated = 130,

        // Добавление купона
        [TrackEvent("Product.Core.Discounts.CouponCreated")]
        Core_Discounts_CouponCreated = 131,

        // Добавление подарочного сертификата
        [TrackEvent("Product.Core.Discounts.CertificateCreated")]
        Core_Discounts_CertificateCreated = 132,

        #endregion

        #region Reports

        // Просмотр страницы отчетов
        [TrackEvent("Product.Core.Reports.ViewReports")]
        Core_Reports_ViewReports = 133,

        #endregion

        #region Common

        // Заход в админку первый раз в день
        [TrackEvent("Product.Core.Common.FirstVisitAdminAreaOfDay")]
        Core_Common_FirstVisitAdminAreaOfDay = 134,

        // Кнопка быстрого добавления. Все события
        [TrackEvent("Product.Core.Common.LeftMenu.QuickAdd")]
        Core_Common_LeftMenu_QuickAdd = 135,

        // Изменены настройки индикаторов на dashboard
        [TrackEvent("Product.Core.Common.StatisticsDashboard.IndicatorsChanged")]
        Core_Common_StatisticsDashboard_IndicatorsChanged = 136,

        // Кликнули на статье Рекомендуем почитать
        [TrackEvent("Product.Core.Common.RecommendationsDashboard.ClickArticle")]
        Core_Common_RecommendationsDashboard_ClickArticle = 137,

        // Кликнули на партнерском баннере
        [TrackEvent("Product.Core.Common.PartnersDashboard.ClickBanner")]
        Core_Common_PartnersDashboard_ClickBanner = 138,

        // Кликнули на статусе заказа на главной
        [TrackEvent("Product.Core.Common.OrdersDashboard.ClickOrderStatus")]
        Core_Common_OrdersDashboard_ClickOrderStatus = 139,

        // Кликнули на заказе в гриде заказов с dashboard
        [TrackEvent("Product.Core.Common.LastOrdersDashboard.ClickOrder")]
        Core_Common_LastOrdersDashboard_ClickOrder = 140,

        // Кликнули по кнопке Витрина магазина
        [TrackEvent("Product.Core.Common.ClickStorefrontLink")]
        Core_Common_ClickStorefrontLink = 141,

        // Изменили название проекта в шапке админки
        [TrackEvent("Product.Core.Common.Head.ShopNameChanged")]
        Core_Common_Head_ShopNameChanged = 142,

        // Загрузили аватар через верхний угол
        [TrackEvent("Product.Core.Common.Head.AddAvatar")]
        Core_Common_Head_AddAvatar = 143,

        // Воспользовались поиском по магазину
        [TrackEvent("Product.Core.Common.Head.UsedSearch")]
        Core_Common_Head_UsedSearch = 144,

        // Переход в нотификации
        [TrackEvent("Product.Core.Common.ViewNotificationsPage")]
        Core_Common_ViewNotificationsPage = 145,

        // Клик по Отметить все уведомления как прочитанные
        [TrackEvent("Product.Core.Common.SetAllNotificationsSeen")]
        Core_Common_SetAllNotificationsSeen = 146,

        // Авторизация в админке (любой сотрудник)
        [TrackEvent("Product.Core.Common.Login.AdminArea")]
        Core_Common_Login_AdminArea = 147,

        // Восстановление пароля к админке (любой сотрудник)
        [TrackEvent("Product.Core.Common.ForgotPassword.AdminArea")]
        Core_Common_ForgotPassword_AdminArea = 148,

        #endregion

        #region Settings

        // Загружен логотип из настроек
        [TrackEvent("Product.Core.Settings.AddLogo.AdminArea")]
        Core_Settings_AddLogo_AdminArea = 149,

        // Создан логотип с помощью конструктора из настроек
        [TrackEvent("Product.Core.Settings.GenerateLogo.AdminArea")]
        Core_Settings_GenerateLogo_AdminArea = 150,

        // Загружена favicon
        [TrackEvent("Product.Core.Settings.AddFavicon")]
        Core_Settings_AddFavicon = 151,

        // Добавлен способ доставки (какой)
        [TrackEvent("Product.Core.Settings.ShippingMethodCreated")]
        Core_Settings_ShippingMethodCreated = 152,

        // Добавлен способ доставки (Boxberry)
        [TrackEvent("Product.Core.Settings.BoxberryShippingMethodCreated")]
        Core_Settings_BoxberryShippingMethodCreated = 153,

        // Добавлен способ оплаты (какой)
        [TrackEvent("Product.Core.Settings.PaymentMethodCreated")]
        Core_Settings_PaymentMethodCreated = 154,

        // Добавлен способ оплаты (Robokassa)
        [TrackEvent("Product.Core.Settings.RobokassaPaymentMethodCreated")]
        Core_Settings_RobokassaPaymentMethodCreated = 155,

        // Привязанная почта на advantshop сервисе
        [TrackEvent("Product.Core.Settings.BindAdvantshopMailService", SendOnce = true)]
        Core_Settings_BindAdvantshopMailService = 156,

        // Изменены настройки отправки почты (любые)
        [TrackEvent("Product.Core.Settings.EditMailSettings")]
        Core_Settings_EditMailSettings = 157,

        // Добавлен сотрудник
        [TrackEvent("Product.Core.Settings.EmployeeCreated")]
        Core_Settings_EmployeeCreated = 158,

        // Изменение параметров соц сетей
        [TrackEvent("Product.Core.Settings.EditSocialSettings")]
        Core_Settings_EditSocialSettings = 159,

        // Изменение формата письма с типом "Смена статуса заказа"
        [TrackEvent("Product.Core.Settings.EditMailFormatWithTypeOnChangeOrderStatus")]
        Core_Settings_EditMailFormatWithTypeOnChangeOrderStatus = 160,

        // Изменение формата письма с типом "При добавлении отзыва о заказе"
        [TrackEvent("Product.Core.Settings.EditMailFormatWithTypeOnNewOrderReview")]
        Core_Settings_EditMailFormatWithTypeOnNewOrderReview = 161,

        #endregion

        #region Warehouse

        // Добавлен новый домен от геолокации
        [TrackEvent("Product.Core.Warehouse.AddDomain")]
        Core_Warehouse_AddDomain = 162,

        #endregion

        #region Bonuses

        // Добавлен/изменен шаблон уведомлений бонусной системы
        [TrackEvent("Product.Core.Bonuses.AddEditNotificationTemplate")]
        Core_Bonuses_AddEditNotificationTemplate = 163,

        // Добавление грейда бонусной системы
        [TrackEvent("Product.Core.Bonuses.AddBonusGrade")]
        Core_Bonuses_AddBonusGrade = 164,

        // Изменение грейда бонусной системы
        [TrackEvent("Product.Core.Bonuses.EditBonusGrade")]
        Core_Bonuses_EditBonusGrade = 165,

        // Добавление триггера бонусной системы
        [TrackEvent("Product.Core.Bonuses.AddBonusTrigger")]
        Core_Bonuses_AddBonusTrigger = 166,

        #endregion

        #region CongratulationsDashboard

        // Заполнена основная информация о магазине
        [TrackEvent("Product.Core.CongratulationsDashboard.StoreInfoDone")]
        Core_CongratulationsDashboard_StoreInfoDone = 167,

        #endregion

        #endregion

        #region Shop

        #region Design

        // Применен шаблон дизайна (какой)
        [TrackEvent("Product.Shop.Design.TemplateApplied")]
        Shop_Design_TemplateApplied = 168,

        // Установлен шаблон из магазина шаблонов (какой)
        [TrackEvent("Product.Shop.Design.TemplateInstalled")]
        Shop_Design_TemplateInstalled = 169,

        // Изменены настройки шаблона
        [TrackEvent("Product.Shop.Design.EditTemplateSettings")]
        Shop_Design_EditTemplateSettings = 170,

        // Запущено пережатие фотографий
        [TrackEvent("Product.Shop.Design.ResizePictures")]
        Shop_Design_ResizePictures = 171,

        // Изменена цветовая схема
        [TrackEvent("Product.Shop.Design.ColorScheme")]
        Shop_Design_ChangeColorScheme = 172,

        #endregion

        #region Modules

        // Установлен модуль (какой)
        [TrackEvent("Product.Shop.Modules.ModuleInstalled")]
        Shop_Modules_ModuleInstalled = 173,

        #endregion

        #region StaticPages

        // Добавлена страница
        [TrackEvent("Product.Shop.StaticPages.StaticPageCreated")]
        Shop_StaticPages_StaticPageCreated = 174,

        // Изменена страница
        [TrackEvent("Product.Shop.StaticPages.EditStaticPage")]
        Shop_StaticPages_EditStaticPage = 175,

        // Изменена страница (Контакты)
        [TrackEvent("Product.Shop.StaticPages.EditContactsStaticPage")]
        Shop_StaticPages_EditContactsStaticPage = 176,

        #endregion

        #region News

        // Добавлена новость
        [TrackEvent("Product.Shop.News.NewsCreated")]
        Shop_News_NewsCreated = 177,

        #endregion

        #region Carousel

        // Загружен слайд
        [TrackEvent("Product.Shop.Carousel.AddSlide")]
        Shop_Carousel_AddSlide = 178,

        // Удален слайд
        [TrackEvent("Product.Shop.Carousel.DeleteSlide")]
        Shop_Carousel_DeleteSlide = 179,

        #endregion

        #region Menu

        // Добавлен пункт меню
        [TrackEvent("Product.Shop.Menu.MenuItemCreated")]
        Shop_Menu_MenuItemCreated = 180,

        #endregion

        #region Files

        // Добавлен файл
        [TrackEvent("Product.Shop.Files.AddFile")]
        Shop_Files_AddFile = 181,

        #endregion

        #region Voting

        // Создано голосование
        [TrackEvent("Product.Shop.Voting.VotingCreated")]
        Shop_Voting_VotingCreated = 182,

        #endregion

        #region Funnels

        // Создана новая воронка продаж (по каждому типу)
        [TrackEvent("Product.Shop.Funnels.FunnelCreated")]
        Shop_Funnels_FunnelCreated = 183,

        // Добавлена страница в воронку
        [TrackEvent("Product.Shop.Funnels.PageCreated")]
        Shop_Funnels_PageCreated = 184,

        // Открыта страница в редакторе
        [TrackEvent("Product.Shop.Funnels.ViewPageEditor")]
        Shop_Funnels_ViewPageEditor = 185,

        // Добавлен новый блок на страницу
        [TrackEvent("Product.Shop.Funnels.AddBlock")]
        Shop_Funnels_AddBlock = 186,

        // Изменены настройки воронки
        [TrackEvent("Product.Shop.Funnels.EditFunnelSettings")]
        Shop_Funnels_EditFunnelSettings = 187,

        // Привязан домен к воронке
        [TrackEvent("Product.Shop.Funnels.BindDomain")]
        Shop_Funnels_BindDomain = 188,

        // Включены воронки в приложениях
        [TrackEvent("Product.Shop.Funnels.FunnelsEnabled")]
        Shop_Funnels_FunnelsEnabled = 189,

        // Клик по приложению "Воронки"
        [TrackEvent("Product.Shop.Funnels.LeftMenuClickFunnels")]
        Shop_Funnels_LeftMenuClickFunnels = 190,

        // Клик по кнопке и блоку "Создать воронку"
        [TrackEvent("Product.Shop.Funnels.ClickCreateFunnel")]
        Shop_Funnels_ClickCreateFunnel = 191,

        // Выбран любой из типов воронки на странице создания воронки (по каждому типу)
        [TrackEvent("Product.Shop.Funnels.ClickFunnelType")]
        Shop_Funnels_ClickFunnelType = 192,

        // Просмотр видео внутри типа воронки
        [TrackEvent("Product.Shop.Funnels.WatchVideo")]
        Shop_Funnels_WatchVideo = 193,

        // Просмотр схемы воронки внутри типа
        [TrackEvent("Product.Shop.Funnels.WatchScheme")]
        Shop_Funnels_WatchScheme = 194,

        // Выбран шаблон внутри типа воронки
        [TrackEvent("Product.Shop.Funnels.ClickTemplate")]
        Shop_Funnels_ClickTemplate = 195,

        // Редактирование блока
        [TrackEvent("Product.Shop.Funnels.EditBlock")]
        Shop_Funnels_EditBlock = 196,

        // Редактирование настроек страницы (на странице воронки)
        [TrackEvent("Product.Shop.Funnels.EditPageSettings")]
        Shop_Funnels_EditPageSettings = 197,

        // Клик по настройкам воронки (в админке)
        [TrackEvent("Product.Shop.Funnels.ClickFunnelSettings")]
        Shop_Funnels_ClickFunnelSettings = 198,

        // Клик по кнопке "купить домен" в настройках воронки
        [TrackEvent("Product.Shop.Funnels.ClickBuyDomain")]
        Shop_Funnels_ClickBuyDomain = 199,

        // Копирование воронки
        [TrackEvent("Product.Shop.Funnels.CopyFunnel")]
        Shop_Funnels_CopyFunnel = 200,

        // Копирование страницы
        [TrackEvent("Product.Shop.Funnels.CopyPage")]
        Shop_Funnels_CopyPage = 201,

        #endregion

        #region Vk

        // Подключена группа
        [TrackEvent("Product.Shop.Vk.GroupConnected")]
        Shop_Vk_GroupConnected = 202,

        // Осуществлена выгрузка товаров первый раз
        [TrackEvent("Product.Shop.Vk.FirstExportProducts", SendOnce = true)]
        Shop_Vk_FirstExportProducts = 203,

        // Осуществлена выгрузка товаров последующие разы
        [TrackEvent("Product.Shop.Vk.ExportProducts")]
        Shop_Vk_ExportProducts = 204,

        #endregion

        #region Instagram

        // Подключен аккаунт
        [TrackEvent("Product.Shop.Instagram.AccountConnected")]
        Shop_Instagram_AccountConnected = 205,

        #endregion

        #region Facebook

        // Подключена группа
        [TrackEvent("Product.Shop.Facebook.GroupConnected")]
        Shop_Facebook_GroupConnected = 206,

        #endregion

        #region Telegram

        // Подключен бот
        [TrackEvent("Product.Shop.Telegram.BotConnected")]
        Shop_Telegram_BotConnected = 207,

        #endregion

        #region OK

        [TrackEvent("Product.Shop.Ok.BotConnected")]
        Shop_Ok_BotConnected = 208,

        [TrackEvent("Product.Shop.Ok.ExportProducts")]
        Shop_Ok_ExportProducts = 209,

        #endregion

        #region ExportFeeds

        // Добавлена новая выгрузка/новый файл (тип)
        [TrackEvent("Product.Shop.ExportFeeds.ExportFeedCreated")]
        Shop_ExportFeeds_ExportFeedCreated = 210,

        // Изменены параметры выгрузки (тип)
        [TrackEvent("Product.Shop.ExportFeeds.EditExportFeed")]
        Shop_ExportFeeds_EditExportFeed = 211,

        // Включена настройка выгружать по расписанию (тип)
        [TrackEvent("Product.Shop.ExportFeeds.EnableJob")]
        Shop_ExportFeeds_EnableJob = 212,

        // Осуществлена выгрузка вручную (тип)
        [TrackEvent("Product.Shop.ExportFeeds.ExportManual")]
        Shop_ExportFeeds_ExportManual = 213,

        // Осуществлена выгрузка автоматически (тип)
        [TrackEvent("Product.Shop.ExportFeeds.ExportAuto")]
        Shop_ExportFeeds_ExportAuto = 214,

        // YandexMarket: Изменение параметров промо-акций (все виды)
        [TrackEvent("Product.Shop.ExportFeeds.YandexMarket.EditPromo")]
        Shop_ExportFeeds_YandexMarket_EditPromo = 215,

        #endregion

        #endregion

        #region Triggers

        // Создание триггера (по типам)
        [TrackEvent("Product.Triggers.TriggerCreated")]
        Triggers_TriggerCreated = 216,

        // Редактирование триггера
        [TrackEvent("Product.Triggers.EditTrigger")]
        Triggers_EditTrigger = 217,

        // Просмотр аналитики триггерной email рассылки
        [TrackEvent("Product.Triggers.ViewTriggerEmailingAnalitycs")]
        Triggers_ViewTriggerEmailingAnalitycs = 218,

        // Просмотр аналитики ручной email рассылки
        [TrackEvent("Product.Triggers.ViewManualEmailingAnalitycs")]
        Triggers_ViewManualEmailingAnalitycs = 219,

        #endregion

        #region SalesChannels

        // Добавлен канал продаж (по типам)
        [TrackEvent("Product.SalesChannels.SalesChannelAdded")]
        SalesChannels_SalesChannelAdded = 220,

        // Добавлен канал продаж (Бонусы)
        [TrackEvent("Product.SalesChannels.BonusSalesChannelAdded")]
        SalesChannels_BonusSalesChannelAdded = 221,

        // Добавлен канал продаж (Vk)
        [TrackEvent("Product.SalesChannels.VkSalesChannelAdded")]
        SalesChannels_VkSalesChannelAdded = 222,

        // Добавлен канал продаж (Ozon)
        [TrackEvent("Product.SalesChannels.OzonSalesChannelAdded")]
        SalesChannels_OzonSalesChannelAdded = 223,

        // Добавлен канал продаж (Yandex)
        [TrackEvent("Product.SalesChannels.YandexSalesChannelAdded")]
        SalesChannels_YandexSalesChannelAdded = 224,

        // Проявлен интерес к функционалу (по каналам)
        [TrackEvent("interest", TrialPrefix = "trial", Delimiter = "_")]
        SalesChannels_Interest = 225,

        // Попытка подключения Покупок на Маркете
        [TrackEvent("connection_attempt_ym", TrialPrefix = "trial", Delimiter = "_")]
        SalesChannels_ConnectAttempt_YM = 226,

        // Попытка подключения vk
        [TrackEvent("connection_attempt_vk", TrialPrefix = "trial", Delimiter = "_")]
        SalesChannels_ConnectAttempt_Vk = 227,

        #region Ozon

        // Попытка подключения Ozon
        [TrackEvent("connection_attempt_ozon", TrialPrefix = "trial", Delimiter = "_")]
        SalesChannels_ConnectAttempt_Ozon = 228,

        // Синхронизация товаров
        [TrackEvent("products_sync_ozon", TrialPrefix = "trial", Delimiter = "_")]
        SalesChannels_ProductsSync_Ozon = 229,

        // Выгрузка товаров
        [TrackEvent("products_import_ozon", TrialPrefix = "trial", Delimiter = "_")]
        SalesChannels_ProductsImport_Ozon = 230,

        // Обновление цен
        [TrackEvent("prices_update_ozon", TrialPrefix = "trial", Delimiter = "_")]
        SalesChannels_PricesUpdate_Ozon = 231,

        // Обновление остатков
        [TrackEvent("stocks_update_ozon", TrialPrefix = "trial", Delimiter = "_")]
        SalesChannels_StocksUpdate_Ozon = 232,

        // Загрузка заказов/статусов
        [TrackEvent("orders_get_ozon", TrialPrefix = "trial", Delimiter = "_")]
        SalesChannels_OrdersGet_Ozon = 233,

        // Выгрузка заказов/статусов
        [TrackEvent("orders_put_ozon", TrialPrefix = "trial", Delimiter = "_")]
        SalesChannels_OrdersPut_Ozon = 234,

        // Первый заказ через Ozon
        [TrackEvent("firstorder_ozon", TrialPrefix = "trial", Delimiter = "_")]
        SalesChannels_FirstOrder_Ozon = 235,

        #endregion

        #region Wildberries

        // Попытка подключения Wildberries
        [TrackEvent("connection_attempt_wb", TrialPrefix = "trial", Delimiter = "_")]
        SalesChannels_ConnectAttempt_Wb = 236,

        // Синхронизация товаров
        [TrackEvent("products_sync_wb", TrialPrefix = "trial", Delimiter = "_")]
        SalesChannels_ProductsSync_Wb = 237,

        // Выгрузка товаров
        [TrackEvent("products_import_wb", TrialPrefix = "trial", Delimiter = "_")]
        SalesChannels_ProductsImport_Wb = 238,

        // Обновление цен
        [TrackEvent("prices_update_wb", TrialPrefix = "trial", Delimiter = "_")]
        SalesChannels_PricesUpdate_Wb = 239,

        // Обновление остатков
        [TrackEvent("stocks_update_wb", TrialPrefix = "trial", Delimiter = "_")]
        SalesChannels_StocksUpdate_Wb = 240,

        // Загрузка заказов/статусов
        [TrackEvent("orders_get_wb", TrialPrefix = "trial", Delimiter = "_")]
        SalesChannels_OrdersGet_Wb = 241,

        // Выгрузка заказов/статусов
        [TrackEvent("orders_put_wb", TrialPrefix = "trial", Delimiter = "_")]
        SalesChannels_OrdersPut_Wb = 242,

        // Первый заказ через Wildberries
        [TrackEvent("firstorder_wb", TrialPrefix = "trial", Delimiter = "_")]
        SalesChannels_FirstOrder_Wb = 243,

        #endregion

        #region AliExpress

        // Попытка подключения AliExpress
        [TrackEvent("connection_attempt_ali", TrialPrefix = "trial", Delimiter = "_")]
        SalesChannels_ConnectAttempt_Ali = 244,

        // Синхронизация товаров
        [TrackEvent("products_sync_ali", TrialPrefix = "trial", Delimiter = "_")]
        SalesChannels_ProductsSync_Ali = 245,

        // Выгрузка товаров
        [TrackEvent("products_import_ali", TrialPrefix = "trial", Delimiter = "_")]
        SalesChannels_ProductsImport_Ali = 246,

        // Обновление цен
        [TrackEvent("prices_update_ali", TrialPrefix = "trial", Delimiter = "_")]
        SalesChannels_PricesUpdate_Ali = 247,

        // Обновление остатков
        [TrackEvent("stocks_update_ali", TrialPrefix = "trial", Delimiter = "_")]
        SalesChannels_StocksUpdate_Ali = 248,

        // Загрузка заказов/статусов
        [TrackEvent("orders_get_ali", TrialPrefix = "trial", Delimiter = "_")]
        SalesChannels_OrdersGet_Ali = 249,

        // Выгрузка заказов/статусов
        [TrackEvent("orders_put_ali", TrialPrefix = "trial", Delimiter = "_")]
        SalesChannels_OrdersPut_Ali = 250,

        // Первый заказ через AliExpress
        [TrackEvent("firstorder_ali", TrialPrefix = "trial", Delimiter = "_")]
        SalesChannels_FirstOrder_Ali = 251,

        #endregion

        #endregion

        #region MobileApp

        // Клик по кнопке установить в демо
        [TrackEvent("Product.MobileApp.ClickBtnInstall")]
        MobileApp_ClickBtnInstall = 252,

        [TrackEvent("mobile_app_device")] MobileApp_Device = 253,

        [TrackEvent("mobile_app_os")] MobileApp_Os = 254,

        [TrackEvent("mobile_app_version")] MobileApp_AppVersion = 255,

        [TrackEvent("mobile_app_ip")] MobileApp_Ip = 256,

        #endregion

        #region Заглушка клиентки в триале

        // Страница открылась
        [TrackEvent(EShopMode.TrialMode, EventKey = "ClientBlocker_visited")]
        ClientBlocker_Visited = 257,

        // Авторизовался успешно как гость или перешел по ссылке "Войти как администратор"
        [TrackEvent(EShopMode.TrialMode, EventKey = "ClientBlocker_authorized")]
        ClientBlocker_Authorized = 258,

        #endregion

        // Создан заказ через CSV
        [TrackEvent("Product.Core.Orders.OrderCreated.Csv")]
        Core_Orders_OrderCreated_Csv = 259,

        // Заказ изменен через CSV
        [TrackEvent("Product.Core.Orders.EditOrder.Csv")]
        Core_Orders_EditOrder_Csv = 260,

        // Создан покупатель через CSV
        [TrackEvent("Product.Core.Customers.CustomerCreated.Csv")]
        Core_Customers_CustomerCreated_Csv = 261,

        // Отредактированы данные покупателя через CSV
        [TrackEvent("Product.Core.Customers.EditCustomer.Csv")]
        Core_Customers_EditCustomer_Csv = 262,

        // Добавлена категория через CSV
        [TrackEvent("Product.Core.Categories.CategoryCreated.Csv")]
        Core_Categories_CategoryCreated_Csv = 263,

        // Добавлена категория через модуль YandexMarketImport
        [TrackEvent("Product.Core.Categories.CategoryCreated.Yml")]
        Core_Categories_CategoryCreated_Yml = 264,

        // Добавлена категория через модуль MoySklad или MoySkladPro
        [TrackEvent("Product.Core.Categories.CategoryCreated.MoySklad")]
        Core_Categories_CategoryCreated_MoySklad = 265,

        // Добавлена категория через модуль OneCCommerceML
        [TrackEvent("Product.Core.Categories.CategoryCreated.OneC")]
        Core_Categories_CategoryCreated_OneC = 266,

        // Добавлена категория через модуль Simaland
        [TrackEvent("Product.Core.Categories.CategoryCreated.Simaland")]
        Core_Categories_CategoryCreated_Simaland = 267,

        // Добавлена категория через модуль SupplierOfHappiness или SupplierOfHappinessSmartFeed
        [TrackEvent("Product.Core.Categories.CategoryCreated.SupplierOfHappiness")]
        Core_Categories_CategoryCreated_SupplierOfHappiness = 268,

        // Изменена категория через CSV
        [TrackEvent("Product.Core.Categories.EditCategory.Csv")]
        Core_Categories_EditCategory_Csv = 269,

        // Изменена категория через модуль MoySklad или MoySkladPro
        [TrackEvent("Product.Core.Categories.EditCategory.MoySklad")]
        Core_Categories_EditCategory_MoySklad = 270,

        // Изменена категория через модуль OneCCommerceML
        [TrackEvent("Product.Core.Categories.EditCategory.OneC")]
        Core_Categories_EditCategory_OneC = 271,

        // Изменена категория через модуль Simaland
        [TrackEvent("Product.Core.Categories.EditCategory.Simaland")]
        Core_Categories_EditCategory_Simaland = 272,

        // Добавлен товар через CSV
        [TrackEvent("Product.Core.Products.ProductCreated.Csv")]
        Core_Products_ProductCreated_Csv = 273,

        // Добавлен товар через модуль YandexMarketImport
        [TrackEvent("Product.Core.Products.ProductCreated.Yml")]
        Core_Products_ProductCreated_Yml = 274,

        // Добавлен товар через модуль MoySklad или MoySkladPro
        [TrackEvent("Product.Core.Products.ProductCreated.MoySklad")]
        Core_Products_ProductCreated_MoySklad = 275,

        // Добавлен товар через модуль OneCCommerceML
        [TrackEvent("Product.Core.Products.ProductCreated.OneC")]
        Core_Products_ProductCreated_OneC = 276,

        // Добавлен товар через модуль Simaland
        [TrackEvent("Product.Core.Products.ProductCreated.Simaland")]
        Core_Products_ProductCreated_Simaland = 277,

        // Добавлен товар через модуль SupplierOfHappiness или SupplierOfHappinessSmartFeed
        [TrackEvent("Product.Core.Products.ProductCreated.SupplierOfHappiness")]
        Core_Products_ProductCreated_SupplierOfHappiness = 278,

        // Товар изменен через CSV
        [TrackEvent("Product.Core.Products.EditProduct.Csv")]
        Core_Products_EditProduct_Csv = 279,

        // Товар изменен через модуль YandexMarketImport
        [TrackEvent("Product.Core.Products.EditProduct.Yml")]
        Core_Products_EditProduct_Yml = 280,

        // Товар изменен через модуль MoySklad или MoySkladPro
        [TrackEvent("Product.Core.Products.EditProduct.MoySklad")]
        Core_Products_EditProduct_MoySklad = 281,

        // Товар изменен через модуль OneCCommerceML
        [TrackEvent("Product.Core.Products.EditProduct.OneC")]
        Core_Products_EditProduct_OneC = 282,

        // Товар изменен через модуль Simaland
        [TrackEvent("Product.Core.Products.EditProduct.Simaland")]
        Core_Products_EditProduct_Simaland = 283,

        // Товар изменен через модуль SupplierOfHappiness или SupplierOfHappinessSmartFeed
        [TrackEvent("Product.Core.Products.EditProduct.SupplierOfHappiness")]
        Core_Products_EditProduct_SupplierOfHappiness = 284,

        // Изменение купона
        [TrackEvent("Product.Core.Discounts.CouponEdited")]
        Core_Discounts_CouponEdited = 285,

        // Изменен способ доставки (какой)
        [TrackEvent("Product.Core.Settings.ShippingMethodEdited")]
        Core_Settings_ShippingMethodEdited = 286,

        // Изменен способ оплаты (какой)
        [TrackEvent("Product.Core.Settings.PaymentMethodEdited")]
        Core_Settings_PaymentMethodEdited = 287,

        // Изменен сотрудник
        [TrackEvent("Product.Core.Settings.EmployeeEdited")]
        Core_Settings_EmployeeEdited = 288,

        // Изменен пароль сотрудника
        [TrackEvent("Product.Core.Settings.ChangeEmployeePassword")]
        Core_Settings_ChangeEmployeePassword = 289,

        // Изменены настройки заказов (любые)
        [TrackEvent("Product.Core.Settings.EditCheckoutSettings")]
        Core_Settings_EditCheckoutSettings = 290,

        // Изменены поля в оформлении заказа
        [TrackEvent("Product.Core.Settings.EditCheckoutFields")]
        Core_Settings_EditCheckoutFields = 291,

        #region Achievements events

        // Подписаться на соцсети
        [TrackEvent("Achievements.SocialNetworks.Subscribe")]
        SocialNetworks_Subscribe = 292,

        // Перейти в центр поддержки
        [TrackEvent("Achievements.Support.GoToSupportCenter")]
        Support_GoToSupportCenter = 293,

        #endregion
    }
}