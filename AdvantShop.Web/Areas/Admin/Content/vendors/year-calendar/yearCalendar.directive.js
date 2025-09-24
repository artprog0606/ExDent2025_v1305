import './yearCalendar.resources.js';

; (function (ng) {
	'use strict';
    ng.module('ngYearCalendar', [])
        .directive('yearCalendar', ['$ocLazyLoad',
            function ($ocLazyLoad) {
                return {
                    scope: {
                        calendarOptions: '<',
                        calendarOnInit: '&'
                    },
                    link: function (scope, element) {
                            var option = ng.extend({ language: document.documentElement.lang }, scope.calendarOptions || {});
                            var calendarObj = $(element).calendar(option);

                            if (scope.calendarOnInit != null) {
                                scope.calendarOnInit({ calendar: calendarObj });
                            }
                    }
                }
            }
        ]);

})(window.angular);
