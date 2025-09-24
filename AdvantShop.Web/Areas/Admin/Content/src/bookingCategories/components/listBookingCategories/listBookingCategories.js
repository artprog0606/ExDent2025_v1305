import listBookingCategoriesTemplate from './listBookingCategories.html';
(function (ng) {
    'use strict';

    var ListBookingCategoriesCtrl = function ($http, toaster, SweetAlert, bookingCategoriesService, $translate) {
        var ctrl = this;

        ctrl.$onInit = function () {
            ctrl.bindingToAffiliate = !!ctrl.affiliateId;

            ctrl.fetch();

            if (ctrl.onInit != null) {
                ctrl.onInit({ items: ctrl.items });
            }

            if (ctrl.giveData != null) {
                ctrl.giveData({ data: ctrl });
            }
        };

        ctrl.fetch = function () {
            ctrl.isProcessing = true;

            var getCategoriesCallBack = ctrl.bindingToAffiliate
                ? bookingCategoriesService.getCategoriesRefAffiliate(ctrl.affiliateId)
                : bookingCategoriesService.getCategories();

            getCategoriesCallBack.then(function (data) {
                ctrl.items = data.items || [];
                ctrl.items.push({ Id: null, AffiliateId: ctrl.affiliateId, Name: '', SortOrder: 500, Enabled: true });

                ctrl.isProcessing = false;
            });
        };

        ctrl.sortableOptions = {
            orderChanged: function (event) {
                var id = event.source.itemScope.item.Id,
                    prev = ctrl.items[event.dest.index - 1],
                    next = ctrl.items[event.dest.index + 1];

                bookingCategoriesService
                    .changeCategorySorting(id, prev != null ? prev.Id : null, next != null ? next.Id : null)
                    .then(function (data) {
                        if (data.result === true) {
                            toaster.pop('success', '', $translate.instant('Admin.Js.ListBookingCategories.ChangesSaved'));
                        } else {
                            data.errors.forEach(function (error) {
                                toaster.pop('error', error);
                            });
                            ctrl.fetch();
                        }
                    });
            },
        };

        ctrl.bindToAffiliate = function (categoryId, checked, skipConfirm) {
            if (!checked && !skipConfirm) {
                SweetAlert.confirm($translate.instant('Admin.Js.BookingCategories.AreYouSureUntie'), {
                    title: '',
                }).then(function (result) {
                    if (result === true || result.value === true) {
                        ctrl.bindToAffiliate(categoryId, checked, true);
                    } else {
                        ctrl.fetch();
                    }
                });
            } else {
                $http
                    .post('bookingCategory/bindToAffiliate', {
                        categoryId: categoryId,
                        affiliateId: ctrl.affiliateId,
                        bind: checked,
                    })
                    .then(function (response) {
                        if (response.data.result === true) {
                            toaster.pop('success', '', $translate.instant('Admin.Js.BookingCategories.ChangesSaved'));
                        } else {
                            response.data.errors.forEach(function (error) {
                                toaster.pop('error', error);
                            });

                            if (!response.data.errors.length) {
                                toaster.pop('error', '', $translate.instant('Admin.Js.BookingCategories.CouldNotSaveChanges'));
                            }
                        }
                        ctrl.fetch();
                    });
            }
        };

        ctrl.deleteItem = function (categoryId) {
            SweetAlert.confirm($translate.instant('Admin.Js.BookingCategories.AreYouSureDelete'), {
                title: $translate.instant('Admin.Js.BookingCategories.Delete'),
            }).then(function (result) {
                if (result === true || result.value === true) {
                    bookingCategoriesService.deleteCategory(categoryId).then(function (data) {
                        if (data.result === true) {
                            toaster.pop('success', '', $translate.instant('Admin.Js.BookingCategories.ChangesSaved'));
                        } else {
                            toaster.pop('error', '', $translate.instant('Admin.Js.BookingCategories.FailedToDeleteCategory'));
                        }
                        ctrl.fetch();
                    });
                }
            });
        };

        ctrl.addItem = function () {
            $http
                .post('bookingCategory/addCategory', {
                    AffiliateId: ctrl.affiliateId,
                    Name: ctrl.newName,
                    Enabled: true,
                })
                .then(function (result) {
                    if (result.data.result === true) {
                        ctrl.newName = '';
                        toaster.pop('success', '', $translate.instant('Admin.Js.BookingCategories.ChangesSaved'));
                    } else {
                        result.data.errors.forEach(function (error) {
                            toaster.pop('error', error);
                        });
                    }
                    ctrl.fetch();
                });
        };

        ctrl.refresh = function (params) {
            ctrl.fetch();
            ctrl.bindToAffiliate(params.id, params.enabled);
        };
    };

    ListBookingCategoriesCtrl.$inject = ['$http', 'toaster', 'SweetAlert', 'bookingCategoriesService', '$translate'];

    ng.module('listBookingCategories', ['as.sortable'])
        .controller('ListBookingCategoriesCtrl', ListBookingCategoriesCtrl)
        .component('listBookingCategories', {
            templateUrl: listBookingCategoriesTemplate,
            controller: 'ListBookingCategoriesCtrl',
            transclude: true,
            bindings: {
                onInit: '&',
                affiliateId: '<',
                giveData: '&',
            },
        });
})(window.angular);
