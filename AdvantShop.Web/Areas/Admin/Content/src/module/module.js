(function (ng) {
    'use strict';
    window.addEventListener('load', function () {
        var portBtns = document.querySelectorAll('.trigger-port');
        if (portBtns.length != 0) {
            for (var i = 0; i < portBtns.length; i++) {
                portBtns[i].data = i;
            }
        }
    });

    var ModuleCtrl = function ($http, $location, toaster, $translate, SweetAlert, isMobileService) {
        var ctrl = this;

        ctrl.isMobile = isMobileService.getValue();

        ctrl.$onInit = function () {
            var urlSearch = $location.search();

            if (urlSearch != null && urlSearch.moduleTab != null) {
                ctrl.tab = parseFloat(urlSearch.moduleTab);
            } else {
                ctrl.tab = 0;
            }
        };

        ctrl.onInit = function () {
            ctrl.activeImport = false;
        };

        ctrl.changeEnabled = function (state, name) {
            $http.post('modules/changeEnabled', { stringId: name, enabled: state }).then(function (response) {
                if (response.data.result === true) {
                    if (!response.data.obj.SaasAndPaid || state) {
                        toaster.pop(
                            'success',
                            '',
                            state ? $translate.instant('Admin.Js.Module.ModuleIsActivated') : $translate.instant('Admin.Js.Module.ModuleIsNotActive'),
                        );
                    } else {
                        SweetAlert.info('', {
                            title: '',
                            html: $translate.instant('Admin.Js.Modules.DeactivatedAndPayable'),
                        });
                    }
                } else {
                    ctrl.enabled = false;
                    toaster.pop('error', '', $translate.instant('Admin.Js.Module.ErrorWhileSaving'));
                }
            });
        };

        ctrl.setTab = function (newTab) {
            ctrl.tab = newTab;
            //remove all search params
            $location.search({});
            $location.search('moduleTab', newTab);
        };

        ctrl.isSet = function (tabNum) {
            return ctrl.tab === tabNum;
        };
    };

    ModuleCtrl.$inject = ['$http', '$location', 'toaster', '$translate', 'SweetAlert', 'isMobileService'];

    ng.module('module', ['uiModal', 'productsSelectvizr', 'pictureUploader', 'isMobile', 'color.picker', 'as.sortable']).controller(
        'ModuleCtrl',
        ModuleCtrl,
    );
})(window.angular);
