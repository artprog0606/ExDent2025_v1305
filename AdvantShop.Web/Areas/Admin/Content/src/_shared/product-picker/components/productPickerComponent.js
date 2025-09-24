import productPickerTemplate from './../templates/productPicker.html';
(function (ng) {
    'use strict';

    ng.module('productPicker').component('productPicker', {
        templateUrl: productPickerTemplate,
        controller: 'ProductPickerCtrl',
        bindings: {
            treeUrl: '@',
            gridUrl: '@',
            treeConfig: '<?',
            gridConfig: '<?',
        },
    });
})(window.angular);
