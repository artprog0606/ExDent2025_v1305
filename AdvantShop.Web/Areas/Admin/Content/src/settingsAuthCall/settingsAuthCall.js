(function (ng) {
    'use strict';

    var SettingsAuthCallCtrl = function ($http) {
        var ctrl = this;

        ctrl.getAuthCallMods = function (moduleId) {
            $http.get('settingsAuthCall/getAuthCallMods', { params: { moduleStringId: moduleId } }).then(function (response) {
                ctrl.AuthCallMods = response.data;
                ctrl.AuthCallMode = '0';
            });
        };
    };

    SettingsAuthCallCtrl.$inject = ['$http'];

    ng.module('settingsAuthCall', []).controller('SettingsAuthCallCtrl', SettingsAuthCallCtrl);
})(window.angular);
