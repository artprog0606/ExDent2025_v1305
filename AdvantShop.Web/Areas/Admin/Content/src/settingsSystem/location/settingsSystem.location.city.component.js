import cityTemplate from './city.html';
(function (ng) {
    'use strict';

    ng.module('settingsSystem').component('gridCity', {
        templateUrl: cityTemplate,
        controller: 'SettingsSystemLocationCityCtrl',
        bindings: {
            onGridInit: '&',
            onSelect: '&',
            gridParams: '<?',
            onGridPreinit: '&',
        },
    });
})(window.angular);
