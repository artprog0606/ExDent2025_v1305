import '../../styles/views/wishlist.scss';

import productViewModule from '../_partials/product-view/productView.module.js';
import WishlistPageCtrl from './controllers/wishlistPageController.js';
import WishlistPageService from '../_partials/wishlist/services/wishlistService.ts';

const moduleName = 'wishlistPage';

angular.module(moduleName, [productViewModule]).service('wishlistService', WishlistPageService).controller('WishlistPageCtrl', WishlistPageCtrl);

export default moduleName;
