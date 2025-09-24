import dialogTemplate from '../templates/dialog.html';
(function (ng) {
    'use strict';

    angular.module('zone').directive('zoneDialogTrigger', [
        'zoneService',
        function (zoneService) {
            return {
                restrict: 'A',
                scope: {
                    showImmediately: '<?',
                },
                link: function (scope, element, attrs, ctrl) {
                    element.on('click', function (e) {
                        e.stopPropagation();
                        scope.$apply(zoneService.zoneDialogOpen);
                    });
                    if (scope.showImmediately) {
                        zoneService.zoneDialogOpen({ showImmediately: scope.showImmediately });
                    }
                },
            };
        },
    ]);

    angular.module('zone').directive('zoneDialog', function () {
        return {
            restrict: 'A',
            scope: {
                hideCountries: '<?',
                hideSearch: '<?',
            },
            replace: true,
            templateUrl: dialogTemplate,
            controller: 'ZoneCtrl',
            controllerAs: 'zone',
            bindToController: true,
        };
    });

    angular.module('zone').directive('zoneCurrent', [
        'zoneService',
        '$parse',
        function (zoneService, $parse) {
            return {
                restrict: 'A',
                scope: true,
                link: function (scope, element, attrs, ctrl) {
                    var startVal = new Function('return ' + attrs.startVal)();
                    const startCity = $parse(attrs.startCity)(scope);
                    scope.zone = { City: startCity?.City };
                    zoneService.getCurrentZone().then(function (data) {
                        if (!data.City && startVal.City) {
                            data.City = startVal.City;
                        }

                        if (data) {
                            scope.zone = zoneService.trustZone(data);
                        } else {
                            scope.zone = {};

                            if (startVal != null) {
                                angular.extend(scope.zone, zoneService.trustZone(startVal));
                            }

                            zoneService.addUpdateList(scope);

                            zoneService.getCurrentZone().then(function (data) {
                                scope.zone = zoneService.trustZone(data);
                            });
                        }
                    });
                },
            };
        },
    ]);

    angular.module('zone').directive('zonePopover', function () {
        return {
            restrict: 'A',
            scope: true,
            controller: 'ZonePopoverCtrl',
            controllerAs: 'zonePopover',
        };
    });
    angular.module('zone').directive('zoneAddCallback', [
        'zoneService',
        '$parse',
        function (zoneService, $parse) {
            return {
                restrict: 'A',
                scope: true,
                controller: 'ZonePopoverCtrl',
                controllerAs: 'zonePopover',
                link: function (scope, element, attrs, ctrl) {
                    const objCallback = $parse(attrs.zoneAddCallback)(scope);
                    if (objCallback != null && objCallback.callback != null && objCallback.callbackName != null) {
                        zoneService.addCallback(objCallback.callbackName, objCallback.callback);
                    }
                },
            };
        },
    ]);
})(window.angular);
