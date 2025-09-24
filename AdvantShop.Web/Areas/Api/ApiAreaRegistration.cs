using System.Web.Mvc;
using System.Web.Routing;

namespace AdvantShop.Areas.Api
{
    public class ApiAreaRegistration : AreaRegistration
    {
        public override string AreaName => "C";

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                name: "Api_1C",
                url: "api/1c/{action}/",
                defaults: new {controller = "OneC", action = "Index"},
                namespaces: new[] {"AdvantShop.Areas.Api.Controllers"}
                );

            #region Customers

            context.MapRoute(
                name: "Api_Customers_Get",
                url: "api/customers/{id}",
                defaults: new { controller = "Customers", action = "Get" },
                constraints: new { httpMethod = new HttpMethodConstraint("GET"), id = "[A-Z0-9]{8}-([A-Z0-9]{4}-){3}[A-Z0-9]{12}" },
                namespaces: new[] { "AdvantShop.Areas.Api.Controllers" }
            );

            context.MapRoute(
                name: "Api_Customers_Update",
                url: "api/customers/{id}",
                defaults: new { controller = "Customers", action = "Update", id = UrlParameter.Optional },
                constraints: new { httpMethod = new HttpMethodConstraint("POST"), id = "[A-Z0-9]{8}-([A-Z0-9]{4}-){3}[A-Z0-9]{12}" },
                namespaces: new[] { "AdvantShop.Areas.Api.Controllers" }
            );

            context.MapRoute(
                name: "Api_Customers_Bonuses",
                url: "api/customers/{id}/bonuses",
                defaults: new { controller = "Customers", action = "Bonuses", id = UrlParameter.Optional },
                constraints: new { httpMethod = new HttpMethodConstraint("GET"), id = "[A-Z0-9]{8}-([A-Z0-9]{4}-){3}[A-Z0-9]{12}" },
                namespaces: new[] { "AdvantShop.Areas.Api.Controllers" }
            );

            context.MapRoute(
                name: "Api_Customers_Filter",
                url: "api/customers",
                defaults: new { controller = "Customers", action = "Filter" },
                namespaces: new[] { "AdvantShop.Areas.Api.Controllers" }
            );

            #endregion

            #region Bonuses

            context.MapRoute(
                name: "Api_Bonuses_GetSettings",
                url: "api/bonus-cards/settings",
                defaults: new { controller = "BonusCards", action = "GetSettings" },
                constraints: new { httpMethod = new HttpMethodConstraint("GET") },
                namespaces: new[] { "AdvantShop.Areas.Api.Controllers" }
            );

            context.MapRoute(
                name: "Api_Bonuses_SaveSettings",
                url: "api/bonus-cards/settings",
                defaults: new { controller = "BonusCards", action = "SaveSettings" },
                constraints: new { httpMethod = new HttpMethodConstraint("POST") },
                namespaces: new[] { "AdvantShop.Areas.Api.Controllers" }
            );


            context.MapRoute(
                name: "Api_Bonuses_GetCard",
                url: "api/bonus-cards/{id}",
                defaults: new { controller = "BonusCards", action = "Card" },
                constraints: new { httpMethod = new HttpMethodConstraint("GET") },
                namespaces: new[] { "AdvantShop.Areas.Api.Controllers" }
            );

            context.MapRoute(
                name: "Api_Bonuses_Create",
                url: "api/bonus-cards/add",
                defaults: new { controller = "BonusCards", action = "Create" },
                constraints: new { httpMethod = new HttpMethodConstraint("POST") },
                namespaces: new[] { "AdvantShop.Areas.Api.Controllers" }
            );
            
            context.MapRoute(
                name: "Api_Bonuses_Me_Create",
                url: "api/bonus-cards/me/add",
                defaults: new { controller = "BonusCards", action = "MeCreate" },
                constraints: new { httpMethod = new HttpMethodConstraint("POST") },
                namespaces: new[] { "AdvantShop.Areas.Api.Controllers" }
            );
            
            context.MapRoute(
                name: "Api_Bonuses_Me_GetCardTransactions",
                url: "api/bonus-cards/me/transactions",
                defaults: new { controller = "BonusCards", action = "GetTransactionsMe" },
                constraints: new { httpMethod = new HttpMethodConstraint("GET") },
                namespaces: new[] { "AdvantShop.Areas.Api.Controllers" }
            );

