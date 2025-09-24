(function (ng) {
    'use strict';

    var sidebarUserService = function ($http, $uibModal) {
        var service = this;

        service.getUser = function (customerId) {
            return $http.get('account/getuserinfo', { params: { customerId: customerId } }).then(function (response) {
                return response.data.obj;
            });
        };

        service.addUser = function (user) {
            return $uibModal.open({
                component: 'sidebarUser',
                controllerAs: '$ctrl',
                openedClass: 'modal-open',
                windowClass: 'sidebar-user-window',
                resolve: { params: { user: user } },
                backdrop: true,
            }).result;
        };
    };

    sidebarUserService.$inject = ['$http', '$uibModal'];

    ng.module('sidebarUser').service('sidebarUserService', sidebarUserService);
})(window.angular);
