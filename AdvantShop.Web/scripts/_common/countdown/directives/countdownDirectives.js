import countdownTemplate from '../templates/countdown.html';

(function (ng) {
    'use strict';

    ng.module('countdown').directive('countdown', function () {
        return {
            restrict: 'A',
            scope: {
                endTime: '=',
                endTimeUtc: '=',
                isShowDays: '<?',
                onFinish: '&',
            },
            replace: true,
            controller: 'CountdownCtrl',
            controllerAs: 'countdown',
            bindToController: true,
            templateUrl: function (element, attrs) {
                return attrs.templateUrl || countdownTemplate;
            },
        };
    });
})(window.angular);
