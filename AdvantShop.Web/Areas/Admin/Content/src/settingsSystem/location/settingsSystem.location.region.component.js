import regionTemplate from './region.html';
(function (ng) {
    'use strict';

    ng.module('settingsSystem').component('gridRegion', {
        templateUrl: regionTemplate,
        controller: 'SettingsSystemLocationRegionCtrl',
        bindings: {
            onGridInit: '&',
            onSelect: '&',
            gridParams: '<?',
        },
    });
})(window.angular);
