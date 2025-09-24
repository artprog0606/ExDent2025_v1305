/* @ngInject */
function orderProductService($http) {
    var service = this;

    service.getOrderProducts = function () {
        return $http.get('/myaccount/GetCustomerOrderProductHistory', { params: { rnd: Math.random() } }).then(function (response) {
            return response.data;
        });
    };
}

export default orderProductService;
