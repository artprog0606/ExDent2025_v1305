(function (ng) {
    'use strict';

    var ModalListRulesCtrl = function ($uibModalInstance, $http, $window, toaster, $q, $translate) {
        var ctrl = this;

        ctrl.$onInit = function () {
            $http.get('rules/GetRuleTypes').then(
                function (result) {
                    ctrl.RuleTypes = result.data.obj;
                },
                function (err) {
                    toaster.pop('error', '', $translate.instant('Admin.Js.SmsTemplate.ErrorFetchingRules') + err);
                },
            );
        };

        ctrl.close = function () {
            $uibModalInstance.dismiss('cancel');
        };

        ctrl.saveRule = function () {
            ctrl.btnLoading = true;
            $http
                .post('rules/create', {
                    RuleType: ctrl.RuleType,
                })
                .then(
                    function (result) {
                        var data = result.data.result;
                        if (data === true) {
                            $window.location.assign('rules/edit/' + ctrl.RuleType);
                            toaster.pop('success', '', $translate.instant('Admin.Js.Rule.RuleAdded'));
                        } else {
                            toaster.pop('error', '', $translate.instant('Admin.Js.Rule.ErrorAddingRule'));
                        }
                    },
                    function () {
                        toaster.pop('error', '', $translate.instant('Admin.Js.Rule.ErrorAddingRule'));
                    },
                )
                .finally(function () {
                    ctrl.btnLoading = false;
                });
        };
    };

    ModalListRulesCtrl.$inject = ['$uibModalInstance', '$http', '$window', 'toaster', '$q', '$translate'];

    ng.module('uiModal').controller('ModalListRulesCtrl', ModalListRulesCtrl);
})(window.angular);
