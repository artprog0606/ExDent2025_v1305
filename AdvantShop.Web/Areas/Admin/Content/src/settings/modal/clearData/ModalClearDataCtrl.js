(function (ng) {
    'use strict';

    var ModalClearDataCtrl = function ($uibModalInstance, $http, toaster, $translate) {
        var ctrl = this;
        ctrl.formInited = false;

        ctrl.$onInit = function () {
            var params = ctrl.$resolve != null ? ctrl.$resolve.params || {} : {};
            switch (params.mode) {
                case 'catalog':
                    ctrl.deleteCategoties = true;
                    ctrl.deleteProducts = true;
                    ctrl.deleteBrands = true;
                    break;
                default:
                    break;
            }

            ctrl.formInited = true;
        };

        ctrl.close = function () {
            $uibModalInstance.dismiss('cancel');
        };

        ctrl.delete = function () {
            if (!ctrl.confirm) return;

            var isSelectSome =
                ctrl.deleteCategoties ||
                ctrl.deleteProducts ||
                ctrl.deleteProperty ||
                ctrl.deleteBrands ||
                ctrl.deleteOrder ||
                ctrl.deleteCustomers ||
                ctrl.deleteSubscription ||
                ctrl.deleteMenu ||
                ctrl.deletePage ||
                ctrl.deleteNews ||
                ctrl.deleteCarosel ||
                ctrl.deleteShippings ||
                ctrl.deletePayments ||
                ctrl.deleteUsers ||
                ctrl.deleteTasks ||
                ctrl.deleteCrm;

            if (!isSelectSome) {
                toaster.pop('info', '', $translate.instant('Admin.Js.ClearData.SelectTheItems'));
                return;
            }

            ctrl.btnSleep = true;

            var params = {
                deleteCategoties: ctrl.deleteCategoties,
                deleteProducts: ctrl.deleteProducts,
                deleteProperty: ctrl.deleteProperty,
                deleteBrands: ctrl.deleteBrands,
                deleteOrder: ctrl.deleteOrder,
                deleteCustomers: ctrl.deleteCustomers,
                deleteSubscription: ctrl.deleteSubscription,
                deleteMenu: ctrl.deleteMenu,
                deletePage: ctrl.deletePage,
                deleteNews: ctrl.deleteNews,
                deleteCarosel: ctrl.deleteCarosel,
                deleteShippings: ctrl.deleteShippings,
                deletePayments: ctrl.deletePayments,
                deleteUsers: ctrl.deleteUsers,
                deleteTasks: ctrl.deleteTasks,
                deleteCrm: ctrl.deleteCrm,
                rnd: Math.random(),
            };

            var url = 'settings/clearData';

            $http.post(url, params).then(
                function (response) {
                    var data = response.data;
                    if (data.result == true) {
                        toaster.pop('success', '', $translate.instant('Admin.Js.Settings.Settings.SpecifiedDataDeleted'));
                        $uibModalInstance.close();
                        window.location.reload();
                    } else {
                        toaster.pop(
                            'error',
                            $translate.instant('Admin.Js.SettingsUsers.Error'),
                            $translate.instant('Admin.Js.Settings.Settings.ErrorWhileDeletingData'),
                        );
                        ctrl.btnSleep = false;
                    }
                },
                function (respose) {
                    toaster.pop(
                        'error',
                        $translate.instant('Admin.Js.SettingsUsers.Error'),
                        $translate.instant('Admin.Js.Settings.Settings.ErrorWhileDeletingData'),
                    );
                },
            );
        };
    };

    ModalClearDataCtrl.$inject = ['$uibModalInstance', '$http', 'toaster', '$translate'];

    ng.module('uiModal').controller('ModalClearDataCtrl', ModalClearDataCtrl);
})(window.angular);
