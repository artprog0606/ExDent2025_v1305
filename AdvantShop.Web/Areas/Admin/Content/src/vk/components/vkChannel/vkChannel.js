import vkChannelTemplate from './vkChannel.html';
(function (ng) {
    'use strict';

    var vkChannelCtrl = function (
        $window,
        toaster,
        SweetAlert,
        vkService,
        $translate,
        vkMarketService,
        advTrackingService,
        isMobileService,
        appService,
        $location,
    ) {
        var ctrl = this;
        ctrl.$onInit = function () {
            ctrl.isLoadded = false;
            ctrl.getSettings();
            ctrl.getReports();
            ctrl.isMobile = isMobileService.getValue();
            ctrl.navMobileAttr = ctrl.isMobile ? 'navigation' : '';
            ctrl.childs = {};
            if (ctrl.onInit != null) {
                ctrl.onInit({
                    vkChannel: ctrl,
                });
            }
        };
        ctrl.removeChannel = function () {
            SweetAlert.confirm('Вы уверены, что хотите отключить канал?', {
                title: 'Удаление',
            }).then(function (result) {
                if (result === true || result.value) {
                    vkService.removeChannel().then(function () {
                        var basePath = document.getElementsByTagName('base')[0].getAttribute('href');
                        $window.location.assign(basePath);
                    });
                }
            });
        };
        ctrl.initChild = function (childName, childCtrl) {
            ctrl.childs[childName] = childCtrl;
        };
        ctrl.getSettings = function () {
            vkService.getVkSettings().then(function (data) {
                ctrl.clientId = data.clientId;
                ctrl.groups = data.groups;
                if (ctrl.groups != null && ctrl.groups.length > 0) {
                    ctrl.selectedGroup = ctrl.groups[0];
                }
                ctrl.group = data.group;
                ctrl.groupId = data.group != null ? data.group.Id : null;
                ctrl.groupName = data.group != null ? data.group.Name : null;
                ctrl.groupScreenName = data.group != null ? data.group.ScreenName : null;
                ctrl.authByUser = data.authByUser;
                ctrl.salesFunnels = data.salesFunnels;
                ctrl.salesFunnelId = data.salesFunnelId;
                ctrl.createLeadFromMessages = data.createLeadFromMessages;
                ctrl.createLeadFromComments = data.createLeadFromComments;
                ctrl.syncOrdersFromVk = data.syncOrdersFromVk;
                ctrl.groupMessageErrorStatus = data.groupMessageErrorStatus;
                ctrl.isLoadded = true;
                ctrl.isPreviewShow = ctrl.groupId == null || ctrl.groupName == null;
            });
        };
        ctrl.deleteGroup = function () {
            SweetAlert.confirm('Вы уверены, что хотите удалить привязку?', {
                title: 'Удаление',
            }).then(function (result) {
                if (result === true || result.value) {
                    vkService.deleteGroup().then(function () {
                        ctrl.getSettings();
                        if (ctrl.isMobile) {
                            window.location.reload();
                        }
                        if (ctrl.onAddDelVk) {
                            ctrl.onAddDelVk();
                        }
                    });
                }
            });
        };
        ctrl.changeGroupMessageErrorStatus = function () {
            vkService.changeGroupMessageErrorStatus().then(function () {
                ctrl.groupMessageErrorStatus = null;
                toaster.pop('success', '', $translate.instant('Admin.Js.SettingsCrm.ChangesSaved'));
            });
        };
        ctrl.authVkByLoginPassword = function () {
            vkService.authVkByLoginPassword(ctrl.login, ctrl.password).then(function (data) {
                if (data.result === true) {
                    ctrl.authByUser = true;
                    toaster.pop('success', '', $translate.instant('Admin.Js.SettingsCrm.AuthSuccessfulSetupComplete'));
                } else {
                    toaster.pop('error', '', $translate.instant('Admin.Js.SettingsCrm.VkFailedLogIn'));
                }
            });
        };
        ctrl.deleteVkByLoginPassword = function () {
            SweetAlert.confirm('Вы уверены, что хотите удалить привязку к личной странице?', {
                title: 'Удаление',
            }).then(function (result) {
                if (result === true || result.value) {
                    vkService.deleteVkByLoginPassword().then(function () {
                        ctrl.getSettings();
                    });
                }
            });
        };
        ctrl.saveSettings = function () {
            vkService
                .saveSettings(ctrl.salesFunnelId, ctrl.createLeadFromMessages, ctrl.createLeadFromComments, ctrl.syncOrdersFromVk)
                .then(function (data) {
                    toaster.pop('success', '', $translate.instant('Admin.Js.SettingsCrm.ChangesSaved'));
                });
        };
        ctrl.export = function () {
            vkMarketService.export().then(function (data) {
                if (data.result === true) {
                    toaster.pop('success', '', 'Начался экспорт товаров в ВКонтате. Длительность переноса зависит от кол-ва товаров и фотографий.');
                    ctrl.IsExportRun = true;
                    ctrl.getExportProgress();
                    ctrl.getReportsTimeout();
                } else if (data.errors != null) {
                    data.errors.forEach(function (e) {
                        toaster.pop('error', '', e);
                    });
                    ctrl.getExportProgress();
                }
            });
        };
        ctrl.getExportProgress = function () {
            vkMarketService.getExportProgress().then(function (data) {
                ctrl.Error = data.Error;
                ctrl.Total = data.Total;
                ctrl.Current = data.Current;
                if (ctrl.Total > 0) {
                    ctrl.Percent = ctrl.Total > 0 ? parseInt((100 / ctrl.Total) * ctrl.Current) : 0;
                    if (ctrl.Current === ctrl.Total) {
                        toaster.pop('success', '', 'Экспорт закончен');
                        ctrl.IsExportRun = false;
                        return;
                    }
                }
                setTimeout(ctrl.getExportProgress, 500);
            });
        };
        ctrl.getReportsTimeout = function () {
            setTimeout(function () {
                ctrl.getReports().then(function () {
                    if (ctrl.Percent != 100) {
                        ctrl.getReportsTimeout();
                    }
                });
            }, 3000);
        };
        ctrl.getReports = function () {
            return vkMarketService.getReports().then(function (data) {
                ctrl.Reports = data.reports;
            });
        };
        ctrl.deleteAllProducts = function () {
            SweetAlert.confirm('Вы уверены, что хотите удалить?', {
                title: 'Удаление',
            }).then(function (result) {
                if (result === true || result.value) {
                    vkMarketService.deleteAllProducts().then(function (data) {
                        toaster.pop('success', '', 'Удаление началось');
                    });
                }
            });
        };
        ctrl.connectVk = function () {
            ctrl.isPreviewShow = false;
            advTrackingService.trackEvent('SalesChannels_Interest', 'vk');
        };
        ctrl.onSelectTab = function (indexTab, name) {
            ctrl.tabActiveIndex = indexTab;
            if (isMobileService.getValue()) {
                //appService.setTitle(name.replace(/(в ВКонтакте|из ВКонтакте)$/g, ''));
            }
        };
    };
    vkChannelCtrl.$inject = [
        '$window',
        'toaster',
        'SweetAlert',
        'vkService',
        '$translate',
        'vkMarketService',
        'advTrackingService',
        'isMobileService',
        'appService',
        '$location',
    ];
    ng.module('vkChannel', ['isMobile'])
        .controller('vkChannelCtrl', vkChannelCtrl)
        .component('vkChannel', {
            templateUrl: vkChannelTemplate,
            controller: 'vkChannelCtrl',
            bindings: {
                redirectUrl: '=',
                onAddDelVk: '&',
                onInit: '&',
            },
        })
        .directive('vkHeaderButton', [
            '$window',
            function ($window) {
                return {
                    restrict: 'A',
                    controller: 'vkChannelCtrl',
                    bindToController: true,
                    link: function (scope, element, attr, ctrl) {
                        element.on('click', function () {
                            switch (attr.vkHeaderButton) {
                                case 'removeChannel':
                                    ctrl.removeChannel();
                                    break;
                                case 'openGroup':
                                    $window.open(`https://vk.com/${ctrl.group.ScreenName}`);
                                    break;
                                case 'deleteGroup':
                                    ctrl.deleteGroup();
                                    break;
                                case 'deleteAllProducts':
                                    ctrl.deleteAllProducts();
                                    break;
                            }
                        });
                    },
                };
            },
        ]);
})(window.angular);
