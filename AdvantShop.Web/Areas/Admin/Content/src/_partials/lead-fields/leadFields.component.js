(function (ng) {
    'use strict';

    var LeadFieldsCtrl = function ($http, $compile, $element, $scope, $timeout) {
        var ctrl = this;
        var scopeChild;
        ctrl.$onInit = function () {
            ctrl.loadForm();
            if (ctrl.onInit) {
                ctrl.onInit({ reloadFn: ctrl.loadForm });
            }
        };

        ctrl.loadForm = function () {
            ctrl.leadfieldsJs = [];
            $timeout(function () {
                $http
                    .get('leadFields/leadFieldsForm', {
                        params: { leadId: ctrl.leadId, salesFunnelId: ctrl.salesFunnelId, rnd: Math.random() },
                    })
                    .then(function (response) {
                        var el = ng.element(response.data);
                        if (scopeChild != null) {
                            scopeChild.$destroy();
                        }
                        scopeChild = $scope.$new();
                        $element.empty();
                        $element.append(el);
                        $compile(el)(scopeChild);
                    });
            }, 0);
        };

        ctrl.onChange = function () {
            if (ctrl.onLeadFieldsChange) {
                ctrl.onLeadFieldsChange();
            }
        };
    };

    LeadFieldsCtrl.$inject = ['$http', '$compile', '$element', '$scope', '$timeout'];

    ng.module('leadFields', [])
        .controller('LeadFieldsCtrl', LeadFieldsCtrl)
        .component('leadFields', {
            controller: LeadFieldsCtrl,
            bindings: {
                leadfieldsJs: '=',
                leadId: '<?',
                onInit: '&',
                onLeadFieldsChange: '&',
                salesFunnelId: '<?',
            },
        });
})(window.angular);