            context.MapRoute(
                name: "Api_Bonuses_GetCardTransactions",
                url: "api/bonus-cards/{id}/transactions",
                defaults: new { controller = "BonusCards", action = "Transactions" },
                constraints: new { httpMethod = new HttpMethodConstraint("GET") },
                namespaces: new[] { "AdvantShop.Areas.Api.Controllers" }
            );

            context.MapRoute(
                name: "Api_Bonuses_GetBonuses",
                url: "api/bonus-cards/{id}/bonuses",
                defaults: new { controller = "BonusCards", action = "GetBonuses" },
                constraints: new { httpMethod = new HttpMethodConstraint("GET") },
                namespaces: new[] { "AdvantShop.Areas.Api.Controllers" }
            );

            context.MapRoute(
                name: "Api_Bonuses_AcceptBonuses",
                url: "api/bonus-cards/{id}/bonuses/accept",
                defaults: new { controller = "BonusCards", action = "AcceptBonuses" },
                constraints: new { httpMethod = new HttpMethodConstraint("POST") },
                namespaces: new[] { "AdvantShop.Areas.Api.Controllers" }
            );

            context.MapRoute(
                name: "Api_Bonuses_SubstractBonuses",
                url: "api/bonus-cards/{id}/bonuses/substract",
                defaults: new { controller = "BonusCards", action = "SubstractBonuses" },
                constraints: new { httpMethod = new HttpMethodConstraint("POST") },
                namespaces: new[] { "AdvantShop.Areas.Api.Controllers" }
            );

            context.MapRoute(
                name: "Api_Bonuses_SubstractFreeAmountBonuses",
                url: "api/bonus-cards/{id}/bonuses/substract-free-amount",
                defaults: new { controller = "BonusCards", action = "SubstractFreeAmountBonuses" },
                constraints: new { httpMethod = new HttpMethodConstraint("POST") },
                namespaces: new[] { "AdvantShop.Areas.Api.Controllers" }
            );

            // Obsolete
            
            context.MapRoute(
                name: "Api_Bonuses_AddMainBonuses",
                url: "api/bonus-cards/{id}/main-bonuses/accept",
                defaults: new { controller = "BonusCards", action = "AcceptMainBonuses" },
                constraints: new { httpMethod = new HttpMethodConstraint("POST") },
                namespaces: new[] { "AdvantShop.Areas.Api.Controllers" }
            );

            context.MapRoute(
                name: "Api_Bonuses_SubstractMainBonuses",
                url: "api/bonus-cards/{id}/main-bonuses/substract",
                defaults: new { controller = "BonusCards", action = "SubstractMainBonuses" },
                constraints: new { httpMethod = new HttpMethodConstraint("POST") },
                namespaces: new[] { "AdvantShop.Areas.Api.Controllers" }
            );

            context.MapRoute(
                name: "Api_Bonuses_GetAdditionalBonuses",
                url: "api/bonus-cards/{id}/additional-bonuses",
                defaults: new { controller = "BonusCards", action = "GetAdditionalBonuses" },
                constraints: new { httpMethod = new HttpMethodConstraint("GET") },
                namespaces: new[] { "AdvantShop.Areas.Api.Controllers" }
            );

            context.MapRoute(
                name: "Api_Bonuses_AcceptAdditionalBonuses",
                url: "api/bonus-cards/{id}/additional-bonuses/accept",
                defaults: new { controller = "BonusCards", action = "AcceptAdditionalBonuses" },
                constraints: new { httpMethod = new HttpMethodConstraint("POST") },
                namespaces: new[] { "AdvantShop.Areas.Api.Controllers" }
            );

            context.MapRoute(
                name: "Api_Bonuses_SubstractAdditionalBonuses",
                url: "api/bonus-cards/{id}/additional-bonuses/substract",
                defaults: new { controller = "BonusCards", action = "SubstractAdditionalBonuses" },
                constraints: new { httpMethod = new HttpMethodConstraint("POST") },
                namespaces: new[] { "AdvantShop.Areas.Api.Controllers" }
            );
            
            // End Obsolete

            #endregion

            #region Bonus grades

