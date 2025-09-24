import '../../../../../node_modules/baguettebox.js/dist/baguetteBox.min.css';
import baguetteBox from 'baguettebox.js';

(function (ng) {
    'use strict';

    var overlaySelector = '#baguetteBox-overlay';

    var AdvBaguetteBoxCtrl = function ($timeout, $document) {
        var ctrl = this;

        ctrl.init = function () {
            baguetteBox.run('#' + ctrl.id, ctrl.options);

            if (ctrl.onInit != null) {
                ctrl.onInit({ advBaguetteBox: ctrl });
            }
        };

        ctrl.reinit = function () {
            $timeout(function () {
                baguetteBox.destroy();
                ctrl.init();
            });
        };

        ctrl.getWindowElement = function (index) {
            return $document[0].querySelector('#baguetteBox-slider .full-image:nth-child(' + (index + 1) + ')');
        };

        ctrl.offTrapFocus = function () {
            $document[0].querySelector(overlaySelector).style.display = 'inline-block';
        };
    };

    AdvBaguetteBoxCtrl.$inject = ['$timeout', '$document'];

    ng.module('advBaguetteBox').controller('AdvBaguetteBoxCtrl', AdvBaguetteBoxCtrl);
})(window.angular);
