import viewEmailTemplate from './modal/viewEmail/templates/viewEmail.html';
(function (ng) {
    'use strict';

    var EmailingLogCtrl = function (uiGridConstants, uiGridCustomConfig, $translate, $uibModal, $http, toaster) {
        var ctrl = this,
            columnDefs = [
                {
                    name: 'Email',
                    displayName: $translate.instant('Admin.Js.EmailingLog.Email'),
                    enableSorting: false,
                    minWidth: 180,
                    maxWidth: 250,
                },
                {
                    name: 'CreatedFormatted',
                    displayName: $translate.instant('Admin.Js.EmailingLog.Created'),
                    width: 140,
                    enableSorting: false,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.EmailingLog.Created'),
                        type: 'date',
                        term: {
                            from: new Date(new Date().setMonth(new Date().getMonth() - 1)),
                            to: new Date(),
                        },
                        dateOptions: {
                            from: {
                                name: 'DateFrom',
                            },
                            to: {
                                name: 'DateTo',
                            },
                        },
                    },
                },
                {
                    name: 'Subject',
                    displayName: $translate.instant('Admin.Js.EmailingLog.Subject'),
                    enableSorting: false,
                },
                {
                    name: 'StatusName',
                    displayName: $translate.instant('Admin.Js.EmailingLog.Status'),
                    enableSorting: false,
                    minWidth: 150,
                    maxWidth: 250,
                    cellTemplate:
                        '<div class="ui-grid-cell-contents"><div><div>{{row.entity.StatusName}}<help-trigger class="ng-cloak m-l-xs" use-template="true"><div class="help-content"><span ng-bind-html="row.entity.StatusDesc"></span></div></help-trigger></div>' +
                        '<div class="m-t-xs" ng-if="row.entity.ShowSubscibeButton"><button class="btn btn-sm btn-success" type="button" ng-click="grid.appScope.$ctrl.gridExtendCtrl.subscribe(row.entity.Email)" ng-bind="\'Admin.Js.EmailingLog.Subscribe\' | translate"></button></div>' +
                        '</div>' +
                        '<div ng-if="row.entity.ErrorStatusDescription != null">({{row.entity.ErrorStatusDescription}})</div>' +
                        '</div>',
                    filter: {
                        placeholder: $translate.instant('Admin.Js.EmailingLog.Status'),
                        type: uiGridConstants.filter.SELECT,
                        name: 'Statuses',
                        fetch: 'emailings/getEmailStatuses',
                    },
                },
                {
                    name: '_serviceColumn',
                    displayName: '',
                    width: 50,
                    enableSorting: false,
                    cellTemplate:
                        '<div class="ui-grid-cell-contents"><div class="">' +
                        '<a href="" class="link-invert ui-grid-custom-service-icon fas fa-eye" ng-click="grid.appScope.$ctrl.gridExtendCtrl.viewEmail(row.entity.Id)"></a> ' +
                        '</div></div>',
                },
            ];
        ctrl.gridOptions = ng.extend({}, uiGridCustomConfig, {
            columnDefs: columnDefs,
            uiGridCustom: {
                rowClick: function ($event, row) {
                    ctrl.viewEmail(row.entity.Id);
                },
            },
        });
        ctrl.gridOnInit = function (grid) {
            ctrl.grid = grid;
        };
        ctrl.viewEmail = function (id) {
            $uibModal.open({
                animation: false,
                bindToController: true,
                controller: 'ModalViewEmailCtrl',
                controllerAs: 'ctrl',
                templateUrl: viewEmailTemplate,
                resolve: {
                    id: function () {
                        return id;
                    },
                },
                size: 'lg',
                backdrop: 'static',
            });
        };
        var gridWithoutColumnDefs = [
            {
                name: 'Email',
                displayName: $translate.instant('Admin.Js.EmailingLog.Email'),
                enableSorting: false,
                minWidth: 180,
                maxWidth: 250,
                filter: {
                    placeholder: $translate.instant('Admin.Js.EmailingLog.Email'),
                    type: uiGridConstants.filter.INPUT,
                    name: 'Email',
                },
            },
            {
                name: 'CreatedFormatted',
                displayName: $translate.instant('Admin.Js.EmailingLog.Created'),
                width: 140,
                enableSorting: false,
                filter: {
                    placeholder: $translate.instant('Admin.Js.EmailingLog.Created'),
                    type: 'date',
                    term: {
                        from: new Date(new Date().setMonth(new Date().getMonth() - 1)),
                        to: new Date(),
                    },
                    dateOptions: {
                        from: {
                            name: 'DateFrom',
                        },
                        to: {
                            name: 'DateTo',
                        },
                    },
                },
            },
            {
                name: 'Subject',
                displayName: $translate.instant('Admin.Js.EmailingLog.Subject'),
                enableSorting: false,
                filter: {
                    placeholder: $translate.instant('Admin.Js.EmailingLog.Subject'),
                    type: uiGridConstants.filter.INPUT,
                    name: 'Subject',
                },
            },
            {
                name: 'StatusName',
                displayName: $translate.instant('Admin.Js.EmailingLog.Status'),
                enableSorting: false,
                minWidth: 150,
                maxWidth: 200,
                cellTemplate:
                    '<div class="ui-grid-cell-contents"><div><div>{{row.entity.StatusName}}<help-trigger class="ng-cloak m-l-xs" use-template="true"><div class="help-content"><span ng-bind-html="row.entity.StatusDesc"></span></div></help-trigger></div>' +
                    '<div class="m-t-xs" ng-if="row.entity.ShowSubscibeButton"><button class="btn btn-sm btn-success" type="button" ng-click="grid.appScope.$ctrl.gridExtendCtrl.subscribe(row.entity.Email)" ng-bind="\'Admin.Js.EmailingLog.Subscribe\' | translate"></button></div>' +
                    '</div>' +
                    '<div ng-if="row.entity.ErrorStatusDescription != null">({{row.entity.ErrorStatusDescription}})</div>' +
                    '</div>',
                filter: {
                    placeholder: $translate.instant('Admin.Js.EmailingLog.Status'),
                    type: uiGridConstants.filter.SELECT,
                    name: 'Statuses',
                    fetch: 'emailings/getEmailStatuses',
                },
            },
            {
                name: '_serviceColumn',
                displayName: '',
                width: 50,
                enableSorting: false,
                cellTemplate:
                    '<div class="ui-grid-cell-contents"><div class="">' +
                    '<a href="" class="link-invert ui-grid-custom-service-icon fas fa-eye" ng-click="grid.appScope.$ctrl.gridExtendCtrl.viewEmail(row.entity.Id)"></a> ' +
                    '</div></div>',
            },
        ];
        ctrl.gridWithoutOptions = ng.extend({}, uiGridCustomConfig, {
            columnDefs: gridWithoutColumnDefs,
            uiGridCustom: {
                rowClick: function ($event, row) {
                    ctrl.viewEmail(row.entity.Id);
                },
            },
        });
        ctrl.gridWithoutOnInit = function (gridWithout) {
            ctrl.gridWithout = gridWithout;
        };
        ctrl.subscribe = function (email) {
            if (email) {
                $http
                    .get('emailings/subscribe', {
                        params: {
                            email: email,
                        },
                    })
                    .then(function (result) {
                        toaster.pop('success', 'Отправлено письмо с подтверждением подписки');
                    })
                    .catch(function () {
                        toaster.pop('error', 'Ошибка');
                    });
            } else {
                toaster.pop('error', 'Отсутвует e-mail');
            }
        };
        ctrl.gridWithoutComebackUrlParams = function () {
            if (ctrl.gridWithout && ctrl.gridWithout.filter && ctrl.gridWithout.filter.gridParams) {
                return '?dateFrom=' + ctrl.gridWithout.filter.gridParams.DateFrom + '&dateTo=' + ctrl.gridWithout.filter.gridParams.DateTo;
            }
            return '';
        };
    };
    EmailingLogCtrl.$inject = ['uiGridConstants', 'uiGridCustomConfig', '$translate', '$uibModal', '$http', 'toaster'];
    ng.module('emailingLog', ['uiGridCustom', 'urlHelper']).controller('EmailingLogCtrl', EmailingLogCtrl);
})(window.angular);
