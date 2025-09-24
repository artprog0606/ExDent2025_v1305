(function (ng) {
    'use strict';

    var CountdownCtrl = function ($scope, $window, $document) {
        var ctrl = this,
            endTime,
            timer;

        ctrl.$postLink = function () {
            if ($document[0].readyState === 'complete') {
                ctrl.init(ctrl.endTime, ctrl.endTimeUtc);
            } else {
                $window.addEventListener('load', function () {
                    ctrl.init(ctrl.endTime, ctrl.endTimeUtc);
                });
            }
        };

        ctrl.dataTime = {};

        ctrl.getTimeleft = function () {
            var dimTime = endTime.getTime() - new Date().getTime();

            return dimTime > 0 ? dimTime : null;
        };

        ctrl.calc = function (timeLeft) {
            var years = 0,
                months = 0,
                days = 0,
                hours = 0,
                minutes = 0,
                seconds = 0;

            if (timeLeft != null) {
                years = Math.floor(timeLeft / 3600000 / 24 / 365);
                months = Math.floor((timeLeft / 3600000 / 24 / 30) % 12);
                days = Math.floor(timeLeft / 3600000 / 24);
                hours = Math.floor((timeLeft / 3600000) % 24);
                minutes = Math.floor((timeLeft / 60000) % 60);
                seconds = Math.floor((timeLeft / 1000) % 60);

                if (ctrl.isShowDays === false) {
                    hours = Math.floor((timeLeft + timeLeft / 3600000 / 24) / 3600000);
                    ctrl.isShowDays = false;
                }
            }

            ctrl.update(years, months, days, hours, minutes, seconds);

            return ctrl.dataTime;
        };

        ctrl.tick = function () {
            var timeLeft = ctrl.getTimeleft();

            ctrl.calc(timeLeft);

            if (timeLeft != null) {
                timer = setTimeout(function () {
                    ctrl.tick();
                    $scope.$digest();
                }, 1000);
            } else if (ctrl.onFinish != null) {
                ctrl.onFinish({});
                if (timer != null) {
                    $scope.$apply();
                }
            }
        };

        ctrl.update = function (years, months, days, hours, minutes, seconds) {
            ctrl.dataTime.years = years;
            ctrl.dataTime.months = months;
            ctrl.dataTime.days = days;
            ctrl.dataTime.hours = hours;
            ctrl.dataTime.minutes = minutes;
            ctrl.dataTime.seconds = seconds;
        };

        ctrl.addMinutes = function (minutes) {
            return new Date(Date.now() + minutes * 60000);
        };

        ctrl.init = function (countdownTime, endTimeUtc) {
            if (ctrl.isLoop != null) {
                endTime = ctrl.addMinutes(ctrl.isLoop);
            } else {
                if (endTimeUtc != null) {
                    endTime = new Date(endTimeUtc);
                } else {
                    endTime = flatpickr.parseDate(countdownTime, 'd.m.Y H:i');
                }
            }

            ctrl.tick();
        };
    };

    ng.module('countdown').controller('CountdownCtrl', CountdownCtrl);

    CountdownCtrl.$inject = ['$scope', '$window', '$document'];
})(window.angular);
