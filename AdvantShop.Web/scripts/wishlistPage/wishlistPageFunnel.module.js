import '../../styles/views/wishlist.scss';
import '../../scripts/_partials/product-view/styles/product-view.scss';

import WishlistPageCtrl from './controllers/wishlistPageController.js';
import WishlistPageService from '../_partials/wishlist/services/wishlistService.ts';

const moduleName = 'wishlistPage';

angular.module(moduleName, []).service('wishlistService', WishlistPageService).controller('WishlistPageCtrl', WishlistPageCtrl);

export default moduleName;
