import gradientPickerTemplate from './../templates/gradientPicker.html';
(function (ng) {
    'use strict';

    ng.module('gradientPicker').component('gradientPicker', {
        templateUrl: gradientPickerTemplate,
        controller: 'GradientPickerCtrl',
        bindings: {
            onUpdate: '&',
            startColor: '<',
            middleColor: '<',
            endColor: '<',
        },
    });
})(window.angular);
