; (function (ng) {
    'use strict';

    angular.module('ui.bootstrap.typeahead').config(['$provide', function ($provide) {

        $provide.decorator('uibTypeaheadPopupDirective', ['$delegate', function ($delegate) {

            var directive = $delegate[0];

            var originalLink = directive.link;

            directive.scope.needOpen = '<?';

            directive.scope.loading = '<?';

            directive.scope.noResult = '<?';

            directive.compile = function (cElement, cAttrs) {

                cAttrs.$set('needOpen', 'needShowPopup');

                cAttrs.$set('loading', 'isLoading');

                cAttrs.$set('noResult', 'isNoResult');

                return originalLink;
            }


            return $delegate;
        }]);


        $provide.decorator('uibTypeaheadDirective', ['$delegate', '$parse', function ($delegate, $parse) {

            var directive = $delegate[0];

            var originalLink = directive.link;


            var link = function (scope, element, attrs, ctrls) {
                var minLength = scope.$eval(attrs.typeaheadMinLength);
                var isFocus = false;
                var isLoadingSetter = $parse(attrs.typeaheadLoading) || angular.noop;
                var isNoResultSetter = $parse(attrs.typeaheadNoResults) || angular.noop;

                element.on('focus', function () {
                    isFocus = true;
                });

                element.on('blur', function () {
                    isFocus = false;
                });

                scope.needShowPopup = function () {
                    return element.val().length >= minLength && isFocus;
                };

                scope.isLoading = function () {
                    return isLoadingSetter(scope);
                }

                scope.isNoResult = function () {
                    return isNoResultSetter(scope);
                }

                return originalLink.apply($delegate, arguments);
            };

            directive.compile = function (cElement, cAttrs) {
                return function (scope, element, attrs, tabsCtrl) {
                    link.apply(this, arguments);
                };
            }

            return $delegate;
        }]);
    }]);
})(window.angular);