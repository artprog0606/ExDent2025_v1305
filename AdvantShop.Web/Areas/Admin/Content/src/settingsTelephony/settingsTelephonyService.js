(function (ng) {
    'use strict';

    var settingsTelephonyService = function ($http, $translate, $q, toaster) {
        var service = this;

        service.addPhone = function (phone, orderSourceId, phoneOrderSources) {
            if (!phone || !orderSourceId) {
                toaster.error($translate.instant('Admin.Js.SettingsTelephony.MissingData'));
                return $q.resolve();
            }
            if (phoneOrderSources[phone]) {
                toaster.error($translate.instant('Admin.Js.SettingsTelephony.BundleWithThisNumber'));
                return $q.resolve();
            }
            phoneOrderSources[phone] = orderSourceId;
            return service.savePhoneOrderSources(phoneOrderSources);
            /*ctrl.newPhone = ctrl.newOrderSourceId = '';*/
        };

        service.savePhoneOrderSources = function (phoneOrderSources) {
            return $http
                .post('settingsTelephony/savePhoneOrderSources', {
                    phoneOrderSources: JSON.stringify(phoneOrderSources),
                })
                .then(function (response) {
                    if (response.data.result == true) {
                        toaster.success($translate.instant('Admin.Js.SettingsTelephony.ChangesSaved'));
                    } else {
                        toaster.error($translate.instant('Admin.Js.SettingsTelephony.ErrorWhileSaving'));
                    }
                    return service.getPhoneOrderSources();
                });
        };

        service.getPhoneOrderSources = function () {
            return $http.post('settingsTelephony/getPhoneOrderSources');
        };
    };

    settingsTelephonyService.$inject = ['$http', '$translate', '$q', 'toaster'];

    ng.module('settingsTelephony').service('settingsTelephonyService', settingsTelephonyService);
})(window.angular);
