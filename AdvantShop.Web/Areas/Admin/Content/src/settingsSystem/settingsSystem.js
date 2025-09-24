(function (ng) {
    'use strict';

    var SettingsSystemCtrl = function (
        Upload,
        $http,
        toaster,
        $q,
        $window,
        uiGridConstants,
        uiGridCustomConfig,
        uiGridCustomParamsConfig,
        uiGridCustomService,
        SweetAlert,
        $translate,
        adminColorSchemeService,
        $location,
        isMobileService,
    ) {
        var ctrl = this;

        ctrl.$onInit = function () {
            var urlSearch = $location.search();

            if (urlSearch != null && urlSearch.trafficSortOrder != null) ctrl.trafficSortOrder = urlSearch.trafficSortOrder;
            else ctrl.trafficSortOrder = 'NoSorting';
            ctrl.getIpBlacklist();
            ctrl.getAvailableCountries();
            ctrl.getTrafficStatistics();
            ctrl.getSiteMaps();
            ctrl.items;
        };

        ctrl.onSelectTab = function (indexTab) {
            ctrl.tabActiveIndex = indexTab;
        };

        ctrl.checkLicense = function () {
            return SweetAlert.confirm($translate.instant('Admin.Js.SettingsSystem.CheckAndSpecifyKey'), {
                title: $translate.instant('Admin.Js.SettingsSystem.LicenseValidation'),
            }).then(function (result) {
                return result === true || result.value
                    ? $http
                          .post('settingsSystem/checkLicense', {
                              licKey: ctrl.LicKey,
                          })
                          .then(function (response) {
                              if (response.data.result === true) {
                                  ctrl.ActiveLic = true;
                                  toaster.pop('success', $translate.instant('Admin.Js.SettingsSystem.LicenseStatusActive'), '');
                              } else {
                                  ctrl.ActiveLic = false;
                                  toaster.pop({
                                      type: 'error',
                                      title: $translate.instant('Admin.Js.SettingsSystem.LicenseStatusNotActive'),
                                      timeout: 0,
                                  });
                              }
                          })
                    : $q.reject('sweetAlertCancel');
            });
        };

        ctrl.updateSiteMaps = function () {
            $http.post('settingsSystem/updateSiteMaps').then(function (response) {
                ctrl.items = response.data.obj.map((item) => {
                    var siteMapFileHtmlLinkText = item.SiteMapFileHtmlLink;
                    var siteMapFileXmlLinkText = item.SiteMapFileXmlLink;
                    return {
                        SiteMapFileHtmlLinkText: siteMapFileHtmlLinkText,
                        SiteMapFileHtmlLink: siteMapFileHtmlLinkText + '?rnd=' + Math.random() * 100000,
                        SiteMapFileXmlLinkText: siteMapFileXmlLinkText,
                        SiteMapFileXmlLink: siteMapFileXmlLinkText + '?rnd=' + Math.random() * 100000,
                    };
                });

                ctrl.SiteMapFileDate = response.data.message;
                if (response.data.result === true) {
                    toaster.pop('success', '', $translate.instant('Admin.Js.SettingsSystem.SiteMapsRefreshed'));
                } else {
                    toaster.pop('error', $translate.instant('Admin.Js.SettingsSystem.ErrorUpdatingSiteMaps'), '');
                }
            });
        };

        ctrl.getSiteMaps = function () {
            $http.post('settingsSystem/getSiteMaps').then(function (response) {
                ctrl.items = response.data.obj.map((item) => {
                    var siteMapFileHtmlLinkText = item.SiteMapFileHtmlLink;
                    var siteMapFileXmlLinkText = item.SiteMapFileXmlLink;
                    return {
                        SiteMapFileHtmlLinkText: siteMapFileHtmlLinkText,
                        SiteMapFileHtmlLink: siteMapFileHtmlLinkText + '?rnd=' + Math.random() * 100000,
                        SiteMapFileXmlLinkText: siteMapFileXmlLinkText,
                        SiteMapFileXmlLink: siteMapFileXmlLinkText + '?rnd=' + Math.random() * 100000,
                    };
                });
                ctrl.SiteMapFileDate = response.data.message;
            });
        };

        ctrl.fileStorageRecalc = function () {
            $http.post('settingsSystem/fileStorageRecalc').then(function (response) {
                toaster.pop('success', '', $translate.instant('Admin.Js.SettingsSystem.RecalculationIsStarted'));
                ctrl.showFileStorageRecalc = false;
            });
        };

        //#region Localization

        ctrl.AllLocalization = true;

        ctrl.changeSelectLanguage = function () {
            ctrl.gridLocalization.setParams({
                Value: ctrl.langLocalization,
                ChangeAll: ctrl.AllLocalization,
            });
            ctrl.gridLocalization.fetchData();
        };

        ctrl.startExportlocalization = function () {
            if (ctrl.langLocalization == null) {
                toaster.pop(
                    'error',
                    $translate.instant('Admin.Js.SettingsSystem.Error'),
                    $translate.instant('Admin.Js.SettingsSystem.SelectLanguageForExport'),
                );
            } else {
                $window.location.assign('localization/export?lang=' + ctrl.langLocalization);
            }
        };

        var columnDefsLocalization = [
            {
                name: 'ResourceKey',
                displayName: $translate.instant('Admin.Js.SettingsSystem.Key'),
                enableCellEdit: false,
                filter: {
                    placeholder: $translate.instant('Admin.Js.SettingsSystem.Key'),
                    type: uiGridConstants.filter.INPUT,
                    name: 'ResourceKey',
                },
            },
            {
                name: 'ResourceValue',
                displayName: $translate.instant('Admin.Js.SettingsSystem.Value'),
                enableCellEdit: true,
                uiGridCustomEdit: {
                    replaceNullable: false,
                },
                filter: {
                    placeholder: $translate.instant('Admin.Js.SettingsSystem.Value'),
                    name: 'ResourceValue',
                    type: uiGridConstants.filter.INPUT,
                },
            },
        ];

        ctrl.gridOptionsLocalization = ng.extend({}, uiGridCustomConfig, {
            columnDefs: columnDefsLocalization,
            uiGridCustom: {
                rowUrl: '',
            },
        });

        ctrl.gridLocalizationOnInit = function (gridLocalization) {
            ctrl.gridLocalization = gridLocalization;
        };

        ctrl.successLocalization = function () {
            toaster.pop('success', '', $translate.instant('Admin.Js.SettingsSystem.LocalizationImportSuccess'));

            // reload page to load new languages
            $window.location.reload(true);
        };

        //#endregion

        ctrl.changeColorScheme = function () {
            adminColorSchemeService.change(ctrl.AdminAreaColorScheme);
        };

        ctrl.addSettingsLocation = function (settingsLocation) {
            ctrl.settingsLocation = settingsLocation;
        };

        ctrl.backTabIndex = function () {
            let path = $location.$$absUrl;
            if (Object.keys($location.$$search)[0]) {
                if (ctrl.settingsLocation != undefined) {
                    if (ctrl.settingsLocation.locationTypeSelected == 'country' || ctrl.settingsLocation.locationTypeSelected == undefined) {
                        path = path.split('?systemTab')[0];
                        history.pushState(null, null, path);
                        ctrl.floatHeader = null;
                        ctrl.tabActiveIndex = null;
                    }
                } else {
                    path = path.split('?systemTab')[0];
                    history.pushState(null, null, path);
                    ctrl.floatHeader = null;
                    ctrl.tabActiveIndex = null;
                }
            }
        };

        ctrl.checkPhoneDuplicates = function () {
            // если выключаем настройку
            if (ctrl.SmsActive === true) {
                ctrl.havePhoneDuplicates = false;
                return;
            }
            $http.get('settingsSystem/checkPhoneDuplicates').then(function (response) {
                if (response.data.result) {
                    var data = response.data.obj;
                    ctrl.SmsActive = !data.haveDuplicates;
                    ctrl.havePhoneDuplicates = data.haveDuplicates;
                    ctrl.duplicateFileName = data.urlPath;
                    if (ctrl.HaveDuplicates) {
                        ctrl.SmsActive = false;
                    }
                } else {
                    toaster.pop('error', $translate.instant('Admin.Js.SettingsSystem.Error'), '');
                }
            });
        };

        ctrl.getIpBlacklist = function () {
            $http.get('settingsSystem/getIpBlacklist').then(function (response) {
                ctrl.ipBlackList = response.data || [];
            });
        };

        ctrl.addIpInBlacklist = function (ips) {
            ips = ips || ctrl.ipBlackListSettings.newIpList;
            if (!ips) {
                toaster.pop('error', $translate.instant('Admin.Js.SettingsSystem.Error'), '');
                return;
            }

            var ipWithMaskList = ips.split('\n').filter(Boolean);

            if (!ipWithMaskList || ipWithMaskList.length == 0) {
                toaster.pop('error', $translate.instant('Admin.Js.SettingsSystem.Error'), '');
                return;
            }

            if (!ctrl.ipBlackList) ctrl.ipBlackList = [];
            ctrl.ipBlackList.forEach(function (existsIp) {
                var index = ipWithMaskList.indexOf(existsIp);
                if (index != -1) {
                    toaster.pop(
                        'error',
                        $translate.instant('Admin.Js.SettingsSystem.IpBlacklist.AlreadyExistIp') + ' - ' + ipWithMaskList[index],
                        '',
                    );
                    ipWithMaskList.splice(index, 1);
                }
            });
            if (ipWithMaskList.length == 0) return;
            $http.post('settingsSystem/addIpInBlacklist', { ipWithMaskList: ipWithMaskList }).then(function (response) {
                var data = response.data;
                if (data.result) {
                    if (ctrl.ipBlackListSettings) ctrl.ipBlackListSettings.newIp = '';
                    ctrl.getIpBlacklist();
                    ctrl.getTrafficStatistics();
                    toaster.pop('success', ipWithMaskList.join(', ') + ' ' + $translate.instant('Admin.Js.SettingsSystem.IpBlacklist.IpBanned'));
                } else if (data.errors)
                    data.errors.forEach(function (error) {
                        toaster.pop('error', error);
                    });
                else toaster.pop('error', $translate.instant('Admin.Js.SettingsSystem.IpBlacklist.ErrorWhenAdding'));
            });
        };

        ctrl.deleteIpFromBlacklist = function (ip) {
            SweetAlert.confirm($translate.instant('Admin.Js.SettingsSystem.IpBlacklist.AreYouSureRemovingBan'), {
                title: $translate.instant('Admin.Js.SettingsSystem.IpBlacklist.RemovingBan'),
            }).then(function (result) {
                if (result === true || result.value) {
                    $http.post('settingsSystem/deleteIpFromBlacklist', { ipWithMask: ip }).then(function (response) {
                        var data = response.data;
                        if (data.result) {
                            ctrl.getIpBlacklist();
                            ctrl.getTrafficStatistics();
                            toaster.pop('success', 'Ip ' + ip + ' ' + $translate.instant('Admin.Js.SettingsSystem.IpBlacklist.IpUnban'));
                        } else if (data.errors)
                            data.errors.forEach(function (error) {
                                toaster.pop('error', error);
                            });
                        else toaster.pop('error', $translate.instant('Admin.Js.SettingsSystem.IpBlacklist.ErrorWhenDeleting'));
                    });
                }
            });
        };

        ctrl.getAvailableCountries = function () {
            $http.get('settingsSystem/getAllowedCountriesForSite').then(function (response) {
                ctrl.AvailableCountries = response.data;
            });
        };

        ctrl.addAvailableCountry = function () {
            $http
                .post('settingsSystem/addCountryToAllowedSite', {
                    methodId: ctrl.methodId,
                    countryName: ctrl.newAvailableCountry,
                })
                .then(function (response) {
                    if (response.data.result === true) {
                        ctrl.newAvailableCountry = '';
                        toaster.pop('success', '', $translate.instant('Admin.Js.ShippingMethods.ChangesSuccessfullySaved'));
                    } else if (response.data.errors)
                        response.data.errors.forEach(function (error) {
                            toaster.pop('error', error);
                        });
                    else toaster.pop('error', '', $translate.instant('Admin.Js.ShippingMethods.CanNotAdd') + ctrl.newAvailableCountry + '" ');
                })
                .then(ctrl.getAvailableCountries);
        };

        ctrl.deleteAvailableCountry = function (countryId) {
            $http
                .get('settingsSystem/deleteCountryFromAllowedSite', {
                    params: {
                        countryId: countryId,
                    },
                })
                .then(function (response) {
                    if (response.data.result === true) {
                        ctrl.newAvailableCountry = '';
                        toaster.pop('success', '', $translate.instant('Admin.Js.ShippingMethods.ChangesSuccessfullySaved'));
                    } else if (response.data.errors)
                        response.data.errors.forEach(function (error) {
                            toaster.pop('error', error);
                        });
                    else toaster.pop('error', '', $translate.instant('Admin.Js.ShippingMethods.CanNotAdd') + ctrl.newAvailableCountry + '" ');
                })
                .then(ctrl.getAvailableCountries);
        };

        ctrl.getTrafficStatistics = function () {
            $http.get('settingsSystem/GetTrafficStatistics').then(function (response) {
                ctrl.unsortedTrafficStatistics = ng.copy(response.data);
                ctrl.trafficStatistics = ng.copy(response.data);
                if (ctrl.trafficSortOrder != 'NoSorting') ctrl.sortTrafficStatistics();
            });
        };

        ctrl.trafficSortOrders = [
            { text: $translate.instant('Admin.Js.TrafficStatistics.SortOrder.NoSorting'), value: 'NoSorting' },
            { text: $translate.instant('Admin.Js.TrafficStatistics.SortOrder.DescByCountRequest'), value: 'DescByCountRequest' },
            { text: $translate.instant('Admin.Js.TrafficStatistics.SortOrder.AscByCountRequest'), value: 'AscByCountRequest' },
            { text: $translate.instant('Admin.Js.TrafficStatistics.SortOrder.DescByIp'), value: 'DescByIp' },
            { text: $translate.instant('Admin.Js.TrafficStatistics.SortOrder.AscByIp'), value: 'AscByIp' },
        ];

        ctrl.sortTrafficStatistics = function () {
            $location.search('trafficSortOrder', ctrl.trafficSortOrder);
            if (!ctrl.trafficSortOrder || ctrl.trafficSortOrder == 'NoSorting') ctrl.trafficStatistics = ng.copy(ctrl.unsortedTrafficStatistics);
            ctrl.trafficStatistics.sort(function (a, b) {
                switch (ctrl.trafficSortOrder) {
                    case 'DescByCountRequest':
                        return b.CountRequest - a.CountRequest;
                    case 'AscByCountRequest':
                        return a.CountRequest - b.CountRequest;
                    case 'DescByIp':
                        if (a.Ip < b.Ip) {
                            return -1;
                        }
                        if (a.Ip > b.Ip) {
                            return 1;
                        }
                        return 0;
                    case 'AscByIp':
                        if (b.Ip < a.Ip) {
                            return -1;
                        }
                        if (b.Ip > a.Ip) {
                            return 1;
                        }
                        return 0;
                }
            });
        };
    };

    SettingsSystemCtrl.$inject = [
        'Upload',
        '$http',
        'toaster',
        '$q',
        '$window',
        'uiGridConstants',
        'uiGridCustomConfig',
        'uiGridCustomParamsConfig',
        'uiGridCustomService',
        'SweetAlert',
        '$translate',
        'adminColorSchemeService',
        '$location',
        'isMobileService',
    ];

    ng.module('settingsSystem', [
        'ngFileUpload',
        'toaster',
        'as.sortable',
        'paymentMethodsList',
        'adminColorScheme',
        'menus',
        'import',
        'fileUploader',
        'isMobile',
    ]).controller('SettingsSystemCtrl', SettingsSystemCtrl);
})(window.angular);
