angular.module('ui.bootstrap.popover').config(/* @ngInject */function ($provide) {
    let uid = 0;
    const prefix = 'popover';
    const backdropDonor = document.createElement('div');
    backdropDonor.classList.add('popover-backdrop');

    ['uibPopoverTemplateDirective', 'uibPopoverHtmlDirective', 'uibPopoverDirective'].forEach(directiveName => {
        $provide.decorator(directiveName, /* @ngInject */function ($delegate, $parse, isMobileService, $timeout) {
            const directive = $delegate[0];
            const originalCompile = directive.compile;

            directive.compile = function (tElement, tAttrs) {
                const originalLink = originalCompile(tElement, tAttrs);

                return function (scope, element, attrs, tooltipCtrl) {
                    if(isMobileService.getValue()){
                        attrs.$set('popoverTrigger', null);
                        attrs.$set('popoverAppendToBody', 'true');
                    }

                    const isOpenAttr = attrs[prefix + 'IsOpen'];

                    if (angular.isDefined(isOpenAttr) === false) {
                        attrs.$set('popoverIsOpen', "app.popoverIsOpenGenerated_" + uid);
                        uid = uid + 1;
                    }

  
                    let onClickBackButtonFunc = function (e) {
                        event.preventDefault();
                        const getter = $parse(attrs.popoverIsOpen);
                        getter.assign(scope, false);
                        scope.$apply();
                    }

                    scope.$watch(attrs.popoverIsOpen, (newValue, oldValue) => {
                        if (newValue === oldValue) {
                            return;
                        }

                        if (newValue === true) {
                            window.history.pushState({ 'popover': 'open' }, null, window.location.href);
                            window.addEventListener('popstate', onClickBackButtonFunc);
                            document.documentElement.classList.add('popover-opened');
                        } else {
                            window.history.replaceState({ 'popover': 'close' }, null, window.location.href);
                            window.removeEventListener('popstate', onClickBackButtonFunc);
                            document.documentElement.classList.remove('popover-opened');
                        }
                    })

                    originalLink.apply(directive, arguments);
                };
            }

            return $delegate;
        });
    });

    ['uibPopoverTemplatePopupDirective', 'uibPopoverHtmlPopupDirective', 'uibPopoverPopupDirective'].forEach(directiveName => {
        $provide.decorator(directiveName, /* @ngInject */function ($delegate, $parse, isMobileService) {
            const directive = $delegate[0];

            const originalCompile = directive.compile;
            let originalLink;

            directive.compile = function (tElement, tAttrs) {

                originalLink = originalCompile ? originalCompile(tElement, tAttrs) : directive.link;

                return function (scope, element, attrs) {
                    if (originalLink != null) {
                        originalLink.apply(directive, arguments);
                    }



                    if (isMobileService.getValue()) {

                        const backdrop = backdropDonor.cloneNode();

                        element[0].insertAdjacentElement(`beforebegin`, backdrop);


                        element.on('$destroy', () => {
               
                            backdrop.remove();
                        });
                    }
                }
            }

            return $delegate;
        });
    });
})