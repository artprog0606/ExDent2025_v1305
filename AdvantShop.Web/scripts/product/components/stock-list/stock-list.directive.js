import stockListTemplate from './stock-list.html';

export default function StockListDirective() {
    return {
        scope: {
            offerId: '<?',
            isMobile: '<',
        },
        controller: [
            'productService',
            function (productService) {
                const ctrl = this;
                ctrl.$onInit = () => {
                    ctrl.isLoadingStocks = true;
                };
                ctrl.getStockList = (offerId) => {
                    productService
                        .getOfferStocks(offerId)
                        .then((data) => {
                            if (data.result === true) {
                                ctrl.stockListData = data.obj.Stocks;
                            } else {
                                data.errors.forEach(function (error) {
                                    console.error(error);
                                });
                            }
                        })
                        .finally(() => {
                            ctrl.isLoadingStocks = false;
                        });
                };
            },
        ],
        bindToController: true,
        controllerAs: '$ctrl',
        templateUrl: stockListTemplate,
        link: (scope, element, attr, ctrl) => {
            scope.$watch('$ctrl.offerId', (newValue, oldValue, scope) => {
                if (newValue != null && oldValue != null) {
                    ctrl.getStockList(newValue);
                }
            });
        },
    };
}
