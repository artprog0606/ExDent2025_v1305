import addEditUserTemplate from './../../settings/modal/addEditUser/AddEditUser.html';
(function (ng) {
    'use strict';

    var topPanelUserCtrl = function ($location, $uibModal) {
        var ctrl = this;
        ctrl.$onInit = function () {
            if ($location.search() != null && $location.search().user == 'me') {
                ctrl.loadCurrentUser();
            }
        };
        ctrl.loadCurrentUser = function () {
            $uibModal
                .open({
                    bindToController: true,
                    controller: 'ModalAddEditUserCtrl',
                    controllerAs: 'ctrl',
                    templateUrl: addEditUserTemplate,
                    resolve: {
                        params: {
                            customerId: 'me',
                        },
                    },
                })
                .result.then(
                    function (result) {
                        ctrl.userName = result.FirstName + ' ' + result.LastName;
                        ctrl.avatarSrc =
                            '../' +
                            (result.Avatar != null && result.Avatar.length > 0
                                ? 'pictures/avatar/' + result.Avatar
                                : 'areas/admin/content/images/no-photo_v2.png');
                        $location.search('user', null);
                        return result;
                    },
                    function (result) {
                        $location.search('user', null);
                        return result;
                    },
                );
        };
    };
    topPanelUserCtrl.$inject = ['$location', '$uibModal'];
    ng.module('topPanelUser', ['uiModal']).controller('topPanelUserCtrl', topPanelUserCtrl);
})(window.angular);
