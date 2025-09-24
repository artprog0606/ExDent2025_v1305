(function (ng) {
    'use strict';

    var NewsCtrl = function ($http) {
        var ctrl = this;

        ctrl.$onInit = function () {
            ctrl.page = 0;
            ctrl.newsData = [];
            ctrl.inProgress = false;
        };

        ctrl.getItems = function (blockId, take, skip) {
            ctrl.inProgress = true;
            ctrl.page += 1;
            return ctrl
                .fetchData(blockId, take, skip)
                .then(function (data) {
                    ctrl.newsData = ctrl.newsData.concat(data.obj);

                    return data;
                })
                .finally(function () {
                    ctrl.inProgress = false;
                });
        };

        ctrl.fetchData = function (blockId, take, skip) {
            return $http
                .get('landing/landing/GetPagingFromSettingsWithRows', {
                    params: { blockId: blockId, take: take, skip: skip },
                })
                .then(function (response) {
                    return response.data;
                });
        };
    };

    ng.module('news').controller('NewsCtrl', NewsCtrl);

    NewsCtrl.$inject = ['$http'];
})(window.angular);
