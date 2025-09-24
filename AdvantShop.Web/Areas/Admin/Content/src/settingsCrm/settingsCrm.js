(function (ng) {
    'use strict';

    var SettingsCrmCtrl = function (toaster, $translate, $window, leadsService, $location, isMobileService, settingsCrmService) {
        var ctrl = this;

        ctrl.$onInit = function () {
            //ctrl.getSaasData();
            ctrl.getSalesFunnels();
            ctrl.getOrderStatuses();
        };

        ctrl.salesFunnelsOnInit = function (salesFunnels) {
            ctrl.salesFunnels = salesFunnels;
        };

        ctrl.getSalesFunnels = function () {
            settingsCrmService.getSalesFunnels().then(function (response) {
                return (ctrl.funnels = response.data);
            });
        };

        ctrl.getOrderStatuses = function () {
            settingsCrmService.getOrderStatuses().then(function (response) {
                return (ctrl.statuses = response.data);
            });
        };

        ctrl.updateFunnels = function () {
            ctrl.getSalesFunnels();

            leadsService.updateList();
        };

        ctrl.saveDefaultSalesFunnelId = function () {
            settingsCrmService.saveDefaultSalesFunnelId(ctrl.DefaultSalesFunnelId).then(function (response) {
                if (response.data.result) {
                    toaster.success('', $translate.instant('Admin.Js.SettingsCrm.ChangesSuccessfullySaved'));
                }
            });
        };

        ctrl.saveOrderStatusIdFromLead = function () {
            settingsCrmService.saveOrderStatusIdFromLead(ctrl.OrderStatusIdFromLead).then(function (response) {
                if (response.data.result) {
                    toaster.success('', $translate.instant('Admin.Js.SettingsCrm.ChangesSuccessfullySaved'));
                }
            });
        };

        ctrl.setCrmActive = function (active) {
            if (isMobileService.getValue()) {
                ctrl.crmActive = active;
            } else {
                settingsCrmService
                    .setCrmActive(active)
                    .then(function (response) {
                        if (response.data.result) {
                            toaster.success('', $translate.instant('Admin.Js.SettingsCrm.ChangesSuccessfullySaved'));
                        }
                    })
                    .then(function (res) {
                        $window.location.reload();
                    });
            }
        };

        ctrl.pushForm = function () {
            settingsCrmService
                .setCrmActive(ctrl.crmActive)
                .then(function (response) {
                    if (response.data.result) {
                        toaster.success('', $translate.instant('Admin.Js.SettingsCrm.ChangesSuccessfullySaved'));
                    }
                })
                .then(function (res) {
                    $window.location.reload();
                });
        };

        //ctrl.getSaasData = function () {
        //    return $http.get('settingsCrm/getIntegrationsData').then(function (response) {
        //        if (response.data != null) {
        //            ctrl.saasData = {
        //                limit: response.data.limit,
        //                count: response.data.count,
        //                limitRiched: response.data.limitRiched
        //            };
        //        }
        //    });
        //};

        ctrl.onSelectTab = function (indexTab) {
            ctrl.tabActiveIndex = indexTab;
        };
    };

    SettingsCrmCtrl.$inject = ['toaster', '$translate', '$window', 'leadsService', '$location', 'isMobileService', 'settingsCrmService'];

    ng.module('settingsCrm', [
        'dealStatuses',
        'as.sortable',
        'facebookAuth',
        'salesFunnels',
        'integrationsLimit',
        'leadFieldsList',
        'lead',
        'leads',
        'callRecord',
        'yaru22.angular-timeago',
        'import',
        'ngFileUpload',
        'color.picker',
        'fileUploader',
        'productsSelectvizr',
        'isMobile',
    ]).controller('SettingsCrmCtrl', SettingsCrmCtrl);
})(window.angular);
