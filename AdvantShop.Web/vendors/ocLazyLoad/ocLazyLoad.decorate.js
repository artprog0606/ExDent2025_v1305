/*
    декорирую метод, чтобы скрипты и стили вставл¤л в конец body иначе идЄт конфликт каскада стилей
    https://github.com/ocombe/ocLazyLoad/blob/master/src/ocLazyLoad.loaders.common.js
 */
(angular => {
    'use strict';
    angular.module('oc.lazyLoad').config(['$provide', function ($provide) {
        $provide.decorator('$ocLazyLoad', ['$delegate', '$q', '$window', '$interval', function ($delegate, $q, $window, $interval) {
            var anchor = $window.document.getElementById('_OcLazyLoadFilesAnchor_') || $window.document.getElementsByTagName('head')[0];

            /**
             * Load a js/css file
             * @param type
             * @param path
             * @param params
             * @returns promise
             */
            //decorate - default paste files in body
            $delegate.buildElement = function buildElement(type, path, params) {
                var deferred = $q.defer(),
                    el,
                    loaded,
                    filesCache = $delegate._getFilesCache(),
                    cacheBuster = function cacheBuster(url) {
                        var dc = new Date().getTime();
                        if (url.indexOf('?') >= 0) {
                            if (url.substring(0, url.length - 1) === '&') {
                                return `${url}_dc=${dc}`;
                            }
                            return `${url}&_dc=${dc}`;
                        } else {
                            return `${url}?_dc=${dc}`;
                        }
                    };

                // Store the promise early so the file load can be detected by other parallel lazy loads
                // (ie: multiple routes on one page) a 'true' value isn't sufficient
                // as it causes false positive load results.
                if (angular.isUndefined(filesCache.get(path))) {
                    filesCache.put(path, deferred.promise);
                }

                // Switch in case more content types are added later
                switch (type) {
                    case 'css':
                        el = $window.document.createElement('link');
                        el.type = 'text/css';
                        el.rel = 'stylesheet';
                        el.href = params.cache === false ? cacheBuster(path) : path;
                        //https://github.com/filamentgroup/loadCSS/blob/master/src/loadCSS.js
                        // temporarily set media to something inapplicable to ensure it'll fetch without blocking render
                        el.media = "only x";
                        break;
                    case 'js':
                        el = $window.document.createElement('script');
                        el.src = params.cache === false ? cacheBuster(path) : path;
                        break;
                    default:
                        filesCache.remove(path);
                        deferred.reject(new Error(`Requested type "${type}" is not known. Could not inject "${path}"`));
                        break;
                }
                el.onload = el['onreadystatechange'] = function (e) {
                    if ((el['readyState'] && !/^c|loade/.test(el['readyState'])) || loaded) return;
                    el.onload = el['onreadystatechange'] = null;
                    loaded = 1;
                    $delegate._broadcast('ocLazyLoad.fileLoaded', path);
                    deferred.resolve(el);

                    if (type === 'css') {
                        el.media = "all";
                    }
                };
                el.onerror = function () {
                    filesCache.remove(path);
                    deferred.reject(new Error(`Unable to load ${path}`));
                };
                el.async = params.serie ? 0 : 1;

                var insertBeforeElem = anchor.lastChild ? anchor.lastChild : anchor;
                if (params.insertBefore) {
                    var element = angular.element(angular.isDefined(window.jQuery) ? params.insertBefore : document.querySelector(params.insertBefore));
                    if (element && element.length > 0) {
                        insertBeforeElem = element[0];
                    }
                }
                insertBeforeElem.parentNode.insertBefore(el, insertBeforeElem);

                return deferred.promise;
            };

            return $delegate;
        }])

        $provide.decorator('ocLazyLoadDirective', ['$delegate', '$ocLazyLoad', '$compile', '$animate', '$parse', '$timeout', '$document', '$window', function ($delegate, $ocLazyLoad, $compile, $animate, $parse, $timeout, $document, $window) {

            $delegate[0].compile = function compile(element, attrs) {
                // we store the content and remove it before compilation
                //var content = element[0].innerHTML;
                //element.addClass('ng-non-bindable');

                var model = $parse(attrs.ocLazyLoad);
                var event = $parse(attrs.ocLazyLoadEventInit);

                return function ($scope, $element, $attr) {
                    var eventInit = event() || 'load';

                    if (eventInit === 'load' && $document[0].readyState !== 'complete') {
                        $window.addEventListener('load', function () {
                            init();
                        })
                    } else {
                        init();
                    }

                    function fire(moduleName) {
                        if (angular.isDefined(moduleName)) {

                            let fn = () => {
                                $ocLazyLoad.load(moduleName).then(function () {
                                    // Attach element contents to DOM and then compile them.
                                    // This prevents an issue where IE invalidates saved element objects (HTMLCollections)
                                    // of the compiled contents when attaching to the parent DOM.
                                    //$animate.enter(content, $element);
                                    //element.removeClass('ng-non-bindable');
                                    // get the new content & compile it
                                    $compile($element.contents())($scope);
                                    $element.removeClass('oc-lazy-load-cloak');
                                })
                            }

                            if ($window.requestIdleCallback) {
                                $window.requestIdleCallback(fn)
                            } else {
                                fn();
                            }
                        }
                    }

                    function init() {
                        var _moduleName = model($scope) || $attr.ocLazyLoad;
                        if (_moduleName) {
                            fire(_moduleName);
                        } else {
                            $scope.$watch(function () {
                                return model($scope) || $attr.ocLazyLoad; // it can be a module name (string), an object, an array, or a scope reference to any of this
                            }, function (moduleName) {
                                fire(moduleName);
                            }, true);
                        }
                    }
                };
            }


            return $delegate;
        }])
    }]);

})(angular);

