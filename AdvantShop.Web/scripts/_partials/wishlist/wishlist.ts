import WishlistService from './services/wishlistService';
import WishlistControlCtrl from './controllers/WishlistControlController';
import WishlistCountCtrl from './controllers/wishlistCountController';

const moduleName = `wishlist`;
angular
    .module(moduleName, [])
    .controller('WishlistCountCtrl', WishlistCountCtrl)
    .controller('WishlistControlCtrl', WishlistControlCtrl)
    .service('wishlistService', WishlistService);
export default moduleName;
