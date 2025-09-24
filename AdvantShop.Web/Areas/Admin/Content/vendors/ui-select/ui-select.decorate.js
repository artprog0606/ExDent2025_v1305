import requiredTmpl from './select-multiple-required.tpl.html';

angular.module('ui.select').config(/* @ngInject */function ($provide) {
    
    $provide.decorator(`uiSelectDirective`, /* @ngInject */function ($delegate, uiSelectConfig, isMobileService, uisOffset, $templateCache, $timeout) {
        const directive = $delegate[0];
        const isMobile = isMobileService.getValue();
        const compileOriginal = directive.compile;
        
        directive.templateUrl = function(tElement, tAttrs) {
            const required = angular.isDefined(tAttrs.ngRequired);
            const theme = tAttrs.theme || uiSelectConfig.theme;
            const isMultiple = angular.isDefined(tAttrs.multiple);
            if(required && isMultiple) {
                return requiredTmpl;
            }
            return theme + (isMultiple ? '/select-multiple.tpl.html' : '/select.tpl.html');
        };

        //в мобилке при щелчке на инпуте
        if (isMobile) {
            directive.compile = function (tElement, tAttrs) {

                const linkOriginal = compileOriginal(tElement, tAttrs);

                return function link(scope, element, attrs, ctrls, transcludeFn) {
                    const appendToBodyFromAttr = scope.$eval(attrs.appendToBody);

                    let selectCloseWhenBlur = function () {
                        $timeout(() => {
                            scope.$select.close();
                            scope.$apply();
                        }, 0)
                        
                    }

                    if (element[0].querySelector('input.ui-select-search') != null) {
                        element[0].querySelector('input.ui-select-search').addEventListener('blur', selectCloseWhenBlur);

                        element.on('$destroy', function () {
                            element[0].querySelector('input.ui-select-search').removeEventListener("blur", selectCloseWhenBlur);
                        });
                    }

                    if (appendToBodyFromAttr !== undefined ? appendToBodyFromAttr : uiSelectConfig.appendToBody) {
                        
                        let timer = null;
                        const parent = element.parent();

                        scope.$watch('$select.open', function (isOpen) {
                            if (timer != null) {
                                clearInterval(timer);
                                timer = null;
                            }

                            if (isOpen) {
                                timer = setInterval(() => {
                                    const placeholder = parent.children('.ui-select-placeholder');

                                    var offset = uisOffset(placeholder);

                                    element[0].style.left = offset.left + 'px';
                                    element[0].style.top = offset.top + 'px';
                                    element[0].style.width = offset.width + 'px';
                                }, 300);
                            }
                        });

                        scope.$on('$destroy', function () {
                            if (timer != null) {
                                clearInterval(timer);
                                timer = null;
                            }
                        });
                    }
                    return linkOriginal(scope, element, attrs, ctrls, transcludeFn);
                }
            }
        }

        return $delegate;
    });
})