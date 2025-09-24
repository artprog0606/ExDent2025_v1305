import addEditGradeTemplate from './modal/addEditGrade/AddEditGrade.html';
(function (ng) {
    'use strict';

    var GradesCtrl = function (
        $location,
        $window,
        $uibModal,
        uiGridConstants,
        uiGridCustomConfig,
        uiGridCustomParamsConfig,
        uiGridCustomService,
        toaster,
        $http,
        $q,
        SweetAlert,
        $translate,
    ) {
        var ctrl = this,
            columnDefs = [
                {
                    name: 'Name',
                    displayName: $translate.instant('Admin.Js.Grades.Name'),
                    cellTemplate: '<div class="ui-grid-cell-contents"><span class="link">{{COL_FIELD}}</span></div>',
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Grades.Name'),
                        type: uiGridConstants.filter.INPUT,
                        name: 'Name',
                    },
                },
                {
                    name: 'BonusPercent',
                    displayName: $translate.instant('Admin.Js.Grades.Percent'),
                    enableCellEdit: true,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Grades.Percent'),
                        type: uiGridConstants.filter.INPUT,
                        name: 'BonusPercent',
                    },
                },
                {
                    name: 'PurchaseBarrier',
                    displayName: $translate.instant('Admin.Js.Grades.Transition'),
                    headerCellClass: 'grid-text-required',
                    enableCellEdit: true,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Grades.Transition'),
                        type: uiGridConstants.filter.INPUT,
                        name: 'PurchaseBarrier',
                    },
                },
                {
                    name: 'SortOrder',
                    displayName: $translate.instant('Admin.Js.Grades.Sorting'),
                    enableCellEdit: true,
                },
                {
                    name: '_serviceColumn',
                    displayName: '',
                    width: 75,
                    enableSorting: false,
                    useInSwipeBlock: true,
                    cellTemplate:
                        '<div ng-if="!grid.appScope.$ctrl.isMobile" class="ui-grid-cell-contents js-grid-not-clicked"><div>' +
                        '<button type="button" class="btn-icon link-invert ui-grid-custom-service-icon fas fa-pencil-alt" ng-click="grid.appScope.$ctrl.gridExtendCtrl.loadGrade(row.entity.Id)" aria-label="Редактировать"></button> ' +
                        '<ui-grid-custom-delete url="grades/delete" params="{\'id\': row.entity.Id}"></ui-grid-custom-delete>' +
                        '</div></div>' +
                        '<ui-grid-custom-delete ng-if="grid.appScope.$ctrl.isMobile" url="grades/delete" params="{\'id\': row.entity.Id}" class="btn btn-sm btn-danger btn--as-swipe-line flex center-xs middle-xs">Удалить</ui-grid-custom-delete>',
                },
            ];
        ctrl.gridOptions = ng.extend({}, uiGridCustomConfig, {
            columnDefs: columnDefs,
            uiGridCustom: {
                rowClick: function ($event, row) {
                    ctrl.loadGrade(row.entity.Id);
                },
                selectionOptions: [
                    {
                        text: $translate.instant('Admin.Js.Grades.DeleteSelected'),
                        url: 'grades/deleteGrade',
                        field: 'Id',
                        before: function () {
                            return SweetAlert.confirm($translate.instant('Admin.Js.Grades.AreYouSureDelete'), {
                                title: $translate.instant('Admin.Js.Grades.Deleting'),
                            }).then(function (result) {
                                return result === true || result.value ? $q.resolve('sweetAlertConfirm') : $q.reject('sweetAlertCancel');
                            });
                        },
                    },
                ],
            },
        });
        ctrl.loadGrade = function (id) {
            $uibModal
                .open({
                    bindToController: true,
                    controller: 'ModalAddEditGradeCtrl',
                    controllerAs: 'ctrl',
                    templateUrl: addEditGradeTemplate,
                    resolve: {
                        id: function () {
                            return id;
                        },
                    },
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
        };
        ctrl.gridOnInit = function (grid) {
            ctrl.grid = grid;
        };
    };
    GradesCtrl.$inject = [
        '$location',
        '$window',
        '$uibModal',
        'uiGridConstants',
        'uiGridCustomConfig',
        'uiGridCustomParamsConfig',
        'uiGridCustomService',
        'toaster',
        '$http',
        '$q',
        'SweetAlert',
        '$translate',
    ];
    ng.module('grades', ['uiGridCustom', 'urlHelper']).controller('GradesCtrl', GradesCtrl);
})(window.angular);
