import partnerInfoTemplate from './templates/partner-info.html';
import partnerInfoContainerTemplate from './templates/partner-info-container.html';
(function (ng) {
    'use strict';

    ng.module('partnerInfo')
        .component('partnerInfoContainer', {
            templateUrl: partnerInfoContainerTemplate,
            controller: 'PartnerInfoContainerCtrl',
        })
        .component('partnerInfo', {
            templateUrl: partnerInfoTemplate,
            controller: 'PartnerInfoCtrl',
            bindings: {
                close: '&',
                dismiss: '&',
                resolve: '<',
            },
        });
})(window.angular);
