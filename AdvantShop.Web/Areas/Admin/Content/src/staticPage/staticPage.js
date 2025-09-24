(function (ng) {
    'use strict';

    var StaticPageCtrl = function ($http, $window, SweetAlert, $translate) {
        var ctrl = this;

        ctrl.deleteStaticPage = function (id) {
            SweetAlert.confirm($translate.instant('Admin.Js.StaticPage.AreYouSureDelete'), {
                title: $translate.instant('Admin.Js.StaticPage.Deleting'),
            }).then(function (result) {
                if (result === true || result.value) {
                    $http.post('StaticPages/DeleteStaticPage', { staticPageId: id }).then(function (response) {
                        $window.location.assign('staticpages');
                    });
                }
            });
        };

        ctrl.changePage = function (result) {
            ctrl.parentId = result.staticPageId;
            ctrl.parentPageName = result.pageName;
        };
    };

    StaticPageCtrl.$inject = ['$http', '$window', 'SweetAlert', '$translate'];

    ng.module('staticPage', ['uiGridCustom', 'urlGenerator']).controller('StaticPageCtrl', StaticPageCtrl);
})(window.angular);
