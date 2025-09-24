(function (ng) {
    'use strict';

    const settingsCouponsService = function () {
        const service = this;
        const storageTabs = {};

        service.setTabDataStorage = function (tabsName, data) {
            storageTabs[tabsName] = data;
        };

        service.getTabDataStorage = function (tabsName) {
            return storageTabs[tabsName];
        };
    };

    settingsCouponsService.$inject = [];

    ng.module('settingsCoupons').service('settingsCouponsService', settingsCouponsService);
})(window.angular);
