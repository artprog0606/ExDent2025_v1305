(function (ng) {
    'use strict';

    /* @ngInject */
    var ModalAddEditSalesFunnelCtrl = function ($uibModalInstance, $http, toaster, $translate, $window, SweetAlert) {
        var ctrl = this;

        ctrl.$onInit = function () {
            var params = ctrl.$resolve;
            ctrl.id = params.Id;
            ctrl.mode = ctrl.id != null ? 'edit' : 'add';
            ctrl.leadFields = [];
            ctrl.tab = 'tab-common';

            if (ctrl.id != null) {
                ctrl.getSalesFunnel(ctrl.id);
            } else {
                ctrl.dealStatuses = [];
                ctrl.systemDealStatuses = [];
                $http.get('salesFunnels/getSalesFunnel').then(function (response) {
                    var data = response.data;
                    if (data.result == true) {
                        ctrl.Item = {
                            Enable: true,
                            FinalSuccessAction: 1,
                            LeadAutoCompleteActionType: 0,
                            Managers: data.obj.Managers,
                            LeadAutoCompleteActionTypes: data.obj.LeadAutoCompleteActionTypes,
                            FinalSuccessActions: data.obj.FinalSuccessActions,
                            LeadAutoCompleteProductIds: [],
                            LeadAutoCompleteCategoryIds: [],
                        };
                    } else {
                        ctrl.close();
                    }
                });
            }
        };

        ctrl.getSalesFunnel = function () {
            $http.get('salesFunnels/getSalesFunnel', { params: { id: ctrl.id } }).then(function (response) {
                var data = response.data;
                if (data.result == true) {
                    ctrl.Item = data.obj;
                } else {
                    toaster.error('', 'Список лидов не найден');
                    ctrl.close();
                }
            });
        };

        ctrl.close = function () {
            $uibModalInstance.dismiss('cancel');
        };

        ctrl.changeTab = function (tab) {
            ctrl.tab = tab;
        };

        ctrl.saveItem = function (item) {
            ctrl.btnSleep = true;

            var url = ctrl.mode == 'edit' ? 'salesFunnels/updateSalesFunnel' : 'salesFunnels/addSalesFunnel';
            if (ctrl.mode == 'add') {
                item.dealStatuses = ctrl.dealStatuses.concat(ctrl.systemDealStatuses);
                ctrl.Item.LeadFields = ctrl.leadFields;
            }
            $http.post(url, item).then(function (response) {
                var data = response.data;
                if (data.result === true) {
                    toaster.success('', $translate.instant('Admin.Js.SettingsCrm.ChangesSaved'));
                    $uibModalInstance.close(data.obj);
                    if (ctrl.mode == 'add' && $window.location.href.indexOf('settingscrm') == -1) {
                        $window.location.assign('leads?salesFunnelId=' + data.obj.Id);
                    }
                } else {
                    toaster.error('', (data.errors || [])[0] || $translate.instant('Admin.Js.ErrorWhileSaving'));
                }
                ctrl.btnSleep = false;
            });
        };

        ctrl.deleteItem = function (id) {
            SweetAlert.confirm($translate.instant('Admin.Js.SettingsCrm.AreYouSureDelete'), {
                title: $translate.instant('Admin.Js.SettingsCrm.Delete'),
            }).then(function (result) {
                if (result === true || result.value == true) {
                    $http.post('salesFunnels/deleteSalesFunnel', { id: id }).then(function (response) {
                        var data = response.data;
                        if (data.result === true) {
                            ctrl.fetch();
                            toaster.success('', $translate.instant('Admin.Js.SettingsCrm.ChangesSaved'));
                        } else {
                            toaster.error('', (data.errors || [])[0] || $translate.instant('Admin.Js.ErrorWhileSaving'));
                        }
                        if (ctrl.onChange != null) {
                            ctrl.onChange();
                        }
                    });
                }
            });
        };

        // #region lead autocomplete products and categories

        ctrl.selectLeadAutoCompleteProducts = function (data) {
            if (data == null || data.ids == null) return;

            data.ids.forEach(function (id) {
                if (ctrl.Item.LeadAutoCompleteProductIds.indexOf(id) != -1) return;
                ctrl.Item.LeadAutoCompleteProductIds.push(id);

                $http
                    .get('salesFunnels/getLeadAutoCompleteProductsInfo', {
                        params: { productIds: ctrl.Item.LeadAutoCompleteProductIds },
                    })
                    .then(function (response) {
                        ctrl.Item.LeadAutoCompleteProducts = (response.data || {}).obj || [];
                    });
            });
        };

        ctrl.deleteLeadAutoCompleteProduct = function (index) {
            ctrl.Item.LeadAutoCompleteProductIds.splice(index, 1);
            ctrl.Item.LeadAutoCompleteProducts.splice(index, 1);
        };

        ctrl.selectLeadAutoCompleteCategories = function (data) {
            if (data == null || data.categoryIds == null) return;

            data.categoryIds.forEach(function (id) {
                if (ctrl.Item.LeadAutoCompleteCategoryIds.indexOf(id) != -1) return;
                ctrl.Item.LeadAutoCompleteCategoryIds.push(id);

                $http
                    .get('salesFunnels/getLeadAutoCompleteCategoriesInfo', {
                        params: { categoryIds: ctrl.Item.LeadAutoCompleteCategoryIds },
                    })
                    .then(function (response) {
                        ctrl.Item.LeadAutoCompleteCategories = (response.data || {}).obj || [];
                    });
            });
        };

        ctrl.deleteLeadAutoCompleteCategory = function (index) {
            ctrl.Item.LeadAutoCompleteCategoryIds.splice(index, 1);
            ctrl.Item.LeadAutoCompleteCategories.splice(index, 1);
        };

        // #endregion
    };

    ng.module('uiModal').controller('ModalAddEditSalesFunnelCtrl', ModalAddEditSalesFunnelCtrl);
})(window.angular);
