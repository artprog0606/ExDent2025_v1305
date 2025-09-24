/* @ngInject */
function SmsConfirmationInputCtrl($scope, $element) {
    const ctrl = this;
    const regexp = /\d{1}/;
    const keyServices = new Set([`Backspace`, `ArrowLeft`, `ArrowRight`]);

    const isNumber = (value) => regexp.test(value);

    //Unidentified - on mobile
    const getValue = (event) => (event.key === `Unidentified` ? event.target.value : event.key);

    const setValue = (value, index) => (ctrl.inputsList[index] = parseFloat(value[value.length - 1]));

    const onPaste = function (event) {
        if (event.clipboardData != null) {
            event.stopPropagation();
            event.preventDefault();

            const pastedData = event.clipboardData.getData('text/plain');

            if (pastedData != null && pastedData.length > 0 && pastedData.length <= ctrl.count) {
                const dataNormalized = pastedData.replaceAll(/\D/g, ``);

                if (dataNormalized.length > 0) {
                    const arrNewValues = dataNormalized.split(``).map((x) => parseFloat(x));
                    arrNewValues.length = arrNewValues.length > ctrl.count ? ctrl.count : arrNewValues.length;

                    let tempIndex = 0;
                    for (let i = ctrl.count - arrNewValues.length; i < ctrl.count; i++) {
                        ctrl.inputsList[i] = arrNewValues[tempIndex];
                        tempIndex += 1;
                    }
                    $scope.$digest();
                }
            }
        }
    };

    ctrl.$onInit = function () {
        ctrl.inputsList = new Array(ctrl.count);
        ctrl.itemIndexFocus = ctrl.focusOnStart === true ? 0 : -1;
        ctrl.maxIndex = ctrl.count - 1;
        ctrl.minIndex = 0;

        if (ctrl.onInit != null) {
            ctrl.onInit({ smsConfirmationInput: ctrl });
        }
    };

    ctrl.$postLink = function () {
        $element[0].addEventListener(`paste`, onPaste);
        $element.on('$destroy', function () {
            $element[0].removeEventListener(`paste`, onPaste);
        });
    };

    ctrl.keydown = function (event, index) {
        const value = getValue(event);
        const isKeyServices = keyServices.has(value);
        if (isNumber(value) === false && isKeyServices === false && event.ctrlKey === false) {
            event.preventDefault();
        }

        if (isKeyServices) {
            if (value === `ArrowLeft` || (value === `Backspace` && ctrl.inputsList[index] == null)) {
                ctrl.itemIndexFocus = Math.max(ctrl.minIndex, index - 1);
            } else if (value === `ArrowRight`) {
                ctrl.itemIndexFocus = Math.min(ctrl.maxIndex, index + 1);
            }
        }
    };

    ctrl.keyup = function (event, index) {
        const value = getValue(event);

        if (isNumber(value) === false || keyServices.has(value)) {
            return;
        }

        setValue(value, index);
        ctrl.itemIndexFocus = Math.min(ctrl.maxIndex, index + 1);

        if (ctrl.onComplete != null && ctrl.inputsList.every((x) => isNaN(x) === false)) {
            ctrl.onComplete({
                value: ctrl.inputsList.join(``),
            });
        }
    };

    ctrl.clear = function () {
        for (let i = 0; i < ctrl.inputsList.length; i++) {
            ctrl.inputsList[i] = null;
        }
    };
}

export default SmsConfirmationInputCtrl;
