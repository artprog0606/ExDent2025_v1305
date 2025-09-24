(function (ng) {
    'use strict';

    var BackgroundPickerCtrl = function (backgroundPickerService, gradientPickerService) {
        var ctrl = this;

        ctrl.$onInit = function () {
            ctrl.colorPickerOptions = {
                swatchBootstrap: false,
                format: 'rgb',
                alpha: true,
                case: 'lower',
                swatchOnly: false,
                allowEmpty: true,
                required: false,
                preserveInputFormat: false,
                restrictToFormat: false,
                inputClass: 'blocks-constructor-input',
            };

            ctrl.colorPickerEventApi = {
                onChange: function (colorPicker, value, $event) {
                    var colorPickerCtrl = colorPicker.getScope().AngularColorPickerController;

                    colorPickerCtrl.setNgModel(value);

                    ctrl.changeColorPicker($event, value);
                },
                onBlur: function (colorPicker, value, $event) {
                    if (value.length === 6) {
                        var colorPickerCtrl = colorPicker.getScope().AngularColorPickerController;

                        if (value.indexOf('rgb') === -1) {
                            value = tinycolor(colorPickerCtrl.getColorValue()).toRgbString();
                        }

                        colorPickerCtrl.setNgModel(value);

                        ctrl.changeColorPicker($event, value);
                    }
                },
            };

            ctrl.updateValue();
            ctrl.selectedColorVariant = ctrl._colorSelected;
            if (ctrl.onInit != null) {
                ctrl.onInit({ backgroundPicker: ctrl });
            }
        };

        ctrl.updateValue = function () {
            //if (gradientPickerService.checkGradient(ctrl.colorSelected) === true) {
            //    var colorsGradient = gradientPickerService.parse(ctrl.colorSelected);

            //    ctrl.isShowGradientPanel = true;

            //    ctrl.startColor = colorsGradient.startColor;
            //    ctrl.middleColor = colorsGradient.middleColor;
            //    ctrl.endColor = colorsGradient.endColor;
            //} else {
            var color = ctrl.findSelectedColor(ctrl.colors, ctrl.colorSelected);

            if (color != null) {
                ctrl._colorSelected = color;
            } else {
                ctrl.isShowCustomColors = true;
                ctrl.customColor = ctrl.colorSelected;
            }
            //}
        };

        ctrl.changeColor = function (event, color) {
            ctrl.isShowCustomColors = false;

            ctrl.customColor = color.ColorCode;

            ctrl.colorCodeSelected = color.ColorCode;

            ctrl.onUpdate({ cssString: ctrl.colorCodeSelected, type: 'color' });
        };

        ctrl.changeColorPicker = function (event, color) {
            ctrl.colorCodeSelected = color;

            ctrl.onUpdate({ cssString: ctrl.colorCodeSelected, type: 'color' });
        };

        //ctrl.changeUseGradient = function (state) {
        //    ctrl.processColors(ctrl.colorCodeSelected, state);
        //};

        //ctrl.changeGradient = function (cssString) {
        //    ctrl.onUpdate({ cssString: cssString, type: 'gradient' });
        //};

        //ctrl.processColors = function (colorCode, state) {

        //    var colorGeneral = colorCode,
        //        colorAlt,
        //        cssString;

        //    colorAlt = state === true ? backgroundPickerService.colorLuminance(colorGeneral, 0.4) : colorGeneral;

        //    cssString = colorGeneral !== colorAlt ? gradientPickerService.getString('right', colorGeneral, colorAlt, colorGeneral) : colorGeneral;

        //    ctrl.startColor = colorGeneral;
        //    ctrl.middleColor = colorAlt;
        //    ctrl.endColor = colorGeneral;

        //    ctrl.onUpdate({ cssString: cssString, type: 'gradient' });
        //};

        ctrl.findSelectedColor = function (colorsArray, currentColor) {
            var defaultColor;

            for (var i = 0, len = colorsArray.length; i < len; i++) {
                if (colorsArray[i].ColorCode === currentColor) {
                    defaultColor = colorsArray[i];
                    break;
                }
            }

            return defaultColor;
        };

        ctrl.changeCustomBackgroundColor = function (useCustomBackgroundColor) {
            if (!useCustomBackgroundColor) {
                ctrl.changeColor(null, ctrl.selectedColorVariant);
            } else {
                if (!ctrl.customColor) {
                    ctrl.customColor = ctrl._colorSelected?.ColorCode;
                }
                ctrl._colorSelected = ctrl.customColor;
            }
        };
    };

    ng.module('backgroundPicker').controller('BackgroundPickerCtrl', BackgroundPickerCtrl);

    BackgroundPickerCtrl.$inject = ['backgroundPickerService', 'gradientPickerService'];
})(window.angular);
