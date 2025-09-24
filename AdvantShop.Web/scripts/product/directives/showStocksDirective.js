/* @ngInject */
function showStocksDirective(isMobileService) {
    return {
        restrict: 'A',
        controller: function () {},
        bindToController: true,
        link: function (scope, element, attrs, ctrl) {
            element.on('click', function () {
                let isMobile = isMobileService.getValue();
                let stocksTab;

                if (isMobile) {
                    stocksTab = document.querySelector('.product-stocks-data .product-data__header label');
                } else {
                    stocksTab = document.querySelector('#tabStocks .tabs-header-item-link');
                }

                if (stocksTab) {
                    stocksTab.click();
                    stocksTab.scrollIntoView();
                }
            });
        },
    };
}

export { showStocksDirective };
