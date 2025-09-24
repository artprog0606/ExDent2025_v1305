import { v4 as uuidv4 } from 'uuid';

(function (ng) {
    'use strict';

    /* @ngInject */
    var ModalAddBonusCtrl = function ($uibModalInstance, $http, $window, toaster, $q, $translate) {
        var ctrl = this;
        ctrl.isStartExport = false;
        ctrl.isProgressbarFinish = false;
        ctrl.showReloadMassAddBtn = false;

        ctrl.$onInit = function () {
            var params = ctrl.$resolve.params;
            ctrl.cardId = params != null && params.cardId != null ? params.cardId : null;
            ctrl.filter = params != null && params.filter != null ? params.filter : null;
            ctrl.SendSms = params != null && params.sendSms != undefined ? params.sendSms : true;
            if (ctrl.cardId == null) {
                ctrl.idempotenceKey = uuidv4(); //todo: generate uuid or random string up to 50 characters
            }
        };

        ctrl.close = function () {
            $uibModalInstance.dismiss('cancel');
        };

        ctrl.addBonus = function () {
            ctrl.btnLoading = true;
            if (ctrl.cardId != null) {
                ctrl.singleAdd();
            } else {
                ctrl.massAdd();
            }
        };

        ctrl.singleAdd = function () {
            $http
                .post('cards/addBonus', {
                    cardId: ctrl.cardId,
                    amount: ctrl.amount,
                    reason: ctrl.reason,
                    name: ctrl.name,
                    startDate: ctrl.startDate,
                    endDate: ctrl.endDate,
                    sendsms: ctrl.SendSms,
                })
                .then(function (result) {
                    var data = result.data.result;
                    if (data === true) {
                        toaster.pop('success', '', $translate.instant('Admin.Js.AdditionBonus.BonusesAdded'));
                        $window.location.assign('cards/edit/' + ctrl.cardId);
                        return data;
                    } else {
                        return $q.reject(data);
                    }
                })
                .catch(function () {
                    toaster.pop('error', '', $translate.instant('Admin.Js.AdditionBonus.ErrorAddingBonuses'));
                    ctrl.btnLoading = false;
                });
        };

        ctrl.massAdd = function () {
            $http.post('ExportImportCommon/GetCommonStatistic').then(function (response) {
                if (!response.data.IsRun) {
                    $http
                        .post('cards/addBonusmass', {
                            amount: ctrl.amount,
                            reason: ctrl.reason,
                            name: ctrl.name,
                            startDate: ctrl.startDate,
                            endDate: ctrl.endDate,
                            sendsms: ctrl.SendSms,
                            idempotenceKey: ctrl.idempotenceKey,
                            filter: ctrl.filter,
                        })
                        .then(
                            function (result) {
                                if (result.data.result === true && result.data.obj === true) {
                                    ctrl.isStartExport = true;
                                    if (response.data.Error === 0) {
                                        toaster.pop('success', '', $translate.instant('Admin.Js.AdditionBonus.AdditionStarted'));
                                    }
                                } else {
                                    toaster.pop(
                                        'error',
                                        '',
                                        result.data.errors != null && result.data.errors.length > 0
                                            ? result.data.errors[0]
                                            : $translate.instant('Admin.Js.AdditionBonus.ErrorAddingBonuses'),
                                    );
                                    ctrl.btnLoading = false;
                                }
                            },
                            function () {
                                toaster.pop('error', '', $translate.instant('Admin.Js.AdditionBonus.ErrorAddingBonuses'));
                                ctrl.btnLoading = false;
                            },
                        )
                        .then(function () {
                            ctrl.isProgressbarFinish = true;
                        })
                        .finally(function () {
                            if (response.data.Error > 0) {
                                toaster.pop('error', '', $translate.instant('Admin.Js.AdditionBonus.ErrorAddingBonuses'));
                                ctrl.showReloadMassAddBtn = true;
                            }
                        });
                } else {
                    toaster.error(
                        '',
                        $translate.instant('Admin.Js.CommonStatistic.AlreadyRunning') +
                            ' <a href="' +
                            response.data.CurrentProcess +
                            '">' +
                            (response.data.CurrentProcessName || response.data.CurrentProcess) +
                            '</a>',
                    );
                    ctrl.btnLoading = false;
                }
            });
        };

        ctrl.reloadProgressbar = function (cmStat) {
            ctrl.isStartExport = false;
            ctrl.isProgressbarFinish = false;
        };

        ctrl.onTick = function (data) {
            ctrl.btnLoading = data.IsRun;
        };

        ctrl.onChangeSwitch = function (checked) {
            ctrl.SendSms = checked;
        };
    };

    ng.module('uiModal').controller('ModalAddBonusCtrl', ModalAddBonusCtrl);
})(window.angular);
