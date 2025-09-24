let list = new Map();
/**
 * ключ - название бандла
 * значение - строка или массив строк url (в случае вовзрата статус кода 404 берется следующий url)
 */
list.set('brand', 'brand');
list.set('cart', 'cart');
list.set('catalog', [
    'categories/kategoriya-1?viewmode=Tile',
    'categories/kategoriya-1?viewmode=List',
    'categories/kategoriya-1?viewmode=Table',
    'categories/kategoriya-1?viewmode=Single',
]);
list.set('catalogsearch', 'search?q=%D0%BF%D0%BB%D0%B0%D1%82%D1%8C%D0%B5');
list.set('checkout', 'checkout');
list.set('compare', 'compare');
list.set('error', { url: '404', ignoreStatusCodeError: true });
list.set('feedback', 'feedback');
list.set('giftcertificate', 'giftcertificate');
list.set('forgotpassword', 'forgotpassword');
list.set('home', ['?mainPageMode=Default', '?mainPageMode=TwoColumns']);
list.set('login', 'login');
list.set('myaccount', { url: 'myaccount', admin: true });
list.set('news', 'news');
list.set('registration', 'registration');
list.set('product', ['products/rubashka', 'products/chasy']);
list.set('productlist', ['productlist/best', 'productlist/new', 'productlist/sale', 'productlist/newarrivals']);
list.set('staticpage', 'pages/contacts');
list.set('wishlist', 'wishlist');

export { list };
