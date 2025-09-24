(function (ng) {
    'use strict';
    var CustomerTagsCtrl = function (uiGridConstants, uiGridCustomConfig, $translate, $http, SweetAlert, $window, $uibModal, $q, $uibModalStack) {
        var ctrl = this,
            columnDefs = [
                {
                    name: 'Name',
                    displayName: $translate.instant('Admin.Js.CustomerTags.Name'),
                    cellTemplate: '<div class="ui-grid-cell-contents">{{COL_FIELD}}</div>',
                    filter: {
                        placeholder: $translate.instant('Admin.Js.CustomerTags.Name'),
                        type: uiGridConstants.filter.INPUT,
                        name: 'Name',
                    },
                },
                {
                    name: 'Enabled',
                    displayName: $translate.instant('Admin.Js.CustomerTags.Enabled'),
                    enableCellEdit: false,
                    cellTemplate: '<ui-grid-custom-switch row="row" field-name="Enabled"></ui-grid-custom-switch>',
                    width: 76,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.CustomerTags.Enabled'),
                        type: uiGridConstants.filter.SELECT,
                        selectOptions: [
                            { label: $translate.instant('Admin.Js.CustomerTags.Active'), value: true },
                            { label: $translate.instant('Admin.Js.CustomerTags.Inactive'), value: false },
                        ],
                    },
                },
                {
                    name: 'SortOrder',
                    displayName: $translate.instant('Admin.Js.CustomerTags.Sorting'),
                    enableCellEdit: true,
                    width: 100,
                },
                {
                    name: '_serviceColumn',
                    displayName: '',
                    width: 75,
                    enableSorting: false,
                    useInSwipeBlock: true,
                    cellTemplate:
                        '<div ng-if="!grid.appScope.$ctrl.isMobile" class="ui-grid-cell-contents"><div>' +
                        '<button type="button" ng-click="grid.appScope.$ctrl.gridOptions.uiGridCustom.rowClick(event,row)" class="btn-icon link-invert ui-grid-custom-service-icon fas fa-pencil-alt" aria-label="Редактировать"></button> ' +
                        '<ui-grid-custom-delete url="customertags/deleteTag" params="{\'id\': row.entity.Id}"></ui-grid-custom-delete>' +
                        '</div></div>' +
                        '<ui-grid-custom-delete ng-if="grid.appScope.$ctrl.isMobile" url="customertags/deleteTag" params="{\'id\': row.entity.Id}" class="btn btn-sm btn-danger btn--as-swipe-line flex center-xs middle-xs">Удалить</ui-grid-custom-delete>',
                },
            ];

        ctrl.gridOptions = ng.extend({}, uiGridCustomConfig, {
            columnDefs: columnDefs,
            uiGridCustom: {
                rowClick: function (event, row) {
                    $uibModalStack.dismissAll('open other modal');

                    $uibModal
                        .open({
                            animation: false,
                            bindToController: true,
                            controller: 'CustomerTagsModalCtrl',
                            controllerAs: 'ctrl',
                            templateUrl: 'customertags/edit/' + row.entity.Id,
                            backdrop: 'true',
                            resolve: { value: row.entity.Id },
                        })
                        .result.then(
                            function (result) {
                                ctrl.grid.fetchData();
                                return result;
                            },
                            function (result) {
                                return result;
                            },
                        );
                },
                selectionOptions: [
                    {
                        text: $translate.instant('Admin.Js.CustomerTags.DeleteSelected'),
                        url: 'customertags/deleteTags',
                        field: 'Id',
                        before: function () {
                            return SweetAlert.confirm($translate.instant('Admin.Js.CustomerTags.AreYouSureDelete'), {
                                title: $translate.instant('Admin.Js.CustomerTags.Deleting'),
                            }).then(function (result) {
                                return result === true || result.value ? $q.resolve('sweetAlertConfirm') : $q.reject('sweetAlertCancel');
                            });
                        },
                    },
                ],
            },
        });

        ctrl.gridOnInit = function (grid) {
            ctrl.grid = grid;
        };

        ctrl.saveTag = function (tag) {
            $http.post('customerTags/add', tag).then(function (response) {});
        };

        ctrl.deleteTag = function (id) {
            SweetAlert.confirm($translate.instant('Admin.Js.CustomerTags.AreYouSureDelete'), {
                title: $translate.instant('Admin.Js.CustomerTags.Deleting'),
            }).then(function (result) {
                if (result === true || result.value) {
                    $http.post('customerTags/deleteTag', { id: id }).then(function (response) {
                        $window.location.assign('settingscustomers?tab=customerTags');
                    });
                }
            });
        };
    };

    CustomerTagsCtrl.$inject = [
        'uiGridConstants',
        'uiGridCustomConfig',
        '$translate',
        '$http',
        'SweetAlert',
        '$window',
        '$uibModal',
        '$q',
        '$uibModalStack',
    ];

    ng.module('customerTags', ['uiGridCustom', 'urlHelper']).controller('CustomerTagsCtrl', CustomerTagsCtrl);
})(window.angular);
