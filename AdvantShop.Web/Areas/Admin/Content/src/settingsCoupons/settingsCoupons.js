(function (ng) {
    'use strict';

    var SettingsCouponsCtrl = function (settingsCouponsService) {
        var ctrl = this;

        ctrl.onSelectTab = function (indexTab) {
            ctrl.tabActiveIndex = indexTab;
            ctrl.tabData = ctrl.getTabData(indexTab);
        };
        //запоминаем данные при смене таба
        ctrl.setTabData = function (indexTab, data) {
            if (!settingsCouponsService.getTabDataStorage(settingsCouponsService)) {
                settingsCouponsService.setTabDataStorage(indexTab, data);
            }
        };

        ctrl.getTabData = function (indexTab) {
            return settingsCouponsService.getTabDataStorage(indexTab);
        };
    };

    SettingsCouponsCtrl.$inject = ['settingsCouponsService'];

    ng.module('settingsCoupons', ['isMobile']).controller('SettingsCouponsCtrl', SettingsCouponsCtrl);
})(window.angular);
