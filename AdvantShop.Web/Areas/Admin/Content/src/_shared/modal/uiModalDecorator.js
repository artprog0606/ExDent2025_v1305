(function (ng) {
    'use strict';
    ng.module('uiModal').config([
        '$provide',
        function ($provide) {
            $provide.decorator('$uibModal', [
                '$delegate',
                'isMobileService',
                '$rootScope',
                function ($delegate, isMobileService, $rootScope) {
                    var originalWarn = $delegate.open;
                    $delegate.open = function (options, useCustomOptions = false) {
                        //useCustomOptions нужен чтоб при нажатии на backdrop не закрывалась модалка TODO
                        if (isMobileService.getValue() && !useCustomOptions) {
                            options.backdrop = true;
                        }
                        const modalInstance = originalWarn.apply($delegate, arguments);
                        modalInstance.result
                            .then((result) => {
                                $rootScope.$broadcast('modal.decorate.closing', { isClose: true });
                                return result;
                            })
                            .catch((err) => {
                                $rootScope.$broadcast('modal.decorate.closing', { isDismiss: true });
                                return err;
                            });

                        return modalInstance;
                    };
                    return $delegate;
                },
            ]);

            $provide.decorator(
                'uibModalTranscludeDirective',
                /* @ngInject */
                function ($delegate, $animate, $document, isMobileService) {
                    var directive = $delegate[0];
                    var originalLink = directive.link;

                    delete directive.link;

                    directive.compile = function () {
                        return function (scope, element) {
                            if (isMobileService.getValue() === false) {
                                element[0].addEventListener('mousedown', function (event) {
                                    scope.$parent.isMouseDownContent = true;
                                });
                            }

                            originalLink.apply(this, arguments);
                        };
                    };

                    return $delegate;
                },
            );

            $provide.decorator(
                'uibModalWindowDirective',
                /* @ngInject */
                function ($delegate, $q, $animateCss, $document, isMobileService) {
                    var directive = $delegate[0];
                    var originalLink = directive.link;

                    directive.compile = function (cElement, cAttrs) {
                        return function (scope, element) {
                            document.body.appendChild(element[0]);

                            originalLink.apply(this, arguments);

                            var originalClose = scope.close;

                            element.off('click', scope.close);

                            scope.close = function (event) {
                                if (isMobileService.getValue() && event != null && event.target.closest('.modal-dialog') != null) {
                                    event.stopPropagation();
                                    if (
                                        event.target.classList.contains('dropdown-menu') === false &&
                                        event.target.classList.contains('dropdown-toggle') === false
                                    ) {
                                        element.find('.dropdown-toggle').dropdown('hide');
                                    }
                                }
                                if (scope.$parent.isMouseDownContent !== true) {
                                    originalClose.apply(scope, arguments);
                                }
                            };

                            element.on('click', scope.close);

                            $document[0].addEventListener('click', function (event) {
                                scope.$parent.isMouseDownContent = null;
                            });
                        };
                    };

                    delete directive.link;

                    return $delegate;
                },
            );

            $provide.decorator('uibModalBackdropDirective', [
                '$delegate',
                function ($delegate) {
                    var directive = $delegate[0];
                    var originalCompile = directive.compile;

                    directive.compile = function () {
                        var linkFn = originalCompile.apply(this, arguments);
                        return function (scope, element) {
                            linkFn.apply(this, arguments);
                            setTimeout(() => document.body.appendChild(element[0]));
                        };
                    };

                    return $delegate;
                },
            ]);
        },
    ]);
})(window.angular);
