update settings.settings set value = 'http://mydomain123.by' where name = 'ShopURL'
update settings.settings set value = '21' where name = 'SellerCountryId'
update settings.settings set value = '164' where name = 'SellerRegionId'
update settings.settings set value = '+375 (00) 000-00-00' where name = 'Phone'
update settings.settings set value = '+375000000000' where name = 'MobilePhone'
update settings.settings set value = 'BYN' where name = 'DefaultCurrencyISO3'
update settings.settings set value = 'Минск' where name = 'City'


update catalog.currency set [CurrencyIso3] = 'BYN', [CurrencyNumIso3] = 933 where CurrencyID = 1
update catalog.currency set [CurrencyValue] = 3 where CurrencyID = 2
update catalog.currency set [CurrencyValue] = 3.5 where CurrencyID = 3


update catalog.Offer set Price = Price/10
update Catalog.ProductExt set MinPrice = MinPrice/10



update [CMS].[StaticPage] set [PageText] = replace([PageText], 'Почтой России', 'Почтой')
update [CMS].[StaticPage] set [PageText] = replace([PageText], 'Почты России', 'Почты')
update [CMS].[StaticPage] set [PageText] = replace([PageText], ' от Москвы', '')
update [CMS].[StaticPage] set [PageText] = replace([PageText], '+7 (495) 000-00-00', '+375 (00) 000-00-00')
update [CMS].[StaticPage] set [PageText] = replace([PageText], 'Москва', 'Минск')
update [CMS].[StaticPage] set [PageText] = replace([PageText], 'myshop.ru', 'myshop.by')
update [CMS].[StaticPage] set [PageText] = replace([PageText], 'ИНН', 'УНП')
update [CMS].[StaticPage] set [PageText] = replace([PageText], 'КПП 4657025431', '')
