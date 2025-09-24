(function (ng) {
    'use strict';

    var SettingsCtrl = function ($http, toaster) {
        var ctrl = this;

        ctrl.saveCommonInfo = function (form) {
            $http.post('settings/saveCommonInfo', ctrl.common).then(function (response) {
                var data = response.data;
                if (data.result == true) {
                    toaster.success('', 'Изменения сохранены');
                    form.$setPristine();
                }
            });
        };
    };

    SettingsCtrl.$inject = ['$http', 'toaster'];

    ng.module('settings', []).controller('SettingsCtrl', SettingsCtrl);
})(window.angular);