            context.MapRoute(
                name: "Api_Grades_GetGrades",
                url: "api/bonus-grades",
                defaults: new { controller = "BonusGrades", action = "Grades" },
                constraints: new { httpMethod = new HttpMethodConstraint("GET") },
                namespaces: new[] { "AdvantShop.Areas.Api.Controllers" }
            );

            #endregion
            
            #region Categories
            
            context.MapRoute(
                name: "Api_Categories_Get",
                url: "api/categories/{id}",
                defaults: new { controller = "Categories", action = "Get" },
                constraints: new { httpMethod = new HttpMethodConstraint("GET") },
                namespaces: new[] { "AdvantShop.Areas.Api.Controllers" }
            );
            
            context.MapRoute(
                name: "Api_Categories_Add",
                url: "api/categories/add",
                defaults: new { controller = "Categories", action = "Add" },
                constraints: new { httpMethod = new HttpMethodConstraint("POST") },
                namespaces: new[] { "AdvantShop.Areas.Api.Controllers" }
            );
            
            context.MapRoute(
                name: "Api_Categories_Delete",
                url: "api/categories/{id}/delete",
                defaults: new { controller = "Categories", action = "Delete" },
                constraints: new { httpMethod = new HttpMethodConstraint("POST") },
                namespaces: new[] { "AdvantShop.Areas.Api.Controllers" }
            );
            
            #region Pictures
            
            context.MapRoute(
                name: "Api_Categories_Picture_AddByLink",
                url: "api/categories/{id}/picture/addbylink",
                defaults: new { controller = "Categories", action = "PictureAddByLink" },
                constraints: new { httpMethod = new HttpMethodConstraint("POST") },
                namespaces: new[] { "AdvantShop.Areas.Api.Controllers" }
            );
            
            context.MapRoute(
                name: "Api_Categories_Picture_Add",
                url: "api/categories/{id}/picture/add",
                defaults: new { controller = "Categories", action = "PictureAdd" },
                constraints: new { httpMethod = new HttpMethodConstraint("POST") },
                namespaces: new[] { "AdvantShop.Areas.Api.Controllers" }
            );

            context.MapRoute(
                name: "Api_Categories_Picture_Delete",
                url: "api/categories/{id}/picture/delete",
                defaults: new { controller = "Categories", action = "PictureDelete" },
                constraints: new { httpMethod = new HttpMethodConstraint("POST") },
                namespaces: new[] { "AdvantShop.Areas.Api.Controllers" }
            );
            
            // mini picture
            context.MapRoute(
                name: "Api_Categories_MiniPicture_AddByLink",
                url: "api/categories/{id}/mini-picture/addbylink",
                defaults: new { controller = "Categories", action = "MiniPictureAddByLink" },
                constraints: new { httpMethod = new HttpMethodConstraint("POST") },
                namespaces: new[] { "AdvantShop.Areas.Api.Controllers" }
            );
            
            context.MapRoute(
                name: "Api_Categories_MiniPicture_Add",
                url: "api/categories/{id}/mini-picture/add",
                defaults: new { controller = "Categories", action = "MiniPictureAdd" },
                constraints: new { httpMethod = new HttpMethodConstraint("POST") },
                namespaces: new[] { "AdvantShop.Areas.Api.Controllers" }
            );

            context.MapRoute(
                name: "Api_Categories_MiniPicture_Delete",
                url: "api/categories/{id}/mini-picture/delete",
                defaults: new { controller = "Categories", action = "MiniPictureDelete" },
                constraints: new { httpMethod = new HttpMethodConstraint("POST") },
                namespaces: new[] { "AdvantShop.Areas.Api.Controllers" }
            );
            
            // icon
            context.MapRoute(
                name: "Api_Categories_MenuIconPicture_AddByLink",
                url: "api/categories/{id}/menu-icon-picture/addbylink",
                defaults: new { controller = "Categories", action = "MenuIconPictureAddByLink" },
                constraints: new { httpMethod = new HttpMethodConstraint("POST") },
                namespaces: new[] { "AdvantShop.Areas.Api.Controllers" }
            );
            
            context.MapRoute(
                name: "Api_Categories_MenuIconPicture_Add",
                url: "api/categories/{id}/menu-icon-picture/add",
                defaults: new { controller = "Categories", action = "MenuIconPictureAdd" },
                constraints: new { httpMethod = new HttpMethodConstraint("POST") },
                namespaces: new[] { "AdvantShop.Areas.Api.Controllers" }
            );

