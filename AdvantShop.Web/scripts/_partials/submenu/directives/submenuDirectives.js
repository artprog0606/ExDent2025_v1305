(function (ng) {
    'use strict';

    angular.module('submenu').directive('submenuContainer', [
        '$window',
        'submenuConfig',
        function ($window, submenuConfig) {
            return {
                restrict: 'A',
                controller: 'SubmenuContainerCtrl',
                controllerAs: 'submenuContainer',
                bindToController: true,
                scope: true,
                link: function (scope, element, attrs, ctrl) {
                    var blockOrientation;

                    ctrl.options = angular.extend(ctrl.options, ng.copy(submenuConfig), new Function('return ' + attrs.submenuContainer)() || {});

                    if (ctrl.options.blockOrientation != null) {
                        blockOrientation = document.querySelector(ctrl.options.blockOrientation);
                    }

                    ctrl.addContainerForOrientation(blockOrientation || element[0]);

                    if (ctrl.options.breakpoints != null && ctrl.options.breakpoints.length > 0) {
                        ctrl.options.breakpoints.forEach(function (breakpoint) {
                            var mql = $window.matchMedia('(min-width:' + breakpoint.media + 'em)');
                            mql.addListener(ctrl.onChangeMatchMedia(breakpoint, mql));
                        });
                    } else {
                        ctrl.init(ctrl.options);
                    }
                },
            };
        },
    ]);

    angular.module('submenu').directive('submenuParent', [
        function () {
            return {
                require: ['submenuParent', '^submenuContainer', '?^^submenuParent'],
                restrict: 'A',
                controller: 'SubmenuParentCtrl',
                controllerAs: 'submenuParent',
                bindToController: true,
                scope: true,
                link: function (scope, element, attrs, ctrls) {
                    var submenuCurrentParent = ctrls[0],
                        submenuContainer = ctrls[1],
                        submenuParent = ctrls[2],
                        index = submenuContainer.items.length;

                    if (submenuParent != null) {
                        submenuCurrentParent.addParent(submenuParent);
                    }

                    submenuContainer.addItem(submenuCurrentParent);

                    submenuCurrentParent.index = index;

                    submenuCurrentParent.memoryElement(element[0]);

                    attrs.$set('data-submenu-parent-index', index);
                },
            };
        },
    ]);

    angular.module('submenu').directive('submenu', function () {
        return {
            //['submenu', '^submenuParent', '^submenuContainer']
            require: {
                submenu: 'submenu',
                submenuParent: '^submenuParent',
                submenuContainer: '^submenuContainer',
            },
            restrict: 'A',
            scope: true,
            controller: 'SubmenuCtrl',
            controllerAs: 'submenu',
            bindToController: true,
            link: function (scope, element, attrs, ctrls) {
                var submenu = ctrls.submenu,
                    submenuParent = ctrls.submenuParent,
                    submenuContainer = ctrls.submenuContainer,
                    offsetBottom = parseFloat(attrs.submenuOffsetBottom),
                    offsetRight = parseFloat(attrs.submenuOffsetRight);

                submenu.options = submenuContainer.getOptions();

                submenu.offset = {};
                submenu.offset.bottom = !isNaN(offsetBottom) ? offsetBottom : 0;
                submenu.offset.right = !isNaN(offsetRight) ? offsetRight : 0;

                submenuParent.addSubmenu(submenu);
            },
        };
    });
})(angular);
