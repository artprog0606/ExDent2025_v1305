import appDependency from './appDependency.js';
import { projectsNames } from '../node_scripts/shopVariables.js';
/* убираем BFCache в FF */
window.addEventListener('unload', function () {});

angular
    .module('app', appDependency.get())
    .config(
        /* @ngInject*/
        function (
            $provide,
            $anchorScrollProvider,
            $compileProvider,
            $cookiesProvider,
            $httpProvider,
            $localeProvider,
            $translateProvider,
            $locationProvider,
            sweetalertDefaultOptions,
            $uibTooltipProvider,
        ) {
            $uibTooltipProvider.options({ trigger: 'outsideClick' });
            $anchorScrollProvider.disableAutoScrolling();

            let date;
            date = new Date();
            const currentYear = date.getFullYear();
            date.setFullYear(currentYear + 1);

            // Turn off URL manipulation in AngularJS
            // this code breaking preventDefault for anchors with empty href
            // $provide.decorator('$browser', ['$delegate', function ($delegate) {
            //    $delegate.onUrlChange = function () { };
            //    $delegate.url = function () { return "" };
            //    return $delegate;
            // }]);

            // #region compile debug
            $compileProvider.debugInfoEnabled(false);
            // #endregion

            // #region set cookie expires
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

            // #endregion

            // #region ie10 bug validation

            $provide.decorator('$sniffer', [
                '$delegate',
                function ($sniffer) {
                    const msie = parseInt((/msie (\d+)/.exec(navigator.userAgent.toLowerCase()) || [])[1], 10);
                    const _hasEvent = $sniffer.hasEvent;
                    $sniffer.hasEvent = function (event) {
                        if (event === 'input' && msie === 10) {
                            return false;
                        }
                        _hasEvent.call(this, event);
                    };
                    return $sniffer;
                },
            ]);

            // #endregion

            // #region prepera ajax url in absolute path

            // $httpProvider.useApplyAsync(true);

            const tokens = document.getElementsByName('__RequestVerificationToken');
            if (tokens.length > 0) {
                $httpProvider.defaults.headers.post.__RequestVerificationToken = tokens[0].value;
            }
            $httpProvider.defaults.headers.common['X-Requested-With'] = 'XMLHttpRequest';

            $httpProvider.interceptors.push([
                'urlHelper',
                'isMobileService',
                function (urlHelper, isMobileService) {
                    return {
                        request: function (config) {
                            const urlOld = config.url;
                            let template;

                            config.url = urlHelper.normalizeBundleFilesUrl(
                                urlHelper.getPhysicalPath(projectsNames.store, isMobileService.getValue()),
                                config.url,
                            );

                            config.url = urlHelper.getAbsUrl(config.url);

                            if (window.v != null && urlOld.indexOf('../') == 0 && config.url.indexOf('.html') != -1) {
                                config.url += '?v=' + (config.url.indexOf('localhost') != -1 ? Math.random() : window.v);
                            }

                            // for templates
                            if (urlOld != config.url && angular.isObject(config.cache) && config.cache.get(urlOld) != null) {
                                template = config.cache.get(urlOld);
                                config.cache.remove(urlOld);
                                config.cache.put(config.url, template);
                            }

                            // config.headers['Pragma'] = 'no-cache';
                            // config.headers['Expires'] = '-1';
                            // config.headers['Cache-Control'] = 'no-cache, no-store';

                            return config;
                        },
                    };
                },
            ]);

            // #endregion

            /* Прописано для # в URL вместо /#/ */
            $locationProvider.html5Mode({
                enabled: true,
                requireBase: true,
                rewriteLinks: false,
            });
            $locationProvider.hashPrefix('#');

            // #region localization

            const localeId = $localeProvider.$get().id;
            $translateProvider
                .translations(localeId, window.AdvantshopResource)
                .preferredLanguage(localeId)
                .useSanitizeValueStrategy('sanitizeParameters');
            // #endregion

            sweetalertDefaultOptions.customClass = sweetalertDefaultOptions.customClass || {};
            sweetalertDefaultOptions.customClass.confirmButton = 'btn btn-small btn-confirm';
            sweetalertDefaultOptions.customClass.cancelButton = 'btn btn-small btn-action';
        },
    )
    .run(
        /* @ngInject */
        function ($cookies, $timeout, toaster, modalService, isMobileService, choiceDefaultConfig) {
            document.documentElement.style.setProperty('--scrollbar-width', window.innerWidth - document.documentElement.clientWidth + 'px');

            if ($cookies.get('zonePopoverVisible') == null || $cookies.get('zonePopoverVisible').length === 0) {
                modalService.stopWorking();
            }

            const toasterContainer = document.querySelector('[data-toaster-container]');
            let toasterItems;

            $timeout(function () {
                if (toasterContainer != null) {
                    toasterItems = document.querySelectorAll('[data-toaster-type]');
                    if (toasterItems != null) {
                        for (let i = 0, len = toasterItems.length; i < len; i++) {
                            toaster.pop({
                                type: toasterItems[i].getAttribute('data-toaster-type'),
                                body: toasterItems[i].innerHTML,
                                bodyOutputType: 'trustedHtml',
                            });
                        }
                    }
                }
            });

            if (isMobileService.getValue() === true) {
                choiceDefaultConfig.classNames.containerOuter.push('choices-container--mobile');
            }
        },
    )
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
