import appDependency from '../../../scripts/appDependency.js';

/*убираем BFCache в FF*/
window.addEventListener('unload', function () {});

angular
    .module('app', appDependency.get())
    .config(
        /* @ngInject */ function (
            $provide,
            $compileProvider,
            $cookiesProvider,
            $httpProvider,
            $localeProvider,
            $translateProvider,
            $locationProvider,
            ngFlatpickrDefaultOptions,
        ) {
            var date = new Date(),
                currentYear = date.getFullYear();

            date.setFullYear(currentYear + 1);

            //#region compile debug
            $compileProvider.debugInfoEnabled(false);
            //#endregion

            //#region set cookie expires
            $cookiesProvider.defaults.expires = date;
            $cookiesProvider.defaults.path = '/';

            if (window.location.protocol === 'https:') {
                $cookiesProvider.defaults.secure = true;
                $cookiesProvider.defaults.samesite = 'none';
            }

            if (
                window.location.hostname !== 'localhost' &&
                window.location.hostname !== 'server' &&
                !/^(?!0)(?!.*\.$)((1?\d?\d|25[0-5]|2[0-4]\d)(\.|$)){4}$/.test(window.location.hostname)
            ) {
                $cookiesProvider.defaults.domain = '.' + window.location.hostname.replace('www.', '');
            }

            //#endregion

            //#region ie10 bug validation

            $provide.decorator('$sniffer', [
                '$delegate',
                function ($sniffer) {
                    var msie = parseInt((/msie (\d+)/.exec(angular.lowercase(navigator.userAgent)) || [])[1], 10);
                    var _hasEvent = $sniffer.hasEvent;
                    $sniffer.hasEvent = function (event) {
                        if (event === 'input' && msie === 10) {
                            return false;
                        }
                        _hasEvent.call(this, event);
                    };
                    return $sniffer;
                },
            ]);

            //#endregion

            //#region prepera ajax url in absolute path
            var basePath = document.getElementsByTagName('base')[0].getAttribute('href'),
                regex = new RegExp('^(?:[a-z]+:)?//', 'i');

            $httpProvider.useApplyAsync(true);

            var tokens = document.getElementsByName('__RequestVerificationToken');
            if (tokens.length > 0) {
                $httpProvider.defaults.headers.post['__RequestVerificationToken'] = tokens[0].value;
            }
            $httpProvider.defaults.headers.common['X-Requested-With'] = 'XMLHttpRequest';

            $httpProvider.interceptors.push(function () {
                return {
                    request: function (config) {
                        var urlOld = config.url,
                            template;

                        if (regex.test(config.url) === false) {
                            if (config.url.charAt(0) === '/') {
                                config.url = config.url.substring(1);
                            }

                            config.url = basePath + config.url;
                        }

                        //for templates
                        if (urlOld != config.url && angular.isObject(config.cache) && config.cache.get(urlOld) != null) {
                            template = config.cache.get(urlOld);
                            config.cache.remove(urlOld);
                            config.cache.put(config.url, template);
                        }

                        //config.headers['Pragma'] = 'no-cache';
                        //config.headers['Expires'] = '-1';
                        //config.headers['Cache-Control'] = 'no-cache, no-store';

                        return config;
                    },
                };
            });

            //#endregion

            /* Прописано для # в URL вместо /#/ */
            $locationProvider.html5Mode({
                enabled: true,
                requireBase: true,
                rewriteLinks: false,
            });
            $locationProvider.hashPrefix('#');

            //#region localization

            var localeId = $localeProvider.$get().id;
            $translateProvider
                .translations(localeId, window.AdvantshopResource)
                .preferredLanguage(localeId)
                .useSanitizeValueStrategy('sanitizeParameters');
            //#endregion

            ngFlatpickrDefaultOptions.locale = localeId.split('-')[0];
            ngFlatpickrDefaultOptions.disableMobile = true;
        },
    )
    .run([
        '$cookies',
        '$timeout',
        'toaster',
        'modalService',
        function ($cookies, $timeout, toaster, modalService) {
            if ($cookies.get('zonePopoverVisible') == null || $cookies.get('zonePopoverVisible').length === 0) {
                modalService.stopWorking();
            }

            var toasterContainer = document.querySelector('[data-toaster-container]'),
                toasterItems;

            $timeout(function () {
                if (toasterContainer != null) {
                    toasterItems = document.querySelectorAll('[data-toaster-type]');
                    if (toasterItems != null) {
                        for (var i = 0, len = toasterItems.length; i < len; i++) {
                            toaster.pop({
                                type: toasterItems[i].getAttribute('data-toaster-type'),
                                body: toasterItems[i].innerHTML,
                                bodyOutputType: 'trustedHtml',
                            });
                        }
                    }
                }
            });
        },
    ])
    .controller('AppCtrl', function () {})
    .filter('sanitize', [
        '$sce',
        function ($sce) {
            return function (htmlCode) {
                return $sce.trustAsHtml(htmlCode);
            };
        },
    ])
    .filter('sanitizeUrl', [
        '$sce',
        function ($sce) {
            return function (htmlCode) {
                return $sce.trustAsResourceUrl(htmlCode);
            };
        },
    ]);
