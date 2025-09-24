(function (ng) {
    'use strict';

    var SettingsCtrl = function (Upload, $http, toaster, $timeout, $translate, $location, isMobileService) {
        var ctrl = this;

        //ctrl.hasRegions = true;

        ctrl.init = function (langs) {
            ctrl.Langs = langs;
            var mass = langs.filter(function (item) {
                return item.Selected === true;
            });

            ctrl.langLocalization = mass.length > 0 ? mass[0] : null;
        };

        //start APi

        ctrl.generateApiKey = function () {
            $http.get('settingsApi/generate').then(function (response) {
                ctrl.Key = response.data;
            });
        };

        ctrl.generateApiKeyAuth = function () {
            $http.get('settingsApi/generate').then(function (response) {
                ctrl.KeyAuth = response.data;
            });
        };

        ctrl.showDetails = function (event) {
            angular.element(event.currentTarget).next('.details').toggle();
        };

        //end API

        ctrl.uploadLogo = function ($files, $file, $newFiles, $duplicateFiles, $invalidFiles, $event) {
            if (($event.type === 'change' || $event.type === 'drop') && $file != null) {
                Upload.upload({
                    url: '/settings/uploadlogo',
                    data: {
                        file: $file,
                        rnd: Math.random(),
                    },
                }).then(function (response) {
                    var data = response.data;
                    if (data.result === true) {
                        ctrl.logoSrc = data.file;
                    } else {
                        toaster.pop('error', $translate.instant('Admin.Js.Settings.Settings.ErrorLoadingLogo'), data.error);
                    }
                });
            } else if ($invalidFiles.length > 0) {
                toaster.pop(
                    'error',
                    $translate.instant('Admin.Js.Settings.Settings.ErrorLoadingLogo'),
                    $translate.instant('Admin.Js.Settings.Settings.FileNotMeetRequirements'),
                );
            }
        };

        ctrl.uploadFavicon = function ($files, $file, $newFiles, $duplicateFiles, $invalidFiles, $event) {
            if (($event.type === 'change' || $event.type === 'drop') && $file != null) {
                Upload.upload({
                    url: '/settings/uploadfavicon',
                    data: {
                        file: $file,
                        rnd: Math.random(),
                    },
                }).then(function (response) {
                    var data = response.data;
                    if (data.result === true) {
                        ctrl.faviconSrc = data.file;
                    } else {
                        toaster.pop('error', $translate.instant('Admin.Js.Settings.Settings.ErrorLoadingFaveicon'), data.error);
                    }
                });
            } else if ($invalidFiles.length > 0) {
                toaster.pop(
                    'error',
                    $translate.instant('Admin.Js.Settings.Settings.ErrorLoadingFaveicon'),
                    $translate.instant('Admin.Js.Settings.Settings.FileNotMeetRequirements'),
                );
            }
        };

        ctrl.uploadStamp = function ($files, $file, $newFiles, $duplicateFiles, $invalidFiles, $event) {
            if (($event.type === 'change' || $event.type === 'drop') && $file != null) {
                Upload.upload({
                    url: '/settings/uploadbankstamp',
                    data: {
                        file: $file,
                        rnd: Math.random(),
                    },
                }).then(function (response) {
                    var data = response.data;
                    if (data.result === true) {
                        ctrl.stampSrc = data.file;
                    } else {
                        toaster.pop('error', $translate.instant('Admin.Js.Settings.Settings.ErrorLoadingStamp'), data.error);
                    }
                });
            } else if ($invalidFiles.length > 0) {
                toaster.pop(
                    'error',
                    $translate.instant('Admin.Js.Settings.Settings.ErrorLoadingStamp'),
                    $translate.instant('Admin.Js.Settings.Settings.FileNotMeetRequirements'),
                );
            }
        };

        ctrl.deleteLogo = function () {
            $http.post('/settings/deletelogo').then(function (response) {
                ctrl.logoSrc = response.data.file;
            });
        };

        ctrl.deleteFavicon = function () {
            $http.post('/settings/deletefavicon').then(function (response) {
                ctrl.faviconSrc = response.data.file;
            });
        };

        ctrl.deleteStamp = function () {
            $http.post('/settings/deletebankstamp').then(function (response) {
                ctrl.stampSrc = response.data.file;
            });
        };

        ctrl.loadRegions = function (currentRegion) {
            ctrl.hasRegions = true;

            $http
                .post('/settings/GetRegions', { countryId: ctrl.countryId })
                .then(function (response) {
                    ctrl.regions = response.data.obj;
                    if (response.data.obj.length) {
                        if (currentRegion == '') {
                            ctrl.regionId = response.data.obj[0].Value;
                        } else {
                            ctrl.regionId = currentRegion;
                        }
                        ctrl.hasRegions = true;
                    } else {
                        ctrl.hasRegions = false;
                    }
                })
                .finally(function () {
                    if (ctrl.isFirstLoadRegions !== true) {
                        ctrl.form.$setPristine();
                    }

                    ctrl.isFirstLoadRegions = true;

                    $timeout(function () {
                        ctrl.regionsLoaded = true;
                    }, 0);
                });
        };

        var debounceFormatMobilePhone;
        ctrl.formatMobilePhone = function (phone) {
            if (debounceFormatMobilePhone != null) {
                clearTimeout(debounceFormatMobilePhone);
            }

            debounceFormatMobilePhone = setTimeout(function () {
                $http.post('settings/convertToStandardPhone', { phone: phone }).then(function (response) {
                    ctrl.mobilePhone = response.data.obj;
                });
            }, 300);
        };

        ctrl.scrollIntoView = function (elementId) {
            if (!elementId) return;

            setTimeout(function () {
                document.getElementById(elementId).scrollIntoView();
            }, 10);
        };

        ctrl.onSelectTab = function (indexTab) {
            ctrl.tabActiveIndex = indexTab;
        };
    };

    SettingsCtrl.$inject = ['Upload', '$http', 'toaster', '$timeout', '$translate', '$location', 'isMobileService'];

    ng.module('settings', [
        'ngFileUpload',
        'toaster',
        'as.sortable',
        'paymentMethodsList',
        'settingsUsers',
        'settingsApiWebhooks',
        'shippingMethod',
        'uiModal',
        'productsSelectvizr',
        'autocompleter',
        'isMobile',
    ]).controller('SettingsCtrl', SettingsCtrl);
})(window.angular);
