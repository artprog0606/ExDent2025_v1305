(function (ng) {
    'use strict';
    /* @ngInject */
    var CustomerFieldsCtrl = function ($http, $compile, $element, $scope, $timeout) {
        var ctrl = this,
            timerProcessCompanyName;

        ctrl.$onInit = function () {
            ctrl.orientation = ctrl.orientation || 'horizontal';

            ctrl.init();
            ctrl.loadForm();

            if (ctrl.onInit) {
                ctrl.onInit({
                    reloadFn: ctrl.loadForm,
                });
            }
        };

        ctrl.init = function () {
            ctrl.allCustomerfieldsJs = ng.copy(ctrl.customerfieldsJs);
        };

        ctrl.loadForm = function () {
            return $timeout(function () {
                return $http
                    .post('customers/customerFieldsForm', {
                        customerFields: ctrl.allCustomerfieldsJs,
                        customerId: ctrl.customerId,
                        customerType: ctrl.customerType,
                        ignoreRequired: !!ctrl.ignoreRequired,
                    })
                    .then(function (response) {
                        const el = ng.element(
                            '<ng-form class="' +
                                (ctrl.orientation === 'horizontal' ? 'flex flex-wrap flex-grow' : '') +
                                '">' +
                                response.data +
                                '</ng-form>',
                        );
                        if (ctrl.childScope != null) {
                            ctrl.childScope.$destroy();
                        }
                        ctrl.childScope = $scope.$new();
                        $element.empty();
                        $element.append(el);
                        $compile(el)(ctrl.childScope);
                    });
            }, 0);
        };

        ctrl.processCompanyName = function (item) {
            if (timerProcessCompanyName != null) {
                $timeout.cancel(timerProcessCompanyName);
            }

            return (timerProcessCompanyName = $timeout(
                function () {
                    if (item != null && item.CompanyData) {
                        ctrl.customerfieldsJs.forEach(function (field) {
                            if (field.FieldAssignment == 1) field.Value = item.CompanyData.CompanyName;
                            else if (field.FieldAssignment == 2) field.Value = item.CompanyData.LegalAddress;
                            else if (field.FieldAssignment == 3) field.Value = item.CompanyData.INN;
                            else if (field.FieldAssignment == 4) field.Value = item.CompanyData.KPP;
                            else if (field.FieldAssignment == 5) field.Value = item.CompanyData.OGRN;
                            else if (field.FieldAssignment == 6) field.Value = item.CompanyData.OKPO;
                        });
                    } else if (item != null && item.BankData) {
                        ctrl.customerfieldsJs.forEach(function (field) {
                            if (field.FieldAssignment == 7) field.Value = item.BankData.BIK;
                            else if (field.FieldAssignment == 8) field.Value = item.BankData.BankName;
                            else if (field.FieldAssignment == 9) field.Value = item.BankData.CorrespondentAccount;
                        });
                    }
                },
                item != null ? 0 : 700,
            ));
        };
    };

    ng.module('customerFields', [])
        .controller('CustomerFieldsCtrl', CustomerFieldsCtrl)
        .component('customerFields', {
            controller: CustomerFieldsCtrl,
            transclude: true,
            bindings: {
                customerfieldsJs: '=?',
                customerId: '<?',
                onInit: '&',
                customerType: '<',
                ignoreRequired: '<',
                orientation: '<?',
            },
        });
})(window.angular);
