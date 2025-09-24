function sidebarsContainerDirective() {
    return {
        controller: 'SidebarsContainerCtrl',
        controllerAs: 'sidebarContainer',
        bindToController: true,
        // scope: true
    };
}

/*@ngInject*/
function sidebarContainerCloseDirective(sidebarsContainerService) {
    return {
        require: {
            sidebarsContainer: '?^sidebarsContainer',
        },
        controller: function () {},
        controllerAs: 'sidebarContainerClose',
        bindToController: true,
        link: function (scope, element, attrs, ctrl) {
            element.on('click', function () {
                if (ctrl.sidebarsContainer != null) {
                    sidebarsContainerService.close();
                }
            });

            element.on('$destroy', function () {
                element.off();
            });
        },
    };
}

function sidebarContainerStateDirective() {
    return {
        controller: [
            '$attrs',
            'sidebarsContainerService',
            function ($attrs, sidebarsContainerService) {
                var ctrl = this;

                ctrl.onChange = function (data, isOpen) {
                    ctrl.isOpen = isOpen;
                };

                ctrl.$onInit = function () {
                    sidebarsContainerService.addObserverState(null, $attrs.sidebarContainerState || null, ctrl.onChange);
                };
            },
        ],
        controllerAs: 'sidebarContainerState',
        bindToController: true,
    };
}

const sidebarContentStaticComponent = {
    require: {
        sidebarsContainer: '^sidebarsContainer',
    },
    bindings: {
        contentId: '@',
    },
    controller: [
        '$element',
        function ($element) {
            var ctrl = this;

            ctrl.$onInit = function () {
                ctrl.sidebarsContainer.addContentStatic(ctrl.contentId, $element);
            };
        },
    ],
};

function sidebarContainerSaveDirective() {
    return {
        require: {
            sidebarsContainer: '?^sidebarsContainer',
        },
        controller: function () {},
        controllerAs: 'sidebarContainerOpen',
        template:
            '<button data-button-validation data-button-validation-success="sidebarContainerOpen.sidebarsContainer.save()" class="sidebar-container-save-btn" data-ladda="sidebarContainerOpen.sidebarsContainer.callbackInProgress" type="button"/>{{::\'Js.Builder.Save\' | translate}}</button>',
        bindToController: true,
        link: function (scope, element, attrs, ctrl) {
            element.on('$destroy', function () {
                element.off();
            });
        },
    };
}

/*@ngInject*/
function sidebarHookCloseDirective($parse, sidebarsContainerService) {
    return {
        require: {
            sidebarsContainer: '?^sidebarsContainer',
        },
        scope: false,
        link: function (scope, element, attrs) {
            const fn = $parse(attrs.sidebarHookClose);
            sidebarsContainerService.addCallback(`onClose`, () => fn(scope));
        },
    };
}

export {
    sidebarsContainerDirective,
    sidebarContainerCloseDirective,
    sidebarContainerStateDirective,
    sidebarContentStaticComponent,
    sidebarContainerSaveDirective,
    sidebarHookCloseDirective,
};
