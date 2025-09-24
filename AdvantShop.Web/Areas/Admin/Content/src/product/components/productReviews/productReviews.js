import productReviewsTemplate from './productReviews.html';
import addEditReviewTemplate from './../../../reviews/modal/addEditReview/AddEditReview.html';
(function (ng) {
    'use strict';

    var ProductReviewsCtrl = function ($http, uiGridConstants, uiGridCustomConfig, SweetAlert, $q, $uibModal, $translate, toaster) {
        var ctrl = this,
            columnDefs = [
                {
                    name: '_noopColumnName',
                    visible: false,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Reviews.Author'),
                        type: uiGridConstants.filter.INPUT,
                        name: 'Name',
                    },
                },
                {
                    name: '_noopColumnEmail',
                    visible: false,
                    filter: {
                        placeholder: 'Email',
                        type: uiGridConstants.filter.INPUT,
                        name: 'Email',
                    },
                },
                {
                    name: '_noopColumnText',
                    visible: false,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Reviews.ReviewText'),
                        type: uiGridConstants.filter.INPUT,
                        name: 'Text',
                    },
                },
                {
                    name: '_noopColumnArtNo',
                    visible: false,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Reviews.VendorCode'),
                        type: uiGridConstants.filter.INPUT,
                        name: 'ArtNo',
                    },
                },
                {
                    name: 'Name',
                    width: 270,
                    displayName: $translate.instant('Admin.Js.Reviews.Author'),
                    cellTemplate: '<div class="ui-grid-cell-contents">' + '{{row.entity.Name}}<br/> ' + '{{row.entity.Email}} ' + '</div>',
                },
                {
                    name: 'Text',
                    displayName: $translate.instant('Admin.Js.Reviews.ReviewText'),
                    cellTemplate:
                        '<div class="ui-grid-cell-contents">' + '{{ grid.appScope.$ctrl.gridExtendCtrl.truncate(row.entity.Text) }}' + '</div>',
                },
                {
                    name: 'AddDateFormatted',
                    displayName: $translate.instant('Admin.Js.Reviews.Added'),
                    width: 90,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Reviews.DateAndTime'),
                        type: 'datetime',
                        term: {
                            from: new Date(new Date().setMonth(new Date().getMonth() - 1)),
                            to: new Date(),
                        },
                        datetimeOptions: {
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
                    name: 'Checked',
                    displayName: $translate.instant('Admin.Js.Reviews.Checked'),
                    width: 80,
                    cellTemplate:
                        '<div class="ui-grid-cell-contents">' +
                        '<label class="ui-grid-custom-edit-field adv-checkbox-label"> ' +
                        '<input type="checkbox" class="adv-checkbox-input" ng-model="row.entity.Checked" disabled /> ' +
                        '<span class="adv-checkbox-emul"></span> ' +
                        '</label>' +
                        '</div>',
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Reviews.Checked'),
                        type: uiGridConstants.filter.SELECT,
                        selectOptions: [
                            {
                                label: $translate.instant('Admin.Js.Reviews.Yes'),
                                value: true,
                            },
                            {
                                label: $translate.instant('Admin.Js.Reviews.No'),
                                value: false,
                            },
                        ],
                    },
                },
                {
                    name: '_serviceColumn',
                    displayName: '',
                    width: 75,
                    enableSorting: false,
                    useInSwipeBlock: true,
                    cellTemplate:
                        '<div ng-if="!grid.appScope.$ctrl.isMobile" class="ui-grid-cell-contents"><div>' +
                        '<button type="button" class="btn-icon link-invert ui-grid-custom-service-icon fas fa-pencil-alt" ng-click="grid.appScope.$ctrl.gridExtendCtrl.openModal(row.entity.ReviewId)" aria-label="Редактировать"></button> ' +
                        '<ui-grid-custom-delete url="reviews/deleteReview" params="{\'reviewId\': row.entity.ReviewId }"></ui-grid-custom-delete>' +
                        '</div></div>' +
                        '<ui-grid-custom-delete ng-if="grid.appScope.$ctrl.isMobile" url="reviews/deleteReview" params="{\'reviewId\': row.entity.ReviewId }" class="btn btn-sm btn-danger btn--as-swipe-line flex center-xs middle-xs">Удалить</ui-grid-custom-delete>',
                },
            ];
        ctrl.$onInit = function () {
            ctrl.loadRatio();
        };
        ctrl.gridOptions = ng.extend({}, uiGridCustomConfig, {
            searchAutofocus: false,
            columnDefs: columnDefs,
            uiGridCustom: {
                rowClick: function ($event, row) {
                    ctrl.openModal(row.entity.ReviewId);
                },
                selectionOptions: [
                    {
                        text: $translate.instant('Admin.Js.Reviews.DeleteSelected'),
                        url: 'reviews/deleteReviews',
                        field: 'ReviewId',
                        before: function () {
                            return SweetAlert.confirm($translate.instant('Admin.Js.Reviews.AreYouSureDelete'), {
                                title: $translate.instant('Admin.Js.Reviews.Deleting'),
                            }).then(function (result) {
                                return result === true || result.value ? $q.resolve('sweetAlertConfirm') : $q.reject('sweetAlertCancel');
                            });
                        },
                    },
                ],
            },
            paginationPageSize: 3,
            paginationPageSizes: [3, 5, 10],
        });
        ctrl.gridOnInit = function (grid) {
            ctrl.grid = grid;
        };
        ctrl.truncate = function (str) {
            if (str.length > 120) {
                str = str.substring(0, 120) + '..';
            }
            return str;
        };
        ctrl.openModal = function (reviewId) {
            $uibModal
                .open({
                    bindToController: true,
                    controller: 'ModalAddEditReviewCtrl',
                    controllerAs: 'ctrl',
                    size: 'lg',
                    templateUrl: addEditReviewTemplate,
                    resolve: {
                        reviewId: function () {
                            return reviewId;
                        },
                    },
                })
                .result.then(function (result) {
                    ctrl.grid.fetchData();
                    return result;
                });
        };
        ctrl.loadRatio = function () {
            $http
                .get('product/getRatioByProductId', {
                    params: {
                        productId: ctrl.productId,
                    },
                })
                .then(function (response) {
                    ctrl.productManualRatio = response.data.ManualRatio;
                    ctrl.productPrevManualRatio = ctrl.productManualRatio;
                    ctrl.productRatio = response.data.Ratio;
                    ctrl.ratioType = ctrl.productManualRatio == null ? 'current' : 'manual';
                });
        };
        ctrl.changeType = function (type) {
            if (ctrl.ratioType != type) {
                ctrl.ratioType = type;
                if (ctrl.ratioType == 'manual') {
                    ctrl.productManualRatio = ctrl.productRatio;
                } else {
                    ctrl.productManualRatio = null;
                    ctrl.setManualRatio();
                }
            }
        };
        ctrl.changeManualRatio = function () {
            if (ctrl.productManualRatio < 0 || ctrl.productManualRatio > 5) {
                toaster.pop('error', '', 'Значение рейтинга может быть от 0 до 5');
            } else {
                ctrl.setManualRatio();
            }
        };
        ctrl.setManualRatio = function () {
            $http
                .post('product/setManualRatioByProductId', {
                    productId: ctrl.productId,
                    manualRatio: ctrl.productManualRatio,
                })
                .then(function (response) {
                    if (response.data.result == true) {
                        toaster.pop('success', '', $translate.instant('Admin.Js.Product.ChangesSuccessfullySaved'));
                        ctrl.productPrevManualRatio = ctrl.productManualRatio;
                    } else {
                        toaster.pop('error', '', $translate.instant('Admin.Js.ErrorWhileSaving'));
                    }
                });
        };
    };
    ProductReviewsCtrl.$inject = ['$http', 'uiGridConstants', 'uiGridCustomConfig', 'SweetAlert', '$q', '$uibModal', '$translate', 'toaster'];
    ng.module('productReviews', ['ui.select'])
        .controller('ProductReviewsCtrl', ProductReviewsCtrl)
        .component('productReviews', {
            templateUrl: productReviewsTemplate,
            controller: 'ProductReviewsCtrl',
            bindings: {
                productId: '@',
                isMobileMode: '<?',
            },
        });
})(window.angular);