            context.MapRoute(
                name: "Api_Categories_MenuIconPicture_Delete",
                url: "api/categories/{id}/menu-icon-picture/delete",
                defaults: new { controller = "Categories", action = "MenuIconPictureDelete" },
                constraints: new { httpMethod = new HttpMethodConstraint("POST") },
                namespaces: new[] { "AdvantShop.Areas.Api.Controllers" }
            );
            
            #endregion
            
            context.MapRoute(
                name: "Api_Categories_Update",
                url: "api/categories/{id}",
                defaults: new { controller = "Categories", action = "Update" },
                constraints: new { httpMethod = new HttpMethodConstraint("POST") },
                namespaces: new[] { "AdvantShop.Areas.Api.Controllers" }
            );
            
            context.MapRoute(
                name: "Api_Categories_Filter",
                url: "api/categories",
                defaults: new { controller = "Categories", action = "Filter" },
                constraints: new { httpMethod = new HttpMethodConstraint("GET") },
                namespaces: new[] { "AdvantShop.Areas.Api.Controllers" }
            );
            
            #endregion
            
            #region Settings 
            
            context.MapRoute(
                name: "Api_Settings_Get",
                url: "api/settings",
                defaults: new { controller = "Settings", action = "Get" },
                constraints: new { httpMethod = new HttpMethodConstraint("GET") },
                namespaces: new[] { "AdvantShop.Areas.Api.Controllers" }
            );
            
            #endregion
            
            #region Location

            context.MapRoute(
                name: "Api_Locations_GetCity",
                url: "api/locations/city",
                defaults: new { controller = "Locations", action = "GetCity" },
                constraints: new { httpMethod = new HttpMethodConstraint("GET") },
                namespaces: new[] { "AdvantShop.Areas.Api.Controllers" }
            );
            
            context.MapRoute(
                name: "Api_Locations_GetCountries",
                url: "api/locations/countries",
                defaults: new { controller = "Locations", action = "GetCountries" },
                constraints: new { httpMethod = new HttpMethodConstraint("POST") },
                namespaces: new[] { "AdvantShop.Areas.Api.Controllers" }
            );

            #endregion
            
            #region Carousels 
            
            context.MapRoute(
                name: "Api_Carousels_Get",
                url: "api/carousels",
                defaults: new { controller = "Carousels", action = "Get" },
                constraints: new { httpMethod = new HttpMethodConstraint("GET") },
                namespaces: new[] { "AdvantShop.Areas.Api.Controllers" }
            );
            
            #endregion
            
            #region Orders
            
            context.MapRoute(
                name: "Api_Orders_MeId_Get",
                url: "api/orders/me/{id}",
                defaults: new { controller = "Orders", action = "MeGetById" },
                constraints: new { httpMethod = new HttpMethodConstraint("GET") },
                namespaces: new[] { "AdvantShop.Areas.Api.Controllers" }
            );
            
            context.MapRoute(
                name: "Api_Orders_MeId_AddReview",
                url: "api/orders/me/{id}/review",
                defaults: new { controller = "Orders", action = "MeAddOrderReview" },
                constraints: new { httpMethod = new HttpMethodConstraint("POST") },
                namespaces: new[] { "AdvantShop.Areas.Api.Controllers" }
            );
            
            context.MapRoute(
                name: "Api_Orders_Me_CancelOrder",
                url: "api/orders/me/{id}/cancel",
                defaults: new { controller = "Orders", action = "MeCancelOrder" },
                constraints: new { httpMethod = new HttpMethodConstraint("POST") },
                namespaces: new[] { "AdvantShop.Areas.Api.Controllers" }
            );
            
            #endregion

            #region Users

            context.MapRoute(
                name: "Api_Users_Me_Get",
                url: "api/users/me",
                defaults: new { controller = "Users", action = "Me", id = UrlParameter.Optional },
                constraints: new { httpMethod = new HttpMethodConstraint("GET") },
                namespaces: new[] { "AdvantShop.Areas.Api.Controllers" }
            );
            
            context.MapRoute(
                name: "Api_Users_Me_Update",
                url: "api/users/me",
                defaults: new { controller = "Users", action = "ChangeMe", id = UrlParameter.Optional },
                constraints: new { httpMethod = new HttpMethodConstraint("POST") },
                namespaces: new[] { "AdvantShop.Areas.Api.Controllers" }
            );
            
