/* @ngInject */
function PriceAmountListCtrl($http, $q) {
    let ctrl = this;

    ctrl.$postLink = function () {
        ctrl.getItems(ctrl.productId, ctrl.startOfferId);
        ctrl.initFn({ priceAmountList: ctrl });
    };

    ctrl.getItems = function (productId, offerId) {
        let defer = $q.defer();

        if (productId == null || offerId == null) {
            defer.resolve();
            return defer.promise;
        }

        $http.get('productExt/getPriceAmountList', { params: { productId: productId, offerId: offerId } }).then(function (response) {
            ctrl.items = response.data != null ? response.data.obj : null;
            defer.resolve(ctrl.items);
        });

        return defer.promise;
    };

    ctrl.update = function () {
        return ctrl.getItems(ctrl.productId, ctrl.offerId).then(function (data) {
            return data;
        });
    };
}

export default PriceAmountListCtrl;
