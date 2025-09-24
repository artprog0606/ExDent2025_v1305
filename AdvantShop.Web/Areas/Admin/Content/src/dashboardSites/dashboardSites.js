(function (ng) {
    'use strict';

    var DashboardSitesCtrl = function ($uibModal, $http, $sce, SweetAlert, toaster) {
        var ctrl = this;

        ctrl.$onInit = function () {
            ctrl.initScaleIframe = [];
            ctrl.isLoadedDashboard = false;
            ctrl.siteIframes = {};
            ctrl.getDashBoard();
        };

        ctrl.scrollToActiveElement = function (id, url) {
            ctrl.siteIframes[id] = url;
        };

        ctrl.getUrl = function (url) {
            return url != null ? $sce.trustAsResourceUrl(url) : null;
        };

        ctrl.deleteSite = function (site) {
            SweetAlert.confirm('Вы уверены что хотите удалить сайт?', { title: 'Удаление' }).then(function (result) {
                if (result && !result.dismiss) {
                    $http.post('dashboard/deleteSite', { id: site.Id, type: site.Type }).then(function (response) {
                        toaster.pop('success', '', 'Сайт успешно удален');
                        window.location.reload();
                    });
                }
            });
        };

        ctrl.createScreenShots = function () {
            $http.post('dashboard/createScreenShots').then(function (response) {
                window.location.reload();
            });
        };

        ctrl.getDashBoard = function () {
            $http
                .get('dashboard/getDashBoard')
                .then(function (response) {
                    ctrl.data = response.data;
                })
                .finally(function () {
                    ctrl.isLoadedDashboard = true;
                });
        };

        ctrl.changeMainUrl = function (item) {
            $http.post('dashboard/changeMainUrl', { url: item.Url }).then(function (response) {
                if (response.data.result) {
                    ctrl.data.SelectedDomain = item;
                    ctrl.getDashBoard();
                    toaster.pop('success', '', 'Основной сайт изменен');
                }
            });
        };

        ctrl.changeEnabled = function (site) {
            $http.post('dashboard/changeEnabled', { id: site.Id, type: site.Type, enabled: !site.Published }).then(function (response) {
                if (response.data.result) {
                    site.Published = !site.Published;
                    toaster.pop('success', '', 'Изменения успешно сохранены');
                }
            });
        };

        ctrl.scaleIframeDashboardSites = function (item) {
            if (window.matchMedia('(max-width: 1170px)').matches == true) {
                return { transform: `scale(${((window.innerWidth - 32) / 1170).toFixed(2)})` };
            }
            return {};
        };
    };

    DashboardSitesCtrl.$inject = ['$uibModal', '$http', '$sce', 'SweetAlert', 'toaster'];

    ng.module('dashboardSites', ['changeAdminShopName', 'uiModal']).controller('DashboardSitesCtrl', DashboardSitesCtrl);
})(window.angular);
