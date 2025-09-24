/* @ngInject */
function TabsCtrl($q, tabsService, $scope) {
    var ctrl = this,
        panes = (ctrl.panes = {}),
        queueHeader = {},
        queueContent = {},
        tabSelected,
        locationUnwatch;

    ctrl.$postLink = function () {
        ctrl.selectFromUrl();

        window.addEventListener('hashchange', function (e) {
            ctrl.selectFromUrl();
            $scope.$digest();
        });
    };

    ctrl.selectFromUrl = function () {
        var tabId = tabsService.getTabIdFromUrl();

        if (tabId != null && panes[tabId] != null) {
            if (tabId != null && ctrl.tabsOnSelect != null) {
                ctrl.tabsOnSelect($scope, { tabHeader: panes[tabId], fromUrl: true });
            }
            var tabObj = tabsService.findTabByid(tabId);
            if (panes[tabId].headerTab != null && panes[tabId].headerTab.length > 0) {
                ctrl.headerTab = panes[tabId].headerTab;
            }
            if (tabObj != null && tabObj.pane != null && tabObj.pane.selected === false) {
                ctrl.change(tabObj.pane, true);
            }
        }
    };

    ctrl.select = function (tabHeader) {
        var keys = Object.keys(panes);

        if (ctrl.tabsOnSelect != null) {
            ctrl.tabsOnSelect($scope, { tabHeader: tabHeader });
        }

        if (ctrl.isToggle === false) {
            for (var i = 0, len = keys.length; i < len; i++) {
                panes[keys[i]].selected = false;
            }

            tabHeader.selected = true;
            tabSelected = tabHeader;
        } else {
            tabHeader.selected = !tabHeader.selected;
            tabSelected = tabHeader.selected === false ? null : tabHeader;
        }
    };

    ctrl.addHeader = function (tabHeader) {
        var defer = $q.defer(),
            searchTabId = tabsService.getTabIdFromUrl();

        panes[tabHeader.id] = tabHeader;

        if (queueHeader[tabHeader.id] != null) {
            queueHeader[tabHeader.id].resolve(tabHeader);

            queueHeader[tabHeader.id].promise.then(function (tabHeader) {
                //tabHeader.isRender = tabContent.isRender;

                if (tabHeader.isRender === true && (tabHeader.id == searchTabId || tabSelected == null)) {
                    ctrl.select(tabHeader);
                } else {
                    tabHeader.selected = false;
                }
            });
        } else {
            if (tabHeader.content == null) {
                queueContent[tabHeader.id] = defer;
            } else {
                defer.resolve(tabHeader.content);
            }

            defer.promise.then(function (tabContent) {
                tabHeader.content = tabContent;
                tabHeader.isRender = tabContent.isRender;

                if (tabHeader.isRender === true && (tabHeader.id == searchTabId || tabSelected == null)) {
                    ctrl.select(tabHeader);
                } else {
                    tabHeader.selected = false;
                }

                return tabContent;
            });
        }
    };

    ctrl.addContent = function (tabContent) {
        var header = panes[tabContent.headerId],
            defer = $q.defer();

        //если заголовок ещё не проинициализовался
        if (header == null) {
            queueHeader[tabContent.headerId] = defer;
        } else {
            defer.resolve(header);
        }

        defer.promise.then(function (header) {
            tabContent.header = header;
            return header;
        });

        //проверяем если обещания на получения контента
        if (queueContent[tabContent.headerId] != null) {
            queueContent[tabContent.headerId].resolve(tabContent);
        }
    };

    ctrl.change = function (tabHeader, ignoreUrl) {
        if (ctrl.allowHideAll === true && tabHeader.selected === true) {
            tabHeader.selected = false;
        } else {
            ctrl.select(tabHeader);
            if (tabHeader.headerTab != null && tabHeader.headerTab.length > 0) {
                ctrl.headerTab = tabHeader.headerTab;
            }
            if (angular.isDefined(tabHeader.id) && !ignoreUrl) {
                tabsService.changeUrl(tabHeader.id);
            }
        }
    };
}

export default TabsCtrl;
