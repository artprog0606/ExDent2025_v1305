(function (ng) {
    'use strict';

    ng.module('shopName', []).directive(
        'shopName',
        /* @ngInject */
        function ($sce, $parse, $filter) {
            return {
                scope: true,
                link: function (scope, element, attrs) {
                    const callback = $parse(attrs.shopNameCallback);
                    const decodeStringFilter = $filter('decodeString');
                    if (decodeStringFilter) {
                        scope.shopName = $sce.trustAsHtml(decodeStringFilter(element[0].textContent.trim()));
                    }
                    if (callback) {
                        callback(scope);
                    }
                },
            };
        },
    );
})(window.angular);
