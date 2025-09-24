import addEditApiWebhookTemplate from './modal/addEditApiWebhook/addEditApiWebhook.html';
(function (ng) {
    'use strict';

    var SettingsApiWebhooksCtrl = function ($http, toaster, $translate, uiGridCustomConfig, $q, $uibModal, SweetAlert) {
        var ctrl = this;
        ctrl.gridInited = false;
        ctrl.loadedWebhooks = false;
        ctrl.$onInit = function () {
            ctrl.loadWebhooks().then(function () {
                ctrl.loadedWebhooks = true;
            });
        };
        ctrl.gridApiWebhooksOptions = ng.extend({}, uiGridCustomConfig, {
            useExternalSorting: false,
            enableCellEdit: false,
            uiGridCustom: {
                rowClick: function ($event, row) {
                    ctrl.editWebhook(row.entity);
                    $event.preventDefault();
                },
            },
            columnDefs: [
                {
                    name: 'EventTypeName',
                    displayName: 'Событие',
                    enableCellEdit: false,
                    width: 300,
                },
                {
                    name: 'Url',
                    displayName: 'Url',
                    enableCellEdit: false,
                },
                {
                    name: '_serviceColumn',
                    displayName: '',
                    width: 75,
                    enableSorting: false,
                    useInSwipeBlock: true,
                    cellTemplate:
                        '<div ng-if="!grid.appScope.$ctrl.isMobile" class="ui-grid-cell-contents"><div>' +
                        '<button type="button" ng-click="grid.appScope.$ctrl.gridExtendCtrl.editWebhook(row.entity); $event.preventDefault();" class="btn-icon ui-grid-custom-service-icon fas fa-pencil-alt" aria-label="Редактировать"></button>' +
                        '<button type="button" ng-click="grid.appScope.$ctrl.gridExtendCtrl.deleteWebhookItem(row.entity)" ' +
                        'class="btn-icon ui-grid-custom-service-icon fa fa-times link-invert" aria-label="Удалить"></button>' +
                        '</div></div>' +
                        '<a href="" ng-if="grid.appScope.$ctrl.isMobile" ng-click="grid.appScope.$ctrl.gridExtendCtrl.deleteWebhookItem(row.entity)" class="btn btn-sm btn-danger btn--as-swipe-line flex center-xs middle-xs">Удалить</a>',
                },
            ],
        });
        ctrl.loadWebhooks = function () {
            return $http.get('settingsApi/getWebhooks').then(function (response) {
                if (response.data.result === true) {
                    ctrl.getGridApiWebhooks().then(function () {
                        ctrl.gridApiWebhooks.gridOptions.data = response.data.obj;
                    });
                } else {
                    response.data.errors.forEach(function (error) {
                        toaster.pop('error', error);
                    });
                    if (!response.data.errors) {
                        toaster.pop('error', 'Не удалось загрузить webhooks');
                    }
                }
            });
        };
        ctrl.gridApiWebhooksOnInit = function (grid) {
            ctrl.gridApiWebhooks = grid;
            ctrl.gridInited = true;
            if (ctrl.gridApiWebhooksPromise) {
                ctrl.gridApiWebhooksPromise.resolve();
            }
        };
        ctrl.getGridApiWebhooks = function () {
            if (ctrl.gridApiWebhooks) {
                return $q.resolve();
            } else {
                ctrl.gridApiWebhooksPromise = ctrl.gridApiWebhooksPromise ? ctrl.gridApiWebhooksPromise : $q.defer();
                return ctrl.gridApiWebhooksPromise.promise;
            }
        };
        ctrl.deleteWebhookItem = function (item) {
            SweetAlert.confirm($translate.instant('Admin.Js.Calls.AreYouSureDelete'), {
                title: $translate.instant('Admin.Js.Deleting'),
            }).then(function (result) {
                if ((result === true && !result.isDismissed) || (result.value && !result.isDismissed)) {
                    var indexItem = ctrl.gridApiWebhooks.gridOptions.data.indexOf(item);
                    if (indexItem > -1) {
                        ctrl.gridApiWebhooks.gridOptions.data.splice(indexItem, 1);
                        ctrl.save();
                        toaster.pop('success', '', $translate.instant('Admin.Js.Design.SuccessfullyDeleted'));
                    }
                }
            });
        };
        ctrl.addApiWebhookModal = function (result) {
            if (result && result.apiWebhook) {
                var indexItem = ctrl.gridApiWebhooks.gridOptions.data.indexOf(result.apiWebhook);
                if (indexItem > -1) {
                    ctrl.gridApiWebhooks.gridOptions.data[indexItem] = result.apiWebhook;
                } else {
                    ctrl.gridApiWebhooks.gridOptions.data.push(result.apiWebhook);
                }
                ctrl.save();
            }
        };
        ctrl.editWebhook = function (webhook) {
            $uibModal
                .open({
                    bindToController: true,
                    controller: 'ModalAddEditApiWebhookCtrl',
                    controllerAs: 'ctrl',
                    size: 'lg',
                    templateUrl: addEditApiWebhookTemplate,
                    resolve: {
                        params: webhook,
                    },
                })
                .result.then(ctrl.addApiWebhookModal);
        };
        ctrl.save = function () {
            return $http.post('settingsApi/saveWebhooks', ctrl.gridApiWebhooks.gridOptions.data).then(function (response) {
                if (response.data.result === true) {
                    ctrl.getGridApiWebhooks().then(function () {
                        ctrl.gridApiWebhooks.gridOptions.data = response.data.obj;
                    });
                } else {
                    response.data.errors.forEach(function (error) {
                        toaster.pop('error', error);
                    });
                    if (!response.data.errors) {
                        toaster.pop('error', 'Не удалось сохранить webhooks');
                    }
                }
            });
        };
    };
    SettingsApiWebhooksCtrl.$inject = ['$http', 'toaster', '$translate', 'uiGridCustomConfig', '$q', '$uibModal', 'SweetAlert'];
    ng.module('settingsApiWebhooks', []).controller('SettingsApiWebhooksCtrl', SettingsApiWebhooksCtrl);
})(window.angular);