            context.MapRoute(
                name: "Api_Users_Me_Bonuses",
                url: "api/users/me/bonuses",
                defaults: new { controller = "Users", action = "MeBonuses", id = UrlParameter.Optional },
                constraints: new { httpMethod = new HttpMethodConstraint("GET") },
                namespaces: new[] { "AdvantShop.Areas.Api.Controllers" }
            );
            
            context.MapRoute(
                name: "Api_Users_Me_SmsPhoneConfirmation",
                url: "api/users/me/smsphoneconfirmation",
                defaults: new { controller = "Users", action = "MeSmsPhoneConfirmation", id = UrlParameter.Optional },
                constraints: new { httpMethod = new HttpMethodConstraint("POST") },
                namespaces: new[] { "AdvantShop.Areas.Api.Controllers" }
            );
            
            context.MapRoute(
                name: "Api_Users_Me_SmsPhoneConfirmationCode",
                url: "api/users/me/smsphoneconfirmationcode",
                defaults: new { controller = "Users", action = "MeSmsPhoneConfirmationCode", id = UrlParameter.Optional },
                constraints: new { httpMethod = new HttpMethodConstraint("POST") },
                namespaces: new[] { "AdvantShop.Areas.Api.Controllers" }
            );
            
            context.MapRoute(
                name: "Api_Users_Me_CustomerFields",
                url: "api/users/me/customer-fields",
                defaults: new { controller = "Users", action = "MeCustomerFields", id = UrlParameter.Optional },
                constraints: new { httpMethod = new HttpMethodConstraint("GET") },
                namespaces: new[] { "AdvantShop.Areas.Api.Controllers" }
            );
            
            context.MapRoute(
                name: "Api_Users_Me_RemoveAccount",
                url: "api/users/me/remove-account",
                defaults: new { controller = "Users", action = "MeRemoveAccount" },
                constraints: new { httpMethod = new HttpMethodConstraint("POST") },
                namespaces: new[] { "AdvantShop.Areas.Api.Controllers" }
            );
            
            context.MapRoute(
                name: "Api_Users_Me_UpdateFcmToken",
                url: "api/users/me/fcmtoken",
                defaults: new { controller = "Users", action = "MeUpdateFcmToken" },
                constraints: new { httpMethod = new HttpMethodConstraint("POST") },
                namespaces: new[] { "AdvantShop.Areas.Api.Controllers" }
            );
            
            context.MapRoute(
                name: "Api_Users_Me_Contacts_Get",
                url: "api/users/me/contacts",
                defaults: new { controller = "Users", action = "MeGetContacts", id = UrlParameter.Optional },
                constraints: new { httpMethod = new HttpMethodConstraint("GET") },
                namespaces: new[] { "AdvantShop.Areas.Api.Controllers" }
            );
            
            context.MapRoute(
                name: "Api_Users_Me_Contacts_Add",
                url: "api/users/me/contacts/add",
                defaults: new { controller = "Users", action = "MeAddContact" },
                constraints: new { httpMethod = new HttpMethodConstraint("POST") },
                namespaces: new[] { "AdvantShop.Areas.Api.Controllers" }
            );
            
            context.MapRoute(
                name: "Api_Users_Me_Contacts_Update",
                url: "api/users/me/contacts/{id}/update",
                defaults: new { controller = "Users", action = "MeUpdateContact" },
                constraints: new { httpMethod = new HttpMethodConstraint("POST") },
                namespaces: new[] { "AdvantShop.Areas.Api.Controllers" }
            );
            
            context.MapRoute(
                name: "Api_Users_Me_Contacts_Delete",
                url: "api/users/me/contacts/{id}/delete",
                defaults: new { controller = "Users", action = "MeDeleteContact" },
                constraints: new { httpMethod = new HttpMethodConstraint("POST") },
                namespaces: new[] { "AdvantShop.Areas.Api.Controllers" }
            );
            
            context.MapRoute(
                name: "Api_Users_Me_Statistics",
                url: "api/users/me/statistics",
                defaults: new { controller = "Users", action = "MeStatistics" },
                constraints: new { httpMethod = new HttpMethodConstraint("GET") },
                namespaces: new[] { "AdvantShop.Areas.Api.Controllers" }
            );

