const moduleName = 'uiHelper';

angular.module(moduleName, []).directive('hndlrEnter', function () {
    return function (scope, element, attrs) {
        element.bind('keydown keypress', function (event) {
            if (event.which === 13) {
                scope.$apply(function () {
                    scope.$eval(attrs.hndlrEnter);
                });
                event.preventDefault();
            }
        });
    };
});

export default moduleName;
