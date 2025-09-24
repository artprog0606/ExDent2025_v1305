(function (ng) {
    'use strict';

    ng.module('scrollToBlock').directive('scrollToBlock', [
        '$parse',
        'scrollToBlockService',
        function ($parse, scrollToBlockService) {
            return {
                restrict: 'AE',
                controller: function () {},
                controllerAs: 'scrollToBlock',
                scope: true,
                link: function (scope, element, attrs, ctrl) {
                    var el = element[0],
                        nextEl,
                        callback = attrs.scrollToBlockCallback != null ? $parse(attrs.scrollToBlockCallback) : null;

                    el.addEventListener('click', function (event) {
                        var selector = attrs.scrollToBlock;
                        var block = document.querySelector(selector);
                        nextEl = attrs.selector != null ? scrollToBlockService.nextUntil(block, attrs.selector) : block;
                        if (nextEl != null) {
                            event.preventDefault();
                            scrollToBlockService.scrollToBlock(nextEl, true);
                            if (callback != null) {
                                callback(scope);
                            }
                        }
                    });
                },
            };
        },
    ]);
})(window.angular);
