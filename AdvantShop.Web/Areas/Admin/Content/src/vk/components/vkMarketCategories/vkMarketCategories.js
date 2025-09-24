import vkMarketCategoriesTemplate from './vkMarketCategories.html';
import modalAddEditVkCategoryTemplate from './modals/ModalAddEditVkCategory.html';
(function (ng) {
    'use strict';

    var vkMarketCategoriesCtrl = function ($http, toaster, vkMarketService, uiGridConstants, uiGridCustomConfig, SweetAlert, $q, $uibModal) {
        var ctrl = this,
            columnDefs = [
                {
                    name: 'Name',
                    displayName: 'Название',
                    enableSorting: false,
                    cellTemplate:
                        '<div class="ui-grid-cell-contents">' +
                        '<a href="" ng-click="grid.appScope.$ctrl.gridExtendCtrl.openModal(row.entity.Id)">{{COL_FIELD}}</a> ' +
                        '</div>',
                },
                {
                    name: 'Categories',
                    displayName: 'Категории магазина',
                    enableSorting: false,
                    width: 300,
                    cellTemplate: '<div class="ui-grid-cell-contents">' + '<span ng-bind-html="row.entity.Categories"></span> ' + '</div>',
                },
                {
                    name: 'SortOrder',
                    displayName: 'Сортировка',
                    enableSorting: false,
                    enableCellEdit: true,
                    width: 100,
                },
                {
                    name: '_serviceColumn',
                    displayName: '',
                    width: 80,
                    enableSorting: false,
                    useInSwipeBlock: true,
                    cellTemplate:
                        '<div ng-if="!grid.appScope.$ctrl.isMobile" class="ui-grid-cell-contents"><div>' +
                        '<button type="button" class="btn-icon link-invert ui-grid-custom-service-icon fa fa-pencil-alt" ng-click="grid.appScope.$ctrl.gridExtendCtrl.openModal(row.entity.Id)" aria-label="Редактировать"></button> ' +
                        '<button type="button" ng-click="grid.appScope.$ctrl.gridExtendCtrl.delete(row.entity.Id, row.entity.VkId)" class="btn-icon ui-grid-custom-service-icon fa fa-times link-invert" aria-label="Удалить"></button> ' +
                        '</div></div>' +
                        '<ui-grid-custom-delete ng-if="grid.appScope.$ctrl.isMobile" url="vkMarket/deleteCategory" params="{\'Id\': row.entity.Id,\'VkId\': row.entity.VkId}" class="btn btn-sm btn-danger btn--as-swipe-line flex center-xs middle-xs">Удалить</ui-grid-custom-delete>',
                },
            ];
        ctrl.gridOptions = ng.extend({}, uiGridCustomConfig, {
            columnDefs: columnDefs,
            uiGridCustom: {
                rowClick: function ($event, row) {
                    ctrl.openModal(row.entity.Id);
                },
                selectionOptions: [
                    {
                        text: 'Удалить выделенные',
                        url: 'vkMarket/deleteCategories',
                        field: 'Id',
                        before: function () {
                            return SweetAlert.confirm(
                                'Категории и товары в них будут удалены из ВКонтакте.<br/> Удаление большого каталога может занять много времени',
                                {
                                    title: 'Вы уверены, что хотите удалить?',
                                },
                            ).then(function (result) {
                                return result === true || result.value ? $q.resolve('sweetAlertConfirm') : $q.reject('sweetAlertCancel');
                            });
                        },
                    },
                ],
            },
        });
        ctrl.$onInit = function () {
            if (ctrl.onInit != null) {
                ctrl.onInit({
                    vkMarketCategories: ctrl,
                });
            }
        };
        ctrl.refresh = function () {
            ctrl.grid.fetchData();
        };
        ctrl.gridOnInit = function (grid) {
            ctrl.grid = grid;
        };
        ctrl.openModal = function (id) {
            $uibModal
                .open({
                    bindToController: true,
                    controller: 'ModalAddEditVkCategoryCtrl',
                    controllerAs: 'ctrl',
                    templateUrl: modalAddEditVkCategoryTemplate,
                    size: 'middle',
                    resolve: {
                        id: function () {
                            return id;
                        },
                    },
                })
                .result.then(
                    function (result) {
                        console.log(ctrl.grid);
                        ctrl.grid.fetchData();
                        return result;
                    },
                    function (result) {
                        ctrl.grid.fetchData();
                        return result;
                    },
                );
        };
        ctrl.delete = function (id, vkId) {
            SweetAlert.confirm('Вы уверены, что хотите удалить? При удалении категории удалится подборка в ВКонтакте и товары в ней.', {
                title: 'Удаление',
            }).then(function (result) {
                if (result === true || result.value) {
                    toaster.pop('success', '', 'Удаление категории и товаров началось');
                    $http
                        .post('vkMarket/deleteCategory', {
                            Id: id,
                            VkId: vkId,
                        })
                        .then(function (response) {
                            ctrl.grid.fetchData();
                        });
                }
            });
        };
    };
    vkMarketCategoriesCtrl.$inject = [
        '$http',
        'toaster',
        'vkMarketService',
        'uiGridConstants',
        'uiGridCustomConfig',
        'SweetAlert',
        '$q',
        '$uibModal',
    ];
    ng.module('vkMarketCategories', ['uiGridCustom'])
        .controller('vkMarketCategoriesCtrl', vkMarketCategoriesCtrl)
        .component('vkMarketCategories', {
            templateUrl: vkMarketCategoriesTemplate,
            controller: 'vkMarketCategoriesCtrl',
            bindings: {
                onInit: '&',
            },
        });
})(window.angular);
