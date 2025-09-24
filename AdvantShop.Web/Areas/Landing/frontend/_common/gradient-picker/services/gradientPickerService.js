(function (ng) {
    'use strict';

    var gradientPickerService = function () {
        var service = this,
            regexp = /^linear-gradient\(to \w+, (.+) 0%, (.+) 50%, (.+) 100%\)$/,
            tplString = 'linear-gradient(to {direction}, {startColor} 0%, {middleColor} 50%, {endColor} 100%)';

        service.getString = function (direction, startColor, middleColor, endColor) {
            return tplString
                .replace('{direction}', direction)
                .replace('{startColor}', startColor)
                .replace('{middleColor}', middleColor)
                .replace('{endColor}', endColor);
        };

        service.parse = function (value) {
            var matches = regexp.exec(value);

            return matches != null ? { startColor: matches[1], middleColor: matches[2], endColor: matches[3] } : null;
        };

        service.checkGradient = function (value) {
            return regexp.test(value);
        };
    };

    ng.module('gradientPicker').service('gradientPickerService', gradientPickerService);

    gradientPickerService.$inject = [];
})(window.angular);
