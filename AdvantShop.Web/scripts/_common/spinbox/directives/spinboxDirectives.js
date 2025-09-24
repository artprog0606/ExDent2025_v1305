import spinboxTemplate from '../templates/spinbox.html';

(function (ng) {
    'use strict';

    angular
        .module('spinbox')
        .directive('spinbox', [
            '$filter',
            function ($filter) {
                return {
                    restrict: 'A',
                    scope: {
                        value: '=',
                        proxy: '=?',
                        min: '<?',
                        max: '<?',
                        step: '<?',
                        beforeUpdate: '&',
                        validationBeforeUpdateFn: '&',
                        updateFn: '&',
                        validationText: '@',
                        inputClasses: '<?',
                        inputClassSize: '<?',
                        inputName: '@?',
                        isVisibleArrows: '<?',
                        isRequired: '<?',
                        needComma: '<?',
                        debounce: '<?',
                        /**
                         *  Валидация переносится в директиву spinboxInput.
                         *  Проиходит только проверка значения, без его корректировки
                         * */
                        onlyValidation: '<?',
                    },
                    replace: true,
                    bindToController: true,
                    templateUrl: spinboxTemplate,
                    controller: 'SpinboxCtrl',
                    controllerAs: 'spinbox',
                };
            },
        ])
        .directive('spinboxInput', [
            function () {
                return {
                    require: {
                        ngModelCtrl: 'ngModel',
                        spinboxCtrl: '^^spinbox',
                    },
                    bindToController: true,
                    /* @ngInject */
                    controller: function ($scope, spinboxTooltipTextType) {
                        var ctrl = this;
                        ctrl.$onInit = function () {
                            if (ctrl.spinboxCtrl.onlyValidation === false) {
                                return;
                            }
                            const watchUpdate = (newValue, oldValue) => {
                                if (ctrl.spinboxCtrl.numberToFloat(newValue) === ctrl.spinboxCtrl.numberToFloat(oldValue)) return;
                                ctrl.ngModelCtrl.$setDirty();
                                ctrl.ngModelCtrl.$validate();
                            };
                            ctrl.ngModelCtrl.$validators.spinboxInput = function (modelValue, viewValue) {
                                const [value, min, max, step] = [viewValue, ctrl.spinboxCtrl.min, ctrl.spinboxCtrl.max, ctrl.spinboxCtrl.step].map(
                                    ctrl.spinboxCtrl.numberToFloat,
                                );

                                if (Number.isNaN(value) && ctrl.spinboxCtrl.isRequired === false) return true;

                                if (value < min) {
                                    ctrl.spinboxCtrl.setValidationText(ctrl.spinboxCtrl.getTooltipText(spinboxTooltipTextType.min));
                                    ctrl.ngModelCtrl.$setDirty();
                                    return false;
                                }

                                if (value > max) {
                                    ctrl.spinboxCtrl.setValidationText(ctrl.spinboxCtrl.getTooltipText(spinboxTooltipTextType.max));

                                    ctrl.ngModelCtrl.$setDirty();
                                    return false;
                                }
                                if (ctrl.spinboxCtrl.isShareWhole(value, step) === false && step !== 0) {
                                    ctrl.spinboxCtrl.setValidationText(ctrl.spinboxCtrl.getTooltipText(spinboxTooltipTextType.multiplicity));
                                    ctrl.ngModelCtrl.$setDirty();
                                    return false;
                                }
                                return true;
                            };

                            $scope.$watch('spinbox.step', watchUpdate);
                            $scope.$watch('spinbox.min', watchUpdate);
                            $scope.$watch('spinbox.max', watchUpdate);
                        };
                    },
                };
            },
        ]);
})(window.angular);
