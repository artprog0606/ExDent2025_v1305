import tabsetTemplate from './tabset.html';
import tabTemplate from './tab.html';

'use strict';

const tryParseNumber = (value) => typeof (value) === 'string' && /^\d+$/.test(value) ? parseInt(value) : value;

const scrollIntoViewByCenter = (element) => {
    const parent = element.parentElement;
    const elWidth = element.offsetWidth;
    const parentCenter = parent.offsetWidth / 2;
    parent.scroll({ behavior: `smooth`, left: element.offsetLeft - parentCenter + (elWidth / 2) });
};
angular.module('ui.bootstrap.tabs').config([
    '$provide',
    function($provide) {
        $provide.decorator('uibTabsetDirective', /* @ngInject */
            function(
                $delegate,
                $location,
                $compile,
                $timeout,
                backService,
                urlHelper,
                isMobileService,
                appService,
                uiTabsService
            ) {
                var directive = $delegate[0];

                directive.bindToController.uid = '@';

                directive.bindToController.onSelectBatch = '&';

                directive.bindToController.classesContent = '@';

                directive.bindToController.headersOverflowType = '@';

                directive.bindToController.tabsMode = '<?';

                directive.bindToController.onInit = '&';

                directive.require = {
                    tabset: 'uibTabset',
                    tabsetParent: '?^^uibTabset',
                };

                directive.transclude = {
                    headerAdditionalControl: '?uibTabsetAdditonalHeader',
                };

                directive.templateUrl = function(element, attrs) {
                    return (
                        attrs.templateUrl ||
                        tabsetTemplate
                    );
                };

                var originalLink = directive.link;

                var originalCompile = directive.compile;

                var link = function myLinkFnOverride(
                    scope,
                    element,
                    attrs,
                    ctrls,
                ) {
                    var tabsCtrl = ctrls.tabset;
                    var tabsParentCtrl = ctrls.tabsetParent;
                    var destroyAll = false;
                    var tabsIsNavigations =
                        tabsCtrl.tabsMode === 'navigation';

                    if (tabsCtrl.uid == null) {
                        throw new Error('Not defined uid the tab');
                    }

                    //remove old static key
                    localStorage.removeItem('admin-URLtab');

                    const nameForStorage = uiTabsService.getNameForStorage();

                    if (tabsIsNavigations === true) {
                        element[0].classList.add('tabs-navigation');

                        backService.addListener(function(data) {
                            //var search = $location.search();
                            //if(search[tabsCtrl.uid] == null){
                            if (data.old[1].state.uid === tabsCtrl.uid) {
                                tabsCtrl.navigationContentShow = false;
                                $location.search(
                                    tabsCtrl.uid,
                                    null,
                                );

                                if (data.current != null && tabsParentCtrl != null && data.current[1].state.uid === tabsParentCtrl.uid) {
                                    const tabActiveParent = tabsParentCtrl.tabs.find(function(item) {
                                        return item.index === tabsParentCtrl.active;
                                    });
                                    if (tabActiveParent != null && tabActiveParent.tab.heading != null) {
                                        appService.setTitle(tabActiveParent.tab.heading);
                                    }
                                }
                                scope.$digest();
                            }
                            //}
                        });
                    }

                    var localStorageData = JSON.parse(
                            localStorage.getItem(nameForStorage),
                        ),
                        urlData = urlHelper.getUrlParamsUniversalAsObject(),
                        activeTab,
                        collapseTabEl = document.createElement('div'),
                        tabs = tabsCtrl.tabs;

                    const uibTabBeforeCompile = element[0].querySelectorAll('uib-tab');

                    // нужно, чтобы вызвать изменение при загрузке, когда будет список табов
                    scope.$watchCollection('tabset.tabs', (newTabs) => {
                        if (uibTabBeforeCompile.length === tabs?.length) {
                            const currentTabIndex = tabsCtrl.tabs.findIndex(x => x.index === activeTab);
                            tabsCtrl.select(currentTabIndex >= 0 ? currentTabIndex : 0);
                        }
                    });

                    if (
                        tabsCtrl.headersOverflowType !== 'scroll' &&
                        tabsCtrl.tabsMode !== 'navigation'
                    ) {
                        collapseTabEl.setAttribute('collapse-tab', '');
                        element[0].appendChild(collapseTabEl);
                        $compile(collapseTabEl)(scope);
                    }

                    if (urlData[tabsCtrl.uid]) {
                        // если есть url
                        activeTab = tryParseNumber(urlData[tabsCtrl.uid]);

                        if (tabsCtrl.tabsMode === 'navigation') {
                            tabsCtrl.navigationContentShow = true;
                            setTimeout(() => {
                                backService.pushHistoryItem(
                                    tabsCtrl.uid + '_' + activeTab,
                                    {
                                        state: {
                                            uid: tabsCtrl.uid,
                                            index: activeTab,
                                        },
                                        title: '',
                                        url: window.location.href,
                                    },
                                );
                            }, 0);
                        }
                    } else if (tabsIsNavigations === false) {
                        // взять из localstorage

                        if (localStorageData != null) {
                            if (
                                new Date(localStorageData.date).getTime() +
                                300000 -
                                new Date().getTime() <
                                0
                            ) {
                                // 300000 - 5 минут

                                localStorage.removeItem(nameForStorage);
                            }
                            if (localStorageData[tabsCtrl.uid]) {
                                activeTab = tryParseNumber(localStorageData[tabsCtrl.uid]);
                            }
                        }
                    }

                    if (activeTab != null) {
                        tabsCtrl.active = activeTab;

                        //tabsCtrl.active = parseFloat(activeTab);

                        //if (isNaN(tabsCtrl.active)) {
                        //    tabsCtrl.active = activeTab;
                        //}
                    }

                    var originalSelect = tabsCtrl.select;
                    var originalAddTab = tabsCtrl.addTab;
                    var timerAddTab;
                    var tabListTemp = [];
                    var isInit = false;

                    function findTabIndex(index) {
                        for (var i = 0; i < tabsCtrl.tabs.length; i++) {
                            if (tabsCtrl.tabs[i].index === index) {
                                return i;
                            }
                        }
                    }

                    tabsCtrl.addTab = function(tab) {

                        tabListTemp.push(tab);

                        if (timerAddTab != null) {
                            $timeout.cancel(timerAddTab);
                        }

                        timerAddTab = $timeout(
                            () => {
                                tabListTemp.forEach((tab) => {

                                        tabsCtrl.tabs.push({
                                            tab: tab,
                                            index: tab.index,
                                        });

                                        if (tab.index === tabsCtrl.active || !angular.isDefined(tabsCtrl.active) && tabsCtrl.tabs.length === 1) {
                                            var newActiveIndex = findTabIndex(tab.index);
                                            tabsCtrl.select(newActiveIndex);
                                        }

                                        if (tabsCtrl.onInit != null) {
                                            tabsCtrl.onInit();
                                        }
                                    },
                                );
                                tabListTemp.length = 0;
                            },
                            100,
                        );
                    };

                    tabsCtrl.select = function(index, evt) {
                        if (
                            destroyAll === true ||
                            tabsCtrl.tabs.length === 0
                        ) {
                            return;
                        }

                        if (index < 0) {
                            return originalSelect.apply(this, arguments);
                        }

                        if (index == null) {
                            index = 0;
                        }
                        if (
                            tabsCtrl.uid.length != 0 &&
                            index != null &&
                            tabs[index] != null &&
                            evt != null &&
                            tabsCtrl.tabsMode !== 'navigation'
                        ) {
                            $location.search(
                                tabsCtrl.uid,
                                tabs[index].index,
                            );

                            // urlHelper.getUrlParamsUniversalAsObject() - получал старые данные, т.к $location.search обновляет url позже
                            // var dataTabs = Object.assign(
                            //     {},
                            //     urlHelper.getUrlParamsUniversalAsObject(),
                            // );
                            // if (!dataTabs[tabsCtrl.uid]) {
                            //     dataTabs[tabsCtrl.uid] =
                            //         urlData[tabsCtrl.uid];
                            // }
                            const dataTabs = {
                                [tabsCtrl.uid]: tabs[index].index,
                                data: new Date(),
                            };

                            localStorage.setItem(
                                nameForStorage,
                                JSON.stringify(dataTabs),
                            );
                        }

                        if (
                            (tabsCtrl.onSelectBatch != null &&
                                (tabsCtrl.tabsMode !== 'navigation' ||
                                    tabsCtrl.navigationContentShow)) ||
                            evt != null
                        ) {

                            appService.setTitle(tabsCtrl.tabs[index].tab.heading);

                            tabsCtrl.onSelectBatch({
                                index: index,
                                event: evt,
                                tab: tabsCtrl.tabs[index].tab,
                                tabset: tabsCtrl,
                            });
                        }

                        var resultOriginSelect = originalSelect.apply(
                            this,
                            isInit ? arguments : [index],
                        );

                        isInit = true;

                        $timeout(function() {
                            scope.$broadcast('uiGridCustomAutoResize');
                        }, 0);

                        //проверяем, что посетитель через интерфейс открыл вкладку
                        if (evt != null) {
                            if (tabsCtrl.tabsMode === 'navigation') {
                                tabsCtrl.navigationContentShow = true;
                                // TODO убрать костыль когда переделают табы разобьют на формы
                                scope.$broadcast('reinitSwipeLineEvent');

                                setTimeout(() => {
                                    backService.pushHistoryItem(
                                        tabsCtrl.uid + '_' + index,
                                        {
                                            state: {
                                                uid: tabsCtrl.uid,
                                                index: index,
                                            },
                                            title: '',
                                            url: window.location.href,
                                        },
                                    );
                                }, 0);
                            } else {
                                scrollIntoViewByCenter(evt.target);
                            }
                        }

                        return resultOriginSelect;
                    };

                    scope.$on('$destroy', function() {
                        destroyAll = true;
                        tabsCtrl.navigationContentShow = false;
                        $timeout(function() {
                            $location.search(tabsCtrl.uid, null);
                        }, 0);
                    });

                    return originalLink.apply($delegate, arguments);
                };

                directive.compile = function(cElement, cAttrs) {
                    return function(scope, element, attrs, tabsCtrl) {
                        link.apply(this, arguments);
                    };
                };

                return $delegate;
            },
        );

        $provide.decorator('uibTabDirective', [
            '$delegate',
            '$timeout',
            function($delegate, $timeout) {
                var directive = $delegate[0];

                directive.scope.removable = '<?';

                directive.scope.classesContent = '@';

                directive.templateUrl = function(element, attrs) {
                    return attrs.templateUrl || tabTemplate;
                };

                var link = directive.link;

                directive.compile = function(cElement, cAttrs) {
                    return function(scope, element, attrs, tabsCtrl) {
                        link.apply(this, arguments);

                        if (scope.index === tabsCtrl.active) {
                            $timeout(function() {
                                scrollIntoViewByCenter(element[0]);
                            }, 100);
                        }
                    };
                };

                return $delegate;
            },
        ]);

        $provide.decorator('uibTabHeadingTranscludeDirective', [
            '$delegate',
            function($delegate) {
                var directive = $delegate[0];

                var arrow =
                    '<svg width="8" height="12" viewBox="0 0 8 12" fill="none" xmlns="http://www.w3.org/2000/svg"><path d="M0.589996 10.59L5.17 6L0.589996 1.41L2 0L8 6L2 12L0.589996 10.59Z" fill="black" /></svg>';

                var originalLink = directive.link;

                var link = function(scope, elm, attrs, uibTab) {
                    scope.$watch(
                        'headingElement',
                        function updateHeadingElement(heading) {
                            if (heading) {
                                elm.html('');
                                elm.append(heading);
                                if (
                                    scope.$parent.$parent.tabset
                                        .tabsMode === 'navigation'
                                ) {
                                    elm.append(arrow);
                                }
                            }
                        },
                    );
                };

                directive.compile = function(cElement, cAttrs) {
                    return function(scope, element, attrs, tabsCtrl) {
                        link.apply(this, arguments);
                    };
                };

                return $delegate;
            },
        ]);
    },
])
    .service('uiTabsService', /* @ngInject */function(urlHelper) {
        const service = this;

        service.getNameForStorage = (name = 'admin-URLtab') => {
            return `${name}_${urlHelper.transformBaseUriToKey()}`;
        }
    });

