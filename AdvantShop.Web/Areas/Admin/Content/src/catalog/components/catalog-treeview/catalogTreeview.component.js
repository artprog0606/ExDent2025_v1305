import catalogTreeviewTemplate from './templates/catalogTreeview.html';
(function (ng) {
    'use strict';

    ng.module('catalog').component('catalogTreeview', {
        templateUrl: catalogTreeviewTemplate,
        controller: 'CatalogTreeviewCtrl',
        bindings: {
            categoryIdSelected: '@',
            onInit: '&',
        },
    });
})(window.angular);
