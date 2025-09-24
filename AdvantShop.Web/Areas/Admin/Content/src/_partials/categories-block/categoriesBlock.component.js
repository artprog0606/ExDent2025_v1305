import categoriesBlockTemplate from './templates/categories-block.html';

(function (ng) {
    'use strict';

    ng.module('categoriesBlock').component('categoriesBlock', {
        templateUrl: categoriesBlockTemplate,
        controller: 'CategoriesBlockCtrl',
        transclude: true,
        bindings: {
            categoryId: '@',
            photoHeight: '@',
            categorysearch: '@',
            onDelete: '&',
        },
    });
})(window.angular);
