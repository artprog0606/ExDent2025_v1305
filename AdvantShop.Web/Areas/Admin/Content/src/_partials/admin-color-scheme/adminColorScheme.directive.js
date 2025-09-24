(function (ng) {
    'use strict';

    ng.module('adminColorScheme', []).directive('adminColorScheme', [
        'adminColorSchemeService',
        function (adminColorSchemeService) {
            return {
                restrict: 'A',
                scope: {},
                link: function (scope, element) {
                    adminColorSchemeService.memoryStylesheet(element);
                },
            };
        },
    ]);
})(window.angular);
