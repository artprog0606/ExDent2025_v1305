import settingsLocationTemplate from './settings-location.html';
(function (ng) {
    'use strict';

    ng.module('settingsSystem').component('settingsLocation', {
        templateUrl: settingsLocationTemplate,
        controller: 'SettingsSystemLocationCtrl',
        bindToController: true,
        bindings: {
            onInit: '&',
        },
    });
})(window.angular);
