(function (ng) {
    'use strict';

    /* @ngInject */
    var ModalSubtractBonusCtrl = function ($uibModalInstance, $http, $window, toaster, $q, $translate) {
        var ctrl = this;

        ctrl.$onInit = function () {
            var params = ctrl.$resolve.params;
            ctrl.cardId = params != null && params.cardId != null ? params.cardId : null;
            ctrl.SendSms = params != null && params.sendSms != undefined ? params.sendSms : true;

            $http.get('cards/getBonuses?cardId=' + ctrl.cardId).then(
                function (result) {
                    ctrl.additionBonuses = result.data.obj;
                },
                function (err) {
                    toaster.pop('error', '', $translate.instant('Admin.Js.AdditionBonus.ErrorGettingBonuses') + err);
                },
            );
        };

        ctrl.close = function () {
            $uibModalInstance.dismiss('cancel');
        };

        ctrl.subctractBonus = function () {
            $http
                .post('cards/subtractBonus', {
                    cardId: ctrl.cardId,
                    amount: ctrl.amount,
                    reason: ctrl.reason,
                    additionId: ctrl.additionId,
                    sendsms: ctrl.SendSms,
                })
                .then(function (result) {
                    var data = result.data;
                    if (data.result === true) {
                        $window.location.assign('cards/edit/' + ctrl.cardId);
                        toaster.pop('success', '', $translate.instant('Admin.Js.AdditionBonus.BonusesAreWrittenOff'));
                    } else {
                        ctrl.btnLoading = false;
                        if (data.errors && data.errors.length) {
                            data.errors.forEach(function (error) {
                                toaster.pop('error', error);
                            });
                        } else {
                            toaster.pop('error', '', $translate.instant('Admin.Js.MainBonus.ErrorWritingOffBonuses'));
                        }
                    }
                })
                .catch(function () {
                    toaster.pop('error', '', $translate.instant('Admin.Js.MainBonus.ErrorWritingOffBonuses'));
                });
        };

        ctrl.onChangeSwitch = function (checked) {
            ctrl.SendSms = checked;
        };
    };

    ng.module('uiModal').controller('ModalSubtractBonusCtrl', ModalSubtractBonusCtrl);
})(window.angular);
