(function (ng) {
    'use strict';

    /* @ngInject */
    var SalesChannelsCtrl = function ($window, SweetAlert, $http, toaster) {
        var ctrl = this;

        ctrl.removeChannel = function (type) {
            SweetAlert.confirm('Вы уверены что хотите удалить канал?', { title: 'Удаление' }).then(function (result) {
                if (result && !result.isDismissed) {
                    $http.post('salesChannels/delete', { type: type }).then(function () {
                        toaster.pop('success', '', 'Канал продаж удален');

                        var basePath = document.getElementsByTagName('base')[0].getAttribute('href');
                        $window.location.assign(basePath);
                    });
                }
            });
        };

        ctrl.addSalesChannel = function (type) {
            $http.post('salesChannels/add', { type: type }).then(function (response) {
                if (response != null && response.data.errors != null) {
                    response.data.errors.forEach(function (err) {
                        toaster.pop('error', '', err);
                    });
                } else {
                    $window.location.reload();
                }
            });
        };
    };

    ng.module('salesChannels', []).controller('SalesChannelsCtrl', SalesChannelsCtrl);
})(window.angular);