            #endregion
            
            #region Products

            context.MapRoute(
                name: "Api_Products_Get",
                url: "api/products/{id}",
                defaults: new { controller = "Products", action = "Get" },
                constraints: new { httpMethod = new HttpMethodConstraint("GET") },
                namespaces: new[] { "AdvantShop.Areas.Api.Controllers" }
            );
            
            context.MapRoute(
                name: "Api_Products_Price",
                url: "api/products/{id}/price",
                defaults: new { controller = "Products", action = "Price" },
                constraints: new { httpMethod = new HttpMethodConstraint("POST") },
                namespaces: new[] { "AdvantShop.Areas.Api.Controllers" }
            );
            
            context.MapRoute(
                name: "Api_Products_Properties",
                url: "api/products/{id}/properties",
                defaults: new { controller = "Products", action = "Properties" },
                constraints: new { httpMethod = new HttpMethodConstraint("GET") },
                namespaces: new[] { "AdvantShop.Areas.Api.Controllers" }
            );
            
            context.MapRoute(
                name: "Api_Products_Reviews",
                url: "api/products/{id}/reviews",
                defaults: new { controller = "Products", action = "Reviews" },
                constraints: new { httpMethod = new HttpMethodConstraint("GET") },
                namespaces: new[] { "AdvantShop.Areas.Api.Controllers" }
            );
            
            context.MapRoute(
                name: "Api_Products_Reviews_Add",
                url: "api/products/{id}/reviews/add",
                defaults: new { controller = "Products", action = "AddReview" },
                constraints: new { httpMethod = new HttpMethodConstraint("POST") },
                namespaces: new[] { "AdvantShop.Areas.Api.Controllers" }
            );
            
            context.MapRoute(
                name: "Api_Products_RelatedProducts",
                url: "api/products/{id}/related-products",
                defaults: new { controller = "Products", action = "RelatedProducts" },
                constraints: new { httpMethod = new HttpMethodConstraint("GET") },
                namespaces: new[] { "AdvantShop.Areas.Api.Controllers" }
            );
            
            context.MapRoute(
                name: "Api_Products_Gifts",
                url: "api/products/{id}/gifts",
                defaults: new { controller = "Products", action = "Gifts" },
                constraints: new { httpMethod = new HttpMethodConstraint("GET") },
                namespaces: new[] { "AdvantShop.Areas.Api.Controllers" }
            );
            
            context.MapRoute(
                name: "Api_Products_Stocks",
                url: "api/products/{id}/stocks",
                defaults: new { controller = "Products", action = "Stocks" },
                constraints: new { httpMethod = new HttpMethodConstraint("GET") },
                namespaces: new[] { "AdvantShop.Areas.Api.Controllers" }
            );
            
            context.MapRoute(
                name: "Api_Products_PriceRuleAmountList",
                url: "api/products/{id}/price-rule-amount-list",
                defaults: new { controller = "Products", action = "PriceRuleAmountList" },
                constraints: new { httpMethod = new HttpMethodConstraint("GET") },
                namespaces: new[] { "AdvantShop.Areas.Api.Controllers" }
            );
            
            #endregion
            
            #region Catalog
            
            context.MapRoute(
                name: "Api_Catalog_All",
                url: "api/catalog/all",
                defaults: new { controller = "Catalog", action = "All" },
                constraints: new { httpMethod = new HttpMethodConstraint("POST") },
                namespaces: new[] { "AdvantShop.Areas.Api.Controllers" }
            );
            
            context.MapRoute(
                name: "Api_Catalog_GetFilter",
                url: "api/catalog/filter",
                defaults: new { controller = "Catalog", action = "GetFilter" },
                constraints: new { httpMethod = new HttpMethodConstraint("POST") },
                namespaces: new[] { "AdvantShop.Areas.Api.Controllers" }
            );

            context.MapRoute(
                name: "Api_Catalog_GetFilterCount",
                url: "api/catalog/filtercount",
                defaults: new { controller = "Catalog", action = "GetFilterCount" },
                constraints: new { httpMethod = new HttpMethodConstraint("POST") },
                namespaces: new[] { "AdvantShop.Areas.Api.Controllers" }
            );

            #endregion
            
            #region Search
            
