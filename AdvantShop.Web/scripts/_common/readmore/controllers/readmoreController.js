(function (ng) {
    'use strict';

    var ReadmoreCtrl = function ($element, $timeout, readmoreConfig, $window, $scope) {
        var ctrl = this;
        var timer;

        ctrl.$onInit = function () {
            ctrl.maxHeight = ctrl.maxHeight || readmoreConfig.maxHeight;
            ctrl.moreText = ctrl.moreText || readmoreConfig.moreText;
            ctrl.lessText = ctrl.lessText || readmoreConfig.lessText;
            ctrl.speed = ctrl.speed || readmoreConfig.speed;
            ctrl.expanded = ctrl.expanded != null ? ctrl.expanded : readmoreConfig.expanded;
            ctrl.isLineClampMode = Boolean(ctrl.lineClamp);
            if (ctrl.isLineClampMode) {
                $element[0].style.setProperty('--line-clamp', ctrl.lineClamp);
            }
        };

        ctrl.init = function () {
            ctrl.checkSizes($element).then(function (isActive) {
                ctrl.isActive = isActive;
                if (!ctrl.isLineClampMode) {
                    ctrl.expanded = ctrl.isActive === true ? ctrl.expanded : true;
                } else {
                    ctrl.expanded = ctrl.isActive !== true;
                }

                ctrl.text = ctrl.expanded ? ctrl.lessText : ctrl.moreText;
            });
        };

        ctrl.$postLink = function () {
            ctrl.init();
            $window.addEventListener('resize', ctrl.init);
            $scope.$on('$destroy', function () {
                $window.removeEventListener('resize', ctrl.init);
            });
            ctrl.$onChanges = function (changesObj) {
                ctrl.init();
            };
        };

        ctrl.switch = function (expanded) {
            if (expanded === true) {
                ctrl.expanded = false;
                ctrl.text = ctrl.moreText;
            } else {
                ctrl.expanded = true;
                ctrl.text = ctrl.lessText;
            }
        };

        ctrl.checkSizes = function ($el) {
            if (timer != null) {
                $timeout.cancel(timer);
            }

            timer = $timeout(function () {
                var content = $element.find('.js-readmore-inner-content'),
                    clone = content.clone(),
                    result = false;

                clone.addClass('readmore-unvisible').css('width', content.width());
                $element.after(clone);
                if (ctrl.isLineClampMode) {
                    clone.addClass('readmore-reset');
                    const originalHeight = clone[0].offsetHeight;
                    clone.removeClass('readmore-reset');
                    clone[0].style.setProperty('--line-clamp', ctrl.lineClamp);
                    clone.addClass('readmore-clamp');
                    const clampHeight = clone[0].offsetHeight;
                    result = originalHeight > clampHeight;
                } else {
                    result = ctrl.maxHeight < clone[0].offsetHeight;
                }
                clone.remove();
                return result;
            });

            return timer;
        };
    };

    ReadmoreCtrl.$inject = ['$element', '$timeout', 'readmoreConfig', '$window', '$scope'];

    angular.module('readmore').controller('ReadmoreCtrl', ReadmoreCtrl);
})(window.angular);
