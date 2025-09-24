; (function (window, ng) {
    'use strict';

    angular.module('qazy', [])
        .service('qazyService', ['$document', '$window', function ($document, $window) {
            var service = this;

            service.whenWindowLoad = function (callback) {
                if ($document[0].readyState === 'complete') {
                    callback();
                } else {
                    $window.addEventListener('load', function () {
                        callback();
                    });
                }
            };

            service.triggerLoad = function (elements) {

                var list;

                if (elements instanceof NodeList) {
                    list = Array.prototype.slice.call(elements, 0);
                } else if (elements instanceof Array) {
                    list = elements;
                } else if (elements instanceof Node) {
                    list = [elements]
                }

                list.forEach(function (item) {
                    if (item.qazy != null) {
                        item.qazy[0].triggerLoad(item);
                    }
                });
            };
        }])
        .directive('qazy', ['$parse', 'domService', 'qazyService', function ($parse, domService, qazyService) {
            return {
                scope: true,
                link: function (scope, element, attrs) {

                    var callbackOnLoaded = attrs.qazyOnLoaded != null ? $parse(attrs.qazyOnLoaded) : null;
                    var onLoaded;

                    if (callbackOnLoaded) {
                        onLoaded = function (target) {
                            return callbackOnLoaded(scope);
                        };

                        element.data('ngQazy', { onLoaded: onLoaded });
                    }

                    qazyService.whenWindowLoad(function () {
                        element[0].classList.remove('js-qazy-loading');

                        if (element[0].classList.contains('js-qazy-loaded') === true && callbackOnLoaded) {
                            onLoaded();
                        } else  if (element[0].classList.contains('js-qazy-loaded') === false && domService.parent(element[0], '[data-inplace-rich]') === null) {
                            var qazy = new window.Qazy();
                            var el = qazy.searchImages([element[0]]);
                            element[0].qazy = qazy.observe(el);
                        }
                    });
                }
            };
        }])
        .directive('qazyContainer', ['qazyService', 'domService', function (qazyService, domService) {
            return {
                link: function (scope, element, attrs) {
                    qazyService.whenWindowLoad(function () {
                        if (domService.matches(element[0], ':not([data-inplace-rich])') === true) {
                            var qazy = new window.Qazy();
                            var els = qazy.searchImages(element[0].querySelectorAll('img:not(.js-qazy-loading):not(.js-qazy-loaded):not([data-inplace-rich])'));
                            element[0].qazy = qazy.observe(els);
                        }
                    });
                }
            };
        }])
        .directive('qazyBackground', ['qazyService', function (qazyService) {
            return {
                link: function (scope, element, attrs) {
                    qazyService.whenWindowLoad(function () {
                        var qazy = new window.Qazy();
                        element[0].qazy = qazy.observe(element[0]);
                    });
                }
            };
        }])
        .directive('qazyIframe', ['qazyService', function (qazyService) {
            return {
                link: function (scope, element, attrs) {
                    qazyService.whenWindowLoad(function () {
                        var qazy = new window.Qazy();
                        var el = qazy.searchImages([element[0]]);
                        element[0].qazy = qazy.observe(el);
                    });
                }
            };
        }]);

})(window, window.angular);

