import productVideosTemplate from './productVideos.html';
import addEditVideoTemplate from './modal/addEditVideo/AddEditVideo.html';
(function (ng) {
    'use strict';

    var ProductVideosCtrl = function ($http, uiGridCustomConfig, $translate) {
        var ctrl = this;

        ctrl.$onInit = function () {
            ctrl.showGridVideos = true;
        };

        ctrl.gridOptions = ng.extend({}, uiGridCustomConfig, {
            columnDefs: [
                {
                    name: 'Name',
                    displayName: $translate.instant('Admin.Js.Product.Name'),
                    enableCellEdit: true,
                    enableSorting: false,
                    cellTemplate:
                        '<div class="ui-grid-cell-contents" ng-bind="row.entity[\'Name\'] || (\'Admin.Js.ProductVideos.NameNotSpecified\'|translate)"><div>',
                    uiGridCustomEdit: {
                        replaceNullable: false,
                        customViewValue: 'videoName',
                        customModel: 'nameEdit',
                        onInit: function (rowEntity, colDef, newValue, uiGridEditCustom) {
                            uiGridEditCustom.nameEdit = rowEntity.Name || $translate.instant('Admin.Js.ProductVideos.NameNotSpecified');
                            uiGridEditCustom.videoName = rowEntity.Name || $translate.instant('Admin.Js.ProductVideos.NameNotSpecified');
                        },
                        onActive: function (rowEntity, colDef, newValue, uiGridEditCustom) {
                            uiGridEditCustom.nameEdit = rowEntity.Name;
                            uiGridEditCustom.videoName = rowEntity.Name || $translate.instant('Admin.Js.ProductVideos.NameNotSpecified');
                        },
                        onDeactive: function (rowEntity, colDef, newValue, uiGridEditCustom) {
                            uiGridEditCustom.nameEdit = rowEntity.Name || $translate.instant('Admin.Js.ProductVideos.NameNotSpecified');
                            uiGridEditCustom.videoName = rowEntity.Name || $translate.instant('Admin.Js.ProductVideos.NameNotSpecified');
                        },
                        onChange: function (rowEntity, colDef, newValue, uiGridEditCustom) {
                            rowEntity.Name = newValue;
                        },
                    },
                },
                {
                    name: 'VideoSortOrder',
                    displayName: $translate.instant('Admin.Js.Product.SortingOrder'),
                    enableCellEdit: true,
                    enableSorting: false,
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
                        '<ui-modal-trigger data-controller="\'ModalAddEditVideoCtrl\'" controller-as="ctrl" ' +
                        'template-url="' +
                        addEditVideoTemplate +
                        '" ' +
                        "data-resolve=\"{'productVideoId': row.entity.ProductVideoId, 'productId': row.entity.ProductId}\" " +
                        'window-class ="product-modal-video"' +
                        'data-on-close="grid.appScope.$ctrl.fetchData()"> ' +
                        '<button type="button" class="btn-icon link-invert ui-grid-custom-service-icon fas fa-pencil-alt" aria-label="Редактировать"></button> ' +
                        '</ui-modal-trigger>' +
                        '<ui-grid-custom-delete url="product/deleteVideo" params="{\'productVideoId\': row.entity.ProductVideoId }"></ui-grid-custom-delete>' +
                        '</div></div>' +
                        '<ui-grid-custom-delete ng-if="grid.appScope.$ctrl.isMobile" url="product/deleteVideo" params="{\'productVideoId\': row.entity.ProductVideoId }" class="btn btn-sm btn-danger btn--as-swipe-line flex center-xs middle-xs">Удалить</ui-grid-custom-delete>',
                },
            ],
        });

        ctrl.gridOnInit = function (grid) {
            ctrl.gridVideos = grid;
            // ctrl.showGridVideos = ctrl.gridVideos.gridOptions.data.length > 0;
        };

        ctrl.gridOnFetch = function (grid) {
            //ctrl.showGridVideos = grid.gridOptions.data.length > 0;
        };
    };

    ProductVideosCtrl.$inject = ['$http', 'uiGridCustomConfig', '$translate'];

    ng.module('productVideos', ['uiGridCustom'])
        .controller('ProductVideosCtrl', ProductVideosCtrl)
        .component('productVideos', {
            templateUrl: productVideosTemplate,
            controller: ProductVideosCtrl,
            controllerAs: 'ctrl',
            bindings: {
                productId: '=',
                isMobileMode: '<?',
            },
        });
})(window.angular);
