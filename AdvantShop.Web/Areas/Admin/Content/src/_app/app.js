import appDependency from '../../../../../scripts/appDependency.js';
import { projectsNames } from '../../../../../node_scripts/shopVariables.js';
import {urlHelper} from "../../../../../scripts/_common/urlHelper/urlHelper.js";

(function (ng) {

    const replacer = {
        q: 'й',
        w: 'ц',
        e: 'у',
        r: 'к',
        t: 'е',
        y: 'н',
        u: 'г',
        i: 'ш',
        o: 'щ',
        p: 'з',
        '[': 'х',
        ']': 'ъ',
        a: 'ф',
        s: 'ы',
        d: 'в',
        f: 'а',
        g: 'п',
        h: 'р',
        j: 'о',
        k: 'л',
        l: 'д',
        ';': 'ж',
        "'": 'э',
        z: 'я',
        x: 'ч',
        c: 'с',
        v: 'м',
        b: 'и',
        n: 'т',
        m: 'ь',
        ',': 'б',
        '.': 'ю',
        '/': '.',
    };
    /*убираем BFCache в FF*/
    window.addEventListener('unload', () => {});

    ng.module('app', appDependency.get())
        .value('duScrollBottomSpy', true)
        .config(
            /* @ngInject */
            (
                $provide,
                $compileProvider,
                $cookiesProvider,
                $localeProvider,
                $httpProvider,
                $translateProvider,
                urlHelperConfig,
                toasterConfig,
                ngFlatpickrDefaultOptions,
                $locationProvider,
                sweetalertDefaultOptions,
                addressListConfig,
            ) => {
                const date = new Date(),
                    currentYear = date.getFullYear();

                //#region Disable comment and css class directives
                $compileProvider.commentDirectivesEnabled(false);
                //$compileProvider.cssClassDirectivesEnabled(false); не использовать так как падает chart.js
                //#endregion

                date.setFullYear(currentYear + 1);

                //#region compile debug
                $compileProvider.debugInfoEnabled(false);
                //#endregion

                // enable if need to use unsave protocols
                //$compileProvider.aHrefSanitizationWhitelist(/^\s*(http|https|ftp|mailto|callto|tel):/);

                //#region set cookie expires
                $cookiesProvider.defaults.expires = date;
                $cookiesProvider.defaults.path = (new urlHelper()).getSubfolder();

                if (window.location.protocol === 'https:') {
                    $cookiesProvider.defaults.secure = true;
                    $cookiesProvider.defaults.samesite = 'none';
                }

                if (
                    window.location.hostname !== 'localhost' &&
                    window.location.hostname !== 'server' &&
                    !/^(?!0)(?!.*\.$)((1?\d?\d|25[0-5]|2[0-4]\d)(\.|$)){4}$/.test(window.location.hostname)
                ) {
                    $cookiesProvider.defaults.domain = `.${  window.location.hostname.replace('www.', '')}`;
                }

                //#endregion

                //закоментировал, т.к. в FF не работает валидация input[type="number"]
                //#region ie10 bug validation

                //$provide.decorator('$sniffer', ['$delegate', function ($sniffer) {
                //    var msie = parseInt((/msie (\d+)/.exec(angular.lowercase(navigator.userAgent)) || [])[1], 10);
                //    var _hasEvent = $sniffer.hasEvent;
                //    $sniffer.hasEvent = function (event) {
                //        if (event === 'input' && msie === 10) {
                //            return false;
                //        }
                //        _hasEvent.call(this, event);
                //    }
                //    return $sniffer;
                //}]);

                //#endregion

                //#region prepera ajax url in absolute path
                $httpProvider.useApplyAsync(true);

                const tokens = document.getElementsByName('__RequestVerificationToken');
                if (tokens.length > 0) {
                    $httpProvider.defaults.headers.post.__RequestVerificationToken = tokens[0].value;
                }
                $httpProvider.defaults.headers.common['X-Requested-With'] = 'XMLHttpRequest';

                $httpProvider.interceptors.push([
                    '$q',
                    'urlHelper',
                    'isMobileService',
                    function ($q, urlHelper, isMobileService) {
                        return {
                            request (config) {
                                let urlOld = config.url,
                                    template;

                                config.url = urlHelper.normalizeBundleFilesUrl(
                                    urlHelper.getPhysicalPath(projectsNames.admin, isMobileService.getValue()),
                                    config.url,
                                );
                                config.url = urlHelper.getAbsUrl(config.url);

                                //for templates
                                if (urlOld != config.url && ng.isObject(config.cache) && config.cache.get(urlOld) != null) {
                                    template = config.cache.get(urlOld);
                                    config.cache.remove(urlOld);
                                    config.cache.put(config.url, template);
                                    //config.cache.put(config.url.replace('adminv2', 'adminv3'), template);
                                }

                                if (
                                    window.location.href.indexOf('adminv3') != -1 &&
                                    config.url.indexOf('adminv2') != -1 &&
                                    config.url.indexOf('.html') === -1 &&
                                    template == null
                                ) {
                                    config.url = config.url.replace('adminv2', 'adminv3');
                                }

                                if (config.url.includes('/modules/') && !config.url.includes('/dist/') && config.url.includes('.html')) {
                                    config.url += `${config.url.includes('?') ? '&' : '?'  }v=${  Math.random()}`;
                                }

                                config.executingAjax = {
                                    url: config.url,
                                    params: config.params,
                                };

                                return config;
                            },
                        };
                    },
                ]);

                //#endregion

                //#region localization

                const localeId = $localeProvider.$get().id;
                $translateProvider
                    .translations(localeId, ng.extend(window.AdvantshopResource || {}, window.AdvantshopAdminResource || {}))
                    .preferredLanguage(localeId)
                    .useSanitizeValueStrategy('sanitizeParameters');
                //#endregion

                urlHelperConfig.isAdmin = true;

                toasterConfig['icon-classes'].call = 'toast-call';
                toasterConfig['position-class'] = 'toast-bottom-right';
                toasterConfig.limit = 3;

                ngFlatpickrDefaultOptions.locale = localeId.split('-')[0];
                ngFlatpickrDefaultOptions.disableMobile = true;

                $locationProvider.html5Mode({ enabled: true, requireBase: true, rewriteLinks: false });

                sweetalertDefaultOptions.customClass = sweetalertDefaultOptions.customClass || {};
                sweetalertDefaultOptions.customClass.confirmButton = 'btn btn-sm btn-success';
                sweetalertDefaultOptions.customClass.cancelButton = 'btn btn-sm btn-action';

                addressListConfig.autocompleteAlt = true;
                addressListConfig.themeAlt = true;
                addressListConfig.overrideFields = {
                    visible: ['IsShowCity', 'IsShowDistrict', 'IsShowState', 'IsShowCountry', 'IsShowZip', 'IsShowAddress', 'IsShowFullAddress'],
                };
                addressListConfig.requiredValidationEnabled = false;
            },
        )
        .run(
            /* @ngInject */ ($document, $window, $q, $timeout, toaster, adminWebNotificationsService, $templateCache) => {
                //replace ng-bind title on ng-bind-html
                const popeverHtml = $templateCache.get('uib/template/popover/popover-html.html');
                $templateCache.put('uib/template/popover/popover-html.html', popeverHtml.replace('ng-bind="uibTitle"', 'ng-bind-html="uibTitle"'));

                $window.addEventListener(`load`, () => {
                    let toasterContainer = document.querySelector('[data-toaster-container]'),
                        toasterItems,
                        linkWithAnchors = document.querySelectorAll('a[href*="#"]:not([target])');

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

                    //old style using anchor
                    if (linkWithAnchors.length > 0) {
                        for (let j = 0, lenJ = linkWithAnchors.length; j < lenJ; j += 1) {
                            linkWithAnchors[j].setAttribute('target', '_self');
                        }
                    }
                });

                adminWebNotificationsService.onPageLoad();
            },
        )
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
        ])
        .filter('nl2br', [
            '$sanitize',
            function ($sanitize) {
                const tag = '<br />';
                return function (msg) {
                    if (!msg) return '';
                    msg = (`${msg  }`).replace(/(\r\n|\n\r|\r|\n|&#10;&#13;|&#13;&#10;|&#10;|&#13;)/g, `${tag  }$1`);
                    return $sanitize(msg);
                };
            },
        ])
        .filter('htmlDecode', [
            '$sce',
            function ($sce) {
                const elem = document.createElement('div');
                return function (value) {
                    elem.innerHTML = value;
                    return $sce.trustAsHtml(elem.textContent);
                };
            },
        ])
        .filter('decodeString', () => function (value) {
                const txt = new DOMParser().parseFromString(value, 'text/html');
                return txt.documentElement.textContent;
            })
        .filter('greedysearch', [
            '$filter',
            function ($filter) {
                return function (array, expression) {
                    if (array == null || array.length === 0 || expression == null || expression === '') {
                        return array;
                    }

                    let altTextExpression,
                        keys,
                        arrayOriginalLang,
                        arrayAltLang,
                        result,
                        arrayUniqueKey = [];

                    if (ng.isString(expression) === true) {
                        altTextExpression = engToRus(expression);
                    } else {
                        keys = Object.keys(expression);

                        altTextExpression = {};

                        for (let i = 0, len = keys.length; i < len; i++) {
                            if (keys[i] !== '$$hashKey') {
                                altTextExpression[keys[i]] = engToRus(expression[keys[i]]);
                            }
                        }
                    }

                    arrayOriginalLang = $filter('filter')(array, expression) || [];
                    arrayAltLang = $filter('filter')(array, altTextExpression) || [];

                    result = arrayOriginalLang.concat(arrayAltLang);

                    const d = result.filter((item) => {
                        let result;

                        if (arrayUniqueKey.indexOf(JSON.stringify(item)) === -1) {
                            arrayUniqueKey.push(JSON.stringify(item));
                            result = true;
                        } else {
                            result = false;
                        }

                        return result;
                    });

                    return d;
                };
            },
        ])
        .filter('numbergreedy', [
            '$locale',
            function ($locale) {
                return function (value) {
                    let result;

                    if (value != null && ((ng.isString(value) === true && value.length > 0) || ng.isNumber(value) === true)) {
                        result = value.toString().replace(/\s*/g, '').replace(/[,.]+/g, $locale.NUMBER_FORMATS.DECIMAL_SEP);
                    } else {
                        result = value;
                    }

                    return result;
                };
            },
        ])
        .service('appService', [
            '$timeout',
            function ($timeout) {
                let _ctrl;
                let titleTimerUpdate;
                let titleCurrent;
                let titleDefault;
                /**
                 * Возвращает заголовок с задержкой
                 * @return {string}
                 */
                this.getTitle = function () {
                    if (titleTimerUpdate) {
                        $timeout.cancel(titleTimerUpdate);
                    }
                    titleTimerUpdate = $timeout(() => (titleCurrent = _ctrl.title), 100);

                    return titleCurrent;
                };
                /**
                 * @param title - Значение заголовка
                 * @param isDefault - Выводить переданный заголовок по-умолчанию
                 */
                this.setTitle = function (title, isDefault) {
                    _ctrl.title = title != null ? title : titleDefault;
                    if (isDefault === true) {
                        titleDefault = title;
                    }
                };

                this.memoryCtrl = function (ctrl) {
                    _ctrl = ctrl;
                };
            },
        ])
        .controller(
            'AppCtrl',
            /* @ngInject */
            function (appService, $timeout, $scope, $location) {
                this.showInput = function () {
                    $timeout(() => {
                        document.querySelector('.header-setting__search-form .search-input').focus();
                    }, 0);
                };

                this.$onInit = function () {
                    appService.memoryCtrl(this);
                };

                this.getTitle = function () {
                    return appService.getTitle();
                };

                this.setTitle = function (title, isDefault) {
                    return appService.setTitle(title, isDefault);
                };

                this.back = function () {
                    if (Object.keys($location.$$search)[0]) {
                        let path = $location.$$absUrl;
                        path = path.split('?tabs')[0];
                        history.pushState(null, null, path);
                    }
                };

                this.getInstCtrl = function (instagramCtrl) {
                    this.instagramCtrl = instagramCtrl;
                };

                this.setIntersectionValue = function (entry) {
                    this.intersectionValue = entry;
                    $scope.$digest();
                };
            },
        );

    function engToRus(text) {
        return text.replace(/[A-z/,.;\'\]\[]/g, (x) => x == x.toLowerCase() ? replacer[x] : replacer[x.toLowerCase()].toUpperCase());
    }
})(window.angular);
