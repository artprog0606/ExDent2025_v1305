import addEditDomainGeoLocationTemplate from './modal/AddEditDomainGeoLocation/addEditDomainGeoLocation.html';
/* @ngInject */
export default function domainGeoLocationService($http, $uibModal) {
    var service = this;
    service.get = function (id) {
        return $http.post('warehouse/getDomainGeoLocation', { id: id }).then(function (response) {
            return response.data;
        });
    };
    service.add = function (params) {
        return $http.post('warehouse/addDomainGeoLocation', params).then(function (response) {
            return response.data;
        });
    };
    service.update = function (params) {
        return $http.post('warehouse/updateDomainGeoLocation', params).then(function (response) {
            return response.data;
        });
    };
    service.showModal = function (id) {
        return $uibModal.open({
            bindToController: true,
            controller: 'ModalAddEditDomainGeoLocationCtrl',
            controllerAs: 'ctrl',
            templateUrl: addEditDomainGeoLocationTemplate,
            resolve: {
                Id: function () {
                    return id;
                },
            },
            backdrop: 'static',
            size: 'middle',
        });
    };
}
