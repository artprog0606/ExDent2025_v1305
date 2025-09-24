/*@ngInject*/
function showMoreDirective($parse) {
    return {
        scope: true,
        controller: 'ShowMoreCtrl',
        controllerAs: 'showMore',
        bindToController: true,
        compile: function (cElement, cAttrs) {
            const initHtmlContainer = cElement[0].querySelector(`[data-show-more-init-html],[show-more-init-html],show-more-init-html`);
            let html = null;
            if (initHtmlContainer) {
                html = initHtmlContainer.innerHTML;
            }
            return function (scope, element, attrs, ctrl) {
                ctrl.requestUrl = attrs.requestUrl || '';
                ctrl.urlParameter = attrs.urlParameter || 'page';
                ctrl.dataParams = $parse(attrs.dataParams)(scope) || {};
                ctrl.addInitHtmlContainer(initHtmlContainer, html);
            };
        },
    };
}

function showMoreInitHtmlDirective() {
    return {
        require: '^showMore',
    };
}

export { showMoreDirective, showMoreInitHtmlDirective };
