(function (ng) {
    'use strict';

    ng.module('ngClickCapture', []).directive('ngClickCapture', [
        '$parse',
        function ($parse) {
            return {
                scope: true,
                link: function (scope, element, attrs) {
                    const eventOption = { capture: true };
                    const callback = $parse(attrs['ngClickCapture']);
                    const handleClick = (event) => {
                        callback(scope, { $event: event, element });
                    };
                    if (callback != null) {
                        element[0].addEventListener('click', handleClick, eventOption);
                    }

                    element.on('$destroy', function () {
                        element[0].removeEventListener('click', handleClick, eventOption);
                    });
                },
            };
        },
    ]);
})(window.angular);
