/* @ngInject */
function clientCodeDirective(clientCodeService, toaster, $timeout) {
    return {
        restrict: 'A',
        scope: true,
        link: function (scope, element, attrs, ctrl) {
            var startVal = new Function('return ' + attrs.startVal)();

            scope.clientCode = {};

            if (startVal != null) {
                angular.extend(scope.clientCode, clientCodeService.trustClientCode(startVal));
            }

            $timeout(function () {
                clientCodeService
                    .getClientCode()
                    .then(function (data) {
                        if (data) {
                            scope.clientCode = clientCodeService.trustClientCode(data);
                        }
                    })
                    .catch((result) => {
                        toaster.pop('error', '', Array.isArray(result) ? result.join('<br>') : result);
                    });
            }, 1000);
        },
    };
}

export { clientCodeDirective };
