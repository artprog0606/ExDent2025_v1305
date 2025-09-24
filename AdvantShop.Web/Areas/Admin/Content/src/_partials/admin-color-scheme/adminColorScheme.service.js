(function (ng) {
    'use strict';

    var adminColorSchemeService = function () {
        var service = this;

        service.memoryStylesheet = function (element) {
            service.element = element;
        };

        service.change = function (colorScheme) {
            service.element[0].href = service.element[0].href.replace(/\/color-schemes\/([\d\w\s_-]*)\//, '/color-schemes/' + colorScheme + '/');
        };
    };

    ng.module('adminColorScheme').service('adminColorSchemeService', adminColorSchemeService);

    adminColorSchemeService.$inject = [];
})(window.angular);
