export const scrollToBlockDirective = /* @ngInject */ ($parse, scrollToBlockService, scrollToBlockConfig) => {
    return {
        restrict: 'AE',
        require: ['scrollToBlock', '?^scrollSpy'],
        controller: function () {},
        controllerAs: 'scrollToBlock',
        scope: true,
        link: function (scope, element, attrs, ctrlList) {
            const el = element[0],
                scrollSpyCtrl = ctrlList[1],
                onClick = attrs.scrollToBlockOnClick != null ? $parse(attrs.scrollToBlockOnClick) : null,
                onStartScroll = attrs.scrollToBlockOnStart != null ? $parse(attrs.scrollToBlockOnStart) : null,
                onEndScroll = attrs.scrollToBlockOnEnd != null ? $parse(attrs.scrollToBlockOnEnd) : null;

            const onClickWrap = () => {
                if (scrollSpyCtrl) {
                    scrollSpyCtrl.setActive();
                }
                if (onClick != null) {
                    onClick(scope, { $event: event });
                }
            };

            const onStartScrollWrap = () => {
                if (scrollSpyCtrl) {
                    scrollSpyCtrl.deactivate();
                }
                if (onStartScroll != null) {
                    onStartScroll(scope);
                }
            };

            const onEndScrollWrap = () => {
                if (scrollSpyCtrl) {
                    scrollSpyCtrl.activate();
                }

                if (onEndScroll != null) {
                    onEndScroll(scope);
                }
            };

            el.addEventListener('click', function (event) {
                const selector = attrs.scrollToBlock || attrs.href;
                const block = document.querySelector(selector);
                if (block == null) {
                    return;
                }
                const options = Object.assign(
                    {},
                    scrollToBlockConfig,
                    attrs.scrollToBlockOptions != null && $parse(attrs.scrollToBlockOptions)(scope),
                );
                event.preventDefault();
                scrollToBlockService.scrollToBlock(block, options || {}, onStartScrollWrap, onEndScrollWrap);

                onClickWrap();

                scope.$apply();
            });
        },
    };
};
