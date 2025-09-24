(function (ng) {
    'use strict';

    /* @ngInject */
    var LeadInfoCtrl = function ($uibModalStack, leadInfoService, $window) {
        var ctrl = this;

        ctrl.$onInit = function () {
            ctrl.leadId = ctrl.resolve.params.leadId;

            ctrl.scrollTop = $window.pageYOffset;

            ctrl.url = ctrl.getLeadInfoUrl();

            ctrl.isShow = true;

            leadInfoService.setUrlParam(ctrl.leadId);
        };

        ctrl.getLeadInfoUrl = function () {
            return 'leads/popup?rnd=' + Math.random() + '&id=' + ctrl.leadId;
        };

        ctrl.closeLeadInfo = function () {
            ctrl.isShow = false;

            ctrl.close({ $value: ctrl });
        };

        ctrl.updateLeadInfo = function () {
            ctrl.isLoaded = false;
            ctrl.url = ctrl.getLeadInfoUrl();
        };
    };

    ng.module('leadInfo', [])
        .run([
            'leadInfoService',
            function (leadInfoService) {
                const urlParam = leadInfoService.getUrlParam();
                if (urlParam != null) {
                    leadInfoService.addInstance({ leadId: urlParam });
                }
            },
        ])
        .controller('LeadInfoCtrl', LeadInfoCtrl);
})(window.angular);
