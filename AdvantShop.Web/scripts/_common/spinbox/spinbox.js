import '../../../styles/common/tooltip.scss';
import tooltip from 'angular-ui-bootstrap/src/tooltip/index.js';

(function (ng) {
    'use strict';

    angular
        .module('spinbox', [tooltip])
        .constant('spinboxKeyCodeAllow', {
            backspace: 8,
            delete: 46,
            decimalPoint: 110,
            comma: 188,
            period: 190,
            forwardSlash: 191,
            leftArrow: 37,
            rightArrow: 39,
            upArrow: 38,
            downArrow: 40,
        })
        .constant('spinboxTooltipTextType', {
            min: 'MIN',
            max: 'MAX',
            multiplicity: 'MULTIPLICITY',
        });
})(window.angular);
