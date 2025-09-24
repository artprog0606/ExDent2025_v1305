(function (ng) {
    'use strict';

    var SettingsTelephonyCtrl = function ($http, $q, $window, SweetAlert, toaster, $translate, settingsTelephonyService, $location, isMobileService) {
        var ctrl = this;

        ctrl.$onInit = function () {
            ctrl.getOrderSources().then(function () {
                ctrl.getPhoneOrderSources();
            });
        };

        ctrl.examinationPhoneOrderSources = function () {
            if (Object.keys(ctrl.phoneOrderSources).length == 0 || Object.keys(ctrl.phoneOrderSources).length == null) {
                ctrl.PhoneOrderSourcesHasChild = false;
            } else {
                ctrl.PhoneOrderSourcesHasChild = true;
            }
        };

        ctrl.copy = function (data) {
            var input = document.createElement('input');
            input.setAttribute('value', data);
            input.style.opacity = 0;
            document.body.appendChild(input);
            input.select();
            if (document.execCommand('copy')) {
                toaster.pop('success', $translate.instant('Admin.Js.SettingsTelephony.LinkCopiedToClipboard'));
            } else {
                toaster.pop('error', $translate.instant('Admin.Js.SettingsTelephony.FailedToCopyLink'));
            }
            document.body.removeChild(input);
        };

        // phone order sources

        ctrl.getOrderSources = function () {
            return $http.post('orders/getOrderSources').then(function (response) {
                ctrl.orderSources = response.data;
            });
        };

        ctrl.getPhoneOrderSources = function () {
            return settingsTelephonyService.getPhoneOrderSources().then(function (response) {
                ctrl.phoneOrderSources = response.data.obj;
                ctrl.examinationPhoneOrderSources();
            });
        };

        ctrl.savePhoneOrderSources = function (phone, orderSourceId) {
            if (phone) {
                ctrl.phoneOrderSources[phone] = orderSourceId;
            }

            return settingsTelephonyService.savePhoneOrderSources(ctrl.phoneOrderSources).then(function () {
                return ctrl.getPhoneOrderSources();
            });
        };

        ctrl.addPhone = function () {
            settingsTelephonyService.addPhone(ctrl.newPhone, ctrl.newOrderSourceId, ctrl.phoneOrderSources).then(function () {
                ctrl.newPhone = ctrl.newOrderSourceId = '';
            });
        };

        ctrl.deletePhone = function (phone) {
            SweetAlert.confirm($translate.instant('Admin.Js.SettingsTelephony.AreYouSureDelete'), {
                title: $translate.instant('Admin.Js.SettingsTelephony.Deleting'),
            }).then(function (result) {
                if (result === true || result.value === true) {
                    delete ctrl.phoneOrderSources[phone];
                    return ctrl.savePhoneOrderSources();
                }
            });
        };

        // telphin

        ctrl.getTelphinExtensions = function () {
            if (!ctrl.telphinAppKey || !ctrl.telphinAppSecret) {
                $window.document.getElementById(ctrl.telphinAppKey ? 'TelphinAppSecret' : 'TelphinAppKey').focus();
                toaster.error($translate.instant('Admin.Js.SettingsTelephony.SpecifyApplicationData'));
                return;
            }
            $http.post('settingsTelephony/getTelphinExtensions').then(function (response) {
                var data = response.data;
                if (data.result == true) {
                    ctrl.telphinExtensions = data.obj;
                    toaster.success($translate.instant('Admin.Js.SettingsTelephony.DataUpdated'));
                } else {
                    toaster.error($translate.instant('Admin.Js.SettingsTelephony.FailedToRetrieveData'));
                }
            });
        };

        ctrl.addTelphinEvents = function (ext) {
            $http.post('settingsTelephony/addTelphinEvents', { extensionId: ext.id }).then(function (response) {
                var data = response.data;
                if (data.result == true) {
                    ctrl.telphinExtensions = data.obj;
                    toaster.success(
                        $translate.instant('Admin.Js.SettingsTelephony.EventNotificationsForAddNumber') +
                            ext.extension +
                            $translate.instant('Admin.Js.SettingsTelephony.Installed'),
                    );
                } else {
                    data.errors.forEach(function (error) {
                        toaster.error($translate.instant('Admin.Js.SettingsTelephony.Error'), error);
                    });
                }
            });
        };

        ctrl.deleteTelphinEvents = function (ext) {
            SweetAlert.confirm($translate.instant('Admin.Js.SettingsTelephony.DeleteTelphinEvents'), {
                title: $translate.instant('Admin.Js.Deleting'),
            }).then(function (result) {
                if (result === true) {
                    $http.post('settingsTelephony/deleteTelphinEvents', { extensionId: ext.id }).then(function (response) {
                        var data = response.data;
                        if (data.result == true || data.result.value === true) {
                            ctrl.telphinExtensions = data.obj;
                            toaster.success(
                                $translate.instant('Admin.Js.SettingsTelephony.EventNotificationsForAddNumber') +
                                    ext.extension +
                                    $translate.instant('Admin.Js.SettingsTelephony.WasDeleted'),
                            );
                        } else {
                            data.errors.forEach(function (error) {
                                toaster.error($translate.instant('Admin.Js.SettingsTelephony.Error'), error);
                            });
                        }
                    });
                }
            });
        };

        ctrl.setTelphinEvents = function (ext) {
            SweetAlert.confirm($translate.instant('Admin.Js.SettingsTelephony.SetDefaultTelphinEvents'), {
                title: '',
            }).then(function (result) {
                if (result === true) {
                    $http.post('settingsTelephony/deleteTelphinEvents', { extensionId: ext.id }).then(function () {
                        ctrl.addTelphinEvents(ext);
                    });
                }
            });
        };

        ctrl.copyToClipboard = function (id) {
            try {
                window.getSelection().removeAllRanges();
                var link = document.querySelector('#' + id);
                link.select();
                var result = document.execCommand('copy');
                if (result) {
                    toaster.pop('success', '', 'Данные успешно скопированы');
                } else {
                    toaster.pop('error', '', 'Упс! Данные не удалось скопировать');
                }
                window.getSelection().removeAllRanges();
            } catch (error) {
                console.log(error);
            }
        };

        ctrl.addItem = function (item) {
            ctrl.phoneOrderSources.push(item);
        };

        ctrl.onSelectTab = function (indexTab) {
            ctrl.tabActiveIndex = indexTab;
        };
    };

    SettingsTelephonyCtrl.$inject = [
        '$http',
        '$q',
        '$window',
        'SweetAlert',
        'toaster',
        '$translate',
        'settingsTelephonyService',
        '$location',
        'isMobileService',
    ];

    ng.module('settingsTelephony', ['isMobile', 'uiModal']).controller('SettingsTelephonyCtrl', SettingsTelephonyCtrl);
})(window.angular);
