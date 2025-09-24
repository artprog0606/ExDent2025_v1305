import countryTemplate from './country.html';
(function (ng) {
    'use strict';

    ng.module('settingsSystem').component('gridCountry', {
        templateUrl: countryTemplate,
        controller: 'SettingsSystemLocationCountryCtrl',
        bindings: {
            onGridInit: '&',
            onSelect: '&',
            gridParams: '<?',
        },
    });
})(window.angular);
