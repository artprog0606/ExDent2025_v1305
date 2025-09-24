(function (ng) {
    'use strict';

    var ColumnsCtrl = function ($http) {
        var ctrl = this;

        ctrl.$onInit = function () {
            ctrl.page = 0;
            ctrl.columnsData = [];
            ctrl.inProgress = false;
        };

        ctrl.getItems = function (blockId, take, skip, modelName) {
            ctrl.inProgress = true;
            ctrl.page += 1;
            return ctrl
                .fetchData(blockId, take, skip, modelName)
                .then(function (data) {
                    ctrl.columnsData = ctrl.columnsData.concat(data.obj);

                    return data;
                })
                .finally(function () {
                    ctrl.inProgress = false;
                });
        };

        ctrl.fetchData = function (blockId, take, skip, modelName) {
            return $http
                .get('landing/landing/GetPagingFromSettingsWithRows', {
                    params: { blockId: blockId, take: take, skip: skip, modelName: modelName },
                })
                .then(function (response) {
                    return response.data;
                });
        };
    };

    ng.module('columns').controller('ColumnsCtrl', ColumnsCtrl);

    ColumnsCtrl.$inject = ['$http'];
})(window.angular);
