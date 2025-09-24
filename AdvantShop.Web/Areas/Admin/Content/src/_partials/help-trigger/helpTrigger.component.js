import helpTriggerTemplate from './templates/help-trigger.html';
import questionIcon from '../../../images/icon/question.svg?raw';

(function (ng) {
    'use strict';

    /*
     * <help-trigger class="ng-cloak" data-title="фывфывфывфыв">
            <strong>sdasfasdas</strong><br />
            <i>asdfsdasd</i>
        </help-trigger>
     */
    var increment = 1;

    ng.module('helpTrigger').directive(
        'helpTrigger',
        /* @ngInject*/ function ($sce, $templateRequest, $compile, $templateCache, $parse, urlHelper, $http) {
            return {
                controller: 'HelpTriggerCtrl',
                bindToController: true,
                controllerAs: '$ctrl',
                transclude: {
                    helpTriggerIcon: '?helpTriggerIcon',
                },
                //scope: {
                //    title: '@',
                //    useTemplate: '<?'
                //},
                scope: true,
                link: async function (scope, element, attrs, ctrl, transclude) {
                    ctrl.title = attrs.title;
                    ctrl.placement = attrs.placement || 'auto right';
                    ctrl.useTemplate = attrs.useTemplate ? attrs.useTemplate === 'true' : false;
                    ctrl.helpAppendToBody = attrs.helpAppendToBody != null ? $parse(attrs.helpAppendToBody)(scope) : true;
                    ctrl.classes = attrs.classes != null ? $parse(attrs.classes)(scope) : '';
                    ctrl.pathSVGSprite = attrs.pathSVGSprite != null ? attrs.pathSVGSprite : null;
                    // remove when in admin will add svg sprite.
                    ctrl.isInfoIcon = attrs.isInfoIcon != null ? attrs.isInfoIcon : null;
                    ctrl.templateUrl = attrs.templateUrl != null ? attrs.templateUrl : null;
                    ctrl.templateUrlParams =
                        ctrl.templateUrl != null && attrs.templateUrlParams != null ? $parse(attrs.templateUrlParams)(scope) : null;
                    ctrl.trigger = attrs.trigger != null ? attrs.trigger : false;
                    ctrl.helpTriggerIconBg = attrs.helpTriggerIconBg === null ? `white` : null;

                    const tpl = await $templateRequest(helpTriggerTemplate);

                    let innerEl = document.createElement('div'),
                        uiPopoverEl,
                        clone,
                        content,
                        iconContent;

                    var childScope = scope.$new();
                    ctrl.innerPopoverContentClass = 'js-help-trigger-content-' + increment;
                    innerEl.innerHTML = tpl;

                    uiPopoverEl = innerEl.querySelector('.help-trigger-icon');

                    const container = document.createElement('div');

                    const iconContentCustomExist = transclude.isSlotFilled(`helpTriggerIcon`);
                    const iconParent = angular.element(innerEl).find(`.js-help-trigger-icon-content`);
                    if (iconContentCustomExist) {
                        transclude(
                            childScope,
                            function (cloneElements, innerScope) {
                                iconParent.append(cloneElements);
                            },
                            null,
                            `helpTriggerIcon`,
                        );
                    } else {
                        iconParent.html(questionIcon);
                    }

                    transclude(childScope, function (cloneElements) {
                        clone = cloneElements;
                        for (var i = 0; i < cloneElements.length; i++) {
                            container.appendChild(cloneElements[i]);
                        }
                    });
                    content = container.outerHTML;

                    if (ctrl.templateUrl != null) {
                        content = await $http.get(ctrl.templateUrl, { params: ctrl.templateUrlParams }).then((response) => response.data);
                    }

                    uiPopoverEl.setAttribute('popover-class', ctrl.innerPopoverContentClass + ' ' + ctrl.classes);

                    if (ctrl.useTemplate === true || ctrl.templateUrl != null) {
                        uiPopoverEl.setAttribute('uib-popover-template', "'helpTrigger_" + increment + ".html'");
                        $templateCache.put('helpTrigger_' + increment + '.html', content);
                    } else {
                        uiPopoverEl.setAttribute('uib-popover-html', '$ctrl.content');
                        ctrl.content = $sce.trustAsHtml(ng.element('<div />').append(clone).html());
                    }

                    element[0].innerHTML = innerEl.innerHTML;
                    var childs = element[0].children;

                    $compile(childs)(scope);

                    increment++;
                },
            };
        },
    );
})(window.angular);
