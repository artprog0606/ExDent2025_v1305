import themeTriggerTemplate from './templates/theme-trigger.html';

const DarkTheme = 'dark-theme';
const LightTheme = 'light-theme';
const CookieThemeKey = 'userTheme';
const CookieThemeImgKey = 'cookieThemeImgKey';

(function (ng) {
    'use strict';

    angular
        .module('themeTrigger', ['ngCookies'])
        .controller('themeTriggerCtrl', [
            '$cookies',
            '$http',
            function ($cookies, $http) {
                var ctrl = this;

                ctrl.$onInit = function () {
                    ctrl.logos = document.querySelectorAll('.site-head-logo-picture');

                    ctrl.userTheme = $cookies.get(CookieThemeKey);
                    ctrl.imageThemeSrc = $cookies.get(CookieThemeImgKey);

                    ctrl.logos.forEach((logo) => {
                        logo.src = ctrl.imageThemeSrc != null ? ctrl.imageThemeSrc : logo.src;
                    });

                    ctrl.getAllLogo();

                    if (!ctrl.userTheme) {
                        ctrl.selected = false;
                        $cookies.put(CookieThemeKey, LightTheme);
                        ctrl.userTheme = LightTheme;
                    } else if (ctrl.userTheme === LightTheme) {
                        ctrl.selected = false;
                    } else if (ctrl.userTheme === DarkTheme) {
                        ctrl.selected = true;
                    }
                };

                ctrl.toogleTheme = function () {
                    ctrl.userTheme = ctrl.userTheme === LightTheme ? DarkTheme : LightTheme;
                    $cookies.put(CookieThemeKey, ctrl.userTheme);
                    document.querySelector('html').classList.toggle(DarkTheme);

                    if (ctrl.useDarkLogo) {
                        ctrl.logos.forEach((logo) => {
                            logo.src = ctrl.userTheme === LightTheme ? ctrl.lightSrc : ctrl.darkSrc;
                            $cookies.put(CookieThemeImgKey, logo.src);
                        });
                    } else {
                        ctrl.logos.forEach((logo) => {
                            logo.src = ctrl.lightSrc;
                            $cookies.put(CookieThemeImgKey, logo.src);
                        });
                    }
                };

                ctrl.getAllLogo = function () {
                    return $http.get('common/getAllLogo').then((result) => {
                        let data = result.data;
                        ctrl.lightSrc = data.light.ImgSource;
                        ctrl.darkSrc = data.dark.ImgSource;
                        ctrl.useDarkLogo = data.useAnotherForDarkTheme;
                    });
                };
            },
        ])
        .directive('themeTrigger', [
            '$cookies',
            function ($cookies) {
                return {
                    restrict: 'E',
                    scope: true,
                    controller: 'themeTriggerCtrl',
                    controllerAs: 'themeTrigger',
                    bindToController: true,
                    templateUrl: themeTriggerTemplate,
                };
            },
        ]);
})(window.angular);
