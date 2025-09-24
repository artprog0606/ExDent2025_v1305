(function (ng) {
    'use strict';

    ng.module('catalogFilter', []).constant('catalogFilterAdvPopoverOptionsDefault', {
        position: 'left',
        isFixed: false,
        showOnLoad: false,
        overlayEnabled: false,
    });
})(window.angular);
