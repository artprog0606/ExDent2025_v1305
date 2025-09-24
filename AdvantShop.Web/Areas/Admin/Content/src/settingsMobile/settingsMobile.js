(function (ng) {
    'use strict';

    var SettingsMobileCtrl = function ($http, toaster, $location) {
        var ctrl = this;
        var SETTINGSTAB_SEARCH_NAME = 'settingsMobileTab';

        ctrl.changeSettingsTab = function (tab) {
            ctrl.settingsTab = tab;
            $location.search(SETTINGSTAB_SEARCH_NAME, tab);
        };

        ctrl.$onInit = function () {
            var search = $location.search();
            ctrl.settingsTab = (search != null && search[SETTINGSTAB_SEARCH_NAME]) || 'settings';

            ctrl.colorPickerOptions = {
                swatchBootstrap: false,
                format: 'hex',
                alpha: false,
                case: 'lower',
                swatchOnly: false,
                allowEmpty: true,
                required: false,
                preserveInputFormat: false,
                restrictToFormat: false,
                inputClass: 'form-control',
            };

            ctrl.colorPickerEventApi = {};

            ctrl.colorPickerEventApi.onBlur = function () {
                ctrl.colorPickerApi.getScope().AngularColorPickerController.update();
            };
        };

        ctrl.getSettings = function (template) {
            $http.get('settings/getMobileTemplateSettings', { params: { template: template } }).then(function (response) {
                var data = response.data;

                ctrl.templateSettings = data;
            });
        };

        ctrl.changeTemplate = function () {
            ctrl.getSettings(ctrl.mobileTemlate);
        };

        ctrl.onChangeMobileAppActiveStateOffOn = function (checked) {
            ctrl.MobileAppActive = checked;
        };

        ctrl.onChangeMobileAppShowBadgesStateOffOn = function (checked) {
            ctrl.MobileAppShowBadges = checked;
        };
    };

    SettingsMobileCtrl.$inject = ['$http', 'toaster', '$location'];

    ng.module('settingsMobile', []).controller('SettingsMobileCtrl', SettingsMobileCtrl);
})(window.angular);
