import addEditStockLabelTemplate from './modal/AddEditStockLabel/addEditStockLabel.html';
/* @ngInject */
export default function stockLabelService($http, $uibModal) {
    var service = this;
    service.getStockLabel = function (id) {
        return $http
            .post('warehouse/getStockLabel', {
                id: id,
            })
            .then(function (response) {
                return response.data;
            });
    };
    service.addStockLabel = function (params) {
        return $http.post('warehouse/addStockLabel', params).then(function (response) {
            return response.data;
        });
    };
    service.updateStockLabel = function (params) {
        return $http.post('warehouse/updateStockLabel', params).then(function (response) {
            return response.data;
        });
    };
    service.showStockLabel = function (id) {
        return $uibModal.open({
            bindToController: true,
            controller: 'ModalAddEditStockLabelCtrl',
            controllerAs: 'ctrl',
            templateUrl: addEditStockLabelTemplate,
            resolve: {
                stockLabelId: function () {
                    return id;
                },
            },
            backdrop: 'static',
        });
    };
}
