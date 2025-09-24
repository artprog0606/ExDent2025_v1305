class WishlistPageCtrl {
    /* @ngInject */
    constructor(wishlistService) {
        this.wishlistService = wishlistService;
    }

    $onInit() {
        this.countObj = this.wishlistService.getCountObj();
    }
}

export default WishlistPageCtrl;
