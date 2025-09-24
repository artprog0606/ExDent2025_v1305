(function (ng) {
    'use strict';

    var SettingsSocialCtrl = function ($http, $q, $location, isMobileService) {
        var ctrl = this;

        ctrl.$onInit = function () {
            ctrl.showBlockCustomCode = false;
        };

        ctrl.onSelectTab = function (indexTab) {
            ctrl.tabActiveIndex = indexTab;
        };
    };

    SettingsSocialCtrl.$inject = ['$http', '$q', '$location', 'isMobileService'];

    ng.module('settingsSocial', ['fileUploader', 'pictureUploader', 'isMobile', 'ngCkeditor']).controller('SettingsSocialCtrl', SettingsSocialCtrl);
})(window.angular);
