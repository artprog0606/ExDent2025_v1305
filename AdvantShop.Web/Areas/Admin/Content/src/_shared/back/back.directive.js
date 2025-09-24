var backTrigger = [
    '$parse',
    'backService',
    'isMobileService',
    'appService',
    function ($parse, backService, isMobileService, appService) {
        return {
            scope: true,
            link: (scope, element, attrs) => {
                var callback = attrs.backTrigger != null ? $parse(attrs.backTrigger) : null;

                element.on('click', (event) => {
                    var preventProcess = null;

                    if (callback != null) {
                        preventProcess = callback(scope, { $event: event }) === false;
                    }

                    if (preventProcess !== true) {
                        var historyItem = backService.popHistoryItem();
                        if (historyItem != null) {
                            event.preventDefault();
                            scope.$apply();
                        }
                    } else if (preventProcess === true) {
                        event.preventDefault();
                        scope.$apply();
                    }

                    if (backService.getLast() == null) {
                        appService.setTitle(null);
                    }

                    if (isMobileService.getValue()) {
                        window.scrollTo({ top: 0 });
                    }
                });
            },
        };
    },
];

angular.module('back').directive('backTrigger', backTrigger);
