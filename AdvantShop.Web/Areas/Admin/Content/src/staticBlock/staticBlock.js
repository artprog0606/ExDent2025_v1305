import addEditStaticBlockTemplate from './modal/addEditStaticBlock/addEditStaticBlock.html';
(function (ng) {
    'use strict';

    var StaticBlockCtrl = function (
        $location,
        $window,
        uiGridConstants,
        uiGridCustomConfig,
        $q,
        SweetAlert,
        $http,
        $uibModal,
        $translate,
        uiGridCustomService,
    ) {
        var ctrl = this,
            columnDefs = [
                {
                    name: 'Key',
                    displayName: $translate.instant('Admin.Js.StaticBlock.AccessKey'),
                    enableSorting: true,
                    enableCellEdit: false,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.StaticBlock.AccessKey'),
                        type: uiGridConstants.filter.INPUT,
                        name: 'Key',
                    },
                    cellTemplate:
                        '<div class="ui-grid-cell-contents"><div>' +
                        '<ui-modal-trigger data-controller="\'ModalAddEditStaticBlockCtrl\'" controller-as="ctrl" size="middle" ' +
                        'template-url="' +
                        addEditStaticBlockTemplate +
                        '" ' +
                        'data-resolve="{\'id\': row.entity.StaticBlockId}" ' +
                        'data-on-close="grid.appScope.$ctrl.fetchData()"> ' +
                        '<a href="">{{COL_FIELD}}</a>' +
                        '</ui-modal-trigger>' +
                        '</div></div>',
                },
                {
                    name: 'InnerName',
                    displayName: $translate.instant('Admin.Js.StaticBlock.Name'),
                    enableCellEdit: false,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.StaticBlock.Name'),
                        type: uiGridConstants.filter.INPUT,
                        name: 'InnerName',
                    },
                },
                {
                    name: 'Enabled',
                    displayName: $translate.instant('Admin.Js.StaticBlock.Activ'),
                    enableCellEdit: false,
                    cellTemplate: '<ui-grid-custom-switch row="row"></ui-grid-custom-switch>',
                    width: 76,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.StaticBlock.Activity'),
                        name: 'Enabled',
                        type: uiGridConstants.filter.SELECT,
                        selectOptions: [
                            { label: $translate.instant('Admin.Js.StaticBlock.Active'), value: true },
                            { label: $translate.instant('Admin.Js.StaticBlock.Inactive'), value: false },
                        ],
                    },
                },
                {
                    name: 'AddedFormatted',
                    displayName: $translate.instant('Admin.Js.StaticBlock.DateOfAdding'),
                    width: 150,
                    enableCellEdit: false,
                },

                {
                    name: 'ModifiedFormatted',
                    displayName: $translate.instant('Admin.Js.StaticBlock.ModificatonDate'),
                    width: 165,
                    enableCellEdit: false,
                },
                {
                    name: 'Content',
                    visible: false,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.StaticBlock.Content'),
                        type: uiGridConstants.filter.INPUT,
                        name: 'Content',
                    },
                },
                {
                    name: '_serviceColumnEdit',
                    displayName: '',
                    width: 75,
                    enableSorting: false,
                    // useInSwipeBlock: true,
                    cellTemplate:
                        '<div ng-if="!grid.appScope.$ctrl.isMobile" class="ui-grid-cell-contents"><div>' +
                        '<ui-modal-trigger data-controller="\'ModalAddEditStaticBlockCtrl\'" controller-as="ctrl" size="middle" ' +
                        'template-url="' +
                        addEditStaticBlockTemplate +
                        '" ' +
                        'data-resolve="{\'id\': row.entity.StaticBlockId}" ' +
                        'data-on-close="grid.appScope.$ctrl.fetchData()"> ' +
                        uiGridCustomService.getTemplateCellLink('') +
                        // '<a href="" class="ui-grid-custom-service-icon fas fa-pencil-alt news-category-pointer">{{COL_FIELD}}</a>' +
                        '</ui-modal-trigger>' +
                        //'<ui-grid-custom-delete url="StaticBlock/deleteItem" params="{\'id\': row.entity.StaticBlockId}"></ui-grid-custom-delete>' +
                        '</div></div>',
                    //'<ui-grid-custom-delete ng-if="grid.appScope.$ctrl.isMobile" url="StaticBlock/deleteItem" params="{\'id\': row.entity.StaticBlockId}" class="btn btn-sm btn-danger btn--as-swipe-line flex center-xs middle-xs">Удалить</ui-grid-custom-delete>'
                },
            ];

        ctrl.gridOptions = ng.extend({}, uiGridCustomConfig, {
            columnDefs: columnDefs,
            uiGridCustom: {
                selectionOptions: [
                    /*{
                        text: $translate.instant('Admin.Js.StaticBlock.DeleteSelected'),
                        url: 'staticBlock/delete',
                        field: 'StaticBlockId',
                        before: function () {
                            return SweetAlert.confirm($translate.instant('Admin.Js.StaticBlock.AreYouSureDelete'), { title: $translate.instant('Admin.Js.StaticBlock.Deleting') }).then(function (result) {
                                return result === true ? $q.resolve('sweetAlertConfirm') : $q.reject('sweetAlertCancel');
                            });
                        }
                    },*/
                    {
                        text: $translate.instant('Admin.Js.StaticBlock.MakeActive'),
                        url: 'staticBlock/active',
                        field: 'StaticBlockId',
                    },
                    {
                        text: $translate.instant('Admin.Js.StaticBlock.MakeInactive'),
                        url: 'staticBlock/deactive',
                        field: 'StaticBlockId',
                    },
                ],
            },
        });

        ctrl.gridOnInit = function (grid) {
            ctrl.grid = grid;
        };
    };

    StaticBlockCtrl.$inject = [
        '$location',
        '$window',
        'uiGridConstants',
        'uiGridCustomConfig',
        '$q',
        'SweetAlert',
        '$http',
        '$uibModal',
        '$translate',
        'uiGridCustomService',
    ];

    ng.module('staticBlock', ['uiGridCustom', 'urlHelper']).controller('StaticBlockCtrl', StaticBlockCtrl);
})(window.angular);
