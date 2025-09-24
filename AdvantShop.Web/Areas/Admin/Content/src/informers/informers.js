(function (ng) {
    'use strict';

    var InformersCtrl = function ($http, toaster) {
        var ctrl = this;

        ctrl.goTo = function (path) {
            window.location.assign(path);
        };

        ctrl.getIcon = function (type) {
            var icon_group, icon_class;

            if (['vk', 'facebook', 'instagram', 'telegram', 'ok'].indexOf(type) !== -1) {
                icon_group = 'fab';
            } else {
                icon_group = 'fa';
            }

            if (type == 'email') {
                icon_class = 'envelope';
            } else if (type == 'review') {
                icon_class = 'commenting';
            } else if (type == 'ok') {
                icon_class = 'odnoklassniki';
            } else {
                icon_class = type;
            }

            return [icon_group, 'fa-' + icon_class];
        };

        ctrl.notifyAll = function () {
            $http.post('common/notifyAll').then(function (response) {
                location.reload();
            });
        };
    };

    InformersCtrl.$inject = ['$http', 'toaster'];

    ng.module('informers', []).controller('InformersCtrl', InformersCtrl);
})(window.angular);
