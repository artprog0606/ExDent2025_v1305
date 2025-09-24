/*@ngInject*/
function SidebarsContainerCtrl($attrs, $compile, $document, $element, $scope, $q, $templateRequest, sidebarsContainerService, $timeout) {
    var ctrl = this;
    var storageContentsStatic = {};
    const DEFAULT_TRANSITION_VALUE = `all 0s ease 0s`;

    ctrl.$postLink = function () {
        ctrl.stylesElement = window.getComputedStyle($element[0]);
        sidebarsContainerService.addContainer($attrs.id, ctrl);

        var inputState = $document[0].getElementById('sidebarState');

        if (inputState != null) {
            inputState.parentNode.removeChild(inputState);
        }
    };

    /**
     * @description open sidebar
     * @param {object} options The options for sidebar
     * @property {string} template Html as template sidebar.
     * @property {string} templateUrl Url for load template.
     * @property {boolean} hideHeader Dont show header.
     * @property {boolean} hideFooter Dont show footer.
     * @property {$scope} scope Sidebar scope.
     * @property {string} title Title sidebar.
     * @property {boolean} isStatic Content insert with static mode
     * @property {string} contentId Id
     * @param {boolean} closeOther Need close other sidebar contents
     * @returns {object} Return state object
     *
     */
    ctrl.open = function (options, closeOther) {
        if (options.contentId == null) {
            throw new Error('Option "contentId" is required for method "open" in sidebarContainer');
        }
        ctrl.loader = options.loader;
        return $q
            .when(closeOther === true ? ctrl.close() : true)
            .then(function () {
                var promise;

                if (options.isStatic === true) {
                    if (storageContentsStatic[options.contentId] != null) {
                        promise = $q.resolve(storageContentsStatic[options.contentId]).then(function (el) {
                            ctrl.sidebarEl = el;
                            ctrl.sidebarEl.addClass('sidebar__content-static--open');
                            return ctrl.sidebarEl;
                        });
                    }
                } else {
                    if (options.template != null && options.template.length > 0) {
                        promise = $q.resolve(options.template);
                    } else if (options.templateUrl != null && options.templateUrl.length > 0) {
                        promise = $templateRequest(options.templateUrl);
                    } else {
                        throw Error('Option "template" or "templateUrl" required');
                    }

                    options.hideHeader = options.hideHeader != null ? options.hideHeader : false;
                    options.hideFooter = options.hideFooter != null ? options.hideFooter : false;

                    const close =
                        '<button type="button" class="sidebar__close" data-sidebar-container-close><svg width="24" height="24" viewBox="0 0 24 24"><path fill-rule="evenodd" clip-rule="evenodd" d="M19.2929 3.29289C19.6834 2.90237 20.3166 2.90237 20.7071 3.29289C21.0976 3.68342 21.0976 4.31658 20.7071 4.70711L13.4142 12L20.7071 19.2929C21.0976 19.6834 21.0976 20.3166 20.7071 20.7071C20.3166 21.0976 19.6834 21.0976 19.2929 20.7071L12 13.4142L4.70711 20.7071C4.31658 21.0976 3.68342 21.0976 3.29289 20.7071C2.90237 20.3166 2.90237 19.6834 3.29289 19.2929L10.5858 12L3.29289 4.70711C2.90237 4.31658 2.90237 3.68342 3.29289 3.29289C3.68342 2.90237 4.31658 2.90237 4.70711 3.29289L12 10.5858L19.2929 3.29289Z" fill="currentColor"/></svg></button> ';
                    const footer = !options.hideFooter ? '<div class="sidebar__footer" data-sidebar-container-save></div>' : '';
                    const header = !options.hideHeader ? '<div class="sidebar__header cs-br-1">' + (options.title || '') + close + '</div>' : '';

                    const contentClass =
                        options.hideHeader === true && options.hideFooter === false
                            ? 'sidebar__content--without-header'
                            : options.hideHeader === false && options.hideFooter === true
                              ? 'sidebar__content--without-footer'
                              : '';
                    const wrapClass = [];

                    if (options.hideHeader) {
                        wrapClass.push('sidebar--without-header');
                    }
                    if (options.hideFooter) {
                        wrapClass.push('sidebar--without-footer');
                    }

                    ctrl.isLoadingData = true;

                    ctrl.sidebarEl = angular.element(
                        (!options.hideFooter ? '<form' : '<div') +
                            ' class="sidebar ' +
                            (options.sidebarClass != null ? options.sidebarClass : '') +
                            ' ' +
                            wrapClass.join(' ') +
                            ' cs-bg-4 cs-br-1">' +
                            header +
                            '<div class="sidebar__content ' +
                            contentClass +
                            '">' +
                            '<div class="sidebar__spinner" data-ng-if="sidebarContainer.isLoadingData && !sidebarContainer.loader"><div class="svg-spinner"></div></div>' +
                            '<div class="sidebar__loader" data-ng-if="sidebarContainer.isLoadingData && sidebarContainer.loader" data-ng-bind-html="sidebarContainer.loader"></div>' +
                            '</div>' +
                            footer +
                            (!options.hideFooter ? '</form>' : '</div>') +
                            '<div class="sidebar_overlay" data-sidebar-container-close></div>',
                    );
                    ctrl.sidebarScope = options.scope != null ? options.scope.$new() : $scope.$new();
                    $element.prepend(ctrl.sidebarEl);
                    $compile($element.contents())(ctrl.sidebarScope);
                    ctrl.sidebarContent = ctrl.sidebarEl[0].querySelector('.sidebar__content');

                    promise
                        .then(function (tpl) {
                            ctrl.sidebarContent = ctrl.sidebarEl[0].querySelector('.sidebar__content');
                            $timeout(() => {
                                ctrl.isLoadingData = false;
                                angular.element(ctrl.sidebarContent).append($compile(tpl)(ctrl.sidebarScope));
                            }, 0);
                        })
                        .finally(function () {
                            ctrl.isLoadingData = false;
                        });
                }

                return;
            })
            .then(function () {
                ctrl.options = options;

                setTimeout(function () {
                    $element.addClass('sidebars-container--activated');
                });

                return ctrl.getState();
            })
            .then(function (data) {
                sidebarsContainerService.processObserver(null, data.options.contentId, data, true);
                $document[0].documentElement.classList.add('sidebar-open');
                return data;
            });
    };

    ctrl.checkTransitionIsDefault = function () {
        return (
            ctrl.stylesElement[`transition`] !== `all` &&
            ctrl.stylesElement[`transitionDelay`] !== `0s` &&
            ctrl.stylesElement[`transitionTimingFunction`] !== `ease` &&
            ctrl.stylesElement[`transitionDuration`] !== `0s`
        );
    };

    ctrl.close = function () {
        var defer = $q.defer();

        if (ctrl.sidebarEl == null) {
            return $q.resolve('not element');
        }

        if (ctrl.checkTransitionIsDefault()) {
            ctrl.sidebarEl.on('transitionend', () => defer.resolve());
        } else {
            defer.resolve();
        }

        $element.removeClass('sidebars-container--activated');
        sidebarsContainerService.callCallbacks(`onClose`);
        return defer.promise
            .then(() => ctrl.destroySidebar())
            .then(function (data) {
                sidebarsContainerService.processObserver(null, data.options.contentId, data, false);
                $document[0].documentElement.classList.remove('sidebar-open');
                return data;
            })
            .catch(function () {});
    };

    ctrl.getState = function () {
        return ctrl.sidebarScope != null || ctrl.sidebarEl != null
            ? {
                  sidebarScope: ctrl.sidebarScope,
                  sidebarEl: ctrl.sidebarEl,
                  options: ctrl.options,
              }
            : null;
    };

    ctrl.addContentStatic = function (id, content) {
        storageContentsStatic[id] = content;
    };

    ctrl.toggle = function (options) {
        var state = ctrl.getState();
        var isSelf = state != null && state.options.contentId === options.contentId;

        if (state != null && isSelf === true) {
            return ctrl.close();
        } else {
            return ctrl.open(options, state !== null && isSelf === false);
        }
    };

    ctrl.save = function () {
        if (ctrl.options.onSave != null) {
            ctrl.callbackInProgress = true;
            Promise.allSettled([ctrl.options.onSave()]).then((data) => {
                ctrl.callbackInProgress = false;
            });
        }
    };

    ctrl.setScrollSidebarContent = function (scrollValue, sidebarContent) {
        if (scrollValue != null) {
            let content;

            if (ctrl.sidebarContent != null) {
                content = ctrl.sidebarContent;
            } else if (sidebarContent != null && typeof sidebarContent === 'string') {
                content = $element[0].querySelector(sidebarContent);
            }

            if (content != null) {
                content.scrollTop = scrollValue;
            }
        }
    };

    ctrl.addCallback = function (eventName, callback, needDeleteAfterCall) {
        sidebarsContainerService.addCallback(eventName, callback, needDeleteAfterCall);
    };

    ctrl.destroySidebar = function () {
        var stateOld = ctrl.getState();

        if (ctrl.sidebarScope != null) {
            ctrl.sidebarScope.$destroy();
            delete ctrl.sidebarScope;
        }

        if (ctrl.sidebarEl != null) {
            if (ctrl.options.isStatic !== true) {
                ctrl.sidebarEl.remove();
            } else {
                ctrl.sidebarEl.removeClass('sidebar__content-static--open');
            }

            ctrl.sidebarEl.off();

            delete ctrl.sidebarEl;
        }

        delete ctrl.options;

        stateOld.isOpen = false;

        return stateOld;
    };
}

export default SidebarsContainerCtrl;
