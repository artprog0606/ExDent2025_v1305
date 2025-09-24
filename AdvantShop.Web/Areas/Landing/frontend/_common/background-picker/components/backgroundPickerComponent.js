import backgroundPickerTemplate from './../templates/backgroundPicker.html';
(function (ng) {
    'use strict';

    ng.module('backgroundPicker').component('backgroundPicker', {
        templateUrl: backgroundPickerTemplate,
        controller: 'BackgroundPickerCtrl',
        bindings: {
            onUpdate: '&',
            colors: '<',
            colorSelected: '<?',
            onInit: '&',
        },
    });
})(window.angular);
