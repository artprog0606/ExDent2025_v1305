const moduleName = `moreButton`;

angular.module(moduleName, []).directive(
    `moreButton`,
    /* @ngInject */ function ($parse, $uibModalStack, $timeout) {
        return {
            link: function (scope, element, attrs) {
                let modalObserverDestroy;
                const attrValue = attrs.popoverIsOpen;
                scope.$watch(attrValue, function (newVal, oldVal) {
                    if (newVal != null && newVal !== oldVal) {
                        if (newVal === true) {
                            modalObserverDestroy = scope.$on(`modal.decorate.closing`, function (event, data) {
                                $timeout(() => {
                                    if (data.isClose === true && $uibModalStack.getTop() == null) {
                                        $parse(attrValue).assign(scope, false);
                                    }
                                });
                            });
                        } else if (modalObserverDestroy != null) {
                            modalObserverDestroy();
                            modalObserverDestroy = null;
                        }
                    }
                });
            },
        };
    },
);

export default moduleName;