            context.MapRoute(
                name: "Api_Search_GetFilter",
                url: "api/search/filter",
                defaults: new { controller = "Search", action = "GetFilter" },
                constraints: new { httpMethod = new HttpMethodConstraint("POST") },
                namespaces: new[] { "AdvantShop.Areas.Api.Controllers" }
            );

            context.MapRoute(
                name: "Api_Search_GetFilterCount",
                url: "api/search/filtercount",
                defaults: new { controller = "Search", action = "GetFilterCount" },
                constraints: new { httpMethod = new HttpMethodConstraint("POST") },
                namespaces: new[] { "AdvantShop.Areas.Api.Controllers" }
            );
            
            context.MapRoute(
                name: "Api_Search_Autocomplete",
                url: "api/search/autocomplete",
                defaults: new { controller = "Search", action = "Autocomplete" },
                constraints: new { httpMethod = new HttpMethodConstraint("POST") },
                namespaces: new[] { "AdvantShop.Areas.Api.Controllers" }
            );

            #endregion

            #region Cart
            
            context.MapRoute(
                name: "Api_Cart_GetCurrentCart",
                url: "api/cart",
                defaults: new { controller = "Cart", action = "GetCurrentCart" },
                constraints: new { httpMethod = new HttpMethodConstraint("GET") },
                namespaces: new[] { "AdvantShop.Areas.Api.Controllers" }
            );
            
            context.MapRoute(
                name: "Api_Cart_Get",
                url: "api/cart",
                defaults: new { controller = "Cart", action = "Get" },
                constraints: new { httpMethod = new HttpMethodConstraint("POST") },
                namespaces: new[] { "AdvantShop.Areas.Api.Controllers" }
            );

            #endregion
            
            #region Coupons
            
            context.MapRoute(
                name: "Api_Coupons_MeAdd",
                url: "api/coupons/me/add",
                defaults: new { controller = "Coupons", action = "MeAdd" },
                constraints: new { httpMethod = new HttpMethodConstraint("POST") },
                namespaces: new[] { "AdvantShop.Areas.Api.Controllers" }
            );
            
            context.MapRoute(
                name: "Api_Coupons_MeRemove",
                url: "api/coupons/me/remove",
                defaults: new { controller = "Coupons", action = "MeRemove" },
                constraints: new { httpMethod = new HttpMethodConstraint("POST") },
                namespaces: new[] { "AdvantShop.Areas.Api.Controllers" }
            );

            #endregion

            #region Deliveries

            context.MapRoute(
                name: "Api_Deliveries_Types",
                url: "api/deliveries/types",
                defaults: new { controller = "Deliveries", action = "GetShippingTypes" },
                constraints: new { httpMethod = new HttpMethodConstraint("GET") },
                namespaces: new[] { "AdvantShop.Areas.Api.Controllers" }
            );

            context.MapRoute(
                name: "Api_Deliveries_CheckShippingByDeliveryZones",
                url: "api/deliveries/check-delivery-zone",
                defaults: new { controller = "Deliveries", action = "CheckShippingByDeliveryZones" },
                constraints: new { httpMethod = new HttpMethodConstraint("POST") },
                namespaces: new[] { "AdvantShop.Areas.Api.Controllers" }
            );
            
            context.MapRoute(
                name: "Api_Deliveries_Calculate",
                url: "api/deliveries/calculate",
                defaults: new { controller = "Deliveries", action = "CalculateDeliveries" },
                constraints: new { httpMethod = new HttpMethodConstraint("POST") },
                namespaces: new[] { "AdvantShop.Areas.Api.Controllers" }
            );
            
            context.MapRoute(
                name: "Api_Deliveries_GetPointDeliveries",
                url: "api/deliveries/point-deliveries",
                defaults: new { controller = "Deliveries", action = "GetPointDeliveries" },
                constraints: new { httpMethod = new HttpMethodConstraint("GET") },
                namespaces: new[] { "AdvantShop.Areas.Api.Controllers" }
            );

            #endregion
            
            #region Checkout

            context.MapRoute(
                name: "Api_Checkout_Get",
                url: "api/checkout",
                defaults: new { controller = "Checkout", action = "Get", area = AreaName },
                constraints: new { httpMethod = new HttpMethodConstraint("POST") },
                namespaces: new[] { "AdvantShop.Areas.Api.Controllers" }
            );

            #endregion
            
            #region Wish list

            context.MapRoute(
                name: "Api_WishList_Get",
                url: "api/wishlist",
                defaults: new { controller = "WishList", action = "GetList", area = AreaName },
                constraints: new { httpMethod = new HttpMethodConstraint("GET") },
                namespaces: new[] { "AdvantShop.Areas.Api.Controllers" }
            );
            
            context.MapRoute(
                name: "Api_WishList_Add",
                url: "api/wishlist/add",
                defaults: new { controller = "WishList", action = "Add", area = AreaName },
                constraints: new { httpMethod = new HttpMethodConstraint("POST") },
                namespaces: new[] { "AdvantShop.Areas.Api.Controllers" }
            );
            
            context.MapRoute(
                name: "Api_WishList_Remove",
                url: "api/wishlist/remove",
                defaults: new { controller = "WishList", action = "Remove", area = AreaName },
                constraints: new { httpMethod = new HttpMethodConstraint("POST") },
                namespaces: new[] { "AdvantShop.Areas.Api.Controllers" }
            );

            #endregion

            #region Static blocks

            context.MapRoute(
                name: "Api_StaticBlocks_Get",
                url: "api/staticblocks",
                defaults: new { controller = "StaticBlocks", action = "GetList", area = AreaName },
                constraints: new { httpMethod = new HttpMethodConstraint("GET") },
                namespaces: new[] { "AdvantShop.Areas.Api.Controllers" }
            );
            
            #endregion
            
            #region Static Pages

            context.MapRoute(
                name: "Api_StaticPages_GetStaticPage",
                url: "api/staticpages/{id}",
                defaults: new { controller = "StaticPages", action = "GetStaticPage" },
                constraints: new { httpMethod = new HttpMethodConstraint("GET") },
                namespaces: new[] { "AdvantShop.Areas.Api.Controllers" }
            );

            context.MapRoute(
                name: "Api_StaticPages_GetList",
                url: "api/staticpages",
                defaults: new { controller = "StaticPages", action = "GetList" },
                constraints: new { httpMethod = new HttpMethodConstraint("GET") },
                namespaces: new[] { "AdvantShop.Areas.Api.Controllers" }
            );

            #endregion
            
            #region Modules
            
            context.MapRoute(
                name: "Api_Modules_Get",
                url: "api/modules/{id}/block",
                defaults: new { controller = "Modules", action = "GetBlock" },
                constraints: new { httpMethod = new HttpMethodConstraint("GET") },
                namespaces: new[] { "AdvantShop.Areas.Api.Controllers" }
            );
            
            #endregion
            
            #region Preorder
            
            context.MapRoute(
                name: "Api_Preorder",
                url: "api/preorder",
                defaults: new { controller = "Preorder", action = "Preorder" },
                constraints: new { httpMethod = new HttpMethodConstraint("POST") },
                namespaces: new[] { "AdvantShop.Areas.Api.Controllers" }
            );
            
            context.MapRoute(
                name: "Api_Preorder_GetSettings",
                url: "api/preorder/settings",
                defaults: new { controller = "Preorder", action = "GetSettings" },
                constraints: new { httpMethod = new HttpMethodConstraint("GET") },
                namespaces: new[] { "AdvantShop.Areas.Api.Controllers" }
            );

            #endregion
            
            #region v2
                
            context.MapRoute(
                name: "Api_v2_Deliveries_Deliveries",
                url: "api/v2/deliveries",
                defaults: new { controller = "Deliveries", action = "GetDeliveries" },
                constraints: new { httpMethod = new HttpMethodConstraint("POST") },
                namespaces: new[] { "AdvantShop.Areas.Api.Controllers.v2" }
            );
            
            #endregion    

            context.MapRoute(
                name: "Api_Default_v2",
                url: "api/v2/{controller}/{action}/{id}",
                defaults: new {controller = "Home", action = "Index", id = UrlParameter.Optional},
                namespaces: new[] {"AdvantShop.Areas.Api.Controllers.v2"}
            );
            
            context.MapRoute(
                name: "Api_Default",
                url: "api/{controller}/{action}/{id}",
                defaults: new {controller = "Home", action = "Index", id = UrlParameter.Optional},
                namespaces: new[] {"AdvantShop.Areas.Api.Controllers"}
            );
        }
    }
}